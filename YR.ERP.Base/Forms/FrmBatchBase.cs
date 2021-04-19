/* 程式名稱: FrmBatchBase.cs
   系統代號: 
   作    者: Allen
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
using System.Windows.Forms;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using YR.Util;
using YR.ERP.DAL.YRModel;
using Infragistics.Win.UltraWinEditors;
using YR.Util.Controls;
using System.Data.SqlClient;
using System.IO;

namespace YR.ERP.Base.Forms
{
    public partial class FrmBatchBase : YR.ERP.Base.Forms.FrmBase
    {
        #region Property
        public BatchInfo TabMaster = new BatchInfo();     // 保存報表相關物件    
        public AdmBLL BoMaster                              // BOMASTER 為主表的 business object
        {
            get { return TabMaster.BoBasic; }
            set { TabMaster.BoBasic = value; }
        }
        //程式編號
        protected string StrFormID = "";

        protected string StrQueryWhere { get; set; }        //查詢條件字串         
        protected string StrQueryWhereAppend { get; set; }  //查詢條件字串-做為被呼叫時由其他視窗載入,前面需加 AND EX:AND ica01='xxx' 

        protected bool IsChanged = false;                   //是否有修改或變更
        protected bool IsInSaveCancle = false;              //按下存檔取消時用來避開控制項驗證
        protected bool IsInCRUDIni = false;                 //按下新增,修改,刪除,查詢時,避開控制項驗證
        protected bool IsInFormLoading = true;              //表單初始化時
        protected bool IsItemchkValid = true;               //檢驗資料驗證是否正常
        protected bool IsInItemchecking = false;            //處於validating行為中
        protected DataRow DrMaster;                         //主表目前 active 的 datarow

        //protected UltraGrid UGrid_Master = new UltraGrid();
        #endregion Property

        #region 建構子
        public FrmBatchBase()
        {
            InitializeComponent();
        }
        #endregion

        #region FrmBatchBase_Load
        private void FrmBatchBase_Load(object sender, EventArgs e)
        {
            try
            {
                IsInFormLoading = true;
                WfIniTabMasterInfo();
                if (WfInitialForm() == false)
                {
                    if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)   //避免desingmode 一直出現錯誤訊息
                    {
                        IniSuccess = false;
                        return;
                    }
                }

                if (WfAfterInitialForm() == false)
                {
                    if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)   //避免desingmode 一直出現錯誤訊息
                    {
                        IniSuccess = false;
                        return;
                    }
                }

                if (AdoModel != null)
                {
                    WfSetFormTitle(AdoModel.ado01, AdoModel.ado02, LoginInfo);
                }
                this.Shown += FrmBatchBase_Shown;
            }
            catch (Exception ex)
            {
                IniSuccess = false;
                WfShowErrorMsg(ex.Message);
            }
            finally
            {
                IsInFormLoading = false;
            }

        }

        private void FrmBatchBase_Shown(object sender, EventArgs e)
        {
            if (TabMaster.IsAutoExecuted)
            {
                WfToolbarExecute();
            }
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected virtual void WfIniTabMasterInfo()
        {
            //設定master l的 tabinfo
        }
        #endregion

        #region WfInitialForm
        private bool WfInitialForm()
        {
            try
            {
                if (WfSetVar() == false)
                    return false;

                WfIniToolBarUI();
                if (WfIniMaster() == false)
                    return false;
                WfBindMasterByTag(this.PnlFillMaster, TabMaster.AzaTbList, this.DateFormat);
                WfBindMaster(); //取得combox的顯示來源                
                WfDisplayMode();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean WfSetVar()
        {
            // 繼承的表單要覆寫, 更改參數值            
            /*
            this.strFormID = "XXX";
             */

            return true;
        }
        #endregion

        /******************* ToolBar 工具列事件 ***********************/
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
                //UtbmMain.Ribbon.GroupSettings.ToolAppearance.ImageAlpha = Alpha.UseAlphaLevel;
                //UtbmMain.Ribbon.GroupSettings.ToolAppearance.AlphaLevel = 100;
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
                //UtbmMain.Style = GetStyleLibrary.UltraWinToolBarDiplayStyle;
                //UtbmMain.UseAppStyling = false;

                //if (this.IsMdiChild)
                //{
                //    UtbmMain.Office2007UICompatibility = false;
                //    UtbmMain.MdiMergeable = false;
                //    UtbmMain.Ribbon.QuickAccessToolbar.Visible = false;
                //}
                //else
                //{
                //    UtbmMain.Office2007UICompatibility = false;
                //    UtbmMain.MdiMergeable = false;
                //    UtbmMain.Ribbon.QuickAccessToolbar.Visible = false;
                //}

                ilLarge = GlobalPictuer.LoadToolBarImage();
                if (ilLarge == null)
                    return;
                UtbmMain.ImageListLarge = ilLarge;

                #region 產生RibbonTab/及Group
                RibbonTab RtData = new RibbonTab("RtData", "資料");
                //RibbonTab RtReport = new RibbonTab("RtReport", "報表");
                UtbmMain.Ribbon.Tabs.AddRange(new RibbonTab[] { RtData });

                //RibbonGroup RibgCrud = new RibbonGroup("RibgCrud", "資料存取");
                RibbonGroup RibgNav = new RibbonGroup("RibgNav", "導覽");
                RibbonGroup RibgDecide = new RibbonGroup("RibgDecide", "處理");
                RibbonGroup RibgExternal = new RibbonGroup("RibgExternal", "其他功能"); //視情況使用 放報表及Action
                //RtData.Groups.AddRange(new RibbonGroup[] { RibgCrud, RibgNav, RibgDecide });
                RtData.Groups.AddRange(new RibbonGroup[] { RibgDecide, RibgNav, RibgExternal });
                #endregion

                #region RtData/RibgDecide 相關按鈕
                buttonKey = "BtOk";
                var BtOk = new ButtonTool(buttonKey);
                //var BtOk = new ControlContainerTool(lsBtKey);
                UtbmMain.Tools.Add(BtOk);
                RibgDecide.Tools.AddTool(buttonKey);
                RibgDecide.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtOk.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_OK];
                BtOk.SharedProps.Caption = "確認";

                buttonKey = "BtCancel";
                var BtCancel = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtCancel);
                RibgDecide.Tools.AddTool(buttonKey);
                RibgDecide.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtCancel.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_CANCEL];
                BtCancel.SharedProps.Caption = "取消";

                buttonKey = "BtClean";
                var BtClean = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtClean);
                RibgDecide.Tools.AddTool(buttonKey);
                RibgDecide.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtClean.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_ERASER];
                BtClean.SharedProps.Caption = "清除";
                #endregion

                #region RtData/RibgNav 相關按鈕
                if (IsMdiChild) //Mdi子視窗才有此功能
                {
                    buttonKey = "BtHome";
                    var BtHome = new ButtonTool(buttonKey);
                    UtbmMain.Tools.Add(BtHome);
                    RibgNav.Tools.AddTool(buttonKey);
                    RibgNav.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                    //BtHome.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["home_32"];
                    BtHome.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_HOME];
                    BtHome.SharedPropsInternal.Shortcut = Shortcut.CtrlH;
                    BtHome.SharedProps.Caption = "主選單";

                    buttonKey = "BtFrmNavigator";
                    var BtFrmNavigateor = new ButtonTool(buttonKey);
                    UtbmMain.Tools.Add(BtFrmNavigateor);
                    RibgNav.Tools.AddTool(buttonKey);
                    RibgNav.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                    //BtFrmNavigateor.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["form_navgiator_32"];
                    BtFrmNavigateor.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_NAVGIATOR];
                    BtFrmNavigateor.SharedPropsInternal.Shortcut = Shortcut.CtrlW;
                    BtFrmNavigateor.SharedProps.Caption = "切換視窗";
                }
                #endregion

                #region RibgExternal 其他功能
                buttonKey = "BtExit";
                var BtExit = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtExit);
                RibgExternal.Tools.AddTool(buttonKey);
                RibgExternal.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtExit.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_EXIT];
                //BtExit.SharedPropsInternal.Shortcut = Shortcut.CtrlF;
                BtExit.SharedProps.Caption = "離 開"; 
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region UtbmMain_ToolClick 工具列 click 事件
        protected virtual void UtbmMain_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                WfCleanBottomMsg();
                WfCleanErrorProvider();
                switch (e.Tool.Key.ToLower())
                {
                    case "btok":
                        WfFireControlValidated(this.ActiveControl);
                        //各驗證控製項會將 isItemchkValid設為true
                        if (IsItemchkValid == false)
                            return;
                        if (WfToolbarExecute() == false)
                            return;

                        break;

                    case "btcancel":
                        this.Close();
                        break;

                    case "btclean":
                        TabMaster.DtSource.Clear();
                        DrMaster = TabMaster.DtSource.NewRow();
                        TabMaster.DtSource.Rows.Add(DrMaster);
                        WfSetMasterRowDefault(DrMaster);
                        BindingMaster.EndEdit();
                        break;
                    case "bthome":
                        var frmMenu = Application.OpenForms
                                                .Cast<System.Windows.Forms.Form>()
                                                .Where(p => p.Name.ToLower() == "frmmenu" || p.Name.ToLower() == "frmprogramlist") //mdi或sdi
                                                .First()
                                                ;
                        if (frmMenu != null)
                        {
                            frmMenu.BringToFront();
                            if (!frmMenu.IsMdiChild)
                                frmMenu.WindowState = FormWindowState.Maximized;
                        }
                        break;
                    case "btfrmnavigator":
                        WfOpenFrmNavigator();
                        break;

                    case "btexit":
                        this.Close();
                        break;
                }

            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
            finally
            {
                IsInSaveCancle = false;
                IsInCRUDIni = false;
                this.Cursor = Cursors.Default;
            }

        }
        #endregion

        #region override ProcessCmdKey 註冊表單熱鍵
        //waitfix:還未考慮跳過權限檢查的問題
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            ToolClickEventArgs eTool;
            ToolBase utb;
            try
            {
                //var control = FindFocusedControl(this.MdiParent);
                if (keyData == (Keys.Control | Keys.Enter))
                {
                    utb = UtbmMain.Tools["BtOk"];
                    eTool = new ToolClickEventArgs(utb, null);
                    UtbmMain_ToolClick(UtbmMain, eTool);
                    return true;
                }

                if (keyData == (Keys.Escape))
                {
                    this.Close();
                    return true;
                }

                //if (keyData == (Keys.F1))
                //{
                //    utb = UtbmMain.Tools["btribbonvisible"];
                //    eTool = new ToolClickEventArgs(utb, null);
                //    UtbmMain_ToolClick(UtbmMain, eTool);
                //    return true;
                //}

                ////表單若無可停註控制項時,會切換mdi tab ,避免引發先做以下處理
                //if (keyData == (Keys.Shift | Keys.Tab) || keyData == (Keys.Tab))
                //{
                //    return true;
                //}
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        /**********************  新增/查詢/拷貝/存檔/刪除 相關 Function **********************/
        #region WfIniMaster() 設定表單主要的資料來源
        protected virtual Boolean WfIniMaster()
        {
            try
            {
                // 以 FormID 由 Pu_pgm 中取得表單設定
                #region 初始化 PU_BUSOBJ
                if (GlobalVar.PU_BUSOBJ != null)
                {
                    this.BoSecurity = (YR.ERP.BLL.MSSQL.AdmBLL)GlobalVar.PU_BUSOBJ;
                }
                else
                {
                    if (GlobalVar.Adm_DAO != null)
                    {
                        this.BoSecurity = new AdmBLL((YR.ERP.DAL.ERP_MSSQLDAL)GlobalVar.Adm_DAO);
                    }
                    else
                    {
                        //this.BoSecurity = new AdmBLL();
                    }
                }
                #endregion

                if (BoSecurity == null)
                    return false;
                this.AdoModel = BoSecurity.OfGetAdoModel(StrFormID);
                if (AdoModel == null)
                    return false;
                this.TabMaster.TargetTable = AdoModel.ado05;
                this.TabMaster.TargetColumn = "*";
                this.TabMaster.ViewTable = AdoModel.ado06;
                if (TabMaster.ViewTable == "") { TabMaster.ViewTable = TabMaster.TargetTable; }

                if (BoSecurity == null)
                {
                    throw new Exception("物件BoSecurity,尚未初始化!");
                    //return false;
                }
                this.TabMaster.AzaTbList = BoSecurity.OfGetAzaModels(TabMaster.ViewTable);
                this.TabMaster.AddTbModel = BoSecurity.OfGetAddModel(LoginInfo.UserRole, StrFormID);

                // 建立 Form Master 的 Business Object : boBasic
                this.WfCreateBoBasic();
                //取得baa_tb共用參數代碼
                if (BaaModel == null)
                {
                    using (var boBas = new BasBLL(BoMaster.OfGetConntion()))
                    {
                        if (BoMaster.TRAN != null)
                            boBas.TRAN = BoMaster.TRAN;
                        BaaModel = boBas.OfGetBaaModel();
                    }
                }

                #region 取得日期格式
                var baa01KvpList = new BasBLL().OfGetBaa01KVPList();//取得日期格式
                this.DateFormat = baa01KvpList.Where(x => x.Key == BaaModel.baa01)
                                .Select(x => x.Value)
                                .FirstOrDefault()
                                ;

                if (GlobalFn.varIsNull(DateFormat))
                    DateFormat = "yyyy/MM/dd";
                else //取得格式會含有 . ex:1.yyyy/MM/dd 要剔除
                {
                    if (DateFormat.IndexOf('.') >= 0)
                    {
                        var dotPosition = DateFormat.IndexOf('.');
                        DateFormat = DateFormat.Substring(dotPosition + 1, DateFormat.Length - dotPosition - 1);
                    }
                }
                #endregion

                // 先取得空的資料表
                if (this.TabMaster.ViewTable != "")
                {
                    this.TabMaster.DtSource = this.TabMaster.BoBasic.OfSelect(" WHERE 1=2 ");
                    TabMaster.DtSource.Rows.Add(TabMaster.DtSource.NewRow());

                    this.WfSetMasterDatasource(this.TabMaster.DtSource);
                    this.BindingMaster.MoveFirst();
                    DrMaster = TabMaster.DtSource.Rows[0];
                    WfSetMasterRowDefault(DrMaster);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        /// <summary>
        /// 建立 business object 
        /// </summary>
        /// <param name="strDALClass">DAL 類別</param>
        /// <param name="m_strTargetTable">存檔的表</param>
        /// <param name="m_strTargetColumn">查詢的 View</param>
        /// <returns></returns>
        protected virtual void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
            return;
        }
        #endregion

        #region WfSetMasterDatasource(DataTable dt) : 設定 bindingMaster 的 datasouce
        /// <summary>
        /// 設定 bindingMaster 的 datasouce 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        protected virtual Boolean WfSetMasterDatasource(DataTable dt)
        {
            this.TabMaster.DtSource = dt;
            this.BindingMaster.DataSource = dt;
            //this.UGrid_Master.DataSource = BindingMaster;

            // 設定 prefix, 用於 datacolumn_changing 檢查欄位長度
            dt.Prefix = this.TabMaster.ViewTable;
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示
        protected override Boolean WfDisplayMode()
        {
            try
            {
                WfSetControlsReadOnlyRecursion(this.PnlFillMaster, false);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMasterRowDefault(DataRow pDr) 設定MasterRow的初始值
        protected virtual bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfToolbarExecute()
        protected virtual Boolean WfToolbarExecute()
        {
            //批次功能的transaction 由各表單自行處理
            BindingMaster.EndEdit();
            try
            {
                this.errorProvider.Clear();
                if (!TabMaster.IsAutoExecuted)  //不做條件檢查   --主要由他窗引用
                {
                    if (this.WfFormCheck() == false)
                    {
                        //WfRollback();
                        return false;
                    }

                    if (this.WfAfterFormCheck() == false)
                    {
                        //WfRollback();
                        return false;
                    }
                }

                //產生資料來源
                if (this.WfExecute() == false)
                {
                    //WfRollback();
                    return false;
                }

                if (WfExecuteEnd() == false)
                    return false;

                if (TabMaster.IsCloseAfterExecuted)
                    this.Close();

                WfShowBottomStatusMsg("執行作業成功!");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Number == 10054)
                {
                    WfShowErrorMsg("與資料庫連線已斷線，作業即將關閉!");
                    this.Close();
                }
                else
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //this.Cursor = Cursors.Default; //改到工具列處理
            }

            return true;
        }
        #endregion

        #region WfFormCheck() 執行報表前檢查
        protected virtual bool WfFormCheck()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAfterFormCheck() 執行報表後處理
        protected virtual bool WfAfterFormCheck()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfExecute 批次執行開始
        protected virtual bool WfExecute()
        {
            try
            {

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfExecuteEnd 批次執行結束
        protected virtual bool WfExecuteEnd()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfGetSecurityString 取得查詢連線字串
        protected virtual string WfGetSecurityString()
        {
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                if (AdoModel == null || TabMaster.AddTbModel == null)
                    return "";
                if (TabMaster.AddTbModel.add10.ToUpper() == "Y")//勾選他組權限時
                    return "";
                else if (TabMaster.AddTbModel.add07.ToUpper() == "Y")//勾選群組權限時
                {
                    if (!GlobalFn.varIsNull(TabMaster.UserColumn) && !GlobalFn.varIsNull(TabMaster.GroupColumn))   //群組與個人都勾選
                    {
                        sbSql.AppendLine("AND (");
                        sbSql.AppendLine(string.Format("   {0}='{1}'", TabMaster.UserColumn, LoginInfo.UserNo));
                        sbSql.AppendLine("  OR EXISTS (");
                        sbSql.AppendLine(string.Format("        SELECT 1 FROM ade_tb "));
                        sbSql.AppendLine(string.Format("        WHERE ade01={0} AND ade03 LIKE '{1}%'", TabMaster.GroupColumn, LoginInfo.GroupLevel));
                        sbSql.AppendLine("  )");
                        sbSql.AppendLine(")");
                    }
                    return sbSql.ToString();
                }
                else  //他組與群繧都未勾時,只能查個人
                {
                    if (!GlobalFn.varIsNull(TabMaster.UserColumn))   //群組與個人都勾選
                    {
                        sbSql.AppendLine(string.Format("AND {0}='{1}'", TabMaster.UserColumn, LoginInfo.UserNo));
                    }
                    return sbSql.ToString();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /******************* 其它 Control 事件 ***********************/
        #region Control_Enter(object sender, EventArgs e) 所有控制項進入(ENTER)事件(未包含GRID)
        protected internal override void Control_Enter(object sender, EventArgs e)
        {
            string msg;
            object tag;
            string aza03;
            string typeName;
            try
            {
                if (this.IsInItemchecking == true)  //避免驗證時又觸發事件(目前作用時機為PICK開窗時會造成...)
                    return;
                WfGetOldValue(sender as Control);
                typeName = sender.GetType().ToString();

                if (TabMaster.AzaTbList == null)
                    return;

                tag = (sender as Control).Tag;
                if (tag == null)
                    return;
                aza03 = GlobalFn.isNullRet(tag, "");

                var azaModel = TabMaster.AzaTbList.Where(p => p.aza03 == aza03).FirstOrDefault();

                if (azaModel == null || azaModel.aza03 == null)
                    return;

                msg = azaModel.aza04 + "[" + azaModel.aza03 + "]";

                //這裡是為了讓編輯模式時,讓小數點數字若為0時,不顯示 ex:123.000==>123
                if (typeName == "Infragistics.Win.UltraWinEditors.UltraTextEditor")
                {
                    if (azaModel.aza08.ToLower() == "numeric" && (sender as UltraTextEditor).Value != DBNull.Value)
                    {
                        WfSetControlEditNumeric(sender as UltraTextEditor);
                    }
                }

#if DEBUG
                msg += "  " + azaModel.aza08;
                switch (azaModel.aza08.ToLower())
                {
                    case "nvarchar":
                        msg += "(" + azaModel.aza09 + ")";
                        break;
                    case "numeric":
                        msg += "(" + azaModel.aza10.ToString() + "." + azaModel.aza11.ToString() + ")";
                        break;
                }
#endif
                WfShowBottomHelpMsg(msg);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 相關控制項Validating事件 方法
        #region UltraTextEditor_Validating
        protected internal override void UltraTextEditor_Validating(object sender, CancelEventArgs e)
        {
            string colName = "";
            string dbValue = "";
            UltraTextEditor control = sender as UltraTextEditor;
            try
            {
                if (IsInSaveCancle == true || IsInCRUDIni == true)
                    return;
                if (IsInFormLoading == true)
                    return;
                
                //if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.NA)
                //    return;
                
                WfCleanBottomMsg();
                IsInItemchecking = true;
                IsItemchkValid = true;
                colName = control.Tag.ToString();
                dbValue = DrMaster[colName] == null ? "" : DrMaster[colName].ToString();
                //if (IsInButtonClick == true || control.Value != OldValue)
                
                //若為數字欄位,將format去掉
                if (control.Tag != null && !GlobalFn.varIsNull(control.Tag))
                {
                    var azaModel = TabMaster.AzaTbList.Where(p => p.aza03.ToLower() == colName.ToLower())
                                        .FirstOrDefault();                    
                    if (azaModel.aza08 == "int" || azaModel.aza08 == "numeric")
                    {
                        decimal parseDecimal = 0;
                        if (decimal.TryParse(GlobalFn.isNullRet(control.Value, "0"), out parseDecimal))
                        {
                            control.Value = parseDecimal;
                        }
                    }
                }
                
                if (control.Value != OldValue)
                {
                    //WfSetBllTransaction();
                    var itemCheckInfo = new ItemCheckInfo();
                    itemCheckInfo.Row = DrMaster;
                    itemCheckInfo.Value = control.Value;
                    itemCheckInfo.Column = colName;
                    
                    if (WfItemCheck(sender, itemCheckInfo) == false)
                    {
                        e.Cancel = true;
                        IsItemchkValid = false;
                        control.Value = OldValue;
                    }
                    else
                    {
                        IsChanged = true;
                        DrMaster.EndEdit();
                    }
                }
            }
            catch (Exception ex)
            {
                //if (IsInButtonClick == true)//由buttonclick觸發時,要拋例外事件來處理
                //    throw ex;
                
                e.Cancel = true;
                IsItemchkValid = false;
                control.Value = OldValue;
                WfShowErrorMsg(ex.ToString());
            }
            finally
            {
                //IsInButtonClick = false;
                IsInItemchecking = false;
            }
            //    string colName = "";
            //    string dbValue = "";
            //    UltraTextEditor control = sender as UltraTextEditor;
            //    try
            //    {
            //        if (IsInSaveCancle == true || IsInCRUDIni == true)
            //            return;
            //        if (IsInFormLoading == true)
            //            return;

            //        IsInItemchecking = true;
            //        IsItemchkValid = true;
            //        colName = control.Tag.ToString();
            //        dbValue = DRMASTER[colName] == null ? "" : DRMASTER[colName].ToString();
            //        //if (IsInButtonClick == true || control.Value != OldValue)
            //        if (control.Value != OldValue)
            //        {

            //            if (WfItemCheck(sender, colName, control.Value, DRMASTER) == false)
            //            {
            //                e.Cancel = true;
            //                IsItemchkValid = false;
            //                control.Value = OldValue;
            //            }
            //            else
            //            {
            //                IsChanged = true;
            //                DRMASTER.EndEdit();
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        e.Cancel = true;
            //        IsItemchkValid = false;
            //        control.Value = OldValue;
            //        WfShowMsg(ex.ToString());
            //    }
            //    finally
            //    {
            //        //IsInButtonClick = false;
            //        IsInItemchecking = false;
            //    }
        }
        #endregion

        #region UltraCombo_Validating
        protected internal override void UltraCombo_Validating(object sender, CancelEventArgs e)
        {
            string colName = "";
            string dbValue;
            string displayValue;
            UltraCombo control = sender as UltraCombo;
            try
            {
                if (IsInSaveCancle == true || IsInCRUDIni == true)
                    return;
                if (IsInFormLoading == true)
                    return;

                //if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.NA)
                //    return;

                WfCleanBottomMsg();
                IsItemchkValid = true;
                colName = control.Tag.ToString();
                dbValue = DrMaster[colName] == null ? "" : DrMaster[colName].ToString();
                displayValue = control.Value == null ? "" : control.Value.ToString();
                if (control.Value != OldValue)
                {
                    //WfSetBllTransaction();
                    var itemCheckInfo = new ItemCheckInfo();
                    itemCheckInfo.Row = DrMaster;
                    itemCheckInfo.Value = displayValue;
                    itemCheckInfo.Column = colName;

                    //if (WfItemCheck(sender, colName, displayValue, DRMASTER) == false)
                    if (WfItemCheck(sender, itemCheckInfo) == false)
                    {
                        e.Cancel = true;
                        IsItemchkValid = false;
                        //control.Value = OldValue;
                        if (OldValue == DBNull.Value)
                            control.Value = null;
                        else
                            control.Value = OldValue;
                    }
                    else
                    {
                        IsChanged = true;
                        DrMaster.EndEdit();
                    }
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                IsItemchkValid = false;
                control.Value = OldValue;
                WfShowErrorMsg(ex.ToString());
            }
            //string colName = "";
            //string dbValue;
            //string displayValue;
            //UltraCombo control = sender as UltraCombo;
            //try
            //{
            //    if (IsInSaveCancle == true || IsInCRUDIni == true)
            //        return;
            //    if (IsInFormLoading == true)
            //        return;

            //    IsItemchkValid = true;
            //    colName = control.Tag.ToString();
            //    dbValue = DrMaster[colName] == null ? "" : DrMaster[colName].ToString();
            //    displayValue = control.Value == null ? "" : control.Value.ToString();
            //    if (control.Value != OldValue)
            //    {
            //        if (WfItemCheck(sender, colName, displayValue, DrMaster) == false)
            //        {
            //            e.Cancel = true;
            //            IsItemchkValid = false;
            //            //control.Value = OldValue;
            //            if (OldValue == DBNull.Value)
            //                control.Value = null;
            //            else
            //                control.Value = OldValue;
            //        }
            //        else
            //        {
            //            IsChanged = true;
            //            DrMaster.EndEdit();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    e.Cancel = true;
            //    IsItemchkValid = false;
            //    control.Value = OldValue;
            //    WfShowMsg(ex.ToString());
            //}
        }
        #endregion

        #region UcCheckBox_Validating
        protected internal override void UcCheckBox_Validating(object sender, CancelEventArgs e)
        {
            string colName = "";
            string dbValue = "";
            //string displayValue = "";
            YR.Util.Controls.UcCheckBox control = sender as YR.Util.Controls.UcCheckBox;
            try
            {
                if (IsInSaveCancle == true || IsInCRUDIni == true)
                    return;
                if (IsInFormLoading == true)
                    return;

                //if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.NA)
                //    return;

                WfCleanBottomMsg();
                IsItemchkValid = true;
                colName = control.Tag.ToString();
                dbValue = DrMaster[colName] == null ? "" : DrMaster[colName].ToString();
                //displayValue = control.CheckValue == null ? "" : control.CheckValue.ToString();

                if (control.CheckValue != OldValue)
                {
                    //WfSetBllTransaction();
                    var itemCheckInfo = new ItemCheckInfo();
                    itemCheckInfo.Row = DrMaster;
                    itemCheckInfo.Value = control.CheckValue;
                    itemCheckInfo.Column = colName;

                    if (WfItemCheck(sender, itemCheckInfo) == false)
                    {
                        e.Cancel = true;
                        IsItemchkValid = false;
                        control.CheckValue = OldValue;
                    }
                    else
                    {
                        IsChanged = true;
                        DrMaster.EndEdit();
                    }
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                IsItemchkValid = false;
                control.CheckValue = OldValue;
                WfShowErrorMsg(ex.ToString());
            }
            //    string colName = "";
            //    string dbValue;
            //    string displayValue = "";
            //    YR.Util.Controls.UcCheckBox control = sender as YR.Util.Controls.UcCheckBox;
            //    try
            //    {
            //        if (IsInSaveCancle == true || IsInCRUDIni == true)
            //            return;
            //        if (IsInFormLoading == true)
            //            return;

            //        IsItemchkValid = true;
            //        colName = control.Tag.ToString();
            //        dbValue = DrMaster[colName] == null ? "" : DrMaster[colName].ToString();
            //        displayValue = control.CheckValue == null ? "" : control.CheckValue.ToString();
            //        if (displayValue != dbValue)
            //        {
            //            if (WfItemCheck(sender, colName, displayValue, DrMaster) == false)
            //            {
            //                e.Cancel = true;
            //                IsItemchkValid = false;
            //                control.CheckValue = OldValue;
            //            }
            //            else
            //            {
            //                IsChanged = true;
            //                DrMaster.EndEdit();
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        e.Cancel = true;
            //        IsItemchkValid = false;
            //        control.CheckValue = OldValue;
            //        WfShowMsg(ex.ToString());
            //    }
        }
        #endregion

        #region UltraGrid_BeforeExitEditMode
        protected internal override void UltraGrid_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {
            string colName = "";
            string dbValue;
            UltraGridCell uGridCell = null;
            DataRowView drvActive = null;
            DataRow drActive;
            try
            {

                //if (IsInGridButtonClick == true)        //避免驗證時又觸發事件(目前作用時機為PICK開窗時會造成...)
                //    return;
                if (IsInSaveCancle == true || IsInCRUDIni == true)
                    return;
                if (IsInFormLoading == true)
                    return;

                //if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.NA)
                //    return;

                WfCleanBottomMsg();
                IsInItemchecking = true;
                IsItemchkValid = true;
                uGridCell = (sender as UltraGrid).ActiveCell;
                colName = uGridCell.Column.Key;
                drvActive = (System.Data.DataRowView)uGridCell.Row.ListObject;
                if (drvActive == null)
                    return;

                (sender as UltraGrid).UpdateData();
                drActive = drvActive.Row;
                dbValue = GlobalFn.isNullRet(drActive[colName], "");

                if (!uGridCell.Value.Equals(OldValue))
                {
                    //WfSetBllTransaction();
                    var itemCheckInfo = new ItemCheckInfo();
                    itemCheckInfo.Row = drActive;
                    itemCheckInfo.Value = uGridCell.Value;
                    itemCheckInfo.Column = colName;
                    if (WfItemCheck(sender, itemCheckInfo) == false)
                    {
                        e.Cancel = true;
                        IsItemchkValid = false;

                        if (uGridCell.Column.Style == Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList)
                            uGridCell.Value = uGridCell.OriginalValue;    //null 問題
                        else
                        {
                            if (OldValue == DBNull.Value)
                            {
                                if (uGridCell.Column.DataType.Name.ToLower() == "string")
                                    uGridCell.Value = null;
                                else//數字會被歸類為system.decimal使用null 會無法還原,但用Dbnull.value 畫面會殘留值
                                    uGridCell.Value = DBNull.Value;
                                //styleresolved
                            }
                            //ugcell.Value = DBNull.Value;
                            else
                                uGridCell.Value = OldValue;
                            uGridCell.SelectAll();
                        }
                    }
                    else
                    {
                        IsChanged = true;
                    }
                    (sender as UltraGrid).UpdateData();
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                IsItemchkValid = false;
                uGridCell.Value = OldValue;
                WfShowErrorMsg(ex.ToString());
            }
            finally
            {
                //IsInButtonClick = false;
                IsInItemchecking = false;
            }
            //    string colName = "";
            //    string dbValue;
            //    UltraGridCell ugcell = null;
            //    DataRowView drvActive = null;
            //    DataRow drActive;
            //    try
            //    {
            //        if (IsInSaveCancle == true || IsInCRUDIni == true)
            //            return;
            //        if (IsInFormLoading == true)
            //            return;

            //        IsInItemchecking = true;
            //        IsItemchkValid = true;
            //        ugcell = (sender as UltraGrid).ActiveCell;
            //        colName = ugcell.Column.Key;
            //        drvActive = (System.Data.DataRowView)ugcell.Row.ListObject;
            //        if (drvActive == null)
            //            return;

            //        (sender as UltraGrid).UpdateData();
            //        drActive = drvActive.Row;
            //        dbValue = GlobalFn.isNullRet(drActive[colName], "");

            //        if (!ugcell.Value.Equals(OldValue))
            //        {
            //            if (WfItemCheck(sender, colName, ugcell.Value, drActive) == false)
            //            {
            //                e.Cancel = true;
            //                IsItemchkValid = false;

            //                if (ugcell.Column.Style == Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList)
            //                    ugcell.Value = ugcell.OriginalValue;    //null 問題
            //                else
            //                {
            //                    if (OldValue == DBNull.Value)
            //                        ugcell.Value = null;
            //                    else
            //                        ugcell.Value = OldValue;
            //                    ugcell.SelectAll();
            //                }
            //            }
            //            else
            //            {
            //                IsChanged = true;
            //            }
            //            (sender as UltraGrid).UpdateData();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        e.Cancel = true;
            //        IsItemchkValid = false;
            //        ugcell.Value = OldValue;
            //        WfShowMsg(ex.ToString());
            //    }
            //    finally
            //    {
            //        //IsInButtonClick = false;
            //        IsInItemchecking = false;
            //    }
        }
        #endregion

        #region UcDatePicker 時間控制項
        //protected internal override void UcDatePicker_Validating(object sender, CancelEventArgs e)
        //{
        //    string colName = "";
        //    string currentValue;
        //    string ctlValue;
        //    UcDatePicker control = sender as UcDatePicker;
        //    try
        //    {
        //        if (IsInSaveCancle == true || IsInCRUDIni == true)
        //            return;
        //        if (IsInFormLoading == true)
        //            return;

        //        colName = control.Tag.ToString();

        //        IsItemchkValid = true;
        //        currentValue = DRMASTER[colName] == null ? "" : DRMASTER[colName].ToString();
        //        ctlValue = control.Value == null ? "" : control.Value.ToString();
        //        if (ctlValue != currentValue)
        //        {
        //            if (WfItemCheck(sender, colName, ctlValue, DRMASTER) == false)
        //            {
        //                e.Cancel = true;
        //                IsItemchkValid = false;
        //                control.Value = OldValue;
        //            }
        //            else
        //            {
        //                IsChanged = true;
        //                DRMASTER.EndEdit();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        e.Cancel = true;
        //        control.Value = OldValue;
        //        IsItemchkValid = false;
        //        WfShowMsg(ex.ToString());
        //    }
        //}
        #endregion

        #region UltraDateTimeEditor_Validating 時間控制項
        protected internal override void UltraDateTimeEditor_Validating(object sender, CancelEventArgs e)
        {
            string colName = "";
            //DateTime? dbValue = null;
            UltraDateTimeEditor control = sender as UltraDateTimeEditor;
            try
            {
                if (IsInSaveCancle == true || IsInCRUDIni == true)
                    return;
                if (IsInFormLoading == true)
                    return;

                //if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.NA)
                //    return;

                WfCleanBottomMsg();
                IsInItemchecking = true;
                IsItemchkValid = true;
                colName = control.Tag.ToString();

                if (GlobalFn.isNullRet(control.Value, "") != GlobalFn.isNullRet(OldValue, ""))
                {
                    //WfSetBllTransaction();

                    var itemCheckInfo = new ItemCheckInfo();
                    itemCheckInfo.Row = DrMaster;
                    itemCheckInfo.Value = control.Value;
                    itemCheckInfo.Column = colName;
                    if (WfItemCheck(sender, itemCheckInfo) == false)
                    {
                        e.Cancel = true;
                        IsItemchkValid = false;
                        control.Value = OldValue;
                    }
                    else
                    {
                        IsChanged = true;
                        DrMaster.EndEdit();
                    }
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                IsItemchkValid = false;
                control.Value = OldValue;
                WfShowErrorMsg(ex.ToString());
            }
            finally
            {
                //IsInButtonClick = false;
                IsInItemchecking = false;
            }
        }
        #endregion

        #region WfItemCheck 控制項離開檢查
        protected virtual bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //protected virtual bool WfItemCheck(object sender, string pColName, object pNewValue, DataRow pDr)
        //{
        //    try
        //    {
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion

        #endregion

        #region PICK按鈕相關事件 含grid及 ultratexteditor 及註冊pick熱鍵
        protected override void UltraTextEditor_EditorPickButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            string pickName = "";
            try
            {
                if ((sender as UltraTextEditor).ReadOnly == true)
                    return;
                if (e.Button.Key != null && e.Button.Key.ToLower() == "pick")
                {

                    if (e.Button.Tag != null)
                    {
                        pickName = e.Button.Tag.ToString();

                        if (WfPickClickOnEditMode(sender, pickName, DrMaster) == true)
                        {
                            DrMaster.EndEdit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.ToString());
            }
        }

        protected virtual bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected override internal void UltraTextEditor_KeyUp(object sender, KeyEventArgs e)
        {
            string pickName = "";
            Infragistics.Win.UltraWinEditors.UltraTextEditor ute;
            try
            {
                ute = sender as UltraTextEditor;
                pickName = ute.Tag.ToString();
                if (ute.ReadOnly == true)
                    return;
                if (e.KeyData == (Keys.Control | Keys.P))
                {
                    if (WfPickClickOnEditMode(sender, pickName, DrMaster) == true)
                    {
                        DrMaster.EndEdit();
                    }
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

        #region WfCleanErrorProvider 清除所有的錯誤警告
        protected virtual void WfCleanErrorProvider()
        {
            this.errorProvider.Clear();
        }
        #endregion

        #region WfFireControlValidated 讓控制項強制離開編輯並產生驗證
        protected void WfFireControlValidated(Control pControl)
        {
            string typeName;
            Control actContrl;
            try
            {
                typeName = pControl.GetType().ToString();

                if (typeName == "Infragistics.Win.EmbeddableTextBoxWithUIPermissions")
                {
                    typeName = pControl.Parent.GetType().ToString();
                    actContrl = pControl.Parent;
                }
                else
                {
                    actContrl = pControl;
                }

                switch (typeName)
                {
                    case "Infragistics.Win.UltraWinGrid.UltraGrid":
                        (actContrl as Infragistics.Win.UltraWinGrid.UltraGrid).PerformAction(UltraGridAction.ExitEditMode);
                        break;
                    case "Infragistics.Win.UltraWinEditors.UltraTextEditor":
                        UltraTextEditor_Validating(actContrl, new CancelEventArgs());
                        //UltraTextEditor_BeforeExitEditMode(actContrl, new Infragistics.Win.BeforeExitEditModeEventArgs(true));
                        break;
                    case "YR.Util.Controls.UcCheckBox":
                        UcCheckBox_Validating(actContrl, new CancelEventArgs());
                        break;
                    case "Infragistics.Win.UltraWinGrid.UltraCombo":
                        UltraCombo_Validating(actContrl, new CancelEventArgs());
                        break;
                    case "Infragistics.Win.UltraWinEditors.UltraDateTimeEditor":
                        UltraDateTimeEditor_Validating(actContrl, new CancelEventArgs());
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
