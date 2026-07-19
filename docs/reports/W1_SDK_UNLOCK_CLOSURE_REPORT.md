---
register_id: DOC-E-W1_SDK_UNLOCK_CLOSURE_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-19'
last_modified: '2026-07-19'
content_language: en
next_review_due: null
review_cadence: none-historical-record
title: 'W1_SDK_UNLOCK closure report -- Wave 1 of the vanilla-separation program: the ratified BD-1 SDK system contract (ISimulationSystem + capability-scoped ISystemContext) + BD-2 unified factory registration landed in Contracts, engine adapter wired, proven by a Contracts-only example mod through the full register->tick->D2-fault->dispose->ALC-unload lifecycle, with the empirical BD-1 gap-proof fixture leak killed; F5 code-truth correction recorded (events route through the live manifest-capability gate)'
special_case_rationale: 'Durable closure report enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-EQ_A4_RENDER_TAIL_CLOSURE_REPORT, DOC-E-BOUNDARY_W0_CLOSURE_REPORT). Records the W1_SDK_UNLOCK cascade evidence per the brief section 7 closure schema. Historical record, no review cadence.'
---

# W1_SDK_UNLOCK -- Closure Report (2026-07-19)

Wave 1 of `VANILLA_SEPARATION_MIGRATION_PLAN` (1.1.0 -> 1.2.0): the SDK surface unlock.
Executed against the ratified brief `tools/briefs/W1_SDK_UNLOCK_BRIEF.md` (DOC-D) on the
`claude/w1-sdk-unlock` branch. MANAGED-ONLY (zero native changes). Not pushed (executor).

## 1. HEAD before / after

- **Before:** `df1541d` (EQ_A4_RENDER_TAIL closure, post-EQ-a).
- **After:** the C7 closure commit carrying this report (following C6 `dce18d6`).

## 2. Per-commit hashes

| Commit | Hash | Summary |
|---|---|---|
| C1 | `d3edbdb` | governance(w1) -- enroll the recon report (DOC-E) + brief (DOC-D); register 361 -> 363 |
| C2 | `6449306` | feat(sdk) -- Contracts SDK surface (ISimulationSystem/ISystemContext/ISystemServices + Sdk handle structs + IWriteBatchCapability) + IPathfindingService relocation + Contracts->Application IVT |
| C3 | `2dea4fa` | feat(sdk) -- SystemAdapter<T> + SystemContextView + unified ModRegistry factory path + widened RegisterSystem + GameBootstrap re-registration + SystemBase attribute hooks; "deferred" marker census 86 -> 88 |
| C4 | `4d86d2d` | feat(mods) -- Mod.Example SDK content (component/system/event, Contracts-only) + Fixture.RegularMod_ReplacesCombat retargeted off Core (leak dead) + reference ratchet |
| C5 | `4264d6f` | test(sdk) -- 7 SDK context behavioural proofs (freshness/access-forms/factory/capability/fault-parity) |
| C6 | `dce18d6` | governance(w1) -- CONTRACTS/ECS/MOD_OS MINOR + plan W1 DONE / BD-1/BD-2 RESOLVED / BD-6 criterion + ROADMAP W1 DONE + F-54 |
| C7 | (this) | governance(closure) -- EVT + closure report + brief EXECUTED |

## 3. Contracts public-surface delta (BD-1)

**+7 public SDK types**, all in `DualFrontier.Contracts` (new TYPES only, so non-breaking;
`ContractsVersion.Current` UNCHANGED):

