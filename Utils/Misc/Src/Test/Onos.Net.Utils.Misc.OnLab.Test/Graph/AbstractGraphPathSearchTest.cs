using Onos.Net.Utils.Misc.OnLab.Graph;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    using TestAdjacencyListsGraph = AdjacencyListsGraph<TestVertex, TestEdge>;

    public abstract class AbstractGraphPathSearchTest : GraphTest
    {
        protected abstract AbstractGraphPathSearch<TestVertex, TestEdge> GraphSearch { get; }

        private ISet<TestVertex> Set1 => new HashSet<TestVertex>() { B, C }.ToImmutableHashSet();
        private ISet<TestEdge> Set2 => new HashSet<TestEdge>() { new TestEdge(B, C) }.ToImmutableHashSet();

        [Fact]
        public void NoSuchSourceArgument()
        {
            Assert.Throws<ArgumentException>(() => Do());

            void Do()
            {
                var graph = new TestAdjacencyListsGraph(Set1, Set2);
                GraphSearch.Search(graph, A, H, Weigher, 1);
            }
        }

        [Fact]
        public void NullGraphArgument()
        {
            Assert.Throws<ArgumentNullException>(() => Do());

            void Do()
            {
                var graph = new TestAdjacencyListsGraph(Set1, Set2);
                GraphSearch.Search(null, A, H, Weigher, 1);
            }
        }

        [Fact]
        public void NullSourceArgument()
        {
            Assert.Throws<ArgumentNullException>(() => Do());

            void Do()
            {
                var graph = new TestAdjacencyListsGraph(Set1, Set2);
                GraphSearch.Search(graph, null, H, Weigher, 1);
            }
        }        
    }
}