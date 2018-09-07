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

    public class SrlgGraphSearchTest : BreadthFirstSearchTest
    {
        protected override TestAbstractGraphPathSearch GraphSearch => new SrlgGraphSearch<TestVertex, TestEdge>();

        public void SetDefaultWeights() => Weigher = null;

        public override void DefaultGraphTest() { }

        public override void DefaultHopCountWeight() { }

        public void CheckIsDisjoint(IPath<TestVertex, TestEdge> p, IDictionary<TestEdge, int> risks)
        {
            Assert.True(p is DisjointPathPair<TestVertex, TestEdge>);
            var q = (TestDisjointPathPair)p;
            var plRisks = new HashSet<int>();
            foreach (TestEdge e in q.Edges)
            {
                plRisks.Add(risks[e]);
            }

            if (!q.HasBackup)
            {
                return;
            }

            var pq = q.Secondary;

            foreach (TestEdge e in pq.Edges)
            {
                Assert.True(!plRisks.Contains(risks[e]));
            }
        }

        [Fact]
        public void OnePathPair()
        {
            SetDefaultWeights();
            var aB = new TestEdge(A, B);
            var bC = new TestEdge(B, C);
            var aD = new TestEdge(A, D);
            var dC = new TestEdge(D, C);
            var vertices = new HashSet<TestVertex>() { A, B, C, D }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>() { aB, bC, aD, dC }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            var riskProfile = new Dictionary<TestEdge, int>()
            {
                { aB, 0 }, { bC, 0 }, { aD, 1 }, { dC, 1 },
            }.ToImmutableDictionary();
            var search = new TestSrlgGraphSearch(2, riskProfile);
            var paths = search.Search(graph, A, C, Weigher).Paths;
            Console.WriteLine($"\n\n\n{paths}\n\n\n");
            Assert.Single(paths);
            CheckIsDisjoint(paths.First(), riskProfile);
        }

        [Fact]
        public void ComplexGraph()
        {
            SetDefaultWeights();
            var aB = new TestEdge(A, B);
            var bC = new TestEdge(B, C);
            var aD = new TestEdge(A, D);
            var dC = new TestEdge(D, C);
            var cE = new TestEdge(C, E);
            var bE = new TestEdge(B, E);
            var vertices = new HashSet<TestVertex>() { A, B, C, D, E }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>() { aB, bC, aD, dC, cE, bE }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            var riskProfile = new Dictionary<TestEdge, int>()
            {
                { aB, 0 }, { bC, 0 }, { aD, 1}, { dC, 1 }, { cE, 2 }, { bE, 3 },
            }.ToImmutableDictionary();
            var search = new TestSrlgGraphSearch(4, riskProfile);
            var paths = search.Search(graph, A, E, Weigher).Paths;
        }

        [Fact]
        public void MultiplePathGraph()
        {
            SetDefaultWeights();
            var aB = new TestEdge(A, B);
            var bE = new TestEdge(B, E);
            var aD = new TestEdge(A, D);
            var dE = new TestEdge(D, E);
            var aC = new TestEdge(A, C);
            var cE = new TestEdge(C, E);
            var vertices = new HashSet<TestVertex>() { A, B, C, D, E, }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>() { aB, bE, aD, dE, aC, cE }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            var riskProfile = new Dictionary<TestEdge, int>()
            {
                { aB, 0 }, { bE, 1 }, { aD, 2 }, { dE, 3}, { aC, 4 }, { cE, 5 }
            }.ToImmutableDictionary();
            var search = new TestSrlgGraphSearch(6, riskProfile);
            var paths = search.Search(graph, A, E, Weigher).Paths;
            Assert.True(paths.Count >= 1);
            CheckIsDisjoint(paths.First(), riskProfile);
        }

        [Fact]
        public void OnePath()
        {
            SetDefaultWeights();
            var aB = new TestEdge(A, B);
            var bC = new TestEdge(B, C);
            var aD = new TestEdge(A, D);
            var dC = new TestEdge(D, C);
            var vertices = new HashSet<TestVertex>() { A, B, C, D }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>() { aB, bC, aD, dC }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            var riskProfile = new Dictionary<TestEdge, int>()
            {
                { aB, 0 }, { bC, 0 }, { aD, 1 }, { dC, 0 },
            }.ToImmutableDictionary();
            var search = new TestSrlgGraphSearch(2, riskProfile);
            var paths = search.Search(graph, A, C, Weigher).Paths;
            Console.WriteLine(paths);
            Assert.Empty(paths);
        }

        [Fact]
        public void NoPath()
        {
            SetDefaultWeights();
            var aB = new TestEdge(A, B);
            var bC = new TestEdge(B, C);
            var aD = new TestEdge(A, D);
            var dC = new TestEdge(D, C);
            var vertices = new HashSet<TestVertex>() { A, B, C, D, E }.ToImmutableHashSet();
            var edges = new HashSet<TestEdge>() { aB, bC, aD, dC }.ToImmutableHashSet();
            var graph = new TestAdjacencyListsGraph(vertices, edges);
            var riskProfile = new Dictionary<TestEdge, int>()
            {
                { aB, 0 }, { bC, 0 }, { aD, 1 }, { dC, 0 },
            }.ToImmutableDictionary();
            var search = new TestSrlgGraphSearch(2, riskProfile);
            var paths = search.Search(graph, A, E, Weigher).Paths;
            Console.WriteLine(paths);
            Assert.Empty(paths);
        }
    }
}