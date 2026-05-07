// Native self-test: exercises the C ABI end-to-end so the library can be
// validated on a machine without a .NET SDK. Prints pass/fail per scenario
// and returns a non-zero exit code on failure.

#include <chrono>
#include <cstdint>
#include <cstdio>
#include <cstring>
#include <vector>

#include "df_capi.h"

namespace {

int g_failures = 0;

#define DF_CHECK(cond, msg)                                                 \
    do {                                                                    \
        if (!(cond)) {                                                      \
            std::printf("  FAIL: %s (line %d)\n", (msg), __LINE__);         \
            ++g_failures;                                                   \
        }                                                                   \
    } while (0)

struct BenchHealth {
    int32_t current;
    int32_t maximum;
};

constexpr uint32_t kHealthTypeId = 0xC0FFEE01u;

void scenario_basic_crud() {
    std::printf("scenario_basic_crud\n");
    df_world_handle w = df_world_create();
    DF_CHECK(w != nullptr, "world created");

    uint64_t e = df_world_create_entity(w);
    DF_CHECK(e != 0, "entity id non-zero");
    DF_CHECK(df_world_is_alive(w, e) == 1, "entity alive after create");

    BenchHealth h{42, 100};
    df_world_add_component(w, e, kHealthTypeId, &h, sizeof(h));
    DF_CHECK(df_world_has_component(w, e, kHealthTypeId) == 1,
             "has component after add");
    DF_CHECK(df_world_component_count(w, kHealthTypeId) == 1,
             "component count == 1");

    BenchHealth read{};
    DF_CHECK(df_world_get_component(w, e, kHealthTypeId, &read,
                                    sizeof(read)) == 1,
             "get component succeeds");
    DF_CHECK(read.current == 42 && read.maximum == 100,
             "round-trip values match");

    df_world_remove_component(w, e, kHealthTypeId);
    DF_CHECK(df_world_has_component(w, e, kHealthTypeId) == 0,
             "has=0 after remove");
    DF_CHECK(df_world_component_count(w, kHealthTypeId) == 0,
             "count=0 after remove");

    df_world_destroy(w);
}

void scenario_deferred_destroy() {
    std::printf("scenario_deferred_destroy\n");
    df_world_handle w = df_world_create();
    uint64_t e = df_world_create_entity(w);
    BenchHealth h{10, 10};
    df_world_add_component(w, e, kHealthTypeId, &h, sizeof(h));

    df_world_destroy_entity(w, e);
    DF_CHECK(df_world_is_alive(w, e) == 0,
             "entity not alive immediately after destroy");
    // Before flush, component is still physically present in the store.
    DF_CHECK(df_world_component_count(w, kHealthTypeId) == 1,
             "component still stored pre-flush (deferred removal)");

    df_world_flush_destroyed(w);
    DF_CHECK(df_world_component_count(w, kHealthTypeId) == 0,
             "component removed post-flush");

    // Slot is recycled on the next create; version must bump so the old
    // handle stays stale.
    uint64_t e2 = df_world_create_entity(w);
    DF_CHECK(df_world_is_alive(w, e2) == 1, "recycled slot is alive");
    DF_CHECK(df_world_is_alive(w, e) == 0,
             "stale handle stays dead after recycle");

    df_world_destroy(w);
}

void scenario_sparse_set_swap_remove() {
    std::printf("scenario_sparse_set_swap_remove\n");
    df_world_handle w = df_world_create();

    constexpr int N = 64;
    std::vector<uint64_t> ids(N);
    for (int i = 0; i < N; ++i) {
        ids[i] = df_world_create_entity(w);
        BenchHealth h{i, 1000};
        df_world_add_component(w, ids[i], kHealthTypeId, &h, sizeof(h));
    }

    DF_CHECK(df_world_component_count(w, kHealthTypeId) == N,
             "count == N after bulk add");

    // Remove every other entity. Each remove triggers the swap-with-last
    // path; the remaining entities must still read back the original values.
    for (int i = 0; i < N; i += 2) {
        df_world_remove_component(w, ids[i], kHealthTypeId);
    }
    DF_CHECK(df_world_component_count(w, kHealthTypeId) == N / 2,
             "count == N/2 after alternating removes");

    for (int i = 1; i < N; i += 2) {
        BenchHealth read{};
        const int ok =
            df_world_get_component(w, ids[i], kHealthTypeId, &read, sizeof(read));
        DF_CHECK(ok == 1, "survivor has component");
        DF_CHECK(read.current == i && read.maximum == 1000,
                 "survivor payload intact");
    }

    df_world_destroy(w);
}

void scenario_bulk_operations() {
    std::printf("scenario_bulk_operations\n");
    df_world_handle w = df_world_create();

    // Create 100 entities and corresponding components.
    constexpr int kCount = 100;
    uint64_t entities[kCount];
    BenchHealth components[kCount];
    for (int i = 0; i < kCount; ++i) {
        entities[i] = df_world_create_entity(w);
        components[i] = BenchHealth{i * 10, 100};
    }

    // Bulk add — single C ABI call.
    df_world_add_components_bulk(w, entities, kHealthTypeId, components,
                                 sizeof(BenchHealth), kCount);

    DF_CHECK(df_world_component_count(w, kHealthTypeId) == kCount,
             "all bulk-added components present");

    // Bulk get.
    BenchHealth read_back[kCount];
    int32_t successful = df_world_get_components_bulk(
        w, entities, kHealthTypeId, read_back, sizeof(BenchHealth), kCount);

    DF_CHECK(successful == kCount, "bulk get returned all components");

    for (int i = 0; i < kCount; ++i) {
        DF_CHECK(read_back[i].current == i * 10,
                 "bulk get value matches bulk add");
    }

    // Bulk get on mixed alive/dead — destroy half, flush, then bulk get all.
    for (int i = 0; i < kCount; i += 2) {
        df_world_destroy_entity(w, entities[i]);
    }
    df_world_flush_destroyed(w);

    int32_t partial = df_world_get_components_bulk(
        w, entities, kHealthTypeId, read_back, sizeof(BenchHealth), kCount);

    DF_CHECK(partial == kCount / 2,
             "bulk get on mixed alive/dead returns alive count");

    df_world_destroy(w);
}

void scenario_span_lifetime() {
    std::printf("scenario_span_lifetime\n");
    df_world_handle w = df_world_create();

    // Setup: 10 entities each with a Health component.
    uint64_t entities[10];
    for (int i = 0; i < 10; ++i) {
        entities[i] = df_world_create_entity(w);
        BenchHealth h{i, 100};
        df_world_add_component(w, entities[i], kHealthTypeId, &h, sizeof(h));
    }

    // Acquire span.
    const void* dense_ptr = nullptr;
    const int32_t* indices_ptr = nullptr;
    int32_t span_count = 0;

    int32_t acq = df_world_acquire_span(w, kHealthTypeId, &dense_ptr,
                                        &indices_ptr, &span_count);

    DF_CHECK(acq == 1, "span acquisition succeeded");
    DF_CHECK(span_count == 10, "span count matches entity count");
    DF_CHECK(dense_ptr != nullptr, "dense pointer is non-null");
    DF_CHECK(indices_ptr != nullptr, "indices pointer is non-null");

    // Read through span (no further P/Invokes — direct memory access).
    const BenchHealth* dense = static_cast<const BenchHealth*>(dense_ptr);
    int sum = 0;
    for (int i = 0; i < span_count; ++i) {
        sum += dense[i].current;
    }
    DF_CHECK(sum == 45, "span iteration sum matches expected (0+1+...+9)");

    // Mutation while span active — must be silently rejected (capi catches).
    uint64_t e_extra = df_world_create_entity(w);  // create itself is allowed
    BenchHealth h_extra{999, 100};
    df_world_add_component(w, e_extra, kHealthTypeId, &h_extra,
                           sizeof(h_extra));

    DF_CHECK(df_world_component_count(w, kHealthTypeId) == 10,
             "mutation rejected while span active");

    // Release span.
    df_world_release_span(w, kHealthTypeId);

    // Now mutation should succeed.
    df_world_add_component(w, e_extra, kHealthTypeId, &h_extra,
                           sizeof(h_extra));
    DF_CHECK(df_world_component_count(w, kHealthTypeId) == 11,
             "mutation succeeds after span released");

    df_world_destroy(w);
}

void scenario_throughput() {
    std::printf("scenario_throughput\n");
    constexpr int N = 100000;
    df_world_handle w = df_world_create();

    std::vector<uint64_t> ids;
    ids.reserve(N);

    auto t0 = std::chrono::steady_clock::now();
    for (int i = 0; i < N; ++i) {
        ids.push_back(df_world_create_entity(w));
    }
    auto t1 = std::chrono::steady_clock::now();

    for (int i = 0; i < N; ++i) {
        BenchHealth h{i, 100};
        df_world_add_component(w, ids[i], kHealthTypeId, &h, sizeof(h));
    }
    auto t2 = std::chrono::steady_clock::now();

    int64_t sum = 0;
    for (int i = 0; i < N; ++i) {
        BenchHealth read{};
        df_world_get_component(w, ids[i], kHealthTypeId, &read, sizeof(read));
        sum += read.current;
    }
    auto t3 = std::chrono::steady_clock::now();

    using ms = std::chrono::duration<double, std::milli>;
    std::printf("  create %d entities:  %.3f ms\n", N, ms(t1 - t0).count());
    std::printf("  add    %d components: %.3f ms\n", N, ms(t2 - t1).count());
    std::printf("  get    %d components: %.3f ms\n", N, ms(t3 - t2).count());
    // Anchor the result so the optimiser cannot discard the read loop.
    std::printf("  checksum: %lld\n", static_cast<long long>(sum));

    df_world_destroy(w);
}

} // namespace

int main() {
    std::printf("DualFrontier.Core.Native self-test\n");
    scenario_basic_crud();
    scenario_deferred_destroy();
    scenario_sparse_set_swap_remove();
    scenario_bulk_operations();
    scenario_span_lifetime();
    scenario_throughput();
    if (g_failures == 0) {
        std::printf("ALL PASSED\n");
        return 0;
    }
    std::printf("%d FAILURE(S)\n", g_failures);
    return 1;
}
