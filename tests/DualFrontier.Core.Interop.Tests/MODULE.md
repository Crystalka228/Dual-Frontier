---
register_id: DOC-F-TESTS-CORE-INTEROP
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-05-11
last_modified: 2026-05-11
content_language: en
next_review_due: null
title: Core Interop tests
last_modified_commit: 80c9ba6
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---

# DualFrontier.Core.Interop.Tests — Bridge Test Project

**Purpose**: xUnit-based equivalence tests verifying managed bridge correctly mirrors managed `World` semantics. Validates marshalling, span lifetime invariants, write batch serialization.

**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` §1.11, K2 milestone

**Status**: scaffolding only. Tests added в K2 milestone (~30-40 tests).

**Test categories** (К2 target):
- `NativeWorldTests` — equivalence с managed World (CRUD round-trip)
- `EntityIdPackingTests` — bit-level pack/unpack invariants
- `ComponentTypeRegistryTests` — registration, GetId, idempotency
- `SpanLeaseTests` — acquisition/release lifecycle, mutation rejection
- `WriteCommandBufferTests` — serialization correctness, flush semantics

**Dependencies**:
- xUnit + xunit.runner.visualstudio + Microsoft.NET.Test.Sdk
- Project reference: `DualFrontier.Core.Interop`
- Project reference: `DualFrontier.Core` (для managed World comparison)
- Native dependency: `DualFrontier.Core.Native.dll` (copied к output via post-build target)

**Goal**: 472 + ~30 = ~500 total tests passing post-K2.