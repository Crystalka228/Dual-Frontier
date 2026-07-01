using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.NativeBoundary.DFK002PInvokeBindingsAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.NativeBoundary;

/// <summary>
/// DFK002 — P/Invoke ([DllImport]/[LibraryImport]) is compliant only inside the §8
/// federated sanctioned surface (Core.Interop + Runtime.Native.*). Coverage anchor:
/// К-L2 / K_CLOSURE §7.2 (DF002) + PHASE_BETA_BRIEF §8. The ManagedBusBridge cluster
/// (DualFrontier.Application.Bus) is the intended positive.
/// </summary>
public sealed class DFK002PInvokeBindingsTests
{
    [Fact]
    public async Task DFK002_Fires_On_DllImport_In_NonSanctioned_Namespace()
    {
        const string source = """
            using System.Runtime.InteropServices;

            namespace DualFrontier.Application.Bus
            {
                internal static class ManagedBusBridge
                {
                    [DllImport("df_native")]
                    internal static extern void {|DFK002:df_bus_publish|}();
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DFK002_Silent_On_DllImport_In_CoreInterop()
    {
        const string source = """
            using System.Runtime.InteropServices;

            namespace DualFrontier.Core.Interop
            {
                internal static class NativeMethods
                {
                    [DllImport("df_native")]
                    internal static extern int df_pipeline_init(int depth);
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DFK002_Silent_On_DllImport_In_RuntimeNative()
    {
        // Runtime.Native.Vulkan is the sanctioned GPU substrate boundary (К-L19).
        const string source = """
            using System.Runtime.InteropServices;

            namespace DualFrontier.Runtime.Native.Vulkan
            {
                internal static class VkApi
                {
                    [DllImport("vulkan-1")]
                    internal static extern int vkCreateInstance();
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }
}
