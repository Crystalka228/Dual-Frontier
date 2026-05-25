using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.Architecture;

/// <summary>
/// DFK003.1 — К-L3.1 storage bridge invariant. Managed bridge surface preserves
/// native ownership; bridge facade discipline binding native owned storage to
/// managed read-side surface.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
/// ANALYZER_RULES.md §4.1.
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L3.1 row.
/// Canonical detection narrative: K_CLOSURE_REPORT.md §7.
/// </para>
/// <para>
/// Class name convention: underscore substitution for dotted ID (DFK003.1
/// → DFK003_1...) per Phase β-prep §2 ratification.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK003_1StorageBridgeAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK003.1";

    private static readonly LocalizableString Title =
        "К-L3.1 storage bridge — managed bridge facade preserves native ownership";

    private static readonly LocalizableString MessageFormat =
        "Managed bridge surface violates К-L3.1 storage bridge invariant: {0}";

    private static readonly LocalizableString Description =
        "Storage bridge facade binds native-owned storage к managed read surface " +
        "per К-L3.1. Bridge must not allocate, copy, or mutate native storage outside " +
        "the sanctioned span/buffer protocol. Bridge contract preserved across mod load/unload " +
        "lifecycles. Phase β cleanup-phase will populate detection patterns per K_CLOSURE_REPORT.md §7.";

    private const string Category = "DualFrontier.Architecture";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk003-1");

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
