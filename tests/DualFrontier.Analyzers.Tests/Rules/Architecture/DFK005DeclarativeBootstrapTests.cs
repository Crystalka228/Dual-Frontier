using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Architecture.DFK005DeclarativeBootstrapAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Architecture;

/// <summary>
/// DFK005 — managed bootstrap composes through a single entry.
/// Coverage anchor: К-L5 / K_CLOSURE §7.2 (DF005). Real violations = 0: the only class in the
/// repository whose name ends in Bootstrap is <c>DualFrontier.Core.Interop.Bootstrap</c>, the
/// sanctioned native-runtime boundary.
///
/// <para>
/// W4 removed the rule's name-based carve-out for a class called <c>GameBootstrap</c>. That was
/// the composition root until the boundary cascade dissolved it, and its successor is
/// <c>EngineComposer</c> — deliberately NOT suffixed, so it needs no exemption. The test below
/// that asserted silence on the name now asserts the opposite, which is the whole behavioural
/// change: a second managed bootstrap is a violation whatever it is called.
/// </para>
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
    public async Task DFK005_Fires_On_TheRetiredCanonicalName()
    {
        // Before W4 this exact source was SILENT, by a name-based exemption for the composition
        // root of the day. The root is gone and the exemption with it, so reusing the name now
        // reads as what it would be: a second managed bootstrap entry.
        const string source = """
            namespace DualFrontier.Application.Loop
            {
                internal static class {|DFK005:GameBootstrap|}
                {
                    public static void Run() { }
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DFK005_Silent_On_TheComposerItself()
    {
        // EngineComposer carries no Bootstrap suffix, which is why К-L5 needs no carve-out for
        // it. This pins that the successor's name is load-bearing rather than incidental.
        const string source = """
            namespace DualFrontier.Application.Loop
            {
                internal static class EngineComposer
                {
                    public static void CreateSession() { }
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
