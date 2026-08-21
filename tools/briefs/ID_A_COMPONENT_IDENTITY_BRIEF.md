---
register_id: DOC-D-ID_A_COMPONENT_IDENTITY_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: Draft
owner: Crystalka
version: '1.0'
first_authored: '2026-08-20'
last_modified: '2026-08-20'
content_language: en
next_review_due: null
title: 'ID-A component identity -- F-60 closure by owner-scoped registry re-key (identity family cascade A, MANAGED-ONLY): ComponentTypeRegistry keyed on (owner, type FullName) instead of the Type object, weak Type cache, eager mod registration at Apply, collectible-ALC loud guard, Weather gate flips, K-L4 amendment record'
authored_by: Claude Fable (architect session, identity-family charter)
basis: DOC-E-F59_F60_IDENTITY_RECON_REPORT (architect-seat recon at main ad4e353, 2026-08-20) + ratified lean set L1-L5 + the operator's OS-model refinement (same-name components are distinct types under distinct owners, like same-named programs are distinct processes under an OS)
---

# ID_A_COMPONENT_IDENTITY -- Execution Brief

This cascade is Cascade A of the ratified F-59+F-60 identity-family charter (one charter, two
cascades; Cascade B = ID-B entity versions, separately briefed). It CLOSES **F-60 (S1)**: a mod
that defines a component leaks its collectible ALC at unload (the full 10 s §9.5 step-7 spin +
`ModUnloadTimeout`) and cannot resume state across a reload (new `Type` -> new id -> empty span ->
re-seed). Root cause, measured: `ComponentTypeRegistry` keys ids on the **`Type` OBJECT** and never
deregisters.

The fix, ratified: **re-key component identity on an owner-scoped stable name** — the OS model:
the type NAME is not the identity; identity = owner ("directory") + name ("file"), so two mods
defining the same `FullName` are two distinct component types with distinct ids, exactly as two
same-named programs are distinct processes. The authoritative registry state holds **no `Type`
reference at all**; `Type`-keyed resolution becomes a weak cache that dies with the mod's ALC.
Identity rows survive unload, so a reloaded mod re-adopts its id and its surviving native store:
**weather RESUMES**. Zero native changes are required (recon R1.5: native `stores_` accepts
arbitrary u32 ids; sequentiality is managed-side law only) — this cascade is **MANAGED-ONLY**.

