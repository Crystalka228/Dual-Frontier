---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K8_3
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "2.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_3
---
# K8.3 — Production system migration to SpanLease/WriteBatch (Path α exclusive at K8.3 time)

**Status**: AUTHORED v2.0 (replaces v1.0 skeleton 2026-05-10; full A'.4.5-standard brief authored 2026-05-13 post-K-L3.1 reframing + post-A'.4.5 governance integration)
**Phase A' position**: A'.5 K8.3 (first post-register milestone; CAPA-2026-05-12 effectiveness verification target)
**Prerequisite**: K8.2 v2 closure (DONE 2026-05-09, commits `547c919..7527d00`) — production components are unmanaged structs on Path α OR deleted per METHODOLOGY §7.1 (no production class-shape components survive K8.2 v2)
**Authority subordinate to**:
- `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.1 LOCKED §0.3 Decision #4 (K8.3 reformulated scope) + §1.2 (architectural design constraints) + §0.3 Decision #9 (K-L3.1 dual-path access mechanism)
- `docs/architecture/KERNEL_ARCHITECTURE.md` v1.5 LOCKED Part 0 K-L3 (post-K-L3.1 reframing) + K-L7 (span protocol) + K-L11 (NativeWorld backbone) + Part 2 K8.3 row
- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` §2 (Phase A' position)
- `docs/methodology/METHODOLOGY.md` v1.7 §12.7 (canonical closure protocol post-A'.4.5)
- `docs/governance/FRAMEWORK.md` v1.0 §6 (post-session protocol Q-A45-X5)
**Estimated time**: 4-6 weeks at hobby pace, ~12-20 hours auto-mode
**Estimated atomic commits**: 14-18 (12 system migrations + 1 test fixture + 1 closure measurement + 1-3 ancillary)

---

## §0 Pre-deliberation locks

This brief carries 5 ratified Q-locks from chat deliberation 2026-05-13 (Crystalka acceptance «Принимаю рекомендации пиши полный бриф по ним»). The locks resolve ambiguities surfaced by K8.3 consistency review against K-L3.1 reframing + A'.4.5 governance integration.

### §0.1 — Q-K8.3-1 LOCKED: scope authoritative count

**Locked answer**: (c) primary + (b) backstop — `GameBootstrap.coreSystems` actual source is the truth; `directory_tree` inventory is the audit backstop; MIGRATION_PLAN v1.1 «34 systems» wording reconciled via amendment within K8.3.

**Verified inventory at brief authoring time (2026-05-13)**:

**12 systems in GameBootstrap.coreSystems** (Application/Loop/GameBootstrap.cs line 138-152):
| # | System | Slice | Source file |
|---|---|---|---|
| 1 | NeedsSystem | Pawn | `src/DualFrontier.Systems/Pawn/NeedsSystem.cs` |
| 2 | MoodSystem | Pawn | `src/DualFrontier.Systems/Pawn/MoodSystem.cs` |
| 3 | JobSystem | Pawn | `src/DualFrontier.Systems/Pawn/JobSystem.cs` |
| 4 | ConsumeSystem (M8.5) | Pawn | `src/DualFrontier.Systems/Pawn/ConsumeSystem.cs` |
| 5 | SleepSystem (M8.6) | Pawn | `src/DualFrontier.Systems/Pawn/SleepSystem.cs` |
| 6 | ComfortAuraSystem (M8.7) | Pawn | `src/DualFrontier.Systems/Pawn/ComfortAuraSystem.cs` |
| 7 | MovementSystem | Pawn | `src/DualFrontier.Systems/Pawn/MovementSystem.cs` |
| 8 | PawnStateReporterSystem | Pawn | `src/DualFrontier.Systems/Pawn/PawnStateReporterSystem.cs` |
| 9 | InventorySystem | Inventory | `src/DualFrontier.Systems/Inventory/InventorySystem.cs` |
| 10 | HaulSystem | Inventory | `src/DualFrontier.Systems/Inventory/HaulSystem.cs` |
| 11 | ElectricGridSystem | Power | `src/DualFrontier.Systems/Power/ElectricGridSystem.cs` |
| 12 | ConverterSystem | Power | `src/DualFrontier.Systems/Power/ConverterSystem.cs` |

**14 systems present in `src/DualFrontier.Systems/**/*.cs` but NOT in coreSystems** (defer to M-series or delete):
- Combat slice: CombatSystem, ComboResolutionSystem, CompositeResolutionSystem, DamageSystem, ProjectileSystem, StatusEffectSystem (6 — full slice deferred)
- Faction slice: RaidSystem, RelationSystem, TradeSystem (3 — full slice deferred)
- Magic slice: EtherGrowthSystem, GolemSystem, ManaSystem, RitualSystem, SpellSystem (5 — full slice deferred)
- Power slice: EtherGridSystem (1 — magic-coupled, defer to M10.B)

**3 orphan `.uid` files without `.cs`** (Godot import metadata for deleted systems):
- `src/DualFrontier.Systems/Combat/ShieldSystem.cs.uid`
- `src/DualFrontier.Systems/Pawn/SocialSystem.cs.uid`
- `src/DualFrontier.Systems/World/BiomeSystem.cs.uid`

Each `.uid` is artifact of K8.2 v2 closure («6 empty TODO stub deletions per METHODOLOGY §7.1») where `.cs` was deleted but `.uid` survived. **METHODOLOGY §7.1 11-th invocation** applies: data does not exist for these `.uid` files; they are placeholder lies.

**K8.3 SCOPE LOCKED**: **12 production systems** (GameBootstrap.coreSystems verbatim). The «34 systems» wording in MIGRATION_PLAN v1.1 §0.3 Decision #4 + §1.2 is amended via §8.3 of this brief (in-scope documentation amendment within K8.3 execution).

**Rationale**: K-L9 «vanilla = mods» principle. Production = what scheduler tickas = `coreSystems`. The other 14 .cs files exist but are not loaded — they are M-series migration source (per MIGRATION_PLAN §3.1-§3.6 per-slice mapping). Migrating them as part of K8.3 would be **wasted work** — М-series will relocate them to `mods/Vanilla.<Slice>/` where they would be **re-migrated** during M-milestone authoring. Single migration pass per file (precedent: Migration Plan §0.2 «No architectural work happens in two places»).

**The 14 not-in-coreSystems files**: per Migration Plan §3.3-§3.5, these migrate during M9 (Combat + Faction folded), M10 (Inventory + Power folded — though EtherGridSystem may go to M10.B Magic), M10.B (Magic). They migrate **directly into vanilla mod assemblies** in M-series, **using SpanLease/WriteBatch from day one** (because K8.3 + K8.4 ship the API surface they consume). No interim K8.3 migration.

**The 3 orphan .uid files**: deleted in Phase 1 of K8.3 execution (per METHODOLOGY §7.1 11-th invocation cleanup).

### §0.2 — Q-K8.3-2 LOCKED: Path β audit disposition

**Locked answer**: (b) — no production components on Path β; K8.3 implements Path α exclusively; K8.4 ships Path β plumbing (`RegisterManagedComponent<T>` + `SystemBase.ManagedStore<T>` resolver) for future use.

**Verification protocol at K8.3 execution time** (Phase 0.4 pre-flight):

```powershell
# Search for [ManagedStorage] attribute usage on production components
Get-ChildItem -Path src/DualFrontier.Components -Recurse -Filter *.cs |
  Select-String -Pattern '\[ManagedStorage\]' -SimpleMatch
# Expected result: zero matches at K8.3 brief authoring time
```

**Brief authoring expectation**: zero matches. K8.2 v2 closure delivered Path α for all 31 production components (6 converted, 6 deleted, 19 already-struct verify-only). No production `[ManagedStorage]` opt-in exists at K8.3 time.

**Implication for K8.3 implementation**: every system in coreSystems reads/writes Path α components exclusively. SpanLease/WriteBatch pattern applies uniformly. `SystemBase.ManagedStore<T>()` accessor is **not called** in any production system at K8.3 time. K8.3 brief implements only Path α access pattern; K8.4 brief (separate milestone) authors `RegisterManagedComponent<T>` + `ManagedStore<T>` accessor as Path β plumbing.

**What K8.4 ships for Path β** (NOT K8.3 scope; documented here for sequencing clarity):
- `IModApi.RegisterManagedComponent<T> where T : class, IComponent` (Mod API v3 surface)
- `RestrictedModApi.ManagedStore<T>` instance per-mod (held in mod's RestrictedModApi)
- `SystemBase.ManagedStore<T>()` accessor (resolves via `SystemExecutionContext.Current.ModId` to owning mod's store)
- `[ManagedStorage]` attribute definition + capability-model integration

**K8.3 verification step**: Phase 5.3 of execution re-runs the `[ManagedStorage]` grep and asserts zero matches. If matches found (someone added Path β between brief authoring and execution), brief halts; surface as deviation; chat session escalation per METHODOLOGY §3 «stop, escalate, lock».

### §0.3 — Q-K8.3-3 LOCKED: test fixture strategy

**Locked answer**: (A) per-system test class — precedent from K8.2 Q3 (per-component test class locked in K8.2 brief authoring); precedent stability preserves architectural cleanness.

**Implementation**: 12 new test classes in `tests/DualFrontier.Tests/Systems/<Slice>/<SystemName>Tests.cs`:

| Test class | Location | Tests scope |
|---|---|---|
| `NeedsSystemTests.cs` | `tests/DualFrontier.Tests/Systems/Pawn/` | SpanLease<NeedsComponent> reads, WriteBatch<NeedsComponent> writes, decay semantics |
| `MoodSystemTests.cs` | `tests/DualFrontier.Tests/Systems/Pawn/` | SpanLease<MoodComponent> reads, multi-component derivation logic |
| `JobSystemTests.cs` | `tests/DualFrontier.Tests/Systems/Pawn/` | Job assignment via WriteBatch, JobKind enum coverage |
| `ConsumeSystemTests.cs` | `tests/DualFrontier.Tests/Systems/Pawn/` | Multi-read (Position + Needs + Consumable + WaterSource); WriteBatch reducer; entity destruction on consume |
| `SleepSystemTests.cs` | `tests/DualFrontier.Tests/Systems/Pawn/` | Bed reservation lifecycle, Comfort+Sleep coupled restoration (hybrid formula from userMemories) |
| `ComfortAuraSystemTests.cs` | `tests/DualFrontier.Tests/Systems/Pawn/` | DecorativeAuraComponent radius query, ambient Comfort delta |
| `MovementSystemTests.cs` | `tests/DualFrontier.Tests/Systems/Pawn/` | Path traversal via WriteBatch<MovementComponent>; A* integration; path step indexing |
| `PawnStateReporterSystemTests.cs` | `tests/DualFrontier.Tests/Systems/Pawn/` | Multi-read pawn snapshot, event publication ordering |
| `InventorySystemTests.cs` | `tests/DualFrontier.Tests/Systems/Inventory/` | Storage component read, item pickup via WriteBatch |
| `HaulSystemTests.cs` | `tests/DualFrontier.Tests/Systems/Inventory/` | Multi-read (Position + Inventory + Reservation); hauling logic |
| `ElectricGridSystemTests.cs` | `tests/DualFrontier.Tests/Systems/Power/` | PowerProducer + PowerConsumer span coordination |
| `ConverterSystemTests.cs` | `tests/DualFrontier.Tests/Systems/Power/` | Workbench state machine via WriteBatch |

**Shared test fixture**: `NativeWorldTestFixture` in `tests/DualFrontier.Tests/Fixtures/NativeWorldTestFixture.cs`:

```csharp
public sealed class NativeWorldTestFixture : IDisposable
{
    public NativeWorld World { get; }
    public ComponentTypeRegistry Registry { get; }

    public NativeWorldTestFixture(params Type[] componentTypes)
    {
        Registry = new ComponentTypeRegistry();
        World = new NativeWorld(Registry);
        foreach (var t in componentTypes) Registry.RegisterByType(t);
    }

    public EntityId SpawnEntity() => World.CreateEntity();

    public void Dispose() => World.Dispose();
}
```

**Estimated test count delta**: +60 to +90 tests (5-8 tests per system × 12 systems). Baseline depends on K9 closure test count; K8.3 closure includes total test count in MIGRATION_PROGRESS entry per §12.7 closure protocol.

**Rationale for (A) over (B) or (C)**:
- **Isolation property**: per-system test class isolates failure surface — if NeedsSystem regresses, NeedsSystemTests fails; no cross-system pollution
- **K8.2 precedent**: per-component test class established 2026-05-09; K8.3 per-system maintains pattern symmetry
- **Discoverability**: agent navigating `tests/DualFrontier.Tests/Systems/Pawn/` immediately sees per-system test surface
- **Atomic commit alignment**: each per-system migration commit includes its per-system test class — verifiable per-commit (precedent K-Lessons #1 atomic commit as compilable unit)
- **Future M-series migration**: when M8.5-M8.7 relocate Pawn systems to `mods/DualFrontier.Mod.Vanilla.Pawn/`, the per-system tests move with them (atomic relocation per Migration Plan §5.1)

### §0.4 — Q-K8.3-4 LOCKED: system migration ordering

**Locked answer**: (d) rule-based; Claude Code applies rule at execution time based on actual per-system read/write surface. Rule (K8.2 Q-K8.2-Ordering precedent extended):

**Tier 1 — single-component read, single-component write** (lowest interleave risk):
- Candidates: NeedsSystem (reads Needs, writes Needs), MoodSystem (reads multiple but writes Mood only), JobSystem (writes Job only)
- Execute first because SpanLease/WriteBatch pattern interaction is simplest

**Tier 2 — multi-component reads, single-component write**:
- Candidates: PawnStateReporterSystem (reads many, writes none — pure publisher), ConverterSystem (reads Workbench + Storage, writes Workbench)

**Tier 3 — multi-component read + multi-component write**:
- Candidates: ConsumeSystem (reads Position + Needs + Consumable + WaterSource, writes Needs + destroys entities), HaulSystem (similar shape)

**Tier 4 — cross-slice coupling** (highest interleave risk):
- Candidates: ElectricGridSystem (reads PowerProducer + PowerConsumer, writes grid state — cross-component coordination), SleepSystem (reads Bed + Reservation + Needs, writes Needs + Reservation — coupling with InventorySystem reservation pattern), MovementSystem (reads Movement + NavGrid, writes Movement + Position; A* coupling)

**Tier 5 — special semantics**:
- ComfortAuraSystem (radius query via spatial primitive — verify K9 RawTileField integration not required at K8.3; aura is component-based per K8.2 closure)

**Within-tier alphabetical** for deterministic execution order.

**Application rule for Claude Code agent**:
```
1. Read each system .cs file
2. Identify read components via `world.GetComponents<T>()` / `SpanLease<T>` call sites
3. Identify write components via mutation call sites (`SetComponent<T>`, `WriteBatch`)
4. Classify into Tier 1-5 per rule above
5. Migrate Tier 1 systems first, atomic per-system commit, all tests pass
6. Tier 2 next, etc.
7. Within tier: alphabetical
```

**Estimated tier distribution** (verifiable at execution time):
- Tier 1: ~3 systems (NeedsSystem, MoodSystem, JobSystem)
- Tier 2: ~2 systems (PawnStateReporterSystem, ConverterSystem)
- Tier 3: ~2 systems (ConsumeSystem, HaulSystem)
- Tier 4: ~3 systems (ElectricGridSystem, SleepSystem, MovementSystem)
- Tier 5: ~1 system (ComfortAuraSystem)
- Plus 1 system not fitting clean tier (InventorySystem — reads Inventory, writes Inventory and reservations; classify at execution)

**Rationale for (d) over (a)/(b)/(c)**:
- **(a) slice order**: arbitrary — Combat/Faction slices excluded from K8.3 entirely, so slice doesn't carry signal
- **(b) complexity order**: subset of rule-based — rule formalizes the implicit ordering
- **(c) dependency order**: GameBootstrap.coreSystems IS the dependency order (DependencyGraph builds from this array), but execution order ≠ migration order; migration order is about interleave risk during refactor, not runtime ordering

### §0.5 — Q-K8.3-5 LOCKED: K8.3 closure protocol exercise scope

**Locked answer**: (a) standard exercise — full METHODOLOGY §12.7 closure protocol invocation; lifecycle AUTHORED → EXECUTED in REGISTER.yaml; new audit_trail entry; CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT closure with effectiveness verification if validation passes clean.

Plus **lightweight (b) data collection passive layer**: register's audit_trail collection structurally records commit ranges + dates per closure — this is **automatic**, not new scope. K8.3 closure produces first v1.7-era data point (METHODOLOGY v1.7 = post-A'.4.5).

**K8.3 closure protocol exercise** (METHODOLOGY §12.7 canonical):

1. **Final verification** (existing): `dotnet build` clean, `dotnet test` all passing, native selftest passing, F5 verification (Godot launch)
2. **Atomic commit** with `feat(systems):` scope prefix per K8.3 migration
3. **Update MIGRATION_PROGRESS.md**: K8.3 closure entry with SHA, date, brief reference, deviations, test count delta
4. **Update brief Status**: change frontmatter `lifecycle: AUTHORED` → `lifecycle: EXECUTED`; append «K8.3 Lessons learned» section к bottom of THIS brief
5. **NEW (Q-A45-X5)**: Update REGISTER.yaml entries
   - DOC-D-K8_3 lifecycle: AUTHORED → EXECUTED + last_modified_commit + governance_events list extended
   - DOC-A-MIGRATION_PLAN: amend §1.2 «34 systems» wording (see §8.3 of this brief for exact amendment text); register entry version bump v1.1 → v1.2 (non-semantic correction)
   - DOC-A-KERNEL: amend Part 2 K8.3 row scope; version bump v1.5 → v1.6 (non-semantic correction)
   - Per-modified production system .cs file: register entry already exists at Tier 4 Category F; last_modified_commit updated automatically via sync_register.ps1 --sync
6. **NEW (Q-A45-X5)**: Append EVT-2026-XX-XX-K8.3-CLOSURE entry to audit_trail collection
7. **NEW (Q-A45-X5)**: Update CAPA entries
   - CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT: if sync validation passes clean → mark CLOSED with effectiveness verification (verification_commit = K8.3 closure commit; verification_outcome = «sync_register.ps1 --validate green at K8.3 closure; register doesn't degrade post-bootstrap»)
   - If validation surfaces register orphans/drift → CAPA remains OPEN; halt closure; surface as deviation
8. **NEW (Q-A45-X5)**: Run `sync_register.ps1 --validate` — must exit 0 before final commit; if non-zero, halt closure
9. **Update RISK entries** (effectiveness measurement):
   - RISK-010 (register degradation): mitigation_status PARTIAL → APPLIED if K8.3 protocol exercise clean
   - RISK-007 (brief staleness density): N+1-th data point — K8.3 brief authored fresh against post-A'.4.5 state; precedent strengthens
10. **Update REQ entries**: REQ-Q-A45-X5 verification_status PENDING → VERIFIED (K8.3 is first real-world exercise of protocol); add verification_milestone = «K8.3 closure»
11. **Push to origin/main** per A'.3 precedent

**This is the operational test of post-A'.4.5 register infrastructure**. K8.3 closure proves (or falsifies) that protocol holds under real milestone load. If protocol breaks at K8.3 closure (e.g., tooling fails, register doesn't sync, validation false-positives) — this is structurally important falsifying evidence; surface, escalate, address before K8.4.

---

## §1 Goal

Migrate **12 production systems** in `GameBootstrap.coreSystems` from `World.GetComponent` / `World.SetComponent` per-call access pattern to **Path α SpanLease/WriteBatch pattern** per K-L7 (read-only spans + write command batching).

**Why now**: post-K8.2 v2 closure (DONE 2026-05-09), all production components are unmanaged structs (Path α). Production systems still use legacy managed `World` access pattern. K8.3 closes the gap — systems consume their structs via zero-P/Invoke spans + batched writes per K5 protocol.

**Post-K8.3 state**:
- Each production system reads via `using var lease = SystemBase.NativeWorld.AcquireSpan<TComponent>();` per K5 protocol
- Each production system writes via `var batch = new WriteBatch<TComponent>(); batch.Add(entity, c); SystemBase.NativeWorld.FlushWrites(batch);`
- Each production system handles dual-component reads via multiple SpanLease<T> acquisitions (one per T)
- Mutation rejection (K5 invariant): any system holding a span and attempting to write the same component type without releasing first triggers `IsolationViolationException`
- `World.GetComponent<T>()` and `World.SetComponent<T>(...)` removed from production system call sites
- Test fixture `NativeWorldTestFixture` shared across all 12 system tests
- Performance verification: K8.3 production-on-NativeWorld tick loop matches K7 V3 numbers within 20% (sanity check)

**Path β explicitly not implemented**: Q-K8.3-2 LOCKED — zero production Path β components at K8.3 time. K8.4 ships Path β plumbing as separate milestone.

---

## §2 Scope inventory

Per Q-K8.3-1 LOCKED:

### §2.1 — 12 production systems (in scope)

(Full table in §0.1 above. Restated here for execution agent reference.)

**Pawn slice (8 systems)**:
1. NeedsSystem — `src/DualFrontier.Systems/Pawn/NeedsSystem.cs`
2. MoodSystem — `src/DualFrontier.Systems/Pawn/MoodSystem.cs`
3. JobSystem — `src/DualFrontier.Systems/Pawn/JobSystem.cs`
4. ConsumeSystem — `src/DualFrontier.Systems/Pawn/ConsumeSystem.cs`
5. SleepSystem — `src/DualFrontier.Systems/Pawn/SleepSystem.cs`
6. ComfortAuraSystem — `src/DualFrontier.Systems/Pawn/ComfortAuraSystem.cs`
7. MovementSystem — `src/DualFrontier.Systems/Pawn/MovementSystem.cs`
8. PawnStateReporterSystem — `src/DualFrontier.Systems/Pawn/PawnStateReporterSystem.cs`

**Inventory slice (2 systems)**:
9. InventorySystem — `src/DualFrontier.Systems/Inventory/InventorySystem.cs`
10. HaulSystem — `src/DualFrontier.Systems/Inventory/HaulSystem.cs`

**Power slice (2 systems)**:
11. ElectricGridSystem — `src/DualFrontier.Systems/Power/ElectricGridSystem.cs`
12. ConverterSystem — `src/DualFrontier.Systems/Power/ConverterSystem.cs`

### §2.2 — Out of K8.3 scope (defer to M-series)

**Combat slice (6 systems)** — full slice deferred to M9 Vanilla.Combat:
- CombatSystem, ComboResolutionSystem, CompositeResolutionSystem, DamageSystem, ProjectileSystem, StatusEffectSystem

**Faction slice (3 systems)** — full slice deferred to M9 (Faction folds under Combat per Migration Plan §3.3):
- RaidSystem, RelationSystem, TradeSystem

**Magic slice (5 systems)** — full slice deferred to M10.B Vanilla.Magic:
- EtherGrowthSystem, GolemSystem, ManaSystem, RitualSystem, SpellSystem

**Power slice tail (1 system)** — magic-coupled, defer to M10.B:
- EtherGridSystem

**Total deferred to M-series**: 15 systems (6 + 3 + 5 + 1)

**Rationale per K-L9 «vanilla = mods»**: these 15 systems are not loaded by production scheduler. Migrating them in K8.3 would force re-migration during M-milestones (relocation to vanilla mod assembly), violating Migration Plan §0.2 «No architectural work happens in two places».

### §2.3 — Orphan .uid cleanup (Phase 1 of K8.3 execution)

3 orphan `.uid` files without corresponding `.cs` (Godot import metadata for deleted systems):
- `src/DualFrontier.Systems/Combat/ShieldSystem.cs.uid`
- `src/DualFrontier.Systems/Pawn/SocialSystem.cs.uid`
- `src/DualFrontier.Systems/World/BiomeSystem.cs.uid`

Each is artifact of K8.2 v2 «6 empty TODO stub deletions per METHODOLOGY §7.1». `.cs` deleted; `.uid` survived as Godot metadata.

**METHODOLOGY §7.1 11-th invocation**: data does not exist for these .uid files; the corresponding component/system was deleted because empty placeholder. The .uid is also a placeholder lie at meta-level.

**K8.3 Phase 1 cleanup**: delete these 3 .uid files in single atomic commit `chore(systems): cleanup orphan .uid files from K8.2 v2 stub deletions`. Verify no references remain via grep.

---

## §3 Architectural design constraints (inherited from MIGRATION_PLAN v1.1 §1.2)

### §3.1 — Read pattern (K-L7 + K5 protocol)

```csharp
public sealed class NeedsSystem : SystemBase
{
    public override void Update(SystemExecutionContext context)
    {
        using var lease = context.NativeWorld.AcquireSpan<NeedsComponent>();
        var writeBatch = new WriteBatch<NeedsComponent>();

        for (int i = 0; i < lease.Count; i++)
        {
            EntityId entity = lease.Entities[i];
            ref readonly NeedsComponent current = ref lease.Span[i];

            NeedsComponent updated = current with
            {
                Satiety = MathF.Max(0, current.Satiety - SatietyDecayPerTick),
                Hydration = MathF.Max(0, current.Hydration - HydrationDecayPerTick),
                Sleep = MathF.Max(0, current.Sleep - SleepDecayPerTick),
                Comfort = MathF.Max(0, current.Comfort - ComfortDecayPerTick),
            };

            writeBatch.Add(entity, updated);
        }

        // lease disposes here, releasing native span counter
        // writeBatch flush after dispose: K5 mutation rejection invariant preserved
        context.NativeWorld.FlushWrites(writeBatch);
    }
}
```

**Key invariants**:
- `lease` is `IDisposable`; using statement ensures release before any mutation
- `writeBatch.Add(entity, c)` accumulates in managed memory; no P/Invoke per entity
- `FlushWrites(batch)` is single P/Invoke per batch — entire mutation transferred via `df_world_flush_write_batch` C ABI
- `current with { ... }` uses C# record-struct copy-and-modify (zero allocation for unmanaged struct components)

### §3.2 — Write pattern (K-L7 + K5 protocol)

Same pattern as §3.1 — writes always via WriteBatch, never direct mutation. WriteBatch supports:
- `Add(entity, component)` — overwrite or insert
- `Remove(entity)` — component removal
- `Destroy(entity)` — entity destruction (cascades to all components on entity)
- `Spawn(out entity)` — entity creation (entity ID assigned at flush time)

Multiple batches across multiple component types collapsible into single P/Invoke via `WriteCommandBuffer` aggregation — K8.3 brief defers this optimization; per-batch flush acceptable at K8.3 (precedent: K7 perf evidence showed batching dominates managed-non-batched even without aggregation).

### §3.3 — Mutation rejection invariant (K5)

```csharp
// ANTI-PATTERN — triggers IsolationViolationException
using var lease = world.AcquireSpan<NeedsComponent>();
foreach (var (entity, _) in lease)
{
    var current = world.GetComponent<NeedsComponent>(entity);
    world.SetComponent(entity, current with { Satiety = 100 }); // VIOLATION
}
```

Native side `active_spans_` atomic counter > 0 during span lifetime; any mutation through P/Invoke layer (which routes through `df_world_*` C ABI) checks counter and throws `std::logic_error` if positive. Managed bridge converts to `IsolationViolationException`.

**K8.3 brief enforces**: every migrated system uses `using var lease = ...` followed by `WriteBatch`; flush after lease disposes. Violation surfaces in tests (per-system test class includes mutation rejection negative test).

### §3.4 — Multi-component reads

Systems reading multiple component types acquire one lease per type:

```csharp
public override void Update(SystemExecutionContext context)
{
    using var positions = context.NativeWorld.AcquireSpan<PositionComponent>();
    using var needs = context.NativeWorld.AcquireSpan<NeedsComponent>();
    using var consumables = context.NativeWorld.AcquireSpan<ConsumableComponent>();

    // Iterate intersecting entity set via entity ID matching
    // (utility helper: NativeWorld.IntersectEntities(positions, needs))

    var consumeBatch = new WriteBatch<NeedsComponent>();
    var destroyBatch = new WriteBatch<ConsumableComponent>();

    // ... consume logic ...

    // All leases dispose at scope end; both batches flush after dispose
    context.NativeWorld.FlushWrites(consumeBatch);
    context.NativeWorld.FlushWrites(destroyBatch);
}
```

**Open question for K8.3 execution**: `NativeWorld.IntersectEntities` helper utility — does it exist in K8.2 v2 plumbing? If not, K8.3 ships it. Pre-flight Phase 0 verifies. If absent, Phase 2 adds before per-system migrations begin.

### §3.5 — Dual-path API surface present but Path β unused

Per Q-K8.3-2 LOCKED, `SystemBase` exposes dual API:

```csharp
public abstract class SystemBase
{
    protected SystemExecutionContext Context { get; private set; }

    // Path α — used by all 12 production systems at K8.3
    protected NativeWorld NativeWorld => Context.NativeWorld;

    // Path β — accessor present but not used in production at K8.3
    // K8.4 ships ManagedStore<T> resolver; K8.3 systems don't call this
    protected ManagedStore<T> ManagedStore<T>() where T : class, IComponent
        => Context.GetManagedStore<T>();  // throws if no [ManagedStorage] component
}
```

K8.3 brief authoring **does not invoke `ManagedStore<T>()`** in any migrated system. The accessor exists for K8.4 + M-series consumers. If any K8.3-migrated system attempts ManagedStore access, brief halts; surface as deviation; escalate per «stop, escalate, lock» rule.

### §3.6 — Performance contract (Q5.a passive)

Each migrated system carries comment annotating performance contract:

```csharp
/// <summary>
/// NeedsSystem — Path α exclusive. Per-tick: 1 SpanLease<NeedsComponent>,
/// 1 WriteBatch<NeedsComponent>. Zero managed heap allocations.
/// </summary>
/// <remarks>
/// Performance contract (K-L7): read via SpanLease (zero-P/Invoke iteration),
/// write via WriteBatch (single P/Invoke flush). K7 V3 evidence base.
/// </remarks>
public sealed class NeedsSystem : SystemBase { ... }
```

K9 closure precedent: `KernelCapabilityRegistry` tags per-system access pattern. K8.3 brief verifies registry tags reflect post-migration shape (Phase 4.3 check).

---

## §4 Per-system migration design

For each of 12 systems, K8.3 execution agent applies the migration pattern. Brief provides per-system design here at high level; agent fills implementation details at execution.

### §4.1 — Tier 1: NeedsSystem, MoodSystem, JobSystem

**NeedsSystem**:
- Reads: `NeedsComponent` (Satiety, Hydration, Sleep, Comfort floats per userMemories wellness-orientation convention 1.0 = good)
- Writes: `NeedsComponent` (decay per tick)
- Pattern: single read lease + single write batch (template §3.1)
- Test class: 5-7 tests (decay rates, clamp to 0, full pawn population scaling)

**MoodSystem**:
- Reads: `NeedsComponent` + `MoodComponent` + `IdentityComponent` (for mood expression strings)
- Writes: `MoodComponent` (mood derived from need levels)
- Pattern: 3 read leases + 1 write batch
- Test class: 6-8 tests (derivation logic, threshold transitions, identity-driven expression)

**JobSystem**:
- Reads: `NeedsComponent` + `JobComponent` + `PositionComponent` (job assignment based on need + location)
- Writes: `JobComponent` (assignment), publishes events
- Pattern: 3 read leases + 1 write batch + event publication
- Test class: 7-9 tests (assignment rules per JobKind enum, urgency override, idle handling)

### §4.2 — Tier 2: PawnStateReporterSystem, ConverterSystem

**PawnStateReporterSystem**:
- Reads: Many components (NeedsComponent, MoodComponent, JobComponent, IdentityComponent, SkillsComponent, PositionComponent)
- Writes: None (pure publisher)
- Pattern: 6 read leases + 0 write batches + publishes `PawnStateChangedEvent` per tick per pawn
- Test class: 5-7 tests (snapshot completeness, event ordering, multi-pawn handling)
- **Special**: pure-publisher pattern — simplest write semantics but most read leases

**ConverterSystem**:
- Reads: `WorkbenchComponent` + `StorageComponent` (workbench state + adjacent storage)
- Writes: `WorkbenchComponent` (production progress)
- Pattern: 2 read leases + 1 write batch
- Test class: 6-8 tests (state machine transitions, storage coupling, production timing)

### §4.3 — Tier 3: ConsumeSystem, HaulSystem

**ConsumeSystem (M8.5)**:
- Reads: `PositionComponent` + `NeedsComponent` + `ConsumableComponent` + `WaterSourceComponent` + `BedComponent` (for surroundings)
- Writes: `NeedsComponent` (restoration on consume) + destroys consumed entities
- Pattern: 5 read leases + 1 write batch + 1 destroy batch
- Test class: 8-10 tests (food consumption, water drinking, satiety boost calculations, entity destruction semantics)

**HaulSystem**:
- Reads: `PositionComponent` + `InventoryComponent` + `ReservationComponent` + storage targets
- Writes: `InventoryComponent` (pickup/drop), `ReservationComponent` (reservation lifecycle)
- Pattern: 4 read leases + 2 write batches
- Test class: 7-9 tests (haul job pickup, reservation conflicts, drop-off semantics)

### §4.4 — Tier 4: ElectricGridSystem, SleepSystem, MovementSystem

**ElectricGridSystem**:
- Reads: `PowerProducerComponent` + `PowerConsumerComponent` (grid sources/sinks)
- Writes: `PowerProducerComponent` (production state), `PowerConsumerComponent` (consumption state)
- Pattern: 2 read leases + 2 write batches + grid coordination logic
- Test class: 7-9 tests (grid topology, supply/demand balance, blackout cascades)

**SleepSystem (M8.6)**:
- Reads: `PositionComponent` + `NeedsComponent` + `BedComponent` + `ReservationComponent`
- Writes: `NeedsComponent` (Sleep + Comfort restoration via hybrid formula `ΔComfort = ΔSleep × 0.3`), `ReservationComponent` (bed reservation)
- Pattern: 4 read leases + 2 write batches
- Test class: 8-10 tests (bed reservation lifecycle, sleep + comfort coupled restoration, partial-sleep on movement)
- **Special semantics**: hybrid restoration formula from userMemories M8 context — Comfort ΔComfort = ΔSleep × 0.3 during bed sleep, plus passive ambient via ComfortAuraSystem

**MovementSystem**:
- Reads: `MovementComponent` + `PositionComponent` (+ NavGrid via injected `IPathfinding`)
- Writes: `MovementComponent` (path step index increment), `PositionComponent` (location update)
- Pattern: 2 read leases + 2 write batches + A* pathfinding integration
- Test class: 8-10 tests (path traversal, A* integration, blocked path handling)

### §4.5 — Tier 5: ComfortAuraSystem

**ComfortAuraSystem (M8.7)**:
- Reads: `PositionComponent` + `NeedsComponent` + `DecorativeAuraComponent`
- Writes: `NeedsComponent` (passive Comfort delta from aura radius coverage)
- Pattern: 3 read leases + 1 write batch + radius query (component-based, NOT K9 RawTileField — verified §4.5 footnote)
- Test class: 6-8 tests (aura radius calculation, multiple-aura overlap, Comfort delta accumulation)

**§4.5 footnote — radius query**: per K8.2 v2 closure delivered DecorativeAuraComponent as Path α struct. Radius query is component-based loop (iterate aura components, compute distance to pawn position per `PositionComponent`), not K9 RawTileField spatial primitive. K9 RawTileField is for fields (e.g., Mana fields M10.B); ambient Comfort aura at K8.3 is component-iteration. Future optimization may move to spatial index but **out of K8.3 scope** (precedent: «Features only on demand»).

### §4.6 — InventorySystem (tier classification at execution)

**InventorySystem**:
- Reads: `InventoryComponent` + ... (item pickup coordination)
- Writes: `InventoryComponent` (item add/remove)
- Pattern: TBD at execution — depends on read surface analysis
- Test class: 6-8 tests (capacity limits, item type filtering, multi-item operations)
- **Tier classification**: deferred to execution; expected Tier 2 or Tier 3 depending on read surface

---

## §5 Phase 0 — Pre-flight verification

Before Phase 1 system migrations begin, agent verifies state. **K9 closure precedent**: every brief Phase 0 reads core authoritative documents to align mental model.

### §5.1 — Read authoritative documents

**Mandatory reads** (in order):
1. `docs/governance/FRAMEWORK.md` v1.0 §6 — post-session protocol
2. `docs/methodology/METHODOLOGY.md` v1.7 §12.7 — canonical closure protocol
3. `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.1 §1.2 — K8.3 reformulated scope (note: §1.2 wording «34 systems» amended via §8.3 of this brief; agent reads both)
4. `docs/architecture/KERNEL_ARCHITECTURE.md` v1.5 Part 0 K-L3 + K-L7 + K-L11 + Part 2 K8.3 row
5. `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` §2 — Phase A'.5 position
6. `tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V2.md` (EXECUTED) — K8.2 v2 plumbing reference for SpanLease/WriteBatch APIs
7. `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` (EXECUTED) + addendum 1 — Path α/β bridge details

**Optional but recommended**:
8. `docs/reports/PERFORMANCE_REPORT_K7.md` — K7 V3 baseline for K8.3 sanity check (Phase 4.4)
9. Recent K9 closure entry in `docs/MIGRATION_PROGRESS.md` — K9 execution lessons learned

### §5.2 — Verify production state

```powershell
# Confirm GameBootstrap.coreSystems matches §0.1 inventory (12 systems)
Get-Content src/DualFrontier.Application/Loop/GameBootstrap.cs |
  Select-String -Pattern 'new \w+System\(' -Context 0,0
# Expected: 12 matches in coreSystems array
```

```powershell
# Confirm zero [ManagedStorage] attributes on production components
Get-ChildItem -Path src/DualFrontier.Components -Recurse -Filter *.cs |
  Select-String -Pattern '\[ManagedStorage\]' -SimpleMatch
# Expected: zero matches
```

```powershell
# Confirm 3 orphan .uid files present
Get-ChildItem -Path src/DualFrontier.Systems -Recurse -Filter *.uid |
  Where-Object { -not (Test-Path ($_.FullName -replace '\.uid$', '')) }
# Expected: 3 files (ShieldSystem, SocialSystem, BiomeSystem)
```

### §5.3 — Verify tooling state

```powershell
# Confirm register tooling operational
.\tools\governance\sync_register.ps1 --validate
# Expected: exit 0, validation report clean
```

```powershell
# Confirm test suite green at brief authoring baseline
dotnet test --logger "console;verbosity=minimal"
# Capture baseline test count for closure delta
```

### §5.4 — Verify no concurrent register modifications

```powershell
# Confirm REGISTER.yaml last-modified-commit matches register_version
.\tools\governance\query_register.ps1 --stale
# Expected: zero stale documents at K8.3 brief authoring time
```

---

## §6 Phase 1-5 execution shape

### §6.1 — Phase 1: Orphan .uid cleanup

**Single atomic commit**: `chore(systems): cleanup orphan .uid files from K8.2 v2 stub deletions`

Actions:
1. Delete `src/DualFrontier.Systems/Combat/ShieldSystem.cs.uid`
2. Delete `src/DualFrontier.Systems/Pawn/SocialSystem.cs.uid`
3. Delete `src/DualFrontier.Systems/World/BiomeSystem.cs.uid`
4. Grep verify no references remain (`Select-String -Pattern 'ShieldSystem\|SocialSystem\|BiomeSystem' -Path src/`)
5. `dotnet build` clean; `dotnet test` no test count delta

Test count delta: 0.
LOC delta: -3 files (no .cs content; just metadata).

### §6.2 — Phase 2: Shared test fixture authoring

**Single atomic commit**: `feat(tests): NativeWorldTestFixture shared fixture for K8.3 system tests`

Actions:
1. Author `tests/DualFrontier.Tests/Fixtures/NativeWorldTestFixture.cs` per §0.3 template
2. Add `tests/DualFrontier.Tests/Fixtures/README.md` (Tier 4 Category F entry — register sync on closure)
3. Add fixture-self-test class `tests/DualFrontier.Tests/Fixtures/NativeWorldTestFixtureTests.cs` (3-5 tests: construction, entity spawn, component registration, disposal)
4. `dotnet build` clean; `dotnet test` 3-5 new tests pass

Test count delta: +3 to +5.

### §6.3 — Phase 3: Per-system migration (12 atomic commits)

**Per Q-K8.3-4 rule**: agent classifies each system into tier at execution time; migrates Tier 1 first, alphabetical within tier; per-system atomic commit.

**Per-system commit shape**:
```
feat(systems): migrate <SystemName> to SpanLease/WriteBatch (K8.3 system N/12)

- Rewrite Update() to use SystemBase.NativeWorld.AcquireSpan<T> for reads
- Replace World.SetComponent<T> with WriteBatch<T>
- Add performance contract comment per Q5.a passive
- Add test class tests/DualFrontier.Tests/Systems/<Slice>/<SystemName>Tests.cs
- Test count: +N (per-system)
- All existing tests pass; new tests pass
```

**Estimated atomic commits**: 12 (one per system). Per-commit time: ~30-60 min auto-mode.

**Verification per commit**:
- `dotnet build` clean
- `dotnet test` all previously-passing tests still pass + new per-system tests pass
- No new compiler warnings
- Native selftest unaffected (K8.3 doesn't touch native code)

### §6.4 — Phase 4: Post-migration verification

**Phase 4.1 — Integration test**: launch Godot via F5; verify simulation runs full tick loop without crash; pawn behavior visually correct (50 pawns navigate, consume food/water, sleep, work).

**Phase 4.2 — Mutation rejection negative test sweep**: per Q-K8.3-3 (A) precedent, each system test class includes negative test verifying mutation rejection if span held during write attempt. Phase 4.2 confirms all 12 negative tests pass.

**Phase 4.3 — KernelCapabilityRegistry tag verification**: per §3.6, each migrated system tagged with «Path α exclusive — SpanLease/WriteBatch». Registry inspection confirms 12 tags present, zero stale tags from pre-migration.

**Phase 4.4 — Performance sanity check**: tick loop benchmark at K8.3 closure vs K7 V3 baseline. Target: within 20% of K7 V3 numbers (per skeleton's original recommendation, preserved). K7 V3 is the production target — K8.3 should match (we're migrating away from V2 baseline, K7 V3 was the migrated reference).

**Single commit**: `test(systems): K8.3 post-migration verification — performance sanity + KCR tags`

Test count delta: 0 (verifications, not new tests).
LOC delta: +50 to +100 (benchmark adjustment + KCR tag inspection script).

### §6.5 — Phase 5: Closure documentation amendments

Per Q-K8.3-5 LOCKED.

**Phase 5.1 — Amend MIGRATION_PLAN v1.1 → v1.2 (non-semantic correction)**:

Edit `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md`:

- §0.3 Decision #4 (K8.3 reformulated LOCKED): replace «all 34 production systems in `src/DualFrontier.Systems/`» with «all 12 production systems in `GameBootstrap.coreSystems` (the 14 non-coreSystems .cs files relocate to vanilla mod assemblies during M-series, where they receive SpanLease/WriteBatch shape directly per Migration Plan §0.2 «no architectural work happens in two places»)»
- §1.2 K8.3 sub-section: scope row reformulated similarly; inventory table replaced with §0.1 (12 systems) reference; closing note added: «K8.3 v2 brief (`tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` v2.0) supersedes original v1.0 wording per Q-K8.3-1 LOCKED 2026-05-13»
- Header version bump: v1.1 → v1.2
- Authored date entry: «2026-05-XX (K8.3 v2 brief Phase 5.1 correction)»

**Phase 5.2 — Amend KERNEL_ARCHITECTURE v1.5 → v1.6 (non-semantic correction)**:

Edit `docs/architecture/KERNEL_ARCHITECTURE.md`:

- Part 2 K8.3 row: replace «12 vanilla systems migrated to SpanLease/WriteBatch. 2-3 weeks. -400/+600 LOC» with «12 production systems (GameBootstrap.coreSystems) migrated to Path α SpanLease/WriteBatch per K-L7 + K-L3.1. 4-6 weeks at hobby pace, ~12-20 hours auto-mode. -1500/+1200 LOC.»
- Header version bump: v1.5 → v1.6
- Status line: append «K8.3 production system migration closure (v1.6, 2026-05-XX): 12 systems migrated; per-system test class precedent; Path β plumbing deferred to K8.4 per Q-K8.3-2 LOCKED.»

**Phase 5.3 — Update MIGRATION_PROGRESS.md**:

Add K8.3 closure entry following K9 entry precedent:
- Date, commits range, test count delta, deviations (if any), lessons learned (4-7 items)
- Cross-reference to this brief
- CAPA-2026-05-12 closure entry if applicable

**Phase 5.4 — Brief lifecycle transition**:

Edit this brief (frontmatter and body):
- Frontmatter `lifecycle: AUTHORED` → `lifecycle: EXECUTED`
- Append section §10 «K8.3 Lessons learned» (7-10 items)
- Append section §11 «K8.3 execution lineage» (commit references, deviations)

**Phase 5.5 — REGISTER.yaml updates** (per §0.5 LOCKED):
- DOC-D-K8_3 lifecycle: AUTHORED → EXECUTED + governance_events list extended with EVT-2026-XX-XX-K8.3-CLOSURE
- DOC-A-MIGRATION_PLAN version bump
- DOC-A-KERNEL version bump
- CAPA-2026-05-12 closure (effectiveness_verification populated)
- RISK-010 mitigation_status update if validation clean
- REQ-Q-A45-X5 verification_status PENDING → VERIFIED
- audit_trail EVT-2026-XX-XX-K8.3-CLOSURE entry appended

**Phase 5.6 — Validation gate**:

```powershell
.\tools\governance\sync_register.ps1 --validate
# Must exit 0
.\tools\governance\sync_register.ps1 --sync
# Frontmatter mirrors regenerated
.\tools\governance\render_register.ps1
# REGISTER_RENDER.md regenerated
```

If validation non-zero: halt closure; surface deviation; do NOT proceed with final commit.

**Phase 5.7 — Final atomic commit**:

`docs(closure,governance): A'.5 K8.3 closure — 12 production systems migrated to Path α SpanLease/WriteBatch`

Includes:
- Phase 5.1-5.3 documentation amendments
- Phase 5.4 brief lifecycle transition
- Phase 5.5 REGISTER.yaml + REGISTER_RENDER.md updates

**Phase 5.8 — Push to origin/main**.

---

## §7 Operational reminders (Phase 0 read mandatory)

Inherited from K-Lessons + K9 execution lessons:

### §7.1 — testhost.exe file lock pattern (RISK-011)

After `dotnet test`, kill testhost processes before rebuild:
```powershell
Stop-Process -Name testhost -ErrorAction SilentlyContinue
Stop-Process -Name "DualFrontier.*" -ErrorAction SilentlyContinue
```

### §7.2 — dotnet test verbosity gotcha (RISK-011)

Use `--logger "console;verbosity=minimal"` for terminal-friendly output. Bare `dotnet test` produces hundreds of lines; minimal is sufficient and parseable.

### §7.3 — Atomic commit per system (K-Lessons #1)

Each per-system migration is single atomic commit. System rewrite + per-system test class + per-commit verification (build clean, tests pass). Reverting one system migration must not affect others.

### §7.4 — Pre-flight grep (K-Lessons #4)

Before modifying any system file, grep for duplicate constants:
```powershell
Get-ChildItem -Path src/ -Recurse -Filter *.cs |
  Select-String -Pattern 'const int Map(Width|Height)' -SimpleMatch
```
Catches duplicate `MapWidth`/`MapHeight` declarations across files (M8.0/M8.2 precedent — pawns wandering only upper-left quadrant due to duplicate constants).

### §7.5 — METHODOLOGY §7.1 «data exists or it doesn't» (11-th invocation at K8.3 Phase 1)

Phase 1 deletes 3 orphan .uid files. This is §7.1 application: data does not exist for these files; the corresponding .cs was deleted in K8.2 v2 because empty placeholder; the .uid is also placeholder lie at meta-level (Godot metadata for nothing).

### §7.6 — PS 5.1 quirks (A'.4.5 lessons)

If touching PowerShell tooling during K8.3 (unlikely but possible):
- `Where-Object` on single result returns Hashtable, .Count counts fields — use `@()` force-array
- Regex needs `(?s)` for multi-line frontmatter detection

---

## §8 Stop conditions

K8.3 execution halts immediately and escalates to chat session if any of these conditions surface:

### §8.1 — `[ManagedStorage]` attribute discovered on production component

Q-K8.3-2 LOCKED assumes zero Path β at K8.3 time. If grep at Phase 0.2 (or any subsequent Phase) finds `[ManagedStorage]` on a production component, K8.3 scope changes structurally. Halt; surface to chat; re-deliberate Q-K8.3-2 lock.

### §8.2 — Native selftest fails

K8.3 doesn't touch native code. If native selftest regresses during K8.3 execution, something is structurally wrong (concurrent K9 work?). Halt; investigate; do not proceed.

### §8.3 — KernelCapabilityRegistry tag count mismatch

Phase 4.3 verifies 12 KCR tags reflect post-migration state. If fewer (some system not registered) or more (stale tag), halt; investigate registry plumbing.

### §8.4 — Performance regression >20% vs K7 V3 baseline

Phase 4.4 sanity check. K8.3 should match K7 V3 within 20%. If regression exceeds threshold, halt; profile; surface for chat-session deliberation. Possible causes: per-batch flush overhead (not yet aggregated), test-fixture-induced re-registration cost, etc.

### §8.5 — Register validation failure at Phase 5.6

`sync_register.ps1 --validate` must exit 0 before final commit. If non-zero, halt; do not bypass via `git commit --no-verify` (BYPASS_LOG.md is for genuine exceptions, not closure protocol violations). Surface validation report; address; re-validate.

### §8.6 — Test count regression (tests failing post-migration)

Each per-system commit must preserve all previously-passing tests. If any test regresses, halt at that commit; do not proceed to next system; investigate before continuing.

---

## §9 Closure verification checklist

At Phase 5 completion, before pushing to origin:

- [ ] All 12 production systems migrated to SpanLease/WriteBatch
- [ ] 12 per-system test classes authored, all passing
- [ ] NativeWorldTestFixture shared fixture authored
- [ ] 3 orphan .uid files deleted
- [ ] Performance sanity check within 20% of K7 V3 baseline (Phase 4.4)
- [ ] KernelCapabilityRegistry tags 12 systems (Phase 4.3)
- [ ] No `[ManagedStorage]` on production components (Q-K8.3-2 verification holds)
- [ ] MIGRATION_PLAN amended v1.1 → v1.2 (scope reformulation)
- [ ] KERNEL_ARCHITECTURE amended v1.5 → v1.6 (Part 2 K8.3 row)
- [ ] MIGRATION_PROGRESS K8.3 closure entry added
- [ ] This brief lifecycle: AUTHORED → EXECUTED
- [ ] REGISTER.yaml entries updated (DOC-D-K8_3 + DOC-A-MIGRATION_PLAN + DOC-A-KERNEL + CAPA-2026-05-12 + RISK-010 + REQ-Q-A45-X5)
- [ ] audit_trail EVT-2026-XX-XX-K8.3-CLOSURE appended
- [ ] `sync_register.ps1 --validate` exit 0
- [ ] `sync_register.ps1 --sync` ran clean (228+ frontmatter mirrors updated)
- [ ] `render_register.ps1` ran clean (REGISTER_RENDER.md regenerated)
- [ ] dotnet build clean
- [ ] dotnet test all passing (delta: ~+65-95 tests vs K9 closure baseline)
- [ ] Native selftest passing
- [ ] F5 verification (Godot simulation runs full tick loop)
- [ ] Pushed to origin/main

---

## §10 Estimated atomic commit log

Anticipated commit sequence at K8.3 execution:

```
01. chore(systems): cleanup orphan .uid files from K8.2 v2 stub deletions
02. feat(tests): NativeWorldTestFixture shared fixture for K8.3 system tests
03. feat(systems): migrate NeedsSystem to SpanLease/WriteBatch (K8.3 1/12)
04. feat(systems): migrate MoodSystem to SpanLease/WriteBatch (K8.3 2/12)
05. feat(systems): migrate JobSystem to SpanLease/WriteBatch (K8.3 3/12)
06. feat(systems): migrate PawnStateReporterSystem to SpanLease/WriteBatch (K8.3 4/12)
07. feat(systems): migrate ConverterSystem to SpanLease/WriteBatch (K8.3 5/12)
08. feat(systems): migrate ConsumeSystem to SpanLease/WriteBatch (K8.3 6/12)
09. feat(systems): migrate HaulSystem to SpanLease/WriteBatch (K8.3 7/12)
10. feat(systems): migrate ElectricGridSystem to SpanLease/WriteBatch (K8.3 8/12)
11. feat(systems): migrate SleepSystem to SpanLease/WriteBatch (K8.3 9/12)
12. feat(systems): migrate MovementSystem to SpanLease/WriteBatch (K8.3 10/12)
13. feat(systems): migrate ComfortAuraSystem to SpanLease/WriteBatch (K8.3 11/12)
14. feat(systems): migrate InventorySystem to SpanLease/WriteBatch (K8.3 12/12)
15. test(systems): K8.3 post-migration verification — performance sanity + KCR tags
16. docs(closure,governance): A'.5 K8.3 closure — 12 production systems migrated to Path α SpanLease/WriteBatch

Total: 16 atomic commits (range commit-N to commit-N+15)
Execution time: ~12-20 hours auto-mode
Wall-clock: distributed over 4-6 weeks at hobby pace
```

**Note on commit ordering**: actual commit order within Phase 3 (commits 03-14) follows Q-K8.3-4 rule-based tier ordering, applied at execution. The order above is a representative estimate; agent may reorder within tier per alphabetical rule.

---

## §11 Brief authoring lineage

- **2026-05-10** — K8.3 v1.0 skeleton authored (~2822 bytes, 9 TODO items) by Opus deliberation post-K-L3.1 closure
- **2026-05-12** — A'.4.5 Document Control Register operational; K8.3 skeleton frontmatter sync'd via sync_register.ps1 (lifecycle AUTHORED registered)
- **2026-05-13** — Consistency review (this brief authoring session) surfaced 5 critical inconsistencies (scope count contradiction, legacy 12-system reference, K-L3.1 dual-path TODO ambiguity, Phase A' position implicit, post-A'.4.5 governance absent)
- **2026-05-13** — 5 Q-K8.3 candidate locks identified; Crystalka acceptance «Принимаю рекомендации пиши полный бриф по ним»
- **2026-05-13** — K8.3 v2.0 full brief authored (this document) replacing v1.0 skeleton; 5 Q-locks ratified
- **(TBD)** — K8.3 execution session (~12-20 hours auto-mode, 4-6 weeks hobby pace); 16 atomic commits; A'.5 closure
- **(TBD)** — A'.4.5.bis or A'.9 analyzer integration may upgrade sync_register.ps1 to pre-commit hook (per Q-A45-X5 future evolution)

---

**Brief end. K8.3 AUTHORED v2.0. Awaits execution session.**
