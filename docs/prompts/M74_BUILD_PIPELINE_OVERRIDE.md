---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-M74_BUILD_PIPELINE_OVERRIDE
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-M74_BUILD_PIPELINE_OVERRIDE
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-M74_BUILD_PIPELINE_OVERRIDE
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-M74_BUILD_PIPELINE_OVERRIDE
---
# M7.4 — Build-pipeline `hotReload` override per D-7

## Context

M3, M4, M5, M6 closed на main с closure verification reports. M7.1 + M7.2 + M7.3 closed (commits `a2ab761`, `c964475`, `2531ed7`, `d68ba93`, `c3f5251`, `9bed1a4`, `46b4f33`, `1d43858`). 369/369 tests passing. Working tree clean. Standalone CODING_STANDARDS update closed (commit `f4b2cb8`) — M7.3's display-class hoisting finding persisted.

`MOD_OS_ARCHITECTURE.md` LOCKED v1.5. D-7 specifies the build-pipeline override mechanism for vanilla mods' `hotReload` flag — vanilla mods declare `hotReload: true` in source so dev gets free hot-reload testing; the shipping build pipeline rewrites this to `hotReload: false` in release manifests so shipped builds get stable session experience. M7.4 implements the build-pipeline tool + MSBuild integration + test fixtures. M8 (vanilla mod skeletons, ⏭ Pending) plugs into this mechanism — its acceptance criterion ("Vanilla manifest `hotReload: true` in source; build-pipeline override correctness verified by a CI test") becomes M7.4's deliverable.

Approved decomposition per METHODOLOGY §2.4: M7 split into M7.1, M7.2, M7.3, **M7.4** ← this session, M7.5, M7-closure.

M7.4 scope: ship the manifest-rewriter CLI tool, the `mods/Directory.Build.targets` MSBuild integration with `<IsVanillaMod>` opt-in, a test fixture vanilla mod for end-to-end integration testing, and unit + integration test coverage of the rewriter contract.

## Out of scope (M7.5 / M7-closure / M8 will do — NOT in this session)

