using System;
using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// Phase 4 HUD bridge: each SLOW tick, emits a PawnStateChangedEvent per
/// pawn so GameBootstrap can forward the data as a PawnStateCommand to
/// the presentation HUD. Read-only on pawn components; only publishes on
/// the Pawns bus.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(NeedsComponent), typeof(MindComponent), typeof(JobComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.SLOW)]
public sealed class PawnStateReporterSystem : SystemBase
{
    private static readonly string[] Names =
    {
        "Brother Cassian", "Sister Maria",   "Magus Ferro",
        "Vexillus Korvin", "Inquisitor Vex", "Acolyte Veneris"
    };

    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        foreach (var pawn in Query<NeedsComponent, JobComponent>())
        {
            var needs = GetComponent<NeedsComponent>(pawn);
            var mind  = GetComponent<MindComponent>(pawn);
            var job   = GetComponent<JobComponent>(pawn);

            Services.Pawns.Publish(new PawnStateChangedEvent
            {
                PawnId    = pawn,
                Name      = ResolveName(pawn.Index),
                Hunger    = 1f - needs.Hunger,
                Thirst    = 1f - needs.Thirst,
                Rest      = 1f - needs.Rest,
                Comfort   = 1f - needs.Comfort,
                Mood      = mind.Mood,
                JobLabel  = TranslateJob(job.Current),
                JobUrgent = job.Current == JobKind.Eat || job.Current == JobKind.Sleep
            });
        }
    }

    private static string ResolveName(int index)
    {
        int i = Math.Abs(index) % Names.Length;
        return Names[i];
    }

    private static string TranslateJob(JobKind kind) => kind switch
    {
        JobKind.Idle         => "Idle",
        JobKind.Haul         => "Hauling",
        JobKind.Build        => "Building",
        JobKind.Mine         => "Mining",
        JobKind.Cook         => "Cooking",
        JobKind.Craft        => "Crafting",
        JobKind.Research     => "Researching",
        JobKind.Medicate     => "Medicating",
        JobKind.Fight        => "Fighting",
        JobKind.Flee         => "Fleeing",
        JobKind.Meditate     => "Meditating",
        JobKind.GolemCommand => "Commanding golem",
        JobKind.Sleep        => "Sleeping",
        JobKind.Eat          => "Foraging",
        JobKind.Social       => "Socialising",
        _                    => "—"
    };
}
