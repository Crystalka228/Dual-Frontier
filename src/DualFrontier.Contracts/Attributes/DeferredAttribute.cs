using System;

namespace DualFrontier.Contracts.Attributes;

/// <summary>
/// Помечает событие как "отложенное". Шина накапливает такие события в
/// текущей фазе и доставляет их подписчикам в следующей фазе планировщика.
/// Это безопасный режим для событий, меняющих состояние мира (смерти,
/// удаления сущностей), — чтобы не нарушить инварианты выполняющихся систем.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class DeferredAttribute : Attribute
{
}
