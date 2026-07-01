using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.NativeBoundary.DFK001NativeLanguageAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.NativeBoundary;

/// <summary>
/// DFK001 — managed→native bridge (NativeLibrary.* / Marshal.GetDelegateForFunctionPointer)
/// outside the §8 sanctioned interop surface (managed-side complement of DFK002).
/// Coverage anchor: К-L1 / K_CLOSURE §7.2 (DF001) + PHASE_BETA_BRIEF §8.
/// </summary>
public sealed class DFK001NativeLanguageTests
{
    [Fact]
    public async Task DFK001_Fires_On_Dynamic_Interop_Outside_Sanctioned()
    {
        const string source = """
            using System;
            using System.Runtime.InteropServices;

            namespace DualFrontier.Runtime.Graphics
            {
                internal static class ValidationLayer
                {
                    public delegate void D();
                    public static D Get(IntPtr p) => {|DFK001:Marshal.GetDelegateForFunctionPointer<D>(p)|};
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DFK001_Silent_In_Sanctioned_RuntimeNative()
    {
        const string source = """
            using System;
            using System.Runtime.InteropServices;

            namespace DualFrontier.Runtime.Native.Vulkan
            {
                internal static class VkDelegates
                {
                    public delegate void D();
                    public static D Get(IntPtr p) => Marshal.GetDelegateForFunctionPointer<D>(p);
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }
}
