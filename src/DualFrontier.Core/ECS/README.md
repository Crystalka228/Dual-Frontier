# ECS — Entity Component System

## Назначение
Ядро ECS архитектуры. World хранит все entities и их компоненты,
ComponentStore обеспечивает O(1) доступ по типу компонента,
SystemExecutionContext — сторож изоляции, который проверяет что
система не обращается к незадекларированным компонентам.

## Зависимости
- DualFrontier.Contracts (интерфейсы IComponent, EntityId)

## Что внутри
- World.cs — реестр всех entities, точка входа для систем
- ComponentStore.cs — типизированный storage для компонентов одного типа
- SystemBase.cs — базовый класс для всех систем игры
- SystemExecutionContext.cs — сторож изоляции (ThreadLocal контекст)
- IsolationViolationException.cs — исключение при нарушении изоляции

## Правила
- World доступен системам ТОЛЬКО через SystemExecutionContext
- Прямой `world.GetComponent<T>()` из системы = краш
- Система обязана декларировать READ/WRITE через [SystemAccess]
- ComponentStore сам по себе НЕ потокобезопасный — защита через
  граф зависимостей на уровне ParallelSystemScheduler

## Примеры использования
```csharp
[SystemAccess(reads: new[] { typeof(HealthComponent) }, writes: new Type[0], bus: nameof(IGameServices.Pawns))]
public class DamageReporterSystem : SystemBase {
    public override void Update(float delta) {
        // foreach (var entity in Query<HealthComponent>()) { ... }
        // var health = GetComponent<HealthComponent>(entity);
        // Доступ разрешён — HealthComponent в reads
    }
}
```

## TODO
- [ ] Реализовать ComponentStore<T> с SparseSet структурой
- [ ] Реализовать генерацию EntityId с версиями
- [ ] Реализовать Query<T1, T2, ...> для поиска entities с набором
- [ ] Реализовать SystemExecutionContext с ThreadLocal
- [ ] Написать тесты на срабатывание IsolationViolationException
