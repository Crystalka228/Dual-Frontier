using System;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Exception thrown by <c>SystemExecutionContext</c> on an isolation violation:
/// access to an undeclared component, direct reference to another system, or
/// use of <c>World</c> bypassing the isolation guard. The message contains the
/// violating system's name, the violated type's name, and a hint on how to fix
/// the declaration.
/// </summary>
public sealed class IsolationViolationException : Exception
{
    /// <summary>
    /// Creates the exception with a diagnostic message. The message format is
    /// fixed and asserted by tests — do not change without coordination.
    /// </summary>
    public IsolationViolationException(string message) : base(message)
    {
    }
}
