using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Implementation of the Dijkstra shortest-path graph search algorithm, 
    /// capable of finding all shortest paths between the source and destination.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class DijkstraGraphSearch<V, E> : AbstractGraphPathSearch<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        /// <inheritdoc/>
        protected override IResult<V, E> InternalSearch(IGraph<V, E> graph, V src, V dst, IEdgeWeigher<V, E> weigher, int maxPaths = -1)
        {
            // Use the default result to remember cumulative costs and
            // parent edges to each respective vertex.
            DefaultResult result = new DefaultResult(src, dst, maxPaths);

            // Cost to reach the source vertex is 0, of course.
            result.UpdateVertex(src, default, weigher.InitialWeight, false);

            if (graph.Edges.Count == 0)
            {
                result.BuildPaths();
                return result;
            }

            // Use the min priority queue to progressively find each
            // nearest vertex until we reach the desired destination, 
            // if one was given, or until we reach all possible destinations.
            Heap<V> minQueue = new Heap<V>(graph.Vertices, new PathCostComparer(result));

            while (minQueue.Count > 0)
            {
                // Get the nearest vertex.
                V nearest = minQueue.ExtractExtreme();
                if (nearest == dst)
                {
                    break;
                }

                // Find its cost and use it to determine if the vertex is reachable.
                if (result.HasCost(nearest))
                {
                    IWeight cost = result.GetCost(nearest);

                    // If the vertex is reachable, relax all its egress edges.
                    foreach (E e in graph.GetEdgesFrom(nearest))
                    {
                        result.RelaxEdge(e, cost, weigher, true);
                    }
                }

                // Reprioritize the min queue.
                minQueue.Heapify();
            }

            // Build a set of paths from the results and return it.
            result.BuildPaths();
            return result;
        }

        /// <summary>
        /// Compares path weights using their accrued costs.
        /// Used for sorting the min priority queue.
        /// </summary>
        private class PathCostComparer : IComparer<V>
        {
            private readonly DefaultResult result;

            public PathCostComparer(DefaultResult result) => this.result = result;

            public int Compare(V v1, V v2)
            {
                // Vertices not accessed should be pushed to the back of the queue.
                if (!result.HasCost(v1) && !result.HasCost(v2))
                {
                    return 0;
                }
                else if (!result.HasCost(v1))
                {
                    return -1;
                }
                else if (!result.HasCost(v2))
                {
                    return 1;
                }

                return result.GetCost(v2).CompareTo(result.GetCost(v1));
            }
        }
    }
}