- `Sdk/ISimulationSystem` (3 members: `Initialize`/`Tick`/`OnDispose`).
- `Sdk/ISystemContext` (15 members: `CurrentTick`; `TryGet`/`Has`/`Get`; `AcquireSpan`/`BeginBatch`; `InternString`/`Resolve`; `CreateComposite` + `CompositeAdd`/`TryGetAt`/`CountFor`/`ClearFor`; `Publish`/`Subscribe`).
- `Sdk/ISystemServices` (1 member: `Pathfinding`).
- `Sdk/SpanScope<T>` (ref struct: `Count`/`Components`/`Pairs`/`Dispose` + 2 nested public `PairsEnumerable`/`PairsEnumerator`).
- `Sdk/WriteScope<T>` (ref struct: `Update`/`Add`/`Remove`/`Flush`/`Cancel`/`Dispose`).
- `Sdk/StringHandle` (readonly struct value handle, forge-proof internal ctor).
- `Sdk/CompositeHandle<T>` (readonly struct value handle, forge-proof internal ctor).

**Relocated in:** `Services/IPathfindingService` (from `DualFrontier.AI`; B-3 SDK sufficiency;
its only types `GridVector` + BCL were already Contracts-safe). **Internal (not surface):**
`Sdk/IWriteBatchCapability` (the engine seam `WriteScope` delegates to; visible to Application
via the new `InternalsVisibleTo`, an engine->engine grant that does not move the BoundaryRatchet).

## 4. Example-mod lifecycle evidence (the wave gate)

| Stage | Evidence | Test anchor |
|---|---|---|
| Author Contracts-only | Mod.Example ProjectReferences == {DualFrontier.Contracts} | `ExampleModReferenceRatchetTests.ExampleMod_ProjectReferences_AreContractsOnly` |
| Register (via adapter) | ISimulationSystem widened path -> SystemAdapter<T> in the scheduler | `SdkContextTests.RegisterSystem_FactoryAndParameterless_BothRegisterCore`; `M73Phase2DebtTests.RegularMod_ReplacesCombat_...` (retargeted fixture loads + registers via the adapter) |
| Tick + access forms | span-read/batch-write/Try-Has-Get/intern-resolve/composite against a live world | `SdkContextTests.AccessForms_DelegateToTheLiveWorld_PreservingSemantics` |
| Faulted tick -> D2 | mod-origin fault CONTAINED + mod quarantined (never rethrown) | `SdkContextTests.FaultedSdkTick_IsContainedByD2_AndQuarantinesTheMod` |
| Dispose + ALC unload | WeakReference(ModLoadContext) released within timeout | `M73Phase2DebtTests.RegularMod_ReplacesCombat_LoadsAndUnloadsCleanly_WrReleasedWithinTimeout` |
| Capability gate (LOUD) | undeclared event -> CapabilityViolationException | `SdkContextTests.ContextPublish_UndeclaredEvent_ThrowsCapabilityViolationLoudly` |
| Per-tick freshness | CurrentTick reads SimTick live, not cached | `SdkContextTests.CurrentTick_ReadsTheLiveSource_NotACachedValue` |

## 5. F4 leak-death census

- `tests/Fixture.RegularMod_ReplacesCombat/Fixture.RegularMod_ReplacesCombat.csproj`: the
  `DualFrontier.Core` `ProjectReference` **REMOVED**; `ReplacementCombatSystem` retargeted
  `SystemBase -> ISimulationSystem`. The fixture -- the recon's named empirical BD-1 gap proof
  (a mod system forced to reference Core to name `SystemBase`) -- is now Contracts-only, proving
  the gap CLOSED.
- **BoundaryRatchet 4+1 UNMOVED**: the fixture's Core edge is a test-fixture edge (not one of the
  6 engine assemblies -> 4 game assemblies counted), so its removal is uncounted -- confirmed by
  the green `BoundaryRatchetTests`.
- **Recon correction:** the `_Alt` sibling fixture was ALREADY Contracts-only (its `Initialize`
  throws; it never names a system), so it needed no change -- the recon's "identical sibling leak"
  claim was wrong.

## 6. Adapter fault-route parity (D2)

