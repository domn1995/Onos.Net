namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    public class KShortestPathsSearch<V, E> : AbstractGraphPathSearch<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        protected override IResult<V, E> InternalSearch(IGraph<V, E> graph, V src, V dst, IEdgeWeigher<V, E> weigher, int maxPaths = -1)
        {
            throw new System.NotImplementedException();
        }
    }
}
