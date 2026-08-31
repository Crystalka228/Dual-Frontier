namespace DualFrontier.Mod.Vanilla.Core;

/// <summary>
/// The vanilla slice roster. MOD-OWNED vocabulary: no engine assembly names this type, and the
/// engine has no idea these values exist.
///
/// <para>
/// It lives in a SHARED mod so that every vanilla slice mod resolves the same
/// <see cref="System.Type"/> through the shared <c>AssemblyLoadContext</c>. A type defined
/// inside a regular mod's collectible ALC would be invisible to its neighbours
/// (MOD_OS_ARCHITECTURE §5), which is the same reason
/// <c>DualFrontier.Mod.Weather.Contracts</c> exists.
/// </para>
/// </summary>
public enum VanillaSlice
{
    /// <summary>Terrain, tiles, and the map itself.</summary>
    World = 0,

    /// <summary>Pawn needs, mood, jobs, movement, sleep, and comfort.</summary>
    Pawn = 1,

    /// <summary>Storage, reservations, hauling, and crafting.</summary>
    Inventory = 2,

    /// <summary>Weapons, damage, projectiles, and status effects.</summary>
    Combat = 3,

    /// <summary>Mana, ether, golem bonds, spells, and rituals.</summary>
    Magic = 4,
}
