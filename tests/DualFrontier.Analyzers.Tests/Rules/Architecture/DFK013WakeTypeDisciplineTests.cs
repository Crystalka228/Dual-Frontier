using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Architecture.DFK013WakeTypeDisciplineAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Architecture;

/// <summary>
/// DFK013 — a concrete SystemBase subclass without [TickRate]/[WakeOn*] activation
/// discipline. Coverage anchor: К-L13 / K_CLOSURE §7.2 (DF013). Real = 0 (codebase
/// thoroughly annotated; doc-comment examples are not symbols and stay silent).
/// </summary>
public sealed class DFK013WakeTypeDisciplineTests
{
    private const string Kernel = """
        using System;

        namespace DualFrontier.Core.ECS { public abstract class SystemBase { } }

        namespace DualFrontier.Contracts.Attributes
        {
            [AttributeUsage(AttributeTargets.Class)]
            public sealed class TickRateAttribute : Attribute { public TickRateAttribute(int ticks) { } }
        }
        """;

    [Fact]
    public async Task DFK013_Fires_On_System_Without_Wake()
    {
        const string test = """


            namespace App
            {
                using DualFrontier.Core.ECS;

                public sealed class {|DFK013:LonelySystem|} : SystemBase { }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Kernel + test);
    }

    [Fact]
    public async Task DFK013_Silent_On_System_With_TickRate()
    {
        const string test = """


            namespace App
            {
                using DualFrontier.Core.ECS;
                using DualFrontier.Contracts.Attributes;

                [TickRate(1)]
                public sealed class GoodSystem : SystemBase { }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Kernel + test);
    }

    [Fact]
    public async Task DFK013_Silent_On_NonSystem()
    {
        const string test = """


            namespace App
            {
                public sealed class Plain { }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Kernel + test);
    }
}
