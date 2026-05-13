---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-M3_CLOSURE_REVIEW
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-M3_CLOSURE_REVIEW
---
---
title: M3 closure verification report
nav_order: 95
---

# M3 — Capability model closure verification report

**Date:** 2026-04-29
**Branch:** `main` (commits `a73669f..95935d7`, six commits inclusive)
**Scope:** Verification only. No new architectural work. Surgical fixes
applied only for typos, broken cross-references, or clearly-wrong facts in
the new documents. Any structural finding is recorded in §10 as a follow-up
item rather than remediated in this session.

---

## §0 Executive summary

| # | Check | Status | Notes |
|---|---|---|---|
| 1 | Build & test integrity | **PASSED** | `dotnet build`: 0 warnings, 0 errors. `dotnet test` at HEAD: 260/260 (Persistence 4, Systems 7, Modding 189, Core 60). Three-commit invariant verified at every commit in the M3 closure batch. |
| 2 | Spec ↔ code ↔ test triple consistency | **PASSED** | All 8 tokens from `MOD_OS_ARCHITECTURE` §2.1 example manifest resolve through `KernelCapabilityRegistry.BuildFromKernelAssemblies`; all 4 negative assertions (write tokens on read-only components) hold. |
| 3 | Cross-document consistency | **PASSED** | `MOD_OS_ARCHITECTURE` v1.2 LOCKED, `ROADMAP` header `Updated: 2026-04-29` with `260/260`, `docs/README` v1.2 LOCKED — three documents in coherent state. |
| 4 | Stale-reference sweep | **PASSED** | All five forbidden patterns return zero hits in `docs/`. `v1.0 LOCKED` survives only as the legitimate historical-attribution of M0 in `ROADMAP`. |
| 5 | Methodology compliance | **PASSED** | All 6 commits have scope prefixes per METHODOLOGY §7.3. v1.2 changelog matches the v1.1 four-rule pattern. The §12 LOCKED decisions D-1 through D-7 are byte-identical between v1.1 and v1.2 — verified by diff. |
| 6 | Sub-phase acceptance criteria coverage | **PASSED** | Every acceptance bullet for M3.1, M3.2, M3.3 maps to an identifiable artifact (commit, file:line, test name). |
| 7 | Carried debts forward | **PASSED** | Phase 2 WeakReference unload tests are tracked in M7; Phase 3 `SocialSystem`/`SkillSystem` stubs are tracked in M10.C; M3.4 (CI Roslyn analyzer) is recorded in `ROADMAP` as `⏸ Deferred` with explicit rationale. |
| 8 | Ready-for-M4 readiness | **PASSED** | `KernelCapabilityRegistry` constructor accepts arbitrary `IEnumerable<Assembly>`; `Capabilities` and `Provides` are exposed for shared-mod usage; `ContractValidator` Phase C correctly delegates to `dependency.Manifest.Capabilities.ProvidesCapability` for non-kernel tokens. No surface-level blocker for M4. |

**Result:** All 8 checks PASSED. Zero findings. Zero surgical fixes
applied. M3 phase closes cleanly; M4 (Shared ALC) is unblocked.

---

## §1 Build & test integrity

