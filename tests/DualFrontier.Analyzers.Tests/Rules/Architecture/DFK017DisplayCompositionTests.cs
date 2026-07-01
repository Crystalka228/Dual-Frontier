using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Architecture.DFK017DisplayCompositionAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Architecture;

/// <summary>
/// DFK017 — a class holding instance LayerType state without [Layer(...)] registration
/// participates in К-L17 composition unregistered. Coverage anchor: К-L17 / K_CLOSURE
/// §7.2 (DF017). Phase-β heuristic; real ≈ 0 (no [Layer] classes on disk yet).
/// </summary>
public sealed class DFK017DisplayCompositionTests
{
    private const string Display = """
        using System;

        namespace DualFrontier.Contracts.Display
        {
            public enum LayerType { SimState, Intent }

            [AttributeUsage(AttributeTargets.Class)]
            public sealed class LayerAttribute : Attribute { public LayerAttribute(LayerType tier) { } }
        }
        """;

    [Fact]
    public async Task DFK017_Fires_On_LayerState_Without_Registration()
    {
        const string test = """


            namespace App
            {
                using DualFrontier.Contracts.Display;

                public sealed class {|DFK017:RogueLayer|} { private LayerType _tier; }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Display + test);
    }

    [Fact]
    public async Task DFK017_Silent_On_Registered_Layer()
    {
        const string test = """


            namespace App
            {
                using DualFrontier.Contracts.Display;

                [Layer(LayerType.SimState)]
                public sealed class GoodLayer { private LayerType _tier; }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Display + test);
    }
}
