using System;
using System.Collections.Concurrent;

namespace DualFrontier.Application.Bridge;

/// <summary>
/// Мост Domain → Presentation. Домен из любого потока складывает
/// <see cref="IRenderCommand"/> в очередь, главный поток Godot читает их
/// через <see cref="DrainCommands"/> в своём <c>_Process</c>.
/// Связь строго однонаправленная (TechArch 11.9).
/// </summary>
public sealed class PresentationBridge
{
    /// <summary>
    /// Внутренняя очередь команд рендера. <see cref="ConcurrentQueue{T}"/> —
    /// потокобезопасный контейнер для сценария «много писателей — один читатель».
    /// </summary>
    private readonly ConcurrentQueue<IRenderCommand> _commands = new();

    /// <summary>
    /// TODO: Фаза 3 — добавляет команду в очередь. Вызывается из любого
    /// потока домена.
    /// </summary>
    /// <param name="cmd">Команда рендера, которую нужно применить на главном потоке.</param>
    public void Enqueue(IRenderCommand cmd)
    {
        if (cmd is null) throw new ArgumentNullException(nameof(cmd));
        _commands.Enqueue(cmd);
    }

    /// <summary>
    /// TODO: Фаза 3 — извлекает и выполняет все накопленные команды.
    /// Вызывается ТОЛЬКО из главного потока Godot (в <c>_Process</c>).
    /// </summary>
    /// <param name="execute">
    /// Делегат, выполняющий команду. Обычно <c>cmd =&gt; cmd.Execute(godotScene)</c>.
    /// </param>
    public void DrainCommands(Action<IRenderCommand> execute)
    {
        if (execute is null) throw new ArgumentNullException(nameof(execute));
        while (_commands.TryDequeue(out var cmd))
        {
            execute(cmd);
        }
    }
}
