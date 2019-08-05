namespace Com.LanIM.Components
{
    partial class UserChatControl
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.messageListBox = new Com.LanIM.Components.MessageListBox();
            this.contextMenuStripMessage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.commonToolBar = new Com.LanIM.UI.CommonToolBar();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStripMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F);
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.messageListBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.commonToolBar);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxInput);
            this.splitContainer1.Size = new System.Drawing.Size(777, 576);
            this.splitContainer1.SplitterDistance = 427;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 9;
            // 
            // messageListBox
            // 
            this.messageListBox.Borders = ((Com.LanIM.UI.Borders)((Com.LanIM.UI.Borders.Top | Com.LanIM.UI.Borders.Bottom)));
            this.messageListBox.ContextMenuStrip = this.contextMenuStripMessage;
            this.messageListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageListBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageListBox.HighlightWithNoFocus = false;
            this.messageListBox.Location = new System.Drawing.Point(0, 0);
            this.messageListBox.Margin = new System.Windows.Forms.Padding(4);
            this.messageListBox.MultipleSelect = false;
            this.messageListBox.Name = "messageListBox";
            this.messageListBox.Size = new System.Drawing.Size(777, 427);
            this.messageListBox.TabIndex = 3;
            this.messageListBox.ToggleSelection = false;
            this.messageListBox.ScrolledTop += new System.EventHandler(this.MessageListBox_ScrolledTop);
            this.messageListBox.ScrolledBottom += new System.EventHandler(this.MessageListBox_ScrolledBottom);
            // 
            // contextMenuStripMessage
            // 
            this.contextMenuStripMessage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCopy,
            this.toolStripMenuItemOpenFolder,
            this.toolStripMenuItemSaveAs});
            this.contextMenuStripMessage.Name = "contextMenuStrip1";
            this.contextMenuStripMessage.ShowImageMargin = false;
            this.contextMenuStripMessage.ShowItemToolTips = false;
            this.contextMenuStripMessage.Size = new System.Drawing.Size(124, 70);
            this.contextMenuStripMessage.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripMessage_Opening);
            // 
            // toolStripMenuItemCopy
            // 
            this.toolStripMenuItemCopy.Name = "toolStripMenuItemCopy";
            this.toolStripMenuItemCopy.Size = new System.Drawing.Size(123, 22);
            this.toolStripMenuItemCopy.Text = "复制";
            this.toolStripMenuItemCopy.Click += new System.EventHandler(this.toolStripMenuItemMessage_Click);
            // 
            // toolStripMenuItemOpenFolder
            // 
            this.toolStripMenuItemOpenFolder.Name = "toolStripMenuItemOpenFolder";
            this.toolStripMenuItemOpenFolder.Size = new System.Drawing.Size(123, 22);
            this.toolStripMenuItemOpenFolder.Text = "打开文件夹...";
            this.toolStripMenuItemOpenFolder.Click += new System.EventHandler(this.toolStripMenuItemMessage_Click);
            // 
            // toolStripMenuItemSaveAs
            // 
            this.toolStripMenuItemSaveAs.Name = "toolStripMenuItemSaveAs";
            this.toolStripMenuItemSaveAs.Size = new System.Drawing.Size(123, 22);
            this.toolStripMenuItemSaveAs.Text = "另存为...";
            this.toolStripMenuItemSaveAs.Click += new System.EventHandler(this.toolStripMenuItemMessage_Click);
            // 
            // commonToolBar
            // 
            this.commonToolBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commonToolBar.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.commonToolBar.Location = new System.Drawing.Point(0, 0);
            this.commonToolBar.Name = "commonToolBar";
            this.commonToolBar.Padding = new System.Windows.Forms.Padding(10, 5, 5, 5);
            this.commonToolBar.Size = new System.Drawing.Size(776, 30);
            this.commonToolBar.TabIndex = 7;
            // 
            // textBoxInput
            // 
            this.textBoxInput.AcceptsReturn = true;
            this.textBoxInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInput.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.textBoxInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxInput.Location = new System.Drawing.Point(0, 30);
            this.textBoxInput.MaxLength = 2048;
            this.textBoxInput.Multiline = true;
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(776, 139);
            this.textBoxInput.TabIndex = 6;
            this.textBoxInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxInput_KeyDown);
            this.textBoxInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInput_KeyPress);
            // 
            // UserChatControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UserChatControl";
            this.Size = new System.Drawing.Size(777, 576);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStripMessage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private MessageListBox messageListBox;
        private UI.CommonToolBar commonToolBar;
        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMessage;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpenFolder;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSaveAs;
    }
}
