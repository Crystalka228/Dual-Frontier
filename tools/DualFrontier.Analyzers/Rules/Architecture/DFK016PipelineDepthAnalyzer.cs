using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

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
/// logic populated at Phase β cleanup-phase per the ROADMAP Analyzer track +
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
        "Detection shipped at A'.9.1 (Phase beta); severity per Release 1.0.";

    private const string Category = "DualFrontier.Architecture";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk016");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // К-L16: pipeline depth D ∈ {1,2,3} is canonical surface via
        // PipelineSlotInterop.DefaultDepth (2) / .MaxDepth (3). A hardcoded integer
        // literal 1/2/3 passed as the depth argument bypasses that surface. A named
        // constant reference (DefaultDepth) is a field reference, not a literal, so
        // the compliant call stays silent.
        context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
    }

    private const string PipelineSlotInteropFqn = "DualFrontier.Core.Interop.PipelineSlotInterop";
    private const string DepthMethodName = "Init";
    private const string DepthParameterName = "depth";

    private static void AnalyzeInvocation(OperationAnalysisContext context)
    {
        var invocation = (IInvocationOperation)context.Operation;
        IMethodSymbol target = invocation.TargetMethod;

        if (target.Name != DepthMethodName
            || target.ContainingType?.ToDisplayString() != PipelineSlotInteropFqn)
        {
            return;
        }

        foreach (IArgumentOperation argument in invocation.Arguments)
        {
            // Only explicitly-passed arguments — an omitted optional parameter's
            // default value surfaces as a synthesized literal in the operation tree
            // and must NOT be flagged (that is the compliant Init() default path).
            if (argument.Parameter?.Name != DepthParameterName
                || argument.ArgumentKind != ArgumentKind.Explicit)
            {
                continue;
            }

            // A literal 1/2/3 (not a named-constant field reference) is the violation.
            if (argument.Value is ILiteralOperation literal
                && literal.ConstantValue.HasValue
                && literal.ConstantValue.Value is int depth
                && depth is 1 or 2 or 3)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Rule,
                    argument.Value.Syntax.GetLocation(),
                    depth));
            }
        }
    }
}
