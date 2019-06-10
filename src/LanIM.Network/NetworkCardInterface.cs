using Microsoft.Win32;
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
    public class NetworkCardInterface
    {
        public static List<IPv4Address> GetIPv4Address()
        {
            List<IPv4Address> addresss = new List<IPv4Address>();

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
                    IPv4Address addr = new IPv4Address();
                    addr.Address = ipAddr;
                    addr.Mask = ipMaskAddr;
                    addr.GateWay = gwIp;
                    addr.MAC = adapter.GetPhysicalAddress().ToString();
                    addr.NetworkCardInterfaceType = GetNetworkInterfaceType(adapter);
                    addresss.Add(addr);
                }
            }
            return addresss;
        }

        public static NetworkCardInterfaceType GetNetworkInterfaceType(NetworkInterface adapter)
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
                    return NetworkCardInterfaceType.Physical;
                else if (fMediaSubType == 1)
                    return NetworkCardInterfaceType.Virtual;
                else if (fMediaSubType == 2)
                    return NetworkCardInterfaceType.Wireless;
                else
                    return NetworkCardInterfaceType.UNKNOW;
            }
            return NetworkCardInterfaceType.UNKNOW;
        }
    }
}
