using Onos.Net.Utils.Misc.OnLab.Graph;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    public class TestDoubleWeight : IWeight
    {
        public static TestDoubleWeight NegativeWeight { get; } =  new TestDoubleWeight(-1);

        public static TestDoubleWeight NonViableWeight { get; } = new TestDoubleWeight(double.PositiveInfinity);

        public double Value { get; }

        public bool IsViable => this != NonViableWeight;

        public bool IsNegative => throw new System.NotImplementedException();

        public TestDoubleWeight(double value) => Value = value;

        public IWeight Merge(IWeight otherWeight)
        {
            throw new System.NotImplementedException();
        }

        public IWeight Subtract(IWeight otherWeight)
        {
            throw new System.NotImplementedException();
        }

        public int CompareTo(IWeight other)
        {
            throw new System.NotImplementedException();
        }
    }
}
