using Onos.Net.Utils.Misc.OnLab.Graph;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    public class TestVertex : IVertex
    {
        private readonly string name;

        public TestVertex(string name) => this.name = name;

        public override bool Equals(object obj) => obj is TestVertex vertex && name == vertex.name;

        public override int GetHashCode() => name.GetHashCode();

        public override string ToString() => name;
    }
}