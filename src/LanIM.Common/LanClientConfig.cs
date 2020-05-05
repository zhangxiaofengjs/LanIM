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
    public class LanClientConfig
    {
        private static readonly string REGKEY_PATH = @"Software\LanIM\Client";
        private static readonly string REGKEY_NICK_NAME = "NickName";
        private static readonly string REGKEY_HIDE_STATUS = "HideStatus";
        private static readonly string REGKEY_USING_MAC = "UsingMAC";
        private static readonly string REGKEY_SELF_VISIBLE = "SelfVisible";
        private static readonly string REGKEY_BROADCAST_ADDRESS = "BroadcastAddress";
        private static readonly string REGKEY_SERVER_ADDRESS = "ServerAddress";
        private static readonly string REGKEY_SERVER_PORT = "ServerPort";
        private static readonly string REGKEY_SERVER_HB_INTERVAL = "HeartBeatInterval";
        
        private static readonly LanClientConfig _instance = new LanClientConfig();
        public static LanClientConfig Instance
        {
            get
            {
                return _instance;
            }
        }

        private readonly string _dbPath;
        public string DbPath
        {
            get
            {
                return _dbPath;
            }
        }

        private string _nickName = "";
        public string NickName
        {
            get
            {
                if (string.IsNullOrEmpty(_nickName))
                {
                    _nickName = Environment.UserName;
                }
                return _nickName;
            }
            set
            {
                _nickName = value;
            }
        }

        /// <summary>
        /// 是否隐藏在线状态
        /// </summary>
        public bool HideStatus { get; set; } = false;
        public List<IPAddress> BroadcastAddress { get; set; } = new List<IPAddress>();
        public string MAC { get; set; }
        public IPAddress ServerAddress { get; set; } = IPAddress.Any;
        public int ServerPort { get; set; } = 2425;
        public int ServerHBInterval { get; set; } = 60000;
        public bool SelfVisible { get; set; } = false;

        public LanClientConfig()
        {
            _dbPath = Path.Combine(LanEnv.AppDataDir, "lanim.db");
        }

        public void Load()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(REGKEY_PATH, false);
            if (regKey == null)
            {
                return;
            }

            this.NickName = (string)regKey.GetValue(REGKEY_NICK_NAME, this.NickName);
            this.HideStatus = (string)regKey.GetValue(REGKEY_HIDE_STATUS, this.HideStatus.ToString()) == true.ToString();
            this.MAC = (string)regKey.GetValue(REGKEY_USING_MAC, "");
            this.SelfVisible = (string)regKey.GetValue(REGKEY_SELF_VISIBLE, "False") == true.ToString();
            this.ServerHBInterval = int.Parse((string)regKey.GetValue(REGKEY_SERVER_HB_INTERVAL, "60000"));
            
            string bdAddrs = (string)regKey.GetValue(REGKEY_BROADCAST_ADDRESS, "");
            this.BroadcastAddress = NCIInfo.ConvertToIPAddIfNotExist(bdAddrs, NCIInfo.GetBroadcastIP(this.MAC));

            string sAddr = (string)regKey.GetValue(REGKEY_SERVER_ADDRESS, "");
            if(!string.IsNullOrEmpty(sAddr))
            {
                this.ServerAddress = IPAddress.Parse(sAddr);
            }
            string sPort = (string)regKey.GetValue(REGKEY_SERVER_PORT, "");
            if (!string.IsNullOrEmpty(sPort))
            {
                this.ServerPort = Int32.Parse(sPort);
            }
            
            regKey.Close();
        }

        public void Save()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(REGKEY_PATH, true);
            if (regKey == null)
            {
                regKey = Registry.CurrentUser.CreateSubKey(REGKEY_PATH, true);
            }

            regKey.SetValue(REGKEY_NICK_NAME, this.NickName, RegistryValueKind.String);
            regKey.SetValue(REGKEY_HIDE_STATUS, this.HideStatus.ToString(), RegistryValueKind.String);
            regKey.SetValue(REGKEY_USING_MAC, this.MAC, RegistryValueKind.String);
            regKey.SetValue(REGKEY_BROADCAST_ADDRESS, NCIInfo.ConvertToString(this.BroadcastAddress), RegistryValueKind.String);
            regKey.SetValue(REGKEY_SERVER_ADDRESS, this.ServerAddress.ToString(), RegistryValueKind.String);
            regKey.SetValue(REGKEY_SERVER_PORT, this.ServerPort.ToString(), RegistryValueKind.String);

            regKey.Flush();
            regKey.Close();
        }
    }
}
