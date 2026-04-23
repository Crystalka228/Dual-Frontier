using System;

namespace DualFrontier.Application.Attributes;

/// <summary>
/// Marks a type or method as belonging exclusively to the Godot DevKit —
/// i.e. editor tooling, debug visualisations, or Godot-specific helpers
/// that must never ship inside the Native runtime. A CI or Roslyn analyser
/// (future work) can walk references and fail the build if any
/// <c>[DevKitOnly]</c> symbol is reached from <c>DualFrontier.Presentation.Native</c>.
/// </summary>
/// <remarks>
/// This attribute is purely a marker. It imposes no runtime behaviour. The
/// <see cref="Reason"/> is intended to be read during code review or by the
/// analyser's diagnostic message.
/// </remarks>
[AttributeUsage(
    AttributeTargets.Class    |
    AttributeTargets.Struct   |
    AttributeTargets.Method   |
    AttributeTargets.Property |
    AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public sealed class DevKitOnlyAttribute : Attribute
{
    /// <summary>Human-readable reason this symbol is DevKit-only.</summary>
    public string Reason { get; }

    public DevKitOnlyAttribute(string reason)
    {
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
    }
}
