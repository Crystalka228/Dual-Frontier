using System;

namespace DualFrontier.Contracts.Attributes;

/// <summary>
/// Marks a system as a temporary bridge implementation that will be replaced
/// by a full implementation in the indicated phase. Bridge systems are stubs
/// whose <c>OnInitialize</c> and <c>Update</c> are no-ops (or a minimal
/// stub-granter); crucially, they do NOT throw <c>NotImplementedException</c>
/// when registered with the scheduler.
///
/// See ROADMAP §"Phase 5 — Magic and Phase 6 — World: bridge between phases".
/// Introduced in TechArch v0.3 §13.2. The <see cref="Phase"/> property is
/// available as a named argument (<c>[BridgeImplementation(Phase = 6)]</c>),
/// which is required for compatibility with the syntax already documented in
/// the ROADMAP and for an analyzer that will later warn about leftover
/// bridges.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class BridgeImplementationAttribute : Attribute
{
    /// <summary>
    /// Phase number in which the bridge must be replaced by a full
    /// implementation (per the phase numbering in <c>docs/ROADMAP.md</c>).
    /// </summary>
    public int Phase { get; set; }

    /// <summary>
    /// When <see langword="true"/>, a mod may list this system's FQN in its
    /// <c>replaces</c> manifest field to supersede this bridge at load time.
    /// When <see langword="false"/> (default), the system is protected and
    /// replacement attempts are rejected with
    /// <c>ValidationErrorKind.ProtectedSystemReplacement</c>.
    /// </summary>
    public bool Replaceable { get; set; }
}