- Mod-menu UI integration (Apply button, Pause/Resume wiring, hot-reload disabled tooltip per §9.6) — M7.5.
- ROADMAP M7 row closure → M7-closure session post-M7.5 (parallel pattern to M5/M6 closure precedent).
- Vanilla mod skeletons themselves — M8. M7.4 ships the mechanism + a test fixture; real vanilla mod manifests appear in M8.
- Modifications to existing `mods/DualFrontier.Mod.Example/` — it is not a vanilla mod (no `<IsVanillaMod>` flag). Stays unchanged as the non-vanilla control case.
- Modifications to `DualFrontier.Core` or `DualFrontier.Contracts` — M-phase boundary discipline preserved. Verified via `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returning empty.
- Per-mod packaging logic (zip/installer creation). The rewriter operates on the `bin/Release/{TFM}/` output directly; downstream packaging is a separate concern not pinned in this session.

## Approved architectural decisions

1. **Mechanism: standalone .NET console tool + MSBuild target hybrid.** New `tools/DualFrontier.Mod.ManifestRewriter/` console app exposes a one-shot CLI (`--path <manifest.json>` reads, flips `hotReload` flag, writes back). New `mods/Directory.Build.targets` defines a target that, after `Build`, scoped to `<IsVanillaMod>true</IsVanillaMod>` projects on `$(Configuration)=='Release'`, invokes the rewriter on the output manifest. Per D-7 wording "single flag in build script — no code branching needed": the flag is the MSBuild property, the script is `Directory.Build.targets`.

2. **Filter: explicit MSBuild opt-in via `<IsVanillaMod>true</IsVanillaMod>`.** Not id-prefix matching, not path-parsing — explicit opt-in by the engine team via .csproj property. Robust, falsifiable, requires deliberate inclusion. Third-party mods do not need to know about `<IsVanillaMod>` at all; default is `false`.

3. **Source preservation: rewriter never modifies source manifest.** Operates on `bin/{Configuration}/{TFM}/mod.manifest.json` after CopyToOutputDirectory. Source file stays `hotReload: true` so git diff between dev and shipped builds is empty for source. Verified by integration test asserting source manifest unchanged after Release build.

4. **`mod.manifest.json` deployment via CopyToOutputDirectory: opt-in same way.** Targets file conditionally adds `<None Include CopyToOutputDirectory="PreserveNewest" />` for `<IsVanillaMod>true</IsVanillaMod>` projects. Existing non-vanilla mods (ExampleMod) unchanged — they do not currently copy manifest to bin and continue not to.

5. **Identification for "vanilla" — MSBuild property, not manifest content.** Rewriter operates on any manifest path it is invoked with, with no filtering of its own. The MSBuild target decides which projects participate based on `<IsVanillaMod>`. Separating identification (MSBuild) from operation (tool) keeps the tool single-purpose and reusable for any future "rewrite on shipping" need.

6. **JSON tooling: System.Text.Json.** Project uses BCL throughout (existing `ManifestParser` already on System.Text.Json — verify before commit). No Newtonsoft.Json dependency introduction.

7. **Idempotency.** Rewriter on already-`hotReload: false` manifest is a no-op, exits 0. Manifest with `hotReload` field absent is treated as no-op (default `false` per §2.2; rewriter does NOT add the field — adding it would silently change the document shape and break round-trip equality for non-vanilla cases). Manifest with `hotReload: true` rewrites to `hotReload: false`.

8. **CLI surface and exit codes.**
   - Invocation: `dotnet DualFrontier.Mod.ManifestRewriter.dll --path <manifest.json>`.
   - Exit 0: Rewritten | AlreadyFalse | FieldAbsent (success or successful no-op).
   - Exit 1: NotFound (path does not exist).
   - Exit 2: ParseError (invalid JSON).
   - Exit 3: WriteError (file unwritable, disk full, etc.).
   - All non-zero exits print a diagnostic line to stderr; stdout silent on success.

9. **Library entry point shape for testability.** Tool exposes a `public static class ManifestRewriter` with `public static Result Rewrite(string manifestPath)` returning a discriminated `Result` enum (`Rewritten`, `AlreadyFalse`, `FieldAbsent`, `NotFound`, `ParseError`, `WriteError`). `Program.Main` parses args, calls `Rewrite`, maps `Result` → exit code + stderr message. Unit tests invoke `ManifestRewriter.Rewrite` directly without subprocess.

10. **Test surface: two layers.** Unit tests on `ManifestRewriter.Rewrite` library entry point (in-process, fast, exhaustive). Integration tests spawn `dotnet build -c Release` subprocess against `tests/Fixture.VanillaMod_HotReloadOverride/`, assert bin output has `hotReload: false` while source untouched. Integration tests marked `[Trait("Category", "Integration")]`; remain in default `dotnet test` run but can be filtered out for fast loops.

11. **§11.4 stop-condition discipline applies.** New integration test surface (subprocess invocation of MSBuild) runs at least 3 consecutive times in CI verification before commit; any flake → halt + escalate as v1.6 ratification candidate.

12. **Atomic phase review per METHODOLOGY §2.4** — implementation, tests, ROADMAP closure all in one session. Three-commit invariant per §7.3.

## Required reading

1. `docs/architecture/MOD_OS_ARCHITECTURE.md` LOCKED v1.5 — §2.2 (`hotReload` field row in manifest reference table; default `false`; v1 manifest grace), §9.6 (hot-reload disabled mods semantics — menu disables button, restart required), D-7 (LOCKED override mechanism wording), §11.1 M7 row, §11.2 (no new error kinds for M7.4).
2. `docs/ROADMAP.md` — M7 sub-phase status block, M7.4 entry "Build-pipeline override per D-7", M8 entry acceptance criterion ("Vanilla manifest `hotReload: true` in source; build-pipeline override correctness verified by a CI test").
3. `docs/methodology/METHODOLOGY.md` — §2.4 atomic phase review, §7.3 three-commit invariant.
4. `docs/methodology/CODING_STANDARDS.md` — English-only comments, file-scoped namespaces, member order, `_camelCase` private fields, PascalCase public, **Stack-frame retention for collected resources** section (added in commit `f4b2cb8`; not directly relevant to M7.4 but locks the broader discipline).
5. Build infrastructure:
   - `Directory.Build.props` — solution-wide props (net8.0, nullable, TreatWarningsAsErrors). New file `mods/Directory.Build.targets` will be subordinate to this; verify property inheritance.
   - `mods/DualFrontier.Mod.Example/DualFrontier.Mod.Example.csproj` + `mod.manifest.json` — control case for non-vanilla mod project shape.
   - `mods/README.md` — current mod conventions (only references `DualFrontier.Contracts`).
6. Existing test fixture pattern — `tests/Fixture.RegularMod_DependedOn/` for shape reference (project structure, manifest layout, .csproj minimum surface).
7. Manifest parser implementation — `src/DualFrontier.Application/Modding/ManifestParser.cs` for confirmation of System.Text.Json as the JSON library used elsewhere in the project.
8. Solution file — confirm `.sln` location and structure for adding new projects.

## Implementation

### 1. New `tools/DualFrontier.Mod.ManifestRewriter/`

Top-level `tools/` directory created. Console app project:

- `tools/DualFrontier.Mod.ManifestRewriter/DualFrontier.Mod.ManifestRewriter.csproj` — `<OutputType>Exe</OutputType>`, net8.0, no project references (pure System.Text.Json).
- `tools/DualFrontier.Mod.ManifestRewriter/Program.cs` — `Main(string[] args)`: parse `--path <value>`, call `ManifestRewriter.Rewrite`, map result → exit code + stderr.
- `tools/DualFrontier.Mod.ManifestRewriter/ManifestRewriter.cs`:

```csharp
namespace DualFrontier.Mod.ManifestRewriter;

