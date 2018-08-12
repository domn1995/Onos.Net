namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Enables implementing classes to represent a mutable path that allows gradual construction.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public interface IMutablePath<V, E> : IPath<V, E> where V : IVertex where E : IEdge<V>
    {
        /// <summary>
        /// Inserts a new edge at the beginning of this path. 
        /// The edge must be adjacent to the prior start of the path.
        /// </summary>
        /// <param name="edge">The edge to insert.</param>
        void InsertEdge(E edge);

        /// <summary>
        /// Appends a new edge at the end of this path.
        /// The edge must be adjacent to the prior end of the path.
        /// </summary>
        /// <param name="edge">The edge to append.</param>
        void AppendEdge(E edge);
        
        /// <summary>
        /// Removes the given edge.
        /// This edge must be either at the start, the end of the path,
        /// or a cyclic edge in order not to violate the contiguous path property.
        /// </summary>
        /// <param name="edge">The edge to remove.</param>
        void RemoveEdge(E edge);

        /// <summary>
        /// Returns an immutable copy of this path.
        /// </summary>
        /// <returns>The immutable copy of this path.</returns>
        IPath<V, E> ToImmutable();
    }
}
