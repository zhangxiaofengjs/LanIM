using Com.LanIM.Network;
using Com.LanIM.UI.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.UI
{
    partial class FormLogin : CommonForm
    {
        private Font _loginLabelFont;
        private Font _loginLabelFontFocus;

        public event EventHandler LoginClicked;

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

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox.BorderStyle = BorderStyle.None;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            labelLogin.Text = Environment.UserName;
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            this.Close();
            //LoginClicked?.Invoke(this, new EventArgs());
        }
    }
}
