# K-L3.1 — Bridge formalization (architectural decision session brief)

**Brief type**: Architectural decision brief (fourth brief type, precedent K8.0)
**Authored**: 2026-05-10 (Opus, post-K8.2 v2 closure)
**Target session**: Crystalka + Opus chat session (no Cloud Code)
**Estimated session length**: 2–4 hours (deliberation + Phase 4 amendment plan authoring)
**Status**: EXECUTED 2026-05-10 — Phase 0 reads + Phase 1 deliberation Q1–Q6 + Phase 3 synthesis (§4.A) + Phase 4 amendment plan authoring complete. Amendment plan at `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` is executable artifact; AMENDMENTS LANDED 2026-05-10 via A'.1.K execution session (commits K-A1 through K-A8 covering KERNEL v1.5 + MOD_OS v1.7 + MIGRATION_PLAN v1.1 + MIGRATION_PROGRESS sync + 4 skeleton brief surgical edits).
**Locks** (session 2026-05-10): Q1=(a) `[ManagedStorage]` attribute on type; Q2=(β-i) mod-side storage + IModApi `RegisterManagedComponent<T>`; Q3=(i) explicit dual API via `SystemBase.NativeWorld` + `SystemBase.ManagedStore<T>()`; Q4=(b) runtime-only managed-path (no persistence); Q5=(a) passive metrics, analyzer (Q5.b) deferred post-migration; Q6=(a) path-blind capability (already structurally true per K4 prerequisite); Synthesis=§4.A amend K-L3 (peer paths, single principle).
**Prerequisite**: K8.2 v2 closure (`7527d00` on main) — DONE 2026-05-09
**Blocks**: README cleanup pass, push to origin, K8.3 brief authoring (all deferred to post-K-L3.1 amendment brief execution)
**Does not block**: K9 brief authoring (kernel-side, RawTileField, independent of bridge decision; surgical disposition B per amendment plan §5.1)

---

## §1 Why this session exists

### §1.1 The drift surfaced

K8.2 v2 closure (2026-05-09) executed cleanly: 6 class→struct conversions, 6 empty TODO stub deletions, 12 `[ModAccessible]` annotations, K8.1 wrapper value-type refactor, 631 passing tests. The closure report framed the achievement as "K-L3 «без exception» state achieved."

In the cleanup session immediately following closure, Crystalka clarified:

> «Тут был частичный перенос то что можно легко преобразовывать в struct было преобразовано, а что нет то managed, так как не все имеет смысл тащить в натив для скорости»

This corrects the closure framing: K8.2 v2 applied **selective performance-driven judgment** in-flight — components where `unmanaged struct` conversion was architecturally justified got converted; the rest stayed class-managed. K-L3 «без exception» as a *universal mandate* was never the intended principle; it was a misframing in the closure report and pre-existing wording across LOCKED documents.

A subsequent clarification raised the architectural shape this implies:

> «Тут наверно должен быть мост где что-то что можно преобразовать из классов модов в struct, а что нельзя остаётся в managed»

This is **Reading γ** — a structural bridge mechanism where component authors declare per-component storage path. Default path = `unmanaged` struct (kernel native, K-L3 LOCKED). Escape hatch = managed class storage when conversion is not architecturally justified.

The session locks Reading γ formally (or rejects it for an alternative) and produces an amendment plan for the affected LOCKED documents.

### §1.2 The stale architectural surface

Four LOCKED documents currently contain wording incompatible with selective per-component path:

1. **`KERNEL_ARCHITECTURE.md` v1.4 Part 0 K-L3** — wording elevates `unmanaged struct` to universal mandate (per K8.2 v2 closure framing). Authoritative pre-flight read in Phase 0.
2. **`MOD_OS_ARCHITECTURE.md` v1.6 LOCKED §«Modding с native ECS kernel» (lines 1149–1150)** — explicit:
   - «Mod component types must be `unmanaged` structs (Path α)»
   - «Class-based component storage prohibited (через ECS — mod state classes acceptable outside ECS)»
   - The «mod state classes acceptable outside ECS» clause is interesting — it's the seed of Reading α (strict universal: ECS components = struct; managed state lives outside ECS). Reading γ extends this to «ECS components themselves can be either path».
