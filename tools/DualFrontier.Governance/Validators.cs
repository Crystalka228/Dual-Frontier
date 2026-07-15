using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace DualFrontier.Governance;

/// <summary>
/// The pure validation core: schema constants, the machine-gate catalog
/// (FRAMEWORK 14.5), cross-reference resolution, and the derived authority
/// surface. No I/O, no process, no exit -- consumed identically by the CLI and
/// the xUnit suite. Every rule mirrors a FRAMEWORK section and cites it; the tool
/// never owns the law (FRAMEWORK 14.2).
/// </summary>
public static class Validators
{
    // --- Schema v2 (FRAMEWORK 14.3) ---

    public static readonly IReadOnlyList<string> RequiredFields = new[]
    {
        "register_id", "project", "category", "tier", "lifecycle", "owner",
        "version", "first_authored", "last_modified", "content_language", "next_review_due",
    };

    public static readonly IReadOnlySet<string> ValidCategories =
        new HashSet<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

    public static readonly IReadOnlySet<int> ValidTiers = new HashSet<int> { 1, 2, 3, 4, 5 };

    public static readonly IReadOnlySet<string> ValidLifecycles = new HashSet<string>
    {
        "Draft", "Live", "LOCKED", "EXECUTED", "AUTHORED",
        "AUTHORED-SKELETON", "DEPRECATED", "SUPERSEDED", "STALE",
    };

    public static readonly IReadOnlySet<string> ValidContentLanguages =
        new HashSet<string> { "en", "ru", "mixed" };

    /// <summary>Terminal lifecycles for the next_review_due 'null' rule (FRAMEWORK 14.4).</summary>
    public static readonly IReadOnlySet<string> TerminalLifecycles =
        new HashSet<string> { "EXECUTED", "DEPRECATED", "SUPERSEDED" };

    // --- Globals enums (FRAMEWORK 4.3-4.7) for G-GLOBALS ---

    public static readonly IReadOnlySet<string> ValidVerificationStatus =
        new HashSet<string> { "PENDING", "PARTIAL", "VERIFIED", "FAILED" };

    public static readonly IReadOnlySet<string> ValidLikelihood =
        new HashSet<string> { "Low", "Medium-Low", "Medium", "Medium-High", "High" };

    public static readonly IReadOnlySet<string> ValidImpact =
        new HashSet<string> { "Low", "Medium", "High", "Critical" };

    public static readonly IReadOnlySet<string> ValidRiskType =
        new HashSet<string> { "Technical", "Architectural", "Methodological", "Operational", "External" };

    public static readonly IReadOnlySet<string> ValidRiskStatus =
        new HashSet<string> { "ACTIVE", "RESIDUAL", "CLOSED", "REALIZED", "ACCEPTED" };

    public static readonly IReadOnlySet<string> ValidCapaStatus =
        new HashSet<string> { "OPEN", "CLOSED" };

    public static readonly IReadOnlySet<string> ValidEventType = new HashSet<string>
    {
        "deliberation_milestone", "execution_milestone", "amendment_landing",
        "lifecycle_transition", "governance_event",
    };

    // Forbidden category x tier (FRAMEWORK 3.4.1), grounded from the corpus law.
    private static readonly IReadOnlyDictionary<string, int[]> ForbiddenCategoryTier =
        new Dictionary<string, int[]>
        {
            ["D"] = new[] { 1, 2, 4, 5 },
            ["E"] = new[] { 1, 2, 4, 5 },
            ["F"] = new[] { 1, 2, 3, 5 },
        };

    // Forbidden tier x lifecycle (FRAMEWORK 3.4.1), grounded from the corpus law.
    private static readonly IReadOnlySet<string> ForbiddenTierLifecycle = new HashSet<string>
    {
        "1+AUTHORED", "1+AUTHORED-SKELETON", "2+AUTHORED-SKELETON", "3+LOCKED",
        "4+LOCKED", "4+AUTHORED-SKELETON", "5+STALE", "5+AUTHORED-SKELETON",
    };

