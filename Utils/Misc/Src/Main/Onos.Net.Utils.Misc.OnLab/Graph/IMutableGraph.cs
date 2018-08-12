namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Enables implementing classes to represent a mutable graph that can be constructed gradually.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public interface IMutableGraph<V, E> : IGraph<V, E> where V : IVertex where E : IEdge<V>
    {
        /// <summary>
        /// Adds the given vertex to this graph.
        /// </summary>
        /// <param name="vertex">The vertex to add.</param>
        void AddVertex(V vertex);

        /// <summary>
        /// Removes the given vertex from this graph.
        /// </summary>
        /// <param name="vertex">The vertex to remove.</param>
        void RemoveVertex(V vertex);

        /// <summary>
        /// Adds the given edge to this graph. If the edge vertices are not
        /// already in the graph, they will also be added.
        /// </summary>
        /// <param name="edge">The edge to add.</param>
        void AddEdge(E edge);

        /// <summary>
        /// Removes the specified edge from the graph.
        /// </summary>
        /// <param name="edge">The edge to remove.</param>
        void RemoveEdge(E edge);

        /// <summary>
        /// Returns an immutable copy of this graph.
        /// </summary>
        /// <returns>The immutable copy of this graph.</returns>
        IGraph<V, E> ToImmutable();
    }
}
