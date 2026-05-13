---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-TESTS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-TESTS
---
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
