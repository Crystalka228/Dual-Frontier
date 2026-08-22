---
register_id: DOC-D-ID_B_ENTITY_VERSIONS_BRIEF
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
title: 'ID-B entity versions -- F-59 closure by the IAC section-2 versions view (identity family cascade B, NATIVE-ADDITIVE): df_world_acquire_versions/release pair over the native versions_ table, SpanLease/WriteBatch/SpanScope reconstruct TRUE versions, fabrication-site migration, EntityId.IsValid alignment both sides, DFK022 rule + K-L22 seated AUTHORED, governance housekeeping rider (templates + Special/ perimeter)'
authored_by: Claude Fable (architect session, identity-family charter)
basis: DOC-E-F59_F60_IDENTITY_RECON_REPORT R3 (architect-seat recon at ad4e353) + IDENTITY_AND_ABI_CONTRACT section 2 (the ratified design fund -- option 1 RECOMMENDED with normative-target signatures) + the ID-A closure (PR #50 merged, main 8e02a48) + ratified lean set L4/L5 (K-L22 AUTHORED seating, DFK022)
---

# ID_B_ENTITY_VERSIONS -- Execution Brief

This cascade is Cascade B of the F-59+F-60 identity-family charter (Cascade A = ID-A, EXECUTED,
PR #50 merged). It CLOSES **F-59 (S2)**: span and batch pair-iterators fabricate `Version = 0` —
correct for a never-recycled index, wrong for a recycled one — so generation validation collapses
to "the index was never recycled" (the C10/N-22 defect), and a stale id circulating after a
recycle is indistinguishable from the live entity. W3 made this urgent: `ISystemContext.
DestroyEntity` puts destruction (and therefore recycling) in MOD hands.

The fix is **already deliberated and ratified**: IAC §2 option 1 — an additive native **versions
view** (`df_world_acquire_versions` / `df_world_release_versions`) exposing the per-slot
`versions_` table read-only under an acquire/release discipline; every managed pair-iterator
reconstructs `new EntityId(index, versions[index])` — the TRUE generation; managed code never
again constructs a Version it did not receive from the world. That law is seated as **К-L22
(AUTHORED)** with analyzer rule **DFK022** enforcing it. Done means: the versions view ships end
to end, a recycled-index round trip proves stale ids are rejected AND fresh spans carry true
generations, every production fabrication site is migrated or waived, DFK022 is Error-enforcing,
and F-59 is CLOSED in the ledger.

Unlike ID-A this cascade is **NATIVE-ADDITIVE**: it adds two exports and one guard to the native
tree and touches nothing shipped — the ABI evolution law (IAC §3.1: adding an entry point is
additive; changing a shipped signature or semantics is forbidden) is the fence.

This brief also carries a small ratified **governance housekeeping rider** (C2): the authoring
templates land at their SCOPE_EXCLUSIONS-sanctioned paths, and the `Special/` reference perimeter
is formalized (operator rulings 2026-08-20).

Executor: Claude Code (flagship model), LOCAL on the operator's machine, repository
`D:\Colony_Simulator\Colony_Simulator` (GitHub `Crystalka228/Dual-Frontier`), branch off `main`
(= `8e02a48`, the PR #50 merge).

Brief-integration notice: this brief CITES standing law by anchor and does not restate it —
commit-body structure and marker law per CODING_STANDARDS; **push law per CODING_STANDARDS §8.4
(v3.0.0): at the closure boundary the executor pushes the WORK BRANCH and opens a PR against
`main`; pushing `main` and merging its own PR are forbidden; atomicity settled BEFORE the push
(§8.3)**; census pin law per TESTING_STRATEGY; mutability license and `Skeleton revisions` form
per RESERVED_SURFACE_MUTABILITY; session closure per METHODOLOGY; test invocation safety
(no-pipe law) per TESTING_STRATEGY §8. Anti-pattern rule: a conflict between this brief and any
standing document, or between this brief and the live code, means THE BRIEF IS WRONG — halt and
escalate; code-truth wins.

## 1. Mission [CORE]

Deliverables:

| #  | Artifact | Action | Version |
|----|----------|--------|---------|
| D1 | Native versions view | ADD `df_world_acquire_versions` + `df_world_release_versions` (df_capi.h + world.h/cpp + capi.cpp), own `active_version_views_` counter, `create_entity` REFUSES while a view is held (resize would invalidate the pointer); `entity_id.h::is_valid` aligned to `index > 0`; selftest extended; exports 207 -> 209 | -- |
| D2 | Managed true-version surface | `NativeMethods` P/Invoke pair; `SpanLease<T>` acquires the view for its lifetime and exposes `Versions` (indexed by ENTITY INDEX, not dense position); `SpanLease.Pairs`, the `WriteBatch<T>` enumerator, and `Sdk/SpanScope<T>.Pairs` reconstruct TRUE versions; `EntityId.IsValid => Index > 0` (Contracts) | Contracts PATCH (vs LIVE) |
| D3 | Fabrication-site migration | The ≈20 engine sites + `GameBootstrap.cs:241` (IAC §2 census = the work order) move to the versions idiom or a world-call form; `EntityEncoder.cs:85` WAIVED until A7 persistence | -- |
| D4 | DFK022 analyzer rule | NEW rule "entity-identity": flag `new EntityId(<expr>, <integer literal>)` outside `DualFrontier.Core.Interop` internals + test projects; Error, NativeBoundary category; EntityEncoder carries the waiver — **HARD pin DFK-WAIVER 2 -> 3, ratified by this brief** | -- |
| D5 | К-L22 seating + doc truth | KERNEL Part 0: NEW invariant row **К-L22 — Entity identity honesty**, AUTHORED (series state 22 -> 23 active; table rows 23 -> 24); IAC §2 rewritten proposed -> shipped (incl. §3.5 row anchor and the WRONG resize-protection parenthetical corrected); ECS §5 defect row resolved + teaching example; ANALYZER_RULES DFK022 row | per §12 |
| D6 | Housekeeping rider | `docs/methodology/BRIEF_TEMPLATE.md` + `RECON_KICKOFF_TEMPLATE.md` placed at their SCOPE_EXCLUSIONS-sanctioned paths (body-only, NOT enrolled — the exclusion rows exist for exactly this) with the template's stale "executor never pushes" rail fixed to §8.4 v3.0.0; `.gitignore` += `Special/` and `TestResults/`; `SCOPE_EXCLUSIONS.yaml` += `Special/**` row | -- |
| D7 | Proof tests | Recycled-index round trip (create -> destroy -> recycle -> stale id REJECTED at flush while the fresh span carries the TRUE generation); versions-view unit + selftest cases; DFK022 rule tests; IsValid corner sweep | -- |
| D8 | Closure | EVT append; ROADMAP write-back (**F-59 CLOSED**; identity family complete; tick-path root charter is the family's successor); brief -> EXECUTED; **push branch + open PR** | -- |

Sequencing: ID-B completes the identity family. It precedes W5 (mods will destroy entities at
scale; recycling without true generations is the corruption window) and is independent of the
F-60(a) tick-path charter.

## 2. Established facts [CORE]

Measured by architect line-reads at `ad4e353` (recon R3) and re-checked at `8e02a48` where noted.
(RV) facts must be re-verified at Phase 0; halting on mismatch.

1. (RV) Native version model (`world.cpp`): `versions_` per-slot table, init 0, doubling growth
   inside `create_entity` (:65-67); `next_index_ = 1` (`world.h:162`); `create_entity` returns
   `{index, versions_[index]}` — fresh never-recycled slot = version 0 (:57-72); `destroy_entity`
   increments the slot version BEFORE free-listing and THROWS while spans or batches are active
   (:84-93); `is_alive` = `index > 0 && version == versions_[index]` (:74-78). The ABA law
   (IAC §1 Note 1): a (Index, Version) pair is issued at most once per world lifetime.
2. (RV) **`create_entity` has NO active-spans guard** (:57-72) — creation while a component span
   is held is LEGAL today. Consequence for D1: IAC §2's parenthetical "the mutation-rejection
   counter already guarantees the table cannot resize while any span is active" is WRONG for
   creation — `create_entity` can resize `versions_` under a live component span. The versions
   view therefore brings its OWN guard (§7.1) and the IAC sentence is corrected at D5.
3. (RV) The silent-drop site: `WriteBatch::flush` skips commands whose reconstructed id fails
   `is_alive` — `continue`, no error (`world.cpp:436-461`); the destructor auto-flush discards
   even the applied count (:328-377). Managed `WriteBatch.Flush()` returns the count
   (`WriteBatch.cs:122-128`).
4. (RV) Fabrication sites, current state (all `Version = 0` post-W3-C5b): `SpanLease<T>.Pairs`
   (`SpanLease.cs` — doc :76-85 carries the W3 correction note and the K7-era deferral text, code
   fabricates 0); the `WriteBatch<T>` enumerator `Current` (`WriteBatch.cs:214-225` —
   reconstructs from `_lease.Indices`, so it is lease-backed and inherits D2's fix);
   `Sdk/SpanScope<T>.PairsEnumerator.Current` (`SpanScope.cs:103-109`; its property doc was
   corrected at ID-A `c0b0fb3` and points at THIS cascade). Engine-side census per IAC §2 (:83):
   ≈20 sites across nine systems + `GameBootstrap.cs:241` + `EntityEncoder.cs:85` — the §2 list
   is the work order; re-count at Phase 0.
5. (RV) `EntityId.IsValid => Index > 0 || Version > 0` (`src/DualFrontier.Contracts/Core/
   EntityId.cs:38`) and native `entity_id.h:14-16` carry the same flawed disjunction; `(0, v>0)`
   is "valid" managed-side and permanently dead native-side (verdict N38). Pack/unpack are honest
   (`entity_id.h:19-30`).
6. IAC §2 is the RATIFIED design fund: option 1 RECOMMENDED with normative-target C signatures
   (:93-104), option 2 rejected (ABI-breaking), option 3 (`EntityRef`) retained as endgame only;
   consequential amendments listed (:122-128) — teaching-site rewrites, the DFK-entity-identity
   rule (:125, its К-L20 pointer already corrected to К-L22 at ID-A `855739c`), the EntityEncoder
   waiver (:126), IsValid alignment (:128).
7. (RV) К-L series state (KERNEL :68): 22 active invariants, К-L21 RESERVED (Mod-API), table
   carries 23 canonical-text rows. **К-L22 is the next free row** — ratified seating target (lean
   L4, operator-ratified 2026-08-20).
8. (RV) Native export census: `grep -c "^DF_API"` over `native/DualFrontier.Core.Native/include/
   *.h` sums to **207** (df_capi.h 153 + pipeline_slot 18 + bus_native 15 + background_queue 8 +
   event_type_registry 5 + phase_compute 5 + mod_unload 3). D1 adds exactly 2 -> 209. Selftest
   case count: read the live figure at Phase 0 (EQ_A3 recorded 104).
9. (RV) HARD census pins at `8e02a48`: `Console.WriteLine` src = 2, DFK-WAIVER = 2 (both
   `RestrictedModApi.cs`), `[ReservedStub` = 34 sites / 13 files. This cascade RATIFIES
   DFK-WAIVER 2 -> 3 (the EntityEncoder waiver, D4); the other pins must not move.
10. (RV) Housekeeping preconditions: `docs/methodology/` contains NO *TEMPLATE* file; the
    SCOPE_EXCLUSIONS rows for `docs/methodology/BRIEF_TEMPLATE.md` + `RECON_KICKOFF_TEMPLATE.md`
    exist (prophylactic, DRAFTS_RATIFICATION H3) — placing body-only files at exactly those paths
    is sanctioned and does NOT enroll them; `SCOPE_EXCLUSIONS.yaml` is Tier 1 LOCKED
    (`tools/governance/SCOPE_EXCLUSIONS.yaml`, DOC-G-SCOPE_EXCLUSIONS); `.gitignore` has no
    `Special/` or `TestResults/` line (`*.zip` at :23 covers the two archives today). The
    authoritative template sources are inside `Special/files.zip` (extract to a scratch location,
    NEVER unpack inside the repo tree).
11. Gates at `8e02a48` (architect-measured at the ID-A verification, full 10-suite TRX run):
    full-sln **1276 passed / 0 failed / 5 skipped** (skips = F-10 family); `validate --armed`
    exit 0 (366 docs). Native baseline: build + selftest figures recorded at Phase 0.

## 3. Phase 0 -- preconditions and checkpoint [CORE]

Run serially before any code change.

1. **Verify the (RV) set** from §2 by direct reads and the verbatim greps. Mismatch -> HALT H1.
2. **Baseline gates**: full managed build + full-sln test run per DEVELOPMENT_HYGIENE and
   TESTING_STRATEGY §8 (no stdout pipes; TRX is truth). Expect 1276/0/5; record actuals. **Native
   baseline**: build `DualFrontier.Core.Native` (CMake, c++23preview pin per STACK_UPDATE) and
   run the native selftest; record the case count and PASS state. Closure must match-or-improve
   modulo tests this cascade adds -> HALT H2 on regression.
3. **Branch prep**: branch off `main` (`claude/id-b-entity-versions` or session-assigned). Push
   law §8.4 v3.0.0 as cited above.
4. **Validation checkpoint**: `dotnet run --project tools/DualFrontier.Governance -- validate
   --armed` exit 0 -> else HALT H3.
5. **Frontmatter-shape read** (Lesson #N14): FRAMEWORK 14.3/14.4/14.5 + live frontmatter of one
   LOCKED tier-2 doc + one `AUDIT_TRAIL.yaml` EVT as the append template. This brief persists
   `lifecycle: Draft`, flips to `EXECUTED` at closure.
6. **Create-under-span sweep (D1 precondition — decides H7)**: enumerate every production call
   path that can invoke `CreateEntity`/`df_world_create_entity` while a `SpanLease`/`SpanScope`/
   `WriteScope` is open in the same scope. Method: read every `CreateEntity` call site in `src/`
   and `mods/` (grep `CreateEntity(` and `EnsureSingleton`-class patterns) and confirm each runs
   with no lease held (the W3 Weather pattern disposes the span BEFORE creating — verify it
   still does). ZERO such paths -> D1's strict guard ships. ANY such path -> HALT H7 with the
   site list (the architect chooses between site fixes and a lazier view-acquisition design).
7. **IsValid corner sweep (decides H9)**: every `IsValid` consumer in `src/` + `mods/` — confirm
   none depends on `(Index == 0, Version > 0)` being valid. A dependent site -> HALT H9.
8. **Fabrication-site census**: re-run the IAC §2 (:83) site list against HEAD; record the exact
   set D3 will migrate (files:lines). New sites since the IAC census join the work order.
9. **Census pre-measure**: the five TESTING_STRATEGY §5.2 marker censuses BEFORE any
   comment-touching commit (canonical `--count-matches`; bi-script К-L in any grep).
10. **Mandatory reads**: this brief in full; recon report R3; IAC §1 Note 1 + §2 + §3.1/§3.5;
    `world.cpp` (entity lifecycle + WriteBatch), `world.h`, `entity_id.h`, `df_capi.h` (span
    acquire/release family :88-99 — the discipline the new pair mirrors), `capi.cpp` (span
    acquire/release wrappers + catch(...) convention), `selftest.cpp` (span + entity sections);
    `SpanLease.cs`, `WriteBatch.cs`, `Sdk/SpanScope.cs`, `SystemContextView.cs` (AcquireSpan +
    WriteScope), `EntityId.cs`, `EntityEncoder.cs`, `NativeMethods` (span P/Invoke shapes);
    the nine systems named in the IAC census + `GameBootstrap.cs`; the analyzer project layout +
    one existing DFK rule + its tests + the DFK-WAIVER mechanism (ANALYZER_RULES doc — locate at
    Phase 0); KERNEL Part 0 (:60-80 series state + К-L20/К-L21 rows as seating exemplars);
    `SCOPE_EXCLUSIONS.yaml` + FRAMEWORK §14 (for C2); CODING_STANDARDS §8.3/§8.4;
    METHODOLOGY closure protocol.
11. **Derived-fold protocol**: `sync` in EVERY frontmatter-touching commit; derived registers
    never hand-edited; `AUDIT_TRAIL.yaml` append-only.

## 4. Topology [CORE]

Single orchestrator, serial execution, no wave agents. Deep-but-narrow: native pair -> managed
surface -> site migration -> analyzer -> docs; every commit builds on the previous surface. Only
the orchestrator runs `git add/commit`.

## 5. Wave R -- survey agents

None (serial cascade; the Phase 0 sweeps ARE the survey).

## 6. Checkpoints [CORE]

Self-audit before C7 (docs commit): native fence re-check (§13 H8 — diff native/ shows ONLY the
D1 additive surface); census re-measure vs Phase 0 (§10); truth-law audit — every enforcement
claim written into KERNEL/IAC/ECS/ANALYZER_RULES names its on-disk artifact (DFK022 exists and is
Error-enforcing BEFORE К-L22's row cites it); citation form (anchors, no living-doc version pins,
no URL anchors).

## 7. Execution / writer specifications [CORE]

Intended forms below are mutable surface per RESERVED_SURFACE_MUTABILITY; deviations recorded as
`Skeleton revisions`.

### 7.1 D1 -- the native versions view (commit C3)

Normative-target signatures (IAC §2 :93-104; names bikesheddable):

```c
/* Same acquire/release discipline as component spans (df_capi.h:88-99).
 * Read-only view over the per-slot versions_ table, indexed by ENTITY INDEX,
 * not dense position. Valid until df_world_release_versions. */
DF_API int32_t df_world_acquire_versions(
    df_world_handle world,
    const int32_t** out_versions_ptr,
    int32_t*        out_count);
DF_API void     df_world_release_versions(df_world_handle world);
```

- **Own counter** `active_version_views_` (atomic, the `active_spans_` pattern) — NOT the span
  counter: component spans deliberately do not block creation, and must continue not to.
- Guards while `active_version_views_ > 0`: `create_entity` THROWS `logic_error` ("cannot create
  entities while a versions view is held — the versions table could resize"), REFUSE-NOT-FORCE
  per the EQ_A3 precedent; `destroy_entity`/`flush_destroyed` extend their existing guard to also
  check the view counter (a destroy mutates `versions_` content the view is reading).
  `out_count` = the table size at acquire (`versions_.size()`), NOT `entity_count`.
- The C-ABI wrappers follow the capi.cpp conventions verbatim: catch(...) -> status/0-sentinel,
  no exceptions across the boundary (IAC §3.4); release is void and tolerant.
- `entity_id.h::is_valid` -> `index > 0` (drop the `|| version > 0` arm) — rides this commit as
  the native half of D2's alignment; grep native for `is_valid` consumers first (expected: none
  load-bearing on the corner; a dependent site -> H9).
- Selftest additions: acquire/release round trip (count == table size, fresh entity's slot reads
  0, destroyed entity's slot reads incremented); create-while-held throws (via the C ABI: the
  wrapper returns 0/status — assert the refusal, then release and create successfully);
  destroy-while-held refused; double-release tolerated; exports pin 207 -> 209 asserted by the
  §10 census method.
- ABI evolution: ADDITIVE ONLY. No shipped signature or semantics changes (H8). MODULE.md export
  list updated.

### 7.2 D2 -- managed true-version surface (commit C4)

- `NativeMethods`: the P/Invoke pair, `[DllImport]` shape mirroring the span acquire family.
- `SpanLease<T>`: acquires the versions view at construction (after the component span), exposes
  `public ReadOnlySpan<int> Versions` — **indexed by ENTITY INDEX** (document loudly: it is NOT
  parallel to `Span`/`Indices`; consumers write `Versions[Indices[i]]`); `Dispose` releases the
  view then the span (reverse acquisition order), idempotent.
- `SpanLease.Pairs` `Current` -> `new EntityId(idx, Versions[idx])` per the IAC §2 sketch; the
  :76-85 doc block is rewritten — the K7-era deferral ENDS here ("the span ABI does not carry
  per-entity versions" stops being true); pre-declared SOFT-pin movement, §10.
- `WriteBatch<T>` enumerator (`WriteBatch.cs:214-225`) is lease-backed — its `Current` picks up
  the same `_lease.Versions[entityIndex]` reconstruction; its fabrication comment dies.
- `Sdk/SpanScope<T>`: ctor gains the versions span (from `SystemContextView.AcquireSpan` passing
  `lease.Versions`); `PairsEnumerator.Current` -> true version; the ID-A note ("true per-entity
  versions arrive with ... ID-B") resolves — reword, census-aware.
- `EntityId.IsValid => Index > 0` (`EntityId.cs:38`), doc updated to name the native mirror.
  ContractsVersion: PATCH vs LIVE (behavioral truth-fix, no surface change) — compute at Phase 0.
- Tests (D7 rows 1-3): recycled-index round trip at the interop layer — create A, destroy A,
  create B (recycles A's index with a higher version), acquire span + versions: the fresh pair
  iterator yields B's TRUE version; a batched write keyed on A's stale id is dropped (flush count
  short by one) while a write keyed on the iterator-yielded id LANDS. Extend
  `SpanWriteRoundTripTests` — this is the exact scenario W3 could not express. Plus: Versions
  indexing semantics (entity-index, not dense), dispose-release symmetry, create-after-dispose
  succeeds.

### 7.3 D3 -- fabrication-site migration (commit C5)

The Phase 0.8 census is the work order. Per site, in priority order: (a) a lease/scope is in
scope -> reconstruct via `Versions`; (b) no lease in scope but the id flows into a
world-answerable call -> use the world's own answer (`TryGetComponent`/`IsAlive` paths that
already carry true versions internally) or thread the lease; (c) genuinely unreachable from a
world surface -> escalate H6 (expected: none). `GameBootstrap.cs:241` follows the same triage.
`EntityEncoder.cs:85` is NOT migrated — it decodes persisted index ranges and version truth is
A7's jurisdiction; it receives the DFK-WAIVER (7.4) with a comment naming A7 as the retirement
trigger. No behavioral change is expected at any migrated site for never-recycled indices —
the migration makes recycled-index behavior CORRECT, not different-for-fresh (the D7 round trip
is the proof).

### 7.4 D4 -- DFK022 (commit C6)

- Rule per IAC §2 :125: flag `new EntityId(<expr>, <integer literal>)` — any integer literal, not
  just 0/1 — outside `DualFrontier.Core.Interop` internals and test projects. Error severity,
  NativeBoundary category, id **DFK022** (1:1 with К-L22 per the ANALYZER_RULES numbering law).
  `EntityId.Invalid`/`default` stay legal (no constructor literal).
- Waiver: `EntityEncoder.cs` site via the existing DFK-WAIVER pragma mechanism — **DFK-WAIVER
  HARD pin 2 -> 3, ratified here** (§10); the waiver comment names the A7 retirement trigger.
- Rule tests per the analyzer suite conventions: fires on fabrication (both literal forms),
  silent on the Versions idiom, silent on `default`/`Invalid`, waiver honored, internals exempt.
- ANALYZER_RULES doc row lands at C7 with the rule ALREADY on disk (truth law ordering).

### 7.5 D5 -- К-L22 + doc truth (commit C7)

- KERNEL_ARCHITECTURE Part 0, NEW row after К-L21 (seating exemplar: the К-L20 EQ_A2 form):
  **К-L22 — Entity identity honesty. AUTHORED at ID_B (2026-08-20).** Canonical text (final
  words, Appendix-A-grade): *"Span and batch ABI surfaces MUST surface true entity versions;
  managed code MUST NOT construct an EntityId whose Version it did not receive from the world.
  Version fabrication collapses generation validation to index-freshness and silently voids the
  ABA law (IAC §1 Note 1)."* Falsifiability: a shipped pair-iterator that fabricates; a new
  `new EntityId(<expr>, <literal>)` production site outside the waiver census; the versions view
  removed without a successor. Implementation artifacts: the D1 export pair, `SpanLease.
  Versions`, DFK022 (Error, enforcing), the DFK-WAIVER census row. Series state 22 -> 23 active
  (:68; table rows 23 -> 24); summary table row added. MINOR bump.
- IAC: §2 rewritten from proposal to SHIPPED law (cite the landed signatures + hashes; option
  history retained); the §2 parenthetical claiming the span counter prevents resize is CORRECTED
  to the shipped truth (the view's OWN counter + create guard — §2 fact 2); §3.5 pointer-table
  versions-view row filled with real anchors; §1 row 1 lifecycle column notes the view. MINOR.
- ECS §5 (fabricated-version defect): resolved note + the versions-view idiom as the canonical
  span example (§4); KERNEL §1.7 example likewise if it still shows index-only construction (RV).
- MODDING.md conditional: only if it teaches span iteration to modders (RV grep).

### 7.6 D6 -- housekeeping rider (commit C2)

- Extract the two templates from `Special/files.zip` OUTSIDE the repo (scratch), then write them
  to `docs/methodology/BRIEF_TEMPLATE.md` + `docs/methodology/RECON_KICKOFF_TEMPLATE.md` —
  body-only (NO frontmatter): those exact paths are SCOPE_EXCLUSIONS-excluded, so they are
  sanctioned un-governed meta-tooling (the prophylactic rows finally match reality). ONE content
  edit at placement, operator-ratified: the BRIEF_TEMPLATE §3 fixed text "The executor NEVER
  pushes -- pushes are the operator's manual step after closure" is replaced with the §8.4
  v3.0.0 rail ("the executor pushes the WORK BRANCH and opens a PR at the closure boundary;
  pushing `main` and merging its own PR are forbidden"); its §13 standing-rails line "no pushes
  to origin" updated to match. Everything else byte-faithful to the archive copies.
- `.gitignore`: `Special/` (the un-governed reference stash) + `TestResults/` (test-run litter).
- `SCOPE_EXCLUSIONS.yaml` (Tier 1 LOCKED — governance edit, sync+validate in THIS commit): append
  row `pattern: "Special/**"`, rationale exactly: *"Operator's un-governed design-reference stash
  (kept zipped; charters cite it, never enroll it) — gitignored AND scanner-excluded because
  sync/validate walk the filesystem, not git (operator-ruled 2026-08-20)."*

## 8. Kind-specific machinery [KIND: phase-execution]

Per-behaviour test obligation carried by §7.2/§7.4 (D7). The native half follows the additive-ABI
fence (§13 H8); the governance half of C2 follows the §12 discipline. No `[ReservedStub]` surface
is touched.

## 9. S-LOCK invariants [CORE]

**К-L22 is the cascade's seated invariant** (AUTHORED, not LOCKED — the LOCK ride belongs to a
later К-series closure, the К-L20 precedent), enforced structurally by DFK022 (Error) + the
DFK-WAIVER census row. No other S-LOCK is added.

## 10. Census discipline [CORE]

- HARD pins: `Console.WriteLine` src = 2 and `[ReservedStub` 34/13 — must not move (H10).
  **DFK-WAIVER 2 -> 3 is the ONE ratified HARD-pin movement** (the EntityEncoder waiver);
  the closure records the exact new site. Native exports: 207 -> **209**, method verbatim:
  `grep -c "^DF_API" native/DualFrontier.Core.Native/include/*.h` (sum).
- SOFT pins (TODO 132/51, deferred 89/55 baselines): C4/C5 comment rewrites RESOLVE deferral
  language (`SpanLease.cs:76-85` K7-deferral, `SpanScope.cs` ID-B pointer, `WriteBatch.cs`
  fabrication note) — movement is EXPECTED, pre-declared here, recorded as same-commit
  census-deltas per TESTING_STRATEGY; pre-measure at Phase 0.9, re-measure in each touching
  commit. Canonical `--count-matches` invocations; bi-script К-L.
- C2 places two body-only .md files — confirm the governance scanner reports them EXCLUDED, not
  orphaned (`validate --armed` green is the proof).

## 11. Commit plan [CORE]

| #  | Subject | Content |
|----|---------|---------|
| C1 | `governance(id-b): enroll ID_B_ENTITY_VERSIONS brief` | this brief (D/3/Draft) + sync + validate --armed |
| C2 | `chore(governance): authoring templates land in-repo; Special/ and TestResults/ perimeter formalized` | §7.6 — templates at excluded paths (rail fixed to §8.4 v3.0.0) + .gitignore + SCOPE_EXCLUSIONS row + sync + validate --armed |
| C3 | `feat(native): df_world_acquire_versions/release pair -- the versions view; create refuses while held` | §7.1 — exports + guards + entity_id.h is_valid + selftest + MODULE.md; native build + selftest green |
| C4 | `feat(interop,sdk,contracts): span surfaces carry TRUE entity versions; IsValid aligned both sides` | §7.2 — P/Invokes + SpanLease.Versions + three pair-iterators + EntityId.IsValid + ContractsVersion PATCH + round-trip tests |
| C5 | `refactor(systems): fabrication sites move to the versions idiom -- managed code never invents a Version` | §7.3 — census-driven migration + GameBootstrap; EntityEncoder deliberately untouched |
| C6 | `feat(analyzers): DFK022 entity-identity rule; EntityEncoder carries the ratified waiver` | §7.4 — rule + tests + waiver (HARD pin 2 -> 3) |
| C7 | `docs(identity): K-L22 seated AUTHORED; IAC section 2 is shipped law; teaching sites tell the versions truth` | §7.5 doc set, frontmatter bumps vs LIVE + sync + validate --armed |
| C8 | `governance(closure): ID_B EVT + ROADMAP write-back -- F-59 CLOSED, identity family complete` | AUDIT_TRAIL append + F-rows per §14 + brief -> EXECUTED + sync + validate --armed |

Commit count is intended-form; defect-iteration splits are recorded, never compressed. After C8:
**push the branch, open the PR** (§8.4 v3.0.0); post-push corrections are new commits only.

## 12. REGISTER cascade [CORE]

Schema-2.0 discipline; Phase 0 verbatim shapes only; `PENDING-*` outlawed; real hashes or omit.

- C1: this brief enrolled (D/3/Draft).
- C2: SCOPE_EXCLUSIONS edit = a LOCKED tier-1 doc mutation — version bump per its live
  frontmatter convention + sync + validate in the same commit. The two template files are NOT
  enrolled (excluded paths, body-only — that is the design, verified by validate green).
- C7 amendments (each = frontmatter edit + body + sync + validate in ONE commit):
  KERNEL_ARCHITECTURE MINOR (К-L22 row + series state); IDENTITY_AND_ABI_CONTRACT MINOR (§2
  shipped + corrections); ECS.md PATCH-or-MINOR per touched surface; ANALYZER_RULES MINOR
  (DFK022 row); MODDING.md conditional; `DualFrontier.Contracts` ContractsVersion PATCH (C4,
  code-side history comment per its convention).
- C8 closure: single `AUDIT_TRAIL.yaml` EVT append (real hashes C1..C7); brief -> EXECUTED;
  ROADMAP write-back per §14. `validate --armed` exit 0 at C1, C2, C7, C8.

Version bumps computed against LIVE frontmatter at Phase 0 — never assumed.

## 13. Halt conditions (H-series) [CORE]

- **H1** precondition/(RV) mismatch (§3.1).
- **H2** build/test regression vs Phase 0 baselines (managed OR native selftest).
- **H3** `validate --armed` nonzero.
- **H4** a mandatory read materially contradicts §2 — stop, report the delta.
- **H5** a REGISTER field/enum/sentinel beyond FRAMEWORK 14.3/14.4 vocabularies — escalate.
- **H6** truth-law unsatisfiable, or a D3 site unreachable from any world surface (§7.3c).
- **H7** the Phase 0.6 sweep finds a production create-under-open-span path — the strict create
  guard would break it; report the site list, the architect rules (site fix vs lazy acquisition).
- **H8** the native fence: any change to a SHIPPED native signature or observable semantics
  (additive-only law, IAC §3.1). The D1 guard on `create_entity` under a VERSIONS VIEW is
  sanctioned new-surface semantics (the view did not exist); changing behavior under plain
  component spans is NOT — creation under a component span stays legal.
- **H9** the IsValid corner sweep (Phase 0.7) or a native `is_valid` consumer depends on the
  `(0, v>0)` corner.
- **H10** a HARD pin moves other than the ratified DFK-WAIVER 2 -> 3 and exports 207 -> 209.
- Standing rails: push law §8.4 v3.0.0 (push branch + PR at closure; never `main`, never
  self-merge; no force-push/rewrite/squash — §8.3); derived registers never hand-edited;
  `AUDIT_TRAIL.yaml` append-only; `historical/` and `Special/` read-only (extract the zip
  OUTSIDE the repo); single-writer ROADMAP.

On halt: stop, report state verbatim, await the operator.

## 14. Closure protocol and report [CORE]

Execute the METHODOLOGY session closure protocol. ROADMAP write-back (C8):

- **F-59 -> CLOSED**: mechanism sentence (versions view shipped, three iterators reconstruct true
  generations, fabrication sites migrated, DFK022 enforcing, К-L22 seated); cite the recycled-
  index round-trip test + C3/C4 hashes; note the EntityEncoder waiver and its A7 retirement
  trigger.
- **Identity family forward state**: ID-A + ID-B complete; the family's remaining OPEN item is
  F-60(a) (tick-path ALC root — its own charter, architect-owned); W5 identity preconditions met.
- Housekeeping recorded: templates in-repo at sanctioned excluded paths (rail fixed), `Special/`
  perimeter formalized both sides (git + scanner).
- The closure report (chat): commits table; versions table (each doc + ContractsVersion + derived
  register state); gates table (managed baseline vs closure, native selftest before/after, both
  match-or-better); census table (HARD: DFK-WAIVER 3 exact, exports 209, CW 2, ReservedStub
  34/13; SOFT deltas with same-commit records); F-ledger final state; consolidated `Skeleton
  revisions`; self-attestation (native diff = the D1 additive surface ONLY; sync in every
  frontmatter-touching commit; single EVT append, prior entries byte-unchanged; no history
  rewrites; **branch pushed + PR opened, `main` untouched, PR not self-merged**); operator
  checklist (review per-commit -> merge = ratification; optional live smoke: Launcher + Weather
  pair still resumes across reload — the ID-A behavior must survive ID-B).

## 15. Out of scope [CORE]

- **F-60(a)**: the tick-path ALC root — its own charter; nothing here chases it.
- **F-58** reclamation; **F-57** sovereign-switch family (event ids, tiers, native bus).
- IAC §2 option 3 (`EntityRef` index-only type) — retained endgame, not built here.
- A7/W7 persistence semantics (the EntityEncoder waiver's retirement trigger, not its fix).
- The legacy FNV fallback path and `[Obsolete]` classes; `useRegistry: false` rigs.
- `gate.py` and the rest of `Special/files.zip` beyond the two templates.
- Analyzer LOCK rides (DFK021 family, К-L21 cascade); К-L22 LOCKING (AUTHORED only here).
- W4/W5 wave work; UI program; G-RATIO deliberation.
