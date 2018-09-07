using System;
using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// A mutable graph implemented using adjacency lists.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class MutableAdjacencyListsGraph<V, E> : IMutableGraph<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        private readonly Dictionary<V, ISet<E>> sources = new Dictionary<V, ISet<E>>();
        private readonly Dictionary<V, ISet<E>> destinations = new Dictionary<V, ISet<E>>();

        /// <inheritdoc/>
        public ISet<V> Vertices { get; }

        /// <inheritdoc/>
        public ISet<E> Edges { get; }

        /// <summary>
        /// Initializes a new <see cref="MutableAdjacencyListsGraph{V, E}"/> instance
        /// comprising the given verticies and edges.
        /// </summary>
        /// <param name="vertices">The set of graph vertices.</param>
        /// <param name="edges">The set of graph edges.</param>
        public MutableAdjacencyListsGraph(ISet<V> vertices, ISet<E> edges)
        {
            Vertices = new HashSet<V>(vertices);
            Edges = new HashSet<E>(edges);
            foreach (E edge in Edges)
            {
                if (!sources.ContainsKey(edge.Src))
                {
                    sources.Add(edge.Src, new HashSet<E>());
                }
                sources[edge.Src].Add(edge);
                Vertices.Add(edge.Src);
                if (!destinations.ContainsKey(edge.Dst))
                {
                    destinations.Add(edge.Dst, new HashSet<E>());
                }
                destinations[edge.Dst].Add(edge);
                Vertices.Add(edge.Dst);
            }
        }

        /// <inheritdoc/>
        public void AddEdge(E edge)
        {
            if (Edges.Add(edge))
            {
                if (!sources.ContainsKey(edge.Src))
                {
                    sources.Add(edge.Src, new HashSet<E>());
                }
                sources[edge.Src].Add(edge);
                if (!destinations.ContainsKey(edge.Dst))
                {
                    destinations.Add(edge.Dst, new HashSet<E>());
                }
                destinations[edge.Dst].Add(edge);
            }
        }

        /// <inheritdoc/>
        public void AddVertex(V vertex) => Vertices.Add(vertex);

        /// <inheritdoc/>
        public ISet<E> GetEdgesFrom(V src) => sources[src];

        /// <inheritdoc/>
        public ISet<E> GetEdgesTo(V dst) => destinations[dst];

        /// <inheritdoc/>
        public void RemoveEdge(E edge)
        {
            if (Edges.Remove(edge))
            {
                sources[edge.Src].Remove(edge);
                destinations[edge.Dst].Remove(edge);
            }
        }

        /// <inheritdoc/>
        public void RemoveVertex(V vertex)
        {
            if (Vertices.Remove(vertex))
            {
                ISet<E> srcEdges = sources[vertex];
                ISet<E> dstEdges = destinations[vertex];
                foreach (E srcEdge in srcEdges)
                {
                    Edges.Remove(srcEdge);
                }
                foreach (E dstEdge in dstEdges)
                {
                    Edges.Remove(dstEdge);
                }
                sources.Remove(vertex, out srcEdges);
                sources.Remove(vertex, out dstEdges);
            }
        }

        /// <inheritdoc/>
        public IGraph<V, E> ToImmutable() => new AdjacencyListsGraph<V, E>(Vertices, Edges);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is MutableAdjacencyListsGraph<V, E> that &&
                EqualityComparer<ISet<V>>.Default.Equals(Vertices, that.Vertices) &&
                EqualityComparer<ISet<E>>.Default.Equals(Edges, that.Edges);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Vertices, Edges);
    }
}