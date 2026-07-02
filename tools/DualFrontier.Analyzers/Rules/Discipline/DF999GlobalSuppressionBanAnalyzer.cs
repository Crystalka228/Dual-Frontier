using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

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
/// logic populated at Phase β cleanup-phase per the ROADMAP Analyzer track +
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
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#df999");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // [assembly: SuppressMessage] anywhere, and any GlobalSuppressions.cs file
        // (the conventional assembly-suppression container), violate DF999. Per-site
        // DFK-WAIVER is the sanctioned alternative (CODING_STANDARDS §5.3).
        context.RegisterSyntaxNodeAction(AnalyzeAssemblyAttribute, SyntaxKind.Attribute);
        context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
    }

    private const string SuppressMessageFqn =
        "System.Diagnostics.CodeAnalysis.SuppressMessageAttribute";

    private const string GlobalSuppressionsFileName = "GlobalSuppressions.cs";

    private static void AnalyzeAssemblyAttribute(SyntaxNodeAnalysisContext context)
    {
        var attribute = (AttributeSyntax)context.Node;

        // Only assembly-target attributes: [assembly: ...].
        if (attribute.Parent is not AttributeListSyntax list
            || list.Target is null
            || !list.Target.Identifier.IsKind(SyntaxKind.AssemblyKeyword))
        {
            return;
        }

        // A GlobalSuppressions.cs file is reported once by the tree-level check;
        // avoid double-reporting each assembly attribute inside it.
        if (EndsWithFileName(attribute.SyntaxTree.FilePath, GlobalSuppressionsFileName))
        {
            return;
        }

        ISymbol? constructed =
            context.SemanticModel.GetSymbolInfo(attribute, context.CancellationToken).Symbol;
        if ((constructed as IMethodSymbol)?.ContainingType?.ToDisplayString() != SuppressMessageFqn)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            attribute.GetLocation(),
            "[assembly: SuppressMessage] — use a per-site DFK-WAIVER instead"));
    }

    private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        if (!EndsWithFileName(context.Tree.FilePath, GlobalSuppressionsFileName))
        {
            return;
        }

        Location location = Location.Create(context.Tree, new TextSpan(0, 0));
        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            location,
            "GlobalSuppressions.cs — solution-wide suppression file banned"));
    }

    private static bool EndsWithFileName(string? path, string fileName)
    {
        if (string.IsNullOrEmpty(path) || path!.Length < fileName.Length)
        {
            return false;
        }

        if (!path.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (path.Length == fileName.Length)
        {
            return true;
        }

        char separator = path[path.Length - fileName.Length - 1];
        return separator == '/' || separator == '\\';
    }
}
