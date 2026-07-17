---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-B-RESERVED_SURFACE_MUTABILITY
category: B
tier: 2
lifecycle: LOCKED
owner: Crystalka
version: "1.0.1"
next_review_due: 2026-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-B-RESERVED_SURFACE_MUTABILITY
---
# Reserved Surface Mutability

## §1 — Why this document exists

Phase α of cascade A'.9.1 (К-extensions cascade #5, executed 2026-05-25) produced **five mid-cascade deltas** between brief-stated forms and on-disk reality.
At least three were surface-naming/surface-count class, yet each consumed halt-or-surface traffic to Crystalka under the pre-law regime (full reconstruction and classification: §6).

Without a governing principle, every subsequent brief must choose between two bad options:

- **(a) defensive literal transcription** — the brief copies Phase-state literals (names, counts, paths) verbatim so the executor cannot diverge. This is the brief-bloat root flagged as Lesson #N18-class. Honesty note: Lesson #N18 («pre-flight empirical scope verification») is referenced by cascade brief artifacts — `tools/briefs/DOCUMENTATION_DUAL_LOAD_DRIFT_RECONNAISSANCE_BRIEF.md` — but is **not yet codified** in `METHODOLOGY.md`, whose lessons top out at #N17. The numbering gap is tracked in the [ROADMAP Findings ledger](../ROADMAP.md) as F-7, architect-owned.
- **(b) refactor reality to match the brief** — the executor renames on-disk artifacts back to the brief's stale forms, producing rippling diffs that obscure the actual work.

This document declares the third path, adapting a proven external skeleton-mutability governance mechanism onto Dual Frontier's substrate.
It **pre-authorizes the trivial class**: surface properties may be revised by any cascade commit, atomically and commit-body-recorded, without governance violation.
Halts remain reserved for genuinely architectural forks.
Briefs may name **intended forms** instead of transcribing literals; the executor verifies empirically and adapts, recording deviations per §5.

## §2 — The principle

A property is **mutable** if and only if changing it changes **no answer in any Tier-1 LOCKED authority**:

- `docs/architecture/KERNEL_ARCHITECTURE.md` Part 0 (К-L invariants);
- `docs/governance/PROJECT_AXIOMS.md` (PA-001..PA-004);
- locked Q-series decisions (Q-L / Q-N / Q-K / Q-G / Q-H series ratification records);
- S-LOCKs declared by any active brief;
- `docs/methodology/METHODOLOGY.md` law (lessons, protocols, §12 register integration).

A property is **immutable** when changing it would change such an answer.

Consequences:

- **Mutable surface** may be revised by any cascade commit **with** a commit-body record per §5. No halt, no pre-ratification.
- **Immutable structure** may not be revised by a cascade commit at all. Revision requires explicit deliberation plus Crystalka ratification through the owning document's amendment protocol (FRAMEWORK §7.2 for Tier 1 LOCKED documents, cited via `PROJECT_AXIOMS.md` §5).

Operating heuristic: if you can rename, recount, or relocate it without any Tier-1 LOCKED document becoming wrong, it is surface.
If a Tier-1 statement would need rewording, it is structure — stop and adjudicate.

## §3 — Mutable catalog (DF-instantiated, explicitly non-exhaustive)

The following are mutable and may be revised by any cascade commit with a §5 record:

