# BehaviourTree

## Назначение
Универсальный behaviour tree для пешек и юнитов. BT не хранит
состояние мира — он принимает `BTContext`, читает из него
данные пешки и возвращает статус (Running / Success / Failure).

## Зависимости
- `DualFrontier.Contracts` — `EntityId`.
- `DualFrontier.Components` — через `BTContext` (не напрямую
  импортировать в ноды).

## Что внутри
- `BTNode.cs` — базовый абстрактный класс + enum `BTStatus`.
- `BTContext.cs` — контекст одного тика BT (entity, сервисы).
- `Selector.cs` — "пробовать по очереди, пока не получится".
- `Sequence.cs` — "делать по очереди, пока все успешны".
- `Leaf.cs` — базовый класс для конкретных действий/условий.

## Правила
- Ноды — чисты: никакого глобального состояния, никаких
  синглтонов. Всё состояние передаётся через `BTContext`.
- Никакого `async/await` — BT синхронный, дорогие операции
  (поиск пути) делим на тики.
- `Leaf` — конкретная работа (идти к точке, съесть еду).
- `Selector` / `Sequence` — композиты, возвращают Running,
  пока ребёнок Running.

## Примеры использования
```csharp
var root = new Selector(
    new Sequence(new IsHungryLeaf(), new EatLeaf()),
    new IdleLeaf()
);
var status = root.Tick(ctx);
```

## TODO
- [ ] Реализовать `Selector.Tick` / `Sequence.Tick` с запоминанием
      текущего ребёнка.
- [ ] Реализовать `Leaf` как абстрактный с полями для отладчика.
- [ ] Добавить `BTBlackboard` для локального состояния пешки.
- [ ] Парсер BT из JSON для модов.
