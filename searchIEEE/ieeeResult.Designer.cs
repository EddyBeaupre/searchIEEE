namespace searchIEEE
{
    partial class ieeeResult
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
            this.ouiDatabaseView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.ouiDatabaseView)).BeginInit();
            this.SuspendLayout();
            // 
            // ouiDatabaseView
            // 
            this.ouiDatabaseView.AllowUserToAddRows = false;
            this.ouiDatabaseView.AllowUserToDeleteRows = false;
            this.ouiDatabaseView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ouiDatabaseView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ouiDatabaseView.Location = new System.Drawing.Point(0, 0);
            this.ouiDatabaseView.Name = "ouiDatabaseView";
            this.ouiDatabaseView.Size = new System.Drawing.Size(624, 361);
            this.ouiDatabaseView.TabIndex = 1;
            // 
            // ieeeResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 361);
            this.Controls.Add(this.ouiDatabaseView);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ieeeResult";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ieeeResult";
            this.Shown += new System.EventHandler(this.ieeeResult_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.ouiDatabaseView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView ouiDatabaseView;
    }
}