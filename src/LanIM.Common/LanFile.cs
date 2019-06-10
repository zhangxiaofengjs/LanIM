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
        public string  Path { get; set; }
        public long Length { get; set; }
        public string HumanReadbleLength {
            get
            {
                string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
                double mod = 1024.0;
                int i = 0;
                double len = Length;
                while (len >= mod)
                {
                    len /= mod;
                    i++;
                }
                return Math.Round(len) + units[i];
            }
        }
        public String Name { get; set; }
        public bool Exist { get; set; }
        public bool IsFolder { get; set; }

        public LanFile(string path)
        {
            this.Path = path;

            FileInfo fileInfo = new FileInfo(this.Path);
            this.Length = fileInfo.Length;
            this.Name = fileInfo.Name;
            this.Exist = fileInfo.Exists;

            if (this.Exist)
            {
                this.IsFolder = File.Exists(this.Path);
            }
        }
    }
}
