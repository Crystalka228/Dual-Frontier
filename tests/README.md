# tests/

xUnit + FluentAssertions. См. [docs/TESTING_STRATEGY.md](../docs/TESTING_STRATEGY.md).

## Что внутри

- `DualFrontier.Core.Tests/` — юнит-тесты ядра: ECS, планировщик, шина, сторож изоляции.
- `DualFrontier.Systems.Tests/` — тесты игровых систем (Pathfinding, Jobs, Inventory и т.п.).
- `DualFrontier.Modding.Tests/` — тесты загрузчика модов и изоляции `AssemblyLoadContext`.

## Запуск

```bash
dotnet test
```
