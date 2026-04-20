using DualFrontier.Contracts.Modding;

namespace DualFrontier.Mod.Example;

/// <summary>
/// Референсный пример мода. Демонстрирует правильный паттерн:
/// импорты только из DualFrontier.Contracts.* — ни одной ссылки на Core/Systems/Components.
/// </summary>
public sealed class ExampleMod : IMod
{
    /// <summary>TODO: Фаза 2 — регистрация компонентов/систем через IModApi.</summary>
    public void Initialize(IModApi api)
    {
        // TODO: api.RegisterComponent<MyCustomComponent>();
        // TODO: api.Subscribe<PawnSpawnedEvent>(OnPawnSpawned);
    }

    /// <summary>TODO: Фаза 2 — отписка и очистка ресурсов при выгрузке.</summary>
    public void Unload()
    {
        // TODO: отписаться от событий.
    }
}
