using Com.LanIM.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI
{
    public class ProfilePhotoPool
    {
        private static Dictionary<string, Image> _cache = new Dictionary<string, Image>();

        public static Image GetPhoto(string key, bool returnDefaultOnNull = true)
        {
            if(string.IsNullOrEmpty(key))
            {
                return returnDefaultOnNull ? Properties.Resources.default_photo : null;
            }

            if (_cache.TryGetValue(key, out Image img))
            {
                return img;
            }

            string fileName = Path.Combine(LanConfig.Instance.ProfilePhotoPath, key);
            if (File.Exists(fileName))
            {
                img = Image.FromFile(fileName);

                //防止后面不好删除文件，和文件解绑
                Bitmap bmp = new Bitmap(img);
                _cache.Add(key, bmp);

                img.Dispose();
                img = null;
                return bmp;
            }

            if (returnDefaultOnNull)
            {
                return Properties.Resources.default_photo;
            }
            return null;
        }

        public static void SetPhoto(string key, Image img)
        {
            string fileName = Path.Combine(LanConfig.Instance.ProfilePhotoPath, key);
            _cache.Remove(key);
            LanFile.Delete(fileName);

            if (img == null)
            {
                return;
            }

            using (Bitmap bmp = new Bitmap(img.Width, img.Height))
            {
                bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImageUnscaled(img, 0, 0);
                }
                bmp.Save(fileName, ImageFormat.Jpeg);
            }
        }

        public static Image ScalePhoto(string fileName)
        {
            //scale to 120*120
            Bitmap bmp = new Bitmap(120, 120);
            using (Image img = Image.FromFile(fileName))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(img, 0, 0, bmp.Width, bmp.Height);
                }
            }

            return bmp;
        }
    }
}
