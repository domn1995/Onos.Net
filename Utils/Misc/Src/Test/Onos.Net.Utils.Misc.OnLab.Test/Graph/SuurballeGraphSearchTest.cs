using System.Collections.Generic;
using System.Collections.Immutable;
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
    }
}
