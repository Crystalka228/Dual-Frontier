using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace DualFrontier.Analyzers.Rules.NativeBoundary;

/// <summary>
/// DFK007_1 — К-L7.1 GPU pipeline slot invariant (К-L7 sub-invariant established
/// at К10.3 v2 closure). Managed access к GPU pipeline slot ring goes through
/// <c>PipelineSlotInterop</c> ReadSlotTail API; no direct slot indexing.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per the ROADMAP Analyzer track +
/// ANALYZER_RULES.md §4.2.
/// </para>
/// <para>
/// Anchor file (Phase β empirical target): <c>src/DualFrontier.Core.Interop/PipelineSlotInterop.cs</c>
/// (ReadSlotTail К-L7.1 API per Phase 0 closure report §2.1).
/// </para>
/// <para>
/// Descriptor ID and class name both use the underscore form (DFK007_1). A
/// dotted or hyphenated diagnostic ID is rejected by Roslyn ReportDiagnostic
/// as an invalid identifier — adjudicated 2026-07-01 (ANALYZER_RULES §4.1).
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
    public const string DiagnosticId = "DFK007_1";

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
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk007_1");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // К-L7.1: ReadSlotTail is the sanctioned managed read of the GPU pipeline slot
        // ring. GetSlot returns a raw slot pointer; a managed consumer outside the
        // interop layer calling it is the direct-slot-indexing bypass.
        context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
    }

    private const string PipelineSlotInteropFqn = "DualFrontier.Core.Interop.PipelineSlotInterop";
    private const string SanctionedNamespace = "DualFrontier.Core.Interop";

    private static void AnalyzeInvocation(OperationAnalysisContext context)
    {
        var invocation = (IInvocationOperation)context.Operation;
        IMethodSymbol target = invocation.TargetMethod;
        if (target.Name != "GetSlot"
            || target.ContainingType?.ToDisplayString() != PipelineSlotInteropFqn)
        {
            return;
        }

        string ns = context.ContainingSymbol?.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        if (ns == SanctionedNamespace
            || ns.StartsWith(SanctionedNamespace + ".", StringComparison.Ordinal))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Rule, invocation.Syntax.GetLocation(),
            "direct PipelineSlotInterop.GetSlot bypasses the sanctioned ReadSlotTail read (К-L7.1)"));
    }
}
