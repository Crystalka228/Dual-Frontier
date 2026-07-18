---
register_id: DOC-E-GAME_ENGINE_BOUNDARY_AUDIT_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-18'
last_modified: '2026-07-18'
content_language: en
next_review_due: null
review_cadence: none-historical-record
title: 'GAME/ENGINE BOUNDARY AUDIT REPORT — 2026-07-18 (R1–R7) — full mechanical verification of the game/engine composition boundary at HEAD 4c58942: ten coupling-claims C-1..C-10 all SUPPORTED (none WRONG); 4 engine→game ProjectReference edges (all in DualFrontier.Application) + 1 engine→game IVT (Core→Systems) + 1 test-fixture leak; native tree + C ABI CLEAN; T3 type inventory (Components 28 / Events 53 / Systems 30 / AI 20); Contracts 65 public types (game-binding subset ≈ 8); decision list D1–D10 + anomalies A1–A9; framing = current→target distance, no kickoff↔law conflict'
special_case_rationale: 'Durable-report recon enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-EQ_A_SHUTDOWN_RECON_REPORT, DOC-E-F29_NATIVE_SCHEDULER_RECON_REPORT). Basis for the forthcoming game/engine boundary-law document + migration plan; produced read-only at HEAD 4c58942 (post-EQ_A1_FAULT_SYMMETRY).'
---

# GAME/ENGINE BOUNDARY AUDIT REPORT — 2026-07-18

Full mechanical verification and census pass for the game/engine composition boundary. Read-only measurement session: **one report, zero repository mutations**. This document produces facts, anchors, and counts — **not designs, not fixes, not wave-ordering opinions**. Design belongs to the architect deliberation that consumes this report.

**Mission input.** The operator has ratified a target composition model — *the engine is a simulation OS; Dual Frontier is its first distribution; game code exists only as mods consuming the engine.* An external architecture assessment (2026-07-17, slice `bc93241`) claims the current codebase violates that boundary in ten specific ways (transcribed as C-1..C-10 in §R2). Two were pre-confirmed by architect line-read (C-2, C-4). This is the full verification the forthcoming boundary-law document and migration plan will rest on.

