using DualFrontier.Contracts.Core;

namespace DualFrontier.AI.Jobs;

/// <summary>
/// Issues a command to a golem. Phase 4 implementation.
/// </summary>
public sealed class JobGolemCommand : IJob
{
    /// <summary>Pawn executing this job.</summary>
    public EntityId PawnId { get; private set; }

    /// <summary>Current execution status.</summary>
    public JobStatus Status { get; private set; }

    /// <inheritdoc />
    public void Start() =>
        throw new System.NotImplementedException("TODO: Phase 6");

    /// <inheritdoc />
    public JobStatus Tick(float delta) =>
        throw new System.NotImplementedException("TODO: Phase 6");

    /// <inheritdoc />
    public void Abort() =>
        throw new System.NotImplementedException("TODO: Phase 6");
}