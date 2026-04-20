# DualFrontier.Modding.Tests

## Назначение
Тесты загрузчика модов (`ModLoader`) и изоляции `AssemblyLoadContext`:
что мод физически не видит внутренностей ядра, что `Unload` выгружает
сборку, что попытка скастить `IModApi` ловится и приводит к выгрузке.

## Зависимости
- `DualFrontier.Application`
- xUnit 2.9+, FluentAssertions 6.12+

## Что внутри
- `.gitkeep` — плейсхолдер. Реальные тесты придут в Фазе 2.

## Правила
- Тесты изоляции используют специально собранные тестовые mod-сборки
  (в `Fixtures/`). Фикстуры собираются отдельно от основных тестов.
- Каждый тест горячей перезагрузки убирает за собой: проверяет, что
  `AssemblyLoadContext` реально освобождён.

## Примеры использования
Запуск: `dotnet test tests/DualFrontier.Modding.Tests/DualFrontier.Modding.Tests.csproj`.

## TODO
- [ ] Фаза 2 — тест: мод не может загрузить `DualFrontier.Core`.
- [ ] Фаза 2 — тест: cast `IModApi` к `RestrictedModApi` ловится.
- [ ] Фаза 3 — тест горячей перезагрузки мода.
