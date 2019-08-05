using Com.LanIM.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.UI
{
    public class DropDownUserControl : UserControl
    {
        private DropDownControl _ddc;

        public DropDownUserControl()
        {
            _ddc = new DropDownControl(this);
        }

        public void Show(Control c)
        {
            _ddc.Show(c, new Rectangle(Point.Empty, c.Size));
        }

        public void Show(Point p)
        {
            _ddc.Show(p.X, p.Y);
        }

        public void Close()
        {
            _ddc.Close();
        }
    }
}
