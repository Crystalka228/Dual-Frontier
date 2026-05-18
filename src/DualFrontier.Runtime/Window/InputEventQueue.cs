using System.Collections.Concurrent;
using DualFrontier.Runtime.Input;

namespace DualFrontier.Runtime.Window;

/// <summary>
/// Cross-thread input event channel. Window thread enqueues (V0.C), simulation thread dequeues.
/// V0.A: placeholder only — Window class WindowProcedure does not yet enqueue events; concrete
/// enqueue logic lands V0.C alongside WM_KEYDOWN/WM_MOUSEMOVE/etc. dispatch.
/// </summary>
public sealed class InputEventQueue
{
    private readonly ConcurrentQueue<IInputEvent> _events = new();

    public void Enqueue(IInputEvent evt) => _events.Enqueue(evt);

    public bool TryDequeue(out IInputEvent? evt) => _events.TryDequeue(out evt);

    public int Count => _events.Count;
}
