using System;

namespace DualFrontier.Contracts.Attributes;

/// <summary>
/// Declares the capability tokens a mod-supplied system requires. Placed
/// on <c>DualFrontier.Core.ECS.SystemBase</c> subclasses registered
/// through <c>IModApi.RegisterSystem</c>.
///
/// At load time the validator cross-checks these tokens against the mod's
/// <c>capabilities.required</c> manifest field. A token present here but
/// absent from the manifest is a <c>ValidationErrorKind.MissingCapability</c>
/// error (§3.7–3.8, D-2 LOCKED).
///
/// Example:
/// <code>
/// [ModCapabilities(
///     "kernel.publish:[DualFrontier.Events](http://DualFrontier.Events).Combat.DamageEvent",
///     "[kernel.read](http://kernel.read):DualFrontier.Components.Combat.WeaponComponent")]
/// public sealed class MyCombatSystem : SystemBase { ... }
/// </code>
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModCapabilitiesAttribute : Attribute
{
    /// <summary>
    /// The capability tokens this system requires. Each token must follow
    /// the format
    /// <c>^(kernel|mod\.[a-z0-9.]+)\.(publish|subscribe|read|write):[A-Za-z][A-Za-z0-9_.]+$</c>.
    /// </summary>
    public string[] Tokens { get; }

    /// <summary>
    /// Initialises the attribute with the given capability token strings.
    /// </summary>
    /// <param name="tokens">One or more capability token strings.</param>
    public ModCapabilitiesAttribute(params string[] tokens)
    {
        Tokens = tokens ?? Array.Empty<string>();
    }
}
