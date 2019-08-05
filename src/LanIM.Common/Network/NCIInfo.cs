using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common.Network
{
    #pragma warning disable 0618

    public class NCIInfo
    {
        public IPAddress Address { get; set; }
        public IPAddress GateWay { get; set; }
        public IPAddress Mask { get; set; }
        public string MAC { get; set; }
        public string Name { get; set; }
        public NCIType Type { get; set; }

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
            return string.Format("ip={0}, gateway={1}, mask={2}, MAC={3}, name={4}, type={5}",
                Address,
                GateWay,
                Mask,
                MAC,
                Name,
                Type);
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

        public static List<NCIInfo> GetNICInfo(NCIType type)
        {
            List<NCIInfo> ret = new List<NCIInfo>();
            List<NCIInfo> ips = NetworkCardInterface.GetNCIInfoList();
            foreach (NCIInfo ip in ips)
            {
                if ((type & ip.Type) != 0)
                {
                    ret.Add(ip);
                }
            }
            return ret;
        }

        public static NCIInfo GetNICInfo(string MAC)
        {
            List<NCIInfo> ips = NetworkCardInterface.GetNCIInfoList();
            foreach (NCIInfo ip in ips)
            {
                if (ip.MAC == MAC)
                {
                    return ip;
                }
            }

            return null;
        }

        public static IPAddress GetIPAddress(string MAC)
        {
            List<NCIInfo> ips = NetworkCardInterface.GetNCIInfoList();
            foreach (NCIInfo ip in ips)
            {
                if (ip.MAC == MAC)
                {
                    return ip.Address;
                }
            }

            return null;
        }

        public static List<IPAddress> ConvertToIPAddIfNotExist(string text, IPAddress ip)
        {
            List<IPAddress> list = ConvertToIP(text);

            if (!list.Exists(new Predicate<IPAddress>((addr) =>
            {
                if (addr.Equals(ip))
                {
                    return true;
                }
                return false;
            })))
            {
                //默认加上当前
                list.Add(ip);
            }

            return list;
        }

        public static List<IPAddress> ConvertToIP(string text)
        {
            string[] strs = text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<IPAddress> ips = new List<IPAddress>();

            for (int i = 0; i < strs.Length; i++)
            {
                IPAddress ip = NCIInfo.Parse(strs[i]);
                if (ip != null)
                {
                    ips.Add(ip);
                }
            }
            return ips;
        }

        public static IPAddress GetBroadcastIP(string defaultMac)
        {
            NCIInfo ipv4 = NCIInfo.GetNICInfo(defaultMac);
            if (ipv4 == null)
            {
                return null;
            }
            return ipv4.BroadcastAddress;
        }

        public static string ConvertToString(List<IPAddress> broadcastAddress)
        {
            string str = "";
            foreach (IPAddress item in broadcastAddress)
            {
                str += item.ToString() + ";";
            }
            return str;
        }
    }
}
