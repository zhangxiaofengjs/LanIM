﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.UI.Components
{
    class CommonToolBar : UserControl
    {
        public readonly CommonToolBarItemCollection Items;

        public CommonToolBar()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            this.BackColor = Color.White;
            this.Height = 30;
            this.Padding = new Padding(10,5,5,5);
            Items = new CommonToolBarItemCollection(this);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawItems(e);
        }

        private void DrawItems(PaintEventArgs e)
        {
            Point mouseP = PointToClient(MousePosition);
            foreach (CommonToolBarItem item in Items)
            {
                Rectangle rect = item.Bounds;
                if (rect.IntersectsWith(e.ClipRectangle))
                {
                    e.Graphics.DrawImage(rect.Contains(mouseP) ? item.ImageFocus : item.Image, item.Bounds);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            //被选中时focus重画
            this.Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            foreach (CommonToolBarItem item in Items)
            {
                if(item.Bounds.Contains(e.Location))
                {
                    item.RaiseClickEvent();
                    break;
                }
            }
        }

        public void MeasureItems()
        {
            //以高度为准，上下偏移this.Padding.Top/Bottom像素
            int offset = (this.Padding.Top + this.Padding.Bottom) / 2;
            Rectangle rect = new Rectangle(this.Padding.Left + offset, offset,
                this.Height - offset * 2, this.Height - offset * 2);

            foreach (CommonToolBarItem item in Items)
            {
                item.Bounds = rect;
                rect.X += this.Height;
            }
        }
    }
}
