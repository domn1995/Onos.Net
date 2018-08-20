using Onos.Net.Utils.Misc.OnLab.Graph;
using Onos.Net.Utils.Misc.OnLab.Test.Graph;
using System;
using static Onos.Net.Utils.Misc.OnLab.Test.Graph.GraphTest;

namespace Onos.Net.Utils.Misc.OnLab.Test
{
    /// <summary>
    /// Represents a test edge.
    /// </summary>
    public class TestEdge : AbstractEdge<TestVertex>, IEquatable<TestEdge>
    {
        /// <summary>
        /// Gets the edge weight.
        /// </summary>
        public IWeight Weight { get; }

        /// <summary>
        /// Creates a new edge between the specified source and destination vertices with the given weight.
        /// </summary>
        /// <param name="src">The source vertex.</param>
        /// <param name="dst">The destination vertex.</param>
        /// <param name="weight">The edge weight.</param>
        public TestEdge(TestVertex src, TestVertex dst, IWeight weight) : base(src, dst)
        {
            Weight = weight;
        }

        /// <summary>
        /// Creates a new edge between the specified source and destination vertices with the default weight.
        /// </summary>
        /// <param name="src">The source vertex.</param>
        /// <param name="dst">The destination vertex.</param>
        public TestEdge(TestVertex src, TestVertex dst) : this(src, dst, W1)
        {

        }

        public static bool operator ==(TestEdge first, TestEdge second) => first.Equals(second);

        public static bool operator !=(TestEdge first, TestEdge second) => !(first == second);

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is TestEdge e ? IsEqual(e) : false;
        }

        public bool Equals(TestEdge other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(TestEdge other) => base.Equals(other) && Weight == other.Weight;

        public override int GetHashCode() => 31 ^ base.GetHashCode() + HashCode.Combine(Weight);

        public override string ToString() => $"[{GetType().Name}] Src = {Src} Dst = {Dst} Weight = {Weight}";
    }
}