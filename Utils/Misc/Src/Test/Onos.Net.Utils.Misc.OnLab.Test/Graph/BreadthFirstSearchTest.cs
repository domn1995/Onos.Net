using Onos.Net.Utils.Misc.OnLab.Graph;
using System;
using System.Linq;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    public class BreadthFirstSearchTest : AbstractGraphPathSearchTest
    {
        protected override AbstractGraphPathSearch<TestVertex, TestEdge> GraphSearch => new BreadthFirstSearch<TestVertex, TestEdge>();

        [Fact]
        public virtual void DefaultGraphTest()
        {
            ExecuteDefaultTest(7, 3, new TestDoubleWeight(8.0));
        }

        [Fact]
        public virtual void DefaultHopCountWeight()
        {
            Weigher = null;
            ExecuteDefaultTest(7, 3, new ScalarWeight(3.0));
        }

        protected void ExecuteDefaultTest(int pathCount, int pathLength, IWeight pathCost)
        {
            Graph = new AdjacencyListsGraph<TestVertex, TestEdge>(Vertices, Edges);
            IGraphPathSearch<TestVertex, TestEdge> search = GraphSearch;
            var paths = search.Search(Graph, A, H, Weigher, AbstractGraphPathSearch<TestVertex, TestEdge>.AllPaths).Paths;
            Assert.Equal(1, paths.Count);
            var p = paths.First();
            Assert.Equal(A, p.Src);
            Assert.Equal(H, p.Dst);
            Assert.Equal(pathLength, p.Edges.Count);
            Assert.Equal(pathCost, p.Cost);
            paths = search.Search(Graph, A, null, Weigher, AbstractGraphPathSearch<TestVertex, TestEdge>.AllPaths).Paths;
            PrintPaths(paths);
            Assert.Equal(pathCount, paths.Count);
        }

        protected void ExecuteSearch(IGraphPathSearch<TestVertex, TestEdge> search,
            IGraph<TestVertex, TestEdge> graph, TestVertex src, TestVertex dst,
            IEdgeWeigher<TestVertex, TestEdge> weigher, int pathCount, IWeight pathCost)
        {
            var result = search.Search(graph, src, dst, weigher, AbstractGraphPathSearch<TestVertex, TestEdge>.AllPaths);
            var paths = result.Paths;
            PrintPaths(paths);
            Assert.Equal(pathCount, paths.Count);
            if (pathCount > 0)
            {
                var path = paths.First();
                Assert.Equal(pathCost, path.Cost);
            }
        }

        protected void ExecuteSinglePathSearch(IGraphPathSearch<TestVertex, TestEdge> search,
           IGraph<TestVertex, TestEdge> graph, TestVertex src, TestVertex dst,
           IEdgeWeigher<TestVertex, TestEdge> weigher, int pathCount, IWeight pathCost)
        {
            var result = search.Search(graph, src, dst, weigher, 1);
            var paths = result.Paths;
            PrintPaths(paths);
            Assert.Equal(Math.Min(pathCount, 1), paths.Count);
            if (pathCount > 0)
            {
                var path = paths.First();
                Assert.Equal(pathCost, path.Cost);
            }
        }
    }
}