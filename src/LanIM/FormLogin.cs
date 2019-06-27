using Com.LanIM.Common;
using Com.LanIM.Network;
using Com.LanIM.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Com.LanIM.UI;

namespace Com.LanIM
{
    partial class FormLogin : CommonForm
    {
        private Font _loginLabelFont;
        private Font _loginLabelFontFocus;

        public FormLogin()
        {
            InitializeComponent();

            this._loginLabelFont = labelLogin.Font;
            this._loginLabelFontFocus = new Font(labelLogin.Font, FontStyle.Bold);
        }

        private void labelLogin_MouseEnter(object sender, EventArgs e)
        {
            labelLogin.Font = _loginLabelFontFocus;
        }

        private void labelLogin_MouseLeave(object sender, EventArgs e)
        {
            labelLogin.Font = _loginLabelFont;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            IPv4Address ip = IPv4Address.GetLocalMachineIPV4();

            pictureBox.Image = ProfilePhotoPool.GetPhoto(ip.MAC);
            labelLogin.Text = Environment.UserName;
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
