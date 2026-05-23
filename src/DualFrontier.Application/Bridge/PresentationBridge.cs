using System;
using System.Collections.Concurrent;

namespace DualFrontier.Application.Bridge;

/// <summary>
/// Domain → Presentation bridge. The domain enqueues
/// <see cref="IRenderCommand"/> instances from any thread; the main thread of
/// the active <see cref="Rendering.IRenderer"/> drains them via
/// <see cref="DrainCommands"/>. The link is strictly one-way (TechArch 11.9).
/// К-extensions cascade #2 (2026-05-23) deprecated Godot + Silk.NET paths;
/// current single backend = Launcher's <c>LauncherRenderer</c> +
/// <c>RenderCommandDispatcher</c>.
/// </summary>
public sealed class PresentationBridge
{
    /// <summary>
    /// Internal queue of render commands. <see cref="ConcurrentQueue{T}"/> is
    /// the thread-safe container for the "many writers, one reader" scenario.
    /// </summary>
    private readonly ConcurrentQueue<IRenderCommand> _commands = new();

    /// <summary>
    /// Appends a command к the queue. Called from any domain thread.
    /// </summary>
    /// <param name="cmd">Render command к apply on the main thread.</param>
    public void Enqueue(IRenderCommand cmd)
    {
        if (cmd is null) throw new ArgumentNullException(nameof(cmd));
        _commands.Enqueue(cmd);
    }

    /// <summary>
    /// Dequeues and dispatches every accumulated command via
    /// <paramref name="execute"/>. Called ONLY from the main thread of the
    /// active render backend (Launcher's per-frame iteration). К-extensions
    /// cascade #2 (Q-G-3): <see cref="IRenderCommand"/> is а pure marker —
    /// dispatch handled by the renderer's dispatcher, не by а per-command
    /// <c>Execute()</c> method.
    /// </summary>
    /// <param name="execute">
    /// Delegate that dispatches the command (typically the renderer's pattern-
    /// matching dispatcher, e.g. <c>RenderCommandDispatcher.Dispatch</c>).
    /// </param>
    public void DrainCommands(Action<IRenderCommand> execute)
    {
        if (execute is null) throw new ArgumentNullException(nameof(execute));
        while (_commands.TryDequeue(out var cmd))
        {
            execute(cmd);
        }
    }

    /// <summary>
    /// Diagnostic surface — current queue depth (number of commands pending
    /// drainage). Read-only. Used by DebugOverlay to surface any backpressure
    /// between simulation production rate and render drainage rate.
    /// <see cref="ConcurrentQueue{T}.Count"/> is thread-safe but eventually
    /// consistent (may show stale value briefly). Acceptable for diagnostic
    /// display.
    /// </summary>
    public int QueueDepth => _commands.Count;
}
