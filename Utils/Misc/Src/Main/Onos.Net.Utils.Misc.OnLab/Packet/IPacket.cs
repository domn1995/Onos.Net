namespace Onos.Net.Utils.Misc.OnLab.Packet
{
    /// <summary>
    /// Packet interface.
    /// </summary>
    public interface IPacket
    {
        /// <summary>
        /// Gets or sets the packet payload.
        /// </summary>
        IPacket Payload { get; set; }

        /// <summary>
        /// Gets or sets the parent packet.
        /// </summary>
        IPacket Parent { get; set; }
        
        /// <summary>
        /// Reset any checksum as needed and calls ResetChecksum on all parents.
        /// </summary>
        void ResetChecksum();

        /// <summary>
        /// Sets all payloads' parent packet if applicable, then serializes this packet and all payloads.
        /// </summary>
        /// <returns>A byte array containing this packet and payloads.</returns>
        byte[] Serialize();
    }
}