public static class ManifestRewriter
{
    public enum Result
    {
        Rewritten,
        AlreadyFalse,
        FieldAbsent,
        NotFound,
        ParseError,
        WriteError
    }

    public static Result Rewrite(string manifestPath)
    {
        if (!File.Exists(manifestPath)) return Result.NotFound;

        string text;
        JsonNode? node;
        try
        {
            text = File.ReadAllText(manifestPath);
            node = JsonNode.Parse(text);
        }
        catch (JsonException) { return Result.ParseError; }
        catch (IOException) { return Result.NotFound; }

        if (node is not JsonObject obj) return Result.ParseError;

        if (!obj.ContainsKey("hotReload")) return Result.FieldAbsent;

        JsonNode? value = obj["hotReload"];
        bool current = value?.GetValue<bool>() ?? false;
        if (!current) return Result.AlreadyFalse;

        obj["hotReload"] = false;

        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(manifestPath, obj.ToJsonString(options));
        }
        catch (IOException) { return Result.WriteError; }
        catch (UnauthorizedAccessException) { return Result.WriteError; }

        return Result.Rewritten;
    }
}
```

`Program.Main` skeleton:

```csharp
namespace DualFrontier.Mod.ManifestRewriter;

internal static class Program
{
    private static int Main(string[] args)
    {
        string? path = null;
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--path") { path = args[i + 1]; break; }
        }
        if (path is null)
        {
            Console.Error.WriteLine("Usage: manifest-rewriter --path <manifest.json>");
            return 1;
        }

