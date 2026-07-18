using AwesomeAssertions;
using DualFrontier.Runtime.Graphics;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

/// <summary>
/// Device-free unit tests for the ELT §2.5 prepare-before-reclaim primitive
/// (<see cref="PrepareBeforeReclaim.Build{TSource,T}"/>). They prove the transaction
/// ordering -- build all-or-nothing, roll back partial work on failure, and never return a
/// (committable) array on failure -- using fakes, with NO Vulkan device. This is the mechanism
/// by which M6 keeps the old swapchain/framebuffer set intact on a mid-prepare failure; the
/// real-handle / validation-layer-clean evidence comes from the C7 real-GPU Launcher run.
/// </summary>
public sealed class PrepareBeforeReclaimTests
{
    private sealed class FakeResource
    {
        public int Id { get; init; }
        public bool Reclaimed { get; set; }
    }

    [Fact]
    public void Build_HappyPath_ReturnsResourcesInOrder_AndNeverReclaims()
    {
        var reclaimed = new List<FakeResource>();

        FakeResource[] result = PrepareBeforeReclaim.Build(
            new[] { 10, 20, 30 },
            build: id => new FakeResource { Id = id },
            reclaim: r => { r.Reclaimed = true; reclaimed.Add(r); });

        result.Should().HaveCount(3);
        result.Select(r => r.Id).Should().Equal(new[] { 10, 20, 30 }, "candidates are built in source order");
        reclaimed.Should().BeEmpty("a fully successful build reclaims nothing");
        result.Should().OnlyContain(r => !r.Reclaimed);
    }

    [Fact]
    public void Build_WhenABuildThrows_RollsBackPriorResourcesInOrder_AndDoesNotReturn()
    {
        var created = new List<FakeResource>();
        var reclaimed = new List<FakeResource>();

        var act = () => PrepareBeforeReclaim.Build(
            new[] { 0, 1, 2, 3 },
            build: i =>
            {
                if (i == 2)
                {
                    // Simulate a mid-prepare vkCreateImageView / vkCreateFramebuffer failure.
                    throw new InvalidOperationException(
                        "vkCreateImageView failed: VK_ERROR_OUT_OF_DEVICE_MEMORY");
                }
                var r = new FakeResource { Id = i };
                created.Add(r);
                return r;
            },
            reclaim: r => { r.Reclaimed = true; reclaimed.Add(r); });

        // The exception propagates with NO array returned, so a caller's COMMIT step is
        // unreachable and the caller's OLD state is never touched (ELT §1.1 corollary 1).
        act.Should().Throw<InvalidOperationException>().WithMessage("*vkCreateImageView failed*");

        created.Should().HaveCount(2, "resources 0 and 1 were built before index 2 threw");
        reclaimed.Should().Equal(created, "exactly the built resources are rolled back, in construction order");
        created.Should().OnlyContain(r => r.Reclaimed);
    }

    [Fact]
    public void Build_WhenFirstBuildThrows_ReclaimsNothing_AndThrows()
    {
        var reclaimCount = 0;

        var act = () => PrepareBeforeReclaim.Build<int, FakeResource>(
            new[] { 0, 1, 2 },
            build: _ => throw new InvalidOperationException("immediate failure"),
            reclaim: _ => reclaimCount++);

        act.Should().Throw<InvalidOperationException>();
        reclaimCount.Should().Be(0, "nothing was built, so nothing is reclaimed");
    }

    [Fact]
    public void Build_EmptySources_ReturnsEmpty_AndNeverBuildsOrReclaims()
    {
        var builds = 0;
        var reclaims = 0;

        FakeResource[] result = PrepareBeforeReclaim.Build(
            Array.Empty<int>(),
            build: _ => { builds++; return new FakeResource(); },
            reclaim: _ => reclaims++);

        result.Should().BeEmpty();
        builds.Should().Be(0);
        reclaims.Should().Be(0);
    }
}
