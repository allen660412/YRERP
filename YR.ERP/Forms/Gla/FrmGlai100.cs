/* 程式名稱: 會計科目資料查詢
   系統代號: admi120
   作　　者: Allen
   描　　述: 設定會計科目底層變形

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

namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlai100 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        GlaBLL BoGla = null;
        //bool _isInDeleteing = false; //避免在刪除模式中,觸發Ultraree的AfterActive事件
        ImageList ImgList = YR.Util.GlobalPictuer.LoadProgramListImage();
        #endregion

        #region 建構子
        public FrmGlai100()
        {
            InitializeComponent();
            InitializeUltraTree();
        }
        #endregion

        #region FrmGlai100_Load
        private void FrmGlai100_Load(object sender, EventArgs e)
        {
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);

            BoGla = new GlaBLL(BoMaster.OfGetConntion());
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
            this.StrFormID = "glai100";
            IntTabCount = 1;
            IntMasterGridPos = 0;
            uTab_Master.Tabs[0].Text = "資料內容";
            //uTab_Master.Tabs[1].Text = "資料瀏覽";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("gba01", SqlDbType.NVarChar) });

                TabMaster.IsAutoQueryFistLoad = true;
                TabMaster.CanQueryMode = false;
                TabMaster.CanAdvancedQueryMode = false;
                //TabMaster.CanCopyMode = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetBllTransaction 以bomaster 註冊transaction至其他 bll
        protected override void WfSetBllTransaction()
        {
            try
            {
                if (BoMaster.TRAN != null)
                {
                    BoGla.TRAN = BoMaster.TRAN;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                //科目性質
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.結轉"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.帳戶"));
                WfSetUcomboxDataSource(ucb_gba03, sourceList);

                //資產/損益
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.資產"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.損益"));
                WfSetUcomboxDataSource(ucb_gba04, sourceList);

                //借/貸
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.借"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.貸"));
                WfSetUcomboxDataSource(ucb_gba05, sourceList);

                //統制明細別
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.統制科目"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.明細科目"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.獨立帳戶"));
                WfSetUcomboxDataSource(ucb_gba06, sourceList);

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
            vw_glai100 masterModel = null;
            try
            {
                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                    uTree.Enabled = true;
                    uTree.Focus();
                }
                else
                {
                    masterModel = DrMaster.ToItem<vw_glai100>();
                    WfSetControlsReadOnlyRecursion(this, false);    //先全開
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    WfSetGba07ReadOnly(masterModel.gba06);
                    uTree.Enabled = false;

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_gba01, true);
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
            //UltraTreeNode addNode;
            try
            {
                //if (uTree.Nodes.Count == 0 || uTree.ActiveNode == null)
                //{
                //    addNode = uTree.Nodes.Add();
                //}
                //else if (uTree.ActiveNode == null)
                //{
                //    addNode = uTree.Nodes[0].Nodes.Add();
                //}
                //else
                //{
                //    if (uTree.ActiveNode.Expanded == false)
                //        uTree.ActiveNode.Expanded = true;
                //    addNode = uTree.ActiveNode.Nodes.Add();
                //}
                //addNode.Key = "";
                //addNode.Text = "";

                ////新增時 Tree 所在節點也需要加入

                pDr["gba08"] = "N";
                pDr["gba09"] = "N";
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
                vw_glai100 masterModel;
                int iChkCnts = 0;

                masterModel = DrMaster.ToItem<vw_glai100>();
                #region 單頭-pick vw_glai100
                if (e.Row.Table.Prefix.ToLower() == "vw_glai100")
                {
                    switch (e.Column.ToLower())
                    {
                        case "gba06":   //統制明細別
                            WfSetGba07ReadOnly(masterModel.gba06);
                            if (masterModel.gba06 == "3")
                            {
                                DrMaster["gba07"] = "";
                            }
                            else
                            {
                                if (GlobalFn.varIsNull(masterModel.gba07))
                                {
                                    DrMaster["gba07"] = "root";
                                }
                            }
                            break;
                        case "gba07":   //所屬統制科目
                            if (GlobalFn.varIsNull(e.Value) || e.Value.ToString().ToLower() == "root")
                                return true;
                            if (GlobalFn.varIsNull(masterModel.gba01))
                            {
                                WfShowErrorMsg("請先輸入會計科目");
                                WfItemChkForceFocus(ute_gba01);
                                return false;
                            }
                            var glai100Model = BoGla.OfGetGbaModel(e.Value.ToString());
                            if (glai100Model == null)
                            {
                                WfShowErrorMsg("無此會科,請檢核!");
                                return false;
                            }
                            if (glai100Model.gba06 != "1")
                            {
                                WfShowErrorMsg("此會科非統制科目,請檢核!");
                                return false;
                            }
                            if (glai100Model.gbavali == "N")
                            {
                                WfShowErrorMsg("此會科非有效科目,請檢核!");
                                return false;
                            }
                            //todo:需加入檢查是否為下階科目
                            if (FormEditMode == YREditType.修改 && e.Value.ToString().ToLower() != "root")
                            {
                                var activeNode = uTree.GetNodeByKey(masterModel.gba01);
                                if (activeNode == null)
                                {
                                    WfShowErrorMsg("查無作用節點!");
                                    return false;
                                }
                                var checkNode = uTree.GetNodeByKey(e.Value.ToString());
                                if (activeNode.IsAncestorOf(checkNode))
                                {
                                    WfShowErrorMsg("所屬統制科目,不可為下階科目!");
                                    return false;
                                }
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

        #region WfPreDeleteCheck 進主檔刪除前檢查
        protected override bool WfPreDeleteCheck(DataRow pDr)
        {
            vw_glai100 masterMode = null;
            try
            {
                masterMode = DrMaster.ToItem<vw_glai100>();
                var delteNode = uTree.GetNodeByKey(masterMode.gba01);
                if (delteNode.Nodes.Count > 0 || (uTree.ActiveNode != null && uTree.ActiveNode.Key.ToLower() == "root"))
                {
                    WfShowErrorMsg("只能刪除尾階的會計科目!");
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

        #region WfAppendUpdate 存檔後處理額外的資料表 新增、修改、刪除...
        protected override bool WfAppendUpdate()
        {
            UltraTreeNode parentsNode = null;
            vw_glai100 masterModel = null;
            string gba06_old = "", gba07_old = "";
            try
            {
                masterModel = DrMaster.ToItem<vw_glai100>();
                //這裡處理新增修改後的樹狀結構
                //refresh父節點下的資料
                if (FormEditMode == YREditType.新增)
                {
                    parentsNode = WfGetParentsNode(masterModel);
                    var addNode = parentsNode.Nodes.Add(masterModel.gba01, string.Concat(masterModel.gba01, "-", masterModel.gba02));
                    uTree.Enabled = true;       //這裡就要先打開,不然會無法設定activenode
                    uTree.ActiveNode = addNode;
                }
                else if (FormEditMode == YREditType.修改)
                {
                    var activeNode = uTree.GetNodeByKey(masterModel.gba01);
                    uTree.Enabled = true;       //這裡就要先打開,不然會無法設定activenode
                    uTree.ActiveNode = activeNode;
                    //檢查是否需要更新節點
                    gba06_old = GlobalFn.isNullRet(DrMaster["gba06", DataRowVersion.Original], "");
                    gba07_old = GlobalFn.isNullRet(DrMaster["gba07", DataRowVersion.Original], "");
                    if (gba06_old != masterModel.gba06 || gba07_old != masterModel.gba07)
                    {
                        if (masterModel.gba06 == "1")    //統制科目
                        {
                            activeNode.Override.ActiveNodeAppearance.Image = ImgList.Images[GlobalPictuer.MENU_TREE_FOLDER_ACTIVE];
                            activeNode.Override.NodeAppearance.Image = ImgList.Images[GlobalPictuer.MENU_TREE_FOLDER];
                        }
                        else
                        {
                            activeNode.Override.ActiveNodeAppearance.Image = null;
                            activeNode.Override.NodeAppearance.Image = null;
                        }
                        if (gba07_old != masterModel.gba07)
                        {
                            var newParentsNode = uTree.GetNodeByKey(masterModel.gba07);
                            activeNode.Reposition(newParentsNode.Nodes);
                            uTree.ActiveNode = activeNode;
                        }
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
        protected override bool WfDeleteAppenUpdate(DataRow pDr)
        {
            vw_glai100 masterModel = null;
            try
            {
                //_isInDeleteing = true;
                masterModel = DrMaster.ToItem<vw_glai100>();
                var parentsNode = WfGetParentsNode(masterModel);
                var deleteNode = uTree.GetNodeByKey(masterModel.gba01);
                parentsNode.Nodes.Remove(deleteNode);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        //*****************************表單自訂Fuction****************************************
        #region WfSetGba07ReadOnly
        private void WfSetGba07ReadOnly(string gba06)
        {
            try
            {
                if (gba06 == "1" || gba06 == "2")
                {
                    WfSetControlReadonly(ute_gab07, false);
                }
                else
                {
                    WfSetControlReadonly(ute_gab07, true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfGetParentsNode
        private UltraTreeNode WfGetParentsNode(vw_glai100 pGlai100Model)
        {
            UltraTreeNode parentsNode = null;
            string nodeKey = "";
            try
            {
                nodeKey = GlobalFn.isNullRet(pGlai100Model.gba07, "root");
                parentsNode = uTree.GetNodeByKey(nodeKey);
                return parentsNode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region UltraTree 相關事件
        #region InitializeUltraTree
        private void InitializeUltraTree()
        {
            this.uTree.Font = GetStyleLibrary.FontControlNormal;
            this.uTree.ViewStyle = Infragistics.Win.UltraWinTree.ViewStyle.Standard;
            this.uTree.Override.CellClickAction = Infragistics.Win.UltraWinTree.CellClickAction.SelectNodeOnly;

            this.uTree.AllowDrop = false;

            this.uTree.Override.SelectionType = Infragistics.Win.UltraWinTree.SelectType.Single;

            //	Set the ShowExpansionIndicator property to 'CheckOnDisplay' so we don't display
            //	an expansion indicator for nodes that have no descendants.
            //_ultraTree1.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnDisplay;
            uTree.Override.ShowExpansionIndicator = Infragistics.Win.UltraWinTree.ShowExpansionIndicator.CheckOnDisplay;

            uTree.UseAppStyling = false;    //不使用才可以出現連接線....??

            this.uTree.AfterActivate += uTree_AfterActivate;
            this.uTree.BeforeActivate += uTree_BeforeActivate;
        }
        #endregion InitializeUI

        #region WfLoadTree
        private void WfLoadTree(DataTable dtTree)
        {
            List<vw_glai100> admi120List;
            try
            {
                if (dtTree == null)
                    return;
                uTree.Nodes.Clear();
                admi120List = dtTree.ToList<vw_glai100>();
                var rootNode = uTree.Nodes.Add("root", "根節點");
                rootNode.Override.ActiveNodeAppearance.Image = ImgList.Images[GlobalPictuer.MENU_TREE_FOLDER_ACTIVE];
                rootNode.Override.NodeAppearance.Image = ImgList.Images[GlobalPictuer.MENU_TREE_FOLDER];

                foreach (vw_glai100 tempModel in admi120List.Where(p => (p.gba06 == "1" && p.gba07.ToLower() == "root") || p.gba06 == "3"))
                {
                    //var addNode = rootNode.Nodes.Add(tempModel.gba01, string.Concat(tempModel.gba01, "-", tempModel.gba02));
                    var addNode = WfAddNode(rootNode, tempModel);
                    if (tempModel.gba06 == "1")
                    {
                        WfLoadTreeRecursive(tempModel.gba01, admi120List, addNode);
                    }
                }
                //uTree.ExpandAll(ExpandAllType.OnlyNodesWithChildren);
                uTree.PerformAction(UltraTreeAction.ExpandNode,false,false);
                uTree.PerformAction(UltraTreeAction.NextNode, false, false);

            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }
        #endregion

        #region WfLoadTreeRecursive
        private void WfLoadTreeRecursive(string pParentsId, List<vw_glai100> admi120List, UltraTreeNode pParentsNode)
        {
            List<vw_glai100> subGlai100List;
            try
            {
                subGlai100List = admi120List.Where(p => p.gba07 == pParentsId)
                                            .OrderBy(p => p.gba01)
                                            .ToList();
                if (subGlai100List == null || subGlai100List.Count == 0)
                    return;
                foreach (vw_glai100 tempModel in subGlai100List)
                {
                    //var addNode = pParentsNode.Nodes.Add(tempModel.gba01, string.Concat(tempModel.gba01, "-", tempModel.gba02));
                    var addNode = WfAddNode(pParentsNode, tempModel);
                    if (tempModel.gba06 == "1")
                    {
                        WfLoadTreeRecursive(tempModel.gba01, admi120List, addNode);
                    }
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
            string gba01;
            try
            {
                //if (_isInDeleteing == true)
                //    return;

                if (e.TreeNode == null)//有可能連根節點都刪掉了
                    return;

                gba01 = e.TreeNode.Key;
                var position = BindingMaster.Find("gba01", gba01);
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

        #region WfAddNode
        private UltraTreeNode WfAddNode(UltraTreeNode paretnsNode, vw_glai100 glai100Model)
        {
            try
            {
                var addNode = paretnsNode.Nodes.Add(glai100Model.gba01, string.Concat(glai100Model.gba01, "-", glai100Model.gba02));
                if (glai100Model.gba06 == "1")    //統制科目
                {
                    addNode.Override.ActiveNodeAppearance.Image = ImgList.Images[GlobalPictuer.MENU_TREE_FOLDER_ACTIVE];
                    addNode.Override.NodeAppearance.Image = ImgList.Images[GlobalPictuer.MENU_TREE_FOLDER];
                }
                return addNode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #endregion

        //******************************覆寫function************************
        #region WfAutoQueryFistLoad 載入時，自動查詢
        protected override void WfAutoQueryFistLoad()
        {
            try
            {
                TabMaster.DtSource.Rows.Add(TabMaster.DtSource.NewRow());
                WfQueryOk();
                WfDisplayMode();
                WfLoadTree(TabMaster.DtSource);
                WfAfterfDisplayMode();
            }
            catch (Exception ex)
            {
                throw ex;
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
                //加入這一段,統一將節點移至root---------開始
                uTree.Enabled = true;
                if (DrMaster != null)
                {
                    uTree.ActiveNode = uTree.GetNodeByKey(DrMaster["gba01"].ToString());
                }
                else
                    uTree.ActiveNode = uTree.GetNodeByKey("root");
                //加入這一段,統一將節點移至root----------結束

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

    }
}
