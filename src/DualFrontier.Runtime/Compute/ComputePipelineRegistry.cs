using DualFrontier.Runtime.Graphics;

namespace DualFrontier.Runtime.Compute;

/// <summary>
/// Named compute pipeline registry. V substrate consumers (V1 diffusion shader, V2 wave shader)
/// register their pipelines at startup; runtime composers лук them up by name. Registry owns
/// the pipelines + descriptors and disposes everything alongside Runtime facade.
/// </summary>
public sealed class ComputePipelineRegistry : IDisposable
{
    private readonly VulkanDevice _device;
    private readonly Dictionary<string, RegistrationEntry> _entries = new();
    private bool _disposed;

    public ComputePipelineRegistry(VulkanDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        _device = device;
    }

    public VulkanComputePipeline Register(
        string name,
        VulkanShaderModule computeShader,
        IReadOnlyList<ComputeDescriptorBinding> bindings)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_entries.ContainsKey(name))
        {
            throw new InvalidOperationException($"Compute pipeline '{name}' already registered.");
        }
        var descriptors = new VulkanComputeDescriptors(_device, bindings);
        VulkanComputePipeline pipeline;
        try
        {
            pipeline = new VulkanComputePipeline(_device, computeShader, descriptors);
        }
        catch
        {
            descriptors.Dispose();
            throw;
        }
        _entries[name] = new RegistrationEntry(pipeline, descriptors);
        return pipeline;
    }

    public VulkanComputePipeline? Get(string name)
        => _entries.TryGetValue(name, out RegistrationEntry entry) ? entry.Pipeline : null;

    public IReadOnlyList<string> RegisteredNames => _entries.Keys.ToList();
    public int Count => _entries.Count;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        foreach ((_, RegistrationEntry entry) in _entries)
        {
            entry.Pipeline.Dispose();
            entry.Descriptors.Dispose();
        }
        _entries.Clear();
        _disposed = true;
    }

    private readonly record struct RegistrationEntry(VulkanComputePipeline Pipeline, VulkanComputeDescriptors Descriptors);
}
