using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace DualFrontier.Analyzers.Tests.Verifiers;

/// <summary>
/// Project-local façade over the
/// <c>Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit</c> (1.1.2) harness.
/// Per-rule verifier classes (Phase β) call <see cref="VerifyAnalyzerAsync"/>
/// with inline synthetic source. Per Lesson #N19 the fixtures declare the
/// FQN-matched types themselves (e.g. a local
/// <c>DualFrontier.Contracts.Analyzer.ReservedStubAttribute</c>) rather than
/// referencing DualFrontier assemblies — the analyzer matches display strings,
/// not compile-time CLR identities, so the fixture surface is self-contained.
/// </summary>
/// <remarks>К-L14 thesis: tooling addition; zero substrate touch.</remarks>
public static class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    /// <summary>Expected-diagnostic factory keyed by descriptor id (e.g. "DFK002").</summary>
    public static DiagnosticResult Diagnostic(string diagnosticId)
        => CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(diagnosticId);

    /// <summary>Expected-diagnostic factory keyed by descriptor instance.</summary>
    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
        => CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(descriptor);

    /// <summary>
    /// Compiles <paramref name="source"/> (net8.0 reference assemblies), runs
    /// <typeparamref name="TAnalyzer"/> over it, and asserts exactly
    /// <paramref name="expected"/> diagnostics. Diagnostic spans may be supplied
    /// inline via the harness markup syntax (<c>{|DiagnosticId:code|}</c>).
    /// </summary>
    public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new Test { TestCode = source };
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None);
    }

    public sealed class Test : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
    {
        public Test()
        {
            // Fixtures compile against the pinned net8.0 reference assemblies.
            // src/ moved to net10.0 at STACK_UPDATE; the rule surface only needs
            // [LibraryImport] (net7+), Span<T>, etc. to resolve, which net8.0 refs
            // provide — a ReferenceAssemblies bump rides the F-46 hygiene bundle.
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
        }
    }
}
