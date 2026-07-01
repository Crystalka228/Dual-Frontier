namespace DualFrontier.Analyzers;

/// <summary>
/// Placeholder marker type used by the analyzer test verifier to confirm the
/// DualFrontier.Analyzers assembly loads in the Roslyn host context. Real
/// <c>DiagnosticAnalyzer</c> subclasses (DFK### / DFL### families) are added
/// during Phase β per-rule implementation cascade per the ROADMAP Analyzer track.
/// </summary>
/// <remarks>
/// <para>
/// Per К-extensions cascade #5 (A'.9.1) Phase α Commit 1 — analyzer csproj
/// scaffolding alone produces no diagnostic output. This type exists so the
/// Phase α Commit 2 placeholder test (<c>tests/DualFrontier.Analyzers.Tests/
/// PlaceholderTests.cs</c>) can assert assembly identity via reflection.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition, zero substrate touch.
/// </para>
/// </remarks>
public static class AnalyzerEntryPoint
{
}
