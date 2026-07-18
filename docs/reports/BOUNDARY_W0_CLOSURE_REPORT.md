---
register_id: DOC-E-BOUNDARY_W0_CLOSURE_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-18'
last_modified: '2026-07-18'
content_language: en
next_review_due: null
review_cadence: none-historical-record
title: 'BOUNDARY_W0 CLOSURE REPORT -- 2026-07-18 -- Wave 0 of the vanilla-separation program: boundary axis enrolled (law LOCKED 1.0.0, plan Live 1.0.1, audit report DOC-E), six vanilla mods enrolled into DualFrontier.sln, engine-to-game reference ratchet (B-1/B-6 interim enforcer) red-once-then-green, stale net8.0 comment fixed; 6-commit cascade da97308..closure; gates build 0W/0E both configs + full-sln 1179/0/5 + validate --armed exit 0 + DFK-WAIVER 2=2; register 349 -> 354, EVT 58 -> 59, surface 134 -> 136'
special_case_rationale: 'Durable closure report enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-EQ_A1_FAULT_SYMMETRY_CLOSURE_REPORT, DOC-E-CODEX_CLOSURE_REPORT). Records the BOUNDARY_W0 (Wave 0, vanilla-separation) execution against the brief DOC-D-BOUNDARY_W0_BRIEF.'
---

# BOUNDARY_W0 Closure Report -- 2026-07-18

Wave 0 of the vanilla-separation program, executed against `DOC-D-BOUNDARY_W0_BRIEF` on branch
`claude/boundary-w0`. Zero gameplay code touched. The boundary axis is enrolled and ratified, the six
vanilla mods are real build participants, and boundary law B-1 is now held by a mechanical ratchet.

## HEAD

