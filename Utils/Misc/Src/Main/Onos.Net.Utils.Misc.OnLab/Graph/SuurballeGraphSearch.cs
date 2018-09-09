using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Onos.Net.Utils.Misc.OnLab.Helpers;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Suurablle shortest-path graph search algorithm capable of finding 
    /// both a shortest path, as well as a backup shortest path, between 
    /// a source and destination such that the sum of the path lengths is minimized.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class SuurballeGraphSearch<V, E> : DijkstraGraphSearch<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        private IEdgeWeigher<V, E> weightF;
        private DefaultResult firstDijkstraS;
        private DefaultResult firstDijkstra;
        private Dictionary<E, E> revToEdge = new Dictionary<E, E>();

        /// <inheritdoc/>
        protected override IResult<V, E> InternalSearch(IGraph<V, E> graph, V src, V dst, IEdgeWeigher<V, E> weigher, int maxPaths = -1)
        {
            // TODO: This method needs to be refactored as it is difficult to follow and debug.
            // TODO: There is a defect here triggered by 3+ edges between the same vertices which
            // makes an attempt to produce looping paths. Protection against this was added to
            // AbstractGraphPathSearch, but the root issue remains here.
            // TODO: There is a defect here where not all paths are truly disjoint.
            // This class needs to filter its own results to make sure that the paths
            // are indeed disjoint. Temporary fix for this is provided, but the issue still
            // needs to be addressed through refactoring.
            weightF = weigher;
            firstDijkstraS = (DefaultResult)base.InternalSearch(graph, src, dst, weigher, AllPaths);
            firstDijkstra = (DefaultResult)base.InternalSearch(graph, src, null, weigher, AllPaths);

            // Choose an arbitrary shortest path to run Suurballe on.
            IPath<V, E> shortPath = null;
            if (firstDijkstraS.Paths.Count == 0)
            {
                return firstDijkstraS;
            }

            DisjointPathResult result = new DisjointPathResult(firstDijkstra, src, dst, maxPaths);

            foreach (IPath<V, E> p in firstDijkstraS.Paths)
            {
                shortPath = p;
                // Transforms the graph so tree edges have 0 weight.
                var modified = new ModifiedWeigher(this);
                // Create a residual graph g' by removing all source vertices and reversing 0 length path edges.
                IMutableGraph<V, E> gt = MutableCopy(graph);
                foreach (E edge in graph.GetEdgesTo(src))
                {
                    gt.RemoveEdge(edge);
                }
                foreach (E edge in shortPath.Edges)
                {
                    gt.RemoveEdge(edge);
                    E reverse = (E)Activator.CreateInstance(typeof(E), edge.Dst, edge.Src);
                    revToEdge.AddOrSet(reverse, edge);
                    gt.AddEdge(reverse);
                }
                // Rerun dijkstra on the temporary graph to get a second path.
                IResult<V, E> secondDijkstra = new DijkstraGraphSearch<V, E>().Search(gt, src, dst, modified);
                IPath<V, E> residualShortPath = null;

                if (secondDijkstra.Paths.Count == 0)
                {
                    result.Dpps.Add(new DisjointPathPair<V, E>(shortPath, null));
                    continue;
                }

                foreach (IPath<V, E> p2 in secondDijkstra.Paths)
                {
                    residualShortPath = p2;
                    IMutableGraph<V, E> roundTrip = MutableCopy(graph);
                    List<E> tmp = roundTrip.Edges.ToList();
                    tmp.ForEach(roundTrip.RemoveEdge);
                    foreach (E edge in shortPath.Edges)
                    {
                        roundTrip.AddEdge(edge);
                    }

                    if (residualShortPath != null)
                    {
                        foreach (E edge in residualShortPath.Edges)
                        {
                            if (revToEdge.ContainsKey(edge))
                            {
                                E edgeToRemove = revToEdge[edge];
                                if (roundTrip.Edges.Contains(edgeToRemove))
                                {
                                    roundTrip.RemoveEdge(edgeToRemove);
                                }
                            }
                            else
                            {
                                roundTrip.AddEdge(edge);
                            }
                        }
                    }

                    // Actually build the final result.
                    DefaultResult lastSearch = (DefaultResult)base.InternalSearch(roundTrip, src, dst, weigher, AllPaths);
                    IPath<V, E> primary = lastSearch.Paths.First();
                    
                    foreach (E edge in primary.Edges)
                    {
                        roundTrip.RemoveEdge(edge);
                    }

                    ISet<IPath<V, E>> backups = base.InternalSearch(roundTrip, src, dst, weigher, AllPaths).Paths;

                    // Find first backup path that does not share any nodes with the primary.
                    foreach (IPath<V, E> backup in backups)
                    {
                        if (IsDisjoint(primary, backup))
                        {
                            result.Dpps.Add(new DisjointPathPair<V, E>(primary, backup));
                            break;
                        }
                    }
                }
            }

            for (int i = result.Dpps.Count - 1; i > 0; --i)
            {
                if (result.Dpps[i].Size <= 1)
                {
                    result.Dpps.RemoveAt(i);
                }
            }

            result.BuildPaths();
            return result;
        }

        private static bool IsDisjoint(IPath<V, E> a, IPath<V, E> b) => !Vertices(a).Intersect(Vertices(b)).Any();

        /// <summary>
        /// Creates a mutable copy of an immutable graph.
        /// </summary>
        /// <param name="graph">The graph to copy.</param>
        /// <returns>A mutable copy of the given graph.</returns>
        private static IMutableGraph<V, E> MutableCopy(IGraph<V, E> graph)
        {
            return new MutableAdjacencyListsGraph<V, E>(graph.Vertices, graph.Edges);
        }

        private static IEnumerable<V> Vertices(IPath<V, E> p)
        {
            ISet<V> set = new HashSet<V>();
            foreach (E edge in p.Edges)
            {
                set.Add(edge.Src);
            }
            set.Remove(p.Src);
            return set;
        }

        private sealed class ModifiedWeigher : IEdgeWeigher<V, E>
        {
            private readonly SuurballeGraphSearch<V, E> search;
            public IWeight InitialWeight => search.weightF.InitialWeight;

            public IWeight NonViableWeight => search.weightF.NonViableWeight;

            public ModifiedWeigher(SuurballeGraphSearch<V, E> search)
            {
                this.search = search;
            }

            public IWeight GetWeight(E edge)
            {
                if (search.revToEdge.ContainsKey(edge))
                {
                    return search.weightF.InitialWeight;
                }
                else if (search.weightF.GetWeight(edge).IsNegative)
                {
                    return new ScalarWeight(-1.0);
                }
                else
                {
                    return search.weightF.GetWeight(edge).Merge(search.firstDijkstra.GetCost(edge.Src))
                        .Subtract(search.firstDijkstra.GetCost(edge.Dst));
                }
            }
        }
        
        /// <summary>
        /// Auxiliary result for disjoint path search.
        /// </summary>
        private sealed class DisjointPathResult : DefaultResult
        {
            private readonly IResult<V, E> searchResult;
            private readonly int maxPaths;
            public IList<DisjointPathPair<V, E>> Dpps { get; } = new List<DisjointPathPair<V, E>>();

            public override V Src { get; }

            public override V Dst { get; }

            public override ISet<IPath<V, E>> Paths { get; } = new HashSet<IPath<V, E>>();

            public override IDictionary<V, ISet<E>> Parents => searchResult.Parents;
            public override IDictionary<V, IWeight> Costs => searchResult.Costs;

            public DisjointPathResult(IResult<V, E> searchResult, V src, V dst, int maxPaths = -1) : base(src, dst, maxPaths)
            {
                this.searchResult = searchResult;
                Src = src;
                Dst = dst;
                this.maxPaths = maxPaths;
            }

            public new void BuildPaths()
            {
                int paths = 0;
                foreach (DisjointPathPair<V, E> path in Dpps)
                {
                    Paths.Add(path);
                    paths++;
                    if (paths == maxPaths)
                    {
                        break;
                    }
                }
            }
        }
    }
}