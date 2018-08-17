using System;
using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Represents a pair of disjoint paths.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class DisjointPathPair<V, E> : IPath<V, E> where V : IVertex where E : IEdge<V>
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
        /// Initializes a new <see cref="DijkstraGraphSearch{V, E}"/> instance with the given paths.
        /// </summary>
        /// <param name="primary">The primary path.</param>
        /// <param name="secondary">The secondary path.</param>
        public DisjointPathPair(IPath<V, E> primary, IPath<V, E> secondary) => (Primary, Secondary) = (primary, secondary);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var other = obj as DisjointPathPair<V, E>;
            return other != null &&
                EqualityComparer<V>.Default.Equals(Src, other.Src) &&
                EqualityComparer<V>.Default.Equals(Dst, other.Dst) &&
                (EqualityComparer<IPath<V, E>>.Default.Equals(Primary, other.Primary) &&
                    EqualityComparer<IPath<V, E>>.Default.Equals(Secondary, other.Secondary)) ||
                (EqualityComparer<IPath<V, E>>.Default.Equals(Primary, other.Secondary) &&
                    EqualityComparer<IPath<V, E>>.Default.Equals(Secondary, other.Primary));

        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HasBackup ? Primary.GetHashCode() + Secondary.GetHashCode() : HashCode.Combine(Primary);
        }

        public override string ToString() => $"[{GetType().Name}] Src = {Src}, Dst = {Dst}, Cost = {Cost}, Edges = {Edges}";
    }
}