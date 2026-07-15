using DualFrontier.Governance;
using FluentAssertions;
using Xunit;
using YamlDotNet.Serialization;

namespace DualFrontier.Governance.Tests;

/// <summary>
/// The D4 migration: the fail-closed guard, the field mapping (drops + backfills),
/// and a fixture round-trip that mirrors the C5 checkpoint on a small scale --
/// migrate -> sync -> reconcile with zero deltas + idempotency + globals extraction.
/// </summary>
public sealed class MigratorTests
{
    [Fact]
    public void Migrate_RefusesLiveCorpus_WithoutTheMutationFlag()
    {
        Action act = () => Migrator.Migrate(RepoPaths.RepoRoot(), allowLiveCorpus: false);

        act.Should().Throw<InvalidOperationException>().WithMessage("*refuses to mutate the live corpus*");
    }

    [Fact]
    public void BuildV2Frontmatter_DropsRatifiedFields_RenamesId_AndBackfillsRequired()
    {
        var old = new Dictionary<string, object?>
        {
            ["id"] = "DOC-A-X",
            ["path"] = "docs/x.md",
            ["category"] = "A",
            ["tier"] = 1,
            ["lifecycle"] = "LOCKED",
            ["owner"] = "Crystalka",
            ["version"] = "1.0",
            ["last_modified"] = "2026-07-15",
            ["content_language"] = "en",
            ["next_review_due"] = "2027-01-01",
            ["register_view_url"] = "docs/governance/REGISTER_RENDER.md#DOC-A-X",
            ["last_modified_commit"] = "PENDING-COMMIT-X",
            ["title"] = "kept",
        };

        var (fm, projectBackfilled, firstAuthoredBackfilled) = Migrator.BuildV2Frontmatter(old);

        fm["register_id"].Should().Be("DOC-A-X");
        fm.Should().ContainKey("project").And.ContainKey("first_authored");
        projectBackfilled.Should().BeTrue();
        firstAuthoredBackfilled.Should().BeTrue();
        fm.Should().NotContainKey("register_view_url", "ratified drop");
        fm.Should().NotContainKey("last_modified_commit", "PENDING-* is a ratified drop");
        fm.Should().NotContainKey("id").And.NotContainKey("path");
        fm["title"].Should().Be("kept", "non-dropped fields carry over");
    }

    [Fact]
    public void Migrate_DryRun_RoundTripsWithZeroDeltas_ExtractsGlobals_AndIsIdempotent()
    {
        using var scratch = new Scratch();
        scratch.WriteExclusions();
        scratch.WriteOldRegister();
        scratch.WriteDoc("docs/EXAMPLE.md", Mirror("DOC-A-EXAMPLE", "A", "1", "LOCKED", "2027-01-01"));
        scratch.WriteDoc("docs/OTHER.md", Mirror("DOC-C-OTHER", "C", "2", "Live", "2026-Q3"));

        List<Dictionary<string, object?>> oldDocs = LoadDocs(scratch.RegisterPath);

        Migrator.MigrationReport report = Migrator.Migrate(scratch.Root, allowLiveCorpus: false);

        report.MarkdownMigrated.Should().Be(2);
        report.NonMarkdownSupplemented.Should().Be(1, "the .cs entry cannot carry frontmatter");
        report.RequirementsExtracted.Should().Be(1);
        File.Exists(Path.Combine(scratch.GovernanceDir, "REQUIREMENTS.yaml")).Should().BeTrue();
        File.Exists(Path.Combine(scratch.GovernanceDir, "REGISTER_SUPPLEMENT.yaml")).Should().BeTrue();

        int syncExit = RegisterSync.Sync(scratch.Root, armed: false, "2.0", TextWriter.Null);
        syncExit.Should().Be(0, "the migrated v2 corpus validates structurally");

        string derived1 = File.ReadAllText(scratch.RegisterPath);
        RegisterSync.Sync(scratch.Root, armed: false, "2.0", TextWriter.Null);
        string derived2 = File.ReadAllText(scratch.RegisterPath);
        derived1.Should().Be(derived2, "an unchanged corpus regenerates byte-identically");

        List<Dictionary<string, object?>> derivedDocs = LoadDocs(scratch.RegisterPath);
        Migrator.Reconcile(oldDocs, derivedDocs)
            .Should().BeEmpty("zero lost documents, zero invented fields beyond the ratified drops");
    }

    private static List<Dictionary<string, object?>> LoadDocs(string registerPath)
    {
        var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        return deserializer.Deserialize<Migrator.OldRegister>(File.ReadAllText(registerPath))?.documents ?? new();
    }

    private static string Mirror(string id, string category, string tier, string lifecycle, string nextReview) =>
        "---\n" +
        "# Auto-generated from docs/governance/REGISTER.yaml\n" +
        $"register_id: {id}\n" +
        $"category: {category}\n" +
        $"tier: {tier}\n" +
        $"lifecycle: {lifecycle}\n" +
        "owner: Crystalka\n" +
        "version: \"1.0\"\n" +
        $"next_review_due: {nextReview}\n" +
        $"register_view_url: docs/governance/REGISTER_RENDER.md#{id}\n" +
        "---\n";

    private sealed class Scratch : IDisposable
    {
        public string Root { get; } = Path.Combine(Path.GetTempPath(), $"df-migtest-{Guid.NewGuid():N}");
        public string GovernanceDir => Path.Combine(Root, "docs", "governance");
        public string RegisterPath => Path.Combine(GovernanceDir, "REGISTER.yaml");

        public Scratch()
        {
            Directory.CreateDirectory(GovernanceDir);
            Directory.CreateDirectory(Path.Combine(Root, "tools", "governance"));
        }

        public void WriteExclusions() =>
            File.WriteAllText(Path.Combine(Root, "tools", "governance", "SCOPE_EXCLUSIONS.yaml"),
                "included_extensions:\n  - \".md\"\n");

        public void WriteDoc(string rel, string mirror)
        {
            string abs = Path.Combine(Root, rel.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(abs)!);
            File.WriteAllText(abs, mirror + "\n# Document\nbody\n");
        }

        public void WriteOldRegister() => File.WriteAllText(RegisterPath, OldRegisterYaml);

        public void Dispose()
        {
            try { Directory.Delete(Root, recursive: true); }
            catch (IOException) { }
        }
    }

    private const string OldRegisterYaml = """
        schema_version: "1.0"
        register_version: "2.24"
        documents:
          - id: DOC-A-EXAMPLE
            path: docs/EXAMPLE.md
            title: "Example"
            category: A
            tier: 1
            lifecycle: LOCKED
            owner: Crystalka
            version: "1.0"
            last_modified: "2026-07-15"
            content_language: en
            next_review_due: "2027-01-01"
            register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-EXAMPLE
          - id: DOC-C-OTHER
            path: docs/OTHER.md
            category: C
            tier: 2
            lifecycle: Live
            owner: Crystalka
            version: "Live"
            last_modified: "2026-07-15"
            content_language: en
            next_review_due: "2026-Q3"
          - id: DOC-F-SRC-THING
            path: src/Thing.cs
            category: F
            tier: 4
            lifecycle: Live
            owner: Crystalka
            version: "Live"
            last_modified: "2026-07-15"
            content_language: en
            next_review_due: "null"
        requirements:
          - id: REQ-1
            title: "req"
            verification_status: VERIFIED
        risks: []
        capa_entries: []
        audit_trail: []
        """;
}