        ManifestRewriter.Result result = ManifestRewriter.Rewrite(path);
        return result switch
        {
            ManifestRewriter.Result.Rewritten or
            ManifestRewriter.Result.AlreadyFalse or
            ManifestRewriter.Result.FieldAbsent => 0,

            ManifestRewriter.Result.NotFound => Fail(1, $"Manifest not found: {path}"),
            ManifestRewriter.Result.ParseError => Fail(2, $"Invalid JSON: {path}"),
            ManifestRewriter.Result.WriteError => Fail(3, $"Cannot write: {path}"),
            _ => Fail(99, "Unknown result")
        };
    }

    private static int Fail(int code, string message)
    {
        Console.Error.WriteLine(message);
        return code;
    }
}
```

XML doc on `ManifestRewriter` class explains D-7 contract: source preservation, idempotency, the absent-field no-op rule and its rationale.

### 2. New `mods/Directory.Build.targets`

Solution-level targets file scoped to projects under `mods/`. Default `<IsVanillaMod>` to `false`; conditional CopyToOutputDirectory and post-build rewrite invocation:

```xml
<Project>
  <PropertyGroup>
    <IsVanillaMod Condition="'$(IsVanillaMod)' == ''">false</IsVanillaMod>
  </PropertyGroup>

  <ItemGroup Condition="'$(IsVanillaMod)' == 'true'">
    <None Include="$(MSBuildProjectDirectory)\mod.manifest.json"
          CopyToOutputDirectory="PreserveNewest" />
  </ItemGroup>

  <Target Name="RewriteHotReloadForRelease"
          AfterTargets="Build"
          Condition="'$(IsVanillaMod)' == 'true' AND '$(Configuration)' == 'Release'">
    <PropertyGroup>
      <ManifestRewriterDll>$(MSBuildThisFileDirectory)..\tools\DualFrontier.Mod.ManifestRewriter\bin\$(Configuration)\$(TargetFramework)\DualFrontier.Mod.ManifestRewriter.dll</ManifestRewriterDll>
      <OutputManifestPath>$(OutDir)mod.manifest.json</OutputManifestPath>
    </PropertyGroup>
    <Exec Command="dotnet &quot;$(ManifestRewriterDll)&quot; --path &quot;$(OutputManifestPath)&quot;"
          ConsoleToMSBuild="true" />
  </Target>