**`dotnet build DualFrontier.sln`** at HEAD (`95935d7`):

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:00:10.15
```

**`dotnet test DualFrontier.sln`** at HEAD:

| Project | Pass | Skip | Total |
|---|---|---|---|
| `DualFrontier.Persistence.Tests` | 4 | 0 | 4 |
| `DualFrontier.Systems.Tests` | 7 | 0 | 7 |
| `DualFrontier.Modding.Tests` | 189 | 0 | 189 |
| `DualFrontier.Core.Tests` | 60 | 0 | 60 |
| **Total** | **260** | **0** | **260** |

### Three-commit invariant (METHODOLOGY §7.3)

Each of the six commits in the M3 closure batch was checked out
independently against a clean working tree; `dotnet build` ran 0 W / 0 E
at every checkout, and `dotnet test` exit code was zero at every
checkout. Per-commit test counts:

| Commit | Subject | Build | Persistence | Systems | Modding | Core | Total |
|---|---|---|---|---|---|---|---|
| `a73669f` | fix(application/modding): scan Components and Events assemblies | 0 W / 0 E | 4 | 7 | 176 | 60 | **247** |
| `f91f065` | feat(components): apply [ModAccessible] for M3 closure | 0 W / 0 E | 4 | 7 | 176 | 60 | **247** |
| `b92fa66` | test(modding): assert §2.1 example manifest tokens | 0 W / 0 E | 4 | 7 | 189 | 60 | **260** |
| `7e44eb2` | docs(modding): ratify MOD_OS_ARCHITECTURE v1.2 | 0 W / 0 E | 4 | 7 | 189 | 60 | **260** |
| `89bbea3` | docs(roadmap): close M3 — sync with v1.2 | 0 W / 0 E | 4 | 7 | 189 | 60 | **260** |
| `95935d7` | docs: update MOD_OS_ARCHITECTURE version reference | 0 W / 0 E | 4 | 7 | 189 | 60 | **260** |

The 247 → 260 jump at `b92fa66` matches the +13 delta declared in that
commit's body (8 positive `[Theory]` rows + 4 negative `[Theory]` rows + 1
non-empty registry `[Fact]`). At `a73669f` and `f91f065` the
`KernelCapabilityRegistry` returns a non-empty set (events scan picks up
production events) but no production-component `kernel.read:*` /
`kernel.write:*` tokens exist yet at `a73669f`, and the §2.1 token tests
do not exist until `b92fa66` — so the invariant is preserved by the
absence of dependent assertions, not by accident.

**Verdict:** PASSED. The three-commit invariant from METHODOLOGY §7.3
holds across the entire batch. No commit ships a broken build or a
failing test.

---

## §2 Spec ↔ code ↔ test triple consistency

`MOD_OS_ARCHITECTURE` §2.1 example manifest declares **8 capability
tokens** in `capabilities.required` (lines 184–193 after the v1.2
expansion). Each token is verified through three legs:

| # | Token | Spec leg | Code leg | Test leg |
|---|---|---|---|---|
| 1 | `kernel.publish:DualFrontier.Events.Combat.DamageEvent` | §2.1 line 185 | [DamageEvent.cs:10](../src/DualFrontier.Events/Combat/DamageEvent.cs:10) `public sealed record DamageEvent : IEvent` | [ProductionComponentCapabilityTests.cs:18](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:18) |
| 2 | `kernel.publish:DualFrontier.Events.Combat.DeathEvent` | §2.1 line 186 | [DeathEvent.cs:12](../src/DualFrontier.Events/Combat/DeathEvent.cs:12) `public sealed record DeathEvent : IEvent` | [ProductionComponentCapabilityTests.cs:19](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:19) |
| 3 | `kernel.subscribe:DualFrontier.Events.Combat.ShootGranted` | §2.1 line 187 | [ShootGranted.cs:16](../src/DualFrontier.Events/Combat/ShootGranted.cs:16) `public sealed record ShootGranted(...) : IEvent` | [ProductionComponentCapabilityTests.cs:20](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:20) |
| 4 | `kernel.read:DualFrontier.Components.Combat.WeaponComponent` | §2.1 line 188 | [WeaponComponent.cs:11–12](../src/DualFrontier.Components/Combat/WeaponComponent.cs:11) `[ModAccessible(Read = true)] public sealed class WeaponComponent : IComponent` | [ProductionComponentCapabilityTests.cs:21](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:21) |
| 5 | `kernel.read:DualFrontier.Components.Combat.ArmorComponent` | §2.1 line 189 | [ArmorComponent.cs:12–13](../src/DualFrontier.Components/Combat/ArmorComponent.cs:12) `[ModAccessible(Read = true)] public sealed class ArmorComponent : IComponent` | [ProductionComponentCapabilityTests.cs:22](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:22) |
| 6 | `kernel.read:DualFrontier.Components.Combat.AmmoComponent` | §2.1 line 190 | [AmmoComponent.cs:11–12](../src/DualFrontier.Components/Combat/AmmoComponent.cs:11) `[ModAccessible(Read = true)] public sealed class AmmoComponent : IComponent` | [ProductionComponentCapabilityTests.cs:23](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:23) |
| 7 | `kernel.read:DualFrontier.Components.Combat.ShieldComponent` | §2.1 line 191 | [ShieldComponent.cs:12–13](../src/DualFrontier.Components/Combat/ShieldComponent.cs:12) `[ModAccessible(Read = true)] public sealed class ShieldComponent : IComponent` | [ProductionComponentCapabilityTests.cs:24](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:24) |
| 8 | `kernel.write:DualFrontier.Components.Shared.HealthComponent` | §2.1 line 192 | [HealthComponent.cs:10–11](../src/DualFrontier.Components/Shared/HealthComponent.cs:10) `[ModAccessible(Read = true, Write = true)] public sealed class HealthComponent : IComponent` | [ProductionComponentCapabilityTests.cs:25](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:25) |

The eight positive assertions are encoded as `[Theory]` rows on
`Section21ExampleManifest_TokensResolveInKernelRegistry` and run against
the production `KernelCapabilityRegistry.BuildFromKernelAssemblies` path
([KernelCapabilityRegistry.cs:80–87](../src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs:80)) — i.e.
the same code path the production loader exercises. No mock, no
test-assembly substitute.

### Negative assertions

The four read-only Combat components must **not** produce a
`kernel.write:*` token (regression guard against a refactor that conflates
the `Read`/`Write` flags or accidentally promotes a read-only component
to read+write):

| Token | Expected | Test |
|---|---|---|
| `kernel.write:DualFrontier.Components.Combat.WeaponComponent` | absent | [ProductionComponentCapabilityTests.cs:33](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:33) |
| `kernel.write:DualFrontier.Components.Combat.ArmorComponent` | absent | [ProductionComponentCapabilityTests.cs:34](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:34) |
| `kernel.write:DualFrontier.Components.Combat.AmmoComponent` | absent | [ProductionComponentCapabilityTests.cs:35](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:35) |
| `kernel.write:DualFrontier.Components.Combat.ShieldComponent` | absent | [ProductionComponentCapabilityTests.cs:36](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:36) |

All four assertions hold. `HealthComponent` legitimately produces both
`kernel.read:*` and `kernel.write:*` tokens because its annotation is
`Read = true, Write = true`; the §2.1 example only references the write
token, but the registry correctly emits both.

### Non-empty registry guard

`BuildFromKernelAssemblies_ProducesNonEmptyCapabilitySet`
([ProductionComponentCapabilityTests.cs:46–56](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:46))
catches the entire bug class addressed by `a73669f`: pre-fix, both
`typeof(IEvent).Assembly` and `typeof(IComponent).Assembly` resolved to
`DualFrontier.Contracts` (where the marker interfaces live, but no
concrete types do), so the registry returned an empty set.
`BuildFromKernelAssemblies` now scans
`typeof(HealthComponent).Assembly` and `typeof(PawnSpawnedEvent).Assembly`
as stable markers for `DualFrontier.Components` and `DualFrontier.Events`
respectively — and the constructor's existing `HashSet<Assembly>` dedup
keeps the marker-collision harmless.

**Verdict:** PASSED. All 8 spec tokens have all 3 legs present and
verified. All 4 negative assertions hold. The non-empty registry guard
catches the regression class fixed in `a73669f`.

---

## §3 Cross-document consistency

Three documents must agree on the v1.2 / M3-closed / 260-tests state:

| Document | Field | Expected | Found | Status |
|---|---|---|---|---|
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | Status line (line 8) | `LOCKED v1.2` | `LOCKED v1.2 — Phase 0 closed; non-semantic corrections from M1–M3.1 audit (v1.1) and M3 closure review (v1.2) applied.` | ✓ |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | Version history (lines 18–22) | v1.2 entry with §3.6, §3.5+§2.1, §11.1, "no semantic changes" | All four bullets present and well-formed | ✓ |
| `docs/ROADMAP.md` | Header date (line 11) | `2026-04-29` | `*Updated: 2026-04-29 (M3 closed — M3.1, M3.2, M3.3 done; M3.4 deferred; M4 next).*` | ✓ |
| `docs/ROADMAP.md` | M3 row (line 25) | `✅ Closed` | `✅ Closed` with M3.1/M3.2/M3.3 acceptance bullets and v1.2 §3.6 reference | ✓ |
| `docs/ROADMAP.md` | M3.4 row (line 26) | `⏸ Deferred` | `⏸ Deferred` with explicit rationale (D-2 hybrid completion; first external mod author trigger) | ✓ |
| `docs/ROADMAP.md` | Engine snapshot (line 36) | 260/260 | `Total at M3 closure: 260/260 passed` | ✓ |
| `docs/ROADMAP.md` | v1.2 LOCKED references | header + see-also | line 3 `v1.2 LOCKED`; line 415 `v1.2 LOCKED specification driving M1–M10` | ✓ |
| `docs/README.md` | Architecture list entry (line 28) | `v1.2 LOCKED` | `**v1.2 LOCKED.** Mod system as a small operating system…` | ✓ |

The `v1.0 LOCKED` references at `ROADMAP.md:22` (M0 row Output column) and
`ROADMAP.md:109` (M0 phase Output prose) describe the artifact M0
**closed** — historically accurate, since v1.1 and v1.2 are non-semantic
ratifications that did not reopen M0. They do not function as navigation
references and therefore do not collide with the rule that
"navigation references must point to v1.2."

**Verdict:** PASSED. No version, date, status, or test-count drift among
the three primary documents.

---

## §4 Stale-reference sweep

Patterns checked across `docs/`:

| Pattern | Hits | Disposition |
|---|---|---|
| `/RegisterSystem capability check` (semantics) | 0 | Clean — `RegisterSystem` itself is mentioned legitimately as the v1 method (e.g. `MODDING.md:38`, `MOD_OS_ARCHITECTURE.md:72,342`); no doc claims the loader runs a capability check on it. The single `RegisterSystem` mention in `ROADMAP.md:175` is the explicit v1.2 §4.1 attestation that no capability check applies. |
| `247` (test count anywhere) | 0 | Clean — every active document either says `260/260` or is silent on the count. |
| `M3 in progress` | 0 | Clean. |
| `M3.2 current` | 0 | Clean. |
| `M3.3 pending` | 0 | Clean. |
| `🔨 Current` | 0 | Clean. The 🔨 glyph survives only in the `ROADMAP.md:101` section heading `## 🔨 Mod-OS Migration (M0–M10)` — a category label, not a status marker. |
| `v1.0 LOCKED` (active navigation) | 2 | Both at `ROADMAP.md:22` (M0 row Output column) and `ROADMAP.md:109` (M0 prose) — historical-attribution of what M0 closed; not navigation references. |
| `v1.1 LOCKED` | 0 | Clean. |
| `2026-04-27` (active "last updated") | 7 | All in commit-bound contexts: `PASS_2_NOTES.md:118` (Pass 2 closure date), `PASS_3_NOTES.md:6` (Pass 3 closure date), `PASS_4_REPORT.md:8` (Pass 4 closure date), `PIPELINE_METRICS.md:74,134` (M0–M3 evening session date), `learning/PHASE_1.md:4` (translation date attribution). None function as an active "last updated" marker. |

