using System;

namespace DualFrontier.Contracts.Attributes;

/// <summary>
/// Помечает событие как "мгновенное" — прерывает текущую фазу планировщика
/// для немедленной доставки подписчикам. Используется редко: для критических
/// событий, которые нельзя откладывать (например, игровой ивент, требующий
/// немедленной паузы). Чрезмерное использование ломает параллелизм.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class ImmediateAttribute : Attribute
{
}
