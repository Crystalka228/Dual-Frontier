using System;
using System.Threading;
using System.Threading.Tasks;
using DualFrontier.Application.Loop;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Application.Tests.Loop;

/// <summary>
/// К10.3 v2 Item 42 — К-L18 helper layer wiring tests.
/// Verifies PauseAsync/ResumeAsync invoke ModUnloadInterop.SetSimPaused + the
/// optional notification delegate, и WaitForQuiescenceAsync polls until the
/// pipeline reports quiescent (or timeout).
/// </summary>
public sealed class SimulationStateControllerTests
{
    [Fact]
    public async Task PauseAsync_sets_sim_paused_flag_and_invokes_callback()
    {
        bool? lastPausedState = null;
        var controller = new SimulationStateController(
            onPausedChanged: paused => lastPausedState = paused);

        // Ensure clean baseline
        ModUnloadInterop.SetSimPaused(false);

        await controller.PauseAsync();

        ModUnloadInterop.IsSimPaused().Should().BeTrue();
        lastPausedState.Should().Be(true);

        // cleanup
        await controller.ResumeAsync();
    }

    [Fact]
    public async Task ResumeAsync_clears_sim_paused_flag_and_invokes_callback()
    {
        bool? lastPausedState = null;
        var controller = new SimulationStateController(
            onPausedChanged: paused => lastPausedState = paused);

        await controller.PauseAsync();
        lastPausedState = null;  // reset to observe Resume callback

        await controller.ResumeAsync();

        ModUnloadInterop.IsSimPaused().Should().BeFalse();
        lastPausedState.Should().Be(false);

        // cleanup
        ModUnloadInterop.SetSimPaused(true);
    }

    [Fact]
    public void IsPipelineQuiescent_returns_true_when_override_reports_true()
    {
        var controller = new SimulationStateController(
            isPipelineQuiescentOverride: () => true);

        controller.IsPipelineQuiescent().Should().BeTrue();
    }

    [Fact]
    public void IsPipelineQuiescent_returns_false_when_override_reports_false()
    {
        var controller = new SimulationStateController(
            isPipelineQuiescentOverride: () => false);

        controller.IsPipelineQuiescent().Should().BeFalse();
    }

    [Fact]
    public async Task WaitForQuiescenceAsync_succeeds_when_quiescent_immediately()
    {
        var controller = new SimulationStateController(
            isPipelineQuiescentOverride: () => true);

        bool quiesced = await controller.WaitForQuiescenceAsync(
            timeout: TimeSpan.FromSeconds(1),
            pollInterval: TimeSpan.FromMilliseconds(10));

        quiesced.Should().BeTrue();
    }

    [Fact]
    public async Task WaitForQuiescenceAsync_times_out_when_never_quiescent()
    {
        var controller = new SimulationStateController(
            isPipelineQuiescentOverride: () => false);

        bool quiesced = await controller.WaitForQuiescenceAsync(
            timeout: TimeSpan.FromMilliseconds(100),
            pollInterval: TimeSpan.FromMilliseconds(10));

        quiesced.Should().BeFalse();
    }

    [Fact]
    public async Task WaitForQuiescenceAsync_returns_true_when_eventually_quiescent()
    {
        int callCount = 0;
        var controller = new SimulationStateController(
            isPipelineQuiescentOverride: () =>
            {
                callCount++;
                return callCount >= 3;  // quiescent on 3rd poll
            });

        bool quiesced = await controller.WaitForQuiescenceAsync(
            timeout: TimeSpan.FromSeconds(1),
            pollInterval: TimeSpan.FromMilliseconds(10));

        quiesced.Should().BeTrue();
        callCount.Should().BeGreaterOrEqualTo(3);
    }

    [Fact]
    public async Task WaitForQuiescenceAsync_respects_cancellation_token()
    {
        var controller = new SimulationStateController(
            isPipelineQuiescentOverride: () => false);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        Func<Task> wait = async () => await controller.WaitForQuiescenceAsync(
            timeout: TimeSpan.FromSeconds(5),
            pollInterval: TimeSpan.FromMilliseconds(10),
            cancellationToken: cts.Token);

        await wait.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public void WaitForQuiescenceAsync_rejects_negative_timeout()
    {
        var controller = new SimulationStateController();

        Func<Task> wait = async () => await controller.WaitForQuiescenceAsync(
            timeout: TimeSpan.FromSeconds(-1));

        wait.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void WaitForQuiescenceAsync_rejects_non_positive_poll_interval()
    {
        var controller = new SimulationStateController();

        Func<Task> wait = async () => await controller.WaitForQuiescenceAsync(
            timeout: TimeSpan.FromSeconds(1),
            pollInterval: TimeSpan.Zero);

        wait.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }
}
