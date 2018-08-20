using System.Collections.Generic;
using System.Linq;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Suurablle shortest-path graph search algorithm capable of finding 
    /// both a shortest path,/// as well as a backup shortest path, between 
    /// a source and destination such that the sum of the path lengths is minimized.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class SuurballeGraphSearch<V, E> : DijkstraGraphSearch<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        private IEdgeWeigher<V, E> weightF;
        private DefaultResult firstDijkstraS;
        private DefaultResult firstDijkstra;

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

            foreach (var p in firstDijkstraS.Paths)
            {
                shortPath = p;
                // Transforms the graph so tree edges have 0 weight.
                var modified = new ModifiedWeigher(this);
                // Create a residual graph g' by removing all source vertices and reversing 0 length path edges.
                var gt = MutableCopy(graph);
                var revToEdge = new Dictionary<E, E>();
                foreach (var edge in graph.GetEdgesTo(src))
                {
                    gt.RemoveEdge(edge);
                }
                foreach (var edge in shortPath.Edges)
                {
                    gt.RemoveEdge(edge);
                    IEdge<V> reverse = new ReverseEdge(edge);
                    revToEdge.Add((E)reverse, edge);
                    gt.AddEdge((E)reverse);
                }
                // Rerun dijkstra on the temporary graph to get a second path.
                var secondDijkstra = new DijkstraGraphSearch<V, E>().Search(gt, src, dst, modified, AllPaths);
                IPath<V, E> residualShortPath = null;
                if (secondDijkstra.Paths.Count == 0)
                {
                    result.Dpps.Add(new DisjointPathPair<V, E>(shortPath, null));
                    continue;
                }

                foreach (var p2 in secondDijkstra.Paths)
                {
                    residualShortPath = p2;
                    var roundTrip = MutableCopy(graph);
                    var tmp = roundTrip.Edges.ToList();
                    tmp.ForEach(roundTrip.RemoveEdge);
                    foreach (var edge in shortPath.Edges)
                    {
                        roundTrip.AddEdge(edge);
                    }

                    if (residualShortPath != null)
                    {
                        foreach (var edge in residualShortPath.Edges)
                        {
                            if (edge is ReverseEdge)
                            {
                                roundTrip.RemoveEdge(revToEdge[edge]);
                            }
                            else
                            {
                                roundTrip.AddEdge(edge);
                            }
                        }
                    }

                    // Actually build the final result.
                    DefaultResult lastSearch = (DefaultResult)base.InternalSearch(roundTrip, src, dst, weigher, AllPaths);
                    var primary = lastSearch.Paths.First();
                    
                    foreach (var edge in primary.Edges)
                    {
                        roundTrip.RemoveEdge(edge);
                    }

                    var backups = base.InternalSearch(roundTrip, src, dst, weigher, AllPaths).Paths;

                    // Find first backup path that does not share any nodes with the primary.
                    foreach (var backup in backups)
                    {
                        if (IsDisjoint(primary, backup))
                        {
                            result.Dpps.Add(new DisjointPathPair<V, E>(primary, backup));
                            break;
                        }
                    }
                }
            }

            // TODO: LINQify.
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

        private bool IsDisjoint(IPath<V, E> a, IPath<V, E> b) => Vertices(a).Intersect(Vertices(b)).Count() == 0;

        /// <summary>
        /// Creates a mutable copy of an immutable graph.
        /// </summary>
        /// <param name="graph">The graph to copy.</param>
        /// <returns>A mutable copy of the given graph.</returns>
        private IMutableGraph<V, E> MutableCopy(IGraph<V, E> graph)
        {
            return new MutableAdjacencyListsGraph<V, E>(graph.Vertices, graph.Edges);
        }

        private ISet<V> Vertices(IPath<V, E> p)
        {
            ISet<V> set = new HashSet<V>();
            foreach (var edge in p.Edges)
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
                if (edge is ReverseEdge)
                {
                    return search.weightF.InitialWeight;
                }
                else if (search.weightF.GetWeight(edge).IsNegative)
                {
                    return new ScalarWeight(-1.0);
                }
                else
                {
                    return search.weightF.GetWeight(edge).Merge(search.firstDijkstra.GetCost(edge.Src)
                        .Subtract(search.firstDijkstra.GetCost(edge.Dst)));
                }
            }
        }

        private sealed class ReverseEdge : AbstractEdge<V> 
        {
            public ReverseEdge(IEdge<V> edge) : base(edge.Src, edge.Dst)
            {

            }

            public override string ToString() => $"ReversedEdge Src={Src} Dst={Dst}";
        }

        /// <summary>
        /// Auxiliary result for disjoint path search.
        /// </summary>
        private sealed class DisjointPathResult : DefaultResult
        {
            private readonly IResult<V, E> searchResult;
            private readonly V src, dst;
            private readonly int maxPaths;
            public IList<DisjointPathPair<V, E>> Dpps { get; } = new List<DisjointPathPair<V, E>>();
            private readonly ISet<IPath<V, E>> disjointPaths = new HashSet<IPath<V, E>>();

            public override V Src => src;
            public override V Dst => dst;
            public override ISet<IPath<V, E>> Paths => disjointPaths;
            public override IDictionary<V, ISet<E>> Parents => searchResult.Parents;
            public override IDictionary<V, IWeight> Costs => searchResult.Costs;

            public DisjointPathResult(IResult<V, E> searchResult, V src, V dst, int maxPaths = -1) : base(src, dst, maxPaths)
            {
                this.searchResult = searchResult;
                this.src = src;
                this.dst = dst;
                this.maxPaths = maxPaths;
            }

            public new void BuildPaths()
            {
                int paths = 0;
                foreach (var path in Dpps)
                {
                    disjointPaths.Add(path);
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