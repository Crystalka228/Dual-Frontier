using System;
using DualFrontier.Contracts.Analyzer;

namespace DualFrontier.Application.Save;

/// <summary>
/// Implementation of <see cref="ISaveSystem"/>. Serialises <c>World</c> and
/// all of its component stores into a binary format with a versioned header
/// (<see cref="SaveFormat"/>).
/// </summary>
public sealed class SaveSystem : ISaveSystem
{
    /// <inheritdoc />
    [ReservedStub(
        ReservedStubPurpose.BuildComposition,
        "Application Save Phase 1/3 roadmap stub (Lesson #N12 sub-pattern A) — World serialisation " +
        "к binary format with versioned header per SaveFormat. " +
        "Activation: Phase 1/3 save-system integration.")]
    public void Save(string path)
    {
        throw new NotImplementedException("TODO: Phase 1/3 — World serialisation");
    }

    /// <inheritdoc />
    [ReservedStub(
        ReservedStubPurpose.BuildComposition,
        "Application Save Phase 1/3 roadmap stub (Lesson #N12 sub-pattern A) — World " +
        "deserialisation (inverse of Save; same versioned header semantics). " +
        "Activation: Phase 1/3 save-system integration.")]
    public void Load(string path)
    {
        throw new NotImplementedException("TODO: Phase 1/3 — World serialisation");
    }
}
