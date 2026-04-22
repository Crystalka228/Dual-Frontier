using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

/// <summary>
/// In-memory implementation of <see cref="IModContractStore"/> backed by a
/// single dictionary keyed by contract type. Not thread-safe: contracts are
/// published and consumed during <c>IMod.Initialize</c> which runs on the
/// menu thread; runtime systems never mutate the store.
/// </summary>
internal sealed class ModContractStore : IModContractStore
{
    private readonly Dictionary<Type, (string ModId, object Contract)> _entries = new();

    /// <inheritdoc />
    public void Publish<T>(string modId, T contract) where T : IModContract
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        if (contract is null) throw new ArgumentNullException(nameof(contract));

        Type key = typeof(T);
        if (_entries.TryGetValue(key, out (string OwnerId, object _) existing)
            && existing.OwnerId != modId)
        {
            // Один тип контракта — один владелец. Иначе потребители не знают,
            // чью реализацию они получили.
            throw new InvalidOperationException(
                $"[CONTRACT STORE ERROR] Contract '{key.FullName}' is already " +
                $"published by mod '{existing.OwnerId}'. Mod '{modId}' cannot overwrite it.");
        }

        _entries[key] = (modId, contract);
    }

    /// <inheritdoc />
    public bool TryGet<T>(out T? contract) where T : class, IModContract
    {
        if (_entries.TryGetValue(typeof(T), out (string _, object Value) entry))
        {
            contract = (T)entry.Value;
            return true;
        }

        contract = null;
        return false;
    }

    /// <inheritdoc />
    public void RevokeAll(string modId)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));

        var toRemove = new List<Type>();
        foreach (KeyValuePair<Type, (string Owner, object _)> pair in _entries)
        {
            if (pair.Value.Owner == modId)
                toRemove.Add(pair.Key);
        }
        foreach (Type key in toRemove)
            _entries.Remove(key);
    }
}
