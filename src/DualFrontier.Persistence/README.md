# DualFrontier.Persistence

## Назначение

Алгоритмы сжатия для сохранений. Сборка движкового слоя — **не зависит**
от `Core`, `Systems` или `Application`; знает только `Contracts`
(для `EntityId`) и `Components` (для `TerrainKind`, `TileComponent` и т.п.).

Никакого ввода-вывода. Вся работа — `byte[] → byte[]` в памяти. Файловую
запись и интеграцию с `GameLoop` подключает `SaveSystem` в `Application` —
этот пакет даёт ему лежащие в основе кодеки.

## Слои внутри

```
Snapshots/      ← read-only снимки мира (data only, без логики)
  ├── SaveMeta            — заголовок: версия, тик, размер карты
  ├── TileMapSnapshot     — flat row-major TerrainKind[]
  ├── PawnSnapshot        — позиция + 5 нужд + job
  ├── StorageSnapshot     — capacity + dictionary стаков
  └── WorldSnapshot       — композиция всего перечисленного

Compression/    ← кодеки, по одному файлу на алгоритм
  ├── TileEncoder         — RLE по тайловой карте
  ├── ComponentEncoder    — квантизация float [0..1] в byte
  ├── EntityEncoder       — range encoding по EntityId.Index
  ├── StringPool          — дедупликация строковых тегов
  └── DfCompressor        — фасад/точка входа
```

## Алгоритмы

### TileEncoder — RLE

Карта 100×100 = 10000 тайлов. Naïve байтовое кодирование = 10000 байт.

Формат: пары `(count: ushort)(kind: byte)`. Прогон длиннее `ushort.MaxValue`
(65 535) разбивается на части. Если 95% тайлов — `Grass`, размер падает
до единиц десятков байт.

```csharp
var bytes = TileEncoder.Encode(snapshot);    // ~30 байт для homogenous биома
var back  = TileEncoder.Decode(bytes, w, h); // round-trip восстанавливает оригинал
```

### ComponentEncoder — квантизация

`NeedsComponent.Hunger` и подобные float-поля живут в `[0, 1]`. Quantisation
в `byte`:

```csharp
byte q = ComponentEncoder.QuantizeFloat(0.75f);     // 191
float v = ComponentEncoder.DequantizeFloat(q);      // 0.7490..
```

Худшая ошибка round-trip — `1/255 ≈ 0.004`, ниже разрешения, на которое
смотрит любая из систем нужд/настроения.

Delta-encoding между сохранениями (запись только изменённых полей с
`fieldMask: byte` и списком значений) запланирован как часть полной
интеграции `DfCompressor.Compress(WorldSnapshot)` в более поздней фазе.

### EntityEncoder — range encoding

`EntityId.Index` монотонно растёт; живые сущности образуют редкие
плотные диапазоны. Вместо «один int на сущность» — пары
`(start: int)(count: ushort)`:

```
[1, 2, 3, 4, 5, 10, 11, 12]
  → (start=1, count=5), (start=10, count=3)
  → 12 байт вместо 32
```

`Version` теряется — сохранения снимают момент, при загрузке у каждой
восстановленной сущности будет свежая версия. Внутренние ссылки в саве
работают по `Index`, а не по полному `EntityId`.

### StringPool — дедупликация

`StorageComponent.Items` использует строки `iron_ore`, `wood`, `cloth`
как ключи; в крупной колонии одни и те же строки повторяются десятки
тысяч раз. Пул интернирует строку при первом обращении и возвращает
16-битный `ushort` handle.

Формат сериализации: `[count: ushort]` затем по каждой строке
`[len: ushort][bytes: utf8]`. Емкость пула ограничена `ushort.MaxValue`
(65 535) уникальных строк — при переполнении кодек кричит, а не теряет
данные молча.

## Граница

```
DualFrontier.Persistence
  ├── зависит от: Contracts, Components
  └── НЕ зависит от: Core, Systems, AI, Application, Presentation
```

Это даёт три гарантии:
1. Алгоритмы тестируются без ECS-инфраструктуры.
2. Persistence можно загрузить как часть мод-API без подтаскивания всего
   движка.
3. Пакет реюзаем в инструментах вне основной сборки (mod editor,
   save-inspector).

## Тесты

`tests/DualFrontier.Persistence.Tests/` — четыре round-trip-теста:

- `TileEncoder_RLE_roundtrip` — 100×100 95% Grass + 5% Rock; проверка
  идентичности и `encoded.Length < 100`.
- `ComponentEncoder_quantize_roundtrip` — `0.75f → byte → float`,
  ошибка `< 0.005f`.
- `EntityEncoder_range_roundtrip` — индексы `{1..5, 10..12}`, проверка
  идентичности и `encoded.Length < 32`.
- `StringPool_intern_dedup` — повторный `Intern` возвращает тот же
  handle и не растит пул.
