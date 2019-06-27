namespace Com.LanIM
{
    partial class FormIPMsgTest
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button6 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonLanIMLogout = new System.Windows.Forms.Button();
            this.buttonLanIMLogin = new System.Windows.Forms.Button();
            this.buttonSendFile = new System.Windows.Forms.Button();
            this.buttonSendPic = new System.Windows.Forms.Button();
            this.buttonLanIMSendMsg = new System.Windows.Forms.Button();
            this.comboBoxUsers = new System.Windows.Forms.ComboBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textBoxNN = new System.Windows.Forms.TextBox();
            this.buttonLogFlush = new System.Windows.Forms.Button();
            this.scrollableUserControl1 = new Com.LanIM.UI.ScrollableList();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 18);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(144, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "上线测试";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(30, 10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(144, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "取得本地IP以及广播";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(6, 48);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(144, 23);
            this.button3.TabIndex = 0;
            this.button3.Text = "下线测试";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.ForeColor = System.Drawing.Color.Blue;
            this.groupBox1.Location = new System.Drawing.Point(16, 209);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(162, 110);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IPMsg测试区";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(6, 77);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(144, 23);
            this.button6.TabIndex = 0;
            this.button6.Text = "发送消息测试";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(202, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.richTextBox1.ShowSelectionMargin = true;
            this.richTextBox1.Size = new System.Drawing.Size(466, 572);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonLanIMLogout);
            this.groupBox2.Controls.Add(this.buttonLanIMLogin);
            this.groupBox2.Controls.Add(this.buttonSendFile);
            this.groupBox2.Controls.Add(this.buttonSendPic);
            this.groupBox2.Controls.Add(this.buttonLanIMSendMsg);
            this.groupBox2.ForeColor = System.Drawing.Color.DeepPink;
            this.groupBox2.Location = new System.Drawing.Point(16, 325);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(162, 165);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "LanIM测试区";
            // 
            // buttonLanIMLogout
            // 
            this.buttonLanIMLogout.Location = new System.Drawing.Point(6, 45);
            this.buttonLanIMLogout.Name = "buttonLanIMLogout";
            this.buttonLanIMLogout.Size = new System.Drawing.Size(144, 23);
            this.buttonLanIMLogout.TabIndex = 0;
            this.buttonLanIMLogout.Text = "下线测试";
            this.buttonLanIMLogout.UseVisualStyleBackColor = true;
            this.buttonLanIMLogout.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonLanIMLogin
            // 
            this.buttonLanIMLogin.Location = new System.Drawing.Point(6, 18);
            this.buttonLanIMLogin.Name = "buttonLanIMLogin";
            this.buttonLanIMLogin.Size = new System.Drawing.Size(144, 23);
            this.buttonLanIMLogin.TabIndex = 0;
            this.buttonLanIMLogin.Text = "上线测试";
            this.buttonLanIMLogin.UseVisualStyleBackColor = true;
            this.buttonLanIMLogin.Click += new System.EventHandler(this.buttonEntry_Click);
            // 
            // buttonSendFile
            // 
            this.buttonSendFile.Location = new System.Drawing.Point(6, 132);
            this.buttonSendFile.Name = "buttonSendFile";
            this.buttonSendFile.Size = new System.Drawing.Size(144, 23);
            this.buttonSendFile.TabIndex = 0;
            this.buttonSendFile.Text = "发送文件测试";
            this.buttonSendFile.UseVisualStyleBackColor = true;
            this.buttonSendFile.Click += new System.EventHandler(this.buttonSendFile_Click);
            // 
            // buttonSendPic
            // 
            this.buttonSendPic.Location = new System.Drawing.Point(6, 103);
            this.buttonSendPic.Name = "buttonSendPic";
            this.buttonSendPic.Size = new System.Drawing.Size(144, 23);
            this.buttonSendPic.TabIndex = 0;
            this.buttonSendPic.Text = "发送图片测试";
            this.buttonSendPic.UseVisualStyleBackColor = true;
            this.buttonSendPic.Click += new System.EventHandler(this.buttonSendPic_Click);
            // 
            // buttonLanIMSendMsg
            // 
            this.buttonLanIMSendMsg.Location = new System.Drawing.Point(6, 74);
            this.buttonLanIMSendMsg.Name = "buttonLanIMSendMsg";
            this.buttonLanIMSendMsg.Size = new System.Drawing.Size(144, 23);
            this.buttonLanIMSendMsg.TabIndex = 0;
            this.buttonLanIMSendMsg.Text = "发送消息测试";
            this.buttonLanIMSendMsg.UseVisualStyleBackColor = true;
            this.buttonLanIMSendMsg.Click += new System.EventHandler(this.buttonSendMsg_Click);
            // 
            // comboBoxUsers
            // 
            this.comboBoxUsers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUsers.FormattingEnabled = true;
            this.comboBoxUsers.Location = new System.Drawing.Point(18, 72);
            this.comboBoxUsers.Name = "comboBoxUsers";
            this.comboBoxUsers.Size = new System.Drawing.Size(144, 20);
            this.comboBoxUsers.TabIndex = 3;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(18, 98);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(144, 19);
            this.textBox2.TabIndex = 2;
            this.textBox2.Text = "测试消息abcあいうえお手帳";
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(93, 18);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(69, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "结束";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(18, 18);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(69, 23);
            this.buttonStart.TabIndex = 2;
            this.buttonStart.Text = "开始";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButton2);
            this.groupBox3.Controls.Add(this.radioButton1);
            this.groupBox3.Controls.Add(this.pictureBox2);
            this.groupBox3.Controls.Add(this.pictureBox1);
            this.groupBox3.Controls.Add(this.textBoxNN);
            this.groupBox3.Controls.Add(this.textBox2);
            this.groupBox3.Controls.Add(this.comboBoxUsers);
            this.groupBox3.Controls.Add(this.buttonStart);
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Controls.Add(this.buttonLogFlush);
            this.groupBox3.Controls.Add(this.buttonClose);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Location = new System.Drawing.Point(12, 39);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(184, 510);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "消息测试";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(93, 158);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(35, 16);
            this.radioButton2.TabIndex = 5;
            this.radioButton2.Text = "小";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(22, 158);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(35, 16);
            this.radioButton1.TabIndex = 5;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "大";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(93, 124);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(69, 50);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(18, 124);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(69, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // textBoxNN
            // 
            this.textBoxNN.Location = new System.Drawing.Point(18, 47);
            this.textBoxNN.Name = "textBoxNN";
            this.textBoxNN.Size = new System.Drawing.Size(144, 19);
            this.textBoxNN.TabIndex = 2;
            this.textBoxNN.Text = "蓝米";
            // 
            // buttonLogFlush
            // 
            this.buttonLogFlush.Location = new System.Drawing.Point(16, 180);
            this.buttonLogFlush.Name = "buttonLogFlush";
            this.buttonLogFlush.Size = new System.Drawing.Size(146, 23);
            this.buttonLogFlush.TabIndex = 1;
            this.buttonLogFlush.Text = "写日志到文件";
            this.buttonLogFlush.UseVisualStyleBackColor = true;
            this.buttonLogFlush.Click += new System.EventHandler(this.buttonLogFlush_Click);
            // 
            // scrollableUserControl1
            // 
            this.scrollableUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollableUserControl1.BackColor = System.Drawing.Color.LavenderBlush;
            this.scrollableUserControl1.Location = new System.Drawing.Point(686, 57);
            this.scrollableUserControl1.MultipleSelect = false;
            this.scrollableUserControl1.Name = "scrollableUserControl1";
            this.scrollableUserControl1.Size = new System.Drawing.Size(285, 346);
            this.scrollableUserControl1.TabIndex = 5;
            // 
            // FormIPMsgTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(998, 590);
            this.Controls.Add(this.scrollableUserControl1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button2);
            this.Name = "FormIPMsgTest";
            this.Text = "FormIPMsgTest";
            this.Load += new System.EventHandler(this.FormIPMsgTest_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonLanIMLogin;
        private System.Windows.Forms.Button buttonLanIMSendMsg;
        private System.Windows.Forms.Button buttonLanIMLogout;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBoxUsers;
        private System.Windows.Forms.TextBox textBoxNN;
        private System.Windows.Forms.Button buttonSendPic;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button buttonSendFile;
        private System.Windows.Forms.Button buttonLogFlush;
        private Com.LanIM.UI.ScrollableList scrollableUserControl1;
    }
}