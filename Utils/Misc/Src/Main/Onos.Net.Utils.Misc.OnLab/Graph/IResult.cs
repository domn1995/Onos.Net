using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Enables implementing classes to represent a path search result.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public interface IResult<V, E> where V : IVertex where E : IEdge<V>
    {
        /// <summary>
        /// Gets the search source.
        /// </summary>
        V Src { get; }

        /// <summary>
        /// Gets the search destination, if given.
        /// </summary>
        V Dst { get; }

        /// <summary>
        /// Gets the set of paths produced as a result of the graph search.
        /// </summary>
        ISet<IPath<V, E>> Paths { get; }

        /// <summary>
        /// Gets the bindings of each vertex to its parent edges in the path.
        /// </summary>
        IDictionary<V, ISet<E>> Parents { get; }

        /// <summary>
        /// Gets the bindings of each vertex to its cost in the path.
        /// </summary>
        IDictionary<V, IWeight> Costs { get; }
    }
}