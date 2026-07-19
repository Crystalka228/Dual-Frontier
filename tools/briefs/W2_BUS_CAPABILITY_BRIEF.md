---
register_id: DOC-D-W2_BUS_CAPABILITY_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: Draft
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-19'
last_modified: '2026-07-19'
content_language: en
next_review_due: null
title: W2_BUS_CAPABILITY execution brief (Wave 2 -- genre-taxonomy death in the engine contract + generic managed router + capability ledger, under ratified BD-3/BD-10/F-54-retire; sovereign native switch explicitly fenced)
---

# W2_BUS_CAPABILITY -- Execution Brief

Wave 2 of VANILLA_SEPARATION_MIGRATION_PLAN (Live 1.2.0): the five-bus genre taxonomy and
`IGameServices` leave the engine contract, event routing becomes one generic managed
router, the capability registry becomes a registration ledger publishing engine
capabilities only, and the dead `_allowedBuses` fossil retires. MANAGED-ONLY; the
sovereign native switch (`UseNativeBusForDispatch`) is NOT this wave -- it stays FENCED
per EVENT_BUS section 5 as its own future program. Anti-pattern rule: brief-vs-LOCKED-law
conflict means THE BRIEF IS WRONG -- H-LAW, quote both.

## 1. Ratified decisions (operator, chat, 2026-07-19)

- **F-54: RETIRE.** `_allowedBuses` (1 write / 0 reads) dies: field + ctor param +
  `ParallelSystemScheduler.BuildContext` argument + test-ctor updates. `[SystemAccess]`
  bus members (`Bus`, `Buses`) die WITH the taxonomy this same wave; `Reads`/`Writes`
  stay (live graph/validator consumers). The orphaned ISOLATION.md citations
  (`SystemAccessAttribute.cs:13-14` + the four docs recon R4.3 lists) re-point to
  MOD_OS_ARCHITECTURE. Per-system scoping, if ever wanted, is future design on the FQN
  gate -- record that sentence where the field dies.
