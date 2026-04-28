using System;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Kind of validation failure produced by <see cref="ContractValidator"/>.
/// Determines how the UI presents the error and which remediation it suggests.
/// </summary>
public enum ValidationErrorKind
{
    /// <summary>
    /// The mod declares a <c>RequiresContractsVersion</c> that the current
    /// build of <c>DualFrontier.Contracts</c> cannot satisfy.
    /// </summary>
    IncompatibleContractsVersion,

    /// <summary>
    /// Two systems (both from mods, or a mod and core) declare writes to the
    /// same component type, which <see cref="DualFrontier.Core.Scheduling.DependencyGraph"/>
    /// would reject during <c>Build</c>.
    /// </summary>
    WriteWriteConflict,

    /// <summary>
    /// The mod graph contains a dependency cycle that cannot be resolved.
    /// </summary>
    CyclicDependency,

    /// <summary>
    /// The mod requires another mod id that is not present in the load set.
    /// </summary>
    MissingDependency,

    /// <summary>
    /// The mod's <c>apiVersion</c> constraint or an inter-mod
    /// <c>dependencies[i].version</c> constraint is not satisfied by the
    /// available version. Replaces ad-hoc string messages currently used
    /// for <c>apiVersion</c> failures.
    /// </summary>
    IncompatibleVersion,

    /// <summary>
    /// The mod declares a <c>capabilities.required</c> entry that is not
    /// present in the kernel's provided set or in any dependency's
    /// <c>capabilities.provided</c>.
    /// </summary>
    MissingCapability,

    /// <summary>
    /// A mod with <c>kind: "shared"</c> has a non-empty <c>entryAssembly</c>
    /// or <c>entryType</c>, or its assembly contains an <c>IMod</c>
    /// implementation. Shared mods are pure type vendors and must have no
    /// entry point.
    /// </summary>
    SharedModWithEntryPoint,

    /// <summary>
    /// A regular mod's assembly exports a type that implements
    /// <c>IModContract</c> or <c>IEvent</c>. Contract and event types must
    /// live in shared mods only.
    /// </summary>
    ContractTypeInRegularMod,

    /// <summary>
    /// Two or more mods in the same load batch both declare the same
    /// fully-qualified system type in their <c>replaces</c> list. Only one
    /// mod may replace a given system.
    /// </summary>
    BridgeReplacementConflict,

    /// <summary>
    /// A mod's <c>replaces</c> list names a system type that has not been
    /// annotated <c>[BridgeImplementation(Replaceable = true)]</c> — the
    /// system is not marked as replaceable by the engine team.
    /// </summary>
    ProtectedSystemReplacement,

    /// <summary>
    /// A mod's <c>replaces</c> list names a fully-qualified type that cannot
    /// be found in any loaded assembly.
    /// </summary>
    UnknownSystemReplacement,
}

/// <summary>
/// Single validation failure attributed to a specific mod. Immutable — the
/// validator produces a list of these and the pipeline decides how to react.
/// </summary>
/// <param name="ModId">Identifier of the mod the error belongs to.</param>
/// <param name="Kind">Category of the failure.</param>
/// <param name="Message">Human-readable diagnostic for the UI and the log.</param>
/// <param name="ConflictingModId">Partner mod in a conflict, when applicable.</param>
/// <param name="ConflictingComponent">Component type involved in a write-write conflict, when applicable.</param>
public sealed record ValidationError(
    string ModId,
    ValidationErrorKind Kind,
    string Message,
    string? ConflictingModId = null,
    Type? ConflictingComponent = null);

/// <summary>
/// Non-blocking advisory attributed to a specific mod. Warnings do not
/// prevent loading — the UI shows them alongside a successful apply.
/// </summary>
/// <param name="ModId">Identifier of the mod the warning belongs to.</param>
/// <param name="Message">Human-readable advisory text.</param>
public sealed record ValidationWarning(
    string ModId,
    string Message);
