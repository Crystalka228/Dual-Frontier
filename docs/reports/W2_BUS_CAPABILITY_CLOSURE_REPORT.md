---
register_id: DOC-E-W2_BUS_CAPABILITY_CLOSURE_REPORT
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
title: 'W2_BUS_CAPABILITY closure report -- Wave 2 of the vanilla-separation program: the genre taxonomy dies in the engine contract. The five genre bus interfaces + IGameServices leave DualFrontier.Contracts for the engine-internal Core.Bus (breaking, ContractsVersion 1->2 MAJOR); the five buses collapse to ONE generic DomainEventBus (getters now cosmetic bridges); KernelCapabilityRegistry becomes an owner-namespaced registration ledger relocated to Core with an EMPTY kernel-provided set + self-access predicate; the dead _allowedBuses fossil retired (F-54 CLOSED). MANAGED-ONLY; sovereign native switch explicitly FENCED (F-57 seeded)'
special_case_rationale: 'Durable closure report enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-W1_SDK_UNLOCK_CLOSURE_REPORT, DOC-E-EQ_A4_RENDER_TAIL_CLOSURE_REPORT, DOC-E-BOUNDARY_W0_CLOSURE_REPORT). Records the W2_BUS_CAPABILITY cascade evidence per the brief closure schema. Historical record, no review cadence.'
---

# W2_BUS_CAPABILITY -- Closure Report (2026-07-19)

Wave 2 of `VANILLA_SEPARATION_MIGRATION_PLAN` (1.2.0 -> 1.3.0): type/bus/capability ownership
(BD-3, BD-10) + the F-54 fossil retirement. Executed against the ratified brief
`tools/briefs/W2_BUS_CAPABILITY_BRIEF.md` (DOC-D) on the `claude/w2-bus-capability` branch.
**MANAGED-ONLY** (zero native changes -- the sovereign native switch stays FENCED). Not pushed
(executor). Operator-ratified in chat 2026-07-19 (C5 mechanism-only scope; the C7 governance plan).

## 1. HEAD before / after

