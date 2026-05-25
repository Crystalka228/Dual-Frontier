using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.Architecture;

/// <summary>
/// DFK013 — К-L13 on-demand activation discipline (wake type). Each system
/// declares its activation via <c>[WakeOnEvent]</c>, <c>[WakeOnState]</c>,
/// <c>[WakeOnInit]</c>, <c>[WakeOnExplicit]</c>, or <c>[TickRate]</c>; absence
/// defaults to eager Timer tick — an efficiency anti-pattern, not a correctness violation.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
/// ANALYZER_RULES.md §4.3 (Phase β secondary).
/// </para>
/// <para>
/// Anchor file (Phase β empirical target): <c>src/DualFrontier.Contracts/Scheduling/WakeAttributes.cs</c>
/// (4 attribute classes mapping native 5 wake types per Phase 0 closure report §2.2).
/// </para>
/// <para>
/// Severity = Info (Warning post-promotion) — efficiency, not correctness.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK013WakeTypeDisciplineAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK013";

    private static readonly LocalizableString Title =
        "К-L13 wake type discipline — SystemBase missing [WakeOn*]/[TickRate] defaults к eager Timer";

    private static readonly LocalizableString MessageFormat =
        "System '{0}' missing wake discipline: no [WakeOn*]/[TickRate] annotation, or eager Initialize() pattern";

    private static readonly LocalizableString Description =
        "Per К-L13, systems declare activation explicitly via WakeAttributes. " +
        "SystemBase subclasses без [WakeOnEvent]/[WakeOnState]/[WakeOnInit]/[WakeOnExplicit] " +
        "or [TickRate] default к Timer rate 1 (eager every-tick activation). " +
        "Initialize() performing component-iteration outside wake-triggered flow is the same " +
        "anti-pattern. Severity warning post-promotion (К-L13 is efficiency discipline, не correctness). " +
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
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk013");

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
