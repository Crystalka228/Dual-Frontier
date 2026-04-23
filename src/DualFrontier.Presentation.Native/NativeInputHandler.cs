using System;
using DualFrontier.Application.Input;

namespace DualFrontier.Presentation.Native;

/// <summary>
/// Native <see cref="IInputSource"/> stub. Full implementation polls
/// Silk.NET input device events and publishes to the domain buses via
/// the <c>IGameServices</c> routing. Lands in Phase 5+.
/// </summary>
public sealed class NativeInputHandler : IInputSource
{
    /// <inheritdoc />
    public void Poll()
        => throw new NotImplementedException("Phase 5+: Silk.NET input polling.");
}
