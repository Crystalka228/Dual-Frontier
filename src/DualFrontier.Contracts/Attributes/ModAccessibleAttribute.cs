using System;

namespace DualFrontier.Contracts.Attributes;

/// <summary>
/// Marks an <see cref="DualFrontier.Contracts.Core.IComponent"/> as
/// accessible to mods through the capability model. Only components
/// annotated with this attribute are visible to the capability resolver;
/// all others are invisible to mods.
///
/// A mod requests access by declaring
/// <c>[kernel.read](http://kernel.read):{FQN}</c> or <c>kernel.write:{FQN}</c> in its manifest's
/// <c>capabilities.required</c> list (§3, D-1 LOCKED).
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModAccessibleAttribute : Attribute
{
    /// <summary>
    /// When <see langword="true"/>, mods may read this component via
    /// <c>[kernel.read](http://kernel.read):{FQN}</c> capability.
    /// </summary>
    public bool Read { get; set; }

    /// <summary>
    /// When <see langword="true"/>, mods may write this component via
    /// <c>kernel.write:{FQN}</c> capability.
    /// </summary>
    public bool Write { get; set; }
}
