using Com.LanIM.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI
{
    public class FileIconPool
    {
        private static Dictionary<string, Image> _cache = new Dictionary<string, Image>();
        public static Image GetIcon(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            if (_cache.TryGetValue(extension, out Image image))
            {
                return image;
            }
            if (File.Exists(filePath))
            {
                Icon icon = Icon.ExtractAssociatedIcon(filePath);

                //防止后面不好删除文件，和文件解绑
                Bitmap bmp = icon.ToBitmap();
                _cache.Add(extension, bmp);

                icon.Dispose();
                icon = null;
                return bmp;
            }

            return Properties.Resources.file;
        }
    }
}
