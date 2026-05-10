# tests/

xUnit + FluentAssertions. See [docs/methodology/TESTING_STRATEGY.md](/docs/methodology/TESTING_STRATEGY.md).

## Contents

- `DualFrontier.Core.Tests/` — core unit tests: ECS, scheduler, bus, isolation guard.
- `DualFrontier.Systems.Tests/` — game-system tests (Pathfinding, Jobs, Inventory, etc.).
- `DualFrontier.Modding.Tests/` — mod loader and `AssemblyLoadContext` isolation tests.

## Running

```bash
dotnet test
```
