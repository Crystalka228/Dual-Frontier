namespace DualFrontier.AI.BehaviourTree;

/// <summary>
/// A per-pawn key-value store for BT nodes to share state between ticks.
/// Nodes read and write named values here instead of keeping their own fields.
/// </summary>
public sealed class BTBlackboard
{
    private readonly Dictionary<string, object?> _values = new();

    /// <summary>
    /// Sets a value by key. Overwrites if key already exists.
    /// </summary>
    /// <param name="key">The key under which the value is set.</param>
    /// <param name="value">The value to store.</param>
    public void Set(string key, object? value)
    {
        _values[key] = value;
    }

    /// <summary>
    /// Gets a value by key. Returns null if key not found or value is explicitly null.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <returns>The stored object, or null if the key does not exist.</returns>
    public object? Get(string key)
    {
        if (_values.TryGetValue(key, out var value))
        {
            return (object?)value;
        }

        return null;
    }

    /// <summary>
    /// Gets a typed value from the blackboard by key. Returns default(T) if key not found or wrong type.
    /// </summary>
    /// <typeparam name="T">The expected type of the value.</typeparam>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <returns>The casted value, or default(T) if the key is not found or the stored value cannot be cast to T.</returns>
    public T? Get<T>(string key)
    {
        if (_values.TryGetValue(key, out var value))
        {
            // Try casting via 'is' pattern match for safe type checking and retrieval.
            return (value is T typed) ? typed : default;
        }

        return default;
    }

    /// <summary>
    /// Returns true if key exists.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    /// <returns><c>true</c> if the key is present; otherwise, <c>false</c>.</returns>
    public bool Has(string key)
    {
        return _values.ContainsKey(key);
    }

    /// <summary>
    /// Removes a key. No-op if key does not exist.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    public void Clear(string key)
    {
        _values.Remove(key);
    }

    /// <summary>
    /// Removes all keys. Called on job reassignment.
    /// </summary>
    public void ClearAll()
    {
        _values.Clear();
    }
}