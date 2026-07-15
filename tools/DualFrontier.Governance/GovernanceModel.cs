namespace DualFrontier.Governance;

/// <summary>
/// Severity of a validation <see cref="Finding"/>, encoding the measure → align →
/// arm deployment discipline (FRAMEWORK 14.8):
/// <list type="bullet">
/// <item><see cref="Error"/> — schema / parse / structural integrity. ALWAYS
///   exit-affecting; nothing downstream is meaningful if these fail.</item>
/// <item><see cref="Report"/> — a semantic gate. Printed but exit-neutral until
///   the gates are armed (<c>--armed</c>); the unaligned corpus is expected to
///   carry these, and the measure report counts them.</item>
/// <item><see cref="Monitor"/> — a never-blocking instrument (G-RATIO).</item>
/// </list>
/// </summary>
public enum Severity
{
    Error,
    Report,
    Monitor,
}

/// <summary>A single validation finding: which gate, how severe, about what, and why.</summary>
public sealed record Finding(string Gate, Severity Severity, string Subject, string Message)
{
    public override string ToString() => $"[{Gate}] {Subject}: {Message}";
}
