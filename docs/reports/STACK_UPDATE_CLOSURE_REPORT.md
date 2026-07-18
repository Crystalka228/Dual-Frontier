---
register_id: DOC-E-STACK_UPDATE_CLOSURE_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-17'
last_modified: '2026-07-17'
content_language: en
next_review_due: null
review_cadence: none-historical-record
title: STACK_UPDATE CLOSURE REPORT — 2026-07-17 — net10.0 + C# 14 + C++23 (К-L1 v1.1.0) + triad hygiene + global.json pin + AwesomeAssertions 9.4.0 landed (no escape) + doc census; executed in the cloud Linux sandbox; all gates green, F-40 carve-out exercised once
special_case_rationale: 'Closure report enrolled DOC-E Tier 3 per the docs/reports/ durable-report convention (precedents: DOC-E-A_PRIME_9_1_PHASE_GAMMA_RECON_REPORT, DOC-E-LINUX_SANDBOX_ENV_BASELINE_REPORT). Companion of DOC-D-STACK_UPDATE_BRIEF section 9; the environment-shift ground truth lives in DOC-E-LINUX_SANDBOX_ENV_BASELINE_REPORT.'
---

# STACK_UPDATE — Closure Report (2026-07-17)

Brief: `tools/briefs/STACK_UPDATE_BRIEF.md` (DOC-D, EXECUTED at this closure). Executor:
Claude Code (Fable 5) in the **cloud Linux sandbox** — the environment shift off the brief's
LOCAL-Windows assumption is operator-ruled and grounded in
`DOC-E-LINUX_SANDBOX_ENV_BASELINE_REPORT` (deviations D1-D8 there; test-shape D5 and C++ flag
D6 govern this cascade's gate readings). Branch: `claude/setup-dev-tools-s4ng3b`
(= `bc93241` main + 5 sanctioned setup commits).

## 1. Commits

| # | Commit | Phase | Content |
|---|---|---|---|
| 1 | `a20d74a` | B | TFM `net8.0 → net10.0` corpus-wide, LangVersion held 12.0 (D1) |
| 2 | `00460a8` | C | LangVersion `12.0 → 14.0`; analyzer pin `latest → 14.0` explicit (D2) |
| 3 | `b82a4d8` | D1 | xunit `2.9.2 → 2.9.3` |
| 4 | `2f45215` | D2 | xunit.runner.visualstudio `2.8.2 → 3.1.5` |
| 5 | `8f7f448` | D3 | Microsoft.NET.Test.Sdk `17.11.1 → 18.8.1` |
| 6 | `b526c06` | E | `global.json` pin + DEVELOPMENT_HYGIENE §6 v2.0.2 + sync (D4) |
| 7 | `6ec75de` | F | К-L1 C++23 (KERNEL v1.1.0, ARCHITECTURE v1.0.2) + CMake 23 + sync (D5) |
| 8 | `c7dc164` | G | AwesomeAssertions 9.4.0 (D6, landed — no escape) |
| 9 | `18474c0` | H1 | Doc census, governed cluster (7 docs) + sync (D7) |
| 10 | `d704da8` | H2 | Doc census, adjacent cluster + orchestrator rider (5 docs) + sync (D7) |
| 11 | `22b1fb7` | Closure | ROADMAP write-back + F-44..F-48 + sync (D8) |
| 12 | *(this commit)* | Closure | Brief EXECUTED enrollment + this report + sync |
| 13 | *(next commit)* | Closure | `EVT-2026-07-17-STACK_UPDATE` append to AUDIT_TRAIL.yaml + sync |

Committer-identity note: phase F/G commits were re-authored pre-push (`294e3e4 → 6ec75de`,
`aa8c3d5 → c7dc164`) to normalize the committer email per the harness signing policy —
content-identical, unpushed at the time, no published-history rewrite.

## 2. Versions (before → after)

| Surface | Before | After |
|---|---|---|
| TFM (Directory.Build.props) | net8.0 | **net10.0** (LTS to 2028-11-14); analyzer stays netstandard2.0 |
| LangVersion | 12.0 | **14.0**; analyzer csproj `latest` → **14.0 explicit** |
| SDK pin | none (sole 10.0.204 installed) | **global.json 10.0.204, rollForward latestFeature** |
| xunit / runner / Test.Sdk | 2.9.2 / 2.8.2 / 17.11.1 | **2.9.3 / 3.1.5 / 18.8.1** |
| Assertions | FluentAssertions 6.12.1 (license-frozen) | **AwesomeAssertions 9.4.0** (Apache-2.0) |
| C++ standard / К-L1 | C++20 / К-L1 v1.0.1 | **C++23 / К-L1 amended, KERNEL v1.1.0** |
| Emitted C++ flag (this sandbox, GNU) | `-std=c++20` | **`-std=c++23` — all 59 TU** (compile_commands census; H-CXX passed) |
| Emitted C++ flag (MSVC, operator machine) | `/std:c++20` (`stdcpp20`) | expected **`/std:c++23preview`** — re-verify on Windows (baseline report D6; F-44) |

## 3. Phase A measured delta (verbatim)

Working-tree TFM flip to net10.0 (LangVersion held 12.0), full-solution build under TWAE:

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

**The delta is ZERO diagnostics.** MECHANICAL fix-set: empty; SEMANTIC class: vacuously
absent — no H-M. The recon's wave-9/10 WarningLevel question resolved empirically to
no surface; `git checkout` restored the tree, and Phase B re-applied the bump for real.

## 4. Gates — baseline vs closure

| Gate | Baseline (setup close, net8.0/C++20) | Closure (net10.0/C#14/C++23/AA) |
|---|---|---|
| Build (TWAE) | 0 W / 0 E | 0 W / 0 E (Debug AND Release) |
| Native selftest | ALL PASSED | ALL PASSED (C++23) |
| Full-sln tests (Debug) | 1048 / 0 / 116 | 1048 / 0 / 116 |
| Full-sln tests (**Release**, §12.7 closure checklist) | — | **1048 / 0 / 116** |
| Governance suite | 64/64 | 64/64 (inside full runs) |
| `validate --armed` | exit 0 | exit 0 (after every frontmatter-touching commit) |

Shape держался at B, D1, D2, D3, F, G, and the Release closure run. Exception: the Phase C
full run read 1047/1/116 — the single fail was `ModUnloadInteropTests.
UnloadModNativeState_VacuousUnload_Succeeds` («Expected ok to be True»), i.e. **F-40 with its
exact recorded profile**; the isolated Core.Interop.Tests re-run was 202/202 green. Per the
brief's Phase 0 carve-out this is the known flake, not an H2 regression. Manual-F5
verification (§12.7 step 1) is N/A in the sandbox (no display; Runtime is WindowsOnly) —
operator checklist item.

## 5. Censuses

- **DFK-WAIVER hard pin**: `grep -rn "DFK-WAIVER" src/` = **2 at Phase 0, 2 at closure** ✔.
- **Phase F C++20 census** (`grep -n 'C++20\|c++20' docs/architecture/KERNEL_ARCHITECTURE.md`):
  before = 7 sites (lines 48, 73, 77, 79, 81, 440, 530 — the recon's six plus the intro
  line 48); after = 0 operative sites (2 amendment-record mentions remain by design:
  frontmatter event + К-L1 status/LOCK-history line). ARCHITECTURE.md: 1 site (line 49)
  moved; only its own frontmatter amendment record mentions C++20.
- **Phase G using census** (`grep -rn "using FluentAssertions" tests/`): **142 before → 0
  after**; 142 `using AwesomeAssertions;` landed (zero sub-namespace usings existed).
  Package sites: CPM pin + **10** test csproj (the sweep grep also matched 2
  `tools/briefs/*.md` files quoting csproj snippets — reverted byte-identical; briefs are
  LEAVE). API breakage: exactly **3 mechanical rename sites** (`BeGreaterOrEqualTo`×2,
  `BeLessOrEqualTo`×1 → `…ThanOrEqualTo`).
- **R4 worklist completion** (Phase H writer + orchestrator rider):

| Site | Outcome |
|---|---|
| CODING_STANDARDS §4.2 + §4.3 mirrors (+§1/§3 C++) | DONE v2.1.4 |
| TESTING_STRATEGY §1.3 mirror | DONE v2.2.2 |
| ANALYZER_RULES §1.2 + netstandard2.0 gap sentence + §6 | DONE v1.0.2 |
| MODDING §10 quickstart `-f net10.0` ×2 | DONE v1.0.2 |
| README stack lines | **RECON MISMATCH — no stack lines exist at HEAD**; nothing to move, nothing invented (brief-vs-code: code is true) |
| VULKAN_SUBSTRATE §6.4 VS floor | DONE v1.0.3 (`Visual Studio 2026 (18.0)+`); Vulkan-1.3 sites untouched |
| PIPELINE_METRICS §5.2 annotation | DONE v0.2.2 (annotation only; §6 line 287 parallel dated record deliberately left — flagged for operator) |
| RESERVED_SURFACE_MUTABILITY §6 token | DONE v1.0.2 |
| native build.md | DONE v1.0.1 (VS floor + honest C++23/preview note + copy path; recon said it mentioned C++20 — it never did) |
| example-mod README | DONE (bin path net10.0; register-block date) |
| src/**/MODULE.md | census EMPTY SET (zero stale facts) |
| Rider (out-of-worklist current-truth): native MODULE.md ×2, Launcher README, Modding README | DONE (H2 commit) |

- LEAVE honored: `historical/` untouched; EXECUTED reports untouched; `tools/briefs/**`
  untouched (G false-positives reverted); REGISTER_RENDER.md untouched; METHODOLOGY:1155
  untouched.

## 6. Phase G outcome

**LANDED — no escape.** Zero semantic test rewrites; the ESCAPE path stayed unused, so the
brief's reserved F-48 (AA breakage inventory) was not needed; the F-48 number carries the
session-found M74 nodeReuse hang class instead (ledger row states the provenance).

## 7. F-ledger finals

F-44 stable `/std:c++23` migration (OPEN) · F-45 xunit v3 own cascade (OPEN) · F-46
Roslyn/BDN/YamlDotNet hygiene bundle + §4.3 mirror-count nit (OPEN) · F-47 LINQ convention
enforcement gap, `src/DualFrontier.Core/Math/SpatialGrid.cs:3` (OPEN) · F-48 M74 nodeReuse
test-infra hang class (OPEN). F-40 unchanged (flaked once in-cascade with the recorded
profile). Skeleton revisions (RESERVED_SURFACE_MUTABILITY §5): **none** — no reserved-surface
skeleton was touched; the §6 edit is a historical-delta token refresh.

## 8. Self-attestation

- **Pushes**: adapted, operator-sanctioned — the ephemeral cloud container makes the feature
  branch the durability mechanism; pushes went ONLY to `claude/setup-dev-tools-s4ng3b`.
  Zero pushes to `main`, zero PRs, zero merges (those remain the operator's).
- `sync` ran in every frontmatter-touching commit (E, F, H1, H2, ROADMAP, this one, the EVT
  append); `validate --armed` exit 0 after each.
- **Single** AUDIT_TRAIL append (`EVT-2026-07-17-STACK_UPDATE`, the 2nd post-migration
  append), priors byte-unchanged (pure-append diff verified at commit).
- `historical/` read-only held; derived registers never hand-edited; **no DFK-WAIVER added**
  (2 = 2); live-flag n/a; C++22-vs-23 confirm-or-halt: the operator ratified C++23 via the
  approved session plan and the start order («можешь начинать по брифу») — no objection at
  Phase 0.

## 9. Operator checklist

1. Review + **merge** `claude/setup-dev-tools-s4ng3b` (setup commits + cascade commits ride
   together; future web sessions self-provision via the SessionStart hook).
2. **Windows re-verification** at home per baseline report D5/D6: full gates expect
   ~1166-class pass shape (the 3 newly-gated WM_SIZE tests now count as passes there); Phase F
   H-CXX on MSVC — regenerate CMake, confirm the vcxproj emits `stdcpp23preview`; manual F5.
3. **EQ-a (shutdown-law family) chartering is next**; G-RATIO matrix deliberation standing.
4. F-48 ruling (in-test nodeReuse hardening) when convenient; PIPELINE_METRICS §6 line 287
   parallel site if a rider is wanted.
