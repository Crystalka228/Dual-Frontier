using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Discipline.DFL025_AReservedStubInvocationAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Discipline;

/// <summary>
/// DFL025_A — a test method exercising a [ReservedStub] type without
/// [Trait("Category","ReservedStub")] on the method or class. Coverage anchor:
/// Lesson #25 / ANALYZER_RULES §4.1 (DFL025_A). Real violations = 0.
/// </summary>
public sealed class DFL025_AReservedStubInvocationTests
{
    private const string Preamble = """
        using System;

        namespace Xunit
        {
            [AttributeUsage(AttributeTargets.Method)]
            public sealed class FactAttribute : Attribute { public string Skip { get; set; } }

            [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
            public sealed class TraitAttribute : Attribute
            {
                public TraitAttribute(string name, string value) { }
            }
        }

        namespace DualFrontier.Contracts.Analyzer
        {
            public enum ReservedStubPurpose { BuildComposition }

            [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
            public sealed class ReservedStubAttribute : Attribute
            {
                public ReservedStubAttribute(ReservedStubPurpose purpose, string reason) { }
            }
        }

        namespace Fixture
        {
            using DualFrontier.Contracts.Analyzer;

            [ReservedStub(ReservedStubPurpose.BuildComposition, "reserved for M-x")]
            public static class ReservedModule { public static void DoWork() { } }

            public static class RegularModule { public static void DoWork() { } }
        }
        """;

    [Fact]
    public async Task DFL025A_Fires_When_Touches_Stub_Without_Trait()
    {
        const string test = """


            namespace FixtureTests
            {
                using Fixture;
                using Xunit;

                public sealed class Tests
                {
                    [Fact]
                    public void {|DFL025_A:Exercises_Stub|}() { ReservedModule.DoWork(); }
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Preamble + test);
    }

    [Fact]
    public async Task DFL025A_Silent_When_Class_Has_Trait()
    {
        const string test = """


            namespace FixtureTests
            {
                using Fixture;
                using Xunit;

                [Trait("Category", "ReservedStub")]
                public sealed class Tests
                {
                    [Fact]
                    public void Exercises_Stub() { ReservedModule.DoWork(); }
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Preamble + test);
    }

    [Fact]
    public async Task DFL025A_Silent_When_No_Stub_Touched()
    {
        const string test = """


            namespace FixtureTests
            {
                using Fixture;
                using Xunit;

                public sealed class Tests
                {
                    [Fact]
                    public void Exercises_Regular() { RegularModule.DoWork(); }
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Preamble + test);
    }
}
