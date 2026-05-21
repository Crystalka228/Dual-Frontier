// Native self-test: exercises the C ABI end-to-end so the library can be
// validated on a machine without a .NET SDK. Prints pass/fail per scenario
// and returns a non-zero exit code on failure.
//
// K10.1 test framework decision (per Lesson #22 — read existing code first):
// The K3-era custom DF_CHECK macro runner established here is the canonical
// native test pattern. K10.1 scheduler scenarios extend this same selftest
// rather than introducing catch2/gtest dependencies. Each new scheduler
// primitive (system_graph, wake_registry, etc.) contributes its scenarios
// to this file, called from main().
//
// K10.2 extension (native bus + mod ALC lifecycle native primitives):
// Continues the same DF_CHECK runner convention. Each К10.2 item (event type
// registry, native bus three-tier dispatcher, background queue, native unload
// primitive, etc.) contributes scenarios to this file in its dedicated commit.
// Per Lesson #22 «match existing convention» — no new test framework introduced.

#include <atomic>
#include <chrono>
#include <cstdint>
#include <cstdio>
#include <cstring>
#include <stdexcept>
#include <thread>
#include <vector>

#include "background_queue.h"
#include "bootstrap_graph.h"
#include "bus_native.h"
#include "df_capi.h"
#include "event_type_registry.h"
#include "mod_unload.h"
#include "phase_compute.h"
#include "pipeline_slot.h"
#include "thread_pool.h"

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
constexpr uint32_t kBatchTypeId  = 0xBA7CF001u;  // K5 batch scenarios

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

