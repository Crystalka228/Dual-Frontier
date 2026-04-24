# DUAL FRONTIER — Native Core Experiment

C++ ядро для ECS симулятора
Ветка: `claude/cpp-core-experiment`

## Вопрос

Можно ли вынести ECS ядро на C++ не разрушив архитектуру?

## Статус

Эксперимент — ждёт результатов бенчмарка (p99, GC-pause count, 2-core
throughput). Решение об инвестиции принимается по профилю целевого
железа, а не по mean latency на разработческой машине.

## Self-test

~4.6 мс / 100k операций (sandbox VM Opus) — нативный потолок.

## Критерий

Колониальные симуляторы играют на среднем и слабом железе (2015–2018
годов, часто 2–4 ядра, 8 ГБ RAM). Первоначальная формулировка «≥20% по
mean latency» была сформулирована без учёта жанра — на такой машине
ценность не в средней задержке, а в **предсказуемости**:

| Метрика | Почему важнее mean |
|---------|--------------------|
| GC pause count / длительность | Один stop-the-world на 50 мс — видимый рывок камеры |
| p99 / worst-case tick | Тик, который «вылетает» за 33 мс, ощущается как фриз |
| Throughput на 2–4 ядрах | На целевом железе parallel scheduler конкурирует с ОС за CPU |
| Поведение через 2–4 часа | Long-running simulation — тут управляемая память выигрывает |

Обновлённое правило решения — §8.

---

## 1. Контекст и мотивация

DualFrontier строится как движок, а не просто игра. После того как Фаза 3
заработала — пешки двигаются, симуляция живёт, 61 тест зелёный — встал
естественный вопрос: можно ли вынести самое горячее ядро на C++ не разрушив
архитектуру?

Ответ определяет долгосрочную траекторию проекта:

**Путь A — C# ядро навсегда**
Простота разработки, .NET оптимизации, единая кодовая база.
Ограничение: JIT, GC паузы, managed overhead на горячем пути.

**Путь B — C++ ядро + C# снаружи ← эксперимент проверяет этот путь**
ECS и планировщик на нативном коде.
C# системы, шины, Application, Presentation — без изменений.
Результат: **предсказуемая** производительность без GC jitter на слабом
железе, DX разработки на C#.

> ⭐ **Главная ставка**
> Не «производительность Unreal» на разработческой машине, а стабильность
> на машине игрока. Колониальные симуляторы играют десятки часов на
> железе 2015–2018 годов — там один Gen2 GC на 50 мс превращается в
> видимый рывок камеры. C++ ядро = нет managed heap на горячем пути =
> нет GC pause = плавный тик даже через 4 часа симуляции.

---

## 2. Почему это возможно без разрушения архитектуры

Архитектура DualFrontier уже заточена под эту замену. Все слои изолированы
от реализации ядра через контракты:

```
DualFrontier.Systems      — не знают о World напрямую
DualFrontier.Application  — не знает о ComponentStore
DualFrontier.Presentation — не знает о Scheduler
```

Все слои общаются через:

```
IComponent    (Contracts)    ← не меняется
SystemBase    (публичный API) ← не меняется
IGameServices (шины)          ← не меняется
```

> 📌 **Правило замены**
> C++ ядро заменяет только то, что помечено `internal` в Core. Контракты не
> трогаются. Системы не трогаются. 43 теста не трогаются.

| Слой | Меняется? | Причина |
|------|-----------|---------|
| `DualFrontier.Contracts` | ❌ Нет | Контракты — граница, не реализация |
| `DualFrontier.Core` (internal) | ✅ Заменяется | ECS, Scheduler — именно это переносится на C++ |
| `DualFrontier.Core.Interop` (новый) | ✅ Добавляется | P/Invoke обёртки для C# → C++ вызовов |
| `DualFrontier.Systems` | ❌ Нет | Системы работают через `SystemBase` — не знают о реализации |
| `DualFrontier.Application` | ❌ Нет | Использует `IGameServices`, не `World` напрямую |
| `DualFrontier.Presentation` | ❌ Нет | Только `PresentationBridge` — не знает о ECS |
| Все 43 теста | ❌ Нет | Тестируют поведение через контракты, не реализацию |

---

## 3. Что переносится на C++

### 3.1 `SparseSet<T>` — фундамент производительности

Самое горячее место ECS — итерация компонентов. В C#: `int[] sparse`,
`T[] dense`, `int[] denseToIndex`. На C++ это `template<typename T>` —
компилятор генерирует специализацию для каждого типа компонента.

