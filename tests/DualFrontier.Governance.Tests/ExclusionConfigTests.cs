using DualFrontier.Governance;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

public sealed class ExclusionConfigTests
{
    private static ExclusionConfig LiveConfig() =>
        ExclusionConfig.Load(RepoPaths.ExclusionsPath(RepoPaths.RepoRoot()));

    [Fact]
    public void Load_RealScopeExclusions_AllowsMarkdownOnly()
    {
        LiveConfig().IncludedExtensions.Should().Contain(".md");
    }

    [Fact]
    public void IsExcluded_BuildArtifactPaths_AreExcluded()
    {
        var cfg = LiveConfig();

        cfg.IsExcluded("obj/project.assets.md").Should().BeTrue("obj/** pattern");
        cfg.IsExcluded("node_modules/pkg/readme.md").Should().BeTrue("node_modules/** pattern");
        cfg.IsExcluded("tests/DualFrontier.Core.Tests/bin/Debug/net8.0/README.md")
            .Should().BeTrue("**/bin/** pattern matches per-project bin");
    }

    [Fact]
    public void IsExcluded_GovernedDocuments_AreNotExcluded()
    {
        var cfg = LiveConfig();

        cfg.IsExcluded("docs/governance/FRAMEWORK.md").Should().BeFalse();
        cfg.IsExcluded("tools/briefs/REGISTER_INVERSION_A_BRIEF.md").Should().BeFalse();
        cfg.IsExcluded("src/DualFrontier.Runtime/MODULE.md").Should().BeFalse();
    }

    [Fact]
    public void Load_MissingFile_YieldsSafeDefault()
    {
        var cfg = ExclusionConfig.Load(Path.Combine(Path.GetTempPath(), "no-such-scope-exclusions.yaml"));

        cfg.IncludedExtensions.Should().Contain(".md");
        cfg.ExcludedPatterns.Should().BeEmpty();
        cfg.IsExcluded("docs/governance/FRAMEWORK.md").Should().BeFalse();
    }
}
