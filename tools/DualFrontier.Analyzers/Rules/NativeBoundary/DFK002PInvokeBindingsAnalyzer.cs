using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.NativeBoundary;

// §8 federated interop-surface list is shared with DFK001 via SanctionedInteropSurface.

/// <summary>
/// DFK002 — К-L2 P/Invoke bindings invariant. Native interop flows through the
/// canonical P/Invoke surface in <c>DualFrontier.Core.Interop</c>; no ad-hoc
/// <c>[DllImport]</c> declarations outside the sanctioned location.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per the ROADMAP Analyzer track +
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
        "К-L2. Detection shipped at A'.9.1 (Phase beta); severity per Release 1.0.";

    private const string Category = "DualFrontier.NativeBoundary";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk002");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // A P/Invoke method ([DllImport]/[LibraryImport]) is compliant iff its
        // containing namespace is under a sanctioned interop surface (§8 federated
        // model — SanctionedInteropSurface). ManagedBusBridge
        // (DualFrontier.Application.Bus) is the one genuine violation, triaged at C9.
        context.RegisterSymbolAction(AnalyzePInvokeMethod, SymbolKind.Method);
    }

    private const string DllImportFqn = "System.Runtime.InteropServices.DllImportAttribute";
    private const string LibraryImportFqn = "System.Runtime.InteropServices.LibraryImportAttribute";

    private static void AnalyzePInvokeMethod(SymbolAnalysisContext context)
    {
        var method = (IMethodSymbol)context.Symbol;

        bool isPInvoke = false;
        foreach (AttributeData attribute in method.GetAttributes())
        {
            string? fqn = attribute.AttributeClass?.ToDisplayString();
            if (fqn == DllImportFqn || fqn == LibraryImportFqn)
            {
                isPInvoke = true;
                break;
            }
        }

        if (!isPInvoke || SanctionedInteropSurface.IsSanctioned(method.ContainingNamespace))
        {
            return;
        }

        Location location = method.Locations.Length > 0 ? method.Locations[0] : Location.None;
        string ns = method.ContainingNamespace is { IsGlobalNamespace: false } declared
            ? declared.ToDisplayString()
            : "<global>";

        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            location,
            $"'{method.Name}' declares P/Invoke in non-sanctioned namespace '{ns}'"));
    }
}
