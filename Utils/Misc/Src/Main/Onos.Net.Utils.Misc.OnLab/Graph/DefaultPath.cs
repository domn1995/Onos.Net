using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    public class DefaultPath<V, E> : IPath<V, E> where V : IVertex where E : IEdge<V>
    {
        private readonly IList<E> edges;

        /// <inheritdoc/>
        public IList<E> Edges => ImmutableList.CreateRange(edges);

        /// <inheritdoc/>
        public IWeight Cost { get; }

        /// <inheritdoc/>
        public V Src { get; }

        /// <inheritdoc/>
        public V Dst { get; }

        /// <summary>
        /// Initializes a new path from the give list of edges and cost.
        /// </summary>
        /// <param name="edges">The list of path edges.</param>
        /// <param name="cost">The path cost.</param>
        public DefaultPath(IList<E> edges, IWeight cost)
        {
            if (edges?.Count is 0)
            {
                throw new ArgumentException("There must be at least one edge.");
            }
            this.edges = edges;
            Src = Edges[0].Src;
            Dst = Edges.Last().Dst;
            Cost = cost;
        }

        /// <inheritdoc/>
        public override string ToString() => $"[{GetType().Name}] Src = {Src}, Dst = {Dst}, Cost = {Cost}, Edges = {edges}";

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var path = obj as DefaultPath<V, E>;
            return path != null &&
                   EqualityComparer<IList<E>>.Default.Equals(edges, path.edges) &&
                   EqualityComparer<IWeight>.Default.Equals(Cost, path.Cost) &&
                   EqualityComparer<V>.Default.Equals(Src, path.Src) &&
                   EqualityComparer<V>.Default.Equals(Dst, path.Dst);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(edges, Cost, Src, Dst);
    }
}
