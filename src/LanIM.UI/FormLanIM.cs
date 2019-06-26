using Com.LanIM.Common;
using Com.LanIM.Network;
using Com.LanIM.Store;
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
            //此处需要注意，一定要把主线程UI传给LanUser，否则下面的线程调用主线程的部分会错，
            //因为基本都没有采用invoke方式（虽然invoke方式也可以对应这种情况）
            SynchronizationContext context = SynchronizationContext.Current;

            TaskFactory tf = new TaskFactory();
            tf.StartNew(() => 
            {
                InitMainUser(context);

                InitUI(context);

                //开始监听
                _user.Listen();

                //1秒后发送上线通知
                Thread.Sleep(1000);
                _user.Login();
            });
        }

        private void InitMainUser(SynchronizationContext context)
        {
            _user = new LanUser(context);
            _user.IP = GetIP();
            _user.NickName = LanConfig.Instance.NickName;
            _user.State = LanUserState.Online;
            _user.UserEntry += _user_UserEntry;
            _user.UserExit += _user_UserExit;
            _user.UserStateChange += _user_UserStateChange;
            _user.Send += _user_Send;
            _user.TextMessageReceived += _user_TextMessageReceived;
            _user.ImageReceived += _user_ImageReceived;
            _user.FileTransportRequested += _user_FileTransportRequested;
            _user.FileReceiveProgressChanged += _user_FileTransportProgressChanged;
            _user.FileSendProgressChanged += _user_FileSendProgressChanged;
            _user.ProfilePhoto = LanConfig.Instance.ProfilePhoto;

            LoadContacters();
        }

        private void LoadContacters()
        {
            ContacterMapper mapper = new ContacterMapper();
            List<Contacter> cs = mapper.QueryList();

            foreach (Contacter contacter in cs)
            {
                LanUser user = new LanUser();
                user.NickName = contacter.NickName;
                user.IP.MAC = contacter.MAC;
                user.IP.Address = IPv4Address.Parse(contacter.IP);
                user.ProfilePhoto = contacter.ProfilePhoto;

                _user.Contacters.Add(user);
            }
        }

        private void InitUI(SynchronizationContext context)
        {
            //task线程调用更新主界面
            context.Post((state) =>
            {
                if (_user.ProfilePhoto != null)
                {
                    pictureBoxFace.Image = _user.ProfilePhoto;
                }

                labelStatus.Text = "在线中...";

                List<UserListItem> ulItems = new List<UserListItem>();

                foreach (LanUser user in _user.Contacters)
                {
                    ulItems.Add(new UserListItem(user));
                }

                userListBox.Items.AddRange(ulItems);
            }, null);
        }

        private IPv4Address GetIP()
        {
            List<IPv4Address> ips = NetworkCardInterface.GetIPv4Address();
            foreach (IPv4Address ip in ips)
            {
                if (ip.NetworkCardInterfaceType == NetworkCardInterfaceType.Physical ||
                    ip.NetworkCardInterfaceType == NetworkCardInterfaceType.Wireless)
                {
                    return ip;
                }
            }
            return null;
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
            UserChatControl ucc = GetUserChatControl(args.User);
            ucc.AddMessage(args.Message);
        }

        private void _user_Send(object sender, SendEventArgs args)
        {
            //OutputLog("发送: success=" + args.Success + ", packet=" + args.Packet);
        }

        private void _user_UserStateChange(object sender, UserStateChangeEventArgs args)
        {
            UpdateUser(args);
        }

        private void _user_UserExit(object sender, UserStateChangeEventArgs args)
        {
            UpdateUser(args);
        }

        private void _user_UserEntry(object sender, UserStateChangeEventArgs args)
        {
            UpdateUser(args);
        }

        private void UpdateUser(UserStateChangeEventArgs args)
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

            UserChatControl ucc = GetUserChatControl(user, false);
            if (ucc != null)
            {
                ucc.Contacter = user;
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
            UserChatControl ucc = GetUserChatControl(user);
                ucc.BringToFront();
        }

        private UserChatControl GetUserChatControl(LanUser user, bool bNullCreate = true)
        {
            UserChatControl ucc = null;
            foreach (Control c in this.Controls)
            {
                if(c is UserChatControl)
                {
                    UserChatControl uccTmp = c as UserChatControl;
                    if (uccTmp.Contacter.ID == user.ID)
                    {
                        ucc = uccTmp;
                        break;
                    }
                }
            }

            //不存在创建
            if (ucc == null && bNullCreate)
            {
                ucc = new UserChatControl();
                ucc.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                ucc.User = _user;
                ucc.Contacter = user;
                this.Controls.Add(ucc);
            }

            if (ucc != null)
            {
                ucc.Location = new Point(this.searchBox.Right, searchBox.Top + 1);
                ucc.Size = new Size(this.ClientSize.Width - this.searchBox.Right - 1, this.ClientSize.Height - searchBox.Top - 2);
            }
            return ucc;
        }
    }
}
