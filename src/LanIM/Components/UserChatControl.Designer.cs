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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.messageListBox = new Com.LanIM.Components.MessageListBox();
            this.commonToolBar = new Com.LanIM.UI.CommonToolBar();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.messageListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageListBox.HighlightWithNoFocus = false;
            this.messageListBox.Location = new System.Drawing.Point(0, 0);
            this.messageListBox.Margin = new System.Windows.Forms.Padding(4);
            this.messageListBox.MultipleSelect = false;
            this.messageListBox.Name = "messageListBox";
            this.messageListBox.Size = new System.Drawing.Size(777, 427);
            this.messageListBox.TabIndex = 3;
            this.messageListBox.ScrolledTop += MessageListBox_ScrolledTop;
            this.messageListBox.ScrolledBottom += MessageListBox_ScrolledBottom;
            // 
            // commonToolBar
            // 
            this.commonToolBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commonToolBar.BackColor = System.Drawing.Color.White;
            this.commonToolBar.Location = new System.Drawing.Point(0, 0);
            this.commonToolBar.Name = "commonToolBar";
            this.commonToolBar.Padding = new System.Windows.Forms.Padding(10, 5, 5, 5);
            this.commonToolBar.Size = new System.Drawing.Size(776, 30);
            this.commonToolBar.TabIndex = 7;
            // 
            // textBoxInput
            // 
            this.textBoxInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInput.BackColor = System.Drawing.Color.White;
            this.textBoxInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxInput.Location = new System.Drawing.Point(0, 30);
            this.textBoxInput.Multiline = true;
            this.textBoxInput.MaxLength = 2048;
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(776, 127);
            this.textBoxInput.TabIndex = 6;
            this.textBoxInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxInput_KeyDown);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private MessageListBox messageListBox;
        private UI.CommonToolBar commonToolBar;
        private System.Windows.Forms.TextBox textBoxInput;
    }
}
