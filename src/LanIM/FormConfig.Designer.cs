namespace Com.LanIM
{
    partial class FormConfig
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
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelIP = new System.Windows.Forms.Label();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.pictureBoxFace = new Com.LanIM.UI.ProfilePhotoPictureBox();
            this.labelSysPhoto = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxBroadcastAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxNIC = new System.Windows.Forms.TextBox();
            this.contextMenuStripMAC = new Com.LanIM.Components.NCIContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFace)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(26, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "昵称";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.DimGray;
            this.label3.Location = new System.Drawing.Point(27, 68);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "网卡";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.DimGray;
            this.label5.Location = new System.Drawing.Point(27, 96);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 17);
            this.label5.TabIndex = 11;
            this.label5.Text = "地址";
            // 
            // labelIP
            // 
            this.labelIP.AutoSize = true;
            this.labelIP.ForeColor = System.Drawing.Color.DimGray;
            this.labelIP.Location = new System.Drawing.Point(63, 96);
            this.labelIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelIP.Name = "labelIP";
            this.labelIP.Size = new System.Drawing.Size(113, 17);
            this.labelIP.TabIndex = 11;
            this.labelIP.Text = "255.255.255.255";
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.BackColor = System.Drawing.Color.White;
            this.textBoxUserName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxUserName.ForeColor = System.Drawing.Color.DimGray;
            this.textBoxUserName.Location = new System.Drawing.Point(66, 39);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(129, 17);
            this.textBoxUserName.TabIndex = 12;
            this.textBoxUserName.Text = "LAN IM";
            // 
            // pictureBoxFace
            // 
            this.pictureBoxFace.Image = global::Com.LanIM.Properties.Resources.logo;
            this.pictureBoxFace.Location = new System.Drawing.Point(208, 38);
            this.pictureBoxFace.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxFace.Name = "pictureBoxFace";
            this.pictureBoxFace.Size = new System.Drawing.Size(90, 90);
            this.pictureBoxFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxFace.TabIndex = 10;
            this.pictureBoxFace.TabStop = false;
            this.pictureBoxFace.Click += new System.EventHandler(this.pictureBoxFace_Click);
            // 
            // labelSysPhoto
            // 
            this.labelSysPhoto.BackColor = System.Drawing.Color.Transparent;
            this.labelSysPhoto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelSysPhoto.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSysPhoto.ForeColor = System.Drawing.Color.DimGray;
            this.labelSysPhoto.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.labelSysPhoto.LinkColor = System.Drawing.Color.DimGray;
            this.labelSysPhoto.Location = new System.Drawing.Point(208, 130);
            this.labelSysPhoto.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSysPhoto.Name = "labelSysPhoto";
            this.labelSysPhoto.Size = new System.Drawing.Size(90, 20);
            this.labelSysPhoto.TabIndex = 11;
            this.labelSysPhoto.TabStop = true;
            this.labelSysPhoto.Text = "选择系统头像";
            this.labelSysPhoto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelSysPhoto.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelSysPhoto.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelSysPhoto_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(27, 124);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 17);
            this.label2.TabIndex = 11;
            this.label2.Text = "网段";
            // 
            // textBoxBroadcastAddress
            // 
            this.textBoxBroadcastAddress.BackColor = System.Drawing.Color.White;
            this.textBoxBroadcastAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxBroadcastAddress.ForeColor = System.Drawing.Color.DimGray;
            this.textBoxBroadcastAddress.Location = new System.Drawing.Point(66, 124);
            this.textBoxBroadcastAddress.Name = "textBoxBroadcastAddress";
            this.textBoxBroadcastAddress.Size = new System.Drawing.Size(129, 17);
            this.textBoxBroadcastAddress.TabIndex = 12;
            this.textBoxBroadcastAddress.Text = "255.255.255.255";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.OrangeRed;
            this.label4.Location = new System.Drawing.Point(27, 154);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(161, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "※设定网段后需要重启LanIM";
            // 
            // textBoxNIC
            // 
            this.textBoxNIC.BackColor = System.Drawing.Color.White;
            this.textBoxNIC.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxNIC.ForeColor = System.Drawing.Color.DimGray;
            this.textBoxNIC.Location = new System.Drawing.Point(66, 68);
            this.textBoxNIC.Name = "textBoxNIC";
            this.textBoxNIC.ReadOnly = true;
            this.textBoxNIC.Size = new System.Drawing.Size(129, 17);
            this.textBoxNIC.TabIndex = 12;
            this.textBoxNIC.Text = "选择网卡";
            this.textBoxNIC.Click += new System.EventHandler(this.textBoxNIC_Click);
            // 
            // contextMenuStripMAC
            // 
            this.contextMenuStripMAC.Name = "contextMenuStripMAC";
            this.contextMenuStripMAC.Size = new System.Drawing.Size(61, 4);
            // 
            // FormConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 192);
            this.Controls.Add(this.textBoxNIC);
            this.Controls.Add(this.textBoxUserName);
            this.Controls.Add(this.textBoxBroadcastAddress);
            this.Controls.Add(this.labelSysPhoto);
            this.Controls.Add(this.labelIP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxFace);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(320, 320);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(320, 180);
            this.Name = "FormConfig";
            this.Padding = new System.Windows.Forms.Padding(7, 35, 7, 7);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormConfig";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormConfig_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFace)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UI.ProfilePhotoPictureBox pictureBoxFace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelIP;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.LinkLabel labelSysPhoto;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxBroadcastAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxNIC;
        private Components.NCIContextMenuStrip contextMenuStripMAC;
    }
}