using System;

namespace DualFrontier.Contracts.Attributes;

/// <summary>
/// Декларирует границы доступа системы: какие типы компонентов она читает,
/// какие пишет, и в какую доменную шину публикует события.
/// <c>ParallelSystemScheduler</c> использует эту декларацию для построения
/// графа зависимостей, а <c>SystemExecutionContext</c> (сторож изоляции) —
/// для проверок в DEBUG-сборке. См. <c>/docs/THREADING.md</c>
/// и <c>/docs/ISOLATION.md</c> (TechArch раздел 11.6 и 11.7).
///
/// Атрибут не наследуется — каждая система обязана объявить свой контракт явно.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SystemAccessAttribute : Attribute
{
    /// <summary>
    /// Типы компонентов, которые система имеет право читать через
    /// <c>SystemBase.GetComponent&lt;T&gt;</c>.
    /// </summary>
    public Type[] Reads { get; }

    /// <summary>
    /// Типы компонентов, которые система имеет право модифицировать.
    /// Write неявно включает Read.
    /// </summary>
    public Type[] Writes { get; }

    /// <summary>
    /// Имя доменной шины из <c>IGameServices</c>, в которую система публикует
    /// события. Используется <c>nameof(IGameServices.Combat)</c> и подобные —
    /// это даёт compile-time проверку имени. Для мульти-шинных систем —
    /// первая шина из <see cref="Buses"/>.
    /// </summary>
    public string Bus { get; }

    /// <summary>
    /// Полный список шин в которые система пишет или из которых читает.
    /// Введено TechArch v0.2 §12.4: CombatSystem теперь работает с
    /// <c>Combat</c> и <c>Magic</c> одновременно (манна стрелка).
    /// Для одно-шинных систем содержит один элемент.
    /// </summary>
    public string[] Buses { get; }

    /// <summary>
    /// Создаёт атрибут с явно указанными reads/writes и одной шиной.
    /// Совместимо с v0.1-стилем деклараций.
    /// </summary>
    public SystemAccessAttribute(Type[] reads, Type[] writes, string bus)
    {
        Reads = reads ?? Array.Empty<Type>();
        Writes = writes ?? Array.Empty<Type>();
        Bus = bus ?? string.Empty;
        Buses = string.IsNullOrEmpty(Bus) ? Array.Empty<string>() : new[] { Bus };
    }

    /// <summary>
    /// Создаёт атрибут с явно указанными reads/writes и списком шин.
    /// Введено TechArch v0.2 §12.4 для мульти-шинных систем
    /// (<c>CombatSystem</c>, <c>CompositeResolutionSystem</c>, <c>ComboResolutionSystem</c>).
    /// </summary>
    public SystemAccessAttribute(Type[] reads, Type[] writes, string[] buses)
    {
        Reads = reads ?? Array.Empty<Type>();
        Writes = writes ?? Array.Empty<Type>();
        Buses = buses ?? Array.Empty<string>();
        Bus = Buses.Length > 0 ? Buses[0] : string.Empty;
    }
}
