---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-APPLICATION-LOOP
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-APPLICATION-LOOP
---
# Loop — Main game loop

## Purpose
Main simulation loop. `GameLoop` ticks systems through the `Core` scheduler
and maintains a fixed time step (simulation tick) independent of render FPS.
`FrameClock` is the time source for `delta` and current time.

## Dependencies
- `DualFrontier.Core` — `World`, `ParallelSystemScheduler`, `GameServices`
- `DualFrontier.Contracts` — `IGameServices`

## Contents
- `GameLoop.cs` — start/stop/tick the simulation.
- `FrameClock.cs` — time source (`Now`, `DeltaTime`).

## Rules
- The simulation ticks at a **fixed** step (target — 30 tps per TechArch).
  Render and input live at a different cadence — they do not drive `Tick`.
- The loop does NOT call Godot. Instead it places commands into
  `PresentationBridge`, which Godot reads in its `_Process`.

## Usage examples
```csharp
var clock = new FrameClock();
var loop  = new GameLoop(services, bridge);
loop.Start();
while (running)
{
    loop.Tick(clock.DeltaTime);
}
```

## TODO
- [x] Phase 1 — fixed accumulator-based tick (30 Hz).
- [x] Phase 1 — pause / speed multiplier (x1/x2/x3).
- [x] Phase 3 — `GameBootstrap` implemented; `NavGrid` and
      `MovementSystem` are wired in.
