using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Represents a path in a directed graph.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class DefaultMutablePath<V, E> : IMutablePath<V, E> where V : class, IVertex where E : class, IEdge<V>
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
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));   
            }
            Cost = path.Cost;
            edges.AddRange(path.Edges);
        }

        /// <inheritdoc/>
        public void AppendEdge(E edge)
        {
            if (edge is null)
            {
                throw new ArgumentNullException(nameof(edge));
            }
            if (edges.Count == 0 || Dst == edge.Src)
            {
                throw new ArgumentException($"Edge source must be the same as the current path destination.");
            }
            edges.Add(edge);
        }

        /// <inheritdoc/>
        public void InsertEdge(E edge)
        {
            if (edge is null)
            {
                throw new ArgumentNullException(nameof(edge));
            }
            if (edges.Count == 0 || Src == edge.Dst)
            {
                throw new ArgumentException($"Edge destination must be the same as the current path source.");
            }
            edges.Insert(0, edge);
        }

        /// <inheritdoc/>
        public void RemoveEdge(E edge)
        {
            if (edge.Src == edge.Dst || 
                edges.IndexOf(edge) == 0 || 
                edges.LastIndexOf(edge) == edges.Count - 1)
            {
                throw new ArgumentException("Edge must be at the start of path, end of path, or a cyclic edge.");
            }
            edges.Remove(edge);
        }

        /// <inheritdoc/>
        public IPath<V, E> ToImmutable() => new DefaultPath<V, E>(edges, Cost);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var path = obj as DefaultMutablePath<V, E>;
            return path != null &&
                   EqualityComparer<List<E>>.Default.Equals(edges, path.edges) &&
                   EqualityComparer<IWeight>.Default.Equals(Cost, path.Cost);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(edges, Cost);
        }
    }
}
