using Com.LanIM.Common.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanIM.Common
{
    public class LanFile
    {
        public string Path { set;get;}
        public long Length{set;get;}
        public string Name { set;get;}
        public bool Exist { set;get;}
        public bool IsFolder { set;get;}
        
        public LanFile()
        {
        }

        public LanFile(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            this.Path = fileInfo.FullName;
            this.Name = fileInfo.Name;
            this.Length = fileInfo.Length;
            this.Exist = fileInfo.Exists;
            this.IsFolder = this.Exist && File.Exists(this.Path);
        }

        public override string ToString()
        {
            return string.Format("{{name={0}, size={1}, isfolder={2}}}",
                Name, HumanReadbleLen(Length), IsFolder);
        }

        public static string HumanReadbleLen(long length)
        {
            string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
            double mod = 1024.0;
            int i = 0;
            double len = length;
            while (len >= mod)
            {
                len /= mod;
                i++;
            }
            return Math.Round(len) + units[i];
        }

        public static bool Delete(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return true;
            }
            catch (Exception e)
            {
                LoggerFactory.Error("文件删除失败。{0}", e);
            }
            return false;
        }

        public static bool Rename(string srcPath, string desPath)
        {
            try
            {
                if (!File.Exists(srcPath))
                {
                    return false;
                }
                if (!Delete(desPath))
                {
                    return false;
                }
                File.Move(srcPath, desPath);
                return true;
            }
            catch (Exception e)
            {
                LoggerFactory.Error("文件移动失败。{0}", e);
            }
            return false;
        }

        public static long GetFileLength(string path)
        {
            FileInfo fi = new FileInfo(path);
            return fi.Length;
        }
    }
}
