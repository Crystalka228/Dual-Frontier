# Input — Источник пользовательского ввода

## Назначение
`IInputSource` — контракт для опроса ввода и публикации нормализованных
доменных событий. Application не знает, откуда физически приходят нажатия;
каждая Presentation-сборка реализует свой polling.

## Зависимости
- `DualFrontier.Contracts` — `IGameServices` для публикации событий в шины.

## Что внутри
- `IInputSource.cs` — единственный метод `Poll()`.

## Правила
- Ни `Godot.InputEvent`, ни `Silk.NET.Input.*` не утекают в Application.
- `Poll` не блокирует — читает и отправляет в шины, не ждёт реакции.
- Маппинг "физический ввод → доменное событие" живёт в конкретной реализации.

## TODO
- [ ] Фаза 3.5 — `GodotInputRouter` в `DualFrontier.Presentation`.
- [ ] Фаза 5+ — `NativeInputHandler` в `DualFrontier.Presentation.Native`
      (Silk.NET input polling).

## См. также
- [../../docs/VISUAL_ENGINE.md](../../../docs/VISUAL_ENGINE.md) — общая стратегия
  DevKit vs Native, три контракта (`IRenderer`, `ISceneLoader`, `IInputSource`).