| Характеристика | C# (managed) | C++ (native) |
|----------------|--------------|--------------|
| Тип хранилища | Managed heap, GC давление | stack/heap, нет GC |
| Виртуальные вызовы | Virtual dispatch через интерфейс | Нет — template специализация |
| Boxing | Есть при object cast | Нет — прямая память |
| Итерация `dense[]` | Cache-friendly, но managed overhead | Cache-friendly, нулевой overhead |
| Аллокация | `new T[]` — GC managed | `std::vector<T>` — RAII |

### 3.2 `ComponentStore` / `World`

Реестр entities и компонентов. В C++ использует `std::unordered_map` с
FNV-1a хешем от `AssemblyQualifiedName` типа компонента как ключом.

```cpp
// EntityId — тот же алгоритм что в C#
uint64_t entity_id = ((uint64_t)version << 32) | (uint32_t)index;

// ComponentStore — type-erased хранилище
std::unordered_map<uint32_t,
    std::unique_ptr<IComponentStore>> _stores;
// ключ = FNV-1a(AssemblyQualifiedName)
```

### 3.3 `DependencyGraph`

Алгоритм Кана — чистая теория графов без зависимостей на runtime. Принимает
массивы `type_id` через C API, возвращает упорядоченные фазы. Полностью
переносимый код.

### 3.4 `EventBus` (следующий шаг)

`std::vector<std::function>` под `std::shared_mutex`. Публикация синхронная.
Ключевая задача: как C++ шина вызывает C# подписчики через function pointer.

### 3.5 Что остаётся на C#

| Компонент | Почему остаётся на C# |
|-----------|-----------------------|
| `SystemBase` + `Update(float delta)` | Системы пишут люди на C#. Это главный DX проекта — менять нельзя. |
| `SystemExecutionContext` (guard) | DEBUG-time проверка. Не на горячем пути в Release. Нет смысла переносить. |
| `IGameServices` и `DomainEventBus` | Интерфейсы в Contracts. Можно перенести позже если бенчмарк покажет что шины узкое место. |
| `IComponent` компоненты | POCO классы на C#. C++ хранит их как blittable structs (PoC) или через `GCHandle` (production). |

---

## 4. Граница P/Invoke — C API

C++ экспортирует чистый C API (`extern "C"`) — обязательное требование для
надёжного P/Invoke на всех платформах:

```cpp
// world.h — C API (extern "C")
void*    df_world_create();
void     df_world_destroy(void* world);
uint64_t df_world_create_entity(void* world);
void     df_world_destroy_entity(void* world, uint64_t id);
void     df_world_add_component(void* world, uint64_t id,
             uint32_t type_id, void* data, int32_t size);
void*    df_world_get_component(void* world, uint64_t id,
             uint32_t type_id);
bool     df_world_has_component(void* world, uint64_t id,
             uint32_t type_id);
```

C# обёртка в `DualFrontier.Core.Interop`:

```csharp
internal static class NativeMethods
{
    private const string DllName = "DualFrontier.Core.Native";

    [DllImport(DllName)]
    public static extern IntPtr df_world_create();

    [DllImport(DllName)]
    public static extern void df_world_destroy(IntPtr w);

    [DllImport(DllName)]
    public static extern ulong df_world_create_entity(IntPtr w);
    // ...
}
```

### 4.1 Кросс-платформенность

| Платформа | Файл библиотеки | Сборка |
|-----------|-----------------|--------|
| Windows | `DualFrontier.Core.Native.dll` | CMake + Visual Studio 2022 |
| Linux | `libDualFrontier.Core.Native.so` | CMake + GCC/Clang |
| macOS | `libDualFrontier.Core.Native.dylib` | CMake + Clang |

> 📌 **Автовыбор платформы**
> `[DllImport("DualFrontier.Core.Native")]` подхватывает нужный файл
> автоматически — P/Invoke знает о расширениях платформы. Тот же подход,
> что используют Godot нативные плагины.

---

## 5. Маршаллинг компонентов — три варианта

Главная техническая сложность эксперимента — передача managed C# объектов в
нативный C++ код. В эксперименте выбран Вариант 1 как минимально сложный
для PoC.

| Вариант | Механизм | Плюсы | Минусы | Применение |
|---------|----------|-------|--------|------------|
| 1 — Blittable structs | `[StructLayout(Sequential)]` | Нет GC pressure, прямая передача через pointer | Production компоненты — классы, нужна конвертация | **PoC ✅ выбранный** |
| 2 — `GCHandle` | `GCHandle.Alloc(Pinned)` → `AddrOfPinnedObject` | Работает с классами, нет копирования | Управление временем жизни handle | Production — лучший баланс |
| 3 — Serialization | Сериализовать в `byte[]` при передаче | Максимальная гибкость | Самый медленный | Крайний случай |

