namespace DualFrontier.Runtime.Window;

/// <summary>
/// Platform-neutral window abstraction: lifecycle, event pump, and Vulkan surface creation.
///
/// <para>The interface carries no platform-tagged handle. A Vulkan surface needs different
/// inputs on every window system — Win32 wants an HINSTANCE and an HWND, XCB wants a
/// connection pointer and a window id — and only the implementation knows its own. Exposing
/// one <c>IntPtr</c> would force every additional backend to widen the contract, so surface
/// creation lives BEHIND the window instead (<see cref="CreateVulkanSurface"/>).</para>
/// </summary>
public interface IWindow : IDisposable
{
    /// <summary>Current client width in pixels. Written by <see cref="PumpMessages"/>.</summary>
    int Width { get; }

    /// <summary>Current client height in pixels. Written by <see cref="PumpMessages"/>.</summary>
    int Height { get; }

    /// <summary>True after successful construction; transitions к false when the window system
    /// reports the window closed (the platform's close/destroy/quit sequence).</summary>
    bool IsOpen { get; }

    void Show();
    void Hide();

    /// <summary>
    /// Drains every pending OS event for this window. Sole writer of <see cref="Width"/>,
    /// <see cref="Height"/> and <see cref="IsOpen"/> during steady state, and the sole producer
    /// of input events into the queue the window was constructed with. Call once per frame from
    /// the thread that created the window.
    /// </summary>
    void PumpMessages();

    /// <summary>
    /// Creates a <c>VkSurfaceKHR</c> for this window against <paramref name="instanceHandle"/>,
    /// using whichever window-system extension the implementation is built on. The caller owns
    /// the returned handle and destroys it with <c>vkDestroySurfaceKHR</c>
    /// (see <c>VulkanSurface</c>).
    /// </summary>
    /// <param name="instanceHandle">A live <c>VkInstance</c>. The instance must have been created
    /// with the surface extension this window's platform requires — see <c>VulkanInstance</c>,
    /// which selects it by the same platform predicate the window factory uses.</param>
    /// <returns>A non-zero <c>VkSurfaceKHR</c> handle.</returns>
    /// <exception cref="InvalidOperationException">Surface creation failed; the message carries
    /// the platform's own diagnostics.</exception>
    IntPtr CreateVulkanSurface(IntPtr instanceHandle);
}
