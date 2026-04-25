using DualFrontier.Contracts.Core;
using DualFrontier.Persistence.Compression;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Persistence.Tests;

public sealed class EntityEncoderTests
{
    [Fact]
    public void EntityEncoder_range_roundtrip()
    {
        EntityId[] input =
        {
            new(1, 0), new(2, 0), new(3, 0), new(4, 0), new(5, 0),
            new(10, 0), new(11, 0), new(12, 0)
        };

        byte[] encoded = EntityEncoder.EncodeRanges(input);
        EntityId[] decoded = EntityEncoder.DecodeRanges(encoded);

        decoded.Should().Equal(input);
        encoded.Length.Should().BeLessThan(32,
            "two consecutive runs collapse to two (start:int, count:ushort) pairs = 12 bytes");
    }
}
