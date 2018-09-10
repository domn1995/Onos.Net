using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Onos.Net.Utils.Misc.OnLab.Packet;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Packet
{
    /// <summary>
    /// Utilities for testing packet methods.
    /// </summary>
    public sealed class PacketUtilsTest
    {
        /// <summary>
        /// Tests that the deserializer delegate is resilient to bad input parameters
        /// such as null input, negative offset, negative length, etc.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="deserialize">The deserializer delegate to test.</param>
        public static void DeserializeBadInput<T>(Deserialize<T> deserialize)
        {
            byte[] bytes = new byte[4];
            try
            {
                deserialize(null, 0, 4);
                Assert.True(false, "NullReferenceException was not thrown.");
            }
            catch (NullReferenceException)
            {
                Assert.True(true);
            }
            catch (DeserializationException)
            {
                Assert.True(false, $"NullReferenceException was not thrown.");
            }

            ExpectDeserializationException(deserialize, bytes, -1, 0);
            ExpectDeserializationException(deserialize, bytes, 0, -1);
            ExpectDeserializationException(deserialize, bytes, 0, 5);
            ExpectDeserializationException(deserialize, bytes, 2, 3);
            ExpectDeserializationException(deserialize, bytes, 5, 0);
        }

        /// <summary>
        /// Tests that the deserializer delegate is resilient to truncated input or cases where
        /// the input byte array does not contain enough bytes to deserialize the packet.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="deserialize">The deserializer delegate to test.</param>
        /// <param name="header">A byte array of a full-size packet.</param>
        public static void DeserializeTruncated<T>(Deserialize<T> deserialize, byte[] header)
        {
            for (int i = 0; i < header.Length; ++i)
            {
                byte[] truncated = new Span<byte>(header, 0, i).ToArray();
                
                ExpectDeserializationException(deserialize, truncated, 0, truncated.Length);
            }
        }

        /// <summary>
        /// Run the given desearilizer delegate against the given inputs and verify
        /// that a <see cref="DeserializationException"/> is thrown. The test will
        /// fail if it is not thrown by the deserializer delegate.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="deserialize">The deserializer delegate.</param>
        /// <param name="bytes">The input byte array.</param>
        /// <param name="offset">The input offset.</param>
        /// <param name="length">The input length.</param>
        public static void ExpectDeserializationException<T>(Deserialize<T> deserialize, byte[] bytes, int offset, int length)
        {
            try
            {
                deserialize(bytes, offset, length);
                Assert.True(false, "DeserializationException was not thrown.");
            }
            catch (DeserializationException)
            {
                Assert.True(true);
            }
        }
    }
}
