using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Onos.Net.Utils.Misc.OnLab.Packet
{
    public class MacAddress
    {
        private byte[] address = new byte[MacAddressLength];

        private static Regex MacRegex { get; } = new Regex(@"^([0-9A-Fa-f]{2}[:]){5}([0-9A-Fa-f]{2})$", RegexOptions.Compiled);

        public const int MacAddressLength = 6;

        


        public MacAddress(byte[] address)
        {
            this.address = address.ToArray();
        }

        public static MacAddress Parse(in byte[] address)
        {
            if (address.Length != MacAddressLength)
            {
                throw new ArgumentException($"The length is not {MacAddressLength}.");
            }

            return new MacAddress(address);
        }

        public static MacAddress Parse(in long address)
        {
            byte[] addressBytes = new byte[]
            {
                (byte)(address >> 40 & 0xff), (byte)(address >> 32 & 0xff),
                (byte)(address >> 24 & 0xff), (byte)(address >> 16 & 0xff),
                (byte)(address >> 8 & 0xff), (byte)(address >> 0 & 0xff)
            };

            return new MacAddress(addressBytes);
        }

        public static MacAddress Parse(in string address)
        {
            if (!IsValid(address))
            {
                throw new ArgumentException($"Specified MAC address must contain 12 hex digits, separated pairwise by ':' (colons).");
            }

            string[] elements = address.Split(':');
            byte[] addressBytes = new byte[MacAddressLength];
            for (int i = 0; i < MacAddressLength; ++i)
            {
                string element = elements[i];
                addressBytes[i] = Convert.ToByte(element, 16);
            }

            return new MacAddress(addressBytes);
        }

        public int Length => address.Length;

        public byte[] ToBytes() => address.ToArray();

        public long ToLong()
        {
            long mac = 0;
            for (int i = 0; i < MacAddressLength; ++i)
            {
                long t = (address[i] & 0xffL) << (5 - i) * 8;
                mac |= t;
            }

            return mac;
        }

        public bool IsBroadcast => address.All(b => b is 0xff);

        public bool IsMulticast
        {
            get
            {
                if (IsBroadcast)
                {
                    return false;
                }

                return (address[0] & 0x01) != 0;
            }
        }

        private static bool IsValid(in string mac) => MacRegex.IsMatch(mac);
    }
}
