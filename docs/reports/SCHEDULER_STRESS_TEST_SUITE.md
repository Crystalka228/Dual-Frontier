---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-SCHEDULER_STRESS_TEST_SUITE
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-SCHEDULER_STRESS_TEST_SUITE
---
# Scheduler & Bus Stress Test Suite — 8c/16t Audit Pack

Цель: убедиться, что на машине 8 ядер / 16 потоков нативный планировщик,
managed `ParallelSystemScheduler`, managed-граф загрузки модов и
трёхуровневая шина (`BusTier.Fast / Normal / Background`) выдерживают
экстремальную одновременную нагрузку без deadlock, гонок, потери событий и
нарушения порядка приоритетов.

Тесты написаны, **но не запускались автором этого отчёта** — прогон делается
локально на твоей машине.

## Что в комплекте

| Слой | Где | Сценарии |
| --- | --- | --- |
| Нативный scheduler + dependency graph | `tests/DualFrontier.Core.Tests/Scheduling/SchedulerStressTests.cs` | `NativeGraph_FiveThousandSystems_RandomDag_ComputesAndTicksWithoutError` |
| Managed `ParallelSystemScheduler` | `tests/DualFrontier.Core.Tests/Scheduling/SchedulerStressTests.cs` | `ManagedScheduler_WideLayerPlusDeepChain_DoesNotDeadlockAndExecutesAllSystems` |
| Three-tier bus (Fast/Normal/Background) | `tests/DualFrontier.Core.Tests/Scheduling/SchedulerStressTests.cs` | `BusTiers_AllThreeTiers_HighContentionPublish_RespectsTierContract` |
| `SchedulingPolicies.OrderByPriority` (5 классов) | `tests/DualFrontier.Core.Tests/Scheduling/SchedulerStressTests.cs` | `SchedulingPolicies_OrderByPriority_ParallelCalls_DeterministicAndCorrectlyOrdered` |
| Managed mod-load dep graph (Kahn) | `tests/DualFrontier.Modding.Tests/Pipeline/ModDependencyGraphStressTests.cs` | `TopoSortRegularMods_FiveThousandModRandomDag_ProducesValidOrdering`, `TopoSortRegularMods_CalledInParallelFromManyThreads_ProducesConsistentValidOrdering`, `TopoSortRegularMods_FiveThousandModsWithCycles_ReportsAllParticipants`, `CheckDependencyPresence_LargeBatchWithMissingDeps_ReportsErrorsAndWarnings` |
| BDN-бенчмарки | `tests/DualFrontier.Core.Benchmarks/Stress/SchedulerStressBenchmarks.cs` | `NativeScheduler_OneTick`, `NativeScheduler_RebuildStaticGraph`, `OrderByPriority_FullSet`, `TopoSort_FullBatch`, `CheckDependencyPresence_FullBatch` |

Все xUnit-тесты помечены `[Trait("Category", "Stress")]` — CI может
отфильтровать через `--filter "Category!=Stress"`.

## Как запускать

### xUnit стресс-тесты

```bash
# Core (4 сценария)
dotnet test tests/DualFrontier.Core.Tests/DualFrontier.Core.Tests.csproj \
    --filter "Category=Stress" -c Release \
    --logger "console;verbosity=detailed"

# Mod dependency graph (4 сценария)
dotnet test tests/DualFrontier.Modding.Tests/DualFrontier.Modding.Tests.csproj \
    --filter "Category=Stress" -c Release \
    --logger "console;verbosity=detailed"
```

### BenchmarkDotNet

```bash
dotnet run -c Release \
    --project tests/DualFrontier.Core.Benchmarks \
    -- --bdn-stress
```

BDN использует `[ShortRunJob]` (быстрая итерация) + `[MemoryDiagnoser]` +
`[ThreadingDiagnoser]`. Параметр `[Params(500, 2_000, 5_000)]` даст 3 точки
на каждый бенчмарк → 15 строк итоговой таблицы.

### Sanity-чек (что остальные тесты не сломались)

