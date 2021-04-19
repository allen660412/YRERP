/* 程式名稱: 庫存異動成本補入作業
   系統代號: invt302
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
using YR.ERP.Shared;

namespace YR.ERP.Customize.Forms.Inv
{
    public partial class FrmZinvt001 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        
        #region Property
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;

        baa_tb BaaTbModel = null;
        bek_tb BekModel = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmZinvt001()
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
            this.StrFormID = "zinvt001";

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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("iga01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "igasecu";
                TabMaster.GroupColumn = "igasecg";

                TabMaster.CanAddMode = false;
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

            BoBas = new BasBLL(BoMaster.OfGetConntion());
            BoInv = new InvBLL(BoMaster.OfGetConntion());
            BoAdm = new AdmBLL(BoMaster.OfGetConntion());
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
                    BoBas.TRAN = BoMaster.TRAN;
                    BoInv.TRAN = BoMaster.TRAN;
                    BoAdm.TRAN = BoMaster.TRAN;
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
                //單據確認
                sourceList = BoInv.OfGetIgaconfKVPList();
                WfSetUcomboxDataSource(ucb_igaconf, sourceList);
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

            this.TabDetailList[0].TargetTable = "igb_tb";
            this.TabDetailList[0].ViewTable = "vw_zinvt001s";
            keyParm = new SqlParameter("igb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "iga01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_zinvt001 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_zinvt001>();
                    WfSetDocPicture("", masterModel.igaconf, "", pbxDoc);
                    if ((FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        if (BaaModel == null || GlobalFn.varIsNull(BaaModel.baa04))
                        {
                            WfShowErrorMsg("未設定本國幣別,請先設定!");
                        }

                        BekModel = BoBas.OfGetBekModel(BaaModel.baa04);
                        if (BekModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", BaaModel.baa04));
                        }
                    }
                }
                else
                {
                    WfSetDocPicture("", "", "", pbxDoc);
                }

                if (FormEditMode == YREditType.NA)
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false); //先全開
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯

                    WfSetControlsReadOnlyRecursion(ute_iga01.Parent, true);
                    WfSetControlsReadOnlyRecursion(ute_igaconu.Parent, true);

                    //明細先全開,並交由 WfSetDetailDisplayMode處理
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
                                columnName == "igb07" 
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
                //SelectNextControl(this.uTab_Master, true, true, true, false);
                var ugrid = TabDetailList[0].UGrid;
                ugrid.Focus();

                if (ugrid.ActiveRow!=null)
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

        #region WfPreInUpdateModeCheck() 進存檔模式前檢查,及設定變數
        protected override bool WfPreInUpdateModeCheck()
        {
            Result result;
            vw_zinvt001 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_zinvt001>();
                BaaTbModel = BoBas.OfGetBaaModel();
                //檢查日期區間是否可修改
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.iga02), BaaModel);
                if (result.Success == false)
                {
                    WfShowBottomStatusMsg(result.Message);
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

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int iChkCnts = 0;
            vw_zinvt001 masterModel = null;
            vw_zinvt001s detailModel = null;
            List<vw_zinvt001s> detailList = null;
            bab_tb l_bab = null;
            UltraGrid uGrid = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_zinvt001>();
                #region 單身-pick vw_zinvt001s
                if (e.Row.Table.Prefix.ToLower() == "vw_zinvt001s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_zinvt001s>();
                    detailList = e.Row.Table.ToList<vw_zinvt001s>();
                    l_bab = BoBas.OfGetBabModel(masterModel.iga01);

                    switch (e.Column.ToLower())
                    {
                        case "igb07":   //異動成本
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.igb07 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.igb07, BekModel.bek03);
                            WfSetDetailAmt(e.Row);
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
        protected override bool WfFormCheck()
        {
            vw_zinvt001 masterModel = null;
            vw_zinvt001s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {

                masterModel = DrMaster.ToItem<vw_zinvt001>();
                if (!GlobalFn.varIsNull(masterModel.iga01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.iga01, ""));

                #region 單身資料檢查
                iChkDetailTab = 0;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)
                        continue;

                    detailModel = drTemp.ToItem<vw_zinvt001s>();
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

        #region WfAfterFormCheck() 存檔後處理,通常為放入Pk
        protected override bool WfAfterFormCheck()
        {
            string iga01New, errMsg;
            vw_zinvt001 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_zinvt001>();
                if (FormEditMode == YREditType.新增)
                {
                    iga01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.iga01, ModuleType.stp, (DateTime)masterModel.iga02, out iga01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["iga01"] = iga01New;
                }
                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["igasecu"] = LoginInfo.UserNo;
                        DrMaster["igasecg"] = LoginInfo.GroupNo;
                        DrMaster["igacreu"] = LoginInfo.UserNo;
                        DrMaster["igacreg"] = LoginInfo.DeptNo;
                        DrMaster["igacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)

                    {
                        DrMaster["igamodu"] = LoginInfo.UserNo;
                        DrMaster["igamodg"] = LoginInfo.DeptNo;
                        DrMaster["igamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["igbcreu"] = LoginInfo.UserNo;
                            drDetail["igbcreg"] = LoginInfo.DeptNo;
                            drDetail["igbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["igbmodu"] = LoginInfo.UserNo;
                            drDetail["igbmodg"] = LoginInfo.DeptNo;
                            drDetail["igbmodd"] = Now;
                        }
                    }
                }

                WfSetDetailPK();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //*****************************表單自訂Fuction****************************************

        #region WfSetDetailAmt 處理小計
        private bool WfSetDetailAmt(DataRow drSeb)
        {
            igb_tb igbModel;
            try
            {
                igbModel = drSeb.ToItem<igb_tb>();
                drSeb["igb08"] = igbModel.igb07 * igbModel.igb05;
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
