using Onos.Net.Utils.Misc.OnLab.Graph;
using System.Collections.Generic;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    public class DefaultPathTest : GraphTest
    {
        [Fact]
        public virtual void Equality()
        {
            List<TestEdge> edges = new List<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, C),
            };
            Assert.Equal(new DefaultPath<TestVertex, TestEdge>(edges, new TestDoubleWeight(2.0)),
                         new DefaultPath<TestVertex, TestEdge>(new List<TestEdge>(edges), new TestDoubleWeight(2.0)));
            Assert.NotEqual(new DefaultPath<TestVertex, TestEdge>(edges, new TestDoubleWeight(2.0)),
                            new DefaultPath<TestVertex, TestEdge>(edges, new TestDoubleWeight(3.0)));
        }

        [Fact]
        public void Basics()
        {
            List<TestEdge> edges = new List<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, C),
            };
            var p = new DefaultPath<TestVertex, TestEdge>(edges, new TestDoubleWeight(2.0));
            ValidatePath(p, A, C, 2, new TestDoubleWeight(2.0));
        }

        protected void ValidatePath(IPath<TestVertex, TestEdge> p,
            TestVertex src, TestVertex dst, int length, IWeight cost)
        {
            Assert.Equal(length, p.Edges.Count);
            Assert.Equal(src, p.Src);
            Assert.Equal(dst, p.Dst);
            Assert.Equal(cost, p.Cost);
        }
    }
}
