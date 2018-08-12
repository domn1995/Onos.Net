using System;
using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Base representation of a graph edge.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    public abstract class AbstractEdge<V> : IEdge<V> where V : IVertex
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
            Src = src;
            Dst = dst;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var edge = obj as AbstractEdge<V>;
            return edge != null &&
                   EqualityComparer<V>.Default.Equals(Src, edge.Src) &&
                   EqualityComparer<V>.Default.Equals(Dst, edge.Dst);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Src, Dst);
        }

        /// <inheritdoc/>
        public override string ToString() => $"[{GetType().Name}] Src = {Src}, Dst = {Dst}";
    }
}
