using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.NativeBoundary.DFK019_AStaticVulkanApiAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.NativeBoundary;

/// <summary>
/// DFK019_A — a `using` of an alternate graphics-API root (OpenGL/DirectX/Metal/…)
/// on the static surface. Coverage anchor: К-L19 / K_CLOSURE §7.2 (DF019). The
/// sanctioned Vulkan 1.3 surface (Runtime.Native.Vulkan) is SILENT. Real = 0.
/// Severity stays Info (F-12; Phase-γ Warning). Pre-1.3-version usage refinement deferred.
/// </summary>
public sealed class DFK019_AStaticVulkanApiTests
{
    [Fact]
    public async Task DFK019_A_Fires_On_Alternate_Graphics_Using()
    {
        const string source = """
            {|DFK019_A:using OpenGL;|}

            namespace OpenGL { public static class GL { } }

            namespace App { internal static class C { } }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DFK019_A_Silent_On_Vulkan_Using()
    {
        const string source = """
            using DualFrontier.Runtime.Native.Vulkan;

            namespace DualFrontier.Runtime.Native.Vulkan { public static class VkApi { } }

            namespace App { internal static class C { } }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }
}
