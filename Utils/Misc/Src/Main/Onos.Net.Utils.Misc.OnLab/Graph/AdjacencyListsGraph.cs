using System;
using System.Collections.Generic;
using static Onos.Net.Utils.Misc.OnLab.Helpers.ArgsChecker;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// An immutable graph implemented using adjacency lists.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class AdjacencyListsGraph<V, E> : IGraph<V, E> where V : IVertex where E : IEdge<V>
    {
        private readonly Dictionary<V, ISet<E>> sources = new Dictionary<V, ISet<E>>();
        private readonly Dictionary<V, ISet<E>> destinations = new Dictionary<V, ISet<E>>();

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
            CheckNotNull(vertices, "Vertices cannot be null.");
            CheckNotNull(edges, "Edges cannot be null.");
            HashSet<V> actualVertices = new HashSet<V>(vertices);

            foreach (E edge in edges)
            {
                if (!sources.ContainsKey(edge.Src))
                {
                    sources.Add(edge.Src, new HashSet<E>());
                }
                sources[edge.Src].Add(edge);
                actualVertices.Add(edge.Src);
                if (!destinations.ContainsKey(edge.Dst))
                {
                    destinations.Add(edge.Dst, new HashSet<E>());
                }
                destinations[edge.Dst].Add(edge);
                actualVertices.Add(edge.Dst);
            }

            Edges = edges;
            Vertices = actualVertices;
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
