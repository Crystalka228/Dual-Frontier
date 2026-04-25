using System;
using DualFrontier.Persistence.Compression;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Persistence.Tests;

public sealed class ComponentEncoderTests
{
    [Fact]
    public void ComponentEncoder_quantize_roundtrip()
    {
        const float input = 0.75f;

        byte quantised = ComponentEncoder.QuantizeFloat(input);
        float restored = ComponentEncoder.DequantizeFloat(quantised);

        Math.Abs(restored - input).Should().BeLessThan(0.005f,
            "quantisation precision is 1/255 ≈ 0.004, well under the 0.005 budget");
    }
}
