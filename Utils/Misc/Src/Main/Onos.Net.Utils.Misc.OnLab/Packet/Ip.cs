using System;

namespace Onos.Net.Utils.Misc.OnLab.Packet
{
    public abstract class Ip : BasePacket
    {
        public abstract byte Version { get; set; }

        public static Deserialize<Ip> Deserialize
        {
            get
            {
                return (data, offset, length) =>
                {
                    byte[] bb = new byte[length];
                    Array.Copy(data, offset, bb, 0, length);
                    byte version = (byte)(bb[0] >> 4 & 0xf);

                    switch (version)
                    {
                        case 4:
                            throw new NotImplementedException();
                        case 6:
                            throw new NotImplementedException();
                        default:
                            throw new DeserializationException("Invalid IP version.");
                    }
                };
            }
        }
    }
}