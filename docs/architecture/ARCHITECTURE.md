---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-ARCHITECTURE
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0.0"
next_review_due: 2027-06-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-ARCHITECTURE
---
# Dual Frontier architecture

Dual Frontier is a colony simulation built as a native C++ kernel under a managed shell: component storage, kernel scheduling decisions, and event routing are native authority; game rules, the mod OS, and orchestration are C#. A system never touches another system's data directly — every interaction runs through declared contracts (`[SystemAccess]`, domain buses, mod capability tokens). This page is the orientation map; each subsystem's truth lives in the document the pointers section names.

Motivation, compressed: the genre's reference architecture runs every system sequentially on one core, lets hot paths scan storage directly, and exposes mods to private internals via runtime patching. Dual Frontier answers with declared access + parallel phases, intent/cache interaction, and `AssemblyLoadContext`-isolated mods.

## Layer map (the real assembly set)

Twelve managed `src/` projects (census per [DEVELOPMENT_HYGIENE](../methodology/DEVELOPMENT_HYGIENE.md) §2) plus the C++ kernel:

```
┌────────────────────────────────────────────────────────────────┐
│ PRESENTATION                                                   │
│   DualFrontier.Launcher — window + render loop (single         │
│   production renderer); DualFrontier.Runtime — Vulkan          │
│   substrate (vulkan-1.dll via pure P/Invoke)                   │
├────────────────────────────────────────────────────────────────┤
│ APPLICATION                                                    │
│   DualFrontier.Application — GameBootstrap/GameLoop, Mod OS    │
│   (loader, registry, capability model, fault handling),        │
│   PresentationBridge command queue, display composition        │
├────────────────────────────────────────────────────────────────┤
│ DOMAIN                                                         │
│   DualFrontier.Systems · Components · Events · AI ·            │
│   Persistence — game rules; multithreaded; renderer-agnostic   │
├────────────────────────────────────────────────────────────────┤
│ INFRASTRUCTURE                                                 │
│   DualFrontier.Core — domain buses, scheduling (dispatch       │
│   facade); DualFrontier.Core.Interop — P/Invoke bridge,        │
│   NativeWorld handle, span/batch protocol;                     │
│   DualFrontier.Crypto.Future — reserved FHE surface            │
├────────────────────────────────────────────────────────────────┤
│ NATIVE KERNEL (C++20)                                          │
│   DualFrontier.Core.Native — NativeWorld storage SSoT (К-L11), │
│   scheduler graph + wake registry (К-L12/К-L13), three-tier    │
│   event bus (К-L15), GPU pipeline slots (К-L16)                │
└────────────────────────────────────────────────────────────────┘
```

`DualFrontier.Contracts` sits beside the stack: interfaces, attributes, and event/component base types referenced by every managed layer. Mods reference **only** Contracts.

## Dependency rules

Verified against the `ProjectReference` entries in `src/*/*.csproj`; each row lists everything that assembly references.

| Assembly | References |
|---|---|
| Contracts | nothing (System.* only) |
| Core.Interop | Contracts; P/Invokes `DualFrontier.Core.Native.dll` |
| Components | Contracts, Core.Interop |
| Events | Contracts, Components |
| AI | Contracts, Components |
| Persistence | Contracts, Components |
| Core | Contracts, Core.Interop |
| Systems | Contracts, Core, Components, Events, AI (Core internals via `InternalsVisibleTo`) |
| Application | Contracts, Core, Core.Interop, Components, Events, Systems, AI |
| Runtime | Core.Interop |
| Launcher | Application, Runtime (Application internals via `InternalsVisibleTo`) |
| Crypto.Future | nothing (reserved surface) |
| Mods | Contracts **only** — enforced by `AssemblyLoadContext` isolation |

Dependency direction is strictly downward in the layer map; Presentation never calls Domain directly — render-relevant domain events cross through the `PresentationBridge` command queue and are dispatched on the render side per frame (`RenderCommandDispatcher` in Launcher).

## Runtime shape

One process, several thread populations:

- **Render main thread** — Launcher's window/render loop; consumes `PresentationBridge` commands each frame.
- **Simulation thread** — `GameLoop` runs the fixed-step tick (30 Hz) on a dedicated background thread.
- **Managed scheduler workers** — `Parallel.ForEach` per phase, capped at `ProcessorCount − 2` (see [THREADING](./THREADING.md)).
- **Native kernel thread pool** — sized to `hardware_concurrency()` at native bootstrap; owns kernel-side task dispatch.

Mods load into per-mod `AssemblyLoadContext`s inside the same process; a faulting mod is soft-unloaded without crashing the core (see [ISOLATION](./ISOLATION.md)).

## Where the truth lives

- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) — kernel truth: Part 0 (К-L invariants), module purposes, two-phase bootstrap, interop patterns.
- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) — mod system: manifests, capability model, ALC lifecycle, unload chain.
- [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) — GPU substrate: device/queue model, compute pipelines, shader toolchain.
- [THREADING](./THREADING.md) — scheduling: native scheduler sovereignty, managed dispatch facade, tick rates, async ban.
- [EVENT_BUS](./EVENT_BUS.md) — messaging: managed domain buses, native three-tier bus, intent and lease models.
- [ECS](./ECS.md) / [ISOLATION](./ISOLATION.md) — storage access protocol; enforcement model and fault lifecycle.
- [docs/ROADMAP.md](../ROADMAP.md) — forward state. Architecture documents answer "what is"; the roadmap alone answers "what's next".

## Amendment protocol

This document is Tier 1 LOCKED. An amendment proceeds: (1) surface the change to the owner (Crystalka); (2) record rationale; (3) semver — PATCH for correction, MINOR for additive sections, MAJOR for layer-map or dependency-rule inversion; (4) bump `version` and `next_review_due` in the register mirror via a governance commit with a validate run folded in; (5) propagate to citing documents.

## Change history

| Version | Date | Change |
|---|---|---|
| **1.0.0** | 2026-06-12 | Rewrite to thin code-truth overview (Architecture Truth Cascade): real 12-project assembly set + native kernel in the layer map (Core.Interop/Runtime/Launcher/Crypto.Future in; Power and the managed `World`/isolation-guard story out); dependency table regenerated from csproj truth; scenario walkthroughs and extended genre-comparison framing dropped in favor of pointers. **MAJOR** (0.4.1 → 1.0.0). |
| 0.3–0.4.1 | 2026-04 → 2026-06 (era) | Managed-ECS era overview (four layers, five buses, isolation guard, scenario shading) plus the DD-1 code-truth notice banner. Superseded by 1.0.0. |
