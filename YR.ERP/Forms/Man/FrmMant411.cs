/* 程式名稱: 託工進貨單維護作業
   系統代號: mant411
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
using YR.ERP.DAL.YRModel;
using YR.Util;
using YR.ERP.BLL.Model;
using System.Windows.Forms;
using System.Text;
using Infragistics.Win.UltraWinToolbars;
using YR.ERP.Shared;

namespace YR.ERP.Forms.Man
{
    public partial class FrmMant411 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region property
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;
        PurBLL BoPur = null;
        ManBLL BoMan = null;

        baa_tb BaaTbModel = null;
        bek_tb BekTbModel = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmMant411()
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
            this.StrFormID = "mant411";
            uTab_Header.Tabs[0].Text = "基本資料";
            uTab_Header.Tabs[1].Text = "狀態";

            this.IntTabCount = 2;
            this.IntMasterGridPos = 2;
            uTab_Master.Tabs[0].Text = "基本資料";
            uTab_Master.Tabs[1].Text = "資料瀏覽";

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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("mha01", SqlDbType.NVarChar) });
                //TabMaster.UserColumn = "seasecu";
                //TabMaster.GroupColumn = "seasecg";

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
            BoPur = new PurBLL(BoMaster.OfGetConntion());
            BoMan = new ManBLL(BoMaster.OfGetConntion());
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
                    BoPur.TRAN = BoMaster.TRAN;
                    BoMan.TRAN = BoMaster.TRAN;
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
                //課稅別
                sourceList = BoPur.OfGetTaxTypeKVPList();
                WfSetUcomboxDataSource(ucb_mha06, sourceList);

                //發票聯數
                sourceList = BoPur.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_mha09, sourceList);

                //單據確認
                sourceList = BoMan.OfGetMhaconfKVPList();
                WfSetUcomboxDataSource(ucb_mhaconf, sourceList);
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
            this.TabDetailList[0].TargetTable = "mhb_tb";
            this.TabDetailList[0].ViewTable = "vw_mant411s";
            keyParm = new SqlParameter("mhb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "mha01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_mant411 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_mant411>();
                    WfSetDocPicture("", masterModel.mhaconf, "", pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.mha10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekTbModel = BoBas.OfGetBekModel(masterModel.mha10);
                        if (BekTbModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.mha10));
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

                    WfSetControlReadonly(new List<Control> { ute_mhacreu, ute_mhacreg, udt_mhacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_mhamodu, ute_mhamodg, udt_mhamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_mhasecu, ute_mhasecg }, true);

                    if (GlobalFn.isNullRet(masterModel.mha01, "") != "")
                        WfSetControlReadonly(ute_mha01, true);

                    WfSetControlReadonly(new List<Control> { ute_mha01_c, ute_mha03_c, ute_mha04_c, ute_mha05_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_mha07, ucx_mha08, ute_mha10 }, true);
                    WfSetControlReadonly(new List<Control> { ute_mha11_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_mha13, ute_mha13t, ute_mha13g }, true);
                    WfSetControlReadonly(new List<Control> { ucb_mhaconf, udt_mhacond, ute_mhaconu }, true);
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_mha01, true);
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
                            if (columnName == "mhb05" ||
                                columnName == "mhb07" ||
                                columnName == "mhb08" ||
                                columnName == "mhb09" ||
                                columnName == "mhb11"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "mhb02")
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
                uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                SelectNextControl(this.uTab_Header, true, true, true, false);
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
                pDr["mha00"] = "2";
                pDr["mha02"] = Today;
                pDr["mha04"] = LoginInfo.UserNo;
                pDr["mha04_c"] = LoginInfo.UserName;
                pDr["mha05"] = LoginInfo.DeptNo;
                pDr["mha05_c"] = LoginInfo.DeptName;
                pDr["mha06"] = "1"; //課稅別預設含稅
                WfSetMha06Relation("1");
                pDr["mha10"] = BaaTbModel.baa04;
                pDr["mha13"] = 0;
                pDr["mha13t"] = 0;
                pDr["mha13g"] = 0;
                pDr["mhaconf"] = "N";
                pDr["mhacomp"] = LoginInfo.CompNo;
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
                        pDr["mhb02"] = WfGetMaxSeq(pDr.Table, "mhb02");
                        pDr["mhb05"] = 0;
                        //來源項次目前均預帶單頭的資料
                        pDr["mhb10"] = DrMaster["mha06"];
                        pDr["mhbcomp"] = LoginInfo.CompNo;
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

        #region WfPreInInsertModeCheck 進新增模式前的檢查及清變數與設定變數
        protected override bool WfPreInInsertModeCheck()
        {
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
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
            vw_mant411 masterModel;
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_mant411>();
                if (masterModel.mhaconf != "N")
                    return false;
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
            vw_mant411 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_mant411>();
                if (masterModel.mhaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認,不可刪除!");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfPreInsertDetailCheck() :新增明細資料前檢查
        protected override bool WfPreInsertDetailCheck(int pCurTabDetail)
        {
            try
            {
                if (WfFormCheck() == false)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfAfterDetailDelete() :刪除明細後調用
        protected override bool WfAfterDetailDelete(int pCurTabDetail, DataRow pDr)
        {
            try
            {
                switch (pDr.Table.Prefix.ToLower())
                {
                    case "vw_mant411s":
                        WfSetTotalAmt();
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

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pD.r)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                MessageInfo messageModel = new MessageInfo();
                vw_mant411 masterModel = null;
                vw_mant411s detailModel = null;

                masterModel = DrMaster.ToItem<vw_mant411>();
                #region 單頭-pick vw_mant411
                if (pDr.Table.Prefix.ToLower() == "vw_mant411")
                {
                    switch (pColName.ToLower())
                    {
                        case "mha01"://進貨單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "man"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "24"));
                            WfShowPickUtility("p_bab1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "mha03"://廠商編號
                            WfShowPickUtility("p_pca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pca01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "mha04"://經辦人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "mha05"://經辦部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "mha11"://付款條件
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@bef01", "1"));
                            WfShowPickUtility("p_bef1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bef02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                            break;

                    }
                }
                #endregion

                #region 單身-pick vw_mant411s
                if (pDr.Table.Prefix.ToLower() == "vw_mant411s")
                {
                    detailModel = pDr.ToItem<vw_mant411s>();
                    switch (pColName.ToLower())
                    {
                        case "mhb07"://入庫倉
                            WfShowPickUtility("p_icb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "mhb11"://來源託工單挑選
                            //todo:先抓全部之後再控卡
                            if (GlobalFn.isNullRet(masterModel.mha03, "") != "")
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.StrWhereAppend = " AND mea03=@mea03";
                                messageModel.StrWhereAppend = " AND (mea22-(mea26-mea27))>0";
                                messageModel.ParamSearchList.Add(new SqlParameter("@mea03", masterModel.mha03));
                            }

                            WfShowPickUtility("p_mea2", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["mea01"], "");
                                else
                                    pDr[pColName] = "";
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
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int iChkCnts = 0;
            vw_mant411 masterModel = null;
            vw_mant411s detailModel = null;
            List<vw_mant411s> detailList = null;
            bab_tb babModel = null;
            UltraGrid uGrid = null;
            int ChkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant411>();
                if (e.Column.ToLower() != "mha01" && GlobalFn.isNullRet(DrMaster["mha01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false; ;
                }
                #region 單頭- vw_mant411
                if (e.Row.Table.Prefix.ToLower() == "vw_mant411")
                {
                    switch (e.Column.ToLower())
                    {
                        case "mha01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "man", "24") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["mha01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            WfSetMha01RelReadonly(GlobalFn.isNullRet(e.Value, ""));
                            break;
                        case "mha02":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            var result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(e.Value), BaaModel);
                            if (result.Success == false)
                            {
                                WfShowErrorMsg(result.Message);
                                return false;
                            }
                            break;

                        case "mha03"://廠商
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mha03_c"] = "";
                                return true;
                            }
                            if (BoPur.OfChkPcaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此廠商資料,請檢核!");
                                return false;
                            }
                            WfSetMha03Relation(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mha04"://經辦人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mha04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["mha04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mha05"://經辦部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mha05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["mha05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "mha06"://課稅別
                            WfSetMha06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByMha06();
                            break;


                        case "mha11"://付款條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["mha11_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBefPKValid("1", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此付款條件,請檢核!");
                                return false;
                            }
                            e.Row["mha11_c"] = BoBas.OfGetBef03("1", GlobalFn.isNullRet(e.Value, ""));
                            break;

                    }
                }
                #endregion

                #region 單身- vw_mant411s
                if (e.Row.Table.Prefix.ToLower() == "vw_mant411s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_mant411s>();
                    detailList = e.Row.Table.ToList<vw_mant411s>();
                    babModel = BoBas.OfGetBabModel(masterModel.mha01);

                    switch (e.Column.ToLower())
                    {
                        case "mhb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.mhb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;
                        case "mhb05"://數量
                            if (GlobalFn.varIsNull(detailModel.mhb11))
                            {
                                WfShowErrorMsg("請先輸入託工單號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["mhb11"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入數量!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["mhb05"]);
                                return false;
                            }
                            //檢查可輸入數量
                            if (WfChkMea22(e.Row, detailModel) == false)
                                return false;
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "mhb07"://入庫倉
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoInv.OfChkIcbPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此倉庫別,請確認!");
                                return false;
                            }
                            break;

                        case "mhb09":   //單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.mhb09 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.mhb09, BekTbModel.bek03);
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "mhb11"://託工單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["mhb03"] = "";//主件料號
                                e.Row["mhb04"] = "";//主件品名
                                e.Row["mhb05"] = 0;//入庫量
                                e.Row["mhb06"] = "";//單位
                                e.Row["mhb09"] = "";//單價
                                WfSetDetailAmt(e.Row);
                                WfSetTotalAmt();
                                return true;
                            }
                            iChkCnts = detailList.Where(x => x.mhb11 == e.Value.ToString())
                                               .Count();
                            if (iChkCnts >= 2)
                            {
                                WfShowErrorMsg("託工單號不可重覆!");
                                return false;

                            }

                            var meaModel = BoMan.OfGetMeaModel(e.Value.ToString());
                            if (meaModel == null)
                            {
                                WfShowErrorMsg("無此託工單號!");
                                return false;
                            }
                            if (meaModel.meaconf != "Y")
                            {
                                WfShowErrorMsg("單據非確認狀態!");
                                return false;
                            }
                            if (masterModel.mha03 != meaModel.mea03)
                            {
                                WfShowErrorMsg("廠商不同，請確認!");
                                return false;
                            }
                            if (masterModel.mha06 != meaModel.mea06)
                            {
                                WfShowErrorMsg("稅別不同，請確認!");
                                return false;
                            }


                            if ((meaModel.mea22 - (meaModel.mea26 - meaModel.mea27)) < 0)
                            {
                                WfShowErrorMsg("此託工單已無可入庫數量!");
                                return false;
                            }
                            e.Row["mhb03"] = meaModel.mea20;//主件料號
                            e.Row["mhb04"] = meaModel.mea21;//主件品名
                            e.Row["mhb05"] = meaModel.mea22 - (meaModel.mea26 - meaModel.mea27);//入庫量
                            e.Row["mhb06"] = meaModel.mea23;//單位
                            e.Row["mhb09"] = meaModel.mea13;//單價
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
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
            vw_mant411 masterModel = null;
            vw_mant411s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {

                masterModel = DrMaster.ToItem<vw_mant411>();
                if (!GlobalFn.varIsNull(masterModel.mha01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.mha01, ""));
                #region 單頭資料檢查
                chkColName = "mha01";       //託工進貨單號
                chkControl = ute_mha01;
                if (GlobalFn.varIsNull(masterModel.mha01))
                {
                    this.uTab_Header.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mha02";       //入庫日期
                chkControl = udt_mha02;
                if (GlobalFn.varIsNull(masterModel.mha02))
                {
                    this.uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.mha02), BaaModel);
                if (result.Success == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                chkColName = "mha03";       //廠商編號
                chkControl = ute_mha03;
                if (GlobalFn.varIsNull(masterModel.mha03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mha04";       //人員
                chkControl = ute_mha04;
                if (GlobalFn.varIsNull(masterModel.mha04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mha05";       //部門
                chkControl = ute_mha05;
                if (GlobalFn.varIsNull(masterModel.mha05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "mha06";       //課稅別
                chkControl = ucb_mha06;
                if (GlobalFn.varIsNull(masterModel.mha06))
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

                    detailModel = drTemp.ToItem<vw_mant411s>();
                    chkColName = "mhb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.mhb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    #region mhb11 託工單號檢查
                    chkColName = "mhb11";
                    if (GlobalFn.varIsNull(detailModel.mhb11))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    var meaModel = BoMan.OfGetMeaModel(detailModel.mhb11);
                    if (babModel == null)
                    {
                        WfShowErrorMsg("無此託工單號!");
                        return false;
                    }
                    if (meaModel.meaconf != "Y")
                    {
                        WfShowErrorMsg("單據非確認狀態!");
                        return false;
                    }
                    if (masterModel.mha03 != meaModel.mea03)
                    {
                        WfShowErrorMsg("廠商不同，請確認!");
                        return false;
                    }
                    if (masterModel.mha06 != meaModel.mea06)
                    {
                        WfShowErrorMsg("稅別不同，請確認!");
                        return false;
                    }
                    #endregion

                    chkColName = "mhb07";   //倉庫別
                    if (GlobalFn.varIsNull(detailModel.mhb07))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
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
            string mha01New, errMsg;
            vw_mant411 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant411>();
                if (FormEditMode == YREditType.新增)
                {
                    mha01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.mha01, ModuleType.man, (DateTime)masterModel.mha02, out mha01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["mha01"] = mha01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["mhasecu"] = LoginInfo.UserNo;
                        DrMaster["mhasecg"] = LoginInfo.GroupNo;
                        DrMaster["mhacreu"] = LoginInfo.UserNo;
                        DrMaster["mhacreg"] = LoginInfo.DeptNo;
                        DrMaster["mhacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["mhamodu"] = LoginInfo.UserNo;
                        DrMaster["mhamodg"] = LoginInfo.DeptNo;
                        DrMaster["mhamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["mhbcreu"] = LoginInfo.UserNo;
                            drDetail["mhbcreg"] = LoginInfo.DeptNo;
                            drDetail["mhbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["mhbmodu"] = LoginInfo.UserNo;
                            drDetail["mhbmodg"] = LoginInfo.DeptNo;
                            drDetail["mhbmodd"] = Now;
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

        #region WfAddAction 新增action按鈕
        protected override List<ButtonTool> WfAddAction()
        {
            List<ButtonTool> buttonList = new List<ButtonTool>();
            ButtonTool bt;
            ado_tb adoModel;
            try
            {
                bt = new ButtonTool("Confirm");
                bt.SharedProps.Caption = "確認";
                bt.SharedProps.Category = "Confirm";
                buttonList.Add(bt);

                bt = new ButtonTool("CancelConfirm");
                bt.SharedProps.Caption = "取消確認";
                bt.SharedProps.Category = "CancelConfirm";
                buttonList.Add(bt);

                bt = new ButtonTool("Invalid");
                bt.SharedProps.Caption = "作廢";
                bt.SharedProps.Category = "Invalid";
                buttonList.Add(bt);


                bt = new ButtonTool("Manr411");
                adoModel = BoAdm.OfGetAdoModel("manr411");
                bt.SharedProps.Caption = adoModel.ado02;
                bt.SharedProps.Category = "Report";
                bt.Tag = adoModel;
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
            try
            {
                switch (pActionName)
                {
                    case "Confirm":
                        if (FormEditMode != YREditType.NA)
                            return;
                        WfConfirm();
                        break;
                    case "CancelConfirm":
                        if (FormEditMode != YREditType.NA)
                            return;
                        WfCancelConfirm();
                        break;
                    case "Invalid":
                        if (FormEditMode != YREditType.NA)
                            return;
                        WfInvalid();
                        break;

                    case "Manr411":
                        vw_manr411 queryModel;
                        vw_mant411 masterModel;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        masterModel = DrMaster.ToItem<vw_mant411>();
                        queryModel = new vw_manr411();
                        queryModel.mha01 = masterModel.mha01;
                        queryModel.mha03 = "";
                        queryModel.jump_yn = "N";
                        queryModel.order_by = "1";

                        FrmManr411 rpt = new FrmManr411(this.LoginInfo, queryModel, true, true);
                        rpt.WindowState = FormWindowState.Minimized;
                        rpt.ShowInTaskbar = false;
                        rpt.Show();

                        break;
                }
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
            DataTable dtIcc;
            DataRow drIcc;
            CommonBLL boAppendIcc;
            StringBuilder sbSql;
            vw_mant411s detailModel = null;
            try
            {
                boAppendIcc = new InvBLL(BoMaster.OfGetConntion()); //新增料號庫存明細資料
                boAppendIcc.TRAN = BoMaster.TRAN;
                boAppendIcc.OfCreateDao("icc_tb", "*", "");
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM icc_tb");
                sbSql.AppendLine("WHERE 1<>1");
                dtIcc = boAppendIcc.OfGetDataTable(sbSql.ToString());

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = dr.ToItem<vw_mant411s>();
                    if (BoInv.OfChkIccPKExists(detailModel.mhb03, detailModel.mhb07) == false)
                    {
                        if (dtIcc.Rows.Count > 0)
                        {
                            var drIccs = dtIcc.Select(string.Format("icc01='{0}' AND icc02='{1}'", detailModel.mhb03, detailModel.mhb07));
                            if (drIccs != null && drIccs.Length > 0)
                                continue;
                        }
                        drIcc = dtIcc.NewRow();
                        drIcc["icc01"] = detailModel.mhb03;  //料號
                        drIcc["icc02"] = detailModel.mhb07;
                        drIcc["icc03"] = "";
                        drIcc["icc04"] = detailModel.mhb06; //庫存單位
                        drIcc["icc05"] = 0;
                        dtIcc.Rows.Add(drIcc);
                    }
                }

                if (dtIcc.Rows.Count > 0)
                {
                    if (boAppendIcc.OfUpdate(dtIcc) < 1)
                    {
                        WfShowErrorMsg("新增料號庫存明細檔(icc_tb)失敗!");
                        return false;
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

        //*****************************表單自訂Fuction****************************************
        #region WfSetMha01RelReadonly 依單別設定相關控制項 readonly
        private void WfSetMha01RelReadonly(string pMha01)
        {
            bab_tb babModel = null;
            string bab01;
            try
            {
                WfSetControlReadonly(new List<Control> { ute_mha01 }, true);
                //bab01 = pMea01.Substring(0, GlobalFn.isNullRet(BaaTbModel.baa06, 0));
                //babModel = BoBas.OfGetBabModel(bab01);
                //if (babModel.bab08 == "Y")   //有來源單據
                //{
                //    WfSetControlReadonly(new List<Control> { ute_pga12 }, true);
                //    WfSetControlReadonly(new List<Control> { ute_pga16 }, false);
                //}
                //else
                //{
                //    WfSetControlReadonly(new List<Control> { ute_pga12 }, false);
                //    WfSetControlReadonly(new List<Control> { ute_pga16 }, true);

                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMha03Relation 設定廠商相關聯
        private void WfSetMha03Relation(string pMha03)
        {
            pca_tb pcaModel;
            bab_tb babModel = null;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["mha01"], ""));
                pcaModel = BoPur.OfGetPcaModel(pMha03);
                if (pcaModel == null)
                    return;

                DrMaster["mha03_c"] = pcaModel.pca03;
                DrMaster["mha06"] = pcaModel.pca22;    //課稅別
                WfSetMha06Relation(pcaModel.pca22);
                DrMaster["mha09"] = pcaModel.pca23;    //發票聯數
                DrMaster["mha11"] = pcaModel.pca21;    //付款條件
                DrMaster["mha11_c"] = BoBas.OfGetBef03("1", pcaModel.pca21);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMha06Relation 設定稅別關聯
        private void WfSetMha06Relation(string pMha06)
        {
            try
            {
                if (pMha06 == "1")
                {
                    DrMaster["mha07"] = BaaTbModel.baa05;
                    DrMaster["mha08"] = "Y";
                }
                else if (pMha06 == "2")
                {
                    DrMaster["mha07"] = BaaTbModel.baa05;
                    DrMaster["mha08"] = "N";
                }
                else
                {
                    DrMaster["mha07"] = 0;
                    DrMaster["mha08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkMea22 數量檢查--入庫數量是否大於預計生產數量
        //目前先檢查是否有超出託工單的預計數量,後續要在單別加入是否可超出的檢查
        private bool WfChkMea22(DataRow pdr, vw_mant411s pListDetail)
        {
            List<vw_mant411s> detailList = null;
            mea_tb meaModel = null;
            bab_tb babModel = null;
            string errMsg;
            decimal docThisQty = 0;         //本項次的轉換送料單數量
            decimal docOtherQtyTotal = 0;   //本單據其他項次的送料單數量加總
            //decimal otherQtyTotal = 0;      //其他單據的數量加總
            decimal maxAvailableQty = 0;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                detailList = pdr.Table.ToList<vw_mant411s>();
                if (GlobalFn.varIsNull(pListDetail.mhb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.mhb11))
                {
                    errMsg = "請先輸入託工單號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }


                if (pListDetail.mhb05 <= 0)
                {
                    errMsg = "入庫量應大於0!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["mha01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                meaModel = BoMan.OfGetMeaModel(pListDetail.mhb11);
                if (babModel == null)
                {
                    errMsg = "Get meb_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                //取得可輸入數量加總
                maxAvailableQty = meaModel.mea22 - (meaModel.mea26 - meaModel.mea27);

                //先取本身這筆
                docThisQty = pListDetail.mhb05;
                //再取其他筆加總
                docOtherQtyTotal = detailList.Where(p => p.mhb02 != pListDetail.mhb02)
                                   .Where(p => p.mhb11 == pListDetail.mhb11)
                                   .Sum(p => p.mhb05);
                if ((maxAvailableQty - (docThisQty + docOtherQtyTotal)) < 0)
                {
                    errMsg = string.Format("項次{0} 最大可輸入數量為 {1}",
                                            pListDetail.mhb02.ToString(),
                                            (maxAvailableQty - docOtherQtyTotal).ToString()
                                           );
                    WfShowErrorMsg(errMsg);
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

        #region WfChkMea26Mea27 數量檢查--取消確認後入庫數量是否小於退庫量
        //目前先檢查是否有超出託工單的預計數量,後續要在單別加入是否可超出的檢查
        private bool WfChkMea26Mea27(DataRow pdr, vw_mant411s pDetailModel)
        {
            List<vw_mant411s> detailList = null;
            mea_tb meaModel = null;
            bab_tb babModel = null;
            string errMsg;
            decimal docThisQty = 0;         //本項次送料單數量
            decimal docOtherQtyTotal = 0;   //本單據其他項次的送料單數量加總
            decimal otherQtyTotal = 0;      //其他單據的數量加總
            decimal maxAvailableQty = 0;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                detailList = pdr.Table.ToList<vw_mant411s>();
                if (GlobalFn.varIsNull(pDetailModel.mhb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pDetailModel.mhb11))
                {
                    errMsg = "請先輸入託工單號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }


                if (pDetailModel.mhb05 <= 0)
                {
                    errMsg = "入庫量應大於0!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["mha01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                meaModel = BoMan.OfGetMeaModel(pDetailModel.mhb11);
                if (babModel == null)
                {
                    errMsg = "Get meb_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                //取得其他單據數量加總
                //sbSql = new StringBuilder();
                //sbSql.AppendLine("SELECT sum(mhb05) FROM mha_tb");
                //sbSql.AppendLine("  INNER JOIN mhb_tb ON mha01=mhb01");
                //sbSql.AppendLine("WHERE mhaconf='Y' ");
                //sbSql.AppendLine("AND mha01<>@mha01 ");
                //sbSql.AppendLine("AND mha11=@mha11 ");
                //sqlParmList = new List<SqlParameter>();
                //sqlParmList.Add(new SqlParameter("mha01", pDetailModel.mhb01));
                //sqlParmList.Add(new SqlParameter("mha11", pDetailModel.mhb11));
                //otherQtyTotal = GlobalFn.isNullRet(BoMan.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()),0);


                //取得可輸入數量加總(入庫量-退庫量)
                maxAvailableQty = meaModel.mea26 - meaModel.mea27;

                //先取本身這筆
                docThisQty = pDetailModel.mhb05;
                //再取其他筆加總
                docOtherQtyTotal = detailList.Where(p => p.mhb02 != pDetailModel.mhb02)
                                   .Where(p => p.mhb11 == pDetailModel.mhb11)
                                   .Sum(p => p.mhb05);

                if ((maxAvailableQty - (docThisQty + docOtherQtyTotal) < 0))
                {
                    errMsg = string.Format("項次{0} 最大可輸入數量為 {1}",
                                            pDetailModel.mhb02.ToString(),
                                            (maxAvailableQty - docOtherQtyTotal).ToString()
                                           );
                    WfShowErrorMsg(errMsg);
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

        #region WfSetDetailAmt 處理小計
        private bool WfSetDetailAmt(DataRow drMhb)
        {
            mha_tb mhaModel;
            mhb_tb mhbModel;
            decimal mhb10t = 0, mhb10 = 0;
            try
            {
                mhaModel = DrMaster.ToItem<mha_tb>();
                mhbModel = drMhb.ToItem<mhb_tb>();

                if (mhaModel.mha08 == "Y")//稅內含
                {
                    mhb10t = mhbModel.mhb09 * mhbModel.mhb05;
                    mhb10t = GlobalFn.Round(mhb10t, BekTbModel.bek04);
                    mhb10 = mhb10t / (1 + (mhaModel.mha07 / 100));
                    mhb10 = GlobalFn.Round(mhb10, BekTbModel.bek04);
                }
                else//稅外加
                {
                    mhb10 = mhbModel.mhb09 * mhbModel.mhb05;
                    mhb10 = GlobalFn.Round(mhb10, BekTbModel.bek04);
                    mhb10t = mhb10 * (1 + (mhaModel.mha07 / 100));
                    mhb10t = GlobalFn.Round(mhb10t, BekTbModel.bek04);
                }
                drMhb["mhb10"] = mhb10;
                drMhb["mhb10t"] = mhb10t;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetTotalAmt 處理總計
        private bool WfSetTotalAmt()
        {
            mha_tb mhaModel;
            decimal mha13 = 0, mha13t = 0, mha13g;
            try
            {
                mhaModel = DrMaster.ToItem<mha_tb>();
                if (mhaModel.mha08 == "Y")//稅內含,先處理含稅金額
                {
                    foreach (mhb_tb mhbModel in TabDetailList[0].DtSource.ToList<mhb_tb>())
                    {
                        mha13t += mhbModel.mhb10t;
                    }
                    mha13t = GlobalFn.Round(mha13t, BekTbModel.bek04);
                    mha13 = mha13t / (1 + mhaModel.mha07 / 100);
                    mha13 = GlobalFn.Round(mha13, BekTbModel.bek04);
                    mha13g = mha13t - mha13;

                }
                else//稅外加
                {
                    foreach (mhb_tb mhbModel in TabDetailList[0].DtSource.ToList<mhb_tb>())
                    {
                        mha13 += mhbModel.mhb10;
                    }
                    mha13 = GlobalFn.Round(mha13, BekTbModel.bek04);
                    mha13g = mha13 * (mhaModel.mha07 / 100);
                    mha13g = GlobalFn.Round(mha13g, BekTbModel.bek04);
                    mha13t = mha13 + mha13g;
                }

                DrMaster["mha13"] = mha13;
                DrMaster["mha13t"] = mha13t;
                DrMaster["mha13g"] = mha13g;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfResetAmtByMha06 依稅別更新單身及單頭的金額
        private void WfResetAmtByMha06()
        {
            try
            {
                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    WfSetDetailAmt(dr);
                    WfSetTotalAmt();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfConfirm 確認
        private void WfConfirm()
        {
            mha_tb mhaModel = null;
            mhb_tb mhbModel = null;
            string errMsg;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();

                mhaModel = DrMaster.ToItem<mha_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }

                //檢查後更改單頭狀態,讓後面可以直接使用
                mhaModel.mhaconf = "Y";
                mhaModel.mhacond = Today;
                mhaModel.mhaconu = LoginInfo.UserNo;

                if (WfUpdMea(true) == false)
                {
                    DrMaster.RejectChanges();
                    WfRollback();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    mhbModel = dr.ToItem<mhb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("1", mhbModel.mhb03, mhbModel.mhb07, mhbModel.mhb05, out errMsg) == false)//入庫
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }

                    //更新庫存交易歷史檔 
                    if (BoInv.OfStockPost("mant411", mhaModel, mhbModel, LoginInfo, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                }

                DrMaster["mhaconf"] = "Y";
                DrMaster["mhacond"] = Today;
                DrMaster["mhaconu"] = LoginInfo.UserNo;
                DrMaster["mhamodu"] = LoginInfo.UserNo;
                DrMaster["mhamodg"] = LoginInfo.DeptNo;
                DrMaster["mhamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                mhaModel = DrMaster.ToItem<mha_tb>();
                WfSetDocPicture("", mhaModel.mhaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfConfirmChk 確認前檢查
        private bool WfConfirmChk()
        {
            vw_mant411 masterModel = null;
            vw_mant411s detailModel = null;
            List<vw_mant411s> detailList = null;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant411>();
                if (masterModel.mhaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    WfRollback();
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.mha02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_mant411s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_mant411s>();
                    if (WfChkMea22(drTemp, detailModel) == false)
                    {
                        WfRollback();
                        return false;
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

        #region WfCancelConfirm 取消確認
        private void WfCancelConfirm()
        {
            mha_tb mhaModel = null;
            mhb_tb mhbModel = null;
            string errMsg;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)        //這裡會LOCK資料
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;

                WfSetBllTransaction();
                mhaModel = DrMaster.ToItem<mha_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    mhbModel = dr.ToItem<mhb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("2", mhbModel.mhb03, mhbModel.mhb07, mhbModel.mhb05, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                    //刪除庫存交易歷史檔
                    if (BoInv.OfDelIna(mhbModel.mhb01, mhbModel.mhb02, "1", out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                }
                if (WfUpdMea(false) == false)
                {
                    DrMaster.RejectChanges();
                    WfRollback();
                    return;
                }

                DrMaster["mhaconf"] = "N";
                DrMaster["mhacond"] = DBNull.Value;
                DrMaster["mhaconu"] = "";
                DrMaster["mhamodu"] = LoginInfo.UserNo;
                DrMaster["mhamodg"] = LoginInfo.DeptNo;
                DrMaster["mhamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                mhaModel = DrMaster.ToItem<mha_tb>();
                WfSetDocPicture("", mhaModel.mhaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfCancelConfirmChk 取消確認前檢查
        private bool WfCancelConfirmChk()
        {
            vw_mant411 masterModel = null;
            vw_mant411s detailModel = null;
            List<vw_mant411s> detailList = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            List<mhb_tb> mhbList = null;    //檢查庫存
            int iChkCnts = 0;
            decimal icc05;
            mea_tb meaModel = null;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_mant411>();
                if (masterModel.mhaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.mha02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_mant411s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_mant411s>();
                    if (WfChkMea26Mea27(drTemp, detailModel) == false)
                    {
                        return false;
                    }
                    meaModel = BoMan.OfGetMeaModel(detailModel.mhb11);
                    if (meaModel.meastat == "8")
                    {
                        WfShowErrorMsg("託工單已指定結案，不可取消確認!");
                        return false;
                    }
                }

                //檢查入庫單的數量是否足夠做庫存還原
                mhbList = TabDetailList[0].DtSource.ToList<mhb_tb>();
                var sumMhbList =                         //依入庫單的料號及倉庫做加總
                        from mhb in mhbList
                        where mhb.mhb05 > 0
                        group mhb by new { mhb.mhb03, mhb.mhb07 } into sumMhbModel
                        select new
                        {
                            mhb03 = sumMhbModel.Key.mhb03,
                            mhb07 = sumMhbModel.Key.mhb07,
                            mhb05 = sumMhbModel.Sum(p => p.mhb05)
                        }
                    ;
                foreach (var sumMhbModel in sumMhbList)
                {
                    icc05 = BoInv.OfGetIcc05(sumMhbModel.mhb03, sumMhbModel.mhb07);
                    if (icc05 < sumMhbModel.mhb05)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足扣量!", sumMhbModel.mhb03, icc05));
                        return false;
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

        #region WfInvalid 作廢/作廢還原
        private void WfInvalid()
        {
            vw_mant411 masterModel = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChkCnts = 0;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_mant411>();

                if (masterModel.mhaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.mhaconf == "N")//走作廢
                {

                    DrMaster["mhaconf"] = "X";
                    DrMaster["mhaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.mhaconf == "X")
                {
                    DrMaster["mhaconf"] = "N";
                    DrMaster["mhaconu"] = "";
                }

                DrMaster["mhamodu"] = LoginInfo.UserNo;
                DrMaster["mhamodg"] = LoginInfo.DeptNo;
                DrMaster["mhamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_mant411>();
                WfSetDocPicture("", masterModel.mhaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfUpdMea 更新託工單
        /* 確認及取消確認
         * 1.入庫數量 
         * 2.狀態
         */
        private bool WfUpdMea(bool pbConfirm)
        {
            bab_tb babModel = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmsList;
            mea_tb meaBeforeModel = null;   //取得更新前託工單資料
            mea_tb meaAfterModel = null;   //取得更新後託工單資料
            DataTable dtMhbDistinct = null;
            string mhb11;
            decimal docQty = 0, otherDocQty = 0;
            int chkMebCnts = 0; //檢查是否有送料單
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["mha01"], ""));
                if (babModel == null)
                {
                    WfShowErrorMsg("Get bab_tb Error!");
                    return false;
                }
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT DISTINCT mhb11,SUM(mhb05) mhb05");
                sbSql.AppendLine("FROM mhb_tb");
                sbSql.AppendLine("WHERE mhb01=@mhb01");
                sbSql.AppendLine("  AND ISNULL(mhb11,'')<>''");
                sbSql.AppendLine("GROUP BY mhb11");
                sqlParmsList = new List<SqlParameter>();
                sqlParmsList.Add(new SqlParameter("@mhb01", GlobalFn.isNullRet(DrMaster["mha01"], "")));
                dtMhbDistinct = BoMan.OfGetDataTable(sbSql.ToString(), sqlParmsList.ToArray());
                if (dtMhbDistinct == null)
                    return true;
                foreach (DataRow dr in dtMhbDistinct.Rows)
                {
                    mhb11 = dr["mhb11"].ToString();
                    docQty = Convert.ToDecimal(dr["mhb05"]);
                    meaBeforeModel = BoMan.OfGetMeaModel(mhb11);    //取得更新前託工單資料

                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT SUM(mhb05) FROM mha_tb");
                    sbSql.AppendLine("  INNER JOIN mhb_tb ON mha01=mhb01");
                    sbSql.AppendLine("WHERE mhaconf='Y'");
                    sbSql.AppendLine("AND mhb11=@mhb11 ");
                    sbSql.AppendLine("AND mha01<>@mha01");


                    sqlParmsList = new List<SqlParameter>();
                    sqlParmsList.Add(new SqlParameter("@mhb11", mhb11));
                    sqlParmsList.Add(new SqlParameter("@mha01", GlobalFn.isNullRet(DrMaster["mha01"], "")));
                    otherDocQty = 0;
                    otherDocQty = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParmsList.ToArray()), 0);

                    sbSql = new StringBuilder();
                    sbSql = sbSql.AppendLine("UPDATE mea_tb");
                    sbSql = sbSql.AppendLine("SET mea26=@mea26");
                    sbSql = sbSql.AppendLine("WHERE mea01=@mea01");
                    sqlParmsList = new List<SqlParameter>();
                    sqlParmsList.Add(new SqlParameter("@mea01", mhb11));
                    if (pbConfirm)  //確認的要加本身單據
                        sqlParmsList.Add(new SqlParameter("@mea26", docQty + otherDocQty));
                    else
                        sqlParmsList.Add(new SqlParameter("@mea26", otherDocQty));

                    if (BoPur.OfExecuteNonquery(sbSql.ToString(), sqlParmsList.ToArray()) != 1)
                    {
                        WfShowErrorMsg("更新託工單實際入庫數量失敗!");
                        return false;
                    }
                    meaAfterModel = BoMan.OfGetMeaModel(mhb11);
                    #region 託工單狀態處理 meastat
                    if (pbConfirm)  //確認時
                    {
                        if (meaBeforeModel.meastat == "2" || meaBeforeModel.meastat == "3" || meaBeforeModel.meastat == "4")
                        {
                            if ((meaBeforeModel.meastat == "2" || meaBeforeModel.meastat == "3")
                                && ((meaAfterModel.mea22 - meaAfterModel.mea26) > 0)
                                )
                            {
                                if (WfUpdMeastat(mhb11, "4") == false) //仍有量時,改為生產中
                                    return false;
                            }
                            else if ((meaBeforeModel.meastat == "2" || meaBeforeModel.meastat == "3")
                                && ((meaAfterModel.mea22 - meaAfterModel.mea26) == 0)
                                )
                            {

                                if (WfUpdMeastat(mhb11, "9") == false) //無量時,改為結案
                                    return false;
                            }
                            else if ((meaBeforeModel.meastat == "4")
                                && ((meaAfterModel.mea22 - meaAfterModel.mea26) == 0)
                                )
                            {
                                if (WfUpdMeastat(mhb11, "9") == false) //無量時,改為結案
                                    return false;
                            }
                        }
                    }
                    else  //取消確認
                    {
                        //取消確認只取託工單更新後 (預計生產數量-實際生產數量)>=0
                        if ((meaAfterModel.mea22-meaAfterModel.mea26)>0)
                        {
                            sbSql = new StringBuilder();
                            sbSql.AppendLine("SELECT COUNT(1)");
                            sbSql.AppendLine("FROM meb_tb");
                            sbSql.AppendLine("WHERE meb01=@meb01");
                            sbSql.AppendLine("AND meb08>0");
                            sqlParmsList = new List<SqlParameter>();
                            sqlParmsList.Add(new SqlParameter("meb01", meaAfterModel.mea01));
                            chkMebCnts = GlobalFn.isNullRet(BoMan.OfGetFieldValue(sbSql.ToString(), sqlParmsList.ToArray()), 0);

                            if (meaAfterModel.mea26 > 0 && meaAfterModel.meastat == "9")
                            {
                                if (WfUpdMeastat(meaAfterModel.mea01, "4") == false)
                                    return false;
                            }
                            else if (meaAfterModel.mea26 == 0 && meaAfterModel.meastat == "9")
                            {
                                if (chkMebCnts == 0)
                                {
                                    if (WfUpdMeastat(meaAfterModel.mea01, "2") == false)
                                        return false;
                                }
                                else
                                {
                                    if (WfUpdMeastat(meaAfterModel.mea01, "3") == false)
                                        return false;
                                }

                            }
                            else if (meaAfterModel.mea26 == 0 && meaAfterModel.meastat == "4")
                            {
                                if (chkMebCnts == 0)
                                {
                                    if (WfUpdMeastat(meaAfterModel.mea01, "2") == false)
                                        return false;
                                }
                                else
                                {
                                    if (WfUpdMeastat(meaAfterModel.mea01, "3") == false)
                                        return false;
                                }
                            }
                        }
                                            }
                    #endregion
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfUpdMea 更新託工單狀態碼
        private bool WfUpdMeastat(string pMea01, string pMeastat)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine(string.Format("UPDATE mea_tb"));
                sbSql.AppendLine(string.Format("SET meastat=@meastat"));
                sbSql.AppendLine(string.Format("WHERE mea01=@mea01"));
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@mea01", pMea01));
                sqlParmList.Add(new SqlParameter("@meastat", pMeastat));
                if (BoMan.OfExecuteNonquery(sbSql.ToString(), sqlParmList.ToArray()) <= 0)
                {
                    WfShowErrorMsg("更新託工單狀態碼，失敗!");
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
