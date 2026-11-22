using DualFrontier.Governance;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

/// <summary>
/// Sync + globals + fail-closed behaviour, exercised end-to-end on a temp corpus
/// and against the live tree: globals merge, idempotency (byte-stable render and
/// skip-identical write), no-frontmatter-is-ERROR, validate on the live tree, and
/// the schema version both derived artifacts declare.
/// </summary>
public sealed class SyncIntegrationTests
{
    [Fact]
    public void RenderArchive_IsDeterministic()
    {
        var docs = new[] { Fixture.Doc(("register_id", "DOC-A-X"), ("category", "A"), ("lifecycle", "LOCKED"), ("tier", 1)) };
        var globals = new GlobalsCollections();

        Validators.RenderArchive(docs, globals, "2.0").Should().Be(Validators.RenderArchive(docs, globals, "2.0"));
    }

    [Fact]
    public void RenderArchive_CarriesSelfEntry_Documents_AndMergedGlobals()
    {
        var docs = new[] { Fixture.Doc(("register_id", "DOC-A-X"), ("category", "A")) };
        var globals = new GlobalsCollections
        {
            Requirements = new() { new() { ["id"] = "REQ-X-1", ["verification_status"] = "VERIFIED" } },
        };

        string archive = Validators.RenderArchive(docs, globals, "2.0");

        archive.Should().Contain("DOC-A-X");
        archive.Should().Contain("DOC-G-REGISTER");     // deterministic self-entry
        archive.Should().Contain("requirements:").And.Contain("REQ-X-1");
        archive.Should().Contain("register_version: 2.0");
    }

    [Fact]
    public void SchemaVersion_IsTwoPointOne_InBothDerivedArtifacts_WhileTheRegisterVersionLabelStaysItsOwn()
    {
        var docs = new[] { Fixture.Doc(("register_id", "DOC-A-X"), ("category", "A"), ("lifecycle", "Live"), ("tier", 2)) };

        Validators.SchemaVersion.Should().Be("2.1");
        Validators.RenderArchive(docs, new GlobalsCollections(), GovernanceCli.DefaultRegisterVersion)
            .Should().Contain("schema_version: 2.1").And.Contain("register_version: 2.0").And.NotContain("schema_version: 2.0");
        Validators.RenderAuthoritySurface(docs)
            .Should().Contain("schema_version: 2.1").And.NotContain("schema_version: 2.0");
        GovernanceCli.DefaultRegisterVersion.Should().Be("2.0", "FRAMEWORK 7.5: the register version is a content label, never derived from the schema version");
    }

    [Fact]
    public void RegisterSelfEntry_CarriesTheNullSentinel_NotAnEventThatCanNeverFire()
    {
        var docs = new[] { Fixture.Doc(("register_id", "DOC-A-X"), ("category", "A"), ("lifecycle", "Live"), ("tier", 2)) };

        string archive = Validators.RenderArchive(docs, new GlobalsCollections(), GovernanceCli.DefaultRegisterVersion);

        var entry = System.Text.RegularExpressions.Regex.Match(archive, @"register_id: DOC-G-REGISTER\n(?:  .*\n)*?  next_review_due: (\S+)\n");
        entry.Success.Should().BeTrue("the archive carries its own self-entry");
        entry.Groups[1].Value.Should().Be("null", "a derived, regenerated archive schedules no review");
    }

