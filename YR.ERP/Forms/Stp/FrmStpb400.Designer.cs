namespace YR.ERP.Forms.Stp
{
    partial class FrmStpb400
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
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ute_sga01 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.PnlFillMaster.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ute_sga01)).BeginInit();
            this.SuspendLayout();
            // 
            // PnlFillMaster
            // 
            this.PnlFillMaster.Controls.Add(this.ultraGroupBox1);
            this.PnlFillMaster.Location = new System.Drawing.Point(1, 31);
            this.PnlFillMaster.Size = new System.Drawing.Size(748, 454);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.label1);
            this.ultraGroupBox1.Controls.Add(this.ute_sga01);
            this.ultraGroupBox1.Location = new System.Drawing.Point(43, 25);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(583, 112);
            this.ultraGroupBox1.TabIndex = 13;
            this.ultraGroupBox1.Text = "Range";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 10F);
            this.label1.Location = new System.Drawing.Point(15, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 14);
            this.label1.TabIndex = 2;
            this.label1.Tag = "sga01";
            this.label1.Text = "sga01";
            // 
            // ute_sga01
            // 
            this.ute_sga01.AccessibleDescription = "";
            this.ute_sga01.Font = new System.Drawing.Font("新細明體", 10F);
            this.ute_sga01.Location = new System.Drawing.Point(115, 27);
            this.ute_sga01.Name = "ute_sga01";
            this.ute_sga01.Size = new System.Drawing.Size(334, 22);
            this.ute_sga01.TabIndex = 0;
            this.ute_sga01.Tag = "sga01";
            // 
            // FrmStpb400
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 503);
            this.Name = "FrmStpb400";
            this.Text = "FrmStpb400";
            this.PnlFillMaster.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ute_sga01)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private System.Windows.Forms.Label label1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ute_sga01;
    }
}