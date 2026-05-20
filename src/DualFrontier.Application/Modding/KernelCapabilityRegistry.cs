using System;
using System.Collections.Generic;
using System.Reflection;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Display;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Catalogue of capability tokens the kernel offers to mods. Built once at
/// startup by reflecting over kernel assemblies — the result is the
/// authoritative set of <c>kernel.publish:*</c>, <c>kernel.subscribe:*</c>,
/// <c>kernel.read:*</c> and <c>kernel.write:*</c> tokens that mods may
/// declare in <c>capabilities.required</c>.
///
/// Token rules (MOD_OS_ARCHITECTURE §3.2, §3.5, D-1):
/// <list type="bullet">
///   <item>Public, concrete <see cref="IEvent"/> implementer →
///     <c>kernel.publish:{FQN}</c> and <c>kernel.subscribe:{FQN}</c>.</item>
///   <item>Public, concrete <see cref="IComponent"/> implementer with
///     <c>[ModAccessible(Read = true)]</c> → <c>kernel.read:{FQN}</c>.</item>
///   <item>Public, concrete <see cref="IComponent"/> implementer with
///     <c>[ModAccessible(Write = true)]</c> → <c>kernel.write:{FQN}</c>.</item>
/// </list>
/// Generic and nested types (FQN containing <c>`</c> or <c>+</c>) are
/// silently skipped — they are not addressable through the capability model.
/// </summary>
internal sealed class KernelCapabilityRegistry
{
    private readonly IReadOnlySet<string> _capabilities;

    /// <summary>
    /// Scans the given assemblies for kernel-provided capabilities. Duplicate
    /// assembly references are deduplicated — passing the same assembly more
    /// than once does not double-count its tokens.
    /// </summary>
    public KernelCapabilityRegistry(IEnumerable<Assembly> assemblies)
    {
        if (assemblies is null) throw new ArgumentNullException(nameof(assemblies));

        var capabilities = new HashSet<string>();
        var visited = new HashSet<Assembly>();

        foreach (Assembly assembly in assemblies)
        {
            if (assembly is null) continue;
            if (!visited.Add(assembly)) continue;

            ScanAssembly(assembly, capabilities);
        }

        _capabilities = capabilities;
    }

    /// <summary>
    /// Returns the complete set of capability tokens the kernel provides.
    /// </summary>
    public IReadOnlySet<string> Capabilities => _capabilities;

    /// <summary>
    /// Returns <see langword="true"/> when the given token is in the
    /// kernel-provided set.
    /// </summary>
    public bool Provides(string token) => _capabilities.Contains(token);

    /// <summary>
    /// Builds a registry from the three kernel-surface assemblies whose public
    /// types may resolve to capability tokens: <c>DualFrontier.Contracts</c>
    /// (host of <see cref="IEvent"/> and <see cref="IComponent"/> markers and
    /// any cross-cutting types that may live alongside them),
    /// <c>DualFrontier.Components</c> (production component types annotated
    /// with <see cref="ModAccessibleAttribute"/>) and <c>DualFrontier.Events</c>
    /// (production event types). The constructor's deduplication keeps the
    /// scan single-pass — passing two markers that resolve to the same
    /// assembly (e.g. <see cref="IEvent"/> and <see cref="IComponent"/> both
    /// currently live in <c>DualFrontier.Contracts</c>) does not double-count.
    /// </summary>
    internal static KernelCapabilityRegistry BuildFromKernelAssemblies()
        => new(new[]
        {
            typeof(IEvent).Assembly,           // DualFrontier.Contracts
            typeof(IComponent).Assembly,       // DualFrontier.Contracts (deduped against IEvent)
            typeof(HealthComponent).Assembly,  // DualFrontier.Components
            typeof(PawnSpawnedEvent).Assembly, // DualFrontier.Events
        });

    private static void ScanAssembly(Assembly assembly, HashSet<string> capabilities)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsPublic) continue;
            if (type.IsAbstract) continue;

            string? fqn = type.FullName;
            if (fqn is null) continue;
            if (fqn.IndexOf('`') >= 0) continue;
            if (fqn.IndexOf('+') >= 0) continue;

            if (typeof(IEvent).IsAssignableFrom(type))
            {
                // К10.2 Item 28 + S-LOCK-4: tier-prefixed tokens based on
                // [EventTier] attribute. Events без attribute default к
                // Normal tier; legacy kernel.publish/subscribe tokens
                // continue к work (backward compatibility).
                EventTierAttribute? tierAttr = type.GetCustomAttribute<EventTierAttribute>();
                BusTier tier = tierAttr?.Tier ?? BusTier.Normal;

                switch (tier)
                {
                    case BusTier.Fast:
                        capabilities.Add($"kernel.fast.publish:{fqn}");
                        capabilities.Add($"kernel.fast.subscribe:{fqn}");
                        break;
                    case BusTier.Normal:
                        capabilities.Add($"kernel.normal.publish:{fqn}");
                        capabilities.Add($"kernel.normal.subscribe:{fqn}");
                        // Backward-compat aliases per S-LOCK-4 — existing
                        // kernel.publish:{FQN} / kernel.subscribe:{FQN} tokens
                        // continue к work for Normal tier events.
                        capabilities.Add($"kernel.publish:{fqn}");
                        capabilities.Add($"kernel.subscribe:{fqn}");
                        break;
                    case BusTier.Background:
                        capabilities.Add($"kernel.background.publish:{fqn}");
                        capabilities.Add($"kernel.background.subscribe:{fqn}");
                        break;
                }
            }

            if (typeof(IComponent).IsAssignableFrom(type))
            {
                ModAccessibleAttribute? attr =
                    type.GetCustomAttribute<ModAccessibleAttribute>();
                if (attr is null) continue;

                if (attr.Read)
                    capabilities.Add($"kernel.read:{fqn}");
                if (attr.Write)
                    capabilities.Add($"kernel.write:{fqn}");
            }

            // К10.3 v2 Items 39+40: К-L17 layer capability tokens. Mod-registered
            // layer classes carry [Layer(LayerType.Intent | CombatFeedback)] и
            // surface as kernel.layer.intent:{FQN} / kernel.layer.combat_feedback:{FQN}
            // tokens per S3-Q5 + S8-Q3 granular FQN pattern. SimState и Static use
            // existing renderer-level capabilities (V substrate primitives) и do
            // не emit layer capability tokens here.
            LayerAttribute? layerAttr = type.GetCustomAttribute<LayerAttribute>();
            if (layerAttr is not null)
            {
                switch (layerAttr.LayerType)
                {
                    case LayerType.Intent:
                        capabilities.Add($"kernel.layer.intent:{fqn}");
                        break;
                    case LayerType.CombatFeedback:
                        capabilities.Add($"kernel.layer.combat_feedback:{fqn}");
                        break;
                    case LayerType.SimState:
                    case LayerType.Static:
                    default:
                        // SimState/Static use renderer-level capabilities; no
                        // К-L17 layer token surfaces here.
                        break;
                }
            }
        }
    }
}
