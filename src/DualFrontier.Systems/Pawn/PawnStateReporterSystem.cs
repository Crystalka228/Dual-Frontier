using System;
using System.Collections.Generic;
using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.ECS;
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
        // Pre-materialise membership sets so per-pawn reads stay branch-free
        // and avoid try/catch in the hot loop. The [SystemAccess] reads
        // declaration above keeps these Query calls inside the isolation
        // guard. Allocation: two short-lived HashSets per SLOW tick — at
        // 10 pawns and SLOW rate this is negligible.
        var identitySet = new HashSet<EntityId>();
        foreach (var e in Query<IdentityComponent>()) identitySet.Add(e);

        var skillsSet = new HashSet<EntityId>();
        foreach (var e in Query<SkillsComponent>()) skillsSet.Add(e);

        foreach (var pawn in Query<NeedsComponent, JobComponent>())
        {
            var needs = GetComponent<NeedsComponent>(pawn);
            var mind  = GetComponent<MindComponent>(pawn);
            var job   = GetComponent<JobComponent>(pawn);

            string name = identitySet.Contains(pawn)
                ? GetComponent<IdentityComponent>(pawn).Name.Resolve(NativeWorld) ?? string.Empty
                : string.Empty;

            IReadOnlyList<(SkillKind Kind, int Level)> topSkills =
                skillsSet.Contains(pawn)
                    ? ComputeTopSkills(GetComponent<SkillsComponent>(pawn))
                    : Array.Empty<(SkillKind, int)>();

            Services.Pawns.Publish(new PawnStateChangedEvent
            {
                PawnId    = pawn,
                Name      = name,
                Satiety   = needs.Satiety,
                Hydration = needs.Hydration,
                Sleep     = needs.Sleep,
                Comfort   = needs.Comfort,
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

        // Build a working array of all (kind, level) pairs, sort by level
        // descending (ties broken by enum order), take top N.
        // NativeMap.Iterate yields entries in sorted-by-key (memcmp) order,
        // not insertion order. Allocates two temp buffers — typical pawn
        // skill count is 13 (one per SkillKind), so heap pressure is low.
        int count = skills.Levels.Count;
        var keysBuf = new SkillKind[count];
        var valuesBuf = new int[count];
        skills.Levels.Iterate(keysBuf, valuesBuf);

        var pairs = new (SkillKind Kind, int Level)[count];
        for (int i = 0; i < count; i++)
            pairs[i] = (keysBuf[i], valuesBuf[i]);

        // Insertion sort — fixed size 13, allocation-free, O(n^2) trivial.
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

    /// <summary>
    /// Comparer: descending by level, ascending by SkillKind enum value
    /// for ties. Returns negative if <paramref name="a"/> should come
    /// BEFORE <paramref name="b"/> in the sorted (top-first) order;
    /// positive otherwise.
    /// </summary>
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
