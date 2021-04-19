namespace YR.ERP.Base.Forms
{
    partial class FrmAdvanceQuery
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
            this.uTabControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.UtbmMain = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this.FrmBase_Fill_Panel = new System.Windows.Forms.Panel();
            this._FrmBase_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._FrmBase_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._FrmBase_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._FrmBase_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTabControl)).BeginInit();
            this.uTabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UtbmMain)).BeginInit();
            this.FrmBase_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // UsbButtom
            // 
            this.UsbButtom.Location = new System.Drawing.Point(0, 592);
            this.UsbButtom.Size = new System.Drawing.Size(749, 18);
            // 
            // uTabControl
            // 
            this.uTabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.uTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uTabControl.Location = new System.Drawing.Point(0, 0);
            this.uTabControl.Name = "uTabControl";
            this.uTabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.uTabControl.Size = new System.Drawing.Size(733, 537);
            this.uTabControl.TabIndex = 1;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(1, 20);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(729, 514);
            // 
            // UtbmMain
            // 
            this.UtbmMain.DesignerFlags = 1;
            this.UtbmMain.DockWithinContainer = this;
            this.UtbmMain.DockWithinContainerBaseType = typeof(YR.ERP.Base.Forms.FrmBase);
            this.UtbmMain.Ribbon.Visible = true;
            this.UtbmMain.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.UtbmMain_ToolClick);
            // 
            // FrmBase_Fill_Panel
            // 
            this.FrmBase_Fill_Panel.Controls.Add(this.uTabControl);
            this.FrmBase_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.FrmBase_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FrmBase_Fill_Panel.Location = new System.Drawing.Point(8, 55);
            this.FrmBase_Fill_Panel.Name = "FrmBase_Fill_Panel";
            this.FrmBase_Fill_Panel.Size = new System.Drawing.Size(733, 537);
            this.FrmBase_Fill_Panel.TabIndex = 1;
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
            this._FrmBase_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(8, 537);
            this._FrmBase_Toolbars_Dock_Area_Left.ToolbarsManager = this.UtbmMain;
            // 
            // _FrmBase_Toolbars_Dock_Area_Right
            // 
            this._FrmBase_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._FrmBase_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._FrmBase_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._FrmBase_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._FrmBase_Toolbars_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._FrmBase_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(741, 55);
            this._FrmBase_Toolbars_Dock_Area_Right.Name = "_FrmBase_Toolbars_Dock_Area_Right";
            this._FrmBase_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(8, 537);
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
            this._FrmBase_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(749, 55);
            this._FrmBase_Toolbars_Dock_Area_Top.ToolbarsManager = this.UtbmMain;
            // 
            // _FrmBase_Toolbars_Dock_Area_Bottom
            // 
            this._FrmBase_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._FrmBase_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._FrmBase_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._FrmBase_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._FrmBase_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 592);
            this._FrmBase_Toolbars_Dock_Area_Bottom.Name = "_FrmBase_Toolbars_Dock_Area_Bottom";
            this._FrmBase_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(749, 0);
            this._FrmBase_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.UtbmMain;
            // 
            // FrmAdvanceQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 610);
            this.Controls.Add(this.FrmBase_Fill_Panel);
            this.Controls.Add(this._FrmBase_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._FrmBase_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._FrmBase_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._FrmBase_Toolbars_Dock_Area_Top);
            this.Name = "FrmAdvanceQuery";
            this.Text = "FrmAdvanceQuery";
            this.Load += new System.EventHandler(this.FrmAdvanceQuery_Load);
            this.Controls.SetChildIndex(this._FrmBase_Toolbars_Dock_Area_Top, 0);
            this.Controls.SetChildIndex(this.UsbButtom, 0);
            this.Controls.SetChildIndex(this._FrmBase_Toolbars_Dock_Area_Bottom, 0);
            this.Controls.SetChildIndex(this._FrmBase_Toolbars_Dock_Area_Right, 0);
            this.Controls.SetChildIndex(this._FrmBase_Toolbars_Dock_Area_Left, 0);
            this.Controls.SetChildIndex(this.FrmBase_Fill_Panel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.BindingMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTabControl)).EndInit();
            this.uTabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UtbmMain)).EndInit();
            this.FrmBase_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabControl uTabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager UtbmMain;
        private System.Windows.Forms.Panel FrmBase_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _FrmBase_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _FrmBase_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _FrmBase_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _FrmBase_Toolbars_Dock_Area_Top;
    }
}