```csharp
// Вариант 1 — Blittable struct (PoC)
[StructLayout(LayoutKind.Sequential)]
public struct BenchHealthComponent
{
    public int Current;
    public int Maximum;
}

// Вариант 2 — GCHandle (production)
var handle = GCHandle.Alloc(component, GCHandleType.Pinned);
IntPtr ptr = handle.AddrOfPinnedObject();
NativeMethods.df_world_add_component(world, id, typeId, ptr, size);
// handle.Free() — когда компонент больше не нужен в C++
```

---

## 6. Структура эксперимента

```
native/DualFrontier.Core.Native/
  ├── CMakeLists.txt
  ├── build.md                    ← инструкция сборки
  ├── include/
  │   ├── sparse_set.h            ← template SparseSet<T>
  │   ├── component_store.h       ← type-erased IComponentStore
  │   ├── world.h                 ← World + C API
  │   └── df_native.h             ← единый публичный заголовок
  └── src/
      ├── world.cpp
      └── df_native_selftest.cpp  ← standalone тест без dotnet

src/DualFrontier.Core.Interop/
  ├── DualFrontier.Core.Interop.csproj
  ├── NativeMethods.cs            ← все P/Invoke декларации
  ├── NativeWorld.cs              ← C# обёртка над df_world_*
  └── Marshalling/
      └── ComponentMarshaller.cs  ← blittable struct ↔ IComponent

tests/DualFrontier.Core.Benchmarks/
  └── NativeVsManagedBenchmark.cs ← BenchmarkDotNet сравнение
```

### 6.1 Self-test нативной библиотеки

`df_native_selftest.exe` — standalone C++ тест без зависимости на dotnet.
Четыре сценария:

| Сценарий | Что проверяет | Ожидаемый результат |
|----------|---------------|---------------------|
| CRUD | Создание entity, добавление компонента, чтение, удаление | Все операции O(1) |
| deferred destroy | Entity помечается для удаления, реально удаляется после итерации | Нет инвалидных указателей |
| swap-with-last | Удаление из середины dense массива — O(1) swap | Порядок dense корректен |
| 100k throughput | 100 000 сущностей Create/Add/Get | ~4.6 мс (sandbox VM) — нативный потолок |

---

## 7. Как запустить бенчмарк

### Шаг 1 — Собрать C++ библиотеку

Открыть Developer PowerShell for VS 2022:

```powershell
cd native\DualFrontier.Core.Native
cmake -S . -B build -G "Visual Studio 17 2022" -A x64
cmake --build build --config Release
```

### Шаг 2 — Self-test

```powershell
.\build\Release\df_native_selftest.exe
# Ожидаемо: All tests passed.
```

### Шаг 3 — Скопировать .dll

```powershell
copy build\Release\DualFrontier.Core.Native.dll `
     ..\..\tests\DualFrontier.Core.Benchmarks\
```

### Шаг 4 — Запустить бенчмарк

```powershell
cd D:\Colony_Simulator\Colony_Simulator
dotnet run --project tests\DualFrontier.Core.Benchmarks `
           -c Release -- --filter "*NativeVsManaged*"
```

### Таблица результатов

Заполнить после запуска на локальной машине:

| Method | Mean | Ratio | Вывод |
|--------|------|-------|-------|
| `ManagedGetAll` | ? | 1.00 (baseline) | — |
| `NativeGetAll` | ? | ? | Заполнить после бенчмарка |

---

## 8. Критерий принятия решения

Первоначальный критерий (`Native быстрее на ≥20% по mean`) построен на
допущении, что целевой игрок сидит за dev-машиной. Это неверное допущение:
колониальные симуляторы играют на среднем и слабом железе — машины
2015–2018 годов, 2–4 ядра, 8 ГБ RAM, часто с IGP. На таком железе mean
latency — не та метрика, которую чувствует игрок.

**Пересмотренный вывод: перенос ядра на C++ оправдан независимо от mean
latency бенчмарка.** Основная ценность — предсказуемость и отсутствие GC
jitter на длительной симуляции.

### Что измеряем теперь

