using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Architecture.DFK004TypeIdRegistryAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Architecture;

/// <summary>
/// DFK004 — hash-derived type id (typeof(...).GetHashCode()) bypasses the explicit
/// ComponentTypeRegistry. Coverage anchor: К-L4 / K_CLOSURE §7.2 (DF004). Plain
/// typeof(X) is NOT flagged.
/// </summary>
public sealed class DFK004TypeIdRegistryTests
{
    [Fact]
    public async Task DFK004_Fires_On_TypeOf_GetHashCode()
    {
        const string source = """
            namespace App
            {
                public sealed class Foo { }

                internal static class Caller
                {
                    public static int Id() => {|DFK004:typeof(Foo).GetHashCode()|};
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DFK004_Silent_On_Plain_TypeOf_And_Other_GetHashCode()
    {
        const string source = """
            using System;

            namespace App
            {
                public sealed class Foo { }

                internal static class Caller
                {
                    public static Type What() => typeof(Foo);
                    public static int Other() => "x".GetHashCode();
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }
}
