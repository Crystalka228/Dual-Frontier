---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-ARCHITECTURE_V2
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0.0"
next_review_due: 2027-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-ARCHITECTURE_V2
---
# Dual Frontier architecture

Dual Frontier is a colony simulation built as a native C++ kernel under a managed C# shell; this page is the orientation map — subsystem detail lives in the documents §6 points to.

> **Document class: authored-rework (current-truth candidate).** Successor of `docs/architecture/historical/ARCHITECTURE.md` (DOC-A-ARCHITECTURE, now SUPERSEDED). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`. Becomes the LOCKED authority upon Crystalka ratification per [FRAMEWORK.md](../governance/FRAMEWORK.md) §7; until then the predecessor remains the last-ratified reference and prevails on conflict.
> **Ratification checklist:** [ ] content spot-audit at ratification HEAD · [ ] lifecycle AUTHORED → LOCKED, version → 1.0.0 · [ ] `next_review_due` set · [ ] predecessor register rationale updated.

| Field | Value |
|---|---|
| Role | normative-current-candidate |
| Successor of | `docs/architecture/historical/ARCHITECTURE.md` (DOC-A-ARCHITECTURE, now SUPERSEDED) |
| Scope | Cross-cutting orientation: project census, layer boundaries, csproj-verified dependency graph, runtime thread shape, pointers to subsystem authorities |
| Non-goals | Scheduling/bus mechanics (SCHEDULER_ARCHITECTURE.md, THREADING.md, EVENT_BUS.md); mod capability grammar (MOD_OS_ARCHITECTURE.md, MODDING.md); contract evolution rules (CONTRACTS.md); domain-authority precedence beyond citing the matrix |
| Authority domains | Project census; inter-assembly reference legality; layer-boundary rule; Presentation/Domain crossing rule |
| Defers to | KERNEL_ARCHITECTURE.md → К-L law · SCHEDULER_ARCHITECTURE.md → native scheduler design · THREADING.md → managed dispatch · EVENT_BUS.md → bus mechanics · MOD_OS_ARCHITECTURE.md → mod lifecycle/ALC · CONTRACTS.md → contract rules · EXECUTION_AUTHORITY_MATRIX.md (AUTHORED draft) → cutover gates |

## §1 Layer map (the real assembly set)

Twelve managed `src/` projects (verified: `src/*/*.csproj` at HEAD, 12 files) plus the C++ kernel.

| Layer | Projects |
|---|---|
| Presentation | `Launcher` — window + render loop, single production renderer; `Runtime` — Vulkan substrate (`vulkan-1.dll` via pure P/Invoke) |
| Application | `Application` — GameBootstrap/GameLoop, Mod OS (loader, registry, capability model, fault handling), PresentationBridge command queue, display composition |
| Domain | `Systems` · `Components` · `Events` · `AI` · `Persistence` — game rules; multithreaded; renderer-agnostic |
| Infrastructure | `Core` — domain buses, scheduling dispatch facade; `Core.Interop` — P/Invoke bridge, NativeWorld handle, span/batch protocol; `Crypto.Future` — reserved FHE surface |
| Native kernel (C++20) | `Core.Native` — NativeWorld storage SSoT (К-L11), scheduler graph + wake registry (К-L12/К-L13), three-tier event bus (К-L15), GPU pipeline slots (К-L16) |

`DualFrontier.Contracts` sits beside the stack, referenced by every managed layer ([CONTRACTS.md](./CONTRACTS.md)). The Core row places "domain buses" inside managed Infrastructure — not a layering error, but it collides with a claim about *authority* over routing; §3 resolves that as one story.

## §2 Dependency rules

Verified against every `ProjectReference` in `src/*/*.csproj` at HEAD — all twelve rows hold unchanged.

| Assembly | References |
|---|---|
| Contracts | nothing (`Contracts.csproj` has no `ProjectReference`) |
| Core.Interop | Contracts; P/Invokes `DualFrontier.Core.Native.dll` (`BackgroundQueueInterop.cs:22`) |
| Components | Contracts, Core.Interop |
| Events | Contracts, Components |
| AI | Contracts, Components |
| Persistence | Contracts, Components |
| Core | Contracts, Core.Interop |
| Systems | Contracts, Core, Components, Events, AI (Core internals via `InternalsVisibleTo`) |
| Application | Contracts, Core, Core.Interop, Components, Events, Systems, AI |
| Runtime | Core.Interop |
| Launcher | Application, Runtime (Application internals via `InternalsVisibleTo`) |
| Crypto.Future | nothing (reserved surface) |
| Mods | Contracts **only** (mechanism below) |

**The mods row is a convention, not an enforced boundary.** *Compile-time:* every shipped mod's `.csproj` references Contracts only — verified for all seven (`mods/DualFrontier.Mod.Vanilla.Core/*.csproj:9` representative); nothing enforces this, it simply holds today (an analyzer candidate is deferred — ANALYZER_RULES.md). *Load-time:* the per-mod collectible `ModLoadContext` does **not** filter assembly names — its `Load` override returns a shared-ALC-cached assembly when one exists and otherwise `null`, deferring to the default context (`ModLoadContext.cs:29-54`; no blocklist — the predecessor's "refusal list" never existed in code, see MODDING.md). What the ALC actually provides is per-mod collectible isolation and shared-ALC type identity, not reference policing.

Every edge runs downward or laterally within a layer — never upward, never circular — sharper than "strictly downward." Two shapes beyond simple downward: lateral edges inside one layer (Events/AI/Persistence → Components, Domain; Core → Core.Interop, Infrastructure) and a layer-skip (`Runtime`, Presentation, references `Core.Interop`, Infrastructure, directly). The standing carve-out is Presentation → Domain: never direct; render-relevant domain events cross the `PresentationBridge` queue, dispatched render-side per frame (`RenderCommandDispatcher`).

### §2.1 InternalsVisibleTo census

Four `InternalsVisibleTo` grants cross project boundaries in production code (`*.Tests`/`*.Benchmarks` excluded). Counts are representative-anchor greps, order of magnitude only.

| Grant | Declared at | Representative consumer | Load-bearing? |
|---|---|---|---|
| Core → Systems | `Core.csproj:16` | none beyond `nameof` on public interfaces + one doc comment | **Unused — removable now** |
| Core → Application | `Core.csproj:17` | `GameServices` (`GameBootstrap.cs:79`, ~24), `SystemMetadata` (`GameBootstrap.cs:189`, ~14), `ParallelSystemScheduler` (`GameLoop.cs:38`, ~11), `DependencyGraph` (`GameBootstrap.cs:145`, ~8) | Heavily load-bearing |
| Core.Interop → Application | `Core.Interop.csproj:23` | internal `NativeMethods`, used by `ManagedBusBridge` + `SchedulerAdapter` (`SchedulerAdapter.cs:27`) | Load-bearing (one type) |
| Application → Launcher | `Application.csproj:20` | `GameLoop` (~9), `GameBootstrap` (~8) | Load-bearing |

`Core → Systems` is the one free cleanup here: nothing would stop compiling if it were deleted today.

## §3 Scheduling and event routing: one story, not two

Per К-L12 (KERNEL_ARCHITECTURE.md Part 0): native kernel scheduling is sovereign — "dependency graph construction, runqueue maintenance, wake-up dispatch, phase composition, parallelism scheduling, priority arbitration, resource quota enforcement." Per К-L15 (KERNEL_ARCHITECTURE.md Part 0): the native three-tier bus is sovereign event-routing authority — "native kernel owns sovereign event routing для kernel-space and cross-layer events... type registry, subscriber registry, payload dispatch, wake firing, tier-based delivery semantics all native authority." Both are LOCKED law today. Component storage is the one leg already achieved in production — `NativeWorld` has been the sole storage backend since the K8.3+K8.4 cutover (К-L11).

Scheduling and event routing have not cut over. Today, in production:

- **Scheduling.** Phases are planned by the managed `DependencyGraph` (`GameBootstrap.cs:145-148`) and executed via `Parallel.ForEach` (`ParallelSystemScheduler.cs:149`, MaxDegreeOfParallelism = ProcessorCount − 2, `:90`). Every Core system is *also* registered with the native `SystemGraph` (`GameBootstrap.cs:162-181`), with empty read/write component-id sets (`:173-174`) and a blanket Timer wake at rate 1 for every system (`:179`) — the native graph holds the systems but has no edges to decide with. The reverse-P/Invoke bridge that would let native dispatch drive managed execution (`ManagedSystemDispatcher.OnBatch`, `[UnmanagedCallersOnly]`, `ManagedSystemDispatcher.cs:75`; registered via `SchedulerAdapter.Register`, `SchedulerAdapter.cs:22`) is on disk and exercised only by `BatchedCallbackTests.cs` — zero production call sites, verified.
- **Event routing.** Every production event travels one of five managed `DomainEventBus` instances behind `IGameServices` (`GameServices.cs:14-113`, constructed `GameBootstrap.cs:79`). `BusFacade.UseNativeBusForDispatch` defaults `false` (`BusFacade.cs:49`); no production code constructs a `BusFacade`. The only live native-bus touchpoint in production is the Background-tier idle-slot drain each tick (`GameLoop.cs:120-128`, via `ManagedBusBridge.DrainBackgroundBatch`).

> **FENCED (target / planned — not current truth):** the cutover to native sovereignty is gated, not scheduled — no date, only conditions. Gate conditions, the equivalence-proof obligation, and the deletion triggers for the managed `DependencyGraph` and the per-bus `DomainEventBus` internals are specified in [EXECUTION_AUTHORITY_MATRIX.md](./EXECUTION_AUTHORITY_MATRIX.md) §3 (AUTHORED draft). Until those gates close, dual registration — every system in both graphs, every event representable on both bus vocabularies — is mandatory per that document's §3.3; a system or event visible to only one plane would make the equivalence gates untestable.

## §4 Governed vs ungoverned subsystems

- **`DualFrontier.AI`** is production-consumed but ungoverned. Only `Pathfinding` is: `MovementSystem.cs`, `GameBootstrap.cs`, and scenario factories `RandomPawnFactory.cs` / `ItemFactory.cs` reference it directly. `BehaviourTree` and `Jobs` have zero consumers outside `DualFrontier.AI` itself — a dormant surface shipping inside the production dependency graph (Systems → AI, Application → AI per §2). No architecture document owns `DualFrontier.AI`.
- **`DualFrontier.Persistence`** is production-orphaned. No `src/` or `mods/` project references it; its sole consumer anywhere is `tests/DualFrontier.Persistence.Tests`. The stub `SaveSystem` (`PipelineSlotInterop.cs:202`, `NotImplementedException`) is deliberate, but no architecture document records that the whole assembly ships disconnected from the running game.

## §5 Runtime shape

One process, several thread populations:

- **Render main thread** — Launcher's window/render loop; consumes `PresentationBridge` commands each frame.
- **Simulation thread** — `GameLoop` runs the fixed-step tick (`TargetTps = 30f`, `GameLoop.cs:29`) on a dedicated background thread (`:64-69`).
- **Managed scheduler workers** — `Parallel.ForEach` per phase, capped at `ProcessorCount − 2` (`ParallelSystemScheduler.cs:90`; see [THREADING.md](./THREADING.md)).
- **Native kernel thread pool** — sized to `std::thread::hardware_concurrency()` at native bootstrap (`native/DualFrontier.Core.Native/src/thread_pool.cpp:9`).

Mods load into per-mod `AssemblyLoadContext`s in the same process; a faulting mod is soft-unloaded without crashing the core ([MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md)).

## §6 Where the truth lives

| Doc | Owns |
|---|---|
| [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) | Part 0 К-L invariants, module purposes, two-phase bootstrap, interop patterns |
| [SCHEDULER_ARCHITECTURE.md](./SCHEDULER_ARCHITECTURE.md) | native scheduler design: work items, wake registry, cutover detail (successor of the former native-scheduler deliberation record) |
| [THREADING.md](./THREADING.md) | managed-side scheduling: dispatch facade, tick rates, async ban |
| [EVENT_BUS.md](./EVENT_BUS.md) | managed domain buses, native three-tier bus, intent and lease models |
| [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) | manifests, capability model, ALC lifecycle, unload chain, enforcement/fault model (folds in former MOD_PIPELINE.md + ISOLATION.md) |
| [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md) | GPU substrate: device/queue model, compute pipelines, shader toolchain |
| [ECS.md](./ECS.md) | storage access protocol |
| [CONTRACTS.md](./CONTRACTS.md) | marker interfaces, five-bus canon, mod-to-mod contracts, evolution rules |
| `docs/mechanics/` (e.g. [COMBO_RESOLUTION.md](../mechanics/COMBO_RESOLUTION.md)) | gameplay-mechanic design intent (combo resolution, composite requests, resource models, golem ownership, feedback loops) — Category J, non-normative for engine authority |
| [docs/ROADMAP.md](../ROADMAP.md) | forward state — architecture answers "what is", the roadmap alone answers "what's next" |

## Cross-references

| Doc | Relation | Note |
|---|---|---|
| KERNEL_ARCHITECTURE.md | defers-to | К-L law, bootstrap, interop patterns |
| SCHEDULER_ARCHITECTURE.md | defers-to | native scheduler design |
| THREADING.md | defers-to | managed dispatch facade, tick rates |
| EVENT_BUS.md | defers-to | bus delivery mechanics |
| MOD_OS_ARCHITECTURE.md | defers-to | mod lifecycle, capability, ALC |
| MODDING.md | cites | ALC resolution truth (no refusal list) + mod-author guide |
| VULKAN_SUBSTRATE.md | defers-to | GPU substrate |
| ECS.md | defers-to | storage access protocol |
| CONTRACTS.md | cites | sibling umbrella document |
| EXECUTION_AUTHORITY_MATRIX.md | constrains (AUTHORED draft) | §3 cutover gates and deletion triggers |
| docs/ROADMAP.md | cites | forward state |

## Amendment protocol

Tier 1, AUTHORED pending ratification. Amendment: surface the change to the owner (Crystalka); semver per FRAMEWORK.md §7.2 (PATCH correction, MINOR additive, MAJOR layer-map/dependency-rule inversion); propagate to citing documents.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.1 | 2026-07-17 | HALT-1-ratified review correction (CORPUS_CLOSURE_INVERSION_B, D1 R1-1): §3 К-L15 quotation restored to canon fidelity — "для" reinstated inside the verbatim quote (the rework had silently anglicized it). |
| **0.1.0** (AUTHORED, pending ratification) | 2026-07-15 | Corpus rework: one current/target story for scheduling + event routing (§3); corrected mods-row mechanism (§2); added IVT census (§2.1) and governed/ungoverned census (§4); repointed §6 to the post-rework corpus. |
| 1.0.0 | 2026-06-12 | Predecessor's last LOCKED version (Architecture Truth Cascade rewrite). See `historical/ARCHITECTURE.md` for its full change history. |
