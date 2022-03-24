/* 程式名稱: purt300
   系統代號: 採購單維護作業
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

namespace YR.ERP.Forms.Pur
{
    public partial class FrmPurt300 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        PurBLL BoPur = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;

        //baa_tb BaaTbModel = null;
        bek_tb BekModel = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmPurt300()
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
            this.StrFormID = "purt300";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("pfa01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "pfasecu";
                TabMaster.GroupColumn = "pfasecg";

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
                    BoPur.TRAN = BoMaster.TRAN;
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
                sourceList = BoPur.OfGetTaxTypeKVPList();
                WfSetUcomboxDataSource(ucb_pfa06, sourceList);

                //發票聯數
                sourceList = BoPur.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_pfa09, sourceList);

                //單據確認
                sourceList = BoPur.OfGetPfaconfKVPList();
                WfSetUcomboxDataSource(ucb_pfaconf, sourceList);

                //單據狀態
                sourceList = BoPur.OfGetPfastatKVPList();
                WfSetUcomboxDataSource(ucb_pfastat, sourceList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_purt300 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_purt300>();
                    WfSetDocPicture("", masterModel.pfaconf, masterModel.pfastat, pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.pfa10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekModel = BoBas.OfGetBekModel(masterModel.pfa10);
                        if (BekModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.pfa10));
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

                    WfSetControlReadonly(new List<Control> { ute_pfacreu, ute_pfacreg, udt_pfacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_pfamodu, ute_pfamodg, udt_pfamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_pfasecu, ute_pfasecg }, true);
                    WfSetControlReadonly(new List<Control> { ute_tot_cnts }, true);

                    if (GlobalFn.isNullRet(masterModel.pfa01, "") != "")
                    {
                        WfSetControlReadonly(ute_pfa01, true);
                        WfSetPfa01RelReadonly(GlobalFn.isNullRet(masterModel.pfa01, ""));
                    }

                    WfSetControlReadonly(new List<Control> { ute_pfa01_c, ute_pfa03_c, ute_pfa04_c, ute_pfa05_c, ute_pfa11_c, ute_pfa12_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_pfa07, ucx_pfa08 }, true);
                    WfSetControlReadonly(new List<Control> { ute_pfa13, ute_pfa13t, ute_pfa13g }, true);
                    WfSetControlReadonly(new List<Control> { ucb_pfaconf, udt_pfacond, ute_pfaconu, ucb_pfastat }, true);
                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_pfa01, true);
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

        #region WfIniTabDetailInfo(): 設定明細的資料來源
        protected override Boolean WfIniTabDetailInfo()
        {
            SqlParameter keyParm;

            this.TabDetailList[0].TargetTable = "pfb_tb";
            this.TabDetailList[0].ViewTable = "vw_purt300s";
            keyParm = new SqlParameter("pfb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "pfa01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
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
                                columnName == "pfb05" ||
                                columnName == "pfb06" ||
                                columnName == "pfb09" ||
                                columnName == "pfb16" ||
                                columnName == "pfb18" ||
                                columnName == "pfb19" ||
                                columnName == "pfb20"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "pfb02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }

                            if (columnName == "pfb03" ||
                                columnName == "pfb11" ||
                                columnName == "pfb12"
                                )
                            {
                                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["pfa01"], ""));
                                if (babModel == null)
                                {
                                    WfShowErrorMsg("請先輸入採購單頭資料!");
                                }
                                if (GlobalFn.isNullRet(babModel.bab08, "") == "Y")    //有來源單據
                                {
                                    if (columnName == "pfb03")//料號
                                        WfSetControlReadonly(ugc, true);    //不可輸入
                                    else
                                        WfSetControlReadonly(ugc, false);
                                }
                                else
                                {
                                    if (columnName == "pfb03")//料號
                                        WfSetControlReadonly(ugc, false);    //可輸入
                                    else
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
            //int iChkCnts = 0;
            //string errMsg;
            //StringBuilder sbSql;
            //List<SqlParameter> sqlParms;
            try
            {
                //this.MsgInfoReturned = new MessageInfo();
                vw_purt300s detailModel = null;
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_purt300
                if (pDr.Table.Prefix.ToLower() == "vw_purt300")
                {
                    switch (pColName.ToLower())
                    {
                        case "pfa01"://採購單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "pur"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "20"));
                            WfShowPickUtility("p_bab1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "pfa03"://廠商編號
                            WfShowPickUtility("p_pca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pca01"], "");
                                else
                                    pDr[pColName] = "";
                            }

                            break;
                        case "pfa04"://採購人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }

                            break;
                        case "pfa05"://請購部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "pfa10"://幣別
                            WfShowPickUtility("p_bek1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bek01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "pfa11"://付款條件
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

                        case "pfa12"://取價條件
                            WfShowPickUtility("p_pbb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pbb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;


                        case "pfa14"://送貨地址
                            messageModel.IsAutoQuery = true;
                            WfShowPickUtility("p_pcc1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pcc01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "pfa15"://帳單地址
                            messageModel.IsAutoQuery = true;
                            WfShowPickUtility("p_pcc2", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pcc01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_purt300s
                if (pDr.Table.Prefix.ToLower() == "vw_purt300s")
                {
                    detailModel = pDr.ToItem<vw_purt300s>();
                    switch (pColName.ToLower())
                    {
                        case "pfb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "pfb06"://採購單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "pfb11"://請購單號
                            WfShowPickUtility("p_peb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                {
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["peb01"], "");
                                    pDr["pfb12"] = GlobalFn.isNullRet(messageModel.DataRowList[0]["peb02"], "");
                                }
                                else
                                {
                                    pDr[pColName] = "";
                                    pDr["pfb12"] = "";
                                }
                            }
                            break;

                        case "pfb16"://倉庫
                            WfShowPickUtility("p_icb1", messageModel);
                            //if (GlobalFn.isNullRet(detailModel.pfb03, "") == "")
                            //    WfShowPickUtility("p_icb1", messageModel);
                            //else
                            //{
                            //    messageModel.ParamSearchList = new List<SqlParameter>();
                            //    messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.pfb03));
                            //    WfShowPickUtility("p_icc1", messageModel);
                            //}
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icb01"], "");
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

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["pfa02"] = Today;
                pDr["pfa04"] = LoginInfo.UserNo;
                pDr["pfa04_c"] = LoginInfo.UserName;
                pDr["pfa05"] = LoginInfo.DeptNo;
                pDr["pfa05_c"] = LoginInfo.DeptName;
                pDr["pfa07"] = 0;
                pDr["pfa08"] = "N";
                pDr["pfa13"] = 0;
                pDr["pfa13t"] = 0;
                pDr["pfa13g"] = 0;
                pDr["pfa10"] = BaaModel.baa04;
                pDr["pfa17"] = 1;
                pDr["pfaconf"] = "N";
                pDr["pfastat"] = "0";
                pDr["pfacomp"] = LoginInfo.CompNo;
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
                        pDr["pfb02"] = WfGetMaxSeq(pDr.Table, "pfb02");
                        pDr["pfb05"] = 0;
                        pDr["pfb08"] = 0;
                        pDr["pfb09"] = 0;
                        pDr["pfb10"] = 0;
                        pDr["pfb10t"] = 0;
                        pDr["pfb13"] = 0;
                        pDr["pfb14"] = 0;
                        pDr["pfb17"] = 0;
                        pDr["pfbcomp"] = LoginInfo.CompNo;
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
                //BaaTbModel = BoBas.OfGetBaaModel();
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
            vw_purt300 masterModel;
            try
            {
                //BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_purt300>();
                if (masterModel.pfaconf != "N")
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
            vw_purt300 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_purt300>();
                if (masterModel.pfaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認,不可作廢!");
                    return false;
                }

                //檢查入庫單是否存在
                if (WfChkPgaExists(masterModel.pfa01) == true)
                {
                    WfShowErrorMsg("已有入庫單資料!不可取消確認!");
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
            vw_purt300 masterModel = null;
            vw_purt300s detailModel, newDetailModel = null;
            List<vw_purt300s> detailList = null;
            bab_tb babModel = null;
            UltraGrid uGrid = null;
            decimal uTaxPrice, taxPrice = 0;
            Result result = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt300>();
                if (e.Column.ToLower() != "pfa01" && GlobalFn.isNullRet(DrMaster["pfa01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false;
                }
                #region 單頭-pick vw_purt300
                if (e.Row.Table.Prefix.ToLower() == "vw_purt300")
                {
                    switch (e.Column.ToLower())
                    {
                        case "pfa01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "pur", "20") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["pfa01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            WfSetPfa01RelReadonly(GlobalFn.isNullRet(e.Value, ""));
                            break;
                        case "pfa03"://廠商
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pfa03_c"] = "";
                                e.Row["pfa14"] = "";    //送貨地址
                                e.Row["pfa15"] = "";    //帳單地址
                                return true;
                            }
                            if (BoPur.OfChkPcaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此廠商資料,請檢核!");
                                return false;
                            }
                            WfSetPfa03Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByPfa06();
                            break;

                        case "pfa04"://採購人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pfa04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["pfa04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pfa05"://採購部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pfa05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["pfa05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pfa06"://課稅別
                            WfSetPfa06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByPfa06();
                            break;

                        case "pfa10"://幣別
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

                        case "pfa11"://付款條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pfa11_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBefPKValid("1", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此付款條件,請檢核!");
                                return false;
                            }
                            e.Row["pfa11_c"] = BoBas.OfGetBef03("1", GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pfa12"://取價條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pfa12_c"] = "";
                                return true;
                            }
                            if (BoPur.OfChkPbbPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此取價條件,請檢核!");
                                return false;
                            }
                            e.Row["pfa12_c"] = BoPur.OfGetPbb02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pfa14"://送貨地址
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoPur.OfChkPccPKValid(GlobalFn.isNullRet(e.Value, ""), "1") == false)
                            {
                                WfShowErrorMsg("無此送貨地址,請檢核!");
                                return false;
                            }
                            break;

                        case "pfa15"://帳單地址
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoPur.OfChkPccPKValid(GlobalFn.isNullRet(e.Value, ""), "2") == false)
                            {
                                WfShowErrorMsg("無此帳單地址,請檢核!");
                                return false;
                            }
                            break;

                        case "pfa17": //匯率
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

                #region 單身-pick vw_purt300s
                if (e.Row.Table.Prefix.ToLower() == "vw_purt300s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_purt300s>();
                    detailList = e.Row.Table.ToList<vw_purt300s>();
                    babModel = BoBas.OfGetBabModel(masterModel.pfa01);

                    switch (e.Column.ToLower())
                    {
                        case "pfb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.pfb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "pfb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["pfb04"] = "";//品名
                                e.Row["pfb05"] = 0;//採購數量
                                e.Row["pfb06"] = "";//採購單位
                                e.Row["pfb07"] = "";//庫存單位
                                e.Row["pfb08"] = 0;//庫存轉換率
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }

                            if (WfSetPfb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;

                            if (!GlobalFn.varIsNull(masterModel.pfa12))
                            {
                                newDetailModel = e.Row.ToItem<vw_purt300s>();
                                result = BoPur.OfGetPrice(masterModel.pfa12, masterModel.pfa03, e.Value.ToString(), newDetailModel.pfb06,
                                    masterModel.pfa02, masterModel.pfa10, "2", newDetailModel.pfb05,
                                    masterModel.pfa08, masterModel.pfa07, masterModel.pfa17, out uTaxPrice, out taxPrice);
                                if (result.Success == true)
                                {
                                    if (masterModel.pfa08 == "Y")
                                        e.Row["pfb09"] = taxPrice;
                                    else
                                        e.Row["pfb09"] = uTaxPrice;
                                }
                                else
                                {
                                    WfShowErrorMsg(result.Message);
                                    return false;
                                }
                            }
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "pfb05"://採購數量
                            if (GlobalFn.varIsNull(detailModel.pfb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["pfb03"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(detailModel.pfb06))
                            {
                                WfShowErrorMsg("請先輸入採購單位!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["pfb06"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入採購數量!");
                                return false;
                            }
                            detailModel.pfb05 = BoBas.OfGetUnitRoundQty(detailModel.pfb06, detailModel.pfb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detailModel.pfb05;
                            if (WfChkPfb05(e.Row, detailModel) == false)
                                return false;
                            e.Row["pfb14"] = BoBas.OfGetUnitRoundQty(detailModel.pfb15, detailModel.pfb05 * detailModel.pfb13); //轉換請購數量(並四拾伍入)
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "pfb06"://採購單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (GlobalFn.varIsNull(detailModel.pfb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["pfb03"]);
                                return false;
                            }
                            if (WfChkPfb06(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;

                            if (WfSetPfb06Relation(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;
                            break;

                        case "pfb09":   //單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.pfb09 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.pfb09, BekModel.bek03);
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;


                        case "pfb11"://採購單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("採購單號不可為空白!");
                                return false;
                            }

                            if (WfChkPfb11(e.Value.ToString()) == false)
                                return false;

                            if (!GlobalFn.varIsNull(detailModel.pfb12))
                            {
                                if (WfChkPfb12(GlobalFn.isNullRet(detailModel.pfb11, ""), GlobalFn.isNullRet(detailModel.pfb12, 0)) == false)
                                    return false;

                                if (WfSetPfb12Relation(e.Row, GlobalFn.isNullRet(detailModel.pfb11, ""), GlobalFn.isNullRet(detailModel.pfb12, 0)) == false)
                                    return false;
                            }
                            WfSetTotalAmt();
                            break;

                        case "pfb12"://採購項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("採購項次不可為空白!");
                                return false;
                            }

                            if (WfChkPfb12(GlobalFn.isNullRet(detailModel.pfb11, ""), GlobalFn.isNullRet(detailModel.pfb12, 0)) == false)
                                return false;
                            WfSetPfb12Relation(e.Row, GlobalFn.isNullRet(detailModel.pfb11, ""), GlobalFn.isNullRet(detailModel.pfb12, 0));
                            WfSetTotalAmt();
                            break;

                        case "pfb16"://倉庫
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

        #region WfAfterDetailDelete() :刪除明細後調用
        protected override bool WfAfterDetailDelete(int pCurTabDetail, DataRow pDr)
        {
            try
            {
                switch (pDr.Table.Prefix.ToLower())
                {
                    case "vw_purt300s":
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_purt300 masterModel = null;
            vw_purt300s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {

                masterModel = DrMaster.ToItem<vw_purt300>();
                if (!GlobalFn.varIsNull(masterModel.pfa01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.pfa01, ""));
                #region 單頭資料檢查
                chkColName = "pfa01";       //採購單號
                chkControl = ute_pfa01;
                if (GlobalFn.varIsNull(masterModel.pfa01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pfa02";       //採購日期
                chkControl = udt_pfa02;
                if (GlobalFn.varIsNull(masterModel.pfa02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pfa03";       //廠商編號
                chkControl = ute_pfa03;
                if (GlobalFn.varIsNull(masterModel.pfa03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pfa04";       //採購人員
                chkControl = ute_pfa04;
                if (GlobalFn.varIsNull(masterModel.pfa04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pfa05";       //採購部門
                chkControl = ute_pfa05;
                if (GlobalFn.varIsNull(masterModel.pfa05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pfa06";       //課稅別
                chkControl = ucb_pfa06;
                if (GlobalFn.varIsNull(masterModel.pfa06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pfa10";       //幣別
                chkControl = ute_pfa10;
                if (GlobalFn.varIsNull(masterModel.pfa10))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pfa12";
                chkControl = ute_pfa12;
                if (GlobalFn.varIsNull(masterModel.pfa12) && babModel.bab08 != "Y")//無來源單據取價條件要輸入
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

                    detailModel = drTemp.ToItem<vw_purt300s>();
                    chkColName = "pfb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.pfb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "pfb11";   //請購單號
                    if (GlobalFn.varIsNull(detailModel.pfb11) && babModel.bab08 == "Y")  //有來源單據請購單號要輸入
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "pfb12";   //請購項次
                    if (GlobalFn.varIsNull(detailModel.pfb12) && babModel.bab08 == "Y")  //有來源單據請購單號要輸入
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "pfb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.pfb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "pfb05";   //採購數量
                    #region pfb05 採購數量
                    if (GlobalFn.varIsNull(detailModel.pfb05) || detailModel.pfb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    if (WfChkPfb05(drTemp, detailModel) == false)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    chkColName = "pfb06";   //採購單位
                    if (GlobalFn.varIsNull(detailModel.pfb06))
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
            string pfa01New, errMsg;
            vw_purt300 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt300>();
                if (FormEditMode == YREditType.新增)
                {
                    pfa01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.pfa01, ModuleType.pur, (DateTime)masterModel.pfa02, out pfa01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["pfa01"] = pfa01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["pfasecu"] = LoginInfo.UserNo;
                        DrMaster["pfasecg"] = LoginInfo.GroupNo;
                        DrMaster["pfacreu"] = LoginInfo.UserNo;
                        DrMaster["pfacreg"] = LoginInfo.DeptNo;
                        DrMaster["pfacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["pfamodu"] = LoginInfo.UserNo;
                        DrMaster["pfamodg"] = LoginInfo.DeptNo;
                        DrMaster["pfamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["pfbcreu"] = LoginInfo.UserNo;
                            drDetail["pfbcreg"] = LoginInfo.DeptNo;
                            drDetail["pfbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["pfbmodu"] = LoginInfo.UserNo;
                            drDetail["pfbmodg"] = LoginInfo.DeptNo;
                            drDetail["pfbmodd"] = Now;
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

                bt = new ButtonTool("Purr300");
                adoModel = BoAdm.OfGetAdoModel("Purr300");
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

                    case "Purr300":
                        vw_purr300 l_vw_purr300;
                        vw_purt300 lMaster;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        lMaster = DrMaster.ToItem<vw_purt300>();
                        l_vw_purr300 = new vw_purr300();
                        l_vw_purr300.pfa01 = lMaster.pfa01;
                        l_vw_purr300.pfa03 = "";
                        l_vw_purr300.jump_yn = "N";
                        l_vw_purr300.order_by = "1";

                        FrmPurr300 rpt = new FrmPurr300(this.LoginInfo, l_vw_purr300, true, true);
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

        //*****************************表單自訂Fuction****************************************
        #region WfSetPfa01RelReadonly 依單別設定相關控制項 readonly
        private void WfSetPfa01RelReadonly(string pPfa01)
        {
            bab_tb babModel = null;
            string bab01;
            try
            {
                WfSetControlReadonly(new List<Control> { ute_pfa01 }, true);
                bab01 = pPfa01.Substring(0, GlobalFn.isNullRet(BaaModel.baa06, 0));
                babModel = BoBas.OfGetBabModel(bab01);
                if (babModel.bab08 == "Y")   //有來源單據
                {
                    WfSetControlReadonly(new List<Control> { ute_pfa12 }, true);
                }
                else
                {
                    WfSetControlReadonly(new List<Control> { ute_pfa12 }, false);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPfa03Relation 設定廠商相關聯
        private void WfSetPfa03Relation(string pPfa03)
        {
            pca_tb pcaModel;
            bab_tb babModel = null;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["pfa01"], ""));
                pcaModel = BoPur.OfGetPcaModel(pPfa03);
                if (pcaModel == null)
                    return;

                DrMaster["pfa03_c"] = pcaModel.pca03;
                DrMaster["pfa06"] = pcaModel.pca22;    //課稅別
                WfSetPfa06Relation(pcaModel.pca22);
                DrMaster["pfa09"] = pcaModel.pca23;    //發票聯數

                DrMaster["pfa11"] = pcaModel.pca21;    //付款條件
                DrMaster["pfa11_c"] = BoBas.OfGetBef03("1", pcaModel.pca21);
                DrMaster["pfa14"] = pcaModel.pca35;    //送貨地址
                DrMaster["pfa15"] = pcaModel.pca36;    //帳單地址

                if (babModel.bab08 != "Y")   //無前置單據則取價條件帶入
                {
                    DrMaster["pfa12"] = pcaModel.pca24;    //取價條件
                    DrMaster["pfa12_c"] = BoPur.OfGetPbb02(pcaModel.pca24);
                }
                else
                {
                    DrMaster["pfa12"] = "";
                    DrMaster["pfa12_c"] = "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPfa06Relation 設定稅別關聯
        private void WfSetPfa06Relation(string pPfa06)
        {
            try
            {
                if (pPfa06 == "1")
                {
                    DrMaster["pfa07"] = BaaModel.baa05;
                    DrMaster["pfa08"] = "Y";
                }
                else if (pPfa06 == "2")
                {
                    DrMaster["pfa07"] = BaaModel.baa05;
                    DrMaster["pfa08"] = "N";
                }
                else
                {
                    DrMaster["pfa07"] = 0;
                    DrMaster["pfa08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPfb03Relation 設定料號關聯
        private bool WfSetPfb03Relation(string pPfb03, DataRow pDr)
        {
            ica_tb icaModel;
            decimal peb08;
            try
            {
                icaModel = BoInv.OfGetIcaModel(pPfb03);
                peb08 = 0;
                if (icaModel == null)
                {
                    pDr["pfb04"] = "";//品名
                    pDr["pfb05"] = 0;//採購數量
                    pDr["pfb06"] = "";//採購單位
                    pDr["pfb07"] = "";//庫存單位
                    pDr["pfb08"] = 0;//庫存轉換率
                }
                else
                {
                    if (BoInv.OfGetUnitCovert(pPfb03, icaModel.ica08, icaModel.ica07, out peb08) == false)
                    {
                        WfShowErrorMsg("未設定料號轉換,請先設定!");
                        return false;
                    }
                    pDr["pfb04"] = icaModel.ica02;//品名
                    pDr["pfb05"] = 0;//採購數量
                    pDr["pfb06"] = icaModel.ica08;//採購單位
                    pDr["pfb07"] = icaModel.ica07;//庫存單位
                    pDr["pfb08"] = peb08;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPfb06Relation 設定採購單位關聯
        //檢查前要先確認料號是否已輸入
        private bool WfSetPfb06Relation(DataRow pDr, string pPfb06, string pBab08)
        {
            vw_purt300s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_purt300s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pPfb06, "")) == false)
                {
                    WfShowErrorMsg("無此採購單位!請確認");
                    return false;
                }
                //取得是否有採購對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.pfb03, pPfb06, detailModel.pfb07, out dConvert) == true)
                {
                    pDr["pfb08"] = dConvert;
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有採購對請購的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.pfb03, pPfb06, detailModel.pfb15, out dConvert) == true)
                    {
                        pDr["pfb13"] = dConvert;
                        pDr["pfb14"] = BoBas.OfGetUnitRoundQty(pPfb06, dConvert * detailModel.pfb05);
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

        #region WfSetPfb12Relation 設定請購單明細資料關聯
        private bool WfSetPfb12Relation(DataRow pdr, string pPfb11, int pPfb12)
        {
            peb_tb pebModel;

            try
            {
                pebModel = BoPur.OfGetPebModel(pPfb11, pPfb12);
                if (pebModel == null)
                    return false;
                pdr["pfb03"] = pebModel.peb03;//料號
                pdr["pfb04"] = pebModel.peb04;//品名
                pdr["pfb05"] = pebModel.peb05;//採購數量
                pdr["pfb06"] = pebModel.peb06;//採購單位
                pdr["pfb07"] = pebModel.peb07;//庫存單位
                pdr["pfb08"] = pebModel.peb08;//庫存轉換率
                pdr["pfb13"] = 1;          //採購對請購轉換率
                pdr["pfb14"] = pebModel.peb05;//轉換請購數量
                pdr["pfb15"] = pebModel.peb06;//原請購單位

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkPfb05 數量檢查
        private bool WfChkPfb05(DataRow pdr, vw_purt300s pListDetail)
        {
            List<vw_purt300s> detailList = null;
            peb_tb pebModel = null;
            bab_tb babModel = null;
            string errMsg;
            decimal docThisQty = 0;         //本項次的轉換請購數量
            decimal docOtherQtyTotal = 0;   //本單據其他項次的請購數量加總
            decimal otherQtyTotal = 0;      //其他單據的數量加總
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                detailList = pdr.Table.ToList<vw_purt300s>();
                if (GlobalFn.varIsNull(pListDetail.pfb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.pfb03))
                {
                    errMsg = "請先輸入料號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.pfb06))
                {
                    errMsg = "請先輸入採購單位!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (pListDetail.pfb05 <= 0)
                {
                    errMsg = "採購數量應大於0!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["pfa01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (babModel.bab08 != "Y") //無來源單據,以下不檢查!
                    return true;

                pebModel = BoPur.OfGetPebModel(pListDetail.pfb11, Convert.ToInt16(pListDetail.pfb12));
                if (babModel == null)
                {
                    errMsg = "Get peb_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                //取得本身單據中的所有數量轉換採購單位後的總和!
                //先取本身這筆
                docThisQty = BoBas.OfGetUnitRoundQty(pListDetail.pfb15, pListDetail.pfb05 * pListDetail.pfb13);
                //再取其他筆加總
                docOtherQtyTotal = detailList.Where(p => p.pfb02 != pListDetail.pfb02)
                                   .Where(p => p.pfb11 == pListDetail.pfb11 && p.pfb12 == pListDetail.pfb12)
                                   .Sum(p => p.pfb14);
                //取得其他單據上的加總
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT SUM(pfb14) FROM pfa_tb");
                sbSql.AppendLine("  INNER JOIN pfb_tb ON pfa01=pfb01");
                sbSql.AppendLine("WHERE pfaconf<>'X'");
                sbSql.AppendLine("  AND pfa01 <> @pfa01");
                sbSql.AppendLine("  AND pfb11 = @pfb11 AND pfb12 = @pfb12");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@pfa01", GlobalFn.isNullRet(DrMaster["pfa01"], "")));
                sqlParmList.Add(new SqlParameter("@pfb11", pListDetail.pfb11));
                sqlParmList.Add(new SqlParameter("@pfb12", pListDetail.pfb12));
                otherQtyTotal = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (pebModel.peb05 < (docThisQty + docOtherQtyTotal + otherQtyTotal))
                {
                    errMsg = string.Format("項次{0}請購單最大可輸入數量為 {1}",
                                            pListDetail.pfb02.ToString(),
                                            (pebModel.peb05 - docOtherQtyTotal - otherQtyTotal).ToString()
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

        #region WfChkPfb06 檢查採購單位
        //檢查前要先確認料號是否已輸入
        private bool WfChkPfb06(DataRow pDr, string pPfb06, string pBab08)
        {
            vw_purt300s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_purt300s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pPfb06, "")) == false)
                {
                    WfShowErrorMsg("無此採購單位!請確認");
                    return false;
                }
                //檢查是否有採購對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.pfb03, pPfb06, detailModel.pfb07, out dConvert) == false)
                {
                    WfShowErrorMsg("未設定採購單位對庫存單位的轉換率,請先設定!");
                    return false;
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有採購對請購的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.pfb03, pPfb06, detailModel.pfb15, out dConvert) == false)
                    {
                        WfShowErrorMsg("未設定採購單位對請購單位的轉換率,請先設定!");
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

        #region WfChkPfb11 請購單檢查
        private bool WfChkPfb11(string pPfb11)
        {
            string errMsg;
            pea_tb peaModel;
            try
            {
                peaModel = BoPur.OfGetPeaModel(pPfb11);
                if (peaModel == null)
                {
                    errMsg = "查無此請購單!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (peaModel.peaconf != "Y")
                {
                    errMsg = "請購單非確認狀態!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (peaModel.peastat == "9")
                {
                    errMsg = "請購單已結案!";
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

        #region WfChkPfb12 請購項次檢查
        private bool WfChkPfb12(string pPfb11, int pPfb12)
        {
            string errMsg;
            peb_tb pebModel;
            try
            {
                pebModel = BoPur.OfGetPebModel(pPfb11, pPfb12);
                if (pebModel == null)
                {
                    errMsg = "查無此請購項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if ((pebModel.peb05 - pebModel.peb11) <= 0)
                {
                    errMsg = string.Format("無可用採購數量!(可用採購數量為{0})", GlobalFn.isNullRet(pebModel.peb05 - pebModel.peb11, 0));
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

        #region WfConfirm 確認
        private void WfConfirm()
        {
            vw_purt300 masterModel = null;
            List<vw_purt300s> detailList = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_purt300>();
                detailList=TabDetailList[0].DtSource.ToList<vw_purt300s>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                if (WfUpdPeb11(true) == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }

                if (WfUpdPebstatByConfirm() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //寫入料件廠商價格檔
                if (WfUpdPdd(masterModel, detailList) == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }

                DrMaster["pfastat"] = "1";
                DrMaster["pfaconf"] = "Y";
                DrMaster["pfacond"] = Today;
                DrMaster["pfaconu"] = LoginInfo.UserNo;
                DrMaster["pfamodu"] = LoginInfo.UserNo;
                DrMaster["pfamodg"] = LoginInfo.DeptNo;
                DrMaster["pfamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_purt300>();
                WfSetDocPicture("", masterModel.pfaconf, masterModel.pfastat, pbxDoc);
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
            vw_purt300 masterModel = null;
            vw_purt300s detailModel = null;
            List<vw_purt300s> detailList = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt300>();
                if (masterModel.pfaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    WfRollback();
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_purt300s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_purt300s>();
                    if (WfChkPfb05(drTemp, detailModel) == false)
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

        #region WfCancelConfirm 取消確認
        private void WfCancelConfirm()
        {
            vw_purt300 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)
                    return;
                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_purt300>();
                if (WfCancleConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                if (WfUpdPeb11(false) == false)
                {
                    WfRollback();
                    return;
                }
                if (WfUpdPebstatByCancleConfirm() == false)
                {
                    WfRollback();
                    return;
                }


                DrMaster["pfastat"] = "0";
                DrMaster["pfaconf"] = "N";
                DrMaster["pfacond"] = DBNull.Value;
                DrMaster["pfaconu"] = "";
                DrMaster["pfamodu"] = LoginInfo.UserNo;
                DrMaster["pfamodg"] = LoginInfo.DeptNo;
                DrMaster["pfamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_purt300>();
                WfSetDocPicture("", masterModel.pfaconf, masterModel.pfastat, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfCancleConfirmChk 取消確認檢查
        private bool WfCancleConfirmChk()
        {
            vw_purt300 masterModel = null;
            vw_purt300s detailModel = null;
            List<vw_purt300s> detailList = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt300>();
                if (masterModel.pfaconf != "Y")
                {
                    WfShowErrorMsg("單據非確認狀態!");
                    WfRollback();
                    return false;
                }

                //檢查入庫單是否存在
                if (WfChkPgaExists(masterModel.pfa01) == true)
                {
                    WfShowErrorMsg("已有入庫單資料!不可取消確認!");
                    WfRollback();
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

        #region WfInvalid 作廢/作廢還原
        private void WfInvalid()
        {
            vw_purt300 masterModel = null;
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
                masterModel = DrMaster.ToItem<vw_purt300>();

                if (masterModel.pfaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.pfaconf == "N")//走作廢
                {
                    DrMaster["pfastat"] = "X";
                    DrMaster["pfaconf"] = "X";
                    DrMaster["pfaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.pfaconf == "X")
                {
                    //檢查請購單是否為有效資料
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT COUNT(1)");
                    sbSql.AppendLine("FROM pea_tb");
                    sbSql.AppendLine("WHERE exists ");
                    sbSql.AppendLine("      (SELECT 1 FROM pfb_tb WHERE pfb01=@pfb01");
                    sbSql.AppendLine("      AND pfb11=pea01)");
                    sbSql.AppendLine("AND ISNULL(peaconf,'')<>'Y'");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@pfb01", GlobalFn.isNullRet(DrMaster["pfa01"], "")));
                    iChkCnts = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                    if (iChkCnts > 0)
                    {
                        WfShowErrorMsg("請購單含有未確認資料!不可作廢還原!");
                        WfRollback();
                        return;
                    }
                    DrMaster["pfastat"] = "0";
                    DrMaster["pfaconf"] = "N";
                    DrMaster["pfaconu"] = "";
                }
                DrMaster["pfamodu"] = LoginInfo.UserNo;
                DrMaster["pfamodg"] = LoginInfo.DeptNo;
                DrMaster["pfamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_purt300>();
                WfSetDocPicture("", masterModel.pfaconf, masterModel.pfastat, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfUpdPeb11 更新請購單已轉採購數量 確認/取消確認
        private bool WfUpdPeb11(bool pbConfirm)
        {
            bab_tb babModel = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            DataTable dtPfbDistinct = null;
            string peb01;
            decimal peb02;
            decimal docQty = 0, otherDocQty = 0;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["pfa01"], ""));
                if (babModel == null)
                {
                    WfShowErrorMsg("Get bab_tb Error!");
                    return false;
                }
                if (babModel.bab08 == "Y")   //更新採購數量
                {
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT DISTINCT pfb11,pfb12,SUM(pfb14) pfb14");
                    sbSql.AppendLine("FROM pfb_tb");
                    sbSql.AppendLine("WHERE pfb01=@pfb01");
                    sbSql.AppendLine("  AND ISNULL(pfb11,'')<>''");
                    sbSql.AppendLine("GROUP BY pfb11,pfb12");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@pfb01", GlobalFn.isNullRet(DrMaster["pfa01"], "")));
                    dtPfbDistinct = BoPur.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                    if (dtPfbDistinct == null)
                        return true;
                    foreach (DataRow dr in dtPfbDistinct.Rows)
                    {
                        peb01 = dr["pfb11"].ToString();
                        peb02 = Convert.ToDecimal(dr["pfb12"]);
                        docQty = Convert.ToDecimal(dr["pfb14"]);

                        sbSql = new StringBuilder();
                        sbSql.AppendLine("SELECT SUM(pfb14) FROM pfa_tb");
                        sbSql.AppendLine("  INNER JOIN pfb_tb ON pfa01=pfb01");
                        sbSql.AppendLine("WHERE pfaconf='Y'");
                        sbSql.AppendLine("AND pfb11=@pfb11 AND pfb12=@pfb12");
                        sbSql.AppendLine("AND pfa01<>@pfa01");


                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@pfb11", peb01));
                        sqlParmList.Add(new SqlParameter("@pfb12", peb02));
                        sqlParmList.Add(new SqlParameter("@pfa01", GlobalFn.isNullRet(DrMaster["pfa01"], "")));
                        otherDocQty = 0;
                        otherDocQty = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);

                        sbSql = new StringBuilder();
                        sbSql = sbSql.AppendLine("UPDATE peb_tb");
                        sbSql = sbSql.AppendLine("SET peb11=@peb11");
                        sbSql = sbSql.AppendLine("WHERE peb01=@peb01");
                        sbSql = sbSql.AppendLine("AND peb02=@peb02");
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@peb01", peb01));
                        sqlParmList.Add(new SqlParameter("@peb02", peb02));
                        if (pbConfirm)  //確認的要加本身單據
                            sqlParmList.Add(new SqlParameter("@peb11", docQty + otherDocQty));
                        else
                            sqlParmList.Add(new SqlParameter("@peb11", otherDocQty));

                        if (BoPur.OfExecuteNonquery(sbSql.ToString(), sqlParmList.ToArray()) != 1)
                        {
                            WfShowErrorMsg("更新請購單數量失敗!");
                            return false;
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

        #region WfUpdPebstatByConfirm() 更新請購單狀態 確認
        private bool WfUpdPebstatByConfirm()
        {
            List<vw_purt300s> detailList;
            int chkCnts = 0;
            string querySql = "", updateSql = "";
            List<SqlParameter> sqlParms;
            try
            {
                detailList = TabDetailList[0].DtSource.ToList<vw_purt300s>();
                //取得請購單單號
                var pfbDistinct = from o in detailList
                                  where !GlobalFn.varIsNull(o.pfb11)
                                  group o by new { o.pfb11 } into pfbTemp
                                  select new
                                  {
                                      pfb11 = pfbTemp.Key.pfb11
                                  }
                               ;

                if (pfbDistinct == null)
                    return true;
                //查詢
                querySql = @"SELECT COUNT(1)
                        FROM pea_tb
	                        INNER JOIN peb_tb ON pea01=peb01
                        WHERE pea01=@pea01
	                        AND peastat='1'
	                        AND (peb05-peb11)>0
                        ";
                //逐筆更新
                updateSql = @"UPDATE dbo.pea_tb
                            SET peastat='9'
                            WHERE pea01=@pea01
                        ";

                foreach (var pfbModel in pfbDistinct)
                {
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@pea01", pfbModel.pfb11));
                    chkCnts = GlobalFn.isNullRet(BoPur.OfGetFieldValue(querySql, sqlParms.ToArray()), 0);
                    if (chkCnts > 0)
                        continue;
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@pea01", pfbModel.pfb11));
                    if (BoPur.OfExecuteNonquery(updateSql, sqlParms.ToArray()) < 1)
                    {
                        WfShowErrorMsg("更新peastat失敗!");
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

        #region WfUpdPebstatByCancleConfirm() 更新請購單狀態 取消確認
        private bool WfUpdPebstatByCancleConfirm()
        {
            List<vw_purt300s> detailList;
            int chkCnts = 0;
            string querySql = "", updateSql = "";
            List<SqlParameter> sqlParms;
            try
            {
                detailList = TabDetailList[0].DtSource.ToList<vw_purt300s>();
                //取得請購單單號
                var pfbDistinct = from o in detailList
                                  where !GlobalFn.varIsNull(o.pfb11)
                                  group o by new { o.pfb11 } into pfbTemp
                                  select new
                                  {
                                      pfb11 = pfbTemp.Key.pfb11
                                  }
                               ;

                if (pfbDistinct == null)
                    return true;
                //查詢
                querySql = @"SELECT COUNT(1)
                        FROM pea_tb
                        WHERE pea01=@pea01
	                        AND peastat='9'
                        ";
                //逐筆更新
                updateSql = @"UPDATE dbo.pea_tb
                            SET peastat='1'
                            WHERE pea01=@pea01
                        ";

                foreach (var pfbModel in pfbDistinct)
                {
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@pea01", pfbModel.pfb11));
                    chkCnts = GlobalFn.isNullRet(BoPur.OfGetFieldValue(querySql, sqlParms.ToArray()), 0);
                    if (chkCnts == 0)
                        continue;
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@pea01", pfbModel.pfb11));
                    if (BoPur.OfExecuteNonquery(updateSql, sqlParms.ToArray()) < 1)
                    {
                        WfShowErrorMsg("更新peastat失敗!");
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

        #region WfSetDetailAmt 處理小計
        private bool WfSetDetailAmt(DataRow drPfb)
        {
            pfa_tb pfaModel;
            pfb_tb pfbModel;
            decimal pfb10t = 0, pfb10 = 0;
            try
            {
                pfaModel = DrMaster.ToItem<pfa_tb>();
                pfbModel = drPfb.ToItem<pfb_tb>();

                if (pfaModel.pfa08 == "Y")//稅內含
                {
                    pfb10t = pfbModel.pfb09 * pfbModel.pfb05;
                    pfb10t = GlobalFn.Round(pfb10t, BekModel.bek04);
                    pfb10 = pfb10t / (1 + (pfaModel.pfa07 / 100));
                    pfb10 = GlobalFn.Round(pfb10, BekModel.bek04);
                }
                else//稅外加
                {
                    pfb10 = pfbModel.pfb09 * pfbModel.pfb05;
                    pfb10 = GlobalFn.Round(pfb10, BekModel.bek04);
                    pfb10t = pfb10 * (1 + (pfaModel.pfa07 / 100));
                    pfb10t = GlobalFn.Round(pfb10t, BekModel.bek04);
                }
                drPfb["pfb10"] = pfb10;
                drPfb["pfb10t"] = pfb10t;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetTotalAmt 處理總計及數量
        private bool WfSetTotalAmt()
        {
            pfa_tb pfaModel;
            decimal tot_cnts = 0;
            decimal pfa13 = 0, pfa13t = 0, pfa13g;
            try
            {
                pfaModel = DrMaster.ToItem<pfa_tb>();
                if (pfaModel.pfa08 == "Y")//稅內含,先處理含稅金額
                {
                    foreach (pfb_tb l_pfb in TabDetailList[0].DtSource.ToList<pfb_tb>())
                    {
                        pfa13t += l_pfb.pfb10t;
                        tot_cnts += l_pfb.pfb05;

                    }
                    pfa13t = GlobalFn.Round(pfa13t, BekModel.bek04);
                    pfa13 = pfa13t / (1 + pfaModel.pfa07 / 100);
                    pfa13 = GlobalFn.Round(pfa13, BekModel.bek04);
                    pfa13g = pfa13t - pfa13;
                }
                else//稅外加
                {
                    foreach (pfb_tb l_pfb in TabDetailList[0].DtSource.ToList<pfb_tb>())
                    {
                        pfa13 += l_pfb.pfb10;
                        tot_cnts += l_pfb.pfb05;
                    }
                    pfa13 = GlobalFn.Round(pfa13, BekModel.bek04);
                    pfa13g = pfa13 * (pfaModel.pfa07 / 100);
                    pfa13g = GlobalFn.Round(pfa13g, BekModel.bek04);
                    pfa13t = pfa13 + pfa13g;
                }

                DrMaster["pfa13"] = pfa13;
                DrMaster["pfa13t"] = pfa13t;
                DrMaster["pfa13g"] = pfa13g;
                DrMaster["tot_cnts"] = tot_cnts;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkPgaExists 檢查入庫單是否存在
        private bool WfChkPgaExists(string pPfa01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChkCnts = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM pga_tb");
                sbSql.AppendLine("  INNER JOIN pgb_tb ON pga01=pgb01");
                sbSql.AppendLine("WHERE");
                sbSql.AppendLine(" pgaconf <>'X' ");
                sbSql.AppendLine(" AND pgb11=@pgb11 ");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@pgb11", pPfa01));
                iChkCnts = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
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

        #region WfResetAmtByPfa06 依稅別更新單身及單頭的金額
        private void WfResetAmtByPfa06()
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

        #region WfUpdPdd 訂單確認時,更新客戶產品價格檔
        private bool WfUpdPdd(vw_purt300 pMasterModel, List<vw_purt300s> pDetailList)
        {
            PurBLL boPddAdd = null;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            DataTable dtPdd = null;
            DataRow drPdd = null;            
            try
            {
                boPddAdd = new PurBLL(BoMaster.OfGetConntion());
                boPddAdd.TRAN = BoMaster.TRAN;
                boPddAdd.OfCreateDao("pdd_tb", "*", "");
                sqlSelect = @"SELECT * FROM pdd_tb
                              WHERE pdd01=@pdd01 AND pdd02=@pdd02 AND pdd03=@pdd03
                            ";
                if (BekModel == null)
                    BekModel = BoBas.OfGetBekModel(pMasterModel.pfa10);
                if (BekModel==null)
                {
                    WfShowErrorMsg("查無幣別資料!");
                    return false;
                }

                foreach (vw_purt300s detailModel in pDetailList)
                {
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@pdd01", detailModel.pfb03)); //料號
                    sqlParmList.Add(new SqlParameter("@pdd02", pMasterModel.pfa03)); //廠商編號
                    sqlParmList.Add(new SqlParameter("@pdd03", pMasterModel.pfa10)); //幣別
                    dtPdd = boPddAdd.OfGetDataTable(sqlSelect, sqlParmList.ToArray());
                    if (dtPdd.Rows.Count == 0) //新增
                    {
                        drPdd = dtPdd.NewRow();
                        drPdd["pdd01"] = detailModel.pfb03;//料號
                        drPdd["pdd02"] = pMasterModel.pfa03;//廠商編號
                        drPdd["pdd03"] = pMasterModel.pfa10;//幣別
                        drPdd["pdd04"] = pMasterModel.pfa17;//匯率
                        drPdd["pdd05"] = pMasterModel.pfa02;//最近採購日期
                        drPdd["pdd06"] = pMasterModel.pfa06;//稅別
                        drPdd["pdd07"] = pMasterModel.pfa07;//稅率
                        drPdd["pdd08"] = pMasterModel.pfa08;//含稅否

                        if (pMasterModel.pfa08=="Y")
                        {
                            drPdd["pdd10"] = detailModel.pfb09;//含稅單價
                            drPdd["pdd09"] = GlobalFn.Round(detailModel.pfb09 / (1 + pMasterModel.pfa07 / 100),BekModel.bek03);
                        }
                        else
                        {
                            drPdd["pdd09"] = detailModel.pfb09;//未稅單價
                            drPdd["pdd10"] = GlobalFn.Round(detailModel.pfb09 * (1 + pMasterModel.pfa07 / 100), BekModel.bek03);
                        }
                        drPdd["pdd11"] = detailModel.pfb05;//最近採購數量
                        drPdd["pdd12"] = detailModel.pfb06;//最近採購單位

                        drPdd["pddcreu"] = LoginInfo.UserNo;
                        drPdd["pddcreg"] = LoginInfo.DeptNo;
                        drPdd["pddcred"] = Now;
                        drPdd["pddsecu"] = LoginInfo.UserNo;
                        drPdd["pddsecg"] = LoginInfo.GroupNo;
                        dtPdd.Rows.Add(drPdd);
                    }
                    else    //修改
                    {
                        drPdd = dtPdd.Rows[0];
                        //drPdd["pdd01"] = detailModel.pfb03;//料號
                        //drPdd["pdd02"] = pMasterModel.pfa03;//廠商編號
                        //drPdd["pdd03"] = pMasterModel.pfa10;//幣別
                        drPdd["pdd04"] = pMasterModel.pfa17;//匯率
                        drPdd["pdd05"] = pMasterModel.pfa02;//最近採購日期
                        drPdd["pdd06"] = pMasterModel.pfa06;//稅別
                        drPdd["pdd07"] = pMasterModel.pfa07;//稅率
                        drPdd["pdd08"] = pMasterModel.pfa08;//含稅否

                        if (pMasterModel.pfa08 == "Y")
                        {
                            drPdd["pdd10"] = detailModel.pfb09;//含稅單價
                            drPdd["pdd09"] = GlobalFn.Round(detailModel.pfb09 / (1 + pMasterModel.pfa07 / 100), BekModel.bek03);
                        }
                        else
                        {
                            drPdd["pdd09"] = detailModel.pfb09;//未稅單價
                            drPdd["pdd10"] = GlobalFn.Round(detailModel.pfb09 * (1 + pMasterModel.pfa07 / 100), BekModel.bek03);
                        }
                        drPdd["pdd11"] = detailModel.pfb05;//最近採購數量
                        drPdd["pdd12"] = detailModel.pfb06;//最近採購單位

                        drPdd["pddcreu"] = LoginInfo.UserNo;
                        drPdd["pddcreg"] = LoginInfo.DeptNo;
                        drPdd["pddcred"] = Now;
                        drPdd["pddsecu"] = LoginInfo.UserNo;
                        drPdd["pddsecg"] = LoginInfo.GroupNo;
                    }

                    if (boPddAdd.OfUpdate(dtPdd) < 1)
                    {
                        WfShowErrorMsg("異動料號廠商金額(pdd_tb)失敗!");
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
    }
}
