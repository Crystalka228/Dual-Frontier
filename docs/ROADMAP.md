# Roadmap

The Dual Frontier implementation has reorganised after the closure of Phase 4. The original Phase 5 (Combat), Phase 6 (Magic), and Phase 7 (World) are dissolved into a broader **Mod-OS Migration** (M1–M10) that simultaneously builds the modding kernel and ships gameplay content as vanilla mods. The architecture for this migration is specified in [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) v1.3 LOCKED; this roadmap is the execution sequence derived from it.

The reorganisation follows the project's central methodological claim: **engine and methodology are the main research result, the game is a test case for the hypothesis** ([METHODOLOGY](./METHODOLOGY.md)). By implementing combat, magic, and world content through the modding system rather than alongside it, we make every gameplay feature also a test of the modding architecture. A combat system that ships as a vanilla mod is the strongest possible falsifiable claim that the contract surface for mods is complete.

Phases do not overlap in code ownership. Closed phases retain their entries here as historical record; current work is the Mod-OS Migration; Phase 9 (Native Runtime) remains the post-launch endpoint.

## Status overview

*Updated: 2026-05-01 (M6 closed — M6.1, M6.2 done; M7 next).*

| Phase | Status | Tests | Notes |
|---|---|---|---|
| Phase 0 — Contracts | ✅ Closed | — | Public surface in `DualFrontier.Contracts` |
| Phase 1 — Core | ✅ Closed | 60/60 | ECS core, scheduler, domain buses, `[Deferred]`/`[Immediate]` |
| Phase 2 — Verification | ✅ Closed | 11/11 | Isolation guard, ContractValidator |
| Phase 3 — Pawns | ✅ Closed | 1/1 | A* pathfinding, Godot bridge, MoodSystem publishing |
| Phase 3.5 — Godot DevKit | ✅ Closed | — | DevKit plugin, `.dfscene` export, F5 launch |
| Phase 4 — Economy | ✅ Closed | 6/6 | Inventory, ElectricGrid, Converter at 30%, HUD |
| Persistence (scaffold) | ✅ Closed | 4/4 | TileEncoder/RLE, ComponentEncoder, EntityEncoder, StringPool |
| **M0 — Mod-OS Phase 0** | ✅ Closed | — | `MOD_OS_ARCHITECTURE` v1.0 LOCKED |
| **M1 — Manifest v2** | ✅ Closed | added | `VersionConstraint`, `ModDependency`, `ManifestCapabilities`, `ModManifest` v2, `ManifestParser`, full `ValidationErrorKind` set |
| **M2 — IModApi v2** | ✅ Closed | added | Real `Publish`/`Subscribe`, `GetKernelCapabilities`, `GetOwnManifest`, `Log`, `RestrictedModApi` v2 |
| **M3 — Capability model** | ✅ Closed | added (`KernelCapabilityRegistryTests`, `RestrictedModApiV2Tests`, `CapabilityValidationTests`, `ProductionComponentCapabilityTests`) | M3.1 `KernelCapabilityRegistry` + `[ModAccessible]` opt-in; M3.2 capability-enforcing `RestrictedModApi` (Publish/Subscribe runtime check, hybrid per `MOD_OS_ARCHITECTURE` §3.6 v1.2); M3.3 `[ModCapabilities]` + load-time cross-check (`ContractValidator` Phases C+D); M3.4 deferred |
| M3.4 — CI capability analyzer | ⏸ Deferred | — | Roslyn analyzer for `[ModCapabilities]` honesty (D-2 hybrid completion); unblocked when first external mod author appears |
| **M4 — Shared ALC** | ✅ Closed | added (`CrossAlcTypeIdentityTests`, `SharedAssemblyResolutionTests`, `ContractTypeInRegularModTests`, `SharedModComplianceTests`) | M4.1 `SharedModLoadContext` + two-pass loader + cross-ALC type identity; M4.2 `ContractValidator` Phase E enforces D-4 (no contract types in regular mods); M4.3 D-5 LOCKED shared-mod cycle detection + Phase F enforces §5.2 shared-mod compliance |
| **M5 — Version constraints** | ✅ Closed | added (`RegularModTopologicalSortTests`, `DependencyPresenceTests`, `M51PipelineIntegrationTests`, `PhaseAModernizationTests`, `PhaseGInterModVersionTests`, `M52IntegrationTests`) | M5.1 pipeline regular-mod toposort + dependency presence (`MissingDependency` / optional warning); M5.2 `ContractValidator` Phase A v1/v2 dual-path modernization + Phase G inter-mod version check; cascade-failure semantics ratified as deliberate accumulation (per §8.7) |
| **M6 — Bridge replacement** | ✅ Closed | added (`PhaseHBridgeReplacementTests`, `Phase5BridgeAnnotationsTests`, `CollectReplacedFqnsTests`, `M62IntegrationTests`) | M6.1 `[BridgeImplementation(Replaceable)]` + Phase 5 combat stubs annotated + `ContractValidator` Phase H bridge replacement validation; M6.2 `ModIntegrationPipeline` skip-on-replace graph build + integration tests across all §7.5 scenarios |
| M7 — Hot reload | ⏭ Pending | — | Menu-driven, paused-only |
| M8 — Vanilla skeletons | ⏭ Pending | — | Five empty mod assemblies |
| M9 — Vanilla.Combat | ⏭ Pending | — | Absorbs original Phase 5 scope |
| M10 — Remaining vanilla | ⏭ Pending | — | Magic, Inventory, Pawn, World — incremental |
| Phase 9 — Native Runtime | ⏭ Post-launch | — | Separate large project |

