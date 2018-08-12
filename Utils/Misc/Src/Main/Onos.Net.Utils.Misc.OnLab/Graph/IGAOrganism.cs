using System;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Enables implementing classes to represent an "organism", a specific solution
    /// to a problem that can be evaluated in terms of fitness. These organisms can 
    /// be used to represent any class of problem that genetic algorithms can solve.
    /// </summary>
    public interface IGAOrganism
    {
        /// <summary>
        /// Gets a fitness function that determines how optimal a given organism is.
        /// </summary>
        IComparable Fitness { get; }

        /// <summary>
        /// Slightly mutatates an organism.
        /// </summary>
        void Mutate();

        /// <summary>
        /// Creates a new random organism.
        /// </summary>
        /// <returns>A new random organism.</returns>
        IGAOrganism GetRandom();

        /// <summary>
        /// Returns a child organism that is the result
        /// of "crossing" this organism with another.
        /// </summary>
        /// <param name="other">The other organism with which to cross.</param>
        /// <returns>A child organism.</returns>
        IGAOrganism CrossWith(IGAOrganism other);
    }
}
