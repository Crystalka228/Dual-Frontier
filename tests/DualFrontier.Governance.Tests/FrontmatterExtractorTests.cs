using DualFrontier.Governance;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

public sealed class FrontmatterExtractorTests
{
    [Fact]
    public void Extract_Top_ReturnsBlockBetweenFences_AndExcludesBody()
    {
        string doc = "---\nregister_id: DOC-A-X\ntier: 1\n---\n\n# Title\nbody\n";

        string? fm = FrontmatterExtractor.Extract(doc, endOfFile: false);

        fm.Should().NotBeNull();
        fm!.Should().Contain("register_id: DOC-A-X").And.Contain("tier: 1");
        fm.Should().NotContain("# Title");
    }

    [Fact]
    public void Extract_Top_ToleratesLeadingBom()
    {
        string doc = "﻿---\nregister_id: DOC-A-X\n---\nbody\n";

        FrontmatterExtractor.Extract(doc, endOfFile: false).Should().NotBeNull();
    }

    [Fact]
    public void Extract_Top_ReturnsNull_WhenFirstNonBlankIsNotAFence()
    {
        string doc = "# A title first\n---\nnot: real-frontmatter\n---\n";

        FrontmatterExtractor.Extract(doc, endOfFile: false).Should().BeNull();
    }

    [Fact]
    public void Extract_EndOfFile_ReturnsLastBlock_ForReadmeClass()
    {
        string doc = "# Dual Frontier\n\nintro text\n\n---\nregister_id: DOC-G-README\ntier: 2\n---\n";

        string? fm = FrontmatterExtractor.Extract(doc, endOfFile: true);

        fm.Should().NotBeNull();
        fm!.Should().Contain("register_id: DOC-G-README");
        fm.Should().NotContain("intro text");
    }

    [Fact]
    public void Extract_EndOfFile_ReturnsNull_WhenNoTrailingBlock()
    {
        string doc = "# A README with no register block\n\njust prose.\n";

        FrontmatterExtractor.Extract(doc, endOfFile: true).Should().BeNull();
    }

    [Theory]
    [InlineData("README.md", true)]
    [InlineData("docs/README.md", true)]
    [InlineData("src/DualFrontier.Runtime/README.md", true)]
    [InlineData("docs/governance/FRAMEWORK.md", false)]
    [InlineData("tools/briefs/SOME_BRIEF.md", false)]
    public void IsReadmeClass_DetectsReadmeLeaf(string path, bool expected)
    {
        FrontmatterExtractor.IsReadmeClass(path).Should().Be(expected);
    }
}
