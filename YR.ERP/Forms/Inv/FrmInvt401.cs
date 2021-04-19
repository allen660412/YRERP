/* 程式名稱: 借出單建立作業
   系統代號: invt401
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

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvt401 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        StpBLL BoStp = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;

        baa_tb BaaTbModel = null;
        bek_tb BekTbModel = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmInvt401()
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
            this.StrFormID = "invt401";
            uTab_Header.Tabs[0].Text = "基本資料";
            uTab_Header.Tabs[1].Text = "狀態";

            this.IntTabCount = 2;
            this.IntMasterGridPos = 2;
            uTab_Master.Tabs[0].Text = "資料內容";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("ila01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "ilasecu";
                TabMaster.GroupColumn = "ilasecg";

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
                    BoStp.TRAN = BoMaster.TRAN;
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
                //課稅別
                sourceList = BoStp.OfGetTaxTypeKVPList();
                WfSetUcomboxDataSource(ucb_ila06, sourceList);

                //發票聯數
                sourceList = BoStp.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_ila09, sourceList);

                //單據確認
                sourceList = BoInv.OfGetIlaconfKVPList();
                WfSetUcomboxDataSource(ucb_ilaconf, sourceList);

                //單據狀態
                sourceList = BoInv.OfGetIlastatKVPList();
                WfSetUcomboxDataSource(ucb_ilastat, sourceList);
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

            this.TabDetailList[0].TargetTable = "ilb_tb";
            this.TabDetailList[0].ViewTable = "vw_invt401s";
            keyParm = new SqlParameter("ilb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "ila01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_invt401 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_invt401>();
                    WfSetDocPicture("", masterModel.ilaconf, masterModel.ilastat, pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.ila10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekTbModel = BoBas.OfGetBekModel(masterModel.ila10);
                        if (BekTbModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.ila10));
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
                    WfSetControlReadonly(new List<Control> { ute_ilacreu, ute_ilacreg, udt_ilacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_ilamodu, ute_ilamodg, udt_ilamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_ilasecu, ute_ilasecg }, true);

                    if (GlobalFn.isNullRet(masterModel.ila01, "") != "")
                    {
                        WfSetControlReadonly(ute_ila01, true);
                        WfSetIla01RelReadonly(GlobalFn.isNullRet(masterModel.ila01, ""));
                    }
                    else
                        WfSetControlReadonly(ute_ila19, true);

                    WfSetControlReadonly(new List<Control> { ute_ila01_c, ute_ila03_c, ute_ila04_c, ute_ila05_c, ute_ila11_c, ute_ila12_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_ila07, ucx_ila08 }, true);
                    WfSetControlReadonly(new List<Control> { ute_ila13, ute_ila13t, ute_ila13g }, true);
                    WfSetControlReadonly(new List<Control> { ute_ila14_c, ute_ila15_c, ute_ila16_c }, true);
                    WfSetControlReadonly(new List<Control> { ucb_ilaconf, ude_ilacond, ute_ilaconu, ucb_ilastat }, true);
                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_ila01, true);
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
                            if (
                                columnName == "ilb03" ||
                                columnName == "ilb05" ||
                                columnName == "ilb09" ||
                                columnName == "ilb11" ||
                                columnName == "ilb12" ||
                                columnName == "ilb13" ||
                                columnName == "ilb14"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "ilb02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
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

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                vw_invt401 masterModel = null;
                vw_invt401s detailModel = null;
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_invt401
                if (pDr.Table.Prefix.ToLower() == "vw_invt401")
                {
                    switch (pColName.ToLower())
                    {
                        case "ila01"://借出單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "inv"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "30"));
                            WfShowPickUtility("p_bab1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "ila03"://客戶編號
                            WfShowPickUtility("p_sca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sca01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "ila04"://業務人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }

                            break;
                        case "ila05"://業務部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "ila10"://幣別
                            WfShowPickUtility("p_bek1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bek01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "ila11"://收款條件
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@bef01", "2"));
                            WfShowPickUtility("p_bef1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bef02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "ila12"://取價條件
                            WfShowPickUtility("p_sbb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "ila14"://貨運方式
                            WfShowPickUtility("p_bel1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bel01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "ila15"://貨運起點
                            WfShowPickUtility("p_sbg1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbg01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "ila16"://貨運終點
                            WfShowPickUtility("p_sbg1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbg01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                    }
                }
                #endregion

                #region 單身-pick vw_invt401s
                if (pDr.Table.Prefix.ToLower() == "vw_invt401s")
                {
                    masterModel = DrMaster.ToItem<vw_invt401>();
                    detailModel = pDr.ToItem<vw_invt401s>();
                    switch (pColName.ToLower())
                    {
                        case "ilb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "ilb06"://借出單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;


                        case "ilb11"://出庫倉
                            if (GlobalFn.isNullRet(detailModel.ilb03, "") == "")
                            {
                                WfShowPickUtility("p_icb1", messageModel);
                                if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                {
                                    if (messageModel.DataRowList.Count > 0)
                                        pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icb01"], "");
                                    else
                                        pDr[pColName] = "";
                                }
                            }
                            else
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.ilb03));
                                WfShowPickUtility("p_icc1", messageModel);
                                if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                {
                                    if (messageModel.DataRowList.Count > 0)
                                        pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icc02"], "");
                                    else
                                        pDr[pColName] = "";
                                }
                            }
                            break;

                        case "ilb12"://入庫倉
                            if (GlobalFn.isNullRet(detailModel.ilb03, "") == "")
                            {
                                WfShowPickUtility("p_icb1", messageModel);
                                if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                {
                                    if (messageModel.DataRowList.Count > 0)
                                        pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icb01"], "");
                                    else
                                        pDr[pColName] = "";
                                }
                            }
                            else
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.ilb03));
                                WfShowPickUtility("p_icc1", messageModel);
                                if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                {
                                    if (messageModel.DataRowList.Count > 0)
                                        pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icc02"], "");
                                    else
                                        pDr[pColName] = "";
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
                pDr["ila02"] = Today;
                pDr["ila04"] = LoginInfo.UserNo;
                pDr["ila04_c"] = LoginInfo.UserName;
                pDr["ila05"] = LoginInfo.DeptNo;
                pDr["ila05_c"] = LoginInfo.DeptName;
                pDr["ila07"] = 0;
                pDr["ila08"] = "N";
                pDr["ila10"] = BaaTbModel.baa04;
                pDr["ila13"] = 0;
                pDr["ila13t"] = 0;
                pDr["ila13g"] = 0;
                pDr["ila20"] = 1;       //匯率
                pDr["ilaconf"] = "N";
                pDr["ilastat"] = "0";
                pDr["ilacomp"] = LoginInfo.CompNo;
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
                        pDr["ilb02"] = WfGetMaxSeq(pDr.Table, "ilb02");
                        pDr["ilb05"] = 0;
                        pDr["ilb08"] = 0;
                        pDr["ilb09"] = 0;
                        pDr["ilb10"] = 0;
                        pDr["ilb10t"] = 0;
                        pDr["ilb15"] = 0;
                        pDr["ilb16"] = 0;
                        pDr["ilb17"] = 0;
                        pDr["ilbcomp"] = LoginInfo.CompNo;
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
            vw_invt401 masterModel;
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_invt401>();
                if (masterModel.ilaconf != "N")
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
            vw_invt401 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_invt401>();
                if (masterModel.ilaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認,不可作廢!");
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

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int iChkCnts = 0;
            vw_invt401 masterModel = null;
            vw_invt401s detailModel = null;
            List<vw_invt401s> detailList = null;
            bab_tb babModel = null;
            UltraGrid uGrid = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt401>();
                if (e.Column.ToLower() != "ila01" && GlobalFn.isNullRet(DrMaster["ila01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false;
                }
                #region 單頭-pick vw_invt401
                if (e.Row.Table.Prefix.ToLower() == "vw_invt401")
                {
                    switch (e.Column.ToLower())
                    {
                        case "ila01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "inv", "30") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["ila01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            WfSetIla01RelReadonly(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ila02"://借出日期
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;

                            var result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(e.Value), BaaModel);
                            if (result.Success == false)
                            {
                                WfShowErrorMsg(result.Message);
                                return false;
                            }
                            break;

                        case "ila03"://客戶
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["ila03_c"] = "";
                                //pDr["sga14"] = "";    //送貨地址
                                //pDr["sga15"] = "";    //帳單地址
                                return true;
                            }
                            if (BoStp.OfChkScaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此客戶資料,請檢核!");
                                return false;
                            }
                            WfSetIla03Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByIla06();
                            break;

                        case "ila04"://業務人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["ila04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["ila04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ila05"://業務部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["ila05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["ila05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ila06"://課稅別
                            WfSetIla06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByIla06();
                            break;

                        case "ila10"://幣別
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoBas.OfChkBekPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此幣別");
                                return false;
                            }
                            break;

                        case "ila11"://收款條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["ila11_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBefPKValid("2", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此收款條件,請檢核!");
                                return false;
                            }
                            e.Row["ila11_c"] = BoBas.OfGetBef03("1", GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ila12"://取價條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["ila12_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbbPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此取價條件,請檢核!");
                                return false;
                            }
                            e.Row["ila12_c"] = BoStp.OfGetSbb02(GlobalFn.isNullRet(e.Value, ""));
                            break;
                            
                        case "ila14"://貨運方式
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["ila14_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBelPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運方式,請檢核!");
                                return false;
                            }
                            e.Row["ila14_c"] = BoBas.OfGetBel02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ila15"://貨運起點
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["ila15_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbgPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運起點,請檢核!");
                                return false;
                            }
                            e.Row["ila15_c"] = BoStp.OfGetSbg02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ila16"://貨運終點
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["ila16_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbgPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運終點,請檢核!");
                                return false;
                            }
                            e.Row["ila16_c"] = BoStp.OfGetSbg02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "ila20": //匯率
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0m) <= 0)
                            {
                                WfShowErrorMsg("匯率應大於0,請檢核!");
                                return false;
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_invt401s
                if (e.Row.Table.Prefix.ToLower() == "vw_invt401s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_invt401s>();
                    detailList = e.Row.Table.ToList<vw_invt401s>();
                    babModel = BoBas.OfGetBabModel(masterModel.ila01);

                    switch (e.Column.ToLower())
                    {
                        case "ilb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            detailList = e.Row.Table.ToList<vw_invt401s>();
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.ilb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "ilb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["ilb04"] = "";//品名
                                e.Row["ilb05"] = 0;//借出數量
                                e.Row["ilb06"] = "";//借出單位
                                e.Row["ilb07"] = "";//庫存單位
                                e.Row["ilb08"] = 0;//庫存轉換率
                                e.Row["ilb17"] = 0;//庫存數量
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }

                            if (WfSetIlb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "ilb05"://借出數量
                            if (GlobalFn.varIsNull(detailModel.ilb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["ilb03"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(detailModel.ilb06))
                            {
                                WfShowErrorMsg("請先輸入借出單位!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["ilb06"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入借出數量!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["ilb05"]);
                                return false;
                            }
                            detailModel.ilb05 = BoBas.OfGetUnitRoundQty(detailModel.ilb06, detailModel.ilb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detailModel.ilb05;
                            if (WfChkIlb05(e.Row, detailModel) == false)
                                return false;
                            e.Row["ilb17"] = BoBas.OfGetUnitRoundQty(detailModel.ilb07, detailModel.ilb05 * detailModel.ilb08); //轉換庫存數量(並四拾伍入)
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "ilb06"://借出單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (GlobalFn.varIsNull(detailModel.ilb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["ilb03"]);
                                return false;
                            }
                            if (WfChkIlb06(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;

                            if (WfSetIlb06Relation(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;
                            break;

                        case "ilb09":   //單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.ilb09 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.ilb09, BekTbModel.bek03);
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "ilb11"://出庫倉
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

                        case "ilb12"://入庫倉
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (!GlobalFn.varIsNull(detailModel.ilb11) && detailModel.ilb11.ToUpper() == e.Value.ToString().ToUpper())
                            {
                                WfShowErrorMsg("出庫倉與入庫倉不可相同!");
                                return false;
                            }

                            if (BoInv.OfChkIcbPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此倉庫別,請確認!");
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_invt401 masterModel = null;
            vw_invt401s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {

                masterModel = DrMaster.ToItem<vw_invt401>();
                if (!GlobalFn.varIsNull(masterModel.ila01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.ila01, ""));
                #region 單頭資料檢查
                chkColName = "ila01";       //出貨單號
                chkControl = ute_ila01;
                if (GlobalFn.varIsNull(masterModel.ila01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ila02";       //單據日期
                chkControl = udt_ila02;
                if (GlobalFn.varIsNull(masterModel.ila02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.ila02), BaaModel);
                if (result.Success == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                chkColName = "ila03";       //客戶編號
                chkControl = ute_ila03;
                if (GlobalFn.varIsNull(masterModel.ila03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ila04";       //業務人員
                chkControl = ute_ila04;
                if (GlobalFn.varIsNull(masterModel.ila04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ila05";       //業務部門
                chkControl = ute_ila05;
                if (GlobalFn.varIsNull(masterModel.ila05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ila06";       //課稅別
                chkControl = ucb_ila06;
                if (GlobalFn.varIsNull(masterModel.ila06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ila10";       //幣別
                chkControl = ute_ila10;
                if (GlobalFn.varIsNull(masterModel.ila10))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ila12";       //取價條件
                chkControl = ute_ila12;
                if (GlobalFn.varIsNull(masterModel.ila12) && babModel.bab08 != "Y")//無來源單據取價條件要輸入
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

                    detailModel = drTemp.ToItem<vw_invt401s>();
                    chkColName = "ilb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.ilb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ilb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.ilb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ilb05";   //出庫數量
                    #region ilb05 出庫數量
                    if (GlobalFn.varIsNull(detailModel.ilb05) || detailModel.ilb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    if (WfChkIlb05(drTemp, detailModel) == false)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    chkColName = "ilb06";   //出庫單位
                    if (GlobalFn.varIsNull(detailModel.ilb06))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ilb11";   //出庫倉
                    if (GlobalFn.varIsNull(detailModel.ilb11))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "ilb12";   //入庫倉
                    if (GlobalFn.varIsNull(detailModel.ilb12))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    if (detailModel.ilb11.ToUpper() == detailModel.ilb12.ToUpper())
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = "出庫與入庫倉不可相同!";
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
            string ila01New, errMsg;
            vw_invt401 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt401>();
                if (FormEditMode == YREditType.新增)
                {
                    ila01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.ila01, ModuleType.inv, (DateTime)masterModel.ila02, out ila01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["ila01"] = ila01New;
                }
                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["ilasecu"] = LoginInfo.UserNo;
                        DrMaster["ilasecg"] = LoginInfo.GroupNo;
                        DrMaster["ilacreu"] = LoginInfo.UserNo;
                        DrMaster["ilacreg"] = LoginInfo.DeptNo;
                        DrMaster["ilacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["ilamodu"] = LoginInfo.UserNo;
                        DrMaster["ilamodg"] = LoginInfo.DeptNo;
                        DrMaster["ilamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["ilbcreu"] = LoginInfo.UserNo;
                            drDetail["ilbcreg"] = LoginInfo.DeptNo;
                            drDetail["ilbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["ilbmodu"] = LoginInfo.UserNo;
                            drDetail["ilbmodg"] = LoginInfo.DeptNo;
                            drDetail["ilbmodd"] = Now;
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

        #region WfAppendUpdate 存檔後處理額外的資料表 新增、修改、刪除...
        protected override bool WfAppendUpdate()
        {
            DataTable dtIcc;
            DataRow drIcc;
            CommonBLL boAppendIcc;
            StringBuilder sbSql;
            ila_tb ilaModel = null;
            ilb_tb ilbModel = null;
            try
            {
                ilaModel = DrMaster.ToItem<ila_tb>();

                boAppendIcc = new InvBLL(BoMaster.OfGetConntion()); //新增料號庫存明細資料
                boAppendIcc.TRAN = BoMaster.TRAN;
                boAppendIcc.OfCreateDao("icc_tb", "*", "");
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM icc_tb");
                sbSql.AppendLine("WHERE 1<>1");
                dtIcc = boAppendIcc.OfGetDataTable(sbSql.ToString());

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    ilbModel = dr.ToItem<ilb_tb>();
                    if (BoInv.OfChkIccPKExists(ilbModel.ilb03, ilbModel.ilb12) == false)  //入庫倉不存在於icc 則新增一筆
                    {
                        if (dtIcc.Rows.Count > 0)
                        {
                            var drIccs = dtIcc.Select(string.Format("icc01='{0}' AND icc02='{1}'", ilbModel.ilb03, ilbModel.ilb12));
                            if (drIccs != null && drIccs.Length > 0)
                                continue;
                        }
                        drIcc = dtIcc.NewRow();
                        drIcc["icc01"] = ilbModel.ilb03;  //料號
                        drIcc["icc02"] = ilbModel.ilb12;
                        drIcc["icc03"] = "";
                        drIcc["icc04"] = ilbModel.ilb07;  //庫存單位
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

                bt = new ButtonTool("Invr401");
                adoModel = BoAdm.OfGetAdoModel("invr401");
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
            vw_invt401 masterModel;
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

                    case "Invr401":
                        vw_invr401 l_vw_invr401;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        masterModel = DrMaster.ToItem<vw_invt401>();
                        l_vw_invr401 = new vw_invr401();
                        l_vw_invr401.ila01 = masterModel.ila01;
                        l_vw_invr401.ila03 = "";
                        l_vw_invr401.jump_yn = "N";
                        l_vw_invr401.order_by = "1";

                        FrmInvr401 rpInvr401 = new FrmInvr401(this.LoginInfo, l_vw_invr401, true, true);
                        rpInvr401.WindowState = FormWindowState.Minimized;
                        rpInvr401.ShowInTaskbar = false;
                        rpInvr401.Show();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        //*****************************表單自訂Fuction****************************************
        #region WfSetIla01RelReadonly 依單別設定相關控制項 readonly
        private void WfSetIla01RelReadonly(string pIla01)
        {
            bab_tb babModel = null;
            try
            {
                WfSetControlReadonly(new List<Control> { ute_ila01 }, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetIla03Relation 設定客戶相關聯
        private void WfSetIla03Relation(string pIla03)
        {
            sca_tb scaModel;
            bab_tb babModel = null;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["ila01"], ""));
                scaModel = BoStp.OfGetScaModel(pIla03);
                if (scaModel == null)
                    return;

                DrMaster["ila03_c"] = scaModel.sca03;
                DrMaster["ila06"] = scaModel.sca22;    //課稅別
                WfSetIla06Relation(scaModel.sca22);
                DrMaster["ila09"] = scaModel.sca23;    //發票聯數
                DrMaster["ila11"] = scaModel.sca21;    //付款條件
                DrMaster["ila11_c"] = BoBas.OfGetBef03("1", scaModel.sca21);
                //DRMASTER["sga14"] = l_sca.pca35;    //送貨地址
                //DRMASTER["sga15"] = l_sca.pca36;    //帳單地址

                DrMaster["ila12"] = scaModel.sca24;    //取價條件
                DrMaster["ila12_c"] = BoStp.OfGetSbb02(scaModel.sca24);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetIla06Relation 設定稅別關聯
        private void WfSetIla06Relation(string pIla06)
        {
            try
            {
                if (pIla06 == "1")
                {
                    DrMaster["ila07"] = BaaTbModel.baa05;
                    DrMaster["ila08"] = "Y";
                }
                else if (pIla06 == "2")
                {
                    DrMaster["ila07"] = BaaTbModel.baa05;
                    DrMaster["ila08"] = "N";
                }
                else
                {
                    DrMaster["ila07"] = 0;
                    DrMaster["ila08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetIlb03Relation 設定料號關聯
        private bool WfSetIlb03Relation(string pIlb03, DataRow pDr)
        {
            ica_tb icaModel;
            decimal lib08;   //轉換率
            try
            {
                icaModel = BoInv.OfGetIcaModel(pIlb03);
                lib08 = 0;
                if (icaModel == null)
                {
                    pDr["ilb04"] = "";//品名
                    pDr["ilb05"] = 0;//出貨數量
                    pDr["ilb06"] = "";//出貨單位
                    pDr["ilb07"] = "";//庫存單位
                    pDr["ilb08"] = 0;//庫存轉換率
                    pDr["ilb17"] = 0;//庫存數量
                }
                else
                {
                    if (BoInv.OfGetUnitCovert(pIlb03, icaModel.ica07, icaModel.ica07, out lib08) == false)     //都先以庫存單位處理
                    {
                        WfShowErrorMsg("未設定料號轉換,請先設定!");
                        return false;
                    }
                    pDr["ilb04"] = icaModel.ica02;//品名
                    pDr["ilb05"] = 0;//出貨數量
                    pDr["ilb06"] = icaModel.ica07;//借出帶庫存單位
                    pDr["ilb07"] = icaModel.ica07;//庫存單位
                    pDr["ilb08"] = lib08;    //對庫存轉換率
                    pDr["ilb17"] = 0;//庫存轉換數量
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetIlb06Relation 設定借出單位關聯
        //檢查前要先確認料號是否已輸入
        private bool WfSetIlb06Relation(DataRow pDr, string pIlb06, string pBab08)
        {
            vw_invt401s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_invt401s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pIlb06, "")) == false)
                {
                    WfShowErrorMsg("無此借出單位!請確認");
                    return false;
                }
                //取得是否有銷售對庫存的轉換率
                dConvert = 0;
                if (BoInv.OfGetUnitCovert(detailModel.ilb03, pIlb06, detailModel.ilb07, out dConvert) == true)
                {
                    pDr["ilb08"] = dConvert;
                    pDr["ilb17"] = BoBas.OfGetUnitRoundQty(detailModel.ilb07, detailModel.ilb05 * dConvert); //轉換庫存數量(並四拾伍入)
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkIlb05 數量檢查
        private bool WfChkIlb05(DataRow pdr, vw_invt401s pListDetail)
        {
            List<vw_invt401s> detailList = null;
            bab_tb babModel = null;
            string errMsg;
            try
            {
                detailList = pdr.Table.ToList<vw_invt401s>();
                if (GlobalFn.varIsNull(pListDetail.ilb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.ilb03))
                {
                    errMsg = "請先輸入料號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.ilb06))
                {
                    errMsg = "請先輸入借出單位!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (pListDetail.ilb05 <= 0)
                {
                    errMsg = "借出數量應大於0!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["ila01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
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

        #region WfChkIlb06 檢查借出單位
        //檢查前要先確認料號是否已輸入
        private bool WfChkIlb06(DataRow pDr, string pIlb06, string pBab08)
        {
            vw_invt401s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_invt401s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pIlb06, "")) == false)
                {
                    WfShowErrorMsg("無此借出單位!請確認");
                    return false;
                }
                //檢查是否有銷售對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.ilb03, pIlb06, detailModel.ilb07, out dConvert) == false)
                {
                    WfShowErrorMsg("未設定借出單位對庫存單位的轉換率,請先設定!");
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
        private bool WfSetDetailAmt(DataRow drIlb)
        {
            ila_tb ilaModel;
            ilb_tb ilbModel;
            decimal ilb10t = 0, ilb10 = 0;
            try
            {
                ilaModel = DrMaster.ToItem<ila_tb>();
                ilbModel = drIlb.ToItem<ilb_tb>();

                if (ilaModel.ila08 == "Y")//稅內含
                {
                    ilb10t = ilbModel.ilb09 * ilbModel.ilb05;
                    ilb10t = GlobalFn.Round(ilb10t, BekTbModel.bek04);
                    ilb10 = ilb10t / (1 + (ilaModel.ila07 / 100));
                    ilb10 = GlobalFn.Round(ilb10, BekTbModel.bek04);
                }
                else//稅外加
                {
                    ilb10 = ilbModel.ilb09 * ilbModel.ilb05;
                    ilb10 = GlobalFn.Round(ilb10, BekTbModel.bek04);
                    ilb10t = ilb10 * (1 + (ilaModel.ila07 / 100));
                    ilb10t = GlobalFn.Round(ilb10t, BekTbModel.bek04);
                }
                drIlb["ilb10"] = ilb10;
                drIlb["ilb10t"] = ilb10t;

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
            ila_tb ilaModel;
            decimal ila13 = 0, ila13t = 0, ila13g;
            try
            {
                ilaModel = DrMaster.ToItem<ila_tb>();
                if (ilaModel.ila08 == "Y")//稅內含,先處理含稅金額
                {
                    foreach (ilb_tb l_ilb in TabDetailList[0].DtSource.ToList<ilb_tb>())
                    {
                        ila13t += l_ilb.ilb10t;
                    }
                    ila13t = GlobalFn.Round(ila13t, BekTbModel.bek04);
                    ila13 = ila13t / (1 + ilaModel.ila07 / 100);
                    ila13 = GlobalFn.Round(ila13, BekTbModel.bek04);
                    ila13g = ila13t - ila13;

                }
                else//稅外加
                {
                    foreach (ilb_tb l_ilb in TabDetailList[0].DtSource.ToList<ilb_tb>())
                    {
                        ila13 += l_ilb.ilb10;
                    }
                    ila13 = GlobalFn.Round(ila13, BekTbModel.bek04);
                    ila13g = ila13 * (ilaModel.ila07 / 100);
                    ila13g = GlobalFn.Round(ila13g, BekTbModel.bek04);
                    ila13t = ila13 + ila13g;
                }

                DrMaster["ila13"] = ila13;
                DrMaster["ila13t"] = ila13t;
                DrMaster["ila13g"] = ila13g;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfResetAmtByIla06 依稅別更新單身及單頭的金額
        private void WfResetAmtByIla06()
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

        #region WfChkImaExists 檢查借出歸還單是否存在
        private bool WfChkImaExists(string pIla01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChkCnts = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM ima_tb");
                sbSql.AppendLine("  INNER JOIN imb_tb ON ima01=imb01");
                sbSql.AppendLine("WHERE");
                sbSql.AppendLine(" imaconf <>'X' ");
                sbSql.AppendLine(" AND imb11=@imb11 ");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@imb11", pIla01));
                iChkCnts = GlobalFn.isNullRet(BoInv.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (iChkCnts > 0)
                    return true;

                return false;
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
            ila_tb ilaModel = null;
            ilb_tb ilbModel = null;

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

                ilaModel = DrMaster.ToItem<ila_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                ilaModel.ilastat = "1";
                ilaModel.ilaconf = "Y";
                ilaModel.ilacond = Today;
                ilaModel.ilaconu = LoginInfo.UserNo;

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    ilbModel = dr.ToItem<ilb_tb>();
                    //更新料件庫存量 icc_tb --出庫
                    if (BoInv.OfUpdIcc05("2", ilbModel.ilb03, ilbModel.ilb11, ilbModel.ilb17, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }

                    //更新料件庫存量 icc_tb --入庫
                    if (BoInv.OfUpdIcc05("1", ilbModel.ilb03, ilbModel.ilb12, ilbModel.ilb17, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }

                    //更新庫存交易歷史檔
                    if (BoInv.OfStockPost("invt401", ilaModel, ilbModel, LoginInfo, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                }

                DrMaster["ilastat"] = "1";
                DrMaster["ilaconf"] = "Y";
                DrMaster["ilacond"] = Today;
                DrMaster["ilaconu"] = LoginInfo.UserNo;
                DrMaster["ilamodu"] = LoginInfo.UserNo;
                DrMaster["ilamodg"] = LoginInfo.DeptNo;
                DrMaster["ilamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                ilaModel = DrMaster.ToItem<ila_tb>();
                WfSetDocPicture("", ilaModel.ilaconf, "", pbxDoc);
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
            vw_invt401 masterModel = null;
            List<vw_invt401s> detailList = null;
            decimal icc05 = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt401>();
                if (masterModel.ilaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    return false;
                }

                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.ila02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_invt401s>();
                //檢查撥出倉別的數量是否足夠做出庫
                detailList = TabDetailList[0].DtSource.ToList<vw_invt401s>();
                var listSumIlb =                         //依料號及倉庫做加總
                        from ilb in detailList
                        where ilb.ilb17 > 0 //庫存轉換數量
                        group ilb by new { ilb.ilb03, ilb.ilb11 } into ilb_sum
                        select new
                        {
                            ilb03 = ilb_sum.Key.ilb03,
                            ilb11 = ilb_sum.Key.ilb11,
                            ilb17 = ilb_sum.Sum(p => p.ilb17)
                        }
                    ;
                foreach (var sumIfb in listSumIlb)
                {
                    icc05 = BoInv.OfGetIcc05(sumIfb.ilb03, sumIfb.ilb11);
                    if (icc05 < sumIfb.ilb17)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", sumIfb.ilb03, icc05));
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
            ila_tb ilaModel = null;
            ilb_tb ilbModel = null;
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
                ilaModel = DrMaster.ToItem<ila_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    ilbModel = dr.ToItem<ilb_tb>();
                    //更新料件庫存量 icc_tb  --出庫倉
                    if (BoInv.OfUpdIcc05("1", ilbModel.ilb03, ilbModel.ilb11, ilbModel.ilb17, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }

                    //更新料件庫存量 icc_tb --入庫倉
                    if (BoInv.OfUpdIcc05("2", ilbModel.ilb03, ilbModel.ilb12, ilbModel.ilb17, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }

                    //刪除庫存交易歷史檔
                    if (BoInv.OfDelIna(ilbModel.ilb01, ilbModel.ilb02, "1", this.StrFormID, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }

                    //刪除庫存交易歷史檔
                    if (BoInv.OfDelIna(ilbModel.ilb01, ilbModel.ilb02, "2", this.StrFormID, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                }

                DrMaster["ilastat"] = "0";
                DrMaster["ilaconf"] = "N";
                DrMaster["ilacond"] = DBNull.Value;
                DrMaster["ilaconu"] = "";
                DrMaster["ilamodu"] = LoginInfo.UserNo;
                DrMaster["ilamodg"] = LoginInfo.DeptNo;
                DrMaster["ilamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                ilaModel = DrMaster.ToItem<ila_tb>();
                WfSetDocPicture("", ilaModel.ilaconf, "", pbxDoc);
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
            vw_invt401 masterModel = null;
            vw_invt401s detailModel = null;
            List<vw_invt401s> detailList = null;
            decimal icc05 = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_invt401>();
                if (masterModel.ilaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.ila02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }
                //檢查是否存在借出歸還單
                if (WfChkImaExists(masterModel.ila01)==true)
                {
                    WfShowErrorMsg("已存在借出歸還單,不可取消確認!");
                    return false;
                }

                //檢查在入庫倉的數量是否足夠做出庫
                detailList = TabDetailList[0].DtSource.ToList<vw_invt401s>();
                var listSumIlb =                         //依料號及倉庫做加總
                        from ilb in detailList
                        where ilb.ilb17 > 0
                        group ilb by new { ilb.ilb03,ilb.ilb12 } into ilb_sum
                        select new
                        {
                            ilb03 = ilb_sum.Key.ilb03,
                            ilb12 = ilb_sum.Key.ilb12,
                            ilb17 = ilb_sum.Sum(p => p.ilb17)
                        }
                    ;
                foreach (var sumIlb in listSumIlb)
                {
                    icc05 = BoInv.OfGetIcc05(sumIlb.ilb03, sumIlb.ilb12);
                    if (icc05 < sumIlb.ilb17)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", sumIlb.ilb17, icc05));
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
            vw_invt401 listMaster = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                listMaster = DrMaster.ToItem<vw_invt401>();

                if (listMaster.ilaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認,不可作廢!");
                    WfRollback();
                    return;
                }

                if (listMaster.ilaconf == "N")//走作廢
                {

                    DrMaster["ilastat"] = "X";
                    DrMaster["ilaconf"] = "X";
                    DrMaster["ilaconu"] = LoginInfo.UserNo;
                }
                else if (listMaster.ilaconf == "X")
                {
                    DrMaster["ilastat"] = "0";
                    DrMaster["ilaconf"] = "N";
                    DrMaster["ilaconu"] = "";
                }
                DrMaster["ilamodu"] = LoginInfo.UserNo;
                DrMaster["ilamodg"] = LoginInfo.DeptNo;
                DrMaster["ilamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                listMaster = DrMaster.ToItem<vw_invt401>();
                WfSetDocPicture("", listMaster.ilaconf, listMaster.ilaconf, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion
    }
}
