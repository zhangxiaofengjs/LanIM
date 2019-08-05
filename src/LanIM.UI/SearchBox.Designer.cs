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
            this.filterTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.filterTextBox.Location = new System.Drawing.Point(14, 3);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(145, 12);
            this.filterTextBox.TabIndex = 0;
            this.filterTextBox.TextChanged += new System.EventHandler(this.filterTextBox_TextChanged);
            // 
            // SearchBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.filterTextBox);
            this.Name = "SearchBox";
            this.Size = new System.Drawing.Size(220, 36);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox filterTextBox;
    }
}