**Law in force (cited, not restated).** `ARCHITECTURE.md` v1.0.2 LOCKED · `MOD_OS_ARCHITECTURE.md` v1.0.2 LOCKED · `EXECUTION_AUTHORITY_MATRIX.md` v1.0.1 LOCKED · `historical/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.4.1 SUPERSEDED. These docs' code anchors were authored/verified at HEAD `35364c2`; this audit re-measures every load-bearing anchor at the current HEAD. Where a claim is confirmed, the report also records **whether the current corpus already documents that behavior as LOCKED design** — because the boundary "violations" are, with near-uniformity, *documented current law*, and the gap is against the *target* model, not against the standing corpus (see §R1 framing and the self-check in §R7).

---

## R1 — Base state

- **Branch / HEAD:** `main` @ `4c58942e970435c56bb1034c932d59aa18709a64` (`4c58942`) — commit `governance(closure): EQ_A1 EVT append + ROADMAP EQ-a annotation`. **Dispatch precondition satisfied:** this is the post-EQ_A1_FAULT_SYMMETRY tree (the audit's anchors reference it).
- **`git status --porcelain`:** empty at session start (clean working tree). At session end the only change is this untracked report file (§Attestation).
- **Derived registers (read directly; `sync` NOT run):**
  - `docs/governance/REGISTER.yaml` → `schema_version: 2.0`, `register_version: 2.0`; **349 documents** — `rg --count-matches '^- register_id:' docs/governance/REGISTER.yaml` → `349`.
  - `docs/governance/AUDIT_TRAIL.yaml` → **58 EVTs** — `rg --count-matches '^- id: EVT-' docs/governance/AUDIT_TRAIL.yaml` → `58`.
  - (The 2026-07-18 EQ_A_SHUTDOWN recon recorded 346/57; the +3 docs / +1 EVT are the EQ_A1 cascade's own enrollments.)
- **The external assessment is NOT in-repo.** `rg 'StartingPawnCount|EtherDensity|bc93241|coupling-claim' docs` matches only two closure reports mentioning field names in passing (`STACK_UPDATE_CLOSURE_REPORT.md`, `LINUX_SANDBOX_ENV_BASELINE_REPORT.md`). The kickoff's C-1..C-10 transcription is the authoritative claim input.
- **No ROADMAP engineering-queue row governs the game-as-mods axis yet.** `rg -n 'vanilla-as-mods|game.{0,3}as.{0,3}mods|BOUNDARY_AUDIT' docs/ROADMAP.md` returns no migration row; EQ-a/EQ-b are the lifecycle/identity axes. **This audit opens a new axis.**
- **HEAD-drift check:** the two crux files (`SystemBase.cs`, `KernelCapabilityRegistry.cs`) were re-read first-hand at `4c58942` — **zero drift** from the line anchors the LOCKED docs cite at `35364c2`.

**Framing the verdicts must carry (the report's most important sentence).** `ARCHITECTURE.md` §1 places `Systems·Components·Events·AI·Persistence` in a first-class **"Domain" layer** ("game rules; multithreaded; renderer-agnostic"), and §2 documents `Application → {Components,Events,Systems,AI}` as legal, intended edges. `MOD_OS_ARCHITECTURE.md` §3.4 *specifies* that `KernelCapabilityRegistry` scans `Contracts`/`Components`/`Events` and publishes vanilla types under `kernel.*`. So the ten "violations" are **documented LOCKED behavior**, not accidents. К-L9 «vanilla = mods» (LOCKED) already moved vanilla *systems* into mod projects; the target model is a *further increment* that also empties the engine of vanilla *data types*. The superseded migration plan migrated *within* a game-in-engine architecture; the target *dissolves* the Domain layer. **The distance the report measures is current→target, and it is not a corpus violation.**

---

## R2 — T1: Claim verification (C-1 .. C-10)

Verdict scale: **EXACT** (claim true, details correct) / **DRIFTED** (true in substance, details off) / **WRONG**. Result: **all ten SUPPORTED; none WRONG.** Each row carries the decisive `file:line` + verbatim code and a current-vs-target note.

### C-1 — A real mod system cannot honor the Contracts-only boundary and compile. **VERDICT: EXACT** (mechanism refinement)

Registering a mod system requires inheriting `DualFrontier.Core.ECS.SystemBase`, which is unnameable from a Contracts-only compilation unit.

- `SystemBase` — root abstract class in **Core**: `src/DualFrontier.Core/ECS/SystemBase.cs:19` `public abstract class SystemBase`.
- Runtime gate in **Application** — `src/DualFrontier.Application/Modding/ModRegistry.cs:103` `public void RegisterSystem(string modId, Type systemType)`, enforcing at `:108`:
  ```csharp
  if (!typeof(SystemBase).IsAssignableFrom(systemType))
      throw new InvalidOperationException(
          $"[MOD REGISTRY ERROR] Type '{systemType.FullName}' does not derive from SystemBase. " +
          "Use 'public sealed class MySystem : SystemBase' for mod systems.");
  ```
  Plus `[SystemAccess]` required at `:116-124` and `[TickRate]` at `:126-134` — the remediation hint at `:123` literally prints `bus: nameof(IGameServices.X)`.
- Mod-facing surface is deliberately weak: `src/DualFrontier.Contracts/Modding/IModApi.cs:66` `void RegisterSystem<T>() where T : class;`.
- **Consequence (reproducible):** `rg -l ': SystemBase' mods | wc -l` → `0`; `rg -l ': SystemBase' src/DualFrontier.Systems -g '!**/obj/**' | wc -l` → `29`. Zero mod-authored systems; every production system class lives in `src/DualFrontier.Systems`. The six vanilla mods are hollow `IMod` skeletons registering nothing (`mods/DualFrontier.Mod.Vanilla.Combat/CombatMod.cs` comment: real systems "remain in place," migration "in M9").
- **Contracts self-documents the breach:** `Contracts/Attributes/ModCapabilitiesAttribute.cs:6-8` ("Placed on `DualFrontier.Core.ECS.SystemBase` subclasses registered through `IModApi.RegisterSystem`"); `Contracts/Modding/ManagedStore.cs:44-48` ("mod systems — which subclass `SystemBase` in Core but cannot reference Application").
- **Refinement for accuracy:** the requirement is a *runtime* `IsAssignableFrom` gate (`ModRegistry.cs:108`), **not** a generic constraint (the constraint is only `where T : class`). The substance — a Contracts-only mod cannot author a type that both compiles and passes registration — is EXACT.
- **Current-vs-target:** documented as intended in `MOD_OS_ARCHITECTURE.md` §4.1 / §3.7; not a corpus violation.

### C-2 — `SystemBase` exposes concrete `NativeWorld` + `IGameServices`. **VERDICT: EXACT** (architect pre-confirmed; closure verified)

`src/DualFrontier.Core/ECS/SystemBase.cs` (verbatim, first-hand):
- `:5` `using DualFrontier.Core.Interop;`
- `:70` `protected IGameServices Services` (from `DualFrontier.Contracts.Bus`)
- `:93` `protected NativeWorld NativeWorld` — body `return ctx.NativeWorld;` (`:100`)
- `:29` `public SystemExecutionContext Context { get; internal set; }`
- `:126` `protected ManagedStore<T>? ManagedStore<T>() where T : class, IComponent`

`NativeWorld` is a concrete sealed Interop type: `src/DualFrontier.Core.Interop/NativeWorld.cs:28` `public sealed class NativeWorld : IDisposable`.

**Dependency closure (the structural finding — see anomaly A4):** relocating `SystemBase` into the leaf `Contracts` would drag in `Core.Interop` (`NativeWorld` + its transitive graph: `SpanLease`, `WriteBatch`, `InternedString`, native P/Invoke) and `SystemExecutionContext` (holds `NativeWorld`, `IGameServices`, internal `SystemOrigin`, internal `IModFaultSink`). Because `Core.Interop` **already** references `Contracts` (`ARCHITECTURE.md` §2; `Core.Interop.csproj:14`), the move inverts that edge into a **`Contracts → Core.Interop → Contracts` cycle**. The naive remedy is structurally impossible; the migration needs a *new* Contracts-level system abstraction, not a relocation.

- **Current-vs-target:** `EXECUTION_AUTHORITY_MATRIX.md` §3.3 explicitly names `SystemBase`/`IGameServices` as legitimate engine *surfaces* new code may consume.

### C-3 — Systems instantiated via parameterless `Activator.CreateInstance`. **VERDICT: EXACT**

`src/DualFrontier.Application/Modding/ModRegistry.cs:283-301`, called from `RegisterSystem` at `:136` (`SystemBase instance = CreateSystemInstance(systemType);`):
```csharp
private static SystemBase CreateSystemInstance(Type systemType)
{
    try {
        object? instance = Activator.CreateInstance(systemType);   // :287
        if (instance is SystemBase system) return system;
        throw new InvalidOperationException(/* returned null or non-SystemBase */);
    }
    catch (MissingMethodException ex) {
        throw new InvalidOperationException(
            $"[MOD REGISTRY ERROR] System '{systemType.FullName}' requires a public parameterless constructor.", ex);  // :297-300
    }
}
```
Parameterless `Activator.CreateInstance` (`:287`); the `MissingMethodException` handler proves a public parameterless ctor is mandatory. Sole system-instantiation site (the only other `Activator.CreateInstance` in `src` is `ModLoader.cs:96`, the `IMod` entry type — also parameterless). Systems with constructor dependencies cannot be authored.

### C-4 — `IGameServices` carries fixed genres, baked into Contracts + scheduler metadata + bus router. **VERDICT: EXACT** (architect pre-confirmed; three legs closed)

- **Contracts leg:** `src/DualFrontier.Contracts/Bus/IGameServices.cs:13` `public interface IGameServices`; members `ICombatBus Combat` (`:20`), `IInventoryBus Inventory` (`:27`), `IMagicBus Magic` (`:34`), `IPawnBus Pawns` (`:41`), `IWorldBus World` (`:48`). Five domain-bus markers, each `: IEventBus` (`Contracts/Bus/{ICombatBus,IInventoryBus,IMagicBus,IPawnBus,IWorldBus}.cs`). The interface's own doc-comments name concrete systems as writers/readers (`CombatSystem`, `ProjectileSystem`, `WeatherSystem`, …).
- **Bus-router leg:** `src/DualFrontier.Application/Modding/ModBusRouter.cs:28` `internal static object? Resolve(Type eventType, IGameServices services)`; `:42` `typeof(IGameServices).GetProperties(...)` — an event's `[EventBus("Combat")]` name is matched against the `IGameServices` property name.
- **Scheduler-metadata leg:** `[SystemAccess(bus: nameof(IGameServices.Combat))]` string-binding (`Contracts/Attributes/SystemAccessAttribute.cs` — fields `Bus`/`Buses`), consumed by `DependencyGraph` edge-building; the registration gate at `ModRegistry.cs:123` requires it.

The five-genre list is hardcoded as named types + `IGameServices` properties and stringly-referenced across scheduler and router. **Current-vs-target:** canon per `CONTRACTS.md` five-bus rule and К-L9 uniformity.

### C-5 — `KernelCapabilityRegistry` hard-scans `Components`/`Events` via marker types; vanilla types under `kernel.*`. **VERDICT: EXACT**

`src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs` (`internal sealed class`, `:32`), first-hand verbatim:
```csharp
using DualFrontier.Components.Shared;   // :4
using DualFrontier.Events.Pawn;         // :9
...
internal static KernelCapabilityRegistry BuildFromKernelAssemblies() => new(new[]  // :82
{
    typeof(IEvent).Assembly,           // DualFrontier.Contracts
    typeof(IComponent).Assembly,       // DualFrontier.Contracts (deduped)
    typeof(HealthComponent).Assembly,  // DualFrontier.Components   :87
    typeof(PawnSpawnedEvent).Assembly, // DualFrontier.Events       :88
});
```
Ownership assignment (`ScanAssembly`): `:124-125` `kernel.publish:{fqn}` / `kernel.subscribe:{fqn}`; `:140-143` `kernel.read:{fqn}` / `kernel.write:{fqn}`; `:158/161` `kernel.layer.intent:{fqn}` / `kernel.layer.combat_feedback:{fqn}`. The marker component is opt-in accessible: `src/DualFrontier.Components/Shared/HealthComponent.cs:10` `[ModAccessible(Read = true, Write = true)]`, `:11` `public struct HealthComponent : IComponent`.

- **Current-vs-target:** matches `MOD_OS_ARCHITECTURE.md` §3.4 LOCKED law *exactly*. The class doc-comment `:70-80` says `IEvent`/`IComponent` "both **currently** live in `DualFrontier.Contracts`" — the code itself flags these types' kernel-ownership as provisional. See anomaly A9 (the "kernel" registry lives in the game composition layer, `Application`).

### C-6 — `GameBootstrap` is a game composition root. **VERDICT: EXACT** (lands stronger)

`src/DualFrontier.Application/Loop/GameBootstrap.cs` (`internal static class`, `:56`; entry `CreateLoop`, `:70`), first-hand:
- **Component registrations — 21**, delegated at `:77` `VanillaComponentRegistration.RegisterAll(nativeWorld.Registry!)` (block `src/DualFrontier.Application/Bootstrap/VanillaComponentRegistration.cs:40-85`, ids 1–21: `HealthComponent`…`StorageComponent`).
- **System registrations — 10**, `:131-143` `var coreSystems = new SystemBase[] { new NeedsSystem(), new MoodSystem(), new JobSystem(), new ConsumeSystem(), new SleepSystem(), new ComfortAuraSystem(), new MovementSystem(pathfinding), new PawnStateReporterSystem(), new InventorySystem(), new HaulSystem() };` (doc `:42` "coreSystems shrinks from 12 to 10"). Then dual-registered into the native scheduler graph with empty access sets `:162-180`.
- **Map/pawn/item factories:** `NavGrid` + obstacle seeding `:95-102`; `AStarPathfinding` `:103`; `RandomPawnFactory` `:105`, `.Spawn(...)` `:106`; `ItemFactory` `:118`, `.Spawn(...)` `:119-125`; `PublishItemSpawnedEvents` `:129`.
- **Render subscriptions — 5**, `:82-93` (`PawnSpawnedCommand`, `ItemSpawnedCommand`, `PawnMovedCommand`, `PawnDiedCommand`, `PawnStateCommand`).
- **Stronger than claimed:** GameBootstrap reads **no** `ScenarioDef`/`SceneMetadata`; every world parameter is a hardcoded `private const` (`:58-68`: `MapWidth=200`, `MapHeight=200`, `InitialPawnCount=50`, seeds, item counts). Ties to `EXECUTION_AUTHORITY_MATRIX.md` R13 (ownerless configuration) and anomaly A3.

### C-7 — `Application` ProjectReferences run engine→game. **VERDICT: EXACT**

`src/DualFrontier.Application/DualFrontier.Application.csproj:10-16` (first-hand):
```xml
<ProjectReference Include="..\DualFrontier.Contracts\..." />       <!-- 10 -->
<ProjectReference Include="..\DualFrontier.Core\..." />            <!-- 11 -->
<ProjectReference Include="..\DualFrontier.Components\..." />      <!-- 12  ENGINE→GAME -->
<ProjectReference Include="..\DualFrontier.Core.Interop\..." />    <!-- 13 -->
<ProjectReference Include="..\DualFrontier.Events\..." />          <!-- 14  ENGINE→GAME -->
<ProjectReference Include="..\DualFrontier.Systems\..." />         <!-- 15  ENGINE→GAME -->
<ProjectReference Include="..\DualFrontier.AI\..." />              <!-- 16  ENGINE→GAME -->
```
`Launcher` (`Launcher.csproj:8` → Application) inherits all four transitively but declares no direct engine→game edge. These four lines are the entire compile-graph violation (§R3).

### C-8 — Presentation bridge has game commands; `LayerType` has game members. **VERDICT: EXACT** (detail nuance)

Six command records in `src/DualFrontier.Application/Bridge/Commands/` (`namespace DualFrontier.Application.Bridge.Commands`), verbatim declarations:
- `PawnSpawnedCommand.cs:16` `public sealed record PawnSpawnedCommand(EntityId PawnId, float X, float Y) : IRenderCommand;`
- `PawnMovedCommand.cs:15`, `PawnDiedCommand.cs:13` `public sealed record PawnDiedCommand(EntityId PawnId) : IRenderCommand;`
- `PawnStateCommand.cs:16` (multi-field: Satiety/Hydration/Sleep/Comfort/Mood/JobLabel/TopSkills), `ItemSpawnedCommand.cs:18` (`ItemKind Kind`), `TickAdvancedCommand.cs:19`.

`LayerType` lives in the engine contract: `src/DualFrontier.Contracts/Display/LayerType.cs:16` `public enum LayerType { SimState=0 (:24), Intent=1 (:31), CombatFeedback=2 (:39), Static=3 (:45) }`. `CombatFeedback` doc (`:33-39`): "Fast tier consumer (К-L15) overlay — damage numbers, hit sparks, weapon glints." Switched on at `KernelCapabilityRegistry.cs:160`, `Application/Display/CombatFeedbackLayer.cs:57`, `Application/Display/CompositionFramework.cs:123`.

- **Detail nuance:** the Launcher renderer `src/DualFrontier.Launcher/RenderCommandDispatcher.cs:42-49` switches on the **command records**, while `LayerType.CombatFeedback` is switched in the Application display composition + capability registry. `LayerType` doc `:6-8`: it lives in Contracts "so mod assemblies can reference the enum … без taking а dependency on `DualFrontier.Application`" — deliberate placement.

### C-9 — Persistence holds game DTOs in the engine layer. **VERDICT: EXACT**

`src/DualFrontier.Persistence/Snapshots/` (`namespace DualFrontier.Persistence.Snapshots`), first-hand:
- `PawnSnapshot.cs:7` `public sealed record PawnSnapshot` (needs+job fields: EntityIndex, X, Y, Satiety, Hydration, Sleep, Comfort, Mood, JobKind).
- `StorageSnapshot.cs:9` `public sealed record StorageSnapshot` (EntityIndex, Capacity, Items dict).
- `TileMapSnapshot.cs:10` `public sealed class TileMapSnapshot`; `:22` `public required TerrainKind[] Tiles { get; init; }`.
- `WorldSnapshot.cs:10` `public sealed class WorldSnapshot` (aggregates the above).

`TerrainKind` is a game enum in `DualFrontier.Components`: `src/DualFrontier.Components/World/TerrainKind.cs:8` `public enum TerrainKind` (Unknown/Grass/Rock/Sand/Water/Ice/Swamp/Arcane). Encoders in `src/DualFrontier.Persistence/Compression/`: `TileEncoder.cs:26` `Encode(TileMapSnapshot map)` / `:62` `Decode`; `ComponentEncoder.cs` (QuantizeFloat); fronted by `DfCompressor.cs`.

- **Note:** `DualFrontier.Persistence` is production-**orphaned** — `rg -l 'DualFrontier\.Persistence\.csproj' -g '*.csproj'` → only `tests/DualFrontier.Persistence.Tests`. Corroborates `ARCHITECTURE.md` §4 and `EXECUTION_AUTHORITY_MATRIX.md` R12 (vacant persistence-snapshot ownership).

### C-10 — `ScenarioDef.StartingPawnCount` and `SceneMetadata.EtherDensity` live in Application. **VERDICT: EXACT** (lands stronger — both orphan)

- `src/DualFrontier.Application/Scenario/ScenarioDef.cs:9` `public sealed class ScenarioDef`; `:18` `public int StartingPawnCount { get; init; } = 3;` (also `MapWidth=100`, `MapHeight=100` — *different* from GameBootstrap's `200`, confirming ScenarioDef is not the live source).
- `src/DualFrontier.Application/Scene/SceneMetadata.cs:9` `public sealed record SceneMetadata(string Biome, float EtherDensity, string CreatedBy, DateTimeOffset ExportedAt);` — doc `:7`: "Not used by the simulation — consumed by the UI and debugging tools."
- **Consumers (reproducible):** `rg -n 'StartingPawnCount' src -g '!**/obj/**'` → 2 code sites, both declaration/default-init (`ScenarioDef.cs:18`, `ScenarioLoader.cs:45`), **no production reader** (the loop uses `GameBootstrap.InitialPawnCount = 50`). `rg -n 'EtherDensity' src -g '!**/obj/**'` → **exactly one** site (`SceneMetadata.cs:11`, the declaration). **Zero consumers.** Both are orphan game config → `EXECUTION_AUTHORITY_MATRIX.md` R13.

---

## R3 — T2: Assembly dependency graph reality

**Adjacency (all `src/` + `mods/`; verbatim `ProjectReference` line numbers).** `[E]` engine, `[G]` game.

| Project | References (→ csproj line) |
|---|---|
| Contracts `[E]` | *(leaf — 0 refs)* |
| Core.Interop `[E]` | Contracts (`:14`) |
| Core `[E]` | Contracts (`:7`), Core.Interop (`:13`) |
| Runtime `[E]` | Core.Interop (`:8`) |
| **Application `[E]`** | Contracts (`:10`), Core (`:11`), **Components (`:12`)**, Core.Interop (`:13`), **Events (`:14`)**, **Systems (`:15`)**, **AI (`:16`)** |
| Launcher `[E]` | Application (`:8`), Runtime (`:9`) |
| Crypto.Future `[E]` | *(leaf — 0 refs)* |
| Components `[G]` | Contracts (`:7`), Core.Interop (`:11`) |
| Events `[G]` | Contracts (`:7`), Components (`:8`) |
| AI `[G]` | Contracts (`:7`), Components (`:8`) |
| Persistence `[G]` | Contracts (`:7`), Components (`:8`) |
| Systems `[G]` | Contracts (`:7`), Core (`:8`), Components (`:9`), Events (`:10`), AI (`:11`) |
| Mods ×7 | Contracts **only** (`mods/DualFrontier.Mod.Vanilla.Combat/*.csproj:9` + siblings; `Mod.Example:8`) |

### Arrow census — engine→game `ProjectReference` edges = **4** (all from `DualFrontier.Application`)

`rg -n 'ProjectReference Include=".*(Components|Events|Systems|DualFrontier\.AI)' src/DualFrontier.Application/DualFrontier.Application.csproj`:

| # | From | → To | Anchor |
|---|---|---|---|
| 1 | Application | Components | `src/DualFrontier.Application/DualFrontier.Application.csproj:12` |
| 2 | Application | Events | `…Application.csproj:14` |
| 3 | Application | Systems | `…Application.csproj:15` |
| 4 | Application | AI | `…Application.csproj:16` |

**No other project produces an engine→game edge.** `Launcher` pulls the game layer in only transitively via `Application`. All game→engine edges point in the allowed (downward) direction and are not census material. **This is the number a boundary analyzer must drive to zero.**

### Couplings that evade the reference graph

- **InternalsVisibleTo — 1 engine→game.** `rg -n 'InternalsVisibleTo' src/DualFrontier.Core/DualFrontier.Core.csproj`: `:16 DualFrontier.Systems` (**engine grants internals to game**; comment cites SystemBase/NativeWorld), `:17 Application`, `:18-20` tests, `:23` benchmarks. Every other IVT (`Core.Interop.csproj:23 → Application`; `Application.csproj:20 → Launcher`) targets engine or test/bench. `ARCHITECTURE.md` §2.1 already flags `Core→Systems` as "Unused — removable now." No attribute-form `[assembly: InternalsVisibleTo]` exists in any `.cs` (`rg 'assembly:\s*InternalsVisibleTo'` → 0).
- **Reflection — 0 engine→game concrete coupling.** The only string/path loaders live in the mod host `src/DualFrontier.Application/Modding/`: `ModLoader.cs:89` `LoadFromAssemblyPath(...)`, `:96` `Activator.CreateInstance(modType)`, `:331` `asm.GetType(manifest.EntryType,...)`; `ModRegistry.cs:287` `Activator.CreateInstance(systemType)`; `ContractValidator.cs:680` `asm.GetType(fqn,...)`. All resolve `IMod`/system *plugins* by manifest string — never the vanilla game assemblies by name. Engine reflection couples to `IMod`/`IComponent`/`IEvent` Contracts abstractions, not to game concretes.
- **Test-fixture leak — 1.** `tests/Fixture.RegularMod_ReplacesCombat/Fixture.RegularMod_ReplacesCombat.csproj:13` references `DualFrontier.Core` (all other mod/vanilla fixtures are Contracts-only). This is the "test fixture references Core" the assessment cited.

### Orphans, off-solution, build config

- **Orphans — 2.** `Persistence` (referenced only by `tests/DualFrontier.Persistence.Tests`) and `Crypto.Future` (`rg -l 'DualFrontier\.Crypto\.Future\.csproj' -g '*.csproj'` → **nothing**).
- **Off-solution — 6.** The six vanilla mods are absent from `DualFrontier.sln` (`rg -c 'DualFrontier\.Mod\.Vanilla' DualFrontier.sln` → `0`); only `Mod.Example` is in the `mods` solution folder. The projects embodying «vanilla = mods» are not in the built solution graph.
- **Build config.** Root `Directory.Build.props` → `net10.0`, `LangVersion 14.0`, `Nullable enable`, `TreatWarningsAsErrors=true`. `src/Directory.Build.props` injects `tools/DualFrontier.Analyzers` into all 12 src projects as `OutputItemType="Analyzer" ReferenceOutputAssembly="false"` (build-only, not a runtime edge). CPM via `Directory.Packages.props` (`Microsoft.CodeAnalysis 5.3.0`, `xunit 2.9.3`, …). **No `PackageReference` influences the engine/game boundary.**

---

## R4 — T3: Gameplay-noun type inventory in engine-resident game assemblies

Raw material for the ownership matrix — **ownership classification is the architect's job, not this session's.** Full inventory (namespace · type · kind); `obj/` excluded.

### `DualFrontier.Components` — 26 files / 28 types
- `.Building` — `WorkbenchComponent`, `StorageComponent` (struct : IComponent)
- `.Combat` — `ArmorComponent` (struct)
- `.Items` — `BedComponent`, `ConsumableComponent`, `DecorativeAuraComponent`, `ReservationComponent`, `WaterSourceComponent` (struct)
- `.Magic` — `EtherComponent`, `GolemBondComponent`, `ManaComponent` (struct)
- `.Pawn` — `IdentityComponent`, `JobComponent`, `MindComponent`, `MovementComponent`, `NeedsComponent`, `SkillsComponent` (struct); `JobKind`, `NeedKind`, `SkillKind` (enum)
- `.Shared` — `FactionComponent`, `HealthComponent`, `PositionComponent` (struct); `RaceKind` (enum) + `RaceComponent` (struct)
- `.World` — `EtherNodeComponent`, `TileComponent` (struct); `TerrainKind` (enum)

### `DualFrontier.Events` — 53 files / 53 types (all `record : IEvent` unless noted)
- `.Combat` (13) — Ammo{Granted,Intent,Refused}, CompoundShotIntent, Damage{Event,Intent}, DeathEvent, ShootAttemptEvent, Shoot{Granted,Refused}, StatusAppliedEvent, ShotRefusalReason (enum), TransactionId (record struct)
- `.Inventory` (4) — CraftRequestEvent, Item{Added,Removed,Reserved}Event
- `.Magic` (18) — EtherLevelUpEvent, EtherSurgeEvent, GolemActivatedEvent, GolemOwnershipChanged, GolemOwnershipTransferRequest, Mana{Granted,Intent,Refused}, ManaLease{Closed,Opened,OpenRequest,Refused}, SpellCastEvent, CloseReason/RefusalReason (enum), LeaseId (record struct)
- `.Pawn` (17) — DeathReactionEvent, ItemSpawnedEvent, Job{Assigned,Completed}Event, MoodBreakEvent, Needs{Critical,Restored}Event, PawnConsume{Finished,Target}Event, PawnMovedEvent, PawnSleep{Finished,Target}Event, PawnSpawnedEvent, PawnStateChangedEvent, SkillGainEvent, ItemKind (enum)
- `.World` (3) — EtherNodeChangedEvent, RaidIncomingEvent, WeatherChangedEvent

### `DualFrontier.Systems` — 30 files / 30 types (all `sealed class : SystemBase` unless noted)
- `.Combat` — CombatSystem, ComboResolutionSystem, CompositeResolutionSystem, DamageSystem, ProjectileSystem, StatusEffectSystem
- `.Faction` — RaidSystem, RelationSystem, TradeSystem
- `.Inventory` — CraftSystem, HaulSystem, InventorySystem
- `.Magic` — EtherGrowthSystem, GolemSystem, ManaSystem, RitualSystem, SpellSystem; `.Internal` — ManaLease, ManaLeaseRegistry (internal, non-SystemBase)
- `.Pawn` — ComfortAuraSystem, ConsumeSystem, JobSystem, MoodSystem, MovementSystem, NeedsSystem, PawnStateReporterSystem, SkillSystem, SleepSystem
- `.World` — MapSystem, WeatherSystem

### `DualFrontier.AI` — 18 files / 20 types
- `.BehaviourTree` — BTBlackboard, BTContext (class); BTStatus (enum) + BTNode (abstract); Leaf (abstract), Selector, Sequence
- `.BehaviourTree.Leaves` — IdleLeaf, IsHungryLeaf, IsExhaustedLeaf
- `.Jobs` — JobStatus (enum) + IJob (interface); JobCast, JobCraft, JobGolemCommand, JobHaul, JobMeditate
- `.Pathfinding` — AStarPathfinding (: IPathfindingService), IPathfindingService, NavGrid

**Reverse-reference map (engine sites, per assembly).** The engine sites that reference these types: `KernelCapabilityRegistry` (marker scan of Components + Events, §C-5), `GameBootstrap`/`VanillaComponentRegistration` (registrations + factories, §C-6), `Application/Bridge/Commands` + `Launcher/RenderCommandDispatcher` (presentation, §C-8), `Persistence/Snapshots` (DTOs, §C-9), `SystemGraphInterop` (scheduler registration, `GameBootstrap.cs:170`). **Governed/ungoverned (ARCHITECTURE §4):** in `DualFrontier.AI` only `Pathfinding` is production-consumed; `BehaviourTree`/`Jobs` are dormant (zero consumers outside `DualFrontier.AI`).

---

## R5 — T4: Public contract surface classification data

### `DualFrontier.Contracts` — 50 source files / **65 public types**

| Namespace | Types (kind) |
|---|---|
| `.Analyzer` | ReservedStubPurpose (enum), ReservedStubAttribute |
| `.Attributes` | BridgeImplementationAttribute, DeferredAttribute, ImmediateAttribute, ModAccessibleAttribute, ModCapabilitiesAttribute, SystemAccessAttribute, TickRateAttribute + TickRates (static) |
| `.Bus` | **ICombatBus, IInventoryBus, IMagicBus, IPawnBus, IWorldBus, IGameServices**, IEventBus, EventBusAttribute, BusTier (enum) + EventTierAttribute |
| `.Core` | EntityId (record struct), IComponent, IEntity, IEvent, ICommand, IQuery, IQueryResult (markers) |
| `.Display` | **LayerType (enum)**, LayerAttribute |
| `.Enums` | **OwnershipMode (enum)** — golem/mage bond states |
| `.IPC` | ShmWriterAttribute |
| `.Math` | GridVector (record struct) |
| `.Modding` | ContractsVersion, IMod, IModApi, IModContract, IModFieldApi, IModComputePipelineApi, IFieldHandle, IManagedStorageResolver, IManagedStore + ManagedStore\<T\>, ManagedStorageAttribute, ManifestCapabilities, ModDependency, ModLogLevel (enum), ModKind (enum) + ModManifest, VersionConstraintKind (enum) + VersionConstraint, CapabilityViolationException |
| `.Scheduling` | SchedulingClass/PreemptionMode/WakeType/BarrierType (enum) + Priority/CpuQuota/Preempt/CpuAffinity/PhaseBarrier/WakeOn*/WakeOnSlotTransition attributes |

