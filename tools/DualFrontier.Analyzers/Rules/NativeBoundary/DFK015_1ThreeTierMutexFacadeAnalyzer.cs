using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace DualFrontier.Analyzers.Rules.NativeBoundary;

/// <summary>
/// DFK015_1 — К-L15.1 three-tier mutex managed facade invariant (К-L15 sub-invariant
/// established at А'.7.x К-extensions cascade #0 closure 2026-05-21). Managed
/// concurrency surface respects the three-tier mutex hierarchy (Layer 1 native
/// compile-time isolation + Layer 2 native runtime + Layer 3 managed facade).
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per the ROADMAP Analyzer track +
/// ANALYZER_RULES.md §4.2.
/// </para>
/// <para>
/// S-LOCK-2 scope note: managed-side facade detection. Layer 1 native
/// compile-time isolation enforced by C++ static_assert / CMake at native side
/// (outside Roslyn scope per Q-L-5). Managed-side DFK015.1 detects facade
/// usage patterns that bypass three-tier discipline.
/// </para>
/// <para>
/// Descriptor ID and class name both use the underscore form (DFK015_1). A
/// dotted or hyphenated diagnostic ID is rejected by Roslyn ReportDiagnostic
/// as an invalid identifier — adjudicated 2026-07-01 (ANALYZER_RULES §4.1).
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
    public const string DiagnosticId = "DFK015_1";

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
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk015_1");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // К-L15.1: where native-owned synchronization applies, managed code uses the
        // three-tier mutex facade — not a raw heavyweight OS primitive. A `new Mutex`
        // / `new Semaphore` is the bypass. ("Native-owned territory" namespace scoping
        // is a documented refinement; the codebase has zero such sites today.)
        context.RegisterOperationAction(AnalyzeObjectCreation, OperationKind.ObjectCreation);
    }

    private static readonly string[] NativeOwnedSyncFqns =
    {
        "System.Threading.Mutex",
        "System.Threading.Semaphore",
    };

    private static void AnalyzeObjectCreation(OperationAnalysisContext context)
    {
        var creation = (IObjectCreationOperation)context.Operation;
        string? typeFqn = creation.Type?.ToDisplayString();

        foreach (string sync in NativeOwnedSyncFqns)
        {
            if (typeFqn == sync)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Rule, creation.Syntax.GetLocation(),
                    $"raw '{typeFqn}' bypasses the three-tier mutex facade (К-L15.1)"));
                return;
            }
        }
    }
}
