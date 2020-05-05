namespace Com.LanIM
{
    partial class FormLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogin));
            this.pictureBox = new Com.LanIM.UI.ProfilePhotoPictureBox();
            this.labelLogin = new System.Windows.Forms.Label();
            this.linkLabelMAC = new System.Windows.Forms.LinkLabel();
            this.contextMenuStripMAC = new Com.LanIM.Components.NCIContextMenuStrip(this.components);
            this.labelNIC = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Image = global::Com.LanIM.Properties.Resources.logo;
            this.pictureBox.Location = new System.Drawing.Point(73, 102);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(120, 120);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // labelLogin
            // 
            this.labelLogin.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLogin.Location = new System.Drawing.Point(2, 225);
            this.labelLogin.Name = "labelLogin";
            this.labelLogin.Size = new System.Drawing.Size(258, 34);
            this.labelLogin.TabIndex = 2;
            this.labelLogin.Text = "User";
            this.labelLogin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelLogin.MouseEnter += new System.EventHandler(this.labelLogin_MouseEnter);
            this.labelLogin.MouseLeave += new System.EventHandler(this.labelLogin_MouseLeave);
            // 
            // linkLabelMAC
            // 
            this.linkLabelMAC.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F);
            this.linkLabelMAC.Image = global::Com.LanIM.Properties.Resources.wifi;
            this.linkLabelMAC.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabelMAC.LinkColor = System.Drawing.Color.Black;
            this.linkLabelMAC.Location = new System.Drawing.Point(4, 356);
            this.linkLabelMAC.Name = "linkLabelMAC";
            this.linkLabelMAC.Size = new System.Drawing.Size(20, 20);
            this.linkLabelMAC.TabIndex = 3;
            this.linkLabelMAC.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabelMAC.Click += new System.EventHandler(this.linkLabelMAC_Clicked);
            // 
            // contextMenuStripMAC
            // 
            this.contextMenuStripMAC.Name = "contextMenuStripMAC";
            this.contextMenuStripMAC.Size = new System.Drawing.Size(61, 4);
            // 
            // labelNIC
            // 
            this.labelNIC.Location = new System.Drawing.Point(22, 355);
            this.labelNIC.Name = "labelNIC";
            this.labelNIC.Size = new System.Drawing.Size(226, 24);
            this.labelNIC.TabIndex = 4;
            this.labelNIC.Text = "选择网卡";
            this.labelNIC.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 386);
            this.Controls.Add(this.labelNIC);
            this.Controls.Add(this.linkLabelMAC);
            this.Controls.Add(this.labelLogin);
            this.Controls.Add(this.pictureBox);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(262, 386);
            this.MinimumSize = new System.Drawing.Size(262, 386);
            this.Name = "FormLogin";
            this.Padding = new System.Windows.Forms.Padding(7, 35, 7, 7);
            this.Text = "LanIM";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private UI.ProfilePhotoPictureBox pictureBox;
        private System.Windows.Forms.Label labelLogin;
        private System.Windows.Forms.LinkLabel linkLabelMAC;
        private Components.NCIContextMenuStrip contextMenuStripMAC;
        private System.Windows.Forms.Label labelNIC;
    }
}