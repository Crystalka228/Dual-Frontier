using System.Runtime.InteropServices;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Input;
using DualFrontier.Runtime.Native.Xcb;
using DualFrontier.Runtime.Window;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Window;

/// <summary>
/// Behavioural pins for the XCB backend against a live X server. Gated on
/// <c>RequiresXcbFact</c>, not the platform-neutral display gate: this suite names XcbWindow
/// directly, so it must be skipped on Windows rather than merely "where a window can open".
///
/// <para>The two load-bearing pins are the close protocol and the resize path. Both are driven
/// by synthesising the real protocol traffic — a WM_DELETE_WINDOW client message and an
/// xcb_configure_window request — rather than by calling the backend's own decode, so they fail
/// if the wiring between the X event and the window's state is broken.</para>
/// </summary>
public sealed class XcbWindowTests
{
    /// <summary>
    /// Pumps until <paramref name="condition"/> holds or the budget expires. X is asynchronous:
    /// a request is answered by an event that arrives some round-trips later, so a single pump
    /// would be a race rather than a test.
    /// </summary>
    private static bool PumpUntil(XcbWindow window, Func<bool> condition, int budgetMilliseconds = 3000)
    {
        int waited = 0;
        while (waited < budgetMilliseconds)
        {
            window.PumpMessages();
            if (condition())
            {
                return true;
            }
            Thread.Sleep(10);
            waited += 10;
        }
        window.PumpMessages();
        return condition();
    }

    [RequiresXcbFact]
    public void Window_opens_with_the_requested_dimensions()
    {
        var opts = new WindowOptions { Title = "XCB open", Width = 640, Height = 480 };
        var queue = new InputEventQueue();
        using var window = new XcbWindow(opts, queue);

        window.IsOpen.Should().BeTrue();
        window.Width.Should().Be(640);
        window.Height.Should().Be(480);
        window.WindowId.Should().NotBe(0u);
        window.Connection.Should().NotBe(IntPtr.Zero);
    }

    [RequiresXcbFact]
    public void Window_is_created_through_the_platform_factory_on_this_host()
    {
        var opts = new WindowOptions { Title = "XCB factory", Width = 320, Height = 240 };
        var queue = new InputEventQueue();
        using IWindow window = PlatformWindow.Create(opts, queue);

        window.Should().BeOfType<XcbWindow>();
        window.IsOpen.Should().BeTrue();
    }

    [RequiresXcbFact]
    public void Show_and_hide_map_and_unmap_without_error()
    {
        var opts = new WindowOptions { Title = "XCB map", Width = 320, Height = 240 };
        var queue = new InputEventQueue();
        using var window = new XcbWindow(opts, queue);

        window.Show();
        window.PumpMessages();
        window.Hide();
        window.PumpMessages();

        // The connection surviving both operations is the assertion: an X protocol error would
        // put it into the error state, which the pump reports by closing the window.
        XcbApi.xcb_connection_has_error(window.Connection).Should().Be(0);
        window.IsOpen.Should().BeTrue();
    }

    [RequiresXcbFact]
    public void Delete_window_client_message_closes_the_window()
    {
        var opts = new WindowOptions { Title = "XCB close", Width = 320, Height = 240 };
        var queue = new InputEventQueue();
        using var window = new XcbWindow(opts, queue);
        window.IsOpen.Should().BeTrue();

        SendDeleteWindowMessage(window);

        PumpUntil(window, () => !window.IsOpen).Should().BeTrue(
            "the WM_DELETE_WINDOW client message is the close-button contract and must clear IsOpen");
        window.IsOpen.Should().BeFalse();
    }

    [RequiresXcbFact]
    public void Configure_notify_updates_width_and_height()
    {
        var opts = new WindowOptions { Title = "XCB resize", Width = 320, Height = 240 };
        var queue = new InputEventQueue();
        using var window = new XcbWindow(opts, queue);

        // Deliberately NOT mapped: an unmapped window still reports geometry changes on
        // StructureNotify, and no window manager is in a position to override the request, so
        // the pin measures the backend rather than the desktop's resize policy.
        ConfigureSize(window, 500, 400);

        PumpUntil(window, () => window.Width == 500 && window.Height == 400).Should().BeTrue(
            "CONFIGURE_NOTIFY must drive Width/Height — the swapchain recreate path reads them, " +
            "and the caps.currentExtent clamp would otherwise mask a missing update");

        window.Width.Should().Be(500);
        window.Height.Should().Be(400);
    }

    [RequiresXcbFact]
    public void Configure_notify_publishes_a_resize_event()
    {
        var opts = new WindowOptions { Title = "XCB resize event", Width = 320, Height = 240 };
        var queue = new InputEventQueue();
        using var window = new XcbWindow(opts, queue);

        ConfigureSize(window, 512, 384);
        PumpUntil(window, () => window.Width == 512 && window.Height == 384).Should().BeTrue();

        WindowResizeEvent? resize = DequeueFirst<WindowResizeEvent>(queue);
        resize.Should().NotBeNull("the XCB arm publishes WindowResizeEvent for parity with WM_SIZE");
        resize!.NewWidth.Should().Be(512);
        resize.NewHeight.Should().Be(384);
    }

