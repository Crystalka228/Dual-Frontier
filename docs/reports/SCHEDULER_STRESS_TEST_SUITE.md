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
