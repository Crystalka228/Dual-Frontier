using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.NativeBoundary;

/// <summary>
/// DFK007.1 — К-L7.1 GPU pipeline slot invariant (К-L7 sub-invariant established
/// at К10.3 v2 closure). Managed access к GPU pipeline slot ring goes through
/// <c>PipelineSlotInterop</c> ReadSlotTail API; no direct slot indexing.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
/// ANALYZER_RULES.md §4.2.
/// </para>
/// <para>
/// Anchor file (Phase β empirical target): <c>src/DualFrontier.Core.Interop/PipelineSlotInterop.cs</c>
/// (ReadSlotTail К-L7.1 API per Phase 0 closure report §2.1).
/// </para>
/// <para>
/// Class name convention: underscore substitution для dotted ID (DFK007.1
/// → DFK007_1...). Consistent с DFK003_1/015_1/019_A.
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L7.1 row.
/// Canonical detection narrative: K_CLOSURE_REPORT.md §7.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK007_1GpuPipelineSlotAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK007.1";

    private static readonly LocalizableString Title =
        "К-L7.1 GPU pipeline slot — managed access via PipelineSlotInterop.ReadSlotTail";

    private static readonly LocalizableString MessageFormat =
        "GPU pipeline slot access violates К-L7.1: {0}";

    private static readonly LocalizableString Description =
        "Per К-L7.1, managed access к GPU pipeline slot ring flows through " +
        "PipelineSlotInterop.ReadSlotTail API. Direct slot indexing, alternate ring " +
        "buffer surfaces, или raw slot pointer arithmetic violate К-L7.1. " +
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
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk007-1");

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
