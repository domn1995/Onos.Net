using System;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Weight implementation based on a double value.
    /// </summary>
    public class ScalarWeight : IWeight
    {
        /// <summary>
        /// Gets or sets the sameness threshold for comparsing cost values.
        /// Default value is <see cref="double.MinValue"/>.
        /// </summary>
        public static double SamenessTreshold { get; set; } = double.MinValue;

        /// <summary>
        /// Gets an instance of <see cref="ScalarWeight"/> to make links/paths which cannot be traversed.
        /// </summary>
        public static ScalarWeight NonViableWeight { get; } = new ScalarWeight(double.PositiveInfinity);

        /// <inheritdoc/>
        public bool IsViable => !this.Equals(NonViableWeight);

        /// <inheritdoc/>
        public bool IsNegative => Value < 0;

        /// <inheritdoc/>
        public double Value { get; }

        /// <summary>
        /// Initializes a new <see cref="ScalarWeight"/> object with the given value.
        /// </summary>
        /// <param name="value">The weight value.</param>
        public ScalarWeight(double value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new <see cref="ScalarWeight"/> with the given value.
        /// </summary>
        /// <param name="value">The weight value.</param>
        /// <returns>A new <see cref="ScalarWeight"/> instance.</returns>
        public static ScalarWeight ToWeight(double value) => new ScalarWeight(value);

        /// <inheritdoc/>
        public int CompareTo(IWeight otherWeight)
        {
            if (Equals(otherWeight))
            {
                return 0;
            }
            return Value.CompareTo(((ScalarWeight)otherWeight).Value);
        }

        /// <inheritdoc/>
        public IWeight Merge(IWeight otherWeight) => new ScalarWeight(Value + otherWeight.Value);

        /// <inheritdoc/>
        public IWeight Subtract(IWeight otherWeight) => new ScalarWeight(Value - otherWeight.Value);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is ScalarWeight sw)
            {
                return Math.Abs(sw.Value - Value) < SamenessTreshold;
            }
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString() => $"[{GetType().Name}] Value = {Value}";
    }
}
