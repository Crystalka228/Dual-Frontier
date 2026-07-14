using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

/// <summary>
/// V0.B SPIR-V toolchain integration tests. Verifies pre-compiled .spv files exist
/// in assets/shaders/, are non-empty, properly aligned (4-byte multiples for SPIR-V),
/// и loadable into VkShaderModule on real Vulkan device.
/// </summary>
public sealed class ShaderCompilationTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;

    public ShaderCompilationTests()
    {
        var opts = new WindowOptions { Title = "ShaderCompile", Width = 320, Height = 240 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
    }

    public void Dispose()
    {
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    private static string FindShaderPath(string name)
    {
        // MSBuild target writes к $(SolutionDir)assets/shaders/. Tests run from
        // tests/DualFrontier.Runtime.Tests/bin/Debug/net8.0 — solution root is 6 levels up.
        // Use AppContext.BaseDirectory + relative climb to локализуй repo root.
        string baseDir = AppContext.BaseDirectory;
        var dir = new DirectoryInfo(baseDir);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "DualFrontier.sln")))
        {
            dir = dir.Parent;
        }
        if (dir == null)
        {
            throw new InvalidOperationException("Could not locate DualFrontier.sln from test base dir.");
        }
        return Path.Combine(dir.FullName, "assets", "shaders", name);
    }

    [WindowsOnlyTheory]
    [InlineData("clearcolor.vert.spv")]
    [InlineData("clearcolor.frag.spv")]
    [InlineData("noop.comp.spv")]
    public void SpvFile_exists_and_is_aligned(string name)
    {
        string path = FindShaderPath(name);
        File.Exists(path).Should().BeTrue($"MSBuild CompileShaders target should produce {name}");
        long len = new FileInfo(path).Length;
        len.Should().BeGreaterThan(0L);
        (len % 4).Should().Be(0L, "SPIR-V bytecode is uint32 aligned");
    }

    [WindowsOnlyTheory]
    [InlineData("clearcolor.vert.spv")]
    [InlineData("clearcolor.frag.spv")]
    [InlineData("noop.comp.spv")]
    public void VulkanShaderModule_loads_compiled_spirv(string name)
    {
        string path = FindShaderPath(name);
        using var module = VulkanShaderModule.LoadFromFile(_device, path);
        module.Handle.Should().NotBe(IntPtr.Zero);
    }
}
