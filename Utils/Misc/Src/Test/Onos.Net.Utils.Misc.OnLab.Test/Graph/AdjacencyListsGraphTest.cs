using Onos.Net.Utils.Misc.OnLab.Graph;
using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    using TestAdjacencyListsGraph = AdjacencyListsGraph<TestVertex, TestEdge>;
    public class AdjacencyListsGraphTest
    {
        private static readonly TestVertex A = new TestVertex("A");
        private static readonly TestVertex B = new TestVertex("B");
        private static readonly TestVertex C = new TestVertex("C");
        private static readonly TestVertex D = new TestVertex("D");
        private static readonly TestVertex E = new TestVertex("E");
        private static readonly TestVertex F = new TestVertex("F");
        private static readonly TestVertex G = new TestVertex("G");

        private static readonly ISet<TestEdge> edges = new HashSet<TestEdge>()
        {
            new TestEdge(A, B),
            new TestEdge(B, C),
            new TestEdge(C, D),
            new TestEdge(D, A),
            new TestEdge(B, D),
        }.ToImmutableHashSet();

        [Fact]
        public void Equality()
        {
            ImmutableHashSet<TestVertex> vertices = new HashSet<TestVertex>() { A, B, C, D, E, F }.ToImmutableHashSet();
            ImmutableHashSet<TestVertex> vertices2 = new HashSet<TestVertex>() { A, B, C, D, E, F, G }.ToImmutableHashSet();

            var graph = new TestAdjacencyListsGraph(vertices, edges);
            var same = new TestAdjacencyListsGraph(vertices, edges);
            var different = new TestAdjacencyListsGraph(vertices2, edges);

            Assert.Equal(graph, same);
            Assert.NotEqual(graph, different);
        }

        [Fact]
        public void Basics()
        {
            ImmutableHashSet<TestVertex> vertices = new HashSet<TestVertex>() { A, B, C, D, E, F }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            Assert.Equal(6, graph.Vertices.Count);
            Assert.Equal(5, graph.Edges.Count);

            Assert.Equal(1, graph.GetEdgesFrom(A).Count);
            Assert.Equal(1, graph.GetEdgesTo(A).Count);
            Assert.Equal(1, graph.GetEdgesTo(C).Count);
            Assert.Equal(2, graph.GetEdgesFrom(B).Count);
            Assert.Equal(2, graph.GetEdgesTo(D).Count);
        }
    }
}