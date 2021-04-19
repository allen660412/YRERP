using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.DAL.YRModel;
using YR.ERP.Shared;
using YR.Util;
using YR.Util.Controls;

namespace YR.ERP.Base.Forms
{
    public partial class FrmGridEditBase : YR.ERP.Base.Forms.FrmBase
    {
        #region Property
        public TabInfo TabMaster = new TabInfo();           // 保存主表相關物件 (Grid, datasource, GLL) 的類別    
        public AdmBLL BoMaster                              // 主表的 business object
        {
            get { return TabMaster.BoBasic; }
            set { TabMaster.BoBasic = value; }
        }
        protected int IntCurTab = 1;                        //設定主表停留在那個頁面
        protected int IntTabCount = 5;                      //設定主表有多少個 Tab
        protected int IntMasterGridPos = 0;                 //uGrid_Master要停在那個頁面 如為0則不顯示
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
        protected bool IsRowChkValid = false;               //用於GridRow換列檢查時,資料檢核是否正常 目前作用在KeyUp 滑鼠換列還有按下存檔確認時
        protected bool IsInGridButtonClick = false;         //clickedGridButton時,不要觸發itemchanged事件
        protected bool IsInCopying = false;                 //處理拷貝時不要觸發相關事件,ex rowactived

        protected DataRow DrMaster;                         //主表目前 active 的 datarow

        protected UltraGrid uGridMaster = new UltraGrid();
        public FrmAdvanceQuery frmQuery;                    //進階查詢視窗        
        #endregion Property

        #region 建構子
        public FrmGridEditBase()
        {
            InitializeComponent();
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
            this.isDirectEdit = false;
            this.isMultiRowEdit = false;
            this.intMasterGridPos=0;
             */

            return true;
        }
        #endregion

        #region FrmGridEditBase_Load
        private void FrmGridEditBase_Load(object sender, EventArgs e)
        {

            try
            {
                this.Shown += FrmGridEditBase_Shown;
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
                WfShowRibbonGroup(YREditType.NA, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
                this.KeyPreview = true;
            }
            catch (Exception ex)
            {
                IniSuccess = false;
                WfShowErrorMsg(ex.Message);
            }
        }

        private void FrmGridEditBase_Shown(object sender, EventArgs e)
        {
            try
            {
                if (TabMaster.IsAutoQueryFistLoad)
                {
                    WfAutoQueryFistLoad();
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
                this.Close();
            }

        }
        #endregion

        #region WfIniForm() : 初始化表單
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

                WfIniMasterGrid();
                if (WfIniMaster() == false)
                    return false;

                WfIniToolBarUI();
                //UsbButtom.Height = 24;      //buttom 的toolbar 高度會跳掉先微調
                WfBindMasterByTag(this, TabMaster.AzaTbList, this.DateFormat);
                WfBindMaster();
                return true;
            }
            catch (Exception ex)
            {
                throw (ex);
            }

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
                if (this.IntTabCount <= 0)
                {
                    WfShowBottomStatusMsg("未定義intTabCount!!");
                    return false;
                }

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
                this.StrQuerySecurity = WfGetSecurityString();      //取得權限查詢字串

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

                // 依   tab 數量設定是否可見
                i = 0;
                foreach (Infragistics.Win.UltraWinTabControl.UltraTab ut in this.uTab_Master.Tabs)
                {
                    i++;
                    if (i <= IntTabCount)
                        ut.Visible = true;
                    else
                        ut.Visible = false;
                }

                // 先取得空的資料表
                if (this.TabMaster.ViewTable != "")
                {
                    if (WfQueryByFirstLoad() == false)  //拆成function 之後繼承時在他窗會比較好override
                        return false;
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

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected virtual void WfIniTabMasterInfo()
        {
            //設定master 或detail的 tabinfo
        }
        #endregion

        #region WfAfterfDisplayMode  新增修改刪除後的 focus調整
        protected virtual void WfAfterfDisplayMode()
        {
            try
            {
                WfSetFirstVisibelCellFocus(uGridMaster);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

                UtbmMain.ToolClick += UtbmMain_ToolClick;

                //UtbmMain.MdiMergeable = false;
                #region 產生RibbonTab/及Group
                RibbonTab RtData = new RibbonTab("RtData", "資料");
                //RibbonTab RtReport = new RibbonTab("RtReport", "報表");         //移至下面程式段
                UtbmMain.Ribbon.Tabs.AddRange(new RibbonTab[] { RtData });

                RibbonGroup RibgCrud = new RibbonGroup("RibgCrud", "資料存取");
                RibbonGroup RibgNav = new RibbonGroup("RibgNav", "導覽");
                RibbonGroup RibgDecide = new RibbonGroup("RibgDecide", "處理");
                RibbonGroup RibgExternal = new RibbonGroup("RibgExternal", "其他功能"); //視情況使用 放報表及Action 增加離開的功能
                RtData.Groups.AddRange(new RibbonGroup[] { RibgCrud, RibgNav, RibgDecide });
                //RibbonGroup RibgReport = new RibbonGroup("RibgReport", "相關報表");
                #endregion

                #region RtData/RibgCrud 相關按鈕
                buttonKey = "BtInsert";
                var BtInsert = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtInsert);
                RibgCrud.Tools.AddTool(buttonKey);
                RibgCrud.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                //BtInsert.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["insert_32"];
                BtInsert.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_INSERT];
                BtInsert.SharedPropsInternal.Shortcut = Shortcut.CtrlI;
                BtInsert.SharedProps.Caption = "新 增";

                buttonKey = "BtUpdate";
                var BtUpdate = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtUpdate);
                RibgCrud.Tools.AddTool(buttonKey);
                RibgCrud.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                //BtUpdate.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["update_32"];
                BtUpdate.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_UPDATE];
                BtUpdate.SharedPropsInternal.Shortcut = Shortcut.CtrlU;
                BtUpdate.SharedProps.Caption = "修 改";

