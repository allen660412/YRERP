/* 程式名稱: 基本PICK查詢設定作業
   系統代號: admi650
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

namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdmi650 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        AdmBLL BoAdm = null;
        #endregion

        #region 建構子
        public FrmAdmi650()
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
            this.StrFormID = "admi650";
            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "資料內容";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("azp01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "azpsecu";
                TabMaster.GroupColumn = "azpsecg";
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
                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else
                {
                    WfSetControlsReadOnlyRecursion(this, false);
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    WfSetControlReadonly(new List<Control> { ute_azpcreu, ute_azpcreg, udt_azpcred }, true);
                    WfSetControlReadonly(new List<Control> { ute_azpmodu, ute_azpmodg, udt_azpmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_azpsecu, ute_azpsecg }, true);

                    WfSetControlReadonly(ute_azp03_c, true);
                    WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "azq03_c", "azq04_c" }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_azp01, true);
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
                            if (columnName == "azq03" ||
                                columnName == "azq04" ||
                                columnName == "azq06" ||
                                columnName == "azq07" ||
                                columnName == "azq08" 
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "azq02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }
                            WfSetControlReadonly(ugc, true);
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
                SelectNextControl(this.uTab_Master, true, true, true, false);

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

            this.TabDetailList[0].TargetTable = "azq_tb";
            this.TabDetailList[0].ViewTable = "vw_admi650s";
            keyParm = new SqlParameter("azq01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "azp01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            vw_admi650s detailModel = null;
            UltraGrid uGrid;
            try
            {
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_admi650
                if (pDr.Table.Prefix.ToLower() == "vw_admi650")
                {
                    switch (pColName.ToLower())
                    {
                        case "azp03"://主要資料表
                            WfShowPickUtility("p_atb", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["atb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["atb01"], "");
                            //}
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_admi650s
                if (pDr.Table.Prefix.ToLower() == "vw_admi650s")
                {
                    detailModel = pDr.ToItem<vw_admi650s>();

                    uGrid = sender as UltraGrid;
                    switch (pColName.ToLower())
                    {
                        case "azq03"://資料表名稱
                            WfShowPickUtility("p_atb", messageModel);
                            if (messageModel != null && messageModel.DataRowList.Count > 0)
                            {
                                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["atb01"], "");
                            }
                            break;

                        case "azq04"://資料欄位名稱
                            if (GlobalFn.isNullRet(detailModel.azq03, "") == "")
                                WfShowPickUtility("p_atc", messageModel);
                            else
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@atc01", detailModel.azq03));
                                WfShowPickUtility("p_atc1", messageModel);
                            }

                            if (messageModel != null && messageModel.DataRowList.Count > 0)
                            {
                                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["atc02"], "");

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

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int iChkCnts = 0;
            vw_admi650s detailModel = null;
            List<vw_admi650s> detailList = null;
            UltraGrid uGrid;
            try
            {
                #region 單頭-pick vw_admi650
                if (e.Row.Table.Prefix.ToLower() == "vw_admi650")
                {
                    switch (e.Column.ToLower())
                    {
                        case "azp01"://開窗代號
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (BoAdm.OfChkAzp01Exists(GlobalFn.isNullRet(e.Value, "")) == true)
                            {
                                WfShowErrorMsg("開窗代號已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "azp03"://主要資料表
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (BoAdm.OfChkAtb01Exists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此資料表代碼,請檢核!");
                                return false;
                            }
                            e.Row["azp03_c"] = BoAdm.OfGetAtb02(GlobalFn.isNullRet(e.Value, ""));
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_admi650s
                if (e.Row.Table.Prefix.ToLower() == "vw_admi650s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_admi650s>();
                    switch (e.Column.ToLower())
                    {
                        case "azq02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            detailList = e.Row.Table.ToList<vw_admi650s>();
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.azq02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;
                        case "azq03"://資料表代碼
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["azq03_c"] = "";
                                return true;
                            }
                            if (BoAdm.OfChkAtb01Exists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此資料表代碼,請檢核!");
                                return false;
                            }
                            e.Row["azq03_c"] = BoAdm.OfGetAtb02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "azq04"://資料欄位代碼
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["azq04_c"] = "";
                                return true;
                            }

                            if (BoAdm.OfChkAtc02Exists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此資料欄位代碼,請檢核!");
                                return false;
                            }
                            e.Row["azq04_c"] = BoAdm.OfGetAtc03(GlobalFn.isNullRet(e.Value, ""));
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
                        ugc = uGrid.DisplayLayout.Bands[0].Columns["azq06"];//元件類型
                        WfSetGridValueList(ugc, BoAdm.OfGetAzq06KVPList());
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
                pDr["azp06"] = "N";
                pDr["azp08"] = "N";
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
                switch (pCurTabDetail)
                {
                    case 0:
                        pDr["azq02"] = WfGetMaxSeq(pDr.Table, "azq02");
                        pDr["azq03"] = DrMaster["azp03"];
                        pDr["azq03_c"] = BoAdm.OfGetAtb02(GlobalFn.isNullRet(DrMaster["azp03"], ""));
                        pDr["azq06"] = "1"; //元件類型 1.edit
                        pDr["azq08"] = 100;//預設欄位長度 100px
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_admi650 masterModel = null;
            vw_admi650s detailModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {
                masterModel = DrMaster.ToItem<vw_admi650>();
                #region 單頭資料檢查
                chkColName = "azp01";
                chkControl = ute_azp01;
                if (GlobalFn.varIsNull(masterModel.azp01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "azp02";
                chkControl = ute_azp02;
                if (GlobalFn.varIsNull(masterModel.azp02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "azp03";
                chkControl = ute_azp03;
                if (GlobalFn.varIsNull(masterModel.azp03))
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

                #region 單身資料檢查
                iChkDetailTab = 0;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_admi650s>();
                    #region azq03-資料表代碼
                    chkColName = "azq03";
                    if (GlobalFn.varIsNull(detailModel.azq03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region azq04-資料欄位代碼
                    chkColName = "azq04";
                    if (GlobalFn.varIsNull(detailModel.azq04))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region azq06-資料元件類型
                    chkColName = "azq06";
                    if (GlobalFn.varIsNull(detailModel.azq06))
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
                        DrMaster["azpsecu"] = LoginInfo.UserNo;
                        DrMaster["azpsecg"] = LoginInfo.GroupNo;
                        DrMaster["azpcreu"] = LoginInfo.UserNo;
                        DrMaster["azpcreg"] = LoginInfo.DeptNo;
                        DrMaster["azpcred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["azpmodu"] = LoginInfo.UserNo;
                        DrMaster["azpmodg"] = LoginInfo.DeptNo;
                        DrMaster["azpmodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["azqcreu"] = LoginInfo.UserNo;
                            drDetail["azqcreg"] = LoginInfo.DeptNo;
                            drDetail["azqcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["azqmodu"] = LoginInfo.UserNo;
                            drDetail["azqmodg"] = LoginInfo.DeptNo;
                            drDetail["azqmodd"] = Now;
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
    }
}
