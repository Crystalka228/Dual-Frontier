# Scene — Engine-neutral scene format

## Назначение
DTO-иерархия `SceneDef` и контракт `ISceneLoader` для формата `.dfscene` —
единого описания сцены для обоих рантаймов (Godot DevKit и Native).
Плагин Godot экспортирует `.tscn` → `.dfscene`; оба `ISceneLoader` читают
один и тот же файл.

## Зависимости
- `DualFrontier.Contracts.Math` — `GridVector` для координат.
- `System.Text.Json` — `JsonElement` в overrides компонентов.

## Что внутри
- `SceneDef.cs` — корневой record + `CurrentVersion` константа.
- `TilemapDef.cs` — тайлмап: размеры, слои, `TilemapLayerDef` с `ushort[]`.
- `EntitySpawnDef.cs` — описание сущности: prefab + position + overrides.
- `MarkerDef.cs` — именованная точка на карте без сущности.
- `SceneMetadata.cs` — биом, плотность эфира, автор, время экспорта.
- `ISceneLoader.cs` — контракт загрузчика.
- `SceneLoadException.cs` — ошибки парсинга/версии.

## Формат .dfscene

Человекочитаемый JSON с версионированием. Тайлы хранятся per-layer как
`base64(ushort[width*height])`, row-major. Сущности — prefab + опциональные
overrides полей компонентов в виде `JsonElement`.

```json
{
  "version": 1,
  "name": "colony_start",
  "tilemap": { "width": 100, "height": 100, "tileSize": 32, "layers": [...] },
  "entities": [
    { "id": "pawn_01", "prefab": "core:pawn",
      "position": { "x": 10, "y": 5 },
      "components": { "HealthComponent": { "max": 150 } } }
  ],
  "markers": [...],
  "metadata": { "biome": "temperate", "etherDensity": 0.4, ... }
}
```

Политика версий: при любом breaking-change инкрементируется
`SceneDef.CurrentVersion`, старые файлы требуют миграции в
реализациях `ISceneLoader`.

## Правила
- `SceneDef` не содержит Godot-типов (нет `Vector2`, `NodePath`, `Node`).
- Только примитивы, строки, `GridVector` и `JsonElement` для opaque overrides.
- `ushort[]` — компактный массив тайлов; в JSON сериализуется как base64.

## См. также
- [../../docs/VISUAL_ENGINE.md](../../../docs/VISUAL_ENGINE.md) — полная
  архитектура DevKit vs Native.
- [../Rendering/README.md](../Rendering/README.md) — контракт `IRenderer`.
