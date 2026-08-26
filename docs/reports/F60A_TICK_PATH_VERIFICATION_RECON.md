---
register_id: DOC-E-F60A_TICK_PATH_VERIFICATION_RECON
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-08-26'
last_modified: '2026-08-26'
content_language: en
next_review_due: null
review_cadence: none-historical-record
title: 'F-60(a) VERIFICATION RECON — 2026-08-26 — independent read-only re-measurement of DOC-E-F60A_TICK_PATH_RECON_REPORT at the same HEAD 3173d2b by a fresh Opus subagent (operator-requested second instrument): every structural claim on TickScheduler._tickRateCache CONFIRMED (Type-keyed :30, GetOrAdd on first ShouldRun :63-64, Reset() zero production callers — sole caller is the thread-safety test that USES it to reopen its race window, _ticks readonly and untouched by Rebuild, GameBootstrap session lifetime with three long-lived referents); design-target measured (SystemMetadata positional record with ONE production construction site in SystemMetadataBuilder rebuilt at exactly the four load boundaries; ModRegistry.cs:253-254 already reads [TickRate] at load time; ShouldRun has ONE production caller; 25 new TickScheduler() sites = 1 prod + 24 test); adjacent class RE-CLASSIFIED: ModeCache/_handlers key shared-ALC event types in this repro (SharedModLoadContext isCollectible: false BY INVARIANT §1.4 — a shared-ALC release assertion is impossible-by-design), BusFacade has ZERO production construction sites (K10.4 gate), IntentBatcher is DEAD CODE (1 hit = its own declaration), NativeComponentTypeRegistry._byId (NOT NativeComponentType — owning-type correction) is legacy-arm-only and write-only (Lookup has 0 callers); NO test pins the 1-tick minimal repro (pinned pair is 340-ticks-fails / 0-ticks-passes); 11 discrepancies/refinements enumerated'
special_case_rationale: 'Durable-report recon enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-F60A_TICK_PATH_RECON_REPORT, DOC-E-F59_F60_IDENTITY_RECON_REPORT). Operator-requested INDEPENDENT verification pass (2026-08-26, "для точности авторинга брифа проведи разведку субагентом") executed by a fresh read-only Opus subagent against the same HEAD — the census-needs-two-instruments discipline applied to recon itself. Read-only: zero repository mutations besides this file, sync never run, zero builds/tests by the subagent. UNTRACKED at authoring — enrolls at the charter cascade C1 alongside the primary recon.'
---

# F-60(a) — Independent Verification Recon (read-only subagent)

**HEAD verified:** `git rev-parse HEAD` → `3173d2b0cf3ec3780ab0b5609ec23d00b478c03d` — **matches the required HEAD `3173d2b`.**
Working tree at recon time: `M .claude/settings.local.json`, `?? docs/reports/F60A_TICK_PATH_RECON_REPORT.md`. No repository file was edited, no build/test/`dotnet` was run, git state untouched.

---

## PART A — verify the candidate

### A1. `TickScheduler.cs` full read — cache declaration, GetOrAdd site, `Reset()` semantics, `CurrentTick` consumers

**VERDICT: CONFIRMED (with two nuances).**

File is 88 lines (`wc -l src/DualFrontier.Core/Scheduling/TickScheduler.cs` → 88).

| Element | Anchor | Fact |
|---|---|---|
| Type | `src/DualFrontier.Core/Scheduling/TickScheduler.cs:27` | `internal sealed class TickScheduler` |
| Tick counter | `:29` | `private long _currentTick;` |
| **The cache** | **`:30`** | `private readonly ConcurrentDictionary<Type, int> _tickRateCache = new();` — **exactly as claimed** |
| `CurrentTick` | `:36` | `public long CurrentTick => _currentTick;` |
| `Advance()` | `:44-47` | `_currentTick++` — sole mutator of the counter |
| **GetOrAdd population** | **`:63-64`** | `_tickRateCache.GetOrAdd(system.GetType(), static (_, s) => ResolveTicksPerUpdate(s), system)` — **key = `system.GetType()`, exactly as claimed** |
| `ShouldRun` | `:54-67` | null-guard, then the GetOrAdd, then `_currentTick % ticksPerUpdate == 0` |
| **`Reset()`** | **`:73-77`** | `_currentTick = 0; _tickRateCache.Clear();` — **resets BOTH, as claimed** |
| Rate resolution | `:79-87` | `ResolveTicksPerUpdate` reads `system.TickRateDeclaration`; null → `TickRates.REALTIME`; `value > 0 ? value : TickRates.REALTIME` |

`_tickRateCache` has exactly three occurrences in the file (`grep -n "_tickRateCache" src/DualFrontier.Core/Scheduling/TickScheduler.cs`): the declaration `:30`, the `GetOrAdd` `:63`, the `Clear()` `:76`. **There is no eviction path other than `Reset()`, and no per-entry removal at all.**

**Nuance 1 (doc drift, harmless but worth knowing for the fix):** the class XML doc at `:15-17` says "The reflection lookup of `TickRateAttribute` is memoised per concrete system type". As of the W1 BD-1 change the scheduler itself performs **no** reflection — it delegates to the `SystemBase.TickRateDeclaration` hook (`:81`). The reflection now lives in `src/DualFrontier.Core/ECS/SystemBase.cs:79-80` (`GetType().GetCustomAttribute<TickRateAttribute>(inherit: false)`) and, for adapters, in a static-per-instantiation field at `src/DualFrontier.Application/Modding/SystemAdapter.cs:51-52`. Consequence for the design target: **the cache no longer buys avoidance of reflection for adapter-wrapped mod systems at all** — `SystemAdapter<T>.CachedTickRate` is already a `static readonly` computed once per closed generic. The cache's only remaining value is for `SystemBase`-authored systems, whose `TickRateDeclaration` getter does reflect on every call.

**Nuance 2:** `using System.Reflection;` at `:3` is now unused in this file (no reflection API is called in it).

**`CurrentTick` consumers.** Command:
```
grep -rn "CurrentTick" --include=*.cs src/ tests/ tools/
grep -rn "CurrentTick" --include=*.cs src/ | wc -l    →  12
```
Of those 12 `src/` hits, **8 are XML-doc/comment prose**. The **real read sites of `TickScheduler.CurrentTick` in production are 3**:

1. `src/DualFrontier.Application/Loop/GameLoop.cs:132` — `_bridge.Enqueue(new TickAdvancedCommand((int)_ticks.CurrentTick));`
2. `src/DualFrontier.Application/Loop/GameBootstrap.cs:146` — `modRegistry.SetTickSource(() => ticks.CurrentTick);` (this lambda is the SDK tick source; it is captured into every `SystemContextView`)
3. `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs:198` — `OnModQuarantined?.Invoke(modId!, (long)_ticks.CurrentTick);`

Plus the declaration itself at `TickScheduler.cs:36`. Distinct member, do **not** conflate: `SystemContextView.CurrentTick` (`src/DualFrontier.Application/Modding/SystemContextView.cs:47`) and `ISystemContext.CurrentTick` (`src/DualFrontier.Contracts/Sdk/ISystemContext.cs:44`) are the SDK surface, fed by the `GameBootstrap:146` lambda.

Test read sites of `TickScheduler.CurrentTick`: **1** — `tests/DualFrontier.Modding.Tests/Weather/WeatherHarness.cs:65`.

---

### A2. Callers of `TickScheduler.Reset()` across the ENTIRE repo

**VERDICT: CONFIRMED — zero production callers; exactly ONE caller repo-wide, and it is a test.**

