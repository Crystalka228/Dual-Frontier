using System;

namespace DualFrontier.Application.Loop;

/// <summary>
/// Главный цикл симуляции. Отвечает за старт/стоп мира и тик систем
/// через планировщик ядра. Цикл работает на фиксированном шаге
/// (simulation tick), независимо от FPS рендера.
///
/// Domain → Presentation связь строго однонаправленная: цикл
/// складывает команды в <c>PresentationBridge</c>, Godot читает их
/// из главного потока в <c>_Process</c>.
/// </summary>
public sealed class GameLoop
{
    /// <summary>
    /// TODO: Фаза 1 — стартует симуляцию. Создаёт рабочий поток(и),
    /// инициализирует планировщик и начинает тикать.
    /// </summary>
    public void Start()
    {
        throw new NotImplementedException("TODO: Фаза 1 — главный цикл");
    }

    /// <summary>
    /// TODO: Фаза 1 — останавливает симуляцию. Ждёт завершения текущего
    /// тика, сохраняет состояние при необходимости, освобождает ресурсы.
    /// </summary>
    public void Stop()
    {
        throw new NotImplementedException("TODO: Фаза 1 — главный цикл");
    }

    /// <summary>
    /// TODO: Фаза 1 — выполняет один тик симуляции. Вызывает
    /// планировщик систем с заданным <paramref name="delta"/>.
    /// </summary>
    /// <param name="delta">Прошедшее время в секундах с последнего тика.</param>
    public void Tick(float delta)
    {
        throw new NotImplementedException("TODO: Фаза 1 — главный цикл");
    }
}
