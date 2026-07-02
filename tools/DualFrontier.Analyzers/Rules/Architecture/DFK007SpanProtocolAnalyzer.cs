using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.Architecture;

/// <summary>
/// DFK007 — К-L7 Span protocol invariant. Storage access between native and
/// managed surfaces flows through <see cref="System.Span{T}"/> / <see cref="System.ReadOnlySpan{T}"/>
/// contracts.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per the ROADMAP Analyzer track +
/// ANALYZER_RULES.md §4.1.
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L7 row.
/// Canonical detection narrative: K_CLOSURE_REPORT.md §7.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK007SpanProtocolAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK007";

    private static readonly LocalizableString Title =
        "К-L7 Span protocol — Span<T>/ReadOnlySpan<T> contract на storage access";

    private static readonly LocalizableString MessageFormat =
        "Storage access violates Span protocol contract: {0}";

    private static readonly LocalizableString Description =
        "Per К-L7, managed access к native storage flows through Span<T>/ReadOnlySpan<T> " +
        "contracts. Storage may not be copied to managed arrays, exposed via IEnumerable, " +
        "or retained across span lifetimes. Detection shipped at A'.9.1 (Phase beta); severity per Release 1.0.";

    private const string Category = "DualFrontier.Architecture";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk007");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // К-L7: a SpanLease<T> is a transient per-tick lease (acquire → read → dispose
        // within the batch). Retaining it as field/property storage state escapes the
        // span protocol. Mutation-through-read-span is compile-prevented (SpanLease
        // exposes ReadOnlySpan<T>); write-outside-batch is a documented refinement.
        // General Span<> use (207 legitimate sites) is NOT flagged.
        context.RegisterSymbolAction(AnalyzeMember, SymbolKind.Field, SymbolKind.Property);
    }

    private const string SpanLeaseFqn = "DualFrontier.Core.Interop.SpanLease<T>";

    private static void AnalyzeMember(SymbolAnalysisContext context)
    {
        // Only a class field/property is long-lived storage. A struct enumerator
        // (readonly struct / ref struct) that holds a lease for the duration of
        // iteration is transient — its lifetime is bounded by the lease — so it is
        // NOT the retained-storage violation.
        if (context.Symbol.ContainingType?.TypeKind != TypeKind.Class)
        {
            return;
        }

        ITypeSymbol? memberType = context.Symbol switch
        {
            IFieldSymbol field => field.Type,
            IPropertySymbol property => property.Type,
            _ => null,
        };

        if (memberType is INamedTypeSymbol named
            && named.OriginalDefinition.ToDisplayString() == SpanLeaseFqn)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Rule, context.Symbol.Locations[0],
                $"'{context.Symbol.Name}' retains a SpanLease<T> as storage state — spans are transient (К-L7)"));
        }
    }
}
