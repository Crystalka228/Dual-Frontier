using DualFrontier.Governance;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

/// <summary>The DF authority predicate (Live OR LOCKED tier-1/2) and the deterministic surface render.</summary>
public sealed class AuthoritySurfaceTests
{
    private static List<Frontmatter> SurfaceCorpus() => new()
    {
        Fixture.Doc(("register_id", "DOC-A-ARCH"), ("category", "A"), ("lifecycle", "LOCKED"), ("tier", 1)),
        Fixture.Doc(("register_id", "DOC-B-STD"), ("category", "B"), ("lifecycle", "LOCKED"), ("tier", 2)),
        Fixture.Doc(("register_id", "DOC-C-TRACK"), ("category", "C"), ("lifecycle", "Live"), ("tier", 2)),
        Fixture.Doc(("register_id", "DOC-A-DEEP"), ("category", "A"), ("lifecycle", "LOCKED"), ("tier", 3)),
        Fixture.Doc(("register_id", "DOC-D-BRIEF"), ("category", "D"), ("lifecycle", "EXECUTED"), ("tier", 3)),
    };

    [Fact]
    public void Predicate_Selects_LiveAndLockedTier12_Only()
    {
        var ids = Validators.SelectAuthoritySurface(SurfaceCorpus())
            .Select(d => d.RegisterId)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();

        ids.Should().Equal("DOC-A-ARCH", "DOC-B-STD", "DOC-C-TRACK");
    }

    [Fact]
    public void Predicate_Excludes_LockedTier3_And_ExecutedBriefs()
    {
        var ids = Validators.SelectAuthoritySurface(SurfaceCorpus()).Select(d => d.RegisterId).ToList();

        ids.Should().NotContain("DOC-A-DEEP");  // LOCKED but tier 3
        ids.Should().NotContain("DOC-D-BRIEF"); // EXECUTED
    }

    [Fact]
    public void Render_IsByteStable_OnUnchangedCorpus()
    {
        var corpus = SurfaceCorpus();
        Validators.RenderAuthoritySurface(corpus).Should().Be(Validators.RenderAuthoritySurface(corpus));
    }

    [Fact]
    public void Render_CarriesDerivedBanner_AndExcludesNonSurfaceDocs()
    {
        string yaml = Validators.RenderAuthoritySurface(SurfaceCorpus());

        yaml.Should().Contain("DERIVED, never hand-edited");
        yaml.Should().Contain("DOC-A-ARCH");
        yaml.Should().NotContain("DOC-D-BRIEF");
    }

    [Fact]
    public void Render_ChangesWhenADocFlipsOntoTheSurface()
    {
        string before = Validators.RenderAuthoritySurface(SurfaceCorpus());

        var withNew = SurfaceCorpus();
        withNew.Add(Fixture.Doc(("register_id", "DOC-C-NEW"), ("category", "C"), ("lifecycle", "Live"), ("tier", 2)));
        string after = Validators.RenderAuthoritySurface(withNew);

        after.Should().NotBe(before);
        after.Should().Contain("DOC-C-NEW");
    }

    [Fact]
    public void RunAll_CleanCorpus_YieldsNoErrorsOrReports()
    {
        var corpus = new[]
        {
            Fixture.Doc(("register_id", "DOC-A-ARCH"), ("category", "A"), ("lifecycle", "LOCKED"),
                ("tier", 1), ("next_review_due", "2027-01-01")),
        };

        Validators.RunAll(corpus).Where(f => f.Severity != Severity.Monitor).Should().BeEmpty();
    }
}
