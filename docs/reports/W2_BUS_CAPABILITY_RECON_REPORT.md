---
register_id: DOC-E-W2_BUS_CAPABILITY_RECON_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-19'
last_modified: '2026-07-19'
content_language: en
next_review_due: null
review_cadence: none-historical-record
title: 'W2 BUS/CAPABILITY RECON REPORT — 2026-07-19 (R1–R5) — bus/capability/native-channel plumbing measurement at HEAD b2805ea for BD-3/BD-10/F-54 chartering: TWO parallel bus systems (5 managed genre buses keyed by System.Type, authoritative production dispatch; native 3-tier bus keyed by FNV1a32(FQN), DORMANT UseNativeBusForDispatch=false — corroborated EVENT_BUS §4/§5-FENCED + TCM row 9); native channels REGISTERED not FIXED (only 3 tiers fixed, channels auto-created per type_id); 15 df_bus_* exports, no "create channel"; ~30 BD-3 coupling points; KernelCapabilityRegistry 12-pattern kernel.* grammar over Contracts/Components/Events; census = 0 on-disk manifest entries / 4 inline / EXACTLY 208 implicit tokens (42 IEvent×4 + 40 component) 100% genre-FQN (BD-10 root); 0 events carry [EventTier] so Fast/Background tiers have zero shipped producer; genre-bus consumer closure = ModBusRouter reflective SHAPE + per-FQN SDK gate (bus resolved AFTER gate); _allowedBuses CONFIRMED DEAD (1 write/0 reads, wired to dead end); ISOLATION 178–183 orphaned (doc SUPERSEDED); bus-scoping unenforced at BOTH build (CONTRACTS §6) and runtime; decision-inputs + anomalies A1–A13; extends DOC-E-W1_SDK_SURFACE_RECON_REPORT R4; kickoff↔law conflict = stale ISOLATION pin only'
special_case_rationale: 'Durable-report recon enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-W1_SDK_SURFACE_RECON_REPORT, DOC-E-GAME_ENGINE_BOUNDARY_AUDIT_REPORT, DOC-E-EQ_A_SHUTDOWN_RECON_REPORT, DOC-E-F29_NATIVE_SCHEDULER_RECON_REPORT). Wave-2 pre-deliberation bus/capability/native-channel plumbing measurement — the empirical basis for the BD-3 (generic event routing) + BD-10 (kernel.* reframing) chartering deliberation and the F-54 fork (revive vs retire _allowedBuses). Produced read-only at HEAD b2805ea (post-W1 merge, PR #47). Load-bearing chains re-verified first-hand at this HEAD; broader inventory measured via a read-only recon sweep. UNTRACKED at authoring — enrolled at the next cascade C1.'
---

# W2 BUS/CAPABILITY RECON REPORT — 2026-07-19

Bus/capability/native-channel plumbing measurement for Wave 2 of the `VANILLA_SEPARATION_MIGRATION_PLAN`. Read-only measurement session: **one report, zero repository mutations, `sync` never run, zero builds/tests**. This document produces facts, anchors, and counts — **not designs, not API sketches, not recommendations**. Design belongs to the BD-3/BD-10/F-54 chartering deliberation that consumes this report.

**Mission.** Wave 2 removes the genre taxonomy from the engine contract by settling three ratification-grade decisions: **BD-3** — generic event routing replacing the five fixed buses + `IGameServices`; **BD-10** — `kernel.*` reframing so the capability registry becomes a registration ledger publishing ENGINE capabilities only; and the **F-54 fork** — revive per-system `_allowedBuses` scoping vs retire the dead field. All three rest on *how events, buses, capability names, and native channel identities actually flow today*. Per the scaffolding ruling (`VANILLA_SEPARATION_MIGRATION_PLAN.md` §1.1): harness wiring is a **sacrificial test harness**; this recon classifies by **mechanism**, not fidelity.

**Read-only law honored.** Single file output (this report). No git mutations; `sync` never run; no builds/tests. HEAD pinned throughout to `main` @ `b2805ea3af0a16668ae3af3e1527955cfaf89cf1` (`b2805ea`) — the exact post-W1 merge (PR #47) the kickoff names; working tree clean at measurement. Every figure below is anchored `file:line` or by grep expression at this HEAD.

**Relation to prior recon (extends, does not re-derive).** `DOC-E-W1_SDK_SURFACE_RECON_REPORT` (HEAD `df1541d`) R4 measured *the genre-bus surface from the 28 systems' side* — "five buses, two live" (Pawns + Inventory carry traffic; Combat/Magic/World are declared-only stubs), 28 `nameof(IGameServices.X)` production bindings, D6/D9, A3/A10 (system taxonomy ≠ bus taxonomy ≠ mod taxonomy). This recon measures **the plumbing beneath that surface**: the publish→native channel path, the capability-name mechanism, the consumer closure *outside* the 28 systems, and the F-54 fork surface. Where this report restates a W1 fact it is to build on it; every figure is re-measured at HEAD `b2805ea` (e.g. `ExampleSystem`, the W1 proof mod, did not exist at W1's own HEAD).

**Law in force (cited, not restated).** `CONTRACTS.md` v1.1.0 LOCKED (§2 five-bus canon, §4 evolution — adding/removing an `IGameServices` property is **breaking**, §6 enforcement reality) · `EVENT_BUS.md` v1.1.0 LOCKED (§3 native three-tier bus, §4 division of labor managed-authoritative/native-dormant, §5 FENCED sovereign-switch is Planned) · `MOD_OS_ARCHITECTURE.md` v1.1.0 LOCKED (§3.2 capability grammar, §3.3 provider namespaces `kernel` vs `mod.<modId>`, §3.4 kernel-provided set = reflection scan of Contracts/Components/Events) · `KERNEL_ARCHITECTURE.md` v1.2.1 (Part 0 К-L15 native-bus authority, К-L15.1 tier independence, К-L9 vanilla=mods facade) · `ENGINE_LIFECYCLE_AND_TRANSACTIONS.md` v1.0.2 (§4 fault classes — mod-origin contained / core-origin fail-fast on the delivery path) · `TIME_AND_CONSISTENCY_MODEL.md` v1.0.0 (visibility class table; row 9/10 native tiers "Dormant: no production call site today") · `GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md` (boundary law B-4) · `VANILLA_SEPARATION_MIGRATION_PLAN.md` v1.2.0 Live (BD-3/BD-10 rows §3; W2 §4) · W1 recon R4.

