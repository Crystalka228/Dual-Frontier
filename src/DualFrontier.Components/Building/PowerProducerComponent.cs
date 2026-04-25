using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Building
{
    /// <summary>
    /// Power producer. Generates watts for the electric grid each tick.
    /// Written exclusively by ElectricGridSystem.
    /// </summary>
    public sealed class PowerProducerComponent : IComponent
    {
        /// <summary>Maximum watts this producer can output.</summary>
        public float MaxWatts;

        /// <summary>Current watts being output (0..MaxWatts).</summary>
        public float CurrentWatts;

        /// <summary>Whether this producer is active and generating power.</summary>
        public bool IsActive = true;

        /// <summary>Fuel efficiency multiplier (1.0 = normal).</summary>
        public float Efficiency = 1.0f;
    }
}