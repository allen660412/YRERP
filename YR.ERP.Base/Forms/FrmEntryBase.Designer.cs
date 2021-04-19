namespace YR.ERP.Base.Forms
{
    partial class FrmEntryBase
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("UltraToolbar1");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("BtRibbonVisible");
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl4 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl5 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.uTab_Master = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this._FrmBase_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.UtbmMain = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._FrmBase_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._FrmBase_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._FrmBase_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTab_Master)).BeginInit();
            this.uTab_Master.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UtbmMain)).BeginInit();
            this.SuspendLayout();
            // 
            // UsbButtom
            // 
            this.UsbButtom.Location = new System.Drawing.Point(0, 548);
            this.UsbButtom.Margin = new System.Windows.Forms.Padding(4);
            this.UsbButtom.Size = new System.Drawing.Size(930, 18);
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 27);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(910, 463);
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(910, 463);
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(910, 463);
            // 
            // ultraTabPageControl4
            // 
            this.ultraTabPageControl4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl4.Name = "ultraTabPageControl4";
            this.ultraTabPageControl4.Size = new System.Drawing.Size(910, 463);
            // 
            // ultraTabPageControl5
            // 
            this.ultraTabPageControl5.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl5.Name = "ultraTabPageControl5";
            this.ultraTabPageControl5.Size = new System.Drawing.Size(910, 463);
            // 
            // uTab_Master
            // 
            this.uTab_Master.Controls.Add(this.ultraTabSharedControlsPage1);
            this.uTab_Master.Controls.Add(this.ultraTabPageControl1);
            this.uTab_Master.Controls.Add(this.ultraTabPageControl2);
            this.uTab_Master.Controls.Add(this.ultraTabPageControl3);
            this.uTab_Master.Controls.Add(this.ultraTabPageControl4);
            this.uTab_Master.Controls.Add(this.ultraTabPageControl5);
            this.uTab_Master.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uTab_Master.Font = new System.Drawing.Font("新細明體", 9F);
            this.uTab_Master.Location = new System.Drawing.Point(8, 55);
            this.uTab_Master.Name = "uTab_Master";
            this.uTab_Master.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.uTab_Master.Size = new System.Drawing.Size(914, 493);
            this.uTab_Master.TabIndex = 5;
            this.uTab_Master.TabPadding = new System.Drawing.Size(1, 3);
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "tab1";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "tab2";
            ultraTab3.TabPage = this.ultraTabPageControl3;
            ultraTab3.Text = "tab3";
            ultraTab4.TabPage = this.ultraTabPageControl4;
            ultraTab4.Text = "tab4";
            ultraTab5.TabPage = this.ultraTabPageControl5;
            ultraTab5.Text = "tab5";
            this.uTab_Master.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3,
            ultraTab4,
            ultraTab5});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(910, 463);
            // 
            // _FrmBase_Toolbars_Dock_Area_Right
            // 
            this._FrmBase_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._FrmBase_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._FrmBase_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._FrmBase_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._FrmBase_Toolbars_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._FrmBase_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(922, 55);
            this._FrmBase_Toolbars_Dock_Area_Right.Name = "_FrmBase_Toolbars_Dock_Area_Right";
            this._FrmBase_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(8, 493);
            this._FrmBase_Toolbars_Dock_Area_Right.ToolbarsManager = this.UtbmMain;
            // 
            // UtbmMain
            // 
            this.UtbmMain.DesignerFlags = 1;
            this.UtbmMain.DockWithinContainer = this;
            this.UtbmMain.DockWithinContainerBaseType = typeof(YR.ERP.Base.Forms.FrmBase);
            this.UtbmMain.Ribbon.FileMenuButtonCaption = "";
            this.UtbmMain.Ribbon.Visible = true;
            this.UtbmMain.SettingsKey = "FrmEntryBase.UtbmMain";
            this.UtbmMain.ShowFullMenusDelay = 500;
            this.UtbmMain.ShowShortcutsInToolTips = true;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Floating;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.FloatingLocation = new System.Drawing.Point(104, 394);
            ultraToolbar1.FloatingSize = new System.Drawing.Size(113, 23);
            ultraToolbar1.Text = "UltraToolbar1";
            ultraToolbar1.Visible = false;
            this.UtbmMain.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            buttonTool2.SharedPropsInternal.Caption = "彈出工具列";
            this.UtbmMain.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool2});
            this.UtbmMain.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.UtbmMain_ToolClick);
            // 
            // _FrmBase_Toolbars_Dock_Area_Left
            // 
            this._FrmBase_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._FrmBase_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._FrmBase_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._FrmBase_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._FrmBase_Toolbars_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._FrmBase_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 55);
            this._FrmBase_Toolbars_Dock_Area_Left.Name = "_FrmBase_Toolbars_Dock_Area_Left";
            this._FrmBase_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(8, 493);
            this._FrmBase_Toolbars_Dock_Area_Left.ToolbarsManager = this.UtbmMain;
            // 
            // _FrmBase_Toolbars_Dock_Area_Bottom
            // 
            this._FrmBase_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._FrmBase_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._FrmBase_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._FrmBase_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._FrmBase_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 548);
            this._FrmBase_Toolbars_Dock_Area_Bottom.Name = "_FrmBase_Toolbars_Dock_Area_Bottom";
            this._FrmBase_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(930, 0);
            this._FrmBase_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.UtbmMain;
            // 
            // _FrmBase_Toolbars_Dock_Area_Top
            // 
            this._FrmBase_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._FrmBase_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._FrmBase_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._FrmBase_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._FrmBase_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._FrmBase_Toolbars_Dock_Area_Top.Name = "_FrmBase_Toolbars_Dock_Area_Top";
            this._FrmBase_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(930, 55);
            this._FrmBase_Toolbars_Dock_Area_Top.ToolbarsManager = this.UtbmMain;
            // 
            // FrmEntryBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 566);
            this.Controls.Add(this.uTab_Master);
            this.Controls.Add(this._FrmBase_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._FrmBase_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._FrmBase_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._FrmBase_Toolbars_Dock_Area_Top);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmEntryBase";
            this.Text = "FrmEntryBase";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmEntryBase_FormClosing);
            this.Load += new System.EventHandler(this.FrmEntryBase_Load);
            this.Controls.SetChildIndex(this._FrmBase_Toolbars_Dock_Area_Top, 0);
            this.Controls.SetChildIndex(this.UsbButtom, 0);
            this.Controls.SetChildIndex(this._FrmBase_Toolbars_Dock_Area_Bottom, 0);
            this.Controls.SetChildIndex(this._FrmBase_Toolbars_Dock_Area_Right, 0);
            this.Controls.SetChildIndex(this._FrmBase_Toolbars_Dock_Area_Left, 0);
            this.Controls.SetChildIndex(this.uTab_Master, 0);
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTab_Master)).EndInit();
            this.uTab_Master.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UtbmMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _FrmBase_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _FrmBase_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _FrmBase_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _FrmBase_Toolbars_Dock_Area_Top;
        protected Infragistics.Win.UltraWinTabControl.UltraTabControl uTab_Master;
        protected Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        protected Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        protected Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl4;
        protected Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl5;
        protected Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        public Infragistics.Win.UltraWinToolbars.UltraToolbarsManager UtbmMain;
    }
}