Executor: Claude Code (flagship model), LOCAL on the operator's machine, repository
`D:\Colony_Simulator\Colony_Simulator` (GitHub `Crystalka228/Dual-Frontier`), branch off `main`
(= `ad4e353`, the PR #49 merge).

Brief-integration notice: this brief CITES standing law by anchor and does not restate it --
commit-body structure and marker law per CODING_STANDARDS; **push law per CODING_STANDARDS §8.4
(v3.0.0): at the closure boundary the executor pushes the WORK BRANCH and opens a PR against
`main`; pushing `main` and merging its own PR are forbidden; commit atomicity is settled BEFORE
the push, corrections after it are new commits only (§8.3)**; census pin law per TESTING_STRATEGY;
mutability license and `Skeleton revisions` form per RESERVED_SURFACE_MUTABILITY; session closure
per METHODOLOGY; test invocation safety (no-pipe law) per TESTING_STRATEGY §8. Anti-pattern rule:
a conflict between this brief and any standing document, or between this brief and the live code,
means THE BRIEF IS WRONG -- halt and escalate; code-truth wins.

## 1. Mission [CORE]

Deliverables:

| #  | Artifact | Action | Version |
|----|----------|--------|---------|
| D1 | `ComponentTypeRegistry` re-key | Authoritative map keyed on `(owner, type FullName)`; sequential `_nextId` preserved; `Type`-keyed side becomes a `ConditionalWeakTable` cache; reverse map repointed `uint -> identity` (no `Type`); thread-safe mutation | -- |
| D2 | Eager mod registration | `ModIntegrationPipeline.Apply` allocates native ids for a mod's claimed component types right after `Initialize`, owner = `mod.<modId>`; pipeline reaches the registry via composition-root wiring (GameBootstrap + harness mirror) | -- |
| D3 | Collectible-ALC loud guard | `NativeWorld.ResolveTypeId` auto-register fallback restricted to NON-collectible ALCs (owner `kernel`); an unregistered collectible-ALC type is a loud throw naming the type and the `IModApi.RegisterComponent` remedy (fail-loud doctrine) | -- |
| D4 | Unload/reload semantics | Identity rows SURVIVE unload (re-adoption by design); same-identity re-registration returns the existing id; same-identity-different-size surfaces as a typed load failure through the existing rollback funnel, never a crash | -- |
| D5 | Gate flips + identity test suite | The two Weather EXPECTED-DEFECT assertions flip to their fixed forms (their doc comments carry the flip conditions); NEW tests: OS-model two-owners-same-FullName, size-mismatch refusal, reload-resume, concurrent-registration smoke | -- |
| D6 | Hygiene | `SpanScope.cs:62` stale `Version = 1` property doc corrected (census-aware; the K7-deferral note survives, repointed at F-59/ID-B) | -- |
| D7 | Doc amendments | KERNEL_ARCHITECTURE К-L4 amendment record + §3 registry description (MINOR); IDENTITY_AND_ABI_CONTRACT §1 row 2 code-truth + §2 stale К-L20-reserved pointer corrected to К-L22 (PATCH); MODDING.md conditional (only if it states the old lazy/leak behavior) | per §12 |
| D8 | Closure | EVT append; ROADMAP write-back (F-60 CLOSED, F-58 row correction, ID-B forward pointer); brief -> EXECUTED; **push branch + open PR** | -- |

Sequencing: ID-A precedes ID-B (F-59 versions view, native-touching) and unblocks W5 (mass
vanilla-mod migration is disqualified while reload leaks and resets). W4 may interleave after ID-A
at the operator's discretion.

## 2. Established facts [CORE]

All facts measured by architect line-reads at `main = ad4e353` (recon report
`docs/reports/F59_F60_IDENTITY_RECON_REPORT.md` R1-R5 — the extended basis; enrolled at C1).
Facts marked (RV) must be re-verified at Phase 0 by direct read.

1. (RV) `ComponentTypeRegistry.cs` (`src/DualFrontier.Core.Interop/Marshalling/`): two strong
   `Dictionary`s (`Type -> uint`, `uint -> Type`), `_nextId = 1`, `Register<T>` idempotent on the
   Type object, native call `df_world_register_component_type(handle, id, size)` with `_nextId`
   rollback on failure (:25-79). NO deregistration API. NOT thread-safe. The reverse map has ZERO
   consumers in `src/` outside the class itself (:26, :77, :111).
2. (RV) `NativeWorld.ResolveTypeId<T>` (:586-599): registry bound -> auto-register on first use;
   registry null -> legacy FNV-1a(`AssemblyQualifiedName`) via `[Obsolete]` `NativeComponentType<T>`.
   The auto-register site has NO owner/mod context (Core.Interop knows no modId).
3. (RV) The К-L4 explicit path registers OWNERSHIP ONLY: `RestrictedModApi.RegisterComponent<T>`
   -> `ModRegistry.RegisterComponent(modId, typeof(T))` -> `_componentOwners[type] = modId` with a
   cross-mod-claim throw (:167-181); cleaned symmetrically at `RemoveMod` (:294-301). No native id
   is allocated on this path — the id is allocated lazily by fact 2 during ticks.
4. (RV) `ModIntegrationPipeline.Apply`: `mod.Instance.Initialize(api)` at :435 (regular pass);
   ledger `RegisterOwner` producers at :354 (pass [1] shared) and :1065. The pipeline ctor takes
   `(ModLoader, ModRegistry, ContractValidator, ModContractStore, IGameServices,
   ParallelSystemScheduler, ModFaultHandler)` — no world/registry reference today (harness
   exemplar `WeatherHarness.cs:81-83`).
5. Native law (NOT touched by this cascade; cited as constraint): `World::register_component_type`
   accepts ANY u32 >= 1; idempotent on same (id, size); **throws on same id + different size**
   (caught at `capi.cpp:253-263` -> returns 0 -> managed `InvalidOperationException`); `stores_` is
   an `unordered_map`, no removal export (`world.cpp:264-291`, `world.h:74`). Sequential ids are a
   managed-side property, not an ABI requirement.
6. (RV) Vanilla set: `VanillaComponentRegistration.RegisterAll` — 21 engine types, ids 1-21,
   order-pinned, registered on the main thread at bootstrap (`GameBootstrap.cs:86-87`).
7. (RV) The unload spin: `TryStep7AlcVerification`, 100 x 100 ms, `ModUnloadTimeout` warning
   (`ModIntegrationPipeline.cs:954-977`). Measured cost 10,668 ms per component-defining-mod
   unload; the Weather gate suite carries 5 `UnloadMod` sites (~53 s of pure spin today).
8. (RV) The two F-60 pins with in-test flip conditions (`WeatherWaveGateTests.cs:156-221`):
   `Reload_DoesNotYetAdoptTheSurvivingSingleton_BecauseComponentIdentityIsPerAlc` (flip
   `EntityCount` 2 -> 1) and `Unload_LeaksTheModAlc_BecauseTheTypeRegistryStillHoldsItsComponentType`
   (flip Contains(ModUnloadTimeout) -> BeEmpty). Harness is production-faithful
   (`Bootstrap.Run(useRegistry: true)`, `WeatherHarness.cs:50-84`).
9. Dispatch runs system bodies through `Parallel.ForEach` (THREADING.md:74), and first-use
   registration is reachable from system bodies via `SystemContextView.AcquireSpan` -> world
   ambient context (`SystemContextView.cs:41-45,92-95`) — the thread-safety motivation for D1.
10. `SpanScope.cs:58-67` property doc still teaches `Version = 1`; the enumerator below fabricates
    0 with the corrected W3 comment (:103-109). The stale comment contains the token `K7-deferred`
    — see §10 census handling.
11. Ledger owner-string convention: `kernel` vs `mod.<modId>` (KernelCapabilityRegistry / MOD_OS
    §3.3) — D2 reuses it verbatim for component identity owners.
12. ROADMAP F-58 row contains a sentence FALSIFIED by the corrected F-60 harness: "reload ADOPTS
    the survivor, which is why weather resumes rather than resets" — written before the harness
    fidelity fix; corrected at D8, not silently.
13. Gates at `ad4e353` (executor re-measures at Phase 0 as the regression anchor): full-sln
    expected 1261/0/5 (5 skips = F-10 family), Modding.Tests 457/0.

## 3. Phase 0 -- preconditions and checkpoint [CORE]

Run serially before any code change.

1. **Verify the (RV) set** from §2 by direct reads. Mismatch -> HALT H1.
2. **Baseline gates** (regression anchor): full managed build + full-sln test run per
   DEVELOPMENT_HYGIENE and TESTING_STRATEGY §8 (no stdout pipes; TRX is truth). Expect
   1261/0/5; record actuals. Closure must match-or-improve modulo tests this cascade adds/flips
   -> HALT H2 on regression.
3. **Branch prep**: create work branch from `main` (`claude/id-a-component-identity` or
   session-assigned). Push law per CODING_STANDARDS §8.4 v3.0.0: the branch is PUSHED and a PR
   opened at the closure boundary — never `main`, never self-merge; atomicity settled before the
   push.
4. **Validation checkpoint**: `dotnet run --project tools/DualFrontier.Governance -- validate
   --armed` exit 0 -> else HALT H3.
5. **Frontmatter-shape read** (Lesson #N14): FRAMEWORK.md 14.3/14.4/14.5 + live frontmatter of one
   LOCKED tier-2 doc + one existing `AUDIT_TRAIL.yaml` EVT as the verbatim append template. Armed
   G-CATLIFE: this brief persists `lifecycle: Draft`, flips to `EXECUTED` at closure — never
   LOCKED in frontmatter.
6. **MANAGED-ONLY fence**: `git status --porcelain native/` clean at Phase 0 and at EVERY commit;
   any `native/` diff -> HALT H7.
7. **Census pre-measure**: run the five TESTING_STRATEGY §5.2 marker censuses BEFORE any
   comment-touching commit (canonical `--count-matches` invocations, never `-c`); record the
   baseline table. Bi-script К-L reality (F-4): match Cyrillic and Latin in any grep.
8. **Mandatory reads** before any edit: this brief in full; the recon report
   (`docs/reports/F59_F60_IDENTITY_RECON_REPORT.md`); `ComponentTypeRegistry.cs`,
   `NativeComponentType.cs`, `NativeWorld.cs` (ResolveTypeId + component/span/batch funnels +
   `Registry` property + `AdoptBootstrappedHandle`), `Bootstrap.cs`, `GameBootstrap.cs`
   (world construction + pipeline construction site), `VanillaComponentRegistration.cs`,
   `ModRegistry.cs` (RegisterComponent/RemoveMod/`_componentOwners` access surface),
   `RestrictedModApi.cs` (RegisterComponent + RegisterManagedComponent), `ModIntegrationPipeline.cs`
   (Apply passes, Initialize site, rollback funnel, UnloadMod chain, step-7 spin),
   `SystemContextView.cs`, `WeatherHarness.cs` + `WeatherWaveGateTests.cs`,
   `SpanScope.cs`; `grep -rn "ComponentTypeRegistry" src/ tests/` and
   `grep -rn "new ModIntegrationPipeline" src/ tests/` (full constructor-site census —
   D2 wiring must cover every production site and the harness exemplars);
   KERNEL_ARCHITECTURE Part 0 К-L4 + §3; IDENTITY_AND_ABI_CONTRACT §1 rows 1-2 + §2;
   CODING_STANDARDS §8.3/§8.4 + commit-body law; METHODOLOGY closure protocol.
9. **Derived-fold protocol**: `sync` runs in EVERY frontmatter-touching commit; derived registers
   never hand-edited; `AUDIT_TRAIL.yaml` append-only.

## 4. Topology [CORE]

Single orchestrator, serial execution, no wave agents. Rationale: deep-but-narrow — every commit
builds on the previous surface (registry core -> pipeline wiring -> tests -> hygiene -> docs).
Only the orchestrator runs `git add/commit`.

## 5. Wave R -- survey agents

None (serial cascade; Phase 0 mandatory reads are the survey).

## 6. Checkpoints [CORE]

Self-audit before C6 (docs commit): MANAGED-ONLY fence re-check (`git status native/` clean across
all commits so far); census re-measure vs the Phase 0 baseline (§10 discipline); truth-law audit
of every enforcement claim about the new guard/tests (each names its on-disk artifact); citation
form (anchors, no living-doc version pins, no URL anchors).

## 7. Execution / writer specifications [CORE]

### 7.1 D1 -- registry re-key (commit C2)

Intended form (symbol names and shapes are mutable surface per RESERVED_SURFACE_MUTABILITY;
deviations recorded as `Skeleton revisions`):

- `readonly record struct ComponentIdentity(string Owner, string TypeFullName)` — the
  authoritative key. Owner strings follow the ledger convention verbatim: `kernel` for engine
  surface, `mod.<modId>` for mods (§2 fact 11).
- Authoritative state: `Dictionary<ComponentIdentity, uint> _idByIdentity` +
  `Dictionary<uint, ComponentIdentity> _identityById` (the reverse map REPOINTED from `Type` to
  identity — diagnostic value preserved, zero `Type` references; `Lookup(uint)` returns
  `ComponentIdentity?` — the old `Type?` form has zero production consumers, §2 fact 1; adjust
  any test callers found by the Phase 0 grep).
- Resolution cache: `ConditionalWeakTable<Type, ...>` mapping a live `Type` object to its boxed
  id. CWT holds keys weakly — a mod ALC's `Type` entry dies with the ALC. **This is the leak
  fix**: after C2, no registry structure roots a collectible ALC.
- `_nextId` sequential allocation preserved (К-L4 determinism: vanilla ids 1-21 byte-identical
  behavior; mod ids follow deterministic load order exactly as today's doc requires).
- Surface: keep `Register<T>()` as the `kernel`-owner convenience (VanillaComponentRegistration
  call sites unchanged); add the owner-scoped form (generic and/or `Register(Type, string owner)`
  runtime form for D2 — if the runtime form is used, obtain the size via
  `MakeGenericMethod`-invocation of the generic path so `Unsafe.SizeOf<T>` semantics are
  byte-identical; do NOT introduce a second size computation).
- Re-registration semantics: same identity -> return the existing id (the native re-register is
  idempotent for same size and is what re-attaches a reloaded mod to its surviving store). The
  native different-size throw path surfaces per 7.3.
- Thread safety: one private lock around authoritative-state mutation; the hot path
  (`ResolveTypeId` cache hit) stays lock-free through the CWT. Registration is cold (bootstrap +
  Apply), so a plain lock is the whole answer — no lock-free cleverness (bez kostylei).
- `GetId<T>`/`TryGetId<T>`/`IsRegistered<T>`: resolve through the cache; a cache miss on a
  non-collectible type may fall through to `(kernel, FullName)` identity lookup; a cache miss on a
  collectible type follows the 7.2 guard (throw / false respectively).
- **C2 also flips the two Weather EXPECTED-DEFECT assertions** (§2 fact 8) to their fixed forms —
  the flip travels in the same commit as the change that makes them true (rename the two tests to
  their fixed-behavior names; record old->new in `Skeleton revisions`). Interim C2 state note: at
  C2 the auto-register path keys mod types as `(kernel, FullName)` — identity is stable across
  reload from C2 on (adoption + leak death both provable at C2); ownership honesty lands at C3.

### 7.2 D3 -- collectible-ALC guard (commit C3, same commit as D2)

In `ResolveTypeId<T>` (and the equivalent non-generic funnel if one is added): on CWT miss —

- `AssemblyLoadContext.GetLoadContext(typeof(T).Assembly)?.IsCollectible != true` -> auto-register
  under `(kernel, FullName)` (engine/test ergonomics preserved; documented in the К-L4 amendment
  record as the sanctioned implicit path for non-collectible surface).
- Collectible -> **throw** `InvalidOperationException` naming the type, the owning-ALC name, and
  the remedy ("register via IModApi.RegisterComponent during Initialize"). Fail-loud doctrine:
  the alternative (silent kernel-owner adoption of a mod type) would merge identities across
  owners and reopen the isolation hole the OS model closes.
- The guard lands in the SAME commit as eager registration (D2): with eager registration in
  place, every legitimate mod component is registered before its first tick, so the guard breaks
  nothing that was correct. Guard-before-eager would break the Weather suite mid-cascade;
  eager-before-guard would leave the hole open — one commit, both halves.

### 7.3 D2 + D4 -- eager registration at Apply, size-mismatch refusal (commit C3)

- Wiring: `ModIntegrationPipeline` gains a `ComponentTypeRegistry?` constructor parameter.
  Production passes `nativeWorld.Registry` (GameBootstrap; locate the pipeline construction site
  at Phase 0). `WeatherHarness` mirrors (gate-harness-fidelity law). Null is tolerated at
  construction (legacy `new NativeWorld()` test rigs have no registry) but is NOT a silent skip:
  if a mod claims a component type and the registry is null -> loud `InvalidOperationException`
  at Apply (fail-loud; a legacy rig loading component-defining mods is exactly the fidelity gap
  that hid F-60).
- Hook point: immediately after `mod.Instance.Initialize(api)` succeeds for a REGULAR mod (and
  the shared-pass equivalent if shared mods can claim components — verify at Phase 0 whether the
  shared pass calls Initialize; if shared mods cannot register components today, record the fact
  in the closure report and guard only the regular pass): for each `Type` newly claimed by this
  mod in `ModRegistry._componentOwners` (expose a narrow read: `ComponentTypesOf(modId)` or the
  set-difference captured around Initialize), call the owner-scoped registration with
  `owner = "mod." + modId`.
- Failure semantics (D4): the native different-size throw (same identity, changed layout across
  mod versions within one session) is caught AT THE EAGER-REGISTRATION SITE and converted into
  the pipeline's existing validation-failure/rollback funnel (typed `ValidationError`, mod load
  fails cleanly, engine healthy, ledger/ModRegistry rollback symmetry preserved). Never a crash,
  never a silent skip.
- Unload: **no registry mutation** — identity rows and native stores survive by design
  (re-adoption is the resume mechanism; residue lifecycle remains F-58, out of scope).

### 7.4 D5 -- test suite (commits C2 gate flips + C4 new tests)

Per-behaviour obligations (each row = at least one test, real pipeline where the behavior is
pipeline-level):

| Behavior | Test (intended name) | Level |
|---|---|---|
| Reload adopts the survivor; weather resumes; EntityCount stays 1 | flip of §2 fact 8 test #1 -> `Reload_AdoptsTheSurvivingSingleton_ComponentIdentitySurvivesAlcReload` | pipeline (Weather harness) |
| Unload releases the ALC inside the step-7 window; NO ModUnloadTimeout | flip of §2 fact 8 test #2 -> `Unload_ReleasesTheModAlc_RegistryHoldsNoTypeReference` | pipeline |
| OS model: same FullName under two owners = two ids | `SameFullName_TwoOwners_DistinctIds` | registry unit |
| Same identity re-registration returns the same id | `SameIdentity_Reregistration_ReturnsExistingId` | registry unit |
| Same identity + different size = typed load failure + clean rollback | `SizeMismatch_OnReload_FailsLoadCleanly` (unit against a real world handle; pipeline-level only if a fixture mod pair with a changed layout is cheap) | unit |
| Collectible unregistered type = loud throw with remedy text | `UnregisteredCollectibleType_Throws_NamingTheRemedy` | unit |
| Concurrent registration storm is safe | `ConcurrentRegistration_IsThreadSafe` (Parallel.For over owner-scoped registrations, distinct + same identities) | unit |
| Legacy `new NativeWorld()` path behavior unchanged | assert one existing FNV-path test still green (no new test; name it in the closure report) | existing |

The suite gains, net, the ~53 s the Weather unloads stop spinning (§2 fact 7) — record the
before/after Weather-suite wall time in the closure report as the measured F-60 dividend.

### 7.5 D6 -- hygiene (commit C5)

`SpanScope.cs:58-67`: correct the `Pairs` property doc to the Version = 0 truth (match the
enumerator's W3 comment at :103-109). The deferral it expresses is STILL TRUE until ID-B — keep
the deferred-ness, repointed: "true per-entity versions arrive with the IAC §2 versions view
(F-59/ID-B)". §10 census discipline applies (the comment carries `K7-deferred`).

## 8. Kind-specific machinery [KIND: phase-execution]

Per-behaviour test obligation carried by 7.4. No multi-agent wave topology (§4). No reserved-stub
surface is created or consumed; no `[ReservedStub]` sites are touched.

## 9. S-LOCK invariants [CORE]

None added. The parked ownership-symmetry S-LOCK candidate (ROADMAP) is adjacent but untouched —
this cascade's unload path deliberately does NOT remove registry rows, so no removal symmetry
exists to lock. The К-L4 amendment (D7) is an amendment RECORD on an existing LOCKED invariant,
not a new S-LOCK.

## 10. Census discipline [CORE]

- HARD pins, exact, unchanged: `Console.WriteLine` src = 2 (both `RestrictedModApi.cs`),
  DFK-WAIVER = 2, ratchet 4+1. Any movement -> HALT H10.
- SOFT pins: TODO 132/51 and deferred 89/55 baselines. The D6 comment edit touches a line
  containing `K7-deferred`; the intended rewording PRESERVES a deferred-vocabulary token (the
  deferral is still true, §7.5), so the expected delta is ZERO — but the census is measured, not
  assumed: pre-measure (Phase 0.7), re-measure in the C5 commit, and if the pin moves, record the
  same-commit census-delta per TESTING_STRATEGY (a SOFT pin moved by a comment edit is a recorded
  delta, not a finding). Method: the canonical §5.2 `--count-matches` invocations verbatim.
- No `.cs` file count changes; no marker-registry vocabulary added.

## 11. Commit plan [CORE]

| #  | Subject | Content |
|----|---------|---------|
| C1 | `governance(id-a): enroll ID_A_COMPONENT_IDENTITY brief + F59/F60 identity recon report` | this brief (D/3/Draft) + `docs/reports/F59_F60_IDENTITY_RECON_REPORT.md` (E/3/EXECUTED) frontmatter-enrolled + sync + validate --armed |
| C2 | `feat(interop): ComponentTypeRegistry keyed on owner-scoped identity, not the Type object -- the F-60 root` | 7.1 re-key + CWT cache + thread-safety + reverse-map repoint + registry unit tests + the two Weather gate flips (renamed to fixed-behavior names) |
| C3 | `feat(modding): eager component registration at Apply under mod ownership; collectible types must register or fail loud` | 7.2 guard + 7.3 pipeline wiring/hook/size-mismatch funnel + GameBootstrap + WeatherHarness mirror + pipeline-ctor census sweep |
| C4 | `test(modding): identity gate suite -- OS-model ownership, re-adoption, size refusal, concurrency` | 7.4 new tests (rows 3-8) |
| C5 | `chore(hygiene): SpanScope.Pairs doc tells the Version=0 truth; deferral repointed at ID-B` | 7.5 + census re-measure (same-commit delta record if a SOFT pin moves) |
| C6 | `docs(identity): K-L4 amendment record -- identity is owner-scoped name, not the Type object; IAC pointers to code-truth` | §12 doc set, frontmatter bumps + sync + validate --armed |
| C7 | `governance(closure): ID_A EVT + ROADMAP write-back -- F-60 CLOSED` | AUDIT_TRAIL append + F-rows per §14 + brief -> EXECUTED + sync + validate --armed |

Commit count is intended-form; a defect-iteration split is recorded in the closure report, never
compressed. After C7: **push the branch, open the PR** (§8.4 v3.0.0); post-push corrections are
new commits only.

## 12. REGISTER cascade [CORE]

Schema-2.0 discipline; Phase 0 verbatim shapes only; `PENDING-*` outlawed; real hashes or omit.

- C1: this brief enrolled (D/3/Draft) + the recon report enrolled (E/3/EXECUTED,
  `special_case_rationale` per the DOC-E recon precedent chain, already in its frontmatter).
- C6 amendments (each = frontmatter edit + body change + sync + validate in ONE commit):
  - `KERNEL_ARCHITECTURE.md` MINOR: К-L4 amendment record — canonical text UNCHANGED (all four
    pinned properties preserved: explicit registration, no hash-derived ids, deterministic
    assignment, cross-mod isolation); the record states the identity key is the owner-scoped
    stable name (OS model), the `Type` object is a resolution cache only, the sanctioned implicit
    path is non-collectible surface under `kernel`, and eager per-mod allocation at Apply is the
    mechanism that makes "explicit per-mod registration" literally true (recon A3 resolved). §3
    registry description updated to the new shape.
  - `IDENTITY_AND_ABI_CONTRACT.md` PATCH: §1 row 2 (component TypeId) updated to code-truth
    (keyed on owner-scoped identity; survives unload; never crosses the save boundary — Note 2
    unchanged); §2:125 stale "К-L20 is already reserved" corrected to К-L22 (К-L20 seated
    shutdown-quiescence at EQ_A2, К-L21 reserved Mod-API — cite KERNEL Part 0).
  - `MODDING.md` CONDITIONAL MINOR: only if Phase 0 grep finds it stating the lazy-id or
    restart-to-reclaim behavior; otherwise record "no change needed" in the closure report.
- C7 closure: single `AUDIT_TRAIL.yaml` EVT append (real hashes C1..C6); brief Draft -> EXECUTED;
  ROADMAP write-back per §14. `validate --armed` exit 0 at C1, C6, C7.

Version bumps computed against the LIVE frontmatter versions read at Phase 0 — never assumed.

## 13. Halt conditions (H-series) [CORE]

- **H1** precondition/(RV) mismatch (§3.1).
- **H2** build/test regression vs the Phase 0 baseline (modulo added/flipped tests named in §7.4).
- **H3** `validate --armed` nonzero.
- **H4** a Phase 0 mandatory read materially contradicts §2 — stop, report the delta.
- **H5** a REGISTER field/enum/sentinel needed beyond FRAMEWORK 14.3/14.4 closed vocabularies —
  escalate, never invent.
- **H6** truth-law unsatisfiable in a D7 doc without an architectural decision.
- **H7** ANY `native/` tree diff at any commit (MANAGED-ONLY fence). The fix needs zero native
  changes (§2 fact 5); needing one means the design premise broke — report, do not improvise.
- **H8** the leak does NOT die at C2 (the flipped `Unload_ReleasesTheModAlc...` test still sees
  `ModUnloadTimeout`): a SECOND ALC root exists beyond the registry. Report with evidence (the
  test output + a heap-dump pointer if cheap); do NOT extend the timeout, do NOT chase further
  roots inside this cascade without a ratified scope extension.
- **H9** eager registration cannot enumerate a mod's claimed types (ModRegistry ownership surface
  inadequate for the 7.3 hook) — a contract gap, not a workaround site; report the option set.
- **H10** a HARD census pin moves (§10).
- Standing rails: push law §8.4 v3.0.0 (push the WORK BRANCH + open a PR at closure; never
  `main`, never self-merge; atomicity settled before the push; no force-push / history rewrite /
  squash ever — §8.3); derived registers never hand-edited; `AUDIT_TRAIL.yaml` append-only;
  `historical/` and reference trees read-only; no `-Sync` invocations.

On halt: stop, report state verbatim, await the operator.

## 14. Closure protocol and report [CORE]

Execute the METHODOLOGY session closure protocol. ROADMAP write-back (C7):

- **F-60 -> CLOSED**: cite the two flipped gate tests by their NEW names + the C2/C3 hashes; the
  mechanism sentence (owner-scoped identity re-key; rows survive unload; CWT cache dies with the
  ALC); the measured Weather-suite wall-time dividend (before/after, §7.4).
- **F-58 row correction**: strike the falsified "reload ADOPTS the survivor" sentence (§2 fact
  12); replace with post-ID-A truth: adoption is now REAL via identity re-key (cite the flipped
  reload test); the row's core — a mod cannot RECLAIM its state at unload, residue persists —
  stands unchanged, still W7+.
- **F-59 row note**: unchanged and OPEN; forward pointer "ID-B (versions view per IAC §2) is the
  chartered successor cascade".
- Forward state: identity family — ID-A DONE, ID-B next (native-touching, IAC §2 option 1 +
  IsValid alignment + DFK-entity-identity/К-L22 seating question to the architect).
- The A9 note (recon): the legacy-FNV test population is unchanged by design; carried in the
  closure report, not the ledger.

Closure report (chat): commits table (hash | subject); versions table (each doc before -> after;
register derived state); gates table (baseline vs closure — match-or-better; flipped/added tests
named; Weather-suite wall time before/after); census table (§10 HARD exact, SOFT deltas with the
same-commit records if any); F-ledger final state; consolidated `Skeleton revisions` list (every
deviation from intended forms, incl. the two test renames); self-attestation (MANAGED-ONLY fence
held — zero native/ diffs; sync in every frontmatter-touching commit; single EVT append, prior
entries byte-unchanged; no history rewrites; **branch pushed + PR opened, `main` untouched, PR not
self-merged**); operator manual checklist (review the PR per-commit -> merge = ratification ->
optional live smoke: Launcher, load Weather pair, observe tint, unload WITHOUT the 10 s hang,
reload, observe weather RESUME rather than reset).

## 15. Out of scope [CORE]

- **ID-B / F-59**: `df_world_acquire_versions` + `df_world_release_versions`, SpanLease/Pairs true
  versions, `EntityId.IsValid`/`entity_id.h` alignment, DFK-entity-identity rule, К-L22 seating —
  the sibling cascade, separately briefed.
- **F-58** reclamation (residue lifecycle, `ClearModScope` wiring — recon proved the mod-scope
  trio is string-pool-only; the reclamation design starts from zero native tracking).
- **F-57** sovereign-switch family: event type ids (`BusFacade.Fnv1a32` untouched), native
  channel identity, tier enforcement.
- Native store removal / registry-row removal at unload (deliberate design absence, §7.3).
- The legacy FNV fallback path and its `[Obsolete]` classes (A9 population; K8-cutover question).
- `RestrictedModApi` stale capability comments (F-20/F-21 orbit — next hygiene pass).
- Vanilla-mod `DeployToLauncherMods` packaging gap; W4 composition root; W5 slice moves.
