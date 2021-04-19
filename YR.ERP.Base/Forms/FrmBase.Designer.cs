namespace YR.ERP.Base.Forms
{
    partial class FrmBase
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
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel1 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel2 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel3 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel4 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel5 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            this.UsbButtom = new Infragistics.Win.UltraWinStatusBar.UltraStatusBar();
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).BeginInit();
            this.SuspendLayout();
            // 
            // UsbButtom
            // 
            this.UsbButtom.Location = new System.Drawing.Point(0, 485);
            this.UsbButtom.Name = "UsbButtom";
            this.UsbButtom.Padding = new Infragistics.Win.UltraWinStatusBar.UIElementMargins(0, 0, 0, 0);
            ultraStatusPanel1.Key = "Status1";
            ultraStatusPanel1.ProgressBarInfo.Maximum = 400;
            ultraStatusPanel1.ProgressBarInfo.Style = Infragistics.Win.UltraWinProgressBar.ProgressBarStyle.Office2007Continuous;
            ultraStatusPanel1.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Spring;
            ultraStatusPanel2.Key = "Status2";
            ultraStatusPanel2.ProgressBarInfo.Maximum = 400;
            ultraStatusPanel2.ProgressBarInfo.Minimum = 100;
            ultraStatusPanel2.ProgressBarInfo.Value = 100;
            ultraStatusPanel2.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Spring;
            ultraStatusPanel3.Key = "Security";
            ultraStatusPanel3.Width = 20;
            ultraStatusPanel4.Key = "KeyStateNum";
            ultraStatusPanel4.Style = Infragistics.Win.UltraWinStatusBar.PanelStyle.KeyState;
            ultraStatusPanel4.Width = 40;
            ultraStatusPanel5.Key = "KeyStateCaps";
            ultraStatusPanel5.KeyStateInfo.Key = Infragistics.Win.UltraWinStatusBar.KeyState.CapsLock;
            ultraStatusPanel5.Style = Infragistics.Win.UltraWinStatusBar.PanelStyle.KeyState;
            ultraStatusPanel5.Width = 40;
            this.UsbButtom.Panels.AddRange(new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel[] {
            ultraStatusPanel1,
            ultraStatusPanel2,
            ultraStatusPanel3,
            ultraStatusPanel4,
            ultraStatusPanel5});
            this.UsbButtom.Size = new System.Drawing.Size(750, 18);
            this.UsbButtom.TabIndex = 0;
            // 
            // FrmBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 503);
            this.Controls.Add(this.UsbButtom);
            this.Name = "FrmBase";
            this.Text = "FrmBase";
            this.Load += new System.EventHandler(this.FrmBase_Load);
            ((System.ComponentModel.ISupportInitialize)(this.UsbButtom)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected internal Infragistics.Win.UltraWinStatusBar.UltraStatusBar UsbButtom;

    }
}