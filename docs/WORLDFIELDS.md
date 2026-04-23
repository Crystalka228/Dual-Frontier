# World Fields Module — Симуляция полей мира

## Контекст

Dual Frontier имеет несколько «полей мира» — распределённых по тайловой карте скалярных значений, обновляющихся каждый тик: эфир, температура, туман, погода. Все эти поля по природе — **однородная диффузия на сетке без ветвлений**: идеальный случай для GPU compute.

Ни RimWorld, ни Dwarf Fortress, ни Oxygen Not Children не используют GPU для полевых расчётов. Они решают аналогичные задачи (тепло, газ, загрязнение) тщательно оптимизированным однопоточным CPU-кодом — Factorio потратил на это годы. GPU-диффузия достигает того же результата архитектурно, без накопления технического долга, и масштабируется на карты любого размера.

## Принцип изоляции модуля

Модуль `DualFrontier.WorldFields` полностью слеп к остальной архитектуре. Domain-системы знают только один интерфейс — `IWorldFieldCompute`. Остальные слои модуля (pipeline, double buffer, backends, layers) живут в сборке `DualFrontier.WorldFields` и снаружи не видны.

```
Domain видит только:
  IWorldFieldCompute.Submit(FieldUpdate update)

Presentation видит только:
  WorldFieldSnapshot.GetTexture(FieldKind kind)

Core не знает о модуле вообще.
DualFrontier.WorldFields не знает о Systems, ECS, планировщике.
```

Модуль можно выключить, заменить, выгрузить — ничего не сломается. Системы просто перестанут получать красивые оверлеи.

## Структура сборки

```
DualFrontier.WorldFields/
  ├── Contracts/
  │     ├── IWorldFieldCompute.cs      ← единственное что видит Domain
  │     ├── IFieldLayer.cs             ← маркер слоя (эфир, темп, погода)
  │     ├── FieldKind.cs               ← enum: Ether, Temperature, Fog, Weather
  │     ├── FieldUpdate.cs             ← readonly record — payload от Domain
  │     └── WorldFieldSnapshot.cs      ← readonly результат для Presentation
  ├── Pipeline/
  │     ├── WorldFieldPipeline.cs      ← оркестратор passes
  │     ├── DoubleBuffer.cs            ← A/B чередование
  │     └── CommandBufferBuilder.cs    ← сборка GPU dispatch
  ├── Layers/
  │     ├── EtherLayer.cs              ← диффузия эфира
  │     ├── TemperatureLayer.cs        ← распространение тепла
  │     ├── FogLayer.cs                ← туман войны / эфирный туман
  │     └── WeatherLayer.cs            ← осадки, грозы, эфирные бури
  ├── Backends/
  │     ├── CpuWorldFieldBackend.cs    ← дефолт, без GPU
  │     └── GpuWorldFieldBackend.cs    ← Vulkan compute (Godot 4)
  └── README.md
```

## Граница с остальными слоями

**Только две точки контакта:**

```
Domain (EtherGrowthSystem, WeatherSystem, BiomeSystem)
  │
  │ IWorldFieldCompute.Submit(update)
  ▼
DualFrontier.WorldFields (модуль)
  │
  │ WorldFieldSnapshot (readonly)
  ▼
Presentation (PresentationBridge → Godot overlays)
```

Regisration через `Application` слой при старте:

```csharp
// GameBootstrap.cs (DualFrontier.Application)
var backend = GpuDetector.IsSupported() && mapSize >= 200
    ? new GpuWorldFieldBackend()
    : new CpuWorldFieldBackend();

var pipeline = new WorldFieldPipeline(backend);
services.Register<IWorldFieldCompute>(pipeline);
services.Register<IWorldFieldSnapshotSource>(pipeline);
```

Domain-системы получают `IWorldFieldCompute` через инъекцию — никакой статики, никаких синглтонов.

## Double buffering

Два буфера A и B чередуются каждый кадр:

```
Кадр N:   GPU пишет в B, Presentation читает из A
Кадр N+1: GPU пишет в A, Presentation читает из B
```

Никаких блокировок, никаких stall. Presentation всегда имеет готовый кадр для отрисовки.

## Temporal upsampling

Симуляция всегда на 1 кадр позади визуала. При 60 FPS это 16.6 мс задержки — физически не воспринимается игроком, но даёт полностью плавный визуал без пропусков:

```
Кадр N:   GPU считает эфир/погоду/туман → результат в буфере A
Кадр N+1: GPU рендерит из буфера A + считает следующий тик → буфер B
Кадр N+2: рендерит из B, считает в A...
```

Симуляция не знает о буферизации — она просто вызывает `Submit()` каждый тик. Pipeline разруливает чередование сам.

## Единый compute pipeline

Все четыре поля считаются в одном command buffer за один dispatch:

```
Compute Pass 1 — симуляция (каждые N кадров по TickRate):
  EtherDiffuse    → etherMap
  TemperatureStep → tempMap
  WeatherStep     → weatherMap
  FogUpdate       → fogMap

Compute Pass 2 — визуализация (каждый кадр):
  EtherGlow       → texture (подсветка узлов)
  WeatherOverlay  → texture (туман/дождь/снег)
  FogOfWar        → texture (затенение неразведанного)
```

Passes 1 и 2 в одном command buffer — GPU не простаивает между ними. Это экономит command buffer overhead по сравнению с раздельными системами.

## Порядок слоёв в compute pass

Слои выполняются в детерминированном порядке, потому что поля зависят друг от друга:

1. **EtherLayer** — базовое поле, не зависит от других.
2. **TemperatureLayer** — может зависеть от эфира (эфирная буря нагревает воздух).
3. **WeatherLayer** — зависит от температуры (осадки при низкой температуре → снег).
4. **FogLayer** — зависит от всех предыдущих (туман формируется из эфира + температуры + погоды).

Порядок фиксирован в `WorldFieldPipeline`. Добавление нового слоя — явное указание позиции через `[FieldLayerOrder(N)]` атрибут.

## Ожидаемые характеристики

| Размер карты | Все 4 поля CPU | Все 4 поля GPU |
|---|---|---|
| 100×100 (10k тайлов)  | ~1.5 мс/тик | ~0.08 мс/тик |
| 300×300 (90k тайлов)  | ~7 мс/тик   | ~0.2 мс/тик  |
| 500×500 (250k тайлов) | деградация  | ~0.4 мс/тик  |

При карте 300×300 CPU отдаёт 7 мс из бюджета 16.6 мс только на полевые расчёты. GPU держит 0.2 мс — освобождает 6.8 мс для игровой логики (пешки, AI, экономика).

## Порог активации GPU backend

`GpuWorldFieldBackend` активируется автоматически при всех условиях:

- GPU поддерживает compute shaders (Godot 4 — Vulkan / Metal / DX12).
- Размер карты ≥ 200×200 тайлов.
- В settings.json не выставлен `ForceCpuFieldBackend: true` (для тестирования / отладки).

Ниже порога активен `CpuWorldFieldBackend` — корректная реализация тех же формул без зависимости от GPU.

## Readback латентность

GPU compute требует readback для систем, которым нужны точные значения (EtherGrowthSystem публикует `EtherSurgeEvent` при пересечении порога). При SLOW/NORMAL тиках (15–60 фреймов) lag на 1 тик полностью незаметен — узел набирает критическую плотность 10+ секунд, 16.6 мс задержки срабатывания порога не меняют ничего.

Для систем, которым не нужны значения (только визуализация — FogOfWar, WeatherOverlay), readback не выполняется вообще. Буфер остаётся на GPU и используется только для рендера.

## Интеграция с существующими системами

| Domain-система | Использование |
|---|---|
| `EtherGrowthSystem` (SLOW) | Submit FieldUpdate для EtherLayer; читает снимок для EtherSurgeEvent |
| `EtherGridSystem` (NORMAL) | Читает снимок эфира для распределения потребителям |
| `WeatherSystem` (RARE) | Submit FieldUpdate для WeatherLayer |
| `BiomeSystem` (RARE) | Читает снимок для определения типа биома тайла |
| `ConverterSystem` (NORMAL) | Читает локальную плотность эфира через снимок |

Все системы остаются в Domain с прежними `[SystemAccess]` декларациями. Они получают `IWorldFieldCompute` через инъекцию в конструкторе — никаких изменений в ECS / планировщике / сторожа изоляции.

## Запрет на прямую работу с GPU

Domain-системы **не имеют права** напрямую работать с compute shaders, GPU-буферами, Godot RenderingDevice. Единственный путь — через `IWorldFieldCompute`. Нарушение ловится на ревью архитектуры (отдельных автоматических проверок нет — модуль запечатан в собственной сборке, снаружи `internal` не виден).

## Планы развития

- **Фаза 6**: EtherLayer + CpuWorldFieldBackend. Базовая диффузия эфира.
- **Фаза 6.5**: GpuWorldFieldBackend + EtherLayer на GPU. Бенчмарк, фиксация порога.
- **Фаза 7**: TemperatureLayer, WeatherLayer, FogLayer. Полный pipeline.
- **После Фазы 7**: модинг API — моды могут регистрировать собственные `IFieldLayer` через `IModApi`. Это сделает модуль расширяемым без правок ядра.

## См. также

- [GPU_COMPUTE](./GPU_COMPUTE.md) — частный случай: ProjectileSystem compute (не поле мира, но того же класса задач)
- [PERFORMANCE](./PERFORMANCE.md) — целевые метрики и бенчмарки
- [ARCHITECTURE](./ARCHITECTURE.md) — слои и правила зависимостей
- [THREADING](./THREADING.md) — модель потоков, запрет async
- [ROADMAP](./ROADMAP.md) — Фаза 6, Фаза 7
