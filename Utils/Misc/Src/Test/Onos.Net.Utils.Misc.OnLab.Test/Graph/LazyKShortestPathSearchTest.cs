using Onos.Net.Utils.Misc.OnLab.Graph;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    using TestAdjacencyListsGraph = AdjacencyListsGraph<TestVertex, TestEdge>;
    using TestLazyKShortestPathsSearch = LazyKShortestPathsSearch<TestVertex, TestEdge>;

    public class LazyKShortestPathSearchTest : GraphTest
    {
        private TestLazyKShortestPathsSearch NewSearch => new TestLazyKShortestPathsSearch();

        [Fact]
        public void NoPath()
        {
            ImmutableHashSet<TestVertex> vertices = new HashSet<TestVertex>() { A, B, C, D }.ToImmutableHashSet();
            ImmutableHashSet<TestEdge> edges = new HashSet<TestEdge>()
            {
                new TestEdge(A, B), new TestEdge(B, A),
                new TestEdge(C, D), new TestEdge(D, C),
            }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            IEnumerable<IPath<TestVertex, TestEdge>> result = NewSearch.LazyPathSearch(graph, A, D, Weigher);
            Assert.Empty(result);
        }

        [Fact]
        public void FourPath()
        {
            var graph = new TestAdjacencyListsGraph(Vertices, Edges);
            IEnumerable<IPath<TestVertex, TestEdge>> result = NewSearch.LazyPathSearch(graph, A, E, Weigher);
            List<IPath<TestVertex, TestEdge>> rList = result.Take(42).ToList();
            Assert.Equal(4, rList.Count);

            var expectedEdges = new List<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(B, C, W1),
                new TestEdge(C, E, W1),
            };

            Assert.Equal(expectedEdges, rList[0].Edges);
            Assert.Equal(W3, rList[0].Cost);

            expectedEdges = new List<TestEdge>()
            {
                new TestEdge(A, C, W3),
                new TestEdge(C, E, W1),
            };

            var alternateEdges = new List<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(B, D, W2),
                new TestEdge(D, E, W1),
            };

            ImmutableHashSet<TestEdge> actual = new HashSet<TestEdge>(rList[1].Edges.Concat(rList[2].Edges)).ToImmutableHashSet();
            ImmutableHashSet<TestEdge> expected = new HashSet<TestEdge>(expectedEdges.Concat(alternateEdges)).ToImmutableHashSet();

            Assert.Subset(expected, actual);

            Assert.Equal(W4, rList[1].Cost);
            Assert.Equal(W4, rList[2].Cost);

            expectedEdges = new List<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(B, E, W4),
            };

            Assert.Equal(expectedEdges, rList[3].Edges);
            Assert.Equal(W5, rList[3].Cost);
        }
    }
}