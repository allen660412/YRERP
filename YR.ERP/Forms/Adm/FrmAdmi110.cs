/* 程式名稱: 使用者角色設定作業
   系統代號: admi110
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
using YR.ERP.BLL.MSSQL;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinGrid;
using YR.ERP.DAL.YRModel;
using YR.Util;
using YR.ERP.BLL.Model;
using System.Windows.Forms;

namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdmi110 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        AdmBLL BoAdm = null;
        #endregion

        #region 建構子
        public FrmAdmi110()
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
            this.StrFormID = "admi110";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("adc01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "adcsecu";
                TabMaster.GroupColumn = "adcsecg";
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
            BoAdm = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
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
                    WfSetControlReadonly(uGridMaster, true);           //grid不可編輯
                    WfSetControlReadonly(new List<Control> { ute_adccreu, ute_adccreg, udt_adccred }, true);
                    WfSetControlReadonly(new List<Control> { ute_adcmodu, ute_adcmodg, udt_adcmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_adcsecu, ute_adcsecg }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_adc01, true);
                    }

                    WfSetControlReadonly(TabDetailList[0].UGrid, new List<string> { "add02_c", "add13" }, true);
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
            SqlParameter lParm;
            // 設定 Detail tab1 資料 : 
            this.TabDetailList[0].TargetTable = "add_tb";
            this.TabDetailList[0].ViewTable = "vw_admi110s";
            lParm = new SqlParameter("add01", SqlDbType.NVarChar);
            lParm.SourceColumn = "adc01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { lParm };
            return true;
        }
        #endregion

        #region WfSeDetailGridLayout
        protected override void WfSeDetailGridLayout(UltraGrid pUgrid)
        {
            Infragistics.Win.UltraWinGrid.UltraGridColumn lugc;
            try
            {
                lugc = pUgrid.DisplayLayout.Bands[0].Columns["add03"];
                WfSetUgridCheckBox(lugc);

                lugc = pUgrid.DisplayLayout.Bands[0].Columns["add04"];
                WfSetUgridCheckBox(lugc);

                lugc = pUgrid.DisplayLayout.Bands[0].Columns["add05"];
                WfSetUgridCheckBox(lugc);

                lugc = pUgrid.DisplayLayout.Bands[0].Columns["add06"];
                WfSetUgridCheckBox(lugc);

                lugc = pUgrid.DisplayLayout.Bands[0].Columns["add07"];
                WfSetUgridCheckBox(lugc);

                lugc = pUgrid.DisplayLayout.Bands[0].Columns["add08"];
                WfSetUgridCheckBox(lugc);

                lugc = pUgrid.DisplayLayout.Bands[0].Columns["add09"];
                WfSetUgridCheckBox(lugc);

                lugc = pUgrid.DisplayLayout.Bands[0].Columns["add10"];
                WfSetUgridCheckBox(lugc);

                lugc = pUgrid.DisplayLayout.Bands[0].Columns["add11"];
                WfSetUgridCheckBox(lugc);

                lugc = pUgrid.DisplayLayout.Bands[0].Columns["add12"];
                WfSetUgridCheckBox(lugc);

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
            int iChkCnts = 0;
            string errMsg;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            List<vw_admi110s> detailList;
            vw_admi110s detailModel;
            try
            {
                MessageInfo messageModel = new MessageInfo();
                #region 單身-pick vw_admi110s
                if (pDr.Table.Prefix.ToLower() == "vw_admi110s")
                {
                    detailModel = pDr.ToItem<vw_admi110s>();
                    detailList = pDr.Table.ToList<vw_admi110s>();
                    switch (pColName.ToLower())
                    {
                        case "add02":
                            #region add02 執行程式
                            messageModel.IntMaxRow = 0;
                            WfShowPickUtility("p_ado1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count == 0)
                                    pDr[pColName] = "";
                                else if (messageModel.DataRowList.Count == 1)
                                    pDr[pColName] = messageModel.DataRowList[0]["ado01"];
                                else
                                {
                                    var drReturns = messageModel.DataRowList;
                                    var addCnts = 0;
                                    foreach (DataRow drTemp in messageModel.DataRowList)
                                    {
                                        var ado01 = GlobalFn.isNullRet(drTemp["ado01"], "");
                                        //檢查是否有重覆
                                        iChkCnts = detailList.Where(p => p.add02 == ado01).Count();
                                        if (iChkCnts > 0)
                                            continue;
                                        addCnts++;
                                        if (addCnts == 1)
                                        {
                                            pDr[pColName] = ado01;
                                            pDr["add02_c"] = BoAdm.OfGetAdo02(ado01);
                                        }
                                        else
                                        {
                                            var drInsert = TabDetailList[0].DtSource.NewRow();
                                            WfSetDetailRowDefault(0, drInsert);
                                            drInsert[pColName] = ado01;
                                            drInsert["add02_c"] = BoAdm.OfGetAdo02(ado01);
                                            TabDetailList[0].DtSource.Rows.Add(drInsert);

                                        }
                                    }
                                }
                            }
                            break;
                            #endregion

                        case "add13":   //可執行功能
                            var adoModel = BoAdm.OfGetAdoModel(detailModel.add02);
                            if (adoModel == null)
                                return false;

                            var adqList = BoAdm.OfGetAdqList(adoModel.ado01);

                            if (adqList == null)
                            {
                                WfShowErrorMsg("程式無任何可執行功能");
                                return false;
                            }

                            using (YR.ERP.Forms.pick.PickAdd1 frmPick =
                                    new YR.ERP.Forms.pick.PickAdd1(adqList, GlobalFn.isNullRet(detailModel.add13, ""), LoginInfo))
                            {
                                messageModel.IntMaxRow = 999;
                                messageModel.IsAutoQuery = true;
                                messageModel.StrMultiColumn = "action";
                                frmPick.MsgInfoReturned = messageModel;
                                frmPick.ShowDialog(this);

                                if (frmPick.MsgInfoReturned.Result == System.Windows.Forms.DialogResult.OK
                                         && frmPick.MsgInfoReturned != null)
                                {
                                    pDr["add13"] = frmPick.MsgInfoReturned.StrMultiRtn;
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

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
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
                    case "vw_admi110s":
                        pDr["add03"] = "Y";
                        pDr["add04"] = "Y";
                        pDr["add05"] = "Y";
                        pDr["add06"] = "Y";
                        pDr["add07"] = "Y";
                        pDr["add08"] = "Y";
                        pDr["add09"] = "Y";
                        pDr["add10"] = "Y";
                        pDr["add11"] = "Y";
                        pDr["add12"] = "Y";
                        pDr["add13"] = "";
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
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            List<vw_admi110s> admi110sList;
            int iChkCnts = 0;
            try
            {
                #region 單頭-pick vw_admi110
                if (e.Row.Table.Prefix.ToLower() == "vw_admi110")
                {
                    switch (e.Column.ToLower())
                    {
                        case "adc01":
                            //if ((pNewValue == null ? "" : pNewValue).Length > 5)
                            //{
                            //    pdr["ada02"] = 456;
                            //    WfShowMsg("字串長度不可大於5!");
                            //    return false;
                            //}
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_admi110s
                if (e.Row.Table.Prefix.ToLower() == "vw_admi110s")
                {
                    admi110sList = e.Row.Table.ToList<vw_admi110s>();
                    switch (e.Column.ToLower())
                    {
                        case "add02":
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["add02_c"] = "";
                                return true;
                            }
                            if (BoAdm.OfChkAdoPKExists(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此程式編號!");
                                return false;
                            }
                            iChkCnts = admi110sList.Where(p => p.add02 == e.Value.ToString()).Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("該程式已在,請檢核!");
                                return false;
                            }

                            e.Row["add02_c"] = BoAdm.OfGetAdo02(e.Value.ToString());
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
                        DrMaster["adcsecu"] = LoginInfo.UserNo;
                        DrMaster["adcsecg"] = LoginInfo.GroupNo;
                        DrMaster["adccreu"] = LoginInfo.UserNo;
                        DrMaster["adccreg"] = LoginInfo.DeptNo;
                        DrMaster["adccred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["adcmodu"] = LoginInfo.UserNo;
                        DrMaster["adcmodg"] = LoginInfo.DeptNo;
                        DrMaster["adcmodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["addcreu"] = LoginInfo.UserNo;
                            drDetail["addcreg"] = LoginInfo.DeptNo;
                            drDetail["addcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["addmodu"] = LoginInfo.UserNo;
                            drDetail["addmodg"] = LoginInfo.DeptNo;
                            drDetail["addmodd"] = Now;
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
