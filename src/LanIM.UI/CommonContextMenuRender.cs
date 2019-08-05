using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.UI
{
    public class CommonContextMenuRender : ToolStripRenderer
    {
        public CommonContextMenuRender()
        {
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.AffectedBounds);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripItem item = e.Item;
            Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
            if (item.Selected)
            {
                e.Graphics.FillRectangle(Brushes.LightGray, rect);
            }
            else if (item.Pressed)
            {
                e.Graphics.FillRectangle(Brushes.LightGray, rect);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            Rectangle rect = e.AffectedBounds;
            rect.Width -= 1;
            rect.Height -= 1;
            e.Graphics.DrawRectangle(Pens.LightGray, rect);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = Color.Black;//总是黑字
            base.OnRenderItemText(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
            e.Graphics.DrawLine(Pens.LightGray, rect.X, rect.Y + rect.Height / 2, rect.Width + rect.X, rect.Y + rect.Height / 2);
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            base.OnRenderItemCheck(e);
        }
    }
}
