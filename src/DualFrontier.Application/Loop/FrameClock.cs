using System;
using System.Diagnostics;

namespace DualFrontier.Application.Loop
{
    /// <summary>
    /// Monotonic wall-clock timer for the simulation loop.
    /// Uses Stopwatch — no DateTime.Now, no drift.
    /// </summary>
    public sealed class FrameClock
    {
        private readonly Stopwatch _sw = Stopwatch.StartNew();
        private long _lastTicks = 0;

        /// <summary>Elapsed time since FrameClock was created.</summary>
        public TimeSpan Now => _sw.Elapsed;

        /// <summary>
        /// Returns seconds elapsed since last call to Update().
        /// Call once per loop iteration.
        /// </summary>
        public float Update()
        {
            long current = _sw.ElapsedTicks;
            float delta = (float)(current - _lastTicks)
                          / Stopwatch.Frequency;
            _lastTicks = current;
            return delta;
        }
    }
}