# assets/scenes — Scene fixtures and test data

Сцены в формате `.dfscene` — engine-neutral JSON, читаемый и Godot-сборкой,
и Native-сборкой через `ISceneLoader`.

## Файлы
- `sample.dfscene` — минимальная валидная сцена для юнит-тестов и smoke-проверки.

## Создание сцены
Предпочтительно — через Godot DevKit plugin (Tools → Export .dfscene).
Авторинг руками возможен для фикстур, но хрупкий: ошибиться в base64
тайлах легко, отладить сложно.

## Версия формата
Текущая версия: `1` (см. `SceneDef.CurrentVersion`).

При повышении версии:
1. Обновить `SceneDef.CurrentVersion`.
2. Обновить все существующие фикстуры ИЛИ добавить миграцию в
   `ISceneLoader` реализациях.
3. Документировать breaking-change в `docs/VISUAL_ENGINE.md`.
