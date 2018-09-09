using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Onos.Net.Utils.Misc.OnLab.Graph;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    using TestAbstractGraphPathSearch = AbstractGraphPathSearch<TestVertex, TestEdge>;
    using TestAdjacencyListsGraph = AdjacencyListsGraph<TestVertex, TestEdge>;
    using TestDisjointPathPair = DisjointPathPair<TestVertex, TestEdge>;
    using TestSrlgGraphSearch = SrlgGraphSearch<TestVertex, TestEdge>;
    using TestSuurballeGraphSearch = SuurballeGraphSearch<TestVertex, TestEdge>;

    public class SuurballeGraphSearchTest : BreadthFirstSearchTest
    {
        protected override AbstractGraphPathSearch<TestVertex, TestEdge> GraphSearch => new TestSuurballeGraphSearch();

        public override void DefaultGraphTest() { }

        public override void DefaultHopCountWeight() { }

        [Fact]
        public void BasicGraph()
        {
            var vertices = new HashSet<TestVertex>() { A, B, C, D }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, C),
                new TestEdge(A, D),
                new TestEdge(D, C),
            }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            ExecuteSearch(GraphSearch, graph, A, C, null, 1, new ScalarWeight(4.0));
        }

        [Fact]
        public void MultiplePathOnePairGraph()
        {
            var vertices = new HashSet<TestVertex>() { A, B, C, D, E }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(B, C, W1),
                new TestEdge(A, D, W1),
                new TestEdge(D, C, W1),
                new TestEdge(B, E, W2),
                new TestEdge(C, E, W1),
            }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            ExecuteSearch(GraphSearch, graph, A, E, Weigher, 1, new TestDoubleWeight(6.0));
        }

        // TODO: This test is inconsistent.
        // It sometimes includes the same intermediate node in different paths.
        [Fact]
        public void MultiplePathMultiplePairs()
        {
            var vertices = new HashSet<TestVertex>() { A, B, C, D, E }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(B, E, W1),
                new TestEdge(A, C, W1),
                new TestEdge(C, E, W1),
                new TestEdge(A, D, W1),
                new TestEdge(D, E, W1),
                new TestEdge(A, E, W2),
            }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            var result = GraphSearch.Search(graph, A, E, Weigher);
            var paths = result.Paths;
            Console.WriteLine($"\n\n{paths}\n\n\ndone\n");
            Assert.Equal(3, paths.Count);
            var dpp = (TestDisjointPathPair)paths.First();
            Assert.Equal(2, dpp.Size);
        }

        [Fact]
        public void DifferentPrimaryAndBackupPathLengths()
        {
            var vertices = new HashSet<TestVertex>() { A, B, C, D, E }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, C),
                new TestEdge(A, D),
                new TestEdge(D, C),
                new TestEdge(B, E),
                new TestEdge(C, E),
            }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            ExecuteSearch(GraphSearch, graph, A, E, Weigher, 1, new TestDoubleWeight(5.0));
        }

        [Fact]
        public void OnePath()
        {
            var vertices = new HashSet<TestVertex>() { A, B, C, D }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(B, C, W1),
                new TestEdge(A, C, W4),
                new TestEdge(C, D, W1),
            }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            var result = GraphSearch.Search(graph, A, D, Weigher);
            var paths = result.Paths;
            Assert.Single(paths);
            var dpp = (TestDisjointPathPair)paths.First();
            Assert.Equal(1, dpp.Size);
        }

        [Fact]
        public void NoPath()
        {
            var vertices = new HashSet<TestVertex>() { A, B, C, D }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(B, C, W1),
                new TestEdge(A, C, W4),
            }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            var result = GraphSearch.Search(graph, A, D, Weigher);
            var paths = result.Paths;
            Assert.Empty(paths);
        }

        [Fact]
        public void Disconnected()
        {
            var vertices = new HashSet<TestVertex>() { A, B, C, D }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            var result = GraphSearch.Search(graph, A, D, Weigher);
            var paths = result.Paths;
            Assert.Empty(paths);
        }
    }
}