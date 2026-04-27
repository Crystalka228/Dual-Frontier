using System;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Thrown when a mod attempts to break isolation — for example, casting
/// <see cref="DualFrontier.Contracts.Modding.IModApi"/> to the concrete
/// <see cref="RestrictedModApi"/> implementation, or reaching into the
/// core's internals bypassing the API.
///
/// This exception is also thrown by <c>SystemExecutionContext</c> when a
/// mod system breaks isolation: accesses an undeclared component, publishes
/// to a foreign bus, reaches into <c>World</c>/<c>ComponentStore</c>
/// directly, or tries to obtain a reference to another system through
/// <c>GetSystem</c>.
///
/// Per TechArch 11.8 such a mod is immediately unloaded by the
/// <c>ModFaultHandler</c> — the core does not crash and the game continues
/// running.
/// </summary>
public sealed class ModIsolationException : Exception
{
    /// <summary>
    /// Creates the exception with no message.
    /// </summary>
    public ModIsolationException()
    {
    }

    /// <summary>
    /// Creates the exception with a diagnostic message.
    /// </summary>
    /// <param name="message">Description of the violation.</param>
    public ModIsolationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates the exception as a wrapper over an inner exception.
    /// </summary>
    /// <param name="message">Description of the violation.</param>
    /// <param name="innerException">Originating exception.</param>
    public ModIsolationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
