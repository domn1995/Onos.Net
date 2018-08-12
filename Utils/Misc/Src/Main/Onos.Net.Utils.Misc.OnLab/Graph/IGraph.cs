using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Enables implementing classes to represent a directed graph structure.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public interface IGraph<V, E> where V : IVertex where E: IEdge<V>
    {
        /// <summary>
        /// Gets the set of vertices comprising the graph.
        /// </summary>
        /// <returns></returns>
        ISet<V> Vertices { get; }

        /// <summary>
        /// Gets the set of edges comprising the graph.
        /// </summary>
        ISet<E> Edges { get; }

        /// <summary>
        /// Returns all the edges leading out from the specified source vertex.
        /// </summary>
        /// <param name="src">The source vertex.</param>
        /// <returns>A set of egress edges. Returns an empty set if no such edges.</returns>
        ISet<E> GetEdgesFrom(V src);

        /// <summary>
        /// Returns all edges leading toward the specified destination vertex.
        /// </summary>
        /// <param name="dst">The destination vertex.</param>
        /// <returns>The set of ingress vertices. Returns an empty set if no such edges.</returns>
        ISet<E> GetEdgesTo(V dst);
    }
}
