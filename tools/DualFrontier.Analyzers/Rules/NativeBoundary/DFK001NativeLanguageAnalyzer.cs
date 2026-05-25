using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.NativeBoundary;

/// <summary>
/// DFK001 — К-L1 native language discipline. C++20 dialect on the native side;
/// managed side bridges via canonical P/Invoke surface. No language-mixing
/// patterns (e.g., raw C# Interop binding to non-canonical native symbols).
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
/// ANALYZER_RULES.md §4.1.
/// </para>
/// <para>
/// S-LOCK-2 scope note: managed-side detection only. Native-side C++20 dialect
/// enforced by CMake / clang-tidy / pre-commit hook (outside Roslyn scope per Q-L-5).
/// Managed-side DFK001 detects bridges that bypass canonical P/Invoke surface
/// or mix language dialects through unsanctioned interop.
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L1 row.
/// Canonical detection narrative: K_CLOSURE_REPORT.md §7.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK001NativeLanguageAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK001";

    private static readonly LocalizableString Title =
        "К-L1 native language discipline — managed bridges flow through canonical P/Invoke surface";

    private static readonly LocalizableString MessageFormat =
        "Managed/native boundary violates К-L1 language discipline: {0}";

    private static readonly LocalizableString Description =
        "Per К-L1, native side uses C++20 dialect with canonical P/Invoke binding " +
        "к managed side. Managed code must not bypass canonical surface (e.g., via " +
        "alternate interop libraries, unsanctioned DllImport, or non-canonical native " +
        "symbol bindings). Phase β cleanup-phase will populate detection patterns per " +
        "K_CLOSURE_REPORT.md §7.";

    private const string Category = "DualFrontier.NativeBoundary";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk001");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Phase β cleanup-phase populates detection logic here.
        // Stub returns zero diagnostics при build time.
    }
}
