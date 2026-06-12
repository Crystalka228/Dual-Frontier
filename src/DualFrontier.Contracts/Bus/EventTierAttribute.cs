using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Three-tier dispatch enum per К-L15 (KERNEL_ARCHITECTURE Part 0; К10.2).
/// Wire format: <see cref="int"/> matching native <c>dualfrontier::BusTier</c>.
/// </summary>
public enum BusTier
{
    /// <summary>
    /// Synchronous bypass; preemption-aware; ≤1ms latency target.
    /// Subscriber contract: bounded exec, no blocking, no GC allocation.
    /// Use cases: combat hits, player input, emergency.
    /// </summary>
    Fast = 0,

    /// <summary>
    /// Batched callback per-phase. Standard subscriber contract.
    /// К-L7 atomic-from-observer preserved within batch boundary.
    /// Default tier when no <see cref="EventTierAttribute"/> is declared
    /// (backward compatibility per S-LOCK-4).
    /// </summary>
    Normal = 1,

    /// <summary>
    /// Coalesce + idle-slot dispatch; multi-tick acceptable.
    /// Subscriber contract: long-running ok, interruptible.
    /// Use cases: off-screen simulation, climate, quest generation.
    /// </summary>
    Background = 2,
}

/// <summary>
/// Declares the bus dispatch tier для an <see cref="Core.IEvent"/> type
/// (К10.2 Item 28). Read by <c>KernelCapabilityRegistry</c> к build per-FQN
/// per-tier capability tokens (S-LOCK-4) and by <c>BusFacade</c> к choose
/// the dispatch path (Item 27).
///
/// Events without this attribute default к <see cref="BusTier.Normal"/>
/// (per S-LOCK-4 backward compatibility — existing
/// <c>kernel.publish:{FQN}</c> / <c>kernel.subscribe:{FQN}</c> capability
/// tokens continue to function without manifest changes).
///
/// Background tier requires a coalesce function (declared on the event type's
/// static factory or via <see cref="CoalesceFunctionTypeName"/>); load-time
/// validation surfaces <c>BackgroundCoalesceMissing</c> when absent
/// (Item 29 + MOD_OS §11.2).
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class EventTierAttribute : Attribute
{
    /// <summary>Dispatch tier per К-L15.</summary>
    public BusTier Tier { get; }

    /// <summary>
    /// Optional hint to native registry for fixed-payload tier semantics.
    /// 0 means «let the registry compute size from Marshal.SizeOf at first
    /// registration». Non-zero pins the payload size constraint.
    /// </summary>
    public int PayloadSizeBytesHint { get; }

    /// <summary>
    /// Fully qualified name of a static method <c>void Coalesce(ref T existing, in T newEvent)</c>
    /// on the event type itself. Required for <see cref="BusTier.Background"/>.
    /// </summary>
    public string? CoalesceFunctionTypeName { get; init; }

    public EventTierAttribute(BusTier tier, int payloadSizeBytesHint = 0)
    {
        Tier = tier;
        PayloadSizeBytesHint = payloadSizeBytesHint;
    }
}
