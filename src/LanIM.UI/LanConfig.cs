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
        private static LanConfig _instance = new LanConfig();
        public static LanConfig Instance
        {
            get
            {
                return _instance;
            }
        }

        private string _tempPath;
        public string TempPath
        {
            get
            {
                if (!Directory.Exists(_tempPath))
                {
                    Directory.CreateDirectory(_tempPath);
                }
                return _tempPath;
            }
        }

        public string AppDataDir
        {
            get
            {
                string dataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                dataDir = Path.Combine(dataDir, "lanmi");
                if(!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }
                return dataDir;
            }
        }

        public LanConfig()
        {
            _tempPath = Path.Combine(AppDataDir, "temp");
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
