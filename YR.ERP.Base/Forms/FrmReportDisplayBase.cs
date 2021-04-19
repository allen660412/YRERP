using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Stimulsoft.Report;
using Infragistics.Win.UltraWinToolbars;
using YR.ERP.Shared;
using Infragistics.Win;
using YR.Util;
using Stimulsoft.Report.Export;
using System.IO;

namespace YR.ERP.Base.Forms
{
    public partial class FrmReportDisplayBase : FrmBase
    {
        private Stimulsoft.Report.StiReport stiReport;

        public FrmReportDisplayBase()
        {
            InitializeComponent();
        }

        public FrmReportDisplayBase(StiReport report)
        {
            InitializeComponent();
            this.stiReport = report;
            this.stiViewerControl1.Report = stiReport;

        }
        

        #region FrmReportDisplayBase_Load
        private void FrmReportDisplayBase_Load(object sender, EventArgs e)
        {

            //WfIniToolBarUI();
            if (stiReport != null)
            {
                //this.stiViewerControl1.Report = stiReport;
                this.stiViewerControl1.Report.Render(true);
                this.Text = this.stiViewerControl1.Report.ReportName;
                //stiViewerControl1.ToolBar.Items[0]
                stiViewerControl1.ToolBar.ItemClick += ToolBar_ItemClick;
                //stiViewerControl1.ToolBar.Items.
            }
            

            //stiReport.SaveDocument();
        }

        void ToolBar_ItemClick(object sender, EventArgs e)
        {
            string btName;
            btName = (sender as Stimulsoft.Controls.Win.DotNetBar.BaseItem).Name;
            switch(btName.ToLower())
            {
                case "tbclose":
                    this.Close();
                    break;
            }
        }

        void ToolBar_Click(object sender, EventArgs e)
        {
           
        }

        #endregion

        #region WfIniToolBarUI 初始化表單工具列--設定圖案及熱鍵
        protected virtual void WfIniToolBarUI()
        {
            ImageList ilLarge = new ImageList();
            string buttonKey;
            try
            {
                UtbmMain.Style = GetStyleLibrary.UltraWinToolBarDiplayStyle;

                UtbmMain.UseAppStyling = false;
                UtbmMain.Office2007UICompatibility = false;
                UtbmMain.UseOsThemes = DefaultableBoolean.True;
                UtbmMain.Style = ToolbarStyle.Office2013;
                UtbmMain.Ribbon.QuickAccessToolbar.Visible = false;

                UtbmMain.Ribbon.FileMenuStyle = FileMenuStyle.None;
                UtbmMain.Ribbon.CaptionAreaAppearance.BackColor = Color.FromArgb(210, 210, 210);
                UtbmMain.Ribbon.FileMenuButtonAppearance.BackColor = Color.FromArgb(67, 168, 152);
                UtbmMain.Ribbon.FileMenuButtonAppearance.BackGradientStyle = GradientStyle.None;

                UtbmMain.Ribbon.GroupSettings.CaptionAppearance.FontData.SizeInPoints = 8.5f;
                UtbmMain.Ribbon.GroupSettings.CaptionAppearance.ForeColor = Color.White;

                UtbmMain.Ribbon.GroupSettings.HotTrackAppearance.BackColor = Color.FromArgb(66, 126, 123);
                UtbmMain.Ribbon.GroupSettings.HotTrackAppearance.BackGradientStyle = GradientStyle.None;
                UtbmMain.Ribbon.GroupSettings.HotTrackAppearance.BorderAlpha = Alpha.Opaque;
                UtbmMain.Ribbon.GroupSettings.HotTrackAppearance.BorderColor = Color.FromArgb(67, 168, 152);
                UtbmMain.Ribbon.GroupSettings.HotTrackAppearance.ForeColor = Color.White;
                UtbmMain.Ribbon.GroupSettings.ToolAppearance.ForeColor = Color.FromArgb(179, 187, 191);

                UtbmMain.Ribbon.TabAreaAppearance.BackColor = Color.FromArgb(67, 168, 152);

                UtbmMain.Ribbon.TabSettings.Appearance.BackColor = Color.FromArgb(66, 84, 94);
                UtbmMain.Ribbon.TabSettings.Appearance.BackGradientStyle = GradientStyle.None;
                UtbmMain.Ribbon.TabSettings.Appearance.BorderAlpha = Alpha.Transparent;
                UtbmMain.Ribbon.TabSettings.Appearance.ForeColor = Color.White;

                UtbmMain.Ribbon.TabSettings.SelectedAppearance.BackColor = Color.FromArgb(66, 84, 94);
                UtbmMain.Ribbon.TabSettings.SelectedAppearance.BorderAlpha = Alpha.Transparent;

                UtbmMain.Ribbon.TabSettings.TabItemAppearance.BackColor = Color.FromArgb(67, 168, 152);


                UtbmMain.MdiMergeable = false;

                ilLarge = GlobalPictuer.LoadToolBarImage();
                if (ilLarge == null)
                    return;
                UtbmMain.ImageListLarge = ilLarge;
                UltraToolbar mainMenuBar = UtbmMain.Toolbars.AddToolbar("MainMenuBar");

                buttonKey = "BtPrint";
                var BtPrint = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtPrint);
                mainMenuBar.Tools.AddTool(buttonKey);
                mainMenuBar.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Default;
                //BtPrint.SharedProps.AppearancesSmall.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_INSERT];
                //BtPrint.SharedPropsInternal.Shortcut = Shortcut.CtrlI;
                BtPrint.SharedProps.Caption = "列 印";
                BtPrint.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;

                buttonKey = "BtExportPdf";
                var BtExportPdf = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtExportPdf);
                mainMenuBar.Tools.AddTool(buttonKey);
                mainMenuBar.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Default;
                //BtPrint.SharedProps.AppearancesSmall.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_INSERT];
                //BtExportPdf.SharedPropsInternal.Shortcut = Shortcut.CtrlI;
                BtExportPdf.SharedProps.Caption = "匯 出";
                BtExportPdf.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region UtbmMain_ToolClick-暫不使用
        private void UtbmMain_ToolClick(object sender, ToolClickEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                switch (e.Tool.Key.ToLower())
                {
                    case "btprint":
                        //stiReport.Print(true);
                        stiViewerControl1.InvokeClickPrintButton();
                        break;
                    case "btexportpdf":
                        //Stimulsoft.Report.SaveLoad.StiDocumentPageSLService serve = new Stimulsoft.Report.SaveLoad.StiDocumentPageSLService();
                        //StiPdfExportService pdfExport = new StiPdfExportService();

                        //StiPdfExportSettings pdfSettings = new StiPdfExportSettings();
                        ////pdfSettings.

                        //stiReport.ExportDocument(StiExportFormat.Pdf, "");

                        StiPdfExportService service = new StiPdfExportService();
                        StiPdfExportSettings settings = new StiPdfExportSettings();



                        MemoryStream stream = new MemoryStream();
                        service.ExportPdf(stiReport, stream, settings);
                        service.Export(stiReport, "test.pdf");
                        break;
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        } 
        #endregion


    }
}
