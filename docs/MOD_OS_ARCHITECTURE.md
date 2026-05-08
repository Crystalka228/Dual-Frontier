---
title: Mod OS Architecture
nav_order: 25
---

# Mod OS Architecture — Dual Frontier

**Status:** LOCKED v1.6 — Phase 0 closed; non-semantic corrections from M1–M3.1 audit (v1.1), M3 closure review (v1.2), M4.3 implementation review (v1.3), M7 pre-flight readiness review (v1.4), Audit Campaign Pass 2 (v1.5), and GPU compute integration ratification (v1.6) applied. Every architectural decision in this document is final input to all subsequent migration phases (M1–M10, K9, G0–G9, see §11). Items marked **✓ LOCKED** reflect decisions taken during Phase 0 deliberation; deviation in implementation requires reopening this document, not improvisation in code.

**Version history:**

- v0.1 (initial draft) — initial specification of the mod-as-process model. Five strategic decisions locked; seven detail decisions (D-1 through D-7) collected in §12 as ⚠ DECISION pending human resolution.
- v1.0 — Phase 0 closed. All seven open decisions resolved and locked. Implementation phases M1–M10 may begin.
- v1.1 — non-semantic corrections from the first independent audit (M1–M3.1):
  - §4.1: `Log()` parameter type is `ModLogLevel`, not `LogLevel`. The original name collided with `Microsoft.Extensions.Logging.LogLevel`; the implementation correctly chose a kernel-namespaced enum, and the spec is brought in line.
  - §2.2: `dependencies[i].optional` (bool, default `false`) documented as a recognised optional flag on inter-mod dependencies. Discovered as `ModDependency.IsOptional` in the M1 implementation, kept on the strength of utility, and now ratified.
  - No semantic changes. No locked decision is altered. M1–M3.1 implementations continue to comply.
- v1.2 — non-semantic corrections from the M3 closure review:
  - §3.6: hybrid enforcement formulation. The v1.0/v1.1 wording described capability checks as load-time only and the hot path as "free of permission lookups." The M2 implementation (commits `35dc5b2`, `0d5b32f`) added a runtime per-call check inside `RestrictedModApi.EnforceCapability` — a hash-set lookup measured negligible on the hot path — which is exactly what §4.2 and §4.3 already specify. v1.2 brings §3.6 in line with §4.2/§4.3 and the implementation: enforcement is hybrid, load-time as primary gate plus runtime as second-layer defence.
  - §3.5 + §2.1: production components consumed by the §2.1 example manifest (`WeaponComponent`, `ArmorComponent`, `AmmoComponent`, `ShieldComponent`, `HealthComponent`) annotated with `[ModAccessible]` per D-1 LOCKED. The §2.1 example itself was expanded to include `kernel.read:AmmoComponent` and `kernel.read:ShieldComponent` — Vanilla.Combat requires both (ammo accounting per §11 of the original Phase 5 spec, shield damage routing per §6.4 of the GDD), but the v1.0/v1.1 example listed only three components as a sketch. v1.2 brings the example in line with what a real combat mod actually needs. Without these annotations the §2.1 example manifest would fail Phase C with `MissingCapability` — the spec example is now end-to-end loadable.
  - §11.1: M3.4 added as deferred milestone (CI Roslyn analyzer per D-2 hybrid completion). M3.1, M3.2, M3.3 closed by M3 closure review; M3.4 unblocked when the first external (non-vanilla) mod author appears — runtime `CapabilityViolationException` already catches dishonest `[ModCapabilities]` attributes, so the analyzer is developer-experience tooling for early feedback before publication, not a runtime safety boundary.
  - No semantic changes. No locked decision (D-1 through D-7) is altered. M3 implementations continue to comply.
- v1.3 — non-semantic correction from the M4.3 implementation review:
  - §2.2: `entryAssembly` and `entryType` rows in the manifest field reference table reworded from "ignored for `kind=shared`" to "must be empty for `kind=shared`". The v1.0–v1.2 wording contradicted §5.2 step 1, which explicitly requires these fields to be empty for shared mods. The M4.3 implementation (`ContractValidator` Phase F, commit `e0151d8`) enforces §5.2 wording — non-empty `entryAssembly` or `entryType` on a shared mod manifest produces `ValidationErrorKind.SharedModWithEntryPoint`. v1.3 brings §2.2 in line with §5.2 and the implementation.
  - No semantic changes. No locked decision (D-1 through D-7) is altered. M4 implementations continue to comply.
- v1.4 — non-semantic clarifications from the M7 pre-flight readiness review:
  - §9.5 step 7: explicit GC pump protocol added. Each iteration of the `WeakReference` spin loop performs `GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect()` before re-checking `WeakReference.IsAlive`. The double-collect bracket is required because `WaitForPendingFinalizers` can resurrect finalizable graph nodes the first collect would have removed; the second collect picks those up, restoring monotonic progress. Default cadence: 100 iterations × 100 ms = 10 s timeout (matches the §9.5 step 7 v1.0 wording). The cadence is implementation-tunable; the GC pump bracket is mandatory. Without this clarification the v1.0 wording «spins on `WeakReference`» admits flaky implementations, and the §11.4 stop condition («WeakReference unload tests are flaky — any failure rate above 0%») would trigger spuriously. v1.4 brings §9.5 step 7 in line with the only stable implementation pattern.
  - §9.5: new sub-section §9.5.1 «Failure semantics» added, locking the best-effort discipline already implicit in the chain. Steps 1–6 are sequential and best-effort: if any step throws, the loader logs the exception with `(modId, stepNumber)`, surfaces a non-blocking `ValidationWarning`, and continues to the next step. The `ModLoader.UnloadMod` swallowed `try/catch` around `mod.Instance.Unload()` (in place since M0) is consistent with this discipline. After step 6, if step 7 times out, the existing `ModUnloadTimeout` warning fires; the mod is removed from the active set regardless. There is no atomic-unload guarantee — `Unload` is conceptually irreversible (subscriptions removed cannot be re-attached without re-running `Subscribe`); the chain is structured so each step is a no-op if its predecessor failed (e.g. `RemoveSystems` on a mod with no registered systems is harmless). This formalises a discipline the M0–M6 implementation already follows; no new state is introduced to §9.1.
  - No semantic changes. No locked decision (D-1 through D-7) is altered. No state added to §9.1. M0–M6 implementations continue to comply.
