using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.NativeBoundary;

/// <summary>
/// DFK015.1 — К-L15.1 three-tier mutex managed facade invariant (К-L15 sub-invariant
/// established at А'.7.x К-extensions cascade #0 closure 2026-05-21). Managed
/// concurrency surface respects the three-tier mutex hierarchy (Layer 1 native
/// compile-time isolation + Layer 2 native runtime + Layer 3 managed facade).
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
/// ANALYZER_RULES.md §4.2.
/// </para>
/// <para>
/// S-LOCK-2 scope note: managed-side facade detection. Layer 1 native
/// compile-time isolation enforced by C++ static_assert / CMake at native side
/// (outside Roslyn scope per Q-L-5). Managed-side DFK015.1 detects facade
/// usage patterns that bypass three-tier discipline.
/// </para>
/// <para>
/// Class name convention: underscore substitution для dotted ID (DFK015.1
/// → DFK015_1...). Consistent с DFK003_1/007_1/019_A.
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L15.1 row
/// (LOCKED at A'.7.x cascade closure 2026-05-21).
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK015_1ThreeTierMutexFacadeAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK015.1";

    private static readonly LocalizableString Title =
        "К-L15.1 three-tier mutex managed facade — concurrency surface respects layer hierarchy";

    private static readonly LocalizableString MessageFormat =
        "Managed concurrency facade violates К-L15.1 three-tier discipline: {0}";

    private static readonly LocalizableString Description =
        "Per К-L15.1, three-tier mutex hierarchy: Layer 1 native compile-time " +
        "isolation + Layer 2 native runtime + Layer 3 managed facade. Managed code " +
        "must use Layer 3 facade APIs only; raw lock/Monitor против native-owned " +
        "resources или alternate locking surfaces violate К-L15.1. " +
        "Phase β cleanup-phase will populate detection patterns per K_CLOSURE_REPORT.md §7.";

    private const string Category = "DualFrontier.NativeBoundary";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk015-1");

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
