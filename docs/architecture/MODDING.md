---
register_id: DOC-A-MODDING_V2
project: Dual Frontier
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 1.0.2
first_authored: 2026-07-15
last_modified: 2026-07-17
content_language: en
next_review_due: 2027-Q3
title: Writing mods (authored rework; guide, non-normative — every example passes the real v3 schema)
supersedes:
- DOC-A-MODDING
last_modified_commit: 6a67da5
review_cadence: on-change+annual
last_review_date: 2026-07-17
last_review_event: 'STACK_UPDATE Phase H doc census — v1.0.1 → v1.0.2 PATCH: §10 quickstart external copy-paste commands, both steps, -f net8.0 → -f net10.0 (the two only stale TFM sites in this guide; solution TFM moved at EVT-2026-07-17-STACK_UPDATE). Prior context: DRAFTS_RATIFICATION MC-1 (C5): candidate-banner class retired - banner to…'
reviewer: Crystalka
special_case_rationale: Ratified LOCKED v1.0.0 2026-07-17 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION (checklist item [1]). Guide successor of DOC-A-MODDING per EVT-2026-07-15-CORPUS_REWORK_R2_PLATFORM (normative law lives in DOC-A-MOD_OS_ARCHITECTURE); the refusal-list fiction stays retired.
---

# Writing Mods

A practical guide for mod authors: what `IMod`/`IModApi` let you do, what the manifest schema actually accepts, and the one proven pattern for sharing an event type across mods.

> **Ratified successor (LOCKED v1.0.0 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION, 2026-07-17).** Successor of `docs/architecture/historical/MODDING.md` (DOC-A-MODDING, now SUPERSEDED). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`.

## Status block

| Field | Value |
|---|---|
| Role | **guide** — non-normative. Every "MUST"/"forbidden" statement is a restatement of law owned elsewhere; on disagreement, the owning doc wins. |
| Successor of | `docs/architecture/historical/MODDING.md` (DOC-A-MODDING) |
| Scope | Writing, packaging, shipping a mod: `IMod`/`IModApi`, `mod.manifest.json` v3 schema, capability tokens, inter-mod contracts, the shared-mod event pattern. |
| Non-goals | Manifest grammar, capability model, ALC lifecycle, unload/reload state machine (MOD_OS_ARCHITECTURE.md law); engine-side pipeline internals. |
| Authority domains | None — worked-examples layer over MOD_OS_ARCHITECTURE.md and CONTRACTS.md. |
| Defers to | MOD_OS_ARCHITECTURE.md — manifest/capability grammar, ALC isolation, unload/reload chain, fault handling. CONTRACTS.md — canonical `IModApi`/`IModContract` surface, bus list. ARCHITECTURE.md — layer map. |

## 1. Why contracts, not references

A mod project has exactly one `ProjectReference`: `DualFrontier.Contracts`. It cannot reference `DualFrontier.Core`, `Systems`, `Components`, `Events`, `AI`, or `Application` — no project file makes `using DualFrontier.Core;` compile. The rule is named in the codebase as **TechArch 11.8** (`mods/DualFrontier.Mod.Example/README.md` §Rules; `ModIsolationException.cs:10`; `ModLoader.cs:11-13`) and holds in practice: every one of the 7 mod projects under `mods/` and every test-fixture mod under `tests/Fixture.*` references only `DualFrontier.Contracts.csproj` (the sole exception is fixture-to-fixture, §9).

A mod interacts with the running game exclusively through `IModApi`, handed to it in `IMod.Initialize`. Slower to write than RimWorld-style Harmony patching, but a mod that only calls declared, versioned surface keeps working across core and other-mod updates.

## 2. `IMod` — the entry point

```csharp
// src/DualFrontier.Contracts/Modding/IMod.cs
public interface IMod
{
    void Initialize(IModApi api);
    void Unload();
}
```

`ModLoader.LoadRegularMod` (`ModLoader.cs:64-102`) loads your entry assembly by path, locates the class implementing `IMod` via reflection, constructs it. `Initialize` must return quickly — other mods' loading waits on it; no blocking I/O. `Unload` must release everything your mod holds; see §11.

## 3. `IModApi` — what a mod may do

`IModApi` (`src/DualFrontier.Contracts/Modding/IModApi.cs`) is the entire surface. The implementation, `RestrictedModApi`, is `internal sealed` — casting the interface to a concrete type is structurally impossible from a mod's ALC (not a runtime check that could be forgotten; see MOD_OS_ARCHITECTURE.md, cast-prevention/structural-barrier rule).

```csharp
public interface IModApi
{
    void RegisterComponent<T>() where T : unmanaged, IComponent;        // Path α — NativeWorld struct storage
    void RegisterManagedComponent<T>() where T : class, IComponent;     // Path β — per-mod managed store, needs [ManagedStorage]
    void RegisterSystem<T>() where T : class;

