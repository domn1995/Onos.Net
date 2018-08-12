namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Enables implementing classes to represent a graph edge.
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    public interface IEdge<V> where V : IVertex
    {
        /// <summary>
        /// Gets the edge source vertex.
        /// </summary>
        V Src { get; }

        /// <summary>
        /// Gets the edge destination vertex.
        /// </summary>
        V Dst { get; }
    }
}