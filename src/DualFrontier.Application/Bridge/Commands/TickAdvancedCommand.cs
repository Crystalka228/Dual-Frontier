namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: the simulation tick counter has advanced к <see cref="Tick"/>.
/// <c>GameLoop</c> publishes one of these onto the <see cref="PresentationBridge"/>
/// after every successful <c>ParallelSystemScheduler.ExecuteTick</c> call inside
/// its fixed-step accumulator loop. The presentation layer (Launcher's
/// <c>RenderCommandDispatcher</c>) drains the command on the main render thread;
/// the <c>HandleTickAdvanced</c> arm is currently a reserved silent stub —
/// HUD tick-label consumption lands with HUD primitives. Per К-extensions
/// cascade #2 (2026-05-23): dispatch handled centrally by Launcher.
/// </summary>
/// <param name="Tick">
/// Current value of <c>TickScheduler.CurrentTick</c> at publish time. Cast от
/// <c>long</c> к <c>int</c> at the publish site — UI labels accept <c>int</c>
/// and realistic sessions stay well below <c>int.MaxValue</c> (~800 days of
/// continuous play at 30 TPS).
/// </param>
public sealed record TickAdvancedCommand(int Tick) : IRenderCommand;
