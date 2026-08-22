using System;
using System.Collections.Generic;
using System.IO;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Persistence.Compression;

/// <summary>
/// Range encoder for live entity ids. <c>EntityId.Index</c> is monotonic and
/// recycled freed slots cluster, so live indices form a few dense runs with
/// sparse gaps. Encoding each run as <c>(start: int)(count: ushort)</c>
/// collapses the typical "first 200 entities, then a few stragglers" layout
/// to a small number of pairs instead of one int per entity.
///
/// <c>EntityId.Version</c> is dropped — saves capture a moment in time and
/// the version reset on load gives every restored entity a fresh, valid
/// stamp. Cross-references inside the snapshot use <c>EntityIndex</c>
/// directly, not the full <c>EntityId</c>.
/// </summary>
public static class EntityEncoder
{
    /// <summary>
    /// Sorts <paramref name="entities"/> by index and emits a packed run-list.
    /// Duplicate indices are collapsed; <see cref="EntityId.Invalid"/> is
    /// skipped.
    /// </summary>
    public static byte[] EncodeRanges(IEnumerable<EntityId> entities)
    {
        if (entities is null) throw new ArgumentNullException(nameof(entities));

        var indices = new List<int>();
        foreach (var e in entities)
        {
            if (e.Index <= 0) continue;
            indices.Add(e.Index);
        }
        indices.Sort();

        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        int i = 0;
        while (i < indices.Count)
        {
            int start = indices[i];
            int prev  = start;
            int count = 1;
            i++;
            while (i < indices.Count
                   && (indices[i] == prev + 1 || indices[i] == prev)
                   && count < ushort.MaxValue)
            {
                if (indices[i] != prev) // dedupe consecutive duplicates
                {
                    count++;
                    prev = indices[i];
                }
                i++;
            }
            bw.Write(start);
            bw.Write((ushort)count);
        }

        return ms.ToArray();
    }

    /// <summary>
    /// Inverse of <see cref="EncodeRanges"/>. Returns entities with version 0;
    /// callers reassign versions when re-creating live entities through their
    /// world.
    /// </summary>
    public static EntityId[] DecodeRanges(byte[] data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));

        var result = new List<EntityId>();
        using var ms = new MemoryStream(data, writable: false);
        using var br = new BinaryReader(ms);

        while (ms.Position < ms.Length)
        {
            int    start = br.ReadInt32();
            ushort count = br.ReadUInt16();
            // DFK-WAIVER(DFK022): the ONE sanctioned version fabrication in the tree
            // (К-L22; IDENTITY_AND_ABI_CONTRACT §2 "consequential amendments"). This
            // decodes an INDEX range out of a save; the encoded form carries no
            // versions, and there is no world to ask — the entities these ids name do
            // not exist yet. How generations cross the save boundary is the A7
            // persistence contract's jurisdiction (PERSISTENCE_SNAPSHOT_CONTRACT.md),
            // not this cascade's. RETIREMENT TRIGGER: when A7 decides the persisted
            // identity format, this site adopts it and the waiver goes away — the
            // waiver census pin (TESTING_STRATEGY §4.3) drops 3 -> 2 at that cascade.
            // Callers are already told (see the DecodeRanges summary) that they must
            // reassign versions when re-creating live entities through their world.
#pragma warning disable DFK022
            for (int j = 0; j < count; j++)
                result.Add(new EntityId(start + j, 0));
#pragma warning restore DFK022
        }

        return result.ToArray();
    }
}
