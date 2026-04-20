# DualFrontier.Mod.Example

## Назначение
Референсный пример мода: минимальная сборка, демонстрирующая правильный
паттерн интеграции. Показывает, как работает `IMod` + `IModApi` и как
описывается `mod.manifest.json`. Используется как живой образец в
документации и как основа для шаблона новых модов.

## Зависимости
- `DualFrontier.Contracts` (и только она).

## Что внутри
- `DualFrontier.Mod.Example.csproj` — сборка; единственный `ProjectReference`
  — на `DualFrontier.Contracts`.
- `ExampleMod.cs` — реализация `IMod`: `Initialize(IModApi)` / `Unload()`.
- `mod.manifest.json` — манифест мода (id, version, entry-assembly, entry-type).

## Правила
- Никаких зависимостей на `Core`, `Systems`, `Components`, `Events`, `AI`,
  `Application`. Только `Contracts`. Это — правило изоляции модов (TechArch 11.8).
- Нельзя кастить `IModApi` к конкретному типу — `ModLoader` обнаружит
  нарушение и выгрузит мод.
- Блокирующих операций в `Initialize` быть не должно — загрузка других
  модов ждёт.

## Примеры использования
```csharp
public sealed class ExampleMod : IMod
{
    public void Initialize(IModApi api) { /* регистрация */ }
    public void Unload() { /* отписка */ }
}
```

Сборка мода попадает в `mods/DualFrontier.Mod.Example/bin/.../net8.0/`;
рядом кладётся `mod.manifest.json`.

## TODO
- [ ] Фаза 2 — добавить пример регистрации компонента и подписки на событие.
- [ ] Фаза 2 — пример `PublishContract<T>` / `TryGetContract<T>`.
