using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.Discipline;

/// <summary>
/// DFL025-A — Lesson #25 (refined 3rd extension) ReservedStub behavior invocation
/// discipline. Test classes invoking a <c>[ReservedStub]</c>-tagged type for
/// behavior assertions MUST carry <c>[Trait("Category", "ReservedStub")]</c> at
/// either class or method level — enabling closure-protocol shell-level filter
/// <c>--filter "Category!=ReservedStub"</c> per DFL025-C.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
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
    public const string DiagnosticId = "DFL025-A";

    /// <summary>
    /// Fully-qualified name of the <c>DualFrontier.Contracts.Analyzer.ReservedStubAttribute</c>
    /// type, used for semantic detection per canonical Roslyn pattern (no compile-time
    /// type reference required — analyzer csproj has no ProjectReference к Contracts).
    /// </summary>
    public const string ReservedStubAttributeFqn = "DualFrontier.Contracts.Analyzer.ReservedStubAttribute";

    private static readonly LocalizableString Title =
        "DFL025-A — test classes invoking [ReservedStub]-tagged behavior need [Trait(\"Category\", \"ReservedStub\")]";

    private static readonly LocalizableString MessageFormat =
        "Test class '{0}' invokes [ReservedStub]-tagged type '{1}' without [Trait(\"Category\", \"ReservedStub\")]";

    private static readonly LocalizableString Description =
        "Per Lesson #25 refined 3rd extension, test classes invoking [ReservedStub]-tagged " +
        "types для behavior assertions must carry [Trait(\"Category\", \"ReservedStub\")] " +
        "for closure-protocol filter discipline. Reflection-only access against reserved " +
        "stubs is permitted без trait. Phase β cleanup-phase will populate detection patterns " +
        "per K_CLOSURE_REPORT.md §7.";

    private const string Category = "DualFrontier.Discipline";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfl025-a");

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
