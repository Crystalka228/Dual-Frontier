using System;
using DualFrontier.Application.Bootstrap;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.Bus;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;

namespace DualFrontier.Systems.Tests.Fixtures;

/// <summary>
/// Shared test fixture for system-level tests post-K8.3+K8.4 cutover.
/// Bootstraps a registry-bound <see cref="NativeWorld"/> through the same
/// helper production uses (<see cref="DualFrontier.Core.Interop.Bootstrap.Run"/>
/// + <see cref="VanillaComponentRegistration.RegisterAll"/>) so every
/// vanilla component type id is registered before any system writes.
///
/// Per brief v2.0 §7.3: single source of truth — the SAME helper production
/// calls. No hand-maintained component-registration list.
/// </summary>
public sealed class NativeWorldTestFixture : IDisposable
{
    public NativeWorld NativeWorld { get; }
    public ComponentTypeRegistry Registry { get; }
    public IGameServices Services { get; }

    public NativeWorldTestFixture()
    {
        NativeWorld = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);
        Registry = NativeWorld.Registry!;
        VanillaComponentRegistration.RegisterAll(Registry);
        Services = new GameServices();
    }

    public void Dispose() => NativeWorld.Dispose();
}
