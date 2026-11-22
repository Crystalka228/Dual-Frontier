---
register_id: DOC-B-DEVELOPMENT_HYGIENE
project: Dual Frontier
category: B
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 3.0.1
first_authored: 2026-06-29
last_modified: '2026-11-22'
content_language: en
next_review_due: 2027-06-11
title: Development hygiene
review_cadence: on-change+annual
last_review_date: 2026-11-22
last_review_event: 'PATCH 3.0.0 -> 3.0.1 2026-11-22 (REV_2 (CORPUS_REVISION_1), the review repairs): the repository map and the governance-tool section no longer describe tools/governance/query_register.ps1, which REV_2 retired (it needed the powershell-yaml module and read a key the schema no longer carries); the map names LEDGER_FORM.yaml, the G-ROWFORM configuration. PATCH per section 8 rule 3 (a path correction). No lifecycle transition (LOCKED).'
reviewer: Crystalka
risks_referenced:
- RISK-011
- RISK-012
---

# Development hygiene

Operational truth for working in this repository: what is on disk, which commands actually build it, which tools have which side effects, and which policies bind the agents that work in it.
Every claim below names its on-disk artifact or its verified invocation.
Architecture lives in `docs/architecture/`; code law lives in `CODING_STANDARDS.md` (what a code comment may say is its section 6.2, which this document does not restate); test law lives in `TESTING_STRATEGY.md`; this document is the layer between them and the keyboard.

## §1 — Repository map

Top two levels, one phrase each (verified 2026-11-21 at `54a9577`; project counts live in section 2, not here):

```
.claude/ .vscode/             tracked session hook and editor settings (.vs/ is git-ignored IDE state)
.github/workflows/            gitleaks.yml, the only workflow: a secret scan (no build or test CI, section 3)
assets/                       shaders/ (committed SPIR-V), sprites/, kenney/ (committed UI sprites and a
                              terrain sheet); the extracted Kenney packs and the Cinzel font are git-ignored
                              local state
BenchmarkDotNet.Artifacts/    git-ignored benchmark outputs (absent until a benchmark runs)
docs/
  architecture/               substrate specs (KERNEL, MOD_OS, VULKAN, ...) + historical/ quarantine
  audit/                      closure reviews
  benchmarks/                 performance measurement records
  governance/                 FRAMEWORK + PROJECT_AXIOMS + SYNTHESIS_RATIONALE; REGISTER.yaml and
                              CURRENT_AUTHORITY_SURFACE.yaml are DERIVED (never hand-edited, section 4);
                              AUDIT_TRAIL / CAPA / RISKS / REQUIREMENTS yaml are hand-edited sources, and
                              so is REGISTER_SUPPLEMENT.yaml (the non-.md enrolled artifacts; sync reads it)
  ideas/ learning/ mechanics/ design + study material
  methodology/                the standing-law corpus (this document, CODING_STANDARDS, TESTING_STRATEGY,
                              METHODOLOGY, PIPELINE_METRICS, RESERVED_SURFACE_MUTABILITY,
                              MAXIMUM_ENGINEERING_REFACTOR) + BRIEF_TEMPLATE and RECON_KICKOFF_TEMPLATE
  prompts/                    execution prompts (e.g. PHASE_BETA_PREP_EXECUTION_PROMPT.md)
  reports/                    reconnaissance + drift reports
  research/ scratch/          study material; working notes
  ROADMAP.md                  the single roadmap target for all future-work content (and the Findings ledger)
  README IDEAS_RESERVOIR MIGRATION_PROGRESS SECRET_SCANNING TRANSLATION_GLOSSARY TRANSLATION_PLAN (.md)
                              the other docs-root documents
mods/                         the mod projects (Example, Vanilla.*, Weather, Weather.Contracts), all solution
                              members + Directory.Build.targets
native/
  DualFrontier.Core.Native/   C++ kernel: CMakeLists.txt, src/, include/, test/;
                              build/ (canonical Ninja tree, git-ignored); out/ (git-ignored residue, not a build tree)
scripts/                      setup-linux-sandbox.sh (provisions a fresh Ubuntu host)
src/                          managed projects + src/Directory.Build.props (analyzer wiring)
tests/                        test projects, executables and fixture projects (section 2)
tools/
  DualFrontier.Governance/    the governance tool (validate, sync, migrate; section 4)
  governance/                 SCOPE_EXCLUSIONS.yaml (the tool's exclusion list), LEDGER_FORM.yaml (the
                              G-ROWFORM configuration), MODULE.md
  DualFrontier.Analyzers/     Roslyn analyzer project (rules under Rules/)
  DualFrontier.Mod.ManifestRewriter/   mod-manifest tooling
  briefs/                     Category D cascade briefs + closure reports
  shaders/                    shader sources for the Windows-only CompileShaders target
  glslangValidator.exe        committed Windows shader-compiler binary (section 4)
  scaffold-runtime.ps1        idempotent runtime-directory materializer (PowerShell)
  scratch/                    working notes
DualFrontier.sln              the managed solution (section 2)
global.json                   SDK pin (section 6)
Directory.Build.props         root MSBuild props (TreatWarningsAsErrors=true; CompileShaders target)
Directory.Packages.props      Central Package Management (single version source)
game.manifest.json            the product manifest (root mods); the Launcher csproj copies it to its output
launcher-smoke*.log tests-run-*.log   tracked logs of a 2026-05-24 Windows run: dated residue, not output
                              of any current command
README.md LICENSE .editorconfig .gitattributes .gitignore .gitleaksignore CppProperties.json
                              the remaining root files (.editorconfig restates the analyzer severities)
```