    private static readonly Regex RegisterIdPattern = new(@"^DOC-[A-J]-[A-Z0-9_]+$", RegexOptions.Compiled);
    private static readonly Regex IsoDate = new(@"^\d{4}-\d{2}-\d{2}$", RegexOptions.Compiled);
    private static readonly Regex Quarter = new(@"^\d{4}-Q[1-4]$", RegexOptions.Compiled);
    private static readonly Regex PostClosure = new(@"^post-.+ closure$", RegexOptions.Compiled);
    private static readonly Regex PendingCommit = new(@"^PENDING", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static Finding Error(string gate, Frontmatter fm, string message) =>
        new(gate, Severity.Error, fm.Subject, message);

    private static Finding Report(string gate, Frontmatter fm, string message) =>
        new(gate, Severity.Report, fm.Subject, message);

    // --- G-SCHEMA (always Error) ---

    /// <summary>Required fields, enum membership, id format, and the outlawed PENDING-* commit placeholder.</summary>
    public static IEnumerable<Finding> ValidateSchema(Frontmatter fm)
    {
        foreach (string field in RequiredFields)
        {
            if (!fm.ContainsKey(field))
            {
                yield return Error("G-SCHEMA", fm, $"missing required field \"{field}\"");
            }
        }

        if (fm.Category is { } cat && !ValidCategories.Contains(cat))
        {
            yield return Error("G-SCHEMA", fm, $"invalid category \"{cat}\" (expected A..J)");
        }

        if (fm.Tier is { } tier && !ValidTiers.Contains(tier))
        {
            yield return Error("G-SCHEMA", fm, $"invalid tier {tier} (expected 1..5)");
        }

        if (fm.Lifecycle is { } lifecycle && !ValidLifecycles.Contains(lifecycle))
        {
            yield return Error("G-SCHEMA", fm, $"invalid lifecycle \"{lifecycle}\"");
        }

        if (fm.ContentLanguage is { } lang && !ValidContentLanguages.Contains(lang))
        {
            yield return Error("G-SCHEMA", fm, $"invalid content_language \"{lang}\" (expected en|ru|mixed)");
        }

        if (fm.RegisterId is { } id && !RegisterIdPattern.IsMatch(id))
        {
            yield return Error("G-SCHEMA", fm, $"register_id \"{id}\" does not match DOC-<A..J>-<NAME>");
        }

        if (fm.LastModifiedCommit is { } commit && PendingCommit.IsMatch(commit))
        {
            yield return Error("G-SCHEMA", fm,
                $"last_modified_commit \"{commit}\" uses the outlawed PENDING-* placeholder (FRAMEWORK 14.3)");
        }
    }

    // --- Semantic gates, per document (Report until armed) ---

    /// <summary>G-CATLIFE -- the FRAMEWORK 3.4.1 forbidden category x tier and tier x lifecycle combinations.</summary>
    public static IEnumerable<Finding> Gate1CategoryLifecycle(Frontmatter fm)
    {
        if (fm.HasSanctionedDeviation)
        {
            yield break; // FRAMEWORK 3.4.2 sanctioned deviation
        }

        if (fm.Category is { } cat && fm.Tier is { } tier &&
            ForbiddenCategoryTier.TryGetValue(cat, out int[]? tiers) && tiers.Contains(tier))
        {
            yield return Report("G-CATLIFE", fm,
                $"forbidden category={cat} + tier={tier} without special_case_rationale (FRAMEWORK 3.4.1)");
        }

        if (fm.Tier is { } t && fm.Lifecycle is { } lc && ForbiddenTierLifecycle.Contains($"{t}+{lc}"))
        {
            yield return Report("G-CATLIFE", fm,
                $"forbidden tier={t} + lifecycle={lc} without special_case_rationale (FRAMEWORK 3.4.1)");
        }
    }

    /// <summary>G-NAMESPACE -- the id prefix letter equals the document's own category (FRAMEWORK 5, 3.1).</summary>
    public static IEnumerable<Finding> Gate3Namespace(Frontmatter fm)
    {
        if (fm.RegisterId is { } id && fm.Category is { } cat &&
            !id.StartsWith($"DOC-{cat}-", StringComparison.Ordinal))
        {
            yield return Report("G-NAMESPACE", fm, $"register_id does not start with DOC-{cat}- (category {cat})");
        }
    }

    /// <summary>G-TERMINAL (per-doc half) -- a terminal lifecycle carries next_review_due 'null' (FRAMEWORK 14.4).</summary>
    public static IEnumerable<Finding> Gate4TerminalNull(Frontmatter fm)
    {
        if (fm.Lifecycle is { } lc && TerminalLifecycles.Contains(lc) && fm.NextReviewDue != "null")
        {
            yield return Report("G-TERMINAL", fm,
                $"terminal lifecycle {lc} must carry next_review_due 'null' (got '{fm.NextReviewDue}') (FRAMEWORK 14.4)");
        }
    }

    /// <summary>G-SENTINEL -- next_review_due is one of the four sanctioned forms (FRAMEWORK 14.4).</summary>
    public static IEnumerable<Finding> Gate5Sentinel(Frontmatter fm)
    {
        string v = fm.NextReviewDue;
        bool ok = v == "null" || IsoDate.IsMatch(v) || Quarter.IsMatch(v) || PostClosure.IsMatch(v);
        if (!ok)
        {
            yield return Report("G-SENTINEL", fm,
                $"next_review_due '{v}' is not in the sanctioned set ('null'|YYYY-MM-DD|YYYY-QN|'post-<event> closure') (FRAMEWORK 14.4)");
        }
    }

    /// <summary>All per-document semantic gates (Report severity).</summary>
    public static IEnumerable<Finding> PerDocumentSemanticGates(Frontmatter fm) =>
        Gate1CategoryLifecycle(fm)
            .Concat(Gate3Namespace(fm))
            .Concat(Gate4TerminalNull(fm))
            .Concat(Gate5Sentinel(fm));

    // --- Cross-document gates ---

    /// <summary>G-UNIQUE -- register_id is unique across the corpus (Error).</summary>
    public static IEnumerable<Finding> DuplicateIdFindings(IReadOnlyList<Frontmatter> docs)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (Frontmatter doc in docs)
        {
            if (doc.RegisterId is { } id && !seen.Add(id))
            {
                yield return new Finding("G-UNIQUE", Severity.Error, id, $"duplicate register_id \"{id}\"");
            }
        }
    }

