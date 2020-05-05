using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Com.LanIM.UI
{
    public partial class SearchBox : UserControl
    {
        private static readonly int MARGIN = 2;
        private static readonly int LEFT_MARGIN = 12;
        
        public event SearchEventHandler SearchTextChanged;

        private Rectangle SearchAreaBounds = Rectangle.Empty;

        public SearchBox()
        {
            InitializeComponent();
            RecalcSize();
        }

        private void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            SearchEventArgs se = new SearchEventArgs(filterTextBox.Text);
            SearchTextChanged(this, se);
        }

        private void SearchBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            g.DrawRectangle(Pens.Gray, SearchAreaBounds);
            g.FillRectangle(BrushPool.GetBrush(200, 255, 255, 255), SearchAreaBounds);
            g.DrawImage(Properties.Resources.search, SearchAreaBounds.X + MARGIN, SearchAreaBounds.Y + MARGIN, filterTextBox.Height, filterTextBox.Height);
        }

        private void SearchBox_Resize(object sender, EventArgs e)
        {
        }

        private void RecalcSize()
        {
            int boxWidth = this.ClientSize.Width - LEFT_MARGIN * 2;
            int boxHeight = filterTextBox.Height + MARGIN * 2;
            SearchAreaBounds = new Rectangle((this.ClientSize.Width - boxWidth) / 2,
                (this.ClientSize.Height - boxHeight) / 2,
                boxWidth, boxHeight);

            filterTextBox.Top = SearchAreaBounds.Y + MARGIN;
            filterTextBox.Left = SearchAreaBounds.X + filterTextBox.Height + MARGIN * 2;
            filterTextBox.Width = SearchAreaBounds.Width - filterTextBox.Height - MARGIN * 3;
        }

        private void filterTextBox_FontChanged(object sender, EventArgs e)
        {
            RecalcSize();
        }

        private void SearchBox_SizeChanged(object sender, EventArgs e)
        {
            RecalcSize();
        }
    }
}
