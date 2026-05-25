using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.Architecture;

/// <summary>
/// DFK017 — К-L17 display composition multi-layer invariant. Display composition
/// flows through declared layer stack; managed renderers respect layer boundaries
/// and composition order.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
/// ANALYZER_RULES.md §4.2.
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L17 row
/// (LOCKED at К10.3 v2 closure 2026-05-20).
/// </para>
/// <para>
/// Canonical detection narrative: K_CLOSURE_REPORT.md §7.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK017DisplayCompositionAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK017";

    private static readonly LocalizableString Title =
        "К-L17 display composition — multi-layer composition discipline";

    private static readonly LocalizableString MessageFormat =
        "Display composition violates К-L17 multi-layer protocol: {0}";

    private static readonly LocalizableString Description =
        "Per К-L17, display composition is a multi-layer stack with declared composition " +
        "order. Managed renderers respect layer boundaries; cross-layer composition, " +
        "out-of-order draws, or alternate composition surfaces violate К-L17. " +
        "Phase β cleanup-phase will populate detection patterns per K_CLOSURE_REPORT.md §7.";

    private const string Category = "DualFrontier.Architecture";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk017");

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
