using DualFrontier.Runtime.Window;

namespace DualFrontier.Runtime;

/// <summary>
/// Composition parameters для <see cref="Runtime.Create"/>. Immutable record passed at
/// Runtime creation; modifying after construction requires recreating Runtime.
/// </summary>
public sealed record RuntimeOptions
{
    public WindowOptions Window { get; init; } = new();

    /// <summary>
    /// When true, Runtime composes a <see cref="Graphics.ValidationLayer"/> consuming Vulkan
    /// validation messages. Defaults to true in DEBUG builds per S-LOCK-4 (validation layer
    /// ALWAYS-ON в DEBUG); false in RELEASE (production binary depends only on vulkan-1.dll).
    /// </summary>
    public bool EnableValidationLayer { get; init; }
#if DEBUG
        = true;
#else
        = false;
#endif

    /// <summary>
    /// Root directory for asset loading via <see cref="Assets.AssetManager"/>. Resolved relative
    /// к executable working directory. Defaults к "assets". V0.C.1+ uses this for PNG sprite
    /// + shader (assets/shaders/*.spv) loading.
    /// </summary>
    public string AssetsDirectory { get; init; } = "assets";
}