3. **`MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.0 LOCKED** — line 62 contains «Option (I) was rejected because vanilla mods would temporarily contain class components (K-L3 violation period)»; lines 130, 522–523 contain «K-L3 LOCKED, no exception post-K8.2» framing.
4. **`MIGRATION_PROGRESS.md`** (live, not LOCKED) — K8.2 v2 closure entry contains «K-L3 «без exception» state achieved» framing in 4 places (lines 35, 407, 443, 457).

All four require synchronized amendment after the session locks the decision. The session output **is** the amendment plan; the amendments themselves are a follow-up brief executed by Cloud Code.

### §1.3 Why this is a separate session, not inline cleanup

Per `METHODOLOGY.md` §3 «stop, escalate, lock»: architectural questions are not resolved through quick clarifications in operational sessions. They get first-class deliberation. K8.0 set the precedent (architectural decision recording brief, fourth brief type per K8.0 §1.8). K-L3.1 follows the same pattern.

Per Crystalka direction 2026-05-10: «Без костылей у меня много времени, а также требуется архитектурная чистота, чтобы проект жил десятилетиями». Resolving the architectural question first, then synchronizing documents, then cleaning operational debt — is the order that preserves long-horizon document coherence.

---

## §2 Pre-flight read inventory (Phase 0)

The session begins with the Opus instance reading these documents end-to-end before deliberation. The session does not deliberate against memory; it deliberates against the disk truth.

### §2.1 Architectural reads (mandatory, full-document)

1. **`docs/architecture/KERNEL_ARCHITECTURE.md` v1.4** — full document. Particular attention:
   - Part 0 — K-L1 through K-L11 (which are foundational invariants and which are conditional)
   - K-L3 specifically — original wording, post-K8.0 implication extension, post-K8.2 v2 implication update
   - K-L8 «storage as architectural concern» — relates to bridge mechanism
   - K-L9 «vanilla = mods» — first-author of bridge if it exists
   - K-L11 «World-as-test-fixture» — defines retained managed surface
   - Part 2 — K8.x master plan (current row wording for K8.2/K8.3/K8.4/K8.5)
   - Part 7 (if present) — error semantics convention from K-Lessons closure
2. **`docs/architecture/MOD_OS_ARCHITECTURE.md` v1.6 LOCKED** — full document. Particular attention:
   - §1.3 vanilla mod definition + §1.4 load graph
   - §3 capability model (§3.1, §3.2, §3.5, §3.6)
   - §4.6 IModApi v3 surface (Fields and ComputePipelines — relate to per-path API extension question)
   - §5 type-sharing protocol (shared ALC, sharing rules)
   - §6 three-level contracts (data/service/protocol — managed-path components have implications here)
   - §11 migration phases (M3.5 deferred entry — capability registry refresh)
   - §12 D-1 LOCKED `[ModAccessible]` (must work uniformly across paths)
   - **Lines 1149–1150** specifically — the precise stale wording
3. **`docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.0 LOCKED** — full document. Particular attention:
   - §0.3 LOCKED architectural decisions — 8 decisions; will likely need decision #9 added or #2 reformulated
   - §1.1 K8.2 scope (already executed — historical reference for what selective judgment was applied)
   - §1.2 K8.3 scope (next milestone — bridge decision affects system access pattern wording)
   - §6 document maintenance requirements (where amendments fold into existing scheduled amendment milestones)
4. **`docs/MIGRATION_PROGRESS.md`** — full document. Particular attention:
   - K8.2 v2 entry (lines ~407–467) — actual delivered state, in-flight decisions recorded
   - Decision log D5 — Solution A choice rationale (NativeWorld single source of truth)
   - Open questions OQ1–OQ4
5. **`docs/methodology/METHODOLOGY.md` v1.5** — Particular attention:
   - §3 «stop, escalate, lock» rule
   - §7.1 «data exists or it doesn't» principle (relates to bridge necessity question)
   - K-Lessons sub-section (atomic commit, Phase 0.4 hypothesis, mod-scope test isolation)

### §2.2 Code reads (mandatory, structured inventory)