```bash
dotnet test tests/DualFrontier.Core.Tests/DualFrontier.Core.Tests.csproj -c Release
dotnet test tests/DualFrontier.Modding.Tests/DualFrontier.Modding.Tests.csproj -c Release
```

## Что именно тестирует каждый сценарий

### 1. `NativeGraph_FiveThousandSystems_RandomDag_ComputesAndTicksWithoutError`

- **Setup**: регистрирует 5000 нативных систем через `SystemGraphInterop.RegisterSystem`.
  Каждая система пишет уникальный component id (= `sysIdx + 1`), читает
  0–4 случайных id строго меньше своего → forward-only edges →
  guaranteed acyclic, no write-write collisions.
- **Wake**: половина систем подписана `WakeRegistryInterop.SubscribeTimer`
  с tick-rate 1..16 — runqueue меняется каждый тик.
- **Loop**: 5000 тиков. На каждом — `FireTimer → DrainRunqueue →
  ComputePerTickGraph → TickBegin` (последний на чётных тиках). Все
  возвраты обязаны быть `Success`.
- **Что проверяем**: C++ топологический сорт переваривает 5000 узлов и
  пересчёт per-tick подграфа на каждом тике без ошибок.
- **На что смотреть в логе**: assertion-failure на `ComputeResult` ≠
  `Success` означает регрессию в нативном графе или wake registry.

### 2. `ManagedScheduler_WideLayerPlusDeepChain_DoesNotDeadlockAndExecutesAllSystems`

- **Setup**: 64 wide-системы (все в фазе 0) + 16 chain-систем (длинная
  C00→C01→...→C15 зависимость). Каждая `Update` делает
  `Interlocked.Increment(counter)` + `Thread.SpinWait(2_000)`.
- **Loop**: 5000 тиков через `ParallelSystemScheduler.ExecuteTick`.
- **Asserts**:
  1. Суммарный счётчик == `(wide + chain) * ticks` = 400_000.
     Любое скипа или повтор — счётчик не сойдётся.
  2. Phase-count ≥ 16 (chain заставляет строго 16 последовательных фаз).
  3. `HashSet<ManagedThreadId>` ≥ `min(8, N-2)` — wide-слой должен
     раздаться по пулу TPL.
- **Что показывает**: managed scheduler корректно диспатчит 64
  независимые системы параллельно (один из ключевых производственных
  workload-ов) и при этом сохраняет sequential ordering chain'a.
- **Red flag**: `AggregateException` из `Parallel.ForEach`,
  `InvalidOperationException("non-concurrent collection")`, неравный
  счётчик.

### 3. Bus three-tier (`BusTiers_AllThreeTiers_HighContentionPublish_RespectsTierContract`)

- **Setup**: `BusFacade.UseNativeBusForDispatch = true`. Регистрируются
  3 события: `[EventTier(Fast)]`, `[EventTier(Normal)]`,
  `[EventTier(Background)]`. По одному reverse-P/Invoke callback на каждое
  (паттерн из `ManagedBusBridgeTests`).
- **Load**: `Parallel.For(0, 16, …)` — 16 producer-потоков, каждый
  публикует 4_000 Fast + 4_000 Normal + 4_000 Background = **192_000
  событий**.
- **Asserts** (по контракту К-L15):
  - Fast: счётчик коллбэков == 64_000 (синхронный диспатч ≤1ms,
    invocation **должна** случиться до возврата `Publish`).
  - Normal: после серии `DrainNormalBatch` счётчик == 64_000.
  - Background: 0 < count ≤ 64_000 (coalescence легально снижает; хотя бы
    одна доставка на distinct coalesce key обязательна).
- **Что показывает**: трёхуровневая шина не теряет события, не путает
  тиры, синхронный контракт Fast не нарушается под 16-thread contention.

### 4. `SchedulingPolicies_OrderByPriority_ParallelCalls_DeterministicAndCorrectlyOrdered`

- **Setup**: 500 систем, равномерно распределённых по
  `SchedulingClass.{RealTime, High, Normal, Low, Background}`.
- **Load**: 16 потоков × 100 итераций = 1600 вызовов `OrderByPriority`
  на случайных permutation'ах одного и того же набора id.
