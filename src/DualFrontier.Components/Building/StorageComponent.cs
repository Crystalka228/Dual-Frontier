namespace DualFrontier.Components.Building;

using System.Collections.Generic;
using DualFrontier.Contracts.Core;

/// <summary>
/// Storage building that holds item stacks.
/// Written exclusively by InventorySystem.
/// One StorageComponent per storage building entity.
/// </summary>
public sealed class StorageComponent : IComponent
{
    /// <summary>Maximum number of distinct item stacks this storage accepts.</summary>
    public int Capacity = 20;

    /// <summary>Item stacks currently stored. Key = item template id, Value = quantity.</summary>
    public Dictionary<string, int> Items = new();

    /// <summary>Whether this storage accepts any item type (true) or only allowed types.</summary>
    public bool AcceptAll = true;

    /// <summary>Allowed item type ids when AcceptAll = false.</summary>
    public HashSet<string> AllowedItems = new();

    /// <summary>True if storage is full (Items.Count >= Capacity).</summary>
    public bool IsFull => Items.Count >= Capacity;

    /// <summary>Total quantity of all items across all stacks.</summary>
    public int TotalQuantity
    {
        get
        {
            int total = 0;
            foreach (var v in Items.Values) total += v;
            return total;
        }
    }
}