using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.UI
{
    public partial class SearchBox : UserControl
    {
        public event SearchEventHandler SearchTextChanged;
        public SearchBox()
        {
            InitializeComponent();
        }

        private void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            SearchEventArgs se = new SearchEventArgs(filterTextBox.Text);
            SearchTextChanged(this, se);
            
            
        }
    }
}