- **Before:** `b2805ea` (W1_SDK_UNLOCK merge, PR #47).
- **After:** the C8 closure commit carrying this report (following C7 `62b281b`).

## 2. Per-commit hashes

| Commit | Hash | Summary |
|---|---|---|
| C1 | `61127cc` | governance(enroll) -- W2_BUS_CAPABILITY brief (DOC-D) + recon report (DOC-E) enrolled |
| C2 | `691aeb2` | refactor(scheduling) -- retire the dead `_allowedBuses` fossil (F-54); `SystemExecutionContext` field + ctor param deleted; scheduler validates `[SystemAccess]` presence without binding buses |
| C3 | `9f7107d` | refactor(bus) -- collapse the five genre buses into one generic router (BD-3b); `ModBusRouter` + `[EventBus]` deleted; mods route by type; `UnifiedGenreBus` bridges the 5 getters over one `DomainEventBus` |
| C4 | `6b0b7d6` | refactor(contracts)! -- genre taxonomy + `IGameServices` leave the engine contract for `Core.Bus` (BD-3a); `[SystemAccess]` loses Bus/Buses; ContractsVersion 1.0.0 -> 2.0.0 MAJOR; manifest apiVersion `^1.0.0` -> `^2.0.0` cascade |
| C5 | `95365af` | refactor(modding)! -- `KernelCapabilityRegistry` -> owner-namespaced registration ledger, relocated Application -> `Core/Modding`; `BuildFromKernelAssemblies` retired -> kernel set empty; `RegisterOwner`/`Owns` self-access (mechanism-only) |
| C6 | `eb7bca8` | test(bus,modding) -- parity (`UnifiedBusParityTests`) + self-access gate (RestrictedModApiV2Tests 23/24) + owner-ledger emission coverage |
| C7 | `62b281b` | governance(reconcile) -- LOCKED docs to W2 code-truth + W2 DONE (CONTRACTS 2.0.0 MAJOR, EVENT_BUS/MOD_OS 1.2.0 MINOR, plan 1.3.0, ROADMAP F-54/F-57) |
| C8 | (this) | governance(closure) -- EVT + closure report + brief EXECUTED |

## 3. The bus collapse (BD-3b) -- five genre buses -> one generic router

The genre taxonomy's premise ("a single bus is a lock-contention bottleneck past ~100 systems")
never materialized, and genre-keyed routing silently dropped cross-genre mod deliveries. C3
collapsed the five `DomainEventBus` instances to **one**: `GameServices` now holds a single
`DomainEventBus`, and the five `IGameServices` getters (`Combat`/`Inventory`/`Magic`/`Pawns`/`World`)
are **cosmetic bridges** that all return it. Delivery is type-keyed, so an event subscribed via any
getter is delivered when published via any other -- proven by `UnifiedBusParityTests`
(`SameGetterDelivery_IsPreserved` + `EventSubscribedViaOneGetter_IsDeliveredViaAnotherGetter`).

The mod routing layer that reflected `IGameServices` properties against each event's `[EventBus]`
attribute -- `ModBusRouter` + the `[EventBus]` attribute -- was **deleted**; mods now route by event
type to the one dispatch through `RestrictedModApi`, gated by `EnforceCapability`. The former
attribute/router tests (RestrictedModApiV2Tests 2-5) were removed; the round-trip test (13) proves
the type-routed path.

## 4. The Contracts departure + MAJOR bump (BD-3a)

The five bus interfaces + `IGameServices` **left `DualFrontier.Contracts`** (git-mv to
`src/DualFrontier.Core/Bus/`, namespace `DualFrontier.Core.Bus`). Removing interfaces from the
contract assembly is a **breaking change**: `ContractsVersion.Current` bumped `1.0.0 -> 2.0.0`
(MAJOR, `ContractsVersion.cs:20`). `[SystemAccess]` lost its `Bus`/`Buses` members and the two
bus-carrying constructors, keeping `(reads, writes)` only. The MAJOR bump cascaded to every manifest
pinning major 1: the two version defaults (`ModManifest.RequiresContractsVersion`,
`ManifestParser` fallback) and the on-disk vanilla + fixture manifests moved `^1.0.0 -> ^2.0.0`
(the deliberate `BadApiVersion` fixture stays `^99.0.0`); the version-gate tests were re-derived
against `Current = 2.0.0`. `IEvent` + the generic-routing marker stay in Contracts.

## 5. The capability ledger (BD-10) -- owner-namespaced, kernel set empty, self-access

`KernelCapabilityRegistry.BuildFromKernelAssemblies()` -- the kernel-assembly reflection scan that
republished every gameplay type as `kernel.*` -- was **retired**. The registry is now an
**owner-namespaced registration ledger**, relocated `Application -> src/DualFrontier.Core/Modding/`:
`RegisterOwner(ownerNamespace, assembly)` (`:61`) emits `<owner>.<verb>:{FQN}` tokens (`kernel` or
`mod.<id>`); the emission rules (IEvent -> tier publish/subscribe, `[ModAccessible]` IComponent ->
read/write, `[Layer]` -> К-L17 tokens, generic/nested skipped) are preserved, parameterized by owner.
The pipeline instantiates it **empty** (`ModIntegrationPipeline.cs:88`, `new()`).

**The kernel-provided FQN set is empty** -- the engine owns no gameplay types (they left at BD-3);
this IS the thesis. Gameplay types still living engine-side are ownerless and ride the v1
grace path this wave. **Self-access** (`Owns`, `:78`): `RegisterOwner` records each FQN under its
owner, and `EnforceCapability` (`RestrictedModApi.cs:246`) consults `Owns($"mod.{modId}", FQN)`
before requiring a declared token -- a mod is auto-granted its own registered types. Per-mod
registration is **mechanism-only** this wave (no producer calls `RegisterOwner` at load -- vanilla
mods define no types yet; wiring deferred to the slice-move wave). Coverage: `KernelCapabilityRegistryTests`
(owner-namespaced emission + ownership) + RestrictedModApiV2Tests 23/24 (self-access grant + cross-owner still gated).

## 6. F-54 -- the dead fossil retired

`SystemExecutionContext._allowedBuses` was captured-but-unread (1 write, 0 reads) since the runtime
isolation guard was deleted at K8.3+K8.4. Operator ratified **retire, not revive** (2026-07-19): C2
deleted the field + ctor param; `ParallelSystemScheduler` now validates `[SystemAccess]` presence
(`AccessDeclaration is null -> throw`) without binding buses. The bus-scoping concept itself dissolved
with the taxonomy (one router, no per-genre scope, BD-3b) -- CONTRACTS.md §6's "real gap" is now moot.
`ISOLATION.md` §178-183 is historical (SUPERSEDED into MOD_OS at the corpus rework). **F-54 CLOSED.**

## 7. Final gates (F6 shape + deltas)

- **Builds:** `dotnet build -c Release` and `-c Debug` -> 0W/0E both (C8 gate).
- **Full-sln test (Release):** `dotnet test DualFrontier.sln -c Release` -> 1213 -> **1215 pass / 0 fail / 5 skip**
  (10 test projects; net +2 -- the C6 parity + self-access/ledger additions minus the C3 genre-attribute/router
  test removals; the 5 skips are the F-10 stress quarantine in Core.Tests).
- **C6 sub-gate (Release, 3 projects):** Core.Tests 106/0/5 (5 = F-10 stress quarantine, +2 parity),
  Modding.Tests 416/0/0 (+3 self-access/ledger; whole suite green confirms the C4 MAJOR-bump manifest
  re-derivations hold), Analyzers.Tests 54/0/0.
- **Native:** `git diff b2805ea..HEAD -- native/` **EMPTY** -- MANAGED-ONLY; the native three-tier bus
  + selftest are untouched, so the selftest is unchanged (not re-run; provably unaffected).
- **Operator real-app smoke (К-L14):** after the C4 breaking change, the operator ran the Launcher with
  live simulation -- "точки двигаются" + exit 0 -- the operator-only live-app verification that the MAJOR
  contract break is runtime-sound.
- **Governance validate --armed:** exit 0 (0 errors, 0 gate findings) after every frontmatter-touching
  commit (C1, C7, C8).
- **Censuses (src, docs-only C7/C8 -> unaffected):** marker family "deferred" net-unchanged at **89/55**
  across the cascade (C3 -1 `EventBusAttribute.cs` deletion, C5 +1 ledger doc -- same-commit deltas
  recorded in CensusMetaTests); all other marker families UNMOVED. DFK-WAIVER src **2=2**; BoundaryRatchet
  **4+1 UNMOVED** (Contracts->Core is an engine-internal move, no engine->game edge). `ContractsVersion.Current`
  1.0.0 -> 2.0.0 (the intended MAJOR).

## 8. Register / EVT deltas

- **Register documents:** 362 -> **363** (+1: this closure report at C8; the recon report + brief were
  enrolled at C1, already in 362).
- **EVT:** +1 (`EVT-2026-07-19-W2_BUS_CAPABILITY` at C8).
- **Doc bumps:** CONTRACTS 1.1.0 -> **2.0.0** (MAJOR, stays LOCKED); EVENT_BUS 1.1.0 -> 1.2.0, MOD_OS
  1.1.0 -> 1.2.0 (MINOR, stay LOCKED); plan 1.2.0 -> 1.3.0 (Live); ROADMAP Live (W2 DONE, F-54 CLOSED,
  F-57 seeded). Code: `ContractsVersion.Current` 1.0.0 -> 2.0.0. No KERNEL / K-L change (native untouched).

## 9. Ratified decisions applied (operator, 2026-07-19)

1. **F-54** -> RETIRE `_allowedBuses` (not revive per-system bus-scoping) -- ratified at W1, applied C2.
2. **Smoke checkpoint** -> the operator smoke-tested the real app after the C4 breaking change before
   C5 continued (Launcher live sim, exit 0).
3. **C5 scope** -> "mechanism-only": the owner-namespaced ledger + self-access predicate are wired into
   the three existing surfaces (registry, `EnforceCapability`, tests) but the per-mod scan is NOT wired
   into the load pipeline (no producer this wave -- avoids an H-GRAMMAR reach), deferred to the slice-move wave.
4. **C7 governance plan** -> ratified "as presented" (CONTRACTS 2.0.0 MAJOR; EVENT_BUS/MOD_OS 1.2.0 MINOR;
   F-54 CLOSED, seed F-57, F-55/F-56 stay OPEN with progress notes).

## 10. Attestation

Executed per the ratified brief and the operator's in-flight decisions. The genre taxonomy is dead in
the engine contract: the bus interfaces + `IGameServices` are engine-internal (`Core.Bus`), the five
buses are one generic router behind cosmetic getters, and the capability ledger owns zero gameplay types
(`kernel.*` empty) -- the measurable expression of "the engine holds no game." The dead `_allowedBuses`
fossil is retired (F-54 CLOSED). The self-access + owner-registration mechanism is in place but inert
(no producer this wave). The sovereign native switch stayed FENCED; its precondition set is seeded as
F-57 (recon A4/A6/A8/A13). All gates green; the operator's live-app smoke confirms the MAJOR break is
runtime-sound; no invariant moved; no push. W0/W1 and EQ-a remain closed; **W2 DONE**; next is W3 (the
walking vertical slice, written fresh).