**Engine snapshot:** Phases 0–4 closed at 82/82 tests. M1 added Manifest/Parser test suites (`VersionConstraintTests`, `ModDependencyTests`, `ManifestCapabilitiesTests`, `ModManifestV2Tests`, `ManifestParserTests`). M2 added `RestrictedModApiV2Tests`. M3 added `KernelCapabilityRegistryTests`, `CapabilityValidationTests`, and `ProductionComponentCapabilityTests` (260/260 at M3 closure). M4 added `CrossAlcTypeIdentityTests` and `SharedAssemblyResolutionTests` (M4.1), `ContractTypeInRegularModTests` (M4.2), and `SharedModComplianceTests` (M4.3). M5 added `RegularModTopologicalSortTests`, `DependencyPresenceTests`, and `M51PipelineIntegrationTests` (M5.1) plus `PhaseAModernizationTests`, `PhaseGInterModVersionTests`, and `M52IntegrationTests` (M5.2). M6 added `PhaseHBridgeReplacementTests` and `Phase5BridgeAnnotationsTests` (M6.1) plus `CollectReplacedFqnsTests` and `M62IntegrationTests` (M6.2). **Total at M6 closure: 338/338 passed** (verify with `dotnet test` against the current solution). The structural foundation laid in Phases 0–4 is the entire prerequisite for the Mod-OS Migration; nothing in M1–M7 requires touching the ECS core, the scheduler, or the bus contracts (`IGameServices`).

---

## Closed phases

The following phases are retained for historical record. Each has been verified by its acceptance tests and is no longer subject to active edits except for translation, refactor, or i18n work.

### ✅ Phase 0 — Contracts (closed)

Goal achieved: every public interface and attribute is pinned in `DualFrontier.Contracts`. The assembly references only `System.*` and serves as the only contract surface a mod sees. Build is clean; every public type carries English XML documentation.

### ✅ Phase 1 — Core (closed)

Result: a working ECS core with multithreaded scheduler and domain buses. `[Deferred]` and `[Immediate]` semantics implemented in `DomainEventBus` (closed in v0.3 as Phase 4 debt). `IntentBatcher`, `DependencyGraph`, `ParallelSystemScheduler`, `TickScheduler` all operational. Main simulation loop (`FrameClock`, `GameLoop`) supports pause and 1×/2×/3× speed.

Tests: 60/60 covering `ComponentStore`, `DependencyGraph`, `DomainEventBus`, parallel execution.

### ✅ Phase 2 — Verification (closed)

Result: structural guarantees provably enforced. Isolation guard catches undeclared component access, direct system access, `GetSystem` calls. `ModIsolationException` raised when a mod assembly tries to load `DualFrontier.Core.dll`. `ContractValidator` returns precise messages on write-write conflicts between mods. `ModIntegrationPipeline.Apply` is atomic — failure at any step rolls back to the previous scheduler phase list.

Tests: 11/11 isolation tests pass in DEBUG; critical subset passes in RELEASE.

**Carried debt (now part of M7):** `AssemblyLoadContext` WeakReference unload tests are not yet implemented. Originally a Phase 2 backlog item, they become a hard requirement when M7 lands hot reload — the unload chain cannot ship without proven ALC release.

### ✅ Phase 3 — Pawns (closed)

Result: a living colony. Pawns walk the map via A* pathfinding (2000-iteration cap, no cache). Needs decay, mood recomputes, jobs assigned by priority through event buses. `JobKind.Eat/Sleep/Idle` works through `JobSystem`. `MovementSystem` publishes `PawnMovedEvent` on every step. `NavGrid` initialises from `GameBootstrap` (50×50, 50 obstacles). `MoodSystem` publishes `MoodBreakEvent` via `Services.Pawns.Publish` on threshold transition (originally a Phase 3 backlog item, closed silently and now ratified here).

Tests: 1/1 (high-level integration); the system-level coverage moves to M9/M10 when the systems migrate to vanilla mods.

**Carried debt (now part of M10):** `SocialSystem` and `SkillSystem` exist as `[BridgeImplementation(Phase = 3)]` stubs in `DualFrontier.Systems.Pawn`. They will move to `Vanilla.Pawn` mod where they get real implementations.

### ✅ Phase 3.5 — Godot DevKit (closed)

Result: Godot works as both editor and temporary runtime. `DfDevKitPlugin`, `SceneExporter`, `EntityExporter` enable scene authoring and `.dfscene` export. `IRenderer`, `ISceneLoader`, `IInputSource` contracts exist with both Godot and Native backends. F5 in Godot starts the game with the loaded scene.

Architectural context: [VISUAL_ENGINE](./VISUAL_ENGINE.md). The decoupling work here unblocks Phase 9 (Native Runtime) without committing to its timeline.

### ✅ Phase 4 — Economy (closed)

Result: storages, crafting workbenches, the power grid, the converter. `InventorySystem` with `_freeSlotCache` and `_cacheDirty`-driven batching. `HaulSystem` uses per-Update reservation set to prevent same-tick double-allocation. `ElectricGridSystem` distributes watts by priority on `IPowerBus`. `ConverterSystem` runs at 30 % efficiency, publishing `[Deferred] ConverterPowerOutputEvent` to break the component cycle with ElectricGrid. Grimdark HUD via `GameHUD`, `ColonyPanel`, `PawnDetail`.

