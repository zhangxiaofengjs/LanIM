using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common
{
   public class LanConfig
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

        public string NickName
        {
            get
            {
                return "大狮子";
            }
        }

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
    }
}
