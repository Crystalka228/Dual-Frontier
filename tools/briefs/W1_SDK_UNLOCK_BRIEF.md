---
register_id: DOC-D-W1_SDK_UNLOCK_BRIEF
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
title: W1_SDK_UNLOCK execution brief (Wave 1 of the vanilla-separation program -- ISimulationSystem + ISystemContext + unified factory registration under ratified BD-1/BD-2, with the Contracts-only example-mod lifecycle proof as the wave gate)
---

# W1_SDK_UNLOCK -- Execution Brief

Wave 1 of VANILLA_SEPARATION_MIGRATION_PLAN (Live 1.1.0): the SDK surface unlock. Lands
the ratified BD-1 contract (`ISimulationSystem` + capability-scoped `ISystemContext`) and
the ratified BD-2 construction model (one factory-registration path for core and mods),
proves them with a Contracts-only example mod through the FULL lifecycle, and kills the
`Fixture -> Core` leak. The 10 live harness systems STAY on `SystemBase` (they die at W5
per the scaffolding ruling); this wave changes what a system CAN be, not what the harness
systems ARE. Anti-pattern rule: brief-vs-LOCKED-law conflict means THE BRIEF IS WRONG --
H-LAW, quote both.

## 1. Ratified decisions (operator, chat, 2026-07-19 -- transcribe faithfully)

- **BD-1.** `ISimulationSystem` in `DualFrontier.Contracts`: `Initialize(ISystemContext)`
  + `Tick(ISystemContext)` + a dispose hook -- dispose is kept DESPITE 0/28 harness usage
  because law mandates it (boundary-law DoD item 7: full mod-resource removal; usage
  cannot veto a law-mandated hook -- put that sentence in the interface doc). NO `float
  delta` anywhere on the contract (measured dead 0/28): temporal input arrives via a
  tick accessor on the context, consistent with TIME_AND_CONSISTENCY_MODEL.
  `ISystemContext` is the CAPABILITY-SCOPED PROMOTION of the existing engine-internal
  `SystemExecutionContext` seed (recon D9) -- promote, do not invent: (a) component
  access = Contracts-safe forms over exactly the measured union {span-read, batch-write,
  per-id Try/Has/Get, string intern+resolve, composite create}; (b) event access =
  Publish/Subscribe capability-gated by the system's declared access (the `allowedBuses`
  seed already exists); (c) NO Fields/compute surface and NO ManagedStore on the
  day-one context (0 consumers; Lesson N17 audience-driven deferral -- record the
  deliberate omission in the context doc). Engine-side `SystemAdapter` bridges
  `ISimulationSystem` onto the internal executor; the `SystemBase` path is a BRIDGE with
  its deletion trigger recorded (dies at W5 with the last harness system) per boundary
  law section 4.
- **BD-2.** ONE registration path for core and mods:
  `RegisterSystem<T>(Func<ISystemServices, T> factory)` plus a parameterless-convenience
  overload. `GameBootstrap` switches to the SAME entry (its 10 `SystemBase` systems
  register through factory delegates -- `MovementSystem`'s factory resolves
  `IPathfindingService` from `ISystemServices`); the core-vs-mod construction
  bifurcation DIES this wave even though the type migration waits for W5.
  `ISystemServices` day-one surface = exactly the measured consumption (pathfinding),
  extensible -- no speculative members. Systems are constructed ONCE at registration;
  the CONTEXT is per-tick: holding engine references across ticks is FORBIDDEN by the
  contract documentation (cite ECS.md section 8's existing anti-pattern text; the
  MovementSystem instance is the named example and dies at W5).

## 2. Established facts (W1_SDK_SURFACE_RECON_REPORT @ df1541d -- re-verify crux at Phase 0)

- F1. `SystemBase` = 7 members; live capability union as in BD-1(a); `delta`/
  `ManagedStore`/`OnDispose`/Fields all 0/28. `SystemExecutionContext` seed =
  {world, services, allowedBuses, origin, modId, faultSink, managedResolver}.
- F2. `NativeWorld`: systems consume 8 of 40 members; 4 leak `Core.Interop` types
  (`SpanLease`, `WriteBatch`, `InternedString`, `NativeComposite`). `Contracts` has
  ZERO project references and must stay reference-free (audit A4 cycle).
- F3. Registration today: `ModRegistry.RegisterSystem` gates on `SystemBase` +
  `Activator.CreateInstance` (`ModRegistry.cs:108/287` era anchors);
  `GameBootstrap.CreateSession` hand-instantiates 10 systems (`GameBootstrap.cs:141-153`),
  injecting `pathfinding` into `MovementSystem` (`:149`).
- F4. `Fixture.RegularMod_ReplacesCombat` references Core SOLELY to name `SystemBase`
  (recon R6) -- the leak dies when the fixture re-targets the SDK contract.
- F5. Buses: `nameof(IGameServices.X)` binding + the five-bus taxonomy are W2/BD-3
  territory -- W1 wires capability gating through the EXISTING router/validator
  machinery, changing none of the taxonomy.
- F6. Gates baseline @ df1541d: builds 0W/0E both configs; full-sln 1205/0/5; selftest
  104 both; validate --armed exit 0; DFK-WAIVER 2=2; Console.WriteLine src=2;
  BoundaryRatchet 4+1; register 361 / EVT 62. `[BridgeImplementation]` partitions
  18 stubs / 10 real.

## 3. Phase 0 (stop on any failure)

