namespace YR.ERP.Forms.Adm
{
    partial class FrmAdms801
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
            this.UbtnOk = new Infragistics.Win.Misc.UltraButton();
            this.UbtnCancel = new Infragistics.Win.Misc.UltraButton();
            this.ute_adb02 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.Label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ute_adb02)).BeginInit();
            this.SuspendLayout();
            // 
            // UsbButtom
            // 
            this.UsbButtom.Location = new System.Drawing.Point(0, 104);
            this.UsbButtom.Size = new System.Drawing.Size(420, 18);
            // 
            // UbtnOk
            // 
            this.UbtnOk.Location = new System.Drawing.Point(328, 20);
            this.UbtnOk.Name = "UbtnOk";
            this.UbtnOk.Size = new System.Drawing.Size(79, 36);
            this.UbtnOk.TabIndex = 1;
            this.UbtnOk.Text = "確定";
            this.UbtnOk.Click += new System.EventHandler(this.Button_Click);
            // 
            // UbtnCancel
            // 
            this.UbtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.UbtnCancel.Location = new System.Drawing.Point(329, 62);
            this.UbtnCancel.Name = "UbtnCancel";
            this.UbtnCancel.Size = new System.Drawing.Size(78, 36);
            this.UbtnCancel.TabIndex = 2;
            this.UbtnCancel.Text = "取消";
            this.UbtnCancel.Click += new System.EventHandler(this.Button_Click);
            // 
            // ute_adb02
            // 
            this.ute_adb02.Location = new System.Drawing.Point(187, 26);
            this.ute_adb02.Name = "ute_adb02";
            this.ute_adb02.Size = new System.Drawing.Size(108, 21);
            this.ute_adb02.TabIndex = 0;
            this.ute_adb02.Tag = "adb02";
            this.ute_adb02.Text = "Y01";
            // 
            // Label1
            // 
            this.Label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(71, 26);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(110, 23);
            this.Label1.TabIndex = 46;
            this.Label1.Text = "請輸入公司別:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FrmAdms801
            // 
            this.AcceptButton = this.UbtnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.UbtnCancel;
            this.ClientSize = new System.Drawing.Size(420, 122);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.ute_adb02);
            this.Controls.Add(this.UbtnCancel);
            this.Controls.Add(this.UbtnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FrmAdms801";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmAdms801";
            this.Load += new System.EventHandler(this.FrmAdms801_Load);
            this.Controls.SetChildIndex(this.UsbButtom, 0);
            this.Controls.SetChildIndex(this.UbtnOk, 0);
            this.Controls.SetChildIndex(this.UbtnCancel, 0);
            this.Controls.SetChildIndex(this.ute_adb02, 0);
            this.Controls.SetChildIndex(this.Label1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ute_adb02)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton UbtnOk;
        private Infragistics.Win.Misc.UltraButton UbtnCancel;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ute_adb02;
        private System.Windows.Forms.Label Label1;
    }
}