using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.NativeBoundary;

/// <summary>
/// DFK019_A — К-L19 static Vulkan API surface (split per Q-L-8: DFK019_A ships
/// A'.9.1 — static API surface; DFK019.B hardware-tier runtime capability check
/// deferred к hardware tier expansion cascade).
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
/// ANALYZER_RULES.md §4.2.
/// </para>
/// <para>
/// S-LOCK-2 scope note: managed-side detection. Hardware capability runtime
/// probe (Vulkan extension query, GPU memory tier) deferred к DFK019.B
/// (hardware tier expansion cascade — multi-tier audience absent per
/// Lesson #N17 candidate audience-driven deferral).
/// </para>
/// <para>
/// Descriptor ID and class name both use the underscore form (DFK019_A). A
/// dotted or hyphenated diagnostic ID is rejected by Roslyn ReportDiagnostic
/// as an invalid identifier — adjudicated 2026-07-01 (ANALYZER_RULES §4.1).
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L19 row
/// (Vulkan 1.3 + async compute baseline).
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK019_AStaticVulkanApiAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK019_A";

    private static readonly LocalizableString Title =
        "К-L19 static Vulkan API surface — Vulkan 1.3 baseline + async compute";

    private static readonly LocalizableString MessageFormat =
        "Managed code references Vulkan API surface incompatible with К-L19 baseline: {0}";

    private static readonly LocalizableString Description =
        "Per К-L19, GPU baseline = Vulkan 1.3 + async compute (single tier at " +
        "current cascade). Managed code referencing Vulkan API versions older than 1.3, " +
        "vendor-specific extensions outside Vulkan core, или alternate graphics APIs " +
        "violate К-L19. Phase β cleanup-phase will populate detection patterns per " +
        "K_CLOSURE_REPORT.md §7. Hardware-tier runtime capability check deferred к " +
        "DFK019.B (hardware tier expansion cascade).";

    private const string Category = "DualFrontier.NativeBoundary";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk019_a");

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
