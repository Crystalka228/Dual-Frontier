using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Runtime registry of inter-mod contracts. One publishing mod owns each
/// contract type; any number of consumers read it back through
/// <see cref="TryGet{T}"/>. When the publishing mod unloads,
/// <see cref="RevokeAll"/> removes its entries and subsequent lookups
/// gracefully return false — consumers degrade without crashing.
/// </summary>
internal interface IModContractStore
{
    /// <summary>
    /// Publishes a contract for the given mod. Overwrites a previous
    /// publication by the same mod; throws when a different mod already owns
    /// the contract type.
    /// </summary>
    void Publish<T>(string modId, T contract) where T : IModContract;

    /// <summary>
    /// Attempts to fetch a previously published contract of type {T}.
    /// Returns <c>false</c> and sets <paramref name="contract"/> to null
    /// when no mod has published one yet or the publisher was revoked.
    /// </summary>
    bool TryGet<T>(out T? contract) where T : class, IModContract;

    /// <summary>
    /// Removes every contract owned by the given mod. Called from the
    /// unload chain before the mod's <see cref="ModLoadContext"/> is
    /// collected.
    /// </summary>
    void RevokeAll(string modId);
}