                buttonKey = "BtDelete";
                var BtDelete = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtDelete);
                RibgCrud.Tools.AddTool(buttonKey);
                RibgCrud.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                //BtDelete.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["del_32"];
                BtDelete.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_DEL];
                BtDelete.SharedPropsInternal.Shortcut = Shortcut.CtrlD;
                BtDelete.SharedProps.Caption = "刪 除";

                buttonKey = "BtCopy";
                var BtCopy = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtCopy);
                RibgCrud.Tools.AddTool(buttonKey);
                RibgCrud.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                //BtCopy.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["copy_32"];
                BtCopy.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_COPY];
                BtCopy.SharedProps.Caption = "拷 貝";

                buttonKey = "BtQuery";
                var BtQuery = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtQuery);
                RibgCrud.Tools.AddTool(buttonKey);
                RibgCrud.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                //BtQuery.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["query_32"];
                BtQuery.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_QUERY];
                BtQuery.SharedPropsInternal.Shortcut = Shortcut.CtrlQ;
                BtQuery.SharedProps.Caption = "查 詢";

                buttonKey = "BtAdvanceQuery";
                var BtAdvanceQuery = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtAdvanceQuery);
                RibgCrud.Tools.AddTool(buttonKey);
                RibgCrud.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                //BtQuery.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["query_32"];
                BtAdvanceQuery.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_QUERY_ADVANCE];
                //BtQuery.SharedPropsInternal.Shortcut = Shortcut.CtrlQ;
                BtAdvanceQuery.SharedProps.Caption = "進階查詢";
                #endregion

                #region RtData/RibgNav 相關按鈕
                buttonKey = "BtFirst";
                var BtFirst = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtFirst);
                RibgNav.Tools.AddTool(buttonKey);
                RibgNav.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                //BtFirst.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["first_32"];
                BtFirst.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_FIRST];
                BtFirst.SharedPropsInternal.Shortcut = Shortcut.CtrlF;
                BtFirst.SharedProps.Caption = "首 筆";

                buttonKey = "BtPrev";
                var BtPrev = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtPrev);
                RibgNav.Tools.AddTool(buttonKey);
                RibgNav.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                //BtPrev.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["previous_32"];
                BtPrev.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_PREVIOUS];
                BtPrev.SharedPropsInternal.Shortcut = Shortcut.CtrlP;
                BtPrev.SharedProps.Caption = "上一筆";

                buttonKey = "BtNext";
                var BtNext = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtNext);
                RibgNav.Tools.AddTool(buttonKey);
                RibgNav.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                //BtNext.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["next_32"];
                BtNext.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_NEXT];
                BtNext.SharedPropsInternal.Shortcut = Shortcut.CtrlN;
                BtNext.SharedProps.Caption = "下一筆";

                buttonKey = "BtEnd";
                var BtEnd = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtEnd);
                RibgNav.Tools.AddTool(buttonKey);
                RibgNav.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                //BtEnd.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["end_32"];
                BtEnd.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_END];
                BtEnd.SharedPropsInternal.Shortcut = Shortcut.CtrlL;
                BtEnd.SharedProps.Caption = "末 筆";

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

                #region RtData/RibgExternal && RtAction/RibgAction 視子視窗狀況,動態加入action及報表按鈕(確認,取消確認,作廢獨立出來)
                //20150910 增加離開功能
                RtData.Groups.AddRange(new RibbonGroup[] { RibgExternal });
                buttonKey = "BtExit";
                var BtExit = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtExit);
                RibgExternal.Tools.AddTool(buttonKey);
                RibgExternal.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtExit.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_EXIT];
                //BtExit.SharedPropsInternal.Shortcut = Shortcut.CtrlF;
                BtExit.SharedProps.Caption = "離 開";

                var listBt = WfAddAction();
                if (AdoModel.ado09 == "Y")   //增加自動報表的Action
                {
                    ButtonTool btReportDefault = new ButtonTool("BtAutoReport");
                    btReportDefault.SharedProps.Caption = AdoModel.ado10;
                    btReportDefault.SharedProps.Category = "Report";
                    btReportDefault.Tag = this.AdoModel;
                    if (listBt == null)
                    {
                        listBt = new List<ButtonTool>();
                        listBt.Add(btReportDefault);
                    }
                    else
                        listBt.Insert(0, btReportDefault);
                }
                #region 新增Action 及Report
                if (listBt != null)
                {

                    #region Confirm,CancleConfirm,Invalid
                    var listBtConfirm = listBt.Where(p => p.SharedProps.Category.ToLower() == "confirm");
                    if (listBtConfirm != null && listBtConfirm.Count() > 0)
                    {
                        if (!RtData.Groups.Exists("RibgExternal"))
                            RtData.Groups.AddRange(new RibbonGroup[] { RibgExternal });

                        foreach (var bt in listBtConfirm)   //只會有一個,先這樣處理
                        {
                            UtbmMain.Tools.Add(bt);
                            RibgExternal.Tools.AddTool(bt.Key);
                            bt.ToolClick += new ToolClickEventHandler(BtAction_Toolclick);

                            RibgExternal.Tools[bt.Key].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                            //bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["confirm_32"];
                            bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_CONFIRM];
                            bt.SharedProps.Caption = "確 認";
                            //bt.SharedPropsInternal.Shortcut = Shortcut.CtrlE;

                            if (_actionDic == null)
                                _actionDic = new Dictionary<string, string>();
                            _actionDic.Add(bt.Key, bt.SharedProps.Caption);
                        }
                    }

                    var listBtCancleConfirm = listBt.Where(p => p.SharedProps.Category.ToLower() == "cancelconfirm");
                    if (listBtCancleConfirm != null && listBtCancleConfirm.Count() > 0)
                    {
                        if (!RtData.Groups.Exists("RibgExternal"))
                            RtData.Groups.AddRange(new RibbonGroup[] { RibgExternal });

                        foreach (var bt in listBtCancleConfirm)   //只會有一個,先這樣處理
                        {
                            UtbmMain.Tools.Add(bt);
                            RibgExternal.Tools.AddTool(bt.Key);
                            bt.ToolClick += new ToolClickEventHandler(BtAction_Toolclick);

                            RibgExternal.Tools[bt.Key].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                            //bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["cancle_confirm_32"];
                            bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_CANCEL_CONFIRM];
                            bt.SharedProps.Caption = "取消確認";
                            //bt.SharedPropsInternal.Shortcut = Shortcut.CtrlE;

                            if (_actionDic == null)
                                _actionDic = new Dictionary<string, string>();
                            _actionDic.Add(bt.Key, bt.SharedProps.Caption);
                        }
                    }

                    var listBtInvalid = listBt.Where(p => p.SharedProps.Category.ToLower() == "invalid");
                    if (listBtInvalid != null && listBtInvalid.Count() > 0)
                    {
                        if (!RtData.Groups.Exists("RibgExternal"))
                            RtData.Groups.AddRange(new RibbonGroup[] { RibgExternal });

                        foreach (var bt in listBtInvalid)   //只會有一個,先這樣處理
                        {
                            UtbmMain.Tools.Add(bt);
                            RibgExternal.Tools.AddTool(bt.Key);
                            bt.ToolClick += new ToolClickEventHandler(BtAction_Toolclick);

                            RibgExternal.Tools[bt.Key].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                            //bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["invalid_32"];
                            bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_INVALID];
                            bt.SharedProps.Caption = "作 廢";
                            //bt.SharedPropsInternal.Shortcut = Shortcut.CtrlE;

                            if (_actionDic == null)
                                _actionDic = new Dictionary<string, string>();
                            _actionDic.Add(bt.Key, bt.SharedProps.Caption);
                        }
                    }
                    #endregion

                    var listBtAction = listBt.Where(p => p.SharedProps.Category.ToLower() == "action");
                    if (listBtAction != null && listBtAction.Count() > 0)
                    {
                        RibbonTab RtAction = new RibbonTab("RtAction", "額外功能");
                        RibbonGroup RibgAction = new RibbonGroup("RibgAction", "其他功能");
                        UtbmMain.Ribbon.Tabs.AddRange(new RibbonTab[] { RtAction });
                        RtAction.Groups.AddRange(new RibbonGroup[] { RibgAction });
                        RibgAction.PreferredToolSize = RibbonToolSize.Large;

                        //action 在第一個tab
                        if (!RtData.Groups.Exists("RibgExternal"))
                            RtData.Groups.AddRange(new RibbonGroup[] { RibgExternal });

                        //增加Action下拉選單
                        buttonKey = "BtAction";
                        var BtAction = new PopupMenuTool(buttonKey);
                        BtAction.Settings.HotTrackAppearance = UtbmMain.Ribbon.GroupSettings.HotTrackAppearance;
                        BtAction.Settings.ToolAppearance.ForeColor = Color.Black;
                        BtAction.Settings.ToolAppearance.FontData.SizeInPoints = GetStyleLibrary.FontGrid.Size;

                        UtbmMain.Tools.Add(BtAction);
                        RibgExternal.Tools.AddTool(buttonKey);
                        RibgExternal.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                        BtAction.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_ACTION];
                        BtAction.SharedProps.Caption = "Action";
                        BtAction.SharedPropsInternal.Shortcut = Shortcut.CtrlE;

                        foreach (var bt in listBtAction)
                        {
                            bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_ACTION];
                            bt.InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                            UtbmMain.Tools.Add(bt);
                            bt.ToolClick += new ToolClickEventHandler(BtAction_Toolclick);
                            RibgAction.Tools.AddTool(bt.Key);

                            BtAction.Tools.AddTool(bt.Key);
                            if (_actionDic == null)
                                _actionDic = new Dictionary<string, string>();
                            _actionDic.Add(bt.Key, bt.SharedProps.Caption);
                        }
                    }

                    #region RtData/RibgExternal  視子視窗狀況,動態加入report按鈕
                    var listBtReport = listBt.Where(p => p.SharedProps.Category.ToLower() == "report");
                    if (listBtReport != null && listBtReport.Count() > 0)
                    {
                        RibbonTab RtReport = new RibbonTab("RtReport", "報表");
                        RibbonGroup RibgReport = new RibbonGroup("RibgReport", "報表");
                        RibgReport.PreferredToolSize = RibbonToolSize.Large;
                        //RibgReport.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;

                        UtbmMain.Ribbon.Tabs.AddRange(new RibbonTab[] { RtReport });
                        RtReport.Groups.AddRange(new RibbonGroup[] { RibgReport });

                        //report 在第一個tab
                        if (!RtData.Groups.Exists("RibgExternal"))
                            RtData.Groups.AddRange(new RibbonGroup[] { RibgExternal });

                        buttonKey = "BtReport";
                        var BtReport = new PopupMenuTool(buttonKey);
                        BtReport.Settings.HotTrackAppearance = UtbmMain.Ribbon.GroupSettings.HotTrackAppearance;
                        //BtReport.Settings.ToolAppearance.BackColor = Color.FromArgb(210, 210, 210);
                        //BtReport.Settings.ToolAppearance.BackColor = Color.FromArgb(67, 168, 152);
                        BtReport.Settings.ToolAppearance.ForeColor = Color.Black;
                        BtReport.Settings.ToolAppearance.FontData.SizeInPoints = GetStyleLibrary.FontGrid.Size;

                        UtbmMain.Tools.Add(BtReport);
                        RibgExternal.Tools.AddTool(buttonKey);
                        RibgExternal.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                        //BtReport.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["report_32"];
                        BtReport.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_REPORT];
                        BtReport.Settings.UseLargeImages = DefaultableBoolean.True;
                        BtReport.SharedProps.Caption = "報 表";
                        BtReport.SharedPropsInternal.Shortcut = Shortcut.CtrlR;

                        foreach (var bt in listBtReport)
                        {
                            //取得報表類型from Tag Property
                            if (bt.Tag != null)
                            {
                                var adoModel = (ado_tb)bt.Tag;
                                bt.InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                                switch (adoModel.ado13)
                                {
                                    case "1":       //1.清單/明細表
                                        //bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["report_list_32"];
                                        bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_REPORT_LIST];
                                        break;
                                    case "2":       //2.憑證
                                        //bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["report_receipt_32"];
                                        bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_REPORT_RECEIPT];
                                        break;
                                    case "3":       //3.統計表
                                        //bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["report_chart_32"];
                                        bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_REPORT_CHART];
                                        break;
                                    default:
                                        //bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["report_list_32"];
                                        bt.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_REPORT_LIST];
                                        break;
                                }
                            }

                            UtbmMain.Tools.Add(bt);
                            bt.ToolClick += new ToolClickEventHandler(BtAction_Toolclick);
                            RibgReport.Tools.AddTool(bt.Key);

                            BtReport.Tools.AddTool(bt.Key);

                            if (_reportDic == null)
                                _reportDic = new Dictionary<string, string>();
                            _reportDic.Add(bt.Key, bt.SharedProps.Caption);
                        }
                    }
                    #endregion
                }
                #endregion

                #endregion

                #region RtData/RibgDecide 相關按鈕
                buttonKey = "BtOk";
                var BtOk = new ButtonTool(buttonKey);
                //var BtOk = new ControlContainerTool(lsBtKey);
                UtbmMain.Tools.Add(BtOk);
                RibgDecide.Tools.AddTool(buttonKey);
                RibgDecide.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtOk.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["ok_32"];
                BtOk.SharedProps.Caption = "確 認";

                buttonKey = "BtCancel";
                var BtCancel = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtCancel);
                RibgDecide.Tools.AddTool(buttonKey);
                RibgDecide.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtCancel.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["cancel_32"];
                BtCancel.SharedProps.Caption = "取 消";
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
                DrMaster = WfGetActiveDatarow();
                WfCleanBottomMsg();
                WfCleanErrorProvider();
                switch (e.Tool.Key.ToLower())
                {
                    case "btfirst":
                        if (FormEditMode == YREditType.NA)
                        {
                            uGridMaster.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.FirstRowInGrid);
                        }
                        break;
                    case "btprev":
                        if (FormEditMode == YREditType.NA)
                        {
                            uGridMaster.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.PrevRow);
                        }
                        break;
                    case "btnext":
                        if (FormEditMode == YREditType.NA)
                        {
                            uGridMaster.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.NextRow);
                        }
                        break;
                    case "btend":
                        if (FormEditMode == YREditType.NA)
                        {
                            uGridMaster.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.LastRowInBand);
                        }
                        break;

                    case "bthome":
                        if (FormEditMode == YREditType.NA)
                        {
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
                        }
                        break;

                    case "btribbonvisible":
                        if (FormEditMode == YREditType.NA)
                        {
                            if (UtbmMain.Ribbon.DisplayMode == Infragistics.Win.UltraWinToolbars.RibbonDisplayMode.TabsOnly)
                                UtbmMain.Ribbon.DisplayMode = Infragistics.Win.UltraWinToolbars.RibbonDisplayMode.Full;
                            else if (UtbmMain.Ribbon.DisplayMode == Infragistics.Win.UltraWinToolbars.RibbonDisplayMode.Full)
                                UtbmMain.Ribbon.DisplayMode = Infragistics.Win.UltraWinToolbars.RibbonDisplayMode.TabsOnly;
                        }
                        break;

                    case "btinsert":
                        if (FormEditMode == YREditType.NA
                            && TabMaster.CanAddMode == true
                            && (TabMaster.IsCheckSecurity == false || TabMaster.AddTbModel.add03.ToUpper() == "Y")
                            )
                        {
                            IsInCRUDIni = true;
                            if (WfPreInInsertModeCheck() == false)
                                return;
                            if (WfToolbarInsert() == false)
                                return;
                            if (WfDisplayMode() == false)
                                return;
                            WfAfterfDisplayMode();
                            if (this.ActiveControl != null)
                                WfGetOldValue(this.ActiveControl);
                        }
                        else if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)//若原本來源是新增或修改
                        {
                            IsInCRUDIni = true;
                            WfFireControlValidated(this.ActiveControl);
                            //各驗證控製項會將 isItemchkValid設為true
                            if (IsItemchkValid == false)
                                return;

                            if (WfToolbarSave() == false)
                                return;


                            if (WfPreInInsertModeCheck() == false)
                                return;
                            if (WfToolbarInsert() == false)
                                return;
                            if (WfDisplayMode() == false)
                                return;
                            WfAfterfDisplayMode();
                            if (this.ActiveControl != null)
                                WfGetOldValue(this.ActiveControl);
                        }
                        break;

                    case "btcopy":
                        if (FormEditMode == YREditType.NA && TabMaster.CanCopyMode == true
                            && TabMaster.CanAddMode == true
                            && (TabMaster.IsCheckSecurity == false || TabMaster.AddTbModel.add03.ToUpper() == "Y")
                            )
                        {
                            IsInCRUDIni = true;
                            if (WfPreInCopyModeCheck() == false)
                                return;
                            if (DrMaster == null)
                                return;
                            if (WfToolbarCopy() == false)
                                return;
                            if (WfDisplayMode() == false)
                                return;
                            WfAfterfDisplayMode();
                            if (this.ActiveControl != null)
                                WfGetOldValue(this.ActiveControl);
                        }
                        break;
                    case "btdelete":
                        if (FormEditMode == YREditType.NA && TabMaster.CanDeleteMode == true)
                        {
                            if (WfChkUpdAndDelAuthority(YREditType.刪除) == false)
                            {
                                WfShowBottomStatusMsg("無刪除權限");
                                return;
                            }
                            IsInCRUDIni = true;
                            if (DrMaster == null)
                                return;

                            if (WfToolbarDelete() == false)
                                return;
                        }
                        else if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改) //原本狀態為新增或修改
                        {
                            if (WfChkUpdAndDelAuthority(YREditType.刪除) == false)
                            {
                                WfShowBottomStatusMsg("無刪除權限");
                                return;
                            }
                            IsInCRUDIni = true;
                            if (DrMaster == null)
                                return;

                            if (WfToolbarDelete() == false)
                                return;
                        }
                        break;

                    case "btupdate":
                        if (DrMaster == null)
                            return;

                        if (FormEditMode == YREditType.NA && TabMaster.CanUpdateMode == true)
                        {
                            if (WfChkUpdAndDelAuthority(YREditType.修改) == false)
                            {
                                WfShowBottomStatusMsg("無修改權限");
                                return;
                            }
                            IsInCRUDIni = true;
                            if (DrMaster == null)
                                return;
                            if (WfPreInUpdateMode() == false)       //狀態是NA
                                return;

                            if (this.WfToolbarModify() == false)    //這裡狀態才改為修改
                            {
                                this.FormEditMode = YREditType.NA;//有異常時,狀態改為NA,避免被編輯
                                if (BoMaster.TRAN != null)
                                    WfRollback();           //先rollback,避免資料一直被鎖住
                            }
                            if (WfDisplayMode() == false)
                                return;
                            WfAfterfDisplayMode();

                            if (this.ActiveControl != null)
                                WfGetOldValue(this.ActiveControl);
                        }
                        break;

                    case "btquery":
                        if (FormEditMode == YREditType.NA && TabMaster.CanQueryMode == true)
                        {
                            IsInCRUDIni = true;
                            this.WfToolbarQuery();
                            if (WfQueryDisplayMode() == false)
                                return;
                            WfAfterfDisplayMode();
                        }
                        break;

                    case "btadvancequery":
                        if (FormEditMode == YREditType.NA && TabMaster.CanAdvancedQueryMode == true)
                        {
                            IsInCRUDIni = true;
                            if (WfToolbarAdvanceQuery())
                                WfAfterfDisplayMode();
                        }
                        break;

                    case "btaction":
                        UtbmMain.Ribbon.SelectedTab = UtbmMain.Ribbon.Tabs[0];
                        (UtbmMain.Ribbon.Tabs[0].Groups["RibgExternal"].Tools["BtAction"] as PopupMenuTool).ShowPopup();
                        break;

                    case "btok":
                        if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
                        {
                            if (WfPressToolBarSave() == false)
                                return;
                        }
                        else if (FormEditMode == YREditType.查詢)
                        {
                            if (WfPressToolBarQueryOk() == false)
                                return;

                        }
                        break;

                    case "btcancel":
                        if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
                        {
                            if (WfPressToolBarSaveCancel() == false)
                                return;
                        }
                        else if (FormEditMode == YREditType.查詢)
                        {
                            if (WfPressToolBarQueryCancel() == false)
                                return;

                        }
                        break;

                    case "btfrmnavigator":
                        if (FormEditMode != YREditType.NA)
                            return;
                        WfOpenFrmNavigator();
                        break;

                    case "btexit":
                        if (FormEditMode == YREditType.NA)
                        {
                            this.Close();
                        }
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

        #region BtAction_Toolclick Action按鈕(報表與ACTION)事件集中管理 不公開
        private void BtAction_Toolclick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            string add13;
            List<string> add13List;
            string actionName;
            int chkCnts = 0;
            try
            {
                //檢查是否具有 action 權限
                actionName = e.Tool.Key;
                if (StrFormID.ToLower() != "admi600")    //有這隻權限的人不需檢查action
                {
                    if (TabMaster.AddTbModel == null || GlobalFn.varIsNull(TabMaster.AddTbModel.add13))
                    {
                        WfShowErrorMsg("無此功能權限!");
                        return;
                    }
                    add13 = TabMaster.AddTbModel.add13;
                    add13List = add13.Split(',').ToList<string>();
                    chkCnts = add13List.Where(p => p.ToLower() == actionName.ToLower()).Count();
                    if (chkCnts == 0)
                    {
                        WfShowErrorMsg("無此功能權限!");
                        return;
                    }
                }

                this.Cursor = Cursors.WaitCursor;
                if (actionName.ToLower() == "btautoreport")
                    WfReport();
                else
                {
                    WfSetBllTransaction();
                    WfActionClick(e.Tool.Key);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    WfShowErrorMsg(ex.Message + Environment.NewLine + ex.InnerException.Message);
                WfShowErrorMsg(ex.Message);
            }
            finally
            {
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
                if (keyData == (Keys.Control | Keys.Enter))
                {
                    if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.修改 || FormEditMode == YREditType.新增)
                    {
                        utb = UtbmMain.Tools["BtOk"];
                        eTool = new ToolClickEventArgs(utb, null);
                        UtbmMain_ToolClick(UtbmMain, eTool);
                        return true;
                    }
                }

                if (keyData == (Keys.Escape))
                {
                    if (FormEditMode == YREditType.NA)
                    {
                        this.Close();
                        return true;
                    }

                    if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.修改 || FormEditMode == YREditType.新增)
                    {
                        utb = UtbmMain.Tools["BtCancel"];
                        eTool = new ToolClickEventArgs(utb, null);
                        UtbmMain_ToolClick(UtbmMain, eTool);
                        return true;
                    }
                }

                if (keyData == (Keys.F1))
                {
                    utb = UtbmMain.Tools["btribbonvisible"];
                    eTool = new ToolClickEventArgs(utb, null);
                    UtbmMain_ToolClick(UtbmMain, eTool);
                    return true;
                }

                //表單若無可停註控制項時,會切換mdi tab ,避免引發先做以下處理
                if (keyData == (Keys.Shift | Keys.Tab) || keyData == (Keys.Tab))
                {
                    if (FormEditMode == YREditType.NA)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        /**********************  新增/查詢/拷貝/存檔/刪除 相關 Function **********************/

        #region WfPressToolBarSave 按下存檔時
        protected virtual bool WfPressToolBarSave()
        {
            try
            {

                WfFireControlValidated(this.ActiveControl);
                //各驗證控製項會將 isItemchkValid設為true
                if (IsItemchkValid == false)
                    return true;

                if (WfToolbarSave() == false)
                    return false;
                this.FormEditMode = YREditType.NA;
                WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
                if (WfDisplayMode() == false)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPressToolBarQueryOk 按下查詢完成時
        protected virtual bool WfPressToolBarQueryOk()
        {
            try
            {
                WfQueryOk();
                WfQueryOkEnd();
                if (WfDisplayMode() == false)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPressSaveCancel 存檔取消時
        protected virtual bool WfPressToolBarSaveCancel()
        {
            try
            {
                if (this.WfRowChangeCheck(1) == true)
                {
                    IsInSaveCancle = true;
                    WfSaveCancel();
                }

                if (WfDisplayMode() == false)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPressSaveCancel 查詢取消時
        protected virtual bool WfPressToolBarQueryCancel()
        {
            try
            {
                if (FormEditMode == YREditType.查詢)
                    WfQueryCancel();

                if (WfDisplayMode() == false)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPreInUpdateMode
        protected virtual bool WfPreInUpdateMode()
        {
            try
            {
                //進修改模式時,要重新查詢master資料,避免資料dirty
                if (WfRetrieveMaster() == false)
                    return false;

                if (TabMaster.CanUseRowLock == true)
                {
                    if (WfLockMasterRow() == false)
                        return false;
                }
                else
                    if (WfBeginTran() == false)
                        return false;


                WfSetBllTransaction();

                if (WfPreInUpdateModeCheck() == false)
                {
                    if (BoMaster.TRAN != null)
                        WfRollback();
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

        #region WfPreInInsertModeCheck 進新增模式前的檢查及清變數與設定變數
        protected virtual bool WfPreInInsertModeCheck()
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

        #region WfPreInCopyModeCheck
        protected virtual bool WfPreInCopyModeCheck()
        {

            return true;
        }
        #endregion

        #region WfRetrieveMaster() 重新查詢Master資料 By KEY值 -Update時使用
        protected bool WfRetrieveMaster()
        {
            StringBuilder sbSql;
            DataRow dr;
            try
            {
                if (TabMaster.PKParms == null || TabMaster.PKParms.Count == 0)
                {
                    WfShowErrorMsg("未設定主表的PK值! (WfRetrieveMaster)");
                    return false;
                }
                sbSql = new StringBuilder();
                sbSql.AppendLine(string.Format("SELECT * FROM {0}", TabMaster.ViewTable));
                sbSql.AppendLine("WHERE 1=1");
                foreach (SqlParameter sp in TabMaster.PKParms)
                {
                    sbSql.AppendLine(string.Format("AND {0}=@{0}", sp.ParameterName));
                    sp.Value = DrMaster[sp.ParameterName];
                }

                dr = BoMaster.OfGetDataRow(sbSql.ToString(), TabMaster.PKParms.ToArray());

                DrMaster.ItemArray = dr.ItemArray;
                DrMaster.AcceptChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
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

        #region WfPreInUpdateModeCheck() 進修改模式前檢查,及設定變數
        protected virtual bool WfPreInUpdateModeCheck()
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

        #region WfToolbarModify() : 主表修改 function
        /// <summary>
        /// 主表修改 function
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean WfToolbarModify()
        {
            try
            {
                this.IsChanged = false;

                this.FormEditMode = YREditType.修改;
                WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
                WfSetControlPropertyByMode(this);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
        #endregion 主表修改 function

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
                WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
                WfSetControlPropertyByMode(this);
                this.IsChanged = true; //為了怕跳脫key檢查 新增的都設為有異動
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
        #endregion 主表新增 function

        #region WfToolbarCopy 主表拷貝
        protected virtual bool WfToolbarCopy()
        {
            try
            {
                IsInCopying = true;
                this.FormEditMode = YREditType.新增;
                DataRow drMaster = DrMaster.Table.NewRow();
                drMaster.ItemArray = DrMaster.ItemArray;

                this.WfSetMasterDefaultByCopy(drMaster);

                this.IsChanged = false;
                this.WfAddMasterDataRow(drMaster);
                WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
                WfSetControlPropertyByMode(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                IsInCopying = false;
            }
        }
        #endregion

        #region WfToolbarDelete() : 刪除主表記錄 function
        /// <summary>
        /// 刪除主表記錄 function
        /// </summary>
        /// <returns></returns>
        //waitfix:交易及檢查未處理
        protected virtual Boolean WfToolbarDelete()
        {
            DataTable dtMasterOld;
            int activeIndex = -1;
            try
            {
                if (WfPreDeleteCheck(DrMaster) == false)
                    return false;

                var result = WfShowConfirmMsg("是否確定要刪除資料 ?");

                if (result != DialogResult.Yes)
                    return true;
                //記錄原來的 GridRow index
                if (this.uGridMaster.ActiveRow != null)
                { activeIndex = this.uGridMaster.ActiveRow.Index; }
                else
                { activeIndex = -1; }

                //複製要刪除的資料
                dtMasterOld = DrMaster.Table.Clone();
                dtMasterOld.ImportRow(DrMaster);

                if (TabMaster.CanUseRowLock == true)
                {
                    if (WfLockMasterRow() == false)
                        return false;
                }
                else
                    if (WfBeginTran() == false)
                        return false;

                WfSetBllTransaction();

                //主檔與明細檔的額外資料要在此先做處理
                if (WfDeleteAppenUpdate(dtMasterOld.Rows[0]) == false)
                {
                    WfRollback();
                    return false;
                }
                DrMaster.Delete();
                this.TabMaster.BoBasic.OfUpdate(BoMaster.TRAN, DrMaster.Table);

                #region 如果已無定位在下一筆 Row
                if (this.TabMaster.DtSource.Rows.Count != 0)
                {
                    if (uGridMaster.Rows.Count == 1)
                    {
                        uGridMaster.ActiveRow = uGridMaster.Rows[0];
                    }
                    else
                    {
                        if (activeIndex < 0)
                        { WfActiveGridNextRow(this.uGridMaster, 9999); }
                        else
                        { WfActiveGridNextRow(this.uGridMaster, activeIndex); }
                    }
                }
                #endregion

                TabMaster.DtSource.AcceptChanges();
                WfCommit();
                return true;
            }
            catch (Exception ex)
            {
                WfRollback();
                throw ex;
            }
            finally
            {
                this.IsChanged = false;
            }
        }
        #endregion

        #region WfDeleteAppenUpdate 刪除時使用,若需單身資料,要先在查此查詢資料庫並且異動
        protected virtual bool WfDeleteAppenUpdate(DataRow pDr)
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

        #region WfPreDeleteCheck 進主檔刪除前檢查
        protected virtual bool WfPreDeleteCheck(DataRow pDr)
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

        #region WfToolbarSave() : 主表存檔 function
        /// <summary>
        /// 主表存檔 function
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean WfToolbarSave()
        {
            uGridMaster.PerformAction(UltraGridAction.ExitEditMode);
            uGridMaster.UpdateData();
            BindingMaster.EndEdit();
            if (DrMaster == null)
            {
                WfShowErrorMsg("查無要存檔的資料!");
                return false;
            }

            try
            {
                if (this.WfBeginTran() == false)
                { return false; }

                WfSetBllTransaction();

                this.errorProvider.Clear();
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

                if (this.TabMaster.DtSource.GetChanges() != null)
                {
                    if (this.TabMaster.BoBasic.OfUpdate(BoMaster.TRAN, this.TabMaster.DtSource.GetChanges()) < 0)
                    {
                        { throw new Exception("儲存主表時失敗(boBasic.of_update)，請檢核 !"); }
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
                throw ex;
            }
            finally
            {
                //this.Cursor = Cursors.Default; //改到工具列處理
            }

            this.TabMaster.DtSource.AcceptChanges();
            //移到各驗證項處理
            //this.FormEditMode = YREditType.NA;
            //WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);   
            WfShowBottomStatusMsg("存檔成功!");
            this.IsChanged = false;
            return true;
        }
        #endregion 刪除主表記錄 function

        #region WfSaveCancel() 新增或修改後按下取消按鈕
        protected virtual void WfSaveCancel()
        {
            try
            {
                if (BoMaster.TRAN != null)
                    WfRollback();//TODO:先全部都ROLLBACK
                IsInSaveCancle = true;

                this.errorProvider.Clear();
                uGridMaster.PerformAction(UltraGridAction.ExitEditMode);
                BindingMaster.CancelEdit();
                TabMaster.DtSource.RejectChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.FormEditMode = YREditType.NA;
                WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
                this.IsChanged = false;
                this.IsInSaveCancle = false;
            }
        }
        #endregion

        #region WfQueryDisplayMode 查詢後的狀態處理 readonly
        protected virtual bool WfQueryDisplayMode()
        {
            try
            {
                WfSetControlsReadOnlyRecursion(this, false);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        #region WfQueryOk() 查詢後按下OK按鈕
        protected virtual bool WfQueryOk()
        {
            DataTable dtSource = null;
            string strQueryAll;
            string strQueryApend = "";
            List<SqlParameter> sqlParmList;
            try
            {
                uGridMaster.PerformAction(UltraGridAction.ExitEditMode);
                this.TabMaster.DtSource.EndInit();
                sqlParmList = new List<SqlParameter>();
                this.StrQueryWhere = BoMaster.WfCombineQuerystring(TabMaster, out sqlParmList);

                StrQueryWhere = GlobalFn.isNullRet(StrQueryWhere, "");
                StrQueryWhereAppend = GlobalFn.isNullRet(StrQueryWhereAppend, "");
                //不下條件仍可以查詢,後續要再做處理
                if (StrQueryWhereAppend.Trim().Length > 0)
                {
                    if (StrQueryWhereAppend.ToLower().TrimStart().IndexOf("and ") < 0)
                    {
                        strQueryApend = " AND " + StrQueryWhereAppend.TrimStart();
                    }
                    else
                        strQueryApend = StrQueryWhereAppend.TrimStart();

                }
                strQueryAll = StrQueryWhere + strQueryApend + this.StrQuerySecurity;

                dtSource = this.TabMaster.BoBasic.OfSelect(strQueryAll, sqlParmList);

                this.ResumeLayout(true);

                this.FormEditMode = YREditType.NA;       //這裡就要把行為改為 YREditType.因為會觸發 retrieve detail for multi form
                this.TabMaster.DtSource = dtSource;
                this.WfSetMasterDatasource(this.TabMaster.DtSource);

                if (this.uGridMaster.Rows.Count > 0)
                {
                    this.uGridMaster.PerformAction(UltraGridAction.FirstRowInGrid);
                    uGridMaster.ActiveRow = uGridMaster.DisplayLayout.Rows.GetRowAtVisibleIndex(0);       //因為這裡不會將第一列設定activeRow 所以人工處理
                    DrMaster = WfGetActiveDatarow();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.FormEditMode = YREditType.NA;
                WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
                this.IsChanged = false;
            }
            return true;
        }
        #endregion

        #region WfQueryOkEnd 查詢完畢時--通常做為後續資料處理
        protected virtual bool WfQueryOkEnd()
        {

            return true;
        }
        #endregion

        #region WfQueryCancel() 查詢後按下取消按鈕
        protected virtual void WfQueryCancel()
        {
            try
            {
                //不能用清除的,因為會有型別問題
                //TabMaster.DtSource.Clear();

                // 取得空的資料表
                if (this.TabMaster.ViewTable != "")
                {
                    this.TabMaster.DtSource = this.TabMaster.BoBasic.OfSelect(" WHERE 1=2 ");
                    this.WfSetMasterDatasource(this.TabMaster.DtSource);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.FormEditMode = YREditType.NA;
                WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
                this.IsChanged = false;
            }
        }
        #endregion

        #region WfShowAllToolRibbons 依RibbonTab 開關所有其中的RibbonGroup
        protected void WfShowAllToolRibbons(Infragistics.Win.UltraWinToolbars.RibbonTab pUrtb, bool PCanVisible)
        {
            try
            {
                foreach (Infragistics.Win.UltraWinToolbars.RibbonGroup uRibbonGroup in pUrtb.Groups)
                {
                    uRibbonGroup.Visible = PCanVisible;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfShowRibbons(YREditType pYRedit) 依表單狀態調整工具列的RibbonGroup
        protected virtual void WfShowRibbonGroup(YREditType pYRedit, bool pIsCheckSecurity, add_tb pAddTb)
        {
            Infragistics.Win.UltraWinToolbars.RibbonTab rbt;
            try
            {

                if (UtbmMain.Ribbon != null && UtbmMain.Ribbon.Tabs.Count > 0)
                    rbt = UtbmMain.Ribbon.Tabs[0];
                else
                    return;

                switch (pYRedit)
                {
                    case YREditType.NA://無狀態
                        WfShowAllToolRibbons(rbt, true);
                        if (rbt.Groups.Exists("RibgCrud"))
                        {
                            //先全開
                            foreach (ButtonTool btCrud in rbt.Groups["RibgCrud"].Tools)
                            {
                                btCrud.SharedProps.Visible = true;
                            }

                            //新增
                            if (TabMaster.CanAddMode == false
                                || (pIsCheckSecurity == true &&
                                    (pAddTb == null || GlobalFn.isNullRet(pAddTb.add03, "").ToUpper() == "N")
                                    )
                                )
                            {
                                rbt.Groups["RibgCrud"].Tools["BtInsert"].SharedProps.Visible = false;
                            }

                            //修改
                            if (TabMaster.CanUpdateMode == false)
                            {
                                rbt.Groups["RibgCrud"].Tools["BtUpdate"].SharedProps.Visible = false;
                            }

                            //複製
                            if (TabMaster.CanCopyMode == false
                               || (pIsCheckSecurity == true &&
                                   (pAddTb == null || GlobalFn.isNullRet(pAddTb.add03, "").ToUpper() == "N")
                                   )
                               )
                            {
                                rbt.Groups["RibgCrud"].Tools["BtCopy"].SharedProps.Visible = false;
                            }

                            //查詢
                            if (TabMaster.CanQueryMode == false)
                            {
                                rbt.Groups["RibgCrud"].Tools["BtQuery"].SharedProps.Visible = false;
                            }

                            //進階查詢
                            if (TabMaster.CanAdvancedQueryMode == false)
                            {
                                rbt.Groups["RibgCrud"].Tools["BtAdvanceQuery"].SharedProps.Visible = false;
                            }
                            else

                                if (TabMaster.CanDeleteMode == false)
                                {
                                    rbt.Groups["RibgCrud"].Tools["BtDelete"].SharedProps.Visible = false;
                                }
                            if (TabMaster.CanAddMode == false && TabMaster.CanCopyMode == false
                                && TabMaster.CanDeleteMode == false
                                && TabMaster.CanQueryMode == false && TabMaster.CanAdvancedQueryMode == false
                                && TabMaster.CanUpdateMode == false
                                )
                                rbt.Groups["RibgCrud"].Visible = false;
                        }
                        rbt.Groups["RibgDecide"].Visible = false;

                        if (rbt.Groups.Exists("RibgDetail"))    //design mode會有問題因此加判斷
                        {
                            rbt.Groups["RibgDetail"].Visible = false;
                        }
                        if (rbt.Groups.Exists("RibgNav"))
                        {
                            if (TabMaster.CanNavigator == false)
                                rbt.Groups["RibgNav"].Visible = false;
                        }
                        break;
                    case YREditType.新增://新增
                        WfShowAllToolRibbons(rbt, false);
                        if (rbt.Groups.Exists("RibgExternal"))
                        {
                            if (rbt.Groups["RibgExternal"].Tools.Exists("BtAction"))
                            {
                                rbt.Groups["RibgExternal"].Visible = true;
                                if (rbt.Groups["RibgExternal"].Tools.Exists("BtReport"))
                                    rbt.Groups["RibgExternal"].Tools["BtReport"].SharedProps.Visible = false;
                            }
                        }
                        if (rbt.Groups.Exists("RibgCrud"))
                        {
                            rbt.Groups["RibgCrud"].Visible = true;
                            foreach (ButtonTool bt in rbt.Groups["RibgCrud"].Tools)
                            {
                                if (bt.Key != "BtInsert" && bt.Key != "BtDelete")
                                    bt.SharedProps.Visible = false;
                            }
                        }

                        rbt.Groups["RibgDecide"].Visible = true;
                        if (rbt.Groups.Exists("RibgDetail"))
                            rbt.Groups["RibgDetail"].Visible = true;
                        break;
                    case YREditType.修改://修改
                        WfShowAllToolRibbons(rbt, false);
                        if (rbt.Groups.Exists("RibgExternal"))
                        {
                            if (rbt.Groups["RibgExternal"].Tools.Exists("BtAction"))
                            {
                                rbt.Groups["RibgExternal"].Visible = true;
                                if (rbt.Groups["RibgExternal"].Tools.Exists("BtReport"))
                                    rbt.Groups["RibgExternal"].Tools["BtReport"].SharedProps.Visible = false;
                            }
                        }
                        if (rbt.Groups.Exists("RibgCrud"))
                        {
                            rbt.Groups["RibgCrud"].Visible = true;
                            foreach (ButtonTool bt in rbt.Groups["RibgCrud"].Tools)
                            {
                                if (bt.Key != "BtInsert" && bt.Key != "BtDelete")
                                    bt.SharedProps.Visible = false;
                            }
                        }

                        rbt.Groups["RibgDecide"].Visible = true;
                        if (rbt.Groups.Exists("RibgDetail"))
                            rbt.Groups["RibgDetail"].Visible = true;
                        break;
                    case YREditType.查詢://查詢
                        WfShowAllToolRibbons(rbt, false);
                        if (rbt.Groups.Exists("RibgDecide"))
                            rbt.Groups["RibgDecide"].Visible = true;
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfToolbarQuery :　查詢表頭資料
        /// <summary>
        /// 查詢表頭資料 Function
        /// </summary>
        /// <returns></returns
        /// 
        protected virtual Boolean WfToolbarQuery()
        {
            try
            {
                //this.isInQuerying = true;
                this.FormEditMode = YREditType.查詢;
                WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
                this.StrQueryWhere = "";

                TabMaster.DtSource = this.TabMaster.BoBasic.OfSelect(" AND 1<>1");
                this.WfSetMasterDatasource(this.TabMaster.DtSource);

                //將所有的型別都改為 STRING
                foreach (DataColumn dcTemp in TabMaster.DtSource.Columns)
                {
                    if (dcTemp.DataType.Name == "Byte[]")
                        continue;

                    dcTemp.Prefix = dcTemp.DataType.Name;
                    dcTemp.DataType = typeof(string);// System.Type.GetType("System.String");
                }

                WfAddMasterDataRow(TabMaster.DtSource.NewRow());
                WfSetControlPropertyByMode(this);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.ResumeLayout(false);
                //this.isInQuerying = false;
                //this.strMode = YREditType.NA;
            }
            return true;
        }
        #endregion
        
        #region WfToolbarAdvanceQuery :　進階查詢功能
        protected virtual Boolean WfToolbarAdvanceQuery()
        {
            DataTable dtSource = null;
            string strQueryAll;
            string strQueryApend = "";
            AdvanceQueryInfo advanceQueryInfo = null;
            List<SqlParameter> sqlParmList = null;
            try
            {
                if (this.frmQuery == null)
                {
                    advanceQueryInfo = new AdvanceQueryInfo();
                    advanceQueryInfo.Key = AdoModel.ado06;
                    advanceQueryInfo.Result = DialogResult.Cancel;
                    frmQuery = new FrmAdvanceQuery(this.LoginInfo, advanceQueryInfo, null);
                    frmQuery.ShowDialog(this);
                }
                else
                {
                    advanceQueryInfo = this.frmQuery.TabMaster;
                    this.frmQuery.ShowDialog(this);
                }


                if (advanceQueryInfo.Result == System.Windows.Forms.DialogResult.OK)
                {
                    strQueryApend = advanceQueryInfo.StrWhereAppend;
                    sqlParmList = advanceQueryInfo.sqlParmsList;

                    StrQueryWhere = GlobalFn.isNullRet(StrQueryWhere, "");
                    StrQueryWhereAppend = GlobalFn.isNullRet(StrQueryWhereAppend, "");
                    //不下條件仍可以查詢,後續要再做處理
                    if (StrQueryWhereAppend.Trim().Length > 0)
                    {
                        if (StrQueryWhereAppend.ToLower().TrimStart().IndexOf("and ") < 0)
                        {
                            strQueryApend = " AND " + StrQueryWhereAppend.TrimStart();
                        }
                        else
                            strQueryApend = StrQueryWhereAppend.TrimStart();
                    }
                    strQueryAll = StrQueryWhere + strQueryApend + this.StrQuerySecurity;
                    dtSource = this.TabMaster.BoBasic.OfSelect(strQueryAll, sqlParmList);

                    this.ResumeLayout(true);

                    this.FormEditMode = YREditType.NA;       //這裡就要把行為改為 YREditType.因為會觸發 retrieve detail for multi form
                    this.TabMaster.DtSource = dtSource;
                    this.WfSetMasterDatasource(this.TabMaster.DtSource);

                    if (this.uGridMaster.Rows.Count > 0)
                    {
                        this.uGridMaster.PerformAction(UltraGridAction.FirstRowInGrid);
                        uGridMaster.ActiveRow = uGridMaster.DisplayLayout.Rows.GetRowAtVisibleIndex(0);       //因為這裡不會將第一列設定activeRow 所以人工處理
                        DrMaster = WfGetActiveDatarow();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.ResumeLayout(false);
                //this.isInQuerying = false;
                //this.strMode = YREditType.NA;
            }
            return true;
        }
        #endregion

        #region WfSetControlPropertyByMode 依目前表單狀態對各元件做不同的處理 Ex:checkbox 三相或兩相
        protected virtual Boolean WfSetControlPropertyByMode(Control pControl)
        {
            string strType = "";
            string tagValue = "";
            StringBuilder lsbTip;
            aza_tb azaModel = null;
            try
            {
                foreach (Control lcontrol in pControl.Controls)
                {
                    strType = lcontrol.GetType().ToString();
                    if (lcontrol.Tag != null)
                    {
                        tagValue = lcontrol.Tag.ToString().Trim();
                        azaModel = (from o in TabMaster.AzaTbList
                                 where o.aza03 == tagValue
                                 select o
                            ).FirstOrDefault<aza_tb>()
                                ;
                        if (azaModel != null)
                        {
                            lsbTip = new StringBuilder();
                            lsbTip.AppendLine(string.Format("{0}-{1}", azaModel.aza03, azaModel.aza04));
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
                            var uGrid = (lcontrol as Infragistics.Win.UltraWinGrid.UltraGrid);
                            if (FormEditMode == YREditType.查詢)
                                uGrid.DataBind();//來源資料表均轉為string 所以重新bind來源~
                            if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
                            {
                                if (uGrid.ActiveRow != null && uGrid.ActiveCell != null)
                                {
                                    //重新觸發BeforeCellActivate 讓資料可做編輯
                                    var e = new CancelableCellEventArgs(uGrid.ActiveCell);
                                    uGridMaster_BeforeCellActivate(uGrid, e);
                                    uGrid.PerformAction(UltraGridAction.EnterEditMode);
                                    uGrid.ActiveCell.SelectAll();
                                }
                            }
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

        #region WfFormCheck() 存檔前檢查
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

        #region WfAfterFormCheck() 存檔後處理,通常為放入Pk
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

        #region WfAppendUpdate 存檔後處理額外的資料表 新增、修改、刪除...
        protected virtual bool WfAppendUpdate()
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

        #region WfChkUpdAndDelAuthority
        /// <summary>
        /// 檢查該筆資料是否有"刪除"及"修改"的權限
        /// </summary>
        /// <param name="pYREditType"></param>
        /// <returns></returns>
        protected bool WfChkUpdAndDelAuthority(YREditType pYREditType)
        {
            bool isPerson = false;    //個人資料
            bool isGroup = false;     //群組資料
            string personValue = "", groupValue = "", groupLevel = "";
            try
            {
                //if (AdoModel == null || Global_Fn.varIsNull(AdoModel.ado15) || Global_Fn.varIsNull(AdoModel.ado16))
                //    return true;
                if (AdoModel == null || GlobalFn.varIsNull(TabMaster.UserColumn) || GlobalFn.varIsNull(TabMaster.GroupColumn))
                    return true;
                if (TabMaster.AddTbModel == null)
                    return true;
                //先檢查他組是否有勾
                if (pYREditType == YREditType.修改)
                {
                    if (TabMaster.AddTbModel.add11 == "Y")
                        return true;
                }
                else  //刪除
                {
                    if (TabMaster.AddTbModel.add12 == "Y")
                        return true;
                }
                //再檢查個人是否有勾且屬於個人資料
                //personValue = DRMASTER[AdoModel.ado15].ToString();
                personValue = DrMaster[TabMaster.UserColumn].ToString();
                if (personValue == LoginInfo.UserNo)
                    isPerson = true;
                if (pYREditType == YREditType.修改)
                {
                    if (TabMaster.AddTbModel.add05 == "Y" && isPerson)
                        return true;
                }
                else  //刪除
                {
                    if (TabMaster.AddTbModel.add06 == "Y" && isPerson)
                        return true;
                }

                groupValue = DrMaster[TabMaster.GroupColumn].ToString();
                var adeModel = BoMaster.OfGetAdeModel(groupValue);
                if (adeModel == null)
                {
                    WfShowBottomStatusMsg("查無使用者群組資料!");
                    return false;
                }
                groupLevel = adeModel.ade03;
                if (groupLevel.StartsWith(LoginInfo.GroupLevel))
                    isGroup = true;
                if (pYREditType == YREditType.修改)
                {
                    if (TabMaster.AddTbModel.add08 == "Y" && isGroup)
                        return true;
                }
                else  //刪除
                {
                    if (TabMaster.AddTbModel.add09 == "Y" && isGroup)
                        return true;
                }


                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAutoQueryFistLoad 載入時，自動查詢
        protected virtual void WfAutoQueryFistLoad()
        {
            try
            {
                TabMaster.DtSource.Rows.Add(TabMaster.DtSource.NewRow());
                WfQueryOk();
                WfQueryOkEnd();
                WfDisplayMode();
                WfAfterfDisplayMode();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /******************* ultraGrid 事件及方法 ***********************/
        #region WfIniMasterGrid
        protected virtual void WfIniMasterGrid()
        {
            try
            {
                uGridMaster.BeforeRowDeactivate += UGrid_BeforeRowDeactivate;
                uGridMaster.AfterRowActivate += UGrid_Master_AfterRowActivate;

                uGridMaster.BeforeCellActivate += uGridMaster_BeforeCellActivate;
                uGridMaster.AfterCellActivate += new System.EventHandler(this.UGridMaster_AfterCellActivate);
                uGridMaster.KeyUp += new KeyEventHandler(ultraGrid_KeyUp);

                //uGridMaster.AfterCellActivate += new System.EventHandler(this.UGridMaster_AfterCellActivate);
                uGridMaster.BeforeExitEditMode += new Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventHandler(UGridMaster_BeforeExitEditMode);
                uGridMaster.AfterEnterEditMode += new System.EventHandler(UGridMaster_AfterEnterEditMode);
                uGridMaster.BeforeRowActivate += new RowEventHandler(UGridMaster_BeforeRowActivate);
                if (IntTabCount > 0)
                {
                    if (IntMasterGridPos == 0)
                    {
                        uGridMaster.Visible = false;
                        uGridMaster.Size = new System.Drawing.Size(0, 0);
                        uTab_Master.Tabs[0].TabPage.Controls.Add(uGridMaster);
                    }
                    if (IntMasterGridPos != 0 && IntMasterGridPos <= IntTabCount)
                    {
                        ((System.ComponentModel.ISupportInitialize)(uGridMaster)).BeginInit();
                        uTab_Master.Tabs[IntMasterGridPos - 1].TabPage.Controls.Add(uGridMaster);
                        uGridMaster.Dock = DockStyle.Fill;
                        uGridMaster.ClickCellButton += new CellEventHandler(UltraGrid_ClickCellButton);
                        WfSetAppearance(uGridMaster, 1);
                        ((System.ComponentModel.ISupportInitialize)(uGridMaster)).EndInit();
                    }
                }
                TabMaster.UGrid = uGridMaster;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region uGridMaster_BeforeCellActivate
        private void uGridMaster_BeforeCellActivate(object sender, CancelableCellEventArgs e)
        {
            try
            {
                if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
                {
                    BeforeEnterCellInfo bec = new BeforeEnterCellInfo();
                    bec.Column = e.Cell.Column.Key;
                    bec.Row = (e.Cell.Row.ListObject as DataRowView).Row;
                    bec.Value = e.Cell.Value;
                    bec.Cell = e.Cell;

                    WfBeforeCellActivate(bec);
                    e.Cancel = bec.Cancel;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfBeforeCellActivate 進入GridCell前事件
        protected virtual bool WfBeforeCellActivate(BeforeEnterCellInfo e)
        {

            return true;
        }
        #endregion

        #region UGrid_Master_AfterRowActivate(sender, EventArgs e)  : GridRow 作用後事件
        protected virtual void UGrid_Master_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                if (InFormClosing == true)
                    return;
                //DrMaster = WfGetActiveDatarow();
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
            IsRowChkValid = false;      //初始化換列檢查為不通過 主要會在keyUp 使用
            try
            {
                if (IsInCopying == true || InFormClosing == true)
                    return;

                if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
                {
                    if (this.IsChanged == true)
                    {
                        if (WfToolbarSave() == false)
                            e.Cancel = true;
                        IsRowChkValid = true;   //通過檢查
                    }
                    return;
                }

                //if (this.WfRowChangeCheck(1) == false)
                //    e.Cancel = true;

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

        #region UGridMaster_BeforeExitEditMode
        protected internal virtual void UGridMaster_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
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

                if (!uGridCell.Value.Equals(OldValue))
                {
                    WfSetBllTransaction();
                    var itemCheckInfo = new ItemCheckInfo();
                    itemCheckInfo.Row = drActive;
                    itemCheckInfo.Value = uGridCell.Value;
                    itemCheckInfo.Column = colName;

                    #region 檢查欄位型別及輸入長度
                    //foreach (TabDetailInfo detailInfo in TabDetailList)
                    //{
                    //    if (detailInfo.DtSource == null || detailInfo.DtSource.Prefix.ToLower() != detailInfo.AzaTbList.FirstOrDefault().aza01)
                    //        continue;

                    //    var azaModel = detailInfo.AzaTbList.Where(p => p.aza03.ToLower() == colName.ToLower())
                    //                        .FirstOrDefault();
                    //    if (azaModel != null && !GlobalFn.varIsNull(azaModel.aza01))
                    //    {
                    //        if (azaModel.aza08 == "nvarchar")
                    //        {
                    //            //檢查欄位長度
                    //            if (azaModel.aza09 < GlobalFn.isNullRet(itemCheckInfo.Value, "").Length)
                    //            {
                    //                WfShowErrorMsg(string.Format("僅可輸入{0}個字元,目前{1}!", azaModel.aza09.ToString(),
                    //                                                            GlobalFn.isNullRet(itemCheckInfo.Value, "").Length));
                    //                e.Cancel = true;
                    //                IsItemchkValid = false;
                    //                uGridCell.Value = OldValue;
                    //                return;
                    //            }
                    //        }
                    //        else if (azaModel.aza08 == "int" || azaModel.aza08 == "numeric")
                    //        {
                    //            //檢查型別
                    //            decimal parseDecimal = 0;
                    //            if (!decimal.TryParse(GlobalFn.isNullRet(itemCheckInfo.Value, "0"), out parseDecimal))
                    //            {
                    //                WfShowErrorMsg(string.Format("請輸入數字!"));
                    //                e.Cancel = true;
                    //                IsItemchkValid = false;
                    //                uGridCell.Value = OldValue;
                    //                return;
                    //            }
                    //            //檢查長度
                    //            var valueArray = parseDecimal.ToString().Split('.');
                    //            if (azaModel.aza10 < valueArray[0].Length)    //判斷整數
                    //            {
                    //                WfShowErrorMsg(string.Format("整數位數僅可輸入{0}個字元,目前{1}!", azaModel.aza10.ToString(),
                    //                                                            valueArray[0].Length));
                    //                e.Cancel = true;
                    //                IsItemchkValid = false;
                    //                uGridCell.Value = OldValue;
                    //                return;
                    //            }
                    //            if (valueArray.Length == 2)
                    //            {
                    //                if (azaModel.aza11 < valueArray[1].Length)    //判斷小數位數
                    //                {
                    //                    WfShowErrorMsg(string.Format("小數位數僅可輸入{0}個字元,目前{1}!", azaModel.aza11.ToString(),
                    //                                                                valueArray[1].Length));
                    //                    e.Cancel = true;
                    //                    IsItemchkValid = false;
                    //                    uGridCell.Value = OldValue;
                    //                    return;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

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
        
        #region UGridDetail_AfterEnterEditMode
        private void UGridMaster_AfterEnterEditMode(object sender, EventArgs e)
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

        #region UGridMaster_BeforeRowActivate 進入grid 資料列,主要做為控制前端cell readonly使用
        private void UGridMaster_BeforeRowActivate(object sender, RowEventArgs e)
        {
            DataRow drOld;
            try
            {
                if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.NA)
                    return;
                //假如進入的資料列是新增狀態也不處理
                var drTemp = WfGetUgridDatarow(e.Row);
                if (drTemp.RowState==DataRowState.Added)
                {
                    DrMaster = drTemp;
                    return;
                }
                
                //不考慮權限 若可以進入到這的 表示是由滑鼠或鍵盤選取資料列
                //可能是由新增資料列移動 狀態要先改為修改
                try
                {
                    FormEditMode = YREditType.修改;
                    IsInCRUDIni = true;
                    drOld = DrMaster;//記錄舊資料列
                    DrMaster = WfGetUgridDatarow(e.Row);

                    if (WfPreInUpdateMode() == false)
                    {
                        if (BoMaster.TRAN != null)
                            WfRollback();               //先rollback,避免資料一直被鎖住
                        return;
                    }

                    if (this.WfToolbarModify() == false)    //這裡狀態才改為修改
                    {
                        this.FormEditMode = YREditType.NA;
                        if (BoMaster.TRAN != null)
                            WfRollback();           //先rollback,避免資料一直被鎖住
                    }
                    if (WfDisplayMode() == false)
                        return;
                    WfAfterfDisplayMode();

                    if (this.ActiveControl != null)
                        WfGetOldValue(this.ActiveControl);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally 
                {
                    IsInCRUDIni = false;

                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
                this.Close();
            }
        }
        #endregion

        #region WfSetDetailDisplayMode 新增修改+無狀態時的明細列可輸入處理,需要每筆資料列微調整時再使用
        //protected virtual void WfSetDetailDisplayMode( UltraGridRow pUgr, DataRow pDr)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion
        /******************* 其它 Control 事件 ***********************/
        #region FrmGridEditBase_FormClosed : Form 關閉事件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmGridEditBase_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.TabMaster.BoBasic != null)
            { this.TabMaster.BoBasic.Dispose(); }

            if (this.TabMaster.DtSource != null)
            {
                this.TabMaster.DtSource.Dispose();
            }
            if (this.BoSecurity != null)
            {
                this.BoSecurity.Dispose();
            }
        }
        #endregion

        #region FrmGridEditBase_FormClosing Form關閉ing事件
        protected virtual void FrmGridEditBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.WfRowChangeCheck(1))
                {
                    e.Cancel = false;
                    InFormClosing = true;
                    return;
                }
                else
                    e.Cancel = true;

            }
            catch (Exception ex)
            {

                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

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
                if (control.Value != OldValue)
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

        //        if (StrMode == YREditType.NA)
        //            return;

        //        colName = control.Tag.ToString();
        //        if (StrMode == YREditType.查詢)//不驗證,但是要把text的值放入 tabel中
        //        {
        //            ctlValue = (control.Text == null || control.Text.Trim() == "") ? null : control.Text;
        //            try
        //            {
        //                control.Value = ctlValue;
        //            }
        //            catch
        //            {
        //            }
        //            if (ctlValue == null)
        //                DRMASTER[colName] = DBNull.Value;
        //            else
        //                DRMASTER[colName] = ctlValue.ToString();

        //            DRMASTER.EndEdit();
        //            return;
        //        }

        //        IsItemchkValid = true;
        //        currentValue = DRMASTER[colName] == null ? "" : DRMASTER[colName].ToString();
        //        ctlValue = control.Value == null ? "" : control.Value.ToString();
        //        if (ctlValue != currentValue)
        //        {
        //            WfSetBllTransaction();
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
        #endregion

        #endregion

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

        protected override internal void UltraCombo_KeyUp(object sender, KeyEventArgs e)
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

        protected override internal void UltraDateTimeEditor_KeyUp(object sender, KeyEventArgs e)
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

        protected override internal void UcCheckBox_KeyUp(object sender, KeyEventArgs e)
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
            int currentVisibleIndex = 0;
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


                        //檢查是否為第一列
                        currentVisibleIndex = cellActive.Row.VisibleIndex;
                        if (currentVisibleIndex == 0)
                            return;
                        uGrid.PerformAction(UltraGridAction.ExitEditMode);  //觸發驗證
                        if (IsItemchkValid == false)
                            return;

                        //改到rowdecative 處理
                        //if (IsChanged==true)
                        //{
                        //    if (WfToolbarSave() == false)
                        //    {
                        //        return;
                        //    }
                        //}

                        if (IsItemchkValid == false)
                            return;

                        if (uGrid.PerformAction(UltraGridAction.AboveCell) == false)    //觸發rowdetactive事件 換列檢查
                            return;

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

                        //檢查是否為最後列
                        currentVisibleIndex = cellActive.Row.VisibleIndex;
                        if (currentVisibleIndex == (uGridMaster.Rows.Count - 1))
                            return;
                        uGrid.PerformAction(UltraGridAction.ExitEditMode);  //觸發驗證
                        if (IsItemchkValid == false)
                            return;
                        //改到rowdecative 處理
                        //if (IsChanged == true)
                        //{
                        //    if (WfToolbarSave() == false)
                        //    {
                        //        return;
                        //    }
                        //}

                        if (IsItemchkValid == false)
                            return;
                        if (uGrid.PerformAction(UltraGridAction.BelowCell) == false)    //觸發rowdetactive事件 換列檢查
                            return;
                        uGrid.PerformAction(UltraGridAction.EnterEditMode);
                        break;
                    case (Keys.Enter):
                        uGrid.PerformAction(UltraGridAction.NextCell);
                        uGrid.PerformAction(UltraGridAction.EnterEditMode);
                        break;
                }
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
                    WfShowErrorMsg("WfCommit : 交易 (TRAN) 為 Null，無法確認，請檢核! ");
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
                    WfShowErrorMsg("WfCommit : 交易 (TRAN) 為 Null，請檢核! ");
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

        #region WfSetBllTransaction 以bomaster 註冊transaction至其他 bll
        protected virtual void WfSetBllTransaction()
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

        /**********************  其他function **********************/
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


                return true;
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

                DataRow drActive = WfGetActiveDatarow();
                if (this.IsChanged == true)
                {

                    if (iAskGiveup == 0)
                    {
                        WfShowErrorMsg("資料尚未存檔,請先存檔再離開!");
                        GlobalFn.ofShowDialog("2", "Data not be saved, please save it first!", "Warning");
                        return false;
                    }

                    var result = WfShowConfirmMsg("資料已變更，尚未存檔，確定要離開 ?");

                    if (result == DialogResult.Yes)
                    {
                        this.BindingMaster.CancelEdit();
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

        #region WfReport
        private void WfReport()
        {
            List<adp_tb> adpList;
            try
            {
                if (this.AdoModel.ado09.ToUpper() != "Y")
                    return;
                adpList = BoMaster.OfGetAdpModels(this.StrFormID);
                if (adpList == null || adpList.Count == 0)
                    return;
                WfPrintDataGrid(TabMaster.UGrid, adpList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPrintDataGrid
        private void WfPrintDataGrid(UltraGrid sender, List<adp_tb> pList_adp)
        {
            aza_tb l_aza = null;
            double columnCount = 0;
            const float contentFontSize = 9.5f;

            FontFamily defaultFont = new FontFamily("新細明體");
            const double columnInterval = 0;//欄位間隔 單位公分
            try
            {
                Stimulsoft.Report.StiReport report = new Stimulsoft.Report.StiReport();
                //report.Printed += report_Printed;

                if (AdoModel.ado10 != null && AdoModel.ado10 != "")
                    report.ReportName = AdoModel.ado10;
                else
                    report.ReportName = this.Name;

                report.ScriptLanguage = StiReportLanguageType.CSharp;
                DataView dataView = (DataView)((BindingSource)sender.DataSource).List;

                //Add data to datastore
                report.RegData("view", dataView);

                //Fill dictionary
                report.Dictionary.Synchronize();
                StiPage page = report.Pages.Items[0];
                //page.PaperSize=System.Drawing.Printing.PaperKind
                //page.Width = 40;

                //page.SetFont(new Font(new FontFamily("新細明體"),9f));
                page.Width = Convert.ToDouble(pList_adp.Sum(p => p.adp04));
                columnCount = pList_adp.Count();
                page.Width += (columnCount - 1) * columnInterval;
                if (page.Width <= 19)//先定義最小為A4 大小--保留左右寬度各1公分
                {
                    page.Width = 19;
                    //page.
                }

                //Create PageHeader Band
                StiPageHeaderBand pageHeaderBand = new StiPageHeaderBand();
                pageHeaderBand.Height = 1.5f;
                pageHeaderBand.Name = "PahgeHeaderBand";
                pageHeaderBand.Border.Side = StiBorderSides.Bottom;

                //公司抬頭
                StiText companyText = new StiText(new RectangleD(0, 0, page.Width, 0.7f));
                //companyText.Font = new Font(companyText.Font.FontFamily, 12);
                companyText.Font = new Font(defaultFont, 12);
                companyText.Text.Value = LoginInfo.CompNameA;
                companyText.HorAlignment = StiTextHorAlignment.Center;
                companyText.Name = "CompanyText";
                pageHeaderBand.Components.Add(companyText);

                //報表名稱
                StiText titleText = new StiText(new RectangleD(0, 0.7, page.Width, 0.71f));
                titleText.Font = new Font(defaultFont, 12);
                titleText.Text.Value = "{ReportName}";
                titleText.HorAlignment = StiTextHorAlignment.Center;
                titleText.Name = "titleText";
                pageHeaderBand.Components.Add(titleText);

                //製表日期
                StiText printDateLabelText = new StiText(new RectangleD(0f, 1.1f, 1.9f, 0.4f));
                printDateLabelText.Font = new Font(defaultFont, contentFontSize);
                printDateLabelText.Text.Value = "製表日期：";
                printDateLabelText.HorAlignment = StiTextHorAlignment.Right;
                printDateLabelText.Name = "PrintDateLabelText";
                pageHeaderBand.Components.Add(printDateLabelText);

                StiText printDateText = new StiText(new RectangleD(1.9f, 1.1f, 4f, 0.4f));
                printDateText.Font = new Font(defaultFont, contentFontSize);
                printDateText.Text.Value = "{Time}";
                printDateText.HorAlignment = StiTextHorAlignment.Left;
                //printDateText.AutoWidth = true;
                printDateText.Name = "PrintDateText";
                pageHeaderBand.Components.Add(printDateText);

                //製表人
                StiText printEmpLabelText = new StiText(new RectangleD(page.Width - 3.4f, 0.7f, 1.4f, 0.4f));
                printEmpLabelText.Font = new Font(defaultFont, contentFontSize);
                printEmpLabelText.Text.Value = "製表人：";
                printEmpLabelText.HorAlignment = StiTextHorAlignment.Right;
                printEmpLabelText.Name = "PrintEmpLabelText";
                pageHeaderBand.Components.Add(printEmpLabelText);

                StiText printEmpText = new StiText(new RectangleD(page.Width - 2f, 0.7f, 2f, 0.4f));
                printEmpText.Font = new Font(defaultFont, contentFontSize);
                printEmpText.Text.Value = LoginInfo.UserName;
                printEmpText.HorAlignment = StiTextHorAlignment.Left;
                printEmpText.Name = "PrintEmpText";
                pageHeaderBand.Components.Add(printEmpText);

                //頁碼
                StiText pageNumLabelText = new StiText(new RectangleD(page.Width - 3.4f, 1.1f, 1.4f, 0.4f));
                pageNumLabelText.Font = new Font(defaultFont, contentFontSize);
                pageNumLabelText.Text.Value = "頁碼：";
                pageNumLabelText.HorAlignment = StiTextHorAlignment.Right;
                pageNumLabelText.Name = "PageNumLabelText";
                pageHeaderBand.Components.Add(pageNumLabelText);

                StiText pageNumText = new StiText(new RectangleD(page.Width - 2f, 1.1f, 2f, 0.4f));
                pageNumText.Font = new Font(defaultFont, contentFontSize);
                pageNumText.Text.Value = "{PageNofM}";
                pageNumText.HorAlignment = StiTextHorAlignment.Left;
                pageNumText.Name = "PageNumText";
                pageHeaderBand.Components.Add(pageNumText);

                page.Components.Add(pageHeaderBand);

                //Create HeaderBand
                StiHeaderBand headerBand = new StiHeaderBand();
                headerBand.Height = 0.5f;
                headerBand.Name = "HeaderBand";
                page.Components.Add(headerBand);

                //Create Dataaband
                StiDataBand dataBand = new StiDataBand();
                dataBand.DataSourceName = "view" + dataView.Table.TableName;
                dataBand.Height = 0.5f;
                dataBand.Name = "DataBand";
                page.Components.Add(dataBand);

                //Create texts
                Double pos = 0;
                //Double columnWidth = StiAlignValue.AlignToMinGrid(page.Width / dataView.Table.Columns.Count, 0.1, true);
                int nameIndex = 1;

                foreach (adp_tb l_adp in pList_adp.OrderBy(p => p.adp03))
                {
                    Double columnWidth = StiAlignValue.AlignToMinGrid(Convert.ToDouble(l_adp.adp04), 0.1, true);
                    //Create text on header
                    StiText headerText = new StiText(new RectangleD(pos, 0, columnWidth, 0.5f));
                    //headerText.Text.Value = column.Caption;
                    if (TabMaster.AzaTbList != null)
                    {
                        l_aza = (from o in TabMaster.AzaTbList
                                 where o.aza03.ToLower() == l_adp.adp02.ToLower()
                                 select o).FirstOrDefault();
                        if (l_aza != null && l_aza.aza04 != null)
                        {
                            headerText.Text.Value = l_aza.aza04;
                        }
                        else
                            headerText.Text.Value = l_aza.aza03;
                    }

                    headerText.Font = new Font(defaultFont, contentFontSize);
                    headerText.WordWrap = true;
                    headerText.GrowToHeight = true;
                    headerText.CanGrow = true;
                    headerText.HorAlignment = StiTextHorAlignment.Center;
                    headerText.VertAlignment = StiVertAlignment.Center;
                    headerText.Name = "HeaderText" + nameIndex.ToString();
                    //headerText.Brush = new StiSolidBrush(Color.LightSalmon);
                    //headerText.Border.Side = StiBorderSides.All;
                    headerText.Border.Side = StiBorderSides.Top | StiBorderSides.Bottom;
                    headerBand.Components.Add(headerText);

                    //Create text on Data Band
                    StiText dataText = new StiText(new RectangleD(pos, 0, columnWidth, 0.5f));
                    dataText.Font = new Font(defaultFont, contentFontSize);
                    dataText.Text.Value = "{view" + dataView.Table.TableName + "." + Stimulsoft.Report.CodeDom.StiCodeDomSerializator.ReplaceSymbols(l_aza.aza03) + "}";
                    dataText.Name = "DataText" + nameIndex.ToString();
                    //dataText.Border.Side = StiBorderSides.All;
                    //dataText.Border.Side = StiBorderSides.Top | StiBorderSides.Bottom;

                    //Add highlight
                    //StiCondition condition = new StiCondition();
                    //condition.BackColor = Color.LightBlue;
                    //condition.TextColor = Color.Black;
                    //condition.Expression = "(Line & 1) == 1";
                    //condition.Item = StiFilterItem.Expression;
                    //dataText.Conditions.Add(condition);

                    dataBand.Components.Add(dataText);
                    pos += columnWidth + columnInterval;
                    nameIndex++;
                }

                //Create FooterBand
                StiFooterBand footerBand = new StiFooterBand();
                footerBand.Height = 0.5f;
                footerBand.Name = "FooterBand";
                page.Components.Add(footerBand);

                //Create text on footer
                StiText footerText = new StiText(new RectangleD(0, 0, page.Width, 0.5f));
                footerText.Font = new Font(defaultFont, contentFontSize);
                footerText.Text.Value = "總筆數 - {Count()}";
                //footerText.Text.Value = "Count - {Count()}";
                footerText.HorAlignment = StiTextHorAlignment.Right;
                footerText.VertAlignment = StiVertAlignment.Center;
                footerText.Name = "FooterText";
                //footerText.Brush = new StiSolidBrush(Color.LightGreen);
                footerBand.Components.Add(footerText);

                //Render without progress bar
                report.Render(false);

                //Stimulsoft.Report.ReportControls.StiForm myForm = new Stimulsoft.Report.ReportControls.StiForm(report);
                //myForm.Name = "Form1"; 
                if (IsMdiChild)
                    report.Show(this.MdiParent);
                else
                    report.Show();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfCleanErrorProvider 清除所有的錯誤警告
        protected virtual void WfCleanErrorProvider()
        {
            this.errorProvider.Clear();
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

    }
}
