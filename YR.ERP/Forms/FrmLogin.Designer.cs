namespace YR.ERP.Forms
{
    partial class FrmLogin
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
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.ucb_comp_no = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.UbtnOk = new Infragistics.Win.Misc.UltraButton();
            this.UbtnCancel = new Infragistics.Win.Misc.UltraButton();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_InputUserid = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txt_InputPassword = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucb_comp_no)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_InputUserid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_InputPassword)).BeginInit();
            this.SuspendLayout();
            // 
            // UsbButtom
            // 
            this.UsbButtom.Location = new System.Drawing.Point(0, 211);
            this.UsbButtom.Size = new System.Drawing.Size(481, 18);
            // 
            // Label1
            // 
            this.Label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(81, 47);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(96, 23);
            this.Label1.TabIndex = 45;
            this.Label1.Text = "使用者名稱:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label2
            // 
            this.Label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(84, 71);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(93, 23);
            this.Label2.TabIndex = 46;
            this.Label2.Text = "使用者密碼:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ucb_comp_no
            // 
            this.ucb_comp_no.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            this.ucb_comp_no.Font = new System.Drawing.Font("新細明體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ucb_comp_no.Location = new System.Drawing.Point(177, 93);
            this.ucb_comp_no.Name = "ucb_comp_no";
            this.ucb_comp_no.Size = new System.Drawing.Size(208, 25);
            this.ucb_comp_no.TabIndex = 2;
            this.ucb_comp_no.Visible = false;
            // 
            // UbtnOk
            // 
            this.UbtnOk.Font = new System.Drawing.Font("新細明體", 11.25F);
            this.UbtnOk.Location = new System.Drawing.Point(194, 123);
            this.UbtnOk.Name = "UbtnOk";
            this.UbtnOk.Size = new System.Drawing.Size(82, 39);
            this.UbtnOk.TabIndex = 3;
            this.UbtnOk.Text = "(&O)確認";
            this.UbtnOk.Click += new System.EventHandler(this.Button_Click);
            // 
            // UbtnCancel
            // 
            this.UbtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.UbtnCancel.Font = new System.Drawing.Font("新細明體", 11.25F);
            this.UbtnCancel.Location = new System.Drawing.Point(282, 123);
            this.UbtnCancel.Name = "UbtnCancel";
            this.UbtnCancel.Size = new System.Drawing.Size(82, 39);
            this.UbtnCancel.TabIndex = 4;
            this.UbtnCancel.Text = "(&C)取消";
            this.UbtnCancel.Click += new System.EventHandler(this.Button_Click);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(65, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 23);
            this.label6.TabIndex = 47;
            this.label6.Text = "登入公司別:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label6.Visible = false;
            // 
            // txt_InputUserid
            // 
            this.txt_InputUserid.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt_InputUserid.Location = new System.Drawing.Point(177, 47);
            this.txt_InputUserid.Name = "txt_InputUserid";
            this.txt_InputUserid.ShowInkButton = Infragistics.Win.ShowInkButton.Never;
            this.txt_InputUserid.Size = new System.Drawing.Size(208, 21);
            this.txt_InputUserid.TabIndex = 0;
            this.txt_InputUserid.Text = "Y001";
            // 
            // txt_InputPassword
            // 
            this.txt_InputPassword.Location = new System.Drawing.Point(177, 71);
            this.txt_InputPassword.Name = "txt_InputPassword";
            this.txt_InputPassword.PasswordChar = '*';
            this.txt_InputPassword.ShowInkButton = Infragistics.Win.ShowInkButton.Never;
            this.txt_InputPassword.Size = new System.Drawing.Size(208, 21);
            this.txt_InputPassword.TabIndex = 1;
            this.txt_InputPassword.Text = "9999";
            // 
            // FrmLogin
            // 
            this.AcceptButton = this.UbtnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.UbtnCancel;
            this.ClientSize = new System.Drawing.Size(481, 229);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.ucb_comp_no);
            this.Controls.Add(this.UbtnOk);
            this.Controls.Add(this.UbtnCancel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txt_InputUserid);
            this.Controls.Add(this.txt_InputPassword);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "使用者登入";
            this.Load += new System.EventHandler(this.FrmLogin_Load);
            this.Controls.SetChildIndex(this.UsbButtom, 0);
            this.Controls.SetChildIndex(this.txt_InputPassword, 0);
            this.Controls.SetChildIndex(this.txt_InputUserid, 0);
            this.Controls.SetChildIndex(this.label6, 0);
            this.Controls.SetChildIndex(this.UbtnCancel, 0);
            this.Controls.SetChildIndex(this.UbtnOk, 0);
            this.Controls.SetChildIndex(this.ucb_comp_no, 0);
            this.Controls.SetChildIndex(this.Label2, 0);
            this.Controls.SetChildIndex(this.Label1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucb_comp_no)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_InputUserid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_InputPassword)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.Label Label2;
        private Infragistics.Win.UltraWinGrid.UltraCombo ucb_comp_no;
        private Infragistics.Win.Misc.UltraButton UbtnOk;
        private Infragistics.Win.Misc.UltraButton UbtnCancel;
        private System.Windows.Forms.Label label6;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txt_InputUserid;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txt_InputPassword;
    }
}