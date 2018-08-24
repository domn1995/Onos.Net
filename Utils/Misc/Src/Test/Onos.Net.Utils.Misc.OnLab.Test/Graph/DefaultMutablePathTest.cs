using Onos.Net.Utils.Misc.OnLab.Graph;
using System.Collections.Generic;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    public class DefaultMutablePathTest : DefaultPathTest
    {
        [Fact]
        public override void Equality()
        {
            List<TestEdge> edges1 = new List<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, C),
            };
            List<TestEdge> edges2 = new List<TestEdge>()
            {
                new TestEdge(A, B),
                new TestEdge(B, D),
            };

            var p1 = new DefaultPath<TestVertex, TestEdge>(edges1, new TestDoubleWeight(2.0));
            var p2 = new DefaultPath<TestVertex, TestEdge>(new List<TestEdge>(edges1), new TestDoubleWeight(2.0));
            var p3 = new DefaultPath<TestVertex, TestEdge>(edges2, new TestDoubleWeight(2.0));

            Assert.Equal(p1, p2);
            Assert.NotEqual(p1, p3);
        }

        [Fact]
        public void Empty()
        {
            var p = new DefaultMutablePath<TestVertex, TestEdge>();
            Assert.Null(p.Src);
            Assert.Null(p.Dst);
            Assert.Equal(0, p.Edges.Count);
            Assert.Null(p.Cost);
        }

        [Fact]
        public void PathCost()
        {
            var p = new DefaultMutablePath<TestVertex, TestEdge>();
            var weight = new TestDoubleWeight(4);
            p.Cost = weight;
            Assert.Equal(weight, p.Cost);
        }

        private void ValidatePath(IPath<TestVertex, TestEdge> p,
            TestVertex src, TestVertex dst, int length)
        {
            ValidatePath(p, src, dst, length, null);
        }

        [Fact]
        public void InsertEdge()
        {
            var p = new DefaultMutablePath<TestVertex, TestEdge>();
            p.InsertEdge(new TestEdge(B, C));
            p.InsertEdge(new TestEdge(A, B));
            ValidatePath(p, A, C, 2);
        }

        [Fact]
        public void AppendEdge()
        {
            var p = new DefaultMutablePath<TestVertex, TestEdge>();
            p.AppendEdge(new TestEdge(A, B));
            p.AppendEdge(new TestEdge(B, C));
            ValidatePath(p, A, C, 2);
        }

        [Fact]
        public void RemoveEdge()
        {
            var p = new DefaultMutablePath<TestVertex, TestEdge>();
            p.AppendEdge(new TestEdge(A, B));
            p.AppendEdge(new TestEdge(B, C));
            p.AppendEdge(new TestEdge(C, C));
            p.AppendEdge(new TestEdge(C, D));
            ValidatePath(p, A, D, 4);

            p.RemoveEdge(new TestEdge(A, B));
            ValidatePath(p, B, D, 3);

            p.RemoveEdge(new TestEdge(C, C));
            ValidatePath(p, B, D, 2);

            p.RemoveEdge(new TestEdge(C, D));
            ValidatePath(p, B, C, 1);
        }

        [Fact]
        public void ToImmutable()
        {
            var p = new DefaultMutablePath<TestVertex, TestEdge>();
            p.AppendEdge(new TestEdge(A, B));
            p.AppendEdge(new TestEdge(B, C));
            ValidatePath(p, A, C, 2);

            // Immutables should equal.
            Assert.Equal(p.ToImmutable(), p.ToImmutable());
            ValidatePath(p.ToImmutable(), A, C, 2);
        }
    }
}
