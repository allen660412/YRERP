/* 程式名稱: 銷退單維護作業
   系統代號: stpt500
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

namespace YR.ERP.Forms.Stp
{
    public partial class FrmStpt500 : YR.ERP.Base.Forms.FrmEntryMDBase
    {

        #region Property
        BasBLL BoBas = null;
        StpBLL BoStp = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;

        //baa_tb BaaTbModel = null;
        bek_tb BekModel = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmStpt500()
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
            this.StrFormID = "stpt500";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("sha01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "shasecu";
                TabMaster.GroupColumn = "shasecg";

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
                WfSetUcomboxDataSource(ucb_sha06, sourceList);

                //發票聯數
                sourceList = BoStp.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_sha09, sourceList);

                //單據確認
                sourceList = BoStp.OfGetShaconfKVPList();
                WfSetUcomboxDataSource(ucb_shaconf, sourceList);
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

            this.TabDetailList[0].TargetTable = "shb_tb";
            this.TabDetailList[0].ViewTable = "vw_stpt500s";
            keyParm = new SqlParameter("shb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "sha01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
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
                        ugc = uGrid.DisplayLayout.Bands[0].Columns["shb17"];//退回類型
                        WfSetGridValueList(ugc, BoStp.OfGetShb17KVPList());
                        break;
                }
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
            vw_stpt500 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_stpt500>();
                    WfSetDocPicture("", masterModel.shaconf, masterModel.shastat, pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.sha10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekModel = BoBas.OfGetBekModel(masterModel.sha10);
                        if (BekModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.sha10));
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

                    WfSetControlReadonly(uGridMaster, true);//grid不可編輯

                    WfSetControlReadonly(new List<Control> { ute_shacreu, ute_shacreg, udt_shacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_shamodu, ute_shamodg, udt_shamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_shasecu, ute_shasecg }, true);

                    if (GlobalFn.isNullRet(masterModel.sha01, "") != "")
                    {
                        WfSetControlReadonly(ute_sha01, true);
                        WfSetSha01RelReadonly(GlobalFn.isNullRet(masterModel.sha01, ""));
                    }
                    else
                        WfSetControlReadonly(ute_sha11, true);

                    if (!GlobalFn.varIsNull(masterModel.sha11)) //客戶狀態處理
                        WfSetControlReadonly(ute_sha03, true);
                    
                    WfSetControlReadonly(new List<Control> { ute_sha01_c, ute_sha03_c, ute_sha04_c, ute_sha05_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_sha07, ucx_sha08 }, true);
                    WfSetControlReadonly(new List<Control> { ute_sha13, ute_sha13t, ute_sha13g }, true);
                    WfSetControlReadonly(new List<Control> { ucb_shaconf, udt_shacond, ute_shaconu, ute_shastat }, true);
                    if (FormEditMode == YREditType.修改)
                    {
                        if (GlobalFn.varIsNull(masterModel.sha11))
                            WfSetControlReadonly(ute_sha03, false);
                        else
                            WfSetControlReadonly(ute_sha03, true);
                    }

                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_sha01, true);
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
            bab_tb babModel;
            vw_stpt500s detailModel;
            try
            {
                switch (pCurTabDetail)
                {
                    case 0:
                        detailModel = pDr.ToItem<vw_stpt500s>();
                        foreach (UltraGridCell ugc in pUgr.Cells)
                        {
                            columnName = ugc.Column.Key.ToLower();
                            //先控可以輸入的
                            if (
                                columnName == "shb06" ||
                                columnName == "shb09" ||
                                columnName == "shb16" ||
                                columnName == "shb17" ||
                                columnName == "shb19" ||
                                columnName == "shb20"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "shb02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }

                            if (columnName == "shb03" ||
                                columnName == "shb11" ||
                                columnName == "shb12"
                                )
                            {
                                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sha01"], ""));
                                if (babModel == null)
                                {
                                    WfShowErrorMsg("請先輸銷退單單頭資料!");
                                }
                                if (GlobalFn.isNullRet(babModel.bab08, "") == "Y")    //有來源單據
                                {
                                    if (columnName == "shb03")//料號
                                        WfSetControlReadonly(ugc, true);    //不可輸入
                                    else
                                        WfSetControlReadonly(ugc, false);
                                }
                                else
                                {
                                    if (columnName == "shb03")//料號
                                        WfSetControlReadonly(ugc, false);    //可輸入
                                    else
                                        WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }

                            if (columnName == "shb05"   //銷退數量
                                )
                            {
                                if (detailModel.shb17 == "1")//銷退
                                    WfSetControlReadonly(ugc, false);    //可輸入
                                else  //折讓
                                    WfSetControlReadonly(ugc, true);    //不可輸入
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
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_stpt500
                if (pDr.Table.Prefix.ToLower() == "vw_stpt500")
                {
                    switch (pColName.ToLower())
                    {
                        case "sha01"://銷貨單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "stp"));
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "40"));
                            WfShowPickUtility("p_bab1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "sha03"://客戶編號
                            WfShowPickUtility("p_sca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sca01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "sha04"://業務人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "sha05"://業務部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;


                        case "sfa10"://幣別
                            WfShowPickUtility("p_bek1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bek01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sha11"://出貨單號
                            WfShowPickUtility("p_sgb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sgb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_stpt500s
                if (pDr.Table.Prefix.ToLower() == "vw_stpt500s")
                {
                    switch (pColName.ToLower())
                    {
                        case "shb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "shb06"://銷貨單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "shb11"://出貨單號
                            WfShowPickUtility("p_sgb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                {
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sgb01"], "");
                                    pDr["shb12"] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sgb02"], "");
                                }
                                else
                                {
                                    pDr[pColName] = "";
                                    pDr["shb12"] = "";
                                }
                            }
                            break;

                        case "shb16"://倉庫
                            WfShowPickUtility("p_icb1", messageModel);
                            if (messageModel != null && messageModel.DataRowList.Count > 0)
                            {
                                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icb01"], "");
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
                pDr["sha02"] = Today;
                pDr["sha04"] = LoginInfo.UserNo;
                pDr["sha04_c"] = LoginInfo.UserName;
                pDr["sha05"] = LoginInfo.DeptNo;
                pDr["sha05_c"] = LoginInfo.DeptName;
                pDr["sha07"] = 0;
                pDr["sha08"] = "N";
                pDr["sha10"] = BaaModel.baa04;
                pDr["sha13"] = 0;
                pDr["sha13t"] = 0;
                pDr["sha13g"] = 0;
                pDr["sha14"] = 1;       //匯率
                pDr["shaconf"] = "N";
                pDr["shastat"] = "1";
                pDr["shacomp"] = LoginInfo.CompNo;
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
                        pDr["shb02"] = WfGetMaxSeq(pDr.Table, "shb02");
                        pDr["shb05"] = 0;
                        pDr["shb08"] = 0;
                        pDr["shb09"] = 0;
                        pDr["shb10"] = 0;
                        pDr["shb10t"] = 0;
                        pDr["shb11"] = DrMaster["sha11"];
                        pDr["shb13"] = 0;
                        pDr["shb14"] = 0;
                        pDr["shb17"] = "1"; //銷退
                        pDr["shb18"] = 0;
                        pDr["shbcomp"] = LoginInfo.CompNo;
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
            vw_stpt500 masterModel;
            try
            {
                //BaaModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_stpt500>();
                if (masterModel.shaconf != "N")
                    return false;
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
            vw_stpt500 masterModel = null;
            vw_stpt500s detailModel = null;
            List<vw_stpt500s> detailList = null;
            bab_tb babModel = null;
            sga_tb sgaModel = null;
            UltraGrid uGrid = null;
            int ChkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt500>();
                if (e.Column.ToLower() != "sha01" && GlobalFn.isNullRet(DrMaster["sha01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false;
                }
                #region 單頭-pick vw_stpt500
                if (e.Row.Table.Prefix.ToLower() == "vw_stpt500")
                {

                    switch (e.Column.ToLower())
                    {
                        case "sha01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "stp", "40") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["sha01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            WfSetSha01RelReadonly(GlobalFn.isNullRet(e.Value, ""));
                            if (ute_sha11.ReadOnly != true)
                                WfItemChkForceFocus(ute_sha11);
                            break;
                        case "sha02":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            var result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(e.Value), BaaModel);
                            if (result.Success == false)
                            {
                                WfShowErrorMsg(result.Message);
                                return false;
                            }
                            break;
                        case "sha03"://客戶
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sha03_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkScaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此客戶資料,請檢核!");
                                return false;
                            }
                            WfSetSha03Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtBySha06();                            
                            break;
                            
                        case "sha04"://業務人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sha04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["sha04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;
                        case "sha05"://業務部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sha05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["sha05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            
                            break;
                            
                        case "sha06"://課稅別
                            WfSetSha06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtBySha06();
                            break;
                        case "sha10"://幣別
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoBas.OfChkBekPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此幣別");
                                return false;
                            }
                            BekModel = BoBas.OfGetBekModel(e.Value.ToString());
                            break;
                            
                        case "sha11"://銷退單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfSetControlReadonly(new List<Control> { ute_sha03 }, false);
                                return true;
                            }
                            sgaModel = BoStp.OfGetSgaModel(GlobalFn.isNullRet(e.Value, ""));
                            if (sgaModel == null)
                            {
                                WfShowErrorMsg("無此出貨!");
                                return false;
                            }
                            if (sgaModel.sgaconf != "Y")
                            {
                                WfShowErrorMsg("出貨單非確認狀態!");
                                return false;
                            }
                            
                            //檢查與單身是否一致!
                            detailList = TabDetailList[0].DtSource.ToList<vw_stpt500s>();
                            ChkCnts = detailList.Where(p => p.shb11 != sgaModel.sga01 && GlobalFn.isNullRet(p.shb11, "") != "")
                                               .Count();
                            if (ChkCnts > 0)
                            {
                                WfShowErrorMsg("單頭與單身的銷貨單單號不一致!");
                                return false;
                            }
                            //這裡會有客戶跟銷退單單搶壓資料的問題:先以銷退單單為主
                            DrMaster["sha03"] = sgaModel.sga03;
                            DrMaster["sha03_c"] = BoStp.OfGetSca02(sgaModel.sga03);
                            WfSetControlReadonly(new List<Control> { ute_sha03 }, true);
                            DrMaster["sha06"] = sgaModel.sga06;
                            DrMaster["sha07"] = sgaModel.sga07;
                            DrMaster["sha08"] = sgaModel.sga08;
                            DrMaster["sha09"] = sgaModel.sga09;
                            DrMaster["sha10"] = sgaModel.sga10;
                            break;
                        case "sha14": //匯率
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
                
                #region 單身-pick vw_stpt500s
                if (e.Row.Table.Prefix.ToLower() == "vw_stpt500s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_stpt500s>();
                    detailList = e.Row.Table.ToList<vw_stpt500s>();
                    babModel = BoBas.OfGetBabModel(masterModel.sha01);

                    switch (e.Column.ToLower())
                    {
                        case "shb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            detailList = e.Row.Table.ToList<vw_stpt500s>();
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.shb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "shb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["shb04"] = "";//品名
                                e.Row["shb05"] = 0;//銷退數量
                                e.Row["shb06"] = "";//銷退單位
                                e.Row["shb07"] = "";//庫存單位
                                e.Row["shb08"] = 0;//庫存轉換率
                                e.Row["shb13"] = 0;//出貨單轉換率
                                e.Row["shb14"] = 0;//出貨數量
                                e.Row["shb15"] = "";//出貨單單位
                                e.Row["shb18"] = 0;//庫存數量
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }

                            if (WfSetShb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;
                            
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "shb05"://銷退數量
                            detailModel.shb05 = BoBas.OfGetUnitRoundQty(detailModel.shb06, detailModel.shb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detailModel.shb05;
                            if (WfChkShb05(e.Row, detailModel) == false)
                                return false;
                            e.Row["shb14"] = BoBas.OfGetUnitRoundQty(detailModel.shb06, detailModel.shb05 * detailModel.shb13); //轉換cfb單數量(並四拾伍入)
                            e.Row["shb18"] = BoBas.OfGetUnitRoundQty(detailModel.shb07, detailModel.shb05 * detailModel.shb08); //轉換庫存數量(並四拾伍入)
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "shb06"://銷貨單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (GlobalFn.varIsNull(detailModel.shb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["shb03"]);
                                return false;
                            }
                            if (WfChkShb06(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;

                            if (WfSetShb06Relation(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;
                            break;
                        case "shb09":   //單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.shb09 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.shb09, BekModel.bek03);
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "shb11"://銷貨單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("銷退單單號不可為空白!");
                                return false;
                            }

                            if (WfChkShb11(e.Value.ToString()) == false)
                                return false;

                            if (!GlobalFn.varIsNull(detailModel.shb12))
                            {
                                if (WfChkShb12(GlobalFn.isNullRet(detailModel.shb11, ""), GlobalFn.isNullRet(detailModel.shb12, 0)) == false)
                                    return false;

                                if (WfSetShb12Relation(e.Row, GlobalFn.isNullRet(detailModel.shb11, ""), GlobalFn.isNullRet(detailModel.shb12, 0)) == false)
                                    return false;
                            }
                            WfSetTotalAmt();
                            break;

                        case "shb12"://銷貨項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("銷貨項次不可為空白!");
                                return false;
                            }

                            if (WfChkShb12(GlobalFn.isNullRet(detailModel.shb11, ""), GlobalFn.isNullRet(detailModel.shb12, 0)) == false)
                                return false;
                            WfSetShb12Relation(e.Row, GlobalFn.isNullRet(detailModel.shb11, ""), GlobalFn.isNullRet(detailModel.shb12, 0));
                            WfSetTotalAmt();
                            break;

                        case "shb16"://倉庫
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

                        case "shb17"://退回類型
                            //if (listDetail.shb17 == "1")  //銷退
                            //{
                            //    WfSetDetailAmt(pDr);
                            //}
                            if (detailModel.shb17 == "2")//拆讓時
                            {
                                //pDr["shb09"] = 0;//單價
                                e.Row["shb05"] = 0;//數量
                                e.Row["shb14"] = 0;//轉換銷貨數量
                                e.Row["shb18"] = 0;//轉換庫存數量
                            }
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            WfSetDetailDisplayMode(0, TabDetailList[0].UGrid.ActiveRow, e.Row);
                            break;
                        case "shb19"://發票號碼
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (e.Value.ToString().Length != 10)
                            {
                                WfShowErrorMsg("發票長度應為10碼!");
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
                    case "vw_stpt500s":
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
            vw_stpt500 masterModel = null;
            vw_stpt500s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {

                masterModel = DrMaster.ToItem<vw_stpt500>();
                if (!GlobalFn.varIsNull(masterModel.sha01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.sha01, ""));
                #region 單頭資料檢查
                chkColName = "sha01";       //銷退單號
                chkControl = ute_sha01;
                if (GlobalFn.varIsNull(masterModel.sha01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sha02";       //出庫日期
                chkControl = udt_sha02;
                if (GlobalFn.varIsNull(masterModel.sha02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.sha02), BaaModel);
                if (result.Success == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                chkColName = "sha03";       //客戶編號
                chkControl = ute_sha03;
                if (GlobalFn.varIsNull(masterModel.sha03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sha04";       //業務人員
                chkControl = ute_sha04;
                if (GlobalFn.varIsNull(masterModel.sha04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sha05";       //業務部門
                chkControl = ute_sha05;
                if (GlobalFn.varIsNull(masterModel.sha05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sha06";       //課稅別
                chkControl = ucb_sha06;
                if (GlobalFn.varIsNull(masterModel.sha06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                
                chkColName = "sha14";
                chkControl = ute_sha14;
                if (GlobalFn.varIsNull(masterModel.sha14))
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

                    detailModel = drTemp.ToItem<vw_stpt500s>();
                    chkColName = "shb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.shb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "shb11";   //銷單單號
                    if (GlobalFn.varIsNull(detailModel.shb11) && babModel.bab08 == "Y")  //有來源單據銷貨單單號要輸入
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "shb12";   //銷貨單項次
                    if (GlobalFn.varIsNull(detailModel.shb12) && babModel.bab08 == "Y")  //有來源單據銷貨單單號要輸入
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "shb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.shb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "shb05";   //銷退數量
                    #region pfb05 銷退數量
                    if (detailModel.shb17 == "1")//退回
                    {
                        if (GlobalFn.varIsNull(detailModel.shb05) || detailModel.shb05 <= 0)
                        {
                            this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                            msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                            msg += "不可為空白或小於0!";
                            WfShowErrorMsg(msg);
                            WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                            return false;
                        }
                    }
                    else //折讓
                    {
                        if (GlobalFn.varIsNull(detailModel.shb05) || detailModel.shb05 != 0)
                        {
                            this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                            msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                            msg += "應為0!";
                            WfShowErrorMsg(msg);
                            WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                            return false;
                        }
                    }

                    if (WfChkShb05(drTemp, detailModel) == false)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    chkColName = "shb06";   //銷退單位
                    if (GlobalFn.varIsNull(detailModel.shb06))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "shb16";   //倉庫別
                    if (GlobalFn.varIsNull(detailModel.shb16))
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
            string sha01New, errMsg;
            vw_stpt500 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt500>();
                if (FormEditMode == YREditType.新增)
                {
                    sha01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.sha01, ModuleType.stp, (DateTime)masterModel.sha02, out sha01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["sha01"] = sha01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["shasecu"] = LoginInfo.UserNo;
                        DrMaster["shasecg"] = LoginInfo.GroupNo;
                        DrMaster["shacreu"] = LoginInfo.UserNo;
                        DrMaster["shacreg"] = LoginInfo.DeptNo;
                        DrMaster["shacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["shamodu"] = LoginInfo.UserNo;
                        DrMaster["shamodg"] = LoginInfo.DeptNo;
                        DrMaster["shamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["shbcreu"] = LoginInfo.UserNo;
                            drDetail["shbcreg"] = LoginInfo.DeptNo;
                            drDetail["shbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["shbmodu"] = LoginInfo.UserNo;
                            drDetail["shbmodg"] = LoginInfo.DeptNo;
                            drDetail["shbmodd"] = Now;
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

                bt = new ButtonTool("Stpr500");
                adoModel = BoAdm.OfGetAdoModel("stpr500");
                bt.SharedProps.Caption = adoModel.ado02;
                bt.SharedProps.Category = "Report";
                bt.Tag = adoModel;
                buttonList.Add(bt);

                bt = new ButtonTool("Stpr501");
                adoModel = BoAdm.OfGetAdoModel("stpr501");
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
            vw_stpt500 masterModel;
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

                    case "Stpr500":
                        vw_stpr500 stpr500Model;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        masterModel = DrMaster.ToItem<vw_stpt500>();
                        stpr500Model = new vw_stpr500();
                        stpr500Model.sha01 = masterModel.sha01;
                        stpr500Model.sha03 = "";
                        stpr500Model.jump_yn = "N";
                        stpr500Model.order_by = "1";

                        FrmStpr500 rpt = new FrmStpr500(this.LoginInfo, stpr500Model, true, true);
                        rpt.WindowState = FormWindowState.Minimized;
                        rpt.ShowInTaskbar = false;
                        rpt.Show();
                        break;

                    case "Stpr501":
                        vw_stpr501 stpr501Model;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        masterModel = DrMaster.ToItem<vw_stpt500>();
                        stpr501Model = new vw_stpr501();
                        stpr501Model.sha01 = masterModel.sha01;
                        stpr501Model.sha03 = "";
                        stpr501Model.jump_yn = "N";
                        stpr501Model.order_by = "1";

                        FrmStpr501 rptStpr501 = new FrmStpr501(this.LoginInfo, stpr501Model, true, true);
                        rptStpr501.WindowState = FormWindowState.Minimized;
                        rptStpr501.ShowInTaskbar = false;
                        rptStpr501.Show();
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
            vw_stpt500s detailModel = null;
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
                    detailModel = dr.ToItem<vw_stpt500s>();
                    if (BoInv.OfChkIccPKExists(detailModel.shb03, detailModel.shb16) == false)
                    {
                        if (dtIcc.Rows.Count > 0)
                        {
                            var drIccs = dtIcc.Select(string.Format("icc01='{0}' AND icc02='{1}'", detailModel.shb03, detailModel.shb16));
                            if (drIccs != null && drIccs.Length > 0)
                                continue;
                        }
                        drIcc = dtIcc.NewRow();
                        drIcc["icc01"] = detailModel.shb03;  //料號
                        drIcc["icc02"] = detailModel.shb16;
                        drIcc["icc03"] = "";
                        drIcc["icc04"] = detailModel.shb07;
                        drIcc["icc05"] = 0;
                        drIcc["icccomp"] = detailModel.shbcomp;
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

        //*****************************表單自opbFuction****************************************
        #region WfSetSha01RelReadonly 依單別設定相關控制項 readonly
        private void WfSetSha01RelReadonly(string pSha01)
        {
            bab_tb babModel = null;
            string bab01;
            try
            {
                WfSetControlReadonly(new List<Control> { ute_sha01 }, true);
                bab01 = pSha01.Substring(0, GlobalFn.isNullRet(BaaModel.baa06, 0));
                babModel = BoBas.OfGetBabModel(bab01);
                if (babModel.bab08 == "Y")   //有來源單據
                {
                    WfSetControlReadonly(new List<Control> { ute_sha11 }, false);
                }
                else
                {
                    WfSetControlReadonly(new List<Control> { ute_sha11 }, true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        
        #region WfSetSha03Relation 設定客戶相關聯
        private void WfSetSha03Relation(string pSha03)
        {
            sca_tb scaModel;
            bab_tb babModel = null;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sha01"], ""));
                scaModel = BoStp.OfGetScaModel(pSha03);
                if (scaModel == null)
                    return;

                DrMaster["sha03_c"] = scaModel.sca03;
                DrMaster["sha06"] = scaModel.sca22;    //課稅別
                WfSetSha06Relation(scaModel.sca22);
                DrMaster["sha09"] = scaModel.sca23;    //發票聯數

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        
        #region WfSetSha06Relation 設定稅別關聯
        private void WfSetSha06Relation(string pSha06)
        {
            try
            {
                if (pSha06 == "1")
                {
                    DrMaster["sha07"] = BaaModel.baa05;
                    DrMaster["sha08"] = "Y";
                }
                else if (pSha06 == "2")
                {
                    DrMaster["sha07"] = BaaModel.baa05;
                    DrMaster["sha08"] = "N";
                }
                else
                {
                    DrMaster["sha07"] = 0;
                    DrMaster["sha08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetShb03Relation 設定料號關聯
        private bool WfSetShb03Relation(string pShb03, DataRow pDr)
        {
            ica_tb icaModel;
            decimal shb08;   //轉換率
            try
            {
                icaModel = BoInv.OfGetIcaModel(pShb03);
                shb08 = 0;
                if (icaModel == null)
                {
                    pDr["shb04"] = "";//品名
                    pDr["shb05"] = 0;//退貨數量
                    pDr["shb06"] = "";//退貨單位
                    pDr["shb07"] = "";//庫存單位
                    pDr["shb08"] = 0;//庫存轉換率
                    pDr["shb18"] = 0;//庫存數量
                }
                else
                {
                    if (BoInv.OfGetUnitCovert(pShb03, icaModel.ica09, icaModel.ica07, out shb08) == false)
                    {
                        WfShowErrorMsg("未設定料號轉換,請先設定!");
                        return false;
                    }
                    pDr["shb04"] = icaModel.ica02;//品名
                    pDr["shb05"] = 0;//退貨數量
                    pDr["shb06"] = icaModel.ica09;//退貨單位帶銷售單位
                    pDr["shb07"] = icaModel.ica07;//庫存單位
                    pDr["shb08"] = shb08;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetShb06Relation 設定銷退單位關聯
        //檢查前要先確認料號是否已輸入
        private bool WfSetShb06Relation(DataRow pDr, string pShb06, string pBab08)
        {
            vw_stpt500s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_stpt500s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pShb06, "")) == false)
                {
                    WfShowErrorMsg("無此銷售單位!請確認");
                    return false;
                }
                //取得是否有銷退對庫存的轉換率
                dConvert = 0;
                if (BoInv.OfGetUnitCovert(detailModel.shb03, pShb06, detailModel.shb07, out dConvert) == true)
                {
                    pDr["shb08"] = dConvert;
                    pDr["shb18"] = BoBas.OfGetUnitRoundQty(detailModel.shb07, detailModel.shb05 * dConvert); //轉換庫存數量(並四拾伍入)
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有銷退對銷貨單的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.shb03, pShb06, detailModel.shb15, out dConvert) == true)
                    {
                        pDr["shb13"] = dConvert;
                        pDr["shb14"] = BoBas.OfGetUnitRoundQty(detailModel.shb15, detailModel.shb05 * dConvert); //轉換庫存數量(並四拾伍入)
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

        #region WfSetShb12Relation 設定銷貨單明細資料關聯
        private bool WfSetShb12Relation(DataRow pdr, string pShb11, int pShb12)
        {
            sgb_tb sgbModel;
            decimal sgb18;
            try
            {
                sgbModel = BoStp.OfGetSgbModel(pShb11, pShb12);
                if (sgbModel == null)
                    return false;
                sgb18 = BoBas.OfGetUnitRoundQty(sgbModel.sgb07, sgbModel.sgb05 * sgbModel.sgb08); //轉換庫存數量(並四拾伍入) 先取,避免錯誤

                pdr["shb03"] = sgbModel.sgb03;//料號
                pdr["shb04"] = sgbModel.sgb04;//品名
                pdr["shb05"] = sgbModel.sgb05;//銷售數量
                pdr["shb06"] = sgbModel.sgb06;//銷售單位
                pdr["shb07"] = sgbModel.sgb07;//庫存單位
                pdr["shb08"] = sgbModel.sgb08;//庫存轉換率
                pdr["shb09"] = sgbModel.sgb09;//單價
                pdr["shb10"] = sgbModel.sgb10;//未稅金額
                pdr["shb10t"] = sgbModel.sgb10t;//含稅金額

                pdr["shb13"] = 1;          //銷退對銷貨單轉換率
                pdr["shb14"] = sgbModel.sgb05;//轉換銷貨數量
                pdr["shb15"] = sgbModel.sgb06;//原銷貨單單位
                pdr["shb16"] = sgbModel.sgb16;//倉庫別

                pdr["shb18"] = sgb18; //轉換庫存數量(並四拾伍入)
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkShb05 數量檢查
        private bool WfChkShb05(DataRow pdr, vw_stpt500s pDetailModel)
        {
            List<vw_stpt500s> detailList = null;
            sgb_tb sgbModel = null;
            bab_tb babModel = null;
            string errMsg;
            decimal docThisQty = 0;         //本項次的轉換銷貨單數量
            decimal docOtherQtyTotal = 0;   //本單據其他項次的銷貨單數量加總
            decimal otherQtyTotal = 0;      //其他單據的數量加總
            StringBuilder sbSql;
            List<SqlParameter> sqlParms;
            try
            {
                detailList = pdr.Table.ToList<vw_stpt500s>();
                if (GlobalFn.varIsNull(pDetailModel.shb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pDetailModel.shb03))
                {
                    errMsg = "請先輸入料號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pDetailModel.shb06))
                {
                    errMsg = "請先輸入銷退單位!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                //if (pListDetail.shb05 <= 0)
                //{
                //    errMsg = "銷退數量應大於0!";
                //    WfShowMsg(errMsg);
                //    return false;
                //}
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sha01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (babModel.bab08 == "Y" && pDetailModel.shb17 == "1") //銷退性質時,檢查退回數量是否超過銷貨單數量!
                {
                    sgbModel = BoStp.OfGetSgbModel(pDetailModel.shb11, Convert.ToInt16(pDetailModel.shb12));
                    if (babModel == null)
                    {
                        errMsg = "Get sgb_tb Error!";
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    //取得本身單據中的所有數量轉換銷貨單位後的總和!
                    //先取本身這筆
                    docThisQty = BoBas.OfGetUnitRoundQty(pDetailModel.shb15, pDetailModel.shb05 * pDetailModel.shb13);
                    //再取其他筆加總
                    docOtherQtyTotal = detailList.Where(p => p.shb02 != pDetailModel.shb02)
                                       .Where(p => p.shb11 == pDetailModel.shb11 && p.shb12 == pDetailModel.shb12)
                                       .Sum(p => p.shb14);
                    //取得其他單據上的加總
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT SUM(shb14) FROM sha_tb");
                    sbSql.AppendLine("  INNER JOIN shb_tb ON sha01=shb01");
                    sbSql.AppendLine("WHERE shaconf<>'X'");
                    sbSql.AppendLine("  AND sha01 <> @sha01");
                    sbSql.AppendLine("  AND shb11 = @shb11 AND shb12 = @shb12");
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@sha01", GlobalFn.isNullRet(DrMaster["sha01"], "")));
                    sqlParms.Add(new SqlParameter("@shb11", pDetailModel.shb11));
                    sqlParms.Add(new SqlParameter("@shb12", pDetailModel.shb12));
                    otherQtyTotal = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParms.ToArray()), 0);
                    if (sgbModel.sgb05 < (docThisQty + docOtherQtyTotal + otherQtyTotal))
                    {
                        errMsg = string.Format("項次{0}銷貨單最大可輸入數量為 {1}",
                                                pDetailModel.shb02.ToString(),
                                                (sgbModel.sgb05 - docOtherQtyTotal - otherQtyTotal).ToString()
                                               );
                        WfShowErrorMsg(errMsg);
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

        #region WfChkShb06 檢查銷退單位
        //檢查前要先確認料號是否已輸入
        private bool WfChkShb06(DataRow pDr, string pShb06, string pBab08)
        {
            vw_stpt500s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_stpt500s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pShb06, "")) == false)
                {
                    WfShowErrorMsg("無此銷退單位!請確認");
                    return false;
                }
                //檢查是否有銷售對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.shb03, pShb06, detailModel.shb07, out dConvert) == false)
                {
                    WfShowErrorMsg("未設定銷退單位對庫存單位的轉換率,請先設定!");
                    return false;
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有銷退對銷貨單的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.shb03, pShb06, detailModel.shb06, out dConvert) == false)
                    {
                        WfShowErrorMsg("未設定銷退單位對銷貨單單位的轉換率,請先設定!");
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

        #region WfChkShb11 銷貨單檢查
        private bool WfChkShb11(string pShb11)
        {
            string errMsg;
            sga_tb sgaModel;
            sha_tb masterModel;
            string sha11;
            try
            {
                masterModel=DrMaster.ToItem<sha_tb>();
                sgaModel = BoStp.OfGetSgaModel(pShb11);
                if (sgaModel == null)
                {
                    errMsg = "查無此銷貨單!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (sgaModel.sgaconf != "Y")
                {
                    errMsg = "銷貨單非確認狀態!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                sha11 = GlobalFn.isNullRet(DrMaster["sha11"], "");
                if (sha11 != "" && sha11 != pShb11)
                {
                    errMsg = "銷退單單號與單頭不同!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (sgaModel.sga03 !=masterModel.sha03)
                {
                    errMsg = "客戶不同,請檢核!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (sgaModel.sga06 != masterModel.sha06)
                {
                    errMsg = "稅別不同,請檢核!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (sgaModel.sga09 != masterModel.sha09)
                {
                    errMsg = "發票聯數不同,請檢核!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (sgaModel.sga10 != masterModel.sha10)
                {
                    errMsg = "發票聯數不同,請檢核!";
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

        #region WfChkShb12 銷貨單項次檢查
        private bool WfChkShb12(string pShb11, int pShb12)
        {
            string errMsg;
            sgb_tb sgbModel;
            try
            {
                sgbModel = BoStp.OfGetSgbModel(pShb11, pShb12);
                if (sgbModel == null)
                {
                    errMsg = "查無此銷貨單項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if ((sgbModel.sgb05 - sgbModel.sgb17) <= 0)
                {
                    errMsg = string.Format("無可用銷貨單數量!(可用銷貨單數量為{0})", GlobalFn.isNullRet(sgbModel.sgb05 - sgbModel.sgb17, 0));
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
            sha_tb shaModel = null;
            shb_tb shbModel = null;

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

                shaModel = DrMaster.ToItem<sha_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                shaModel.shaconf = "Y";
                shaModel.shacond = Today;
                shaModel.shaconu = LoginInfo.UserNo;

                //更新銷貨單銷貨量
                if (WfUpdSgb17(true) == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    shbModel = dr.ToItem<shb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("1", shbModel.shb03, shbModel.shb16, shbModel.shb18, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                    //更新庫存交易歷史檔
                    if (BoInv.OfStockPost("stpt500", shaModel, shbModel, LoginInfo, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                }

                DrMaster["shaconf"] = "Y";
                DrMaster["shacond"] = Today;
                DrMaster["shaconu"] = LoginInfo.UserNo;
                DrMaster["shamodu"] = LoginInfo.UserNo;
                DrMaster["shamodg"] = LoginInfo.DeptNo;
                DrMaster["shamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                shaModel = DrMaster.ToItem<sha_tb>();
                WfSetDocPicture("", shaModel.shaconf, shaModel.shastat, pbxDoc);
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
            vw_stpt500 masterModel = null;
            vw_stpt500s detailModel = null;
            List<vw_stpt500s> detailList = null;
            bab_tb babModel = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList = null;
            string errMsg = "";
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt500>();
                if (masterModel.shaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.sha02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_stpt500s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_stpt500s>();
                    //檢查數量
                    if (WfChkShb05(drTemp, detailModel) == false)
                    {
                        return false;
                    }
                }
                //檢查銷退金額是否有大於來源單據
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sha01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (babModel.bab08 == "Y")
                {
                    var listSumShbAmt = from o in detailList
                                        where o.shb10 > 0 && o.shb11 != "" && o.shb12 != null
                                        group o by new { o.shb11, o.shb12 } into shbSum
                                        select new
                                        {
                                            shb11 = shbSum.Key.shb11,
                                            shb12 = shbSum.Key.shb12,
                                            shb10 = shbSum.Sum(p => p.shb10)
                                        }
                                   ;
                    foreach (var sumShb in listSumShbAmt)
                    {
                        sbSql = new StringBuilder();
                        sbSql.AppendLine("SELECT SUM(shb10) FROM shb_tb");
                        sbSql.AppendLine("  INNER JOIN sha_tb ON shb01=sha01");
                        sbSql.AppendLine("WHERE shb01<>@shb01");
                        sbSql.AppendLine("AND shaconf='Y'");
                        sbSql.AppendLine("AND shb11=@shb11 AND shb12=@shb12");
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@shb01", GlobalFn.isNullRet(DrMaster["sha01"], "")));
                        sqlParmList.Add(new SqlParameter("@shb11", sumShb.shb11));
                        sqlParmList.Add(new SqlParameter("@shb12", sumShb.shb12));
                        var otherDocTot = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                        var l_sgb = BoStp.OfGetSgbModel(sumShb.shb11, (Int16)sumShb.shb12);
                        if (l_sgb == null)
                        {
                            errMsg = "取得銷貨明細失敗!";
                            WfShowErrorMsg(errMsg);
                            return false;
                        }
                        if (l_sgb.sgb10 < (otherDocTot + sumShb.shb10))
                        {
                            errMsg = string.Format("銷貨單號{0} 項次{1} 單據金額{2}+已退金額{3}={4}已大於出貨金額{5},是否繼續?",
                                        sumShb.shb11,
                                        sumShb.shb12,
                                        GlobalFn.FormatDigit(sumShb.shb10).ToString(),
                                        GlobalFn.FormatDigit(otherDocTot).ToString(),
                                        GlobalFn.FormatDigit((sumShb.shb10 + otherDocTot)).ToString(),
                                        GlobalFn.FormatDigit(l_sgb.sgb10).ToString());
                            var dialogResult = WfShowConfirmMsg(errMsg);
                            //if (WfShowConfirmMsg(errMsg) != 1)
                            if (dialogResult != DialogResult.Yes)
                            {
                                return false;
                            }
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

        #region WfCancelConfirm 取消確認
        private void WfCancelConfirm()
        {
            sha_tb shaModel = null;
            shb_tb shbModel = null;
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
                shaModel = DrMaster.ToItem<sha_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                if (WfUpdSgb17(false) == false)
                {
                    WfRollback();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    shbModel = dr.ToItem<shb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("2", shbModel.shb03, shbModel.shb16, shbModel.shb18, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }

                    //更新庫存交易歷史檔
                    if (BoInv.OfDelIna(shbModel.shb01, shbModel.shb02, "1", out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                }

                DrMaster["shaconf"] = "N";
                DrMaster["shacond"] = DBNull.Value;
                DrMaster["shaconu"] = "";
                DrMaster["shamodu"] = LoginInfo.UserNo;
                DrMaster["shamodg"] = LoginInfo.DeptNo;
                DrMaster["shamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                shaModel = DrMaster.ToItem<sha_tb>();
                WfSetDocPicture("", shaModel.shaconf, shaModel.shastat, pbxDoc);
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
            vw_stpt500 masterModel = null;
            List<vw_stpt500s> detailList = null;
            decimal icc05 = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt500>();
                if (masterModel.shaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.sha02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                //檢查庫存的數量是否足夠做銷退出庫
                detailList = TabDetailList[0].DtSource.ToList<vw_stpt500s>();
                var listSumShb =                         //依銷貨單的料號及倉庫做加總
                        from sgb in detailList
                        where sgb.shb18 > 0
                        group sgb by new { sgb.shb03, sgb.shb16 } into shb_sum
                        select new
                        {
                            shb03 = shb_sum.Key.shb03,
                            shb16 = shb_sum.Key.shb16,
                            shb18 = shb_sum.Sum(p => p.shb18)
                        }
                    ;
                foreach (var sumShb in listSumShb)
                {
                    icc05 = BoInv.OfGetIcc05(sumShb.shb03, sumShb.shb16);
                    if (icc05 < sumShb.shb18)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", sumShb.shb03, icc05));
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
            vw_stpt500 masterModel = null;
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
                masterModel = DrMaster.ToItem<vw_stpt500>();

                if (masterModel.shaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.shaconf == "N")//走作廢
                {

                    DrMaster["shaconf"] = "X";
                    DrMaster["shaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.shaconf == "X")//走還原
                {
                    //檢查銷貨單是否為有效資料
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT COUNT(1)");
                    sbSql.AppendLine("FROM sga_tb");
                    sbSql.AppendLine("WHERE exists ");
                    sbSql.AppendLine("      (SELECT 1 FROM shb_tb WHERE shb01=@shb01");
                    sbSql.AppendLine("      AND shb11=sga01)");
                    sbSql.AppendLine("AND ISNULL(sgaconf,'')<>'Y'");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@shb01", GlobalFn.isNullRet(DrMaster["sha01"], "")));
                    iChkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                    if (iChkCnts > 0)
                    {
                        WfShowErrorMsg("銷貨單含有未確認資料!不可作廢還原!");
                        WfRollback();
                        return;
                    }
                    DrMaster["shaconf"] = "N";
                    DrMaster["shaconu"] = "";
                }
                DrMaster["shamodu"] = LoginInfo.UserNo;
                DrMaster["shamodg"] = LoginInfo.DeptNo;
                DrMaster["shamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_stpt500>();
                WfSetDocPicture("", masterModel.shaconf, masterModel.shastat, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfUpdSgb17 更新銷貨單已轉銷退數量 確認/取消確認
        private bool WfUpdSgb17(bool pbConfirm)
        {
            bab_tb babModel = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            DataTable dtShbDistinct = null;
            string sgb01;
            decimal sgb02;
            decimal docQty = 0, otherDocQty = 0;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sha01"], ""));
                if (babModel == null)
                {
                    WfShowErrorMsg("Get bab_tb Error!");
                    return false;
                }
                if (babModel.bab08 == "Y")   //更新銷貨單出庫數量
                {
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT DISTINCT shb11,shb12,SUM(shb14) shb14");
                    sbSql.AppendLine("FROM shb_tb");
                    sbSql.AppendLine("WHERE shb01=@shb01");
                    sbSql.AppendLine("  AND ISNULL(shb11,'')<>''");
                    sbSql.AppendLine("GROUP BY shb11,shb12");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@shb01", GlobalFn.isNullRet(DrMaster["sha01"], "")));
                    dtShbDistinct = BoStp.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                    if (dtShbDistinct == null)
                        return true;
                    foreach (DataRow dr in dtShbDistinct.Rows)
                    {
                        sgb01 = dr["shb11"].ToString();
                        sgb02 = Convert.ToDecimal(dr["shb12"]);
                        docQty = Convert.ToDecimal(dr["shb14"]);

                        sbSql = new StringBuilder();
                        sbSql.AppendLine("SELECT SUM(shb14) FROM sha_tb");
                        sbSql.AppendLine("  INNER JOIN shb_tb ON sha01=shb01");
                        sbSql.AppendLine("WHERE shaconf='Y'");
                        sbSql.AppendLine("AND shb11=@shb11 AND shb12=@shb12");
                        sbSql.AppendLine("AND sha01<>@sha01");

                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@shb11", sgb01));
                        sqlParmList.Add(new SqlParameter("@shb12", sgb02));
                        sqlParmList.Add(new SqlParameter("@sha01", GlobalFn.isNullRet(DrMaster["sha01"], "")));
                        otherDocQty = 0;
                        otherDocQty = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);

                        sbSql = new StringBuilder();
                        sbSql = sbSql.AppendLine("UPDATE sgb_tb");
                        sbSql = sbSql.AppendLine("SET sgb17=@sgb17");
                        sbSql = sbSql.AppendLine("WHERE sgb01=@sgb01");
                        sbSql = sbSql.AppendLine("AND sgb02=@sgb02");
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@sgb01", sgb01));
                        sqlParmList.Add(new SqlParameter("@sgb02", sgb02));
                        if (pbConfirm)  //確認的要加本身單據
                            sqlParmList.Add(new SqlParameter("@sgb17", docQty + otherDocQty));
                        else
                            sqlParmList.Add(new SqlParameter("@sgb17", otherDocQty));

                        if (BoStp.OfExecuteNonquery(sbSql.ToString(), sqlParmList.ToArray()) != 1)
                        {
                            WfShowErrorMsg("更新銷貨單銷退數量失敗!");
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

        #region WfSetDetailAmt 處理小計
        private bool WfSetDetailAmt(DataRow drShb)
        {
            sha_tb shaModel;
            shb_tb shbModel;
            decimal shb10t = 0, shb10 = 0;
            try
            {
                shaModel = DrMaster.ToItem<sha_tb>();
                shbModel = drShb.ToItem<shb_tb>();

                if (shbModel.shb17 == "2")//如為折讓,數量會為0 ,因此先以1帶入
                    shbModel.shb05 = 1;

                if (shaModel.sha08 == "Y")//稅內含
                {
                    shb10t = shbModel.shb09 * shbModel.shb05;
                    shb10t = GlobalFn.Round(shb10t, BekModel.bek04);
                    shb10 = shb10t / (1 + (shaModel.sha07 / 100));
                    shb10 = GlobalFn.Round(shb10, BekModel.bek04);
                }
                else//稅外加
                {
                    shb10 = shbModel.shb09 * shbModel.shb05;
                    shb10 = GlobalFn.Round(shb10, BekModel.bek04);
                    shb10t = shb10 * (1 + (shaModel.sha07 / 100));
                    shb10t = GlobalFn.Round(shb10t, BekModel.bek04);
                }
                drShb["shb10"] = shb10;
                drShb["shb10t"] = shb10t;

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
            sha_tb shaModel;
            decimal sha13 = 0, sha13t = 0, sha13g;
            try
            {
                shaModel = DrMaster.ToItem<sha_tb>();
                if (shaModel.sha08 == "Y")//稅內含,先處理含稅金額
                {
                    foreach (shb_tb l_shb in TabDetailList[0].DtSource.ToList<shb_tb>())
                    {
                        sha13t += l_shb.shb10t;
                    }
                    sha13t = GlobalFn.Round(sha13t, BekModel.bek04);
                    sha13 = sha13t / (1 + shaModel.sha07 / 100);
                    sha13 = GlobalFn.Round(sha13, BekModel.bek04);
                    sha13g = sha13t - sha13;

                }
                else//稅外加
                {
                    foreach (shb_tb l_shb in TabDetailList[0].DtSource.ToList<shb_tb>())
                    {
                        sha13 += l_shb.shb10;
                    }
                    sha13 = GlobalFn.Round(sha13, BekModel.bek04);
                    sha13g = sha13 * (shaModel.sha07 / 100);
                    sha13g = GlobalFn.Round(sha13g, BekModel.bek04);
                    sha13t = sha13 + sha13g;
                }

                DrMaster["sha13"] = sha13;
                DrMaster["sha13t"] = sha13t;
                DrMaster["sha13g"] = sha13g;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfResetAmtBySha06 依稅別更新單身及單頭的金額
        private void WfResetAmtBySha06()
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
    }
}
