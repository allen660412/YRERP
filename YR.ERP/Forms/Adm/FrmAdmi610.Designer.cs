namespace YR.ERP.Forms.Adm
{
    partial class FrmAdmi610
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            YR.ERP.BLL.MSSQL.AdmBLL commonBLL1 = new YR.ERP.BLL.MSSQL.AdmBLL();
            this.ute_adm01_c = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ute_adm01 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            ((System.ComponentModel.ISupportInitialize)(this.uTab_Detail)).BeginInit();
            this.uTab_Detail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uGridMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTab_Master)).BeginInit();
            this.uTab_Master.SuspendLayout();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UtbmMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ute_adm01_c)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ute_adm01)).BeginInit();
            this.SuspendLayout();
            // 
            // uTab_Detail
            // 
            this.uTab_Detail.Location = new System.Drawing.Point(8, 287);
            this.uTab_Detail.Size = new System.Drawing.Size(914, 253);
            this.uTab_Detail.TabIndex = 0;
            this.uTab_Detail.TabPageMargins.ForceSerialization = true;
            this.uTab_Detail.TabStop = false;
            // 
            // ultraTabPageControl8
            // 
            this.ultraTabPageControl8.Size = new System.Drawing.Size(910, 349);
            // 
            // ultraTabPageControl9
            // 
            this.ultraTabPageControl9.Size = new System.Drawing.Size(910, 349);
            // 
            // ultraTabPageControl10
            // 
            this.ultraTabPageControl10.Size = new System.Drawing.Size(910, 349);
            // 
            // ultraTabPageControl6
            // 
            this.ultraTabPageControl6.Size = new System.Drawing.Size(910, 227);
            // 
            // ultraTabPageControl7
            // 
            this.ultraTabPageControl7.Size = new System.Drawing.Size(910, 349);
            // 
            // ultraSplitter1
            // 
            this.ultraSplitter1.Location = new System.Drawing.Point(8, 283);
            this.ultraSplitter1.Size = new System.Drawing.Size(914, 4);
            // 
            // UGrid_Master
            // 
            appearance1.BackColor = System.Drawing.Color.WhiteSmoke;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.uGridMaster.DisplayLayout.Appearance = appearance1;
            this.uGridMaster.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.uGridMaster.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.uGridMaster.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.uGridMaster.DisplayLayout.GroupByBox.Hidden = true;
            this.uGridMaster.DisplayLayout.MaxColScrollRegions = 1;
            this.uGridMaster.DisplayLayout.MaxRowScrollRegions = 1;
            appearance2.ForeColor = System.Drawing.Color.Black;
            this.uGridMaster.DisplayLayout.Override.ActiveCellAppearance = appearance2;
            appearance3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(208)))), ((int)(((byte)(245)))));
            appearance3.ForeColor = System.Drawing.Color.Black;
            this.uGridMaster.DisplayLayout.Override.ActiveRowAppearance = appearance3;
            this.uGridMaster.DisplayLayout.Override.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
            this.uGridMaster.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.uGridMaster.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance4.BackColor = System.Drawing.SystemColors.Window;
            this.uGridMaster.DisplayLayout.Override.CardAreaAppearance = appearance4;
            this.uGridMaster.DisplayLayout.Override.CardSpacing = 2;
            appearance5.BorderColor = System.Drawing.Color.Silver;
            appearance5.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            appearance5.TextVAlignAsString = "Middle";
            this.uGridMaster.DisplayLayout.Override.CellAppearance = appearance5;
            this.uGridMaster.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.uGridMaster.DisplayLayout.Override.CellPadding = 2;
            appearance6.ForeColor = System.Drawing.Color.Red;
            this.uGridMaster.DisplayLayout.Override.DataErrorCellAppearance = appearance6;
            appearance7.BackColor = System.Drawing.Color.LightYellow;
            this.uGridMaster.DisplayLayout.Override.DataErrorRowAppearance = appearance7;
            appearance8.BackColor = System.Drawing.Color.Red;
            this.uGridMaster.DisplayLayout.Override.DataErrorRowSelectorAppearance = appearance8;
            appearance9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.uGridMaster.DisplayLayout.Override.EditCellAppearance = appearance9;
            appearance10.BackColor = System.Drawing.SystemColors.Control;
            appearance10.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance10.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance10.BorderColor = System.Drawing.SystemColors.Window;
            this.uGridMaster.DisplayLayout.Override.GroupByRowAppearance = appearance10;
            appearance11.TextHAlignAsString = "Left";
            this.uGridMaster.DisplayLayout.Override.HeaderAppearance = appearance11;
            this.uGridMaster.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.uGridMaster.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(208)))));
            this.uGridMaster.DisplayLayout.Override.RowAlternateAppearance = appearance12;
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.uGridMaster.DisplayLayout.Override.RowAppearance = appearance13;
            this.uGridMaster.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.uGridMaster.DisplayLayout.Override.RowSelectorWidth = 20;
            this.uGridMaster.DisplayLayout.Override.SupportDataErrorInfo = Infragistics.Win.UltraWinGrid.SupportDataErrorInfo.RowsAndCells;
            appearance14.BackColor = System.Drawing.SystemColors.ControlLight;
            this.uGridMaster.DisplayLayout.Override.TemplateAddRowAppearance = appearance14;
            this.uGridMaster.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.uGridMaster.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.uGridMaster.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.uGridMaster.Font = new System.Drawing.Font("新細明體", 9.5F);
            this.uGridMaster.Size = new System.Drawing.Size(924, 207);
            // 
            // uTab_Master
            // 
            this.uTab_Master.Location = new System.Drawing.Point(8, 54);
            this.uTab_Master.Size = new System.Drawing.Size(914, 229);
            this.uTab_Master.TabPageMargins.ForceSerialization = true;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Size = new System.Drawing.Size(910, 77);
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Size = new System.Drawing.Size(910, 77);
            // 
            // ultraTabPageControl4
            // 
            this.ultraTabPageControl4.Size = new System.Drawing.Size(910, 77);
            // 
            // ultraTabPageControl5
            // 
            this.ultraTabPageControl5.Size = new System.Drawing.Size(910, 77);
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.uGridMaster);
            this.ultraTabPageControl1.Controls.Add(this.ute_adm01);
            this.ultraTabPageControl1.Controls.Add(this.ute_adm01_c);
            this.ultraTabPageControl1.Controls.Add(this.label1);
            this.ultraTabPageControl1.Controls.Add(this.label2);
            this.ultraTabPageControl1.Size = new System.Drawing.Size(910, 199);
            this.ultraTabPageControl1.Controls.SetChildIndex(this.label2, 0);
            this.ultraTabPageControl1.Controls.SetChildIndex(this.label1, 0);
            this.ultraTabPageControl1.Controls.SetChildIndex(this.ute_adm01_c, 0);
            this.ultraTabPageControl1.Controls.SetChildIndex(this.ute_adm01, 0);
            this.ultraTabPageControl1.Controls.SetChildIndex(this.uGridMaster, 0);
            // 
            // UtbmMain
            // 
            this.UtbmMain.MenuSettings.ForceSerialization = true;
            this.UtbmMain.Ribbon.AllowAutoHide = Infragistics.Win.DefaultableBoolean.True;
            this.UtbmMain.Ribbon.FileMenuButtonCaption = "";
            this.UtbmMain.Ribbon.QuickAccessToolbar.Visible = false;
            this.UtbmMain.Ribbon.Visible = true;
            this.UtbmMain.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.ScenicRibbon;
            this.UtbmMain.ToolbarSettings.ForceSerialization = true;
            // 
            // UsbButtom
            // 
            this.UsbButtom.Location = new System.Drawing.Point(8, 540);
            this.UsbButtom.Size = new System.Drawing.Size(914, 18);
            // 
            // ute_adm01_c
            // 
            this.ute_adm01_c.Font = new System.Drawing.Font("新細明體", 10F);
            this.ute_adm01_c.Location = new System.Drawing.Point(118, 40);
            this.ute_adm01_c.Name = "ute_adm01_c";
            this.ute_adm01_c.Size = new System.Drawing.Size(387, 22);
            this.ute_adm01_c.TabIndex = 1;
            this.ute_adm01_c.Tag = "adm01_c";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 10F);
            this.label1.Location = new System.Drawing.Point(14, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 14);
            this.label1.TabIndex = 19;
            this.label1.Tag = "adm01";
            this.label1.Text = "adm01";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 10F);
            this.label2.Location = new System.Drawing.Point(14, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 14);
            this.label2.TabIndex = 20;
            this.label2.Tag = "adm01_c";
            this.label2.Text = "adm01_c";
            // 
            // ute_adm01
            // 
            this.ute_adm01.AccessibleDescription = "R";
            this.ute_adm01.Font = new System.Drawing.Font("新細明體", 10F);
            this.ute_adm01.Location = new System.Drawing.Point(118, 11);
            this.ute_adm01.Name = "ute_adm01";
            this.ute_adm01.Size = new System.Drawing.Size(140, 22);
            this.ute_adm01.TabIndex = 0;
            this.ute_adm01.Tag = "adm01";
            // 
            // FrmAdmi610
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BoMaster = commonBLL1;
            this.ClientSize = new System.Drawing.Size(930, 566);
            this.Name = "FrmAdmi610";
            this.Text = "FrmAdmi610";
            ((System.ComponentModel.ISupportInitialize)(this.uTab_Detail)).EndInit();
            this.uTab_Detail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uGridMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTab_Master)).EndInit();
            this.uTab_Master.ResumeLayout(false);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UtbmMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ute_adm01_c)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ute_adm01)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor ute_adm01_c;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ute_adm01;
    }
}