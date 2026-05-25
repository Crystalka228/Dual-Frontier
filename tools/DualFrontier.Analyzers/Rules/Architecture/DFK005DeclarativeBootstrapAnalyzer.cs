using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.Architecture;

/// <summary>
/// DFK005 — К-L5 declarative bootstrap invariant. Bootstrap composition derives
/// from <c>bootstrap_graph.h</c> declarative source; managed bootstrap is a
/// consumer of native declaration, not a parallel composition surface.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
/// ANALYZER_RULES.md §4.1.
/// </para>
/// <para>
/// Anchor file (Phase β empirical target): <c>src/DualFrontier.Core.Interop/Bootstrap.cs</c>.
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L5 row.
/// Canonical detection narrative: K_CLOSURE_REPORT.md §7.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK005DeclarativeBootstrapAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK005";

    private static readonly LocalizableString Title =
        "К-L5 declarative bootstrap — bootstrap_graph.h single composition source";

    private static readonly LocalizableString MessageFormat =
        "Managed bootstrap diverges from declarative bootstrap_graph: {0}";

    private static readonly LocalizableString Description =
        "Per К-L5, bootstrap composition is declared в <c>bootstrap_graph.h</c> and " +
        "managed consumers (<c>Bootstrap.cs</c>) bind к the declaration. Multiple bootstrap " +
        "entry points, managed-only system composition, or imperative bootstrap construction " +
        "outside the declared graph violate К-L5. Phase β cleanup-phase will populate detection " +
        "patterns per K_CLOSURE_REPORT.md §7.";

    private const string Category = "DualFrontier.Architecture";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk005");

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
