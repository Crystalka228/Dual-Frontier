using FluentAssertions;
using Xunit;

namespace DualFrontier.Analyzers.Tests;

/// <summary>
/// Phase α assembly-identity verifier — confirms the DualFrontier.Analyzers
/// assembly loads in the Roslyn analyzer test host context. Replaced by per-rule
/// verifier tests at Phase β when DFK### / DFL### DiagnosticAnalyzer subclasses
/// ship per brief A'.9.1 §6 + §10.
/// </summary>
public sealed class PlaceholderTests
{
    [Fact]
    public void Placeholder_AnalyzerAssembly_LoadsCleanly()
    {
        typeof(DualFrontier.Analyzers.AnalyzerEntryPoint)
            .Assembly
            .Should()
            .NotBeNull();
    }
}
