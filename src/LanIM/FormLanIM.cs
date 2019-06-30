using Com.LanIM.Common;
using Com.LanIM.Network;
using Com.LanIM.Store;
using Com.LanIM.Store.Models;
using Com.LanIM.Components;
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
using Com.LanIM.UI;

namespace Com.LanIM
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
            _user.IP = IPv4Address.GetLocalMachineIPV4();
            _user.NickName = LanConfig.Instance.NickName;
            _user.State = LanUserStatus.Online;
            _user.UserEntry += _user_UserEntry;
            _user.UserExit += _user_UserExit;
            _user.UserStateChange += _user_UserStateChange;
            _user.Send += _user_Send;
            _user.TextMessageReceived += _user_TextMessageReceived;
            _user.ImageReceived += _user_ImageReceived;
            _user.FileTransportRequested += _user_FileTransportRequested;
            _user.FileReceiveProgressChanged += _user_FileReceiveProgressChanged;
            _user.FileReceiveCompleted += _user_FileReceiveCompleted;
            _user.FileReceiveError += _user_FileReceiveError;
            _user.FileSendProgressChanged += _user_FileSendProgressChanged;
            _user.FileSendCompleted += _user_FileSendCompleted;
            _user.FileSendError += _user_FileSendError;
            LoadContacters();
        }

        private void LoadContacters()
        {
            ContacterMapper mapper = new ContacterMapper();
            List<Contacter> cs = mapper.Query();

            foreach (Contacter contacter in cs)
            {
                LanUser user = new LanUser();
                user.NickName = contacter.NickName;
                user.IP.MAC = contacter.MAC;
                user.IP.Address = IPv4Address.Parse(contacter.IP);

                _user.Contacters.Add(user);
            }
        }

        private void InitUI(SynchronizationContext context)
        {
            //task线程调用更新主界面
            context.Post((state) =>
            {
                pictureBoxFace.Image = ProfilePhotoPool.GetPhoto(_user.ID);
                labelName.Text = _user.NickName;
                labelStatus.Text = "在线中...";

                List<UserListItem> ulItems = new List<UserListItem>();

                foreach (LanUser user in _user.Contacters)
                {
                    if (user.ID == _user.ID)
                    {
                        //忽视自己
                        continue;
                    }
                    ulItems.Add(new UserListItem(user));
                }

                userListBox.Items.AddRange(ulItems);
            }, null);
        }

        private void _user_FileReceiveError(object sender, FileTransportErrorEventArgs args)
        {
        }

        private void _user_FileSendError(object sender, FileTransportErrorEventArgs args)
        {
        }

        private void _user_FileSendCompleted(object sender, FileTransportEventArgs args)
        {

        }

        private void _user_FileReceiveCompleted(object sender, FileTransportEventArgs args)
        {
            UserChatControl ucc = GetUserChatControl(_user[args.File.MAC]);
            ucc.AddFileMessage(args.File.SavePath);
        }

        private void _user_FileSendProgressChanged(object sender, FileTransportEventArgs args)
        {
            TransportFile file = args.File;
            //OutputLog("发送文件[" + file.File.Name + "]," + file.Progress + "%," +
            //    LanFile.HumanReadbleLen(file.TransportedLength) + "/" +
            //    LanFile.HumanReadbleLen(file.File.Length) + "," +
            //    LanFile.HumanReadbleLen(file.TransportedSpeed) + "/s");
        }

        private void _user_FileReceiveProgressChanged(object sender, FileTransportEventArgs args)
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
            ucc.AddTextMessage(args.ID, args.Message);
        }

        private void _user_Send(object sender, SendEventArgs args)
        {
            LanUser user = _user[args.Packet.ToMAC];
            UserChatControl ucc = GetUserChatControl(user);

            ucc.SetMessageSendResult(args.Packet.ID, args.Success);
        }

        private void _user_UserStateChange(object sender, UserStateChangeEventArgs args)
        {
            UpdateContacter(args);
        }

        private void _user_UserExit(object sender, UserStateChangeEventArgs args)
        {
            UpdateContacter(args);
        }

        private void _user_UserEntry(object sender, UserStateChangeEventArgs args)
        {
            UpdateContacter(args);
        }

        private void UpdateContacter(UserStateChangeEventArgs args)
        {
            LanUser user = args.User;
            if(user.ID == _user.ID)
            {
                //自己就忽略掉，本身也不显示在list里面
                return;
            }

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
            UserChatControl ucc = GetUserChatControl(args.User);
            ucc.AddImageMessage(args.Image);
        }

        private void UserListBox_SelectionChanged(object sender, System.EventArgs e)
        {
            LanUser user = this.userListBox.SelectedUser;
            if (user == null)
            {
                return;
            }

            labelUserName.Text = user.NickName;
            UserChatControl ucc = GetUserChatControl(user);
            ucc.BringToFront();
        }

        private UserChatControl GetUserChatControl(LanUser user, bool bNullCreate = true)
        {
            UserChatControl ucc = null;
            foreach (Control c in this.Controls)
            {
                if (c is UserChatControl)
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
                ucc.InitializeLatastMessage();
                this.Controls.Add(ucc);
            }

            if (ucc != null)
            {
                ucc.Location = new Point(this.searchBox.Right, searchBox.Top + 1);
                ucc.Size = new Size(this.ClientSize.Width - this.searchBox.Right - 1, this.ClientSize.Height - searchBox.Top - 2);
            }
            return ucc;
        }

        private void pictureBoxFace_Click(object sender, EventArgs e)
        {
            FormConfig fc = new FormConfig();
            fc.User = _user;
            fc.ShowDialog(this);

            //更新头像等一系列
            pictureBoxFace.Image = ProfilePhotoPool.GetPhoto(_user.ID);
            labelName.Text = _user.NickName;
        }
    }
}
