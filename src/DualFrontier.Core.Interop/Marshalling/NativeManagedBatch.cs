using System.Runtime.InteropServices;

namespace DualFrontier.Core.Interop.Marshalling;

/// <summary>
/// K10.1 Item 15 — C struct mirroring native <c>df_managed_system_batch</c>
/// for the batched callback ABI. Pointer + count + delta + opaque GCHandle.
/// Layout matches native sequentially; sizeof must equal C struct size.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct NativeManagedBatch
{
    /// <summary>Pointer к array of managed system IDs.</summary>
    public uint* SystemIds;
    /// <summary>Number of system ids в the batch.</summary>
    public uint Count;
    /// <summary>Tick delta for the dispatch (seconds).</summary>
    public float Delta;
    /// <summary>Opaque managed context handle (GCHandle).</summary>
    public void* UserData;
}
