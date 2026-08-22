using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace DualFrontier.Analyzers.Rules.NativeBoundary;

/// <summary>
/// DFK022 — К-L22 entity-identity honesty. Managed code must not construct an
/// <c>EntityId</c> whose <c>Version</c> it did not receive from the world.
/// </summary>
/// <remarks>
/// <para>
/// Detection: an object creation of <c>DualFrontier.Contracts.Core.EntityId</c>
/// whose <c>Version</c> argument is an integer LITERAL — any literal, not just
/// 0 or 1. A literal in that position is a fabrication by construction: the
/// world is the only thing that knows a slot's current generation, so a value
/// the author typed cannot be one the world handed back.
/// </para>
/// <para>
/// Why fabrication is a correctness defect rather than a style issue: native
/// <c>is_alive</c> is exact version equality, so an id with the right index and
/// the wrong version is indistinguishable from a dead one — reads return false,
/// batched writes are dropped at flush, both in silence. A fabricated
/// <c>Version = 0</c> matches only a slot that was never recycled, which
/// collapses generation validation to "this index was never recycled" (the
/// C10 / N-22 defect) and voids the ABA law of
/// IDENTITY_AND_ABI_CONTRACT.md §1 note 1. The honest form reads the version
/// from the world: <c>new EntityId(idx, lease.Versions[idx])</c>.
/// </para>
/// <para>
/// Exemptions, both narrow:
/// </para>
/// <list type="bullet">
///   <item><c>DualFrontier.Core.Interop</c> and namespaces nested under it —
///   the interop layer is where an id is reconstituted from the ABI
///   (<c>EntityIdPacking.Unpack</c>), so it is the one place entitled to
///   assemble a pair from raw halves. In practice it constructs from unpacked
///   values rather than literals, so this exemption is belt-and-braces.</item>
///   <item>Namespaces containing <c>Test</c> — fixtures legitimately name
///   specific (index, version) pairs to assert behaviour at a corner, which is
///   the DFK011 precedent. Structurally the analyzers are wired to
///   <c>src/</c> projects only (<c>src/Directory.Build.props</c>), so this
///   arm rarely fires either; it exists so the rule stays correct if that
///   wiring ever widens.</item>
/// </list>
/// <para>
/// <c>EntityId.Invalid</c> and <c>default(EntityId)</c> stay legal — neither is
/// a constructor invocation, so neither is reachable by this rule. Naming the
/// sentinel is not fabricating a generation.
/// </para>
/// <para>
/// One waived site exists in the tree:
/// <c>src/DualFrontier.Persistence/Compression/EntityEncoder.cs</c>, which
/// decodes persisted index ranges. How versions cross the save boundary is the
/// A7 persistence contract's jurisdiction, so the site carries a
/// <c>DFK-WAIVER(DFK022)</c> naming A7 as its retirement trigger, and the
/// waiver census (<c>CensusMetaTests.DfkWaiverCensus_MatchesPin</c>) pins it.
/// </para>
/// <para>
/// Canonical К-L invariant text: KERNEL_ARCHITECTURE.md Part 0 К-L22 row.
/// Specified at IDENTITY_AND_ABI_CONTRACT.md §2 (consequential amendments);
/// shipped by the ID_B_ENTITY_VERSIONS cascade (F-59).
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DFK022EntityIdentityAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DFK022";

    private static readonly LocalizableString Title =
        "К-L22 entity identity — do not construct an EntityId with a fabricated Version";

    private static readonly LocalizableString MessageFormat =
        "Fabricated entity version: 'new EntityId(..., {0})' invents a generation the world " +
        "did not hand back — read it from the world (e.g. lease.Versions[index])";

    private static readonly LocalizableString Description =
        "Per К-L22, span and batch surfaces carry true entity versions and managed code must " +
        "not construct an EntityId whose Version it did not receive from the world. A literal " +
        "version matches only a slot whose generation happens to equal it, so generation " +
        "validation collapses to index-freshness and stale ids stop being detectable. " +
        "EntityId.Invalid and default(EntityId) remain legal; DualFrontier.Core.Interop " +
        "internals and test namespaces are exempt.";

    private const string Category = "DualFrontier.NativeBoundary";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk022");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterOperationAction(AnalyzeObjectCreation, OperationKind.ObjectCreation);
    }

    private const string EntityIdTypeName = "EntityId";
    private const string VersionParameterName = "Version";
    private const string InteropRoot = "DualFrontier.Core.Interop";

    private static void AnalyzeObjectCreation(OperationAnalysisContext context)
    {
        var creation = (IObjectCreationOperation)context.Operation;
        if (creation.Type?.Name != EntityIdTypeName)
        {
            return;
        }

        string declaringNamespace =
            context.ContainingSymbol?.ContainingNamespace?.ToDisplayString() ?? string.Empty;

        if (declaringNamespace == InteropRoot
            || declaringNamespace.StartsWith(InteropRoot + ".", StringComparison.Ordinal)
            || declaringNamespace.Contains("Test"))
        {
            return;
        }

        foreach (IArgumentOperation argument in creation.Arguments)
        {
            // Positional record-struct parameters carry their declared names, so
            // the Version half is identified by name rather than by ordinal —
            // a `new EntityId(Version: v, Index: i)` call is the same violation.
            if (argument.Parameter?.Name != VersionParameterName)
            {
                continue;
            }

            if (argument.Value is ILiteralOperation literal
                && literal.ConstantValue.HasValue
                && literal.ConstantValue.Value is int version)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Rule,
                    argument.Value.Syntax.GetLocation(),
                    version));
            }
        }
    }
}
