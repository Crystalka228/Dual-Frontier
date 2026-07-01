using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Architecture.DFK011NativeWorldSsotAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Architecture;

/// <summary>
/// DFK011 — ManagedWorld (retired managed backbone, test-fixture-only) reintroduced
/// via `new ManagedWorld()` in production. Coverage anchor: К-L11 / K_CLOSURE §7.2
/// (DF011). Real violations = 0 (ManagedWorld retired A'.7, 0 hits on disk).
/// </summary>
public sealed class DFK011NativeWorldSsotTests
{
    private const string Worlds = """
        namespace Worlds
        {
            public sealed class ManagedWorld { }
            public sealed class NativeWorld { }
        }
        """;

    [Fact]
    public async Task DFK011_Fires_On_ManagedWorld_In_Production()
    {
        const string test = """


            namespace DualFrontier.Systems
            {
                internal static class Caller
                {
                    public static object Go() => {|DFK011:new Worlds.ManagedWorld()|};
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Worlds + test);
    }

    [Fact]
    public async Task DFK011_Silent_On_NativeWorld()
    {
        const string test = """


            namespace DualFrontier.Systems
            {
                internal static class Caller
                {
                    public static object Go() => new Worlds.NativeWorld();
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Worlds + test);
    }

    [Fact]
    public async Task DFK011_Silent_On_ManagedWorld_In_Tests()
    {
        // ManagedWorld is sanctioned test-fixture-only; a Tests namespace is exempt.
        const string test = """


            namespace DualFrontier.Core.Tests
            {
                internal static class Caller
                {
                    public static object Go() => new Worlds.ManagedWorld();
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Worlds + test);
    }
}
