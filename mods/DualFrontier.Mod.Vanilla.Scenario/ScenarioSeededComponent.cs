using DualFrontier.Contracts.Core;

namespace DualFrontier.Mod.Vanilla.Scenario;

/// <summary>
/// The mark this mod leaves on a world it has already seeded, held on a single entity minted for
/// the purpose. A plain <c>unmanaged</c> struct authored against <c>DualFrontier.Contracts</c>
/// alone (K-L3 Path α), owned by this mod and read by nothing else.
///
/// <para>
/// <b>Why a mark rather than a census.</b> The seeder used to decide "have I run before?" by
/// looking for a colonist. That answer is only correct for a scenario that asks for colonists. A
/// distribution may legitimately ask for none — the loader accepts a zero count deliberately, and
/// there is a test pinning that it does — and such a world seeds items, then reports itself
/// unseeded on the next reload and mints the whole item set again. Every reload would add another
/// full set of food, water, beds and decorations to a world that already had one.
/// </para>
///
/// <para>
/// Probing for a POSITION instead would cover the reported case and introduce a worse one: any
/// entity placed by anything else would read as this mod's work and silently suppress the real
/// seeding. A private mark is true exactly when this seeder has run, whatever the scenario asked
/// for and whoever else has touched the world.
/// </para>
///
/// <para>
/// It carries the scenario's identity rather than being empty, so the mark says WHICH scenario
/// wrote the world instead of merely that something did. Nothing reads that field today; it is
/// stored because a mark without it could not answer the first question anyone asks of a loaded
/// world, and because the field is free — the component needs at least one either way.
/// </para>
/// </summary>
public struct ScenarioSeededComponent : IComponent
{
    /// <summary>
    /// A stable hash of the seeding scenario's id. An int rather than the string because a
    /// component must be <c>unmanaged</c> with a fixed native layout.
    /// </summary>
    public int ScenarioId;

    /// <summary>The world seed the scenario declared, carried verbatim.</summary>
    public int WorldSeed;
}
