using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DualFrontier.Application.Scene;

/// <summary>
/// Serialises <see cref="TilemapLayerDef.Tiles"/> as a base64 string over
/// the underlying little-endian byte representation of the <c>ushort[]</c>.
/// A 100×100 tilemap layer shrinks from ~30 KB of JSON numbers to ~27 KB
/// of base64 and survives a text-diff more gracefully than an unbounded
/// array literal.
/// </summary>
public sealed class UshortArrayBase64JsonConverter : JsonConverter<ushort[]>
{
    /// <inheritdoc />
    public override ushort[] Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return Array.Empty<ushort>();

        string? base64 = reader.GetString();
        if (string.IsNullOrEmpty(base64))
            return Array.Empty<ushort>();

        byte[] bytes = Convert.FromBase64String(base64);
        if ((bytes.Length & 1) != 0)
            throw new JsonException(
                "Tilemap tiles base64 payload must have an even byte length.");

        ushort[] tiles = new ushort[bytes.Length / 2];
        Buffer.BlockCopy(bytes, 0, tiles, 0, bytes.Length);
        return tiles;
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer, ushort[] value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        byte[] bytes = new byte[value.Length * sizeof(ushort)];
        Buffer.BlockCopy(value, 0, bytes, 0, bytes.Length);
        writer.WriteStringValue(Convert.ToBase64String(bytes));
    }
}
