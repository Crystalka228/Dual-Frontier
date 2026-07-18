using System.IO;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.Interop;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// К10.2 Item 31 — Background queue save-integrated storage tests
/// (S3-Q3 untargeted persistence).
/// </summary>
[Collection("SharedNativeSingleton")]
public sealed class BackgroundQueueInteropTests
{
    [Fact]
    public void Roundtrip_EmptyQueue_HeaderOnly()
    {
        EventTypeRegistryInterop.ClearForTesting();
        var bridge = new DualFrontier.Application.Bus.ManagedBusBridge();
        bridge.ClearForTesting();

        int size = BackgroundQueueInterop.ComputeSaveSize();
        size.Should().Be(12, "header bytes-only for empty queue");

        byte[] serialized = BackgroundQueueInterop.SerializeToArray();
        serialized.Length.Should().Be(12);

        int loaded = BackgroundQueueInterop.DeserializeFromArray(serialized);
        loaded.Should().Be(0);
    }

    [Fact]
    public void Roundtrip_StreamSection_WithLengthPrefix()
    {
        EventTypeRegistryInterop.ClearForTesting();
        var bridge = new DualFrontier.Application.Bus.ManagedBusBridge();
        bridge.ClearForTesting();

        using var ms = new MemoryStream();
        BackgroundQueueInterop.WriteSection(ms);
        ms.Position = 0;

        int loaded = BackgroundQueueInterop.ReadSection(ms);
        loaded.Should().Be(0, "empty queue round-trip");
    }

    [Fact]
    public void ReadSection_FromEmptyStream_ReturnsZero()
    {
        using var ms = new MemoryStream();
        int loaded = BackgroundQueueInterop.ReadSection(ms);
        loaded.Should().Be(0, "missing section gracefully degrades (older save behavior)");
    }

    [Fact]
    public void DeserializeFromArray_Malformed_ReturnsMinusOne()
    {
        // 4 bytes = less than header — should fail gracefully
        byte[] malformed = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
        int loaded = BackgroundQueueInterop.DeserializeFromArray(malformed);
        loaded.Should().Be(-1);
    }

    [Fact]
    public void DeserializeFromArray_UnsupportedSchemaVersion_ReturnsMinusOne()
    {
        // Header с schema_version=999, count=0, total_payload_bytes=0
        byte[] buffer = new byte[12];
        // Little-endian uint32 999 = 0xE7 0x03 0x00 0x00
        buffer[0] = 0xE7;
        buffer[1] = 0x03;
        int loaded = BackgroundQueueInterop.DeserializeFromArray(buffer);
        loaded.Should().Be(-1, "unsupported schema version is graceful rejection");
    }
}
