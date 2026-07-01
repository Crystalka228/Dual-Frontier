using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Architecture.DFK007SpanProtocolAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Architecture;

/// <summary>
/// DFK007 — retaining a SpanLease&lt;T&gt; as field/property storage state escapes the
/// transient per-tick span protocol. Coverage anchor: К-L7 / K_CLOSURE §7.2 (DF007).
/// Transient local use is silent; general Span&lt;&gt; is not flagged.
/// </summary>
public sealed class DFK007SpanProtocolTests
{
    private const string Span = """
        using System;

        namespace DualFrontier.Core.Interop
        {
            public sealed class SpanLease<T> : IDisposable where T : struct
            {
                public SpanLease() { }
                public void Dispose() { }
            }
        }
        """;

    [Fact]
    public async Task DFK007_Fires_On_SpanLease_Field()
    {
        const string test = """


            namespace App
            {
                public struct Comp { }

                public sealed class Holder
                {
                    private DualFrontier.Core.Interop.SpanLease<Comp> {|DFK007:_held|};
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Span + test);
    }

    [Fact]
    public async Task DFK007_Silent_On_SpanLease_Local()
    {
        const string test = """


            namespace App
            {
                public struct Comp { }

                public sealed class User
                {
                    public void Use()
                    {
                        using var lease = new DualFrontier.Core.Interop.SpanLease<Comp>();
                    }
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Span + test);
    }
}