**Kickoff↔law conflict (one, recorded).** The kickoff names "`ISOLATION.md` (the stale 178-183 block F-54 named)" as an input. That block does not exist: the only `ISOLATION.md` in the tree is `docs/architecture/historical/ISOLATION.md`, **SUPERSEDED** (`superseded_by: DOC-A-MOD_OS_ARCHITECTURE`), 120 lines, containing no `_allowedBuses` text and no lines 178–183. The named input is orphaned; recorded as **A11**, not fabricated. No standing-law line was stopped — the measurement proceeds on the live MOD_OS successor.

---

## R1 — T1: Native channel identity (the BD-3 "dynamic type IDs" question)

### 1.1 Headline — two parallel bus systems, only one live

There are **two** event-bus systems, keyed differently, and they do not meet on the production hot path:

| | **System A — managed genre buses** | **System B — native three-tier bus** |
|---|---|---|
| Count / shape | 5 fixed domain buses (`Combat/Inventory/Magic/World/Pawns`) | 3 fixed tiers (`Fast/Normal/Background`) |
| Channel key | the C# **`System.Type` object** | **`uint32` FNV-1a-32 hash of the event FQN** (`type_id`) |
| Storage | `ConcurrentDictionary<Type, List<Subscription>>` per bus | `unordered_map<uint32_t, vector<…SubscriberRecord>>` per tier |
| Reaches native? | **No** — zero P/Invoke on this path | Yes — `df_bus_publish_*` |
| Live today? | **Yes — authoritative production dispatch** | **No — dormant** (`UseNativeBusForDispatch=false`, never flipped in `src/`) |

This is not an undocumented state: it is the canon. `EVENT_BUS.md:25` — "five managed domain buses carrying every production event today, and the sovereign native three-tier kernel bus (К-L15) that exists fully behind a C ABI but carries almost none of that traffic yet." `EVENT_BUS.md` §5 (`:92`) FENCES the sovereign switch as "target / planned — not current truth … `BusFacade` and the bridge are the cutover scaffolding for that switch, not cruft." `TIME_AND_CONSISTENCY_MODEL.md:121` (visibility class 9) — "Dormant: no production call site today — production events travel the managed path." **The measured plumbing agrees with the corpus.**

Consequence for BD-3: the five "genre buses" are a **purely managed `GameServices` construct with no native representation**; the "dynamic type IDs from a dynamic registry" the plan's BD-3 row envisions **already exist and idle** in System B. The BD-3 coupling to break is therefore overwhelmingly **managed-side** (§1.7).

### 1.2 System A — the authoritative managed publish path (no native hop)

Entry points and the hop-by-hop identity:

| Hop | Anchor | Channel key at this hop |
|---|---|---|
| Core publisher | e.g. `NeedsSystem.cs:125` `Services.Pawns.Publish(new NeedsCriticalEvent{…})` | domain fixed at compile time (which `IGameServices` property) |
| Mod/SDK publisher | `RestrictedModApi.cs:163` `ModBusRouter.Resolve(typeof(T), _services)` | interned **string** from `[EventBus("…")]` → `IGameServices` property |
| Aggregator | `GameServices.cs:23-35` (5 properties → 5 `DomainEventBus` wrappers `:76-129`) | property identity (the five-ness) |
| **Bus publish** | `DomainEventBus.cs:91-103` | **the `System.Type` object** (`typeof(TEvent)`), the dictionary key |

`GameServices` hardcodes the five buses and fans out lifecycle by hand (`GameServices.cs:16-20, 41-73`); each wrapper (`CombatBus`…`PawnBus`, `:76-129`) forwards to a private `DomainEventBus`. The bus keys on `Type` and resolves an orthogonal delivery axis (`DomainEventBus.cs:25, 27, 91-103, 240-245`):

```csharp
private static readonly ConcurrentDictionary<Type, DeliveryMode> ModeCache = new();
private readonly ConcurrentDictionary<Type, List<Subscription>> _handlers = new();
// …
public void Publish<TEvent>(TEvent evt) where TEvent : IEvent
{
    if (evt is null) throw new ArgumentNullException(nameof(evt));
    Type eventType = typeof(TEvent);
    if (GetDeliveryMode(eventType) == DeliveryMode.Deferred)
    {
        _deferred.Enqueue(new DeferredItem(eventType, evt));
        return;
    }
    DeliverSync(eventType, evt);
}
// … private enum DeliveryMode { Sync, Deferred, Immediate }
```

The chain **ends here** — no interop. Delivery faults route under the ELT §4 origin-asymmetric policy (`DomainEventBus.cs:161-192, 194-226` → `SystemExecutionContext.RouteFault`, §4.2).

### 1.3 System B — the native publish→`df_bus_*` chain (dormant, opt-in)

The only place a managed event *could* reach native, gated off by default (`BusFacade.cs:49, 61-62, 69-71, 98-108, 176-187`):

```csharp
public bool UseNativeBusForDispatch { get; set; } = false;      // :49 — never set true in src/
public uint GetOrAssignTypeId<T>() where T : IEvent              // :61 — FQN → uint32
    => _typeIdCache.GetOrAdd(typeof(T), static t => Fnv1a32(t.FullName ?? t.Name));
public BusTier GetTier<T>() where T : IEvent                     // :69 — [EventTier] → enum, default Normal
    => _tierCache.GetOrAdd(typeof(T), static t => t.GetCustomAttribute<EventTierAttribute>()?.Tier ?? BusTier.Normal);
public int Publish<T>(T evt) where T : unmanaged, IEvent
{
    if (!UseNativeBusForDispatch) return 0;                      // :100 — early no-op in production
    uint typeId = GetOrAssignTypeId<T>();
    BusTier tier = GetTier<T>();
    unsafe { T local = evt; return _bridge.PublishViaNative(typeId, tier, (IntPtr)(&local), (uint)sizeof(T)); }
}
```

