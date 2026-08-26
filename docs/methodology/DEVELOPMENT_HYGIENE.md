---
register_id: DOC-B-DEVELOPMENT_HYGIENE
project: Dual Frontier
category: B
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 2.2.0
first_authored: 2026-06-29
last_modified: '2026-08-26'
content_language: en
next_review_due: 2027-06-11
title: Development hygiene
last_modified_commit: 27807f7
review_cadence: on-change+annual
last_review_date: 2026-08-26
last_review_event: 'MINOR 2.1.0 -> 2.2.0 2026-08-26 (F60A_TICK_PATH cascade, brief section 7.1 D3): section 3 gains the canonical LINUX native-kernel build commands alongside the existing Windows invocation, plus a new subsection recording how the native library reaches managed TEST output on both hosts. Rationale: the operator ruled Linux the PERMANENT development environment on 2026-08-26, and section 3 documented only the VS-bundled MSVC invocation, so the operative build law was unwritten for the environment development actually runs in. Records the three facts that cost time to rediscover: Ninja is single-config so outputs land at the build root with no Release/ subdirectory, a Windows-era build/ tree must be deleted rather than reconfigured, and the paired Exists-guarded None items make the DllImport bare name resolve on either host. MINOR per CODING_STANDARDS 10.1 rule 3 (a new rule or family, nothing inverted). No lifecycle transition (LOCKED).'
reviewer: Crystalka
risks_referenced:
- RISK-011
- RISK-012
---

# Development hygiene

Operational truth for working in this repository: what is on disk, which commands actually build it, which scripts have which side effects, and which policies bind the executor.
Every claim below names its on-disk artifact or its verified invocation.
Architecture lives in `docs/architecture/`; code law lives in `CODING_STANDARDS.md`; test law lives in `TESTING_STRATEGY.md`; this document is the layer between them and the keyboard.

## §1 — Repository map

Top two levels, one phrase each (verified 2026-06-11):

```
.claude/ .vs/ .vscode/        IDE + session configuration (machine-local state)
assets/                       Kenney art packs, Cinzel font, scenes/, shaders/
BenchmarkDotNet.Artifacts/    gitignored benchmark outputs
docs/
  architecture/               substrate specs (KERNEL, MOD_OS, VULKAN, ...) + historical/ quarantine
  audit/                      closure reviews
  benchmarks/                 performance measurement records
  governance/                 REGISTER.yaml + FRAMEWORK + PROJECT_AXIOMS + validation/render outputs
  ideas/ learning/ mechanics/ design + study material
  methodology/                this document + CODING_STANDARDS + TESTING_STRATEGY + METHODOLOGY
                              + RESERVED_SURFACE_MUTABILITY
  prompts/                    execution prompts (e.g. PHASE_BETA_PREP_EXECUTION_PROMPT.md)
  reports/                    reconnaissance + drift reports
  research/ scratch/          study material; working notes
  ROADMAP.md                  the single roadmap target for all future-work content
mods/                         DualFrontier.Mod.Example (sln) + 6 Vanilla.* mods (disk-only)
                              + Directory.Build.targets
native/
  DualFrontier.Core.Native/   C++ kernel: CMakeLists.txt, src/, include/,
                              build/ (canonical MSVC tree), out/ (VS-IDE preset tree — non-canonical)
src/                          12 managed projects + src/Directory.Build.props (analyzer wiring)
tests/                        11 test/benchmark projects + 19 fixture projects
tools/
  governance/                 sync_register.ps1, render_register.ps1, query_register.ps1,
                              SCOPE_EXCLUSIONS.yaml
  DualFrontier.Analyzers/     Roslyn analyzer project (17 rule stubs under Rules/)
  DualFrontier.Mod.ManifestRewriter/   mod-manifest tooling
  briefs/                     Category D cascade briefs + closure reports
  shaders/                    shader sources for the CompileShaders target
  glslangValidator.exe        committed shader-compiler binary
  scaffold-runtime.ps1        idempotent runtime-directory materializer
  scratch/                    working notes
DualFrontier.sln              the managed solution (§2)
Directory.Build.props         root MSBuild props (TreatWarningsAsErrors=true; CompileShaders target)
Directory.Packages.props      Central Package Management (single version source, A'.9.1 Phase α Commit 3)
```

## §2 — Project set truth (sln vs disk)

`DualFrontier.sln` contains **32 projects** plus 4 solution folders (`src`, `tests`, `tools`, `mods`) — counted from `Project(` entries with the solution-folder GUID `{2150E333-…}` excluded. Composition:

**12 src projects** (all wired to the analyzer via `src/Directory.Build.props`):

