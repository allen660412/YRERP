/* 程式名稱: FrmEntryMDBase.cs
   系統代號: 
   作    者: Allen
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using YR.Util;
using System.Data.SqlClient;
using YR.ERP.DAL.YRModel;

namespace YR.ERP.Base.Forms
{
    public partial class FrmEntryMDBase : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        protected int IntCurTabDetail = 0;                 //設定 Detail 停留在那個頁面 
        protected int IntTabDetailCount = 0;               //設定  Detail 有多少個 Tab

        protected List<TabDetailInfo> TabDetailList;      // Keep 事先定義的所有 Detail Tab 的設定

        //protected Infragistics.Win.UltraWinGrid.UltraGrid uGridDetail1 = null;
        //protected Infragistics.Win.UltraWinGrid.UltraGrid uGridDetail2 = null;
        //protected Infragistics.Win.UltraWinGrid.UltraGrid uGridDetail3 = null;
        //protected Infragistics.Win.UltraWinGrid.UltraGrid uGridDetail4 = null;
        //protected Infragistics.Win.UltraWinGrid.UltraGrid uGridDetail5 = null;
        #endregion Property

        #region 建構子
        public FrmEntryMDBase()
        {
            InitializeComponent();
        }
        #endregion

        /**********************  新增/拷貝/存檔/查詢/刪除 相關 Function **********************/
        #region WfIniToolBarUI 初始化表單工具列--設定圖案及熱鍵
        protected override void WfIniToolBarUI()
        {
            ImageList ilLarge;
            string buttonKey;
            try
            {
                base.WfIniToolBarUI();
                ilLarge = GlobalPictuer.LoadToolBarImage();
                if (ilLarge == null)
                    return;
                #region RibData/RibgDetail
                RibbonGroup RibgDetail = new RibbonGroup("RibgDetail", "明細處理");
                UtbmMain.Ribbon.Tabs["RtData"].Groups.Add(RibgDetail);

                buttonKey = "BtInsertDetail";
                var BtInsertDetail = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtInsertDetail);
                RibgDetail.Tools.AddTool(buttonKey);
                //BtInsertDetail.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["insert_detail_32"];
                BtInsertDetail.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_INSERT_DETAIL];
                BtInsertDetail.SharedPropsInternal.Shortcut = Shortcut.CtrlIns;
                BtInsertDetail.SharedProps.Caption = "新增明細";
                RibgDetail.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;

                buttonKey = "BtDelDetail";
                var BtDeleteDetail = new ButtonTool(buttonKey);
                UtbmMain.Tools.Add(BtDeleteDetail);
                RibgDetail.Tools.AddTool(buttonKey);
                //BtDeleteDetail.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["del_detail_32"];
                BtDeleteDetail.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images[GlobalPictuer.TOOLBAR_DEL_DETAIL];
                BtDeleteDetail.SharedPropsInternal.Shortcut = Shortcut.CtrlDel;
                BtDeleteDetail.SharedProps.Caption = "刪除明細";
                RibgDetail.Tools[buttonKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                #endregion

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfInitialForm() : 初始化表單
        /// <summary>
        /// 初始化變數
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfInitialForm()
        {
            try
            {
                //先設定 mater tab
                if (base.WfInitialForm() == false)
                    return false;

                // 設定 detail Tab 屬性
                if (this.WfIniDetail() == false)
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //this.wf_post_initial_form();
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
                foreach (Infragistics.Win.UltraWinTabControl.UltraTab ut in this.uTab_Detail.Tabs)
                {

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

                    iTabsIndex++;
                    if (iTabsIndex <= IntTabDetailCount)
                        ut.Visible = true;
                    else
                        ut.Visible = false;

                }
                #endregion 依 detail tab 數量設定是否可見

                this.IntCurTabDetail = this.uTab_Detail.SelectedTab.Index;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
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

                uGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
                WfSetAppearance(uGrid, 1);
                uGrid.KeyUp += new KeyEventHandler(ultraGrid_KeyUp);
                uGrid.AfterCellActivate += new System.EventHandler(this.UGridDetail_AfterCellActivate);
                uGrid.ClickCellButton += new CellEventHandler(UltraGrid_ClickCellButton);
                uGrid.BeforeExitEditMode += new BeforeExitEditModeEventHandler(UltraGrid_BeforeExitEditMode);
                uGrid.AfterEnterEditMode += new System.EventHandler(UGridDetail_AfterEnterEditMode);
                uGrid.BeforeRowActivate += new RowEventHandler(UGridDetail_BeforeRowActivate);
                uTab_Detail.Tabs[pTabindex].TabPage.Controls.Add(uGrid);
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

        #region WfIniTabDetailInfo() : TabDetail 初始化並設定master 與detail 的資料關聯及detail是否有新增或刪除功能
        /// <summary>
        /// 設定 Detail Tab 與 master 的資料關聯
        /// </summary>
        /// <returns></returns>        
        protected virtual Boolean WfIniTabDetailInfo()
        {
            //不同維護視窗要視情況 override            
            return true;
        }
        #endregion

        #region WfSetDetailDisplayMode 新增修改時的明細列可輸入處理,需要每筆資料列微調整時再使用
        protected virtual void WfSetDetailDisplayMode(int pCurTabDetail, UltraGridRow pUgr, DataRow pDr)
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

        #region WfQueryOk() 查詢後按下OK按鈕
        protected override bool WfQueryOk()
        {
            DataTable dtSource = null;
            string strQueryAll;
            string strQueryApend = "";
            List<SqlParameter> sqlParmList;
            try
            {
                uGridMaster.PerformAction(UltraGridAction.ExitEditMode);
                this.TabMaster.DtSource.EndInit();
                //detail grid
                if (IntCurTabDetail >= 0)
                {
                    TabDetailList[IntCurTabDetail].UGrid.PerformAction(UltraGridAction.ExitEditMode);
                    TabDetailList[IntCurTabDetail].DtSource.EndInit();
                }

                sqlParmList = new List<SqlParameter>();
                //this.StrQueryWhere = WfCombineQuerystring(out sqlParmList);
                //this.StrQueryWhere = BoMaster.WfCombineQuerystring(TabMaster, out sqlParmList);
                this.StrQueryWhere = BoMaster.WfCombineQuerystring(TabMaster,TabDetailList, out sqlParmList);

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
                //strQueryAll = StrQueryWhere + strQueryApend;

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
                    WfRetrieveDetail();
                }
                else
                {
                    for (int i = 0; i < IntTabDetailCount; i++)
                    {
                        TabDetailList[i].DtSource.Clear();
                    }
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

        #region WfSaveCancel() 新增或修改後按下取消按鈕
        protected override void WfSaveCancel()
        {
            try
            {
                //if (BOMASTER.TRAN != null && StrMode == YREditType.修改)//修改時會鎖資料,才需要rollback
                if (BoMaster.TRAN != null)
                    WfRollback();//TODO:先全部都ROLLBACK
                IsInSaveCancle = true;

                this.errorProvider.Clear();
                if (this.IntTabDetailCount > 0)
                {
                    foreach (TabDetailInfo tabDetailinfo in TabDetailList)
                    {
                        // 無資料或唯讀，不刪除
                        if (tabDetailinfo.DtSource == null) continue;

                        tabDetailinfo.UGrid.PerformAction(UltraGridAction.ExitEditMode);
                        tabDetailinfo.DtSource.RejectChanges();
                    }
                }
                BindingMaster.CancelEdit();
                TabMaster.DtSource.RejectChanges();
                uGridMaster.DataBind();
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

        #region WfToolbarQuery :　查詢資料
        protected override Boolean WfToolbarQuery()
        {
            try
            {
                int i = 0;
                base.WfToolbarQuery();

                foreach (TabDetailInfo tabDetail in TabDetailList)
                {
                    tabDetail.DtSource.Clear();
                    //20161129 Allen add:增加grid查詢控管功能
                    if (tabDetail.CanUgridQuery == false)
                        continue;

                    DataTable dt = tabDetail.DtSource.Clone();
                    tabDetail.DtSource = tabDetail.BoBasic.OfSelect(" AND 1<>1");
                    //todo:參數還需要賦值
                    WfSetTabInfoDataSource(i, tabDetail, dt, "", "");

                    foreach (DataColumn ldc_temp in tabDetail.DtSource.Columns)
                    {
                        ldc_temp.Prefix = ldc_temp.DataType.Name;
                        ldc_temp.DataType = System.Type.GetType("System.String");
                    }

                    tabDetail.DtSource.Rows.Add(tabDetail.DtSource.NewRow());//建立一筆空資料列
                    i++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        #endregion

        #region WfToolbarAdvanceQuery :　進階查詢功能
        protected override Boolean WfToolbarAdvanceQuery()
        {
            DataTable dtSource = null;
            string strQueryAll;
            string strQueryApend = "";
            AdvanceQueryInfo advanceQueryInfo = null;
            List<SqlParameter> sqlParmList = null;
            List<AdvanceQueryInfo> advanceQueryList = null;
            try
            {
                if (this.frmQuery == null)
                {
                    advanceQueryInfo = new AdvanceQueryInfo();
                    advanceQueryInfo.Key = AdoModel.ado06;
                    advanceQueryInfo.Result = DialogResult.Cancel;

                    advanceQueryList = new List<AdvanceQueryInfo>();
                    foreach (TabDetailInfo tabInfo in TabDetailList)
                    {
                        //20161129 Allen add:新增控管進階查詢功能
                        if (tabInfo.CanAdvanceQuery == false)
                            continue;

                        var info = new AdvanceQueryInfo();
                        info.Key = tabInfo.ViewTable;
                        info.RelationParams = tabInfo.RelationParams;
                        advanceQueryList.Add(info);
                    }

                    frmQuery = new FrmAdvanceQuery(this.LoginInfo, advanceQueryInfo, advanceQueryList);
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

        #region WfQueryCancel() 查詢後按下取消按鈕
        protected override void WfQueryCancel()
        {
            string tableName, tablePrefix;
            int i;
            DataTable ldt;
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

                //foreach (var tabDetail in TabDetailList)
                //{
                //    tabDetail.DtSource.Clear();
                //}

                i = 0;
                foreach (var tabDetail in TabDetailList)
                {
                    tableName = tabDetail.ViewTable;
                    tablePrefix = tabDetail.ViewTable;

                    if (tabDetail.ViewTable != "")
                    {
                        ldt = tabDetail.BoBasic.OfSelect(" WHERE 1=2 ");
                        this.WfSetTabInfoDataSource(i, tabDetail, ldt, tableName, tablePrefix);
                    }
                    i++;
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
        /*******************  Grid 事件 ***********************/

        #region uGrid_Master_AfterRowActivate override 事件
        /// <summary>
        /// Master Grid Row 換列時，重新查詢 Detail 的資料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void UGrid_Master_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                if (InFormClosing == true)
                    return;

                DrMaster = WfGetActiveDatarow();
                if (FormEditMode == YREditType.NA)   //新增修改交給toolbar處理
                    WfDisplayMode();

                if (DrMaster == null) return;

                if (this.FormEditMode == YREditType.新增 || this.FormEditMode == YREditType.NA)
                {
                    WfRetrieveDetail();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //isInLoadRowDefault = false;
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
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
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
                if (GlobalFn.isNullRet(uGridCell.Value, "") != GlobalFn.isNullRet(OldValue,"")) //20170331 Allen modify:避免null 跟空字串不同
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

        /**********************  新增/查詢/拷貝/存檔/刪除 相關 Function **********************/
        #region UtbmMain_ToolClick
        protected override void UtbmMain_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            try
            {
                DataRow drDetail;
                this.Cursor = Cursors.WaitCursor;
                DrMaster = WfGetActiveDatarow();
                WfCleanBottomMsg();
                WfCleanErrorProvider();
                //base.UtbmMain_ToolClick(sender, e);
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
                            WfRetrieveDetail();
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
                            //20170315 Allen modify:移至後面 為了WfSetDetailDisplayMode 先處理readonly 再來做focus 
                            //WfAfterfDisplayMode();
                            //因為grid的beforerowactive會抓不到因此再這裡加入detail的readonly控制
                            for (int i = 0; i < IntTabDetailCount; i++)
                            {
                                if (TabDetailList[i].UGrid.ActiveRow != null)
                                {
                                    var drDetailActive = WfGetUgridDatarow(TabDetailList[i].UGrid.ActiveRow);
                                    WfSetDetailDisplayMode(i, TabDetailList[i].UGrid.ActiveRow, drDetailActive);
                                }
                            }
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
                            {
                                if (this.uGridMaster.Rows.Count > 0)
                                {
                                    this.uGridMaster.PerformAction(UltraGridAction.FirstRowInGrid);
                                    uGridMaster.ActiveRow = uGridMaster.DisplayLayout.Rows.GetRowAtVisibleIndex(0);       //因為這裡不會將第一列設定activeRow 所以人工處理
                                    DrMaster = WfGetActiveDatarow();
                                    WfRetrieveDetail();
                                }
                                else
                                {
                                    for (int i = 0; i < IntTabDetailCount; i++)
                                    {
                                        TabDetailList[i].DtSource.Clear();
                                    }
                                }
                                WfAfterfDisplayMode();
                            }
                        }
                        break;

                    case "btaction":
                        UtbmMain.Ribbon.SelectedTab = UtbmMain.Ribbon.Tabs[0];
                        (UtbmMain.Ribbon.Tabs[0].Groups["RibgExternal"].Tools["BtAction"] as PopupMenuTool).ShowPopup();
                        break;
                        
                    case "btok":
                        //if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
                        //{
                        //    WfControlValidated(this.ActiveControl);
                        //    //各驗證控製項會將 isItemchkValid設為true
                        //    if (IsItemchkValid == false)
                        //        return;
                        //}

                        //if (FormEditMode == YREditType.查詢)
                        //{
                        //    WfQueryOk();
                        //    WfQueryOkEnd();
                        //}
                        //if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
                        //    if (WfToolbarSave() == false)
                        //        return;

                        //if (WfDisplayMode() == false)
                        //    return;

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
                        //if (FormEditMode == YREditType.查詢)
                        //{
                        //    WfQueryCancel();
                        //    WfRetrieveDetail();
                        //}

                        //if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
                        //{
                        //    if (this.WfRowChangeCheck(1) == true)
                        //    {
                        //        IsInSaveCancle = true;
                        //        //20150105 改為至savecancel 處理
                        //        //if (StrMode == YREditType.修改)
                        //        //{
                        //        //    WfRollback();//解除transaction
                        //        //}
                        //        WfSaveCancel();
                        //        WfRetrieveDetail();
                        //    }
                        //}

                        //if (WfDisplayMode() == false)
                        //    return;

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

                    case "btinsertdetail":
                        if (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改)
                        {
                            WfFireControlValidated(this.ActiveControl);
                            if (IsItemchkValid == false)
                                return;

                            if (WfPreInsertDetailCheck(IntCurTabDetail) == false)
                                return;
                            this.WfToolbarDetailInsert();
                        }
                        break;

                    case "btdeldetail":
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

        #region WfPressSaveCancel 存檔取消時
        protected override bool WfPressToolBarSaveCancel()
        {
            try
            {
                if (this.WfRowChangeCheck(1) == true)
                {
                    IsInSaveCancle = true;
                    WfSaveCancel();
                    if (WfRetrieveDetail() == false)
                        return false;
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
        protected override bool WfPressToolBarQueryCancel()
        {
            try
            {
                WfQueryCancel();
                if (WfRetrieveDetail() == false)
                    return false;

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
        protected override bool WfPreInUpdateMode()
        {
            try
            {
                //進修改模式時,要重新查詢master資料,避免資料dirty
                if (WfRetrieveMaster() == false)
                    return false;

                if (TabMaster.CanUseRowLock == true)
                    if (WfLockMasterRow() == false)
                        return false;

                WfSetBllTransaction();

                if (WfRetrieveDetail() == false)
                    return false;

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

        #region WfToolbarCopy 主表拷貝
        protected override bool WfToolbarCopy()
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

                for (int i = 0; i < IntTabDetailCount; i++)
                {
                    foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                    {
                        this.WfSetDetailDefaultBycopy(i, drDetail);
                        drDetail.SetAdded();
                    }
                }
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

        #region WfSetDetailDefaultBycopy 設定明細default資料列 (因為狀態將拷貝與區分整合在一起,但流程上還是得區分出來)
        protected virtual void WfSetDetailDefaultBycopy(int pTabIndex, DataRow pDrDetail)
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

        #region WfToolbarSave : 工具列存檔 function
        /// <summary>
        /// 工具列存檔 function
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfToolbarSave()
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
            WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
            this.IsChanged = false;
            return true;
        }
        #endregion

        #region WfToolbarDetailInsert() : 新增明細行
        protected virtual void WfToolbarDetailInsert()
        {
            // 先獲取 Grid 組件當前列的 ID
            int activeIndex = -1;
            int iDone = 0;
            DataRow dr = null;
            UltraGrid uGrid = TabDetailList[IntCurTabDetail].UGrid;
            if (uGrid == null)
            {
                WfShowErrorMsg("請選擇要新增明細的頁面!");
                return;
            }

            if (TabDetailList[IntCurTabDetail].IsReadOnly)
            {
                WfShowErrorMsg(string.Format("頁面 [{0}] 只可查詢無法修改!", IntCurTabDetail));
                return;
            }

            if (uGrid.ActiveRow != null)
            {
                activeIndex = uGrid.ActiveRow.Index;
            }
            else
            { activeIndex = -1; }

            if (this.IntCurTabDetail < 0) return;
            if (TabDetailList[this.IntCurTabDetail].DtSource == null) return;
            dr = TabDetailList[this.IntCurTabDetail].DtSource.NewRow();
            TabDetailList[this.IntCurTabDetail].DtSource.Rows.Add(dr);

            if (this.WfSetDetailRowDefault(this.IntCurTabDetail, dr) == false)
                return;

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
            return;
        }
        #endregion

        #region WfToolbarDelete() : 刪除主表記錄 function
        /// <summary>
        /// 刪除主表記錄 function
        /// </summary>
        /// <returns></returns>
        //waitfix:交易及檢查未處理
        protected override Boolean WfToolbarDelete()
        {
            DataTable dtMasterOld;
            int activeRowIndex = -1;
            try
            {
                //更新主檔更明細資料,供後續資料檢查
                if (WfRetrieveMaster() == false)
                    return false;
                if (WfRetrieveDetail() == false)
                    return false;

                if (WfPreDeleteCheck(DrMaster) == false)
                    return false;

                var result = WfShowConfirmMsg("是否確定要刪除資料 ?");

                if (result != DialogResult.Yes)
                    return true;
                //if (WfShowConfirmMsg("是否確定要刪除資料 ?") != 1)
                //    return true;
                //記錄原來的 GridRow index
                if (this.uGridMaster.ActiveRow != null)
                { activeRowIndex = this.uGridMaster.ActiveRow.Index; }
                else
                { activeRowIndex = -1; }

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

                #region 刪除所有 detail tab 的 line
                if (this.IntTabDetailCount > 0)
                {
                    foreach (TabDetailInfo tabinfo in TabDetailList)
                    {
                        // 無資料或唯讀，不刪除
                        if (tabinfo.DtSource == null) continue;
                        if (tabinfo.IsReadOnly == true) continue;

                        tabinfo.DtSource.RejectChanges();
                        foreach (DataRow drd in tabinfo.DtSource.Rows)
                        {
                            drd.Delete();
                        }
                        tabinfo.BoBasic.OfUpdate(BoMaster.TRAN, tabinfo.DtSource);
                    }
                }
                #endregion 刪除所有 detail tab 的 line

                DrMaster.Delete();
                if (!TabMaster.IsReadOnly)      //不是假雙檔時
                {
                    this.TabMaster.BoBasic.OfUpdate(BoMaster.TRAN, DrMaster.Table);
                }

                #region 如果已無定位在下一筆 Row
                if (this.TabMaster.DtSource.Rows.Count != 0)
                {
                    if (uGridMaster.Rows.Count == 1)
                    {
                        uGridMaster.ActiveRow = uGridMaster.Rows[0];
                    }
                    else
                    {
                        if (activeRowIndex < 0)
                        { WfActiveGridNextRow(this.uGridMaster, 9999); }
                        else
                        { WfActiveGridNextRow(this.uGridMaster, activeRowIndex); }
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
        #region old 沒問題可刪除
        //protected virtual bool WfToolbarDetailDelete(int pTabIndex, DataRow pDr, bool pDeleteConfirm)
        //{
        //    UltraGrid uGrid = null;
        //    int activeIndex = 0;
        //    bool isLockMaster = false;//判斷是否需要LOCK
        //    DataTable dtOld = null; //複製要刪除的資料

        //    if (pTabIndex < 0)
        //    { pTabIndex = this.IntCurTabDetail; }

        //    if (pTabIndex < 0) return false;
        //    if (TabDetailList[pTabIndex].DtSource == null) return false;

        //    //20161108:Allen mark 搬到toolbar處理
        //    //if (TabDetailList[pTabIndex].IsReadOnly)
        //    //{
        //    //    WfShowBottomStatusMsg("本頁面只可查詢無法刪除!");
        //    //    return false;
        //    //}

        //    try
        //    {
        //        DataRow dr = null;
        //        uGrid = TabDetailList[pTabIndex].UGrid;
        //        if (pDr == null)
        //        { dr = WfGetUgridDatarow(TabDetailList[pTabIndex].UGrid.ActiveRow); }
        //        else
        //        { dr = pDr; }

        //        if (DrMaster == null || dr == null)
        //        {
        //            WfShowErrorMsg("查無要刪除的資料!!");
        //            return false;
        //        }

        //        // 先獲取 Grid 組件當前列的 ID
        //        activeIndex = TabDetailList[pTabIndex].UGrid.ActiveRow.Index;

        //        if (!(dr.RowState == DataRowState.Modified || dr.RowState == DataRowState.Unchanged))
        //        {
        //            TabDetailList[pTabIndex].DtSource.Rows.Remove(dr);
        //            WfActiveGridNextRow(TabDetailList[pTabIndex].UGrid, activeIndex);
        //            WfSetFirstVisibelCellFocus(uGrid);
        //            return true;
        //        }
        //        else if ((dr.RowState == DataRowState.Modified || dr.RowState == DataRowState.Unchanged)
        //            && pDeleteConfirm == true
        //            )
        //        {
        //            var result = WfShowConfirmMsg("是否確定要刪除資料?");

        //            if (result != DialogResult.Yes)
        //                return false;
        //        }

        //        //複製要刪除的資料
        //        dtOld = dr.Table.Clone();
        //        dtOld.ImportRow(dr);
        //        dtOld.RejectChanges();  //只取未變更前的值,做為後續WfAfterDetailDelete 使用

        //        dr.Delete();
        //        this.TabDetailList[this.IntCurTabDetail].BoBasic.OfUpdate(BoMaster.TRAN, new DataRow[] { dr });
        //        //todo:要考量主表已被Lock的情形
        //        if (BoMaster.TRAN == null)
        //        {
        //            if (WfBeginTran() == false)
        //                return false;
        //        }
        //        WfSetBllTransaction();

        //        this.WfAfterDetailDelete(IntCurTabDetail, dtOld.Rows[0]);

        //        #region 重新選定一筆 Row 並且移至第一個可編輯的資料列
        //        WfActiveGridNextRow(uGrid, activeIndex);
        //        #endregion 重新選定一筆 Row

        //        WfSetFirstVisibelCellFocus(uGrid);
        //        WfCommit();
        //        isLockMaster = true;
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        WfRollback();
        //        throw ex;
        //    }
        //    finally
        //    {
        //        //重新Lock mater
        //        if (TabMaster.CanUseRowLock && isLockMaster == true)
        //        {
        //            WfLockMasterRow();
        //            WfSetBllTransaction();
        //        }
        //    }

        //} 
        #endregion
        #endregion

        #endregion

        #region WfShowRibbons(YREditType pYRedit) 依表單狀態調整工具列的RibbonGroup
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pYRedit"></param>
        /// <param name="pIsCheckSecurity">是否檢查權限</param>
        /// <param name="pAddTb"></param>
        protected override void WfShowRibbonGroup(YREditType pYRedit, bool pIsCheckSecurity, add_tb pAddTb)
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
                            //新增
                            if (TabMaster.CanAddMode == false
                               || (pIsCheckSecurity == true &&
                                   (pAddTb == null || GlobalFn.isNullRet(pAddTb.add03, "").ToUpper() == "N")
                                   )
                               )
                            {
                                rbt.Groups["RibgCrud"].Tools["BtInsert"].SharedProps.Visible = false;
                            }
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
                            if (TabMaster.CanQueryMode == false)
                            {
                                rbt.Groups["RibgCrud"].Tools["BtQuery"].SharedProps.Visible = false;
                            }
                            if (TabMaster.CanAdvancedQueryMode == false)
                            {
                                rbt.Groups["RibgCrud"].Tools["BtAdvanceQuery"].SharedProps.Visible = false;
                            }
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
                            rbt.Groups["RibgDetail"].Visible = false;

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
                        rbt.Groups["RibgDecide"].Visible = true;
                        if (rbt.Groups.Exists("RibgDetail"))
                            rbt.Groups["RibgDetail"].Visible = true;

                        WfShwRibbonDeside();
                        break;
                    case YREditType.修改://修改
                        WfShowAllToolRibbons(rbt, false);
                        rbt.Groups["RibgDecide"].Visible = true;
                        if (rbt.Groups.Exists("RibgExternal"))
                        {
                            if (rbt.Groups["RibgExternal"].Tools.Exists("BtAction"))
                            {
                                rbt.Groups["RibgExternal"].Visible = true;
                                if (rbt.Groups["RibgExternal"].Tools.Exists("BtReport"))
                                    rbt.Groups["RibgExternal"].Tools["BtReport"].SharedProps.Visible = false;
                            }
                        }
                        if (rbt.Groups.Exists("RibgDetail"))
                            rbt.Groups["RibgDetail"].Visible = true;
                        
                        WfShwRibbonDeside();
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

        #region WfShwRibbonDeside 處理單身功能的enable 與diabled 目前仍不是很完整
        protected virtual void WfShwRibbonDeside()
        {
            try
            {
                if (FormEditMode == YREditType.NA || FormEditMode == YREditType.查詢)
                    return;

                //處理可否新增刪除明細功能
                if (TabDetailList == null || TabDetailList[IntCurTabDetail] == null)
                    return;
                var ribgDetail = UtbmMain.Ribbon.Tabs["RtData"].Groups["RibgDetail"];
                foreach (ButtonTool bt in ribgDetail.Tools)
                {
                    bt.SharedProps.Enabled = true;
                    if (bt.Key == "BtInsertDetail" && TabDetailList[IntCurTabDetail].CanAddMode == false)
                        bt.SharedProps.Enabled = false;
                    if (bt.Key == "BtDelDetail" && TabDetailList[IntCurTabDetail].CanDeleteMode == false)
                        bt.SharedProps.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPreDeleteDetailCheck (): 刪除明細前檢查
        protected virtual bool WfPreDeleteDetailCheck(int pCurTabDetail, DataRow pDrDetail)
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
        #endregion

        #region WfSetDetailRowDefault(int pCurTabDetail, DataRow pDr) 設定明細資料列的初始值
        protected virtual bool WfSetDetailRowDefault(int pCurTabDetail, DataRow pDr)
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

        /******************* 其它 Function ***********************/
        #region ProcessCmdKey 註冊表單熱鍵
        //waitfix:還未考慮跳過權限檢查的問題
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                //if (base.ProcessCmdKey(ref msg, keyData) == true)
                //    return true;

                if (keyData == (Keys.Control | Keys.R))
                {
                    ultraSplitter1.Collapsed = !ultraSplitter1.Collapsed;
                }
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
            return base.ProcessCmdKey(ref msg, keyData);
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

        #region WfRowChangeCheck() : 表頭換列時檢查是否有修改尚未存檔
        /// <summary>
        /// 表頭換列時檢查是否有修改尚未存檔 
        /// </summary>
        /// <returns> true :  放棄編輯中資料, 可換列 </returns>
        ///           false : 不可換列
        protected override Boolean WfRowChangeCheck(int iAskGiveup)
        {
            try
            {
                //this.SelectNextControl(this.txt_focus, true, true, false, false);
                //this.txt_focus.Focus();

                DataRow dr = WfGetActiveDatarow();
                //if (ldr != null)
                //{
                //    ldr.EndEdit();
                //}
                if (this.IsChanged == true)
                {

                    if (iAskGiveup == 0)
                    {
                        WfShowErrorMsg("資料尚未存檔,請先存檔再離開!");
                        //GlobalFn.ofShowDialog("2", "Data not be saved, please save it first!", "Warning");
                        return false;
                    }

                    var result = WfShowConfirmMsg("資料已變更，尚未存檔，確定要離開 ? ");

                    //if (WfShowConfirmMsg("資料已變更，尚未存檔，確定要離開 ? ") == 1)
                    if (result == DialogResult.Yes)
                    {
                        this.BindingMaster.CancelEdit();
                        ((DataTable)this.BindingMaster.DataSource).RejectChanges();

                        foreach (TabDetailInfo tabdetailinfo in TabDetailList)
                        {
                            if (tabdetailinfo.DtSource == null) continue;

                            foreach (DataRow drDetail in tabdetailinfo.DtSource.GetErrors())
                            {
                                drDetail.ClearErrors();
                            }
                            tabdetailinfo.DtSource.RejectChanges();
                        }
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

        #region WfCombineQuerystring 組合查詢的字串(送至frmbase處理) 不使用 改搬到 CommonBLL處理
        //protected override string WfCombineQuerystring(out List<SqlParameter> pSqlParmList)
        //{
        //    string rtnQueryString = "";
        //    List<QueryInfo> queryInfoList;
        //    QueryInfo queryInfo;
        //    string tempSring;
        //    StringBuilder sbSqlDetail;
        //    QueryInfo queryModel;
        //    try
        //    {
        //        #region 先取單頭資料
        //        queryInfoList = new List<QueryInfo>();
        //        pSqlParmList = new List<SqlParameter>();
        //        foreach (DataColumn dc in TabMaster.DtSource.Columns)
        //        {
        //            if (GlobalFn.varIsNull(TabMaster.DtSource.Rows[0][dc.ColumnName]))
        //                continue;

        //            queryModel = new QueryInfo();
        //            queryModel.TableName = TabMaster.ViewTable;
        //            queryModel.ColumnName = dc.ColumnName;
        //            queryModel.ColumnType = dc.Prefix.ToString();//改用記在 prefix的型別
        //            queryModel.Value = TabMaster.DtSource.Rows[0][dc.ColumnName];
        //            queryInfoList.Add(queryModel);
        //        }
        //        var tempSqlParmList = new List<SqlParameter>();
        //        rtnQueryString = WfGetQueryString(queryInfoList, out tempSqlParmList);
        //        if (tempSqlParmList != null)
        //            pSqlParmList.AddRange(tempSqlParmList);
        //        #endregion

        //        #region 再取單身資料
        //        for (int i = 0; i < TabDetailList.Count; i++)
        //        {
        //            if (TabDetailList[i].ViewTable != "" && TabDetailList[i].DtSource != null && TabDetailList[i].DtSource.Rows.Count > 0)
        //            {
        //                queryInfoList = new List<QueryInfo>();
        //                foreach (DataColumn dc in TabDetailList[i].DtSource.Columns)
        //                {
        //                    if (GlobalFn.varIsNull(TabDetailList[i].DtSource.Rows[0][dc.ColumnName]))
        //                        continue;

        //                    queryInfo = new QueryInfo();
        //                    queryInfo.TableName = TabDetailList[i].ViewTable;
        //                    queryInfo.ColumnName = dc.ColumnName;
        //                    queryInfo.ColumnType = dc.Prefix.ToString();//改用記在 prefix的型別
        //                    queryInfo.Value = TabDetailList[i].DtSource.Rows[0][dc.ColumnName];
        //                    queryInfoList.Add(queryInfo);
        //                }
        //                //ls_temp = WfGetQueryString(TabDetailList[i].DtSource.Rows[0], queryInfoList, out sqlParmList);
        //                var tempDetailSqlParmList = new List<SqlParameter>();
        //                tempSring = WfGetQueryString(queryInfoList, out tempDetailSqlParmList);
        //                if (tempSring == "")
        //                    continue;

        //                if (tempDetailSqlParmList != null)
        //                    pSqlParmList.AddRange(tempDetailSqlParmList);

        //                if (TabDetailList[i].RelationParams.Count == 0)
        //                    throw new Exception("tabdetaillist未設定與主表關聯性!(WfCombineQuerystring)");
        //                sbSqlDetail = new StringBuilder();
        //                sbSqlDetail.AppendLine(string.Format("AND EXISTS ("));
        //                sbSqlDetail.AppendLine(string.Format("SELECT 1 FROM {0}", TabDetailList[i].ViewTable));
        //                sbSqlDetail.AppendLine(string.Format("WHERE 1=1 "));
        //                foreach (SqlParameter lp_temp in TabDetailList[i].RelationParams)
        //                {
        //                    sbSqlDetail.AppendLine(string.Format("AND {0}.{1} ={2}.{3} ", TabMaster.ViewTable, lp_temp.SourceColumn, TabDetailList[i].ViewTable, lp_temp.ParameterName));
        //                }
        //                sbSqlDetail.AppendLine(tempSring);
        //                sbSqlDetail.AppendLine(string.Format(")"));
        //                rtnQueryString += sbSqlDetail.ToString();

        //            }

        //        }
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return rtnQueryString;
        //}
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

        #region uTab_Detail_SelectedTabChanged
        private void uTab_Detail_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            try
            {
                this.IntCurTabDetail = e.Tab.Index;
                WfShwRibbonDeside();
            }
            catch (Exception ex)
            {
                var errMsg = new StringBuilder();
                errMsg.AppendLine("uTab_Detail_SelectedTabChanged Eror");
                errMsg.AppendLine(string.Format(ex.Message));
                WfShowErrorMsg(errMsg.ToString());
            }

        }
        #endregion

        #region WfCleanErrorProvider 清除所有的錯誤警告
        protected override void WfCleanErrorProvider()
        {
            this.errorProvider.Clear();
            for (int i = 0; i < IntTabDetailCount; i++)
            {
                foreach (DataRow drDetail in TabDetailList[i].DtSource.GetErrors())
                {
                    drDetail.ClearErrors();
                }
            }
        }
        #endregion
    }
}
