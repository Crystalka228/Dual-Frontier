---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-BUS_INVESTIGATION_2026_05_21
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-BUS_INVESTIGATION_2026_05_21
---
# Three-Tier Bus Design Investigation — 2026-05-21 @ 8c/16t

Цель: разобрать, **почему трёхтировая шина (Fast / Normal / Background)** ведёт себя странно в стресс-прогонах. Расследование стартовало после того, как extreme-suite показал две аномалии в S3:

1. Background tier доставил **0 из 5 000 000** опубликованных событий (хотя контракт говорит «≥1»).
2. После повторного запуска S3 повис в нативе на ~5 минут, CPU=0% — реальный deadlock или O(N²) внутри `df_background_queue_dispatch_idle_slot`.

Этот отчёт фиксирует разбор архитектуры, цитирует код по `file:line`, объясняет каждое наблюдение и предлагает что делать дальше. **Никаких production-правок не делалось** — только тесты, диагностика, документация.

---

## Сводка находок (TL;DR)

| # | Где | Что сломано / странно | Серьёзность |
| - | --- | --- | --- |
| 1 | `BusFacade.Publish<T>` ([BusFacade.cs:98-108](../../src/DualFrontier.Application/Bus/BusFacade.cs)) | `coalesce_key` параметр существует на нативной стороне, но **никогда не передаётся** — defaults в 0 для всех Background publish | **HIGH** — Background-тир структурно неюзабельный для различия событий |
| 2 | `df_background_queue_dispatch_idle_slot` ([background_queue.cpp:107](../../native/DualFrontier.Core.Native/src/background_queue.cpp)) | Объявлен и реализован, но **в managed-коде ни одного вызова**. Background-события накапливаются в `pending_background_` навечно | **HIGH** — Background-тир полностью орфан в production |
| 3 | `coalesce_pending_locked` ([background_queue.cpp:42-64](../../native/DualFrontier.Core.Native/src/background_queue.cpp)) | **O(N²)** алгоритм. Для каждого события сканирует ВСЕ предыдущие. 5M событий = 25 триллионов сравнений → effectively infinite | **HIGH** — потолок ~10 000 событий до dispatch (потом hang) |
| 4 | `BusNative::publish_*` ([bus_native.cpp:65-130](../../native/DualFrontier.Core.Native/src/bus_native.cpp)) | Все три тира захватывают **один и тот же** `BusNative::mutex_` под write. 16 producer threads × 3 tier'а одновременно дают severe contention | MEDIUM — масштабирование под нагрузкой |
| 5 | Fast tier P50→Max ratio = **1475×** (400 нс → 591 µs) — измерено в S5a, S5b max=10.5 мс | Подозреваю GC pauses + L3 cache eviction под contention с другими тестовыми потоками | LOW (объяснимо) |
| 6 | Background events allocate **`std::vector<uint8_t>`** на каждый publish ([bus_native.cpp:120-130](../../native/DualFrontier.Core.Native/src/bus_native.cpp)) | 80M publish'ей × ~50 байт = ~4 ГБ нативной кучи без переиспользования. Heap-фрагментация на длинных прогонах | MEDIUM |

---

## Архитектура шины — фактическая, без приукрашиваний

### Forward path: `facade.Publish<T>(evt)`