    void Publish<T>(T evt) where T : IEvent;
    void Subscribe<T>(Action<T> handler) where T : IEvent;

    void PublishContract<T>(T contract) where T : IModContract;
    bool TryGetContract<T>(out T? contract) where T : class, IModContract;

    IReadOnlySet<string> GetKernelCapabilities();
    ModManifest GetOwnManifest();
    void Log(ModLogLevel level, string message);

    IModFieldApi? Fields { get; }
    IModComputePipelineApi? ComputePipelines { get; }
}
```

Two members need a current-truth correction against the old guide:

- **`Fields`** — doc comment says "null on builds without K9 field storage support." K9 storage itself *is* shipped (`RawTileField<T>`, see [FIELDS](./FIELDS.md)), but the sub-API is gated separately: the sole production construction site, `ModIntegrationPipeline.cs:399`, calls `new RestrictedModApi(mod.ModId, mod.Manifest, _registry, _contractStore, _services, _kernelCapabilities)` — no `fieldRegistry` argument. `Fields` is `null` in every mod loaded through the production pipeline today, regardless of K9's storage-layer status. Null-check unconditionally.
- **`ComputePipelines`** — always `null` (`RestrictedModApi.cs:216`, hardwired). The old guide's "null pre-K9 (lands at G0)" is stale terminology: G0 was superseded by the V substrate (Q-G-2); V0/V1 shipped the kernel-side compute path, but mod-facing registration is Planned — see [ROADMAP](../ROADMAP.md) §Native foundation tracks.

`Publish`/`Subscribe` are capability-gated: `RestrictedModApi.EnforceCapability` (`RestrictedModApi.cs:230-247`) builds the token `kernel.{publish|subscribe}:{EventType.FullName}` and requires it in `capabilities.required` — *unless* the manifest's capability set is empty entirely, in which case the call is allowed with a console warning ("v1 manifests are accepted in a grace period"). Don't rely on the grace period; declare your tokens (§7).

## 4. Allowed and forbidden actions

| Action | Allowed | Reason |
|---|---|---|
| Publish / subscribe to events | Yes | Through `IModApi`, capability-gated (§3, §7) |
| Register a Path α component | Yes | `RegisterComponent<T>()` — `T : unmanaged, IComponent` |
| Register a Path β component | Yes | `RegisterManagedComponent<T>()` — class + `[ManagedStorage]` |
| Register a system | Yes | Through `IModApi`; requires `[SystemAccess]` |
| Publish / retrieve an inter-mod contract | Yes | `PublishContract`/`TryGetContract` — the only mod-to-mod channel |
| Register a field | Depends | `IModApi.Fields` — `null` in production today (§3) |
| Register a compute pipeline | Depends | `IModApi.ComputePipelines` — always `null` today (§3) |
| Obtain a reference to `NativeWorld` or any concrete system | No | Not in the reference surface (§1) |
| Cast `IModApi` to its concrete type | No | `internal sealed`; unresolvable from a mod ALC |
| Reference another mod's assembly directly | No | Different ALCs; use `IModContract` (§8) or a shared mod (§9) |

## 5. What is physically reachable

The old edition of this guide described an `AssemblyLoadContext` that inspects each requested assembly name and explicitly refuses `DualFrontier.Core`/`Systems`/`Components`/`Events`/`Application` by string. **That mechanism does not exist in the current loader.** The whole of `ModLoadContext` (`ModLoadContext.cs:29-54`) is a collectible-ALC constructor plus a two-line `Load` override: if a shared ALC was supplied and already has an assembly of that simple name cached, return it; otherwise return `null` and let the default context resolve it. No blocklist, no name filtering. `SharedAssemblyResolutionTests.ModLoadContext_without_shared_alc_returns_null_for_any_name` pins exactly this: "with no shared ALC supplied, the override must defer all resolution to the default context" — for *any* name.

The real boundary is three-legged: (1) the **compile-time reference surface** (§1) — your project has no reference to `DualFrontier.Core`, so code that needs `World` does not compile; (2) **shared-ALC resolution** for cross-mod types (§9); (3) **internal/sealed types** — `RestrictedModApi` and `ModLoadContext` are both `internal sealed` (`RestrictedModApi.cs:38`, `ModLoadContext.cs:16`), unresolvable and uncastable from a mod. `ModIsolationException.cs:15-20` states the model: "isolation is now enforced at compile time via `[SystemAccess]`, and the `RestrictedModApi` cast-prevention rule is a structural barrier..., not a runtime check." Treat "only reference Contracts" as a hard rule, not a wall the loader enforces for you. The full isolation model is MOD_OS_ARCHITECTURE.md's domain; this section only flags what an author needs to not get wrong.

## 6. `mod.manifest.json` — the real v3 schema

`manifestVersion` must be the literal string `"3"` — `ManifestParser.Parse` (`ManifestParser.cs:53-59`) rejects anything else before reading another field. This is the only accepted schema version; there is no grace period for v1/v2 manifests.

Below is a trimmed, working manifest — the actual shape shipped at `mods/DualFrontier.Mod.Vanilla.Combat/mod.manifest.json`, field-for-field:

```json
{
  "manifestVersion": "3", "id": "com.example.voidmagic", "name": "Void Magic",
  "version": "1.2.0", "author": "Example Modder", "kind": "regular",
  "apiVersion": "^1.0.0",
  "entryAssembly": "VoidMagic.dll", "entryType": "VoidMagic.VoidMagicMod",
  "hotReload": true,
  "dependencies": [ { "id": "com.example.artifactmod", "optional": true } ],
  "replaces": [],
  "capabilities": {
    "required": ["kernel.subscribe:VoidMagic.Shared.VoidSpellCastEvent"],
    "provided": []
  }
}
```

Field notes, re-read against `ManifestParser.cs` and `ModManifest.cs`:

- `id`, `name`, `version`, `author` — required strings (`ReadRequiredString`, `ManifestParser.cs:61-63`; `author` optional, defaults `""`).
- `kind` — `"regular"` (has an `IMod`, default) or `"shared"` (pure type vendor, no entry point — §9).
- **Contracts-version constraint — two real fields, not one.** `ModManifest.cs` carries both: `RequiresContractsVersion` (line 81), the v1-era field — consumed by the legacy Phase-A path, which strips any `^`/`~` prefix (`ContractsVersion.cs:59-61`) and applies floor-within-major semantics via `IsCompatible`; it cannot express an exact pin; and `ApiVersion` (line 115), the v2 field, a `VersionConstraint` that accepts an optional `^` caret prefix. `ManifestParser` reads both (`:66` and `:192-220`); Phase A prefers `ApiVersion`, falling back to `RequiresContractsVersion` only when `apiVersion` is absent (default `"1.0.0"`). **Six of the seven shipped mods declare `apiVersion` with a caret** (`"^1.0.0"`); the Example mod omits it and falls back to the `requiresContractsVersion` default (`"1.0.0"`) — declare `apiVersion` in new manifests; declaring both is pointless since `ApiVersion` wins.
- `entryAssembly` / `entryType` — required for `kind: "regular"`; must be empty for `kind: "shared"` (`ContractValidator` Phase F, `ContractValidator.cs:687-776`).
- `hotReload` — opt-in boolean, defaults `false` (`ManifestParser.cs:72`).
- `dependencies` — bare id strings or `{ "id", "version", "optional" }` objects, not mixable in one manifest (`ManifestParser.cs:274-285`). **No separate `optionalDependencies` array** — optionality is the per-entry `"optional": true` flag (real fixture: `tests/Fixture.RegularMod_MissingOptional/mod.manifest.json:11`; read at `ManifestParser.cs:345-356`). A missing required dependency blocks loading; a missing optional one does not.
- `replaces` — FQNs of kernel bridge systems this mod supersedes; target must carry `[BridgeImplementation(Replaceable = true)]`; two mods may not replace the same FQN in one batch (`ContractValidator.cs:566-667`; bridge-replacement law in MOD_OS_ARCHITECTURE.md).
- `capabilities.required` / `.provided` — validated by `ManifestCapabilities.Parse`; grammar in §7.

No `description` field exists in the parsed schema — `ManifestParser` never reads one. `mods/DualFrontier.Mod.Example/mod.manifest.json` includes it anyway; it is silently ignored.

## 7. Capability grammar

Capability tokens are validated by `ManifestCapabilities.Parse` against one compiled pattern (`ManifestCapabilities.cs:24-26`):

```
^(kernel|mod\.[a-z0-9.]+)\.((?:fast|normal|background)\.(?:publish|subscribe)
   |publish|subscribe|read|write
   |field\.(read|write|acquire|conductivity|storage|dispatch)|pipeline\.register
  ):[A-Za-z][A-Za-z0-9_.]+$
