# Loop — Главный цикл игры

## Назначение
Главный цикл симуляции. `GameLoop` тикает систем через планировщик `Core`,
поддерживает фиксированный шаг времени (simulation tick) независимо от FPS
рендера. `FrameClock` — часовой таймер, источник `delta` и времени.

## Зависимости
- `DualFrontier.Core` — `World`, `ParallelSystemScheduler`, `GameServices`
- `DualFrontier.Contracts` — `IGameServices`

## Что внутри
- `GameLoop.cs` — запуск/остановка/тик симуляции.
- `FrameClock.cs` — источник времени (`Now`, `DeltaTime`).

## Правила
- Симуляция тикает с **фиксированным** шагом (цель — 30 tps по TechArch).
  Рендер и ввод живут на другой частоте — они не управляют `Tick`.
- Цикл НЕ вызывает Godot. Вместо этого он складывает команды в
  `PresentationBridge`, который Godot читает в своём `_Process`.

## Примеры использования
```csharp
var clock = new FrameClock();
var loop  = new GameLoop(services, bridge);
loop.Start();
while (running)
{
    loop.Tick(clock.DeltaTime);
}
```

## TODO
- [x] Фаза 1 — фиксированный accumulator-based tick (30 Hz).
- [x] Фаза 1 — пауза / speed-multiplier (x1/x2/x3).
- [x] Фаза 3 — `GameBootstrap` реализован, `NavGrid` и
      `MovementSystem` подключены.
