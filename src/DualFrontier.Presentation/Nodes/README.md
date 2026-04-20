# Nodes — Godot ноды игровой сцены

## Назначение
Визуальные ноды сцены: корневая (`GameRoot`) и её дочерние
(`PawnVisual`, `TileMapRenderer`, `ProjectileVisual`). Читают команды
из `PresentationBridge` и применяют к сцене.

## Зависимости
- `DualFrontier.Application` — `PresentationBridge`, команды рендера.
- `GodotSharp` (Фаза 3).

## Что внутри
- `GameRoot.cs` — корневая нода сцены (будущий `Node2D`). Тикает `PresentationBridge`.
- `PawnVisual.cs` — визуал отдельной пешки (будущий `Node2D`).
- `TileMapRenderer.cs` — карта тайлов (будущий `TileMap`).
- `ProjectileVisual.cs` — визуал летящего снаряда (будущий `Node2D`).

## Правила
- Фаза 0: классы объявлены как `public sealed class X` **без** наследования
  от `Godot.Node*`, и без `using Godot;`. Фаза 3 поменяет наследование.
- Ноды не ходят в Domain/Application за данными — только читают команды.

## Примеры использования
```csharp
// Фаза 3+ (после подключения GodotSharp):
public override void _Process(double delta)
{
    _bridge.DrainCommands(cmd => cmd.Execute(this));
}
```

## TODO
- [ ] Фаза 3 — унаследовать от соответствующих Godot-типов.
- [ ] Фаза 3 — связать с `.tscn` сценами из `Scenes/`.
