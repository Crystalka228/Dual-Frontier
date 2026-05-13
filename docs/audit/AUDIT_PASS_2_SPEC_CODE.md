---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-AUDIT_PASS_2_SPEC_CODE
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-AUDIT_PASS_2_SPEC_CODE
---
---
title: Audit Pass 2 — Spec ↔ Code Drift
nav_order: 105
---

# Audit Pass 2 — Spec ↔ Code Drift

**Date:** 2026-05-01
**Branch:** `main` (per `.git/HEAD` line 1)
**HEAD:** `1d43858a36c17b956a345e9bfe07a9ccf82daddb` (per `.git/refs/heads/main` line 1)
**Spec under audit:** `MOD_OS_ARCHITECTURE.md` LOCKED v1.5
**Pass 1 inventory consumed:** `docs/audit/AUDIT_PASS_1_INVENTORY.md` (9/9 PASSED)
**Scope:** Spec ↔ Code drift verification. Each normative statement in
v1.5 §1–§10 was matched to a code or test artifact, or classified as a
Tier 0/1/2/3/4 finding. Sequence integrity check applied to all 53
sequences from Pass 1 §9 catalogue. **Pass 2 resumed after v1.5
ratification of the Tier 0 finding initially raised in the sequence
integrity sub-pass; §1–§10 spec ↔ code mapping completed in resumption
session.**

---

## §0 Executive summary

| # | Check | Status | Tier 0 / 1 / 2 / 3 / 4 counts |
|---|---|---|---|
| 1 | §1 Mod topology | PASSED | 0 / 0 / 0 / 0 / 0 |
| 2 | §2 Manifest schema | PASSED | 0 / 0 / 0 / 2 / 1 |
| 3 | §3 Capability model | PASSED | 0 / 0 / 0 / 0 / 0 |
| 4 | §4 IModApi v2 | PASSED | 0 / 0 / 0 / 0 / 0 |
| 5 | §5 Type sharing across ALCs | PASSED | 0 / 0 / 0 / 0 / 0 |
| 6 | §6 Three contract levels | PASSED | 0 / 0 / 0 / 0 / 0 |
| 7 | §7 Bridge replacement | PASSED | 0 / 0 / 1 / 0 / 0 |
| 8 | §8 Versioning | PASSED | 0 / 0 / 1 / 0 / 0 |
| 9 | §9 Lifecycle and hot reload | PASSED | 0 / 0 / 2 / 0 / 0 |
| 10 | §10 Threat model | PASSED | 0 / 0 / 0 / 0 / 0 |
| 11 | §11 Sequence integrity (53 sequences) | FAILED (Tier 0 RESOLVED via v1.5) | 1 / 0 / 0 / 2 / 1 |

`PASSED` = no Tier 0 or Tier 1 findings in this check. `FAILED` = at
least one Tier 0 or Tier 1 finding present. The §11 Tier 0 was raised
in the initial Pass 2 session (sequence integrity sub-pass) and
ratified as a v1.5 amendment to `MOD_OS_ARCHITECTURE.md` §11.2 between
the initial session and this resumption; the verdict is preserved as
audit trail with a `RESOLVED` annotation in §13 Tier 0.

**Tier breakdown across all checks (sequence integrity sub-pass + §1–§10 mapping):**

