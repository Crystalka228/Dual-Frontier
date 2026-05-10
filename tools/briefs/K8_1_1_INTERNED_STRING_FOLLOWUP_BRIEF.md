# K8.1.1 — InternedString closure follow-up: cross-pool equality, doc semantics, empty-string coverage

**Brief version**: 1.0 (full, executable)
**Authored**: 2026-05-09 (post-K8.1 closure, pre-K8.2 authoring)
**Status**: EXECUTED (2026-05-09, branch `feat/k8-1-1-interned-string-followup`, closure `f8273ca`..`16afdf3`). See `docs/MIGRATION_PROGRESS.md` § "K8.1.1 — InternedString closure follow-up" for closure record.
**Reference docs**: `docs/architecture/KERNEL_ARCHITECTURE.md` v1.2 LOCKED (K-L11), `docs/MIGRATION_PROGRESS.md` (K8.1 closure record), `tools/briefs/K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md` §1.2 (LOCKED #5 InternedString equality semantics — current authority for this brief)
**Companion**: `docs/MIGRATION_PROGRESS.md` (K8.1.1 row authored as part of this brief execution)
**Methodology lineage**: `tools/briefs/K6_1_FAULT_WIRING_BRIEF.md` (focused follow-up brief precedent), `tools/briefs/K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md` (K8.1 baseline this brief amends)
**Predecessor**: K8.1 (`a62c1f3..059f712`) — native-side reference handling primitives EXECUTED
**Target**: fresh feature branch `feat/k8-1-1-interned-string-followup` from `main` after K8.1 closure
**Estimated time**: 1-3 hours auto-mode (small focused scope)
**Estimated LOC delta**: ~+200/-10

---

## Goal

Close three observations surfaced by Opus closure verification of K8.1 (post-execution review, 2026-05-09):

1. **Cross-pool equality** — K8.1 LOCKED #5 (per `K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md` §1.2) specified dual semantics: same-pool comparison via id (fast path), cross-pool comparison via content (correctness path). The K8.1 implementation realised the same-pool path correctly via `(Id, Generation)` equality, but did **not** expose an explicit cross-pool comparison method. Solution A (single NativeWorld backbone) makes cross-pool comparison rare in production, but the lack of an explicit API surface for it leaves callers without a future-proof contract: they cannot tell from the type whether `==` is safe across pools, and may silently get false-positive id collisions in any future scenario where two pools coexist (test fixtures, research, multi-world experiments). The fix codifies the cross-pool path as an explicit method.

2. **Cross-pool semantics doc-comment** — `InternedString` doc-comment as written communicates the same-pool fast path but not the cross-pool path or the multi-world failure mode. A reader inheriting the type for K8.2 component conversion may infer that `==` is safe regardless of issuing world, which is incorrect outside Solution A.

3. **Empty-string sentinel coverage** — the `intern("") → 0` / `Resolve(empty) → null` sentinel path is implemented in two places (native `string_pool.cpp::intern` and managed `InternedString.Resolve`), but no native selftest scenario or managed bridge test exercises it. A regression here would surface late, possibly via mod-author bug report rather than CI gate.

K8.1.1 is a **closure follow-up brief** — it does not redesign K8.1, does not migrate any components (that's K8.2), and does not add new primitives. It tightens the API surface and test coverage of one existing K8.1 deliverable: `InternedString`.

---

## Phase 0 — Pre-flight verification

### 0.1 — Working tree clean

```
git status
```

**Expected**: `nothing to commit, working tree clean` on branch `main`.

**Halt condition**: any uncommitted modifications. Resolution: stash via `git stash push -m "pre-K8-1-1-WIP"` and re-verify.

### 0.2 — Prerequisite milestone closed

```
git log --oneline -25
```

**Expected**: K8.1 closure commits visible. Most recent K8.1 commit per closure record: `059f712`. K8.1 row in MIGRATION_PROGRESS.md status DONE.

**Halt condition**: K8.1 not closed, or closure SHA does not match. K8.1.1 is a follow-up amendment to K8.1; the predecessor must be on disk and closed before amendments can land cleanly.

### 0.3 — Prerequisite documents at expected versions

```
head -10 docs/architecture/KERNEL_ARCHITECTURE.md
head -10 docs/MIGRATION_PROGRESS.md
```

**Expected**:
- `KERNEL_ARCHITECTURE.md` Status: AUTHORITATIVE LOCKED v1.2 (K-L11 for Solution A in force)
- `MIGRATION_PROGRESS.md` Last updated: 2026-05-09 (K8.1 closure)

**Halt condition**: any spec at unexpected version. K8.1.1 builds on K-L11 verbatim and on the K8.1 closure as recorded.

### 0.4 — Code state inventory

```
ls src/DualFrontier.Core.Interop/
ls tests/DualFrontier.Core.Interop.Tests/
ls native/DualFrontier.Core.Native/test/
```

**Expected** present (delivered by K8.1):
- `src/DualFrontier.Core.Interop/InternedString.cs`
- `src/DualFrontier.Core.Interop/NativeWorld.cs` (with K8.1 reference primitives section appended)
- `src/DualFrontier.Core.Interop/NativeMethods.cs` (with K8.1 P/Invoke entries)
- `tests/DualFrontier.Core.Interop.Tests/InternedStringTests.cs`
- `native/DualFrontier.Core.Native/test/selftest.cpp` (with `scenario_string_pool` registered)

**Halt condition**: any expected file missing. K8.1.1 amends these files; if any is absent, K8.1 closure was not actually executed and K8.1.1 cannot proceed.

### 0.5 — Native build clean

```
cd native/DualFrontier.Core.Native
cmake --build build --config Release
build/Release/df_native_selftest.exe
```

**Expected**: build succeeds without warnings or errors. Selftest passes including `scenario_string_pool`.

**Halt condition**: native build failure or selftest failure on baseline. K8.1.1 starts from a known-good native state.

### 0.6 — Managed build + test baseline

```
cd ../..
dotnet build
dotnet test
```

**Expected**: build clean, **583 tests passing** (post-K8.1 baseline).

**Halt condition**: any regression. K8.1.1 must not regress baseline; any regression indicates either an environment issue or an upstream regression that should be addressed before K8.1.1 work.

### 0.7 — Brief itself committed

If this brief is on disk in the working tree but uncommitted at the start of execution, commit it before any other work:

```
git add tools/briefs/K8_1_1_INTERNED_STRING_FOLLOWUP_BRIEF.md
git commit -m "docs(briefs): author K8.1.1 InternedString follow-up brief"
```

This keeps the brief's authoring date and content auditable independently of execution commits.

---

## Phase 1 — Architectural design (LOCKED — read-only, no edits)

This phase is the architectural foundation for K8.1.1. The executor reads this section as the design contract; decisions here are LOCKED by Crystalka's stated commitment («сложная архитектура без костылей, проект на десятилетия»), the K8.0 architectural commitment (K-L11 Solution A), and the K8.1 LOCKED #5 dual-semantics specification.

### 1.1 — `EqualsByContent` method signature (LOCKED)

The cross-pool content equality path is exposed as an instance method on `InternedString`:

```csharp
public bool EqualsByContent(InternedString other, NativeWorld thisWorld, NativeWorld otherWorld)
```

**Two-world signature is mandatory** (LOCKED). Single-world signatures (`EqualsByContent(other, world)` with implicit assumption that both ids belong to the same world) reproduce the silent-collision risk inside the API surface — a caller misuse passing two ids issued by different worlds gets a false answer, exactly the failure mode the brief is meant to prevent. Two-world signature makes the cross-pool nature explicit at every call site; misuse becomes structurally impossible.

**Semantics**:

- If both `this.IsEmpty` and `other.IsEmpty` → return `true` (empty equals empty regardless of world).
- If exactly one of `this.IsEmpty` / `other.IsEmpty` → return `false`.
- If `thisWorld` is `null` or `otherWorld` is `null` → throw `ArgumentNullException` (callers cannot omit the world; cross-pool semantics requires both).
- Otherwise, resolve `this` against `thisWorld` and `other` against `otherWorld`, then compare resolved strings via `string.Equals` (ordinal). If either resolution returns `null` (stale generation, wrong world) → return `false`.

**Fast-path optimisation** (LOCKED):

When `ReferenceEquals(thisWorld, otherWorld)` and the `(Id, Generation)` pairs are equal, the method can short-circuit to `true` without resolving. This preserves the same-pool fast path behaviour at the cross-pool API surface: callers who happen to pass the same world twice pay no resolution cost when ids match.

When `ReferenceEquals(thisWorld, otherWorld)` and `(Id, Generation)` pairs differ, the method must still resolve and compare content — it is permitted (though unusual) for two ids in the same pool to refer to identical content if a reclaim-and-reuse cycle reissued an id. The content-equality path remains the source of truth.

**No fallback to `==`**: callers must explicitly choose between `==` (same-pool fast path, by id+gen) and `EqualsByContent` (cross-pool correctness path, by resolved content). The choice is part of the API contract.

### 1.2 — Doc-comment expansion (LOCKED)

The `InternedString` XML doc-comment is expanded to communicate three points the current text omits:

1. **Multi-world failure mode of `==`**: `==` (and `Equals(InternedString)`) compares only `(Id, Generation)`. If two `InternedString` values come from different `NativeWorld` pools, `==` may return `true` for unrelated content (false positive) or `false` for identical content (false negative). Use `EqualsByContent(other, thisWorld, otherWorld)` for cross-pool comparison.

2. **Solution A applicability** (per K-L11): in Solution A production, only one `NativeWorld` exists, and `==` is sufficient. Multi-world scenarios are limited to test fixtures, research, and any future relaxation of K-L11. The cross-pool method exists for those scenarios and for future-proofing the API.

3. **Save/load contract restated**: components serialise resolved string content, not `(Id, Generation)`. On load, content is re-interned and a fresh pair is issued. `EqualsByContent` is the path for any post-load reconciliation that needs to compare content across snapshot boundaries.

The doc-comment continues to describe the same-pool fast path as the common runtime case; the additions clarify when that path is unsafe.

### 1.3 — Empty-string sentinel coverage (LOCKED)

The `intern("") → 0` sentinel path is exercised by:

- **One native selftest sub-scenario** added to `scenario_string_pool` in `selftest.cpp`. Tests: `df_world_intern_string` with empty content returns `0`; `df_world_resolve_string` with id `0` and any generation returns `0` (no bytes written, treated as not-found); `df_world_string_pool_count` is unchanged after intern of empty content.

- **One managed bridge test** added to `InternedStringTests.cs`. Tests: `NativeWorld.InternString("")` returns an `InternedString` with `IsEmpty == true` (`Id == 0`, `Generation == 0`); `interned.Resolve(world)` returns `null`; the empty result is value-equal to a default-constructed `InternedString` (`new InternedString() == default`); `EqualsByContent` returns `true` when both sides are empty regardless of which worlds are passed (including different worlds).

These tests are not exhaustive — they cover the round-trip, the `Resolve` contract, and the value-equality of empty values. Edge cases like "string of length 0 distinct from null reference" are already covered by the `string content` parameter of `InternString` and tested in K8.1 baseline.

### 1.4 — Scope limits (LOCKED)

K8.1.1 does **not**:

- Modify the native `StringPool` implementation (the empty-string path already works correctly; only test coverage is added).
- Modify the C ABI surface (`df_capi.h`/`capi.cpp`). All K8.1.1 changes are managed-side; the cross-pool comparison method calls existing ABI entries (`df_world_resolve_string`).
- Touch other K8.1 primitives (`KeyedMap`, `Composite`, `SetPrimitive`). They have their own contracts; the cross-pool concern is specific to `InternedString`.
- Add new ABI entries. `EqualsByContent` is implemented entirely on the managed side using existing `df_world_resolve_string`.
- Migrate any production code to use `EqualsByContent`. The method is added as a future-proofed API; K8.2 components migrating from `string` to `InternedString` will use it where cross-pool semantics applies. K8.1.1 only adds the surface, not the consumers.

### 1.5 — Atomic commit shape (LOCKED)

K8.1.1 lands as **3-5 atomic commits**:

1. `feat(interop): add InternedString.EqualsByContent for cross-pool comparison` — adds the method, no tests yet.
2. `test(interop): cover InternedString.EqualsByContent across same-pool and cross-pool scenarios` — adds bridge tests for the new method.
3. `docs(interop): expand InternedString doc-comment with multi-world semantics` — doc-comment expansion only.
4. `test(native): cover string pool empty-string sentinel in selftest` — adds the selftest sub-scenario for empty intern/resolve.
5. `test(interop): cover InternedString empty-string sentinel round-trip` — adds the bridge test for empty input.

Commits 4 and 5 may be bundled into one commit if the executor judges them logically inseparable, in line with the K8.1 lesson on atomic-commit-as-compilable-unit; document the choice in the closure section if so.

---

## Phase 2 — `EqualsByContent` implementation

**File**: `src/DualFrontier.Core.Interop/InternedString.cs`

Append the method to the `InternedString` struct, after the existing `Resolve` method and before the existing `Equals(InternedString)` method:

```csharp
/// <summary>
/// Compares this <see cref="InternedString"/> with <paramref name="other"/>
/// by resolved content rather than by <c>(Id, Generation)</c>. Use this for
/// cross-pool comparisons where the two values were issued by different
/// <see cref="NativeWorld"/> instances; <c>==</c> and
/// <see cref="Equals(InternedString)"/> compare only the id pair and may
/// return false positives or false negatives across pools.
///
/// Empty values compare equal to each other regardless of world. If both
/// sides resolve successfully, content is compared via ordinal string
/// equality. If either resolution returns <c>null</c> (stale generation,
/// wrong world), the method returns <c>false</c>.
///
/// Fast path: when <paramref name="thisWorld"/> and
/// <paramref name="otherWorld"/> are the same instance and the
/// <c>(Id, Generation)</c> pairs are equal, returns <c>true</c> without
/// resolving. Same-pool callers pay no resolution cost on equal ids.
/// </summary>
/// <exception cref="ArgumentNullException">
/// Thrown when either world is <c>null</c>. Cross-pool semantics requires
/// both worlds to be supplied — the method cannot infer issuer.
/// </exception>
public bool EqualsByContent(InternedString other, NativeWorld thisWorld, NativeWorld otherWorld)
{
    if (thisWorld is null) throw new ArgumentNullException(nameof(thisWorld));
    if (otherWorld is null) throw new ArgumentNullException(nameof(otherWorld));

    // Empty values are equal to each other irrespective of world.
    if (IsEmpty && other.IsEmpty) return true;
    if (IsEmpty || other.IsEmpty) return false;

    // Same-pool fast path: identical (Id, Generation) on the same world.
    if (ReferenceEquals(thisWorld, otherWorld) && Equals(other))
    {
        return true;
    }

    string? thisContent = Resolve(thisWorld);
    string? otherContent = other.Resolve(otherWorld);
    if (thisContent is null || otherContent is null) return false;
    return string.Equals(thisContent, otherContent, StringComparison.Ordinal);
}
```

**Atomic commit**: `feat(interop): add InternedString.EqualsByContent for cross-pool comparison`

---

## Phase 3 — Doc-comment expansion

**File**: `src/DualFrontier.Core.Interop/InternedString.cs`

Replace the existing struct-level XML doc-comment with the expanded version below. The struct body stays unchanged; only the comment block is rewritten.

Existing comment to remove:

```csharp
/// <summary>
/// Managed handle over a string interned in <c>dualfrontier::StringPool</c>.
///
/// Equality is by (Id, Generation) — both halves must match for two
/// <see cref="InternedString"/> values to compare equal. This is the
/// fast path: same-pool comparisons are a single 64-bit equality test.
///
/// To recover the underlying content, call <see cref="Resolve"/> with
/// the <see cref="NativeWorld"/> the id was issued by. Resolution may
/// return <c>null</c> if the generation tag is stale (the id was
/// reclaimed during a <see cref="NativeWorld.ClearModScope(string)"/>
/// and possibly reissued for different content). Callers that hold
/// stale IDs should re-intern from the original content rather than
/// guess at recovery.
///
/// Save/load (LOCKED, per K8.1 brief §1.2): components serialise the
/// resolved string, not the (Id, Generation) pair. On reload the
/// content is re-interned and a fresh pair is issued. The generation
/// tag is the safety net for any path that did persist an id (in-memory
/// snapshots, diagnostic dumps).
/// </summary>
```

Replacement:

```csharp
/// <summary>
/// Managed handle over a string interned in <c>dualfrontier::StringPool</c>.
///
/// <para>
/// <b>Same-pool equality (fast path).</b> <c>==</c>,
/// <see cref="Equals(InternedString)"/>, and <see cref="GetHashCode"/>
/// compare by <c>(Id, Generation)</c>. Within a single
/// <see cref="NativeWorld"/>, equal pairs always refer to identical content
/// and unequal pairs to distinct content; comparison is a single 64-bit
/// equality test.
/// </para>
///
/// <para>
/// <b>Cross-pool equality.</b> If two <see cref="InternedString"/> values
/// were issued by <i>different</i> <see cref="NativeWorld"/> instances,
/// <c>==</c> may return <c>true</c> for unrelated content (false positive,
/// when the two pools happen to issue the same id for different strings)
/// or <c>false</c> for identical content (false negative, when the same
/// content received different ids in each pool). Use
/// <see cref="EqualsByContent"/> for cross-pool comparisons; it resolves
/// both sides and compares string content.
/// </para>
///
/// <para>
/// <b>Solution A applicability.</b> Per K-L11 (production storage backbone),
/// production code uses one <see cref="NativeWorld"/> and the same-pool
/// fast path is sufficient. Multi-world scenarios are limited to test
/// fixtures, research code, and any future relaxation of K-L11; the
/// cross-pool method exists for those scenarios and for future-proofing
/// the API surface.
/// </para>
///
/// <para>
/// <b>Stale ids.</b> Resolution may return <c>null</c> if the generation
/// tag is stale (the id was reclaimed during a
/// <see cref="NativeWorld.ClearModScope(string)"/> and possibly reissued
/// for different content) or if the id was issued by a world other than
/// the one supplied to <see cref="Resolve"/>. Callers that hold stale ids
/// should re-intern from the original content rather than guess at
/// recovery.
/// </para>
///
/// <para>
/// <b>Save/load (LOCKED, per K8.1 brief §1.2).</b> Components serialise
/// resolved string content, not the <c>(Id, Generation)</c> pair. On
/// reload the content is re-interned and a fresh pair is issued. The
/// generation tag is the safety net for any path that did persist an id
/// (in-memory snapshots, diagnostic dumps); it is not the primary
/// persistence mechanism. Cross-snapshot reconciliation that needs to
/// compare interned values uses <see cref="EqualsByContent"/>, since
/// snapshots may have been written by different worlds.
/// </para>
/// </summary>
```

**Atomic commit**: `docs(interop): expand InternedString doc-comment with multi-world semantics`

---

## Phase 4 — Native selftest empty-string coverage

**File**: `native/DualFrontier.Core.Native/test/selftest.cpp`

Locate `scenario_string_pool`. After sub-scenario 5 (the existing "Mod scope clear bumps current_generation" assertions) and before `df_world_destroy(w);`, append a new sub-scenario:

```cpp
    // 6. Empty-string sentinel: intern returns 0, resolve returns 0 bytes,
    //    pool count unchanged. The sentinel path is hand-written in
    //    string_pool.cpp::intern; this scenario guards against accidental
    //    refactors of that branch.
    const int32_t count_before_empty = df_world_string_pool_count(w);
    uint32_t id_empty = df_world_intern_string(w, "", 0);
    DF_CHECK(id_empty == 0, "intern of empty content returns id 0 sentinel");
    DF_CHECK(df_world_string_pool_count(w) == count_before_empty,
             "string pool count unchanged after intern of empty content");

    char empty_buf[8] = {0};
    int32_t empty_written = df_world_resolve_string(
        w, /*id=*/0, /*generation=*/0, empty_buf, sizeof(empty_buf));
    DF_CHECK(empty_written == 0,
             "resolve of empty sentinel id returns 0 bytes (treated as not-found)");

    // Cross-check: also resolve with a non-zero generation (any value).
    // Empty sentinel id 0 is shaped as "always not-found" for resolve,
    // independent of the generation tag passed in.
    empty_written = df_world_resolve_string(
        w, /*id=*/0, /*generation=*/12345, empty_buf, sizeof(empty_buf));
    DF_CHECK(empty_written == 0,
             "resolve of empty sentinel id is generation-independent");
```

The existing line `df_world_destroy(w);` follows immediately and remains unchanged.

**Verify the selftest builds and passes**:

```
cd native/DualFrontier.Core.Native
cmake --build build --config Release
build/Release/df_native_selftest.exe
```

**Expected**: `ALL PASSED` with the new sub-scenario integrated into `scenario_string_pool`.

**Atomic commit**: `test(native): cover string pool empty-string sentinel in selftest`

---

## Phase 5 — Managed bridge tests

**File**: `tests/DualFrontier.Core.Interop.Tests/InternedStringTests.cs`

Append two new tests at the end of the existing test class (preserve any existing using statements and class declaration):

```csharp
    [Fact]
    public void EmptyString_RoundTrip_YieldsEmptySentinel()
    {
        using var world = new NativeWorld();

        InternedString empty = world.InternString("");
        empty.IsEmpty.Should().BeTrue("intern of empty content yields the empty sentinel");
        empty.Id.Should().Be(0u);
        empty.Generation.Should().Be(0u);

        // Empty sentinel value-equals a default-constructed InternedString.
        empty.Should().Be(default(InternedString));
        (empty == default).Should().BeTrue();

        // Resolve of the empty sentinel returns null.
        empty.Resolve(world).Should().BeNull("empty sentinel has no content to resolve");

        // Pool count is unchanged by interning the empty string.
        int countAfterEmpty = world.StringPoolCount;
        countAfterEmpty.Should().Be(0, "string pool count is unchanged after empty intern");
    }

    [Fact]
    public void EqualsByContent_ReturnsTrueForBothEmpty_RegardlessOfWorld()
    {
        using var worldA = new NativeWorld();
        using var worldB = new NativeWorld();

        InternedString emptyA = worldA.InternString("");
        InternedString emptyB = worldB.InternString("");

        emptyA.EqualsByContent(emptyB, worldA, worldB).Should().BeTrue(
            "two empty InternedStrings are equal by content irrespective of world");
        emptyA.EqualsByContent(emptyA, worldA, worldA).Should().BeTrue(
            "an empty InternedString equals itself by content trivially");
    }

    [Fact]
    public void EqualsByContent_ReturnsFalseWhenOnlyOneSideIsEmpty()
    {
        using var world = new NativeWorld();

        InternedString empty = world.InternString("");
        InternedString nonEmpty = world.InternString("Foo");

        empty.EqualsByContent(nonEmpty, world, world).Should().BeFalse();
        nonEmpty.EqualsByContent(empty, world, world).Should().BeFalse();
    }

    [Fact]
    public void EqualsByContent_SamePool_FastPathReturnsTrueOnEqualIds()
    {
        using var world = new NativeWorld();

        InternedString a = world.InternString("Foo");
        InternedString b = world.InternString("Foo");

        a.Equals(b).Should().BeTrue("same-pool intern of same content returns equal id pair");
        a.EqualsByContent(b, world, world).Should().BeTrue(
            "EqualsByContent agrees with == on the same-pool fast path");
    }

    [Fact]
    public void EqualsByContent_SamePool_DifferentContentReturnsFalse()
    {
        using var world = new NativeWorld();

        InternedString foo = world.InternString("Foo");
        InternedString bar = world.InternString("Bar");

        foo.EqualsByContent(bar, world, world).Should().BeFalse();
    }

    [Fact]
    public void EqualsByContent_CrossPool_ReturnsTrueForIdenticalContent()
    {
        using var worldA = new NativeWorld();
        using var worldB = new NativeWorld();

        InternedString fromA = worldA.InternString("SharedContent");
        InternedString fromB = worldB.InternString("SharedContent");

        // The (Id, Generation) pairs may or may not match across worlds —
        // pool allocation is independent. Whether == returns true is therefore
        // not specified across pools; EqualsByContent must return true
        // because the resolved content is identical.
        fromA.EqualsByContent(fromB, worldA, worldB).Should().BeTrue(
            "cross-pool comparison by content returns true when content matches");
    }

    [Fact]
    public void EqualsByContent_CrossPool_ReturnsFalseForDifferentContent()
    {
        using var worldA = new NativeWorld();
        using var worldB = new NativeWorld();

        InternedString fromA = worldA.InternString("ContentA");
        InternedString fromB = worldB.InternString("ContentB");

        fromA.EqualsByContent(fromB, worldA, worldB).Should().BeFalse();
    }

    [Fact]
    public void EqualsByContent_NullWorlds_Throws()
    {
        using var world = new NativeWorld();

        InternedString s = world.InternString("Foo");

        Action callWithNullThis = () => s.EqualsByContent(s, null!, world);
        Action callWithNullOther = () => s.EqualsByContent(s, world, null!);

        callWithNullThis.Should().Throw<ArgumentNullException>().WithParameterName("thisWorld");
        callWithNullOther.Should().Throw<ArgumentNullException>().WithParameterName("otherWorld");
    }

    [Fact]
    public void EqualsByContent_StaleGeneration_ReturnsFalse()
    {
        using var world = new NativeWorld();

        // Intern a string under ModA scope.
        world.BeginModScope("ModA");
        InternedString underModA = world.InternString("ModAExclusiveContent");
        world.EndModScope("ModA");

        // Capture the same-content id as it currently stands.
        InternedString freshLookup = world.InternString("ModAExclusiveContent");
        underModA.Equals(freshLookup).Should().BeTrue(
            "before ModA clear, the pre-clear and fresh-intern handles match");

        // Clear ModA — the id is reclaimed and the generation advances.
        world.ClearModScope("ModA");

        // Re-intern the same content. The id may be reused but with a new
        // generation tag, so the pre-clear handle is now stale.
        InternedString postClear = world.InternString("ModAExclusiveContent");

        // Same-pool fast path: pre-clear handle compares unequal to the
        // post-clear handle by (Id, Generation) — generations differ even
        // if ids coincide.
        underModA.Equals(postClear).Should().BeFalse(
            "post-clear re-intern advances the generation tag");

        // EqualsByContent: the pre-clear handle resolves to null (stale
        // generation), so the comparison returns false even though the
        // content of postClear matches what underModA used to refer to.
        underModA.EqualsByContent(postClear, world, world).Should().BeFalse(
            "stale generation makes content resolution null; comparison is false");
    }
```

**Run the tests**:

```
dotnet test tests/DualFrontier.Core.Interop.Tests/DualFrontier.Core.Interop.Tests.csproj
```

**Expected**: all 583 prior tests + 9 new K8.1.1 tests = **592 tests passing**.

**Atomic commits**:

1. `test(interop): cover InternedString.EqualsByContent across same-pool and cross-pool scenarios` — covers the 7 `EqualsByContent_*` tests.
2. `test(interop): cover InternedString empty-string sentinel round-trip` — covers `EmptyString_RoundTrip_YieldsEmptySentinel`.

Order: empty-sentinel test commits first if the executor prefers (it depends only on K8.1 baseline); the EqualsByContent tests commit immediately after Phase 2's method commit. Adjust ordering only to keep each commit individually buildable; do not reorder if it forces a broken-build intermediate state.

---

## Phase 6 — Closure verification

### 6.1 — Update `MIGRATION_PROGRESS.md`

Locate the K8.1 closure section. Immediately after it (and before any K8.2 row that may exist as a skeleton), insert the K8.1.1 closure section:

```markdown
### K8.1.1 — InternedString closure follow-up: cross-pool equality, doc semantics, empty-string coverage

**Status**: DONE
**Closure**: `<phase-2-sha>..<phase-5-sha>` on `feat/k8-1-1-interned-string-followup` (fast-forward merged to main)
**Brief**: `tools/briefs/K8_1_1_INTERNED_STRING_FOLLOWUP_BRIEF.md`
**Test count**: 583 → 592 (+9: 7 EqualsByContent scenarios + 2 empty-string round-trip + native selftest sub-scenario count unchanged at 21 scenarios but 3 new sub-checks inside scenario_string_pool)

**Deliverables**:

- `InternedString.EqualsByContent(InternedString, NativeWorld, NativeWorld)` — explicit cross-pool comparison method with two-world signature. Same-pool fast path preserved via short-circuit when `ReferenceEquals(thisWorld, otherWorld) && Equals(other)`. Empty-on-both-sides returns true regardless of worlds. Stale-generation resolution returns false.
- `InternedString` doc-comment expanded with same-pool/cross-pool/Solution-A/stale-id/save-load sections. Communicates the multi-world failure mode of `==` that LOCKED #5 in K8.1 brief specified but the K8.1 implementation comment elided.
- Empty-string sentinel coverage: native selftest sub-scenario in `scenario_string_pool` (intern empty → id 0; resolve id 0 with any generation → 0 bytes; pool count unchanged). Managed bridge test `EmptyString_RoundTrip_YieldsEmptySentinel`.

**Brief deviations** (if any): record here per session findings. Expected zero structural deviations — Phase 1 design is implemented as written. Test count match to projection (592) is the gate.

**Architectural decisions LOCKED in this milestone**:

- Two-world signature for cross-pool comparison (no single-world overload). Misuse becomes structurally impossible at the API surface; same-pool callers pay zero extra cost via reference-equality short-circuit.

**Cross-cutting impact**:

- K8.2 component conversion may use `EqualsByContent` where cross-pool semantics applies. K8.1.1 only adds the surface; K8.2 onward is the consumer.
```

Replace `<phase-2-sha>..<phase-5-sha>` with the actual commit range after merge. The section is final and ready for K8.2 brief authoring to reference.

Also update the "Current state snapshot" or equivalent live tracker section at the top of `MIGRATION_PROGRESS.md` to reflect:

- K8.1.1 status DONE
- Test count 592
- Most-recent closure: K8.1.1

**Atomic commit**: `docs(progress): record K8.1.1 closure (InternedString follow-up)`

### 6.2 — Mark brief EXECUTED

Edit the front matter of this brief (`tools/briefs/K8_1_1_INTERNED_STRING_FOLLOWUP_BRIEF.md`):

```markdown
**Status**: EXECUTED (2026-MM-DD, branch `feat/k8-1-1-interned-string-followup`, closure `<phase-2-sha>..<phase-5-sha>`). See `docs/MIGRATION_PROGRESS.md` § "K8.1.1 — InternedString closure follow-up" for closure record.
```

Replace `2026-MM-DD` with the actual execution date and the SHA range with the actual commits.

**Atomic commit**: `docs(briefs): mark K8.1.1 brief EXECUTED`

### 6.3 — Final verification

Full rebuild + test + selftest + debt grep on K8.1.1-touched files:

```
cd native/DualFrontier.Core.Native
cmake --build build --config Release
build/Release/df_native_selftest.exe
cd ../..
dotnet build
dotnet test
grep -nE "TODO|FIXME|XXX|HACK" \
  src/DualFrontier.Core.Interop/InternedString.cs \
  tests/DualFrontier.Core.Interop.Tests/InternedStringTests.cs \
  native/DualFrontier.Core.Native/test/selftest.cpp
```

**Expected**:

- Native build clean
- Native selftest ALL PASSED
- Managed solution: **592 tests passing**
- Zero TODO/FIXME/XXX/HACK in K8.1.1-touched files (the grep on `selftest.cpp` may match pre-existing markers in K0-K7 scenarios; these are out of K8.1.1 scope. Limit the assertion to "no new debt markers introduced by K8.1.1" — diff against K8.1 baseline if needed).

### 6.4 — Merge to main

Branch is N commits ahead of `origin/main`. Fast-forward merge:

```
git checkout main
git merge --ff-only feat/k8-1-1-interned-string-followup
```

If a non-fast-forward situation arises (main moved during execution), halt and report — K8.1.1 expects to land cleanly on top of K8.1 closure SHA `059f712` with no intervening commits.

Do **not** push to origin. Per established auto-mode convention, `git push` is a Crystalka decision after closure report.

---

## Stop conditions

Halt and surface the situation to Crystalka if any of the following occur:

1. **Phase 0 baseline failure**: K8.1 closure SHA does not match `059f712`, baseline test count is not 583, or native selftest fails on baseline. K8.1.1 cannot proceed against an unknown predecessor state.

2. **Test count drift after Phase 5**: managed test count is not exactly 592 (583 + 9). Either a test was missed, an assertion failed, or an unexpected test was added/removed. Investigate before committing the final closure docs.

3. **`EqualsByContent_StaleGeneration_ReturnsFalse` test failure**: the test depends on the K8.1 mod-scope reclaim semantics. If it fails, either the K8.1 reclaim implementation differs from what the brief assumes or the test setup is wrong. Read `string_pool.cpp::clear_mod_scope` and reconcile before patching the test.

4. **Doc-comment stale-ref grep**: after Phase 3 doc-comment expansion, grep the codebase for any references to the *old* doc-comment phrasing (e.g., the original "Equality is by (Id, Generation) — both halves must match" line). If matched anywhere outside `InternedString.cs` itself (e.g., in another file's `<see cref>` or in copy-pasted doc snippets), update those references in the same Phase 3 commit.

5. **CMakeLists.txt requires update**: Phase 4 adds new sub-scenario lines inside an existing scenario function in `selftest.cpp`. The file is already in CMakeLists; no CMake update is required. If for any reason `selftest.cpp` was split out or refactored between K8.1 and K8.1.1, halt and reconcile.

6. **Surprise architectural drift**: any observation during execution that suggests `InternedString` semantics differ from K8.1 LOCKED #5 in ways not captured by this brief — e.g., id reuse without generation bump, mod-scope clear not reclaiming uniquely-owned ids, generation overflow concerns. Halt; this becomes K8.1.2 territory, not silent fix-in-flight.

---

## Atomic commit log expected

| # | Phase | Message |
|---|---|---|
| 1 | 0.7 | `docs(briefs): author K8.1.1 InternedString follow-up brief` |
| 2 | 2 | `feat(interop): add InternedString.EqualsByContent for cross-pool comparison` |
| 3 | 3 | `docs(interop): expand InternedString doc-comment with multi-world semantics` |
| 4 | 4 | `test(native): cover string pool empty-string sentinel in selftest` |
| 5 | 5 | `test(interop): cover InternedString empty-string sentinel round-trip` |
| 6 | 5 | `test(interop): cover InternedString.EqualsByContent across same-pool and cross-pool scenarios` |
| 7 | 6.1 | `docs(progress): record K8.1.1 closure (InternedString follow-up)` |
| 8 | 6.2 | `docs(briefs): mark K8.1.1 brief EXECUTED` |

Total: **8 atomic commits**. Order between commits 5 and 6 is interchangeable; both are test-only and both depend only on commit 2's method addition.

---

## Cross-cutting design constraints

- **No native-side changes**: K8.1.1 is managed-side and test-only. The native `StringPool` and C ABI are untouched. This keeps K8.1.1 a pure managed amendment, isolating risk.

- **No new ABI entries**: `EqualsByContent` is implemented entirely on the managed side using `df_world_resolve_string`. The ABI stays at the K8.1 surface count.

- **Atomic-commit-as-compilable-unit** (per K8.1 lesson): each commit must build clean. Phase 5 split into two commits (empty-sentinel, EqualsByContent suite) is permitted because each subset compiles and tests independently. If the executor judges them inseparable, bundle into one commit — but commits 4 (native) and 5/6 (managed) are independently buildable and stay separate.

- **Doc-comment as design contract**: the expanded XML doc-comment in Phase 3 is the user-facing source of truth for `InternedString` multi-world semantics. K8.2 component authors will read it as authoritative when deciding between `==` and `EqualsByContent`. Treat it with the same rigour as the Phase 1 architectural decisions.

- **No dependency on K9 or K8.2**: K8.1.1 closes cleanly without any forward references to milestones not yet authored or executed. K9 and K8.2 will reference K8.1.1 deliverables in their own briefs once those briefs are authored; K8.1.1 itself has no such forward dependencies.

---

## Execution closure

When all phases complete and all gates pass, K8.1.1 is **DONE**.

Closure report to Crystalka should include:

- Commit range merged to main
- Test count delta (583 → 592)
- Native selftest sub-check delta (existing `scenario_string_pool` body extended with 3 sub-checks; scenario count unchanged at 21)
- Any deviations from this brief recorded under "Brief deviations" in the MIGRATION_PROGRESS.md K8.1.1 closure section
- Any architectural surprises that should inform K8.2 brief authoring

After closure report, Crystalka decides:

- Whether to push `main` to `origin`
- Whether K8.2 brief authoring proceeds immediately (recommended: yes, K8.1.1 unblocks the API surface K8.2 needs)
- Any K8.1.2-shaped follow-up if architectural surprises were surfaced

End of K8.1.1 brief.
