# DualFrontier.Presentation

## Назначение
Godot-слой: ноды, UI-контролы, ввод. Единственная сборка, которой
разрешено иметь `using Godot;`. Работает ТОЛЬКО в главном потоке Godot
(ограничение `SceneTree`/`Node` API), читает команды из `PresentationBridge`
и применяет их к сцене. См. TechArch 11.9.

> **TODO — GodotSharp nuget package будет подключён на Фазе 3**, когда мы
> интегрируем настоящий Godot-проект. Сейчас все классы — **стабы без
> `using Godot;`**: они не наследуются от `Node`/`Control` и содержат
> заголовочный комментарий «после подключения GodotSharp унаследовать от
> Godot.Node2D/Control». Фаза 0 создаёт только структуру и контракт.

## Зависимости
- `DualFrontier.Application` — `PresentationBridge`, команды рендера.
- `GodotSharp` (будет добавлен в Фазе 3).

## Что внутри
- `Nodes/` — корневые ноды сцены (`GameRoot`, `PawnVisual`, `TileMapRenderer`, `ProjectileVisual`).
- `UI/` — контролы интерфейса (`PawnInspector`, `ManaBar`, `BuildMenu`, `AlertPanel`).
- `Input/` — маршрутизация ввода (`InputRouter`).
- `Scenes/` — `.tscn` файлы, добавляются через Godot editor позднее.
- `project.godot` — плейсхолдер, будет перезаписан Godot при открытии проекта.

## Правила
- Любой `using Godot;` ДОЛЖЕН находиться ТОЛЬКО в этой сборке.
- Presentation НИКОГДА не вызывает `DualFrontier.Core` или `Systems` напрямую —
  общение только через `PresentationBridge`.
- Весь код Presentation выполняется в главном потоке. Никаких тасков и
  `async void` в ноды.

## Примеры использования
```csharp
// В Godot-слое (Фаза 3+):
public override void _Process(double delta)
{
    _bridge.DrainCommands(cmd => cmd.Execute(this));
}
```

## TODO
- [ ] Фаза 3 — подключить `PackageReference Include="GodotSharp"` в `.csproj`.
- [ ] Фаза 3 — унаследовать ноды от соответствующих базовых Godot-классов.
- [ ] Фаза 3 — создать настоящий Godot-проект и заменить `project.godot`.
