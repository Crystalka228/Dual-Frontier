using System;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Метаданные мода. Заполняются из файла <c>mod.manifest.json</c> при загрузке.
/// Используется ModLoader для разрешения зависимостей и отображения информации
/// пользователю в меню модов.
/// </summary>
public sealed class ModManifest
{
    // TODO: Фаза 2 — public required string Id { get; init; }
    // TODO: Фаза 2 — public required string Name { get; init; }
    // TODO: Фаза 2 — public required string Version { get; init; }
    // TODO: Фаза 2 — public required string Author { get; init; }
    // TODO: Фаза 2 — public required IReadOnlyList<string> Dependencies { get; init; }
}
