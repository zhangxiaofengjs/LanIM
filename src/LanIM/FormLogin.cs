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
using Com.LanIM.Common.Network;

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
            List<NCIInfo> nciInfos = NCIInfo.GetNICInfo( NCIType.Physical | NCIType.Wireless);
            NCIInfo nciInfo = nciInfos.Find(new Predicate<NCIInfo>((item) =>
            {
                if (item.MAC == LanClientConfig.Instance.MAC)
                {
                    return true;
                }
                return false;
            }));

            if(nciInfo == null)
            {
                if(nciInfos.Count >= 1)
                {
                    nciInfo = nciInfos[0];
                    LanClientConfig.Instance.MAC = nciInfo.MAC;
                }
            }

            pictureBox.Image = ProfilePhotoPool.GetPhoto(LanClientConfig.Instance.MAC);
            labelLogin.Text = LanClientConfig.Instance.NickName;
            labelNIC.Text = nciInfo.Name;
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void linkLabelMAC_Clicked(object sender, EventArgs e)
        {
            contextMenuStripMAC.Show(this.linkLabelMAC, this.linkLabelMAC.Left, this.linkLabelMAC.Height);
        }

        private void ContextMenuStripMAC_NCIInfoSelected(object sender, NCIInfoEventArgs args)
        {
            labelNIC.Text = args.NCIInfo.Name;
            LanClientConfig.Instance.MAC = args.NCIInfo.MAC;
        }
    }
}
