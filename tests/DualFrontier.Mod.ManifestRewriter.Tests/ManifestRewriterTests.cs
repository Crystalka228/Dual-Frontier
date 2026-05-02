using System;
using System.IO;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Mod.ManifestRewriter.Tests;

/// <summary>
/// M7.4 unit tests for the <see cref="ManifestRewriter"/> library entry
/// point. The CLI wrapper (<c>Program.Main</c>) is exercised
/// indirectly via the <c>M74BuildPipelineTests</c> integration suite,
/// which spawns <c>dotnet build</c> against the vanilla mod fixture.
///
/// Each test owns a temp directory cleaned up by the
/// <see cref="IDisposable"/> pattern; assertions cover the AD #7
/// idempotency contract (Rewritten / AlreadyFalse / FieldAbsent),
/// AD #8 failure semantics (NotFound, ParseError), and the source
/// preservation discipline (no fields silently added, all unrelated
/// fields preserved on rewrite).
/// </summary>
public sealed class ManifestRewriterTests : IDisposable
{
    private readonly string _tempDir;

    public ManifestRewriterTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "M74-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, recursive: true); }
        catch (IOException) { /* best-effort cleanup */ }
        catch (UnauthorizedAccessException) { /* best-effort cleanup */ }
    }

    [Fact]
    public void Rewrite_HotReloadTrue_FlipsToFalse_ReturnsRewritten()
    {
        string path = WriteManifest("""
            {
              "id": "tests.m74.flips",
              "name": "Flips Mod",
              "version": "1.0.0",
              "kind": "regular",
              "apiVersion": "^1.0.0",
              "hotReload": true
            }
            """);

        ManifestRewriter.Result result = ManifestRewriter.Rewrite(path);

        result.Should().Be(ManifestRewriter.Result.Rewritten);
        ReadHotReload(path).Should().BeFalse(
            "the rewriter must flip hotReload from true to false on disk");
        ReadString(path, "id").Should().Be("tests.m74.flips",
            "all unrelated fields must be preserved across the rewrite");
        ReadString(path, "name").Should().Be("Flips Mod");
        ReadString(path, "version").Should().Be("1.0.0");
    }

    [Fact]
    public void Rewrite_HotReloadFalse_NoOp_ReturnsAlreadyFalse()
    {
        string original = """
            {
              "id": "tests.m74.alreadyfalse",
              "name": "Already False",
              "version": "1.0.0",
              "hotReload": false
            }
            """;
        string path = WriteManifest(original);
        byte[] before = File.ReadAllBytes(path);

        ManifestRewriter.Result result = ManifestRewriter.Rewrite(path);

        result.Should().Be(ManifestRewriter.Result.AlreadyFalse);
        File.ReadAllBytes(path).Should().Equal(before,
            "AlreadyFalse must not rewrite the file (byte-identical content)");
    }

    [Fact]
    public void Rewrite_HotReloadAbsent_NoOp_ReturnsFieldAbsent()
    {
        string original = """
            {
              "id": "tests.m74.absent",
              "name": "Absent",
              "version": "1.0.0"
            }
            """;
        string path = WriteManifest(original);
        byte[] before = File.ReadAllBytes(path);

        ManifestRewriter.Result result = ManifestRewriter.Rewrite(path);

        result.Should().Be(ManifestRewriter.Result.FieldAbsent);
        File.ReadAllBytes(path).Should().Equal(before,
            "FieldAbsent must not rewrite the file — adding the field would "
            + "break round-trip equality for non-vanilla manifests that rely "
            + "on the §2.2 default (hotReload absent ⇒ false)");

        using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(path));
        doc.RootElement.TryGetProperty("hotReload", out _).Should().BeFalse(
            "the rewriter must NOT add the hotReload field when it was absent");
    }

    [Fact]
    public void Rewrite_PreservesAllOtherFields_AfterFlip()
    {
        string path = WriteManifest("""
            {
              "id": "tests.m74.preserves",
              "name": "Preserves",
              "version": "1.2.3",
              "author": "Test Suite",
              "kind": "regular",
              "apiVersion": "^1.0.0",
              "entryAssembly": "Preserves.dll",
              "entryType": "Preserves.Mod",
              "hotReload": true,
              "dependencies": [
                { "id": "dep.one", "version": "^1.0.0" },
                { "id": "dep.two", "version": "^2.0.0", "optional": true }
              ],
              "replaces": [
                "Some.Namespace.SystemA",
                "Some.Namespace.SystemB"
              ],
              "capabilities": {
                "required": [ "kernel.read:Foo.Bar" ],
                "provided": [ "mod.preserves.publish:Baz" ]
              }
            }
            """);

        ManifestRewriter.Rewrite(path).Should().Be(ManifestRewriter.Result.Rewritten);

        using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(path));
        JsonElement root = doc.RootElement;

        root.GetProperty("id").GetString().Should().Be("tests.m74.preserves");
        root.GetProperty("name").GetString().Should().Be("Preserves");
        root.GetProperty("version").GetString().Should().Be("1.2.3");
        root.GetProperty("author").GetString().Should().Be("Test Suite");
        root.GetProperty("kind").GetString().Should().Be("regular");
        root.GetProperty("apiVersion").GetString().Should().Be("^1.0.0");
        root.GetProperty("entryAssembly").GetString().Should().Be("Preserves.dll");
        root.GetProperty("entryType").GetString().Should().Be("Preserves.Mod");
        root.GetProperty("hotReload").GetBoolean().Should().BeFalse();

        JsonElement deps = root.GetProperty("dependencies");
        deps.GetArrayLength().Should().Be(2);
        deps[0].GetProperty("id").GetString().Should().Be("dep.one");
        deps[1].GetProperty("optional").GetBoolean().Should().BeTrue();

        JsonElement replaces = root.GetProperty("replaces");
        replaces.GetArrayLength().Should().Be(2);
        replaces[0].GetString().Should().Be("Some.Namespace.SystemA");

        JsonElement caps = root.GetProperty("capabilities");
        caps.GetProperty("required").GetArrayLength().Should().Be(1);
        caps.GetProperty("provided").GetArrayLength().Should().Be(1);
    }

    [Fact]
    public void Rewrite_NonexistentPath_ReturnsNotFound()
    {
        string path = Path.Combine(_tempDir, "does-not-exist.json");
        File.Exists(path).Should().BeFalse("precondition: path must not exist");

        ManifestRewriter.Result result = ManifestRewriter.Rewrite(path);

        result.Should().Be(ManifestRewriter.Result.NotFound);
        File.Exists(path).Should().BeFalse(
            "the rewriter must not create files on missing-path input");
    }

    [Fact]
    public void Rewrite_InvalidJson_ReturnsParseError_FileNotCorrupted()
    {
        string original = "{ this is not valid json }";
        string path = WriteRaw("bad.json", original);

        ManifestRewriter.Result result = ManifestRewriter.Rewrite(path);

        result.Should().Be(ManifestRewriter.Result.ParseError);
        File.ReadAllText(path).Should().Be(original,
            "ParseError must not corrupt or rewrite the original file");
    }

    [Fact]
    public void Rewrite_Idempotent_TwiceProducesSameOutput()
    {
        string path = WriteManifest("""
            {
              "id": "tests.m74.idempotent",
              "name": "Idempotent",
              "version": "1.0.0",
              "hotReload": true
            }
            """);

        ManifestRewriter.Result first = ManifestRewriter.Rewrite(path);
        first.Should().Be(ManifestRewriter.Result.Rewritten);
        byte[] afterFirst = File.ReadAllBytes(path);

        ManifestRewriter.Result second = ManifestRewriter.Rewrite(path);
        second.Should().Be(ManifestRewriter.Result.AlreadyFalse,
            "a second invocation on a flipped manifest must be a no-op");
        File.ReadAllBytes(path).Should().Equal(afterFirst,
            "idempotency: a second invocation must not re-touch the file");
    }

    private string WriteManifest(string json)
    {
        return WriteRaw("mod.manifest.json", json);
    }

    private string WriteRaw(string fileName, string content)
    {
        string path = Path.Combine(_tempDir, fileName);
        File.WriteAllText(path, content);
        return path;
    }

    private static bool ReadHotReload(string path)
    {
        using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(path));
        return doc.RootElement.GetProperty("hotReload").GetBoolean();
    }

    private static string ReadString(string path, string key)
    {
        using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(path));
        return doc.RootElement.GetProperty(key).GetString() ?? string.Empty;
    }
}
