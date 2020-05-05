using Com.LanIM.Common.Logger;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common
{
    public class LanImage
    {
        private const int THUMBNAIL_IMAGE_WIDTH = 100;

        public static Icon CreateNumberIcon(Icon icon, int number, Font f)
        {
            //if (number == 0)
            {
                return icon;
            }

            //Image image = Image.FromHbitmap(icon.ToBitmap().GetHbitmap());
            //Size size = icon.Size;
            //Rectangle rect = new Rectangle(0, 0, size.Width, size.Height);
            //using (Graphics g = Graphics.FromImage(image))
            //{
            //    g.FillEllipse(Brushes.Red, new Rectangle(rect.Width / 2 - 2, rect.Height / 2 - 2, rect.Width / 2, rect.Height / 2));
            //    //g.DrawString(number > 99 ? "..." : number.ToString(), f, Brushes.Red, rect.Width / 2 - 2, rect.Height / 2 - 2);
            //    //    new Rectangle(rect.Width / 2 - 2, rect.Height / 2 - 2, rect.Width / 2, rect.Height / 2), new StringFormat()
            //    //{
            //    //    LineAlignment = StringAlignment.Center,
            //    //    Alignment = StringAlignment.Center
            //    //});
            //}

            //return ImageToIcon(image);
        }

        public static Icon ImageToIcon(Image img)
        {
            Bitmap bitmap = new Bitmap(img);
            System.IntPtr iconHandle = bitmap.GetHicon();
            return Icon.FromHandle(iconHandle);
        }

        public static Image GetThumbnailImage(string path, int width = THUMBNAIL_IMAGE_WIDTH)
        {
            try
            {
                Image img = Image.FromFile(path);
                return GetThumbnailImage(img, width);
            }
            catch(Exception e)
            {
                LoggerFactory.Error("无效的图片文件:{0}\r\n{1}", path, e);
                return null;
            }
        }

        public static Image GetThumbnailImage(Image image, int width = THUMBNAIL_IMAGE_WIDTH)
        {
            if(image == null)
            {
                return null;
            }

            if(image.Width < THUMBNAIL_IMAGE_WIDTH && image.Height < THUMBNAIL_IMAGE_WIDTH)
            {
                return new Bitmap(image);
            }
            else 
            {
                int w = image.Width; int h = image.Height;
                if (image.Width > image.Height)
                {
                    w = THUMBNAIL_IMAGE_WIDTH;
                    h = Math.Max(1, (int)(1.0 * image.Height * w / image.Width));
                }
                else
                {
                    h = THUMBNAIL_IMAGE_WIDTH;
                    w = Math.Max(1, (int)(1.0 * image.Width * h / image.Height));
                }

                return image.GetThumbnailImage(w, h, null, IntPtr.Zero);
            }
        }
    }
}
