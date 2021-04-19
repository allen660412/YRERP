namespace YR.ERP.Forms.Adm
{
    partial class FrmAdmq910
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
            this.ucx_qryIsBloced = new YR.Util.Controls.UcCheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
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
            this.uTab_Master.Location = new System.Drawing.Point(8, 55);
            this.uTab_Master.Size = new System.Drawing.Size(914, 493);
            this.uTab_Master.TabPageMargins.ForceSerialization = true;
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.panel2);
            this.ultraTabPageControl1.Controls.Add(this.uGridMaster);
            this.ultraTabPageControl1.Controls.Add(this.panel1);
            this.ultraTabPageControl1.Controls.SetChildIndex(this.panel1, 0);
            this.ultraTabPageControl1.Controls.SetChildIndex(this.uGridMaster, 0);
            this.ultraTabPageControl1.Controls.SetChildIndex(this.panel2, 0);
            // 
            // UtbmMain
            // 
            this.UtbmMain.MenuSettings.ForceSerialization = true;
            this.UtbmMain.Ribbon.FileMenuButtonCaption = "";
            this.UtbmMain.Ribbon.Visible = true;
            this.UtbmMain.ToolbarSettings.ForceSerialization = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ucx_qryIsBloced);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(910, 68);
            this.panel1.TabIndex = 1;
            // 
            // ucx_qryIsBloced
            // 
            this.ucx_qryIsBloced.AutoSize = true;
            this.ucx_qryIsBloced.Checked = true;
            this.ucx_qryIsBloced.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ucx_qryIsBloced.CheckValue = "Y";
            this.ucx_qryIsBloced.Font = new System.Drawing.Font("新細明體", 10F);
            this.ucx_qryIsBloced.Location = new System.Drawing.Point(30, 23);
            this.ucx_qryIsBloced.Name = "ucx_qryIsBloced";
            this.ucx_qryIsBloced.NullValue = "";
            this.ucx_qryIsBloced.Size = new System.Drawing.Size(147, 18);
            this.ucx_qryIsBloced.TabIndex = 10;
            this.ucx_qryIsBloced.Tag = "";
            this.ucx_qryIsBloced.Text = "僅顯示被locked資料";
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 68);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(910, 395);
            this.panel2.TabIndex = 2;
            // 
            // FrmAdmq910
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 566);
            this.KeyPreview = true;
            this.Name = "FrmAdmq910";
            this.Text = "FrmAdmq910";
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
        private System.Windows.Forms.Panel panel2;
        private Util.Controls.UcCheckBox ucx_qryIsBloced;
    }
}