using System;

namespace DualFrontier.Contracts.Analyzer;

/// <summary>
/// Marks a type, method, or property as an intentional reserved stub —
/// a placeholder structurally required для build composition or architectural
/// sketching, not a runtime-functional implementation.
/// </summary>
/// <remarks>
/// <para>
/// Per К-extensions cascade #5 (Brief A'.9.1) deliberation Q-L-10: reserved-stub
/// patterns are acceptable for two purposes only:
/// </para>
/// <list type="bullet">
///   <item>
///     <see cref="ReservedStubPurpose.BuildComposition"/> — placeholder needed
///     для assembly composition (e.g., dispatch arm reserved для future cascade
///     activation).
///   </item>
///   <item>
///     <see cref="ReservedStubPurpose.ArchitecturalSketch"/> — structural anchor
///     для forward-design (e.g., interface shape committed before consumer
///     materialization).
///   </item>
/// </list>
/// <para>
/// The <see cref="Reason"/> field is MANDATORY per Q-L-10 + PA-002 axiom («без
/// костылей» / no marker without justification). The constructor throws
/// <see cref="ArgumentException"/> when <c>reason</c> is null or whitespace.
/// The DFL025-A analyzer rule (Phase β implementation) enforces presence at
/// compile time + restricts behavior invocation against tagged members
/// (reflection-only access permitted).
/// </para>
/// <para>
/// К-extensions cascade #3 precedent: deferred dispatch arms (HandlePawnState,
/// HandleItemSpawned, HandleTickAdvanced) marked with this attribute at A'.9.1
/// Phase α Commit 7 (Lesson #N12 sub-pattern B silent stub).
/// </para>
/// </remarks>
/// <seealso cref="ReservedStubPurpose"/>
/// <seealso href="https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#4--a91-active-rules--first-batch-enforcement-surface">DFL025 family detection (ANALYZER_RULES.md §4.4)</seealso>
[AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = false)]
public sealed class ReservedStubAttribute : Attribute
{
    /// <summary>
    /// The architectural purpose justifying the reserved-stub state.
    /// </summary>
    public ReservedStubPurpose Purpose { get; }

    /// <summary>
    /// MANDATORY per Q-L-10 + PA-002 axiom — specific rationale + activation
    /// trigger reference (e.g., К-L20 LOCK cascade, M3.4 milestone).
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// Creates a <see cref="ReservedStubAttribute"/> with the architectural
    /// purpose and mandatory rationale.
    /// </summary>
    /// <param name="purpose">The architectural purpose justifying the stub.</param>
    /// <param name="reason">
    /// Specific rationale + activation trigger reference. Cannot be null or
    /// whitespace per Q-L-10 + PA-002 axiom enforcement.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="reason"/> is null, empty, or whitespace.
    /// </exception>
    public ReservedStubAttribute(ReservedStubPurpose purpose, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException(
                "ReservedStub Reason MANDATORY per Q-L-10 + PA-002 axiom — " +
                "specific rationale + activation trigger reference required.",
                nameof(reason));
        }

        Purpose = purpose;
        Reason = reason;
    }
}
