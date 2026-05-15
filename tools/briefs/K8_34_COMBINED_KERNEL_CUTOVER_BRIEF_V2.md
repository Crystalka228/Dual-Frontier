---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K8_34_COMBINED_V2
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "2.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_34_COMBINED_V2
supersedes:
  - DOC-D-K8_34_COMBINED                      # combined brief v1.0 — Phase 4+5 design replaced
  - DOC-D-K8_34_COMBINED_BRIEF_REFRESH_PATCH  # patch v1 — Phases 0-3 corrections absorbed into v2.0 as executed-state
---
# K8.3+K8.4 — Combined Kernel Cutover Brief v2.0

**Status**: AUTHORED 2026-05-14 — full re-author of `K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md` v1.0 (2026-05-13) + `K8_34_COMBINED_BRIEF_REFRESH_PATCH.md` (2026-05-14).
**Why a v2.0 and not a third patch**: three execution halts on the v1.0 brief — storage-location premise (2026-05-13), API-surface premise (2026-05-14), mid-transition drift (2026-05-14) — are a pattern, not a coincidence. The shared root: v1.0's Phase 4+5 were authored from architecture documents and prior-session notes, not from a fresh read of the actual system / scheduler / test code. A third override layer would repeat the error. v2.0 is authored from the **actual code**, read in full during the 2026-05-14 deliberation session: all 10 surviving production systems, `SystemBase`, `SystemExecutionContext`, `ParallelSystemScheduler`, `World`, `GameBootstrap`, `DependencyGraph`, `ComponentTypeRegistry`, `NativeWorld`, `Bootstrap`, `VanillaComponentRegistration`, both factories, and a representative test fixture.
**Scope**: Single atomic milestone (A'.5). Phases 0-3 are **already executed on disk** — they shipped cleanly via patch v1 and are preserved verbatim as the starting state. Phase 4+5 are **re-designed from scratch** as a single big-bang storage cutover. Phase 6 closure is unchanged in structure.
**Authority**: v2.0 supersedes v1.0 + patch v1 at its execution closure. The architectural locks (Q-COMBINED-1 through Q-COMBINED-8) are **preserved unchanged** — they were never the problem. What is replaced is the Phase 4+5 *execution shape*: v1.0's «12 incremental per-system commits + deferred World retirement» is replaced by «one atomic storage cutover commit».
**Milestone**: A'.5 (combined K8.3+K8.4 — this brief is the sole execution artifact).

---

## ⚠ READ ORDER

This is a single self-contained brief. Read straight through. It supersedes two prior artifacts which remain on disk for historical traceability — **do not execute against them**:
- `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md` (v1.0) — SUPERSEDED
- `tools/briefs/K8_34_COMBINED_BRIEF_REFRESH_PATCH.md` (patch v1) — SUPERSEDED

v2.0 carries everything those two carried that proved correct in execution, restated as fact rather than instruction (Phases 0-3 already shipped). What v2.0 changes is Phase 4+5.

If at any point this brief proves insufficient for mechanical execution, invoke METHODOLOGY §3 «stop, escalate, lock». Do not improvise. Three halts have already taught the cost of authoring from assumption; v2.0 was authored from code to prevent a fourth, but if a premise still proves wrong, halt — that is the system working.

---

## §0 — Q-COMBINED locks (ratified 2026-05-13, preserved unchanged in v2.0)

The eight architectural locks from v1.0 are **unchanged**. They were never the cause of any halt. Restated here so v2.0 is self-contained.

### §0.1 Q-COMBINED-1 — Scope consolidation

Combined K8.3+K8.4 milestone = Track A (storage migration) + Track B (system migration) + Track C (Mod API v3 surface + governance), atomically executed.

**v2.0 amendment to scope**: the production system count drops from 12 to **10**. `ElectricGridSystem` and `ConverterSystem` are **deleted** as disposable vanilla CPU systems (Crystalka direction 2026-05-14 — electricity belongs on the GPU compute pipeline, to be designed in a separate future brief, not migrated here). `EtherGridSystem` (already absent from `coreSystems`, dead code) is deleted as a file. `PowerConsumerComponent` / `PowerProducerComponent` are deleted from `src/DualFrontier.Components/` and from `VanillaComponentRegistration.RegisterAll`. See §2 of this brief.

### §0.2 Q-COMBINED-2 — α hard cutover decision

α hard cutover. No backward compatibility. No legacy API survival. No deprecation warnings. `World` moved to test project as `ManagedTestWorld`. `SystemBase.GetComponent/SetComponent/Query/Query<T1,T2>/GetSystem` deleted. `SystemExecutionContext` refactored: `World` parameter removed, `NativeWorld` required. Isolation guard runtime checks deleted (compile-time `[SystemAccess]` + future A'.9 analyzer).

**v2.0 amendment**: v1.0 permitted «dual-write scaffolding inside milestone execution». v2.0 **removes** this permission. The dual-write bridge was the costyl that the three halts circled around. v2.0's cutover is a single atomic commit with **no dual-write phase at all** — see §4 + §6.4 for why this is now possible and correct.

### §0.3 Q-COMBINED-3 — IModApi v3 surface specification

IModApi v3 ships as the sole IModApi: `RegisterComponent<T> where T : unmanaged, IComponent` (Path α) + `RegisterManagedComponent<T> where T : class, IComponent` (Path β) + `Fields` + `ComputePipelines`. Supporting types: `[ManagedStorage]` attribute, `ManagedStore<T>`, `IManagedStorageResolver`, `ValidationErrorKind.MissingManagedStorageAttribute`.

**v2.0 status**: **already executed on disk** (commits 42f9b91, 74c1b13, 4bf1c62 on `b903b91`). v2.0 preserves it as shipped state. No re-work.

### §0.4 Q-COMBINED-4 — Atomic commit shape

**This is the lock v2.0 substantively re-interprets.** v1.0 specified 24 atomic commits in 6 phases, with Phase 4 as 12 incremental per-system migrations. v2.0's commit shape:

- **Phase 0** (revert + verification): Commit 1 — revert the v1.0 Phase 4-6 deviation commits back to `b903b91`
- **Phase 1-3**: **already on disk** (`b903b91` carries them — 8 commits, all clean, 671 tests green)
- **Phase 4+5 MERGED** (the storage cutover): Commit 2 — **one atomic commit** performing the entire storage backend cutover (system layer + scheduler + World retirement + test rewrite). This is the load-bearing commit of the milestone.
- **Phase 6** (closure): Commits 3-4 — documentation amendments + REGISTER.yaml closure

**Total: 4 commits.** Not 24. The «24 commits» of v1.0 was the original error — it presumed storage cutover was incrementally divisible. It is not. See §4.

### §0.5 Q-COMBINED-5 — Manifest version strict gating

`ManifestParser` accepts only `manifestVersion: "3"`. Hard reject otherwise. **Already executed on disk** (commit b903b91, «ManifestVersion field + strict v3-only parser + 26 manifest updates»). v2.0 preserves it.

### §0.6 Q-COMBINED-6 — Test strategy

Per-system test classes; `NativeWorldTestFixture` shared fixture; `ManagedTestWorld` for legacy `Core.Tests`.

**v2.0 amendment**: v1.0 said «rewrite ~50 test fixtures». v2.0 says **delete and re-author** `DualFrontier.Systems.Tests` entirely (Crystalka direction 2026-05-14 — vanilla mechanics are disposable; migrating tests that clutch the managed `World` is itself a costyl). `DualFrontier.Core.Tests` is **not** deleted — it tests the managed `World` *as the subject under test*, and migrates mechanically to `ManagedTestWorld`. See §7.

### §0.7 Q-COMBINED-7 — Closure protocol

Full METHODOLOGY §12.7 closure protocol. v2.0 preserves the structure; the specific REGISTER updates are restated in §9 with v2.0's commit hashes and the power-deletion reflected.

### §0.8 Q-COMBINED-8 — Brief structure

v1.0 was concern-axis organized. v2.0 keeps that but the document is shorter — Phases 0-3 are stated as executed fact (a few paragraphs) rather than instruction (sections), and Phase 4+5 are one cutover rather than two phases of many commits.

---

## §1 — What changed from v1.0 + patch v1, and why

This section exists so a reader who knows v1.0 can map it to v2.0 fast. A reader new to the milestone can skip to §2.

### §1.1 — The three halts and their shared root

| Halt | Date | Premise that was wrong | Resolved by |
|---|---|---|---|
| 1 — storage-location | 2026-05-13 | K8.3 v2.0 brief presumed component storage already in NativeWorld | combined brief v1.0 absorbed K8.3+K8.4 |
| 2 — API-surface | 2026-05-14 | v1.0 §2.1 prescribed `new ComponentTypeRegistry()` — no such ctor; invented a helper that already existed; wrong sln path; factory bulk-write shape incompatible with real K8.1-primitive structure | patch v1 (5 findings) |
| 3 — mid-transition drift | 2026-05-14 | v1.0 §6.5 split Phase 4 into 12 incremental commits; the dual-write bridge between them diverges per tick because Tier-1 consumers read components Tier-3 producers write, a 7-commit gap | **this brief, v2.0** |

Shared root across all three: **v1.0's Phase 4+5 were authored from architecture docs and session notes, not from the actual code.** Patch v1 fixed the API-surface layer (Phases 0-3) by reading the kernel files. v2.0 fixes the Phase 4+5 layer by reading the system / scheduler / test files. Same discipline, applied to the layer patch v1 did not reach.

### §1.2 — What patch v1 already shipped (Phases 0-3, on disk at `b903b91`)

Verified via `git log` — commits `65e696d..b903b91`:

- `65e696d` — combined brief + refresh patch + Phase 0 halt report committed
- `981caa3` — 3 orphan `.uid` files deleted (METHODOLOGY §7.1)
- `6262d77` — **Bootstrap.Run constructs registry internally** (`Run(bool useRegistry = true)`; internal ctor reachable same-assembly; stale K3 overload deleted) + `RegisterAll` extension
- `ec753b6` — factory two-phase pattern + `GameBootstrap` wired to registry
- `42f9b91` — IModApi v3 surface (RegisterComponent constraint split + RegisterManagedComponent)
- `74c1b13` — `[ManagedStorage]` attribute + `ManagedStore<T>` + `ValidationErrorKind` extension
- `4bf1c62` — Path β bridge wired (RestrictedModApi v3 + ModRegistry resolver + SystemBase accessor)
- `b903b91` — ManifestVersion field + strict v3-only parser + 26 manifest updates

All 8 commits clean, 671 tests green at each. **This is the starting state for v2.0.** Phases 0-3 are not re-done; they are fact.

### §1.3 — What v1.0's Phase 4 actually produced (the deviation being reverted)

Verified by reading the on-disk systems. v1.0's Phase 4 (commits `11c64e0..02f9ecf`) produced:

- A Phase 4 halt report (`11c64e0`)
- 12 «system migration» commits that, on inspection, did **one thing each**: wrapped a single `SetComponent` call in `BeginBatch` and left the legacy `SetComponent` beside it as a «dual-write mirror». Example — `ConsumeSystem.ApplyRestoration` batches `ConsumableComponent` but the **entire rest of the system** — `Query<JobComponent, NeedsComponent>()`, every `GetComponent<...>` — still reads the managed `World`. The system is not migrated. Four of the twelve are «no-op marker» commits (ComfortAura, PawnStateReporter, Haul, Converter — pure publishers, nothing to batch).
- Phase 5 commits 21-22 «deferred» — scratch docs only, no code
- Phase 6 commits 23-24 — «partial closure» entry + «REGISTER deferred» scratch doc

The in-code comments are honest about it: every batched write carries `// Phase 5 commit 21 removes the legacy mirror when reads switch fully to NativeWorld`. The real work was always still in the deferred Phase 5. v1.0's Phase 4 on disk is **symbolic** — it renamed the problem, it did not solve it. It carries no architectural value worth preserving. **It is reverted in full (§3).**

### §1.4 — The architectural reason Phase 4 cannot be incremental

Storage backend is a **binary property of a component type**: `NeedsComponent` lives either in the managed `World` or in `NativeWorld`. There is no «half». While any system reads `NeedsComponent` from the managed `World`, the factories must populate it there. The moment the last reader moves to `NativeWorld`, the managed `World` is dead for that type.

There are **no valid intermediate states** at «one system at a time» granularity. There is «before cutover» and «after cutover». v1.0 split Phase 4 into 12 commits presuming each compiles and passes — that presumption is false. `SystemExecutionContext` with `_world` removed does not compile while any system calls `Query<T>()`; the last system does not compile until `SystemBase.Query` is deleted. The K8.1 lesson «atomic = compilable unit» means the **atomic unit is the entire cutover** — one commit. Splitting it is what re-introduces drift. v1.0's «dual-write bridge» was the attempt to paper over a split that should never have been made.

v2.0's Commit 2 is that one atom.

---

## §2 — Power subsystem deletion (do this inside Commit 2, before the cutover proper)

Crystalka direction 2026-05-14: electricity is a vanilla mechanic and a CPU-tick implementation of it is the wrong shape — it belongs on the GPU compute pipeline. The compute-pipeline design is a **separate future brief**; v2.0 does **not** design it and does **not** preserve the CPU implementation «just in case». Per the project's §10.5 principle «data exists or it doesn't», an orphaned component with no system is a smell — so the power components are deleted too, not parked.

This deletion happens **inside Commit 2**, as its first mechanical step, before the storage cutover proper. Reason: deleting two of the twelve systems first means the cutover migrates 10, not 12 — smaller, cleaner atom. Doing it as a separate commit would leave an intermediate state where `coreSystems` references deleted components; folding it into Commit 2 keeps every commit compilable.

### §2.1 — Systems deleted

| File | Disposition |
|---|---|
| `src/DualFrontier.Systems/Power/ElectricGridSystem.cs` (+ `.cs.uid`) | DELETE |
| `src/DualFrontier.Systems/Power/ConverterSystem.cs` (+ `.cs.uid`) | DELETE |
| `src/DualFrontier.Systems/Power/EtherGridSystem.cs` (+ `.cs.uid`) | DELETE — already dead code, absent from `coreSystems`; removed now so the dead file does not accumulate |
| `src/DualFrontier.Systems/Power/README.md` | DELETE — the directory is emptied |

After deletion the `src/DualFrontier.Systems/Power/` directory is empty — remove the directory.

### §2.2 — Components deleted

| File | Disposition |
|---|---|
| `PowerConsumerComponent` (in `src/DualFrontier.Components/Building/`) | DELETE |
| `PowerProducerComponent` (in `src/DualFrontier.Components/Building/`) | DELETE |

**Phase 0 verification task**: confirm the exact file paths and whether the two components share a file or have one file each — read `src/DualFrontier.Components/Building/` before deleting. They are referenced as `DualFrontier.Components.Building` in `ElectricGridSystem` / `ConverterSystem`, so they live under `Building/`.

### §2.3 — `VanillaComponentRegistration.RegisterAll` edit

`RegisterAll` currently registers `PowerConsumerComponent` + `PowerProducerComponent` under a «Building (2)» category block. **Delete both registration calls and the Building category block.** Update the category-count comment. This shifts the registered component count down by 2 — that is correct and intended; the K-L4 contract (stable order, sequential ids) holds for the *remaining* components because deletion of a trailing-category pair does not reorder anything above it. Verify at Phase 0 that the Building block is last or near-last in `RegisterAll`; if a later category exists, its ids shift down by 2 — acceptable (ids are deterministic per-run, not persisted across versions), but note it in the Commit 2 message.

### §2.4 — Power events

`DualFrontier.Events.Power` namespace carries `PowerGrantedEvent`, `GridOverloadEvent`, `ConverterPowerOutputEvent` (verified — referenced in the deleted systems). **Phase 0 verification task**: grep the entire `src/` and `tests/` tree for each of these three event types. If a deleted power system is the **only** producer and consumer of an event, delete the event type too. If any non-power code references one, **do not delete it** — halt and escalate, because that would mean a power event leaked into non-power code, which is an architectural finding worth surfacing, not silently working around.

```powershell
# Phase 0 — power event consumer sweep
Select-String -Path src/,tests/ -Pattern 'PowerGrantedEvent|GridOverloadEvent|ConverterPowerOutputEvent' -Recurse
# Expected: matches only inside the files being deleted. Any other match → halt + escalate.
```

If the three events are deleted, the `DualFrontier.Events.Power` namespace / directory may become empty — remove it if so.

### §2.5 — Tests deleted

| Path | Disposition |
|---|---|
| `tests/DualFrontier.Systems.Tests/Power/ConverterEfficiencyTests.cs` | DELETE |
| `tests/DualFrontier.Systems.Tests/Power/ElectricGridOverloadTests.cs` | DELETE |

These are deleted with the systems, **not** re-authored — there is no power system left to test. (Note: the entire `DualFrontier.Systems.Tests` project is deleted and re-authored anyway per §7 — the power tests are simply not among the re-authored set.)

### §2.6 — `GameBootstrap.cs` edit

The `coreSystems` array in `GameBootstrap.CreateLoop` currently lists 12 systems. **Remove the `new ElectricGridSystem()` and `new ConverterSystem()` entries.** The array becomes 10 systems. Remove the now-unused `using DualFrontier.Systems.Power;`. This edit is part of the larger `GameBootstrap` rewrite in Commit 2 (§6) — listed here so the power-deletion picture is complete in one place.

### §2.7 — Sweep for stragglers

After the above, grep for any remaining reference to the deleted types:

```powershell
Select-String -Path src/,tests/ -Pattern 'ElectricGridSystem|ConverterSystem|EtherGridSystem|PowerConsumerComponent|PowerProducerComponent' -Recurse
# Expected: zero matches after §2.1-§2.6 applied.
```

Any straggler is fixed where found (a `using`, a doc-comment cross-reference, a `README` mention). If a straggler is in a LOCKED architecture document, **do not edit the LOCKED doc inline** — note it for the Phase 6 closure amendment batch (§9). Architecture docs may legitimately mention electricity as a *planned* feature — that is not a straggler, leave it; only code references and stale «implemented» claims are stragglers.

---

## §3 — Phase 0: revert the v1.0 deviation (Commit 1)

Phase 0 is one commit: revert `11c64e0..02f9ecf` so the working tree returns to `b903b91` (clean end-of-Phase-3). The big-bang cutover (Commit 2) then starts from solid ground.

### §3.1 — Why revert, not build-on-top

v1.0's Phase 4 commits carry per-system dual-write into production system bodies (§1.3). Commit 2 rewrites those same system bodies. Rewriting *on top of* the symbolic migrations means holding two layers of change in mind at once and raising the risk of missing a half-migrated fragment. A clean revert gives Commit 2 untouched end-of-Phase-3 systems. The revert loses nothing of value — v1.0's Phase 4 is symbolic (§1.3).

### §3.2 — The revert

```powershell
# Confirm starting HEAD
git log --oneline -1
# Expected: 02f9ecf — "docs(scratch): Phase 6 commit 24 — REGISTER.yaml updates deferred ..."

# Confirm b903b91 is the end-of-Phase-3 commit
git log --oneline b903b91 -1
# Expected: b903b91 — "feat(modding): ManifestVersion field + strict v3-only parser + 26 manifest updates (Phase 3 commit 8)"

# Revert the entire v1.0 Phase 4-6 deviation range as a single revert commit.
# --no-commit accumulates all reverts; one commit closes Phase 0.
git revert --no-commit 11c64e0..02f9ecf
```

`git revert A..B` reverts every commit in the range `(A, B]` — i.e. `11c64e0` exclusive through `02f9ecf` inclusive is **not** what we want; we want `11c64e0` **inclusive**. Use:

```powershell
# Correct: revert 11c64e0 through 02f9ecf inclusive.
# git revert RANGE is exclusive of the left endpoint, so use 11c64e0^.
git revert --no-commit 11c64e0^..02f9ecf
```

**Phase 0 verification — confirm the revert lands the tree at b903b91 content:**

```powershell
# The working tree after revert must be content-identical to b903b91.
git diff b903b91 --stat
# Expected: empty output (no diff) — the revert exactly undoes the deviation.
```

If `git diff b903b91 --stat` is **not** empty, the revert did not cleanly undo the deviation — **halt and escalate**. Do not hand-patch the difference. A non-empty diff means either the range was wrong or a deviation commit touched something outside the deviation scope; either way it is a finding, not something to improvise around.

### §3.3 — Preserve the halt reports

The revert undoes the *code* deviations and the Phase 5-6 scratch *deferral* docs. The three halt reports are historical record and should survive:
- `docs/scratch/A_PRIME_5/HALT_REPORT.md`
- `docs/scratch/A_PRIME_5_CONTINUED/HALT_REPORT.md`
- `docs/scratch/A_PRIME_5_CONTINUED/HALT_REPORT_PHASE_4.md`

After the revert, check whether these files still exist in the working tree:

```powershell
Test-Path docs/scratch/A_PRIME_5/HALT_REPORT.md
Test-Path docs/scratch/A_PRIME_5_CONTINUED/HALT_REPORT.md
Test-Path docs/scratch/A_PRIME_5_CONTINUED/HALT_REPORT_PHASE_4.md
```

If the revert removed any of them (because the commit that created it is in the reverted range), restore just that file from its creating commit:

```powershell
# Example — restore a halt report the revert removed:
git checkout 11c64e0 -- docs/scratch/A_PRIME_5_CONTINUED/HALT_REPORT_PHASE_4.md
```

The halt reports are evidence the methodology worked — they stay.

### §3.4 — Commit 1

```powershell
git add -A
git commit -m "revert(a-prime-5): undo v1.0 Phase 4-6 deviation — return to end-of-Phase-3 (b903b91)

v1.0's Phase 4 (11c64e0..02f9ecf) produced 12 symbolic per-system
'migrations' — each wrapped one SetComponent in BeginBatch and left the
legacy mirror beside it; all reads stayed on the managed World. Four were
no-op marker commits. Phase 5-6 were deferred scratch docs. The deviation
carries no architectural value (see brief v2.0 §1.3) and per-system
dual-write is the costyl this milestone exists to remove.

This revert returns the tree to b903b91 (clean end-of-Phase-3: Bootstrap.Run
+ RegisterAll + Mod API v3 + Path β bridge, 671 tests green). The big-bang
storage cutover (Commit 2) starts from here.

Halt reports preserved as historical record (methodology working as designed).

Phase 0 of K8.3+K8.4 combined milestone v2.0. Commit 1 of 4."
```

### §3.5 — Phase 0 build + test gate

After Commit 1, the tree is `b903b91` content. Confirm it builds and tests green — this is the floor the cutover builds from:

```powershell
dotnet build DualFrontier.sln
dotnet test DualFrontier.sln
# Expected: build clean, 671 tests green. This is the verified starting state.
```

If build or tests fail here, something is wrong with the revert or with `b903b91` itself — **halt and escalate**. Commit 2 cannot start from an unverified floor.

---

## §4 — The big-bang storage cutover (Commit 2 — the load-bearing commit)

Commit 2 is the entire storage backend cutover, atomically. It does not compile until it is complete, and it must not be committed until it is complete and `dotnet test DualFrontier.sln` is green. There is no valid partial state (§1.4). This section specifies it in full.

### §4.0 — Phase 0 deep-read requirement (do this before writing a line of Commit 2)

v2.0 was authored from a read of these files at a point in time. Before executing Commit 2, **re-read every one of them** to confirm nothing drifted, and to pick up the exact current line content this brief paraphrases. This is Lesson #7 applied to the executor: a brief that prescribes API must be checked against the API at execution time.

Mandatory re-reads:
- `src/DualFrontier.Core/ECS/SystemBase.cs`
- `src/DualFrontier.Core/ECS/SystemExecutionContext.cs`
- `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs`
- `src/DualFrontier.Core/Scheduling/DependencyGraph.cs` (read-only context — not modified, but the executor must understand it)
- `src/DualFrontier.Core/ECS/World.cs`
- `src/DualFrontier.Core/ECS/ComponentStore.cs` (referenced by World; read to understand the move in §4.7)
- `src/DualFrontier.Application/Loop/GameBootstrap.cs`
- `src/DualFrontier.Application/Scenario/RandomPawnFactory.cs`
- `src/DualFrontier.Application/Scenario/ItemFactory.cs`
- All 10 surviving system files (§4.5 lists them)
- `tests/DualFrontier.Systems.Tests/` — every file, to know what is being deleted (§7)
- `tests/DualFrontier.Core.Tests/` — directory listing + a representative sample, to scope the mechanical rename (§7)
- `src/DualFrontier.Core.Interop/NativeWorld.cs` — the cutover's target API; the executor must know the exact signatures of `AcquireSpan<T>`, `BeginBatch<T>`, `WriteBatch<T>.Update`, `GetEntitiesWith`/equivalent, `CreateComposite`, `InternString`, `CreateMap`

If any re-read surfaces a contradiction with this brief — **halt and escalate**. Do not improvise.

### §4.1 — Internal ordering of Commit 2

Commit 2's edits, in dependency order so the executor can build mentally even though only the final state compiles:

1. **Power deletion** (§2) — delete the 2 systems + dead `EtherGridSystem` + 2 components + power tests + power events (if orphaned); edit `RegisterAll`; edit `coreSystems`.
2. **`SystemExecutionContext` refactor** (§4.2) — remove `_world`, make `_nativeWorld` required, delete the isolation-guard methods + the `_allowedReads`/`_allowedWrites` sets.
3. **`SystemBase` refactor** (§4.3) — delete `GetComponent`/`SetComponent`/`Query`/`Query<T1,T2>`/`GetSystem`.
4. **`ParallelSystemScheduler` refactor** (§4.4) — remove the `World world` ctor parameter + `_world` field; `BuildContext` no longer passes `_world`.
5. **10 system bodies rewritten** (§4.5) — `Query`/`GetComponent`/`SetComponent` → `AcquireSpan`/`BeginBatch`.
6. **`GameBootstrap.CreateLoop` rewrite** (§4.6) — remove `new World()`, the dual-write loop, the `world` parameters; rewrite `PublishItemSpawnedEvents` + `excludedPositions` on NativeWorld.
7. **Factories native-only** (§4.6) — remove the dual-write loop from `RandomPawnFactory`/`ItemFactory`; drop the `World world` parameter.
8. **`World` → `ManagedTestWorld`** (§4.7) — move `World.cs` + `ComponentStore.cs` + `IRemovable` to the test project, rename, widen visibility.
9. **Test rename + rewrite** (§7) — `Core.Tests` mechanical rename `World` → `ManagedTestWorld`; `Systems.Tests` deleted and re-authored against `NativeWorldTestFixture`.

### §4.2 — `SystemExecutionContext` refactor (exact)

Current state (verified): `SystemExecutionContext` holds `private readonly World _world`, takes `World world` as the first ctor parameter (required, null-checked), holds `_allowedReads`/`_allowedWrites` HashSets, and exposes `GetComponent<T>`/`SetComponent<T>`/`Query<T>`/`Query<T1,T2>`/`QueryIntersection<T1,T2>`/`GetSystem<TSystem>` — all routing to `_world`. It also holds `_nativeWorld` (optional, nullable) and `_managedStorageResolver` (optional), exposed via `NativeWorld` and `ResolveManagedStore<T>`.

**Target state:**
- `private readonly World _world` — **deleted**.
- The `World world` ctor parameter — **deleted**. Update the ctor signature, the XML doc, and the `_world = world ?? throw...` line.
- `private readonly NativeWorld? _nativeWorld` → `private readonly NativeWorld _nativeWorld` — **now required**, null-checked in the ctor. The ctor parameter `NativeWorld? nativeWorld = null` → `NativeWorld nativeWorld` (required, no default).
- `internal NativeWorld? NativeWorld => _nativeWorld;` → `internal NativeWorld NativeWorld => _nativeWorld;` (non-nullable).
- `_allowedReads`, `_allowedWrites` HashSets + the `allowedReads`/`allowedWrites` ctor params + `IsReadAllowed` — **deleted**. Q-COMBINED-2 Option A: isolation is enforced at compile time by `[SystemAccess]` declarations consumed by `DependencyGraph` (which still reads them for edge-building — that is unaffected) and by the future A'.9 analyzer. The *runtime* guard methods go.
- `GetComponent<T>`, `SetComponent<T>`, `Query<T>`, `Query<T1,T2>`, `QueryIntersection<T1,T2>` — **deleted** (they were the managed-`World` access surface).
- `GetSystem<TSystem>` — **deleted**. It always threw `IsolationViolationException` anyway; with the analyzer catching system-to-system references at compile time, the always-throwing runtime stub is dead weight. (Verify at Phase 0 that no system actually calls it — it should not, it always threw.)
- `ThrowUndeclaredRead`/`ThrowUndeclaredWrite`/`BuildReadViolationMessage`/`BuildWriteViolationMessage`/`RouteAndThrow` + the `IsolationDiagnostics` usage — **deleted** along with the guard. **Phase 0 check**: confirm `IsolationViolationException` and `IsolationDiagnostics` have no other consumers; if they are now fully orphaned, delete them too — if anything else references them, leave them and note it.
- **Kept unchanged**: `_managedStorageResolver`, `ResolveManagedStore<T>`, `_services`/`Services`, `_systemName`, `_origin`/`_modId`/`_faultSink` (still used for mod-fault routing — the fault sink is not storage-related), `_allowedBuses`, `Current`/`PushContext`/`PopContext`, the `ThreadLocal` machinery.

The `_origin`/`_modId`/`_faultSink` triple stays because mod-fault routing is orthogonal to storage. `_allowedBuses` stays — bus-access declaration is still meaningful. Only the *component-access* guard goes.

### §4.3 — `SystemBase` refactor (exact)

Current state (verified): `SystemBase` exposes `protected T GetComponent<T>(EntityId)`, `protected void SetComponent<T>(EntityId, T)`, `protected IEnumerable<EntityId> Query<T>()`, `protected IEnumerable<EntityId> Query<T1,T2>()`, `protected TSystem GetSystem<TSystem>()` — all delegating to `SystemExecutionContext.Current`. It also exposes `protected IGameServices Services`, `protected NativeWorld NativeWorld`, `protected ManagedStore<T>? ManagedStore<T>()`.

**Target state:**
- `GetComponent<T>`, `SetComponent<T>`, `Query<T>`, `Query<T1,T2>`, `GetSystem<TSystem>` — **deleted**. These were the managed-`World` access surface; their `SystemExecutionContext` counterparts are gone (§4.2).
- **Kept unchanged**: `Context` property, `OnInitialize`/`Update`/`OnDispose`/`Initialize`/`Dispose` lifecycle, `Services` accessor, `NativeWorld` accessor (already present, Phase 3), `ManagedStore<T>()` accessor (already present, Phase 3).

After this, systems reach component storage **only** through `NativeWorld` (the accessor that is already there). There is no managed-`World` path left in `SystemBase`.

### §4.4 — `ParallelSystemScheduler` refactor (exact)

Current state (verified): ctor takes `World world` as the third parameter (required, null-checked into `_world`); `_world` is passed to every `SystemExecutionContext` in `BuildContext`; `Rebuild` goes through the same `BuildContext`.

**Target state:**
- `private readonly World _world` — **deleted**.
- The `World world` ctor parameter — **deleted**. Update signature, XML doc, the `_world = world ?? throw...` line.
- `_nativeWorld` — currently `NativeWorld? nativeWorld = null` (optional). **Make it required**: `NativeWorld nativeWorld` (no default, null-checked). The scheduler cannot build a `SystemExecutionContext` without it now (§4.2 made it required there).
- `BuildContext` — the `new SystemExecutionContext(_world, systemType.FullName ...)` call drops `_world` as the first argument. The argument list now starts with `systemType.FullName`. The `_nativeWorld` argument is now non-null.
- `Rebuild` — no signature change (it never took `_world`), but it calls `BuildContext`, which changed — so it is correct automatically once `BuildContext` is fixed.
- **Kept unchanged**: everything else — `_phases`, `_ticks`, `_faultSink`, `_services`, `_managedStorageResolver`, `_parallelOptions`, `_contextCache`, `_systemMetadata`, `ExecutePhase`, `ExecuteTick`, `InitializeAllSystems`, the `IDeferredFlush` flush.

### §4.5 — The 10 system bodies (per-system migration specs)

All 10 surviving systems migrate their bodies from the managed-`World` access surface (`Query`/`GetComponent`/`SetComponent`) to `NativeWorld` (`AcquireSpan`/`BeginBatch`). The **game logic between storage calls does not change** — Chebyshev distance, job-picking, mood formula, the event publications, the `[SystemAccess]` attributes, the `OnInitialize` subscriptions — all unchanged. Only the storage access changes.

**General migration rules** (apply to every system):
- `foreach (EntityId e in Query<T>())` → iterate `NativeWorld.AcquireSpan<T>()` (confirm the exact span/enumeration API at Phase 0 from `NativeWorld.cs` — the brief uses `AcquireSpan<T>` per Q-COMBINED naming; the executor uses whatever the real method is).
- `Query<T1,T2>()` — there is no two-type span query on `NativeWorld`. Replace with: acquire the span of the smaller-cardinality type, and for each entity check presence of the second type via `NativeWorld`'s has-component / try-get API. This mirrors what `QueryIntersection<T1,T2>` did internally (it picked the smaller store and filtered). Confirm the has-component API name at Phase 0.
- `GetComponent<T>(id)` → read from the acquired span by entity id (confirm span indexing API at Phase 0 — by `EntityId` key, or by dense index).
- `SetComponent<T>(id, value)` → `using var batch = NativeWorld.BeginBatch<T>(); batch.Update(id, value);` — one batch per component type per `Update` call, disposed at end of scope (one P/Invoke at dispose). Event-handler writes (in `OnInitialize` subscriptions) each get their own `BeginBatch` scope inside the handler.
- Pure publishers (`writes: Type[0]`) get **no** `BeginBatch` — only their reads migrate.
- K8.1 primitives embedded in components (`NativeComposite<GridVector> Path`, `NativeMap<...> Items`, `NativeMap<...> Levels`, `InternedString Name`) are **already** in `NativeWorld` — they do not migrate, they are accessed exactly as they are today (`move.Path.CountFor(...)`, `storage.Items.TryGet(...)`, `skills.Levels.Iterate(...)`, `name.Resolve(NativeWorld)`).

**Per-system table** — read each file at Phase 0; this table is the migration map:

| System | File | `[SystemAccess]` writes | Migration shape |
|---|---|---|---|
| `NeedsSystem` | `Pawn/NeedsSystem.cs` | `NeedsComponent` | `Update`: `Query<NeedsComponent>` → span; one `BeginBatch<NeedsComponent>` wraps the loop. `OnNeedsRestored` handler: read+write `NeedsComponent`, own `BeginBatch`. Drop the legacy `SetComponent` mirror entirely. |
| `MoodSystem` | `Pawn/MoodSystem.cs` | `MindComponent` | `Query<NeedsComponent, MindComponent>` → span of one + presence-check of the other; one `BeginBatch<MindComponent>` wraps the loop. Drop the legacy mirror. |
| `JobSystem` | `Pawn/JobSystem.cs` | `JobComponent` | `Update`: `Query<NeedsComponent, JobComponent>` → span + presence-check. 5 event handlers (`OnConsumeTarget`/`OnConsumeFinished`/`OnSleepTarget`/`OnSleepFinished` + the `Update` write) all funnel through the `WriteJob` helper — rewrite `WriteJob` to be NativeWorld-only `BeginBatch<JobComponent>`, drop the legacy mirror. The `_urgentPawns` HashSet + `OnNeedsCritical` are unchanged (not storage). |
| `ConsumeSystem` | `Pawn/ConsumeSystem.cs` | `ConsumableComponent` | `Update` + `FindNearest*` helpers: many `Query<...>` and `GetComponent<...>` → spans. `ApplyRestoration` writes `ConsumableComponent` via `BeginBatch`, drop the legacy mirror. The event publications (`PawnConsumeTargetEvent`/`PawnConsumeFinishedEvent`/`NeedsRestoredEvent`) are unchanged. |
| `SleepSystem` | `Pawn/SleepSystem.cs` | `BedComponent` | 3-phase `Update` with multiple `Query<BedComponent>` / `Query<JobComponent, PositionComponent>` / `Query<BedComponent, PositionComponent>` sites — all → spans. `BedComponent` writes (occupant claim, occupant release) via `BeginBatch<BedComponent>`, drop the legacy mirrors. Event publications unchanged. |
| `MovementSystem` | `Pawn/MovementSystem.cs` | `PositionComponent`, `MovementComponent` | **Two** write components — two `BeginBatch` scopes (or confirm at Phase 0 whether `BeginBatch` can span two types; the brief assumes one type per batch, so two batches). `Update`: `Query<MovementComponent>` → span, reads `PositionComponent` via span. `WriteMove`/`WritePos` helpers → NativeWorld-only, drop legacy mirrors. 4 event handlers funnel through `WriteMove`. `move.Path` (`NativeComposite<GridVector>`) is already native — `CountFor`/`TryGetAt`/`Add`/`ClearFor`/`CreateComposite` unchanged. |
| `PawnStateReporterSystem` | `Pawn/PawnStateReporterSystem.cs` | `Type[0]` — pure publisher | Reads only. `Query<IdentityComponent>` / `Query<SkillsComponent>` / `Query<NeedsComponent, JobComponent>` → spans + presence-checks. `GetComponent<...>` → span reads. `IdentityComponent.Name.Resolve(NativeWorld)` and `SkillsComponent.Levels.Iterate(...)` unchanged (K8.1). No `BeginBatch`. |
| `InventorySystem` | `Inventory/InventorySystem.cs` | `StorageComponent` | `RebuildCache`: `Query<StorageComponent>` → span. 3 event handlers (`OnItemAdded`/`OnItemRemoved`/`OnItemReserved`) — the first two read+write `StorageComponent` via `BeginBatch`, drop legacy mirrors. `storage.Items` (`NativeMap<InternedString,int>`) + `InternString` unchanged (K8.1). The `_reservedQuantities` / `_freeSlotCache` dictionaries are plain managed state — unchanged. |
| `HaulSystem` | `Inventory/HaulSystem.cs` | `Type[0]` — pure publisher | Reads only. `Query<JobComponent>` / `Query<StorageComponent>` → spans. `GetComponent<...>` → span reads. `s.Items.Iterate(...)` / `Resolve(NativeWorld)` unchanged. `_inCallReservations` HashSet unchanged. No `BeginBatch`. |
| `ComfortAuraSystem` | `Pawn/ComfortAuraSystem.cs` | `Type[0]` — pure publisher | Reads only. Nested `Query<DecorativeAuraComponent, PositionComponent>` × `Query<NeedsComponent, PositionComponent>` → spans + presence-checks. `GetComponent<...>` → span reads. No `BeginBatch`. |

**Per-system threshold — migrate the body, or delete-and-rewrite the file?** (Crystalka direction 2026-05-14.) Default: **migrate the body, keep the file** — the game logic is already written and correct; rewriting it loses work for no gain. **Exception**: if migrating a system's body hits a managed-`World`-specific assumption that does not survive the cutover — e.g. logic that depends on `Query`'s lazy `yield` semantics in a way `AcquireSpan` cannot replicate, or state shaped around `World`'s deferred-destruction invariant — then **delete that system file and re-author it against `NativeWorld` from scratch**. This is a per-system judgment made *at execution time*, on contact. If the executor hits such a case, it should note it in the Commit 2 message (`<SystemName> re-authored rather than migrated — <reason>`). The brief's expectation, from the read of all 10 systems: most or all migrate cleanly (the systems are already structured around `[SystemAccess]` and event-driven cross-component writes — the managed-`World` coupling is shallow). But the threshold is stated so the executor does not force a costyl-migration where a clean rewrite is cheaper.

### §4.6 — `GameBootstrap.CreateLoop` + factories rewrite (exact)

Current state (verified): `CreateLoop` calls `Bootstrap.Run(useRegistry: true)` + `RegisterAll`, then `var world = new World();`, builds the factories, calls `pawnFactory.Spawn(nativeWorld, world, services, ...)` and `itemFactory.Spawn(nativeWorld, world, excludedPositions, ...)` (dual-write), builds `excludedPositions` by reading `world.TryGetComponent<PositionComponent>`, calls `PublishItemSpawnedEvents(world, services)` which reads `world.GetEntitiesWith<T>()`, builds the 12-entry `coreSystems` array, and passes `world` + `nativeWorld` to the `ParallelSystemScheduler` ctor.

**Target state:**
- `var world = new World();` — **deleted**.
- `coreSystems` — 12 → **10** entries (§2.6: drop `ElectricGridSystem`, `ConverterSystem`).
- `pawnFactory.Spawn(nativeWorld, world, services, ...)` → `pawnFactory.Spawn(nativeWorld, services, ...)` — the `world` argument drops (factory signature change below).
- `itemFactory.Spawn(nativeWorld, world, excludedPositions, ...)` → `itemFactory.Spawn(nativeWorld, excludedPositions, ...)` — `world` drops.
- `excludedPositions` build loop — currently `world.TryGetComponent<PositionComponent>(pid, out var pawnPos)`. Rewrite to read `PositionComponent` from `nativeWorld` (acquire the `PositionComponent` span, look up each pawn id). Confirm the exact read API at Phase 0.
- `PublishItemSpawnedEvents(world, services)` → `PublishItemSpawnedEvents(nativeWorld, services)`. Its body currently does `world.GetEntitiesWith<ConsumableComponent>()` etc. — rewrite to acquire the `NativeWorld` span for each of the 4 component types (`ConsumableComponent`, `WaterSourceComponent`, `BedComponent`, `DecorativeAuraComponent`) and iterate. `PublishOne` currently does `world.TryGetComponent<PositionComponent>` — rewrite to read from the `PositionComponent` span.
- `ParallelSystemScheduler` ctor call — drop the `world` argument (§4.4 removed the parameter). The call now passes `graph.GetPhases(), ticks, initialMetadata, faultHandler, services, nativeWorld, modRegistry`.
- Remove `using DualFrontier.Core.ECS;` if `World` was its only consumer in this file — verify at Phase 0 (other `Core.ECS` types like `EntityId` may keep the using alive; `EntityId` is in `Contracts.Core` per the system files, so check).
- Remove `using DualFrontier.Systems.Power;` (§2.6).

**`RandomPawnFactory` — target signature:** `Spawn(NativeWorld nativeWorld, GameServices services, int count)` — the `World world` parameter drops. The body's two-phase pattern (Phase A: per-entity K8.1-primitive allocation — `InternString`/`CreateMap`/`CreateComposite`; Phase B: bulk component add to `nativeWorld`) is **kept** — patch v1 already shipped it. What drops is the «Dual-write to managed World» loop that follows Phase B. Verify at Phase 0 that patch v1's factory shape is on disk (it should be — commit `ec753b6`); the cutover only removes the dual-write tail.

**`ItemFactory` — target signature:** `Spawn(NativeWorld nativeWorld, IReadOnlyCollection<GridVector> excludedPositions, int foodCount, int waterCount, int bedCount, int decorationCount)` — the `World world` parameter drops. The `NativeWorld nativeWorld` ctor parameter that patch v1 added is **kept**. The dual-write loop drops. None of `ItemFactory`'s four component types embed K8.1 primitives — its Phase A is trivial, Phase B is a straight bulk add per type.

**`RandomPawnFactory` ctor** — patch v1 gave it a `NativeWorld` field already (verify at Phase 0). It is kept. The ctor in `GameBootstrap` is `new RandomPawnFactory(FactorySeed, navGrid, MapWidth, MapHeight)` — confirm whether patch v1's on-disk ctor takes `NativeWorld` or not; the factory body uses `_nativeWorld`, so the ctor must supply it. Read the on-disk factory at Phase 0 and match.

### §4.7 — `World` → `ManagedTestWorld` move (exact)

Current state (verified): `internal sealed class World` in `src/DualFrontier.Core/ECS/World.cs`, with `internal World()` ctor, `public` methods (`CreateEntity`, `IsAlive`, `DestroyEntity`, `AddComponent`, `RemoveComponent`, `HasComponent`, `TryGetComponent`) and `internal` methods (`FlushDestroyedEntities`, `GetComponentUnsafe`, `SetComponent`, `GetEntitiesWith`, `GetComponentCount`). The file also declares `internal interface IRemovable` at its end. `World` depends on `ComponentStore<T>` (in `ComponentStore.cs`).

**Target:** `World` survives **only as a test-project type** — `DualFrontier.Core.Tests` tests it as the subject under test (it is a legitimate ECS primitive; it is just no longer the *production* storage backend).

- Move `src/DualFrontier.Core/ECS/World.cs` → `tests/DualFrontier.Core.Tests/Fixtures/ManagedTestWorld.cs`.
- Move `src/DualFrontier.Core/ECS/ComponentStore.cs` → `tests/DualFrontier.Core.Tests/Fixtures/` (it is `World`'s dependency; read it at Phase 0 to confirm it has no other production consumer — if it does, that consumer also needs migrating and the executor should halt+escalate to surface it).
- Rename `class World` → `class ManagedTestWorld`. Rename the file accordingly. `IRemovable` moves with it (same file or a sibling — keep it adjacent).
- Visibility: `internal sealed class World` → `public sealed class ManagedTestWorld`. The `internal` methods (`FlushDestroyedEntities`, `GetComponentUnsafe`, `SetComponent`, `GetEntitiesWith`, `GetComponentCount`) → `public` (the test project needs them; there is no cross-assembly `internal` story once it lives in the test project itself). `internal World()` ctor → `public ManagedTestWorld()`. `internal interface IRemovable` → `internal` is fine if it stays in the same assembly as `ManagedTestWorld`; widen to `public` only if a test references it directly.
- **Phase 0 sweep**: grep `src/` for every reference to `World` (the type). After the cutover, production has **zero** references to `World` — every one is either deleted (the `SystemExecutionContext`/`scheduler`/`GameBootstrap` references, handled in §4.2/§4.4/§4.6) or was never there. If a `src/` reference to `World` survives the cutover, it is a missed migration — halt+escalate.

```powershell
# Phase 0 — confirm the World references that must all be gone post-cutover
Select-String -Path src/ -Pattern '\bWorld\b' -Recurse
# Catalogue every hit. Each must be accounted for by §4.2/§4.4/§4.6 (deleted) or be a
# false positive (NativeWorld, a comment, a string). Post-Commit-2, re-run: zero
# production references to the bare `World` type.
```

---

## §5 — `DualFrontier.Core.Tests` mechanical rename (part of Commit 2, §4.1 step 9a)

`DualFrontier.Core.Tests` (~472 tests — confirm the exact count at Phase 0) tests the managed `World` directly: it constructs `new World()`, exercises `CreateEntity`/`AddComponent`/`DestroyEntity`/`FlushDestroyedEntities`/the deferred-destruction invariant. These tests are **not deleted** — `World` (as `ManagedTestWorld`) is still a real, tested ECS primitive. They migrate **mechanically**:

- Every `new World()` → `new ManagedTestWorld()`.
- Every `World` type reference (variable declarations, parameters, fields) → `ManagedTestWorld`.
- The `using DualFrontier.Core.ECS;` that imported `World` → import the `Fixtures` namespace of the test project where `ManagedTestWorld` now lives.
- No test *logic* changes — the assertions, the scenarios, the deferred-destruction tests are all still valid; `ManagedTestWorld` is `World` with a new name and wider visibility.

This is a find-and-replace of a type name across one test project. The risk is low but real: a too-broad replace could hit `NativeWorld` or a comment. Use a word-boundary-anchored replace and review the diff. The PowerShell-script lesson from A'.4.5 applies — **do not** run an unreviewed bulk script; if scripting the rename, dry-run first and read the diff.

**Phase 0 task**: list `tests/DualFrontier.Core.Tests/` in full, count the test files and the `World` reference sites, so the rename scope is known before Commit 2 starts.

---

## §6 — Commit shape + stop conditions

### §6.1 — The 4 commits

| # | Phase | Content | Gate before commit |
|---|---|---|---|
| 1 | Phase 0 | Revert `11c64e0^..02f9ecf` → tree at `b903b91` content (§3). Halt reports preserved. | `git diff b903b91 --stat` empty; `dotnet build` + `dotnet test` green (671). |
| 2 | Phase 4+5 merged | The entire storage cutover (§4 — power deletion, `SystemExecutionContext`/`SystemBase`/`scheduler` refactor, 10 system bodies, `GameBootstrap` + factories, `World`→`ManagedTestWorld`, `Core.Tests` rename, `Systems.Tests` delete+re-author). | `dotnet build DualFrontier.sln` clean; `dotnet test DualFrontier.sln` green (count changes — see §6.3). |
| 3 | Phase 6 | LOCKED-doc amendments (§9.2). | `dotnet build` still clean (docs don't affect build, but confirm nothing slipped). |
| 4 | Phase 6 | REGISTER.yaml closure + `sync_register.ps1 --validate` (§9.3). | `sync_register.ps1 --validate` passes. |

Commit 2 is the milestone. Commits 1, 3, 4 are bookends. There is no Commit 5+ — v1.0's 24-commit plan is gone (§1.4).

### §6.2 — Commit 2 message template

```
feat(kernel,systems,scenario): K8.3+K8.4 storage cutover — NativeWorld is the sole production storage path

The big-bang storage backend cutover. NativeWorld becomes the only
production component storage; the managed World is retired to the test
project as ManagedTestWorld. No dual-write, no transition phase — storage
backend is a binary property, the atomic unit is the whole cutover
(brief v2.0 §1.4).

Power subsystem deleted as disposable vanilla CPU systems (brief v2.0 §2):
- ElectricGridSystem, ConverterSystem removed from coreSystems + files
- EtherGridSystem (dead code) removed
- PowerConsumerComponent, PowerProducerComponent removed from Components
  + VanillaComponentRegistration.RegisterAll
- power events <list — deleted or kept per §2.4 sweep result>
- power tests removed
Electricity will be redesigned on the GPU compute pipeline in a separate
future brief.

Kernel:
- SystemExecutionContext: managed World removed; NativeWorld now required;
  isolation-guard runtime checks deleted (compile-time [SystemAccess] +
  future A'.9 analyzer enforce isolation — Q-COMBINED-2 Option A)
- SystemBase: GetComponent/SetComponent/Query/Query<T1,T2>/GetSystem deleted
- ParallelSystemScheduler: World ctor param removed; NativeWorld required

Systems (10 migrated): NeedsSystem, MoodSystem, JobSystem, ConsumeSystem,
SleepSystem, MovementSystem, PawnStateReporterSystem, InventorySystem,
HaulSystem, ComfortAuraSystem — bodies migrated Query/GetComponent/
SetComponent → NativeWorld AcquireSpan/BeginBatch. Game logic unchanged.
<note any system re-authored rather than migrated, with reason — §4.5>

Scenario: GameBootstrap native-only — new World() removed, dual-write loop
removed, factory + scheduler signatures drop the World parameter.
PublishItemSpawnedEvents + excludedPositions read from NativeWorld.

Tests: World moved to tests/DualFrontier.Core.Tests/Fixtures/ as
ManagedTestWorld (public, methods widened). Core.Tests migrated
mechanically (World → ManagedTestWorld). Systems.Tests deleted and
re-authored against NativeWorldTestFixture (brief v2.0 §7).

Build clean; <N> tests green.

Phase 4+5 of K8.3+K8.4 combined milestone v2.0. Commit 2 of 4.
```

### §6.3 — Test count expectations

The 671-test baseline at `b903b91` will **change** — expected, not a regression:
- `Core.Tests` (~472) — count unchanged (mechanical rename, same tests).
- `Systems.Tests` — deleted and re-authored. The re-authored count will differ from the original: power tests are gone (not re-authored), and the re-authored vanilla-mechanic tests are written against `NativeWorldTestFixture` covering post-cutover behaviour. The executor reports the new total in the Commit 2 message. The new total is **not** expected to equal 671 — it is expected to be *coherent*: every surviving system has test coverage, every test is green, no test clutches the managed `World`.
- The bar is not «same number» — it is «every test green, every surviving system covered, no managed-`World` coupling in `Systems.Tests`».

### §6.4 — Why there is no dual-write phase (the core of v2.0)

Stated plainly because it is the load-bearing decision: **v2.0 has no dual-write phase because storage backend is binary** (§1.4). v1.0's dual-write bridge existed to make a 12-commit incremental Phase 4 «work» — but the increments were never independently valid, so the bridge was load-bearing costyl, and the three halts were the structure rejecting it. Commit 2 changes the storage backend in one atom: before it, everything reads/writes the managed `World`; after it, everything reads/writes `NativeWorld`; in between, an incomplete commit that does not build. That is the honest shape. The factories mint entities on `nativeWorld` only; the systems read `nativeWorld` only; there is never a tick where two stores both hold live data. Drift is structurally impossible because there is only ever one store.

---

## §7 — Test strategy

### §7.1 — `DualFrontier.Core.Tests` — mechanical rename, NOT deleted

Covered in §5. Restated for the test-strategy picture: these tests have the managed `World` as their *subject*, they are legitimate and kept, they migrate by type-name rename to `ManagedTestWorld`.

### §7.2 — `DualFrontier.Systems.Tests` — deleted and re-authored

Crystalka direction 2026-05-14: vanilla mechanics are disposable; a test that constructs `new World()` and populates only the managed `World` is clutching the architecture being retired — migrating it is itself a costyl. So the entire `tests/DualFrontier.Systems.Tests/` project is **deleted** and **re-authored** against `NativeWorldTestFixture`.

What is deleted (verified on-disk listing):
- `Combat/Phase5BridgeAnnotationsTests.cs`
- `Inventory/CrossSystemMutationIsolationTests.cs`, `Inventory/HaulReservationTests.cs`
- `Pawn/ComfortAuraSystemTests.cs`, `Pawn/ConsumeSystemTests.cs`, `Pawn/NeedsAccumulationTests.cs`, `Pawn/NeedsJobIntegrationTests.cs`, `Pawn/SleepSystemTests.cs`
- `Power/ConverterEfficiencyTests.cs`, `Power/ElectricGridOverloadTests.cs` — **not re-authored** (power systems deleted, §2.5)
- `Fixtures/` — currently empty

What is re-authored: a test per surviving system (and the cross-system integration tests that still make sense post-cutover), against `NativeWorldTestFixture`. The re-authored tests verify **post-cutover behaviour** — the actual contract that survives. They do not attempt to preserve the old tests' internal structure. Coverage parity is the bar: every one of the 10 surviving systems has a test class; the autonomous-consume-loop, sleep-loop, needs-accumulation, mood, haul-reservation, and cross-system-mutation-isolation contracts that the old tests locked are re-expressed against `NativeWorld`.

**Phase 0 task**: read every file in `tests/DualFrontier.Systems.Tests/` before deleting — the executor must know what contracts the old tests locked, so the re-authored set covers the same contracts. The old tests are the *spec* for the new tests' coverage, even though the old code is discarded.

### §7.3 — `NativeWorldTestFixture`

The shared fixture for the re-authored `Systems.Tests`. Per patch v1 §6 (which v2.0 absorbs): the fixture **delegates to `VanillaComponentRegistration.RegisterAll`** — it does not hand-maintain a component-registration list. Shape:

```csharp
public sealed class NativeWorldTestFixture : IDisposable
{
    public NativeWorld NativeWorld { get; }
    public ComponentTypeRegistry Registry { get; }
    public GameServices Services { get; }

    public NativeWorldTestFixture()
    {
        // Same path production uses — Bootstrap.Run builds + binds the registry.
        NativeWorld = Bootstrap.Run(useRegistry: true);
        Registry = NativeWorld.Registry!;
        // Single source of truth — the SAME helper production calls.
        VanillaComponentRegistration.RegisterAll(Registry);
        Services = new GameServices();
    }

    public void Dispose() => NativeWorld.Dispose();
}
```

It lives at `tests/DualFrontier.Systems.Tests/Fixtures/NativeWorldTestFixture.cs` (the `Fixtures/` directory exists, currently empty). Confirm at Phase 0 the exact `Bootstrap.Run` / `NativeWorld.Registry` / `RegisterAll` signatures — patch v1 shipped them, they are on disk at `b903b91`.

The re-authored system tests build entities **on `NativeWorld`** via the fixture — the equivalent of the old `SpawnEatingPawn`/`SpawnFood` helpers, but writing to `NativeWorld` not the managed `World`. A test scheduler is still built (the old tests used `ParallelSystemScheduler` directly) — but with the cutover, the scheduler ctor no longer takes `World`, so the fixture's `NativeWorld` is what flows in.

### §7.4 — What stays green throughout

There is no «throughout» — Commit 2 is one atom. Tests are red *during* Commit 2's authoring (the tree does not compile mid-cutover) and green at Commit 2's completion. The gate is binary: Commit 2 is not committed until `dotnet test DualFrontier.sln` is green. Commit 1's gate (671 green at `b903b91` content) and Commit 2's gate (re-authored suite green) are the only two test gates.

---

## §8 — Stop conditions / escalation

Three halts have already happened on this milestone. Each was correct — the structure catching a bad premise before damage. v2.0 was authored from code to prevent a fourth, but if a premise is still wrong, **halt** — do not improvise. METHODOLOGY §3 «stop, escalate, lock».

Halt triggers specific to v2.0:

- **SC-1 — Phase 0 revert does not land at `b903b91`.** `git diff b903b91 --stat` non-empty after the revert (§3.2). Means the revert range was wrong or a deviation commit touched out-of-scope files. Halt, surface the diff.
- **SC-2 — Phase 0 floor is not green.** `dotnet build`/`dotnet test` fails on the reverted tree (§3.5). The cutover cannot start from an unverified floor.
- **SC-3 — deep-read contradiction.** Any §4.0 mandatory re-read surfaces a file shape that contradicts this brief (a signature changed, a method named differently, `NativeWorld` lacks an API the brief assumes). Halt, surface the contradiction — v2.0 was authored from a point-in-time read; drift since then is exactly what this trigger catches.
- **SC-4 — `NativeWorld` lacks a needed API.** The cutover assumes `NativeWorld` exposes span-acquire, batch-write, has-component, and the K8.1-primitive factory methods. If any is missing or shaped incompatibly, the cutover cannot proceed as specified — halt, because this would mean K8.1/K8.2 did not ship a surface this milestone depends on, which is a kernel finding, not something to work around.
- **SC-5 — a system cannot migrate AND cannot cleanly re-author.** §4.5's threshold says: migrate the body, or if it hits a managed-`World`-specific assumption, delete+re-author. If a system can do *neither* — its logic is structurally entangled with the managed `World` in a way that neither migration nor rewrite resolves within this milestone's scope — halt. That would be a finding about the system's design, worth surfacing.
- **SC-6 — power deletion sweep finds a non-power consumer.** §2.4: if a `DualFrontier.Events.Power` event is referenced by non-power code, halt — it means a power event leaked into non-power code, an architectural finding.
- **SC-7 — `ComponentStore` or `World` has an unexpected production consumer.** §4.7: if grepping `src/` surfaces a `World` or `ComponentStore` reference that none of §4.2/§4.4/§4.6 accounts for, halt — it is a missed migration path.
- **SC-8 — push-to-main classifier block.** Known behaviour (memory: Claude Code auto-mode classifier blocks push-to-main even with explicit instruction). Not a halt — expected. Re-confirm in-session after the work is done, then push. This is an operational reminder, not an escalation.

When halting: author a HALT_REPORT in `docs/scratch/A_PRIME_5_V2/`, state the trigger, state what was and was not committed, stop. Do not commit a partial Commit 2 — it does not compile, it cannot be committed anyway; that is the atomicity protecting the milestone.

---

## §9 — Closure protocol (Phase 6 — Commits 3 + 4)

METHODOLOGY §12.7 closure protocol. Phases 0-3's closure work was **already done** by patch v1's execution; v2.0's Phase 6 closes Phase 4+5 (the cutover) and reconciles the governance state that v1.0's «partial closure» left dangling.

### §9.1 — Commit 3: LOCKED-doc amendments

The cutover changes facts that LOCKED architecture documents assert. Amend them — each is a semantic amendment requiring the version bump its own protocol specifies.

| Document | Amendment | Version |
|---|---|---|
| `docs/architecture/KERNEL_ARCHITECTURE.md` | K-L11 (NativeWorld single source of truth) moves from «target» to «achieved». Part 2 K8.3/K8.4 rows → closed. Note the combined execution + the power-deletion. | v1.5 → v1.6 |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | K8.3+K8.4 rows → done; managed `World` retirement recorded; `coreSystems` count 12→10 with power deletion noted; the K8.3/K8.4 ordering history (swap, then combine) recorded. | v1.1 → v1.2 |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | If it asserts anything about the managed `World` as production storage or about a 12-system core set — amend. **Phase 0 task**: grep it for `World`, `coreSystems`, `12 systems`, `ElectricGrid`, `Converter`, `Power` and amend each stale assertion. If nothing is stale, no version bump. | v1.7 → v1.8 only if amended |
| `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` | A'.5 → closed (combined K8.3+K8.4 via brief v2.0). The A'.5→A'.6→A'.7 sequence: A'.5 absorbed both K8.3 and K8.4, so A'.6 = K8.5, A'.7 = Roslyn analyzer (or per the current sequencing doc — read it at Phase 0 and reconcile). Note: a new milestone «electricity on GPU compute» is now on the horizon — record it as a horizon item, not yet sequenced. | per its own protocol |
| `docs/METHODOLOGY.md` | Add Lesson #7 + Lesson #8 to §K-Lessons (§9.4). | v1.7 → v1.8 |

**Phase 0 task**: read each of these documents and locate the exact stale assertions before Commit 3. Do not amend from this table alone — the table says *what* changed, the documents say *where*. If an amendment touches a passage another in-flight item also touches, note it; do not create a conflict.

Also fold in any stragglers §2.7 deferred (stale «electricity implemented» claims in architecture docs — amend to «electricity deferred to a GPU-compute brief»).

### §9.2 — Commit 3: stale-entry amendments inherited from the A'.1 backlog

Memory records four stale items scheduled for amendment («K8.2 v2 closure entry, K-L3 wording, MOD_OS lines 1149–1150, MIGRATION_PLAN line 62»). If those are still open at v2.0 closure and they intersect the documents Commit 3 touches anyway, fold them into Commit 3 — it is cheaper to amend once. If they do not intersect, leave them for their own A'.1 batch. **Phase 0 task**: check whether A'.1 has run; if not, check intersection.

### §9.3 — Commit 4: REGISTER.yaml closure

Per the mandatory post-A'.4.5 protocol — `sync_register.ps1 --validate` is mandatory.

Lifecycle transitions:
```yaml
# v2.0 brief itself
- id: DOC-D-K8_34_COMBINED_V2
  lifecycle: EXECUTED          # AUTHORED → EXECUTED at closure
  version: "2.0"

# v1.0 brief + patch v1 — superseded by v2.0
- id: DOC-D-K8_34_COMBINED
  lifecycle: SUPERSEDED        # superseded_by: DOC-D-K8_34_COMBINED_V2
- id: DOC-D-K8_34_COMBINED_BRIEF_REFRESH_PATCH
  lifecycle: SUPERSEDED        # superseded_by: DOC-D-K8_34_COMBINED_V2
```

Note on the supersession: v1.0 + patch v1 are marked SUPERSEDED, **not** deprecated-as-wrong. Their Phases 0-3 *executed correctly and are on disk* — v2.0 builds on that work. They are superseded because v2.0 is now the single coherent execution artifact; the pair-of-documents form is replaced by one. The audit trail preserves them.

CAPA closures:
- **CAPA-2026-05-13-K8.3-PREMISE-MISS** — OPEN since the storage-location halt. Memory says it «closes at A'.7». Re-evaluate: it was opened for the storage-premise miss; v2.0's clean execution of the storage cutover is the effectiveness evidence. If v2.0 closes cleanly, this CAPA can close here — confirm against its `effectiveness_verification` criteria at Phase 0.
- **CAPA-2026-05-14-K8.34-API-SURFACE-MISS** — opened by patch v1. Its corrective action (read API before prescribing it) is exactly what v2.0's §4.0 mandatory-re-read encodes. Closes at v2.0 closure if the cutover executed without an API-surface halt.
- **New: CAPA-2026-05-14-K8.34-MID-TRANSITION-DRIFT** — opened for the third halt. Trigger: v1.0 §6.5 split the storage cutover into 12 incremental commits; the increments were never independently valid; the dual-write bridge between them was load-bearing costyl. Root cause: brief authored the Phase 4+5 *shape* from architecture docs without simulating the intermediate per-commit states. Corrective action: v2.0 re-designed Phase 4+5 as a single atomic cutover (§1.4, §6.4); Lesson #8 added to METHODOLOGY. Closes at v2.0 closure.
- **CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT** — if still open, v2.0 closure is another post-register milestone providing effectiveness evidence. Check at Phase 0.

Audit-trail entry: `EVT-<date>-K8.3-K8.4-COMBINED-V2-CLOSURE` — records the 4-commit execution, the supersession of v1.0+patch v1, the power deletion, the three-CAPA closure, the 10-system cutover. `documents_affected` lists every doc Commit 3 amended + the three brief documents.

REGISTER count: the power-component deletion removes types; if REGISTER tracks component-level entries (it tracks documents, so likely not — confirm at Phase 0), reconcile. The brief-document count: +1 (v2.0) with 2 transitioning to SUPERSEDED.

### §9.4 — Lesson #7 + Lesson #8 (METHODOLOGY §K-Lessons)

Both deferred to this closure (memory notes Lesson #6 already pending a batch revision — Lessons #7 and #8 join it, or go in their own v1.7→v1.8 bump per §9.1).

**Lesson #7 — A brief that prescribes an API must transcribe the API, not paraphrase it.** When a brief tells the executor to call a constructor, a helper, or a file path, the brief author must open the actual source and copy the real signature into the brief at authoring time. «K2-era registry ready» is a note, not a signature. CAPA-2026-05-13's lesson («read entry-point files in full») addressed transitional-state comments; Lesson #7 addresses *API surface*. A brief is a contract for mechanical execution; a contract cannot reference an interface it has not read. (Originated in patch v1; formalized here.)

**Lesson #8 — A brief that splits a change into N steps must prove each of the N−1 intermediate states is valid.** v1.0 split the storage cutover into 12 commits and never simulated what the data looked like between commit 9 and commit 20 — the answer was «two stores diverging per tick», and the dual-write bridge was the costyl that hid it. Before a brief prescribes an incremental sequence, the author must walk each intermediate state and confirm it compiles, passes tests, and is architecturally coherent. If an intermediate state cannot be made valid, the change is not incrementally divisible — the atomic unit is larger than one step, and the brief must say so. Storage-backend cutover is the canonical example: it is binary, the atom is the whole thing. (Originated in the third halt; formalized here.)

Corollary to both, for METHODOLOGY: **a brief cannot promise «zero halts»; it can promise «halts before damage».** Three halts on this milestone, zero harmful commits — that is the system working. A brief's honest guarantee is that bad premises surface at Phase 0 / at deep-read / at the compile gate, before they reach `main`.

---

## §10 — Estimated commit log

```
1. revert(a-prime-5): undo v1.0 Phase 4-6 deviation — return to end-of-Phase-3 (b903b91)
2. feat(kernel,systems,scenario): K8.3+K8.4 storage cutover — NativeWorld is the sole production storage path
3. docs(architecture,methodology): A'.5 closure — LOCKED-doc amendments + Lesson #7/#8
4. docs(governance): A'.5 closure — REGISTER.yaml lifecycle + 3 CAPA closures + audit trail

Total: 4 commits.
```

Contrast with v1.0's estimated 24. The reduction is not optimization — it is the correct count. The storage cutover is one atom (§1.4); the bookends are revert + 2 closure commits. v1.0's 24 was the artifact of treating an indivisible change as 12 divisible steps.

Execution time estimate: Commit 2 is large — 10 system bodies + 3 kernel files + `GameBootstrap` + 2 factories + `World` move + `Core.Tests` rename + `Systems.Tests` re-author. Realistically a long single session or two. The atomicity does not allow it to be split across commits, but the *authoring* of Commit 2 can be paced — the executor builds the whole thing, gets it compiling and green, then commits once. If the session runs out before Commit 2 compiles, that is not a partial commit — it is uncommitted work; the next session resumes it. Only a compiling, green tree gets committed.

---

## §11 — Provenance / relationship to superseded artifacts

v2.0 supersedes:
- `K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md` (v1.0, 2026-05-13) — its Q-COMBINED locks 1-8 are carried forward unchanged (§0); its Phases 0-3 executed correctly and are on disk; its Phase 4+5 design is replaced (§4).
- `K8_34_COMBINED_BRIEF_REFRESH_PATCH.md` (patch v1, 2026-05-14) — its 5 findings were resolved and executed (Phases 0-3 on disk at `b903b91`); v2.0 absorbs its corrections as executed-state fact and carries its `NativeWorldTestFixture`-delegates-to-`RegisterAll` decision (§7.3) and its Lesson #7 (§9.4).

Neither superseded document is «wrong» — they are superseded because v2.0 is now the single coherent artifact. The lineage:

```
2026-05-13  K8.3 v2.0 brief — halt 1 (storage-location premise)
2026-05-13  combined brief v1.0 — absorbed K8.3+K8.4
2026-05-14  combined v1.0 execution — halt 2 (API-surface premise) — patch v1 authored
2026-05-14  patch v1 execution — Phases 0-3 shipped clean (b903b91); Phase 4 — halt 3 (mid-transition drift)
2026-05-14  [a session continued past halt 3 with a hybrid dual-write — 02f9ecf —
             reverted by v2.0 Commit 1 as a deviation carrying no architectural value]
2026-05-14  deliberation session — full deep-read of systems/scheduler/tests;
             big-bang cutover designed; v2.0 authored from code
2026-XX-XX  v2.0 execution — 4 commits — A'.5 closure
```

The «session continued past halt 3» (commits `11c64e0..02f9ecf`) is the deviation §1.3 describes and §3 reverts. It is part of the honest history — recorded, not hidden — but it carries no architectural value and v2.0 does not build on it.

---

## §12 — Reading sign-off

Before executing, the executor confirms:

1. **This is v2.0** — not v1.0, not patch v1. Those two are SUPERSEDED and on disk for history only.
2. **The starting state is `b903b91`** — Phase 0 (Commit 1) reverts to it; if `git diff b903b91 --stat` is not empty after the revert, halt (SC-1).
3. **Phases 0-3 are not re-done** — they are on disk, executed, 671 tests green. v2.0's work is Commit 1 (revert the deviation) + Commit 2 (the cutover) + Commits 3-4 (closure).
4. **Commit 2 is one atom** — it does not compile until complete, it is not committed until `dotnet test DualFrontier.sln` is green. There is no partial Commit 2.
5. **§4.0's mandatory re-reads happen first** — v2.0 was authored from a point-in-time read; the executor confirms nothing drifted before writing a line of Commit 2. A contradiction is a halt (SC-3).
6. **The power subsystem is deleted, not migrated** — 10 systems, not 12. Electricity is a separate future brief.
7. **`Systems.Tests` is deleted and re-authored; `Core.Tests` is mechanically renamed** — the two test projects have opposite dispositions for a reason (§7).
8. **Halt is success, not failure** — if a premise is still wrong despite v2.0 being authored from code, halt per §8. Three halts have already protected this milestone from a bad commit; a fourth would do the same.

The Q-COMBINED locks (§0) are ratified and unchanged. v2.0 changes the *execution shape* of Phase 4+5, not the architecture. The architecture was never the problem — the execution shape was, and v2.0 fixes it: one atom for one indivisible change.

**Brief end. Execution begins at §3 (Phase 0 — the revert), preceded by §4.0's mandatory deep-reads.**

