#include "pipeline_slot.h"

#include <array>
#include <atomic>
#include <cstring>

namespace {

// Global pipeline state. Static storage — process lifetime ownership.
// Per К-L16: D=1-3 slots, default 2; static max=3 covers full range.
//
// Slot access pattern (per spec §3.10 Item 33 state machine):
//   allocate_slot: cycles cursor through D slots; transitions slot к Dispatched.
//   get_slot(offset): relative read (-1 = tail, -2..-D = display tail).
//   check_fences: external Vulkan integration (Commit 4 wires actual vkGetFenceStatus).
//   transition_to_tail: FenceCompleted → ReadableAsTail (fires wake via filter Commit 7).
//   is_quiescent: K-L18 prerequisite — all slots Empty or ReadableAsTail.
struct PipelineState {
    std::array<PipelineSlot, DF_PIPELINE_MAX_DEPTH> slots{};
    std::atomic<int32_t> depth{0};            // 0 = не initialized
    std::atomic<int32_t> cursor{0};           // Next slot index к allocate (mod depth)
    std::atomic<uint64_t> allocation_count{0}; // Total slots ever allocated (для offset math)
    std::atomic<int32_t> paused{0};           // Pause protocol (Item 34): 1 = no new allocations
    std::atomic<int32_t> wake_fire_count{0};  // Item 37: K10.3 v2 transition wake fire counter
};

PipelineState g_pipeline;

}  // namespace