`SystemAdapter<T>` IS a `SystemBase`, so a wrapped `ISimulationSystem`'s faulted `Tick` propagates
through `SystemAdapter.Update` into the scheduler's EXISTING per-system D2 catch
(`ParallelSystemScheduler.cs:183-199` -> `SystemExecutionContext.RouteFault`): mod-origin ->
`IModFaultSink.ReportFault` + `ModQuarantine` (contained, `OnModQuarantined` fires), core-origin ->
rethrow (fail-fast). No new fault code. Proven live by
`SdkContextTests.FaultedSdkTick_IsContainedByD2_AndQuarantinesTheMod` (ExecuteTick does not throw;
the mod id is quarantined).

## 7. Final gates (F6 shape + deltas)

- **Builds:** `dotnet build -c Release` and `-c Debug` -> 0W/0E both.
- **Full-sln test:** `dotnet test -c Release` -> 1205 -> **1213 pass / 0 fail / 5 skip** (+8: C4 +1
  reference ratchet, C5 +7 SDK context).
- **Native selftest:** 104 ALL PASSED both configs -- UNCHANGED (native untouched; `git diff
  df1541d..HEAD -- native/` empty).
- **Governance validate --armed:** exit 0 (0 errors, 0 gate findings) after every
  frontmatter-touching commit (C1, C6, C7).
- **Censuses:** DFK-WAIVER src **2=2**; Console.WriteLine src **2=2** (both RestrictedModApi.cs);
  BoundaryRatchet **4+1** UNMOVED. Marker family "deferred" **86 -> 88 sites / 52 -> 54 files** (the
  C2 SDK docs' two documentation sites -- `Sdk/ISystemContext.cs` deliberate-deferral note +
  `Sdk/SpanScope.cs` K7-deferred caveat; pin updated + census-delta recorded in C3); all OTHER
  marker families UNMOVED. `ContractsVersion.Current` UNCHANGED.

## 8. Register / EVT deltas

- **Register documents:** 361 -> **364** (+3: recon report + brief at C1, this closure report at C7).
- **EVT:** 62 -> **63** (+1: `EVT-2026-07-19-W1_SDK_UNLOCK` at C7).
- **Doc bumps:** CONTRACTS 1.0.1 -> 1.1.0, ECS 1.0.1 -> 1.1.0, MOD_OS 1.0.2 -> 1.1.0 (all MINOR,
  stay LOCKED); plan 1.1.0 -> 1.2.0 (Live); ROADMAP Live (W1 DONE + F-54). No KERNEL / K-L change.

## 9. Ratified decisions applied (operator, 2026-07-19) + code-truth corrections

1. **Bus/event gating** -> route `ISystemContext.Publish/Subscribe` through the LIVE
   `RestrictedModApi` manifest-capability gate (F5's `allowedBuses` router/validator was deleted at
   K8.3+K8.4 -- recorded as ROADMAP F-54).
2. **Construction** -> unify the mechanism (one `ModRegistry` factory path); defer the mod-facing
   `IModApi` factory overload (N17) -> CONTRACTS.md MINOR, ContractsVersion unchanged.
3. **Span/batch form** -> scoped `ref struct` (allocation-free by construction).
- Layer correction: `SystemAdapter`/`SystemContextView` placed in `DualFrontier.Application` (not
  the brief's "Core" -- the view needs `RestrictedModApi` + Core.Interop's existing IVT, reachable
  only from Application). `SystemExecutionContext` seed is 8 fields (recon said 7).

## 10. Attestation

Executed per the ratified brief and the three operator decisions. The SDK contract is authored in
reference-free Contracts; the engine adapter inherits the scheduler's D2 machinery by composition;
the example mod proves the full lifecycle Contracts-only; the BD-1 gap-proof leak is dead. All gates
green; no invariant moved; no push. `SystemBase` + `SystemAdapter` are a recorded bridge retiring at
W5. EQ-a and W0 remain closed; W1 DONE; next is W2 (bus/capability taxonomy, BD-3/BD-10).