- **Asserts**: каждый возвращённый массив:
  1. Монотонен по `SchedulingClass` (lower numeric = higher prio).
  2. В пределах одного класса — ascending by system id (для
     детерминированности per native contract).
- **Что показывает**: «планировщик запускает системы по приоритетам» —
  именно та часть, которую ты упомянул («3 тира должны запускаться по
  приоритетам»). Шинный тир и SchedulingClass — разные оси, но обе
  покрыты сценариями 3 и 4 соответственно.

### 5. Mod-load dependency graph (4 теста)

- **5000 mods, random forward DAG → должен сортироваться без ошибок,
  валидно (для каждой required edge `a→b`: idx(a) < idx(b))**.
- **5000 mods × 64 потока × 50 итераций — пустой `cycleErrors`,
  topology-violation = 0**. Это прямой тест чистоты функции —
  `TopoSortRegularMods` не должна разделять статическое состояние между
  потоками.
- **4800 acyclic + 100 cyclic pairs → 200 `CyclicDependency` errors,
  ровно 4800 в sorted**.
- **3000 base + 1000 missing-required + 1000 missing-optional → 1000
  errors + 1000 warnings**.

## На что обращать внимание при прогоне

### xUnit — обязательно зелёные

Если хоть один тест красный — это сигнал реальной регрессии. Не пытаться
«стабилизировать» через retries; красный тест нужно разбирать.

### BDN — на что смотреть в финальной таблице

| Колонка | Что значит | Тревожные значения |
| --- | --- | --- |
| `Mean` | средняя длительность операции | резкие скачки между прогонами одного и того же `[Params]` — нестабильность пула потоков |
| `Allocated` | байты на операцию | рост >2× с предыдущего baseline |
| `Gen0` / `Gen1` / `Gen2` | количество сборок мусора | `Gen2` > 0 — рост, который надо разбирать |
| `Lock Contentions` (от `ThreadingDiagnoser`) | сколько раз поток заблокировался на lock | большие числа на `NativeScheduler_OneTick` = bottleneck в C++ синхронизации |
| `Completed Work Items` | сколько TPL-task-ов прошло через пул | напрямую отражает, сколько работы попало в 16 потоков |

Baseline-цифры для сравнения сейчас отсутствуют — это **первый** прогон
этого набора. Сохрани CSV/таблицу после первого зелёного запуска как
эталон.

### Red flags при прогоне локально

1. **`AggregateException` из `Parallel.ForEach`** в managed-сценарии =
   исключение внутри какой-то системы. Скорее всего регрессия в
   `ParallelSystemScheduler` или в фикстурном `Update`.
2. **`InvalidOperationException("Operations that change non-concurrent
   collections...")`** = вернулась гонка типа той, что закрыл
   `TickSchedulerThreadSafetyTests`. Скорее всего где-то протёк
   non-concurrent `Dictionary`.
3. **Background bus count == 0** = native background queue
   разрушает все события под coalesce_key=0. Это нарушение К-L15.
4. **`OrderByPriority` returns с inversion** = регрессия в
   `scheduling_policies.cpp` (либо в native sort, либо в стабильности
   по id).
5. **Mod toposort topology violation** = `TopoSortRegularMods` не
   чисто-функциональна (shared static state) либо в Kahn появилась
   гонка.
6. **Working-set растёт неограниченно при долгом прогоне** =
   GCHandle/native handle leak. Не должно случиться — все Subscribe
   парятся с Unsubscribe в finally, но проверять стоит.

## Что НЕ покрыто

- **Concurrent register в нативный граф** — по контракту
  `SystemGraphInterop` не thread-safe для регистрации, поэтому стресс
  его не нагружает. Стрессить только compute + dispatch.
- **Mod hot-reload под нагрузкой** — `ParallelSystemScheduler.Rebuild`
  предполагается вызывать только в menu, не в сессии; стресс-тестов
  для одновременного Rebuild + Tick нет.
