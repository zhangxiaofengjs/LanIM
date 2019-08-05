namespace Com.LanIM.Components
{
    partial class UserProfileControl
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
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBoxFace = new Com.LanIM.UI.ProfilePhotoPictureBox();
            this.labelNickName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelMAC = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxMemo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFace)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxIP
            // 
            this.textBoxIP.BackColor = System.Drawing.Color.WhiteSmoke;
            this.textBoxIP.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxIP.ForeColor = System.Drawing.Color.Black;
            this.textBoxIP.Location = new System.Drawing.Point(56, 90);
            this.textBoxIP.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.ReadOnly = true;
            this.textBoxIP.Size = new System.Drawing.Size(158, 17);
            this.textBoxIP.TabIndex = 23;
            this.textBoxIP.Text = "XXXXX";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.DimGray;
            this.label5.Location = new System.Drawing.Point(21, 90);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 17);
            this.label5.TabIndex = 17;
            this.label5.Text = "IP";
            // 
            // pictureBoxFace
            // 
            this.pictureBoxFace.DrawUserStatus = false;
            this.pictureBoxFace.Image = global::Com.LanIM.Properties.Resources.logo;
            this.pictureBoxFace.Location = new System.Drawing.Point(218, 32);
            this.pictureBoxFace.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.pictureBoxFace.Name = "pictureBoxFace";
            this.pictureBoxFace.Size = new System.Drawing.Size(120, 120);
            this.pictureBoxFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxFace.TabIndex = 13;
            this.pictureBoxFace.TabStop = false;
            this.pictureBoxFace.UserStatus = Com.LanIM.Common.UserStatus.Online;
            // 
            // labelNickName
            // 
            this.labelNickName.AutoSize = true;
            this.labelNickName.Font = new System.Drawing.Font("Microsoft YaHei UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNickName.ForeColor = System.Drawing.Color.Black;
            this.labelNickName.Location = new System.Drawing.Point(17, 33);
            this.labelNickName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelNickName.Name = "labelNickName";
            this.labelNickName.Size = new System.Drawing.Size(190, 26);
            this.labelNickName.TabIndex = 17;
            this.labelNickName.Text = "昵称NNNNNNNN";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(21, 65);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "ID";
            // 
            // labelMAC
            // 
            this.labelMAC.AutoSize = true;
            this.labelMAC.ForeColor = System.Drawing.Color.Black;
            this.labelMAC.Location = new System.Drawing.Point(53, 65);
            this.labelMAC.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelMAC.Name = "labelMAC";
            this.labelMAC.Size = new System.Drawing.Size(129, 17);
            this.labelMAC.TabIndex = 17;
            this.labelMAC.Text = "NNNNNNNNNNN";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.DimGray;
            this.label3.Location = new System.Drawing.Point(21, 115);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 17);
            this.label3.TabIndex = 17;
            this.label3.Text = "备注";
            // 
            // textBoxMemo
            // 
            this.textBoxMemo.BackColor = System.Drawing.Color.White;
            this.textBoxMemo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxMemo.ForeColor = System.Drawing.Color.Black;
            this.textBoxMemo.Location = new System.Drawing.Point(56, 115);
            this.textBoxMemo.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxMemo.Name = "textBoxMemo";
            this.textBoxMemo.Size = new System.Drawing.Size(137, 17);
            this.textBoxMemo.TabIndex = 23;
            this.textBoxMemo.Text = "XXXXX";
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Image = global::Com.LanIM.Properties.Resources.refresh;
            this.label2.Location = new System.Drawing.Point(194, 113);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 20);
            this.label2.TabIndex = 17;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // UserProfileControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.textBoxMemo);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.labelNickName);
            this.Controls.Add(this.labelMAC);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pictureBoxFace);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "UserProfileControl";
            this.Size = new System.Drawing.Size(360, 182);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFace)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Label label5;
        private UI.ProfilePhotoPictureBox pictureBoxFace;
        private System.Windows.Forms.Label labelNickName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelMAC;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxMemo;
        private System.Windows.Forms.Label label2;
    }
}