**Game-binding subset in the public contract (bold above):** the five `I*Bus : IEventBus` + `IGameServices` (`Bus/`), `LayerType.CombatFeedback` (`Display/LayerType.cs:39`), and `OwnershipMode` (`Enums/OwnershipMode.cs:10` — "Used by both components (Components.Magic) and events (Events.Magic), hence it lives in Contracts"). `GridVector`/`EntityId` are arguably engine-generic. **Structural force:** game types get hoisted into `Contracts` precisely when two game assemblies must share them (`OwnershipMode`, `LayerType` both document this rationale) — so the public contract accretes game vocabulary by design, not by accident.

### What a mod reaches through Contracts alone

`IModApi` (12 members, `Contracts/Modding/IModApi.cs:28`): `RegisterComponent<T> where T:unmanaged,IComponent` (`:38`), `RegisterManagedComponent<T> where T:class,IComponent` (`:59`), **`RegisterSystem<T> where T:class` (`:66`)** — the only system path, runtime-gated to `SystemBase` (§C-1); `Publish<T>`/`Subscribe<T> where T:IEvent` (`:72,:78`); `PublishContract<T>`/`TryGetContract<T>` (`:85,:93`); `GetKernelCapabilities()` (`:100`); `GetOwnManifest()` (`:105`); `Log(...)` (`:110`); `Fields` (`:117`, production-null); `ComputePipelines` (`:126`, hardwired null). `IMod` (`IMod.cs:11`): `Initialize(IModApi)`, `Unload()`. `IModContract` (`IModContract.cs:12`): empty marker, the sole legal mod-to-mod channel.