- v1.5 (this version) — non-semantic correction from Audit Campaign Pass 2 (cumulative drift audit, 2026-05-01):
  - §11.2: baseline enumeration of `ValidationErrorKind` expanded to include `IncompatibleContractsVersion` and `WriteWriteConflict` alongside `MissingDependency` and `CyclicDependency`. The v1.0–v1.4 wording «The current enum has `MissingDependency` and `CyclicDependency`» is declarative without a non-exhaustive qualifier (no `e.g.`, no `among others`); on a strict reading it implies a complete 2-member baseline. The actual baseline at every v1.x snapshot is 4 members — `IncompatibleContractsVersion` is used by `ContractValidator` Phase A (`ValidateContractsVersions`) for `RequiresContractsVersion` failures, and `WriteWriteConflict` is used by Phase B (`ValidateWriteWriteConflicts`) for component write-collision detection per §10.1 («Mod registers a system that conflicts with another mod's system»). Both errors existed before any M-phase migration; the spec wording understated the baseline. v1.5 brings §11.2 in line with `src/DualFrontier.Application/Modding/ValidationError.cs:9–83` (11 enum members total: 4 baseline + 7 migration additions, matching the §11.2 «migration adds» list).
  - No semantic changes. No locked decision (D-1 through D-7) is altered. No state added to §9.1. M0–M7.3 implementations continue to comply.

---

## Preamble — How to use this document

**Authority.** This document is the single architectural authority for the mod system of Dual Frontier. During implementation, every interface, attribute, manifest field, and lifecycle step traces back to a section here. Disagreement with the specification is escalated to the human (via §12 open decisions) — never resolved by improvisation in code.

**Scope.** The specification governs:

- The structural relationship between the kernel (`DualFrontier.Core` + `DualFrontier.Contracts`) and a loaded mod.
- The manifest schema and the loader pipeline that consumes it.
- The `IModApi` surface and its semantic guarantees.
- Capability declaration, capability checking, and capability granularity.
- Type sharing across `AssemblyLoadContext` boundaries.
- Bridge replacement and conflict resolution.
- Versioning across three independent axes (kernel API, mod self, inter-mod).
- The mod lifecycle, including hot reload through the mod menu.
- The threat model, distinguishing what mods can and cannot do.

The specification does **not** govern:

- Specific gameplay content (weapons, recipes, biomes) — these are decided by the mod author within the architecture.
- Game-design questions (balance, narrative, pacing).
- Performance budgets for individual systems — covered by [PERFORMANCE](./PERFORMANCE.md).
- Methodology of the development pipeline — covered by [METHODOLOGY](./METHODOLOGY.md).

**Strategic locked decisions.** Five top-level decisions taken during Phase 0; the seven detail decisions (D-1 through D-7) are listed in §12:

1. **✓ LOCKED.** Capability granularity is **per-event-type and per-component-type**.
2. **✓ LOCKED.** Bridge replacement is **explicit** via the manifest's `replaces` field.
3. **✓ LOCKED.** Hot reload is fully supported, but **only through the mod menu** with the simulation paused.
4. **✓ LOCKED.** Vanilla content ships as **multiple mods** mirroring the existing `DualFrontier.Systems.*` structure.
5. **✓ LOCKED.** Versioning is a **three-tier model**: kernel API SemVer (existing `ContractsVersion`), mod self SemVer, inter-mod dependency SemVer with caret-prefix support.

**The "stop, escalate, lock" rule.** When implementation encounters a design question not answered here, the response is "stop, document in §12, wait for the human to lock" — not "guess." The structural strength of the mod system depends on the specification being the only source of architectural truth.

---

## 0. Executive summary and OS mapping

The design treats Dual Frontier as a small operating system and each mod as a process running on top of it. The mapping below is not decorative — every column on the right has a counterpart we are obligated to provide, and the document is structured around closing the gaps.

| OS concept | Dual Frontier counterpart | Status |
|---|---|---|
| Kernel | `DualFrontier.Core` + `DualFrontier.Contracts` | Implemented |
| Process | A mod loaded into its own `AssemblyLoadContext` | Implemented |
| Process isolation (MMU) | `SystemExecutionContext` + `[SystemAccess]` | Implemented |
| Syscall | `IModApi` | Half-implemented — `Publish`/`Subscribe` are no-ops |
| IPC between processes | `IModContract` via `IModContractStore` | Implemented |
| Device driver | A mod-supplied `SystemBase` registered via `RegisterSystem` | Implemented |
| Register a new "syscall" or new device | Mod adding new event types or services | **Not implemented** |
| Shared library (`.so` / `.dll`) | A library-only mod with shared types, no entry point | **Not implemented** |
| `dlopen` / hot reload | Pause-aware reload via `ModIntegrationPipeline.Apply` | Partial — atomic batch only |
| Capability model | Manifest `capabilities` list + loader check | **Not implemented** |
| Package manager dependency resolution | Manifest `dependencies` with SemVer ranges | **Not implemented** |

The status column drives the migration plan in §11. Sections 1–10 specify the target architecture; §11 sequences the path from current state to target.

---

## 1. Mod topology — three mod kinds

Mods are not uniform. The architecture distinguishes three categories, each with a different role in the load graph and different rules at the loader.

### 1.1 Regular mod

The default kind. Has an entry point (`IMod` implementation), runs `Initialize(api)` to register components, systems, and subscriptions, may publish and consume contracts.

- Lives in its own `AssemblyLoadContext`.
- May depend on shared mods.
- May not depend on regular mods directly — only on contracts they publish.
- Manifest: `kind: "regular"`. Default when omitted.

### 1.2 Shared mod (library)

Defines types — `record`s, `interface`s, `enum`s — that other mods reference at compile time. Has **no `IMod` implementation** and registers no systems. The loader places its assembly in a **shared `AssemblyLoadContext`** so dependent mods see the same `Type` instances.

- Lives in the shared ALC, separate from regular mods.
- Cannot subscribe, publish, or register — it is a pure type vendor.
- Loaded before any dependent regular mod.
- Manifest: `kind: "shared"`. Required.

The shared mod is the solution to the type-sharing problem (§5). Without it, Mod A defining `record FireballCastEvent : IEvent` and Mod B subscribing to that event cannot interoperate — the two ALCs produce two distinct `Type` instances even when the assembly bytes are identical.

### 1.3 Vanilla mod

A regular mod authored by the engine team that ships with the base game. Architecturally identical to a third-party regular mod — same manifest schema, same `IModApi` surface, same isolation rules. The distinction is editorial: vanilla mods are the canonical reference implementations and the test polygon for the mod system itself.

- Manifest: `kind: "regular"` (vanilla is not a separate kind).
- Convention: id begins with `dualfrontier.vanilla.` (e.g. `dualfrontier.vanilla.combat`).
- Convention: shipped in `mods/` directory under `DualFrontier.Mod.Vanilla.<Slice>/`.

The split into multiple vanilla mods follows the existing `DualFrontier.Systems.*` decomposition. Initial set: `Vanilla.Combat`, `Vanilla.Magic`, `Vanilla.Inventory`, `Vanilla.Pawn`, `Vanilla.World`. Each may depend on `Vanilla.Core` (a shared mod with definition records used across slices).

### 1.4 Load graph

```
                 ┌────────────────────────────────┐
                 │  shared ALC                    │
                 │  ┌──────────────────────────┐  │
                 │  │ DualFrontier.Mod.        │  │
                 │  │   Vanilla.Core (shared)  │  │
                 │  │ DualFrontier.Mod.        │  │
                 │  │   MagicProtocol (shared) │  │
                 │  └──────────────────────────┘  │
                 └────────────────────────────────┘
                          ▲                ▲
                          │ references     │
   ┌──────────────────────┴┐              ┌┴───────────────────────┐
   │ regular ALC            │              │ regular ALC            │
   │ Vanilla.Combat (mod)   │              │ Vanilla.Magic (mod)    │
   └────────────────────────┘              └────────────────────────┘
                          ▲                ▲
                          │  depends-on    │
                          └────────┬───────┘
                                   │
                          ┌────────┴───────────┐
                          │ regular ALC        │
                          │ Vanilla.Inventory  │
                          └────────────────────┘
```

**Invariants:**

- Shared ALC is loaded once at game start and never unloaded during the session.
- Each regular mod has its own ALC with `IsCollectible = true`, allowing unload.
- A regular mod's ALC may resolve types from the shared ALC, but never from another regular ALC.
- Cycles between regular mods are forbidden; cycles in the shared ALC dependency graph are forbidden.

---

## 2. Manifest v2

The current `ModManifest` (v1) carries `Id`, `Name`, `Version`, `Author`, `RequiresContractsVersion`, `Dependencies` (as list of mod ids without version), `EntryAssembly`, `EntryType`. Manifest v2 extends this surface in a backward-compatible way: every new field has a default that reproduces v1 behavior.

### 2.1 Schema (v2)

```json
{
  "id": "dualfrontier.vanilla.combat",
  "name": "Vanilla Combat",
  "version": "1.0.0",
  "author": "Dual Frontier Team",
  "kind": "regular",
  "apiVersion": "^1.0.0",
  "entryAssembly": "DualFrontier.Mod.Vanilla.Combat.dll",
  "entryType": "DualFrontier.Mod.Vanilla.Combat.CombatMod",
  "hotReload": true,

  "dependencies": [
    { "id": "dualfrontier.vanilla.core", "version": "^1.0.0" },
    { "id": "dualfrontier.vanilla.inventory", "version": "^1.0.0" }
  ],

  "replaces": [
    "DualFrontier.Systems.Combat.CombatSystem",
    "DualFrontier.Systems.Combat.DamageSystem",
    "DualFrontier.Systems.Combat.ProjectileSystem"
  ],

  "capabilities": {
    "required": [
      "kernel.publish:DualFrontier.Events.Combat.DamageEvent",
      "kernel.publish:DualFrontier.Events.Combat.DeathEvent",
      "kernel.subscribe:DualFrontier.Events.Combat.ShootGranted",
      "kernel.read:DualFrontier.Components.Combat.WeaponComponent",
      "kernel.read:DualFrontier.Components.Combat.ArmorComponent",
      "kernel.read:DualFrontier.Components.Combat.AmmoComponent",
      "kernel.read:DualFrontier.Components.Combat.ShieldComponent",
      "kernel.write:DualFrontier.Components.Shared.HealthComponent"
    ],
    "provided": [
      "mod.dualfrontier.vanilla.combat.publish:DualFrontier.Mod.Vanilla.Combat.WeaponDef"
    ]
  }
}
```

### 2.2 Field reference

| Field | Type | Required | Default | Notes |
|---|---|---|---|---|
| `id` | string | yes | — | Reverse-domain. Must be globally unique. |
| `name` | string | yes | — | Human-readable. |
| `version` | string (SemVer) | yes | — | Mod self-version. Strict `MAJOR.MINOR.PATCH`. |
| `author` | string | no | `""` | Free-form. |
| `kind` | enum | no | `"regular"` | One of `regular`, `shared`. |
| `apiVersion` | string (SemVer with caret) | yes | — | Compatibility against `ContractsVersion.Current`. |
| `entryAssembly` | string | conditional | `"{id}.dll"` | Required for `kind=regular`; **must be empty for `kind=shared`** (per §5.2 step 1). |
| `entryType` | string | conditional | scan-for-IMod | Required for `kind=regular`; **must be empty for `kind=shared`** (per §5.2 step 1). |
| `hotReload` | bool | no | `false` | When `false`, mod loads only at session start; menu refuses to reload it. |
| `dependencies` | array of `{id, version, optional}` | no | `[]` | Each `version` is a SemVer constraint (§8). The optional `optional` boolean (default `false`) marks a dependency that the loader may treat as soft: when the named mod is absent, an optional dependency emits a warning rather than a `MissingDependency` error. Required (default) dependencies still hard-fail. |
| `replaces` | array of string (FQN) | no | `[]` | Fully-qualified type names of systems this mod replaces. Only meaningful for `kind=regular`. |
| `capabilities.required` | array of string | no | `[]` | See §3 for capability syntax. |
| `capabilities.provided` | array of string | no | `[]` | See §3. |

### 2.3 Validation steps at parse time

1. Every required field present and non-empty.
2. `version` and the `version` field of every `dependencies` entry parses as SemVer.
3. `apiVersion` parses as SemVer with optional caret prefix.
4. No duplicate ids in `dependencies`.
5. No type listed in `replaces` is also listed by another mod's `replaces` in the current load batch (cross-batch check at §7).
6. Every capability string matches the regex `^(kernel|mod\.[a-z0-9.]+)\.(publish|subscribe|read|write):[A-Za-z][A-Za-z0-9_.]+$`.

A failure at any step rejects the mod with a typed `ValidationError` (extending the existing `ValidationErrorKind` enum — see §11.2).

---

## 3. Capability model

Capabilities are **named, declared, and statically checked permissions**. Every operation a mod performs that touches the kernel or another mod must be backed by an entry in `capabilities.required`. Capabilities that the mod itself adds to the system (new event types, new contracts) appear in `capabilities.provided`.

### 3.1 Granularity ✓ LOCKED

Granularity is **per-event-type and per-component-type**. A capability never applies to a category, namespace, or wildcard:

- Permitted: `kernel.publish:DualFrontier.Events.Combat.DamageEvent`
- Forbidden: `kernel.publish:DualFrontier.Events.Combat.*`
- Forbidden: `kernel.publish:combat`
- Forbidden: `kernel.publish:*`

The cost is verbose manifests for content-rich mods. The benefit is that a `git diff` of a manifest reveals exactly which new kernel surface the mod began touching, enabling reviewable security and change control.

### 3.2 Syntax

```
<provider>.<verb>:<fully-qualified-type-name>
```

- `provider`:
  - `kernel` — provided by `DualFrontier.Contracts` itself.
  - `mod.<modId>` — provided by another loaded mod (typically a shared mod publishing event types).
- `verb`: one of `publish`, `subscribe`, `read`, `write`.
- `fully-qualified-type-name`: the C# FQN of the event or component type.

The `read` and `write` verbs apply to components (`IComponent`). The `publish` and `subscribe` verbs apply to events (`IEvent`).

### 3.3 Reserved namespaces

- `kernel.*` is reserved for capabilities the kernel itself provides. Only the kernel may list these in `capabilities.provided` (as part of its own internal manifest — see §3.5).
- `mod.<modId>.*` is reserved for the mod with that exact `id`. Mods cannot claim to provide capabilities under another mod's namespace; the loader rejects such manifests.

### 3.4 Static check at load time

When a mod loads, the loader validates:

- For every `capabilities.required` entry of the mod, an entry with the same string exists in either:
  - The kernel's own provided set (§3.5), **or**
  - The `capabilities.provided` set of an already-loaded shared mod or regular mod listed in this mod's `dependencies`.
- A required capability cannot be satisfied by a mod *not* listed in `dependencies`. This is a hard rule — implicit dependency through shared capability is forbidden.

Failure produces a `ValidationError` of new kind `MissingCapability`, listing each unsatisfied capability and the mod that needs it.

### 3.5 Kernel-provided capability set

The kernel exposes a fixed list of capabilities derived from public types in `DualFrontier.Contracts` and `DualFrontier.Components`. The list is generated at build time from a reflection scan and embedded as a resource in the kernel assembly. Mods read this resource through a new `IModApi.GetKernelCapabilities()` accessor.

> **✓ LOCKED (D-1).** `read` and `write` capabilities apply only to a **curated, opt-in subset** of public components. A component is reachable from a mod only when annotated with `[ModAccessible(Read = true, Write = false)]`. The component author actively decides what mods can touch; everything else is invisible to the capability resolver and produces a `MissingCapability` error if requested. Aligns with the project's structural-isolation philosophy: tighter blast radius, falsifiable surface.

### 3.6 Hybrid enforcement — load-time + runtime

Capability enforcement operates on two layers.

**Load-time** (primary gate, before the mod reaches `Active` state §9.1):

- §3.4 — every `capabilities.required` token must be provided by the kernel or by a listed dependency. Failure: `MissingCapability`.
- §3.7 — every registered system's `[SystemAccess]` declarations must be a subset of the mod's `capabilities.required`.
- §3.8 / D-2 — every registered system's `[ModCapabilities]` tokens must appear in the manifest's `capabilities.required`.

**Runtime** (second-layer defence inside `RestrictedModApi`):

- §4.2 — `Publish<T>` checks the per-mod required set; mismatch raises `CapabilityViolationException`. Hash-set lookup, `O(1)`, measured negligible on the hot path.
- §4.3 — `Subscribe<T>` checks the same set at subscribe time. Same exception, same cost.

Enforcement on isolated component access (`SystemExecutionContext` reads/writes via `[SystemAccess]`) and on bus delivery (subscribers receive only events for types they declared) operates independently of the capability layer and continues to function as in v1.

The runtime layer covers three cases the load-time gate cannot reach:

1. Reflection-based bypass of `[ModCapabilities]` declarations (deliberate violation rather than accident).
2. Event types constructed at runtime via generics or reflection.
3. v1 manifest grace period (§4.5) — v1 mods bypass load-time `MissingCapability` because their `capabilities.required` is empty, but the per-call runtime check still catches actual violations and emits a deprecation warning directing the author to the v2 manifest.

### 3.7 Cross-check with `[SystemAccess]`

A mod's `capabilities.required` must be a **superset** of every `[SystemAccess]` declaration on every system the mod registers. The loader performs this cross-check after `Initialize(api)` returns:

```
for each system S registered by the mod:
    for each component C in S.[SystemAccess].Reads:
        require "kernel.read:<FQN of C>" or "mod.<provider>.read:<FQN of C>"
        in mod.capabilities.required
    for each component C in S.[SystemAccess].Writes:
        require "kernel.write:<FQN of C>" or "mod.<provider>.write:<FQN of C>"
    for each bus B in S.[SystemAccess].Buses:
        require capabilities consistent with what the system actually publishes (§3.8)
```

A drift between the manifest and the code is a load-time error, never silent.

### 3.8 Bus capability mapping

`[SystemAccess]` declares buses by name, not by event type. Capabilities are by event type. The loader cross-references: a system declaring `buses: ["Combat"]` and publishing `DamageEvent` must have either `publish:DamageEvent` in the manifest or `subscribe:DamageEvent` for received events. The mapping from system code to required capabilities is computed at load time by static analysis of the assembly (Roslyn-based scan of `Services.Combat.Publish<T>` call sites).

> **✓ LOCKED (D-2).** Hybrid enforcement. Each mod-supplied system carries a `[ModCapabilities("publish:DamageEvent", "subscribe:ShootGranted")]` attribute; the loader cross-checks this attribute against the manifest at load time (cheap, no Roslyn dependency). A separate static-analysis pass runs in CI before mod publication, scanning `Services.X.Publish<T>` call sites and verifying the attribute is honest. Load-time check stays fast; CI catches drift between attribute and code reality.

---

## 4. IModApi v2

The current `IModApi` (v1) defines six methods, of which `Publish` and `Subscribe` are functionally no-ops. v2 closes the gap and adds three accessors required by §3 and §6.

### 4.1 Surface

```csharp
public interface IModApi
{
    // ── Registration (v1, semantics unchanged) ────────────────────────────
    void RegisterComponent<T>() where T : IComponent;
    void RegisterSystem<T>() where T : SystemBase;

    // ── Bus operations (v1 signatures, v2 semantics) ──────────────────────
    void Publish<T>(T evt) where T : IEvent;
    void Subscribe<T>(Action<T> handler) where T : IEvent;

    // ── Inter-mod contracts (v1, semantics unchanged) ─────────────────────
    void PublishContract<T>(T contract) where T : IModContract;
    bool TryGetContract<T>(out T? contract) where T : class, IModContract;

    // ── New in v2 ─────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the set of capability strings the kernel provides to mods
    /// in the current build.
    /// </summary>
    IReadOnlySet<string> GetKernelCapabilities();

    /// <summary>
    /// Returns the manifest of the mod making the call. Useful for
    /// self-introspection and for debug logging.
    /// </summary>
    ModManifest GetOwnManifest();

    /// <summary>
    /// Logs a structured message under the calling mod's id. Replaces
    /// ad-hoc Console.WriteLine in mod code.
    /// </summary>
    void Log(ModLogLevel level, string message);
}
```

### 4.2 `Publish<T>` semantics (closed in v2)

- The implementation routes the event to the bus whose marker the event type carries (`[Combat]`, `[Magic]`, etc.). The mapping is a compile-time attribute on the event record.
- If the mod did not declare `publish:<FQN of T>` in its manifest, the call throws `CapabilityViolationException`. This is enforced via a per-mod capability set held inside `RestrictedModApi`.
- `[Deferred]` events are queued; `[Immediate]` events are delivered synchronously. Same rules as for kernel-published events (§EVENT_BUS).

### 4.3 `Subscribe<T>` semantics (closed in v2)

- The handler is wrapped to capture the calling mod's `SystemExecutionContext` at subscribe time, so that handler invocation runs under the mod's isolation rules.
- A subscription is bound to the mod's lifetime: `RestrictedModApi.UnsubscribeAll` (called from the unload chain) removes every wrapper from the bus dispatcher.
- A capability check on `subscribe:<FQN of T>` runs at subscribe time. Without the capability, the call throws `CapabilityViolationException`.
- Multiple subscriptions to the same `T` from the same mod are permitted; each handler runs.

### 4.4 The cast-prevention rule

A mod is forbidden from casting `IModApi` to a concrete type. The `RestrictedModApi` class is `internal sealed` and its constructor is internal. The cast attempt is caught by an analyzer rule (Roslyn) that scans mod assemblies before load:

> **✓ LOCKED (D-3).** `RestrictedModApi` is `internal sealed` with `internal` constructors. Combined with the rule that a regular mod's `AssemblyLoadContext` cannot resolve `DualFrontier.Application.*` types (the assembly is loaded only into the kernel's default ALC), the type is structurally unreachable from a mod's compilation unit. No Roslyn analyzer or runtime check is required in v1. If a real bypass attempt is observed in the wild, a defensive analyzer is added as v1.x amendment — but the structural barrier is the primary defense.

