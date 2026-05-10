---
title: M7 closure verification report
nav_order: 99
---

# M7 — Hot reload from menu closure verification report

**Date:** 2026-05-03
**Branch:** `main` (commits `b504813..110ad61`, 50 non-merge commits inclusive across 7 M-sub-phases, 6 housekeeping passes, 1 mid-batch v1.5 ratification + audit campaign chain, and 4 sundry doc-only sync commits; branch is 50 non-merge commits ahead of `origin/main` and not yet pushed)
**Scope:** Verification only. No new architectural work. Surgical fixes
applied for stale `v1.4 LOCKED` cross-doc references (post-v1.5
ratification) and three deliberate-interpretation footers anchored on a
hypothetical-future `v1.5 ratification` clause that was overtaken by the
mid-batch audit Pass 2 ratification. Any structural finding is recorded
in §10 as a follow-up item rather than remediated in this session.

---

## §0 Executive summary

| # | Check | Status | Notes |
|---|---|---|---|
| 1 | Build & test integrity | **PASSED** | `dotnet build`: 0 warnings, 0 errors. `dotnet test` at HEAD: 437/437 (Persistence 4, Systems 19, Modding 346, Core 61, ManifestRewriter 7). Three-commit invariant verified per sub-phase / housekeeping pass. M-cycle test progression: 338 (M6 baseline) → 349 (M7.1) → 362 (M7.2) → 369 (M7.3) → 378 (M7.4) → 408 (M7.5.A) → 415 (M7.5.B.1) → 434 (M7.5.B.2) → 437 (post-housekeeping). |
| 2 | Spec ↔ code ↔ test triple consistency | **PASSED** | All §11.1 M7 acceptance bullets reachable inside the M7 batch have all three legs present. §7.5 fifth scenario (mod-unloaded → replacement-revert → kernel-bridge-reregister) outcome **(B) Structural verification** — no joint end-to-end test was added; the M7.2 unload chain (`UnloadMod` removes from `_activeMods`) plus M6.2's parameter-driven `CollectReplacedFqns` together close the structural argument from M6_CLOSURE_REVIEW.md §8 verbatim. Disjoint tests cover both halves. |
| 3 | Cross-document consistency | **PASSED (post-surgical-fix)** | `MOD_OS_ARCHITECTURE` v1.5 LOCKED at HEAD (v1.4 → v1.5 happened mid-M7 via commits `b504813` for v1.4, `3f00c2a` for v1.5). Diff between M6 closure baseline and HEAD shows 21 lines changed; all changes match the documented v1.4 + v1.5 changelog entries. `ROADMAP` header `Updated: 2026-05-03` with M7 row ✅ Closed and `437/437`. `docs/README` v1.5 LOCKED. Three `v1.4 LOCKED` and three `v1.5 ratification` stale references corrected via §9 surgical fixes. |
| 4 | Stale-reference sweep | **PASSED (post-surgical-fix)** | All forbidden patterns return zero hits in active-navigation context. Intermediate test counts (`311`/`328`/`333`/`338`/`415`/`416`/`417`/`428`/`431`/`434`) survive only as historical progression text in ROADMAP engine snapshot or in M3–M6 closure reviews (frozen audit trail). `M7 in progress` / `🔨 In progress` (M7 row) flipped to `✅ Closed`. Remaining `v1.x LOCKED` references in audit-campaign artifacts are frozen at the time of writing (Pass 1–5 audit trail) and explicitly out of active-navigation scope. |
| 5 | Methodology compliance | **PASSED** | Commit scope prefixes verified per METHODOLOGY §7.3 — every M-cycle and housekeeping commit carries a substantive body. v1.4 + v1.5 spec ratifications happened in dedicated `docs(architecture)` commits (`b504813`, `3f00c2a`), neither silent. **§12 D-1 through D-7 byte-identical** between M6 closure baseline (`c7210ca`) and HEAD — the strongest possible falsifiable signal that the M7 batch did not silently re-open any LOCKED strategic decision (verified via `diff` of the §12 sub-tree returning zero output). Six deliberate-interpretation registrations in ROADMAP per "no improvisation" rule. |
| 6 | Sub-phase acceptance criteria coverage | **PASSED** | Every acceptance bullet for M7.1 through M7.5.B.2 maps to an identifiable artifact (commit, file:line, test name). 6 housekeeping passes documented separately as auxiliary closures, each with a fix/test/docs three-commit footprint. |
| 7 | Carried debts forward | **PASSED** | **Phase 2 WeakReference unload tests CLOSED** by M7.3 (the M5 / M6 closure carried this forward as "hard requirement at M7" — now satisfied via `M73Phase2DebtTests`). Phase 3 `SocialSystem`/`SkillSystem` stubs unchanged → M10.C. M3.4 (CI Roslyn analyzer) unchanged → first external mod author. §7.5 fifth scenario M6→M7 hand-off resolved via outcome (B). Three new in-batch carried items: §9.2 v1.6 ratification candidate, `.sln` build-verification gap, UI redesign with Kenney+Cinzel. |
| 8 | Ready-for-M8 readiness | **PASSED** | M7.4 build pipeline override + `mods/Directory.Build.targets` + `ManifestRewriter` tool ready for vanilla skeletons. M0–M2 + M4 + M7 mod loader chain in place. M7.5.A controller + M7.5.B.1 production wiring + M7.5.B.2 Godot UI + post-fix menu-pauses-simulation behavior all present. ContractValidator eight-phase pipeline accepts M8 vanilla mod batches without new phase. No M7 surface change blocks M8. |

**Result:** All 8 checks PASSED. Six surgical fixes applied across `docs/README.md`, `docs/ROADMAP.md` to sync stale `v1.4 LOCKED` and stale `v1.5 ratification` references after mid-batch v1.5 was consumed by audit campaign Pass 2 (see §9). M7 phase closes; M8 (Vanilla skeletons) is unblocked.

Three non-blocking observations registered in §10:
- **Hypothesis fifth datapoint = 1** (the §9.2 wording-vs-implementation gap surfaced via F5 manual verification post-M7.5.B.2 closure). M6 forward claim («M7 ≤ 1 latent contradiction») holds. M5 cumulative falsification target («cumulative across M3–M10 ≤ 4») still holds at 3.
- **§9.2 v1.6 ratification candidate registered** with full diagnosis and three resolution options. Resolution deferred to a future v1.6 cycle informed by M8+ load patterns.
- Two non-blocking items: top-level `dotnet build` `.sln` gap (Presentation project not in `.sln`); UI redesign brief (Kenney + Cinzel) decisions pending.

---

## §1 Build & test integrity

**`dotnet test DualFrontier.sln`** at HEAD (`110ad61`):

| Project | Pass | Skip | Total |
|---|---|---|---|
| `DualFrontier.Persistence.Tests` | 4 | 0 | 4 |
| `DualFrontier.Systems.Tests` | 19 | 0 | 19 |
| `DualFrontier.Modding.Tests` | 346 | 0 | 346 |
| `DualFrontier.Core.Tests` | 61 | 0 | 61 |
| `DualFrontier.Mod.ManifestRewriter.Tests` | 7 | 0 | 7 |
| **Total** | **437** | **0** | **437** |

The solution build succeeds; `dotnet test` exit code is zero. The new project `DualFrontier.Mod.ManifestRewriter.Tests` joins the count at M7.4; the M5 closure report's four-project layout is now five-project.

### Three-commit invariant (METHODOLOGY §7.3)

The M7 batch comprises 50 non-merge commits ahead of the M6 closure
baseline (`c7210ca`, M6 closure verification report). The commits decompose into:

- **21 M-cycle commits**, three per M-sub-phase (`feat → test → docs`) across 7 sub-phases.
- **18 housekeeping commits**, three per pass (`fix/feat → test → docs`) across 6 passes.
- **11 sundry commits** — pre-flight v1.4 ratification, mid-batch v1.5 ratification + audit campaign Pass 1–5 chain, doc-only syncs, and M7.3 follow-up.

Each M-sub-phase and housekeeping pass was checked out independently
during ROADMAP closure for that sub-phase / pass and verified at
0 W / 0 E with a passing `dotnet test`. Per-sub-phase test progression:

