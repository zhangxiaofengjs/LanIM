using Com.LanIM.Common;
using Com.LanIM.Common.Network;
using Com.LanIM.Components;
using Com.LanIM.Network;
using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM
{
    public partial class FormConfig : CommonForm
    {
        public FormConfig()
        {
            InitializeComponent();
        }

        private LanUser _user;
        internal LanUser User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                textBoxUserName.Text = _user.NickName;
                pictureBoxFace.Image = ProfilePhotoPool.GetPhoto(_user.ID);

                NCIInfo nicInfo = NCIInfo.GetNICInfo(_user.ID);
                textBoxNIC.Text = nicInfo == null ? "" : nicInfo.Name;
                labelIP.Text = _user.Address.ToString();
                textBoxBroadcastAddress.Text = NCIInfo.ConvertToString(_user.BroadcastAddress);
            }
        }

        private void pictureBoxFace_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "图像文件|*.png;*.jpg;*.bmp;";
                if(ofd.ShowDialog(this) == DialogResult.OK)
                {
                    string fileName = ofd.FileName;

                    //统一缩小处理后保存，随后会更新给各个客户端User.UpdateState()
                    Image img = ProfilePhotoPool.ScalePhoto(fileName);
                    UpdateProfilePhoto(img);
                }
            }
        }

        private void UpdateProfilePhoto(Image img)
        {
            ProfilePhotoPool.SetPhoto(User.ID, img);

            pictureBoxFace.Image = img;
            User.UpdateMyStateByBroadcast(UpdateState.Photo);
        }

        private void FormConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (User.NickName != textBoxUserName.Text)
            {
                User.NickName = textBoxUserName.Text;
                User.UpdateMyStateByBroadcast(UpdateState.NickName);

                LanClientConfig.Instance.NickName = User.NickName;
            }

            string str = NCIInfo.ConvertToString(LanClientConfig.Instance.BroadcastAddress);
            if (str != textBoxBroadcastAddress.Text)
            {
                LanClientConfig.Instance.BroadcastAddress = NCIInfo.ConvertToIPAddIfNotExist(textBoxBroadcastAddress.Text,
                    NCIInfo.GetBroadcastIP(LanClientConfig.Instance.MAC));
                User.BroadcastAddress = LanClientConfig.Instance.BroadcastAddress;
            }
        }

        private void labelSysPhoto_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SystemProfilePhotoControl sysPhC = new SystemProfilePhotoControl();
            sysPhC.ImageSelected += new ImageEventHandler((snder, args)=>
            {
                UpdateProfilePhoto(args.Image);
            });
            sysPhC.Show(labelSysPhoto);
        }

        private void ContextMenuStripMAC_NCIInfoSelected(object sender, NCIInfoEventArgs args)
        {
            textBoxNIC.Text = args.NCIInfo.Name;
            labelIP.Text = args.NCIInfo.Address.ToString();
            LanClientConfig.Instance.MAC = args.NCIInfo.MAC;
        }

        private void textBoxNIC_Click(object sender, EventArgs e)
        {
            contextMenuStripMAC.Show(textBoxNIC, new Point(0, textBoxNIC.Height));
        }
    }
}
