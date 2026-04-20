namespace DualFrontier.Application.Save;

/// <summary>
/// Версия и заголовок формата сохранения. При несовместимом значении
/// <see cref="Version"/> сохранение считается старым и должно проходить
/// миграцию (TODO: Фаза 3).
/// </summary>
public sealed class SaveFormat
{
    /// <summary>
    /// TODO: Фаза 1 — текущая версия формата. Увеличивается при любом
    /// несовместимом изменении сериализации.
    /// </summary>
    public int Version { get; init; }

    /// <summary>
    /// TODO: Фаза 1 — магический заголовок файла (например, "DFSAVE\0"),
    /// чтобы быстро отличить сохранение от произвольного бинарника.
    /// </summary>
    public string Header { get; init; } = "DFSAVE";
}
