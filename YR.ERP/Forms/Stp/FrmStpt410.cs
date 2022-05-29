/* 程式名稱: POS收銀管理系統
   系統代號: FrmStpt410.cs
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

using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Controls;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using YR.ERP.Base.Forms;
using System.IO;
using bpac;

namespace YR.ERP.Forms.Stp
{
    public partial class FrmStpt410 : FrmBase
    {

        #region Property
        public TabInfo TabMaster = new TabInfo();           // 保存主表相關物件 (Grid, datasource, GLL) 的類別    
        public AdmBLL BoMaster                              // 主表的 business object
        {
            get { return TabMaster.BoBasic; }
            set { TabMaster.BoBasic = value; }
        }
        //protected int IntCurTab = 1;                        //設定主表停留在那個頁面
        //protected int IntTabCount = 5;                      //設定主表有多少個 Tab
        //protected int IntMasterGridPos = 0;                 //uGrid_Master要停在那個頁面 如為0則不顯示
        //程式編號
        protected string StrFormID = "";

        protected YREditType FormEditMode = YREditType.NA;  //表單目前編輯動作為 "新增" "修改"....
        protected string StrQueryWhere { get; set; }        //輸入查詢條件字串         

        private string _strQueryWhereAppend = "";
        protected string StrQueryWhereAppend
        {
            get { return _strQueryWhereAppend; }
            set
            {
                _strQueryWhereAppend = value;
                if (!GlobalFn.varIsNull(_strQueryWhereAppend))
                {
                    TabMaster.IsAutoQueryFistLoad = true;
                }
            }
        }  //查詢條件字串-做為被呼叫時由其他視窗載入,前面需加 AND EX:AND ica01='xxx' 
        protected string StrQuerySecurity { get; set; }     //表單組合後的查詢權限

        protected bool IsChanged = false;                   //是否有修改或變更
        protected bool IsInSaveCancle = false;              //按下存檔取消時用來避開控制項驗證
        protected bool IsInCRUDIni = false;                 //按下新增,修改,刪除,查詢時,避開控制項驗證
        protected bool IsItemchkValid = true;               //檢驗資料驗證是否正常
        protected bool IsInItemchecking = false;            //處於validating行為中
        protected bool IsInGridButtonClick = false;         //clickedGridButton時,不要觸發itemchanged事件
        protected bool IsInCopying = false;                 //處理拷貝時不要觸發相關事件,ex rowactived
        protected DataRow DrMaster;                         //主表目前 active 的 datarow

        protected UltraGrid uGridMaster = new UltraGrid();
        protected int IntCurTabDetail = 0;                 //設定 Detail 停留在那個頁面 變形目前都只有0
        protected int IntTabDetailCount = 1;               //設定  Detail 有多少個 Tab
        protected List<TabDetailInfo> TabDetailList;      // Keep 事先定義的所有 Detail Tab 的設定

        //public FrmAdvanceQuery frmQuery;                    //進階查詢視窗        
        BasBLL BoBas = null;
        StpBLL BoStp = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;
        TaxBLL BoTax = null;

        bek_tb BekTbModel = null;      //幣別檔,在display mode載入
        #endregion Property

        #region FrmStpt410
        public FrmStpt410()
        {
            InitializeComponent();
        }
        #endregion

        #region WfInitialForm() : 初始化表單
        /// <summary>
        /// 初始化表單
        /// </summary>
        /// <returns></returns>
        protected virtual bool WfInitialForm()
        {
            try
            {
                if (WfSetVar() == false)
                    return false;

                //WfIniMasterGrid();
                if (WfIniMaster() == false)
                    return false;

                //WfIniToolBarUI();
                //UsbButtom.Height = 24;      //buttom 的toolbar 高度會跳掉先微調
                WfBindMasterByTag(this, TabMaster.AzaTbList, this.DateFormat);
                WfBindMaster();

                // 設定 detail Tab 屬性
                if (this.WfIniDetail() == false)
                    return false;

                panel5.Controls.Add(uGridMaster);
                uGridMaster.Width = 1;
                uGridMaster.Height = 1;
                //uGridMaster.Dock = DockStyle.Fill;
                //uGridMaster.Visible = false;
                return true;
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected Boolean WfSetVar()
        {
            // 繼承的表單要覆寫, 更改參數值            
            /*
            this.strFormID = "XXX";
            this.isDirectEdit = false;
            this.isMultiRowEdit = false;
            this.intMasterGridPos=0;
             */
            StrFormID = "stpt410";
            this.FormEditMode = YREditType.新增;
            TabMaster.CanUseRowLock = false;
            return true;
        }
        #endregion

        #region WfIniMaster() : 依定義的 itabCount 設定 uTab_Master
        /// <summary>
        /// 依定義的 itabCount 設定 uTab_Master
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean WfIniMaster()
        {
            int i = 0;
            try
            {
                //if (this.IntTabCount <= 0)
                //{
                //    WfShowBottomStatusMsg("未定義intTabCount!!");
                //    return false;
                //}

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
                //this.StrQuerySecurity = WfGetSecurityString();      //取得權限查詢字串

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
                if (BaaModel == null)
                {
                    if (StrFormID.ToLower() != "basi001"
                        && StrFormID.ToLower() != "basi080"
                        )
                        throw new Exception("共用參數代碼檔未設定(basi001)!");
                }


                #region 取得日期格式
                if (BaaModel == null || GlobalFn.varIsNull(BaaModel.baa01))
                    DateFormat = "yyyy/MM/dd";
                else
                {
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
                }
                #endregion

                //// 依   tab 數量設定是否可見
                //i = 0;
                //foreach (Infragistics.Win.UltraWinTabControl.UltraTab ut in this.uTab_Master.Tabs)
                //{
                //    i++;
                //    if (i <= IntTabCount)
                //        ut.Visible = true;
                //    else
                //        ut.Visible = false;
                //}

                // 先取得空的資料表
                //if (this.TabMaster.ViewTable != "")
                //{
                //    if (WfQueryByFirstLoad() == false)  //拆成function 之後繼承時在他窗會比較好override
                //        return false;
                //}

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

        #region WfIniDetail : 依定義的 itabsCount 設定 uTab_Detail
        /// <summary>
        /// 依定義的 itabsCount 設定 uTab_Detail
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean WfIniDetail()
        {
            string tableName = "", tablePrefix = "";
            int iTabsIndex = 0;
            DataTable dtTemp = null;
            if (this.IntTabDetailCount <= 0)
            {
                return false;
            }

            try
            {
                // 初始 TabInfo 記錄 tabMaster 設定
                this.TabDetailList = new List<TabDetailInfo>();

                // 依 Detail 建立 tabInfo            
                while (iTabsIndex < this.IntTabDetailCount)
                {
                    TabDetailInfo ltabinfo = new TabDetailInfo();
                    //ltabinfo.isAddMode = this.PupgmInfo.Update_right == "Y" ? true : false;
                    //ltabinfo.isDeleteMode = ltabinfo.isAddMode;
                    //ltabinfo.isReadOnly = false;
                    this.TabDetailList.Add(ltabinfo);
                    iTabsIndex++;
                }

                this.WfIniTabDetailInfo();

                iTabsIndex = 0;
                // 依 detail tab 數量設定是否可見
                #region 依 detail tab 數量設定是否可見
                //foreach (Infragistics.Win.UltraWinTabControl.UltraTab ut in this.uTab_Detail.Tabs)
                //{

                if (iTabsIndex < this.IntTabDetailCount)
                {
                    TabDetailInfo tabDetailinfo = TabDetailList[iTabsIndex];
                    if (tabDetailinfo.TargetColumn.Trim().Length == 0)
                    { tabDetailinfo.TargetColumn = "*"; }

                    //CommonBLL bo = new CommonBLL(LoginInfo.CompNo, tabDetailinfo.TargetTable, tabDetailinfo.TargetColumn, tabDetailinfo.ViewTable);
                    AdmBLL boDetail = new AdmBLL(BoMaster.OfGetConntion(), tabDetailinfo.TargetTable, tabDetailinfo.TargetColumn, tabDetailinfo.ViewTable);
                    if (boDetail != null)
                    {
                        tabDetailinfo.BoBasic = boDetail;
                        //bo.OfCreateDao(tabDetailinfo.TargetTable, tabDetailinfo.TargetColumn, tabDetailinfo.ViewTable);

                        if (BoSecurity != null)
                            tabDetailinfo.AzaTbList = BoSecurity.OfGetAzaModels(tabDetailinfo.ViewTable);

                        if (tabDetailinfo.ViewTable != "")
                        {
                            dtTemp = tabDetailinfo.BoBasic.OfSelect(" WHERE 1=2 ");
                        }

                    }
                    tabDetailinfo.UGrid = WfIniDetailGrid(iTabsIndex);
                    tabDetailinfo.UGrid.DataSource = dtTemp;
                    tableName = this.TabDetailList[iTabsIndex].ViewTable;
                    tablePrefix = this.TabDetailList[iTabsIndex].ViewTable;
                    #region switch (iTabsIndex) 暫刪,不使用
                    //switch (iTabsIndex)
                    //{
                    //    case 0:
                    //        WfIniDetailGrid(0);  //20130510 Allen add 第一個grid還是跑這段,只是不動態新增grid
                    //        tabDetailinfo.UGrid = this.uGridDetail1;
                    //        this.uGridDetail1.DataSource = ldt;
                    //        tableName = this.TabDetailList[iTabsIndex].ViewTable;
                    //        ls_prefix = this.TabDetailList[iTabsIndex].ViewTable;
                    //        break;
                    //    case 1:
                    //        WfIniDetailGrid(1);
                    //        tabDetailinfo.UGrid = this.uGridDetail2;
                    //        this.uGridDetail2.DataSource = ldt;
                    //        tableName = this.TabDetailList[iTabsIndex].ViewTable;
                    //        ls_prefix = this.TabDetailList[iTabsIndex].ViewTable;
                    //        break;
                    //    case 2:
                    //        WfIniDetailGrid(2);
                    //        tabDetailinfo.UGrid = this.uGridDetail3;
                    //        this.uGridDetail3.DataSource = ldt;
                    //        tableName = this.TabDetailList[iTabsIndex].ViewTable;
                    //        ls_prefix = this.TabDetailList[iTabsIndex].ViewTable;
                    //        break;
                    //    case 3:
                    //        WfIniDetailGrid(3);
                    //        tabDetailinfo.UGrid = this.uGridDetail4;
                    //        this.uGridDetail4.DataSource = ldt;
                    //        tableName = this.TabDetailList[iTabsIndex].ViewTable;
                    //        ls_prefix = this.TabDetailList[iTabsIndex].ViewTable;
                    //        break;
                    //    case 4:
                    //        WfIniDetailGrid(4);
                    //        tabDetailinfo.UGrid = this.uGridDetail5;
                    //        this.uGridDetail5.DataSource = ldt;
                    //        tableName = this.TabDetailList[iTabsIndex].ViewTable;
                    //        ls_prefix = this.TabDetailList[iTabsIndex].ViewTable;
                    //        break;

                    //}
                    #endregion switch (iTabsIndex)

                    this.WfSetTabInfoDataSource(iTabsIndex, tabDetailinfo, dtTemp, tableName, tablePrefix);
                    WfIniDetailComboSource(iTabsIndex); //處理下拉式資料來源
                    WfSetControlReadonly(tabDetailinfo.UGrid, true);
                }

                //iTabsIndex++;
                //if (iTabsIndex <= IntTabDetailCount)
                //    ut.Visible = true;
                //else
                //    ut.Visible = false;

                //}
                #endregion 依 detail tab 數量設定是否可見

                //this.IntCurTabDetail = this.uTab_Detail.SelectedTab.Index;

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

            BoStp = new StpBLL(BoMaster.OfGetConntion());
            BoBas = new BasBLL(BoMaster.OfGetConntion());
            BoInv = new InvBLL(BoMaster.OfGetConntion());
            BoAdm = new AdmBLL(BoMaster.OfGetConntion());
            BoTax = new TaxBLL(BoMaster.OfGetConntion());
            return;
        }
        #endregion

        #region FrmStpt410_Load
        private void FrmStpt410_Load(object sender, EventArgs e)
        {
            try
            {
                this.Shown += FrmStpt410_Shown;
                WfIniTabMasterInfo();
                if (WfInitialForm() == false)
                {
                    if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)   //避免desingmode 一直出現錯誤訊息
                    {
                        //this.Close();
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
                if (this.IsMdiChild)
                    this.ControlBox = false;
                //WfShowRibbonGroup(YREditType.NA, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
                //設定圖檔
                ImageList ilLarge = new ImageList();
                ilLarge = GlobalPictuer.LoadToolBarImage();

                ImageList ilModule = new ImageList();
                ilModule = GlobalPictuer.LoadModuleImage();

                btnSave.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_SAVE];
                btnSave.TextImageRelation = TextImageRelation.ImageBeforeText;

                btnInvi100.Image = ilLarge.Images["pick_32"];
                btnInvi100.TextImageRelation = TextImageRelation.ImageBeforeText;

                btnStp400.Image = ilLarge.Images["pick_32"];
                btnStp400.TextImageRelation = TextImageRelation.ImageBeforeText;


                btnDelete.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_TRANSH_CAN];
                btnDelete.TextImageRelation = TextImageRelation.ImageBeforeText;

                btnLabel.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_PRINTER];
                btnLabel.TextImageRelation = TextImageRelation.ImageBeforeText;

                btnLabelMoney.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_PRINTER];
                btnLabelMoney.TextImageRelation = TextImageRelation.ImageBeforeText;

                btnShipping.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_SHIPPING];
                btnShipping.TextImageRelation = TextImageRelation.ImageBeforeText;


                btnAutoCal.Image = ilModule.Images[GlobalPictuer.MODULE_GLA];
                btnAutoCal.TextImageRelation = TextImageRelation.ImageBeforeText;

                btnCreditCard.Image = ilModule.Images[GlobalPictuer.MODULE_STP];
                btnCreditCard.TextImageRelation = TextImageRelation.ImageBeforeText;


                WfDisplayMode();
                this.KeyPreview = true;
            }
            catch (Exception ex)
            {
                IniSuccess = false;
                WfShowErrorMsg(ex.Message);
            }
        }

        private void FrmStpt410_Shown(object sender, EventArgs e)
        {
            try
            {
                //if (TabMaster.IsAutoQueryFistLoad)
                //{
                //    WfAutoQueryFistLoad();
                //}
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
                this.Close();
            }

        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected void WfIniTabMasterInfo()
        {
            //設定master 或detail的 tabinfo
        }
        #endregion

        /**********************  其他function **********************/

        #region WfToolbarInsert() : 主表新增 function
        /// <summary>
        /// 主表修改 function
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean WfToolbarInsert()
        {
            DataRow drNew;
            try
            {
                drNew = TabMaster.DtSource.NewRow();
                if (WfAddMasterDataRow(drNew) == false)
                    return false;

                if (WfSetMasterRowDefault(drNew) == false)
                    return false;

                this.FormEditMode = YREditType.新增;
                //WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
                WfSetControlPropertyByMode(this);
                this.IsChanged = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
        #endregion 主表新增 function

        #region WfAddMasterDataRow(DataRow pDrNew) : 新增一個 master row
        /// <summary>
        /// 新增一個 master row
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean WfAddMasterDataRow(DataRow pDrNew)
        {
            int activeIndex = -1;
            DataRow dr = null;
            DataRowView drv = null;
            try
            {
                //if (this.strMode == YREditType.新增 && this.tabMaster.isAddMode == false)
                //{
                //    Global_Fn.of_err_msg("2", "使用者無新增權限，請檢核!", "錯誤");
                //    return false;
                //}

                //// 如果已是在新增中則返回
                //if (this.strMode == YREditType.新增 || this.strMode == YREditType.複製)
                //{
                //    if (DRMASTER != null && DRMASTER.RowState == DataRowState.Added) return false;
                //}

                //if (this.wf_rowchange_check(1) == false)
                //    return false;

                if (this.uGridMaster.ActiveRow != null)
                { activeIndex = this.uGridMaster.ActiveRow.Index; }
                else
                { activeIndex = -1; }


                if (pDrNew == null)
                {
                    drv = this.TabMaster.DtSource.DefaultView.AddNew();
                    dr = ((DataRowView)drv).Row;
                }
                else
                {
                    dr = pDrNew;
                }
                if (dr == null)
                {
                    throw new Exception(" 新的 datarow == null !");
                }

                this.TabMaster.DtSource.Rows.Add(dr);

                if (dr == null) { return false; }
                if (this.WfSetActiveRowToNewRow(this.uGridMaster, activeIndex))
                {
                    ////重新查詢明細
                    //if (this.strMode == YREditType.新增)
                    //{ wf_retrieve_detail_fist(); }
                }
                else { return false; }

                // 設定預設值, 不同視窗 overrride 內容
                //this.isInLoadRowDefault = true;
                //try
                //{
                //    if (this.strMode == YREditType.新增)
                //    {
                //        dr.BeginEdit();
                //        this.isItemChecking = true;//20121228 Allen add 避免觸發 DATACOLUMN ITEMCHANGE事件
                //        if (this.wf_set_master_default(dr) == false)
                //        {
                //            this.isItemChecking = false;//20121228 Allen add 避免觸發 DATACOLUMN ITEMCHANGE事件
                //            return false;
                //        }
                //        dr.EndEdit();
                //        DRMASTER = dr;
                //    }
                //}
                //finally
                //{
                //    this.isInLoadRowDefault = false;
                //    this.isItemChecking = false;//20121228 Allen add 避免觸發 DATACOLUMN ITEMCHANGE事件
                //}
                //this.errorProvider_entrybase.Clear();

                //this.ResumeLayout(true);
                //if (this.intTabCount > 1)
                //    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                //this.ResumeLayout(false);


                //if (this.tabMaster.dtSource.Rows.Count > 0)
                //    this.uTab_Master.Enabled = true;
                //this.wf_post_add_master_row();
                //this.isChanged = false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMasterDefaultByCopy 設定主表預設值
        protected virtual void WfSetMasterDefaultByCopy(DataRow pDrMaster)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetActiveRowToNewRow : 將 Grid Active Row  定位至新增的 DataRow
        protected bool WfSetActiveRowToNewRow(UltraGrid pGrid, int pCurrentIndex)
        {
            if (pGrid.Rows.Count == 0) return false;
            if (pGrid.Rows.Count == 1)
            {
                pGrid.ActiveRow = uGridMaster.Rows[0];
                pGrid.PerformAction(UltraGridAction.FirstCellInRow);
                return true;
            }
            if (pCurrentIndex < 0) pCurrentIndex = pGrid.Rows.Count;

            #region 最後一列是新列
            if (pGrid.Rows.Count > 0)
            {
                try
                {
                    DataRowView drv = (DataRowView)pGrid.Rows[pGrid.Rows.Count - 1].ListObject;
                    if (drv.Row.RowState == DataRowState.Added)
                    {
                        pGrid.ActiveRow = pGrid.Rows[pGrid.Rows.Count - 1];
                        pGrid.PerformAction(UltraGridAction.FirstCellInRow);
                        return true;
                    }
                }
                catch { }
            }
            #endregion

            #region 第一列是新列
            if (pGrid.Rows.Count > 0)
            {
                try
                {
                    DataRowView drv = (DataRowView)pGrid.Rows[0].ListObject;
                    if (drv.Row.RowState == DataRowState.Added)
                    {
                        pGrid.ActiveRow = pGrid.Rows[0];
                        return true;
                    }
                }
                catch { }
            }
            #endregion

            #region 如次一列是新列，則定位次一列
            if (pGrid.Rows.Count > (pCurrentIndex + 1))
            {
                try
                {
                    DataRowView drv = (DataRowView)pGrid.Rows[pCurrentIndex + 1].ListObject;
                    if (drv.Row.RowState == DataRowState.Added)
                    {
                        pGrid.ActiveRow = pGrid.Rows[pCurrentIndex + 1];
                        pGrid.PerformAction(UltraGridAction.FirstCellInRow);
                        return true;
                    }
                }
                catch { }
            }
            #endregion

            #region 如前一列是新列，則定位前一列
            if (pGrid.Rows.Count > (pCurrentIndex - 1))
            {
                try
                {
                    DataRowView drv = (DataRowView)pGrid.Rows[(pCurrentIndex - 1)].ListObject;
                    if (drv.Row.RowState == DataRowState.Added)
                    {
                        pGrid.ActiveRow = pGrid.Rows[(pCurrentIndex - 1)];
                        pGrid.PerformAction(UltraGridAction.FirstCellInRow);
                        return true;
                    }
                }
                catch { }
            }
            #endregion

            #region 如當前列是新列, 則定位在當前列
            if (pGrid.Rows.Count > (pCurrentIndex))
            {
                try
                {
                    DataRowView drv = (DataRowView)pGrid.Rows[(pCurrentIndex)].ListObject;
                    if (drv.Row.RowState == DataRowState.Added)
                    {
                        pGrid.ActiveRow = pGrid.Rows[(pCurrentIndex)];
                        pGrid.PerformAction(UltraGridAction.FirstCellInRow);
                        return true;
                    }
                }
                catch { }
            }
            #endregion

            return false;
        }
        #endregion

        #region WfRowChangeCheck() : 表頭換列時檢查是否有修改尚未存檔
        /// <summary>
        /// 表頭換列時檢查是否有修改尚未存檔 
        /// </summary>
        /// <returns> true :  放棄編輯中資料, 可換列 </returns>
        ///           false : 不可換列
        protected virtual Boolean WfRowChangeCheck(int iAskGiveup)
        {
            try
            {
                //this.SelectNextControl(this.txt_focus, true, true, false, false);
                //this.txt_focus.Focus();

                DataRow drActive = WfGetActiveDatarow();
                //if (ldr != null)
                //{
                //    ldr.EndEdit();
                //}
                if (this.IsChanged == true)
                {

                    if (iAskGiveup == 0)
                    {
                        WfShowErrorMsg("資料尚未存檔,請先存檔再離開!");
                        GlobalFn.ofShowDialog("2", "Data not be saved, please save it first!", "Warning");
                        return false;
                    }

                    var result = WfShowConfirmMsg("資料已變更，尚未存檔，確定要離開 ?");

                    //if (WfShowConfirmMsg("資料已變更，尚未存檔，確定要離開 ? ") == 1)
                    if (result == DialogResult.Yes)
                    {
                        this.BindingMaster.CancelEdit();
                        //((DataTable)this.bindingMaster.DataSource).RejectChanges();

                        //this.isChanged = false;
                        return true;
                    }
                    else
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetDocPicture 設定單據顯示圖片
        protected virtual void WfSetDocPicture(string pValid, string pConfirm, string pState, PictureBox pPbx)
        {
            //ImageList imgList = null;
            //try
            //{
            //    pPbx.Visible = true;
            //    imgList = GlobalPictuer.LoadDocImage();
            //    if (imgList == null)
            //        return;

            //    if (imgList == null)
            //        return;
            //    if (pValid == "N")
            //    {
            //        pPbx.Image = imgList.Images["doc_invalid_32"];
            //        return;
            //    }

            //    if (pConfirm == "X")
            //    {
            //        pPbx.Image = imgList.Images["doc_invalid_32"];
            //        return;
            //    }
            //    else if (pConfirm == "N" || pConfirm == "")
            //    {
            //        pPbx.Image = imgList.Images["doc_open_32"];
            //        return;
            //    }
            //    else if (pConfirm == "Y")
            //    {
            //        pPbx.Image = imgList.Images["doc_confirm_32"];  //至少會有確認的圖
            //        if (pState == "9")
            //            pPbx.Image = imgList.Images["doc_closed_32"];
            //        return;
            //    }

            //    pPbx.Visible = false;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        #endregion

        #region WfCleanErrorProvider 清除所有的錯誤警告
        protected virtual void WfCleanErrorProvider()
        {
            this.errorProvider.Clear();
        }
        #endregion

        /**********************  enum **********************/



        /******************* ultraGrid 事件及方法 ***********************/
        #region WfIniMasterGrid
        protected virtual void WfIniMasterGrid()
        {
            try
            {
                uGridMaster.BeforeRowDeactivate += UGrid_BeforeRowDeactivate;
                uGridMaster.AfterRowActivate += UGrid_Master_AfterRowActivate;
                uGridMaster.AfterCellActivate += new System.EventHandler(this.UGridMaster_AfterCellActivate);
                //if (IntTabCount > 0)
                //{
                //    if (IntMasterGridPos == 0)
                //    {
                //        uGridMaster.Visible = false;
                //        uGridMaster.Size = new System.Drawing.Size(0, 0);
                //        uTab_Master.Tabs[0].TabPage.Controls.Add(uGridMaster);

                //    }
                //    if (IntMasterGridPos != 0 && IntMasterGridPos <= IntTabCount)
                //    {
                //        ((System.ComponentModel.ISupportInitialize)(uGridMaster)).BeginInit();
                //        uTab_Master.Tabs[IntMasterGridPos - 1].TabPage.Controls.Add(uGridMaster);
                //        uGridMaster.Dock = DockStyle.Fill;
                //        uGridMaster.ClickCellButton += new CellEventHandler(UltraGrid_ClickCellButton);
                //        WfSetAppearance(uGridMaster, 1);
                //        ((System.ComponentModel.ISupportInitialize)(uGridMaster)).EndInit();
                //    }
                //}
                TabMaster.UGrid = uGridMaster;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfActiveGridNextRow(...) : 設定 Grid Active Row 為指定的 Index
        /// <summary>
        /// 設定 Grid Active Row 為指定的 Index
        /// </summary>
        /// <param name="pGrid">要定位的 Grid</param>
        /// <param name="currentIndex">目前所在列的 index</param>
        public void WfActiveGridNextRow(Infragistics.Win.UltraWinGrid.UltraGrid pGrid, int currentIndex)
        {
            try
            {
                ResumeLayout(false);
                pGrid.PerformAction(UltraGridAction.ExitEditMode);
                if (pGrid.Rows.Count == 0) { return; }

                //刪除列為最後一筆
                if (pGrid.Rows.Count > currentIndex)
                {
                    pGrid.ActiveRow = pGrid.Rows[currentIndex];
                }
                else
                {
                    pGrid.ActiveRow = pGrid.Rows[pGrid.Rows.Count - 1];
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ResumeLayout(true);
            }
        }
        #endregion 設定 Grid Active Row 為指定的 Index

        #region WfGetActiveDatarow() : 取得 master 表頭的目前資料列 DataRow
        /// <summary>
        /// 取得 master 表頭的目前資料列 DataRow
        /// </summary>
        /// <returns></returns>
        protected virtual DataRow WfGetActiveDatarow()
        {
            DataRow dr = null;
            if (uGridMaster.ActiveRow == null || uGridMaster.ActiveRow.IsUnmodifiedTemplateAddRow || uGridMaster.ActiveRow.ListObject == null)
                return null;

            if (uGridMaster.ActiveRow.IsUnmodifiedTemplateAddRow == false)
            {
                if (uGridMaster.ActiveRow.ListObject.GetType() == typeof(System.Data.DataRowView))
                {
                    System.Data.DataRowView drv = (System.Data.DataRowView)uGridMaster.ActiveRow.ListObject;
                    return drv.Row;
                }
            }

            return dr;
        }
        #endregion

        #region UGrid_Master_AfterRowActivate(sender, EventArgs e)  : GridRow 作用後事件
        protected virtual void UGrid_Master_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                if (InFormClosing == true)
                    return;
                DrMaster = WfGetActiveDatarow();
                if (FormEditMode == YREditType.NA)   //新增修改交給toolbar處理
                    WfDisplayMode();

                //if (DrMaster == null) return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region UGrid_BeforeRowDeactivate(sender, e) : 主表換 Row 前的事件
        /// <summary>
        /// 主表換 Row 前的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void UGrid_BeforeRowDeactivate(object sender, CancelEventArgs e)
        {
            try
            {
                if (IsInCopying == true || InFormClosing == true)
                    return;
                //if (strMode == YREditType.新增 || strMode == YREditType.修改 || strMode == YREditType.複製)
                if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
                {
                    e.Cancel = true; ;
                    return;
                }

                if (this.WfRowChangeCheck(1) == false)
                    e.Cancel = true;

            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

        #region UGridMaster_AfterCellActivate(object sender, EventArgs e)
        private void UGridMaster_AfterCellActivate(object sender, EventArgs e)
        {
            UltraGrid uGrid;
            UltraGridCell uCellActive;
            string aza03;
            string msg;
            try
            {
                uGrid = sender as UltraGrid;
                if (uGrid.ActiveCell == null)
                    return;

                //WfGetOldValue(sender as Control); //移到 UGridDetail_AfterEnterEditMode
                if (TabMaster.AzaTbList == null)
                    return;

                uCellActive = uGrid.ActiveCell;
                aza03 = GlobalFn.isNullRet(uCellActive.Column.Key, "");
                if (aza03 == "")
                    return;
                var azaModel = TabMaster.AzaTbList.Where(p => p.aza03 == aza03).FirstOrDefault();
                if (azaModel == null || azaModel.aza03 == null) return;
                msg = azaModel.aza04 + "[" + azaModel.aza03 + "]";

                //這裡是為了讓編輯模式時,讓小數點數字若為0時,不顯示 ex:123.000==>123
                if (azaModel.aza08.ToLower() == "numeric" && uCellActive.Value != DBNull.Value
                        && FormEditMode != YREditType.查詢)
                {
                    WfSetControlEditNumeric(uCellActive);
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
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

        /**********************  Transaction 交易處理  **********************/
        #region Transaction 交易處理
        #region  WfBeginTran() : 開始新的交易
        protected bool WfBeginTran()
        {
            try
            {
                WfBeginTran(IsolationLevel.ReadCommitted);
                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected bool WfBeginTran(IsolationLevel pIsolationLevel)
        {
            try
            {
                // 如已有交易存在，則先 commit
                if (BoMaster.TRAN != null)
                {
                    try { BoMaster.TRAN.Commit(); }
                    catch (InvalidOperationException) { }//這個 SqlTransaction 已經完成，它已不再是個可使用的項目 錯誤類型不處理
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                if (this.BoMaster == null)
                {
                    WfShowErrorMsg("商業物件 BOMASTER 為 null，無法創建新的交易，請檢核!");
                    return false;
                }

                System.Data.Common.DbConnection dbConnection = BoMaster.OfGetConntion();

                if (dbConnection == null)
                {
                    WfShowErrorMsg("取得資料庫連線錯誤 (DBConnection)，無法創建新的交易，請檢核!");
                    return false;
                }

                BoMaster.TRAN = dbConnection.BeginTransaction(pIsolationLevel);
                if (BoMaster.TRAN == null || BoMaster.TRAN.Connection == null)
                {
                    WfShowErrorMsg("取得資料庫交易錯誤 (BOMASTER.TRAN)，無法創建新的交易，請檢核!");
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region  WfCommit() : Commit 交易
        /// <summary>
        /// Commit 交易
        /// </summary>
        /// <returns></returns>
        protected bool WfCommit()
        {
            try
            {
                if (BoMaster.TRAN == null)
                {
                    WfShowErrorMsg("FrmEntryBase.wf_commit_tran : 交易 (TRAN) 為 Null，無法確認，請檢核! ");
                    return false;
                }
                BoMaster.TRAN.Commit();
                return true;
            }
            catch (InvalidOperationException) { return true; }//這個 SqlTransaction 已經完成，它已不再是個可使用的項目 錯誤類型不處理
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfRollback() : Rollback 交易
        /// <summary>
        /// Rollback 交易
        /// </summary>
        /// <returns></returns>
        protected bool WfRollback()
        {
            try
            {
                if (BoMaster.TRAN == null)
                {
                    WfShowErrorMsg("FrmEntryBase.wf_rollback_tran : 交易 (TRAN) 為 Null，請檢核! ");
                    return false;
                }
                BoMaster.TRAN.Rollback();
                return true;
            }
            catch (InvalidOperationException) { return true; }//這個 SqlTransaction 已經完成，它已不再是個可使用的項目 錯誤類型不處理
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #endregion

        /******************* 其它 Control 事件 ***********************/

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                //課稅別
                sourceList = BoStp.OfGetTaxTypeKVPList();
                WfSetUcomboxDataSource(ucb_sga06, sourceList);

                //發票聯數
                sourceList = BoStp.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_sga09, sourceList);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfBindMasterByTag() : 將 mater tab 所有的頁面的控制項，以 control.tag 屬性與 bindingmaster 做 binding
        //只做一次,因此順便處理元件不可編輯,及各種控制項的appearance
        protected override Boolean WfBindMasterByTag(Control pctrl, List<aza_tb> pAzaTbList, string pDateFormat)
        {
            //string ls_type = "";
            string tagValue = "";
            Type controlType;
            aza_tb azaModel = null;
            try
            {
                foreach (Control control in pctrl.Controls)
                {
                    controlType = control.GetType();
                    if (control.Tag != null
                        )
                    {
                        tagValue = control.Tag.ToString().Trim();
                        azaModel = (from o in pAzaTbList
                                    where o.aza03 == tagValue
                                    select o
                            ).FirstOrDefault<aza_tb>()
                                ;
                        if (azaModel != null)
                        {
                            control.Enter += new System.EventHandler(this.Control_Enter);

                        }
                    }
                    if (control.HasChildren)
                    {
                        if (controlType == typeof(Infragistics.Win.UltraWinTabControl.UltraTabControl))
                        {
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).TabStop = false;
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Default;
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).UseAppStyling = false;
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Office2007Ribbon;
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).TabButtonStyle = UIElementButtonStyle.Office2013Button;
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).Appearance.BackColor = ColorTranslator.FromHtml("#D8E6D1");
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).SelectedTabAppearance.BackColor = GetStyleLibrary.FormBackGRoundColor;
                        }
                        WfBindMasterByTag(control, pAzaTbList, pDateFormat);    //自我遞迴
                    }
                    #region switch (ls_type)
                    switch (controlType.ToString())
                    {
                        case "System.Windows.Forms.Label":
                            //if (GetStyleLibrary.FontControlNormal != null)
                            //    (control as Label).Font = GetStyleLibrary.FontControlNormal;
                            (control as Label).BackColor = Color.Transparent;
                            if (tagValue.Length > 0)
                            {
                                if (azaModel != null)
                                {
                                    control.Text = azaModel.aza04;
                                }
                            }
                            break;
                        case "Infragistics.Win.UltraWinEditors.UltraTextEditor":
                            //if (GetStyleLibrary.FontControlNormal != null)
                            //    (control as UltraTextEditor).Font = GetStyleLibrary.FontControlNormal;
                            (control as UltraTextEditor).AlwaysInEditMode = true;
                            (control as UltraTextEditor).UseAppStyling = false;
                            (control as UltraTextEditor).DisplayStyle = GetStyleLibrary.ControlDisplayStyle;
                            if (tagValue.Length > 0)
                            {
                                control.DataBindings.Clear();

                                (control as UltraTextEditor).Validating += new CancelEventHandler(UltraTextEditor_Validating);
                                if (azaModel != null)
                                {
                                    control.DataBindings.Add("Text", BindingMaster, tagValue);
                                    control.DataBindings["Text"].DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
                                    if (azaModel.aza08 == "numeric" || azaModel.aza08 == "int")
                                    {
                                        (control as UltraTextEditor).Appearance.TextHAlign = HAlign.Right;         //靠右對齊
                                        control.DataBindings["Text"].FormattingEnabled = true;
                                        control.DataBindings["Text"].FormatString = "###,###,###,###,###0.######";
                                    }

                                    (control as UltraTextEditor).KeyUp += new KeyEventHandler(UltraTextEditor_KeyUp);
                                    if ((azaModel.aza13 == null ? "" : azaModel.aza13).ToUpper() == "Y")                  //是否有PICK功能
                                    {
                                        WfSetUltraTxtEditPick(control as Infragistics.Win.UltraWinEditors.UltraTextEditor);
                                        (control as UltraTextEditor).EditorButtonClick += new EditorButtonEventHandler(UltraTextEditor_EditorPickButtonClick);
                                    }
                                }
                            }
                            WfSetControlReadonly(control as Infragistics.Win.UltraWinEditors.UltraTextEditor, true);
                            break;

                        case "Infragistics.Win.UltraWinGrid.UltraCombo":
                            if (GetStyleLibrary.FontControlNormal != null)
                                (control as UltraCombo).Font = GetStyleLibrary.FontControlNormal;
                            WfSetControlReadonly(control as Infragistics.Win.UltraWinGrid.UltraCombo, true);
                            (control as UltraCombo).RowSelected += new RowSelectedEventHandler(this.Ucombo_RowSelected);
                            (control as UltraCombo).UseAppStyling = false;
                            (control as UltraCombo).DisplayStyle = GetStyleLibrary.ControlDisplayStyle;
                            (control as UltraCombo).KeyUp += new KeyEventHandler(UltraCombo_KeyUp);
                            if (tagValue.Length > 0)
                            {
                                control.DataBindings.Clear();
                                control.DataBindings.Add("Value", BindingMaster, tagValue, false, DataSourceUpdateMode.OnPropertyChanged);
                                (control as UltraCombo).Validating += new CancelEventHandler(UltraCombo_Validating);
                            }
                            break;

                        case "YR.Util.Controls.UcCheckBox":
                            if (GetStyleLibrary.FontControlNormal != null)
                                (control as YR.Util.Controls.UcCheckBox).Font = GetStyleLibrary.FontControlNormal;
                            (control as UcCheckBox).BackColor = Color.Transparent;
                            WfSetControlReadonly(control as YR.Util.Controls.UcCheckBox, true);
                            (control as UcCheckBox).KeyUp += new KeyEventHandler(UcCheckBox_KeyUp);
                            if (tagValue.Length > 0)
                            {
                                control.DataBindings.Clear();
                                control.DataBindings.Add("CheckValue", BindingMaster, tagValue, false, DataSourceUpdateMode.OnPropertyChanged);
                                (control as YR.Util.Controls.UcCheckBox).Validating += new CancelEventHandler(UcCheckBox_Validating);
                                if (azaModel != null)
                                {
                                    control.Text = azaModel.aza04;
                                }
                            }
                            break;

                        case "Infragistics.Win.UltraWinEditors.UltraDateTimeEditor":
                            (control as UltraDateTimeEditor).AlwaysInEditMode = true;
                            (control as UltraDateTimeEditor).PromptChar = ' ';

                            if (GetStyleLibrary.FontControlNormal != null)
                                (control as Infragistics.Win.UltraWinEditors.UltraDateTimeEditor).Font = GetStyleLibrary.FontControlNormal;
                            (control as Infragistics.Win.UltraWinEditors.UltraDateTimeEditor).UseAppStyling = false;
                            if (((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).FormatString == null)
                                ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).FormatString = "yyyy/MM/dd";
                            if (((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).MaskInput == null)
                                ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).MaskInput = "yyyy/mm/dd";
                            ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).Appearance.TextHAlign = HAlign.Center;
                            ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.OnMouseEnter;
                            ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).DisplayStyle = GetStyleLibrary.ControlDisplayStyle;

                            WfSetControlReadonly(control as UltraDateTimeEditor, true);
                            (control as UltraDateTimeEditor).KeyUp += new KeyEventHandler(UltraDateTimeEditor_KeyUp);
                            if (tagValue.Length > 0)
                            {
                                control.DataBindings.Clear();
                                control.DataBindings.Add("Value", BindingMaster, tagValue, false, DataSourceUpdateMode.OnPropertyChanged);
                                (control as UltraDateTimeEditor).Validating += new CancelEventHandler(UltraDateTimeEditor_Validating);
                            }
                            if (azaModel.aza14 == "Y")   //是否顯示時間
                            {
                                ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).FormatString = pDateFormat + " hh:mm";
                                ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).MaskInput = pDateFormat.ToLower() + " hh:mm";
                            }
                            else
                            {
                                ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).FormatString = pDateFormat;
                                ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).MaskInput = pDateFormat.ToLower();
                            }
                            //((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(lcontrol)).FormatString = "yyyy/MM/dd tt hh:mm";
                            //((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(lcontrol)).MaskInput = "{LOC}yyyy/mm/dd hh:mm";
                            break;
                        case "System.Windows.Forms.PictureBox":
                            control.DataBindings.Add("Image", BindingMaster, tagValue, true);
                            break;
                        case "Infragistics.Win.Misc.UltraSplitter":
                            WfSetAppearance(control as Infragistics.Win.Misc.UltraSplitter);
                            break;
                        default:
                            break;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return true;
        }

        #endregion

        #region Control_Enter(object sender, EventArgs e) 所有控制項進入(ENTER)事件(未包含GRID)
        protected override void Control_Enter(object sender, EventArgs e)
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
                    if (azaModel.aza08.ToLower() == "numeric" && (sender as UltraTextEditor).Value != DBNull.Value
                            && FormEditMode != YREditType.查詢)
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

        #region WfSetControlPropertyByMode 依目前表單狀態對各元件做不同的處理 Ex:checkbox 三相或兩相
        protected virtual Boolean WfSetControlPropertyByMode(Control pControl)
        {
            string strType = "";
            string tagValue = "";
            StringBuilder lsbTip;
            aza_tb l_aza = null;
            try
            {
                foreach (Control lcontrol in pControl.Controls)
                {
                    strType = lcontrol.GetType().ToString();
                    if (lcontrol.Tag != null)
                    {
                        tagValue = lcontrol.Tag.ToString().Trim();
                        l_aza = (from o in TabMaster.AzaTbList
                                 where o.aza03 == tagValue
                                 select o
                            ).FirstOrDefault<aza_tb>()
                                ;
                        if (l_aza != null)
                        {
                            lsbTip = new StringBuilder();
                            lsbTip.AppendLine(string.Format("{0}-{1}", l_aza.aza03, l_aza.aza04));
                            WfShowTip(lcontrol, lsbTip.ToString());
                        }
                    }
                    if (lcontrol.HasChildren)
                    {
                        WfSetControlPropertyByMode(lcontrol);    //自我遞迴
                    }
                    #region switch (strType)
                    switch (strType)
                    {
                        case "Infragistics.Win.UltraWinGrid.UltraGrid":
                            if (FormEditMode == YREditType.查詢)
                                (lcontrol as Infragistics.Win.UltraWinGrid.UltraGrid).DataBind();//來源資料表均轉為string 所以重新bind來源~
                            break;
                        case "Infragistics.Win.UltraWinGrid.UltraCombo":
                            if (FormEditMode == YREditType.查詢)
                                (lcontrol as Infragistics.Win.UltraWinGrid.UltraCombo).DropDownStyle = UltraComboStyle.DropDown;
                            else
                                (lcontrol as Infragistics.Win.UltraWinGrid.UltraCombo).DropDownStyle = UltraComboStyle.DropDownList;
                            break;

                        case "YR.Util.Controls.UcCheckBox":
                            if (FormEditMode == YREditType.查詢)
                            {
                                if (!GlobalFn.varIsNull(lcontrol.Tag))        //2016.09.13 Allen加入 tag未輸入時,不受控制
                                {
                                    (lcontrol as YR.Util.Controls.UcCheckBox).ThreeState = true;
                                    (lcontrol as YR.Util.Controls.UcCheckBox).CheckState = CheckState.Indeterminate;
                                    (lcontrol as YR.Util.Controls.UcCheckBox).AllowNull = true;
                                    (lcontrol as YR.Util.Controls.UcCheckBox).NullValue = DBNull.Value;
                                }
                            }
                            else
                            {
                                (lcontrol as YR.Util.Controls.UcCheckBox).ThreeState = false;
                                (lcontrol as YR.Util.Controls.UcCheckBox).AllowNull = false;
                                (lcontrol as YR.Util.Controls.UcCheckBox).NullValue = (lcontrol as YR.Util.Controls.UcCheckBox).FalseValue;
                            }
                            break;

                        case "YR.Util.Controls.UcDatePicker":
                            if (FormEditMode == YREditType.查詢)
                            {
                                //(lcontrol as YR.Util.Controls.UcDatePicker).CheckedValue = false;
                                //(lcontrol as YR.Util.Controls.UcDatePicker).DataFilter = new YR.Util.Controls.DatePickerDataFilter();
                            }
                            else
                            {
                                //(lcontrol as YR.Util.Controls.UcDatePicker).CheckedValue = true;
                                //(lcontrol as YR.Util.Controls.UcDatePicker).DataFilter = null;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
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
            this.uGridMaster.DataSource = BindingMaster;
            //this.uGridMaster.DataBind(); //這裡bind後grid的資料有時會消失,因此mark掉


            //設定 Grid 的 title , 寬度
            if (this.TabMaster.IsGridFormatFinshed == false)
            {
                this.WfSetGridHeader(this.uGridMaster, TabMaster.AzaTbList, this.DateFormat);
                WfSetMasterGridLayout();
                WfSetGridButtomRowCount(this.uGridMaster);
                this.TabMaster.IsGridFormatFinshed = true;
            }

            //dt.TableName = this.tabMaster.ViewTable;

            // 設定 prefix, 用於 datacolumn_changing 檢查欄位長度
            dt.Prefix = this.TabMaster.ViewTable;
            //this.wf_set_master_grid_layout();
            return true;
        }
        #endregion

        #region WfQueryByFirstLoad 表單第一次進入時載入空資料,或被引用時可傳查詢條件或變形資料來源
        protected virtual bool WfQueryByFirstLoad()
        {
            try
            {
                //若是由作為他窗引用時
                if (GlobalFn.isNullRet(this.StrQueryWhereAppend, "")
                            .Trim().Length == 0)
                    this.TabMaster.DtSource = this.TabMaster.BoBasic.OfSelect(" WHERE 1=2 ");
                else
                    this.TabMaster.DtSource = this.TabMaster.BoBasic.OfSelect(this.StrQueryWhereAppend);

                this.WfSetMasterDatasource(this.TabMaster.DtSource);

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfIniDetailGrid : 初始化明細Grid
        /// <summary>
        /// 初始化明細頁的 Grid 
        /// </summary>
        /// <param name="pTabindex">tab 索引</param>
        /// <returns>回傳grid</returns>
        protected virtual UltraGrid WfIniDetailGrid(int pTabindex)
        {
            Infragistics.Win.UltraWinGrid.UltraGrid uGrid = null;
            try
            {
                //if (DRMASTER == null) return;

                //uGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
                uGrid = uGridDetail;
                WfSetAppearance(uGrid, 1);
                uGrid.KeyUp += new KeyEventHandler(ultraGrid_KeyUp);
                uGrid.AfterCellActivate += new System.EventHandler(this.UGridDetail_AfterCellActivate);
                uGrid.ClickCellButton += new CellEventHandler(UltraGrid_ClickCellButton);
                uGrid.BeforeExitEditMode += new Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventHandler(UltraGrid_BeforeExitEditMode);
                uGrid.AfterEnterEditMode += new System.EventHandler(UGridDetail_AfterEnterEditMode);
                uGrid.BeforeRowActivate += new RowEventHandler(UGridDetail_BeforeRowActivate);
                //uTab_Detail.Tabs[pTabindex].TabPage.Controls.Add(uGrid);
                ((System.ComponentModel.ISupportInitialize)(uGrid)).EndInit();
                // 依頁面索引初始化 Grid
                uGrid.Name = string.Format("uGrid_Detail{0}", pTabindex.ToString());
                uGrid.TabStop = false;
                return uGrid;
                //switch (pi_tabindex)
                //{
                //    case 0:
                //        lugrid.Name = "uGrid_Detail1";
                //        this.uGridDetail1 = lugrid;
                //        break;
                //    case 1:
                //        lugrid.Name = "uGrid_Detail2";
                //        this.uGridDetail2 = lugrid;
                //        break;
                //    case 2:
                //        lugrid.Name = "uGrid_Detail3";
                //        this.uGridDetail3 = lugrid;
                //        break;
                //    case 3:
                //        lugrid.Name = "uGrid_Detail4";
                //        this.uGridDetail5 = lugrid;
                //        break;
                //    case 4:
                //        lugrid.Name = "uGrid_Detail2";
                //        this.uGridDetail5 = lugrid;
                //        break;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region PICK按鈕相關事件 含grid及 ultratexteditor 及註冊pick熱鍵 及keyup
        protected void UltraGrid_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            DataRowView drvActive;
            DataRow drActive;
            try
            {
                if (FormEditMode == YREditType.NA)
                    return;
                if (e.Cell.Activation != Activation.AllowEdit)
                    return;
                drvActive = (System.Data.DataRowView)e.Cell.Row.ListObject;
                if (drvActive == null)
                    return;
                drActive = drvActive.Row;
                if (FormEditMode == YREditType.查詢)
                    WfPickClickOnQueryMode(sender, e.Cell.Column.Key, drActive);
                else
                {
                    this.IsInGridButtonClick = true;
                    WfSetBllTransaction();
                    if (WfPickClickOnEditMode(sender, e.Cell.Column.Key, drActive) == true)
                    {
                        drActive.EndEdit();
                        EmbeddableEditorBase editor = (sender as UltraGrid).ActiveCell.EditorResolved;
                        if (editor != null)
                        {
                            editor.Value = drActive[e.Cell.Column.Key] == DBNull.Value ? null : drActive[e.Cell.Column.Key];
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.ToString());
            }
            finally
            {
                this.IsInGridButtonClick = false;
            }
        }

        protected override void UltraTextEditor_EditorPickButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            string pickName = "";
            try
            {
                if ((sender as UltraTextEditor).ReadOnly == true)
                    return;
                if (e.Button.Key != null && e.Button.Key.ToLower() == "pick")
                {
                    if (FormEditMode == YREditType.NA)
                        return;

                    if (e.Button.Tag != null)
                    {
                        pickName = e.Button.Tag.ToString();

                        if (FormEditMode == YREditType.查詢)
                            WfPickClickOnQueryMode(sender, pickName, DrMaster);
                        else
                        {
                            WfSetBllTransaction();
                            if (WfPickClickOnEditMode(sender, pickName, DrMaster) == true)
                            {
                                DrMaster.EndEdit();
                                //UltraTextEditor_Validating(sender, new CancelEventArgs());
                                //IsInButtonClick = true;
                                //UltraTextEditor_BeforeExitEditMode(sender, new Infragistics.Win.BeforeExitEditModeEventArgs(false));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.ToString());
            }
        }

        protected virtual bool WfPickClickOnQueryMode(object sender, string pColName, DataRow pDr)
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

        protected override void UltraTextEditor_KeyUp(object sender, KeyEventArgs e)
        {
            //Infragistics.Win.EmbeddableEditorBase emebBase;
            Infragistics.Win.UltraWinEditors.UltraTextEditor ute;
            string pickName = "";
            try
            {
                if (FormEditMode == YREditType.NA)
                    return;

                ute = sender as UltraTextEditor;
                pickName = ute.Tag.ToString();
                if (ute.ReadOnly == true)
                    return;

                switch (e.KeyData)
                {
                    case (Keys.Control | Keys.P):
                        if (FormEditMode == YREditType.查詢)
                            WfPickClickOnQueryMode(sender, pickName, DrMaster);
                        else
                        {
                            WfSetBllTransaction();
                            if (WfPickClickOnEditMode(sender, pickName, DrMaster) == true)
                            {
                                DrMaster.EndEdit();
                                //IsInButtonClick = true;
                                //UltraTextEditor_Validating(sender, new CancelEventArgs());
                            }
                        }
                        break;
                    case (Keys.Enter):
                        if (ute.Multiline == true)
                            return;
                        this.SelectNextControl(ute, true, true, true, true);

                        break;
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }

        protected override void UltraCombo_KeyUp(object sender, KeyEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraCombo ucb;
            try
            {
                if (FormEditMode == YREditType.NA)
                    return;

                ucb = sender as UltraCombo;
                if (ucb.ReadOnly == true)
                    return;

                switch (e.KeyData)
                {
                    case (Keys.Enter):
                        this.SelectNextControl(ucb, true, true, true, true);
                        break;
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }

        protected override void UltraDateTimeEditor_KeyUp(object sender, KeyEventArgs e)
        {
            UltraDateTimeEditor udt;
            try
            {
                if (FormEditMode == YREditType.NA)
                    return;

                udt = sender as UltraDateTimeEditor;
                if (udt.ReadOnly == true)
                    return;

                switch (e.KeyData)
                {
                    case (Keys.Enter):
                        this.SelectNextControl(udt, true, true, true, true);
                        break;
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }

        protected override void UcCheckBox_KeyUp(object sender, KeyEventArgs e)
        {
            UcCheckBox ucb;
            try
            {
                if (FormEditMode == YREditType.NA)
                    return;

                ucb = sender as UcCheckBox;

                switch (e.KeyData)
                {
                    case (Keys.Enter):
                        this.SelectNextControl(ucb, true, true, true, true);
                        break;
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }

        protected void ultraGrid_KeyUp(object sender, KeyEventArgs e)
        {
            //Infragistics.Win.EmbeddableEditorBase emebBase;
            Infragistics.Win.UltraWinGrid.UltraGrid uGrid;
            DataRowView drvActive;
            DataRow drActive;
            string pickName = "";
            UltraGridCell cellActive = null;
            try
            {
                uGrid = sender as UltraGrid;
                if (uGrid.ActiveCell == null)
                    return;

                if (FormEditMode == YREditType.NA)
                    return;
                switch (e.KeyData)
                {
                    case (Keys.Control | Keys.P):
                        cellActive = uGrid.ActiveCell;
                        if (cellActive == null)
                            return;
                        pickName = cellActive.Column.Key;

                        drvActive = (System.Data.DataRowView)uGrid.ActiveCell.Row.ListObject;

                        if (drvActive == null)
                            return;
                        drActive = drvActive.Row;

                        //cellActive.Column.ValueList.DropDown(cellActive.GetUIElement().ClipRect, cellActive.Column.ValueList.SelectedItemIndex, "");

                        if (uGrid.ActiveCell.Activation != Activation.AllowEdit)
                            return;
                        if (FormEditMode == YREditType.查詢)
                            WfPickClickOnQueryMode(sender, pickName, drActive);
                        else
                        {
                            this.IsInGridButtonClick = true;
                            WfSetBllTransaction();
                            if (WfPickClickOnEditMode(sender, pickName, drActive) == true)
                            {
                                drActive.EndEdit();
                                EmbeddableEditorBase editor = (sender as UltraGrid).ActiveCell.EditorResolved;
                                if (editor != null)
                                {
                                    editor.Value = drActive[pickName] == DBNull.Value ? null : drActive[pickName];
                                }
                            }
                        }
                        break;
                    case (Keys.Up):
                        cellActive = uGrid.ActiveCell;
                        if (cellActive == null)
                            return;

                        //日期及下拉選單不處理
                        if (cellActive.Column.DataType == typeof(DateTime) ||
                            cellActive.Column.ValueList != null
                            )
                        {
                            return;
                        }

                        if (!cellActive.IsInEditMode)
                            return;

                        uGrid.PerformAction(UltraGridAction.ExitEditMode);
                        if (IsItemchkValid == false)
                            return;

                        uGrid.PerformAction(UltraGridAction.AboveCell);
                        uGrid.PerformAction(UltraGridAction.EnterEditMode);
                        e.SuppressKeyPress = false;
                        break;
                    case (Keys.Down):
                        cellActive = uGrid.ActiveCell;
                        if (cellActive == null)
                            return;

                        //日期及下拉選單不處理
                        if (cellActive.Column.DataType == typeof(DateTime) ||
                            cellActive.Column.ValueList != null
                            )
                        {
                            return;
                        }

                        if (!cellActive.IsInEditMode)
                            return;
                        uGrid.PerformAction(UltraGridAction.ExitEditMode);
                        if (IsItemchkValid == false)
                            return;
                        uGrid.PerformAction(UltraGridAction.BelowCell);
                        uGrid.PerformAction(UltraGridAction.EnterEditMode);
                        break;
                    case (Keys.Enter):
                        uGrid.PerformAction(UltraGridAction.NextCell);
                        uGrid.PerformAction(UltraGridAction.EnterEditMode);
                        break;
                }

                #region 舊的不使用 沒問題的時候可以刪
                //if (e.KeyData == (Keys.Control | Keys.P))
                //{
                //    var cellActive = uGrid.ActiveCell;
                //    if (cellActive == null)
                //        return;
                //    pickName = cellActive.Column.Key;

                //    drvActive = (System.Data.DataRowView)uGrid.ActiveCell.Row.ListObject;

                //    if (drvActive == null)
                //        return;
                //    drActive = drvActive.Row;

                //    //cellActive.Column.ValueList.DropDown(cellActive.GetUIElement().ClipRect, cellActive.Column.ValueList.SelectedItemIndex, "");

                //    if (uGrid.ActiveCell.Activation != Activation.AllowEdit)
                //        return;
                //    if (FormEditMode == YREditType.查詢)
                //        WfPickClickOnQueryMode(sender, pickName, drActive);
                //    else
                //    {
                //        this.IsInGridButtonClick = true;
                //        WfSetBllTransaction();
                //        if (WfPickClickOnEditMode(sender, pickName, drActive) == true)
                //        {
                //            drActive.EndEdit();
                //            EmbeddableEditorBase editor = (sender as UltraGrid).ActiveCell.EditorResolved;
                //            if (editor != null)
                //            {
                //                editor.Value = drActive[pickName] == DBNull.Value ? null : drActive[pickName];
                //            }
                //        }
                //    }
                //}

                //if (e.KeyData == Keys.Up)
                //{
                //    var cellActive = uGrid.ActiveCell;
                //    if (cellActive == null)
                //        return;

                //    //日期及下拉選單不處理
                //    if (cellActive.Column.DataType == typeof(DateTime) ||
                //        cellActive.Column.ValueList != null
                //        )
                //    {
                //        return;
                //    }

                //    if (!cellActive.IsInEditMode)
                //        return;

                //    uGrid.PerformAction(UltraGridAction.ExitEditMode);
                //    if (IsItemchkValid == false)
                //        return;

                //    uGrid.PerformAction(UltraGridAction.AboveCell);
                //    uGrid.PerformAction(UltraGridAction.EnterEditMode);
                //    e.SuppressKeyPress = false;
                //    return;
                //}

                //if (e.KeyData == Keys.Down)
                //{
                //    var cellActive = uGrid.ActiveCell;
                //    if (cellActive == null)
                //        return;

                //    //日期及下拉選單不處理
                //    if (cellActive.Column.DataType == typeof(DateTime) ||
                //        cellActive.Column.ValueList != null
                //        )
                //    {
                //        return;
                //    }

                //    if (!cellActive.IsInEditMode)
                //        return;
                //    uGrid.PerformAction(UltraGridAction.ExitEditMode);
                //    if (IsItemchkValid == false)
                //        return;
                //    uGrid.PerformAction(UltraGridAction.BelowCell);
                //    uGrid.PerformAction(UltraGridAction.EnterEditMode);
                //} 
                #endregion
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
            finally
            {
                this.IsInGridButtonClick = false;
            }
        }
        #endregion

        #region UGridDetail_AfterCellActivate(object sender, EventArgs e)
        private void UGridDetail_AfterCellActivate(object sender, EventArgs e)
        {
            UltraGrid uGrid;
            UltraGridCell uCellActive;
            string aza03;
            string msg;
            try
            {
                uGrid = sender as UltraGrid;
                if (uGrid.ActiveCell == null)
                    return;

                //WfGetOldValue(sender as Control); //移到 UGridDetail_AfterEnterEditMode
                if (TabDetailList[IntCurTabDetail].AzaTbList == null)
                    return;

                uCellActive = uGrid.ActiveCell;
                aza03 = GlobalFn.isNullRet(uCellActive.Column.Key, "");
                if (aza03 == "")
                    return;
                var azaModel = TabDetailList[IntCurTabDetail].AzaTbList.Where(p => p.aza03 == aza03).FirstOrDefault();
                if (azaModel == null || azaModel.aza03 == null) return;
                msg = azaModel.aza04 + "[" + azaModel.aza03 + "]";

                //這裡是為了讓編輯模式時,讓小數點數字若為0時,不顯示 ex:123.000==>123
                if (azaModel.aza08.ToLower() == "numeric" && uCellActive.Value != DBNull.Value
                        && FormEditMode != YREditType.查詢)
                {
                    WfSetControlEditNumeric(uCellActive);
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
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

        #region UGridDetail_AfterEnterEditMode
        private void UGridDetail_AfterEnterEditMode(object sender, EventArgs e)
        {
            try
            {
                WfGetOldValue(sender as Control);
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

        #region UGridDetail_BeforeRowActivate 進入grid 資料列,主要做為控制前端cell readonly使用
        private void UGridDetail_BeforeRowActivate(object sender, RowEventArgs e)
        {
            DataRow drActive;
            try
            {
                drActive = WfGetUgridDatarow(e.Row);
                //先處理新增修改時的明細列處理
                if (FormEditMode == YREditType.修改 || FormEditMode == YREditType.新增)
                    WfSetDetailDisplayMode(this.IntCurTabDetail, e.Row, drActive);

                WfSetControlReadonly(e.Row.Cells["print_label"], false);
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

        #region UltraGrid_BeforeExitEditMode
        protected override void UltraGrid_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {
            string colName = "";
            string dbValue;
            UltraGridCell uGridCell = null;
            DataRowView drvActive = null;
            DataRow drActive;
            try
            {

                if (IsInGridButtonClick == true)        //避免驗證時又觸發事件(目前作用時機為PICK開窗時會造成...)
                    return;
                if (IsInSaveCancle == true || IsInCRUDIni == true)
                    return;

                if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.NA)
                    return;

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

                //if (!uGridCell.Value.Equals(OldValue))
                if (GlobalFn.isNullRet(uGridCell.Value, "") != GlobalFn.isNullRet(OldValue, "")) //20170331 Allen modify:避免null 跟空字串不同
                {
                    WfSetBllTransaction();
                    var itemCheckInfo = new ItemCheckInfo();
                    itemCheckInfo.Row = drActive;
                    itemCheckInfo.Value = uGridCell.Value;
                    itemCheckInfo.Column = colName;

                    #region 檢查欄位型別及輸入長度
                    foreach (TabDetailInfo detailInfo in TabDetailList)
                    {
                        if (detailInfo.DtSource == null || detailInfo.DtSource.Prefix.ToLower() != detailInfo.AzaTbList.FirstOrDefault().aza01)
                            continue;

                        var azaModel = detailInfo.AzaTbList.Where(p => p.aza03.ToLower() == colName.ToLower())
                                            .FirstOrDefault();
                        if (azaModel != null && !GlobalFn.varIsNull(azaModel.aza01))
                        {
                            if (azaModel.aza08 == "nvarchar")
                            {
                                //檢查欄位長度
                                if (azaModel.aza09 < GlobalFn.isNullRet(itemCheckInfo.Value, "").Length)
                                {
                                    WfShowErrorMsg(string.Format("僅可輸入{0}個字元,目前{1}!", azaModel.aza09.ToString(),
                                                                                GlobalFn.isNullRet(itemCheckInfo.Value, "").Length));
                                    e.Cancel = true;
                                    IsItemchkValid = false;
                                    uGridCell.Value = OldValue;
                                    return;
                                }
                            }
                            else if (azaModel.aza08 == "int" || azaModel.aza08 == "numeric")
                            {
                                //檢查型別
                                decimal parseDecimal = 0;
                                if (!decimal.TryParse(GlobalFn.isNullRet(itemCheckInfo.Value, "0"), out parseDecimal))
                                {
                                    WfShowErrorMsg(string.Format("請輸入數字!"));
                                    e.Cancel = true;
                                    IsItemchkValid = false;
                                    uGridCell.Value = OldValue;
                                    return;
                                }
                                //檢查長度
                                var valueArray = parseDecimal.ToString().Split('.');
                                if (azaModel.aza10 < valueArray[0].Length)    //判斷整數
                                {
                                    WfShowErrorMsg(string.Format("整數位數僅可輸入{0}個字元,目前{1}!", azaModel.aza10.ToString(),
                                                                                valueArray[0].Length));
                                    e.Cancel = true;
                                    IsItemchkValid = false;
                                    uGridCell.Value = OldValue;
                                    return;
                                }
                                if (valueArray.Length == 2)
                                {
                                    if (azaModel.aza11 < valueArray[1].Length)    //判斷小數位數
                                    {
                                        WfShowErrorMsg(string.Format("小數位數僅可輸入{0}個字元,目前{1}!", azaModel.aza11.ToString(),
                                                                                    valueArray[1].Length));
                                        e.Cancel = true;
                                        IsItemchkValid = false;
                                        uGridCell.Value = OldValue;
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    #endregion


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
        }
        #endregion

        #region WfSetTabInfoDataSource(pi_tabindex, ptabinfo, pdt) : 設定明細頁面的數據源
        protected Boolean WfSetTabInfoDataSource(int pTabindex, TabDetailInfo pTabinfo, DataTable pDt, string pTableName, string pViewName)
        {
            pTabinfo.DtSource = pDt;
            pTabinfo.DtSource.TableName = pTableName;
            pTabinfo.DtSource.Prefix = pViewName;
            pTabinfo.UGrid.DataSource = pDt;
            //20161103 Allen Fix:不重新bind
            pTabinfo.UGrid.DataBind();  //重新bind 為了讓grid可取得active row
            try
            {
                if (pTabinfo != null && pTabinfo.IsGridFormatFinshed == false && pTabinfo.AzaTbList != null)
                    if (pTabinfo.IsGridFormatFinshed == false)
                    {
                        this.WfSetGridHeader(pTabinfo.UGrid, pTabinfo.AzaTbList, this.DateFormat);
                        WfSeDetailGridLayout(pTabinfo.UGrid);
                        WfSetGridButtomRowCount(pTabinfo.UGrid);
                        pTabinfo.IsGridFormatFinshed = true;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //if (strMode != YREditType.查詢)
            //{
            //    pdt.ColumnChanging += new DataColumnChangeEventHandler(this.datacolumn_changing_event);
            //    pdt.ColumnChanged += new DataColumnChangeEventHandler(this.datacolumn_changed_event);
            //}
            //else//查詢時,將所有的型別丟到 column.prefix
            //{
            //    pdt.ColumnChanging += new DataColumnChangeEventHandler(this.datacolumn_changing_query_event);
            //    foreach (DataColumn ldc_temp in pdt.Columns)
            //    {
            //        ldc_temp.Prefix = ldc_temp.DataType.Name;
            //        ldc_temp.DataType = System.Type.GetType("System.String");
            //    }
            //}
            return true;
        }
        #endregion

        #region WfSetMasterGridLayout 僅一開始觸發
        protected virtual void WfSeDetailGridLayout(UltraGrid pUgrid)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    WFInsertDetail();

        //    string msg = "";
        //    msg = string.Format("i");


        //}

        #region WfFireControlValidated 讓控制項強制離開編輯並產生驗證
        protected void WfFireControlValidated(Control pControl)
        {
            string typeName;
            Control activeControl;
            try
            {
                typeName = pControl.GetType().ToString();

                if (typeName == "Infragistics.Win.EmbeddableTextBoxWithUIPermissions")
                {
                    typeName = pControl.Parent.GetType().ToString();
                    activeControl = pControl.Parent;
                }
                else
                {
                    activeControl = pControl;
                }

                switch (typeName)
                {
                    case "Infragistics.Win.UltraWinGrid.UltraGrid":
                        (activeControl as Infragistics.Win.UltraWinGrid.UltraGrid).PerformAction(UltraGridAction.ExitEditMode);
                        break;
                    case "Infragistics.Win.UltraWinEditors.UltraTextEditor":
                        UltraTextEditor_Validating(activeControl, new CancelEventArgs());
                        //UltraTextEditor_BeforeExitEditMode(actContrl, new Infragistics.Win.BeforeExitEditModeEventArgs(true));
                        break;
                    case "YR.Util.Controls.UcCheckBox":
                        UcCheckBox_Validating(activeControl, new CancelEventArgs());
                        break;
                    case "Infragistics.Win.UltraWinGrid.UltraCombo":
                        UltraCombo_Validating(activeControl, new CancelEventArgs());
                        break;
                    case "Infragistics.Win.UltraWinEditors.UltraDateTimeEditor":
                        UltraDateTimeEditor_Validating(activeControl, new CancelEventArgs());
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPreInsertDetailCheck() :新增明細資料前檢查
        protected virtual bool WfPreInsertDetailCheck(int pCurTabDetail)
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

        #region WfPreInInsertModeCheck 進新增模式前的檢查及清變數與設定變數
        protected bool WfPreInInsertModeCheck()
        {
            try
            {
                //sgaOrg = null;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfToolbarDetailInsert() : 新增明細行
        protected DataRow WfToolbarDetailInsert()
        {
            // 先獲取 Grid 組件當前列的 ID
            int activeIndex = -1;
            int iDone = 0;
            DataRow dr = null;
            UltraGrid uGrid = TabDetailList[IntCurTabDetail].UGrid;
            if (uGrid == null)
            {
                WfShowErrorMsg("請選擇要新增明細的頁面!");
                return null;
            }

            if (TabDetailList[IntCurTabDetail].IsReadOnly)
            {
                WfShowErrorMsg(string.Format("頁面 [{0}] 只可查詢無法修改!", IntCurTabDetail));
                return null;
            }

            if (uGrid.ActiveRow != null)
            {
                activeIndex = uGrid.ActiveRow.Index;
            }
            else
            { activeIndex = -1; }

            if (this.IntCurTabDetail < 0) return null;
            if (TabDetailList[this.IntCurTabDetail].DtSource == null) return null;
            dr = TabDetailList[this.IntCurTabDetail].DtSource.NewRow();
            TabDetailList[this.IntCurTabDetail].DtSource.Rows.Add(dr);

            if (this.WfSetDetailRowDefault(this.IntCurTabDetail, dr) == false)
                return dr;

            #region 定位在新列
            if (uGrid.Rows.Count == 1)
            {
                uGrid.ActiveRow = uGrid.Rows[0];
            }
            else
            {
                //如次一列是新列，則定位次一列
                if (iDone == 0)
                {
                    try
                    {
                        DataRowView drv = (DataRowView)uGrid.Rows[activeIndex + 1].ListObject;
                        if (drv.Row.RowState == DataRowState.Added)
                        {
                            WfSetGridActiveRow(uGrid, activeIndex + 1);
                            iDone = 1;
                        }
                    }
                    catch { }
                }

                //如前一列是新列，則定位前一列
                if (iDone == 0 && activeIndex > 0)
                {
                    try
                    {
                        DataRowView drv = (DataRowView)uGrid.Rows[activeIndex - 1].ListObject;
                        if (drv.Row.RowState == DataRowState.Added)
                        {
                            WfSetGridActiveRow(uGrid, activeIndex - 1);
                            iDone = 1;
                        }
                    }
                    catch { }
                }
                //定位, 如當前列是新列，則定位在當前列
                if (iDone == 0 && activeIndex >= 0)
                {
                    try
                    {
                        DataRowView drv = (DataRowView)uGrid.Rows[activeIndex].ListObject;
                        if (drv.Row.RowState == DataRowState.Added)
                        {
                            WfSetGridActiveRow(uGrid, activeIndex);
                            iDone = 1;
                        }
                    }
                    catch { }
                }

                //最後一列是新列
                if (iDone == 0 && uGrid.Rows.Count > 0)
                {
                    try
                    {
                        DataRowView drv = (DataRowView)uGrid.Rows[uGrid.Rows.Count - 1].ListObject;
                        if (drv.Row.RowState == DataRowState.Added)
                        {
                            WfSetGridActiveRow(uGrid, uGrid.Rows.Count - 1);
                            iDone = 1;
                        }
                    }
                    catch { }
                }
                if (iDone == 0 && uGrid.Rows.Count > 0)
                {
                    try
                    {
                        DataRowView drv = (DataRowView)uGrid.Rows[0].ListObject;
                        if (drv.Row.RowState == DataRowState.Added)
                        {
                            WfSetGridActiveRow(uGrid, 0);
                            iDone = 1;
                        }
                    }
                    catch { }
                }
            }
            #endregion 定位在新列

            WfSetFirstVisibelCellFocus(uGrid);

            this.WfAfterDetailInsert(this.IntCurTabDetail, dr);
            this.IsChanged = true;

            return dr;
        }
        #endregion

        #region WfSetDetailRowDefault(int pCurTabDetail, DataRow pDr) 設定明細資料列的初始值
        protected virtual bool WfSetDetailRowDefault(int pCurTabDetail, DataRow pDr)
        {
            try
            {

                switch (pCurTabDetail)
                {
                    case 0:
                        pDr["sgb02"] = WfGetMaxSeq(pDr.Table, "sgb02");
                        pDr["sgb05"] = 0;
                        pDr["sgb08"] = 0;
                        pDr["sgb09"] = 0;
                        pDr["sgb10"] = 0;
                        pDr["sgb10t"] = 0;
                        pDr["sgb13"] = 0;
                        pDr["sgb14"] = 0;
                        pDr["sgb17"] = 0;
                        pDr["sgb18"] = 0;
                        pDr["sgb19"] = 0;
                        pDr["sgbcomp"] = LoginInfo.CompNo;
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAfterDetailInsert() :新增明細資料後調用
        protected virtual void WfAfterDetailInsert(int pCurTabDetail, DataRow pDr)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 相關控制項Validating事件 方法
        #region UltraTextEditor_Validating
        protected override void UltraTextEditor_Validating(object sender, CancelEventArgs e)
        {
            string colName = "";
            string dbValue = "";
            UltraTextEditor control = sender as UltraTextEditor;
            try
            {
                if (IsInItemchecking == true)
                    return;

                if (IsInSaveCancle == true || IsInCRUDIni == true)
                    return;

                if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.NA)
                    return;


                WfCleanBottomMsg();
                IsInItemchecking = true;
                IsItemchkValid = true;
                colName = control.Tag.ToString();
                dbValue = DrMaster[colName] == null ? "" : DrMaster[colName].ToString();


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
                //if (control.Value != OldValue)
                if (GlobalFn.isNullRet(control.Value, "") != GlobalFn.isNullRet(OldValue, ""))      //20170330 Allen modify:均轉為字串比較
                {
                    WfSetBllTransaction();
                    var itemCheckInfo = new ItemCheckInfo();
                    itemCheckInfo.Row = DrMaster;
                    itemCheckInfo.Value = control.Value;
                    itemCheckInfo.Column = colName;

                    #region 檢查欄位型別及輸入長度
                    if (control.Tag != null && !GlobalFn.varIsNull(control.Tag))
                    {
                        var azaModel = TabMaster.AzaTbList.Where(p => p.aza03.ToLower() == colName.ToLower())
                                            .FirstOrDefault();
                        if (azaModel != null && !GlobalFn.varIsNull(azaModel.aza01))
                        {
                            if (azaModel.aza08 == "nvarchar")
                            {
                                //檢查欄位長度
                                if (azaModel.aza09 < GlobalFn.isNullRet(itemCheckInfo.Value, "").Length)
                                {
                                    WfShowErrorMsg(string.Format("僅可輸入{0}個字元,目前{1}!", azaModel.aza09.ToString(),
                                                                                GlobalFn.isNullRet(itemCheckInfo.Value, "").Length));
                                    e.Cancel = true;
                                    IsItemchkValid = false;
                                    control.Value = OldValue;
                                    return;
                                }
                            }
                            else if (azaModel.aza08 == "int" || azaModel.aza08 == "numeric")
                            {
                                //檢查型別
                                decimal parseDecimal = 0;
                                if (!decimal.TryParse(GlobalFn.isNullRet(itemCheckInfo.Value, "0"), out parseDecimal))
                                {
                                    WfShowErrorMsg(string.Format("請輸入數字!"));
                                    e.Cancel = true;
                                    IsItemchkValid = false;
                                    control.Value = OldValue;
                                    return;
                                }
                                //檢查長度
                                var valueArray = parseDecimal.ToString().Split('.');
                                if (azaModel.aza10 < valueArray[0].Length)    //判斷整數
                                {
                                    WfShowErrorMsg(string.Format("整數位數僅可輸入{0}個字元,目前{1}!", azaModel.aza10.ToString(),
                                                                                valueArray[0].Length));
                                    e.Cancel = true;
                                    IsItemchkValid = false;
                                    control.Value = OldValue;
                                    return;
                                }
                                if (valueArray.Length == 2)
                                {
                                    if (azaModel.aza11 < valueArray[1].Length)    //判斷小數位數
                                    {
                                        WfShowErrorMsg(string.Format("小數位數僅可輸入{0}個字元,目前{1}!", azaModel.aza11.ToString(),
                                                                                    valueArray[1].Length));
                                        e.Cancel = true;
                                        IsItemchkValid = false;
                                        control.Value = OldValue;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    if (WfItemCheck(sender, itemCheckInfo) == false)
                    {
                        e.Cancel = true;
                        IsItemchkValid = false;
                        control.Value = OldValue;
                        return;
                    }
                    else
                    {
                        IsChanged = true;
                        DrMaster.EndEdit();
                        return;
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
        }
        #endregion

        #region UltraCombo_Validating
        protected override void UltraCombo_Validating(object sender, CancelEventArgs e)
        {
            string colName = "";
            string dbValue;
            string displayValue;
            UltraCombo control = sender as UltraCombo;
            try
            {
                if (IsInSaveCancle == true || IsInCRUDIni == true)
                    return;

                if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.NA)
                    return;

                WfCleanBottomMsg();
                IsItemchkValid = true;
                colName = control.Tag.ToString();
                dbValue = DrMaster[colName] == null ? "" : DrMaster[colName].ToString();
                displayValue = control.Value == null ? "" : control.Value.ToString();
                if (control.Value != OldValue)
                {
                    WfSetBllTransaction();
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
        }
        #endregion

        #region UcCheckBox_Validating
        protected override void UcCheckBox_Validating(object sender, CancelEventArgs e)
        {
            string colName = "";
            string dbValue = "";
            //string displayValue = "";
            YR.Util.Controls.UcCheckBox control = sender as YR.Util.Controls.UcCheckBox;
            try
            {
                if (IsInSaveCancle == true || IsInCRUDIni == true)
                    return;

                if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.NA)
                    return;

                WfCleanBottomMsg();
                IsItemchkValid = true;
                colName = control.Tag.ToString();
                dbValue = DrMaster[colName] == null ? "" : DrMaster[colName].ToString();
                //displayValue = control.CheckValue == null ? "" : control.CheckValue.ToString();

                if (control.CheckValue != OldValue)
                {
                    WfSetBllTransaction();
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
        }
        #endregion

        #region UltraDateTimeEditor_Validating 時間控制項
        protected override void UltraDateTimeEditor_Validating(object sender, CancelEventArgs e)
        {
            string colName = "";
            //DateTime? dbValue = null;
            UltraDateTimeEditor control = sender as UltraDateTimeEditor;
            try
            {
                if (IsInSaveCancle == true || IsInCRUDIni == true)
                    return;

                if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.NA)
                    return;

                WfCleanBottomMsg();
                IsInItemchecking = true;
                IsItemchkValid = true;
                colName = control.Tag.ToString();

                if (GlobalFn.isNullRet(control.Value, "") != GlobalFn.isNullRet(OldValue, ""))
                {
                    WfSetBllTransaction();

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
        #endregion

        #region WfToolbarSave : 工具列存檔 function
        /// <summary>
        /// 工具列存檔 function
        /// </summary>
        /// <returns></returns>
        protected Boolean WfToolbarSave()
        {
            //if (this.isDisplayOnly)
            //{ return true; }

            if (DrMaster == null)
            {
                WfShowErrorMsg("查無要存檔的資料!");
                return false;
            }

            WfCleanErrorProvider();
            BindingMaster.EndEdit();

            //if (this.isChanged)
            //{
            try
            {
                if (this.WfBeginTran() == false)
                { return false; }
                WfSetBllTransaction();

                if (this.WfFormCheck() == false)
                {
                    WfRollback();
                    return false;
                }

                if (this.WfAfterFormCheck() == false)
                {
                    WfRollback();
                    return false;
                }

                if (this.TabMaster.IsReadOnly != true)
                {
                    if (this.TabMaster.DtSource.GetChanges() != null)
                    {
                        if (this.TabMaster.BoBasic.OfUpdate(BoMaster.TRAN, this.TabMaster.DtSource.GetChanges()) < 0)
                        {
                            { throw new Exception("儲存主表時失敗(boBasic.of_update)，請檢核 !"); }
                        }
                    }
                }

                for (int i = 0; i < IntTabDetailCount; i++)
                {
                    if (TabDetailList[i].IsReadOnly == true) continue;
                    if (TabDetailList[i].DtSource == null) { continue; }
                    if (TabDetailList[i].DtSource.GetChanges() == null) continue;

                    TabDetailList[i].BoBasic.TRAN = BoMaster.TRAN;

                    if (TabDetailList[i].BoBasic.OfUpdate(BoMaster.TRAN, TabDetailList[i].DtSource.GetChanges()) < 0)
                    {
                        { throw new Exception("儲存明細時失敗，請檢核 !"); }
                    }
                }

                if (!this.WfAppendUpdate())
                    throw (new Exception("同步資料時發生錯誤(wf_append_update)，請檢核 !"));

                if (this.WfCommit() == false)
                { throw new Exception("確認 (commit) 交易時發生錯誤，請檢核 !"); }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                this.WfRollback();
                DrMaster.CancelEdit();
                BindingMaster.CancelEdit();
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
                this.WfRollback();
                DrMaster.CancelEdit();
                BindingMaster.CancelEdit();
                throw ex;
            }

            // commit datatable changes
            this.TabMaster.DtSource.AcceptChanges();

            if (TabDetailList != null)
            {
                foreach (TabDetailInfo tbinfo in TabDetailList)
                {
                    if (tbinfo.DtSource == null) { continue; }
                    tbinfo.DtSource.AcceptChanges();
                }
                //WfRetrieveDetail();
            }

            this.FormEditMode = YREditType.NA;
            WfShowBottomStatusMsg("存檔成功!");
            //WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
            this.IsChanged = false;
            return true;
        }
        #endregion

        #region WfRetrieveDetail : 查詢更新 detail 資料
        /// <summary>
        /// 更新 detail 資料
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean WfRetrieveDetail()
        {
            if (TabDetailList == null) return true;

            int tabindex = this.TabDetailList.Count - 1;
            while (tabindex >= 0)
            {
                this.WfRetrieveDetail(tabindex);

                tabindex--;
            }
            return true;
        }
        #endregion

        #region WfRetrieveDetail(tabindex) : 更新指定頁面的 detail 資料
        /// <summary>
        /// 更新 detail 資料
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean WfRetrieveDetail(int pTabindex)
        {
            Infragistics.Win.UltraWinGrid.UltraGrid uGrid = null;
            string strWhere = "";
            string tableName = "";
            string strSelect = "";
            DataTable dt = null;

            // 取得頁面資料 tabinformaion
            TabDetailInfo tabinfo = this.TabDetailList[pTabindex];
            // 如主表 activerow 為空，或未定義關聯參數，則返回
            if (this.uGridMaster.ActiveRow == null || this.uGridMaster.ActiveRow.Cells == null || tabinfo.RelationParams == null)
            {
                if (this.TabDetailList[pTabindex].DtSource != null)
                {
                    this.TabDetailList[pTabindex].DtSource.Rows.Clear();
                }
                return true;
            }

            // 未定義 TargetTable 或 ViewTable 則返回
            if (GlobalFn.isNullRet(tabinfo.ViewTable, "") == "")
            { tabinfo.ViewTable = tabinfo.TargetTable; }

            if (GlobalFn.isNullRet(tabinfo.ViewTable, "") == "")
            { return true; }

            //20161129 Allen add:增加可自定義單身Sql
            if (!GlobalFn.varIsNull(tabinfo.SelectSql))
                strSelect = tabinfo.SelectSql;
            else
                strSelect = " SELECT " + tabinfo.TargetColumn + " FROM " + tabinfo.ViewTable;

            foreach (SqlParameter param in tabinfo.RelationParams)
            {
                strWhere += " AND " + param.ParameterName + " = @" + param.ParameterName;

                if (this.uGridMaster.ActiveRow == null || this.uGridMaster.ActiveRow.Cells[param.SourceColumn].Value == DBNull.Value)
                {
                    if (param.SqlDbType == SqlDbType.Int || param.SqlDbType == SqlDbType.Decimal)
                    {
                        param.Value = 999999;
                    }
                    else
                        param.Value = "XYZ!123";
                }
                else
                    param.Value = this.uGridMaster.ActiveRow.Cells[param.SourceColumn].Value;
            }
            strWhere = " WHERE 1=1 " + strWhere;
            if (GlobalFn.isNullRet(this.TabDetailList[pTabindex].QueryWhereString, "") != "")
                strWhere = strWhere + " " + this.TabDetailList[pTabindex].QueryWhereString;

            dt = this.TabMaster.BoBasic.OfGetDataTable(strSelect + strWhere, tabinfo.RelationParams.ToArray());

            if (dt == null)
            { return false; }

            uGrid = TabDetailList[pTabindex].UGrid;
            tableName = string.Format(string.Format("tb_grid{0}", pTabindex.ToString()));//目前已不使用,之後視情況來移除

            this.WfSetTabInfoDataSource(pTabindex, this.TabDetailList[pTabindex], dt, tableName, this.TabDetailList[pTabindex].ViewTable);
            return true;
        }
        #endregion

        //*****************************原本應ovverideFuction****************************************

        #region WfDisplayMode 新增修改刪除後的readonly顯示
        protected override Boolean WfDisplayMode()
        {
            vw_stpt410 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_stpt410>();
                    //WfSetDocPicture("", masterModel.sgaconf, masterModel.sgastat, pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.sga10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekTbModel = BoBas.OfGetBekModel(masterModel.sga10);
                        if (BekTbModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.sga10));
                        }
                    }
                }
                else
                {
                    //WfSetDocPicture("", "", "", pbxDoc);
                }

                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false); //先全開
                    //WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    //WfSetControlReadonly(new List<Control> { udt_sga02, udt_sga25 }, true);

                    WfSetControlReadonly(new List<Control> { ute_sga03_c, ute_sga04_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_sga13t }, true);
                    //WfSetControlReadonly(new List<Control> { ute_sgamodu, ute_sgamodg, udt_sgamodd }, true);
                    //WfSetControlReadonly(new List<Control> { ute_sgasecu, ute_sgasecg }, true);

                    //if (GlobalFn.isNullRet(masterModel.sga01, "") != "")
                    //{
                    //    WfSetControlReadonly(ute_sga01, true);
                    //    WfSetSga01RelReadonly(GlobalFn.isNullRet(masterModel.sga01, ""));
                    //}
                    //else
                    //    WfSetControlReadonly(ute_sga19, true);

                    //if (!GlobalFn.varIsNull(masterModel.sga19)) //客戶狀態處理
                    //    WfSetControlReadonly(ute_sga03, true);

                    //WfSetControlReadonly(new List<Control> { ute_sga01_c, ute_sga03_c, ute_sga04_c, ute_sga05_c, ute_sga11_c, ute_sga12_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_sga07, ucx_sga08 }, true);
                    //WfSetControlReadonly(new List<Control> { ute_sga13, ute_sga13t, ute_sga13g }, true);
                    //WfSetControlReadonly(new List<Control> { ute_sga14_c, ute_sga15_c, ute_sga16_c }, true);
                    //WfSetControlReadonly(new List<Control> { ute_sga22 }, true);
                    //WfSetControlReadonly(new List<Control> { ucb_sgaconf, udt_sgacond, ute_sgaconu, ute_sgastat }, true);
                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改 || FormEditMode == YREditType.新增)
                    {
                        //WfSetControlReadonly(ute_sga01, true);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAfterfDisplayMode  新增修改刪除後的 focus調整
        protected virtual void WfAfterfDisplayMode()
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetBllTransaction 以bomaster 註冊transaction至其他 bll
        protected virtual void WfSetBllTransaction()
        {
            try
            {
                BoStp.TRAN = BoMaster.TRAN;
                BoBas.TRAN = BoMaster.TRAN;
                BoInv.TRAN = BoMaster.TRAN;
                BoAdm.TRAN = BoMaster.TRAN;
                BoTax.TRAN = BoMaster.TRAN;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfIniDetailComboSource 依detail處理下拉來源
        protected virtual void WfIniDetailComboSource(int pTabIndex)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetDetailDisplayMode 新增修改時的明細列可輸入處理,需要每筆資料列微調整時再使用
        protected void WfSetDetailDisplayMode(int pCurTabDetail, UltraGridRow pUgr, DataRow pDr)
        {
            string columnName;
            //bab_tb babModel;
            try
            {
                switch (pCurTabDetail)
                {
                    case 0:
                        foreach (UltraGridCell ugc in pUgr.Cells)
                        {
                            columnName = ugc.Column.Key.ToLower();
                            //先控可以輸入的
                            if (
                                columnName == "sgb05" ||
                                columnName == "sgb09" ||
                                columnName == "sgb16"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            //if (columnName == "sgb02")
                            //{
                            //    if (pDr.RowState == DataRowState.Added)//新增時
                            //        WfSetControlReadonly(ugc, false);
                            //    else    //修改時
                            //    {
                            //        WfSetControlReadonly(ugc, true);
                            //    }
                            //    continue;
                            //}

                            //if (columnName == "sgb03" ||
                            //    columnName == "sgb11" ||
                            //    columnName == "sgb12"
                            //    )
                            //{
                            //    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sga01"], ""));
                            //    if (babModel == null)
                            //    {
                            //        WfShowErrorMsg("請先輸銷貨單單頭資料!");
                            //    }
                            //    if (GlobalFn.isNullRet(babModel.bab08, "") == "Y")    //有來源單據
                            //    {
                            //        if (columnName == "sgb03")//料號
                            //            WfSetControlReadonly(ugc, true);    //不可輸入
                            //        else
                            //            WfSetControlReadonly(ugc, false);
                            //    }
                            //    else
                            //    {
                            //        if (columnName == "sgb03")//料號
                            //            WfSetControlReadonly(ugc, false);    //可輸入
                            //        else
                            //            WfSetControlReadonly(ugc, true);
                            //    }
                            //    continue;
                            //}

                            WfSetControlReadonly(ugc, true);    //其餘的都關閉
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfIniTabDetailInfo(): 設定明細的資料來源
        protected Boolean WfIniTabDetailInfo()
        {
            SqlParameter keyParm;

            this.TabDetailList[0].TargetTable = "sgb_tb";
            this.TabDetailList[0].ViewTable = "vw_stpt410s";
            keyParm = new SqlParameter("sgb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "sga01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfSetMasterRowDefault(DataRow pDr) 設定MasterRow的初始值
        protected virtual bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["sga01"] = "1332";  //先寫死

                pDr["sga02"] = Today;
                pDr["sga04"] = LoginInfo.UserNo;
                pDr["sga04_c"] = LoginInfo.UserName;
                pDr["sga05"] = LoginInfo.DeptNo;
                pDr["sga05_c"] = LoginInfo.DeptName;
                pDr["sga07"] = 0;
                pDr["sga08"] = "N";
                pDr["sga10"] = BaaModel.baa04;
                pDr["sga13"] = 0;
                pDr["sga13t"] = 0;
                pDr["sga13g"] = 0;
                pDr["sga21"] = 1;       //匯率
                pDr["sga23"] = 0;       //出貨成本
                pDr["sga25"] = Today;   //訂單日期
                pDr["sga27"] = Today;   //發票日期
                pDr["sgaconf"] = "N";
                pDr["sgastat"] = "1";
                pDr["sgacomp"] = LoginInfo.CompNo;

                //預設客人資料
                pDr["sga03"] = "C000001";    //門市客人
                if ((rdb_01.Checked == false && rdb_02.Checked == false && rdb_03.Checked == false
                        && rdb_04.Checked == false && rdb_05.Checked == false
                        && rdb_06.Checked == false && rdb_07.Checked == false
                        )
                    || rdb_01.Checked == true)
                    pDr["sga03"] = "C000001";    //門市客人
                else if (rdb_02.Checked == true)
                    pDr["sga03"] = "C000002";    //蝦皮客人
                else if (rdb_03.Checked == true)
                    pDr["sga03"] = "C000003";    //露天客人
                else if (rdb_04.Checked == true)
                    pDr["sga03"] = "C000004";    //奇摩客人
                else if (rdb_05.Checked == true)
                    pDr["sga03"] = "C000005";    //松果客人
                else if (rdb_06.Checked == true)
                    pDr["sga03"] = "C000006";    //蝦皮客人2
                else if (rdb_07.Checked == true)
                    pDr["sga03"] = "C000007";    //露天艾達

                WfSetSga03Relation(pDr["sga03"].ToString());
                pDr["sgb16_pick"] = "400A";    //慣用倉庫 先寫死

                //發票別
                if ((rdb_invoice1.Checked == false && rdb_invoice2.Checked == false && rdb_invoice3.Checked == false)
                    || rdb_invoice1.Checked == true)
                    pDr["sga26"] = "";    //清空
                else if (rdb_invoice2.Checked == true)
                    pDr["sga26"] = "I01";    //艾達
                else if (rdb_invoice3.Checked == true)
                    pDr["sga26"] = "Z01";    //正中
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int iChkCnts = 0;
            ica_tb icaModel = null;
            vw_stpt410 masterModel = null;
            vw_stpt410s detailModel = null, newDetailModel = null;
            List<vw_stpt410s> detailList = null;
            bab_tb babModel = null;
            sfa_tb sfaModel = null;
            UltraGrid uGrid = null;
            int ChkCnts = 0;
            decimal price = 0;
            Result result = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            DataRow dr;
            string ica01;
            decimal sga23 = 0;
            sga_tb sgaModel;
            MessageInfo messageModel = new MessageInfo();
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt410>();
                sgaModel = DrMaster.ToItem<sga_tb>();
                //if (e.Column.ToLower() != "sga01" && GlobalFn.isNullRet(DrMaster["sga01"], "") == "")
                //{
                //    WfShowErrorMsg("請先輸入單別!");
                //    return false;
                //}
                #region 單頭-vw_stpt410
                if (e.Row.Table.Prefix.ToLower() == "vw_stpt410")
                {
                    switch (e.Column.ToLower())
                    {
                        case "sga01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "stp", "30") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            //e.Row["sga01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            break;
                        case "sga03"://客戶
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sga03_c"] = "";
                                return true;
                            }

                            if (BoStp.OfChkScaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此客戶資料,請檢核!");
                                return false;
                            }
                            DrMaster["sga03_c"] = BoStp.OfGetSca03(e.Value.ToString());
                            WfSetSga03Relation(GlobalFn.isNullRet(e.Value, ""));
                            //自動計算成本
                            if (BoStp.OfChkEbusinessPlateForm(masterModel.sga03) == false)
                            {
                                DrMaster["sga23"] = 0;
                            }
                            else
                            {
                                result = BoStp.OfGetSga23(sgaModel, out sga23);
                                DrMaster["sga23"] = sga23;
                            }
                            break;

                        case "sga04"://業務人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sga04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["sga04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sgb04_pick":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;

                            ica01 = e.Value.ToString();//用這個來調整料號或是原廠料號 如為原廠料號會取出該料號來替代
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {

                                //再檢查是否有原廠料號
                                sbSql = new StringBuilder();
                                //sbSql.AppendLine("SELECT COUNT(1) FROM ica_tb");
                                //sbSql.AppendLine("WHERE ica21=@ica21");
                                //sbSql.AppendLine("AND icaconf='Y' AND icavali='Y'");
                                sbSql.AppendLine("SELECT COUNT(1) FROM ica_tb");
                                sbSql.AppendLine("WHERE icaconf='Y' AND icavali='Y'");
                                sbSql.AppendLine("AND (ica21=@ica21");
                                sbSql.AppendLine("OR EXISTS (");
                                sbSql.AppendLine("SELECT 1");
                                sbSql.AppendLine("FROM icd_tb");
                                sbSql.AppendLine("WHERE ica01=icd01");
                                sbSql.AppendLine(" AND icd02=@ica21)");
                                sbSql.AppendLine(")");

                                sqlParmList = new List<SqlParameter>();
                                sqlParmList.Add(new SqlParameter("@ica21", e.Value));

                                iChkCnts = GlobalFn.isNullRet(BoMaster.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                                if (iChkCnts == 0)
                                {
                                    WfShowErrorMsg("無此料號或原廠料號!");
                                    return false;
                                }

                                if (iChkCnts == 1)
                                {
                                    sbSql = new StringBuilder();
                                    //sbSql.AppendLine("SELECT TOP 1 * FROM ica_tb");
                                    //sbSql.AppendLine("WHERE ica21=@ica21");
                                    //sbSql.AppendLine("AND icaconf='Y' AND icavali='Y'");
                                    sbSql.AppendLine("SELECT TOP 1 * FROM ica_tb");
                                    sbSql.AppendLine("WHERE icaconf='Y' AND icavali='Y'");
                                    sbSql.AppendLine("AND (ica21=@ica21");
                                    sbSql.AppendLine("OR EXISTS (");
                                    sbSql.AppendLine("SELECT 1");
                                    sbSql.AppendLine("FROM icd_tb");
                                    sbSql.AppendLine("WHERE ica01=icd01");
                                    sbSql.AppendLine(" AND icd02=@ica21)");
                                    sbSql.AppendLine(")");

                                    sqlParmList = new List<SqlParameter>();
                                    sqlParmList.Add(new SqlParameter("@ica21", e.Value));
                                    dr = BoMaster.OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                                    icaModel = dr.ToItem<ica_tb>();
                                    ica01 = icaModel.ica01;
                                }
                                else //假如有多筆原廠料號時，跳出料號視窗做選擇
                                {
                                    messageModel = new MessageInfo();
                                    messageModel.IsAutoQuery = true;
                                    messageModel.ParamSearchList = new List<SqlParameter>();
                                    messageModel.ParamSearchList.Add(new SqlParameter("@ica21", e.Value));

                                    //WfShowPickUtility("p_ica21", messageModel);
                                    WfShowPickUtility("p_ica21a", messageModel);
                                    if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                    {
                                        if (messageModel.DataRowList.Count > 0)
                                            ica01 = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                        else
                                            return false;
                                    }
                                    else
                                        return false;
                                }
                            }


                            DataRow newRow = WFInsertDetail();

                            if (newRow != null)
                            {
                                newRow["sgb03"] = ica01;
                                //取得最近採購單價
                                icaModel = BoInv.OfGetIcaModel(ica01);
                                newRow["ica18"] = icaModel.ica18;


                                if (WfSetSgb03Relation(GlobalFn.isNullRet(ica01, ""), newRow) == false)
                                    return false;
                                newRow["sgb05"] = 1;
                                detailModel = newRow.ToItem<vw_stpt410s>();

                                detailModel.sgb05 = BoBas.OfGetUnitRoundQty(detailModel.sgb06, detailModel.sgb05);    //先轉換數量(四捨伍入)
                                newRow["sgb05"] = detailModel.sgb05;
                                if (WfChkSgb05(newRow, detailModel) == false)
                                    return false;
                                newRow["sgb14"] = BoBas.OfGetUnitRoundQty(detailModel.sgb06, detailModel.sgb05 * detailModel.sgb13); //轉換訂單數量(並四拾伍入)
                                newRow["sgb18"] = BoBas.OfGetUnitRoundQty(detailModel.sgb07, detailModel.sgb05 * detailModel.sgb08); //轉換庫存數量(並四拾伍入)

                                //預帶倉庫
                                if (!GlobalFn.varIsNull(masterModel.sgb16_pick))
                                    newRow["sgb16"] = masterModel.sgb16_pick;

                                if (!GlobalFn.varIsNull(masterModel.sga12))
                                {
                                    newDetailModel = newRow.ToItem<vw_stpt410s>();
                                    result = BoStp.OfGetPrice(masterModel.sga12, masterModel.sga03, ica01.ToString(), newDetailModel.sgb06,
                                        masterModel.sga02, masterModel.sga10, "3", newDetailModel.sgb05,
                                        masterModel.sga08, masterModel.sga07, masterModel.sga21, out price);
                                    if (result.Success == true)
                                    {
                                        newRow["sgb09"] = price;
                                    }
                                    else
                                    {
                                        WfShowErrorMsg(result.Message);
                                        return false;
                                    }
                                }
                                //料號為MISC0001 手續費時另外計算以總金額的2%來特別計算
                                if (e.Value.ToString().ToLower() == "misc0001")
                                {
                                    newRow["sgb09"] = GlobalFn.Round(masterModel.sga13t * 0.02m, 0);
                                }


                                WfLoadIcp03(ica01);//取得圖檔
                                WfSetDetailAmt(newRow);
                                WfSetTotalAmt();
                                WfGetQtyTot();
                                WfSetStockQty(newRow);
                                //自動計算成本
                                if (BoStp.OfChkEbusinessPlateForm(masterModel.sga03) == false)
                                {
                                    DrMaster["sga23"] = 0;
                                }
                                else
                                {
                                    result = BoStp.OfGetSga23(sgaModel, out sga23);
                                    DrMaster["sga23"] = sga23;
                                }

                                if (GlobalFn.isNullRet(newRow["stock_qty"], 0) < 2)
                                {
                                    TabDetailList[0].UGrid.ActiveRow.Cells["stock_qty"].Appearance.ForeColor = Color.Red;
                                }

                            }

                            WfItemChkForceFocus(udt_sgb04_pick);
                            e.Row["sgb04_pick"] = "";
                            uGridMaster.ActiveRow.Refresh();
                            break;

                        case "sga06"://課稅別
                            WfSetSga06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtBySga06();
                            break;

                        case "sga26"://發票別

                            e.Row["sga24"] = "";
                            break;
                    }
                }
                #endregion

                #region 單身-vw_stpt410s
                if (e.Row.Table.Prefix.ToLower() == "vw_stpt410s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_stpt410s>();
                    detailList = e.Row.Table.ToList<vw_stpt410s>();
                    //babModel = BoBas.OfGetBabModel(masterModel.sga01);
                    switch (e.Column.ToLower())
                    {
                        case "sgb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            detailList = e.Row.Table.ToList<vw_stpt410s>();
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.sgb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;
                        case "sgb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sgb04"] = "";//品名
                                e.Row["sgb05"] = 0;//銷貨數量
                                e.Row["sgb06"] = "";//銷貨單位
                                e.Row["sgb07"] = "";//庫存單位
                                e.Row["sgb08"] = 0;//庫存轉換率
                                e.Row["sgb13"] = 0;//訂單轉換率
                                e.Row["sgb14"] = 0;//訂單數量
                                e.Row["sgb15"] = "";//訂單單位
                                e.Row["sgb18"] = 0;//庫存數量
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }

                            if (WfSetSgb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;

                            if (!GlobalFn.varIsNull(masterModel.sga12))
                            {
                                newDetailModel = e.Row.ToItem<vw_stpt410s>();
                                result = BoStp.OfGetPrice(masterModel.sga12, masterModel.sga03, e.Value.ToString(), newDetailModel.sgb06,
                                    masterModel.sga02, masterModel.sga10, "3", newDetailModel.sgb05,
                                    masterModel.sga08, masterModel.sga07, masterModel.sga21, out price);
                                if (result.Success == true)
                                {
                                    e.Row["sgb09"] = price;
                                }
                                else
                                {
                                    WfShowErrorMsg(result.Message);
                                    return false;
                                }
                            }
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            WfGetQtyTot();
                            WfSetStockQty(e.Row);
                            break;
                        case "sgb05"://出貨數量
                            if (GlobalFn.varIsNull(detailModel.sgb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["sgb03"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(detailModel.sgb06))
                            {
                                WfShowErrorMsg("請先輸入銷貨單位!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["sgb06"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入銷貨數量!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["sgb05"]);
                                return false;
                            }
                            detailModel.sgb05 = BoBas.OfGetUnitRoundQty(detailModel.sgb06, detailModel.sgb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detailModel.sgb05;
                            if (WfChkSgb05(e.Row, detailModel) == false)
                                return false;
                            e.Row["sgb14"] = BoBas.OfGetUnitRoundQty(detailModel.sgb06, detailModel.sgb05 * detailModel.sgb13); //轉換訂單數量(並四拾伍入)
                            e.Row["sgb18"] = BoBas.OfGetUnitRoundQty(detailModel.sgb07, detailModel.sgb05 * detailModel.sgb08); //轉換庫存數量(並四拾伍入)
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            //自動計算成本
                            sgaModel = DrMaster.ToItem<sga_tb>();
                            if (BoStp.OfChkEbusinessPlateForm(masterModel.sga03) == false)
                            {
                                DrMaster["sga23"] = 0;
                            }
                            else
                            {
                                result = BoStp.OfGetSga23(sgaModel, out sga23);
                                DrMaster["sga23"] = sga23;
                            }
                            break;
                        case "sgb06"://銷貨單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (GlobalFn.varIsNull(detailModel.sgb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["sgb03"]);
                                return false;
                            }
                            //if (WfChkSgb06(e.Row, e.Value.ToString(), babModel.bab08) == false)
                            //    return false;

                            //if (WfSetSgb06Relation(e.Row, e.Value.ToString(), babModel.bab08) == false)
                            //    return false;
                            break;

                        case "sgb09":   //單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.sgb09 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.sgb09, BekTbModel.bek03);
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            //自動計算成本
                            sgaModel = DrMaster.ToItem<sga_tb>();
                            if (BoStp.OfChkEbusinessPlateForm(masterModel.sga03) == false)
                            {
                                DrMaster["sga23"] = 0;
                            }
                            else
                            {
                                result = BoStp.OfGetSga23(sgaModel, out sga23);
                                DrMaster["sga23"] = sga23;
                            }
                            break;


                        case "sgb16"://倉庫
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoInv.OfChkIcbPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此倉庫別,請確認!");
                                return false;
                            }
                            break;
                    }
                }
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAppendUpdate 存檔後處理額外的資料表 新增、修改、刪除...
        protected bool WfAppendUpdate()
        {
            sga_tb sgaModel;
            sgb_tb sgbModel;
            Result rtnResult = null;

            try
            {
                //因為pk取號有問題,先在這個地方做同步檢查
                //檢查有頭無身
                if (DrMaster == null)
                {
                    WfShowErrorMsg("查無單頭資料");
                    return false;
                }
                if (TabDetailList[0].DtSource.Rows.Count == 0)
                {
                    WfShowErrorMsg("查無單身資料");
                    return false;
                }
                sgaModel = DrMaster.ToItem<sga_tb>();
                foreach (DataRow drSgb in TabDetailList[0].DtSource.Rows)
                {
                    sgbModel = drSgb.ToItem<sgb_tb>();
                    if (sgaModel.sga01 != sgbModel.sgb01)
                    {
                        WfShowErrorMsg("單頭與單身取得單號不同,請聯絡MIS人員!!");
                        return false;
                    }
                }

                //更新發票本
                if (!GlobalFn.varIsNull(sgaModel.sga24))
                {
                    rtnResult = new Result();
                    rtnResult = BoTax.OfUpdTbe09(sgaModel.sga26, sgaModel.sga09, sgaModel.sga24, Convert.ToDateTime(sgaModel.sga27));
                    if (rtnResult.Success == false)
                    {
                        WfShowErrorMsg(rtnResult.Message);
                        return false;
                    }
                }


                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAfterFormCheck() 存檔後處理,通常為放入Pk
        protected bool WfAfterFormCheck()
        {
            string sga01New, errMsg;
            vw_stpt410 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt410>();
                if (FormEditMode == YREditType.新增)
                {
                    sga01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.sga01, ModuleType.stp, (DateTime)masterModel.sga02, out sga01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["sga01"] = sga01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["sgasecu"] = LoginInfo.UserNo;
                        DrMaster["sgasecg"] = LoginInfo.GroupNo;
                        DrMaster["sgacreu"] = LoginInfo.UserNo;
                        DrMaster["sgacreg"] = LoginInfo.DeptNo;
                        DrMaster["sgacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["sgamodu"] = LoginInfo.UserNo;
                        DrMaster["sgamodg"] = LoginInfo.DeptNo;
                        DrMaster["sgamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["sgbcreu"] = LoginInfo.UserNo;
                            drDetail["sgbcreg"] = LoginInfo.DeptNo;
                            drDetail["sgbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["sgbmodu"] = LoginInfo.UserNo;
                            drDetail["sgbmodg"] = LoginInfo.DeptNo;
                            drDetail["sgbmodd"] = Now;
                        }
                    }
                }

                DrMaster.EndEdit();
                WfSetDetailPK();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                vw_stpt410 masterModel = null;
                vw_stpt410s detailModel = null;
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_stpt410
                if (pDr.Table.Prefix.ToLower() == "vw_stpt410")
                {
                    masterModel = DrMaster.ToItem<vw_stpt410>();
                    switch (pColName.ToLower())
                    {
                        case "sga01"://銷貨單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "stp"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "30"));
                            WfShowPickUtility("p_bab1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "sga03"://客戶編號
                            WfShowPickUtility("p_sca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sca01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "sga04"://業務人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sga05"://業務部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;


                        case "sgb04_pick"://料號挑選
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sga26"://發票別
                            WfShowPickUtility("p_tbe2", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["tbe04"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_stpt400s
                if (pDr.Table.Prefix.ToLower() == "vw_stpt410s")
                {
                    masterModel = DrMaster.ToItem<vw_stpt410>();
                    detailModel = pDr.ToItem<vw_stpt410s>();
                    switch (pColName.ToLower())
                    {
                        case "sgb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sgb06"://銷貨單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sgb11"://訂單單號
                            if (GlobalFn.isNullRet(masterModel.sga03, "") == "")
                                WfShowPickUtility("p_sfb1", messageModel);
                            else
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@sfa03", masterModel.sga03));
                                WfShowPickUtility("p_sfb2", messageModel);
                            }
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                {
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sfb01"], "");
                                    pDr["sgb12"] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sfb02"], "");
                                }
                                else
                                {
                                    pDr[pColName] = "";
                                    pDr["sgb12"] = "";

                                }
                            }
                            break;

                        case "sgb16"://倉庫
                            if (GlobalFn.isNullRet(detailModel.sgb03, "") == "")
                                WfShowPickUtility("p_icb1", messageModel);
                            else
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.sgb03));
                                WfShowPickUtility("p_icc1", messageModel);
                            }
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icc02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "print_label":
                            WfPrintLabelMoney(detailModel.sgb03);
                            break;
                    }
                }
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfFormCheck() 存檔前檢查
        protected bool WfFormCheck()
        {
            vw_stpt410 masterModel = null;
            vw_stpt410s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {
                WfCleanErrorProvider();
                masterModel = DrMaster.ToItem<vw_stpt410>();
                if (!GlobalFn.varIsNull(masterModel.sga01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.sga01, ""));
                #region 單頭資料檢查
                chkColName = "sga01";       //銷貨單號
                chkControl = ute_sga01;
                if (GlobalFn.varIsNull(masterModel.sga01))
                {
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sga02";       //出庫日期
                chkControl = udt_sga02;
                if (GlobalFn.varIsNull(masterModel.sga02))
                {
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.sga02), BaaModel);
                if (result.Success == false)
                {
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                chkColName = "sga03";       //客戶編號
                chkControl = ute_sga03;
                if (GlobalFn.varIsNull(masterModel.sga03))
                {
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sga04";       //業務人員
                chkControl = ute_sga04;
                if (GlobalFn.varIsNull(masterModel.sga04))
                {
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sga17";       //客戶單號
                chkControl = ute_sga17;
                //檢查是否為網路平台客戶
                if (BoStp.OfChkEbusinessPlateForm(masterModel.sga03) == true)
                {
                    if (GlobalFn.varIsNull(masterModel.sga17))
                    {
                        chkControl.Focus();
                        msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        errorProvider.SetError(chkControl, msg);
                        WfShowErrorMsg(msg);
                        return false;
                    }

                    if (masterModel.sga23 == 0)
                    {
                        msg = "電商平台出貨成本不可為0";
                        errorProvider.SetError(chkControl, msg);
                        WfShowErrorMsg(msg);
                        return false;
                    }

                    if (GlobalFn.varIsNull(masterModel.sga17))
                    {
                        if (WfShowConfirmMsg("客戶單號為空白，是否繼續？") == DialogResult.No)
                            return false;
                    }
                    else  //比對是否為重覆輸入的單號                    
                    {
                        if (BoStp.OfChkSga03Sga17(masterModel.sga03, masterModel.sga17, "0") == true)
                        {

                            if (WfShowConfirmMsg("客戶單號為已重覆，是否繼續？") == DialogResult.No)
                                return false;
                        }
                    }
                }


                chkColName = "sga23";       //出貨成本
                chkControl = ute_sga23;
                //檢查是否為網路平台客戶
                if (BoStp.OfChkEbusinessPlateForm(masterModel.sga03) == true)
                {
                    if (masterModel.sga23 == 0)
                    {
                        msg = "電商平台出貨成本不可為0";
                        errorProvider.SetError(chkControl, msg);
                        WfShowErrorMsg(msg);
                        return false;
                    }
                }

                chkColName = "sga24";       //發票號碼
                chkControl = ute_sga24;
                if (!GlobalFn.varIsNull(masterModel.sga24))
                {
                    //檢查發票重覆
                    if (BoTax.OfChkInvDupl("2",masterModel.sga01,masterModel.sga24,masterModel.sga27,masterModel.sga09)==true)
                    {
                        DialogResult resultChk=WfShowConfirmMsg("發票號碼重覆，請確認是否繼續？");
                        if (resultChk == DialogResult.No)
                            return false;
                    }
                }

                //chkColName = "sga12";
                //chkControl = ute_sga12;
                //if (GlobalFn.varIsNull(masterModel.sga12) && babModel.bab08 != "Y")//無來源單據取價條件要輸入
                //{
                //    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                //    chkControl.Focus();
                //    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                //    msg += "不可為空白";
                //    errorProvider.SetError(chkControl, msg);
                //    WfShowErrorMsg(msg);
                //    return false;
                //}

                //chkColName = "sga21";
                //chkControl = ute_sga21;
                //if (GlobalFn.varIsNull(masterModel.sga21))
                //{
                //    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                //    chkControl.Focus();
                //    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                //    msg += "不可為空白";
                //    errorProvider.SetError(chkControl, msg);
                //    WfShowErrorMsg(msg);
                //    return false;
                //}
                #endregion

                #region 單身資料檢查
                iChkDetailTab = 0;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                if (TabDetailList[iChkDetailTab].DtSource.Rows.Count == 0)
                {

                    msg = "未新增任何明細資料到";
                    WfShowErrorMsg(msg);
                    return false;
                }

                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)
                        continue;

                    detailModel = drTemp.ToItem<vw_stpt410s>();
                    chkColName = "sgb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.sgb02))
                    {
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sgb11";   //訂單單號
                    if (GlobalFn.varIsNull(detailModel.sgb11) && babModel.bab08 == "Y")  //有來源單據訂單單號要輸入
                    {
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sgb12";   //訂單項次
                    if (GlobalFn.varIsNull(detailModel.sgb12) && babModel.bab08 == "Y")  //有來源單據訂單單號要輸入
                    {
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sgb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.sgb03))
                    {
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sgb05";   //出庫數量
                    #region pfb05 銷貨數量
                    if (GlobalFn.varIsNull(detailModel.sgb05) || detailModel.sgb05 <= 0)
                    {
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    if (WfChkSgb05(drTemp, detailModel) == false)
                    {
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    chkColName = "sgb06";   //出庫單位
                    if (GlobalFn.varIsNull(detailModel.sgb06))
                    {
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sgb16";   //倉庫別
                    if (GlobalFn.varIsNull(detailModel.sgb16))
                    {
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                }
                #endregion

                //WfGenSga23();//網拍平台自動產生手續費
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //*****************************表單自訂Fuction****************************************
        #region WFInsertDetail 新增明細資料
        private DataRow WFInsertDetail()
        {
            //if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
            //{
            WfFireControlValidated(this.ActiveControl);
            if (IsItemchkValid == false)
                return null;

            if (WfPreInsertDetailCheck(IntCurTabDetail) == false)
                return null;
            return this.WfToolbarDetailInsert();
            //}
        }
        #endregion

        #region WfSetSga03Relation 設定客戶相關聯
        private void WfSetSga03Relation(string pSga03)
        {
            sca_tb scaModel;
            bab_tb babModel = null;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sga01"], ""));
                scaModel = BoStp.OfGetScaModel(pSga03);
                if (scaModel == null)
                    return;

                DrMaster["sga03_c"] = scaModel.sca03;
                DrMaster["sga06"] = scaModel.sca22;    //課稅別
                WfSetSga06Relation(scaModel.sca22);
                DrMaster["sga09"] = scaModel.sca23;    //發票聯數
                DrMaster["sga11"] = scaModel.sca21;    //付款條件
                DrMaster["sga11_c"] = BoBas.OfGetBef03("1", scaModel.sca21);
                //DRMASTER["sga14"] = l_sca.pca35;    //送貨地址
                //DRMASTER["sga15"] = l_sca.pca36;    //帳單地址

                DrMaster["sga12"] = scaModel.sca24;    //取價條件
                DrMaster["sga12_c"] = BoStp.OfGetSbb02(scaModel.sca24);
                DrMaster["sga26"] = scaModel.sca29;    //發票別
                //依發票別重新調整radiogroup
                scaModel.sca29 = GlobalFn.isNullRet(scaModel.sca29, "");
                switch (scaModel.sca29)
                {
                    case "Z01": //正中
                        rdb_invoice3.Checked = true;
                        break;
                    case "I01":  //艾達
                        rdb_invoice2.Checked = true;
                        break;
                    default:
                        rdb_invoice1.Checked = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetSga06Relation 設定稅別關聯
        private void WfSetSga06Relation(string pSga06)
        {
            try
            {
                if (pSga06 == "1")
                {
                    DrMaster["sga07"] = BaaModel.baa05;
                    DrMaster["sga08"] = "Y";
                }
                else if (pSga06 == "2")
                {
                    DrMaster["sga07"] = BaaModel.baa05;
                    DrMaster["sga08"] = "N";
                }
                else
                {
                    DrMaster["sga07"] = 0;
                    DrMaster["sga08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetDetailAmt 處理小計
        private bool WfSetDetailAmt(DataRow drSeb)
        {
            sga_tb sgaModel;
            sgb_tb sgbModel;
            decimal sgb10t = 0, sgb10 = 0;
            try
            {
                sgaModel = DrMaster.ToItem<sga_tb>();
                sgbModel = drSeb.ToItem<sgb_tb>();

                if (sgaModel.sga08 == "Y")//稅內含
                {
                    sgb10t = sgbModel.sgb09 * sgbModel.sgb05;
                    sgb10t = GlobalFn.Round(sgb10t, BekTbModel.bek04);
                    sgb10 = sgb10t / (1 + (sgaModel.sga07 / 100));
                    sgb10 = GlobalFn.Round(sgb10, BekTbModel.bek04);
                }
                else//稅外加
                {
                    sgb10 = sgbModel.sgb09 * sgbModel.sgb05;
                    sgb10 = GlobalFn.Round(sgb10, BekTbModel.bek04);
                    sgb10t = sgb10 * (1 + (sgaModel.sga07 / 100));
                    sgb10t = GlobalFn.Round(sgb10t, BekTbModel.bek04);
                }
                drSeb["sgb10"] = sgb10;
                drSeb["sgb10t"] = sgb10t;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetTotalAmt 處理總計
        private bool WfSetTotalAmt()
        {
            sga_tb sgaModel;
            decimal sga13 = 0, sga13t = 0, sga13g;
            try
            {
                sgaModel = DrMaster.ToItem<sga_tb>();
                if (sgaModel.sga08 == "Y")//稅內含,先處理含稅金額
                {
                    foreach (sgb_tb l_sgb in TabDetailList[0].DtSource.ToList<sgb_tb>())
                    {
                        sga13t += l_sgb.sgb10t;
                    }
                    sga13t = GlobalFn.Round(sga13t, BekTbModel.bek04);
                    sga13 = sga13t / (1 + sgaModel.sga07 / 100);
                    sga13 = GlobalFn.Round(sga13, BekTbModel.bek04);
                    sga13g = sga13t - sga13;
                }
                else//稅外加
                {
                    foreach (sgb_tb sgbModel in TabDetailList[0].DtSource.ToList<sgb_tb>())
                    {
                        sga13 += sgbModel.sgb10;
                    }
                    sga13 = GlobalFn.Round(sga13, BekTbModel.bek04);
                    sga13g = sga13 * (sgaModel.sga07 / 100);
                    sga13g = GlobalFn.Round(sga13g, BekTbModel.bek04);
                    sga13t = sga13 + sga13g;
                }

                DrMaster["sga13"] = sga13;
                DrMaster["sga13t"] = sga13t;
                DrMaster["sga13g"] = sga13g;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetSgb03Relation 設定料號關聯
        private bool WfSetSgb03Relation(string pSgb03, DataRow pDr)
        {
            ica_tb icaModel;
            decimal sgb08;   //轉換率
            try
            {
                icaModel = BoInv.OfGetIcaModel(pSgb03);
                sgb08 = 0;
                if (icaModel == null)
                {
                    pDr["sgb04"] = "";//品名
                    pDr["sgb05"] = 0;//出貨數量
                    pDr["sgb06"] = "";//出貨單位
                    pDr["sgb07"] = "";//庫存單位
                    pDr["sgb08"] = 0;//庫存轉換率
                    pDr["sgb18"] = 0;//庫存數量
                }
                else
                {
                    if (BoInv.OfGetUnitCovert(pSgb03, icaModel.ica09, icaModel.ica07, out sgb08) == false)
                    {
                        WfShowErrorMsg("未設定料號轉換,請先設定!");
                        return false;
                    }
                    pDr["sgb04"] = icaModel.ica02;//品名
                    pDr["sgb05"] = 0;//出貨數量
                    pDr["sgb06"] = icaModel.ica09;//出貨單位帶銷售單位
                    pDr["sgb07"] = icaModel.ica07;//庫存單位
                    pDr["sgb08"] = sgb08;
                    pDr["sgb18"] = 0;//庫存數量
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkSgb05 數量檢查
        private bool WfChkSgb05(DataRow pdr, vw_stpt410s pListDetail)
        {
            List<vw_stpt410s> detailList = null;
            sfb_tb sfbModel = null;
            bab_tb babModel = null;
            string errMsg;
            decimal docThisQty = 0;         //本項次的轉換訂單數量
            decimal docOtherQtyTotal = 0;   //本單據其他項次的訂單數量加總
            decimal otherQtyTotal = 0;      //其他單據的數量加總
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                detailList = pdr.Table.ToList<vw_stpt410s>();
                if (GlobalFn.varIsNull(pListDetail.sgb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.sgb03))
                {
                    errMsg = "請先輸入料號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.sgb06))
                {
                    errMsg = "請先輸入銷售單位!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (pListDetail.sgb05 <= 0)
                {
                    errMsg = "銷貨數量應大於0!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkSgb06 檢查銷貨單位
        //檢查前要先確認料號是否已輸入
        private bool WfChkSgb06(DataRow pDr, string pSgb06, string pBab08)
        {
            vw_stpt410s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_stpt410s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pSgb06, "")) == false)
                {
                    WfShowErrorMsg("無此銷售單位!請確認");
                    return false;
                }
                //檢查是否有銷售對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.sgb03, pSgb06, detailModel.sgb07, out dConvert) == false)
                {
                    WfShowErrorMsg("未設定銷貨單位對庫存單位的轉換率,請先設定!");
                    return false;
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有銷售對訂單的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.sgb03, pSgb06, detailModel.sgb06, out dConvert) == false)
                    {
                        WfShowErrorMsg("未設定銷售單位對訂單單位的轉換率,請先設定!");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfResetAmtBySga06 依稅別更新單身及單頭的金額
        private void WfResetAmtBySga06()
        {
            try
            {
                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    WfSetDetailAmt(dr);
                    WfSetTotalAmt();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetSgb06Relation 設定銷貨單位關聯
        //檢查前要先確認料號是否已輸入
        private bool WfSetSgb06Relation(DataRow pDr, string pSgb06, string pBab08)
        {
            vw_stpt410s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_stpt410s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pSgb06, "")) == false)
                {
                    WfShowErrorMsg("無此銷售單位!請確認");
                    return false;
                }
                //取得是否有銷售對庫存的轉換率
                dConvert = 0;
                if (BoInv.OfGetUnitCovert(detailModel.sgb03, pSgb06, detailModel.sgb07, out dConvert) == true)
                {
                    pDr["sgb08"] = dConvert;
                    pDr["sgb18"] = BoBas.OfGetUnitRoundQty(detailModel.sgb07, detailModel.sgb05 * dConvert); //轉換庫存數量(並四拾伍入)
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有銷售對訂單的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.sgb03, pSgb06, detailModel.sgb15, out dConvert) == true)
                    {
                        pDr["sgb13"] = dConvert;
                        pDr["sgb14"] = BoBas.OfGetUnitRoundQty(detailModel.sgb15, detailModel.sgb05 * dConvert); //轉換庫存數量(並四拾伍入)
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region btnDelete_Click 刪除明細資料
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DataRow drDetail;
            sga_tb sgaModel = null;
            decimal sga23 = 0;
            Result result;
            if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
            {
                WfFireControlValidated(this.ActiveControl);
                if (IsItemchkValid == false)
                    return;

                drDetail = WfGetUgridDatarow(TabDetailList[this.IntCurTabDetail].UGrid.ActiveRow);

                if (drDetail == null)
                {
                    WfShowBottomStatusMsg("查無要刪除的資料!!");
                    return;
                }

                if (TabDetailList[IntCurTabDetail].IsReadOnly)
                {
                    WfShowBottomStatusMsg("本頁面只可查詢無法刪除!");
                    return;
                }

                if (WfPreDeleteDetailCheck(IntCurTabDetail, drDetail) == false)
                    return;

                if (this.WfToolbarDetailDelete())
                {
                    this.IsChanged = true;
                    WfShowBottomStatusMsg("刪除明細成功!");
                }
                WfSetTotalAmt();

                sgaModel = DrMaster.ToItem<sga_tb>();
                if (BoStp.OfChkEbusinessPlateForm(sgaModel.sga03) == false)
                {
                    DrMaster["sga23"] = 0;
                }
                else
                {
                    result = BoStp.OfGetSga23(sgaModel, out sga23);
                    DrMaster["sga23"] = sga23;
                }
            }
        }
        #endregion

        #region WfPreDeleteDetailCheck (): 刪除明細前檢查
        protected bool WfPreDeleteDetailCheck(int pCurTabDetail, DataRow pDrDetail)
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

        #region WfToolbarDetailDelete : 刪除明細資料
        #region WfToolbarDetailDelete()
        protected bool WfToolbarDetailDelete()
        {
            return this.WfToolbarDetailDelete(-1, null);
        }
        #endregion

        #region WfToolbarDetailDelete(int pTabIndex, DataRow pDr): 刪除明細資料
        /// <summary>
        /// wf_toolbar_detail_delete : 刪除明細資料
        /// </summary>
        protected virtual bool WfToolbarDetailDelete(int pTabIndex, DataRow pDr)
        {
            return WfToolbarDetailDelete(pTabIndex, pDr, true);

        }
        #endregion

        #region WfToolbarDetailDelete(int pi_tabindex, DataRow pdr): 刪除明細資料
        /// <summary>
        /// 刪除明細資料
        /// </summary>
        /// <param name="pTabIndex"></param>
        /// <param name="pDr"></param>
        /// <param name="pDeleteConfirm">是否要顯示提示訊息,由其他功能引用時可不出現訊息</param>
        /// <returns></returns>
        protected virtual bool WfToolbarDetailDelete(int pTabIndex, DataRow pDr, bool pDeleteConfirm)
        {
            UltraGrid uGrid = null;
            int activeIndex = 0;
            bool isLockMaster = false;//判斷是否需要LOCK
            DataTable dtOld = null; //複製要刪除的資料

            if (pTabIndex < 0)
            { pTabIndex = this.IntCurTabDetail; }

            if (pTabIndex < 0) return false;
            if (TabDetailList[pTabIndex].DtSource == null) return false;
            try
            {
                DataRow dr = null;
                uGrid = TabDetailList[pTabIndex].UGrid;
                if (pDr == null)
                { dr = WfGetUgridDatarow(TabDetailList[pTabIndex].UGrid.ActiveRow); }
                else
                { dr = pDr; }

                if (DrMaster == null || dr == null)
                {
                    WfShowErrorMsg("查無要刪除的資料!!");
                    return false;
                }

                // 先獲取 Grid 組件當前列的 ID
                activeIndex = TabDetailList[pTabIndex].UGrid.ActiveRow.Index;

                if (!(dr.RowState == DataRowState.Modified || dr.RowState == DataRowState.Unchanged))
                {

                    //複製要刪除的資料
                    dtOld = dr.Table.Clone();
                    dtOld.ImportRow(dr);
                    dr.Delete();
                    //dtOld.RejectChanges();  //只取未變更前的值,做為後續WfAfterDetailDelete 使用

                    //TabDetailList[pTabIndex].DtSource.Rows.Remove(dr);
                    //WfActiveGridNextRow(TabDetailList[pTabIndex].UGrid, activeIndex);
                    //WfSetFirstVisibelCellFocus(uGrid);
                    //return true;
                }
                else if ((dr.RowState == DataRowState.Modified || dr.RowState == DataRowState.Unchanged))
                {
                    if (pDeleteConfirm == true)
                    {
                        var result = WfShowConfirmMsg("是否確定要刪除資料?");
                        if (result != DialogResult.Yes)
                            return false;

                    }

                    //複製要刪除的資料
                    dtOld = dr.Table.Clone();
                    dtOld.ImportRow(dr);
                    dtOld.RejectChanges();  //只取未變更前的值,做為後續WfAfterDetailDelete 使用
                    dr.Delete();
                    this.TabDetailList[pTabIndex].BoBasic.OfUpdate(BoMaster.TRAN, new DataRow[] { dr });
                }

                //todo:要考量主表已被Lock的情形
                if (BoMaster.TRAN == null)
                {
                    if (WfBeginTran() == false)
                        return false;
                }
                WfSetBllTransaction();

                this.WfAfterDetailDelete(pTabIndex, dtOld.Rows[0]);

                #region 重新選定一筆 Row 並且移至第一個可編輯的資料列
                WfActiveGridNextRow(uGrid, activeIndex);
                #endregion 重新選定一筆 Row

                WfSetFirstVisibelCellFocus(uGrid);
                WfCommit();
                //TabDetailList[pTabIndex].DtSource.Rows.Remove(dr);
                isLockMaster = true;
                return true;
            }
            catch (Exception ex)
            {
                WfRollback();
                throw ex;
            }
            finally
            {
                //重新Lock mater
                if (TabMaster.CanUseRowLock && isLockMaster == true)
                {
                    WfLockMasterRow();
                    WfSetBllTransaction();
                }
            }
        }
        #endregion

        #endregion

        #region WfLockMasterRow Lock ActiveRow
        protected bool WfLockMasterRow()
        {
            StringBuilder sbSql;
            try
            {
                if (TabMaster.PKParms == null || TabMaster.PKParms.Count == 0)
                {
                    WfShowErrorMsg("未設定主表的PK值! (WfRetrieveMaster)");
                    return false;
                }

                WfBeginTran(IsolationLevel.ReadCommitted);
                sbSql = new StringBuilder();
                sbSql.AppendLine("SET LOCK_TIMEOUT 0");
                sbSql.AppendLine(string.Format("SELECT COUNT(1) FROM {0}", TabMaster.TargetTable));
                sbSql.AppendLine("(UPDLOCK) ");
                sbSql.AppendLine("WHERE 1=1");
                foreach (SqlParameter sp in TabMaster.PKParms)
                {
                    sbSql.AppendLine(string.Format("AND {0}=@{0}", sp.ParameterName));
                    sp.Value = DrMaster[sp.ParameterName];
                }

                BoMaster.OfGetFieldValue(sbSql.ToString(), TabMaster.PKParms.ToArray());
                //BOMASTER.OfGetDataRow(sbSql.ToString(), TabMaster.PKParms.ToArray());
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 1222)//lock error
                {
                    WfShowErrorMsg("此筆資料已被其他人鎖定(LOCK),請稍後再做修改!");
                    WfRollback();
                    return false;
                }
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                WfRollback();
                throw ex;
            }
        }
        #endregion

        #region WfAfterDetailDelete() :刪除明細後調用 pdr為clone出來的舊資料
        protected virtual bool WfAfterDetailDelete(int pCurTabDetail, DataRow pDr)
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

        #region WfSetDetailPK() 依單頭與單身的對應關係賦予 PK
        protected virtual void WfSetDetailPK()
        {
            try
            {
                for (int i = 0; i < IntTabDetailCount; i++)
                {
                    if (TabDetailList[i].IsReadOnly == true) continue;
                    foreach (DataRow drDetail in TabDetailList[i].DtSource.Rows)
                    {
                        foreach (SqlParameter sp in TabDetailList[i].RelationParams)
                        {
                            drDetail[sp.ParameterName] = DrMaster[sp.SourceColumn];
                            drDetail.EndEdit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 取得料件圖檔
        private void WfLoadIcp03(string ica01)
        {
            string selectSql;
            object icp03;
            List<SqlParameter> sqlParmsList;
            try
            {
                selectSql = @"
                            SELECT TOP 1 icp03
                            FROM icp_tb
                            WHERE icp01=@icp01
                            ORDER BY ISNULL(icp06,'N') DESC,
                                     icp05
                           ";
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@icp01", ica01));
                icp03 = BoInv.OfGetFieldValue(selectSql, sqlParmsList.ToArray());
                if (icp03 == null)
                {
                    pbx_icp03.Image = null;
                    pbxUsed.Visible = false;
                    return;
                }

                pbx_icp03.Image = Image.FromStream(new MemoryStream((byte[])icp03));
                WfSetUsedPicture(ica01);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfGetQtyTot 取得出庫總數量
        private void WfGetQtyTot()
        {
            DataTable dtSgb;
            List<sgb_tb> sgbList;
            try
            {
                if (DrMaster == null)
                    return;

                dtSgb = TabDetailList[0].DtSource;
                if (dtSgb.Rows.Count == 0)
                {
                    DrMaster["qty_tot"] = 0;
                    return;
                }
                sgbList = dtSgb.ToList<sgb_tb>();
                DrMaster["qty_tot"] = sgbList.Sum(o => o.sgb05);

            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

        #region WfSetStockQty 取得庫存總量
        private void WfSetStockQty(DataRow drSgb)
        {
            string sgb03;
            try
            {
                sgb03 = GlobalFn.isNullRet(drSgb["sgb03"], "");
                drSgb["stock_qty"] = BoInv.OfGetIcc05TotByIcc01(sgb03);
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }

        }
        #endregion

        private void btnQtyAdd_Click(object sender, EventArgs e)
        {
            DataRow drDetail;
            vw_stpt410s detailModel;
            sga_tb sgaModel;
            Result result;
            decimal sga23 = 0;
            if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
            {
                WfFireControlValidated(this.ActiveControl);
                if (IsItemchkValid == false)
                    return;

                drDetail = WfGetUgridDatarow(TabDetailList[this.IntCurTabDetail].UGrid.ActiveRow);

                if (drDetail == null)
                {
                    WfShowBottomStatusMsg("查無要異動的資料!!");
                    return;
                }

                detailModel = drDetail.ToItem<vw_stpt410s>();

                detailModel.sgb05 += 1;
                detailModel.sgb05 = BoBas.OfGetUnitRoundQty(detailModel.sgb06, detailModel.sgb05);    //先轉換數量(四捨伍入)
                drDetail["sgb05"] = detailModel.sgb05;
                if (WfChkSgb05(drDetail, detailModel) == false)
                    return;
                drDetail["sgb14"] = BoBas.OfGetUnitRoundQty(detailModel.sgb06, detailModel.sgb05 * detailModel.sgb13); //轉換訂單數量(並四拾伍入)
                drDetail["sgb18"] = BoBas.OfGetUnitRoundQty(detailModel.sgb07, detailModel.sgb05 * detailModel.sgb08); //轉換庫存數量(並四拾伍入)
                WfSetDetailAmt(drDetail);
                WfSetTotalAmt();
                WfGetQtyTot();
                //自動計算成本
                sgaModel = DrMaster.ToItem<sga_tb>();
                if (BoStp.OfChkEbusinessPlateForm(sgaModel.sga03) == false)
                {
                    DrMaster["sga23"] = 0;
                }
                else
                {
                    result = BoStp.OfGetSga23(sgaModel, out sga23);
                    DrMaster["sga23"] = sga23;
                }
            }
        }

        private void btnQtyDec_Click(object sender, EventArgs e)
        {

            DataRow drDetail;
            vw_stpt410s detailModel;
            sga_tb sgaModel;
            decimal sga23 = 0;
            Result result;
            if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
            {
                WfFireControlValidated(this.ActiveControl);
                if (IsItemchkValid == false)
                    return;

                drDetail = WfGetUgridDatarow(TabDetailList[this.IntCurTabDetail].UGrid.ActiveRow);

                if (drDetail == null)
                {
                    WfShowBottomStatusMsg("查無要異動的資料!!");
                    return;
                }

                detailModel = drDetail.ToItem<vw_stpt410s>();
                if (detailModel.sgb05 <= 1)
                {
                    return;
                }
                detailModel.sgb05 = detailModel.sgb05 - 1;



                detailModel = drDetail.ToItem<vw_stpt410s>();
                detailModel.sgb05 -= 1;
                detailModel.sgb05 = BoBas.OfGetUnitRoundQty(detailModel.sgb06, detailModel.sgb05);    //先轉換數量(四捨伍入)
                drDetail["sgb05"] = detailModel.sgb05;
                if (WfChkSgb05(drDetail, detailModel) == false)
                    return;
                drDetail["sgb14"] = BoBas.OfGetUnitRoundQty(detailModel.sgb06, detailModel.sgb05 * detailModel.sgb13); //轉換訂單數量(並四拾伍入)
                drDetail["sgb18"] = BoBas.OfGetUnitRoundQty(detailModel.sgb07, detailModel.sgb05 * detailModel.sgb08); //轉換庫存數量(並四拾伍入)
                WfSetDetailAmt(drDetail);
                WfSetTotalAmt();
                WfGetQtyTot();
                //自動計算成本
                sgaModel = DrMaster.ToItem<sga_tb>();
                if (BoStp.OfChkEbusinessPlateForm(sgaModel.sga03) == false)
                {
                    DrMaster["sga23"] = 0;
                }
                else
                {
                    result = BoStp.OfGetSga23(sgaModel, out sga23);
                    DrMaster["sga23"] = sga23;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                try
                {
                    if (FormEditMode != YREditType.NA
                        )
                    {
                        IsInCRUDIni = true;
                        if (this.TabMaster.ViewTable != "")
                        {
                            WfFireControlValidated(this.ActiveControl);
                            //各驗證控製項會將 isItemchkValid設為true
                            if (IsItemchkValid == false)
                                return;

                            if (WfToolbarSave() == false)
                                return;

                            if (WfDisplayMode() == false)
                                return;
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }

                //WfFireControlValidated(this.ActiveControl);
                ////各驗證控製項會將 isItemchkValid設為true
                //if (IsItemchkValid == false)
                //    return;

                //if (WfToolbarSave() == false)
                //    return;

                //if (WfDisplayMode() == false)
                //    return;
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
            finally
            {
                //IsInSaveCancle = false;
                //IsInCRUDIni = false;
                this.Cursor = Cursors.Default;
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (FormEditMode == YREditType.NA
                    && TabMaster.CanAddMode == true
                    && (TabMaster.IsCheckSecurity == false || TabMaster.AddTbModel.add03.ToUpper() == "Y")
                    )
                {
                    IsInCRUDIni = true;
                    if (this.TabMaster.ViewTable != "")
                    {
                        this.TabMaster.DtSource = this.TabMaster.BoBasic.OfSelect(" WHERE 1=2 ");
                        TabMaster.DtSource.Rows.Add(TabMaster.DtSource.NewRow());

                        this.WfSetMasterDatasource(this.TabMaster.DtSource);
                        this.BindingMaster.MoveFirst();
                        DrMaster = TabMaster.DtSource.Rows[0];
                        this.FormEditMode = YREditType.新增;
                        //WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
                        WfSetControlPropertyByMode(this);
                        this.IsChanged = false;

                        WfSetMasterRowDefault(DrMaster);
                        WfRetrieveDetail();
                        if (WfDisplayMode() == false)
                            return;
                        if (this.ActiveControl != null)
                            WfGetOldValue(this.ActiveControl);
                        //rdb_01.Select();
                        udt_sgb04_pick.Focus();
                    }
                    pbx_icp03.Image = null;


                    //if (WfPreInInsertModeCheck() == false)
                    //    return;
                    //if (WfToolbarInsert() == false)
                    //    return;
                    //WfRetrieveDetail();
                    //if (WfDisplayMode() == false)
                    //    return;
                    //WfAfterfDisplayMode();
                    //if (this.ActiveControl != null)
                    //    WfGetOldValue(this.ActiveControl);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                IsInSaveCancle = false;
                IsInCRUDIni = false;
                this.Cursor = Cursors.Default;
            }
        }

        private void btnStp400_Click(object sender, EventArgs e)
        {
            vw_stpt410 masterModel;
            StringBuilder sbSql;
            try
            {
                WfFireControlValidated(this.ActiveControl);
                //各驗證控製項會將 isItemchkValid設為true
                if (IsItemchkValid == false)
                    return;

                if (FormEditMode != YREditType.NA)
                    return;
                if (DrMaster == null)
                    return;
                masterModel = DrMaster.ToItem<vw_stpt410>();
                sbSql = new StringBuilder();
                sbSql.AppendLine(string.Format(" AND sga01='{0}'", masterModel.sga01));
                WfShowForm("stpt400", false, new object[] { "stpt400", this.LoginInfo, sbSql.ToString() });
            }
            catch (Exception ex)
            {

                WfShowErrorMsg(ex.Message);
            }

        }

        private void btnInvi100_Click(object sender, EventArgs e)
        {

            vw_stpt410s detailModel;
            StringBuilder sbSql;
            DataRow drDetail;
            try
            {
                //if (FormEditMode != YREditType.NA)
                //    return;
                if (DrMaster == null)
                    return;
                if (uGridDetail.ActiveRow == null)
                    return;
                drDetail = WfGetUgridDatarow(uGridDetail.ActiveRow);
                detailModel = drDetail.ToItem<vw_stpt410s>();
                if (GlobalFn.varIsNull(detailModel.sgb03))
                    return;

                sbSql = new StringBuilder();
                sbSql.AppendLine(string.Format(" AND ica01='{0}'", detailModel.sgb03));
                WfShowForm("invi100", false, new object[] { "invi100", this.LoginInfo, sbSql.ToString() });
            }
            catch (Exception ex)
            {

                WfShowErrorMsg(ex.Message);
            }
        }


        private void btnUpdIca11_Click(object sender, EventArgs e)
        {
            vw_stpt410 masterModel;
            StringBuilder sbSql;
            vw_stpt410s detailModel;
            DataRow drDetail;
            InvBLL boIcaUpd = null;
            DataTable dtIca = null;
            DataRow drIca = null;
            string selectSql = "";
            List<SqlParameter> sqlParms;

            try
            {
                WfFireControlValidated(this.ActiveControl);
                //各驗證控製項會將 isItemchkValid設為true
                if (IsItemchkValid == false)
                    return;

                if (DrMaster == null)
                    return;
                masterModel = DrMaster.ToItem<vw_stpt410>();
                if (uGridDetail.ActiveRow == null)
                    return;
                drDetail = WfGetUgridDatarow(uGridDetail.ActiveRow);
                detailModel = drDetail.ToItem<vw_stpt410s>();
                if (GlobalFn.varIsNull(detailModel.sgb03))
                    return;

                boIcaUpd = new InvBLL(BoMaster.OfGetConntion());
                boIcaUpd.OfCreateDao("ica_tb", "*", "");
                selectSql = "SELECT * FROM ica_tb WHERE ica01=@ica01 ";
                sqlParms =new List<SqlParameter>();
                sqlParms.Add(new SqlParameter("@ica01", detailModel.sgb03));
                dtIca = boIcaUpd.OfGetDataTable(selectSql,sqlParms.ToArray());
                if (dtIca==null ||dtIca.Rows.Count==0)
                {
                    WfShowErrorMsg("查無此料號");
                    return;
                }

                if (dtIca.Rows.Count != 1)
                {
                    WfShowErrorMsg("料號並非只有一筆");
                    return;
                }
                drIca = dtIca.Rows[0];
                drIca["ica11"] = detailModel.sgb09;

                if (boIcaUpd.OfUpdate(dtIca) != 1)
                {
                    WfShowErrorMsg("異動料號訂價失敗!");
                    return;
                }
                WfShowBottomStatusMsg("異動成功!");
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        private void uGridDetail_AfterRowActivate(object sender, EventArgs e)
        {
            DataRow dr;
            string sgb03;
            try
            {
                dr = WfGetUgridDatarow(uGridDetail.ActiveRow);
                if (dr != null)
                {
                    sgb03 = GlobalFn.isNullRet(dr["sgb03"], "");
                    if (sgb03 != "")
                    {
                        WfLoadIcp03(sgb03);
                    }
                }
            }
            catch (Exception ex)
            {

                WfShowErrorMsg(ex.Message);
            }
        }

        #region rdb_cust_CheckedChanged 選取客戶編號
        private void rdb_cust_CheckedChanged(object sender, EventArgs e)
        {
            string senderName;
            string sca03 = "";
            sga_tb sgaModel;
            Result result;
            decimal sga23;
            try
            {
                senderName = WfGetControlName(sender);
                if (DrMaster != null && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                {
                    switch (senderName.ToLower())
                    {
                        case "rdb_01":
                            sca03 = "C000001";
                            break;
                        case "rdb_02":
                            sca03 = "C000002";
                            break;
                        case "rdb_03":
                            sca03 = "C000003";
                            break;
                        case "rdb_04":
                            sca03 = "C000004";
                            break;
                        case "rdb_05":
                            sca03 = "C000005";
                            break;
                        case "rdb_06":  //蝦皮2
                            sca03 = "C000006";
                            break;
                        case "rdb_07":  //露天艾達
                            sca03 = "C000007";
                            break;
                    }

                    sgaModel = DrMaster.ToItem<sga_tb>();
                    DrMaster["sga03"] = sca03;
                    DrMaster["sga03_c"] = BoStp.OfGetSca03(sca03);
                    sgaModel.sga03 = sca03;
                    WfSetSga03Relation(sca03);
                    //自動計算成本
                    if (BoStp.OfChkEbusinessPlateForm(sca03) == false)
                    {
                        DrMaster["sga23"] = 0;
                    }
                    else
                    {
                        result = BoStp.OfGetSga23(sgaModel, out sga23);
                        DrMaster["sga23"] = sga23;
                    }
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

        #region rdb_invoice_CheckedChanged 選取發票別 #依發票機選取
        private void rdb_invoice_CheckedChanged(object sender, EventArgs e)
        {

            string senderName;
            string sga26 = "";
            sga_tb sgaModel;
            Result result;
            try
            {
                senderName = WfGetControlName(sender);
                if (DrMaster != null && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                {
                    switch (senderName.ToLower())
                    {
                        case "rdb_invoice1":
                            sga26 = "";
                            break;
                        case "rdb_invoice2":
                            sga26 = "I01";
                            break;
                        case "rdb_invoice3":
                            sga26 = "Z01";
                            break;
                    }

                    DrMaster["sga26"] = sga26;
                    DrMaster["sga24"] = "";
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

        #region WfSetDocPicture 設定單據顯示圖片
        protected void WfSetUsedPicture(string ica01)
        {
            ImageList imgList = null;
            try
            {
                pbxUsed.Visible = false;
                imgList = GlobalPictuer.LoadUsedImage();
                if (imgList == null)
                    return;

                if (imgList == null)
                    return;
                pbxUsed.Image = imgList.Images["old_goods"];
                if (ica01.Length == 11 && ica01.Substring(10, 1).ToUpper() == "B")
                {
                    pbxUsed.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPrintLabel 列印標籤
        private void WfPrintLabel(string ica01)
        {
            ica_tb icaModel;
            try
            {
                icaModel = BoInv.OfGetIcaModel(ica01);
                if (icaModel == null)
                    return;

                const string TEMPLATE_DIRECTORY = @"Label\";	// Template file path
                const string TEMPLATE_SIMPLE = "barcode.LBX";	// Template file name

                string templatePath = TEMPLATE_DIRECTORY;
                templatePath = TEMPLATE_DIRECTORY + TEMPLATE_SIMPLE;
                bpac.DocumentClass doc = new DocumentClass();

                if (doc.Open(templatePath) != false)
                {
                    doc.GetObject("ica01").Text = icaModel.ica01;
                    doc.GetObject("ica02").Text = icaModel.ica02;

                    doc.StartPrint("", PrintOptionConstants.bpoDefault);
                    doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                    doc.EndPrint();
                    doc.Close();
                }
                else
                {
                    MessageBox.Show("Open() Error: " + doc.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private void WfPrintLabelMoney(string ica01)
        {
            ica_tb icaModel;
            try
            {
                icaModel = BoInv.OfGetIcaModel(ica01);
                if (icaModel == null)
                    return;

                const string TEMPLATE_DIRECTORY = @"Label\";	// Template file path
                string TEMPLATE_SIMPLE = "barcode_money.LBX";	// Template file name

                string templatePath = TEMPLATE_DIRECTORY;
                templatePath = TEMPLATE_DIRECTORY + TEMPLATE_SIMPLE;
                bpac.DocumentClass doc = new DocumentClass();




                if (icaModel.ica01.Length == 11 && icaModel.ica01.Substring(10, 1) == "B")
                {
                    TEMPLATE_SIMPLE = "barcode_money_used.LBX";
                    templatePath = TEMPLATE_DIRECTORY + TEMPLATE_SIMPLE;
                    if (doc.Open(templatePath) == false)
                    {
                        MessageBox.Show("Open() Error: " + doc.ErrorCode);
                        return;
                    }
                }
                else
                {
                    if (doc.Open(templatePath) == false)
                    {
                        MessageBox.Show("Open() Error: " + doc.ErrorCode);
                        return;
                    }
                }
                doc.GetObject("ica01").Text = icaModel.ica01;
                doc.GetObject("ica02").Text = icaModel.ica02;
                doc.GetObject("ica11").Text = string.Format("{0:N0}", icaModel.ica11);

                doc.StartPrint("", PrintOptionConstants.bpoDefault);
                doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                doc.EndPrint();
                doc.Close();

            }
            catch (Exception ex)
            {

                throw;
            }

        }
        #endregion

        #region 雙擊圖片
        private void pbx_icp03_DoubleClick(object sender, EventArgs e)
        {
            if (pbx_icp03.Image != null)
            {
                Image img = pbx_icp03.Image;
                Clipboard.SetImage(img);
            }
        }
        #endregion

        #region 按鈕功能集合
        private void btnButton_Click(object sender, EventArgs e)
        {
            vw_stpt410 masterModel;
            sga_tb sgaMode;
            Result rtnResult = null;
            string invoice = "";
            decimal sga23 = 0;
            ica_tb icaModel;
            vw_stpt410s detailModel;
            StringBuilder sbSql;
            DataRow drDetail;
            try
            {
                Control control = (System.Windows.Forms.Control)sender;
                masterModel = DrMaster.ToItem<vw_stpt410>();
                switch (control.Name.ToLower())
                {
                    case "btninvoice":  //取得發票
                        #region 取得發票
                        if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
                        {
                            masterModel = DrMaster.ToItem<vw_stpt410>();
                            if (masterModel.sga27 == null || GlobalFn.varIsNull(masterModel.sga26)
                                || GlobalFn.varIsNull(masterModel.sga09)
                                )
                            {
                                WfShowErrorMsg("發票日期、發票聯級及發票別不可為空!");
                                return;
                            }

                            rtnResult = BoTax.OfGetInvoice(masterModel.sga26, masterModel.sga09, Convert.ToDateTime(masterModel.sga27), out invoice);
                            if (rtnResult.Success == false)
                            {
                                WfShowErrorMsg(rtnResult.Message);
                                return;
                            }
                            DrMaster["sga24"] = invoice;
                        }
                        #endregion
                        break;
                    case "btnshipping": //新增一筆運費
                        #region 新增一筆運費
                        DrMaster["sgb04_pick"] = "MISC0002";
                        WfFireControlValidated(udt_sgb04_pick);
                        #endregion
                        break;
                    case "btncreditcard": //新增一筆手續費
                        #region 新增一筆手續費
                        if (GlobalFn.isNullRet(masterModel.sga03, "") != "C000001")
                        {
                            WfShowErrorMsg("此功能限門市客人使用");
                            return;
                        }

                        DrMaster["sgb04_pick"] = "MISC0001";
                        WfFireControlValidated(udt_sgb04_pick);
                        #endregion
                        break;
                    case "btnautocal":  //計算成本
                        #region 計算成本
                        if (DrMaster == null)
                            return;
                        sgaMode = DrMaster.ToItem<sga_tb>();
                        rtnResult = BoStp.OfGetSga23(sgaMode, out sga23);

                        if (rtnResult.Success == false)
                        {
                            WfShowErrorMsg(rtnResult.Message);
                            DrMaster["sga23"] = 0;
                            return;
                        }

                        sga23 = GlobalFn.Round(sga23, 0);
                        DrMaster["sga23"] = sga23;
                        #endregion
                        break;
                    case "btnlabel":
                        #region 列印標籤
                        if (DrMaster == null)
                            return;
                        if (uGridDetail.ActiveRow == null)
                            return;
                        drDetail = WfGetUgridDatarow(uGridDetail.ActiveRow);
                        detailModel = drDetail.ToItem<vw_stpt410s>();
                        if (GlobalFn.varIsNull(detailModel.sgb03))
                            return;
                        WfPrintLabel(detailModel.sgb03);
                        #endregion
                        break;

                    case "btnlabelmoney":
                        #region 列印標籤-含價格
                        if (DrMaster == null)
                            return;
                        if (uGridDetail.ActiveRow == null)
                            return;
                        drDetail = WfGetUgridDatarow(uGridDetail.ActiveRow);
                        detailModel = drDetail.ToItem<vw_stpt410s>();
                        if (GlobalFn.varIsNull(detailModel.sgb03))
                            return;

                        WfPrintLabelMoney(detailModel.sgb03);
                        #endregion
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
