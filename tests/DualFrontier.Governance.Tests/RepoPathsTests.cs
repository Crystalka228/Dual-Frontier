using DualFrontier.Governance;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

public sealed class RepoPathsTests
{
    [Fact]
    public void RepoRoot_ResolvesToDirectoryContainingSolution()
    {
        string root = RepoPaths.RepoRoot();

        File.Exists(Path.Combine(root, RepoPaths.SolutionMarker))
            .Should().BeTrue("RepoRoot must land on the directory holding {0}", RepoPaths.SolutionMarker);
    }

    [Fact]
    public void GovernancePaths_ComposeUnderRoot_AndResolveLiveFiles()
    {
        string root = RepoPaths.RepoRoot();

        RepoPaths.RegisterPath(root).Should().EndWith(Path.Combine("docs", "governance", "REGISTER.yaml"));
        RepoPaths.ExclusionsPath(root).Should().EndWith(Path.Combine("tools", "governance", "SCOPE_EXCLUSIONS.yaml"));

        File.Exists(RepoPaths.RegisterPath(root)).Should().BeTrue("the live register is present under the resolved root");
        File.Exists(RepoPaths.ExclusionsPath(root)).Should().BeTrue("the exclusion config is present under the resolved root");
    }
}
