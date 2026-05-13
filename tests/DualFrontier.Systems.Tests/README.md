---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-TESTS-SYSTEMS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-TESTS-SYSTEMS
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-TESTS-SYSTEMS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-TESTS-SYSTEMS
---
# DualFrontier.Systems.Tests

## Purpose
Tests for game systems: Pathfinding, Jobs, Inventory, Combat, Magic, etc.
They verify system behavior on deterministic scenarios (fixed seed, fixed
World state).

## Dependencies
- `DualFrontier.Systems`
- xUnit 2.9+, FluentAssertions 6.12+

## Contents
- `.gitkeep` — placeholder. Real tests will arrive in Phase 2+.

## Rules
- No tests with the real Godot layer: systems must be tested without
  Presentation.
- Systems with parallel execution have separate tests for correctness in a
  multi-thread scenario.

## Usage examples
Run: `dotnet test tests/DualFrontier.Systems.Tests/DualFrontier.Systems.Tests.csproj`.

## TODO
- [ ] Phase 3 — Pathfinding (A*) tests.
- [ ] Phase 3 — Jobs/Needs tests.
- [ ] Phase 4 — Inventory/Craft tests.
- [ ] Phase 5 — Combat/Projectile tests.