### 4.5 Backward compatibility with v1

All v1 mods (using `Publish`/`Subscribe` with no-op semantics) continue to load and run, but log a v1-API warning. The mod author updates capability declarations in the manifest to migrate to functional v2 semantics. This grace period closes at kernel API version `2.0.0`.

---

## 5. Type-sharing protocol

Without a shared `AssemblyLoadContext`, two mods that both reference a third assembly load that assembly twice and obtain two distinct `Type` instances. `typeof(FireballCastEvent)` in mod A and mod B refers to two different runtime types, and a subscription registered with the bus in one mod cannot be matched against an event published from another. Every interesting cross-mod scenario breaks at this boundary.

### 5.1 The shared ALC

The kernel creates a single `AssemblyLoadContext` named `"shared"` at startup. Its `IsCollectible = false` (the shared ALC never unloads while the game runs). Its `Resolving` event delegates to the kernel's own context for `DualFrontier.*` references.

```csharp
internal sealed class SharedModLoadContext : AssemblyLoadContext
{
    public SharedModLoadContext() : base("shared", isCollectible: false) { }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // 1. DualFrontier.Contracts and friends — resolve via Default ALC.
        // 2. Other shared-mod assemblies — resolve from the shared cache.
        // 3. Otherwise — return null and let the runtime pick.
        ...
    }
}
```

