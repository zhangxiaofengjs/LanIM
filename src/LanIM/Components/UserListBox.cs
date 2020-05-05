using Com.LanIM.Common;
using Com.LanIM.Network;
using Com.LanIM.Store;
using Com.LanIM.Store.Models;
using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Com.LanIM.Components.MessageListItem;

namespace Com.LanIM.Components
{
    class UserListBox : ScrollableList
    {
        private static readonly int ICON_WIDTH = 36;
        private static readonly int MARGIN = 5;
        private static readonly Font MSG_COUNT_FONT = new Font("arial", 8, FontStyle.Bold);
        private static readonly int MSG_COUNT_DIAM = 16;

        public LanUser OwnerUser { get; set; }

        public LanUser SelectedUser
        {
            get
            {
                UserListItem item = this.SelectedItem as UserListItem;
                if(item == null)
                {
                    return null;
                }

                return item.User;
            }
        }

        public int UnreadMessageCount
        {
            get
            {
                int count = 0;
                foreach(UserListItem item in this.Items)
                {
                    count += item.UnreadMessageCount;
                }
                return count;
            }
        }
        public UserChatControl SelectedChatControl
        {
            get
            {
                UserListItem item = this.SelectedItem as UserListItem;
                if (item == null)
                {
                    return null;
                }

                return item.ChatControl;
            }
        }

        public UserListBox()
        {
        }

        public void Add(LanUser user)
        {
            UserListItem item = new UserListItem();
            item.User = user;
            this.Items.Add(item);
        }

