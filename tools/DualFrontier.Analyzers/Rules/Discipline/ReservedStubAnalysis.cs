using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DualFrontier.Analyzers.Rules.Discipline;

/// <summary>
/// Shared detection helpers for the Lesson #25 / DFL025 family (DFL025_B here at C3,
/// DFL025_A at C4). Attribute identity is matched by canonical FQN display string per
/// Lesson #N19 — the analyzer holds no compile-time reference to DualFrontier.Contracts
/// or xUnit; fixtures declare the FQN-matched types themselves.
/// </summary>
internal static class ReservedStubAnalysis
{
    public const string ReservedStubFqn = "DualFrontier.Contracts.Analyzer.ReservedStubAttribute";
    public const string FactFqn = "Xunit.FactAttribute";
    public const string TheoryFqn = "Xunit.TheoryAttribute";
    public const string TraitFqn = "Xunit.TraitAttribute";

    /// <summary>The <c>[Fact]</c>/<c>[Theory]</c> attribute on the method, or null.</summary>
    public static AttributeData? GetXunitTestAttribute(IMethodSymbol method)
    {
        foreach (AttributeData attribute in method.GetAttributes())
        {
            string? fqn = attribute.AttributeClass?.ToDisplayString();
            if (fqn == FactFqn || fqn == TheoryFqn)
            {
                return attribute;
            }
        }

        return null;
    }

    /// <summary>True if a <c>[Fact]</c>/<c>[Theory]</c> carries a non-empty <c>Skip</c>.</summary>
    public static bool HasSkipArgument(AttributeData testAttribute)
    {
        foreach (System.Collections.Generic.KeyValuePair<string, TypedConstant> named in testAttribute.NamedArguments)
        {
            if (named.Key == "Skip" && named.Value.Value is string skip && !string.IsNullOrEmpty(skip))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>True if <paramref name="type"/> carries <c>[ReservedStub]</c>.</summary>
    public static bool HasReservedStub(ITypeSymbol? type)
    {
        if (type is null)
        {
            return false;
        }

        foreach (AttributeData attribute in type.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == ReservedStubFqn)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// True if the method or its containing type carries
    /// <c>[Trait("Category", "ReservedStub")]</c> (DFL025_A compliance marker).
    /// </summary>
    public static bool HasReservedStubTrait(IMethodSymbol method)
    {
        if (SymbolHasReservedStubTrait(method))
        {
            return true;
        }

        return method.ContainingType is not null && SymbolHasReservedStubTrait(method.ContainingType);
    }

    /// <summary>
    /// True if any symbol referenced in <paramref name="method"/>'s body is owned by
    /// (or is) a <c>[ReservedStub]</c>-tagged type.
    /// </summary>
    public static bool MethodTouchesReservedStub(
        SemanticModel model, MethodDeclarationSyntax method, CancellationToken cancellationToken)
    {
        if (method.Body is null && method.ExpressionBody is null)
        {
            return false;
        }

        foreach (SyntaxNode node in method.DescendantNodes())
        {
            ISymbol? symbol = model.GetSymbolInfo(node, cancellationToken).Symbol;
            if (symbol is null)
            {
                continue;
            }

            ITypeSymbol? owner = symbol switch
            {
                INamedTypeSymbol named => named,
                IMethodSymbol m => m.ContainingType,
                IPropertySymbol p => p.ContainingType,
                IFieldSymbol f => f.ContainingType,
                IEventSymbol e => e.ContainingType,
                _ => null,
            };

            if (HasReservedStub(owner))
            {
                return true;
            }
        }

        return false;
    }

    private static bool SymbolHasReservedStubTrait(ISymbol symbol)
    {
        foreach (AttributeData attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() != TraitFqn)
            {
                continue;
            }

            System.Collections.Immutable.ImmutableArray<TypedConstant> args = attribute.ConstructorArguments;
            if (args.Length == 2
                && args[0].Value is string category
                && args[1].Value is string value
                && category == "Category"
                && value == "ReservedStub")
            {
                return true;
            }
        }

        return false;
    }
}
