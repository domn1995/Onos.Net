using System;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Enables implementing classes to represent a graph edge weight.
    /// </summary>
    public interface IWeight : IComparable<IWeight>
    {
        /// <summary>
        /// Gets the weight's value.
        /// </summary>
        double Value { get; }

        /// <summary>
        /// Merges the given weight with this one, returning a new aggregrated weight.
        /// </summary>
        /// <param name="otherWeight">The weight to merge.</param>
        /// <returns>The aggregated weight.</returns>
        IWeight Merge(IWeight otherWeight);

        /// <summary>
        /// Subtracts the given weight from this one and produces a new weight.
        /// </summary>
        /// <param name="otherWeight">The weight to subtract.</param>
        /// <returns>The residual weight.</returns>
        IWeight Subtract(IWeight otherWeight);

        /// <summary>
        /// Gets whether the weighted subject can be traversed.
        /// Returns true if the weight is adequate, false if the weight is infinite.
        /// </summary>
        bool IsViable { get; }

        /// <summary>
        /// Gets whether the weight is negative.
        /// This means that the aggregated path cost will decrease
        /// if we add a weighted subject to it.
        /// Returns true if the weight is negative, false otherwise.
        /// </summary>
        bool IsNegative { get; }
    }
}
