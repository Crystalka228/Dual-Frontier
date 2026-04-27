# GPU Compute

Research document on moving `ProjectileSystem`'s bulk computation to a GPU compute shader. Pins the decision made in Phase 3: at current loads the CPU implementation covers every scenario, and the threshold at which GPU pays off is deferred to Phase 5 alongside its stress test.

## Context

`ProjectileSystem` is marked `REALTIME` and runs on the CPU. At typical play load (10–20 pawns, ~100 simultaneous projectiles) the CPU fully copes: position updates, collision checks, and damage application fit within the frame budget without a noticeable share of the 16.6 ms.

GPU compute pays off when the number of simultaneous projectiles steadily exceeds ~500. Below that mark, dispatch and synchronization overhead eats the parallelism gain.

## Why not now

A CPU→GPU→CPU roundtrip at 60 FPS costs 0.5–2 ms per frame: uploading data into the buffer, dispatching the shader, asynchronous readback of the result. With fewer than 500 projectiles, this fixed overhead is larger than all the CPU work it would have replaced.

The frame budget is 16.6 ms at 60 FPS. Spending 0.5–2 ms of it on dispatch to compute a hundred objects is a worse trade than leaving the work on the CPU. The decision is deferred until profiling shows real degradation in the CPU implementation.

## Architectural pattern (when the time comes)

`ProjectileSystem` stays in the Domain layer as an ordinary system, subject to the isolation contracts and the scheduler's rules. The bulk computation of positions and collisions is delegated through the `IProjectileCompute` interface, declared in the Infrastructure layer.

Two implementations:

- `CpuProjectileCompute` — the default; direct computation in managed code.
- `GpuProjectileCompute` — compute shader + asynchronous readback with one-tick buffering.

Domain does not know which implementation is active: selection happens at startup via DI registration. The isolation contract is not broken — `ProjectileSystem` operates only against `IProjectileCompute`, not against a concrete backend.

## Switchover threshold

The threshold is pinned experimentally in the **"Battle of the Gods"** stress test: 500 mages × spell spam → ~5,000 simultaneous projectiles and ~50,000 collisions/sec. This scenario is chosen so that the CPU implementation reliably degrades and the GPU roundtrip pays off.

Migration to `GpuProjectileCompute` is justified when the scene steadily exceeds 500 projectiles. The exact number is refined by the Phase 5 benchmark results (see [PERFORMANCE.md](./PERFORMANCE.md)).

## Constraint

The GPU implementation introduces a 1-tick lag between logic and the visual representation of projectiles — the price of asynchronous readback. For a colony simulator at 60 FPS (16.6 ms per tick) this delay is invisible and acceptable. On a turn-based or hitbox-critical project the constraint would be a blocker.

## See also

- [PERFORMANCE](./PERFORMANCE.md)
- [THREADING](./THREADING.md)
- [ROADMAP](./ROADMAP.md)
