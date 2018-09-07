using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Implementation of the breadth-first search algorithm.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class BreadthFirstSearch<V, E> : AbstractGraphPathSearch<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        /// <inheritdoc/>
        protected override IResult<V, E> InternalSearch(IGraph<V, E> graph, V src, V dst, IEdgeWeigher<V, E> weigher, int maxPaths = -1)
        {
            // Prepare the graph result.
            var result = new DefaultResult(src, dst, maxPaths);

            // Set up the starting frontier with the source as the sole vertex.
            var frontier = new HashSet<V>();
            result.UpdateVertex(src, default, weigher.InitialWeight, true);
            frontier.Add(src);

            bool reachedEnd = false;
            while (!reachedEnd && frontier.Count > 0)
            {
                // Prepare the next frontier.
                var next = new HashSet<V>();

                // Visit all vertices in the current frontier.
                foreach (V vertex in frontier)
                {
                    IWeight cost = result.GetCost(vertex);

                    // Visit all egress edges of the current frontier vertex.
                    foreach (E edge in graph.GetEdgesFrom(vertex))
                    {
                        V nextVertex = edge.Dst;
                        if (!result.HasCost(nextVertex))
                        {
                            // If this vertex has not been visited yet, updated it.
                            IWeight newCost = cost.Merge(weigher.GetWeight(edge));
                            result.UpdateVertex(nextVertex, edge, newCost, true);

                            // If we have reached our intended destination, bail.
                            if (nextVertex == dst)
                            {
                                reachedEnd = true;
                                break;
                            }
                            next.Add(nextVertex);
                        }

                        if (reachedEnd)
                        {
                            break;
                        }
                    }
                }

                // Promote the next frontier.
                frontier = next;
            }

            // Finally, build the paths on the search result and return it.
            result.BuildPaths();
            return result;
        }
    }
}