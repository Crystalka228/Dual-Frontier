namespace DualFrontier.Governance;

/// <summary>
/// The shared orchestration for <c>validate</c> and <c>sync</c>: walk the corpus,
/// extract + parse frontmatter (no-frontmatter outside exclusions = ERROR), run
/// the schema + gate catalog + globals validation, and -- for sync -- regenerate
/// the two derived artifacts idempotently. Semantic gates are REPORT-ONLY unless
/// <c>armed</c> (FRAMEWORK 14.8); schema / parse / no-frontmatter / duplicate-id
/// errors are always exit-affecting.
/// </summary>
public static class RegisterSync
{
    /// <summary>Cascade A ships gates report-only; Cascade B flips this by passing armed:true.</summary>
    public const bool SemanticGatesEnforcingDefault = false;

    public sealed record CorpusLoad(List<Frontmatter> Docs, List<Finding> WalkFindings, int Scanned, int InScope);

    /// <summary>Walks the Markdown corpus and parses frontmatter into documents + walk-time errors.</summary>
    public static CorpusLoad LoadCorpus(string repoRoot)
    {
        var exclusions = ExclusionConfig.Load(RepoPaths.ExclusionsPath(repoRoot));
        var docs = new List<Frontmatter>();
        var findings = new List<Finding>();
        int scanned = 0, inScope = 0;

        foreach (string rel in CorpusWalker.MarkdownRelPaths(repoRoot))
        {
            scanned++;
            if (exclusions.IsExcluded(rel))
            {
                continue;
            }

            inScope++;
            string abs = Path.Combine(repoRoot, rel.Replace('/', Path.DirectorySeparatorChar));
            string? fmText = FrontmatterExtractor.Extract(File.ReadAllText(abs), FrontmatterExtractor.IsReadmeClass(rel));

            if (fmText is null)
            {
                findings.Add(new Finding("G-PATH", Severity.Error, rel,
                    "no frontmatter block and not on the exclusion list (fail-closed, FRAMEWORK 14.2)"));
                continue;
            }

            var (doc, error) = Frontmatter.TryParse(fmText, rel);
            if (doc is null)
            {
                findings.Add(new Finding("G-SCHEMA", Severity.Error, rel, error ?? "frontmatter parse error"));
                continue;
            }

            docs.Add(doc);
        }

        return new CorpusLoad(docs, findings, scanned, inScope);
    }

    /// <summary>Validate only: no writes. Returns the exit code (0 green / 1 blocking).</summary>
    public static int Validate(string repoRoot, bool armed, TextWriter output)
    {
        CorpusLoad load = LoadCorpus(repoRoot);
        List<Finding> findings = CollectFindings(load, repoRoot);
        return ReportAndExit(findings, armed, load, output, "validate", wrote: null);
    }

    /// <summary>Validate, then (if not blocked) regenerate the two derived artifacts idempotently.</summary>
    public static int Sync(string repoRoot, bool armed, string registerVersion, TextWriter output)
    {
        CorpusLoad load = LoadCorpus(repoRoot);
        var globals = GlobalsCollections.Load(RepoPaths.GovernanceDir(repoRoot));
        List<Finding> findings = CollectFindings(load, repoRoot, globals);

        if (IsBlocking(findings, armed))
        {
            return ReportAndExit(findings, armed, load, output, "sync", wrote: false);
        }

        string archive = Validators.RenderArchive(load.Docs, globals, registerVersion, LoadSupplement(repoRoot));
        string surface = Validators.RenderAuthoritySurface(load.Docs);
        bool wroteArchive = WriteIfChanged(RepoPaths.RegisterPath(repoRoot), archive);
        bool wroteSurface = WriteIfChanged(RepoPaths.AuthoritySurfacePath(repoRoot), surface);

        return ReportAndExit(findings, armed, load, output, "sync", wrote: wroteArchive || wroteSurface);
    }

    /// <summary>Writes only when the LF-normalized content differs; returns whether a write occurred.</summary>
    public static bool WriteIfChanged(string path, string content)
    {
        string? existing = File.Exists(path) ? File.ReadAllText(path) : null;
        if (Normalize(existing) == Normalize(content))
        {
            return false;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return true;

        static string Normalize(string? s) => s is null ? "\0(absent)" : s.Replace("\r\n", "\n");
    }

    private static readonly YamlDotNet.Serialization.IDeserializer SupplementDeserializer =
        new YamlDotNet.Serialization.DeserializerBuilder().IgnoreUnmatchedProperties().Build();

    /// <summary>Loads the provisional non-.md supplement's documents, or an empty list when absent.</summary>
    private static IReadOnlyList<Dictionary<string, object?>> LoadSupplement(string repoRoot)
    {
        string path = Path.Combine(RepoPaths.GovernanceDir(repoRoot), "REGISTER_SUPPLEMENT.yaml");
        if (!File.Exists(path))
        {
            return Array.Empty<Dictionary<string, object?>>();
        }

        var file = SupplementDeserializer.Deserialize<SupplementFile>(File.ReadAllText(path));
        return file?.documents ?? new List<Dictionary<string, object?>>();
    }

    private sealed class SupplementFile
    {
        public List<Dictionary<string, object?>>? documents { get; set; }
    }

    private static List<Finding> CollectFindings(CorpusLoad load, string repoRoot, GlobalsCollections? globals = null)
    {
        var findings = new List<Finding>(load.WalkFindings);
        findings.AddRange(Validators.RunAll(load.Docs));
        findings.AddRange((globals ?? GlobalsCollections.Load(RepoPaths.GovernanceDir(repoRoot))).Validate());
        return findings;
    }

    private static bool IsBlocking(IEnumerable<Finding> findings, bool armed) =>
        findings.Any(f => f.Severity == Severity.Error || (armed && f.Severity == Severity.Report));

    private static int ReportAndExit(
        List<Finding> findings, bool armed, CorpusLoad load, TextWriter output, string command, bool? wrote)
    {
        var errors = findings.Where(f => f.Severity == Severity.Error).ToList();
        var reports = findings.Where(f => f.Severity == Severity.Report).ToList();
        var monitors = findings.Where(f => f.Severity == Severity.Monitor).ToList();

        output.WriteLine($"[governance] {command}: {load.Docs.Count} documents parsed "
            + $"({load.InScope} in-scope of {load.Scanned} scanned .md)");
        output.WriteLine($"  errors (exit-affecting) : {errors.Count}");
        output.WriteLine($"  gate findings (report-only{(armed ? "; ARMED = exit-affecting" : "")}) : {reports.Count}");

        foreach (Finding e in errors.Take(40))
        {
            output.WriteLine($"  ERROR  {e}");
        }

        if (errors.Count > 40)
        {
            output.WriteLine($"  ... (+{errors.Count - 40} more errors)");
        }

        foreach (IGrouping<string, Finding> group in reports.GroupBy(f => f.Gate).OrderBy(g => g.Key))
        {
            output.WriteLine($"  report {group.Key} : {group.Count()}");
        }

        foreach (Finding r in reports.Take(80))
        {
            output.WriteLine($"  REPORT {r}");
        }

        if (reports.Count > 80)
        {
            output.WriteLine($"  ... (+{reports.Count - 80} more report findings)");
        }

        foreach (Finding m in monitors)
        {
            output.WriteLine($"  monitor {m}");
        }

        if (wrote is not null)
        {
            output.WriteLine($"  derived artifacts written : {wrote}");
        }

        bool blocking = IsBlocking(findings, armed);
        output.WriteLine(blocking ? $"[governance] {command} FAILED" : $"[governance] {command} OK");
        return blocking ? 1 : 0;
    }
}
