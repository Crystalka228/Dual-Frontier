namespace DualFrontier.Application.Bridge;

/// <summary>
/// Базовый интерфейс команды рендера, передаваемой из домена в
/// Godot-слой. <see cref="Execute"/> вызывается строго в главном потоке
/// Godot и получает корневую сцену как <c>object</c> (Application не может
/// иметь ссылку на <c>Godot.Node</c>; приведение выполняет Presentation).
/// </summary>
public interface IRenderCommand
{
    /// <summary>
    /// TODO: Фаза 3 — применить команду к сцене Godot. Вызывается только
    /// из главного потока.
    /// </summary>
    /// <param name="godotScene">Корневая сцена Godot (cast в нужный тип на Presentation).</param>
    void Execute(object godotScene);
}
