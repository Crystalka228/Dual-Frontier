---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-PHASE_BETA_BRIEF
category: D
tier: 4
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-PHASE_BETA_BRIEF
---
---
register_id: DOC-D-A_PRIME_9_1_PHASE_BETA_BRIEF
project: Dual Frontier
category: D
tier: 4
lifecycle: Draft (-> LOCKED on Crystalka ratification -> EXECUTED at cascade closure)
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-01'
content_language: en
authored_by: Claude Opus (deliberation session, Phase beta prep)
basis: PHASE BETA RECON REPORT 2026-07-01 (R1-R8)
supersedes_task: A'.9.1 brief §4.2 Task 7 (self-contradictory stub-dry-run; replaced by §11 here)
---

# A'.9.1 PHASE beta CLEANUP-PHASE -- Execution Brief

Single-pass execution (contingent on the Q-L-1 gate at §11). Executor: **Claude Code, flagship model, LOCAL on Skarlet**. Repository: `D:\Colony_Simulator\Colony_Simulator`. No other tree involved.

**Brief-integration notice** (first phase-execution application of `TESTING_STRATEGY` v2.0.0 §6): this brief CITES standing law by anchor, never restating it. Binding by citation: commit-body structure + marker law -> `CODING_STANDARDS` §8/§5; DFK-WAIVER suppression law -> `CODING_STANDARDS` §5.3; truth law (no enforcement verb without an on-disk enforcer) -> the v2.0.0 docs; census pins + brief-integration + analyzer-test convention -> `TESTING_STRATEGY` §4/§6/§3.5; mutability license + `Skeleton revisions` form -> `RESERVED_SURFACE_MUTABILITY` §3/§5; session closure -> `METHODOLOGY` v1.13.0; the 17-rule registry + Phase-gamma targets -> `ANALYZER_RULES` §4 (v0.2.1); canonical detection narratives + DFK019.A=Warning -> `K_CLOSURE_REPORT` §7.2. **Anti-pattern rule:** a conflict between this brief and any standing doc means THIS BRIEF IS WRONG -- halt and escalate.

**The recon substitutes for a survey wave.** The 2026-07-01 Phase beta recon (R1-R8) is the code-truth substrate; the executor receives it as input and verifies against code before writing (recon is the work order, code is truth). This brief does not re-run the survey.

---

## 1. Mission [CORE]

Populate real detection logic into the 17 analyzer stubs, prove each with tests, materialize the census meta-tests, evaluate the Q-L-1 adaptive gate on the true violation count, and triage the real violations -- all with rules held at **Info severity, Unshipped** (severity promotion + Shipped transition + `.editorconfig` priming are Phase gamma, gated by F-12, explicitly OUT of scope here). After this cascade the analyzer detects; it does not yet enforce (Info does not fail the `TreatWarningsAsErrors` build).

| # | Deliverable | Action |
|---|---|---|
| D1 | Test harness upgrade: `PlaceholderTests` -> `CSharpAnalyzerVerifier<TAnalyzer, XUnitVerifier>` scaffolding | rewrite |
| D2 | 17 rule detection implementations, in 4 complexity-tier batches (§7) | new logic in existing stub files |
| D3 | Per-rule analyzer tests (>=1 positive + >=1 negative each; ~50-80 total) | new |
| D4 | Census meta-tests (~7): reserved-surface 34/13 exact-pin, 5 marker families, DFK-WAIVER=0 | new |
| D5 | DFK002 **federated interop-surface model** (§8) -- the ratified architectural refinement | encoded in DFK002 detection |
| D6 | Real-violation triage (fix / DFK-WAIVER / refine), dominated by the ManagedBusBridge cluster | code + waivers |
| D7 | Q-L-1 gate evaluation at Phase beta closure + per-rule violation-inventory artifact (§11) | new artifact |
| D8 | Citation-drift fix: stubs + `src/Directory.Build.props` "brief §10.3/§10.5" -> `ANALYZER_RULES §10.5` + `ROADMAP` Analyzer track | annotation-surface edit |
| D9 | REGISTER closure + validate + render | governance |

## 2. Established facts (recon digest -- re-verify markers) [CORE]

