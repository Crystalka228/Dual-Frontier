# A'.0.5 Phase 1 — Category F (module-local) staleness report

Scoped to `src/`, `mods/`, `tests/`, `assets/`, `native/` per brief §3.3. Architectural-staleness items in non-F docs (A/B/C/D/E) are not in Phase 6 refresh scope; K-L3-related staleness is A'.1 scope per brief §1.4.

Disposition codes:
- **AF** — auto-fix in Phase 6 (mechanical: deleted-type removal, terminology refresh)
- **REFRESH** — Phase 6 prose refresh (descriptive, not prescriptive); requires re-reading current source
- **STOP** — Stop #2 (architectural meaning); flag for A'.1 or A'.0.7
- **SKIP** — historical / forward-looking content; not stale

---

## §7.1 SCOPE — 5 component READMEs (Phase 5 atomic commit)

### src/DualFrontier.Components/README.md (top-level)
- L21 «Magic/ — mana, schools of magic, ether level, golem bond (GDD 4–5).» — references «schools of magic» concept; SchoolComponent deleted but conceptual narrative may be salvageable. **REFRESH** (Phase 6 not Phase 5; scope here is stub-reference removal).
- L22 «Combat/ — weapons, armor, shields, ammunition (GDD 6, Combat Extended).» — references deleted Weapon/Shield/Ammo concepts. **AF** Phase 5 (rewrite as «armor (post-K8.2v2 stub-only state); future ranged combat per ROADMAP backlog»).
- L24 «World/ — tiles, ether nodes, biomes.» — Biome deleted. **AF** Phase 5.
- L54-56 «`MagicSchool`, `ShieldKind`, `AmmoType`, `BiomeKind`» enums in TODO list — referencing deleted-component-tied enums. **AF** Phase 5 (remove the enum names; retain `RaceKind, SkillKind, JobKind, DamageType, PowerType, WorkbenchKind, TerrainKind` if still referenced).

### src/DualFrontier.Components/Combat/README.md ← HEAVILY STALE
- L11-14 Contents lists Weapon/Armor/Shield/Ammo — 3 deleted. **AF** Phase 5 (reduce to ArmorComponent only).
- L17-22 Rules section (Weapon Penetration / ArmorComponent / Shield HpPool / AmmoComponent) — 3 of 4 dead. **AF** Phase 5 (retain only Armor-relevant rules).
- L26-29 Usage example with WeaponComponent + AmmoComponent. **AF** Phase 5 (rewrite or remove example; possibly use ArmorComponent example instead).
- L32-35 TODO with DamageType / ShieldKind / AmmoType / weapon durability — 3 of 4 dead concepts. **AF** Phase 5 (remove ShieldKind, AmmoType, weapon-durability lines; retain DamageType only if still referenced by ArmorComponent).

### src/DualFrontier.Components/World/README.md
- L14 Contents «BiomeComponent.cs — biome kind». Biome deleted. **AF** Phase 5.
- L19-20 Rules «Biomes affect pawn mood modifiers and the available flora/fauna.» — depends on deleted BiomeComponent. **AF** Phase 5.
- L32 TODO «BiomeKind enum». **AF** Phase 5.

### src/DualFrontier.Components/Magic/README.md
- L13 Contents «SchoolComponent.cs — magic levels per school». School deleted. **AF** Phase 5.
- L26 Usage example references SchoolComponent. **AF** Phase 5.
- L31 TODO «MagicSchool enum». **AF** Phase 5.
- L37-46 «v02 Addendum additions» — extends GolemBondComponent — surviving content. **SKIP**.

### src/DualFrontier.Components/Pawn/README.md
- L16 Contents «SocialComponent.cs». Social deleted. **AF** Phase 5.
- L23-24 Rules «Direct mutation of `Relations` from a non-social system is forbidden — only through `[SystemAccess(writes: SocialComponent)]` on SocialSystem.» — 2 deleted refs. **AF** Phase 5.

---

## §6 SCOPE — module-local refresh (Phase 6, kernel-area first)

### KERNEL-AREA (Phase 6 priority per Crystalka direction)

#### src/DualFrontier.Core/README.md
- Reads as generally clean architectural overview. **SKIP** Phase 6 unless re-read surfaces drift.
- L52 «public sealed class» component example may need cross-check vs current K-L3.1 dual-API. **REFRESH** if applicable.

#### src/DualFrontier.Core/Bus/README.md, ECS/README.md, Math/README.md, Registry/README.md, Scheduling/README.md
- Need read-pass in Phase 6. Suspected drift: post-K8.x native handle types (NativeWorld, SpanLease, WriteBatch, InternedString, NativeMap, NativeComposite, NativeSet) may not be reflected. **REFRESH**.

#### src/DualFrontier.Core.Interop/MODULE.md
- Likely stale: K8.1 wrapper value-type refactor + K8.2v2 component conversions changed structural shape; INTEROP module is the heart of K-L3.1 bridge. **REFRESH** Phase 6.

#### src/DualFrontier.Contracts/Modding/README.md (and parent)
- IModApi v3 surface (per K_L3_1_AMENDMENT_PLAN §1.4) adds RegisterManagedComponent<T> for Path β. Current IModApi v2 surface in code; v3 lands at K8.4 closure (future). README likely describes v2 — that is current truth and should remain so until K8.4. **VERIFY only**.
- L? «Mod component types must be `unmanaged` structs (Path α)» — if exists, this is K-L3-pre-locked language; K-L3.1 reformulated as «Path α default + Path β bridge». **STOP** flag for A'.1 (K-L3.1 amendment propagates to module-local docs as part of A'.1, not A'.0.5).

