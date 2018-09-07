using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Simple concrete implementation of a directed graph path.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class DefaultPath<V, E> : IEquatable<DefaultPath<V, E>>, IPath<V, E>
        where V : class, IVertex where E : class, IEdge<V>
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
        public override string ToString() =>
            $"[{GetType().Name}] Src = {Src}, Dst = {Dst}, Cost = {Cost}, Edges = {edges}";

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is DefaultPath<V, E> path && IsEqual(path);
        }

        /// <inheritdoc/>
        public bool Equals(DefaultPath<V, E> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        /// <summary>
        /// Determines value equality.
        /// </summary>
        /// <param name="other">The other object to compare against.</param>
        /// <returns>True if the objects are equal.</returns>
        protected virtual bool IsEqual(DefaultPath<V, E> other)
        {
            bool result = Src.Equals(other.Src) &&
                   Dst.Equals(other.Dst) &&
                   edges.SequenceEqual(other.Edges) &&
                   // TODO: This is clunky, is there a better way?
                   other.Cost is null ? Cost is null : other.Cost.Equals(Cost);
            return result;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int edgesHash = int.MinValue;
            foreach (int hash in edges.Select(e => e.GetHashCode()))
            {
                edgesHash ^= hash;
            }
            int finalHash = HashCode.Combine(edgesHash, Cost, Src, Dst);
            return finalHash;
        }
    }
}