namespace YR.ERP.Base.Forms
{
    partial class FrmPickBase
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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.uGrid_Master = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.UtbmMain = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._FrmBase_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._FrmBase_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._FrmBase_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._FrmBase_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uGrid_Master)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UtbmMain)).BeginInit();
            this.SuspendLayout();
            // 
            // UsbButtom
            // 
            this.UsbButtom.Location = new System.Drawing.Point(0, 387);
            this.UsbButtom.Size = new System.Drawing.Size(547, 18);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.uGrid_Master);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(531, 307);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(8, 55);
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.RightToolStripPanel
            // 
            this.toolStripContainer1.RightToolStripPanel.BackColor = System.Drawing.Color.PapayaWhip;
            this.toolStripContainer1.Size = new System.Drawing.Size(531, 332);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // uGrid_Master
            // 
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.uGrid_Master.DisplayLayout.Appearance = appearance1;
            this.uGrid_Master.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.uGrid_Master.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.uGrid_Master.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.uGrid_Master.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.uGrid_Master.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.uGrid_Master.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.uGrid_Master.DisplayLayout.MaxColScrollRegions = 1;
            this.uGrid_Master.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.uGrid_Master.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.uGrid_Master.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.uGrid_Master.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.uGrid_Master.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.uGrid_Master.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.uGrid_Master.DisplayLayout.Override.CellAppearance = appearance8;
            this.uGrid_Master.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.uGrid_Master.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.uGrid_Master.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.uGrid_Master.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.uGrid_Master.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.uGrid_Master.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.uGrid_Master.DisplayLayout.Override.RowAppearance = appearance11;
            this.uGrid_Master.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.uGrid_Master.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.uGrid_Master.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.uGrid_Master.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.uGrid_Master.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.uGrid_Master.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uGrid_Master.Location = new System.Drawing.Point(0, 0);
            this.uGrid_Master.Name = "uGrid_Master";
            this.uGrid_Master.Size = new System.Drawing.Size(531, 307);
            this.uGrid_Master.TabIndex = 0;
            this.uGrid_Master.Text = "ultraGrid1";
            // 
            // UtbmMain
            // 
            this.UtbmMain.DesignerFlags = 1;
            this.UtbmMain.DockWithinContainer = this;
            this.UtbmMain.DockWithinContainerBaseType = typeof(YR.ERP.Base.Forms.FrmBase);
            this.UtbmMain.Ribbon.Visible = true;
            this.UtbmMain.ShowFullMenusDelay = 500;
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
            this._FrmBase_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(8, 332);
            this._FrmBase_Toolbars_Dock_Area_Left.ToolbarsManager = this.UtbmMain;
            // 
            // _FrmBase_Toolbars_Dock_Area_Right
            // 
            this._FrmBase_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._FrmBase_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._FrmBase_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._FrmBase_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._FrmBase_Toolbars_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._FrmBase_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(539, 55);
            this._FrmBase_Toolbars_Dock_Area_Right.Name = "_FrmBase_Toolbars_Dock_Area_Right";
            this._FrmBase_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(8, 332);
            this._FrmBase_Toolbars_Dock_Area_Right.ToolbarsManager = this.UtbmMain;
            // 
            // _FrmBase_Toolbars_Dock_Area_Top
            // 
            this._FrmBase_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._FrmBase_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._FrmBase_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._FrmBase_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._FrmBase_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._FrmBase_Toolbars_Dock_Area_Top.Name = "_FrmBase_Toolbars_Dock_Area_Top";
            this._FrmBase_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(547, 55);
            this._FrmBase_Toolbars_Dock_Area_Top.ToolbarsManager = this.UtbmMain;
            // 
            // _FrmBase_Toolbars_Dock_Area_Bottom
            // 
            this._FrmBase_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._FrmBase_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._FrmBase_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._FrmBase_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._FrmBase_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 387);
            this._FrmBase_Toolbars_Dock_Area_Bottom.Name = "_FrmBase_Toolbars_Dock_Area_Bottom";
            this._FrmBase_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(547, 0);
            this._FrmBase_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.UtbmMain;
            // 
            // FrmPickBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 405);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this._FrmBase_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._FrmBase_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._FrmBase_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._FrmBase_Toolbars_Dock_Area_Top);
            this.Name = "FrmPickBase";
            this.Text = "FrmPickBase";
            this.Load += new System.EventHandler(this.FrmPickBase_Load);
            this.Controls.SetChildIndex(this._FrmBase_Toolbars_Dock_Area_Top, 0);
            this.Controls.SetChildIndex(this.UsbButtom, 0);
            this.Controls.SetChildIndex(this._FrmBase_Toolbars_Dock_Area_Bottom, 0);
            this.Controls.SetChildIndex(this._FrmBase_Toolbars_Dock_Area_Right, 0);
            this.Controls.SetChildIndex(this._FrmBase_Toolbars_Dock_Area_Left, 0);
            this.Controls.SetChildIndex(this.toolStripContainer1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uGrid_Master)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UtbmMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager UtbmMain;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _FrmBase_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _FrmBase_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _FrmBase_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _FrmBase_Toolbars_Dock_Area_Top;
        protected internal Infragistics.Win.UltraWinGrid.UltraGrid uGrid_Master;
    }
}