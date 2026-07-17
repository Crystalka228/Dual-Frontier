# Jobs

## Purpose
A job is a "task" the pawn executes step by step: walk, pick up, process, drop.
JobSystem from `DualFrontier.Systems/Pawn/` creates a job and ticks it to
completion. The job itself does not write components — it returns a status,
and the system applies component changes and event publication.

## Dependencies
- `DualFrontier.Contracts` — `EntityId`.
- `DualFrontier.Components` — reads pawn data through `Tick` arguments
  (e.g., position).

## Contents
- `IJob.cs` — job interface + the `JobStatus` enum.
- `JobHaul.cs` — moves a stack between storages.
- `JobCraft.cs` — work at a workbench.
- `JobCast.cs` — spell casting.
- `JobMeditate.cs` — mage progression through meditation.
- `JobGolemCommand.cs` — execution of a golem order.

## Rules
- Jobs are stateful: progress and the current step live inside.
- Jobs are synchronous: `Tick` returns a status in O(1) compute, no async.
- Jobs MUST NOT write components directly — they return a result or publish
  an intent (ammo, craft, spell) through arguments.
- Jobs can be aborted: they MUST cleanly roll back what they started.

## Usage examples
```csharp
var job = new JobHaul(/* args */);
job.Start();
while (job.Tick(delta) == JobStatus.Running) { /* next frame */ }
```

## TODO
- [ ] Implement every job.
- [ ] Add job serialization for save files (through `Application`).
- [ ] Write Abort unit tests for every job.

---
register_id: DOC-F-SRC-AI-JOBS
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-04-XX
last_modified: 2026-04-XX
content_language: en
next_review_due: null
title: AI Jobs submodule
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
