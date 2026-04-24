using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.Interop.Marshalling;

/// <summary>
/// Packing between <see cref="EntityId"/> and the <c>ulong</c> layout used
/// on the C ABI. The native header (<c>df_capi.h</c>) defines:
///   high 32 bits = Version, low 32 bits = Index.
/// Index and Version are signed ints on both sides; the cast through
/// <c>uint</c> is purely to preserve the bit pattern of negative values
/// (there shouldn't be any in practice — indices are &gt; 0 and versions
/// only grow from 0 — but keeping the reinterpret explicit avoids a
/// silent ABI drift if that ever changes).
/// </summary>
internal static class EntityIdPacking
{
    internal static ulong Pack(EntityId id)
    {
        ulong lo = (uint)id.Index;
        ulong hi = (uint)id.Version;
        return (hi << 32) | lo;
    }

    internal static EntityId Unpack(ulong packed)
    {
        int index = (int)(packed & 0xFFFFFFFFu);
        int version = (int)((packed >> 32) & 0xFFFFFFFFu);
        return new EntityId(index, version);
    }
}
