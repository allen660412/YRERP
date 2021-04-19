namespace YR.ERP.Forms.Csp
{
    partial class FrmCspb200
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
            this.ucx_delete_yn = new YR.Util.Controls.UcCheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ute_jja04 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.PnlFillMaster.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ute_jja04)).BeginInit();
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
            this.ultraGroupBox1.Controls.Add(this.ucx_delete_yn);
            this.ultraGroupBox1.Controls.Add(this.label1);
            this.ultraGroupBox1.Controls.Add(this.ute_jja04);
            this.ultraGroupBox1.Location = new System.Drawing.Point(29, 26);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(583, 112);
            this.ultraGroupBox1.TabIndex = 12;
            this.ultraGroupBox1.Text = "Range";
            // 
            // ucx_delete_yn
            // 
            this.ucx_delete_yn.AutoSize = true;
            this.ucx_delete_yn.Font = new System.Drawing.Font("新細明體", 10F);
            this.ucx_delete_yn.Location = new System.Drawing.Point(18, 72);
            this.ucx_delete_yn.Name = "ucx_delete_yn";
            this.ucx_delete_yn.NullValue = "";
            this.ucx_delete_yn.Size = new System.Drawing.Size(80, 18);
            this.ucx_delete_yn.TabIndex = 177;
            this.ucx_delete_yn.Tag = "delete_yn";
            this.ucx_delete_yn.Text = "delete_yn";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 10F);
            this.label1.Location = new System.Drawing.Point(15, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 14);
            this.label1.TabIndex = 2;
            this.label1.Tag = "jja04";
            this.label1.Text = "jja04";
            // 
            // ute_jja04
            // 
            this.ute_jja04.AccessibleDescription = "";
            this.ute_jja04.Font = new System.Drawing.Font("新細明體", 10F);
            this.ute_jja04.Location = new System.Drawing.Point(115, 27);
            this.ute_jja04.Name = "ute_jja04";
            this.ute_jja04.Size = new System.Drawing.Size(334, 22);
            this.ute_jja04.TabIndex = 0;
            this.ute_jja04.Tag = "jja04";
            // 
            // FrmCspb200
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 503);
            this.Name = "FrmCspb200";
            this.Text = "FrmCspb200";
            this.PnlFillMaster.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ute_jja04)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private System.Windows.Forms.Label label1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ute_jja04;
        private Util.Controls.UcCheckBox ucx_delete_yn;
    }
}