### 5.2 Loader rules for shared mods

When the loader encounters a manifest with `kind: "shared"`:

1. Validate manifest as in §2.3, plus: `entryAssembly` and `entryType` must be empty.
2. Load the assembly **into the shared ALC**, not a new one.
3. Verify the assembly contains no `IMod` implementation. A shared mod with an entry point is a manifest-code mismatch and is rejected.
4. Run a reflection pass to enumerate public exported types. Cache by FQN.
5. Add the mod's `capabilities.provided` to the global capability resolver.
6. The shared mod is now loaded; dependent regular mods can begin loading.

### 5.3 Loader rules for regular mods that depend on shared mods

When loading a regular mod whose `dependencies` include a shared mod:

1. Verify all listed shared dependencies are already loaded.
2. Create a new collectible `AssemblyLoadContext` for the mod.
3. Configure the context's `Resolving` event to delegate to the shared ALC for any FQN that begins with the prefix of a depended-on shared assembly.
4. Load the mod's entry assembly.
5. Continue with the standard `IMod.Initialize` flow.

### 5.4 Restrictions on shared mods

- A shared mod must export only types: `record`, `class`, `interface`, `enum`, `struct`. Public methods are permitted only as members of these types.
- A shared mod cannot reference `DualFrontier.Core` or `DualFrontier.Application` (the same isolation rule as regular mods). It can reference only `DualFrontier.Contracts` and other shared mods.
- A shared mod cannot have a static constructor that touches mutable state. Loaders cannot guarantee initialization order across shared mods.
- A shared mod cannot read environment variables, files, or network resources at type-load time. Detection is best-effort; violations are documented in the threat model (§10).

