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
using Com.LanIM.Network;
using System.Collections.Specialized;

namespace Com.LanIM.Components
{
    partial class UserChatControl : UserControl
    {
        public UserChatControl()
        {
            InitializeComponent();

            SetupToolBar();

            contextMenuStripMessage.Renderer = new CommonContextMenuRender();
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
        private bool _sendMessageEnabled = false;
        public bool SendMessageEnabled
        {
            get
            {
                return _sendMessageEnabled;
            }
            set
            {
                _sendMessageEnabled = value;
                if (_sendMessageEnabled)
                {
                    this.textBoxInput.BackColor = Color.White;
                    this.commonToolBar.BackColor = Color.White;
                }
                else
                {
                    this.textBoxInput.BackColor = Color.LightGoldenrodYellow;
                    this.commonToolBar.BackColor = Color.LightGoldenrodYellow;
                }
            }
        }
        public event SendMessageEventHandler SendMessage;

        internal void InitializeLatastMessage(List<MessageListItem> cacheWaitDisplayMessages)
        {
            long id = -1;
            MessageListItem item = null;
            if (messageListBox.Items.Count != 0)
            {
                item = messageListBox.Items[0] as MessageListItem;
                id = item.Message.ID;
            }

            MessageHistoryMapper messageHistoryMapper = new MessageHistoryMapper();
            List<Message> list = messageHistoryMapper.QueryUserLatestMessages(this.Contacter.ID, id);

            List<MessageListItem> items = new List<MessageListItem>();
            foreach (Message m in list)
            {
                //优先追加缓存中的数据
                int index = -1;
                if (cacheWaitDisplayMessages != null)
                {
                    index = cacheWaitDisplayMessages.FindIndex((i) =>
                    {
                        if (i.Message.ID == m.ID)
                        {
                            return true;
                        }
                        return false;
                    }
                    );
                }

                if (index == -1)
                {
                    item = new MessageListItem();
                    item.Message = m;
                    if (m.Flag)
                    {
                        if (m.FromUserId == User.ID)
                        {
                            item.State = MessageState.SendSuccess;
                        }
                        else
                        {
                            item.State = MessageState.Received;
                        }
                    }
                    else
                    {
                        if (m.FromUserId == User.ID)
                        {
                            item.State = MessageState.SendError;
                        }
                        else
                        {
                            item.State = MessageState.ReceiveError;
                        }
                    }
                    item.User = m.FromUserId == User.ID ? User : Contacter;
                }
                else
                {
                    item = cacheWaitDisplayMessages[index];
                    cacheWaitDisplayMessages.RemoveAt(index);
                }
                items.Add(item);
            }

            if (cacheWaitDisplayMessages != null)
            {
                items.AddRange(cacheWaitDisplayMessages);
            }

            messageListBox.Items.InsertRange(0, items);

            if (items.Count != 0)
            {
                //向上滚动取有记录的话，选中添加的最后1个
                messageListBox.ScrollToBottom();
            }
        }

        private void SendFile_Click(object sender, EventArgs e)
        {
            if(!this.SendMessageEnabled)
            {
                return;
            }
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "所有文件|*.*";
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    string fileName = ofd.FileName;
                    long id = User.SendFile(Contacter, fileName);

                    //保存发送记录
                    Store.Models.FileMessage m = new Store.Models.FileMessage();
                    m.FromUserId = this.User.ID;
                    m.ToUserId = this.Contacter.ID;
                    m.OriginFilePath = fileName;
                    m.FileName = Path.GetFileName(fileName);
                    m.FileLength = LanFile.GetFileLength(fileName);
                    m.Flag = true; //默认成功，后面按照失败结果设定为false

                    MessageListItem item = new MessageListItem();
                    item.ID = id;
                    item.Message = m;
                    item.User = this.User;
                    item.State = MessageState.Sending;

                    item.Save();

                    AddMessageItem(item, true);

                    OnSendMessage(m);
                }
            }
        }

