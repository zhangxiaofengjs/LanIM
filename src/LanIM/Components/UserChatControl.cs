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

        private void SendFile_Click(object sender, EventArgs e)
        {
            MessageBox.Show((sender as CommonToolBarItem).Name + "clicked!!!!");
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
                    Image smallImg = img.GetThumbnailImage(MessageListBox.PICTURE_THUMBNAIL_WIDTH, MessageListBox.PICTURE_THUMBNAIL_WIDTH, null, IntPtr.Zero);
                    AddImageMessage(this.User, this.Contacter, smallImg, fileName);
                }
            }
        }
        
        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Control)
            {
                User.SendTextMessage(Contacter, textBoxInput.Text);

                AddTextMessage(this.User, this.Contacter, textBoxInput.Text);

                textBoxInput.Text = "";
                e.Handled = true;
            }
        }

        public void AddTextMessage(string message)
        {
            AddTextMessage(this.Contacter, this.User, message);
        }

        public void AddImageMessage(Image img)
        {
            AddImageMessage(this.Contacter, this.User, img, string.Empty);
        }

        private void AddTextMessage(LanUser from, LanUser to, string message)
        {
            Store.Models.Message m = new Store.Models.Message(MessageType.Text);
            m.FromUserId = from.ID;
            m.ToUserId = to.ID;
            m.Content = message;

            _messageHistoryMapper.Add(m);

            MessageListItem item = new MessageListItem();
            item.Message = m;
            item.User = from;

            messageListBox.Items.Add(item);
        }

        private void AddImageMessage(LanUser from, LanUser to, Image img, string originPath)
        {
            //保存记录
            Store.Models.ImageMessage m = new Store.Models.ImageMessage(img);
            m.FromUserId = from.ID;
            m.ToUserId = to.ID;
            m.OriginPath = originPath;

            _messageHistoryMapper.Add(m);

            MessageListItem item = new MessageListItem();
            item.Message = m;
            item.User = from;

            messageListBox.Items.Add(item);
        }
    }
}
