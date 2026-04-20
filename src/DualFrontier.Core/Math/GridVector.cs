using System;

namespace DualFrontier.Core.Math;

/// <summary>
/// Неизменяемая целочисленная координата тайла. Аналог Godot.Vector2I, но
/// без зависимости на Godot namespace (Core не знает о Godot).
/// </summary>
public readonly record struct GridVector(int X, int Y)
{
    /// <summary>Нулевой вектор — (0, 0).</summary>
    public static GridVector Zero => default;

    /// <summary>
    /// TODO: Фаза 1 — евклидова длина (как double, чтобы не терять точность
    /// при длинных расстояниях). Для сравнений предпочтительнее
    /// <see cref="Manhattan"/> или сравнение квадратов длин.
    /// </summary>
    public double Length
        => throw new NotImplementedException("TODO: Фаза 1 — GridVector.Length");

    /// <summary>
    /// TODO: Фаза 1 — манхэттенское расстояние до другой точки.
    /// Основная метрика для grid-мира и A*.
    /// </summary>
    public int Manhattan(GridVector other)
        => throw new NotImplementedException("TODO: Фаза 1 — GridVector.Manhattan");

    /// <summary>
    /// TODO: Фаза 1 — покомпонентное сложение.
    /// </summary>
    public static GridVector operator +(GridVector a, GridVector b)
        => throw new NotImplementedException("TODO: Фаза 1 — operator+");

    /// <summary>
    /// TODO: Фаза 1 — покомпонентное вычитание.
    /// </summary>
    public static GridVector operator -(GridVector a, GridVector b)
        => throw new NotImplementedException("TODO: Фаза 1 — operator-");
}