```

Plain terms: `<provider>.<verb>:<FullyQualifiedTypeName>`, `<provider>` is `kernel` or `mod.<lowercase-dotted-id>`, `<verb>` is `publish`/`subscribe`/`read`/`write`, a tier-prefixed `fast.`/`normal.`/`background.` + `publish`/`subscribe`, a `field.*` verb, or `pipeline.register`. `ManifestCapabilitiesTests.cs` pins the edge cases: uppercase providers reject, a bare verb with no `:FQN` rejects, `mod.` with no id segment rejects, an FQN starting with a digit rejects.

A manifest with an empty (or absent) `capabilities` block bypasses the gate at runtime with a console warning, not a hard failure (`RestrictedModApi.cs:234-241`) — a grace period, not a recommendation; declare the tokens for every event type you publish or subscribe.

The full grammar, tier-capability/bus interaction, and how the kernel's own provided-capability set is built are MOD_OS_ARCHITECTURE.md's domain (manifest-grammar and capability-model sections); this section only pins the validator's actual regex so your manifest passes on the first try.

## 8. Contract discovery between mods

Mods do not reference each other's assemblies. A mod that wants to expose an API publishes an interface deriving from `IModContract` (a plain marker, `IModContract.cs`) via `api.PublishContract<IVoidMagicContract>(new VoidMagicImpl())`; a consumer retrieves it via `if (api.TryGetContract<IVoidMagicContract>(out var voidMagic)) { /* integrate */ }` — no crash, no hard dependency if the provider isn't loaded. `RestrictedModApi.PublishContract`/`TryGetContract` (`RestrictedModApi.cs:192-200`) proxy straight to `IModContractStore`; when the provider unloads, its contracts are revoked and later `TryGetContract` calls return `false`.

`IVoidMagicContract` itself has to live somewhere both mods' ALCs resolve to the *same* `Type` — the shared-mod pattern below. `ContractValidator` Phase E (`ContractValidator.cs:317-374`) actively rejects a regular mod whose own assembly exports a type implementing `IModContract` or `IEvent`, forcing this pattern rather than letting it compile by accident and break under multi-mod loads.

## 9. The shared-mod pattern — the only way to share an event type

This is the fix for the old guide's broken quickstart: `DualFrontier.Events`/`Components` are not in a mod's reference surface (§1), and `DualFrontier.Contracts` defines only the `IEvent` marker interface and bus-routing attributes — no concrete event type lives there to subscribe to (verified: the only `*Event*` files in `DualFrontier.Contracts` are the `IEvent` marker, the `IEventBus` interface, and the two bus-routing attributes `EventBusAttribute.cs`/`EventTierAttribute.cs`). A mod cannot `Subscribe<DeathEvent>` any core event, and cannot `Subscribe<T>` another mod's event type unless that type comes from somewhere both ALCs agree on.

That somewhere is a **shared mod** — `"kind": "shared"`, no `IMod`, no entry point, loaded once into a singleton `SharedModLoadContext` that every regular mod's `ModLoadContext` delegates to for names it recognizes (`ModLoadContext.cs:45-54`). Tested and working (`tests/Fixture.SharedEvents` + `Fixture.PublisherMod` + `Fixture.SubscriberMod`), not a hypothetical:

```json
// MyFirstMod.Events/mod.manifest.json — the shared mod. No entryAssembly/entryType.
{ "manifestVersion": "3", "id": "com.example.myfirstmod.events",
  "name": "MyFirstMod Shared Events", "version": "1.0.0", "author": "You", "kind": "shared" }
