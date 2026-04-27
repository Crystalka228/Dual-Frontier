using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Pawn-domain bus. Events: mood break, death reaction, skill gain.
/// Writers: <c>NeedsSystem</c>, <c>MoodSystem</c>.
/// Readers: <c>JobSystem</c>, <c>SocialSystem</c>.
/// </summary>
public interface IPawnBus : IEventBus
{
}
