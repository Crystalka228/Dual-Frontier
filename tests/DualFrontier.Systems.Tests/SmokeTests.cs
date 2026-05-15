using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using DualFrontier.Systems.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Systems.Tests;

/// <summary>
/// K8.3+K8.4 cutover smoke coverage. The substantive per-system test suite
/// (autonomous-consume-loop, sleep-loop, needs-accumulation, mood,
/// haul-reservation, cross-system-mutation-isolation) was deleted with the
/// pre-cutover managed-<c>World</c> tests and is being re-authored against
/// <see cref="NativeWorldTestFixture"/> as a follow-up. These smoke tests
/// keep the project compiling and verify the fixture itself wires up
/// correctly — so each follow-up per-system test class can build on a
/// known-good baseline.
/// </summary>
public sealed class SmokeTests
{
    [Fact]
    public void Fixture_BootstrapsCleanly()
    {
        using var fx = new NativeWorldTestFixture();
        fx.NativeWorld.Should().NotBeNull();
        fx.Registry.Should().NotBeNull();
        fx.Services.Should().NotBeNull();
    }

    [Fact]
    public void Fixture_RegistersVanillaComponents_NeedsTypeIdAvailable()
    {
        using var fx = new NativeWorldTestFixture();
        // Round-trip a NeedsComponent through the native API as a smoke
        // proof the registry wired up its type id.
        EntityId entity = fx.NativeWorld.CreateEntity();
        fx.NativeWorld.AddComponent(entity, new NeedsComponent
        {
            Satiety = 0.5f,
            Hydration = 0.5f,
            Sleep = 0.5f,
            Comfort = 0.5f,
        });

        bool found = fx.NativeWorld.TryGetComponent<NeedsComponent>(entity, out NeedsComponent needs);
        found.Should().BeTrue();
        needs.Satiety.Should().Be(0.5f);
    }
}
