namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    public class DefaultEdgeWeigher<V, E> : IEdgeWeigher<V, E> where V : IVertex where E : IEdge<V>
    {
        /// <summary>
        /// Common weight value for any link.
        /// </summary>
        protected const double HopWeightValue = 1;

        /// <summary>
        /// Weight value for a null path (no links).
        /// </summary>
        protected const double NullWeightValue = 0;

        /// <summary>
        /// Gets the default weight based on hop count.
        /// </summary>
        public static ScalarWeight DefaultHopWeight { get; } = new ScalarWeight(HopWeightValue);

        /// <summary>
        /// Gets the default initial weight.
        /// </summary>
        public static ScalarWeight DefaultInitialWeight { get; } = new ScalarWeight(NullWeightValue);

        /// <inheritdoc/>
        public IWeight InitialWeight => DefaultInitialWeight;

        /// <inheritdoc/>
        public IWeight NonViableWeight => ScalarWeight.NonViableWeight;

        /// <inheritdoc/>
        public IWeight GetWeight(E edge) => DefaultHopWeight;
    }
}
