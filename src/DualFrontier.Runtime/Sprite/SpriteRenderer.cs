using System.Numerics;
using System.Runtime.InteropServices;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// V0.C.1 sprite renderer. Single-sprite per draw call: 6 vertices (2 triangles forming a quad)
/// uploaded к dynamic vertex buffer + descriptor set bound per texture + push-constant MVP +
/// vkCmdDraw(6, 1, 0, 0).
///
/// V0.C.2 will refactor к batched (one draw call per atlas, ~10,000 sprites at 60+ FPS) by:
/// - Accumulating sprites by SpriteTexture key in a frame buffer
/// - Building one large vertex buffer per atlas
/// - Issuing one vkCmdDraw per atlas
/// - Maintaining per-frame descriptor pool sized to active atlas count
///
/// Descriptor set caching: each unique SpriteTexture gets an allocated descriptor set
/// populated via vkUpdateDescriptorSets; subsequent draws с same texture reuse the cached set.
/// </summary>
public sealed class SpriteRenderer : IDisposable
{
    private const ulong VertexBufferSize = 64 * 1024;  // 64 KB — accommodates V0.C.2 batched expansion
    private const uint DescriptorPoolCapacity = 32;

    private readonly VulkanDevice _device;
    private readonly VulkanSpritePipeline _pipeline;
    private readonly VulkanBuffer _vertexBuffer;
    private IntPtr _descriptorPool;
    private readonly Dictionary<SpriteTexture, IntPtr> _descriptorSetCache = new();
    private bool _disposed;

    public SpriteRenderer(
        VulkanDevice device,
        MemoryAllocator allocator,
        VulkanSpritePipeline pipeline)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        ArgumentNullException.ThrowIfNull(pipeline);
        _device = device;
        _pipeline = pipeline;

        _vertexBuffer = new VulkanBuffer(
            _device, allocator, VertexBufferSize,
            VkMemoryPropertyFlagsPublic.HostVisible | VkMemoryPropertyFlagsPublic.HostCoherent,
            VkBufferUsageFlagsPublic.VertexBuffer);