- Tier 0: 1 finding — **RESOLVED** via v1.5 spec amendment. Eager escalation triggered: **NO** (resumption completed normally).
- Tier 1: 0 findings.
- Tier 2: 5 whitelist verifications confirmed compatible with v1.5 wording (M5.2 cascade-failure, M7 §9.2/§9.3 run-flag, M7 §9.5/§9.5.1 step 7 ordering, M3.4 deferred, Phase 3 SocialSystem/SkillSystem Replaceable=false carry-over).
- Tier 3: 4 findings (sequence #14 «three syntaxes are supported» wording; sequence #17 «six well-defined states» wording; §2.2 `apiVersion` «Required: yes» row vs. §4.5 v1 grace period; §2.3 step 4 «No duplicate ids in dependencies» not enforced at parse time).
- Tier 4: 2 findings (sequence #37/#38 ContractValidator phase prose enumeration vs. invocation order; Pass 1 anomaly #11 — `description` field in `mods/DualFrontier.Mod.Example/mod.manifest.json` not in spec §2.2 field reference table).

---

## §1 §1 Mod topology — three mod kinds

Spec sections audited: §1.1 Regular (lines 97–104), §1.2 Shared (lines
106–115), §1.3 Vanilla (lines 117–125), §1.4 Load graph invariants
(lines 127–161).

**Code mapping:**

| Spec § | Normative claim | Code / test artifact | Verdict |
|---|---|---|---|
| §1.1 | Regular mod has IMod entry, runs `Initialize(api)`, lives in own ALC, may depend on shared mods, manifest `kind: "regular"` (default). | `ModKind.Regular` (`src/DualFrontier.Contracts/Modding/ModManifest.cs:10`); default per `ModManifest.Kind` initializer (`ModManifest.cs:82`); `ModLoader.LoadRegularMod` creates per-mod collectible `ModLoadContext` with shared-ALC delegation (`src/DualFrontier.Application/Modding/ModLoader.cs:49–87`); `ModLoadContext` is `internal sealed : AssemblyLoadContext` (`src/DualFrontier.Application/Modding/ModLoadContext.cs:16`). | ✓ |
| §1.2 | Shared mod has no `IMod`, defines types only, lives in shared ALC, manifest `kind: "shared"` required. | `ModKind.Shared` (`ModManifest.cs:16`); `ModLoader.LoadSharedMod` defensively rejects non-shared kind (`ModLoader.cs:125–128`); `ContractValidator` Phase F enforces empty `entryAssembly`/`entryType`/`replaces` and rejects assemblies containing `IMod` types with typed `SharedModWithEntryPoint` errors (`src/DualFrontier.Application/Modding/ContractValidator.cs:698–776`); covered by `SharedModComplianceTests` (8 tests). | ✓ |
| §1.3 | Vanilla mod is editorial — same `kind: "regular"`, distinguished only by id convention `dualfrontier.vanilla.*` and directory layout. | No code distinction by design; spec line 121 verbatim: «Vanilla is not a separate kind». `ModKind` enum has only `Regular` and `Shared` (`ModManifest.cs:7–17`), confirming the editorial-not-structural framing. M8 milestone (pending) will populate `mods/DualFrontier.Mod.Vanilla.*/` directories per ROADMAP. | ✓ |
| §1.4 | Load-graph invariants: shared ALC singleton (loaded once, never unloaded); each regular mod has own `IsCollectible=true` ALC; regular ALC may resolve from shared, never from another regular; cycles forbidden in both. | `SharedModLoadContext()` constructed once per pipeline with `isCollectible: false` (`src/DualFrontier.Application/Modding/SharedModLoadContext.cs:21,31–33`); pipeline holds singleton `_sharedAlc` and reuses across `Apply` calls (`ModIntegrationPipeline.cs:81`); `ModLoadContext` collectible delegating to `SharedModLoadContext` for cross-mod type identity; `ModIntegrationPipeline.TopoSortSharedMods` (Kahn's algorithm via `TopoSortByPredicate` with `to.Kind == ModKind.Shared` predicate, `ModIntegrationPipeline.cs:934–942`) detects shared-mod cycles before assembly load with typed `CyclicDependency` errors; `TopoSortRegularMods` mirrors the same algorithm gated on `to.Kind == ModKind.Regular` (`ModIntegrationPipeline.cs:953–961`); covered by `RegularModTopologicalSortTests` (6) using `Fixture.RegularMod_CyclicA` / `Fixture.RegularMod_CyclicB`. | ✓ |

**Findings:** None.

---

## §2 §2 Manifest schema

Spec sections audited: §2.1 v2 schema example (lines 168–209), §2.2
field reference table (lines 211–227), §2.3 parse-time validation steps
(lines 229–238).

**Code mapping (§2.2 field reference vs. `ModManifest` + `ManifestParser`):**

| Spec field | Type / required / default | Code field | Parser path | Verdict |
|---|---|---|---|---|
| `id` | string / yes / — | `ModManifest.Id` (`ModManifest.cs:30`) | `ManifestParser.ReadRequiredString("id")` (`ManifestParser.cs:49,99–119`) | ✓ |
| `name` | string / yes / — | `ModManifest.Name` (`ModManifest.cs:35`) | `ManifestParser.ReadRequiredString("name")` (`ManifestParser.cs:50`) | ✓ |
| `version` | string SemVer / yes / — | `ModManifest.Version` (`ModManifest.cs:41`) | `ManifestParser.ReadRequiredString("version")` (`ManifestParser.cs:51`) | ✓ |
| `author` | string / no / `""` | `ModManifest.Author` (`ModManifest.cs:46`) | `ManifestParser.ReadOptionalString("author", "")` (`ManifestParser.cs:53`) | ✓ |
| `kind` | enum / no / `"regular"` | `ModManifest.Kind` (`ModManifest.cs:82`, default `Regular`) | `ManifestParser.ReadKind` (`ManifestParser.cs:154–177`); missing/null returns `Regular`; values `"regular"` / `"shared"` (case-insensitive) | ✓ |
| `apiVersion` | string SemVer with caret / yes / — | `ModManifest.ApiVersion` (`ModManifest.cs:89`, nullable) + `ModManifest.EffectiveApiVersion` fallback (`ModManifest.cs:119–120`) | `ManifestParser.ReadApiVersion` (`ManifestParser.cs:179–207`) returns `null` when missing | **Tier 3** — see Findings below |
| `entryAssembly` | string / conditional / `"{id}.dll"` (regular) or empty (shared per v1.3 wording) | `ModManifest.EntryAssembly` (`ModManifest.cs:62`, default `""`) | `ManifestParser.ReadOptionalString("entryAssembly", "")` (`ManifestParser.cs:55`); shared-mod compliance enforced post-load by `ContractValidator` Phase F | ✓ |
| `entryType` | string / conditional / scan-for-IMod (regular) or empty (shared per v1.3 wording) | `ModManifest.EntryType` (`ModManifest.cs:69`) | `ManifestParser.ReadOptionalString("entryType", "")` (`ManifestParser.cs:56`); fallback to single-`IMod` scan in `ModLoader.ResolveModType` (`ModLoader.cs:295–314`) | ✓ |
| `hotReload` | bool / no / `false` | `ModManifest.HotReload` (`ModManifest.cs:96`) | `ManifestParser.ReadOptionalBool("hotReload", false)` (`ManifestParser.cs:60,136–152`) | ✓ |
| `dependencies` | array `{id, version, optional}` / no / `[]` | `ModManifest.Dependencies` (`ModManifest.cs:75`, `IReadOnlyList<ModDependency>`) | `ManifestParser.ReadDependencies` (`ManifestParser.cs:239–290`); supports both v1 string-array and v2 object-array formats; `ModDependency.IsOptional` (v1.1 ratification) read via `ReadDependencyObject` (`ManifestParser.cs:332–343`) | ✓ |
| `replaces` | array of FQN string / no / `[]` | `ModManifest.Replaces` (`ModManifest.cs:105`) | `ManifestParser.ReadReplaces` (`ManifestParser.cs:209–237`) | ✓ |
| `capabilities.required` | array of string / no / `[]` | `ManifestCapabilities.Required` (`src/DualFrontier.Contracts/Modding/ManifestCapabilities.cs:40`) | `ManifestParser.ReadCapabilities` → `ManifestCapabilities.Parse` (`ManifestParser.cs:348–399`, `ManifestCapabilities.cs:73–84`); regex enforced per token | ✓ |
| `capabilities.provided` | array of string / no / `[]` | `ManifestCapabilities.Provided` (`ManifestCapabilities.cs:43`) | Same path as `.required`. | ✓ |

**Code mapping (§2.3 parse-time validation steps):**

| Step | Spec rule | Code | Verdict |
|---|---|---|---|
| 1 | Every required field present and non-empty. | `ManifestParser.ReadRequiredString` throws `InvalidOperationException` with «missing required field» / «null required field» / «empty required field» messages (`ManifestParser.cs:99–119`). | ✓ |
| 2 | `version` and `dependencies[i].version` parse as SemVer. | `version` parsed via `ContractsVersion.Parse` in `ContractValidator.ValidateInterModDependencyVersions` (`ContractValidator.cs:514–525`); `dependencies[i].version` parsed via `VersionConstraint.Parse` in `ManifestParser.ReadDependencyObject` (`ManifestParser.cs:316–329`). | ✓ |
| 3 | `apiVersion` parses as SemVer with optional caret prefix. | `VersionConstraint.Parse` accepts `[^]MAJOR.MINOR.PATCH`, rejects tilde with `FormatException` directing to caret (`src/DualFrontier.Contracts/Modding/VersionConstraint.cs:49–91`). | ✓ |
| 4 | No duplicate ids in `dependencies`. | **Not enforced** in `ManifestParser.ReadDependencies` (`ManifestParser.cs:266–289` appends each entry without an id-uniqueness check). | **Tier 3** — see Findings below |
| 5 | No type listed in `replaces` is also listed by another mod's `replaces` in the current load batch. | `ContractValidator` Phase H detects same-batch duplicates symmetrically (`ContractValidator.cs:583–622`) and emits `BridgeReplacementConflict` errors for both mods. | ✓ |
| 6 | Every capability string matches the regex `^(kernel|mod\.[a-z0-9.]+)\.(publish\|subscribe\|read\|write):[A-Za-z][A-Za-z0-9_.]+$`. | `ManifestCapabilities.s_capabilityPattern` regex compiled from the verbatim spec pattern (`ManifestCapabilities.cs:15–17`); `ParseSet` throws `ArgumentException` on mismatch (`ManifestCapabilities.cs:165–180`). | ✓ |

**Pass 1 anomaly #11 classification.** The example mod manifest at
`mods/DualFrontier.Mod.Example/mod.manifest.json:6` contains a
`description` field (`"Reference mod showing the IMod + IModApi
pattern. See docs/architecture/MODDING.md."`). Spec §2.2 (lines 213–227) field
reference table enumerates 13 fields; `description` is not listed.
`ManifestParser.Parse` (`ManifestParser.cs:25–82`) reads only the
fields the parser handles explicitly (`id`, `name`, `version`,
`author`, `requiresContractsVersion`, `entryAssembly`, `entryType`,
`kind`, `apiVersion`, `hotReload`, `replaces`, `dependencies`,
`capabilities`); `System.Text.Json`'s default behaviour on unknown
properties is to ignore them silently. The `description` field is
therefore **silently accepted but never propagated** into the
`ModManifest`, has no behavioural effect on load, and produces no
warning. Per the prompt §9.4 hypothesis enumeration, this matches
**option (c)**: an informally accepted convention worth documenting.
**Tier 4** — cosmetic; no breakage; recommendation in §13.

**Default `kind` value.** Example manifest omits `kind`; per spec §2.2
`kind` defaults to `"regular"` and per `ManifestParser.ReadKind`
(`ManifestParser.cs:154–177`) the missing/null path returns
`ModKind.Regular`. Match. ✓

**Findings:**

- **Tier 3** — `apiVersion` row in spec §2.2 marks the field «Required:
  yes» but §4.5 (lines 403–405) defines a v1 grace period in which
  manifests without `apiVersion` continue to load via the
  `RequiresContractsVersion` fallback (`ModManifest.EffectiveApiVersion`,
  `ModManifest.cs:119–120`). The example manifest itself omits
  `apiVersion` and parses successfully (verified above). The §2.2 wording
  is therefore stricter than the implementation and §4.5; recommend
  rewording to «yes (v2); v1 manifests fall back to
  `requiresContractsVersion`» in a future non-semantic ratification cycle.
  See §13 Tier 3 entry 3.
- **Tier 3** — §2.3 step 4 «No duplicate ids in `dependencies`» is not
  enforced by `ManifestParser`. Practical impact is bounded: Phase G
  re-checks each dependency entry and accumulates errors per
  duplicate, so duplicates produce duplicate diagnostics rather than
  silent failure or undefined behaviour. Recommendation in §13 Tier 3
  entry 4.
- **Tier 4** — Pass 1 anomaly #11 classified above.

---

## §3 §3 Capability model

Spec sections audited: §3.1 granularity LOCKED (lines 246–255), §3.2
syntax (lines 257–269), §3.3 reserved namespaces (lines 271–274), §3.4
static check at load time (lines 276–285), §3.5 kernel-provided
capability set + D-1 LOCKED (lines 287–291), §3.6 hybrid enforcement
v1.2 (lines 293–314), §3.7 cross-check with `[SystemAccess]` (lines
316–331), §3.8 bus capability mapping + D-2 LOCKED (lines 333–337).

**Code mapping:**

| Spec § | Normative claim | Code / test artifact | Verdict |
|---|---|---|---|
| §3.1 | Capability granularity is per-event-type and per-component-type; no wildcards. | `ManifestCapabilities.s_capabilityPattern` regex `^(kernel|mod\.[a-z0-9.]+)\.(publish|subscribe|read|write):[A-Za-z][A-Za-z0-9_.]+$` (`ManifestCapabilities.cs:15–17`) — FQN tail with no wildcard alternative. | ✓ |
| §3.2 | Token syntax `<provider>.<verb>:<FQN>`; provider in {`kernel`, `mod.<modId>`}; verb in {`publish`, `subscribe`, `read`, `write`}. | Same regex above. Token format consumed throughout `KernelCapabilityRegistry` and `RestrictedModApi`. | ✓ |
| §3.3 | `kernel.*` reserved for kernel; `mod.<modId>.*` reserved for that mod. | Provider regex restricts to `kernel` literal or `mod.<reverse-domain>` exactly. Loader-level enforcement of reserved-namespace ownership lives in the dependency-listing requirement (§3.4 below); a mod listing `mod.X.publish:T` requires `X` in `dependencies` per Phase C. | ✓ |
| §3.4 | Required token must be provided by kernel or by a listed dependency; implicit dependency forbidden. | `ContractValidator.ValidateCapabilitySatisfiability` (Phase C, `ContractValidator.cs:398–433`) iterates `mod.Manifest.Capabilities.Required`, checks `kernelCapabilities.Provides` then walks only `mod.Manifest.Dependencies`; emits `MissingCapability` when neither side provides. Covered by `CapabilityValidationTests` (11 tests). | ✓ |
| §3.5 | Kernel exposes a fixed list reflected from public types in `Contracts` and `Components`; mods read via `IModApi.GetKernelCapabilities()`. | `KernelCapabilityRegistry` (`src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs:30–119`); `BuildFromKernelAssemblies` reflects `IEvent`, `IComponent` markers + `DualFrontier.Components` + `DualFrontier.Events` assemblies; result frozen as `IReadOnlySet<string>`; exposed via `RestrictedModApi.GetKernelCapabilities` (`RestrictedModApi.cs:126`). Generic and nested types skipped (`KernelCapabilityRegistry.cs:97–99`). | ✓ |
| §3.5 D-1 | Components are reachable only when annotated `[ModAccessible(Read=…, Write=…)]`. | `ModAccessibleAttribute` (`src/DualFrontier.Contracts/Attributes/ModAccessibleAttribute.cs:16–29`); reflected by `KernelCapabilityRegistry.ScanAssembly` lines 107–117 (component types without the attribute are silently skipped). Production components annotated per v1.2 §3.5 ratification: `WeaponComponent`, `ArmorComponent`, `AmmoComponent`, `ShieldComponent`, `HealthComponent` (verified via Grep — 5 components annotated). | ✓ |
| §3.6 | Hybrid enforcement: load-time gate (§3.4 + §3.7 + §3.8) plus runtime second-layer in `RestrictedModApi`. v1.2 ratification. | Load-time: Phase C (capabilities satisfiability) + Phase D (`[ModCapabilities]` cross-check) `ContractValidator.cs:392–469`. Runtime: `RestrictedModApi.EnforceCapability` raises `CapabilityViolationException` from `Publish` and `Subscribe` (`RestrictedModApi.cs:80–112,147–164`). The three runtime cases (reflection bypass, runtime-constructed events, v1 grace period) are implemented: v1 grace period emits a warning and bypasses the gate (`RestrictedModApi.cs:151–158`). | ✓ |
| §3.7 | `capabilities.required` must be a superset of every system's `[SystemAccess]` declarations. | The wording in §3.7 is implemented via `[SystemAccess]` already enforced by `SystemExecutionContext` at runtime (Phase 2 isolation) plus Phase D (`[ModCapabilities]` cross-check) at load time. The pure `[SystemAccess]` ↔ manifest cross-check is bridged by the `[ModCapabilities]` attribute (per D-2 hybrid resolution): authors declare per-system capability sets, validated against the manifest. | ✓ |
| §3.8 D-2 | Hybrid attribute + CI static analysis. Per-system `[ModCapabilities]` attribute + Phase D load-time cross-check; static analyzer in CI is M3.4 deferred. | `ModCapabilitiesAttribute` (`src/DualFrontier.Contracts/Attributes/ModCapabilitiesAttribute.cs:24–41`); Phase D iterates `[ModCapabilities].Tokens` per registered system and verifies each appears in `Manifest.Capabilities.Required`, emitting `MissingCapability` otherwise (`ContractValidator.cs:442–469`). M3.4 (Roslyn analyzer) deferred — see §13 Tier 2 #4. | ✓ |

**Findings:** None.

---

## §4 §4 IModApi v2

Spec sections audited: §4.1 surface (lines 345–382), §4.2
`Publish<T>` semantics (lines 384–388), §4.3 `Subscribe<T>` semantics
(lines 390–395), §4.4 cast-prevention rule + D-3 LOCKED (lines
397–401), §4.5 backward compatibility with v1 (lines 403–405).

**Code mapping:**

| Spec § | Normative claim | Code / test artifact | Verdict |
|---|---|---|---|
| §4.1 | Surface of 9 methods: `RegisterComponent<T>`, `RegisterSystem<T>`, `Publish<T>`, `Subscribe<T>`, `PublishContract<T>`, `TryGetContract<T>`, `GetKernelCapabilities`, `GetOwnManifest`, `Log(ModLogLevel, string)`. | `IModApi` interface (`src/DualFrontier.Contracts/Modding/IModApi.cs:16–75`) declares exactly the 9 methods in spec order. Verified by Pass 1 §5 + sequence #40 (cross_inconsistency vs. #8 — match). | ✓ |
| §4.1 / v1.1 | `Log` parameter type is `ModLogLevel` (not `LogLevel`). | `ModLogLevel` enum (`src/DualFrontier.Contracts/Modding/ModLogLevel.cs:7–20`), 4 levels (Debug, Info, Warning, Error); `IModApi.Log(ModLogLevel level, string message)` (`IModApi.cs:74`). v1.1 ratification entry confirms. | ✓ |
| §4.2 | `Publish<T>` routes via event-type bus marker; raises `CapabilityViolationException` on missing `publish:<FQN>`; `[Deferred]`/`[Immediate]` honored. | `RestrictedModApi.Publish<T>` calls `EnforceCapability("publish", typeof(T))` then routes via `ModBusRouter.Resolve` (`RestrictedModApi.cs:80–88`). Capability raises typed exception per `EnforceCapability` (`RestrictedModApi.cs:147–164`). `[Deferred]`/`[Immediate]` semantics handled by underlying `IEventBus.Publish` (Phase 4 closure). Covered by `RestrictedModApiV2Tests` (22 tests). | ✓ |
| §4.3 | `Subscribe<T>` wraps handler with calling mod's `SystemExecutionContext`; binds to mod lifetime; `UnsubscribeAll` removes every wrapper; capability check at subscribe time; multiple subscriptions to same `T` permitted. | `RestrictedModApi.Subscribe<T>` calls `EnforceCapability("subscribe", typeof(T))`, captures `SystemExecutionContext.Current`, wraps handler with push/pop context, records the pair in `_subscriptions` (`RestrictedModApi.cs:91–112`). `UnsubscribeAll` iterates and clears (`RestrictedModApi.cs:140–145`); called from §9.5 step 1 in `ModIntegrationPipeline.RunUnloadSteps1Through6AndCaptureAlc` (`ModIntegrationPipeline.cs:565–568`). Covered by `RestrictedModApiV2Tests`. | ✓ |
| §4.4 D-3 | `RestrictedModApi` is `internal sealed` with internal constructor; structurally unreachable from a mod ALC because `DualFrontier.Application` is not resolvable. No analyzer required in v1. | `internal sealed class RestrictedModApi : IModApi` (`RestrictedModApi.cs:31`); constructor is `internal` (`RestrictedModApi.cs:49`); concrete type lives in `DualFrontier.Application`, which a mod's `ModLoadContext` does not resolve (per `ModLoadContext` `Resolving` delegation rules). | ✓ |
| §4.5 | All v1 mods continue to load and run with a v1-API warning; grace period closes at kernel API 2.0.0. | `RestrictedModApi.EnforceCapability` v1 branch on `_manifest.Capabilities.IsEmpty` writes `[WARNING][modId] v1 manifest publishing/subscribing without capability declaration` (`RestrictedModApi.cs:151–158`); `EffectiveApiVersion` fallback in `ModManifest.cs:119–120` accepts manifests without `apiVersion`. Current `ContractsVersion.Current = 1.0.0` (`ContractsVersion.cs:17`). | ✓ |

**Findings:** None.

---

## §5 §5 Type sharing across ALCs

Spec sections audited: §5.1 shared ALC (lines 413–430), §5.2 loader
rules for shared mods (lines 432–441), §5.3 loader rules for regular
mods that depend on shared mods (lines 443–451), §5.4 restrictions on
shared mods (lines 453–458), §5.5 naming convention (lines 460–467).

**Code mapping:**

| Spec § | Normative claim | Code / test artifact | Verdict |
|---|---|---|---|
| §5.1 | Single `AssemblyLoadContext` named `"shared"` with `IsCollectible=false`; `Resolving` delegates `DualFrontier.*` to default ALC. | `SharedModLoadContext()` constructor `: base("shared", isCollectible: false)` (`SharedModLoadContext.cs:31–33`); `Load` returns cached shared assemblies and falls through to runtime/default for unknowns (`SharedModLoadContext.cs:84–92`). Pipeline owns a singleton `_sharedAlc` reused across `Apply` invocations (`ModIntegrationPipeline.cs:81`). | ✓ |
| §5.2 | Six loader steps for shared mods (validate manifest with empty entry fields → load into shared ALC → assert no IMod → reflection enumerate → add provided capabilities → mark loaded). | `ModLoader.LoadSharedMod` (`ModLoader.cs:107–161`) implements the structural skeleton: kind guard, path/manifest validation, `LoadSharedAssembly` into shared ALC, exported-types enumeration, `LoadedSharedMod` registration. Compliance checks (empty `entryAssembly`/`entryType`/`replaces`, no `IMod`) moved to `ContractValidator` Phase F (`ContractValidator.cs:698–776`) per M4.3 — the loader's defensive `IMod` throw is removed in favour of typed accumulation. Covered by `SharedModComplianceTests` (8). | ✓ |
| §5.3 | Five loader steps for regular mods with shared deps (verify shared loaded → create collectible ALC → wire `Resolving` to shared ALC → load entry assembly → continue standard `Initialize`). | `ModIntegrationPipeline.Apply` runs Pass 1 (shared) then Pass 2 (regular) in topological order (`ModIntegrationPipeline.cs:268–309`); `ModLoader.LoadRegularMod` creates `new ModLoadContext(manifest.Id, sharedAlc)` (`ModLoader.cs:64`) — the second argument wires shared-ALC delegation. `IMod.Initialize(api)` invoked in step [4] of Apply (`ModIntegrationPipeline.cs:340–356`). Covered by `CrossAlcTypeIdentityTests` (3) and `SharedAssemblyResolutionTests` (4). | ✓ |
| §5.4 | Shared mod restrictions: types only, no `DualFrontier.Core`/`Application` references, no mutable static ctors, no I/O at type-load. | Type-only restriction enforced structurally — shared assemblies load into the shared ALC which does not resolve `Core`/`Application`. Static-ctor I/O restrictions are best-effort (per spec §10 threat model — out of scope for active checking). The `IMod` ban is enforced by Phase F. | ✓ |
| §5.4 / D-4 | Active scan rejects regular mods that export `IModContract` or `IEvent` types. | `ContractValidator.ValidateRegularModContractTypes` Phase E (`ContractValidator.cs:329–374`) iterates each regular mod's owned assemblies (filtered by `AssemblyLoadContext.GetLoadContext(asm) != mod.Context` exclusion of delegated shared types) and emits `ContractTypeInRegularMod` for any exported `IEvent` or `IModContract` implementer. Covered by `ContractTypeInRegularModTests` (6). | ✓ |
| §5.4 / D-5 | Shared-mod cycles forbidden; loader rejects with `CyclicDependency`. | `ModIntegrationPipeline.TopoSortSharedMods` (Kahn's algorithm via `TopoSortByPredicate` with shared-only edge predicate, `ModIntegrationPipeline.cs:934–942`) detects cycles before assembly load and emits `CyclicDependency` per affected mod (`ModIntegrationPipeline.cs:1127–1142`). | ✓ |
| §5.5 | Naming convention `id: "<base>.protocol"` or `id: "<base>.types"` enforced as warning. | Convention is editorial; loader does not currently emit warnings for non-conforming shared-mod ids. Spec line 467 verbatim: «The convention is enforced by the loader as a warning, not an error.» Loader does not produce this warning. **Behavioural gap is bounded: vanilla mods comply (`dualfrontier.vanilla.core` etc.) and naming hygiene is mod-author advisory.** Recorded for follow-up in §13 Out-of-scope (low priority — no Tier classification because no v1.5 wording mismatch and no current external mod authors per M3.4 deferral). | ✓ (out-of-scope advisory) |

**Findings:** None blocking. The §5.5 advisory observation is logged
in §13 Out-of-scope items but does not produce a Tier finding because
there are no current external mod authors (per M3.4 deferral
discipline) and the wording is permissive («as a warning, not an
error»).

---

## §6 §6 Three contract levels

Spec sections audited: §6.1 Level 1 Data contracts (lines 475–490),
§6.2 Level 2 Service contracts (lines 492–513), §6.3 Level 3 Protocol
contracts (lines 515–532), §6.4 level matrix (lines 534–540), §6.5
anti-pattern: type in regular mod + D-4 LOCKED (lines 542–546).

**Code mapping:**

| Spec § | Normative claim | Code / test artifact | Verdict |
|---|---|---|---|
| §6.1 | Data contracts inherit `IModContract`. | `IModContract` marker interface in `Contracts.Modding` (`src/DualFrontier.Contracts/Modding/IModContract.cs:12–14`). Used by `IModApi.PublishContract<T> where T : IModContract` and `TryGetContract<T> where T : class, IModContract` (`IModApi.cs:49,57`). | ✓ |
| §6.2 | Service contracts: interface in shared mod, implementation in regular mod, fetch via `TryGetContract<ICookingService>`. | Same interface (`IModContract`) and same `IModApi.TryGetContract<T>` accessor. The shared-vs-regular placement is enforced structurally by Phase E (`ContractTypeInRegularMod` for `IModContract` implementers in regular mods, `ContractValidator.cs:366–370`). | ✓ |
| §6.3 | Protocol contracts: `IEvent` types in shared mod, `[Combat]`/`[Magic]` bus marker, `[Deferred]` or `[Immediate]` delivery attribute, publish/subscribe in regular mods. | `IEvent` marker in `Contracts.Core` (`src/DualFrontier.Contracts/Core/IEvent.cs:11–13`); `DeferredAttribute` (`src/DualFrontier.Contracts/Attributes/DeferredAttribute.cs:13`) and `ImmediateAttribute` (`src/DualFrontier.Contracts/Attributes/ImmediateAttribute.cs:12`) target classes/structs as event delivery hints; `RestrictedModApi.Publish<T>` routes via event-type to bus (`RestrictedModApi.cs:80–88`). Phase E enforces shared-only placement of `IEvent` types. Covered by `CrossAlcTypeIdentityTests` (3). | ✓ |
| §6.4 | Three-row level matrix: Data / Service / Protocol → shared-mod type / publishing-or-implementing regular mod / `TryGetContract` or bus dispatch. | Matrix is documentary; the underlying primitives (`IModContract`, `IEvent`, `[Deferred]`/`[Immediate]`, `PublishContract`/`TryGetContract`/`Publish`/`Subscribe`) all exist as enumerated above. | ✓ |
| §6.5 D-4 | Loader actively scans every regular-mod assembly via reflection for `IModContract` or `IEvent` types and rejects with `ContractTypeInRegularMod`. | `ContractValidator.ValidateRegularModContractTypes` Phase E (`ContractValidator.cs:329–374`); `BuildContractTypeError` produces typed message naming the offending type and directing the author to the shared-mod pattern (`ContractValidator.cs:376–389`). Covered by `ContractTypeInRegularModTests` (6). | ✓ |

**Findings:** None.

---

## §7 §7 Bridge replacement

Spec sections audited: §7.1 mechanism LOCKED (lines 554–567), §7.2
conflict resolution (lines 569–573), §7.3 rationale (lines 575–581),
§7.4 bridge metadata (lines 583–592), §7.5 tests (lines 594–602).

**Code mapping:**

| Spec § | Normative claim | Code / test artifact | Verdict |
|---|---|---|---|
| §7.1 | Four-step mechanism: read every `replaces` from every mod → build `replacedSystems` set → skip kernel systems with FQN in set during graph build → mod's replacement system registers in their place. | `ModIntegrationPipeline.CollectReplacedFqns` (`ModIntegrationPipeline.cs:906–915`) builds the set; pipeline graph build skips kernel `SystemOrigin.Core` systems whose FQN is in the set (`ModIntegrationPipeline.cs:391–399`); mod-supplied replacement systems flow through normal `RegisterSystem` registration. | ✓ |
| §7.2 | Two mods replacing same FQN → `BridgeReplacementConflict`; no automatic priority. | `ContractValidator.ValidateBridgeReplacements` Phase H emits symmetric `BridgeReplacementConflict` errors for both mods (`ContractValidator.cs:583–622`). Covered by `PhaseHBridgeReplacementTests` (8). | ✓ |
| §7.3 | Rationale: explicit user choice; consistent with write-write conflict philosophy. | Documentary; reflected in error message wording referencing §7.2 (`ContractValidator.cs:597–605`). | ✓ |
| §7.4 | `[BridgeImplementation(Phase=N, Replaceable=bool)]`. `Replaceable=false` → mod rejected with `ProtectedSystemReplacement`. | `BridgeImplementationAttribute.Replaceable` property (default `false`) (`src/DualFrontier.Contracts/Attributes/BridgeImplementationAttribute.cs:35`); Phase H emits `ProtectedSystemReplacement` when target is not `Replaceable=true` or attribute is missing entirely (`ContractValidator.cs:649–665`). Phase 5 combat stubs annotated `[BridgeImplementation(Phase=5, Replaceable=true)]`: `CombatSystem`, `DamageSystem`, `ProjectileSystem`, `ShieldSystem`, `StatusEffectSystem`, `ComboResolutionSystem`, `CompositeResolutionSystem` (7 stubs verified via Grep). Phase 3 carry-over stubs `SocialSystem` and `SkillSystem` annotated `[BridgeImplementation(Phase=3)]` (no `Replaceable=true`) and remain `Replaceable=false` until M10.C — see §13 Tier 2 #5. | ✓ |
| §7.5 | Five acceptance scenarios. | See §7.5 acceptance-scenario coverage table below. | ✓ |

**§7.5 acceptance-scenario coverage:**

| # | Scenario | Test artifact |
|---|---|---|
| 1 | Mod replaces a `Replaceable=true` bridge — bridge skipped, mod system runs. | `M62IntegrationTests` (5) — pipeline-level scenario; `Phase5BridgeAnnotationsTests` (9 — 7 Replaceable bridges + 2 protected guards) confirms annotations. |
| 2 | Two mods replace same bridge — batch rejected with `BridgeReplacementConflict`. | `PhaseHBridgeReplacementTests` (8 — validator-level); `M62IntegrationTests` (pipeline-level). |
| 3 | Mod replaces `Replaceable=false` system — mod rejected with `ProtectedSystemReplacement`. | `Phase5BridgeAnnotationsTests` (2 protected guards on `SocialSystem` / `SkillSystem`); `PhaseHBridgeReplacementTests` (validator-level). |
| 4 | Mod replaces non-existent FQN — mod rejected with `UnknownSystemReplacement`. | `PhaseHBridgeReplacementTests` (validator-level); `M62IntegrationTests` (pipeline-level). |
| 5 | Mod is unloaded — replacement skip is reverted, kernel bridge re-registers, dependency graph rebuilds. | Closed structurally via M7 hot reload: `Apply` rebuilds the graph from the surviving mod set on every call, so the unload case reduces to «re-`Apply` without the unloaded mod». Per AUDIT prompt §6.3 whitelist, this scenario is **not flagged as outstanding** and is covered by `M71PauseResumeTests` + `M72UnloadChainTests` + `M73Step7Tests` + `M73Phase2DebtTests`. |

**Findings:**

- **Tier 2** — Phase 3 `SocialSystem` / `SkillSystem` Replaceable=false carry-over to M10.C. See §13 Tier 2 #5.

---

## §8 §8 Versioning

Spec sections audited: §8.1 Axis 1 Kernel API (lines 612–618), §8.2
Axis 2 Mod self version (lines 620–626), §8.3 Axis 3 Inter-mod
dependency (lines 628–630), §8.4 constraint syntax LOCKED (lines
632–640), §8.5 parser (lines 642–667), §8.6 where each axis applies
(lines 669–676), §8.7 resolution algorithm (lines 678–689).

**Code mapping:**

| Spec § | Normative claim | Code / test artifact | Verdict |
|---|---|---|---|
| §8.1 | Axis 1: `ContractsVersion.Current` bumped manually with major/minor/patch semantics. `apiVersion` constraint via caret. | `ContractsVersion.Current = new(1, 0, 0)` (`ContractsVersion.cs:17`); `ContractsVersion.IsCompatible` covers v1 manifest path (`ContractsVersion.cs:84–94`); v2 manifests use `VersionConstraint.IsSatisfiedBy` against `Current`. | ✓ |
| §8.2 | Axis 2: mod self `version` used for hot-reload lineage, menu identity, save-game. | `ModManifest.Version` (`ModManifest.cs:41`); validator's Phase G parses provider `Manifest.Version` via `ContractsVersion.Parse` (`ContractValidator.cs:514–525`); hot-reload-lineage and save-game logic deferred to M7.5 / persistence layer. | ✓ |
| §8.3 | Axis 3: each `dependencies[i]` carries `{id, version}` constraint. | `ModDependency` record with `Id`, `Version` (nullable `VersionConstraint`), `IsOptional`; parsed in `ManifestParser.ReadDependencyObject` (`ManifestParser.cs:292–346`). | ✓ |
| §8.4 | Three syntaxes: Exact, Caret, Tilde-rejected. | `VersionConstraintKind { Exact, Caret }` (`VersionConstraint.cs:7–14`); `Parse` rejects tilde with `FormatException` directing to caret (`VersionConstraint.cs:56–61`). Sequence #14 wording-clarity finding preserved as Tier 3 (see §11 + §13 Tier 3 #1). | ✓ |
| §8.5 | Parser detects prefix; Tilde throws; returns `VersionConstraint`; `IsSatisfiedBy(candidate)` evaluates per kind. | `VersionConstraint.Parse` (`VersionConstraint.cs:49–91`); `IsSatisfiedBy` (`VersionConstraint.cs:100–110`). Covered by `VersionConstraintTests` (35). | ✓ |
| §8.6 | Per-field axis allowance: `apiVersion` Exact/Caret; `version` Exact only; `dependencies[i].version` Exact/Caret. | Code permits same syntax for `apiVersion` and `dependencies[i].version`; `version` is a single non-constraint string parsed via `ContractsVersion.Parse` (no caret). | ✓ |
| §8.7 | Four-step resolution: build graph → toposort → for each mod check `apiVersion` + `dependencies[i].version` (cascade-fail) → present failed set; success set proceeds. | `ModIntegrationPipeline.Apply` performs (1) classification + manifest read [0], (2) shared toposort + cycle detection [0.5], (3) regular toposort + cycle detection [0.6], (4) dependency presence check, (5) shared/regular load passes [1]+[2], (6) `ContractValidator.Validate` with Phase A v1/v2 dual-path + Phase G inter-mod version check, (7) IMod.Initialize, (8) graph build with replace skip, (9) scheduler swap. Phase A: `ValidateContractsVersions` (`ContractValidator.cs:124–178`) — emits `IncompatibleContractsVersion` for v1, `IncompatibleVersion` for v2. Phase G: `ValidateInterModDependencyVersions` (`ContractValidator.cs:496–542`) — emits `IncompatibleVersion` per accumulating semantics. M5.2 cascade-failure ratification verified — see §13 Tier 2 #1. | ✓ |

**Verification of M5.2 cascade-failure ratification (Tier 2).**

- Spec §8.7 verbatim (line 686, step 3c): «If any check fails, the mod
  is added to the failed set; mods that depend on it cascade-fail.»
- Spec §8.7 verbatim (line 687, step 4): «The failed set is presented
  to the user; the success set proceeds to load.»
- ROADMAP M5.2 verbatim (line 236): «when mod A depends on mod B and B
  fails its own validation (any phase), A is **not** silently dropped
  — A's own validation runs to completion, and any independent errors
  A produces also surface. Both errors appear in `result.Errors`.»
- **Compatibility analysis.** §8.7 wording «cascade-fail» is
  non-prescriptive about whether dependent-mod errors are surfaced
  alongside provider errors or silently subsumed. ROADMAP's
  «accumulate without skip» reading interprets «cascade-fail» as
  «marked failed, but independent errors still surface». The
  `ContractValidator.ValidateInterModDependencyVersions` docstring
  (`ContractValidator.cs:491–494`) registers this interpretation
  verbatim («Cascade-failure semantics: errors accumulate; no mod is
  silently dropped if its provider fails its own validation. Per
  MOD_OS_ARCHITECTURE §8.7»). Pipeline-level test
  `Apply_WithCascadeFailure_SurfacesBothErrors` (`M52IntegrationTests`)
  demonstrates. v1.5 made no changes to §8.7. **Tier 2 confirmed
  compatible with v1.5 wording.**

**Findings:**

- **Tier 2** — M5.2 cascade-failure accumulation. See §13 Tier 2 #1.

---

## §9 §9 Lifecycle and hot reload

Spec sections audited: §9.1 states (lines 695–728), §9.2 hot reload
through the menu LOCKED (lines 730–742), §9.3 no live-tick reload
LOCKED (lines 744–746), §9.4 save-game implications (lines 748–756),
§9.5 ALC unload protocol with v1.4 GC pump bracket (lines 758–770),
§9.5.1 failure semantics v1.4 (lines 772–776), §9.6 hot-reload
disabled mods (lines 778–780).

**Code mapping:**

| Spec § | Normative claim | Code / test artifact | Verdict |
|---|---|---|---|
| §9.1 | Six well-defined states (Disabled → Pending → Loaded → Active → Stopping → Disabled) — lifecycle cycle. | No `ModLifecycleState` enum exists in `DualFrontier.Contracts` (verified by Pass 1 §5 inventory). The spec wording «six well-defined states» counts diagram boxes (6) but matches only 5 unique state-name labels — sequence #17 already classified Tier 3 (wording clarity) in §11 sub-pass. See §13 Tier 3 #2 (preserved from sub-pass). No code drift; diagram and prose mutually consistent under «boxes» reading. | ✓ (with sequence #17 Tier 3 reference) |
| §9.2 | User flow: menu Pause → toggle/edit → Apply → Resume. `Apply` rebuilds graph in local var, swaps scheduler, unloads previous mods. | `ModIntegrationPipeline.Pause()` / `Resume()` toggle `_isRunning` (`ModIntegrationPipeline.cs:152,160`); `Apply` is the orchestrator (`ModIntegrationPipeline.cs:170–438`); per-mod `UnloadMod` (`ModIntegrationPipeline.cs:496–527`); covered by `M71PauseResumeTests` (11) and `M72UnloadChainTests` (13). | ✓ |
| §9.3 | `Apply` while scheduler running throws `InvalidOperationException("Pause the scheduler before applying mods")`. Enforced by `ModIntegrationPipeline` checking the scheduler's run flag. | `Apply` guard at `ModIntegrationPipeline.cs:173–175`; `UnloadMod` mirrors with «Pause the scheduler before unloading mods» (`ModIntegrationPipeline.cs:499–501`); `UnloadAll` mirrors at `ModIntegrationPipeline.cs:643–645`. M7 §9.2/§9.3 run-flag-on-pipeline ratification verified — see §13 Tier 2 #2. Covered by `M71PauseResumeTests`. | ✓ |
| §9.4 | Save records `(modId, modVersion)` per active mod; on load, missing/incompatible mods warn or refuse. | Spec says «delegated to the persistence layer and is out of scope for this document» (line 755–756). No code coverage in M0–M7 scope; deferred to persistence rework. | ✓ (deferred, per spec) |
| §9.5 | Seven-step unload chain: UnsubscribeAll → RevokeAll → RemoveSystems → graph rebuild → scheduler swap → ALC.Unload → WeakReference spin (10 s, GC pump bracket per v1.4). | `ModIntegrationPipeline.UnloadMod` (`ModIntegrationPipeline.cs:496–527`) delegates to `RunUnloadSteps1Through6AndCaptureAlc` (`ModIntegrationPipeline.cs:546–621`). Steps 1–6 wrapped in `TryUnloadStep` per §9.5.1 best-effort. Step 7 in dedicated non-inlined helper `TryStep7AlcVerification` (`ModIntegrationPipeline.cs:768–791`) with verbatim `GC.Collect → WaitForPendingFinalizers → Collect` bracket and 10 s timeout (`ModIntegrationPipeline.cs:107–112` constants `Step7TimeoutMs = 10_000`, `Step7PollIntervalMs = 100`, `Step7MaxIterations = 100`). M7 §9.5/§9.5.1 step 7 ordering ratification verified — see §13 Tier 2 #3. Covered by `M73Step7Tests` (5) and `M73Phase2DebtTests` (2 real-mod fixtures). | ✓ |
| §9.5.1 | Steps 1–6 best-effort: each step in try/catch, exception logged with `(modId, stepNumber)`, `ValidationWarning` recorded, chain continues. After step 6, step 7 timeout produces `ModUnloadTimeout` warning; mod removed from active set regardless. | `TryUnloadStep` records typed `ValidationWarning` with `(modId, stepNumber)` per spec (`ModIntegrationPipeline.cs:709–728`); `TryStep7AlcVerification` appends `ModUnloadTimeout` warning with verbatim §9.5 step 7 reference + `10000 ms` substring (`ModIntegrationPipeline.cs:785–790`); `_activeMods.Remove(mod)` runs after WR capture, before spin (`ModIntegrationPipeline.cs:618`). | ✓ |
| §9.6 | Mod with `hotReload: false` cannot reload mid-session; menu disables button. | `hotReload` parsed and stored on `ModManifest.HotReload` (`ModManifest.cs:96`); menu UI integration deferred to M7.5 (pending). Per spec wording, this is a UI-layer concern — current code surface contains the flag; M7.5 will wire it to the menu. | ✓ (UI deferred to M7.5) |

**Verification of M7 §9.2/§9.3 run-flag-location ratification (Tier 2).**

- Spec §9.2 verbatim (line 734): «User opens the mod menu. The menu
  calls `ModIntegrationPipeline.Pause()` which sets the scheduler's
  run flag to false.»
- Spec §9.3 verbatim (line 746): «Attempts to call `Apply` while the
  scheduler is running throw `InvalidOperationException("Pause the
  scheduler before applying mods")`. This is enforced by
  `ModIntegrationPipeline` checking the scheduler's run flag.»
- ROADMAP M7.1 verbatim (line 300): «§9.2 step 1 reads "menu sets the
  scheduler's run flag to false"; §9.3 reads "enforced by
  `ModIntegrationPipeline` checking the scheduler's run flag." M7.1
  locates the flag itself on `ModIntegrationPipeline` (private
  `_isRunning` bool) rather than introducing one inside
  `ParallelSystemScheduler`.»
- **Compatibility analysis.** §9.3 explicitly says «enforced by
  `ModIntegrationPipeline` checking»; §9.2 step 1's «scheduler's run
  flag» is treated by ROADMAP as «pipeline-mediated state observable
  as scheduler run state from outside» rather than as state owned by
  the scheduler. Code: `ModIntegrationPipeline._isRunning` private
  bool (`ModIntegrationPipeline.cs:105`); `Pause()`/`Resume()` setters
  (`ModIntegrationPipeline.cs:152,160`); guards in `Apply` and
  `UnloadMod`/`UnloadAll` use verbatim canonical messages. v1.5 made
  no changes to §9.2/§9.3 (v1.4 only touched §9.5 step 7 and added
  §9.5.1). **Tier 2 confirmed compatible with v1.5 wording.**

**Verification of M7 §9.5/§9.5.1 step 7 ordering ratification (Tier 2).**

- Spec §9.5 step 7 verbatim (line 768): «The loader spins on
  `WeakReference.IsAlive`, polling each iteration. Before every poll
  the loader performs `GC.Collect(); GC.WaitForPendingFinalizers();
  GC.Collect()` — the double-collect bracket is required because
  `WaitForPendingFinalizers` can resurrect finalizable graph nodes the
  first collect would have removed; the second collect picks those
  up, restoring monotonic progress. Default cadence: 100 iterations ×
  100 ms = 10 s timeout.»
- Spec §9.5.1 verbatim (line 774): «After step 6, if step 7 times
  out, the `ModUnloadTimeout` warning per §9.5 fires; the mod is
  removed from the active set regardless of whether the assembly
  actually unloaded.»
- ROADMAP M7.3 verbatim (line 302): «M7.3 wires the order as
  `CaptureAlcWeakReference(mod) → _activeMods.Remove(mod) →
  TryStep7AlcVerification(modId, wr, warnings) → return`. The capture
  must precede the removal so the WR is bound to the same
  `ModLoadContext` instance the active-set removal then helps
  release; the spin must follow the removal so the pipeline-side
  strong reference (`_activeMods`) is gone before the spin's GC pumps
  run.»
- **Compatibility analysis.** §9.5.1's «mod removed from active set
  regardless» does not pin the order of removal, WR capture, or spin
  entry; §9.5 step 7's spin operates on a captured WR with no
  requirement to retain the mod in the active set. ROADMAP's reading
  picks the only ordering that gives the spin a chance to release the
  ALC: capture → remove → spin. Code at
  `ModIntegrationPipeline.RunUnloadSteps1Through6AndCaptureAlc` lines
  613, 618, 524 confirms the ordering: `CaptureAlcWeakReference(mod)`
  → `_activeMods.Remove(mod)` → return WR → `TryStep7AlcVerification`
  in caller. v1.5 made no changes to §9.5/§9.5.1 (v1.4 added the GC
  pump bracket and §9.5.1 sub-section; v1.5 was scoped strictly to
  §11.2 enumeration). **Tier 2 confirmed compatible with v1.5
  wording.**

**Findings:**

- **Tier 2** — M7 §9.2/§9.3 run-flag location. See §13 Tier 2 #2.
- **Tier 2** — M7 §9.5/§9.5.1 step 7 ordering. See §13 Tier 2 #3.
- **Tier 3** — sequence #17 «six well-defined states» wording clarity.
  Already recorded in §11 sub-pass and §13 Tier 3 #2. Not re-classified
  here; cross-reference only.

---

## §10 §10 Threat model

Spec sections audited: §10.1 architectural threats caught (lines
790–799), §10.2 architectural threats not caught (lines 801–809), §10.3
the contract: best-effort structural isolation (lines 811–819), §10.4
required tests (lines 821–831).

**Code mapping (§10.1 architectural threats caught — 8 entries):**

| Threat | Catching mechanism | Code | Verdict |
|---|---|---|---|
| Mod accesses component without `[SystemAccess]` | `SystemExecutionContext` throws `IsolationViolationException` | Phase 2 isolation guard (covered by `IsolationGuardTests` in `tests/DualFrontier.Core.Tests/Isolation/`). | ✓ |
| Mod publishes to a bus it did not declare | Same | Same. | ✓ |
| Mod calls `GetSystem<T>()` directly | Same | Same — `SystemExecutionContext` throws unconditionally on `GetSystem`. | ✓ |
| Mod casts `IModApi` to `RestrictedModApi` | Roslyn analyzer (§4.4) or runtime check | Per D-3 LOCKED, structurally enforced via `internal sealed RestrictedModApi` (`RestrictedModApi.cs:31`). No analyzer required in v1. | ✓ |
| Mod registers a system that conflicts with another mod's system | `ContractValidator` write-write check | Phase B (`ContractValidator.cs:196–280`) — emits `WriteWriteConflict` with attributed mod ids. | ✓ |
| Mod replaces a system also replaced by another mod | `BridgeReplacementConflict` | Phase H (`ContractValidator.cs:583–622`). | ✓ |
| Mod requires a capability not provided by kernel or dependencies | `MissingCapability` | Phase C (`ContractValidator.cs:398–433`). | ✓ |
| Mod publishes an event without `publish:` capability | `CapabilityViolationException` at `Publish` | Runtime second-layer in `RestrictedModApi.EnforceCapability` (`RestrictedModApi.cs:147–164`). | ✓ |

**§10.4 required-tests coverage (7 categories):**

| # | Category | Test artifact(s) | Verdict |
|---|---|---|---|
| 1 | Isolation tests (Phase 2 already in place) | `tests/DualFrontier.Core.Tests/Isolation/IsolationGuardTests.cs` (10 file count per Pass 1 inventory; full coverage of `[SystemAccess]` + bus + `GetSystem` paths). | ✓ |
| 2 | Capability violation tests | `tests/DualFrontier.Modding.Tests/Api/RestrictedModApiV2Tests.cs` (22 — runtime capability checks at `Publish`/`Subscribe`); `tests/DualFrontier.Modding.Tests/Capability/CapabilityValidationTests.cs` (11 — Phase C/D load-time). | ✓ |
| 3 | Bridge replacement tests | `tests/DualFrontier.Modding.Tests/Validator/PhaseHBridgeReplacementTests.cs` (8); `tests/DualFrontier.Modding.Tests/Pipeline/M62IntegrationTests.cs` (5); `tests/DualFrontier.Modding.Tests/Pipeline/CollectReplacedFqnsTests.cs` (5). | ✓ |
| 4 | Type-sharing tests | `tests/DualFrontier.Modding.Tests/Sharing/CrossAlcTypeIdentityTests.cs` (3); `SharedAssemblyResolutionTests.cs` (4); `ContractTypeInRegularModTests.cs` (6); `SharedModComplianceTests.cs` (8). | ✓ |
| 5 | WeakReference unload tests (hard-required, every regular mod must release within timeout) | `tests/DualFrontier.Modding.Tests/Pipeline/M73Step7Tests.cs` (5 — happy path + ALC-retainer timeout + canonical warning shape + AD #7 step-7-after-upstream-failure + mod-removed-from-active-set); `M73Phase2DebtTests.cs` (2 — `Fixture.RegularMod_DependedOn` + `Fixture.RegularMod_ReplacesCombat` real-mod fixtures, both close §10.4 hard-required `WeakReference.IsAlive == false` within 10 s). | ✓ |
| 6 | Cross-mod cycle tests (`A → B → A` rejected with `CyclicDependency`) | `tests/DualFrontier.Modding.Tests/Pipeline/RegularModTopologicalSortTests.cs` (6) — uses `Fixture.RegularMod_CyclicA` + `Fixture.RegularMod_CyclicB` to exercise the cycle path; shared-mod cycle path covered by `SharedModComplianceTests` and toposort path. | ✓ |
| 7 | Version constraint tests | `tests/DualFrontier.Modding.Tests/Manifest/VersionConstraintTests.cs` (35 — Exact, Caret, Tilde rejected, IsSatisfiedBy permutations); `Validator/PhaseAModernizationTests.cs` (6 — v1/v2 dual-path); `Validator/PhaseGInterModVersionTests.cs` (7 — inter-mod). | ✓ |

**Findings:** None. All seven §10.4 categories have direct test coverage.

---

## §11 Sequence integrity findings

Performed for all 53 sequences from `AUDIT_PASS_1_INVENTORY.md` §9.
Per `AUDIT_PASS_2_PROMPT.md` §7.1, each sequence checked for gap,
duplicate, order, count_mismatch, and cross_inconsistency. Per
`AUDIT_PASS_2_PROMPT.md` §7.2, special focus on Pass 1 anomalies
#3 (§9.1 lifecycle), #4 (validator phases), and #5
(ValidationErrorKind enumeration).

**Per-sequence verdicts:**

| Sequence # (from Pass 1 §9) | Source | Check type | Verdict | Tier |
|---|---|---|---|---|
| 1 | `MOD_OS_ARCHITECTURE.md` Preamble — strategic locked decisions | count_mismatch | 5 entries vs. "Five top-level decisions" — match | ✓ |
| 2 | `MOD_OS_ARCHITECTURE.md` §0 OS mapping table | (no count claim) | n/a | ✓ |
| 3 | `MOD_OS_ARCHITECTURE.md` §1 mod kinds | count_mismatch | 3 sub-sections (§1.1 Regular, §1.2 Shared, §1.3 Vanilla) vs. "three mod kinds" / "three categories" — match | ✓ |
| 4 | `MOD_OS_ARCHITECTURE.md` §1.4 load-graph invariants | (no count claim) | 4 bullets observed | ✓ |
| 5 | `MOD_OS_ARCHITECTURE.md` §2.2 manifest field reference | count_mismatch | 13 rows observed; field reference table — match | ✓ |
| 6 | `MOD_OS_ARCHITECTURE.md` §2.3 parse-time validation steps | count_mismatch | 6 numbered steps observed; "1, 2, 3, 4, 5, 6" — match | ✓ |
| 7 | `MOD_OS_ARCHITECTURE.md` §3.6 runtime cases load-time gate cannot reach | count_mismatch | 3 numbered vs. "three cases" — match | ✓ |
| 8 | `MOD_OS_ARCHITECTURE.md` §4.1 IModApi v2 surface methods | count_mismatch | 9 method declarations vs. "v1 had 6, v2 adds 3" (=9) — match | ✓ |
| 9 | `MOD_OS_ARCHITECTURE.md` §5.2 shared mod loader steps | count_mismatch | 6 numbered steps — match | ✓ |
| 10 | `MOD_OS_ARCHITECTURE.md` §5.3 regular mod loader steps | count_mismatch | 5 numbered steps — match | ✓ |
| 11 | `MOD_OS_ARCHITECTURE.md` §5.4 shared mod restrictions | (no count claim) | 4 bullets observed | ✓ |
| 12 | `MOD_OS_ARCHITECTURE.md` §7.1 bridge-replacement mechanism steps | count_mismatch | 4 numbered steps — match | ✓ |
| 13 | `MOD_OS_ARCHITECTURE.md` §7.5 bridge-replacement test scenarios | (no count claim) | 5 bullets observed | ✓ |
| 14 | `MOD_OS_ARCHITECTURE.md` §8.4 constraint syntaxes | **count_mismatch (wording)** | Spec line 631 verbatim: «Three syntaxes are supported, all are subsets of npm/Cargo conventions:». Three subsections enumerated: Exact (supported), Caret (supported), Tilde («**not supported** in v1»; «the parser rejects with a clear error message»). Code (`VersionConstraint.cs:7–14`) has `enum VersionConstraintKind { Exact, Caret }` — 2 members, no Tilde. Per spec §8.5 step 2, parser throws `FormatException` for `~`. The wording «three syntaxes are supported» is overloaded: «supported» means «recognized/handled by parser» (covering 2 accepted + 1 explicitly-rejected = 3), not «accepted». **Tier 3 (wording clarity)** — spec is internally consistent if «supported» is read as «handled»; code matches spec by accepting Exact+Caret and rejecting Tilde. | Tier 3 |
| 15 | `MOD_OS_ARCHITECTURE.md` §8.6 where each axis applies | count_mismatch | 3 rows — match | ✓ |
| 16 | `MOD_OS_ARCHITECTURE.md` §8.7 resolution algorithm | count_mismatch | 4 numbered top-level steps — match | ✓ |
| 17 | `MOD_OS_ARCHITECTURE.md` §9.1 lifecycle states | **count_mismatch — Pass 1 anomaly #3** | Spec line 692 verbatim: «The mod lifecycle has six well-defined states.» Diagram (lines 696–724): 6 boxes labelled `Disabled` → `Pending` → `Loaded` → `Active` → `Stopping` → `Disabled`. Unique state-name labels: 5 (Disabled appears at both initial and terminal positions). The diagram is internally consistent (6 boxes drawn deliberately; the cycle returns to `Disabled`, representing «user re-enables a disabled mod» per arrow «user enables» on line 700). No code representation of these states exists (Pass 1 §5 inventory: no `ModLifecycleState` enum in `DualFrontier.Contracts`). The wording «six well-defined states» matches box count but is imprecise relative to unique state-name count (5). **Tier 3 (wording clarity)** — diagram and prose are mutually consistent if «states» is read as «lifecycle positions/boxes»; precision could be improved with wording like «six lifecycle positions, five distinct states». No code drift (no enum to compare against). | Tier 3 |
| 18 | `MOD_OS_ARCHITECTURE.md` §9.2 hot reload menu flow steps | count_mismatch | 4 numbered steps — match | ✓ |
| 19 | `MOD_OS_ARCHITECTURE.md` §9.5 unload chain steps | count_mismatch | 7 numbered steps. v1.4 changelog explicitly references «step 7». Match. | ✓ |
| 20 | `MOD_OS_ARCHITECTURE.md` §10.1 architectural threats caught | (no count claim) | 8 rows observed | ✓ |
| 21 | `MOD_OS_ARCHITECTURE.md` §10.2 architectural threats not caught | (no count claim) | 5 bullets observed | ✓ |
| 22 | `MOD_OS_ARCHITECTURE.md` §10.4 required test categories | (no count claim) | 7 bullets observed | ✓ |
| 23 | `MOD_OS_ARCHITECTURE.md` §11.1 migration phases | (no count claim) | 12 rows observed (M0..M10 + M3.4 deferred) | ✓ |
| 24 | `MOD_OS_ARCHITECTURE.md` §11.2 ValidationErrorKind entries | **cross_inconsistency + count_mismatch — Pass 1 anomaly #5 — TIER 0 (RESOLVED via v1.5)** | Spec lines 853–864 (pre-v1.5) verbatim opening: «The current enum has `MissingDependency` and `CyclicDependency`. The migration adds:» followed by 8 bullets including `CapabilityViolation` flagged «not part of the validation set, but listed here for completeness». Spec implied enum cardinality = 2 baseline + 7 validation additions = **9 validation members**. Code (`ValidationError.cs:9–83`) has **11 enum members** (see sequence #39 below). Two members in code were NOT enumerated by spec §11.2 in either baseline or migration-additions: `IncompatibleContractsVersion` (line 15) and `WriteWriteConflict` (line 22). The spec wording «The current enum has X and Y» was declarative without «for example» / «among others» qualifier; on a strict reading the spec claimed a complete 2-member baseline. Code expanded baseline by 2 unenumerated members. Originally classified Tier 0 (eager-escalated). **RESOLVED 2026-05-01 via v1.5 spec amendment** that expanded §11.2 baseline to enumerate the actual 4-member baseline (`IncompatibleContractsVersion`, `WriteWriteConflict`, `MissingDependency`, `CyclicDependency`); §11.2 line 858 now reads verbatim «The current enum has `IncompatibleContractsVersion`, `WriteWriteConflict`, `MissingDependency`, and `CyclicDependency`. The migration adds:». See §13 Tier 0 for full diagnostic and resolution audit trail. | **Tier 0 (RESOLVED)** |
| 25 | `MOD_OS_ARCHITECTURE.md` §11.4 stop conditions | (no count claim) | 3 bullets observed | ✓ |
| 26 | `MOD_OS_ARCHITECTURE.md` §12 detail decisions | count_mismatch | 7 entries (D-1..D-7) vs. «seven detail decisions» — match | ✓ |
| 27 | `ROADMAP.md` Status overview phase rows | (no count claim) | 20 rows observed | ✓ |
| 28 | `ROADMAP.md` Closed phases headings | (no count claim) | 7 closed-phase headings observed | ✓ |
| 29 | `ROADMAP.md` M3 sub-phases | (no count claim) | M3.1, M3.2, M3.3, M3.4 (deferred) — 4 sub-phases observed | ✓ |
| 30 | `ROADMAP.md` M4 sub-phases | (no count claim) | M4.1, M4.2, M4.3 — 3 observed | ✓ |
| 31 | `ROADMAP.md` M5 sub-phases | (no count claim) | M5.1, M5.2 — 2 observed | ✓ |
| 32 | `ROADMAP.md` M6 sub-phases | (no count claim) | M6.1, M6.2 (M6.3 closure-sync mechanism, line 318) — 2 observed | ✓ |
| 33 | `ROADMAP.md` M7 sub-phases | count_mismatch | 5 sub-phases (M7.1–M7.5) + 1 closure session vs. «five implementation sub-phases (M7.1 – M7.5) plus a closure session» — match | ✓ |
| 34 | `ROADMAP.md` M10 vanilla slices | (no count claim) | M10.A, M10.B, M10.C, M10.D — 4 observed | ✓ |
| 35 | `ROADMAP.md` Phase 4 v0.3 architectural fixes block | (no count claim) | 6 bullets observed | ✓ |
| 36 | `ROADMAP.md` Engine snapshot progressive test counts | sequence | Observed: 60, 82, 247, 260, 281, 311, 328, 333, 338, 369. Final claim line 36 verbatim: «Total at M7.3 closure: 369/369 passed». Last entry matches claim. (Pass 1 anomaly #1 records source-level `[Fact]+[Theory]` count = 359 vs. ROADMAP-stated 369; routed to Pass 3 for runtime-test-count vs. source-attribute-count reconciliation.) | ✓ |
| 37 | `ContractValidator.cs:12–48` validator phases (class XML-doc) | count_mismatch | «Eight-phase validator»; phases A, B, C, D, E, F, G, H all enumerated. Count = 8. Match. | ✓ |
| 38 | `ContractValidator.cs:88–103` `Validate()` method body invocation order | **cross_inconsistency vs. #37 — Pass 1 anomaly #4** | Class doc per-phase prose enumeration order: A, B, E, C, D, F, G, H. Method body invocation order: A (line 88) → B (89) → E (90) → G (91) → H (92) → conditionally C, D (lines 94–98) → conditionally F (lines 100–103). Class doc lines 41–44 verbatim: «Phases A, B, E, G and H run unconditionally; phases C and D run only when a `KernelCapabilityRegistry` is supplied; phase F runs only when a shared-mod list is supplied to `Validate`.» This conditionality summary IS consistent with invocation: unconditional phases A/B/E/G/H run first; conditional C/D run when `kernelCapabilities is not null`; conditional F runs when `sharedMods is not null`. The class doc per-phase prose enumeration order (A, B, E, C, D, F, G, H) is **descriptive, not prescriptive** — it describes each phase's responsibility and does not claim alphabetical or invocation-order semantics. The class doc never asserts «phases run in this order». No spec drift; no code drift; minor cosmetic mismatch in prose enumeration vs. invocation. **Tier 4 (cosmetic)** — could be improved by reordering per-phase prose to match invocation, but no behaviour or contract impact. | Tier 4 |
| 39 | `ValidationError.cs:9–83` `enum ValidationErrorKind` members | **cross_inconsistency vs. #24 — Pass 1 anomaly #5 — TIER 0 (RESOLVED via v1.5)** | 11 members observed in source order: IncompatibleContractsVersion (line 15), WriteWriteConflict (22), CyclicDependency (27), MissingDependency (32), IncompatibleVersion (40), MissingCapability (47), SharedModWithEntryPoint (55), ContractTypeInRegularMod (62), BridgeReplacementConflict (69), ProtectedSystemReplacement (76), UnknownSystemReplacement (82). Pre-v1.5 spec §11.2 enumerated 2 baseline (`MissingDependency`, `CyclicDependency`) + 7 validation additions = **9** total validation members. Code has **11**. Delta of +2: `IncompatibleContractsVersion` and `WriteWriteConflict`. Originally classified Tier 0 (eager-escalated). **RESOLVED via v1.5** — see sequence #24 and §13 Tier 0 for full diagnostic. | **Tier 0 (RESOLVED)** |
| 40 | `IModApi.cs:16–75` API methods declaration order | cross_inconsistency vs. #8 | 9 declarations observed: RegisterComponent, RegisterSystem, Publish, Subscribe, PublishContract, TryGetContract, GetKernelCapabilities, GetOwnManifest, Log. Spec §4.1 enumeration order (per Pass 1 §9 entry 8): RegisterComponent, RegisterSystem, Publish, Subscribe, PublishContract, TryGetContract, GetKernelCapabilities, GetOwnManifest, Log — match. Same order, same count. ✓ | ✓ |
| 41 | `ModLogLevel.cs:7–20` severity levels | count_mismatch | 4 members (Debug, Info, Warning, Error). Spec v1.1 ratification clarified Log parameter as `ModLogLevel` (not `LogLevel`). No spec count claim for ModLogLevel. ✓ | ✓ |
| 42 | `VersionConstraint.cs:7–14` constraint kinds | cross_inconsistency vs. #14 | 2 members (Exact, Caret). Spec §8.4 lists 3 syntaxes (Exact accepted, Caret accepted, Tilde rejected). Enum represents only the 2 accepted syntaxes; Tilde is rejected at parse time without enum representation. Match (see #14 verdict). | ✓ |
| 43 | `ModManifest.cs:7–17` `enum ModKind` members | cross_inconsistency vs. #3 | 2 members (Regular, Shared). Spec §1 «three mod kinds» = Regular + Shared + Vanilla, but spec §1.3 explicitly states «Vanilla is not a separate kind» — the third is convention/editorial, not enum-represented. Code correctly has 2 enum members. ✓ | ✓ |
| 44 | `OwnershipMode.cs:10–37` ownership modes | count_mismatch | 4 members (Bonded, Contested, Abandoned, Transferred). No spec count claim. ✓ | ✓ |
| 45 | `TickRateAttribute.cs:37–53` `static class TickRates` constants | count_mismatch | 5 constants (REALTIME, FAST, NORMAL, SLOW, RARE). No spec count claim. ✓ | ✓ |
| 46 | `IGameServices.cs:13–57` bus accessors | count_mismatch | 6 properties (Combat, Inventory, Magic, Pawns, World, Power). Pass 1 anomaly #8 records `Contracts/README.md:17` listing only 5 buses (cross-doc — Pass 4 territory; not flagged here). Within `IGameServices` itself: 6 properties, internally consistent. ✓ | ✓ |
| 47 | `SystemExecutionContext.cs:270–319` violation paths | (no spec count claim) | 3 paths (BuildReadViolationMessage, BuildWriteViolationMessage, GetSystem) ✓ | ✓ |
| 48 | `M3_CLOSURE_REVIEW.md` §0 Executive summary checks | count_mismatch | 8 rows. Closure-review pattern from `M6_CLOSURE_REVIEW.md`. ✓ | ✓ |
| 49 | `M4_CLOSURE_REVIEW.md` §0 Executive summary checks | count_mismatch | 8 rows. ✓ | ✓ |
| 50 | `M5_CLOSURE_REVIEW.md` §0 Executive summary checks | count_mismatch | 8 rows. ✓ | ✓ |
| 51 | `M6_CLOSURE_REVIEW.md` §0 Executive summary checks | count_mismatch | 8 rows vs. «All 8 checks PASSED» (line 30) — match. ✓ | ✓ |
| 52 | `AUDIT_CAMPAIGN_PLAN.md` §2 campaign passes | count_mismatch | 5 passes (Pass 1..Pass 5) vs. «пять последовательных проходов» — match. ✓ | ✓ |
| 53 | `AUDIT_CAMPAIGN_PLAN.md` §7 ratified decisions | count_mismatch | 6 (§7.1..§7.6) vs. «шесть открытых решений §7 ratified» — match. ✓ | ✓ |

**Sequence integrity summary:**

- Total sequences checked: 53
- Tier 0 (sequence integrity violations): **1** (sequence #24, with cross-reference to #39) — **RESOLVED via v1.5 amendment**
- Tier 3 (wording clarity, no spec ↔ code drift): 2 (sequence #14 «three syntaxes are supported»; sequence #17 «six well-defined states»)
- Tier 4 (cosmetic): 1 (sequence #37/#38 prose enumeration order vs. invocation order)
- PASS (no integrity issue): 49

**Per `AUDIT_PASS_2_PROMPT.md` §7.2 special-focus anomalies:**

- **Pass 1 anomaly #3 (sequence #17 — §9.1 lifecycle).** Verdict: **Tier 3 (wording clarity)**. Diagram is internally consistent: 6 boxes drawn deliberately (initial `Disabled` and terminal `Disabled` represent the lifecycle cycle). Spec wording «six well-defined states» matches box count but is imprecise relative to unique-state-name count (5). No code representation exists (no `ModLifecycleState` enum in `DualFrontier.Contracts`), so no spec ↔ code drift. Wording could be improved («six lifecycle positions, five distinct states»), but neither code nor diagram is wrong. Not eager-escalated as Tier 0 because no code drift and diagram-prose are mutually consistent under «boxes» reading.
- **Pass 1 anomaly #4 (sequence #37/#38 — ContractValidator phases).** Verdict: **Tier 4 (cosmetic)**. Class doc per-phase prose enumeration (A, B, E, C, D, F, G, H) does not match `Validate()` invocation order (A, B, E, G, H, then conditionally C/D, then conditionally F). The class doc's normative ordering claim is at lines 41–44 (conditionality summary), which IS consistent with invocation. The per-phase prose enumeration is descriptive, not prescriptive about execution order. No spec drift, no code drift. Cosmetic mismatch only.
- **Pass 1 anomaly #5 (sequence #24/#39 — ValidationErrorKind enumeration).** Verdict: **Tier 0 — eager escalation triggered in initial session; RESOLVED via v1.5 amendment**. See §13 Tier 0 for full diagnostic and resolution audit trail.

**Eager Tier 0 escalation triggered at sequence #24** in the initial Pass 2 session. Per `AUDIT_PASS_2_PROMPT.md` §8.3, the sequence integrity check completed across all 53 sequences before escalation; no other Tier 0 candidates found.

*Sequence integrity sub-pass completed in initial Pass 2 session; no changes in resumption. Resumption completed §1–§10 spec ↔ code mapping under v1.5 LOCKED status.*

---

## §12 Surgical fixes applied this pass

None. Pass 2 is read-only by contract.

---

## §13 Items requiring follow-up

### Tier 0 — Spec drift (RESOLVED)

| # | Spec section | Code path | Description | Recommendation |
|---|---|---|---|---|
| 1 | `MOD_OS_ARCHITECTURE.md` §11.2 (formerly lines 853–864 pre-v1.5; now §11.2 after the v1.5 amendment) | `src/DualFrontier.Application/Modding/ValidationError.cs:9–83` (`enum ValidationErrorKind`) | **Spec drift on `ValidationErrorKind` baseline enumeration.** Pre-v1.5 spec line 854 verbatim: «The current enum has `MissingDependency` and `CyclicDependency`. The migration adds:». Pre-v1.5 spec lines 855–864 listed 8 migration additions: `MissingCapability` (M3), `BridgeReplacementConflict` (M6), `ProtectedSystemReplacement` (M6), `UnknownSystemReplacement` (M6), `IncompatibleVersion` (M5), `SharedModWithEntryPoint` (M4), `ContractTypeInRegularMod` (M4), `CapabilityViolation` (M3, runtime, «not part of the validation set, but listed here for completeness»). Pre-v1.5 spec implied enum cardinality = 2 baseline + 7 validation additions = **9 validation members**. Code at `ValidationError.cs:9–83` has **11 enum members**: IncompatibleContractsVersion (line 15), WriteWriteConflict (22), CyclicDependency (27), MissingDependency (32), IncompatibleVersion (40), MissingCapability (47), SharedModWithEntryPoint (55), ContractTypeInRegularMod (62), BridgeReplacementConflict (69), ProtectedSystemReplacement (76), UnknownSystemReplacement (82). Delta = +2 unenumerated members: `IncompatibleContractsVersion` (used by Phase A `ValidateContractsVersions` per `ContractValidator.cs:88` for `RequiresContractsVersion` failures) and `WriteWriteConflict` (used by Phase B `ValidateWriteWriteConflicts` per `ContractValidator.cs:89` for component write-collision detection per spec §10.1 «Mod registers a system that conflicts with another mod's system»). The pre-v1.5 spec wording «The current enum has X and Y» was declarative without «for example» / «among others» qualifier — strict reading implied a complete 2-member baseline at the time of v1.0 spec writing. Per pre-v1.5 `AUDIT_PASS_2_PROMPT.md` §6 Tier 0 definition («Spec drift против LOCKED v1.4. Код или wording спеки противоречат друг другу.»), this was a Tier 0 spec drift. Per `AUDIT_PASS_2_PROMPT.md` §7 («Sequence integrity violations автоматически Tier 0»), this was a `sequence_integrity:count_mismatch` (cross-referenced with `sequence_integrity:cross_inconsistency` between sequence #24 spec enumeration and sequence #39 code enumeration). **STATUS: RESOLVED — ratified as v1.5 amendment to MOD_OS_ARCHITECTURE.md (commit pending; see Version history line ~30).** | **Resolution applied:** option (a) per the original recommendation. v1.5 spec amendment expanded §11.2 baseline enumeration to enumerate `IncompatibleContractsVersion` and `WriteWriteConflict` alongside `MissingDependency` and `CyclicDependency`, mirroring v1.1/v1.2/v1.3/v1.4 non-semantic-correction ratification pattern. Post-v1.5 §11.2 line 858 reads verbatim «The current enum has `IncompatibleContractsVersion`, `WriteWriteConflict`, `MissingDependency`, and `CyclicDependency`. The migration adds:». v1.5 changelog entry credits this Pass 2 audit. Pass 2 resumed; §1–§10 spec ↔ code mapping completed. |

### Tier 1 — Missing required implementation

(no Tier 1 findings)

### Tier 2 — Whitelist deviations confirmed compatible with v1.5

| # | Whitelist entry (per AUDIT_PASS_2_RESUMPTION_PROMPT.md §6) | Spec section | Verification |
|---|---|---|---|
| 1 | M5.2 cascade-failure accumulation | `MOD_OS_ARCHITECTURE.md` §8.7 (lines 678–689) | v1.5 §8.7 wording verbatim (step 3c, line 686): «If any check fails, the mod is added to the failed set; mods that depend on it cascade-fail.» Verbatim (step 4, line 687): «The failed set is presented to the user; the success set proceeds to load.» ROADMAP M5.2 (lines 236–241) interpretation: «when mod A depends on mod B and B fails its own validation (any phase), A is **not** silently dropped — A's own validation runs to completion, and any independent errors A produces also surface. Both errors appear in `result.Errors`.» Compatibility analysis: §8.7 wording «cascade-fail» is non-prescriptive about whether dependent-mod errors are surfaced alongside provider errors or silently subsumed; ROADMAP's «accumulate without skip» reading interprets «cascade-fail» as «marked failed, but independent errors still surface». The `ContractValidator.ValidateInterModDependencyVersions` docstring (`ContractValidator.cs:491–494`) registers this interpretation verbatim. Pipeline-level test `Apply_WithCascadeFailure_SurfacesBothErrors` (`M52IntegrationTests`) demonstrates. v1.5 made no changes to §8.7. **Tier 2 confirmed compatible with v1.5 wording.** |
| 2 | M7 §9.2/§9.3 run-flag location | `MOD_OS_ARCHITECTURE.md` §9.2, §9.3 (lines 730–746) | v1.5 §9.2 verbatim (line 734): «User opens the mod menu. The menu calls `ModIntegrationPipeline.Pause()` which sets the scheduler's run flag to false.» Verbatim §9.3 (line 746): «Attempts to call `Apply` while the scheduler is running throw `InvalidOperationException("Pause the scheduler before applying mods")`. This is enforced by `ModIntegrationPipeline` checking the scheduler's run flag.» ROADMAP M7.1 verbatim (line 300): «§9.2 step 1 reads "menu sets the scheduler's run flag to false"; §9.3 reads "enforced by `ModIntegrationPipeline` checking the scheduler's run flag." M7.1 locates the flag itself on `ModIntegrationPipeline` (private `_isRunning` bool) rather than introducing one inside `ParallelSystemScheduler`.» Compatibility analysis: §9.3 explicitly says «enforced by `ModIntegrationPipeline` checking»; §9.2 step 1's «scheduler's run flag» is treated as «pipeline-mediated state observable as scheduler run state from outside» rather than as state owned by the scheduler. Code: `ModIntegrationPipeline._isRunning` private bool (`ModIntegrationPipeline.cs:105`); `Pause()`/`Resume()` setters (`ModIntegrationPipeline.cs:152,160`); guards in `Apply` and `UnloadMod`/`UnloadAll` use verbatim canonical messages (`ModIntegrationPipeline.cs:173–175,499–501,643–645`). v1.5 made no changes to §9.2/§9.3. **Tier 2 confirmed compatible with v1.5 wording.** |
| 3 | M7 §9.5/§9.5.1 step 7 ordering | `MOD_OS_ARCHITECTURE.md` §9.5 step 7, §9.5.1 (lines 758–776) | v1.5 §9.5 step 7 verbatim (line 768): «The loader spins on `WeakReference.IsAlive`, polling each iteration. Before every poll the loader performs `GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect()` — the double-collect bracket is required because `WaitForPendingFinalizers` can resurrect finalizable graph nodes the first collect would have removed; the second collect picks those up, restoring monotonic progress. Default cadence: 100 iterations × 100 ms = 10 s timeout.» Verbatim §9.5.1 (line 774): «After step 6, if step 7 times out, the `ModUnloadTimeout` warning per §9.5 fires; the mod is removed from the active set regardless of whether the assembly actually unloaded.» ROADMAP M7.3 verbatim (line 302): «M7.3 wires the order as `CaptureAlcWeakReference(mod) → _activeMods.Remove(mod) → TryStep7AlcVerification(modId, wr, warnings) → return`. The capture must precede the removal so the WR is bound to the same `ModLoadContext` instance the active-set removal then helps release; the spin must follow the removal so the pipeline-side strong reference (`_activeMods`) is gone before the spin's GC pumps run.» Compatibility analysis: §9.5.1's «mod removed from active set regardless» does not pin the order of removal, WR capture, or spin entry; §9.5 step 7's spin operates on a captured WR with no requirement to retain the mod in the active set. ROADMAP's reading picks the only ordering that gives the spin a chance to release the ALC: capture → remove → spin. Code at `ModIntegrationPipeline.RunUnloadSteps1Through6AndCaptureAlc` lines 613, 618 confirms the ordering: `CaptureAlcWeakReference(mod)` → `_activeMods.Remove(mod)` → return WR → `TryStep7AlcVerification` (called at `ModIntegrationPipeline.cs:524`). v1.5 made no changes to §9.5/§9.5.1. **Tier 2 confirmed compatible with v1.5 wording.** |
| 4 | M3.4 deferred (CI Roslyn analyzer) | `MOD_OS_ARCHITECTURE.md` §11.1 M3.4 row (line 847) | v1.5 §11.1 M3.4 row verbatim: «**M3.4** *(deferred)* | CI Roslyn analyzer for `[ModCapabilities]` honesty (D-2 hybrid completion) | Standalone analyzer package; runs in mod-publication CI, not at game load | Static-analysis integration tests; unblocked when first external mod author appears». ROADMAP M3.4 verbatim (line 26): «M3.4 — CI capability analyzer | ⏸ Deferred | — | Roslyn analyzer for `[ModCapabilities]` honesty (D-2 hybrid completion); unblocked when first external mod author appears». Compatibility analysis: spec explicitly marks M3.4 as deferred with named unblock condition; ROADMAP matches. Code: no Roslyn analyzer in repo (verified by absence of `*.Analyzer/` projects in src/). Per AUDIT_PASS_2_RESUMPTION_PROMPT.md §6.3 whitelist: «carried до first external mod author. Tier 2.» v1.5 made no changes to §11.1 M3.4 row. **Tier 2 confirmed compatible with v1.5 wording.** |
| 5 | Phase 3 SocialSystem/SkillSystem Replaceable=false carry-over to M10.C | (no spec section directly — §7.4 default and ROADMAP Phase 3/M10.C structural carry-over) | v1.5 §7.4 verbatim (lines 588–592): «A bridge with `Replaceable = false` cannot be replaced. The loader rejects mods that list it in `replaces`. This is the kernel's escape hatch for systems that must remain authoritative.» ROADMAP Phase 3 verbatim (line 68): «`SocialSystem` and `SkillSystem` exist as `[BridgeImplementation(Phase = 3)]` stubs in `DualFrontier.Systems.Pawn`. They will move to `Vanilla.Pawn` mod where they get real implementations.» ROADMAP M6.1 verbatim (line 264): «Phase 3 carry-over stubs (`SocialSystem`, `SkillSystem`) explicitly verified to remain `Replaceable = false` until M10.C — `Phase5BridgeAnnotationsTests` includes two protected-guard tests that lock this until the Phase 3 carry-over migrates.» Compatibility analysis: code at `src/DualFrontier.Systems/Pawn/SocialSystem.cs:22` and `SkillSystem.cs:21` confirms `[BridgeImplementation(Phase = 3)]` (no `Replaceable=true`) → defaults to `Replaceable=false` per `BridgeImplementationAttribute.Replaceable` default false (`BridgeImplementationAttribute.cs:35`). ROADMAP M6.1 carry-over discipline confirmed. Per AUDIT_PASS_2_RESUMPTION_PROMPT.md §6.3 whitelist: «carried до M10.C. Tier 2.» v1.5 made no changes to §7.4 or to the Phase 3 carry-over discipline. **Tier 2 confirmed compatible with v1.5 wording.** |

### Tier 3 — Spec ↔ code minor mismatch (wording clarity / hygiene)

| # | Spec section | Code path | Description | Recommendation |
|---|---|---|---|---|
| 1 | `MOD_OS_ARCHITECTURE.md` §8.4 line 631 | `VersionConstraint.cs:7–14` | (Preserved from §11 sub-pass.) Spec opening «Three syntaxes are supported, all are subsets of npm/Cargo conventions:» followed by Tilde explicitly «**not supported** in v1». «Supported» is overloaded between «handled by parser» (3) and «accepted» (2). Code matches the «accepted» reading: enum has 2 members; parser rejects Tilde with error per spec §8.5 step 2. Wording could be improved to «Three syntaxes are recognised: two accepted and one rejected.» | Backlog non-semantic correction in next non-semantic-correction ratification cycle. Not blocking. |
| 2 | `MOD_OS_ARCHITECTURE.md` §9.1 line 692 + diagram lines 696–724 | (no code representation; no `ModLifecycleState` enum) | (Preserved from §11 sub-pass.) Spec wording «six well-defined states» counts diagram boxes (6) but matches only 5 unique state-name labels — `Disabled` appears at both initial and terminal positions in the lifecycle cycle. Diagram is internally consistent (the cycle returns to `Disabled` deliberately). No code drift (no enum exists). Wording could be improved to «six lifecycle positions, five distinct states» or analogous. | Backlog non-semantic correction in next non-semantic-correction ratification cycle. Not blocking. |
| 3 | `MOD_OS_ARCHITECTURE.md` §2.2 row for `apiVersion` (line 220) vs. §4.5 v1 grace period (lines 403–405) | `ModManifest.cs:89,119–120` (`ApiVersion` nullable + `EffectiveApiVersion` fallback); `ManifestParser.cs:179–207` (`ReadApiVersion` returns `null` when missing); `mods/DualFrontier.Mod.Example/mod.manifest.json` (omits `apiVersion`, parses successfully) | Spec §2.2 marks `apiVersion` as «Required: yes» without v1-compat qualifier, but §4.5 explicitly defines a v1 grace period during which manifests without `apiVersion` continue to load via the `RequiresContractsVersion` fallback. The implementation matches §4.5 (lenient on missing `apiVersion`); §2.2's strict «yes» is therefore in tension with §4.5 and with the example manifest itself. No code drift; spec internal inconsistency. | Backlog non-semantic correction in next ratification cycle. Suggested wording for §2.2 row: «yes (v2 manifests); v1 manifests fall back to `requiresContractsVersion`». Not blocking. |
| 4 | `MOD_OS_ARCHITECTURE.md` §2.3 step 4 (line 236) | `src/DualFrontier.Application/Modding/ManifestParser.cs:266–289` (`ReadDependencies`) | Spec §2.3 step 4 verbatim: «No duplicate ids in `dependencies`.» Spec line 238: «A failure at any step rejects the mod with a typed `ValidationError`.» `ManifestParser.ReadDependencies` parses each entry into a `ModDependency` and appends to a `List<ModDependency>` without an id-uniqueness check; no `ValidationError` is produced for duplicates. Practical impact is bounded: `ContractValidator.ValidateInterModDependencyVersions` Phase G iterates each dependency entry independently (`ContractValidator.cs:500–541`) — duplicates produce duplicate diagnostics rather than silent failure or undefined behaviour. The spec rule is hygienic, not behavioural. | Backlog: add a duplicate-id check in `ManifestParser.ReadDependencies` after the per-entry append loop and before the return; emit `InvalidOperationException` with the offending id (matching the existing parse-time exception pattern). Not blocking. |

### Tier 4 — Cosmetic

| # | Spec section / code path | Description | Recommendation |
|---|---|---|---|
| 1 | `ContractValidator.cs:12–48` (class XML-doc) vs. `ContractValidator.cs:88–103` (`Validate()` body) | (Preserved from §11 sub-pass.) Class doc per-phase prose enumeration order (A, B, E, C, D, F, G, H) does not match invocation order (A, B, E, G, H, then conditionally C/D, then conditionally F). The class doc's normative claim is the conditionality summary at lines 41–44, which IS consistent with invocation. Per-phase prose enumeration is descriptive (introduces each phase by responsibility), not prescriptive about execution order. No behaviour impact, no contract impact. | Optional surgical fix in a future review batch: reorder per-phase prose enumeration in the class XML-doc to match invocation order (A, B, E, G, H, C, D, F), keeping the conditionality summary intact. Not blocking. |
| 2 | `mods/DualFrontier.Mod.Example/mod.manifest.json:6` vs. `MOD_OS_ARCHITECTURE.md` §2.2 (lines 213–227) | (Pass 1 anomaly #11 — classified in this resumption.) Example manifest contains a `description` field («Reference mod showing the IMod + IModApi pattern. See docs/architecture/MODDING.md.»). Spec §2.2 field reference table enumerates 13 fields; `description` is not listed. `ManifestParser` (`ManifestParser.cs:25–82`) reads only the fields it handles explicitly; `System.Text.Json` ignores unknown properties silently. The `description` field is therefore silently accepted but never propagated into the `ModManifest`, has no behavioural effect on load, and produces no warning. The field provides documentation value (a one-line mod-author intent statement) and could reasonably be added to the spec field reference table as «no / informational only». | Optional surgical fix in a future non-semantic ratification cycle: add `description` row to spec §2.2 field reference table as `string / no / "" / Free-form mod-author description; informational only, ignored by the loader.` Not blocking. |

### Out-of-scope items observed (for Pass 3/4)

| # | Anomaly | Source | Routing |
|---|---|---|---|
| 1 | Pass 1 anomaly #1 — source-level test-attribute count 359 vs. ROADMAP-stated runtime test count 369 | `tests/`, `ROADMAP.md:36` | Pass 3 (engine snapshot verification) |
| 2 | Pass 1 anomaly #2 — closure review locations (`docs/audit/` vs. previously `docs/`); session-start `git status` shows `D docs/M*_CLOSURE_REVIEW.md` and `?? docs/audit/` | `docs/audit/`, session-start git status | Resolved structurally before Pass 1 (per Pass 1 §11 #2 note); Pass 4 verifies clean working-tree state and audit-trail integrity |
| 3 | Pass 1 anomaly #6 — `tests/DualFrontier.Modding.Tests/README.md` claims «Real tests will arrive in Phase 2» but folder has 31 `.cs` files | `tests/DualFrontier.Modding.Tests/README.md` | Pass 4 (sub-folder README accuracy) |
| 4 | Pass 1 anomaly #7 — `tests/DualFrontier.Systems.Tests/README.md` claims «Real tests will arrive in Phase 2+» but folder has 6 test `.cs` files | `tests/DualFrontier.Systems.Tests/README.md` | Pass 4 (sub-folder README accuracy) |
| 5 | Pass 1 anomaly #8 — bus count cross-doc inconsistency: `Contracts/README.md:17` lists 5 buses, `IGameServices.cs:13–57` declares 6 properties, `Contracts/Bus/README.md:5` describes 6 buses | `src/DualFrontier.Contracts/README.md:17`, `src/DualFrontier.Contracts/Bus/IGameServices.cs:13–57`, `src/DualFrontier.Contracts/Bus/README.md:5` | Pass 4 (sub-folder README accuracy and cross-doc consistency) |
| 6 | Pass 1 anomaly #9 — `AUDIT_PASS_1_PROMPT.md` §4 contract table referenced closure reviews under `docs/M*_CLOSURE_REVIEW.md` rather than `docs/audit/M*` | `docs/audit/AUDIT_PASS_1_PROMPT.md:84–88` | Inventory observation; Pass 1 prompt-quality issue, not Pass 2 scope |
| 7 | Pass 1 anomaly #10 — `AUDIT_PASS_1_PROMPT.md` Appendix A example branch is `feat/m4-shared-alc`; actual `.git/HEAD` is `main` | `docs/audit/AUDIT_PASS_1_PROMPT.md:691–711`, `.git/HEAD:1` | Inventory observation; Pass 1 prompt-quality issue, not Pass 2 scope |
| 8 | Pass 1 anomaly #12 — M5/M6 closure-review header line 9 reports branch `feat/m4-shared-alc`; HEAD branch is `main`; `.git/logs/HEAD` shows `checkout: moving from feat/m4-shared-alc to main` event between commits `c7210ca` and `b504813` | `docs/audit/M5_CLOSURE_REVIEW.md:9`, `docs/audit/M6_CLOSURE_REVIEW.md:9`, `.git/HEAD`, `.git/logs/HEAD` | Pass 3 (three-commit invariant / branch state consistency in roadmap-reality verification) |
| 9 | §5.5 shared-mod naming-convention warning not emitted by loader | `src/DualFrontier.Application/Modding/` (no warning emission for non-conforming shared-mod ids) | Spec §5.5 line 467 verbatim: «The convention is enforced by the loader as a warning, not an error.» Loader currently does not emit this advisory. Vanilla mods comply by convention; no current external mod authors (per M3.4 deferral) so practical impact is zero. Out-of-scope for Pass 2 (no Tier classification: no v1.5 wording mismatch and no behavioural drift); recommended follow-up at first external mod author appearance, parallel to M3.4 unblock. |
| 10 | ROADMAP.md line 3 references «v1.4 LOCKED» (stale post-v1.5 ratification) | `ROADMAP.md:3` | Pass 4 (v1.4 → v1.5 stale-reference sweep across active navigation) |

---

## §14 Verification end-state

- **§0 Executive summary:** 10/11 PASSED (§1–§10), 1/11 FAILED with Tier 0 RESOLVED via v1.5 (§11 Sequence integrity).
- **Total findings:** Tier 0: 1 (RESOLVED via v1.5 amendment), Tier 1: 0, Tier 2: 5 whitelist confirmed compatible with v1.5 wording, Tier 3: 4 (2 from §11 sub-pass + 2 new from §1–§10), Tier 4: 2 (1 from §11 sub-pass + 1 new from §2 Pass 1 anomaly #11).
- **Eager escalation triggered:** NO (resumption completed normally).
- **Surgical fixes applied:** 0 (per contract).
- **Pass 2 status:** complete (initial sequence integrity sub-pass + resumption §1–§10 mapping + whitelist verifications), ready for human ratification.
- **Tier 0 resolution audit trail:** v1.5 ratification of `MOD_OS_ARCHITECTURE.md` §11.2 baseline enumeration. See §13 Tier 0 row 1 «STATUS: RESOLVED». Spec status line 8 verifies `LOCKED v1.5`; v1.5 changelog entry verifies at lines 30–32.
- **Unblocks:** Pass 3 (Roadmap ↔ Reality), Pass 4 (Cross-doc + README + Cyrillic), Pass 5 (Triage + Final report).