### Mod inventory (`mods/`)

All manifests `manifestVersion: "3"`. The six vanilla mods: `version 0.1.0`, `apiVersion ^1.0.0`, empty `replaces`, **empty `capabilities.required/provided`**, each `.csproj` sets `<IsVanillaMod>true` and references **only** `DualFrontier.Contracts`.

| Mod | id | kind | entryType | deps |
|---|---|---|---|---|
| Mod.Example | `dualfrontier.example` | regular (default) | `…ExampleMod` | [] |
| Vanilla.Core | `dualfrontier.vanilla.core` | **shared** | `""` (type-vendor) | [] |
| Vanilla.Combat | `dualfrontier.vanilla.combat` | regular | `…CombatMod` | vanilla.core ^0.1.0 |
| Vanilla.Magic / Inventory / Pawn / World | `dualfrontier.vanilla.*` | regular | `…{Magic,Inventory,Pawn,World}Mod` | vanilla.core ^0.1.0 |

**Every entry class is a hollow `IMod` skeleton registering nothing** — `CombatMod.cs` comment: real `DualFrontier.Systems.Combat` systems "remain in place," migration "in M9." К-L9 «vanilla = mods» is realized at the *project* level but aspirational at the *content* level (anomaly A7). The one fixture escaping Contracts is `Fixture.RegularMod_ReplacesCombat` (§R3).

