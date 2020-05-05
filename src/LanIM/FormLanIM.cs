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
using Com.LanIM.Network.Packets;
using Com.LanIM.Common.Network;
using Com.LanIM.Common.Logger;

namespace Com.LanIM
{
    partial class FormLanIM : CommonForm
    {
        private LanUser _user = null;
        private bool _trayBlinkIconFlg = false;

        public FormLanIM()
        {
            InitializeComponent();

            this.contextMenuStripStatus.Renderer = new CommonContextMenuRender();
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

                InitUserListBox(context, "");

                //开始监听
                _user.Listen();

                //1秒后发送上线通知
                Thread.Sleep(800);
                _user.Login();
            });
        }

        private void InitMainUser(SynchronizationContext context)
        {
            _user = new LanUser(context);

            if (LanClientConfig.Instance.HideStatus)
            {
                _user.Status = UserStatus.Offline;
            }
            else
            {
                _user.Status = UserStatus.Online;
            }
            _user.MAC = LanClientConfig.Instance.MAC;
            _user.Address = NCIInfo.GetIPAddress(LanClientConfig.Instance.MAC);
            _user.BroadcastAddress = LanClientConfig.Instance.BroadcastAddress;
            _user.NickName = LanClientConfig.Instance.NickName;

            //events
            _user.UserEntry += _user_UserEntry;
            _user.UserExit += _user_UserExit;
            _user.UserStateChange += _user_UserStateChange;
            _user.Send += _user_Send;
            _user.TextMessageReceived += _user_TextMessageReceived;
            _user.ImageReceived += _user_ImageReceived;
            _user.ImageReceiveError += _user_ImageReceiveError;
            _user.ImageReceiveProgressChanged += _user_ImageReceiveProgressChanged;
            _user.FileTransportRequested += _user_FileTransportRequested;
            _user.FileReceiveProgressChanged += _user_FileReceiveProgressChanged;
            _user.FileReceiveCompleted += _user_FileReceiveCompleted;
            _user.FileReceiveError += _user_FileReceiveError;
            _user.FileSendProgressChanged += _user_FileSendProgressChanged;
            _user.FileSendCompleted += _user_FileSendCompleted;
            _user.FileSendError += _user_FileSendError;

            //加载用户列表
            LoadContacters();
        }

        private void _user_ImageReceiveProgressChanged(object sender, FileTransportEventArgs args)
        {
            //因为接收速度很快，暂未对应图片的接受进度
        }

        private void _user_ImageReceiveError(object sender, FileTransportErrorEventArgs args)
        {
            TransportFile file = args.File;
            LanUser user = _user[args.File.MAC];

            this.userListBox.SetMessageResult(user.ID, _user.ID, file.ID, false);
        }

        private void LoadContacters()
        {
            ContacterMapper mapper = new ContacterMapper();
            List<Contacter> cs = mapper.Query();

            foreach (Contacter contacter in cs)
            {
                LanUser user = new LanUser();
                user.Contacter = contacter;
                _user.Contacters.Add(user);
            }
        }

        private void InitUserListBox(SynchronizationContext context, string filterNickName)
        {
            //task线程调用更新主界面
            context.Post((state) =>
            {
                pictureBoxFace.Image = ProfilePhotoPool.GetPhoto(_user.ID);
                labelName.Text = _user.NickName;

                UpdateUserStatus();

                userListBox.OwnerUser = _user;

                List<UserListItem> ulItems = new List<UserListItem>();

                foreach (LanUser user in _user.Contacters)
                {
                    if (user.ID == _user.ID && !LanClientConfig.Instance.SelfVisible)
                    {
                        //忽视自己
                        continue;
                    }
                    if (user.NickName.Contains(filterNickName))
                    {
                        ulItems.Add(new UserListItem(user));
                    }
                }
                userListBox.Items.Clear();
                userListBox.Items.AddRange(ulItems);

                UpdateTaskBarIcon();
            }, null);
        }

        private void _user_FileReceiveError(object sender, FileTransportErrorEventArgs args)
        {
            TransportFile file = args.File;
            LanUser user = _user[args.File.MAC];

            this.userListBox.SetMessageResult(user.ID, _user.ID, file.ID, false);
        }

        private void _user_FileSendError(object sender, FileTransportErrorEventArgs args)
        {
            TransportFile file = args.File;
            this.userListBox.SetMessageResult(_user.ID, file.MAC, file.ID, false);
        }

        private void _user_FileSendCompleted(object sender, FileTransportEventArgs args)
        {
            TransportFile file = args.File;
            this.userListBox.SetFileTransportProgress(file);
        }

        private void _user_FileReceiveCompleted(object sender, FileTransportEventArgs args)
        {
            TransportFile file = args.File;
            this.userListBox.SetFileTransportProgress(file);
        }

        private void _user_FileSendProgressChanged(object sender, FileTransportEventArgs args)
        {
            TransportFile file = args.File;
            this.userListBox.SetFileTransportProgress(file);
        }

        private void _user_FileReceiveProgressChanged(object sender, FileTransportEventArgs args)
        {
            TransportFile file = args.File;
            this.userListBox.SetFileTransportProgress(file);
        }

        private void _user_FileTransportRequested(object sender, FileTransportRequestedEventArgs args)
        {
            TransportFile file = args.File;
            //保存到默认接收文件夹
            string fileName = LanEnv.GetNotExistFileName(LanEnv.ReceivedFilePath, file.File.Name);
            file.SavePath = LanEnv.GetReceivedFilePath(fileName);

            this.userListBox.AddFileReceivingMessage(args.User, file);

            //自动接收文件
            _user.ReceiveFile(file);

            UpdateUnreadMessageUI();
        }

        private void _user_TextMessageReceived(object sender, TextMessageReceivedEventArgs args)
        {
            this.userListBox.AddReceivedTextMessage(args.User, args.ID, args.Message);
            UpdateUnreadMessageUI();
        }

        private void _user_Send(object sender, SendEventArgs args)
        {
            if (args.IsUserPacket)
            {
                //此处需要处理分为txt，image，file传送，其他的发送结果用户应不关心
                this.userListBox.SetMessageResult(args.Packet.FromMAC, args.Packet.ToMAC, args.Packet.ID, args.Success);
            }
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
            if (user.ID == _user.ID && !LanClientConfig.Instance.SelfVisible)
            {
                //自己就忽略掉，本身也不显示在list里面
                return;
            }
            userListBox.AddOrUpdateUser(user, args.UpdateState);
        }

        private void _user_ImageReceived(object sender, ImageReceivedEventArgs args)
        {
            this.userListBox.AddReceivedImageMessage(args.User, args.ID, args.Image, args.FileName);
            UpdateUnreadMessageUI();
        }

        private void UserListBox_SelectionChanged(object sender, System.EventArgs e)
        {
            UserListItem item = this.userListBox.SelectedItem as UserListItem;

            LanUser user = item.User;
            if (user == null)
            {
                return;
            }

            labelUserName.Text = user.NickName;

            UserChatControl ucc = item.ChatControl;
            if (ucc == null)
            {
                ucc = new UserChatControl();
                ucc.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                ucc.User = _user;
                ucc.Contacter = user;
                ucc.SendMessageEnabled = user.Status != UserStatus.Offline;
                ucc.SendMessage += Ucc_SendMessage;
                ucc.InitializeLatastMessage(item.WaitDisplayMessages);
                //显示到界面后，缓存就清理掉
                item.WaitDisplayMessages.Clear();
                this.Controls.Add(ucc);

                item.ChatControl = ucc;
            }
            ucc.Location = new Point(this.searchBox.Right, searchBox.Top + 1);
            ucc.Size = new Size(this.ClientSize.Width - this.searchBox.Right - 1, this.ClientSize.Height - searchBox.Top - 2);
            ucc.BringToFront();
        }

        private void Ucc_SendMessage(object sender, SendMessageEventArgs args)
        {
            this.userListBox.UpdateItemOnSendMessage(args.Message);
        }

        private void pictureBoxFace_Click(object sender, EventArgs e)
        {
            FormConfig fc = new FormConfig();
            fc.User = _user;
            fc.ShowDialog(this);

            //更新头像等一系列
            pictureBoxFace.Image = ProfilePhotoPool.GetPhoto(_user.ID);
            labelName.Text = _user.NickName;

            UpdateTaskBarIcon();
        }

        private void toolStripMenuItemStatus_Click(object sender, EventArgs e)
        {
            if (sender == toolStripMenuItemStatusBusy)
            {
                LanClientConfig.Instance.HideStatus = false;
                _user.Status = UserStatus.Busy;
            }
            else if (sender == toolStripMenuItemStatusOnline)
            {
                LanClientConfig.Instance.HideStatus = false;
                _user.Status = UserStatus.Online;
            }
            else if (sender == this.toolStripMenuItemStatusHide)
            {
                LanClientConfig.Instance.HideStatus = true;
                _user.Status = UserStatus.Offline;
            }
            else
            {
                return;
            }

            _user.UpdateMyStateByBroadcast(UpdateState.Status);

            UpdateUserStatus();
        }

        private void UpdateUserStatus()
        {
            if (_user.Status ==  UserStatus.Busy)
            {
                this.labelStatus.Image = Properties.Resources.leaf_red;
            }
            else if(_user.Status ==  UserStatus.Online)
            {
                this.labelStatus.Image = Properties.Resources.leaf_green;
            }
            else
            {
                LanClientConfig.Instance.HideStatus = true;
                _user.Status = UserStatus.Offline;
                this.labelStatus.Image = Properties.Resources.leaf_gray;
            }
           
            this.pictureBoxFace.Refresh();
        }

        private void labelStatus_Clicked(object sender, EventArgs e)
        {
            contextMenuStripStatus.Show(this.labelStatus, new Point(0, this.labelStatus.Height));
        }

        private void contextMenuStripStatus_Opening(object sender, CancelEventArgs e)
        {
            //更新下菜单的Check状态
            foreach (ToolStripItem item in contextMenuStripStatus.Items)
            {
                if (item is ToolStripMenuItem)
                {
                    ToolStripMenuItem menu = item as ToolStripMenuItem;
                    menu.Checked = false;
                }
            }

            switch (_user.Status)
            {
                case UserStatus.Online: toolStripMenuItemStatusOnline.Checked = true; break;
                case UserStatus.Busy: toolStripMenuItemStatusBusy.Checked = true; break;
                default:
                    if (LanClientConfig.Instance.HideStatus)
                    {
                        toolStripMenuItemStatusHide.Checked = true;
                    }
                    else
                    {
                        toolStripMenuItemStatusOnline.Checked = true; break;
                    }
                    break;
            }
        }

        private void searchBox_SearchTextChanged(object sender, SearchEventArgs e)
        {
            SynchronizationContext context = SynchronizationContext.Current;
            InitUserListBox(context, e.Text);
        }

        private void FormLanIM_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO 保存一些必要信息
            //检测 还有缓存中正在接受的文件之类，发送消息未收到回复的标记失败等

            //登出
            _user.Exit();

            //退出时，把托盘清理一下
            this.notifyIcon.Visible = false;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.notifyIcon.Icon = _trayBlinkIconFlg ? this.Icon : Properties.Resources.tray_trans;
            _trayBlinkIconFlg = !_trayBlinkIconFlg;
        }

        /// <summary>
        /// 未读消息时，界面表示的变化
        /// </summary>
        private void UpdateUnreadMessageUI()
        {
            int count = this.userListBox.UnreadMessageCount;

            //托盘 闪烁
            if (count != 0 || !this.IsActived)
            {
                timer.Start();

                WndsApi.FlashWindow(this.Handle, false);
            }
            else
            {
                timer.Stop();
                this.notifyIcon.Icon = this.Icon;
            }

            //任务栏 暂时默默的无动作，以后考虑更换图标
            this.Icon = LanImage.CreateNumberIcon(this.Icon, count, this.Font);
        }

        private void userListBox_ItemClicked(object sender, ItemClickedEventArgs args)
        {
            //点击头像代表已经读了吧
            UserListItem userItem = args.Item as UserListItem;
            userItem.UnreadMessageCount = 0;

            UpdateUnreadMessageUI();
        }

        private void FormLanIM_Activated(object sender, EventArgs e)
        {
            //窗口激活后就不进行闪烁了
            timer.Stop();
            this.notifyIcon.Icon = this.Icon;
        }

        private void UpdateTaskBarIcon()
        {
            Icon icon = LanImage.ImageToIcon(ProfilePhotoPool.GetPhoto(_user.ID));
            this.Icon = icon;
            this.notifyIcon.Icon = icon;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoggerFactory.Flush();
        }
    }
}
