---
register_id: DOC-E-EQ_A4_RENDER_TAIL_CLOSURE_REPORT
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
title: EQ_A4_RENDER_TAIL closure report (Cascade D -- the EQ-a family CLOSER -- M6 swapchain prepare-before-reclaim + M9 device-lost v1 fail-fast under ratified D1)
---

# EQ_A4_RENDER_TAIL -- Closure Report

Cascade D of the EQ-a shutdown-law family: the render/device tail, and the LAST two
members. Pure managed render-stack scope (no native / ABI change). Landing this **closes
the EQ-a row end-to-end (M1-M9)**.

## 1. HEAD before / after

- **Before:** `15f4ed0` (main, EQ_A3 closure).
- **Branch:** `claude/eq-a4-render-tail` off that HEAD.
- **After:** the C7 closure-append commit (this report + AUDIT_TRAIL EVT + brief EXECUTED).
  NOT pushed -- the operator pushes `claude/eq-a4-render-tail` + merges.

## 2. Per-commit hashes

| Commit | Hash | What |
|---|---|---|
| C1 enroll | `70b5ac8` | Brief (Draft) enrolled; REGISTER +1 doc; sync; validate --armed exit 0. |
| C2 M6 impl | `f28b3f4` | `PrepareBeforeReclaim.Build` primitive + `VulkanSwapchain.Recreate` / `Runtime.RecreateFramebuffersForSwapchain` -> prepare->commit->reclaim; both configs 0W/0E. |
| C3 M6 test | `ae4dc48` | `PrepareBeforeReclaimTests` (4 device-free cases); full-sln 1191 -> 1195. |
| C4 M9 impl | `41cdef4` | `DeviceLostException`/`DeviceLostContext`/`DeviceLostDiagnostic` + `DeviceLost.ThrowIfLost` classifier + `DeviceLossBoundary` + 5-site detection + `Program.cs` wiring; both configs 0W/0E. (Amended in-branch to reword one redundant "deferred" -> "future work" keeping the deferred-marker census at 86.) |
| C5 M9 tests | `e40e028` | `DeviceLostTests` (10 device-free cases via the OnDeviceLost recorder seam); full-sln 1195 -> 1205. |
| C6 governance | `315191a` | ELT 1.0.1->1.0.2, IAC 1.1.0->1.1.1, VULKAN 1.0.3->1.0.4 (all PATCH); ROADMAP EQ-a CLOSED + F-52/F-53; sync; validate --armed exit 0. |
| C7 closure | (this commit) | AUDIT_TRAIL EVT; brief -> EXECUTED; this report; sync; validate --armed. |

## 3. M6 -- swapchain recreation order: BEFORE -> AFTER (ELT §2.5)

Quiesce (`VulkanDevice.WaitIdle`) is unchanged in both. The transaction changes:

| Step | BEFORE (reclaim-before-prepare) | AFTER (prepare-before-reclaim) |
|---|---|---|
| Image views | `DestroyImageViews` (old) **then** `CreateSwapchain` builds new views | `PrepareSwapchain` builds new swapchain + views into **locals** (old alive, fed as `oldSwapchain` hint) |
| Swapchain handle | new created with old as hint; old destroyed after (already prepare-before-reclaim) | unchanged (part of the same PREPARE); old destroyed only in RECLAIM |
| Commit | implicit (fields written mid-create) | explicit infallible `Commit` -- one field assignment, no vk calls |
| Reclaim | -- | `DestroyImageViews(oldImages)` + `vkDestroySwapchainKHR(old)`, only after commit |
| Framebuffers | dispose all old + clear list **then** build new (`Runtime.cs`) | build new list (candidate) -> swap list (commit) -> dispose old (reclaim) |
| Failure semantics | mid-rebuild throw -> **zero framebuffers, no rollback** (violated ELT §1.1 corollary 1) | mid-prepare throw -> partial new work rolled back by `PrepareBeforeReclaim.Build`; **old set intact, no rollback gap** |

Both sites reuse `PrepareBeforeReclaim.Build<TSource,T>(sources, build, reclaim)`: it builds a
candidate array, and on any build failure reclaims exactly what it built (construction order)
and rethrows with **no array returned** -- so the caller's commit is unreachable and old state
is untouched. Files: `src/DualFrontier.Runtime/Graphics/{PrepareBeforeReclaim,VulkanSwapchain}.cs`,
`src/DualFrontier.Runtime/Runtime.cs`.

## 4. M9 -- device-lost site inventory (D1: fail-fast + structured diagnostic, no recovery v1)

`VK_ERROR_DEVICE_LOST` (`VkEnums.cs:14`) is now classified at **5** sites (recon under-counted 4).
Each adds one `DeviceLost.ThrowIfLost(result, context)` before its own generic-error handling
(device-lost is distinct from every other classified result, so the generic path is unchanged):

