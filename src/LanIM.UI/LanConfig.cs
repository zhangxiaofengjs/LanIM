using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.UI
{
    class LanConfig
    {
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

        private readonly string _receivedFilePath;
        public string ReceivedFilePath
        {
            get
            {
                return MakeSureFolderExist(_receivedFilePath);
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
                dataDir = Path.Combine(dataDir, "lanmi");
                return MakeSureFolderExist(dataDir);
            }
        }

        public string NickName
        {
            get
            {
                return "小兔子 " + DateTime.Now.ToString();
            }
        }

        public LanConfig()
        {
            _tempPath = Path.Combine(AppDataDir, "temp");
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
    }
}
