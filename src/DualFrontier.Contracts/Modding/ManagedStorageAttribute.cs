using System;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Marks a class IComponent type as Path β (managed-class storage) per K-L3.1.
/// Mods register such components via <c>IModApi.RegisterManagedComponent&lt;T&gt;</c>.
///
/// Without this attribute, <c>RegisterManagedComponent&lt;T&gt;</c> raises
/// <c>ValidationErrorKind.MissingManagedStorageAttribute</c> at registration
/// time (per MOD_OS_ARCHITECTURE §11.2, ValidationErrorKind registry).
///
/// Path β components are runtime-only (Q4.b K-L3.1 lock) — not persisted
/// by the save system (save-system reconstruction is future work; the
/// former G-series that carried it was superseded by the V substrate per
/// Q-G-2). Storage lives in per-mod
/// <c>RestrictedModApi.ManagedStore&lt;T&gt;</c> instance; reclaimed on
/// <c>AssemblyLoadContext.Unload</c> per MOD_OS_ARCHITECTURE §9.5 unload chain.
///
/// Authority: KERNEL_ARCHITECTURE Part 0 (K-L3 implication post-K-L3.1);
/// K-L3.1 amendment plan at docs/architecture/K_L3_1_AMENDMENT_PLAN.md.
///
/// Lives in <c>DualFrontier.Contracts.Modding</c> because mod assemblies
/// annotate their own component classes with this attribute and must be able
/// to reference it directly. Cannot live in <c>DualFrontier.Application.Modding</c>
/// (which mods do not reference).
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ManagedStorageAttribute : Attribute
{
}
