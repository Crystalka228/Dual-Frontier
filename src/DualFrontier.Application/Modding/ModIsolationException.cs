using System;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Typed payload of a mod isolation fault. Carried by the surviving fault
/// entry point <see cref="ModLoader.HandleModFault"/>, which reports it to
/// <see cref="ModFaultHandler"/> so the offending mod is queued for deferred
/// unload at the next menu open — the core does not crash and the game
/// continues running (TechArch 11.8; MOD_OS_ARCHITECTURE §10.3).
///
/// History: <c>SystemExecutionContext</c> previously threw this class of
/// exception at runtime for undeclared component access, foreign-bus
/// publication, direct <c>World</c>/<c>ComponentStore</c> reach-in, and
/// <c>GetSystem</c> calls. That runtime guard route was deleted at the
/// К8.3+К8.4 cutover; isolation is now enforced at compile time via
/// <c>[SystemAccess]</c>, and the <see cref="RestrictedModApi"/>
/// cast-prevention rule is a structural barrier (internal sealed type,
/// unresolvable from a mod ALC — MOD_OS_ARCHITECTURE §4.4), not a runtime
/// check.
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
