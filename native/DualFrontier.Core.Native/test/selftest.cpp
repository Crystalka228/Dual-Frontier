// Native self-test: exercises the C ABI end-to-end so the library can be
// validated on a machine without a .NET SDK. Prints pass/fail per scenario
// and returns a non-zero exit code on failure.

#include <atomic>
#include <chrono>
#include <cstdint>
#include <cstdio>
#include <cstring>
#include <stdexcept>
#include <thread>
#include <vector>

#include "bootstrap_graph.h"
#include "df_capi.h"
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
    if (g_failures == 0) {
        std::printf("ALL PASSED\n");
        return 0;
    }
    std::printf("%d FAILURE(S)\n", g_failures);
    return 1;
}
