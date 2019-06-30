using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Com.LanIM.Components;
using Com.LanIM.Store.Models;
using Com.LanIM.Store;
using Com.LanIM.UI;
using Com.LanIM.Common;
using System.IO;
using System.Drawing.Imaging;
using Message = Com.LanIM.Store.Models.Message;
using static Com.LanIM.Components.MessageListItem;

namespace Com.LanIM.Components
{
    public partial class UserChatControl : UserControl
    {
        private MessageHistoryMapper _messageHistoryMapper = new MessageHistoryMapper();

        public UserChatControl()
        {
            InitializeComponent();

            SetupToolBar();
        }

        private void SetupToolBar()
        {
            //工具栏
            CommonToolBarItem item = new CommonToolBarItem();
            item.Name = "sendImageCmnToolBarItem";
            item.Image = Properties.Resources.picture;
            item.ImageFocus = Properties.Resources.picture_focus;
            item.Click += SendImage_Click;
            this.commonToolBar.Items.Add(item);

            item = new CommonToolBarItem();
            item.Name = "sendFileCmnToolBarItem";
            item.Image = Properties.Resources.folder;
            item.ImageFocus = Properties.Resources.folder_focus;
            item.Click += SendFile_Click;
            this.commonToolBar.Items.Add(item);

            item = new CommonToolBarItem();
            item.Name = "chatHistoryCmnToolBarItem";
            item.Image = Properties.Resources.chat_history;
            item.ImageFocus = Properties.Resources.chat_history_focus;
            item.Click += SendImage_Click;
            this.commonToolBar.Items.Add(item);
        }

        internal LanUser User { get; set; }
        internal LanUser Contacter { get; set; }

        internal void InitializeLatastMessage()
        {
            long id = -1;
            MessageListItem item = null;
            if (messageListBox.Items.Count != 0)
            {
                item = messageListBox.Items[0] as MessageListItem;
                id = item.Message.ID;
            }

            List<Message> list = _messageHistoryMapper.QueryUserLatestMessages(this.Contacter.ID, id);

            List<MessageListItem> items = new List<MessageListItem>();
            foreach (Message m in list)
            {
                MessageListItem i = new MessageListItem();
                i.Message = m;
                i.User = m.FromUserId == User.ID ? User : Contacter;

                items.Add(i);
            }
            messageListBox.Items.InsertRange(0, items);

            if (items.Count != 0)
            {
                //向上滚动取有记录的话，选中添加的最后1个
                messageListBox.TopItem = items[items.Count - 1];
            }
        }

        private void SendFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "所有文件|*.*";
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    string fileName = ofd.FileName;
                    User.SendFile(Contacter, fileName);

                    //保存发送记录
                    AddFileMessage(this.User, this.Contacter, fileName);
                }
            }
        }

        private void SendImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "图像文件|*.png;*.jpg;*.bmp;";
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    string fileName = ofd.FileName;
                    User.SendImage(Contacter, fileName);

                    //保存发送记录，只保存缩略图，原图的Path也保存
                    Image img = Image.FromFile(fileName);
                    int w = (int)(1.0 * img.Width * MessageListBox.PICTURE_THUMBNAIL_HEIGHT / img.Height);
                    Image smallImg = img.GetThumbnailImage(w, MessageListBox.PICTURE_THUMBNAIL_HEIGHT, null, IntPtr.Zero);
                    AddImageMessage(this.User, this.Contacter, smallImg, fileName);
                }
            }
        }
        
        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Control)
            {
                long id = User.SendTextMessage(Contacter, textBoxInput.Text);

                AddTextMessage(this.User, this.Contacter, id, textBoxInput.Text, MessageState.Sending);

                textBoxInput.Text = "";
                e.Handled = true;
            }
        }

        public void AddTextMessage(long id, string message)
        {
            AddTextMessage(this.Contacter, this.User, id, message, MessageState.Received);
        }

        public void AddImageMessage(Image img)
        {
            AddImageMessage(this.Contacter, this.User, img, string.Empty);
        }

        public void AddFileMessage(string filePath)
        {
            AddFileMessage(this.Contacter, this.User, filePath);
        }

        private void AddTextMessage(LanUser from, LanUser to, long id, string message, MessageState state)
        {
            Store.Models.Message m = new Store.Models.Message(MessageType.Text);
            m.FromUserId = from.ID;
            m.ToUserId = to.ID;
            m.Content = message;
            m.Flag = true; //默认成功，后面按照失败结果设定为false

            _messageHistoryMapper.Add(m);

            MessageListItem item = new MessageListItem();
            item.State = state;
            item.ID = id;
            item.Message = m;
            item.User = from;

            messageListBox.Items.Add(item);
            messageListBox.ScrollToBottom();
        }

        private void AddImageMessage(LanUser from, LanUser to, Image img, string originPath)
        {
            //保存记录
            Store.Models.ImageMessage m = new Store.Models.ImageMessage(img);
            m.FromUserId = from.ID;
            m.ToUserId = to.ID;
            m.OriginPath = originPath;
            m.Flag = true; //默认成功，后面按照失败结果设定为false

            _messageHistoryMapper.Add(m);

            MessageListItem item = new MessageListItem();
            item.Message = m;
            item.User = from;

            messageListBox.Items.Add(item);
            messageListBox.ScrollToBottom();
        }

        private void AddFileMessage(LanUser from, LanUser to, string filePath)
        {
            //保存记录
            Store.Models.ImageMessage m = new Store.Models.ImageMessage(Image.FromFile(filePath));
            m.FromUserId = from.ID;
            m.ToUserId = to.ID;
            m.Content = filePath;
            m.Flag = true; //默认成功，后面按照失败结果设定为false

            _messageHistoryMapper.Add(m);

            MessageListItem item = new MessageListItem();
            item.Message = m;
            item.User = from;

            messageListBox.Items.Add(item);
            messageListBox.ScrollToBottom();
        }

        public void SetMessageSendResult(long id, bool success)
        {
            if(success)
            {
                //发送时就按照默认成功的，发送成功的消息可以忽略了
                return;
            }

            //因为后后加的，倒过来循环速度更快应该
            for (int i = messageListBox.Items.Count - 1; i > -1; i--)
            {
                MessageListItem item  = messageListBox.Items[i] as MessageListItem;

                if (item.ID == id)
                {
                    Message m = item.Message;
                    m.Flag = success;
                    _messageHistoryMapper.UpdateState(m);
                    break;
                }
            }
        }

        private void MessageListBox_ScrolledTop(object sender, System.EventArgs e)
        {
            //滚动到最上面，继续取靠近的消息
            InitializeLatastMessage();
        }

        private void MessageListBox_ScrolledBottom(object sender, System.EventArgs e)
        {
            //TODO 考虑效率问题，当前滚动部分加载固定的条数，通过滚动到最上最下来调整，暂时不对应等有效率问题再说吧
        }
    }
}
