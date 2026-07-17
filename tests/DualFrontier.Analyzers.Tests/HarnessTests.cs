using System.Threading.Tasks;
using DualFrontier.Analyzers.Rules.Discipline;
using AwesomeAssertions;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Discipline.DF999GlobalSuppressionBanAnalyzer>;

namespace DualFrontier.Analyzers.Tests;

/// <summary>
/// Harness-level tests for the Phase β analyzer verifier scaffolding. Replaces the
/// Phase α PlaceholderTests assembly-identity check: keeps that assertion and adds
/// a smoke test that exercises the full CSharpAnalyzerVerifier path (compile a
/// fixture, run a DiagnosticAnalyzer, assert diagnostics). Per-rule verifier
/// classes land in Rules/ at C3–C6.
/// </summary>
public sealed class HarnessTests
{
    [Fact]
    public void AnalyzerAssembly_LoadsCleanly()
    {
        typeof(DualFrontier.Analyzers.AnalyzerEntryPoint)
            .Assembly
            .Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Harness_RunsAnalyzer_OnCompliantSource_ReportsNoDiagnostics()
    {
        // Clearly-compliant source (no assembly-level suppression, no
        // GlobalSuppressions): DF999 must stay silent both as a stub and after
        // its C3 detection lands. Proves the scaffolding compiles + executes.
        const string source = "namespace N { public sealed class C { } }";
        await Verify.VerifyAnalyzerAsync(source);
    }
}