    /// <summary>
    /// G-XREF + G-TERMINAL (cross-ref half). Every supersedes / superseded_by /
    /// deprecated_by names an enrolled id; a supersedes[T] requires T.superseded_by
    /// to RESOLVE (the authorizing-record semantic -- successor OR enrolled cascade
    /// brief -- FRAMEWORK 3.3.2), not to equal the asserting id.
    /// </summary>
    public static IEnumerable<Finding> CrossReferenceFindings(IReadOnlyList<Frontmatter> docs)
    {
        var byId = new Dictionary<string, Frontmatter>(StringComparer.Ordinal);
        foreach (Frontmatter doc in docs)
        {
            if (doc.RegisterId is { } id)
            {
                byId[id] = doc;
            }
        }

        foreach (Frontmatter doc in docs)
        {
            foreach (string target in doc.Supersedes)
            {
                if (!byId.TryGetValue(target, out Frontmatter? other))
                {
                    yield return Report("G-XREF", doc, $"supersedes \"{target}\" which is not enrolled");
                    continue;
                }

                if (other.SupersededBy is not { } sb || !byId.ContainsKey(sb))
                {
                    yield return Report("G-XREF", doc,
                        $"supersedes \"{target}\" but \"{target}\" has no resolving superseded_by pointer (FRAMEWORK 3.3.2)");
                }
            }

            if (doc.SupersededBy is { } supersededBy && !byId.ContainsKey(supersededBy))
            {
                yield return Report("G-XREF", doc, $"superseded_by \"{supersededBy}\" which is not enrolled");
            }

            if (doc.DeprecatedBy is { } deprecatedBy && !byId.ContainsKey(deprecatedBy))
            {
                yield return Report("G-XREF", doc, $"deprecated_by \"{deprecatedBy}\" which is not enrolled");
            }

            if (doc.Lifecycle == "SUPERSEDED" && (doc.SupersededBy is not { } s || !byId.ContainsKey(s)))
            {
                yield return Report("G-TERMINAL", doc,
                    "lifecycle SUPERSEDED requires superseded_by to resolve to an enrolled id (FRAMEWORK 3.3.2, 14.4)");
            }

            if (doc.Lifecycle == "DEPRECATED" && (doc.DeprecatedBy is not { } d || !byId.ContainsKey(d)))
            {
                yield return Report("G-TERMINAL", doc,
                    "lifecycle DEPRECATED requires deprecated_by to resolve to an enrolled id (FRAMEWORK 3.3.2, 14.4)");
            }
        }
    }

