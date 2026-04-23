# Документация Dual Frontier

Dual Frontier — колониальный симулятор на Godot 4 + C# с двумя параллельными ветками технологий (индустриальная и аркано-магическая). Архитектура проекта решает три проблемы RimWorld: производительность, многопоточность и модифицируемость.

Данный каталог содержит всю техническую и процессную документацию. Перед погружением в код рекомендуется прочитать [ARCHITECTURE](./ARCHITECTURE.md) и [CONTRACTS](./CONTRACTS.md) — без них структура исходников выглядит избыточно.

## Архитектура

Документы, описывающие фундамент: слои, контракты, ядро ECS, шины событий, многопоточность и изоляцию.

- [ARCHITECTURE](./ARCHITECTURE.md) — четыре слоя, правила зависимостей, диаграмма сборок.
- [CONTRACTS](./CONTRACTS.md) — маркер-интерфейсы, пять доменных шин, эволюция и версионирование.
- [ECS](./ECS.md) — World, EntityId, Component, SparseSet, Query, SystemBase.
- [EVENT_BUS](./EVENT_BUS.md) — доменные шины, двухшаговая модель Intent→Granted/Refused, батч-обработка.
- [THREADING](./THREADING.md) — DependencyGraph, фазы, TickRates, запрет async.
- [ISOLATION](./ISOLATION.md) — SystemExecutionContext, сторож, DEBUG vs RELEASE, типы нарушений.
- [GODOT_INTEGRATION](./GODOT_INTEGRATION.md) — PresentationBridge, IRenderCommand, InputRouter, main thread.
- [VISUAL_ENGINE](./VISUAL_ENGINE.md) — DevKit vs Native, контракты IRenderer/ISceneLoader/IInputSource, формат .dfscene.

## Разработка

Документы для тех, кто пишет код: модинг, производительность, стиль, тесты.

- [MODDING](./MODDING.md) — IMod, IModApi, AssemblyLoadContext, IModContract, манифест.
- [MOD_PIPELINE](./MOD_PIPELINE.md) — конвейер интеграции, ContractValidator, ModRegistry, атомарность.
- [PERFORMANCE](./PERFORMANCE.md) — целевые метрики, профилирование, горячие пути, кэши.
- [CODING_STANDARDS](./CODING_STANDARDS.md) — naming, file-scoped namespaces, nullable, порядок членов.
- [TESTING_STRATEGY](./TESTING_STRATEGY.md) — unit, integration, isolation, modding, performance.

## Процесс

Организация работы: порядок фаз и критерии приёмки.

- [ROADMAP](./ROADMAP.md) — фазы 0–7 с составом файлов, критериями приёмки и разблокировкой следующих.

## v02 Addendum

Дополнения второй ревизии архитектуры: модели ресурсов, composite-запросы, обратные связи через tick lag, детерминированная резолюция урона, переходы владения големом.

- [RESOURCE_MODELS](./RESOURCE_MODELS.md) — Intent vs Lease, правило выбора, reserve-then-consume.
- [COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md) — двухфазный commit для multi-bus запросов, `CompositeResolutionSystem`.
- [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md) — `Mana[N-1]`, снимки предыдущего тика.
- [COMBO_RESOLUTION](./COMBO_RESOLUTION.md) — `ComboResolutionSystem`, детерминированная сортировка `DamageIntent`.
- [OWNERSHIP_TRANSITION](./OWNERSHIP_TRANSITION.md) — состояния `GolemBondComponent`, таблица переходов.

## См. также

- [ARCHITECTURE](./ARCHITECTURE.md)
- [ROADMAP](./ROADMAP.md)
- [CONTRACTS](./CONTRACTS.md)