| # | vk call | Site | BEFORE | AFTER |
|---|---|---|---|---|
| 1 | `vkQueueSubmit` | `VulkanCommandBuffer.SubmitTo` | throw `InvalidOperationException` | `ThrowIfLost(QueueSubmit)` -> `DeviceLostException`, else generic |
| 2 | `vkQueuePresentKHR` | `VulkanSwapchain.Present` | throw | `ThrowIfLost(QueuePresent, this)` (+ swapchain state) |
| 3 | `vkAcquireNextImageKHR` | `VulkanSwapchain.AcquireNextImage` | throw | `ThrowIfLost(AcquireNextImage, this)` (+ swapchain state) |
| 4 | `vkWaitForFences` | `VulkanFence.Wait` | throw | `ThrowIfLost(WaitForFences)` |
| 5 | `vkDeviceWaitIdle` | `VulkanDevice.WaitIdle` | **VkResult DISCARDED** (no check) | capture result + `ThrowIfLost(DeviceWaitIdle)` |

**Boundary:** `DeviceLossBoundary.RunGuarded(frameIndex, frame)` (wired at `Program.cs` around the
per-frame `renderer.RenderFrame`) catches `DeviceLostException`, composes the structured
`DeviceLostDiagnostic` (failed call + loop-owned frame index + swapchain state) and routes it to
`Environment.FailFast(diagnostic.Describe(), ex)` -- a line-for-line analog of `EngineSession.Abort`
(К-L20). No recovery in v1: device re-creation is a separate epic.

**Threading:** the wrappers carry only what they own (site + swapchain extent); the loop-owned
frame index is composed at the boundary. No AsyncLocal, no signature pollution.

**v1 edge (documented, not a gap):** a device-lost surfaced from a **teardown-path** `WaitIdle`
(`Runtime.Dispose` / `LauncherRenderer.Shutdown`) is outside the render-loop boundary and
terminates without the OnDeviceLost path -- acceptable per D1 (crash with diagnostic; the message
still names device-lost). `WaitIdle` gets device-lost classification ONLY (a generic throw there
would inject a new failure into Dispose unwinding); the residual is F-53.

## 5. Injection-test evidence (device-free; H-SEAM resolved by "factor for device-free tests")

- **C3 -- M6** `PrepareBeforeReclaimTests` (4 `[Fact]`): happy path returns candidates in order and
  never reclaims; a mid-build throw rolls back exactly the built resources in construction order
  and returns no array (commit unreachable -> old state untouched); a first-build throw reclaims
  nothing; empty sources build/reclaim nothing. Real-handle leak-clean deferred to §8 (real-GPU).
- **C5 -- M9** `DeviceLostTests` (10 cases): classifier `[Theory x5]` (throws only for
  `VK_ERROR_DEVICE_LOST`; SUCCESS / OUT_OF_DATE / SUBOPTIMAL / OUT_OF_DEVICE_MEMORY do not);
  classifier carries the site context; `RunGuarded` + injected `OnDeviceLost` recorder asserts the
  composed diagnostic (site + frame index + swapchain state) **without a real `Environment.FailFast`**
  (mirrors the EngineSession OnAbort seam); happy path never records; a non-device-lost exception
  propagates; a null frame throws. The production null-hook (real fail-fast) path is intentionally
  not unit-testable -- covered by §8.

Seam note: `VkResult` is internal (visible via `InternalsVisibleTo`), but a public xUnit method
cannot expose it (CS0051), so the classifier `[Theory]` passes the raw int and casts inside.

## 6. EQ-a family completion map (M1-M9 -> cascade -> commit)

| Member | Work | Cascade | Commit(s) |
|---|---|---|---|
| M1 | `ExecutePhase` per-system catch + origin dispatch + ELT §2.3 quarantine skip-set | EQ_A1 | `4dae07b` |
| M4 | `DomainEventBus` deferred/sync catch symmetry + fault-sink | EQ_A1 | `c72d3b9` |
| M2 | world-shutdown transaction (quiesce->fence->teardown + fail-fast abort); `UnloadAll` first caller; deterministic `NativeWorld.Dispose` | EQ_A2 | `f207f02` |
| M3 | `EngineSession : IDisposable` composition root | EQ_A2 | `f207f02` |
| M7 | Degraded/EngineHealth surface (ELT §4.1) + `ModQuarantine`->Degraded link | EQ_A2 | `1e07c2d` |
| M8 | `df_bus_clear` promoted to production teardown | EQ_A2 | `f207f02` / `e733797` |
| M5 | checked-destroy ABI pair + `NativeWorld.DisposeChecked` + S7 switch | EQ_A3 | `7bc4e07` / `31dfb26` |
| **M6** | **swapchain prepare-before-reclaim (ELT §2.5) + failure-injection test** | **EQ_A4** | **`f28b3f4` / `ae4dc48`** |
| **M9** | **device-lost v1 fail-fast (D1 / ELT OQ-3 / IAC §7-7)** | **EQ_A4** | **`41cdef4` / `e40e028`** |

