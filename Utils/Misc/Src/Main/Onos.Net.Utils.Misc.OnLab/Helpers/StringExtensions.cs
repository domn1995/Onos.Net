namespace Onos.Net.Utils.Misc.OnLab.Helpers
{
    /// <summary>
    /// Provides extension methods for string input/output.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Gets the hexadecimal string representation of the given value.
        /// </summary>
        /// <param name="value">The value to convert to a hex string.</param>
        /// <returns>The hex representation of the given value.</returns>
        public static string ToHexString(this long value)
        {
            return string.Format("{0:X}", value);
        }
    }
}