Command:
```
grep -rn "\.Reset()" --include=*.cs src/ tests/ tools/
```
26 hits total. Every one of them resolves to a *different* `Reset()` (Vulkan command buffers/fences, `MemoryAllocator`, `PipelineSlotInterop`, `DependencyGraph`, a test double's static `Reset`). Filtering to `TickScheduler`:

| Caller | Origin | Note |
|---|---|---|
| `tests/DualFrontier.Core.Tests/Scheduling/TickSchedulerThreadSafetyTests.cs:47` | **test** | `scheduler.Reset();` inside a 500-iteration loop — **the whole point of the test is to re-empty the cache each iteration so the concurrent-population race window reopens** |

- **`src/` (production) callers: 0.**
- **`tests/` callers: 1.**
- **`tools/` callers: 0.**

Not-a-caller, for completeness: `tests/DualFrontier.Core.Tests/Scheduling/DependencyGraphTests.cs:145` is `graph.Reset()` (`DependencyGraph`, unrelated).

**Consequence for the fix:** killing the lazy cache does not break any production code path, but it *does* break the only `Reset()` caller — see B3.

---

### A3. `ParallelSystemScheduler.cs` — `_ticks` readonly, `Rebuild` does not touch it, `ShouldRun` is the only non-`Advance` tick-path write

**VERDICT: CONFIRMED on all three points.**

| Claim | Anchor | Evidence |
|---|---|---|
| `_ticks` is `readonly` | `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs:52` | `private readonly TickScheduler _ticks;` |
| assigned once, at construction | `:103` | `_ticks = ticks ?? throw new ArgumentNullException(nameof(ticks));` |
| `Rebuild` never replaces/resets it | `:231-262` | `Rebuild` body swaps **only** `_systemMetadata` (`:238` region), builds `newCache`, then `_phases = newPhases; _contextCache = newCache;` and calls `InitializeAllSystems()`. **`_ticks` appears nowhere in the method** — confirmed by the full `_ticks` occurrence list below. |

Every `_ticks` occurrence in the file (`grep -n "_ticks" src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs`) — **4 uses + 1 declaration, none inside `Rebuild`**:
- `:52` declaration
- `:103` constructor assignment
- **`:172` `if (!_ticks.ShouldRun(system)) return;`** — inside `Parallel.ForEach` in `ExecutePhase`
- `:198` `_ticks.CurrentTick` (read-only, quarantine observer)
- `:220` `_ticks.Advance();` — in `ExecuteTick`, after all phases

So the tick-path writes into `TickScheduler` state are exactly two: the `GetOrAdd` reached from `ShouldRun` (`:172`) and `Advance()` (`:220`). **CONFIRMED.**

**Corroborating fact the CONTEXT did not state, and which materially strengthens the candidate:** the scheduler's *other* per-system container, `_contextCache` (`:23` in the excerpt / `private Dictionary<SystemBase, SystemExecutionContext> _contextCache`), is keyed by **`SystemBase` instance**, not by `Type`, **and IS wholly replaced by `Rebuild`** (`newCache` assigned at the bottom of `Rebuild`). `_quarantine` (`:69`, `ModQuarantine`) is keyed by `modId` **string**. Therefore, of the scheduler's own state, **`TickScheduler._tickRateCache` is the only `Type`-keyed container that survives `Rebuild`.**

---

### A4. `SystemAdapter.cs` — generic per wrapped type; mod `SystemBase` systems key the cache with their own ALC-bound Type; construction site

**VERDICT: CONFIRMED.**

`src/DualFrontier.Application/Modding/SystemAdapter.cs` (74 lines):
- `:45` — `internal sealed class SystemAdapter<TSystem> : SystemBase where TSystem : class, ISimulationSystem` — **generic in the wrapped type**, so each distinct mod SDK system yields a distinct closed generic type.
- `:19-22` — the file's own doc states this is deliberate and names the consequence verbatim: *"each distinct mod system yields a distinct `SystemBase` type — keeping the executor's type-keyed logic correct (duplicate detection in `DependencyGraph`, **the per-type tick-rate cache**)"*. The design intent that produces the leak is documented in the source.
- `:44` — `[TickRate(TickRates.REALTIME)]` is a **bridge placeholder** for the DFK013 analyzer, explicitly "read by nobody at runtime" (`:38-43`).
- `:51-52` — `private static readonly TickRateAttribute? CachedTickRate = typeof(TSystem).GetCustomAttribute<TickRateAttribute>(inherit: false);`
- `:67` — `internal override TickRateAttribute? TickRateDeclaration => CachedTickRate;`

**Key-identity reasoning (both authoring paths root the ALC):**
- **SDK path (`ISimulationSystem`)** — the cache key is the closed generic `SystemAdapter<TSystem>` where `TSystem` is a type from the mod's collectible `ModLoadContext`. A generic instantiation over a collectible-ALC type argument is itself owned by that collectible ALC, so holding the closed `Type` keeps the mod ALC alive.
- **Bridge path (`SystemBase`-derived mod system)** — `ModRegistry.RegisterSystem` calls `CreateSystemInstance` (`src/DualFrontier.Application/Modding/ModRegistry.cs:263-264`, body at `:460-479`), so the instance's concrete `GetType()` **is** the mod's own ALC-bound type. `TickScheduler.ShouldRun` keys on `system.GetType()` directly (`TickScheduler.cs:64`). **Same leak, one indirection shorter.**

**Construction site of `SystemAdapter` instances.** Command: `grep -rn "MakeGenericType|typeof(SystemAdapter" --include=*.cs src/ tests/`. Exactly one production site:

```
src/DualFrontier.Application/Modding/ModRegistry.cs:434   private SystemBase CreateContractAdapter(string modId, Type systemType)
src/DualFrontier.Application/Modding/ModRegistry.cs:439       sim = Activator.CreateInstance(systemType);
src/DualFrontier.Application/Modding/ModRegistry.cs:456       Type adapterType = typeof(SystemAdapter<>).MakeGenericType(systemType);
src/DualFrontier.Application/Modding/ModRegistry.cs:457       return (SystemBase)Activator.CreateInstance(adapterType, sim, this, modId, _tickSource)!;
```
Reached from `ModRegistry.RegisterSystem(string modId, Type systemType)` at `:225`, dispatch at `:263-265`. The only other `MakeGenericType` in the repo is unrelated (`tests/DualFrontier.Modding.Tests/Weather/WeatherCapabilityGateTests.cs:191`, `typeof(Action<>)`).

**Weather-fixture corroboration (measured, not claimed by CONTEXT).** Both weather mod systems are SDK-path, so the two cache keys the F-60 repro plants are `SystemAdapter<WeatherSystem>` and `SystemAdapter<WeatherPresentationSystem>`:
- `mods/DualFrontier.Mod.Weather/WeatherSystem.cs:23-24` — `[TickRate(TickRates.NORMAL)] public sealed class WeatherSystem : ISimulationSystem`
- `mods/DualFrontier.Mod.Weather/WeatherPresentationSystem.cs:31-32` — `[TickRate(TickRates.NORMAL)] public sealed class WeatherPresentationSystem : ISimulationSystem`

---

### A5. The pinned tests, the harness, the WeakReference, the 10 s spin

**VERDICT: CONFIRMED, read in full.**

#### The two tests — `tests/DualFrontier.Modding.Tests/Weather/WeatherWaveGateTests.cs`

**`Unload_LeaksTheModAlc_RootIsOnTheTickPath_NotTheTypeRegistry`** — `[Fact]` at `:227`, method at `:228-241`, doc `:205-226`.
Body verbatim shape:
```
using var h = new WeatherHarness();
h.ApplyWeatherPair().Success.Should().BeTrue();
h.Tick(TicksPastFirstTransition);                       // TicksPastFirstTransition = 340 (:36)
IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadMod(RegularId);
warnings.Should().Contain(w => w.Message.Contains("ModUnloadTimeout"), ...)   // :236
```
It asserts the **presence** of a `ModUnloadTimeout` warning — an **EXPECTED-DEFECT assertion** (`:222-225`: *"Kept as an EXPECTED-DEFECT assertion, deliberately… Flip it to BeEmpty when the tick-path root is closed"*). The bisection table is recorded in the doc at `:215-216`: `0 → 3 ms, no warnings; 1 → 10,459 ms + ModUnloadTimeout; 16 → 10,518 ms; 100 → 10,456 ms; 340 → 10,565 ms`.

**`Unload_WithoutTicking_ReleasesTheModAlc_Immediately`** — `[Fact]` at `:256`, method at `:257-267`, doc `:243-255`.
```
using var h = new WeatherHarness();
h.ApplyWeatherPair().Success.Should().BeTrue();
IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadMod(RegularId);
warnings.Should().BeEmpty(...)                          // :264
```
Identical composition, **no `h.Tick(...)` call**. This is the control that makes the pair a measurement rather than a complaint.

#### The harness — `tests/DualFrontier.Modding.Tests/Weather/WeatherHarness.cs` (105 lines)

Production-faithful composition, mirroring `GameBootstrap.CreateSession`:
- `:58` — `World = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);` (explicitly documented at `:52-57` as load-bearing: a bare `new NativeWorld()` takes the legacy FNV-1a arm and masks the defect)
- `:59-60` — `Registry = new ModRegistry(); Registry.SetCoreSystems(Array.Empty<SystemBase>());`
- **`:64` — `Ticks = new TickScheduler();`**
- `:65` — `Registry.SetTickSource(() => Ticks.CurrentTick);`
- `:78-79` — `Scheduler = SchedulerTestFixture.BuildIsolated(graph.GetPhases(), Ticks, World, services: Services);`
- `:85-87` — `Pipeline = new ModIntegrationPipeline(new ModLoader(), Registry, new ContractValidator(), new ModContractStore(), Services, Scheduler, new ModFaultHandler(), World.Registry);`
- `:90-91` — `ApplyWeatherPair() => Pipeline.Apply(new[] { ContractsPath, WeatherPath })`
- **`:93-97` — `Tick(int count) { for (...) Scheduler.ExecuteTick(1f/30f); }`** — this is the sole path into `ParallelSystemScheduler.ExecutePhase` → `_ticks.ShouldRun` → `GetOrAdd`.

Note: the `TickScheduler` instance is a **harness field** (`internal TickScheduler Ticks { get; }`, `:44`), alive for the whole `WeatherHarness` lifetime; `Dispose()` (`:101`) only disposes the world. So the cache outlives `UnloadMod` and the entire step-7 spin.

#### The WeakReference and the §9.5 step-7 spin — `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs`

| Element | Anchor |
|---|---|
| **Spin constants** | **`:135` `Step7TimeoutMs = 10_000`; `:136` `Step7PollIntervalMs = 100`; `:137` `Step7MaxIterations = Step7TimeoutMs / Step7PollIntervalMs` (=100)** |
| WR capture helper | `:967-968` — `private static WeakReference CaptureAlcWeakReference(LoadedMod mod) => new WeakReference(mod.Context);` |
| WR captured in the unload flow | `:834` — `WeakReference alcRef = CaptureAlcWeakReference(mod);` inside `RunUnloadSteps1Through6AndCaptureAlc` |
| `_activeMods` drop | `:839` — `_activeMods.Remove(mod);` |
| Steps 1–6 helper | `:683-684` — `[MethodImpl(MethodImplOptions.NoInlining)] private WeakReference? RunUnloadSteps1Through6AndCaptureAlc(...)` (non-inlined precisely so no stack local roots the ALC during the spin — rationale `:642-652`, `:674-681`) |
| Spin invocation | `:653` `WeakReference? alcRef = RunUnloadSteps1Through6AndCaptureAlc(modId, warnings);` … `:661` `TryStep7AlcVerification(modId, alcRef, warnings);` |
| **The spin** | **`:1084-1107`** |
| **`ModUnloadTimeout` emission site** | **`:1101-1106`** |

The spin body verbatim (`:1090-1099`):
```csharp
for (int i = 0; i < Step7MaxIterations; i++)
{
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();

    if (!alcRef.IsAlive) return;

    Thread.Sleep(Step7PollIntervalMs);
}
```
and on fall-through (`:1101-1106`):
```csharp
warnings.Add(new ValidationWarning(
    modId,
    $"ModUnloadTimeout: mod '{modId}' assembly load context did not " +
    $"release within {Step7TimeoutMs} ms after Unload (§9.5 step 7). " +
    "The mod has been removed from the active set; restart the game " +
    "to fully reclaim memory."));
```

The unload chain steps, for the cascade's benefit (`:700-829`): step 1 `mod.Api?.UnsubscribeAll()` (`:704`), step 2 `_contractStore.RevokeAll(modId)` (`:711`), step 2.5 `_kernelCapabilities.RemoveOwner("mod." + modId)` (`:721`), step 3 `_registry.RemoveMod(modId)` (`:730`), step 3.5 native unload + `_registry.RemoveSubScheduler(modId)` (`:749-772`), step 3.6 V-resource cleanup (`:789`), **steps 4+5 `_scheduler.Rebuild(localGraph.GetPhases(), newMetadata)` (`:806-819`)**, step 6 `_loader.UnloadMod(modId)` (`:828`).
**Nothing in the chain touches `TickScheduler`.** Step 4/5 is the closest — and it goes through `Rebuild`, which A3 confirms leaves `_ticks` untouched.

---

## PART B — design-target measurements

### B1. `SystemMetadata` — definition, construction sites, and whether a precomputed ticks field is structurally natural

**MEASURED. Verdict: structurally natural, with one boundary detail that must be honoured.**

**Definition** — `src/DualFrontier.Core/Scheduling/SystemMetadata.cs` (15 lines, whole file):
```csharp
internal sealed record SystemMetadata(SystemOrigin Origin, string? ModId);   // :15
```
A positional record in **`DualFrontier.Core`**, 2 members, no behaviour. Doc `:9-11`: *"Application-side `SystemRegistration` projects to this record at bootstrap time; the scheduler stays in Core and does not depend on the modding layer."*

**Construction sites of `SystemMetadata`.** Command `grep -rn "new SystemMetadata(" --include=*.cs src/ tests/` — **2 total, 1 production**:
- `src/DualFrontier.Application/Modding/SystemMetadataBuilder.cs:38` — `lookup[reg.Instance] = new SystemMetadata(reg.Origin, reg.ModId);` (the only production site)
- `tests/DualFrontier.Core.Tests/Scheduling/SchedulerFaultDispatchTests.cs:161` — test helper

**The builder** — `src/DualFrontier.Application/Modding/SystemMetadataBuilder.cs` (42 lines):
```csharp
public static IReadOnlyDictionary<SystemBase, SystemMetadata> Build(ModRegistry registry)   // :31
{
    var lookup = new Dictionary<SystemBase, SystemMetadata>();                              // :35
    foreach (SystemRegistration reg in registry.GetAllSystems())                            // :36
        lookup[reg.Instance] = new SystemMetadata(reg.Origin, reg.ModId);                   // :38
    return lookup;                                                                          // :40
}
```
**Its input is `SystemRegistration`** — `src/DualFrontier.Application/Modding/SystemRegistration.cs:14-17`: `internal sealed record SystemRegistration(SystemBase Instance, SystemOrigin Origin, string? ModId)`. Crucially, **the builder already holds the live `SystemBase` instance** (`reg.Instance`), which is exactly the argument `TickScheduler.ResolveTicksPerUpdate` needs.

**`SystemRegistration` construction sites** (`grep -rn "new SystemRegistration(" --include=*.cs src/ tests/` — 3, all production, all in `ModRegistry`):
- `src/DualFrontier.Application/Modding/ModRegistry.cs:104` — core, factory path
- `src/DualFrontier.Application/Modding/ModRegistry.cs:136` — core, `RegisterSystem<T>` path
- `src/DualFrontier.Application/Modding/ModRegistry.cs:266` — **mod path** (`new SystemRegistration(instance, SystemOrigin.Mod, modId)`), where `instance` is the possibly-adapter-wrapped system

**`SystemMetadataBuilder.Build` call sites** (`grep -rn "SystemMetadataBuilder.Build"` — 5 production + 1 test):
- `src/DualFrontier.Application/Loop/GameBootstrap.cs:210` — initial, core-only
- `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:564` — successful `Apply`
- `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:817` — **`UnloadMod` step 4**
- `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:901` — (rollback / UnloadAll boundary)
- `tests/DualFrontier.Modding.Tests/Sdk/SdkContextTests.cs:226` — test

**Structural verdict.** Adding a precomputed `int TicksPerUpdate` to `SystemMetadata` is natural: the record is a positional record with one production construction site, the builder already has the instance, and the metadata table is rebuilt at **exactly** the four boundaries that matter (bootstrap, Apply, UnloadMod, rollback/UnloadAll) — which is precisely the load-time-resolution cadence the fix wants. `SystemMetadata` is keyed by `SystemBase` **instance**, not `Type`, and the whole dictionary is **replaced** by `ParallelSystemScheduler.Rebuild` (`:238` region → `_systemMetadata = newSystemMetadata`), so a rate stored there is evicted at unload for free.

**Boundary detail that must be honoured:** the value the builder would compute comes from `SystemBase.TickRateDeclaration`, which is **`internal virtual`** (`src/DualFrontier.Core/ECS/SystemBase.cs:79-80`). `SystemMetadataBuilder` lives in `DualFrontier.Application`. This is fine — `src/DualFrontier.Core/DualFrontier.Core.csproj:17` grants `<InternalsVisibleTo Include="DualFrontier.Application" />`. Also note `SystemMetadata` itself is `internal` in Core and Application already consumes it, so no visibility change is needed either way.

**Second boundary detail:** `ParallelSystemScheduler`'s own doc (`:88`, the `systemMetadata` param doc) states that *"Systems absent from the table fall through to `Core`/`null` defaults — covers core systems registered via local arrays in tests where the table is empty."* Several `Core.Tests` scheduler tests pass an **empty** metadata dictionary. If `ShouldRun` is re-plumbed to read the rate from metadata, those tests need a fall-through default (`TickRates.REALTIME`) or migration — see B3.

---

### B2. Consumers of `TickScheduler.ShouldRun`, and of `TickRateDeclaration` / `TickRateAttribute`

**MEASURED.**

Command: `grep -rn "ShouldRun" --include=*.cs src/ tests/ tools/` → 16 hits, of which 5 are XML-doc prose.

**`ShouldRun` call sites — 3 total:**

| Site | Kind |
|---|---|
| `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs:172` | **the ONLY production call site** |
| `tests/DualFrontier.Core.Tests/Scheduling/TickSchedulerThreadSafetyTests.cs:53` | test (parallel pass) |
| `tests/DualFrontier.Core.Tests/Scheduling/TickSchedulerThreadSafetyTests.cs:62` | test (serial re-query) |

Command: `grep -rn "TickRateDeclaration|TickRateAttribute|\[TickRate|TickRates\." --include=*.cs src/ tests/ tools/` → 100 hits.

**`TickRateDeclaration` (the hook) — 4 sites, all structural:**
- `src/DualFrontier.Core/ECS/SystemBase.cs:79-80` — **the base definition**, `internal virtual`, reflects `GetType().GetCustomAttribute<TickRateAttribute>(inherit: false)`
- `src/DualFrontier.Application/Modding/SystemAdapter.cs:67` — the adapter's forwarding override
- `src/DualFrontier.Core/Scheduling/TickScheduler.cs:81` — **the sole reader**
- (`SystemAdapter.cs:24` — doc reference)

**Places that actually reflect `[TickRate]` — 4 distinct mechanisms:**

| # | Site | Mechanism | On the tick path? |
|---|---|---|---|
| 1 | `src/DualFrontier.Core/ECS/SystemBase.cs:80` | `GetType().GetCustomAttribute<TickRateAttribute>(inherit: false)` — **per call**, no memo | **Yes** (via `TickScheduler:81`) |
| 2 | `src/DualFrontier.Application/Modding/SystemAdapter.cs:51-52` | `typeof(TSystem).GetCustomAttribute<...>` — `static readonly`, **once per closed generic** | No (already cached at type-init) |
| 3 | `src/DualFrontier.Application/Modding/ModRegistry.cs:253-254` | `systemType.GetCustomAttribute<TickRateAttribute>(inherit: false)` — **load-time validation gate**; missing → `InvalidOperationException` with remediation hint (`:255-261`) | No — **runs at `RegisterSystem`, i.e. at Apply** |
| 4 | `tools/DualFrontier.Analyzers/Rules/Architecture/DFK013WakeTypeDisciplineAnalyzer.cs:82` | Roslyn **symbol** match on `"DualFrontier.Contracts.Attributes.TickRateAttribute"` — compile-time, **not runtime reflection** | No |

**No other place reflects `[TickRate]`.** In particular `ContractValidator`, the interop layer, and the native-side `WakeRegistryInterop` do not (`WakeRegistryInterop.cs:11` only mentions it in prose; `GameBootstrap.cs:191` likewise).

**Design-relevant consequence:** mechanism #3 (`ModRegistry.cs:253-254`) **already proves every mod system's `[TickRate]` is readable at load time** — the validation gate reads it from the raw `Type` before the instance/adapter even exists. The design target "resolve tick rate at load time" therefore requires no new reflection capability; it only needs the already-read value carried forward instead of thrown away.

**`[TickRate]` attribute application sites**, for blast-radius sizing: **23 in `src/DualFrontier.Systems/`** (production systems), **1 in `src/DualFrontier.Application/`** (the adapter placeholder, `SystemAdapter.cs:44`), **~45 in `tests/`** (32 of which are the synthetic `S00`–`S31` pool in `TickSchedulerThreadSafetyTests.cs:92-123`), **2 in `mods/DualFrontier.Mod.Weather/`**, 1 in `tests/Fixture.RegularMod_ReplacesCombat/`.

---

### B3. Existing tests that pin `TickScheduler` behaviour

**MEASURED.**

Command: `grep -rln "TickScheduler" --include=*.cs tests/` → **22 files**.

**Of those, exactly ONE is a behavioural pin on `TickScheduler` itself:**

**`tests/DualFrontier.Core.Tests/Scheduling/TickSchedulerThreadSafetyTests.cs`** — `[Fact]`/`[Theory]` count = **1** (`grep -c "\[Fact\]\|\[Theory\]"` → 1).
- Class `:24`, the single test `ShouldRun_InvokedFromManyThreadsWithEmptyCache_DoesNotThrowAndReturnsConsistentResults` at `:26-70`.
- **It is a regression guard for the `Dictionary` → `ConcurrentDictionary` fix** (doc `:10-23`, naming commit `e0b0ecf`).
- Its mechanics are *entirely* about the lazy cache: `scheduler.Reset()` at `:47` to re-empty it, 500 iterations (`:45`), 32 distinct `SystemBase` subclasses (`:36`, pool at `:72-83`, types at `:92-123`) chosen (`:31-35`) to force dictionary resizes during concurrent inserts, `Parallel.For` over `ShouldRun` (`:51-54`), then a serial re-query agreement check (`:60-68`).
- **This test cannot survive the fix as written.** If the lazy cache is removed, `Reset()` loses its cache-clearing meaning and the entire race window the test exercises ceases to exist. It must be **migrated** (e.g. re-pointed at whatever concurrent read the new design does perform) or **retired with an explicit note** that the race it guards is structurally impossible once the rate is resolved at load time.

**The other 21 files** use `TickScheduler` only as a **collaborator** — `new TickScheduler()` handed to a `ParallelSystemScheduler` or a `ModRegistry.SetTickSource`. They pin no `TickScheduler` behaviour and will not need migration, but they *are* the compile surface for any constructor-signature change. Full `new TickScheduler()` inventory (`grep -rn "new TickScheduler()" --include=*.cs src/ tests/ tools/` → **25 sites: 1 production, 24 test**):
- production: `src/DualFrontier.Application/Loop/GameBootstrap.cs:90`
- tests (24): `ContractTypeInRegularModTests.cs:136`, `SdkPipelineTests.cs:67`, `SdkContextTests.cs:232`, `WeatherHarness.cs:64`, `ModMenuControllerTests.cs:497`, `M73Phase2DebtTests.cs:125`, `M72UnloadChainTests.cs:346`, `M51PipelineIntegrationTests.cs:137`, `M71PauseResumeTests.cs:221`, `M62IntegrationTests.cs:207`, `PipelineGetActiveModsTests.cs:131`, `M52IntegrationTests.cs:118`, `ModIntegrationPipelineTests.cs:178`, `M73Step7Tests.cs:234`, `ModFaultHandlerTests.cs:210`, `DeferredEventDeliveryTests.cs:89` and `:130`, `SchedulerFaultDispatchTests.cs:157`, `TickSchedulerThreadSafetyTests.cs:29`, `ParallelExecutionTests.cs:33`/`:63`/`:88`, `SchedulerStressTests.cs:236`, `SchedulerExtremeTests.cs:285`.

Also in scope for a metadata-carried rate: the `ParallelSystemScheduler` construction sites that pass an **empty or minimal** metadata dictionary — `tests/DualFrontier.Core.Tests/Scheduling/ParallelExecutionTests.cs:32/62/87`, `SchedulerStressTests.cs:234`, `SchedulerExtremeTests.cs:283`, `SchedulerFaultDispatchTests.cs:156` (metadata helper at `:161`), `DeferredEventDeliveryTests.cs:98/139`, and the modding fixture `tests/DualFrontier.Modding.Tests/Fixtures/SchedulerTestFixture.cs:31`.

---

### B4. Production composition — who constructs `ParallelSystemScheduler` and `TickScheduler`

**MEASURED. The production composition root is `GameBootstrap`, not `EngineSession`.**

Both live in `src/DualFrontier.Application/Loop/GameBootstrap.cs`, inside the same factory method (the `CreateSession` body, params ending `:80`):

| Object | file:line |
|---|---|
| `NativeWorld` | `src/DualFrontier.Application/Loop/GameBootstrap.cs:86` — `Bootstrap.Run(useRegistry: true)` |
| `GameServices` | `src/DualFrontier.Application/Loop/GameBootstrap.cs:89` |
| **`TickScheduler`** | **`src/DualFrontier.Application/Loop/GameBootstrap.cs:90` — `var ticks = new TickScheduler();`** |
| `ModRegistry` | `:144`; `SetTickSource(() => ticks.CurrentTick)` at **`:146`** |
| `SystemMetadataBuilder.Build` (initial) | `:209-210` |
| **`ParallelSystemScheduler`** | **`src/DualFrontier.Application/Loop/GameBootstrap.cs:212-219`** — `new ParallelSystemScheduler(graph.GetPhases(), ticks, initialMetadata, faultHandler, nativeWorld, services, modRegistry)` |
| `ModIntegrationPipeline` | `:225-227` (receives `scheduler` and `nativeWorld.Registry`) |
| `GameLoop` holds the same `ticks` | `src/DualFrontier.Application/Loop/GameLoop.cs:39` `private readonly TickScheduler _ticks;`, assigned `:54` |

**Lifetime consequence:** one `TickScheduler` instance per session, referenced by three long-lived owners (`GameLoop._ticks`, `ParallelSystemScheduler._ticks`, and the `GameBootstrap:146` closure captured into `ModRegistry._tickSource` and thence into every `SystemContextView`). **None of these is dropped or reset by mod unload.** The `_tickRateCache` therefore has full-session lifetime with no eviction — exactly the shape required to root a collectible ALC indefinitely.

`grep -rn "new ParallelSystemScheduler" --include=*.cs src/ tests/ tools/` → 10 sites: **1 production (`GameBootstrap.cs:212`)**, 9 test.

---

## PART C — adjacent Type-keyed containers reachable from mod types

**Sweep command** (basis for the completeness claims below):
```
grep -rn "static.*Dictionary<Type|ConditionalWeakTable" --include=*.cs src/
grep -rn "Dictionary<Type," --include=*.cs src/ | grep -v static
```
Complete inventory of `Type`-keyed containers in `src/`:

| Container | Anchor | Static? | Eviction |
|---|---|---|---|
| `DomainEventBus.ModeCache` | `src/DualFrontier.Core/Bus/DomainEventBus.cs:25` | **static** | **none** |
| `DomainEventBus._handlers` | `src/DualFrontier.Core/Bus/DomainEventBus.cs:27` | instance | `Clear()` only (whole map) |
| **`TickScheduler._tickRateCache`** | **`src/DualFrontier.Core/Scheduling/TickScheduler.cs:30`** | instance | **`Reset()` only — 0 prod callers** |
| `IntentBatcher._pending` | `src/DualFrontier.Core/Bus/IntentBatcher.cs:15` | instance | `FlushAll()`/`Clear()` |
| `BusFacade._typeIdCache` / `_tierCache` | `src/DualFrontier.Application/Bus/BusFacade.cs:41-42` | instance | none |
| `ModRegistry._componentOwners` | `src/DualFrontier.Application/Modding/ModRegistry.cs:36` | instance | **per-mod, at unload** (`:327-334`) |
| `ModContractStore._entries` | `src/DualFrontier.Application/Modding/ModContractStore.cs:15` | instance | `RevokeAll(modId)` (unload step 2) |
| `RestrictedModApi._managedStores` | `src/DualFrontier.Application/Modding/RestrictedModApi.cs:56` | instance | `ClearManagedStores()` (unload step 3) |
| `ComponentTypeRegistry._bindings` | `src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs:109` | instance | **`ConditionalWeakTable` — keys held weakly** (ID-A) |

### C1. `DomainEventBus.ModeCache` — static, population, eviction, bus lifetime

**MEASURED. VERDICT on "static, no eviction": CONFIRMED.**

- Declaration: `src/DualFrontier.Core/Bus/DomainEventBus.cs:25` — `private static readonly ConcurrentDictionary<Type, DeliveryMode> ModeCache = new();`
- **Sole population site:** `:228-229` — `private static DeliveryMode GetDeliveryMode(Type eventType) => ModeCache.GetOrAdd(eventType, ResolveDeliveryMode);`
- **Sole caller of `GetDeliveryMode`:** `:96` — inside `Publish<TEvent>`, with `eventType = typeof(TEvent)` (`:94`). **This is a tick-path write** whenever a mod system publishes from `Tick`.
- Resolver `:231-238` — reflects `[Deferred]` / `[Immediate]`, defaults `Sync`.
- **Eviction: NONE.** `ModeCache` appears exactly twice in the file (`:25`, `:229`). `Clear()` (`:141-145`) and `DropDeferred()` (`:154-158`) touch only `_handlers` and `_deferred` — **they do not touch `ModeCache`**. Being `static readonly`, it is process-lived.

**`DomainEventBus` instance lifetime / ownership.** It is **not** independently constructed anywhere in `src/`. Command `grep -rn "new DomainEventBus" --include=*.cs src/ tests/` → **0 production sites, 6 test sites** (all `tests/DualFrontier.Core.Tests/Bus/BusFaultIsolationTests.cs:45,66,83,98,120,142`).

Production ownership is **one per `GameServices`**, session-lived:
- `src/DualFrontier.Core/Bus/GameServices.cs:23` — `private readonly DomainEventBus _bus = new();`
- `:26` — `public GameServices() => _genre = new UnifiedGenreBus(_bus);`
- Post-W2/BD-3 the five genre getters (`:29-41`) all return the same `UnifiedGenreBus` (`:70-79`) over the same single `_bus` — **there is exactly one managed `DomainEventBus` per session**, not one per domain.
- Production construction: **`src/DualFrontier.Application/Loop/GameBootstrap.cs:89` — `var services = new GameServices();`** (single site). The same instance is handed to both the scheduler (`:218`) and the pipeline (`:226`).

**Leak assessment for the F-60(a) repro specifically:** in the weather scenario the published event type is `WeatherChangedEvent`, declared in `mods/DualFrontier.Mod.Weather.Contracts/WeatherChangedEvent.cs`. That mod's manifest declares `"kind": "shared"` (`mods/DualFrontier.Mod.Weather.Contracts/mod.manifest.json`), so it loads into `SharedModLoadContext`, which is **non-collectible** (C5). **`ModeCache` therefore does not root the regular mod's collectible ALC in this repro** — consistent with the 0-tick control passing. It remains a genuine unbounded static leak for any mod that declares its own event types in its *regular* (collectible) assembly, and it is a structural sibling of the same class as `_tickRateCache`.

### C2. `BusFacade._typeIdCache` / `_tierCache` — population, facade lifetime

**MEASURED. VERDICT: CONFIRMED as unevicted Type-keyed caches — but they are NOT reachable in production at this HEAD.**

- Declarations: `src/DualFrontier.Application/Bus/BusFacade.cs:41-42`.
- **Population sites (2, both `GetOrAdd`, both keyed on `typeof(T)`):**
  - `:62` — `GetOrAssignTypeId<T>() => _typeIdCache.GetOrAdd(typeof(T), static t => Fnv1a32(t.FullName ?? t.Name));`
  - `:70-71` — `GetTier<T>() => _tierCache.GetOrAdd(typeof(T), static t => t.GetCustomAttribute<EventTierAttribute>()?.Tier ?? BusTier.Normal);`
  - Reached from `RegisterEventType<T>` (`:80-81`), `Publish<T>` (`:101-102`), `Publish<T>(T, uint)` (`:135`).
- **Eviction: NONE.** No `Clear`/`TryRemove` anywhere in the file.
- **Facade lifetime — the material finding: `BusFacade` has NO production construction site.** Command `grep -rn "new BusFacade" --include=*.cs src/ tests/` → **0 in `src/`, 9 in `tests/`** (`ManagedBusBridgeTests.cs:57`, `SchedulerStressTests.cs:273`, `SchedulerExtremeTests.cs:351,495,611,719,870,934,1016`). `grep -rn "BusFacade" --include=*.cs src/` returns only the class itself plus 3 doc-comment mentions (`ManagedBusBridge.cs:10,26`, `Display/CombatFeedbackLayer.cs:21`, `Contracts/Bus/EventTierAttribute.cs:37`).
- Its dispatch is additionally gated off by default: `:49` — `public bool UseNativeBusForDispatch { get; set; } = false;` and every `Publish` short-circuits on it (`:100`, `:134`).

**Assessment: parallel/verification infrastructure only at this HEAD (К10.2 "managed-facade-preserved").** It cannot be the F-60(a) root because nothing in production ever constructs it. It becomes a live leak surface at the К10.4 sovereign-authority switch and should be fixed as a rider, not as the cascade's spine.

### C3. `DomainEventBus._handlers` key retention, and the unload-time unsubscribe path

**MEASURED. VERDICT: CONFIRMED — `Unsubscribe` removes subscriptions but never the `Type` key.**

- `_handlers` declaration: `src/DualFrontier.Core/Bus/DomainEventBus.cs:27` — `ConcurrentDictionary<Type, List<Subscription>>`.
- **Key created:** `:41` — `List<Subscription> list = _handlers.GetOrAdd(eventType, _ => new List<Subscription>());` inside `Subscribe<TEvent>` (`:36`), `eventType = typeof(TEvent)` (`:39`).
- **`Unsubscribe<TEvent>` (`:65-83`)** does `_handlers.TryGetValue(...)` (`:68`) then `list.RemoveAt(i)` (`:78`) under lock. **It never calls `TryRemove` on `_handlers`.** The `Type` key survives with an empty (or emptied) `List<Subscription>` attached.
- The only thing that drops keys is `Clear()` (`:141-145`, `_handlers.Clear()`), whose doc says *"Used by tests and scene reloads"*.

**How a mod's subscriptions are removed at unload TODAY:**
1. `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:702-705` — unload **step 1**: `TryUnloadStep(1, modId, warnings, () => { mod.Api?.UnsubscribeAll(); });`
2. `src/DualFrontier.Application/Modding/RestrictedModApi.cs:264-269`:
   ```csharp
   internal void UnsubscribeAll()
   {
       foreach ((IEventBus _, Action unsubscribe) in _subscriptions)
           unsubscribe();
       _subscriptions.Clear();
   }
   ```
   backed by `_subscriptions` at `RestrictedModApi.cs:50` — `private readonly List<(IEventBus Bus, Action Unsubscribe)> _subscriptions = new();`
3. Each captured `unsubscribe` action lands in `DomainEventBus.Unsubscribe<TEvent>` via `UnifiedGenreBus.Unsubscribe` (`src/DualFrontier.Core/Bus/GameServices.cs:77`).
4. `ModRegistry.RemoveMod(modId)` (unload step 3, `ModIntegrationPipeline.cs:730`; body `src/DualFrontier.Application/Modding/ModRegistry.cs:308-335`) drops `_restrictedModApis[modId]` (`:314-318`, after `api.ClearManagedStores()`), the mod's `SystemRegistration`s (`:321-325`), and the mod's `_componentOwners` `Type` keys (`:327-334`). **`_componentOwners` is the one Type-keyed container with a correct per-mod eviction — it is the model the fix should imitate.**

**So: yes, something removes ALL of a mod's subscriptions by owner** (`RestrictedModApi.UnsubscribeAll`, which owns the per-mod tracking list) — **but the `Type` key and its now-empty `List<Subscription>` stay in `_handlers` forever.** `DomainEventBus.Clear()` / `GameServices.Clear()` have **no unload-path caller** (`grep -rn "\.Clear()" --include=*.cs src/DualFrontier.Application/Modding/ src/DualFrontier.Application/Loop/ | grep -i "servic\|bus"` → 0 hits).

The retained `Type` key is a genuine collectible-ALC root **for a mod that declares its event types in its own regular assembly**. In the F-60(a) weather repro it is not (shared, non-collectible ALC — see C1), and it would be populated at **Initialize**, i.e. at Apply, before any tick — which the 0-tick control also rules out as the tick-path root.

### C4. `IntentBatcher._pending` — population, drain semantics, key retention

**MEASURED. VERDICT: NUANCED — the container behaves as described, but it is DEAD CODE.**

- Declaration: `src/DualFrontier.Core/Bus/IntentBatcher.cs:15` — `private readonly ConcurrentDictionary<Type, List<IEvent>> _pending = new();` (class `internal sealed class IntentBatcher` at `:12`).
- **Population site (1):** `Enqueue<TEvent>` `:23-33` — `_pending[typeof(TEvent)] = new List<IEvent>()` at `:29` under `lock (_lock)`.
- **Drain semantics — asymmetric, worth recording:**
  - `Flush<TEvent>()` (`:41-63`) snapshots then `events.Clear()` at `:59` — **clears the LIST, leaves the `Type` KEY.** Key retained.
  - `FlushAll()` (`:70-89`) clears each list *and* `_pending.Clear()` at `:85` — **keys dropped.** (The comment at `:83-84` contradicts the code it precedes: it says "let's just clear all underlying lists" immediately above the line that clears the dictionary. Cosmetic, but a reader trap.)
  - `Clear()` (`:111-117`) — `_pending.Clear()`, keys dropped.
- **Reachability: zero.** Command `grep -rn "IntentBatcher" --include=*.cs .` → **1 hit, the class declaration itself** (`src/DualFrontier.Core/Bus/IntentBatcher.cs:12`). **Nothing anywhere in `src/`, `tests/`, `tools/`, or `mods/` constructs, references, or calls it.** It is not on the tick path because it is not on any path.

**Assessment: not a leak candidate; it is dead code.** Worth a separate disposal finding rather than cascade scope.

### C5. `SharedModLoadContext` / `ModLoadContext` collectibility, and shared-ALC release tests

**MEASURED. VERDICT: the shared context is explicitly NON-collectible; no test asserts shared-ALC release, and none can.**

- **`src/DualFrontier.Application/Modding/SharedModLoadContext.cs:31-34`:**
  ```csharp
  public SharedModLoadContext()
      : base("shared", isCollectible: false)
  ```
  Documented as a lifecycle **invariant** at `:18-19`: *"Lifecycle invariant (§1.4): `IsCollectible = false` — the shared ALC is loaded once at game start and never unloaded during the session."*
- **`src/DualFrontier.Application/Modding/ModLoadContext.cs:29-33`:**
  ```csharp
  public ModLoadContext(string name, SharedModLoadContext? sharedAlc = null)
      : base(name, isCollectible: true)
  ```
  — the per-regular-mod context **is** collectible (doc `:9`).
- Production ownership of the shared context: **`src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:92` — `private readonly SharedModLoadContext _sharedAlc = new();`** — one singleton per pipeline, per session (doc `:142-144`).

**Does ANY test assert shared-ALC release?** Command: `grep -rn "SharedModLoadContext" --include=*.cs tests/` → 12 hits across 6 files (`Sharing/CrossAlcTypeIdentityTests.cs:28,48,70`, `Sharing/SharedModComplianceTests.cs:102,285`, `Sharing/SharedAssemblyResolutionTests.cs:23,47,63`, `Capability/PhaseCSharedProviderTests.cs:140`, `Sharing/ContractTypeInRegularModTests.cs:85,105`). Cross-referenced against `grep -rn "WeakReference" --include=*.cs tests/` → 17 hits across 6 files (`Pipeline/M73Step7Tests.cs`, `Pipeline/M71PauseResumeTests.cs`, `Weather/WeatherWaveGateTests.cs:238`, `Pipeline/ModUnloadAssertions.cs:33,35,50`, `Pipeline/M72UnloadChainTests.cs`, `Pipeline/M73Phase2DebtTests.cs:57,86,97,101,106,113`).

**The two sets are disjoint — zero files contain both.** **No test asserts shared-ALC release.** This is correct-by-construction: a non-collectible ALC cannot be unloaded, so such an assertion would be a permanent red. Every `WeakReference` assertion in the suite targets `LoadedMod.Context` (the collectible `ModLoadContext`) — e.g. `tests/DualFrontier.Modding.Tests/Pipeline/M73Phase2DebtTests.cs:113` `return new WeakReference(loaded.Context);` and the pipeline's own `ModIntegrationPipeline.cs:968` `new WeakReference(mod.Context)`.

### C6. `NativeComponentTypeRegistry._byId` — write sites, and whether the production path writes mod types

**MEASURED. VERDICT: the production path (`useRegistry: true`) NEVER writes it — only the legacy arm does. Also: the CONTEXT names the wrong owning type (see DISCREPANCIES).**

- Declaration: `src/DualFrontier.Core.Interop/Marshalling/NativeComponentType.cs:55` — `private static readonly ConcurrentDictionary<uint, Type> _byId = new();` — **inside `internal static class NativeComponentTypeRegistry` (declared `:53`)**, which is a *separate* static class from `internal static class NativeComponentType<T>` (`:23`). Both are `[Obsolete]` (`:21-22`, `:50-52`).
- **WRITE sites — exactly 1 repo-wide.** Command `grep -rn "NativeComponentTypeRegistry|NativeComponentType<" --include=*.cs .` → 7 hits, of which 5 are the declarations/doc in `NativeComponentType.cs` and 2 are in `NativeWorld.cs`:
  ```
  src/DualFrontier.Core.Interop/NativeWorld.cs:687    uint typeId = NativeComponentType<T>.TypeId;
  src/DualFrontier.Core.Interop/NativeWorld.cs:688    NativeComponentTypeRegistry.Register(typeId, typeof(T));
  ```
  (`Register` itself is `NativeComponentType.cs:57`.)
- **READ sites — zero.** `grep -rn "NativeComponentTypeRegistry.Lookup" --include=*.cs . | wc -l` → **0**. The `Lookup` method (`:59-60`) has no callers at all: `_byId` is write-only.
- **The gating.** Those two lines sit in `NativeWorld.ResolveTypeId<T>()` (`src/DualFrontier.Core.Interop/NativeWorld.cs:661-691`) **inside the `else` fall-through** — they are reached **only when `_registry == null`**:
  ```csharp
  private uint ResolveTypeId<T>() where T : unmanaged
  {
      if (_registry != null)
      {
          if (_registry.TryGetCachedId(typeof(T), out uint bound)) return bound;      // :666-669
          AssemblyLoadContext? context = AssemblyLoadContext.GetLoadContext(typeof(T).Assembly);
          if (!ReferenceEquals(context, AssemblyLoadContext.Default))                 // :672
              throw new InvalidOperationException(...);                               // :674-679
          return _registry.Register<T>();                                             // :684
      }
  #pragma warning disable CS0618
      uint typeId = NativeComponentType<T>.TypeId;                                    // :687
      NativeComponentTypeRegistry.Register(typeId, typeof(T));                        // :688
      return typeId;
  #pragma warning restore CS0618
  }
  ```
- `_registry` is non-null exactly when the world was built with `useRegistry: true` — `src/DualFrontier.Core.Interop/Bootstrap.cs:79` — `useRegistry ? new ComponentTypeRegistry(handle) : null;`, and `Bootstrap.Run`'s default is `useRegistry = true` (`Bootstrap.cs:61`).
- **Production takes the registry arm:** `src/DualFrontier.Application/Loop/GameBootstrap.cs:86` — `Bootstrap.Run(useRegistry: true)`. **The F-60(a) harness likewise:** `tests/DualFrontier.Modding.Tests/Weather/WeatherHarness.cs:58`, with `:52-57` documenting that a bare `new NativeWorld()` (legacy arm) *"silently masked the reload defect this harness exists to catch."*
- Moreover, on the registry arm a **mod** type can never reach implicit registration at all: `:671-679` throws loudly for any type whose `AssemblyLoadContext` is not `Default`, naming `IModApi.RegisterComponent<T>()` as the remedy.

**Conclusion: `NativeComponentTypeRegistry._byId` is unreachable in production and in the F-60(a) repro. It is NOT a candidate.** It is nonetheless a real static `Type`-retaining container, still live for any `useRegistry: false` legacy/diagnostic path, and it is write-only — a clean deletion candidate at the K8 cutover the `[Obsolete]` message already names.

---

## PART D — riders

### D1. `docs/methodology/BRIEF_TEMPLATE.md` — the three stale push mentions

**MEASURED — quoted verbatim at this HEAD.** Command: `grep -n "push" docs/methodology/BRIEF_TEMPLATE.md` (all 9 hits inspected; the three in the 250–270 window are below).

**Line 258:**
```
> self-attestation (no pushes; `sync` run in every frontmatter-touching commit;
```

**Line 261:**
```
> (push; the standing F-queue items that remain operator-owned).
```

**Line 266:**
```
> it -- adjacent cascades, architect-owned findings, the reference tree, pushes,
```

Line numbers match the CONTEXT's `~258/261/266` **exactly**. Immediate context: line 258 sits inside the `## 14. Closure protocol and report [CORE]` block (heading `:248`, guidance `:250-261`); line 266 sits inside `## 15. Out of scope [CORE]` (heading `:263`, guidance `:265-267`).

For completeness, the **other six** `push` mentions in the file are outside the requested window and are *not* stale in the same sense — they state the operative push law rather than assume it: `:120-121` (executor pushes the work branch and opens a PR; pushing `main` and self-merging forbidden), `:237-238` (standing rails, `CODING_STANDARDS.md` §8.4 v3.0.0), `:241` (no rewrite/force-push/squash), `:245` (auto-mode push-to-main re-confirmation).

### D2. `docs/reports/F60A_TICK_PATH_RECON_REPORT.md`

**CONFIRMED PRESENT and UNTRACKED. Not modified, not read for content, not touched.**
```
$ ls -la docs/reports/F60A_TICK_PATH_RECON_REPORT.md
-rw-rw-r-- 1 crystalka crystalka 12769 Aug 26 15:58 docs/reports/F60A_TICK_PATH_RECON_REPORT.md
$ git status --porcelain docs/reports/F60A_TICK_PATH_RECON_REPORT.md
?? docs/reports/F60A_TICK_PATH_RECON_REPORT.md
```
12,769 bytes, mtime 2026-08-26 15:58, git status `??` (untracked).

### D3. Governance — the sanctioned command form and where it is written

**CONFIRMED. Both forms are written verbatim in `docs/methodology/BRIEF_TEMPLATE.md`; there is NO README in `tools/DualFrontier.Governance/`.**

**`sync` form** — `docs/methodology/BRIEF_TEMPLATE.md:47-52` (the "Register discipline (schema 2.0)" bullet), the command spanning `:48-49`:
```
- **Register discipline (schema 2.0)**: frontmatter is the SoT; a governance
  mutation = frontmatter edit + `dotnet run --project
  tools/DualFrontier.Governance -- sync` in the same commit (the derived
  `REGISTER.yaml` + `CURRENT_AUTHORITY_SURFACE.yaml` fold in); `validate
  --armed` exit 0 gates every governance-touching commit; derived artifacts are
  never hand-edited; `AUDIT_TRAIL.yaml` is append-only.
```

**`validate --armed` form** — `docs/methodology/BRIEF_TEMPLATE.md:105-107`, step 4 of the grounding checklist:
```
4. **Validation checkpoint** (schema 2.0):
   `dotnet run --project tools/DualFrontier.Governance -- validate --armed`.
   Exit code != 0 -> HALT H3. Governance test suite green.
```

**Corroborated in the tool itself** — `tools/DualFrontier.Governance/GovernanceCli.cs`:
- `:15` — `bool armed = args.Contains("--armed");`
- `:19` — `"validate" => RegisterSync.Validate(ResolveRoot(args), armed, Console.Out),`
- `:20` — `"sync" => RegisterSync.Sync(ResolveRoot(args), armed, ResolveRegisterVersion(args), Console.Out),`
- `:172` — help text: `"  --armed                    make semantic gate findings exit-affecting (Cascade B; default report-only)"`
- `:6` — class doc: *"are report-only unless `--armed` (FRAMEWORK 14.8)"*
- entry point `tools/DualFrontier.Governance/Program.cs:7` — `return GovernanceCli.Run(args);`

**`FRAMEWORK.md` section 14 states the LAW, not the command string.** `docs/governance/FRAMEWORK.md:772` is `## 14. Register schema 2.0 — the inversion (MAJOR amendment)`; `§14.2` (`:780-782`) states the measure → align → arm discipline and the failability requirement; `§14.8` (`:832-834`) states the deployment discipline and the arming flip. **Neither prints the `dotnet run` invocation.** `§14.9` (`:838-840`) records that the PowerShell writers (`sync_register.ps1`, `render_register.ps1`) are **retired** as of Cascade B (F-34, executed 2026-07-17) — so the `sync_register.ps1 --validate` strings still present in the pre-2.0 body sections (`:479`, `:490`, `:530`, `:585`, `:638`, `:694`, `:710`) are historical, **not** the operative command form.

Other repository sites carrying the same command form (for cross-reference): `tools/briefs/DRAFTS_RATIFICATION_BRIEF.md:34,67`, `tools/briefs/STACK_UPDATE_BRIEF.md:35`, `tools/briefs/BOUNDARY_W0_BRIEF.md:46,66`, `tools/briefs/EQ_A1_FAULT_SYMMETRY_BRIEF.md:31`, `tools/briefs/REGISTER_INVERSION_A_BRIEF.md:64`, `tools/briefs/ID_B_ENTITY_VERSIONS_BRIEF.md:149`, `tools/briefs/ID_A_COMPONENT_IDENTITY_BRIEF.md:134`, `tools/briefs/W3_WEATHER_SLICE_BRIEF.md:135`, `tools/briefs/CORPUS_CLOSURE_INVERSION_B_BRIEF.md:168`, `docs/reports/DRAFTS_RATIFICATION_REVIEW_REPORT.md:40`.

---

## Consolidated verdict on the prime candidate

Every structural precondition for `TickScheduler._tickRateCache` being the F-60(a) root is **independently confirmed at HEAD `3173d2b`**:

1. It is `Type`-keyed (`TickScheduler.cs:30`) and keyed on `system.GetType()` (`:64`) — for SDK mod systems that is `SystemAdapter<TSystem>` closed over a collectible-ALC type argument; for `SystemBase` mod systems it is the mod's own type directly.
2. It is populated **exclusively on the tick path** — the sole production `ShouldRun` caller is `ParallelSystemScheduler.cs:172`, inside `Parallel.ForEach` in `ExecutePhase`. **No load-time or Apply-time path writes it.**
3. It has **no eviction** other than `Reset()`, which has **zero production callers** repo-wide (A2).
4. The owning instance is `readonly` on the scheduler (`ParallelSystemScheduler.cs:52`), **untouched by `Rebuild`** (A3), and session-lived from `GameBootstrap.cs:90` with three long-lived referents (B4).
5. It is the **only** `Type`-keyed container in the scheduler that survives `Rebuild` — `_contextCache` is instance-keyed and fully replaced, `_quarantine` is string-keyed.
6. It is populated on the **first** tick and never again for the same type — matching the bisection signature exactly (0 → clean; 1 ≡ 16 ≡ 100 ≡ 340 → ~10.5 s + `ModUnloadTimeout`).
7. The adjacent same-class members are ruled out for **this** repro: `ModeCache`/`_handlers` key on `WeatherChangedEvent`, which lives in a `"kind": "shared"` mod loaded into the **non-collectible** `SharedModLoadContext` (C1/C5); `BusFacade` has no production construction site (C2); `IntentBatcher` is dead code (C4); `NativeComponentTypeRegistry._byId` is on the legacy `useRegistry: false` arm only (C6); `ComponentTypeRegistry._bindings` is a `ConditionalWeakTable` with weakly-held keys (ID-A).

I did not execute anything, so this is a structural confirmation, not a measurement of the leak itself. The one residual class I did **not** exhaustively sweep is static fields on *generic* types instantiated over mod types in `Core.Interop` (e.g. any `static readonly` on a `Foo<T>` reachable from `AcquireSpan<T>`/`BeginBatch<T>` with `T` = a mod component). Those live *inside* the collectible ALC and so cannot root it from outside, but a static on a **non-collectible-ALC** generic closed over a mod type would. `NativeComponentType<T>.TypeId` (`NativeComponentType.cs:25`) is exactly that shape — and C6 establishes it is unreachable on the production arm.

---

## DISCREPANCIES

Every place my findings differ from, or materially qualify, the claims in CONTEXT:

1. **C6 — wrong owning type named.** CONTEXT says *"`NativeComponentType._byId` static `ConcurrentDictionary<uint,Type>` (`src/DualFrontier.Core.Interop/Marshalling/NativeComponentType.cs:55`)"*. The **file:line is correct**, but `_byId` belongs to `internal static class **NativeComponentTypeRegistry**` (declared at `NativeComponentType.cs:53`), which is a *different* type from `internal static class NativeComponentType<T>` (declared at `:23`) that shares the file. The remediation brief must name `NativeComponentTypeRegistry` or a search for the member will miss.

2. **C1 — `DomainEventBus` ownership is not "per-domain".** CONTEXT asks *"who owns instances — GameServices? per-domain?"*. Post-W2/BD-3 there is **exactly one** `DomainEventBus` per session (`GameServices.cs:23`), and all five genre getters return the same `UnifiedGenreBus` over it (`GameServices.cs:29-41`, `:70-79`). There is no per-domain instance, and there is **no production `new DomainEventBus()` site at all**.

3. **C2 — `BusFacade` is not a live production surface.** CONTEXT frames `_typeIdCache`/`_tierCache` as reachable adjacent members. At this HEAD `BusFacade` has **zero** construction sites in `src/` (9 test-only), and its dispatch is gated off by `UseNativeBusForDispatch = false` (`BusFacade.cs:49`). Its caches are unevictable and genuinely leaky **in shape**, but unreachable **in fact** until К10.4.

4. **C4 — `IntentBatcher` is dead code, not a tick-path container.** CONTEXT asks whether its population is "tick path or not". The answer is *neither*: `grep -rn "IntentBatcher" --include=*.cs .` returns **one hit — the class declaration itself** (`IntentBatcher.cs:12`). Nothing constructs or references it anywhere in the repo.

5. **A1 — the cache no longer memoises reflection for the leaking case.** CONTEXT (and the class's own doc at `TickScheduler.cs:15-17`) frames `_tickRateCache` as memoising a reflection lookup. Since W1 BD-1 the scheduler performs no reflection; for **adapter-wrapped mod systems** — precisely the ones whose keys leak — the value is already `static readonly`-cached per closed generic at `SystemAdapter.cs:51-52`, so the dictionary entry buys **nothing** for them. The cache's only surviving benefit is for `SystemBase`-authored systems, whose `TickRateDeclaration` (`SystemBase.cs:79-80`) reflects on every call. This strengthens rather than weakens the case for removal, but it changes the cost/benefit argument the brief should make.

6. **A2 — "zero production callers" is right, but the single test caller is load-bearing.** CONTEXT says `Reset()` "allegedly has zero production callers". Confirmed. What CONTEXT does not say: the sole caller, `TickSchedulerThreadSafetyTests.cs:47`, uses `Reset()` **as the mechanism that reopens the concurrent-population race** it exists to guard. Removing the lazy cache does not merely orphan `Reset()` — it **invalidates that entire test**, which must be migrated or explicitly retired (B3). Test count for that file is **1**, not a suite.

7. **C3 — the unload path DOES remove all of a mod's subscriptions by owner.** The CONTEXT question implies uncertainty. `RestrictedModApi.UnsubscribeAll` (`RestrictedModApi.cs:264-269`) walks the per-mod `_subscriptions` list (`:50`) and is invoked as unload **step 1** (`ModIntegrationPipeline.cs:702-705`). What is *not* removed is the `Type` **key** in `DomainEventBus._handlers` — `Unsubscribe` (`DomainEventBus.cs:65-83`) never calls `TryRemove`. And `DomainEventBus.Clear()` has **no unload-path caller**.

8. **B4 — the composition root is `GameBootstrap`, not `EngineSession`.** CONTEXT offers both. Both `TickScheduler` (`GameBootstrap.cs:90`) and `ParallelSystemScheduler` (`GameBootstrap.cs:212-219`) are constructed in `GameBootstrap`. `EngineSession` appears in this area only as the recipient of the quarantine observer (`ParallelSystemScheduler.cs:33-36` doc, `EngineSession.ReportDegraded`).

9. **D3 — there is no `tools/DualFrontier.Governance` README, and `FRAMEWORK.md` §14 does not print the command.** CONTEXT asks to confirm the form *"from `tools/DualFrontier.Governance` README or `FRAMEWORK.md` section 14"*. Neither source carries it: `ls tools/DualFrontier.Governance/*.md` and `find tools -maxdepth 2 -name "README*"` both return nothing, and §14/§14.2/§14.8 state the law without the invocation. **The authoritative written form is `docs/methodology/BRIEF_TEMPLATE.md:48-49` (`sync`) and `:106` (`validate --armed`)**, corroborated by the verb dispatch in `tools/DualFrontier.Governance/GovernanceCli.cs:19-20`.

10. **A5 — the leaking test's tick count is 340, not an arbitrary "1".** `TicksPastFirstTransition = 340` (`WeatherWaveGateTests.cs:36`) is what the pinned failing test actually runs (`:232`). The 1-tick datum is recorded only in the doc's bisection table (`:215-216`); **no test in the suite pins the 1-tick case.** If the cascade wants the minimal-repro pinned, it must add that test — the current pair pins only 340-ticks-fails and 0-ticks-passes.

11. **Corroboration CONTEXT did not claim (offered as strengthening, not as a difference):** the F-60(a) repro's event type `WeatherChangedEvent` lives in a `"kind": "shared"` mod (`mods/DualFrontier.Mod.Weather.Contracts/mod.manifest.json`) loaded into the **non-collectible** `SharedModLoadContext` (`SharedModLoadContext.cs:32`, `isCollectible: false`). This independently rules the bus caches out **for this repro** and leaves `_tickRateCache` as the only tick-path-populated container in the sweep holding a **collectible**-ALC `Type`.
