using System;
using System.Collections.Generic;
using System.Linq;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// SRLG graph search finds a pair of paths with disjoint risk groups; i.e. if one path goes
    /// through an edge in risk group 1, the othe rpath will go through no edges in risk group 1.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class SrlgGraphSearch<V, E> : AbstractGraphPathSearch<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        private readonly int iterations = 100;
        private readonly int popSize = 50;
        private readonly int numGroups;
        private IDictionary<E, int> riskGrouping;
        private IGraph<V, E> orig;
        private V src, dst;
        private IEdgeWeigher<V, E> weigher;
        private readonly bool useSuurballe = false;

        /// <summary>
        /// Creates an SRLG graph search object with the given groups and risk mappings.
        /// </summary>
        /// <param name="groups">The number of disjoint risk groups.</param>
        /// <param name="grouping">The map linking edges to internal group assignments.</param>
        public SrlgGraphSearch(int groups, IDictionary<E, int> grouping)
        {
            numGroups = groups;
            riskGrouping = grouping;
        }

        /// <summary>
        /// Creates an SRLG graph search object from a map, inferring
        /// the number of groups and creating an integral mapping.
        /// </summary>
        /// <param name="grouping">The map linking edges to object group assignemnts,
        /// with same-group status linked to equality.</param>
        public SrlgGraphSearch(IDictionary<E, object> grouping)
        {
            if (grouping is null)
            {
                useSuurballe = true;
                return;
            }
            numGroups = 0;
            Dictionary<object, int> tmpMap = new Dictionary<object, int>();
            riskGrouping = new Dictionary<E, int>();
            foreach (E key in grouping.Keys)
            {
                object value = grouping[key];
                if (!tmpMap.ContainsKey(value))
                {
                    tmpMap.Add(value, numGroups);
                    numGroups++;
                }
                riskGrouping.Add(key, tmpMap[value]);
            }
        }
        
        /// <inheritdoc/>
        protected override IResult<V, E> InternalSearch(IGraph<V, E> graph, V src, V dst, IEdgeWeigher<V, E> weigher, int maxPaths = -1)
        {
            if (maxPaths == AllPaths)
            {
                maxPaths = popSize;
            }
            if (useSuurballe)
            {
                return new SuurballeGraphSearch<V, E>().Search(graph, src, dst, weigher, AllPaths);
            }
            orig = graph;
            this.src = src;
            this.dst = dst;
            this.weigher = weigher;
            var best = new GAPopulation<Subset>().RunGA(iterations, popSize, maxPaths, new Subset(this, new bool[numGroups]));
            var dpps = new HashSet<DisjointPathPair<V, E>>();
            foreach (Subset s in best)
            {
                foreach (var p in s.BuildPaths())
                {
                    dpps.Add(p);
                }
            }
            var firstDijkstra = new DijkstraGraphSearch<V, E>().Search(orig, src, dst, weigher, 1);
            var result = new InternalResult(this, firstDijkstra, dpps);
            return result;
        }

        /// <summary>
        /// Finds the shortest path in the graph given a subset of edge types to use.
        /// </summary>
        /// <param name="subset"></param>
        /// <returns></returns>
        private IResult<V, E> FindShortestPathFromSubset(bool[] subset)
        {
            IGraph<V, E> graph = orig;
            IEdgeWeigher<V, E> modified = new InternalWeigher(this, subset);
            var res = new DijkstraGraphSearch<V, E>().Search(graph, src, dst, modified, 1);
            return res;
        }

        /// <summary>
        /// Represents a search result internal to SRLG graph search.
        /// </summary>
        private sealed class InternalResult : IResult<V, E>
        {
            private readonly SrlgGraphSearch<V, E> search;
            private readonly IResult<V, E> result;
            private readonly ISet<DisjointPathPair<V, E>> dpps;

            public InternalResult(SrlgGraphSearch<V, E> search, IResult<V, E> result, ISet<DisjointPathPair<V, E>> dpps)
            {
                this.search = search;
                this.result = result;
                this.dpps = dpps;
            }

            public V Src => search.src;

            public V Dst => search.dst;

            public ISet<IPath<V, E>> Paths
            {
                get
                {
                    var pathsD = new HashSet<IPath<V, E>>();
                    foreach (var path in dpps)
                    {
                        pathsD.Add(path);
                    }
                    return pathsD;
                }
            }

            public IDictionary<V, ISet<E>> Parents => result.Parents;

            public IDictionary<V, IWeight> Costs => result.Costs;
        }

        /// <summary>
        /// Weighs edges in an SRLG graph search.
        /// </summary>
        private sealed class InternalWeigher : IEdgeWeigher<V, E>
        {
            private readonly SrlgGraphSearch<V, E> search;
            private readonly bool[] subset;

            public InternalWeigher(SrlgGraphSearch<V, E> search, bool[] subset) => (this.search, this.subset) = (search, subset);

            public IWeight InitialWeight => search.weigher.InitialWeight;

            public IWeight NonViableWeight => search.weigher.NonViableWeight;

            public IWeight GetWeight(E edge)
            {
                if (subset[search.riskGrouping[edge]])
                {
                    return search.weigher.GetWeight(edge);
                }
                return search.weigher.NonViableWeight;
            }
        }

        /// <summary>
        /// A subset is a type of GA organism that represents a subset of allowed 
        /// shortest paths (and its complement). Its fitness is determined 
        /// by the sum of the weights of the first two shortest paths.
        /// </summary>
        private class Subset : IGAOrganism
        {
            private readonly SrlgGraphSearch<V, E> search;
            private readonly bool[] subset;
            private readonly bool[] not;
            private readonly Random r = new Random();

            public IComparable Fitness
            {
                get
                {
                    var paths1 = search.FindShortestPathFromSubset(subset).Paths;
                    var paths2 = search.FindShortestPathFromSubset(not).Paths;
                    if (paths1.Count == 0 || paths2.Count == 0)
                    {
                        return (IComparable)search.weigher.NonViableWeight;
                    }
                    return (IComparable)paths1.First().Cost.Merge(paths2.First().Cost);
                }
            }

            /// <summary>
            /// Creates a subset from the given subset array.
            /// </summary>
            /// <param name="search">The associated search.</param>
            /// <param name="sub">The subset array.</param>
            public Subset(SrlgGraphSearch<V, E> search, bool[] sub)
            {
                this.search = search;
                subset = (bool[])sub.Clone();
                not = new bool[subset.Length];
                for (int i = 0; i < subset.Length; ++i)
                {
                    not[i] = !subset[i];
                }
            }

            public IGAOrganism GetRandom()
            {
                bool[] sub = new bool[subset.Length];
                for (int i = 0; i < sub.Length; ++i)
                {
                    sub[i] = r.Next(2) == 1;
                }
                return new Subset(search, sub);
            }

            public IGAOrganism CrossWith(IGAOrganism org)
            {
                if (!(org.GetType().Equals(this.GetType())))
                {
                    return this;
                }
                Subset other = (Subset)org;
                bool[] sub = new bool[subset.Length];
                for (int i = 0; i < subset.Length; ++i)
                {
                    sub[i] = subset[i];
                    if (r.Next(2) == 1)
                    {
                        sub[i] = other.subset[i];
                    }
                }
                return new Subset(search, sub);
            }

            public void Mutate()
            {
                int turns = r.Next((int)Math.Sqrt(subset.Length));
                while (turns > 0)
                {
                    int choose = r.Next(subset.Length);
                    subset[choose] = !subset[choose];
                    not[choose] = !not[choose];
                    turns--;
                }
            }

            /// <summary>
            /// Builds the set of disjoint path pairs for a given subset
            /// using Dijkstra's algorithm on both the subset and complement
            /// and returning all pairs with one from each set.
            /// </summary>
            /// <returns>All shortest disjoint paths given this subset.</returns>
            public ISet<DisjointPathPair<V, E>> BuildPaths()
            {
                var dpps = new HashSet<DisjointPathPair<V, E>>();
                foreach (var path1 in search.FindShortestPathFromSubset(subset).Paths)
                {
                    foreach (var path2 in search.FindShortestPathFromSubset(not).Paths)
                    {
                        var dpp = new DisjointPathPair<V, E>(path1, path2);
                        dpps.Add(dpp);
                    }
                }
                return dpps;
            }
        }
    }
}