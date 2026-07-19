using System;
using System.Reflection;
using DualFrontier.Core.Modding;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Capability;

// Top-level public test types — the registry skips non-public and nested
// types by design, so they must be hoisted to namespace scope to be scanned.

public sealed record TestPublishEvent() : IEvent;

public abstract record AbstractTestEvent() : IEvent;

public sealed record GenericTestEvent<T>() : IEvent;

[ModAccessible(Read = true)]
public sealed class ReadableTestComponent : IComponent { }

[ModAccessible(Write = true)]
public sealed class WritableTestComponent : IComponent { }

[ModAccessible(Read = true, Write = true)]
public sealed class ReadWriteTestComponent : IComponent { }

public sealed class UnmarkedTestComponent : IComponent { }

[ModAccessible(Read = false, Write = false)]
public sealed class NotReadOrWriteTestComponent : IComponent { }

/// <summary>
/// Acceptance coverage for <see cref="KernelCapabilityRegistry"/>: event
/// scanning, component scanning under <c>[ModAccessible]</c>, generic and
/// abstract skip rules, and assembly deduplication.
/// </summary>
public sealed class KernelCapabilityRegistryTests
{
    private static readonly Assembly TestAssembly = typeof(TestPublishEvent).Assembly;

    private static KernelCapabilityRegistry BuildFromTestAssembly()
    {
        // W2/BD-10: register the test assembly under the "kernel" owner to exercise the
        // emission logic; the kernel-assembly scan (BuildFromKernelAssemblies) itself is retired.
        var registry = new KernelCapabilityRegistry();
        registry.RegisterOwner("kernel", TestAssembly);
        return registry;
    }

    // --- IEvent scanning ----------------------------------------------------

    [Fact]
    public void ConcreteEvent_ProducesPublishAndSubscribeTokens()
    {
        KernelCapabilityRegistry registry = BuildFromTestAssembly();
        string fqn = typeof(TestPublishEvent).FullName!;

        registry.Provides($"kernel.publish:{fqn}").Should().BeTrue();
        registry.Provides($"kernel.subscribe:{fqn}").Should().BeTrue();
    }

    [Fact]
    public void AbstractEvent_IsSkipped()
    {
        KernelCapabilityRegistry registry = BuildFromTestAssembly();
        string fqn = typeof(AbstractTestEvent).FullName!;

        registry.Provides($"kernel.publish:{fqn}").Should().BeFalse();
        registry.Provides($"kernel.subscribe:{fqn}").Should().BeFalse();
    }

    [Fact]
    public void GenericEvent_IsSkipped()
    {
        KernelCapabilityRegistry registry = BuildFromTestAssembly();
        // Open generic FullName is e.g. "DualFrontier.Modding.Tests.Capability.GenericTestEvent`1".
        // No token should match the publish prefix for that name.
        string genericName = typeof(GenericTestEvent<>).FullName!;
        foreach (string token in registry.Capabilities)
            token.Should().NotContain(genericName);
    }

    // --- IComponent scanning ------------------------------------------------

    [Fact]
    public void ReadableComponent_ProducesReadTokenOnly()
    {
        KernelCapabilityRegistry registry = BuildFromTestAssembly();
        string fqn = typeof(ReadableTestComponent).FullName!;

        registry.Provides($"kernel.read:{fqn}").Should().BeTrue();
        registry.Provides($"kernel.write:{fqn}").Should().BeFalse();
    }

    [Fact]
    public void WritableComponent_ProducesWriteTokenOnly()
    {
        KernelCapabilityRegistry registry = BuildFromTestAssembly();
        string fqn = typeof(WritableTestComponent).FullName!;

        registry.Provides($"kernel.read:{fqn}").Should().BeFalse();
        registry.Provides($"kernel.write:{fqn}").Should().BeTrue();
    }

    [Fact]
    public void ReadWriteComponent_ProducesBothTokens()
    {
        KernelCapabilityRegistry registry = BuildFromTestAssembly();
        string fqn = typeof(ReadWriteTestComponent).FullName!;

        registry.Provides($"kernel.read:{fqn}").Should().BeTrue();
        registry.Provides($"kernel.write:{fqn}").Should().BeTrue();
    }

    [Fact]
    public void UnmarkedComponent_ProducesNeitherToken()
    {
        KernelCapabilityRegistry registry = BuildFromTestAssembly();
        string fqn = typeof(UnmarkedTestComponent).FullName!;

        registry.Provides($"kernel.read:{fqn}").Should().BeFalse();
        registry.Provides($"kernel.write:{fqn}").Should().BeFalse();
    }

    [Fact]
    public void ComponentWithReadFalseWriteFalse_ProducesNeitherToken()
    {
        KernelCapabilityRegistry registry = BuildFromTestAssembly();
        string fqn = typeof(NotReadOrWriteTestComponent).FullName!;

        registry.Provides($"kernel.read:{fqn}").Should().BeFalse();
        registry.Provides($"kernel.write:{fqn}").Should().BeFalse();
    }

    // --- Deduplication ------------------------------------------------------

    [Fact]
    public void DuplicateRegistration_DoesNotDoubleCountTokens()
    {
        KernelCapabilityRegistry single = new();
        single.RegisterOwner("kernel", TestAssembly);

        KernelCapabilityRegistry doubled = new();
        doubled.RegisterOwner("kernel", TestAssembly);
        doubled.RegisterOwner("kernel", TestAssembly);

        doubled.Capabilities.Count.Should().Be(single.Capabilities.Count);
    }

    [Fact]
    public void FreshLedger_IsEmpty()
    {
        // W2/BD-10: a ledger scans nothing at construction — the kernel-assembly scan
        // (BuildFromKernelAssemblies) is retired; the engine owns no gameplay capabilities.
        new KernelCapabilityRegistry().Capabilities.Should().BeEmpty();
    }

    // --- Capabilities property ----------------------------------------------

    [Fact]
    public void Capabilities_IsSameInstanceAcrossCalls()
    {
        KernelCapabilityRegistry registry = BuildFromTestAssembly();
        registry.Capabilities.Should().BeSameAs(registry.Capabilities);
    }

    // --- Owner-namespacing + self-access predicate (W2/BD-10) ---

    [Fact]
    public void RegisterOwner_EmitsOwnerNamespacedTokens_AndRecordsOwnership()
    {
        var ledger = new KernelCapabilityRegistry();
        ledger.RegisterOwner("mod.example", TestAssembly);
        string fqn = typeof(TestPublishEvent).FullName!;

        // Tokens are namespaced to the owner, NOT to kernel.
        ledger.Provides($"mod.example.publish:{fqn}").Should().BeTrue();
        ledger.Provides($"mod.example.subscribe:{fqn}").Should().BeTrue();
        ledger.Provides($"kernel.publish:{fqn}").Should().BeFalse();

        // Ownership is recorded for the self-access predicate; a different owner does not own it.
        ledger.Owns("mod.example", fqn).Should().BeTrue();
        ledger.Owns("mod.other", fqn).Should().BeFalse();
    }
}
