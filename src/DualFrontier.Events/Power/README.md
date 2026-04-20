# Power Events

## Назначение
События энергосети (электричество и эфир): запросы мощности, выдача,
перегрузка. Тот же двухшаговый Intent-паттерн, что и для маны/патронов,
но на уровне построек.

## Зависимости
- `DualFrontier.Contracts` — `IEvent`, `EntityId`.

## Что внутри
- `PowerRequestEvent.cs` — постройка запрашивает мощность на тик.
- `PowerGrantedEvent.cs` — PowerSystem подтверждает выдачу.
- `GridOverloadEvent.cs` — перегрузка сети: потребление превышает выработку.

## Правила
- Две независимые сети: Electric и Ether. События содержат `PowerType`,
  чтобы PowerSystem разводила потоки.
- При `GridOverloadEvent` потребители с низким приоритетом отключаются;
  порядок отключения — в PowerSystem.
- Отсутствие `PowerGrantedEvent` в течение тика = постройка не работала;
  производные системы (WorkbenchSystem) обязаны это учитывать.

## Примеры использования
```csharp
_bus.Publish(new PowerRequestEvent { /* ConsumerId = forge, Type = PowerType.Ether, Watts = 50 */ });
// → PowerSystem при успехе публикует PowerGrantedEvent, при дефиците — GridOverloadEvent
```

## TODO
- [ ] Решить: отдельные `GridOverload` для Electric и Ether или общий enum в теле.
- [ ] Добавить `PowerShutdownEvent` для принудительного отключения (Фаза 4).