### 5.5 Naming convention

Shared mods follow the convention `id: "<base>.protocol"` or `id: "<base>.types"`. Examples:

- `dualfrontier.vanilla.core` — shared types used by every vanilla slice (e.g., `WeaponDef`, `RecipeDef`).
- `dualfrontier.magic.protocol` — shared types for a magic-system protocol used by multiple magic mods.

The convention is enforced by the loader as a warning, not an error.

---

## 6. Three-level contracts

Mods communicate with the kernel and with each other through three distinct contract levels. Mixing levels (e.g. using a data contract for service dispatch) is a design error caught at review.

### 6.1 Level 1: Data contracts

A pure record describing a thing. No methods, no behavior, no inheritance other than `IModContract`.

```csharp
// In a shared mod (e.g. dualfrontier.vanilla.core):
public sealed record WeaponDef(
    string Id,
    int BaseDamage,
    DamageKind Kind,
    float Range,
    int AmmoPerShot
) : IModContract;
```

Use cases: weapon definitions, recipe definitions, biome parameters, faction templates. Mods publish instances via `IModApi.PublishContract`. The kernel and other mods read them via `TryGetContract`, typically iterating over the contract store at registration time.

### 6.2 Level 2: Service contracts

An interface a mod provides as a callable service. Implementations live in a regular mod; the interface lives in a shared mod so other mods can reference it at compile time.

```csharp
// In a shared mod (dualfrontier.cooking.protocol):
public interface ICookingService : IModContract
{
    bool TryCook(EntityId chef, RecipeId recipe, out CookResult result);
}

// In a regular mod (dualfrontier.vanilla.cooking):
internal sealed class VanillaCookingService : ICookingService
{
    public bool TryCook(EntityId chef, RecipeId recipe, out CookResult result) { /* ... */ }
}

// Registered at Initialize:
api.PublishContract<ICookingService>(new VanillaCookingService());
```

Use cases: pluggable behaviors where exactly one provider answers (cooking, smithing, lockpicking). Rule: the service interface is in a shared mod; the implementation is in a regular mod. A consumer fetches via `TryGetContract<ICookingService>(out var svc)` and gracefully degrades when `svc` is null.

### 6.3 Level 3: Protocol contracts

A new `IEvent` type defined by a mod, with publish/subscribe semantics. The event lives in a shared mod; publishers and subscribers are regular mods.

```csharp
// In a shared mod (dualfrontier.magic.protocol):
[Combat]
[Deferred]
public sealed record FireballCastEvent(EntityId Caster, GridVector Target, int ManaCost) : IEvent;

// Publisher (regular mod, dualfrontier.vanilla.magic):
api.Publish(new FireballCastEvent(caster, target, 25));

// Subscriber (another regular mod, dualfrontier.community.fireshield):
api.Subscribe<FireballCastEvent>(OnFireballCast);
```

Use cases: cross-mod gameplay protocols. A magic mod publishes spell events; defensive mods subscribe to add countermeasures.

### 6.4 Level matrix

| Level | Where the type lives | Where the implementation lives | How a consumer reaches it |
|---|---|---|---|
| Data | shared mod | publishing regular mod | `TryGetContract<T>` returns instance |
| Service | shared mod | publishing regular mod | `TryGetContract<T>` returns implementation |
| Protocol | shared mod | publisher and subscribers in regular mods | bus dispatch via `Publish` / `Subscribe` |

### 6.5 Anti-pattern: type in regular mod

A type used in any contract level **must** live in a shared mod. A `WeaponDef` defined in a regular mod's assembly is unreachable from another regular mod (different ALC, different `Type`). The loader rejects mods that declare contract types in non-shared assemblies.

> **✓ LOCKED (D-4).** The loader actively scans every regular-mod assembly via reflection for types implementing `IModContract` or `IEvent`. Detection rejects the mod with `ValidationErrorKind.ContractTypeInRegularMod`, naming the offending type and directing the author to the shared-mod pattern (§5). The cost (one reflection pass per load) is negligible compared to the architectural signal: contracts in regular mods break cross-mod interoperability silently, and silent breakage is what this architecture exists to prevent.

---

## 7. Bridge replacement ✓ LOCKED

Phase 5 systems (`CombatSystem`, `DamageSystem`, `ProjectileSystem`) currently exist in the kernel as `[BridgeImplementation(Phase = 5)]` stubs with empty `Update()` bodies. The bridge mechanism keeps the dependency graph valid for downstream phases that reference the system identity. When a vanilla mod ships with a real implementation, the bridge must step aside.

### 7.1 Mechanism: explicit `replaces` ✓ LOCKED

A mod listing a fully-qualified type name in `replaces`:

```json
"replaces": ["DualFrontier.Systems.Combat.CombatSystem"]
```

declares that **its** registered system supersedes the named kernel system. The loader, when applying the mod set:

1. Reads every `replaces` entry from every mod in the batch.
2. Builds the `replacedSystems` set.
3. When the kernel's bootstrap system list is being added to the dependency graph, every entry in `replacedSystems` is **skipped**. The bridge stays compiled, but never registered.
4. The mod's replacement system is registered in its place.

### 7.2 Conflict resolution

If two mods in the same load batch both list the same FQN in `replaces`, the loader rejects the batch with `ValidationError` of new kind `BridgeReplacementConflict`. The user is presented with the conflict in the mod menu and asked to disable one of the conflicting mods.

There is no automatic priority. Two combat mods cannot coexist if both replace `CombatSystem`. This is intentional: silent precedence is a debugging nightmare; explicit user choice is the architectural answer.

### 7.3 Rationale

Implicit replacement (silently letting mod systems shadow kernel systems) was rejected during Phase 0 deliberation because:

- A user investigating "why is combat acting strangely" must be able to discover, from manifests alone, which mod is responsible.
- A mod author adding combat mechanics deliberately (rather than accidentally) signals intent through `replaces`.
- The `ContractValidator` already detects write-write conflicts on components; explicit `replaces` extends the same philosophy ("conflicts are surfaced, never resolved silently") to system identity.

### 7.4 Bridge metadata

The existing `[BridgeImplementation(Phase = N)]` attribute is extended with a `Replaceable` flag:

```csharp
[BridgeImplementation(Phase = 5, Replaceable = true)]
public sealed class CombatSystem : SystemBase { /* ... */ }
```

A bridge with `Replaceable = false` cannot be replaced. The loader rejects mods that list it in `replaces`. This is the kernel's escape hatch for systems that must remain authoritative (e.g. `SystemExecutionContext` itself if it were a registered system).

### 7.5 Tests

The integration test set extends with:

- "Mod replaces a `Replaceable = true` bridge — bridge skipped, mod system runs."
- "Two mods replace same bridge — batch rejected with `BridgeReplacementConflict`."
- "Mod replaces `Replaceable = false` system — mod rejected with `ProtectedSystemReplacement`."
- "Mod replaces non-existent FQN — mod rejected with `UnknownSystemReplacement`."
- "Mod is unloaded — replacement skip is reverted, kernel bridge re-registers, dependency graph rebuilds."

---

## 8. Versioning ✓ LOCKED

Three independent SemVer axes govern the mod system. Each axis answers a distinct compatibility question.

### 8.1 Axis 1 — Kernel API version

The version of the `DualFrontier.Contracts` assembly. Already exists as `ContractsVersion.Current`; currently `1.0.0`. Bumped manually:

- **Major** — breaking change to `IModApi`, manifest schema, attribute set, or any public type signature in `Contracts`.
- **Minor** — additive change (new method on `IModApi` with a default fallback, new optional manifest field, new attribute).
- **Patch** — bug fix that does not touch the public surface.

The mod's manifest declares the required kernel API version in `apiVersion`. The loader uses `ContractsVersion.IsCompatible(required, ContractsVersion.Current)`. Caret prefix support is added (§8.4).

### 8.2 Axis 2 — Mod self version

The mod's own `version` field. Used for:

