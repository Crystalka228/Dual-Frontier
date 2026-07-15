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
            "migrate" => Migrate(args, armed),
            "help" or "-h" or "--help" => Usage(),
            _ => Unknown(command),
        };
    }

    private static int Migrate(string[] args, bool armed)
    {
        string? target = ArgValue(args, "--target");
        if (target is null)
        {
            Console.Error.WriteLine("migrate requires --target <scratch copy path>.");
            return 2;
        }

        target = Path.GetFullPath(target);
        bool allowLive = args.Contains("--i-understand-this-mutates-the-corpus");
        string registerPath = RepoPaths.RegisterPath(target);
        if (!File.Exists(registerPath))
        {
            Console.Error.WriteLine($"migrate: no REGISTER.yaml at {registerPath}.");
            return 2;
        }

        var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        List<Dictionary<string, object?>> oldDocs =
            deserializer.Deserialize<Migrator.OldRegister>(File.ReadAllText(registerPath))?.documents ?? new();

        Migrator.MigrationReport report;
        try
        {
            report = Migrator.Migrate(target, allowLive);
        }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 2;
        }

        int syncExit = RegisterSync.Sync(target, armed: false, "2.0", TextWriter.Null);
        string derived1 = File.ReadAllText(registerPath);
        RegisterSync.Sync(target, armed: false, "2.0", TextWriter.Null);
        string derived2 = File.ReadAllText(registerPath);
        bool idempotent = derived1 == derived2;

        List<Dictionary<string, object?>> derivedDocs =
            deserializer.Deserialize<Migrator.OldRegister>(derived2)?.documents ?? new();
        List<string> deltas = Migrator.Reconcile(oldDocs, derivedDocs);

        Console.WriteLine("[governance] migrate (dry-run) round-trip reconciliation");
        Console.WriteLine($"  markdown documents migrated          : {report.MarkdownMigrated}");
        Console.WriteLine($"  non-.md carried to supplement        : {report.NonMarkdownSupplemented}");
        Console.WriteLine($"  globals REQ/RISK/CAPA/EVT             : {report.RequirementsExtracted}/{report.RisksExtracted}/{report.CapaExtracted}/{report.AuditTrailExtracted}");
        Console.WriteLine($"  backfills project/first_authored     : {report.ProjectBackfilled}/{report.FirstAuthoredBackfilled}");
        Console.WriteLine($"  orphans excluded (Cascade B triage)  : {report.OrphansExcluded}");
        Console.WriteLine($"  derived documents                    : {derivedDocs.Count}");
        Console.WriteLine($"  sync exit                            : {syncExit}");
        Console.WriteLine($"  idempotent (2nd sync byte-identical) : {idempotent}");
        Console.WriteLine($"  reconciliation deltas beyond drops   : {deltas.Count}");
        foreach (string delta in deltas.Take(40))
        {
            Console.WriteLine($"    {delta}");
        }

        if (deltas.Count > 40)
        {
            Console.WriteLine($"    ... (+{deltas.Count - 40} more)");
        }

        if (report.ArchitectRuling.Count > 0)
        {
            Console.WriteLine($"  architect-ruling items ({report.ArchitectRuling.Count}):");
            foreach (string ruling in report.ArchitectRuling)
            {
                Console.WriteLine($"    {ruling}");
            }
        }

        bool clean = syncExit == 0 && idempotent && deltas.Count == 0;
        Console.WriteLine(clean
            ? "[governance] migrate dry-run round-trip CLEAN"
            : "[governance] migrate dry-run has unreconciled deltas");
        return clean ? 0 : 1;
    }

    private static string? ArgValue(string[] args, string name)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == name)
            {
                return args[i + 1];
            }
        }

        return null;
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
