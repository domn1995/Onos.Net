using System;

namespace Onos.Net.Utils.Misc.OnLab.Helpers
{
    /// <summary>
    /// Provides static methods for checking method arguments.
    /// </summary>
    public static class ArgsChecker
    {
        /// <summary>
        /// Checks that the given reference is not null.
        /// </summary>
        /// <typeparam name="T">The reference type.</typeparam>
        /// <param name="reference">The reference to check.</param>
        /// <param name="errorMessage">The error message to throw with the exception.</param>
        /// <returns>The given reference, if not null.</returns>
        /// <exception cref="ArgumentNullException">The given reference is null.</exception>
        public static T CheckNotNull<T>(T reference, string errorMessage = "") where T : class
        {
            if (reference is null)
            {
                throw new ArgumentNullException(errorMessage);
            }
            return reference;
        }

        /// <summary>
        /// Checks that the given value is not default.
        /// </summary>
        /// <typeparam name="T">The value/reference type.</typeparam>
        /// <param name="valueOrReference">The value/reference.</param>
        /// <param name="errorMessage">The error message to throw with the exception.</param>
        /// <returns>The given value/reference, if not default.</returns>
        /// <exception cref="ArgumentException">The given value/reference is default.</exception>
        public static T CheckNotDefault<T>(T valueOrReference, string errorMessage = "")
        {
            if (valueOrReference.Equals(default))
            {
                throw new ArgumentException(errorMessage);
            }

            return valueOrReference;
        }

        /// <summary>
        /// Checks the given condition and throws <see cref="ArgumentException"/> if not met.
        /// </summary>
        /// /// <param name="successCondition">The success condition to check.</param>
        /// <param name="errorMessage">The message to throw with the exception.</param>
        public static void CheckArgument(bool successCondition, string errorMessage)
        {
            if (!successCondition)
            {
                throw new ArgumentException(errorMessage);
            }            
        }
    }
}