        public UserListItem this[string userId]
        {
            get
            {
                foreach (UserListItem item in this.Items)
                {
                    if (item.User.ID == userId)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public bool ContainsUser(LanUser user)
        {
            return this[user.ID] != null;
        }

        public void AddOrUpdateUser(LanUser user, UpdateState updateState)
        {
            if (this.ContainsUser(user))
            {
                UserListItem item = this[user.ID];

                if ((updateState & UpdateState.NickName) != 0 ||
                    (updateState & UpdateState.IP) != 0 ||
                    (updateState & UpdateState.Port) != 0)
                {
                    item.Update();
                }

                this.Invalidate(item.Bounds);

                UserChatControl chatControl = item.ChatControl;
                if (chatControl != null)
                {
                    chatControl.Contacter = user;
                    chatControl.SendMessageEnabled = (user.Status != UserStatus.Offline);
                }

                if(this.SelectedUser != null && this.SelectedUser.ID == user.ID)
                {
                    //当前刚好表示着聊天记录则更新
                    if (chatControl != null)
                    {
                        chatControl.RefreshMessageList(null);
                    }
                }
            }
            else
            {
                UserListItem item = new UserListItem(user);
                item.Save();

                this.Items.Add(item);
            }
        }

        protected override void OnDrawItem(UI.DrawItemEventArgs args)
        {
            args.DrawBackground();
            UserListItem userListItem = args.Item as UserListItem;

            Graphics g = args.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            Rectangle rect = userListItem.Bounds;

            //头像
            int x = rect.X + MARGIN;
            int y = rect.Y + (rect.Height - ICON_WIDTH) / 2;
            g.DrawImage(ProfilePhotoPool.GetPhoto(userListItem.User.ID), x, y, ICON_WIDTH, ICON_WIDTH);

            //在线状态
            Brush brush;
            switch(userListItem.User.Status)
            {
                case UserStatus.Online: brush = BrushPool.GetBrush(Color.Green);break;
                case UserStatus.Busy: brush = BrushPool.GetBrush(Color.Crimson); break;
                default: brush = BrushPool.GetBrush(Color.Gray); break;
            }
            g.FillEllipse(brush, x + ICON_WIDTH - 8, y + ICON_WIDTH - 8, 10, 10);

            g.DrawEllipse(Pens.White, x+ICON_WIDTH-8, y + ICON_WIDTH - 8, 11, 11);

            //昵称
            x += ICON_WIDTH + MARGIN;

            Rectangle commandRect = new Rectangle(new Point(x, y), new Size(rect.Width - ICON_WIDTH - 3 * MARGIN, rect.Height - MARGIN));
            TextRenderer.DrawText(g, userListItem.User.NickName,
                args.Font, commandRect, Color.FromArgb(0, 0, 0), TextFormatFlags.Left|TextFormatFlags.Top);

            //IP
            string ip = "";
            if ( userListItem.User.Address != null )
            {
                ip = userListItem.User.Address.ToString();
            }
            TextRenderer.DrawText(g, ip,
                args.Font, commandRect, Color.FromArgb(150, 150, 150), TextFormatFlags.Right|TextFormatFlags.Top);

            //字体位置
            if (userListItem.UnreadMessageCount != 0)
            {
                Rectangle rectMsgCnt = new Rectangle(rect.Right - MSG_COUNT_DIAM - 10, rect.Bottom - MSG_COUNT_DIAM - 12, MSG_COUNT_DIAM, MSG_COUNT_DIAM);
                g.DrawEllipse(Pens.DarkGray, rectMsgCnt);
                g.FillEllipse(Brushes.DarkGray, rectMsgCnt);
                int number = userListItem.UnreadMessageCount;
                rectMsgCnt.X += number > 99 ? 1 : 0;//字体位置微调
                rectMsgCnt.Y += number > 99 ? -2 : 1;
                g.DrawString(number > 99 ? "..." : number.ToString(), MSG_COUNT_FONT, Brushes.White, rectMsgCnt, new StringFormat()
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                });
            }
        }

        public void AddReceivedTextMessage(LanUser from, long id, string message)
        {
            Store.Models.Message m = new Store.Models.Message(MessageType.Text);
            m.FromUserId = from.ID;
            m.ToUserId = this.OwnerUser.ID;
            m.Content = message;
            m.Flag = true; //默认成功，后面按照失败结果设定为false

            MessageListItem item = new MessageListItem();
            item.State = MessageState.Received;
            item.ID = id;
            item.Message = m;
            item.User = from;

            item.Save();

            AddMessageListItem(from, item);
        }

        private void AddMessageListItem(LanUser from, MessageListItem item)
        {
            UserListItem userItem = this[from.ID];

            bool bRefresh = false;
            if (this.SelectedUser == null || from.ID != this.SelectedUser.ID)
            {
                //不是当前用户则增加未读数
                userItem.UnreadMessageCount++;
                bRefresh = true;
            }

            //移动到第一个
            if (this.Items[0] != userItem)
            {
                this.Items.Remove(userItem);
                this.Items.Insert(0, userItem);
                bRefresh = true;
            }

            if(bRefresh)
            {
                this.Invalidate(userItem.Bounds);
            }

            UserChatControl chatCtrl = userItem.ChatControl;
            if (chatCtrl == null)
            {
                //如果还没表示则先缓存一下
                userItem.WaitDisplayMessages.Add(item);
            }
            else
            {
                chatCtrl.AddMessageItem(item, userItem.UserID == from.ID);
            }
        }

        public void AddReceivedImageMessage(LanUser from, long id, Image image, string fileName)
        {
            //保存记录
            Store.Models.ImageMessage m = new Store.Models.ImageMessage(image);
            m.FromUserId = from.ID;
            m.ToUserId = this.OwnerUser.ID;
            m.OriginPath = "";
            m.FileName = Path.GetFileName(fileName);
            m.Flag = true; //默认成功，后面按照失败结果设定为false

            MessageListItem item = new MessageListItem();
            item.ID = id;
            item.State = MessageState.Received;
            item.Message = m;
            item.User = from;

            item.Save();

            AddMessageListItem(from, item);
        }

        public void AddFileReceivingMessage(LanUser from, TransportFile file)
        {
            //保存记录
            Store.Models.FileMessage m = new Store.Models.FileMessage();
            m.FromUserId = from.ID;
            m.ToUserId = this.OwnerUser.ID;
            m.OriginFilePath = file.SavePath;
            m.FileName = Path.GetFileName(file.SavePath);
            m.FileLength = file.File.Length;
            m.Flag = true; //默认成功，后面按照失败结果设定为false

            MessageListItem item = new MessageListItem();
            item.ID = file.ID;
            item.Message = m;
            item.User = from;
            item.State = MessageState.Receiving;

            //保存到数据库
            item.Save();

            AddMessageListItem(from, item);
        }

        public void SetFileTransportProgress(TransportFile file)
        {
            UserListItem userItem = this[file.MAC];
            UserChatControl chatCtrl = userItem.ChatControl;

            MessageListItem item = null;
            if (chatCtrl == null)
            {
                //如果还没表示则取出来
                item = userItem.GetWaitDisplayMessageItem(file.ID);
            }
            else
            {
                item = chatCtrl.GetMessageItem(file.ID);
            }

            item.FileTransportedLength = file.TransportedLength;
            item.FileTransportedSpeed = file.TransportedSpeed;
            item.Progress = file.Progress;

            if (file.Progress == 100)
            {
                //传输完毕
                Store.Models.Message m = item.Message;
                m.Flag = true;
                item.Update();

                item.State = MessageState.Received;
            }

            if (chatCtrl != null && file.MAC == this.SelectedUser.MAC)
            {
                //当前表示着，更新
                chatCtrl.RefreshMessageList(item);
            }
        }

        public void SetMessageResult(string fromUserId, string toUserId, long id, bool success)
        {
            string userItemId = fromUserId;
            if (fromUserId == this.OwnerUser.ID)
            {
                //如果是我自己，那么则是发送消息
                if (success)
                {
                    //发送时就按照默认成功的，发送成功的消息可以忽略了
                    return;
                }
                userItemId = toUserId;
            }

            UserListItem userItem = this[userItemId];
            UserChatControl chatCtrl = userItem.ChatControl;

            MessageListItem item = null;
            if (chatCtrl == null)
            {
                //如果还没表示则取出来
                item = userItem.GetWaitDisplayMessageItem(id);
            }
            else
            {
                item = chatCtrl.GetMessageItem(id);
            }

            Store.Models.Message m = item.Message;
            m.Flag = success;
            item.Update();

            if (chatCtrl != null && userItemId == this.SelectedUser.ID)
            {
                //当前表示着，更新
                chatCtrl.Invalidate();
            }
        }

        internal void UpdateItemOnSendMessage(Store.Models.Message message)
        {
            UserListItem userItem = this[message.ToUserId];

            //移动到第一个
            if (this.Items[0] != userItem)
            {
                this.Items.Remove(userItem);
                this.Items.Insert(0, userItem);
            }
        }
    }
}
