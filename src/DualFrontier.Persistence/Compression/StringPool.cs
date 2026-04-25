using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DualFrontier.Persistence.Compression;

/// <summary>
/// Bounded interning table for save-time string deduplication. Item ids,
/// recipe ids, and similar tag strings recur thousands of times across a
/// simulation; the pool emits each unique string once and references it
/// elsewhere by a 16-bit handle.
///
/// Capacity is bounded to <see cref="ushort.MaxValue"/> entries — beyond that
/// the encoder must spill to a wider id type. The pool fails loudly on
/// overflow rather than silently truncating.
/// </summary>
public sealed class StringPool
{
    private readonly Dictionary<string, ushort> _index = new(StringComparer.Ordinal);
    private readonly List<string> _strings = new();

    /// <summary>Number of unique strings currently held.</summary>
    public int Count => _strings.Count;

    /// <summary>
    /// Returns the existing handle for <paramref name="value"/> or assigns a
    /// fresh one. Throws if the pool would exceed <see cref="ushort.MaxValue"/>
    /// distinct entries.
    /// </summary>
    public ushort Intern(string value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        if (_index.TryGetValue(value, out ushort id)) return id;
        if (_strings.Count >= ushort.MaxValue)
            throw new InvalidOperationException(
                "StringPool overflow: more than 65535 unique strings; widen the handle type.");
        id = (ushort)_strings.Count;
        _strings.Add(value);
        _index[value] = id;
        return id;
    }

    /// <summary>
    /// Resolves a previously-interned handle. Throws on out-of-range ids so
    /// corrupt saves fail fast rather than returning garbage.
    /// </summary>
    public string Resolve(ushort id)
    {
        if (id >= _strings.Count)
            throw new ArgumentOutOfRangeException(
                nameof(id), id, $"unknown string handle (pool size = {_strings.Count}).");
        return _strings[id];
    }

    /// <summary>
    /// Serialises the pool as <c>[count: ushort][len0: ushort][bytes0: utf8]…</c>.
    /// Format is self-delimiting; deserialiser does not need an out-of-band
    /// length, only the byte buffer.
    /// </summary>
    public byte[] Serialize()
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: false);
        bw.Write((ushort)_strings.Count);
        foreach (var s in _strings)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            if (bytes.Length > ushort.MaxValue)
                throw new InvalidOperationException(
                    $"StringPool entry exceeds 65535 bytes (UTF-8 length = {bytes.Length}).");
            bw.Write((ushort)bytes.Length);
            bw.Write(bytes);
        }
        return ms.ToArray();
    }

    /// <summary>
    /// Reconstructs a pool from the byte format produced by <see cref="Serialize"/>.
    /// </summary>
    public static StringPool Deserialize(byte[] data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        var pool = new StringPool();
        using var ms = new MemoryStream(data, writable: false);
        using var br = new BinaryReader(ms, Encoding.UTF8, leaveOpen: false);
        ushort count = br.ReadUInt16();
        for (int i = 0; i < count; i++)
        {
            ushort len = br.ReadUInt16();
            byte[] bytes = br.ReadBytes(len);
            pool.Intern(Encoding.UTF8.GetString(bytes));
        }
        return pool;
    }
}
