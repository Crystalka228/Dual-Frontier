using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Шина домена пешек. События: срыв настроения, реакция на смерть, прирост навыка.
/// Пишут: <c>NeedsSystem</c>, <c>MoodSystem</c>.
/// Читают: <c>JobSystem</c>, <c>SocialSystem</c>.
/// </summary>
public interface IPawnBus : IEventBus
{
}
