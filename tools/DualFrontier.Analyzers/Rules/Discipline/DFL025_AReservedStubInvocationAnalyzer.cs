using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.Discipline;

/// <summary>
/// DFL025_A — Lesson #25 (refined 3rd extension) ReservedStub behavior invocation
/// discipline. Test classes invoking a <c>[ReservedStub]</c>-tagged type for
/// behavior assertions MUST carry <c>[Trait("Category", "ReservedStub")]</c> at
/// either class or method level — enabling closure-protocol shell-level filter
/// <c>--filter "Category!=ReservedStub"</c> per DFL025-C.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per the ROADMAP Analyzer track +
/// ANALYZER_RULES.md §4.4.
/// </para>
/// <para>
/// Phase β detection mechanism (forward reference):
/// SemanticModel + AttributeData check via fully-qualified name string match
/// against <c>"DualFrontier.Contracts.Analyzer.ReservedStubAttribute"</c>.
/// This is the canonical Roslyn pattern — analyzers run в compiler host and
/// access the analyzed Compilation through Microsoft.CodeAnalysis APIs, не
/// through compile-time CLR type reference (which would require ProjectReference
/// dropped per analyzer csproj rationale comment).
/// </para>
/// <para>
/// Reflection-only access against <c>[ReservedStub]</c>-tagged members is
/// permitted; behavior-invoking calls require the <c>[Trait]</c> annotation.
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
public sealed class DFL025_AReservedStubInvocationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFL025_A";

    /// <summary>
    /// Fully-qualified name of the <c>DualFrontier.Contracts.Analyzer.ReservedStubAttribute</c>
    /// type, used for semantic detection per canonical Roslyn pattern (no compile-time
    /// type reference required — analyzer csproj has no ProjectReference к Contracts).
    /// </summary>
    public const string ReservedStubAttributeFqn = "DualFrontier.Contracts.Analyzer.ReservedStubAttribute";

    private static readonly LocalizableString Title =
        "DFL025_A — test classes invoking [ReservedStub]-tagged behavior need [Trait(\"Category\", \"ReservedStub\")]";

    private static readonly LocalizableString MessageFormat =
        "Test class '{0}' invokes [ReservedStub]-tagged type '{1}' without [Trait(\"Category\", \"ReservedStub\")]";

    private static readonly LocalizableString Description =
        "Per Lesson #25 refined 3rd extension, test classes invoking [ReservedStub]-tagged " +
        "types для behavior assertions must carry [Trait(\"Category\", \"ReservedStub\")] " +
        "for closure-protocol filter discipline. Reflection-only access against reserved " +
        "stubs is permitted без trait. Detection shipped at A'.9.1 (Phase beta); severity per Release 1.0.";

    private const string Category = "DualFrontier.Discipline";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfl025_a");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // A [Fact]/[Theory] test method that exercises a [ReservedStub]-tagged type,
        // without [Trait("Category","ReservedStub")] on the method or its class, breaks
        // the closure-protocol filter discipline. Reserved-stub + trait identity is
        // resolved by ReservedStubAnalysis (FQN matching, Lesson #N19).
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

        if (ReservedStubAnalysis.GetXunitTestAttribute(method) is null
            || ReservedStubAnalysis.HasReservedStubTrait(method))
        {
            return;
        }

        ITypeSymbol? stub = ReservedStubAnalysis.FirstTouchedReservedStub(
            context.SemanticModel, methodDeclaration, context.CancellationToken);
        if (stub is null)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            methodDeclaration.Identifier.GetLocation(),
            method.ContainingType?.Name ?? method.Name,
            stub.Name));
    }
}