**Verdict:** PASSED. No active document carries a stale status, an
incorrect test count, or a stale version pointer. The handful of
historical-context occurrences are by design.

---

## §5 Methodology compliance

### §5.1 Commit scope prefixes (METHODOLOGY §7.3)

| Commit | Subject | Prefix | Scope | Verdict |
|---|---|---|---|---|
| `a73669f` | fix(application/modding): scan Components and Events assemblies in BuildFromKernelAssemblies | `fix` | `application/modding` | ✓ |
| `f91f065` | feat(components): apply [ModAccessible] for M3 closure (D-1, MOD_OS_ARCHITECTURE §3.5 v1.2) | `feat` | `components` | ✓ |
| `b92fa66` | test(modding): assert §2.1 example manifest tokens resolve in kernel registry | `test` | `modding` | ✓ |
| `7e44eb2` | docs(modding): ratify MOD_OS_ARCHITECTURE v1.2 | `docs` | `modding` | ✓ |
| `89bbea3` | docs(roadmap): close M3 — sync with MOD_OS_ARCHITECTURE v1.2 | `docs` | `roadmap` | ✓ |
| `95935d7` | docs: update MOD_OS_ARCHITECTURE version reference in docs index (v1.0 → v1.2) | `docs` | (parenthetical scope omitted; permitted by METHODOLOGY §7.3 which lists `docs:` as an acceptable form) | ✓ |

