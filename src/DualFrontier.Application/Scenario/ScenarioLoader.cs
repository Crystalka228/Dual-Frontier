using System;

namespace DualFrontier.Application.Scenario;

/// <summary>
/// Загружает описание стартового сценария (<see cref="ScenarioDef"/>)
/// с диска. Используется при старте новой игры.
/// </summary>
public sealed class ScenarioLoader
{
    /// <summary>
    /// TODO: Фаза 3 — парсит файл сценария по указанному пути и возвращает
    /// неизменяемое описание для инициализации мира.
    /// </summary>
    /// <param name="path">Полный путь к файлу сценария.</param>
    public ScenarioDef Load(string path)
    {
        throw new NotImplementedException("TODO: Фаза 3 — парсер сценариев");
    }
}