Downstream hops (each records `uint32 type_id` as the only per-event key; tier is a fixed enum that selects *which* export):

- `ManagedBusBridge.cs:55-64` — the tier `switch` selects one of three publish exports:
  ```csharp
  public int PublishViaNative(uint typeId, BusTier tier, IntPtr payloadPtr, uint payloadSize, uint coalesceKey = 0)
      => tier switch {
          BusTier.Fast       => NativeMethods.df_bus_publish_fast(typeId, payloadPtr, payloadSize),
          BusTier.Normal     => NativeMethods.df_bus_publish_normal(typeId, payloadPtr, payloadSize),
          BusTier.Background => NativeMethods.df_bus_publish_background(typeId, payloadPtr, payloadSize, coalesceKey),
          _ => 0, };
  ```
- P/Invoke — `NativeMethods.Bus.cs:20-27` — `df_bus_publish_fast/normal/background(uint type_id, IntPtr payload, uint payload_size[, uint coalesce_key])`. Tier is encoded in the **symbol name**.
- C ABI — `bus_native.h:79-98` — same three exports; `uint32_t type_id` the only channel key.
- Native impl — `bus_fast.cpp:37` indexes the per-tier subscriber map by `type_id` (`tier.subscribers.find(type_id)`); Normal/Background enqueue a `Pending…EventRecord` carrying `type_id` for later drain.

### 1.4 Fixed tiers vs registered channels

**Definitive: the tier is FIXED; the per-event channel is REGISTERED (dynamic).**

- Tiers = a 3-value enum, wire format, three files: `EventTierAttribute.cs:9-32` (`enum BusTier { Fast=0, Normal=1, Background=2 }`), native `bus_native_internal.h:100-104` (`enum class TierTag : uint8_t { Fast=0, Normal=1, Background=2 }`), and the registry mirror in `event_type_registry.h`.
- Channels within a tier = **no fixed enum, no fixed array**. Each tier is a hash map auto-creating a slot on first subscribe (`bus_native_internal.h:57-78`):
  ```cpp
  struct FastTierState {
      std::mutex                                                       mutex;
      uint64_t                                                         next_seq = 1;
      std::unordered_map<uint32_t, std::vector<FastSubscriberRecord>>  subscribers;
  };  // Normal/Background identical + a `pending` vector each
  ```
  There is **no explicit "create channel" call** — `subscribers[type_id].push_back(...)` materializes the slot.
- The channel id (`type_id`) is assigned by `BusFacade.Fnv1a32` (`:176-187`), a **pure function of the FQN** (assembly-independent by design, `BusFacade.cs:30-36`). The subscription id (not the channel id) packs the tier into its high 8 bits (`bus_native_internal.h:96-112`, `TIER_SHIFT = 56`).

### 1.5 `df_bus_*` export inventory (from `bus_native.h`; 15 symbols + 1 adjacent)

| Symbol | Header | Class |
|---|---|---|
| `df_bus_publish_fast` / `_normal` / `_background` | `bus_native.h:79/86/94` | TRAFFIC (publish) |
| `df_bus_subscribe_fast` / `_normal` / `_background` | `:108/114/120` | LIFECYCLE (subscribe — implicit channel create) |
| `df_bus_unsubscribe` | `:130` | LIFECYCLE (unsubscribe by id) |
| `df_bus_unsubscribe_fast_by_mod` / `_normal_` / `_background_` | `:134-136` | LIFECYCLE (bulk-by-mod) — **no managed P/Invoke** |
| `df_bus_drain_normal_batch` | `:145` | TRAFFIC (drain) |
| `df_bus_subscriber_count_fast` / `_normal` / `_background` | `:151-153` | TRAFFIC (diagnostic) |
| `df_bus_clear` | `:155` | LIFECYCLE (teardown — clears all 3 tiers) |
| `df_background_queue_dispatch_idle_slot` | `background_queue.h` (bound `NativeMethods.Bus.cs:47`) | TRAFFIC (drain) — not a `df_bus_*` symbol |

The header itself is organized "Publish API — one per tier / Subscribe API — one per tier / Unsubscribe — single + bulk-by-mod" (`bus_native.h:70-128`). **There is no channel-create or channel-register export.** Channel lifecycle = subscribe / unsubscribe / clear only; everything else is traffic on already-materialized `(tier, type_id)` slots. Tier lifecycle is not an API concept — tiers are compile-time.

### 1.6 The Fast tier (K-L15)

A Fast channel is identified by resolving `[EventTier(BusTier.Fast)]` **once upstream** into a physically separate native path: `BusFacade.GetTier` (`:69-71`) → `ManagedBusBridge` `switch` (`:57-63`) → `df_bus_publish_fast`. Fast is the only tier with **synchronous, no-pending-queue** dispatch, callbacks fired outside the lock, returning the invoked-subscriber count (`bus_fast.cpp:30-52`), and a distinct callback signature `df_bus_fast_subscriber_fn(type_id, payload, size, user_data)` vs the batched `df_bus_batched_subscriber_fn(const df_managed_system_batch*)` for Normal/Background (`bus_native.h:52-64`). Latency contract (≤1ms; `KERNEL_ARCHITECTURE.md` Part 0 К-L15) is policed by a separate managed monitor keyed by `(modId, eventFqn)` (`FastTierContractMonitor.cs`). **Measured, not solved:** making channel identity `(provider, event-FQN)`-derived would touch the tier-selection sites in §1.7 Group A; the Fast callback-signature split and the FNV-1a `type_id` (Group C) are the concrete Fast coupling points.

### 1.7 Coupling points if channel identity became `(provider, FQN)`-derived

~30 distinct definition/edit sites (≈46 raw, expanding the multi-site item), grouped by which assumption they encode:

