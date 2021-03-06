﻿using Onos.Net.Utils.Misc.OnLab.Graph;
using Onos.Net.Utils.Misc.OnLab.Helpers;
using System;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    /// <summary>
    /// Test weight based on double.
    /// </summary>
    public class TestDoubleWeight : IWeight, IEquatable<TestDoubleWeight>
    {
        /// <summary>
        /// Instance of a negative test weight.
        /// </summary>
        public static TestDoubleWeight NegativeWeight { get; } =  new TestDoubleWeight(-1);

        /// <summary>
        /// Instance of test weight to makr links/paths which cannot be traversed.
        /// </summary>
        public static TestDoubleWeight NonViableWeight { get; } = new TestDoubleWeight(double.PositiveInfinity);

        public double Value { get; }

        public bool IsViable => !Equals(NonViableWeight);

        public bool IsNegative => Value < 0;

        /// <summary>
        /// Initializes a new <see cref="TestDoubleWeight"/> instance with the given value.
        /// </summary>
        /// <param name="value">The weight value.</param>
        public TestDoubleWeight(double value) => Value = value;

        public IWeight Merge(IWeight otherWeight) => new TestDoubleWeight(Value + otherWeight.Value);

        public IWeight Subtract(IWeight otherWeight) => new TestDoubleWeight(Value - otherWeight.Value);

        public int CompareTo(object obj)
        {
            if (obj is TestDoubleWeight other)
            {
                return Value.CompareTo(other.Value);
            }

            throw new InvalidOperationException($"Invalid comparison target type.");
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is TestDoubleWeight t && IsEqual(t);
        }

        public bool Equals(TestDoubleWeight other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        protected virtual bool IsEqual(TestDoubleWeight other) => Value.FuzzyEquals(other.Value, 0.1);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
    }
}
