using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Interop;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Capability-enforcing wrapper over <see cref="FieldRegistry"/> handed
/// to a single mod through <see cref="IModApi.Fields"/>. Each
/// <see cref="RegisterField{T}"/> and <see cref="GetField{T}"/> call is
/// gated against the mod's manifest capabilities per
/// <c>MOD_OS_ARCHITECTURE.md</c> v1.7 §3.2 / §4.6.
/// </summary>
/// <remarks>
/// Per-cell <c>ReadCell</c> / <c>WriteCell</c> traffic through the returned
/// <c>FieldHandle&lt;T&gt;</c> is NOT re-checked — the handle is already
/// capability-gated at acquisition, mirroring how
/// <see cref="RestrictedModApi"/> gates event traffic at the
/// publish/subscribe boundary rather than per-event.
/// </remarks>
internal sealed class RestrictedFieldApi : IModFieldApi
{
    private readonly FieldRegistry _registry;
    private readonly string _modId;
    private readonly IReadOnlySet<string> _modCapabilities;

    internal RestrictedFieldApi(FieldRegistry registry, string modId, IReadOnlySet<string> capabilities)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _modId = modId ?? throw new ArgumentNullException(nameof(modId));
        _modCapabilities = capabilities ?? throw new ArgumentNullException(nameof(capabilities));
    }

    public IFieldHandle RegisterField<T>(string id, int width, int height) where T : unmanaged
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Field id must be non-empty", nameof(id));

        string expectedPrefix = _modId + ".";
        if (!id.StartsWith(expectedPrefix, StringComparison.Ordinal))
        {
            throw new CapabilityViolationException(
                $"Mod '{_modId}' attempted to register field '{id}' outside its own namespace");
        }

        string requiredCap = $"mod.{_modId}.field.write:{id}";
        if (!_modCapabilities.Contains(requiredCap))
        {
            throw new CapabilityViolationException(
                $"Mod '{_modId}' lacks capability '{requiredCap}' required to register field '{id}'");
        }

        return _registry.Register<T>(id, width, height);
    }

    public IFieldHandle GetField<T>(string id) where T : unmanaged
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Field id must be non-empty", nameof(id));

        string ownNamespacePrefix = _modId + ".";
        string requiredCap;
        if (id.StartsWith(ownNamespacePrefix, StringComparison.Ordinal))
        {
            requiredCap = $"mod.{_modId}.field.read:{id}";
        }
        else
        {
            int dot = id.IndexOf('.');
            if (dot <= 0) throw new ArgumentException($"Field id '{id}' must be namespaced");
            string foreignMod = id[..dot];
            requiredCap = $"mod.{foreignMod}.field.read:{id}";
        }

        if (!_modCapabilities.Contains(requiredCap))
        {
            throw new CapabilityViolationException(
                $"Mod '{_modId}' lacks capability '{requiredCap}' required to access field '{id}'");
        }

        return _registry.Get<T>(id);
    }

    public bool IsRegistered(string id) => _registry.IsRegistered(id);
}
