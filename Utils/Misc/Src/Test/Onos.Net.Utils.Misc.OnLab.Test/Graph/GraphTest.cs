using Onos.Net.Utils.Misc.OnLab.Graph;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    public class GraphTest
    {
        internal static readonly TestVertex A = new TestVertex("A");
        internal static readonly TestVertex B = new TestVertex("B");
        internal static readonly TestVertex C = new TestVertex("C");
        internal static readonly TestVertex D = new TestVertex("D");
        internal static readonly TestVertex E = new TestVertex("E");
        internal static readonly TestVertex F = new TestVertex("F");
        internal static readonly TestVertex G = new TestVertex("G");
        internal static readonly TestVertex H = new TestVertex("H");
        internal static readonly TestVertex I = new TestVertex("I");

        internal static readonly TestDoubleWeight ZW = new TestDoubleWeight(0);
        internal static readonly TestDoubleWeight NW5 = new TestDoubleWeight(-5);
        internal static readonly TestDoubleWeight NW2 = new TestDoubleWeight(-2);
        internal static readonly TestDoubleWeight NW1 = new TestDoubleWeight(-1);
        internal static readonly TestDoubleWeight W1 = new TestDoubleWeight(1);
        internal static readonly TestDoubleWeight W2 = new TestDoubleWeight(2);
        internal static readonly TestDoubleWeight W3 = new TestDoubleWeight(3);
        internal static readonly TestDoubleWeight W4 = new TestDoubleWeight(4);
        internal static readonly TestDoubleWeight W5 = new TestDoubleWeight(5);

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
            foreach (IPath<TestVertex, TestEdge> p in paths)
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