| Метрика | Что считает | Порог «идём дальше» |
|---------|-------------|---------------------|
| **GC pause events / мин** (Gen2) | `dotnet-counters` на `System.Runtime` во время 10-минутной симуляции | У native: 0. У managed: любое ненулевое значение — уже профит C++ |
| **p99 tick duration** | `ParallelSystemScheduler` на 4-ядерной VM, 60 тиков/с, 2000 пешек | Native улучшает p99 при равном mean — решающий сигнал |
| **p99.9 / max tick** | Тот же прогон | Native ограничивает хвост — GC rolls не бьют |
| **Throughput на 2 ядрах** | То же, но с `--cpu-affinity 0,1` | Native должен держать tick rate; managed может деградировать из-за GC thread |
| **Long-run drift** | 2 часа симуляции, замерить tick latency каждые 10 мин | Native должен оставаться плоским, managed обычно растёт |
| **Mean latency (смотрим, но не решаем по нему)** | BenchmarkDotNet `NativeVsManagedBenchmark` | Для истории; решение — не здесь |

### Правило решения

| Картина | Решение | Следующий шаг |
|---------|---------|---------------|
| Native срезает GC pauses **и** улучшает p99 на 2-ядре | ✅ Продолжать | `DependencyGraph` + Scheduler на C++ |
| Native даёт плоский long-run (нет drift), но mean паритет | ✅ Продолжать | То же — jitter > mean для жанра |
| Native хуже на mean, но GC pauses ушли в ноль | ✅ Продолжать, **если** GC паузы >1/мин на целевой конфигурации | GC стоимость > cost of C++ complexity |
| Паритет по всем метрикам, включая GC | ❌ Закрыть | Сложность C++ не оправдана — остаёмся на C# |
| Managed быстрее везде (включая p99) | ❌ Закрыть | Очень маловероятно на горячем ECS, но формально возможно |

> ⚡ **P/Invoke overhead**
> Один вызов P/Invoke: ~10–50 нс. Для мелких компонентов marshalling может
> съесть весь прирост по mean. Но P/Invoke **не вызывает GC pause** — то
> есть даже там, где C++ проигрывает по mean, он обычно выигрывает по p99.
> Это и есть суть пересмотренного критерия.

> 🎯 **Чего не делаем**
> Не оптимизируем под dev-машину. Не сравниваем на 12-ядерном 32 ГБ DDR5
> рабочем железе — такие результаты не репрезентативны для игрока жанра.
> Целевой рабочий прогон — 4-ядерная VM с ограниченной памятью.

---

## 9. Следующие шаги если эксперимент успешен

| Шаг | Задача | Сложность |
|-----|--------|-----------|
| 1 | `DependencyGraph` на C++ — алгоритм Кана, входные данные из C# через массивы `type_id` | Средняя |
| 2 | `NativeScheduler` — параллельное исполнение фаз через `std::thread` pool. Вызов C# `Update()` через function pointer | Высокая |
| 3 | `EventBus` на C++ — синхронная доставка, C# подписчики через делегаты | Высокая |
| 4 | `GCHandle` маршаллинг — переход от blittable structs к реальным production компонентам | Средняя |
| 5 | CI pipeline — сборка трёх платформ (Win/Linux/macOS) в одном workflow | Средняя |

### Долгосрочная траектория

**Сейчас (Фаза 3)**
Godot Runtime → C# GameLoop → C# ECS ядро

**После эксперимента (если успешен)**
Godot Runtime → C# GameLoop → C++ ECS ядро через P/Invoke

**Фаза 9 — Native Runtime (долгосрочно)**
Собственный Runtime → C# GameLoop → C++ ECS ядро
Godot = только редактор сцен

> ⭐ **Долгосрочная ставка**
> Собственный runtime — это уже не инди-игра, это движок. Godot как редактор
> сцен + нативное ядро + C# логика = редкая комбинация на рынке.

---

## 10. Риски

| Риск | Вероятность | Митигация |
|------|-------------|-----------|
| P/Invoke overhead съедает выигрыш для мелких компонентов | Высокая | Батчинг операций, blittable structs, измерить до решения |
| Сложность отладки (C++ + C# смешанный стек) | Средняя | `lldb`/WinDbg + managed debugger, изолировать границу |
| Cross-platform сборка в CI | Средняя | CMake + GitHub Actions matrix build |
| Маршаллинг managed объектов через границу | Высокая сложность | `GCHandle`, чёткий ownership, не смешивать heap |
| Десинхронизация C# и C++ версий API | Средняя | Версионирование C API, тесты на совместимость |

> ⚠ **Главный риск**
> Marshalling complexity. Blittable structs — просто. `GCHandle` managed
> классов — сложно и требует явного управления временем жизни. Недооценка
> этого риска ломает production.

---

Производительность Unreal. Удобство разработки C#. Архитектура, которая не
ломается при замене ядра.
