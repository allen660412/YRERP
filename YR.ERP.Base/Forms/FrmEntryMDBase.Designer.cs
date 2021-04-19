namespace YR.ERP.Base.Forms
{
    partial class FrmEntryMDBase
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
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            YR.ERP.BLL.MSSQL.AdmBLL admBLL1 = new YR.ERP.BLL.MSSQL.AdmBLL();
            this.ultraTabPageControl6 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl7 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl8 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl9 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl10 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.uTab_Detail = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage2 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraSplitter1 = new Infragistics.Win.Misc.UltraSplitter();
            ((System.ComponentModel.ISupportInitialize)(this.uGridMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTab_Master)).BeginInit();
            this.uTab_Master.SuspendLayout();
            this.ultraTabPageControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UtbmMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTab_Detail)).BeginInit();
            this.uTab_Detail.SuspendLayout();
            this.SuspendLayout();
            // 
            // uGridMaster
            // 
            this.uGridMaster.DataSource = this.BindingMaster;
            this.uGridMaster.DisplayLayout.ForceSerialization = true;
            this.uGridMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uGridMaster.Size = new System.Drawing.Size(910, 207);
            // 
            // uTab_Master
            // 
            this.uTab_Master.Dock = System.Windows.Forms.DockStyle.Top;
            this.uTab_Master.Location = new System.Drawing.Point(1, 54);
            this.uTab_Master.Margin = new System.Windows.Forms.Padding(4);
            this.uTab_Master.Size = new System.Drawing.Size(914, 235);
            this.uTab_Master.TabIndex = 0;
            this.uTab_Master.TabPageMargins.ForceSerialization = true;
            this.uTab_Master.TabStop = false;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Margin = new System.Windows.Forms.Padding(4);
            this.ultraTabPageControl2.Size = new System.Drawing.Size(910, 205);
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Margin = new System.Windows.Forms.Padding(4);
            this.ultraTabPageControl3.Size = new System.Drawing.Size(910, 205);
            // 
            // ultraTabPageControl4
            // 
            this.ultraTabPageControl4.Margin = new System.Windows.Forms.Padding(4);
            this.ultraTabPageControl4.Size = new System.Drawing.Size(910, 205);
            // 
            // ultraTabPageControl5
            // 
            this.ultraTabPageControl5.Controls.Add(this.uGridMaster);
            this.ultraTabPageControl5.Margin = new System.Windows.Forms.Padding(4);
            this.ultraTabPageControl5.Size = new System.Drawing.Size(910, 205);
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Margin = new System.Windows.Forms.Padding(4);
            this.ultraTabPageControl1.Size = new System.Drawing.Size(910, 205);
            // 
            // UtbmMain
            // 
            this.UtbmMain.MenuSettings.ForceSerialization = true;
            this.UtbmMain.Ribbon.AllowAutoHide = Infragistics.Win.DefaultableBoolean.True;
            this.UtbmMain.Ribbon.FileMenuButtonCaption = "";
            this.UtbmMain.Ribbon.QuickAccessToolbar.Visible = false;
            this.UtbmMain.Ribbon.Visible = true;
            this.UtbmMain.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2013;
            this.UtbmMain.ToolbarSettings.ForceSerialization = true;
            // 
            // UsbButtom
            // 
            this.UsbButtom.Location = new System.Drawing.Point(0, 509);
            this.UsbButtom.Margin = new System.Windows.Forms.Padding(3);
            this.UsbButtom.Size = new System.Drawing.Size(916, 18);
            // 
            // ultraTabPageControl6
            // 
            this.ultraTabPageControl6.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl6.Name = "ultraTabPageControl6";
            this.ultraTabPageControl6.Size = new System.Drawing.Size(910, 190);
            // 
            // ultraTabPageControl7
            // 
            this.ultraTabPageControl7.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl7.Name = "ultraTabPageControl7";
            this.ultraTabPageControl7.Size = new System.Drawing.Size(910, 190);
            // 
            // ultraTabPageControl8
            // 
            this.ultraTabPageControl8.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl8.Name = "ultraTabPageControl8";
            this.ultraTabPageControl8.Size = new System.Drawing.Size(910, 190);
            // 
            // ultraTabPageControl9
            // 
            this.ultraTabPageControl9.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl9.Name = "ultraTabPageControl9";
            this.ultraTabPageControl9.Size = new System.Drawing.Size(910, 190);
            // 
            // ultraTabPageControl10
            // 
            this.ultraTabPageControl10.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl10.Name = "ultraTabPageControl10";
            this.ultraTabPageControl10.Size = new System.Drawing.Size(910, 190);
            // 
            // uTab_Detail
            // 
            this.uTab_Detail.Controls.Add(this.ultraTabSharedControlsPage2);
            this.uTab_Detail.Controls.Add(this.ultraTabPageControl6);
            this.uTab_Detail.Controls.Add(this.ultraTabPageControl7);
            this.uTab_Detail.Controls.Add(this.ultraTabPageControl8);
            this.uTab_Detail.Controls.Add(this.ultraTabPageControl9);
            this.uTab_Detail.Controls.Add(this.ultraTabPageControl10);
            this.uTab_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uTab_Detail.Location = new System.Drawing.Point(1, 293);
            this.uTab_Detail.Name = "uTab_Detail";
            this.uTab_Detail.SharedControlsPage = this.ultraTabSharedControlsPage2;
            this.uTab_Detail.Size = new System.Drawing.Size(914, 216);
            this.uTab_Detail.TabIndex = 11;
            ultraTab1.TabPage = this.ultraTabPageControl6;
            ultraTab1.Text = "tab1";
            ultraTab2.TabPage = this.ultraTabPageControl7;
            ultraTab2.Text = "tab2";
            ultraTab3.TabPage = this.ultraTabPageControl8;
            ultraTab3.Text = "tab3";
            ultraTab4.TabPage = this.ultraTabPageControl9;
            ultraTab4.Text = "tab4";
            ultraTab5.TabPage = this.ultraTabPageControl10;
            ultraTab5.Text = "tab5";
            this.uTab_Detail.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3,
            ultraTab4,
            ultraTab5});
            this.uTab_Detail.SelectedTabChanged += new Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventHandler(this.uTab_Detail_SelectedTabChanged);
            // 
            // ultraTabSharedControlsPage2
            // 
            this.ultraTabSharedControlsPage2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage2.Name = "ultraTabSharedControlsPage2";
            this.ultraTabSharedControlsPage2.Size = new System.Drawing.Size(910, 190);
            // 
            // ultraSplitter1
            // 
            this.ultraSplitter1.BackColor = System.Drawing.SystemColors.Control;
            this.ultraSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraSplitter1.Location = new System.Drawing.Point(1, 289);
            this.ultraSplitter1.Name = "ultraSplitter1";
            this.ultraSplitter1.RestoreExtent = 158;
            this.ultraSplitter1.Size = new System.Drawing.Size(914, 4);
            this.ultraSplitter1.TabIndex = 12;
            // 
            // FrmEntryMDBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BoMaster = admBLL1;
            this.ClientSize = new System.Drawing.Size(916, 527);
            this.Controls.Add(this.uTab_Detail);
            this.Controls.Add(this.ultraSplitter1);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FrmEntryMDBase";
            this.Text = "FrmEntryMDBase";
            this.Controls.SetChildIndex(this.UsbButtom, 0);
            this.Controls.SetChildIndex(this.uTab_Master, 0);
            this.Controls.SetChildIndex(this.ultraSplitter1, 0);
            this.Controls.SetChildIndex(this.uTab_Detail, 0);
            ((System.ComponentModel.ISupportInitialize)(this.uGridMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTab_Master)).EndInit();
            this.uTab_Master.ResumeLayout(false);
            this.ultraTabPageControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UtbmMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTab_Detail)).EndInit();
            this.uTab_Detail.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage2;
        protected Infragistics.Win.UltraWinTabControl.UltraTabControl uTab_Detail;
        protected Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl8;
        protected Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl9;
        protected Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl10;
        protected Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl6;
        protected Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl7;
        protected Infragistics.Win.Misc.UltraSplitter ultraSplitter1;

    }
}