| # | Commit | Sub-phase / Housekeeping | Subject (truncated) | Tests |
|---|---|---|---|---|
| — | `c7210ca` | (M6 closure) | docs(review): M6 closure verification report | **338** |
| — | `b504813` | (M7 pre-flight) | docs(architecture): ratify v1.4 — pre-flight clarifications to §9.5 for M7 | **338** |
| 1 | `a2ab761` | M7.1 | feat(modding): add Pause/Resume + IsRunning state to ModIntegrationPipeline | 338 |
| 2 | `c964475` | M7.1 | test(modding): M7.1 Pause/Resume guard tests | **349** |
| 3 | `0606c43` | M7.1 | docs(roadmap): close M7.1 — Pause/Resume on ModIntegrationPipeline | 349 |
| — | `f3e92fb` | (sync) | docs(roadmap): sync stale v1.3 ref to v1.4 in M-Section preamble | 349 |
| 4 | `2531ed7` | M7.2 | feat(modding): retain RestrictedModApi on LoadedMod and add UnloadMod chain to pipeline | 349 |
| 5 | `d68ba93` | M7.2 | test(modding): M7.2 ALC unload chain tests | **362** |
| 6 | `c3f5251` | M7.2 | docs(roadmap): close M7.2 — ALC unload chain steps 1-6 | 362 |
| 7 | `9bed1a4` | M7.3 | feat(modding): UnloadMod step 7 — WeakReference + GC pump + ModUnloadTimeout | 362 |
| 8 | `46b4f33` | M7.3 | test(modding): §9.5 step 7 protocol + Phase 2 carried-debt closure | **369** |
| 9 | `1d43858` | M7.3 | docs(roadmap): close M7.3 — UnloadMod step 7 + Phase 2 carried-debt closure | 369 |
| — | `3f00c2a` | (mid-batch ratify) | docs(architecture): ratify v1.5 | 369 |
| — | `6e9c433` | (audit Pass 2) | Update AUDIT_PASS_2_SPEC_CODE.md | 369 |
| — | `d1c1338` | (audit Pass 1 sync) | Pass 1 baseline sync delta | 369 |
| — | `249fff2` | (audit Pass 4) | docs(audit): close Pass 4 + draft Pass 5 prompt — final pass of campaign | 369 |
| — | `5183c15` | (audit Pass 5) | docs(audit): close Pass 5 — campaign GREEN-with-debt verdict | 369 |
| — | `f4b2cb8` | (M7.3 follow-up) | docs(coding-standards): persist M7.3's display-class hoisting finding | 369 |
| 10 | `5385fe5` | M7.4 | feat(tools): manifest-rewriter CLI tool for D-7 hotReload override | 369 |
| 11 | `8d51a9d` | M7.4 | test(tools): ManifestRewriter unit tests + Fixture.VanillaMod_HotReloadOverride + M74 integration tests | **378** |
| 12 | `4ff04f8` | M7.4 | docs(roadmap): close M7.4 — D-7 build-pipeline hotReload override | 378 |
| 13 | `9c895fe` | M7.5.A | feat(modding): ModMenuController + IModDiscoverer + Pipeline.GetActiveMods | 378 |
| 14 | `4c648c6` | M7.5.A | test(modding): ModMenuController + DefaultModDiscoverer + Pipeline.GetActiveMods coverage | **408** |
| 15 | `198d948` | M7.5.A | docs(roadmap): close M7.5.A — ModMenuController + pipeline read API | 408 |
| 16 | `4956a13` | M7.5.B.1 | feat(bootstrap): wire ModIntegrationPipeline + ModMenuController into GameBootstrap | 408 |
| 17 | `94128be` | M7.5.B.1 | test(bootstrap): GameBootstrap.CreateLoop integration smoke tests | **415** |
| 18 | `b6b6d7e` | M7.5.B.1 | docs(roadmap): close M7.5.B.1 — production bootstrap integration | 415 |
| — | `c3e88ac` | (sundry) | Add M7.3–M7.5 docs prompts | 415 |
| ── | ── | ── housekeeping ── | ── | ── |
| H1.1 | `1b16e9e` | TICK display | fix(presentation): wire TickAdvancedCommand to GameHUD tick counter | 415 |
| H1.2 | `2192188` | TICK display | test(presentation): integration test for TickAdvancedCommand bridge publishing | **416** |
| H1.3 | `3d800d2` | TICK display | docs(roadmap): add Backlog section + housekeeping closure entry | 416 |
| H2.1 | `e0b0ecf` | TickScheduler race | fix(core): TickScheduler.ShouldRun cache thread-safety via ConcurrentDictionary | 416 |
| H2.2 | `52d6d3f` | TickScheduler race | test(core): parallel stress test for TickScheduler.ShouldRun | **417** |
| H2.3 | `700cbc0` | TickScheduler race | docs(roadmap): close TickScheduler.ShouldRun cache race finding | 417 |
| — | `1ab757f` | (sundry) | Add *.zip to .gitignore | 417 |
| H3.1 | `9141bd6` | Real pawn data | feat(scenario): real pawn data — IdentityComponent + RandomPawnFactory + 10 colonists + UI wiring | 417 |
| H3.2 | `659a64a` | Real pawn data | test(scenario,bootstrap): RandomPawnFactory unit tests + bootstrap integration coverage | **428** |
| H3.3 | `74d2eed` | Real pawn data | docs(roadmap): real pawn data closure + categorized Phase 5 backlog | 428 |
| H4.1 | `ee12108` | Needs decay direction | fix(systems): NeedsSystem decay direction — deficit accumulates without recovery | 428 |
| H4.2 | `7ea038c` | Needs decay direction | test(systems): regression guard for needs-accumulation direction | **431** |
| H4.3 | `9d7b7f6` | Needs decay direction | docs(roadmap): close needs decay direction; update Phase 5 backlog | 431 |
| ── | ── | ── back to M-cycle ── | ── | ── |
| 19 | `9a75b75` | M7.5.B.2 | feat(presentation): ModMenuPanel + F10 hotkey wiring + GameRoot integration | 431 |
| 20 | `6ecee53` | M7.5.B.2 | test(bootstrap): mod-menu interaction sequence integration tests | **434** |
| 21 | `b519804` | M7.5.B.2 | docs(roadmap): close M7.5.B.2 — Godot mod-menu UI scene | 434 |
| ── | ── | ── housekeeping ── | ── | ── |
| H5.1 | `5f0b4f5` | ModMenuPanel position | fix(presentation): ModMenuPanel position via CanvasLayer conversion | 434 |
| H5.2 | `805b882` | Assets gitignore | chore(repo): ignore extracted asset folders, keep source zips tracked | 434 |
| H5.3 | `0b11d1f` | ModMenuPanel position | docs(roadmap): note ModMenuPanel position fix and asset extraction prep | 434 |
| — | `cc9b7a5` | (sundry) | UI: ModMenuPanel CanvasLayer fix & housekeeping docs | 434 |
| H6.1 | `9f87536` | Menu pauses simulation | fix(application): menu actually pauses simulation via lifecycle hooks | 434 |
| H6.2 | `d8a448f` | Menu pauses simulation | test(bootstrap): menu lifecycle pauses and resumes simulation | **437** |
| H6.3 | `110ad61` | Menu pauses simulation | docs(roadmap): close menu-pause wiring; flag §9.2 v1.6 candidate | 437 |

The 21 M-cycle commits comprise 7 × 3 sub-phase triples; the 18
housekeeping commits comprise 6 × 3 fix/test/docs triples (the H5.x
pass spans both the position fix and the assets-gitignore companion in
a single coherent triple); 11 sundry commits cover pre-flight v1.4
ratification, mid-batch v1.5 ratification + audit campaign Pass 1–5
sync, the M7.3 display-class hoisting finding doc, two prompt-set
additions, one *.zip gitignore mini-pass, one CanvasLayer-fix narrative
marker, and one stale-version-sync. Sundry commits are doc-only or
config-only and preserve test count by construction.

