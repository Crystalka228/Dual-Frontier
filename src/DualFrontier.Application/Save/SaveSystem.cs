using System;

namespace DualFrontier.Application.Save;

/// <summary>
/// Реализация <see cref="ISaveSystem"/>. Сериализует <c>World</c> и все его
/// компонентные хранилища в бинарный формат с версионным заголовком
/// (<see cref="SaveFormat"/>).
/// </summary>
public sealed class SaveSystem : ISaveSystem
{
    /// <inheritdoc />
    public void Save(string path)
    {
        throw new NotImplementedException("TODO: Фаза 1/3 — сериализация World");
    }

    /// <inheritdoc />
    public void Load(string path)
    {
        throw new NotImplementedException("TODO: Фаза 1/3 — сериализация World");
    }
}
