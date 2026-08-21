---
register_id: DOC-E-F59_F60_IDENTITY_RECON_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-08-20'
last_modified: '2026-08-20'
content_language: en
next_review_due: null
review_cadence: none-historical-record
title: 'F-59+F-60 IDENTITY RECON REPORT — 2026-08-20 (R1–R5) — component-type + entity-version identity measurement at HEAD ad4e353 for the identity-family charter: ComponentTypeRegistry = 2 strong Dictionaries (Type→id, id→Type) + sequential _nextId, NO deregistration API, reverse map production-dead (0 consumers); native stores_ = unordered_map<u32,RawComponentStore> accepts ARBITRARY u32 (sequentiality is managed-side doc-law only, FNV legacy path proves it), NO store-removal export; К-L4 explicit-registration path (IModApi.RegisterComponent) records OWNERSHIP ONLY (ModRegistry._componentOwners, cleaned at RemoveMod) — the native id is allocated LAZILY by NativeWorld.ResolveTypeId auto-register on first span use, no owner context at that site; leak = Dictionary key roots collectible ALC → §9.5 step-7 spin 100×100 ms full 10 s (measured 10,668 ms, ledger F-60), 5 UnloadMod sites in Weather gate suite pay it; reset = reloaded ALC mints new Type → new id → empty span → re-seed (EntityCount 2), both halves pinned with flip-condition test names; F-59: versions_ per-slot table, ABA law honest native-side (create returns versions_[idx], destroy ++version, recycled slot reuses stored version), WriteBatch flush + destructor auto-flush SILENTLY skip is_alive-failed commands (destructor discards even the count), SpanScope.Pairs code fabricates 0 (C5b) but the :62 property doc still says Version = 1 (stale), EntityId.IsValid and entity_id.h is_valid both carry the flawed index>0||version>0 disjunction (unfixed, IAC §2 pending); IAC §2 is a RATIFIED design fund: option 1 df_world_acquire_versions RECOMMENDED with normative-target signatures, DFK-entity-identity rule sketch, EntityEncoder waiver — but its К-L20-reserved pointer is STALE (К-L20 seated shutdown-quiescence at EQ_A2, К-L21 reserved Mod-API; next free row = К-L22); registry Dictionaries are NOT thread-safe while dispatch runs Parallel.ForEach and first-use registration is reachable from system bodies; BeginModScope/EndModScope/ClearModScope = string-pool co-ownership ONLY (F-58 pointer falsified); anomalies A1–A10'
special_case_rationale: 'Durable-report recon enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-W1_SDK_SURFACE_RECON_REPORT, DOC-E-W2_BUS_CAPABILITY_RECON_REPORT, DOC-E-F29_NATIVE_SCHEDULER_RECON_REPORT, DOC-E-F10_TEST_ISOLATION_RECON_REPORT). Pre-deliberation grounding for the ratified F-59+F-60 identity-family charter (one charter, operator-ratified 2026-08-20). Measured by the architect seat directly in the Claude Code environment (live line-reads at HEAD; the operator-ratified grounding step of the cascade rhythm — recon-session dispatch subsumed, W3 direct-brief precedent). Read-only: zero repository mutations besides this file, sync never run, zero builds/tests. UNTRACKED at authoring — enrolled at the charter cascade C1.'
---

# F-59+F-60 IDENTITY RECON REPORT — 2026-08-20

Component-type and entity-version identity measurement for the **identity-family charter** (F-59 + F-60, one family: identity across ALC/native boundaries — operator-ratified 2026-08-20). Read-only measurement: **one report, zero repository mutations, `sync` never run, zero builds/tests.** This document produces facts, anchors, and counts — **not designs, not recommendations**. Design belongs to the chartering deliberation that consumes this report.

**Mission.** F-60 (S1): a mod that defines a component leaks its ALC at unload and cannot resume state across a reload. F-59 (S2): span/batch surfaces fabricate entity versions, collapsing generation validation to "the index was never recycled". Both live on one axis — *how identity crosses the ALC and native boundaries* — and both interact with F-57(A4) (owner-scoped native type ids) and W7 persistence. The charter must land before W5 (mass vanilla-mod migration puts components into every mod; a leaking, non-resuming reload path is disqualifying there — ledger F-60 row).

