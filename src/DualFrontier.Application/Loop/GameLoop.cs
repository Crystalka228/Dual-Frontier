using System;
using System.Diagnostics;
using System.Threading;
using DualFrontier.Core.Scheduling;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Application.Bus;

namespace DualFrontier.Application.Loop
{
    /// <summary>
    /// Fixed-step simulation loop running on a dedicated background thread.
    /// Ticks at TargetTps (30 Hz) independent of render FPS.
    /// Communicates with Presentation only through PresentationBridge: per
    /// fixed-step tick the loop enqueues a <see cref="TickAdvancedCommand"/>
    /// carrying the current <c>TickScheduler.CurrentTick</c> value so the
    /// HUD's tick label can update on the Godot main thread.
    /// Internal — created by GameBootstrap, not exposed to Presentation.
    ///
    /// After each fixed step the loop also drains the native Background-tier
    /// event queue (К-L15 §3.8 Item 30 idle-slot dispatch) within the
    /// remaining tick budget. If <c>busBridge</c> is null (test/legacy
    /// callers) the drain is skipped entirely.
    /// </summary>
    internal sealed class GameLoop : IDisposable
    {
        public const float TargetTps = 30f;
        private const float FixedDelta = 1f / TargetTps;
        private const float MaxAccumulator = FixedDelta * 5f;
        // Reserve 10% of the fixed step for next-tick scheduler overhead +
        // display thread synchronisation — Background drain stops short of
        // burning the entire period.
        private const long FixedDeltaMicros   = (long)(FixedDelta * 1_000_000f);
        private const long IdleSafetyMargin   = FixedDeltaMicros / 10;

        private readonly ParallelSystemScheduler _scheduler;
        private readonly TickScheduler _ticks;
        private readonly PresentationBridge _bridge;
        private readonly ManagedBusBridge? _busBridge;
        private readonly CancellationTokenSource _cts = new();
        private Thread? _thread;
        private volatile float _speedMultiplier = 1f;
        private volatile bool _paused = false;

        public GameLoop(ParallelSystemScheduler scheduler,
                        TickScheduler ticks,
                        PresentationBridge bridge,
                        ManagedBusBridge? busBridge = null)
        {
            _scheduler = scheduler
                ?? throw new ArgumentNullException(nameof(scheduler));
            _ticks = ticks
                ?? throw new ArgumentNullException(nameof(ticks));
            _bridge = bridge
                ?? throw new ArgumentNullException(nameof(bridge));
            _busBridge = busBridge;
        }

        /// <summary>Start simulation on a background thread.</summary>
        public void Start()
        {
            _thread = new Thread(RunLoop)
            {
                Name = "SimulationLoop",
                IsBackground = true
            };
            _thread.Start();
        }

        /// <summary>Stop simulation and wait for thread to exit.</summary>
        public void Stop()
        {
            _cts.Cancel();
            _thread?.Join(2000);
        }

        /// <summary>Pause or resume simulation.</summary>
        public void SetPaused(bool paused) => _paused = paused;

        /// <summary>
        /// True iff the simulation thread is currently sleeping the tick
        /// advance per <see cref="SetPaused"/>. Read-only observation
        /// surface for tests and diagnostics; production code mutates
        /// state via <see cref="SetPaused"/>.
        /// </summary>
        public bool IsPaused => _paused;

        /// <summary>Set speed multiplier. Accepted values: 1, 2, 3.</summary>
        public void SetSpeed(float multiplier) =>
            _speedMultiplier = Math.Clamp(multiplier, 1f, 3f);

        private void RunLoop()
        {
            var clock = new FrameClock();
            float accumulator = 0f;

            while (!_cts.Token.IsCancellationRequested)
            {
                float realDelta = clock.Update();

                if (_paused)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(16));
                    continue;
                }

                accumulator += realDelta * _speedMultiplier;
                accumulator = Math.Min(accumulator, MaxAccumulator);

                while (accumulator >= FixedDelta)
                {
                    long tickStartTicks = Stopwatch.GetTimestamp();
                    _scheduler.ExecuteTick(FixedDelta);
                    _bridge.Enqueue(new TickAdvancedCommand((int)_ticks.CurrentTick));

                    // К-L15 §3.8 Item 30 — drain Background tier with whatever
                    // budget is left before the next fixed step is due.
                    if (_busBridge is not null)
                    {
                        long elapsedMicros =
                            (Stopwatch.GetTimestamp() - tickStartTicks) * 1_000_000L
                            / Stopwatch.Frequency;
                        long idleBudgetMicros = FixedDeltaMicros - elapsedMicros - IdleSafetyMargin;
                        if (idleBudgetMicros > 0)
                            _busBridge.DrainBackgroundBatch((ulong)idleBudgetMicros);
                    }

                    accumulator -= FixedDelta;
                }

                float remaining = (FixedDelta - accumulator)
                                  / _speedMultiplier;
                int sleepMs = (int)(remaining * 1000f);
                if (sleepMs > 1)
                    Thread.Sleep(sleepMs);
            }
        }

        public void Dispose() => Stop();
     }
}