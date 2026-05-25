using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.Architecture;

/// <summary>
/// DFK016 — К-L16 pipeline depth invariant. Pipeline depth D ∈ {1, 2, 3}
/// (default 2) is exposed via <c>PipelineSlotInterop.DefaultDepth</c> and
/// <c>PipelineSlotInterop.MaxDepth</c> constants; hardcoded literals diverge
/// from the canonical surface.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
/// ANALYZER_RULES.md §4.3 (Phase β secondary, retain α per Q-L-16 + Phase 0 §2.1).
/// </para>
/// <para>
/// Anchor file (Phase β empirical target): <c>src/DualFrontier.Core.Interop/PipelineSlotInterop.cs</c>
/// (DefaultDepth=2, MaxDepth=3 constants per Phase 0 closure report §2.1).
/// </para>
/// <para>
/// Severity = Info (Warning post-promotion).
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK016PipelineDepthAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK016";

    private static readonly LocalizableString Title =
        "К-L16 pipeline depth — hardcoded D literal should reference PipelineSlotInterop constants";

    private static readonly LocalizableString MessageFormat =
        "Hardcoded pipeline depth literal '{0}' — reference PipelineSlotInterop.DefaultDepth or .MaxDepth";

    private static readonly LocalizableString Description =
        "Per К-L16, pipeline depth D ∈ {1, 2, 3} is canonical surface exposed via " +
        "PipelineSlotInterop.DefaultDepth (2) and PipelineSlotInterop.MaxDepth (3). " +
        "Managed paths hardcoding literal 1/2/3 для pipeline depth bypass the canonical surface. " +
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
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk016");

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
