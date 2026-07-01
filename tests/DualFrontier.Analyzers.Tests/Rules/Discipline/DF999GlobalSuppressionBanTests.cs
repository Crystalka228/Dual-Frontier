using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Discipline.DF999GlobalSuppressionBanAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Discipline;

/// <summary>
/// DF999 — bans solution-wide GlobalSuppressions.cs / [assembly: SuppressMessage].
/// Coverage anchor: ANALYZER_RULES §4.1 (DF999) + CODING_STANDARDS §5.3
/// (GlobalSuppressions ban). Baseline real violations = 0 (recon R3).
/// </summary>
public sealed class DF999GlobalSuppressionBanTests
{
    private const string GlobalSuppressionsFileName = "GlobalSuppressions.cs";

    [Fact]
    public async Task DF999_Fires_On_AssemblySuppressMessage()
    {
        const string source = """
            using System.Diagnostics.CodeAnalysis;

            [assembly: {|DF999:SuppressMessage("Style", "IDE0001")|}]

            namespace N
            {
                public sealed class C { }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DF999_Fires_On_GlobalSuppressionsFile()
    {
        // A file named GlobalSuppressions.cs is banned regardless of content.
        var test = new Verify.Test();
        // A whole-file ban has no code location a #pragma covers; skip the harness's
        // pragma-suppression round-trip for this file-level diagnostic.
        test.TestBehaviors |= TestBehaviors.SkipSuppressionCheck;
        test.TestState.Sources.Add((GlobalSuppressionsFileName,
            "namespace N { public sealed class C { } }"));
        test.ExpectedDiagnostics.Add(
            Verify.Diagnostic("DF999")
                .WithSpan(GlobalSuppressionsFileName, 1, 1, 1, 1)
                .WithArguments("GlobalSuppressions.cs — solution-wide suppression file banned"));
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task DF999_Silent_On_CompliantCode()
    {
        // Ordinary source with no assembly-level suppression: the codebase norm.
        const string source = """
            namespace N
            {
                public sealed class C { }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }
}
