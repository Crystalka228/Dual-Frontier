namespace DualFrontier.Application.Scenario;

/// <summary>
/// Неизменяемое описание стартового сценария. Читается
/// <see cref="ScenarioLoader"/> с диска и потребляется инициализатором мира.
/// </summary>
public sealed class ScenarioDef
{
    /// <summary>
    /// TODO: Фаза 3 — человеко-читаемое имя сценария (UI список).
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// TODO: Фаза 3 — зерно генератора псевдослучайных чисел.
    /// Фиксированное зерно обеспечивает воспроизводимую генерацию мира.
    /// </summary>
    public int Seed { get; init; }

    /// <summary>
    /// TODO: Фаза 3 — идентификатор биома стартовой локации.
    /// </summary>
    public string Biome { get; init; } = string.Empty;

    /// <summary>
    /// TODO: Фаза 3 — количество пешек, с которых начинается колония.
    /// </summary>
    public int StartingPawns { get; init; }
}
