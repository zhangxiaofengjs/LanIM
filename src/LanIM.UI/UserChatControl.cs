using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Com.LanIM.UI.Components;
using Com.LanIM.Store.Models;
using Com.LanIM.Store;

namespace Com.LanIM.UI
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
            item.Click += SendFile_Click;
            this.commonToolBar.Items.Add(item);
        }

        internal LanUser User { get; set; }
        internal LanUser Contacter { get; set; }

        private void SendFile_Click(object sender, EventArgs e)
        {
            MessageBox.Show((sender as CommonToolBarItem).Name + "clicked!!!!");
        }

        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Control)
            {
                User.SendTextMessage(Contacter, textBoxInput.Text);

                AddMessage(this.User, this.Contacter, textBoxInput.Text);

                textBoxInput.Text = "";
                e.Handled = true;
            }
        }

        public void AddMessage(string message)
        {
            AddMessage(this.Contacter, this.User, message);
        }

        private void AddMessage(LanUser from, LanUser to, string message)
        {
            Store.Models.Message m = new Store.Models.Message(MessageType.Text);
            m.FromUserId = from.ID;
            m.ToUserId = to.ID;
            m.Content = message;
            m.Time = DateTime.Now;

            _messageHistoryMapper.Add(m);

            MessageListItem item = new MessageListItem();
            item.Message = m;
            item.User = from;

            messageListBox.Items.Add(item);
        }
    }
}
