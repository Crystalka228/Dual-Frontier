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
- Optional: video/blog-post outline if Crystalka wants to publish migration content (deferred decision)

## TODO

- [ ] Author full brief
- [ ] Sample mod project status (does it exist post-K8.4? If not, K8.5 creates it)
- [ ] Identify all v2 documentation that needs DEPRECATED notice or rewrite

**Brief authoring trigger**: after K8.4 closure.
