namespace DualFrontier.Components.Building;

using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;

/// <summary>
/// Storage building that holds item stacks.
/// Written exclusively by InventorySystem.
/// One StorageComponent per storage building entity.
/// </summary>
[ModAccessible(Read = true, Write = true)]
public struct StorageComponent : IComponent
{
    /// <summary>
    /// Maximum number of distinct item stacks this storage accepts. No
    /// implicit default after K8.2 v2 conversion — the construction site
    /// must set Capacity explicitly. Production callers (factories,
    /// scenario loaders) own the value.
    /// </summary>
    public int Capacity;

    /// <summary>
    /// Item stacks currently stored. Key = item template id (interned),
    /// Value = quantity. Native-backed; default (IsValid == false) for
    /// un-initialised storage components — construct via
    /// <c>NativeWorld.CreateMap&lt;InternedString, int&gt;()</c>.
    /// </summary>
    public NativeMap<InternedString, int> Items;

    /// <summary>
    /// Whether this storage accepts any item type (true) or only allowed
    /// types (false, see <see cref="AllowedItems"/>).
    /// </summary>
    public bool AcceptAll;

    /// <summary>
    /// Allowed item type ids when <see cref="AcceptAll"/> is false.
    /// Native-backed; default (IsValid == false) for un-initialised
    /// storage components — construct via
    /// <c>NativeWorld.CreateSet&lt;InternedString&gt;()</c>.
    /// </summary>
    public NativeSet<InternedString> AllowedItems;

    /// <summary>True if storage is full (Items.Count >= Capacity).</summary>
    public bool IsFull => Items.IsValid && Items.Count >= Capacity;

    /// <summary>Total quantity of all items across all stacks.</summary>
    public int TotalQuantity
    {
        get
        {
            if (!Items.IsValid) return 0;
            int count = Items.Count;
            if (count == 0) return 0;
            var keysBuf = new InternedString[count];
            var valuesBuf = new int[count];
            Items.Iterate(keysBuf, valuesBuf);
            int total = 0;
            for (int i = 0; i < count; i++) total += valuesBuf[i];
            return total;
        }
    }
}
