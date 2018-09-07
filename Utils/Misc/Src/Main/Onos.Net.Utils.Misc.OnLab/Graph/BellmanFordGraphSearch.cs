namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Bellman-Ford graph search algorithm for locating shortest paths
    /// in directed graphs that may contain negative cycles.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class BellmanFordGraphSearch<V, E> : AbstractGraphPathSearch<V, E> where V : class, IVertex where E : class, IEdge<V>
    {

        /// <inheritdoc/>
        protected override IResult<V, E> InternalSearch(IGraph<V, E> graph, V src, V dst, IEdgeWeigher<V, E> weigher, int maxPaths = -1)
        {
            // Prepare the graph search result.
            var result = new DefaultResult(src, dst, maxPaths);
            // The source vertex has cost 0, of course.
            result.UpdateVertex(src, default, weigher.InitialWeight, true);
            int max = graph.Vertices.Count - 1;
            for (int i = 0; i < max; ++i)
            {
                // If possible, relax all egress edges of the current vertex.
                foreach (E edge in graph.Edges)
                {
                    if (result.HasCost(edge.Src))
                    {
                        result.RelaxEdge(edge, result.GetCost(edge.Src), weigher);
                    }
                }
            }

            // Remove any vertices reached by traversing edges with negative weights.
            foreach (E edge in graph.Edges)
            {
                if (result.HasCost(edge.Src))
                {
                    if (result.RelaxEdge(edge, result.GetCost(edge.Src), weigher))
                    {
                        result.RemoveVertex(edge.Dst);
                    }
                }
            }

            // Finally, build the paths on the search result and return it.
            result.BuildPaths();
            return result;
        }
    }
}