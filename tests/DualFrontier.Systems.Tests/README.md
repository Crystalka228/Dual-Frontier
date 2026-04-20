# DualFrontier.Systems.Tests

## Назначение
Тесты игровых систем: Pathfinding, Jobs, Inventory, Combat, Magic и т.п.
Проверяют корректность поведения систем на детерминированных сценариях
(фиксированные seed, фиксированный World state).

## Зависимости
- `DualFrontier.Systems`
- xUnit 2.9+, FluentAssertions 6.12+

## Что внутри
- `.gitkeep` — плейсхолдер. Реальные тесты придут в Фазе 2+.

## Правила
- Никаких тестов с реальным Godot-слоем: системы должны тестироваться
  без Presentation.
- Системы с параллельным выполнением — отдельные тесты на корректность
  в multi-thread сценарии.

## Примеры использования
Запуск: `dotnet test tests/DualFrontier.Systems.Tests/DualFrontier.Systems.Tests.csproj`.

## TODO
- [ ] Фаза 3 — тесты Pathfinding (A*).
- [ ] Фаза 3 — тесты Jobs/Needs.
- [ ] Фаза 4 — тесты Inventory/Craft.
- [ ] Фаза 5 — тесты Combat/Projectile.
