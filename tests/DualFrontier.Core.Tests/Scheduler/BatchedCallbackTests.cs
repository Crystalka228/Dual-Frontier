using System;
using System.Collections.Generic;
using DualFrontier.Application.Scheduler;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Scheduler;

/// <summary>
/// K10.1 Item 15 — end-to-end batched callback ABI tests.
///
/// Exercises managed→native registration → native dispatch →
/// managed callback executes batch → results validated. Confirms the
/// reverse-P/Invoke entrypoint (<see cref="ManagedSystemDispatcher.OnBatch"/>)
/// is reachable through the native registry and that the GCHandle round-trip
/// resolves к the right instance.
/// </summary>
public sealed class BatchedCallbackTests : IDisposable
{
    private readonly ManagedSystemDispatcher _dispatcher;
    private readonly List<(uint[] ids, float delta)> _capturedBatches;

    public BatchedCallbackTests()
    {
        _capturedBatches = new List<(uint[] ids, float delta)>();
        _dispatcher = new ManagedSystemDispatcher((ids, delta) =>
        {
            _capturedBatches.Add((ids.ToArray(), delta));
        });
        SchedulerAdapter.Register(_dispatcher);
    }

    public void Dispose()
    {
        SchedulerAdapter.ClearCallback();
        _dispatcher.Dispose();
    }

    [Fact]
    public void Register_records_the_callback()
    {
        SchedulerAdapter.IsCallbackRegistered.Should().BeTrue();
    }

    [Fact]
    public void DispatchBatch_round_trips_through_native()
    {
        uint[] ids = new uint[] { 10, 20, 30 };
        bool dispatched = SchedulerAdapter.DispatchBatch(
            ids, delta: 0.033f, userData: _dispatcher.Handle);

        dispatched.Should().BeTrue();
        _capturedBatches.Should().HaveCount(1);
        _capturedBatches[0].ids.Should().BeEquivalentTo(ids);
        _capturedBatches[0].delta.Should().BeApproximately(0.033f, 1e-6f);
    }

    [Fact]
    public void DispatchBatch_with_empty_ids_returns_false_and_does_not_invoke()
    {
        bool dispatched = SchedulerAdapter.DispatchBatch(
            ReadOnlySpan<uint>.Empty, delta: 0.5f, userData: _dispatcher.Handle);

        dispatched.Should().BeFalse();
        _capturedBatches.Should().BeEmpty();
    }

    [Fact]
    public void ClearCallback_unregisters()
    {
        SchedulerAdapter.IsCallbackRegistered.Should().BeTrue();
        SchedulerAdapter.ClearCallback();
        SchedulerAdapter.IsCallbackRegistered.Should().BeFalse();

        uint[] ids = new uint[] { 1, 2 };
        bool dispatched = SchedulerAdapter.DispatchBatch(
            ids, delta: 0.016f, userData: _dispatcher.Handle);
        dispatched.Should().BeFalse();
    }
}
