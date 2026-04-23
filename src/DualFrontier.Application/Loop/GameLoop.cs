using System;
using System.Threading;
using DualFrontier.Core.Scheduling;
using DualFrontier.Application.Bridge;

namespace DualFrontier.Application.Loop
{
    /// <summary>
    /// Fixed-step simulation loop running on a dedicated background thread.
    /// Ticks at TargetTps (30 Hz) independent of render FPS.
    /// Communicates with Presentation only through PresentationBridge.
    /// Internal — created by GameBootstrap, not exposed to Presentation.
    /// </summary>
    internal sealed class GameLoop : IDisposable
    {
        public const float TargetTps = 30f;
        private const float FixedDelta = 1f / TargetTps;
        private const float MaxAccumulator = FixedDelta * 5f;

        private readonly ParallelSystemScheduler _scheduler;
        private readonly PresentationBridge _bridge;
        private readonly CancellationTokenSource _cts = new();
        private Thread? _thread;
        private volatile float _speedMultiplier = 1f;
        private volatile bool _paused = false;

        public GameLoop(ParallelSystemScheduler scheduler,
                        PresentationBridge bridge)
        {
            _scheduler = scheduler
                ?? throw new ArgumentNullException(nameof(scheduler));
            _bridge = bridge
                ?? throw new ArgumentNullException(nameof(bridge));
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
                    _scheduler.ExecuteTick(FixedDelta);
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