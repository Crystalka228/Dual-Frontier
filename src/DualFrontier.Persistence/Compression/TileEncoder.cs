using System;
using System.IO;
using DualFrontier.Components.World;
using DualFrontier.Persistence.Snapshots;

namespace DualFrontier.Persistence.Compression;

/// <summary>
/// Run-length encoder for the tile grid. Writes <c>(count: ushort)(kind: byte)</c>
/// pairs in row-major order. A 100×100 map dominated by a single biome
/// shrinks from 10 000 bytes to a handful of pairs (six bytes for "9 500 grass
/// + 500 rock"). The encoder never mixes runs across the row break — same
/// terrain across a row boundary continues a single run, which lets large
/// homogeneous biomes collapse maximally.
/// </summary>
public static class TileEncoder
{
    /// <summary>
    /// Serialises <paramref name="map"/> as RLE. The output carries no header —
    /// width and height are recovered from <c>SaveMeta</c> on the decode side.
    /// </summary>
    public static byte[] Encode(TileMapSnapshot map)
    {
        if (map is null) throw new ArgumentNullException(nameof(map));

        int total = map.Width * map.Height;
        if (map.Tiles.Length != total)
            throw new ArgumentException(
                $"tile array length ({map.Tiles.Length}) does not match Width*Height ({total}).",
                nameof(map));

        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        int i = 0;
        while (i < total)
        {
            TerrainKind current = map.Tiles[i];
            int runStart = i;
            while (i < total
                   && map.Tiles[i] == current
                   && (i - runStart) < ushort.MaxValue)
            {
                i++;
            }
            bw.Write((ushort)(i - runStart));
            bw.Write((byte)current);
        }

        return ms.ToArray();
    }

    /// <summary>
    /// Reconstructs a <see cref="TileMapSnapshot"/> from RLE bytes. Throws
    /// when the stream under- or over-fills the expected <c>width * height</c>
    /// area so corrupt saves fail at load rather than at first read.
    /// </summary>
    public static TileMapSnapshot Decode(byte[] data, int width, int height)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));

        int total = width * height;
        var tiles = new TerrainKind[total];

        using var ms = new MemoryStream(data, writable: false);
        using var br = new BinaryReader(ms);

        int written = 0;
        while (ms.Position < ms.Length)
        {
            ushort count = br.ReadUInt16();
            var kind = (TerrainKind)br.ReadByte();
            if (written + count > total)
                throw new InvalidDataException(
                    $"RLE overrun: would write {written + count} tiles into a {total}-tile map.");
            for (int j = 0; j < count; j++)
                tiles[written++] = kind;
        }

        if (written != total)
            throw new InvalidDataException(
                $"RLE underrun: wrote {written} tiles into a {total}-tile map.");

        return new TileMapSnapshot { Width = width, Height = height, Tiles = tiles };
    }
}
