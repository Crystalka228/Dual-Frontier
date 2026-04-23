using DualFrontier.Contracts.Core;
using System;

namespace DualFrontier.AI.Jobs
{
    /// <summary>A pawn meditates in place for a fixed duration, gaining ether growth.</summary>
    /// No movement required — pawn must already be at the meditation spot.
    /// JobSystem reads MeditationTicksCompleted to apply ether gain on Done.
    public sealed class JobMeditate : IJob
    {
        // Dependency: EntityId from DualFrontier.Contracts.Core is used here.
        private readonly EntityId _pawnId;
        public EntityId PawnId => _pawnId;

        private JobStatus _status = JobStatus.Failed; // Default to Failed until Start() is called.
        public JobStatus Status => _status;

        private readonly int _durationTicks;
        /// <summary>Read by JobSystem after completion.</summary>
        public int MeditationTicksCompleted { get; private set; } 

        private int _ticksElapsed = 0;

        /// <summary>
        /// Initializes a new instance of the JobMeditate class.
        /// </summary>
        /// <param name="pawnId">The ID of the pawn performing the job.</param>
        /// <param name="durationTicks">The number of ticks to meditate for (default is 120).</param>
        public JobMeditate(EntityId pawnId, int durationTicks = 120)
        {
            PawnId = pawnId;

            if (durationTicks <= 0)
            {
                throw new ArgumentException("Duration ticks must be positive.", nameof(durationTicks));
            }
            _durationTicks = durationTicks;
        }

        public void Start()
        {
            // Set status to running and reset counters for a fresh execution.
            _status = JobStatus.Running;
            _ticksElapsed = 0;
            MeditationTicksCompleted = 0;
        }

        public JobStatus Tick(float delta)
        {
            if (_status != JobStatus.Running)
            {
                return _status;
            }

            // Increment elapsed ticks (assuming delta represents time passed, which translates to ticks).
            _ticksElapsed++; 

            if (_ticksElapsed >= _durationTicks)
            {
                MeditationTicksCompleted = _ticksElapsed;
                _status = JobStatus.Done;
            }

            return _status;
        }

        public void Abort()
        {
            // Must be aborted if running, otherwise it's a state error, but we set it to Aborted as per contract.
            _status = JobStatus.Aborted;
        }
    }
}