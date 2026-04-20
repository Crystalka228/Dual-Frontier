# Интеграция с Godot

Godot — отличный движок для 2D-симуляции, но у него есть жёсткое ограничение: `SceneTree` и `Node` API работают только из главного потока. Domain-логика Dual Frontier многопоточная и не должна знать, что рядом вообще есть Godot. Связь между слоями строится на одностороннем мосте команд.

## Ограничение main thread

Любая попытка вызвать `AddChild`, `QueueFree`, `SetPosition`, `EmitSignal` или `GetTree` из фонового потока в Godot приводит к неопределённому поведению: чаще всего — падение в рандомном месте через несколько кадров, гонка за очередь отложенных сообщений SceneTree. Повторить баг сложно, отлаживать невозможно.

Решение: Domain и Application запускаются в собственных потоках, Presentation живёт исключительно в главном потоке Godot. Коммуникация Domain → Presentation — через `PresentationBridge`. Коммуникация Presentation → Domain — через `InputRouter` и шины.

## PresentationBridge — однонаправленная очередь

`PresentationBridge` принадлежит Application слою. Он хранит `ConcurrentQueue<IRenderCommand>`. Domain пишет в очередь из любого потока; Presentation дренирует очередь в `_Process()` главного потока.

```csharp
public sealed class PresentationBridge
{
    // Domain пишет сюда из любого потока
    private readonly ConcurrentQueue<IRenderCommand> _commands = new();

    // Domain: сообщить о смерти пешки
    public void EnqueuePawnDied(EntityId id, Vector2 position)
        => _commands.Enqueue(new PawnDiedCommand(id, position));

    // Godot _Process() — только main thread
    public void DrainCommands()
    {
        while (_commands.TryDequeue(out var cmd))
            cmd.Execute(_godotScene);
    }
}
```

Ключевые свойства:

- **Однонаправленность.** Presentation пишет в очередь ввода, Domain пишет в очередь render-команд. Но Presentation не читает Domain напрямую никогда.
- **Потокобезопасность.** `ConcurrentQueue` достаточно: пишущих потоков много, читающий — один (main thread).
- **Отсутствие lock contention у Domain.** `Enqueue` — lock-free на всех современных .NET runtime.
- **Backpressure контролируется.** Если Presentation отстаёт, очередь растёт и это видно на графике `_commands.Count`. Разбирать причины — задача PERFORMANCE пайплайна.

## IRenderCommand — список команд

Набор команд Domain → Presentation расширяется по мере разработки, но ядро фиксировано.

```csharp
public interface IRenderCommand : ICommand
{
    void Execute(GodotScene scene);
}
```

| Команда                       | Кто создаёт                           | Что делает                                      |
|-------------------------------|---------------------------------------|-------------------------------------------------|
| `PawnDiedCommand`             | `DamageSystem` → через `Application`  | Анимация смерти, удаление `PawnVisual`          |
| `ProjectileSpawnedCommand`    | `ProjectileSystem`                    | Создание `ProjectileVisual` ноды                |
| `SpellCastCommand`            | `SpellSystem`                         | VFX каста, звук, анимация                       |
| `UIUpdateCommand`             | `MoodSystem`, `NeedsSystem` и др.     | Обновление панели пешки, иконок алертов         |
| `PawnMovedCommand`            | `JobSystem` / movement                | Обновление позиции `PawnVisual`                 |
| `BuildingPlacedCommand`       | `CraftSystem`                         | Спавн ноды здания                               |

Systems не создают команды напрямую — они публикуют доменные события, Application-мост слушает и кладёт команды. Это сохраняет чистоту Domain-слоя: ни одной ссылки на Godot-тип.

## Почему Presentation не зовёт Domain

Прямой вызов из Presentation в Domain сломал бы всё:

1. **Блокировка main thread.** Systems многопоточны; их вызов потребует синхронизации, и main thread будет ждать результат, роняя FPS.
2. **Ломает сторож изоляции.** Presentation не зарегистрирован в `SystemExecutionContext`, вызов `world.GetComponent` через обходной путь обнулит декларации.
3. **Ломает модифицируемость.** Моды расширяют Domain; если Presentation звонит Domain через конкретные имена классов, любая правка мода требует перекомпиляции Presentation.

Правильный поток ввода: `InputRouter` публикует событие в шину, Domain-система это событие обрабатывает, по результату публикует другое событие или кладёт команду в мост.

## InputRouter

`InputRouter` — единственная точка, где Godot-ввод превращается в доменные события. Живёт в `DualFrontier.Presentation/Input`.

```csharp
public partial class InputRouter : Node
{
    [Export] public GameServicesHolder Services { get; set; } = null!;

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            var grid = ScreenToGrid(mouseButton.Position);
            Services.Bus.Pawns.Publish(new SelectPawnIntent { GridPosition = grid });
        }

        if (Input.IsActionJustPressed("build_mode"))
        {
            Services.Bus.World.Publish(new ToggleBuildModeEvent());
        }
    }
}
```

`InputEvent` не утекает в Domain — маппинг клавиши/кнопки в доменное событие происходит в роутере. Это даёт гибкость (новое управление = правки только в роутере) и чистоту (Domain не знает о Godot).

## Жизненный цикл Godot-нод vs Entity

Entity и Godot-нода — два разных объекта с разной продолжительностью жизни.

| Сущность          | Живёт пока                                    | Кто создаёт                           |
|-------------------|-----------------------------------------------|---------------------------------------|
| `EntityId`        | Не вызван `world.DestroyEntity`               | `Application` / системы               |
| Компонент         | Прикреплён к живой entity                     | `Application` / системы               |
| Godot `Node`      | Не вызван `QueueFree`                         | Presentation по команде из моста      |

Правило: ноде нужна entity, но entity может существовать без ноды (например, серверная симуляция, headless-тесты). Нода — визуальная тень entity. Связь — через `EntityId`, хранящийся в ноде как поле.

```csharp
public partial class PawnVisual : Node2D
{
    public EntityId EntityId;

    public override void _Process(double delta)
    {
        // Только чтение PresentationBridge — прямого доступа к World нет.
    }
}
```

При получении `PawnDiedCommand` Presentation находит ноду по `EntityId`, играет анимацию смерти, по завершении — `QueueFree`. Entity к тому моменту уже давно удалена в Domain.

Обратная сторона: если в середине фазы Domain удалил entity, а команда на удаление ноды ещё в очереди — ничего страшного. Команда придёт в следующий `_Process`, нода скажет `QueueFree`, Godot уберёт. Рассинхрон безопасен, потому что направление всегда одно.

## См. также

- [ARCHITECTURE](./ARCHITECTURE.md)
- [THREADING](./THREADING.md)
- [EVENT_BUS](./EVENT_BUS.md)
