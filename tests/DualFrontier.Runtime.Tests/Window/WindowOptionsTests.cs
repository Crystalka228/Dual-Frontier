using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Window;

public sealed class WindowOptionsTests
{
    [Fact]
    public void Defaults_match_brief_S_LOCK_1_dimensions()
    {
        var opts = new WindowOptions();
        opts.Title.Should().Be("Dual Frontier");
        opts.Width.Should().Be(1280);
        opts.Height.Should().Be(720);
        opts.Resizable.Should().BeTrue();
    }

    [Fact]
    public void Init_with_record_syntax_overrides_defaults()
    {
        var opts = new WindowOptions
        {
            Title = "Smoke Test",
            Width = 800,
            Height = 600,
            Resizable = false,
        };
        opts.Title.Should().Be("Smoke Test");
        opts.Width.Should().Be(800);
        opts.Height.Should().Be(600);
        opts.Resizable.Should().BeFalse();
    }

    [Fact]
    public void Records_compare_by_value()
    {
        var a = new WindowOptions { Title = "T", Width = 100, Height = 200 };
        var b = new WindowOptions { Title = "T", Width = 100, Height = 200 };
        a.Should().Be(b);
    }
}
