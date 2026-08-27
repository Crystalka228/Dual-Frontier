---
register_id: DOC-D-LINUX_PRESENT_1_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-08-26'
last_modified: '2026-08-26'
content_language: en
next_review_due: null
title: 'LINUX_PRESENT_1 -- the Launcher opens, renders, and closes on the permanent Linux environment: per-platform Vulkan loader resolution, platform-selected instance surface extension, a platform-neutral windowing seam (surface creation behind the window) with an XCB backend over XWayland, capability-probing test gates replacing the 111 WindowsOnly skips (71 loader-only + 40 display), the Launcher native .so arm, and the live Launcher smoke that pays the debt owed since W3/PR #51'
authored_by: Claude Fable (architect session, LINUX_PRESENT_1 charter)
basis: 'DOC-E-LINUX_PRESENT_1_RECON_REPORT (Opus subagent, read-only, 2026-08-26 at 683b343, sections A-H + 6 discrepancies + 7 surprises) + the architect-seat scope measurement 2026-08-26 (Launcher DllNotFoundException repro at Window.cs:46) + ratified lean set 2026-08-26 (Q1 XCB-first; Q2 surface-behind-window + runtime factory; Q3 loader resolver + platform extension pick; Q4 two capability attributes; Q5 Launcher smoke as closure gate; Q6 scope fences) + operator environment ruling 2026-08-26 (Linux PERMANENT)'
---

# LINUX_PRESENT_1 -- Execution Brief

