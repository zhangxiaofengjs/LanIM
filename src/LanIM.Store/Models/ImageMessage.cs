using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store.Models
{
    public class ImageMessage : Message
    {
        public Image Image { get; set; }

        public ImageMessage(Image image)
        {
            this.Image = image;
        }
    }
}
