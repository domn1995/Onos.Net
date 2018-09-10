using System;
using Onos.Net.Utils.Misc.OnLab.Helpers;
using Onos.Net.Utils.Misc.OnLab.Util;

namespace Onos.Net.Utils.Misc.OnLab.Packet
{
    /// <summary>
    /// Represents a network device's chassis identifier. This class is immutable.
    /// </summary>
    public sealed class ChassisId : Identifier<long>, IEquatable<ChassisId>
    {
        private const long unknown = 0;

        /// <summary>
        /// Gets the value of the chassis id.
        /// </summary>
        public long Value => Id;

        /// <summary>
        /// Initializes a new <see cref="ChassisId"/> object with unknown value.
        /// </summary>
        public ChassisId() : base(unknown) { }

        /// <summary>
        /// Initializes a new <see cref="ChassisId"/> object with the given value.
        /// </summary>
        /// <param name="value">The chassis id value.</param>
        public ChassisId(long value) : base(value) { }

        /// <summary>
        /// Initializes a new <see cref="ChassisId"/> object with the given value.
        /// </summary>
        /// <param name="value">The chassis id value.</param>
        public ChassisId(string value) : base(Convert.ToInt64(value, 16)) { }

        /// <summary>
        /// Converts the chassis id value to a ':' separated hex string.
        /// </summary>
        /// <returns>The chassis id value as a ':' separated hex string.</returns>
        public override string ToString() => Id.ToHexString();

        /// <inheritdoc />
        public bool Equals(ChassisId other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Identifier<long> other && IsEqual(other);
        }

        /// <inheritdoc />
        public override int GetHashCode() => Id.GetHashCode();


    }
}
