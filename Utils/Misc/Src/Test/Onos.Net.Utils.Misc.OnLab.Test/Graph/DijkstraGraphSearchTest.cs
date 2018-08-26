using Onos.Net.Utils.Misc.OnLab.Graph;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    using TestAbstractGraphPathSearch = AbstractGraphPathSearch<TestVertex, TestEdge>;
    using TestAdjacencyListsGraph = AdjacencyListsGraph<TestVertex, TestEdge>;

    public class DijkstraGraphSearchTest : BreadthFirstSearchTest
    {
        protected override TestAbstractGraphPathSearch GraphSearch => new DijkstraGraphSearch<TestVertex, TestEdge>();

        [Fact]
        public override void DefaultGraphTest() => ExecuteDefaultTest(7, 5, new TestDoubleWeight(5.0));

        [Fact]
        public override void DefaultHopCountWeight()
        {
            Weigher = null;
            ExecuteDefaultTest(10, 3, new ScalarWeight(3.0));
        }

        [Fact]
        public void NoPath()
        {
            var set1 = new HashSet<TestVertex>() { A, B, C, D }.ToImmutableHashSet();
            var set2 = new HashSet<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(B, A, W1),
                new TestEdge(C, D, W1),
                new TestEdge(D, C, W1),
            }.ToImmutableHashSet();

            Graph = new TestAdjacencyListsGraph(set1, set2);
            var gs = GraphSearch;
            var paths = gs.Search(Graph, A, B, Weigher, 1).Paths;
            PrintPaths(paths);
            Assert.Equal(1, paths.Count);
            Assert.Equal(new TestDoubleWeight(1.0), paths.First().Cost);

            paths = gs.Search(Graph, A, D, Weigher, 1).Paths;
            PrintPaths(paths);
            Assert.Equal(0, paths.Count);

            paths = gs.Search(Graph, A, null, Weigher, 1).Paths;
            PrintPaths(paths);
            Assert.Equal(1, paths.Count);
            Assert.Equal(new TestDoubleWeight(1.0), paths.First().Cost);
        }

        [Fact]
        public void Exceptions()
        {
            var set1 = new HashSet<TestVertex>() { A, B, C, D }.ToImmutableHashSet();
            var set2 = new HashSet<TestEdge>()
            {
                new TestEdge(A, B, W2),
                new TestEdge(B, A, W1),
                new TestEdge(A, A, W3),
                new TestEdge(A, C, NW1),
                new TestEdge(C, D, W3),
            }.ToImmutableHashSet();
            Graph = new TestAdjacencyListsGraph(set1, set2);
            var gs = GraphSearch;
            var paths = gs.Search(Graph, A, D, Weigher, TestAbstractGraphPathSearch.AllPaths).Paths;
            PrintPaths(paths);
            Assert.Equal(0, paths.Count);

            paths = gs.Search(Graph, A, A, Weigher, 5).Paths;
            // Should find no paths between the same vertices.
            Assert.Equal(0, paths.Count);
            // Invalid operation since there should be no paths.
            Assert.Throws<InvalidOperationException>(() => paths.First().Cost);
        }

        [Fact]
        public void NoEdges()
        {
            Graph = new TestAdjacencyListsGraph(Vertices, new HashSet<TestEdge>());
            foreach (TestVertex v in Vertices)
            {
                ExecuteSearch(GraphSearch, Graph, v, null, Weigher, 0, new TestDoubleWeight(0));
            }
        }

        [Fact]
        public void SimpleMultiplePath()
        {
            var set1 = new HashSet<TestVertex>() { A, B, C, D }.ToImmutableHashSet();
            var set2 = new HashSet<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(A, C, W1),
                new TestEdge(B, D, W1),
                new TestEdge(C, D, W1),
                new TestEdge(D, E, W1),
                new TestEdge(A, E, ZW),
                new TestEdge(E, F, NW1),
                new TestEdge(F, B, ZW),
            }.ToImmutableHashSet();
            Graph = new TestAdjacencyListsGraph(set1, set2);
            ExecuteSearch(GraphSearch, Graph, A, D, Weigher, 2, W2);
            ExecuteSinglePathSearch(GraphSearch, Graph, A, D, Weigher, 1, W2);
            ExecuteSearch(GraphSearch, Graph, A, B, Weigher, 1, W1);
            ExecuteSearch(GraphSearch, Graph, D, A, Weigher, 0, null);
        }

        [Fact]
        public void ManualDoubleWeights()
        {
            var set1 = new HashSet<TestVertex>() { A, B, C, D, E }.ToImmutableHashSet();
            var set2 = new HashSet<TestEdge>()
            {
                new TestEdge(A, B, new TestDoubleWeight(1.5)),
                new TestEdge(B, D, new TestDoubleWeight(3.5)),
                new TestEdge(A, C, new TestDoubleWeight(2.2)),
                new TestEdge(C, E, new TestDoubleWeight(1.1)),
                new TestEdge(E, D, new TestDoubleWeight(1.7)),
                new TestEdge(A, D, new TestDoubleWeight(5.0)),
            }.ToImmutableHashSet();
            Graph = new TestAdjacencyListsGraph(set1, set2);
            ExecuteSearch(GraphSearch, Graph, A, D, Weigher, 3, new TestDoubleWeight(5.0));
            ExecuteSinglePathSearch(GraphSearch, Graph, A, D, Weigher, 1, W5);
        }

        [Fact]
        public void DenseMultiplePath()
        {
            var set1 = new HashSet<TestVertex>() { A, B, C, D, E, F, G }.ToImmutableHashSet();
            var set2 = new HashSet<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(A, C, W1),
                new TestEdge(B, D, W1),
                new TestEdge(C, D, W1),
                new TestEdge(D, E, W1),
                new TestEdge(D, F, W1),
                new TestEdge(E, G, W1),
                new TestEdge(F, G, W1),
                new TestEdge(A, G, W4),
            }.ToImmutableHashSet();
            Graph = new TestAdjacencyListsGraph(set1, set2);
            ExecuteSearch(GraphSearch, Graph, A, G, Weigher, 5, W4);
            ExecuteSinglePathSearch(GraphSearch, Graph, A, G, Weigher, 1, W4);
        }

        [Fact]
        public void DualEdgeMultiplePath()
        {
            var set1 = new HashSet<TestVertex>() { A, B, C, D, E, F, G, H }.ToImmutableHashSet();
            var set2 = new HashSet<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(A, C, W3),
                new TestEdge(B, D, W2),
                new TestEdge(B, C, W1),
                new TestEdge(B, E, W4),
                new TestEdge(C, E, W1),
                new TestEdge(D, H, W5),
                new TestEdge(D, E, W1),
                new TestEdge(E, F, W1),
                new TestEdge(F, D, W1),
                new TestEdge(F, G, W1),
                new TestEdge(F, H, W1),
                new TestEdge(A, E, W3),
                new TestEdge(B, D, W1),
            }.ToImmutableHashSet();
            Graph = new TestAdjacencyListsGraph(set1, set2);
            ExecuteSearch(GraphSearch, Graph, A, E, Weigher, 3, W3);
            ExecuteSinglePathSearch(GraphSearch, Graph, A, E, Weigher, 1, W3);

            var gs = GraphSearch;
            var pathF = gs.Search(Graph, A, F, Weigher, TestAbstractGraphPathSearch.AllPaths).Paths;
            var pathE = gs.Search(Graph, A, E, Weigher, TestAbstractGraphPathSearch.AllPaths).Paths;
            Assert.Equal(0, pathF.Count - pathE.Count);
            Assert.Equal(new TestDoubleWeight(1.0), pathF.First().Cost.Subtract(pathE.First().Cost));
        }

        [Fact]
        public void NegativeWeights()
        {
            var set1 = new HashSet<TestVertex>() { A, B, C, D, E, F, G }.ToImmutableHashSet();
            var set2 = new HashSet<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(A, C, NW1),
                new TestEdge(B, D, W1),
                new TestEdge(D, A, NW2),
                new TestEdge(C, D, W1),
                new TestEdge(D, E, W1),
                new TestEdge(D, F, W1),
                new TestEdge(E, G, W1),
                new TestEdge(F, G, W1),
                new TestEdge(G, A, NW5),
                new TestEdge(A, G, W4),
            }.ToImmutableHashSet();
            Graph = new TestAdjacencyListsGraph(set1, set2);
            ExecuteSearch(GraphSearch, Graph, A, G, Weigher, 3, new TestDoubleWeight(4.0));
            ExecuteSinglePathSearch(GraphSearch, Graph, A, G, Weigher, 1, new TestDoubleWeight(4.0));
        }

        [Fact]
        public void Disconnected()
        {
            var vertices = new HashSet<TestVertex>();
            for (int i = 0; i < 200; ++i)
            {
                vertices.Add(new TestVertex($"v{i}"));
            }

            Graph = new TestAdjacencyListsGraph(vertices, new HashSet<TestEdge>().ToImmutableHashSet());
            Stopwatch sw = Stopwatch.StartNew();
            foreach (var src in vertices)
            {
                ExecuteSearch(GraphSearch, Graph, src, null, null, 0, new TestDoubleWeight(0));
            }
            var elapsed = sw.ElapsedTicks;
            Console.WriteLine($"Compute cost is {elapsed}");
        }
    }
}
