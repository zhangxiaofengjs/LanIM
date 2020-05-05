using Com.LanIM.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.UI
{
   public class ProfilePhotoPictureBox : PictureBox
    {
        private const int USER_STATUS_D = 12;

        private bool _mouseIn = false;
        //public UserStatus UserStatus { get; set; }
        //public bool DrawUserStatus { get; set; }

        public ProfilePhotoPictureBox()
        {
            this.SizeMode = PictureBoxSizeMode.Zoom;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;

            if (_mouseIn)
            {
                //鼠标在内加边框凸显
                Rectangle rect = this.ClientRectangle;
                rect.Width -= 1;
                rect.Height -= 1;
                g.DrawRectangle(Pens.DimGray, rect);
            }

            //画在线状态
            //if (DrawUserStatus)
            //{
            //    Rectangle rect = new Rectangle(this.Width - USER_STATUS_D - 2, this.Height - USER_STATUS_D - 2, USER_STATUS_D, USER_STATUS_D);
            //    g.FillEllipse(Brushes.White, rect);

            //    rect.X += 1;
            //    rect.Y += 1;
            //    rect.Width -= 2;
            //    rect.Height -= 2;
            //    switch (UserStatus)
            //    {
            //        case UserStatus.Busy:
            //            g.FillEllipse(Brushes.Crimson, rect);
            //            break;
            //        case UserStatus.Online:
            //            g.FillEllipse(Brushes.Green, rect);
            //            break;
            //        case UserStatus.Offline:
            //            g.FillEllipse(Brushes.Gray, rect);
            //            break;
            //    }
            //}
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
