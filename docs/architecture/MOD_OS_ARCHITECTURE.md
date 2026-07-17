---
register_id: DOC-A-MOD_OS_ARCHITECTURE
project: Dual Frontier
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 1.0.0
first_authored: 2026-07-15
last_modified: 2026-07-17
content_language: en
next_review_due: 2027-Q3
title: Mod OS Architecture (authored rework; merges MOD_PIPELINE + ISOLATION — one unload chain, one fault timing, one capability grammar)
supersedes:
- DOC-A-MOD_OS
- DOC-A-MOD_PIPELINE
- DOC-A-ISOLATION
last_modified_commit: 4a36abe
review_cadence: on-change+annual
last_review_date: 2026-07-17
last_review_event: "CORPUS_CLOSURE_INVERSION_B: D1 full-corpus review (141 anchors — the wave's largest) + HALT-1-ratified corrections R2-4..R2-8 at 4a36abe (EffectiveApiVersion mechanism truth; phantom loader warning retired; Path β reclamation honesty); ratified AUTHORED → LOCKED v1.0.0 at Phase C (EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION); EVT-54 rider executed at 717641b (METHODOLOGY/MAX_ENG ISOLATION links retargeted forward)."
reviewer: Crystalka
special_case_rationale: 'Ratified LOCKED v1.0.0 2026-07-17 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION (checklist item [1]). Merged successor of DOC-A-MOD_OS + DOC-A-MOD_PIPELINE + DOC-A-ISOLATION per EVT-2026-07-15-CORPUS_REWORK_R2_PLATFORM: one unload chain, one fault timing, one capability grammar, 15-member ValidationErrorKind; commit/reclaim split per the ENGINE_LIFECYCLE draft.'
---

# Mod OS Architecture

The mod platform of Dual Frontier — topology, manifest, capabilities, `IModApi`, contracts, versioning, the integration pipeline, the lifecycle (one apply transaction, one unload chain, one fault path), the isolation model, and the threat model, stated once and verified against code.