        CreateDescriptorPool();
    }

    private unsafe void CreateDescriptorPool()
    {
        var poolSize = new VkDescriptorPoolSize
        {
            type = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
            descriptorCount = DescriptorPoolCapacity,
        };
        var poolInfo = new VkDescriptorPoolCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            maxSets = DescriptorPoolCapacity,
            poolSizeCount = 1,
            _padBeforePool = 0,
            pPoolSizes = &poolSize,
        };
        VkResult result = VkApi.vkCreateDescriptorPool(_device.Handle, in poolInfo, IntPtr.Zero, out _descriptorPool);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreateDescriptorPool failed: {result}");
        }
    }

    private unsafe IntPtr GetOrCreateDescriptorSet(SpriteTexture texture)
    {
        if (_descriptorSetCache.TryGetValue(texture, out IntPtr cached))
        {
            return cached;
        }
        if (_descriptorSetCache.Count >= DescriptorPoolCapacity)
        {
            throw new InvalidOperationException(
                $"SpriteRenderer descriptor pool exhausted ({DescriptorPoolCapacity} textures). V0.C.2 expands.");
        }

        IntPtr layout = _pipeline.DescriptorSetLayout.Handle;
        var allocInfo = new VkDescriptorSetAllocateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_SET_ALLOCATE_INFO,
            pNext = IntPtr.Zero,
            descriptorPool = _descriptorPool,
            descriptorSetCount = 1,
            _padBeforeLayouts = 0,
            pSetLayouts = &layout,
        };
        IntPtr descriptorSet = IntPtr.Zero;
        VkResult allocResult = VkApi.vkAllocateDescriptorSets(_device.Handle, in allocInfo, &descriptorSet);
        if (allocResult != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkAllocateDescriptorSets failed: {allocResult}");
        }

        var imageInfo = new VkDescriptorImageInfo
        {
            sampler = texture.Sampler.Handle,
            imageView = texture.Image.ViewHandle,
            imageLayout = VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL,
            _padTrailing = 0,
        };
        var write = new VkWriteDescriptorSet
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET,
            pNext = IntPtr.Zero,
            dstSet = descriptorSet,
            dstBinding = 0,
            dstArrayElement = 0,
            descriptorCount = 1,
            descriptorType = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
            pImageInfo = &imageInfo,
            pBufferInfo = null,
            pTexelBufferView = IntPtr.Zero,
        };
        VkApi.vkUpdateDescriptorSets(_device.Handle, 1, &write, 0, IntPtr.Zero);

        _descriptorSetCache[texture] = descriptorSet;
        return descriptorSet;
    }

    /// <summary>
    /// Record sprite draw commands к command buffer. Caller must have already begun a render pass.
    /// V0.C.1 single-sprite per call; V0.C.2 will refactor к batched.
    /// </summary>
    public unsafe void DrawSprite(Sprite sprite, VulkanCommandBuffer commandBuffer, Matrix4x4 mvp)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(sprite.Texture);
        ArgumentNullException.ThrowIfNull(commandBuffer);

        // 1. Build 6 vertices for sprite quad (2 triangles, CCW front face).
        // Quad corners в local space; transform applied via MVP push constant (Camera2D) +
        // SpriteTransform's Position/Scale baked into vertex positions.
        Vector2 halfSize = new(sprite.Transform.Scale.X * 0.5f, sprite.Transform.Scale.Y * 0.5f);
        Vector2 pos = sprite.Transform.Position;
        AtlasRegion uv = sprite.Region;
        uint tint = sprite.Transform.TintRgba;

        // Vulkan NDC: +Y is down, +X is right. UV: (0,0) = top-left, (1,1) = bottom-right.
        Span<SpriteVertex> vertices = stackalloc SpriteVertex[6];
        // Triangle 1: top-left, top-right, bottom-right
        vertices[0] = new(new Vector2(pos.X - halfSize.X, pos.Y - halfSize.Y), new Vector2(uv.U0, uv.V0), tint);  // TL
        vertices[1] = new(new Vector2(pos.X + halfSize.X, pos.Y - halfSize.Y), new Vector2(uv.U1, uv.V0), tint);  // TR
        vertices[2] = new(new Vector2(pos.X + halfSize.X, pos.Y + halfSize.Y), new Vector2(uv.U1, uv.V1), tint);  // BR
        // Triangle 2: top-left, bottom-right, bottom-left
        vertices[3] = new(new Vector2(pos.X - halfSize.X, pos.Y - halfSize.Y), new Vector2(uv.U0, uv.V0), tint);  // TL
        vertices[4] = new(new Vector2(pos.X + halfSize.X, pos.Y + halfSize.Y), new Vector2(uv.U1, uv.V1), tint);  // BR
        vertices[5] = new(new Vector2(pos.X - halfSize.X, pos.Y + halfSize.Y), new Vector2(uv.U0, uv.V1), tint);  // BL

        // 2. Upload vertices к dynamic vertex buffer (host-visible, mapped writes).
        ulong vertexBytes = 6UL * 20UL;
        IntPtr mappedPtr;
        VkResult mapResult = VkApi.vkMapMemory(
            _device.Handle,
            _vertexBuffer.Allocation.DeviceMemory,
            _vertexBuffer.Allocation.Offset,
            vertexBytes,
            0,
            out mappedPtr);
        if (mapResult != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkMapMemory (vertex buffer) failed: {mapResult}");
        }
        try
        {
            fixed (SpriteVertex* vptr = vertices)
            {
                Buffer.MemoryCopy(vptr, mappedPtr.ToPointer(), vertexBytes, vertexBytes);
            }
        }
        finally
        {
            VkApi.vkUnmapMemory(_device.Handle, _vertexBuffer.Allocation.DeviceMemory);
        }

        // 3. Get или create descriptor set for the texture.
        IntPtr descriptorSet = GetOrCreateDescriptorSet(sprite.Texture);

        // 4. Record commands.
        VkApi.vkCmdBindPipeline(
            commandBuffer.Handle,
            VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
            _pipeline.Handle);

        IntPtr setHandle = descriptorSet;
        VkApi.vkCmdBindDescriptorSets(
            commandBuffer.Handle,
            VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
            _pipeline.Layout.Handle,
            firstSet: 0,
            descriptorSetCount: 1,
            pDescriptorSets: &setHandle,
            dynamicOffsetCount: 0,
            pDynamicOffsets: null);

        Matrix4x4 mvpLocal = mvp;
        VkApi.vkCmdPushConstants(
            commandBuffer.Handle,
            _pipeline.Layout.Handle,
            VkShaderStageFlags.VK_SHADER_STAGE_VERTEX_BIT,
            offset: 0,
            size: 64,
            pValues: &mvpLocal);

        IntPtr vbuffer = _vertexBuffer.Handle;
        ulong vbufferOffset = 0;
        VkApi.vkCmdBindVertexBuffers(
            commandBuffer.Handle,
            firstBinding: 0,
            bindingCount: 1,
            pBuffers: &vbuffer,
            pOffsets: &vbufferOffset);

        VkApi.vkCmdDraw(commandBuffer.Handle, vertexCount: 6, instanceCount: 1, firstVertex: 0, firstInstance: 0);
    }

    public int CachedDescriptorSetCount => _descriptorSetCache.Count;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_descriptorPool != IntPtr.Zero)
        {
            VkApi.vkDestroyDescriptorPool(_device.Handle, _descriptorPool, IntPtr.Zero);
            _descriptorPool = IntPtr.Zero;
        }
        _vertexBuffer.Dispose();
        _descriptorSetCache.Clear();
        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(SpriteRenderer));
        }
    }
}
