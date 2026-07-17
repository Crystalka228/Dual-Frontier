using DualFrontier.Core.Interop;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// К10.2 Item 32 — native unload primitive managed binding tests.
/// </summary>
public sealed class ModUnloadInteropTests
{
    [Fact]
    public void SimPaused_DefaultIsTrue()
    {
        ModUnloadInterop.SetSimPaused(true);
        ModUnloadInterop.IsSimPaused().Should().BeTrue();
    }

    [Fact]
    public void UnloadModNativeState_VacuousUnload_Succeeds()
    {
        ModUnloadInterop.SetSimPaused(true);
        bool ok = ModUnloadInterop.UnloadModNativeState(0xFFFFFFFFu, out ModUnloadResult result);
        ok.Should().BeTrue();
        result.Success.Should().Be(1);
        result.FastSubscriptionsCleared.Should().Be(0);
        result.NormalSubscriptionsCleared.Should().Be(0);
        result.BackgroundSubscriptionsCleared.Should().Be(0);
        result.ErrorCount.Should().Be(0);
    }

    [Fact]
    public void UnloadModNativeState_SimNotPaused_FailsWithError()
    {
        ModUnloadInterop.SetSimPaused(false);
        bool ok = ModUnloadInterop.UnloadModNativeState(1u, out ModUnloadResult result);
        ok.Should().BeFalse();
        result.Success.Should().Be(0);
        result.ErrorCount.Should().BeGreaterThan(0);
        string err = result.GetErrorMessage(0);
        err.Should().Contain("K-L18");

        ModUnloadInterop.SetSimPaused(true);  // restore default for subsequent tests
    }

    [Fact]
    public void ModUnloadResult_StructLayoutMatchesNative()
    {
        // Marshalling sanity: the struct layout must match native ModUnloadResult.
        // Failing this catches struct shape drift между native + managed.
        int size = System.Runtime.InteropServices.Marshal.SizeOf<ModUnloadResult>();
        // Native: 11 × int (44) + 8×256 byte array (2048) + 1 int (4) = 2096
        size.Should().Be(2096, "struct must marshal к native layout без padding drift");
    }
}
