using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop.Marshalling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class EntityIdPackingTests
{
    [Fact]
    public void Pack_then_Unpack_preserves_index_and_version()
    {
        var original = new EntityId(42, 7);
        ulong packed = EntityIdPacking.Pack(original);
        EntityId unpacked = EntityIdPacking.Unpack(packed);

        unpacked.Index.Should().Be(42);
        unpacked.Version.Should().Be(7);
    }

    [Fact]
    public void Pack_zero_entity_yields_zero_ulong()
    {
        var zero = new EntityId(0, 0);
        ulong packed = EntityIdPacking.Pack(zero);

        packed.Should().Be(0UL);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(1_000_000, 999)]
    public void Pack_unpack_roundtrip(int index, int version)
    {
        var original = new EntityId(index, version);
        ulong packed = EntityIdPacking.Pack(original);
        EntityId roundtrip = EntityIdPacking.Unpack(packed);

        roundtrip.Index.Should().Be(index);
        roundtrip.Version.Should().Be(version);
    }

    [Fact]
    public void Pack_layout_matches_capi_h_specification()
    {
        // df_capi.h spec: high 32 bits = Version, low 32 bits = Index.
        var id = new EntityId(unchecked((int)0xCAFEBABE), 0x12345678);
        ulong packed = EntityIdPacking.Pack(id);

        ulong expectedHigh = (ulong)0x12345678 << 32;
        ulong expectedLow = (ulong)0xCAFEBABEu;

        packed.Should().Be(expectedHigh | expectedLow);
    }
}