## §2 — Project set truth (sln vs disk)

`DualFrontier.sln` contains **57 projects** plus 4 solution folders (`src`, `tests`, `tools`, `mods`), and every tracked `.csproj` is a member. Measured at `54a9577`:

```
grep '^Project(' DualFrontier.sln | grep -vc '{2150E333-8FDC-42A3-9474-1A3956D46DE8}'   # 57 projects
grep '^Project(' DualFrontier.sln | grep -c '{2150E333-8FDC-42A3-9474-1A3956D46DE8}'    # 4 solution folders
git ls-files '*.csproj' | wc -l                                                         # 57 tracked projects
for p in $(git ls-files '*.csproj'); do grep -q "\"$(basename "$p" .csproj)\"" DualFrontier.sln || echo "missing $p"; done   # prints nothing
```

Composition by directory (`git ls-files '<dir>/*.csproj' | wc -l`: src 12, tests 32, tools 3, mods 10):

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

**32 projects under `tests/`**:

- 10 test projects (`grep -l Microsoft.NET.Test.Sdk tests/*/*.csproj`): `DualFrontier.Analyzers.Tests`, `DualFrontier.Application.Tests`, `DualFrontier.Core.Interop.Tests`, `DualFrontier.Core.Tests`, `DualFrontier.Governance.Tests`, `DualFrontier.Mod.ManifestRewriter.Tests`, `DualFrontier.Modding.Tests`, `DualFrontier.Persistence.Tests`, `DualFrontier.Runtime.Tests`, `DualFrontier.Systems.Tests`;
- 2 executables (`grep -l '<OutputType>Exe' tests/*/*.csproj`): `DualFrontier.Core.Benchmarks` (BenchmarkDotNet) and `DualFrontier.Runtime.SmokeTest`;
- 20 fixture projects (`ls -d tests/Fixture.*`): `Fixture.BadRegularMod`, `Fixture.BadSharedMod_WithIMod`, `Fixture.PublisherMod`, `Fixture.SharedEvents`, `Fixture.SubscriberMod`, `Fixture.VanillaMod_HotReloadOverride`, `Fixture.WeatherGateNegative` and the 13 `Fixture.RegularMod_*` projects.

**3 tools projects**: `DualFrontier.Analyzers`, `DualFrontier.Governance`, `DualFrontier.Mod.ManifestRewriter`.

**10 mods projects**: `DualFrontier.Mod.Example`, the seven `DualFrontier.Mod.Vanilla.*` mods (`Combat`, `Core`, `Inventory`, `Magic`, `Pawn`, `Scenario`, `World`), `DualFrontier.Mod.Weather` and `DualFrontier.Mod.Weather.Contracts`.

**Solution membership is complete.** Every project above is a solution member; the six original vanilla mods joined at `4aa1fa0` and the 13 `Fixture.RegularMod_*` projects at `ff51dca`. The vanilla mods stay runtime-loaded through the mod pipeline like any third-party mod (К-L9, "Vanilla = mods"); solution membership makes them build participants and creates no engine-to-game reference (the boundary ratchet in `tests/DualFrontier.Governance.Tests/BoundaryRatchetTests.cs` pins that). The 13 `Fixture.RegularMod_*` projects are additionally referenced by `tests/DualFrontier.Modding.Tests/DualFrontier.Modding.Tests.csproj` with `ReferenceOutputAssembly=false` (the cross-ALC projection pattern, `TESTING_STRATEGY.md` section 2.3). **`native/DualFrontier.Core.Native`** is the one build target outside the sln -- a C++ CMake project built per section 3.

