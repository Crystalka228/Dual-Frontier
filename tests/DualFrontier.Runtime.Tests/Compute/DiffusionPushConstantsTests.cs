using System.Runtime.InteropServices;
using DualFrontier.Runtime.Compute;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Compute;

/// <summary>
/// S-LOCK-7 alignment audit gate per Lesson #7 strengthening discipline.
/// V0.A surfaced 1 correction, V0.B 5 corrections, V0.C.1 0 corrections,
/// V0.C.2 1 correction (enum sizeof в .NET 8) — V1 push constant struct
/// continues the audit pattern.
/// </summary>
public sealed class DiffusionPushConstantsTests
{
    [Fact]
    public void Size_matches_expected_16_bytes()
    {
        Marshal.SizeOf<DiffusionPushConstants>().Should().Be(16);
    }

    [Fact]
    public void Fields_accessible_and_assignable()
    {
        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = 0.01f,
            DeltaTime = 1.0f,
            Width = 200,
            Height = 200,
        };

        pc.DecayCoefficient.Should().Be(0.01f);
        pc.DeltaTime.Should().Be(1.0f);
        pc.Width.Should().Be(200u);
        pc.Height.Should().Be(200u);
    }
}
