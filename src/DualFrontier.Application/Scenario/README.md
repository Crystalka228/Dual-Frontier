# Scenario — Загрузка сценариев

## Назначение
Стартовые сценарии описывают начальное состояние мира: биом, зерно генерации,
количество и параметры начальных пешек, стартовый инвентарь и т.п.
`ScenarioLoader` парсит файлы сценариев с диска, `ScenarioDef` — DTO-описание.

## Зависимости
- `DualFrontier.Core` — для создания `World` на основе дефиниции.

## Что внутри
- `ScenarioLoader.cs` — парсер файлов сценариев (JSON/TOML, TBD).
- `ScenarioDef.cs` — неизменяемая дефиниция сценария.

## Правила
- `ScenarioDef` — чистый DTO, без логики.
- Парсинг синхронный; асинхронный прогресс — ответственность вышестоящего слоя.
- Сценарий НЕ должен ссылаться на конкретные мод-типы по имени C# — только по
  зарегистрированному идентификатору.

## Примеры использования
```csharp
var loader   = new ScenarioLoader();
ScenarioDef scenario = loader.Load("scenarios/default.json");
// Дальше — создать World по scenario.
```

## TODO
- [x] Фаза 3 — `ScenarioLoader.Load(path)` парсит JSON через
      `System.Text.Json`.
- [x] Фаза 3 — `ScenarioDef` с полями `Id`, `Name`, `StartingPawnCount`,
      `MapWidth`, `MapHeight`, `WorldSeed`, `StartingItems`.
- [ ] Валидация схемы и понятные ошибки парсинга.
- [ ] `LoadDefault()` используется при отсутствии файла сценария
      (сейчас метод есть, но вызывающий код не делает fallback).