- **Group A — fixed 3-tier enum / per-tier duplication (17):** `EventTierAttribute.cs:9-32`; native `TierTag`/registry enums (`bus_native_internal.h:100-104`, `event_type_registry.h`); `ManagedBusBridge.cs:57-63` (publish switch) + `:73-92` (3 subscribe methods) + `:120-126` (count switch); `BusFacade.cs:132-148` (Background coalesce guard); `KernelCapabilityRegistry.cs:112-131` (per-tier token prefix); `SubscriberContractValidator.cs:110-116`; `EventTypeRegistryInterop.cs:48-66`; `NativeMethods.Bus.cs:20-57` (3× publish/subscribe/count); `bus_native_internal.h:57-78` (3 state structs); `bus_common.cpp:21-34, 44-64, 68-85` (3 singletons, unsubscribe tier-decode, clear order); the 3 per-tier `.cpp` TUs; `mod_unload.cpp:101-111` + the 3 `_by_mod` exports.
- **Group B — the fixed 5 managed buses (9 definition sites + ~17 call sites):** `GameServices.cs:16-20, 23-35, 41-73, 76-129`; `IGameServices.cs:13-49` + the 5 marker interfaces; `EventBusAttribute.cs`; `ModBusRouter.cs:28-47`; the ~17 core publisher call sites naming a domain (enumerated in §3.1 / R3).
- **Group C — `type_id = FNV1a32(FQN-only)` (4):** `BusFacade.cs:61-62, 176-187`; `EventTypeRegistryInterop.cs:48-66`; `event_type_registry.cpp` (registry keyed by `type_id`, 1 FQN ↔ 1 id assumption); `DomainEventBus.cs:25, 27` (managed authoritative bus keyed by `Type`, no provider/FQN notion — a provider-derived id would need a parallel index here).

Note the plan's BD-3 target id scheme is `(providerId, schemaId)` (plan `:88, :128`); today's scheme is `FNV1a32(FQN-only)` — a **32-bit hash with no collision handling** (A6).

---

## R2 — T2: Capability registry mechanics + name census (BD-10)

### 2.1 `KernelCapabilityRegistry` mechanism

`KernelCapabilityRegistry` (`internal sealed`, `src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs`, 173 lines) builds its provider set once, by reflection, over three assemblies (`:82-89`):

```csharp
internal static KernelCapabilityRegistry BuildFromKernelAssemblies()
    => new(new[] {
        typeof(IEvent).Assembly,           // DualFrontier.Contracts
        typeof(IComponent).Assembly,       // DualFrontier.Contracts (deduped)
        typeof(HealthComponent).Assembly,  // DualFrontier.Components
        typeof(PawnSpawnedEvent).Assembly, // DualFrontier.Events
    });
```

