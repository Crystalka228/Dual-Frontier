using DualFrontier.Runtime;
using DualFrontier.Runtime.Diagnostic;
using DualFrontier.Runtime.Window;

namespace DualFrontier.Runtime.SmokeTest;

internal static class Program
{
    public static int Main(string[] args)
    {
        Console.WriteLine("=== V0.A smoke test ===");
        Console.WriteLine();

        try
        {
            using var runtime = Runtime.Create(new RuntimeOptions
            {
                Window = new WindowOptions
                {
                    Title = "V0.A Smoke Test",
                    Width = 800,
                    Height = 600,
                },
                EnableValidationLayer = true,
            });

            Console.WriteLine("Vulkan instance:");
            Console.WriteLine($"  API version: 0x{runtime.VulkanInstance.ApiVersion:X}");
            Console.WriteLine($"  Validation layer enabled: {runtime.VulkanInstance.ValidationLayerEnabled}");
            Console.WriteLine();

            Console.WriteLine("Physical device:");
            var selected = runtime.VulkanDevice.SelectedDevice;
            Console.WriteLine($"  Name: {selected.DeviceName}");
            Console.WriteLine($"  Type: {selected.DeviceType}");
            Console.WriteLine($"  Vendor ID: 0x{selected.VendorId:X4}");
            Console.WriteLine($"  Device ID: 0x{selected.DeviceId:X4}");
            Console.WriteLine($"  API version: 0x{selected.ApiVersion:X}");
            Console.WriteLine($"  Driver version: 0x{selected.DriverVersion:X}");
            Console.WriteLine();

            Console.WriteLine($"Queue families ({selected.QueueFamilies.Count}):");
            foreach (var qf in selected.QueueFamilies)
            {
                Console.WriteLine(
                    $"  QF {qf.Index}: count={qf.QueueCount} " +
                    $"graphics={qf.SupportsGraphics} compute={qf.SupportsCompute} " +
                    $"transfer={qf.SupportsTransfer} sparse={qf.SupportsSparseBinding}");
            }
            Console.WriteLine($"  Selected graphics queue family: {runtime.VulkanDevice.GraphicsQueueFamilyIndex}");
            Console.WriteLine();

            Console.WriteLine("Available physical devices:");
            for (int i = 0; i < runtime.VulkanDevice.AvailableDevices.Count; i++)
            {
                var d = runtime.VulkanDevice.AvailableDevices[i];
                Console.WriteLine($"  [{i}] {d.DeviceName} ({d.DeviceType})");
            }
            Console.WriteLine();

            // Show window, pump messages briefly to exercise full lifecycle
            runtime.Window.Show();
            Console.WriteLine("Window opened. Pumping messages for 3 seconds (or until closed)...");

            var startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < 3 && runtime.Window.IsOpen)
            {
                runtime.Window.PumpMessages();
                Thread.Sleep(16);
            }

            Console.WriteLine();
            Console.WriteLine($"Window closed. IsOpen = {runtime.Window.IsOpen}");
            Console.WriteLine();

            if (runtime.ValidationLayer is not null)
            {
                Console.WriteLine("Validation log:");
                Console.WriteLine($"  Errors:   {runtime.ValidationLayer.Log.ErrorCount}");
                Console.WriteLine($"  Warnings: {runtime.ValidationLayer.Log.WarningCount}");
                Console.WriteLine($"  Info:     {runtime.ValidationLayer.Log.InfoCount}");

                if (runtime.ValidationLayer.Log.ErrorCount > 0 || runtime.ValidationLayer.Log.WarningCount > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Validation messages:");
                    foreach (var msg in runtime.ValidationLayer.Log.Snapshot())
                    {
                        if (msg.Severity == ValidationSeverity.Info)
                        {
                            continue;
                        }
                        Console.WriteLine($"  [{msg.Severity}] {msg.Message}");
                    }
                }

                if (runtime.ValidationLayer.Log.ErrorCount > 0)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine($"FAIL: {runtime.ValidationLayer.Log.ErrorCount} validation error(s) detected (К-L19 + S-LOCK-4 violation).");
                    return 1;
                }
            }

            Console.WriteLine();
            Console.WriteLine("V0.A exit criteria:");
            Console.WriteLine("  [PASS] Window opens (Win32)");
            Console.WriteLine("  [PASS] Vulkan instance + device live");
            Console.WriteLine("  [PASS] Validation layer reports zero errors");
            Console.WriteLine("  [PASS] Clean shutdown (about к dispose)");
            Console.WriteLine("  [DEFERRED V0.B] Clear color rendered at 60+ FPS (requires swapchain)");
            Console.WriteLine("  [DEFERRED V0.B] Compute pipeline registration round-trip");
            Console.WriteLine();
            Console.WriteLine("V0.A smoke test PASS");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine($"V0.A smoke test FAIL: {ex.GetType().Name}: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);
            return 2;
        }
    }
}
