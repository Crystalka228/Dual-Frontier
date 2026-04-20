namespace DualFrontier.Application.Save;

/// <summary>
/// Контракт системы сохранения/загрузки мира. Оба метода синхронные —
/// асинхронность (прогресс, отмена) обязан обеспечить вышестоящий слой.
/// </summary>
public interface ISaveSystem
{
    /// <summary>
    /// TODO: Фаза 1 — сохраняет текущее состояние мира в файл по указанному пути.
    /// </summary>
    /// <param name="path">Полный путь к файлу сохранения.</param>
    void Save(string path);

    /// <summary>
    /// TODO: Фаза 1 — загружает состояние мира из файла. Текущее состояние мира
    /// перезаписывается.
    /// </summary>
    /// <param name="path">Полный путь к файлу сохранения.</param>
    void Load(string path);
}