1. **Symbol names of reserved stubs and analyzer rule classes.**
   The diagnostic **descriptor ID string is the contract; the file/class name is surface.**
   Since the A'.9.1 Phase β descriptor-ID adjudication (Crystalka-ratified 2026-07-01), every descriptor ID uses the underscore form identical to the file/class form — on-disk truth:
   - `tools/DualFrontier.Analyzers/Rules/Architecture/DFK003_1StorageBridgeAnalyzer.cs` declares `DiagnosticId = "DFK003_1"`;
   - `tools/DualFrontier.Analyzers/Rules/Discipline/DFL025_AReservedStubInvocationAnalyzer.cs` declares `DiagnosticId = "DFL025_A"`.
   The earlier dot/hyphen descriptor duality (`"DFK003.1"` / `"DFL025-A"`) was retired through the contract channel, not as a surface move: Roslyn `ReportDiagnostic` rejects dotted/hyphenated IDs as invalid identifiers, so the duality was Roslyn-illegal (ANALYZER_RULES §4 naming convention; CODING_STANDARDS §5.3 diagnostic-ID form).
   Renaming the file or class is surface; changing the descriptor ID string is a contract change (immutable-or-adjudicate) — the underscore normalization is the worked example of the adjudicate path.
2. **File and directory layout below the project level.**
   Placement of sub-files within a project may be revised freely.
   The **project-to-layer assignment is NOT mutable** — a type belonging to `DualFrontier.Contracts` cannot migrate to `DualFrontier.Systems` as a "surface" move; layer boundaries are К-L / architecture territory.
3. **Annotation surface forms.**
   Attribute argument shapes, `Reason`-field wording, XML doc-tag wording, `<seealso>` link targets.
   The marker family's registered semantics (see `CODING_STANDARDS.md` §5 marker registry) are not surface — only the textual form at a site is.
