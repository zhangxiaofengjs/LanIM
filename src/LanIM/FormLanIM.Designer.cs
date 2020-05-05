namespace Com.LanIM
{
    partial class FormLanIM
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLanIM));
            this.contextMenuStripStatus = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemStatusOnline = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStatusBusy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemStatusHide = new System.Windows.Forms.ToolStripMenuItem();
            this.userListBox = new Com.LanIM.Components.UserListBox();
            this.searchBox = new Com.LanIM.UI.SearchBox();
            this.labelUserName = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.pictureBoxFace = new Com.LanIM.UI.ProfilePhotoPictureBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.labelServerMode = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.contextMenuStripStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFace)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripStatus
            // 
            this.contextMenuStripStatus.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F);
            this.contextMenuStripStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemStatusOnline,
            this.toolStripMenuItemStatusBusy,
            this.toolStripSeparator1,
            this.toolStripMenuItemStatusHide});
            this.contextMenuStripStatus.Name = "contextMenuStrip1";
            this.contextMenuStripStatus.ShowImageMargin = false;
            this.contextMenuStripStatus.ShowItemToolTips = false;
            this.contextMenuStripStatus.Size = new System.Drawing.Size(104, 76);
            this.contextMenuStripStatus.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripStatus_Opening);
            // 
            // toolStripMenuItemStatusOnline
            // 
            this.toolStripMenuItemStatusOnline.CheckOnClick = true;
            this.toolStripMenuItemStatusOnline.Name = "toolStripMenuItemStatusOnline";
            this.toolStripMenuItemStatusOnline.Size = new System.Drawing.Size(103, 22);
            this.toolStripMenuItemStatusOnline.Text = "在线";
            this.toolStripMenuItemStatusOnline.Click += new System.EventHandler(this.toolStripMenuItemStatus_Click);
            // 
            // toolStripMenuItemStatusBusy
            // 
            this.toolStripMenuItemStatusBusy.CheckOnClick = true;
            this.toolStripMenuItemStatusBusy.Name = "toolStripMenuItemStatusBusy";
            this.toolStripMenuItemStatusBusy.Size = new System.Drawing.Size(103, 22);
            this.toolStripMenuItemStatusBusy.Text = "忙碌...";
            this.toolStripMenuItemStatusBusy.Click += new System.EventHandler(this.toolStripMenuItemStatus_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(100, 6);
            // 
            // toolStripMenuItemStatusHide
            // 
            this.toolStripMenuItemStatusHide.CheckOnClick = true;
            this.toolStripMenuItemStatusHide.Name = "toolStripMenuItemStatusHide";
            this.toolStripMenuItemStatusHide.Size = new System.Drawing.Size(103, 22);
            this.toolStripMenuItemStatusHide.Text = "隐藏状态";
            this.toolStripMenuItemStatusHide.Click += new System.EventHandler(this.toolStripMenuItemStatus_Click);
            // 
            // userListBox
            // 
            this.userListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.userListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.userListBox.Borders = Com.LanIM.UI.Borders.None;
            this.userListBox.HighlightWithNoFocus = false;
            this.userListBox.Location = new System.Drawing.Point(1, 117);
            this.userListBox.Margin = new System.Windows.Forms.Padding(4);
            this.userListBox.MultipleSelect = false;
            this.userListBox.Name = "userListBox";
            this.userListBox.OwnerUser = null;
            this.userListBox.Size = new System.Drawing.Size(287, 534);
            this.userListBox.TabIndex = 4;
            this.userListBox.ToggleSelection = false;
            this.userListBox.ItemClicked += new Com.LanIM.UI.ItemClickedEventHandler(this.userListBox_ItemClicked);
            this.userListBox.SelectionChanged += new System.EventHandler(this.UserListBox_SelectionChanged);
            // 
            // searchBox
            // 
            this.searchBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.searchBox.Location = new System.Drawing.Point(1, 75);
            this.searchBox.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(287, 45);
            this.searchBox.TabIndex = 5;
            this.searchBox.SearchTextChanged += new Com.LanIM.UI.SearchEventHandler(this.searchBox_SearchTextChanged);
            // 
            // labelUserName
            // 
            this.labelUserName.AutoSize = true;
            this.labelUserName.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelUserName.Location = new System.Drawing.Point(290, 46);
            this.labelUserName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(144, 24);
            this.labelUserName.TabIndex = 2;
            this.labelUserName.Text = "欢迎光临LanIM";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(72, 32);
            this.labelName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(41, 17);
            this.labelName.TabIndex = 2;
            this.labelName.Text = "lanim";
            // 
            // pictureBoxFace
            // 
            this.pictureBoxFace.Image = global::Com.LanIM.Properties.Resources.logo;
            this.pictureBoxFace.Location = new System.Drawing.Point(8, 6);
            this.pictureBoxFace.Name = "pictureBoxFace";
            this.pictureBoxFace.Size = new System.Drawing.Size(64, 64);
            this.pictureBoxFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxFace.TabIndex = 9;
            this.pictureBoxFace.TabStop = false;
            this.pictureBoxFace.Click += new System.EventHandler(this.pictureBoxFace_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipTitle = "LanIM";
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "LanIM, connect the friendship!";
            this.notifyIcon.Visible = true;
            // 
            // timer
            // 
            this.timer.Interval = 800;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(692, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "FlushLog";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelServerMode
            // 
            this.labelServerMode.Image = global::Com.LanIM.Properties.Resources.server;
            this.labelServerMode.Location = new System.Drawing.Point(19, 1);
            this.labelServerMode.Margin = new System.Windows.Forms.Padding(1);
            this.labelServerMode.Name = "labelServerMode";
            this.labelServerMode.Size = new System.Drawing.Size(16, 16);
            this.labelServerMode.TabIndex = 2;
            // 
            // labelStatus
            // 
            this.labelStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelStatus.Image = global::Com.LanIM.Properties.Resources.leaf_green;
            this.labelStatus.Location = new System.Drawing.Point(1, 1);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(1);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(16, 16);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Click += new System.EventHandler(this.labelStatus_Clicked);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.labelStatus);
            this.flowLayoutPanel1.Controls.Add(this.labelServerMode);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(75, 50);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(211, 20);
            this.flowLayoutPanel1.TabIndex = 11;
            // 
            // FormLanIM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1067, 652);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.userListBox);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.labelUserName);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.pictureBoxFace);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormLanIM";
            this.Padding = new System.Windows.Forms.Padding(7, 35, 7, 7);
            this.Text = "FormLanIM";
            this.Activated += new System.EventHandler(this.FormLanIM_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLanIM_FormClosing);
            this.Load += new System.EventHandler(this.FormLanIM_Load);
            this.contextMenuStripStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFace)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Components.UserListBox userListBox;
        private UI.SearchBox searchBox;
        private System.Windows.Forms.Label labelUserName;
        private UI.ProfilePhotoPictureBox pictureBoxFace;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripStatus;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStatusOnline;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStatusHide;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStatusBusy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelServerMode;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}