> **Document class: authored-rework (current-truth candidate).** Successor of `docs/architecture/historical/MOD_OS_ARCHITECTURE.md` (DOC-A-MOD_OS, now SUPERSEDED), `docs/architecture/historical/MOD_PIPELINE.md` (DOC-A-MOD_PIPELINE, now SUPERSEDED), and `docs/architecture/historical/ISOLATION.md` (DOC-A-ISOLATION, now SUPERSEDED) — the three documents are **merged** here because they described one subsystem in three voices and had drifted into mutual contradiction (unload step order, fault-unload timing, enum size, capability grammar; session report §3 C7/C8, §6.1 N-8/N-9/N-11). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`. Becomes the LOCKED authority upon Crystalka ratification per [FRAMEWORK.md](../governance/FRAMEWORK.md) §7; until then the predecessors remain the last-ratified reference and prevail on conflict.
> **Ratification checklist:** [ ] content spot-audit at ratification HEAD · [ ] lifecycle AUTHORED → LOCKED, version → 1.0.0 · [ ] `next_review_due` set · [ ] predecessor register rationale updated (all three predecessors) · [ ] D-1…D-7 lock texts re-verified against §§3–9 · [ ] §9 commit/reclaim framing cross-checked against the ENGINE_LIFECYCLE_AND_TRANSACTIONS ratification outcome.

**Resolution law used throughout.** Where the three predecessors disagreed, the code is the truth. Every resolved conflict in this document names the winning behavior with a `file:line` anchor and retires the losing wording; the retirement is recorded in one line at the point of resolution, not in a chronicle.

| | |
|---|---|
| **Role** | normative-current-candidate |
| **Successor of** | `docs/architecture/historical/MOD_OS_ARCHITECTURE.md` (DOC-A-MOD_OS) · `docs/architecture/historical/MOD_PIPELINE.md` (DOC-A-MOD_PIPELINE) · `docs/architecture/historical/ISOLATION.md` (DOC-A-ISOLATION) |
| **Scope** | Structural relationship kernel ↔ mod; manifest schema and parser behavior; capability declaration/grammar/enforcement; `IModApi` surface and semantics; type sharing across `AssemblyLoadContext` boundaries; inter-mod contracts; bridge replacement; three-axis versioning; the integration pipeline (`Apply`), the unload chain, the fault path; system isolation enforcement; the threat model; the `ValidationErrorKind` registry. |
| **Non-goals** | Gameplay content and balance (mod authors' domain); per-system performance budgets ([PERFORMANCE.md](./PERFORMANCE.md)); bus dispatch semantics ([EVENT_BUS.md](./EVENT_BUS.md)); author-facing tutorials and the Mod SDK reference-set statement ([MODDING.md](./MODDING.md)); migration/milestone state ([docs/ROADMAP.md](../ROADMAP.md)); persistence of mod data (PERSISTENCE_SNAPSHOT_CONTRACT.md, AUTHORED draft). |
| **Authority domains** | mod-lifecycle (load, apply, unload, fault, hot reload) · boundary/layering **for mods** (ALC model, compile surface, cast prevention) · error-handling **for mod faults** (fault path, `ValidationErrorKind`, warning discipline) |
| **Defers to** | [MODDING.md](./MODDING.md) → mod-author guidance and the SDK assembly set · [EVENT_BUS.md](./EVENT_BUS.md) → bus tiers, delivery and flush semantics · [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) Part 0 → К-L9 (vanilla = mods), К-L18 (quiescent state), К-L3/К-L3.1 (storage paths) · ENGINE_LIFECYCLE_AND_TRANSACTIONS.md (AUTHORED draft) → transition vocabulary (prepare/validate/quiesce/commit/reclaim/recover/resume) · [ANALYZER_RULES.md](./ANALYZER_RULES.md) → shipped analyzer surface · [docs/ROADMAP.md](../ROADMAP.md) → migration state, deferred milestones |

**Strategic locked decisions** (Phase 0, unchanged by this rework): (1) capability granularity is per-event-type and per-component-type; (2) bridge replacement is explicit via `replaces`; (3) hot reload only through the mod menu with the simulation paused; (4) vanilla ships as multiple mods; (5) three-tier SemVer versioning. The seven detail locks D-1…D-7 are preserved inline in the sections that own them (D-1/D-2 → §3, D-3 → §4, D-4/D-5 → §5, D-6/D-7 → §9); their Question/Options deliberation history is preserved in the historical predecessor only. The "stop, escalate, lock" rule stands: a design question this document does not answer is documented and escalated, never improvised in code.

---

## §0 Executive summary — the OS mapping

The design treats Dual Frontier as a small operating system and each mod as a process running on top of it. Every row names the engine counterpart and the on-disk artifact that realizes it; every row is shipped.

| OS concept | Dual Frontier counterpart | Realized by |
|---|---|---|
| Kernel | Native ECS core behind `DualFrontier.Core` + `DualFrontier.Contracts` | `NativeWorld` interop surface; [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) |
| Process | A mod loaded into its own `AssemblyLoadContext` | `ModLoadContext` (collectible per regular mod; `ModLoadContext.cs:29-33`) |
| Process isolation | Structural: Contracts-only compile surface, `internal sealed` implementation types, `[SystemAccess]`-driven scheduling | §1.4 boundary statement; `DependencyGraph`; `SystemExecutionContext` |
| Syscall | `IModApi` | `RestrictedModApi` (`RestrictedModApi.cs:38`); `Publish`/`Subscribe` routed via `ModBusRouter` to the `IGameServices` domain buses |
| IPC between processes | `IModContract` via `IModContractStore` | `ModContractStore`; three contract levels (§5.6) |
| Device driver | A mod-supplied system registered via `RegisterSystem` | `ModRegistry` + scheduler graph rebuild |
| Registering a new "syscall"/device | Mod adding new event/contract types | Shared mods + `capabilities.provided` resolution (`ContractValidator` Phase C) |
| Shared library | A library-only mod with shared types, no entry point | `SharedModLoadContext` (`SharedModLoadContext.cs:21`) |
| `dlopen` / hot reload | Pause-aware reload via the mod menu | `ModIntegrationPipeline.Pause()/Apply()/Resume()` + the §9.4 unload chain |
| Capability model | Manifest `capabilities` + load-time and runtime checks | `KernelCapabilityRegistry`, `[ModAccessible]`, `ContractValidator` Phases C+D, `CapabilityViolationException` |
| Package-manager dependency resolution | Manifest `dependencies` with SemVer constraints | `VersionConstraint`, `ContractValidator` Phases A+G, topological load order |

Migration state (which milestones are closed, pending, or deferred) is tracked in [docs/ROADMAP.md](../ROADMAP.md), not here.

---

## §1 Mod topology and the ALC model

### §1.1 Three mod kinds

**Regular mod** — the default kind. Has an entry point (`IMod` implementation), runs `Initialize(api)` to register components, systems, and subscriptions, may publish and consume contracts. Lives in its own collectible `AssemblyLoadContext`. May depend on shared mods; may not depend on regular mods directly — only on contracts they publish. Manifest `kind: "regular"` (default when omitted; `ManifestParser.cs:167-190`).

**Shared mod (library)** — defines types (`record`s, `interface`s, `enum`s) that other mods reference at compile time. Has **no `IMod` implementation** and registers no systems; it is a pure type vendor. The loader places its assembly in the single shared ALC so dependent mods see the same `Type` instances (§5). Manifest `kind: "shared"`, required. `ModLoader.LoadSharedMod` defensively rejects a non-shared manifest routed to it (`ModLoader.cs:140-143`); architectural compliance (no entry point, no `IMod`) is enforced by `ContractValidator` Phase F as `SharedModWithEntryPoint` (§12).

**Vanilla mod** — a regular mod authored by the engine team, shipped with the base game. Architecturally identical to a third-party regular mod: same manifest schema, same `IModApi`, same isolation rules, per К-L9 «vanilla = mods» (KERNEL_ARCHITECTURE.md Part 0). Conventions: id begins `dualfrontier.vanilla.`; shipped under `mods/DualFrontier.Mod.Vanilla.<Slice>/`. On-disk set: `Vanilla.Combat`, `Vanilla.Core` (shared), `Vanilla.Inventory`, `Vanilla.Magic`, `Vanilla.Pawn`, `Vanilla.World`, plus `Mod.Example` — seven projects, each referencing exactly one engine assembly (§1.4).

### §1.2 Load graph invariants

- The shared ALC is created once per pipeline instance and never unloaded during the session (`isCollectible: false`, name `"shared"` — `SharedModLoadContext.cs:31-34`; owned singleton at `ModIntegrationPipeline.cs:84`).
- Each regular mod gets its own ALC with `IsCollectible = true` (`ModLoadContext.cs:29-33`), allowing unload.
- A regular mod's ALC may resolve types from the shared ALC, never from another regular ALC (§1.3).
- Cycles between regular mods are forbidden; cycles among shared mods are forbidden (D-5, §5.5). Both are rejected before any assembly load (`ModIntegrationPipeline.cs:270-310`).
- Shared mods load before any dependent regular mod: the pipeline runs two passes in topological order (`ModIntegrationPipeline.cs:320-362`).

### §1.3 What the mod ALC actually does — resolution semantics

Code truth (`ModLoadContext.Load`, `ModLoadContext.cs:45-54`):

1. If the requested simple name is cached in the shared ALC, return that assembly — this preserves cross-mod type identity (§5).
2. Otherwise return `null`, which hands resolution to the runtime's default logic — in practice the **Default ALC**, where `DualFrontier.Contracts` (and the rest of the engine) is loaded.
3. The mod's own assemblies, loaded via `LoadFromAssemblyPath` (`ModLoader.cs:89`), stay inside the private context — isolated from other mods and collectible on unload.

`SharedModLoadContext.Load` behaves the same way for shared mods: cached shared assemblies resolve locally; everything else — notably `DualFrontier.Contracts` — falls through to the Default ALC (`SharedModLoadContext.cs:84-92`). Duplicate simple names are rejected at shared load time (`SharedModLoadContext.cs:59-61`).

### §1.4 The boundary statement (definitive; replaces the predecessor "refusal list")

The retired MODDING.md and ISOLATION.md texts described the mod ALC as **refusing** to resolve `DualFrontier.Core`, `DualFrontier.Systems`, `DualFrontier.Components`, `DualFrontier.Events`, and `DualFrontier.Application` ("the load attempt fails… `ModLoadContext` will not supply the assembly"). **That mechanism does not exist.** There is no name-based blocklist anywhere in `ModLoadContext` or `SharedModLoadContext`; a mod assembly that ships a reference to a kernel assembly will resolve it at runtime through the Default-ALC fall-through described in §1.3. The session report records this as the C8 incoherence; the resolution is to state the real boundary, which is layered and load-bearing without any refusal list:

1. **Compile surface (primary).** Mods compile against `DualFrontier.Contracts` and nothing else. Verified census at HEAD: all seven mod projects reference exactly one engine project (`mods/DualFrontier.Mod.Vanilla.Combat/DualFrontier.Mod.Vanilla.Combat.csproj:9` and siblings; likewise every test fixture, with fixture→fixture references only for the shared-mod pattern). A mod additionally references the shared mods it depends on. This — not the ALC — is what makes `DualFrontier.Core`/`Application` types unnameable from a mod's compilation unit. The authoritative author-facing statement of the SDK reference set lives in [MODDING.md](./MODDING.md).
2. **Implementation visibility (second layer).** The concrete API type is `internal sealed` with internal construction (`RestrictedModApi.cs:38,67`), so even a mod that somehow obtained `DualFrontier.Application.dll` cannot compile a cast against it (D-3, §4.5).
3. **Declaration-driven scheduling (third layer).** `[SystemAccess]` declarations feed `DependencyGraph` edge-building; component access flows through `NativeWorld` span/batch APIs or per-mod Path β stores (§10.1).
4. **Honesty (threat model).** A *determined* mod running in-process can still reach kernel types via reflection over loaded assemblies. That is accepted and documented in §11 — the boundary is structural, not security-grade.

The ALC's real contributions are exactly two: **collectibility** (a regular mod can be physically unloaded) and **type identity** (shared types resolve to one `Type` instance across mods). Isolation claims beyond those two properties in the predecessor documents are retired.

---

## §2 Manifest v3

The manifest schema is **strict v3**: every `mod.manifest.json` must declare `"manifestVersion": "3"`, and `ManifestParser.Parse` rejects a missing field or any other value at parse time (`ManifestParser.cs:53-59`). There is no backward-compatible extension path and no grace period — the К8.3+К8.4 cutover removed v1/v2 manifest acceptance together with the v2 `IModApi` (§4.4). The vanilla mod skeletons under `mods/DualFrontier.Mod.Vanilla.*/mod.manifest.json` are the on-disk reference instances.

### §2.1 Schema

```json
{
  "manifestVersion": "3",
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
    { "id": "dualfrontier.vanilla.inventory", "version": "^1.0.0", "optional": false }
  ],

  "replaces": [
    "DualFrontier.Systems.Combat.CombatSystem"
  ],

  "capabilities": {
    "required": [
      "kernel.publish:DualFrontier.Events.Combat.DamageEvent",
      "kernel.subscribe:DualFrontier.Events.Combat.ShootGranted",
      "kernel.read:DualFrontier.Components.Combat.WeaponComponent",
      "kernel.write:DualFrontier.Components.Shared.HealthComponent"
    ],
    "provided": [
      "mod.dualfrontier.vanilla.combat.publish:DualFrontier.Mod.Vanilla.Combat.WeaponDef"
    ]
  }
}
```

### §2.2 Field reference

Field names below are the parsed truth (`ModManifest`, `src/DualFrontier.Contracts/Modding/ModManifest.cs:50-145`; reader behavior per `ManifestParser.cs`). Property lookup is **case-insensitive** (`ManifestParser.cs:98-110`); JSON comments and trailing commas are tolerated (`ManifestParser.cs:31-35`).

| Field | Type | Required | Default | Notes |
|---|---|---|---|---|
| `manifestVersion` | string | yes | — | Must be exactly `"3"`; anything else is rejected at parse time with the `ValidationErrorKind.IncompatibleContractsVersion` semantic in the message (`ManifestParser.cs:53-59`). |
| `id` | string | yes | — | Reverse-domain, globally unique. Empty id also rejected at load (`ModLoader.cs:71-73`). |
| `name` | string | yes | — | Human-readable. |
| `version` | string (SemVer) | yes | — | Mod self-version, strict `MAJOR.MINOR.PATCH` (axis 2, §7.2). |
| `author` | string | no | `""` | Free-form (`ManifestParser.cs:65`). |
| `kind` | enum | no | `"regular"` | `regular` or `shared`; unknown values rejected (`ManifestParser.cs:167-190`). |
| `apiVersion` | string (SemVer, optional `^`) | no | — | Compatibility constraint against `ContractsVersion.Current`. When absent, Phase A falls back to parsing `requiresContractsVersion` via `ContractsVersion.Parse` + `IsCompatible` (`ContractValidator.cs:128-159`); `ModManifest.EffectiveApiVersion` (`ModManifest.cs:145`) encodes the same preference order but has no production caller. |
| `requiresContractsVersion` | string (SemVer) | no | `"1.0.0"` | v1-era field retained solely as the `apiVersion` fallback (`ManifestParser.cs:66`). New manifests should declare `apiVersion`. |
| `entryAssembly` | string | conditional | `"{id}.dll"` | Required for `kind=regular` (default derived from id at load — `ModLoader.cs:81-83`); **must be empty for `kind=shared`** — enforced by `ContractValidator` Phase F, not the parser. |
| `entryType` | string | conditional | scan-for-`IMod` | For `kind=regular`: when omitted the loader reflection-scans; multiple `IMod` implementations without an explicit `entryType` are rejected (`ModLoader.cs:328-346`). **Must be empty for `kind=shared`** (Phase F). |
| `hotReload` | bool | no | `false` | When `false`, the menu refuses to toggle a currently-active instance of the mod (§9.7; `EditableModInfo.CanToggle`). |
| `dependencies` | array | no | `[]` | Entries are **all strings** (id only) or **all objects** `{id, version, optional}` — mixing forms in one array is rejected (`ManifestParser.cs:274-285`). `version` is a `VersionConstraint` (§7.4); `optional` (default `false`) downgrades a missing dependency from `MissingDependency` error to a non-blocking warning (`ModIntegrationPipeline.cs:1146-1170`). |
| `replaces` | array of string (FQN) | no | `[]` | Fully-qualified system type names this mod replaces (§6). Meaningful for `kind=regular` only; Phase F rejects shared mods that populate it. |
| `capabilities.required` / `.provided` | array of string | no | `[]` | Every token must match the §3.2 grammar; the offending token is named in the parse error (`ManifestParser.cs:361-385`, `ManifestCapabilities.cs:83-94,175-190`). |

### §2.3 Validation — parse time vs batch time

**Parse time** (`ManifestParser.Parse`; any failure throws `InvalidOperationException` naming the manifest path): the strict-v3 gate; `id`/`name`/`version` present and non-empty; `apiVersion` and object-form dependency `version`s parse as `VersionConstraint` (`^`-caret or exact; tilde rejected with a directive to use caret — `VersionConstraint.cs:56-60`); `kind` is `regular` or `shared`; dependency array unmixed with non-empty ids; every capability token matches the §3.2 grammar.

**Batch time** (`ContractValidator.Validate` — `ContractValidator.cs:76`; produces typed `ValidationError`s per §12): Phase A kernel API version compatibility (`:124`), Phase B write-write conflicts (`:196`), Phase E contract-type placement per D-4 (`:329`), Phase C capability resolution against kernel + listed dependencies (`:398`), Phase D `[ModCapabilities]` cross-check (`:442`), Phase G inter-mod dependency versions (`:496`), Phase H `replaces` validation including the cross-batch conflict check (`:566`), Phase F shared-mod compliance including the empty-entry-point rule (`:698`). Additionally the pipeline itself performs shared- and regular-mod cycle detection and dependency-presence checks before any assembly loads (§8.2 steps [0.5]/[0.6]).

---

## §3 Capability model

Capabilities are **named, declared, statically checked permissions**. Every operation a mod performs against the kernel or another mod must be backed by an entry in `capabilities.required`; capabilities the mod adds to the system (new event types, contracts, fields) appear in `capabilities.provided`.

### §3.1 Granularity ✓ LOCKED (D-1 companion)

Granularity is **per-event-type and per-component-type**. A capability never applies to a category, namespace, or wildcard: `kernel.publish:DualFrontier.Events.Combat.DamageEvent` is permitted; `kernel.publish:DualFrontier.Events.Combat.*`, `kernel.publish:combat`, and `kernel.publish:*` are forbidden (and fail the §3.2 grammar). The cost is verbose manifests for content-rich mods; the benefit is that a `git diff` of a manifest reveals exactly which new kernel surface the mod began touching.

### §3.2 The grammar — one authority

There is exactly **one** capability grammar, and it is the compiled pattern in `ManifestCapabilities` (`src/DualFrontier.Contracts/Modding/ManifestCapabilities.cs:24-26`), applied to every token in both manifest lists at parse time:

```
^(kernel|mod\.[a-z0-9.]+)\.((?:fast|normal|background)\.(?:publish|subscribe)
  |publish|subscribe|read|write
  |field\.(read|write|acquire|conductivity|storage|dispatch)
  |pipeline\.register)
