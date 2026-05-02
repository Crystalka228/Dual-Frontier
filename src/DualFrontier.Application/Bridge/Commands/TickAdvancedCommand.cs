namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: the simulation tick counter has advanced to <see cref="Tick"/>.
/// <c>GameLoop</c> publishes one of these onto the <see cref="PresentationBridge"/>
/// after every successful <c>ParallelSystemScheduler.ExecuteTick</c> call inside
/// its fixed-step accumulator loop. The presentation layer drains the command
/// on the Godot main thread and forwards <see cref="Tick"/> to the HUD's tick
/// label via <c>RenderCommandDispatcher</c>.
/// </summary>
/// <param name="Tick">
/// Current value of <c>TickScheduler.CurrentTick</c> at publish time. Cast from
/// <c>long</c> to <c>int</c> at the publish site — UI labels accept <c>int</c>
/// and realistic sessions stay well below <c>int.MaxValue</c> (~800 days of
/// continuous play at 30 TPS).
/// </param>
public sealed record TickAdvancedCommand(int Tick) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* No-op — same Phase-5 IRenderer placeholder pattern as the other
         * commands. Actual routing happens through
         * RenderCommandDispatcher.Dispatch's switch arm. */
    }
}