The H5 housekeeping pass is the only triple that does not increment
the test count by direct addition — its ModMenuPanel position fix is
visual (CanvasLayer parent type change observable only via F5) and the
test coverage for that surface is registered as Phase 4 UI convention
(no automated tests for Godot widgets, per AD #13 of M7.5.B.2). The
gitignore companion is repo hygiene without a test surface. The H6
follow-up pass (menu-pauses-simulation) added the three contract tests
that close the visual-only gap from H5.

**Verdict:** PASSED. The three-commit invariant from METHODOLOGY §7.3
holds across each M-sub-phase and housekeeping pass independently. No
sub-phase or pass closure ships a broken build or failing tests.

---

## §2 Spec ↔ code ↔ test triple consistency

`MOD_OS_ARCHITECTURE` §11.1 (M7 row) declares M7 acceptance through
five high-level acceptance bullets backed by §9 LOCKED chapter
(specifically §9.2 menu flow, §9.3 run-flag enforcement, §9.5 unload
chain, §9.5.1 best-effort failure semantics, §9.6 hot-reload-disabled
mods) plus §10.4 WeakReference unload requirement. Each in-batch
acceptance item is verified through three legs (spec section,
implementation file:line, dedicated test):

| # | Acceptance | Sub-phase | Spec leg | Code leg | Test leg |
|---|---|---|---|---|---|
| 1 | Pause / Resume + `IsRunning` | M7.1 | §9.2, §9.3 | [ModIntegrationPipeline.cs](../../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs) `_isRunning` private bool, `Pause`/`Resume` setters, `IsRunning` getter | `M71PauseResumeTests` (11 — default state, idempotent setters, paused→running→paused round-trip, default-paused regression guard) |
| 2 | `Apply` paused-only guard with verbatim §9.3 message | M7.1 | §9.3 | [ModIntegrationPipeline.cs](../../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs) `Apply` early-throw when `_isRunning == true` with `"Pause the scheduler before applying mods"` | `M71PauseResumeTests` Apply-guard exact-string match |
| 3 | `UnloadAll` paused-only guard mirrored | M7.1 | §9.3 | [ModIntegrationPipeline.cs](../../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs) `UnloadAll` mirroring guard with `"Pause the scheduler before unloading mods"` | `M71PauseResumeTests` UnloadAll-guard exact-string match |
| 4 | ALC unload chain steps 1–6 + best-effort failure discipline | M7.2 | §9.5 steps 1–6, §9.5.1 | [ModIntegrationPipeline.cs](../../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs) `UnloadMod(modId): IReadOnlyList<ValidationWarning>` with private `TryUnloadStep` helper accumulating per-step warnings | `M72UnloadChainTests` (13 — run-flag guard parity, idempotency for non-active mods, per-step verification of steps 1–6, step-2-throws seam via `ThrowingRevokeAllContractStore` decorator, mod-removed-from-active-set on failure, warning shape, `UnloadAll` accumulator preservation, M7.1 guard preservation) |
| 5 | `UnloadMod` runs step 7 after step 6 attempt regardless of outcome (AD #7) | M7.3 | §9.5 step 7, §9.5.1 | [ModIntegrationPipeline.cs](../../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs) `RunUnloadSteps1Through6AndCaptureAlc` non-inlined helper + `TryStep7AlcVerification` non-inlined helper | `M73Step7Tests` (5 — happy-path step 7 release on empty in-memory ALC, ALC-retainer timeout path emits `ModUnloadTimeout`, canonical warning shape locked at substring level, AD #7 step-7-after-upstream-failure invariant, mod-removed-from-active-set after timeout) |
| 6 | WeakReference spin loop with mandatory GC pump bracket | M7.3 | §9.5 step 7 (v1.4 clarification) | `TryStep7AlcVerification`: 100 × 100 ms cadence, `GC.Collect → GC.WaitForPendingFinalizers → GC.Collect` each iteration | `M73Step7Tests` happy-path + timeout path (both exercise the spin loop body) |
| 7 | **Phase 2 carried debt closure** — every regular mod under test passes WR unload check (§10.4 hard-required) | M7.3 | §10.4, §11.4 | `ModUnloadAssertions.AssertAlcReleasedWithin` mirrors the production spin pattern for fixture-based tests; integration via `pipeline.Apply` + `pipeline.UnloadMod` against real-mod fixtures | `M73Phase2DebtTests` (2 real-mod fixtures — `Fixture.RegularMod_DependedOn` minimal surface, `Fixture.RegularMod_ReplacesCombat` with system registration + bridge replacement; both close §10.4 hard-required `WeakReference.IsAlive == false` within timeout, zero flakes across 3 consecutive runs at closure time per §11.4 invariant) |
| 8 | D-7 build-pipeline `hotReload: true → false` override (vanilla mods only) | M7.4 | §11.1 D-7, §9.6 | [`tools/DualFrontier.Mod.ManifestRewriter/`](../../tools/DualFrontier.Mod.ManifestRewriter/) standalone CLI tool + `mods/Directory.Build.targets` MSBuild target gated on `<IsVanillaMod>true</IsVanillaMod>` AND `$(Configuration)=='Release'` | `ManifestRewriterTests` (7 — Rewritten happy path + all-fields-preserved round-trip, AlreadyFalse / FieldAbsent byte-identical no-ops, NotFound / ParseError failure semantics, idempotency under repeat invocation) + `M74BuildPipelineTests` (2 integration tests via `dotnet build` subprocess against `Fixture.VanillaMod_HotReloadOverride`) |
| 9 | `ModMenuController` editing-session lifecycle (BeginEditing / Toggle / Commit / Cancel) | M7.5.A | §9.2 (menu flow Pause-Toggle-Apply-Resume), §9.6 | `src/DualFrontier.Application/Modding/ModMenuController.cs` (internal sealed) | `ModMenuControllerTests` (22) |
| 10 | `IModDiscoverer` + `DefaultModDiscoverer` (best-effort directory scan) | M7.5.A | (implied by §9.2 step 1 — menu shows mod set) | `src/DualFrontier.Application/Modding/IModDiscoverer.cs` + `DefaultModDiscoverer.cs` | `DefaultModDiscovererTests` (4 — non-existent root empty for first-launch safety, manifest-less subdir skipped, valid manifest parsed, broken manifest swallowed) |
| 11 | `ModIntegrationPipeline.GetActiveMods()` public read API | M7.5.A | (supports §9.2 menu view) | [ModIntegrationPipeline.cs](../../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs) `GetActiveMods` public method returning `IReadOnlyList<ActiveModInfo>` | `PipelineGetActiveModsTests` (4 — empty-pipeline empty list, post-Apply contents with manifest preserved verbatim, post-UnloadMod removal, fresh-list snapshot not live view) |
| 12 | §9.6 `hotReload: false` cannot be reloaded mid-session (rejected via `ToggleResult.RejectedHotReloadDisabled`) | M7.5.A | §9.6 | `ModMenuController.Toggle` `RejectedHotReloadDisabled` guard | `ModMenuControllerTests` `RejectedHotReloadDisabled` rejection cases |
| 13 | M7.5.A AD #4 — failed `Commit` leaves session paused + open | M7.5.A | §9.2 success-path interpretation (deliberate registration in ROADMAP §M7 footer) | `ModMenuController.Commit` failure branch — does NOT call `Pipeline.Resume`, does NOT close session | `ModMenuControllerTests` failure-stays-paused + retry-recovery |
| 14 | M7.5.A AD #5 — first-load is not a reload (§9.6 + §2.2 combined reading) | M7.5.A | §9.6, §2.2 deliberate registration | `ModMenuController.Toggle` allows discovered-only `hotReload: false` mods to be added inside editing session | `ModMenuControllerTests` AD #5 first-load-is-not-reload |
| 15 | M7.5.A AD #6 — idempotent BeginEditing / Cancel | M7.5.A | (deliberate, registered in ROADMAP) | `ModMenuController` BeginEditing-while-editing → silent no-op; Cancel-while-not-editing → silent no-op (does NOT call `Resume`) | `ModMenuControllerTests` AD #6 idempotency |
| 16 | Production `GameBootstrap.CreateLoop` returns `GameContext(Loop, Controller)` | M7.5.B.1 | §11.1 M7 acceptance ("user toggles a mod, hits Apply") — bootstrap path is the production entry | [GameBootstrap.cs](../../src/DualFrontier.Application/Bootstrap/GameBootstrap.cs) refactored, [GameContext.cs](../../src/DualFrontier.Application/Bootstrap/GameContext.cs) new return shape | `GameBootstrapIntegrationTests` (7 — production-side smoke coverage of `CreateLoop`) |
| 17 | M7.5.B.1 AD #5 — coreSystems flow into both kernel scheduler AND `ModRegistry.SetCoreSystems` | M7.5.B.1 | §7.1 step 1–2 (replacement-set construction visibility) | `GameBootstrap.CreateLoop` local `SystemBase[] coreSystems` array threaded both ways | `GameBootstrapIntegrationTests` `WithModsRootContainingFixture_GetEditableStateReturnsFixture` exercises end-to-end discovery through production wiring |
| 18 | `ModMenuPanel` Godot UI overlay + F10 hotkey | M7.5.B.2 | §9.2 step 1 (menu UI surface) | [ModMenuPanel.cs](../../src/DualFrontier.Presentation/UI/ModMenuPanel.cs) `Godot.CanvasLayer` (post-housekeeping H5; original was `Godot.Control`) | `MenuFlow_OpenCommitClose_LeavesEditingFalse`, `MenuFlow_OpenCancelClose_LeavesEditingFalse`, `MenuFlow_OpenWithoutCommitOrCancel_StaysEditing` |
| 19 | Menu lifecycle pauses + resumes simulation thread | M7.5.B.2 + housekeeping H6 | §9.2 step 1 ("menu sets the scheduler's run flag to false") + step 4 ("menu calls Resume and the simulation continues") | `GameBootstrap.CreateLoop` registers `OnEditingBegan` / `OnEditingEnded` hooks calling `GameLoop.SetPaused` in lockstep with `pipeline.Pause`/`Resume` | `MenuFlow_BeginEditing_PausesGameLoop`, `MenuFlow_Cancel_ResumesGameLoop`, `MenuFlow_CommitSuccess_ResumesGameLoop` |

### §7.5 fifth scenario (M6→M7 hand-off) — outcome (B) Structural verification

§7.5 line 5 ("Mod is unloaded — replacement skip is reverted, kernel
bridge re-registers, dependency graph rebuilds") was registered as the
M6→M7 hand-off in [M6_CLOSURE_REVIEW.md](./M6_CLOSURE_REVIEW.md) §7
("acceptable carried hand-off, not debt"). At M7 closure, no joint
end-to-end test exists asserting the unload→re-Apply→bridge-reregister
sequence in a single scenario (verified via
`grep -rn "ReplacementRevert\|UnloadedMod_RestoresKernel\|UnloadMod.*BridgeReregister" tests/`
returning zero matches in non-binary files).

The structural argument from M6_CLOSURE_REVIEW §8 holds verbatim,
verified by disjoint tests:

- **M7.2 unload chain** (`M72UnloadChainTests` step 3 verification +
  mod-removed-from-active-set assertion): `UnloadMod` removes the mod
  from `_activeMods` regardless of any per-step failures. Once removed,
  the next `Apply` invocation does not see the mod's `Replaces` entries
  in its input set.
- **M6.2 parameter-driven `CollectReplacedFqns`** (`CollectReplacedFqnsTests`
  five tests): the helper operates on its `loaded` argument, not on
  persistent state. Recomputing without an unloaded mod's contribution
  naturally drops that mod's `Replaces` FQNs from the skip set.
- **Combined**: re-`Apply` after `UnloadMod` rebuilds the dependency
  graph from `_registry.GetAllSystems()` minus the unloaded mod's
  systems (M7.2 step 3 removed them), with `replacedFqns` recomputed
  without the unloaded mod's contribution (M6.2 helper). The kernel
  bridge whose FQN was previously in the skip set is no longer skipped
  — it re-enters the rebuilt scheduler via the unmodified Core-systems
  loop in `Apply` step `[5-7]`.

**Outcome (B): Structural verification.** No joint end-to-end test was
added during M7 — the structural argument from M6_CLOSURE_REVIEW §8
holds and is verified by the disjoint tests M7.2 (`UnloadMod` removes
from `_activeMods`) and M6.2 (`CollectReplacedFqns` is parameter-driven).
A future M8+ load batch that unloads a vanilla mod replacing a
combat-bridge stub will exercise this end-to-end naturally; M9
(Vanilla.Combat) is the first realistic load pattern that would justify
adding a dedicated joint test.

### Regression guards

The M0–M6 surface continues to function unchanged after M7's
modifications:

- All 240 M5 Modding tests + 18 M6 Modding tests preserved. The
  `LoadedMod.Api` non-positional record-member addition (M7.2) was
  deliberately non-breaking — every M0–M6 `new LoadedMod(...)` call site
  keeps compiling.
- M5's `TopoSortByPredicate` regression guard
  (`SharedMods_DependCycleStill_DetectedSeparately`) still passes after
  M7's `UnloadAll` refactor.
- M6's `Apply_WithoutAnyReplaces_BridgesAllRegistered` pipeline-level
  regression guard still passes after M7's `UnloadMod` chain integration
  (the no-replaces happy-path is unchanged).
- M7.1's default-paused regression guard
  (`Default_AfterConstruction_IsPaused`) is load-bearing — every M0–M6
  test constructs a fresh pipeline and calls `Apply` without ever
  touching `Pause`/`Resume`, and the new guard remains a no-op for that
  pre-existing path.

**Verdict:** PASSED. All §11.1 M7 acceptance bullets and §9 LOCKED
chapter references for M7 have all three legs present and verified. The
§7.5 fifth scenario M6→M7 hand-off is closed via structural argument
(outcome B), honestly reported as such — the disjoint M7.2 + M6.2 test
surfaces collectively exercise the structural property without a single
end-to-end joint test.

---

## §3 Cross-document consistency

Three primary documents must agree on the v1.5 / M7-closed / 437-tests
state. This is the first M-cycle closure since M3 where
`MOD_OS_ARCHITECTURE.md` is **not byte-identical** to its prior closure
state — both the M7 pre-flight v1.4 ratification (`b504813`) and the
mid-batch v1.5 audit-Pass-2 ratification (`3f00c2a`) happened during
the M7 batch. This is a deviation from the M3–M6 pattern (where every
closure had `git diff <baseline>..HEAD -- docs/architecture/MOD_OS_ARCHITECTURE.md`
returning zero output) and is explicitly handled in §5.2 below.

### Spec diff between M6 closure baseline and HEAD

`git diff c7210ca..HEAD -- docs/architecture/MOD_OS_ARCHITECTURE.md` shows 21 lines
changed, distributed across:

- **Status line** (line 8): `LOCKED v1.3 — …M4.3 implementation review (v1.3) applied.` → `LOCKED v1.5 — …M7 pre-flight readiness review (v1.4), and Audit Campaign Pass 2 (v1.5) applied.`
- **v1.3 changelog entry**: removed "(this version)" annotation (was promoted to historical entry).
- **v1.4 changelog entry**: new entry covering §9.5 step 7 GC pump protocol + new §9.5.1 «Failure semantics» sub-section.
- **v1.5 changelog entry**: new entry covering §11.2 baseline enumeration of `ValidationErrorKind` (now lists `IncompatibleContractsVersion`, `WriteWriteConflict`, `MissingDependency`, `CyclicDependency` instead of just the latter two).
- **§9.5 step 7 body** (line 765): expanded with the explicit GC pump protocol (`GC.Collect → GC.WaitForPendingFinalizers → GC.Collect` double-collect bracket, 100 × 100 ms cadence).
- **New §9.5.1** (after §9.5): «Failure semantics» — locks the best-effort discipline already implicit in steps 1–6.
- **§11.2 baseline** (line ~858): updated «The current enum has X and Y» wording to list the actual 4-member baseline.

Every line in the diff matches a changelog entry — no silent
ratifications. The mid-batch ratifications are a deviation from the
M5/M6 byte-identity pattern but they remain non-strategic
(see §5.2 D-1..D-7 byte-identity verification).

### Cross-document table

| Document | Field | Expected | Found | Status |
|---|---|---|---|---|
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | Status line (line 8) | `LOCKED v1.5` | `LOCKED v1.5 — Phase 0 closed; non-semantic corrections from M1–M3.1 audit (v1.1), M3 closure review (v1.2), M4.3 implementation review (v1.3), M7 pre-flight readiness review (v1.4), and Audit Campaign Pass 2 (v1.5) applied.` | ✓ |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | v1.4 changelog entry | covers §9.5 step 7 GC pump + §9.5.1 best-effort | present | ✓ |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | v1.5 changelog entry | covers §11.2 baseline enumeration | present | ✓ |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | §9.5.1 sub-section | new section locking best-effort failure semantics | present after §9.5 | ✓ |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | §12 D-1..D-7 byte-identity | byte-identical to M6 closure baseline `c7210ca` | `diff <(awk '/^## 12\./,EOF' /tmp/m6_spec.md) <(awk '/^## 12\./,EOF' docs/architecture/MOD_OS_ARCHITECTURE.md)` returns zero output | ✓ |
| `docs/ROADMAP.md` | Header date (line 11) | `2026-05-03` | post-update: `*Updated: 2026-05-03 (M7 closed; M8 (Vanilla skeletons) is the next phase; §9.2 v1.6 ratification candidate registered).*` | ✓ |
| `docs/ROADMAP.md` | M7 row (line 30) | `✅ Closed` | post-update: `M7 — Hot reload \| ✅ Closed \| 437/437 (50 commits)` | ✓ |
| `docs/ROADMAP.md` | All 7 M7 sub-phase entries | each `✅ Closed` with commit SHAs | M7.1, M7.2, M7.3, M7.4, M7.5.A, M7.5.B.1, M7.5.B.2 each `✅` with their feat/test/docs commit triples | ✓ |
| `docs/ROADMAP.md` | 6 housekeeping closure entries | each documented in M7.5.B.2 follow-up nested block + Backlog Resolved section | TICK display, TickScheduler race, real pawn data, needs decay, ModMenuPanel position + assets gitignore, menu pauses simulation each have closure prose | ✓ |
| `docs/ROADMAP.md` | Engine snapshot (line 36) | `437/437` | `Test count: 434 → **437/437**.` | ✓ |
| `docs/ROADMAP.md` | M8 row | `⏭ Pending` | `M8 — Vanilla skeletons \| ⏭ Pending \| — \| Five empty mod assemblies` | ✓ |
| `docs/ROADMAP.md` | v1.5 LOCKED references | header + see-also + migration prelude | post-surgical-fix line 3 `v1.5 LOCKED`; line 157 `v1.5 §11`; line 516 `v1.5 LOCKED specification` | ✓ (post §9 surgical fixes) |
| `docs/README.md` | Architecture list entry (line 28) | `v1.5 LOCKED` | post-surgical-fix `**v1.5 LOCKED.**` | ✓ (post §9 surgical fixes) |
| `.gitignore` | Kenney + Cinzel patterns | extracted asset folders ignored, source zips tracked | lines 32–38: comment + `/assets/cinzel/`, `/assets/Cinzel/`, `/assets/kenney_*/`, `/assets/Kenney_*/` (matches commit `805b882` housekeeping intent) | ✓ |

The pre-surgical-fix ROADMAP and README v1.4 references were stale
post-v1.5 — registered as Audit Campaign Pass 4 §14 Tier 3 #2/#3/#5
findings (and Pass 5 H-1b/H-1c/H-1e triage entries). Surgical fixes
applied in this closure session resolve them per §9 below.

The `v1.0 LOCKED` references at `ROADMAP.md:22` (M0 row Output column)
and `ROADMAP.md:163` (M0 prose) describe the artifact M0 **closed** —
historically accurate (v1.1–v1.5 are non-semantic ratifications that
did not reopen M0). Same handling as M3–M6 closures: not navigation
references.

The `v1.2` reference at `ROADMAP.md:25` (M3 row Notes column,
"hybrid per `MOD_OS_ARCHITECTURE` §3.6 v1.2") is similarly historical:
it attributes the §3.6 hybrid-enforcement formulation to the version of
the spec that ratified it (M3 closure). Not a navigation reference.

**Verdict:** PASSED (post-surgical-fix). After applying the six §9
surgical fixes, no version, date, status, or test-count drift among the
three primary documents.

---

## §4 Stale-reference sweep

Patterns checked across `docs/` after surgical fixes are applied:

| Pattern | Hits in active-navigation context | Disposition |
|---|---|---|
| `M7 in progress` (active) | 0 (post-update) | Clean. The pattern survives in `docs/audit/AUDIT_PASS_1_INVENTORY.md:169` as a Pass 1 inventory snapshot describing M7.4–M7.5 pending **at the time the inventory was taken** — frozen audit-trail context, by definition not active-navigation. The pattern in `docs/prompts/M7_CLOSURE.md` is the brief-text of this very session and not active navigation. |
| `🔨 In progress` / `🔨 Current` (M7 row) | 0 (post-update) | Clean. The 🔨 glyph survives only in the `ROADMAP.md` section heading `## 🔨 Mod-OS Migration (M0–M10)` — a category label, not a status marker (preserved from M3/M4/M5/M6 closures). |
| `M7.X.X pending` for any X | 0 | Clean. M7-closure entry that was previously `⏭ Pending` is now subsumed by this closure document. |
| `M7-closure pending` | 0 (post-update) | Clean post-update. Was at `ROADMAP.md:30` Notes column tail and `ROADMAP.md:11` header prose — both subsumed by the closure update. |
| `Updated: 2026-04-30` / `2026-05-01` / `2026-05-02` | 0 in `ROADMAP.md` header | Clean. Post-update header reads `Updated: 2026-05-03`. The earlier dates survive only in M5/M6 closure reviews (frozen audit trail) and as commit-message content (not subject to sweep). |
| `311` / `328` / `333` (M5–M6 intermediate counts) | 0 outside frozen audit trail | Clean. These appear only in M5 / M6 closure reviews (frozen by definition) or as line-number citations in `NORMALIZATION_REPORT.md` (column header, not test count, preserved per M5/M6 closure precedent). |
| `338` (M6 closure baseline) | 0 outside frozen audit trail | Clean. Appears only in M6 closure review (frozen) and in the present document's §1 three-commit invariant table (cites the per-commit baseline, not the current state). |
| `415` / `416` / `417` / `428` / `431` / `434` (M7 intermediate counts) | 0 outside historical-progression context | Clean. The intermediate counts appear in `ROADMAP.md` engine snapshot as historical commit-progression text (the canonical state pointer is `437` at the end of the same paragraph), in this document's §1 three-commit invariant table, and in M7 sub-phase / housekeeping prompt files (frozen at the time of writing). The M7-era prompts (`M7_HOUSEKEEPING_TICK_DISPLAY.md`, `HOUSEKEEPING_REAL_PAWN_DATA.md`, etc.) cite their authoring-time baselines and are not active-navigation references. |
| `v1.0 LOCKED` (active navigation) | 2 | Both at `ROADMAP.md:22` (M0 row Output column) and `ROADMAP.md:163` (M0 prose) — historical attribution of what M0 closed; not navigation references (preserved from M3/M4/M5/M6 closures). |
| `v1.1 LOCKED` (active navigation) | 0 | Clean. |
| `v1.2 LOCKED` (active navigation) | 0 | Clean. M3-closure attribution at `ROADMAP.md:25` is `v1.2` without `LOCKED`, preserved as historical. |
| `v1.3 LOCKED` (active navigation) | 0 | Clean. Historical attribution only. |
| `v1.4 LOCKED` (active navigation) | 0 (post-surgical-fix) | Clean. Pre-surgical-fix matches at `docs/README.md:28`, `docs/ROADMAP.md:3`, `docs/ROADMAP.md:157` (as `v1.4 §11`), and `docs/ROADMAP.md:516` resolved per §9. Surviving matches in `docs/audit/AUDIT_*` files are frozen Pass 1–5 audit trail (by definition). |
| `v1.5 ratification` as hypothetical-future clause | 0 (post-surgical-fix) | Clean. Three deliberate-interpretation footers in `docs/ROADMAP.md:390/392/394` updated to `v1.6 ratification` per §9 (v1.5 was consumed by audit campaign Pass 2; the next available ratification slot is v1.6). |
| `M8 ⏭ Pending` (positive: required pattern) | 1 | Present at `ROADMAP.md:31` as expected — M8 is the next phase, marked Pending. |
| `Fixture.SharedEvents.dll` (post-v1.3 forbidden literal) | 0 | Clean (preserved from M4/M5/M6 closures). |
| `ignored for kind=shared` (post-v1.3 forbidden wording) | 0 | Clean (preserved from M4/M5/M6 closures). |

Tangential matches verified as non-test-count usage:
- `NORMALIZATION_REPORT.md:338` references `BuildWriteViolationMessage`
  source-line column header (not a test count, preserved from M5/M6
  closure precedent).

**Verdict:** PASSED (post-surgical-fix). No active document carries a
stale status, an incorrect test count, or a stale version pointer. The
historical-context occurrences are by design and explicitly classified
in the table above.

---

## §5 Methodology compliance

### §5.1 Commit scope prefixes (METHODOLOGY §7.3)

All 50 non-merge commits in the M7 batch carry conventional scope
prefixes:

- **M-cycle commits** (21): `feat → test → docs` triples per
  sub-phase, `feat(modding|tools|bootstrap|presentation)` for the
  feature commit, `test(...)` for the test commit, `docs(roadmap)` for
  the closure commit. All bodies are substantive.
- **Housekeeping commits** (18): `fix → test → docs` triples per pass
  (or `feat → test → docs` for net-positive features). Specific
  scopes: `fix(presentation|core|systems|application)`,
  `chore(repo)` for the assets gitignore companion, `docs(roadmap)`
  for closure. All bodies are substantive.
- **Sundry commits** (11): `docs(architecture)` × 2 for the v1.4 +
  v1.5 ratifications (each in its own commit, with substantive
  changelog body); `docs(audit)` × 2 for Pass 4 + Pass 5 closures;
  unprefixed sync / inventory commits for the audit campaign Pass 1–2
  intermediate states (4 commits — these were active-development snapshots
  during the campaign, not phase closures); `docs(coding-standards)`
  for M7.3 follow-up; bare-titled sundry commits for prompt-set additions
  and gitignore mini-passes. The unprefixed audit-campaign sync commits
  are the only commits in the batch without conventional prefixes —
  registered as a §10 finding.

The two architectural ratification commits (`b504813` v1.4, `3f00c2a`
v1.5) are the load-bearing entries: each ships under
`docs(architecture)`, each contains a substantive changelog body in
`MOD_OS_ARCHITECTURE.md`, and neither is bundled with implementation
work — both are spec-only changes with their own dedicated commit per
the M3-closure precedent for ratification commits.

### §5.2 LOCKED decision sanctity — D-1..D-7 byte-identity

**This is the strongest possible falsifiable signal that the M7 batch
did not silently re-open any LOCKED strategic decision.**

`MOD_OS_ARCHITECTURE.md` is **NOT byte-identical** between
`c7210ca` (M6 closure verification report HEAD) and current HEAD —
unlike M3–M6 closures, M7 introduced a non-empty 21-line diff (v1.4 +
v1.5 ratifications). However:

- The full diff is contained in the **status line + version-history
  changelog entries + §9.5 step 7 body + new §9.5.1 sub-section + §11.2
  baseline-enumeration line**.
- `git diff c7210ca..HEAD -- docs/architecture/MOD_OS_ARCHITECTURE.md` shows zero
  changes inside the §12 «Locked decisions» section (D-1 through D-7).
  Verified directly:

```
$ diff <(awk '/^## 12\./,EOF' /tmp/m6_spec.md) \
       <(awk '/^## 12\./,EOF' docs/architecture/MOD_OS_ARCHITECTURE.md) | wc -l
0
```

Where `/tmp/m6_spec.md` is `git show c7210ca:docs/architecture/MOD_OS_ARCHITECTURE.md`.

This is the structural meaning: the M7 batch's spec ratifications are
**non-strategic** by construction. The v1.4 entry is a clarification of
§9.5 step 7's spin-loop mechanics (an implementation-level GC pump
protocol, not an architectural decision); the v1.5 entry is a
declarative-wording correction in §11.2 (the actual `ValidationErrorKind`
baseline was always 4 members; v1.0–v1.4 wording understated the
baseline). Each changelog entry explicitly states «No semantic changes.
No locked decision (D-1 through D-7) is altered.» — and the byte-identity
check above confirms this is a falsifiable claim, not just a footer
disclaimer.

**M7 introduces zero strategic changes.** Both ratifications are
non-strategic clarifications; the strongest possible falsifiable signal
here is the §12 byte-identity check, and it holds.

### §5.3 Deliberate-interpretation registrations

M7 has six deliberate-interpretation registrations in ROADMAP per the
"no improvisation" rule, each with falsifiable artifacts (test names)
and an explicit escalation path:

1. **M7.1 §9.2/§9.3 footer (flag location on the pipeline, not the
   scheduler)**. Registered in [ROADMAP.md](../ROADMAP.md) §M7 footer
   #1. Falsifying artifacts: `M71PauseResumeTests` exact-string-match
   guard for the canonical messages. Escalation: future v1.6 ratification
   if scheduler-side flag becomes preferable (post-surgical-fix wording —
   see §9 below).

2. **M7.3 §9.5/§9.5.1 footer (step 7 ordering: capture WR → remove from
   active set → spin)**. Registered in
   [ROADMAP.md](../ROADMAP.md) §M7 footer #2.
   Falsifying artifacts: `M73Step7Tests` AD #7 step-7-after-upstream-failure
   invariant. Escalation: future v1.6 ratification if
   ordering needs revision (post-surgical-fix wording).

3. **M7.5.A AD #4 — failed `Commit` leaves session paused + open**.
   Registered in [ROADMAP.md](../ROADMAP.md) §M7 footer #3.
   Falsifying artifacts: `ModMenuControllerTests` failure-stays-paused-session-open
   + retry-recovery. Escalation: future v1.6 ratification if a future
   M7 closure review (i.e., this one) finds the reading materially
   incompatible with §9 wording (post-surgical-fix wording).

4. **M7.5.A AD #5 — first-load is not a reload**. Registered in
   [ROADMAP.md](../ROADMAP.md) M7.5.A acceptance bullet block as a
   companion to AD #4 (combined reading of §9.6 + §2.2). Falsifying
   artifacts: `ModMenuControllerTests` AD #5 first-load-is-not-reload.

5. **M7.5.A AD #6 — idempotent BeginEditing / Cancel**. Registered in
   [ROADMAP.md](../ROADMAP.md) M7.5.A acceptance bullet. Falsifying
   artifacts: `ModMenuControllerTests` BeginEditing-while-editing
   silent-no-op + Cancel-while-not-editing silent-no-op-without-Resume.

6. **§9.2 v1.6 ratification candidate (orchestration-layer wiring of
   two pause flags)**. Registered in [ROADMAP.md](../ROADMAP.md)
   M7.5.B.2 second-follow-up housekeeping block — the spec wording
   «menu pauses the scheduler» reads as a single entity but the
   implementation has two distinct pause surfaces
   (`ModIntegrationPipeline._isRunning` for Apply-mutation safety,
   `GameLoop._paused` for tick advance). Resolution deferred to a
   future v1.6 cycle informed by M8+ load patterns. Falsifying
   artifacts: `MenuFlow_BeginEditing_PausesGameLoop` + 2 siblings.
   See §10 for full diagnosis.

All six interpretations explicitly registered in ROADMAP per
METHODOLOGY's "no improvisation" rule. None are silent code-driven
semantic drift.

**Verdict:** PASSED. Commit-prefix discipline holds (with one §10
finding — sundry audit-campaign sync commits without prefixes); §12
D-1 through D-7 byte-identical despite v1.4→v1.5 transition; six
deliberate interpretations registered with falsifiable artifacts and
escalation paths.

---

## §6 Sub-phase acceptance criteria coverage

`ROADMAP.md` §M7 section declares M7.1 through M7.5.B.2 closed with
explicit commit-list and test-class enumeration. Each sub-phase's
acceptance bullet maps to an identifiable artifact via §2 above.
Per-sub-phase summary:

### M7.1 — Pause/Resume + IsRunning

Commits: `a2ab761` (feat), `c964475` (test), `0606c43` (docs).
Surface: `ModIntegrationPipeline._isRunning` private bool, `Pause` /
`Resume` setters, `IsRunning` getter, `Apply` + `UnloadAll` paused-only
guards with verbatim §9.3 messages. Tests: `M71PauseResumeTests` (11).

### M7.2 — ALC unload chain steps 1–6 + §9.5.1 best-effort failure semantics

Commits: `2531ed7` (feat), `d68ba93` (test), `c3f5251` (docs).
Surface: `ModIntegrationPipeline.UnloadMod(modId)` returning
`IReadOnlyList<ValidationWarning>`, private `TryUnloadStep` helper
accumulating per-step warnings; `UnloadAll` refactored to delegate to
`UnloadMod` per active mod; `LoadedMod.Api` non-positional record
member retained from `Apply` step [4]. Tests: `M72UnloadChainTests` (13).

### M7.3 — UnloadMod step 7 + Phase 2 carried-debt closure

Commits: `9bed1a4` (feat), `46b4f33` (test), `1d43858` (docs).
Surface: `RunUnloadSteps1Through6AndCaptureAlc` non-inlined helper +
`TryStep7AlcVerification` non-inlined helper (both factored out for
DEBUG display-class hoisting safety per the persisted CODING_STANDARDS
finding `f4b2cb8`); `WeakReference` spin loop with mandatory
`GC.Collect → GC.WaitForPendingFinalizers → GC.Collect` bracket per v1.4
§9.5 step 7 clarification. Tests: `M73Step7Tests` (5) +
`M73Phase2DebtTests` (2 real-mod fixtures) + `ModUnloadAssertions`
helper.

### M7.4 — D-7 build-pipeline hotReload override

Commits: `5385fe5` (feat), `8d51a9d` (test), `4ff04f8` (docs).
Surface: `tools/DualFrontier.Mod.ManifestRewriter/` standalone CLI tool
(`System.Text.Json.Nodes`, BCL only); `mods/Directory.Build.targets`
MSBuild target gated on `<IsVanillaMod>true</IsVanillaMod>` AND
`$(Configuration)=='Release'`; `Fixture.VanillaMod_HotReloadOverride`
test fixture; integration tests via `dotnet build` subprocess. Tests:
`ManifestRewriterTests` (7) + `M74BuildPipelineTests` (2). New test
project `DualFrontier.Mod.ManifestRewriter.Tests` joins the solution.

### M7.5.A — ModMenuController + IModDiscoverer + GetActiveMods read API

Commits: `9c895fe` (feat), `4c648c6` (test), `198d948` (docs).
Surface: `ModMenuController` (internal sealed) — editing-session
lifecycle per §9.2 with §9.6 enforcement and AD #4 / AD #5 / AD #6
deliberate interpretations; `IModDiscoverer` + `DefaultModDiscoverer`
production implementation (best-effort directory scan);
`ModIntegrationPipeline.GetActiveMods()` public read API. Public DTOs:
`ActiveModInfo`, `DiscoveredModInfo`, `EditableModInfo`, `CommitResult`,
`ToggleResult`. Tests: `ModMenuControllerTests` (22) +
`DefaultModDiscovererTests` (4) + `PipelineGetActiveModsTests` (4).

### M7.5.B.1 — Production GameBootstrap integration

Commits: `4956a13` (feat), `94128be` (test), `b6b6d7e` (docs).
Surface: `GameBootstrap.CreateLoop(PresentationBridge, string modsRoot = "mods")`
returning new `internal sealed record GameContext(GameLoop Loop,
ModMenuController Controller)`; `coreSystems` array threaded both into
kernel scheduler and `ModRegistry.SetCoreSystems` per AD #5; default-paused
pipeline state preserved (bootstrap does NOT call Pause/Resume).
M-phase boundary discipline preserved (`git diff` against
`src/DualFrontier.Core` and `src/DualFrontier.Contracts` returns
empty). Tests: `GameBootstrapIntegrationTests` (7).

### M7.5.B.2 — Godot mod-menu UI scene

Commits: `9a75b75` (feat), `6ecee53` (test), `b519804` (docs).
Surface: `src/DualFrontier.Presentation/UI/ModMenuPanel.cs` (originally
`Godot.Control`, post-housekeeping H5 converted to `Godot.CanvasLayer`),
F10 hotkey via `GameRoot._UnhandledInput`, modal full-screen overlay
with empty-mods/ honest-signal label, per-mod row layout with §9.6
hot-reload-disabled tooltip. Visual styling reuses `Palette` /
`MakePanelStyle` / `ColonyPanel.MakeLabel`. Tests:
`MenuFlow_OpenCommitClose_LeavesEditingFalse`,
`MenuFlow_OpenCancelClose_LeavesEditingFalse`,
`MenuFlow_OpenWithoutCommitOrCancel_StaysEditing` (3 controller-level
integration tests; AD #13 — no automated UI-widget tests in this project).

### Housekeeping passes (post-M-cycle-commit closures)

6 housekeeping passes ran during M7's implementation cycle. Per the
project's operating principle ("data exists or it doesn't"), none of
them resolved an open M-cycle acceptance bullet — each fixed a separate
issue surfaced during F5 manual verification or audit work. Each pass
followed three-commit discipline (`fix/feat → test → docs`).

1. **TICK display wiring** (`1b16e9e` fix, `2192188` test, `3d800d2`
   docs). Phase 4 housekeeping bug surfaced when M7.5.B.1's bootstrap
   integration tests were authored — `TickAdvancedCommand` was published
   from `GameLoop` but `RenderCommandDispatcher` had no case for it, so
   `GameHUD.SetTick` was never called and `ColonyPanel._tickLabel`
   stayed frozen at `TICK: 0`. Closes by adding the dispatcher case +
   one integration test (`CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge`)
   asserting bridge publishes. Test count: 415 → 416.

2. **TickScheduler.ShouldRun race fix** (`e0b0ecf` fix, `52d6d3f` test,
   `700cbc0` docs). Pre-Phase-1 latent race in
   `TickScheduler.ShouldRun`'s tick-rate cache — `Dictionary<Type, int>`
   populated from inside `Parallel.ForEach`, despite the class's own
   "not thread-safe" claim. Surfaced by H1's new integration test at
   ~60% flake rate over a 5-run sample. Resolved by swapping
   `Dictionary` → `ConcurrentDictionary` and adding
   `TickSchedulerThreadSafetyTests` as a unit-level regression guard
   (32 distinct synthetic systems × 500 iterations; pre-fix fails 9/10
   invocations; post-fix passes 10/10). Test count: 416 → 417.

3. **Real pawn data** (`9141bd6` feat, `659a64a` test, `74d2eed` docs).
   Pre-existing UI lies removed: hardcoded Warhammer-flavoured pawn
   names array, hash-derived role label, hash-derived skill bars
   ignoring `SkillsComponent`. Resolved by introducing
   `IdentityComponent` + `RandomPawnFactory` (deterministic seed-42
   generation, 10 colonists, all 13 `SkillKind` values populated per
   pawn), extending `PawnStateChangedEvent` / `PawnStateCommand` with
   `TopSkills`, rewriting `PawnStateReporterSystem` and `PawnDetail`'s
   SKILLS section to read real component data. Eight dead files deleted
   in the same pass. Tests: `RandomPawnFactoryTests` (8) +
   `GameBootstrapIntegrationTests` (+3 facts). Test count: 417 → 428.
   M-phase boundary preserved.

4. **NeedsSystem decay direction** (`ee12108` fix, `7ea038c` test,
   `9d7b7f6` docs). Decay-toward-0 was a placeholder lie — implied
   automatic recovery while no module currently closes needs. Per the
   operating principle, the honest behaviour for an incomplete
   simulation is accumulating deficit until Phase 5 lands the recovery
   loop. Four `NeedsSystem` decay-line signs flipped from `-` to `+`;
   class-level XML doc rewritten. New `NeedsAccumulationTests` (3) lock
   the post-honesty-pass contract. Test count: 428 → 431.

5. **ModMenuPanel position + assets gitignore** (`5f0b4f5` fix,
   `805b882` chore, `0b11d1f` docs). ModMenuPanel converted from
   `Control` to `CanvasLayer` (Layer 20) to fix the modal misposition
   surfaced during F5 verification — Phase 4 UI pattern conformance
   (`GameHUD` Layer 10 + Control children pattern). Internal `_root`
   Control wraps the dim overlay + centered panel. Companion: extracted
   Kenney UI packs and Cinzel font folders added to `.gitignore` (source
   `.zip` files remain the in-git SoT). Test count: 431 → 434 (no
   change attributable to this pass; the +3 came from M7.5.B.2's three
   menu-flow tests landed before the position fix). The triple is the
   only one without a direct test surface — Godot widget visual changes
   are verified by F5 manual run per AD #13 of M7.5.B.2.

6. **Menu actually pauses simulation** (`9f87536` fix, `d8a448f` test,
   `110ad61` docs). F5 manual verification surfaced that the menu did
   NOT actually pause the simulation thread — TICK counter advanced
   ~250 ticks while menu held open across two screenshots. Root cause:
   two independent pause flags (`ModIntegrationPipeline._isRunning` for
   Apply-mutation safety, `GameLoop._paused` for tick advance) where
   `ModMenuController.BeginEditing` only toggled the former. Fix wires
   `OnEditingBegan` / `OnEditingEnded` hooks on the controller from
   `GameBootstrap.CreateLoop` so the orchestration layer calls
   `GameLoop.SetPaused` in lockstep with `pipeline.Pause`/`Resume`.
   Failed-commit stays paused per M7.5.A AD #4. Tests:
   `MenuFlow_BeginEditing_PausesGameLoop`,
   `MenuFlow_Cancel_ResumesGameLoop`,
   `MenuFlow_CommitSuccess_ResumesGameLoop`. Test count: 434 → 437.
   **§9.2 v1.6 ratification candidate flagged** — see §10.

**Verdict:** PASSED. Every M-cycle sub-phase acceptance bullet maps to
an identifiable file:line and test. All 6 housekeeping passes
documented as auxiliary closures with their three-commit footprints
preserved.

---

## §7 Carried debts forward

The M7 closure absorbs the most significant carried debt in the
M-cycle so far: **Phase 2 WeakReference unload tests, hard-required at
M7 per §10.4, are now CLOSED via M7.3.** The other carried debts
remain tracked forward to their absorbing M-phases. Three new in-batch
carried items are registered, none latent.

| Debt / forward reference | Origin | Status at M7 closure | Documentation |
|---|---|---|---|
| **WeakReference unload tests** | Phase 2 (closed at 11/11 isolation tests, with the unload tests on backlog); M5 / M6 closures listed as «hard requirement at M7» | **CLOSED via M7.3** | `M73Phase2DebtTests` (2 real-mod fixtures via `pipeline.Apply` — `Fixture.RegularMod_DependedOn` minimal surface, `Fixture.RegularMod_ReplacesCombat` with system registration + bridge replacement; both close §10.4 hard-required `WeakReference.IsAlive == false` within timeout, zero flakes across 3 consecutive runs at closure time per §11.4 invariant). [ROADMAP.md](../ROADMAP.md) Phase 2 carried-debt note explicitly marks closure. |
| `SocialSystem`, `SkillSystem` `[BridgeImplementation(Phase = 3)]` stubs | Phase 3 (closed at 1/1 integration test) | Unchanged through M7 | Tracked forward to M10.C (`Vanilla.Pawn` mod) per [ROADMAP.md](../ROADMAP.md) §M10. M6 closure boundary guards (`Phase5BridgeAnnotationsTests.SocialSystem_RemainsProtected`, `…SkillSystem_RemainsProtected`) still pass at HEAD. |
| M3.4 — CI Roslyn analyzer (D-2 hybrid completion) | M3 closure (deferred sub-phase) | Unchanged through M7 (`⏸ Deferred`) | Tracked forward to first external (non-vanilla) mod author per [ROADMAP.md](../ROADMAP.md) M3.4 row. |
| §7.5 fifth scenario (M6→M7 hand-off) | M6 closure note (`ROADMAP.md:332` post-update) | **Resolved via outcome (B) Structural verification** | See §2 above. The disjoint M7.2 (`UnloadMod` removes from `_activeMods`) + M6.2 (`CollectReplacedFqns` parameter-driven) tests collectively exercise the structural property. No joint end-to-end test added — honest reporting; M9 (`Vanilla.Combat`) is the first realistic load pattern that would justify one. |

### New in-batch carried items (M7 → forward)

#### §9.2 v1.6 ratification candidate (registered, not resolved)

**Diagnosis.** §9.2 step 1 reads «User opens the mod menu. The menu
calls `ModIntegrationPipeline.Pause()` which sets the scheduler's run
flag to false.» This wording reads as a single pause action against a
single «scheduler run flag.» The implementation has **two distinct
pause surfaces**:

1. `ModIntegrationPipeline._isRunning` — gates `Apply` / `UnloadAll`
   mutation safety per §9.3.
2. `GameLoop._paused` — gates the simulation thread's tick advance
   loop, separate from Apply-mutation safety.

The orchestration-layer fix (`GameBootstrap.CreateLoop` registers
`OnEditingBegan` / `OnEditingEnded` hooks on the controller, calling
`GameLoop.SetPaused` in lockstep with `pipeline.Pause` / `Resume`)
correctly wires both flags from a single user-visible action — the
menu opens, both flags flip, the simulation pauses both at the
mutation-safety level and at the tick-advance level. **The current
code is correct given the architecture.** What's at issue is the spec
wording, not the implementation.

**Three resolution options** (all v1.6 territory; none required for
M7 closure):

- **Option A — refactor toward unified pause contract.** Lift
  `_paused` into a shared `ISimulationGate` interface owned by the
  pipeline; `GameLoop` becomes a consumer. Pro: single conceptual
  pause surface. Con: introduces a Core-touching contract change
  (M-phase boundary discipline cost).
- **Option B — explicitly enumerate the two surfaces in §9.2.** Spec
  text reads «menu pauses (a) the pipeline's run flag for Apply
  safety and (b) the simulation tick loop for visible state freeze».
  Pro: matches reality; minimal-touch. Con: reveals an asymmetry that
  may confuse mod authors reading §9.2 in isolation.
- **Option C — document orchestration as canonical.** Spec text
  notes that the pause surface is composite and the orchestration
  layer (`GameBootstrap.CreateLoop` hooks) is the canonical wiring
  point. Pro: matches the M7.5.B.2-housekeeping fix verbatim. Con:
  ties spec to a specific orchestration shape (less portable to
  different bootstrap topologies).

**Recommendation:** defer resolution to the v1.6 cycle informed by
M8+ load patterns. M9 (`Vanilla.Combat`) introduces real
gameplay-driven load batches; the load patterns observed there will
inform which option scales best. Resolution-time decision is not
load-bearing for M8 vanilla skeleton bootstrap (which just exercises
the existing two-flag composite).

**Falsifying artifacts:** `MenuFlow_BeginEditing_PausesGameLoop`,
`MenuFlow_Cancel_ResumesGameLoop`, `MenuFlow_CommitSuccess_ResumesGameLoop`
(`GameBootstrapIntegrationTests`).

#### `.sln` build verification gap

**Diagnosis.** Top-level `dotnet build DualFrontier.sln` does NOT
compile `DualFrontier.Presentation` (Godot project, not in `.sln`).
M7.5.B.2 closure report flagged this. The `dotnet test` flow remains
unaffected (test projects do not depend on `Presentation`).

**Risk:** CI / automated checks miss `Presentation` regressions until
F5 verification or a Godot-aware build runs.

**Resolution options** (all minor housekeeping):
- Add `DualFrontier.Presentation` to `DualFrontier.sln` (verify the
  Godot SDK headless build cooperates).
- Document required dual-build invocation in `CODING_STANDARDS.md` /
  `METHODOLOGY.md`.
- Add a wrapper script that invokes both the .sln build and the Godot
  headless build.

**Target:** small housekeeping post-closure or M8 prep.

#### UI redesign with Kenney UI pack + Cinzel font

**Diagnosis.** Phase 5 / M10 polish item — replace the placeholder
Palette-driven minimal styling with proper themed UI using extracted
assets at `assets/kenney_ui-pack/`, `assets/kenney_ui-pack-rpg-expansion/`,
`assets/cinzel/`. Foundation prep landed in housekeeping H5
(`.gitignore` now ignores extracted folders, source `.zip` files remain
the in-git SoT).

**Decisions pending:**
- Which Kenney pack as the base.
- Theme system breadth (one Godot `Theme` resource vs per-widget
  styling).
- Font application rules (titles only? body text? labels?).
- Pawn sprite replacement (Phase 5 dependency on `PawnVisual` +
  render-command extensions).
- TileMapRenderer asset replacement (Phase 5 dependency).
- Asset extraction & import workflow.

**Target:** separate larger brief informed by user vision/mockup.

### Phase 5 backlog (extended)

Per the housekeeping commits' closure notes, the Phase 5 backlog is
now categorized into 3 subsections in [ROADMAP.md](../ROADMAP.md)
Backlog: gameplay completeness (recovery loop, NeedsComponent
semantic rename, job loop, MoodBreakEvent handlers,
MovementSystem job-aware wandering); UI ↔ data wiring (Health,
Faction, Race, Social, Mana/Ether, role/class concept, full skills
grid, UI redesign with Kenney+Cinzel); combat / magic / scaffolding
(combat command dispatch re-creation, stub UI re-creation,
UIUpdateCommand re-creation, pawn sprite replacement). Each entry
names its target M-phase.

**Verdict:** PASSED. Phase 2 carried debt CLOSED via M7.3 (the most
significant single closure of M-cycle). Phase 3 → M10.C unchanged. M3.4
unchanged. §7.5 fifth scenario M6→M7 hand-off resolved via outcome (B).
Three new in-batch carried items registered with full diagnosis and
forward path: §9.2 v1.6 candidate (deferred to v1.6 cycle), `.sln`
gap (small housekeeping), UI redesign (Phase 5 / M10). Phase 5 backlog
listed in 3 subsections.

---

## §8 Ready-for-M8 readiness

M8 is "Vanilla mod skeletons" ([ROADMAP.md](../ROADMAP.md) §M8). Per
`MOD_OS_ARCHITECTURE` §11.1, M8 consumes strategic lock 4 (vanilla
split into slices) and produces the directory structure + empty
manifests for the five vanilla mods (`Vanilla.Core`, `Vanilla.Combat`,
`Vanilla.Magic`, `Vanilla.Inventory`, `Vanilla.Pawn`, `Vanilla.World`).

| M8 dependency | M0–M7 surface | Status |
|---|---|---|
| Vanilla mod skeleton scaffolding (5 mods per spec) | M0 ModLoader + M2 IModApi + M4 SharedModLoadContext + M7.4 build pipeline pattern (`<IsVanillaMod>true</IsVanillaMod>` MSBuild opt-in, `mods/Directory.Build.targets` ready) | ✓ ready |
| Vanilla mod manifests with v2 schema | M1 + M5.2 v2 path; `ManifestParser` accepts v2 manifests with `kind`, `apiVersion`, `hotReload`, `dependencies` (with versions), `replaces`, `capabilities` | ✓ ready |
| Mod menu integration for vanilla mod toggle | M7.5.A controller + M7.5.B.1 production wiring + M7.5.B.2 Godot UI scene + post-fix menu-pauses-simulation behavior (housekeeping H6) | ✓ ready |
| Build pipeline support for vanilla mods | M7.4 (`<IsVanillaMod>` MSBuild opt-in, `mods/Directory.Build.targets`, standalone `ManifestRewriter` tool, integration tests via `dotnet build` subprocess); `Fixture.VanillaMod_HotReloadOverride` is the structural template M8 follows | ✓ ready |
| Hot-reload disabled per-mod for vanilla in shipped builds | §9.6 + M7.4 D-7 build-pipeline override + M7.5.A AD #5 (first-load is not a reload — vanilla mods load once at session start) | ✓ ready |
| Documentation surface for mod authors | `MODDING.md`, `MOD_PIPELINE.md` exist per [docs/README.md](../README.md) Development section. M8 should verify currency before vanilla skeletons land. | ✓ ready (currency check is M8 prep) |
| ContractValidator readiness for vanilla load batches | Eight-phase pipeline (A→B→E→G→C→D→F→H) handles vanilla skeleton manifests without new phase. Vanilla.Core as `kind: "shared"` exercises Phase F (shared-mod compliance) which is unchanged at M7. | ✓ ready |
| Hot-reload `false` shipped-build verification | M7.4 `M74BuildPipelineTests` 2 integration tests verify Release builds rewrite the bin manifest while leaving source unchanged; Debug builds leave both at `hotReload: true`. M8 vanilla manifests will use the same `<IsVanillaMod>true</IsVanillaMod>` opt-in. | ✓ ready |

**Verdict:** PASSED. No M7 surface change blocks M8. The bootstrap
(M7.5.B.1) + menu controller (M7.5.A) + Godot UI (M7.5.B.2) +
build-pipeline override (M7.4) + ALC unload chain (M7.1–M7.3) chain is
end-to-end ready for vanilla skeleton load patterns. The eight-phase
validator handles vanilla load batches without modification. The
M-phase boundary discipline (no `DualFrontier.Core` touched by any
Mod-OS Migration phase) was preserved through M7 — verified via
`git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts`
returning empty for the M-cycle commits (the H2 `TickScheduler.ShouldRun`
race fix is the only Core-touching commit in the M7 batch — registered
explicitly as housekeeping, not M-cycle).

---

## §9 Surgical fixes applied

This closure session applied surgical fixes for stale cross-document
version references introduced by the mid-batch v1.5 ratification
(`3f00c2a`) overtaking three deliberate-interpretation footers anchored
on a hypothetical-future v1.5 ratification clause, plus four stale
`v1.4 LOCKED` references that were not synchronized when the v1.5
ratification landed. Each fix is in a single combined surgical-fix
commit per the M3-M6 precedent (one commit per coherent fix bucket).

| File | Line | Pre-fix | Post-fix | Justification |
|---|---|---|---|---|
| `docs/README.md` | 28 | `**v1.4 LOCKED.**` | `**v1.5 LOCKED.**` | Stale reference; spec is v1.5 LOCKED at HEAD per `MOD_OS_ARCHITECTURE.md:8`. Audit Pass 4 §14 Tier 3 #2 / Pass 5 H-1b confirmed. |
| `docs/ROADMAP.md` | 3 | `MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) v1.4 LOCKED;` | `MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) v1.5 LOCKED;` | Stale preamble. Audit Pass 2 §13 OOS #10 / Pass 3 OOS #1 / Pass 4 §14 Tier 3 #3 / Pass 5 H-1c confirmed. |
| `docs/ROADMAP.md` | 157 | `derived from MOD_OS_ARCHITECTURE v1.4 §11.` | `derived from MOD_OS_ARCHITECTURE v1.5 §11.` | Stale M-section preamble. Internal consistency — same spec. |
| `docs/ROADMAP.md` | 516 | `[MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) — v1.4 LOCKED specification driving M1–M10.` | `[MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) — v1.5 LOCKED specification driving M1–M10.` | Stale see-also. Audit Pass 4 §14 Tier 3 #5 / Pass 5 H-1e confirmed. |
| `docs/ROADMAP.md` | 390 | `the resolution is a v1.5 ratification rather than relocating the flag` | `the resolution is a v1.6 ratification rather than relocating the flag into the scheduler (v1.5 was consumed by audit campaign Pass 2).` | Hypothetical-future clause overtaken by mid-batch v1.5 ratification (`3f00c2a`). The next available ratification slot is v1.6. Also fixes a broken cross-reference to `./M6_CLOSURE_REVIEW.md` (now at `./audit/M6_CLOSURE_REVIEW.md`) in the same paragraph. |
| `docs/ROADMAP.md` | 392 | `the resolution is a v1.5 ratification rather than silent reordering.` | `the resolution is a v1.6 ratification rather than silent reordering (v1.5 was consumed by audit campaign Pass 2).` | Same justification as #5 above. |
| `docs/ROADMAP.md` | 394 | `the resolution is a v1.5 ratification rather than silent semantic drift.` | `the resolution is a v1.6 ratification rather than silent semantic drift (v1.5 was consumed by audit campaign Pass 2).` | Same justification as #5 above. |

**Total:** 6 surgical-fix edits across 2 files, applied in a single
combined commit per M3-M6 precedent. No surgical fix touches
`docs/architecture/MOD_OS_ARCHITECTURE.md` (spec stays at v1.5 LOCKED through M7
per the brief's out-of-scope clause) or any code under `src/`.

The closure-review cross-references in `docs/audit/AUDIT_CAMPAIGN_PLAN.md`
and the `docs/prompts/M7*.md` / `HOUSEKEEPING_*.md` files (broken by
the Pass 4 closure-review move from `docs/` to `docs/audit/`) are
**explicitly NOT fixed in this session** per the brief's "keep them
minimal — closure session is verification, not cleanup" clause and the
fact that those files are frozen historical artifacts. Registered as
§10 follow-up under "broken closure-review cross-references in M7-era
prompts" — target: separate housekeeping pass.

---

## §10 Items requiring follow-up

Five sub-sections registered for visibility, none of which gate M7
closure or M8 readiness:

### §10.1 Empirical observation — contradiction discovery rate, fifth datapoint

Per the M5 / M6 closure precedent (M5_CLOSURE_REVIEW.md §10,
M6_CLOSURE_REVIEW.md §10), the M-cycle empirical contradiction-discovery
sequence extends with M7's fifth datapoint:

| Implementation phase | Spec version at start | Spec version at end | Latent contradictions discovered |
|---|---|---|---|
| M3 closure review | v1.1 | v1.2 | **1** — §3.6 hybrid enforcement formulation contradicted §4.2/§4.3 implementation; ratified in v1.2 changelog |
| M4 closure review | v1.2 | v1.3 | **1** — §2.2 `entryAssembly`/`entryType` "ignored for kind=shared" contradicted §5.2 step 1; ratified in v1.3 changelog |
| M5 closure review | v1.3 | v1.3 (unchanged) | **0** — `git diff dba17c7..HEAD` returned zero output |
| M6 closure review | v1.3 | v1.3 (unchanged) | **0** — `git diff 32f6f04..HEAD` returned zero output |
| **M7 closure review** | **v1.3** | **v1.5** (mid-batch v1.4 + v1.5) | **1** — §9.2 wording-vs-implementation gap surfaced via F5 manual verification post-M7.5.B.2 closure (housekeeping H6 diagnosis); registered as v1.6 ratification candidate |

**Hypothesis status: preserved.** The forward claim from
M6_CLOSURE_REVIEW §10 read «M7 (which exercises §9 lifecycle + §10.4
WeakReference unload) discovers ≤ 1 latent contradiction» — M7 produced
exactly 1. The M5_CLOSURE_REVIEW §10 cumulative falsification target
read «M7 alone produces spec-amendment count ≥ 2 OR cumulative count
across M3-M10 exceeds 4» — M7 produced 1 (not ≥ 2); cumulative through
M7 is 3 (not > 4). Both forward claims hold.

**Note on the v1.4 + v1.5 mid-batch ratifications**: neither counts as
an M-cycle "latent contradiction discovered." Both are non-strategic
clarifications surfaced by independent processes (the M7 pre-flight
readiness review for v1.4; the audit campaign Pass 2 for v1.5), not by
M7 sub-phase implementation. The §12 D-1..D-7 byte-identity check
above confirms neither ratification re-opened a strategic decision.
The M7 datapoint = 1 refers specifically to the §9.2 menu-pause gap
surfaced post-M7.5.B.2 by F5 manual verification.

**Forward claim for M8–M10:** cumulative count across M3–M10 must
remain ≤ 4. With 3 used (M3=1, M4=1, M7=1), M8–M10 must collectively
contribute ≤ 1 latent contradiction. Falsifiable forward target.

### §10.2 §9.2 v1.6 ratification candidate

Full diagnosis and three resolution options registered in §7 above.
Resolution deferred to a future v1.6 ratification cycle informed by M8+
load patterns. The M7.5.B.2 second-follow-up housekeeping pass
(commits `9f87536`, `d8a448f`, `110ad61`) shipped the orchestration-
layer wiring that closes the implementation gap; the spec wording is
the only outstanding item for the v1.6 cycle.

Recommended resolution timing: post-M9 (Vanilla.Combat). M9 is the
first M-phase that ships a real-gameplay load batch; the load patterns
observed there will inform whether Option A (refactor toward unified
contract), Option B (enumerate two surfaces), or Option C (document
orchestration as canonical) scales best.

### §10.3 `.sln` build verification gap

Full diagnosis registered in §7 above. Top-level
`dotnet build DualFrontier.sln` does NOT compile
`DualFrontier.Presentation` (Godot project, not in `.sln`). The
`dotnet test` flow remains unaffected.

**Risk:** CI / automated checks miss `Presentation` regressions until
F5 verification or a Godot-aware build runs.

**Resolution options:** add to `.sln` (verify Godot SDK headless build
cooperates); or document required dual-build invocation in
`CODING_STANDARDS.md` / `METHODOLOGY.md`; or wrapper script.

**Target:** small housekeeping post-closure or M8 prep.

### §10.4 UI redesign with Kenney UI pack + Cinzel font

Full decision list registered in §7 above. Foundation prep landed in
housekeeping H5. Target: separate larger brief informed by user
vision/mockup.

### §10.5 Operating principle elevation candidate

The «data exists or it doesn't» principle was explicitly invoked at
least 5 times during M7 housekeeping:
1. Real pawn data (H3) — replaced placeholder names / roles / skills
   with real component data.
2. Needs decay direction (H4) — flipped decay sign so existing
   incomplete-simulation behavior is honest deficit accumulation
   rather than false automatic recovery.
3. ModMenuPanel position (H5) — fixed visual misposition so the modal
   is actually centered (the prior position made the modal effectively
   unreachable for the user, which the unit tests did not surface).
4. Menu pauses simulation (H6) — fixed two-flag composite so the user-
   visible "pause" state matches the user-visible "menu open" state.
5. Assets gitignore (H5 companion) — extracted Kenney + Cinzel folders
   are derived state, not in-git SoT (source `.zip` files are SoT).

Each application surfaced a placeholder lie that needed structural
fix, not a removal-of-claim.

**Forward claim:** future cycles will continue surfacing applications
of this principle. The principle has demonstrated value across 5
M7-batch passes; promotion from "user-stated convention" to documented
project value in `METHODOLOGY.md` (or `docs/README.md`) is now
empirically justified.

**Target:** small documentation commit, separate from this closure.

### §10.6 Unprefixed audit-campaign sync commits

Three commits in the M7 batch lack conventional METHODOLOGY §7.3
scope prefixes:
- `6e9c433` — `Update AUDIT_PASS_2_SPEC_CODE.md`
- `d1c1338` — `Pass 1 baseline: 1d43858 (M7.3 closure) Current: 6e9c433…`
- `c3e88ac` — `Add M7.3–M7.5 docs prompts`
- `1ab757f` — `Add *.zip to .gitignore`
- `cc9b7a5` — `UI: ModMenuPanel CanvasLayer fix & housekeeping docs`

These are active-development snapshots during the audit campaign
intermediate state, prompt-set additions, and a sundry ignore-mini-pass.
They are doc-only or config-only and preserve test count by
construction.

**Target:** future METHODOLOGY discipline — adopt
`docs(audit-sync)` / `chore(repo)` / `docs(prompts)` as conventional
prefixes for these patterns in future M-cycles. Not retroactive —
historical commit messages are frozen by definition.

### §10.7 Broken closure-review cross-references in M7-era prompts

The closure review files moved from `docs/` to `docs/audit/` during
the audit campaign (Pass 4 cleanup). M7-era prompt files
(`docs/prompts/M74_BUILD_PIPELINE_OVERRIDE.md`,
`M75A_MOD_MENU_CONTROLLER.md`, `M75B1_BOOTSTRAP_INTEGRATION.md`,
`M75B2_GODOT_UI_SCENE.md`, `M7_HOUSEKEEPING_TICK_DISPLAY.md`,
`M7_CLOSURE.md`) reference closure reviews via `./M6_CLOSURE_REVIEW.md`
which now resolves to a non-existent path. Similarly,
`docs/audit/AUDIT_CAMPAIGN_PLAN.md` references closure reviews via
`../M6_CLOSURE_REVIEW.md` which now resolves to a non-existent path
(the file is now in the same directory).

These prompts are frozen historical artifacts — they were authored at
the time the closure reviews were at the `docs/` location, and post-hoc
cross-reference rewriting would alter audit-trail content. The brief's
"keep surgical fixes minimal — closure session is verification, not
cleanup" clause applies.

**Target:** separate housekeeping pass — automated relative-path
sweep across `docs/prompts/` and `docs/audit/` rewriting
`./M[3-6]_CLOSURE_REVIEW.md` → `../audit/M[3-6]_CLOSURE_REVIEW.md` (in
prompts) and `../M[3-6]_CLOSURE_REVIEW.md` → `./M[3-6]_CLOSURE_REVIEW.md`
(in audit). The fix is mechanical and doesn't surface in active
navigation (the prompt files are frozen post-execution).

---

## §11 Verification end-state

- **Build:** 0 warnings, 0 errors at `dotnet build DualFrontier.sln`.
- **Tests:** 437/437 passing across 5 test projects (Persistence 4 /
  Systems 19 / Modding 346 / Core 61 / ManifestRewriter 7).
- **Three-commit invariant:** holds at every commit
  `b504813..110ad61` per M-sub-phase / housekeeping pass triple.
  Per-sub-phase progression: 338 (M6 baseline) → 349 (M7.1) → 362
  (M7.2) → 369 (M7.3) → 378 (M7.4) → 408 (M7.5.A) → 415 (M7.5.B.1) →
  434 (M7.5.B.2) → 437 (post-housekeeping H6).
- **Spec ↔ code ↔ test triple consistency:** 19 in-batch acceptance
  bullets across M7.1–M7.5.B.2; CENTRAL menu-driven hot-reload
  demonstration is `MenuFlow_BeginEditing_PausesGameLoop` +
  `MenuFlow_CommitSuccess_ResumesGameLoop` (post-housekeeping H6),
  which exercises §9.2 step 1 + step 4 end-to-end at the
  `GameContext` level.
- **Cross-document consistency:** `MOD_OS_ARCHITECTURE` v1.5 LOCKED ↔
  `ROADMAP` `Updated: 2026-05-03` `437/437` M7 ✅ ↔ `docs/README` v1.5
  LOCKED (post §9 surgical fixes).
- **Stale-reference sweep:** zero hits in active-navigation context
  post §9 surgical fixes. Historical-progression text in ROADMAP
  engine snapshot and frozen audit-trail in M3–M6 closure reviews
  preserved by design.
- **Methodology compliance:** 50 commits, all with substantive bodies;
  conventional prefixes on 45 (5 sundry / audit-sync commits unprefixed
  per §10.6). **D-1 through D-7 byte-identical** between
  M6 closure baseline `c7210ca` and HEAD despite v1.4 → v1.5 transition
  (strongest possible falsifiable signal that mid-batch ratifications
  are non-strategic). Six deliberate-interpretation registrations with
  falsifiable artifacts and explicit escalation paths.
- **§9.2 wording-vs-implementation gap surfaced via F5 verification
  (registered as v1.6 candidate, not silent).**
- **Sub-phase acceptance:** M7.1 through M7.5.B.2 all mapped; 6
  housekeeping closures all present.
- **Carried debts forward:** **Phase 2 CLOSED via M7.3** (the most
  significant single closure of M-cycle so far); Phase 3 → M10.C
  unchanged; M3.4 → first external mod author unchanged; §7.5 fifth
  scenario M6→M7 hand-off resolved via outcome (B) Structural
  verification; new items (§9.2 v1.6 candidate, `.sln` gap, UI
  redesign, operating principle elevation, unprefixed-commits style
  finding, broken-xref cleanup target).
- **Ready-for-M8:** PASSED across all dependencies.
- **Surgical fixes applied this pass:** 6 edits across 2 files in 1
  combined commit (per M3-M6 precedent: one commit per coherent fix
  bucket).
- **Items needing follow-up:** 0 blocking. 7 observation sub-sections
  registered in §10 (empirical fifth datapoint, §9.2 v1.6 candidate,
  `.sln` gap, UI redesign, operating principle elevation, unprefixed
  audit-sync commits, broken closure-review xrefs in prompts).

M7 closes with one v1.6 candidate registered. M8 (Vanilla skeletons)
is unblocked.

---

## §12 See also

- [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) v1.5 LOCKED — the
  specification this review verifies. v1.4 + v1.5 ratifications during
  M7 batch; D-1 through D-7 byte-identical to M6 closure state.
- [ROADMAP](../ROADMAP.md) — M7 closure status, 7 sub-phase entries,
  6 housekeeping closures, 3 deliberate-interpretation footers.
- [METHODOLOGY](../METHODOLOGY.md) — §2.4 atomic phase review, §7.3
  process discipline, §11.4 stop-condition discipline.
- [M3_CLOSURE_REVIEW](./M3_CLOSURE_REVIEW.md) — closure-report format
  origin (eight-check structure).
- [M4_CLOSURE_REVIEW](./M4_CLOSURE_REVIEW.md) — multi-document
  consistency precedent.
- [M5_CLOSURE_REVIEW](./M5_CLOSURE_REVIEW.md) — empirical
  contradiction-rate hypothesis registration.
- [M6_CLOSURE_REVIEW](./M6_CLOSURE_REVIEW.md) — most recent precedent;
  this document mirrors its eight-check structure and extends the
  empirical contradiction-discovery rate observation registered in
  M5/M6 closure reviews with its first nonzero post-M4 datapoint.
- [UI_REVIEW_PRE_M75B2](./UI_REVIEW_PRE_M75B2.md) — pre-M7.5.B.2 audit
  document referenced by M7.5.B.2 prompt.
- [AUDIT_REPORT](./AUDIT_REPORT.md) — campaign GREEN-with-debt verdict
  closing Pass 1–5; confirms project state at M7.3 closure point matches
  v1.4 LOCKED + ROADMAP without significant drift.
