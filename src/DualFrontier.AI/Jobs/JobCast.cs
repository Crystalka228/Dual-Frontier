namespace DualFrontier.AI.Jobs;
using DualFrontier.Contracts.Analyzer;
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
    [ReservedStub(
        ReservedStubPurpose.BuildComposition,
        "Magic Phase 4 roadmap stub (Lesson #N12 sub-pattern A defensive throw) — JobCast.Start: " +
        "mana validation + school check + target reservation pending. " +
        "Activation: Phase 4 magic system integration.")]
    public void Start()
    {
        throw new NotImplementedException("TODO: Phase 4 — JobCast.Start: check mana and school, reserve target");
    }

    /// <inheritdoc />
    [ReservedStub(
        ReservedStubPurpose.BuildComposition,
        "Magic Phase 4 roadmap stub (Lesson #N12 sub-pattern A) — JobCast.Tick: cast time " +
        "progression + interruption-on-damage handling. " +
        "Activation: Phase 4 magic system integration.")]
    public JobStatus Tick(float delta)
    {
        throw new NotImplementedException("TODO: Phase 4 — JobCast.Tick: cast time, interrupt on damage");
    }

    /// <inheritdoc />
    [ReservedStub(
        ReservedStubPurpose.BuildComposition,
        "Magic Phase 4 roadmap stub (Lesson #N12 sub-pattern A) — JobCast.Abort: cast " +
        "cancellation + mana refund. " +
        "Activation: Phase 4 magic system integration.")]
    public void Abort()
    {
        throw new NotImplementedException("TODO: Phase 4 — JobCast.Abort: cancel the cast, refund mana");
    }
}
