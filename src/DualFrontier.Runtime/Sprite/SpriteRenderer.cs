using System.Numerics;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// V0.C.2 batched sprite renderer per S-LOCK-7 (90% rewrite from V0.C.1 single-sprite).
///
/// API:
///   BeginFrame(uint frameIndex) — starts batch on ring buffer chunk N (frameIndex % frameCount)
///   Submit(Sprite) — accumulates sprite into atlas group bucket
///   EndFrame(VulkanCommandBuffer, Camera2D) — writes vertices, records commands,
///     issues one vkCmdDrawIndexed per atlas group, MVP = camera.ViewProjectionMatrix
///
/// Vertex layout (per quad, consumed via uint16 index pattern 0,1,2,2,3,0):
///   [0] TL = top-left   (pos - halfSize, uv(U0,V0))
///   [1] TR = top-right  (pos.X + halfSize.X, pos.Y - halfSize.Y, uv(U1,V0))
///   [2] BR = bottom-right (pos + halfSize, uv(U1,V1))
///   [3] BL = bottom-left (pos.X - halfSize.X, pos.Y + halfSize.Y, uv(U0,V1))
///
/// Single-atlas optimization (R.2 10K stress): all sprites share one SpriteTexture key
/// → single dictionary entry → single vkCmdDrawIndexed. Verifiable в RenderDoc.
///
/// Per-frame descriptor pool reset NOT implemented in V0.C.2 (per-frame state hassle);
/// descriptor pool sized к 64 keeps cached sets across frames per V0.C.1 precedent.
/// </summary>
public sealed class SpriteRenderer : IDisposable
{
    private const uint DescriptorPoolCapacity = 64;
    private const int DefaultMaxSpritesPerFrame = 10_000;

    private readonly VulkanDevice _device;
    private readonly VulkanSpritePipeline _pipeline;
    private readonly VertexBufferRing _vertexRing;
    private readonly SpriteIndexBuffer _indexBuffer;
    private readonly int _maxSpritesPerFrame;
    private readonly Dictionary<SpriteTexture, List<Sprite>> _frameAtlasGroups = new();
    private readonly Dictionary<SpriteTexture, IntPtr> _descriptorSetCache = new();
    private IntPtr _descriptorPool;
    private uint _currentFrameIndex;
    private bool _frameActive;
    private bool _disposed;

    public int MaxSpritesPerFrame => _maxSpritesPerFrame;
    public int CurrentFrameSubmissionCount
    {
        get
        {
            int total = 0;
            foreach (var list in _frameAtlasGroups.Values)
            {
                total += list.Count;
            }
            return total;
        }
    }
    public int CachedDescriptorSetCount => _descriptorSetCache.Count;

