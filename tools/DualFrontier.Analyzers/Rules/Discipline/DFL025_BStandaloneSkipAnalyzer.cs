using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.Discipline;

/// <summary>
/// DFL025_B — Lesson #25 (refined 3rd extension) standalone test Skip discipline.
/// Standalone test methods against reserved-stub modules SHOULD use
/// <c>[Fact(Skip="...")]</c> to avoid false-pass at xUnit collection time
/// (edge case discipline).
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
/// ANALYZER_RULES.md §4.4.
/// </para>
/// <para>
/// Suggestion severity (not Warning) — edge case discipline distinct от
/// DFL025_A behavior invocation discipline. Phase γ promotion target:
/// Suggestion (informational only).
/// </para>
/// <para>
/// Canonical Lesson source: METHODOLOGY.md Lesson #25 refined 3rd extension
/// (FORMALIZED at A'.8 closure 2026-05-23).
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFL025_BStandaloneSkipAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFL025_B";

    private static readonly LocalizableString Title =
        "DFL025_B — standalone test methods against reserved-stub modules should use [Fact(Skip=...)]";

    private static readonly LocalizableString MessageFormat =
        "Standalone test '{0}' against reserved-stub module — consider [Fact(Skip=\"<reason>\")]";

    private static readonly LocalizableString Description =
        "Per Lesson #25 refined 3rd extension edge case discipline, standalone test methods " +
        "against modules tagged with [ReservedStub] should declare [Fact(Skip=\"<reason>\")] " +
        "rather than relying on closure-protocol filter exclusion. Suggestion severity — " +
        "informational only. Phase β cleanup-phase will populate detection patterns per " +
        "K_CLOSURE_REPORT.md §7.";

    private const string Category = "DualFrontier.Discipline";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfl025_b");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // A [Fact]/[Theory] test method (without a Skip) whose body exercises a
        // [ReservedStub]-tagged type should declare [Fact(Skip="...")] rather than
        // rely on the closure-protocol category filter. Reserved-stub identity is
        // shared with DFL025_A via ReservedStubAnalysis (Lesson #N19 FQN matching).
        context.RegisterSyntaxNodeAction(AnalyzeTestMethod, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeTestMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken)
            is not IMethodSymbol method)
        {
            return;
        }

        AttributeData? test = ReservedStubAnalysis.GetXunitTestAttribute(method);
        if (test is null || ReservedStubAnalysis.HasSkipArgument(test))
        {
            return;
        }

        if (!ReservedStubAnalysis.MethodTouchesReservedStub(
                context.SemanticModel, methodDeclaration, context.CancellationToken))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            methodDeclaration.Identifier.GetLocation(),
            method.Name));
    }
}