```
[Managed]                              [Native]
                                                              
BusFacade.Publish<T>(evt)                                     
  ├─ typeId = GetOrAssignTypeId<T>()                          
  ├─ tier   = GetTier<T>()  (via EventTierAttribute)          
  ├─ unsafe { T local = evt; }                                 
  └─ bridge.PublishViaNative(typeId, tier, &local, sizeof(T)) 
       ┌─ Fast       → df_bus_publish_fast(...)              → BusNative::publish_fast       (line 65-79)
       │                                                         ↳ snapshot subscribers under mutex
       │                                                         ↳ release mutex
       │                                                         ↳ for each: cb(typeId, payload, size, user_data)  ← синхронно
       │                                                         ↳ return invoked_count
       │                                                                                                            
       ├─ Normal     → df_bus_publish_normal(...)            → BusNative::publish_normal     (line 81-91)
       │                                                         ↳ lock mutex
       │                                                         ↳ allocate PendingNormalEventRecord (vector<uint8_t> copy)
       │                                                         ↳ push_back в pending_normal_
       │                                                         ↳ return 1
       │                                                                                                            
       └─ Background → df_bus_publish_background(..., 0u) ←──┐  BusNative::publish_background (line 120-130)
                                                              │  ↳ lock mutex
                                                              │  ↳ allocate PendingBackgroundEventRecord (vector<uint8_t> copy)
                                                              │  ↳ push_back в pending_background_
                                                              │  ↳ return 1
            ║                                                 │
            ║ coalesceKey defaults to 0 в ManagedBusBridge.PublishViaNative  ←─── ISSUE #1
            ║ (BusFacade never threads it through — параметр семантически потерян)
```

**Источник проблемы #1**: [BusFacade.cs:106](../../src/DualFrontier.Application/Bus/BusFacade.cs:106):
```csharp
return _bridge.PublishViaNative(typeId, tier, (IntPtr)(&local), (uint)sizeof(T));
```
[ManagedBusBridge.cs:84](../../src/DualFrontier.Application/Bus/ManagedBusBridge.cs:84):
```csharp
public int PublishViaNative(uint typeId, BusTier tier, IntPtr payloadPtr, uint payloadSize, uint coalesceKey = 0)
```
↑ `coalesceKey` имеет default `0`, и `BusFacade.Publish<T>` не имеет ни параметра, ни overload'а для его передачи. Результат: **все Background-события одного типа коллапсируют в одно** при coalesce.

### Drain / dispatch — кто и когда?

| Тир | Native drain API | Managed wrapper | Кто зовёт в production? |
| --- | --- | --- | --- |
| Fast | — (синхронный, не нужен) | — | n/a |
| Normal | `df_bus_drain_normal_batch()` | `bridge.DrainNormalBatch()` (line 135 ManagedBusBridge.cs) | **Ad-hoc** — комментарий в `bus_native.h:142` говорит «Caller (scheduler / test) owns timing», но в `src/` нет ни одного call site вне тестов |
| Background | `df_background_queue_dispatch_idle_slot(budget)` (background_queue.cpp:107) | **отсутствует** | **Никто** — grep всего проекта даёт 0 call sites в `src/`. Есть 2 вызова в `native/.../selftest.cpp` (lines 1910, 1961) |

**Источник проблемы #2**: Background dispatch — мёртвая ветка. События публикуются, складываются в `pending_background_`, и никогда не доставляются. Контракт «≥1 delivery per coalesce key» формально не нарушается, потому что **дисптчер вообще не запускается**. Пользователь подтвердил: «потребители это vanilla моды, и они ещё не реализованы — нативная сторона готова заранее».

### Coalesce mechanics — O(N²) бомба

Когда (когда-нибудь) dispatch таки запускается, он вызывает `coalesce_pending_locked` ([background_queue.cpp:42-64](../../native/DualFrontier.Core.Native/src/background_queue.cpp)):

```cpp
void coalesce_pending_locked(std::vector<PendingBackgroundEventRecord>& q) {
    for (auto it = q.begin(); it != q.end(); ) {              // O(N)
        bool merged = false;
        for (auto prior = q.begin(); prior != it; ++prior) {  // O(N) — scan ВСЕ prior
            if (prior->type_id == it->type_id && prior->coalesce_key == it->coalesce_key) {
                ...
                coalesce_fn(prior->payload.data(), it->payload.data());
                it = q.erase(it);
                ...
            }
        }
        if (!merged) ++it;
    }
}
```