## §3 — Build commands (verified invocations only)

Every command below assumes the shell setup of section 6 (`DOTNET_ROOT` and `PATH` pointing at `~/.dotnet`, `MSBUILDDISABLENODEREUSE=1`) and runs from the repository root.

**Managed solution:**

```
dotnet build DualFrontier.sln -c Release
```

- Compile-clean: 0 warnings, 0 errors (`TreatWarningsAsErrors=true` in root `Directory.Build.props` -- any warning is a build break).
- The environmental hazards are MSBuild node reuse (a persistent node inherits redirected pipes and hangs a run that spawns child builds) and a testhost lingering in shutdown (RISK-011, section 6). Neither is a code defect; both are handled by section 6 and `TESTING_STRATEGY.md` section 8.

**Native kernel (Linux, the permanent development environment, operator ruling 2026-08-26):**

```
cmake -S native/DualFrontier.Core.Native -B native/DualFrontier.Core.Native/build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build native/DualFrontier.Core.Native/build
```

- cmake, ninja and gcc/g++ are on `PATH`. The MSVC `/std:c++23preview` pin (К-L1) lives under `if(MSVC)` in `CMakeLists.txt` and is not exercised here.
- **Ninja is single-config: outputs land at the build ROOT `native/DualFrontier.Core.Native/build/`, with NO `Release/` subdirectory** -- `DualFrontier.Core.Native.so` plus `df_native_selftest`. This is the path the `.so` copy items below point at.
- The build tree is **generator-specific and not portable**: a `build/` directory configured by one generator refuses reconfiguration by another (`CMakeCache.txt directory ... is different`). A tree from another generator must be deleted, not reconfigured: `rm -rf native/DualFrontier.Core.Native/build`. The directory is git-ignored, so deleting it touches no tracked file. `native/DualFrontier.Core.Native/out/` is git-ignored residue of a Windows-era IDE preset and is not a build tree.
- Selftest: `./native/DualFrontier.Core.Native/build/df_native_selftest` prints `ALL PASSED` as its last line and exits 0.
- The Windows arm (the MSVC branch of `CMakeLists.txt`, the `build\Release\*.dll` items of the csproj files) stays in those files; it is neither exercised nor supported on the reference host, and no Windows invocation is law here.

**How the native library reaches managed output (Launcher and tests):**

- The Launcher (`src/DualFrontier.Launcher/DualFrontier.Launcher.csproj`) and each native-dependent test project carry **two sibling `<None>` items** with `CopyToOutputDirectory=PreserveNewest`: the Windows one at `build\Release\DualFrontier.Core.Native.dll`, the Linux one at `build\DualFrontier.Core.Native.so`. Both are `Exists(...)`-guarded, so exactly one applies on a given host and no OS condition is needed (MSBuild normalises the backslash separators on Linux).
- The 8 test projects carrying this pair: `DualFrontier.Modding.Tests`, `DualFrontier.Runtime.Tests`, `DualFrontier.Core.Tests`, `DualFrontier.Core.Interop.Tests`, `DualFrontier.Application.Tests`, `DualFrontier.Systems.Tests`, `DualFrontier.Core.Benchmarks`, `DualFrontier.Runtime.SmokeTest`.
- `DllImport` uses the bare name `"DualFrontier.Core.Native"` (`NativeMethods.cs`), which default .NET probing resolves to `<name>.so` on Linux -- no managed rename is needed.
- Build the native tree BEFORE the managed solution: a missing library makes the `Exists` guard skip the copy silently, and the failure surfaces as `DllNotFoundException` at test time, not as a build error.
- A git worktree carries none of the git-ignored build outputs. When the worktree's native inputs are identical to the donor checkout's (`git diff --quiet <donor-commit> <worktree-commit> -- native/` exits 0), copy the donor's built `DualFrontier.Core.Native.so` (and `df_native_selftest` where the selftest is to run) into the same relative path of the worktree instead of rebuilding. When they differ -- the work under review changes `native/` -- build the worktree's own native tree with the canonical commands above: a copied library would exercise the donor's code, not the change, and the suite and selftest would pass without testing it. A count taken in a worktree without the library is not a measurement.

**Single project:**

```
dotnet build src/DualFrontier.Launcher/DualFrontier.Launcher.csproj -c Release
```