Tests: 6/6 covering inventory deferred mutation, ElectricGrid overload, Converter efficiency.

**v0.3 architectural fixes (Phase 4 debt closure):** all items closed.

- `[Deferred]`/`[Immediate]` delivery in `DomainEventBus`.
- `IPowerBus` added to `IGameServices` (non-breaking).
- Inventory events marked `[Deferred]`; `HaulSystem.writes=[]` isolation preserved through subscriber context capture.
- `ConverterSystem.writes=[]`; output via `[Deferred] ConverterPowerOutputEvent`.
- All `OnInitialize` `NotImplementedException` stubs replaced with empty body + `[BridgeImplementation(Phase = N)]`.
- `HaulSystem` `return` after failed `TryFindHaul` replaced with `continue`; `DomainEventBus.Subscribe` TOCTOU fixed via `GetOrAdd`.

### ✅ Persistence scaffold (closed)

Result: save-compression scaffold complete. `TileEncoder` (RLE), `ComponentEncoder` (quantisation), `EntityEncoder` (range encoding), `StringPool`. Snapshot types in `DualFrontier.Persistence.Snapshots`. No Godot dependency.

Tests: 4/4 covering encoder round-trips and string pool semantics.

The save-game compatibility policy when a mod is missing (mod-OS decision D-6) targets this layer for the component-stripping logic; that work is out of M0–M10 scope and tracked separately.

---

## 🔨 Mod-OS Migration (M0–M10)

The migration sequence is derived from `MOD_OS_ARCHITECTURE` v1.3 §11. Each M-phase has a clear output artifact, acceptance criteria, and the set of architectural decisions (D-N) it consumes. Phases run in strict order — M(N+1) depends on M(N) — except where noted.

### ✅ M0 — Mod-OS Phase 0 (closed)

Goal achieved: architectural specification produced, reviewed, and locked.

**Output:** [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) v1.0 LOCKED. All twelve decisions resolved (five strategic + seven detail D-1 through D-7). Implementation phases unblocked.

**No code changes.** This phase exists to make every subsequent code change traceable to a documented decision.

---

### ✅ M1 — Manifest v2 schema and parser (closed)

Goal achieved: `ModManifest` extended to the v2 schema specified in `MOD_OS_ARCHITECTURE` §2, with a backward-compatible JSON parser that accepts existing v1 manifests unchanged.

**Consumes decisions:** structural locks 1–5 (manifest fields for `replaces`, `hotReload`, three-tier versioning).

**What we implement**

- `ModManifest` v2 fields: `Kind` (`regular` | `shared`), `ApiVersion` (string with caret support), `HotReload` (bool, default `false`), `Replaces` (`IReadOnlyList<string>`), `Capabilities` (`ManifestCapabilities` record with `Required` and `Provided` lists).
- `ModDependency` record replacing the current `IReadOnlyList<string> Dependencies` — each entry carries `Id` and `Version` (constraint string).
- JSON parser that reads v2 fields, falls back to v1 defaults when fields are absent.
- New `ValidationErrorKind` entries: `MissingCapability`, `BridgeReplacementConflict`, `ProtectedSystemReplacement`, `UnknownSystemReplacement`, `IncompatibleVersion`, `SharedModWithEntryPoint`, `ContractTypeInRegularMod`.
- `ContractsVersion.Parse` extension (or new `VersionConstraint` struct) to handle `"^X.Y.Z"`. Tilde explicitly rejected with a `FormatException` directing the author to caret.

**Acceptance criteria**

- Existing `ExampleMod`'s `mod.manifest.json` loads unchanged (backward compatibility).
- A v2 manifest with `kind`, `apiVersion`, `hotReload`, `dependencies` (with versions), `replaces`, `capabilities` parses to a fully populated `ModManifest`.
- Caret-prefixed versions parse correctly; tilde produces a clean `FormatException`.
- Manifest with malformed capability strings rejected at parse time with a clear message.
- All seven new `ValidationErrorKind` entries are exercised by unit tests with their canonical error messages.

**Unblocks:** every subsequent M-phase. The manifest is the single input every loader stage reads from.

---

### ✅ M2 — IModApi v2: Publish and Subscribe (closed)

Goal achieved: replaced the no-op semantics of `RestrictedModApi.Publish` and `Subscribe` with real bus routing through `ModBusRouter`, while preserving the v1 `IModApi` signature for backward compatibility.

**Consumes decisions:** D-3 (cast prevention via structural barrier — no analyzer, no runtime check).

**What we implement**

- `RestrictedModApi.Publish<T>` resolves the target bus from the event type (via a marker attribute on the event record — `[Combat]`, `[Magic]`, etc.) and dispatches through the kernel's `IGameServices`. `[Deferred]` and `[Immediate]` semantics honored.
- `RestrictedModApi.Subscribe<T>` registers the handler on the appropriate bus, wrapping it to capture the calling mod's `SystemExecutionContext`. The wrapper is recorded for `UnsubscribeAll`.
- New `IModApi.GetKernelCapabilities()` accessor returning the kernel's frozen capability set (used by mods for self-introspection).
- New `IModApi.GetOwnManifest()` accessor.
- New `IModApi.Log(LogLevel level, string message)` — replaces ad-hoc `Console.WriteLine` in mod code, prefixes log lines with the mod id.
- `RestrictedModApi` declared `internal sealed` (per D-3 lock); confirm `DualFrontier.Application` is not in the resolution path of any mod ALC.

**Acceptance criteria**

