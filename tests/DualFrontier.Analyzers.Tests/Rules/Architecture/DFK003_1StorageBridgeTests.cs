using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Architecture.DFK003_1StorageBridgeAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Architecture;

/// <summary>
/// DFK003_1 — direct `new ManagedStore&lt;T&gt;(...)` outside the sanctioned per-mod
/// provider (RestrictedModApi, DualFrontier.Application.Modding) bypasses the
/// SystemBase.ManagedStore&lt;T&gt;() / resolver API. Coverage anchor: К-L3.1 /
/// K_CLOSURE §7.2 (DF003.1). Real violations = 0 (only RestrictedModApi:118).
/// </summary>
public sealed class DFK003_1StorageBridgeTests
{
    private const string Store = """
        namespace DualFrontier.Contracts.Modding
        {
            public sealed class ManagedStore<T> where T : class
            {
                public ManagedStore(string modId) { }
            }
        }
        """;

    [Fact]
    public async Task DFK003_1_Fires_On_Direct_Instantiation_Outside_Provider()
    {
        const string test = """


            namespace DualFrontier.Systems
            {
                public sealed class Comp { }

                internal static class Caller
                {
                    public static object Go() =>
                        {|DFK003_1:new DualFrontier.Contracts.Modding.ManagedStore<Comp>("mod")|};
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Store + test);
    }

    [Fact]
    public async Task DFK003_1_Silent_In_Sanctioned_Provider()
    {
        const string test = """


            namespace DualFrontier.Application.Modding
            {
                public sealed class Comp { }

                internal static class RestrictedModApi
                {
                    public static object Go() =>
                        new DualFrontier.Contracts.Modding.ManagedStore<Comp>("mod");
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Store + test);
    }
}