    [RequiresXcbFact]
    public void Configure_notify_at_unchanged_size_publishes_nothing()
    {
        var opts = new WindowOptions { Title = "XCB resize noop", Width = 320, Height = 240 };
        var queue = new InputEventQueue();
        using var window = new XcbWindow(opts, queue);

        // X sends CONFIGURE_NOTIFY for moves as well as resizes, so the unchanged-size case is
        // ordinary traffic, not a corner case.
        ConfigureSize(window, 320, 240);
        PumpUntil(window, () => false, budgetMilliseconds: 300);

        window.Width.Should().Be(320);
        window.Height.Should().Be(240);
        DequeueFirst<WindowResizeEvent>(queue).Should().BeNull();
    }

    [RequiresXcbFact]
    public void Window_creates_a_vulkan_surface()
    {
        var opts = new WindowOptions { Title = "XCB surface", Width = 320, Height = 240 };
        var queue = new InputEventQueue();
        using var window = new XcbWindow(opts, queue);
        using var instance = new VulkanInstance(enableValidation: false);

        using var surface = new VulkanSurface(instance, window);

        surface.Handle.Should().NotBe(IntPtr.Zero);
    }

    [RequiresXcbFact]
    public void Dimensions_outside_the_x11_ushort_range_are_rejected()
    {
        var queue = new InputEventQueue();

        // 65536 would narrow to 0 and 70000 to 4464 — silently, because the CreateWindow
        // request is unchecked and X reports protocol errors asynchronously.
        Action tooWide = () => new XcbWindow(
            new WindowOptions { Title = "XCB wide", Width = 65536, Height = 240 }, queue);
        Action tooTall = () => new XcbWindow(
            new WindowOptions { Title = "XCB tall", Width = 320, Height = 70000 }, queue);

        tooWide.Should().Throw<ArgumentOutOfRangeException>();
        tooTall.Should().Throw<ArgumentOutOfRangeException>();
    }

    [RequiresXcbFact]
    public void Non_positive_dimensions_are_rejected()
    {
        var queue = new InputEventQueue();

        Action zeroWidth = () => new XcbWindow(
            new WindowOptions { Title = "XCB zero", Width = 0, Height = 240 }, queue);
        Action negativeHeight = () => new XcbWindow(
            new WindowOptions { Title = "XCB negative", Width = 320, Height = -1 }, queue);

        zeroWidth.Should().Throw<ArgumentOutOfRangeException>();
        negativeHeight.Should().Throw<ArgumentOutOfRangeException>();
    }

    // --- protocol helpers -------------------------------------------------------------------

    private static void ConfigureSize(XcbWindow window, uint width, uint height)
    {
        // value_list follows the value_mask bits in ascending order: WIDTH (0x04), HEIGHT (0x08).
        var values = new uint[2];
        values[0] = width;
        values[1] = height;
        XcbApi.xcb_configure_window(
            window.Connection,
            window.WindowId,
            (ushort)(XcbConstants.XCB_CONFIG_WINDOW_WIDTH | XcbConstants.XCB_CONFIG_WINDOW_HEIGHT),
            ref values[0]);
        XcbApi.xcb_flush(window.Connection);
    }

    private static void SendDeleteWindowMessage(XcbWindow window)
    {
        uint wmProtocols = InternAtom(window.Connection, "WM_PROTOCOLS");

        var message = new XcbClientMessageEvent
        {
            response_type = XcbConstants.XCB_CLIENT_MESSAGE,
            format = 32,
            sequence = 0,
            window = window.WindowId,
            type = wmProtocols,
            data0 = window.WmDeleteWindowAtom,
        };

        // xcb_send_event always reads a 32-byte event buffer regardless of the event's own size.
        const int XcbEventBytes = 32;
        IntPtr buffer = Marshal.AllocHGlobal(XcbEventBytes);
        try
        {
            for (int i = 0; i < XcbEventBytes; i++)
            {
                Marshal.WriteByte(buffer, i, 0);
            }
            Marshal.StructureToPtr(message, buffer, fDeleteOld: false);
            XcbApi.xcb_send_event(
                window.Connection, propagate: false, destination: window.WindowId,
                event_mask: 0, @event: buffer);
            XcbApi.xcb_flush(window.Connection);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private static uint InternAtom(IntPtr connection, string name)
    {
        XcbInternAtomCookie cookie = XcbApi.xcb_intern_atom(
            connection, only_if_exists: false, (ushort)name.Length, name);
        IntPtr reply = XcbApi.xcb_intern_atom_reply(connection, cookie, IntPtr.Zero);
        try
        {
            return Marshal.PtrToStructure<XcbInternAtomReply>(reply).atom;
        }
        finally
        {
            XcbApi.free(reply);
        }
    }

    private static T? DequeueFirst<T>(InputEventQueue queue) where T : class, IInputEvent
    {
        while (queue.TryDequeue(out IInputEvent? evt))
        {
            if (evt is T match)
            {
                return match;
            }
        }
        return null;
    }
}
