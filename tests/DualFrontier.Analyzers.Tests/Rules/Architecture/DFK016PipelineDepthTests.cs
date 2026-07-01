using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Architecture.DFK016PipelineDepthAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Architecture;

/// <summary>
/// DFK016 — a hardcoded pipeline-depth literal 1/2/3 passed to
/// PipelineSlotInterop.Init bypasses the canonical DefaultDepth/MaxDepth constants.
/// Coverage anchor: К-L16 / K_CLOSURE §7.2 (DF016). Real violations = 0 (no literal
/// Init calls on disk). Severity stays Info (retain-α, Q-L-16).
/// </summary>
public sealed class DFK016PipelineDepthTests
{
    [Fact]
    public async Task DFK016_Fires_On_Hardcoded_Depth_Literal()
    {
        const string source = """
            namespace DualFrontier.Core.Interop
            {
                public static class PipelineSlotInterop
                {
                    public const int DefaultDepth = 2;
                    public const int MaxDepth = 3;
                    public static bool Init(int depth = DefaultDepth) => true;
                }
            }

            namespace App
            {
                internal static class Caller
                {
                    public static bool Go() =>
                        DualFrontier.Core.Interop.PipelineSlotInterop.Init({|DFK016:2|});
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DFK016_Silent_On_DefaultDepth_Constant()
    {
        const string source = """
            namespace DualFrontier.Core.Interop
            {
                public static class PipelineSlotInterop
                {
                    public const int DefaultDepth = 2;
                    public const int MaxDepth = 3;
                    public static bool Init(int depth = DefaultDepth) => true;
                }
            }

            namespace App
            {
                internal static class Caller
                {
                    public static bool Go() =>
                        DualFrontier.Core.Interop.PipelineSlotInterop.Init(
                            DualFrontier.Core.Interop.PipelineSlotInterop.DefaultDepth);
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DFK016_Silent_On_Default_Argument()
    {
        const string source = """
            namespace DualFrontier.Core.Interop
            {
                public static class PipelineSlotInterop
                {
                    public const int DefaultDepth = 2;
                    public const int MaxDepth = 3;
                    public static bool Init(int depth = DefaultDepth) => true;
                }
            }

            namespace App
            {
                internal static class Caller
                {
                    public static bool Go() => DualFrontier.Core.Interop.PipelineSlotInterop.Init();
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }
}
