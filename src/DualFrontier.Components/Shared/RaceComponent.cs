namespace DualFrontier.Components.Shared;

using DualFrontier.Contracts.Core;

/// <summary>
/// Defines the possible races an entity can belong to.
/// </summary>
public enum RaceKind
{
    Human,
    Elf,
    Dwarf,
    Orc,
    Goblin,
    Halfling,
    Tiefling,
    Dragonborn,
    Gnome,
    HalfElf,
    HalfOrc,
    Troll,
    Golem,    // constructs — no biological needs
    Undead    // no biological needs, immune to psychic damage
}

/// <summary>
/// Represents the racial component of an entity, defining its race and inherent traits.
/// </summary>
public struct RaceComponent : IComponent
{
    // Fields

    /// <summary>
    /// The race of this entity; defaults to Human.
    /// </summary>
    public RaceKind Kind { get; init; }

    /// <summary>
    /// Indicates whether this race can use arcane magic (set by systems at creation).
    /// </summary>
    public bool HasEtherChannels { get; init; }

    /// <summary>
    /// Indicates whether this race has an industrial technology bonus.
    /// </summary>
    public bool HasIndustrialAffinity { get; init; }

    /// <inheritdoc />
    /// <summary>
    /// Determines if the entity's race is considered organic (not Golem or Undead).
    /// </summary>
    public bool IsOrganic => Kind != RaceKind.Golem && Kind != RaceKind.Undead;

    /// <inheritdoc />
    /// <summary>
    /// Checks if the entity requires food to survive, based on its race.
    /// </summary>
    public bool NeedsFood => IsOrganic;

    /// <inheritdoc />
    /// <summary>
    /// Checks if the entity requires sleep to survive, based on its race.
    /// </summary>
    public bool NeedsSleep => IsOrganic;
}