1. **`src/DualFrontier.Components/`** — current state post-K8.2 v2:
   - `directory_tree` of slice subdirectories (Building, Combat, Items, Magic, Pawn, Shared, World)
   - For each surviving component (~25 after 6 stub deletions): note whether it's `unmanaged struct` or `class`, what fields it carries, whether it uses K8.1 primitives. **This is the empirical baseline**: any component currently surviving as `class` is direct evidence that selective judgment was already applied; the bridge decision formalizes what's already practice.
2. **`src/DualFrontier.Core/`** — managed storage path inventory:
   - Does `ManagedComponentStore` (or analog) exist? `World` class structure
   - How does current managed-path read/write differ from native-path
   - Test fixture surface (per K-L11 retained for tests)
3. **`src/DualFrontier.Core.Interop/`** — native-path bridge:
   - `NativeWorld`, `SpanLease<T>`, `WriteBatch<T>`, K8.1 wrappers post-refactor
   - `SystemBase.NativeWorld` accessor (added in K8.2 v2 per Phase 2.7 §3 stop)
4. **`src/DualFrontier.Application/Modding/`** — Mod API surface:
   - `IModApi` v2 implementation (`RestrictedModApi`)
   - `KernelCapabilityRegistry`
   - How `[ModAccessible]` is consumed at load time
5. **`src/DualFrontier.Contracts/Modding/IModApi.cs`** — surface that mods consume:
   - `RegisterComponent<T>` constraint (currently `where T : IComponent`?)
   - Whether constraint is `unmanaged` or open

### §2.3 Optional reads

- `src/DualFrontier.Persistence/` — codec layer. Becomes relevant only if save/load contract decision (Question 4 below) requires codec extension.
- `mods/DualFrontier.Mod.Vanilla.*/` — skeleton state (post-M8.1). Relevant to assess what the first-author user of the bridge looks like in practice.

---

## §3 The 6 design questions (Phase 1 deliberation space)

Each question gets independent treatment in session. Alternatives are formulated to be **mutually exclusive within question** but **jointly compatible across questions** (any answer to Q1 should compose with any answer to Q2, etc.).

### §3.1 Q1 — Storage path declaration mechanism

**Question**: How does a mod (or kernel-side) component author declare which storage path applies to a given component type?

**Alternatives**:

**Q1.a — Attribute on type**
```csharp
[ManagedStorage]
public sealed class ComplexGameplayComponent : IComponent { /* class fields */ }

// Default (no attribute) implies native unmanaged struct path
public struct SimpleStatComponent : IComponent { int Value; }
```
- Read by ModLoader at registration time
- Compile-time pairing: `[ManagedStorage]` + `class` consistent; absence + `struct` consistent
- Compile error if `[ManagedStorage]` on a struct or absence on a class? Or runtime warning?
- Pro: type-author-authoritative, single source of truth, refactor-safe (rename catches everything)
- Con: requires Roslyn analyzer to enforce attribute–shape consistency cleanly

**Q1.b — Marker interface dichotomy**
```csharp
public interface IComponent { } // existing
public interface IManagedComponent : IComponent { } // new

public class ComplexGameplayComponent : IManagedComponent { /* class fields */ }
public struct SimpleStatComponent : IComponent { int Value; }
```
- Type-system-enforced: implementing `IManagedComponent` requires class (interface allowed on classes)
- Generic constraint dispatch: `RegisterComponent<T>() where T : IComponent` vs `RegisterManagedComponent<T>() where T : IManagedComponent`
- Pro: zero runtime check; the compiler enforces the path
- Con: interface explosion if multiple categories surface (e.g. future `INativeComputeComponent`); breaks existing mods if `IComponent` constraint tightens

**Q1.c — Manifest declaration**
```json
{
  "components": [
    { "type": "Vanilla.Pawn.IdentityComponent", "path": "native" },
    { "type": "Vanilla.Pawn.JobQueueComponent", "path": "managed" }
  ]
}
```
- Declared per-mod in `mod.manifest.json`
- ModLoader reads manifest, dispatches type registration to native or managed registry
- Pro: external surface stays clean; no attribute pollution; consistent with capability model location
- Con: type and path declared in different files (manifest drift risk per §3.7 Mod-OS); manifest authors must maintain it manually

**Q1.d — Inferred from type shape, no declaration**
- Reflection at registration time: if type is `unmanaged struct` → native path; if class → managed path
- Pro: zero declaration overhead; type shape is the declaration
- Con: invisible behavior — author cannot intend native-path for a class type and get a clear error; performance characteristics depend on shape decisions made for unrelated reasons

