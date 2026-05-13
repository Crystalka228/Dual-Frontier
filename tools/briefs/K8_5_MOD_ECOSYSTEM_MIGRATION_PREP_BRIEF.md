---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K8_5
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_5
---
# K8.5 — Mod ecosystem migration prep (documentation + guide)

**Status**: SKELETON
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` K-L9 (mod parity), MOD_OS_ARCHITECTURE.md §4.6 (Mod API v3, post-K8.4)
**Prerequisite**: K8.4 closure (Mod API v3 shipped)

## Goal

Documentation deliverables enabling existing mod authors to migrate from v2 to v3. Project does not yet have an external mod ecosystem (game pre-release), but documentation is authored against the assumption that a future mod ecosystem will need it. This is forward-looking infrastructure work.

## Time estimate

3-5 days at hobby pace.

## Deliverables (high-level)

- `docs/MOD_AUTHORING_GUIDE.md` rewritten for v3 (NativeWorld access patterns, SpanLease/WriteBatch usage, performance considerations, debugging tips)
- `docs/MOD_API_V2_TO_V3_MIGRATION.md` step-by-step migration guide
- Sample mod project updated: `samples/SampleMod/` migrated v2 → v3 as exemplar
- DEPRECATED notices in any v2 reference documentation (e.g., MOD_OS_ARCHITECTURE.md §4.6 v2 sections explicitly marked DEPRECATED with v3 cross-reference)
- Bridge mechanism documentation in `docs/MOD_AUTHORING_GUIDE.md`: per-component path choice criterion (Path α default, Path β opt-in via `[ManagedStorage]`); decision tree (when to use struct vs class); K8.1 primitive coverage table (which managed types convert to which K8.1 primitive); concrete examples (Identity-style InternedString, Skills-style NativeMap, Storage-style NativeMap+NativeSet, Movement-style NativeComposite); managed-path examples (job queue, AI working memory, animation state — runtime-only patterns).
- Dual-API access pattern documentation: `SystemBase.NativeWorld.AcquireSpan<T>()` + `WriteBatch<T>` for Path α; `SystemBase.ManagedStore<T>()` for Path β; mixed-path system examples (system reads native PositionComponent + writes managed JobQueueComponent on same entity); cross-mod managed-path NOT-allowed pattern (compile-time barrier via ALC) + event-mediated alternative.
- v2 → v3 migration guide includes: how to add `[ManagedStorage]` to existing class components, how to call `RegisterManagedComponent<T>` instead of `RegisterComponent<T>` for class types, how to access per-mod `ManagedStore<T>` from systems via `SystemBase.ManagedStore<T>()`.
- Optional: video/blog-post outline if Crystalka wants to publish migration content (deferred decision)

## TODO

- [ ] Author full brief
- [ ] Sample mod project status (does it exist post-K8.4? If not, K8.5 creates it)
- [ ] Identify all v2 documentation that needs DEPRECATED notice or rewrite
- [ ] Bridge documentation completeness audit — every K-L3.1 lock (Q1–Q6) documented with concrete example

**Brief authoring trigger**: after K8.4 closure.