**Launcher dev smoke** (needs a reachable X display, section 6):

```
DISPLAY=:0 timeout --signal=TERM 15 dotnet run --project src/DualFrontier.Launcher -c Release > <log outside the tree> 2>&1
```

PASS = exit code 124 (the timeout killed a live process) AND the log holds no unhandled exception AND a window was observed (the operator confirms the visual; the agent states what the log shows). An exception, or an exit before the timeout, is a failure.

**Tests:** invocation law lives in `TESTING_STRATEGY.md` section 8 -- cited, not restated here.

**Local acceptance chain.** CI truth: the repository's only workflow is `.github/workflows/gitleaks.yml` (secret scan); no CI job runs the build, the suite or `validate --armed` -- the ledger row F-92 owns that gap. Acceptance is the local chain. In order, over the tree being accepted:

1. native selftest `ALL PASSED` (the built or copied library, above);
2. `dotnet build DualFrontier.sln -c Release` -- 0 warnings, 0 errors;
3. the full suite per `TESTING_STRATEGY.md` section 8, its verdict read from the TRX files;
4. the Launcher dev smoke above -- exit 124 and a clean log;
5. `dotnet run --project tools/DualFrontier.Governance -- validate --armed` -- exit 0 (section 4).

The gates and their pass criteria are `CODING_STANDARDS.md` section 9; this list is their invocation order on the reference host.

**Per-commit duty by tier.** The chain above is the duty of a STAGE, not of every
commit. A DOCUMENT-tier commit -- its file set (`git diff --name-only <base>`) is only
Markdown documents, governance YAML under `docs/governance/` and the derived registers --
runs `validate --armed` (exit 0; `sync` first when a frontmatter block changed) and the
governance test project (`tests/DualFrontier.Governance.Tests`), nothing more. A
CODE-tier commit -- any other file -- runs the build (0W/0E), the test projects whose
assemblies the file set reaches, and `validate --armed`. A commit that touches
`native/`, MSBuild or CMake wiring (`*.csproj`, `*.props`, `*.targets`, `CMakeLists.txt`),
`global.json` or `Directory.Packages.props` is CODE tier and runs the FULL suite. The
whole chain above (selftest, build, full suite, dev smoke, validate) runs once per
stage: after a wave's last landing and before the push at closure. A red stage gate is
bisected over that stage's commits.

## §4 — Tooling reality

**Shell substrate:**

- The reference host is Linux with bash. .NET tooling runs from the user-local SDK (section 6). `pwsh` 7 exists but the `powershell-yaml` module is not installed, so no PowerShell governance script is a working command here, and no gate needs one.

**Governance tool** (`tools/DualFrontier.Governance`; `tools/governance/SCOPE_EXCLUSIONS.yaml` is its exclusion list):

- `dotnet run --project tools/DualFrontier.Governance -- validate --armed` -- reads the corpus and **writes nothing**. Armed, the semantic gates of `FRAMEWORK.md` section 14.5 are exit-affecting; exit 0 is the closure gate. Without `--armed` the semantic gates are report-only (`FRAMEWORK.md` section 14.8).
- `dotnet run --project tools/DualFrontier.Governance -- sync` -- validates, then regenerates the two DERIVED files `docs/governance/REGISTER.yaml` and `docs/governance/CURRENT_AUTHORITY_SURFACE.yaml`, writing only when content changed; a blocking finding stops it before any write. Run it after any frontmatter change and stage the regenerated files in the same commit; never hand-edit them.
- `-- migrate --target <scratch copy>` is the corpus-migration dry run; it is not part of routine work.
- No validate run leaves a tracked change behind, so there is no commit-folding obligation: `docs/governance/VALIDATION_REPORT.md` and `docs/governance/REGISTER_RENDER.md` are no longer written by anything.
- The walk is the FILESYSTEM, not the git index: every `*.md` under the repository root outside `.git`, `bin`, `obj`, `node_modules`, `.vs`, `.pnpm-store` and the exclusion list is in scope, and an in-scope `.md` with no frontmatter is a G-PATH error. Run artifacts, patches and reports that are `.md` files therefore live OUTSIDE the tree, or carry frontmatter if they are tracked.

**Other tools:**