- A mod publishes `DamageEvent`; a kernel system subscribed to `IGameServices.Combat` receives it on the next tick (deferred) or in the same tick (immediate).
- A mod subscribes to `MoodBreakEvent`; the kernel `MoodSystem` publishes; the mod's handler runs under the mod's `SystemExecutionContext`.
- On mod unload, `RestrictedModApi.UnsubscribeAll` removes every wrapper from the bus dispatcher; subsequent kernel publishes do not invoke unloaded handlers.
- Capability check at `Subscribe` and `Publish` time still raises `CapabilityViolationException` (the actual capability registry lands in M3; here we stub the registry with a "permit-all" implementation behind a feature flag).
- `[Deferred]` events from mod systems queue correctly and deliver on the next phase boundary.

**Unblocks:** M3 (capability model needs working publish/subscribe to enforce against), M9, M10 (vanilla mods need real bus operations).

---

### ✅ M3 — Capability model (closed)

Goal: implement the capability registry, the `[ModAccessible]` and `[ModCapabilities]` attributes, and the load-time cross-check between manifest, code, and kernel-provided capabilities.

**Sub-phase status:**

- **M3.1 ✅ Closed.** Acceptance: `KernelCapabilityRegistry` scans `DualFrontier.Components` + `DualFrontier.Events` assemblies (commit `a73669f`); production components annotated `[ModAccessible]` per D-1 (commit `f91f065`); `KernelCapabilityRegistryTests` + `ProductionComponentCapabilityTests` (commit `b92fa66`) verify both registry mechanics and the §2.1 example manifest token resolution.
- **M3.2 ✅ Closed.** Acceptance: `RestrictedModApi.EnforceCapability` raises `CapabilityViolationException` on `Publish<T>` / `Subscribe<T>` when the manifest does not declare the capability; per `MOD_OS_ARCHITECTURE` §4.2 / §4.3 v1.2 hybrid enforcement (ratified in v1.2 §3.6). `RegisterSystem` is locked as v1 semantics unchanged per §4.1 — no capability check applies.
- **M3.3 ✅ Closed.** Acceptance: `ContractValidator` Phase C (capability satisfiability via kernel + listed dependencies) and Phase D (`[ModCapabilities]` × manifest cross-check) implemented; `CapabilityValidationTests` covers both phases.
- **M3.4 ⏸ Deferred.** CI Roslyn analyzer for `[ModCapabilities]` honesty (D-2 hybrid completion, see `MOD_OS_ARCHITECTURE` §3.8). Unblocked when the first external (non-vanilla) mod author appears. Rationale: runtime `CapabilityViolationException` already catches dishonest attribute declarations; the analyzer is developer-experience tooling for early feedback before publication, not a runtime safety boundary.

**Consumes decisions:** D-1 (curated opt-in `[ModAccessible]`), D-2 (hybrid attribute + CI static analysis).

**What we implement**

- `[ModAccessible(Read = bool, Write = bool)]` attribute applied to public components in `DualFrontier.Components`. Per D-1, every component is invisible to mods until annotated. Components touched by Phase 5 — `WeaponComponent`, `ArmorComponent`, `AmmoComponent`, `ShieldComponent`, `HealthComponent` — get annotated as part of M3 in preparation for M9.
- `[ModCapabilities("publish:DualFrontier.Events.Combat.DamageEvent", "subscribe:..." )]` attribute on mod-supplied systems. Per D-2, this is the load-time declaration; the loader cross-checks it against the manifest's `capabilities.required`.
- `KernelCapabilityRegistry` — built at startup by reflecting over `[ModAccessible]` types and over `IEvent` types in `DualFrontier.Events.*`. Frozen after build; exposed via `IModApi.GetKernelCapabilities`.
- Capability-enforcing `RestrictedModApi`: `Publish` and `Subscribe` consult a per-mod capability set (built from manifest + provided-capabilities of dependency mods) and throw `CapabilityViolationException` on mismatch.
- CI-level static analyzer (Roslyn, separate package): scans mod assemblies for actual `Publish`/`Subscribe` call sites and verifies `[ModCapabilities]` declarations are honest. Runs in mod-publication CI, not at game load time. **(Deferred — see M3.4 above.)**

**Acceptance criteria**

- A mod declares `kernel.read:HealthComponent` in manifest; HealthComponent is annotated `[ModAccessible(Read = true)]`; load succeeds.
- A mod declares `kernel.read:MindComponent` in manifest; MindComponent is **not** annotated; load fails with `MissingCapability` naming the component.
- A mod's system has `[ModCapabilities("publish:DamageEvent")]` but the manifest does not list `kernel.publish:DualFrontier.Events.Combat.DamageEvent` in `capabilities.required`; load fails with `MissingCapability`.
- A mod's system publishes an event for which the system declares `[ModCapabilities]` but the manifest is correctly populated; load succeeds, publish reaches the bus.
- *(M3.4 deferred)* The CI analyzer detects a `Services.Combat.Publish<DamageEvent>` call in code that has `[ModCapabilities]` not listing `publish:DamageEvent`; CI fails.

**Unblocks:** M6 (bridge replacement validates capability of replacing system), M9, M10 (vanilla mods declare capabilities honestly).

---

### ✅ M4 — Shared ALC and shared mod kind (closed)

Goal achieved: shared `AssemblyLoadContext`, two-pass mod loading, cross-ALC type identity, and the three M4 enforcement points (D-4 contract types in regular mods, D-5 shared-mod cycles, §5.2 shared-mod compliance) are operational. Migration unblocked through M5.

