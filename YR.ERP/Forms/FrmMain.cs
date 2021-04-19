/* 程式名稱: FrmReportBase.cs
   系統代號: 
   作　　者: Allen
   描　　述: 做為MDI父視窗使用
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YR.ERP.Shared;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinTree;
using YR.Util;
using System.Threading;
using Infragistics.Win.UltraWinTabbedMdi;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using YR.ERP.Base.Forms;

namespace YR.ERP.Forms
{
    public partial class FrmMain : YR.ERP.Base.Forms.FrmBase
    {
        #region Property
        private string m_strSecConString = GlobalVar.SQLCA_SecConSTR;
        ImageList ImgList = YR.Util.GlobalPictuer.LoadProgramListImage();

        public Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager ultraTabbedMdiManager1;
        #endregion

        #region 建構子
        public FrmMain()
        {
            InitializeComponent();
        }
        #endregion
        
        #region FrmMain_Load
        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.Shown += FrmMain_Shown;

            WfShowStatusBar(false);
            WfIniUltraTabbedMdiManager(ultraTabbedMdiManager1);
            this.WindowState = FormWindowState.Maximized;
            //FrmProgramList frmPgm = new FrmProgramList();
            //frmPgm.MdiParent = this;
            //frmPgm.FormBorderStyle = FormBorderStyle.None;
            //frmPgm.Dock = DockStyle.Fill;
            //frmPgm.WindowState = FormWindowState.Maximized;
            //frmPgm.BringToFront();

            //frmPgm.Show();

            //if (frmPgm.DialogResult == DialogResult.Cancel)
            //    this.Close();
            //else
            //{
            //    this.WindowState = FormWindowState.Maximized;
            //}
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            try
            {
                FrmProgramList frmPgm = new FrmProgramList();
                frmPgm.MdiParent = this;
                frmPgm.FormBorderStyle = FormBorderStyle.None;
                frmPgm.Dock = DockStyle.Fill;
                frmPgm.WindowState = FormWindowState.Maximized;
                frmPgm.BringToFront();

                frmPgm.Show();

                if (frmPgm.DialogResult == DialogResult.Cancel)
                    this.Close();
                //else
                //{
                //    this.WindowState = FormWindowState.Maximized;
                //}

            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
                this.Close();
            }

        }
        #endregion

        #region WfIniUltraTabbedMdiManager
        private void WfIniUltraTabbedMdiManager(UltraTabbedMdiManager pUtmm)
        {
            try
            {
                //pUtmm = new Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager(this.components);
                pUtmm = new Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager();
                pUtmm.MdiParent = this;
                pUtmm.InitializeTab += new Infragistics.Win.UltraWinTabbedMdi.MdiTabEventHandler(this.ultraTabbedMdiManager1_InitializeTab);
                pUtmm.TabActivated += new MdiTabEventHandler(this.ultraTabbedMdiManager1_TabActivated);

                pUtmm.ViewStyle = Infragistics.Win.UltraWinTabbedMdi.ViewStyle.Office2007;
                pUtmm.UseAppStyling = true;

                pUtmm.AllowNestedTabGroups = DefaultableBoolean.False;
                pUtmm.TabNavigationMode = MdiTabNavigationMode.VisibleOrder;
                pUtmm.AllowHorizontalTabGroups = false;
                pUtmm.AllowVerticalTabGroups = false;
                pUtmm.TabGroupSettings.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.Default;
                pUtmm.TabGroupSettings.TabStyle = Infragistics.Win.UltraWinTabs.TabStyle.Office2010Ribbon;
                pUtmm.TabGroupSettings.CloseButtonLocation = Infragistics.Win.UltraWinTabs.TabCloseButtonLocation.Tab;
                pUtmm.TabGroupSettings.ShowTabListButton = DefaultableBoolean.True;
                //pUtmm.TabGroupSettings.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.LeftTop;
                //pUtmm.TabGroupSettings.TextOrientation = Infragistics.Win.UltraWinTabs.TextOrientation.Horizontal;

                //pUtmm.TabSettings.AllowDrag = MdiTabDragStyle.None;
                pUtmm.TabSettings.CloseButtonVisibility = Infragistics.Win.UltraWinTabs.TabCloseButtonVisibility.WhenSelectedOrHotTracked;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ultraTabbedMdiManager1_InitializeTab
        private void ultraTabbedMdiManager1_InitializeTab(object sender, Infragistics.Win.UltraWinTabbedMdi.MdiTabEventArgs e)
        {          
            string frmName;
            try
            {
                //frmName = e.Tab.Form.Name;
                //20170117 frmGridEditBase繼承的表單都有問題帶不出來 改為帶Text
                frmName = e.Tab.Form.Text;
                frmName = frmName.Replace("Frm", "");
                if (frmName.ToUpper() == "PROGRAMLIST")
                {
                    e.Tab.Settings.CloseButtonVisibility = Infragistics.Win.UltraWinTabs.TabCloseButtonVisibility.Never;
                    frmName = "主選單";
                }
                
                e.Tab.Text = frmName;   //修改駐列tab 的名稱               
                
                //e.Tab.Form.Tag = e.Tab;
                //e.Tab.ToolTip = e.Tab.Form.Text;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // ultraTabbedMdiManager1_InitializeTab

        #region ultraTabbedMdiManager1_TabActivated
        private void ultraTabbedMdiManager1_TabActivated(object sender, Infragistics.Win.UltraWinTabbedMdi.MdiTabEventArgs e)
        {
            try
            {
                this.Text = (e.Tab.Form as YR.ERP.Base.Forms.FrmBase).Title;
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.ToString());
            }

        }
        #endregion
    }

}
