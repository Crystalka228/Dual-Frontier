using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
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
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk011");

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
