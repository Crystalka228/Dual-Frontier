using DualFrontier.Contracts.Core;

namespace DualFrontier.AI.Jobs;

/// <summary>
/// Job for crafting an item at a workbench. Phase 4 implementation.
/// </summary>
public sealed class JobCraft : IJob
{
    /// <summary>Pawn executing this job.</summary>
    public EntityId PawnId { get; private set; }

    /// <summary>Current execution status.</summary>
    public JobStatus Status { get; private set; }

    /// <inheritdoc />
    public void Start() => throw new System.NotImplementedException("TODO: Фаза 4");

    /// <inheritdoc />
    public JobStatus Tick(float delta) => throw new System.NotImplementedException("TODO: Фаза 4");

    /// <inheritdoc />
    public void Abort() => throw new System.NotImplementedException("TODO: Фаза 4");
}