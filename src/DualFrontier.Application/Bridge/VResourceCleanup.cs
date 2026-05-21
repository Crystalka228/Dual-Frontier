using System;
using System.Collections.Generic;

namespace DualFrontier.Application.Bridge;

/// <summary>
/// К10.3 v2 Item 42 + S-LOCK-12 helpers-only — V (Vulkan) resource cleanup
/// placeholder для mod unload Step 3.6.
///
/// Wraps the future <c>df_vulkan_unload_mod_resources</c> C ABI primitive
/// (VULKAN_SUBSTRATE.md §3.4 К10.3 v2 amendment). К10.3 v2 lands the managed
/// surface + placeholder result returning <c>Result.Success</c> =
/// <see langword="true"/> + zero counts (no pipeline-managed mod resources
/// yet registered). Full
/// implementation (VkDestroyPipeline / VkFreeDescriptorSets / vkDestroyBuffer /
/// vkDestroyImage for mod-registered Vulkan handles) lands в V-cycle work или
/// К-extensions per managed-facade-preserved strategy.
///
/// К-L18 quiescent state precondition satisfied before invocation (Step 3.5
/// К10.2 native primitive already verified sim paused + pipeline quiescent;
/// Step 3.6 inherits the same critical section).
///
/// Per MOD_OS §9.5.1 best-effort sequential semantics: Step 3.6 failures
/// surface as <see cref="Modding.ValidationWarning"/> entries, не halt the
/// unload chain.
/// </summary>
public sealed class VResourceCleanup
{
    /// <summary>
    /// Result of а Step 3.6 V resource cleanup invocation. Mirrors the
    /// shape of future <c>VulkanModUnloadResult</c> C struct, без the
    /// fixed-size error_messages array (managed-side keeps а list).
    /// </summary>
    public sealed class Result
    {
        public bool Success { get; init; }
        public int PipelinesDestroyed { get; init; }
        public int DescriptorSetsDestroyed { get; init; }
        public int BuffersDestroyed { get; init; }
        public int ImagesDestroyed { get; init; }
        public IReadOnlyList<string> ErrorMessages { get; init; } = Array.Empty<string>();
    }

    /// <summary>
    /// К10.3 v2 placeholder. Returns success + zero counts. К10.3 v2 brief
    /// §1.6 + §3 Commit 13 explicitly scopes this as а managed wrapper
    /// placeholder; native <c>df_vulkan_unload_mod_resources</c> implementation
    /// lands V-cycle или К-extensions.
    /// </summary>
    /// <param name="modId">Mod id being unloaded.</param>
    /// <returns>Placeholder result reporting success + zero counts.</returns>
    public Result UnloadModResources(string modId)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));

        // К10.3 v2 placeholder: no pipeline-managed mod resources registered
        // yet, so cleanup is vacuously successful. Future implementation will
        // P/Invoke df_vulkan_unload_mod_resources and translate the
        // VulkanModUnloadResult C struct к this managed Result.
        return new Result
        {
            Success = true,
            PipelinesDestroyed = 0,
            DescriptorSetsDestroyed = 0,
            BuffersDestroyed = 0,
            ImagesDestroyed = 0,
            ErrorMessages = Array.Empty<string>(),
        };
    }
}
