namespace DualFrontier.Governance;

/// <summary>
/// DualFrontier.Governance command dispatch. <c>validate</c> and <c>sync</c> run
/// the full schema + gate catalog over the corpus (RegisterSync); semantic gates
/// are report-only unless <c>--armed</c> (FRAMEWORK 14.8). The corpus <c>migrate</c>
/// (dry-run) is wired next in the cascade. Exit codes follow FRAMEWORK 6: 0 = green,
/// 1 = blocking validation error, 2 = tooling failure / unknown command.
/// </summary>
public static class GovernanceCli
{
    public static int Run(string[] args)
    {
        string command = args.Length > 0 ? args[0].ToLowerInvariant() : "help";
        bool armed = args.Contains("--armed");

        return command switch
        {
            "validate" => RegisterSync.Validate(ResolveRoot(args), armed, Console.Out),
            "sync" => RegisterSync.Sync(ResolveRoot(args), armed, ResolveRegisterVersion(args), Console.Out),
            "migrate" => NotWiredInThisBuild("migrate", "corpus migration (dry-run)"),
            "help" or "-h" or "--help" => Usage(),
            _ => Unknown(command),
        };
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

    private static string ResolveRegisterVersion(string[] args)
    {
        for (int i = 1; i < args.Length - 1; i++)
        {
            if (args[i] == "--register-version")
            {
                return args[i + 1];
            }
        }

        return "2.0";
    }

    private static int NotWiredInThisBuild(string command, string what)
    {
        Console.Error.WriteLine($"{command}: {what} is not wired in this build.");
        return 2;
    }

    private static int Unknown(string command)
    {
        Console.Error.WriteLine($"unknown command '{command}'.");
        Usage();
        return 2;
    }

    private static int Usage()
    {
        Console.WriteLine("DualFrontier.Governance -- inverted register tool");
        Console.WriteLine();
        Console.WriteLine("Usage: dotnet run --project tools/DualFrontier.Governance -- <command> [options]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  validate   schema + gate validation over the corpus");
        Console.WriteLine("  sync       regenerate the derived REGISTER.yaml + CURRENT_AUTHORITY_SURFACE.yaml");
        Console.WriteLine("  migrate    dry-run corpus migration into schema-v2 frontmatter (--target required)");
        Console.WriteLine("  help       show this message");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --repo <path>              operate on an explicit root (default: the repo containing DualFrontier.sln)");
        Console.WriteLine("  --armed                    make semantic gate findings exit-affecting (Cascade B; default report-only)");
        Console.WriteLine("  --register-version <ver>   register_version stamped into the derived archive (sync; default 2.0)");
        return 0;
    }
}
