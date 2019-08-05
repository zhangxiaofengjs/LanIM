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
   public class LanConfig
    {
        
        private static readonly string REGKEY_PATH = @"Software\LanIM";
        private static readonly string REGKEY_NICK_NAME = "NickName";
        private static readonly string REGKEY_HIDE_STATUS = "HideStatus";
        private static readonly string REGKEY_USING_MAC = "UsingMAC";
        private static readonly string REGKEY_BROADCAST_ADDRESS = "BroadcastAddress";
        
        private static readonly LanConfig _instance = new LanConfig();
        public static LanConfig Instance
        {
            get
            {
                return _instance;
            }
        }

        private readonly string _tempPath;
        public string TempPath
        {
            get
            {
                return MakeSureFolderExist(_tempPath);
            }
        }

        private readonly string _picturePath;
        public string PicturePath
        {
            get
            {
                return MakeSureFolderExist(_picturePath);
            }
        }

        private readonly string _receivedFilePath;
        public string ReceivedFilePath
        {
            get
            {
                return MakeSureFolderExist(_receivedFilePath);
            }
        }

        private readonly string _profilePhotoPath;
        public string ProfilePhotoPath
        {
            get
            {
                return MakeSureFolderExist(_profilePhotoPath);
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

        private string MakeSureFolderExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public string AppDataDir
        {
            get
            {
                string dataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                dataDir = Path.Combine(dataDir, "lanim");
                return MakeSureFolderExist(dataDir);
            }
        }

        private string _nickName = "";
        public string NickName
        {
            get
            {
                if(string.IsNullOrEmpty(_nickName))
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
        private List<IPAddress> _broadcastAddress = new List<IPAddress>();
        public List<IPAddress> BroadcastAddress { get; set; } = new List<IPAddress>();
        public string MAC { get; set; }

        public LanConfig()
        {
            _tempPath = Path.Combine(AppDataDir, "temp");
            _profilePhotoPath = Path.Combine(AppDataDir, "profile-photo");
            _dbPath = Path.Combine(AppDataDir, "lanim.db");
            _picturePath = Path.Combine(AppDataDir, "picture");
            _receivedFilePath = Path.Combine(AppDataDir, "files");
        }

        public string GetTempFileName(string extension)
        {
            string path = Path.Combine(this.TempPath, Path.GetRandomFileName());
            if(!string.IsNullOrEmpty(extension))
            {
                path += extension;
            }
            return path;
        }

        public string GetPictureFilePath(string fileName)
        {
            return Path.Combine(this.PicturePath, fileName);
        }

        public string GetReceivedFilePath(string fileName)
        {
            string path = Path.Combine(this.ReceivedFilePath, fileName);

            int i = 1;
            while(File.Exists(path))
            {
                path = Path.Combine(this.ReceivedFilePath,
                    Path.GetFileNameWithoutExtension(fileName) + "(" + i + ")" + Path.GetExtension(fileName));
                i++;
            }

            return path;
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

            regKey.SetValue(REGKEY_NICK_NAME, this.NickName, RegistryValueKind.String);
            regKey.SetValue(REGKEY_HIDE_STATUS, this.HideStatus.ToString(), RegistryValueKind.String);
            regKey.SetValue(REGKEY_USING_MAC, this.MAC, RegistryValueKind.String);
            regKey.SetValue(REGKEY_BROADCAST_ADDRESS, NCIInfo.ConvertToString(this.BroadcastAddress), RegistryValueKind.String);

            regKey.Flush();
            regKey.Close();
        }
    }
}
