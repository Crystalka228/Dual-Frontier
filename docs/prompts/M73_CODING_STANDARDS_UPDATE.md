# CODING_STANDARDS update — Stack-frame retention discipline (M7.3 empirical contribution)

## Контекст

M7.3 closed (commits `9bed1a4`, `46b4f33`, `1d43858`). 369/369 tests passing. Working tree clean. Implementation surfaced an empirical engineering finding worth персистить в `docs/methodology/CODING_STANDARDS.md` отдельным docs commit перед M7.4 — display-class hoisting от lambdas hoist'ит captured args в compiler-synthesized `<>c__DisplayClass`, allocated в caller scope. Этот display class остаётся strongly rooted из caller's stack frame на ВЕСЬ scope метода, что ломает любой downstream code path который зависит на отсутствии strong refs (ALC release, finalizer-based cleanup, weak-reference-based cache eviction).

Сценарий не tied к §9.5 step 7 specifically — это general runtime discipline для любого code path который должен дать GC release некий resource. AD #3 в M7.3 brief формулировал «non-inlined helpers + WR-only signature for spin», но empirical finding показал scope broader: то же дисциплина применяется к **closure-display-class hoisting**, не только к explicit locals или lambda spin signatures.

## Задача

Single docs commit. Modifies `docs/methodology/CODING_STANDARDS.md` only. Adds new section persisting M7.3's empirical lesson так чтобы M8+/M10 implementations не наступили на те же грабли независимо.

## Required reading

1. `docs/methodology/CODING_STANDARDS.md` — full document. Особое внимание: existing structure (PascalCase / file-scoped namespaces / Nullable / English comments / one class per file / member order / Additional rules / See also). Tone — sober, prescriptive, motivation-first, code snippets BAD vs CORRECT.
2. `docs/ROADMAP.md` — M7.3 sub-phase closure entry в M7 section. **Это authoritative source для empirical narrative**: full account of `RunUnloadSteps1Through6AndCaptureAlc` extraction rationale, `SnapshotActiveModIds` extraction rationale, why both helpers are non-inlined, why DEBUG-build retention specifically. Не выдумывать additional context — rule of thumb в новой секции paraphrases этот paragraph.
3. `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` — реальная implementation reference: `RunUnloadSteps1Through6AndCaptureAlc`, `SnapshotActiveModIds`, `CaptureAlcWeakReference`, `TryStep7AlcVerification`. The XML docs of these helpers carry the finding inline — readable for code-comment style reference if the new CODING_STANDARDS section needs cross-link wording.

## Required edits

### Add new section between «Additional rules» and «See also»

Header: `## Stack-frame retention для collected resources`.

Section structure parallel к existing sections (motivation paragraph, code BAD/CORRECT pair, rationale):

#### Opening paragraph (1–2 sentences)

«Some code paths must give the GC an opportunity to collect a resource — `AssemblyLoadContext` unload, finalizer-based cleanup, weak-reference-based cache eviction. These paths run AFTER all strong refs to the resource have been dropped from the executing thread's stack frames. Two C# constructs silently retain strong refs in ways that can defeat such code paths if not handled with discipline: the iteration variable of a `foreach` loop (in DEBUG builds, until the enclosing method returns) and **lambda closure display classes** (always, in any build).»

#### The display-class hoisting problem (2–3 sentences)

Explain that a lambda capturing a local variable causes the C# compiler to synthesize `<>c__DisplayClass` heap object containing that variable as field; the display class is allocated на entry to the enclosing method и rooted from that method's stack frame for the entire method scope. Any local captured by even one lambda is strongly reachable until the enclosing method returns, regardless of how «out of scope» the local appears textually.

#### BAD example

Single function где lambda captures a `LoadedMod mod` (или ANY resource holder), и потом the function tries to spin on a `WeakReference` к that resource later in the same scope:

```csharp
// BAD — display class rooted in this method's frame keeps `mod` (and
// thereby mod.Context) alive through the spin. WR.IsAlive never flips
// to false; spin times out на every invocation.
public IReadOnlyList<ValidationWarning> UnloadMod(string modId)
{
    if (!_activeMods.TryGetValue(modId, out LoadedMod? mod)) return [];

    var warnings = new List<ValidationWarning>();
    TryUnloadStep(1, modId, warnings, () => mod.Api?.UnsubscribeAll());
    // ... more lambdas capturing mod ...

    var alcRef = new WeakReference(mod.Context);
    _activeMods.Remove(mod);

    // <-- display class still rooted; mod.Context still strongly reachable.
    for (int i = 0; i < 100; i++)
    {
        GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
        if (!alcRef.IsAlive) return warnings;
        Thread.Sleep(100);
    }
    // Always reaches here. Always emits ModUnloadTimeout.
    warnings.Add(new ValidationWarning(modId, "ModUnloadTimeout: ..."));
    return warnings;
}
```

#### CORRECT example

Extract any code path that captures the resource via lambda (or holds it as a local) into a non-inlined helper that returns ONLY the WeakReference; the caller invokes the spin/wait phase with an empty stack frame containing no display class и no local strong refs:

```csharp
// CORRECT — work-with-refs phase isolated в non-inlined helper that
// returns only the WeakReference. Caller's frame holds nothing strong.
public IReadOnlyList<ValidationWarning> UnloadMod(string modId)
{
    if (!_activeMods.TryGetValue(modId, out LoadedMod? mod)) return [];

    var warnings = new List<ValidationWarning>();
    WeakReference alcRef = RunUnloadSteps1Through6AndCaptureAlc(modId, mod, warnings);

    // <-- mod is no longer referenced; display class lives only в
    //     the helper's frame, which has returned. Frame is clean.
    TryStep7AlcVerification(modId, alcRef, warnings);
    return warnings;
}

[MethodImpl(MethodImplOptions.NoInlining)]
private WeakReference RunUnloadSteps1Through6AndCaptureAlc(
    string modId, LoadedMod mod, List<ValidationWarning> warnings)
{
    TryUnloadStep(1, modId, warnings, () => mod.Api?.UnsubscribeAll());
    // ... lambdas captured here are confined to THIS frame's display class ...
    var alcRef = new WeakReference(mod.Context);
    _activeMods.Remove(mod);
    return alcRef;
}

[MethodImpl(MethodImplOptions.NoInlining)]
private static void TryStep7AlcVerification(
    string modId, WeakReference alcRef, List<ValidationWarning> warnings)
{ /* spin with GC pump bracket; signature carries no resource ref */ }
```

#### Rationale paragraph (3–4 sentences)

- Pattern surfaced empirically в M7.3 (commit `9bed1a4`) когда initial implementation kept `mod` as a local in `UnloadMod` and 3 of 13 M7.2 regression tests started emitting `ModUnloadTimeout` warnings the assertions weren't expecting.
- Root cause was NOT the JIT retaining the explicit local — it was C# compiler's display-class hoisting for the lambdas passed to `TryUnloadStep`. Step 1's lambda captures `mod` (`mod.Api?.UnsubscribeAll()`), so the compiler synthesized a heap `<>c__DisplayClass` holding `mod` as a field, allocated on entry to `UnloadMod` and rooted from `UnloadMod`'s stack frame for the entire method scope.
- Same patología surfaced в `UnloadAll`'s `foreach (LoadedMod mod in _activeMods)` snapshot loop: the iteration variable's last value persists в DEBUG stack slot through the rest of the method (fixed by extracting `SnapshotActiveModIds`).
- Rule of thumb: if a method needs to give the GC a chance to collect a resource it referenced, that method's body must split into **(a)** a non-inlined helper that captures, works with, and releases its strong refs to the resource, returning only a WeakReference (or nothing at all), and **(b)** the caller, which invokes the wait/spin phase with no resource-holding locals or lambdas in its frame.

#### Sub-bullet — `[MethodImpl(MethodImplOptions.NoInlining)]`

The non-inlined attribute is non-negotiable on both helpers — без него JIT may inline the helper back into the caller, recreating the display class в caller's frame и defeating the discipline. The attribute MUST sit on every helper that exists для the sole purpose of containing strong refs to a resource that subsequently must be released.

#### Sub-bullet — closing

Reuse this pattern for any future code path с similar shape: ALC unload (M7.x), finalizer-driven cleanup, weak-reference-based caches, or any GC-dependent test assertion. Test-side helpers (e.g. `ModUnloadAssertions.AssertAlcReleasedWithin`) follow the same discipline as production-side helpers (`TryStep7AlcVerification`).

### Cross-reference cleanup

«See also» list at bottom: existing ARCHITECTURE / TESTING_STRATEGY / ISOLATION cross-refs remain accurate. Add `[MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md)` если ещё не присутствует — new section references §9.5; verify before adding.

## Acceptance criteria

1. `dotnet build` clean — pure docs commit, нечего билдить, но verify no broken markdown.
2. New section reads coherently after «Additional rules» и before «See also».
3. Tone matches existing sections (sober, prescriptive, motivation-first, BAD/CORRECT code pairs).
4. M7.3 commit `9bed1a4` referenced by SHA в the rationale paragraph.
5. `[MethodImpl(MethodImplOptions.NoInlining)]` requirement explicit (not optional).
6. Markdown renders cleanly.

## Финал

Pure docs commit:

```
docs(coding-standards): persist M7.3's display-class hoisting finding

Adds "Stack-frame retention для collected resources" section after the
"Additional rules" block. Captures the empirical engineering rule that
surfaced in M7.3 commit 9bed1a4: any code path needing to give the GC
a chance to collect a resource must split (a) a non-inlined helper
holding all strong refs (locals + lambda display classes) which
returns only a WeakReference, and (b) the caller, which invokes the
wait/spin phase with an empty frame. The rule applies to ALC unload,
finalizer cleanup, WR-based caches, and GC-dependent test assertions;
carries forward to M7.4+ and M10.X implementations.
```

Report back: commit SHA + brief confirmation that section added between «Additional rules» и «See also», tone matches existing prose, markdown renders cleanly.
