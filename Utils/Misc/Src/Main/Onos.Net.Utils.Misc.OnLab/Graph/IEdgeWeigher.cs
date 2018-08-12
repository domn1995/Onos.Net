namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Enables implementing classes to represent a graph edge weight function.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">THe edge type.</typeparam>
    public interface IEdgeWeigher<V, E> where V : IVertex where E : IEdge<V>
    {
        /// <summary>
        /// Returns the weight of the given edge.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        IWeight GetWeight(E edge);

        /// <summary>
        /// Returns the initial weight value (i.e. the weight of a "path" starting and
        /// terminating in the same vertex, typically 0).
        /// </summary>
        /// <returns></returns>
        IWeight InitialWeight { get; }

        /// <summary>
        /// Returns the weight of link/path that should be skipped.
        /// Can be considered as an infinite weight.
        /// </summary>
        /// <returns></returns>
        IWeight NonViableWeight { get; }
    }
}