- Hot-reload lineage. When the menu reloads a mod, the loader compares the new manifest's `version` against the loaded one. A lower version triggers a warning ("you are downgrading"); equal versions are permitted (re-apply); higher versions proceed.
- User-visible identity in the mod menu.
- Save-game compatibility: each save records the set of `(modId, modVersion)` it used. Loading a save with a mod at a major version below what the save expects warns the user.

### 8.3 Axis 3 — Inter-mod dependency version

Each entry in `dependencies` is `{id, version}`, where `version` is a SemVer constraint. The loader verifies that for every dependency, the loaded mod with that id satisfies the constraint.

### 8.4 Constraint syntax: caret subset ✓ LOCKED

Three syntaxes are supported, all are subsets of npm/Cargo conventions:

- **Exact**: `"1.2.3"` — matches `1.2.3` only.
- **Caret**: `"^1.2.3"` — matches any version `>= 1.2.3` and `< 2.0.0`. The major number is pinned; minor and patch may be higher.
- **Tilde** (rejected): `"~1.2.3"` — explicitly **not supported** in v1. Reserved syntax; the parser rejects with a clear error message pointing to caret.

Range syntaxes (`">=1.0 <2.0"`) and OR (`"1.x || 2.x"`) are not supported. If a mod author needs a non-caret constraint, the design escalates to §12 for case-by-case handling.

### 8.5 Parser

`ContractsVersion.Parse` already handles strict `MAJOR.MINOR.PATCH` and silently strips a leading caret/tilde. The v2 parser:

1. Detects the prefix (`^`, exact, or `~`).
2. For `~`, throws `FormatException` with a directive to use caret instead.
3. Returns a new `VersionConstraint` struct: `{ ContractsVersion Version, ConstraintKind Kind }`.
4. `VersionConstraint.IsSatisfiedBy(ContractsVersion candidate)` evaluates per kind.

```csharp
public readonly struct VersionConstraint
{
    public ContractsVersion Version { get; }
    public ConstraintKind Kind { get; } // Exact, Caret

    public bool IsSatisfiedBy(ContractsVersion candidate) => Kind switch
    {
        ConstraintKind.Exact => candidate == Version,
        ConstraintKind.Caret =>
            candidate.Major == Version.Major
            && (candidate.Minor > Version.Minor
                || (candidate.Minor == Version.Minor && candidate.Patch >= Version.Patch)),
        _ => throw new InvalidOperationException()
    };
}
```

### 8.6 Where each axis applies

