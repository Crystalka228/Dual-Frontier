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

Документы для тех, кто пишет код: методика, модинг, производительность, стиль, тесты.

- [METHODOLOGY](./METHODOLOGY.md) — четырёхагентный pipeline, контракты как IPC между агентами, цикл верификации, модель угроз, экономика, границы применимости.
- [MODDING](./MODDING.md) — IMod, IModApi, AssemblyLoadContext, IModContract, манифест.
- [MOD_PIPELINE](./MOD_PIPELINE.md) — конвейер интеграции, ContractValidator, ModRegistry, атомарность.
- [PERFORMANCE](./PERFORMANCE.md) — целевые метрики, профилирование, горячие пути, кэши.
- [GPU_COMPUTE](./GPU_COMPUTE.md) — исследование compute shader для ProjectileSystem, порог переключения.
- [CODING_STANDARDS](./CODING_STANDARDS.md) — naming, file-scoped namespaces, nullable, порядок членов.
- [TESTING_STRATEGY](./TESTING_STRATEGY.md) — unit, integration, isolation, modding, performance.
- [DEVELOPMENT_HYGIENE](./DEVELOPMENT_HYGIENE.md) — чек-лист гигиены на каждый PR, граница движок/игра, красные флаги.
- [NATIVE_CORE_EXPERIMENT](./NATIVE_CORE_EXPERIMENT.md) — C++ ядро эксперимент, P/Invoke граница, результаты бенчмарка, план батчинг API для Фазы 9.
- [PERSISTENCE](../src/DualFrontier.Persistence/README.md) — алгоритмы сжатия сохранений: RLE тайлов, квантизация компонентов, range encoding entity, StringPool.

## Процесс

Организация работы: порядок фаз и критерии приёмки.

- [ROADMAP](./ROADMAP.md) — фазы 0–7 с составом файлов, критериями приёмки и разблокировкой следующих.

## Учебные материалы

Артефакты self-teaching ритуала после закрытия каждой существенной фазы (см. [METHODOLOGY §4.5](./METHODOLOGY.md)).

- [learning/PHASE_1](./learning/PHASE_1.md) — C# и многопоточность через призму Core ECS: class vs struct, generics, атрибуты через Reflection, nullable, ThreadLocal, race conditions, stack trace, тесты как доказательство инварианта. Включает 14-дневный учебный маршрут.

## Логи сессий

Точные логи фазовых ревью и других ключевых сессий pipeline. Не подлежат редактированию задним числом — служат аудитным следом.

- [SESSION_PHASE_4_CLOSURE_REVIEW](./SESSION_PHASE_4_CLOSURE_REVIEW.md) — закрытие Phase 4 силами Opus 4.7: валидация диагностики, 6 архитектурных решений, 17 новых тестов, 7 атомарных коммитов.

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
