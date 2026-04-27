using System;

namespace DualFrontier.Application.Save;

/// <summary>
/// Implementation of <see cref="ISaveSystem"/>. Serialises <c>World</c> and
/// all of its component stores into a binary format with a versioned header
/// (<see cref="SaveFormat"/>).
/// </summary>
public sealed class SaveSystem : ISaveSystem
{
    /// <inheritdoc />
    public void Save(string path)
    {
        throw new NotImplementedException("TODO: Phase 1/3 — World serialisation");
    }

    /// <inheritdoc />
    public void Load(string path)
    {
        throw new NotImplementedException("TODO: Phase 1/3 — World serialisation");
    }
}
