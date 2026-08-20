using System.Reflection;
using DualFrontier.Core.Modding;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Capability;

/// <summary>
/// W3/D3 — the ledger's removal surface and its registration symmetry. These are the unit-level
/// proofs behind the pipeline wiring: <c>RegisterOwner</c> is idempotent per owner,
/// <c>RemoveOwner</c> subtracts EXACTLY one owner's tokens, and the nesting hazard that makes a
/// prefix sweep unsafe is pinned by a test rather than only by a comment.
/// </summary>
public sealed class OwnerLedgerLifecycleTests
{
    private static readonly Assembly TestAssembly = typeof(TestPublishEvent).Assembly;
    private static readonly string EventFqn = typeof(TestPublishEvent).FullName!;

    [Fact]
    public void RemoveOwner_DropsEveryTokenAndOwnershipRecordForThatOwner()
    {
        var registry = new KernelCapabilityRegistry();
        registry.RegisterOwner("mod.weather", TestAssembly);

        registry.Provides($"mod.weather.publish:{EventFqn}").Should().BeTrue();
        registry.Owns("mod.weather", EventFqn).Should().BeTrue();
        registry.OwnerOf(EventFqn).Should().Be("mod.weather");

        registry.RemoveOwner("mod.weather");

        registry.Provides($"mod.weather.publish:{EventFqn}").Should().BeFalse();
        registry.Provides($"mod.weather.normal.publish:{EventFqn}").Should().BeFalse();
        registry.Owns("mod.weather", EventFqn).Should().BeFalse();
        registry.OwnerOf(EventFqn).Should().BeNull("no owner records the type once its owner is gone");
    }

    [Fact]
    public void RemoveOwner_IsIdempotent_AndUnknownOwnersAreANoOp()
    {
        var registry = new KernelCapabilityRegistry();
        registry.RegisterOwner("mod.weather", TestAssembly);

        registry.RemoveOwner("mod.weather");
        registry.RemoveOwner("mod.weather");
        registry.RemoveOwner("mod.never.registered");

        registry.Capabilities.Should().BeEmpty();
    }

    [Fact]
    public void RemoveOwner_DoesNotTakeANestedOwnerIdsTokensWithIt()
    {
        // "mod.a" is a string PREFIX of "mod.ab". A prefix sweep over the flat token set would
        // delete mod.ab's tokens while removing mod.a — the exact latent cross-owner deletion bug
        // the per-owner token bookkeeping exists to prevent.
        var registry = new KernelCapabilityRegistry();
        registry.RegisterOwner("mod.a", TestAssembly);
        registry.RegisterOwner("mod.ab", TestAssembly);

        registry.RemoveOwner("mod.a");

        registry.Provides($"mod.a.publish:{EventFqn}").Should().BeFalse("mod.a was removed");
        registry.Provides($"mod.ab.publish:{EventFqn}").Should().BeTrue(
            "mod.ab is a DIFFERENT owner whose id merely starts with mod.a's");
        registry.Owns("mod.ab", EventFqn).Should().BeTrue();
    }

    [Fact]
    public void RegisterOwner_IsIdempotentPerOwner_SoAReloadDoesNotDoubleCount()
    {
        var registry = new KernelCapabilityRegistry();

        registry.RegisterOwner("mod.weather", TestAssembly);
        int afterFirst = registry.Capabilities.Count;

        registry.RegisterOwner("mod.weather", TestAssembly);

        registry.Capabilities.Count.Should().Be(afterFirst,
            "re-registering the same assembly under the same owner is what a mod RELOAD does");
        registry.Owns("mod.weather", EventFqn).Should().BeTrue();

        // And the re-registration is still fully removable — the token bookkeeping did not
        // accumulate a second, unremovable copy.
        registry.RemoveOwner("mod.weather");
        registry.Provides($"mod.weather.publish:{EventFqn}").Should().BeFalse();
    }

    [Fact]
    public void RemoveOwner_LeavesOtherOwnersUntouched()
    {
        var registry = new KernelCapabilityRegistry();
        registry.RegisterOwner("kernel", TestAssembly);
        registry.RegisterOwner("mod.weather", TestAssembly);

        registry.RemoveOwner("mod.weather");

        registry.ProvidesKernel($"kernel.publish:{EventFqn}").Should().BeTrue(
            "the kernel owner's registration is independent of any mod's");
        registry.OwnerOf(EventFqn).Should().Be("kernel");
    }
}
