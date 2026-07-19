using System;
using System.Collections.Generic;
using System.Reflection;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Display;

namespace DualFrontier.Core.Modding;

/// <summary>
/// Capability registration ledger (W2/BD-3, BD-10). Formerly a kernel-assembly reflection
/// scanner that republished every gameplay type as <c>kernel.*</c>; now an owner-namespaced
/// ledger. Capability tokens name their owner: <c>kernel.{verb}:{FQN}</c> for engine-owned
/// types, <c>mod.&lt;ownerId&gt;.{verb}:{FQN}</c> for mod-owned types (MOD_OS §3.2-3.4).
///
/// The genre taxonomy left the engine contract at W2/BD-3, so the engine owns no gameplay
/// types and the kernel-provided FQN set is empty. Gameplay types still living engine-side
/// this wave are ownerless and ride the v1 grace path (sacrificial scaffolding) until a later
/// wave moves them into vanilla mods, where they become <c>mod.&lt;id&gt;</c>-owned.
///
/// Self-access: a registered owner is auto-granted its OWN types -- a mod never declares
/// capabilities for the types it registered (<see cref="Owns"/>, consulted by the capability
/// gate before requiring a declared token); declared capabilities gate CROSS-owner access.
///
/// W2 note (mechanism, not live): <see cref="RegisterOwner"/> is exercised by tests. The
/// per-mod scan is NOT wired into the load pipeline this wave -- vanilla mods define no types
/// yet, so it would be a no-op; wiring it is deferred to the wave that moves gameplay types
/// into the vanilla mods (owner-registration then has a producer).
/// </summary>
internal sealed class KernelCapabilityRegistry
{
    private readonly HashSet<string> _capabilities = new(StringComparer.Ordinal);

    // Owner namespace ("kernel" | "mod.<modId>") -> the FQNs that owner registered.
    // Backs Owns(), the self-access predicate. An owner never needs a declared token for a
    // type it registered here.
    private readonly Dictionary<string, HashSet<string>> _ownedByOwner =
        new(StringComparer.Ordinal);

    /// <summary>
    /// The complete set of capability tokens registered across all owners. Empty until an
    /// owner registers types; the kernel surface is empty by construction post-BD-10.
    /// </summary>
    public IReadOnlySet<string> Capabilities => _capabilities;

    /// <summary>
    /// Returns <see langword="true"/> when the given token is registered by some owner.
    /// </summary>
    public bool Provides(string token) => _capabilities.Contains(token);

    /// <summary>
    /// Returns <see langword="true"/> only when <paramref name="token"/> is registered AND is a
    /// kernel-owned token (the <c>kernel.</c> owner prefix). The Phase-C kernel fast path
    /// (<c>ContractValidator</c>) MUST use this, NOT <see cref="Provides"/>: a mod-owned
    /// <c>mod.&lt;id&gt;.*</c> token is satisfiable only through an explicitly-listed dependency,
    /// never through the kernel-provided set (MOD_OS §3.5). Keeping the two apart stops a consumer
    /// from satisfying a cross-mod capability without declaring the provider in <c>dependencies</c>
    /// once per-mod owner registration is wired.
    /// </summary>
    public bool ProvidesKernel(string token)
        => token.StartsWith("kernel.", StringComparison.Ordinal) && _capabilities.Contains(token);