- `tools/scaffold-runtime.ps1` -- idempotent runtime-directory materializer (PowerShell).
- `tools/glslangValidator.exe` + `tools/shaders/` -- shader compilation through the `CompileShaders` target in root `Directory.Build.props`, which runs only on Windows (`'$(OS)' == 'Windows_NT'`); elsewhere the committed `assets/shaders/*.spv` are the artifacts and are not regenerated.
- `tools/briefs/` -- Category D cascade briefs and closure reports (governance artifacts, not scripts).

## §5 — Branch and push policy

- **Roles:** the OPERATOR ratifies and merges; the ARCHITECT authors briefs and orchestrates the cascade through subagents; the INTEGRATOR (a subagent) is the only committer on the work branch, pushes it once at closure and opens the PR.
- **Push** (`CODING_STANDARDS.md` section 8.4): at closure the integrator runs `git push -u origin <branch>`, then `gh pr create --base main`. It never pushes to `main` and never merges the PR -- the merge is Crystalka's act, and it is where ratification happens. Expect an auto-mode re-confirmation on the push.
- **Merge** (`CODING_STANDARDS.md` section 8.3): A pull request lands on `main` as a two-parent merge commit made by the operator. Squash-merge and rebase-merge are forbidden: each replaces the authored sequence with commits nobody authored. Fast-forward-only is not required. Recorded exceptions, not precedent: PRs #55 and #56 reached `main` squash-merged (`17b3719`, `3000285`); history is not rewritten to repair them.
  The measure is `git log --first-parent main --format='%h %p %s'`: a landed PR shows two parent hashes.
- **Atomic commits** per `CODING_STANDARDS.md` section 8 -- one conceptual change per commit, structured body, no squash, no history rewrite. Surface revisions ride inside commits as `Skeleton revisions:` records per `RESERVED_SURFACE_MUTABILITY.md` section 5.
- **Branches kept unmerged:** `claude/cpp-core-experiment-cEsyH` (the original C++ kernel experiment; К-L source-of-truth reference, cited by KERNEL_ARCHITECTURE.md), `claude/godot-removal-deliberation-Vfg2R` (deliberation record) and `feat/m0-m3-mod-os-migration` (historical migration). Every other local branch is merged into `main` and is a pruning candidate tracked in the Findings ledger (F-11); `git branch --no-merged main` is the measure.

## §6 — Environment notes

- **OS:** Linux with bash -- the permanent development environment (operator ruling 2026-08-26).
- **.NET:** the SDK is user-local at `~/.dotnet` (10.0.400 on the reference host). The repo-root `global.json` pins `10.0.204` with `"rollForward": "latestFeature"`, which 10.0.400 satisfies. Before any `dotnet` command: `export DOTNET_ROOT="$HOME/.dotnet"; export PATH="$HOME/.dotnet:$PATH"`. Do not rely on a system or snap `dotnet` (on the reference host `/snap/bin/dotnet` is an installer stub). Projects target `net10.0` (LangVersion 14.0, root `Directory.Build.props`). The pin is the Q1-beta ruling recorded in `EVT-2026-07-17-STACK_UPDATE` (AUDIT_TRAIL.yaml).
- **Native toolchain:** cmake, ninja and gcc/g++ on `PATH` (section 3).
- **MSBuild node reuse:** `export MSBUILDDISABLENODEREUSE=1` in every shell that runs `dotnet build` or `dotnet test`. Persistent node-reuse MSBuild nodes inherit redirected pipes and hang tests that spawn child builds (M74BuildPipelineTests, F-48). The default shell does not set it; `scripts/setup-linux-sandbox.sh` sets it only on a provisioned sandbox. `dotnet build-server shutdown` releases the servers after a run.
- **Display:** the Launcher smoke and the display-gated tests need a reachable X display (the reference host runs XWayland, `DISPLAY=:0`). Without one the gated tests skip at discovery (`tests/DualFrontier.Runtime.Tests/TestInfrastructure/PlatformFacts.cs`), so the suite totals change; see `TESTING_STRATEGY.md` sections 2.6 and 8.
- **Known failure pattern (RISK-011):** a testhost lingering in shutdown holds an inherited stdout pipe, so a piped `dotnet test` blocks forever -- never pipe it (`TESTING_STRATEGY.md` section 8). Treat environmental noise as noise only if it does not reproduce with a clean process table.

## §7 — Godot status

Godot is **fully eradicated** — deprecated and physically purged at К-extensions cascade #2 (2026-05-23), with the residual inert file surface deleted at the Godot Eradication Cascade (2026-06-29; F-5 authorized + closed). Current state, verified against disk:

