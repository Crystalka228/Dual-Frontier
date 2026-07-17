---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-CODEX_REVIEW_REMEDIATION_20260714
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.1"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-CODEX_REVIEW_REMEDIATION_20260714
---
# Codex Review Remediation — closed-PR findings cascade — 2026-07-14

> Verification + remediation of the 21 Codex inline review comments inventoried across the 42
> closed PRs (see the companion audit **CODEX_REVIEW_INVENTORY_20260714**). Executor: Claude Code
> (Opus), Claude-Code-on-the-web remote session (Linux container). Base: `main` @
> [`c8fa0e58`](https://github.com/Crystalka228/Dual-Frontier/commit/c8fa0e58b85d65b0484d842449522f06abc145b8)
> — the exact commit the inventory was verified against. Fixes developed on branch
> `claude/closed-pr-accessibility-2yxtu8`. Every finding was **independently re-verified against the
> source** before any change; the inventory held on all 21. This report is the closure record for
> that cascade. Bez kostylei — the two local build gates used to compile Runtime / run tests on this
> toolchain-limited host were reverted before commit and are **not** part of the diff.

---

## 1. Result

> **Amended 2026-07-14** -- the outcome arithmetic in this section and the CX-14 outcome are corrected by the Amendment section at the end of this report.

All 21 Codex findings were independently re-verified, then remediated to the extent the runtime
environment allows. **16 findings changed code/docs; 1 (F07) is documented as accepted-latent; 4 are
confirmed already-closed and left untouched (F10, F12, F16, F17, F18 — 5 items).** F20 was confirmed
architecturally closed with existing regression coverage; no change was required.

| Metric | Value |
|---|---:|
| Findings verified | 21 |
| Fixed in this cascade | 16 |
| Confirmed already-closed (no change) | 5 (F10, F12, F16, F17, F18) |
| Architecturally closed, existing coverage (no change) | 1 (F20) |
| Documented as accepted-latent | 1 (F07) |
| Files changed | 51 (+560 / −197) |
| New regression tests | F03, F19 (×2), F21 (×2) |

Independent verification agreed with the inventory on **every** finding, including its two subtle
calls: F20 (architecturally closed) and F07 (latent, no x86 target). The one GitHub-`resolved` thread
the inventory flagged as still-open in code — **F13** — was confirmed open and fixed.

## 2. Environment & tooling

This is a Linux remote session; the project targets `net8.0` with a Win32 + Vulkan Runtime and a
C++ native core. What was available / done, per the "verify .NET availability first" instruction:

- **.NET SDK** — not preinstalled; installed `dotnet-sdk-8.0` (target framework) and, because the
  repo's analyzer pins `Microsoft.CodeAnalysis.CSharp 5.3.0` (a Roslyn newer than any SDK here ships),
  also `dotnet-sdk-10.0`. Neither ships Roslyn 5.3, so `DualFrontier.Analyzers` cannot load in this
  environment regardless of SDK — a **toolchain limitation, not a code defect**. The project's real
  CI must use a Roslyn ≥5.3 toolchain.
- **glslang** — installed `glslang-tools`; used to regenerate `sprite.frag.spv` for F04 (the
  committed SPIR-V, since the in-repo `glslangValidator.exe` is a Windows binary).
- **Stress tests** — **not run**, per instruction. The scheduler stress/extreme suites were excluded
  from every test invocation.
- **Not available here:** a Vulkan SDK (so the native `.so` cannot be built — `find_package(Vulkan
  REQUIRED)` fails), a GPU, Windows, and PowerShell + `powershell-yaml` (so the register
  `sync_register.ps1` / `render_register.ps1` tools could not be run — see §6).
- **Local-only build gates (reverted before commit):** to compile the Runtime project and run the
  managed test subsets on this host, the Windows-only shader step and the un-loadable analyzer
  reference were temporarily gated behind `DFSkipShaders` / `DFSkipAnalyzers` MSBuild properties.
  Both `Directory.Build.props` files were restored to their original state before commit; **no gate
  appears in the diff.**

## 3. Verification & remediation — all 21 findings

Verdict = independent re-verification result. Verified-by = how the fix was confirmed here
(runtime-tested / compile-verified / SPIR-V-regenerated / doc/YAML-checked / static — where the
platform prevents execution).

| ID | Area | Verdict | Action | Verified by |
|---|---|---|---|---|
| F01 | Runtime `RecordSpriteFrame` discards `mvp` | CONFIRMED | Threaded caller `mvp` through a new explicit-MVP `RecordSpritesFrame`/`EndFrame` overload; shim now honours it (identity reproduces old smoke behaviour) | compile-verified |
| F02 | Sprite `VertexBufferRing` chunk overwrite | CONFIRMED (deterministic) | Fail-fast guard: reusing the same ring slot for a second unsubmitted batch now throws with the resolution, instead of silently overwriting | compile-verified |
| F03 | `AssetManager` path traversal on case-sensitive FS | CONFIRMED | Containment check is now OS-aware (`Ordinal` off-Windows, `OrdinalIgnoreCase` on Windows) | **runtime-tested** (new test, Linux) |
| F04 | Sprite straight-vs-premultiplied alpha | CONFIRMED | `sprite.frag` now premultiplies RGB by alpha to match the `ONE/ONE_MINUS_SRC_ALPHA` blend; `sprite.frag.spv` regenerated | **SPIR-V regenerated** (glslang) |
| F05 | Win32 `WM_SYSKEYDOWN/UP` swallowed | CONFIRMED | System keys split into their own arms that record the key **and** return `DefWindowProc` (Alt+F4 / Alt+Space / F10 restored) | compile-verified |
| F06 | Vulkan present-capable queue unvalidated | CONFIRMED | After surface creation, `vkGetPhysicalDeviceSurfaceSupportKHR` is now called and a non-present-capable graphics family fails fast (full present-family *selection* noted as follow-up) | compile-verified |
| F07 | Vulkan callback calling convention (x86) | PARTIAL (latent) | **Documented as accepted-latent** — no project targets x86 (every x86 sln config maps to Any CPU/x64 where the ABI is identical). Harden to `Winapi` only if a real Win32-x86 target is ever added | n/a (latent) |
| F08 | Win32 window: no cleanup on failed init | CONFIRMED | `InitializeWin32` wraps the acquisition sequence; a throw now rolls back the pinned `GCHandle`, the class registration, and the window | compile-verified |
| F09 | Runtime tests unconditionally construct Win32/Vulkan | CONFIRMED | Added `[WindowsOnlyFact]`/`[WindowsOnlyTheory]` (skip off-Windows at discovery); applied to the 22 GPU/Win32-instantiating test files (108 methods) | compile-verified + skip observed |
| F10 | `CLEANUP_CASCADE_BRIEF` lifecycle render drift | CONFIRMED CLOSED | none | source+render agree |
| F11 | `ISOLATION.md` overclaims analyzer enforcement | PARTIAL | Softened the opening to future tense ("**will extend**… once its detection logic lands"), matching §3 / the runtime-guard note; no call-site rule exists yet (17 shipped rules, none check `[SystemAccess]` vs `AcquireSpan`/`BeginBatch`) | doc; version 1.1.1→1.1.2 |
| F12 | `REGISTER_RENDER.md` stale | CONFIRMED CLOSED | none | render header current |
| F13 | Risk register cites removed doc IDs | CONFIRMED OPEN (mis-`resolved`) | RISK-004 + RISK-013 `affected_documents` / `mitigation_artifact` / title repointed `DOC-A-RUNTIME` + `DOC-A-GPU_COMPUTE` → `DOC-A-VULKAN_SUBSTRATE` (deduped) | YAML-checked |
| F14 | Native lib copy is Windows-only (`.dll` only) | CONFIRMED (all 9 consumers) | Added shared `NativeArtifactCopy.props` (Linux `.so` + macOS `.dylib`, `Exists`-gated) imported by all 9 consumers | inspection (`.so` unbuildable here — no Vulkan SDK) |
| F15 | Capability validation bypassed in production | CONFIRMED | `ModIntegrationPipeline` now passes the already-built `_kernelCapabilities` registry into load-time `Validate` (was `null`, silently skipping Phase C/D) | **runtime-tested** (capability suite intact) |
| F16 | `apiVersion` now enforced | CONFIRMED CLOSED | none | existing regression tests |
| F17 | Capability registry scans all kernel assemblies | CONFIRMED CLOSED | none | existing regression tests |
| F18 | `PHASE_1.md` METHODOLOGY link | CONFIRMED CLOSED | none | target exists |
| F19 | Event bus uses `ReferenceEquals` for handlers | CONFIRMED | `Subscribe`/`Unsubscribe` now match by delegate **value** equality (`Delegate.Equals`), so method-group handlers dedupe/unsubscribe correctly | **runtime-tested** (2 new tests) |
| F20 | Static `ModeCache` vs collectible ALC | REFUTED (arch. closed) | No change — regular mods are barred from exporting `IEvent`; existing `ContractTypeInRegularModTests` (5 tests) confirm the invariant | existing tests |
| F21 | `TerrainKind.Unknown` not the zero value | CONFIRMED | Explicit enum values with `Unknown = 0` (so `default`/zero-init reads uninitialized, not Grass); pins the persisted RLE byte; `SaveMeta.Version` 1→2 marks the layout change | **runtime-tested** (2 new tests) |

### 3.1 Notes on the harder fixes

- **F02** — the ring is sized for exactly one batch per swapchain image and selects its chunk by
  `frameIndex % frameCount` (an intentional cross-frame invariant, asserted by existing tests). The
  `>10K sprites → multiple BeginFrame/EndFrame cycles` path reused one chunk and silently corrupted
  every cycle but the last. A safe in-place ring redesign is not verifiable without a GPU, so the fix
  converts the silent corruption into an immediate, clearly-messaged exception and documents the two
  resolutions (raise `maxSpritesPerFrame`, or submit+present between batches). A larger-capacity ring
  is a GPU-validated follow-up.
- **F06** — validation was added without reordering device/surface creation (the invasive,
  GPU-only-testable part). The graphics family is now *checked* for present support instead of
  *assumed*; selecting a distinct present family remains a documented follow-up.
- **F04** — the fragment shader now premultiplies; the committed `sprite.frag.spv` was regenerated
  with Linux `glslangValidator` (byte-for-byte the build's `-V` invocation). On Windows CI the build
  target regenerates it identically.

## 4. Build & test evidence

- **Full managed solution:** builds with **0 errors** (`TreatWarningsAsErrors=true`) once the
  un-loadable analyzer is gated off for this host. `DualFrontier.Runtime` compiles clean, so the
  F01/F02/F03/F05/F06/F08 Vulkan/Win32 fixes are compile-verified.
- **Runtime.Tests:** compiles clean; the 108 gated methods report **skipped** (not errored) on Linux
  — F09 behaviour confirmed.
- **Managed regression runs (stress excluded):**
  - `TileEncoderTests` (F21): **3/3 passed**.
  - `AssetManagerTests` (F03): **10/10 passed** — the case-variant escape is rejected on Linux.
  - `MethodGroupSubscriptionTests` (F19): **2/2 passed**.
  - Modding capability + `ContractTypeInRegularMod` suites (F15/F20): **passed**.
- **Pre-existing native-lib failures are unrelated to this cascade.** Tests that construct the native
  world fail on this host with `DllNotFoundException: DualFrontier.Core.Native` because the C++ `.so`
  cannot be built without a Vulkan SDK. These fail identically on the unmodified base; they are the
  real-world symptom of F14, not regressions.

## 5. Deferred / follow-up items

1. **F07** — apply the `Winapi`/`stdcall` calling convention only if/when a real Win32-x86 target is
   added; latent today.
2. **F02 / F06** — GPU-validated follow-ups: a larger sprite ring (or per-cycle submit) for genuine
   `>maxSpritesPerFrame` scenes; present-family *selection* (not just validation).
3. **F14** — the `.so`/`.dylib` copy is `Exists`-gated and correct, but was verifiable only by
   inspection here (no Vulkan SDK to build the native artifact); confirm on a Linux CI that builds it.
4. **F20 residual (defense-in-depth, optional)** — `ValidateRegularModContractTypes` scans only
   `GetExportedTypes()`; a non-public `internal IEvent` in a regular mod would slip past. Only
   theoretical (the bus is `internal` to Core, so a regular mod has no sanctioned publish path).

## 6. Register actions

- **Applied to source of truth (`REGISTER.yaml`):** F13 risk-reference repointing; DOC-A-ISOLATION
  version 1.1.1→1.1.2 + `last_modified`; this report enrolled as `DOC-E-CODEX_REVIEW_REMEDIATION_20260714`
  (Category E, Tier 3, EXECUTED).
- **Pending the PowerShell tooling** (unavailable on this Linux host — `sync_register.ps1` requires
  `powershell-yaml`, mirroring the repo's Windows-tool workflow): regenerate `REGISTER_RENDER.md`
  (new entry + document count + register-version bump), sync the frontmatter mirrors, and backfill
  `last_modified_commit` for the touched entries. These are derived artifacts; the YAML source is the
  authority and is already updated.

## 7. Limitations

Fixes were verified by the strongest means each finding allows on a Linux, no-GPU, no-Windows,
no-native-lib host: managed logic fixes are **runtime-tested**; Vulkan/Win32 fixes are
**compile-verified** against a clean `TreatWarningsAsErrors` build plus static trace; the shader fix
is **SPIR-V-regenerated**; portability/test-infra fixes are **inspection-verified**. Hardware- and
Windows-specific behaviour (F02/F04/F06 on a GPU; F05/F08 Win32 message flow) warrants a confirming
pass on the target platform, and the native `.so`/`.dylib` copy (F14) warrants confirmation on a
Linux/macOS CI that builds the native artifact.

---

## 8. Amendment -- 2026-07-14 (v1.1)

Appended per the closure-integrity convention (an EXECUTED report is corrected by
amendment, never by rewriting its body). Three corrections and one namespace ruling.

### 8.1 Finding-series namespace

To end the collision with the repository's own ROADMAP F-ledger (F-1..F-32), the 21
findings of this report are hereafter referenced as **CX-01..CX-21** (CX-NN = this
report's FNN). The body above retains its original FNN labels as published history.

### 8.2 Corrected outcome arithmetic

Section 1 above states 16 + 5 + 1 + 1 = 23 outcome slots for 21 findings. The
correct, mutually-exclusive split per this report's own section-3 table:

| Outcome | Count | Findings |
| --- | ---: | --- |
| Changed code/docs in this cascade | 14 | CX-01..06, 08, 09, 11, 13, 14, 15, 19, 21 |
| Confirmed already-closed, no change | 5 | CX-10, 12, 16, 17, 18 |
| Architecturally closed, existing coverage, no change | 1 | CX-20 |
| Accepted-latent, no change | 1 | CX-07 |

Verification strength of the 14 changed items (from the section-3 verified-by
column): runtime-tested 4 (CX-03, 15, 19, 21); SPIR-V-regenerated 1 (CX-04);
compile-verified 6 (CX-01, 02, 05, 06, 08, 09); doc/YAML-checked 2 (CX-11, 13);
inspection-only 1 (CX-14). Outcome category and verification strength are distinct
axes and are not conflated hereafter.

Definitive diff stat, computed from git at amendment time: 52 files, +742/-197,
over the single squashed merge commit `61f08ef` (range `c8fa0e5..61f08ef`). Scope:
the count includes this report file (`docs/audit/CODEX_REVIEW_REMEDIATION_20260714.md`,
+165, created in the commit) and `docs/governance/REGISTER.yaml` (+30/-7, the DOC-E
enrollment plus the F13 risk-reference and DOC-A-ISOLATION edits); there is no
separate frontmatter-mirror file -- the report's inline frontmatter is its mirror.
The section-1 self-count of 51 files / +560 excluded this report file and the
register artifacts.

### 8.3 CX-14 outcome correction (was counted "fixed"; actual: structural no-op, now retired)

The CX-14 change (`NativeArtifactCopy.props`) could never copy anything:
`CMakeLists.txt` sets `PREFIX ""`, so the non-Windows artifacts are
`DualFrontier.Core.Native.so` / `.dylib` WITHOUT the `lib` prefix, while the props
searched exclusively `lib`-prefixed names; `Exists`-gating turned the mismatch into a
silent no-op. Verified by line-read 2026-07-14. Under the operator ratification of
the same date -- **the product is Windows-only per L7**; Linux is a limited host for
managed unit tests only -- the props is RETIRED in the CODEX_CLOSURE cascade rather
than repaired. CX-14's outcome is reclassified: not "fixed" but
"retired-out-of-scope (Windows-only), original change was inoperative".

### 8.4 Standing accepted records

- **CX-07** (Vulkan callback calling convention on x86): accepted-latent -- no
  project targets x86; harden to `Winapi` only if a real Win32-x86 target is added.
- **CX-20 residual** (`ValidateRegularModContractTypes` scans `GetExportedTypes()`
  only; a non-public `internal IEvent` in a regular mod would slip past):
  accepted-theoretical -- the bus is `internal` to Core, so a regular mod has no
  sanctioned publish path. Re-open on demand as defense-in-depth.
- **CX-02 + CX-06** GPU-validated follow-ups: tracked as ROADMAP **F-32** (OPEN).