</Project>
```

**Path-resolution caveat**: `$(MSBuildThisFileDirectory)` and `$(OutDir)` semantics need empirical validation. Path may resolve differently for fixture project located at `tests/Fixture.VanillaMod_HotReloadOverride/` than for projects at `mods/DualFrontier.Mod.Vanilla.Combat/`. If the targets file relative path breaks for the test fixture, fall back to absolute path or use `$(SolutionDir)`. Confirm during implementation; STOP and escalate if relative paths cannot resolve uniformly.

**Build ordering caveat**: the rewriter tool must be built before any vanilla mod project's Release build runs. Two options:
- (a) Tool project listed first in solution build order (default `dotnet build` builds projects in dependency order; tool has no project references, so it builds before anything else if added first).
- (b) Vanilla mod projects add `<ProjectReference Include="...DualFrontier.Mod.ManifestRewriter.csproj" ReferenceOutputAssembly="false" />` to force build dependency without runtime reference.

(b) is more explicit and less fragile. Brief specifies (b); fixture project should include the ProjectReference. Confirm during implementation that ReferenceOutputAssembly="false" prevents the tool's assembly from being copied into the mod's bin output.

### 3. New `tests/Fixture.VanillaMod_HotReloadOverride/`

Test fixture directory with:

- `Fixture.VanillaMod_HotReloadOverride.csproj`:
  ```xml
  <Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
      <AssemblyName>Fixture.VanillaMod_HotReloadOverride</AssemblyName>
      <RootNamespace>Fixture.VanillaMod_HotReloadOverride</RootNamespace>
      <GenerateDocumentationFile>false</GenerateDocumentationFile>
      <IsVanillaMod>true</IsVanillaMod>
    </PropertyGroup>
    <ItemGroup>
      <ProjectReference Include="..\..\src\DualFrontier.Contracts\DualFrontier.Contracts.csproj" />
      <ProjectReference Include="..\..\tools\DualFrontier.Mod.ManifestRewriter\DualFrontier.Mod.ManifestRewriter.csproj"
                        ReferenceOutputAssembly="false" />
    </ItemGroup>
    <Import Project="..\..\mods\Directory.Build.targets" />
  </Project>
  ```
- `mod.manifest.json` — minimal v2 manifest:
  ```json
  {
    "id": "tests.fixture.vanillamod.hotreloadoverride",
    "name": "Vanilla Mod HotReload Override Fixture",
    "version": "1.0.0",
    "author": "Test",
    "kind": "regular",
    "apiVersion": "^1.0.0",
    "hotReload": true
  }
  ```
- `VanillaModFixture.cs` — trivial `IMod` implementation (empty `Initialize`/`Unload`).

### 4. New `tests/DualFrontier.Mod.ManifestRewriter.Tests/`

xUnit test project for the rewriter tool's library entry point. References `DualFrontier.Mod.ManifestRewriter` and uses FluentAssertions per project precedent.

`ManifestRewriterTests.cs`:

1. **`Rewrite_HotReloadTrue_FlipsToFalse_ReturnsRewritten`** — write a temp manifest with `hotReload: true`, call `Rewrite`, assert returns `Rewritten`, assert file now has `hotReload: false`, assert all other fields preserved.

2. **`Rewrite_HotReloadFalse_NoOp_ReturnsAlreadyFalse`** — manifest with `hotReload: false`, call returns `AlreadyFalse`, file content byte-identical (no rewrite occurred).

3. **`Rewrite_HotReloadAbsent_NoOp_ReturnsFieldAbsent`** — manifest without `hotReload` key, call returns `FieldAbsent`, file content byte-identical (rewriter does NOT add the field per AD #7).

4. **`Rewrite_PreservesAllOtherFields_AfterFlip`** — input with id, name, version, capabilities, dependencies, replaces; after flip, parse output and assert every field except `hotReload` matches input exactly.

5. **`Rewrite_NonexistentPath_ReturnsNotFound`** — path to non-existent file, returns `NotFound`, no file created.

6. **`Rewrite_InvalidJson_ReturnsParseError_FileNotCorrupted`** — manifest with garbage content (e.g. `{ this is not json }`), call returns `ParseError`, original file content unchanged.

7. **`Rewrite_Idempotent_TwiceProducesSameOutput`** — call `Rewrite` twice on `hotReload: true` manifest. First returns `Rewritten`, second returns `AlreadyFalse`. File content stable after second call.

All tests use `Path.GetTempFileName()` or similar for isolated temp files; cleanup in `IDisposable` test class pattern.

### 5. Integration tests

`tests/DualFrontier.Modding.Tests/Pipeline/M74BuildPipelineTests.cs` — two integration tests via subprocess `dotnet build` invocation. Located in existing Modding.Tests project (not a separate test project, to keep test wiring minimal). Both tests use `[Trait("Category", "Integration")]`.

```csharp
public sealed class M74BuildPipelineTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public void BuildRelease_VanillaModFixture_RewritesOutputManifest_PreservesSource() { ... }

    [Fact]
    [Trait("Category", "Integration")]
    public void BuildDebug_VanillaModFixture_DoesNotRewriteOutputManifest() { ... }
}
```

Test 1 implementation outline:
- Locate fixture `.csproj` path relative to test assembly directory.
- Spawn `dotnet build -c Release <fixture.csproj>` via `System.Diagnostics.Process`. Capture stdout + stderr + exit code.
- Assert exit code 0.
- Read source manifest, assert `hotReload: true` (untouched).
- Read `<fixtureDir>/bin/Release/net8.0/mod.manifest.json`, assert `hotReload: false`.

Test 2 — same but `-c Debug`. Assert source `hotReload: true`, output (if it was copied; conditional on `<IsVanillaMod>` ItemGroup which DOES copy) also `hotReload: true`. NB: Debug also copies the manifest because the ItemGroup condition is on `<IsVanillaMod>`, not on configuration; only the rewrite target is configuration-conditional.

If subprocess approach proves flaky in CI (unlikely on dev machines, but worth mentioning), fallback is `Microsoft.Build.Execution.BuildManager` API for in-process MSBuild invocation. STOP and escalate if subprocess hangs or hits I/O timeout.

### 6. Solution file wiring

Add to `.sln`:
- `tools/DualFrontier.Mod.ManifestRewriter/DualFrontier.Mod.ManifestRewriter.csproj`
- `tests/DualFrontier.Mod.ManifestRewriter.Tests/DualFrontier.Mod.ManifestRewriter.Tests.csproj`
- `tests/Fixture.VanillaMod_HotReloadOverride/Fixture.VanillaMod_HotReloadOverride.csproj`

Use `dotnet sln add <path>` for each. Verify after addition that `dotnet build` from solution root builds all three new projects without error.

## Tests

### Unit tests (in `tests/DualFrontier.Mod.ManifestRewriter.Tests/`)

7 tests as enumerated in §4 above.

### Integration tests (in `tests/DualFrontier.Modding.Tests/Pipeline/M74BuildPipelineTests.cs`)

2 tests as enumerated in §5 above.

### Out-of-scope tests (M7.5 / M8 will add)

- Mod-menu UI tests for the `hotReload: false` tooltip — M7.5.
- Real vanilla mod build verification (Vanilla.Combat, Vanilla.Magic, etc. all carrying `<IsVanillaMod>true</IsVanillaMod>` and v2 manifests) — M8 acceptance criterion exercise.
- Per-vanilla-slice dependency tests — M9, M10.

## Acceptance criteria

1. `dotnet build` clean — 0 warnings, 0 errors. New tool project + test project + fixture project all build green.
2. `dotnet test` — 369 existing pass; 9 new pass (7 unit + 2 integration). Expected total: **378/378**.
3. `tools/DualFrontier.Mod.ManifestRewriter/` exists with `ManifestRewriter` library entry point and `Program.Main` CLI wrapper. Exit codes match AD #8.
4. `mods/Directory.Build.targets` exists and is conditional on `<IsVanillaMod>true</IsVanillaMod>`. Default `<IsVanillaMod>` is `false` so existing ExampleMod is unaffected.
5. `tests/Fixture.VanillaMod_HotReloadOverride/` exists, imports `mods/Directory.Build.targets`, has v2 manifest with `hotReload: true`, includes ProjectReference to the rewriter with `ReferenceOutputAssembly="false"` for build-order discipline.
6. After `dotnet build -c Release` against the fixture, source manifest has `hotReload: true` and bin output has `hotReload: false` — verified by integration test 1.
7. After `dotnet build -c Debug` against the fixture, source AND bin output both have `hotReload: true` — verified by integration test 2.
8. ExampleMod (non-vanilla) build behaviour unchanged: `bin/{Configuration}/net8.0/` does NOT contain `mod.manifest.json` (ItemGroup conditional on `<IsVanillaMod>` excludes it), and rewriter is not invoked. Manual verification required before commit; no automated test for non-action.
9. **Zero flake** on integration tests across **at least 3 consecutive `dotnet test` runs**. §11.4 stop-condition discipline: any flake → halt + escalate as v1.6 ratification candidate (subprocess invocation timing, MSBuild concurrency, or path-resolution edge case).
10. M7.1 + M7.2 + M7.3 regression guards still pass: `M71PauseResumeTests` (11) + `M72UnloadChainTests` (13) + `M73Step7Tests` (5) + `M73Phase2DebtTests` (2) all green. No behavioural drift in the unload chain.
11. `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returns empty (M-phase boundary discipline preserved through M7.4).
12. Solution file (`.sln`) includes the three new projects; `dotnet sln list` shows them.

