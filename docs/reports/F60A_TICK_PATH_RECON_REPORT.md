---
register_id: DOC-E-F60A_TICK_PATH_RECON_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-08-25'
last_modified: '2026-08-25'
content_language: en
next_review_due: null
review_cadence: none-historical-record
title: 'F-60(a) TICK-PATH RECON REPORT — 2026-08-25 (R1–R5) — tick-path ALC-root measurement at HEAD 3173d2b for the F-60(a) charter: the bisect lens (0 ticks clean / 1 tick leaks) exonerates every load-time-populated container and demands a lazily-populated one; the tick path (ExecuteTick -> ExecutePhase -> ShouldRun -> Push/Update/Pop -> FlushDeferred) contains exactly ONE container lazily keyed by a regular-mod-ALC type: TickScheduler._tickRateCache (ConcurrentDictionary<Type,int>, first-ShouldRun GetOrAdd, key includes each SystemAdapter<T> closed generic by documented design, ZERO evictions — Reset() has no production caller and the TickScheduler instance survives every ModIntegrationPipeline Rebuild via the readonly _ticks field); adjacent same-class members keyed by SHARED-ALC event types (DomainEventBus.ModeCache STATIC, BusFacade._typeIdCache/_tierCache, DomainEventBus._handlers key retention after full unsubscribe, IntentBatcher._pending) are invisible to the F-60(a) pin but form the same defect class (lazy Type-keyed tick-path containers with no unload eviction); SystemExecutionContext/ModQuarantine/scheduler _contextCache/CWT bindings read clean; anomalies A1–A3'
special_case_rationale: 'Durable-report recon enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-F59_F60_IDENTITY_RECON_REPORT, DOC-E-W2_BUS_CAPABILITY_RECON_REPORT, DOC-E-F29_NATIVE_SCHEDULER_RECON_REPORT). Pre-deliberation grounding for the F-60(a) tick-path charter (program order operator-ratified 2026-08-25: F-60(a) next, LINUX_PRESENT_1 second). Measured by the architect seat directly (live line-reads at HEAD; ID-A precedent). Read-only: zero repository mutations besides this file, sync never run, zero builds/tests beyond the session-restore verification already recorded. UNTRACKED at authoring — enrolls at the charter cascade C1.'
---

# F-60(a) TICK-PATH RECON REPORT — 2026-08-25

Tick-path ALC-root measurement for the **F-60(a) charter** (the identity family's only
open item — ledger row F-60, half (a)). Read-only measurement: **one report, zero
repository mutations, `sync` never run.** This document produces facts, anchors, and
counts — **not designs, not recommendations**. Design belongs to the chartering
deliberation that consumes this report.

**Mission.** F-60(a) (S1): a mod that ticks even ONCE leaks its collectible ALC at
unload — the §9.5 step-7 WeakReference spin runs its full 10 s and reports
`ModUnloadTimeout`. The ID-A bisection (ledger row F-60, EVT-2026-08-20) established
the root is created ON THE TICK PATH, not by component identity: 0 ticks -> 3 ms clean;
1 tick -> 10,459 ms; 16/100/340 ticks -> statistically identical to 1. Affects every
mod that ticks, not only component-defining ones. Pins:
`WeatherWaveGateTests.Unload_LeaksTheModAlc_RootIsOnTheTickPath_NotTheTypeRegistry`
(tests/DualFrontier.Modding.Tests/Weather/WeatherWaveGateTests.cs:228, carries its flip
condition) and `Unload_WithoutTicking_ReleasesTheModAlc_Immediately` (:257, the clean
control).

**HEAD pinned** to `main` @ `3173d2b0cf3ec3780ab0b5609ec23d00b478c03d` (`3173d2b`,
the PR #51 merge), working tree clean at measurement (sole modification:
`.claude/settings.local.json`, session-local, out of corpus scope). Every figure below
is anchored `file:line` at this HEAD.

---

## R1. The discriminating lens: what the bisect already proves

The ID-A bisection varied ONE variable (tick count between load and unload) against the
production composition. Its two ends discriminate the entire candidate space:

- **0 ticks -> immediate release.** Every container populated at LOAD or REBUILD time
  is therefore exonerated wholesale: the scheduler's `_contextCache`
  (ParallelSystemScheduler.cs:114-123, rebuilt and swapped at Rebuild :247-259), the
  `DomainEventBus._handlers` subscription made at `InitializeAllSystems`
  (ParallelSystemScheduler.cs:136-154 pushes context around `system.Initialize()`),
  the ModRegistry/ledger state, and the ID-A CWT re-key
  (ComponentTypeRegistry.cs:109, `ConditionalWeakTable<Type, Binding>`) — all of these
  exist in the 0-tick case too, and the 0-tick case releases in 3 ms.
