/* 程式名稱: 初盤維護作業
   系統代號: invt520
   作　　者: Allen
   描　　述: 
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
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.ERP.BLL.Model;

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvt520 : YR.ERP.Base.Forms.FrmEntryMDBase
    {

        #region Property
        InvBLL BoInv = null;
        #endregion

        #region 建構子
        public FrmInvt520()
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
            this.StrFormID = "invt520";

            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "基本資料";
            uTab_Master.Tabs[1].Text = "狀態";
            uTab_Master.Tabs[2].Text = "資料瀏覽";

            IntTabDetailCount = 1;
            uTab_Detail.Tabs[0].Text = "明細資料";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("ipa01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "ipasecu";
                TabMaster.GroupColumn = "ipasecg";

                TabMaster.CanAddMode = false;
                TabMaster.CanCopyMode = false;
                TabMaster.CanDeleteMode = false;
                TabMaster.CanCopyMode = false;
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
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
            BoInv = new InvBLL(BoMaster.OfGetConntion());
            return;
        }
        #endregion
        
        #region WfSetBllTransaction 以bomaster 註冊transaction至其他 bll
        protected override void WfSetBllTransaction()
        {
            try
            {
                if (BoMaster.TRAN != null)
                {
                    BoInv.TRAN = BoMaster.TRAN;
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

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfIniTabDetailInfo(): 設定明細的資料來源
        protected override Boolean WfIniTabDetailInfo()
        {
            SqlParameter keyParm;

            this.TabDetailList[0].TargetTable = "ipb_tb";
            this.TabDetailList[0].ViewTable = "vw_invt520s";
            keyParm = new SqlParameter("ipb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "ipa01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_invt520 masterModel = null;
            try
            {
                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false); //先全開
                    WfSetControlsReadOnlyRecursion(ute_ipa01.Parent, true);
                    WfSetControlsReadOnlyRecursion(ute_ipacreu.Parent, true);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetDetailDisplayMode 新增修改時的明細列可輸入處理,需要每筆資料列微調整時再使用
        protected override void WfSetDetailDisplayMode(int pCurTabDetail, UltraGridRow pUgr, DataRow pDr)
        {
            string columnName;
            bab_tb babModel;
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
                                columnName == "ipb30" 
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

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

        #region WfAfterfDisplayMode  新增修改刪除查詢後的 focus調整
        protected override void WfAfterfDisplayMode()
        {
            try
            {
                uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                if (FormEditMode==YREditType.查詢)
                {
                    SelectNextControl(this.uTab_Master, true, true, true, false);
                    return;
                }

                var ugrid = TabDetailList[0].UGrid;
                ugrid.Focus();

                if (ugrid.ActiveRow != null)
                {
                    var drActive = WfGetUgridDatarow(ugrid.ActiveRow);
                    WfSetDetailDisplayMode(0, ugrid.ActiveRow, drActive);
                    WfSetFirstVisibelCellFocus(ugrid);                    
                }                
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }
        #endregion

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int iChkCnts = 0;
            vw_invt520 masterModel = null;
            vw_invt520s detailModel = null;
            List<vw_invt520s> listDetails = null;
            UltraGrid uGrid = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt520>();
                #region 單身-pick vw_invt520s
                if (e.Row.Table.Prefix.ToLower() == "vw_invt520s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_invt520s>();
                    listDetails = e.Row.Table.ToList<vw_invt520s>();
                    switch (e.Column.ToLower())
                    {
                        case "ipb30"://初盤數量
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入異動數量!");
                                return false;
                            }
                            if (GlobalFn.isNullRet(e.Value,0)<0)
                            {
                                WfShowErrorMsg("數量不可小於0!");
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

        #region WfPreInUpdateModeCheck() 進存檔模式前檢查,及設定變數
        protected override bool WfPreInUpdateModeCheck()
        {
            vw_invt520 masterModel;            
            try
            {
                masterModel = DrMaster.ToItem<vw_invt520>();
                if (masterModel.ipa05 == "N")
                {
                    WfShowBottomStatusMsg("未執行重計，不可修改!");
                    return false;
                }

                if (masterModel.ipa07 == "Y")
                {
                    WfShowBottomStatusMsg("已過帳，不可修改!");
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

    }
}
