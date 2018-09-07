using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Onos.Net.Utils.Misc.OnLab.Graph;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    using TestAbstractGraphPathSearch = AbstractGraphPathSearch<TestVertex, TestEdge>;
    public class SrlgGraphSearchTest : BreadthFirstSearchTest
    {
        protected override TestAbstractGraphPathSearch GraphSearch => new SrlgGraphSearch<TestVertex, TestEdge>();

        public void SetDefaultWeights() => Weigher = null;

        public override void DefaultGraphTest() { }

        public override void DefaultHopCountWeight() { }

        public void CheckIsDisjoint(IPath<TestVertex, TestEdge> p, IDictionary<TestEdge, int> risks)
        {
            Assert.True(p is DisjointPathPair<TestVertex, TestEdge>);
            var q = (DisjointPathPair<TestVertex, TestEdge>)p;
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
            var graph = new AdjacencyListsGraph<TestVertex, TestEdge>(vertices, edges);
            var riskProfile = new Dictionary<TestEdge, int>()
            {
                { aB, 0 }, { bC, 0 }, { aD, 1 }, { dC, 1 },
            }.ToImmutableDictionary();
            var search = new SrlgGraphSearch<TestVertex, TestEdge>(2, riskProfile);
            var paths = search.Search(graph, A, C, Weigher).Paths;
            Console.WriteLine($"\n\n\n{paths}\n\n\n");
            Assert.Single(paths);
            CheckIsDisjoint(paths.First(), riskProfile);
        }
    }
}