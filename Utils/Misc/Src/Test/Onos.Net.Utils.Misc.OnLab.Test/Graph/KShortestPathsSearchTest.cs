using Onos.Net.Utils.Misc.OnLab.Graph;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    using TestAdjacencyListsGraph = AdjacencyListsGraph<TestVertex, TestEdge>;
    using TestKShortestPathsSearch = KShortestPathsSearch<TestVertex, TestEdge>;

    public class KShortestPathsSearchTest : GraphTest
    {
        private TestKShortestPathsSearch NewSearch => new TestKShortestPathsSearch();
        private TestAdjacencyListsGraph NewGraph => new TestAdjacencyListsGraph(Vertices, Edges);

        [Fact]
        public void NoPath()
        {
            var vertices = new HashSet<TestVertex>() { A, B, C, D }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, A),
                new TestEdge(C, D),
                new TestEdge(D, C),
            }.ToImmutableHashSet();

            var graph = new TestAdjacencyListsGraph(vertices, edges);
            var result = NewSearch.Search(graph, A, D, Weigher, 1);
            var resultPathSet = result.Paths;
            Assert.True(resultPathSet.Count == 0);
        }

        [Fact]
        public void SinglePath()
        {
            var result = NewSearch.Search(NewGraph, A, B, Weigher, 2);
            Assert.Equal(1, result.Paths.Count);
            var correctEdgeList = new List<TestEdge>()
            {
                new TestEdge(A, B, W1),
            };
            Assert.True(correctEdgeList.SequenceEqual(result.Paths.First().Edges));
        }

        [Fact]
        public void ResultsAreOnlyOneHopPathPlusLongerOnes()
        {
            var result = NewSearch.Search(NewGraph, B, D, HopWeigher, 42);
            var paths = result.Paths.ToList();
            Assert.Equal(3, paths.Count);
            // The shorest path is 1 hop.
            Assert.Equal(1, paths[0].Edges.Count);
            // The 2nd shortest path is 3 hops.
            Assert.Equal(3, paths[1].Edges.Count);
            // The 3rd shortest path is 4 hops.
            Assert.Equal(4, paths[2].Edges.Count);
        }

        [Fact]
        public void TwoPath()
        {
            var result = NewSearch.Search(NewGraph, A, C, Weigher, 3);
            Assert.True(result.Paths.Count == 2);
            var paths = result.Paths.ToList();
            var correctEdgeList = new List<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(B, C, W1),
            };
            Assert.True(paths[0].Edges.SequenceEqual(correctEdgeList));
            correctEdgeList.Clear();
            correctEdgeList.Add(new TestEdge(A, C, W3));
            Assert.True(paths[1].Edges.SequenceEqual(correctEdgeList));
        }

        [Fact]
        public void FourPath()
        {
            var result = NewSearch.Search(NewGraph, A, E, Weigher, 5);
            Assert.True(result.Paths.Count == 4);
            var paths = result.Paths.ToList();
            var correctEdgeList = new List<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(B, C, W1),
                new TestEdge(C, E, W1),
            };
            Assert.True(paths[0].Edges.SequenceEqual(correctEdgeList));
            correctEdgeList.Clear();

            correctEdgeList = new List<TestEdge>()
            {
                new TestEdge(A, C, W3),
                new TestEdge(C, E, W1),
            };
            var alternateCorrectEdgeList = new List<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(B, D, W2),
                new TestEdge(D, E, W1),
            };
            var candidateOne = paths[1].Edges;
            var candidateTwo = paths[2].Edges;
            if (candidateOne.Count == 2)
            {
                Assert.True(candidateOne.SequenceEqual(correctEdgeList));
                Assert.True(candidateTwo.SequenceEqual(alternateCorrectEdgeList));
            }
            else
            {
                Assert.True(candidateOne.SequenceEqual(alternateCorrectEdgeList));
                Assert.True(candidateTwo.SequenceEqual(correctEdgeList));
            }
            correctEdgeList.Clear();
            correctEdgeList = new List<TestEdge>()
            {
                new TestEdge(A, B, W1),
                new TestEdge(B, E, W4),
            };
            Assert.True(paths[3].Edges.SequenceEqual(correctEdgeList));
        }

        [Fact]
        public void PathsFromSink()
        {
            foreach (TestVertex vertex in Vertices)
            {
                Assert.True(NewSearch.Search(NewGraph, H, vertex, Weigher, 1).Paths.Count == 0);
            }
        }

        [Fact]
        public void LimitPathSetSize()
        {
            var result = NewSearch.Search(NewGraph, A, E, Weigher, 3);
            Assert.Equal(3, result.Paths.Count);
            result = NewSearch.Search(NewGraph, A, G, Weigher, 1);
            Assert.Equal(1, result.Paths.Count);
        }

        [Fact]
        public void VariableLenPathsWithConstantLinkWeight()
        {
            var vertices = new HashSet<TestVertex>() { A, B, C, D, E, F, G, H }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, A), new TestEdge(B, C),
                new TestEdge(C, B), new TestEdge(C, D),
                new TestEdge(D, C), new TestEdge(D, E),
                new TestEdge(E, D), new TestEdge(E, H),
                new TestEdge(H, E), new TestEdge(H, G),
                new TestEdge(G, H), new TestEdge(G, F),
                new TestEdge(F, G), new TestEdge(F, B),
                new TestEdge(B, F),
            }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            var result = NewSearch.Search(graph, A, G, Weigher, 8);
            Assert.Equal(2, result.Paths.Count);
            var paths = result.Paths.ToList();
            var correctEdgeList = new List<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, F),
                new TestEdge(F, G),
            };
            Assert.True(paths[0].Edges.SequenceEqual(correctEdgeList));
            correctEdgeList.Clear();
            correctEdgeList = new List<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, C),
                new TestEdge(C, D),
                new TestEdge(D, E),
                new TestEdge(E, H),
                new TestEdge(H, G),
            };
            Assert.True(paths[1].Edges.SequenceEqual(correctEdgeList));
        }
    }
}