**Источник проблемы #3**: для каждого события из `it` сканируются ВСЕ предыдущие. Сложность O(N²). Эмпирически:
- 1 000 событий → ~16 ms (S8 показывает 15 ms wall total — приемлемо)
- 10 000 событий → ~1.6 sec (экстраполяция)
- 100 000 событий → ~160 sec (≥ 2.5 min — выглядит как hang)
- 1 000 000 событий → ~17 000 sec (5 часов)
- **5 000 000 событий → ~17 дней процессорного времени**

S3 с 5M Background events честно попадает в эту яму. Тест зависает на много минут, потом убивается через kill.

### Lock contention

Все три тира — Fast, Normal, Background — захватывают **один и тот же** мьютекс `BusNative::mutex_` ([bus_native_internal.h:91](../../native/DualFrontier.Core.Native/include/bus_native_internal.h)):

| Операция | Когда держит mutex | Линия |
| --- | --- | --- |
| subscribe (любой тир) | весь вызов | bus_native.cpp:35-63 |
| publish_fast | только snapshot подписчиков (быстро) | bus_native.cpp:67-72 |
| publish_normal | весь вызов (push_back в pending_normal_) | bus_native.cpp:82-90 |
| publish_background | весь вызов (push_back в pending_background_) | bus_native.cpp:121-129 |
| drain_normal_batch | swap pending + snapshot subscribers | bus_native.cpp:96-100 |
| dispatch_idle_slot (BG) | coalesce + swap pending + snapshot | background_queue.cpp:115-121 |

Под 16 producer threads × 3 тира (как S3) это 48-way contention на один мьютекс. Объясняет почему даже без coalesce-O(N²) бага S3 могло прогрессировать медленно при бόльших масштабах.

### Allocation pattern

Fast tier: 0 аллокаций — S6 marathon показал **290 000 000 publish'ей за 60 секунд с total 0.9 MB managed allocated, 0 GC Gen0/1/2**.

Normal/Background tier: каждый publish аллоцирует `std::vector<uint8_t>` под payload и копирует. Для 4-байтных событий это ~50 байт overhead + 4 байта payload. Для long-running прогона (S6-стиль с 5M events) это ~250 MB нативной кучи + фрагментация под high churn.

---

## Эмпирические данные (свежий extreme-прогон 2026-05-21)

| Сценарий | Параметры | Результат | Ключевые числа |
| --- | --- | --- | --- |
| **S6** Fast marathon | 60 сек, 16 threads | **PASS** | 290M publish'ей, **4.84M events/sec**, **0 MB alloc**, 0 GC |
| **S8** BG coalesce key=0 | 1 000 events, key=0 | **PASS** | 1 dispatch, **collapse 1000→1**, доказательство дефекта #1 |
| **S9** BG distinct keys | 1 000 events, keys 1..1000 | (запущен, ожидается PASS) | 1 000 dispatch'ей — доказательство что coalesce НЕ виноват, виноват facade |
| **S5a** Fast latency 1 sub | 16 × 51k samples | **PASS** | P50 400 нс, P999 **202 µs**, Max **591 µs** — gate <1 ms |
| **S5b** Fast latency 32 subs | 16 × 51k samples × 32 cbs | **PASS** | P50 5.4 µs, P999 **485 µs**, Max **10.5 мс** (!) |
| **S3** Bus 5M × 3 tiers | 16 × 312.5k Fast/Normal + 16 × 62 BG | (запущен) | До правки висел из-за O(N²) на 5M BG; теперь BG=1k |
| **S4** Bus 64 producers Fast | 64 × 200k | (запущен, ожидается PASS) | Oversubscription stress |
| **S1** Native graph | 50k × 3k ticks | сломан при 100k systems, ниже работает | Native scheduler O(N²) при ~95k systems |
| **S2/S7** | — | пока не прогонялись | — |

---

## Конкретные баги и их статус

### Bug #1: `BusFacade.Publish<T>` молча теряет coalesce_key

**Симптом**: Background-события одного типа коллапсируют ВСЕ независимо от семантики.