- **BD-3.** (a) The five bus interfaces + `IGameServices` LEAVE `DualFrontier.Contracts`
  -- a BREAKING change executed per CONTRACTS.md section 4's own letter (MAJOR bump of
  the doc AND `ContractsVersion.Current`). They survive only as an ENGINE-INTERNAL
  harness bridge (outside Contracts; mechanical home is the executor's call within B-1)
  so the ~10 live harness systems keep `Services.<Bus>.Publish` until W5; the bridge
  carries its deletion trigger (dies at W5 with `SystemBase`) per boundary law section
  4. (b) Event routing collapses onto ONE generic managed router keyed by event type --
  the W1 SDK `Publish<T>/Subscribe<T>` + FQN capability gate IS the durable contract
  and its signatures do not change; `ModBusRouter`'s five-property reflection and the
  `[EventBus]` attribute die; the `ItemSpawnedEvent` off-taxonomy anomaly (recon A12)
  dies with them. (c) `nameof(IGameServices.X)` bindings die (37 production
  occurrences; the wave gate below). (d) The sovereign native switch is OUT OF SCOPE:
  no flip, no native edits; recon A4/A6/A8/A13 are seeded as the sovereign-switch
  precondition set (section 5, C7), not fixed here. Runtime capability tokens stay
  UN-TIERED (current canon); the tier machinery stays dormant-honest.
- **BD-10.** `KernelCapabilityRegistry`'s reflection scan of `DualFrontier.Components`
  / `DualFrontier.Events` DIES -- the 208 implicit genre-FQN tokens leave `kernel.*`.
  The registry becomes a REGISTRATION LEDGER: engine capabilities only under
  `kernel.*`; mod-registered types enter owner-namespaced; the ledger relocates out of
  the `DualFrontier.Application/Modding` composition layer into the engine layer
  (mechanical home the executor's call within B-1; the audit A9 misplacement dies).
  Harness-registered types ride the existing grace path (sacrificial; scaffolding
  ruling).
- **BD-10 consequence -- token grammar (transcribe as law).** Verb tokens name the
  OWNER: `kernel.{verb}:<FQN>` for engine-owned types; `mod.<ownerId>.{verb}:<FQN>` for
  mod-owned types (the `mod.<modId>.*` namespace already exists in the MOD_OS 3.3
  grammar). SELF-ACCESS IS AUTO-GRANTED: a mod never declares capabilities for its OWN
  registered types (requiring that would be absurd ceremony); declared capabilities
  gate CROSS-OWNER access. The v1 grace path (empty capability set -> warn + allow)
  is PRESERVED unchanged. If implementing this grammar cleanly requires more than the
  gate + ledger + manifest-validation surfaces, H-GRAMMAR: report, stop.

## 2. Established facts (W2_BUS_CAPABILITY_RECON_REPORT @ b2805ea; re-verify crux at Phase 0)

- F1. Two bus systems: managed five-bus (Type-keyed, authoritative, zero native hop) vs
  native 3-tier (FNV1a32(FQN) channels, auto-created, DORMANT,
  `UseNativeBusForDispatch=false` -- never flipped in src/). ~30 BD-3 coupling points,
  managed-dominated (recon R1.7 groups).
- F2. `IGameServices`: 5 members, 96 occurrences / 62 files (43 prod / 19 test); bus
  interface TYPES consumed only by the `GameServices` implementation; 37 production
  `nameof(IGameServices.X)` occurrences (32 in the 28 harness systems, 1 ExampleSystem,
  4 doc/error literals) + 17 test.
- F3. Capability census: 12-pattern `kernel.*` grammar; EXACTLY 208 implicit tokens (168
  event x4-verb + 40 component), 100% genre-FQN; ZERO on-disk manifest capability
  entries (4 inline in tests); harness publication = implicit reflection (unvalidated),
  mod publication = explicit + validated.
- F4. SDK gate chain (F-54-corrected): `ISystemContext.Publish` ->
  `SystemContextView.RequireApi` -> `RestrictedModApi.EnforceCapability`
  (`kernel.{verb}:{FQN}`, un-tiered) -> `ModBusRouter.Resolve` AFTER the gate. The
  grace path currently emits `Console.WriteLine` -- ONE of the src census-pin-of-2
  sites: if C4/C5 touch it, the census pin must be handled honestly same-commit (keep,
  or move with pin update + delta note; never silently).
- F5. `_allowedBuses`: exactly 2 sites (`SystemExecutionContext.cs:38, :83`); feeder
  `attr.Buses` at `ParallelSystemScheduler.cs:308`; test ctors pass bus arrays.
- F6. Gates baseline @ b2805ea: builds 0W/0E both configs; full-sln totals at the W1
  closure shape (re-measure and pin exact numbers at Phase 0); selftest 104 both;
  validate --armed exit 0; DFK-WAIVER 2=2; Console.WriteLine src=2; BoundaryRatchet
  4+1; K-L canon untouched by this wave.

## 3. Phase 0 (stop on any failure)

1. HEAD recorded (b2805ea or descendant); clean tree; F6 gates re-run, exact totals
   pinned.
2. MANDATORY LAW READS in full: CONTRACTS.md sections 2/4/6 (the breaking-change letter
   governs C4), EVENT_BUS.md sections 3-5 (the sovereign fence this brief must not
   cross), MOD_OS_ARCHITECTURE sections 3.2-3.4 + 4 + 9.4-9.5, boundary law B-1/B-4 +
   section 4, plan BD-3/BD-10/F-54 rows + W2 row (the gate text), ELT section 4, recon
   R1-R5.
3. Re-verify F1-F5 crux anchors by line-read at HEAD.
4. Enumerate every consumer that breaks when the five types leave Contracts (recon R3
   closure + tests) -- the C4 work list, measured before touched.

## 4. Commit plan (atomic; sync folded into frontmatter-touching commits)

- **C1 -- enroll.** This brief + the recon report committed; sync; validate.
- **C2 -- F-54 retire.** The R4.1 census executed exactly; ISOLATION re-points;
  the future-design sentence recorded; tests updated. Small and independent.
- **C3 -- the generic router.** One event-type-keyed managed router replaces
  `ModBusRouter`'s five-property resolution end-to-end; `[EventBus]` dies; SDK
  Publish/Subscribe signatures UNCHANGED (behavioral parity asserted: same events reach
  same subscribers, immediate + deferred, fault route D2 intact); A12's off-taxonomy
  publish site routes like every other event.
- **C4 -- taxonomy leaves Contracts.** The five bus interfaces + `IGameServices` move
  out of Contracts into the engine-internal harness bridge; `ContractsVersion.Current`
  MAJOR bump + CONTRACTS.md section-4-conformant change record; harness systems compile
  against the bridge unchanged; `[SystemAccess]` bus members die; the 37 production
  nameof bindings die; Contracts public surface delta recorded. THE WAVE GATE
  (verbatim from the plan): kernel capability surface contains zero
  Pawn/Combat/Magic/Inventory/World types; `[SystemAccess]` no longer binds
  `nameof(IGameServices.X)`.
- **C5 -- BD-10 ledger.** Scan of Components/Events dies; ledger relocated engine-side;
  engine capabilities only under `kernel.*`; owner-namespaced registration for
  mod-owned types + self-access auto-grant per the section-1 grammar; grace path
  preserved (F4 census handling); manifest validation follows the grammar.
- **C6 -- tests.** Router parity suite; ledger surface (zero genre types under
  kernel.*); token grammar (own-type auto-grant; cross-owner gated; engine-owned
  gated); retire regressions; gate-text assertions updated (SdkContextTests token
  forms).
- **C7 -- governance.** CONTRACTS.md MAJOR (taxonomy removal per its own section 4);
  EVENT_BUS.md amendment (section 2/4 current-truth: five-bus text -> the generic
  router; section 5 sovereign fence UNTOUCHED); MOD_OS sections 3.2-3.4 amendment
  (ledger law + owner grammar + self-access grant); plan: W2 DONE w/ hashes,
  BD-3/BD-10 RESOLVED w/ ratified text, F-54 CLOSED; ROADMAP: W2 DONE + ONE seeded row
  F-55 "sovereign-switch precondition set" citing recon A4 (collision-safe
  (providerId, schemaId) ids), A6 (tiered-token runtime mismatch), A8 (validator dead
  in production), A13 (by_mod P/Invoke gap + dual tier-truth) with anchors -- OPEN,
  rides the sovereign-switch program; sync; validate --armed exit 0.
- **C8 -- closure.** EVT append (prior entries byte-unchanged); brief -> EXECUTED;
  closure report enrolled; final gates + census deltas + Contracts surface delta.

## 5. Halt catalog

H-LAW / H-GRAMMAR (section-1 token grammar needs surfaces beyond gate+ledger+manifest
validation) / H-BRIDGE (the harness `Services.X` bridge cannot live engine-internal
cleanly) / H-SCOPE (ANY native edit, ANY `UseNativeBusForDispatch` change, ANY tier
machinery change beyond honest-dormant doc text -> stop) / H-GATE (validate nonzero;
build/test/selftest regression vs pinned Phase-0 totals; DFK-WAIVER moves;
Console.WriteLine census moved silently; BoundaryRatchet moves). Standing rails: never
push; no history rewrite; AUDIT_TRAIL append-only; derived via sync only; historical/
read-only.

## 6. Closure report schema

HEAD before/after; per-commit hashes; the wave-gate evidence (zero genre types under
kernel.*; zero nameof bindings) with grep proofs; router parity evidence; Contracts
surface delta + ContractsVersion bump record; token-grammar test evidence; F-54 retire
census; F-55 seed text; final gates; register/EVT deltas; attestation.

## 7. Out of scope (fenced)

The sovereign native switch + everything behind EVENT_BUS section 5's fence; native
code; tier machinery changes (A3/A6/A8 beyond honest doc text + the F-55 seed); W4/W5
work (harness systems stay SystemBase on the bridge); Fields/compute surfaces; PSC;
F-51/F-52/F-53.