        private void SendImage_Click(object sender, EventArgs e)
        {
            if (!this.SendMessageEnabled)
            {
                return;
            }
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "图像文件|*.png;*.jpg;*.bmp;";
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    string fileName = ofd.FileName;
                    Image smallImg = LanImage.GetThumbnailImage(fileName, MessageListBox.PICTURE_THUMBNAIL_HEIGHT);
                    if (smallImg == null)
                    {
                        //可能不是合法的图片
                        return;
                    }

                    long id = User.SendImage(Contacter, fileName);

                    //保存发送记录，只保存缩略图，原图的Path也保存
                    string thPath = LanEnv.GetNotExistFileName(LanEnv.PicturePath, Path.GetFileName(fileName));
                    smallImg.Save(thPath);

                    Store.Models.ImageMessage m = new Store.Models.ImageMessage(smallImg);
                    m.FromUserId = this.User.ID;
                    m.ToUserId = this.Contacter.ID;
                    m.OriginPath = fileName;
                    m.FileName = Path.GetFileName(fileName);
                    m.Flag = true; //默认成功，后面按照失败结果设定为false

                    MessageListItem item = new MessageListItem();
                    item.ID = id;
                    item.State = MessageState.Sending;
                    item.Message = m;
                    item.User = this.User;

                    item.Save();

                    AddMessageItem(item, true);

                    OnSendMessage(m);
                }
            }
        }

        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Control)
            {
                if (!this.SendMessageEnabled)
                {
                    return;
                }
                if(string.IsNullOrEmpty(textBoxInput.Text) ||
                    string.IsNullOrEmpty(textBoxInput.Text.Replace("\r", "").Replace("\n","")))
                {
                    //空或者只有空行不发送
                    return;
                }
                long id = User.SendTextMessage(Contacter, textBoxInput.Text);

                Store.Models.Message m = new Store.Models.Message(MessageType.Text);
                m.FromUserId = this.User.ID;
                m.ToUserId = this.Contacter.ID;
                m.Content = textBoxInput.Text;
                m.Flag = true; //默认成功，后面按照失败结果设定为false

                MessageListItem item = new MessageListItem();
                item.State = MessageState.Sending;
                item.ID = id;
                item.Message = m;
                item.User = this.User;

                item.Save();

                AddMessageItem(item, true);

                textBoxInput.Text = "";
                e.Handled = true;

                OnSendMessage(m);
            }
        }

        private void OnSendMessage(Message m)
        {
            SendMessage?.Invoke(this, new SendMessageEventArgs(m));
        }

        private void textBoxInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                //无视直接回车的输入
                e.Handled = true;
            }
        }

        internal void AddMessageItem(MessageListItem item, bool forceDisplayedBottom)
        {
            messageListBox.Items.Add(item);
            if(forceDisplayedBottom)
            {
                messageListBox.ScrollToBottom();
            }
        }

        internal MessageListItem GetMessageItem(long id)
        {
            //因为后后加的，倒过来循环速度更快应该
            for (int i = messageListBox.Items.Count - 1; i > -1; i--)
            {
                MessageListItem item = messageListBox.Items[i] as MessageListItem;

                if (item.ID == id)
                {
                    return item;
                }
            }

            return null;
        }

        internal void RefreshMessageList(MessageListItem item)
        {
            this.messageListBox.Invalidate(item);
        }

        private void MessageListBox_ScrolledTop(object sender, System.EventArgs e)
        {
            //滚动到最上面，继续取靠近的消息
            InitializeLatastMessage(null);
        }

        private void MessageListBox_ScrolledBottom(object sender, System.EventArgs e)
        {
            //TODO 考虑效率问题，当前滚动部分加载固定的条数，通过滚动到最上最下来调整，暂时不对应等有效率问题再说吧
        }

        private void toolStripMenuItemMessage_Click(object sender, EventArgs e)
        {
            MessageListItem item = contextMenuStripMessage.Tag as MessageListItem;
            if (item == null)
            {
                return;
            }

            Message m = item.Message;

            if (sender == toolStripMenuItemCopy)
            {
                if (m.Type == MessageType.Text)
                {
                    string selection = item.SelectedText;
                    if(!string.IsNullOrEmpty(selection))
                    {
                        Clipboard.SetText(selection);
                    }
                    else
                    {
                        Clipboard.SetText(m.Content);
                    }
                }
                else if (m.Type == MessageType.Image)
                {
                    Clipboard.SetImage((m as ImageMessage).Image);
                }
                else if (m.Type == MessageType.File)
                {
                    string filePath = (m as FileMessage).OriginFilePath;
                    if(File.Exists(filePath))
                    {
                        StringCollection sc = new StringCollection();
                        sc.Add(filePath);
                        Clipboard.SetFileDropList(sc);
                    }
                }
            }
            else if(sender == toolStripMenuItemOpenFolder)
            {
                string filePath;
                if (m.Type == MessageType.Image)
                {
                    filePath = (m as ImageMessage).Path;
                }
                else if (m.Type == MessageType.File)
                {
                    filePath = (m as FileMessage).OriginFilePath;
                }
                else
                {
                    return;
                }
                if (File.Exists(filePath))
                {
                    System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + filePath + "\"");
                }
            }
            else if(sender == toolStripMenuItemSaveAs)
            {
                string filePath;
                if (m.Type == MessageType.Image)
                {
                    filePath = (m as ImageMessage).Path;
                }
                else if (m.Type == MessageType.File)
                {
                    filePath = (m as FileMessage).OriginFilePath;
                }
                else
                {
                    return;
                }

                if (!File.Exists(filePath))
                {
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "所有文件(*.*)|*.*";
                    sfd.RestoreDirectory = true;
                    sfd.FileName = Path.GetFileName(filePath);
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(filePath, sfd.FileName, true);
                    }
                }
            }
        }

        private void contextMenuStripMessage_Opening(object sender, CancelEventArgs e)
        {
            MessageListItem item = messageListBox.GetItemAtPosition(messageListBox.PointToClient(MousePosition)) as MessageListItem;
            if(item == null)
            {
                e.Cancel = true;
                return;
            }
            contextMenuStripMessage.Tag = item;

            contextMenuStripMessage.Items.Clear();
            contextMenuStripMessage.Items.Add(toolStripMenuItemCopy);
            if (item.Message.Type != MessageType.Text)
            {
                contextMenuStripMessage.Items.Add(new ToolStripSeparator());
                contextMenuStripMessage.Items.Add(toolStripMenuItemOpenFolder);
                contextMenuStripMessage.Items.Add(toolStripMenuItemSaveAs);
            }
        }
    }
}
