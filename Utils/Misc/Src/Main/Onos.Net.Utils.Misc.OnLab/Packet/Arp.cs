using System;
using System.IO;
using Onos.Net.Utils.Misc.OnLab.Helpers;

namespace Onos.Net.Utils.Misc.OnLab.Packet
{
    public class Arp : BasePacket
    {
        public const ushort HwTypeEthernet = 0x1;
        public const ushort ProtoTypeIp = 0x800;
        public const ushort OpRequest = 0x1;
        public const ushort OpReply = 0x2;
        public const ushort OpRarpRequest = 0x3;
        public const ushort OpRarpReply = 0x4;
        public const ushort InitialHeaderLength = 8;

        public ushort HardwareType { get; set; }
        public ushort ProtocolType { get; set; }
        public byte HardwareAddressLength { get; set; }
        public byte ProtocolAddressLength { get; set; }
        public ushort OpCode { get; set; }
        public byte[] SenderHardwareAddress { get; set; }
        public byte[] SenderProtocolAddress { get; set; }
        public byte[] TargetHardwareAddress { get; set; }
        public byte[] TargetProtocolAddress { get; set; }

        public bool IsGratuitous()
        {
            if (SenderProtocolAddress.Length != TargetProtocolAddress.Length)
            {
                throw new InvalidOperationException($"Sender protocol address length is not equal to target protocol address length.");
            }

            int index = 0;
            while (index < SenderProtocolAddress.Length)
            {
                if (SenderProtocolAddress[index] != TargetProtocolAddress[index])
                {
                    return false;
                }
                index++;
            }

            return true;
        }

        public override byte[] Serialize()
        {
            int length = 8 + 2 * (0xff & HardwareAddressLength) + 2 * (0xff & ProtocolAddressLength);

            using (MemoryStream ms = new MemoryStream(length))
            using (BinaryWriter bb = new BinaryWriter(ms))
            {
                bb.Write(HardwareType);
                bb.Write(ProtocolType);
                bb.Write(HardwareAddressLength);
                bb.Write(ProtocolAddressLength);
                bb.Write(OpCode);
                bb.Write(SenderHardwareAddress, 0, 0xff & HardwareAddressLength);
                bb.Write(SenderProtocolAddress, 0, 0xff & ProtocolAddressLength);
                bb.Write(TargetHardwareAddress, 0, 0xff & HardwareAddressLength);
                bb.Write(TargetProtocolAddress, 0, 0xff & ProtocolAddressLength);
                return ms.ToArray();
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int prime = 13121;
                int result = base.GetHashCode();
                result = prime * result + HardwareAddressLength;
                result = prime * result + HardwareType;
                result = prime * result + OpCode;
                result = prime * result + ProtocolAddressLength;
                result = prime * result + ProtocolType;
                result = prime * result + SenderHardwareAddress.GetArrayHashCode();
                result = prime * result + SenderProtocolAddress.GetArrayHashCode();
                result = prime * result + TargetHardwareAddress.GetArrayHashCode();
                result = prime * result + TargetProtocolAddress.GetArrayHashCode();
                return result;
            }
        }
    }
}