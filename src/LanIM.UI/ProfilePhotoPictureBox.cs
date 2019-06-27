using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.UI
{
   public class ProfilePhotoPictureBox : PictureBox
    {
        private bool _mouseIn = false;

        public ProfilePhotoPictureBox()
        {
            this.SizeMode = PictureBoxSizeMode.Zoom;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if(_mouseIn)
            {
                Rectangle rect = this.ClientRectangle;
                rect.Width -= 1;
                rect.Height -= 1;
                pe.Graphics.DrawRectangle(Pens.DimGray, rect);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this._mouseIn = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this._mouseIn = false;
            this.Invalidate();
        }
    }
}
