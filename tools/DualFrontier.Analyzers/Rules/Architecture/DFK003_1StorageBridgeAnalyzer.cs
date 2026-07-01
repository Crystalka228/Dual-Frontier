using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace DualFrontier.Analyzers.Rules.Architecture;

/// <summary>
/// DFK003_1 — К-L3.1 storage bridge invariant. Managed bridge surface preserves
/// native ownership; bridge facade discipline binding native owned storage to
/// managed read-side surface.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per the ROADMAP Analyzer track +
/// ANALYZER_RULES.md §4.1.
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L3.1 row.
/// Canonical detection narrative: K_CLOSURE_REPORT.md §7.
/// </para>
/// <para>
/// Descriptor ID and class name both use the underscore form (DFK003_1). A
/// dotted or hyphenated diagnostic ID is rejected by Roslyn ReportDiagnostic
/// as an invalid identifier — adjudicated 2026-07-01 (ANALYZER_RULES §4.1).
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK003_1StorageBridgeAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK003_1";

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
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk003_1");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Direct `new ManagedStore<T>(...)` is sanctioned only in the per-mod storage
        // provider (RestrictedModApi, DualFrontier.Application.Modding). Anywhere else
        // bypasses the SystemBase.ManagedStore<T>() / resolver API — the К-L3.1 bridge
        // discipline. (Save-path persistence + cross-mod access are documented refinements.)
        context.RegisterOperationAction(AnalyzeObjectCreation, OperationKind.ObjectCreation);
    }

    private const string ManagedStoreFqn = "DualFrontier.Contracts.Modding.ManagedStore<T>";
    private const string SanctionedProviderNamespace = "DualFrontier.Application.Modding";

    private static void AnalyzeObjectCreation(OperationAnalysisContext context)
    {
        var creation = (IObjectCreationOperation)context.Operation;
        if (creation.Type is not INamedTypeSymbol type
            || type.OriginalDefinition.ToDisplayString() != ManagedStoreFqn)
        {
            return;
        }

        string ns = context.ContainingSymbol?.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        if (ns == SanctionedProviderNamespace
            || ns.StartsWith(SanctionedProviderNamespace + ".", StringComparison.Ordinal))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            creation.Syntax.GetLocation(),
            $"direct 'new ManagedStore<T>' bypasses the SystemBase.ManagedStore<T>() API (in '{ns}')"));
    }
}
