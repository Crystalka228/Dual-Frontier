using System;

namespace DualFrontier.Contracts.IPC;

/// <summary>
/// K10.1 Item 9 — Declares the system as the writer для a shared memory region.
/// Enforces single-writer / multi-reader convention для high-frequency IPC
/// data (positions, velocities, animation state) where bus event serialization
/// overhead dominates.
///
/// K10.1: convention metadata only; runtime panic-enforcement deferred к К10.2
/// where mod ALC lifecycle integration makes runtime ownership tracking
/// fully meaningful.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class ShmWriterAttribute : Attribute
{
    /// <summary>Region id this system writes к.</summary>
    public uint RegionId { get; }
    /// <summary>Region size в bytes.</summary>
    public int SizeBytes { get; }

    /// <summary>Create the attribute for the given region id and size.</summary>
    public ShmWriterAttribute(uint regionId, int sizeBytes)
    {
        RegionId = regionId;
        SizeBytes = sizeBytes;
    }
}
