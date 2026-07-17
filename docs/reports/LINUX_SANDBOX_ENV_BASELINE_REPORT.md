---
register_id: DOC-E-LINUX_SANDBOX_ENV_BASELINE_REPORT
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
title: LINUX SANDBOX ENV BASELINE REPORT — 2026-07-17 — cloud Linux sandbox provisioned for the STACK_UPDATE cascade; deviation register vs the Windows reference environment (D1-D8) + measured full baseline (build/tests/native selftest/armed validate)
special_case_rationale: 'Durable environment-baseline report enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-A_PRIME_9_1_PHASE_GAMMA_RECON_REPORT, DOC-E-BUS_INVESTIGATION_2026_05_21). Records the operator-sanctioned (2026-07-17 ruling) environment shift: the STACK_UPDATE cascade executes in the cloud Linux sandbox, not the LOCAL Windows machine the brief was authored against; the operator re-runs the gates on the home Windows machine post-merge.'
---

# Linux Sandbox Environment Baseline — 2026-07-17

**Operator ruling (2026-07-17, chat)**: the STACK_UPDATE cascade executes in the cloud
Linux sandbox ("мы тестируем код, а сам процесс на Windows или Linux отличаются лишь
сборками"); the operator re-runs the full gates on the home Windows machine afterwards.
This report records the environment delta so cascade Phase 0 grounds against the LINUX
baseline below instead of halting H1 on the Windows-shape mismatch.

Setup session branch: `claude/setup-dev-tools-s4ng3b`, base `bc93241` (= brief anchor).

## 1. Toolchain (installed and verified)

| Component | Sandbox (Linux) | Windows reference (recon) |
|---|---|---|
| OS | Ubuntu 24.04.4 LTS x64 | Windows 10/11 x64 |
| .NET SDK | **10.0.204** (pinned, dotnet-install) | 10.0.204 |
| .NET runtimes | 10.0.8 + **8.0.29** (for net8.0 test hosts) | 10.0.8 |
| CMake / generator | 3.28.3 / Unix Makefiles | 4.2.3 / Visual Studio 18 2026 |
| C++ compiler | gcc 13.3.0 (clang 18.1.3 fallback) | MSVC 14.50.35717 (v145) |
| Vulkan | headers/loader 1.3.275 + lavapipe (software 1.3) | SDK 1.4.350.0 + GPU |
| glslang | system 15.1.0 (NOT used for .spv — see D8) | bundled tools/glslangValidator.exe |

Provisioning is reproducible: `scripts/setup-linux-sandbox.sh` (idempotent; warm re-run
~4 s) + a `SessionStart` hook (`.claude/settings.json`) that runs it only when
`CLAUDE_CODE_REMOTE=true` — local Windows sessions are untouched.

## 2. Deviation register (Linux vs the Windows reference)

| # | Deviation | Disposition |
|---|---|---|
| D1 | `CompileShaders` target ran the committed Windows `glslangValidator.exe` → whole solution unbuildable on Linux | Target now conditioned on `'$(OS)' == 'Windows_NT'`; committed `assets/shaders/*.spv` are the Linux artifacts (commit `da9663c`) |
| D2 | 3 `WindowResizeEventTests` WM_SIZE tests were ungated `[Fact]` → `DllNotFoundException: kernel32.dll` (the F09 failure mode PlatformFacts documents) | Gated `WindowsOnlyFact` (commit `b9af14a`); Windows suite shape unchanged |
| D3 | Interop-test native delivery: the csproj `<None>` copy covers only the Windows `build/Release/*.dll` layout | `.so` discovered via loader: `ld.so.conf.d` entry + lib-prefixed symlink (setup script step 4); no csproj change |
| D4 | `Modding.Tests` M74BuildPipelineTests hung indefinitely: child `dotnet build` leaves persistent `nodeReuse` MSBuild nodes inheriting redirected pipes → the flush `WaitForExit()` never sees EOF (Linux-visible; Windows machines with prewarmed nodes don't hit it) | `MSBUILDDISABLENODEREUSE=1` exported session-wide by the setup script. RECOMMENDATION (operator to rule): harden `RunDotnetBuild` with `-nodeReuse:false` — F-ledger candidate |
| D5 | Test shape differs from the Windows reference 1166/0/5 | Linux baseline shape: **1048 passed / 0 failed / 116 skipped** (1164 entries). Delta fully accounted: 111 WindowsOnly skips (108 pre-existing + 3 from D2) + skipped-Theory row collapse (7 rows). Phase 0 of STACK_UPDATE grounds against THIS shape |
| D6 | Phase F C++23 flag: MSVC `/std:c++23preview` does not exist here | GNU/Clang emit `-std=c++23` (gcc 13 partial C++23; clang 18 fallback). The H-CXX emitted-flag check re-scopes to the GNU/Clang flag; the MSVC preview-flag verification re-runs on the operator's Windows machine |
| D7 | No GPU; Vulkan = lavapipe software device | Runtime GPU paths are WindowsOnly-skipped anyway; native lib links loader 1.3.275 fine (selftest green) |
| D8 | System glslang 15.1.0 ≠ bundled exe | Never invoked for `.spv` regeneration — committed artifacts stay byte-stable (D1) |

## 3. Measured baseline (2026-07-17, HEAD `a8e437f` + report commit)

- **Build**: `dotnet build DualFrontier.sln` — succeeded, **0 warnings / 0 errors** (TWAE).
- **Native**: CMake Release build clean; `df_native_selftest` — **ALL PASSED** (full scenario set).
- **Tests** (per assembly: passed/failed/skipped): Systems 2/0/0 · Persistence 6/0/0 ·
  ManifestRewriter 7/0/0 · Core.Interop **202/0/0** (native P/Invoke exercised) ·
  Application 45/0/0 · Runtime 175/0/111 · Governance **64/0/0** (net10.0) ·
  Analyzers 54/0/0 · Core 93/0/5 (the F-30/F-31 unconditional skips) · Modding 400/0/0.
  **Total: 1048/0/116.** The F-40 known flake did not manifest in this run.
- **Governance**: `validate --armed` — **exit 0** (339 documents; 0 errors; 0 gate findings;
  G-RATIO monitor 44.5% — standing deliberation, unchanged).

## 4. Operator checklist

1. Merge `claude/setup-dev-tools-s4ng3b`; future web sessions self-provision via the hook.
2. Re-run the full gates on the home Windows machine post-merge (D1/D2 are behavior-neutral
   there by construction; confirm 1166/0/5 + the 3 D2 tests now counted green-as-before).
3. Rule on the D4 recommendation (`-nodeReuse:false` in M74 `RunDotnetBuild`) — F-candidate.
4. STACK_UPDATE Phase 0 in this sandbox: baseline = section 3 shapes; H1/H-CXX ground
   against D5/D6 of this report.
