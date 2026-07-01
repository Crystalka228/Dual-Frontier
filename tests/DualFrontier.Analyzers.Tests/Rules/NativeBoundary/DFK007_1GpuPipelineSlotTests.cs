using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.NativeBoundary.DFK007_1GpuPipelineSlotAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.NativeBoundary;

/// <summary>
/// DFK007_1 — direct PipelineSlotInterop.GetSlot from a managed consumer outside the
/// interop layer bypasses the sanctioned ReadSlotTail read. Coverage anchor: К-L7.1 /
/// K_CLOSURE §7.2 (DF007.1). Real = 0 (no GetSlot callers on disk).
/// </summary>
public sealed class DFK007_1GpuPipelineSlotTests
{
    private const string Interop = """
        namespace DualFrontier.Core.Interop
        {
            public static class PipelineSlotInterop
            {
                public static int GetSlot(int offset) => 0;
                public static bool ReadSlotTail(int offset) => true;
            }
        }
        """;

    [Fact]
    public async Task DFK007_1_Fires_On_GetSlot_Outside_Interop()
    {
        const string test = """


            namespace DualFrontier.Systems
            {
                internal static class Consumer
                {
                    public static int Read() =>
                        {|DFK007_1:DualFrontier.Core.Interop.PipelineSlotInterop.GetSlot(-1)|};
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Interop + test);
    }

    [Fact]
    public async Task DFK007_1_Silent_On_GetSlot_Inside_Interop()
    {
        const string test = """


            namespace DualFrontier.Core.Interop
            {
                internal static class InteropInternal
                {
                    public static int Read() => PipelineSlotInterop.GetSlot(-1);
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Interop + test);
    }
}
