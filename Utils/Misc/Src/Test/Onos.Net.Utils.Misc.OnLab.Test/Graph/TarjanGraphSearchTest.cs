using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Onos.Net.Utils.Misc.OnLab.Graph;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    using TestTarjanGraphSearch = TarjanGraphSearch<TestVertex, TestEdge>;
    using TestAdjacencyListsGraph = AdjacencyListsGraph<TestVertex, TestEdge>;

    public class TarjanGraphSearchTest : GraphTest
    {
        private void Validate(TestTarjanGraphSearch.SccResult result, int cc)
        {
            Console.WriteLine($"Cluster count: {result.ClusterVertices.Count}");
            Console.WriteLine($"Clusters: {result.ClusterVertices}");
            Assert.Equal(cc, result.ClusterCount);
        }

        private void Validate(TestTarjanGraphSearch.SccResult result, int i, int vc, int ec)
        {
            Assert.Equal(vc, result.ClusterVertices[i].Count);
            Assert.Equal(ec, result.ClusterEdges[i].Count);
        }
           
        [Fact]
        public void Basic()
        {
            var graph = new TestAdjacencyListsGraph(Vertices, Edges);
            var gs = new TestTarjanGraphSearch();
            var result = (TestTarjanGraphSearch.SccResult)gs.Search(graph, null);
            Validate(result, 6);
        }

        [Fact]
        public void SingleCluster()
        {
            var edges = new HashSet<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, C),
                new TestEdge(C, D),
                new TestEdge(D, E),
                new TestEdge(E, F),
                new TestEdge(F, G),
                new TestEdge(G, H),
                new TestEdge(H, A),
            }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(Vertices, edges);
            var gs = new TestTarjanGraphSearch();
            var result = (TestTarjanGraphSearch.SccResult)gs.Search(graph, null);
            Validate(result, 1);
            Validate(result, 0, 8, 8);
        }

        [Fact]
        public void TwoUnconnectedCluster()
        {
            var edges = new HashSet<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, C),
                new TestEdge(C, D),
                new TestEdge(D, A),
                new TestEdge(E, F),
                new TestEdge(F, G),
                new TestEdge(G, H),
                new TestEdge(H, E),
            }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(Vertices, edges);
            var gs = new TestTarjanGraphSearch();
            var result = (TestTarjanGraphSearch.SccResult)gs.Search(graph, null);
            Validate(result, 2);
            Validate(result, 0, 4, 4);
            Validate(result, 1, 4, 4);
        }

        [Fact]
        public void TwoWeaklyConnectedClusters()
        {
            var edges = new HashSet<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, C),
                new TestEdge(C, D),
                new TestEdge(D, A),
                new TestEdge(E, F),
                new TestEdge(F, G),
                new TestEdge(G, H),
                new TestEdge(H, E),
                new TestEdge(B, E),
            }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(Vertices, edges);
            var gs = new TestTarjanGraphSearch();
            var result = (TestTarjanGraphSearch.SccResult)gs.Search(graph, null);
            Validate(result, 2);
            Validate(result, 0, 4, 4);
            Validate(result, 1, 4, 4);
        }

        [Fact]
        public void TwoClustersConnectedWithIgnoredEdges()
        {
            var edges = new HashSet<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, C),
                new TestEdge(C, D),
                new TestEdge(D, A),
                new TestEdge(E, F),
                new TestEdge(F, G),
                new TestEdge(G, H),
                new TestEdge(H, E),
                new TestEdge(B, E, Weigher.NonViableWeight),
                new TestEdge(E, B, Weigher.NonViableWeight),
            }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(Vertices, edges);
            var gs = new TestTarjanGraphSearch();
            var result = (TestTarjanGraphSearch.SccResult)gs.Search(graph, Weigher);
            Validate(result, 2);
            Validate(result, 0, 4, 4);
            Validate(result, 1, 4, 4);
        }
    }
}