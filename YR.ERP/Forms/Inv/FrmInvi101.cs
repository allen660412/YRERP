/* 程式名稱: 料件單位換算設定
   系統代號: invi101
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
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinGrid;
using YR.Util;
using YR.ERP.DAL.YRModel;
using YR.ERP.BLL.Model;


namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvi101 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        #endregion

        #region 建構子
        public FrmInvi101()
        {
            InitializeComponent();
        }

        public FrmInvi101(string pSourceForm, YR.ERP.Shared.UserInfo pUserInfo, string pWhere)
        {
            InitializeComponent();
            StrQueryWhereAppend = pWhere;
            this.LoginInfo = pUserInfo;
        }
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfSetVar()
        {
            this.StrFormID = "invi101";
            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            TabMaster.IsReadOnly = true;    //單頭不做存檔
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("ica01", SqlDbType.NVarChar)
                                });

                TabMaster.UserColumn = "icasecu";
                TabMaster.GroupColumn = "icasecg";
                TabMaster.CanAddMode = false;
                TabMaster.CanCopyMode = false;
                TabMaster.CanDeleteMode = false;
                TabMaster.CanUseRowLock = false;    //不Lock主表
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
                {
                    WfSetControlsReadOnlyRecursion(this, true);
                }
                else
                {
                    WfSetControlsReadOnlyRecursion(uTab_Master, true);
                    WfSetControlsReadOnlyRecursion(uTab_Detail, false);  //先全開其餘的由WfSetDetailDisplayMode處理
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
                            if (columnName == "icn04" ||
                                columnName == "icn05" ||
                                columnName == "icn06" ||
                                columnName == "icnvali" 
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "icn02"||columnName=="icn03")
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

                        //foreach (UltraGridCell ugc in pUgr.Cells)
                        //{
                        //    if (pDr.RowState == DataRowState.Added)//新增時
                        //        WfSetControlReadonly(ugc, false);
                        //    else    //修改時
                        //    {
                        //        if (ugc.Column.Key.ToLower() == "icn02" ||
                        //            ugc.Column.Key.ToLower() == "icn03")
                        //            WfSetControlReadonly(ugc, true);
                        //        else
                        //            WfSetControlReadonly(ugc, false);
                        //    }
                        //    WfSetControlReadonly(ugc, true);
                        //}
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
                if (FormEditMode == YREditType.查詢)
                {
                    uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    SelectNextControl(this.uTab_Master, true, true, true, false);
                }
                else
                {
                    uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    SelectNextControl(this.uTab_Detail, true, true, true, false);
                    WfSetFirstVisibelCellFocus(TabDetailList[0].UGrid);
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

            this.TabDetailList[0].TargetTable = "icn_tb";
            this.TabDetailList[0].ViewTable = "vw_invi101s";
            keyParm = new SqlParameter("icn01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "ica01";
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
                        pDr["icn01"] = DrMaster["ica01"];
                        pDr["icn04"] = 0;
                        pDr["icn05"] = 0;
                        pDr["icnvali"] = "Y";
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

        #region WfSeDetailGridLayout
        protected override void WfSeDetailGridLayout(UltraGrid pUgrid)
        {
            Infragistics.Win.UltraWinGrid.UltraGridColumn ugc;
            try
            {
                if (pUgrid.DisplayLayout.Bands[0].Columns.Exists("icnvali"))
                {
                    ugc = pUgrid.DisplayLayout.Bands[0].Columns["icnvali"];
                    WfSetUgridCheckBox(ugc);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單身-pick vw_invi101
                if (pDr.Table.Prefix.ToLower() == "vw_invi101s")
                {
                    switch (pColName.ToLower())
                    {
                        case "icn02"://來源單位
                            WfShowPickUtility("p_bej", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                            //}
                            break;
                        case "icn03"://目的單位
                            WfShowPickUtility("p_bej", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                            //}
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
            vw_invi101 masterModel = null;
            try
            {
                #region 單頭-pick vw_invi101
                if (e.Row.Table.Prefix.ToLower() == "vw_invi101s")
                {
                    masterModel = DrMaster.ToItem<vw_invi101>();
                    switch (e.Column.ToLower())
                    {
                        case "icn02"://來源單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此單位存在,請檢核!");
                                return false;
                            }
                            break;

                        case "icn03"://目的單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此單位存在,請檢核!");
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_invi101s detailModel = null;
            UltraGrid uGrid;
            string msg;
            string chkColName;
            int iChkDetailTab;
            try
            {
                #region 單身資料檢查
                iChkDetailTab = 0;
                uGrid = TabDetailList[iChkDetailTab].UGrid;
                foreach (DataRow drTemp in TabDetailList[iChkDetailTab].DtSource.Rows)
                {
                    if (drTemp.RowState == DataRowState.Unchanged)
                        continue;

                    detailModel = drTemp.ToItem<vw_invi101s>();
                    #region icn02-來源單位
                    chkColName = "icn02";
                    if (GlobalFn.varIsNull(detailModel.icn02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region icn03-目的單位
                    chkColName = "icn03";
                    if (GlobalFn.varIsNull(detailModel.icn03))
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
                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["icncreu"] = LoginInfo.UserNo;
                            drDetail["icncreg"] = LoginInfo.DeptNo;
                            drDetail["icncred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["icnmodu"] = LoginInfo.UserNo;
                            drDetail["icnmodg"] = LoginInfo.DeptNo;
                            drDetail["icnmodd"] = Now;
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