    /// <summary>G-RATIO -- the special_case_rationale override share (Monitor; FRAMEWORK 10 #5).</summary>
    public static Finding RationaleRatio(IReadOnlyList<Frontmatter> docs)
    {
        int overrides = docs.Count(d => d.HasSanctionedDeviation);
        int total = docs.Count;
        double pct = total == 0 ? 0 : 100.0 * overrides / total;
        return new Finding("G-RATIO", Severity.Monitor, "(corpus)",
            $"special_case_rationale overrides: {overrides}/{total} ({pct:F1}%); FRAMEWORK 10 #5 threshold is 20%");
    }

    /// <summary>Schema + all gates over a whole corpus (does not include the no-frontmatter / parse errors, which the walker emits).</summary>
    public static IReadOnlyList<Finding> RunAll(IReadOnlyList<Frontmatter> docs)
    {
        var findings = new List<Finding>();
        foreach (Frontmatter doc in docs)
        {
            findings.AddRange(ValidateSchema(doc));
            findings.AddRange(PerDocumentSemanticGates(doc));
        }

        findings.AddRange(DuplicateIdFindings(docs));
        findings.AddRange(CrossReferenceFindings(docs));
        findings.Add(RationaleRatio(docs));
        return findings;
    }

    // --- Authority surface (FRAMEWORK 14.7) ---

    /// <summary>The DF boot subset predicate: Live, OR LOCKED of tier 1 or 2.</summary>
    public static IEnumerable<Frontmatter> SelectAuthoritySurface(IReadOnlyList<Frontmatter> docs) =>
        docs.Where(d => d.Lifecycle == "Live" || (d.Lifecycle == "LOCKED" && d.Tier is 1 or 2));

    private const string AuthorityHeader =
        "# DualFrontier -- Current Authority Surface (DERIVED)\n" +
        "# DERIVED, never hand-edited. Regenerated by tools/DualFrontier.Governance on\n" +
        "# every sync from the corpus, per FRAMEWORK 14.7. The boot subset: Live documents\n" +
        "# plus LOCKED tier-1/2 masters. The full docs/governance/REGISTER.yaml remains the\n" +
        "# audit corpus.\n";

    /// <summary>
    /// Renders CURRENT_AUTHORITY_SURFACE.yaml: the boot subset sorted by
    /// register_id, a stable field set, LF-normalized, under a generated header.
    /// Deterministic (no volatile timestamp) so an unchanged corpus is byte-stable.
    /// </summary>
    public static string RenderAuthoritySurface(IReadOnlyList<Frontmatter> docs)
    {
        var surface = new AuthoritySurfaceFile
        {
            documents = SelectAuthoritySurface(docs)
                .OrderBy(d => d.RegisterId, StringComparer.Ordinal)
                .Select(d => new AuthorityDoc
                {
                    register_id = d.RegisterId ?? string.Empty,
                    path = d.RelPath,
                    category = d.Category ?? string.Empty,
                    tier = d.Tier ?? 0,
                    lifecycle = d.Lifecycle ?? string.Empty,
                    version = d.Version ?? string.Empty,
                })
                .ToList(),
        };

        var serializer = new SerializerBuilder().Build();
        string body = serializer.Serialize(surface).Replace("\r\n", "\n");
        return AuthorityHeader + body;
    }