    /// <summary>
    /// Registers, under <paramref name="ownerNamespace"/> (e.g. <c>"kernel"</c> or
    /// <c>"mod.&lt;modId&gt;"</c>), the capability tokens for every public, concrete
    /// <see cref="IEvent"/> / <c>[ModAccessible]</c> <see cref="IComponent"/> / <c>[Layer]</c>
    /// type in <paramref name="assembly"/>, and records ownership of each such FQN for
    /// self-access. Generic and nested types (FQN containing <c>`</c> or <c>+</c>) are
    /// silently skipped. Idempotent -- re-registering the same assembly under the same owner
    /// does not double-count.
    /// </summary>
    public void RegisterOwner(string ownerNamespace, Assembly assembly)
    {
        if (ownerNamespace is null) throw new ArgumentNullException(nameof(ownerNamespace));
        if (assembly is null) throw new ArgumentNullException(nameof(assembly));

        HashSet<string> owned = _ownedByOwner.TryGetValue(ownerNamespace, out HashSet<string>? set)
            ? set
            : _ownedByOwner[ownerNamespace] = new HashSet<string>(StringComparer.Ordinal);

        ScanAssembly(assembly, ownerNamespace, owned);
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="ownerNamespace"/> registered
    /// <paramref name="fqn"/> -- the self-access predicate. The capability gate consults this
    /// before requiring a declared token: an owner is auto-granted its own registered types.
    /// </summary>
    public bool Owns(string ownerNamespace, string fqn)
        => _ownedByOwner.TryGetValue(ownerNamespace, out HashSet<string>? owned)
           && owned.Contains(fqn);

    /// <summary>
    /// Returns the owner namespace that registered <paramref name="fqn"/> (a type has one defining
    /// assembly, hence one owner), or <see langword="null"/> when no owner registered it. The
    /// runtime capability gate resolves the owner to build the owner-namespaced token -- a
    /// cross-owner event is declared as <c>mod.&lt;provider&gt;.{verb}:{FQN}</c>, not
    /// <c>kernel.{verb}:{FQN}</c>. A null result falls back to <c>kernel</c> at the gate, the
    /// behavior when nothing is registered (this wave, no producer wires <see cref="RegisterOwner"/>).
    /// </summary>
    public string? OwnerOf(string fqn)
    {
        foreach (KeyValuePair<string, HashSet<string>> entry in _ownedByOwner)
            if (entry.Value.Contains(fqn))
                return entry.Key;
        return null;
    }

    private void ScanAssembly(Assembly assembly, string owner, HashSet<string> owned)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsPublic) continue;
            if (type.IsAbstract) continue;

            string? fqn = type.FullName;
            if (fqn is null) continue;
            if (fqn.IndexOf('`') >= 0) continue;
            if (fqn.IndexOf('+') >= 0) continue;

            bool registered = false;

            if (typeof(IEvent).IsAssignableFrom(type))
            {
                // Tier-prefixed tokens per [EventTier] (Normal when unattributed); Normal
                // tier additionally emits the legacy un-prefixed publish/subscribe aliases
                // (S-LOCK-4 backward compatibility). Owner-prefixed post-BD-10.
                EventTierAttribute? tierAttr = type.GetCustomAttribute<EventTierAttribute>();
                BusTier tier = tierAttr?.Tier ?? BusTier.Normal;

                switch (tier)
                {
                    case BusTier.Fast:
                        _capabilities.Add($"{owner}.fast.publish:{fqn}");
                        _capabilities.Add($"{owner}.fast.subscribe:{fqn}");
                        break;
                    case BusTier.Normal:
                        _capabilities.Add($"{owner}.normal.publish:{fqn}");
                        _capabilities.Add($"{owner}.normal.subscribe:{fqn}");
                        _capabilities.Add($"{owner}.publish:{fqn}");
                        _capabilities.Add($"{owner}.subscribe:{fqn}");
                        break;
                    case BusTier.Background:
                        _capabilities.Add($"{owner}.background.publish:{fqn}");
                        _capabilities.Add($"{owner}.background.subscribe:{fqn}");
                        break;
                }

                registered = true;
            }

            if (typeof(IComponent).IsAssignableFrom(type))
            {
                // read/write apply only to the opt-in [ModAccessible] subset (D-1 LOCKED).
                ModAccessibleAttribute? attr =
                    type.GetCustomAttribute<ModAccessibleAttribute>();
                if (attr is not null)
                {
                    if (attr.Read) { _capabilities.Add($"{owner}.read:{fqn}"); registered = true; }
                    if (attr.Write) { _capabilities.Add($"{owner}.write:{fqn}"); registered = true; }
                }
            }

            // К-L17 layer tokens: observable descriptors of the display-composition surface,
            // NOT declarable manifest permissions (the §3.2 grammar has no layer verb).
            LayerAttribute? layerAttr = type.GetCustomAttribute<LayerAttribute>();
            if (layerAttr is not null)
            {
                switch (layerAttr.LayerType)
                {
                    case LayerType.Intent:
                        _capabilities.Add($"{owner}.layer.intent:{fqn}");
                        registered = true;
                        break;
                    case LayerType.CombatFeedback:
                        _capabilities.Add($"{owner}.layer.combat_feedback:{fqn}");
                        registered = true;
                        break;
                    case LayerType.SimState:
                    case LayerType.Static:
                    default:
                        // SimState/Static use renderer-level capabilities; no layer token.
                        break;
                }
            }

            if (registered)
                owned.Add(fqn);
        }
    }
}
