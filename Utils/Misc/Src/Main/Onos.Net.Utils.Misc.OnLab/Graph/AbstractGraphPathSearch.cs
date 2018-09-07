using Onos.Net.Utils.Misc.OnLab.Helpers;
using System.Collections.Generic;
using System.Linq;
using static Onos.Net.Utils.Misc.OnLab.Helpers.ArgsChecker;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// A base representation of graph path search algorithms.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public abstract class AbstractGraphPathSearch<V, E> : IGraphPathSearch<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        /// <summary>
        /// On methods that allow the user to choose the number of paths to find,
        /// this number indicates that all paths should be found.
        /// </summary>
        public const int AllPaths = -1;

        /// <summary>
        /// Represents a path search result that uses <see cref="DefaultPath{V, E}"/> to convey the graph paths.
        /// </summary>
        public class DefaultResult : IResult<V, E>
        {
            /// <summary>
            /// Gets the max number of paths to find.
            /// </summary>
            protected int MaxPaths { get; }

            /// <inheritdoc/>
            public virtual V Src { get; }

            /// <inheritdoc/>
            public virtual V Dst { get; }

            /// <inheritdoc/>
            public virtual ISet<IPath<V, E>> Paths { get; } = new HashSet<IPath<V, E>>();

            /// <inheritdoc/>
            public virtual IDictionary<V, ISet<E>> Parents { get; } = new Dictionary<V, ISet<E>>();

            /// <inheritdoc/>
            public virtual IDictionary<V, IWeight> Costs { get; } = new Dictionary<V, IWeight>();

            /// <summary>
            /// Initializes a new <see cref="DefaultResult"/> class with the given values.
            /// </summary>
            /// <param name="src">The source vertex.</param>
            /// <param name="dst">The destination vertex.</param>
            /// <param name="maxPaths">The maximum number of paths to find.</param>
            public DefaultResult(V src, V dst, int maxPaths = 1)
            {
                Src = CheckNotNull(src, "The source vertex cannot be null.");
                Dst = dst;
                MaxPaths = maxPaths;
            }

            /// <summary>
            /// Inidicates whether or not the given vertex has a cost yet.
            /// </summary>
            /// <param name="v">The vertex to test.</param>
            /// <returns>True if the vertex has a cost already.</returns>
            public bool HasCost(V v) => Costs.ContainsKey(v);

            /// <summary>
            /// Gets the current cost to reach the specified vertex.
            /// If the vertex has not been accessed yet, it has no
            /// cost associated with it, and null will be returned.
            /// </summary>
            /// <param name="v">The vertex to search.</param>
            /// <returns>The weight cost to reach the vertex if already accessed, otherwise null.</returns>
            public IWeight GetCost(V v) => Costs.GetOrDefault(v);

            /// <summary>
            /// Updates the cost of the vertex using its existing cost plus the 
            /// cost to traverse the specified edge. If the search is in simple
            /// path mode, only one path will be accrued.
            /// </summary>
            /// <param name="vertex">The vertex to update.</param>
            /// <param name="edge">The edge through which the vertex is reached.</param>
            /// <param name="cost">The current cost to reach the vertex from the source.</param>
            /// <param name="replace">True to indicate that any accrued edges are not to be cleared.
            /// False to indicate that the edge should be added to the previously accrued edges as they yield the same cost.</param>
            public void UpdateVertex(V vertex, E edge, IWeight cost, bool replace)
            {
                Costs.SetOrAdd(vertex, cost);

                if (edge is null)
                {
                    return;
                }

                ISet<E> edges = Parents.GetOrDefault(vertex);
                if (edges == null)
                {
                    edges = new HashSet<E>();
                    Parents.Add(vertex, edges);
                }
                if (replace)
                {
                    edges.Clear();
                }
                if (MaxPaths == AllPaths || edges.Count < MaxPaths)
                {
                    edges.Add(edge);
                }
            }

            /// <summary>
            /// Removes the set of parent edges for the specified vertex.
            /// </summary>
            /// <param name="v">The vertex to remove.</param>
            public void RemoveVertex(V v) => Parents.Remove(v);

            /// <summary>
            /// If possible, relax the given edge using the supplied base cost and edge-weight function.
            /// </summary>
            /// <param name="edge">The edge to relax.</param>
            /// <param name="cost">The base cost to reach the edge destination vertex.</param>
            /// <param name="ew">The edge weigher.</param>
            /// <param name="forbidNegatives">If true, negative values will forbid the link.</param>
            /// <returns>True if the edge was relaxed, otherwise false.</returns>
            public bool RelaxEdge(E edge, IWeight cost, IEdgeWeigher<V, E> ew, bool forbidNegatives = true)
            {
                V v = edge.Dst;
                IWeight hopCost = ew.GetWeight(edge);

                if ((!hopCost.IsViable) || (hopCost.IsNegative && forbidNegatives))
                {
                    return false;
                }

                IWeight newCost = cost.Merge(hopCost);

                int compareResult = -1;

                if (HasCost(v))
                {
                    IWeight oldCost = GetCost(v);
                    compareResult = newCost.CompareTo(oldCost);
                }

                if (compareResult <= 0)
                {
                    UpdateVertex(v, edge, newCost, compareResult < 0);
                }

                return compareResult < 0;
            }

            /// <summary>
            /// Builds a set of paths for the specified src/dst vertex pair.
            /// </summary>
            public void BuildPaths()
            {
                var destinations = new HashSet<V>();

                if (Dst == null)
                {
                    foreach (V cost in Costs.Keys)
                    {
                        destinations.Add(cost);
                    }
                }
                else
                {
                    destinations.Add(Dst);
                }

                foreach (V v in destinations)
                {
                    if (v != Src)
                    {
                        BuildAllPaths(this, Src, v, MaxPaths);
                    }
                }
            }

        }
        
        /// <summary>
        /// Builds a set of all paths between the source and destination using the graph search
        /// result by applying breadth-first search through the parent edges and vertex costs.
        /// </summary>
        /// <param name="result">The graph search result.</param>
        /// <param name="src">The source vertex.</param>
        /// <param name="dst">The destination vertex.</param>
        /// <param name="maxPaths">The limit on the number of paths built, <see cref="AllPaths"/> for no limit.</param>
        private static void BuildAllPaths(DefaultResult result, V src, V dst, int maxPaths = AllPaths)
        {
            var basePath = new DefaultMutablePath<V, E>();
            basePath.Cost = result.GetCost(dst);
            var pendingPaths = new HashSet<DefaultMutablePath<V, E>>();
            pendingPaths.Add(basePath);

            while (pendingPaths.Count != 0 && 
                (maxPaths == AllPaths || result.Paths.Count < maxPaths))
            {
                var frontier = new HashSet<DefaultMutablePath<V, E>>();

                foreach (DefaultMutablePath<V, E> path in pendingPaths)
                {
                    // For each pending path, locate its first vertex
                    // since we will be moving backwards from it.
                    V firstVertex = GetFirstVertex(path, dst);

                    // If the first vertex is our expected source, we have reached
                    // the beginning, so add this path to the result paths.
                    if (firstVertex.Equals(src))
                    {
                        path.Cost = result.GetCost(dst);
                        result.Paths.Add(new DefaultPath<V, E>(path.Edges, path.Cost));
                    }
                    else
                    {
                        // If we have not reached the beginning, i.e. the source, fetch the 
                        // set of edges leading to the first vertex of this pending path.
                        // If there are none, abandon processing this path for good.
                        ISet<E> firstVertexParents = result.Parents.GetOrDefault(firstVertex, new HashSet<E>());
                        if (firstVertexParents is null || firstVertexParents?.Count is 0)
                        {
                            break;
                        }

                        // Now, iterate over all the edges and for each of them cloning the current
                        // path and then inserting that edge to the path and adding that path to the
                        // pending ones. When processing the last edge, modify the current pending
                        // path rather than cloning a new one.
                        List<E> edges = firstVertexParents.ToList();
                        for (int i = 0; i < edges.Count; ++i)
                        {
                            E edge = edges[i];
                            bool isLast = i == edges.Count - 1;
                            // Exclude any looping paths.
                            if (!IsInPath(edge, path))
                            {
                                DefaultMutablePath<V, E> pendingPath = isLast ? path : new DefaultMutablePath<V, E>(path);
                                pendingPath.InsertEdge(edge);
                                frontier.Add(pendingPath);
                            }
                        }
                    }
                }

                // All pending paths have been scaned, so promote the next frontier.
                pendingPaths = frontier;
            }
        }

        /// <summary>
        /// Gets the first vertex in the specified path. This is either the source
        /// of the first edge or, if there are no edges yet, the given destination.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="dst">The destination to search.</param>
        /// <returns>The first vertex found, otherwise returns the given destination vertex.</returns>
        private static V GetFirstVertex(IPath<V, E> path, V dst) => path.Edges.Count is 0 ? dst : path.Edges[0].Src;

        /// <summary>
        /// Determines whether or not the specified edge source is already visited in the specified path.
        /// </summary>
        /// <param name="edge">The edge to test.</param>
        /// <param name="path">The path to test.</param>
        /// <returns>True if the edge's source is a vertex in the path already.</returns>
        private static bool IsInPath(E edge, DefaultMutablePath<V, E> path) => path.Edges.Any(e => edge.Src.Equals(e.Dst));

        /// <summary>
        /// Checks the specified path search arguments for validity.
        /// </summary>
        /// <param name="graph">The graph to check; must not be null.</param>
        /// <param name="src">The source vertex; must not be null and must belong to the graph.</param>
        /// <param name="dst">An optional target vertex; must belong to the graph.</param>
        protected void CheckArguments(IGraph<V, E> graph, V src, V dst)
        {
            CheckNotNull(graph, "The graph cannot be null.");
            CheckNotNull(src, "The source vertex cannot be null.");
            ISet<V> vertices = graph.Vertices;
            CheckArgument(vertices.Contains(src), "Source is not in the graph.");
            CheckArgument(dst is null || vertices.Contains(dst), "Destination is not in the graph.");
        }

        /// <summary>
        /// The abstract implementation of this algorithm's search function.
        /// </summary>
        /// <param name="graph">The graph to search.</param>
        /// <param name="src">The source vertex.</param>
        /// <param name="dst">The destination vertex.</param>
        /// <param name="weigher">The edge weigher.</param>
        /// <param name="maxPaths">The maximum number of paths to find.</param>
        /// <returns>A search result.</returns>
        protected abstract IResult<V, E> InternalSearch(IGraph<V, E> graph, V src, V dst, IEdgeWeigher<V, E> weigher, int maxPaths = AllPaths);

        /// <inheritdoc/>
        public IResult<V, E> Search(IGraph<V, E> graph, V src, V dst, IEdgeWeigher<V, E> weigher, int maxPaths = -1)
        {
            CheckArguments(graph, src, dst);
            return InternalSearch(graph, src, dst, weigher ?? new DefaultEdgeWeigher<V, E>(), maxPaths);
        }
    }
}