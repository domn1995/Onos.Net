using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// A depth-first search implementation via iteration rather than recursion.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class DepthFirstSearch<V, E> : AbstractGraphPathSearch<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        /// <summary>
        /// Enumerates the possible types of edges in a depth first search.
        /// </summary>
        public enum EdgeType
        {
            /// <summary>
            /// Represents a tree edge.
            /// </summary>
            TreeEdge,
            /// <summary>
            /// Represents a forward edge.
            /// </summary>
            ForwardEdge,
            /// <summary>
            /// Represents a back edge.
            /// </summary>
            BackEdge,
            /// <summary>
            /// Represents a cross edge.
            /// </summary>
            CrossEdge,
        }

        /// <inheritdoc/>
        protected override IResult<V, E> InternalSearch(IGraph<V, E> graph, V src, V dst, IEdgeWeigher<V, E> weigher, int maxPaths = -1)
        {
            // Prepare the search result.
            SpanningTreeResult result = new SpanningTreeResult(src, dst, maxPaths);

            // The source vertex has cost 0, of course.
            result.UpdateVertex(src, default, weigher.GetInitialWeight(), true);

            // Track finished vertices and keep a stack of vertices that have
            // been started. Start this stack with the source on it.
            var finished = new HashSet<V>();
            Stack<V> stack = new Stack<V>();
            stack.Push(src);

            while (stack.Count > 0)
            {
                V vertex = stack.Peek();
                if (vertex == dst)
                {
                    // If we have reached our destination, we're done.
                    break;
                }

                IWeight cost = result.GetCost(vertex);
                bool tangent = false;

                // Visit all egress edges of the current vertex.
                foreach (E edge in graph.GetEdgesFrom(vertex))
                {
                    // If we have seen the edge already, skip it.
                    if (result.IsEdgeMarked(edge))
                    {
                        continue;
                    }

                    // Examine the destination of the current edge.
                    V nextVertex = edge.Dst;
                    if (!result.HasCost(nextVertex))
                    {
                        // If this searched has not finished this vertex yet
                        // and not started it, then start it as a tree edge.
                        result.MarkEdge(edge, EdgeType.TreeEdge);
                        IWeight newCost = cost.Merge(weigher.GetWeight(edge));
                        result.UpdateVertex(nextVertex, edge, newCost, true);
                        stack.Push(nextVertex);
                        tangent = true;
                        break;
                    }
                    else if (!finished.Contains(nextVertex))
                    {
                        // We started the vertex, but did not finish it yet,
                        // so it must be a back edge.
                        result.MarkEdge(edge, EdgeType.BackEdge);
                    }
                    else
                    {
                        // The target has been finished already, so what we
                        // have here is either a forward edge or a cross edge.
                        EdgeType type = IsForwardEdge(result, edge) ? EdgeType.ForwardEdge : EdgeType.CrossEdge;
                        result.MarkEdge(edge, type);
                    }
                }

                // If we have not been sent on a tangent search and reached the end of the current
                // scan normally, mark the node as finished and pop it off the vertex stack.
                if (!tangent)
                {
                    finished.Add(vertex);
                    stack.Pop();
                }
            }

            // Finally, build the paths on the search result and return it.
            result.BuildPaths();
            return result;
        }

        /// <summary>
        /// Determines whether the given edge is a forward edge using
        /// the accumulated set of parent edges for each vertex.
        /// </summary>
        /// <param name="result">The search result.</param>
        /// <param name="edge">The edge to be classified.</param>
        /// <returns>True if the edge is a forward edge, otherwise false.</returns>
        protected bool IsForwardEdge(DefaultResult result, E edge)
        {
            V target = edge.Src;
            V vertex = edge.Dst;
            ISet<E> parentEdges;
            while ((parentEdges = result.Parents[vertex]) != null)
            {
                foreach (E parentEdge in parentEdges)
                {
                    vertex = parentEdge.Src;
                    if (vertex == target)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// Graph search result which includes edge classification for building a spanning tree.
        /// </summary>
        protected class SpanningTreeResult : DefaultResult
        {
            /// <summary>
            /// Returns the map of edge types.
            /// </summary>
            public IDictionary<E, EdgeType> Edges { get; } = new Dictionary<E, EdgeType>();

            /// <summary>
            /// Initializes a new <see cref="SpanningTreeResult"/> instance with the given values.
            /// </summary>
            /// <param name="src">The source vertex.</param>
            /// <param name="dst">The destination vertex.</param>
            /// <param name="maxPaths">The limit on the number of paths.</param>
            public SpanningTreeResult(V src, V dst, int maxPaths = 1) : base(src, dst, maxPaths)
            {

            }

            /// <summary>
            /// Determines whether or not the edge has already been marked with an edge type.
            /// </summary>
            /// <param name="edge">The edge to test.</param>
            /// <returns>True if the edge has already been marked, otherwise false.</returns>
            public bool IsEdgeMarked(E edge) => Edges.ContainsKey(edge);

            /// <summary>
            /// Marks the given edge with the given edge type.
            /// </summary>
            /// <param name="edge">The edge to mark.</param>
            /// <param name="type">The type with which to mark the given edge.</param>
            public void MarkEdge(E edge, EdgeType type) => Edges[edge] = type;
        }
    }    
}