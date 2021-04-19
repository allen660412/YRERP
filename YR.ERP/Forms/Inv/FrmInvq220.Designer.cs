namespace YR.ERP.Forms.Inv
{
    partial class FrmInvq220
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_sum_in10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.uGridMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTab_Master)).BeginInit();
            this.uTab_Master.SuspendLayout();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UtbmMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uGridMaster
            // 
            this.uGridMaster.DisplayLayout.ForceSerialization = true;
            this.uGridMaster.Size = new System.Drawing.Size(0, 0);
            // 
            // uTab_Master
            // 
            this.uTab_Master.Location = new System.Drawing.Point(8, 54);
            this.uTab_Master.Size = new System.Drawing.Size(914, 494);
            this.uTab_Master.TabPageMargins.ForceSerialization = true;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Size = new System.Drawing.Size(910, 464);
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Size = new System.Drawing.Size(910, 464);
            // 
            // ultraTabPageControl4
            // 
            this.ultraTabPageControl4.Size = new System.Drawing.Size(910, 464);
            // 
            // ultraTabPageControl5
            // 
            this.ultraTabPageControl5.Size = new System.Drawing.Size(910, 464);
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.panel1);
            this.ultraTabPageControl1.Controls.Add(this.uGridMaster);
            this.ultraTabPageControl1.Size = new System.Drawing.Size(910, 464);
            this.ultraTabPageControl1.Controls.SetChildIndex(this.uGridMaster, 0);
            this.ultraTabPageControl1.Controls.SetChildIndex(this.panel1, 0);
            // 
            // UtbmMain
            // 
            this.UtbmMain.MdiMergeable = false;
            this.UtbmMain.MenuSettings.ForceSerialization = true;
            this.UtbmMain.Office2007UICompatibility = false;
            this.UtbmMain.Ribbon.AllowAutoHide = Infragistics.Win.DefaultableBoolean.True;
            this.UtbmMain.Ribbon.FileMenuButtonCaption = "";
            this.UtbmMain.Ribbon.QuickAccessToolbar.Visible = false;
            this.UtbmMain.Ribbon.Visible = true;
            this.UtbmMain.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2010;
            this.UtbmMain.ToolbarSettings.ForceSerialization = true;
            this.UtbmMain.UseAppStyling = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbl_sum_in10);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 425);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(910, 39);
            this.panel1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 10F);
            this.label2.Location = new System.Drawing.Point(239, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 14);
            this.label2.TabIndex = 6;
            this.label2.Tag = "";
            this.label2.Text = "庫存總異動量:";
            // 
            // lbl_sum_in10
            // 
            this.lbl_sum_in10.AutoSize = true;
            this.lbl_sum_in10.Font = new System.Drawing.Font("新細明體", 10F);
            this.lbl_sum_in10.Location = new System.Drawing.Point(378, 13);
            this.lbl_sum_in10.Name = "lbl_sum_in10";
            this.lbl_sum_in10.Size = new System.Drawing.Size(14, 14);
            this.lbl_sum_in10.TabIndex = 7;
            this.lbl_sum_in10.Tag = "";
            this.lbl_sum_in10.Text = "0";
            // 
            // FrmInvq220
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 566);
            this.KeyPreview = true;
            this.Name = "FrmInvq220";
            this.Text = "FrmInvq220";
            this.Load += new System.EventHandler(this.FrmInvq220_Load);
            ((System.ComponentModel.ISupportInitialize)(this.uGridMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTab_Master)).EndInit();
            this.uTab_Master.ResumeLayout(false);
            this.ultraTabPageControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UtbmMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_sum_in10;
    }
}