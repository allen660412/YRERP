/* 程式名稱: 客戶取價原則設定作業
   系統代號: stpi030
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


namespace YR.ERP.Forms.Stp
{
    public partial class FrmStpi030 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        StpBLL BoStp = null;
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmStpi030()
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
            this.StrFormID = "stpi030";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("sbb01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "sbbsecu";
                TabMaster.GroupColumn = "sbbsecg";

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

            BoStp = new StpBLL(BoMaster.OfGetConntion());
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
                    BoStp.TRAN = BoMaster.TRAN;
                    BoBas.TRAN = BoMaster.TRAN;
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

                    WfSetControlReadonly(new List<Control> { ute_sbbcreu, ute_sbbcreg, udt_sbbcred }, true);
                    WfSetControlReadonly(new List<Control> { ute_sbbmodu, ute_sbbmodg, udt_sbbmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_sbbsecu, ute_sbbsecg }, true);

                    WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "sbc02" }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_sbb01, true);
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

        #region WfIniTabDetailInfo(): 設定明細的資料來源
        protected override Boolean WfIniTabDetailInfo()
        {
            SqlParameter keyParm;
            this.TabDetailList[0].TargetTable = "sbc_tb";
            this.TabDetailList[0].ViewTable = "vw_stpi030s";
            keyParm = new SqlParameter("sbc01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "sbb01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
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
                        pDr["sbc02"] = WfGetMaxSeq(pDr.Table, "sbc02");
                        pDr["sbc04"] = WfGetMaxSeq(pDr.Table, "sbc04");
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
            vw_stpi030s detailModel;
            List<vw_stpi030s> detailList;
            int iChkCnts = 0;
            try
            {
                #region 單頭-pick vw_stpi030
                if (e.Row.Table.Prefix.ToLower() == "vw_stpi030")
                {
                    switch (e.Column.ToLower())
                    {
                        case "sbb01"://銷售取價代號
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (BoStp.OfChkSbbPKExists(GlobalFn.isNullRet(e.Value, "")) == true)
                            {
                                WfShowErrorMsg("採購取價代號已存在,請檢核!");
                                return false;
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_stpi030s
                if (e.Row.Table.Prefix.ToLower() == "vw_stpi030s")
                {
                    detailModel = e.Row.ToItem<vw_stpi030s>();
                    detailList = e.Row.Table.ToList<vw_stpi030s>();
                    switch (e.Column.ToLower())
                    {
                        case "sbc03"://取價類型
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            iChkCnts = detailList.Where(p=>p.sbc02!=detailModel.sbc02)
                                                .Where(p => p.sbc03 == e.Value.ToString())
                                                .Count();
                            if (iChkCnts > 0)
                            {
                                WfShowErrorMsg("取價類型不得重覆!");
                                return false;
                            }
                            break;

                        case "sbc04"://取價順序
                            if (GlobalFn.varIsNull(e.Value) || GlobalFn.isNullRet(e.Value, 0) == 0)
                            {
                                return true;
                            }
                            iChkCnts = detailList.Where(p => p.sbc02 != detailModel.sbc02)
                                                .Where(p => p.sbc04 == Convert.ToInt16(e.Value))
                                                .Count();
                            if (iChkCnts > 0)
                            {
                                WfShowErrorMsg("取價順序不得重覆!");
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
                        ugc = uGrid.DisplayLayout.Bands[0].Columns["sbc03"];
                        WfSetGridValueList(ugc, BoStp.OfGetSbc03KVPList());
                        break;
                }
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
            vw_stpi030 masterModel = null;
            vw_stpi030s detailModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpi030>();
                #region 單頭資料檢查
                chkColName = "sbb01";
                chkControl = ute_sbb01;
                if (GlobalFn.varIsNull(masterModel.sbb01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sbb02";
                chkControl = ute_sbb02;
                if (GlobalFn.varIsNull(masterModel.sbb02))
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
                    if (drTemp.RowState == DataRowState.Unchanged)
                        continue;

                    detailModel = drTemp.ToItem<vw_stpi030s>();
                    #region sbc03-取價類型
                    chkColName = "sbc03";
                    if (GlobalFn.varIsNull(detailModel.sbc03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region sbc04-順序
                    chkColName = "sbc04";
                    if (GlobalFn.varIsNull(detailModel.sbc04) || GlobalFn.isNullRet(detailModel.sbc04, 0) < 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "應大於0";
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
                        DrMaster["sbbsecu"] = LoginInfo.UserNo;
                        DrMaster["sbbsecg"] = LoginInfo.GroupNo;
                        DrMaster["sbbcreu"] = LoginInfo.UserNo;
                        DrMaster["sbbcreg"] = LoginInfo.DeptNo;
                        DrMaster["sbbcred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["sbbmodu"] = LoginInfo.UserNo;
                        DrMaster["sbbmodg"] = LoginInfo.DeptNo;
                        DrMaster["sbbmodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["sbccreu"] = LoginInfo.UserNo;
                            drDetail["sbccreg"] = LoginInfo.DeptNo;
                            drDetail["sbccred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["sbcmodu"] = LoginInfo.UserNo;
                            drDetail["sbcmodg"] = LoginInfo.DeptNo;
                            drDetail["sbcmodd"] = Now;
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
