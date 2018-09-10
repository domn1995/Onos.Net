using System;
using System.Linq;
using static Onos.Net.Utils.Misc.OnLab.Packet.PacketUtils;

namespace Onos.Net.Utils.Misc.OnLab.Packet
{
    /// <summary>
    /// Represents a packet's data.
    /// </summary>
    public class Data : BasePacket, IEquatable<Data>
    {
        /// <summary>
        /// Gets the data bytes.
        /// </summary>
        public byte[] DataBytes { get; set; }

        /// <summary>
        /// Initialize a new instance of the <see cref="Data"/> class with empty data.
        /// </summary>
        public Data() => DataBytes = new byte[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="Data"/> class with the given data.
        /// </summary>
        /// <param name="data">The data to initialize with.</param>
        public Data(byte[] data) => DataBytes = data;

        /// <inheritdoc />
        public override byte[] Serialize() => DataBytes;

        /// <summary>
        /// Deserializer delegate for generic payload data.
        /// </summary>
        public static Deserialize<Data> Deserializer
        {
            get
            {
                return (data, offset, length) =>
                {
                    // Allow zero-length data for now.
                    if (length is 0)
                    {
                        return new Data();
                    }

                    CheckInput(data, offset, length, 1);

                    Data dataObject = new Data();

                    Array.Copy(data, offset, dataObject.DataBytes, 0, data.Length);

                    return dataObject;
                };
            }
        }

        /// <inheritdoc />
        public bool Equals(Data other)
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
            return obj is Data other && IsEqual(other);
        }

        /// <summary>
        /// Determines whether two data objects are equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool IsEqual(Data other) => DataBytes.SequenceEqual(other.DataBytes) && base.IsEqual(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            const int prime = 1571;
            int result = base.GetHashCode();
            unchecked
            {
                result = prime * result * DataBytes.GetHashCode();
            }
            return result;
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{GetType().Name}] DataBytes = {DataBytes}";
        }
    }
}