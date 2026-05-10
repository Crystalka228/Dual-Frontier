# G0 — Vulkan compute pipeline plumbing

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/architecture/GPU_COMPUTE.md` v2.0 Roadmap §G0
**Prerequisites**: K9 closed; M9.0–M9.4 closed (Vulkan instance/device live)

## Goal

Establish compute path; no game-visible behavior yet. Native kernel gains linkage to `vulkan-1.dll` for compute dispatch (separate from rendering layer linkage); `VkBuffer`/storage image creation, descriptor sets, pipeline registration C ABI, fence-based sync, build-time SPIR-V compilation extending `RUNTIME_ARCHITECTURE.md` §1.7.

## Time estimate

~1 week

## Deliverables (high-level)

- `VkBuffer` / storage image creation для fields marked compute-managed
- `VkDescriptorSetLayout`, `VkDescriptorPool`, `VkPipelineLayout`
- `df_world_register_compute_pipeline` C ABI
- `df_world_field_dispatch_compute` C ABI
- Fence-based sync between CPU writes (conductivity updates) и GPU dispatch
- Build-time compute shader compilation in mod build process
- Tests: empty dispatch (no-op pipeline) executes без error; pipeline registration round-trip

## Success criteria

- Managed code can register a compute pipeline и trigger empty dispatch
- Framework operational без specific gameplay use yet

## Status: NOT STARTED

Awaiting K9 closed; M9.0–M9.4 closed.

См. также `MOD_OS_ARCHITECTURE.md` v1.6 §4.6 IModApi v3 для compute pipeline registration surface.