    private const string ArchiveHeader =
        "# DualFrontier Document Control Register (DERIVED ARCHIVE)\n" +
        "# Generated by tools/DualFrontier.Governance from per-document frontmatter\n" +
        "# (FRAMEWORK 14.1). Source of truth: the frontmatter in each document file.\n" +
        "# Re-run `sync` after editing any document's frontmatter. Never hand-edited.\n";

    /// <summary>
    /// Renders the derived archive REGISTER.yaml: every document as an entry
    /// (register_id + path first, then its frontmatter fields), plus the
    /// deterministic DOC-G-REGISTER self-entry, sorted by register_id, with the four
    /// global collections merged in. LF-normalized and free of volatile timestamps,
    /// so an unchanged corpus is byte-reproducible (the derived-register integrity
    /// invariant, FRAMEWORK 14.1).
    /// </summary>
    public static string RenderArchive(
        IReadOnlyList<Frontmatter> docs, GlobalsCollections globals, string registerVersion)
    {
        var entries = new List<Dictionary<string, object?>>();
        foreach (Frontmatter fm in docs)
        {
            var entry = new Dictionary<string, object?>
            {
                ["register_id"] = fm.RegisterId,
                ["path"] = fm.RelPath,
            };
            foreach (var kv in fm.Fields)
            {
                if (kv.Key != "register_id")
                {
                    entry[kv.Key] = kv.Value;
                }
            }

            entries.Add(entry);
        }

        entries.Add(RegisterSelfEntry());
        entries = entries.OrderBy(e => e["register_id"]?.ToString(), StringComparer.Ordinal).ToList();

        var archive = new Dictionary<string, object?>
        {
            ["schema_version"] = "2.0",
            ["register_version"] = registerVersion,
            ["documents"] = entries,
            ["requirements"] = globals.Requirements,
            ["risks"] = globals.Risks,
            ["capa_entries"] = globals.Capa,
            ["audit_trail"] = globals.AuditTrail,
        };

        var serializer = new SerializerBuilder().Build();
        return ArchiveHeader + serializer.Serialize(archive).Replace("\r\n", "\n");
    }

    /// <summary>
    /// The DOC-G-REGISTER self-entry, emitted deterministically every sync
    /// (FRAMEWORK 8.3 / 14.9). No volatile field, so the archive stays
    /// byte-reproducible; the meta_role reflects the inverted derived-archive role.
    /// </summary>
    private static Dictionary<string, object?> RegisterSelfEntry() => new()
    {
        ["register_id"] = "DOC-G-REGISTER",
        ["path"] = "docs/governance/REGISTER.yaml",
        ["project"] = "Dual Frontier",
        ["category"] = "G",
        ["tier"] = 2,
        ["lifecycle"] = "Live",
        ["is_meta_entry"] = true,
        ["meta_role"] = "register_derived_archive",
        ["owner"] = "Crystalka",
        ["version"] = "Live",
        ["first_authored"] = "2026-05-12",
        ["last_modified"] = "2026-07-15",
        ["content_language"] = "en",
        ["next_review_due"] = "post-Cascade B closure",
    };

    private sealed class AuthoritySurfaceFile
    {
        public string schema_version { get; set; } = "2.0";
        public List<AuthorityDoc> documents { get; set; } = new();
    }

    private sealed class AuthorityDoc
    {
        public string register_id { get; set; } = string.Empty;
        public string path { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public int tier { get; set; }
        public string lifecycle { get; set; } = string.Empty;
        public string version { get; set; } = string.Empty;
    }
}
