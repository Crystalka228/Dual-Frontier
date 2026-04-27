using System;
using System.Collections.Concurrent;

namespace DualFrontier.Application.Bridge;

/// <summary>
/// Domain → Presentation bridge. The domain enqueues
/// <see cref="IRenderCommand"/> instances from any thread; the main thread of
/// the active <see cref="Rendering.IRenderer"/> (Godot or Native) drains them
/// via <see cref="DrainCommands"/>. The link is strictly one-way (TechArch 11.9).
/// </summary>
public sealed class PresentationBridge
{
    /// <summary>
    /// Internal queue of render commands. <see cref="ConcurrentQueue{T}"/> is
    /// the thread-safe container for the "many writers, one reader" scenario.
    /// </summary>
    private readonly ConcurrentQueue<IRenderCommand> _commands = new();

    /// <summary>
    /// TODO: Phase 3 — appends a command to the queue. Called from any
    /// domain thread.
    /// </summary>
    /// <param name="cmd">Render command to apply on the main thread.</param>
    public void Enqueue(IRenderCommand cmd)
    {
        if (cmd is null) throw new ArgumentNullException(nameof(cmd));
        _commands.Enqueue(cmd);
    }

    /// <summary>
    /// TODO: Phase 3 — dequeues and executes every accumulated command.
    /// Called ONLY from the main thread of the active render backend.
    /// </summary>
    /// <param name="execute">
    /// Delegate that executes the command. Typically <c>cmd =&gt; cmd.Execute(renderContext)</c>.
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
