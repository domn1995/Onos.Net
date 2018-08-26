using System;
using System.Linq;
using Onos.Net.Utils.Misc.OnLab.Graph;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    using SpanningTreeResult = DepthFirstSearch<TestVertex, TestEdge>.SpanningTreeResult;
    using TestAbstractGraphPathSearch = AbstractGraphPathSearch<TestVertex, TestEdge>;
    using TestAdjacencyListsGraph = AdjacencyListsGraph<TestVertex, TestEdge>;
    using static DepthFirstSearch<TestVertex, TestEdge>;

    /// <summary>
    /// Tests the DFS algorithm.
    /// </summary>
    public class DepthFirstSearchTest : AbstractGraphPathSearchTest
    {
        protected override TestAbstractGraphPathSearch GraphSearch => new DepthFirstSearch<TestVertex, TestEdge>();

        [Fact]
        public void DefaultGraphTest()
        {
            ExecuteDefaultTest(3, 6, new TestDoubleWeight(5.0), new TestDoubleWeight(12.0));
            ExecuteBroadSearch();
        }

        [Fact]
        public void DefaultHopCountWeight()
        {
            Weigher = null;
            ExecuteDefaultTest(3, 6, new ScalarWeight(3.0), new ScalarWeight(6.0));
            ExecuteBroadSearch();
        }

        protected void ExecuteDefaultTest(int minLength, int maxLength, IWeight minCost, IWeight maxCost)
        {
            Graph = new TestAdjacencyListsGraph(Vertices, Edges);
            var search = GraphSearch;
            SpanningTreeResult result = (SpanningTreeResult)search.Search(Graph, A, H, Weigher, 1);
            var paths = result.Paths;
            Assert.Equal(1, paths.Count);

            var path = paths.First();
            Console.WriteLine(path);
            Assert.Equal(A, path.Src);
            Assert.Equal(H, path.Dst);

            int l = path.Edges.Count;
            Assert.True(minLength <= l && l <= maxLength);
            Assert.True(path.Cost.CompareTo(minCost) >= 0 && path.Cost.CompareTo(maxCost) <= 0);

            Console.WriteLine(result.Edges);
            PrintPaths(paths);
        }

        public void ExecuteBroadSearch()
        {
            Graph = new TestAdjacencyListsGraph(Vertices, Edges);
            var search = GraphSearch;
            SpanningTreeResult result = (SpanningTreeResult)search.Search(Graph, A, null, Weigher, TestAbstractGraphPathSearch.AllPaths);
            Assert.Equal(7, result.Paths.Count);

            int[] types = new int[] { 0, 0, 0, 0 };
            foreach (var type in result.Edges.Values)
            {
                types[(int)type] += 1;
            }
            Assert.Equal(7, types[(int)EdgeType.TreeEdge]);
            Assert.Equal(1, types[(int)EdgeType.BackEdge]);
            Assert.Equal(4, types[(int)EdgeType.ForwardEdge] + types[(int)EdgeType.CrossEdge]);
        }
    }
}