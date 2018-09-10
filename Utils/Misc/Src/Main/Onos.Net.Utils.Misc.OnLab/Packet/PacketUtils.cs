using static Onos.Net.Utils.Misc.OnLab.Helpers.ArgsChecker;

namespace Onos.Net.Utils.Misc.OnLab.Packet
{
    /// <summary>
    /// Utilities for working with packet headers.
    /// </summary>
    public static class PacketUtils
    {
        /// <summary>
        /// Checks the length of the input buffer is appropriate given the offset and length parameters.
        /// </summary>
        /// <param name="byteLength">Length of the input buffer array.</param>
        /// <param name="offset">The offset given to begin reading bytes from.</param>
        /// <param name="length">The length given to read up until.</param>
        /// <exception cref="DeserializationException">The input paremeters don't match up
        /// (i.e. we can't read that many bytes from the buffer at the given offset)</exception>
        public static void CheckBufferLength(int byteLength, int offset, int length)
        {
            bool ok = (offset >= 0 && offset < byteLength);
            ok = ok && (length >= 0 && offset + length <= byteLength);

            if (!ok)
            {
                throw new DeserializationException($"Unable to read {length} bytes from a " +
                    $"{byteLength} byte array starting at offset {offset}.");
            }
        }

        /// <summary>
        /// Checks that there are enough bytes in the buffer to read some number of bytes that we need to read a full header.
        /// </summary>
        /// <param name="givenLength">The size of the buffer.</param>
        /// <param name="requiredLength">The number of bytes we need to read some header fully.</param>
        /// <exception cref="DeserializationException">There aren't enough bytes.</exception>
        public static void CheckHeaderLength(int givenLength, int requiredLength)
        {
            if (requiredLength > givenLength)
            {
                throw new DeserializationException($"{requiredLength} bytes are needed to" +
                    $"continue deserialization; however, only {givenLength} bytes remain in buffer.");
            }
        }

        /// <summary>
        /// Checks the input parameters are sane and there's enough bytes to read the required length.
        /// </summary>
        /// <param name="data">The input byte buffer.</param>
        /// <param name="offset">The offset of the start of the header.</param>
        /// <param name="length">The length given to deserialize the header.</param>
        /// <param name="requiredLength">The length needed to deserialize the header.</param>
        /// <exception cref="DeserializationException">Unable to deserialize the packet based on the input parameters.</exception>
        public static void CheckInput(byte[] data, int offset, int length, int requiredLength)
        {
            CheckNotNull(data);
            CheckBufferLength(data.Length, offset, length);
            CheckHeaderLength(length, requiredLength);
        }
    }
}
