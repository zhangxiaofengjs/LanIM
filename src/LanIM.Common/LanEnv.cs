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
    public class LanEnv
    {
        private static readonly string _tempPath;
        public static string TempPath
        {
            get
            {
                return MakeSureFolderExist(_tempPath);
            }
        }

        private static readonly string _picturePath;
        public static string PicturePath
        {
            get
            {
                return MakeSureFolderExist(_picturePath);
            }
        }

        private static readonly string _receivedFilePath;
        public static string ReceivedFilePath
        {
            get
            {
                return MakeSureFolderExist(_receivedFilePath);
            }
        }

        private static readonly string _profilePhotoPath;
        public static string ProfilePhotoPath
        {
            get
            {
                return MakeSureFolderExist(_profilePhotoPath);
            }
        }

        private static string MakeSureFolderExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string AppDataDir
        {
            get
            {
                string dataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                dataDir = Path.Combine(dataDir, "lanim");
                return MakeSureFolderExist(dataDir);
            }
        }

        static LanEnv()
        {
            _tempPath = Path.Combine(AppDataDir, "temp");
            _profilePhotoPath = Path.Combine(AppDataDir, "profile-photo");
            _picturePath = Path.Combine(AppDataDir, "picture");
            _receivedFilePath = Path.Combine(AppDataDir, "files");
        }

        public static string GetTempFileName(string extension)
        {
            string path = Path.Combine(TempPath, Path.GetRandomFileName());
            if (!string.IsNullOrEmpty(extension))
            {
                path += extension;
            }
            return path;
        }

        public static string GetPictureFilePath(string fileName)
        {
            return Path.Combine(PicturePath, fileName);
        }

        public static string GetReceivedFilePath(string fileName)
        {
            return Path.Combine(ReceivedFilePath, fileName);
        }

        public static string GetNotExistFileName(string path, string fileName)
        {
            string filePath = Path.Combine(path, fileName);

            int i = 1;
            while (File.Exists(filePath))
            {
                filePath = Path.Combine(path,
                    Path.GetFileNameWithoutExtension(fileName) + "(" + i + ")" + Path.GetExtension(fileName));
                i++;
            }

            return filePath;
        }
    }
}