---

## R6 — T5: Composition, presentation, persistence, native boundary

- **GameBootstrap** (§C-6): 21 component regs, 10 systems, map/pawn/item factories, 5 render subs, hardcoded consts `:58-68`. Note `PositionComponent` used generically in the item-exclusion logic (`:114`) — relevant to decision #6.
- **`ScenarioDef` / `SceneMetadata`** (§C-10): both orphan; `EtherDensity` has zero consumers; `ScenarioDef.MapWidth=100` ≠ live `GameBootstrap.MapWidth=200`.
- **Presentation** (§C-8): 6 command records + `LayerType` (SimState/Intent/CombatFeedback/Static) + `RenderCommandDispatcher.cs:42-49` command switch.
- **Persistence** (§C-9): `PawnSnapshot`/`StorageSnapshot`/`TileMapSnapshot(TerrainKind[])`/`WorldSnapshot` + `Compression/` encoders; the assembly ships production-disconnected. A "generic engine snapshot" today would lose every needs/job/terrain/storage field (inventory only — no design).

### Native boundary — **CLEAN** (verified)

- **Gameplay vocabulary in the native tree — none.** `rg -oiw 'Pawn|Combat|Magic|Golem|Inventory|Workbench|Faction|Raid|Weather|Terrain|Mana|Ether' native/DualFrontier.Core.Native -g '*.{h,hpp,c,cpp,cc,cxx}' | wc -l` → **0** standalone game-noun words. The non-word-bounded seed grep returns 135, of which 131 are `managed`/`whether` substrings (`rg -oi 'managed|whether' … | wc -l` → 131). All filenames are engine primitives (`world`, `sparse_set`, `system_graph`, `thread_pool`, `bus_*`, `pipeline_slot`, `tile_field`, `df_capi`).
- **C ABI game-semantic exports — zero.** `df_capi.h` has 154 `DF_API` exports (`rg -c 'DF_API' native/DualFrontier.Core.Native/include/df_capi.h` → 154); `rg -i 'pawn|combat|magic|golem|inventory|faction|raid|weather|terrain|\bmana\b|\bether\b' native/DualFrontier.Core.Native/include/df_capi.h | wc -l` → **0**. The world is type-erased (`uint32_t type_id` + byte size). **The native kernel is already exactly where the target model wants it.**

