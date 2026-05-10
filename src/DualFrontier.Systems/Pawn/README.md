# Pawn Systems

## Purpose
Pawn lifecycle systems: needs (hunger/sleep/rest), current job selection,
mood and breaks, social ties, and skill growth. See the GDD "Pawns" and
"Needs" sections.

## Dependencies
- `DualFrontier.Contracts` — attributes, the `IPawnBus` bus.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.Pawn` — `NeedsComponent`, `MindComponent`,
  `JobComponent`, `SkillsComponent`.
- `DualFrontier.Components.Shared` — `PositionComponent`, `HealthComponent`.
- `DualFrontier.Events.Pawn` — `MoodBreakEvent`, `SkillGainEvent`, ...

## Contents
- `NeedsSystem.cs` — SLOW: needs decay over time.
- `JobSystem.cs` — NORMAL: job selection and assignment to pawns.
- `MoodSystem.cs` — SLOW: recomputes mood from needs and health.
- `MovementSystem.cs` — NORMAL: pawn movement via A*,
  publishes `PawnMovedEvent`.
- `SkillSystem.cs` — NORMAL: skill growth from experience.

## Rules
- Domain bus — `nameof(IGameServices.Pawns)`.
- Writes / reads strictly per the `SystemAccess` attribute declaration.
- `JobSystem` is the only system that WRITES `JobComponent`.
- A mood break publishes `MoodBreakEvent` to the `Pawns` bus;
  `JobSystem` reacts by switching task.

## Usage examples
```csharp
// Inside JobSystem.Update:
foreach (var pawn in Query<NeedsComponent, SkillsComponent, PositionComponent>()) {
    ref var needs = ref GetComponent<NeedsComponent>(pawn);
    // TODO: select a job for the most urgent need
}
```

## TODO
- [x] Implement `NeedsSystem`: hunger/sleep decay over time.
- [x] Implement `JobSystem` (basic): job priority by need,
      `JobKind.Eat/Sleep/Idle`.
- [x] Implement `MoodSystem`: formula `mood = f(needs)`, transition into
      MoodBreak.
- [x] Implement `MovementSystem`: pawn movement along the A* route,
      publishes `PawnMovedEvent`.
- [ ] Implement `SkillSystem`: experience curve and decay (currently a stub).
- [ ] Wire event publication through `IGameServices` — `MoodSystem` currently
      has a stub instead of actually publishing `MoodBreakEvent`; systems do
      not yet have bus access.
