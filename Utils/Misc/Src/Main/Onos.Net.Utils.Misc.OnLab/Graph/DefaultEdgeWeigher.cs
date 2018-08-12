namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    public class DefaultEdgeWeigher<V, E> : IEdgeWeigher<V, E> where V : IVertex where E : IEdge<V>
    {
        protected const double HopWeightValue = 1;
        protected const double NullWeightValue = 0;

        public IWeight GetInitialWeight()
        {
            throw new System.NotImplementedException();
        }

        public IWeight GetNonViableWeight()
        {
            throw new System.NotImplementedException();
        }

        public IWeight GetWeight(E edge)
        {
            throw new System.NotImplementedException();
        }
    }
}