void scenario_explicit_registration() {
    std::printf("scenario_explicit_registration\n");
    df_world_handle w = df_world_create();

    constexpr uint32_t kCustomTypeId = 42;
    constexpr int32_t kSize = sizeof(BenchHealth);

    // First registration succeeds.
    int32_t reg1 = df_world_register_component_type(w, kCustomTypeId, kSize);
    DF_CHECK(reg1 == 1, "first registration succeeded");

    // Idempotent — re-register same (id, size).
    int32_t reg2 = df_world_register_component_type(w, kCustomTypeId, kSize);
    DF_CHECK(reg2 == 1, "idempotent re-registration succeeded");

    // Conflict — re-register with different size fails (boundary swallows
    // the std::invalid_argument and returns 0).
    int32_t reg3 = df_world_register_component_type(w, kCustomTypeId, kSize * 2);
    DF_CHECK(reg3 == 0, "size conflict rejected");

    // type_id 0 reserved.
    int32_t reg4 = df_world_register_component_type(w, 0, kSize);
    DF_CHECK(reg4 == 0, "type_id 0 rejected");

    // Pre-registered type usable for Add/Get.
    uint64_t e = df_world_create_entity(w);
    BenchHealth h{50, 100};
    df_world_add_component(w, e, kCustomTypeId, &h, kSize);

    BenchHealth h_read{};
    int32_t got = df_world_get_component(w, e, kCustomTypeId, &h_read, kSize);
    DF_CHECK(got == 1, "get from pre-registered type succeeded");
    DF_CHECK(h_read.current == 50, "value preserved through pre-registered type");

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

void scenario_bootstrap_basic() {
    std::printf("scenario_bootstrap_basic\n");
    df_world_handle w = df_engine_bootstrap();
    DF_CHECK(w != nullptr, "bootstrap returned valid handle");

    uint64_t e = df_world_create_entity(w);
    DF_CHECK(df_world_is_alive(w, e) == 1, "entity created post-bootstrap");

    df_world_destroy(w);
}

void scenario_bootstrap_double_rejected() {
    std::printf("scenario_bootstrap_double_rejected\n");
    df_world_handle w1 = df_engine_bootstrap();
    DF_CHECK(w1 != nullptr, "first bootstrap succeeded");

    // Second bootstrap creates a second independent engine — both valid.
    // (Multiple engines supported per docstring.)
    df_world_handle w2 = df_engine_bootstrap();
    DF_CHECK(w2 != nullptr, "second bootstrap creates independent engine");
    DF_CHECK(w1 != w2, "engines are distinct");

    df_world_destroy(w1);
    df_world_destroy(w2);
}

void scenario_bootstrap_graph_topological() {
    std::printf("scenario_bootstrap_graph_topological\n");
    using namespace dualfrontier;

    BootstrapGraph graph;
    std::atomic<int> counter{0};
    std::atomic<int> task_a_order{-1};
    std::atomic<int> task_b_order{-1};
    std::atomic<int> task_c_order{-1};

    graph.add_task("A", {},
        [&]() { task_a_order.store(counter.fetch_add(1)); },
        []() {});
    graph.add_task("B", {"A"},
        [&]() { task_b_order.store(counter.fetch_add(1)); },
        []() {});
    graph.add_task("C", {"B"},
        [&]() { task_c_order.store(counter.fetch_add(1)); },
        []() {});

    ThreadPool pool(2);
    bool ok = graph.run(pool);
    pool.shutdown();

    DF_CHECK(ok, "linear graph executed successfully");
    DF_CHECK(task_a_order.load() < task_b_order.load(),
             "A executed before B");
    DF_CHECK(task_b_order.load() < task_c_order.load(),
             "B executed before C");
}

void scenario_bootstrap_graph_parallel() {
    std::printf("scenario_bootstrap_graph_parallel\n");
    using namespace dualfrontier;

    // Diamond: A -> (B || C) -> D
    // B and C should execute concurrently (overlapping wall-clock time).
    BootstrapGraph graph;
    std::atomic<bool> b_started{false};
    std::atomic<bool> c_started{false};
    std::atomic<bool> b_saw_c_started{false};
    std::atomic<bool> c_saw_b_started{false};

    graph.add_task("A", {},
        []() {},
        []() {});
    graph.add_task("B", {"A"},
        [&]() {
            b_started.store(true);
            std::this_thread::sleep_for(std::chrono::milliseconds(20));
            b_saw_c_started.store(c_started.load());
        },
        []() {});
    graph.add_task("C", {"A"},
        [&]() {
            c_started.store(true);
            std::this_thread::sleep_for(std::chrono::milliseconds(20));
            c_saw_b_started.store(b_started.load());
        },
        []() {});
    graph.add_task("D", {"B", "C"},
        []() {},
        []() {});

    ThreadPool pool(4);
    bool ok = graph.run(pool);
    pool.shutdown();

    DF_CHECK(ok, "diamond graph executed successfully");
    DF_CHECK(b_saw_c_started.load() || c_saw_b_started.load(),
             "B and C executed concurrently (parallelism evidence)");
}

// =============================================================================
// K5 Command Buffer scenarios (added 2026-05-08).
// =============================================================================

void scenario_batch_basic() {
    std::printf("scenario_batch_basic\n");
    df_world_handle w = df_world_create();

    uint64_t e1 = df_world_create_entity(w);
    uint64_t e2 = df_world_create_entity(w);

    int32_t initial_a = 10;
    int32_t initial_b = 20;
    df_world_add_component(w, e1, kBatchTypeId, &initial_a, sizeof(int32_t));
    df_world_add_component(w, e2, kBatchTypeId, &initial_b, sizeof(int32_t));

    df_batch_handle batch =
        df_world_begin_batch(w, kBatchTypeId, sizeof(int32_t));
    DF_CHECK(batch != nullptr, "batch handle non-null");

    int32_t new_a = 100;
    int32_t new_b = 200;
    DF_CHECK(df_batch_record_update(batch, e1, &new_a) == 1,
             "record_update e1 succeeded");
    DF_CHECK(df_batch_record_update(batch, e2, &new_b) == 1,
             "record_update e2 succeeded");

    int32_t applied = df_batch_flush(batch);
    DF_CHECK(applied == 2, "flush applied 2 commands");

    df_batch_destroy(batch);

    int32_t out_a = 0;
    int32_t out_b = 0;
    DF_CHECK(df_world_get_component(w, e1, kBatchTypeId, &out_a,
                                    sizeof(int32_t)) == 1,
             "e1 component readable post-flush");
    DF_CHECK(df_world_get_component(w, e2, kBatchTypeId, &out_b,
                                    sizeof(int32_t)) == 1,
             "e2 component readable post-flush");
    DF_CHECK(out_a == 100 && out_b == 200, "values updated atomically");

    df_world_destroy(w);
}

void scenario_batch_mixed_commands() {
    std::printf("scenario_batch_mixed_commands\n");
    df_world_handle w = df_world_create();

    uint64_t e1 = df_world_create_entity(w);
    uint64_t e2 = df_world_create_entity(w);
    uint64_t e3 = df_world_create_entity(w);

    int32_t v = 42;
    df_world_add_component(w, e1, kBatchTypeId, &v, sizeof(int32_t));
    df_world_add_component(w, e3, kBatchTypeId, &v, sizeof(int32_t));
    // e2 has no component — will be Added via batch.

    df_batch_handle batch =
        df_world_begin_batch(w, kBatchTypeId, sizeof(int32_t));

    int32_t new_v = 99;
    DF_CHECK(df_batch_record_update(batch, e1, &new_v) == 1, "update e1");
    DF_CHECK(df_batch_record_add(batch, e2, &new_v) == 1, "add e2");
    DF_CHECK(df_batch_record_remove(batch, e3) == 1, "remove e3");

    int32_t applied = df_batch_flush(batch);
    DF_CHECK(applied == 3, "all 3 mixed commands applied");

    df_batch_destroy(batch);

    int32_t out = 0;
    DF_CHECK(df_world_get_component(w, e1, kBatchTypeId, &out,
                                    sizeof(int32_t)) == 1 && out == 99,
             "e1 updated to 99");
    DF_CHECK(df_world_get_component(w, e2, kBatchTypeId, &out,
                                    sizeof(int32_t)) == 1 && out == 99,
             "e2 added with value 99");
    DF_CHECK(df_world_has_component(w, e3, kBatchTypeId) == 0,
             "e3 component removed");

    df_world_destroy(w);
}

void scenario_batch_cancel() {
    std::printf("scenario_batch_cancel\n");
    df_world_handle w = df_world_create();

    uint64_t e = df_world_create_entity(w);
    int32_t initial = 10;
    df_world_add_component(w, e, kBatchTypeId, &initial, sizeof(int32_t));

    df_batch_handle batch =
        df_world_begin_batch(w, kBatchTypeId, sizeof(int32_t));

    int32_t modified = 999;
    df_batch_record_update(batch, e, &modified);
    df_batch_cancel(batch);

    // Subsequent destroy is no-op for commands; counter still decremented.
    df_batch_destroy(batch);

    int32_t out = 0;
    df_world_get_component(w, e, kBatchTypeId, &out, sizeof(int32_t));
    DF_CHECK(out == 10, "value unchanged after cancel");

    df_world_destroy(w);
}

void scenario_batch_dead_entity_skipped() {
    std::printf("scenario_batch_dead_entity_skipped\n");
    df_world_handle w = df_world_create();

    uint64_t alive = df_world_create_entity(w);
    uint64_t soon_dead = df_world_create_entity(w);

    int32_t v = 1;
    df_world_add_component(w, alive, kBatchTypeId, &v, sizeof(int32_t));
    df_world_add_component(w, soon_dead, kBatchTypeId, &v, sizeof(int32_t));

    // Capture stale handle before destroy.
    uint64_t stale = soon_dead;
    df_world_destroy_entity(w, soon_dead);
    df_world_flush_destroyed(w);

    df_batch_handle batch =
        df_world_begin_batch(w, kBatchTypeId, sizeof(int32_t));

    int32_t new_v = 100;
    DF_CHECK(df_batch_record_update(batch, alive, &new_v) == 1,
             "record alive entity");
    DF_CHECK(df_batch_record_update(batch, stale, &new_v) == 1,
             "record (stale) destroyed entity — accepted at record, skipped at flush");

    int32_t applied = df_batch_flush(batch);
    DF_CHECK(applied == 1, "only the live entity's command applied");

    df_batch_destroy(batch);

    int32_t out = 0;
    df_world_get_component(w, alive, kBatchTypeId, &out, sizeof(int32_t));
    DF_CHECK(out == 100, "alive entity updated");

    df_world_destroy(w);
}

void scenario_batch_mutation_rejection() {
    std::printf("scenario_batch_mutation_rejection\n");
    df_world_handle w = df_world_create();

    uint64_t e = df_world_create_entity(w);
    int32_t v = 10;
    df_world_add_component(w, e, kBatchTypeId, &v, sizeof(int32_t));

    df_batch_handle batch =
        df_world_begin_batch(w, kBatchTypeId, sizeof(int32_t));

    // While batch is active, direct mutations are rejected. The C ABI
    // catches the std::logic_error and silently returns; the value remains
    // unchanged.
    int32_t direct_v = 999;
    df_world_add_component(w, e, kBatchTypeId, &direct_v, sizeof(int32_t));

    int32_t out = 0;
    df_world_get_component(w, e, kBatchTypeId, &out, sizeof(int32_t));
    DF_CHECK(out == 10, "direct mutation rejected while batch active");

    df_batch_cancel(batch);
    df_batch_destroy(batch);

    // After batch destruction, mutation succeeds.
    df_world_add_component(w, e, kBatchTypeId, &direct_v, sizeof(int32_t));
    df_world_get_component(w, e, kBatchTypeId, &out, sizeof(int32_t));
    DF_CHECK(out == 999, "direct mutation succeeds after batch destroyed");

    df_world_destroy(w);
}

void scenario_bootstrap_rollback_on_failure() {
    std::printf("scenario_bootstrap_rollback_on_failure\n");
    using namespace dualfrontier;

    BootstrapGraph graph;
    std::atomic<int> a_cleanup_count{0};
    std::atomic<int> b_cleanup_count{0};

    // A succeeds, B fails — A's cleanup must be invoked.
    graph.add_task("A", {},
        []() { /* succeeds */ },
        [&]() { a_cleanup_count.fetch_add(1); });
    graph.add_task("B", {"A"},
        []() { throw std::runtime_error("intentional failure for test"); },
        [&]() { b_cleanup_count.fetch_add(1); });

    ThreadPool pool(2);
    bool ok = graph.run(pool);
    pool.shutdown();

    DF_CHECK(!ok, "graph reported failure");
    DF_CHECK(a_cleanup_count.load() == 1, "A's cleanup invoked exactly once");
    DF_CHECK(b_cleanup_count.load() == 0,
             "B's cleanup NOT invoked (B never completed)");
    DF_CHECK(!graph.last_failure().empty(), "failure message recorded");
}

// ---- K8.1 reference primitives selftest -----------------------------------

void scenario_string_pool() {
    std::printf("scenario_string_pool\n");
    df_world_handle w = df_world_create();
    DF_CHECK(w != nullptr, "world created");

    // 1. Round-trip: intern, resolve.
    const char foo[] = "Foo";
    uint32_t id_foo = df_world_intern_string(w, foo, sizeof(foo) - 1);
    DF_CHECK(id_foo != 0, "intern returned non-zero id");
    uint32_t gen_foo = df_world_string_generation(w, id_foo);
    DF_CHECK(gen_foo != 0, "generation non-zero after intern");

    char out[16] = {0};
    int32_t written = df_world_resolve_string(w, id_foo, gen_foo, out, sizeof(out));
    DF_CHECK(written == 3, "resolve wrote 3 bytes");
    DF_CHECK(std::memcmp(out, "Foo", 3) == 0, "resolved content matches");

    // 2. Dedup: intern same string returns same id.
    uint32_t id_foo2 = df_world_intern_string(w, foo, sizeof(foo) - 1);
    DF_CHECK(id_foo2 == id_foo, "second intern of same content returns same id");

    // 3. Cross-mod sharing: ModA interns Foo, ModB also interns Foo.
    df_world_begin_mod_scope(w, "ModA");
    uint32_t id_a_only = df_world_intern_string(w, "ModAExclusive", 13);
    uint32_t id_shared_a = df_world_intern_string(w, foo, 3);
    df_world_end_mod_scope(w, "ModA");
    DF_CHECK(id_shared_a == id_foo, "ModA's intern of Foo returns shared id");

    df_world_begin_mod_scope(w, "ModB");
    uint32_t id_shared_b = df_world_intern_string(w, foo, 3);
    uint32_t id_b_only = df_world_intern_string(w, "ModBExclusive", 13);
    df_world_end_mod_scope(w, "ModB");
    DF_CHECK(id_shared_b == id_foo, "ModB's intern of Foo returns shared id");
    DF_CHECK(id_b_only != id_foo && id_b_only != id_a_only,
             "ModB-exclusive id distinct from shared and from ModA-exclusive");

    // 4. Mod scope clear with retained reference.
    uint32_t gen_before_clear = df_world_string_pool_current_generation(w);
    df_world_clear_mod_scope(w, "ModA");
    // id_foo (shared with ModB and core) must still resolve at the SAME
    // generation tag — no reclaim happened on a co-owned id.
    DF_CHECK(df_world_string_generation(w, id_foo) == gen_foo,
             "shared id retains generation after mod-A clear");
    written = df_world_resolve_string(w, id_foo, gen_foo, out, sizeof(out));
    DF_CHECK(written == 3, "shared id still resolvable after mod-A clear");

    // ModA-exclusive id must be reclaimed; the SLOT may stay alive but its
    // generation must have advanced, so the old (id, gen) no longer resolves.
    uint32_t gen_a_after = df_world_string_generation(w, id_a_only);
    DF_CHECK(gen_a_after != gen_foo,
             "ModA-exclusive id generation advanced after clear");
    char tmp[32] = {0};
    int32_t reclaim_check = df_world_resolve_string(
        w, id_a_only, gen_foo, tmp, sizeof(tmp));
    DF_CHECK(reclaim_check == 0,
             "stale reference to reclaimed id resolves to not-found");

    // 5. Mod scope clear bumps current_generation when something is reclaimed.
    DF_CHECK(df_world_string_pool_current_generation(w) > gen_before_clear,
             "current_generation advanced after reclaim");

    // 6. Empty-string sentinel: intern returns 0, resolve returns 0 bytes,
    //    pool count unchanged. The sentinel path is hand-written in
    //    string_pool.cpp::intern; this scenario guards against accidental
    //    refactors of that branch.
    const int32_t count_before_empty = df_world_string_pool_count(w);
    uint32_t id_empty = df_world_intern_string(w, "", 0);
    DF_CHECK(id_empty == 0, "intern of empty content returns id 0 sentinel");
    DF_CHECK(df_world_string_pool_count(w) == count_before_empty,
             "string pool count unchanged after intern of empty content");

    char empty_buf[8] = {0};
    int32_t empty_written = df_world_resolve_string(
        w, /*id=*/0, /*generation=*/0, empty_buf, sizeof(empty_buf));
    DF_CHECK(empty_written == 0,
             "resolve of empty sentinel id returns 0 bytes (treated as not-found)");

    // Cross-check: also resolve with a non-zero generation (any value).
    // Empty sentinel id 0 is shaped as "always not-found" for resolve,
    // independent of the generation tag passed in.
    empty_written = df_world_resolve_string(
        w, /*id=*/0, /*generation=*/12345, empty_buf, sizeof(empty_buf));
    DF_CHECK(empty_written == 0,
             "resolve of empty sentinel id is generation-independent");

    df_world_destroy(w);
}

void scenario_keyed_map() {
    std::printf("scenario_keyed_map\n");
    df_world_handle w = df_world_create();
    DF_CHECK(w != nullptr, "world created");

    df_keyed_map_handle map = df_world_get_keyed_map(
        w, /*map_id=*/0xA1u, sizeof(uint32_t), sizeof(int32_t));
    DF_CHECK(map != nullptr, "map handle non-null");

    // 1. Round-trip.
    uint32_t k1 = 1, k2 = 2;
    int32_t v100 = 100, v200 = 200;
    DF_CHECK(df_keyed_map_set(map, &k1, &v100) == 1, "first set inserted");
    DF_CHECK(df_keyed_map_set(map, &k2, &v200) == 1, "second set inserted");
    int32_t out_v = 0;
    DF_CHECK(df_keyed_map_get(map, &k1, &out_v) == 1 && out_v == 100, "get k1");
    DF_CHECK(df_keyed_map_get(map, &k2, &out_v) == 1 && out_v == 200, "get k2");

    // 2. Sorted iteration.
    df_keyed_map_clear(map);
    uint32_t k5 = 5, k1b = 1, k3 = 3;
    int32_t a = 50, b = 10, c = 30;
    df_keyed_map_set(map, &k5, &a);
    df_keyed_map_set(map, &k1b, &b);
    df_keyed_map_set(map, &k3, &c);
    uint32_t keys_out[8] = {0};
    int32_t values_out[8] = {0};
    int32_t n = df_keyed_map_iterate(map, keys_out, values_out, 8);
    DF_CHECK(n == 3, "iterate count == 3");
    DF_CHECK(keys_out[0] == 1 && keys_out[1] == 3 && keys_out[2] == 5,
             "keys in sorted order");
    DF_CHECK(values_out[0] == 10 && values_out[1] == 30 && values_out[2] == 50,
             "values aligned with sorted keys");

    // 3. Update overwrites.
    int32_t v999 = 999;
    DF_CHECK(df_keyed_map_set(map, &k1b, &v999) == 0, "update returns 0");
    df_keyed_map_get(map, &k1b, &out_v);
    DF_CHECK(out_v == 999, "value overwritten");
    DF_CHECK(df_keyed_map_count(map) == 3, "count unchanged after update");

    // 4. Remove + iterate.
    DF_CHECK(df_keyed_map_remove(map, &k3) == 1, "remove k3");
    DF_CHECK(df_keyed_map_count(map) == 2, "count == 2 after remove");
    n = df_keyed_map_iterate(map, keys_out, values_out, 8);
    DF_CHECK(n == 2, "iterate count == 2");
    DF_CHECK(keys_out[0] == 1 && keys_out[1] == 5, "remaining keys still sorted");

    df_world_destroy(w);
}

void scenario_composite() {
    std::printf("scenario_composite\n");
    df_world_handle w = df_world_create();
    DF_CHECK(w != nullptr, "world created");

    df_composite_handle comp = df_world_get_composite(
        w, /*composite_id=*/0xC1u, sizeof(int32_t));
    DF_CHECK(comp != nullptr, "composite handle non-null");

    // 1. Round-trip.
    int32_t e1 = 11;
    DF_CHECK(df_composite_add(comp, /*parent=*/42, &e1) == 1, "add to entity 42");
    DF_CHECK(df_composite_get_count(comp, 42) == 1, "count for 42 == 1");
    int32_t out_e = 0;
    DF_CHECK(df_composite_get_at(comp, 42, 0, &out_e) == 1 && out_e == 11,
             "get_at 0 returns 11");

    // 2. Multi-element insertion order.
    int32_t e2 = 22, e3 = 33;
    df_composite_add(comp, 42, &e2);
    df_composite_add(comp, 42, &e3);
    int32_t buf[8] = {0};
    int32_t n = df_composite_iterate(comp, 42, buf, 8);
    DF_CHECK(n == 3, "iterate count == 3");
    DF_CHECK(buf[0] == 11 && buf[1] == 22 && buf[2] == 33,
             "iterate preserves insertion order");

    // 3. remove_at(0) swaps with last.
    DF_CHECK(df_composite_remove_at(comp, 42, 0) == 1, "remove_at 0");
    DF_CHECK(df_composite_get_count(comp, 42) == 2, "count == 2 after remove");
    n = df_composite_iterate(comp, 42, buf, 8);
    DF_CHECK(n == 2, "iterate count == 2 after remove");
    DF_CHECK(buf[0] == 33 && buf[1] == 22,
             "swap-with-last places former tail at removed index");

    // 4. Multi-entity isolation.
    int32_t e_other = 99;
    df_composite_add(comp, /*parent=*/99, &e_other);
    DF_CHECK(df_composite_get_count(comp, 99) == 1, "entity 99 has its own slot");
    DF_CHECK(df_composite_get_count(comp, 42) == 2, "entity 42 unaffected by entity 99");

    df_world_destroy(w);
}

void scenario_set_primitive() {
    std::printf("scenario_set_primitive\n");
    df_world_handle w = df_world_create();
    DF_CHECK(w != nullptr, "world created");

    df_set_handle set = df_world_get_set(
        w, /*set_id=*/0xD1u, sizeof(int32_t));
    DF_CHECK(set != nullptr, "set handle non-null");

    // 1. Round-trip + contains.
    int32_t five = 5, three = 3, seven = 7;
    DF_CHECK(df_set_add(set, &five) == 1, "add 5 inserted");
    DF_CHECK(df_set_add(set, &three) == 1, "add 3 inserted");
    DF_CHECK(df_set_contains(set, &five) == 1, "contains 5");
    DF_CHECK(df_set_contains(set, &three) == 1, "contains 3");
    DF_CHECK(df_set_contains(set, &seven) == 0, "does not contain 7");

    // 2. Dedup.
    DF_CHECK(df_set_add(set, &five) == 0, "duplicate add returns 0");
    DF_CHECK(df_set_count(set) == 2, "count unchanged after duplicate add");

    // 3. Sorted iteration.
    df_set_handle set2 = df_world_get_set(w, /*set_id=*/0xD2u, sizeof(int32_t));
    int32_t a5 = 5, a1 = 1, a3 = 3;
    df_set_add(set2, &a5);
    df_set_add(set2, &a1);
    df_set_add(set2, &a3);
    int32_t buf[8] = {0};
    int32_t n = df_set_iterate(set2, buf, 8);
    DF_CHECK(n == 3, "iterate count == 3");
    DF_CHECK(buf[0] == 1 && buf[1] == 3 && buf[2] == 5, "sorted iteration order");

    // 4. Remove + iterate.
    df_set_handle set3 = df_world_get_set(w, /*set_id=*/0xD3u, sizeof(int32_t));
    int32_t b1 = 1, b2 = 2, b3 = 3, b4 = 4;
    df_set_add(set3, &b1);
    df_set_add(set3, &b2);
    df_set_add(set3, &b3);
    df_set_add(set3, &b4);
    DF_CHECK(df_set_remove(set3, &b2) == 1, "remove 2 succeeded");
    n = df_set_iterate(set3, buf, 8);
    DF_CHECK(n == 3, "iterate count == 3 after remove");
    DF_CHECK(buf[0] == 1 && buf[1] == 3 && buf[2] == 4,
             "remaining elements still sorted");

    df_world_destroy(w);
}

// K9 — field storage scenarios.

void scenario_field_register_and_read() {
    std::printf("scenario_field_register_and_read\n");
    df_world_handle w = df_world_create();
    DF_CHECK(w != nullptr, "world created");

    DF_CHECK(df_world_register_field(w, "test.scalar", 10, 10, sizeof(float)) == 1,
             "register field test.scalar");

    float value = 99.0f;  // pre-set to detect lack of zero-init
    DF_CHECK(df_world_field_read_cell(w, "test.scalar", 5, 5, &value, sizeof(float)) == 1,
             "read_cell at (5,5) succeeds");
    DF_CHECK(value == 0.0f, "freshly registered field is zero-initialized");

    df_world_destroy(w);
}

void scenario_field_write_and_read_roundtrip() {
    std::printf("scenario_field_write_and_read_roundtrip\n");
    df_world_handle w = df_world_create();
    df_world_register_field(w, "test.roundtrip", 5, 5, sizeof(float));

    float in = 42.5f;
    DF_CHECK(df_world_field_write_cell(w, "test.roundtrip", 2, 3, &in, sizeof(float)) == 1,
             "write_cell at (2,3) succeeds");

    float out = 0.0f;
    df_world_field_read_cell(w, "test.roundtrip", 2, 3, &out, sizeof(float));
    DF_CHECK(out == 42.5f, "round-trip value preserved");

    df_world_destroy(w);
}

void scenario_field_span_lifecycle() {
    std::printf("scenario_field_span_lifecycle\n");
    df_world_handle w = df_world_create();
    df_world_register_field(w, "test.span", 4, 4, sizeof(float));

    const void* data = nullptr;
    int32_t fw = 0, fh = 0;
    DF_CHECK(df_world_field_acquire_span(w, "test.span", &data, &fw, &fh) == 1,
             "acquire_span succeeds");
    DF_CHECK(fw == 4 && fh == 4, "span dimensions match");
    DF_CHECK(data != nullptr, "span data pointer non-null");

    // Mutation must reject during active span.
    float x = 1.0f;
    DF_CHECK(df_world_field_write_cell(w, "test.span", 0, 0, &x, sizeof(float)) == 0,
             "write_cell rejected during active span");

    DF_CHECK(df_world_field_release_span(w, "test.span") == 1,
             "release_span succeeds");

    // After release, mutation succeeds.
    DF_CHECK(df_world_field_write_cell(w, "test.span", 0, 0, &x, sizeof(float)) == 1,
             "write_cell succeeds after span released");

    df_world_destroy(w);
}

void scenario_field_conductivity_default_and_set() {
    std::printf("scenario_field_conductivity_default_and_set\n");
    df_world_handle w = df_world_create();
    df_world_register_field(w, "test.cond", 3, 3, sizeof(float));

    DF_CHECK(df_world_field_get_conductivity(w, "test.cond", 1, 1) == 1.0f,
             "default conductivity == 1.0");

    DF_CHECK(df_world_field_set_conductivity(w, "test.cond", 1, 1, 0.5f) == 1,
             "set_conductivity succeeds");
    DF_CHECK(df_world_field_get_conductivity(w, "test.cond", 1, 1) == 0.5f,
             "conductivity reads back at 0.5");

    df_world_destroy(w);
}

void scenario_field_storage_flag_toggle() {
    std::printf("scenario_field_storage_flag_toggle\n");
    df_world_handle w = df_world_create();
    df_world_register_field(w, "test.stor", 3, 3, sizeof(float));

    DF_CHECK(df_world_field_get_storage_flag(w, "test.stor", 1, 1) == 0,
             "default storage flag == 0");

    df_world_field_set_storage_flag(w, "test.stor", 1, 1, 1);
    DF_CHECK(df_world_field_get_storage_flag(w, "test.stor", 1, 1) == 1,
             "storage flag reads back as 1");

    df_world_field_set_storage_flag(w, "test.stor", 1, 1, 0);
    DF_CHECK(df_world_field_get_storage_flag(w, "test.stor", 1, 1) == 0,
             "storage flag reads back as 0 after clear");

    df_world_destroy(w);
}

void scenario_field_swap_buffers() {
    std::printf("scenario_field_swap_buffers\n");
    df_world_handle w = df_world_create();
    df_world_register_field(w, "test.swap", 2, 2, sizeof(float));

    float a = 1.0f, b = 2.0f, c = 3.0f, d = 4.0f;
    df_world_field_write_cell(w, "test.swap", 0, 0, &a, sizeof(float));
    df_world_field_write_cell(w, "test.swap", 1, 0, &b, sizeof(float));
    df_world_field_write_cell(w, "test.swap", 0, 1, &c, sizeof(float));
    df_world_field_write_cell(w, "test.swap", 1, 1, &d, sizeof(float));

    DF_CHECK(df_world_field_swap_buffers(w, "test.swap") == 1, "swap_buffers succeeds");

    // After swap, primary buffer is the back buffer (zero-initialized).
    float check = 99.0f;
    df_world_field_read_cell(w, "test.swap", 0, 0, &check, sizeof(float));
    DF_CHECK(check == 0.0f, "post-swap primary at (0,0) == 0 (back buffer)");
    df_world_field_read_cell(w, "test.swap", 1, 1, &check, sizeof(float));
    DF_CHECK(check == 0.0f, "post-swap primary at (1,1) == 0 (back buffer)");

    df_world_destroy(w);
}

void scenario_field_register_idempotent_and_conflict() {
    std::printf("scenario_field_register_idempotent_and_conflict\n");
    df_world_handle w = df_world_create();

    DF_CHECK(df_world_register_field(w, "test.idem", 5, 5, 4) == 1, "initial register");
    DF_CHECK(df_world_register_field(w, "test.idem", 5, 5, 4) == 1,
             "re-register with same dims is idempotent");
    DF_CHECK(df_world_register_field(w, "test.idem", 6, 6, 4) == 0,
             "re-register with different dims rejected");

    df_world_destroy(w);
}

void scenario_field_unregister() {
    std::printf("scenario_field_unregister\n");
    df_world_handle w = df_world_create();
    df_world_register_field(w, "test.unreg", 3, 3, 4);

    DF_CHECK(df_world_field_count(w) == 1, "field_count == 1 after register");
    DF_CHECK(df_world_field_unregister(w, "test.unreg") == 1, "unregister succeeds");
    DF_CHECK(df_world_field_count(w) == 0, "field_count == 0 after unregister");

    // Read on unregistered field fails cleanly.
    float v = 0.0f;
    DF_CHECK(df_world_field_read_cell(w, "test.unreg", 0, 0, &v, sizeof(float)) == 0,
             "read_cell on unregistered field returns 0");

    df_world_destroy(w);
}

// =============================================================================
// K10.1 scheduler scenarios. Native scheduler primitives extend the existing
// K3-era selftest pattern rather than adopting catch2/gtest (per Item 24
// decision + Lesson #22). Each primitive adds its scenario_* functions here.
// =============================================================================

void scenario_k10_smoke() {
    std::printf("scenario_k10_smoke\n");
    // K10.1 framework smoke: verifies the selftest runner hosts scheduler
    // scope correctly. Real scheduler scenarios in scenario_system_graph_*
    // and forward (added per K10.1 commits 3-13).
    constexpr uint32_t k10_marker = 0x4B313031u;  // ASCII 'K101'
    DF_CHECK(k10_marker == 0x4B313031u, "K10.1 scheduler scope marker resolves");
}

// =============================================================================
// K10.1 Item 1 — system graph scenarios.
// =============================================================================

void scenario_system_graph_basic_register() {
    std::printf("scenario_system_graph_basic_register\n");
    df_scheduler_clear();
    DF_CHECK(df_scheduler_system_count() == 0, "empty after clear");

    uint32_t reads_a[] = {100};
    uint32_t writes_a[] = {101};
    int32_t ok = df_scheduler_register_system(
        1, "SystemA", reads_a, 1, writes_a, 1, /*priority*/2, /*wake*/0);
    DF_CHECK(ok == 1, "register SystemA");
    DF_CHECK(df_scheduler_system_count() == 1, "count=1 after first register");

    // Duplicate id rejected.
    int32_t dup = df_scheduler_register_system(
        1, "SystemA", nullptr, 0, nullptr, 0, 2, 0);
    DF_CHECK(dup == 0, "duplicate id rejected");

    // Empty fqn rejected.
    int32_t empty = df_scheduler_register_system(
        2, "", nullptr, 0, nullptr, 0, 2, 0);
    DF_CHECK(empty == 0, "empty fqn rejected");

    DF_CHECK(df_scheduler_unregister_system(1) == 1, "unregister SystemA");
    DF_CHECK(df_scheduler_system_count() == 0, "count=0 after unregister");
    DF_CHECK(df_scheduler_unregister_system(1) == 0, "unregister unknown id");
}

void scenario_system_graph_linear_chain() {
    std::printf("scenario_system_graph_linear_chain\n");
    df_scheduler_clear();
    // A writes X; B reads X writes Y; C reads Y.  Expect A → B → C in 3 phases.
    constexpr uint32_t cX = 100, cY = 101;
    uint32_t a_writes[] = {cX};
    uint32_t b_reads[]  = {cX};
    uint32_t b_writes[] = {cY};
    uint32_t c_reads[]  = {cY};

    df_scheduler_register_system(1, "A", nullptr, 0, a_writes, 1, 2, 0);
    df_scheduler_register_system(2, "B", b_reads, 1, b_writes, 1, 2, 0);
    df_scheduler_register_system(3, "C", c_reads, 1, nullptr, 0, 2, 0);

    int32_t result = df_scheduler_compute_static_graph();
    DF_CHECK(result == 1, "compute success on linear chain");
    DF_CHECK(df_scheduler_static_phase_count() == 3, "linear chain produces 3 phases");

    uint32_t buf[4] = {0};
    DF_CHECK(df_scheduler_static_phase_size(0) == 1, "phase 0 size=1");
    df_scheduler_static_phase_systems(0, buf, 4);
    DF_CHECK(buf[0] == 1, "phase 0 is A");
    df_scheduler_static_phase_systems(1, buf, 4);
    DF_CHECK(buf[0] == 2, "phase 1 is B");
    df_scheduler_static_phase_systems(2, buf, 4);
    DF_CHECK(buf[0] == 3, "phase 2 is C");
}

void scenario_system_graph_parallel_layer() {
    std::printf("scenario_system_graph_parallel_layer\n");
    df_scheduler_clear();
    // Two independent systems → both in phase 0 (no edges between them).
    df_scheduler_register_system(1, "X", nullptr, 0, nullptr, 0, 2, 0);
    df_scheduler_register_system(2, "Y", nullptr, 0, nullptr, 0, 2, 0);
    int32_t r = df_scheduler_compute_static_graph();
    DF_CHECK(r == 1, "compute success on independent pair");
    DF_CHECK(df_scheduler_static_phase_count() == 1, "single phase");
    DF_CHECK(df_scheduler_static_phase_size(0) == 2, "phase contains both systems");
    uint32_t buf[4] = {0};
    df_scheduler_static_phase_systems(0, buf, 4);
    DF_CHECK(buf[0] == 1 && buf[1] == 2, "sorted phase ids");
}

void scenario_system_graph_write_conflict() {
    std::printf("scenario_system_graph_write_conflict\n");
    df_scheduler_clear();
    constexpr uint32_t cX = 100;
    uint32_t writes[] = {cX};
    df_scheduler_register_system(1, "A", nullptr, 0, writes, 1, 2, 0);
    df_scheduler_register_system(2, "B", nullptr, 0, writes, 1, 2, 0);
    int32_t r = df_scheduler_compute_static_graph();
    DF_CHECK(r == -1, "write-write conflict detected (-1)");
    DF_CHECK(df_scheduler_static_phase_count() == 0, "no phases produced on conflict");
}

void scenario_system_graph_cycle_detection() {
    std::printf("scenario_system_graph_cycle_detection\n");
    df_scheduler_clear();
    constexpr uint32_t cX = 100, cY = 101;
    // A: reads Y, writes X. B: reads X, writes Y. Cycle A↔B.
    uint32_t a_r[] = {cY};
    uint32_t a_w[] = {cX};
    uint32_t b_r[] = {cX};
    uint32_t b_w[] = {cY};
    df_scheduler_register_system(1, "A", a_r, 1, a_w, 1, 2, 0);
    df_scheduler_register_system(2, "B", b_r, 1, b_w, 1, 2, 0);
    int32_t r = df_scheduler_compute_static_graph();
    DF_CHECK(r == -2, "cycle detected (-2)");
}

// =============================================================================
// K10.1 Item 2 — thread pool extension scenarios.
// =============================================================================

// =============================================================================
// K10.1 Item 3 — wake registry scenarios.
// =============================================================================

void scenario_wake_registry_subscription_lifecycle() {
    std::printf("scenario_wake_registry_subscription_lifecycle\n");
    df_wake_registry_clear();
    DF_CHECK(df_wake_registry_subscription_count(0) == 0, "no timer subs initially");

    DF_CHECK(df_wake_registry_subscribe_timer(1, 15) == 1, "subscribe timer(1, 15)");
    DF_CHECK(df_wake_registry_subscribe_timer(1, 0) == 0, "rate=0 rejected");
    DF_CHECK(df_wake_registry_subscription_count(0) == 1, "one timer sub");

    DF_CHECK(df_wake_registry_subscribe_event(2, 0xA1) == 1, "subscribe event(2, 0xA1)");
    DF_CHECK(df_wake_registry_subscribe_state(3, 0xC0) == 1, "subscribe state(3, 0xC0)");
    DF_CHECK(df_wake_registry_subscribe_init(4) == 1, "subscribe init(4)");
    DF_CHECK(df_wake_registry_subscribe_explicit(5, 0xE0) == 1, "subscribe explicit(5, 0xE0)");

    DF_CHECK(df_wake_registry_subscription_count(0) == 1, "timer subs");
    DF_CHECK(df_wake_registry_subscription_count(1) == 1, "event subs");
    DF_CHECK(df_wake_registry_subscription_count(2) == 1, "state subs");
    DF_CHECK(df_wake_registry_subscription_count(3) == 1, "init subs");
    DF_CHECK(df_wake_registry_subscription_count(4) == 1, "explicit subs");

    DF_CHECK(df_wake_registry_unsubscribe(1, 0) == 1, "unsubscribe timer");
    DF_CHECK(df_wake_registry_subscription_count(0) == 0, "timer subs now 0");
    DF_CHECK(df_wake_registry_unsubscribe(99, 0) == 0, "unsubscribe unknown");
}

void scenario_wake_registry_fire_timer() {
    std::printf("scenario_wake_registry_fire_timer\n");
    df_wake_registry_clear();
    df_wake_registry_subscribe_timer(/*sys*/10, /*rate*/1);   // every tick
    df_wake_registry_subscribe_timer(/*sys*/20, /*rate*/3);   // every 3rd tick
    df_wake_registry_subscribe_timer(/*sys*/30, /*rate*/60);  // every 60th

    // Tick 0: all three fire (0 % anything == 0).
    DF_CHECK(df_wake_registry_fire_timer(0) == 3, "tick 0 wakes all three");
    uint32_t buf[8] = {0};
    int32_t drained = df_wake_registry_drain_runqueue(buf, 8);
    DF_CHECK(drained == 3, "runqueue drains 3 ids");
    DF_CHECK(buf[0] == 10 && buf[1] == 20 && buf[2] == 30, "ids sorted");

    // Tick 1: only rate=1 system fires.
    DF_CHECK(df_wake_registry_fire_timer(1) == 1, "tick 1 wakes only rate=1");
    df_wake_registry_drain_runqueue(buf, 8);

    // Tick 3: rate=1 and rate=3 fire.
    DF_CHECK(df_wake_registry_fire_timer(3) == 2, "tick 3 wakes rate=1 and rate=3");
}

void scenario_wake_registry_fire_event_and_state() {
    std::printf("scenario_wake_registry_fire_event_and_state\n");
    df_wake_registry_clear();
    df_wake_registry_subscribe_event(/*sys*/10, /*evt*/0xA1);
    df_wake_registry_subscribe_event(/*sys*/11, /*evt*/0xA1);
    df_wake_registry_subscribe_event(/*sys*/20, /*evt*/0xB2);

    DF_CHECK(df_wake_registry_fire_event(0xA1) == 2, "event 0xA1 wakes 10 and 11");
    DF_CHECK(df_wake_registry_runqueue_size() == 2, "runqueue size 2");
    uint32_t buf[4] = {0};
    df_wake_registry_drain_runqueue(buf, 4);
    DF_CHECK(buf[0] == 10 && buf[1] == 11, "correct ids drained");

    DF_CHECK(df_wake_registry_fire_event(0xB2) == 1, "event 0xB2 wakes 20");
    df_wake_registry_drain_runqueue(buf, 4);

    df_wake_registry_subscribe_state(/*sys*/30, /*comp*/0xC0);
    df_wake_registry_subscribe_state(/*sys*/40, /*comp*/0xC1);
    DF_CHECK(df_wake_registry_fire_state_change(0xC0, /*entity*/0) == 1,
             "state change comp=0xC0 wakes 30");
}

void scenario_wake_registry_fire_init_one_shot() {
    std::printf("scenario_wake_registry_fire_init_one_shot\n");
    df_wake_registry_clear();
    df_wake_registry_subscribe_init(10);
    df_wake_registry_subscribe_init(20);
    DF_CHECK(df_wake_registry_fire_init() == 2, "first init fires both");
    uint32_t buf[4] = {0};
    df_wake_registry_drain_runqueue(buf, 4);

    DF_CHECK(df_wake_registry_fire_init() == 0, "second init fires zero (one-shot)");
}

// =============================================================================
// K10.1 Items 6+7+8 — scheduling policies scenarios.
// =============================================================================

void scenario_scheduler_affinity_workstealing_barriers() {
    std::printf("scenario_scheduler_affinity_workstealing_barriers\n");
    df_scheduler_policies_clear();
    df_scheduler_clear();
    // Affinity round-trip.
    DF_CHECK(df_scheduler_policies_get_affinity(7) == -1, "unset affinity is -1");
    DF_CHECK(df_scheduler_policies_set_affinity(7, 4) == 1, "set affinity 4");
    DF_CHECK(df_scheduler_policies_get_affinity(7) == 4, "affinity read 4");

    // Work stealing toggle.
    DF_CHECK(df_scheduler_work_stealing_enabled() == 1, "default work stealing on");
    df_scheduler_set_work_stealing_enabled(0);
    DF_CHECK(df_scheduler_work_stealing_enabled() == 0, "work stealing toggled off");
    df_scheduler_set_work_stealing_enabled(1);
    DF_CHECK(df_scheduler_work_stealing_enabled() == 1, "work stealing toggled on");

    // Phase barrier round-trip (default Full).
    DF_CHECK(df_scheduler_get_phase_barrier(0) == 0, "default barrier Full");
    DF_CHECK(df_scheduler_set_phase_barrier(0, 1) == 1, "set barrier Partial");
    DF_CHECK(df_scheduler_get_phase_barrier(0) == 1, "barrier read Partial");
    DF_CHECK(df_scheduler_set_phase_barrier(2, 2) == 1, "set barrier None on phase 2");
    DF_CHECK(df_scheduler_get_phase_barrier(2) == 2, "barrier read None");
    DF_CHECK(df_scheduler_set_phase_barrier(-1, 0) == 0, "negative phase rejected");
    DF_CHECK(df_scheduler_set_phase_barrier(0, 99) == 0, "invalid barrier type rejected");
}

// =============================================================================
// K10.1 Item 15 — batched callback ABI scenarios.
// =============================================================================

struct BatchCaptureState {
    int call_count = 0;
    uint32_t last_count = 0;
    float last_delta = 0.0f;
    std::vector<uint32_t> last_ids;
};

void batch_capture_cb(const df_managed_system_batch* batch) {
    auto* state = static_cast<BatchCaptureState*>(batch->user_data);
    state->call_count++;
    state->last_count = batch->count;
    state->last_delta = batch->delta;
    state->last_ids.assign(batch->system_ids, batch->system_ids + batch->count);
}

// =============================================================================
// K10.1 Item 17 — write-through hook (state change filter) scenarios.
// =============================================================================

void scenario_scheduler_trace() {
    std::printf("scenario_scheduler_trace\n");
    df_scheduler_trace_clear();
    df_scheduler_trace_set_enabled(0);
    DF_CHECK(df_scheduler_trace_enabled() == 0, "trace off by default");
    df_scheduler_trace_push(0, 1, 2, 1000, 50);
    DF_CHECK(df_scheduler_trace_event_count() == 0, "disabled push ignored");

    df_scheduler_trace_set_enabled(1);
    DF_CHECK(df_scheduler_trace_enabled() == 1, "trace enabled");
    df_scheduler_trace_push(/*type*/0, /*arg0*/10, /*arg1*/2, /*ts*/100, /*val*/0);
    df_scheduler_trace_push(/*type*/1, /*arg0*/10, /*arg1*/0, /*ts*/200, /*val*/50);
    df_scheduler_trace_push(/*type*/2, /*arg0*/10, /*arg1*/0, /*ts*/300, /*val*/150);
    DF_CHECK(df_scheduler_trace_event_count() == 3, "3 events recorded");

    df_trace_event buf[8] = {};
    int32_t n = df_scheduler_trace_dump(buf, 8);
    DF_CHECK(n == 3, "dump returns 3");
    // Most-recent first.
    DF_CHECK(buf[0].event_type == 2 && buf[0].value == 150, "newest = SystemCompleted");
    DF_CHECK(buf[1].event_type == 1, "next = SystemDispatched");
    DF_CHECK(buf[2].event_type == 0, "oldest = SystemWoken");

    df_scheduler_trace_clear();
    DF_CHECK(df_scheduler_trace_event_count() == 0, "cleared");
    df_scheduler_trace_set_enabled(0);
}

void scenario_scheduler_intrinsics() {
    std::printf("scenario_scheduler_intrinsics\n");
    df_scheduler_intrinsics_reset();
    DF_CHECK(df_scheduler_is_suspended() == 0, "default not suspended");
    DF_CHECK(df_scheduler_is_panic() == 0, "default not panic");

    df_scheduler_suspend();
    DF_CHECK(df_scheduler_is_suspended() == 1, "suspended");
    df_scheduler_resume();
    DF_CHECK(df_scheduler_is_suspended() == 0, "resumed");

    df_scheduler_panic_halt("test panic message");
    DF_CHECK(df_scheduler_is_panic() == 1, "panic latched");
    DF_CHECK(df_scheduler_is_suspended() == 1, "panic implies suspend");

    char snap[256] = {0};
    int32_t bytes = df_scheduler_snapshot(snap, 256);
    DF_CHECK(bytes > 0, "snapshot wrote bytes");
    DF_CHECK(std::strstr(snap, "K10.1") != nullptr, "snapshot contains K10.1 marker");

    df_scheduler_intrinsics_reset();
    DF_CHECK(df_scheduler_is_panic() == 0, "reset clears panic");
    DF_CHECK(df_scheduler_is_suspended() == 0, "reset clears suspend");
}

void scenario_state_filter_cold_path_bypass() {
    std::printf("scenario_state_filter_cold_path_bypass\n");
    df_state_filter_clear();
    df_wake_registry_clear();
    // Empty filter — cold path bypass: may_have_subscribers returns false.
    DF_CHECK(df_state_filter_may_have_subscribers(42) == 0, "no subscribers → bypass");
    // Hook is a no-op when no subscribers.
    df_native_world_commit_hook(42, /*entity*/1);
    DF_CHECK(df_wake_registry_runqueue_size() == 0, "hook adds nothing к runqueue");
}

void scenario_state_filter_subscribe_and_hook() {
    std::printf("scenario_state_filter_subscribe_and_hook\n");
    df_state_filter_clear();
    df_wake_registry_clear();
    // Subscribe SystemA type-wide on component 42.
    DF_CHECK(df_state_filter_subscribe_type(42, /*systemA*/1) == 1, "subscribe type");
    // Also subscribe к state-change для wake routing.
    df_wake_registry_subscribe_state(1, 42);

    DF_CHECK(df_state_filter_may_have_subscribers(42) == 1, "filter Level 1 hit");
    DF_CHECK(df_state_filter_may_have_subscribers(99) == 0, "other types still cold");

    // Commit hook fires wake for SystemA.
    df_native_world_commit_hook(42, /*entity*/5);
    DF_CHECK(df_wake_registry_runqueue_size() == 1, "system A in runqueue");

    // Unsubscribe drops the bit.
    DF_CHECK(df_state_filter_unsubscribe_type(42, 1) == 1, "unsubscribe");
    DF_CHECK(df_state_filter_may_have_subscribers(42) == 0, "bit cleared");
}

void scenario_state_filter_entity_specific() {
    std::printf("scenario_state_filter_entity_specific\n");
    df_state_filter_clear();
    df_wake_registry_clear();
    df_state_filter_subscribe_entity(42, /*entity*/7, /*systemA*/1);
    DF_CHECK(df_state_filter_may_have_subscribers(42) == 1, "Level 1 hit on entity sub");
    DF_CHECK(df_state_filter_has_entity_specific_subscriber(42, 7) == 1, "entity 7 specifically");
    DF_CHECK(df_state_filter_has_entity_specific_subscriber(42, 99) == 0, "entity 99 not");
    DF_CHECK(df_state_filter_entity_subscriber_count(42) == 1, "1 entity subscriber");
}

void scenario_state_filter_out_of_range() {
    std::printf("scenario_state_filter_out_of_range\n");
    df_state_filter_clear();
    // Type id ≥ 256 — conservative may_have_subscribers = true.
    DF_CHECK(df_state_filter_may_have_subscribers(256) == 1, "out-of-range conservative true");
    DF_CHECK(df_state_filter_may_have_subscribers(0xFFFFFFFFu) == 1, "uint max conservative true");
}

void scenario_managed_callback_roundtrip() {
    std::printf("scenario_managed_callback_roundtrip\n");
    df_scheduler_clear_managed_callback();
    DF_CHECK(df_scheduler_managed_callback_registered() == 0, "no callback initially");

    BatchCaptureState state;
    df_scheduler_register_managed_callback(batch_capture_cb, &state);
    DF_CHECK(df_scheduler_managed_callback_registered() == 1, "callback registered");

    uint32_t ids[] = {7, 11, 13};
    df_managed_system_batch batch;
    batch.system_ids = ids;
    batch.count = 3;
    batch.delta = 0.033f;  // ~30Hz
    batch.user_data = &state;

    DF_CHECK(df_scheduler_dispatch_managed_batch(&batch) == 1, "dispatch succeeded");
    DF_CHECK(state.call_count == 1, "callback invoked once");
    DF_CHECK(state.last_count == 3, "count round-tripped");
    DF_CHECK(state.last_ids.size() == 3 &&
             state.last_ids[0] == 7 &&
             state.last_ids[1] == 11 &&
             state.last_ids[2] == 13,
             "ids round-tripped");
    DF_CHECK(state.last_delta == 0.033f, "delta round-tripped");

    // Dispatch с null batch returns 0, no crash.
    DF_CHECK(df_scheduler_dispatch_managed_batch(nullptr) == 0, "null batch rejected");
    DF_CHECK(state.call_count == 1, "no extra invocation on null batch");

    // Clear and re-dispatch — should fail (no callback).
    df_scheduler_clear_managed_callback();
    DF_CHECK(df_scheduler_managed_callback_registered() == 0, "cleared");
    DF_CHECK(df_scheduler_dispatch_managed_batch(&batch) == 0, "dispatch fails post-clear");
}

void scenario_shm_region_basic() {
    std::printf("scenario_shm_region_basic\n");
    df_shm_clear();
    DF_CHECK(df_shm_region_count() == 0, "no regions initially");

    DF_CHECK(df_shm_create(1, 256) == 1, "create region 1 (256 bytes)");
    DF_CHECK(df_shm_create(1, 256) == 0, "duplicate region rejected");
    DF_CHECK(df_shm_create(2, 0) == 0, "size 0 rejected");
    DF_CHECK(df_shm_create(2, -1) == 0, "negative size rejected");
    DF_CHECK(df_shm_size(1) == 256, "size queries to 256");
    DF_CHECK(df_shm_size(99) == 0, "unknown region size 0");

    void* ptr = df_shm_map(1);
    DF_CHECK(ptr != nullptr, "map returns non-null");
    // Memory is zero-initialized.
    auto* bytes = static_cast<uint8_t*>(ptr);
    DF_CHECK(bytes[0] == 0 && bytes[255] == 0, "region zero-initialized");
    bytes[42] = 0xAB;
    auto* re_map = static_cast<uint8_t*>(df_shm_map(1));
    DF_CHECK(re_map[42] == 0xAB, "writes visible on re-map");

    DF_CHECK(df_shm_register_writer(1, 99) == 1, "register writer 99");
    DF_CHECK(df_shm_writer(1) == 99, "writer is 99");
    DF_CHECK(df_shm_register_writer(99, 42) == 0, "unknown region writer rejected");

    DF_CHECK(df_shm_unmap(1) == 1, "unmap (no-op) returns ok");
    DF_CHECK(df_shm_destroy(1) == 1, "destroy region 1");
    DF_CHECK(df_shm_destroy(1) == 0, "second destroy fails");
    DF_CHECK(df_shm_map(1) == nullptr, "post-destroy map nullptr");
}

void scenario_scheduling_policies_default() {
    std::printf("scenario_scheduling_policies_default\n");
    df_scheduler_policies_clear();
    // Unset systems get default: Normal class (2), no quota.
    DF_CHECK(df_scheduler_policies_get_class(99) == 2, "default class is Normal");
    DF_CHECK(df_scheduler_policies_get_quota(99) == 0, "default quota is 0 (unbounded)");
    DF_CHECK(df_scheduler_policies_quota_exceeded(99) == 0, "no quota → never exceeded");
}

void scenario_scheduling_policies_set_and_get() {
    std::printf("scenario_scheduling_policies_set_and_get\n");
    df_scheduler_policies_clear();
    DF_CHECK(df_scheduler_policies_set(10, /*RT*/0, 100, 50, 200, /*Forced*/1) == 1,
             "set policy RT/100us/50us/200us/Forced");
    DF_CHECK(df_scheduler_policies_get_class(10) == 0, "class is RealTime");
    DF_CHECK(df_scheduler_policies_get_quota(10) == 200, "quota is 200");

    // Invalid class rejected.
    DF_CHECK(df_scheduler_policies_set(11, 99, 0, 0, 0, 0) == 0, "invalid class rejected");
}

void scenario_scheduling_policies_quota_enforcement() {
    std::printf("scenario_scheduling_policies_quota_enforcement\n");
    df_scheduler_policies_clear();
    df_scheduler_policies_set(20, /*Normal*/2, 0, 0, /*quota*/100, 0);

    // Under quota.
    DF_CHECK(df_scheduler_policies_record_execution(20, 50) == 0, "50us under 100us");
    DF_CHECK(df_scheduler_policies_quota_exceeded(20) == 0, "not yet exceeded");

    // Push over quota.
    DF_CHECK(df_scheduler_policies_record_execution(20, 60) == 1, "50+60=110 exceeds 100");
    DF_CHECK(df_scheduler_policies_quota_exceeded(20) == 1, "quota now exceeded");
    DF_CHECK(df_scheduler_policies_quota_violations(20) == 1, "one violation event");

    DF_CHECK(df_scheduler_policies_total_micros(20) == 110, "total micros tracked");

    // Reset tick stats clears per-tick state but preserves totals.
    df_scheduler_policies_reset_tick_stats();
    DF_CHECK(df_scheduler_policies_quota_exceeded(20) == 0, "post-reset not exceeded");
    DF_CHECK(df_scheduler_policies_total_micros(20) == 110, "total micros preserved");
    DF_CHECK(df_scheduler_policies_quota_violations(20) == 1, "violation count preserved");
}

void scenario_scheduling_policies_order_by_priority() {
    std::printf("scenario_scheduling_policies_order_by_priority\n");
    df_scheduler_policies_clear();
    df_scheduler_policies_set(1, /*Normal*/2, 0, 0, 0, 0);
    df_scheduler_policies_set(2, /*RealTime*/0, 0, 0, 0, 0);
    df_scheduler_policies_set(3, /*High*/1, 0, 0, 0, 0);
    df_scheduler_policies_set(4, /*Background*/4, 0, 0, 0, 0);
    df_scheduler_policies_set(5, /*Normal*/2, 0, 0, 0, 0);

    uint32_t in_ids[] = {1, 2, 3, 4, 5};
    uint32_t out_ids[5] = {0};
    int32_t n = df_scheduler_policies_order_by_priority(in_ids, 5, out_ids, 5);
    DF_CHECK(n == 5, "5 ids ordered");
    // Expected: 2 (RT), 3 (High), 1 (Normal id=1), 5 (Normal id=5), 4 (Background)
    DF_CHECK(out_ids[0] == 2, "RT first");
    DF_CHECK(out_ids[1] == 3, "High second");
    DF_CHECK(out_ids[2] == 1, "Normal id=1 third");
    DF_CHECK(out_ids[3] == 5, "Normal id=5 fourth");
    DF_CHECK(out_ids[4] == 4, "Background last");
}

void scenario_scheduler_tick_begin_orchestration() {
    std::printf("scenario_scheduler_tick_begin_orchestration\n");
    df_wake_registry_clear();
    df_scheduler_clear();
    // Register three systems with a linear write→read chain A→B→C.
    constexpr uint32_t cX = 100, cY = 101;
    uint32_t a_w[] = {cX};
    uint32_t b_r[] = {cX};
    uint32_t b_w[] = {cY};
    uint32_t c_r[] = {cY};
    df_scheduler_register_system(1, "A", nullptr, 0, a_w, 1, 2, 0);
    df_scheduler_register_system(2, "B", b_r, 1, b_w, 1, 2, 0);
    df_scheduler_register_system(3, "C", c_r, 1, nullptr, 0, 2, 0);
    // Subscribe: A timer every tick, B timer rate 2, C timer rate 4.
    df_wake_registry_subscribe_timer(1, 1);
    df_wake_registry_subscribe_timer(2, 2);
    df_wake_registry_subscribe_timer(3, 4);

    // Tick 0: all three rates divide 0, so {1,2,3} run → phases 1→2→3.
    DF_CHECK(df_scheduler_tick_begin(0) == 1, "tick 0 success");
    DF_CHECK(df_scheduler_per_tick_phase_count() == 3, "3 per-tick phases at tick 0");

    // Tick 1: only A (rate 1) fires. Single phase containing only {1}.
    DF_CHECK(df_scheduler_tick_begin(1) == 1, "tick 1 success");
    DF_CHECK(df_scheduler_per_tick_phase_count() == 1, "1 per-tick phase at tick 1");
    DF_CHECK(df_scheduler_per_tick_phase_size(0) == 1, "phase 0 has 1 system");

    // Tick 2: A (rate 1) + B (rate 2). B reads cX which A writes, so A → B
    // edge in subset → phases [A] then [B].
    DF_CHECK(df_scheduler_tick_begin(2) == 1, "tick 2 success");
    DF_CHECK(df_scheduler_per_tick_phase_count() == 2, "2 phases at tick 2 (A→B chain)");
}

void scenario_scheduler_diagnostics() {
    std::printf("scenario_scheduler_diagnostics\n");
    df_wake_registry_clear();
    df_wake_registry_subscribe_timer(10, 1);
    df_wake_registry_subscribe_event(10, 0xA1);
    df_wake_registry_subscribe_init(10);
    df_wake_registry_subscribe_state(20, 0xC0);

    int32_t mask10 = df_scheduler_query_wake_subscriptions(10);
    DF_CHECK(mask10 == (1 | (1<<1) | (1<<3)), "system 10 has Timer|Event|Init");
    int32_t mask20 = df_scheduler_query_wake_subscriptions(20);
    DF_CHECK(mask20 == (1<<2), "system 20 has only StateChange");
    int32_t mask99 = df_scheduler_query_wake_subscriptions(99);
    DF_CHECK(mask99 == 0, "unknown system has no subscriptions");

    // Fire timer + event so runqueue has both 10 (from timer or event) and 20 (state)
    df_wake_registry_fire_timer(0);  // 10 wakes (rate=1, tick=0)
    df_wake_registry_fire_state_change(0xC0, 0);  // 20 wakes
    uint32_t buf[4] = {0};
    int32_t n = df_scheduler_query_runnable(buf, 4);
    DF_CHECK(n == 2, "peek shows 2 runnable");
    DF_CHECK(buf[0] == 10 && buf[1] == 20, "ids sorted");
    // Peek is non-draining — runqueue still has the same.
    int32_t n2 = df_scheduler_query_runnable(buf, 4);
    DF_CHECK(n2 == 2, "second peek still shows 2 (non-draining)");
}

void scenario_wake_registry_fire_explicit() {
    std::printf("scenario_wake_registry_fire_explicit\n");
    df_wake_registry_clear();
    df_wake_registry_subscribe_explicit(/*sys*/10, /*wake_id*/0xE0);
    df_wake_registry_subscribe_explicit(/*sys*/10, /*wake_id*/0xE1);

    DF_CHECK(df_wake_registry_fire_explicit(10, 0xE0) == 1, "fire E0 on 10");
    DF_CHECK(df_wake_registry_fire_explicit(10, 0xE2) == 0, "fire unknown E2 fires zero");
    DF_CHECK(df_wake_registry_fire_explicit(99, 0xE0) == 0, "fire on unknown target");
}

void scenario_thread_pool_mode_transition() {
    std::printf("scenario_thread_pool_mode_transition\n");
    using namespace dualfrontier;
    ThreadPool pool(2);
    DF_CHECK(pool.current_mode() == ThreadPool::Mode::Bootstrap,
             "default mode is Bootstrap");

    pool.transition_to_scheduler_mode();
    DF_CHECK(pool.current_mode() == ThreadPool::Mode::Scheduler,
             "transition к Scheduler mode");

    pool.transition_to_bootstrap_mode();
    DF_CHECK(pool.current_mode() == ThreadPool::Mode::Bootstrap,
             "revert к Bootstrap mode");
    pool.shutdown();
}

void scenario_thread_pool_submit_batch() {
    std::printf("scenario_thread_pool_submit_batch\n");
    using namespace dualfrontier;
    ThreadPool pool(4);
    std::atomic<int> counter{0};
    std::vector<ThreadPool::Task> batch;
    for (int i = 0; i < 50; ++i) {
        batch.push_back([&counter]() { counter.fetch_add(1); });
    }
    pool.submit_batch(std::move(batch));
    pool.wait_phase_barrier();
    DF_CHECK(counter.load() == 50, "all 50 batched tasks executed");

    // Empty batch is a no-op.
    pool.submit_batch({});
    pool.wait_phase_barrier();

    pool.shutdown();
}

void scenario_thread_pool_phase_barrier() {
    std::printf("scenario_thread_pool_phase_barrier\n");
    using namespace dualfrontier;
    ThreadPool pool(2);
    std::atomic<int> seen{0};
    // First phase: 10 tasks, all bumping seen by 1.
    std::vector<ThreadPool::Task> phase1;
    for (int i = 0; i < 10; ++i) {
        phase1.push_back([&seen]() { seen.fetch_add(1); });
    }
    pool.submit_batch(std::move(phase1));
    pool.wait_phase_barrier();
    DF_CHECK(seen.load() == 10, "phase 1 completed before barrier release");

    // Second phase reads pre-barrier value, asserts it.
    std::atomic<int> observed{-1};
    pool.submit([&]() { observed.store(seen.load()); });
    pool.wait_phase_barrier();
    DF_CHECK(observed.load() == 10, "phase 2 observes phase 1 final state");

    pool.shutdown();
}

void scenario_system_graph_per_tick_subset() {
    std::printf("scenario_system_graph_per_tick_subset\n");
    df_scheduler_clear();
    // Static graph: A → B → C (linear). Per-tick over {A, C} only — B blocked.
    // Edges between A and C? A writes X (read by B), B writes Y (read by C).
    // A's writes do NOT directly touch C's reads, so no edge A → C in subset.
    // Per-tick phases over {A, C}: both runnable in phase 0 (no edges between
    // them within subset).
    constexpr uint32_t cX = 100, cY = 101;
    uint32_t a_w[] = {cX};
    uint32_t b_r[] = {cX};
    uint32_t b_w[] = {cY};
    uint32_t c_r[] = {cY};
    df_scheduler_register_system(1, "A", nullptr, 0, a_w, 1, 2, 0);
    df_scheduler_register_system(2, "B", b_r, 1, b_w, 1, 2, 0);
    df_scheduler_register_system(3, "C", c_r, 1, nullptr, 0, 2, 0);

    uint32_t runnable[] = {1, 3};
    int32_t r = df_scheduler_compute_per_tick_graph(runnable, 2);
    DF_CHECK(r == 1, "per-tick compute success");
    DF_CHECK(df_scheduler_per_tick_phase_count() == 1,
             "single per-tick phase (no edge A→C in subset)");
    DF_CHECK(df_scheduler_per_tick_phase_size(0) == 2, "both A and C in phase 0");

    // Unknown id rejected.
    uint32_t bad[] = {99};
    int32_t br = df_scheduler_compute_per_tick_graph(bad, 1);
    DF_CHECK(br == 0, "unknown runnable id rejected");
}

// ===== K10.2 Item 28 — event type registry (tier-annotated) =====

void coalesce_sum_int32(void* existing, const void* new_event) {
    int32_t* dst = static_cast<int32_t*>(existing);
    const int32_t* src = static_cast<const int32_t*>(new_event);
    *dst += *src;
}

void scenario_event_type_registry_register_and_lookup() {
    std::printf("scenario_event_type_registry_register_and_lookup\n");
    df_event_type_registry_clear();

    // Register Fast tier event
    const char* fast_fqn = "DualFrontier.Test.FastEvent";
    int32_t rc = df_event_type_registry_register(
        0x1000u, /*Fast*/0, sizeof(int32_t), fast_fqn, nullptr);
    DF_CHECK(rc == 1, "fast tier register succeeds without coalesce");

    // Register Normal tier event
    const char* normal_fqn = "DualFrontier.Test.NormalEvent";
    rc = df_event_type_registry_register(
        0x2000u, /*Normal*/1, sizeof(int32_t) * 2, normal_fqn, nullptr);
    DF_CHECK(rc == 1, "normal tier register succeeds without coalesce");

    // Register Background tier event с coalesce
    const char* bg_fqn = "DualFrontier.Test.BackgroundEvent";
    rc = df_event_type_registry_register(
        0x3000u, /*Background*/2, sizeof(int32_t), bg_fqn, coalesce_sum_int32);
    DF_CHECK(rc == 1, "background tier register succeeds with coalesce");

    // Background tier без coalesce fails
    rc = df_event_type_registry_register(
        0x3001u, /*Background*/2, sizeof(int32_t), "Test.Other", nullptr);
    DF_CHECK(rc == 0, "background tier register without coalesce fails");

    // Lookup
    int32_t  tier = -1;
    uint32_t size = 0;
    const char* fqn = nullptr;
    void (*coalesce)(void*, const void*) = nullptr;
    rc = df_event_type_registry_lookup(0x1000u, &tier, &size, &fqn, &coalesce);
    DF_CHECK(rc == 1 && tier == 0 && size == sizeof(int32_t) && fqn == fast_fqn && coalesce == nullptr,
             "fast tier metadata round-trip");

    rc = df_event_type_registry_lookup(0x3000u, &tier, &size, &fqn, &coalesce);
    DF_CHECK(rc == 1 && tier == 2 && coalesce == coalesce_sum_int32,
             "background tier coalesce fn pointer round-trip");

    df_event_type_registry_clear();
}

void scenario_event_type_registry_get_tier_default_normal() {
    std::printf("scenario_event_type_registry_get_tier_default_normal\n");
    df_event_type_registry_clear();

    // Per S-LOCK-4 backward compatibility: unregistered events return Normal
    int32_t tier = -1;
    int32_t rc = df_event_type_registry_get_tier(0xDEAD0001u, &tier);
    DF_CHECK(rc == 1 && tier == 1, "unregistered event defaults к Normal tier");

    // After registration, get_tier returns explicit value
    df_event_type_registry_register(0x4000u, /*Fast*/0, 4, "Test.Default.Fast", nullptr);
    rc = df_event_type_registry_get_tier(0x4000u, &tier);
    DF_CHECK(rc == 1 && tier == 0, "registered event returns explicit Fast tier");

    df_event_type_registry_clear();
}

void scenario_event_type_registry_reregister_idempotent_or_conflict() {
    std::printf("scenario_event_type_registry_reregister_idempotent_or_conflict\n");
    df_event_type_registry_clear();

    // First registration succeeds
    int32_t rc = df_event_type_registry_register(
        0x5000u, /*Normal*/1, 8, "Test.Reg.A", nullptr);
    DF_CHECK(rc == 1, "first register succeeds");

    // Idempotent re-registration с identical metadata
    rc = df_event_type_registry_register(
        0x5000u, /*Normal*/1, 8, "Test.Reg.A", nullptr);
    DF_CHECK(rc == 1, "idempotent re-register с identical metadata succeeds");

    // Conflict re-registration с different tier fails
    rc = df_event_type_registry_register(
        0x5000u, /*Fast*/0, 8, "Test.Reg.A", nullptr);
    DF_CHECK(rc == 0, "conflict re-register с different tier fails");

    DF_CHECK(df_event_type_registry_count() == 1,
             "registry count remains 1 after conflict");
    df_event_type_registry_clear();
}

// ===== K10.2 Item 26 — native bus three-tier dispatcher =====

struct BusTestPayload {
    int32_t value;
};

std::atomic<int32_t> g_fast_subscriber_invocations{0};
std::atomic<int32_t> g_fast_subscriber_last_value{0};

void test_fast_subscriber(uint32_t type_id, const void* payload, uint32_t /*size*/, void* /*user_data*/) {
    (void)type_id;
    const BusTestPayload* p = static_cast<const BusTestPayload*>(payload);
    g_fast_subscriber_last_value.store(p->value, std::memory_order_relaxed);
    g_fast_subscriber_invocations.fetch_add(1, std::memory_order_relaxed);
}

std::atomic<int32_t> g_normal_subscriber_invocations{0};

void test_normal_subscriber(const df_managed_system_batch* batch) {
    (void)batch;
    g_normal_subscriber_invocations.fetch_add(1, std::memory_order_relaxed);
}

void scenario_bus_fast_publish_subscribe_roundtrip() {
    std::printf("scenario_bus_fast_publish_subscribe_roundtrip\n");
    df_bus_clear();

    const uint32_t type_id = 0xFA570001u;
    g_fast_subscriber_invocations.store(0);
    g_fast_subscriber_last_value.store(0);

    df_bus_subscription_id sid = df_bus_subscribe_fast(
        type_id, /*mod_id=*/100u, &test_fast_subscriber, nullptr);
    DF_CHECK(sid != 0, "fast subscribe returns non-zero id");
    DF_CHECK(df_bus_subscriber_count_fast(type_id) == 1, "fast subscriber count = 1");

    BusTestPayload payload{42};
    int32_t invoked = df_bus_publish_fast(type_id, &payload, sizeof(payload));
    DF_CHECK(invoked == 1, "fast publish invoked 1 subscriber");
    DF_CHECK(g_fast_subscriber_invocations.load() == 1, "subscriber callback fired once");
    DF_CHECK(g_fast_subscriber_last_value.load() == 42, "payload delivered with value=42");

    // Unsubscribe + publish again: no invocation
    int32_t unsubbed = df_bus_unsubscribe(sid);
    DF_CHECK(unsubbed == 1, "unsubscribe succeeds");
    DF_CHECK(df_bus_subscriber_count_fast(type_id) == 0, "subscriber count = 0 after unsubscribe");
    g_fast_subscriber_invocations.store(0);
    df_bus_publish_fast(type_id, &payload, sizeof(payload));
    DF_CHECK(g_fast_subscriber_invocations.load() == 0, "no invocation after unsubscribe");

    df_bus_clear();
}

void scenario_bus_normal_publish_batched_drain() {
    std::printf("scenario_bus_normal_publish_batched_drain\n");
    df_bus_clear();

    const uint32_t type_id = 0xA0BAFF01u;
    g_normal_subscriber_invocations.store(0);

    df_bus_subscription_id sid = df_bus_subscribe_normal(
        type_id, /*mod_id=*/200u, &test_normal_subscriber, nullptr);
    DF_CHECK(sid != 0, "normal subscribe returns non-zero id");

    // Publish 3 events; no dispatch yet (Normal tier batches к phase boundary)
    BusTestPayload payload{1};
    df_bus_publish_normal(type_id, &payload, sizeof(payload));
    df_bus_publish_normal(type_id, &payload, sizeof(payload));
    df_bus_publish_normal(type_id, &payload, sizeof(payload));
    DF_CHECK(g_normal_subscriber_invocations.load() == 0, "no dispatch before drain");

    // Drain — dispatches all pending to subscriber
    int32_t batches = df_bus_drain_normal_batch();
    DF_CHECK(batches == 3, "3 batches dispatched after drain");
    DF_CHECK(g_normal_subscriber_invocations.load() == 3, "subscriber received 3 invocations");

    // Drain again — empty pending, no invocations
    batches = df_bus_drain_normal_batch();
    DF_CHECK(batches == 0, "drain on empty pending returns 0");

    df_bus_clear();
}

void scenario_bus_background_publish_stores_pending() {
    std::printf("scenario_bus_background_publish_stores_pending\n");
    df_bus_clear();

    const uint32_t type_id = 0xB6000101u;
    df_bus_subscription_id sid = df_bus_subscribe_background(
        type_id, /*mod_id=*/300u, &test_normal_subscriber, nullptr);
    DF_CHECK(sid != 0, "background subscribe returns non-zero id");

    // Commit 4 lands the storage path; coalesce + idle-slot dispatch in Commit 5
    BusTestPayload payload{99};
    int32_t rc = df_bus_publish_background(type_id, &payload, sizeof(payload), /*coalesce_key=*/7u);
    DF_CHECK(rc == 1, "background publish accepts event");
    DF_CHECK(df_bus_subscriber_count_background(type_id) == 1, "background subscriber registry preserved");

    df_bus_clear();
}

// ===== K10.2 Item 30 — background queue + idle-slot scheduling =====

std::atomic<int32_t> g_bg_subscriber_invocations{0};

void test_background_subscriber(const df_managed_system_batch* /*batch*/) {
    g_bg_subscriber_invocations.fetch_add(1, std::memory_order_relaxed);
}

void coalesce_sum_int32_bg(void* existing, const void* new_event) {
    int32_t* dst = static_cast<int32_t*>(existing);
    const int32_t* src = static_cast<const int32_t*>(new_event);
    *dst += *src;
}

void scenario_bg_queue_coalesce_same_key() {
    std::printf("scenario_bg_queue_coalesce_same_key\n");
    df_bus_clear();
    df_event_type_registry_clear();

    const uint32_t type_id = 0xBA110001u;
    df_event_type_registry_register(
        type_id, /*Background*/2, sizeof(int32_t), "Test.Bg.Coalesce", coalesce_sum_int32_bg);

    df_bus_subscription_id sid = df_bus_subscribe_background(
        type_id, /*mod_id=*/700u, &test_background_subscriber, nullptr);
    DF_CHECK(sid != 0, "background subscribe id non-zero");

    // Publish 3 events с same coalesce_key — should coalesce к 1
    int32_t v1 = 10;
    int32_t v2 = 20;
    int32_t v3 = 5;
    df_bus_publish_background(type_id, &v1, sizeof(v1), /*coalesce_key=*/1u);
    df_bus_publish_background(type_id, &v2, sizeof(v2), /*coalesce_key=*/1u);
    df_bus_publish_background(type_id, &v3, sizeof(v3), /*coalesce_key=*/1u);

    uint32_t count_before = 0;
    df_background_queue_size(&count_before, nullptr);
    DF_CHECK(count_before == 3, "3 events queued before coalesce");

    int32_t merged = df_background_queue_force_coalesce();
    DF_CHECK(merged == 2, "2 events merged into 1 by coalesce");

    uint32_t count_after = 0;
    df_background_queue_size(&count_after, nullptr);
    DF_CHECK(count_after == 1, "1 event remains after coalesce");

    // Dispatch with unlimited budget (0)
    g_bg_subscriber_invocations.store(0);
    int32_t dispatched = df_background_queue_dispatch_idle_slot(0);
    DF_CHECK(dispatched == 1, "1 event dispatched in idle slot");
    DF_CHECK(g_bg_subscriber_invocations.load() == 1, "subscriber invoked once");

    df_bus_clear();
    df_event_type_registry_clear();
}

void scenario_bg_queue_different_keys_not_coalesced() {
    std::printf("scenario_bg_queue_different_keys_not_coalesced\n");
    df_bus_clear();
    df_event_type_registry_clear();

    const uint32_t type_id = 0xBA110002u;
    df_event_type_registry_register(
        type_id, /*Background*/2, sizeof(int32_t), "Test.Bg.NoCoalesce", coalesce_sum_int32_bg);
    df_bus_subscribe_background(type_id, /*mod_id=*/701u, &test_background_subscriber, nullptr);

    int32_t v1 = 10, v2 = 20;
    df_bus_publish_background(type_id, &v1, sizeof(v1), /*coalesce_key=*/1u);
    df_bus_publish_background(type_id, &v2, sizeof(v2), /*coalesce_key=*/2u);

    int32_t merged = df_background_queue_force_coalesce();
    DF_CHECK(merged == 0, "no events merged when keys differ");

    uint32_t count = 0;
    df_background_queue_size(&count, nullptr);
    DF_CHECK(count == 2, "2 events remain (different keys)");

    df_bus_clear();
    df_event_type_registry_clear();
}

void scenario_bg_queue_idle_slot_budget_partial_dispatch() {
    std::printf("scenario_bg_queue_idle_slot_budget_partial_dispatch\n");
    df_bus_clear();
    df_event_type_registry_clear();

    const uint32_t type_id = 0xBA110003u;
    df_event_type_registry_register(
        type_id, /*Background*/2, sizeof(int32_t), "Test.Bg.Partial", coalesce_sum_int32_bg);
    df_bus_subscribe_background(type_id, /*mod_id=*/702u, &test_background_subscriber, nullptr);

    // Publish 5 events с unique coalesce keys
    for (int32_t i = 0; i < 5; ++i) {
        int32_t v = i + 1;
        df_bus_publish_background(type_id, &v, sizeof(v), static_cast<uint32_t>(i + 100));
    }

    g_bg_subscriber_invocations.store(0);
    // 250 micros budget at 100 micros/event = 2 events dispatched, 3 requeued
    int32_t dispatched = df_background_queue_dispatch_idle_slot(250);
    DF_CHECK(dispatched == 2, "2 events dispatched in 250µs budget");
    DF_CHECK(g_bg_subscriber_invocations.load() == 2, "subscriber invoked twice");

    uint32_t remaining = 0;
    df_background_queue_size(&remaining, nullptr);
    DF_CHECK(remaining == 3, "3 events requeued for next idle slot");

    df_bus_clear();
    df_event_type_registry_clear();
}

// ===== K10.2 Item 31 — background queue save-integrated storage =====

void scenario_bg_queue_serialize_deserialize_roundtrip() {
    std::printf("scenario_bg_queue_serialize_deserialize_roundtrip\n");
    df_bus_clear();
    df_event_type_registry_clear();

    const uint32_t type_id = 0xC0FFEE10u;
    df_event_type_registry_register(
        type_id, /*Background*/2, sizeof(int32_t), "Test.Bg.Save", coalesce_sum_int32_bg);
    df_bus_subscribe_background(type_id, /*mod_id=*/800u, &test_background_subscriber, nullptr);

    // Publish 3 events с unique coalesce keys
    int32_t v1 = 11, v2 = 22, v3 = 33;
    df_bus_publish_background(type_id, &v1, sizeof(v1), 10u);
    df_bus_publish_background(type_id, &v2, sizeof(v2), 20u);
    df_bus_publish_background(type_id, &v3, sizeof(v3), 30u);

    // Compute size + serialize
    uint32_t required = 0;
    DF_CHECK(df_background_queue_compute_save_size(&required) == 1, "compute size succeeds");
    DF_CHECK(required >= 12u, "header at minimum 12 bytes"); // header

    std::vector<uint8_t> buffer(required);
    uint32_t written = 0;
    DF_CHECK(df_background_queue_serialize(buffer.data(), required, &written) == 1, "serialize succeeds");
    DF_CHECK(written == required, "bytes written matches computed size");

    // Drain (verify queue is empty)
    df_bus_clear();
    uint32_t after_clear_count = 0;
    df_background_queue_size(&after_clear_count, nullptr);
    DF_CHECK(after_clear_count == 0, "queue empty after clear");

    // Re-subscribe (clear wipes subscribers too); register event type
    df_event_type_registry_register(
        type_id, /*Background*/2, sizeof(int32_t), "Test.Bg.Save", coalesce_sum_int32_bg);
    df_bus_subscribe_background(type_id, /*mod_id=*/800u, &test_background_subscriber, nullptr);

    // Deserialize
    uint32_t loaded = 0;
    DF_CHECK(df_background_queue_deserialize(buffer.data(), written, &loaded) == 1, "deserialize succeeds");
    DF_CHECK(loaded == 3, "3 events loaded");

    uint32_t after_load_count = 0;
    df_background_queue_size(&after_load_count, nullptr);
    DF_CHECK(after_load_count == 3, "3 events in queue после load");

    df_bus_clear();
    df_event_type_registry_clear();
}

void scenario_bg_queue_deserialize_schema_mismatch_rejected() {
    std::printf("scenario_bg_queue_deserialize_schema_mismatch_rejected\n");
    df_bus_clear();

    // Header с schema version 999 (unsupported) + zero events
    uint8_t buffer[12] = {0};
    buffer[0] = 0xE7;  // 999 little-endian = 0x03E7
    buffer[1] = 0x03;
    // event_count + total_payload_bytes already 0

    uint32_t loaded = 999;
    int32_t rc = df_background_queue_deserialize(buffer, sizeof(buffer), &loaded);
    DF_CHECK(rc == 0, "deserialize fails on schema version mismatch");

    df_bus_clear();
}

void scenario_bg_queue_deserialize_malformed_returns_zero() {
    std::printf("scenario_bg_queue_deserialize_malformed_returns_zero\n");
    df_bus_clear();

    // Buffer too short для header
    uint8_t short_buf[4] = {0};
    uint32_t loaded = 999;
    int32_t rc = df_background_queue_deserialize(short_buf, sizeof(short_buf), &loaded);
    DF_CHECK(rc == 0, "deserialize fails when buffer truncated");

    df_bus_clear();
}

void scenario_bg_queue_saturation_drop_oldest() {
    std::printf("scenario_bg_queue_saturation_drop_oldest\n");
    df_bus_clear();
    df_event_type_registry_clear();

    const uint32_t type_id = 0xBA110004u;
    df_event_type_registry_register(
        type_id, /*Background*/2, sizeof(int32_t), "Test.Bg.Sat", coalesce_sum_int32_bg);
    df_bus_subscribe_background(type_id, /*mod_id=*/703u, &test_background_subscriber, nullptr);

    // Configure 20-byte cap (5 events × 4 bytes each)
    int32_t cfg_rc = df_background_queue_configure(20u, 16u, DF_BG_QUEUE_DROP_OLDEST);
    DF_CHECK(cfg_rc == 1, "configure with 20-byte cap succeeds");

    // Publish 8 events (32 bytes total — exceeds cap by 12 bytes)
    int32_t before_sat = df_background_queue_saturation_events();
    for (int32_t i = 0; i < 8; ++i) {
        int32_t v = i + 1;
        df_bus_publish_background(type_id, &v, sizeof(v), static_cast<uint32_t>(i + 200));
    }
    // Force coalesce + apply saturation policy
    df_background_queue_force_coalesce();
    int32_t after_sat = df_background_queue_saturation_events();
    DF_CHECK(after_sat - before_sat == 3, "3 events dropped по drop-oldest");

    uint32_t remaining = 0;
    df_background_queue_size(&remaining, nullptr);
    DF_CHECK(remaining == 5, "5 events remain after saturation policy");

    // Restore defaults для other tests
    df_background_queue_configure(10u * 1024u * 1024u, 8u * 1024u * 1024u, DF_BG_QUEUE_DROP_OLDEST);
    df_bus_clear();
    df_event_type_registry_clear();
}

// ===== K10.2 Item 32 — native unload primitive + ModUnloadResult =====

void scenario_mod_unload_native_state_t0_t7_sequence() {
    std::printf("scenario_mod_unload_native_state_t0_t7_sequence\n");
    df_bus_clear();
    df_event_type_registry_clear();
    df_scheduler_set_sim_paused(1);

    const uint32_t mod_id = 900u;
    const uint32_t type_id_fast = 0xC0FF0001u;
    const uint32_t type_id_normal = 0xC0FF0002u;
    const uint32_t type_id_bg = 0xC0FF0003u;

    df_event_type_registry_register(type_id_fast, 0, sizeof(int32_t), "Test.U.Fast", nullptr);
    df_event_type_registry_register(type_id_normal, 1, sizeof(int32_t), "Test.U.Normal", nullptr);
    df_event_type_registry_register(type_id_bg, 2, sizeof(int32_t), "Test.U.Bg", coalesce_sum_int32_bg);

    df_bus_subscribe_fast(type_id_fast, mod_id, &test_fast_subscriber, nullptr);
    df_bus_subscribe_normal(type_id_normal, mod_id, &test_normal_subscriber, nullptr);
    df_bus_subscribe_background(type_id_bg, mod_id, &test_normal_subscriber, nullptr);

    // Publish a normal event (will be drained during T2)
    int32_t v = 42;
    df_bus_publish_normal(type_id_normal, &v, sizeof(v));

    // Publish a background event (preserved during T3 per S3-Q3/Q4)
    df_bus_publish_background(type_id_bg, &v, sizeof(v), 1u);

    ModUnloadResult result{};
    int32_t rc = df_scheduler_unload_mod_native_state(mod_id, &result);
    DF_CHECK(rc == 1 && result.success == 1, "T0-T7 sequence completes");
    DF_CHECK(result.fast_subscriptions_cleared == 1, "T1: 1 fast sub cleared");
    DF_CHECK(result.normal_subscriptions_cleared == 1, "T2: 1 normal sub cleared");
    DF_CHECK(result.normal_batch_commit_completed == 1, "T2: batch commit");
    DF_CHECK(result.normal_events_drained == 1, "T2: 1 event drained");
    DF_CHECK(result.background_subscriptions_cleared == 1, "T3: 1 bg sub cleared");
    DF_CHECK(result.background_events_preserved == 1,
             "T3: 1 bg event preserved (S3-Q3/Q4 untargeted persistence)");
    DF_CHECK(result.error_count == 0, "no errors");

    // Verify bus subscribers actually cleared
    DF_CHECK(df_bus_subscriber_count_fast(type_id_fast) == 0, "fast bus empty post-unload");
    DF_CHECK(df_bus_subscriber_count_normal(type_id_normal) == 0, "normal bus empty post-unload");
    DF_CHECK(df_bus_subscriber_count_background(type_id_bg) == 0, "background subs empty post-unload");

    df_bus_clear();
    df_event_type_registry_clear();
}

void scenario_mod_unload_quiescent_precondition_violation() {
    std::printf("scenario_mod_unload_quiescent_precondition_violation\n");
    df_bus_clear();

    // К-L18 violation: sim is running
    df_scheduler_set_sim_paused(0);

    ModUnloadResult result{};
    int32_t rc = df_scheduler_unload_mod_native_state(123u, &result);
    DF_CHECK(rc == 0, "unload fails when sim is not paused");
    DF_CHECK(result.success == 0, "ModUnloadResult.success = 0");
    DF_CHECK(result.error_count >= 1, "error message recorded");

    // Restore paused for subsequent scenarios
    df_scheduler_set_sim_paused(1);
    df_bus_clear();
}

void scenario_mod_unload_no_subscriptions_succeeds_vacuously() {
    std::printf("scenario_mod_unload_no_subscriptions_succeeds_vacuously\n");
    df_bus_clear();
    df_scheduler_set_sim_paused(1);

    ModUnloadResult result{};
    int32_t rc = df_scheduler_unload_mod_native_state(/*unknown_mod_id=*/0xFFFFFFFFu, &result);
    DF_CHECK(rc == 1 && result.success == 1, "vacuous unload succeeds");
    DF_CHECK(result.fast_subscriptions_cleared == 0, "0 fast subs cleared");
    DF_CHECK(result.normal_subscriptions_cleared == 0, "0 normal subs cleared");
    DF_CHECK(result.background_subscriptions_cleared == 0, "0 bg subs cleared");

    df_bus_clear();
}

// ===== K10.3 v2 Item 41 — К-L18 quiescent state enforcement (pipeline slot) =====

void scenario_mod_unload_fails_when_pipeline_has_dispatched_slot() {
    std::printf("scenario_mod_unload_fails_when_pipeline_has_dispatched_slot\n");
    df_bus_clear();
    df_scheduler_set_sim_paused(1);
    df_pipeline_reset();
    df_pipeline_init(2);

    // Allocate slot → Dispatched state. Pipeline is не quiescent.
    PipelineSlot* slot = nullptr;
    df_pipeline_allocate_slot(500, &slot);

    int32_t quiescent = -1;
    DF_CHECK(df_pipeline_is_quiescent(&quiescent) == 1 && quiescent == 0,
             "pipeline reports not quiescent под Dispatched slot");

    ModUnloadResult result{};
    int32_t rc = df_scheduler_unload_mod_native_state(700u, &result);
    DF_CHECK(rc == 0, "unload fails when pipeline has Dispatched slot");
    DF_CHECK(result.success == 0, "ModUnloadResult.success = 0");
    DF_CHECK(result.error_count >= 1, "К-L18 pipeline error message recorded");

    df_pipeline_reset();
    df_bus_clear();
}

void scenario_mod_unload_fails_when_pipeline_has_fence_completed_slot() {
    std::printf("scenario_mod_unload_fails_when_pipeline_has_fence_completed_slot\n");
    df_bus_clear();
    df_scheduler_set_sim_paused(1);
    df_pipeline_reset();
    df_pipeline_init(2);

    // Dispatched → force fence → FenceCompleted (still in-flight per К-L18).
    PipelineSlot* slot = nullptr;
    df_pipeline_allocate_slot(501, &slot);
    df_pipeline_force_fence_completed(slot);

    int32_t quiescent = -1;
    DF_CHECK(df_pipeline_is_quiescent(&quiescent) == 1 && quiescent == 0,
             "pipeline reports not quiescent под FenceCompleted slot");

    ModUnloadResult result{};
    int32_t rc = df_scheduler_unload_mod_native_state(701u, &result);
    DF_CHECK(rc == 0, "unload fails when pipeline has FenceCompleted slot");
    DF_CHECK(result.success == 0, "ModUnloadResult.success = 0");

    df_pipeline_reset();
    df_bus_clear();
}

void scenario_mod_unload_succeeds_when_pipeline_quiescent_after_tail_transition() {
    std::printf("scenario_mod_unload_succeeds_when_pipeline_quiescent_after_tail_transition\n");
    df_bus_clear();
    df_scheduler_set_sim_paused(1);
    df_pipeline_reset();
    df_pipeline_init(2);

    // Cycle slot through full state machine to ReadableAsTail → quiescent.
    PipelineSlot* slot = nullptr;
    df_pipeline_allocate_slot(502, &slot);
    df_pipeline_force_fence_completed(slot);
    df_pipeline_transition_to_tail(slot);

    int32_t quiescent = -1;
    DF_CHECK(df_pipeline_is_quiescent(&quiescent) == 1 && quiescent == 1,
             "pipeline reports quiescent после ReadableAsTail");

    ModUnloadResult result{};
    int32_t rc = df_scheduler_unload_mod_native_state(702u, &result);
    DF_CHECK(rc == 1 && result.success == 1, "unload succeeds when pipeline quiescent");
    DF_CHECK(result.error_count == 0, "no errors");

    df_pipeline_reset();
    df_bus_clear();
}

void scenario_mod_unload_succeeds_when_pipeline_uninitialized() {
    std::printf("scenario_mod_unload_succeeds_when_pipeline_uninitialized\n");
    df_bus_clear();
    df_scheduler_set_sim_paused(1);
    df_pipeline_reset();
    // Не init the pipeline.

    int32_t quiescent = -1;
    // df_pipeline_is_quiescent returns -1 when depth==0 — mod_unload treats as quiescent.
    DF_CHECK(df_pipeline_is_quiescent(&quiescent) == -1, "is_quiescent returns -1 pre-init");

    ModUnloadResult result{};
    int32_t rc = df_scheduler_unload_mod_native_state(703u, &result);
    DF_CHECK(rc == 1 && result.success == 1,
             "unload succeeds when pipeline не initialized (no in-flight compute)");
    DF_CHECK(result.error_count == 0, "no errors");

    df_bus_clear();
}

void scenario_v0b_compute_pipeline_registration_roundtrip() {
    std::printf("scenario_v0b_compute_pipeline_registration_roundtrip\n");
    df_world_handle world = df_world_create();
    DF_CHECK(world != nullptr, "world created");

    // V1+: with а real VkDevice attached, register_pipeline calls
    // vkCreateShaderModule / vkCreateComputePipelines on the SPIR-V. The
    // selftest does не bring up а live Vulkan instance, so only the negative
    // "rejected without attach" path is exercised here. The positive path is
    // covered by C# FieldStorageBindingTests which constructs а real Vulkan
    // instance + device per К-L19 hardware tier.

    // Without Vulkan attached, registration is rejected.
    const uint8_t fake_spirv[] = {0x03, 0x02, 0x23, 0x07, 0x00, 0x00, 0x01, 0x00};
    uint32_t pid = df_world_register_compute_pipeline(world, "noop", fake_spirv, sizeof(fake_spirv), 0, 0);
    DF_CHECK(pid == 0, "register without attach rejected");

    // Misaligned SPIR-V rejected без needing attach (size check is unconditional).
    const uint8_t bad_spirv[] = {0x01, 0x02, 0x03};
    uint32_t pid3 = df_world_register_compute_pipeline(world, "other", bad_spirv, sizeof(bad_spirv), 0, 0);
    DF_CHECK(pid3 == 0, "misaligned SPIR-V rejected");

    // Dispatch against unknown pipeline_id returns 0 (does не require attach
    // since the world rejects all dispatches when not attached).
    int32_t disp = df_world_field_dispatch_compute(world, "test_field", 0xDEADBEEF, nullptr, 0, 1, 1, 1);
    DF_CHECK(disp == 0, "dispatch against unknown pipeline_id fails (no attach)");

    df_world_destroy(world);
}

void scenario_bus_per_mod_bulk_unsubscribe() {
    std::printf("scenario_bus_per_mod_bulk_unsubscribe\n");
    df_bus_clear();

    const uint32_t type_a = 0xAA110001u;
    const uint32_t type_b = 0xAA110002u;

    // Mod 500 subscribes Fast (type_a) + Normal (type_b) + Background (type_a)
    df_bus_subscribe_fast(type_a, /*mod_id=*/500u, &test_fast_subscriber, nullptr);
    df_bus_subscribe_normal(type_b, /*mod_id=*/500u, &test_normal_subscriber, nullptr);
    df_bus_subscribe_background(type_a, /*mod_id=*/500u, &test_normal_subscriber, nullptr);
    // Mod 501 subscribes Fast (type_a) only — should NOT be reaped
    df_bus_subscribe_fast(type_a, /*mod_id=*/501u, &test_fast_subscriber, nullptr);

    DF_CHECK(df_bus_subscriber_count_fast(type_a) == 2, "before bulk: 2 fast subs on type_a");

    int32_t removed_fast = df_bus_unsubscribe_fast_by_mod(500u);
    int32_t removed_normal = df_bus_unsubscribe_normal_by_mod(500u);
    int32_t removed_bg = df_bus_unsubscribe_background_by_mod(500u);

    DF_CHECK(removed_fast == 1, "1 fast subscription removed for mod 500");
    DF_CHECK(removed_normal == 1, "1 normal subscription removed for mod 500");
    DF_CHECK(removed_bg == 1, "1 background subscription removed for mod 500");
    DF_CHECK(df_bus_subscriber_count_fast(type_a) == 1, "mod 501 fast subscription preserved");

    df_bus_clear();
}

void scenario_event_type_registry_invalid_args() {
    std::printf("scenario_event_type_registry_invalid_args\n");
    df_event_type_registry_clear();

    // Null FQN fails
    int32_t rc = df_event_type_registry_register(0x6000u, 1, 4, nullptr, nullptr);
    DF_CHECK(rc == 0, "null fqn fails");

    // Out-of-range tier fails
    rc = df_event_type_registry_register(0x6001u, 99, 4, "Test", nullptr);
    DF_CHECK(rc == 0, "out-of-range tier fails");

    rc = df_event_type_registry_register(0x6002u, -1, 4, "Test", nullptr);
    DF_CHECK(rc == 0, "negative tier fails");

    DF_CHECK(df_event_type_registry_count() == 0,
             "invalid registrations not persisted");
    df_event_type_registry_clear();
}

// ===== K10.3 v2 Item 33 scenarios — pipeline_slot state machine =====
//
// Pipeline depth (К-L16 D=2 default), К-L7.1 sub-invariant binding (slot tail
// reads), К-L18 forward dependency (df_pipeline_is_quiescent). Per Lesson #22
// DF_CHECK runner convention preserved.

void scenario_pipeline_slot_init_and_depth() {
    std::printf("scenario_pipeline_slot_init_and_depth\n");
    df_pipeline_reset();

    int32_t depth_out = -1;
    DF_CHECK(df_pipeline_get_depth(&depth_out) == 1, "get_depth returns ok pre-init");
    DF_CHECK(depth_out == 0, "depth=0 before init");

    // К-L16 D=1-3 range; out-of-range fails.
    DF_CHECK(df_pipeline_init(0) == 0, "init depth=0 rejected");
    DF_CHECK(df_pipeline_init(4) == 0, "init depth=4 rejected (max=3)");

    // Default D=2 per S-LOCK-4.
    DF_CHECK(df_pipeline_init(DF_PIPELINE_DEFAULT_DEPTH) == 1, "init D=2 succeeds");
    DF_CHECK(df_pipeline_get_depth(&depth_out) == 1, "get_depth after init");
    DF_CHECK(depth_out == 2, "depth=2 after init D=2");

    // Re-init без reset fails (idempotency through error per К10.2 pattern).
    DF_CHECK(df_pipeline_init(3) == 0, "re-init без reset rejected");

    df_pipeline_reset();
    DF_CHECK(df_pipeline_get_depth(&depth_out) == 1 && depth_out == 0,
             "reset returns depth к 0");
}

void scenario_pipeline_slot_state_machine_cycle() {
    std::printf("scenario_pipeline_slot_state_machine_cycle\n");
    df_pipeline_reset();
    df_pipeline_init(2);

    PipelineSlot* slot = nullptr;
    DF_CHECK(df_pipeline_allocate_slot(100, &slot) == 1, "allocate slot tick=100");
    DF_CHECK(slot != nullptr, "slot pointer non-null");
    DF_CHECK(slot->sim_tick == 100, "sim_tick assigned");
    DF_CHECK(slot->state == SlotState_Dispatched, "state=Dispatched after allocate");

    // Fence handle binding.
    void* fake_fence = reinterpret_cast<void*>(static_cast<uintptr_t>(0xDEADBEEFu));
    DF_CHECK(df_pipeline_set_fence(slot, fake_fence) == 1, "set_fence ok");
    DF_CHECK(slot->compute_fence_handle == fake_fence, "fence handle stored");

    // Transition Dispatched → FenceCompleted (test path via force_fence_completed
    // until Commit 4 wires real vkGetFenceStatus в df_pipeline_check_fences).
    DF_CHECK(df_pipeline_force_fence_completed(slot) == 1,
             "force_fence_completed transitions к FenceCompleted");
    DF_CHECK(slot->state == SlotState_FenceCompleted, "state=FenceCompleted");

    // Transition FenceCompleted → ReadableAsTail.
    DF_CHECK(df_pipeline_transition_to_tail(slot) == 1, "transition_to_tail ok");
    DF_CHECK(slot->state == SlotState_ReadableAsTail, "state=ReadableAsTail");

    // Invalid transitions: transition_to_tail again fails (already in tail state).
    DF_CHECK(df_pipeline_transition_to_tail(slot) == 0,
             "transition_to_tail rejected from ReadableAsTail");

    df_pipeline_reset();
}

void scenario_pipeline_slot_d2_default_cycling() {
    std::printf("scenario_pipeline_slot_d2_default_cycling\n");
    df_pipeline_reset();
    df_pipeline_init(2);

    // Allocate 2 slots — both succeed, fill the cycle.
    PipelineSlot* s0 = nullptr;
    PipelineSlot* s1 = nullptr;
    DF_CHECK(df_pipeline_allocate_slot(200, &s0) == 1, "tick=200 allocate");
    DF_CHECK(df_pipeline_allocate_slot(201, &s1) == 1, "tick=201 allocate");
    DF_CHECK(s0 != s1, "different slot pointers");

    // Drain both slots through full state machine.
    df_pipeline_force_fence_completed(s0);
    df_pipeline_transition_to_tail(s0);
    df_pipeline_force_fence_completed(s1);
    df_pipeline_transition_to_tail(s1);

    DF_CHECK(s0->state == SlotState_ReadableAsTail, "s0 ReadableAsTail");
    DF_CHECK(s1->state == SlotState_ReadableAsTail, "s1 ReadableAsTail");

    // Third allocation cycles back к slot 0 (recycle).
    PipelineSlot* s2 = nullptr;
    DF_CHECK(df_pipeline_allocate_slot(202, &s2) == 1, "tick=202 allocate (recycle)");
    DF_CHECK(s2 == s0, "recycles slot 0");
    DF_CHECK(s2->sim_tick == 202, "recycled slot has new sim_tick");
    DF_CHECK(s2->state == SlotState_Dispatched, "recycled slot in Dispatched");

    df_pipeline_reset();
}

void scenario_pipeline_slot_get_slot_offsets() {
    std::printf("scenario_pipeline_slot_get_slot_offsets\n");
    df_pipeline_reset();
    df_pipeline_init(2);

    // No allocations — get_slot returns 0.
    PipelineSlot* out = nullptr;
    DF_CHECK(df_pipeline_get_slot(0, &out) == 0, "no slot returned pre-allocate");

    PipelineSlot* s0 = nullptr;
    PipelineSlot* s1 = nullptr;
    df_pipeline_allocate_slot(300, &s0);
    df_pipeline_allocate_slot(301, &s1);

    // get_slot(0) = most-recently-allocated.
    DF_CHECK(df_pipeline_get_slot(0, &out) == 1 && out == s1, "offset 0 = current");
    DF_CHECK(out->sim_tick == 301, "current sim_tick=301");

    // get_slot(-1) = tail (К-L7.1 sim-thread read pattern, T-1).
    DF_CHECK(df_pipeline_get_slot(-1, &out) == 1 && out == s0, "offset -1 = tail");
    DF_CHECK(out->sim_tick == 300, "tail sim_tick=300");

    // Out-of-range offsets rejected.
    DF_CHECK(df_pipeline_get_slot(1, &out) == 0, "positive offset rejected");
    DF_CHECK(df_pipeline_get_slot(-2, &out) == 0, "offset -D rejected (D=2)");

    df_pipeline_reset();
}

void scenario_pipeline_slot_is_quiescent() {
    std::printf("scenario_pipeline_slot_is_quiescent\n");
    df_pipeline_reset();

    // Pre-init: not quiescent + return code -1.
    int32_t quiescent = -1;
    DF_CHECK(df_pipeline_is_quiescent(&quiescent) == -1,
             "is_quiescent rejects pre-init");

    df_pipeline_init(2);

    // Just initialized: all slots Empty → quiescent.
    DF_CHECK(df_pipeline_is_quiescent(&quiescent) == 1, "is_quiescent succeeds");
    DF_CHECK(quiescent == 1, "quiescent=1 after init (all Empty)");

    // Allocate slot — Dispatched state → not quiescent.
    PipelineSlot* s = nullptr;
    df_pipeline_allocate_slot(400, &s);
    DF_CHECK(df_pipeline_is_quiescent(&quiescent) == 1 && quiescent == 0,
             "not quiescent с Dispatched slot");

    // Fence completed → still not quiescent (FenceCompleted = in-flight per К-L18).
    df_pipeline_force_fence_completed(s);
    DF_CHECK(df_pipeline_is_quiescent(&quiescent) == 1 && quiescent == 0,
             "not quiescent с FenceCompleted slot");

    // Transition к tail → quiescent (ReadableAsTail = work done, slot drained).
    df_pipeline_transition_to_tail(s);
    DF_CHECK(df_pipeline_is_quiescent(&quiescent) == 1 && quiescent == 1,
             "quiescent после ReadableAsTail");

    df_pipeline_reset();
}

// ===== K10.3 v2 Item 36 scenarios — pipeline slot read API (К-L7.1 opt-in) =====

void scenario_pipeline_read_slot_tail_K_L7_1() {
    std::printf("scenario_pipeline_read_slot_tail_K_L7_1\n");
    df_pipeline_reset();
    df_pipeline_init(2);

    PipelineSlot* slot = nullptr;
    df_pipeline_allocate_slot(1000, &slot);

    // Bind fields_snapshot_ptr (К-L7.1 binding subject).
    void* fake_fields = reinterpret_cast<void*>(static_cast<uintptr_t>(0xCAFE0001u));
    slot->fields_snapshot_ptr = fake_fields;

    // Dispatched state → read_slot_tail returns 0 (fence не signaled).
    void* snapshot_out = nullptr;
    uint64_t tick_out = 0;
    DF_CHECK(df_pipeline_read_slot_tail(0, &snapshot_out, &tick_out) == 0,
             "Dispatched slot rejects read (fence не signaled)");
    DF_CHECK(snapshot_out == nullptr, "snapshot null after reject");

    // Transition к ReadableAsTail.
    df_pipeline_force_fence_completed(slot);
    df_pipeline_transition_to_tail(slot);

    // Now read succeeds.
    DF_CHECK(df_pipeline_read_slot_tail(0, &snapshot_out, &tick_out) == 1,
             "ReadableAsTail slot allows read");
    DF_CHECK(snapshot_out == fake_fields, "fields_snapshot_ptr roundtrip");
    DF_CHECK(tick_out == 1000, "sim_tick roundtrip");

    // К-L7.1 sim-thread read pattern: offset=-1 = tail (one-tick lag).
    PipelineSlot* slot2 = nullptr;
    df_pipeline_allocate_slot(1001, &slot2);
    slot2->fields_snapshot_ptr = reinterpret_cast<void*>(static_cast<uintptr_t>(0xCAFE0002u));
    // slot is now offset=-1 (tail per К-L7.1).
    DF_CHECK(df_pipeline_read_slot_tail(-1, &snapshot_out, &tick_out) == 1,
             "tail read (offset=-1) succeeds");
    DF_CHECK(snapshot_out == fake_fields, "tail = prior slot snapshot");
    DF_CHECK(tick_out == 1000, "tail = prior sim_tick (К-L7.1 one-tick lag)");

    df_pipeline_reset();
}

// ===== K10.3 v2 Item 37 scenarios — slot transition wake (К-L13 extension) =====

void scenario_pipeline_transition_fires_wake() {
    std::printf("scenario_pipeline_transition_fires_wake\n");
    df_pipeline_reset();
    df_pipeline_init(2);
    df_pipeline_reset_wake_fire_count();

    int32_t count = -1;
    DF_CHECK(df_pipeline_get_wake_fire_count(&count) == 1 && count == 0,
             "wake fire count = 0 initially");

    // Allocate + dispatch 2 slots, transition both к ReadableAsTail.
    PipelineSlot* s0 = nullptr;
    PipelineSlot* s1 = nullptr;
    df_pipeline_allocate_slot(1400, &s0);
    df_pipeline_allocate_slot(1401, &s1);

    // Transition s0 — fires wake (count → 1).
    df_pipeline_force_fence_completed(s0);
    df_pipeline_transition_to_tail(s0);
    df_pipeline_get_wake_fire_count(&count);
    DF_CHECK(count == 1, "wake fire count = 1 after first transition");

    // Transition s1 — fires wake (count → 2).
    df_pipeline_force_fence_completed(s1);
    df_pipeline_transition_to_tail(s1);
    df_pipeline_get_wake_fire_count(&count);
    DF_CHECK(count == 2, "wake fire count = 2 after second transition");

    // Failed transition (slot не в FenceCompleted) → wake не fires.
    PipelineSlot* s2 = nullptr;
    df_pipeline_allocate_slot(1402, &s2);  // Dispatched, не FenceCompleted
    DF_CHECK(df_pipeline_transition_to_tail(s2) == 0, "transition rejected от Dispatched");
    df_pipeline_get_wake_fire_count(&count);
    DF_CHECK(count == 2, "wake count unchanged on failed transition");

    df_pipeline_reset();
}

// ===== K10.3 v2 Item 34 scenarios — pipeline drain/refill protocols =====

void scenario_pipeline_drain_pause_resume() {
    std::printf("scenario_pipeline_drain_pause_resume\n");
    df_pipeline_reset();
    df_pipeline_init(2);

    // Before pause — allocation succeeds.
    PipelineSlot* s0 = nullptr;
    DF_CHECK(df_pipeline_allocate_slot(1100, &s0) == 1, "alloc before pause");

    int32_t paused = -1;
    DF_CHECK(df_pipeline_is_paused(&paused) == 1 && paused == 0, "not paused initially");

    DF_CHECK(df_pipeline_pause() == 1, "pause ok");
    DF_CHECK(df_pipeline_is_paused(&paused) == 1 && paused == 1, "now paused");

    // Allocation rejected while paused.
    PipelineSlot* s1 = nullptr;
    DF_CHECK(df_pipeline_allocate_slot(1101, &s1) == 0, "alloc rejected while paused");

    // Existing in-flight slot still drains naturally (force_fence_completed +
    // transition_to_tail).
    df_pipeline_force_fence_completed(s0);
    df_pipeline_transition_to_tail(s0);

    DF_CHECK(df_pipeline_resume() == 1, "resume ok");
    DF_CHECK(df_pipeline_is_paused(&paused) == 1 && paused == 0, "resumed");
    DF_CHECK(df_pipeline_allocate_slot(1102, &s1) == 1, "alloc accepted after resume");

    df_pipeline_reset();
}

void scenario_pipeline_serialize_deserialize_roundtrip() {
    std::printf("scenario_pipeline_serialize_deserialize_roundtrip\n");
    df_pipeline_reset();
    df_pipeline_init(2);

    // Populate slots с distinct states.
    PipelineSlot* s0 = nullptr;
    PipelineSlot* s1 = nullptr;
    df_pipeline_allocate_slot(1200, &s0);
    df_pipeline_allocate_slot(1201, &s1);
    df_pipeline_force_fence_completed(s0);
    df_pipeline_transition_to_tail(s0);  // s0 = ReadableAsTail
    // s1 = Dispatched

    // Serialize.
    uint8_t buffer[DF_PIPELINE_SNAPSHOT_MAX_SIZE];
    int32_t bytes_written = 0;
    DF_CHECK(df_pipeline_serialize_display_state(buffer, sizeof(buffer), &bytes_written) == 1,
             "serialize ok");
    DF_CHECK(bytes_written == DF_PIPELINE_SNAPSHOT_HEADER_SIZE +
             2 * DF_PIPELINE_SNAPSHOT_PER_SLOT_SIZE,
             "expected snapshot size для D=2");

    // Reset + re-init с matching depth + deserialize.
    df_pipeline_reset();
    df_pipeline_init(2);
    DF_CHECK(df_pipeline_deserialize_display_state(buffer, bytes_written) == 1,
             "deserialize ok");

    // After deserialize, cursor points к (max_sim_tick % depth) + 1 = (1201 % 2) + 1 = 2.
    // Most recent slot index = (cursor-1) % depth = 1 % 2 = 1, so s1 (sim_tick=1201) at
    // offset 0, s0 (sim_tick=1200) at offset -1.
    PipelineSlot* out = nullptr;
    DF_CHECK(df_pipeline_get_slot(0, &out) == 1, "offset 0 ok after restore");
    DF_CHECK(out->sim_tick == 1201, "current slot sim_tick=1201 restored");
    DF_CHECK(out->state == SlotState_Dispatched, "current slot state=Dispatched restored");

    DF_CHECK(df_pipeline_get_slot(-1, &out) == 1, "offset -1 ok after restore");
    DF_CHECK(out->sim_tick == 1200, "tail slot sim_tick=1200 restored");
    DF_CHECK(out->state == SlotState_ReadableAsTail, "tail slot state=ReadableAsTail restored");

    df_pipeline_reset();
}

void scenario_pipeline_deserialize_depth_mismatch_rejected() {
    std::printf("scenario_pipeline_deserialize_depth_mismatch_rejected\n");
    df_pipeline_reset();
    df_pipeline_init(3);  // Init D=3.

    PipelineSlot* slot = nullptr;
    df_pipeline_allocate_slot(1300, &slot);
    uint8_t buffer[DF_PIPELINE_SNAPSHOT_MAX_SIZE];
    int32_t bytes_written = 0;
    df_pipeline_serialize_display_state(buffer, sizeof(buffer), &bytes_written);

    // Reset + init с DIFFERENT depth = 2; deserialize rejected.
    df_pipeline_reset();
    df_pipeline_init(2);
    DF_CHECK(df_pipeline_deserialize_display_state(buffer, bytes_written) == 0,
             "deserialize rejects depth mismatch (saved=3, current=2)");

    df_pipeline_reset();
}

// ===== K10.3 v2 Item 35 scenarios — Phase.Compute scheduler integration =====
//
// Phase enum (Update/Compute/Display); VkQueueSubmit batching coalesces
// dispatches per slot per tick. S-LOCK-13 coexistence: V1 sync path orthogonal.

void scenario_phase_compute_registration_and_count() {
    std::printf("scenario_phase_compute_registration_and_count\n");
    df_pipeline_reset();
    df_pipeline_init(2);
    df_scheduler_phase_compute_reset();

    PipelineSlot* slot = nullptr;
    df_pipeline_allocate_slot(600, &slot);
    DF_CHECK(df_scheduler_compute_dispatch_count(slot) == 0, "no dispatches initially");

    // Register 3 dispatches against slot.
    void* fake_pipeline = reinterpret_cast<void*>(static_cast<uintptr_t>(0x1000u));
    void* fake_descriptor = reinterpret_cast<void*>(static_cast<uintptr_t>(0x2000u));
    DF_CHECK(df_scheduler_register_compute_dispatch(slot, fake_pipeline, fake_descriptor, 16, 16, 1) == 1,
             "register dispatch 1");
    DF_CHECK(df_scheduler_register_compute_dispatch(slot, fake_pipeline, fake_descriptor, 32, 32, 1) == 1,
             "register dispatch 2");
    DF_CHECK(df_scheduler_register_compute_dispatch(slot, fake_pipeline, fake_descriptor, 64, 64, 1) == 1,
             "register dispatch 3");
    DF_CHECK(df_scheduler_compute_dispatch_count(slot) == 3,
             "3 dispatches registered против slot");

    df_scheduler_phase_compute_reset();
    df_pipeline_reset();
}

void scenario_phase_compute_dispatch_empty_slot() {
    std::printf("scenario_phase_compute_dispatch_empty_slot\n");
    df_pipeline_reset();
    df_pipeline_init(2);
    df_scheduler_phase_compute_reset();

    PipelineSlot* slot = nullptr;
    df_pipeline_allocate_slot(700, &slot);

    // No dispatches registered против slot — dispatch_phase_compute returns ok
    // (vacuous submit, slot still Dispatched).
    DF_CHECK(df_scheduler_dispatch_phase_compute(slot) == 1, "dispatch ok с 0 records");
    DF_CHECK(slot->state == SlotState_Dispatched, "slot still Dispatched after empty dispatch");

    df_scheduler_phase_compute_reset();
    df_pipeline_reset();
}

void scenario_phase_compute_batch_coalesce_per_slot() {
    std::printf("scenario_phase_compute_batch_coalesce_per_slot\n");
    df_pipeline_reset();
    df_pipeline_init(2);
    df_scheduler_phase_compute_reset();

    PipelineSlot* s0 = nullptr;
    PipelineSlot* s1 = nullptr;
    df_pipeline_allocate_slot(800, &s0);
    df_pipeline_allocate_slot(801, &s1);

    // 2 dispatches на s0, 3 dispatches на s1.
    void* fake_pipeline = reinterpret_cast<void*>(static_cast<uintptr_t>(0x3000u));
    void* fake_descriptor = reinterpret_cast<void*>(static_cast<uintptr_t>(0x4000u));
    df_scheduler_register_compute_dispatch(s0, fake_pipeline, fake_descriptor, 8, 8, 1);
    df_scheduler_register_compute_dispatch(s0, fake_pipeline, fake_descriptor, 8, 8, 1);
    df_scheduler_register_compute_dispatch(s1, fake_pipeline, fake_descriptor, 16, 16, 1);
    df_scheduler_register_compute_dispatch(s1, fake_pipeline, fake_descriptor, 16, 16, 1);
    df_scheduler_register_compute_dispatch(s1, fake_pipeline, fake_descriptor, 16, 16, 1);

    DF_CHECK(df_scheduler_compute_dispatch_count(s0) == 2, "s0 has 2 dispatches");
    DF_CHECK(df_scheduler_compute_dispatch_count(s1) == 3, "s1 has 3 dispatches");

    // submit_compute_batch reports per-slot count.
    void* fake_queue = reinterpret_cast<void*>(static_cast<uintptr_t>(0x5000u));
    DF_CHECK(df_scheduler_submit_compute_batch(s0, fake_queue) == 2, "s0 batch coalesces 2");
    DF_CHECK(df_scheduler_submit_compute_batch(s1, fake_queue) == 3, "s1 batch coalesces 3");

    df_scheduler_phase_compute_reset();
    df_pipeline_reset();
}

void scenario_phase_compute_reset_clears_registry() {
    std::printf("scenario_phase_compute_reset_clears_registry\n");
    df_pipeline_reset();
    df_pipeline_init(2);
    df_scheduler_phase_compute_reset();

    PipelineSlot* slot = nullptr;
    df_pipeline_allocate_slot(900, &slot);

    void* fake_pipeline = reinterpret_cast<void*>(static_cast<uintptr_t>(0x6000u));
    void* fake_descriptor = reinterpret_cast<void*>(static_cast<uintptr_t>(0x7000u));
    df_scheduler_register_compute_dispatch(slot, fake_pipeline, fake_descriptor, 4, 4, 1);
    DF_CHECK(df_scheduler_compute_dispatch_count(slot) == 1, "1 dispatch registered");

    df_scheduler_phase_compute_reset();
    DF_CHECK(df_scheduler_compute_dispatch_count(slot) == 0, "registry cleared after reset");

    df_pipeline_reset();
}

void scenario_pipeline_slot_backpressure_on_inflight() {
    std::printf("scenario_pipeline_slot_backpressure_on_inflight\n");
    df_pipeline_reset();
    df_pipeline_init(2);

    // Fill both slots без draining.
    PipelineSlot* s0 = nullptr;
    PipelineSlot* s1 = nullptr;
    DF_CHECK(df_pipeline_allocate_slot(500, &s0) == 1, "alloc s0 ok");
    DF_CHECK(df_pipeline_allocate_slot(501, &s1) == 1, "alloc s1 ok");
    DF_CHECK(s0->state == SlotState_Dispatched, "s0 Dispatched");
    DF_CHECK(s1->state == SlotState_Dispatched, "s1 Dispatched");

    // Third allocation fails (backpressure — slot 0 still Dispatched).
    PipelineSlot* s2 = nullptr;
    DF_CHECK(df_pipeline_allocate_slot(502, &s2) == 0,
             "allocate rejected when slot still in-flight");
    DF_CHECK(s2 == nullptr, "out_slot null on backpressure");

    // Drain slot 0 through к ReadableAsTail — allocation can now recycle slot 0.
    df_pipeline_force_fence_completed(s0);
    df_pipeline_transition_to_tail(s0);

    DF_CHECK(df_pipeline_allocate_slot(502, &s2) == 1,
             "allocate succeeds after slot 0 drained");
    DF_CHECK(s2 == s0, "recycles slot 0");

    df_pipeline_reset();
}

} // namespace

