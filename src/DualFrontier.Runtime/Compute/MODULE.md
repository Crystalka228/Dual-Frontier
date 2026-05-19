# DualFrontier.Runtime / Compute

V0.B compute pipeline plumbing per VULKAN_SUBSTRATE.md §1.1 V0 compute use case +
§3 (compute use case detail).

## Surface

- `VulkanComputePipeline` — VkPipeline с VK_PIPELINE_BIND_POINT_COMPUTE, owns
  the pipeline lifetime (caller passes shader module + descriptor set layouts +
  pipeline layout).
- `VulkanComputeDescriptors` — descriptor set layout + descriptor pool + allocated
  descriptor sets bundle.
- `ComputeDispatch` — wraps command buffer recording + queue submit + fence wait.
  V0.B synchronous (К-L7 atomic-from-observer); V1+ may add async variant.
- `ComputePipelineRegistry` — named pipeline lookup, owns pipelines, disposed
  alongside Runtime facade.

## Out of scope (V0.B)

- Actual K9 field storage binding via descriptor sets — managed-side bridge in
  Commit 15 (FieldStorageBinding). Native-side bridge in Commit 14
  (df_world_register_compute_pipeline + df_world_field_dispatch_compute).
- V1 diffusion / V2 wave compute shaders — separate briefs after V0.B closure.