## Финал

Atomic commits in order. Each commit individually must pass `dotnet build && dotnet test`:

**1.** `feat(tools): manifest-rewriter CLI tool for D-7 hotReload override`

- Create `tools/DualFrontier.Mod.ManifestRewriter/` with `DualFrontier.Mod.ManifestRewriter.csproj`, `Program.cs`, `ManifestRewriter.cs`.
- Add to solution.
- Create `mods/Directory.Build.targets` with the conditional ItemGroup and `RewriteHotReloadForRelease` target.
- No tests yet — verify standalone via direct invocation: `dotnet run --project tools/DualFrontier.Mod.ManifestRewriter -- --path <test-manifest.json>` against a temp manifest; manually inspect that flip behaviour matches AD #7 / #8. Document the manual verification in commit message body.

**2.** `test(tools): ManifestRewriter unit tests + Fixture.VanillaMod_HotReloadOverride + M74 integration tests`

- Create `tests/DualFrontier.Mod.ManifestRewriter.Tests/` with `ManifestRewriterTests.cs` (7 unit tests).
- Create `tests/Fixture.VanillaMod_HotReloadOverride/` with `.csproj` (containing `<IsVanillaMod>true</IsVanillaMod>`, ProjectReference to rewriter with `ReferenceOutputAssembly="false"`, import of `mods/Directory.Build.targets`), `mod.manifest.json` (v2, `hotReload: true`), `VanillaModFixture.cs` (trivial `IMod`).
- Add `tests/DualFrontier.Modding.Tests/Pipeline/M74BuildPipelineTests.cs` with 2 integration tests subprocess-invoking `dotnet build`.
- Add new test project + fixture project to solution.
- Run full suite + integration tests at least 3 consecutive times, verify zero flake.

