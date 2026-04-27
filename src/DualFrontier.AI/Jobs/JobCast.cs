namespace DualFrontier.AI.Jobs;
using DualFrontier.Contracts.Core;

/// <summary>
/// Spell-cast job: walk to range → start the cast → wait for the cast time
/// to finish → publish the event on the <c>Magic</c> bus through
/// <c>SpellSystem</c>.
///
/// See GDD "Magic", "Magic Schools".
/// </summary>
public sealed class JobCast : IJob
{
    // Dependency properties required by the interface IJob
    /// <summary>Pawn executing this job.</summary>
    public EntityId PawnId { get; private set; }

    /// <summary>Current execution status.</summary>
    public JobStatus Status { get; private set; }

    /// <inheritdoc />
    public void Start()
    {
        throw new NotImplementedException("TODO: Phase 4 — JobCast.Start: check mana and school, reserve target");
    }

    /// <inheritdoc />
    public JobStatus Tick(float delta)
    {
        throw new NotImplementedException("TODO: Phase 4 — JobCast.Tick: cast time, interrupt on damage");
    }

    /// <inheritdoc />
    public void Abort()
    {
        throw new NotImplementedException("TODO: Phase 4 — JobCast.Abort: cancel the cast, refund mana");
    }
}
