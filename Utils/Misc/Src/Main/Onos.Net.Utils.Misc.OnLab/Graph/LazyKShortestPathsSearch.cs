using ConcurrentCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Onos.Net.Utils.Misc.OnLab.Helpers.ArgsChecker;
using Onos.Net.Utils.Misc.OnLab.Helpers;
using System.Collections.Immutable;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Lazily runs a K-shortest paths algorithm on the given directed graph.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    /// TODO: Make this less complex. Can we just use IEnumerable for Dijkstra?
    public class LazyKShortestPathsSearch<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        private static readonly IGraphPathSearch<V, E> search = new DijkstraGraphSearch<V, E>();

        /// <summary>
        /// Searches the given graph for paths between the given vertices.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <param name="weigher"></param>
        /// <returns></returns>
        public IEnumerable<IPath<V, E>> LazyPathSearch(IGraph<V, E> graph, V src, V dst, IEdgeWeigher<V, E> weigher)
        {
            var enumerator = new ShortestPathEnumerator(graph, src, dst, weigher);
            var paths = new SortedSet<IPath<V, E>>(EnumeratePaths(enumerator), new PathComparer());

            foreach (IPath<V, E> path in paths)
            {
                yield return path;
            }
        }

        private IEnumerable<IPath<V, E>> EnumeratePaths(ShortestPathEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        /// <summary>
        /// Enumerates the shortest paths in the given graph, between the given vertices, using the given edge weigher.
        /// </summary>
        private sealed class ShortestPathEnumerator : IEnumerator<IPath<V, E>>
        {
            private readonly IGraph<V, E> graph;
            private readonly V dst;
            private readonly IEdgeWeigher<V, E> weigher;
            private readonly InnerEdgeWeigher maskingWeigher;
            private readonly IList<IPath<V, E>> resultPaths = new List<IPath<V, E>>();
            private readonly C5.IPriorityQueue<IPath<V, E>> potentialPaths = new C5.IntervalHeap<IPath<V, E>>(new PathComparer());
            private Func<IPath<V, E>> next;
            public ShortestPathEnumerator(IGraph<V, E> graph, V src, V dst, IEdgeWeigher<V, E> weigher)
            {
                this.graph = CheckNotNull(graph);
                CheckNotNull(src);
                this.dst = CheckNotNull(dst);
                this.weigher = CheckNotNull(weigher);
                maskingWeigher = new InnerEdgeWeigher(weigher);
                next = () => search.Search(graph, src, dst, weigher, 1).Paths.FirstOrDefault();
            }

            public IPath<V, E> Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                IPath<V, E> current = next();

                if (current is null)
                {
                    return false;
                }

                // TODO: Short circuiting here in order to prevent duplicate paths.
                // Could this introduce unexpected behavior to its consumers?
                //if (resultPaths.Contains(current))
                //{
                //    return false;
                //}

                Current = current;                
                resultPaths.Add(current);
                next = Memoizer.Memoize(() => ComputeNext(current));
                return true;
            }

            public void Reset()
            {
                Current = null;
            }

            private IPath<V, E> ComputeNext(IPath<V, E> lastPath)
            {
                // Start searching for the next path.
                for (int i = 0; i < lastPath.Edges.Count; ++i)
                {
                    V spurNode = lastPath.Edges[i].Src;
                    List<E> rootPathEdgeList = lastPath.Edges.Take(i).ToList();

                    foreach (IPath<V, E> path in resultPaths)
                    {
                        if (path.Edges.Count >= i && rootPathEdgeList.SequenceEqual(path.Edges.Take(i)))
                        {
                            maskingWeigher.Excluded.Add(path.Edges[i]);
                        }
                    }

                    // Effectively remove all root path nodes other than the spur node.
                    foreach (E edge in rootPathEdgeList)
                    {
                        foreach (E e in graph.GetEdgesFrom(edge.Src))
                        {
                            maskingWeigher.Excluded.Add(e);
                        }
                        foreach (E e in graph.GetEdgesTo(edge.Src))
                        {
                            maskingWeigher.Excluded.Add(e);
                        }
                    }

                    IPath<V, E> spurPath = search.Search(graph, spurNode, dst, maskingWeigher, 1).Paths.FirstOrDefault();
                    if (spurPath != null)
                    {
                        ImmutableList<E>.Builder builder = ImmutableList.CreateBuilder<E>();
                        builder.AddRange(rootPathEdgeList);
                        builder.AddRange(spurPath.Edges);
                        potentialPaths.Add(Path(builder.ToImmutable()));
                    }

                    // Restore all removed paths and nodes.
                    maskingWeigher.Excluded.Clear();
                }

                return potentialPaths.IsEmpty ? null : potentialPaths.DeleteMax();
            }

            /// <summary>
            /// Builds a path with the given edges.
            /// </summary>
            /// <param name="edges">The edges in the path.</param>
            /// <returns>The path built from the given edges.</returns>
            /// <remarks>This must return the original weigher, not the modified weigher, because the modified
            /// weigher will count -1 values used for modifying the graph and return an inaccurate cost.</remarks>
            private IPath<V, E> Path(IList<E> edges) => new DefaultPath<V, E>(edges, CalculatePathCost(weigher, edges));

            /// <summary>
            /// Sums the given edges using the given weigher.
            /// </summary>
            /// <param name="weigher">The weigher to use.</param>
            /// <param name="edges">The edges to sum.</param>
            /// <returns>The sum of path cost between the given edges.</returns>
            private IWeight CalculatePathCost(IEdgeWeigher<V, E> weigher, IEnumerable<E> edges)
            {
                IWeight totalCost = weigher.InitialWeight;
                foreach (E edge in edges)
                {
                    totalCost = totalCost.Merge(weigher.GetWeight(edge));
                }
                return totalCost;
            }
        }


        /// <inheritdoc />
        /// <summary>
        /// Provides an edge weigher which excludes specified edges from path computation.
        /// </summary>
        private sealed class InnerEdgeWeigher : IEdgeWeigher<V, E>
        {
            public ConcurrentHashSet<E> Excluded { get; } = new ConcurrentHashSet<E>();
            private readonly IEdgeWeigher<V, E> weigher;

            public InnerEdgeWeigher(IEdgeWeigher<V, E> weigher) => this.weigher = weigher;

            public IWeight InitialWeight => weigher.InitialWeight;

            public IWeight NonViableWeight => weigher.NonViableWeight;

            public IWeight GetWeight(E edge) => Excluded.Contains(edge) ? weigher.NonViableWeight : weigher.GetWeight(edge);
        }

        /// <summary>
        /// Provides a comparer to order a set of paths.
        /// Compares first by cost then by hop count.
        /// </summary>
        private sealed class PathComparer : IComparer<IPath<V, E>>
        {
            public int Compare(IPath<V, E> path1, IPath<V, E> path2)
            {
                int result;
                // If they're the same cost, let's take the one with fewer hops.
                if (path1.Cost.Equals(path2?.Cost))
                {
                    result = path1.Edges.Count.CompareTo(path2.Edges.Count);
                }
                else
                {
                    result = path1.Cost.CompareTo(path2.Cost);
                }
                return result;
            }
        }
    }
}