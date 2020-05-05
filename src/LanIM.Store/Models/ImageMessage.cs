using Com.LanIM.Common;
using Com.LanIM.Common.Logger;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Com.LanIM.Store.Models
{
    public class ImageMessage : Message, IPrepare, IPost
    {
        //原图路径
        public string OriginPath { get; set; }
        public string FileName { get; set; }
        public Image Image { get; set; }
        public string Path
        {
            get
            {
                if(!string.IsNullOrEmpty(this.OriginPath))
                {
                    return this.OriginPath;
                }
                return LanEnv.GetPictureFilePath(this.FileName);
            }
        }

        public ImageMessage()
           : base(MessageType.Image)
        {
        }

        public ImageMessage(Image m)
            : base(MessageType.Image)
        {
            this.Image = m;
        }

        public void Prepare()
        {
            //<r>
            //  <f></f>
            //  <o></o>
            //</r>
            XmlDocument doc = new XmlDocument();

            XmlElement xe = doc.CreateElement("r");
            doc.AppendChild(xe);

            XmlNode fn = doc.CreateElement("f");
            fn.InnerText = FileName;
            xe.AppendChild(fn);

            XmlNode on = doc.CreateElement("o");
            on.InnerText = OriginPath;
            xe.AppendChild(on);

            StringWriter swt = new StringWriter();
            doc.Save(swt);
            
            this.Content = swt.ToString();
        }

        public void Post()
        {
            try
            {
                StringReader reader = new StringReader(this.Content);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);

                XmlNode node = xmlDoc.SelectSingleNode("r/f");
                if (node != null)
                {
                    this.FileName = node.InnerText;
                    string filePath = LanEnv.GetPictureFilePath(FileName);
                    if (File.Exists(filePath))
                    {
                        this.Image = Image.FromFile(filePath);
                    }
                }

                node = xmlDoc.SelectSingleNode("r/o");
                if (node != null)
                {
                    this.OriginPath = node.InnerText;
                }
            }
            catch (Exception e)
            {
                LoggerFactory.Error("xml 转换失败:{0}\n{1}", this.Content, e);
            }

            if(this.Image == null)
            {
                this.Image = Properties.Resources.broken_picture;
            }
        }
    }
}
