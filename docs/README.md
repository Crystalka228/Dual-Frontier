# Dual Frontier documentation

This directory documents a research artifact: a falsifiable test of
LLM-augmented systems engineering at one-person scale, operationalized
as a moddable simulation engine with declared invariants. The full
research framing is in the [root README](../README.md); the engine
exists to stress-test the methodology under non-trivial workload.

Three documents carry the primary research weight:
[METHODOLOGY](/docs/methodology/METHODOLOGY.md) describes the pipeline as
designed; [PIPELINE_METRICS](/docs/methodology/PIPELINE_METRICS.md) records the
operational data measured while running it;
[MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) describes the
capability-based mod isolation as an OS-style architecture;
[NATIVE_CORE_EXPERIMENT](/docs/reports/NATIVE_CORE_EXPERIMENT.md) records a
measured negative result with criterion reformulation. The remaining
documents below describe the engine that supports the claim.

Before reading the source layout, read [ARCHITECTURE](/docs/architecture/ARCHITECTURE.md)
and [CONTRACTS](/docs/architecture/CONTRACTS.md). Without them the assembly structure
looks excessive.

## Architecture

Technical documents describing the engine that stress-tests the methodology: layers, contracts, ECS core, event buses, multithreading, and isolation.

- [ARCHITECTURE](/docs/architecture/ARCHITECTURE.md) — the four layers, dependency rules, assembly diagram.
- [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) — **v1.5 LOCKED.** Mod system as a small operating system: capabilities, shared ALC, three-level contracts, bridge replacement, three-tier versioning, hot reload, threat model. Drives the M1–M10 migration plan in ROADMAP.
- [RUNTIME_ARCHITECTURE](/docs/architecture/RUNTIME_ARCHITECTURE.md) — **v1.0 LOCKED.** Pure-Vulkan + pure-Win32 2D runtime to replace the Godot DevKit production path. Ten foundation decisions (L1–L10), nine migration milestones (M9.0–M9.8), parallel-development cutover at M9.5, Godot deletion at M9.8. Domain layer preserved verbatim.
- [CONTRACTS](/docs/architecture/CONTRACTS.md) — marker interfaces, six domain buses, evolution and versioning.
- [ECS](/docs/architecture/ECS.md) — `World`, `EntityId`, `Component`, `SparseSet`, `Query`, `SystemBase`.
- [EVENT_BUS](/docs/architecture/EVENT_BUS.md) — domain buses, the two-step Intent→Granted/Refused model, batch processing.
- [THREADING](/docs/architecture/THREADING.md) — `DependencyGraph`, phases, tick rates, the ban on `async`.
- [ISOLATION](/docs/architecture/ISOLATION.md) — `SystemExecutionContext`, the isolation guard, DEBUG vs RELEASE, types of violations.
- [GODOT_INTEGRATION](/docs/architecture/GODOT_INTEGRATION.md) — `PresentationBridge`, `IRenderCommand`, `InputRouter`, the main thread.
- [VISUAL_ENGINE](/docs/architecture/VISUAL_ENGINE.md) — DevKit vs Native, the `IRenderer` / `ISceneLoader` / `IInputSource` contracts, the `.dfscene` format.

## Development

Documents required to reproduce the work or contribute to it: methodology, modding, performance, coding standards, and tests.

