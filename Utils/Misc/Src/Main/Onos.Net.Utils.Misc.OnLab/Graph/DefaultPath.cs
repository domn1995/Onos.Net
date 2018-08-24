using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static Onos.Net.Utils.Misc.OnLab.Helpers.ArgsChecker;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    public class DefaultPath<V, E> : IEquatable<DefaultPath<V, E>>, IPath<V, E> where V : class, IVertex where E : class, IEdge<V>
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
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is DefaultPath<V, E> path ? IsEqual(path) : false;
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
            return Src.Equals(other.Src) &&
                   Dst.Equals(other.Dst) &&
                   // TODO: This is clunky, is there a better way?
                   other.Cost is null ? Cost is null 
                      : Cost.Equals(other.Cost)  &&
                   Edges.SequenceEqual(other.Edges);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(edges, Cost, Src, Dst);
    }
}
