using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.Discipline;

/// <summary>
/// DF999 — Analyzer self-policing rule per Q-L-18 default. Bans solution-wide
/// <c>GlobalSuppressions.cs</c> files and assembly-level
/// <c>[assembly: SuppressMessage]</c> attributes — forces per-site suppression
/// discipline (Q-L-20 default (c) hybrid site-scoped first, rule-scoped thereafter).
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
/// ANALYZER_RULES.md §4.5.
/// </para>
/// <para>
/// Q-L-18 default ratification (batch 3 pending — ships at A'.9.1 per default
/// option (a) alongside first DFK### rules per recon Q-K-38).
/// </para>
/// <para>
/// PA-002 anchor («без костылей»): assembly-wide suppressions hide the locus of
/// the violation, making CAPA cascade hard to trace. Per-site suppression
/// requires citation comment (<c>// DFK###-SUPPRESS: &lt;rationale&gt;</c>)
/// preserving locus + audit trail.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DF999GlobalSuppressionBanAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DF999";

    private static readonly LocalizableString Title =
        "DF999 — solution-wide GlobalSuppressions / [assembly: SuppressMessage] banned";

    private static readonly LocalizableString MessageFormat =
        "Solution-wide suppression violates DF999 self-policing discipline: {0}";

    private static readonly LocalizableString Description =
        "Per Q-L-18 + PA-002 axiom, assembly-level suppression attributes " +
        "([assembly: SuppressMessage]) and GlobalSuppressions.cs files hide " +
        "violation locus. Per-site suppression discipline preserves locus + audit " +
        "trail per Q-L-20 hybrid site-scoped suppression CAPA tracking. " +
        "Phase β cleanup-phase will populate detection patterns per K_CLOSURE_REPORT.md §7.";

    private const string Category = "DualFrontier.Discipline";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#df999");

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
