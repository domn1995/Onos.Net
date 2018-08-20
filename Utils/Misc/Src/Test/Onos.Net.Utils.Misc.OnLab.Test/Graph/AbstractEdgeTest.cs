using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    public class AbstractEdgeTest
    {
        [Fact]
        public void Equality()
        {
            TestVertex v1 = new TestVertex("1");
            TestVertex v2 = new TestVertex("2");
            Assert.Equal(new TestEdge(v1, v2), new TestEdge(v1, v2));
            Assert.NotEqual(new TestEdge(v1, v2), new TestEdge(v2, v1));
        }
    }
}