This cascade makes the presentation path real on the permanent Linux environment.
Today the Launcher dies at `Window.cs:46` on `kernel32.dll`; behind that death sit
three more blockers the recon measured (the hard-coded `vulkan-1.dll` library name
with no resolver, the unconditional `VK_KHR_win32_surface` assertion, and the
Launcher's missing native `.so` copy arm). The cascade removes all four in firing
order, re-contracts the windowing seam so surface creation lives behind the window
implementation, ships an XCB backend over XWayland, replaces the 111
`WindowsOnlyFact` skips with capability-probing gates that flip ALL of them on this
box, and closes with the live Launcher smoke owed since W3/PR #51. Done = the
Launcher opens a window, renders, and shuts down cleanly on this machine; the
Runtime test suite runs with zero WindowsOnly skips; Windows behaviour is
byte-equivalent.

Executor: fresh Claude Code session (flagship model), LOCAL, repository
`/mnt/Work/Colony_Simulator` (GitHub `Crystalka228/Dual-Frontier`). MANAGED-ONLY:
the `native/` tree is byte-untouched (H7). The cascade does NOT touch
`Input/VirtualKeyMapper.cs` (see section 15).

Brief-integration notice: this brief CITES standing law and does not restate it --
commit-body structure per `CODING_STANDARDS.md`; push law per `CODING_STANDARDS.md`
section 8.4 (v3.0.0); census pins per `TESTING_STRATEGY.md`; mutability license per
`RESERVED_SURFACE_MUTABILITY.md`; session closure per `METHODOLOGY.md`. A conflict
between this brief and any standing doc means THE BRIEF IS WRONG -- halt and
escalate. A conflict between this brief and the live code means THE CODE IS THE
TRUTH -- record and proceed per section 2's re-verify discipline.

## 1. Mission [CORE]

| #  | Deliverable | Action |
| -- | ----------- | ------ |
| D1 | This brief + the recon report | enroll (frontmatter + sync); brief Draft -> EXECUTED at closure |
| D2 | Vulkan loader portability | per-platform library-name resolver under `Runtime.Native.Vulkan`; no P/Invoke rename |
| D3 | Instance extension portability | surface extension selected per platform (win32 <-> xcb); fail-fast preserved |
| D4 | The windowing seam | `IWindow` re-contracted platform-neutral (surface creation BEHIND the window; `Handle` retired); Win32 arm adapted with byte-equivalent behaviour; resize decode extracted pure |
| D5 | XCB backend | `Runtime.Native.Xcb` P/Invokes + `XcbWindow` + xcb Vulkan surface + xkbcommon-x11 key mapping (new mapper, own tests) |
| D6 | Test gates | `WindowsOnlyFact`/`Theory` (111 sites) replaced by two capability-probing attributes along the measured 71 loader-only / 40 display split; `WindowResizeEventTests` rewritten against the pure decode (ungated) |
| D7 | Launcher runnable | Launcher csproj gains the Linux `.so` copy arm; LIVE Launcher smoke on this box = closure gate (the W3/PR #51 debt) |
| D8 | Doc + ledger truth | MODULE.md / VULKAN_SUBSTRATE count drift fixed; dead surface trimmed (RECT, WM_CREATE, SendMessage); F-rows seeded (Wayland-native backend, headless path, Resizable dead property); ROADMAP write-back; closure EVT |

Why now: ratified program order (F-60(a) closed at PR #52; LINUX_PRESENT_1 second).
Every future cascade's gates and the whole UI program land on this seam.

## 2. Established facts [CORE]

(RV) facts are re-verified at Phase 0; mismatch -> H1. Anchors at HEAD
`683b343d03974f952e304ae4a18c692f1b1f13de` unless stated. Source: the recon report
(sections A-H, S# = its SURPRISES).

**The four blockers, in firing order:**

1. (RV) `Win32Api.GetModuleHandle` (kernel32) at `Window.cs:46` -- the measured
   death site -- AND an independent second call at `VulkanSurface.cs:25`. Fixing
   only Window.cs moves the death from `Runtime.cs:72` to `:86`.
2. (RV) `VkApi.VulkanLib = "vulkan-1.dll"` (`src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs:7`)
   feeds every `[LibraryImport]` in the 545-line file; there is NO
   `NativeLibrary.SetDllImportResolver` anywhere in the repo; the host loader is
   `libvulkan.so.1`, which .NET's Unix probing of "vulkan-1.dll" never matches (S1).
   Hidden until now BY the 111-skip gate.
3. (RV) `VulkanInstance.cs:48-52` asserts `VK_KHR_WIN32_SURFACE_EXTENSION_NAME`
   unconditionally (no `vkEnumerateInstanceExtensionProperties` in the file);
   `vkCreateInstance` fails `VK_ERROR_EXTENSION_NOT_PRESENT` at `:101-109` BEFORE
   any surface is requested (S2). This alone keeps the 71 loader-only tests red.
4. (RV) `src/DualFrontier.Launcher/DualFrontier.Launcher.csproj` has NO `.so` copy
   arm (`grep -c "Native.so"` = 0) while all 8 test projects got theirs at
   `e30375c`; silent under `Condition="Exists"`, fatal at `GameBootstrap` (S3).

**The coupling set (recon A4 -- five production files + the Vulkan binding arms):**

5. `Native/Win32/` leaf: `Win32Api.cs` (15 P/Invokes = 13 user32 + 2 kernel32; 14
   `[LibraryImport]` + 1 `[DllImport]` `RegisterClassExW` -- the WNDCLASSEX LPWStr
   fields force it), `Win32Constants.cs` (5 families; the `VK_*` family is consumed
   by `Input/VirtualKeyMapper.cs`, NOT by Window.cs), `Win32Structs.cs` (4 structs;
   `RECT` dead), `WindowProc.cs` (StdCall delegate). `SendMessageW` has ZERO
   production callers -- it exists solely for `WindowResizeEventTests` (S4).
6. `Window.cs` (302): GUID-unique class name (:53), GCHandle-pinned WndProc (:56-57),
   F08 rollback pattern (:121-139, duplicated in Dispose :283-301), drain-all pump
   (:269-281, hWnd=Zero), 14 enqueue sites publishing 7 event types, WM_SIZE arm
   skips 0x0-and-unchanged and is the ONLY Width/Height writer (:153-166).
7. (RV) `IWindow` production surface = exactly `Handle`, `Width`, `Height`,
   `IsOpen`, `Show`, `PumpMessages`, `Dispose`. `Handle` has ONE production reader:
   `VulkanSurface.cs:38`. `Hide()` has zero consumers. `WindowOptions.Resizable` is
   never read by Window.cs but IS pinned by the ungated `WindowOptionsTests`.
8. `VirtualKeyMapper.cs` consumes `Win32Constants.VK_*` and is pinned by the
   UNGATED, green-on-Linux `VirtualKeyMapperTests` (raw hex InlineData) -- touching
   it breaks a green test. OUT OF SCOPE (section 15); the XCB arm gets its OWN
   mapper.

**Resize truth (recon B4/E, S5 -- load-bearing):**

9. `WindowResizeEvent` is DECORATIVE: the Launcher drains-and-drops
   (`Program.cs:86-89`); swapchain recreation is driven by out-of-date results
   reading `IWindow.Width/Height` (`LauncherRenderer.cs:130-147`, `:191-215`).
   The XCB arm MUST update Width/Height from `XCB_CONFIGURE_NOTIFY`; the
   `caps.currentExtent` preference (`VulkanSwapchain.cs:149-152`) would MASK
   forgetting it -- hence the deliberate-break proof in section 6.

**Structure that stays (recon B3, S6):**

10. Device/queue selection is surface-independent (`VulkanDevice.cs` -- no surface
    parameter; no `vkGetPhysicalDeviceWin32PresentationSupportKHR` exists);
    the present-support assert at `Runtime.cs:95-111` is surface-generic. ZERO
    change to `VulkanDevice`, `VulkanSwapchain`, `Compute/` (grep-clean).
11. `Runtime.Create` (`Runtime.cs:63-186`) constructs Window (:72) then Surface
    (:86) unconditionally; no headless arm exists (1 comment hit repo-wide).
    Composition ORDER does not change in this cascade.

**Test gate anatomy (recon D):**

12. (RV) The gate is `WindowsOnlyFactAttribute`/`WindowsOnlyTheoryAttribute`
    (`tests/DualFrontier.Runtime.Tests/TestInfrastructure/PlatformFacts.cs`) -- a
    pure `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)` check at discovery
    time. Census: 108 `[WindowsOnlyFact]` + 3 `[WindowsOnlyTheory]` = 111, ALL in
    `DualFrontier.Runtime.Tests`. Measured split: 71 gated tests across 14 files
    need only loader+GPU (Class A); 40 across 9 files need a display (Class B);
    the per-file table is in recon section D and is the work order for D6.
13. `WindowResizeEventTests` (3, Class B) synthesise WM_SIZE via the test-only
    `SendMessage` P/Invoke -- not portable; rewritten against the pure decode (D4),
    after which they run UNGATED everywhere.

**Analyzer + environment rails (recon F/G):**

14. (RV) DFK002 (Error): every new P/Invoke MUST live under a namespace rooted in
    `DualFrontier.Runtime.Native` (or `DualFrontier.Core.Interop`) --
    `SanctionedInteropSurface.cs:30-34`. DFK019_A forbids `using` of other graphics
    API roots; `Xcb`/`Wayland` are not on its list. No `[SupportedOSPlatform]`
    exists anywhere in `src/`; do NOT introduce it (CA1416 under
    `TreatWarningsAsErrors` would cascade; recorded design decision).
15. (RV) Host: Wayland session, XWayland at `/usr/bin/Xwayland`, `DISPLAY=:0`,
    `libxcb.so.1` present, `libxkbcommon.so.0` + `libxkbcommon-x11.so.0` present,
    `libxcb-keysyms`/`libxcb-icccm` ABSENT (do not design against them; core xcb
    atoms handle WM_DELETE_WINDOW), Vulkan loader `libvulkan.so.1`, GPU render node
    `/dev/dri/renderD128`. An xcb window IS creatable in this session.
16. No-LINQ/no-async in `src/` is doctrinal (F-47) -- honour it in every new file.
    The `CompileShaders` MSBuild target is Windows-only by condition; committed
    `.spv` artifacts serve Linux -- no change needed.

**Doc drift to fix (recon S4/S7):** `Native/Win32/MODULE.md:44` says "14 functions";
`docs/architecture/VULKAN_SUBSTRATE.md:61` says "15 LibraryImport declarations";
actual: 15 P/Invokes = 14 LI + 1 DllImport (and after this cascade: 14 = 13 user32
+ 2 kernel32 minus SendMessage... re-census at D8 and write the measured number).

## 3. Phase 0 -- preconditions and checkpoint [CORE]

1. **Verify recon facts**: HEAD `683b343` (or fast-forward not touching section-2
   anchor files); every (RV) fact re-measured with the same instruments, INCLUDING
   the host-environment probes (`$DISPLAY`, `ldconfig -p | grep -E "libxcb\.|libxkbcommon-x11|vulkan"`,
   `ls /usr/bin/Xwayland`). Mismatch -> H1. **If `DISPLAY` is empty or X
   connection is refused in YOUR shell -> H9 (environment, not code) -- report,
   do not improvise a workaround.**
2. **Native kernel present**: run the canonical Linux build (commands in the
   kickoff / DEVELOPMENT_HYGIENE) if `native/DualFrontier.Core.Native/build/DualFrontier.Core.Native.so`
   is absent; selftest expect 110 ALL PASSED. Record.
3. **Baseline gates** (regression anchor, recorded verbatim): managed build 0W/0E;
   full test sweep -- expected Linux shape per the F60A closure: ~1194 passed /
   0 failed / 116 skipped (111 WindowsOnly + 5 F-10). Never pipe `dotnet test`
   (file-redirect only, `MSBUILDDISABLENODEREUSE=1`).
4. **Validation checkpoint**: `dotnet run --project tools/DualFrontier.Governance
   -- validate --armed` exit 0 -> else H3.
5. **Frontmatter-shape read** (Lesson #N14): FRAMEWORK 14.3/14.4 + one live LOCKED
   doc + one AUDIT_TRAIL EVT as the verbatim append template. Verify the recon
   report's `register_id` is absent from the register (collision -> H5). Check the
   incoming report frontmatter parses (the F60A lesson: two shape defects arrived
   in enrolled-ready reports; fix as shape-conformance at C1 if found).
6. **Mandatory reads**: the recon report FULL; this brief FULL; `Window.cs`,
   `IWindow.cs`, `WindowOptions.cs`, `VulkanSurface.cs`, `VulkanInstance.cs`,
   `VkApi.cs` (head + the KHR section), `VkConstants.cs`, `VkStructs.cs:125-139`,
   `VkEnums.cs:80-90`, `Runtime.cs:63-186` + `:449-490`, `Launcher/Program.cs`,
   `LauncherRenderer.cs:120-220`, `VulkanSwapchain.cs:130-220`,
   `PlatformFacts.cs`, `WindowResizeEventTests.cs`, `VirtualKeyMapperTests.cs`
   (READ-ONLY -- it must stay green and untouched), `SanctionedInteropSurface.cs`,
   `METHODOLOGY.md` closure protocol, `CODING_STANDARDS.md` 8.4.

`sync` in EVERY frontmatter-touching commit; derived registers never hand-edited;
`AUDIT_TRAIL.yaml` append-only. Closure boundary: push the WORK BRANCH, open a PR
against `main`; never push `main`, never self-merge (8.4 v3.0.0).

## 4. Topology [CORE]

Single orchestrator, serial. The recon did the survey twice over (subagent A-H +
architect scope repro); the change set is one seam + one backend + gates, with
strict commit ordering (each blocker's fix is provable only in order). No wave.

## 5. Wave R -- survey agents

None -- survey is the enrolled recon report. Phase 0 (RV) re-verification is the
freshness check.

## 6. Checkpoints [CORE]

- **C-resolver (after C2):** prove the instrument fires. On Linux, BEFORE C3, a
  direct `VulkanInstance` construction attempt must now fail with the
  EXTENSION error (`VK_ERROR_EXTENSION_NOT_PRESENT` path), NOT
  `DllNotFoundException` -- the death moved past the loader. Record both shapes
  (pre-C2 DllNotFound observed at baseline; post-C2 extension error).
- **C-instance (after C3):** a `VulkanInstance(enableValidation: false)` +
  `VulkanDevice` construction succeeds on this box (the Class-A precondition).
- **C-seam (after C4):** full suite on the WINDOWS-EQUIVALENT arm: everything that
  passed at baseline still passes; the rewritten decode tests pass ungated; zero
  behaviour deltas beyond the seam (the Win32 arm is a refactor, not a redesign).
- **C-xcb (after C5):** deliberate-break proofs, each broken once then restored:
  (a) comment out the CONFIGURE_NOTIFY Width/Height update -> the new resize pin
  must FAIL (else the pin is vacuous -- the currentExtent clamp is masking; fix the
  pin, not the code); (b) the WM_DELETE_WINDOW atom path -> close-button pin fails.
- **C-gates (after C6):** on this box the Runtime suite reports ZERO WindowsOnly
  skips; expected shape: skipped 116 -> 5 (the F-10 family only), passed grows
  accordingly. Reconcile the arithmetic explicitly against baseline (the
  Linux-baseline-shape lesson); any residual skip is named and justified or H2.
- **C-smoke (after C7):** the live smoke (7.6 protocol). Failure -> H8.
- **C-audit (before closure):** truth-law self-audit on every touched doc/comment;
  citation-form compliance; no roadmap load outside ROADMAP pointers.

## 7. Execution / writer specifications [CORE]

Global laws by reference. Intended forms are mutable surface
(`RESERVED_SURFACE_MUTABILITY`); deviations -> `Skeleton revisions`; architectural
forks -> H6. No LINQ, no async, in every new file.

**7.1 D2 -- Vulkan loader resolver (C2).** New static class under
`DualFrontier.Runtime.Native.Vulkan` (DFK002-sanctioned), e.g.
`VulkanLibraryResolver`: a `[ModuleInitializer]` registering
`NativeLibrary.SetDllImportResolver` for the Runtime assembly; when
`libraryName == VkApi.VulkanLib` and `OperatingSystem.IsLinux()`, return
`NativeLibrary.Load("libvulkan.so.1")`; otherwise `IntPtr.Zero` (default probing --
Windows behaviour untouched). Do NOT rename the `VulkanLib` constant (the Windows
arm keeps working by default probing). One resolver, whole-assembly, registered
once (ModuleInitializer is idempotent per load). Test: a Linux-only-capability
fact (7.5's Vulkan gate) asserting `VulkanInstance` construction reaches Vulkan
(post-C3); the C-resolver checkpoint carries the pre/post proof at C2 time.

**7.2 D3 -- platform-selected instance extension (C3).** `VkConstants` gains
`VK_KHR_XCB_SURFACE_EXTENSION_NAME = "VK_KHR_xcb_surface"`. `VulkanInstance.cs`
extension-list construction picks the platform surface extension
(`OperatingSystem.IsWindows()` -> win32, else xcb); `VK_KHR_surface` stays
unconditional; NO enumeration query (fail-fast assert semantics preserved,
symmetric with the Windows arm). The marshalling pattern is untouched.

**7.3 D4 -- the seam (C4; Windows-equivalent, no XCB yet).** Intended form:

- `IWindow`: REMOVE `Handle`; ADD `IntPtr CreateVulkanSurface(IntPtr instanceHandle)`
  (returns a VkSurfaceKHR handle; throws on failure with the platform's own
  diagnostics). Keep `Width`, `Height`, `IsOpen`, `Show()`, `Hide()`,
  `PumpMessages()`, `Dispose()`. Re-write the interface docs platform-neutrally
  (pump = "drain pending OS events; sole Width/Height/IsOpen writer during steady
  state; call once per frame from the owning thread").
- `Window.cs` -> `Win32Window` (file/class rename is intended-form; keep namespace
  `DualFrontier.Runtime.Window`): implements `CreateVulkanSurface` by moving the
  hinstance+`VkWin32SurfaceCreateInfoKHR`+`vkCreateWin32SurfaceKHR` block out of
  `VulkanSurface.cs`; behaviour byte-equivalent otherwise (class name GUID, F08
  rollback, message table unchanged).
- `VulkanSurface.cs` becomes platform-neutral RAII: ctor
  `(VulkanInstance instance, IWindow window)` calls
  `window.CreateVulkanSurface(instance.Handle)`; Dispose keeps
  `vkDestroySurfaceKHR` (already neutral). Its `using ...Native.Win32;` dies.
- A window factory (intended form: `static IWindow PlatformWindow.Create(WindowOptions, InputEventQueue)`)
  returns Win32 on Windows, XCB on Linux (C5 adds the arm; at C4 the Linux arm
  throws `PlatformNotSupportedException` with a clear message). `Runtime.cs:72`
  calls the factory; nothing else in `Runtime.Create` moves.
- Resize decode extracted as a pure function (intended form: static
  `WindowEventDecode.TrySize(int packedOrRaw..., int currentW, int currentH, out int w, out int h)`
  encoding the skip-0x0-and-skip-unchanged law once); the Win32 WM_SIZE arm and
  (later) the XCB CONFIGURE_NOTIFY arm both call it. `WindowResizeEventTests`
  rewritten against the pure function -- UNGATED, `SendMessage` P/Invoke deleted
  with them.
- Tests touching `Handle` (`RuntimeCompositionTests`) migrate to the new contract.

**7.4 D5 -- the XCB backend (C5).** New namespace `DualFrontier.Runtime.Native.Xcb`
(DFK002) with hand-rolled `[LibraryImport("libxcb.so.1")]` declarations -- the
intended minimum: `xcb_connect`, `xcb_disconnect`, `xcb_connection_has_error`,
`xcb_get_setup`/`xcb_setup_roots_iterator` (root screen + visual),
`xcb_generate_id`, `xcb_create_window`, `xcb_map_window`, `xcb_unmap_window`,
`xcb_destroy_window`, `xcb_intern_atom` + `xcb_intern_atom_reply`,
`xcb_change_property` (WM_PROTOCOLS <- WM_DELETE_WINDOW; window title via
`_NET_WM_NAME` UTF8 + `WM_NAME` fallback), `xcb_poll_for_event`, `xcb_flush`, and
libc `free` for event/reply buffers (declare it here, not via a new dependency).
Key mapping via `libxkbcommon-x11.so.0` + `libxkbcommon.so.0`:
`xkb_context_new`, `xkb_x11_get_core_keyboard_device_id`,
`xkb_x11_keymap_new_from_device`, `xkb_x11_state_new_from_device`,
`xkb_state_key_get_one_sym`, `xkb_state_update_key`, unrefs. A NEW
`XkbKeysymMapper` (keysym -> `Key`; arrows, modifiers, F1-F12, A-Z via latin
keysyms, digits, the special set -- mirror the `Key` enum coverage) with its OWN
ungated unit tests on raw keysym constants. `VirtualKeyMapper` untouched.

`XcbWindow : IWindow`: constructor connects, checks `xcb_connection_has_error`,
creates the window with event mask
(STRUCTURE_NOTIFY | KEY_PRESS/RELEASE | BUTTON_PRESS/RELEASE | POINTER_MOTION |
FOCUS_CHANGE), registers WM_DELETE_WINDOW, sets title, flushes; rollback pattern
mirrors F08 (partial-construction teardown; connection is the last thing released).
`PumpMessages` = `xcb_poll_for_event` drain loop; the event table mirrors A2's
message table:

| XCB event | Action |
| --- | --- |
| CLIENT_MESSAGE with WM_DELETE_WINDOW | `IsOpen = false` (the WM_CLOSE analogue) |
| CONFIGURE_NOTIFY | pure decode (7.3): skip 0x0 and unchanged; update Width/Height; enqueue `WindowResizeEvent` |
| KEY_PRESS/KEY_RELEASE | xkb state update + keysym -> `Key`; skip `Key.Unknown`; enqueue Pressed/Released |
| BUTTON_PRESS/RELEASE detail 1/2/3 | `MouseButtonEvent(Left/Middle/Right, ...)` (XCB detail 2 = middle, 3 = right -- map by meaning, not by Win32 order) |
| BUTTON_PRESS detail 4/5 | `MouseWheelEvent(+1/-1)` (press only; ignore the paired release) |
| MOTION_NOTIFY | `MouseMovedEvent(x, y)` |
| FOCUS_IN/FOCUS_OUT | `WindowFocusEvent(true/false)` |

`CreateVulkanSurface`: `VkXcbSurfaceCreateInfoKHR` (new struct in `VkStructs.cs`,
sType `VK_STRUCTURE_TYPE_XCB_SURFACE_CREATE_INFO_KHR = 1000005000` in `VkEnums.cs`)
+ `[LibraryImport] vkCreateXcbSurfaceKHR` added beside the win32 one in
`VkApi.cs`. `Show`/`Hide` = map/unmap + flush. Thread contract: connection used
only from the creating/pump thread (same law as Win32; state it in the class doc).
Per-behaviour tests: display-gated construction/open/close pin; the resize pin
(synthesised via an actual X resize is not scriptable -- pin via the pure decode +
a display-gated pin that `xcb_configure_notify`-driven Width/Height update fires on
a programmatic `xcb_configure_window` resize call; if the latter proves flaky,
record the deviation and keep the decode-level pin as the lock).

**7.5 D6 -- capability gates (C6).** Two attributes replace `WindowsOnlyFact`/
`Theory` in `TestInfrastructure` (intended names): `RequiresVulkanFactAttribute` --
skip unless the platform Vulkan loader loads (`NativeLibrary.TryLoad("vulkan-1.dll")`
on Windows / `"libvulkan.so.1"` on Linux); `RequiresDisplayFactAttribute` (+Theory
variants as needed by the 3 theory sites) -- Vulkan probe AND a display probe
(Windows -> true; Linux -> `DISPLAY` or `WAYLAND_DISPLAY` non-empty AND
`NativeLibrary.TryLoad("libxcb.so.1")`). Discovery-time skip (constructor-set
`Skip`), same as today -- the F09 rationale carries over. Re-tag per the recon's
measured per-file split (section D table: 14 Class-A files -> Vulkan gate, 9
Class-B files -> Display gate); `WindowsOnlyFact`/`Theory` are then DELETED.
`WindowResizeEventTests` are rewritten (7.3), not re-tagged.

**7.6 D7 -- Launcher arm + the live smoke (C7).** Launcher csproj gains the `.so`
item (copy the ratified test-project block verbatim, path
`native/DualFrontier.Core.Native/build/DualFrontier.Core.Native.so`). Smoke
protocol (record verbatim output in the closure report):
`timeout --signal=TERM 15 dotnet run --project src/DualFrontier.Launcher -c Release`
redirected to a file. PASS = exit code 124 (killed by timeout while alive) AND the
log contains no unhandled exception AND a window was observed (the operator is on
the box; the executor states what the log shows, the operator confirms the visual
-- the К-ext-3 γ-smoke precedent). Any exception, or exit before the timeout -> H8.

**7.7 D8 -- docs + dead surface (C8).** `Native/Win32/MODULE.md` +
`Native/Xcb/MODULE.md` (new, mirror form) + `Window/MODULE.md` +
`Graphics/MODULE.md:41` + `docs/architecture/VULKAN_SUBSTRATE.md` drift lines:
write the RE-MEASURED counts (post-SendMessage-removal) and the platform-neutral
seam description; input-drained-and-discarded statements stay (still true). Trim:
`RECT` struct, `WM_CREATE` constant, `SendMessageW` (dies at 7.3). Do NOT touch
`WindowOptions.Resizable` or `IWindow.Hide` (pinned/kept -- F-row instead).

**7.8 D8 -- ledger (C9, closure).** F-rows (executor drafts per live row form):
- NEW: Wayland-native windowing backend -- future cascade; XCB-over-XWayland is the
  ratified present; note `libwayland-client.so.0` measured present.
- NEW: headless/offscreen runtime path -- `Runtime.Create` unconditionally builds a
  window (measured); CI and compute-only workloads would benefit; design open.
- NEW: `WindowOptions.Resizable` is dead (never read; `WS_OVERLAPPEDWINDOW`
  hard-coded; pinned by an ungated test) -- honest-record row, resolution deferred.
- ROADMAP write-back: LINUX_PRESENT_1 DONE with hashes; the W3/PR #51 owed live
  Launcher smoke recorded PAID (name the smoke evidence); forward queue per the
  operator's standing order (W4 / UI charter / F-61 next -- the operator re-ranks).

## 8. Kind-specific machinery [KIND: phase-execution]

Per-behaviour test obligation as specified in 7.3-7.5. Deliberate-break proofs in
section 6 (C-xcb, C-resolver) -- each broken once, observed failing, restored.

## 9. S-LOCK invariants [CORE]

No new structural S-LOCK: DFK002 already polices P/Invoke placement structurally
(cite, do not duplicate); the seam's behavioural locks are the capability-gated
pins + the smoke. The "no platform-tagged handle on IWindow" rule is design intent
recorded in the interface doc, not yet an analyzer rule -- claiming otherwise would
violate the truth law.

## 10. Census discipline [CORE]

- HARD pins unchanged at closure: DFK-WAIVER **3**
  (`rg -n "DFK-WAIVER" src/ --count-matches` canonical form); native exports
  **209**; `native/` tree diff **0** (H7).
- Win32 P/Invoke census MOVES BY DESIGN: 15 -> 14 (SendMessageW deleted). Method:
  `grep -c "LibraryImport\|DllImport" src/DualFrontier.Runtime/Native/Win32/Win32Api.cs`
  minus the comment line -- state the measured before/after in the closure report
  and write the number into the D8 docs.
- New census (recorded, not pinned): P/Invoke count in `Native/Xcb/` (the measured
  final number goes into its MODULE.md).
- Gate census: `[WindowsOnly` sites 111 -> **0**; `[RequiresVulkan` +
  `[RequiresDisplay` sites sum to the re-tag ledger (expected 108 re-tagged: 71
  Vulkan + 37 Display + 3 theory-variants along the same split; the 3 resize tests
  leave the gated set entirely). Exact split per the recon D table; deviations
  recorded with reasons.
- SOFT pins: none expected to move.

## 11. Commit plan [CORE]

| #  | Subject | Content |
| -- | ------- | ------- |
| C1 | `governance(enroll): LINUX_PRESENT_1 brief + recon enrolled` | brief + recon frontmatter-enrolled + sync + validate --armed |
| C2 | `fix(interop): Vulkan loader name resolves per platform` | 7.1 + C-resolver proof |
| C3 | `fix(graphics): instance surface extension selected per platform` | 7.2 + C-instance proof |
| C4 | `refactor(window): surface creation moves behind the window; resize decode goes pure` | 7.3, Windows-equivalent, decode tests ungated |
| C5 | `feat(window): XCB backend over XWayland (Native.Xcb + XkbKeysymMapper)` | 7.4 + C-xcb deliberate-break proofs |
| C6 | `test(infra): capability gates replace the WindowsOnly skips (71 loader / 40 display)` | 7.5 + C-gates arithmetic |
| C7 | `build(launcher): Linux native arm; live Launcher smoke` | 7.6 + smoke evidence |
| C8 | `docs(runtime): windowing seam truth -- MODULE/VULKAN_SUBSTRATE counts + dead-surface trim` | 7.7 |
| C9 | `governance(closure): LINUX_PRESENT_1 EVT + ROADMAP write-back -- W3 smoke debt PAID` | 7.8 + EVT + brief -> EXECUTED + sync + validate --armed |

Intended-form count; splits recorded, history never compressed.

## 12. REGISTER cascade [CORE]

Schema-2.0, Phase 0 verbatim shapes only. C1 enrolls this brief (D/3/Draft) + the
recon report (E/3/EXECUTED, frontmatter pre-written -- verify shape, correct only
as shape-conformance). C9 flips the brief to EXECUTED (+`last_modified`), appends
the single closure EVT with real hashes of C1-C8, re-syncs. `validate --armed`
exit 0 at every frontmatter-touching commit. Vocabulary gap -> H5.

## 13. Halt conditions (H-series) [CORE]

- **H1** Phase 0 precondition/(RV) mismatch.
- **H2** build/test regression vs baseline, or C-gates arithmetic that does not
  reconcile.
- **H3** `validate --armed` nonzero.
- **H4** (unused -- no wave).
- **H5** REGISTER vocabulary gap -- escalate, never invent.
- **H6** an intended form proves architecturally wrong (e.g. the
  surface-behind-window contract cannot serve a documented consumer; a
  platform-tagged handle seems "needed" -- that is a fork, stop).
- **H7** ANY `native/` tree modification.
- **H8** the live smoke fails after C7, or a deliberate-break proof does NOT fail
  when broken (a vacuous pin) -- STOP, report verbatim, await the architect.
- **H9** the executor environment lacks a usable display/X path (empty `DISPLAY`,
  refused connection) -- environment halt, report, do not stub the smoke.
- Standing rails: push law 8.4 v3.0.0 (branch + PR; never `main`; no self-merge);
  derived registers never hand-edited; `AUDIT_TRAIL.yaml` append-only; no history
  rewrite; `historical/` read-only; never pipe `dotnet test`;
  `VirtualKeyMapperTests` and `WindowOptionsTests` must remain green and untouched.

On halt: stop, report state verbatim, await the operator.

## 14. Closure protocol and report [CORE]

`METHODOLOGY` closure protocol: (a) ROADMAP write-back (7.8); (b) frontmatter +
sync folded per commit; (c) single EVT append, prior entries byte-unchanged;
(d) F-rows -- never chat-only; (e) the closure report (chat) carrying: commits
table (hash | subject); versions table; census before/after (section 10 -- HARD
exact, the Win32 15->14 and gate 111->0 movements with commands); gates table
baseline vs closure INCLUDING: build 0W/0E, full-suite counts with the skip
arithmetic reconciled (expected: skipped 116 -> 5, passed grows by the flipped
minus rewritten delta -- state the exact numbers), native selftest 110,
`validate --armed` exit 0, the C-resolver/C-xcb deliberate-break records, and the
SMOKE EVIDENCE (command, exit code, log tail); F-ledger final table; consolidated
`Skeleton revisions`; self-attestation (work branch pushed + PR opened per 8.4
v3.0.0, `main` untouched, no self-merge; sync in every frontmatter-touching
commit; single EVT append; no history rewrites; `native/` + `historical/` +
reference trees untouched; VirtualKeyMapperTests/WindowOptionsTests untouched);
operator checklist (merge the PR; VISUALLY confirm the smoke window; the forward
queue: W4 / UI charter / F-61 -- operator re-ranks).

## 15. Out of scope [CORE]

Wayland-native backend (F-row); headless/offscreen path (F-row);
`Input/VirtualKeyMapper.cs` and its tests (Win32 arm keeps them; XCB gets its own
mapper); input CONSUMERS (InputBridge -- the Launcher keeps drain-and-drop);
present-family selection (F06 stays open); frame pacing; `WindowOptions.Resizable`
semantics (F-row records it dead); `IWindow.Hide` removal; UI program;
`VulkanDevice`/`VulkanSwapchain`/`Compute` internals; F-57/F-61/F-62..F-65
families; any `native/` change; enumeration-based extension negotiation; pushes to
`main` and merging (operator's act).

---

**End of LINUX_PRESENT_1_BRIEF.md v1.0**
