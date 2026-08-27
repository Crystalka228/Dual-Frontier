using System.Reflection;
using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Vulkan;

/// <summary>
/// Resolves the Vulkan loader library name per platform for every <see cref="VkApi"/>
/// P/Invoke in this assembly.
///
/// <para><see cref="VkApi.VulkanLib"/> is the literal <c>"vulkan-1.dll"</c> — the Windows
/// loader trampoline — and it feeds every <c>[LibraryImport]</c> declaration in that file.
/// On Windows the default probing finds it, so nothing here applies. On Linux the host
/// loader is <c>libvulkan.so.1</c>, and .NET's Unix probing sequence for the name
/// <c>vulkan-1.dll</c> tries <c>vulkan-1.dll.so</c>, <c>libvulkan-1.dll.so</c>,
/// <c>vulkan-1.dll</c> and <c>libvulkan-1.dll</c> — none of which is the real soname, so
/// every Vulkan call would throw <see cref="DllNotFoundException"/>. This resolver maps the
/// one name to the platform's real loader instead of renaming the constant, which keeps the
/// Windows arm on untouched default probing.</para>
///
/// <para>Registered from the static constructor of <see cref="VkApi"/>. Declaring that
/// constructor strips the type's <c>beforefieldinit</c> flag, so the CLR must run it before the
/// first access to any <see cref="VkApi"/> member — and every Vulkan P/Invoke in this assembly
/// IS such a member, so the resolver is always installed before the first call that needs it,
/// exactly once per assembly load, with no initialization call asked of any consumer.
/// Returning <see cref="IntPtr.Zero"/> hands the name back to default probing — the resolver
/// claims exactly one library name and nothing else.</para>
/// </summary>
internal static class VulkanLibraryResolver
{
    /// <summary>The Vulkan loader soname on Linux hosts (ELF versioned, no <c>lib</c>-less alias).</summary>
    private const string LinuxVulkanLoader = "libvulkan.so.1";

    internal static void Register()
    {
        NativeLibrary.SetDllImportResolver(typeof(VulkanLibraryResolver).Assembly, Resolve);
    }

    private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (!string.Equals(libraryName, VkApi.VulkanLib, StringComparison.Ordinal))
        {
            // Not ours — default probing decides (this is the only non-Vulkan path).
            return IntPtr.Zero;
        }

        if (OperatingSystem.IsLinux())
        {
            // Throws DllNotFoundException naming libvulkan.so.1 when the loader is genuinely
            // absent, which is the truthful diagnostic; falling through to default probing
            // would report the Windows name instead and hide what the host actually lacks.
            return NativeLibrary.Load(LinuxVulkanLoader, assembly, searchPath);
        }

        return IntPtr.Zero;
    }
}
