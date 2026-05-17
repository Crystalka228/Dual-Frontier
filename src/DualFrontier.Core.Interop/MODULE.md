---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-CORE-INTEROP
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-CORE-INTEROP
---
# DualFrontier.Core.Interop — Module Documentation

**Purpose**: P/Invoke bridge layer между managed Application и native `DualFrontier.Core.Native.dll`. Translates managed API calls к C ABI invocations с appropriate marshalling.

**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` §1.2

**Public API surface** (post-K8):
- `NativeWorld` — managed handle wrapper (`IDisposable`)
- `WriteCommandBuffer` — mod mutation accumulator
- `SpanLease<T>` — span lifetime guard (`IDisposable`)
- `ComponentTypeRegistry` — explicit type-id registration

**Dependencies**:
- BCL only (`System.Runtime.InteropServices`, `System.Numerics`)
- Project reference: `DualFrontier.Contracts`
- Native dependency: `DualFrontier.Core.Native.dll` (loaded at runtime)

**Layering**: bridge layer — Domain calls Interop, Interop calls Native. Domain не calls Native directly.

**TODO list** (post-K8.2v2 closure 2026-05-09):
- [x] K0: cherry-pick existing project from experimental branch
- [x] K1: bulk operations + Span<T> primitives
- [x] K2: ComponentTypeRegistry replaces FNV-1a hash
- [x] K5: WriteCommandBuffer + SpanLease<T> production version
- [x] K8.1: NativeMap<K,V> / NativeComposite / NativeSet wrapper value types
- [x] K8.1.1: InternedString refactor + mod-scope test isolation
- [x] K8.2 v2: kernel-side foundation closure (component conversions + ModAccessible pass)
- [x] K9: RawTileField field-storage abstraction (DONE A'.4 — 2026-05-11)
- [x] K8.3+K8.4 (combined): 10 production systems on NativeWorld span/batch; managed World retired to ManagedTestWorld; Path β `ManagedStore<T>` plumbing for K-L3.1 bridge; Mod API v3 strict ships (DONE A'.5 — 2026-05-14, commits `24e5f56..fc8ecb6`)
- [ ] K8.5: mod-ecosystem migration prep (pending — A'.6)

**Status**: production wrappers operational post-A'.5 K8.3+K8.4 (NativeWorld sole production storage + WriteCommandBuffer + SpanLease<T> + ComponentTypeRegistry + InternedString + NativeMap/NativeComposite/NativeSet + RawTileField + Path β ManagedStore<T> bridge). K8.5 pending per `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md`.
