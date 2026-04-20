# DualFrontier.Core.Tests

## Назначение
Юнит-тесты ядра: ECS (`World`, `ComponentStore`), планировщик
(`ParallelSystemScheduler`, `DependencyGraph`), шина событий
(`DomainEventBus`) и сторож изоляции (`SystemExecutionContext`).

## Зависимости
- `DualFrontier.Core`
- xUnit 2.9+, FluentAssertions 6.12+

## Что внутри
- `ECS/` — тесты `World`, `ComponentStore`, `EntityId` (версии, удаление, переиспользование).
- `Scheduling/` — тесты `DependencyGraph`, `ParallelSystemScheduler`.
- `Bus/` — тесты `DomainEventBus` и публикации/подписки.
- `Isolation/` — тесты сторожа (`IsolationViolationException` при нелегальном доступе).

## Правила
- Один тестовый класс — одна тема. Имя файла совпадает с темой.
- Тесты — чистые: никаких статических состояний, никаких файлов.
- FluentAssertions стиль: `actual.Should().Be(expected);`.

## Примеры использования
Запуск: `dotnet test tests/DualFrontier.Core.Tests/DualFrontier.Core.Tests.csproj`.

## TODO
- [ ] Фаза 2 — наполнить `Isolation/` тестами на `[SystemAccess]`.
- [ ] Фаза 2 — `Scheduling/` с графом зависимостей.
- [ ] Фаза 1 — `ECS/` с базовым жизненным циклом entity.
