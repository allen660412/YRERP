/* 程式名稱: 廠商取價原則設定作業
   系統代號: puri030
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

namespace YR.ERP.Forms.Pur
{
    public partial class FrmPuri030 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        PurBLL BoPur = null;
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmPuri030()
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
            this.StrFormID = "puri030";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("pbb01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "pbbsecu";
                TabMaster.GroupColumn = "pbbsecg";
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

            BoPur = new PurBLL(BoMaster.OfGetConntion());
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
                    BoPur.TRAN = BoMaster.TRAN;
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
                    WfSetControlReadonly(new List<Control> { ute_pbbcreu, ute_pbbcreg, udt_pbbcred }, true);
                    WfSetControlReadonly(new List<Control> { ute_pbbmodu, ute_pbbmodg, udt_pbbmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_pbbsecu, ute_pbbsecg }, true);

                    WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "pbc02" }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_pbb01, true);
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
            this.TabDetailList[0].TargetTable = "pbc_tb";
            this.TabDetailList[0].ViewTable = "vw_puri030s";
            keyParm = new SqlParameter("pbc01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "pbb01";
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
                        pDr["pbc02"] = WfGetMaxSeq(pDr.Table, "pbc02");
                        pDr["pbc04"] = WfGetMaxSeq(pDr.Table, "pbc04");
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
            vw_puri030s detailModel;
            List<vw_puri030s> detailList;
            int iChkCnts = 0;
            try
            {
                #region 單頭-pick vw_puri030
                if (e.Row.Table.Prefix.ToLower() == "vw_puri030")
                {
                    switch (e.Column.ToLower())
                    {
                        case "pba01"://採購取價代號
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (BoPur.OfChkPbaPKExists(GlobalFn.isNullRet(e.Value, "")) == true)
                            {
                                WfShowErrorMsg("採購取價代號已存在,請檢核!");
                                return false;
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_puri030s
                if (e.Row.Table.Prefix.ToLower() == "vw_puri030s")
                {
                    detailModel = e.Row.ToItem<vw_puri030s>();
                    detailList = e.Row.Table.ToList<vw_puri030s>();
                    switch (e.Column.ToLower())
                    {
                        case "pbc03"://取價類型
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            iChkCnts=detailList.Where(p => p.pbc02 != detailModel.pbc02)
                                                .Where(p => p.pbc03 == e.Value.ToString())
                                                .Count();
                            if (iChkCnts>0)
                            {
                                WfShowErrorMsg("取價類型不得重覆!");
                                return false;
                            }
                            break;

                        case "pbc04"://取價順序
                            if (GlobalFn.varIsNull(e.Value) || GlobalFn.isNullRet(e.Value, 0) == 0)
                            {
                                return true;
                            }
                            iChkCnts = detailList.Where(p => p.pbc02 != detailModel.pbc02)
                                                .Where(p => p.pbc04 == Convert.ToInt16(e.Value))
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
                        ugc = uGrid.DisplayLayout.Bands[0].Columns["pbc03"];
                        WfSetGridValueList(ugc, BoPur.OfGetPbc03KVPList());
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
            vw_puri030 masterModel = null;
            vw_puri030s detailModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {
                masterModel = DrMaster.ToItem<vw_puri030>();
                #region 單頭資料檢查
                chkColName = "pbb01";
                chkControl = ute_pbb01;
                if (GlobalFn.varIsNull(masterModel.pbb01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pbb02";
                chkControl = ute_pbb02;
                if (GlobalFn.varIsNull(masterModel.pbb02))
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

                    detailModel = drTemp.ToItem<vw_puri030s>();
                    #region pbc03-取價類型
                    chkColName = "pbc03";
                    if (GlobalFn.varIsNull(detailModel.pbc03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region pbc04-順序
                    chkColName = "pbc04";
                    if (GlobalFn.varIsNull(detailModel.pbc04) || GlobalFn.isNullRet(detailModel.pbc04,0)<0)
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
                        DrMaster["pbbsecu"] = LoginInfo.UserNo;
                        DrMaster["pbbsecg"] = LoginInfo.GroupNo;
                        DrMaster["pbbcreu"] = LoginInfo.UserNo;
                        DrMaster["pbbcreg"] = LoginInfo.DeptNo;
                        DrMaster["pbbcred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["pbbmodu"] = LoginInfo.UserNo;
                        DrMaster["pbbmodg"] = LoginInfo.DeptNo;
                        DrMaster["pbbmodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["pbccreu"] = LoginInfo.UserNo;
                            drDetail["pbccreg"] = LoginInfo.DeptNo;
                            drDetail["pbccred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["pbcmodu"] = LoginInfo.UserNo;
                            drDetail["pbcmodg"] = LoginInfo.DeptNo;
                            drDetail["pbcmodd"] = Now;
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

    }
}
