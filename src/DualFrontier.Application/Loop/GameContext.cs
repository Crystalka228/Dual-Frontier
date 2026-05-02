using DualFrontier.Application.Modding;

namespace DualFrontier.Application.Loop;

/// <summary>
/// Aggregate handle returned by <see cref="GameBootstrap.CreateLoop"/>.
/// Carries the simulation loop and the mod-menu controller wired to the
/// same scheduler / services / pipeline state. The caller (GameRoot in
/// production, integration tests in DualFrontier.Modding.Tests) uses
/// <see cref="Loop"/> to start/stop the simulation and
/// <see cref="Controller"/> to drive the menu-side editing session.
///
/// Internal because it carries an internal <see cref="ModMenuController"/>
/// — a public record carrying an internal member would not compile (C#
/// accessibility rules). Reachable from DualFrontier.Modding.Tests and
/// DualFrontier.Presentation via the existing
/// <c>InternalsVisibleTo</c> declarations on the Application project.
/// </summary>
internal sealed record GameContext(GameLoop Loop, ModMenuController Controller);