**3.** `docs(roadmap): close M7.4 — D-7 build-pipeline hotReload override`

- `ROADMAP.md` M7 sub-phase status block: M7.4 ⏭ Pending → ✅ Closed with commits 1+2 SHA + acceptance summary in M7.1/M7.2/M7.3 entry pattern. Mention `<IsVanillaMod>` opt-in, `mods/Directory.Build.targets` location, integration test surface.
- Header status line: `*Updated: YYYY-MM-DD (M7.4 closed — D-7 build-pipeline hotReload override via tools/DualFrontier.Mod.ManifestRewriter and mods/Directory.Build.targets; M7.5 + M7-closure pending).*`
- Engine snapshot: 369 → 378 tests. List the 7 unit tests + 2 integration tests (by class name — `ManifestRewriterTests`, `M74BuildPipelineTests`).
- Status overview table M7 row tests column extended: "M7.4 added (`ManifestRewriterTests`, `M74BuildPipelineTests`, `Fixture.VanillaMod_HotReloadOverride` test fixture)".

**Special verification preamble for commits 1 + 2:**

- After commit 1: `dotnet build` green (no test changes, only new projects). Manual: `dotnet run --project tools/DualFrontier.Mod.ManifestRewriter -- --path <temp-manifest.json>` against three temp manifests (one with `hotReload: true`, one with `false`, one without the field). Verify exit codes 0 in all three cases and file content matches expected post-state. Document this in commit message body.
- After commit 2: `dotnet test --filter "FullyQualifiedName~ManifestRewriter|FullyQualifiedName~M74BuildPipeline"` — 9 new tests green. Full suite at 378/378 expected. Run integration tests **at least 3 consecutive times**. If any of the 3 runs fails, STOP, do not commit, escalate as v1.6 ratification candidate with `(observed iteration, exit code, stderr capture, environment notes)`.
- After commit 3: ROADMAP renders cleanly; cross-references resolve.