| Field | Axis | Constraint kinds allowed |
|---|---|---|
| `apiVersion` | 1 (Kernel API) | Exact or Caret |
| `version` | 2 (Mod self) | Exact only (it's a single value, not a constraint) |
| `dependencies[i].version` | 3 (Inter-mod) | Exact or Caret |

### 8.7 Resolution algorithm

Given a load batch (a list of mod manifests):

1. Build the dependency graph.
2. Topologically sort. A cycle is `ValidationError.CyclicDependency`.
3. In topological order, for each mod:
   a. Verify `apiVersion` against `ContractsVersion.Current`.
   b. For each dependency, verify the loaded version satisfies the constraint.
   c. If any check fails, the mod is added to the failed set; mods that depend on it cascade-fail.
4. The failed set is presented to the user; the success set proceeds to load.

There is no version-resolution backtracking ("find a combination that works"). The loader takes manifests at face value. A cycle or unsatisfied constraint is a user-resolvable error, not a solver problem.

---

## 9. Lifecycle

The mod lifecycle has six well-defined states. Transitions between states are atomic; failure mid-transition rolls back to the previous state.

### 9.1 States

```
        ┌──────────┐
        │ Disabled │  ← user toggled off in menu, or never enabled
        └────┬─────┘
             │ user enables
             ▼
        ┌──────────┐
        │ Pending  │  ← manifest read, not yet validated
        └────┬─────┘
             │ validate
             ▼
        ┌──────────┐
        │  Loaded  │  ← assembly in ALC, IMod.Initialize ran
        └────┬─────┘
             │ scheduler.Rebuild
             ▼
        ┌──────────┐
        │  Active  │  ← system is in the dependency graph, ticking
        └────┬─────┘
             │ user disables (or HotReload)
             ▼
        ┌──────────┐
        │ Stopping │  ← graph rebuild excludes this mod
        └────┬─────┘
             │ ALC.Unload
             ▼
        ┌──────────┐
        │ Disabled │
        └──────────┘
```

### 9.2 Hot reload through the menu ✓ LOCKED

Hot reload is supported only via the mod menu, with the simulation paused. The user flow:

1. User opens the mod menu. The menu calls `ModIntegrationPipeline.Pause()` which sets the scheduler's run flag to false.
2. User toggles mods, edits versions, clicks "Apply."
3. The menu invokes `ModIntegrationPipeline.Apply(newModSet)`. This call:
   - Builds the new graph in a local variable (existing behavior).
   - On success, calls `_scheduler.Rebuild(newPhases)`.
   - Calls `ALC.Unload()` on every mod in the previous set that is not in the new set.
4. The menu calls `ModIntegrationPipeline.Resume()` and the simulation continues from the current world state.

Reloading a mod (same id, possibly different version) follows the same flow: old version unloads, new version loads, replacement systems re-register if listed in `replaces`.

### 9.3 No live-tick reload ✓ LOCKED

The architecture explicitly forbids reloading a mod during a tick. Attempts to call `Apply` while the scheduler is running throw `InvalidOperationException("Pause the scheduler before applying mods")`. This is enforced by `ModIntegrationPipeline` checking the scheduler's run flag.

### 9.4 Save-game implications

A save records `(modId, modVersion)` for every active mod at the time of save. On load:

- If a recorded mod is missing → user warned, save loads with that mod absent (entities with components from that mod are removed; this is destructive but explicit).
- If a recorded mod is at a higher major version → user warned, save may behave incorrectly.
- If a recorded mod is at a lower or equal version → load proceeds.

The fine-grained handling of component data from missing mods is delegated to the persistence layer ([PERSISTENCE](../src/DualFrontier.Persistence/README.md)) and is out of scope for this document.

### 9.5 ALC unload protocol

`AssemblyLoadContext.Unload()` is asynchronous; the runtime waits for all references to the assembly to be released. The unload chain:

1. `RestrictedModApi.UnsubscribeAll()` — drops bus subscriptions.
2. `IModContractStore.RevokeAll(modId)` — drops contract registrations.
3. `ModRegistry.RemoveSystems(modId)` — drops system instances.
4. The dependency graph is rebuilt without this mod's systems.
5. The scheduler swaps to the new phase list.
6. `ALC.Unload()` is called.
7. The loader spins on `WeakReference.IsAlive`, polling each iteration. Before every poll the loader performs `GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect()` — the double-collect bracket is required because `WaitForPendingFinalizers` can resurrect finalizable graph nodes the first collect would have removed; the second collect picks those up, restoring monotonic progress. Default cadence: 100 iterations × 100 ms = 10 s timeout. On timeout, a `ModUnloadTimeout` warning fires; the mod is marked as a leaked reference and the user is advised to restart.

WeakReference-based unload tests are mandatory for every regular mod (§10.4).

### 9.5.1 Failure semantics

Steps 1–6 of the unload protocol (§9.5) are sequential and best-effort. If any step throws, the loader logs the exception with `(modId, stepNumber)`, surfaces a non-blocking `ValidationWarning`, and continues to the next step. After step 6, if step 7 times out, the `ModUnloadTimeout` warning per §9.5 fires; the mod is removed from the active set regardless of whether the assembly actually unloaded.

There is no atomic-unload guarantee. `Unload` is conceptually irreversible: subscriptions removed in step 1 cannot be re-attached without re-running `Subscribe`. The chain is structured so each step is a no-op if its predecessor failed (e.g. `RemoveSystems` on a mod with no registered systems is harmless), making best-effort progression safe. The `ModLoader.UnloadMod` swallowed `try/catch` around `mod.Instance.Unload()` is the canonical example of this discipline, in place since M0.

### 9.6 Hot-reload disabled mods

A mod with `"hotReload": false` cannot be reloaded mid-session. The menu disables the reload button for that mod and presents a tooltip. To change such a mod, the user must restart the game.

---

## 10. Threat model

The mod system is not a sandbox. A mod runs in-process with full .NET access. Isolation is **structural and architectural**, not security-grade. The threat model documents what the architecture catches and what it does not.

### 10.1 Architectural threats: caught

| Threat | Mechanism that catches it |
|---|---|
| Mod accesses a component without declaring `[SystemAccess]` | `SystemExecutionContext` throws `IsolationViolationException` |
| Mod publishes to a bus it did not declare | Same |
| Mod calls `GetSystem<T>()` directly | Same — always throws |
| Mod casts `IModApi` to `RestrictedModApi` | Roslyn analyzer (§4.4) or runtime check |
| Mod registers a system that conflicts with another mod's system | `ContractValidator` write-write check |
| Mod replaces a system also replaced by another mod | `BridgeReplacementConflict` |
| Mod requires a capability not provided by kernel or dependencies | `MissingCapability` |
| Mod publishes an event without `publish:` capability | `CapabilityViolationException` at `Publish` call |

### 10.2 Architectural threats: not caught

These are explicitly out of scope and documented:

- **Mod calls `Process.Kill(0)` or `Environment.Exit(1)`.** The .NET runtime gives mods full process access. We do not sandbox via AppDomain (deprecated in .NET 8) or process isolation (would break the in-process performance assumptions).
- **Mod opens network sockets, reads arbitrary files, executes shell commands.** Same reason.
- **Mod consumes unbounded memory or CPU.** Performance budgets are advisory, not enforced.
- **Mod mutates `IComponent` instances obtained via `GetComponent` after the call returns.** Component records are returned by reference for performance. A mod ignoring the [CODING_STANDARDS](./CODING_STANDARDS.md) immutability convention can corrupt state. This is caught at code review, not at runtime.
- **Mod uses reflection to access internal types.** A mod that calls `Type.GetType("DualFrontier.Core.ECS.World")` and casts to it bypasses the contract surface. The `ALC.Resolving` event refuses to load `DualFrontier.Core` into the mod's context, but a determined mod can still find loaded instances via static lookups in shared types. This is the cost of in-process execution.

### 10.3 The contract: best-effort structural isolation

Dual Frontier's mod system promises:

- A well-behaved mod (one not deliberately attempting to subvert the architecture) cannot accidentally crash the engine, corrupt state, or break other mods.
- A misbehaving mod can be detected, named, and unloaded with high probability.
- A malicious mod can break the game; the user accepts this risk by installing the mod.

This is the same contract operating systems offer to processes, scaled down: ring 3 is enforced; ring 0 is reachable through deliberate effort.

### 10.4 Required tests

The mod system test set (`tests/DualFrontier.Modding.Tests/`) covers:

- **Isolation tests.** Already in place via `IsolationGuardTests` (Phase 2).
- **Capability violation tests.** New: a mod declares no `publish:DamageEvent`, calls `api.Publish(new DamageEvent(...))`, expects `CapabilityViolationException`.
- **Bridge replacement tests.** New: every scenario from §7.5.
- **Type-sharing tests.** New: a regular mod subscribes to a shared-mod event, another regular mod publishes it; the subscriber receives the event.
- **WeakReference unload tests.** Already on the Phase 2 backlog (still open per ROADMAP). Now hard-required: every regular mod loaded in tests must unload to a successful `WeakReference.IsAlive == false` within the timeout.
- **Cross-mod cycle tests.** New: a load batch with `A → B → A` is rejected with `CyclicDependency`.
- **Version constraint tests.** New: a mod requiring `^1.0.0` of a dependency at version `1.5.3` loads; same mod at dependency version `2.0.0` is rejected.

---

## 11. Migration plan

The current state of the codebase is ~30% of the target. The migration is staged so that the engine remains buildable and the test suite remains green at every step.

### 11.1 Migration phases

| Phase | Scope | Output | Tests added |
|---|---|---|---|
| **M0** | This document at v1.0 | All ⚠ DECISION items closed | — |
| **M1** | Manifest v2 schema + parser, backward compatible | `ModManifest` extended, JSON loader handles v2 | Manifest validation tests |
| **M2** | `IModApi.Publish`/`Subscribe` real implementation | `RestrictedModApi` no longer no-ops | Publish/subscribe round-trip tests |
| **M3** | Capability model: parser, kernel-provided set, load-time + runtime check | `CapabilityRegistry`, `[ModAccessible]` attribute, `[ModCapabilities]` attribute, `RestrictedModApi.EnforceCapability` | `KernelCapabilityRegistryTests`, `CapabilityValidationTests`, capability violation tests |
| **M3.4** *(deferred)* | CI Roslyn analyzer for `[ModCapabilities]` honesty (D-2 hybrid completion) | Standalone analyzer package; runs in mod-publication CI, not at game load | Static-analysis integration tests; unblocked when first external mod author appears |
| **M4** | Shared ALC + shared mod kind | `SharedModLoadContext`, manifest `kind` field | Type-sharing test (shared-mod event published from one regular mod, received in another) |
| **M5** | Inter-mod dependency resolution with caret syntax | `VersionConstraint` parser, dependency graph in `ModIntegrationPipeline` | Version constraint tests |
| **M6** | Bridge replacement via `replaces` | `[BridgeImplementation(Replaceable=true)]`, loader skip logic | Bridge replacement tests (all of §7.5) |
| **M7** | Hot reload from menu | Pause/Resume on `ModIntegrationPipeline`, ALC unload chain | WeakReference unload tests |
| **M8** | Vanilla mod skeletons | Five empty regular mods under `mods/`, each with manifest and `IMod` stub | Smoke load tests |
| **M9** | Vanilla.Combat real implementation | Full combat mechanics in the mod | Combat mechanics tests, originally on the Phase 5 backlog |
| **M10** | Vanilla.Magic, Vanilla.Inventory, Vanilla.Pawn, Vanilla.World — incremental | One slice per cycle | Per-slice mechanics tests |

### 11.2 New error kinds in `ValidationErrorKind`

The current enum has `IncompatibleContractsVersion`, `WriteWriteConflict`, `MissingDependency`, and `CyclicDependency`. The migration adds:

- `MissingCapability` (M3)
- `BridgeReplacementConflict` (M6)
- `ProtectedSystemReplacement` (M6)
- `UnknownSystemReplacement` (M6)
- `IncompatibleVersion` (M5) — replaces ad-hoc strings currently used for `apiVersion` failures
- `SharedModWithEntryPoint` (M4)
- `ContractTypeInRegularMod` (M4) — when a regular mod exports `IModContract` types
- `CapabilityViolation` (M3) — runtime, raised as `CapabilityViolationException` (not part of the validation set, but listed here for completeness)

### 11.3 Closing `ROADMAP` debt incidentally

The migration closes Phase 2 backlog items (AssemblyLoadContext WeakReference tests — required by M7) and Phase 3 backlog items (`SocialSystem`, `SkillSystem` will move to `Vanilla.Pawn` mod where they get real implementations as M10 work). The roadmap is updated at the close of each migration phase to reflect the actual surface.

### 11.4 Stop conditions

The migration halts and escalates to the human if:

- The static analysis cost of capability cross-check (§3.7) exceeds 5 seconds per mod load.
- WeakReference unload tests are flaky (any failure rate above 0%).
- A mod author successfully bypasses capability enforcement using documented .NET features.

Each stop is a Phase 0 re-entry: the architecture is amended in this document before code resumes.

---

## 12. Locked decisions

These items were unresolved in v0.1 and were locked during Phase 0 closure (v1.0). Each is referenced from the section that introduced it. The full Question/Options/Locked-resolution structure is preserved verbatim — Options are kept for traceability, so future re-opens of any decision can read the alternatives that were considered.

### D-1. `read`/`write` capability scope

**Question.** Does a mod's `kernel.read:<Component>` capability apply to *every* public component in `DualFrontier.Components`, or only to components opted in via `[ModAccessible(Read = true)]`?

**Options.**
- **a) Blanket.** Every public component is reachable; capability is a reservation, not a gate. Simpler manifest semantics, broader access surface.
- **b) Curated, opt-in.** Only components annotated `[ModAccessible(Read = true, Write = false)]` can be requested by mods. Tighter security, requires every component author to opt in.
- **c) Curated, opt-out.** Every public component is accessible *except* those marked `[ModRestricted]`. Middle ground.

