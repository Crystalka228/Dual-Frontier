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

---
register_id: DOC-F-TESTS
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-04-XX
last_modified: 2026-04-XX
content_language: en
next_review_due: null
title: Tests module index
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
