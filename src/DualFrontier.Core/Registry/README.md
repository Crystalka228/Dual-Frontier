# Registry — Реестры типов

## Назначение
Реестры типов компонентов и систем. Нужны для:
1. Присвоения стабильных числовых ID компонентам (для Save/Load и сетевого
   протокола модов).
2. Динамической регистрации компонентов/систем модами через `IModApi`.
3. Итерации по всем зарегистрированным системам планировщиком при построении графа.

## Зависимости
- `DualFrontier.Contracts.Core` (`IComponent`).
- `DualFrontier.Core.ECS` (`SystemBase`).

## Что внутри
- `ComponentRegistry.cs` — маппинг `Type ↔ int ComponentTypeId`.
- `SystemRegistry.cs` — хранилище зарегистрированных систем, итератор для планировщика.

## Правила
- Регистрация — операция старта игры или загрузки мода. В рантайме не вызывается.
- ID компонента стабилен между запусками (важно для Save/Load). Для модов —
  ID выдаются после базовых типов и зависят от порядка загрузки модов.
- SystemRegistry не гарантирует стабильность порядка итерации.

## Примеры использования
```csharp
var components = new ComponentRegistry();
components.Register<HealthComponent>();
int id = components.GetTypeId(typeof(HealthComponent));

var systems = new SystemRegistry();
systems.Register(new CombatSystem());
foreach (var system in systems.GetAll()) { /* build graph */ }
```

## TODO
- [ ] Фаза 1 — реализовать ComponentRegistry со стабильными ID.
- [ ] Фаза 1 — реализовать SystemRegistry.
- [ ] Фаза 2 — добавить сериализацию соответствия Type ↔ ID в save-файл.