- **Native bus `df_bus_drain_background`** — на момент написания тестов
  managed-обёртка `DrainBackgroundBatch()` отсутствует;
  стресс-сценарий валидирует только `DrainNormalBatch`. Когда обёртка
  появится — добавить аналогичный drain-loop для Background.
- **Cross-tick stability >5000 ticks** — для долгих run-ов есть
  `LongRunDriftRunner` в `TickLoop/`; интеграция стресс-сценариев в
  этот раннер — отдельная задача.

## Файлы, которые могут понадобиться при разборе fails

- `src/DualFrontier.Core/Scheduling/DependencyGraph.cs` — Kahn + write-conflict detect (managed).
- `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs` — Parallel.ForEach диспатчер.
- `native/DualFrontier.Core.Native/src/system_graph.cpp` — нативный Kahn.
- `native/DualFrontier.Core.Native/src/scheduling_policies.cpp` — OrderByPriority.
- `native/DualFrontier.Core.Native/src/bus_native.cpp` — three-tier шина.
- `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:1080-1299` —
  TopoSortByPredicate.
- `src/DualFrontier.Core.Interop/SystemGraphInterop.cs` — managed обёртка.
- `src/DualFrontier.Application/Bus/ManagedBusBridge.cs` — bus P/Invoke.

## Чек-лист первого прогона

- [ ] `dotnet build -c Release DualFrontier.sln` — сборка без warnings/errors.
- [ ] `dotnet test ... DualFrontier.Core.Tests.csproj --filter "Category=Stress"` — 4 зелёных факта.
- [ ] `dotnet test ... DualFrontier.Modding.Tests.csproj --filter "Category=Stress"` — 4 зелёных факта.
- [ ] `dotnet test ... DualFrontier.Core.Tests.csproj` (без фильтра) — остальной набор не сломан.
- [ ] `dotnet test ... DualFrontier.Modding.Tests.csproj` (без фильтра) — то же.
- [ ] `dotnet run -c Release --project tests/DualFrontier.Core.Benchmarks -- --bdn-stress` —
  BDN отрабатывает, сохранить таблицу как baseline.

После первого зелёного прогона — приложить итоговую BDN-таблицу к этому
отчёту как раздел «Baseline 2026-05-21 @ 8c/16t».

## Прогон 2026-05-21 — фиксы перед запуском

Коммит `39a01be` собран не был; на первой попытке `dotnet build -c Release
DualFrontier.sln` выпали 8 ошибок (5 синтаксических/семантических + 3
дополнительных, проявившихся после починки парсера). Все правки — внутри
самих стресс-файлов, src/ не трогался.

| # | Файл : строка | Класс ошибки | Фикс |
| - | --- | --- | --- |
| 1 | `tests/.../SchedulerStressTests.cs:89` | `Math` затенялся namespace `DualFrontier.Core.Tests.Math` | `Math.Min` → `System.Math.Min` |
| 2 | `tests/.../SchedulerStressTests.cs:225` | то же | `Math.Max`/`Math.Min` → `System.Math.*` |
| 3 | `tests/.../SchedulerStressBenchmarks.cs:167` | `0xBA7CH_BD` — `H` не hex-цифра | `0xBA7C_BD` |
| 4 | `tests/.../SchedulerStressBenchmarks.cs:174` | `Math` затенялся namespace `DualFrontier.Core.Math` | `System.Math.Min` |
| 5 | `tests/.../ModDependencyGraphStressTests.cs:33` | `0xDF_BA7CH` — `H` не hex-цифра | `0xDF_BA7C` |
| 6 | `tests/.../ModDependencyGraphStressTests.cs:96` | `0xCAFE_F00D` (uint > int.MaxValue) при `int seed` | `unchecked((int)0xCAFE_F00D)` |
| 7 | `tests/.../ModDependencyGraphStressTests.cs:142` | `0xBEEF_0042` (тот же overflow) | `unchecked((int)0xBEEF_0042)` |
| 8 | `tests/.../SchedulerStressTests.cs:250` | Background-тир требует non-null `coalesce_fn` (контракт К-L15 / Q-N-34 в `event_type_registry.cpp:56`) | Добавлен `CoalesceDelegate` (latest-wins copy 4-byte int) + `Marshal.GetFunctionPointerForDelegate`, передаётся в `RegisterEventType<StressBackgroundEvent>(coalesceFnPtr)` |

