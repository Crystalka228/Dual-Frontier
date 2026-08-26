using DualFrontier.Runtime.Input;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Input;

/// <summary>
/// Pins X11 keysym → <see cref="Key"/> mapping on raw keysym constants, so the table is verified
/// without a keyboard, a display, or a live window. Ungated: pure arithmetic on integers.
///
/// <para>Deliberately a separate suite from VirtualKeyMapperTests: the two mappers consume
/// different input domains (layout-independent Win32 virtual keys vs layout-resolved X11
/// keysyms) and neither file constrains the other.</para>
/// </summary>
public sealed class XkbKeysymMapperTests
{
    [Theory]
    [InlineData(0xFF51u, Key.Left)]
    [InlineData(0xFF52u, Key.Up)]
    [InlineData(0xFF53u, Key.Right)]
    [InlineData(0xFF54u, Key.Down)]
    public void Arrow_keysyms_map_to_arrow_keys(uint keysym, Key expected)
        => XkbKeysymMapper.Map(keysym).Should().Be(expected);

    [Theory]
    [InlineData(0xFF1Bu, Key.Escape)]
    [InlineData(0x0020u, Key.Space)]
    [InlineData(0xFF0Du, Key.Enter)]
    [InlineData(0xFF8Du, Key.Enter)]     // keypad Enter folds to the same Key
    [InlineData(0xFF09u, Key.Tab)]
    [InlineData(0xFF08u, Key.Backspace)]
    [InlineData(0xFFFFu, Key.Delete)]
    [InlineData(0xFF50u, Key.Home)]
    [InlineData(0xFF57u, Key.End)]
    [InlineData(0xFF55u, Key.PageUp)]
    [InlineData(0xFF56u, Key.PageDown)]
    public void Special_keysyms_map_to_special_keys(uint keysym, Key expected)
        => XkbKeysymMapper.Map(keysym).Should().Be(expected);

    [Theory]
    [InlineData(0xFFE1u, Key.Shift)]     // Shift_L
    [InlineData(0xFFE2u, Key.Shift)]     // Shift_R
    [InlineData(0xFFE3u, Key.Control)]   // Control_L
    [InlineData(0xFFE4u, Key.Control)]   // Control_R
    [InlineData(0xFFE9u, Key.Alt)]       // Alt_L
    [InlineData(0xFFEAu, Key.Alt)]       // Alt_R
    public void Both_sides_of_a_modifier_map_to_one_key(uint keysym, Key expected)
        => XkbKeysymMapper.Map(keysym).Should().Be(expected);

    [Theory]
    [InlineData(0xFFBEu, Key.F1)]
    [InlineData(0xFFC3u, Key.F6)]
    [InlineData(0xFFC9u, Key.F12)]
    public void Function_keysyms_map_across_the_whole_range(uint keysym, Key expected)
        => XkbKeysymMapper.Map(keysym).Should().Be(expected);

    [Theory]
    [InlineData(0x0061u, Key.A)]   // 'a'
    [InlineData(0x0041u, Key.A)]   // 'A' — same physical key, shifted level
    [InlineData(0x007Au, Key.Z)]
    [InlineData(0x005Au, Key.Z)]
    [InlineData(0x006Du, Key.M)]
    [InlineData(0x004Du, Key.M)]
    public void Letter_keysyms_fold_both_cases_to_one_key(uint keysym, Key expected)
        => XkbKeysymMapper.Map(keysym).Should().Be(expected);

    [Theory]
    [InlineData(0x0030u, Key.Digit0)]
    [InlineData(0x0035u, Key.Digit5)]
    [InlineData(0x0039u, Key.Digit9)]
    public void Digit_keysyms_map_to_digit_keys(uint keysym, Key expected)
        => XkbKeysymMapper.Map(keysym).Should().Be(expected);

    [Theory]
    [InlineData(0x0000u)]            // NoSymbol
    [InlineData(0xFF20u)]            // Multi_key — outside the supported set
    [InlineData(0x00E9u)]            // e-acute — Latin-1 letter with no Key member
    [InlineData(0xFFCAu)]            // F13 — one past the mapped function range
    [InlineData(0xFFBDu)]            // KP_Equal — one before F1
    public void Unsupported_keysyms_map_to_unknown(uint keysym)
        => XkbKeysymMapper.Map(keysym).Should().Be(Key.Unknown);

    [Fact]
    public void Function_range_boundaries_do_not_leak_into_neighbours()
    {
        // F1..F12 is a contiguous slice; the members either side must NOT be swept in by the
        // range arm, which is the failure mode a range-based switch invites.
        XkbKeysymMapper.Map(0xFFBEu).Should().Be(Key.F1);
        XkbKeysymMapper.Map(0xFFC9u).Should().Be(Key.F12);
        XkbKeysymMapper.Map(0xFFBDu).Should().Be(Key.Unknown);
        XkbKeysymMapper.Map(0xFFCAu).Should().Be(Key.Unknown);
    }
}
