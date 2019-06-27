using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM
{
    public partial class FormConfig : CommonForm
    {
        bool _edit = false;

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
                    ProfilePhotoPool.SetPhoto(User.ID, img);

                    pictureBoxFace.Image = img;
                    _edit = true;
                }
            }
        }

        private void FormConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_edit)
            {
                User.NickName = textBoxUserName.Text;
                User.UpdateState();
            }
        }

        private void textBoxUserName_TextChanged(object sender, EventArgs e)
        {
            _edit = true;
        }
    }
}