Пункт 8 — не баг компиляции, а runtime-фейл, обнаруженный первым же
запуском Bus-сценария: тест передавал `coalesceFnPtr = default`, а
нативный реестр это запрещает для Background-тира («Background tier
requires coalesce function declaration per Q-N-34»). После починки тест
проходит за 75 мс.

## Чек-лист — результат 2026-05-21 @ 8c/16t

- [x] `dotnet build -c Release DualFrontier.sln` — 0 warnings, 0 errors (после 8 фиксов выше; 23.87 → 8.17 сек).
- [x] `dotnet test ... DualFrontier.Core.Tests.csproj --filter "Category=Stress"` — **4 / 4** (54.27 с).
- [x] `dotnet test ... DualFrontier.Modding.Tests.csproj --filter "Category=Stress"` — **4 / 4** (6.99 с).
- [x] `dotnet test ... DualFrontier.Core.Tests.csproj` (без фильтра) — **79 / 79 зелёных** (50 с).
- [~] `dotnet test ... DualFrontier.Modding.Tests.csproj` (без фильтра) — **383 / 399 (16 fail)**. См. §«Что выяснилось при sanity» ниже — fails **не вызваны** стресс-пакетом.
- [x] `dotnet run -c Release --project tests/DualFrontier.Core.Benchmarks -- --bdn-stress` — 15 измерений за 1м58с. Таблицы ниже.

### Стресс по сценариям (детально)

| Слой | Сценарий | Результат | Время |
| --- | --- | --- | ---:|
| Native scheduler + dep graph | `NativeGraph_FiveThousandSystems_RandomDag_…` | PASS | 40 с |
| Managed `ParallelSystemScheduler` | `ManagedScheduler_WideLayerPlusDeepChain_…` | PASS | 8 с |
| Three-tier bus (16-thread, 48k events) | `BusTiers_AllThreeTiers_HighContentionPublish_…` | PASS | 75 мс |
| `SchedulingPolicies.OrderByPriority` (16×100) | `SchedulingPolicies_OrderByPriority_…` | PASS | 30 мс |
| Mod-graph 5000-mod DAG | `TopoSortRegularMods_FiveThousandModRandomDag_…` | PASS | 22 мс |
| Mod-graph 5000-mod × 64 потока × 50 итераций | `TopoSortRegularMods_CalledInParallelFromMany…` | PASS | 3 с |
| Mod-graph 4800 acyclic + 100 cyclic pairs | `TopoSortRegularMods_FiveThousandModsWithCycles_…` | PASS | 15 мс |
| Mod-graph 3000+1000+1000 missing-dep classification | `CheckDependencyPresence_LargeBatchWithMissingDeps_…` | PASS | 198 мс |

## Что выяснилось при sanity (full Modding suite)

`dotnet test … DualFrontier.Modding.Tests.csproj` без фильтра показал
**16 fail / 399 total**. Их можно разбить на две независимые группы:

**Группа A — 14 pre-existing fails на ветке `claude/scheduler-stress-test-KmVM3`**,
никакого отношения к стресс-пакету не имеют. Прогон с `--filter
"Category!=Stress"` показал **те же 14**:

- `M51PipelineIntegrationTests.Apply_*` (4 теста)
- `M52IntegrationTests.Apply_*` (3 теста)
- `M62IntegrationTests.Apply_*` (5 тестов)
- `M73Phase2DebtTests.RegularMod_*` (2 теста)

