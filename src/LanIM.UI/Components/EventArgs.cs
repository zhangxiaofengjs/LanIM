﻿using Com.LanIM.Common;
using Com.LanIM.Network;
using Com.LanIM.Network.Packets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.UI.Components
{
    class MeasureItemEventArgs
    {
        public Graphics Graphics { get; }
        public ScrollableListItem Item { get; }
        public int Index { get; }

        public MeasureItemEventArgs(int index, ScrollableListItem item, Graphics g)
        {
            this.Index = index;
            this.Item = item;
            this.Graphics = g; 
        }
    }

    class DrawItemEventArgs
    {
        public Graphics Graphics { get; }
        public ScrollableListItem Item { get; }
        public int Index { get; }
        public Rectangle Bounds { get; }
        public Font Font{ get; set; }
        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }
        public Color SelectedBackColor { get; set; }
        public Color FocusBackColor { get; set; }
        public bool Focus { get; }
        public bool Selected { get; }

        public DrawItemEventArgs(int index, ScrollableListItem item, 
            Graphics g, Rectangle itemBounds, bool focus, bool selected,
            Font font, Color foreColor, Color backColor)
        {
            this.Index = index;
            this.Item = item;
            this.Graphics = g;
            this.Bounds = itemBounds;
            this.Font = font;
            this.ForeColor = foreColor;
            this.BackColor = backColor;
            this.SelectedBackColor = LanColor.DarkLight(backColor, -0.3f);
            this.FocusBackColor = LanColor.DarkLight(backColor, -0.1f);
            this.Focus = focus;
            this.Selected = selected;
        }

        internal void DrawBackground()
        {
            using (Brush brush = new SolidBrush(this.Selected ? this.SelectedBackColor : 
                this.Focus ? this.FocusBackColor : this.BackColor))
            {
                this.Graphics.FillRectangle(brush, this.Bounds);
            }
        }
    }
    class ItemClickedEventArgs
    {
        public ScrollableListItem Item { get; }

        public ItemClickedEventArgs(ScrollableListItem item)
        {
            this.Item = item;
        }
    }
}
