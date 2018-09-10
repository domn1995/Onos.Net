namespace Onos.Net.Utils.Misc.OnLab.Packet
{
    public enum EtherType : ushort
    {
        Arp = 0x806,
        Rarp = 0x8035,
        Ipv4 = 0x800,
        Ipv6 = 0x86dd,
        Lldp = 0x88cc,
        Vlan = 0x8100,
        Qinq = 0x88a8,
        Bddp = 0x8942,
        MplsUnicast = 0x8847,
        MplsMulticast = 0x8848,
        Eapol = 0x888e,
        Unknown = 0,
    }
}