- [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — the pipeline, contracts as IPC between agents, verification cycle, threat model, economics, boundaries of applicability.
- [PIPELINE_METRICS](/docs/methodology/PIPELINE_METRICS.md) — pipeline configuration, empirical metrics, throughput, subscription headroom, reproducibility requirements. Companion to METHODOLOGY.
- [MODDING](/docs/architecture/MODDING.md) — `IMod`, `IModApi`, `AssemblyLoadContext`, `IModContract`, the mod manifest.
- [MOD_PIPELINE](/docs/architecture/MOD_PIPELINE.md) — the integration pipeline, `ContractValidator`, `ModRegistry`, atomicity.
- [PERFORMANCE](/docs/architecture/PERFORMANCE.md) — target metrics, profiling, hot paths, caches.
- [GPU_COMPUTE](/docs/architecture/GPU_COMPUTE.md) — **v2.0 LOCKED.** Field-based GPU compute as a foundational architectural capability (Domain A: mana / electricity / water / heat / sound / scent), with `ProjectileSystem` preserved as Domain B (entity-keyed bulk compute). K9 field storage abstraction + G0–G5 Vulkan compute roadmap.
- [CODING_STANDARDS](/docs/methodology/CODING_STANDARDS.md) — naming, file-scoped namespaces, nullable, member order.
- [TESTING_STRATEGY](/docs/methodology/TESTING_STRATEGY.md) — unit, integration, isolation, modding, performance.
- [DEVELOPMENT_HYGIENE](/docs/methodology/DEVELOPMENT_HYGIENE.md) — the hygiene checklist for every PR, the engine/game boundary, red flags.
- [NATIVE_CORE_EXPERIMENT](/docs/reports/NATIVE_CORE_EXPERIMENT.md) — the C++ core experiment, the P/Invoke boundary, benchmark results, plan for the batching API in Phase 9.
- [PERSISTENCE](../src/DualFrontier.Persistence/README.md) — save-compression algorithms: tile RLE, component quantization, entity range encoding, StringPool.

## Process

Work organization: phase order and acceptance criteria.

- [ROADMAP](./ROADMAP.md) — closed phases 0–4 plus the Mod-OS Migration (M0–M10), with acceptance criteria and what each phase unlocks next.
- [MAXIMUM_ENGINEERING_REFACTOR](/docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md) — **v1.0 RATIFIED.** Three-track discipline escalation brief: formal verification (Track A), type-theoretic architecture (Track B), methodology replication kit (Track C). Activation deferred per-track; each track requires its own brief at activation time.

## Learning materials

Self-teaching artifacts produced after each substantial phase closes (see [METHODOLOGY §4.5](/docs/methodology/METHODOLOGY.md)).

- [learning/PHASE_1](./learning/PHASE_1.md) — C# and multithreading through the lens of Core ECS: class vs struct, generics, attributes via reflection, nullable, `ThreadLocal`, race conditions, stack traces, tests as invariant proof. Includes a 14-day study path.

## Session logs

Verbatim logs of phase reviews and other key pipeline sessions. Not subject to post-hoc editing — they serve as an audit trail.

- [SESSION_PHASE_4_CLOSURE_REVIEW](./SESSION_PHASE_4_CLOSURE_REVIEW.md) — Phase 4 closure conducted by Opus 4.7: diagnostic validation, 6 architectural decisions, 17 new tests, 7 atomic commits. *(Russian-language audit trail; preserved verbatim per the i18n campaign rules.)*

## v0.2 addendum

Second-revision architecture additions: resource models, composite requests, feedback through tick lag, deterministic damage resolution, golem ownership transitions.

- [RESOURCE_MODELS](/docs/architecture/RESOURCE_MODELS.md) — Intent vs Lease, the choice rule, reserve-then-consume.
- [COMPOSITE_REQUESTS](/docs/architecture/COMPOSITE_REQUESTS.md) — two-phase commit for multi-bus requests, `CompositeResolutionSystem`.
- [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md) — `Mana[N-1]`, snapshots of the previous tick.
- [COMBO_RESOLUTION](/docs/architecture/COMBO_RESOLUTION.md) — `ComboResolutionSystem`, deterministic sort of `DamageIntent`.
- [OWNERSHIP_TRANSITION](/docs/architecture/OWNERSHIP_TRANSITION.md) — `GolemBondComponent` states, transition table.

## See also

- [ARCHITECTURE](/docs/architecture/ARCHITECTURE.md)
- [ROADMAP](./ROADMAP.md)
- [CONTRACTS](/docs/architecture/CONTRACTS.md)