4. **csproj wiring lists in briefs.**
   The **on-disk set is truth; brief lists are intent.**
   Package counts, ProjectReference enumerations, and project-name spellings in a brief are estimates to be verified empirically (Lesson #N14 discipline), not specifications to be forced onto disk.
5. **Grep patterns and census expressions.**
   The canonical census method lives in `TESTING_STRATEGY.md` §4 (census pins), with family patterns registered in `CODING_STANDARDS.md` §5; both update by census-delta record (§5 below).
   A brief quoting a pattern quotes intent.
6. **Doc section numbering in citations.**
   Cite by **anchor + topic** («CODING_STANDARDS §8 — atomic commit discipline»), not by bare number.
   The owning document may renumber freely, and the citation survives by topic.

This list is non-exhaustive in instances but exhaustive in spirit: anything analogous to these classes is surface.
Anything **not** analogous falls under §7's immutable-or-adjudicate rule.

## §4 — Immutable catalog

The following are NOT mutable. A cascade commit altering any of them is a governance violation regardless of sign-off:

- **The 21 К-L invariants, per KERNEL Part 0's own stated composition.**
  Pinned here to neutralize counting ambiguity (Findings F-4/F-9 class).
  KERNEL_ARCHITECTURE.md Part 0 states the running total as **«21 final»** (cascade #2/#3/#4 chronicle entries: «К-L count unchanged: 21 final»), composed as:
  - **К-L1..К-L5 + К-L7..К-L11** — ten (**К-L6 SUPERSEDED by К-L12 and excluded from the count**);
  - **sub-invariants К-L3.1, К-L7.1, К-L15.1** — three;
  - **К-L12..К-L19** — eight;
  - total **= 21**. **К-L20 is reserved post-Mod-API-lock and excluded from the count.**

  Script warning (F-4): Part 0 table rows carry Latin `K-L` identifiers while prose uses Cyrillic «К-L» — **any census or grep over invariant IDs must match BOTH scripts** or it silently undercounts.
- **PA-001..PA-004** (`PROJECT_AXIOMS.md`). Amendment only per its §5 protocol; PA-001's «PERMANENT» qualifier carries the highest bar.
- **All locked Q-series decisions.** A locked ratification is answered; surface moves may not re-answer it.
- **Declared S-LOCKs.** A cascade commit can satisfy or fail an S-LOCK; it can never weaken or remove one.
  (An S-LOCK's own stated conditional — e.g., S-LOCK-4's «+0 or +1 only» clause — is part of the lock; staying within the conditional is compliance, not revision.)
- **`register_id` permanence.** File renames are surface; the register identity is not. Identity changes require a supersedes chain.
- **The lifecycle state machine** (Draft → AUTHORED → LOCKED → EXECUTED / SUPERSEDED per FRAMEWORK). No skipping, no reverting.
- **Atomic-commit discipline** per `CODING_STANDARDS.md` §8 — no squash, no force-push, no history rewrite. The history is the research dataset; §6 below is direct evidence of why.
- **REGISTER-derived governance state** (lifecycle, tier, category, audit trail). Mutated only through the register protocols of `METHODOLOGY.md` §12.5 (post-session update protocol, Q-A45-X5 lock) and §12.7 (closure protocol) — cited here, not restated.
- **The additive-semver Mod API law (К-L20, reserved).** Reserved status does not make it surface; it is structure-in-waiting.

## §5 — How a cascade commit records a surface revision

A commit that revises mutable surface carries a **`Skeleton revisions:`** section in its body, placed per the commit body order of `CODING_STANDARDS.md` §8. Entry form:

```
Skeleton revisions:
  - <from> → <to> (one-line rationale)
```

Rules:

- **One conceptual change = one entry**, regardless of how many files or sites it touches.
  A convention rename across 30 sites is one entry naming the convention, not 30 entries.
- **Census-delta records are Skeleton-revisions entries.**
  When a commit changes a pinned census count (see `TESTING_STRATEGY.md` §4), the entry takes the form `census pin <name>: X → Y (cause)` and lands in the same commit that moves the count.
- Each rationale is one line and, where applicable, cites the authority that makes the change surface-legal (this document's §3 item, or the brief's intended-form clause).

Worked example (composite, in the on-disk forms of this repository):

```
Skeleton revisions:
  - brief §6.3 CPM migration ~30 csprojs → 11 csprojs (on-disk set is truth; brief count was estimate)
  - census pin ReservedStub sites: 3 → 34 (Phase α NotImplementedException audit surfaced 31 Pattern A sites)
```

## §6 — Retrospective classification — A'.9.1 Phase α deltas

The five Phase α mid-cascade deltas are **all five reconstructable from on-disk artifacts** — the Phase α commit bodies on `main` (`5030fa2..a23556f`, 2026-05-25), `PROJECT_AXIOMS.md` §2.4, and `docs/prompts/PHASE_BETA_PREP_EXECUTION_PROMPT.md`.
None had to be taken on faith. Classification per §2/§3/§4:

| # | Delta (Phase α commit) | On-disk record | Class under this law |
|---|---|---|---|
| 1 | **Commit 1 ProjectReference drop** (`5030fa2`): brief §6.1 specified analyzer→Contracts ProjectReference; netstandard2.0 cannot reference net8.0; reference dropped entirely — Roslyn analyzers detect attributes by FQN string via SemanticModel, no compile-time CLR reference | commit body; `PROJECT_AXIOMS.md` §2.4 last bullet (PA-002 anchor: the kostyl path — overriding framework checks — was rejected) | **Architectural.** Changed a dependency-structure answer of the brief spec. Correctly halted for ratification; this law changes nothing here — this is exactly what halts are reserved for. |
| 2 | **Commit 3 CPM migration count** (`16436b7`): 11 csproj files migrated vs brief §6.3 estimate ~30 (most projects are ProjectReference-only); surfaced to Crystalka via commit message | commit body; cited as calibration precedent in `PHASE_BETA_PREP_EXECUTION_PROMPT.md` («Phase α Commit 3 CPM migration count 11 vs ~30 estimate») | **Mutable surface** (§3 item 4 — on-disk set is truth, brief lists are intent). Would NOT have needed halt traffic under this law; a §5 record suffices. |
| 3 | **Commit 5 first-batch rule-count reconciliation** (`d0b4c41`): brief narrative said «15-16 own rules», amendments log §3.3 said 13; both pre-dated Q-L-9 (DFK010 dropped) + Phase 0 DFK016 retain-α; effective arithmetic = 17, recorded as ANALYZER_RULES §4.6 audit note, within S-LOCK-4's own «+0 or +1 only» conditional | commit body | **Mutable surface** (count-narrative reconciliation; the locked decisions and the S-LOCK conditional itself were untouched — §4 note on S-LOCK conditionals). A §5 census-delta record suffices. |
| 4 | **Commit 6 `<seealso>` URL correction** (`fc66156`): brief §6.6 draft carried pre-rename repo placeholder `Crystalka228/Colony_Simulator`; actual remote is `Crystalka228/Dual-Frontier` per `git remote -v` | commit body («Brief §6.6 spec deviation») | **Mutable surface** (§3 item 3 — annotation surface form). A §5 record suffices. |
| 5 | **Commit 7 annotation-scope expansion** (`f4a94e6`): Phase 0 §3.6 grep reported 3 annotation sites («0 additional»); Phase α `throw new NotImplementedException` audit surfaced 31 more Pattern A sites → 34 annotated; **ratified by Crystalka mid-cascade** (2026-05-25 deliberation surface) | commit body; Lesson #N14 4th application flagged therein | **Mutable surface** (§3 item 5 — census class; the grep pattern, not the architecture, was wrong). Under this law the mid-cascade halt would have been a §5 census-delta record: `census pin ReservedStub sites: 3 → 34`. |

Honest tally: **4 of 5 deltas were surface class; 1 was genuinely architectural.**
This is consistent with (and sharpens) the cascade brief's «≥3 surface-naming class» estimate.
Nothing was unreconstructable: the atomic-commit discipline of `CODING_STANDARDS.md` §8 — every deviation recorded in the commit body at the moment it happened — is what made this audit possible without any session log.

## §7 — Out of scope

- This document is **not a license for architectural change in disguise**.
  A revision whose honest description requires rewording any Tier-1 LOCKED statement is architectural no matter how it is labeled.
- The mutable catalog is exhaustive in the conceptual sense: anything not on it, and not analogous to an item on it, is **immutable-or-adjudicate** — escalate to Crystalka before touching it.
  Default is halt, not improvisation.
- This document does **not** amend KERNEL_ARCHITECTURE.md Part 0, PROJECT_AXIOMS.md, or any other Tier-1 authority.
  §4's composition pin restates what Part 0 already says, in one place, so that citations and censuses stop diverging — it pins **how to cite** the authorities, never what they answer.

## Amendment protocol

This document is Category B, Tier 2, LOCKED. Amendments require:

1. **Surfaced trigger** — a delta class the catalogs misjudge, or a new surface family;
2. **Documented rationale** against §2's principle;
3. **Semver bump** — PATCH for catalog clarification, MINOR for new catalog entries, MAJOR for a principle change;
4. **Register update** per `METHODOLOGY.md` §12.5;
5. **Cross-document propagation** to `CODING_STANDARDS.md` §5/§8 and `TESTING_STRATEGY.md` §4 if the commit-record or census forms change.

Principle changes (§2) additionally require Crystalka ratification.

## Change history

- **v1.0.1 (2026-07-02)** — F-27(a) PATCH at the A'.9.1 Phase δ rider: §3 item 1's on-disk-proof bullets updated from the superseded dot/hyphen descriptor duality to the underscore truth (Phase β descriptor-ID adjudication, Crystalka-ratified 2026-07-01); the item's principle (descriptor ID = contract, file/class name = surface) unchanged. Catalog clarification per the amendment protocol's PATCH class.
- **v1.0 (2026-06-11)** — Initial authoring at the Standing-Law Cascade, per `tools/briefs/STANDING_LAW_CASCADE_BRIEF.md` §7-W3. Mechanism adapted from a proven external skeleton-mutability governance document (read-only reference corpus) onto Dual Frontier's substrate; §6 retrospective classifies the A'.9.1 Phase α deltas from on-disk commit-body evidence.
