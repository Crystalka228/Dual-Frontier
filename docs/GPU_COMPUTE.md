# GPU Compute — массовая физика за пределами World Fields

## Контекст

Отдельно от полей мира ([WORLDFIELDS](./WORLDFIELDS.md)) есть задачи массовой физики, которые также хорошо ложатся на GPU compute: позиции снарядов, broad-phase коллизий, flood-fill зон достижимости. Они не являются полями (не распределены по всей карте), но имеют ту же природу — однородные независимые вычисления над массивом.

## Почему не World Fields

Поля мира — **скалярные значения на сетке тайлов**. Массовая физика — **произвольный список объектов с позициями**. Это разные структуры данных, разные compute shaders, разный размер буферов. Смешивать их в одном модуле было бы неправильной абстракцией.

Для массовой физики используется параллельный паттерн: отдельный интерфейс в Infrastructure слое, CPU fallback, GPU-реализация при превышении порога.

## ProjectileSystem — первый кандидат (Фаза 5)

`ProjectileSystem` помечена REALTIME (каждый кадр). При типичной нагрузке (10–20 пешек, ~100 снарядов) CPU справляется с запасом. GPU compute оправдывает overhead на dispatch начиная от ~500 одновременных снарядов.

**Архитектурный паттерн:**

```csharp
// DualFrontier.Contracts/Infrastructure/IProjectileCompute.cs
public interface IProjectileCompute
{
    void Step(Span<ProjectileData> projectiles, float delta);
}
```

Реализации:
- `CpuProjectileCompute` — дефолт.
- `GpuProjectileCompute` — compute shader + асинхронный readback с буферизацией на 1 тик.

`ProjectileSystem` остаётся в Domain, внутри делегирует расчёт. Domain не знает какая реализация активна. Сторож изоляции не затрагивается.

**Порог переключения:** измеряется в stress-тесте «Битва богов» (500 магов × спам заклинаниями = ~5 000 снарядов + ~50 000 коллизий/сек). До этого порога CPU эффективнее из-за dispatch overhead.

## SpatialGrid broad-phase (Фаза 5+, исследование)

При 5 000+ entity в бою broad-phase коллизий (кто рядом с кем) можно параллельно считать на GPU, возвращая пары-кандидаты. Narrow phase (реальная коллизия) остаётся на CPU.

Статус: TODO на случай если встретим реальную деградацию CPU SpatialGrid в бенчмарках Фазы 5.

## Pathfinding flood-fill (Фаза 3+, исследование)

Сам A* плохо ложится на GPU (ветвления, приоритетная очередь). Но **предвычисление карты зон достижимости** — flood-fill от опорных точек — идеально параллельно. GPU flood-fill один раз при изменении карты даёт готовую карту расстояний; A* каждой пешки просто читает эту карту как эвристику.

Статус: TODO если карты станут больше 200×200 и A* станет bottleneck.

## Бенчмарк (TODO Фаза 5)

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class ProjectileStressBenchmark
{
    [Params(100, 500, 1000, 5000)] public int ProjectileCount;

    [Benchmark(Baseline = true)] public void Cpu() { /* CpuProjectileCompute */ }
    [Benchmark]                  public void Gpu() { /* GpuProjectileCompute */ }
}
```

Зафиксировать порог переключения CPU→GPU в этом документе.

## Полная карта GPU в проекте

| Задача | Модуль | Фаза | Порог |
|---|---|---|---|
| Диффузия эфира | WorldFields.EtherLayer | 6 | Любой размер карты |
| Температура | WorldFields.TemperatureLayer | 7 | Любой размер карты |
| Туман | WorldFields.FogLayer | 7 | Любой размер карты |
| Погода (визуал) | WorldFields.WeatherLayer | 7 | Всегда дёшево |
| Позиции снарядов | IProjectileCompute | 5 | 500+ снарядов |
| Broad-phase коллизий | SpatialGrid (GPU backend) | 5+ | 5 000+ entity |
| Flood-fill зон | Pathfinding (GPU backend) | 3+ | Карта > 200×200 |
| LLM-нарратор | OllamaLlmNarrator | 8 | VRAM ≥ 6 ГБ |

## См. также

- [WORLDFIELDS](./WORLDFIELDS.md) — отдельный модуль для полей мира
- [PERFORMANCE](./PERFORMANCE.md) — целевые метрики и бенчмарки
- [ARCHITECTURE](./ARCHITECTURE.md) — правила зависимостей
- [ROADMAP](./ROADMAP.md) — Фазы 3, 5, 6, 7
