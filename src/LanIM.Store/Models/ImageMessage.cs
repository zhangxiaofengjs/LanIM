using Com.LanIM.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store.Models
{
    public class ImageMessage : Message, IPrepare
    {
        //LanIM的存储路径
        public string FilePath { get; set; }
        //原图路径
        public string OriginPath { get; set; }

        public Image Image{ get; set; }

        public ImageMessage(Image m)
            : base(MessageType.Image)
        {
            this.Image = m;
        }

        public void Prepare()
        {
            //保存文件
            string fileName = Path.GetRandomFileName() + ".png";
            string filePath = Path.Combine(LanConfig.Instance.PicturePath, fileName);
            Image.Save(filePath, ImageFormat.Png);

            this.FilePath = filePath;
            this.Content = fileName + "\n" + (string.IsNullOrEmpty(this.OriginPath)?filePath:this.OriginPath);
        }
    }
}
