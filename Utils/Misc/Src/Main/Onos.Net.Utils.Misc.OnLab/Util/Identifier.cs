using System;
using static Onos.Net.Utils.Misc.OnLab.Helpers.ArgsChecker;

namespace Onos.Net.Utils.Misc.OnLab.Util
{
    /// <summary>
    /// Abstract identifier backed by another value, e.g. string, int, etc.
    /// </summary>
    /// <typeparam name="T">The identifier type.</typeparam>
    public class Identifier<T> : IEquatable<Identifier<T>>
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        protected T Id { get; }

        /// <summary>
        /// Initializes a new <see cref="Identifier{T}"/> class with a default ID.
        /// </summary>
        protected Identifier() => Id = default;

        /// <summary>
        /// Initializes a new <see cref="Identifier{T}"/> class with the given value.
        /// </summary>
        /// <param name="value">The value to use as an identifier.</param>
        protected Identifier(T value)
        {
            Id = CheckNotDefault(value, "Identifier cannot be null.");
        }

        /// <inheritdoc />
        public override int GetHashCode() => Id.GetHashCode();

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Identifier<T> other && IsEqual(other);
        }

        /// <inheritdoc />
        public bool Equals(Identifier<T> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        /// <summary>
        /// Determines equality between identifiers.
        /// </summary>
        /// <param name="other">The other identifier to compare against.</param>
        /// <returns>True if both identifiers are equal.</returns>
        protected bool IsEqual(Identifier<T> other) => Id.Equals(other.Id);

        /// <inheritdoc />
        public override string ToString() => Id.ToString();
    }
}