:[A-Za-z][A-Za-z0-9_.]+$
```

(line-wrapped here for readability; the source is a single-line regex). Decomposed:

- **provider** — `kernel` (provided by the kernel surface, §3.4) or `mod.<modId>` (provided by another loaded mod, typically a shared mod).
- **verb** — one of: `publish`, `subscribe` (Normal-tier aliases); tier-prefixed `fast.publish`, `fast.subscribe`, `normal.publish`, `normal.subscribe`, `background.publish`, `background.subscribe`; `read`, `write` (entity-keyed components); `field.read`, `field.write`, `field.acquire`, `field.conductivity`, `field.storage`, `field.dispatch` (spatial fields); `pipeline.register` (reserved for the compute surface, §4.3).
- **subject** — the C# FQN of the event or component type, or the namespaced field/pipeline id for `field.*`/`pipeline.*`.

**Resolved (was the §3.2-vs-§2.3 self-contradiction, session N-11):** `layer.intent` and `layer.combat_feedback` are **not capability verbs and cannot appear in a manifest** — the grammar above has no `layer` alternative, so `ManifestCapabilities.Parse` rejects any such token and the manifest fails to parse. What does exist: `KernelCapabilityRegistry` **emits** `kernel.layer.intent:{FQN}` / `kernel.layer.combat_feedback:{FQN}` tokens into the kernel-provided set for `[Layer(LayerType.Intent|CombatFeedback)]`-annotated classes (`KernelCapabilityRegistry.cs:152-170`, per К-L17, KERNEL_ARCHITECTURE.md Part 0), and mods can observe those strings through `GetKernelCapabilities()`. They are registry-emitted descriptors of the display-composition surface, not declarable permissions. The predecessor's listing of `layer.*` among manifest verbs is retired.

Tier semantics (declaration side only; dispatch semantics are owned by [EVENT_BUS.md](./EVENT_BUS.md)): `fast.*` marks Fast-tier (bounded-execution contract, `FastTierContractViolation` §12), `background.*` marks Background-tier (coalesce declaration required, `BackgroundCoalesceMissing` §12), and un-prefixed `publish`/`subscribe` remain valid Normal-tier aliases. A manifest tier not matching the event type's `[EventTier]` is `BusTierMismatch` (§12).

Field verb semantics (storage contract owned by [FIELDS.md](./FIELDS.md)): `field.read`/`field.write` — point access; `field.acquire` — dense span lease; `field.conductivity`/`field.storage` — per-cell diffusion/capacitance maps; `field.dispatch` — issue a compute dispatch through a registered pipeline (no separate `pipeline.dispatch` verb exists — the field-side verb covers it).

### §3.3 Reserved namespaces

`kernel.*` is reserved for the kernel's own provided set (§3.4); a mod cannot list `kernel.*` tokens in `capabilities.provided`. `mod.<modId>.*` is reserved for the mod with that exact id; mods cannot provide capabilities under another mod's namespace (Phase C resolves provider namespaces against actual mod ids).

### §3.4 Kernel-provided capability set

The kernel's provided set is built **once at pipeline construction** by a reflection scan — `KernelCapabilityRegistry.BuildFromKernelAssemblies()` over `DualFrontier.Contracts`, `DualFrontier.Components`, and `DualFrontier.Events` (`KernelCapabilityRegistry.cs:82-89`; instantiated at `ModIntegrationPipeline.cs:83`). Emission rules (`KernelCapabilityRegistry.cs:91-171`):

- Public, concrete `IEvent` implementer → tier-prefixed `kernel.<tier>.publish/subscribe:{FQN}` per its `[EventTier]` (Normal when unattributed), plus the legacy un-prefixed `kernel.publish/subscribe:{FQN}` aliases for Normal-tier types.
- Public, concrete `IComponent` implementer annotated `[ModAccessible(Read/Write)]` → `kernel.read:{FQN}` / `kernel.write:{FQN}`.
- `[Layer]`-annotated class → the К-L17 layer tokens (observable, not declarable — §3.2).
- Generic and nested types (FQN containing `` ` `` or `+`) are silently skipped.

No `field.*` token is kernel-emitted: field capabilities are mod-namespace tokens (`mod.<modId>.field.*:<id>`) enforced at the field API boundary (§4.3). The predecessor's v1.6 sketch of kernel-provided "infrastructure field verbs" (`kernel.field.acquire` etc.) never matched the implementation and is retired.

Mods read the resulting set through `IModApi.GetKernelCapabilities()` (`RestrictedModApi.cs:203`).

> **✓ LOCKED (D-1).** `read`/`write` capabilities apply only to a **curated, opt-in subset** of public components: a component is reachable from a mod only when annotated `[ModAccessible(Read = …, Write = …)]`. The component author actively decides what mods can touch; everything else is invisible to the capability resolver and produces `MissingCapability` if requested. Path orthogonality (K-L3.1): the attribute targets `Class | Struct` (`ModAccessibleAttribute.cs:15`) and applies uniformly across Path α (`unmanaged struct`, `NativeWorld` storage) and Path β (managed `class` via `[ManagedStorage]`); capability strings are path-blind — the provider prefix differs by ownership (kernel vs mod), never by storage path.

### §3.5 Static check at load time

For every `capabilities.required` token of a mod, an identical string must exist in the kernel-provided set **or** in the `capabilities.provided` set of a mod listed in this mod's `dependencies` (Phase C, `ContractValidator.cs:398`). A required capability cannot be satisfied by a mod *not* listed in `dependencies` — implicit dependency through shared capability is forbidden. Failure: `MissingCapability`, naming each unsatisfied token. The pipeline passes the kernel registry into validation explicitly (`ModIntegrationPipeline.cs:366-375`) — omitting it once silently skipped Phases C/D, which is why the parameter is now required on the production path.

### §3.6 Hybrid enforcement — load-time plus runtime