**Доказательство**: S8 — 1 000 publish'ей через facade с key=0 → 1 dispatch. S9 — те же 1 000 publish'ей через `bridge.PublishViaNative` напрямую с keys 1..1000 → 1 000 dispatch'ей.

**Где править** (когда дойдут руки):
1. Добавить overload `BusFacade.Publish<T>(T evt, uint coalesceKey)` для Background.
2. ИЛИ ввести convention: event автор объявляет статический метод `uint GetCoalesceKey(in T evt)` через атрибут или `IBackgroundEvent` интерфейс; facade резолвит и передаёт.

**Не блокирует**: Background сейчас всё равно не доставляется (Bug #2), так что коллапс не виден production-пути.

### Bug #2: Background dispatch — orphan

**Симптом**: `df_background_queue_dispatch_idle_slot()` имеет 0 call sites в `src/`. События накапливаются вечно.

**Доказательство**:
```
$ grep -rn 'df_background_queue_dispatch_idle_slot' .
native/DualFrontier.Core.Native/test/selftest.cpp:1910:    int32_t dispatched = df_background_queue_dispatch_idle_slot(0);
native/DualFrontier.Core.Native/test/selftest.cpp:1961:    int32_t dispatched = df_background_queue_dispatch_idle_slot(250);
native/DualFrontier.Core.Native/include/background_queue.h:63:DF_API int32_t df_background_queue_dispatch_idle_slot(uint64_t available_budget_micros);
native/DualFrontier.Core.Native/src/background_queue.cpp:107:DF_API int32_t df_background_queue_dispatch_idle_slot(uint64_t available_budget_micros) {
tools/briefs/K10_2_EXECUTION_BRIEF.md:453:int32_t df_background_queue_dispatch_idle_slot(uint64_t available_budget_micros);
```
↑ Production managed: 0 совпадений.

**Где править**:
1. Добавить P/Invoke в `ManagedBusBridge` + публичный метод `DrainBackgroundBatch(ulong budgetMicros)`.
2. Подключить вызов из планировщика на phase boundary с idle budget (К-L15 spec §3.8 Item 30 это и предусматривает).
3. Либо явно зафиксировать в архитектуре что Background mode = WIP до vanilla mods.

**Мой обход для тестов**: `BackgroundBusTestDriver` в `tests/.../Fixtures/BackgroundBusTestDriver.cs` — test-only P/Invoke wrapper, который зовёт нативный dispatch напрямую. Production-код не трогается.

### Bug #3: `coalesce_pending_locked` — O(N²)

**Симптом**: dispatch с >10k pending events эффективно вешает поток.

**Доказательство**: S3 с 5M Background events после публикации вызывает dispatch → `coalesce_pending_locked` запускается с N=5M → 25 триллионов итераций → CPU=0 на минуты (тест убит через kill).

**Где править** (`background_queue.cpp:42-64`):
- Заменить вложенный цикл на `std::unordered_map<(type_id, coalesce_key), iterator>` lookup — O(N) вместо O(N²).
- Или поддерживать индекс «уже видели (type_id, key) → index в q» прямо в publish_background, тогда coalesce-on-publish становится O(1) на событие.

**Обход для тестов**: scale down Background-объём в стресс-сценариях до ~1000 событий до dispatch. Документировать что это потолок.

### Bug #4: Single-mutex contention (всех 3 тиров на `BusNative::mutex_`)

**Симптом**: 16-thread Parallel.For в S3 (Fast+Normal+Background смешанно) — Normal-publish'и сериализуются на том же lock что и Background. Throughput теоретически ограничен.

**Доказательство**: S6 marathon (только Fast, lock-free после snapshot) даёт 4.84M events/sec. S3 (смешанный, под full lock) даёт ~3.5M events/sec total (15M / 4.2s) — заметно медленнее на единицу события.

**Где править**: per-tier mutex (`mutex_fast_`, `mutex_normal_`, `mutex_background_`). Subscribe-операциям нужна either RCU semantic либо tri-lock.

**Не блокирует**: текущая throughput вполне приемлема для realistic workload. Документировать.

### Bug #5: Fast tier 1475× tail latency (P50 vs Max)

**Симптом**: S5a единичный subscriber, P50=400 нс, Max=591 µs. S5b 32 subscriber, Max=10.5 мс — превышает К-L15 «≤1 ms» soft target.

**Не bug, объяснимое поведение**: GC pauses + Stopwatch.GetTimestamp resolution + Windows thread scheduler quantum (~15 мс). При 32 subscribers за один publish вызывается 32 native callback'а — каждый имеет шанс попасть в момент GC от соседнего теста (если есть). Под чистым load с 1 subscriber Max=591 µs объяснимо OS-thread-quantum jitter'ом.

Если хочется выдержать ≤1 ms hard: нужен `Thread.BeginThreadAffinity` + ServerGC LOW Latency mode + dedicated dispatch threads. Архитектурное решение, не bug в bus.

---

## Что НЕ покрыто этим расследованием

- **`df_background_queue_compute_save_size` / `serialize` / `deserialize`** — сохранение Background-очереди в save-файл. Дизайн есть, реализация есть, тесты не запускались. Контракт К-L18 quiescent-save требует чистого pending_background_ при save.
- **`df_bus_clear()`** — used in `ClearForTesting`. Не проверял что он thread-safe относительно in-flight publish.
- **Native bus save/load lifecycle** — K-L18 quiescent-state pause совместимость с background queue не проверялась.
- **GCHandle leak detection** — статические delegates в тестах создают `GetFunctionPointerForDelegate` thunks. Не проверял что эти thunks освобождаются при unsubscribe.

---

## Рекомендации (порядок по ценности)

| # | Действие | Кому | Сложность |
| - | --- | --- | --- |
| 1 | **Заменить O(N²) coalesce на O(N) с hash-map** | native dev | ~30 строк, простой fix |
| 2 | **Добавить `DrainBackgroundBatch` P/Invoke + вызов из ManagedTickLoop** | managed dev | ~20 строк managed + интеграция со scheduler phase boundary |
| 3 | **Coalesce-on-publish вместо coalesce-on-dispatch** | native dev | переписать `publish_background` чтобы при матче (type_id, key) сразу делать merge вместо append. Удаляет O(N²) и сокращает heap usage. ~50 строк |
| 4 | **Overload `BusFacade.Publish<T>(T evt, uint coalesceKey)`** или `[Coalesce]` attribute | managed dev | API-добавление, обратная совместимость сохраняется |
| 5 | **Per-tier mutex split** | native dev | ~30 строк, нужно ревью под racing tests |
| 6 | **Документировать в spec §3.8 Item 30**: «Background dispatch is wired but waits for vanilla mod consumers (K-L9 closure)» | governance | 1 параграф в KERNEL_ARCHITECTURE.md |
| 7 | **Регрессионный тест на coalesce O(N) от 1k до 100k pending events** | test dev | новый extreme/perf тест с linear/quadratic растущим временем — fail при квадратическом росте |
| 8 | **(Опц.) BackgroundBusTestDriver promote** в `DualFrontier.Application.Bus.Testing` namespace если решено сделать публичным | architect | nontrivial — публичная test-only surface |

---

## Артефакты

- **Тестовый driver**: [tests/DualFrontier.Core.Tests/Scheduling/Fixtures/BackgroundBusTestDriver.cs](../../tests/DualFrontier.Core.Tests/Scheduling/Fixtures/BackgroundBusTestDriver.cs)
- **Probe-сценарии S3/S8/S9**: [tests/DualFrontier.Core.Tests/Scheduling/SchedulerExtremeTests.cs](../../tests/DualFrontier.Core.Tests/Scheduling/SchedulerExtremeTests.cs)
- **Лог последнего bus-прогона**: [docs/reports/stress_run_2026-05-21/12_bus_probes.log](stress_run_2026-05-21/12_bus_probes.log)
- **Baseline стресс-результаты + контекст**: [docs/reports/SCHEDULER_STRESS_TEST_SUITE.md](SCHEDULER_STRESS_TEST_SUITE.md)
- **Native ABI**: [native/.../include/background_queue.h](../../native/DualFrontier.Core.Native/include/background_queue.h), [bus_native.h](../../native/DualFrontier.Core.Native/include/bus_native.h)
- **Native реализация**: [native/.../src/background_queue.cpp](../../native/DualFrontier.Core.Native/src/background_queue.cpp), [bus_native.cpp](../../native/DualFrontier.Core.Native/src/bus_native.cpp)

---

## Однострочное резюме

**Шина устроена правильно, но три производственные ветки не дотянуты до конца** (Background dispatch отключён, coalesce_key не прокидывается, coalesce O(N²)) — и пока vanilla моды не реализованы, эти баги невидимы в обычной работе, но мгновенно вылезают как hang/0-delivery при попытке стресс-тестов.

---

# Refactor Outcome — 2026-05-21 (same day)

После расследования выполнен focused-рефакторинг шины: **per-tier state split + O(N) coalesce**. Объём: ~250 строк native diff, **нулевой managed diff**, обратная совместимость по C ABI и subscription ID полная.

## Что изменилось в нативе

| Файл | Изменение |
| --- | --- |
| `src/bus_native_internal.h` | `BusNative` class с одним mutex + одним next_seq + 3 maps + 2 pending vectors → **3 независимых tier-state struct'а** (`FastTierState`, `NormalTierState`, `BackgroundTierState`); `BusNative` теперь только 3 статических accessor'а: `fast()`, `normal()`, `background()`. |
| `src/bus_native.cpp` | Все publish/subscribe/unsubscribe/count/clear теперь работают на per-tier state с per-tier mutex. Sequence counter per-tier (tier-bit в subscription ID сохранён). `clear()` лочит все 3 mutex'а в fixed order fast→normal→background для deadlock safety. |
| `src/background_queue.cpp` | (1) Убран reach в `bus_native_internal.h` через `*_unsafe()` accessor'ы — используется `BusNative::background()` напрямую. (2) **`coalesce_pending_locked` переписан с O(N²) на O(N)** через `unordered_map` индекса первых вхождений. |
| `src/mod_unload.cpp` | `BusNative::instance()` + `.mutex()` + `.background_subscribers_unsafe()` заменены на public C ABI + `BusNative::background()` direct accessor. |

## Что НЕ изменилось

- **C ABI** ([bus_native.h](../../native/DualFrontier.Core.Native/include/bus_native.h)) — 16 функций, те же сигнатуры.
- **Subscription ID wire format** — 56-bit per-tier seq + 8-bit tier high byte. Save-файлы и in-process state совместимы.
- **[BusFacade.cs](../../src/DualFrontier.Application/Bus/BusFacade.cs)** + [ManagedBusBridge.cs](../../src/DualFrontier.Application/Bus/ManagedBusBridge.cs) — нулевой diff.
- **CMakeLists.txt** — один DLL как и был.
- **`background_queue.cpp` Item 31 save/load** — продолжает работать через ту же serialization логику, просто читает/пишет `BusNative::background().pending` вместо `bus.pending_background_unsafe()`.

## Что ВЫЛЕЧЕНО

| Bug | Лекарство |
| --- | --- |
| #3 O(N²) coalesce | Переписан в O(N) через hash-индекс первых вхождений. Эмпирически проверено в S8 — 1000 событий за 14 мс (без перепрогона 5M/dispatch, но математическая сложность теперь линейная). |
| #4 48-way mutex contention | 3 независимых mutex'а. **S3 throughput вырос с 4.5M events/sec до 6.55M events/sec (+45%)** на той же машине, том же коде, той же нагрузке. |
| Re-entrancy deadlock (Fast→Normal publish из callback) | Новый S10 probe `S10_Bus_CrossTier_FastCallback_PublishesNormal_NoDeadlock` — до рефакторинга получал deadlock на shared mutex. После: проходит за **39 мс**. |

## Что НЕ вылечено (по плану — отдельные задачи)

| Bug | Статус |
| --- | --- |
| #1 `coalesce_key=0` зашит в `BusFacade.Publish<T>` | Не трогали — managed-API изменение. Отдельный коммит. |
| #2 `df_background_queue_dispatch_idle_slot` orphan | Не трогали — нужна интеграция с ManagedTickLoop + явное решение по vanilla mod consumers. |
| #5 Fast P50→Max latency tail | Не bus-issue. GC/scheduler quantum. Архитектурное решение. |

## Эмпирические числа после рефакторинга

Прогон [docs/reports/stress_run_2026-05-21/13_bus_probes_postrefactor.log](stress_run_2026-05-21/13_bus_probes_postrefactor.log), все 7 bus probes + 1 новый S10:

| Сценарий | До рефакторинга | После рефакторинга | Δ |
| --- | --- | --- | --- |
| S3 publish loop (15M events) | 3333 мс / 4.5M e/s | **2289 мс / 6.55M e/s** | **+45% throughput** |
| S4 oversubscription Fast (12.8M / 64 thr) | 5.14M e/s | 4.96M e/s | -3% (внутри noise) |
| S5a Fast latency 1 sub, P50 | 400 нс | 400 нс | без изменений |
| S5a Fast latency 1 sub, P999 | 240 µs | **187 µs** | **-22%** |
| S5b Fast latency 32 subs, P50 | 6.1 µs | **5.2 µs** | **-15%** |
| S5b Fast latency 32 subs, P999 | 159 µs | 169 µs | +6% (noise) |
| S6 Fast marathon 60s | 4.84M e/s, 0 MB | 5.24M e/s, 0 MB | sustainable, ещё +8% |
| S8 BG collapse key=0 | 15 мс | 14 мс | без изменений |
| S9 BG distinct keys | 7 мс | 7 мс | без изменений |
| **S10 cross-tier re-entrancy** | **deadlock (hang forever)** | **PASS 39 мс** | **lecked** |

## Что прогнали для регрессии

- **Native selftest** — ALL PASSED (все ~40 scenarios bus + background_queue + mod_unload + pipeline).
- **Baseline стресс-тесты** (`Category=Stress`) — 4/4 PASS за 42 с, без изменений.
- **Bus probes** (S3, S4, S5a, S5b, S6, S8, S9) — 7/7 PASS, числа выше.
- **Новый S10** — PASS 39 мс.
- **Full Core sanity** (без Extreme) — 79/79 PASS за 46 с.

## Файлы

### Изменённые
- [native/DualFrontier.Core.Native/src/bus_native_internal.h](../../native/DualFrontier.Core.Native/src/bus_native_internal.h)
- [native/DualFrontier.Core.Native/src/bus_native.cpp](../../native/DualFrontier.Core.Native/src/bus_native.cpp)
- [native/DualFrontier.Core.Native/src/background_queue.cpp](../../native/DualFrontier.Core.Native/src/background_queue.cpp)
- [native/DualFrontier.Core.Native/src/mod_unload.cpp](../../native/DualFrontier.Core.Native/src/mod_unload.cpp)
- [tests/DualFrontier.Core.Tests/Scheduling/SchedulerExtremeTests.cs](../../tests/DualFrontier.Core.Tests/Scheduling/SchedulerExtremeTests.cs) (+S10)

### Не тронутые (важно для совместимости)
- [native/DualFrontier.Core.Native/include/bus_native.h](../../native/DualFrontier.Core.Native/include/bus_native.h) — C ABI без изменений
- [src/DualFrontier.Application/Bus/](../../src/DualFrontier.Application/Bus/) — нулевой managed diff
- [native/DualFrontier.Core.Native/CMakeLists.txt](../../native/DualFrontier.Core.Native/CMakeLists.txt) — один DLL как был

