using System.Collections.Generic;
using System.Text.Json;
using DualFrontier.Contracts.Math;

namespace DualFrontier.Application.Scene;

/// <summary>
/// Describes a single entity to spawn when the scene loads. The
/// <see cref="Prefab"/> is a registered identifier (for example
/// <c>"core:pawn"</c>) that resolves to a component bundle at runtime.
/// Individual component fields can be overridden via <see cref="Components"/>.
/// </summary>
/// <param name="Id">Unique identifier within the scene — used for debug only.</param>
/// <param name="Prefab">Registered prefab identifier, namespaced (<c>mod:name</c>).</param>
/// <param name="Position">Spawn position on the tilemap.</param>
/// <param name="Components">Component-name → JSON-override mapping.</param>
public sealed record EntitySpawnDef(
    string                                   Id,
    string                                   Prefab,
    GridVector                               Position,
    IReadOnlyDictionary<string, JsonElement> Components
);