Scan filter (`:91-101`): `type.IsPublic` ∧ `!type.IsAbstract` ∧ `FullName != null` ∧ no `` ` `` (generic) ∧ no `+` (nested). Markers: `IEvent` (+ `[EventTier]`), `IComponent` + `[ModAccessible]`, `[Layer]`. This is the `MOD_OS_ARCHITECTURE.md` §3.4 kernel-provided set (`:216-225`): "built once at pipeline construction by a reflection scan … over `DualFrontier.Contracts`, `DualFrontier.Components`, and `DualFrontier.Events`."

Consumption: instantiated at `ModIntegrationPipeline.cs:83`; queried at load by `ContractValidator` Phase C (`:400-435`, `kernelCapabilities.Provides(token)`) and Phase D (`:444-471`, `[ModCapabilities]` cross-check); surfaced to mods via `RestrictedModApi.GetKernelCapabilities()` (`:203`). **Note:** runtime `EnforceCapability` (§3.3) does *not* consult the registry — it checks the mod's own manifest set.

### 2.2 The name grammar — 12 emitted patterns (wildcard always `type.FullName`)

| # | Anchor | Emitted | Gate |
|---|---|---|---|
| 1–2 | `:115-116` | `kernel.fast.publish:{FQN}` / `kernel.fast.subscribe:{FQN}` | `[EventTier(Fast)]` |
| 3–4 | `:119-120` | `kernel.normal.publish:{FQN}` / `kernel.normal.subscribe:{FQN}` | Normal (default) |
| 5–6 | `:124-125` | `kernel.publish:{FQN}` / `kernel.subscribe:{FQN}` | Normal **only** (legacy alias) |
| 7–8 | `:128-129` | `kernel.background.publish:{FQN}` / `kernel.background.subscribe:{FQN}` | Background |
| 9–10 | `:141, :143` | `kernel.read:{FQN}` / `kernel.write:{FQN}` | `[ModAccessible(Read/Write)]` |
| 11–12 | `:158, :161` | `kernel.layer.intent:{FQN}` / `kernel.layer.combat_feedback:{FQN}` | `[Layer(Intent/CombatFeedback)]` |

The manifest-side grammar a mod *may declare* (`ManifestCapabilities.cs:24-26`) is a **superset** — it also admits `kernel.field.(read|write|acquire|conductivity|storage|dispatch)`, `kernel.pipeline.register`, and the `mod.<id>.<verb>:` provider namespace, **none of which the registry emits** (A7). Provider namespaces: `kernel` (kernel's own set) vs `mod.<modId>` (another mod), reserved and non-overlapping per `MOD_OS_ARCHITECTURE.md` §3.3 (`:214`). Granularity is strictly per-type — no wildcards (`MOD_OS` §3.2, `:186`).

### 2.3 Capability name census (the BD-10 blast radius, measured)

**On-disk manifests — 0 capability entries.** All 6 vanilla mods (e.g. `mods/DualFrontier.Mod.Vanilla.Combat/mod.manifest.json:19-22` → `"required": [], "provided": []`), the Example mod, and all 19 test fixtures declare zero `kernel.*` capabilities (several fixtures omit the block entirely). The declaration surface is **dormant on disk**.

**Inline JSON-string manifests in tests — 4 entries:** `ManifestParserTests.cs:35` (`kernel.publish:A.B.DamageEvent`), `:36` (`mod.com.example.combat.publish:A.B.WeaponDef`); `ManifestRewriterTests.cs:134` (`kernel.read:Foo.Bar`), `:135` (`mod.preserves.publish:Baz`).

**IMPLICIT computed set — EXACTLY 208 tokens, 100% genre-FQN.** No manifest declares them; the registry emits them by scanning the vanilla assemblies. Measured at HEAD:
- **Events:** `rg ": IEvent" src/DualFrontier.Events` → **42** types (one per file); `rg "EventTier" src/DualFrontier.Events` → **0** — every event defaults to Normal, emitting all 4 Normal tokens. **42 × 4 = 168.**
- **Components:** `rg "ModAccessible\(" src/DualFrontier.Components` → **23** component annotations = **6 read-only** (`Read=true`) + **17 read-write**. **6×1 + 17×2 = 40.**
- **Layers:** patterns exist but **0** production types carry `[Layer(...)]` (production display layers use the `Layer` base class, which the registry does not scan) → **0**.
- **Total = 168 + 40 = 208 `kernel.*` tokens, all keyed on `DualFrontier.Events.*` / `DualFrontier.Components.*` genre FQNs** (Combat, Magic, Pawn, World, Inventory, Building, Items, Shared). This is the BD-10 rename blast radius — the exact set that must move from `kernel.*` to `mod.<vanilla-id>.*` for the plan's W2 gate ("kernel capability surface contains zero Pawn/Combat/Magic/Inventory names", plan `:129`).

**Reconciliation with the plan's inventory.** Plan `:168-169` lists "`DualFrontier.Components` — 28 types" and "`DualFrontier.Events` — 53 event records." Those are full-assembly inventories; the token-emitting subset is smaller: **42** public-concrete `IEvent` types and **23** `[ModAccessible]` components. The delta (53→42 events, 28→23 components) is types that don't carry the marker or aren't scan-eligible; it is not drift, but the report states the measured subset explicitly so the 208 figure is defensible.

### 2.4 The two publication paths

- **HARNESS / vanilla** — *implicit reflection.* `GameBootstrap` registers built-in systems/components but produces **no capability tokens of its own** (systems aren't `IEvent`/`IComponent`/`[Layer]`); vanilla capabilities exist purely because `BuildFromKernelAssemblies` scans the assemblies. **No manifest, no validation, no runtime enforcement** — a vanilla type "provides" its `kernel.*` token merely by being public+concrete in a scanned assembly.
- **MOD** — *explicit declaration.* `manifest.capabilities.required/provided`, validated at load (Phase C/D, `ContractValidator.cs:400-471`), enforced at runtime (`RestrictedModApi.EnforceCapability`, §3.3).
- **Convergence:** both meet at the single registry instance (`ModIntegrationPipeline.cs:83`): the vanilla side *populates* the provider set; the mod side *queries* it. Because the provider set is the vanilla scan, **every satisfiable `kernel.*` token a mod can legally declare today is a vanilla/genre FQN** — the BD-10 problem in one sentence.

---

## R3 — T3: Genre-bus consumer closure beyond the 28 systems

Classification throughout: **SHAPE** = depends on the five-ness (property names, interface types, the fixed count); **TRAFFIC** = publishes/subscribes without caring about the five-ness.

### 3.1 Consumer closure outside the 28 systems

**SHAPE consumers:**
- `ModBusRouter` (`:18, 28-47`) — the single point that turns the five-ness into runtime routing, **data-driven off `IGameServices`' property set** (so it auto-tracks any bus add/remove):
  ```csharp
  internal static object? Resolve(Type eventType, IGameServices services) {
      EventBusAttribute? attr = eventType.GetCustomAttribute<EventBusAttribute>(inherit: false);
      if (attr is null) return null;
      if (!_busProperties.TryGetValue(attr.BusName, out PropertyInfo? prop)) return null;
      return prop.GetValue(services);
  }
  private static Dictionary<string, PropertyInfo> BuildMap() {
      var map = new Dictionary<string, PropertyInfo>(StringComparer.Ordinal);
      foreach (PropertyInfo prop in typeof(IGameServices).GetProperties(BindingFlags.Public | BindingFlags.Instance))
          map.Add(prop.Name, prop);
      return map;
  }
  ```
- `GameServices` (`:14-129`) — hardcodes the 5 buses/props/wrappers + hand-fans `Clear/FlushDeferred/DropDeferred`; the **only** code naming the five bus *interface types* besides the interface declarations.
- Taxonomy defs — `IGameServices.cs:13-49` (5 properties); the 5 empty marker interfaces (`ICombatBus.cs:11` `interface ICombatBus : IEventBus {}` … **0 declared members each**); `EventBusAttribute.cs` (its whole purpose is naming an `IGameServices` property).
- `GameBootstrap` wiring — constructs `new GameServices()` and subscribes on specific properties (`.Pawns`/`.Combat`, ~6 sites incl. `GameBootstrap.cs:265` `services.Pawns.Publish(new ItemSpawnedEvent…)`); `RandomPawnFactory.cs:159` (`services.Pawns.Publish`).

**TRAFFIC consumers (five-ness-agnostic):** the scheduler / `SystemExecutionContext` / `SystemBase` passthrough (`_services` opaque); the **entire mod/SDK path** — `RestrictedModApi` (treats `_services` as an opaque routing target), `SystemContextView`, `ISystemContext`, `IModApi`; and the capability validators (`ContractValidator`, `ManifestCapabilities`, `KernelCapabilityRegistry`) which key on the **event FQN**, never the bus. **`ISystemServices` is bus-free** — its only member is `IPathfindingService Pathfinding` (the W1 construction-time surface deliberately excludes the buses). So the W1 SDK surface reaches events *only* as traffic, never as shape.

### 3.2 The `[SystemAccess]` consumer list

Definition `SystemAccessAttribute.cs`: `Reads:27`, `Writes:33`, `Bus:41`, `Buses:49`; doc `:8-14` names the live readers. Every reader at HEAD:

| Reader | Anchor | Reads | Purpose |
|---|---|---|---|
| `SystemBase.AccessDeclaration` | `SystemBase.cs:71-72` | the attribute | reflection primitive (SDK-forwardable hook) |
| `SystemAdapter` | `SystemAdapter.cs:48-49, 65` | cached attribute | W1 BD-1 forwarding of the wrapped `ISimulationSystem` |
| `DependencyGraph` | `DependencyGraph.cs:54, 84, 87, 91` | `Reads`, `Writes` | edge-building (write-write + write→read) |
| `ParallelSystemScheduler` | `ParallelSystemScheduler.cs:284-288, 308` | `Buses` | passes `attr.Buses` as `allowedBuses` → **dead field** (R4) |
| `ContractValidator` | `ContractValidator.cs:283-291` | `Writes` | Phase B write-write conflict |
| `ModRegistry` | `ModRegistry.cs:181-189` | presence only | registration gate (value discarded) |

`Reads`→graph edges. `Writes`→edges + conflict. `Buses`→the dead `_allowedBuses`. `Bus`→never read as a field (only doc/error-string `nameof(IGameServices.X)` literals). Native does **not** read `[SystemAccess]` yet (`GameBootstrap.cs:186-192` registers with `ReadOnlySpan<uint>.Empty`, "К10.2 will marshal … once a per-system component-id resolver is wired").

### 3.3 The W1 SDK live gate chain (Publish/Subscribe) — F-54-corrected

The live gate is `kernel.{publish|subscribe}:<FQN>` via `RestrictedModApi.EnforceCapability`; **there is no allowed-bus list on this path.** Full chain (`RestrictedModApi.cs:157-189, 230-247`):

```csharp
public void Publish<T>(T evt) where T : IEvent {
    if (evt is null) throw new ArgumentNullException(nameof(evt));
    EnforceCapability("publish", typeof(T));                       // ← GATE
    if (ModBusRouter.Resolve(typeof(T), _services) is IEventBus bus)
        bus.Publish(evt);                                          // ← ROUTE (after gate)
}
private void EnforceCapability(string verb, Type eventType) {
    string token = $"kernel.{verb}:{eventType.FullName}";          // per-FQN, un-tiered
    if (_manifest.Capabilities.IsEmpty) { Console.WriteLine(/* v1 grace-period warning */); return; }
    if (!_manifest.Capabilities.RequiresCapability(token))
        throw new CapabilityViolationException(
            $"Mod '{_modId}' attempted to {verb} '{eventType.FullName}' without declaring capability '{token}'.");
}
```

Chain of custody: `ISystemContext.Publish` (`ISystemContext.cs:105`) → `SystemContextView.cs:100-107` `RequireApi().Publish` → `ModRegistry.GetModApi(_modId)` → the same `RestrictedModApi`. **The gate decides on `(mod id, event FQN)` only; the bus is resolved *after* the gate and plays no role in the decision.** Confirmed by `SdkContextTests.cs:177-200` (asserts the thrown message contains `kernel.publish:`). Two forms diverge (A8): the runtime token is always the **un-tiered** `kernel.{verb}:{FQN}`, so a Fast/Background-only declared token (no legacy alias emitted, §2.2) would pass load-time Phase C but miss the runtime lookup.

---

## R4 — T4: The F-54 fork surface

### 4.1 `_allowedBuses` — the retire-cost census

`rg "_allowedBuses"` → **exactly 2 code sites, both in `SystemExecutionContext.cs`:**

```csharp
:38    private readonly IReadOnlyList<string> _allowedBuses;   // declaration
:83        _allowedBuses = buses;                              // sole write (ctor)
```

- **Write sites: 1** (`:83`, in the ctor; the value built `:80-82` from the `allowedBuses` param). **Read sites of the field: 0.** Never read after assignment, anywhere in the tree. **Prior finding CONFIRMED: captured-but-unread (dead).**
- The full wiring is a dead end: `[SystemAccess].Buses → ParallelSystemScheduler.BuildContext (attr.Buses, :308) → ctor param allowedBuses (:67, null-checked :76, iterated :81) → _allowedBuses (:83) → (nothing)`. `attr.Buses`' **only** runtime consumer is this dead field.
- **Retire cost:** delete 1 field decl + 1 write; drop the `allowedBuses` ctor param (`:58 doc, :67, :76, :81`); drop the `attr.Buses` argument at `ParallelSystemScheduler.cs:308`; update the test ctor calls (e.g. `SdkContextTests.cs:100 new SystemExecutionContext("T", new[]{ "World" }, …)`). `[SystemAccess].Bus`/`.Buses` themselves may remain (they still print in diagnostics and pin the taxonomy) but would have no runtime consumer.
- **Docs referencing the concept** (all describe it as unimplemented/dead): `docs/governance/AUDIT_TRAIL.yaml` ("stays captured-but-unread, W2/BD-3 decides its fate"), `docs/TRANSLATION_GLOSSARY.md` ("captured but never validated … documentation-only specification … Not yet implemented"), `docs/reports/NORMALIZATION_REPORT.md` ("No corresponding code exists"), `docs/audit/PASS_4_REPORT.md` ("doc-only pre-spec … NOT YET IMPLEMENTED"), plus `W1_SDK_SURFACE_RECON_REPORT.md`, `ROADMAP.md`, `REGISTER.yaml`. Two briefs describe a stale shape ("HashSets", "_allowedReads/_allowedWrites/_allowedBuses") — at HEAD only `_allowedBuses` survives, and as `IReadOnlyList<string>` (A9).

### 4.2 System-identity visibility (F-54 revival attachment)

If per-**system** (not per-mod) scoping were revived, the gate would need the *publishing system's* identity. **Measured: the gate sees mod identity + event FQN only; per-system identity exists on the thread-local context but is unexposed.**

- `RestrictedModApi.EnforceCapability` uses only `_modId` + `typeof(T)` (`:232, 245`) — **mod-granular**. `Publish` never touches `SystemExecutionContext.Current` (`:157-165`); `Subscribe` captures it only to re-push around the handler (`:177-185`), reading no identity.
- `SystemContextView` is mod-granular (holds `_modId` only; resolves the API by mod id).
- Where per-system identity *does* live at that moment: the thread-local `SystemExecutionContext.Current` carries `_systemName` (`:37`, set to `systemType.FullName` at `ParallelSystemScheduler.cs:307`) **beside** `_allowedBuses` (`:38`) — both populated in the same `BuildContext` call. But `SystemExecutionContext` exposes **no accessor** for either: `_systemName` is read only inside a `RouteFault` DEBUG string (`:153`), `_allowedBuses` nowhere. Its surface is `Services` / `NativeWorld` / `Current` / `PushContext`/`PopContext` / `RouteFault` / `ResolveManagedStore`.
- **Net:** revival requires **surfacing new state** — either exposing `_systemName`/`_allowedBuses` off `SystemExecutionContext.Current`, or threading a system id into the mod-granular `RestrictedModApi`/`SystemContextView`. `SystemExecutionContext` is the natural attachment point (identity + the dead scoping list already coexist there, per-system, populated per `BuildContext`), but nothing is readable today.

### 4.3 The orphaned ISOLATION pin + the double enforcement gap

The kickoff's `_allowedBuses` backing spec ("`ISOLATION.md` 178–183") is orphaned: `historical/ISOLATION.md` is SUPERSEDED (→ `DOC-A-MOD_OS_ARCHITECTURE`), 120 lines, no `_allowedBuses`, no lines 178–183 (A11). The `[SystemAccess]` attribute's own doc still points at it (`SystemAccessAttribute.cs:13-14`, "ISOLATION.md sections 11.6 and 11.7"). And the enforcement it once promised exists at **neither** layer: **build-time** — `CONTRACTS.md` §6 (`:143-145`): "None of the 17 [analyzer] rules is a bus-publish-scoping check … That verification remains dependency-graph-only today — a real gap"; **runtime** — `_allowedBuses` is dead. So today bus-publication scoping is unenforced everywhere; F-54's "revive" fork would be *net-new* enforcement, not a restoration (A10).

---

## R5 — T5: Scale, decision inputs, anomalies

### 5.1 Scale counts (grep-pinned at HEAD `b2805ea`)

| Metric | Value | Source |
|---|---|---|
| `IEventBus` declared members | 3 (`Publish`/`Subscribe`/`Unsubscribe`) | `IEventBus.cs:23,30,37` |
| Each of 5 genre bus interfaces | 0 declared (inherit 3) | `ICombatBus.cs:11` etc. |
| `IGameServices` members | 5 | `IGameServices.cs:20,27,34,41,48` |
| `IGameServices` occurrences | 96 / 62 files | `rg -c IGameServices --glob '*.cs'` |
| — production files | 43 (28 systems + 1 mod + 14 non-system) | `rg -l … src/ mods/` |
| — test files | 19 | `rg -l … tests/` |
| Bus interface *types* consumers | 1 (`GameServices` impl only) | `rg "ICombatBus\|IInventoryBus\|IMagicBus\|IWorldBus\|IPawnBus"` |
| `nameof(IGameServices.X)` — prod | 37 occ (32 across the 28 systems + 1 `ExampleSystem` + 4 doc/error) | `rg -o 'nameof\(IGameServices\.\w+'` |
| `nameof(IGameServices.X)` — test | 17 | same |
| Manifest capability entries (on-disk / inline) | 0 / 4 | §2.3 |
| Implicit `kernel.*` tokens | 208 (168 event + 40 component) | §2.3 |
| Events carrying `[EventTier]` | 0 | `rg EventTier src/DualFrontier.Events` |
| BD-3 native-coupling points | ~30 (≈46 raw) | §1.7 |

**W1 "28" reconciliation:** W1 R4's "28 `nameof(IGameServices.X)` production bindings" counted *systems carrying a bus binding*. At HEAD `b2805ea` there are still 28 such systems, but **37** production `nameof` occurrences (multi-bus systems bind 2–3 each = 32; + `ExampleSystem` = 33; + 4 doc/error literals = 37). `ExampleSystem` is a W1 artifact absent at W1's own HEAD `df1541d`. No drift — different metrics.

### 5.2 Decision-input list (empirical forks, no leans)

**BD-3 (generic event routing).**
- The five genre buses have **no native representation** and (per W1 R4) almost no live traffic — 3 of 5 (Combat/Magic/World) carry zero live system producers. The "dynamic type IDs from a dynamic registry" the plan wants already exist in System B (dormant).
- What breaks at each coupling point: §1.7 Groups A/B/C — ~30 sites, dominated by the managed five-bus construct (Group B) and the fixed 3-tier duplication (Group A), not by inventing native machinery.
- Removing `IGameServices` properties is a **breaking, major-bump** change (`CONTRACTS.md` §4, `:112`) — the interface has no default bodies; `GameServices` and any mod test-double would fail to compile.
- Bridge need until W5: the ~10 REAL harness systems (per W1) publish via `Services.<Bus>.Publish`; any BD-3 cutover must keep a `Services.X`-shaped bridge (or migrate those call sites) until the harness systems relocate at W5.
- Id-scheme fork: today `FNV1a32(FQN-only)`, 32-bit, no collision handling; plan target `(providerId, schemaId)`.

**BD-10 (kernel.* reframing).**
- Rename blast radius = **exactly 208 implicit tokens**, 100% genre-FQN (§2.3), emitted with no manifest / no validation / no enforcement.
- The `mod.<modId>.*` provider namespace BD-10 wants vanilla types to move into **already exists** in the grammar (`MOD_OS` §3.3); the registry relocation (out of `DualFrontier.Application`, plan `:95, :176`) and the scan-scope change (stop scanning `DualFrontier.Events`/`Components` as kernel) are the levers.
- Two publication paths (§2.4): harness = implicit reflection (unvalidated), mod = explicit+validated. A ledger that publishes "engine capabilities only" must decide what the harness/vanilla types register *as* (mod-namespace) and who validates them.

**F-54 (revive vs retire `_allowedBuses`).**
- Retire cost: tiny and mechanical (§4.1) — 1 field + 1 write + 1 ctor param + 1 scheduler arg + test-ctor updates; strands `[SystemAccess].Buses`' only consumer.
- Revive cost: net-new (§4.2/§4.3) — no runtime read exists to restore, the backing spec is orphaned, system identity is unexposed at the gate, and neither the build-time analyzer nor the runtime gate scopes by bus today.

### 5.3 Anomalies

- **A1 — Two parallel bus systems; the authoritative managed path never reaches native** (`UseNativeBusForDispatch=false`, `BusFacade.cs:49/100`; never flipped in `src/`). *Corroborated by canon* (EVENT_BUS §4/§5-FENCED, TCM row 9) — a state-of-the-world finding, not an undocumented gap.
- **A2 — The native bus is already generic dynamic-type-id routing** (channels hash-keyed, auto-created on subscribe); only the 3 tiers are fixed. The five "genre buses" have no native form.
- **A3 — Zero events carry `[EventTier]`** (`rg EventTier src/DualFrontier.Events` = 0). The entire Fast/Background tier machinery (separate exports, queues, callback signatures, capability prefixes) has **no shipped producer**; exercised only by tests. Every vanilla event is Normal-tier.
- **A4 — `type_id = FNV1a32(FQN-only)` is a 32-bit hash with no collision handling** (`BusFacade.cs:176-187`). Two colliding FQNs would silently share a native channel; the id-space assumption (1 FQN ↔ 1 id, `event_type_registry.cpp`) breaks under growth (esp. mod-provided types). Plan target `(providerId, schemaId)` differs.
- **A5 — 208 implicit `kernel.*` tokens, 100% genre-FQN, 0 declared in any shipped manifest.** The capability model's entire live surface is a reflection artifact over vanilla assemblies; the declaration/enforcement machinery is dormant on disk (BD-10 root).
- **A6 — Runtime gate can't match tiered-only tokens.** `EnforceCapability` always builds the un-tiered `kernel.{verb}:{FQN}` (`:232`); a Fast/Background-only event (no legacy alias emitted) passes load-time Phase C but misses the runtime lookup.
- **A7 — Manifest grammar is a superset of the registry's emissions.** `kernel.field.*` and `kernel.pipeline.register` are accepted by the pattern (`ManifestCapabilities.cs:24-26`) but never kernel-emitted — structurally un-providable by the kernel (satisfiable only via a dependency's `provided` set).
- **A8 — `SubscriberContractValidator` is dead in production** — parses tier tokens but is instantiated only by tests; its error kinds (`BusTierMismatch`/`BackgroundCoalesceMissing`/`FastTierContractViolation`) never fire at load.
- **A9 — `_allowedBuses` fully wired to a dead end**; retiring it strands `[SystemAccess].Buses`' only consumer. Two briefs describe a stale shape (HashSets / `_allowedReads`/`_allowedWrites`); at HEAD only `_allowedBuses` survives, as `IReadOnlyList<string>`.
- **A10 — Bus-publication scoping is unenforced at BOTH layers** — build (`CONTRACTS.md` §6: no analyzer rule) and runtime (`_allowedBuses` dead). F-54 "revive" would be net-new enforcement, not restoration.
- **A11 — Kickoff input pin orphaned** — `ISOLATION.md` "178–183" does not exist (doc SUPERSEDED → MOD_OS; 120 lines). `SystemAccessAttribute.cs:13-14` and four other docs still cite the superseded doc.
- **A12 — `ItemSpawnedEvent` routes off-taxonomy** — carries no `[EventBus]`, published directly via `services.Pawns.Publish` (`GameBootstrap.cs:265`), bypassing `ModBusRouter`; its doc comment names a future "Items bus" (a documented pressure point on the fixed five-ness).
- **A13 — `df_bus_*_by_mod` exports unbound managed-side** — the 3 bulk-unsubscribe exports (`bus_native.h:134-136`) are consumed natively (`mod_unload.cpp:101-111`) but have no P/Invoke in `NativeMethods.Bus.cs`. Also: two sources of truth for tier (`BusFacade` attribute reflection vs the native `event_type_registry`), which can drift.

---

## Attestation

- **Read-only law honored.** One file authored (this report). Zero git mutations; `sync` never run; zero builds; zero tests executed.
- **HEAD.** `main` @ `b2805ea3af0a16668ae3af3e1527955cfaf89cf1` (`b2805ea`), working tree clean; the exact post-W1 merge (PR #47) the kickoff names. Every figure anchors to this tree.
- **Method.** Load-bearing chains re-verified **first-hand** at this HEAD with zero drift against the recon sweep: the Publish→native chain incl. the `UseNativeBusForDispatch=false` default (`DomainEventBus.cs`, `BusFacade.cs`, `ManagedBusBridge.cs`, `NativeMethods.Bus.cs`, `bus_native.h`, `bus_native_internal.h`); `KernelCapabilityRegistry` scan + 12-pattern grammar; `RestrictedModApi.EnforceCapability`; `ModBusRouter.BuildMap/Resolve`; both `_allowedBuses` sites + `ParallelSystemScheduler.BuildContext`; `GameServices`; the event/component census counts. The broader inventory (secondary consumer/reader lists) was measured via the read-only recon sweep.
- **Kickoff↔law conflict.** One, recorded as A11 (orphaned `ISOLATION.md` pin); no standing-law line stopped.
- **Scope discipline.** Measurement only — no designs, no recommendations, no leans. Decision-inputs (§5.2) state empirical forks for the BD-3/BD-10/F-54 deliberation to weigh.
- **Enrollment.** DOC-E Tier 3, UNTRACKED at authoring — enrolled at the next cascade's C1.
- Wall-clock best-effort; partial-but-honest over padded-complete.
