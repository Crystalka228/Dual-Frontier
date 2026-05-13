---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K6_1_AFFECTED_TESTS
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K6_1_AFFECTED_TESTS
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K6_1_AFFECTED_TESTS
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K6_1_AFFECTED_TESTS
---
# K6.1 — Affected test files

Operational artifact recording the tests touched by the K6.1 scheduler-ctor
signature change (`IReadOnlyList<SystemPhase>, TickScheduler, World,
IReadOnlyDictionary<SystemBase, SystemMetadata>, IModFaultSink, IGameServices?`).
Listed by directory; each entry notes the kind of fix applied.

## ParallelSystemScheduler construction sites (24 files)

Each of these constructs `new ParallelSystemScheduler(...)` directly; the
ctor signature change forces an update to every call site.

### tests/DualFrontier.Modding.Tests/

- `Pipeline/ModFaultHandlerTests.cs` — Group B (fault scenario): pipeline + scheduler + real ModFaultHandler. Helper `BuildPipeline` updated to construct a real handler, wire the loader, and pass it through both ctors.
- `Pipeline/PipelineGetActiveModsTests.cs` — Group A (non-fault): scheduler + pipeline. Use `SchedulerTestFixture.BuildIsolated`, pass `new ModFaultHandler()` to pipeline ctor.
- `Pipeline/M73Phase2DebtTests.cs` — Group A.
- `Pipeline/M73Step7Tests.cs` — Group A.
- `Pipeline/M72UnloadChainTests.cs` — Group A.
- `Pipeline/M71PauseResumeTests.cs` — Group A.
- `Pipeline/M62IntegrationTests.cs` — Group A.
- `Pipeline/M52IntegrationTests.cs` — Group A.
- `Pipeline/M51PipelineIntegrationTests.cs` — Group A.
- `Pipeline/ModIntegrationPipelineTests.cs` — Group A.
- `Sharing/ContractTypeInRegularModTests.cs` — Group A.
- `Menu/ModMenuControllerTests.cs` — Group A.

### tests/DualFrontier.Core.Tests/

- `Scheduling/ConverterCycleResolutionTests.cs` — Group A (no pipeline involved): scheduler only.
- `Scheduling/ParallelExecutionTests.cs` — Group A.
- `Bus/DeferredEventDeliveryTests.cs` — Group A.

### tests/DualFrontier.Systems.Tests/

- `Pawn/ComfortAuraSystemTests.cs` — Group A.
- `Pawn/SleepSystemTests.cs` — Group A.
- `Pawn/ConsumeSystemTests.cs` — Group A.
- `Pawn/NeedsAccumulationTests.cs` — Group A.
- `Pawn/NeedsJobIntegrationTests.cs` — Group A.
- `Inventory/CrossSystemMutationIsolationTests.cs` — Group A.
- `Inventory/HaulReservationTests.cs` — Group A.
- `Power/ElectricGridOverloadTests.cs` — Group A.
- `Power/ConverterEfficiencyTests.cs` — Group A.

## ModIntegrationPipeline construction sites (12 files)

All overlap with the scheduler list above (every Modding.Tests file that
constructs the pipeline also constructs the scheduler beforehand). The
pipeline ctor change adds a trailing `ModFaultHandler` parameter; Group A
tests pass `new ModFaultHandler()`, Group B tests share the handler with
the scheduler so the drain path can be exercised end-to-end.

## ModFaultHandler direct construction (0 files)

No test directly constructs `new ModFaultHandler(...)`; existing K6
`ModFaultHandlerTests` accesses the handler via
`pipeline.GetFaultHandlerForTests()`. The ctor change from
`(ModIntegrationPipeline)` to `()` is therefore visible through the
pipeline's ctor surface, not direct test construction.

## Fix categories

- **Group A (non-fault scenarios)**: every test that doesn't observe fault
  routing. Switch scheduler construction to `SchedulerTestFixture.BuildIsolated`
  (defaults to `NullModFaultSink`, empty metadata). Pipeline construction
  passes `new ModFaultHandler()`.
- **Group B (fault scenarios)**: K6 `ModFaultHandlerTests` and the new
  K6.1 `K6_1_FaultRoutingEndToEndTests`. Construct a real `ModFaultHandler`,
  wire it as `faultSink` to the scheduler and as the trailing parameter to
  the pipeline ctor, and `loader.SetFaultHandler(handler)` so
  `ModLoader.HandleModFault` routes to the same instance.
