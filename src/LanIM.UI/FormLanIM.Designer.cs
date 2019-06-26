namespace Com.LanIM.UI
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
            this.labelStatus = new System.Windows.Forms.Label();
            this.userListBox = new Com.LanIM.UI.Components.UserListBox();
            this.searchBox = new Com.LanIM.UI.Components.SearchBox();
            this.labelUserName = new System.Windows.Forms.Label();
            this.pictureBoxFace = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFace)).BeginInit();
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(79, 53);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(34, 17);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "在线";
            // 
            // userListBox
            // 
            this.userListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.userListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.userListBox.Borders = Com.LanIM.UI.Components.Borders.None;
            this.userListBox.HighlightWithNoFocus = false;
            this.userListBox.ToggleSelection= false;
            this.userListBox.Location = new System.Drawing.Point(1, 134);
            this.userListBox.Margin = new System.Windows.Forms.Padding(4);
            this.userListBox.MultipleSelect = false;
            this.userListBox.Name = "userListBox";
            this.userListBox.Size = new System.Drawing.Size(287, 517);
            this.userListBox.TabIndex = 4;
            this.userListBox.SelectionChanged += new System.EventHandler(this.UserListBox_SelectionChanged);
            // 
            // searchBox
            // 
            this.searchBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.searchBox.Location = new System.Drawing.Point(1, 75);
            this.searchBox.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(287, 51);
            this.searchBox.TabIndex = 5;
            // 
            // labelUserName
            // 
            this.labelUserName.AutoSize = true;
            this.labelUserName.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelUserName.Location = new System.Drawing.Point(290, 46);
            this.labelUserName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(200, 24);
            this.labelUserName.TabIndex = 2;
            this.labelUserName.Text = "上海海隆软件有限公司";
            // 
            // pictureBoxFace
            // 
            this.pictureBoxFace.Image = global::Com.LanIM.UI.Properties.Resources.logo;
            this.pictureBoxFace.Location = new System.Drawing.Point(8, 6);
            this.pictureBoxFace.Name = "pictureBoxFace";
            this.pictureBoxFace.Size = new System.Drawing.Size(64, 64);
            this.pictureBoxFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxFace.TabIndex = 9;
            this.pictureBoxFace.TabStop = false;
            this.pictureBoxFace.MouseEnter += new System.EventHandler(this.pictureBoxFace_MouseEnter);
            this.pictureBoxFace.MouseLeave += new System.EventHandler(this.pictureBoxFace_MouseLeave);
            // 
            // FormLanIM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1067, 652);
            this.Controls.Add(this.pictureBoxFace);
            this.Controls.Add(this.userListBox);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.labelUserName);
            this.Controls.Add(this.labelStatus);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormLanIM";
            this.Padding = new System.Windows.Forms.Padding(7, 35, 7, 7);
            this.Text = "FormLanIM";
            this.Load += new System.EventHandler(this.FormLanIM_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFace)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelStatus;
        private Components.UserListBox userListBox;
        private Components.SearchBox searchBox;
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.PictureBox pictureBoxFace;
    }
}