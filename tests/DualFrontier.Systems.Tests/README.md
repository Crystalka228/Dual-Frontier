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

---
register_id: DOC-F-TESTS-SYSTEMS
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
title: Systems tests
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
