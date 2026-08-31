using System.Xml.Linq;
using AwesomeAssertions;
using DualFrontier.Governance;
using Xunit;

namespace DualFrontier.Governance.Tests;

/// <summary>
/// Engine-to-game reference RATCHET -- the interim mechanical enforcer of boundary law B-1
/// (docs/architecture/GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md), standing in until the B-6
/// boundary analyzer ships (ROADMAP "Vanilla-separation track").
///
/// This is a transitional structure and carries the EXECUTION_AUTHORITY_MATRIX.md section 3.0
/// three-element contract:
///
///  1. NAMED GATE -- the set of engine-to-game <c>ProjectReference</c> edges and engine-to-game
///     <c>InternalsVisibleTo</c> grants, measured from the six engine csprojs, must EQUAL the
///     current baseline: NO ProjectReference edges (W4 removed the last four, all of them
///     DualFrontier.Application's) and the single Core -> Systems IVT that audit R3 froze.
///     Independently falsifiable: a stray engine-to-game edge makes the measured set unequal and
///     reddens the test.
///  2. EQUIVALENCE EVIDENCE -- the red-once-then-green proof recorded in this test's introducing
///     commit body: a transient engine-to-game ProjectReference reddened it; reverting greened it.
///  3. DELETION TRIGGER -- superseded by the B-6 boundary analyzer rule when one ships. This class is
///     DELETED in the same cascade that lands that analyzer: it pins a boundary the analyzer will then
///     own, and keeping both is a zombie oracle (EAM section 3.1).
///
/// The migration drives the engine-to-game edges toward zero, so the baseline constants below may only
/// SHRINK. Shrinkage = update the constant + a census-delta note in the removing commit's body.
/// Growth is forbidden -- a new engine-to-game reference is a boundary-law B-1 violation.
/// </summary>
public sealed class BoundaryRatchetTests
{
    // Boundary law B-1 engine set: assemblies that must NOT reference a game assembly.
    private static readonly string[] EngineAssemblies =
    {
        "DualFrontier.Contracts",
        "DualFrontier.Core",
        "DualFrontier.Core.Interop",
        "DualFrontier.Runtime",
        "DualFrontier.Application",
        "DualFrontier.Launcher",
    };

    // Audit F2 game set: the vanilla Domain-layer assemblies the migration will dissolve. Exact-name
    // membership (ordinal) -- so "DualFrontier.Systems.Tests" never matches "DualFrontier.Systems".
    private static readonly HashSet<string> GameAssemblies = new(StringComparer.Ordinal)
    {
        "DualFrontier.Components",
        "DualFrontier.Events",
        "DualFrontier.Systems",
        "DualFrontier.AI",
    };

    // Frozen at HEAD 4c58942 (audit R3) with four DualFrontier.Application edges; EMPTIED by W4,
    // which dissolved the composition root that was their only reason to exist. Edge string form:
    // "<engine> -> <game>".
    //
    // An empty baseline is the migration's terminal state for this half of the ratchet, not a
    // disabled test: the measurement still runs over all six engine csprojs, and any engine->game
    // ProjectReference that reappears makes the measured set non-empty and reddens the test. The
    // deletion trigger in the class summary is unchanged -- the B-6 analyzer supersedes this class
    // whole, including the IVT half below, which is still non-empty.
    private static readonly HashSet<string> ProjectReferenceBaseline = new(StringComparer.Ordinal);

    private static readonly HashSet<string> InternalsVisibleToBaseline = new(StringComparer.Ordinal)
    {
        "DualFrontier.Core -> DualFrontier.Systems",
    };

    [Fact]
    public void EngineToGame_ProjectReferenceEdges_EqualFrozenBaseline()
    {
        HashSet<string> measured = MeasureEngineToGameEdges(
            csproj => References(csproj, "ProjectReference").Select(ProjectFileStem));

        measured.Should().BeEquivalentTo(ProjectReferenceBaseline,
            "boundary law B-1 (docs/architecture/GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md): no engine " +
            "assembly may ProjectReference a game assembly beyond the frozen migration baseline. A new " +
            "edge is forbidden; a removed edge means the migration shrank the boundary -- update " +
            "ProjectReferenceBaseline and record the census delta in the commit body.");
    }

    [Fact]
    public void EngineToGame_InternalsVisibleTo_EqualFrozenBaseline()
    {
        HashSet<string> measured = MeasureEngineToGameEdges(
            csproj => References(csproj, "InternalsVisibleTo"));

        measured.Should().BeEquivalentTo(InternalsVisibleToBaseline,
            "boundary law B-1: the only sanctioned engine->game InternalsVisibleTo grant is " +
            "DualFrontier.Core -> DualFrontier.Systems (already flagged removable). A new engine->game " +
            "grant is forbidden; a removed one shrinks the boundary -- update InternalsVisibleToBaseline " +
            "and record the census delta in the commit body.");
    }

    private static HashSet<string> MeasureEngineToGameEdges(Func<XDocument, IEnumerable<string>> targets)
    {
        string srcRoot = Path.Combine(RepoPaths.RepoRoot(), "src");
        var edges = new HashSet<string>(StringComparer.Ordinal);

        foreach (string engine in EngineAssemblies)
        {
            string csprojPath = Path.Combine(srcRoot, engine, engine + ".csproj");
            File.Exists(csprojPath).Should().BeTrue($"engine project {engine} must exist under src/");

            XDocument doc = XDocument.Load(csprojPath);
            foreach (string target in targets(doc))
            {
                if (GameAssemblies.Contains(target))
                {
                    edges.Add($"{engine} -> {target}");
                }
            }
        }

        return edges;
    }

    // SDK-style csprojs carry no XML namespace, so match items by local name.
    private static IEnumerable<string> References(XDocument csproj, string itemName) =>
        csproj.Descendants()
            .Where(e => e.Name.LocalName == itemName)
            .Select(e => (string?)e.Attribute("Include"))
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!.Trim());

    // "..\DualFrontier.Components\DualFrontier.Components.csproj" -> "DualFrontier.Components".
    private static string ProjectFileStem(string include) =>
        Path.GetFileNameWithoutExtension(include.Replace('\\', '/'));
}