---

## R7 — T6: Scale, decision list, anomalies

### Scale (canonical `rg --count-matches` totals; word-bounded nets out substring noise)

Engine C# gameplay-vocabulary census. Pattern: `rg -oi 'Pawn|Combat|Magic|Mana|Ether|Golem|Inventory|Workbench|Faction|Raid|Weather|Terrain' src/DualFrontier.<P> -g '!**/obj/**' | wc -l` (seed total); `rg -oiw '<same>'` (word-bounded); `rg -oi 'managed|unmanaged|management|manager'` (FP). **These are canonical `--count-matches` totals (`rg -o | wc -l`); the recon subagents' figures were `rg -c` matching-line counts — do not conflate.**

| Project | seed total | word-bounded | managed-FP | genuine binding |
|---|---|---|---|---|
| Core `[E]` | 106 | 12 | 48 | thin — the `Bus/GameServices.cs` domain-bus taxonomy |
| Contracts `[E]` | 183 | 56 | 62 | yes — 5 bus ifaces + IGameServices + LayerType.CombatFeedback + OwnershipMode |
| Core.Interop `[E]` | 113 | **3** | 108 | **none** — doc examples only |
| Application `[E]` | 344 | 52 | 143 | heavy — composition root |
| Launcher `[E]` | 120 | 23 | 1 | heavy — presentation |
| Runtime `[E]` | 54 | 9 | 44 | **none** — doc examples only |
| native | 135 | **0** | 131 | **none (clean)** |

