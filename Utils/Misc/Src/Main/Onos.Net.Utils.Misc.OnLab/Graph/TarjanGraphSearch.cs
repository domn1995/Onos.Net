using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Tarjan algorithm for searching a graph and producing results
    /// describing the graph strongly-connected components (SCCs).
    /// </summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    /// <typeparam name="E">The edge type.</typeparam>
    public class TarjanGraphSearch<V, E> : IGraphSearch<V, E> where V : class, IVertex where E : class, IEdge<V>
    {
        /// <summary>
        /// <para>
        /// This implementation produces results augmented with information on SCCs within the graph.
        /// </para>
        /// <para>
        /// To prevent traversal of an edge, the <see cref="IEdgeWeigher{V, E}"/> should
        /// return a negative value as an edge weigher.
        /// </para>
        /// </summary>
        /// <param name="graph">The graph to search.</param>
        /// <param name="weigher">The weigher to use.</param>
        /// <returns>The SCC search result.</returns>
        public IResultBase<V, E> Search(IGraph<V, E> graph, IEdgeWeigher<V, E> weigher)
        {
            SccResult result = new SccResult(graph);
            foreach (V vertex in graph.Vertices)
            {
                VertexData data = result.GetData(vertex);
                if (data is null)
                {
                    Connect(graph, vertex, weigher, result);
                }
            }
            return result.Build();
        }

        /// <summary>
        /// Scans the specified graph, using recursion, and produces SCC results.
        /// </summary>
        /// <param name="graph">The graph to search.</param>
        /// <param name="vertex">The current vertex to scan and connect.</param>
        /// <param name="weigher">The optional weigher to use.</param>
        /// <param name="result">The graph search result.</param>
        /// <returns>Augmentation vertex data for the current vertex.</returns>
        private VertexData Connect(IGraph<V, E> graph, V vertex, IEdgeWeigher<V, E> weigher, SccResult result)
        {
            VertexData data = result.AddData(vertex);

            // Scan through all egress edges of the current vertex.
            foreach (E edge in graph.GetEdgesFrom(vertex))
            {
                V nextVertex = edge.Dst;

                // If edge is not viable, skip it.
                if (weigher != null && !weigher.GetWeight(edge).IsViable)
                {
                    continue;
                }

                // Attempt to get the augmentation vertex data for the next vertex.
                VertexData nextData = result.GetData(nextVertex);
                if (nextData is null)
                {
                    // Next vertex has not been visited yet, so do this now.
                    nextData = Connect(graph, nextVertex, weigher, result);
                    data.LowLink = Math.Min(data.LowLink, nextData.LowLink);
                }
                else if (result.Visited(nextData))
                {
                    // Next vertex has been visited, which means
                    // it is in the same cluster as the current vertex.
                    data.LowLink = Math.Min(data.LowLink, nextData.Index);
                }
            }

            if (data.LowLink == data.Index)
            {
                result.AddCluster(data);
            }
            return data;
        }

        /// <summary>
        /// Graph search result augmented with SCC vertex data.
        /// </summary>
        public sealed class SccResult : IResultBase<V, E>
        {
            private readonly IGraph<V, E> graph;
            private int index = 0;
            private IList<ISet<V>> clusterVertices = new List<ISet<V>>();
            private IList<ISet<E>> clusterEdges = new List<ISet<E>>();
            private readonly Dictionary<V, VertexData> vertexData = new Dictionary<V, VertexData>();
            private readonly List<VertexData> visited = new List<VertexData>();

            /// <summary>
            /// Gets the list of strongly connected vertex clusters.
            /// </summary>
            public IImmutableList<ISet<V>> ClusterVertices => ImmutableList.CreateRange(clusterVertices);

            /// <summary>
            /// Gets the list of edges linking strongly connected vertex clusters.
            /// </summary>
            public IImmutableList<ISet<E>> ClusterEdges => ImmutableList.CreateRange(clusterEdges);

            /// <summary>
            /// Gets the number of SCC clusters in the graph.
            /// </summary>
            public int ClusterCount => clusterEdges.Count;

            /// <summary>
            /// Initializes a new <see cref="SccResult"/> object for the given graph.
            /// </summary>
            /// <param name="graph">The relevant graph.</param>
            public SccResult(IGraph<V, E> graph) => this.graph = graph;

            /// <summary>
            /// Gets the augmentation vertex data for the given vertex.
            /// </summary>
            /// <param name="vertex">The vertex to get the augmentation vertex data for.</param>
            /// <returns>The augmentation vertex data.</returns>
            internal VertexData GetData(V vertex) => vertexData.ContainsKey(vertex) ? vertexData[vertex] : null;

            /// <summary>
            /// Adds augmentation vertex data for the specified vertex.
            /// </summary>
            /// <param name="vertex">The vertex to add.</param>
            /// <returns>The added vertex data.</returns>
            internal VertexData AddData(V vertex)
            {
                VertexData d = new VertexData(vertex, index);
                vertexData.Add(vertex, d);
                visited.Insert(0, d);
                index++;
                return d;
            }

            /// <summary>
            /// Determines whether the given vertex has been visited.
            /// </summary>
            /// <param name="data">The vertex to check.</param>
            /// <returns>True if the vertex has been visited, otherwise false.</returns>
            internal bool Visited(VertexData data) => visited.Contains(data);

            /// <summary>
            /// Adds a new cluster for the given vertex.
            /// </summary>
            /// <param name="data">The vertex to add a new cluster for.</param>
            internal void AddCluster(VertexData data)
            {
                var vertices = FindClusterVertices(data);
                clusterVertices.Add(vertices);
                clusterEdges.Add(FindClusterEdges(vertices));
            }

            private ISet<V> FindClusterVertices(VertexData data)
            {
                VertexData nextVertexData;
                var vertices = new HashSet<V>();
                do
                {
                    nextVertexData = visited[0];
                    visited.RemoveAt(0);
                    vertices.Add(nextVertexData.Vertex);
                }
                while (data != nextVertexData);
                return ImmutableHashSet.CreateRange(vertices);
            }

            private ISet<E> FindClusterEdges(ISet<V> vertices)
            {
                var edges = new HashSet<E>();
                foreach (V vertex in vertices)
                {
                    foreach (E edge in graph.GetEdgesFrom(vertex))
                    {
                        if (vertices.Contains(edge.Dst))
                        {
                            edges.Add(edge);
                        }
                    }
                }
                return ImmutableHashSet.CreateRange(edges);
            }

            internal IResultBase<V, E> Build() => this;
        }

        /// <summary>
        /// Augments the vertex to assist in determining SCC clusters.
        /// </summary>
        internal sealed class VertexData
        {
            internal V Vertex { get; }
            internal int Index { get; }
            internal int LowLink { get; set; }

            public VertexData(V vertex, int index)
            {
                Vertex = vertex;
                Index = index;
                LowLink = index;
            }
        }
    }
}