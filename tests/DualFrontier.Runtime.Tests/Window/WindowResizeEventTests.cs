using DualFrontier.Runtime.Window;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Window;

/// <summary>
/// Pins the resize law every windowing backend obeys (<c>WindowEventDecode</c>): skip a 0×0
/// report, skip an unchanged report, accept anything else.
///
/// <para>These tests were previously three <c>[WindowsOnlyFact]</c> cases that opened a live
/// HWND and injected a synthetic <c>WM_SIZE</c> through the test-only <c>SendMessage</c>
/// P/Invoke. There is no portable analogue of calling a window procedure directly, so the
/// decode was extracted as a pure function instead and <c>SendMessageW</c> retired with its
/// only caller. The assertions are the same three behaviours, now ungated on every host.</para>
/// </summary>
public sealed class WindowResizeEventTests
{
    private static long PackLParam(int width, int height)
    {
        // LOWORD = width, HIWORD = height per the Win32 WM_SIZE contract.
        return (long)((uint)width & 0xFFFF) | (((long)height & 0xFFFF) << 16);
    }

    [Fact]
    public void New_dimensions_are_accepted()
    {
        bool accepted = WindowEventDecode.TrySize(
            candidateWidth: 800, candidateHeight: 600,
            currentWidth: 400, currentHeight: 300,
            out int newWidth, out int newHeight);

        accepted.Should().BeTrue();
        newWidth.Should().Be(800);
        newHeight.Should().Be(600);
    }

    [Fact]
    public void Zero_dimensions_are_skipped()
    {
        // Minimize reports 0×0 (Win32 WM_SIZE; XCB CONFIGURE_NOTIFY while unmapped) — it must
        // not be adopted, and the current dimensions must survive unchanged.
        bool accepted = WindowEventDecode.TrySize(
            candidateWidth: 0, candidateHeight: 0,
            currentWidth: 400, currentHeight: 300,
            out int newWidth, out int newHeight);

        accepted.Should().BeFalse();
        newWidth.Should().Be(400);
        newHeight.Should().Be(300);
    }

    [Fact]
    public void Unchanged_dimensions_are_skipped()
    {
        bool accepted = WindowEventDecode.TrySize(
            candidateWidth: 400, candidateHeight: 300,
            currentWidth: 400, currentHeight: 300,
            out int newWidth, out int newHeight);

        accepted.Should().BeFalse();
        newWidth.Should().Be(400);
        newHeight.Should().Be(300);
    }

    [Theory]
    [InlineData(0, 600)]
    [InlineData(800, 0)]
    public void One_zero_dimension_is_enough_to_skip(int candidateWidth, int candidateHeight)
    {
        WindowEventDecode.TrySize(
            candidateWidth, candidateHeight, 400, 300, out int newWidth, out int newHeight)
            .Should().BeFalse();
        newWidth.Should().Be(400);
        newHeight.Should().Be(300);
    }

    [Fact]
    public void One_changed_dimension_is_enough_to_accept()
    {
        bool accepted = WindowEventDecode.TrySize(
            candidateWidth: 400, candidateHeight: 301,
            currentWidth: 400, currentHeight: 300,
            out int newWidth, out int newHeight);

        accepted.Should().BeTrue();
        newWidth.Should().Be(400);
        newHeight.Should().Be(301);
    }

    [Theory]
    [InlineData(800, 600)]
    [InlineData(0, 0)]
    [InlineData(400, 300)]
    [InlineData(65535, 65535)]
    public void WM_SIZE_lParam_unpacks_to_its_packed_dimensions(int width, int height)
    {
        WindowEventDecode.UnpackSizeLParam(PackLParam(width, height), out int w, out int h);

        w.Should().Be(width);
        h.Should().Be(height);
    }

    [Fact]
    public void WM_SIZE_lParam_decodes_end_to_end_as_the_window_procedure_does()
    {
        // The exact composition Win32Window's WM_SIZE arm performs: unpack, then apply the law.
        WindowEventDecode.UnpackSizeLParam(PackLParam(800, 600), out int reportedW, out int reportedH);
        bool accepted = WindowEventDecode.TrySize(
            reportedW, reportedH, 400, 300, out int newWidth, out int newHeight);

        accepted.Should().BeTrue();
        newWidth.Should().Be(800);
        newHeight.Should().Be(600);
    }
}