extern "C" {

int32_t df_pipeline_init(int32_t depth) {
    if (depth < 1 || depth > DF_PIPELINE_MAX_DEPTH) {
        return 0;
    }
    int32_t expected = 0;
    if (!g_pipeline.depth.compare_exchange_strong(expected, depth)) {
        return 0;  // Already initialized — idempotency through error per К10.2 pattern.
    }
    // Clear все slots к Empty.
    for (auto& slot : g_pipeline.slots) {
        std::memset(&slot, 0, sizeof(slot));
        slot.state = SlotState_Empty;
    }
    g_pipeline.cursor.store(0);
    g_pipeline.allocation_count.store(0);
    g_pipeline.paused.store(0);
    g_pipeline.wake_fire_count.store(0);
    return 1;
}

void df_pipeline_reset(void) {
    g_pipeline.depth.store(0);
    g_pipeline.cursor.store(0);
    g_pipeline.allocation_count.store(0);
    g_pipeline.paused.store(0);
    g_pipeline.wake_fire_count.store(0);
    for (auto& slot : g_pipeline.slots) {
        std::memset(&slot, 0, sizeof(slot));
        slot.state = SlotState_Empty;
    }
}

int32_t df_pipeline_get_depth(int32_t* out_depth) {
    if (!out_depth) {
        return 0;
    }
    *out_depth = g_pipeline.depth.load();
    return 1;
}

int32_t df_pipeline_allocate_slot(uint64_t sim_tick, PipelineSlot** out_slot) {
    if (!out_slot) {
        return 0;
    }
    const int32_t depth = g_pipeline.depth.load();
    if (depth == 0) {
        return 0;  // Не initialized.
    }
    // Pause protocol (Item 34): no new allocations accepted while paused.
    // Existing in-flight slots drain naturally via fence check + transition.
    if (g_pipeline.paused.load()) {
        *out_slot = nullptr;
        return 0;
    }
    // Cycle cursor через D slots. Backpressure: if next slot is Dispatched/
    // FenceCompleted (in-flight compute), caller must wait or К-L16 invariant
    // violated. Allocate proceeds only on Empty или ReadableAsTail slots
    // (latter recycled per К-L16 cycle).
    const int32_t cursor_value = g_pipeline.cursor.load();
    const int32_t slot_index = cursor_value % depth;
    PipelineSlot* slot = &g_pipeline.slots[slot_index];

    const int32_t state = slot->state;
    if (state != SlotState_Empty && state != SlotState_ReadableAsTail) {
        // Slot still in-flight — backpressure. Caller must wait для slot к drain.
        *out_slot = nullptr;
        return 0;
    }

    // Recycle slot — assign new sim_tick + transition к Dispatched.
    slot->sim_tick = sim_tick;
    slot->world_snapshot_ptr = nullptr;
    slot->fields_snapshot_ptr = nullptr;
    slot->compute_fence_handle = nullptr;
    slot->state = SlotState_Dispatched;

    g_pipeline.cursor.store(cursor_value + 1);
    g_pipeline.allocation_count.fetch_add(1);

    *out_slot = slot;
    return 1;
}

int32_t df_pipeline_get_slot(int32_t slot_offset, PipelineSlot** out_slot) {
    if (!out_slot) {
        return 0;
    }
    const int32_t depth = g_pipeline.depth.load();
    if (depth == 0) {
        *out_slot = nullptr;
        return 0;
    }
    // Offset semantics: 0 = most-recently-allocated (current), -1 = previous,
    // -(D-1) = display tail. Out-of-range offsets fail.
    if (slot_offset > 0 || slot_offset <= -depth) {
        *out_slot = nullptr;
        return 0;
    }
    const uint64_t count = g_pipeline.allocation_count.load();
    if (count == 0) {
        // Never allocated — no slots accessible.
        *out_slot = nullptr;
        return 0;
    }
    // Cursor points к next-к-allocate. Most-recent = cursor-1.
    const int32_t cursor_value = g_pipeline.cursor.load();
    const int32_t recent_index = (cursor_value - 1 + depth) % depth;
    const int32_t target_index = (recent_index + slot_offset + depth) % depth;
    *out_slot = &g_pipeline.slots[target_index];
    return 1;
}

int32_t df_pipeline_set_fence(PipelineSlot* slot, void* vk_fence_handle) {
    if (!slot) {
        return 0;
    }
    slot->compute_fence_handle = vk_fence_handle;
    return 1;
}

int32_t df_pipeline_check_fences(int32_t* out_slots_transitioned) {
    // K10.3 v2 Commit 3 scope: stub. Real vkGetFenceStatus integration lands
    // в Commit 4 (Phase.Compute) когда VulkanAttachment context surfaces.
    // Callers wiring fence polling должны use df_pipeline_force_fence_completed
    // (test path) или direct vkGetFenceStatus + df_pipeline_force_fence_completed
    // until Commit 4 wires the Vulkan integration.
    if (out_slots_transitioned) {
        *out_slots_transitioned = 0;
    }
    return 1;
}

int32_t df_pipeline_force_fence_completed(PipelineSlot* slot) {
    if (!slot) {
        return 0;
    }
    if (slot->state != SlotState_Dispatched) {
        return 0;
    }
    slot->state = SlotState_FenceCompleted;
    return 1;
}

int32_t df_pipeline_transition_to_tail(PipelineSlot* slot) {
    if (!slot) {
        return 0;
    }
    if (slot->state != SlotState_FenceCompleted) {
        return 0;
    }
    slot->state = SlotState_ReadableAsTail;
    // K10.3 v2 Item 37: slot transition fires wake hook. Full subscriber
    // registry integration deferred к К-extensions; counter increment proves
    // infrastructure operational + provides observability surface.
    g_pipeline.wake_fire_count.fetch_add(1);
    return 1;
}

int32_t df_pipeline_read_slot_tail(
    int32_t slot_offset,
    void** out_field_snapshot,
    uint64_t* out_sim_tick) {
    if (!out_field_snapshot || !out_sim_tick) {
        return 0;
    }
    PipelineSlot* slot = nullptr;
    if (df_pipeline_get_slot(slot_offset, &slot) != 1 || !slot) {
        *out_field_snapshot = nullptr;
        *out_sim_tick = 0;
        return 0;
    }
    // К-L7.1 validation: slot must be в ReadableAsTail or FenceCompleted state
    // (fence signaled, results available). Reading от Dispatched slot returns 0.
    if (slot->state != SlotState_ReadableAsTail &&
        slot->state != SlotState_FenceCompleted) {
        *out_field_snapshot = nullptr;
        *out_sim_tick = 0;
        return 0;
    }
    *out_field_snapshot = slot->fields_snapshot_ptr;
    *out_sim_tick = slot->sim_tick;
    return 1;
}

int32_t df_pipeline_serialize_display_state(
    void* buffer,
    int32_t buffer_size,
    int32_t* out_bytes_written) {
    if (!buffer || !out_bytes_written) {
        return 0;
    }
    const int32_t depth = g_pipeline.depth.load();
    if (depth == 0) {
        return 0;
    }
    const int32_t needed = DF_PIPELINE_SNAPSHOT_HEADER_SIZE +
                           depth * DF_PIPELINE_SNAPSHOT_PER_SLOT_SIZE;
    if (buffer_size < needed) {
        *out_bytes_written = 0;
        return 0;
    }
    uint8_t* cursor = static_cast<uint8_t*>(buffer);
    // Header: depth (int32).
    std::memcpy(cursor, &depth, sizeof(int32_t));
    cursor += sizeof(int32_t);
    // Per-slot: sim_tick (8) + state (4) + reserved (4).
    for (int32_t i = 0; i < depth; ++i) {
        const PipelineSlot& slot = g_pipeline.slots[i];
        std::memcpy(cursor, &slot.sim_tick, sizeof(uint64_t));
        cursor += sizeof(uint64_t);
        std::memcpy(cursor, &slot.state, sizeof(int32_t));
        cursor += sizeof(int32_t);
        // Reserved 4 bytes для future expansion (fields_snapshot_size hint, etc.).
        const int32_t reserved = 0;
        std::memcpy(cursor, &reserved, sizeof(int32_t));
        cursor += sizeof(int32_t);
    }
    *out_bytes_written = needed;
    return 1;
}

int32_t df_pipeline_deserialize_display_state(
    const void* buffer,
    int32_t buffer_size) {
    if (!buffer) {
        return 0;
    }
    if (buffer_size < DF_PIPELINE_SNAPSHOT_HEADER_SIZE) {
        return 0;
    }
    const uint8_t* cursor = static_cast<const uint8_t*>(buffer);
    int32_t saved_depth = 0;
    std::memcpy(&saved_depth, cursor, sizeof(int32_t));
    cursor += sizeof(int32_t);
    if (saved_depth < 1 || saved_depth > DF_PIPELINE_MAX_DEPTH) {
        return 0;
    }
    const int32_t current_depth = g_pipeline.depth.load();
    if (current_depth != saved_depth) {
        return 0;  // Depth mismatch — caller must init с matching depth first.
    }
    const int32_t needed = DF_PIPELINE_SNAPSHOT_HEADER_SIZE +
                           saved_depth * DF_PIPELINE_SNAPSHOT_PER_SLOT_SIZE;
    if (buffer_size < needed) {
        return 0;
    }
    uint64_t max_sim_tick = 0;
    for (int32_t i = 0; i < saved_depth; ++i) {
        PipelineSlot& slot = g_pipeline.slots[i];
        std::memcpy(&slot.sim_tick, cursor, sizeof(uint64_t));
        cursor += sizeof(uint64_t);
        std::memcpy(&slot.state, cursor, sizeof(int32_t));
        cursor += sizeof(int32_t);
        cursor += sizeof(int32_t);  // Skip reserved.
        slot.world_snapshot_ptr = nullptr;
        slot.fields_snapshot_ptr = nullptr;
        slot.compute_fence_handle = nullptr;
        if (slot.sim_tick > max_sim_tick) {
            max_sim_tick = slot.sim_tick;
        }
    }
    // Restore allocation_count + cursor consistent с saved state.
    g_pipeline.allocation_count.store(max_sim_tick);
    g_pipeline.cursor.store(static_cast<int32_t>(max_sim_tick % saved_depth) + 1);
    return 1;
}

int32_t df_pipeline_pause(void) {
    if (g_pipeline.depth.load() == 0) {
        return 0;
    }
    g_pipeline.paused.store(1);
    return 1;
}

int32_t df_pipeline_resume(void) {
    if (g_pipeline.depth.load() == 0) {
        return 0;
    }
    g_pipeline.paused.store(0);
    return 1;
}

int32_t df_pipeline_is_paused(int32_t* out_is_paused) {
    if (!out_is_paused) {
        return -1;
    }
    if (g_pipeline.depth.load() == 0) {
        *out_is_paused = 0;
        return -1;
    }
    *out_is_paused = g_pipeline.paused.load();
    return 1;
}

int32_t df_pipeline_get_wake_fire_count(int32_t* out_count) {
    if (!out_count) {
        return 0;
    }
    *out_count = g_pipeline.wake_fire_count.load();
    return 1;
}

void df_pipeline_reset_wake_fire_count(void) {
    g_pipeline.wake_fire_count.store(0);
}

int32_t df_pipeline_is_quiescent(int32_t* out_is_quiescent) {
    if (!out_is_quiescent) {
        return -1;
    }
    const int32_t depth = g_pipeline.depth.load();
    if (depth == 0) {
        *out_is_quiescent = 0;
        return -1;
    }
    // Quiescent: all slots Empty или ReadableAsTail (no Dispatched/FenceCompleted
    // = no in-flight compute). Per К-L18 spec verbatim: «pipeline slots quiescent
    // (all fences completed); no concurrent compute dispatches in-flight».
    for (int32_t i = 0; i < depth; ++i) {
        const int32_t state = g_pipeline.slots[i].state;
        if (state == SlotState_Dispatched || state == SlotState_FenceCompleted) {
            *out_is_quiescent = 0;
            return 1;
        }
    }
    *out_is_quiescent = 1;
    return 1;
}

}  // extern "C"
