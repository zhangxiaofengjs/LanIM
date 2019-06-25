using Com.LanIM.Network;
using Com.LanIM.Store.Models;
using Com.LanIM.UI.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = Com.LanIM.Store.Models.Message;

namespace Com.LanIM.UI
{
    partial class FormLanIM : CommonForm
    {
        private LanUser _user = null;

        public FormLanIM()
        {
            InitializeComponent();
        }


        private void FormLanIM_Load(object sender, EventArgs e)
        {
            _user = new LanUser();
            _user.NickName = LanConfig.Instance.NickName;
            _user.UserEntry += _user_UserEntry;
            _user.UserExit += _user_UserExit;
            _user.UserStateChange += _user_UserStateChange;
            _user.Send += _user_Send;
            _user.TextMessageReceived += _user_TextMessageReceived;
            _user.ImageReceived += _user_ImageReceived;
            _user.FileTransportRequested += _user_FileTransportRequested;
            _user.FileReceiveProgressChanged += _user_FileTransportProgressChanged;
            _user.FileSendProgressChanged += _user_FileSendProgressChanged;
            _user.Listen();

            //1秒后发送上线通知
            Thread.Sleep(1000);
            _user.Login();
        }

        private void _user_FileSendProgressChanged(object sender, FileTransportEventArgs args)
        {
            TransportFile file = args.File;
            //OutputLog("发送文件[" + file.File.Name + "]," + file.Progress + "%," +
            //    LanFile.HumanReadbleLen(file.TransportedLength) + "/" +
            //    LanFile.HumanReadbleLen(file.File.Length) + "," +
            //    LanFile.HumanReadbleLen(file.TransportedSpeed) + "/s");
        }

        private void _user_FileTransportProgressChanged(object sender, FileTransportEventArgs args)
        {
            TransportFile file = args.File;
            //OutputLog("接收文件[" + file.File.Name + "]," + file.Progress + "%," +
            //    LanFile.HumanReadbleLen(file.TransportedLength) + "/" +
            //    LanFile.HumanReadbleLen(file.File.Length) + "," +
            //    LanFile.HumanReadbleLen(file.TransportedSpeed) + "/s");
        }

        private void _user_FileTransportRequested(object sender, FileTransportRequestedEventArgs args)
        {
            if (MessageBox.Show("是否接受文件：\r\nFrom:" + args.User + "\r\nFile:" + args.File, "", MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    TransportFile file = args.File;
                    file.SavePath = LanConfig.Instance.ReceivedFilePath;
                    _user.ReceiveFile(file);
                }
            }
            else
            {
                //_user.RejectReceiveFile(args.ID);
            }
        }

        private void _user_TextMessageReceived(object sender, TextMessageReceivedEventArgs args)
        {
            Message m = new TextMessage(args.Message);
            m.UserId = args.User.IP.MAC;

            MessageListItem item = new MessageListItem();
            item.Message = m;

            //messageListBox.Items.Add(item);
        }

        private void _user_Send(object sender, SendEventArgs args)
        {
            //OutputLog("发送: success=" + args.Success + ", packet=" + args.Packet);
        }

        private void _user_UserStateChange(object sender, UserStateChangeEventArgs args)
        {
            //comboBoxUsers.Items.Clear();
            //comboBoxUsers.Items.AddRange(_user.Contacters.ToArray());
            //if (comboBoxUsers.Items.Count > 0)
            //    comboBoxUsers.SelectedIndex = 0;
            //OutputLog("状态更新:" + args.User.ToString());
            LanUser user = args.User;
            userListBox.UpdateUser(user);
        }

        private void _user_UserExit(object sender, UserStateChangeEventArgs args)
        {
            LanUser user = args.User;
            userListBox.UpdateUser(user);
        }

        private void _user_UserEntry(object sender, UserStateChangeEventArgs args)
        {
            LanUser user = args.User;
            if (userListBox.ContainsUser(user))
            {
                userListBox.UpdateUser(user);
            }
            else
            {
                userListBox.AddUser(user);
            }
         }

        private void _user_ImageReceived(object sender, ImageReceivedEventArgs args)
        {
            //OutputLog("接受图片: user=" + args.User + ", image=");
            //Clipboard.SetImage(args.Image);
            //richTextBox1.Paste();
            //richTextBox1.AppendText("\r\n");
        }

        private void pictureBoxFace_MouseEnter(object sender, EventArgs e)
        {
            pictureBoxFace.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pictureBoxFace_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxFace.BorderStyle = BorderStyle.None;
        }

        private void UserListBox_SelectionChanged(object sender, System.EventArgs e)
        {
            LanUser user = this.userListBox.SelectedUser;
            if(user == null)
            {
                return;
            }
            ShowUserChatControl(user);
        }

        private UserChatControl ShowUserChatControl(LanUser user)
        {
            UserChatControl ucc = null;
            foreach (Control c in this.Controls)
            {
                if(c is UserChatControl)
                {
                    UserChatControl uccTmp = c as UserChatControl;
                    if (uccTmp.User.ID == user.ID)
                    {
                        ucc = uccTmp;
                        break;
                    }
                }
            }

            //不存在创建
            //if (ucc == null)
            {
                ucc = new UserChatControl();
                ucc.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                ucc.User = _user;
                ucc.Contacter = user;
                this.Controls.Add(ucc);
            }
            ucc.Location = new Point(this.searchBox.Right, searchBox.Top + 1);
            ucc.Size = new Size(this.ClientSize.Width - this.searchBox.Right - 1, this.ClientSize.Height - searchBox.Top - 2);
            ucc.BringToFront();
            return ucc;
        }

        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            //if(e.KeyCode == Keys.Enter && !e.Control )
            //{
            //    UserListItem selectItem = userListBox.SelectedItem as UserListItem;
            //    if (selectItem == null)
            //    {
            //        PrepareUserChatControl();
            //        ucc.SendTextMessage();
            //    }
            //}
        }
    }
}