    [Fact]
    public void WriteIfChanged_SkipsWhenContentIsIdentical()
    {
        string path = Path.Combine(Path.GetTempPath(), $"df-gov-write-{Guid.NewGuid():N}.txt");
        try
        {
            RegisterSync.WriteIfChanged(path, "alpha\nbeta\n").Should().BeTrue("first write creates the file");
            RegisterSync.WriteIfChanged(path, "alpha\nbeta\n").Should().BeFalse("identical content is not rewritten");
            RegisterSync.WriteIfChanged(path, "alpha\ngamma\n").Should().BeTrue("changed content is rewritten");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Sync_OnCleanScratchCorpus_MergesGlobals_AndIsByteStable()
    {
        using var corpus = new TempCorpus();
        corpus.WriteDoc("A.md", "DOC-A-ARCH", "A", "1", "LOCKED", "2027-01-01");
        corpus.WriteDoc("C.md", "DOC-C-TRACK", "C", "2", "Live", "2026-Q3");
        corpus.WriteGovernanceFile("REQUIREMENTS.yaml",
            "requirements:\n  - id: REQ-X-1\n    verification_status: VERIFIED\n");

        int first = RegisterSync.Sync(corpus.Root, armed: false, registerVersion: "2.0", TextWriter.Null);
        string archive1 = File.ReadAllText(corpus.RegisterPath);

        int second = RegisterSync.Sync(corpus.Root, armed: false, registerVersion: "2.0", TextWriter.Null);
        string archive2 = File.ReadAllText(corpus.RegisterPath);

        first.Should().Be(0, "a clean v2 corpus validates");
        second.Should().Be(0);
        archive1.Should().Be(archive2, "an unchanged corpus regenerates byte-identically (derived-register integrity invariant)");
        archive1.Should().Contain("DOC-A-ARCH").And.Contain("DOC-C-TRACK").And.Contain("REQ-X-1");
        File.Exists(corpus.AuthoritySurfacePath).Should().BeTrue();
        File.ReadAllText(corpus.AuthoritySurfacePath).Should().Contain("DOC-A-ARCH").And.Contain("DOC-C-TRACK");
    }

    [Fact]
    public void LoadCorpus_InScopeDocWithoutFrontmatter_IsAnError()
    {
        using var corpus = new TempCorpus();
        corpus.WriteGovernanceFile("ORPHAN.md", "# A doc with no frontmatter\n\njust prose.\n");

        RegisterSync.CorpusLoad load = RegisterSync.LoadCorpus(corpus.Root);

        load.WalkFindings.Should().ContainSingle(f =>
            f.Gate == "G-PATH" && f.Severity == Severity.Error && f.Subject.Contains("ORPHAN.md"));
    }

    [Fact]
    public void Validate_AgainstLiveTree_Succeeds()
    {
        // Frontmatter is the source of truth (FRAMEWORK 14.1), so the live corpus
        // validates green unarmed. The fail-closed half is pinned on a synthetic
        // fixture by LoadCorpus_InScopeDocWithoutFrontmatter_IsAnError.
        int exit = RegisterSync.Validate(RepoPaths.RepoRoot(), armed: false, TextWriter.Null);

        exit.Should().Be(0, "the live corpus carries schema-2 frontmatter throughout");
    }

    private sealed class TempCorpus : IDisposable
    {
        public string Root { get; }
        private string GovernanceDir => Path.Combine(Root, "docs", "governance");
        public string RegisterPath => Path.Combine(GovernanceDir, "REGISTER.yaml");
        public string AuthoritySurfacePath => Path.Combine(GovernanceDir, "CURRENT_AUTHORITY_SURFACE.yaml");

        public TempCorpus()
        {
            Root = Path.Combine(Path.GetTempPath(), $"df-gov-corpus-{Guid.NewGuid():N}");
            Directory.CreateDirectory(GovernanceDir);
        }

        public void WriteGovernanceFile(string name, string content) =>
            File.WriteAllText(Path.Combine(GovernanceDir, name), content);

        public void WriteDoc(string name, string id, string category, string tier, string lifecycle, string nextReview)
        {
            string md =
                "---\n" +
                $"register_id: {id}\n" +
                "project: Dual Frontier\n" +
                $"category: {category}\n" +
                $"tier: {tier}\n" +
                $"lifecycle: {lifecycle}\n" +
                "owner: Crystalka\n" +
                "version: '1.0'\n" +
                "first_authored: '2026-07-15'\n" +
                "last_modified: '2026-07-15'\n" +
                "content_language: en\n" +
                $"next_review_due: {nextReview}\n" +
                "---\n\n" +
                $"# {id}\nbody\n";
            WriteGovernanceFile(name, md);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(Root, recursive: true);
            }
            catch (IOException)
            {
                // best-effort temp cleanup
            }
        }
    }
}
