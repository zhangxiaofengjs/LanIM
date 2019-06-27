using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network
{
    #pragma warning disable 0618

    public class IPv4Address
    {
        public IPAddress Address { get; set; }
        public IPAddress GateWay { get; set; }
        public IPAddress Mask { get; set; }
        public string MAC { get; set; }
        public NetworkCardInterfaceType NetworkCardInterfaceType { get; set; }

        public IPAddress BroadcastAddress
        {
            get
            {
                uint ibroadcastAddr = ((uint)Address.Address & (uint)Mask.Address) | ~(uint)Mask.Address;
                IPAddress broadcastAddr = new IPAddress(ibroadcastAddr);
                return broadcastAddr;
            }
        }

        public override string ToString()
        {
            return string.Format("ip={0}, gateway={1}, mask={2}, MAC={3}",
                Address,
                GateWay,
                Mask,
                MAC);
        }

        public static IPAddress Parse(string iP)
        {
            if (!string.IsNullOrEmpty(iP) &&
            IPAddress.TryParse(iP, out IPAddress ip))
            {
                return ip;
            }
            return null;
        }

        public static IPv4Address GetLocalMachineIPV4()
        {
            List<IPv4Address> ips = NetworkCardInterface.GetIPv4Address();
            foreach (IPv4Address ip in ips)
            {
                if (ip.NetworkCardInterfaceType == NetworkCardInterfaceType.Physical ||
                    ip.NetworkCardInterfaceType == NetworkCardInterfaceType.Wireless)
                {
                    return ip;
                }
            }
            return null;
        }
    }
}
