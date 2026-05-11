using System;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Type-erased contract surface for a registered K9 field. The generic
/// concrete type <c>FieldHandle&lt;T&gt;</c> lives in
/// <c>DualFrontier.Core.Interop</c>; this interface exists so the
/// <c>DualFrontier.Contracts</c> assembly can describe a mod's field
/// handles without referencing Core.Interop (which would invert the
/// dependency direction).
/// </summary>
/// <remarks>
/// Mods that consume <see cref="IModFieldApi.RegisterField{T}"/> or
/// <see cref="IModFieldApi.GetField{T}"/> downcast the returned
/// <see cref="IFieldHandle"/> to <c>FieldHandle&lt;T&gt;</c> at the call
/// site (mods reference Core.Interop transitively through the runtime
/// assembly load context, so the type is available at runtime).
/// </remarks>
public interface IFieldHandle
{
    string Id { get; }
    int Width { get; }
    int Height { get; }
    Type ElementType { get; }
}
