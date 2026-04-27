using System;

namespace DualFrontier.Contracts.Core;

/// <summary>
/// Marker interface for a command — an imperative action on the domain.
/// Unlike an event (the fact that something "has happened"), a command is
/// "please do X". Commands are addressed to a specific handler.
/// </summary>
public interface ICommand
{
}
