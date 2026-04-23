# Commands — Команды рендера

## Назначение
Конкретные реализации `IRenderCommand`. Каждый файл — одна команда:
данные о событии из домена, плюс метод `Execute`, который в Фазе 5
применит эффект к сцене Godot.

## Зависимости
- `DualFrontier.Contracts.Core` — `EntityId` и базовые типы.
- `DualFrontier.Application.Bridge` — `IRenderCommand`.

## Что внутри
- `PawnDiedCommand.cs` — смерть пешки (эффект, звук, обновление UI).
- `ProjectileSpawnedCommand.cs` — появление снаряда (визуал траектории).
- `SpellCastCommand.cs` — каст заклинания (VFX школы магии).
- `UIUpdateCommand.cs` — обновление UI-элемента (счётчик/плашка).

## Правила
- Команды — **immutable** `record`-типы с простыми полями
  (`EntityId`, `int`, `float`, `string`). Никаких ссылок на `IComponent`
  или системы.
- `Execute` работает через `object renderContext`; конкретный cast делает
  вызывающий из активной Presentation-сборки (Godot → `GameRoot`, Native → `NativeRenderer`).

## Примеры использования
```csharp
// Domain публикует команду после обработки события.
bridge.Enqueue(new PawnDiedCommand(pawnId, x: 42, y: 17));
```

## TODO
- [ ] Фаза 5 — наполнить `Execute` реальной Godot-логикой через
      Presentation-хелперы.
- [ ] Фаза 5 — добавить команды под остальные доменные события.
