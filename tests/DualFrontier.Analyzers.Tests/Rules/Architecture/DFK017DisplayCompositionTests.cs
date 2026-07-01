using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Architecture.DFK017DisplayCompositionAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Architecture;

/// <summary>
/// DFK017 — a class carrying [Layer(...)] that does NOT derive from the sanctioned
/// Layer base is registering as an alternate composition surface. Coverage anchor:
/// К-L17 / K_CLOSURE §7.2 (DF017). Real = 0 (the Layer hierarchy registers via the
/// base, not [Layer]; refined at C9 from the mis-scoped LayerType-member heuristic).
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

        namespace DualFrontier.Application.Display
        {
            using DualFrontier.Contracts.Display;

            public abstract class Layer { public abstract LayerType Type { get; } }
        }
        """;

    [Fact]
    public async Task DFK017_Fires_On_LayerAttribute_Without_Base()
    {
        const string test = """


            namespace App
            {
                using DualFrontier.Contracts.Display;

                [Layer(LayerType.SimState)]
                public sealed class {|DFK017:RogueLayer|} { }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Display + test);
    }

    [Fact]
    public async Task DFK017_Silent_On_Layer_Subclass_Without_Attribute()
    {
        // The real registration mechanism: subclass the Layer base, override Type.
        const string test = """


            namespace App
            {
                using DualFrontier.Contracts.Display;
                using DualFrontier.Application.Display;

                public sealed class GoodLayer : Layer { public override LayerType Type => LayerType.SimState; }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Display + test);
    }

    [Fact]
    public async Task DFK017_Silent_On_LayerAttribute_With_Base()
    {
        const string test = """


            namespace App
            {
                using DualFrontier.Contracts.Display;
                using DualFrontier.Application.Display;

                [Layer(LayerType.SimState)]
                public sealed class GoodLayer : Layer { public override LayerType Type => LayerType.SimState; }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Display + test);
    }
}
