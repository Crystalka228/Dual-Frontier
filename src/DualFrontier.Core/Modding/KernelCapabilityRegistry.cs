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
/// W3 note (LIVE): <see cref="RegisterOwner"/> is wired into
/// <c>ModIntegrationPipeline.Apply</c> -- pass [1] registers each loaded shared mod's assembly,
/// pass [2] each regular mod's own assemblies. <see cref="RemoveOwner"/> keeps the ledger
/// symmetric on every regular-mod exit (rollback and unload). Shared registrations PERSIST for
/// the session, mirroring the non-collectible shared ALC (MOD_OS §5.1): the types stay resolvable,
/// so their ownership must stay recorded.
/// </summary>
internal sealed class KernelCapabilityRegistry
{
    private readonly HashSet<string> _capabilities = new(StringComparer.Ordinal);

    // Owner namespace ("kernel" | "mod.<modId>") -> the FQNs that owner registered.
    // Backs Owns(), the self-access predicate. An owner never needs a declared token for a
    // type it registered here.
    private readonly Dictionary<string, HashSet<string>> _ownedByOwner =
        new(StringComparer.Ordinal);

    // Owner namespace -> the exact capability TOKENS that owner's registration added.
    // RemoveOwner subtracts precisely this set. It is NOT re-derived by prefix matching on
    // _capabilities: owner ids nest ("mod.a" is a string prefix of "mod.ab"), so a prefix sweep
    // is a latent cross-owner deletion bug. Exact bookkeeping is the only safe removal.
    private readonly Dictionary<string, HashSet<string>> _tokensByOwner =
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

        HashSet<string> tokens = _tokensByOwner.TryGetValue(ownerNamespace, out HashSet<string>? tset)
            ? tset
            : _tokensByOwner[ownerNamespace] = new HashSet<string>(StringComparer.Ordinal);

        ScanAssembly(assembly, ownerNamespace, owned, tokens);
    }

    /// <summary>
    /// Removes every capability token and every ownership record registered under
    /// <paramref name="ownerNamespace"/>, restoring the ledger to its pre-registration state for
    /// that owner. Idempotent: removing an owner that never registered is a no-op.
    ///
    /// <para>
    /// Subtracts exactly the token set that owner's <see cref="RegisterOwner"/> calls added --
    /// never a prefix sweep over the flat token set, because owner ids nest and a sweep for
    /// <c>mod.a</c> would take <c>mod.ab</c>'s tokens with it.
    /// </para>
    ///
    /// <para>
    /// Called for REGULAR mods only. A shared mod's registration persists for the session because
    /// its assembly does too (the shared ALC is non-collectible, MOD_OS §5.1); revoking ownership
    /// of types that are still loaded and resolvable would make the ledger lie.
    /// </para>
    /// </summary>
    public void RemoveOwner(string ownerNamespace)
    {
        if (ownerNamespace is null) throw new ArgumentNullException(nameof(ownerNamespace));

        if (_tokensByOwner.TryGetValue(ownerNamespace, out HashSet<string>? tokens))
        {
            foreach (string token in tokens)
                _capabilities.Remove(token);
            _tokensByOwner.Remove(ownerNamespace);
        }

        _ownedByOwner.Remove(ownerNamespace);
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

    /// <summary>
    /// Records one capability token in BOTH the flat set the gate queries and the per-owner set
    /// <see cref="RemoveOwner"/> subtracts. Keeping the two in lockstep here is what makes removal
    /// exact -- there is no second place a token can enter the ledger.
    /// </summary>
    private void AddToken(HashSet<string> tokens, string token)
    {
        _capabilities.Add(token);
        tokens.Add(token);
    }

    private void ScanAssembly(Assembly assembly, string owner, HashSet<string> owned, HashSet<string> tokens)
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
                        AddToken(tokens, $"{owner}.fast.publish:{fqn}");
                        AddToken(tokens, $"{owner}.fast.subscribe:{fqn}");
                        break;
                    case BusTier.Normal:
                        AddToken(tokens, $"{owner}.normal.publish:{fqn}");
                        AddToken(tokens, $"{owner}.normal.subscribe:{fqn}");
                        AddToken(tokens, $"{owner}.publish:{fqn}");
                        AddToken(tokens, $"{owner}.subscribe:{fqn}");
                        break;
                    case BusTier.Background:
                        AddToken(tokens, $"{owner}.background.publish:{fqn}");
                        AddToken(tokens, $"{owner}.background.subscribe:{fqn}");
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
                    if (attr.Read) { AddToken(tokens, $"{owner}.read:{fqn}"); registered = true; }
                    if (attr.Write) { AddToken(tokens, $"{owner}.write:{fqn}"); registered = true; }
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
                        AddToken(tokens, $"{owner}.layer.intent:{fqn}");
                        registered = true;
                        break;
                    case LayerType.CombatFeedback:
                        AddToken(tokens, $"{owner}.layer.combat_feedback:{fqn}");
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
