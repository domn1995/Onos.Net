using System;
using System.Collections.Concurrent;

namespace Onos.Net.Utils.Misc.OnLab.Helpers
{
    /// <summary>
    /// Provides memoization functionality.
    /// </summary>
    public static class Memoizer
    {
        /// <summary>
        /// Memoizes the given function.
        /// </summary>
        /// <typeparam name="R">The return type.</typeparam>
        /// <param name="f">The function to memoize.</param>
        /// <returns>The given function, memoized.</returns>
        public static Func<R> Memoize<R>(Func<R> f)
        {
            R value = default;
            bool hasValue = false;
            return () =>
            {
                if (!hasValue)
                {
                    hasValue = true;
                    value = f();
                }
                return value;
            };
        }

        /// <summary>
        /// Memoizes the given function.
        /// </summary>
        /// <typeparam name="A">The argument type.</typeparam>
        /// <typeparam name="R">The return type.</typeparam>
        /// <param name="func">The function to memoize.</param>
        /// <returns>The given function, memoized.</returns>
        public static Func<A, R> Memoize<A, R>(Func<A, R> func)
        {
            var cache = new ConcurrentDictionary<A, R>();

            return arg => cache.GetOrAdd(arg, func);
        }
    }
}
