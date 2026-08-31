using System;
using System.IO;
using System.Reflection;
using AwesomeAssertions;
using DualFrontier.Launcher;
using Xunit;

namespace DualFrontier.Runtime.Tests;

/// <summary>
/// W4 — the host's half of assembly resolution after the boundary cut.
///
/// <para>
/// Removing the engine's references to the game's assemblies also removed them from the host's
/// dependency file, and the default load context resolves from that file rather than by scanning
/// its own directory. The files still ship beside the executable; nothing could load them. The
/// live symptom was total: the mod that owns the colony failed to validate and the product would
/// not start.
/// </para>
///
/// <para>
/// The lookup is pure and is tested here. The one line that registers it on the default context
/// is not — installing a process-wide resolver inside a test process would leak into every other
/// test in the assembly — and is covered instead by the live Launcher smoke, which is the only
/// place the whole path exists.
/// </para>
/// </summary>
public sealed class DistributionAssemblyProbeTests : IDisposable
{
    private readonly string _root =
        Path.Combine(Path.GetTempPath(), $"df-probe-{Guid.NewGuid():N}");

    public DistributionAssemblyProbeTests() => Directory.CreateDirectory(_root);

    public void Dispose()
    {
        try { Directory.Delete(_root, recursive: true); } catch { /* best effort */ }
    }

    [Fact]
    public void AnAssemblyPresentInTheDistributionRootIsFound()
    {
        string expected = Path.Combine(_root, "DualFrontier.Components.dll");
        File.WriteAllBytes(expected, Array.Empty<byte>());

        string? found = DistributionAssemblyProbe.PathFor(
            _root, new AssemblyName("DualFrontier.Components"));

        found.Should().Be(expected);
    }

    [Fact]
    public void AnAssemblyTheDistributionDoesNotCarryIsNotFound()
    {
        // Returning null hands the request back to the runtime, whose own error names the
        // assembly. Throwing here would replace a precise message with a vaguer one.
        string? found = DistributionAssemblyProbe.PathFor(
            _root, new AssemblyName("Some.Other.Package"));

        found.Should().BeNull();
    }

    [Theory]
    [InlineData("../DualFrontier.Components")]
    [InlineData("nested/DualFrontier.Components")]
    public void AnAssemblyNameCarryingAPathSeparatorIsRefused(string hostile)
    {
        // A simple assembly name never contains a separator, so one that does is either malformed
        // or an attempt to reach outside the distribution root. Refuse before touching the disk.
        string outside = Path.Combine(_root, "DualFrontier.Components.dll");
        File.WriteAllBytes(outside, Array.Empty<byte>());

        string? found = DistributionAssemblyProbe.PathFor(_root, new AssemblyName(hostile));

        found.Should().BeNull();
    }

    [Fact]
    public void VersionIsNotMatched()
    {
        // Deliberate: everything in a distribution root was built together from one repository,
        // so a version mismatch cannot arise, and checking for one would be theatre that only
        // fires when the check itself is wrong.
        string expected = Path.Combine(_root, "DualFrontier.Systems.dll");
        File.WriteAllBytes(expected, Array.Empty<byte>());

        var requested = new AssemblyName("DualFrontier.Systems") { Version = new Version(9, 9, 9, 9) };

        DistributionAssemblyProbe.PathFor(_root, requested).Should().Be(expected);
    }
}
