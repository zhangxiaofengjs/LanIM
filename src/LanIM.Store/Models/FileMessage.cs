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
    public class FileMessage : Message, IPrepare, IPost
    {
        //原图路径
        public string OriginFilePath { get; set; }
        public string FileName { get; set; }
        public long FileLength { get; set; }

        public FileMessage()
           : base(MessageType.File)
        {
        }

        public void Prepare()
        {
            //<r>
            //  <f></f>
            //  <l></l>
            //  <o></o>
            //</r>
            XmlDocument doc = new XmlDocument();

            XmlElement xe = doc.CreateElement("r");
            doc.AppendChild(xe);

            XmlNode fn = doc.CreateElement("f");
            fn.InnerText = FileName;
            xe.AppendChild(fn);

            XmlNode ln = doc.CreateElement("l");
            ln.InnerText = Convert.ToString(FileLength);
            xe.AppendChild(ln);

            XmlNode on = doc.CreateElement("o");
            on.InnerText = OriginFilePath;
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
                }

                node = xmlDoc.SelectSingleNode("r/l");
                if (node != null)
                {
                    this.FileLength = Convert.ToInt64(node.InnerText);
                }

                node = xmlDoc.SelectSingleNode("r/o");
                if (node != null)
                {
                    this.OriginFilePath = node.InnerText;
                }
            }
            catch (Exception e)
            {
                LoggerFactory.Error("xml 转换失败:{0}\n{1}", this.Content, e);
            }
        }
    }
}
