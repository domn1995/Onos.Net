using System;
using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// An immutable graph implemented using adjacency lists.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class AdjacencyListsGraph<V, E> : IGraph<V, E> where V : IVertex where E : IEdge<V>
    {
        private readonly Dictionary<V, ISet<E>> sources;
        private readonly Dictionary<V, ISet<E>> destinations;

        /// <inheritdoc/>
        public ISet<V> Vertices { get; }

        /// <inheritdoc/>
        public ISet<E> Edges { get; }

        /// <summary>
        /// Initializes a new <see cref="AdjacencyListsGraph{V, E}"/> object with the given vertices and edges.
        /// </summary>
        /// <param name="vertices">The set of graph vertices.</param>
        /// <param name="edges">The set of graph edges.</param>
        public AdjacencyListsGraph(ISet<V> vertices, ISet<E> edges)
        {
            if (vertices is null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }
            if (edges is null)
            {
                throw new ArgumentNullException(nameof(edges));
            }

            Dictionary<V, ISet<E>> srcMap = new Dictionary<V, ISet<E>>();
            Dictionary<V, ISet<E>> dstMap = new Dictionary<V, ISet<E>>();
            HashSet<V> actualVertices = new HashSet<V>(vertices);

            foreach (E edge in edges)
            {
                if (!srcMap.ContainsKey(edge.Src))
                {
                    srcMap.Add(edge.Src, new HashSet<E>());
                }
                srcMap[edge.Src].Add(edge);
                actualVertices.Add(edge.Src);
                if (!srcMap.ContainsKey(edge.Dst))
                {
                    dstMap.Add(edge.Dst, new HashSet<E>());
                }
                dstMap[edge.Dst].Add(edge);
                actualVertices.Add(edge.Dst);
            }

            Edges = edges;
            Vertices = actualVertices;
            sources = srcMap;
            destinations = dstMap;
        }

        /// <inheritdoc/>
        public ISet<E> GetEdgesFrom(V src) => sources[src];

        /// <inheritdoc/>
        public ISet<E> GetEdgesTo(V dst) => destinations[dst];

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var graph = obj as AdjacencyListsGraph<V, E>;
            return graph != null &&
                   EqualityComparer<ISet<V>>.Default.Equals(Vertices, graph.Vertices) &&
                   EqualityComparer<ISet<E>>.Default.Equals(Edges, graph.Edges);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Vertices, Edges);

        /// <inheritdoc/>
        public override string ToString() => $"[{GetType().Name}] Vertices = {Vertices}, Edges = {Edges}";
    }
}
