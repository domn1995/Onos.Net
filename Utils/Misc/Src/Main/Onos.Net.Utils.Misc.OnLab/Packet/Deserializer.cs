namespace Onos.Net.Utils.Misc.OnLab.Packet
{
    public delegate T Deserialize<out T>(byte[] data, int offset, int length);
}