using System;
using System.Collections.Generic;
using System.Reflection;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

/// <summary>
/// K10.2 Item 29 — Subscriber contract enforcement (per-tier load-time
/// validation). Reads <see cref="EventTierAttribute"/> on event types
/// и cross-checks against manifest-declared tier-specific capabilities.
///
/// Emits <see cref="ValidationErrorKind.FastTierContractViolation"/>,
/// <see cref="ValidationErrorKind.BusTierMismatch"/>,
/// <see cref="ValidationErrorKind.BackgroundCoalesceMissing"/> диагностики
/// per MOD_OS_ARCHITECTURE.md §11.2 (amended at K10.2 closure, v1.9).
///
/// Fast tier static-contract verification (blocking-call detection в
/// subscriber method body) is a Roslyn analyzer concern deferred к A'.9 —
/// К10.2 lands the runtime monitor (FastTierContractMonitor) and reserves
/// the diagnostic enum entry.
/// </summary>
public sealed class SubscriberContractValidator
{
    /// <summary>
    /// Validates one event type's tier declarations against the manifest
    /// that references it. Returns 0 or more errors.
    /// </summary>
    public IReadOnlyList<ValidationError> Validate(
        string modId, Type eventType, ModManifest manifest)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        if (eventType is null) throw new ArgumentNullException(nameof(eventType));
        if (manifest is null) throw new ArgumentNullException(nameof(manifest));
        if (!typeof(IEvent).IsAssignableFrom(eventType))
            return Array.Empty<ValidationError>();

        var errors = new List<ValidationError>();

        EventTierAttribute? tierAttr = eventType.GetCustomAttribute<EventTierAttribute>();
        BusTier eventTier = tierAttr?.Tier ?? BusTier.Normal;
        string fqn = eventType.FullName ?? eventType.Name;

        // Background coalesce missing diagnostic (К10.2 Item 29 + Q-N-34).
        if (eventTier == BusTier.Background && string.IsNullOrEmpty(tierAttr?.CoalesceFunctionTypeName))
        {
            errors.Add(new ValidationError(
                modId,
                ValidationErrorKind.BackgroundCoalesceMissing,
                $"Event type '{fqn}' is declared с BusTier.Background but does not " +
                $"specify a CoalesceFunctionTypeName в [EventTier]. Background tier dispatch " +
                $"requires per-(type_id, coalesce_key) coalesce semantics."));
        }

        // Tier mismatch diagnostic: manifest declares tier-specific token
        // (e.g. `kernel.fast.subscribe:{FQN}`) но event tier is different.
        foreach (string required in manifest.Capabilities.Required)
        {
            if (!TryParseTierToken(required, out BusTier manifestTier, out string tokenFqn))
                continue;
            if (!string.Equals(tokenFqn, fqn, StringComparison.Ordinal))
                continue;
            if (manifestTier != eventTier)
            {
                errors.Add(new ValidationError(
                    modId,
                    ValidationErrorKind.BusTierMismatch,
                    $"Mod '{modId}' declares capability '{required}' ({manifestTier} tier) " +
                    $"but event type '{fqn}' is annotated as {eventTier} tier."));
            }
        }

        return errors;
    }

    /// <summary>
    /// Parses tokens of the form <c>kernel.&lt;tier&gt;.&lt;verb&gt;:{FQN}</c>
    /// (e.g. <c>kernel.fast.publish:DualFrontier.Combat.DamageEvent</c>).
    /// Returns false for backward-compatible legacy tokens
    /// <c>kernel.publish:{FQN}</c> / <c>kernel.subscribe:{FQN}</c>, which
    /// map к Normal tier per S-LOCK-4.
    /// </summary>
    public static bool TryParseTierToken(string token, out BusTier tier, out string fqn)
    {
        tier = BusTier.Normal;
        fqn = string.Empty;
        if (string.IsNullOrEmpty(token) || !token.StartsWith("kernel.", StringComparison.Ordinal))
            return false;

        int colonIdx = token.IndexOf(':');
        if (colonIdx <= 0) return false;

        string prefix = token.Substring(0, colonIdx);
        string typeFqn = token.Substring(colonIdx + 1);

        // kernel.<tier>.<verb> — 3 segments after "kernel.": "tier.verb"
        // kernel.<verb> — 1 segment: "verb"
        string suffix = prefix.Substring("kernel.".Length);
        string[] parts = suffix.Split('.');
        if (parts.Length == 1)
        {
            // Legacy token kernel.publish:{FQN} / kernel.subscribe:{FQN}
            return false;
        }
        if (parts.Length == 2)
        {
            string tierStr = parts[0];
            tier = tierStr switch
            {
                "fast"       => BusTier.Fast,
                "normal"     => BusTier.Normal,
                "background" => BusTier.Background,
                _ => BusTier.Normal,
            };
            fqn = typeFqn;
            return tierStr is "fast" or "normal" or "background";
        }
        return false;
    }
}
