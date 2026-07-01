using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Discipline.DFL025_BStandaloneSkipAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Discipline;

/// <summary>
/// DFL025_B — a [Fact]/[Theory] test (without Skip) exercising a [ReservedStub]-tagged
/// type should use [Fact(Skip="...")]. Coverage anchor: Lesson #25 / ANALYZER_RULES
/// §4.1 (DFL025_B) + TESTING_STRATEGY §5.3 (lying-stub law). Real violations = 0
/// (no stub-touching tests exist). Fixtures declare Xunit + ReservedStub locally
/// (Lesson #N19).
/// </summary>
public sealed class DFL025_BStandaloneSkipTests
{
    private const string Preamble = """
        using System;

        namespace Xunit
        {
            [AttributeUsage(AttributeTargets.Method)]
            public sealed class FactAttribute : Attribute
            {
                public string Skip { get; set; }
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
            public static class ReservedModule
            {
                public static void DoWork() { }
            }

            public static class RegularModule
            {
                public static void DoWork() { }
            }
        }
        """;

    [Fact]
    public async Task DFL025B_Fires_On_Standalone_Test_Touching_ReservedStub()
    {
        const string test = """


            namespace FixtureTests
            {
                using Fixture;
                using Xunit;

                public sealed class Tests
                {
                    [Fact]
                    public void {|DFL025_B:Standalone_Exercises_Stub|}()
                    {
                        ReservedModule.DoWork();
                    }
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Preamble + test);
    }

    [Fact]
    public async Task DFL025B_Silent_When_Skip_Present()
    {
        const string test = """


            namespace FixtureTests
            {
                using Fixture;
                using Xunit;

                public sealed class Tests
                {
                    [Fact(Skip = "reserved stub — activates at M-x")]
                    public void Standalone_Exercises_Stub()
                    {
                        ReservedModule.DoWork();
                    }
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Preamble + test);
    }

    [Fact]
    public async Task DFL025B_Silent_When_No_ReservedStub_Touched()
    {
        const string test = """


            namespace FixtureTests
            {
                using Fixture;
                using Xunit;

                public sealed class Tests
                {
                    [Fact]
                    public void Standalone_Exercises_Regular()
                    {
                        RegularModule.DoWork();
                    }
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Preamble + test);
    }
}
