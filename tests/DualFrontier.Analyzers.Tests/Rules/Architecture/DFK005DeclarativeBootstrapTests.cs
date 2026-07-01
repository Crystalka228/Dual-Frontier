using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Architecture.DFK005DeclarativeBootstrapAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Architecture;

/// <summary>
/// DFK005 — managed bootstrap composes through the single canonical GameBootstrap.
/// Coverage anchor: К-L5 / K_CLOSURE §7.2 (DF005). Real violations = 0 (only
/// GameBootstrap + Core.Interop.Bootstrap exist, both sanctioned).
/// </summary>
public sealed class DFK005DeclarativeBootstrapTests
{
    [Fact]
    public async Task DFK005_Fires_On_Additional_Managed_Bootstrap()
    {
        const string source = """
            namespace DualFrontier.Systems
            {
                internal static class {|DFK005:ModBootstrap|}
                {
                    public static void Run() { }
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DFK005_Silent_On_Canonical_GameBootstrap()
    {
        const string source = """
            namespace DualFrontier.Application.Loop
            {
                internal static class GameBootstrap
                {
                    public static void Run() { }
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DFK005_Silent_On_CoreInterop_Bootstrap()
    {
        // Core.Interop hosts the sanctioned native-runtime bootstrap boundary.
        const string source = """
            namespace DualFrontier.Core.Interop
            {
                public static class Bootstrap
                {
                    public static void Run() { }
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }
}
