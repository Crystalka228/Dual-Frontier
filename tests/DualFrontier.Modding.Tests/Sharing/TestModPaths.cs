using System;
using System.IO;

namespace DualFrontier.Modding.Tests.Sharing;

/// <summary>
/// Resolves the on-disk paths to the cross-ALC test fixture mods. Each
/// fixture project deploys its built assembly together with its
/// <c>mod.manifest.json</c> into <c>Fixtures/&lt;assemblyName&gt;/</c> under
/// the test project's output directory; this helper hides that layout from
/// the tests so they can request fixtures by purpose, not by path.
/// </summary>
internal static class TestModPaths
{
    private static readonly string FixturesRoot =
        Path.Combine(AppContext.BaseDirectory, "Fixtures");

    /// <summary>Path to the shared mod fixture defining <c>SharedTestEvent</c>.</summary>
    public static string SharedEvents => Path.Combine(FixturesRoot, "tests.shared.events");

    /// <summary>Path to the regular mod fixture that publishes <c>SharedTestEvent</c>.</summary>
    public static string PublisherMod => Path.Combine(FixturesRoot, "Fixture.PublisherMod");

    /// <summary>Path to the regular mod fixture that subscribes to <c>SharedTestEvent</c>.</summary>
    public static string SubscriberMod => Path.Combine(FixturesRoot, "Fixture.SubscriberMod");

    /// <summary>
    /// Path to the regular mod fixture that exports <c>IEvent</c> and
    /// <c>IModContract</c> types — the precise §6.5 D-4 violation Phase E
    /// catches. Used as the negative case in
    /// <c>ContractTypeInRegularModTests</c>.
    /// </summary>
    public static string BadRegularMod => Path.Combine(FixturesRoot, "Fixture.BadRegularMod");

    /// <summary>
    /// Path to the shared mod fixture whose assembly contains an
    /// <c>IMod</c> implementation — the precise §5.2 violation Phase F
    /// catches. The deployed folder name matches the assembly's
    /// <c>AssemblyName</c> (which equals the manifest id so the loader's
    /// default <c>{id}.dll</c> lookup works without an
    /// <c>entryAssembly</c> field).
    /// </summary>
    public static string BadSharedModWithIMod => Path.Combine(FixturesRoot, "tests.bad-shared-imod");
}
