using DualFrontier.Runtime.Input;
using DualFrontier.Runtime.Native.Win32;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Window;

/// <summary>
/// Verifies V0.B Window.WindowProcedure WM_SIZE handler emits WindowResizeEvent into the
/// shared InputEventQueue for swapchain consumers (V0.B Commit 7).
/// </summary>
public sealed class WindowResizeEventTests
{
    private const uint WM_SIZE = 0x0005;

    private static IntPtr PackLParam(int width, int height)
    {
        // LOWORD = width, HIWORD = height per Win32 WM_SIZE contract.
        return new IntPtr((long)((uint)width & 0xFFFF) | (((long)height & 0xFFFF) << 16));
    }

    [WindowsOnlyFact]
    public void WM_SIZE_with_new_dimensions_emits_WindowResizeEvent()
    {
        var opts = new WindowOptions { Title = "Resize A", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        using var window = new global::DualFrontier.Runtime.Window.Window(opts, queue);

        Win32Api.SendMessage(window.Handle, WM_SIZE, IntPtr.Zero, PackLParam(800, 600));

        queue.TryDequeue(out IInputEvent? evt).Should().BeTrue();
        evt.Should().BeOfType<WindowResizeEvent>();
        var resize = (WindowResizeEvent)evt!;
        resize.NewWidth.Should().Be(800);
        resize.NewHeight.Should().Be(600);
        window.Width.Should().Be(800);
        window.Height.Should().Be(600);
    }

    [WindowsOnlyFact]
    public void WM_SIZE_with_zero_dimensions_skips_event()
    {
        // Minimize generates WM_SIZE с 0×0 — must NOT enqueue resize event.
        var opts = new WindowOptions { Title = "Resize B", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        using var window = new global::DualFrontier.Runtime.Window.Window(opts, queue);

        Win32Api.SendMessage(window.Handle, WM_SIZE, IntPtr.Zero, PackLParam(0, 0));

        queue.Count.Should().Be(0);
        // Dimensions retain pre-message values.
        window.Width.Should().Be(400);
        window.Height.Should().Be(300);
    }

    [WindowsOnlyFact]
    public void WM_SIZE_unchanged_dimensions_skips_event()
    {
        var opts = new WindowOptions { Title = "Resize C", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        using var window = new global::DualFrontier.Runtime.Window.Window(opts, queue);

        Win32Api.SendMessage(window.Handle, WM_SIZE, IntPtr.Zero, PackLParam(400, 300));

        queue.Count.Should().Be(0);
    }
}