int main() {
    std::printf("DualFrontier.Core.Native self-test\n");
    scenario_basic_crud();
    scenario_deferred_destroy();
    scenario_sparse_set_swap_remove();
    scenario_bulk_operations();
    scenario_span_lifetime();
    scenario_explicit_registration();
    scenario_throughput();
    scenario_bootstrap_basic();
    scenario_bootstrap_double_rejected();
    scenario_bootstrap_graph_topological();
    scenario_bootstrap_graph_parallel();
    scenario_bootstrap_rollback_on_failure();
    scenario_batch_basic();
    scenario_batch_mixed_commands();
    scenario_batch_cancel();
    scenario_batch_dead_entity_skipped();
    scenario_batch_mutation_rejection();
    scenario_string_pool();
    scenario_keyed_map();
    scenario_composite();
    scenario_set_primitive();
    scenario_field_register_and_read();
    scenario_field_write_and_read_roundtrip();
    scenario_field_span_lifecycle();
    scenario_field_conductivity_default_and_set();
    scenario_field_storage_flag_toggle();
    scenario_field_swap_buffers();
    scenario_field_register_idempotent_and_conflict();
    scenario_field_unregister();
    scenario_k10_smoke();
    scenario_system_graph_basic_register();
    scenario_system_graph_linear_chain();
    scenario_system_graph_parallel_layer();
    scenario_system_graph_write_conflict();
    scenario_system_graph_cycle_detection();
    scenario_system_graph_per_tick_subset();
    scenario_thread_pool_mode_transition();
    scenario_thread_pool_submit_batch();
    scenario_thread_pool_phase_barrier();
    scenario_wake_registry_subscription_lifecycle();
    scenario_wake_registry_fire_timer();
    scenario_wake_registry_fire_event_and_state();
    scenario_wake_registry_fire_init_one_shot();
    scenario_wake_registry_fire_explicit();
    scenario_scheduler_diagnostics();
    scenario_scheduler_tick_begin_orchestration();
    scenario_scheduling_policies_default();
    scenario_scheduling_policies_set_and_get();
    scenario_scheduling_policies_quota_enforcement();
    scenario_scheduling_policies_order_by_priority();
    scenario_shm_region_basic();
    scenario_scheduler_affinity_workstealing_barriers();
    scenario_managed_callback_roundtrip();
    scenario_state_filter_cold_path_bypass();
    scenario_state_filter_subscribe_and_hook();
    scenario_state_filter_entity_specific();
    scenario_state_filter_out_of_range();
    scenario_scheduler_trace();
    scenario_scheduler_intrinsics();
    // ===== K10.2 scenarios (added per-item commits below) =====
    // Event type registry, native bus three-tier dispatcher, background queue,
    // native unload primitive — each lands its scenarios in the dedicated commit.
    scenario_event_type_registry_register_and_lookup();
    scenario_event_type_registry_get_tier_default_normal();
    scenario_event_type_registry_reregister_idempotent_or_conflict();
    scenario_event_type_registry_invalid_args();
    scenario_bus_fast_publish_subscribe_roundtrip();
    scenario_bus_normal_publish_batched_drain();
    scenario_bus_background_publish_stores_pending();
    scenario_bus_per_mod_bulk_unsubscribe();
    scenario_bg_queue_coalesce_same_key();
    scenario_bg_queue_different_keys_not_coalesced();
    scenario_bg_queue_idle_slot_budget_partial_dispatch();
    scenario_bg_queue_saturation_drop_oldest();
    scenario_bg_queue_serialize_deserialize_roundtrip();
    scenario_bg_queue_deserialize_schema_mismatch_rejected();
    scenario_bg_queue_deserialize_malformed_returns_zero();
    scenario_mod_unload_native_state_t0_t7_sequence();
    scenario_mod_unload_quiescent_precondition_violation();
    scenario_mod_unload_no_subscriptions_succeeds_vacuously();
    scenario_v0b_compute_pipeline_registration_roundtrip();
    // ===== K10.3 v2 scenarios (added per-item commits below) =====
    // Pipeline depth (Items 33-37 — pipeline_slot state machine, Phase.Compute,
    // slot read API, drain/refill protocols, filter primitive integration);
    // К-L18 quiescent state enforcement (Item 41 — extends mod_unload primitive
    // с pipeline quiescence check). Each item lands its scenarios в dedicated
    // commit. Per Lesson #22 «match existing convention» — no new test framework
    // introduced; DF_CHECK runner pattern preserved per K3/K10.1/K10.2 lineage.
    scenario_pipeline_slot_init_and_depth();
    scenario_pipeline_slot_state_machine_cycle();
    scenario_pipeline_slot_d2_default_cycling();
    scenario_pipeline_slot_get_slot_offsets();
    scenario_pipeline_slot_is_quiescent();
    scenario_pipeline_slot_backpressure_on_inflight();
    scenario_pipeline_read_slot_tail_K_L7_1();
    scenario_pipeline_transition_fires_wake();
    scenario_pipeline_drain_pause_resume();
    scenario_pipeline_serialize_deserialize_roundtrip();
    scenario_pipeline_deserialize_depth_mismatch_rejected();
    scenario_phase_compute_registration_and_count();
    scenario_phase_compute_dispatch_empty_slot();
    scenario_phase_compute_batch_coalesce_per_slot();
    scenario_phase_compute_reset_clears_registry();
    // К-L18 quiescent state enforcement integration (Item 41)
    scenario_mod_unload_fails_when_pipeline_has_dispatched_slot();
    scenario_mod_unload_fails_when_pipeline_has_fence_completed_slot();
    scenario_mod_unload_succeeds_when_pipeline_quiescent_after_tail_transition();
    scenario_mod_unload_succeeds_when_pipeline_uninitialized();
    if (g_failures == 0) {
        std::printf("ALL PASSED\n");
        return 0;
    }
    std::printf("%d FAILURE(S)\n", g_failures);
    return 1;
}