#### src/DualFrontier.Contracts/Attributes/README.md
- 1 stub-name match per Phase 1 grep. Likely SocialComponent or similar referenced in capability example. **REFRESH** Phase 6 if mechanical; **STOP** if architectural.

### NON-KERNEL (Phase 6 second pass)

#### src/DualFrontier.Components/{Shared,Items,Building}/README.md
- Reads as clean per Phase 1 spot-checks. **SKIP** Phase 6 unless re-read surfaces drift.

#### src/DualFrontier.Systems/Combat/README.md
- L10-14 Dependencies list includes deleted ShieldComponent, ProjectileComponent (verify current). **AF**.
- L17-21 Contents lists ShieldSystem, StatusEffectSystem; both deleted/stub-only post-K8.2v2. **AF**.
- L29-30 Rules reference ProjectileSystem REALTIME tick (verify still exists or deferred to G5). **REFRESH**.
- L38-44 TODO list with 5 unimplemented systems — matches «Phase 5 backlog» in ROADMAP; can stay as TODO. **SKIP**.
- L46-59 «v02 Addendum (TechArch §12.4)» — CompositeResolutionSystem, ComboResolutionSystem; verify still present. **REFRESH**.

#### src/DualFrontier.Systems/World/README.md
- L11 Dependencies «BiomeComponent» — deleted. **AF**.
- L14 Events «BiomeShiftEvent» — verify if event still exists; if not, **AF**.
- L18 Contents «BiomeSystem.cs — RARE» — system deleted (per K8.2v2 K-Lessons). **AF**.
- L22-24 Rules «WeatherSystem does not write BiomeComponent directly — only via event publication; BiomeSystem reacts.» — both BiomeComponent and BiomeSystem deleted. **AF**.
- L36 TODO «BiomeSystem». **AF**.

#### src/DualFrontier.Systems/Pawn/README.md
- L12 Dependencies «SocialComponent» — deleted. **AF**.
- L22 Contents «SocialSystem.cs — RARE: social ties». System deleted. **AF**.
- L49 TODO «Implement SocialSystem». **AF**.

#### src/DualFrontier.Systems/Magic/README.md
- L11 Dependencies «SchoolComponent» — deleted. **AF**.

#### src/DualFrontier.Systems/Faction/README.md
- L11 Dependencies «SocialComponent» — deleted. **AF**.

#### src/DualFrontier.Systems/README.md
- L21 Contents «Pawn/ — needs, jobs, mood, social, skills.» — «social» concept dead. **AF**.
- L23 Contents «Combat/ — combat initiation, projectiles, damage, shields, effects.» — «shields» dead concept post-K8.2v2; «projectiles» effectively deferred to G5. **AF/REFRESH**.

#### src/DualFrontier.Events/Combat/README.md
- L23-24 Rules «ShootAttemptEvent ... CombatSystem decides whether the shot is possible (it inspects WeaponComponent/AmmoComponent).» — both deleted. **AF**.
- L36 TODO «AmmoType, DamageType, GridVector, StatusKind». AmmoType-tied; DamageType may still exist. **AF**.

#### src/DualFrontier.Events/Pawn/README.md
- L? — 1 stub match per Phase 1 grep; likely SocialSystem reference. **AF** if mechanical.

#### src/DualFrontier.Core/Math/README.md
- 1 stub match per Phase 1 grep. **AF/REFRESH** in Phase 6.

#### Remaining src/ module READMEs (~50 files)
- Need read-pass in Phase 6. Most expected clean; opportunistic auto-fix for any incidental references.

#### mods/DualFrontier.Mod.Vanilla.{Combat,Magic,Pawn,World}/README.md
- These are mod placeholders; currently empty mods. May have outdated narrative descriptions of intended scope. **REFRESH** if current narrative inaccurate.

#### tests/**/README.md
- Generally describe test fixture conventions; expected stable post-K8.2v2. **VERIFY only**.

#### native/**/MODULE.md
- C++ kernel module notes; expected aligned with KERNEL_ARCHITECTURE v1.4. **VERIFY only**.

---

## STOP #2 candidates (architectural-meaning items flagged for A'.1)

- Any module-local README claiming K-L3 «без exception» or describing K8.2v2 closure state in K-L3-pre-locked language. K-L3.1 reformulation is A'.1 scope; A'.0.5 only flags occurrences here.
- Any module-local README describing IModApi v3 surface or Path β registration paths — these don't exist in code yet (land at K8.4); current docs should describe v2 truth. **VERIFY** that no module-local doc has been forward-edited to claim v3.

---

## SKIP (historical / forward-looking)

- ROADMAP.md Backlog references SocialSystem migration to Vanilla.Pawn (M10), ManaComponent display (Phase 5/M10), Combat command dispatch re-create — these are forward-looking gameplay backlogs, not stale architectural claims. SKIP — no edits.
- Brief files (Category D) referencing deleted stubs — historical record, intentionally preserved. SKIP — no edits.
- Audit reports (Category E) — historical record. SKIP — no edits.