```

```csharp
// MyFirstMod.Events/ItemBlessedEvent.cs
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
namespace MyFirstMod.Events;

[EventBus("World")]
public sealed record ItemBlessedEvent(EntityId ItemId) : IEvent;
```

The regular mod references the shared project with `Private=false`, exactly as the test fixtures do (`Fixture.PublisherMod.csproj:13-15`, `Fixture.SubscriberMod.csproj:13-15`):

```xml
<ProjectReference Include="..\MyFirstMod.Events\MyFirstMod.Events.csproj">
  <Private>false</Private>
</ProjectReference>
```

Different reason than the old guide gave for the `Contracts` reference (that blanket claim matches no shipped mod's `.csproj` — none of the 7 vanilla/example mods set `Private` on `Contracts`, which resolves from the default context regardless). For a shared-mod reference specifically, `Private=false` stops your build bundling a redundant copy into your own output folder — a copy that, if ever loaded instead of the shared ALC's, hands you a different `Type` for the "same" event and silently breaks `Subscribe`/`Publish` matching.

```csharp
public sealed class MyFirstMod : IMod
{
    public void Initialize(IModApi api) => api.Subscribe<ItemBlessedEvent>(OnItemBlessed);
    public void Unload() { /* subscription removed automatically — §11 */ }
    private void OnItemBlessed(ItemBlessedEvent e) { /* ... */ }
}
```

## 10. Step-by-step: your first mod

1. **Shared-events project** (skip if you only consume events another mod already vends): `dotnet new classlib -n MyFirstMod.Events -f net10.0`, reference only `DualFrontier.Contracts`, add your event type(s), `"kind": "shared"` manifest (§9).
2. **Mod project**: `dotnet new classlib -n MyFirstMod -f net10.0`. Reference `DualFrontier.Contracts` and, if used, the shared-events project with `Private=false`.
3. **Write the `IMod` implementation** — as in §9.
4. **Write `mod.manifest.json`** — `manifestVersion: "3"`, `apiVersion` with a caret range, `capabilities.required` naming every event FQN you publish/subscribe (§7).
5. **Build and place in `mods/com.example.myfirstmod/`** (and `mods/com.example.myfirstmod.events/` if used) — each directory holds its built `.dll` alongside its `mod.manifest.json`.
6. **Launch the game.** The mod menu applies mods and rebuilds the scheduler; nothing changes mid-session (MOD_OS_ARCHITECTURE.md owns that pipeline).

## 11. Unload and reload

`IMod.Unload` must release everything your mod holds; event subscriptions registered via `Subscribe<T>` are removed automatically as part of the unload chain (`RestrictedModApi.UnsubscribeAll`) — no need to track and unsubscribe yourself. Path β managed-component data does not survive a reload — it lives in a per-mod store collected with the `AssemblyLoadContext`.

The exact unload sequence — order, atomic vs. best-effort, timeout behavior — is normative content owned by [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md); this guide does not restate it, so the two documents cannot drift the way the old MODDING/MOD_PIPELINE/ISOLATION trio did. Practical takeaway: don't leave a reference to your mod's types in another mod's static field — `AssemblyLoadContext.Unload` is collaborative, and a stray external reference is what keeps an assembly pinned after the chain runs.

## Cross-references

| Doc | Relation | Note |
|---|---|---|
| [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) | defers-to | Manifest/capability grammar, ALC isolation, unload/reload chain, fault handling — normative for everything this guide demonstrates. |
| [CONTRACTS](./CONTRACTS.md) | defers-to | Canonical `IModApi`/`IModContract` surface and the bus list `Publish`/`Subscribe` route through. |
| [ARCHITECTURE](./ARCHITECTURE.md) | cites | Layer map; the "mods reference only Contracts" row this guide demonstrates in practice. |
| [FIELDS](./FIELDS.md) | cites | `RawTileField<T>` storage backing `IModFieldApi`, when wired. |
| [EVENT_BUS](./EVENT_BUS.md) | cites | Bus tiers (`fast`/`normal`/`background`) that capability tokens and `[EventBus]` route through. |

## Amendment protocol

This guide changes freely under normal review — it carries no independent law, so an edit never requires ratification. Post-ratification, edits need only a spot-check that examples still compile; only MOD_OS_ARCHITECTURE.md/CONTRACTS.md changes require re-verifying this doc's examples.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.1 | 2026-07-17 | HALT-1-ratified review corrections (CORPUS_CLOSURE_INVERSION_B, D1 R2-9..R2-14): apiVersion census corrected 7→6-of-7 (Example omits it); v1-field characterization corrected to code truth (parser strips `^`/`~`, gate is floor-within-major — no exact pin); `*Event*` census gains `IEventBus.cs`; fixture anchor `:9`→`:11`; TechArch 11.8 naming anchor `:16-20`→`:10`; §7 regex restored verbatim (both `?:` non-capturing markers). |
| 0.1.0 (this doc) | 2026-07-15 | Corpus rework: reclassified guide/non-normative; manifest examples corrected to the real v3 schema (`apiVersion`/`requiresContractsVersion` dual path, `dependencies[].optional`, no `optionalDependencies`); quickstart replaced with the verified shared-mod pattern; ALC "refusal list" corrected to the actual compile-time-reference-surface model; unload/reload deferred to MOD_OS_ARCHITECTURE.md. |
| 1.1 | pre-rework | Last state of predecessor `DOC-A-MODDING` (see historical/). |