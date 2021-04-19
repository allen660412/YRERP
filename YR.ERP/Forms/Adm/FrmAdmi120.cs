/* 程式名稱: 群組設定作業
   系統代號: admi120
   作　　者: Allen
   描　　述: 設定權限群組屬底層變形
           1.增加批次存檔功能,單筆新增修改刪除,均不進DB,存檔時才整批存入
           2.刪除時相關節點資料均需要一起刪除

   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.DAL.YRModel;
using YR.ERP.Shared;
using YR.Util;

namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdmi120 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        AdmBLL BoAdm = null;

        //Create a new instance of the DrawFilter class to 
        //Handle drawing the DropHighlight/DropLines
        private UltraTree_DropHightLight_DrawFilter_Class UltraTree_DropHightLight_DrawFilter = new UltraTree_DropHightLight_DrawFilter_Class();

        private bool IsDataChange = false;  //記錄表單是否有做資料的異動
        #endregion

        #region 建構子
        public FrmAdmi120()
        {
            InitializeComponent();
        }
        #endregion

        #region FrmAdmi120_Load
        private void FrmAdmi120_Load(object sender, EventArgs e)
        {
            //InitializeDataSet();
            InitializeUltraTree();
            TabMaster.DtSource.Rows.Add(TabMaster.DtSource.NewRow());
            WfQueryOk();//直接查詢
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);

            BoAdm = new AdmBLL(BoMaster.OfGetConntion());
            return;
        }
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfSetVar()
        {
            // 繼承的表單要覆寫, 更改參數值            
            /*
            this.strFormID = "XXX";
            this.isDirectEdit = false;
            this.isMultiRowEdit = false;
             */
            this.StrFormID = "admi120";
            IntTabCount = 2;
            IntMasterGridPos = 2;
            uTab_Master.Tabs[0].Text = "資料內容";
            uTab_Master.Tabs[1].Text = "資料瀏覽";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("ade01", SqlDbType.NVarChar) });

                TabMaster.CanQueryMode = false;
                TabMaster.CanCopyMode = false;
                TabMaster.CanUseRowLock = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            try
            {
                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                    uTree.Focus();
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false);    //先全開
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯

                    //WfSetControlReadonly(new List<Control> { ute_sca05_c, ute_sca21_c, ute_sca24_c }, true);
                    //WfSetControlReadonly(new List<Control> { ute_sca25_c, ute_sca26_c, ute_sca27_c }, true);

                    //if (StrMode == YREditType.修改)
                    //{
                    //    WfSetControlReadonly(ute_sca01, true);
                    //}

                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAfterfDisplayMode  新增修改刪除查詢後的 focus調整
        protected override void WfAfterfDisplayMode()
        {
            try
            {
                uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                SelectNextControl(this.uTab_Master, true, true, true, false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMasterRowDefault 設定主表新增時的預設值,並新增樹狀節點
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            UltraTreeNode addNode;
            try
            {
                if (uTree.Nodes.Count == 0 || uTree.ActiveNode == null)
                {
                    addNode = uTree.Nodes.Add();
                }
                else if (uTree.ActiveNode == null)
                {
                    addNode = uTree.Nodes[0].Nodes.Add();
                }
                else
                {
                    if (uTree.ActiveNode.Expanded == false)
                        uTree.ActiveNode.Expanded = true;
                    addNode = uTree.ActiveNode.Nodes.Add();
                }
                addNode.Key = "";
                addNode.Text = "";

                //新增時 Tree 所在節點也需要加入
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            try
            {
                vw_admi120 admi120Model;
                List<vw_admi120> admi120List;
                int iChkCnts = 0;

                admi120Model = DrMaster.ToItem<vw_admi120>();
                admi120List = DrMaster.Table.ToList<vw_admi120>();
                #region 單頭-pick vw_admi120
                if (e.Row.Table.Prefix.ToLower() == "vw_admi120")
                {
                    switch (e.Column.ToLower())
                    {
                        case "ade01"://群組代號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("群組代號不可空白!");
                                return false;
                            }
                            //檢查群組代號是否重覆
                            iChkCnts = admi120List.Where(p => p.ade01 == e.Value.ToString())
                                                .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("群組代號重覆!");
                                return false;
                            }

                            uTree.ActiveNode.Key = admi120Model.ade01;
                            uTree.ActiveNode.Text = admi120Model.ade01 + "-" + admi120Model.ade02;
                            break;

                        case "ade02"://群組名稱
                            uTree.ActiveNode.Text = admi120Model.ade01 + "-" + admi120Model.ade02;

                            break;
                    }
                }
                #endregion
                IsDataChange = true;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAfterFormCheck() 存檔後處理,通常為放入Pk
        protected override bool WfAfterFormCheck()
        {
            try
            {
                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["adesecu"] = LoginInfo.UserNo;
                        DrMaster["adesecg"] = LoginInfo.GroupNo;
                        DrMaster["adecreu"] = LoginInfo.UserNo;
                        DrMaster["adecreg"] = LoginInfo.DeptNo;
                        DrMaster["adecred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["ademodu"] = LoginInfo.UserNo;
                        DrMaster["ademodg"] = LoginInfo.DeptNo;
                        DrMaster["ademodd"] = Now;
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

        #region WfDeleteAppenUpdate 刪除時使用,若需單身資料,要先在查此查詢資料庫並且異動
        //刪除,除作用節點外的子節點資料
        protected override bool WfDeleteAppenUpdate(DataRow pDr)
        {
            List<DataRow> deleteDrList;
            UltraTreeNode activeNode;
            try
            {
                activeNode = uTree.ActiveNode;
                deleteDrList = new List<DataRow>();

                if (!activeNode.HasNodes)
                    return true;

                foreach (UltraTreeNode tempNode in activeNode.Nodes)
                {
                    WfDeleteTreeRecursive(ref deleteDrList, tempNode);
                }

                foreach (DataRow dr in deleteDrList)
                {
                    TabMaster.DtSource.Rows.Remove(dr);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void WfDeleteTreeRecursive(ref List<DataRow> deleteDrList, UltraTreeNode currentNode)
        {
            DataRow[] drNodeCollection;
            try
            {
                drNodeCollection = TabMaster.DtSource.Select(string.Format("ade01='{0}'", currentNode.Key));
                if (drNodeCollection != null)
                    deleteDrList.Add(drNodeCollection[0]);

                if (!currentNode.HasNodes)
                    return;
                foreach (UltraTreeNode tempNode in currentNode.Nodes)
                {
                    WfDeleteTreeRecursive(ref deleteDrList, tempNode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 底層變形override區
        #region WfIniToolBarUI 初始化表單工具列--設定圖案及熱鍵
        protected override void WfIniToolBarUI()
        {
            ImageList ilLarge = new ImageList();
            string lsBtKey;
            try
            {
                base.WfIniToolBarUI();
                ilLarge = GlobalPictuer.LoadToolBarImage();
                if (ilLarge == null)
                    return;
                RibbonGroup RibgCrud = UtbmMain.Ribbon.Tabs[0].Groups["RibgCrud"];
                //RibbonGroup RibgCrud = new RibbonGroup("RibgCrud", "資料存取");

                lsBtKey = "BtBatchSave";
                var BtInsert = new ButtonTool(lsBtKey);
                UtbmMain.Tools.Add(BtInsert);
                RibgCrud.Tools.AddTool(lsBtKey);
                RibgCrud.Tools[lsBtKey].InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                BtInsert.SharedProps.AppearancesLarge.Appearance.Image = ilLarge.Images["save_32"];
                //BtInsert.SharedPropsInternal.Shortcut = Shortcut.CtrlI;
                BtInsert.SharedProps.Caption = "存檔";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMasterDatasource(DataTable dt) : 設定 bindingMaster 的 datasouce
        /// <summary>
        /// 設定 bindingMaster 的 datasouce 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        protected override Boolean WfSetMasterDatasource(DataTable dt)
        {
            this.TabMaster.DtSource = dt;
            this.BindingMaster.DataSource = dt;
            this.uGridMaster.DataSource = BindingMaster;

            //dt.DataSet.Relations.Add("relation", dt.Columns["ade01"], dt.Columns["parents_ade01"], false);
            //uTree.SetDataBinding(bindingMaster, "relation");
            WfLoadTree(dt);
            if (uTree.Nodes.Count > 0)
                uTree.Nodes[0].ExpandAll();

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

        #region WfPreInUpdateMode
        protected override bool WfPreInUpdateMode()
        {
            try
            {
                //進修改模式時,要重新查詢master資料,避免資料dirty
                //不重查了,因為此為批次處理,以畫面資料為主
                //if (WfRetrieveMaster() == false)
                //    return false;

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

        #region UtbmMain_ToolClick 工具列 click 事件
        protected override void UtbmMain_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            try
            {
                base.UtbmMain_ToolClick(sender, e);
                this.Cursor = Cursors.WaitCursor;
                //DRMASTER = WfGetActiveDatarow();
                //WfCleanBottomMsg();
                //WfCleanErrorProvider();
                switch (e.Tool.Key.ToLower())
                {
                    case "btbatchsave":
                        if (FormEditMode == YREditType.NA && (TabMaster.CanAddMode == true || TabMaster.CanUpdateMode == true))
                        {
                            if (WfBatchSave() == true)
                                WfShowBottomStatusMsg("存檔成功!");
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
                //IsInSaveCancle = false;
                //IsInCRUDIni = false;
                this.Cursor = Cursors.Default;
            }

        }
        #endregion

        #region WfToolbarSave() : 主表存檔 function
        /// <summary>
        /// 主表存檔 function
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfToolbarSave()
        {
            uGridMaster.PerformAction(UltraGridAction.ExitEditMode);
            uGridMaster.UpdateData();
            BindingMaster.EndEdit();
            //tabMaster.dtSource.EndInit();
            //if (this.isDisplayOnly)
            //{ return true; }

            //this.SelectNextControl(this.ActiveControl, true, true, true, true);
            //DataRow dr1 = wf_get_active_datarow();

            //if (this.isMultiRowEdit == false && DRMASTER == null)
            //{
            //    Global_Fn.of_err_msg("2", "查無要存檔的資料!!", "錯誤");
            //    return false;
            //}

            if (DrMaster == null)
            {
                WfShowErrorMsg("查無要存檔的資料!");
                return false;
            }

            try
            {
                //if (this.wf_pre_toolbar_save() == false)
                //    return false;

                //this.Cursor = Cursors.WaitCursor; //改到工具列處理
                //wf_set_bottom_message("存檔中...");

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

                //不做資料庫存檔處理

                //if (this.TabMaster.DtSource.GetChanges() != null)
                //{
                //    if (this.TabMaster.BoBasic.OfUpdate(BOMASTER.TRAN, this.TabMaster.DtSource.GetChanges()) < 0)
                //    {
                //        { throw new Exception("儲存主表時失敗(boBasic.of_update)，請檢核 !"); }
                //    }
                //}

                if (!this.WfAppendUpdate())
                    throw (new Exception("同步資料時發生錯誤(wf_append_update)，請檢核 !"));

                if (this.WfCommit() == false)
                { throw new Exception("確認 (commit) 交易時發生錯誤，請檢核 !"); }
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
            this.FormEditMode = YREditType.NA;
            WfShowRibbonGroup(FormEditMode, TabMaster.IsCheckSecurity, TabMaster.AddTbModel);
            WfShowBottomStatusMsg("存檔成功!");
            this.IsChanged = false;
            UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
            return true;
        }
        #endregion 刪除主表記錄 function

        #region WfSaveCancel() 新增或修改後按下取消按鈕
        protected override void WfSaveCancel()
        {
            vw_admi120 admi120Model;
            UltraTreeNode parentsNode;
            try
            {
                //if (BOMASTER.TRAN != null && StrMode == YREditType.修改)//修改時會鎖資料,才需要rollback
                if (BoMaster.TRAN != null)
                    WfRollback();//TODO:先全部都ROLLBACK
                IsInSaveCancle = true;

                this.errorProvider.Clear();
                BindingMaster.CancelEdit();
                //記錄取消後要返回的節點,目前做法仍無法返回,待修正
                parentsNode = uTree.ActiveNode.Parent;

                if (FormEditMode == YREditType.新增)
                {
                    //var temp = uTree.ActiveNode;
                    uTree.ActiveNode.Remove();
                    TabMaster.DtSource.RejectChanges();
                    if (parentsNode != null)
                    {
                        uTree.SelectedNodes.Clear();
                        uTree.ActiveNode = parentsNode;
                        uTree.SelectedNodes.AddRange(new UltraTreeNode[] { parentsNode }, true);
                    }
                    //temp.Remove();
                }
                else if (FormEditMode == YREditType.修改)
                {
                    //admi120Model = DRMASTER.ToItem<vw_admi120>();
                    ////uTree.ActiveNode.Key = admi120Model.ade01;

                    //uTree.ActiveNode.Text = string.Format("{0}-{1}", admi120Model.ade01, admi120Model.ade02);
                    var ade01_old = GlobalFn.isNullRet(DrMaster["ade01", DataRowVersion.Original], "");
                    var ade02_old = GlobalFn.isNullRet(DrMaster["ade02", DataRowVersion.Original], "");
                    TabMaster.DtSource.RejectChanges();
                }
                UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
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

        #region WfToolbarDelete() : 刪除主表記錄 function
        /// <summary>
        /// 刪除主表記錄 function
        /// </summary>
        /// <returns></returns>
        //waitfix:交易及檢查未處理
        protected override Boolean WfToolbarDelete()
        {
            DataTable dtMasterOld;
            UltraTreeNode activeNode;
            try
            {
                if (WfPreDeleteCheck(DrMaster) == false)
                    return false;

                var result = WfShowConfirmMsg("是否確定要刪除資料 ?");

                //if (WfShowConfirmMsg("是否確定要刪除資料 ?") != 1)
                if (result != DialogResult.Yes)
                    return true;
                //記錄原來的 GridRow index
                int li_active_index = -1;
                if (this.uGridMaster.ActiveRow != null)
                { li_active_index = this.uGridMaster.ActiveRow.Index; }
                else
                { li_active_index = -1; }

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
                activeNode = uTree.ActiveNode;
                DrMaster.Delete();
                //資料庫不存檔
                //this.TabMaster.BoBasic.OfUpdate(BOMASTER.TRAN, DRMASTER.Table);

                #region 如果已無定位在下一筆 Row
                //if (this.TabMaster.DtSource.Rows.Count != 0)
                //{
                //    if (UGrid_Master.Rows.Count == 1)
                //    {
                //        UGrid_Master.ActiveRow = UGrid_Master.Rows[0];
                //    }
                //    else
                //    {
                //        if (li_active_index < 0)
                //        { WfActiveGridNextRow(this.UGrid_Master, 9999); }
                //        else
                //        { WfActiveGridNextRow(this.UGrid_Master, li_active_index); }
                //    }
                //}
                #endregion

                TabMaster.DtSource.AcceptChanges();
                WfCommit();
                //刪除節點要在最後一項,會順便重新定位DRMASTER
                activeNode.Remove();
                UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
                IsDataChange = true;
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
                    if (FormEditMode == YREditType.查詢 || FormEditMode == YREditType.修改 || FormEditMode == YREditType.新增)
                    {
                        utb = UtbmMain.Tools["BtOk"];
                        eTool = new ToolClickEventArgs(utb, null);
                        UtbmMain_ToolClick(UtbmMain, eTool);
                        return true;
                        //}
                    }
                }

                if (keyData == (Keys.Escape))
                {
                    if (FormEditMode == YREditType.NA)
                    {
                        if (IsDataChange == true)
                        {
                            var result = WfShowConfirmMsg("資料尚未存檔,請確認是否離開");
                            //if (WfShowConfirmMsg("資料尚未存檔,請確認是否離開") == 2)
                            if (result == DialogResult.No)
                            {
                                return false;//不離開時
                            }
                        }
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

                if (keyData == (Keys.Shift | Keys.Enter))
                {
                    var visbileCnt = uTab_Master.Tabs.VisibleTabsCount;
                    var iCurrentTab = uTab_Master.SelectedTab.VisibleIndex;
                    if (iCurrentTab + 1 == visbileCnt)
                        uTab_Master.SelectedTab = uTab_Master.VisibleTabs[0];
                    else
                        uTab_Master.SelectedTab = uTab_Master.VisibleTabs[iCurrentTab + 1];

                    SelectNextControl(this.uTab_Master, true, true, true, false);
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

        #endregion

        //*****************************表單自訂Fuction****************************************
        #region UltraTree 相關事件
        #region InitializeUltraTree
        private void InitializeUltraTree()
        {
            this.uTree.Font = GetStyleLibrary.FontControlNormal;
            this.uTree.ViewStyle = Infragistics.Win.UltraWinTree.ViewStyle.Standard;
            //this.uTree.ViewStyle = Infragistics.Win.UltraWinTree.ViewStyle.OutlookExpress;
            this.uTree.Override.CellClickAction = Infragistics.Win.UltraWinTree.CellClickAction.SelectNodeOnly;

            this.uTree.AllowDrop = true;
            this.uTree.Override.SelectionType = Infragistics.Win.UltraWinTree.SelectType.Single;

            UltraTree_DropHightLight_DrawFilter.Invalidate += new EventHandler(this.UltraTree_DropHightLight_DrawFilter_Invalidate);
            UltraTree_DropHightLight_DrawFilter.QueryStateAllowedForNode += new UltraTree_DropHightLight_DrawFilter_Class.QueryStateAllowedForNodeEventHandler(this.UltraTree_DropHightLight_DrawFilter_QueryStateAllowedForNode);
            this.uTree.DrawFilter = UltraTree_DropHightLight_DrawFilter;

            //	Set the ShowExpansionIndicator property to 'CheckOnDisplay' so we don't display
            //	an expansion indicator for nodes that have no descendants.
            //_ultraTree1.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnDisplay;
            uTree.Override.ShowExpansionIndicator = Infragistics.Win.UltraWinTree.ShowExpansionIndicator.CheckOnDisplay;

            this.uTree.AfterActivate += uTree_AfterActivate;
            this.uTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.UltraTree1_DragDrop);
            this.uTree.DragLeave += new System.EventHandler(this.UltraTree1_DragLeave);
            this.uTree.DragOver += new System.Windows.Forms.DragEventHandler(this.UltraTree1_DragOver);
            this.uTree.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.uTree_QueryContinueDrag);
            this.uTree.SelectionDragStart += new System.EventHandler(this.UltraTree1_SelectionDragStart);
            this.uTree.BeforeActivate += uTree_BeforeActivate;
        }
        #endregion InitializeUI

        #region WfLoadTree
        private void WfLoadTree(DataTable dtTree)
        {
            List<vw_admi120> admi120List;
            vw_admi120 admi120Model;
            try
            {
                if (dtTree == null)
                    return;
                admi120List = dtTree.ToList<vw_admi120>();

                admi120Model = admi120List.Where(p => p.ade03 == "0").SingleOrDefault();
                if (admi120Model == null)
                    return;

                var addNode = uTree.Nodes.Add(admi120Model.ade01, string.Format("{0}-{1}", admi120Model.ade01, admi120Model.ade02));
                WfLoadTreeRecursive(admi120Model.ade01, admi120List, uTree, addNode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfLoadTreeRecursive
        private void WfLoadTreeRecursive(string pParentsId, List<vw_admi120> pAdmi120List, UltraTree pTree, UltraTreeNode pParentsNode)
        {
            List<vw_admi120> subAdmi120List;
            try
            {
                subAdmi120List = pAdmi120List.Where(p => p.parents_ade01 == pParentsId)
                                            .OrderBy(p => p.ade03)
                                            .ToList();
                if (subAdmi120List == null)
                    return;
                foreach (vw_admi120 admi120Model in subAdmi120List)
                {
                    var addNode = pParentsNode.Nodes.Add(admi120Model.ade01, string.Format("{0}-{1}", admi120Model.ade01, admi120Model.ade02));
                    WfLoadTreeRecursive(admi120Model.ade01, pAdmi120List, uTree, addNode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region uTree_BeforeActivate
        void uTree_BeforeActivate(object sender, CancelableNodeEventArgs e)
        {
            try
            {
                //if (StrMode != YREditType.NA)
                //{
                //    e.Cancel = true;
                //}


            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region uTree_AfterActivate
        void uTree_AfterActivate(object sender, NodeEventArgs e)
        {
            string ade01;
            try
            {
                if (e.TreeNode == null)//有可能連根節點都刪掉了
                    return;

                ade01 = e.TreeNode.Key;
                var position = BindingMaster.Find("ade01", ade01);
                if (position == -1)
                    return;
                BindingMaster.Position = position;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region uTree_QueryContinueDrag
        //Test to see if we want to continue dragging		
        private void uTree_QueryContinueDrag(object sender, System.Windows.Forms.QueryContinueDragEventArgs e)
        {
            //Did the user press escape? 
            if (e.EscapePressed)
            {
                //User pressed escape
                //Cancel the Drag
                e.Action = DragAction.Cancel;
                //Clear the Drop highlight, since we are no longer
                //dragging
                UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
            }
        }
        #endregion

        #region UltraTree1_SelectionDragStart
        //This event will fire when the user attempts to drag
        //a node. 
        private void UltraTree1_SelectionDragStart(object sender, System.EventArgs e)
        {
            if (FormEditMode != YREditType.NA)
                return;

            //Start a DragDrop operation
            uTree.DoDragDrop(uTree.SelectedNodes, DragDropEffects.Move);
        }
        #endregion

        #region UltraTree1_DragDrop
        //The DragDrop event. Here we respond to a Drop on the tree
        private void UltraTree1_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            //A dummy node variable used for various things
            UltraTreeNode aNode;
            //The SelectedNodes which will be dropped
            SelectedNodesCollection SelectedNodes;
            //The Node to Drop On
            Infragistics.Win.UltraWinTree.UltraTreeNode DropNode;
            //An integer used for loops
            int i;

            //Set the DropNode
            DropNode = UltraTree_DropHightLight_DrawFilter.DropHightLightNode;

            //Get the Data and put it into a SelectedNodes collection,
            //then clone it and work with the clone
            //These are the nodes that are being dragged and dropped
            SelectedNodes = (SelectedNodesCollection)e.Data.GetData(typeof(SelectedNodesCollection));
            SelectedNodes = SelectedNodes.Clone() as SelectedNodesCollection;



            //Sort the selected nodes into their visible position. 
            //This is done so that they stay in the same order when
            //they are repositioned. 
            SelectedNodes.SortByPosition();

            //Determine where we are dropping based on the current
            //DropLinePosition of the DrawFilter
            switch (UltraTree_DropHightLight_DrawFilter.DropLinePosition)
            {
                case DropLinePositionEnum.OnNode: //Drop ON the node
                    {
                        //Loop through the SelectedNodes and reposition
                        //them to the node that was dropped on.
                        //Note that the DrawFilter keeps track of what
                        //node the mouse is over, so we can just use
                        //DropHighLightNode as the drop target. 
                        for (i = 0; i <= (SelectedNodes.Count - 1); i++)
                        {
                            aNode = SelectedNodes[i];
                            aNode.Reposition(DropNode.Nodes);
                        }
                        break;
                    }
                case DropLinePositionEnum.BelowNode: //Drop Below the node
                    {
                        for (i = 0; i <= (SelectedNodes.Count - 1); i++)
                        {
                            aNode = SelectedNodes[i];
                            aNode.Reposition(DropNode, Infragistics.Win.UltraWinTree.NodePosition.Next);
                            //Change the DropNode to the node that was
                            //just repositioned so that the next 
                            //added node goes below it. 
                            DropNode = aNode;
                        }
                        break;
                    }
                case DropLinePositionEnum.AboveNode: //New Index should be the same as the Drop
                    {
                        for (i = 0; i <= (SelectedNodes.Count - 1); i++)
                        {
                            aNode = SelectedNodes[i];
                            aNode.Reposition(DropNode, Infragistics.Win.UltraWinTree.NodePosition.Previous);
                        }
                        break;
                    }
            }

            //After the drop is complete, erase the current drop
            //highlight. 
            UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
            IsDataChange = true;
        }
        #endregion

        #region UltraTree1_DragOver
        private void UltraTree1_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            //A dummy node variable used to hold nodes for various 
            //things
            UltraTreeNode aNode;
            //The Point that the mouse cursor is on, in Tree coords. 
            //This event passes X and Y in form coords. 
            System.Drawing.Point PointInTree;

            //Get the position of the mouse in the tree, as opposed
            //to form coords
            PointInTree = uTree.PointToClient(new Point(e.X, e.Y));

            //Get the node the mouse is over.
            aNode = uTree.GetNodeFromPoint(PointInTree);

            //Make sure the mouse is over a node
            if (aNode == null)
            {
                //The Mouse is not over a node
                //Do not allow dropping here
                e.Effect = DragDropEffects.None;
                //Erase any DropHighlight
                UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
                //Exit stage left
                return;
            }

            //	Don't let continent nodes be dropped onto other continent nodes
            if (this.IsContinentNode(aNode) && this.IsContinentNodeSelected(this.uTree))
            {
                if (PointInTree.Y > (aNode.Bounds.Top + 2) &&
                     PointInTree.Y < (aNode.Bounds.Bottom - 2))
                {
                    e.Effect = DragDropEffects.None;
                    UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
                    return;
                }
            }

            //Check to see if we are dropping onto a node who//s
            //parent (grandparent, etc) is selected.
            //This is to prevent the user from dropping a node onto
            //one of it//s own descendents. 
            if (IsAnyParentSelected(aNode))
            {
                //Mouse is over a node whose parent is selected
                //Do not allow the drop
                e.Effect = DragDropEffects.None;
                //Clear the DropHighlight
                UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
                //Exit stage left
                return;
            }

            //If we//ve reached this point, it//s okay to drop on this node
            //Tell the DrawFilter where we are by calling SetDropHighlightNode
            UltraTree_DropHightLight_DrawFilter.SetDropHighlightNode(aNode, PointInTree);
            //Allow Dropping here. 
            e.Effect = DragDropEffects.Move;
        }
        #endregion

        #region IsContinentNode
        //Proc to check whether a node is a continent or not. 
        //This is necessary because we only want continents to be 
        //parent nodes. We don't want countries to contain other
        //countries
        private bool IsContinentNode(UltraTreeNode Node)
        {
            //The Key of the node
            string NodeKey;
            //The beginning of the key
            string[] ParsedNodeKey;

            //Get the key of the node
            NodeKey = Node.Key;

            //Parse it out by colon
            ParsedNodeKey = NodeKey.Split(':');

            //If the beginning of the key says Continent, then 
            //we know it's a continent node. 
            return (ParsedNodeKey[0] == "Continent");
        }
        #endregion

        #region IsContinentNodeSelected
        /// <summary>
        /// Returns whether any of the currently selected nodes in the
        /// specified UltraTree control represent continents.
        /// </summary>
        /// <param name="tree">The UltraTree control to evaluate.</param>
        /// <returns>A boolean indicating whether any continent nodes are selected.</returns>
        private bool IsContinentNodeSelected(UltraTree tree)
        {
            foreach (UltraTreeNode selectedNode in tree.SelectedNodes)
            {
                if (this.IsContinentNode(selectedNode))
                    return true;
            }

            return false;
        }
        #endregion

        #region UltraTree_DropHightLight_DrawFilter_Invalidate
        //Occassionally, the DrawFilter will let us know that the 
        //control needs to be invalidated. 
        private void UltraTree_DropHightLight_DrawFilter_Invalidate(object sender, System.EventArgs e)
        {
            //Any time the drophighlight changes, the control needs 
            //to know that it has to repaint. 
            //It would be more efficient to only invalidate the area
            //that needs it, but this works and is very clean.
            uTree.Invalidate();
        }
        #endregion

        #region UltraTree_DropHightLight_DrawFilter_QueryStateAllowedForNode
        //This event is fired by the DrawFilter to let us determine
        //what kinds of drops we want to allow on any particular node
        private void UltraTree_DropHightLight_DrawFilter_QueryStateAllowedForNode(Object sender, UltraTree_DropHightLight_DrawFilter_Class.QueryStateAllowedForNodeEventArgs e)
        {
            ////	Don't let continent nodes be dropped onto other continent nodes
            //if (this.IsContinentNode(e.Node) && this.IsContinentNodeSelected(this.uTree))
            //{
            //    e.StatesAllowed = DropLinePositionEnum.AboveNode | DropLinePositionEnum.BelowNode;
            //    return;
            //}

            ////Check to see if this is a continent node. 
            //if (!IsContinentNode(e.Node))
            //{
            //    //This is not a continent
            //    //Allow users to drop above or below this node - but not on
            //    //it, because countries don//t have child countries
            //    e.StatesAllowed = DropLinePositionEnum.AboveNode | DropLinePositionEnum.BelowNode;

            //    //Since we can only drop above or below this node, 
            //    //we don//t want a middle section. So we set the 
            //    //sensitivity to half the height of the node
            //    //This means the DrawFilter will respond to the top half
            //    //bottom half of the node, but not the middle. 
            //    UltraTree_DropHightLight_DrawFilter.EdgeSensitivity = e.Node.Bounds.Height / 2;
            //}
            //else
            //{
            //    if (e.Node.Selected)
            //    {
            //        //This is a selected Continent node. 
            //        //Since it is selected, we don't want to allow
            //        //dropping ON this node. But we can allow the
            //        //the user to drop above or below it. 
            //        e.StatesAllowed = DropLinePositionEnum.AboveNode | DropLinePositionEnum.BelowNode;
            //        UltraTree_DropHightLight_DrawFilter.EdgeSensitivity = e.Node.Bounds.Height / 2;
            //    }
            //    else
            //    {
            //        //This is a continent node and is not selected
            //        //We can allow dropping here above, below, or on this
            //        //node. Since the StatesAllow defaults to All, we don't 
            //        //need to change it. 
            //        //We set the EdgeSensitivity to 1/3 so that the 
            //        //Drawfilter will respond at the top, bottom, or 
            //        //middle of the node. 
            //        UltraTree_DropHightLight_DrawFilter.EdgeSensitivity = e.Node.Bounds.Height / 3;
            //    }
            //}

            if (e.Node.Selected)
            {
                //This is a selected Continent node. 
                //Since it is selected, we don't want to allow
                //dropping ON this node. But we can allow the
                //the user to drop above or below it. 
                e.StatesAllowed = DropLinePositionEnum.AboveNode | DropLinePositionEnum.BelowNode;
                UltraTree_DropHightLight_DrawFilter.EdgeSensitivity = e.Node.Bounds.Height / 2;
            }
            else
            {
                //This is a continent node and is not selected
                //We can allow dropping here above, below, or on this
                //node. Since the StatesAllow defaults to All, we don't 
                //need to change it. 
                //We set the EdgeSensitivity to 1/3 so that the 
                //Drawfilter will respond at the top, bottom, or 
                //middle of the node. 
                UltraTree_DropHightLight_DrawFilter.EdgeSensitivity = e.Node.Bounds.Height / 3;
            }
        }
        #endregion

        #region IsAnyParentSelected
        //Walks up the parent chain for a node to determine if any
        //of it's parent nodes are selected
        private bool IsAnyParentSelected(UltraTreeNode Node)
        {
            UltraTreeNode ParentNode;
            bool ReturnValue = false;

            ParentNode = Node.Parent;
            while (ParentNode != null)
            {
                if (ParentNode.Selected)
                {
                    ReturnValue = true;
                    break;
                }
                else
                {
                    ParentNode = ParentNode.Parent;
                }
            }

            return ReturnValue;
        }
        #endregion

        #region UltraTree1_DragLeave
        //Fires when the user drags outside the control. 
        private void UltraTree1_DragLeave(object sender, System.EventArgs e)
        {
            //When the mouse goes outside the control, clear the 
            //drophighlight. 
            //Since the DropHighlight is cleared when the 
            //mouse is not over a node, anyway, 
            //this is probably not needed
            //But, just in case the user goes from a node directly
            //off the control...
            UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                WfBatchSave();
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion

        #region WfBatchSave 相關
        #region WfBatchSave
        private bool WfBatchSave()
        {
            AdmBLL boBatch;
            StringBuilder sbSql;
            DataTable dtAde;
            try
            {
                //if (IsDataChange == false)
                //{
                //    WfShowMsg("未更改任何資料!");
                //    return false;
                //}
                if (BoMaster.TRAN == null || BoMaster.TRAN.Connection==null)
                    WfBeginTran();
                //if (this.WfBeginTran() == false)
                //{ return false; }
                boBatch = new AdmBLL(BoMaster.OfGetConntion());
                boBatch.TRAN = BoMaster.TRAN;
                boBatch.OfCreateDao("ade_tb", "*", "");

                List<ade_tb> returnAdeTbList = new List<ade_tb>();
                if (WfGetDataByTree(ref returnAdeTbList) == false)
                {
                    return false;
                }

                //先刪除所有的資料後,再新增回去
                sbSql = new StringBuilder();
                sbSql.AppendLine("DELETE FROM ade_tb");
                boBatch.OfExecuteNonquery(sbSql.ToString());

                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ade_tb");
                dtAde = boBatch.OfGetDataTable(sbSql.ToString());
                foreach (ade_tb adeTb in returnAdeTbList)
                {
                    var newRow = dtAde.NewRow();
                    newRow["ade01"] = adeTb.ade01;
                    newRow["ade02"] = adeTb.ade02;
                    newRow["ade03"] = adeTb.ade03;
                    newRow["ade04"] = adeTb.ade04;
                    newRow["adecreu"] = adeTb.adecreu;
                    newRow["adecreg"] = adeTb.adecreg;
                    if (adeTb.adecred == null)
                        newRow["adecred"] = DBNull.Value;
                    else
                        newRow["adecred"] = adeTb.adecred;
                    newRow["ademodu"] = adeTb.ademodu;
                    newRow["ademodg"] = adeTb.ademodg;
                    if (adeTb.ademodd == null)
                        newRow["ademodd"] = DBNull.Value;
                    else
                        newRow["ademodd"] = adeTb.ademodd;
                    dtAde.Rows.Add(newRow);
                }

                boBatch.OfUpdate(dtAde);

                WfCommit();
                IsDataChange = false;
                return true;
            }
            catch (Exception ex)
            {
                if (BoMaster.TRAN != null)
                    WfRollback();
                throw ex;
            }
        }
        #endregion

        #region WfGetDataByTree 依dataTree取得資料內容
        private bool WfGetDataByTree(ref List<ade_tb> pReturnAdeTbList)
        {
            List<ade_tb> masterList;
            try
            {
                UltraTreeNode rootNode = uTree.Nodes[0];
                pReturnAdeTbList = new List<ade_tb>() { 
                    new ade_tb(){
                        ade01=rootNode.Key,
                        ade02="",
                        ade03="0",
                        ade04=""
                    }
                };
                //依樹狀節點,整理階層關係
                WfGetDataRecursive(pReturnAdeTbList, rootNode, "0");
                //從主表資料來源,取得其他名稱及備註等資料後寫入rtnList
                masterList = DrMaster.Table.ToList<ade_tb>();
                foreach (ade_tb adeTb in pReturnAdeTbList)
                {
                    var masterModel = masterList.Where(p => p.ade01 == adeTb.ade01).FirstOrDefault();
                    if (masterModel == null)
                    {
                        WfShowErrorMsg("查無主表對應資料來源!");
                        return false;
                    }
                    adeTb.ade02 = masterModel.ade02;
                    adeTb.ade04 = masterModel.ade04;
                    adeTb.adecreu = masterModel.adecreu;
                    adeTb.adecreg = masterModel.adecreg;
                    adeTb.adecred = masterModel.adecred;
                    adeTb.ademodu = masterModel.ademodu;
                    adeTb.ademodg = masterModel.ademodg;
                    adeTb.ademodd = masterModel.ademodd;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void WfGetDataRecursive(List<ade_tb> pRtnAdeList, UltraTreeNode parentsNode, string parentsLevel)
        {
            UltraTreeNode currentNode;
            try
            {
                if (!parentsNode.HasNodes)
                    return;
                for (int i = 0; i < parentsNode.Nodes.Count; i++)
                {
                    currentNode = parentsNode.Nodes[i];
                    var currentAdeTb = new ade_tb();
                    currentAdeTb.ade01 = currentNode.Key;
                    currentAdeTb.ade02 = "";
                    currentAdeTb.ade03 = string.Format("{0}{1}", parentsLevel, WfLevelMapping(i));
                    currentAdeTb.ade04 = "";
                    pRtnAdeList.Add(currentAdeTb);

                    WfGetDataRecursive(pRtnAdeList, currentNode, currentAdeTb.ade03);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfLevelMapping 傳回對應字元 0-Z 目前共計36個字元
        private string WfLevelMapping(int pKey)
        {
            Dictionary<int, char> mappingDic;
            try
            {
                //先做36個
                mappingDic = new Dictionary<int, char>();
                mappingDic.Add(0, '0');
                mappingDic.Add(1, '1');
                mappingDic.Add(2, '2');
                mappingDic.Add(3, '3');
                mappingDic.Add(4, '4');
                mappingDic.Add(5, '5');
                mappingDic.Add(6, '6');
                mappingDic.Add(7, '7');
                mappingDic.Add(8, '8');
                mappingDic.Add(9, '9');
                mappingDic.Add(10, 'A');
                mappingDic.Add(11, 'B');
                mappingDic.Add(12, 'C');
                mappingDic.Add(13, 'D');
                mappingDic.Add(14, 'E');
                mappingDic.Add(15, 'F');
                mappingDic.Add(16, 'G');
                mappingDic.Add(17, 'H');
                mappingDic.Add(18, 'I');
                mappingDic.Add(19, 'J');
                mappingDic.Add(20, 'K');
                mappingDic.Add(21, 'L');
                mappingDic.Add(22, 'M');
                mappingDic.Add(23, 'N');
                mappingDic.Add(24, 'O');
                mappingDic.Add(25, 'P');
                mappingDic.Add(26, 'Q');
                mappingDic.Add(27, 'R');
                mappingDic.Add(28, 'S');
                mappingDic.Add(29, 'T');
                mappingDic.Add(30, 'U');
                mappingDic.Add(31, 'V');
                mappingDic.Add(32, 'W');
                mappingDic.Add(33, 'X');
                mappingDic.Add(34, 'Y');
                mappingDic.Add(35, 'Z');



                return mappingDic[pKey].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #endregion

    }

}
