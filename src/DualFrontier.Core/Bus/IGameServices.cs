namespace DualFrontier.Core.Bus;

/// <summary>
/// Engine-internal harness bridge (W2/BD-3): the five genre-bus getters plus this aggregator
/// left <c>DualFrontier.Contracts</c> and now live engine-side. Since the W2/BD-3 collapse
/// every getter returns the SAME unified managed dispatch (see <c>GameServices</c> /
/// <c>UnifiedGenreBus</c>); the taxonomy no longer partitions routing. The interface survives
/// only so the live <c>src/</c> harness systems keep their <c>Services.&lt;Bus&gt;.Publish</c>
/// call shape.
///
/// Cutover discipline (GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY §4 / EXECUTION_AUTHORITY_MATRIX
/// §3.0): DELETION TRIGGER = W5, when the last <c>src/</c> harness system migrates off
/// <c>SystemBase</c> and this bridge is removed. Gate: W5 harness migration complete.
/// Equivalence obligation: harness publish/subscribe behaviour is unchanged by the bridge.
/// </summary>
public interface IGameServices
{
    /// <summary>Combat-domain events (the genre routing collapsed at W2/BD-3).</summary>
    ICombatBus Combat { get; }

    /// <summary>Inventory-domain events.</summary>
    IInventoryBus Inventory { get; }

    /// <summary>Magic-domain events.</summary>
    IMagicBus Magic { get; }

    /// <summary>Pawn-domain events.</summary>
    IPawnBus Pawns { get; }

    /// <summary>World-domain events.</summary>
    IWorldBus World { get; }
}