- `DualFrontier.AI`
- `DualFrontier.Application`
- `DualFrontier.Components`
- `DualFrontier.Contracts`
- `DualFrontier.Core`
- `DualFrontier.Core.Interop`
- `DualFrontier.Crypto.Future`
- `DualFrontier.Events`
- `DualFrontier.Launcher`
- `DualFrontier.Persistence`
- `DualFrontier.Runtime`
- `DualFrontier.Systems`

**17 projects under `tests/`**:

- 9 test projects: `DualFrontier.Analyzers.Tests`, `DualFrontier.Application.Tests`, `DualFrontier.Core.Interop.Tests`, `DualFrontier.Core.Tests`, `DualFrontier.Mod.ManifestRewriter.Tests`, `DualFrontier.Modding.Tests`, `DualFrontier.Persistence.Tests`, `DualFrontier.Runtime.Tests`, `DualFrontier.Systems.Tests`;
- 2 executables: `DualFrontier.Core.Benchmarks` (BenchmarkDotNet) and `DualFrontier.Runtime.SmokeTest`;
- 6 fixture projects: `Fixture.BadRegularMod`, `Fixture.BadSharedMod_WithIMod`, `Fixture.PublisherMod`, `Fixture.SharedEvents`, `Fixture.SubscriberMod`, `Fixture.VanillaMod_HotReloadOverride`.

**2 tools projects**: `DualFrontier.Analyzers`, `DualFrontier.Mod.ManifestRewriter`.

**1 mods project**: `DualFrontier.Mod.Example`.

**Disk-only by design — do not "fix" by enrolling into the sln:**

- **6 Vanilla mods** (`mods/DualFrontier.Mod.Vanilla.{Combat,Core,Inventory,Magic,Pawn,World}`) — runtime-loaded through the mod pipeline like any third-party mod (К-L9 «Vanilla = mods»); they are deliberately not solution members.
- **13 `Fixture.RegularMod_*` projects** under `tests/` — transitively built via 13 `ProjectReference` entries in `tests/DualFrontier.Modding.Tests/DualFrontier.Modding.Tests.csproj` (verified count), then consumed as runtime fixture artifacts by the Modding suite. Solution membership would add nothing and bloat the build matrix.
- **`native/DualFrontier.Core.Native`** — C++ CMake project, not representable in the managed sln; built per §3.

Disk total under `tests/` is therefore 11 + 19 = 30 project directories, of which 17 are sln members.

## §3 — Build commands (verified invocations only)

**Managed solution:**

```
dotnet build DualFrontier.sln -c Release
```

- Compile-clean: 0 warnings, 0 errors (`TreatWarningsAsErrors=true` in root `Directory.Build.props` — any warning is a build break).
- Known caveat: **MSB3026/MSB3027 copy-step failures occur when stale `testhost.exe` processes hold test output binaries.** Close test runners first, or wait out MSBuild's retry loop. This is environmental (RISK-011, §6), not a code defect.

**Native kernel:**

```
"D:\Visual Studio\Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe" --build native\DualFrontier.Core.Native\build --config Release
```

