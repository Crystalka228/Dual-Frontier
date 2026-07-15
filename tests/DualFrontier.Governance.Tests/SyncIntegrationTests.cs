using DualFrontier.Governance;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

/// <summary>
/// Sync + globals + fail-closed behaviour, exercised end-to-end on a temp corpus
/// and against the live tree. Covers the C4 checkpoint items the pure-gate suite
/// does not: globals merge, idempotency (byte-stable render + skip-identical
/// write), no-frontmatter-is-ERROR, and validate-against-live-fails-loudly.
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
    public void Validate_AgainstLiveTree_FailsLoudly()
    {
        // The live corpus still runs the forward regime: mirrors lack the v2
        // required fields and 8 enrolled docs + orphans have no frontmatter, so
        // validate must be exit-affecting (brief 7.2 -- this failure is CORRECT).
        int exit = RegisterSync.Validate(RepoPaths.RepoRoot(), armed: false, TextWriter.Null);

        exit.Should().Be(1, "the un-migrated live tree carries no-frontmatter + missing-v2-field errors");
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
