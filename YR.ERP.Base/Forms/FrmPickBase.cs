/*
  程式名稱: FrmPickBase.cs
  系統代號: 
  作　　者: Allen
  描　　述: 做為共用的PICK視窗,由作業admi650來取用
  異動記錄:
  Date        PG         Memo
  ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using YR.Util;
using Infragistics.Win;
using YR.ERP.Shared;
using YR.ERP.DAL.YRModel;
using Infragistics.Win.UltraWinToolbars;
using YR.ERP.BLL.Model;
using System.Data.SqlClient;

namespace YR.ERP.Base.Forms
{
    public partial class FrmPickBase : FrmBase
    {
        #region Property
        public MessageInfo MsgInfoReturned { get; set; }                    //不同表單之間的物件傳遞
        private int _iMaxPickRow = 1;                                       //可取回的筆數限制
        protected YR.ERP.BLL.MSSQL.AdmBLL BoMaster;                         //由使用者登入公司,取得連線Common Objoct

        protected string StrPickNo;                                         //由azp_tb.azp01 取回查詢資料
        protected string StrSqlBody;                                        //組合好的字串 含 select from inner where 不含order by
        protected vw_admi650 Admi650Model;                                  //azp_tb Model
        protected List<vw_admi650s> Admi650sList;                           //開窗主表 Model
        protected YRQueryType FormQueryMode;                                //辨識目前pick的查詢狀態
        protected DataTable DtMaster;                                       //grid的datasource
        #endregion

        #region 建構子
        public FrmPickBase()
        {
            InitializeComponent();
        }

        public FrmPickBase(string pPickNo, UserInfo pUserInfoModel)
        {
            InitializeComponent();
            this.StrPickNo = pPickNo;
            this.LoginInfo = pUserInfoModel;
        }
        #endregion

        #region FrmPickBase_Load
        private void FrmPickBase_Load(object sender, EventArgs e)
        {
            ToolClickEventArgs eTool;
            ToolBase utb;
            try
            {
                if (MsgInfoReturned == null)
                {
                    WfSetSelectMode(1);
                    return;
                }

                WfIniVar();
                WfIniSqlBody();    //初始化sql字串
                WfIniMasterGrid();
                WfIniToolBarUI();

                utb = UtbmMain.Tools["BtQuery"];
                eTool = new ToolClickEventArgs(utb, null);
                UtbmMain_ToolClick(UtbmMain, eTool);

                if (MsgInfoReturned.IsAutoQuery == true ||
                    (MsgInfoReturned.IsAutoQuery == null && GlobalFn.isNullRet(Admi650Model.azp08, "") == "Y"))
                {
                    this.FormQueryMode = YRQueryType.INQUERY;
                    utb = UtbmMain.Tools["BtOk"];
                    eTool = new ToolClickEventArgs(utb, null);
                    UtbmMain_ToolClick(UtbmMain, eTool);
                }
                else
                    WfDisplayMode();

                this.Shown += FrmPickBase_Shown;

            }
            catch (Exception ex)
            {
                GlobalFn.ofShowDialog(ex.Message);
                this.Close();
            }

        }
        #endregion

        #region FrmBase_Shown
        private void FrmPickBase_Shown(object sender, EventArgs e)
        {
            if (FormQueryMode == YRQueryType.INQUERY)
            {
                WfSetFirstVisibelCellFocus(uGrid_Master);//移至第一個可編輯欄位
            }
        }
        #endregion

        #region  WfIniVar時設定變數
        private void WfIniVar()
        {
            if (BoMaster == null)
            {
                BoMaster = new YR.ERP.BLL.MSSQL.AdmBLL(LoginInfo.CompNo, "", "", "");
            }
            //if (_boAdm == null)
            //    _boAdm = new YR.ERP.BLL.MSSQL.AdmBLL(LoginInfo.CompNo, "", "", "");


            _iMaxPickRow = GlobalFn.isNullRet(MsgInfoReturned.IntMaxRow, 1);
            WfSetSelectMode(_iMaxPickRow);
        }
        #endregion

        #region WfIniSqlBody 初始化sql字串
        protected virtual void WfIniSqlBody()
        {
            StringBuilder sbSql;
            StringBuilder sbSqlSelect, sbSqlFrom, sbSqlInner, sbSqlWhere;
            try
            {
                if (MsgInfoReturned == null)
                    throw new Exception("未實體化msgInfoReturned");

                sbSql = new StringBuilder();
                sbSql.AppendLine(string.Format(" SELECT * FROM vw_admi650 WHERE azp01='{0}' ", StrPickNo));
                var drAzp = BoMaster.OfGetDataRow(sbSql.ToString());
                if (drAzp == null)
                    throw new Exception("查無此pick_no!");

                sbSql = new StringBuilder();
                sbSql.AppendLine(string.Format(" SELECT *  FROM vw_admi650s WHERE azq01='{0}' ", StrPickNo));
                sbSql.AppendLine(string.Format(" ORDER BY azq02 "));
                var dtAzq = BoMaster.OfGetDataTable(sbSql.ToString());
                if (drAzp == null)
                    throw new Exception("查無此pick_no 明細資料!");
                Admi650Model = drAzp.ToItem<vw_admi650>();
                Admi650sList = dtAzq.ToList<vw_admi650s>();

                //設定form名稱
                this.Text = string.Format("{0} {1}", Admi650Model.azp01, Admi650Model.azp02);

                #region 取得select body
                sbSqlSelect = new StringBuilder();
                sbSqlSelect.AppendLine(string.Format("SELECT "));
                if (Admi650Model.azp06 == "Y")
                    sbSqlSelect.AppendLine(string.Format("DISTINCT"));

                if (_iMaxPickRow != 1) //表示多選,增加is_pick_yn
                    sbSqlSelect.AppendLine(string.Format("convert(nvarchar(1),'N') is_pick,"));   //0,1


                //組合select 欄位,最後一欄不加,
                for (int i = 0; i < Admi650sList.Count; i++)
                {
                    var selectField = string.Format(Admi650sList[i].azq03 + "." + Admi650sList[i].azq04);
                    if (i != Admi650sList.Count - 1)
                        selectField += ",";

                    sbSqlSelect.AppendLine(selectField);
                }
                #endregion

                #region 取得from body
                sbSqlFrom = new StringBuilder();
                sbSqlFrom.AppendLine(string.Format("FROM " + Admi650Model.azp03));
                #endregion

                #region 取得join
                sbSqlInner = new StringBuilder();
                //if (GlobalFn.isNullRet(drAzp["azp04"], "") != "")
                if (GlobalFn.isNullRet(Admi650Model.azp04, "") != "")
                {
                    //sbSqlInner.AppendLine(string.Format(GlobalFn.isNullRet(drAzp["azp04"], "")));
                    sbSqlInner.AppendLine(string.Format(GlobalFn.isNullRet(Admi650Model.azp04, "")));
                }
                #endregion

                #region 取得where
                sbSqlWhere = new StringBuilder();

                if (GlobalFn.isNullRet(Admi650Model.azp05, "") != "")
                    sbSqlWhere.AppendLine(string.Format("WHERE " + GlobalFn.isNullRet(Admi650Model.azp05, "")));
                else
                    sbSqlWhere.AppendLine(string.Format("WHERE 1=1 "));
                #endregion

                //組合好不含order by的字串
                StrSqlBody = sbSqlSelect.ToString() + sbSqlFrom.ToString() + sbSqlInner.ToString() + sbSqlWhere.ToString();
                if (MsgInfoReturned.StrWhereAppend != "")
                    StrSqlBody += MsgInfoReturned.StrWhereAppend;

                if (MsgInfoReturned.ParamSearchList == null)
                    DtMaster = BoMaster.OfGetDataTable(StrSqlBody + " AND 1<>1 ");
                else
                    DtMaster = BoMaster.OfGetDataTable(StrSqlBody + " AND 1<>1 ", MsgInfoReturned.ParamSearchList.ToArray());
                //修改column 型別為string 並將實際的型別丟到　column.prefix
                foreach (DataColumn dcTempColumn in DtMaster.Columns)
                {
                    if (dcTempColumn.Prefix != "")
                        continue;
                    dcTempColumn.Prefix = dcTempColumn.DataType.Name;
                    dcTempColumn.DataType = typeof(string);
                }
                BindingMaster.DataSource = DtMaster;
                uGrid_Master.DataSource = BindingMaster;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfDisplayMode() : 設定 control 的顯示(操作) 屬性
        protected override Boolean WfDisplayMode()
        {
            try
            {
                this.SuspendLayout();

                WfSetGridReadOnly();

                //工具列顯示
                if (FormQueryMode == YRQueryType.NA)
                {
                    WfShowRibbonGroup(YRQueryType.NA, _iMaxPickRow);
                }
                else if (FormQueryMode == YRQueryType.INQUERY)
                {
                    WfShowRibbonGroup(YRQueryType.INQUERY, _iMaxPickRow);

                    uGrid_Master.Focus();
                    WfSetFirstVisibelCellFocus(uGrid_Master);//移至第一個可編輯欄位
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.ResumeLayout();
            }
            return true;
        }
        #endregion

        /******************* ToolBar 工具列相關 ***********************/

        #region WfIniToolBarUI 初始化表單工具列--設定圖案及熱鍵
        protected virtual void WfIniToolBarUI()
        {
            ImageList ilLarge = new ImageList();
            string lsBtKey;
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

                #region 產生RibbonTab/及Group
                RibbonTab RtData = new RibbonTab("RtData", "資料");
                UtbmMain.Ribbon.Tabs.AddRange(new RibbonTab[] { RtData });

                RibbonGroup RibgCrud = new RibbonGroup("RibgCrud", "資料存取");
                RibbonGroup RibgDecide = new RibbonGroup("RibgDecide", "處理");
                RibbonGroup RibgNav = new RibbonGroup("RibgNav", "導覽");
                RtData.Groups.AddRange(new RibbonGroup[] { RibgCrud, RibgDecide, RibgNav });
                #endregion

                #region RtData/RibgCrud 相關按鈕
                lsBtKey = "BtQuery";
                var BtQuery = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtQuery);
                RibgCrud.Tools.AddTool(lsBtKey);
                RibgCrud.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtQuery.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_QUERY];
                BtQuery.SharedPropsInternal.Shortcut = Shortcut.CtrlQ;
                BtQuery.SharedProps.Caption = "查 詢";
                #endregion

                #region RtData/RibgDecide 相關按鈕
                lsBtKey = "BtOk";
                var BtOk = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtOk);
                RibgDecide.Tools.AddTool(lsBtKey);
                RibgDecide.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtOk.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_OK];
                BtOk.SharedProps.Caption = "確 認";

                lsBtKey = "BtCancel";
                var BtCancel = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtCancel);
                RibgDecide.Tools.AddTool(lsBtKey);
                RibgDecide.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtCancel.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_CANCEL];
                BtCancel.SharedProps.Caption = "取 消";

                lsBtKey = "BtAll";
                var BtAll = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtAll);
                RibgDecide.Tools.AddTool(lsBtKey);
                RibgDecide.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtAll.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_SELECT_ALL];
                BtAll.SharedPropsInternal.Shortcut = Shortcut.CtrlA;
                BtAll.SharedProps.Caption = "全 選";

                lsBtKey = "BtNone";
                var BtNone = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtNone);
                RibgDecide.Tools.AddTool(lsBtKey);
                RibgDecide.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtNone.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_SELECT_NONE];
                //BtNone.SharedPropsInternal.Shortcut = Shortcut.CtrlQ;
                BtNone.SharedProps.Caption = "全不選";
                #endregion

                #region RtData/RibgNav 相關按鈕
                lsBtKey = "BtFirst";
                var BtFirst = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtFirst);
                RibgNav.Tools.AddTool(lsBtKey);
                RibgNav.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtFirst.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_FIRST];
                BtFirst.SharedPropsInternal.Shortcut = Shortcut.CtrlF;
                BtFirst.SharedProps.Caption = "首 筆";

                lsBtKey = "BtPrev";
                var BtPrev = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtPrev);
                RibgNav.Tools.AddTool(lsBtKey);
                RibgNav.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtPrev.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_PREVIOUS];
                BtPrev.SharedPropsInternal.Shortcut = Shortcut.CtrlP;
                BtPrev.SharedProps.Caption = "上一筆";

                lsBtKey = "BtNext";
                var BtNext = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtNext);
                RibgNav.Tools.AddTool(lsBtKey);
                RibgNav.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtNext.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_NEXT];
                BtNext.SharedPropsInternal.Shortcut = Shortcut.CtrlN;
                BtNext.SharedProps.Caption = "下一筆";

                lsBtKey = "BtEnd";
                var BtEnd = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtEnd);
                RibgNav.Tools.AddTool(lsBtKey);
                RibgNav.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtEnd.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_END];
                BtEnd.SharedPropsInternal.Shortcut = Shortcut.CtrlL;
                BtEnd.SharedProps.Caption = "末 筆";
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
                switch (e.Tool.Key.ToLower())
                {
                    case "btfirst":
                        if (FormQueryMode == YRQueryType.NA)
                        {
                            this.uGrid_Master.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.FirstRowInGrid);
                        }
                        break;
                    case "btprev":
                        if (FormQueryMode == YRQueryType.NA)
                        {
                            this.uGrid_Master.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.PrevRow);
                        }
                        break;
                    case "btnext":
                        if (FormQueryMode == YRQueryType.NA)
                        {
                            this.uGrid_Master.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.NextRow);
                        }
                        break;
                    case "btend":
                        if (FormQueryMode == YRQueryType.NA)
                        {
                            this.uGrid_Master.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.LastRowInBand);
                        }
                        break;

                    case "btquery":
                        if (WfQuery() == true)
                        {
                            FormQueryMode = YRQueryType.INQUERY;
                            WfDisplayMode();
                        }
                        break;

                    case "btok":
                        uGrid_Master.UpdateData();
                        if (FormQueryMode == YRQueryType.INQUERY)   //查詢中組合查詢條件
                        {
                            WfQueryOk();
                            FormQueryMode = YRQueryType.NA;
                            WfDisplayMode();
                            break;
                        }
                        if (FormQueryMode == YRQueryType.NA)        //查詢完時
                        {
                            WfReturnOk();
                        }
                        break;

                    case "btcancel":
                        if (FormQueryMode == YRQueryType.INQUERY)
                        {
                            if (MsgInfoReturned.ParamSearchList != null)
                                DtMaster = BoMaster.OfGetDataTable(StrSqlBody + " AND 1<>1 ", MsgInfoReturned.ParamSearchList.ToArray());
                            else
                                DtMaster = BoMaster.OfGetDataTable(StrSqlBody + " AND 1<>1 ");
                            BindingMaster.DataSource = DtMaster;

                            this.FormQueryMode = YRQueryType.NA;
                            WfDisplayMode();
                        }
                        else
                        {
                            MsgInfoReturned.Result = DialogResult.Cancel;
                            this.Close();
                        }
                        break;
                    case "btall":
                        foreach (DataRow ldr in DtMaster.Rows)
                        {
                            ldr["is_pick"] = "Y";
                        }
                        break;
                    case "btnone":
                        foreach (DataRow ldr in DtMaster.Rows)
                        {
                            ldr["is_pick"] = "N";
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
                this.Cursor = Cursors.Default;
            }

        }
        #endregion

        #region WfShowRibbons(YREditType pYRedit) 依表單狀態調整工具列的RibbonGroup
        protected virtual void WfShowRibbonGroup(YRQueryType pYRQueryState, int pMaxRowCount)
        {
            Infragistics.Win.UltraWinToolbars.RibbonTab rbt;
            try
            {
                if (UtbmMain.Ribbon != null && UtbmMain.Ribbon.Tabs.Count > 0)
                    rbt = UtbmMain.Ribbon.Tabs[0];
                else
                    return;

                switch (pYRQueryState)
                {
                    case YRQueryType.NA://無狀態
                        WfShowAllToolRibbons(rbt, true);
                        if (pMaxRowCount == 1)
                        {
                            rbt.Groups["RibgDecide"].Tools["BtAll"].SharedProps.Visible = false;
                            rbt.Groups["RibgDecide"].Tools["BtNone"].SharedProps.Visible = false;
                        }
                        else
                        {
                            rbt.Groups["RibgDecide"].Tools["BtAll"].SharedProps.Visible = true;
                            rbt.Groups["RibgDecide"].Tools["BtNone"].SharedProps.Visible = true;
                        }
                        break;
                    case YRQueryType.INQUERY://處於查詢狀態
                        WfShowAllToolRibbons(rbt, true);
                        rbt.Groups["RibgCrud"].Visible = false;
                        rbt.Groups["RibgNav"].Visible = false;
                        rbt.Groups["RibgDecide"].Tools["BtAll"].SharedProps.Visible = false;
                        rbt.Groups["RibgDecide"].Tools["BtNone"].SharedProps.Visible = false;
                        //if (pMaxRowCount == 1)
                        //{
                        //    rbt.Groups["RibgDecide"].Tools["BtAll"].SharedProps.Visible = false;
                        //    rbt.Groups["RibgDecide"].Tools["BtNone"].SharedProps.Visible = false;
                        //}
                        //else
                        //{
                        //    rbt.Groups["RibgDecide"].Tools["BtAll"].SharedProps.Visible = true;
                        //    rbt.Groups["RibgDecide"].Tools["BtNone"].SharedProps.Visible = true;
                        //}
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfShowAllToolRibbons 依RibbonTab 開關所有其中的RibbonGroup
        protected void WfShowAllToolRibbons(Infragistics.Win.UltraWinToolbars.RibbonTab pUrtb, bool PCanVisible)
        {
            try
            {
                foreach (Infragistics.Win.UltraWinToolbars.RibbonGroup uribg in pUrtb.Groups)
                {
                    uribg.Visible = PCanVisible;
                }

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
            string extendSqlWhere = "";
            try
            {
                var queryInfoList = (from o in Admi650sList
                                     select new QueryInfo
                                     {
                                         TableName = o.azq03,
                                         ColumnDesc = "",
                                         Value = DtMaster.Rows[0][o.azq04],
                                         ColumnName = o.azq04,
                                         ColumnType = DtMaster.Columns[o.azq04].DataType.Name//todo:型別問題還得再考量~~
                                         //ColumnType = DtMaster.Columns[o.azq04].Prefix.ToString()
                                     }
                                    ).ToList()
                      ;
                var SqlParmKeyInList = new List<SqlParameter>();    //使用者輸入的資料
                //extendSqlWhere = WfGetQueryString(queryInfoList, out SqlParmKeyInList);
                extendSqlWhere =BoMaster.WfGetQueryString(queryInfoList, out SqlParmKeyInList);
                var SqlParmTotalList = new List<SqlParameter>();    //使用者輸入+從父視窗傳入的參數
                if (SqlParmKeyInList != null)
                    SqlParmTotalList.AddRange(SqlParmKeyInList);
                if (MsgInfoReturned.ParamSearchList != null)
                    SqlParmTotalList.AddRange(MsgInfoReturned.ParamSearchList);
                DtMaster = BoMaster.OfGetDataTable(StrSqlBody + extendSqlWhere, SqlParmTotalList.ToArray());

                BindingMaster.DataSource = DtMaster;

                //改取sort後的第一筆
                if (DtMaster != null && DtMaster.Rows.Count > 0)
                    uGrid_Master.Rows.GetRowWithListIndex(0).Selected = true;

                if (_iMaxPickRow != 1)
                {
                    uGrid_Master.DisplayLayout.Bands[0].Columns["is_pick"].Hidden = false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfReturnOk() 選擇後按下OK按鈕
        protected virtual bool WfReturnOk()
        {
            DataRow[] returnRowCollection;
            try
            {
                MsgInfoReturned.DataRowList.Clear();
                MsgInfoReturned.Result = DialogResult.OK;
                if (_iMaxPickRow == 1)
                {
                    if (uGrid_Master.ActiveRow != null)
                    {
                        MsgInfoReturned.DataRowList.Add(((uGrid_Master.ActiveRow as UltraGridRow).ListObject as DataRowView).Row);
                    }
                }
                else
                {
                    returnRowCollection = DtMaster.Select(" is_pick ='Y' ");
                    if (returnRowCollection != null)
                        MsgInfoReturned.DataRowList.AddRange(returnRowCollection);
                    if (returnRowCollection.Length == 0)
                        MsgInfoReturned.StrMultiRtn = "";
                    else
                        MsgInfoReturned.StrMultiRtn = WfGetStrMultiRtrn(returnRowCollection, '|');
                }
                this.Close();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfQuery() 按下查詢按鈕時
        protected virtual bool WfQuery()
        {
            DataRow drNew;
            try
            {
                if (MsgInfoReturned.ParamSearchList == null)
                    DtMaster = BoMaster.OfGetDataTable(StrSqlBody + " AND 1<>1 ");
                else
                    DtMaster = BoMaster.OfGetDataTable(StrSqlBody + " AND 1<>1 ", MsgInfoReturned.ParamSearchList.ToArray());
                ////修改column 型別為string 並將實際的型別丟到　column.prefix
                //foreach (DataColumn ldc_temp in DtMaster.Columns)
                //{
                //    if (ldc_temp.Prefix != "")
                //        continue;
                //    ldc_temp.Prefix = ldc_temp.DataType.Name;
                //    ldc_temp.DataType = typeof(string);
                //}
                BindingMaster.DataSource = DtMaster;
                uGrid_Master.DataSource = BindingMaster;

                drNew = DtMaster.NewRow();
                DtMaster.Rows.Add(drNew);

                if (_iMaxPickRow == 1)
                    uGrid_Master.ActiveCell = uGrid_Master.Rows[0].Cells[0];
                else
                {
                    uGrid_Master.DisplayLayout.Bands[0].Columns["is_pick"].Hidden = true;
                    uGrid_Master.ActiveCell = uGrid_Master.Rows[0].Cells[0];
                }

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
        /******************* Grid 相關 ***********************/

        #region WfIniMasterGrid grid 初始化
        private void WfIniMasterGrid()
        {
            try
            {
                this.uGrid_Master.DoubleClick += new System.EventHandler(this.uGrid_Master_DoubleClick);
                WfSetGridHeader(Admi650sList);
                WfSetAppearance(uGrid_Master, 1);   //設定grid樣式
                uGrid_Master.UpdateMode = UpdateMode.OnCellChangeOrLostFocus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetGridHeader 設定欄位名及顯示長度
        private void WfSetGridHeader(List<vw_admi650s> pAdmi650List)
        {
            int iTotWidth = 0;
            int iScreenWidth = 0;
            try
            {
                UltraGrid uGrid = uGrid_Master;
                Infragistics.Win.UltraWinGrid.UltraGridColumn ugc = null;
                List<string> spiltList;
                List<KeyValuePair<string, string>> sourceList;

                #region 取得日期格式
                var boBas = new YR.ERP.BLL.MSSQL.BasBLL(BoMaster.OfGetConntion());
                boBas.TRAN = BoMaster.TRAN;
                this.BaaModel = boBas.OfGetBaaModel();
                if (BaaModel == null || GlobalFn.varIsNull(BaaModel.baa01))
                    this.DateFormat = "yyyy/MM/dd";
                else
                {
                    var baa01KvpList = boBas.OfGetBaa01KVPList();//取得日期格式
                    this.DateFormat = baa01KvpList.Where(x => x.Key == BaaModel.baa01)
                                    .Select(x => x.Value)
                                    .FirstOrDefault()
                                    ;

                    if (GlobalFn.varIsNull(DateFormat))
                        this.DateFormat = "yyyy/MM/dd";
                    else //取得格式會含有 . ex:1.yyyy/MM/dd 要剔除
                    {
                        if (DateFormat.IndexOf('.') >= 0)
                        {
                            var dotPosition = DateFormat.IndexOf('.');
                            this.DateFormat = DateFormat.Substring(dotPosition + 1, DateFormat.Length - dotPosition - 1);
                        }
                    }
                }
                #endregion

                if (uGrid.DisplayLayout.Bands[0].Columns.Exists("is_pick"))
                {
                    ugc = uGrid.DisplayLayout.Bands[0].Columns["is_pick"];
                    ugc.Header.Caption = "勾選否";
                    ugc.Width = 70;

                    Infragistics.Win.UltraWinEditors.UltraCheckEditor uCheckeditor = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
                    uCheckeditor.Editor.DataFilter = new YR.Util.Controls.CheckEditorDataFilter();
                    uCheckeditor.Visible = false;
                    uCheckeditor.CheckAlign = ContentAlignment.MiddleCenter;

                    this.Controls.Add(uCheckeditor);
                    ugc.EditorComponent = uCheckeditor;
                    ugc.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                    iTotWidth += ugc.Width;
                }

                //設定抬頭
                if (pAdmi650List != null)
                {
                    foreach (vw_admi650s admi650Model in pAdmi650List)
                    {

                        if (uGrid.DisplayLayout.Bands[0].Columns.Exists(admi650Model.azq04))
                        {
                            ugc = uGrid.DisplayLayout.Bands[0].Columns[admi650Model.azq04];
                            //uGridColumn.Header.Caption = _boAdm.OfGetAtc03(azqModel.azq04);
                            ugc.Header.Caption = admi650Model.azq04_c;
                            //uGridColumn.Header.Caption = BoMaster.OfGetAtc03(admi650Model.azq04);
                            if (GlobalFn.isNullRet(admi650Model.azq08, 0) > 0)
                                ugc.Width = GlobalFn.isNullRet(admi650Model.azq08, 0);
                        }
                    }

                    foreach (vw_admi650s admi650Model in pAdmi650List)
                    {
                        if (uGrid_Master.DisplayLayout.Bands[0].Columns.Exists(admi650Model.azq04))
                        {
                            ugc = uGrid_Master.DisplayLayout.Bands[0].Columns[admi650Model.azq04];
                            iTotWidth += GlobalFn.isNullRet(admi650Model.azq08, 10);

                            switch (admi650Model.azq06)
                            {
                                case "1"://edit
                                    break;
                                case "2"://checkbox
                                    WfSetUgridCheckBox(ugc);
                                    break;
                                case "3"://combox
                                    if (admi650Model.azq07 != null && admi650Model.azq07 != "")
                                    {
                                        spiltList = admi650Model.azq07.Split(new char[] { '|' }).ToList<string>();
                                        sourceList = new List<KeyValuePair<string, string>>();
                                        foreach (string ls_keyvalue in spiltList)
                                        {
                                            List<KeyValuePair<string, string>> lstSoruce = new List<KeyValuePair<string, string>>();

                                            sourceList.Add(new KeyValuePair<string, string>(ls_keyvalue.Split(new char[] { '.' }).ToList<string>()[0],
                                                                                               ls_keyvalue
                                                                            ));
                                        }

                                        WfSetGridValueList(ugc, sourceList);
                                    }
                                    break;
                                case "4"://date                         
                                    ugc.CellAppearance.TextHAlign = HAlign.Center;
                                    ugc.Format = this.DateFormat;
                                    ugc.MaskInput = this.DateFormat.ToLower();
                                    //uGridColumn.Format = "yyyy/MM/dd";  //時間預設顯示格式至日期
                                    //uGridColumn.MaskInput = "yyyy/mm/dd";
                                    ugc.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                                    break;
                            }
                        }
                    }
                }

                //依總長度設定視窗
                iTotWidth += 40;//保留距離
                iScreenWidth = SystemInformation.PrimaryMonitorSize.Width;
                if (iTotWidth > iScreenWidth)
                {
                    this.Width = iTotWidth - 50;
                }
                else if (iTotWidth < 500)
                    this.Width = 500;
                else
                    this.Width = iTotWidth;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #endregion

        #region uGrid_Master_DoubleClick 單選時,雙擊滑鼠會自動取回資料
        private void uGrid_Master_DoubleClick(object sender, EventArgs e)
        {
            ToolClickEventArgs eTool;
            ToolBase utb;
            try
            {
                if (MsgInfoReturned.IntMaxRow != 1 || FormQueryMode != YRQueryType.NA)
                    return;

                //Cast the sender into an UltraGrid
                UltraGrid grid = sender as UltraGrid;

                //Get the last element that the mouse entered
                UIElement lastElementEntered = grid.DisplayLayout.UIElement.LastElementEntered;

                //See if there's a RowUIElement in the chain.
                RowUIElement rowElement;
                if (lastElementEntered is RowUIElement)
                    rowElement = (RowUIElement)lastElementEntered;
                else
                {
                    rowElement = (RowUIElement)lastElementEntered.GetAncestor(typeof(RowUIElement));
                }

                if (rowElement == null) return;

                //Try to get a row from the element
                UltraGridRow row = (UltraGridRow)rowElement.GetContext(typeof(UltraGridRow));

                //If no row was returned, then the mouse is not over a row.
                if (row == null)
                    return;

                //The mouse is over a row.

                //This part is optional, but if the user double-clicks the line
                //between Row selectors, the row is AutoSized by
                //default. In that case, we probably don't want to do
                //the double-click code.

                //Get the current mouse pointer location and convert it
                //to grid coords.
                Point MousePosition = grid.PointToClient(Control.MousePosition);
                //See if the Point is on an AdjustableElement - meaning that
                //the user is clicking the line on the row selector
                if (lastElementEntered.AdjustableElementFromPoint(MousePosition) != null)
                    return;

                //取回資料
                utb = UtbmMain.Tools["BtOk"];
                eTool = new ToolClickEventArgs(utb, null);
                UtbmMain_ToolClick(UtbmMain, eTool);

            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.ToString());
            }
        }
        #endregion

        #region WfSetSelectMode 設定grid是否可多選_傳入最多可選擇筆數
        /// <summary>
        /// 設定grid是否可多選_傳入最多可選擇筆數
        /// </summary>
        /// <param name="iMax">傳入最多可選擇筆數</param>
        protected void WfSetSelectMode(int iMax)
        {
            //if (iMax == 1)
            //    uGrid_Master. = false;
            //else
            //    uGrid_Master.max = true;
        }
        #endregion

        /******************* 其他方法 ***********************/
        #region WfGetStrMultiRtrn
        protected string WfGetStrMultiRtrn(DataRow[] pDrs, char pSeparateSign)
        {
            StringBuilder sbSqlReturn;
            try
            {
                if (pDrs == null) return "";
                if (MsgInfoReturned.StrMultiColumn == null || MsgInfoReturned.StrMultiColumn == "") return "";
                if (!(pDrs[0].Table.Columns.Contains(MsgInfoReturned.StrMultiColumn)))
                    return "";
                sbSqlReturn = new StringBuilder();
                for (int i = 0; i < pDrs.Length; i++)
                {
                    sbSqlReturn.Append(GlobalFn.isNullRet(pDrs[i][MsgInfoReturned.StrMultiColumn], ""));
                    if (i != pDrs.Length - 1)
                        sbSqlReturn.Append(pSeparateSign);
                }
                return sbSqlReturn.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfSetGridReadOnly : 設定明細頁的欄位 layout
        private Boolean WfSetGridReadOnly()
        {
            try
            {
                if (FormQueryMode == YRQueryType.INQUERY)
                {
                    foreach (UltraGridColumn ugc in uGrid_Master.DisplayLayout.Bands[0].Columns)
                    {
                        if (ugc.Key == "is_pick")
                            WfSetControlReadonly(ugc, true);
                        else
                            WfSetControlReadonly(ugc, false);
                    }
                }

                if (FormQueryMode == YRQueryType.NA)
                {
                    foreach (UltraGridColumn ugc in uGrid_Master.DisplayLayout.Bands[0].Columns)
                    {
                        if (ugc.Key == "is_pick")
                            WfSetControlReadonly(ugc, false);
                        else
                            WfSetControlReadonly(ugc, true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        #endregion

        #region ProcessCmdKey 註冊表單熱鍵
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            ToolClickEventArgs eTool;
            ToolBase utb;
            try
            {
                if (keyData == (Keys.Control | Keys.Enter))
                {
                    utb = UtbmMain.Tools["BtOk"];
                    eTool = new ToolClickEventArgs(utb, null);
                    UtbmMain_ToolClick(UtbmMain, eTool);
                    //toolstripbutton_click(BtnOk, null);
                    return true;
                }

                //if (keyData == (Keys.Control | Keys.A))
                //{
                //    if (FormQueryMode == YRQueryType.NA)
                //    {
                //        this.Close();
                //        return true;
                //    }
                //}

                if (keyData == (Keys.Escape))
                {
                    if (FormQueryMode == YRQueryType.NA)
                    {
                        this.Close();
                        return true;
                    }
                    else if (FormQueryMode == YRQueryType.INQUERY)
                    {
                        if (uGrid_Master.ActiveCell != null && uGrid_Master.ActiveCell.IsInEditMode)
                        {
                            return false;   //由grid原生事件處理                            
                        }
                        else
                        {
                            utb = UtbmMain.Tools["BtCancel"];
                            eTool = new ToolClickEventArgs(utb, null);
                            UtbmMain_ToolClick(UtbmMain, eTool);
                            //toolstripbutton_click(BtnOk, null);
                            return true;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                GlobalFn.ofShowDialog(ex.Message);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region enum FormEditMode
        protected enum YRQueryType
        {
            NA = 0,
            INQUERY = 1
        }
        #endregion

    }
}
