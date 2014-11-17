using System;
using System.Net;

namespace RGeoIP
{
    public static class IPAddressExtensions
    {
        public static long ToNumber(this IPAddress ip)
        {
            var num = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(ip.GetAddressBytes(), 0));
            return num;
        }
    }
}
