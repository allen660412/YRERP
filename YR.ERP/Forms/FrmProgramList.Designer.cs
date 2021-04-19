namespace YR.ERP.Forms
{
    partial class FrmProgramList
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
            this.UebMenu = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Label1 = new System.Windows.Forms.Label();
            this.ubtnChangeComp = new Infragistics.Win.Misc.UltraButton();
            this.ute_ado01 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ubtnGo = new Infragistics.Win.Misc.UltraButton();
            this.pnlFlow = new System.Windows.Forms.Panel();
            this.pbxFlow = new System.Windows.Forms.PictureBox();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.ubtnNavigator = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UebMenu)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ute_ado01)).BeginInit();
            this.pnlFlow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxFlow)).BeginInit();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // UsbButtom
            // 
            this.UsbButtom.Location = new System.Drawing.Point(359, 623);
            this.UsbButtom.Size = new System.Drawing.Size(772, 18);
            // 
            // UebMenu
            // 
            this.UebMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.UebMenu.GroupSettings.Style = Infragistics.Win.UltraWinExplorerBar.GroupStyle.SmallImagesWithText;
            this.UebMenu.Location = new System.Drawing.Point(0, 0);
            this.UebMenu.Name = "UebMenu";
            this.UebMenu.Size = new System.Drawing.Size(359, 641);
            this.UebMenu.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.OutlookNavigationPane;
            this.UebMenu.TabIndex = 3;
            this.UebMenu.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ubtnNavigator);
            this.panel1.Controls.Add(this.Label1);
            this.panel1.Controls.Add(this.ubtnChangeComp);
            this.panel1.Controls.Add(this.ute_ado01);
            this.panel1.Controls.Add(this.ubtnGo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(359, 76);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(772, 73);
            this.panel1.TabIndex = 50;
            // 
            // Label1
            // 
            this.Label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(25, 10);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(125, 23);
            this.Label1.TabIndex = 47;
            this.Label1.Text = "請輸入程式代號:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ubtnChangeComp
            // 
            this.ubtnChangeComp.Location = new System.Drawing.Point(391, 39);
            this.ubtnChangeComp.Name = "ubtnChangeComp";
            this.ubtnChangeComp.Size = new System.Drawing.Size(87, 26);
            this.ubtnChangeComp.TabIndex = 2;
            this.ubtnChangeComp.Text = "切換公司別";
            this.ubtnChangeComp.Click += new System.EventHandler(this.Button_Click);
            // 
            // ute_ado01
            // 
            this.ute_ado01.Location = new System.Drawing.Point(154, 10);
            this.ute_ado01.Name = "ute_ado01";
            this.ute_ado01.Size = new System.Drawing.Size(215, 21);
            this.ute_ado01.TabIndex = 0;
            // 
            // ubtnGo
            // 
            this.ubtnGo.Location = new System.Drawing.Point(391, 11);
            this.ubtnGo.Name = "ubtnGo";
            this.ubtnGo.Size = new System.Drawing.Size(87, 26);
            this.ubtnGo.TabIndex = 1;
            this.ubtnGo.Text = "GO";
            this.ubtnGo.Click += new System.EventHandler(this.Button_Click);
            // 
            // pnlFlow
            // 
            this.pnlFlow.Controls.Add(this.pbxFlow);
            this.pnlFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFlow.Location = new System.Drawing.Point(359, 149);
            this.pnlFlow.Name = "pnlFlow";
            this.pnlFlow.Size = new System.Drawing.Size(772, 474);
            this.pnlFlow.TabIndex = 51;
            // 
            // pbxFlow
            // 
            this.pbxFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbxFlow.Location = new System.Drawing.Point(0, 0);
            this.pbxFlow.Name = "pbxFlow";
            this.pbxFlow.Size = new System.Drawing.Size(772, 474);
            this.pbxFlow.TabIndex = 0;
            this.pbxFlow.TabStop = false;
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.label2);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(359, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(772, 76);
            this.pnlTop.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(21, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(345, 47);
            this.label2.TabIndex = 0;
            this.label2.Text = "好夥計ERP管理系統";
            // 
            // ubtnNavigator
            // 
            this.ubtnNavigator.Location = new System.Drawing.Point(484, 11);
            this.ubtnNavigator.Name = "ubtnNavigator";
            this.ubtnNavigator.Size = new System.Drawing.Size(87, 26);
            this.ubtnNavigator.TabIndex = 48;
            this.ubtnNavigator.Text = "切換視窗";
            this.ubtnNavigator.Click += new System.EventHandler(this.Button_Click);
            // 
            // FrmProgramList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1131, 641);
            this.Controls.Add(this.pnlFlow);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.UebMenu);
            this.Name = "FrmProgramList";
            this.Text = "FrmProgramList";
            this.Load += new System.EventHandler(this.FrmProgramList_Load);
            this.Controls.SetChildIndex(this.UebMenu, 0);
            this.Controls.SetChildIndex(this.pnlTop, 0);
            this.Controls.SetChildIndex(this.UsbButtom, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.pnlFlow, 0);
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UebMenu)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ute_ado01)).EndInit();
            this.pnlFlow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxFlow)).EndInit();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar UebMenu;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Label1;
        private Infragistics.Win.Misc.UltraButton ubtnChangeComp;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ute_ado01;
        private Infragistics.Win.Misc.UltraButton ubtnGo;
        private System.Windows.Forms.Panel pnlFlow;
        private System.Windows.Forms.PictureBox pbxFlow;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label label2;
        private Infragistics.Win.Misc.UltraButton ubtnNavigator;
    }
}