**Load-time (primary gate, before the mod's systems reach the scheduler):** Phase C token satisfiability (§3.5); Phase D `[ModCapabilities]` attribute cross-check (§3.7); Phase B/E/F/G/H structural checks (§2.3).

**Runtime (second-layer defence inside `RestrictedModApi.EnforceCapability`, `RestrictedModApi.cs:230-247`):** `Publish<T>` and `Subscribe<T>` check the per-mod required set at call/subscribe time and throw `CapabilityViolationException` on a miss. The check is a hash-set lookup against the token `kernel.<verb>:<FQN>` — O(1), measured negligible on the hot path.

The runtime layer covers what the load-time gate cannot reach: reflection-constructed event types, deliberately dishonest `[ModCapabilities]` attributes, and the **empty-capability leniency**: when a manifest declares no capabilities at all (`ManifestCapabilities.IsEmpty` — both lists empty or absent, `ManifestCapabilities.cs:55-57`), `EnforceCapability` short-circuits with a console warning instead of throwing (`RestrictedModApi.cs:234-241`). This keeps capability-less skeleton mods (the on-disk vanilla skeletons ship empty lists) loadable and publishing; a manifest that declares at least one capability is fully enforced on every call. (Code honesty note: the console warning's wording still speaks of a "v1 manifest grace period" — text drift from the pre-cutover era; the *behavior* is the empty-capability short-circuit described here, and v1/v2 manifests cannot reach it because the parser rejects them.)

Runtime field-capability enforcement is separate and unconditional (§4.3).

### §3.7 `[SystemAccess]` and `[ModCapabilities]` cross-checks

A mod's `capabilities.required` must be a superset of every registered system's declared surface. Two mechanisms:

- `[SystemAccess]` declares component reads/writes and buses per system; `ModRegistry.RegisterSystem` requires the attribute's presence (with `[TickRate]`) at registration time, and the declarations drive scheduler edge-building (§10.1).
- `[ModCapabilities("publish:DamageEvent", …)]` on each mod system carries the per-system bus tokens; Phase D verifies every attribute token appears in the manifest's `capabilities.required` (`ContractValidator.cs:442`). Drift between manifest and code is a load-time error, never silent.

> **✓ LOCKED (D-2).** Hybrid enforcement: the per-system `[ModCapabilities(…)]` attribute is cross-checked against the manifest at load time (cheap, no Roslyn dependency); a separate static-analysis pass verifying the attribute is honest against actual `Publish`/`Subscribe` call sites runs in CI before mod publication.
>
> **FENCED (target / planned — not current truth):** the CI-side Roslyn honesty scan (D-2 completion, historically "M3.4") does not exist on disk; no load-time static analysis of call sites exists either. Runtime `CapabilityViolationException` already catches dishonest attributes, so the scan is developer-experience tooling, not a safety boundary. Tracked in [docs/ROADMAP.md](../ROADMAP.md) §Analyzer track / Mod-OS rows.

---

## §4 IModApi

`IModApi` is a single, strict surface — version 3. A manifest's `manifestVersion: "3"` declares compatibility with exactly this surface. The interface below is quoted from `src/DualFrontier.Contracts/Modding/IModApi.cs:28-127` (the source of truth); signatures verbatim, comments condensed.

```csharp
public interface IModApi
{
    // ── Component registration — Path α / Path β (K-L3 / K-L3.1) ──────────
    // Path α: NativeWorld-backed struct storage; unmanaged so the type
    // crosses the P/Invoke boundary without GCHandle pinning.
    void RegisterComponent<T>() where T : unmanaged, IComponent;
    // Path β: per-mod managed-class storage; T must carry [ManagedStorage],
    // absence raises the MissingManagedStorageAttribute semantic (§12).
    void RegisterManagedComponent<T>() where T : class, IComponent;

    // ── System registration ────────────────────────────────────────────────
    void RegisterSystem<T>() where T : class;

    // ── Bus operations (capability-gated; §4.2) ────────────────────────────
    void Publish<T>(T evt) where T : IEvent;
    void Subscribe<T>(Action<T> handler) where T : IEvent;

    // ── Inter-mod contracts (§5.6) ─────────────────────────────────────────
    void PublishContract<T>(T contract) where T : IModContract;
    bool TryGetContract<T>(out T? contract) where T : class, IModContract;

    // ── Introspection and logging ──────────────────────────────────────────
    IReadOnlySet<string> GetKernelCapabilities();
    ModManifest GetOwnManifest();
    void Log(ModLogLevel level, string message);

    // ── Sub-APIs (§4.3) — nullable; mods check for null and degrade ───────
    IModFieldApi? Fields { get; }
    IModComputePipelineApi? ComputePipelines { get; }
}
```

### §4.1 Registration semantics

`RegisterComponent<T>` records the type in `ModRegistry` under the calling mod's id; a type already registered by another mod is an `InvalidOperationException` naming both owners. `RegisterManagedComponent<T>` first verifies `[ManagedStorage]` on `T` (violation throws with the `MissingManagedStorageAttribute` semantic), is idempotent per type, creates the per-mod `ManagedStore<T>`, and records the type in the registry for resolver dispatch (`RestrictedModApi.cs:99-123`). Path β data is runtime-only (K-L3.1 lock) — not persisted; reclaimed with the mod (§9.4 step 3). `RegisterSystem<T>` requires `[SystemAccess]` and `[TickRate]` on the type; registration order is preserved, and `ModRegistry.GetAllSystems` returns core systems first, then mod systems (`ModRegistry.cs:145-153`).

### §4.2 `Publish<T>` / `Subscribe<T>` semantics

- The event routes to the bus named by the event type's `[EventBus("…")]` marker, resolved by `ModBusRouter` against the matching `IGameServices` bus property (`ModBusRouter.cs:28-35`). An event type without the marker, or naming an unknown bus, resolves to no bus — the publish is a **no-op** and the subscribe registers nothing (`RestrictedModApi.cs:163-164,174-175`).
- Both calls run the §3.6 capability gate first (`kernel.publish:<FQN>` / `kernel.subscribe:<FQN>`); a miss throws `CapabilityViolationException`, except under the empty-capability leniency.
- `Subscribe<T>` wraps the handler to capture the calling thread's `SystemExecutionContext` when one is current; the wrapper pushes that context around handler invocation, so the handler observes the same per-system context as the subscribing code (`RestrictedModApi.cs:177-188`).
- Subscriptions are tracked per API instance in `_subscriptions` so `UnsubscribeAll` can release every wrapper on unload (`RestrictedModApi.cs:47,223-228`). Multiple subscriptions to the same `T` from one mod are permitted; each handler runs.
- Delivery semantics (deferred vs immediate dispatch, tier behavior) are owned by [EVENT_BUS.md](./EVENT_BUS.md).

### §4.3 Sub-APIs: `Fields` and `ComputePipelines` — nullability truths

Sub-API contracts, quoted from `src/DualFrontier.Contracts/Modding/` (signatures verbatim):

```csharp
public interface IModFieldApi
{
    IFieldHandle RegisterField<T>(string id, int width, int height) where T : unmanaged;
    IFieldHandle GetField<T>(string id) where T : unmanaged;
    bool IsRegistered(string id);
}

public interface IModComputePipelineApi   // placeholder — no implementation exists
{
    string Name { get; }
}

public interface IFieldHandle
{
    string Id { get; }
    int Width { get; }
    int Height { get; }
    Type ElementType { get; }
}
```

(`IModFieldApi.cs:23-49`, `IModComputePipelineApi.cs`, `IFieldHandle.cs:20-26`.) Returns are the type-erased `IFieldHandle` because the concrete `FieldHandle<T>` lives in `DualFrontier.Core.Interop` (a reverse Contracts→Interop reference would invert the dependency direction); mods downcast at the call site. The concrete handle surface and field failure modes are owned by [FIELDS.md](./FIELDS.md); field operation failures raise `FieldOperationFailedException` (`DualFrontier.Core.Interop`).

**Capability gating at the field boundary** (runtime-only; no `[FieldAccess]`-style load-time attribute exists): `RegisterField` demands the id start with `<modId>.` and the manifest token `mod.<modId>.field.write:<id>`; `GetField` demands `mod.<modId>.field.read:<id>` for own-namespace ids and `mod.<foreignMod>.field.read:<id>` for cross-mod ids; violations raise `CapabilityViolationException` (`RestrictedFieldApi.cs:41-52,63-74`). Per-cell traffic through an acquired handle is not re-checked — gating is at acquisition, mirroring how bus traffic is gated at publish/subscribe rather than per event.

**Nullability (code truth):**

- `Fields` is non-null only when a `FieldRegistry` is supplied to `RestrictedModApi` at construction (`RestrictedModApi.cs:74,83-85`). The production pipeline constructs the API **without one** (six-argument call, `ModIntegrationPipeline.cs:399`), so mods loaded through the pipeline observe `Fields == null` and must degrade gracefully. The field stack (`FieldRegistry` — `src/DualFrontier.Core.Interop/FieldRegistry.cs:22`, `RestrictedFieldApi`, `FieldHandle<T>`) is real and test-exercised.
- `ComputePipelines` is unconditionally `null` — hardwired (`RestrictedModApi.cs:216`); `IModComputePipelineApi` has no implementing type.

> **FENCED (target / planned — not current truth):** wiring a production `FieldRegistry` into the pipeline, and the mod-facing compute surface (SPIR-V registration/dispatch behind `pipeline.register` / `field.dispatch`), are Planned — see [docs/ROADMAP.md](../ROADMAP.md) §V substrate. Until then a correct mod checks both properties for null (the [MODDING.md](./MODDING.md) startup pattern).

### §4.4 Strict v3 — no backward compatibility

The v2 `IModApi` was deleted entirely at the К8.3+К8.4 cutover (2026-05-14); there is no grace period. Mods that registered class-shape components without `RegisterManagedComponent` fail to compile post-cutover — `RegisterComponent<T>` carries `where T : unmanaged, IComponent` — and `ManifestParser` rejects any `manifestVersion` other than `"3"` (§2). The canonical statement lives in the interface header (`IModApi.cs:16-27`). The nullable sub-APIs are forward-compatibility *within* v3, not backward-compatibility with prior versions.

### §4.5 Cast prevention ✓ LOCKED (D-3)

A mod is forbidden from casting `IModApi` to a concrete type. Enforcement is **structural only**: `RestrictedModApi` is `internal sealed` with internal construction (`RestrictedModApi.cs:38,67`), and its assembly (`DualFrontier.Application`) is outside the mod compile surface (§1.4) — the cast cannot compile against the kernel's actual type. No Roslyn analyzer and no runtime cast check exist for this rule; if a real bypass attempt is observed in the wild, a defensive analyzer is the sanctioned v1.x response. (The interface's own doc-comment claim that "the ModLoader detects the attempt and unloads the mod" — `IModApi.cs:14` — describes no shipped mechanism and is superseded by this statement.)

---

## §5 Type sharing and the three contract levels

### §5.1 The problem

Without a shared `AssemblyLoadContext`, two mods that both reference a third assembly load it twice and obtain two distinct `Type` instances: `typeof(FireballCastEvent)` in mod A and mod B are different runtime types, and a subscription registered in one cannot match an event published from the other. Every interesting cross-mod scenario breaks at this boundary.

### §5.2 The shared ALC

One process-wide `AssemblyLoadContext` named `"shared"`, `IsCollectible = false`, owned by the pipeline and reused across every `Apply` (`SharedModLoadContext.cs:21-34`; `ModIntegrationPipeline.cs:84`). `LoadSharedAssembly` indexes each shared assembly by simple name and rejects duplicates (`SharedModLoadContext.cs:50-65`); resolution consults the cache and otherwise defers to the runtime (§1.3).

### §5.3 Loader rules

**Shared mods** (`ModLoader.LoadSharedMod`, `ModLoader.cs:122-176`): manifest validated (§2.3) with `kind=shared` asserted; assembly loaded into the shared ALC; exported types enumerated (an unloadable exported type fails the load); the mod's `capabilities.provided` join the resolver's world at validation time. Architectural compliance — no `IMod` implementation, empty `entryAssembly`/`entryType`/`replaces` — surfaces as Phase F `SharedModWithEntryPoint` errors.

**Regular mods that depend on shared mods** (`ModLoader.LoadRegularMod`, `ModLoader.cs:64-102`): listed shared dependencies are loaded first (topological pass 1, §8.2); a new collectible `ModLoadContext` is created wired to the shared ALC; the entry assembly loads; the `IMod` type resolves via `entryType` or reflection scan; `Initialize` is **not** called by the loader — the pipeline runs it after validation (§8.2 step [4]).

### §5.4 Restrictions on shared mods

- A shared mod exports only types (`record`/`class`/`interface`/`enum`/`struct`); public methods only as members of those types.
- A shared mod references only `DualFrontier.Contracts` and other shared mods — same compile surface as regular mods (§1.4).
- No static constructors touching mutable state (initialization order across shared mods is not guaranteed); no environment/file/network access at type-load time. Detection is best-effort; violations belong to the threat model (§11).
- Naming convention `<base>.protocol` / `<base>.types` (e.g. `dualfrontier.vanilla.core`, `dualfrontier.magic.protocol`) — convention only; no loader check exists at HEAD.

### §5.5 Cycle rules ✓ LOCKED (D-5)

Cycles in the shared-mod dependency graph are forbidden; the loader rejects them with `CyclicDependency` **before any assembly load** (`ModIntegrationPipeline.TopoSortSharedMods`, `ModIntegrationPipeline.cs:270-283,1091-1099`). Cycles between regular mods are rejected the same way (`TopoSortRegularMods`, `:1110-1118`). Kahn's algorithm produces the load order; cycle members are excluded from the sorted set and surfaced one error per affected mod (`TopoSortByPredicate`, `:1211-1303`).

### §5.6 Three contract levels

Mods communicate with the kernel and each other through three distinct levels. Mixing levels (e.g. using a data contract for service dispatch) is a design error caught at review.

| Level | Shape | Where the type lives | Where the implementation lives | Consumer reach |
|---|---|---|---|---|
| **1 — Data** | pure `record : IModContract`, no behavior | shared mod | publishing regular mod | `TryGetContract<T>` returns the instance |
| **2 — Service** | interface `: IModContract` + implementation class | interface in shared mod | implementation in regular mod | `TryGetContract<T>` returns the implementation |
| **3 — Protocol** | `IEvent` type with publish/subscribe | shared mod | publishers/subscribers in regular mods | bus dispatch via `Publish`/`Subscribe` |

Level 1 examples: weapon/recipe/biome definitions published at registration time. Level 2: pluggable single-provider behaviors (`ICookingService`) — consumers fetch with `TryGetContract` and degrade gracefully on `false`. Level 3: cross-mod gameplay protocols (a magic mod publishes spell events; defensive mods subscribe). The store surface is `IModContractStore` (`IModContractStore.cs:19-33`): `Publish<T>(modId, contract)`, `TryGet<T>`, and `RevokeAll(modId)` — revocation runs in the unload chain (§9.4 step 2), after which `TryGetContract` returns `false` for the departed provider's contracts.

### §5.7 Contract types must live in shared mods ✓ LOCKED (D-4)

A type used at any contract level **must** live in a shared mod — a `WeaponDef` defined in a regular mod's assembly is unreachable from another regular mod (different ALC, different `Type`). The loader actively scans every regular-mod assembly for exported `IModContract`/`IEvent` implementers and rejects the mod with `ContractTypeInRegularMod`, naming the offending type (Phase E, `ContractValidator.cs:329`). The one reflection pass per load is negligible against the architectural signal: contracts in regular mods break interoperability *silently*, and silent breakage is what this architecture exists to prevent.

---

## §6 Bridge replacement ✓ LOCKED

Kernel bridge systems exist as `[BridgeImplementation(Phase = N)]` stubs that keep the dependency graph valid for downstream phases referencing the system identity. When a mod ships the real implementation, the bridge steps aside — explicitly.

### §6.1 Mechanism: explicit `replaces`

A mod listing a fully-qualified type name in `replaces` declares that **its** registered system supersedes the named kernel system. At apply time (`ModIntegrationPipeline.cs:432-457,1063-1072`):

1. Every `replaces` entry across the batch is collected into the `replacedFqns` set (Phase H has already vetted each entry — see below).
2. During graph construction, every **core-origin** system whose FQN is in the set is **skipped**: the bridge stays compiled but is never registered.
3. The mod's replacement system, registered through `RegisterSystem` during `Initialize`, enters the graph as a mod-origin system like any other.

### §6.2 Conflict resolution and protection

Phase H (`ContractValidator.cs:566`) rejects, per §12: two mods in one batch replacing the same FQN (`BridgeReplacementConflict` — the user must disable one; there is no automatic priority, because silent precedence is a debugging nightmare); a `replaces` entry naming a system not annotated `[BridgeImplementation(Replaceable = true)]` (`ProtectedSystemReplacement` — the kernel's escape hatch for systems that must remain authoritative); an FQN found in no loaded assembly (`UnknownSystemReplacement`).

### §6.3 Rationale and reversal

Implicit replacement was rejected in Phase 0: a user investigating "why is combat acting strangely" must be able to discover, from manifests alone, which mod is responsible; explicit `replaces` extends the write-write-conflict philosophy ("conflicts are surfaced, never resolved silently") to system identity. Unloading a replacing mod reverts the skip: the next graph rebuild re-registers the kernel bridge (the skip-set is recomputed per apply from the *loaded* batch).

---

## §7 Versioning — three SemVer axes ✓ LOCKED

| Axis | Field | Question it answers | Constraint kinds |
|---|---|---|---|
| 1 — Kernel API | `apiVersion` (fallback `requiresContractsVersion`) | "does this mod fit this engine build?" | Exact or Caret |
| 2 — Mod self | `version` | identity, reload lineage, save records | Exact value (not a constraint) |
| 3 — Inter-mod | `dependencies[i].version` | "does my dependency fit?" | Exact or Caret |

**Axis 1.** The version of `DualFrontier.Contracts`, existing as `ContractsVersion.Current` — currently `1.0.0` (`ContractsVersion.cs:17`). Bumped manually: Major for breaking changes to `IModApi`/manifest schema/attribute set/public Contracts signatures; Minor for additive changes; Patch for fixes that do not touch the public surface. Phase A checks `ApiVersion` via `IsSatisfiedBy` when present, else the legacy `requiresContractsVersion` path via `ContractsVersion.IsCompatible` (`ContractValidator.cs:124-178`).

**Axis 2.** Used for hot-reload lineage (downgrade warns, equal re-applies, higher proceeds), user-visible identity in the menu, and save records `(modId, modVersion)` (§9.8).

**Axis 3.** Verified by Phase G for every dependency edge (`ContractValidator.cs:482-496`); presence (as opposed to version fit) is checked earlier by the pipeline with optional-dependency downgrade to warning (`ModIntegrationPipeline.cs:1138-1173`).

**Constraint syntax — caret subset.** `"1.2.3"` exact; `"^1.2.3"` caret — `>= 1.2.3 < 2.0.0` (major pinned); `"~1.2.3"` tilde **rejected** with a `FormatException` directing the author to caret (`VersionConstraint.cs:56-60`). Ranges and OR-syntax are not supported; a mod needing a non-caret constraint escalates per the stop rule. `VersionConstraint` carries `{Kind: Exact|Caret, floor}` with `IsSatisfiedBy` evaluating per kind (`VersionConstraint.cs:7-38,100-110`).

**Resolution algorithm.** Build the dependency graph → topologically sort (cycle ⇒ `CyclicDependency`) → in topological order verify axis 1, then each axis-3 constraint; failures accumulate and the affected mods cascade-fail. There is no version-resolution backtracking: the loader takes manifests at face value — an unsatisfied constraint is a user-resolvable error, not a solver problem. In the shipped pipeline the whole batch fails together on any error (§8.3), so "cascade" is currently subsumed by batch atomicity.

---

## §8 The integration pipeline and the Apply transaction

### §8.1 Components

| Component | Role | Canonical source |
|---|---|---|
| `ModIntegrationPipeline` | Orchestrator: classify → load → validate → initialize → rebuild → swap; unload chains; fault-set drain | `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` |
| `ModLoader` | Manifest read, ALC creation, `IMod` resolution, per-mod unload of instance + context | `ModLoader.cs` |
| `ContractValidator` | Phases A–H batch validation (§2.3) → `ValidationReport` | `ContractValidator.cs` |
| `ModRegistry` | Registry of mod components/systems; core-first enumeration; per-mod removal incl. Path β store reclamation | `ModRegistry.cs` |
| `RestrictedModApi` | The `IModApi` v3 implementation handed to `IMod.Initialize` (§4) | `RestrictedModApi.cs` |
| `IModContractStore` / `ModContractStore` | Inter-mod contracts, revocable per mod | `IModContractStore.cs`, `ModContractStore.cs` |
| `ModFaultHandler` | Thread-safe faulted-mod accumulator drained by `Apply` (§9.5) | `ModFaultHandler.cs` |
| `ModMenuController` | Menu editing session: `BeginEditing`/`Toggle`/`Commit`/`Cancel` over pipeline snapshots (§9.2) | `ModMenuController.cs` |
| `KernelCapabilityRegistry` | Kernel-provided capability set (§3.4) | `KernelCapabilityRegistry.cs` |

Construction and ownership (K6.1): `GameBootstrap` constructs the `ModFaultHandler` before the scheduler so the scheduler ctor takes it as an immutable `IModFaultSink`, and installs it into the loader (`GameBootstrap.cs:186-187`); the pipeline receives all collaborators by injection (`ModIntegrationPipeline.cs:138-154`) and owns the singleton shared ALC.

### §8.2 `Apply` — the stages as coded

`public PipelineResult Apply(IReadOnlyList<string> modPaths)` (`ModIntegrationPipeline.cs:209`). Executed from the mod menu; the simulation must be paused — a running pipeline throws `InvalidOperationException("Pause the scheduler before applying mods")` (`:212-214`, §9.7).

| Stage | What happens | Anchor |
|---|---|---|
| [-1] Fault drain | Every mod queued by `ModFaultHandler` since the last apply is unloaded through the full §9.4 chain, then cleared; warnings fold into the result | `:222-228` |
| [0] Classify | Pre-injected mods taken as regular; on-disk manifests parsed and split by `ModKind`; manifest-read failures become errors (recorded under `MissingDependency` — see note below) | `:233-268` |
| [0.5] Shared cycle check | D-5 topological sort of shared manifests; cycle members error out before any assembly load | `:270-283` |
| [0.6] Regular sort + presence | Regular-mod topological sort (`CyclicDependency` for cycles); dependency-presence check (required missing ⇒ `MissingDependency` error; optional missing ⇒ warning) | `:291-318` |
| [1] Pass 1 — shared | Shared mods load into the shared ALC in topological order | `:320-339` |
| [2] Pass 2 — regular | Regular mods load, each `ModLoadContext` wired to the shared ALC | `:341-362` |
| [3] Validate | `ContractValidator.Validate(loaded, coreSystems, kernelCapabilities, sharedMods)` — Phases A–H; on any error (or any earlier load error) the batch **rolls back** and returns failure | `:364-392` |
| [4] Initialize | Per mod: construct `RestrictedModApi` (six args — no field registry, §4.3) and call `IMod.Initialize(api)`; the API instance is retained on the mod for the unload chain; any throw ⇒ full rollback | `:394-430` |
| [5-7] Local graph build | A **local** `DependencyGraph` receives every registered system, skipping core systems whose FQN is in the replaces set (§6.1); `Build()` proves the graph; a throw ⇒ full rollback, scheduler untouched | `:432-480` |
| [8] Commit | `_activeMods`/`_activeShared` extended; `SystemMetadataBuilder.Build(_registry)` snapshots origin/modId per system; `_scheduler.Rebuild(localGraph.GetPhases(), newMetadata)` swaps phases, metadata, and context cache together | `:482-502`; `ParallelSystemScheduler.cs:191-222` |

Error-kind note (code truth): stage [0]/[1]/[2] load failures and stage [4] `Initialize` throws are reported under `ValidationErrorKind.MissingDependency` with a descriptive message (`:263-266,334-337,357-360,408-411`) — the enum has no dedicated "load failed"/"init threw" kinds; the message, not the kind, carries the diagnosis.

### §8.3 Graph-rebuild atomicity — the commit guarantee

`DependencyGraph` construction and `Build()` run on a **local variable**; the scheduler is replaced only after `Build()` succeeds. On any error the old scheduler stays active — phases, metadata, and per-system execution contexts swap in one assignment sequence inside `Rebuild` (`ParallelSystemScheduler.cs:200-221`). A partially built graph never exists from the scheduler's point of view. This is the engine's model prepare/commit transaction — the vocabulary mapping (prepare/validate/quiesce/commit) is per ENGINE_LIFECYCLE_AND_TRANSACTIONS.md (AUTHORED draft), which uses this exact flow as its conformant example.

Rollback inventory on the failure paths: `RollbackLoaded` physically unloads every regular mod that reached memory (`:984-992`, swallowing rollback-time errors); `_registry.ResetModSystems()` clears mod systems and component ownership (`ModRegistry.cs:160-164`); `_contractStore.RevokeAll(modId)` per mod. Registry/contract rollback runs on the stage [4]/[5-7] paths — after stage [3] failures nothing has registered yet.

### §8.4 `PipelineResult` — partial-success semantics

```csharp
public sealed record PipelineResult(
    bool Success,                                // true only when the scheduler was rebuilt
    IReadOnlyList<ValidationError> Errors,       // empty on success
    IReadOnlyList<string> LoadedModIds,          // active after the apply
    IReadOnlyList<string> FailedModIds)          // ids (or paths) that failed
{
    public IReadOnlyList<ValidationWarning> Warnings { get; init; }  // both paths
}
```

(`ModIntegrationPipeline.cs:21-36`.) The nuances that matter:

- **The regular-mod batch is all-or-nothing.** Errors accumulate across stages; if any error fires, the entire batch rolls back and `Success = false` — no mod is silently skipped (`:299-300,380-392`). `FailedModIds` distinguishes *which* inputs caused it.
- **Shared mods are the partial-success exception.** Once loaded into the non-collectible shared ALC they persist for the session even when the batch fails (`:56-57` design comment; nothing in the rollback paths unloads them — only `_activeShared` bookkeeping is withheld until commit, `:490`). A retry of the same batch therefore finds the shared assemblies already cached; re-loading the same shared simple name is rejected (`SharedModLoadContext.cs:59-61`, surfaced through the stage-[1] error path).
- **Warnings ride both outcomes** — optional-dependency advisories, fault-drain unload warnings, and validator warnings are merged and attached to success and failure results alike (`:377-378` and every return).

### §8.5 `UnloadAll`

`public IReadOnlyList<ValidationWarning> UnloadAll()` (`ModIntegrationPipeline.cs:780-824`) delegates to `UnloadMod` per active mod (snapshot-by-id through a non-inlined helper for GC-cleanliness) and, when no mod was active, still rebuilds the kernel-only graph with fresh metadata. Shared mods are never unloaded. **Code truth: no production call site invokes `UnloadAll`** — the menu path unloads removed mods individually and applies additions (`ModMenuController.Commit`, `ModMenuController.cs:236` region); `UnloadAll` is exercised by tests. It is retained as the bulk-teardown surface pending the shutdown-transaction design (ENGINE_LIFECYCLE_AND_TRANSACTIONS.md, AUTHORED draft — world shutdown row).

---

## §9 Lifecycle

### §9.1 The commit/reclaim split — one law instead of a contradiction

The predecessor stated both "transitions between states are atomic; failure mid-transition rolls back" (old §9) and "there is no atomic-unload guarantee; `Unload` is conceptually irreversible" (old §9.5.1) — the session's C7 finding. Both sentences were correct **about different stages**, and this document adopts the split explicitly, per ENGINE_LIFECYCLE_AND_TRANSACTIONS.md (AUTHORED draft) §1:

- **Atomic applies to the desired-state commit.** Apply commits by a single scheduler swap after a locally proven graph (§8.3); unload commits by removing the mod from the active set and swapping in the rebuilt graph. Before the commit point, failure rolls the candidate back and the old state stands (§8.3 rollback inventory). After it, the new desired state is published — never a mixture.
- **Best-effort applies to reclamation of the old state.** The unload chain's cleanup steps (unsubscribe, revoke, remove, native teardown, ALC unload, GC pump) are sequential, best-effort, and monotonic: a step's failure logs a warning and the chain continues; nothing un-commits (§9.4). Reclamation can end in "leaked — restart advised" while the desired state honestly reads "disabled".

**States as coded — say it plainly:** there is no mod-state enum and no `Degraded` state anywhere on disk. Current state is carried by booleans and set membership: the pipeline's `_activeMods`/`_activeShared` lists, the `_isRunning` flag (`ModIntegrationPipeline.cs:113`), the loader's `_loaded` map, the menu's per-row flags `IsCurrentlyActive`/`IsPendingActive`/`CanToggle` (`EditableModInfo.cs:31-36`), and the fault handler's queued set. The predecessor's six-state diagram (Disabled → Pending → Loaded → Active → Stopping → Disabled) survives only as a *reading* of those booleans along the happy path; it is not code, and the fault path (§9.5) and the leaked-reclamation terminal were never representable in it. Separated desired-state/reclamation-state variables are the draft's proposal, not current truth.

> **FENCED (target / planned — not current truth):** ENGINE_LIFECYCLE_AND_TRANSACTIONS.md (AUTHORED draft) §3 proposes named state machines (`DesiredState` incl. `LogicallyDisabled(faulted)`; `ReclamationState` incl. `Reclaiming/Reclaimed/ReclaimFailed(leaked)`) and a recover stage (bounded pump retry, Degraded reason). None of that exists in code; adopting it is that draft's ratification question, not this document's.

### §9.2 Hot reload through the menu ✓ LOCKED

Hot reload is supported **only** via the mod menu with the simulation paused. Flow (menu implementation: `ModMenuController`, `ModMenuController.cs:102-320`):

1. The menu opens an editing session: `BeginEditing` snapshots the active set (`GetActiveMods`, `ModIntegrationPipeline.cs:177-183`) and pauses via `Pause()` (run flag false — `:191`).
2. The user toggles mods (`Toggle`/`CanToggle`, honoring §9.7 restrictions); `GetEditableState` renders the §9.1 boolean flags.
3. `Commit` unloads every removed mod via `UnloadMod`, then applies added paths via `Apply` (skipped entirely when nothing was added), then `Resume()` (`ModMenuController.cs:236` region). `Cancel` discards the pending set.
4. The simulation continues from the current world state against the new graph.

Reloading a mod (same id, possibly different version) is unload-then-load in one session. Version lineage warnings per §7 axis 2.

### §9.3 No live-tick reload ✓ LOCKED

Reloading during a tick is forbidden. `Apply`, `UnloadMod`, and `UnloadAll` all throw `InvalidOperationException("Pause the scheduler before …")` while the run flag is up (`ModIntegrationPipeline.cs:212-214,564-566,782-784`). The flag lives on the pipeline rather than the scheduler — a recorded M7.1 interpretation (`:93-113`); its default `false` ("paused") is load-bearing for every construct-then-Apply flow.

### §9.4 THE unload chain — single, code-verified

One chain exists: `ModIntegrationPipeline.UnloadMod(modId)` (`ModIntegrationPipeline.cs:561-592`, steps in `:612-759`). It is idempotent (unknown id returns an empty warning list) and rejects calls while running (§9.3). Steps, in the coded order and with the coded numbering:

| Step | Action | Anchor |
|---|---|---|
| 1 | `RestrictedModApi.UnsubscribeAll()` — every tracked bus subscription removed | `:630-633`; `RestrictedModApi.cs:223-228` |
| 2 | `IModContractStore.RevokeAll(modId)` — inter-mod contracts revoked | `:637-640` |
| 3 | `ModRegistry.RemoveMod(modId)` — the mod's systems and component ownerships removed; **includes Path β reclamation**: `ClearManagedStores()` empties and drops every per-mod `ManagedStore<T>` and the resolver entry before system removal (the `_restrictedModApis` map this branch drains is populated by `RegisterRestrictedModApi`, which no production or test code invokes at HEAD — Path β data is in practice reclaimed with the ALC at steps 6-7; wiring the registration is an open item) | `:646-649`; `ModRegistry.cs:173-200` (`:179-183`) |
| 3.5 | Native scheduler-state teardown: `df_scheduler_unload_mod_native_state(hash)` clears per-tier subscriber registries, capability registrations, wake-registry subscriptions, shared-memory registrations inside one native critical section, returning `ModUnloadResult` with per-tier metrics; native failure messages surface as step-3.5 warnings; then the per-mod `ModSubScheduler` is torn down (`RemoveSubScheduler`) | `:665-691`; `src/DualFrontier.Core.Interop/ModUnloadInterop.cs:73,92,112-116` |
| 3.6 | V (Vulkan) resource cleanup — `VResourceCleanup.UnloadModResources(modId)`, today a managed placeholder returning vacuous success (no pipeline-managed mod resources are registered); failures would surface as step-3.6 warnings | `:705-717`; `src/DualFrontier.Application/Bridge/VResourceCleanup.cs:27,52-62` |
| 4 (fused 4+5) | Graph rebuild **and** scheduler swap, coupled in one step: a local `DependencyGraph` over the post-removal registry, fresh `SystemMetadata` snapshot, `_scheduler.Rebuild(...)` — coupled so the scheduler never briefly references systems whose types are being collected in step 6 | `:719-737` |
| 6 | `ModLoader.UnloadMod(modId)`: `mod.Instance.Unload()` inside a swallowed try/catch (**no timeout, no abort** — the M0-era discipline), then `ModLoadContext.Unload()` (asynchronous, collaborative), then removal from the loader map | `:744-747`; `ModLoader.cs:212-231` |
| — | `WeakReference` to the mod's ALC captured in a non-inlined frame; the mod is removed from `_activeMods` — **this is the desired-state commit**: it happens regardless of any step's outcome | `:749-757` |
| 7 | Reclamation verification: spin on `WeakReference.IsAlive`, each iteration running the mandatory GC pump bracket `GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect()` (the double collect restores monotonic progress because `WaitForPendingFinalizers` can resurrect finalizable graph nodes); cadence 100 × 100 ms = 10 s (`Step7TimeoutMs`, `:115-120`). On timeout a `ModUnloadTimeout` warning is appended — "the mod has been removed from the active set; restart the game to fully reclaim memory" — i.e. **leaked ⇒ advise restart** | `:585-589,912-935` |

**Failure semantics (per-step best-effort).** Steps 1–3.6 and 4 and 6 are each wrapped by `TryUnloadStep`: on exception the loader records a non-blocking `ValidationWarning` carrying `(modId, stepNumber)` and the message, and continues to the next step (`:853-872`). The chain is structured so each step is a no-op if its predecessor failed (removing systems from a mod that registered none is harmless). Step 7 is timeout-based, not exception-based, and runs even after a step-6 failure. A native step-3.5 rejection (e.g. the К-L18 precondition, §9.6) surfaces through the same warning pipeline and the chain **continues** — the shipped behavior matches the predecessor's §9.5.1 wording, and tightening it to abort-on-quiesce-failure is the lifecycle draft's proposal, fenced above. There is no atomic-unload guarantee: subscriptions removed in step 1 cannot be re-attached without re-running `Subscribe`.

Contract-adjacent GC engineering (it is what makes step 7 pass): steps 1–6, the WR capture, and the active-set removal all run in `NoInlining` helpers so no stack frame or lambda display class roots the `LoadedMod`/`ModLoadContext` during the spin; the spin helper takes only `(modId, WR, warnings)` (`:611-614,884-916`; same pattern in `UnloadAll`'s id snapshot, `:836-843`).

**Resolved (was N-9 — the three-document step-order conflict):** the coded order above — *unsubscribe first*, then contracts, then registry removal — is the truth; the retired MOD_PIPELINE listing (RemoveSystems first, `IMod.Unload` as its own step 4, ClearManagedStores as its own step 5, a trailing `ModDisabledEvent` publish) does not match the code: `IMod.Unload` lives *inside* step 6 via `ModLoader.UnloadMod`, Path β reclamation lives *inside* step 3 via `ModRegistry.RemoveMod`, spec-steps 4 and 5 are fused into one coded step, and **no `ModDisabledEvent` type exists in the codebase** (repo-wide grep at HEAD).

### §9.5 THE fault path — single, code-verified timing

What actually happens when a mod fault is reported, in full (this is the entire mechanism):

1. **Entry point.** `ModLoader.HandleModFault(modId, ModIsolationException)` — the surviving public fault surface post-К8.3+К8.4 (`ModLoader.cs:303-309`). It routes to the installed handler; with no handler installed it is a no-op.
2. **Recording.** `ModFaultHandler.ReportFault(modId, message)` adds the id to a deduplicating `HashSet` under a lock — nothing else (`ModFaultHandler.cs:65-72`). No unsubscribe, no scheduler eviction, no graph change, no log file, no UI event happens at fault time. Rationale (retained design comment, `:29-37`): a fault arrives mid-tick on a worker thread; rebuilding the graph synchronously would race with other workers.
3. **Deferred unload.** At the next menu-open `Apply`, stage [-1] drains the faulted set: each queued mod goes through the full §9.4 chain, then `ClearFault` (`ModIntegrationPipeline.cs:222-228`). Reentrance is harmless (the set deduplicates; the handler never calls back into the pipeline at fault time).

**Detection is the honest gap.** No production code path invokes `HandleModFault` or `ReportFault` today: `ParallelSystemScheduler.ExecutePhase` wraps `system.Update` in try/**finally** only (context pop — `ParallelSystemScheduler.cs:149-164`), the game loop has no mod-fault catch, and the deleted runtime isolation guard was the previous caller (`NullModFaultSink` doc: "post-К8.3+К8.4 no Core call site invokes the sink", `IModFaultSink.cs`). Consequently a mod system that throws during `Update` today propagates exactly like a core system's exception (through `Parallel.ForEach` as `AggregateException`) — the designed core-crash/mod-unload split (§10.3) is plumbed (origin metadata, sink, drain) but not triggered anywhere. The plumbing is real and test-exercised via `ReportFault` directly.

**Resolved (was N-8 — the three-way timing conflict):** the predecessors specified this event three ways — MOD_OS §10.1 *queued-to-next-menu-open*; MOD_PIPELINE *chain-now-with-deferred-rebuild plus "systems marked Disabled in the scheduler"*; ISOLATION *immediate six-step sequence with an abort-on-timeout `IMod.Unload`*. The code implements the first: queue at fault time, full chain at next `Apply`. The other two are retired — no "Disabled" marking exists in the scheduler, no immediate sequence exists, no `IMod.Unload` timeout/abort exists (the call is a swallowed try/catch with no time bound — `ModLoader.cs:219-227`), no `logs/mods/<mod-id>.log` writer exists, and no `ModDisabledEvent` exists.

> **FENCED (target / planned — not current truth):** the missing detection stage — a per-system catch in `ExecutePhase` dispatching on `SystemOrigin` (core → rethrow, mod → quarantine skip-set at the tick boundary + `ReportFault`) — and the fault-time quarantine commit are specified in ENGINE_LIFECYCLE_AND_TRANSACTIONS.md (AUTHORED draft) §2.3. User-facing surfacing (banner, per-mod fault log, "disabled due to error" persistence across restarts) ships with that work; until then the §11 threat-model row for runtime misbehavior reads "reported faults are honored; unreported faults propagate".

### §9.6 К-L18 quiescence

Per К-L18 (KERNEL_ARCHITECTURE.md Part 0): mod load/unload operations require *simulation paused + pipeline slots quiescent* (all fences completed; no compute dispatches in flight). Where that stands in code:

- **Managed helper layer (helpers-only per S-LOCK-12):** `SimulationStateController` provides `PauseAsync()` → `WaitForQuiescenceAsync(timeout)` → operation → `ResumeAsync()`; quiescence timeout is typed as `PipelineQuiescenceTimeoutException` (`src/DualFrontier.Application/Loop/SimulationStateController.cs:35,71,110,142`). **Zero production callers today** — the menu path enforces pause through the pipeline's `_isRunning` flag only (§9.3); the controller is the sanctioned hookup point when the settings/menu UI work lands.
- **Native precondition:** the step-3.5 primitive checks sim-paused + pipeline quiescence natively (К10.2 stub + К10.3 v2 Item 41, `df_pipeline_is_quiescent`) and returns failure with diagnostics instead of partial teardown; the managed wrapper converts that to step-3.5 warnings (`ModUnloadInterop.UnloadModNativeState`, `ModUnloadInterop.cs:112-116`; §9.4 failure semantics).
- **Vacuity note:** while the native bus carries no production subscribers (dispatch default managed — see [EVENT_BUS.md](./EVENT_BUS.md) current/target wiring), the native teardown is vacuously successful; it becomes load-bearing at the native-bus authority switch (per EXECUTION_AUTHORITY_MATRIX.md §3, AUTHORED draft).

### §9.7 Hot-reload-disabled mods ✓ LOCKED (D-7)

A mod with `"hotReload": false` cannot be reloaded or disabled mid-session: the menu grays the toggle (`EditableModInfo.CanToggle`) and `Toggle` rejects defensively (`ModMenuController.cs:152-201`); the user restarts instead.

> **✓ LOCKED (D-7).** Hybrid by build flavor: vanilla mods declare `hotReload: true` in source; the shipping build pipeline rewrites this to `false` in release manifests — development gets free hot-reload testing of vanilla mechanics, shipped builds get session stability.
> **FENCED (target / planned — not current truth):** the release-manifest rewrite step exists in no build script at HEAD; the vanilla manifests on disk carry their source-time values. The lock binds the shipping pipeline when one exists.

### §9.8 Save-game implications ✓ LOCKED (D-6)

A save records `(modId, modVersion)` for every active mod. On load: a missing recorded mod ⇒ warn and load without it; a higher-major recorded mod ⇒ warn (may misbehave); lower-or-equal ⇒ proceed.

> **✓ LOCKED (D-6).** Default policy for missing mods: **strip the missing mod's components, keep the entities**, with a clear warning naming each missing mod; a user-toggleable "strict mod compatibility" setting refuses the load instead. Workshop reality is that mods get abandoned; strict-by-default would block too many real loads.
> **FENCED (target / planned — not current truth):** no persistence implementation consumes this policy yet — the fine-grained handling belongs to PERSISTENCE_SNAPSHOT_CONTRACT.md (AUTHORED draft) and the save milestone; Path β components are runtime-only by K-L3.1 lock regardless (§4.1).

---

## §10 Enforcement and the isolation model

A silent isolation violation is worse than a crash: corrupted state surfaces an hour into play as an inexplicable bug. Dual Frontier declares system isolation at **compile time** and enforces it structurally.

### §10.1 The three mechanisms

1. **Compile-time declarations.** Every `SystemBase` subclass carries `[SystemAccess(reads, writes, bus)]` — the single source of truth for what the system reads, writes, and publishes. Registration without it (or without `[TickRate]`) is rejected (`ModRegistry.RegisterSystem`; the scheduler independently refuses to build a context for an undeclared system — `ParallelSystemScheduler.cs:233-238`).
2. **DependencyGraph edge-building.** The scheduler consumes `[SystemAccess]` to compute writer-vs-reader edges, bus contention, and phase ordering: systems with no edge run on parallel scheduler threads; conflicting systems run sequentially. Detailed model in [THREADING.md](./THREADING.md).
3. **Roslyn analyzers — shipped and enforcing.** 17 rules carry real detection logic and are enforced at shipped severities: 11 Error + 5 Warning are build-breaking under `TreatWarningsAsErrors=true`; one (DFL025_B) is IDE-only — per [ANALYZER_RULES.md](./ANALYZER_RULES.md) §4 (ground truth verified on disk; `tools/DualFrontier.Analyzers/Rules/{Architecture,Discipline,NativeBoundary}`). **This supersedes the retired ISOLATION.md statement that the analyzer stubs "emit zero diagnostics"** — that was true pre-A'.9.1 Phase β and is stale.

> **FENCED (target / planned — not current truth):** the *specific* call-site completeness rule ISOLATION promised — flag any `NativeWorld.AcquireSpan<T>()`/`BeginBatch<T>()` whose `T` is undeclared in the enclosing `[SystemAccess]` — is **not** among the 17 (the shipped set encodes К-L invariants; the access-completeness family is registered in ANALYZER_RULES.md §5 as an unassigned-ID DEFERRED candidate scoped to the К-L20 LOCK cascade, alongside the deferred DFK009/012/015/018/020 groups, and [docs/ROADMAP.md](../ROADMAP.md) §Analyzer track). Until it lands, undeclared component access is caught by manual review and scheduler integration tests, not by the build.

The runtime guard methods that previously threw `IsolationViolationException` from `SystemExecutionContext.GetComponent/SetComponent` were **deleted at the К8.3+К8.4 cutover (2026-05-14)**; systems reach component storage exclusively through `NativeWorld` span/batch APIs (Path α — see [ECS.md](./ECS.md)) or `SystemBase.ManagedStore<T>()` (Path β). The compile-time + structural model replaced the runtime check; the enforcement-gap window until the call-site analyzer lands is stated above, not hidden.

### §10.2 SystemExecutionContext

Still active per tick as the *context carrier* (not a guard). It holds: the system name and declared buses (diagnostics), `SystemOrigin` (`Core`/`Mod`) and `ModId` for mod-origin systems, the `IModFaultSink`, the `NativeWorld` handle, the `IGameServices` aggregator, and the Path β `IManagedStorageResolver` (`src/DualFrontier.Core/ECS/SystemExecutionContext.cs:33-73`). The scheduler pushes it before each `Update` and pops in `finally` (`ParallelSystemScheduler.cs:154-163`); it lives in a `ThreadLocal` slot per scheduler thread (`SystemExecutionContext.cs:35`); systems reach it through `SystemBase.NativeWorld`/`Services`/`ManagedStore<T>()`. Out-of-context calls fail loudly with `InvalidOperationException`. `async`/`await` inside system code is forbidden per [THREADING.md](./THREADING.md) — suspension would resume on a thread whose slot is null. Origin/modId flow from the `SystemMetadata` snapshot built at every apply/unload commit (`SystemMetadataBuilder`; `ParallelSystemScheduler.cs:240-264`).

### §10.3 Two response modes: core crash vs mod soft-unload

The designed split, keyed on `SystemOrigin` stamped at registration (`ModRegistry.cs:137`; enum in `src/DualFrontier.Core/ECS/SystemOrigin.cs`):

| Origin | Designed behavior | Reason |
|---|---|---|
| `Core` (engine systems) | Exception propagates → **process crashes** | A developer bug; must be fixed, not survived |
| `Mod` (loaded via ModLoader) | Fault reported → **mod queued and unloaded at next menu open**, game continues | User content must not take the session down |

Current wiring honesty (one line, per §9.5): the mod row's *reporting* step has no production trigger — an uncaught mod-system exception today propagates like a core one; the split becomes operational with the fenced detection stage. The trigger definition is already generalized post-cutover from the deleted guard's `IsolationViolationException` to "any unhandled exception originating in mod code", carried as `ModIsolationException` through `HandleModFault` (`ModIsolationException.cs`, `ModLoader.cs:303-309`).

### §10.4 System addition checklist (engine and mod authors)

Before committing a new system: inherits `SystemBase`; carries `[SystemAccess(reads, writes, bus)]` and an explicit `[TickRate]`; every component touched in `Update` is listed in reads/writes (build-time enforcement pending the fenced call-site rule — review and integration tests until then); `bus` matches actual publications; subscriptions in `OnInitialize`, released in `OnDispose`; no `async`/`await`/`Task`; no direct references to other systems (buses only); component access via `NativeWorld.AcquireSpan<T>()`/`BeginBatch<T>()` (Path α) or `SystemBase.ManagedStore<T>()` (Path β) — never a managed `World` (none exists in production post-cutover); integration test under `ParallelSystemScheduler`. Unsatisfied items reject the PR.

---

## §11 Threat model

The mod system is not a sandbox. A mod runs in-process with full .NET access. Isolation is **structural and architectural**, not security-grade. The threat model documents what the architecture catches and what it does not.

### §11.1 Architectural threats: caught

| Threat | Mechanism |
|---|---|
| Mod reaches for component storage outside its `[SystemAccess]` declaration | Structural: no managed component-access surface exists to misuse post-cutover — storage is reached through `NativeWorld`/`ManagedStore<T>()`, and declarations drive scheduler edges (§10.1). Build-time call-site detection is the fenced analyzer rule. |
| Mod system misbehaves at runtime and is **reported** | `ModLoader.HandleModFault` → `ModFaultHandler.ReportFault` → queued → unloaded via the §9.4 chain at the next menu open; the core does not crash (§9.5; detection gap stated there). |
| Mod calls `GetSystem<T>()` | Surface removed at К8.3+К8.4 — no `GetSystem` exists on any mod-reachable type. |
| Mod casts `IModApi` to `RestrictedModApi` | Structural unreachability (D-3, §4.5): `internal sealed`, internal construction, assembly outside the mod compile surface. |
| Mod registers a system conflicting with another mod's writes | `ContractValidator` Phase B `WriteWriteConflict`, naming both mods and the component. |
| Two mods replace the same system | Phase H `BridgeReplacementConflict` (§6.2). |
| Mod requires a capability nobody provides | Phase C `MissingCapability` (§3.5). |
| Mod publishes/subscribes without the declared capability | `CapabilityViolationException` from `EnforceCapability` (empty-capability manifests short-circuit with a warning — §3.6). |
| Mod defines contract/event types in a regular assembly | Phase E `ContractTypeInRegularMod` (D-4, §5.7). |

### §11.2 Architectural threats: not caught (explicitly out of scope)

- **`Process.Kill(0)` / `Environment.Exit(1)`.** The .NET runtime gives mods full process access; no AppDomain or process isolation (would break the in-process performance assumptions).
- **Network sockets, arbitrary file reads, shell commands.** Same reason.
- **Unbounded memory or CPU consumption.** Performance budgets are advisory, not enforced.
- **State mutation through legitimately acquired spans.** A mod holding a `kernel.write:` capability writes whatever it wants into those cells; correctness of *values* is review territory, not runtime enforcement.
- **Reflection over loaded assemblies.** A determined mod can locate kernel types in the Default ALC at runtime (§1.4 honesty) — the cost of in-process execution.

### §11.3 The contract: best-effort structural isolation

Dual Frontier's mod system promises: a well-behaved mod cannot *accidentally* crash the engine, corrupt state, or break other mods; a misbehaving mod can be detected, named, and unloaded with high probability; a malicious mod can break the game, and the user accepts that risk by installing it. This is the contract operating systems offer processes, scaled down: ring 3 is enforced; ring 0 is reachable through deliberate effort.

### §11.4 Required tests

The realized suites (`tests/DualFrontier.Modding.Tests/`): capability registry emission + violation path (`Capability/`); bridge-replacement Phase H scenarios (`Validator/PhaseHBridgeReplacementTests`); cross-ALC type identity (`Sharing/CrossAlcTypeIdentityTests`); WeakReference unload — closed at M7.3, asserting `IsAlive == false` within timeout on real-mod fixtures with the production GC-pump pattern mirrored (`Pipeline/M73Step7Tests`, `M73Phase2DebtTests`); regular-mod cycle rejection (`Pipeline/RegularModTopologicalSortTests`); inter-mod version constraints (`Validator/PhaseGInterModVersionTests`); pipeline atomicity (`Pipeline/ModIntegrationPipelineTests` — build failure leaves the old scheduler intact; unload removes mod systems); pause/resume and unload-chain step failures (`Pipeline/M71PauseResumeTests`, `M72UnloadChainTests`). The Phase-2-era runtime isolation-guard suite was retired with the guard; isolation is exercised structurally through the loader/ALC suites. Standing stop conditions (Phase 0 re-entry, unchanged): capability cross-check cost > 5 s per mod load; WeakReference unload tests flaky at any rate above 0%; capability enforcement bypassed using documented .NET features.

---

## §12 Error kinds — the `ValidationErrorKind` registry

`ValidationErrorKind` (`src/DualFrontier.Application/Modding/ValidationError.cs:9-125`) has **exactly fifteen members**. (Resolved: the retired MOD_PIPELINE quoted a 4-member Phase-2-era snapshot of the same file; the 15-member listing is the code truth.) Carriers: `ValidationError(ModId, Kind, Message, ConflictingModId?, ConflictingComponent?)` — blocking; `ValidationWarning(ModId, Message)` — non-blocking advisory shown alongside success (`ValidationError.cs:136-151`).

| Member | Introduced | Meaning / producing check |
|---|---|---|
| `IncompatibleContractsVersion` | baseline | `requiresContractsVersion`/`apiVersion` unsatisfiable by the current build (Phase A); the strict-v3 parse gate reuses this semantic in its rejection message (§2.2). |
| `WriteWriteConflict` | baseline | Two systems declare writes to the same component type (Phase B). |
| `CyclicDependency` | baseline | Dependency cycle among shared or regular mods (§5.5); also the kind used when a local `DependencyGraph.Build()` fails at apply (`ModIntegrationPipeline.cs:466-474`). |
| `MissingDependency` | baseline | Required mod id absent from the load batch (optional deps downgrade to warnings); also the umbrella kind for manifest-read/assembly-load/`Initialize` failures (§8.2 note). |
| `IncompatibleVersion` | M5 | `apiVersion` or `dependencies[i].version` constraint not satisfied (Phases A+G). |
| `MissingCapability` | M3 | A `capabilities.required` token provided by neither kernel nor listed dependency (Phase C). |
| `SharedModWithEntryPoint` | M4 | `kind: "shared"` with a non-empty entry point or an `IMod` implementation (Phase F). |
| `ContractTypeInRegularMod` | M4 | Regular mod exports `IModContract`/`IEvent` types (Phase E, D-4). |
| `BridgeReplacementConflict` | M6 | Two mods in one batch replace the same FQN (Phase H, §6.2). |
| `ProtectedSystemReplacement` | M6 | `replaces` names a system not `Replaceable = true` (§6.2). |
| `UnknownSystemReplacement` | M6 | `replaces` names an FQN found in no loaded assembly (§6.2). |
| `MissingManagedStorageAttribute` | К8.3+К8.4 | `RegisterManagedComponent<T>` without `[ManagedStorage]` — caught at registration before the store is created (§4.1). |
| `FastTierContractViolation` | К10.2 | Fast-tier subscriber violates the bounded-execution contract (tier semantics: [EVENT_BUS.md](./EVENT_BUS.md)). |
| `BusTierMismatch` | К10.2 | Manifest tier-specific capability vs the event type's `[EventTier]` disagree (§3.2). |
| `BackgroundCoalesceMissing` | К10.2 | Background-tier event type missing its coalesce-function declaration (§3.2). |

**Not every violation is a `ValidationErrorKind`.** Runtime capability misses surface as `CapabilityViolationException` (§3.6, §4.3); field operation failures as `FieldOperationFailedException` (`DualFrontier.Core.Interop`); parse and registration failures as `InvalidOperationException` naming the path/type (§2.3, §4.1); quiescence timeout exists as `PipelineQuiescenceTimeoutException` (`SimulationStateController.cs:142`).

**Documented-but-reserved names (no enum member exists):** `QuiescentStatePreconditionViolated`, `PipelineQuiescenceTimeout`, `LayerCapabilityMismatch`, `VulkanModResourceCleanupFailed` (К10.3 v2 era) and `FieldRegistrationConflict`, `InvalidFieldDimensions`, `FieldCapabilityMismatch`, `ComputePipelineCompilationFailed`, `ComputePipelineRegistrationConflict`, `ComputeUnsupportedWarning` (K9/compute era). Their enum entries land with the implementations that need them — [docs/ROADMAP.md](../ROADMAP.md) §V substrate / §Native foundation tracks.

---

## Cross-references

| Document | Relation | Note |
|---|---|---|
| [MODDING.md](./MODDING.md) | defers-to | Mod-author guide; owns the SDK reference-set statement and tutorials; its examples must satisfy §2/§3 here. |
| [EVENT_BUS.md](./EVENT_BUS.md) | defers-to | Bus tiers, dispatch/flush semantics, current/target native-bus wiring; §3.2/§12 declare only the capability-side of tiers. |
| [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) | defers-to | Part 0 К-L9 (vanilla = mods), К-L18 (quiescent state), К-L3/К-L3.1 (storage paths), К-L17 (layer tokens). |
| ENGINE_LIFECYCLE_AND_TRANSACTIONS.md (AUTHORED draft) | cites | Transition vocabulary; the §9.1 commit/reclaim law framing; fenced state-machine and fault-quarantine targets. |
| [ANALYZER_RULES.md](./ANALYZER_RULES.md) | defers-to | Shipped analyzer registry (17 detecting; severities); deferral tables for the access-completeness family. |
| [ECS.md](./ECS.md) | cites | `NativeWorld` span/batch access surface referenced by §10. |
| [THREADING.md](./THREADING.md) | cites | Scheduler thread model, `DependencyGraph` edge-building, async ban. |
| [FIELDS.md](./FIELDS.md) | defers-to | Field storage contract behind `IModFieldApi`; field failure modes. |
| [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md) | cites | V resource cleanup primitive (step 3.6), compute substrate that will implement `ComputePipelines`. |
| [CONTRACTS.md](./CONTRACTS.md) | cites | Bus/marker conventions; `ContractsVersion` evolution rules. |
| [ARCHITECTURE.md](./ARCHITECTURE.md) | cites | Layer map; mods live above Domain through `IModApi`. |
| EXECUTION_AUTHORITY_MATRIX.md (AUTHORED draft) | cites | Cutover gates for the native-bus authority switch referenced in §9.6. |
| PERSISTENCE_SNAPSHOT_CONTRACT.md (AUTHORED draft) | cites | Owner of the D-6 missing-mod policy implementation (§9.8). |
| [docs/ROADMAP.md](../ROADMAP.md) | defers-to | Migration state authority; deferred milestones (analyzer scan, field registry wiring, compute surface). |
| [FRAMEWORK.md](../governance/FRAMEWORK.md) | governance | Ratification and amendment protocol. |

## Amendment protocol

This document changes by ratified amendment per [FRAMEWORK.md](../governance/FRAMEWORK.md) §7: propose the diff with code anchors re-verified at the amendment HEAD, obtain Crystalka ratification, bump the register version. Deviation discovered in implementation is escalated ("stop, document, wait for the lock") — never resolved by improvisation in code. Locked decisions (the five strategic locks and D-1…D-7) reopen only by explicit amendment naming the lock.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.1 | 2026-07-17 | HALT-1-ratified review corrections (CORPUS_CLOSURE_INVERSION_B, D1 R2-4..R2-8): §2.2/§7 `EffectiveApiVersion` claim corrected to the real Phase-A dual-path mechanism (the property has zero production callers); §5.4 phantom "loader warning" retired to convention-only truth; §9.4 step-3 Path β reclamation gains the `RegisterRestrictedModApi`-unwired honesty clause (reclamation rides ALC collection at steps 6-7 today); §7 `VersionConstraint` anchor widened to `:7-38,100-110`; §10.1 deferral-registry phrasing aligned to ANALYZER_RULES §5's unassigned-ID candidate record. |
| (candidate) | 2026-07-15 | Authored as the merged successor of MOD_OS_ARCHITECTURE v1.12.0 + MOD_PIPELINE v0.3 + ISOLATION v1.1.2; one unload chain, one fault timing, one capability grammar, one enum registry, commit/reclaim split adopted; all code claims re-verified at HEAD `35364c2`. |

Pre-merge history lives in git and in the three historical predecessors; per-decision provenance (D-1…D-7 options) is preserved verbatim in `historical/MOD_OS_ARCHITECTURE.md` §12.