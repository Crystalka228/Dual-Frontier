namespace DualFrontier.Governance;

/// <summary>
/// DualFrontier.Governance command dispatch. This build provides the extraction
/// census (<c>validate</c>), built on the RepoPaths + CorpusWalker +
/// FrontmatterExtractor + ExclusionConfig primitives. The schema/gate validation
/// core and the derived-artifact regeneration (<c>sync</c>) are added next in the
/// REGISTER_INVERSION cascade; the corpus <c>migrate</c> (dry-run) after that.
/// Exit codes follow FRAMEWORK 6: 0 = green, 1 = validation error, 2 = tooling
/// failure / unknown command.
/// </summary>
public static class GovernanceCli
{
    public static int Run(string[] args)
    {
        string command = args.Length > 0 ? args[0].ToLowerInvariant() : "help";
        return command switch
        {
            "validate" => ExtractionCensus(ResolveRoot(args), args.Contains("--list")),
            "sync" => NotWiredInThisBuild("sync", "derived-artifact regeneration"),
            "migrate" => NotWiredInThisBuild("migrate", "corpus migration (dry-run)"),
            "help" or "-h" or "--help" => Usage(),
            _ => Unknown(command),
        };
    }

    /// <summary>
    /// Extraction census over the corpus: walks the Markdown tree, applies the
    /// exclusion config, and reports how many in-scope documents carry frontmatter.
    /// The schema + semantic-gate validation is layered on this primitive next in
    /// the cascade; this proves the extraction path end-to-end.
    /// </summary>
    private static int ExtractionCensus(string repoRoot, bool listMissing)
    {
        var exclusions = ExclusionConfig.Load(RepoPaths.ExclusionsPath(repoRoot));

        int scanned = 0, inScope = 0, withFrontmatter = 0;
        var missing = new List<string>();
        foreach (string rel in CorpusWalker.MarkdownRelPaths(repoRoot))
        {
            scanned++;
            if (exclusions.IsExcluded(rel))
            {
                continue;
            }

            inScope++;
            string abs = Path.Combine(repoRoot, rel.Replace('/', Path.DirectorySeparatorChar));
            string? frontmatter = FrontmatterExtractor.Extract(
                File.ReadAllText(abs),
                FrontmatterExtractor.IsReadmeClass(rel));

            if (frontmatter is null)
            {
                missing.Add(rel);
            }
            else
            {
                withFrontmatter++;
            }
        }

        Console.WriteLine($"[governance] extraction census (root: {repoRoot})");
        Console.WriteLine($"  markdown files scanned : {scanned}");
        Console.WriteLine($"  in governed scope      : {inScope}");
        Console.WriteLine($"  with frontmatter       : {withFrontmatter}");
        Console.WriteLine($"  without frontmatter    : {missing.Count}  (in-scope, no frontmatter block)");
        if (listMissing)
        {
            foreach (string rel in missing.OrderBy(p => p, StringComparer.Ordinal))
            {
                Console.WriteLine($"    - {rel}");
            }
        }

        return 0;
    }

    private static string ResolveRoot(string[] args)
    {
        for (int i = 1; i < args.Length - 1; i++)
        {
            if (args[i] == "--repo")
            {
                return Path.GetFullPath(args[i + 1]);
            }
        }

        return RepoPaths.RepoRoot();
    }

    private static int NotWiredInThisBuild(string command, string what)
    {
        Console.Error.WriteLine($"{command}: {what} is not wired in this build.");
        return 2;
    }

    private static int Unknown(string command)
    {
        Console.Error.WriteLine($"unknown command '{command}'.");
        return Usage() == 0 ? 2 : 2;
    }

    private static int Usage()
    {
        Console.WriteLine("DualFrontier.Governance -- inverted register tool");
        Console.WriteLine();
        Console.WriteLine("Usage: dotnet run --project tools/DualFrontier.Governance -- <command> [--repo <path>]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  validate   extraction census over the corpus (schema + gates layered on next)");
        Console.WriteLine("  sync       regenerate the derived REGISTER.yaml + CURRENT_AUTHORITY_SURFACE.yaml");
        Console.WriteLine("  migrate    dry-run corpus migration into schema-v2 frontmatter (--target required)");
        Console.WriteLine("  help       show this message");
        return 0;
    }
}