- cmake 4.2.3-msvc3 is **VS-bundled, NOT on PATH** — the absolute path above is the verified invocation; the install root is discoverable via `vswhere.exe` (`${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe`).
- Outputs land at `native\DualFrontier.Core.Native\build\Release\`: `DualFrontier.Core.Native.dll` plus `df_native_selftest.exe`.
- The sibling `native\DualFrontier.Core.Native\out\` directory is the **VS-IDE CMake-preset Debug tree — it is NOT the canonical build tree.** The canonical tree is `build\`.

**Native kernel — Linux (the permanent development environment, operator ruling 2026-08-26):**

```
cmake -S native/DualFrontier.Core.Native -B native/DualFrontier.Core.Native/build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build native/DualFrontier.Core.Native/build
```

- cmake **is** on `PATH` here (4.2.3), with ninja 1.13.2 and gcc/g++ 15.2; the MSVC `/std:c++23preview` pin (К-L1) is Windows-arm-only and the CMake GNU arm needed no change.
- **Ninja is single-config: outputs land at the build ROOT `native/DualFrontier.Core.Native/build/`, with NO `Release/` subdirectory** — `DualFrontier.Core.Native.so` plus `df_native_selftest`. This is the single most load-bearing difference from the Windows layout above, and it is what the test-project `.so` copy items point at.
- The build tree is **generator-specific and not portable**: a `build/` directory configured by one generator refuses reconfiguration by another (`CMakeCache.txt directory ... is different`). A tree carried over from the Windows era must be deleted, not reconfigured — `rm -rf native/DualFrontier.Core.Native/build`. The directory is git-ignored (`.gitignore`), so deleting it touches no tracked file.
- Selftest: `./native/DualFrontier.Core.Native/build/df_native_selftest` → `ALL PASSED` (110 scenarios at this writing).

**How the native library reaches managed TEST output (both hosts):**

- Each native-dependent test project carries **two sibling `<None>` items** with `CopyToOutputDirectory=PreserveNewest`: the Windows one pointing at `build\Release\DualFrontier.Core.Native.dll`, the Linux one at `build\DualFrontier.Core.Native.so`. Both are `Exists(...)`-guarded, so exactly one applies on any given host and no OS condition is needed (MSBuild normalises the backslash separators on Linux).
- The 8 projects carrying this pair: `DualFrontier.Modding.Tests`, `DualFrontier.Runtime.Tests`, `DualFrontier.Core.Tests`, `DualFrontier.Core.Interop.Tests`, `DualFrontier.Application.Tests`, `DualFrontier.Systems.Tests`, `DualFrontier.Core.Benchmarks`, `DualFrontier.Runtime.SmokeTest`.
- `DllImport` uses the bare name `"DualFrontier.Core.Native"` (`NativeMethods.cs`), which default .NET probing resolves to `<name>.so` on Linux and `<name>.dll` on Windows — **no managed rename is needed for either host.**
- Build the native tree BEFORE the managed solution: the `Exists` guard means a missing library makes the copy silently skip, and the failure then surfaces as `DllNotFoundException` at test time rather than as a build error.

**Single project:**

```
dotnet build src\DualFrontier.Launcher\DualFrontier.Launcher.csproj -c Release
```

**How Native.dll reaches managed output:**

- `src/DualFrontier.Launcher/DualFrontier.Launcher.csproj` carries a `<None>` item with `CopyToOutputDirectory=PreserveNewest` pointing at `native\DualFrontier.Core.Native\build\Release\DualFrontier.Core.Native.dll` (the К-extensions cascade #3 γ0 fix).
- The item has an `Exists(...)` `Condition` guard — **if only the `out\` tree was built, the copy silently skips** and the Launcher starts without its kernel. Build the canonical `build\` tree first.

**Observed durations** (R3 survey, 2026-06-11, this machine — calibration data, not contract):

- full sln ≈ 36 s when fighting testhost lock contention; faster on a clean process table;
- native kernel ≈ 7 s;
- Launcher single-project ≈ 5 s, 0 warnings / 0 errors.

**Tests:** invocation law lives in `TESTING_STRATEGY.md` §8 — cited, not restated here.

## §4 — Tooling reality

**Shell substrate:**

- Windows PowerShell **5.1** (5.1.26100.8655). `pwsh` (PowerShell 7+) is **absent** on this machine — scripts and invocation forms must be 5.1-compatible.
- Module `powershell-yaml` **0.4.12** is installed and required by the governance scripts.

**Governance suite** (`tools/governance/`; `SCOPE_EXCLUSIONS.yaml` is consumed by sync and render):

- **`sync_register.ps1 -Validate`** — validates REGISTER consistency **and unconditionally rewrites `docs/governance/VALIDATION_REPORT.md`** (`Set-Content -Path $REPORT_PATH …` at line 380 — it runs on every invocation, pass or fail).
  Consequence: **every validate run is folded into a commit** — run validate, then stage the refreshed report with the governance commit it verifies.
  A validate run that leaves a dirty report in the tree is a protocol violation.
- **`sync_register.ps1 -Sync`** — strips and rewrites the frontmatter mirrors **in place across all registered `.md` files**.
  **FORBIDDEN outside ratified register cascades.** It only runs when validation is clean, but its blast radius is every registered document — never run it casually.
- **Default no-switch mode runs both** validate and sync. **Never use it.** Always pass an explicit switch.
- **`render_register.ps1`** — writes `docs/governance/REGISTER_RENDER.md` from REGISTER.yaml.
  Run **only at register-cascade closure**; between closures the render is allowed to go stale, and the staleness is recorded explicitly (Findings ledger) rather than silently re-rendered.
- **`query_register.ps1`** — read-only register queries (`-Tier`/`-Lifecycle`/`-Category`/`-Stale`/`-CapaOpen` filters). Safe at any time.

**Working invocation forms** (PowerShell 5.1, this machine):

```
& .\tools\governance\sync_register.ps1 -Validate
```

— in-session form: proven, complies with machine policy. Subprocess form:

```
powershell -NoProfile -File tools\governance\sync_register.ps1 -Validate
```

— policy-dependent. Note: `-ExecutionPolicy Bypass` is **declined by the auto-mode permission classifier on this machine** — use the in-session form.

**Other tools:**

- `tools/scaffold-runtime.ps1` — idempotent runtime-directory materializer.
- `tools/glslangValidator.exe` + `tools/shaders/` — shader compilation; wired through the `CompileShaders` target in root `Directory.Build.props` (runs `BeforeTargets="Build"` using the in-repo committed binary).
- `tools/briefs/` — Category D cascade briefs and closure reports (governance artifacts, not scripts).

## §5 — Branch and push policy

- **The executor pushes its BRANCH and opens a PR; the architect merges** (`CODING_STANDARDS.md` §8.4, v3.0.0).
  An execution session ends with local commits, a closure report, a pushed work branch and an open pull request
  against `main`. The executor never pushes to `main` and never merges its own PR — the merge is Crystalka's act,
  and it is where ratification happens. Expect an auto-mode re-confirmation on the push.
  Superseded 2026-08-20: the previous "the executor NEVER pushes" could not hold in a remote/cloud session, where
  an unpushed branch is lost work.
- **Fast-forward-only merges** for linear cascades (`git merge --ff-only`).
  A cascade branch that cannot fast-forward onto `main` halts for adjudication instead of generating a merge commit.
- **Atomic commits** per `CODING_STANDARDS.md` §8 — one conceptual change per commit, structured body, no squash, no history rewrite.
  Surface revisions ride inside commits as `Skeleton revisions:` records per `RESERVED_SURFACE_MUTABILITY.md` §5.
- **Branch census (2026-06-11):** 44 local branches besides `main`; **41 are fully merged into `main`** and are pruning candidates — tracked as a ROADMAP Findings-ledger entry, **NOT pruned in this cascade.**
  The 3 unmerged branches, by name:
  - `claude/cpp-core-experiment-cEsyH` — the original C++ kernel experiment; **К-L source-of-truth reference** (cited by KERNEL_ARCHITECTURE.md) — keep;
  - `claude/godot-removal-deliberation-Vfg2R` — deliberation record branch;
  - `feat/m0-m3-mod-os-migration` — historical migration branch.

## §6 — Environment notes (Skarlet)

- **OS:** Windows 11 Home 10.0.26200.
- **.NET:** dotnet SDK **10.0.204** — the single installed SDK, now pinned by the repo-root `global.json` (`"rollForward": "latestFeature"`).
  Projects target `net10.0` (STACK_UPDATE cascade, 2026-07-17). The former «do not add `global.json` pinning without deliberation» prohibition was discharged by exactly that deliberation: the Q1-beta operator ruling (ADD with pin — the commit history is the research dataset; reproducibility wins) is recorded in `EVT-2026-07-17-STACK_UPDATE` (AUDIT_TRAIL.yaml).
- **MSVC toolchain:** Visual Studio at `D:\Visual Studio` (MSBuild 18.5.4).
  cmake is VS-bundled, not on PATH — see §3 for the exact invocation path.
- **Known failure pattern — testhost.exe locks (RISK-011):** stale `testhost.exe` processes from previous `dotnet test` runs hold test-bin files and break the solution build's copy steps (MSB3026/MSB3027).
  Mitigation: close test runners / kill stale testhosts, or let MSBuild's retry window expire and rebuild.
  Treat as environmental noise, not regression, unless it reproduces with a clean process table.

## §7 — Godot status

Godot is **fully eradicated** — deprecated and physically purged at К-extensions cascade #2 (2026-05-23), with the residual inert file surface deleted at the Godot Eradication Cascade (2026-06-29; F-5 authorized + closed). Current state, verified against disk:

- **Zero Godot package or SDK references** in any `.csproj` or `Directory.Build.props` (repo-wide grep clean).
- Remaining `Godot` mentions in `src/**/*.cs` are the **9 retirement-notice / historical-context comments only** on the reserved seams (e.g., the deprecation note in `src/DualFrontier.Application/Bridge/PresentationBridge.cs`) — no code references.
- Historical Godot documentation is quarantined under `docs/architecture/historical/`.
- **The Godot file surface is gone.** `project.godot`, `icon.svg` / `icon.svg.import`, the 11 `assets/**/*.import` image sidecars, and the 204 `.cs.uid` resource-UID sidecars were deleted (the build stayed green, proving inertness); the Godot-specific `.gitignore` block and the `.godot/**` / `.gdignore` SCOPE_EXCLUSIONS entries were removed with them.

Rendering today is the in-house Vulkan substrate (`VULKAN_SUBSTRATE.md`) consumed by `DualFrontier.Launcher`; there is no engine dependency.

## §8 — Amendment protocol

This document is Category B, **Tier 1, LOCKED**. It states operational truth, so it amends whenever the truth moves:

1. **Trigger** — a verified change in repository layout, project set, build invocation, tooling behavior, branch policy, or environment.
2. **Rationale** — the commit (or cascade) that changed the underlying reality is named in the change-history entry.
3. **Semver** — PATCH for corrections within a section (counts, paths); MINOR for new sections or new tooling; MAJOR for a rewrite or an operational-model change.
4. **Register update** — per `METHODOLOGY.md` §12.5 post-session update protocol; closure-grade changes per §12.7.
5. **Cross-document propagation** — `CODING_STANDARDS.md` §9 (verification gates) and `TESTING_STRATEGY.md` §8 (invocations) are re-checked whenever §3/§4 here change.

Tier 1 LOCKED amendment ratification follows FRAMEWORK §7.2 (via the `PROJECT_AXIOMS.md` §5 chain).

## Change history

- **v2.2.0 (2026-08-26)** — F60A_TICK_PATH cascade (brief §7.1, D3). §3 gains the canonical **Linux** native-kernel
  build commands beside the existing Windows invocation, and a new subsection records how the native library reaches
  managed **test** output on both hosts. The operator ruled Linux the PERMANENT development environment on 2026-08-26,
  and §3 documented only the VS-bundled MSVC path — the operative build law was unwritten for the environment
  development actually runs in. Three facts are pinned because each cost time to rediscover: Ninja is single-config so
  outputs land at the build ROOT with no `Release\` subdirectory; a Windows-era `build/` tree must be **deleted**, not
  reconfigured (CMake refuses a generator change in place); and the paired `Exists`-guarded `<None>` items let the bare
  `DllImport` name resolve on either host with no managed rename. **MINOR** per `CODING_STANDARDS.md` §10.1 rule 3
  (a new rule or family; nothing inverted).
- **v2.1.0 (2026-08-20)** — Push-policy inversion propagated from `CODING_STANDARDS.md` §8.4 v3.0.0 (§10.1 rule 5).
  §5's "the executor NEVER pushes" replaced with "the executor pushes its BRANCH and opens a PR; the architect merges."
  The executor still never pushes `main` and never merges its own PR — ratification moved from the push to the merge.
  Operator direction 2026-08-20: the old rule could not hold in a remote/cloud session, where an unpushed branch is
  lost work. **MINOR** (new rule content, not an inversion originating here). Backfill in the same commit: the
  **v2.0.2** row below, which landed in `last_review_event` at STACK_UPDATE Phase E but was never written into this list.
- **v2.0.2 (2026-07-17)** — STACK_UPDATE Phase E **PATCH** (backfilled 2026-08-20): §6 .NET bullet updated to the
  landed stack truth — projects target `net10.0`, repo-root `global.json` pins SDK `10.0.204` with
  `rollForward: latestFeature` per the Q1-beta operator ruling of 2026-07-17 — and the "do not add global" guidance
  adjusted accordingly. See this document's `last_review_event` for the full record.
- **v2.0.1 (2026-06-29)** — Godot Eradication Cascade (F-5 closed). §7 flipped from "tails remain, gated by F-5" to "fully eradicated": `project.godot`, `icon.svg` / `icon.svg.import`, the 11 `assets/**/*.import` + 204 `.cs.uid` sidecars deleted (build stayed green — inertness proven); the Godot-specific `.gitignore` block + `.godot/**` / `.gdignore` SCOPE_EXCLUSIONS exclusions removed. §1 repo-map `project.godot` line retired (the file is gone). PATCH per §8 (corrections within sections).
- **v2.0.0 (2026-06-11)** — Full rewrite to post-Godot operational truth per `tools/briefs/STANDING_LAW_CASCADE_BRIEF.md` §7-W3 (Standing-Law Cascade).
  v1.0's Godot-era engine/game project table, phantom `tools/build-all.sh|.ps1` wrappers, and `DualFrontier.Presentation` projects (none of which exist on disk) purged.
  Replaced with the verified sln/disk project census (§2), verified build invocations (§3), governance-script side-effect law (§4), branch/push policy with census (§5), environment notes (§6), and Godot end-state (§7).
  All enforcement claims name on-disk artifacts.
- **v1.0 (2026-05-12, historical)** — Initial engine/game-boundary PR checklist, authored in the Godot two-track era; superseded in full by v2.0.0.