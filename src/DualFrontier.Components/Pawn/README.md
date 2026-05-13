---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-COMPONENTS-PAWN
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-COMPONENTS-PAWN
---
# Pawn

## Purpose
Components specific to sapient pawns: needs (hunger, sleep, comfort), skills,
mood, current job, social ties. Pawn AI (see `DualFrontier.AI`) operates on top
of this data.

## Dependencies
- `DualFrontier.Contracts` — `IComponent`, `EntityId`.

## Contents
- `NeedsComponent.cs` — physiological needs (hunger, sleep, comfort).
- `SkillsComponent.cs` — skill levels keyed by `SkillKind`.
- `MindComponent.cs` — mood and break threshold.
- `JobComponent.cs` — current job and its goal.

## Rules
- `Dictionary` fields are initialized by systems when a pawn is created, not in
  the component's constructor (we want pooling without extra allocations).
- `Mood` and `MoodBreakThreshold` live in one structure so MoodSystem reads
  them atomically.

## Usage examples
```csharp
[SystemAccess(writes: new[] { typeof(NeedsComponent) })]
public class NeedsDecaySystem : SystemBase
{
    public override void Update(float delta)
    {
        foreach (var e in Query<NeedsComponent>())
        {
            var needs = GetComponent<NeedsComponent>(e);
            // needs.Hunger -= hungerRate * delta;
        }
    }
}
```

## TODO
- [ ] Define the `SkillKind` enum (Construction, Mining, Cooking, Combat, Magic …) — GDD.
- [ ] Define the `JobKind` enum (Idle, Build, Haul, Research, Fight …).
- [ ] Thoughts/character traits (`TraitsComponent`) — Phase 3.