- HEAD `02be616` post-Godot; tree clean; register **2.18** / 275 docs / 41 EVT / 41 REQ / 17 CAPA / 14 RISK. **[re-verify at Phase 0; mismatch -> H1]**
- 17 stubs verified: Architecture 9 (DFK003, DFK003.1, DFK004, DFK005, DFK007, DFK011, DFK013, DFK016, DFK017), NativeBoundary 5 (DFK001, DFK002, DFK007.1, DFK015.1, DFK019.A), Discipline 3 (DF999, DFL025-A, DFL025-B). All `DiagnosticSeverity.Info`, `isEnabledByDefault: true`, register zero actions (0 `context.Register*` across all 17 -- the zero-diagnostic proof), all in `AnalyzerReleases.Unshipped.md`, `Shipped.md` empty.
- Per-rule intended semantics + detection kind (SYNTAX / FQN-STRING per Lesson #N19 / SEMANTIC): recon R2 table is the specification -- the executor implements from it, cross-checked against `ANALYZER_RULES` §4 and `K_CLOSURE` §7.2 detection narratives.
- Harness present: `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit` 1.1.2 + explicit `.CSharp.Workspaces` 5.3.0 (CPM-pin wins MEF over transitive 1.0.1); CodeAnalysis 5.3.0; xUnit 2.9.2; SDK 10.0.204; analyzer `netstandard2.0`, `IsRoslynComponent`, `EnforceExtendedAnalyzerRules`. Pattern to extend: `CSharpAnalyzerVerifier<T>`.
- Baseline: analyzer + full solution build **0W/0E** at Info (Info diagnostics do not fail `TreatWarningsAsErrors`); analyzer wired to all 12 src projects via `src/Directory.Build.props`. **[re-verify baseline at Phase 0; regression -> H2]**
- **HARD pins verified:** `[ReservedStub` = 34/13 (matches `TESTING_STRATEGY` §4.1 / `CODING_STANDARDS` §5.1); DFK-WAIVER `#pragma disable DFK|DFL|DF9` = 0; `[SuppressMessage]` = 0.
- Violation-surface (recon R3, UPPER-BOUND proxy): raw ~1,771 is non-actionable (49% sanctioned Vulkan, 16% canonical Core.Interop, rest legitimate language use). **Refined lever = DFK002 alone.** Predicted Q-L-1 branch: **<=80 continue** (Scenario B, DFK002 federated) most probable; 80-150 worst realistic; >150 not realistic.
- Phantom projects: brief §2.3 (and the A'.9.1 brief) name `Application.Scheduler / Application.Modding / Core.Scheduling` -- these DO NOT EXIST (Lesson #N18 recurrence). On-disk truth = 12 projects: AI, Application, Components, Contracts, Core, Core.Interop, Crypto.Future, Events, Launcher, Persistence, Runtime, Systems. Analyzer wiring is already current.
- F-12 (S2 OPEN): DFK019.A Phase-gamma Warning vs the old blanket-Error reading -- gates **Phase gamma**, not this cascade. DFK016 retained-at-alpha (Q-L-16), Phase-gamma Warning.

## 3. Phase 0 [CORE]

1. **Verify** §2 re-verify set (HEAD `02be616`, clean tree, register 2.18, 34/13 pin, 0 waivers, baseline 0W/0E). Mismatch -> **H1**.
2. **Baseline gates**: analyzer build + full solution build (Release) + `dotnet test tests/DualFrontier.Analyzers.Tests`. Record 0W/0E + placeholder 1/1 as the regression anchor. Full-suite fast test sweep recorded as the closure comparison (the pre-existing F-10 stress shape is not a halt). Regression -> **H2**.
3. **REGISTER enum read** (Lesson #N14): empirical category/tier/lifecycle enums + `DOC-`/`EVT-` shapes from `REGISTER.yaml`.
4. **Mandatory reads**: the Phase beta recon report (full); `ANALYZER_RULES` §4 + §10.5 + §12; `K_CLOSURE_REPORT` §7 (detection narratives) + §2.22 (K-L19); `CODING_STANDARDS` §5.3 (DFK-WAIVER) + §8; `TESTING_STRATEGY` §3.5 + §4 + §6; `RESERVED_SURFACE_MUTABILITY` §3-§5; the A'.9.1 brief §7. Confirm each read before the writer wave spawns.
5. Validate-fold protocol (VALIDATION_REPORT.md into the running commit); `-Sync` forbidden; render once at the end; executor never pushes.

## 4. Topology [CORE]

```
Orchestrator
 |- Phase 0 (serial)
 |- D1 harness upgrade (serial, C2) -- the wave's test scaffolding must exist first
 |- Writer wave -- 4 agents, parallel, DISJOINT rule+test files by complexity tier:
 |    W1 SYNTAX      : DF999, DFK002, DFK005, DFK016, DFL025-B
 |    W2 FQN-STRING  : DFL025-A, DFK011, DFK003.1, DFK019.A
 |    W3 SEM-Arch    : DFK003, DFK004, DFK007, DFK013, DFK017
 |    W4 SEM-NB      : DFK001, DFK007.1, DFK015.1
 |- Checkpoint C-W (truth-law + per-rule test-pass + build-still-0W/0E-at-Info + census pins)
 |- BARRIER (all detection must be landed before the count is meaningful)
 `- Serial closure (orchestrator only): C7 census meta-tests, C8 gate-eval + inventory,
    C9 triage, C10 citation-drift, C11 REGISTER, C12 render
```

**Hard rules:** only the orchestrator runs `git add`/`commit` (atomic discipline is incompatible with parallel committers -- writers draft rule+test files in worktree isolation, never stage). Rule `.cs` files and test classes are one-per-rule -> the fan-out is clean. **`AnalyzerReleases.Unshipped.md` is NOT mutated in Phase beta** (all 17 already listed at Info; severity/Shipped moves are Phase gamma) -- so there is no shared-file write contention. `.editorconfig` is NOT touched (Phase gamma). The DFK002 sanctioned-surface list (§8) is authored once by W1 as part of DFK002; if DFK001 (W4) needs it, W4 reads the same §8 definition -- coordinate through this brief, not a shared edit.

## 5. Wave R -- survey [KIND: phase-execution]
None -- the 2026-07-01 recon is the survey (see the brief-integration notice). Its R2 table is the per-rule specification and its R3 the calibration.

## 6. Checkpoints [CORE]

**C-W (after the writer wave):**
1. **Truth law** -- no test or doc claims coverage/enforcement absent an on-disk artifact; ANALYZER_RULES stays at its v0.2.1 stub-truth wording until Phase gamma (Phase beta does NOT rewrite §4 to say "enforced" -- detection exists, enforcement is Info-only; the honest phrasing is "detects at Info").
2. **Per-rule test-pass** -- every rule has >=1 passing positive (fires on the violation) and >=1 passing negative (silent on compliant code); `dotnet test` green.
3. **Build still 0W/0E** -- detection landed at Info must not fail the build (Info is non-failing); a Warning/Error escaping here means a severity was wrongly set -> return to the writer.
4. **Census pins** -- `[ReservedStub` still 34/13 (HARD); DFK-WAIVER count is whatever triage produced, each waiver carrying its §5.3 authority citation (verified at C9).
Violations -> return to the owning writer once; unresolvable without an architectural decision -> **H6**.

## 7. Writer specifications [CORE]

Global: each rule's detection is implemented from recon R2's intended-semantics + detection-kind, cross-checked against `ANALYZER_RULES` §4 and the `K_CLOSURE` §7.2 narrative for that rule. **Detection via canonical FQN strings, not CLR type references** (Lesson #N19 -- the analyzer csproj deliberately has no `ProjectReference` to Contracts; a rule that needs a type identity matches its fully-qualified display string, e.g. `symbol.ToDisplayString() == "DualFrontier.Contracts.Analyzer.ReservedStubAttribute"`). Severity stays **Info** for every rule (Phase gamma promotes). Each rule keeps its existing `DiagnosticDescriptor` id; only the `Initialize` body gains `context.Register*` actions. Per-rule tests use `CSharpAnalyzerVerifier<T>` with inline source fixtures.

Test obligation per `TESTING_STRATEGY` §6.1: for each rule the brief carries the named-test list (convention: `{RuleId}_Fires_On_{ViolationCase}` positive, `{RuleId}_Silent_On_{CompliantCase}` negative), layer = analyzer-test (§3.5), coverage anchor = the rule's intended-semantics line, count delta recorded at closure.

### W1 -- SYNTAX tier (cheap node walks)
- **DF999** -- flag `GlobalSuppressions.cs` files / `[assembly: SuppressMessage]`. Syntax: attribute-list walk for assembly-target `SuppressMessage` + a compilation-level check for a `GlobalSuppressions.cs` file. Baseline real = 0 (recon), so the negative fixture is the codebase norm; the positive is a synthetic `[assembly: SuppressMessage(...)]`.
- **DFK002** -- P/Invoke (`DllImportAttribute` / `LibraryImportAttribute` by FQN) whose containing symbol's namespace is NOT under a sanctioned interop surface (§8). This is the federated model. Positive: a `[DllImport]` in a non-sanctioned namespace; negatives: `[LibraryImport]` in `Runtime.Native.Vulkan`, `[DllImport]` in `Core.Interop`.
- **DFK005** -- extra bootstrap entry points diverging from the declarative `bootstrap_graph` (canonical `GameBootstrap` present; violation = additional `class *Bootstrap` entry). Syntax + a name/shape check.
- **DFK016** -- hardcoded pipeline-depth literal (1/2/3) where `PipelineSlotInterop.DefaultDepth`/`.MaxDepth` is the sanctioned source. Syntax literal check near pipeline-slot usage.
- **DFL025-B** -- standalone test vs a reserved-stub module not using `[Fact(Skip="...")]`. Syntax attribute check on test methods. Baseline real = 0.

### W2 -- FQN-STRING tier (Lesson #N19 canonical)
- **DFL025-A** -- test class invoking a `[ReservedStub]`-tagged type without `[Trait("Category","ReservedStub")]`. The canonical FQN `DualFrontier.Contracts.Analyzer.ReservedStubAttribute` is documented verbatim in the stub; match it. Baseline real = 0 (no stub-touching tests exist per `TESTING_STRATEGY` §5.3) -- positive is synthetic.
- **DFK011** -- `new ManagedWorld(...)` in production / repeated `NativeWorld` instantiation / shadow world. `ManagedWorld` FQN match outside the test namespace. Recon: `ManagedWorld` = 0 on disk (retired A'.7) -> real 0; positive synthetic.
- **DFK003.1** -- storage-bridge facade allocating/copying/mutating native storage; cross-mod `ManagedStore<T>`; Path-beta persistence; API bypass. `ManagedStore<T>` FQN + the bypass shapes.
- **DFK019.A** -- Vulkan <1.3 API / vendor extensions / alternate graphics API, STATIC surface only (runtime tier = DFK019.B, deferred). Match the non-1.3 FQN set (`OpenGL`/`DirectX`/`D3D*`/`Metal`/pre-1.3 Vulkan version symbols); the 871-occurrence sanctioned Vulkan-1.3 surface must be SILENT (recon: real ~0). Severity Info now; Phase-gamma target Warning per K_CLOSURE §7.2 (do NOT set Warning here -- F-12).

### W3 -- SEMANTIC tier, Architecture
- **DFK003** -- managed allocation of ECS-shaped storage / ownership transfer; struct+`[ManagedStorage]` or class-`IComponent`-without-`[ManagedStorage]`; cross-path bypass. Needs SemanticModel; compile-time isolation already enforced (A'.6) so real is small.
- **DFK004** -- implicit type-ID derivation (`typeof().FullName.GetHashCode()` etc.) bypassing `ComponentTypeRegistry`. Semantic; `typeof` alone (137) is overwhelmingly legitimate -- flag only the hash-derivation-as-type-id shape.
- **DFK007** -- span-protocol violation (mutation through read span, write outside batch, storage retained/copied out). **Costliest** -- span lifetime/dataflow. Scope the detection tightly to the storage-span protocol; do not flag general `Span<>` use (207 occ, ~all legitimate).
- **DFK013** -- `SystemBase` subclass without `[WakeOn*]`/`[TickRate]`. Semantic subclass enumeration; the codebase is thoroughly annotated (39 `[TickRate]` + 2 `[WakeOn]` over 31 subclasses), recon real = 0 (the lone proxy hit is a doc-comment example -- must be SILENT). Severity Info; Phase-gamma Warning.
- **DFK017** -- display multi-layer composition: cross-layer / out-of-order draws / alternate surface. Semantic layer-order analysis around `LayerType`.

### W4 -- SEMANTIC tier, NativeBoundary
- **DFK001** -- managed bridge bypassing the canonical P/Invoke surface / unsanctioned interop (native C++20 is outside Roslyn per S-LOCK-2, so this is the managed-side complement of DFK002). Semantic + the §8 sanctioned-surface definition.
- **DFK007.1** -- GPU pipeline-slot access bypassing `PipelineSlotInterop.ReadSlotTail` (direct slot indexing). Semantic; `ReadSlotTail` is the sanctioned call.
- **DFK015.1** -- managed concurrency bypassing the 3-tier mutex facade (raw `lock`/`Monitor`/`Mutex`/`Semaphore` where native-owned synchronization is mandated). Semantic; many managed-only locks are legitimate -- flag only those crossing into native-owned territory.

## 8. DFK002 federated interop-surface model [KIND: phase-execution -- ratified architectural machinery]

**Ratified 2026-07-01.** DFK002's definition of "canonical interop surface" is refined from single (`Core.Interop` only) to **federated, domain-bounded**: P/Invoke (`DllImport`/`LibraryImport`) is COMPLIANT iff its containing type's namespace is under one of the sanctioned interop namespaces:

- `DualFrontier.Core.Interop` -- the C++20 kernel boundary (K-L1 / K-L3).
- `DualFrontier.Runtime.Native.*` -- the native runtime boundary: `Runtime.Native.Vulkan` (GPU substrate, K-L19; `VkApi.cs` 86 P/Invoke) and `Runtime.Native.Win32` (Launcher OS surface; `Win32Api.cs` ~16).

Rationale (code-truth-wins, not a crutch): DFK002 was authored before the post-Godot native runtime existed; its single-surface definition drifted from the real architecture. The refinement aligns the rule with code truth. A `<NoWarn>` would be the crutch (forbidden, §5.3); forcing VkApi/Win32Api into `Core.Interop` would be architecturally WORSE (Vulkan and windowing belong to Runtime, not the kernel-interop boundary -- consolidation destroys cohesion). The federated boundary is the coherent model. Encode the sanctioned list in DFK002's detection (FQN-namespace prefix check).

**The ManagedBusBridge cluster (the one genuine violation):** `src/DualFrontier.Application/Bus/ManagedBusBridge.cs` carries 13 `DllImport` in the Application layer -- outside every sanctioned surface, so DFK002 fires. Triage (D6, at C9): **preferred** -- relocate its P/Invoke into a `Runtime.Native.Bus` surface (extends the federated set to a 4th native namespace; the coherent home for managed<->native bus marshalling), IF the relocation is a clean move without architectural ripple beyond the file. **Fallback** -- if the bridge is genuinely Application-coupled, a DFK-WAIVER with a sanctioned-architectural-exception citation (§5.3, citing the bus design). **Escalate (H6)** if the relocation would ripple into the bus contract. Record the chosen resolution in the triage commit and the closure.

## 9. S-LOCK invariants [CORE]

- **S-LOCK-4 preserved**: the 17-rule set is the analyzer's own-rule surface; Phase beta populates detection without adding or removing a rule. No rule ID changes (descriptor ID is the contract, `RESERVED_SURFACE_MUTABILITY` §3).
- **New structural invariants (become census meta-tests, §10):** the `[ReservedStub` 34/13 exact-pin and the DFK-WAIVER baseline are enforced by meta-tests (`TESTING_STRATEGY` §4.4: every S-LOCK names its verifying artifact) -- these are the S-LOCKs this cascade adds a verifying artifact for.

## 10. Census discipline [CORE]

Materialize the `TESTING_STRATEGY` §4 meta-tests as part of the buildout (§4.1-§4.3 say they land with Phase beta):
- **Reserved-surface exact-pin**: assert `[ReservedStub` application sites in `src/` (excluding the definition file) == **34** across 13 files, verbatim expression `rg -t cs -o '\[ReservedStub' src -g '!**/ReservedStubAttribute.cs'`. EXACT, not monotonic (HARD pin). A legitimate future change updates the pin in the same commit; this cascade must LEAVE it at 34/13 (Phase beta adds no reserved surface) -- movement here is a defect.
- **Five marker-family baselines** (SOFT): stub / deferred / TODO / "Phase 6" / "not yet" doc-tag counts per `TESTING_STRATEGY` §4.2 -- recorded as advisory baselines; Phase beta is code-detection, not comment churn, so movement is not expected (record any as a census-delta, not a finding).
- **DFK-WAIVER baseline**: 0 at Phase 0; at closure == the number triage produced, each waiver carrying a resolvable §5.3 authority citation. The meta-test asserts every `DFK-WAIVER(` marker has a matching `#pragma disable` in scope and an authority token.

## 11. Q-L-1 adaptive gate evaluation [CORE -- supersedes A'.9.1 Task 7]

Task 7 as originally authored (build with stubs -> dry-run enumeration) is self-contradictory: stubs emit zero, so the dry-run yields 0. **Corrected mechanism:** the gate is evaluated at **Phase beta closure**, on the ACTUAL Info-diagnostic count produced once all detection is landed.

At the BARRIER (after the writer wave + census meta-tests), C8:
1. Run the analyzer over `src/` and enumerate the real Info diagnostics per rule (the count the stubs could never produce). Record a **per-rule violation-inventory artifact** (`docs/reports/PHASE_BETA_VIOLATION_INVENTORY.yaml`: rule | count | file:line list | proposed triage) -- this is the A'.9.1 §7 required inventory and the gate evidence.
2. `violation_count` = total real Info diagnostics on `src/` paths. Apply Q-L-1 (brief §7.4 verbatim):
   - **<= 80 -> continue** this cascade through triage (C9) to Phase delta closure. **Pre-committed expected path** (recon Scenario B: DFK002 federated -> VkApi/Win32Api compliant -> ManagedBusBridge 13 + residuals ~10-32 = ~23-45).
   - **80 < count <= 150 -> HALT to Crystalka** (hybrid split decision) -- do not proceed to triage without ratification.
   - **> 150 -> split** (not predicted): close this cascade with the Phase beta subset, author A'.9.1b for cleanup + Phase gamma. Escalate before splitting.
3. The pre-commit does NOT skip the barrier -- the real count is measured and recorded even when it lands <=80 as expected; the inventory artifact is produced regardless.

## 12. Commit plan [CORE]

| # | Subject | Content |
|---|---|---|
| C1 | `governance(analyzer): enroll Phase beta brief + validation checkpoint` | brief + VALIDATION_REPORT |
| C2 | `test(analyzer): upgrade harness to CSharpAnalyzerVerifier scaffolding` | D1 (build + placeholder-replacement gate) |
| C3 | `feat(analyzer): implement SYNTAX-tier detection (DF999, DFK002, DFK005, DFK016, DFL025-B) + tests` | W1 (incl. §8 federated surface list) |
| C4 | `feat(analyzer): implement FQN-string detection (DFL025-A, DFK011, DFK003.1, DFK019.A) + tests` | W2 |
| C5 | `feat(analyzer): implement semantic Architecture detection (DFK003, DFK004, DFK007, DFK013, DFK017) + tests` | W3 |
| C6 | `feat(analyzer): implement semantic NativeBoundary detection (DFK001, DFK007.1, DFK015.1) + tests` | W4 |
| C7 | `test(analyzer): census meta-tests (reserved-surface 34/13, marker families, DFK-WAIVER=0)` | D4 |
| C8 | `chore(analyzer): Q-L-1 gate evaluation + violation inventory` | D7 (barrier; inventory artifact; branch determination) |
| C9 | `refactor(analyzer): real-violation triage (DFK002 ManagedBusBridge + residuals)` | D6 (relocate/waiver/refine per §8) |
| C10 | `docs(analyzer): citation-drift fix -- brief §10.3/§10.5 -> ANALYZER_RULES §10.5 + ROADMAP` | D8 |
| C11 | `governance(register): Phase beta REGISTER closure (2.18 -> 2.19)` | D9 + validate folded |
| C12 | `governance(register): render regeneration + header backfill` | render + Option-B backfill |

Commit count is intended-form; a rule needing isolation may split from its batch commit, and if the gate lands 80-150 the cascade halts at C8 (C9-C12 deferred to the ratified path). Record deviations in the closure. Commit subjects use `feat`/`test`/`refactor`/`chore`/`docs`/`governance` -- verify these against `CODING_STANDARDS` §8.1 scope vocabulary at Phase 0 (the `src`/`native` scope gap is a known open finding; use a codified scope or the closest valid one and record the choice).

## 13. REGISTER cascade (C11) [CORE]

Empirical enum shapes only (Phase 0.3). Enroll this brief (DOC-D -> EXECUTED at closure). Enroll the violation-inventory artifact if it warrants a register_id (assess -- a report under `docs/reports/` per the DOC-C/E convention). Bumps: `ANALYZER_RULES` -> PATCH only if its §12 citation or stub-truth wording is touched (it should NOT be rewritten to "enforced" -- detection is Info; if untouched, no bump). No new PENDING-COMMIT beyond the header self-reference (backfilled C12). EVT `EVT-PHASE_BETA-CLOSURE` (41 -> 42) with C1-C10 real hashes. register_version 2.18 -> 2.19. Validate exit 0 (**H3**); fix only within the empirical enum vocabulary (**H5**).

## 14. Halt conditions (H-series) [CORE]

- **H1** base-state mismatch (§3.1).
- **H2** build/test regression vs the Phase 0 baseline (a Warning/Error where only Info was intended is this class).
- **H3** validate nonzero.
- **H5** a REGISTER enum value is needed that Phase 0 vocabulary lacks -- escalate, never invent.
- **H6** truth-law unsatisfiable / an architectural decision required -- specifically: the ManagedBusBridge relocation rippling into the bus contract (§8); or a rule whose correct detection cannot be expressed without a design change.
- **H-gate** the Q-L-1 count lands **80 < count <= 150** (halt to Crystalka) or **> 150** (escalate before splitting) -- §11.
- **H-severity** any rule set to Warning/Error in Phase beta -- severity promotion is Phase gamma (F-12); Phase beta is Info-only.
- Standing rails: no pushes; no `-Sync` outside C11; no history rewrite; `AnalyzerReleases.Shipped.md` / `.editorconfig` untouched (Phase gamma).

On halt: stop, report state verbatim, await Crystalka; in-session re-confirmation before resuming is expected.

## 15. Closure protocol & report [CORE]

Execute `METHODOLOGY` v1.13.0 session closure. Report (chat): commits table (hash | subject); versions table (register 2.18 -> 2.19); **the Q-L-1 gate result** (the real per-rule count, the branch taken, the inventory artifact path); the triage table (each real violation -> fix/waiver/refine, ManagedBusBridge resolution named); census pins (reserved-surface 34/13 EXACT held, DFK-WAIVER final count with citations, marker-family deltas if any); the test census (per-rule positive+negative count, meta-test count, the named-test lists per `TESTING_STRATEGY` §6.1); gates table (baseline 0W/0E -> closure 0W/0E-at-Info, full test sweep vs baseline); `Skeleton revisions` consolidated (deviations from intended forms -- incl. the phantom-project recap correction and citation-drift targets); self-attestation (no pushes; single render; no `-Sync`; Shipped.md/.editorconfig untouched; severities all still Info; no rule ID changed); operator manual checklist (push; ratify lifecycle; **F-12 before Phase gamma**; the standing F-queue F-4/F-9, F-7, F-10, F-13, and the commit-scope-vocabulary reconciliation).

## 16. Out of scope [CORE]

Phase gamma (severity promotion Info->Error/Warning/Suggestion, `AnalyzerReleases` Unshipped->Shipped, `.editorconfig` `dotnet_diagnostic.DFK*` priming) -- gated by **F-12** (Crystalka ratifies DFK019.A Warning first) | DFK019.B runtime-tier detection (deferred) | KERNEL rewrites (F-4/F-9) | the native C++20 kernel (outside Roslyn, S-LOCK-2) | doc_role schema / hybrid register (F-2/F-13) | branch pruning (F-11) | the two failing stress tests (F-10 -- baseline) | PIPELINE_METRICS falsified forward-claim (methodology-corpus queue) | NIH | pushes.

---

*Authored 2026-07-01 from PHASE BETA RECON (R1-R8). Federated DFK002 + Task 7 supersession + Info-only scope ratified by Crystalka 2026-07-01. Ratification: Crystalka. Bez kostylei.*

**End of PHASE_BETA_BRIEF v1.0**
