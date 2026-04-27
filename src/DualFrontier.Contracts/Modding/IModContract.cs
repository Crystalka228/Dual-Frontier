using System;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Marker interface for an inter-mod contract. One mod publishes the
/// implementation through <c>IModApi.PublishContract</c>; others retrieve it
/// through <c>IModApi.TryGetContract</c>. This is the only legal channel for
/// mod-to-mod communication: a direct reference to another mod's assembly is
/// impossible (different AssemblyLoadContexts).
/// </summary>
public interface IModContract
{
}
