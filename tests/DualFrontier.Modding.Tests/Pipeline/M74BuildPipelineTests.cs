using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// M7.4 integration tests for the D-7 build-pipeline override
/// (MOD_OS_ARCHITECTURE v1.5). The two tests spawn `dotnet build`
/// subprocesses against the
/// <c>Fixture.VanillaMod_HotReloadOverride</c> project and assert the
/// rewriter's source-preservation contract end-to-end:
/// <list type="number">
///   <item><c>BuildRelease_VanillaModFixture_RewritesOutputManifest_PreservesSource</c>
///   — Release build flips bin/Release/net8.0/mod.manifest.json from
///   <c>hotReload: true</c> to <c>hotReload: false</c> while leaving
///   the source manifest at the project root unchanged.</item>
///   <item><c>BuildDebug_VanillaModFixture_DoesNotRewriteOutputManifest</c>
///   — Debug build leaves both source and bin manifests at
///   <c>hotReload: true</c> (the rewriter target is configuration-
///   conditional; the CopyToOutputDirectory ItemGroup is gated on
///   <c>IsVanillaMod</c> only, so the Debug bin still contains a
///   manifest copy — but that copy is never rewritten).</item>
/// </list>
/// Marked <c>[Trait("Category", "Integration")]</c> so the suite can
/// be filtered when running fast unit-only loops; remains in the
/// default <c>dotnet test</c> run per the M7.4 acceptance criterion.
/// Per §11.4: must run zero-flake across at least 3 consecutive
/// runs before commit.
/// </summary>
public sealed class M74BuildPipelineTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public void BuildRelease_VanillaModFixture_RewritesOutputManifest_PreservesSource()
    {
        FixturePaths paths = FixturePaths.Resolve();
        string sourceBefore = File.ReadAllText(paths.SourceManifest);

        BuildResult build = RunDotnetBuild(paths.FixtureCsproj, configuration: "Release");

        build.ExitCode.Should().Be(
            0,
            $"`dotnet build -c Release` against the vanilla-mod fixture must succeed.\n"
            + $"stdout:\n{build.Stdout}\nstderr:\n{build.Stderr}");

        File.ReadAllText(paths.SourceManifest).Should().Be(
            sourceBefore,
            "the rewriter must NEVER touch the source manifest — D-7 requires "
            + "git diff between dev and shipped builds to be empty for source");
        ReadHotReload(paths.SourceManifest).Should().BeTrue(
            "the source manifest's hotReload must remain true so dev still gets "
            + "free hot-reload testing of vanilla mods");

        string outputManifest = paths.ReleaseOutputManifest;
        File.Exists(outputManifest).Should().BeTrue(
            $"the Release build must deploy mod.manifest.json into bin: {outputManifest}");
        ReadHotReload(outputManifest).Should().BeFalse(
            "the build-pipeline override (mods/Directory.Build.targets) must "
            + "rewrite the bin manifest's hotReload to false on Release builds");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void BuildDebug_VanillaModFixture_DoesNotRewriteOutputManifest()
    {
        FixturePaths paths = FixturePaths.Resolve();
        string sourceBefore = File.ReadAllText(paths.SourceManifest);

        BuildResult build = RunDotnetBuild(paths.FixtureCsproj, configuration: "Debug");

        build.ExitCode.Should().Be(
            0,
            $"`dotnet build -c Debug` against the vanilla-mod fixture must succeed.\n"
            + $"stdout:\n{build.Stdout}\nstderr:\n{build.Stderr}");

        File.ReadAllText(paths.SourceManifest).Should().Be(
            sourceBefore,
            "the source manifest must remain unchanged regardless of configuration");
        ReadHotReload(paths.SourceManifest).Should().BeTrue();

        string outputManifest = paths.DebugOutputManifest;
        File.Exists(outputManifest).Should().BeTrue(
            $"the Debug build must still deploy mod.manifest.json into bin (the "
            + $"CopyToOutputDirectory ItemGroup is gated on IsVanillaMod, not on "
            + $"Configuration); only the rewriter target is configuration-"
            + $"conditional: {outputManifest}");
        ReadHotReload(outputManifest).Should().BeTrue(
            "the rewriter target is gated on $(Configuration)=='Release', so a "
            + "Debug build must leave the bin manifest at hotReload: true to "
            + "preserve dev-time hot-reload testing");
    }

    private static bool ReadHotReload(string path)
    {
        using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(path));
        return doc.RootElement.GetProperty("hotReload").GetBoolean();
    }

    private static BuildResult RunDotnetBuild(string csproj, string configuration)
    {
        ProcessStartInfo info = new()
        {
            FileName = "dotnet",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        info.ArgumentList.Add("build");
        info.ArgumentList.Add("-c");
        info.ArgumentList.Add(configuration);
        info.ArgumentList.Add("--nologo");
        info.ArgumentList.Add(csproj);

        StringBuilder stdout = new();
        StringBuilder stderr = new();
        using Process process = new() { StartInfo = info };
        process.OutputDataReceived += (_, e) => { if (e.Data is not null) stdout.AppendLine(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data is not null) stderr.AppendLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        if (!process.WaitForExit(milliseconds: 180_000))
        {
            try { process.Kill(entireProcessTree: true); } catch { /* best-effort */ }
            throw new InvalidOperationException(
                $"`dotnet build -c {configuration} {csproj}` did not exit within 180s; "
                + $"this is a §11.4 stop-condition signal — escalate as v1.6 ratification "
                + $"candidate (subprocess timing or MSBuild concurrency).\n"
                + $"stdout so far:\n{stdout}\nstderr so far:\n{stderr}");
        }

        process.WaitForExit();

        return new BuildResult(process.ExitCode, stdout.ToString(), stderr.ToString());
    }

    private readonly record struct BuildResult(int ExitCode, string Stdout, string Stderr);

    private readonly record struct FixturePaths(
        string FixtureCsproj,
        string SourceManifest,
        string DebugOutputManifest,
        string ReleaseOutputManifest)
    {
        public static FixturePaths Resolve()
        {
            string repoRoot = FindRepoRoot();
            string fixtureDir = Path.Combine(
                repoRoot, "tests", "Fixture.VanillaMod_HotReloadOverride");
            return new FixturePaths(
                FixtureCsproj: Path.Combine(fixtureDir, "Fixture.VanillaMod_HotReloadOverride.csproj"),
                SourceManifest: Path.Combine(fixtureDir, "mod.manifest.json"),
                DebugOutputManifest: Path.Combine(fixtureDir, "bin", "Debug", "net8.0", "mod.manifest.json"),
                ReleaseOutputManifest: Path.Combine(fixtureDir, "bin", "Release", "net8.0", "mod.manifest.json"));
        }

        private static string FindRepoRoot()
        {
            // Walks up from the test assembly's runtime directory until
            // a directory containing DualFrontier.sln is found. The
            // sentinel file pins the repo root unambiguously regardless
            // of where bin/ is configured.
            DirectoryInfo? dir = new(AppContext.BaseDirectory);
            while (dir is not null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "DualFrontier.sln")))
                    return dir.FullName;
                dir = dir.Parent;
            }
            throw new InvalidOperationException(
                $"Could not locate DualFrontier.sln walking up from {AppContext.BaseDirectory}");
        }
    }
}