Type/edge scale: 4 engine→game `ProjectReference` edges; 1 engine→game IVT; 1 test-fixture leak; Components 26f/28t, Events 53f/53t, Systems 30f/30t, AI 18f/20t; Contracts 65 public types (game-binding subset ≈ 8). `rg --count-matches 'class \w+ : SystemBase' src` → 34, `tests` → 38, `mods` → 0. Concentration is exact: the C# migration surface is `Application` (composition) + `Launcher` (presentation) + the genre taxonomy in `Core`/`Contracts`; `Core.Interop`, `Runtime`, and the entire native kernel are clean.

### Decision list (forks the migration plan must settle — measured context only, no recommendations)

1. **SDK system-contract form.** `SystemBase` cannot relocate to `Contracts` (cycle, A4); a new Contracts-level system abstraction is required. Its shape (interface + engine-side base, source-gen, etc.) is unsettled.
2. **Construction / DI model.** Parameterless `Activator.CreateInstance` (`ModRegistry.cs:287`) forbids constructor dependencies; service-locator vs factory vs registration-time injection is open.
3. **Generic event-hub vs channel-ID.** Five hardcoded genre buses (`IGameServices`, `ModBusRouter`) vs a generic router; the five-property surface is canon per `CONTRACTS.md` + К-L9 uniformity, so any change touches LOCKED law.
4. **Distribution-manifest shape.** Vanilla-as-mods: the 6 off-solution mods (A1), the hollow skeletons (A7), and how a shipped distribution is assembled.
5. **Optional engine modules vs vanilla.** `DualFrontier.AI` (only `Pathfinding` consumed; `BehaviourTree`/`Jobs` dormant); `Persistence` + `Crypto.Future` orphans — engine module, vanilla mod, or deletion?
6. **`PositionComponent` / `HealthComponent` boundary tests.** Which component types are engine-generic (the kernel scans `HealthComponent` as a marker; `GameBootstrap` uses `PositionComponent` generically) vs game-owned.
7. **Persistence ownership** (EAM R12 vacant) — game DTOs in the engine layer; who owns the snapshot boundary.
8. **Config ownership** (EAM R13 vacant) — `ScenarioDef`/`SceneMetadata` orphan + hardcoded `GameBootstrap` consts; who owns tunables.
9. **Game types in `Contracts`** — `LayerType.CombatFeedback`, `OwnershipMode` (and the force that hoists shared game types into the public contract).
10. **`kernel.*` ownership reframing** — the marker-scan republishing vanilla types under `kernel.*` (C-5) must be redefined once vanilla types leave the engine.

