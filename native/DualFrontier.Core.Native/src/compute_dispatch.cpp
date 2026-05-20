#include "compute_dispatch.h"

#include "compute_pipeline.h"
#include "tile_field.h"
#include "world.h"

#include <cstring>
#include <vector>

namespace dualfrontier {

namespace {

// Helper: find а memory type satisfying both `required_flags` and the bits of
// `type_bits` reported by vkGetBufferMemoryRequirements. Returns UINT32_MAX if
// no compatible type exists.
uint32_t find_memory_type(VkPhysicalDevice physical_device,
                          uint32_t type_bits,
                          VkMemoryPropertyFlags required_flags) noexcept
{
    VkPhysicalDeviceMemoryProperties props{};
    vkGetPhysicalDeviceMemoryProperties(physical_device, &props);
    for (uint32_t i = 0; i < props.memoryTypeCount; ++i) {
        const bool type_bit_ok = (type_bits & (1u << i)) != 0;
        const bool flags_ok = (props.memoryTypes[i].propertyFlags & required_flags) == required_flags;
        if (type_bit_ok && flags_ok) {
            return i;
        }
    }
    return UINT32_MAX;
}

// Lazy allocation of one VkBuffer + bound device memory of byte_size bytes,
// usage = STORAGE_BUFFER, host-visible + host-coherent. Returns true on
// success. On failure, both handles remain VK_NULL_HANDLE.
bool ensure_buffer(VkDevice device,
                   VkPhysicalDevice physical_device,
                   uint32_t& cached_memory_type,
                   int32_t byte_size,
                   VkBuffer& out_buffer,
                   VkDeviceMemory& out_memory) noexcept
{
    if (out_buffer != VK_NULL_HANDLE) {
        return true;
    }
    if (byte_size <= 0) {
        return false;
    }

    VkBufferCreateInfo buf_info{};
    buf_info.sType = VK_STRUCTURE_TYPE_BUFFER_CREATE_INFO;
    buf_info.size = static_cast<VkDeviceSize>(byte_size);
    buf_info.usage = VK_BUFFER_USAGE_STORAGE_BUFFER_BIT;
    buf_info.sharingMode = VK_SHARING_MODE_EXCLUSIVE;

    if (vkCreateBuffer(device, &buf_info, nullptr, &out_buffer) != VK_SUCCESS) {
        out_buffer = VK_NULL_HANDLE;
        return false;
    }

    VkMemoryRequirements mem_reqs{};
    vkGetBufferMemoryRequirements(device, out_buffer, &mem_reqs);

    if (cached_memory_type == UINT32_MAX) {
        cached_memory_type = find_memory_type(
            physical_device, mem_reqs.memoryTypeBits,
            VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT);
    }
    if (cached_memory_type == UINT32_MAX || (mem_reqs.memoryTypeBits & (1u << cached_memory_type)) == 0) {
        vkDestroyBuffer(device, out_buffer, nullptr);
        out_buffer = VK_NULL_HANDLE;
        return false;
    }

    VkMemoryAllocateInfo alloc_info{};
    alloc_info.sType = VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO;
    alloc_info.allocationSize = mem_reqs.size;
    alloc_info.memoryTypeIndex = cached_memory_type;

    if (vkAllocateMemory(device, &alloc_info, nullptr, &out_memory) != VK_SUCCESS) {
        vkDestroyBuffer(device, out_buffer, nullptr);
        out_buffer = VK_NULL_HANDLE;
        return false;
    }
    if (vkBindBufferMemory(device, out_buffer, out_memory, 0) != VK_SUCCESS) {
        vkFreeMemory(device, out_memory, nullptr);
        vkDestroyBuffer(device, out_buffer, nullptr);
        out_buffer = VK_NULL_HANDLE;
        out_memory = VK_NULL_HANDLE;
        return false;
    }
    return true;
}

// Copy `byte_size` bytes from `src` к the mapped memory of `dst_memory`. The
// memory must be HOST_VISIBLE | HOST_COHERENT (no explicit flush required).
bool upload_to_buffer(VkDevice device, VkDeviceMemory dst_memory,
                      const void* src, int32_t byte_size) noexcept
{
    void* mapped = nullptr;
    if (vkMapMemory(device, dst_memory, 0, static_cast<VkDeviceSize>(byte_size), 0, &mapped) != VK_SUCCESS) {
        return false;
    }
    std::memcpy(mapped, src, static_cast<size_t>(byte_size));
    vkUnmapMemory(device, dst_memory);
    return true;
}

// Mirror of upload_to_buffer in reverse: copy GPU memory к CPU.
bool download_from_buffer(VkDevice device, VkDeviceMemory src_memory,
                          void* dst, int32_t byte_size) noexcept
{
    void* mapped = nullptr;
    if (vkMapMemory(device, src_memory, 0, static_cast<VkDeviceSize>(byte_size), 0, &mapped) != VK_SUCCESS) {
        return false;
    }
    std::memcpy(dst, mapped, static_cast<size_t>(byte_size));
    vkUnmapMemory(device, src_memory);
    return true;
}

// Lazy command pool + command buffer + fence creation. Idempotent на success.
bool ensure_dispatch_resources(VkDevice device, VulkanAttachment& att) noexcept
{
    if (att.command_pool == VK_NULL_HANDLE) {
        VkCommandPoolCreateInfo pool_info{};
        pool_info.sType = VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO;
        pool_info.flags = VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT;
        pool_info.queueFamilyIndex = att.async_compute_queue_family_index;
        if (vkCreateCommandPool(device, &pool_info, nullptr, &att.command_pool) != VK_SUCCESS) {
            att.command_pool = VK_NULL_HANDLE;
            return false;
        }
    }
    if (att.command_buffer == VK_NULL_HANDLE) {
        VkCommandBufferAllocateInfo alloc_info{};
        alloc_info.sType = VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO;
        alloc_info.commandPool = att.command_pool;
        alloc_info.level = VK_COMMAND_BUFFER_LEVEL_PRIMARY;
        alloc_info.commandBufferCount = 1;
        if (vkAllocateCommandBuffers(device, &alloc_info, &att.command_buffer) != VK_SUCCESS) {
            att.command_buffer = VK_NULL_HANDLE;
            return false;
        }
    }
    if (att.dispatch_fence == VK_NULL_HANDLE) {
        VkFenceCreateInfo fence_info{};
        fence_info.sType = VK_STRUCTURE_TYPE_FENCE_CREATE_INFO;
        if (vkCreateFence(device, &fence_info, nullptr, &att.dispatch_fence) != VK_SUCCESS) {
            att.dispatch_fence = VK_NULL_HANDLE;
            return false;
        }
    }
    return true;
}

// Lazy descriptor pool creation. We allocate а pool sized for one set of up
// to kMaxBindings storage buffer descriptors, with FREE_DESCRIPTOR_SET_BIT so
// per-dispatch sets can be freed back. К-L19 hardware tier easily satisfies
// this — even mobile-class Vulkan implementations support hundreds of pool
// sizes. V1+ multi-field coexistence will tax this further; bump kMaxBindings
// if needed.
constexpr uint32_t kMaxBindingsPerSet = 8;
constexpr uint32_t kDescriptorPoolSetCount = 64;

bool ensure_descriptor_pool(VkDevice device, VulkanAttachment& att) noexcept
{
    if (att.descriptor_pool != VK_NULL_HANDLE) {
        return true;
    }
    VkDescriptorPoolSize pool_size{};
    pool_size.type = VK_DESCRIPTOR_TYPE_STORAGE_BUFFER;
    pool_size.descriptorCount = kMaxBindingsPerSet * kDescriptorPoolSetCount;

    VkDescriptorPoolCreateInfo pool_info{};
    pool_info.sType = VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO;
    pool_info.flags = VK_DESCRIPTOR_POOL_CREATE_FREE_DESCRIPTOR_SET_BIT;
    pool_info.maxSets = kDescriptorPoolSetCount;
    pool_info.poolSizeCount = 1;
    pool_info.pPoolSizes = &pool_size;
    return vkCreateDescriptorPool(device, &pool_info, nullptr, &att.descriptor_pool) == VK_SUCCESS;
}

}  // anonymous namespace

bool dispatch_compute_field(World& world,
                            const std::string& field_name,
                            uint32_t pipeline_id,
                            const uint8_t* push_constant_data,
                            int32_t push_constant_size,
                            uint32_t dispatch_x,
                            uint32_t dispatch_y,
                            uint32_t dispatch_z)
{
    if (!world.has_vulkan_attached()) return false;

    const ComputePipelineEntry* entry = world.compute_pipelines().get_pipeline(pipeline_id);
    if (entry == nullptr) return false;

    RawTileField* field = world.get_field(field_name);
    if (field == nullptr) return false;

    VulkanAttachment& att = const_cast<VulkanAttachment&>(world.vulkan_attachment());
    VkDevice device = static_cast<VkDevice>(att.device);
    VkPhysicalDevice physical_device = static_cast<VkPhysicalDevice>(att.physical_device);
    VkQueue queue = static_cast<VkQueue>(att.async_compute_queue);

    // 1. Ensure dispatch resources (cmd pool, cmd buffer, fence, descriptor pool).
    if (!ensure_dispatch_resources(device, att)) return false;
    if (!ensure_descriptor_pool(device, att)) return false;

    // 2. Ensure shadow VkBuffers exist для this field.
    const int32_t cell_size = field->cell_size();
    const int32_t width = field->width();
    const int32_t height = field->height();
    const int32_t byte_size = width * height * cell_size;
    const int32_t cond_byte_size = width * height * static_cast<int32_t>(sizeof(float));

    FieldShadowBuffers& bufs = att.field_buffers[field_name];
    if (bufs.byte_size != byte_size) {
        // Field dimensions changed — free stale buffers (rare).
        if (bufs.input != VK_NULL_HANDLE) { vkDestroyBuffer(device, bufs.input, nullptr); bufs.input = VK_NULL_HANDLE; }
        if (bufs.output != VK_NULL_HANDLE) { vkDestroyBuffer(device, bufs.output, nullptr); bufs.output = VK_NULL_HANDLE; }
        if (bufs.conductivity != VK_NULL_HANDLE) { vkDestroyBuffer(device, bufs.conductivity, nullptr); bufs.conductivity = VK_NULL_HANDLE; }
        if (bufs.input_memory != VK_NULL_HANDLE) { vkFreeMemory(device, bufs.input_memory, nullptr); bufs.input_memory = VK_NULL_HANDLE; }
        if (bufs.output_memory != VK_NULL_HANDLE) { vkFreeMemory(device, bufs.output_memory, nullptr); bufs.output_memory = VK_NULL_HANDLE; }
        if (bufs.conductivity_memory != VK_NULL_HANDLE) { vkFreeMemory(device, bufs.conductivity_memory, nullptr); bufs.conductivity_memory = VK_NULL_HANDLE; }
        bufs.byte_size = 0;
        bufs.conductivity_byte_size = 0;
    }
    if (!ensure_buffer(device, physical_device, att.host_coherent_memory_type,
                       byte_size, bufs.input, bufs.input_memory)) return false;
    if (!ensure_buffer(device, physical_device, att.host_coherent_memory_type,
                       byte_size, bufs.output, bufs.output_memory)) return false;
    if (entry->descriptor_binding_count >= 3) {
        if (!ensure_buffer(device, physical_device, att.host_coherent_memory_type,
                           cond_byte_size, bufs.conductivity, bufs.conductivity_memory)) return false;
    }
    bufs.byte_size = byte_size;
    bufs.conductivity_byte_size = cond_byte_size;

    // 3. Upload field's CPU state → input buffer.
    if (!upload_to_buffer(device, bufs.input_memory, field->data_ptr(), byte_size)) return false;
    if (entry->descriptor_binding_count >= 3) {
        if (!upload_to_buffer(device, bufs.conductivity_memory, field->conductivity_ptr(), cond_byte_size)) return false;
    }

    // 4. Allocate descriptor set.
    VkDescriptorSetAllocateInfo dset_info{};
    dset_info.sType = VK_STRUCTURE_TYPE_DESCRIPTOR_SET_ALLOCATE_INFO;
    dset_info.descriptorPool = att.descriptor_pool;
    dset_info.descriptorSetCount = 1;
    dset_info.pSetLayouts = &entry->descriptor_set_layout;

    VkDescriptorSet descriptor_set = VK_NULL_HANDLE;
    if (vkAllocateDescriptorSets(device, &dset_info, &descriptor_set) != VK_SUCCESS) {
        return false;
    }

    // 5. Bind buffers к descriptor set.
    std::vector<VkDescriptorBufferInfo> buffer_infos;
    std::vector<VkWriteDescriptorSet> writes;
    buffer_infos.reserve(entry->descriptor_binding_count);
    writes.reserve(entry->descriptor_binding_count);

    VkBuffer chosen[3] = {bufs.input, bufs.output, bufs.conductivity};
    int32_t chosen_size[3] = {byte_size, byte_size, cond_byte_size};

    for (uint32_t i = 0; i < entry->descriptor_binding_count; ++i) {
        VkDescriptorBufferInfo info{};
        info.buffer = (i < 3) ? chosen[i] : VK_NULL_HANDLE;
        info.offset = 0;
        info.range = (i < 3) ? static_cast<VkDeviceSize>(chosen_size[i]) : 0;
        if (info.buffer == VK_NULL_HANDLE) {
            vkFreeDescriptorSets(device, att.descriptor_pool, 1, &descriptor_set);
            return false;
        }
        buffer_infos.push_back(info);
    }
    for (uint32_t i = 0; i < entry->descriptor_binding_count; ++i) {
        VkWriteDescriptorSet w{};
        w.sType = VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
        w.dstSet = descriptor_set;
        w.dstBinding = i;
        w.dstArrayElement = 0;
        w.descriptorCount = 1;
        w.descriptorType = VK_DESCRIPTOR_TYPE_STORAGE_BUFFER;
        w.pBufferInfo = &buffer_infos[i];
        writes.push_back(w);
    }
    if (!writes.empty()) {
        vkUpdateDescriptorSets(device, static_cast<uint32_t>(writes.size()),
                               writes.data(), 0, nullptr);
    }

    // 6. Record + submit command buffer.
    vkResetCommandBuffer(att.command_buffer, 0);
    VkCommandBufferBeginInfo begin_info{};
    begin_info.sType = VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO;
    begin_info.flags = VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT;
    if (vkBeginCommandBuffer(att.command_buffer, &begin_info) != VK_SUCCESS) {
        vkFreeDescriptorSets(device, att.descriptor_pool, 1, &descriptor_set);
        return false;
    }
    vkCmdBindPipeline(att.command_buffer, VK_PIPELINE_BIND_POINT_COMPUTE, entry->pipeline);
    vkCmdBindDescriptorSets(att.command_buffer, VK_PIPELINE_BIND_POINT_COMPUTE,
                            entry->pipeline_layout, 0, 1, &descriptor_set, 0, nullptr);
    if (push_constant_data != nullptr && push_constant_size > 0
        && entry->push_constant_size >= static_cast<uint32_t>(push_constant_size)) {
        vkCmdPushConstants(att.command_buffer, entry->pipeline_layout,
                           VK_SHADER_STAGE_COMPUTE_BIT, 0,
                           static_cast<uint32_t>(push_constant_size),
                           push_constant_data);
    }
    vkCmdDispatch(att.command_buffer, dispatch_x, dispatch_y, dispatch_z);
    if (vkEndCommandBuffer(att.command_buffer) != VK_SUCCESS) {
        vkFreeDescriptorSets(device, att.descriptor_pool, 1, &descriptor_set);
        return false;
    }

    VkSubmitInfo submit{};
    submit.sType = VK_STRUCTURE_TYPE_SUBMIT_INFO;
    submit.commandBufferCount = 1;
    submit.pCommandBuffers = &att.command_buffer;

    vkResetFences(device, 1, &att.dispatch_fence);
    if (vkQueueSubmit(queue, 1, &submit, att.dispatch_fence) != VK_SUCCESS) {
        vkFreeDescriptorSets(device, att.descriptor_pool, 1, &descriptor_set);
        return false;
    }
    // К-L7 atomic-from-observer: synchronously wait for compute completion.
    vkWaitForFences(device, 1, &att.dispatch_fence, VK_TRUE, UINT64_MAX);

    // 7. Read back output buffer → field's primary CPU storage.
    if (!download_from_buffer(device, bufs.output_memory, field->data_ptr(), byte_size)) {
        vkFreeDescriptorSets(device, att.descriptor_pool, 1, &descriptor_set);
        return false;
    }

    // 8. Free the per-dispatch descriptor set (pool keeps slot для next dispatch).
    vkFreeDescriptorSets(device, att.descriptor_pool, 1, &descriptor_set);

    return true;
}

void release_dispatch_resources(VulkanAttachment& att) noexcept
{
    if (!att.attached || att.device == nullptr) {
        att.field_buffers.clear();
        return;
    }
    VkDevice device = static_cast<VkDevice>(att.device);

    // Wait for any in-flight work signalled on the fence; idempotent if fence
    // is already signalled or never used.
    if (att.dispatch_fence != VK_NULL_HANDLE) {
        vkWaitForFences(device, 1, &att.dispatch_fence, VK_TRUE, UINT64_MAX);
    }

    for (auto& kv : att.field_buffers) {
        auto& b = kv.second;
        if (b.input != VK_NULL_HANDLE) vkDestroyBuffer(device, b.input, nullptr);
        if (b.output != VK_NULL_HANDLE) vkDestroyBuffer(device, b.output, nullptr);
        if (b.conductivity != VK_NULL_HANDLE) vkDestroyBuffer(device, b.conductivity, nullptr);
        if (b.input_memory != VK_NULL_HANDLE) vkFreeMemory(device, b.input_memory, nullptr);
        if (b.output_memory != VK_NULL_HANDLE) vkFreeMemory(device, b.output_memory, nullptr);
        if (b.conductivity_memory != VK_NULL_HANDLE) vkFreeMemory(device, b.conductivity_memory, nullptr);
    }
    att.field_buffers.clear();

    if (att.descriptor_pool != VK_NULL_HANDLE) {
        vkDestroyDescriptorPool(device, att.descriptor_pool, nullptr);
        att.descriptor_pool = VK_NULL_HANDLE;
    }
    if (att.dispatch_fence != VK_NULL_HANDLE) {
        vkDestroyFence(device, att.dispatch_fence, nullptr);
        att.dispatch_fence = VK_NULL_HANDLE;
    }
    if (att.command_pool != VK_NULL_HANDLE) {
        // Command buffers allocated from the pool are freed implicitly.
        vkDestroyCommandPool(device, att.command_pool, nullptr);
        att.command_pool = VK_NULL_HANDLE;
        att.command_buffer = VK_NULL_HANDLE;
    }
    att.host_coherent_memory_type = UINT32_MAX;
}

}  // namespace dualfrontier
