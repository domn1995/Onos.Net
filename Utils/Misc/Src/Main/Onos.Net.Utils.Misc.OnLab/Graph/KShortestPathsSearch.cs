using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static Onos.Net.Utils.Misc.OnLab.Helpers.ArgsChecker;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Runs a K-shortest paths algorithm on a provided directed graph.
    /// Returns results in the form of an <see cref="InnerOrderedResult"/>
    /// so iteration through the returned paths will return paths in
    /// ascending order according to the provided <see cref="IEdgeWeigher{V, E}"/>.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class KShortestPathsSearch<V, E> : AbstractGraphPathSearch<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <inheritdoc/>
        protected override IResult<V, E> InternalSearch(IGraph<V, E> graph, V src, V dst, IEdgeWeigher<V, E> weigher, int maxPaths = 1)
        {
            CheckNotNull(weigher, "The edge weigher cannot be null.");
            CheckArgument(maxPaths != AllPaths, "KShortestPath cannot search all paths.");
            CheckArgument(maxPaths > 0, "The max number of paths must be greater than 0.");

            var originalGraph = CheckNotNull(graph, "The graph cannot be null.");
            var modifiedWeigher = new InnerEdgeWeigher(weigher);
            var result = new InnerOrderedResult(src, dst, maxPaths);
            var resultPaths = new List<IPath<V, E>>(maxPaths);
            var potentialPaths = new List<IPath<V, E>>();
            var dijkstraSearch = new DijkstraGraphSearch<V, E>();
            var dijkstraResults = dijkstraSearch.Search(originalGraph, src, dst, modifiedWeigher, 1).Paths;

            // Checks if the destination was reachable.
            if (dijkstraResults.Count == 0)
            {
                log.Warn("No path was found.");
                return result;
            }

            // If it was reachable, add the first shortest path to the set of results.
            resultPaths.Add(dijkstraResults.First());

            for (int k = 1; k < maxPaths; ++k)
            {
                for (int i = 0; i < resultPaths[k - 1].Edges.Count; ++ i)
                {
                    V spurNode = resultPaths[k - 1].Edges[i].Src;
                    var rootPathEdgeList = resultPaths[k - 1].Edges.Take(i).ToList();

                    foreach (var path in resultPaths)
                    {
                        if (path.Edges.Count >= i && rootPathEdgeList.SequenceEqual(path.Edges.Take(i)))
                        {
                            modifiedWeigher.RemovedEdges.Add(path.Edges[i]);
                        }
                    }

                    // Effectively remove all nodes from the source path.
                    foreach (E edge in rootPathEdgeList)
                    {
                        foreach (var e in originalGraph.GetEdgesFrom(edge.Src))
                        {
                            modifiedWeigher.RemovedEdges.Add(e);
                        }
                        foreach (var e in originalGraph.GetEdgesTo(edge.Src))
                        {
                            modifiedWeigher.RemovedEdges.Add(e);
                        }
                    }

                    dijkstraResults = dijkstraSearch.Search(originalGraph, spurNode, dst, modifiedWeigher, 1).Paths;

                    if (dijkstraResults.Count > 0)
                    {
                        var spurPath = dijkstraResults.First();
                        var totalPath = new List<E>(rootPathEdgeList);
                        foreach (E edge in spurPath.Edges)
                        {
                            totalPath.Add(edge);
                        }
                        // The following line must use the original weigher, not the modified weigher, because the
                        // modifed weigher will count -1 values used for modifying the graph and return an inaccurate cost.
                        potentialPaths.Add(new DefaultPath<V, E>(totalPath, CalculatePathCost(weigher, totalPath)));
                    }

                    // Restore all removed paths and nodes.
                    modifiedWeigher.RemovedEdges.Clear();
                }

                if (potentialPaths.Count == 0)
                {
                    break;
                }

                potentialPaths.Sort(new InnerPathComparer());
                resultPaths.Add(potentialPaths[0]);
                potentialPaths.RemoveAt(0);
            }

            resultPaths.ForEach(p => result.PathSet.Add(p));

            return result;
        }

        /// <summary>
        /// Calculates the total path cost of the given edges using the given weigher.
        /// </summary>
        /// <param name="weigher">The edge weigher to use.</param>
        /// <param name="edges">The edges to measure.</param>
        /// <returns>The total path cost as an <see cref="IWeight"/> object.</returns>
        private IWeight CalculatePathCost(IEdgeWeigher<V, E> weigher, IEnumerable<E> edges)
        {
            IWeight totalCost = weigher.InitialWeight;

            foreach (var edge in edges)
            {
                totalCost = totalCost.Merge(weigher.GetWeight(edge));
            }

            return totalCost;
        }

        /// <summary>
        /// Weighs edges to make them inaccessible if set.
        /// Otherwise, returns the result of the original <see cref="IEdgeWeigher{V, E}"/>.
        /// </summary>
        private sealed class InnerEdgeWeigher : IEdgeWeigher<V, E>
        {
            private readonly IEdgeWeigher<V, E> innerEdgeWeigher;
            public ISet<E> RemovedEdges { get; } = new HashSet<E>();

            public InnerEdgeWeigher(IEdgeWeigher<V, E> weigher) => innerEdgeWeigher = weigher;

            public IWeight InitialWeight => innerEdgeWeigher.InitialWeight;

            public IWeight NonViableWeight => innerEdgeWeigher.NonViableWeight;

            public IWeight GetWeight(E edge)
            {
                if (RemovedEdges.Contains(edge))
                {
                    return innerEdgeWeigher.NonViableWeight;
                }
                return innerEdgeWeigher.GetWeight(edge);
            }
        }

        /// <summary>
        /// A result modified to return paths ordered according to the provided comparer.
        /// </summary>
        protected class InnerOrderedResult : DefaultResult
        {
            /// <summary>
            /// Gets the sorted set of paths for this result.
            /// </summary>
            public SortedSet<IPath<V, E>> PathSet { get; } = new SortedSet<IPath<V, E>>(new InnerPathComparer());

            /// <summary>
            /// Gets an immutable set of paths for this result.
            /// </summary>
            public override ISet<IPath<V, E>> Paths => PathSet.ToImmutableSortedSet(new InnerPathComparer());

            /// <summary>
            /// Initializes a new <see cref="InnerOrderedResult"/> instance with the given values.
            /// </summary>
            /// <param name="src">The source vertex.</param>
            /// <param name="dst">The source destination.</param>
            /// <param name="maxPaths">The maximum number of paths.</param>
            public InnerOrderedResult(V src, V dst, int maxPaths = 1) 
                : base(src, dst, maxPaths)
            {

            }
        }

        /// <summary>
        /// A comparator to order a set of paths.
        /// </summary>
        private class InnerPathComparer : IComparer<IPath<V, E>>
        {
            public int Compare(IPath<V, E> pathOne, IPath<V, E> pathTwo)
            {
                int comparisonValue = pathOne.Cost.CompareTo(pathTwo.Cost);
                if (comparisonValue != 0)
                {
                    return comparisonValue;
                }
                else if (pathOne.Edges.SequenceEqual(pathTwo.Edges))
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }

    }
}