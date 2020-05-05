using Com.LanIM.Store;
using Com.LanIM.Store.Models;
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

namespace Com.LanIM.Components
{
    partial class UserProfileControl : DropDownUserControl
    {
        private LanUser _user;
        public LanUser User
        {
            set
            {
                _user = value;
                this.labelNickName.Text = _user.NickName;
                this.labelMAC.Text = _user.MAC;
                this.textBoxIP.Text = _user.Address.ToString();
                this.textBoxMemo.Text = _user.Memo;
            }
        }
        public UserProfileControl()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            ContacterMapper contacterMapper = new ContacterMapper();

            Contacter c = new Contacter();
            c.MAC = this._user.MAC;
            c.Memo = this.textBoxMemo.Text;

            contacterMapper.UpdateMemo(c);

            _user.Memo = textBoxMemo.Text;
        }
    }
}
