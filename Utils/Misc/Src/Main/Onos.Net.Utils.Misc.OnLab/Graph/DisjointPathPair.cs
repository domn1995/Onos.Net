using System;
using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Represents a pair of disjoint paths.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class DisjointPathPair<V, E> : IEquatable<DisjointPathPair<V, E>>,
                                          IPath<V, E> where V : IVertex where E : IEdge<V>
    {
        /// <summary>
        /// Gets the primary path.
        /// </summary>
        public IPath<V, E> Primary { get; }

        /// <summary>
        /// Gets the secondary path or null if no secondary path is available.
        /// </summary>
        public IPath<V, E> Secondary { get; }

        /// <inheritdoc/>
        public IList<E> Edges => Primary.Edges;

        /// <inheritdoc/>
        public IWeight Cost => HasBackup ? Primary.Cost.Merge(Secondary.Cost) : Primary.Cost;

        /// <inheritdoc/>
        public V Src => Primary.Src;

        /// <inheritdoc/>
        public V Dst => Primary.Dst;

        /// <summary>
        /// Gets whether this path pair contains a backup/secondary path.
        /// Returns true if there is a backup path, false otherwise.
        /// </summary>
        public bool HasBackup => Secondary != null;

        /// <summary>
        /// Gets the number of paths inside this path pair object.
        /// </summary>
        public int Size => HasBackup ? 2 : 1;

        /// <summary>
        /// Initializes a new <see cref="DijkstraGraphSearch{V, E}"/> instance with the given paths.
        /// </summary>
        /// <param name="primary">The primary path.</param>
        /// <param name="secondary">The secondary path.</param>
        public DisjointPathPair(IPath<V, E> primary, IPath<V, E> secondary) => (Primary, Secondary) = (primary, secondary);

        public bool Equals(DisjointPathPair<V, E> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is DisjointPathPair<V, E> other && IsEqual(other);
        }

        protected virtual bool IsEqual(DisjointPathPair<V, E> other)
        {
            bool result = Src.Equals(other.Src) &&
                   Dst.Equals(other.Dst) &&
                   (Primary.Equals(other.Primary) &&
                        Secondary.Equals(other.Secondary)) ||
                   (Primary.Equals(other.Secondary) &&
                        Secondary.Equals(other.Primary));
            return result;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HasBackup ? Primary.GetHashCode() + Secondary.GetHashCode() : Primary.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => $"[{GetType().Name}] Src = {Src}, Dst = {Dst}, Cost = {Cost}, Edges = {Edges}";
    }
}