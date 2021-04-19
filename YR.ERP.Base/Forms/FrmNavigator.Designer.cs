namespace YR.ERP.Base.Forms
{
    partial class FrmNavigator
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
            this.ultraLiveTileView1 = new Infragistics.Win.UltraWinLiveTileView.UltraLiveTileView();
            this.btnChangeGroup = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.ultraLiveTileView1)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLiveTileView1
            // 
            this.ultraLiveTileView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraLiveTileView1.Location = new System.Drawing.Point(0, 0);
            this.ultraLiveTileView1.Name = "ultraLiveTileView1";
            this.ultraLiveTileView1.Size = new System.Drawing.Size(886, 598);
            this.ultraLiveTileView1.TabIndex = 0;
            this.ultraLiveTileView1.Text = "ultraLiveTileView1";
            this.ultraLiveTileView1.UseAppStyling = false;
            // 
            // btnChangeGroup
            // 
            this.btnChangeGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeGroup.Location = new System.Drawing.Point(833, 0);
            this.btnChangeGroup.Name = "btnChangeGroup";
            this.btnChangeGroup.Size = new System.Drawing.Size(53, 46);
            this.btnChangeGroup.TabIndex = 1;
            this.btnChangeGroup.Text = "依公司分類";
            this.btnChangeGroup.Click += new System.EventHandler(this.btnChangeGroup_Click);
            // 
            // FrmNavigator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 598);
            this.Controls.Add(this.btnChangeGroup);
            this.Controls.Add(this.ultraLiveTileView1);
            this.Name = "FrmNavigator";
            this.Text = "FrmNavigateor";
            this.Load += new System.EventHandler(this.FrmNavigateor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraLiveTileView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinLiveTileView.UltraLiveTileView ultraLiveTileView1;
        private Infragistics.Win.Misc.UltraButton btnChangeGroup;
    }
}