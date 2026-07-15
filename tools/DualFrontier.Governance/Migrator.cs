using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace DualFrontier.Governance;

/// <summary>
/// The corpus migration (D4). Operates ONLY on an explicit <c>--target</c> scratch
/// copy: it refuses the live repo root unless the mutation flag is passed
/// (fail-closed by construction). It (1) extracts the four global collections from
/// the forward-regime REGISTER.yaml into the four SoT files, (2) injects schema-v2
/// frontmatter into every enrolled .md document (the ratified drops applied,
/// project + first_authored backfilled), (3) carries the non-.md enrolled entries
/// into a provisional supplement (architect-ruling for Cascade B), and (4) leaves
/// the corpus ready for `sync` to regenerate the derived archive. The reconciliation
/// (old vs derived) is computed by <see cref="Reconcile"/>.
/// </summary>
public static class Migrator
{
    private static readonly IDeserializer Deserializer =
        new DeserializerBuilder().IgnoreUnmatchedProperties().Build();

    private static readonly ISerializer Serializer = new SerializerBuilder().Build();

    private static readonly Regex PendingCommit = new(@"^PENDING", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Fields the migration renames, drops, or that are file-location (not frontmatter).
    private static readonly IReadOnlySet<string> NotCarried =
        new HashSet<string> { "id", "path", "register_view_url" };

    // The canonical schema-v2 required order (FRAMEWORK 14.3); other fields follow.
    private static readonly string[] RequiredOrder =
    {
        "register_id", "project", "category", "tier", "lifecycle", "owner",
        "version", "first_authored", "last_modified", "content_language", "next_review_due",
    };

    public sealed record MigrationReport(
        int MarkdownMigrated,
        int NonMarkdownSupplemented,
        int RequirementsExtracted,
        int RisksExtracted,
        int CapaExtracted,
        int AuditTrailExtracted,
        int ProjectBackfilled,
        int FirstAuthoredBackfilled,
        int OrphansExcluded,
        List<string> ArchitectRuling);

    /// <summary>
    /// Runs the migration against <paramref name="targetRoot"/>. Throws if the target
    /// is the live repo root and <paramref name="allowLiveCorpus"/> is false.
    /// </summary>
    public static MigrationReport Migrate(string targetRoot, bool allowLiveCorpus)
    {
        if (!allowLiveCorpus && PathsEqual(targetRoot, SafeRepoRoot()))
        {
            throw new InvalidOperationException(
                "migrate refuses to mutate the live corpus. Pass --target <scratch copy>, "
                + "or --i-understand-this-mutates-the-corpus to override (Cascade B).");
        }

        string governanceDir = RepoPaths.GovernanceDir(targetRoot);
        string registerPath = Path.Combine(governanceDir, "REGISTER.yaml");
        OldRegister old = Deserializer.Deserialize<OldRegister>(File.ReadAllText(registerPath)) ?? new OldRegister();

        ExtractGlobals(governanceDir, old);

        int mdMigrated = 0, projectBackfilled = 0, firstAuthoredBackfilled = 0;
        var nonMarkdown = new List<Dictionary<string, object?>>();
        var architectRuling = new List<string>();
        var enrolledMarkdown = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (Dictionary<string, object?> entry in old.documents ?? new())
        {
            string path = entry.GetValueOrDefault("path")?.ToString() ?? "";
            string id = entry.GetValueOrDefault("id")?.ToString() ?? "(no id)";
            if (id == "DOC-G-REGISTER")
            {
                continue; // regenerated as the deterministic self-entry
            }

            if (!path.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                var carried = new Dictionary<string, object?> { ["register_id"] = id };
                foreach (var kv in entry)
                {
                    if (kv.Key != "id")
                    {
                        carried[kv.Key] = kv.Value;
                    }
                }

                nonMarkdown.Add(carried);
                architectRuling.Add($"{id} ({path}) -- non-.md enrolled entry, cannot carry frontmatter; ruling: exclude / relocate to MODULE.md / formalize supplement");
                continue;
            }

            enrolledMarkdown.Add(path);
            (Dictionary<string, object?> fm, bool pbf, bool fbf) = BuildV2Frontmatter(entry);
            if (pbf) { projectBackfilled++; }
            if (fbf) { firstAuthoredBackfilled++; }

            string absPath = Path.Combine(targetRoot, path.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(absPath))
            {
                InjectFrontmatter(absPath, fm, FrontmatterExtractor.IsReadmeClass(path));
                mdMigrated++;
            }
            else
            {
                architectRuling.Add($"{id} ({path}) -- registered path missing on disk");
            }
        }

        WriteSupplement(governanceDir, nonMarkdown);
        int orphansExcluded = ExcludeOrphans(targetRoot, enrolledMarkdown);

        return new MigrationReport(
            mdMigrated, nonMarkdown.Count,
            (old.requirements ?? new()).Count, (old.risks ?? new()).Count,
            (old.capa_entries ?? new()).Count, (old.audit_trail ?? new()).Count,
            projectBackfilled, firstAuthoredBackfilled, orphansExcluded, architectRuling);
    }

    /// <summary>
    /// Adds every in-scope orphan (an in-scope .md not enrolled in the old register)
    /// to the scratch copy's exclusion config, so `sync` over the migrated copy is
    /// not blocked by un-migrated orphans. This isolates the 288 round-trip; the
    /// orphan triage (enroll / exclude / architect-ruling) is Cascade B work
    /// recorded in the measure report (brief 2 / R4.2).
    /// </summary>
    private static int ExcludeOrphans(string targetRoot, HashSet<string> enrolledMarkdown)
    {
        var exclusions = ExclusionConfig.Load(RepoPaths.ExclusionsPath(targetRoot));
        var orphans = CorpusWalker.MarkdownRelPaths(targetRoot)
            .Where(rel => !exclusions.IsExcluded(rel) && !enrolledMarkdown.Contains(rel))
            .ToList();

        if (orphans.Count == 0)
        {
            return 0;
        }

        string exclusionsPath = RepoPaths.ExclusionsPath(targetRoot);
        Dictionary<string, object?> config = File.Exists(exclusionsPath)
            ? Deserializer.Deserialize<Dictionary<string, object?>>(File.ReadAllText(exclusionsPath)) ?? new()
            : new Dictionary<string, object?> { ["included_extensions"] = new List<object> { ".md" } };
        Directory.CreateDirectory(Path.GetDirectoryName(exclusionsPath)!);
        var excluded = config.GetValueOrDefault("excluded_paths") as List<object> ?? new List<object>();
        foreach (string orphan in orphans)
        {
            excluded.Add(new Dictionary<string, object?>
            {
                ["pattern"] = orphan,
                ["rationale"] = "Cascade A dry-run: orphan excluded to isolate the 288 round-trip (triage is Cascade B)",
            });
        }

        config["excluded_paths"] = excluded;
        File.WriteAllText(exclusionsPath, Serializer.Serialize(config).Replace("\r\n", "\n"));
        return orphans.Count;
    }

    /// <summary>Builds the schema-v2 frontmatter for one old entry: rename id, backfill project/first_authored, drop the ratified fields, carry the rest.</summary>
    public static (Dictionary<string, object?> Frontmatter, bool ProjectBackfilled, bool FirstAuthoredBackfilled)
        BuildV2Frontmatter(Dictionary<string, object?> old)
    {
        var fm = new Dictionary<string, object?>();
        fm["register_id"] = old.GetValueOrDefault("id");

        bool projectBackfilled = false;
        if (old.GetValueOrDefault("project") is { } project)
        {
            fm["project"] = project;
        }
        else
        {
            fm["project"] = "Dual Frontier";
            projectBackfilled = true;
        }

        foreach (string key in new[] { "category", "tier", "lifecycle", "owner", "version" })
        {
            if (old.TryGetValue(key, out object? value))
            {
                fm[key] = value;
            }
        }

        bool firstAuthoredBackfilled = false;
        if (old.GetValueOrDefault("first_authored") is { } firstAuthored)
        {
            fm["first_authored"] = firstAuthored;
        }
        else
        {
            fm["first_authored"] = old.GetValueOrDefault("last_modified") ?? "unknown";
            firstAuthoredBackfilled = true;
        }

        if (old.TryGetValue("last_modified", out object? lastModified)) { fm["last_modified"] = lastModified; }
        if (old.TryGetValue("content_language", out object? lang)) { fm["content_language"] = lang; }
        fm["next_review_due"] = old.GetValueOrDefault("next_review_due") ?? "null";

        foreach (var kv in old)
        {
            if (fm.ContainsKey(kv.Key) || NotCarried.Contains(kv.Key))
            {
                continue;
            }

            if (kv.Key == "last_modified_commit" && kv.Value?.ToString() is { } commit && PendingCommit.IsMatch(commit))
            {
                continue; // ratified drop: PENDING-* placeholder
            }

            fm[kv.Key] = kv.Value;
        }

        return (fm, projectBackfilled, firstAuthoredBackfilled);
    }

    /// <summary>
    /// Reconciles the derived documents against the old register: matches by id,
    /// reports lost / added ids and per-field deltas beyond the sanctioned drops
    /// (register_view_url, PENDING last_modified_commit) and required backfills
    /// (project, first_authored). Non-.md ids are compared via the supplement.
    /// </summary>
    public static List<string> Reconcile(
        List<Dictionary<string, object?>> oldDocs, List<Dictionary<string, object?>> derivedDocs)
    {
        var deltas = new List<string>();
        var oldById = oldDocs.Where(d => d.GetValueOrDefault("id") is not null)
            .ToDictionary(d => d["id"]!.ToString()!, d => d);
        var newById = derivedDocs.Where(d => d.GetValueOrDefault("register_id") is not null)
            .ToDictionary(d => d["register_id"]!.ToString()!, d => d);

        foreach (string id in oldById.Keys)
        {
            if (id == "DOC-G-REGISTER")
            {
                continue; // deterministic self-entry
            }

            if (!newById.ContainsKey(id))
            {
                deltas.Add($"LOST: {id} present in old register, absent from derived");
            }
        }

        foreach (string id in newById.Keys)
        {
            if (id == "DOC-G-REGISTER")
            {
                continue;
            }

            if (!oldById.ContainsKey(id))
            {
                deltas.Add($"ADDED: {id} in derived but not in old register");
                continue;
            }

            deltas.AddRange(FieldDeltas(id, oldById[id], newById[id]));
        }

        return deltas;
    }

    private static IEnumerable<string> FieldDeltas(string id, Dictionary<string, object?> old, Dictionary<string, object?> derived)
    {
        foreach (var kv in old)
        {
            if (kv.Key is "id" or "path")
            {
                continue; // id -> register_id rename; path re-derived from file location
            }

            if (kv.Key == "register_view_url")
            {
                continue; // ratified drop
            }

            if (kv.Key == "last_modified_commit" && kv.Value?.ToString() is { } commit && PendingCommit.IsMatch(commit))
            {
                continue; // ratified drop
            }

            // Normalize C# null and the string "null" to the same token: the
            // next_review_due sentinel 'null' (FRAMEWORK 14.4) round-trips through
            // YAML as bare null, which is the same "no scheduled review" value.
            string oldValue = kv.Value?.ToString() ?? "null";
            string newValue = derived.GetValueOrDefault(kv.Key)?.ToString() ?? "null";
            if (oldValue != newValue)
            {
                yield return $"DELTA {id}.{kv.Key}: old='{Trim(oldValue)}' derived='{Trim(newValue)}'";
            }
        }
    }

    private static string Trim(string? s) => s is null ? "(absent)" : s.Length > 60 ? s[..60] + "..." : s;

    private static void ExtractGlobals(string governanceDir, OldRegister old)
    {
        WriteCollection(Path.Combine(governanceDir, "REQUIREMENTS.yaml"), "requirements", old.requirements);
        WriteCollection(Path.Combine(governanceDir, "RISKS.yaml"), "risks", old.risks);
        WriteCollection(Path.Combine(governanceDir, "CAPA.yaml"), "capa_entries", old.capa_entries);
        WriteCollection(Path.Combine(governanceDir, "AUDIT_TRAIL.yaml"), "audit_trail", old.audit_trail);
    }

    private static void WriteCollection(string path, string key, List<Dictionary<string, object?>>? entries)
    {
        var wrapper = new Dictionary<string, object?> { [key] = entries ?? new() };
        string header =
            $"# {key} -- hand-edited source of truth (FRAMEWORK 14.6). Validated by the\n" +
            "# governance tool and merged into the derived REGISTER.yaml archive.\n";
        File.WriteAllText(path, header + Serializer.Serialize(wrapper).Replace("\r\n", "\n"));
    }

    private static void WriteSupplement(string governanceDir, List<Dictionary<string, object?>> nonMarkdown)
    {
        var wrapper = new Dictionary<string, object?> { ["documents"] = nonMarkdown };
        string header =
            "# REGISTER_SUPPLEMENT -- PROVISIONAL. Enrolled artifacts that cannot carry\n" +
            "# .md frontmatter (non-.md governed docs). Architect-ruling for Cascade B\n" +
            "# (exclude / relocate to MODULE.md / formalize). Merged into the derived archive by sync.\n";
        File.WriteAllText(Path.Combine(governanceDir, "REGISTER_SUPPLEMENT.yaml"),
            header + Serializer.Serialize(wrapper).Replace("\r\n", "\n"));
    }

    private static void InjectFrontmatter(string absPath, Dictionary<string, object?> fm, bool isReadme)
    {
        string body = StripRegisterFrontmatter(File.ReadAllText(absPath), isReadme);
        string block = "---\n" + Serializer.Serialize(fm).Replace("\r\n", "\n") + "---\n";

        string result = isReadme
            ? body.TrimEnd('\r', '\n', ' ', '\t') + "\n\n" + block
            : block + "\n" + body;

        File.WriteAllText(absPath, result);
    }

    private static string StripRegisterFrontmatter(string text, bool isReadme)
    {
        var lines = text.Replace("\r\n", "\n").TrimStart('﻿').Split('\n').ToList();
        (int start, int end) = isReadme ? FindEndBlock(lines) : FindStartBlock(lines);
        if (start < 0)
        {
            return string.Join("\n", lines);
        }

        // Only strip a register-generated / register block (identified by register_id).
        string block = string.Join("\n", lines.GetRange(start, end - start + 1));
        if (!block.Contains("register_id"))
        {
            return string.Join("\n", lines);
        }

        lines.RemoveRange(start, end - start + 1);
        return string.Join("\n", lines).Trim('\n');
    }

    private static (int Start, int End) FindStartBlock(List<string> lines)
    {
        int start = -1;
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Trim().Length == 0) { continue; }
            if (lines[i].Trim() == "---") { start = i; }
            break;
        }