    public SpriteRenderer(
        VulkanDevice device,
        MemoryAllocator allocator,
        VulkanSpritePipeline pipeline,
        int swapchainImageCount,
        int maxSpritesPerFrame = DefaultMaxSpritesPerFrame)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        ArgumentNullException.ThrowIfNull(pipeline);
        if (swapchainImageCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(swapchainImageCount));
        }
        if (maxSpritesPerFrame <= 0 || maxSpritesPerFrame > SpriteIndexBuffer.MaxUint16Quads)
        {
            throw new ArgumentOutOfRangeException(nameof(maxSpritesPerFrame),
                $"Must be 1..{SpriteIndexBuffer.MaxUint16Quads}.");
        }

        _device = device;
        _pipeline = pipeline;
        _maxSpritesPerFrame = maxSpritesPerFrame;

        _vertexRing = new VertexBufferRing(device, allocator, swapchainImageCount, maxSpritesPerFrame);
        _indexBuffer = new SpriteIndexBuffer(device, allocator, maxSpritesPerFrame);
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

    /// <summary>Begin batched frame. frameIndex typically = swapchain image index.</summary>
    public void BeginFrame(uint frameIndex)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_frameActive)
        {
            throw new InvalidOperationException("SpriteRenderer.BeginFrame called twice without EndFrame.");
        }

        _currentFrameIndex = frameIndex;
        _frameAtlasGroups.Clear();
        _vertexRing.BeginFrame(frameIndex);
        _frameActive = true;
    }

    /// <summary>Submit sprite для batched rendering. Sprites grouped by SpriteTexture key.</summary>
    public void Submit(Sprite sprite)
    {
        if (!_frameActive)
        {
            throw new InvalidOperationException("SpriteRenderer.Submit called without BeginFrame.");
        }
        if (sprite.Texture is null)
        {
            throw new ArgumentNullException(nameof(sprite), "Sprite.Texture is null.");
        }

        if (CurrentFrameSubmissionCount >= _maxSpritesPerFrame)
        {
            throw new InvalidOperationException(
                $"SpriteRenderer frame capacity exceeded ({_maxSpritesPerFrame} per S-LOCK-3a hard cap). " +
                "Multiple BeginFrame/EndFrame cycles required.");
        }

        if (!_frameAtlasGroups.TryGetValue(sprite.Texture, out var list))
        {
            list = new List<Sprite>();
            _frameAtlasGroups[sprite.Texture] = list;
        }
        list.Add(sprite);
    }

    /// <summary>
    /// Record draw commands к command buffer. One vkCmdDrawIndexed per atlas group.
    /// MVP push constant = camera.ViewProjectionMatrix.
    /// </summary>
    public unsafe void EndFrame(VulkanCommandBuffer commandBuffer, Camera2D camera)
    {
        ArgumentNullException.ThrowIfNull(camera);
        EndFrame(commandBuffer, camera.ViewProjectionMatrix);
    }

    /// <summary>
    /// Record draw commands using an explicit MVP push constant instead of deriving it from a
    /// <see cref="Camera2D"/>. Lets callers that already hold a matrix — e.g. the V0.C.1
    /// backward-compat single-sprite shim — pass it straight through. One vkCmdDrawIndexed per
    /// atlas group.
    /// </summary>
    public unsafe void EndFrame(VulkanCommandBuffer commandBuffer, Matrix4x4 mvp)
    {
        if (!_frameActive)
        {
            throw new InvalidOperationException("SpriteRenderer.EndFrame called without BeginFrame.");
        }
        ArgumentNullException.ThrowIfNull(commandBuffer);

        try
        {
            // 1. Write vertices к ring buffer chunk grouped by atlas.
            var atlasDrawRanges = new List<(SpriteTexture Texture, int VertexOffset, int IndexCount)>();
            int totalSpritesWritten = 0;

            foreach (var kvp in _frameAtlasGroups)
            {
                SpriteTexture texture = kvp.Key;
                List<Sprite> sprites = kvp.Value;
                int vertexOffset = totalSpritesWritten * 4;
                int indexCount = sprites.Count * SpriteIndexBuffer.IndicesPerQuad;

                foreach (Sprite sprite in sprites)
                {
                    WriteSpriteVertices(sprite);
                    totalSpritesWritten++;
                }

                atlasDrawRanges.Add((texture, vertexOffset, indexCount));
            }

            ulong vertexChunkOffset = _vertexRing.EndFrame();

            // 2. Bind pipeline + push constants + vertex/index buffers.
            VkApi.vkCmdBindPipeline(
                commandBuffer.Handle,
                VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
                _pipeline.Handle);

            VkApi.vkCmdPushConstants(
                commandBuffer.Handle,
                _pipeline.Layout.Handle,
                VkShaderStageFlags.VK_SHADER_STAGE_VERTEX_BIT,
                offset: 0,
                size: 64,
                pValues: &mvp);

            IntPtr vbuffer = _vertexRing.Handle;
            ulong vbufferOffset = vertexChunkOffset;
            VkApi.vkCmdBindVertexBuffers(
                commandBuffer.Handle,
                firstBinding: 0,
                bindingCount: 1,
                pBuffers: &vbuffer,
                pOffsets: &vbufferOffset);

            VkApi.vkCmdBindIndexBuffer(
                commandBuffer.Handle,
                _indexBuffer.Handle,
                offset: 0,
                VkIndexType.VK_INDEX_TYPE_UINT16);

            // 3. Issue one vkCmdDrawIndexed per atlas group.
            int currentFirstIndex = 0;
            foreach (var range in atlasDrawRanges)
            {
                IntPtr descriptorSet = GetOrCreateDescriptorSet(range.Texture);
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

                VkApi.vkCmdDrawIndexed(
                    commandBuffer.Handle,
                    indexCount: (uint)range.IndexCount,
                    instanceCount: 1,
                    firstIndex: (uint)currentFirstIndex,
                    vertexOffset: range.VertexOffset,
                    firstInstance: 0);

                currentFirstIndex += range.IndexCount;
            }
        }
        finally
        {
            _frameActive = false;
        }
    }

    private void WriteSpriteVertices(Sprite sprite)
    {
        Vector2 halfSize = new(sprite.Transform.Scale.X * 0.5f, sprite.Transform.Scale.Y * 0.5f);
        Vector2 pos = sprite.Transform.Position;
        AtlasRegion uv = sprite.Region;
        uint tint = sprite.Transform.TintRgba;

        // Vulkan NDC +Y down: TL has smaller Y; BR has larger Y.
        SpriteVertex tl = new(new Vector2(pos.X - halfSize.X, pos.Y - halfSize.Y), new Vector2(uv.U0, uv.V0), tint);
        SpriteVertex tr = new(new Vector2(pos.X + halfSize.X, pos.Y - halfSize.Y), new Vector2(uv.U1, uv.V0), tint);
        SpriteVertex br = new(new Vector2(pos.X + halfSize.X, pos.Y + halfSize.Y), new Vector2(uv.U1, uv.V1), tint);
        SpriteVertex bl = new(new Vector2(pos.X - halfSize.X, pos.Y + halfSize.Y), new Vector2(uv.U0, uv.V1), tint);

        _vertexRing.WriteSprite(in tl, in tr, in br, in bl);
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
                $"SpriteRenderer descriptor pool exhausted ({DescriptorPoolCapacity} textures).");
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
        _indexBuffer.Dispose();
        _vertexRing.Dispose();
        _descriptorSetCache.Clear();
        _disposed = true;
    }
}
