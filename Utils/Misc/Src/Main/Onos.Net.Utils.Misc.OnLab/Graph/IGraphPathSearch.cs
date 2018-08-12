namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Enables implementing classes to represent a graph path search algorithm.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public interface IGraphPathSearch<V, E> where V : IVertex where E : IEdge<V>
    {
        /// <summary>
        /// Searches the specified graph for paths between vertices.
        /// </summary>
        /// <param name="graph">The graph to be searched.</param>
        /// <param name="src">An optional source vertex.</param>
        /// <param name="dst">An optional destination vertex.
        /// If null, all destinations will be searched.</param>
        /// <param name="weighter">An optional edge weigher. 
        /// If null, <see cref="DefaultEdgeWeigher{V, E}"/> will be used. </param>
        /// <param name="maxPaths"></param>
        /// <returns></returns>
        IResult<V, E> Search(IGraph<V, E> graph, V src, V dst, IEdgeWeigher<V, E> weighter, int maxPaths = -1);
    }
}
