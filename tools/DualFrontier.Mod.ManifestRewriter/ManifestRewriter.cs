using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DualFrontier.Mod.ManifestRewriter;

/// <summary>
/// Build-pipeline rewriter that flips <c>hotReload: true</c> to
/// <c>hotReload: false</c> in a mod manifest, per
/// <see href="../docs/MOD_OS_ARCHITECTURE.md">MOD_OS_ARCHITECTURE</see>
/// v1.5 D-7 (LOCKED): vanilla mods declare <c>hotReload: true</c> in
/// source so development gets free hot-reload testing of vanilla
/// mechanics; the shipping build pipeline rewrites this to
/// <c>hotReload: false</c> in release manifests so shipped builds get
/// stable session experience.
/// </summary>
/// <remarks>
/// <para><b>Source preservation contract.</b> The rewriter operates on
/// the manifest copy in <c>bin/{Configuration}/{TFM}/</c> after
/// MSBuild's <c>CopyToOutputDirectory</c> has run. The source manifest
/// at the project root is never modified, so <c>git diff</c> between
/// dev and shipped builds is empty for the source tree. The MSBuild
/// target (<c>mods/Directory.Build.targets</c>) is responsible for
/// gating invocation by <c>$(IsVanillaMod)=='true'</c> and
/// <c>$(Configuration)=='Release'</c>; the rewriter operates on any
/// manifest path it is invoked with and applies no filtering of its
/// own.</para>
///
/// <para><b>Idempotency contract.</b> A rewriter run on a manifest
/// already at <c>hotReload: false</c> is a no-op
/// (<see cref="Result.AlreadyFalse"/>, exit 0). A manifest with no
/// <c>hotReload</c> field is also a no-op
/// (<see cref="Result.FieldAbsent"/>, exit 0): adding the field would
/// silently change the document shape and break round-trip equality
/// for non-vanilla cases that rely on the §2.2 default
/// (<c>hotReload</c> absent ⇒ <c>false</c>). Only manifests with
/// <c>hotReload: true</c> are rewritten.</para>
///
/// <para><b>Library entry point shape.</b> Tests invoke
/// <see cref="Rewrite(string)"/> directly (in-process, fast).
/// <c>Program.Main</c> is a thin CLI wrapper that maps the
/// <see cref="Result"/> to a process exit code and writes a single
/// diagnostic line to <c>stderr</c> on failure.</para>
/// </remarks>
public static class ManifestRewriter
{
    /// <summary>
    /// Outcome of <see cref="Rewrite(string)"/>. Success values
    /// (<see cref="Rewritten"/>, <see cref="AlreadyFalse"/>,
    /// <see cref="FieldAbsent"/>) map to exit code 0; failure values
    /// each map to a distinct non-zero exit code per the CLI contract.
    /// </summary>
    public enum Result
    {
        /// <summary>Manifest had <c>hotReload: true</c>; flipped to <c>false</c>.</summary>
        Rewritten,

        /// <summary>Manifest already at <c>hotReload: false</c>; file unchanged.</summary>
        AlreadyFalse,

        /// <summary>Manifest has no <c>hotReload</c> field; file unchanged (§2.2 default applies).</summary>
        FieldAbsent,

        /// <summary>The path does not exist or could not be opened for reading.</summary>
        NotFound,

        /// <summary>The file is not valid JSON or its root is not an object.</summary>
        ParseError,

        /// <summary>The flipped manifest could not be written back to disk.</summary>
        WriteError,
    }

    /// <summary>
    /// Reads the manifest at <paramref name="manifestPath"/>, and if
    /// it carries <c>hotReload: true</c>, writes it back with that
    /// field set to <c>false</c>. All other fields and the document's
    /// indented formatting are preserved.
    /// </summary>
    public static Result Rewrite(string manifestPath)
    {
        if (!File.Exists(manifestPath)) return Result.NotFound;

        string text;
        try
        {
            text = File.ReadAllText(manifestPath);
        }
        catch (IOException) { return Result.NotFound; }
        catch (UnauthorizedAccessException) { return Result.NotFound; }

        JsonNode? node;
        try
        {
            node = JsonNode.Parse(text);
        }
        catch (JsonException) { return Result.ParseError; }

        if (node is not JsonObject obj) return Result.ParseError;

        if (!obj.ContainsKey("hotReload")) return Result.FieldAbsent;

        JsonNode? value = obj["hotReload"];
        bool current;
        try
        {
            current = value is not null && value.GetValue<bool>();
        }
        catch (InvalidOperationException) { return Result.ParseError; }
        catch (FormatException) { return Result.ParseError; }

        if (!current) return Result.AlreadyFalse;

        obj["hotReload"] = false;

        try
        {
            JsonSerializerOptions options = new() { WriteIndented = true };
            File.WriteAllText(manifestPath, obj.ToJsonString(options));
        }
        catch (IOException) { return Result.WriteError; }
        catch (UnauthorizedAccessException) { return Result.WriteError; }

        return Result.Rewritten;
    }
}