## 7. Final gates (F5 shape + deltas)

| Gate | Baseline (15f4ed0) | At closure | Delta |
|---|---|---|---|
| Build Release / Debug | 0W / 0E | 0W / 0E | -- |
| Full-sln `dotnet test -c Release` | 1191 / 0 / 5 | **1205 / 0 / 5** | +14 (M6 +4, M9 +10) |
| Native selftest (both configs) | 104 ALL PASSED | 104 ALL PASSED | -- (no native change) |
| `validate --armed` | exit 0 | exit 0 | -- |
| Console.WriteLine src `.cs` | 2 | 2 | -- (diagnostic via `Environment.FailFast`, never console) |
| DFK-WAIVER | 2 = 2 | 2 = 2 | -- (no new suppressions) |
| `Environment.FailFast` src calls | 1 | **2** | +1 (the M9 boundary; expected, not a pinned census) |
| BoundaryRatchet | 4 + 1 | 4 + 1 | -- |
| Marker censuses (stub/deferred/TODO/not-yet) | 51/86/136/10 | 51/86/136/10 | -- (deferred stayed 86 after the C4 reword) |
| Register (in-scope docs) | 355 (pre-brief) | 357 | +2 (brief C1 + this report C7) |

Doc version bumps (all C6): ELT 1.0.1 -> 1.0.2 PATCH; IAC 1.1.0 -> 1.1.1 PATCH;
VULKAN_SUBSTRATE 1.0.3 -> 1.0.4 PATCH; ROADMAP is a Live ledger (no version). No KERNEL change
(K-L19/K-L20 cited, not amended; no new invariant).

## 8. Real-GPU evidence (operator machine, RX 7600S)

Automated launch-and-close of the Release `DualFrontier.Launcher` from the repo root:
- window created (`CloseMainWindow` found a top-level window -> swapchain **creation** via the
  refactored `PrepareSwapchain` ctor path succeeded), graceful WM_CLOSE shutdown (**not**
  force-killed), **exit code 0**, no stdout/stderr errors.
- **Covers:** swapchain creation (M6-refactored ctor path) + render loop (with the M9
  `DeviceLossBoundary` wrap) + clean shutdown, on real К-L19 hardware.
- **NOT covered by the automated run (operator touchpoint):** swapchain **recreation** fires only
  on a window resize / `VK_ERROR_OUT_OF_DATE_KHR`, and the visual render confirmation needs eyes.
  The fuller check -- resize the window under validation layers (DEBUG), confirm clean recreation +
  zero validation errors + visual render ("dots move") -- remains an operator confirmation, matching
  the prior-smoke precedent. The C3 device-free test proves the recreation transaction *ordering*.

## 9. Register / EVT deltas

- REGISTER: +2 docs net -- the brief (C1) + this closure report (C7); 355 -> 357 in-scope.
  `CURRENT_AUTHORITY_SURFACE.yaml` moved at C6 (the three LOCKED tier-1 A-doc version bumps + the
  Live ROADMAP). `last_modified_commit` removed from VULKAN + ROADMAP (real-hashes-only; the C6/C7
  hash is unknowable pre-commit; EQ_A3 precedent for IAC/ROL).
- AUDIT_TRAIL: +1 EVT (`EVT-2026-07-18-EQ_A4_RENDER_TAIL`), append-only (prior entries
  byte-unchanged).
- New F-ledger rows: **F-52** (`_renderFinishedPerImage` recreation sizing) + **F-53**
  (`vkDeviceWaitIdle` non-device-lost result discard) -- both RECORDED, not fixed. **F-51** untouched.

## 10. Attestation

M6 + M9 executed to the ratified D1 policy and the ELT §2.5 letter under the "factor for
device-free tests" seam (no interfaces over concrete wrappers, no production test hooks). All
gates green at closure; every frontmatter-touching commit (C1, C6, C7) passed `validate --armed`
exit 0. No halts fired (H-SCOPE cleared: the recreation gap was exactly views+framebuffers;
H-ABI cleared: no native change; H-LAW cleared: no brief-vs-LOCKED conflict; H-SEAM pre-resolved
by the ratified seam strategy). One in-branch amend (C4) reworded a redundant "deferred" to keep
the marker census pinned; recorded here for transparency. **No push (executor); the operator
pushes `claude/eq-a4-render-tail` and merges.** The EQ-a shutdown-law family is CLOSED end-to-end.
