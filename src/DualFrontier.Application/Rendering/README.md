# Rendering — Контракт рендер-бэкенда

## Назначение
`IRenderer` — единая точка входа для любого visual-backend'а. Application
держит только контракт; конкретные реализации живут в Presentation-сборках:
Godot DevKit и Native (Silk.NET). В рантайме активен ровно один.

## Зависимости
- `DualFrontier.Contracts` — базовые типы (через `PresentationBridge`).

## Что внутри
- `IRenderer.cs` — `Initialize` / `RenderFrame` / `Shutdown` / `IsRunning`.

## Правила
- Никаких `using Godot;` или `using Silk.NET;` — только абстрактный контракт.
- Реализация обязана дренировать `PresentationBridge` в своём `RenderFrame`.
- Все GPU-ресурсы освобождаются в `Shutdown` — утечки недопустимы.

## TODO
- [ ] Фаза 3.5 — `GodotRenderer` в `DualFrontier.Presentation`.
- [ ] Фаза 5+ — `NativeRenderer` в `DualFrontier.Presentation.Native`
      (Silk.NET + OpenGL, SpriteBatch, TilemapRenderer).

## См. также
- [../../docs/VISUAL_ENGINE.md](../../../docs/VISUAL_ENGINE.md) — общая стратегия
  DevKit vs Native, формат `.dfscene`, пайплайн разработчика.
