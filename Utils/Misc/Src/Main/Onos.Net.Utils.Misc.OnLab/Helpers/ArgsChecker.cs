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
        /// <returns>The reference, if not null.</returns>
        public static T CheckNotNull<T>(T reference, string errorMessage = "") where T : class
        {
            if (reference is null)
            {
                throw new ArgumentNullException(errorMessage);
            }
            return reference;
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