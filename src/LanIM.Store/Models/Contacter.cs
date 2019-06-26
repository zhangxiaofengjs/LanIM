using Com.LanIM.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store.Models
{
    public class Contacter
    {
        public long ID { get; set; }
        public string NickName { get; set; }
        public string MAC { get; set; }
        public string IP { get; set; }
        public Image ProfilePhoto {
            get
            {
                string path = Path.Combine(LanConfig.Instance.ProfilePhotoPath, this.MAC);
                if(File.Exists(path))
                {
                    return Image.FromFile(path);
                }
                return null;
            }
        }
    }
}