- **Zero Godot package or SDK references** in any `.csproj` or `Directory.Build.props` (repo-wide grep clean).
- Remaining `Godot` mentions in `src/**/*.cs` are retirement-notice / historical-context comments only on the reserved seams (e.g., the deprecation note in `src/DualFrontier.Application/Bridge/PresentationBridge.cs`) -- no code references (`rg --count-matches -i godot src --type cs`: 7 matches in 6 files at `54a9577`). They predate the comment law of `CODING_STANDARDS.md` section 6.2 and are brought to it when a landing edits their file (section 6.3 there).
- Historical Godot documentation is quarantined under `docs/architecture/historical/`.
- **The Godot file surface is gone.** `project.godot`, `icon.svg` / `icon.svg.import`, the 11 `assets/**/*.import` image sidecars, and the 204 `.cs.uid` resource-UID sidecars were deleted (the build stayed green, proving inertness); the Godot-specific `.gitignore` block and the `.godot/**` / `.gdignore` SCOPE_EXCLUSIONS entries were removed with them.

Rendering today is the in-house Vulkan substrate (`VULKAN_SUBSTRATE.md`) consumed by `DualFrontier.Launcher`; there is no engine dependency.

## §8 — Amendment protocol

This document is Category B, **Tier 1, LOCKED**. It states operational truth, so it amends whenever the truth moves:

1. **Trigger** — a verified change in repository layout, project set, build invocation, tooling behavior, branch policy, or environment.
2. **Rationale** — the commit (or cascade) that changed the underlying reality is named in the change-history entry.
3. **Semver** — PATCH for corrections within a section (counts, paths); MINOR for new sections or new tooling; MAJOR for a rewrite or an operational-model change.
4. **Register update** -- bump `version`, `last_modified` and the review fields in this document's own frontmatter, which is the source of truth (`FRAMEWORK.md` section 14.1); regenerate the derived register with `dotnet run --project tools/DualFrontier.Governance -- sync` and run `-- validate --armed` (exit 0); the cascade's closure record carries the audit-trail event.
5. **Cross-document propagation** -- `CODING_STANDARDS.md` section 9 (verification gates) and `TESTING_STRATEGY.md` section 8 (invocations) are re-checked whenever section 3, 4 or 6 here change.

Tier 1 LOCKED amendment ratification follows FRAMEWORK §7.2 (via the `PROJECT_AXIOMS.md` §5 chain).

## Change history

- **v3.0.1 (2026-11-22)** -- REV_2 (CORPUS_REVISION_1), the review repairs. `tools/governance/query_register.ps1` was
  retired in REV_2 (`git rm`; it needed the `powershell-yaml` module, absent on the reference host, and read a key
  the schema no longer carries). The repository map no longer lists it and names `LEDGER_FORM.yaml`, the
  G-ROWFORM configuration; the governance-tool list of section 4 drops its bullet. **PATCH** per section 8 rule 3
  (a path correction). `CODING_STANDARDS.md` section 9 and `TESTING_STRATEGY.md` section 8 were re-checked per
  section 8 rule 5 and name no retired script.
- **v3.0.0 (2026-11-21)** -- REV_1 (CORPUS_REVISION_1). Sections 1-2 are re-measured against the tree: a 57-project
  solution in which every project is a member (the six original vanilla mods and the 13 `Fixture.RegularMod_*`
  projects are no longer "disk-only"), with the counting commands stated. Section 3 is Linux-only and gains the
  Launcher dev smoke, the worktree rule for the native library and the local acceptance chain (no CI job runs the
  build, the suite or `validate --armed`; F-92). Section 4 describes `tools/DualFrontier.Governance` (validate reads
  only, sync regenerates the two derived files) and drops the retired PowerShell suite, the `VALIDATION_REPORT.md`
  rewrite and the commit-folding protocol. Section 5 states the roles and the merge law: a PR lands as a two-parent
  merge commit made by the operator, squash- and rebase-merge are forbidden, PRs #55 and #56 are the recorded squash
  exceptions, and the earlier fast-forward-only rule is inverted. Section 6 is the Linux environment; section 7
  recounts the Godot mentions and points at the comment law; section 8 rule 4 follows the schema-2.0 register
  protocol. **MAJOR** per section 8 rule 3 (operational-model change) and `CODING_STANDARDS.md` section 10.1 rule 3
  (an existing rule inverted). Section 3 also states the per-commit duty by tier: a document-tier commit runs
  `validate --armed` and the governance test project, a code-tier commit the build and the test projects it
  reaches, and the whole local chain runs once per stage.
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