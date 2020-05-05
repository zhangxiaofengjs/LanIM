namespace Com.LanIM.UI
{
    partial class SearchBox
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
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // filterTextBox
            // 
            this.filterTextBox.BackColor = System.Drawing.Color.White;
            this.filterTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.filterTextBox.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filterTextBox.Location = new System.Drawing.Point(36, 12);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(162, 16);
            this.filterTextBox.TabIndex = 0;
            this.filterTextBox.FontChanged += new System.EventHandler(this.filterTextBox_FontChanged);
            this.filterTextBox.TextChanged += new System.EventHandler(this.filterTextBox_TextChanged);
            // 
            // SearchBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.filterTextBox);
            this.Name = "SearchBox";
            this.Size = new System.Drawing.Size(237, 50);
            this.SizeChanged += new System.EventHandler(this.SearchBox_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SearchBox_Paint);
            this.Resize += new System.EventHandler(this.SearchBox_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox filterTextBox;
    }
}