### Anomalies (A-numbered — divergences neither the assessment nor the kickoff anticipated)

- **A1** — The 6 vanilla mods are absent from `DualFrontier.sln` (`rg -c 'DualFrontier\.Mod\.Vanilla' DualFrontier.sln` → 0); only `Mod.Example` is in the solution. The projects embodying «vanilla = mods» are not in the built solution graph.
- **A2** — `OwnershipMode` (golem/mage bond states, `Contracts/Enums/OwnershipMode.cs:10`) is a game-domain enum in the public contract, not on the assessment's list.
- **A3** — `ScenarioDef.StartingPawnCount` and `SceneMetadata.EtherDensity` are **orphan** (never read in production) — stronger than C-10; ties to EAM R13.
- **A4** — Relocating `SystemBase` to `Contracts` is blocked by a `Contracts → Core.Interop → Contracts` cycle (§C-2). Shapes the whole migration.
- **A5** — `Persistence` and `Crypto.Future` are production-orphaned (corroborates ARCHITECTURE §4 / EAM R12).
- **A6** — `src/Directory.Build.props` scope comment references "net8.0" while the root props sets `net10.0` (doc drift).
- **A7** — The vanilla mods are hollow `IMod` skeletons (empty `Initialize`; `CombatMod.cs` defers migration to "M9"). «vanilla = mods» is aspirational at the content level.
- **A8** — `rg` availability differs by environment: it is on PATH in this session (ripgrep 14.1.1) but was absent for the recon subagents, whose figures were therefore `rg -c` matching-line counts. All §R6/§R7 census figures here are canonical `--count-matches` totals.
- **A9** — `KernelCapabilityRegistry` — the surface that authors the `kernel.*` capability set — lives in `DualFrontier.Application` (the game composition layer), not in a kernel assembly, despite the name.

### Self-check (kickoff vs standing law)

**No kickoff↔standing-law conflict found.** The kickoff's target model ("game code exists only as mods") is a *further increment* beyond К-L9 «vanilla = mods» (LOCKED), which moved vanilla *systems* into mod projects while leaving vanilla *data types* kernel-owned. The current corpus (`ARCHITECTURE.md` Domain layer §1/§2; `MOD_OS_ARCHITECTURE.md` §3.4; `EXECUTION_AUTHORITY_MATRIX.md` §3.3) documents the game-in-engine couplings as intended present-tense law. The audit therefore measures current→target distance, and every C-verdict is simultaneously "EXACT (claim matches code)" and "conformant to the standing corpus." The forthcoming boundary-law document — not a ROADMAP row — is needed because no matrix row or architecture section yet governs the game-as-mods axis; `EXECUTION_AUTHORITY_MATRIX.md` §3.0 supplies the discipline template (named gate conditions + equivalence-proof obligation + a **deletion trigger**) that the migration contract will need.

---

## Attestation

- **Read-only law honored.** Zero writes beyond this single report file; zero git mutations (no branch, commit, or stage — the report is UNTRACKED); zero builds; zero test runs; `dotnet run … Governance -- sync` **never run**. Derived registers and frontmatter were read directly only.
- **HEAD pinned:** `main` @ `4c58942e970435c56bb1034c932d59aa18709a64` (`4c58942`). All anchors reference this tree; the two crux files were re-verified first-hand at this HEAD with zero drift from the LOCKED docs' `35364c2` anchors.
- **Method:** three parallel read-only recon passes (assembly graph; Contracts surface + system-authoring path; engine-side coupling + native check), followed by first-hand re-verification of every load-bearing anchor and canonical `rg --count-matches` re-measurement of the censuses. Every census names its exact rg expression; every claim is anchored `file:line`; quotes are verbatim.
- **Wall-clock:** single continuous session on 2026-07-18. Measured recon-pass durations (parallel): ~6.2 min / ~7.8 min / ~13.3 min; total session (governance-doc reads + first-hand verification + authoring) ≈ 40 min. This total is not independently timestamped (no wall-clock instrument available); the per-pass figures are the tool-reported measured components.
- **Completeness:** T1–T6 all completed; no section left partial. The one executor-flagged open leg (C-4 scheduler-metadata + bus-router) was closed first-hand (`ModBusRouter.cs:28,42`; `SystemAccessAttribute` in `Contracts/Attributes`).

---

*End of report.*
