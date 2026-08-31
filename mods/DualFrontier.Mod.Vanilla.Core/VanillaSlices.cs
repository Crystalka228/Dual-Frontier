using System;

namespace DualFrontier.Mod.Vanilla.Core;

/// <summary>
/// What each vanilla slice mod promises to own once its mechanics land (W5, per the
/// vanilla-separation migration plan). Until then the slice mods carry no mechanics — they
/// announce their promise at load, which is what makes them REAL loadable mods rather than
/// inert directories.
///
/// <para>
/// <b>Why a method and not a <c>const</c>.</b> A <c>const string</c> is baked into the
/// consumer's IL at compile time, so a slice mod reading one would never touch this assembly at
/// runtime and would prove nothing about type identity across the ALC boundary. A static method
/// taking a <see cref="VanillaSlice"/> forces a real cross-ALC call with a shared-owned
/// parameter type, so the shared-vendor seam is exercised rather than assumed.
/// </para>
/// </summary>
public static class VanillaSlices
{
    /// <summary>
    /// The one-line promise for <paramref name="slice"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The value is not a declared <see cref="VanillaSlice"/>. Deliberately loud rather than a
    /// fabricated fallback string: an unknown slice is an authoring error, not a display case.
    /// </exception>
    public static string PromiseFor(VanillaSlice slice) => slice switch
    {
        VanillaSlice.World => "vanilla.world will own terrain, tiles, and the map.",
        VanillaSlice.Pawn => "vanilla.pawn will own needs, mood, jobs, movement, sleep, and comfort.",
        VanillaSlice.Inventory => "vanilla.inventory will own storage, reservations, hauling, and crafting.",
        VanillaSlice.Combat => "vanilla.combat will own weapons, damage, projectiles, and status effects.",
        VanillaSlice.Magic => "vanilla.magic will own mana, ether, golem bonds, spells, and rituals.",
        _ => throw new ArgumentOutOfRangeException(nameof(slice), slice, "Unknown vanilla slice."),
    };
}
