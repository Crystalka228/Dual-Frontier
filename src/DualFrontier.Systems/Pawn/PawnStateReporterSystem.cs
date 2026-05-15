using System;
using System.Collections.Generic;
using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// HUD bridge: each SLOW tick, emits a PawnStateChangedEvent per pawn so
/// GameBootstrap can forward the data as a PawnStateCommand to the
/// presentation HUD. Read-only on pawn components; only publishes on
/// the Pawns bus. Need values pass through directly from NeedsComponent
/// (already wellness 0..1, 1 = best) — no translation layer.
///
/// Operating principle "data exists or it doesn't": Name comes from
/// IdentityComponent (empty string if absent), TopSkills from
/// SkillsComponent.Levels (empty list if absent or uninitialized). No
/// hardcoded placeholder names, no hash-derived fallback values.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(NeedsComponent), typeof(MindComponent), typeof(JobComponent),
                    typeof(IdentityComponent), typeof(SkillsComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.SLOW)]
public sealed class PawnStateReporterSystem : SystemBase
{
    private const int TopSkillCount = 3;

    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // Pre-materialise membership sets so per-pawn presence checks stay
        // branch-free. Iterate the (smaller) NeedsComponent + JobComponent
        // intersection as the outer loop; identity / skills are optional.
        var identitySet = new HashSet<int>();
        using (SpanLease<IdentityComponent> ids = NativeWorld.AcquireSpan<IdentityComponent>())
        {
            ReadOnlySpan<int> idIndices = ids.Indices;
            for (int i = 0; i < ids.Count; i++) identitySet.Add(idIndices[i]);
        }

        var skillsSet = new HashSet<int>();
        using (SpanLease<SkillsComponent> skills = NativeWorld.AcquireSpan<SkillsComponent>())
        {
            ReadOnlySpan<int> sIndices = skills.Indices;
            for (int i = 0; i < skills.Count; i++) skillsSet.Add(sIndices[i]);
        }

        using SpanLease<NeedsComponent> needs = NativeWorld.AcquireSpan<NeedsComponent>();
        ReadOnlySpan<NeedsComponent> needsSpan = needs.Span;
        ReadOnlySpan<int> needsIndices = needs.Indices;

        for (int i = 0; i < needs.Count; i++)
        {
            var pawn = new EntityId(needsIndices[i], 0);
            if (!NativeWorld.HasComponent<JobComponent>(pawn)) continue;

            NeedsComponent n = needsSpan[i];
            MindComponent mind = NativeWorld.GetComponent<MindComponent>(pawn);
            JobComponent job = NativeWorld.GetComponent<JobComponent>(pawn);

            string name = identitySet.Contains(pawn.Index)
                ? NativeWorld.GetComponent<IdentityComponent>(pawn).Name.Resolve(NativeWorld) ?? string.Empty
                : string.Empty;

            IReadOnlyList<(SkillKind Kind, int Level)> topSkills =
                skillsSet.Contains(pawn.Index)
                    ? ComputeTopSkills(NativeWorld.GetComponent<SkillsComponent>(pawn))
                    : Array.Empty<(SkillKind, int)>();

            Services.Pawns.Publish(new PawnStateChangedEvent
            {
                PawnId    = pawn,
                Name      = name,
                Satiety   = n.Satiety,
                Hydration = n.Hydration,
                Sleep     = n.Sleep,
                Comfort   = n.Comfort,
                Mood      = mind.Mood,
                JobLabel  = TranslateJob(job.Current),
                JobUrgent = job.Current == JobKind.Eat || job.Current == JobKind.Sleep,
                TopSkills = topSkills,
            });
        }
    }

    private static IReadOnlyList<(SkillKind Kind, int Level)> ComputeTopSkills(SkillsComponent skills)
    {
        if (!skills.Levels.IsValid || skills.Levels.Count == 0)
            return Array.Empty<(SkillKind, int)>();

        int count = skills.Levels.Count;
        var keysBuf = new SkillKind[count];
        var valuesBuf = new int[count];
        skills.Levels.Iterate(keysBuf, valuesBuf);

        var pairs = new (SkillKind Kind, int Level)[count];
        for (int i = 0; i < count; i++)
            pairs[i] = (keysBuf[i], valuesBuf[i]);

        for (int a = 1; a < pairs.Length; a++)
        {
            var cur = pairs[a];
            int b = a - 1;
            while (b >= 0 && CompareDesc(pairs[b], cur) > 0)
            {
                pairs[b + 1] = pairs[b];
                b--;
            }
            pairs[b + 1] = cur;
        }

        int take = Math.Min(TopSkillCount, pairs.Length);
        var result = new (SkillKind, int)[take];
        Array.Copy(pairs, result, take);
        return result;
    }

    private static int CompareDesc((SkillKind Kind, int Level) a, (SkillKind Kind, int Level) b)
    {
        if (a.Level != b.Level) return b.Level - a.Level;
        return (int)a.Kind - (int)b.Kind;
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
