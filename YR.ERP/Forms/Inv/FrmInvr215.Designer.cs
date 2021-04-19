namespace YR.ERP.Forms.Inv
{
    partial class FrmInvr215
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
            this.label7 = new System.Windows.Forms.Label();
            this.ute_icc01 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label2 = new System.Windows.Forms.Label();
            this.ute_icc02 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucx_show_warehouse_yn = new YR.Util.Controls.UcCheckBox();
            this.PnlFillMaster.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ute_icc01)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ute_icc02)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // PnlFillMaster
            // 
            this.PnlFillMaster.Controls.Add(this.ultraGroupBox2);
            this.PnlFillMaster.Controls.Add(this.ultraGroupBox1);
            this.PnlFillMaster.Location = new System.Drawing.Point(1, 31);
            this.PnlFillMaster.Size = new System.Drawing.Size(802, 500);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.label7);
            this.ultraGroupBox1.Controls.Add(this.ute_icc01);
            this.ultraGroupBox1.Controls.Add(this.label2);
            this.ultraGroupBox1.Controls.Add(this.ute_icc02);
            this.ultraGroupBox1.Location = new System.Drawing.Point(37, 44);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(465, 135);
            this.ultraGroupBox1.TabIndex = 15;
            this.ultraGroupBox1.Text = "Range";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("新細明體", 10F);
            this.label7.Location = new System.Drawing.Point(17, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 14);
            this.label7.TabIndex = 8;
            this.label7.Tag = "icc01";
            this.label7.Text = "icc01";
            // 
            // ute_icc01
            // 
            this.ute_icc01.AccessibleDescription = "";
            this.ute_icc01.Font = new System.Drawing.Font("新細明體", 10F);
            this.ute_icc01.Location = new System.Drawing.Point(116, 25);
            this.ute_icc01.Name = "ute_icc01";
            this.ute_icc01.Size = new System.Drawing.Size(334, 22);
            this.ute_icc01.TabIndex = 0;
            this.ute_icc01.Tag = "icc01";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 10F);
            this.label2.Location = new System.Drawing.Point(17, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 14);
            this.label2.TabIndex = 4;
            this.label2.Tag = "icc02";
            this.label2.Text = "icc02";
            // 
            // ute_icc02
            // 
            this.ute_icc02.AccessibleDescription = "";
            this.ute_icc02.Font = new System.Drawing.Font("新細明體", 10F);
            this.ute_icc02.Location = new System.Drawing.Point(116, 60);
            this.ute_icc02.Name = "ute_icc02";
            this.ute_icc02.Size = new System.Drawing.Size(334, 22);
            this.ute_icc02.TabIndex = 1;
            this.ute_icc02.Tag = "icc02";
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.ucx_show_warehouse_yn);
            this.ultraGroupBox2.Location = new System.Drawing.Point(37, 194);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(465, 84);
            this.ultraGroupBox2.TabIndex = 16;
            this.ultraGroupBox2.Text = "Single";
            // 
            // ucx_show_warehouse_yn
            // 
            this.ucx_show_warehouse_yn.AutoSize = true;
            this.ucx_show_warehouse_yn.Font = new System.Drawing.Font("新細明體", 10F);
            this.ucx_show_warehouse_yn.Location = new System.Drawing.Point(20, 34);
            this.ucx_show_warehouse_yn.Name = "ucx_show_warehouse_yn";
            this.ucx_show_warehouse_yn.NullValue = "";
            this.ucx_show_warehouse_yn.Size = new System.Drawing.Size(75, 18);
            this.ucx_show_warehouse_yn.TabIndex = 0;
            this.ucx_show_warehouse_yn.Tag = "show_warehouse_yn";
            this.ucx_show_warehouse_yn.Text = "jump_yn";
            // 
            // FrmInvr215
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 549);
            this.Name = "FrmInvr215";
            this.Text = "FrmInvr215";
            this.PnlFillMaster.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ute_icc01)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ute_icc02)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private System.Windows.Forms.Label label7;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ute_icc01;
        private System.Windows.Forms.Label label2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ute_icc02;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Util.Controls.UcCheckBox ucx_show_warehouse_yn;
    }
}