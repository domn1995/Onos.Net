namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Enables implementing classes to represent a graph search algorithm and its result.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    public interface IGraphSearch<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        /// <summary>
        /// Searches the specified graph.
        /// </summary>
        /// <param name="graph">The graph to search.</param>
        /// <param name="weigher">An optional edge weigher. 
        /// If null, <see cref="DefaultEdgeWeigher{V, E}"/> will be used.</param>
        /// <returns></returns>
        IResultBase<V, E> Search(IGraph<V, E> graph, IEdgeWeigher<V, E> weigher);
    }

    public interface IResultBase<V, E> where V : class, IVertex where E : class, IEdge<V>
    {

    }
}
