# Визуальный движок — DevKit и Native

Dual Frontier использует два параллельных visual-backend'а. Godot — **DevKit**:
редактор сцен, визуальный тест-стенд, среда быстрой итерации. Native — **Production**:
собственный рантайм на Silk.NET + OpenGL, который поставляется игрокам. Оба бэкенда
реализуют одинаковый контракт и потребляют одинаковый формат сцен.

## Почему два бэкенда

Godot как production — это lock-in. Его сцены `.tscn` привязаны к конкретному
рендеру, его `SceneTree` — к main thread, его UI требует `Control`-нод. Для игры
на 500+ пешек с глубокой магией это потолок. Но **как редактор** Godot незаменим:
TileMap, инспектор, plugin API, scene tree.

Решение: Godot живёт как инструмент разработки. Дизайнер открывает `.tscn`,
расставляет ноды с `DFEntityMeta`, экспортирует в `.dfscene`. Дальше — всё
одинаково для обоих бэкендов.

## Контракты

Три контракта в `DualFrontier.Application`:

- `IRenderer` — главный цикл рендера (`Initialize`, `RenderFrame`, `Shutdown`).
- `ISceneLoader` — парсит `.dfscene`, возвращает `SceneDef`.
- `IInputSource` — источник ввода, публикует события в шины.

Каждая Presentation-сборка реализует все три. Application не импортирует ни
Godot, ни Silk.NET — только использует контракты.

## Формат .dfscene

Человекочитаемый JSON, версионированный:

```json
{
  "version": 1,
  "name": "colony_start",
  "tilemap": { "width": 100, "height": 100, "tileSize": 32, "layers": [...] },
  "entities": [...],
  "markers": [...],
  "metadata": {...}
}
```

Tilemap хранит тайлы per-layer как base64-encoded `ushort[]` длиной `width * height`.
Entity описывает prefab + position + component overrides. Marker — именованная
точка без сущности.

Подробная схема — `src/DualFrontier.Application/Scene/README.md`.

## Godot DevKit plugin

Живёт в `src/DualFrontier.Presentation/addons/df_devkit/`. Флаг `#if TOOLS`
гарантирует что плагин не попадает в production-сборку Godot.

Возможности:

- `DFEntityMeta` Resource — привязывается к нодам, задаёт prefab и overrides.
- `SceneExporter` — обходит SceneTree, сериализует в `.dfscene`.
- Меню "Tools → Export .dfscene" — кнопка экспорта для разработчика.

Полная реализация — Фаза 3.5.

## Native runtime

Сборка `DualFrontier.Presentation.Native`. Зависит только от `Contracts` и
`Application`. Стек технологий:

| Слой               | Библиотека                                |
|--------------------|-------------------------------------------|
| Окно + GL контекст | Silk.NET.Windowing                        |
| Ввод               | Silk.NET.Input                            |
| 2D рендер          | Silk.NET.OpenGL + собственный SpriteBatch |
| UI                 | ImGui.NET                                 |
| Аудио              | OpenAL-CS                                 |

Сейчас это stub'ы с `throw new NotImplementedException`. Реализация — Фаза 5+.

## Правила

- Domain и Application никогда не импортируют `Godot;` и никогда не импортируют
  `Silk.NET;`. Только контракты.
- `[DevKitOnly]` помечает весь Godot-специфичный код, который не должен
  переехать в Native.
- Оба бэкенда должны давать одинаковый результат на одних и тех же `.dfscene`.
  Если поведение расходится — баг в bridge-командах.
- `.dfscene` — единственный формат сцен. `.tscn` только для Godot Editor,
  не читается в рантайме.

## Пайплайн разработчика

```
Godot Editor (автор сцены)
    ↓  Tools → Export .dfscene
assets/scenes/colony.dfscene
    ↓  (одинаковый файл)
    ├─ GodotRenderer + GodotSceneLoader   ← F5 в Godot, быстрая итерация
    └─ NativeRenderer + NativeSceneLoader ← dotnet run, production-проверка
```

CI прогоняет обе реализации на одной фикстуре `sample.dfscene` — поведение
должно быть идентично.

## См. также

- [ARCHITECTURE](./ARCHITECTURE.md) — четыре слоя, правила зависимостей.
- [GODOT_INTEGRATION](./GODOT_INTEGRATION.md) — PresentationBridge, main thread.
- [ROADMAP](./ROADMAP.md) — Фаза 3.5 (DevKit plugin), Фаза 5+ (Native).
