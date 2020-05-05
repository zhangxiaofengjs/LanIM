using Com.LanIM.Common.Network;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common
{
   public class LanServerConfig
    {
        private static readonly string REGKEY_PATH = @"Software\LanIM\Server";
        private static readonly string REGKEY_USING_MAC = "UsingMAC";
        private static readonly string REGKEY_BROADCAST_ADDRESS = "BroadcastAddress";

        private static readonly LanServerConfig _instance = new LanServerConfig();
        public static LanServerConfig Instance
        {
            get
            {
                return _instance;
            }
        }

        public List<IPAddress> BroadcastAddress { get; set; } = new List<IPAddress>();
        public string MAC { get; set; }

        public LanServerConfig()
        {
        }

        public void Load()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(REGKEY_PATH, false);
            if (regKey == null)
            {
                return;
            }

            this.MAC = (string)regKey.GetValue(REGKEY_USING_MAC, "");
            if (string.IsNullOrEmpty(this.MAC))
            {
                List<NCIInfo> nciInfos = NCIInfo.GetNICInfo(NCIType.Physical | NCIType.Wireless);
                if(nciInfos.Count!=0)
                {
                    this.MAC = nciInfos[0].MAC;
                }
            }

            string bdAddrs = (string)regKey.GetValue(REGKEY_BROADCAST_ADDRESS, "");
            this.BroadcastAddress = NCIInfo.ConvertToIPAddIfNotExist(bdAddrs, NCIInfo.GetBroadcastIP(this.MAC));
            regKey.Close();
        }

        public void Save()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(REGKEY_PATH, true);
            if (regKey == null)
            {
                regKey = Registry.CurrentUser.CreateSubKey(REGKEY_PATH, true);
            }

            regKey.SetValue(REGKEY_USING_MAC, this.MAC, RegistryValueKind.String);
            regKey.SetValue(REGKEY_BROADCAST_ADDRESS, NCIInfo.ConvertToString(this.BroadcastAddress), RegistryValueKind.String);

            regKey.Flush();
            regKey.Close();
        }
    }
}