1. HEAD recorded (df1541d or descendant); clean tree; F6 gates re-run and pinned.
2. MANDATORY LAW READS in full: boundary law (B-3, section 4, DoD), plan sections 1.1/3
   (BD-1/BD-2 rows)/W1, ECS.md sections 7-8, MOD_OS_ARCHITECTURE section 4,
   CONTRACTS.md sections 2/4/6, EAM sections 3.0/3.3, TCM (tick/temporal law), CMM
   (allocation/hot-path law), the recon report R2-R7.
3. Re-verify F1-F4 crux anchors by line-read at HEAD.
4. C1 enrolls the recon report (already frontmattered; add + sync) alongside this brief.

## 4. Design constraints (binding on C2; the mechanical form is the executor's, the
semantics are ratified law)

- Contracts stays REFERENCE-FREE. The Contracts-safe component-access forms must not
  name any `Core.Interop` type. Acceptable shapes include Contracts-defined handle
  structs/ref structs whose operations are supplied engine-side at context construction
  (delegate/function-pointer table or interface implemented in Core); mods must be
  UNABLE to forge instances (internal-construction discipline).
- NO new per-tick allocation on the span-read/batch-write hot paths relative to the
  `SystemBase` path (CMM law). If a clean allocation-free Contracts-safe form is NOT
  reachable, H-DESIGN: present the measured obstacle + candidate shapes, stop.
- Lease/batch lifetime semantics (scoped acquire/dispose) are PRESERVED exactly --
  the safety model does not weaken through the promotion.
- Capability gating reuses the existing `allowedBuses`/router/validator enforcement --
  wire, do not duplicate (F5).

## 5. Commit plan (atomic; sync folded into frontmatter-touching commits)

- **C1 -- enroll.** This brief (Draft) + the recon report committed; sync; validate.
- **C2 -- the SDK contract.** `ISimulationSystem`, `ISystemContext`, `ISystemServices`
  + the Contracts-safe access forms, under section-4 constraints; XML docs carry the
  BD-1 law sentences (dispose-despite-usage; deliberate Fields/ManagedStore omission;
  no-held-references rule). Contracts public-surface growth recorded (count).
- **C3 -- engine adapter + unified registration.** `SystemAdapter` (internal, Core) maps
  contract systems onto the executor (fault route D2 preserved: mod-origin faults from
  `Tick` reach `RouteFault` exactly as `SystemBase` ones do); `ModRegistry` gains the
  factory path (BD-2) while the `SystemBase` path remains as the recorded bridge
  (deletion trigger: W5, cite in code doc); `GameBootstrap` re-registers its 10 systems
  through the unified factory entry (MovementSystem factory resolves pathfinding via
  `ISystemServices`); `Activator.CreateInstance` at the system gate dies or narrows to
  the convenience overload's implementation detail.
- **C4 -- the proof mod (the wave gate).** `Mod.Example` (already sln-enrolled) gains a
  REAL component + system + event on the new contract, Contracts-only: register ->
  ticks under the scheduler -> a deliberately faulted tick routes through D2
  (quarantine observed) -> dispose -> collectible ALC unload proof (mirror the existing
  unload-test pattern). A reference-assertion test pins Mod.Example's references ==
  {Contracts} (ratchet-style constant). F4: `Fixture.RegularMod_ReplacesCombat`
  re-targeted onto the SDK contract -- the Core leak DIES (record the census delta).
- **C5 -- tests.** Context capability tests (gated Publish/Subscribe: undeclared bus
  rejected LOUDLY); access-form semantics (lease scope, batch commit, Try/Has/Get,
  intern/resolve, composite) against a live world; factory registration both paths;
  per-tick context freshness; adapter fault-route parity; BD-6 criterion RECORDED
  (the bytes-vs-interprets boundary test text lands in the plan's BD-6 row as its
  operational form -- doc change, rides C6 if cleaner).
- **C6 -- governance.** CONTRACTS.md MINOR (new SDK members recorded in its canon;
  five-bus taxonomy untouched); ECS.md MINOR (sections 7-8: the contract story +
  SystemBase bridge status + no-held-references law); MOD_OS section 4.1 amendment
  (factory registration law); plan: W1 row -> DONE w/ hashes + BD-1/BD-2 rows ->
  RESOLVED w/ the ratified text + BD-6 operational criterion recorded; ROADMAP W1 DONE;
  sync; validate --armed exit 0.
- **C7 -- closure.** EVT append (prior entries byte-unchanged); brief -> EXECUTED;
  closure report enrolled; final gates + census deltas recorded.

## 6. Halt catalog

H-LAW / H-DESIGN (section-4 constraints unreachable cleanly) / H-SCOPE (any need to
modify the five-bus taxonomy, native code, or the 18 stubs -> stop, it is W2/W5/other
scope) / H-GATE (validate nonzero; build/test/selftest regression vs F6; DFK-WAIVER
moves; Console.WriteLine census moves; BoundaryRatchet census moves -- note: this wave
must NOT change engine->game edges; Mod.Example is mods/, outside the ratchet's engine
set). Standing rails: never push; no history rewrite; AUDIT_TRAIL append-only; derived
via sync only; historical/ read-only.

## 7. Closure report schema

HEAD before/after; per-commit hashes; the Contracts surface delta (new public types +
member counts); the example-mod lifecycle evidence (register/tick/fault/dispose/unload,
each with its test anchor); the F4 leak-death census; adapter fault-parity evidence;
final gates (F6 shape + deltas); register/EVT deltas; attestation.

## 8. Out of scope (fenced)

W2 bus/capability taxonomy (five buses + nameof bindings + kernel.* stay); W5 slice
moves (the 10 live systems + 18 stubs stay SystemBase); porting ANY harness system to
the new contract; Fields/compute/ManagedStore context surfaces; scenario/manifest work
(W4); native changes; F-51/F-52/F-53; PSC.
