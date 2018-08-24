using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static Onos.Net.Utils.Misc.OnLab.Helpers.ArgsChecker;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Represents a path in a directed graph.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class DefaultMutablePath<V, E> : IEquatable<DefaultMutablePath<V, E>>, IMutablePath<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        private readonly List<E> edges = new List<E>();

        /// <inheritdoc/>
        public IList<E> Edges => ImmutableList.CreateRange(edges);

        /// <inheritdoc/>
        public IWeight Cost { get; set; }

        /// <inheritdoc/>
        public V Src => edges.FirstOrDefault()?.Src;

        /// <inheritdoc/>
        public V Dst => edges.LastOrDefault()?.Dst;

        /// <summary>
        /// Creates a new empty path.
        /// </summary>
        public DefaultMutablePath()
        {

        }

        /// <summary>
        /// Creates a new path as the copy of the given path.
        /// </summary>
        /// <param name="path">The path to copy.</param>
        public DefaultMutablePath(IPath<V, E> path)
        {
            CheckNotNull(path, "The path cannot be null.");
            Cost = path.Cost;
            edges.AddRange(path.Edges);
        }

        /// <inheritdoc/>
        public void AppendEdge(E edge)
        {
            CheckNotNull(edge, "The edge cannot be null.");
            CheckArgument(edges.Count == 0 || Dst == edge.Src, 
                "Edge source must be the same as the current path destination.");
            edges.Add(edge);
        }

        /// <inheritdoc/>
        public void InsertEdge(E edge)
        {
            CheckNotNull(edge, "The edge cannot be null.");
            CheckArgument(edges.Count == 0 || Src == edge.Dst, 
                "The destination edge must be the same as the current path source.");
            edges.Insert(0, edge);
        }

        /// <inheritdoc/>
        public void RemoveEdge(E edge)
        {
            CheckArgument(edge.Src == edge.Dst ||
                edges.IndexOf(edge) == 0 ||
                edges.LastIndexOf(edge) == edges.Count - 1,
                "Edge must be at the start of path, end of path, or a cyclic edge.");
            edges.Remove(edge);
        }

        /// <inheritdoc/>
        public IPath<V, E> ToImmutable() => new DefaultPath<V, E>(edges, Cost);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is DefaultMutablePath<V, E> other ? IsEqual(other) : false;
        }

        /// <inheritdoc/>
        public bool Equals(DefaultMutablePath<V, E> other)
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
        /// TODO: Should this be virtual?
        protected virtual bool IsEqual(DefaultMutablePath<V, E> other)
        {
            return Src.Equals(other.Src) &&
                   Dst.Equals(other.Dst) &&
                   Cost.Equals(other.Cost) &&
                   Edges.SequenceEqual(other.Edges);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(edges, Cost);
        }
    }
}
