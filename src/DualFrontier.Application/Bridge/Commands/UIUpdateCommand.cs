namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Команда обновления UI-элемента по идентификатору виджета
/// (<paramref name="WidgetId"/>) и строковому сериализованному
/// значению (<paramref name="Payload"/>).
/// </summary>
/// <param name="WidgetId">Имя/идентификатор UI-виджета в Presentation.</param>
/// <param name="Payload">Новое значение в сериализованном виде.</param>
public sealed record UIUpdateCommand(string WidgetId, string Payload) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* TODO Фаза 5 — apply via active IRenderer backend. */
    }
}
