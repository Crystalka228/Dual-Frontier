# K9 — Field storage abstraction

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K9
**Specification**: `docs/GPU_COMPUTE.md` v2.0 LOCKED, "Architectural integration → Native kernel (K9)" + "Roadmap → K9"
**API contract**: `docs/MOD_OS_ARCHITECTURE.md` v1.6 §4.6 (IModApi v3 — Fields and Compute Pipelines)

## Goal

Native `RawTileField<T>` storage as a parallel abstraction alongside `RawComponentStore`. CPU functional path first; GPU compute (G-series) builds on top of K9 contract без API churn.

## Time estimate

1-2 weeks at hobby pace.

## Deliverables (high-level)

- `RawTileField<T>` C++ class (data + back buffer + conductivity map + storage flags)
- C ABI extensions: `df_world_register_field`, `df_world_field_read_cell`, `df_world_field_write_cell`, `df_world_field_acquire_span`, `df_world_field_set_conductivity`, `df_world_field_set_storage_flag`
- Managed bridge: `FieldRegistry`, `FieldHandle<T>` в `DualFrontier.Core.Interop`
- IModApi v3 implementation: `IModFieldApi` surface (per MOD_OS v1.6 §4.6)
- CPU-side reference diffusion (also serves as G1+ shader equivalence oracle and as CPU fallback per GPU_COMPUTE.md "Failure modes → CPU fallback")
- Selftest extensions: round-trip, span access, mutation, conductivity update, storage flag toggle
- Capability validation: `field.*` verbs cross-checked в loader (per MOD_OS v1.6 §3.7)

## Success criteria

- Any field type registrable / readable / writeable from managed
- CPU diffusion produces correct results on test grids
- Capability checks enforced (mods cannot access fields without declarations)
- No GPU dependency (G-series can take over later без API churn)
- All existing tests passing

## Status: NOT STARTED

Awaiting K6, K7, K8 closure (per β6 sequencing).

## Architecture notes

K9 ships **CPU-functional first** — no GPU dependency. The same C ABI surface that K9 exposes will, in G0, gain optional GPU-backed implementations behind feature flags. Mods using `IModApi.Fields` write code once and run unchanged when G-series lands.

This separation is deliberate per `KERNEL_ARCHITECTURE.md` design:
- К9 proves the field abstraction is sound в isolation от Vulkan compute
- Game design iteration happens on CPU path; G-series adds performance, не functionality
- Capability syntax (MOD_OS v1.6 §3.2) and IModApi v3 (MOD_OS v1.6 §4.6) are stable от К9 onwards
