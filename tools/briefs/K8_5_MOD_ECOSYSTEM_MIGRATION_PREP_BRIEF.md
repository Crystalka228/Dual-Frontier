---
register_id: DOC-D-K8_5
project: Dual Frontier
category: D
tier: 3
lifecycle: AUTHORED-SKELETON
owner: Crystalka
version: 0.1
first_authored: 2026-05-18
last_modified: 2026-05-18
content_language: en
next_review_due: null
title: K8.5 — Mod Ecosystem Migration Prep
last_modified_commit: 4bc34c1
review_cadence: on-status-transition
last_review_date: 2026-05-18
last_review_event: Reclassified AUTHORED → AUTHORED-SKELETON per K8.5 deferral cascade (actual content skeleton-grade; mod authors audience deferred к post-Phase B)
reviewer: Crystalka
risks_referenced:
- RISK-005
- RISK-007
special_case_rationale: "Skeleton brief awaiting full brief authoring at proper milestone timing. Content (mod ecosystem migration prep from v2 to v3) premised on external mod authors audience; vanilla mods deferred к Phase B per composite namespace ratification (PR #34, 2026-05-16) means no current audience. Promotion к AUTHORED triggers when Phase B initial M-series sprint begins establishing mod author audience. Deferred from Phase А'.6 slot 2026-05-18 per Crystalka direction."
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