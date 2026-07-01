using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.Architecture;

/// <summary>
/// DFK003 — К-L3 storage ownership invariant. Native side owns ECS storage;
/// managed accesses occur via stable spans per К-L7 protocol.
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time
/// wiring + violation count enumeration (Q-L-1 adaptive gate). Detection
/// logic populated at Phase β cleanup-phase per the ROADMAP Analyzer track +
/// ANALYZER_RULES.md §4.1.
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L3 row.
/// Canonical detection narrative: K_CLOSURE_REPORT.md §7.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK003StorageOwnershipAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK003";

    private static readonly LocalizableString Title =
        "К-L3 storage ownership — native owns ECS storage; managed accesses via stable spans";

    private static readonly LocalizableString MessageFormat =
        "Managed code violates К-L3 storage ownership invariant: {0}";

    private static readonly LocalizableString Description =
        "Native side owns ECS storage allocation, layout, and lifetime per К-L3. " +
        "Managed code may only access storage через stable spans exposed by Span protocol (К-L7). " +
        "Direct managed allocation of ECS-shaped storage or unsanctioned ownership transfer violates К-L3. " +
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
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk003");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // К-L3 Path α/β: a struct IComponent is Path α (native storage, no
        // [ManagedStorage]); a class IComponent is Path β (managed store, requires
        // [ManagedStorage]). The two mismatches are the storage-ownership violations.
        // Compile-time isolation (A'.6) already keeps this small.
        context.RegisterSymbolAction(AnalyzeComponentType, SymbolKind.NamedType);
    }

    private const string IComponentFqn = "DualFrontier.Contracts.Core.IComponent";
    private const string ManagedStorageFqn = "DualFrontier.Contracts.Modding.ManagedStorageAttribute";

    private static void AnalyzeComponentType(SymbolAnalysisContext context)
    {
        var type = (INamedTypeSymbol)context.Symbol;
        if (type.TypeKind != TypeKind.Class)
        {
            return;
        }

        bool isComponent = false;
        foreach (INamedTypeSymbol iface in type.AllInterfaces)
        {
            if (iface.ToDisplayString() == IComponentFqn)
            {
                isComponent = true;
                break;
            }
        }

        if (!isComponent)
        {
            return;
        }

        bool hasManagedStorage = false;
        foreach (AttributeData attribute in type.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == ManagedStorageFqn)
            {
                hasManagedStorage = true;
                break;
            }
        }

        // A struct IComponent is Path α by construction: [ManagedStorage] is
        // AttributeUsage(Class)-only, so a struct cannot carry it (compile-prevented,
        // CS0592). The reachable violation is a class IComponent missing [ManagedStorage].
        if (type.TypeKind == TypeKind.Class && !hasManagedStorage)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Rule, type.Locations[0],
                $"class component '{type.Name}' must carry [ManagedStorage] (Path β = managed store)"));
        }
    }
}
