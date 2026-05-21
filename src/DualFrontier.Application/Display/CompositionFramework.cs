using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Display;

namespace DualFrontier.Application.Display;

/// <summary>
/// К-L17 display composition framework (К10.3 v2 Item 38).
///
/// Layer registry с deterministic composition order: primary by
/// <see cref="LayerType"/> (SimState → Intent → CombatFeedback → Static),
/// secondary by <see cref="Layer.CompositionOrder"/>, tertiary by
/// <see cref="Layer.Fqn"/> for stable tie-breaking.
///
/// Per S-LOCK-11: framework lives в <c>DualFrontier.Application.Display</c>
/// above the renderer abstraction. Concrete backends invoke
/// <see cref="RenderFrame"/> from their <see cref="Rendering.IRenderer.RenderFrame"/>
/// implementation, passing their substrate-specific render context.
///
/// К10.3 v2 К-L17 boundary: framework establishes layer registration и
/// composition order. Mod-registered layers (К10.3 v2 Items 39+40) plug in
/// via <see cref="RegisterLayer"/>; vanilla SimState layer wraps existing
/// V0.C.2 batched sprite path per К-L9 «Vanilla = mods» uniformity.
/// </summary>
public sealed class CompositionFramework
{
    private readonly Dictionary<string, Layer> _layers = new(StringComparer.Ordinal);
    private List<Layer>? _orderedCache;

    /// <summary>Current registered layer count.</summary>
    public int LayerCount => _layers.Count;

    /// <summary>
    /// Registers <paramref name="layer"/>. Throws if a layer with the same
    /// <see cref="Layer.Fqn"/> is already registered — К-L17 composition
    /// order requires unique identity per layer.
    /// </summary>
    public void RegisterLayer(Layer layer)
    {
        if (layer is null) throw new ArgumentNullException(nameof(layer));
        if (string.IsNullOrWhiteSpace(layer.Fqn))
        {
            throw new ArgumentException(
                "Layer.Fqn must be non-empty for capability tokens + dictionary keying.",
                nameof(layer));
        }
        if (_layers.ContainsKey(layer.Fqn))
        {
            throw new InvalidOperationException(
                $"Layer with FQN '{layer.Fqn}' is already registered. " +
                "Unregister first or use a distinct FQN.");
        }

        _layers.Add(layer.Fqn, layer);
        _orderedCache = null;
    }

    /// <summary>
    /// Unregisters the layer с the given <paramref name="fqn"/>. Returns
    /// <see langword="true"/> if a layer was removed; <see langword="false"/>
    /// if no layer был registered с that FQN. Invokes
    /// <see cref="Layer.OnUnregistered"/> on removal.
    /// </summary>
    public bool UnregisterLayer(string fqn)
    {
        if (fqn is null) throw new ArgumentNullException(nameof(fqn));
        if (!_layers.TryGetValue(fqn, out Layer? layer)) return false;

        _layers.Remove(fqn);
        _orderedCache = null;
        layer.OnUnregistered();
        return true;
    }

    /// <summary>
    /// Returns the layer registered под <paramref name="fqn"/>, либо
    /// <see langword="null"/> if absent.
    /// </summary>
    public Layer? GetLayer(string fqn)
    {
        if (fqn is null) throw new ArgumentNullException(nameof(fqn));
        return _layers.TryGetValue(fqn, out Layer? layer) ? layer : null;
    }

    /// <summary>
    /// Returns registered layers в К-L17 composition order:
    /// <see cref="LayerType"/> ascending, then <see cref="Layer.CompositionOrder"/>
    /// ascending, then <see cref="Layer.Fqn"/> ordinal for stable tie-break.
    /// Cached between mutations.
    /// </summary>
    public IReadOnlyList<Layer> GetLayersInCompositionOrder()
    {
        if (_orderedCache is not null) return _orderedCache;

        var ordered = new List<Layer>(_layers.Count);
        foreach (Layer layer in _layers.Values)
        {
            ordered.Add(layer);
        }
        ordered.Sort(CompareLayers);
        _orderedCache = ordered;
        return ordered;
    }

    /// <summary>
    /// Renders all registered layers в composition order. К-L17 invariant:
    /// SimState layers first, intent + combat feedback overlays composited
    /// on top, static layers last.
    /// </summary>
    public void RenderFrame(ILayerRenderContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        IReadOnlyList<Layer> ordered = GetLayersInCompositionOrder();
        for (int i = 0; i < ordered.Count; i++)
        {
            ordered[i].Render(context);
        }
    }

    private static int CompareLayers(Layer a, Layer b)
    {
        int byType = ((int)a.Type).CompareTo((int)b.Type);
        if (byType != 0) return byType;

        int byOrder = a.CompositionOrder.CompareTo(b.CompositionOrder);
        if (byOrder != 0) return byOrder;

        return string.CompareOrdinal(a.Fqn, b.Fqn);
    }
}