- **1 tick -> full leak; more ticks change nothing.** The root is therefore written by
  the FIRST traversal of the tick path and is idempotent under repetition — the
  signature of a memoising `GetOrAdd`, not of an accumulating queue.

The root must be: (a) populated lazily on the tick path, (b) keyed or valued by
something reaching the REGULAR mod's collectible ALC, (c) never evicted at unload.

## R2. The tick path, enumerated

`ParallelSystemScheduler.ExecuteTick` (ParallelSystemScheduler.cs:215-221) ->
`ExecutePhase` per phase (:165-208). Inside the `Parallel.ForEach` body per system:

1. `_ticks.ShouldRun(system)` — TickScheduler.cs:54-67. **Writes a cache** (R3).
2. Quarantine consult `IsQuarantined` (:274-277) — reads `_systemMetadata`, holds
   string modIds only. No write on the healthy path; on fault, `ModQuarantine`
   stores a STRING modId. Clean.
3. `SystemExecutionContext.PushContext` (SystemExecutionContext.cs:165-174) —
   `ThreadLocal<SystemExecutionContext?>` slot (:34); `PopContext` (:182-185) nulls it
   in a `finally` (ParallelSystemScheduler.cs:200-203). Balanced; the context object
   itself holds a NAME STRING, origin enum, modId string, kernel-side handles
   (:36-50) — no `Type`, no system instance. Clean.
4. `system.Update(delta)` — the mod system body; publishes via
   `SystemBase.Services` -> `BusFacade`/`DomainEventBus` (R4).
5. After the phase barrier: `FlushDeferred` (DomainEventBus.cs:115+) — drains
   `_deferred` (ConcurrentQueue) into a local batch; queue drained, batch local. The
   EVENT INSTANCES pass through; retention is via the Type-keyed dictionaries (R4),
   not the queue.

## R3. The prime candidate: `TickScheduler._tickRateCache`

`TickScheduler.cs:30`:
`private readonly ConcurrentDictionary<Type, int> _tickRateCache = new();`

- **Lazily populated on the tick path**: `ShouldRun` (:63-64) does
  `_tickRateCache.GetOrAdd(system.GetType(), ...)` — the first `ExecutePhase`
  traversal inserts the entry. Zero ticks -> no entry. One tick -> entry forever.
  `GetOrAdd` inserts once — repetition changes nothing. This matches the bisect's
  shape EXACTLY.
- **The key reaches the regular mod's ALC by documented design**: the comment at
  :59-62 says the cache is keyed "by concrete type (stable per type, incl. each
  distinct `SystemAdapter<T>` instantiation)". `SystemAdapter<TSystem>`
  (SystemAdapter.cs:45) is a generic closed over the MOD's system type; a closed
  generic constructed over a collectible-ALC type argument is itself
  collectible-context-bound, so a strong `Type` key roots the entire ALC. The same
  holds for a mod system deriving `SystemBase` directly (its own `Type` is the key).
- **Never evicted**: `Reset()` (:73-77) clears the cache and self-describes as
  "Intended for test isolation and future mod hot-reload" — it has **ZERO production
  callers** (grep over `src/` finds only Launcher/Compute fence `Reset()` calls,
  different objects). The `TickScheduler` instance survives every rebuild: it is the
  scheduler's `readonly _ticks` field (ParallelSystemScheduler.cs:52), and `Rebuild`
  (:231-262) swaps `_phases`, `_contextCache`, `_systemMetadata` — never `_ticks`.
  `ModIntegrationPipeline` calls `Rebuild` at 3 sites (ModIntegrationPipeline.cs:565,
  :818, :902 — the apply/unload paths), so the context cache turns over while the
  tick-rate cache accumulates for the session.