**Sub-phase status:**

- **M4.1 ✅ Closed.** Acceptance: `SharedModLoadContext` (singleton, non-collectible, `Resolving` delegates `DualFrontier.*` to default ALC) and the two-pass loader (shared mods first into the shared ALC, then regular mods whose `ModLoadContext` delegates to that shared ALC for cross-mod type references) implemented. Cross-ALC type identity verified by `CrossAlcTypeIdentityTests`; assembly resolution semantics verified by `SharedAssemblyResolutionTests` (commits `0a3a858`, `cf14edb`, `56772fc`, `e5e0e30`, `1ec1354`, `cdb48f0`).
- **M4.2 ✅ Closed.** Acceptance: `ContractValidator` Phase E enforces D-4 — every regular mod's assemblies are scanned, and any exported type implementing `IEvent` or `IModContract` produces a typed `ContractTypeInRegularMod` error before `IMod.Initialize` runs (commits `68cb693`, `14e1dd0`, `c410add`); covered by `ContractTypeInRegularModTests`.
- **M4.3 ✅ Closed.** Acceptance: `ModIntegrationPipeline.TopoSortSharedMods` (Kahn's algorithm) detects D-5 LOCKED shared-mod cycles between manifest parse and shared-mod load — cyclic mods never reach assembly load and surface as `CyclicDependency` errors naming the affected mod set. `ContractValidator` Phase F enforces §5.2 shared-mod compliance — non-empty `entryAssembly`/`entryType`/`replaces` and any `IMod` implementation in the assembly each produce a typed `SharedModWithEntryPoint` error; `ModLoader.LoadSharedMod`'s M4.1 defensive IMod throw is removed in favour of the validator's typed accumulation (commits `df582d3`, `e0151d8`, `d628692`, `b71e9e2`, `90f8012`); covered by `SharedModComplianceTests`.

**Consumes decisions:** D-4 (active scan, reject contract types in regular mods — M4.2), D-5 (forbid shared-mod cycles — M4.3), §5.2 shared-mod compliance (M4.3).

**Acceptance criteria met (per `MOD_OS_ARCHITECTURE` §11.1):**

- A shared mod with `record FooEvent : IEvent` loads into the shared ALC (M4.1).
- A regular mod (different ALC) `Subscribe<FooEvent>(handler)`; another regular mod `Publish(new FooEvent(...))`; the subscriber's handler runs — cross-ALC type identity preserved (M4.1).
- A regular mod containing `record BadEvent : IEvent` is rejected at load with `ContractTypeInRegularMod` (M4.2).
- A shared mod with circular dependency on another shared mod is rejected with `CyclicDependency` (M4.3).
- A shared mod manifest with non-empty `EntryAssembly` is rejected with `SharedModWithEntryPoint` (M4.3).
- A shared mod assembly containing an `IMod` implementation is rejected with `SharedModWithEntryPoint` (M4.3).

**Unblocks:** M5 (dependency resolution with versions needs the shared/regular distinction), M9, M10 (vanilla.core shared mod precedes the slice mods).

---

### ✅ M5 — Inter-mod dependency resolution with caret syntax (closed)

Goal achieved: `ModIntegrationPipeline` resolves regular-mod dependencies using the three-tier SemVer model from `MOD_OS_ARCHITECTURE` §8. Regular-mod topological sort, dependency presence check, and validator-level inter-mod version check are operational; cascade-failure semantics ratified as deliberate accumulation per §8.7.

**Sub-phase status:**

- **M5.1 ✅ Closed.** Acceptance: regular-mod topological sort via `TopoSortByPredicate` (extracted from `TopoSortSharedMods`) detects regular-mod cycles before assembly load; dependency presence check produces `MissingDependency` for required deps and a `ValidationWarning` for optional deps; pass `[0.6]` in `ModIntegrationPipeline.Apply` runs between shared-mod cycle detection and shared-mod load; `PipelineResult.Warnings` field flows through every return path. Commits: `fffd785` (extract `TopoSortByPredicate` from `TopoSortSharedMods`), `13400bb` (add `TopoSortRegularMods` + `CheckDependencyPresence` helpers), `a3968f4` (wire regular-mod toposort and dep presence into pipeline), `bab4d85` (integration tests). Tests: `RegularModTopologicalSortTests` (6), `DependencyPresenceTests` (4), `M51PipelineIntegrationTests` (4).

- **M5.2 ✅ Closed.** Acceptance: `ContractValidator` Phase A modernized for v1/v2 dual-path — legacy `IncompatibleContractsVersion` retained for v1 manifests, new `IncompatibleVersion` emitted for v2 manifests through the full `VersionConstraint` pipeline; `ContractValidator` Phase G inter-mod dependency version check produces `IncompatibleVersion` when a regular mod's `dependencies[i].version` constraint is unsatisfied by the depended-on mod's actual version; `ContractValidator` class XML-doc updated to "seven-phase validator" (Phases A–G). Commits: `50efe9d` (Phase A modernization for v2 manifests via `VersionConstraint` pipeline), `f8f18ee` (Phase G inter-mod dependency version check), `376be7e` (integration tests). Tests: `PhaseAModernizationTests` (6), `PhaseGInterModVersionTests` (7), `M52IntegrationTests` (3).

**Cascade-failure semantics — accumulation, not skip.** Per §8.7 of [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md), "the failed set is presented to the user; the success set proceeds to load." M5 implementation interprets this as: when mod A depends on mod B and B fails its own validation (any phase), A is **not** silently dropped — A's own validation runs to completion, and any independent errors A produces also surface. Both errors appear in `result.Errors`. This matches the existing pipeline accumulation pattern (Phases B / C / D / E / F / G all accumulate without short-circuit) and gives mod authors maximum diagnostic information per `Apply` call. Demonstrated by:

- Validator-level: `Mod_WithCascadeFailure_BothErrorsReportedNotSkipped` (`PhaseGInterModVersionTests`).
- Pipeline-level: `Apply_WithCascadeFailure_SurfacesBothErrors` (`M52IntegrationTests`).

This is a deliberate interpretation of §8.7 wording "cascade-fail," registered here per [METHODOLOGY](./METHODOLOGY.md)'s "no improvisation" rule. If the interpretation needs revision, escalate via §12 ratification process.

**Consumes decisions:** strategic lock 5 (caret syntax for inter-mod deps).

**Acceptance criteria met (per `MOD_OS_ARCHITECTURE` §11.1):**

- Mod A v1.0.0, Mod B requires `^1.0.0` of A: load succeeds.
- Mod A v2.0.0, Mod B requires `^1.0.0` of A: Mod B rejected with `IncompatibleVersion`.
- Mod A v1.5.3, Mod B requires `^1.0.0` of A: load succeeds (caret matches any 1.x).
- Mod A v1.2.0, Mod B requires `1.0.0` (exact): Mod B rejected with `IncompatibleVersion`.
- Topological sort detects circular dependency between regular mods; both rejected with `CyclicDependency`.
- A mod requires kernel `apiVersion: "^2.0.0"` against `ContractsVersion.Current = 1.0.0`: rejected with `IncompatibleVersion`.

**Unblocks:** M6 (vanilla mods have inter-slice dependencies), M8 onwards.

---

### ✅ M6 — Bridge replacement via `replaces` (closed)

Goal achieved: explicit bridge replacement is operational. Vanilla mods can now supersede `[BridgeImplementation]` kernel systems without write-write conflict; the validator catches every §7.5 misuse before assembly load; the pipeline graph build skips replaced kernel systems while still registering mod-supplied replacements.

**Sub-phase status:**

- **M6.1 ✅ Closed.** Acceptance: `[BridgeImplementation]` extended with `Replaceable` bool (per `MOD_OS_ARCHITECTURE` §7.4); Phase 5 combat stubs annotated `[BridgeImplementation(Phase = 5, Replaceable = true)]` (`CombatSystem`, `DamageSystem`, `ProjectileSystem`, `ShieldSystem`, `StatusEffectSystem`, `ComboResolutionSystem`, `CompositeResolutionSystem`); `ContractValidator` Phase H bridge replacement validation emits `BridgeReplacementConflict` (two mods replace same FQN), `ProtectedSystemReplacement` (mod replaces `Replaceable = false` system), and `UnknownSystemReplacement` (mod replaces non-existent FQN); class XML-doc updated to "eight-phase validator" (Phases A–H). Phase 3 carry-over stubs (`SocialSystem`, `SkillSystem`) explicitly verified to remain `Replaceable = false` until M10.C — `Phase5BridgeAnnotationsTests` includes two protected-guard tests that lock this until the Phase 3 carry-over migrates. Commits: `1af73ad` (Phase 5 annotations), `a408f44` (Phase H), `b0f1ee5` (tests). Tests: `PhaseHBridgeReplacementTests` (8), `Phase5BridgeAnnotationsTests` (9 — 7 Replaceable bridges + 2 protected-system guards).

- **M6.2 ✅ Closed.** Acceptance: `ModIntegrationPipeline.CollectReplacedFqns` helper builds the set of replaced kernel-system FQNs from all loaded mods' `Manifest.Replaces` lists; pipeline graph build skips kernel `SystemOrigin.Core` systems whose FQN is in the replaced set; mod-supplied replacement systems register normally per the existing flow (no special path); integration tests cover all four §7.5 scenarios end-to-end at the pipeline level (Replaceable success, Protected reject, Unknown reject, Conflict reject). Commits: `23f2933` (pipeline skip wiring), `602a84e` (helper unit tests), `adad506` (integration scenarios + replacement fixtures). Tests: `CollectReplacedFqnsTests` (5), `M62IntegrationTests` (5).

**Consumes decisions:** strategic lock 2 (explicit `replaces`), D-2 (capability cross-check — Phase H reuses M3 capability resolution semantics for replacement systems).

**Acceptance criteria met (per `MOD_OS_ARCHITECTURE` §11.1):**

- A mod replacing a `Replaceable = true` bridge: bridge skipped at graph build, mod system runs (M6.1 + M6.2).
- Two mods replace the same bridge: batch rejected with `BridgeReplacementConflict` (M6.1 validator-level; M6.2 pipeline-level).
- A mod replaces a `Replaceable = false` system: rejected with `ProtectedSystemReplacement` (M6.1 validator-level; M6.2 pipeline-level).
- A mod replaces a non-existent FQN: rejected with `UnknownSystemReplacement` (M6.1 validator-level; M6.2 pipeline-level).
- Replacement system passes the M3 capability cross-check unchanged (M6 introduces no capability-layer change; replacement systems flow through Phase C/D as ordinary mod systems).

**Unblocks:** M9, M10 (vanilla mods declare `replaces` for their slice). On-unload re-registration of skipped kernel bridges is handled by M7's hot-reload path — at M6 closure, `Apply` rebuilds the graph from the surviving mod set on every call, so the unload case reduces to "re-`Apply` without the unloaded mod" once M7 lands.

---

### M7 — Hot reload from menu

Goal: complete the hot-reload path through `ModIntegrationPipeline`, including `AssemblyLoadContext` unload with WeakReference verification.

**Consumes decisions:** strategic lock 3 (menu-driven, paused-only), D-7 (vanilla `hotReload` flag, build-pipeline override).

**What we implement**

- `ModIntegrationPipeline.Pause()` and `Resume()` setting the scheduler's run flag. `Apply` checks the flag and throws `InvalidOperationException("Pause the scheduler before applying mods")` if invoked while running.
- ALC unload chain:
  1. `RestrictedModApi.UnsubscribeAll` (drops bus subscriptions).
  2. `IModContractStore.RevokeAll(modId)`.
  3. `ModRegistry.RemoveSystems(modId)`.
  4. Dependency graph rebuilt without the mod.
  5. Scheduler swap.
  6. `ALC.Unload()`.
  7. WeakReference spin loop: poll `WeakReference.IsAlive` until `false` or 10-second timeout; timeout escalates to `ModUnloadTimeout` warning.
- WeakReference unload tests (originally Phase 2 backlog, now hard requirement): every regular mod loaded in tests must unload cleanly. Test fixture provides the `WeakReference` and asserts `false` within timeout.
- Mod-menu UI: list active mods, toggle, version display, "Apply" button. Hot-reload button disabled for mods with `hotReload: false`.
- Build-pipeline override (per D-7): a build-time tool rewrites `hotReload: true` to `hotReload: false` for every vanilla manifest in shipped builds. Single flag in build script, no code branching.

**Acceptance criteria**

- User pauses simulation via menu, toggles a mod, hits Apply: target mod loads/unloads, scheduler rebuilds, simulation resumes from the same world state.
- A mod with `hotReload: false` cannot be reloaded mid-session; menu shows tooltip directing user to restart the game.
- Every regular mod under test passes the WeakReference unload check within 10 seconds; failures fail the test (no flakiness tolerated).
- Calling `Apply` while the scheduler is running throws `InvalidOperationException` with the canonical message.
- A mod that retains a static reference to a kernel object after `Unload` triggers `ModUnloadTimeout` and is reported by id.

**Unblocks:** M8 onwards. Vanilla mods authored with `hotReload: true` for development; shipped builds override.

---

### M8 — Vanilla mod skeletons

Goal: create the directory structure and empty manifests for the five vanilla mods so subsequent M-phases have load targets.

**Consumes decisions:** strategic lock 4 (vanilla split into slices).

**What we implement**

- `mods/DualFrontier.Mod.Vanilla.Core/` — shared mod (`kind: "shared"`) for cross-slice types: `WeaponDef`, `RecipeDef`, common enums, helper records. Initial set is small; types are added as M9/M10 demand.
- `mods/DualFrontier.Mod.Vanilla.Combat/` — regular mod, depends on `Vanilla.Core`. Empty `IMod.Initialize`. `replaces` lists `CombatSystem`, `DamageSystem`, `ProjectileSystem`, `ShieldSystem`, `StatusEffectSystem` (eventually).
- `mods/DualFrontier.Mod.Vanilla.Magic/` — same shape.
- `mods/DualFrontier.Mod.Vanilla.Inventory/` — same shape (reuses Phase 4 `InventorySystem` once migrated).
- `mods/DualFrontier.Mod.Vanilla.Pawn/` — same shape; will absorb `SocialSystem`, `SkillSystem`.
- `mods/DualFrontier.Mod.Vanilla.World/` — same shape.
- Each mod's `.csproj` references only `DualFrontier.Contracts` and any shared-mod dependencies (typically `Vanilla.Core`).

**Acceptance criteria**

- All five mods build without warnings.
- Loading the full vanilla mod set into `ModIntegrationPipeline` succeeds: all manifests parse, dependency graph builds, scheduler rebuilds with the mods registered (still empty `Initialize`).
- Smoke test: kernel runs one tick with vanilla mods loaded; no exceptions; no system actually does anything yet (empty `Initialize`).
- Vanilla manifest `hotReload: true` in source; build-pipeline override correctness verified by a CI test.

**Unblocks:** M9, M10. From this point, new gameplay code lands inside vanilla mod assemblies.

---

### M9 — Vanilla.Combat real implementation

Goal: implement the gameplay scope originally planned for Phase 5 — but inside `Vanilla.Combat` rather than `DualFrontier.Systems.Combat`. The kernel `[BridgeImplementation]` stubs become bypassed at scheduler build via `replaces`.

**Consumes decisions:** strategic locks 1, 2, D-1, D-2.

**What we implement**

- Move the scope of `CombatSystem`, `DamageSystem`, `ProjectileSystem`, `ShieldSystem`, `StatusEffectSystem` from kernel bridges to `Vanilla.Combat` mod systems. Public surface (event types, component types) stays in kernel; only the system implementations move.
- Two-step `AmmoIntent → AmmoGranted` model through `IntentBatcher`. The composite shot pattern (TechArch v0.2 §12.4) — `CompoundShotIntent` → `CompositeResolutionSystem` → `ShootGranted`/`ShootRefused` — implemented end-to-end.
- Damage model: damage types (Heat, Sharp, Blunt, EMP, Toxic, Psychic, Stagger) with armour accounting per [GDD] §6.1.
- Shields: HP pool, regeneration, weaknesses.
- `DeathEvent` published `[Deferred]`; subscribers (mood, social, eventually faction) receive on next tick.
- `Vanilla.Core` shared mod populated with `WeaponDef`, `AmmoDef`, `DamageProfile` records used by combat. Vanilla weapons (rifle, pistol, sword, ammo types) registered via `PublishContract` in `IMod.Initialize`.
- Bridge stubs in `DualFrontier.Systems.Combat` retain their `[BridgeImplementation(Phase = 5, Replaceable = true)]` annotations; `Vanilla.Combat` manifest lists them in `replaces`.

**Acceptance criteria**

- Loading game with `Vanilla.Combat` enabled: combat works, pawns shoot, take damage, die. `DeathEvent` propagates.
- Disabling `Vanilla.Combat`: kernel bridges re-register, combat goes silent (pawns equipped with weapons do not shoot — bridges are no-op). The game does not crash; this is the architectural baseline for "an engine that ships without combat."
- All capability checks pass: `Vanilla.Combat` declares every capability honestly in its manifest.
- 100 pawns × 60 ticks × ammo request: ≤ 100 scans/sec (the original Phase 5 target from §11.11 of architecture; preserved here unchanged).

**Unblocks:** M10. Magic and other slices follow the same pattern with confidence the migration path works.

---

### M10 — Remaining vanilla slices: Magic, Inventory, Pawn, World

Goal: incremental delivery of the four remaining vanilla mods, in any order the development pipeline finds useful. Each follows the M9 template.

**Consumes decisions:** locks already in effect from M0–M9.

**M10.A — Vanilla.Magic.** Consumes original Phase 6 scope. Eight schools of magic per [GDD] §6.1. Five levels of ether perception per §4.1. Five golem types per §5.1. Combo mechanics per §6.3. Replaces kernel `ManaSystem`, `SpellSystem`, `GolemSystem`, `EtherGrowthSystem`, `RitualSystem` bridges. Cross-slice: depends on `Vanilla.Combat` for damage profiles, on `Vanilla.Inventory` for crystal storage.

**M10.B — Vanilla.Inventory.** The Phase 4 `InventorySystem`, `HaulSystem`, and the power-grid systems (`ElectricGridSystem`, `ConverterSystem`) migrate from kernel to mod. The kernel keeps the bus contracts and the `[Deferred]` event types; the systems move. This is a **refactor migration**, not new gameplay; covered by existing tests.

**M10.C — Vanilla.Pawn.** Consumes Phase 3 backlog: `SocialSystem` and `SkillSystem` get real implementations inside the mod. The kernel `JobSystem`, `MoodSystem`, `MovementSystem`, `NeedsSystem`, `PawnStateReporterSystem` migrate as part of the same refactor.

**M10.D — Vanilla.World.** Consumes original Phase 7 scope. Biomes affect resources, weather, passability. Ether nodes form coverage radii. Inter-faction relations per [GDD] §3.3. Raids by colony level. Trade caravans by season. Replaces kernel `BiomeSystem`, `WeatherSystem`, `MapSystem`, `RaidSystem`, `RelationSystem`, `TradeSystem` bridges.

**Acceptance criteria** (per slice, applied uniformly)

- All gameplay scope from the original Phase 5/6/7 spec delivered as mod content.
- Disabling the slice mod: the kernel runs with that domain silent; no crashes.
- The slice's `replaces` list covers every kernel bridge it supersedes; capability cross-check passes; test count grows with each slice.
- Inter-slice dependencies (e.g. Magic depends on Combat for damage types) declared via `dependencies` with caret-versioned constraints.

**Unblocks:** the full game. Subsequent expansions are content mods (community or first-party), shipped as additional regular mods that depend on the vanilla slices.

---

## ⏭ Phase 9 — Native Runtime (post-launch)

Unchanged from the original plan. Goal: own entry point bypassing Godot's runtime. `IRenderer`, `IInputProvider` already abstracted (Phase 3.5 work). `GameLoop` runs on a pure .NET thread. `PresentationBridge` abstracts rendering behind `IRenderCommand`. Simulation is Godot-agnostic — proven by the test suite running without any Godot dependency.

**When:** after M10 closure and Steam launch. Separate large project; does not block release.

**Why this is possible:** the architecture is already ready. The Mod-OS Migration does not change the relationship between Application and Presentation; `Vanilla.Combat` runs identically under Godot and under a future Native runtime, because both consume the same `IGameServices` contracts.

---

## See also

- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) — v1.3 LOCKED specification driving M1–M10.
- [METHODOLOGY](./METHODOLOGY.md) — the four-agent pipeline; M1–M10 are exercised through it.
- [ARCHITECTURE](./ARCHITECTURE.md) — the four layers; the Mod-OS migration touches only Application and below.
- [CONTRACTS](./CONTRACTS.md) — bus and marker conventions; capability syntax mirrors bus naming.
- [MODDING](./MODDING.md) — v1 mod-author guide; M-phase outputs supersede this document for v2.
- [MOD_PIPELINE](./MOD_PIPELINE.md) — `ModIntegrationPipeline` mechanics; M2, M5, M6, M7 extend it.
- [TESTING_STRATEGY](./TESTING_STRATEGY.md) — test discipline; M-phase acceptance criteria slot into the existing isolation/modding/integration tiers.
- [DEVELOPMENT_HYGIENE](./DEVELOPMENT_HYGIENE.md) — PR hygiene; the Mod-OS migration is exercised through the same checklist.
