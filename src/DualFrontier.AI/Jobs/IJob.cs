using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.AI.Jobs
{
    /// <summary>Defines the execution status of a job.</summary>
    public enum JobStatus
    {
        /// <summary>Job is still executing — call Tick again next frame.</summary>
        Running,
        /// <summary>Job completed successfully.</summary>
        Done,
        /// <summary>Job failed and cannot continue.</summary>
        Failed,
        /// <summary>Job was aborted by external request.</summary>
        Aborted
    }

    /// <summary>
    /// Defines the contract for an AI Job executed by a pawn.
    /// </summary>
    public interface IJob
    {
        /// <summary>Unique identifier of the pawn executing this job.</summary>
        EntityId PawnId { get; }

        /// <summary>Current execution status.</summary>
        JobStatus Status { get; }

        /// <summary>
        /// Initialises the job. Called once before the first Tick.
        /// </summary>
        void Start();

        /// <summary>
        /// Advances the job by one step. Returns current status.
        /// </summary>
        JobStatus Tick(float delta);

        /// <summary>
        /// Aborts the job, rolling back any reserved resources.
        /// After Abort, Status must be Aborted.
        /// </summary>
        void Abort();
    }
}