**HEAD pinned** to `main` @ `ad4e353507f6aa6c44f7b43357febd1d22225e39` (`ad4e353`, the PR #49 merge), working tree clean at measurement. Every figure below is anchored `file:line` at this HEAD.

**Law in force (cited, not restated).** `KERNEL_ARCHITECTURE.md` Part 0 К-L4 canon (:130-140: "explicit per-mod registration; FNV-1a hash auto-generation prohibited (collision-prone); per-mod registration ensures deterministic ID assignment + cross-mod isolation"; falsifiability: hash auto-generation introduced / cross-mod id collision / compile-time generation; DFK004 Error enforcing) · К-L series state (:68: 22 invariants, К-L20 seated shutdown-quiescence at EQ_A2 :431, К-L21 RESERVED Mod-API forward-compat :441-447) · `IDENTITY_AND_ABI_CONTRACT.md` §1 rows 1-2 + Note 1 (EntityId ABA law, normative, :69) + Note 2 (component ids never cross the save boundary — proposal, :71) + §2 (the version-0 resolution: option 1 `df_world_acquire_versions` RECOMMENDED, normative-target signatures :93-104, DFK-entity-identity rule :125, EntityEncoder waiver :126, IsValid alignment :128) · `ECS.md` §5 (fabricated-version defect) / §6 (entity lifecycle) · `docs/ROADMAP.md` F-ledger rows F-57 (:1117), F-58 (:1118), F-59 (:1119), F-60 (:1120).

---

## R1 — Component identity today: the full chain

### 1.1 The registry (the К-L4 implementation artifact)

`src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs`:

- **Two strong dictionaries + a sequential counter**: `Dictionary<Type, uint> _typeToId`, `Dictionary<uint, Type> _idToType`, `uint _nextId = 1` (0 reserved invalid) (:25-28).
- `Register<T>()` — idempotent on the **`Type` OBJECT**; allocates `_nextId++`, calls `NativeMethods.df_world_register_component_type(_worldHandle, id, size)`, rolls the counter back on native failure (:54-79).
- **No deregistration, no removal, no weakening API of any kind** — the class is append-only for the world's lifetime.
- `Lookup(uint) → Type?` reverse map: **zero production consumers** — the only `_idToType`/`.Lookup(` references in `src/` are the registry's own three lines (:26, :77, :111). The reverse map exists but nothing reads it.
- Not thread-safe: plain `Dictionary`, no locks, no `Concurrent*` (:25-26). See A2 for why this matters.
- Instance-per-`NativeWorld` — independent id spaces per world (:20-21 doc).
- Header doc carries the determinism dependency: "Mod load order matters for id stability across runs. ModLoader must process mods deterministically" (:16-18).

### 1.2 Construction and the two id paths

- `Bootstrap.Run(useRegistry = true)` constructs the registry against the bootstrapped handle (the single point where the handle exists pre-hand-out) and adopts it into the returned world (`Bootstrap.cs:61-82`). `useRegistry: false` = legacy fallback; doc states "no production caller passes false post-K8.3+K8.4" (:39-47).
- Production wiring: `GameBootstrap.cs:86-87` — `Bootstrap.Run(useRegistry: true)` then `VanillaComponentRegistration.RegisterAll(nativeWorld.Registry!)`.
- **Path resolution site**: `NativeWorld.ResolveTypeId<T>()` (`NativeWorld.cs:586-599`) — registry bound → **auto-register on first use** (`_registry.Register<T>()`); registry null → legacy `NativeComponentType<T>.TypeId` = **FNV-1a-32 over `AssemblyQualifiedName`** (`NativeComponentType.cs:25-42`, `[Obsolete]`) plus a diagnostic static `ConcurrentDictionary<uint, Type>` (`NativeComponentTypeRegistry`, :53-61).
- The legacy FNV ids are a pure function of the type NAME — **stable across ALCs**. This is exactly why the original W3 harness (bare `new NativeWorld()`) could not see F-60 (`WeatherHarness.cs:52-57`).

### 1.3 The vanilla set

`VanillaComponentRegistration.RegisterAll` registers **21 engine component types, ids 1-21**, order-pinned by source position (K4-era block 1-17 preserved verbatim; K8.3+K8.4 extension 18-21 appended; the deleted Power pair shifted ids down by 2 with an explicit comment that this is "acceptable because registry ids are deterministic per-run, **not persisted across versions**") (`VanillaComponentRegistration.cs:37-86`). That comment is the shipped ancestor of IAC §1 Note 2 (ids never cross the save boundary).

### 1.4 The К-L4 "explicit per-mod registration" path registers OWNERSHIP, not ids

- `IModApi.RegisterComponent<T>()` (Path α, `unmanaged`) and `RegisterManagedComponent<T>()` (Path β) (`IModApi.cs:28-59`).
- `RestrictedModApi.RegisterComponent<T>()` → `_registry.RegisterComponent(_modId, typeof(T))` — **that is `ModRegistry`, not `ComponentTypeRegistry`** (`RestrictedModApi.cs:98-99,125`).
- `ModRegistry.RegisterComponent(modId, Type)` records `_componentOwners[type] = modId` and throws on a cross-mod claim (conflict names both mods) (`ModRegistry.cs:167-181`). **No native id is allocated here.**
- `ModRegistry.RemoveMod(modId)` **does** clean `_componentOwners` of the mod's `Type` keys at unload (:294-301) — ModRegistry is NOT a leak site; the ownership map is unload-symmetric.
- The mod component's NATIVE id is therefore allocated **lazily**, on first component/span use, inside `ResolveTypeId` — a site with **no owner/mod context** (Core.Interop knows no modId). Call chain measured: `SystemContextView.AcquireSpan<T>()` → `World.AcquireSpan<T>()` → `ResolveTypeId<T>()` → `_registry.Register<T>()` (`SystemContextView.cs:92-95`, `NativeWorld.cs:586-593`).

### 1.5 Native side: stores are keyed by arbitrary u32; sequentiality is a managed-side law only

- `World::register_component_type(type_id, size)` (`world.cpp:264-281`): rejects id 0 and size ≤ 0 (throws → `capi.cpp:253-263` catches → returns 0 to managed, which throws `InvalidOperationException` and rolls back `_nextId`); **idempotent** on same (id, size); **throws** on same id + different size. Storage: `stores_.emplace(type_id, make_unique<RawComponentStore>(size))` — `stores_` is an `unordered_map` (`world.h:74`, :102).
- `get_or_create_store(type_id, size)` creates on demand for ANY u32 (`world.cpp:283-291`) — this is how the legacy FNV path stores components under hash ids with no registration at all.
- **Consequence (fact, not design): the native ABI imposes NO sequentiality or density requirement on component type ids.** The "sequential 1, 2, 3" property lives only in the managed registry and its К-L4 doc.
- **There is no store-removal export** — nothing in `df_capi.h` or `World` deletes a `RawComponentStore`. Per-mod deregistration would require new native surface plus span-safety around store destruction; a registry re-key requires zero native changes for storage.
- Native re-registration guards are selftest-covered (`selftest.cpp:262-275`).

## R2 — F-60 mechanics, measured

### 2.1 The leak half (a)

- Root: `_typeToId`'s **key** and `_idToType`'s **value** are strong references from a session-lifetime engine object to a `Type` in the mod's collectible ALC. Nothing removes them (R1.1).
- The unload chain honestly fights for collectibility everywhere else: `CaptureAlcWeakReference` is `NoInlining` to keep the strong ref out of the spin's stack frame; `TryStep7AlcVerification(modId, alcRef, warnings)` deliberately never takes `LoadedMod`; the spin is the mandatory double-collect GC pump bracket, 100 iterations × 100 ms = 10 s, then appends the `ModUnloadTimeout` warning ("restart the game to fully reclaim memory") (`ModIntegrationPipeline.cs:926-977`).
- Measured cost at W3: **10,668 ms per unload** of a component-defining mod (ledger F-60 row). The Weather gate suite carries **5 `UnloadMod` call sites** (`WeatherWaveGateTests.cs`), ≈53 s of pure spin per suite run until F-60 closes.
- Scope boundary: only REGULAR (collectible-ALC) mods leak. A shared-ALC mod's types live in the non-collectible shared ALC for the session anyway — registry rooting adds nothing. The W3 Weather pair defines its component (`WeatherStateComponent`) in the REGULAR mod.

### 2.2 The reset half (b)

- Reload loads a new collectible ALC → new `Type` object for the same component FQN → `Register<T>` sees an unknown key → allocates a NEW id → the mod's span over its own component reads EMPTY → ensure-singleton re-seeds. Measured: `EntityCount` 2 instead of 1; weather resets rather than resumes (ledger F-60; pinned).
- Both halves are pinned as EXPECTED-DEFECT assertions carrying their flip conditions: `Reload_DoesNotYetAdoptTheSurvivingSingleton_BecauseComponentIdentityIsPerAlc` (flip `EntityCount` 2 → 1 when fixed) and `Unload_LeaksTheModAlc_BecauseTheTypeRegistryStillHoldsItsComponentType` (flip Contains(ModUnloadTimeout) → BeEmpty) (`WeatherWaveGateTests.cs:156-221`).
- The harness is production-faithful since `1614fcc`: `Bootstrap.Run(useRegistry: true)`, real pipeline/ledger/scheduler, one shared `GameServices` (`WeatherHarness.cs:50-84`).
- The surviving singleton's native store and entity persist across unload (that residue is F-58's domain, out of this charter's scope but adjacent: adoption-on-reload is only meaningful because the residue survives).

## R3 — F-59 surface: entity versions

### 3.1 Native version model (honest)

- `versions_` per-slot table, init 0, doubling growth (`world.cpp:10-11`, :65-67); `next_index_ = 1`, index 0 reserved Invalid (`world.h:162`).
- `create_entity`: recycled slot → `EntityId{recycled, versions_[recycled]}`; fresh slot → `{index, versions_[index]}` — a never-recycled index carries **version 0** (:57-72). C5b's narrowing (fabricate 0, not 1) matches this exactly.
- `destroy_entity`: `++versions_[index]` BEFORE the slot returns to the free list; throws `logic_error` while spans or batches are active (:84-93); `flush_destroyed` recycles the index only (:95-108). The ABA law (IAC §1 Note 1): a `(Index, Version)` pair is issued at most once per world lifetime.
- `is_alive`: `index <= 0` → false; else `version == versions_[index]` (:74-78).
- Packing is honest end-to-end: `pack_entity`/`unpack_entity` preserve version (`entity_id.h:19-30`).

### 3.2 The silent-drop site (the fail-open shape)

- `WriteBatch::flush()`: per command, reconstruct `EntityId{index, version}` → `is_alive` fails → **`continue`** — the command is skipped with no error, no log; only the `successful` return count differs (`world.cpp:436-461`).
- The **destructor auto-flush** (managed `using`/Dispose path) inlines the same loop and **discards even the count**, suppressing all exceptions (destructor noexcept discipline) (:328-377).
- Managed `WriteBatch.Flush()` surfaces the applied count (`WriteBatch.cs:122-128`), so a caller COULD compare recorded-vs-applied — but the drop itself is indistinguishable from success unless the caller does.
- This is the mechanism that turned fabricated versions into "the canonical mod loop wrote nothing" (F-59 row): a fabricated `Version = 1` matched no entity, every keyed command dropped silently.

### 3.3 Managed fabrication surface, current state

- `Sdk/SpanScope<T>.PairsEnumerator.Current` fabricates **0** with the corrected W3 comment (`SpanScope.cs:103-109`) — C5b landed. **But the `Pairs` property doc three lines above still says "reconstructed with `Version = 1`"** (`SpanScope.cs:58-67`) — a stale comment C5b missed (A1).
- Engine-side: ≈20 sites across nine systems fabricate version 0, plus `GameBootstrap.cs:241` and `EntityEncoder.cs:85` (census per IAC §2 :83, not re-measured here; version 0 is correct for never-recycled indices and wrong for recycled ones).
- `EntityId.IsValid => Index > 0 || Version > 0` (`src/DualFrontier.Contracts/Core/EntityId.cs:38`) and native `entity_id.h:14-16` carry the same flawed disjunction IAC §2 :128 orders aligned to `Index > 0`. **Unfixed at this HEAD.**
- W3 raised F-59 urgency: `ISystemContext.DestroyEntity` puts destruction in MOD hands, so recycling is mod-reachable; the SDK context already refuses destroys while any span/write scope is open (`SystemContextView.cs:57-71`).

### 3.4 The ratified design fund (IAC §2 — deliberation already done there)

IAC §2 is not a blank page: option 1 (**parallel versions view**, `df_world_acquire_versions` + `df_world_release_versions`, sharing the `active_spans_` mutation-rejection discipline) is RECOMMENDED with normative-target C signatures and the corrected `SpanLease.Pairs` sketch (:87-104); option 2 (extended acquire) rejected as ABI-breaking; option 3 (`EntityRef` index-only type) retained as structural endgame. Consequential amendments listed: teaching-site rewrites, a new analyzer rule **DFK-entity-identity** (flag `new EntityId(<expr>, <integer literal>)` outside Core.Interop internals + fixtures, Error severity) (:125), `EntityEncoder` census-pinned waiver until A7 persistence (:126), `IsValid` alignment (:128).

## R4 — Interaction surfaces

- **F-57(A4), event ids**: `BusFacade.Fnv1a32(FQN)` (`BusFacade.cs:176-187`) — the same algorithm the legacy component path uses. К-L4's scope note explicitly keeps event ids a separate space (KERNEL :138), and A4's plan target is `(providerId, schemaId)`. Whatever identity model the charter fixes for components should be stated FOR components with the A4 interaction recorded, not silently generalized.
- **К-L numbering drift (A5)**: IAC §2 :125 says the DFK-entity-identity rule lands "as the DFK number matching the К-L row this law ratifies as — К-L20 is already reserved post-Mod-API lock". **Stale**: К-L20 was seated as shutdown-quiescence at EQ_A2 (KERNEL :431) and Mod-API moved to К-L21 RESERVED (:441-447). If the identity law is seated as an invariant, the next free row is **К-L22**; the IAC pointer needs a same-cascade correction.
- **Persistence (Note 2 / W7)**: numeric component ids are per-run by shipped comment (R1.3) and by IAC §1 Note 2 proposal (FQN→id map in the save header). Any charter decision that makes ids stable-across-reload must still NOT promise stability across runs/saves — that boundary already has its own proposed law.
- **Mod-scope trio is a false lead for this family**: `NativeWorld.BeginModScope/EndModScope/ClearModScope` (`NativeWorld.cs:810-829`) route to `StringPool` co-ownership windows ONLY (`world.cpp:564-574`, `string_pool.h:20-23`, `df_capi.h:295-296`); no entity/component tracking exists behind them, and the only `BeginModScope` reference in `src/` is the wrapper itself. The F-58 row's investigation pointer ("may carry the native tracking a reclamation pass would need") is hereby MEASURED: it does not.
- **Shared-ALC placement as a non-fix**: defining components in a shared (non-collectible) assembly would sidestep both F-60 halves for that mod — at the price of making every component-defining mod permanently resident (no reclamation, ever). Recorded as a fact of the current mechanics, not a candidate design.

## R5 — Anomalies and decision inputs

**Anomalies:**

- **A1** — `SpanScope.cs:62` property doc still teaches `Version = 1`; the enumerator below it fabricates 0 with the corrected comment. Stale since C5b (`9699aa8`). Comment-only.
- **A2** — `ComponentTypeRegistry` is plain-`Dictionary`, lock-free, while (i) production dispatch runs system bodies through `Parallel.ForEach` (THREADING.md :74) and (ii) first-use auto-registration is reachable from inside system bodies via `AcquireSpan` (R1.4). Today's exposure is narrow (vanilla pre-registers on the main thread; W3 has one mod system), but W5 (many component-defining mods) makes concurrent first-use registration plausible. No defect observed; structural hazard.
- **A3** — К-L4 canon says "explicit per-mod registration"; the shipped mechanism is ownership-only explicit registration (ModRegistry) + **implicit lazy id allocation** (auto-register on first use, no owner context). The letter and the mechanism have drifted; the charter decides which one moves.
- **A4** — `_idToType`/`Lookup` reverse map: zero consumers in `src/`. Free to weaken, replace, or delete.
- **A5** — IAC §2 :125 К-L20-reserved pointer stale (see R4).
- **A6** — `EntityId.IsValid` + `entity_id.h::is_valid` flawed disjunction unfixed (IAC §2 :128 pending; verdict N38).
- **A7** — No native store-removal export; `stores_` is append-only for the world's lifetime (R1.5).
- **A8** — The destructor auto-flush discards the applied-count, so the Dispose path can never detect a version-mismatch drop even in principle (R3.2). The explicit `Flush()` path returns the count; nothing measured compares it.
- **A9** — `NativeComponentType<T>`/`NativeComponentTypeRegistry` legacy FNV path is `[Obsolete]`-retained and still reachable via `new NativeWorld()`/`useRegistry: false`; 14+ test files construct bare `new NativeWorld()` worlds (grep at this HEAD), i.e. a substantial test population still runs on ALC-stable hash ids — the exact fidelity gap that hid F-60 (gate-harness-fidelity lesson).
- **A10** — Registry is per-world; `EngineSession`/menu reload flows that rebuild the world get a fresh id space, so "stability across reload" has two distinct senses: same-world mod reload (F-60's case) vs world teardown/rebuild (out of scope, ids legitimately reset).

**Decision inputs for the charter (questions, not answers):**

1. **Identity key**: what stable identity replaces the `Type` object as the registry's key — and is it owner-scoped (`modId` + type name) per К-L4's cross-mod-isolation clause, or bare FQN? Where does the owner come from, given the auto-register site has no mod context (R1.4)?
2. **Registration moment**: does the charter make the К-L4 letter real (ids allocated eagerly at mod Apply, where modId IS in scope) and demote `ResolveTypeId` auto-register to an engine-internal/cache path — or keep lazy allocation and thread owner context down to it?
3. **Leak mechanics**: which reference is weakened/removed so the ALC dies — and does the id's row survive unload (reserved for re-adoption) or is it removed with the mod?
4. **Re-adoption semantics**: on reload, same key → same id → the surviving native store becomes visible again (weather RESUMES). Is that adoption always wanted, or does it need an owner/schema-compatibility check (component size change across mod versions → native throws on same-id-different-size, R1.5)?
5. **К-L4 disposition**: implementation-artifact change under the existing canon (explicit registration + no hash ids + determinism + cross-mod isolation all preserved) vs a successor/amendment — and if the identity law is seated as an invariant row, it is К-L22 (A5).
6. **F-59 execution**: IAC §2 option 1 is already the ratified recommendation with signatures — the charter's job is sequencing (same cascade as F-60 or sibling), the native+managed edit list, the DFK-entity-identity rule, and the flip of the two F-60 gate assertions plus `SpanWriteRoundTripTests` extension to a recycled-index case.
7. **Thread-safety**: does the reworked registry become synchronized (A2), and is that in-scope for this charter or ledgered separately?
8. **Census/cost**: closing F-60 removes ≈53 s of spin from the Weather suite (5 unload sites) and flips two EXPECTED-DEFECT assertions; the charter's gate should demand the flipped forms.

---

*End of report. Facts measured at `ad4e353`; design and sequencing belong to the chartering deliberation.*
