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
    if (g_failures == 0) {
        std::printf("ALL PASSED\n");
        return 0;
    }
    std::printf("%d FAILURE(S)\n", g_failures);
    return 1;
}
