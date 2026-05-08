using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Building
{
    /// <summary>
    /// Power consumer. Draws watts from the electric grid each tick.
    /// Written exclusively by ElectricGridSystem.
    /// </summary>
    public struct PowerConsumerComponent : IComponent
    {
        /// <summary>Power required to operate normally (watts).</summary>
        public float RequiredWatts;

        /// <summary>Whether this consumer is currently receiving power.</summary>
        public bool IsPowered;

        /// <summary>Priority for power allocation. Higher = served first.</summary>
        public int Priority;
    }
}
