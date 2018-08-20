using Onos.Net.Utils.Misc.OnLab.Graph;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    public class GraphTest
    {
        internal static TestVertex A = new TestVertex("A");
        internal static TestVertex B = new TestVertex("B");
        internal static TestVertex C = new TestVertex("C");
        internal static TestVertex D = new TestVertex("D");
        internal static TestVertex E = new TestVertex("E");
        internal static TestVertex F = new TestVertex("F");
        internal static TestVertex G = new TestVertex("G");
        internal static TestVertex H = new TestVertex("H");
        internal static TestVertex I = new TestVertex("I");

        internal static TestDoubleWeight ZW = new TestDoubleWeight(0);
        internal static TestDoubleWeight NW5 = new TestDoubleWeight(-5);
        internal static TestDoubleWeight NW2 = new TestDoubleWeight(-2);
        internal static TestDoubleWeight NW1 = new TestDoubleWeight(-1);
        internal static TestDoubleWeight W1 = new TestDoubleWeight(1);
        internal static TestDoubleWeight W2 = new TestDoubleWeight(2);
        internal static TestDoubleWeight W3 = new TestDoubleWeight(3);
        internal static TestDoubleWeight W4 = new TestDoubleWeight(4);
        internal static TestDoubleWeight W5 = new TestDoubleWeight(5);

        protected IGraph<TestVertex, TestEdge> Graph { get; set; }

        protected ISet<TestVertex> Vertices => new HashSet<TestVertex>() { A, B, C, D, E, F, G, H }.ToImmutableHashSet();

        protected IEdgeWeigher<TestVertex, TestEdge> Weigher { get; set; } = new NormalWeigher();

        protected IEdgeWeigher<TestVertex, TestEdge> HopWeigher { get; set; } = new HopsWeigher();

        /// <summary>
        /// A → B → D → H<para/>
        /// ↓ ↙ ↓ ↙ ↑ ↗ <para/>
        /// C → E → F → G
        /// </summary>
        protected ISet<TestEdge> Edges
        {
            get
            {
                return new HashSet<TestEdge>()
                {
                    new TestEdge(A, B, W1),
                    new TestEdge(A, C, W3),
                    new TestEdge(B, D, W2),
                    new TestEdge(B, C, W1),
                    new TestEdge(B, E, W4),
                    new TestEdge(C, E, W1),
                    new TestEdge(D, H, W5),
                    new TestEdge(D, E, W1),
                    new TestEdge(E, F, W1),
                    new TestEdge(F, D, W1),
                    new TestEdge(F, G, W1),
                    new TestEdge(F, H, W1),
                }
                .ToImmutableHashSet();
            }
        }

        protected void PrintPaths(ISet<IPath<TestVertex, TestEdge>> paths)
        {
            foreach (var p in paths)
            {
                Console.WriteLine(p);
            }
        }

        protected class NormalWeigher : IEdgeWeigher<TestVertex, TestEdge>
        {
            public IWeight InitialWeight => ZW;

            public IWeight NonViableWeight => TestDoubleWeight.NonViableWeight;

            public IWeight GetWeight(TestEdge edge) => edge.Weight;
        }

        protected class HopsWeigher : IEdgeWeigher<TestVertex, TestEdge>
        {
            public IWeight InitialWeight => ZW;

            public IWeight NonViableWeight => TestDoubleWeight.NonViableWeight;

            public IWeight GetWeight(TestEdge edge) => W1;
        }


    }


}
