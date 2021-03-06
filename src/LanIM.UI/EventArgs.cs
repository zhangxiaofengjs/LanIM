﻿using Com.LanIM.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.UI
{
    public class MeasureItemEventArgs
    {
        public Graphics Graphics { get; }
        public ScrollableListItem Item { get; }
        public Font Font { get; set; }

        public MeasureItemEventArgs(ScrollableListItem item, Graphics g, Font font)
        {
            this.Item = item;
            this.Graphics = g;
            this.Font = font;
        }
    }

    public class DrawItemEventArgs
    {
        public Graphics Graphics { get; }
        public ScrollableListItem Item { get; }
        public Font Font{ get; set; }
        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }
        public Color SelectedBackColor { get; set; }
        public Color FocusBackColor { get; set; }
        public bool Focus { get; }
        public bool Selected { get; }

        public DrawItemEventArgs(ScrollableListItem item, 
            Graphics g, bool focus, bool selected,
            Font font, Color foreColor, Color backColor)
        {
            this.Item = item;
            this.Graphics = g;
            this.Font = font;
            this.ForeColor = foreColor;
            this.BackColor = backColor;
            this.SelectedBackColor = LanColor.DarkLight(backColor, -0.06f);
            this.FocusBackColor = LanColor.DarkLight(backColor, -0.03f);
            this.Focus = focus;
            this.Selected = selected;
        }

        public void DrawBackground()
        {
            using (Brush brush = new SolidBrush(this.Selected ? this.SelectedBackColor : 
                this.Focus ? this.FocusBackColor : this.BackColor))
            {
                this.Graphics.FillRectangle(brush, this.Item.Bounds);
            }
        }
    }
    public class ItemClickedEventArgs
    {
        public ScrollableListItem Item { get; }
        public MouseButtons Button { get; }
        public Point Location { get; }

        public ItemClickedEventArgs(ScrollableListItem item, MouseButtons button, Point loc)
        {
            this.Item = item;
            this.Button = button;
            this.Location = loc;
        }
    }

    public class ItemHoverEventArgs
    {
        public ScrollableListItem Item { get; }
        public Point Location { get; }

        public ItemHoverEventArgs(ScrollableListItem item, Point loc)
        {
            this.Item = item;
            this.Location = loc;
        }
    }
    public class SearchEventArgs
    {
        public string Text { get; }

        public SearchEventArgs(string text)
        {
            this.Text = text;
        }
    }
}
