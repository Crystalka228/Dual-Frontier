using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace DualFrontier.Analyzers.Rules.Architecture;

/// <summary>
/// DFK011 — К-L11 NativeWorld single source of truth. One NativeWorld instance
/// is authoritative for world state per process; managed code observes through
/// the canonical handle, not via shadow world surfaces.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per the ROADMAP Analyzer track +
/// ANALYZER_RULES.md §4.1.
/// </para>
/// <para>
/// Anchor file (Phase β empirical target): <c>src/DualFrontier.Application/Loop/GameBootstrap.cs</c>
/// (L76 NativeWorld instantiation site per Phase 0 closure report §3.10).
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L11 row.
/// Canonical detection narrative: K_CLOSURE_REPORT.md §7.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK011NativeWorldSsotAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK011";

    private static readonly LocalizableString DiagTitle =
        "К-L11 NativeWorld SSoT — single instance authoritative for world state";

    private static readonly LocalizableString MessageFormat =
        "Multiple NativeWorld instantiation or alternate world surface: {0}";

    private static readonly LocalizableString Description =
        "Per К-L11, NativeWorld is instantiated once per process at bootstrap; managed " +
        "consumers observe through the canonical handle. Repeated instantiation, shadow " +
        "world types, or per-system world caches violate К-L11. Phase β cleanup-phase will " +
        "populate detection patterns per K_CLOSURE_REPORT.md §7.";

    private const string Category = "DualFrontier.Architecture";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: DiagTitle,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk011");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // ManagedWorld is the retired managed storage backbone (test-fixture-only per
        // К-L11); NativeWorld is the process SSoT. A `new ManagedWorld(...)` in non-test
        // production code is the shadow-world / dual-backbone reintroduction.
        context.RegisterOperationAction(AnalyzeObjectCreation, OperationKind.ObjectCreation);
    }

    private const string ShadowWorldTypeName = "ManagedWorld";

    private static void AnalyzeObjectCreation(OperationAnalysisContext context)
    {
        var creation = (IObjectCreationOperation)context.Operation;
        if (creation.Type?.Name != ShadowWorldTypeName)
        {
            return;
        }

        string ns = context.ContainingSymbol?.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        if (ns.Contains("Test"))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            creation.Syntax.GetLocation(),
            $"'{ShadowWorldTypeName}' shadow-world reintroduced in production — NativeWorld is the SSoT"));
    }
}
