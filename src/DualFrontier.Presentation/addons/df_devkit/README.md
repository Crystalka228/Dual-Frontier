# DF DevKit — Godot editor plugin

## Назначение
Редактор-плагин для авторинга игровых сцен и экспорта в формат `.dfscene`.
Плагин работает ТОЛЬКО в Godot Editor (флаг `#if TOOLS`), не попадает в
production-сборку.

## Архитектура
- `DfDevKitPlugin.cs` — EditorPlugin entry point, меню "Tools → Export .dfscene".
- `meta/DFEntityMeta.cs` — `[GlobalClass] Resource` с полями `Prefab` и
  `ComponentOverrides`, привязывается к нодам в инспекторе.
- `export/SceneExporter.cs` — обход SceneTree → `SceneDef` → JSON.

## Пайплайн разработчика
1. Открыть сцену в Godot Editor.
2. Разместить Node2D, добавить ему DFEntityMeta (Prefab = "core:pawn").
3. В инспекторе задать overrides компонентов (HealthComponent.max = 150).
4. Tools → Export .dfscene → выбрать путь → плагин пишет файл.
5. Запустить игру через `dotnet run` (или F5 в Godot) — загрузчик подхватит.

## Статус
- [ ] Фаза 3.5 — `DFEntityMeta`: регистрация в редакторе, UI в инспекторе.
- [ ] Фаза 3.5 — `SceneExporter`: обход дерева, TilemapExporter, EntityExporter.
- [ ] Фаза 3.5 — меню "Export .dfscene" в Tools.
- [ ] Фаза 4 — выборочный экспорт (только выделенные ноды).
- [ ] Фаза 4 — live-preview: Godot показывает ECS-состояние в отдельной панели.

## Правила
- Плагин код — всегда под `#if TOOLS`. Никогда не компилируется в production.
- Никакой игровой логики здесь. Только авторинг и экспорт.
- JSON output валидируется против `SceneDef` DTO в `DualFrontier.Application.Scene`.
