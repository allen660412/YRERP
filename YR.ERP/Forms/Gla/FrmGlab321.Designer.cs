namespace YR.ERP.Forms.Gla
{
    partial class FrmGlab321
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
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ute_gfa08 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label3 = new System.Windows.Forms.Label();
            this.PnlFillMaster.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ute_gfa08)).BeginInit();
            this.SuspendLayout();
            // 
            // PnlFillMaster
            // 
            this.PnlFillMaster.Controls.Add(this.ultraGroupBox2);
            this.PnlFillMaster.Location = new System.Drawing.Point(1, 31);
            this.PnlFillMaster.Size = new System.Drawing.Size(748, 454);
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.ute_gfa08);
            this.ultraGroupBox2.Controls.Add(this.label3);
            this.ultraGroupBox2.Location = new System.Drawing.Point(28, 28);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(577, 87);
            this.ultraGroupBox2.TabIndex = 2;
            this.ultraGroupBox2.Text = "Single";
            // 
            // ute_gfa08
            // 
            this.ute_gfa08.AccessibleDescription = "";
            this.ute_gfa08.Font = new System.Drawing.Font("新細明體", 10F);
            this.ute_gfa08.Location = new System.Drawing.Point(108, 27);
            this.ute_gfa08.Name = "ute_gfa08";
            this.ute_gfa08.Size = new System.Drawing.Size(104, 22);
            this.ute_gfa08.TabIndex = 3;
            this.ute_gfa08.Tag = "gfa08";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("新細明體", 10F);
            this.label3.Location = new System.Drawing.Point(15, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 14);
            this.label3.TabIndex = 2;
            this.label3.Tag = "gfa08";
            this.label3.Text = "gfa08";
            // 
            // FrmGlab321
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 503);
            this.Name = "FrmGlab321";
            this.Text = "FrmGlab321";
            this.PnlFillMaster.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ute_gfa08)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ute_gfa08;
        private System.Windows.Forms.Label label3;
    }
}