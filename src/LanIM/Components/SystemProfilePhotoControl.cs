using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Com.LanIM.UI;

namespace Com.LanIM.Components
{
    partial class SystemProfilePhotoControl : DropDownUserControl
    {
        public event ImageEventHandler ImageSelected;
        public SystemProfilePhotoControl()
        {
            InitializeComponent();
        }

        private void profilePhotoPictureBox_Click(object sender, EventArgs e)
        {
            Image selectedImage = (sender as PictureBox).Image;
            this.Close();

            ImageSelected?.Invoke(this, new ImageEventArgs(selectedImage));
        }
    }
}