        if (start == -1) { return (-1, -1); }
        for (int i = start + 1; i < lines.Count; i++)
        {
            if (lines[i].Trim() == "---") { return (start, i); }
        }

        return (-1, -1);
    }

    private static (int Start, int End) FindEndBlock(List<string> lines)
    {
        int end = -1;
        for (int i = lines.Count - 1; i >= 0; i--)
        {
            if (lines[i].Trim().Length == 0) { continue; }
            if (lines[i].Trim() == "---") { end = i; }
            break;
        }

        if (end == -1) { return (-1, -1); }
        for (int i = end - 1; i >= 0; i--)
        {
            if (lines[i].Trim() == "---") { return (i, end); }
        }

        return (-1, -1);
    }

    private static string SafeRepoRoot()
    {
        try { return RepoPaths.RepoRoot(); }
        catch (InvalidOperationException) { return "\0"; }
    }

    private static bool PathsEqual(string a, string b) =>
        string.Equals(Path.GetFullPath(a).TrimEnd(Path.DirectorySeparatorChar),
            Path.GetFullPath(b).TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase);

    /// <summary>Deserialization shape for the forward-regime REGISTER.yaml.</summary>
    public sealed class OldRegister
    {
        public List<Dictionary<string, object?>>? documents { get; set; }
        public List<Dictionary<string, object?>>? requirements { get; set; }
        public List<Dictionary<string, object?>>? risks { get; set; }
        public List<Dictionary<string, object?>>? capa_entries { get; set; }
        public List<Dictionary<string, object?>>? audit_trail { get; set; }
    }
}
