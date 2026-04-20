# Изоляция систем

Тихое нарушение изоляции хуже краша: corrupted state проявится через час игры как непонятный баг. Dual Frontier закладывает активный сторож: каждый доступ к компоненту проходит через `SystemExecutionContext`, незадекларированный доступ = немедленное исключение с диагностикой.

## SystemExecutionContext

Контекст исполнения живёт в `ThreadLocal<SystemExecutionContext>` и активен на всё время вызова `system.Update(delta)`. Планировщик до вызова `PushContext(system)` кладёт в контекст:

- Имя системы (для сообщений).
- Набор разрешённых для чтения типов компонентов.
- Набор разрешённых для записи типов компонентов.
- Имя активной шины.
- Ссылку на `World` (но только внутри контекста — снаружи `internal`).

```csharp
internal T GetComponent<T>(EntityId id) where T : IComponent
{
    #if DEBUG
    if (!_allowedReads.Contains(typeof(T))
     && !_allowedWrites.Contains(typeof(T)))
    {
        throw new IsolationViolationException(
            $"[ИЗОЛЯЦИЯ НАРУШЕНА]\n" +
            $"Система '{_systemName}'\n" +
            $"обратилась к '{typeof(T).Name}'\n" +
            $"без декларации доступа.\n" +
            $"Добавь: [SystemAccess(reads: " +
            $"new[]{{typeof({typeof(T).Name})}})]"
        );
    }
    #endif
    return _world.GetComponentUnsafe<T>(id);
}
```

После `system.Update` планировщик вызывает `PopContext`, что обнуляет `ThreadLocal`-значение. Следующая система увидит чистый контекст.

## DEBUG vs RELEASE

Полный сторож стоит ~2% CPU, и этого слишком много для релиза. Режимы различаются.

### DEBUG (разработка, тесты, CI)

- Проверка чтения: компонент должен быть в `reads` или `writes`.
- Проверка записи: компонент должен быть в `writes`.
- Проверка шины: `Publish` допустим только в `bus`, указанный в атрибуте.
- Проверка async: stack-trace анализируется на наличие `AsyncStateMachine`.
- Проверка прямого доступа: `World`, `ComponentStore`, `GetSystem` — всегда крашат.

### RELEASE (продакшн билд)

- Проверка прямого доступа: `GetSystem` и прямые ссылки на системы — **всегда крашат, даже в Release**. Это семантическое нарушение, не оптимизационное.
- Остальные проверки отключаются через `#if DEBUG`.

Таким образом разработчик пишет код с гарантией, что все ошибки контрактов будут пойманы на этапе разработки. В продакшне остаётся только защита от самых грубых нарушений — чтобы мод-эксперимент не повредил сохранение.

## Типы нарушений

### Обращение к незадекларированному компоненту

Самое частое: система решила «по-быстрому» прочитать компонент, не обновив атрибут.

```csharp
[SystemAccess(reads: new[] { typeof(PositionComponent) })]
public sealed class WrongSystem : SystemBase
{
    public override void Update(float delta)
    {
        foreach (var entity in Query<PositionComponent>())
        {
            var health = GetComponent<HealthComponent>(entity); // краш
        }
    }
}
```

Сообщение:

```
[ИЗОЛЯЦИЯ НАРУШЕНА]
Система 'WrongSystem'
обратилась к 'HealthComponent'
без декларации доступа.
Добавь: [SystemAccess(reads: new[]{typeof(HealthComponent)})]
```

### Прямой доступ к другой системе

Прямой вызов ломает развязку и делает код непредсказуемым при параллельном исполнении.

```csharp
// ПЛОХО
var inventory = GetSystem<InventorySystem>();
```

Сторож кидает (даже в Release):

```
[IsolationViolationException]
Прямой доступ к системам запрещён.
Используй EventBus вместо
прямой ссылки на систему.
```

### Использование World напрямую

`World` — `internal`, но `InternalsVisibleTo` открывает его для `Systems`. Тем не менее сторож следит, чтобы системы не обращались к `World` напрямую — любой вызов `world.CreateEntity()` из `Update` идёт через `SystemBase.CreateEntity`, который зарегистрирует операцию в контексте и проверит права.

```
[IsolationViolationException]
Система 'XxxSystem' вызвала World.GetComponentUnsafe напрямую.
Используй SystemBase.GetComponent / Query вместо прямого World-доступа.
```

### Запись в компонент из чужой шины

`[SystemAccess(bus: nameof(IGameServices.Combat))]` означает, что система публикует только в Combat-шину.

```
[IsolationViolationException]
Система 'CombatSystem' публикует 'ItemAddedEvent'
в шину 'Inventory', но декларирует только 'Combat'.
Либо измени шину события, либо добавь в декларацию системы.
```

## Формат сообщений

Все сообщения строятся по единому шаблону:

```
[ЗАГОЛОВОК]
Система '<имя>'
<что сделала неправильно>
<подсказка, как исправить>
```

Обязательные элементы:

- Имя системы (включая namespace).
- Что именно пошло не так, с типом/полем.
- Готовая подсказка — строка кода, которую можно скопировать в атрибут.

Ошибка без подсказки считается недостаточной: Claude, мод-автор или джуниор должны иметь возможность исправить по стектрейсу без похода в документацию.

## Чеклист добавления системы

Перед `git commit` новой системы пройди чеклист:

1. Класс наследует `SystemBase`.
2. Атрибут `[SystemAccess(reads: [...], writes: [...], bus: ...)]` присутствует.
3. Атрибут `[TickRate(TickRates.XXX)]` задан явно.
4. В `reads`/`writes` перечислены ВСЕ используемые в `Update` компоненты.
5. Bus соответствует реальной публикации внутри обработчиков.
6. `Subscribe()` переопределён и отписывает в `OnDestroy`.
7. Нет `async`/`await`/`Task` в коде системы.
8. Нет прямых ссылок на другие системы или `World`.
9. Unit-тест на сторож: тест, в котором система намеренно нарушает декларацию, ожидает `IsolationViolationException`.
10. Интеграционный тест: система запускается в составе `ParallelSystemScheduler` без ошибок.

Если хотя бы один пункт не выполнен — pull request отклоняется.

## См. также

- [ECS](./ECS.md)
- [THREADING](./THREADING.md)
- [TESTING_STRATEGY](./TESTING_STRATEGY.md)