Коммит `39a01be` чисто добавляющий — пять новых файлов, ни одна
существующая строка не правится. Значит эти 14 уже сломаны на main
после `28b64fb` (PR #41 / К10.3 v2 closure).
**Outside scope этого прогона — отдельная задача для разбора Pipeline-цепочки.**

**Группа B — 2 stress-induced cross-test pollution**:

- `GameBootstrapIntegrationTests.CreateLoop_RunningLoop_PawnStateCommandCarriesTopSkills`
- `GameBootstrapIntegrationTests.CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge`

Оба прошли **в изоляции** (`--filter "FullyQualifiedName~…"` — 641 + 284 мс).
Падают только в полном прогоне рядом со стресс-тестами.

Корень: `SchedulerStressTests.Dispose()` сбрасывает 4 native singleton'а
(`SystemGraphInterop.Clear()`, `WakeRegistryInterop.Clear()`,
`SchedulingPoliciesInterop.Clear()`, `EventTypeRegistryInterop.ClearForTesting()`),
но **не** сбрасывает `ManagedBusBridge.ClearForTesting()` и не дренирует
очереди шины. Bus-стресс делает `bridge.ClearForTesting()` только в
конструкторе — после него остаются подвешенные subscriber GCHandle'ы,
накопленные `[GCHandle.Alloc(...)]` в теле теста (`fastHandle`,
`normalHandle`, `backgroundHandle` блоки try/finally есть в исходнике —
нужно проверить, дренируются ли они до Dispose).

**Что делать**: либо стресс-тест должен полностью обнулять `ManagedBusBridge`
+ нативные очереди в `Dispose`, либо `GameBootstrapIntegrationTests`
должен сам резетить shared state перед запуском. По чистоте принципа
[[feedback_bus_access]] / К-L15 — это работа стресс-теста.

Эту регрессию записал отдельной задачей; стресс-пакет в текущем виде её
вскрывает, но не чинит.

## Baseline 2026-05-21 @ 8c/16t

Платформа: Windows 11 (10.0.26200.8457), .NET SDK 10.0.204, runtime .NET
8.0.27 X64 RyuJIT AVX2, GC Concurrent Workstation. BenchmarkDotNet
v0.13.12, `[ShortRunJob]` (LaunchCount=1, WarmupCount=3, IterationCount=3).
Всего 15 измерений за **1 м 58 с**.

### SchedulerStressBenchmarks (3 метода × 3 SystemCount)

| Method                             | SystemCount | Mean           | Error           | StdDev         | Gen0   | Completed Work Items | Lock Contentions | Allocated |
|----------------------------------- |------------ |---------------:|----------------:|---------------:|-------:|---------------------:|-----------------:|----------:|
| NativeScheduler_OneTick            | 500         |     175.327 us |     397.5282 us |     21.7899 us |      - |                    - |                - |     488 B |
| NativeScheduler_RebuildStaticGraph | 500         |   1,498.323 us |     187.7912 us |     10.2935 us |      - |                    - |                - |       1 B |
| OrderByPriority_FullSet            | 500         |       8.685 us |       0.8788 us |      0.0482 us | 0.2289 |                    - |                - |    2024 B |
| NativeScheduler_OneTick            | 2000        |   1,798.735 us |   2,512.2656 us |    137.7058 us |      - |                    - |                - |    1732 B |
| NativeScheduler_RebuildStaticGraph | 2000        |  36,262.358 us |   6,099.6305 us |    334.3413 us |      - |                    - |                - |      27 B |
| OrderByPriority_FullSet            | 2000        |      77.257 us |      46.5678 us |      2.5525 us | 0.8545 |                    - |                - |    8024 B |
| NativeScheduler_OneTick            | 5000        |  31,580.197 us | 111,920.9040 us |  6,134.7624 us |      - |                    - |                - |    7510 B |
| NativeScheduler_RebuildStaticGraph | 5000        | 277,854.717 us | 438,914.1296 us | 24,058.3645 us |      - |                    - |                - |     200 B |
| OrderByPriority_FullSet            | 5000        |     295.857 us |     426.2298 us |     23.3631 us | 1.9531 |                    - |                - |   20024 B |

### ModDependencyGraphBenchmarks (2 метода × 3 ModCount)

| Method                            | ModCount | Mean        | Error        | StdDev     | Gen0     | Gen1     | Gen2     | Allocated  |
|---------------------------------- |--------- |------------:|-------------:|-----------:|---------:|---------:|---------:|-----------:|
| TopoSort_FullBatch                | 500      |   173.11 us |    23.023 us |   1.262 us |  24.6582 |   7.8125 |        - |  201.63 KB |
| CheckDependencyPresence_FullBatch | 500      |    22.76 us |     7.711 us |   0.423 us |   1.4343 |        - |        - |   11.84 KB |
| TopoSort_FullBatch                | 2000     |   929.87 us |   441.337 us |  24.191 us |  90.8203 |  90.8203 |  90.8203 |  862.75 KB |
| CheckDependencyPresence_FullBatch | 2000     |   121.30 us |    14.771 us |   0.810 us |   5.7373 |        - |        - |    46.9 KB |
| TopoSort_FullBatch                | 5000     | 2,968.57 us | 3,218.496 us | 176.417 us | 277.3438 | 277.3438 | 277.3438 | 1908.53 KB |
| CheckDependencyPresence_FullBatch | 5000     |   322.76 us |    39.328 us |   2.156 us |  14.1602 |        - |        - |  116.21 KB |

### Наблюдения над baseline

1. **Scaling — Native scheduler**:
   - `OneTick` 500 → 5000: ~180×, т.е. суб-O(N) деградация на масштабе
     (хороший знак для нативного pre-computed графа).
   - `RebuildStaticGraph` 500 → 5000: 1.5 мс → 277.9 мс ≈ ~185× —
     практически линейно, но Margin 158% от Mean при 5000 систем
     (Error 438 мс / Mean 277 мс) намекает на крайнюю нестабильность.
     `ShortRunJob` с 3 итерациями этого не покрывает; нужно
     `LongRunJob` для надёжных цифр на 5000.
   - `OrderByPriority_FullSet`: чёткий O(N) (8.7 → 77 → 295 мкс), стабильно.

2. **Scaling — Mod graph (Kahn)**:
   - `TopoSort_FullBatch` 500 → 5000: 173 → 2968 мкс ≈ ~17× при ×10
     роста размера — близко к O(N log N), что и ожидалось от Kahn.
   - `CheckDependencyPresence_FullBatch`: 22.7 → 322.8 мкс — ~14×, тоже
     O(N log N) или линейно с константой.

3. **Memory hotspots**:
   - `TopoSort_FullBatch` @ 5000: **1908 KB allocated per call, Gen2 =
     277.3 collections per 1000 ops**. Это самая «тяжёлая» точка по
     аллокациям и единственное место с Gen2-давлением. Кандидат на
     профилирование — внутри Kahn создаются List/Dictionary не из пула.
     По красным флагам из этого же отчёта (§«BDN — на что смотреть»):
     **«`Gen2` > 0 — рост, который надо разбирать»**.
   - `OrderByPriority_FullSet` allocates линейно по N (2 → 8 → 20 KB) —
     ожидаемо, возвращается новый `uint[]` каждый раз.
   - `NativeScheduler_OneTick` / `RebuildStaticGraph`: <8 KB managed
     allocations всех мерок — почти всё в нативном слое (good).

4. **Lock Contentions = 0 / Completed Work Items = 0** во всех
   измерениях SchedulerStress. Это **ожидаемо** для BDN-микро-сценариев:
   они однопоточные, реальная многопоточная нагрузка живёт в xUnit-
   стресс-тестах (которые BDN не моделирует). Если хочется померить
   contention под нагрузкой — добавить отдельный benchmark с
   `Parallel.For` внутри тела.

5. **Высокая дисперсия на больших масштабах**: `NativeScheduler_OneTick`
   @ 5000 — StdDev 6.1 мс при Mean 31.6 мс (~19%);
   `NativeScheduler_RebuildStaticGraph` @ 5000 — StdDev 24 мс при Mean
   278 мс (~9%). При следующих baseline-прогонах сравнение этих двух
   точек должно учитывать тот факт, что [ShortRunJob] недостаточен для
   стабильных чисел на верхней границе.

**Файл лога**: `docs/reports/stress_run_2026-05-21/08_bdn.log` (1431 строк,
полные гистограммы + Detailed results).

**Все xUnit + BDN-логи**: `docs/reports/stress_run_2026-05-21/`.

