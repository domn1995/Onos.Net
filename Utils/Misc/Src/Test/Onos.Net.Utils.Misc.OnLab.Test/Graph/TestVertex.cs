using Onos.Net.Utils.Misc.OnLab.Graph;
using System;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    public class TestVertex : IVertex, IEquatable<TestVertex>
    {
        private readonly string name;

        public TestVertex(string name) => this.name = name;

        public bool Equals(TestVertex other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is TestVertex v && IsEqual(v);
        }

        private bool IsEqual(TestVertex other) => name == other.name;

        public override int GetHashCode() => name.GetHashCode();

        public override string ToString() => name;
    }
}