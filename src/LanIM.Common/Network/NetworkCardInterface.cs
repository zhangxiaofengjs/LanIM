using Microsoft.Win32;
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
    public class NetworkCardInterface
    {
        private static List<NCIInfo> s_nicInfoListCache = new List<NCIInfo>();

        public static List<NCIInfo> GetNCIInfoList()
        {
            if(s_nicInfoListCache.Count != 0)
            {
                //考虑到网卡不是一直变，用缓存提高速度
                return s_nicInfoListCache;
            }

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
                {
                    continue;
                }
                if (!adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    continue;
                }

                IPInterfaceProperties ipIntProps = adapter.GetIPProperties();
                IPAddress ipAddr = null;
                IPAddress ipMaskAddr = null;
                UnicastIPAddressInformationCollection ipAddrInfos = ipIntProps.UnicastAddresses;
                foreach (UnicastIPAddressInformation ipAddrInfo in ipAddrInfos)
                {
                    IPAddress ipAddrTmp = ipAddrInfo.Address;
                    if (ipAddrTmp.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddr = ipAddrTmp;
                        ipMaskAddr = ipAddrInfo.IPv4Mask;
                        break;
                    }
                }

                GatewayIPAddressInformationCollection gwIps = ipIntProps.GatewayAddresses;
                IPAddress gwIp = null;
                if (gwIps.Count != 0)
                {
                    gwIp = gwIps[0].Address;
                }

                
                if(ipAddr != null)
                {
                    NCIInfo addr = new NCIInfo
                    {
                        Address = ipAddr,
                        Mask = ipMaskAddr,
                        GateWay = gwIp,
                        MAC = adapter.GetPhysicalAddress().ToString(),
                        Type = GetNetworkInterfaceType(adapter),
                        Name = adapter.Name,
                    };
                    s_nicInfoListCache.Add(addr);
                }
            }
            return s_nicInfoListCache;
        }

        private static NCIType GetNetworkInterfaceType(NetworkInterface adapter)
        {
            string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + adapter.Id + "\\Connection";
            RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
            if (rk != null)
            {
                // 区分 PnpInstanceID   
                // 如果前面有 PCI 就是本机的真实网卡  
                // MediaSubType 为 01 则是常见网卡，02为无线网卡。  
                string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                if (fPnpInstanceID.Length > 3 &&
                    fPnpInstanceID.Substring(0, 3) == "PCI")
                    return NCIType.Physical;
                else if (fMediaSubType == 1)
                    return NCIType.Virtual;
                else if (fMediaSubType == 2)
                    return NCIType.Wireless;
                else
                    return NCIType.UNKNOW;
            }
            return NCIType.UNKNOW;
        }
    }
}
