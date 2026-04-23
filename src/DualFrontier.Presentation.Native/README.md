# DualFrontier.Presentation.Native — Production Runtime Stub

## Назначение
Production-рантайм на Silk.NET + OpenGL. Реализует контракты
`IRenderer`, `ISceneLoader`, `IInputSource` из Application без
единой ссылки на Godot. Это та сборка, которая поставляется игрокам.

Сейчас — только stub'ы с правильной структурой классов и XML-документацией.
Реальная реализация начинается в Фазе 5.

## Зависимости
- `DualFrontier.Contracts`
- `DualFrontier.Application`

Ни `Core`, ни `Systems`, ни `Godot`.

## Что внутри
- `NativeRenderer.cs` — stub реализации `IRenderer`.
- `NativeSceneLoader.cs` — stub `ISceneLoader` (System.IO + System.Text.Json).
- `NativeInputHandler.cs` — stub `IInputSource`.

## Правила
- Native **никогда** не референсит `IDevKitRenderer` или любой другой тип,
  помеченный `[DevKitOnly]`. CI-проверка (grep сейчас, Roslyn analyser позже)
  падает, если такой reference появляется.
- Ни одной строки `using Godot;` в этой сборке — CI проверяет grep.
- Ни одного `DualFrontier.Presentation` reference — это peer-сборка, не дочка.
- Все типы помечаются `[DevKitOnly]` только если они **НЕ** переживут переход
  в production. Здесь таких быть не должно.

## TODO
- [ ] Фаза 5 — добавить `PackageReference` на Silk.NET.
- [ ] Фаза 5 — реализовать `Initialize`/`RenderFrame`/`Shutdown` в NativeRenderer.
- [ ] Фаза 5 — реализовать JSON-парсинг в NativeSceneLoader.
- [ ] Фаза 5 — реализовать Silk.NET input polling в NativeInputHandler.
- [ ] Фаза 6 — добавить SpriteBatch + TilemapRenderer + ShaderProgram.
- [ ] Фаза 6 — добавить ImGui.NET для HUD.
