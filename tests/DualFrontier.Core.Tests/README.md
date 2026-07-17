# DualFrontier.Core.Tests

## Purpose
Core unit tests: ECS (`World`, `ComponentStore`), scheduler
(`ParallelSystemScheduler`, `DependencyGraph`), event bus
(`DomainEventBus`), and the isolation guard (`SystemExecutionContext`).

## Dependencies
- `DualFrontier.Core`
- xUnit 2.9+, FluentAssertions 6.12+

## Contents
- `ECS/` — tests for `World`, `ComponentStore`, `EntityId` (versions, destruction, reuse).
- `Scheduling/` — tests for `DependencyGraph`, `ParallelSystemScheduler`.
- `Bus/` — tests for `DomainEventBus` and publish/subscribe.
- `Isolation/` — guard tests (`IsolationViolationException` on illegal access).

## Rules
- One test class — one topic. The file name matches the topic.
- Tests are pure: no static state, no files.
- FluentAssertions style: `actual.Should().Be(expected);`.

## Usage examples
Run: `dotnet test tests/DualFrontier.Core.Tests/DualFrontier.Core.Tests.csproj`.

## TODO
- [ ] Phase 2 — fill `Isolation/` with `[SystemAccess]` tests.
- [ ] Phase 2 — `Scheduling/` with the dependency graph.
- [ ] Phase 1 — `ECS/` with the basic entity lifecycle.

---
register_id: DOC-F-TESTS-CORE
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
title: Core tests
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
