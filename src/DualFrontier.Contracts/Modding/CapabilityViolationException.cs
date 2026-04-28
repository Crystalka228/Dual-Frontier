using System;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Thrown when a mod attempts to publish or subscribe to an event type
/// without declaring the corresponding capability in its manifest.
/// </summary>
public sealed class CapabilityViolationException : Exception
{
    /// <summary>Creates an instance with no message.</summary>
    public CapabilityViolationException()
    {
    }

    /// <summary>Creates an instance with the given message.</summary>
    public CapabilityViolationException(string message)
        : base(message)
    {
    }

    /// <summary>Creates an instance with the given message and inner exception.</summary>
    public CapabilityViolationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
