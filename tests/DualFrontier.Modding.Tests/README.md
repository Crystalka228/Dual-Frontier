# DualFrontier.Modding.Tests

## Purpose
Tests for the mod loader (`ModLoader`) and `AssemblyLoadContext` isolation:
that a mod physically cannot see the core's internals, that `Unload` releases
the assembly, and that an attempt to cast `IModApi` is caught and triggers
unload.

## Dependencies
- `DualFrontier.Application`
- xUnit 2.9+, FluentAssertions 6.12+

## Contents
- `.gitkeep` — placeholder. Real tests will arrive in Phase 2.

## Rules
- Isolation tests use specially built test mod assemblies (in `Fixtures/`).
  Fixtures are built separately from the main tests.
- Each hot-reload test cleans up after itself: it confirms that the
  `AssemblyLoadContext` was actually released.

## Usage examples
Run: `dotnet test tests/DualFrontier.Modding.Tests/DualFrontier.Modding.Tests.csproj`.

## TODO
- [ ] Phase 2 — test: a mod cannot load `DualFrontier.Core`.
- [ ] Phase 2 — test: casting `IModApi` to `RestrictedModApi` is caught.
- [ ] Phase 3 — mod hot-reload test.

---
register_id: DOC-F-TESTS-MODDING
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
title: Modding tests
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
