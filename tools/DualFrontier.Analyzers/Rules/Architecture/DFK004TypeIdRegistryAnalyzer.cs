using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.Architecture;

/// <summary>
/// DFK004 — К-L4 type ID registry invariant. ComponentTypeRegistry is the
/// single source of truth for component type identity mapping (managed ↔ native).
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per the ROADMAP Analyzer track +
/// ANALYZER_RULES.md §4.1.
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L4 row.
/// Canonical detection narrative: K_CLOSURE_REPORT.md §7.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK004TypeIdRegistryAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK004";

    private static readonly LocalizableString Title =
        "К-L4 type ID registry — ComponentTypeRegistry SSoT for component identity";

    private static readonly LocalizableString MessageFormat =
        "Component type identity access bypasses ComponentTypeRegistry: {0}";

    private static readonly LocalizableString Description =
        "Per К-L4, ComponentTypeRegistry is the canonical mapping between managed " +
        "component type and native type ID. Direct numeric literal type IDs, alternate " +
        "registry surfaces, or per-system type ID caching outside the registry violate К-L4. " +
        "Phase β cleanup-phase will populate detection patterns per K_CLOSURE_REPORT.md §7.";

    private const string Category = "DualFrontier.Architecture";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk004");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // К-L4: component type ids are explicit (ComponentTypeRegistry), never derived
        // implicitly. The violation shape is a hash taken over a typeof(...) expression:
        // typeof(X).GetHashCode() / typeof(X).FullName.GetHashCode() / .Name.GetHashCode().
        // Plain typeof(X) (137 legitimate sites) is NOT flagged.
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess
            || memberAccess.Name.Identifier.Text != "GetHashCode")
        {
            return;
        }

        bool derivesFromTypeOf = false;
        foreach (SyntaxNode node in memberAccess.Expression.DescendantNodesAndSelf())
        {
            if (node is TypeOfExpressionSyntax)
            {
                derivesFromTypeOf = true;
                break;
            }
        }

        if (!derivesFromTypeOf)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Rule, invocation.GetLocation(),
            "hash-derived type id (typeof(...).GetHashCode()) bypasses ComponentTypeRegistry"));
    }
}