**Crystalka decides**: a/b/c/d, with reasoning for the rejected alternatives recorded in session log.

**Recommendation surface (Opus)**: probably (a) attribute or (d) inferred. (b) marker interface fragments the contract surface; (c) manifest creates drift risk. (a) gives explicit author intent; (d) is minimum-ceremony but loses intentionality.

### §3.2 Q2 — Where the managed-storage path lives architecturally

**Question**: Does the managed-storage path live as a peer of `NativeWorld` in `DualFrontier.Core.Interop` / `DualFrontier.Core`, or as a separate architectural artifact?

**Alternatives**:

**Q2.α — Peer storage in DualFrontier.Core**
```
DualFrontier.Core/
├── World.cs                    // existing managed World (test fixture per K-L11)
├── ManagedComponentStore.cs    // NEW — production managed-path storage
│
DualFrontier.Core.Interop/
├── NativeWorld.cs              // existing native-path
```
Both stores accessible via single `IWorld` interface (or `NativeWorld + ManagedComponentStore` separate handles).
- Pro: kernel-side architectural reality; single point of registration; simple per-system access
- Con: violates D3 «native organicity Lvl 1» principle? — answer depends on whether managed-store counts as a «native artifact» (it's not native, but it's another concurrent storage stack)

**Q2.β — Mod-side concern, kernel agnostic**
- Each mod that uses managed-path declares its own component storage in mod assembly
- Kernel only sees native-path components; managed-path is mod-internal
- Cross-mod managed-path access through events/intents only (no shared storage)
- Pro: maximum isolation; kernel surface unchanged; consistent with "every mod is a process" mental model
- Con: cross-mod managed-path systems impossible (e.g. Vanilla.Pawn defines `JobQueueComponent`, Vanilla.Combat needs to read it — would require event-mediation that's awkward); shared types via Vanilla.Core become harder

**Q2.γ — Hybrid via IModApi extension**
```csharp
public interface IModApi
{
    // v2 existing:
    void RegisterComponent<T>() where T : unmanaged, IComponent;
    
    // v3 (this question's proposed addition):
    void RegisterManagedComponent<T>() where T : class, IComponent;
    
    // Both register into kernel-managed registries (one native, one managed),
    // both queryable via IWorld at runtime.
}
```
- Kernel provides storage abstraction through ModApi; both paths registered through ModApi; kernel internally dispatches to NativeWorld or ManagedComponentStore
- Pro: ModApi is the contract surface, not implementation details; consistent with §4.6 IModApi v3 Fields/ComputePipelines extension pattern
- Con: implies kernel owns ManagedComponentStore (Q2.α-equivalent at kernel level); doesn't actually decouple from kernel

**Crystalka decides**: α/β/γ, with reasoning recorded.

**Recommendation surface (Opus)**: depends heavily on Q3. If cross-path access is allowed (Q3.i or Q3.ii), then Q2.α or Q2.γ is structurally required (kernel needs unified view). If cross-path access is forbidden (Q3.iii), Q2.β becomes viable.

### §3.3 Q3 — Cross-path access semantics

**Question**: Can a single system read/write components from both paths simultaneously?

Concrete: `JobAssignmentSystem` reads `IdentityComponent` (native struct, `InternedString Name`) AND reads `JobQueueComponent` (managed class, `Queue<JobIntent>` field) on the same entity. Allowed?

**Alternatives**:

**Q3.i — Explicit dual API**
```csharp
public class JobAssignmentSystem : SystemBase
{
    public override void Tick()
    {
        using var identityLease = world.AcquireSpan<IdentityComponent>();   // native path
        var jobQueueStore = world.GetManagedStore<JobQueueComponent>();     // managed path
        
        for (int i = 0; i < identityLease.Length; i++)
        {
            var entity = identityLease.Entities[i];
            ref readonly var identity = ref identityLease.Components[i];
            var jobQueue = jobQueueStore.Get(entity);
            // ... mixed read
        }
    }
}
```
- System author explicitly chooses API per access
- Pro: zero magic; performance characteristics visible per-call; matches existing `SpanLease`/`WriteBatch` precedent
- Con: API surface widens; system code mentions both `Acquire`/`AcquireManaged`

**Q3.ii — Single API, opaque dispatch**
```csharp
public class JobAssignmentSystem : SystemBase
{
    public override void Tick()
    {
        var identity = world.Get<IdentityComponent>(entity);       // native (dispatched)
        var jobQueue = world.Get<JobQueueComponent>(entity);       // managed (dispatched)
        // World.Get<T> dispatches per-T to native or managed store
    }
}
```
- Single contract surface; kernel dispatches per-T
- Pro: minimum API surface; system author sees one world
- Con: P/Invoke ceremony for native path becomes per-call (kills K8.2 v2 wrapper refactor performance gains for native side); needs explicit batched access alternative

**Q3.iii — Cross-path access forbidden**
- System declares `[SystemAccess(Read = typeof(IdentityComponent))]` (native path) OR `[ManagedSystemAccess(...)]` (managed path), never both
- Mixed-path workflows decomposed into two systems + event/intent bus mediation
- Pro: maximum isolation; per-path performance contracts strict; native-path systems pay zero managed-path overhead
- Con: every workflow needing both paths now needs event hop — adds tick latency, complicates control flow

**Crystalka decides**: i/ii/iii.

**Recommendation surface (Opus)**: probably (i) explicit dual API. (ii) sacrifices the K8.2 v2 wrapper refactor wins; (iii) forces gameplay decomposition that may be artificial. (i) preserves both performance contracts and allows cross-path systems where genuinely useful.

### §3.4 Q4 — Save/load contract for managed-path components

**Question**: How do managed-path components persist? Native-path uses K8.1 InternedString + value codec; managed components may contain refs that don't trivially serialize.

**Alternatives**:

**Q4.a — Each managed component implements `ISerializableComponent`**
```csharp
public interface ISerializableComponent : IComponent
{
    void Serialize(IComponentWriter writer);
    void Deserialize(IComponentReader reader);
}
```
- Component author owns serialization
- Pro: arbitrary complexity supported; codec layer doesn't need extension
- Con: per-component effort; serialization bugs spread across N components

**Q4.b — Managed-path forbidden for persisted components**
- Managed-path is for runtime-only state (caches, computed handles, transient gameplay state)
- Save system simply doesn't see managed-path components — they are reconstructed on load from native-path data + system logic
- Pro: clean separation of persistence concerns; codec layer untouched
- Con: limits managed-path usefulness; some gameplay state genuinely IS persistent and complex (e.g. skill progression history with dynamic structure)

**Q4.c — Codec layer extension for both paths**
- `src/DualFrontier.Persistence/` extended with `ManagedComponentCodec<T>` interface
- Per-managed-component codec authored alongside the component
- Pro: persistence layer becomes path-aware; uniform save/load surface
- Con: codec layer architectural change (currently designed for K8.1 primitives only); may force premature codec layer redesign

**Q4.d — Hybrid: managed default forbidden, opt-in serializable subtype**
- `[ManagedStorage]` default = runtime only
- `[ManagedStorage(Persistent = true)]` opts in to serialization, requires `ISerializableComponent` impl
- Pro: best of (a) and (b); explicit author intent; codec layer needs only minor extension
- Con: still per-component effort for the persistent subset

**Crystalka decides**: a/b/c/d.

**Recommendation surface (Opus)**: (b) is cleanest *if* the use case for persistent managed-path components is rare. (d) is best if persistent managed-path is a real first-class need. The decision depends on which gameplay patterns Crystalka anticipates needing managed-path support.

### §3.5 Q5 — Performance contract for managed-path

**Question**: Native-path publishes specific guarantees (zero-allocation reads, structure-of-arrays layout, span access). Managed-path can't guarantee these. How is the contract difference made visible to system authors?

**Alternatives**:

**Q5.a — Path tagged in registry, performance metrics per-path**
- `KernelCapabilityRegistry` tracks per-component path
- Performance reporting (analog of `PERFORMANCE_REPORT_K7.md`) splits metrics native-path vs managed-path
- Pro: visibility through tooling; doesn't constrain author choices
- Con: passive — author can mix paths inattentively, only sees consequences in profiler

**Q5.b — Compiler/analyzer enforcement**
- Roslyn analyzer: any system accessing managed-path component cannot also be tagged `[NoAllocate]` (or analog perf annotation)
- Pro: active feedback at edit time
- Con: requires analyzer infrastructure (M3.4 deferred work); adds another quality gate

**Q5.c — No guarantee, treat managed-path as second-class**
- Managed-path exists for cases where unmanaged is impossible/architecturally wrong
- No performance contract published; managed-path is "the slower path, use only when necessary"
- Pro: simple contract
- Con: insufficient guidance; future authors won't know when "necessary" applies

**Q5.d — Performance budget per-path with overflow detection**
- Per-tick performance budget, budgets per-path; managed-path systems have larger allocation budget
- Tick-loop tracks managed-path allocations, raises warning at threshold breach
- Pro: visibility + active feedback without prescriptive ban
- Con: complex telemetry layer; adds tick-time overhead

**Crystalka decides**: a/b/c/d.

**Recommendation surface (Opus)**: (a) for now — passive visibility without committing to analyzer infrastructure that doesn't exist. (b) becomes available once M3.4 (CI Roslyn analyzer) ships; can be K-L3.1 follow-up amendment. (c) is honest but unhelpful; (d) is over-engineered for current need.

### §3.6 Q6 — Capability system extension

**Question**: `[ModAccessible(Read = true, Write = false)]` works on native-path. Same attribute on managed-path components?

**Alternatives**:

**Q6.a — Same attribute, same semantics, orthogonal to path**
- `[ModAccessible]` declares mod-readable/writable access
- Path declared separately (Q1)
- Capability strings unchanged: `kernel.read:Vanilla.Pawn.JobQueueComponent` whether component is native or managed
- Pro: capability system is path-blind; D-1 LOCKED unchanged
- Con: capability resolver must dispatch to correct path internally — hidden complexity

**Q6.b — Separate annotation + capability namespace**
- `[ModAccessible]` for native, `[ManagedModAccessible]` for managed
- Capability strings: `kernel.read:` (native), `kernel.managed.read:` (managed)
- Pro: explicit at capability boundary; manifest declares path-aware capabilities
- Con: doubles surface; mod authors must distinguish at every access point

**Crystalka decides**: a/b.

**Recommendation surface (Opus)**: (a). Consistent with K8.2 v2 closure where `[ModAccessible]` was applied uniformly across 25 surviving components without path distinction. Keeping it path-blind preserves the ergonomics of a single capability declaration mechanism.

---

## §4 Synthesis (Phase 3)

After Q1–Q6 are answered, the session synthesizes the 6 answers into one coherent architectural lock. Three forms the synthesis can take:

**§4.A — Amendment to existing K-L3**
K-L3 wording in `KERNEL_ARCHITECTURE.md` Part 0 expanded to:
> «K-L3 (post-K8.2 v2 amendment): default storage path for components is `unmanaged struct` (Path α). Per-component selective managed-class storage (Path β) is permitted via [bridge mechanism per Q1]. Decision criterion: Path α applies when conversion to `unmanaged struct` is architecturally justified (performance, locality, save/load simplicity). Path β applies when conversion forces structural compromise (managed-only references, lazy state, complex object graphs).»

**§4.B — New K-L12 «Bridge for managed-path components»**
K-L3 stays as-is (universal mandate framing); new K-L12 adds the bridge as **explicit exception mechanism** with strict opt-in. Stronger isolation: bridge is the named exception, not the new normal.

**§4.C — Rejection of Reading γ; alternative direction**
If during deliberation an alternative emerges (e.g., Reading α reaffirmed: «mod state classes outside ECS» is sufficient escape hatch, no bridge needed), the session locks the rejection and amends documents to make the existing rule clearer rather than adding a bridge.

**Crystalka chooses synthesis form** at end of Phase 3.

---

## §5 Amendment plan (Phase 4 output)

The session's deliverable is an amendment plan, not the amendments themselves. The plan specifies:

### §5.1 KERNEL_ARCHITECTURE.md amendment

- Version bump v1.4 → v1.5 (likely; **possibly v1.4 → v2.0** if synthesis form §4.B introduces K-L12 — semantic change, not non-semantic correction)
- Part 0: K-L3 reformulated OR K-L12 added (per §4 synthesis)
- Part 0: K-L8 implication updated (storage abstraction now has two concrete paths)
- Part 2: K8.2 row text amended — closure framing «K-L3 «без exception» state achieved» replaced with «K-L3 selective application: 6 components converted to Path α, 6 stub deletions, 19 verify-only annotations; 25 survivors annotated for capability access»
- Part 7 (if exists, error semantics): unchanged or extended for managed-path errors
- Status line updated with v1.5/v2.0 + date

### §5.2 MOD_OS_ARCHITECTURE.md amendment

- Version bump v1.6 → v1.7 (non-semantic if §4.A; **semantic v1.6 → v2.0** if §4.B)
- §«Modding с native ECS kernel» (lines 1149–1150 currently): rewritten to reflect bridge mechanism. Concrete wording depends on synthesis:
  - §4.A reformulation: «Mod component types default to `unmanaged` struct (Path α). Path β (managed class storage) is available [per Q1 declaration]. Both paths are first-class kernel storage; mod authors choose per-component based on architectural fit.»
  - §4.B addition: «Mod component types must be `unmanaged` struct (Path α) by default. Path β (managed class storage) is available as architectural exception via [Q1 declaration] for components where Path α conversion is not architecturally justified.»
- §3 capability model: Q6 answer integration
- §4.6 IModApi v3 surface: Q2 + Q1 answer integration (e.g., new `RegisterManagedComponent<T>()` if Q2.γ chosen)
- §11 migration phases: M3.5 deferred entry — bridge readiness check folded into M3.5 if applicable
- §12 LOCKED decisions: D-1 reformulated if Q6.b chosen
- Lines 1149–1150 specifically rewritten

### §5.3 MIGRATION_PLAN_KERNEL_TO_VANILLA.md amendment

- Version bump v1.0 → v1.1 (non-semantic correction)
- §0.3 LOCKED decisions: decision #9 added («K-L3 selective per-component performance-driven, bridge per Q1») OR decision #2 reformulated
- Line 62 (Option I rejection paragraph): «K-L3 violation period» wording corrected to «Path α default; Path β bridge unavailable until K-L3.1 closure» OR similar per synthesis form
- §1.2 K8.3 scope: system access pattern wording extended to acknowledge path-aware access (Q3 answer)
- §1.3 K8.4 scope: Mod API v3 surface extended per Q2 answer (e.g., RegisterManagedComponent if Q2.γ)
- §1.4 K8.5 scope: capability annotation pass extended per Q6 answer
- §6 document maintenance: K-L3.1 amendment milestone added to scheduled amendments

### §5.4 MIGRATION_PROGRESS.md amendment

- Live tracker update — not a version bump, just content sync
- K8.2 v2 entry framing correction:
  - Line 35 (Current state snapshot): «K-L3 «без exception» state achieved» → «K-L3 selective application: bridge formalization deferred to K-L3.1»
  - Line 407 (entry header): same correction
  - Line 443 (KERNEL doc bump entry): correction
  - Line 457 (Architectural decisions LOCKED): correction
- New entry: K-L3.1 closure with SHA, brief reference, lessons learned
- Open questions: any new OQ surfacing from session

### §5.5 Cross-document drift audit (post-amendment)

After the amendment brief executes, a cross-document grep verifies no stale «without exception», «no exception», «K-L3 violation», «must be unmanaged struct» wording survives in any of the 4 LOCKED documents. This is a Phase 5 step in the amendment brief, not in the K-L3.1 session itself.

---

## §6 Closure (Phase 5)

### §6.1 K-L3.1 session output artifacts

1. This brief, marked EXECUTED with session SHA reference (the brief itself becomes a tracked document at `docs/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` per K8.0 precedent)
2. Session log capturing Q1–Q6 deliberation, alternatives considered, decision rationale
3. Amendment plan document at `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` (or analog)
4. `MIGRATION_PROGRESS.md` K-L3.1 closure entry

### §6.2 Follow-up brief (separate, post-K-L3.1)

After K-L3.1 closure, a small amendment brief is authored and executed by Cloud Code:
- Scope: docs-only amendments per §5.1–§5.4
- Estimated size: 30–60 min auto-mode
- Test count delta: zero (docs-only)
- Atomic commit shape: per-document (one commit per amended LOCKED doc), version bumps bundled per K-Lessons Lesson 1 «atomic commit as compilable unit»

### §6.3 Resumption of deferred work

After K-L3.1 closure + amendment brief execution, the deferred work resumes in this order:
1. **README cleanup** — remove deleted-stub references from 5 READMEs (top-level Components, Combat, World, Magic, Pawn). Single atomic commit, scope `docs(components)`, no other changes.
2. **Push to origin** — main branch, all K-L3.1 era commits + foundation closure commits go up together.
3. **K9 status check** — if not closed, K9 brief authoring resumes per Option c sequencing. If closed, proceed to K8.3.
4. **K8.3 brief authoring** — system migration brief, authored against post-K-L3.1 amended architecture (Q3 answer determines system access pattern wording).

### §6.4 Stop conditions for the session

The session halts and escalates if:
- Q1–Q6 deliberation surfaces a question not anticipated by this brief (record as new question, decide whether in-scope)
- Synthesis (Phase 3) cannot reconcile Q1–Q6 answers into coherent architectural lock (re-examine answers; if irreducible conflict, defer some Q answers to follow-up sub-session)
- Pre-flight reads (Phase 0) reveal that one of the 4 LOCKED documents has changed between brief authoring (2026-05-10) and session execution (record drift, adjust amendment plan)

---

## §7 What this session deliberately does not do

- **No code changes**. Pure architectural deliberation.
- **No new tests**. Test count stays at 631 throughout the session.
- **No commits to source files**. Only the amendment plan document is authored (and possibly the brief itself committed as `docs(briefs): author K-L3.1 brief`).
- **No execution of amendments**. Amendment plan is the deliverable; amendments themselves are a follow-up brief.
- **No K9 work**. K9 is independent of bridge decision per §1 above.
- **No cleanup**. README cleanup is post-K-L3.1 per §6.3.

---

## §8 Out of scope (explicit)

- **Save system implementation** — out of scope per `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` §8.1. Q4 considers save/load contract *for managed-path components*, but does not design or schedule the save system itself.
- **Performance benchmarking** — Q5 considers the performance contract surface, not measurement. Concrete benchmarks are post-K-L3.1 work, possibly part of K8.5 readiness gate.
- **Roslyn analyzer authoring** — Q5.b mentions analyzer enforcement; the analyzer itself is M3.4 deferred work, not K-L3.1 scope.
- **Mod ecosystem migration (M-series)** — bridge decision affects M-series brief authoring, but M-series itself is post-Phase-A per migration plan §0.1.
- **Native-path optimization work** — K-L3.1 does not optimize the existing native path; it formalizes the managed path as peer.

---

## §9 Pre-session checklist (Crystalka readiness)

Before invoking the K-L3.1 session, Crystalka confirms:

- [ ] K8.2 v2 closure verified (`git log --oneline -1` shows `7527d00` or descendant on main)
- [ ] No uncommitted work on `src/` (working tree clean)
- [ ] No active feature branch in mid-execution state
- [ ] Test baseline holds (`dotnet test` shows 631 passing, or current updated count if K9 has closed since)
- [ ] All 5 documents enumerated in §2.1 are present in repo at expected paths

If any check fails, K-L3.1 session pre-flight halts; the failing condition is resolved (or its absence acknowledged with explicit rationale) before deliberation begins.

---

## §10 Document provenance

- **Authored**: 2026-05-10, Opus 4.7, post-K8.2 v2 closure cleanup session
- **Authority**: Crystalka direction 2026-05-10 («Без костылей у меня много времени, а также требуется архитектурная чистота, чтобы проект жил десятилетиями»; «Тут наверно должен быть мост где что-то что можно преобразовать из классов модов в struct, а что нельзя остаётся в managed»)
- **Precedent**: K8.0 architectural decision recording brief (fourth brief type, K8.0 §1.8)
- **Methodology basis**: METHODOLOGY.md §3 «stop, escalate, lock»; K-Lessons Lesson 4 «error semantics convention» (analog: architectural decisions get formal recording)
- **Deferred work tracked by**: `userMemories` 2026-05-10 entry («K-L3 Bridge concept raised...»)

---

**Brief end. Pre-session reads (Phase 0) begin when Crystalka invokes the session. Phase 1 deliberation proceeds against the disk truth, not against this brief's understanding of it.**
