using System;

namespace Onos.Net.Utils.Misc.OnLab.Packet
{
    /// <summary>
    /// Signals that an error occurred during deserialization of a packet.
    /// </summary>
    public class DeserializationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializationException"/> class with the given error message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public DeserializationException(string message) : base(message) { }
    }
}