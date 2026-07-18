using DualFrontier.Runtime.Input;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Input;

/// <summary>
/// VirtualKeyMapper tests verify Win32 VK_* code → Key enum mapping per S-LOCK-10.
/// </summary>
public sealed class VirtualKeyMapperTests
{
    [Theory]
    [InlineData(0x25, Key.Left)]       // VK_LEFT
    [InlineData(0x26, Key.Up)]         // VK_UP
    [InlineData(0x27, Key.Right)]      // VK_RIGHT
    [InlineData(0x28, Key.Down)]       // VK_DOWN
    [InlineData(0x1B, Key.Escape)]     // VK_ESCAPE
    [InlineData(0x20, Key.Space)]      // VK_SPACE
    [InlineData(0x0D, Key.Enter)]      // VK_RETURN
    [InlineData(0x09, Key.Tab)]        // VK_TAB
    [InlineData(0x08, Key.Backspace)]  // VK_BACK
    [InlineData(0x10, Key.Shift)]      // VK_SHIFT
    [InlineData(0x11, Key.Control)]    // VK_CONTROL
    [InlineData(0x12, Key.Alt)]        // VK_MENU
    [InlineData(0x21, Key.PageUp)]     // VK_PRIOR
    [InlineData(0x22, Key.PageDown)]   // VK_NEXT
    [InlineData(0x70, Key.F1)]
    [InlineData(0x7B, Key.F12)]
    public void Special_keys_map_correctly(int vk, Key expected)
    {
        VirtualKeyMapper.Map(vk).Should().Be(expected);
    }

    [Theory]
    [InlineData(0x41, Key.A)]
    [InlineData(0x5A, Key.Z)]
    [InlineData(0x4D, Key.M)]
    public void Letters_map_correctly(int vk, Key expected)
    {
        VirtualKeyMapper.Map(vk).Should().Be(expected);
    }

    [Theory]
    [InlineData(0x30, Key.Digit0)]
    [InlineData(0x39, Key.Digit9)]
    [InlineData(0x35, Key.Digit5)]
    public void Digits_map_correctly(int vk, Key expected)
    {
        VirtualKeyMapper.Map(vk).Should().Be(expected);
    }

    [Theory]
    [InlineData(0x00)]
    [InlineData(0x29)]   // VK_SELECT — not in V0.C.1 scope
    [InlineData(0xFE)]
    [InlineData(0xFFFF)]
    public void Unsupported_keys_return_Unknown(int vk)
    {
        VirtualKeyMapper.Map(vk).Should().Be(Key.Unknown);
    }

    [Fact]
    public void Event_records_construct_with_expected_properties()
    {
        var pressed = new KeyPressedEvent(Key.A);
        pressed.Key.Should().Be(Key.A);

        var released = new KeyReleasedEvent(Key.Escape);
        released.Key.Should().Be(Key.Escape);

        var move = new MouseMovedEvent(100, 200);
        move.X.Should().Be(100);
        move.Y.Should().Be(200);

        var btn = new MouseButtonEvent(MouseButton.Right, Pressed: true);
        btn.Button.Should().Be(MouseButton.Right);
        btn.Pressed.Should().BeTrue();

        var wheel = new MouseWheelEvent(Delta: -3);
        wheel.Delta.Should().Be(-3);

        var focus = new WindowFocusEvent(Focused: false);
        focus.Focused.Should().BeFalse();
    }

    [Fact]
    public void Input_events_implement_IInputEvent()
    {
        IInputEvent[] events =
        {
            new KeyPressedEvent(Key.A),
            new KeyReleasedEvent(Key.A),
            new MouseMovedEvent(0, 0),
            new MouseButtonEvent(MouseButton.Left, true),
            new MouseWheelEvent(0),
            new WindowFocusEvent(true),
        };
        events.Should().AllBeAssignableTo<IInputEvent>();
    }
}
