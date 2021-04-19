/*
  程式名稱: FrmPickUtility.cs
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

namespace YR.ERP.Base.Forms
{
    public partial class FrmPickUtility : FrmBase
    {
        #region Property
        private int iMaxPickRow = 1;                            //可取回的筆數限制

        private YR.ERP.BLL.MSSQL.AdmBLL _boMaster;              //由使用者登入公司,取得連線Common Objoct
        private YR.ERP.BLL.MSSQL.AdmBLL _boAdm;                 //由使用者登入公司,取得連線Adm Objoct   ??為何需要兩個??

        private string _strPickNo;                              //由azp_tb.azp01 取回查詢資料
        private string _strSqlBody;                             //組合好的字串 含 select from inner where 不含order by
        private DataRow _drAzp;                                 //azp_tb 資料列
        private azp_tb _azpModel;                               //azp_tb Model
        private DataTable _dtAzq;                               //azq_tb 資料表
        private List<azq_tb> _azqTbList = new List<azq_tb>();   //開窗主表 Model
        private YRQueryType _formQueryMode;                     //辨識目前pick的查詢狀態
        private DataTable _dtMaster;                            //grid的datasource
        #endregion

        #region 建構子
        public FrmPickUtility()
        {
            InitializeComponent();
        }

        public FrmPickUtility(string pPickNo, UserInfo pUserInfoModel)
        {
            InitializeComponent();
            this._strPickNo = pPickNo;
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
                WfIniVar();
                WfIniSqlBody();    //設定外觀畫面
                WfIniMasterGrid();
                WfIniToolBarUI();

                utb = UtbmMain.Tools["BtQuery"];
                eTool = new ToolClickEventArgs(utb, null);
                UtbmMain_ToolClick(UtbmMain, eTool);

                if (MsgInfoReturned.IsAutoQuery == true ||
                    (MsgInfoReturned.IsAutoQuery == null && GlobalFn.isNullRet(_drAzp["azp08"], "") == "Y"))
                {
                    this._formQueryMode = YRQueryType.INQUERY;
                    utb = UtbmMain.Tools["BtOk"];
                    eTool = new ToolClickEventArgs(utb, null);
                    UtbmMain_ToolClick(UtbmMain, eTool);
                }
                else
                    WfDisplayMode();

                this.Shown += FrmPickUtility_Shown;

            }
            catch (Exception ex)
            {
                GlobalFn.ofShowDialog(ex.Message);
                this.Close();
            }

        }
        #endregion

        #region FrmBase_Shown
        private void FrmPickUtility_Shown(object sender, EventArgs e)
        {
            if (_formQueryMode == YRQueryType.INQUERY)
            {
                WfSetFirstVisibelCellFocus(uGrid_Master);//移至第一個可編輯欄位
            }
        }
        #endregion

        #region  WfIniVar時設定變數
        private void WfIniVar()
        {
            if (_boMaster == null)
            {
                _boMaster = new YR.ERP.BLL.MSSQL.AdmBLL(LoginInfo.CompNo, "", "", "");
            }
            if (_boAdm == null)
                _boAdm = new YR.ERP.BLL.MSSQL.AdmBLL(LoginInfo.CompNo, "", "", "");

            iMaxPickRow = GlobalFn.isNullRet(MsgInfoReturned.IntMaxRow, 1);
            if (MsgInfoReturned == null)
            {
                WfSetSelectMode(1);
                return;
            }
            WfSetSelectMode(iMaxPickRow);
        }
        #endregion

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
                ilLarge = GlobalPictuer.LoadLargeImage();
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
                BtOk.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["ok_32"];
                BtOk.SharedProps.Caption = "確 認";

                lsBtKey = "BtCancel";
                var BtCancel = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtCancel);
                RibgDecide.Tools.AddTool(lsBtKey);
                RibgDecide.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtCancel.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["cancel_32"];
                BtCancel.SharedProps.Caption = "取 消";

                lsBtKey = "BtAll";
                var BtAll = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtAll);
                RibgDecide.Tools.AddTool(lsBtKey);
                RibgDecide.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtAll.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_QUERY];
                BtAll.SharedPropsInternal.Shortcut = Shortcut.CtrlQ;
                BtAll.SharedProps.Caption = "全 選";

                lsBtKey = "BtNone";
                var BtNone = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtNone);
                RibgDecide.Tools.AddTool(lsBtKey);
                RibgDecide.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtNone.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_QUERY];
                BtNone.SharedPropsInternal.Shortcut = Shortcut.CtrlQ;
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

        #region WfIniSqlBody 初始化sql字串
        private void WfIniSqlBody()
        {
            StringBuilder sbSql;
            StringBuilder sbSqlSelect, sbSqlFrom, sbSqlInner, sbSqlWhere;
            //string masterTable;
            //string distinctYn;
            try
            {
                if (MsgInfoReturned == null)
                    throw new Exception("未實體化msgInfoReturned");

                sbSql = new StringBuilder();
                sbSql.AppendLine(string.Format(" SELECT * FROM azp_tb WHERE azp01='{0}' ", _strPickNo));
                _drAzp = _boMaster.OfGetDataRow(sbSql.ToString());
                if (_drAzp == null)
                    throw new Exception("查無此pick_no!");

                sbSql = new StringBuilder();
                sbSql.AppendLine(string.Format(" SELECT *  FROM azq_tb WHERE azq01='{0}' ", _strPickNo));
                sbSql.AppendLine(string.Format(" ORDER BY azq02 "));
                _dtAzq = _boMaster.OfGetDataTable(sbSql.ToString());
                if (_drAzp == null)
                    throw new Exception("查無此pick_no 明細資料!");
                _azpModel = _drAzp.ToItem<azp_tb>();
                _azqTbList = _dtAzq.ToList<azq_tb>();

                //distinctYn = GlobalFn.isNullRet(_drAzp["azp06"], "N");
                //masterTable = GlobalFn.isNullRet(_drAzp["azp03"], "");
                //this.Text = GlobalFn.isNullRet(_drAzp["azp01"], "") + " " + GlobalFn.isNullRet(_drAzp["azp02"], "");
                //設定form名稱
                this.Text = string.Format("{0} {1}", _azpModel.azp01, _azpModel.azp02);

                #region 取得select body
                sbSqlSelect = new StringBuilder();
                sbSqlSelect.AppendLine(string.Format("SELECT "));
                if (_azpModel.azp06 == "Y")
                    sbSqlSelect.AppendLine(string.Format("DISTINCT"));

                if (iMaxPickRow != 1) //表示多選,增加is_pick_yn
                    sbSqlSelect.AppendLine(string.Format("convert(nvarchar(1),'N') is_pick,"));   //0,1


                //組合select 欄位,最後一欄不加,
                for (int i = 0; i < _azqTbList.Count; i++)
                {
                    var selectField = string.Format(_azqTbList[i].azq03 + "." + _azqTbList[i].azq04);
                    if (i != _azqTbList.Count - 1)
                        selectField += ",";

                    sbSqlSelect.AppendLine(selectField);
                }
                #endregion

                #region 取得from body
                sbSqlFrom = new StringBuilder();
                sbSqlFrom.AppendLine(string.Format("FROM " + _azpModel.azp03));
                #endregion

                #region 取得join
                sbSqlInner = new StringBuilder();
                if (GlobalFn.isNullRet(_drAzp["azp04"], "") != "")
                {
                    sbSqlInner.AppendLine(string.Format(GlobalFn.isNullRet(_drAzp["azp04"], "")));
                }
                #endregion

                #region 取得where
                sbSqlWhere = new StringBuilder();

                if (GlobalFn.isNullRet(_drAzp["azp05"], "") != "")
                    sbSqlWhere.AppendLine(string.Format("WHERE " + GlobalFn.isNullRet(_drAzp["azp05"], "")));
                else
                    sbSqlWhere.AppendLine(string.Format("WHERE 1=1 "));
                #endregion

                //組合好不含order by的字串
                _strSqlBody = sbSqlSelect.ToString() + sbSqlFrom.ToString() + sbSqlInner.ToString() + sbSqlWhere.ToString();
                if (MsgInfoReturned.StrWhereAppend != "")
                    _strSqlBody += MsgInfoReturned.StrWhereAppend;

                if (MsgInfoReturned.ParamSearchList == null)
                    _dtMaster = _boMaster.OfGetDataTable(_strSqlBody + " AND 1<>1 ");
                else
                    _dtMaster = _boMaster.OfGetDataTable(_strSqlBody + " AND 1<>1 ", MsgInfoReturned.ParamSearchList.ToArray());
                //修改column 型別為string 並將實際的型別丟到　column.prefix
                foreach (DataColumn dcTempColumn in _dtMaster.Columns)
                {
                    if (dcTempColumn.Prefix != "")
                        continue;
                    dcTempColumn.Prefix = dcTempColumn.DataType.Name;
                    dcTempColumn.DataType = typeof(string);
                }
                BindingMaster.DataSource = _dtMaster;
                uGrid_Master.DataSource = BindingMaster;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfIniMasterGrid grid 初始化
        private void WfIniMasterGrid()
        {
            try
            {
                this.uGrid_Master.DoubleClick += new System.EventHandler(this.uGrid_Master_DoubleClick);
                WfSetGridHeader(_azqTbList);
                WfSetAppearance(uGrid_Master, 1);   //設定grid樣式
                uGrid_Master.UpdateMode = UpdateMode.OnCellChangeOrLostFocus;
            }
            catch (Exception ex)
            {
                throw ex;
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

        #region WfSetGridHeader 設定欄位名及顯示長度
        private void WfSetGridHeader(List<azq_tb> pAzqList)
        {
            int iTotWidth = 0;
            int iScreenWidth = 0;
            try
            {
                UltraGrid uGrid = uGrid_Master;
                Infragistics.Win.UltraWinGrid.UltraGridColumn uGridColumn = null;
                List<string> lst_temp;
                List<KeyValuePair<string, string>> sourceList;

                if (uGrid.DisplayLayout.Bands[0].Columns.Exists("is_pick"))
                {
                    uGridColumn = uGrid.DisplayLayout.Bands[0].Columns["is_pick"];
                    uGridColumn.Header.Caption = "勾選否";
                    uGridColumn.Width = 70;

                    Infragistics.Win.UltraWinEditors.UltraCheckEditor uCheckeditor = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
                    uCheckeditor.Editor.DataFilter = new YR.Util.Controls.CheckEditorDataFilter();
                    uCheckeditor.Visible = false;
                    uCheckeditor.CheckAlign = ContentAlignment.MiddleCenter;

                    this.Controls.Add(uCheckeditor);
                    uGridColumn.EditorComponent = uCheckeditor;
                    uGridColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                    iTotWidth += uGridColumn.Width;
                }

                //設定抬頭
                foreach (azq_tb azqModel in pAzqList)
                {

                    if (uGrid.DisplayLayout.Bands[0].Columns.Exists(azqModel.azq04))
                    {
                        uGridColumn = uGrid.DisplayLayout.Bands[0].Columns[azqModel.azq04];
                        uGridColumn.Header.Caption = _boAdm.OfGetAtc03(azqModel.azq04);
                        //lugc.Header.Caption = lrec.azq05;
                        if (GlobalFn.isNullRet(azqModel.azq08, 0) > 0)
                            uGridColumn.Width = GlobalFn.isNullRet(azqModel.azq08, 0);
                    }
                }

                foreach (azq_tb azqModel in pAzqList)
                {
                    if (uGrid_Master.DisplayLayout.Bands[0].Columns.Exists(azqModel.azq04))
                    {
                        uGridColumn = uGrid_Master.DisplayLayout.Bands[0].Columns[azqModel.azq04];
                        iTotWidth += GlobalFn.isNullRet(azqModel.azq08, 10);

                        switch (azqModel.azq06)
                        {
                            case "1"://edit
                                break;
                            case "2"://checkbox
                                WfSetUgridCheckBox(uGridColumn);
                                break;
                            case "3"://combox
                                if (azqModel.azq07 != null && azqModel.azq07 != "")
                                {
                                    lst_temp = azqModel.azq07.Split(new char[] { '|' }).ToList<string>();
                                    sourceList = new List<KeyValuePair<string, string>>();
                                    foreach (string ls_keyvalue in lst_temp)
                                    {
                                        List<KeyValuePair<string, string>> lstSoruce = new List<KeyValuePair<string, string>>();


                                        sourceList.Add(new KeyValuePair<string, string>(ls_keyvalue.Split(new char[] { '.' }).ToList<string>()[0],
                                                                                           ls_keyvalue
                                                                        ));
                                    }

                                    WfSetGridValueList(uGridColumn, sourceList);
                                }
                                break;
                            case "4"://date                                
                                uGridColumn.Format = "yyyy/MM/dd";  //時間預設顯示格式至日期
                                uGridColumn.MaskInput = "yyyy/mm/dd";
                                uGridColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                                break;
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

        #region UtbmMain_ToolClick 工具列 click 事件
        protected virtual void UtbmMain_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                DataRow drNew;
                DataRow[] returnRowCollection;
                string extendSqlWhere = "";

                WfCleanBottomMsg();
                switch (e.Tool.Key.ToLower())
                {
                    case "btfirst":
                        if (_formQueryMode == YRQueryType.NA)
                        {
                            this.uGrid_Master.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.FirstRowInGrid);
                        }
                        break;
                    case "btprev":
                        if (_formQueryMode == YRQueryType.NA)
                        {
                            this.uGrid_Master.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.PrevRow);
                        }
                        break;
                    case "btnext":
                        if (_formQueryMode == YRQueryType.NA)
                        {
                            this.uGrid_Master.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.NextRow);
                        }
                        break;
                    case "btend":
                        if (_formQueryMode == YRQueryType.NA)
                        {
                            this.uGrid_Master.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.LastRowInBand);
                        }
                        break;

                    case "btquery":
                        if (MsgInfoReturned.ParamSearchList == null)
                            _dtMaster = _boMaster.OfGetDataTable(_strSqlBody + " AND 1<>1 ");
                        else
                            _dtMaster = _boMaster.OfGetDataTable(_strSqlBody + " AND 1<>1 ", MsgInfoReturned.ParamSearchList.ToArray());
                        //修改column 型別為string 並將實際的型別丟到　column.prefix
                        foreach (DataColumn ldc_temp in _dtMaster.Columns)
                        {
                            if (ldc_temp.Prefix != "")
                                continue;
                            ldc_temp.Prefix = ldc_temp.DataType.Name;
                            ldc_temp.DataType = typeof(string);
                            //ldc_temp.DataType = System.Type.GetType("System.String");
                        }
                        BindingMaster.DataSource = _dtMaster;
                        uGrid_Master.DataSource = BindingMaster;


                        drNew = _dtMaster.NewRow();
                        _dtMaster.Rows.Add(drNew);
                        _formQueryMode = YRQueryType.INQUERY;

                        if (iMaxPickRow == 1)
                            uGrid_Master.ActiveCell = uGrid_Master.Rows[0].Cells[0];
                        else
                            uGrid_Master.ActiveCell = uGrid_Master.Rows[0].Cells[1];

                        WfDisplayMode();
                        break;

                    case "btok":
                        if (_formQueryMode == YRQueryType.INQUERY)   //查詢中組合查詢條件
                        {
                            uGrid_Master.UpdateData();
                            extendSqlWhere = WfGetQueryString(_dtMaster.Rows[0], _azqTbList);

                            if (MsgInfoReturned.ParamSearchList == null)
                                _dtMaster = _boMaster.OfGetDataTable(_strSqlBody + extendSqlWhere);
                            else
                                _dtMaster = _boMaster.OfGetDataTable(_strSqlBody + extendSqlWhere, MsgInfoReturned.ParamSearchList.ToArray());

                            BindingMaster.DataSource = _dtMaster;

                            //改取sort後的第一筆
                            if (_dtMaster != null && _dtMaster.Rows.Count > 0)
                                uGrid_Master.Rows.GetRowWithListIndex(0).Selected = true;
                            _formQueryMode = YRQueryType.NA;
                            WfDisplayMode();

                            break;
                        }
                        if (_formQueryMode == YRQueryType.NA)   //查詢完時
                        {
                            uGrid_Master.UpdateData();
                            MsgInfoReturned.DataRowList.Clear();
                            if (iMaxPickRow == 1)
                            {
                                //if (uGrid_Master.Selected.Rows != null)
                                if (uGrid_Master.ActiveRow != null)
                                {
                                    MsgInfoReturned.DataRowList.Add(((uGrid_Master.ActiveRow as UltraGridRow).ListObject as DataRowView).Row);
                                }
                            }
                            else
                            {
                                returnRowCollection = _dtMaster.Select(" is_pick ='Y' ");
                                if (returnRowCollection != null)
                                    MsgInfoReturned.DataRowList.AddRange(returnRowCollection);
                                MsgInfoReturned.StrMultiRtn = WfGetStrMultiRtrn(returnRowCollection);
                            }
                            this.Close();
                        }
                        break;

                    case "btcancel":
                        this.Close();
                        break;
                    case "btall":
                        foreach (DataRow ldr in _dtMaster.Rows)
                        {
                            ldr["is_pick"] = "Y";
                        }
                        break;
                    case "btnone":
                        foreach (DataRow ldr in _dtMaster.Rows)
                        {
                            ldr["is_pick"] = "N";
                        }
                        break;

                }

            }
            catch (Exception ex)
            {
                WfShowMsg(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
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
                if (_formQueryMode == YRQueryType.NA)
                {
                    WfShowRibbonGroup(YRQueryType.NA, iMaxPickRow);
                }
                else if (_formQueryMode == YRQueryType.INQUERY)
                {
                    WfShowRibbonGroup(YRQueryType.INQUERY, iMaxPickRow);

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

        #region  WfGetQueryString(DataRow pdr, List<azq_tb> pAzqTbList)
        protected string WfGetQueryString(DataRow pdr, List<azq_tb> pAzqTbList)
        {
            string returnString = "";
            StringBuilder sbSqlExtendWhere;
            string strColumnType;
            string strOriginalValue = "";
            string strCondition = "", strNewValue = "";
            List<string> strTempList = new List<string>();
            try
            {
                if (pdr == null)
                    throw new Exception("WfGetQueryString 傳入datarow為空值!");

                sbSqlExtendWhere = new StringBuilder();
                foreach (azq_tb azpModel in pAzqTbList)
                {
                    if (!(pdr.Table.Columns.Contains(azpModel.azq04)))//有些是用pu_form_field刷資料的,會有殘留,所以要避掉
                        continue;

                    strColumnType = pdr.Table.Columns[azpModel.azq04].Prefix.ToString();//改用記在 prefix的型別
                    if (strColumnType.ToLower() == "datetime")
                    {
                        if (pdr[azpModel.azq04] == DBNull.Value)
                            strOriginalValue = "";
                        else
                        {
                            //已經都轉為字串了所以不用再轉型
                            strOriginalValue = pdr[azpModel.azq04].ToString();
                        }
                    }
                    else
                        strOriginalValue = GlobalFn.isNullRet(pdr[azpModel.azq04], "");

                    strCondition = ""; strNewValue = ""; strTempList = new List<string>();
                    if (strOriginalValue == "")
                        continue;
                    #region 取得運算式 及截取後的字串
                    if (strColumnType.ToLower() == "datetime")//日期限定用
                    {
                        strCondition = "=";
                        strTempList.Add(strOriginalValue);
                    }
                    else if (strOriginalValue.StartsWith("<>"))
                    {
                        strCondition = "<>";
                        strTempList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
                    }
                    else if (strOriginalValue.StartsWith(">="))
                    {
                        strCondition = ">=";
                        strTempList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
                    }
                    else if (strOriginalValue.StartsWith("<="))
                    {
                        strCondition = "<=";
                        strTempList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
                    }
                    else if (strOriginalValue.StartsWith("<"))
                    {
                        strCondition = "<";
                        strTempList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
                    }
                    else if (strOriginalValue.StartsWith(">"))
                    {
                        strCondition = ">";
                        strTempList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
                    }
                    else if (strOriginalValue.IndexOf('=') >= 0)
                    {
                        strCondition = "=";
                        strTempList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
                    }
                    else if (strOriginalValue.IndexOf("%") >= 0)
                    {
                        strCondition = "LIKE";
                        strTempList.Add(strOriginalValue);
                    }
                    else if ((strOriginalValue.IndexOf(":") >= 0))  //遇到日期格式時會有問題
                    {
                        strCondition = "BETWEEN";
                        strTempList = strOriginalValue.Split(new char[] { ':' }, 2).ToList<string>();
                    }
                    else if ((strOriginalValue.IndexOf("|") >= 0))
                    {
                        strCondition = "IN";
                        strTempList = strOriginalValue.Split(new char[] { '|' }).ToList<string>();
                    }
                    else
                    {
                        strCondition = "=";
                        strTempList.Add(strOriginalValue);
                    }
                    #endregion

                    if (strColumnType.ToLower() == "string")
                    {
                        if (strCondition.ToLower() == "between")
                        {
                            strNewValue = string.Format(" N'{0}' AND N'{1}'", strTempList[0], strTempList[1]);
                        }
                        else if (strCondition.ToLower() == "in")
                        {
                            strNewValue = " (";
                            foreach (string ls_temp in strTempList)
                            {
                                if (strTempList.LastOrDefault<string>() != ls_temp)
                                    strNewValue += string.Format("N'{0}',", ls_temp);
                                else
                                    strNewValue += string.Format("N'{0}'", ls_temp);
                            }
                            strNewValue += ")";
                        }
                        else
                            strNewValue = string.Format("N'{0}'", strTempList[0]);
                        sbSqlExtendWhere.AppendLine(string.Format(" AND {0}.{1} {2} {3} ", azpModel.azq03, azpModel.azq04, strCondition, strNewValue));
                    }
                    //else if (ls_db_type.ToLower() == "numeric" || ls_db_type.ToLower() == "double")
                    else if (strColumnType.ToLower() == "decimal" || strColumnType.ToLower() == "double" || strColumnType.ToLower().IndexOf("int") >= 0)
                    {
                        if (strCondition.ToLower() == "between")
                        {
                            strNewValue = string.Format(" {0} AND {1}", strTempList[0], strTempList[1]);
                        }
                        else if (strCondition.ToLower() == "in")
                        {
                            strNewValue = " (";
                            foreach (string ls_temp in strTempList)
                            {
                                if (strTempList.LastOrDefault<string>() != ls_temp)
                                    strNewValue += string.Format("{0},", ls_temp);
                                else
                                    strNewValue += string.Format("{0}", ls_temp);
                            }
                            strNewValue += ")";
                        }
                        else
                            strNewValue = strTempList[0];
                        sbSqlExtendWhere.AppendLine(string.Format(" AND {0}.{1} {2} {3} ", azpModel.azq03, azpModel.azq04, strCondition, strNewValue));
                    }
                    //else if (ls_db_type.ToLower() == "datetime" || ls_db_type.ToLower() == "date")
                    else if (strColumnType.ToLower() == "datetime")
                    {
                        if (strCondition.ToLower() == "between")
                        {
                            strNewValue = string.Format(" '{0}' AND '{1}'", strTempList[0], strTempList[1]);
                        }
                        else if (strCondition.ToLower() == "in")
                        {
                            strNewValue = " (";
                            foreach (string ls_temp in strTempList)
                            {
                                if (strTempList.LastOrDefault<string>() != ls_temp)
                                    strNewValue += string.Format("'{0}',", ls_temp);
                                else
                                    strNewValue += string.Format("'{0}'", ls_temp);
                            }
                            strNewValue += ")";
                        }
                        else
                        {
                            strNewValue = string.Format(" '{0}' ", strTempList[0]);
                        }
                        sbSqlExtendWhere.AppendLine(string.Format(" AND convert(nvarchar(8),{0}.{1},112) {2} {3} ", azpModel.azq03, azpModel.azq04, strCondition, strNewValue));
                    }
                    else
                        sbSqlExtendWhere.AppendLine();
                }
                returnString = sbSqlExtendWhere.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return returnString;
        }
        #endregion

        #region WfGetStrMultiRtrn
        private string WfGetStrMultiRtrn(DataRow[] pDrs)
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
                        sbSqlReturn.Append("|");
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
                if (_formQueryMode == YRQueryType.INQUERY)
                {
                    foreach (UltraGridColumn ugc in uGrid_Master.DisplayLayout.Bands[0].Columns)
                    {
                        if (ugc.Key == "is_pick")
                            WfSetControlReadonly(ugc, true);
                        else
                            WfSetControlReadonly(ugc, false);
                    }
                }

                if (_formQueryMode == YRQueryType.NA)
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

                if (keyData == (Keys.Q))
                {
                    if (_formQueryMode == YRQueryType.NA)
                    {
                        utb = UtbmMain.Tools["BtQuery"];
                        eTool = new ToolClickEventArgs(utb, null);
                        UtbmMain_ToolClick(UtbmMain, eTool);
                        //toolstripbutton_click(BtnQuery, null);
                        return true;
                    }
                }

                if (keyData == (Keys.Escape))
                {
                    this.Close();
                    return true;
                }

            }
            catch (Exception ex)
            {

                GlobalFn.ofShowDialog(ex.Message);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region uGrid_Master_DoubleClick 單選時,雙擊滑鼠會自動取回資料
        private void uGrid_Master_DoubleClick(object sender, EventArgs e)
        {
            ToolClickEventArgs eTool;
            ToolBase utb;
            try
            {
                if (MsgInfoReturned.IntMaxRow != 1 || _formQueryMode != YRQueryType.NA)
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
                WfShowMsg(ex.ToString());
            }
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
