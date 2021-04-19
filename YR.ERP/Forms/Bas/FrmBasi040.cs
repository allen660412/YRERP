/* 程式名稱: 編碼性質維護作業
   系統代號: basi040
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
using System.Windows.Forms;
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinGrid;
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.ERP.BLL.Model;


namespace YR.ERP.Forms.Bas
{
    public partial class FrmBasi040 : YR.ERP.Base.Forms.FrmEntryMDBase
    {

        #region Property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmBasi040()
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
            this.StrFormID = "basi040";
            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "資料內容";
            uTab_Master.Tabs[1].Text = "狀態";
            uTab_Master.Tabs[2].Text = "資料瀏覽";
            IntTabDetailCount = 2;
            uTab_Detail.Tabs[0].Text = "段數設定";
            uTab_Detail.Tabs[1].Text = "分類編號";
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            try
            {
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("bga01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "bgasecu";
                TabMaster.GroupColumn = "bgasecg";
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
                }
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
            // 設定 Detail tab1 資料 : 段數設定
            this.TabDetailList[0].TargetTable = "bgb_tb";
            this.TabDetailList[0].ViewTable = "vw_basi040a";
            keyParm = new SqlParameter("bgb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "bga01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };

            // 設定 Detail tab1 資料 : 細項分類設定檔
            this.TabDetailList[1].TargetTable = "bgc_tb";
            this.TabDetailList[1].ViewTable = "vw_basi040b";
            keyParm = new SqlParameter("bgc01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "bga01";
            this.TabDetailList[1].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示
        protected override Boolean WfDisplayMode()
        {
            try
            {
                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false);
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    WfSetControlReadonly(new List<Control> { ute_bgacreu, ute_bgacreg, udt_bgacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_bgamodu, ute_bgamodg, udt_bgamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_bgasecu, ute_bgasecg }, true);

                    WfSetControlReadonly(ute_bga04, true);
                    WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "bgb02" }, true);
                    WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "bgc02" }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_bga01, true);
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

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                //編碼性質
                sourceList = BoBas.OfGetBga03KVPList();
                WfSetUcomboxDataSource(ucb_bga03, sourceList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfIniDetailComboSource 處理grid下拉選單
        protected override void WfIniDetailComboSource(int pTabIndex)
        {
            UltraGrid uGrid;
            UltraGridColumn ugc;
            try
            {
                switch (pTabIndex)
                {
                    case 0:
                        uGrid = TabDetailList[pTabIndex].UGrid;
                        ugc = uGrid.DisplayLayout.Bands[0].Columns["bgb05"];//元件類型
                        WfSetGridValueList(ugc, BoBas.OfGetBgb05KVPList());
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["bga04"] = 0;
                pDr["bgavali"] = "Y";
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetDetailRowDefault(int pCurTabDetail, DataRow pDr) 設定明細資料列的初始值
        protected override bool WfSetDetailRowDefault(int pCurTabDetail, DataRow pDr)
        {
            try
            {
                switch (pDr.Table.Prefix.ToLower())
                {
                    case "vw_basi040a":
                        pDr["bgb02"] = WfGetMaxSeq(pDr.Table, "bgb02");
                        break;
                    case "vw_basi040b":
                        pDr["bgc02"] = WfGetMaxSeq(pDr.Table, "bgc02");
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
        protected override void WfAfterDetailInsert(int pCurTabDetail, DataRow pDr)
        {
            try
            {
                switch (pDr.Table.Prefix.ToLower())
                {
                    case "vw_basi040a":
                        WfSetBga04();
                        break;
                }
                return ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAfterDetailDelete() :刪除明細後調用
        protected override bool  WfAfterDetailDelete(int pCurTabDetail, DataRow pDr)
        {
            try
            {
                switch (pDr.Table.Prefix.ToLower())
                {
                    case "vw_basi040a":
                        WfSetBga04();
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

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            List<vw_basi040a> basi040aList;
            int chkCnts = 0;
            try
            {
                #region 單頭- vw_basi040
                if (e.Row.Table.Prefix.ToLower() == "vw_basi040")
                {
                    switch (e.Column.ToLower())
                    {
                        case "bga01"://編碼類別
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (BoBas.OfChkBgaPKValid(GlobalFn.isNullRet(e.Value, "")) == true)
                            {
                                WfShowErrorMsg("編碼類別已存在,請檢核!");
                                return false;
                            }
                            break;
                    }
                }
                #endregion

                #region 單身- vw_basi040a 編碼各段數設定
                if (e.Row.Table.Prefix.ToLower() == "vw_basi040a")
                {
                    basi040aList=e.Row.Table.ToList<vw_basi040a>();
                    switch (e.Column.ToLower())
                    {
                        case "bgb04"://長度
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            WfSetBga04();//設定單頭的總長度
                            break;
                        case "bgb05"://段次類別 1固定 2流水號 3.分類碼
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (e.Value.ToString() == "2" || e.Value.ToString() == "3")
                            {
                                e.Row["bgb06"] = "";//固定值清空
                            }
                            //除 1.固定值以外,同一個項目只能存一筆
                            if (e.Value.ToString()!="1")
                            {
                                chkCnts = basi040aList.Where(x => x.bgb05 == e.Value.ToString())
                                                   .Count();
                                if (chkCnts>1)
                                {
                                    WfShowErrorMsg("除固定值以外,僅能輸入一筆資料!");
                                    return false;
                                }
                            }
                            break;

                    }
                }
                #endregion

                #region 單身- vw_basi040b 編碼細項分類設定檔
                if (e.Row.Table.Prefix.ToLower() == "vw_basi040b")
                {
                    switch (e.Column.ToLower())
                    {
                        case "bgc03"://分類編號
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (WfChkbgc03(e.Value.ToString()) == false)
                                return false;
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
            vw_basi040 masterModel = null;
            vw_basi040a basi040aModel = null;
            List<vw_basi040a> basi040aList = null;
            vw_basi040b basi040bModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab, iChkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_basi040>();
                #region 單頭資料檢查
                chkColName = "bga01";//編碼類別
                chkControl = ute_bga01;
                if (GlobalFn.varIsNull(masterModel.bga01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "bga02";//編碼名稱
                chkControl = ute_bga02;
                if (GlobalFn.varIsNull(masterModel.bga02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "bga03";//編碼性質
                chkControl = ucb_bga03;
                if (GlobalFn.varIsNull(masterModel.bga03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                #endregion

                #region 單身資料檢查--編碼各段數設定
                iChkDetailTab = 0;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)
                        continue;

                    basi040aModel = drTemp.ToItem<vw_basi040a>();
                    #region bgb03-段數說明
                    chkColName = "bgb03";
                    if (GlobalFn.varIsNull(basi040aModel.bgb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region bgb04-長度
                    chkColName = "bgb04";
                    if (GlobalFn.varIsNull(basi040aModel.bgb04) || basi040aModel.bgb04 == 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為零或空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region bgb05-段次類別
                    chkColName = "bgb05";
                    if (GlobalFn.varIsNull(basi040aModel.bgb05))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion
                }
                #endregion

                #region 單身資料檢查--編碼各段數設定
                iChkDetailTab = 1;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                basi040aList = TabDetailList[0].DtSource.ToList<vw_basi040a>();

                iChkCnts = basi040aList.Where(x => x.bgb05 == "3").Count();
                if (iChkCnts == 0 && TabDetailList[iChkDetailTab].DtSource.Rows.Count > 0)
                {
                    this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                    msg = "編碼段數未設定分類碼,因此不需新增分類明細!";
                    WfShowErrorMsg(msg);
                    return false;
                }
                if (iChkCnts > 0 && TabDetailList[iChkDetailTab].DtSource.Rows.Count == 0)
                {
                    this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                    msg = "編碼段數已未設定分類碼,請新增分類明細!";
                    WfShowErrorMsg(msg);
                    return false;
                }

                basi040aModel = basi040aList.Where(x => x.bgb05 == "3").FirstOrDefault();
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    basi040bModel = drTemp.ToItem<vw_basi040b>();
                    #region bgc03-分類編號
                    chkColName = "bgc03";
                    if (GlobalFn.varIsNull(basi040bModel.bgc03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    if (WfChkbgc03(basi040bModel.bgc03) == false)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region bgc04-分類名稱
                    chkColName = "bgc04";
                    if (GlobalFn.varIsNull(basi040bModel.bgc04))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion
                }
                #endregion

                WfSetDetailPK();
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
                        DrMaster["bgasecu"] = LoginInfo.UserNo;
                        DrMaster["bgasecg"] = LoginInfo.GroupNo;
                        DrMaster["bgacreu"] = LoginInfo.UserNo;
                        DrMaster["bgacreg"] = LoginInfo.DeptNo;
                        DrMaster["bgacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["bgamodu"] = LoginInfo.UserNo;
                        DrMaster["bgamodg"] = LoginInfo.DeptNo;
                        DrMaster["bgamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["bgbcreu"] = LoginInfo.UserNo;
                            drDetail["bgbcreg"] = LoginInfo.DeptNo;
                            drDetail["bgbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["bgbmodu"] = LoginInfo.UserNo;
                            drDetail["bgbmodg"] = LoginInfo.DeptNo;
                            drDetail["bgbmodd"] = Now;
                        }
                    }
                }

                foreach (DataRow drDetail in TabDetailList[1].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["bgccreu"] = LoginInfo.UserNo;
                            drDetail["bgccreg"] = LoginInfo.DeptNo;
                            drDetail["bgccred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["bgcmodu"] = LoginInfo.UserNo;
                            drDetail["bgcmodg"] = LoginInfo.DeptNo;
                            drDetail["bgcmodd"] = Now;
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
        #region WfChkbgc03
        private bool WfChkbgc03(string pBgc03)
        {
            DataTable dtBgb;
            DataRow[] drBgbArray;
            vw_basi040a basi040aModel;
            try
            {
                dtBgb = TabDetailList[0].DtSource;
                if (pBgc03 == "")
                    return true;

                drBgbArray = dtBgb.Select("bgb05='3'");
                if (drBgbArray.Length == 0)
                {
                    WfShowErrorMsg("段次類別無分類碼設定");
                    return false;
                }
                basi040aModel = drBgbArray[0].ToItem<vw_basi040a>();
                if (pBgc03.Length != basi040aModel.bgb04)
                {
                    WfShowErrorMsg("分類編號長度與分類設定長度不同!");
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

        #region WfSetBga04 依bgb04 更新 bga04
        void WfSetBga04()
        {
            int bgb04;
            try
            {
                bgb04 = 0;
                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    bgb04 += GlobalFn.isNullRet(dr["bgb04"], 0);
                }
                DrMaster["bga04"] = bgb04;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