If during execution an architectural fork is encountered not foreseen here — STOP, ask, document choice. Per spec preamble "stop, escalate, lock — never guess".

**Hypothesis-falsification clause:**

Datapoints (per [M6 closure review §10](../audit/M6_CLOSURE_REVIEW.md)): M3=1, M4=1, M5=0, M6=0, M7.1=0, M7.2=0, M7.3=0. M7.4 closure pending = potentially eighth consecutive zero.

M7.4 exercises D-7 + §9.6 + §2.2 — narrow surface in v1.5. The implementation surface is build-system tooling (MSBuild target wiring, JSON manipulation, subprocess test infrastructure) — closer to .NET tooling contracts than to mod-OS spec wording. **If implementation surfaces a §9 / D-7 contradiction requiring v1.6 ratification → hypothesis falsified. Report immediately.**

Plausible v1.6 candidates worth flagging if encountered:

(a) **D-7 underspec on identification.** "Every vanilla manifest" doesn't define how vanilla mods are recognised by the build pipeline. We chose `<IsVanillaMod>` MSBuild opt-in (AD #2). If empirical reality demands id-prefix matching or a different mechanism, ratification candidate.

(b) **§2.2 hotReload absence semantics.** We treat absent field as no-op (AD #7). If the spec semantics actually expect the rewriter to add the field set to false when absent (so the shipped manifest unconditionally carries `hotReload: false`), ratification needed.

(c) **MSBuild path-resolution edge case.** `$(MSBuildThisFileDirectory)` or `$(OutDir)` may not resolve uniformly across `mods/` projects and `tests/Fixture.VanillaMod_*` projects (see "Path-resolution caveat" in §2). If empirically broken, may require fallback to absolute paths or restructuring of where the targets file lives.

(d) **Build-order issue**. `ReferenceOutputAssembly="false"` ProjectReference may not actually enforce build order in all .NET SDK versions. If integration test fails because rewriter dll isn't built when MSBuild target executes, this is an infrastructure issue requiring pattern revision (e.g. global pre-build target that builds the tool first).

(e) **Subprocess flakiness**. If `dotnet build` subprocess invoked from xUnit hangs, deadlocks, or hits I/O timeouts on shared CI runners, the integration test approach itself may need replacement with `Microsoft.Build.Execution.BuildManager` in-process API.

## Report-back format

- 3 commit SHAs (full hex).
- Final `dotnet test` count (369 + 9 = 378 expected, or actual with discrepancy noted).
- Per-test confirmation: 9 new tests all green by name (7 `ManifestRewriterTests` + 2 `M74BuildPipelineTests`).
- Regression confirmation: M7.1 (11) + M7.2 (13) + M7.3 (7) + remaining M0–M6 + Persistence + Systems + Core all still green.
- Working tree state: clean / dirty.
- **§11.4 stop-condition status**: zero flakes across **at least 3** consecutive integration test runs (or REPORT FLAKE IMMEDIATELY with iteration count + exit code + stderr capture + environment notes).
- **§9 / D-7 contradiction status**: zero (or REPORT IMMEDIATELY with section reference + proposed ratification candidate + which of the 5 plausible categories above it falls into).
- **Manual verification of rewriter on temp manifests**: confirmed for 3 cases (true → false, false → no-op, absent → no-op).
- **MSBuild path resolution**: verified in `mods/Directory.Build.targets` for both `mods/` projects (non-existent yet, simulated by manual ad-hoc test) and `tests/Fixture.VanillaMod_HotReloadOverride/` import.
- **M-phase boundary discipline**: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` = empty (verify and report).
- **Solution file**: `dotnet sln list` includes the 3 new projects.
- Any unexpected findings.
- **Special**: any §9 / D-7 spec contradiction discovered (would be ratification candidate for v1.6 — flag immediately with category from list above).
