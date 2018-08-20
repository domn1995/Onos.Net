using System;
using System.Collections.Generic;
using static Onos.Net.Utils.Misc.OnLab.Helpers.ArgsChecker;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Base representation of a graph edge.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    public abstract class AbstractEdge<V> : IEquatable<AbstractEdge<V>>, IEdge<V> where V : class, IVertex
    {
        /// <inheritdoc/>
        public V Src { get; }

        /// <inheritdoc/>
        public V Dst { get; }

        /// <summary>
        /// Initializes a new <see cref="AbstractEdge{V}"/> subclass 
        /// with the given source and destination vertices.
        /// </summary>
        /// <param name="src">The source vertex.</param>
        /// <param name="dst">The destination vertex.</param>
        protected AbstractEdge(V src, V dst)
        {
            Src = CheckNotNull(src);
            Dst = CheckNotNull(dst);
        }

        public static bool operator ==(AbstractEdge<V> first, AbstractEdge<V> second) => first.Equals(second);

        public static bool operator !=(AbstractEdge<V> first, AbstractEdge<V> second) => !(first == second);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is AbstractEdge<V> edge ? IsEqual(edge) : false;
        }

        /// <inheritdoc/>
        public bool Equals(AbstractEdge<V> other)
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
        protected bool IsEqual(AbstractEdge<V> other) => Src.Equals(other.Src) && Dst.Equals(other.Dst);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Src, Dst);
        }

        /// <inheritdoc/>
        public override string ToString() => $"[{GetType().Name}] Src = {Src}, Dst = {Dst}";

        
    }
}