**✓ LOCKED.** (b) Curated, opt-in via `[ModAccessible(Read = true, Write = false)]`. Aligns with the project's structural-isolation philosophy. A component author actively decides what mods can touch; everything else is unreachable.

**Blocking phase.** M3 (unblocked).

### D-2. `[SystemAccess]` ↔ capability cross-check enforcement

**Question.** How does the loader verify that a registered mod system's `[SystemAccess]` declarations are a subset of the mod's manifest capabilities?

**Options.**
- **a) Static analysis at load time.** Roslyn-based scan of every `Services.X.Publish<T>` call site in the mod assembly. Heavyweight (multi-second on large mods), authoritative, no drift.
- **b) Per-system attribute.** `[ModCapabilities("publish:DamageEvent")]` on each system, manually maintained by the mod author. Lightweight, drift-prone.
- **c) Hybrid.** Attribute at load time, static analysis only as a CI check before publication.

**✓ LOCKED.** (c) Hybrid. Per-system `[ModCapabilities(...)]` attribute checked at load time; CI static analysis verifies the attribute matches actual `Publish`/`Subscribe` call sites in the assembly. Load-time stays fast; CI catches drift before release.

**Blocking phase.** M3 (unblocked).

### D-3. Cast-prevention enforcement

**Question.** How is the rule "a mod cannot cast `IModApi` to a concrete type" enforced?

**Options.**
- **a) Roslyn analyzer at load time.** Scans for `(RestrictedModApi)api` and similar patterns. Slow, requires shipping the analyzer.
- **b) Runtime check on first use.** Cheaper but allows the cast to compile and run before being detected.
- **c) Make `RestrictedModApi` `internal sealed` and rely on the type being unreachable from a mod's ALC.** This is structurally true today.

**✓ LOCKED.** (c) Structural barrier only in v1. `RestrictedModApi` is `internal sealed`; its containing assembly (`DualFrontier.Application`) is not resolvable from a mod's ALC. The cast cannot compile against the kernel's actual type. (a) is held in reserve for v1.x if a real bypass attempt is observed.

**Blocking phase.** M2 (unblocked).

### D-4. Loader scan for `IModContract`/`IEvent` in regular mods

**Question.** Does the loader actively scan a regular mod's exported types for `IModContract` or `IEvent` implementations and reject the mod if found?

**Options.**
- **a) Active scan, reject.** Strong signal to mod authors that contracts belong in shared mods.
- **b) Passive: warn but load.** Mods with mistakes still work, with a runtime warning.
- **c) Documentation-only.** Rely on review and code style.

**✓ LOCKED.** (a) Active scan and reject. The reflection cost is negligible; the architectural signal — that contract types belong in shared mods, period — is large enough to justify load-time enforcement.

**Blocking phase.** M4 (unblocked).

### D-5. Shared-mod cycle detection

**Question.** Are cycles in the shared-mod dependency graph forbidden? The architecture in §1.4 says yes, but enforcement requires the loader to refuse such configurations explicitly.

**✓ LOCKED.** Forbidden. The loader rejects shared-mod cycles with `ValidationErrorKind.CyclicDependency`. Cycles break compilation order at the mod author's IDE before reaching the loader, but explicit runtime rejection is cheaper than implicit failure further downstream.

**Blocking phase.** M4 (unblocked).

### D-6. Save-game compatibility policy when a mod is missing

**Question.** When loading a save that recorded a mod no longer present, what happens to entity components owned by that mod?

**Options.**
- **a) Strip components, keep entities.** The entity continues to exist with reduced state.
- **b) Strip entities entirely.** The world loses everything tied to the absent mod.
- **c) Refuse to load.** Save is bound to its mod set; missing mods make it unloadable.

**✓ LOCKED.** (a) Default: strip components owned by the missing mod, keep entities; load with a clear warning naming each missing mod. (c) Available as a user-toggle "strict mod compatibility" setting; when enabled, the save refuses to load. Workshop reality is that mods get abandoned, and the community patches around them — strict-by-default would block too many real save loads.

**Blocking phase.** Out of M0–M10 scope. Tracked here for the persistence rework that will implement the component-stripping logic.

### D-7. `hotReload: false` semantics for vanilla mods

**Question.** Vanilla mods are the engine team's reference implementation. Does it make sense to allow them to be hot-reloaded? A hot-reload of `Vanilla.Combat` changes core combat math during a session.

**Options.**
- **a) Vanilla mods are hot-reloadable like any other.** Maximum testability of the mod system.
- **b) Vanilla mods set `hotReload: false` by default.** Stable session experience.

**✓ LOCKED.** Hybrid by build flavor. Vanilla mods declare `hotReload: true` in source; the shipping build pipeline rewrites this to `hotReload: false` in release manifests. Development gets free hot-reload testing of vanilla mechanics; shipped builds get stable session experience. Override is a single flag in the build script — no code branching needed.

**Blocking phase.** M7 (unblocked).

---

## See also

- [METHODOLOGY](./METHODOLOGY.md) — the development pipeline; this architecture is the artifact of the same methodology applied to the engine's modding layer.
- [ARCHITECTURE](./ARCHITECTURE.md) — the four layers; mods live above Domain through `IModApi`.
- [MODDING](./MODDING.md) — the existing v1 mod-author guide; this document specifies v2.
- [MOD_PIPELINE](./MOD_PIPELINE.md) — the existing pipeline implementation; M2–M7 extend it.
- [CONTRACTS](./CONTRACTS.md) — bus and marker conventions; capability syntax mirrors bus naming.
- [ISOLATION](./ISOLATION.md) — `SystemExecutionContext`; capability enforcement at runtime delegates here.
- [ROADMAP](./ROADMAP.md) — phase order; the migration plan in §11 is the new authoritative sequence for the mod-OS work.
- `src/DualFrontier.Contracts/Modding/` — the source-of-truth surface for `IMod`, `IModApi`, `IModContract`, `ContractsVersion`, `ModManifest`.
- [GPU_COMPUTE](./GPU_COMPUTE.md) — **v2.0 LOCKED.** Field-based GPU compute as a foundational architectural capability. Per K-L9, vanilla and third-party mods register fields and compute pipelines through the same `IModApi` (see "Mod-driven shader registration" and "Mod parity (KERNEL K-L9)").

## Modding с native ECS kernel

When kernel migration к native completes (K-series, see `KERNEL_ARCHITECTURE.md`):
- Mod component types must be `unmanaged` structs (Path α)
- Class-based component storage prohibited (через ECS — mod state classes acceptable outside ECS)
- Vanilla mods register components и systems through same IModApi as third-party (vanilla = mods principle preserved)
- Mod replacement triggers second-graph rebuild (managed) — native side untouched
- Mod fields (`RawTileField<T>`) и compute pipelines registered through same `IModApi` extension (`api.Fields` / `api.ComputePipelines`); see [GPU_COMPUTE](./GPU_COMPUTE.md) "Architectural integration → Mod-driven shader registration"

See `KERNEL_ARCHITECTURE.md` §1.9 (mod system registration) и §1.10 (component type registry) для full detail. See [GPU_COMPUTE](./GPU_COMPUTE.md) "Architectural integration with existing systems → Mod parity (KERNEL K-L9)" для GPU compute mod-parity invariant.
