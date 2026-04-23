# Bridge — Мост Domain → Presentation

## Назначение
Однонаправленная очередь команд между доменом (многопоточно)
и Godot-слоем (только главный поток). Домен из любого потока
складывает `IRenderCommand` в `PresentationBridge`; Godot в
своём `_Process` читает очередь и применяет команды к сцене.
См. TechArch 11.9.

## Зависимости
- `DualFrontier.Contracts` — `EntityId` и базовые интерфейсы событий.

## Что внутри
- `PresentationBridge.cs` — `ConcurrentQueue<IRenderCommand>` + API push/drain.
- `IRenderCommand.cs` — базовый интерфейс всех команд рендера.
- `Commands/` — конкретные команды (смерть пешки, снаряд, заклинание, UI).

## Правила
- Никаких `using Godot;` в этой папке. Ссылок на Godot нет — команды
  принимают `object renderContext` — корневой объект активного
  `IRenderer`. В Godot-сборке это `GameRoot`, в Native-сборке это
  `NativeRenderer`.
- Поток домена **только пишет**; главный поток **только читает и выполняет**.
- Команды не должны содержать ссылок на компоненты ECS — только простые
  значения (`EntityId`, координаты, идентификаторы).

## Примеры использования
```csharp
// Domain-сторона (любой поток):
bridge.Enqueue(new PawnDiedCommand(entityId, x, y));

// Presentation-сторона (главный поток активного IRenderer):
bridge.DrainCommands(cmd => cmd.Execute(renderContext));
```

## TODO
- [ ] Фаза 3 — подключить `DrainCommands` к `GameRoot._Process` в Presentation.
- [ ] Фаза 5 — наполнить `Commands/` реальными эффектами.