All 6 commits also carry substantive bodies — explanation, rationale,
and (where applicable) commit-cross-reference. None ship a one-line
subject without a body.

### §5.2 v1.2 ratification pattern parity

The v1.2 changelog at `MOD_OS_ARCHITECTURE.md:18–22` follows the same
four-rule shape as v1.1 (`MOD_OS_ARCHITECTURE.md:14–17`):

1. Each changed section identified by §-anchor (v1.1: §4.1, §2.2; v1.2:
   §3.6, §3.5+§2.1, §11.1).
2. Cause described in implementation-vs-spec terms ("brings spec in line
   with implementation" / "non-semantic correction").
3. Explicit "no semantic changes" clause.
4. Explicit "no locked decision is altered" clause — extended in v1.2 to
   "no locked decision (D-1 through D-7) is altered" for clarity.

### §5.3 LOCKED decision sanctity

`git diff e37ca25..7e44eb2 -- docs/architecture/MOD_OS_ARCHITECTURE.md` (the v1.1
ratification through the v1.2 ratification) contains no edits to any of
the seven D-N declarations (§12). The diff is confined to:

- Status line (v1.1 → v1.2 with extended explanation).
- Version history (v1.1 entry stripped of "(this version)"; v1.2 entry
  added with 4 bullets).
- §2.1 example manifest (added `kernel.read:AmmoComponent` and
  `kernel.read:ShieldComponent`).
- §3.6 rewrite (single-layer "Runtime enforcement" → two-layer "Hybrid
  enforcement — load-time + runtime").
- §3.7 / §3.8 narrative updates referring to D-2 (these reference
  D-2 but do not alter the D-2 declaration text in §12).
- §11.1 migration table (M3 row updated with runtime check + new
  artifacts; M3.4 row added).

The §12 declaration text for D-1, D-2, D-3, D-4, D-5, D-6, D-7 is
byte-identical between v1.1 and v1.2.

**Verdict:** PASSED. Commit-prefix discipline holds, the v1.2 ratification
pattern matches v1.1, and all seven LOCKED decisions are untouched.

---

## §6 Sub-phase acceptance criteria coverage

`ROADMAP.md` M3 section (lines 168–197) declares M3.1, M3.2, M3.3 closed
with explicit acceptance bullets. Each bullet maps to an identifiable
artifact:

### M3.1 — Kernel capability registry + opt-in

| Acceptance bullet | Artifact |
|---|---|
| `KernelCapabilityRegistry` scans `DualFrontier.Components` + `DualFrontier.Events` assemblies | Commit `a73669f`. [KernelCapabilityRegistry.cs:80–87](../src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs:80) `BuildFromKernelAssemblies` enumerates the four marker assemblies (with `IEvent` and `IComponent` deduped to `DualFrontier.Contracts`). [KernelCapabilityRegistry.cs:89–119](../src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs:89) `ScanAssembly` walks public, concrete, non-generic, non-nested types and emits `kernel.publish:` / `kernel.subscribe:` for `IEvent` implementers and `kernel.read:` / `kernel.write:` for `IComponent` implementers carrying `[ModAccessible]`. |
| Production components annotated `[ModAccessible]` per D-1 | Commit `f91f065`. Five files: [WeaponComponent.cs:11](../src/DualFrontier.Components/Combat/WeaponComponent.cs:11), [ArmorComponent.cs:12](../src/DualFrontier.Components/Combat/ArmorComponent.cs:12), [AmmoComponent.cs:11](../src/DualFrontier.Components/Combat/AmmoComponent.cs:11), [ShieldComponent.cs:12](../src/DualFrontier.Components/Combat/ShieldComponent.cs:12) — all `Read = true`. [HealthComponent.cs:10](../src/DualFrontier.Components/Shared/HealthComponent.cs:10) — `Read = true, Write = true`. |
| Tests verify registry mechanics and §2.1 example manifest token resolution | [KernelCapabilityRegistryTests.cs](../tests/DualFrontier.Modding.Tests/Capability/KernelCapabilityRegistryTests.cs) (10 facts covering event scanning, component scanning, generic/abstract skip, dedup, identity invariants); [ProductionComponentCapabilityTests.cs](../tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs) (8 positive + 4 negative `[Theory]` rows + 1 non-empty fact, commit `b92fa66`). |

### M3.2 — Capability-enforcing `RestrictedModApi`

| Acceptance bullet | Artifact |
|---|---|
| `Publish<T>` / `Subscribe<T>` runtime check raises `CapabilityViolationException` | [RestrictedModApi.cs:80–88](../src/DualFrontier.Application/Modding/RestrictedModApi.cs:80) `Publish<T>` calls `EnforceCapability("publish", typeof(T))` before bus dispatch; [RestrictedModApi.cs:91–112](../src/DualFrontier.Application/Modding/RestrictedModApi.cs:91) `Subscribe<T>` calls `EnforceCapability("subscribe", typeof(T))` before bus subscription; [RestrictedModApi.cs:147–164](../src/DualFrontier.Application/Modding/RestrictedModApi.cs:147) `EnforceCapability` throws `CapabilityViolationException` when `_manifest.Capabilities.RequiresCapability(token)` is false (skipping the check with a deprecation warning when `IsEmpty` — v1 manifest grace period per §4.5). |
| Hybrid enforcement per §4.2/§4.3 v1.2 ratified in §3.6 | `MOD_OS_ARCHITECTURE.md:283–305` v1.2 §3.6 documents the two layers; the runtime layer is implemented exactly as specified (hash-set lookup on `_manifest.Capabilities.RequiresCapability`, `O(1)`, "measured negligible on the hot path"). |
| Coverage tests | [RestrictedModApiV2Tests.cs](../tests/DualFrontier.Modding.Tests/Api/RestrictedModApiV2Tests.cs) — 22 facts including: capability hit/miss on `Publish`/`Subscribe` (#7, #8, #11, #12), v1 grace-period warning (#9), context push/pop wrapping (#19–22). |
| `RegisterSystem` v1 semantics preserved (no capability check) | [RestrictedModApi.cs:76–77](../src/DualFrontier.Application/Modding/RestrictedModApi.cs:76) `RegisterSystem<T>` delegates straight to `_registry.RegisterSystem` — no `EnforceCapability` call. Per `ROADMAP.md:175` and `MOD_OS_ARCHITECTURE.md` §4.1 LOCKED. |

### M3.3 — `ContractValidator` Phase C + Phase D

| Acceptance bullet | Artifact |
|---|---|
| Phase C — capability satisfiability via kernel + listed dependencies | [ContractValidator.cs:250–285](../src/DualFrontier.Application/Modding/ContractValidator.cs:250) `ValidateCapabilitySatisfiability`: for each mod, for each `Capabilities.Required` token, check `kernel.Provides(token)` first; otherwise iterate `Manifest.Dependencies` and require `provider.Manifest.Capabilities.ProvidesCapability(token)`. Implicit satisfaction (a loaded mod that provides the token but is not listed as a dependency) raises `MissingCapability`. |
| Phase D — `[ModCapabilities]` × manifest cross-check | [ContractValidator.cs:294–321](../src/DualFrontier.Application/Modding/ContractValidator.cs:294) `ValidateModCapabilitiesAttributes`: for each mod-system carrying `[ModCapabilities]`, every token must appear in the owning manifest's `Capabilities.Required`. Mismatch raises `MissingCapability` with system FQN, mod id, and the offending token. |
| Coverage tests | [CapabilityValidationTests.cs](../tests/DualFrontier.Modding.Tests/Capability/CapabilityValidationTests.cs) — Phase C: kernel-satisfied valid (#1), unsatisfied missing (#2), dependency-listed valid (#3), implicit-provider rejected (#4), null-registry skip (#5), empty-manifest valid (#6). Phase D: attribute-token-in-manifest valid, attribute-token-missing rejected, system-without-attribute valid, null-registry skip. Plus a self-check guarding the hard-coded const against `TestPublishEvent` rename. |

### M3.4 — CI Roslyn analyzer (deferred)

`ROADMAP.md:26` records M3.4 as `⏸ Deferred` with explicit rationale:
runtime `CapabilityViolationException` already catches dishonest
`[ModCapabilities]` declarations; the analyzer is developer-experience
tooling for early feedback before publication, not a runtime safety
boundary. Unblocks when the first external (non-vanilla) mod author
appears.

**Verdict:** PASSED. Every acceptance bullet for M3.1, M3.2, M3.3 maps to
an identifiable file:line and test. M3.4 carries an explicit deferral
rationale.

---

## §7 Carried debts forward

The M3 closure does not absorb earlier-phase debts; rather, it confirms
they are tracked forward to the milestones that need them.

| Debt | Origin | Forward target | Documentation |
|---|---|---|---|
| WeakReference unload tests | Phase 2 (closed at 11/11 isolation tests, with the unload tests on backlog) | M7 (Hot reload from menu) — hard requirement | `ROADMAP.md:60` ("Carried debt (now part of M7): AssemblyLoadContext WeakReference unload tests are not yet implemented…they become a hard requirement when M7 lands hot reload"); `ROADMAP.md:308–309` (M7 acceptance: "every regular mod under test passes the WeakReference unload check within 10 seconds; failures fail the test (no flakiness tolerated)"); also `MOD_OS_ARCHITECTURE.md:813` (§10.4 "WeakReference unload tests… now hard-required"). |
| `SocialSystem`, `SkillSystem` `[BridgeImplementation(Phase = 3)]` stubs | Phase 3 (closed at 1/1 integration test, with social/skill stubs on backlog) | M10.C (`Vanilla.Pawn` mod) | `ROADMAP.md:68` ("Carried debt (now part of M10): SocialSystem and SkillSystem exist as `[BridgeImplementation(Phase = 3)]` stubs in `DualFrontier.Systems.Pawn`. They will move to `Vanilla.Pawn` mod where they get real implementations"); `ROADMAP.md:387–388` (M10.C acceptance: "Consumes Phase 3 backlog: SocialSystem and SkillSystem get real implementations inside the mod"). |
| M3.4 — CI Roslyn analyzer (D-2 hybrid completion) | M3 closure (deferred sub-phase) | First external (non-vanilla) mod author | `ROADMAP.md:26` (deferred row with rationale); `ROADMAP.md:177` (M3.4 sub-phase status); `MOD_OS_ARCHITECTURE.md:21` (v1.2 §11.1 changelog entry); `MOD_OS_ARCHITECTURE.md:830` (M3.4 row in migration table with deferred unblock condition). |

**Verdict:** PASSED. Phase 2 and Phase 3 debts are tracked forward to
their absorbing M-phases, and M3.4 carries an explicit deferral with a
named unblock trigger.

---

## §8 Ready-for-M4 readiness

M4 is "Shared ALC + shared mod kind" (`ROADMAP.md:201–225`). Per
`MOD_OS_ARCHITECTURE` §11.1, M4 consumes D-4 (active scan reject contract
types in regular mods) and D-5 (forbid shared-mod cycles) — both still
LOCKED and untouched by M3.

The M3 surface that M4 will exercise:

| M4 dependency | M3 surface | Status |
|---|---|---|
| Loader must add shared-mod assemblies to the capability scan | [KernelCapabilityRegistry.cs:39–55](../src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs:39) constructor accepts arbitrary `IEnumerable<Assembly>` and dedupes via `HashSet<Assembly>`. Not hard-coded to two markers — `BuildFromKernelAssemblies` is one factory; M4 can introduce a `BuildFromKernelAndSharedAssemblies(IEnumerable<Assembly>)` factory or compose at the loader level. | ✓ ready |
| Capability resolver must accept shared-mod-provided tokens | [ContractValidator.cs:262–273](../src/DualFrontier.Application/Modding/ContractValidator.cs:262) Phase C iterates `mod.Manifest.Dependencies` and resolves each to a `LoadedMod`, then calls `provider.Manifest.Capabilities.ProvidesCapability(token)`. Shared mods loaded via M4 will appear in the mod set with their `Capabilities.Provided` populated; the resolver path is uniform across regular and shared mods. | ✓ ready |
| `Capabilities` property exposed for shared-mod self-introspection | [KernelCapabilityRegistry.cs:60](../src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs:60) `IReadOnlySet<string> Capabilities`; [KernelCapabilityRegistry.cs:66](../src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs:66) `bool Provides(string token)`. Both reachable through `IModApi.GetKernelCapabilities` ([RestrictedModApi.cs:126](../src/DualFrontier.Application/Modding/RestrictedModApi.cs:126)). | ✓ ready |
| Manifest `kind` field already exists | M1 (closed) added `Kind` to `ModManifest`; v2 schema is operational. | ✓ ready |
| No unintentional coupling between M3 capability code and ALC mechanics | `KernelCapabilityRegistry`, `RestrictedModApi.EnforceCapability`, `ContractValidator` Phases C/D all operate on `Type` / `Assembly` / `ManifestCapabilities` — none reference `AssemblyLoadContext` directly. | ✓ ready |

**Verdict:** PASSED. No M3 surface change blocks M4. The capability layer
is correctly layered above the ALC layer and remains agnostic to whether
a contributing assembly comes from a regular ALC, the (future) shared
ALC, or the kernel default ALC.

---

## §9 Surgical fixes applied

None.

---

## §10 Items requiring follow-up

None.

All eight checks PASSED with no findings, no notes, and no surgical
fixes. The M3 closure batch is consistent with itself, with the v1.2
specification, with the methodology, and with the M4 entry surface.

---

## Verification end-state

- **Build:** 0 warnings, 0 errors.
- **Tests:** 260/260 passing across all four test projects.
- **Three-commit invariant:** holds at every commit `a73669f..95935d7`.
- **Spec ↔ code ↔ test triple consistency:** 8/8 tokens, 4/4 negative
  assertions, 1/1 non-empty registry guard.
- **Cross-document consistency:** `MOD_OS_ARCHITECTURE` v1.2 LOCKED ↔
  `ROADMAP` `2026-04-29` `260/260` M3 ✅ ↔ `docs/README` v1.2 LOCKED.
- **Stale-reference sweep:** zero hits on every forbidden pattern.
- **Methodology compliance:** scope prefixes 6/6, ratification pattern
  v1.2 ≡ v1.1, LOCKED decisions D-1..D-7 byte-identical between v1.1 and
  v1.2.
- **Sub-phase acceptance:** M3.1, M3.2, M3.3 fully mapped; M3.4 explicitly
  deferred with rationale.
- **Carried debts forward:** Phase 2 → M7, Phase 3 → M10.C, M3.4 → first
  external mod author.
- **Ready-for-M4:** no surface blocker.
- **Surgical fixes applied this pass:** 0.
- **Items needing follow-up:** 0.

M3 closes cleanly. M4 (Shared ALC) is unblocked.

---

## See also

- [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) v1.2 LOCKED — the
  specification this review verifies.
- [ROADMAP](./ROADMAP.md) — M3 closure status, M3.4 deferral, M4
  pre-conditions.
- [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — §2.4 atomic phase review, §7.3
  process discipline; the verification cycle this report instantiates.
- [PASS_4_REPORT](./PASS_4_REPORT.md) — closure-report format model.
