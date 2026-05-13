---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-TESTS-MODDING
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-TESTS-MODDING
---
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
