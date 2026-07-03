using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Sharing;

/// <summary>
/// Meta (repo-discipline) guard — TESTING_STRATEGY §3.8. Asserts that every
/// fixture the Modding suite loads across the ALC boundary is staged under
/// <c>Fixtures/</c>, turning a missing-fixture deployment gap into ONE clear
/// failure instead of many cryptic assembly/manifest-load errors. Landed at the
/// F-10 isolation cascade alongside the DualFrontier.sln fixture-membership fix
/// (D2): if a fixture is not a solution member, a solution build stages it in the
/// wrong configuration and this test fails fast with the reason.
/// </summary>
public sealed class FixtureDeploymentTests
{
    // The fixtures Modding.Tests.csproj references (ReferenceOutputAssembly=false),
    // named by their DEPLOYED folder = each fixture's AssemblyName. Two differ from
    // the project name: Fixture.SharedEvents -> tests.shared.events, and
    // Fixture.BadSharedMod_WithIMod -> tests.bad-shared-imod (TESTING_STRATEGY §2.3).
    private static readonly string[] ExpectedFixtures =
    {
        "tests.shared.events", "Fixture.PublisherMod", "Fixture.SubscriberMod",
        "Fixture.BadRegularMod", "tests.bad-shared-imod",
        "Fixture.RegularMod_DependsOnAnother", "Fixture.RegularMod_DependedOn",
        "Fixture.RegularMod_CyclicA", "Fixture.RegularMod_CyclicB",
        "Fixture.RegularMod_MissingRequired", "Fixture.RegularMod_MissingOptional",
        "Fixture.RegularMod_BadApiVersion", "Fixture.RegularMod_DepsBadVersion",
        "Fixture.RegularMod_DependsOnBadApi", "Fixture.RegularMod_ReplacesCombat",
        "Fixture.RegularMod_ReplacesCombat_Alt", "Fixture.RegularMod_ReplacesProtected",
        "Fixture.RegularMod_ReplacesUnknown",
    };

    [Fact]
    public void AllReferencedFixtures_DeployedUnderFixturesRoot()
    {
        string root = Path.Combine(AppContext.BaseDirectory, "Fixtures");
        var missing = ExpectedFixtures
            .Where(n => !Directory.Exists(Path.Combine(root, n)))
            .ToList();

        missing.Should().BeEmpty(
            "every referenced fixture must be staged under Fixtures/. A missing fixture usually "
            + "means it is not a DualFrontier.sln member, so a solution build stages it in the wrong "
            + "configuration (TESTING_STRATEGY §2.3). Missing: " + string.Join(", ", missing));
    }
}
