using System;

namespace DualFrontier.Persistence.Compression;

/// <summary>
/// Quantisation primitives for components whose fields live in a known
/// continuous range (typically pawn needs, mood, percentages). Float-to-byte
/// quantisation drops a 4× overhead with a worst-case error of <c>1/255</c>
/// (~0.4%), which is below the resolution any gameplay system reads back.
///
/// Delta encoding between successive saves is sketched in the README — the
/// concrete layout lives with <c>DfCompressor</c> once incremental saves
/// land. This file owns only the field-level primitives the higher-level
/// encoder composes.
/// </summary>
public static class ComponentEncoder
{
    /// <summary>
    /// Linear quantisation of <paramref name="value"/> in <c>[0, 1]</c> to a
    /// byte in <c>[0, 255]</c>. Out-of-range inputs are clamped — the saver
    /// rejects nothing; corrupt inputs round-trip to a clamped value rather
    /// than throw, since a full save must always succeed.
    /// </summary>
    public static byte QuantizeFloat(float value)
        => (byte)(Math.Clamp(value, 0f, 1f) * 255f);

    /// <summary>
    /// Inverse of <see cref="QuantizeFloat"/>. Maps <c>0..255</c> back into
    /// <c>[0, 1]</c>. Round-trip error is at most <c>1/255</c>.
    /// </summary>
    public static float DequantizeFloat(byte value)
        => value / 255f;
}
