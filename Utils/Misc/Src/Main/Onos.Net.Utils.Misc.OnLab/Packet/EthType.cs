using System;
using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Packet
{
    /// <summary>
    /// Representation of Ethertype.
    /// </summary>
    public class EthType : IEquatable<EthType>
    {
        private readonly Dictionary<EtherType, Deserialize<BasePacket>> etherTypeToDeserializer = 
            new Dictionary<EtherType, Deserialize<BasePacket>>()
        {
            
        };

        private readonly Dictionary<EtherType, string> etherTypeToString = 
            new Dictionary<EtherType, string>()
        {

        };

        /// <summary>
        /// Gets the ethertype.
        /// </summary>
        private readonly ushort etherType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EthType"/> class with the given ethertype.
        /// </summary>
        /// <param name="etherType">The ethertype to initialize with.</param>
        public EthType(int etherType) => this.etherType = (ushort)(etherType & 0xffff);

        /// <summary>
        /// Initializes a new instance of the <see cref="EthType"/> class with the given ethertype.
        /// </summary>
        /// <param name="etherType">The ethertype to initialize with.</param>
        public EthType(ushort etherType) => this.etherType = etherType;

        /// <summary>
        /// Looks up the ethertype by its numerical representation and return its enum.
        /// </summary>
        /// <param name="etherType">The value to look up.</param>
        /// <returns>The ethertype enum.</returns>
        public static EtherType LookUp(ushort etherType)
        {
            EtherType value = Packet.EtherType.Unknown;
            if (Enum.IsDefined(typeof(EtherType), etherType))
            {
                value = (EtherType)etherType;
            }
            return value;
        }

        /// <inheritdoc />
        public bool Equals(EthType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is EthType other && IsEqual(other);
        }

        /// <summary>
        /// Determines whether two <see cref="EthType"/>s are equal.
        /// </summary>
        /// <param name="other">The other <see cref="EthType"/> to compare against.</param>
        /// <returns></returns>
        protected virtual bool IsEqual(EthType other)
        {
            return etherType == other.etherType;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return etherType.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            EtherType ethType = LookUp(etherType);
            return string.Format($"0x{0:X4}", ethType);
        }
    }
}