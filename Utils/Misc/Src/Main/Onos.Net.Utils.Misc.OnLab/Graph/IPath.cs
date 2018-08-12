using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Enables implementing classes to represent a path in a graph as a sequence of edges.
    /// Paths are assumed to be continuous, where adjacent edges must share a vertex.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public interface IPath<V, E> : IEdge<V> where V : IVertex where E : IEdge<V>
    {
        /// <summary>
        /// Gets the list of edges comprosing the path. Adjacent edges will
        /// share the same vertex, meaning that a source of one edge will be the
        /// same as the destination of the prior edge.
        /// </summary>
        IList<E> Edges { get; }

        /// <summary>
        /// Gets the total cost of the path as an <see cref="IWeight"/> object.
        /// </summary>
        IWeight Cost { get; }
    }
}
