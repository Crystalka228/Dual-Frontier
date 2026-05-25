using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.NativeBoundary;

/// <summary>
/// DFK002 — К-L2 P/Invoke bindings invariant. Native interop flows through the
/// canonical P/Invoke surface in <c>DualFrontier.Core.Interop</c>; no ad-hoc
/// <c>[DllImport]</c> declarations outside the sanctioned location.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per Brief A'.9.1 §10.5 +
/// ANALYZER_RULES.md §4.1.
/// </para>
/// <para>
/// Anchor file (Phase β empirical target): <c>src/DualFrontier.Core.Interop/*.cs</c>
/// (P/Invoke surface).
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L2 row.
/// Canonical detection narrative: K_CLOSURE_REPORT.md §7.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK002PInvokeBindingsAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK002";

    private static readonly LocalizableString Title =
        "К-L2 P/Invoke bindings — interop через canonical Core.Interop surface only";

    private static readonly LocalizableString MessageFormat =
        "Ad-hoc [DllImport] or non-canonical interop outside DualFrontier.Core.Interop: {0}";

    private static readonly LocalizableString Description =
        "Per К-L2, native interop is centralized в DualFrontier.Core.Interop; ad-hoc " +
        "[DllImport] declarations или [LibraryImport] sources outside this assembly violate " +
        "К-L2. Phase β cleanup-phase will populate detection patterns per K_CLOSURE_REPORT.md §7.";

    private const string Category = "DualFrontier.NativeBoundary";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk002");

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
