namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// UI-element update command keyed by widget identifier
/// (<paramref name="WidgetId"/>) and a serialised payload string
/// (<paramref name="Payload"/>).
/// </summary>
/// <param name="WidgetId">Name/identifier of the UI widget in Presentation.</param>
/// <param name="Payload">New value in serialised form.</param>
public sealed record UIUpdateCommand(string WidgetId, string Payload) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* TODO Phase 5 — apply via active IRenderer backend. */
    }
}
