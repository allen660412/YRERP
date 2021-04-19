/* 程式名稱: 資料庫Lock查詢作業
   系統代號: admq910
   作　　者: Allen
   描　　述: 
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data;
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinGrid;
using YR.ERP.DAL.YRModel;
using YR.Util;
using System.Linq;
using Infragistics.Win;
using System.Drawing;
using Infragistics.Win.CalcEngine;
using Infragistics.Win.UltraWinToolbars;
using System.Text;

namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdmq910 : YR.ERP.Base.Forms.FrmEntryBase
    {
        #region Property
        AdmBLL BoAdm = null;
        #endregion

        #region 建構子
        public FrmAdmq910()
        {
            InitializeComponent();
        } 
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfSetVar()
        {
            this.StrFormID = "admq910";
            this.IntTabCount = 1;
            this.IntMasterGridPos = 1;
            uTab_Master.Tabs[0].Text = "資料瀏覽";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("spid", SqlDbType.Int)
                                });                

                TabMaster.CanAddMode = false;
                TabMaster.CanCopyMode = false;
                TabMaster.CanDeleteMode = false;
                TabMaster.CanUpdateMode = false;
                TabMaster.CanUseRowLock = false;
                TabMaster.CanAdvancedQueryMode = false;
                TabMaster.IsAutoQueryFistLoad = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable,false);

            BoAdm = new AdmBLL(BoMaster.OfGetConntion());
            return;
        }
        #endregion
        
        #region WfIniMasterGrid
        protected override void WfIniMasterGrid()
        {
            try
            {
                base.WfIniMasterGrid();
                this.panel2.Controls.Add(uGridMaster);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMasterGridLayout
        protected override void WfSetMasterGridLayout()
        {
            try
            {
                UltraGridColumn ugc;
                ugc=uGridMaster.DisplayLayout.Bands[0].Columns["text"];
                ugc.CellMultiLine = DefaultableBoolean.True;
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
                    BoAdm.TRAN = BoMaster.TRAN;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示
        protected override Boolean WfDisplayMode()
        {
            try
            {
                WfSetControlsReadOnlyRecursion(this, true);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfQueryDisplayMode 查詢後的狀態處理 readonly
        protected override bool WfQueryDisplayMode()
        {
            try
            {
                WfSetControlsReadOnlyRecursion(this, false);
                WfSetControlReadonly(uGridMaster, true);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfQueryOk() 查詢後按下OK按鈕
        protected override bool WfQueryOk()
        {
            DataTable dtSource = null;
            string strQueryAll;
            string strQueryApend = "";
            List<SqlParameter> sqlParmList;
            string strQueryExternal = "";   
            try
            {
                uGridMaster.PerformAction(UltraGridAction.ExitEditMode);
                this.TabMaster.DtSource.EndInit();
                sqlParmList = new List<SqlParameter>();
                this.StrQueryWhere = BoMaster.WfCombineQuerystring(TabMaster,out sqlParmList);

                StrQueryWhere = GlobalFn.isNullRet(StrQueryWhere, "");
                StrQueryWhereAppend = GlobalFn.isNullRet(StrQueryWhereAppend, "");

                //額外加入的查詢內容------開始
                if (ucx_qryIsBloced.Checked)
                {
                    strQueryExternal = " AND blkBy >0";
                }
                //額外加入的查詢內容------ end
                

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
                //額外加入的查詢內容------開始
                strQueryAll = StrQueryWhere + strQueryApend + this.StrQuerySecurity + strQueryExternal;
                //額外加入的查詢內容------ end

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

        #region WfAddAction 新增action按鈕
        protected override List<ButtonTool> WfAddAction()
        {
            List<ButtonTool> buttonList = new List<ButtonTool>();
            ButtonTool bt;
            try
            {
                bt = new ButtonTool("KillSession");
                bt.SharedProps.Caption = "刪除session";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);

                bt = new ButtonTool("Refresh");
                bt.SharedProps.Caption = "重新整理";
                bt.SharedProps.Category = "action";
                buttonList.Add(bt);
                                
                return buttonList;
            }
            catch (Exception ex)
            {
                throw ex;
            }  
        }
        #endregion

        #region WfActionClick(string pActionName)
        protected override void WfActionClick(string pActionName)
        {
            vw_admq910 masterModel;
            string killSql;
            try
            {
                switch (pActionName.ToLower())
                {
                    case "killsession":
                        if (DrMaster == null)
                            return;
                        masterModel = DrMaster.ToItem<vw_admq910>();
                        if (masterModel.blkBy<=0)
                        {
                            WfShowBottomStatusMsg("連線未被Lock不可刪除");
                            return;
                        }

                        if (WfShowConfirmMsg("請確認是否要刪除連線?") == DialogResult.Yes)
                        {
                            killSql = string.Concat("kill ",masterModel.blkBy,";"); 
                            BoMaster.OfExecuteNonquery(killSql);
                            WfShowBottomStatusMsg("連線刪除成功!");
                            if (WfQueryOk())
                            {
                                WfDisplayMode();
                                WfAfterfDisplayMode();
                            }
                        }
                        break;
                    case "refresh":
                        if (FormEditMode != YREditType.NA)
                            return;

                        //新增一列,模擬查詢
                        TabMaster.DtSource.Rows.Add(TabMaster.DtSource.NewRow());
                        WfGetActiveDatarow();
                        if (WfQueryOk())
                        {
                            WfDisplayMode();
                            WfAfterfDisplayMode();
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

    }
}