- **Before:** `4c58942` (post-EQ_A1_FAULT_SYMMETRY; the brief's Phase 0 anchor; clean tree + 4 untracked files).
- **After:** the C6 closure commit (tip of `claude/boundary-w0`; this report + the EVT + the doc closures).
- **Branch:** `claude/boundary-w0`, never pushed (executor law).

## Commits

| # | Hash | Subject | One-line delta |
|---|---|---|---|
| C1 | `da97308` | governance(boundary): enroll law/plan/audit + brief | law+plan moved to docs/architecture/, audit DOC-E frontmatter lifted from appendix to head, brief enrolled Draft; register 349 -> 353 |
| C2 | `4aa1fa0` | build(mods): enroll the six vanilla mods | 6 Vanilla mods into DualFrontier.sln + sln ProjectDependencies build-order fix on the rewriter; Debug+Release 0W/0E |
| C3 | `b973192` | governance(boundary): ratify law -> LOCKED, plan -> Live | law AUTHORED -> LOCKED 1.0.0, plan Draft -> Live 1.0.0; surface 134 -> 136; B-2 ACTIVE |
| C4 | `a26e8ac` | test(governance): engine-to-game ratchet | BoundaryRatchetTests (2 facts) freezing 4 edges + 1 IVT; red-once-then-green proven |
| C5 | `c9387c1` | chore(build): fix stale net8.0 scope comment | src/Directory.Build.props comment net8.0 -> net10.0 (audit A6); comment-only |
| C6 | closure | governance(closure): EVT + ROADMAP track + plan W0 DONE | brief -> EXECUTED, plan W0 DONE + 1.0.1, ROADMAP separation track, EVT #59, this report; register 353 -> 354 |

## Ratchet failability proof (red-once-then-green, verbatim)

Green baseline: full `DualFrontier.Governance.Tests` 66/0/0 (64 prior facts all passing after the C1
audit-frontmatter fix greened `Validate_AgainstLiveTree_Succeeds`, + 2 ratchet facts). A transient
uncommitted engine->game edge (`<ProjectReference Include="..\DualFrontier.AI\..." />` added to
`src/DualFrontier.Core/DualFrontier.Core.csproj`) reddened the ProjectReference ratchet:

```
FAIL DualFrontier.Governance.Tests.BoundaryRatchetTests.EngineToGame_ProjectReferenceEdges_EqualFrozenBaseline
  Expected measured to be a collection with 4 item(s) because boundary law B-1
  (docs/architecture/GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md): no engine assembly may
  ProjectReference a game assembly beyond the frozen migration baseline. ...,
  but {"DualFrontier.Core -> DualFrontier.AI", "DualFrontier.Application -> DualFrontier.Components",
  "DualFrontier.Application -> DualFrontier.Events", "DualFrontier.Application -> DualFrontier.Systems",
  "DualFrontier.Application -> DualFrontier.AI"} contains 1 item(s) more than
  {"DualFrontier.Application -> DualFrontier.Components", "DualFrontier.Application -> DualFrontier.Events",
  "DualFrontier.Application -> DualFrontier.Systems", "DualFrontier.Application -> DualFrontier.AI"}.
```

The transient edge was reverted; the ratchet returned 2/0/0 green and was committed green at C4.

## Final gates (Release, METHODOLOGY 12.7)

- **Build:** `dotnet build DualFrontier.sln` Debug 0W/0E and Release 0W/0E (both, on the final tree).
- **Full-sln test (section 8 harness, no-pipe):** **1179 / 0 / 5** (total 1184). The Phase-0 tree read
  1176/1/5 -- the single failure was `SyncIntegrationTests.Validate_AgainstLiveTree_Succeeds`, caused by
  the pre-C1 untracked body-only audit report (`validate` G-PATH-failed on the missing head frontmatter);
  C1 lifted that frontmatter and restored the F4 shape 1177/0/5, and C4 added the +2 ratchet facts. The
  5 skips are the unchanged F-30/F-31 Extreme guards.
- **validate --armed exit 0** after every frontmatter-touching commit (C1, C3, C6).
- **DFK-WAIVER census 2 = 2** (both `src/DualFrontier.Runtime/Graphics/ValidationLayer.cs` DFK001; untouched).
- `BoundaryRatchetTests` 2/2 + `Validate_AgainstLiveTree_Succeeds` green (within the Governance.Tests 66/0/0);
  `CensusMetaTests`/`Analyzers.Tests` 54/54 green (within the 1179).

## Register / surface / EVT

- **Register documents:** 349 -> **353** (C1: +3 law/plan/audit + brief Draft) -> **354** (C6: +1 this report).
- **Authority surface:** 134 -> **136** (C3: `DOC-A-GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY` LOCKED tier 1 +
  `DOC-A-VANILLA_SEPARATION_MIGRATION_PLAN` Live).
- **AUDIT_TRAIL EVTs:** 58 -> **59** (`EVT-2026-07-18-BOUNDARY_W0`, execution_milestone; prior entries byte-unchanged).

## Surface delta

`CURRENT_AUTHORITY_SURFACE.yaml` gained exactly the two ratified boundary docs: the law appears LOCKED
(tier 1, 14.7 predicate) and the plan appears Live. No other surface entry changed.

## Mutable-surface / Skeleton revisions

- No reserved-surface symbol or intended-form was revised. Mutable-surface touches: the `DualFrontier.sln`
  project enrollment (6 mods) + `ProjectSection(ProjectDependencies)` on the rewriter (C2 build wiring,
  recorded in the C2 body); the `BoundaryRatchetTests` frozen baseline constants (C4, a new census pin under
  the shrink-only rule documented in the class doc).
- The law's `special_case_rationale` (a 1+AUTHORED forbidden-pair sanction) was retired at C3 -- 1+LOCKED is a
  legal tier x lifecycle pair (FRAMEWORK 14.5); the ratification provenance moved to `last_review_event`.

## Discrepancies recorded (brief vs measured)

- The brief's C1 estimated "register 349 -> 352"; measured **349 -> 353**. The +1 is the brief's own file:
  `sync` scans the whole repo filesystem, so the on-disk brief enrolls the instant sync runs. Operator-ruled
  to fold the brief into C1 as Draft (keeping the committed register consistent with tracked files), flipping
  it to EXECUTED at C6. Both handling paths converge on 354 at closure.
- The brief's C6 said "EVT #59"; the id form is date-slug (`EVT-2026-07-18-BOUNDARY_W0`), not a numeric
  counter -- "#59" is the ordinal.
- F2 re-verified exact at Phase 0: 4 engine->game ProjectReference edges + 1 IVT. `Persistence -> Components`
  is game->game (Persistence is not in the boundary-law B-1 engine set), correctly uncounted -- no H-RATCHET.

## Attestation

- **No push** (executor law); the operator pushes `claude/boundary-w0` + merges.
- `sync` run in the same commit after every frontmatter-touching change (C1, C3, C6); `validate --armed`
  exit 0 each time; derived `REGISTER.yaml` / `CURRENT_AUTHORITY_SURFACE.yaml` regenerated by the tool,
  never hand-edited.
- `AUDIT_TRAIL.yaml` prior entries byte-unchanged (append-only, FRAMEWORK 14.6).
- HEAD pinned `4c58942` at Phase 0; the audit report's read-only authoring attestation is preserved as the
  DOC-E historical record.

*End of report.*
