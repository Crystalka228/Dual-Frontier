using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Command to deal damage — unlike the actual <c>DamageEvent</c>,
/// this is a "deal damage" request that <c>DamageSystem</c> validates,
/// applies resistances/vulnerabilities to, and publishes the resulting <c>DamageEvent</c> for.
/// </summary>
/// <param name="Source">Damage source (attacking entity, projectile, trap).</param>
/// <param name="Target">Damage target.</param>
/// <param name="Amount">Raw damage amount before resistances/modifiers.</param>
/// <param name="DamageKind">String identifier for the damage type
/// (TODO: Phase 4 — replace with the <c>DamageType</c> enum).</param>
public sealed record DamageIntent(
    EntityId Source,
    EntityId Target,
    float Amount,
    string DamageKind) : ICommand;
