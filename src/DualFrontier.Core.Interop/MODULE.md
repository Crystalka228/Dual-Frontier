# DualFrontier.Core.Interop — Module Documentation

**Purpose**: P/Invoke bridge layer между managed Application и native `DualFrontier.Core.Native.dll`. Translates managed API calls к C ABI invocations с appropriate marshalling.

**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` §1.2

**Public API surface** (post-K8):
- `NativeWorld` — managed handle wrapper (`IDisposable`)
- `WriteCommandBuffer` — mod mutation accumulator
- `SpanLease<T>` — span lifetime guard (`IDisposable`)
- `ComponentTypeRegistry` — explicit type-id registration

**Dependencies**:
- BCL only (`System.Runtime.InteropServices`, `System.Numerics`)
- Project reference: `DualFrontier.Contracts`
- Native dependency: `DualFrontier.Core.Native.dll` (loaded at runtime)

**Layering**: bridge layer — Domain calls Interop, Interop calls Native. Domain не calls Native directly.

**TODO list**:
- K0: cherry-pick existing project from experimental branch
- K1: bulk operations + Span<T> primitives
- K2: ComponentTypeRegistry replaces FNV-1a hash
- K5: WriteCommandBuffer + SpanLease<T> production version

**Status**: scaffolding only. Implementation lives in cherry-picked branch contents (K0) и subsequent K-series milestones.