Consistency check against every measured fact: single tick sufficient (GetOrAdd on
first ShouldRun); tick count irrelevant beyond 1 (memoisation); registry exonerated
(different container, ID-A); affects every ticking mod (every system passes ShouldRun
regardless of components); MANAGED-ONLY scheduler surface (ledger's charter pointer).

**Anti-inference duty (Lesson: bisect, don't infer — feedback ledger, ID-A).** This
report deliberately stops at "the only tick-path container lazily keyed by a
regular-ALC-reaching Type at this HEAD". The charter's first executable step must be
the falsification experiment the pin already encodes: evict the entry (or clear the
cache) at the unload rebuild in a harness and observe
`Unload_LeaksTheModAlc_RootIsOnTheTickPath_NotTheTypeRegistry` FLIP while
`Unload_WithoutTicking_ReleasesTheModAlc_Immediately` holds. Only the flip promotes
this from prime candidate to proven root. If it does not flip, R4's members are next,
and the report's enumeration is the search order.

## R4. Adjacent same-class members (invisible to THIS pin, same defect shape)

Lazy Type-keyed containers populated from the tick path with no unload eviction,
whose keys at this HEAD are EVENT types — for the Weather pair those live in the
SHARED ALC (`DualFrontier.Mod.Weather.Contracts`), not the regular ALC the F-60(a)
pin measures. They cannot explain the pinned leak, but they are the same class and
would root a shared ALC the same way:

- `DomainEventBus.ModeCache` — **STATIC** `ConcurrentDictionary<Type, DeliveryMode>`
  (DomainEventBus.cs:25), populated at `Publish` -> `GetDeliveryMode` (:229). Static:
  survives even bus-instance replacement, process-lived.
- `BusFacade._typeIdCache` / `_tierCache` — `ConcurrentDictionary<Type, uint/BusTier>`
  (BusFacade.cs:41-42), populated on the publish path (:62, :70). Session-lived
  (facade instance).
- `DomainEventBus._handlers` — `GetOrAdd(eventType, _ => new List<Subscription>())`
  (:41): `Unsubscribe` removes the SUBSCRIPTION (:78) but the Type key + empty list
  persist. Populated at Initialize (load path — exonerated for the pin by R1), but the
  KEY retention after full unsubscribe is the same rooting shape for shared-ALC types.
- `IntentBatcher._pending` — `ConcurrentDictionary<Type, List<IEvent>>`
  (IntentBatcher.cs:15).
- `ManagedBusBridge._handles` — `ConcurrentDictionary<ulong, GCHandle>`
  (ManagedBusBridge.cs:35) — native-bus arm, dormant while
  `UseNativeBusForDispatch` is off (F-57 fence); GCHandle-on-delegate is the
  strongest rooting primitive of the family when that arm wakes.
- `NativeComponentType._byId` — static `ConcurrentDictionary<uint, Type>`
  (NativeComponentType.cs:55) — the ledgered legacy false-lead arm (F-60 row); its
  production write-sites need a census before any claim.

**No shared-ALC release assertion exists today**: the wave gate pins the REGULAR
mod's ALC only. Whether the shared ALC leaks via these members is UNMEASURED at this
HEAD — a charter scope decision, not a fact this report can assert.

## R5. Surfaces read clean (with the reason)

- `SystemExecutionContext` — ThreadLocal push/pop balanced under `finally`; holds
  strings/enums/kernel handles; no Type, no system reference
  (SystemExecutionContext.cs:34-50, :165-185).
- `ModQuarantine` — string modIds only (consulted ParallelSystemScheduler.cs:178,
  committed :197).
- Scheduler `_contextCache` — load/rebuild-time populated, swapped wholesale at
  `Rebuild` (:247-259); exonerated by the 0-tick control.
- `ComponentTypeRegistry._bindings` — `ConditionalWeakTable` (weak keys), the ID-A
  re-key; proven non-rooting by the ID-A bisection itself.
- `SystemAdapter<TSystem>` statics (`CachedAccess`/`CachedTickRate`,
  SystemAdapter.cs:48-52) — statics of a collectible-context-bound closed generic
  live WITH the ALC; they are what leaks, not what roots.

## Anomalies

- **A1.** The rooting is a DESIGN CONSEQUENCE, not an accident: SystemAdapter.cs:22
  advertises "the per-type tick-rate cache" as the reason adapters are generic per
  wrapped type — the cache and the collectible keys were composed without an eviction
  story. The comment survives any fix and should be re-grounded by the cascade.
- **A2.** `TickScheduler.Reset()` self-describes "future mod hot-reload" — the
  eviction hook was anticipated at authoring time and never wired to the unload path;
  as-is it also resets `_currentTick`, which is NOT unload-neutral (cadence phase of
  every surviving system would shift), so the existing method is not the fix shape.
- **A3.** `DomainEventBus.Unsubscribe` leaves the Type key + empty subscription list
  in `_handlers` after the last handler leaves (:68-83) — benign for kernel-owned
  event types, rooting for ALC-owned ones.

-- End of report. Facts only; the charter deliberation consumes this.
