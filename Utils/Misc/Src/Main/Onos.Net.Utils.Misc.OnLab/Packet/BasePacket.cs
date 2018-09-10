using System;
using System.Linq;
using System.Reflection;

namespace Onos.Net.Utils.Misc.OnLab.Packet
{
    /// <summary>
    /// Base representation of a packet.
    /// </summary>
    public abstract class BasePacket : IPacket, IEquatable<BasePacket>
    {
        /// <inheritdoc />
        public IPacket Payload { get; set; }

        /// <inheritdoc />
        public IPacket Parent { get; set; }

        /// <inheritdoc />
        public void ResetChecksum() => Parent?.ResetChecksum();

        /// <inheritdoc />
        public abstract byte[] Serialize();
        
        /// <inheritdoc />
        public override int GetHashCode()
        {
            const int prime = 6733;
            int result = 1;
            result = prime * result + (Payload is null ? 0 : Payload.GetHashCode());
            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is BasePacket other && IsEqual(other);
        }

        /// <inheritdoc />
        public bool Equals(BasePacket other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        /// <summary>
        /// Determines equality between BasePackets.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool IsEqual(BasePacket other)
        {
            if (Payload is null)
            {
                if (other.Payload != null)
                {
                    return false;
                }
            }

            return Payload.Equals(other.Payload);
        }

        /// <summary>
        /// Clones a packet's contents.
        /// </summary>
        /// <returns>A copy of this packet.</returns>
        [Obsolete("Use Duplicate() method instead.")]
        public object Clone<T>() where T : BasePacket
        {
            Type packetClass = this.GetType();
            PropertyInfo deserializerFactory = packetClass.GetProperty("Deserializer", BindingFlags.Static | BindingFlags.Public);

            if (deserializerFactory is null)
            {
                throw new InvalidOperationException($"No deserializer found for {packetClass.Name}.");
            }

            MethodInfo deserializer = deserializerFactory.GetMethod;
            Deserialize<T> deserialize = deserializer.Invoke(this, null) as Deserialize<T>;

            if (deserialize is null)
            {
                throw new InvalidOperationException($"Unable to cast {GetType()}'s Deserializer to an invokable delegate on type {typeof(T)}.");
            }

            byte[] data = Serialize();
            
            return deserialize(data, 0, data.Length);
        }
    }
}