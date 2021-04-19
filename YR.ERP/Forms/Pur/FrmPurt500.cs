/* 程式名稱: 退貨單維護作業
   系統代號: purt500
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
    public partial class FrmPurt500 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        PurBLL BoPur = null;
        InvBLL BoInv = null;

        baa_tb BaaTbModel = null;
        bek_tb BekTbModel = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmPurt500()
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
            this.StrFormID = "purt500";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("pha01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "phasecu";
                TabMaster.GroupColumn = "phasecg";

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
                WfSetUcomboxDataSource(ucb_pha06, sourceList);

                //發票聯數
                sourceList = BoPur.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_pha09, sourceList);

                //單據確認
                sourceList = BoPur.OfGetPhaconfKVPList();
                WfSetUcomboxDataSource(ucb_phaconf, sourceList);
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

            this.TabDetailList[0].TargetTable = "phb_tb";
            this.TabDetailList[0].ViewTable = "vw_purt500s";
            keyParm = new SqlParameter("phb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "pha01";
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
                        ugc = uGrid.DisplayLayout.Bands[0].Columns["phb17"];//退回類型
                        WfSetGridValueList(ugc, BoPur.OfGetPhb17KVPList());
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
            vw_purt500 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_purt500>();
                    WfSetDocPicture("", masterModel.phaconf, "", pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.pha10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekTbModel = BoBas.OfGetBekModel(masterModel.pha10);
                        if (BekTbModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.pha10));
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

                    WfSetControlReadonly(new List<Control> { ute_phacreu, ute_phacreg, udt_phacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_phamodu, ute_phamodg, udt_phamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_phasecu, ute_phasecg }, true);

                    if (GlobalFn.isNullRet(masterModel.pha01, "") != "")
                    {
                        WfSetControlReadonly(ute_pha01, true);
                        WfSetPha01RelReadonly(GlobalFn.isNullRet(masterModel.pha01, ""));
                    }
                    else
                        WfSetControlReadonly(ute_pha11, true);

                    if (!GlobalFn.varIsNull(masterModel.pha11)) //客戶狀態處理
                        WfSetControlReadonly(ute_pha03, true);

                    WfSetControlReadonly(new List<Control> { ute_pha01_c, ute_pha03_c, ute_pha04_c, ute_pha05_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_pha07, ucx_pha08 }, true);
                    WfSetControlReadonly(new List<Control> { ute_pha13, ute_pha13t, ute_pha13g }, true);
                    WfSetControlReadonly(new List<Control> { ucb_phaconf, udt_phacond, ute_phaconu }, true);
                    if (FormEditMode == YREditType.修改)
                    {
                        if (GlobalFn.varIsNull(masterModel.pha11))
                            WfSetControlReadonly(ute_pha03, false);
                        else
                            WfSetControlReadonly(ute_pha03, true);
                    }

                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_pha01, true);
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
            vw_purt500s detailModel;
            try
            {
                switch (pCurTabDetail)
                {
                    case 0:
                        detailModel = pDr.ToItem<vw_purt500s>();
                        foreach (UltraGridCell ugc in pUgr.Cells)
                        {
                            columnName = ugc.Column.Key.ToLower();
                            //先控可以輸入的
                            if (
                                columnName == "phb06" ||
                                columnName == "phb09" ||
                                columnName == "phb16" ||
                                columnName == "phb17"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "phb02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }

                            if (columnName == "phb03" ||
                                columnName == "phb11" ||
                                columnName == "phb12"
                                )
                            {
                                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["pha01"], ""));
                                if (babModel == null)
                                {
                                    WfShowErrorMsg("請先輸入退貨單,單頭資料!");
                                }
                                if (GlobalFn.isNullRet(babModel.bab08, "") == "Y")    //有來源單據
                                {
                                    if (columnName == "phb03")//料號
                                        WfSetControlReadonly(ugc, true);    //不可輸入
                                    else
                                        WfSetControlReadonly(ugc, false);
                                }
                                else
                                {
                                    if (columnName == "phb03")//料號
                                        WfSetControlReadonly(ugc, false);    //可輸入
                                    else
                                        WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }

                            if (columnName == "phb05"   //退貨數量
                                )
                            {
                                if (detailModel.phb17 == "1")//退貨
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
                #region 單頭-pick vw_purt500
                if (pDr.Table.Prefix.ToLower() == "vw_purt500")
                {
                    switch (pColName.ToLower())
                    {
                        case "pha01"://銷貨單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "pur"));
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
                        case "pha03"://客戶編號
                            WfShowPickUtility("p_pca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pca01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "pha04"://業務人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }

                            break;
                        case "pha05"://業務部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "pha10"://幣別
                            WfShowPickUtility("p_bek1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bek01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                            
                        //case "pha10"://出貨單號
                        //    WfShowPickUtility("p_pgb1", messageModel);
                        //    if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        //    {
                        //        if (messageModel.DataRowList.Count > 0)
                        //            pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pgb01"], "");
                        //        else
                        //            pDr[pColName] = "";
                        //    }
                        //    break;
                    }
                }
                #endregion

                #region 單身-pick vw_purt500s
                if (pDr.Table.Prefix.ToLower() == "vw_purt500s")
                {
                    switch (pColName.ToLower())
                    {
                        case "phb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "phb06"://退貨單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "phb11"://退貨單號
                            WfShowPickUtility("p_pgb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                {
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pgb01"], "");
                                    pDr["phb12"] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pgb02"], "");
                                }
                                else
                                {
                                    pDr[pColName] = "";
                                    pDr["phb12"] = "";
                                }
                            }
                            break;
                            
                        case "phb16"://倉庫
                            WfShowPickUtility("p_icb1", messageModel);
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
                pDr["Pha02"] = Today;
                pDr["Pha04"] = LoginInfo.UserNo;
                pDr["Pha04_c"] = LoginInfo.UserName;
                pDr["Pha05"] = LoginInfo.DeptNo;
                pDr["Pha05_c"] = LoginInfo.DeptName;
                pDr["Pha07"] = 0;
                pDr["Pha08"] = "N";
                pDr["Pha10"] = BaaTbModel.baa04;
                pDr["Pha13"] = 0;
                pDr["Pha13t"] = 0;
                pDr["Pha13g"] = 0;
                pDr["Pha14"] = 1;
                pDr["Phaconf"] = "N";
                pDr["phacomp"] = LoginInfo.CompNo;
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
                        pDr["Phb02"] = WfGetMaxSeq(pDr.Table, "phb02");
                        pDr["Phb05"] = 0;
                        pDr["Phb08"] = 0;
                        pDr["Phb09"] = 0;
                        pDr["Phb10"] = 0;
                        pDr["Phb10t"] = 0;
                        pDr["Phb11"] = DrMaster["pha11"];
                        pDr["Phb13"] = 0;
                        pDr["Phb14"] = 0;
                        pDr["Phb17"] = "1"; //退貨
                        pDr["Phb18"] = 0;
                        pDr["phbcomp"] = LoginInfo.CompNo;
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
            vw_purt500 masterModel;
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_purt500>();
                if (masterModel.phaconf != "N")
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
            vw_purt500 masterModel = null;
            vw_purt500s detailModel = null;
            List<vw_purt500s> detailList = null;
            bab_tb babModel = null;
            pga_tb pgaModel = null;
            UltraGrid uGrid = null;
            int ChkCnts = 0;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt500>();
                if (e.Column.ToLower() != "pha01" && GlobalFn.isNullRet(DrMaster["pha01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false;
                }
                #region 單頭-pick vw_purt500
                if (e.Row.Table.Prefix.ToLower() == "vw_purt500")
                {
                    switch (e.Column.ToLower())
                    {
                        case "pha01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "pur", "40") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["pha01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            WfSetPha01RelReadonly(GlobalFn.isNullRet(e.Value, ""));
                            if (ute_pha11.ReadOnly != true)
                                WfItemChkForceFocus(ute_pha11);
                            break;
                        case "pha02":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            var result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(e.Value), BaaModel);
                            if (result.Success == false)
                            {
                                WfShowErrorMsg(result.Message);
                                return false;
                            }
                            break;
                        case "pha03"://廠商
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pha03_c"] = "";
                                return true;
                            }
                            if (BoPur.OfChkPcaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此廠商資料,請檢核!");
                                return false;
                            }
                            WfSetPha03Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByPha06();
                            break;

                        case "pha04"://業務人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pha04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["pha04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pha05"://業務部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pha05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["pha05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pha06"://課稅別
                            WfSetPha06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByPha06();
                            break;

                        case "pha10"://幣別
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

                        case "pha11"://退貨單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfSetControlReadonly(new List<Control> { ute_pha03 }, false);
                                return true;
                            }
                            pgaModel = BoPur.OfGetPgaModel(GlobalFn.isNullRet(e.Value, ""));
                            if (pgaModel == null)
                            {
                                WfShowErrorMsg("無此入庫單!");
                                return false;
                            }
                            if (pgaModel.pgaconf != "Y")
                            {
                                WfShowErrorMsg("出貨單非確認狀態!");
                                return false;
                            }
                            //檢查與單身是否一致!
                            detailList = TabDetailList[0].DtSource.ToList<vw_purt500s>();
                            ChkCnts = detailList.Where(p => p.phb11 != pgaModel.pga01 && GlobalFn.isNullRet(p.phb11, "") != "")
                                               .Count();
                            if (ChkCnts > 0)
                            {
                                WfShowErrorMsg("單頭與單身的入庫單單號不一致!");
                                return false;
                            }
                            //這裡會有客戶跟銷退單單搶壓資料的問題:先以銷退單單為主
                            DrMaster["pha03"] = pgaModel.pga03;
                            DrMaster["pha03_c"] = BoPur.OfGetPca02(pgaModel.pga03);
                            WfSetControlReadonly(new List<Control> { ute_pha03 }, true);
                            DrMaster["pha06"] = pgaModel.pga06;
                            DrMaster["pha07"] = pgaModel.pga07;
                            DrMaster["pha08"] = pgaModel.pga08;
                            DrMaster["pha09"] = pgaModel.pga09;
                            DrMaster["pha10"] = pgaModel.pga10;
                            break;                            

                        case "pha14": //匯率
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
                
                #region 單身-pick vw_purt500s
                if (e.Row.Table.Prefix.ToLower() == "vw_purt500s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_purt500s>();
                    detailList = e.Row.Table.ToList<vw_purt500s>();
                    babModel = BoBas.OfGetBabModel(masterModel.pha01);

                    switch (e.Column.ToLower())
                    {
                        case "phb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            detailList = e.Row.Table.ToList<vw_purt500s>();
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.phb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;
                            
                        case "phb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["phb04"] = "";//品名
                                e.Row["phb05"] = 0;//退貨數量
                                e.Row["phb06"] = "";//退貨單位
                                e.Row["phb07"] = "";//庫存單位
                                e.Row["phb08"] = 0;//庫存轉換率
                                e.Row["phb13"] = 0;//入庫單轉換率
                                e.Row["phb14"] = 0;//入庫數量
                                e.Row["phb15"] = "";//入庫單單位
                                e.Row["phb18"] = 0;//庫存數量
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }
                            
                            if (WfSetPhb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "phb05"://退貨數量
                            detailModel.phb05 = BoBas.OfGetUnitRoundQty(detailModel.phb06, detailModel.phb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detailModel.phb05;
                            if (WfChkPhb05(e.Row, detailModel) == false)
                                return false;
                            e.Row["phb14"] = BoBas.OfGetUnitRoundQty(detailModel.phb06, detailModel.phb05 * detailModel.phb13); //轉換cfb單數量(並四拾伍入)
                            e.Row["phb18"] = BoBas.OfGetUnitRoundQty(detailModel.phb07, detailModel.phb05 * detailModel.phb08); //轉換庫存數量(並四拾伍入)
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "phb06"://銷貨單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (GlobalFn.varIsNull(detailModel.phb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["phb03"]);
                                return false;
                            }
                            if (WfChkPhb06(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;

                            if (WfSetPhb06Relation(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;
                            break;

                        case "phb09":   //單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.phb09 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.phb09, BekTbModel.bek03);
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                            
                        case "phb11"://銷貨單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("銷退單單號不可為空白!");
                                return false;
                            }

                            if (WfChkPhb11(e.Value.ToString()) == false)
                                return false;

                            if (!GlobalFn.varIsNull(detailModel.phb12))
                            {
                                if (WfChkPhb12(GlobalFn.isNullRet(detailModel.phb11, ""), GlobalFn.isNullRet(detailModel.phb12, 0)) == false)
                                    return false;

                                if (WfSetPhb12Relation(e.Row, GlobalFn.isNullRet(detailModel.phb11, ""), GlobalFn.isNullRet(detailModel.phb12, 0)) == false)
                                    return false;
                            }
                            WfSetTotalAmt();
                            break;

                        case "phb12"://銷貨項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("銷貨項次不可為空白!");
                                return false;
                            }

                            if (WfChkPhb12(GlobalFn.isNullRet(detailModel.phb11, ""), GlobalFn.isNullRet(detailModel.phb12, 0)) == false)
                                return false;
                            WfSetPhb12Relation(e.Row, GlobalFn.isNullRet(detailModel.phb11, ""), GlobalFn.isNullRet(detailModel.phb12, 0));
                            WfSetTotalAmt();
                            break;

                        case "phb16"://倉庫
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

                        case "phb17"://退回類型
                            if (detailModel.phb17 == "2")//折讓時
                            {
                                e.Row["phb05"] = 0;//數量
                                e.Row["phb14"] = 0;//轉換入庫數量
                                e.Row["phb18"] = 0;//轉換庫存數量
                            }
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            WfSetDetailDisplayMode(0, TabDetailList[0].UGrid.ActiveRow, e.Row);
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
                    case "vw_purt500s":
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
            vw_purt500 masterModel = null;
            vw_purt500s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt500>();
                if (!GlobalFn.varIsNull(masterModel.pha01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.pha01, ""));
                #region 單頭資料檢查
                chkColName = "pha01";       //退回單號
                chkControl = ute_pha01;
                if (GlobalFn.varIsNull(masterModel.pha01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pha02";       //退庫日期
                chkControl = udt_pha02;
                if (GlobalFn.varIsNull(masterModel.pha02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.pha02), BaaModel);
                if (result.Success == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                chkColName = "pha03";       //客戶編號
                chkControl = ute_pha03;
                if (GlobalFn.varIsNull(masterModel.pha03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }


                chkColName = "pha04";       //業務人員
                chkControl = ute_pha04;
                if (GlobalFn.varIsNull(masterModel.pha04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                
                chkColName = "pha05";       //業務部門
                chkControl = ute_pha05;
                if (GlobalFn.varIsNull(masterModel.pha05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                
                chkColName = "pha06";       //課稅別
                chkControl = ucb_pha06;
                if (GlobalFn.varIsNull(masterModel.pha06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pha10";       //幣別
                chkControl = ute_pha10;
                if (GlobalFn.varIsNull(masterModel.pha10))
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

                    detailModel = drTemp.ToItem<vw_purt500s>();
                    chkColName = "phb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.phb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "phb11";   //入庫單號
                    if (GlobalFn.varIsNull(detailModel.phb11) && babModel.bab08 == "Y")  //有來源單據銷貨單單號要輸入
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "phb12";   //入庫單項次
                    if (GlobalFn.varIsNull(detailModel.phb12) && babModel.bab08 == "Y")  //有來源單據銷貨單單號要輸入
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "phb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.phb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }                    
                    
                    chkColName = "phb05";   //銷退數量
                    #region phb05 銷退數量
                    if (detailModel.phb17 == "1")//退回
                    {
                        if (GlobalFn.varIsNull(detailModel.phb05) || detailModel.phb05 <= 0)
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
                        if (GlobalFn.varIsNull(detailModel.phb05) || detailModel.phb05 != 0)
                        {
                            this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                            msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                            msg += "應為0!";
                            WfShowErrorMsg(msg);
                            WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                            return false;
                        }
                    }

                    if (WfChkPhb05(drTemp, detailModel) == false)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    chkColName = "phb06";   //退貨單位
                    if (GlobalFn.varIsNull(detailModel.phb06))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "phb16";   //倉庫別
                    if (GlobalFn.varIsNull(detailModel.phb16))
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
            string pha01New, errMsg;
            vw_purt500 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt500>();
                if (FormEditMode == YREditType.新增)
                {
                    pha01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.pha01, ModuleType.pur, (DateTime)masterModel.pha02, out pha01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["pha01"] = pha01New;
                }
                
                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["phasecu"] = LoginInfo.UserNo;
                        DrMaster["phasecg"] = LoginInfo.GroupNo;
                        DrMaster["phacreu"] = LoginInfo.UserNo;
                        DrMaster["phacreg"] = LoginInfo.DeptNo;
                        DrMaster["phacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["phamodu"] = LoginInfo.UserNo;
                        DrMaster["phamodg"] = LoginInfo.DeptNo;
                        DrMaster["phamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["phbcreu"] = LoginInfo.UserNo;
                            drDetail["phbcreg"] = LoginInfo.DeptNo;
                            drDetail["phbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["phbmodu"] = LoginInfo.UserNo;
                            drDetail["phbmodg"] = LoginInfo.DeptNo;
                            drDetail["phbmodd"] = Now;
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
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        
        //*****************************表單自opbFuction****************************************
        #region WfSetPha01RelReadonly 依單別設定相關控制項 readonly
        private void WfSetPha01RelReadonly(string pPha01)
        {
            bab_tb babModel = null;
            string bab01;
            try
            {
                WfSetControlReadonly(new List<Control> { ute_pha01 }, true);
                bab01 = pPha01.Substring(0, GlobalFn.isNullRet(BaaTbModel.baa06, 0));
                babModel = BoBas.OfGetBabModel(bab01);
                if (babModel.bab08 == "Y")   //有來源單據
                {
                    WfSetControlReadonly(new List<Control> { ute_pha11 }, false);
                }
                else
                {
                    WfSetControlReadonly(new List<Control> { ute_pha11 }, true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPha03Relation 設定客戶相關聯
        private void WfSetPha03Relation(string pPha03)
        {
            pca_tb pcaModel;
            bab_tb babModel = null;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["pha01"], ""));
                pcaModel = BoPur.OfGetPcaModel(pPha03);
                if (pcaModel == null)
                    return;

                DrMaster["pha03_c"] = pcaModel.pca03;
                DrMaster["pha06"] = pcaModel.pca22;    //課稅別
                WfSetPha06Relation(pcaModel.pca22);
                DrMaster["pha09"] = pcaModel.pca23;    //發票聯數

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPha06Relation 設定稅別關聯
        private void WfSetPha06Relation(string pPha06)
        {
            try
            {
                if (pPha06 == "1")
                {
                    DrMaster["pha07"] = BaaTbModel.baa05;
                    DrMaster["pha08"] = "Y";
                }
                else if (pPha06 == "2")
                {
                    DrMaster["pha07"] = BaaTbModel.baa05;
                    DrMaster["pha08"] = "N";
                }
                else
                {
                    DrMaster["pha07"] = 0;
                    DrMaster["pha08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPhb03Relation 設定料號關聯
        private bool WfSetPhb03Relation(string pPhb03, DataRow pDr)
        {
            ica_tb icaModel;
            decimal phb08;   //轉換率
            try
            {
                icaModel = BoInv.OfGetIcaModel(pPhb03);
                phb08 = 0;
                if (icaModel == null)
                {
                    pDr["phb04"] = "";//品名
                    pDr["phb05"] = 0;//退貨數量
                    pDr["phb06"] = "";//退貨單位
                    pDr["phb07"] = "";//庫存單位
                    pDr["phb08"] = 0;//庫存轉換率
                    pDr["phb18"] = 0;//庫存數量
                }
                else
                {
                    if (BoInv.OfGetUnitCovert(pPhb03, icaModel.ica08, icaModel.ica07, out phb08) == false)
                    {
                        WfShowErrorMsg("未設定料號轉換,請先設定!");
                        return false;
                    }
                    pDr["phb04"] = icaModel.ica02;//品名
                    pDr["phb05"] = 0;//退貨數量
                    pDr["phb06"] = icaModel.ica08;//退貨單位帶採購單位
                    pDr["phb07"] = icaModel.ica07;//庫存單位
                    pDr["phb08"] = phb08;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPhb06Relation 設定退貨單位關聯
        //檢查前要先確認料號是否已輸入
        private bool WfSetPhb06Relation(DataRow pDr, string pPhb06, string pBab08)
        {
            vw_purt500s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_purt500s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pPhb06, "")) == false)
                {
                    WfShowErrorMsg("無此退貨單位!請確認");
                    return false;
                }
                //取得是否有銷退對庫存的轉換率
                dConvert = 0;
                if (BoInv.OfGetUnitCovert(detailModel.phb03, pPhb06, detailModel.phb07, out dConvert) == true)
                {
                    pDr["phb08"] = dConvert;
                    pDr["phb18"] = BoBas.OfGetUnitRoundQty(detailModel.phb07, detailModel.phb05 * dConvert); //轉換庫存數量(並四拾伍入)
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有銷退對銷貨單的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.phb03, pPhb06, detailModel.phb15, out dConvert) == true)
                    {
                        pDr["phb13"] = dConvert;
                        pDr["phb14"] = BoBas.OfGetUnitRoundQty(detailModel.phb15, detailModel.phb05 * dConvert); //轉換庫存數量(並四拾伍入)
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
        private bool WfSetPhb12Relation(DataRow pdr, string pPhb11, int pPhb12)
        {
            pgb_tb pgbModel;
            decimal pgb18;
            try
            {
                pgbModel = BoPur.OfGetPgbModel(pPhb11, pPhb12);
                if (pgbModel == null)
                    return false;
                pgb18 = BoBas.OfGetUnitRoundQty(pgbModel.pgb07, pgbModel.pgb05 * pgbModel.pgb08); //轉換庫存數量(並四拾伍入) 先取,避免錯誤

                pdr["phb03"] = pgbModel.pgb03;//料號
                pdr["phb04"] = pgbModel.pgb04;//品名
                pdr["phb05"] = pgbModel.pgb05;//銷售數量
                pdr["phb06"] = pgbModel.pgb06;//原入庫單位
                pdr["phb07"] = pgbModel.pgb07;//庫存單位
                pdr["phb08"] = pgbModel.pgb08;//庫存轉換率
                pdr["phb09"] = pgbModel.pgb09;//單價
                pdr["phb10"] = pgbModel.pgb10;//未稅金額
                pdr["phb10t"] = pgbModel.pgb10t;//含稅金額

                pdr["phb13"] = 1;          //入庫退回對入庫單轉換率
                pdr["phb14"] = pgbModel.pgb05;//轉換入庫數量
                pdr["phb15"] = pgbModel.pgb06;//原入庫單單位
                pdr["phb16"] = pgbModel.pgb16;//倉庫別

                pdr["phb18"] = pgb18; //轉換庫存數量(並四拾伍入)
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkPhb05 數量檢查
        private bool WfChkPhb05(DataRow pdr, vw_purt500s pDetailModel)
        {
            List<vw_purt500s> detailList = null;
            pgb_tb pgbModel = null;
            bab_tb babModel = null;
            string errMsg;
            decimal docThisQty = 0;         //本項次的轉換入庫單數量
            decimal docOtherQtyTotal = 0;   //本單據其他項次的入庫單數量加總
            decimal otherQtyTotal = 0;      //其他單據的數量加總
            StringBuilder sbSql;
            List<SqlParameter> sqlParms;
            try
            {
                detailList = pdr.Table.ToList<vw_purt500s>();
                if (GlobalFn.varIsNull(pDetailModel.phb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pDetailModel.phb03))
                {
                    errMsg = "請先輸入料號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pDetailModel.phb06))
                {
                    errMsg = "請先輸入退貨單位!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["pha01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (babModel.bab08 == "Y" && pDetailModel.phb17 == "1") //退貨性質時,檢查退回數量是否超過入庫單數量!
                {
                    pgbModel = BoPur.OfGetPgbModel(pDetailModel.phb11, Convert.ToInt16(pDetailModel.phb12));
                    if (babModel == null)
                    {
                        errMsg = "Get sgb_tb Error!";
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    //取得本身單據中的所有數量轉換銷貨單位後的總和!
                    //先取本身這筆
                    docThisQty = BoBas.OfGetUnitRoundQty(pDetailModel.phb15, pDetailModel.phb05 * pDetailModel.phb13);
                    //再取其他筆加總
                    docOtherQtyTotal = detailList.Where(p => p.phb02 != pDetailModel.phb02)
                                       .Where(p => p.phb11 == pDetailModel.phb11 && p.phb12 == pDetailModel.phb12)
                                       .Sum(p => p.phb14);
                    //取得其他單據上的加總
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT SUM(phb14) FROM pha_tb");
                    sbSql.AppendLine("  INNER JOIN phb_tb ON pha01=phb01");
                    sbSql.AppendLine("WHERE phaconf<>'X'");
                    sbSql.AppendLine("  AND pha01 <> @pha01");
                    sbSql.AppendLine("  AND phb11 = @phb11 AND phb12 = @phb12");
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@pha01", GlobalFn.isNullRet(DrMaster["pha01"], "")));
                    sqlParms.Add(new SqlParameter("@phb11", pDetailModel.phb11));
                    sqlParms.Add(new SqlParameter("@phb12", pDetailModel.phb12));
                    otherQtyTotal = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParms.ToArray()), 0);
                    if (pgbModel.pgb05 < (docThisQty + docOtherQtyTotal + otherQtyTotal))
                    {
                        errMsg = string.Format("項次{0}退貨單最大可輸入數量為 {1}",
                                                pDetailModel.phb02.ToString(),
                                                (pgbModel.pgb05 - docOtherQtyTotal - otherQtyTotal).ToString()
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

        #region WfChkPhb06 檢查退貨單位
        //檢查前要先確認料號是否已輸入
        private bool WfChkPhb06(DataRow pDr, string pPhb06, string pBab08)
        {
            vw_purt500s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_purt500s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pPhb06, "")) == false)
                {
                    WfShowErrorMsg("無此銷退單位!請確認");
                    return false;
                }
                //檢查是否有銷售對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.phb03, pPhb06, detailModel.phb07, out dConvert) == false)
                {
                    WfShowErrorMsg("未設定銷退單位對庫存單位的轉換率,請先設定!");
                    return false;
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有銷退對銷貨單的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.phb03, pPhb06, detailModel.phb06, out dConvert) == false)
                    {
                        WfShowErrorMsg("未設定退貨單位對入庫單單位的轉換率,請先設定!");
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

        #region WfChkPhb11 入庫單檢查
        private bool WfChkPhb11(string pPhb11)
        {
            string errMsg;
            pga_tb pgaModel;
            string pha11;
            try
            {
                pgaModel = BoPur.OfGetPgaModel(pPhb11);
                if (pgaModel == null)
                {
                    errMsg = "查無此入庫單!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (pgaModel.pgaconf != "Y")
                {
                    errMsg = "入庫單非確認狀態!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                pha11 = GlobalFn.isNullRet(DrMaster["pha11"], "");
                if (pha11 != "" && pha11 != pPhb11)
                {
                    errMsg = "入庫單單號與單頭不同!";
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

        #region WfChkPhb12 退貨單項次檢查
        private bool WfChkPhb12(string pPhb11, int pPhb12)
        {
            string errMsg;
            pgb_tb pgbModel;
            try
            {
                pgbModel = BoPur.OfGetPgbModel(pPhb11, pPhb12);
                if (pgbModel == null)
                {
                    errMsg = "查無此入庫單項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if ((pgbModel.pgb05 - pgbModel.pgb17) <= 0)
                {
                    errMsg = string.Format("無可用入庫單數量!(可用入庫單數量為{0})", GlobalFn.isNullRet(pgbModel.pgb05 - pgbModel.pgb17, 0));
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
            pha_tb phaModel = null;
            phb_tb phbModel = null;

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

                phaModel = DrMaster.ToItem<pha_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                phaModel.phaconf = "Y";
                phaModel.phacond = Today;
                phaModel.phaconu = LoginInfo.UserNo;

                //更新銷貨單銷貨量
                if (WfUpdPgb17(true) == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    phbModel = dr.ToItem<phb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("2", phbModel.phb03, phbModel.phb16, phbModel.phb18, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                    //更新庫存交易歷史檔
                    if (BoInv.OfStockPost("purt500", phaModel, phbModel, LoginInfo, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                }

                DrMaster["phaconf"] = "Y";
                DrMaster["phacond"] = Today;
                DrMaster["phaconu"] = LoginInfo.UserNo;
                DrMaster["phamodu"] = LoginInfo.UserNo;
                DrMaster["phamodg"] = LoginInfo.DeptNo;
                DrMaster["phamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                phaModel = DrMaster.ToItem<pha_tb>();
                WfSetDocPicture("", phaModel.phaconf, "", pbxDoc);
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
            vw_purt500 masterModel = null;
            vw_purt500s detailModel = null;
            List<vw_purt500s> detailList = null;
            bab_tb l_bab = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList = null;
            string errMsg = "";
            decimal icc05 = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt500>();
                if (masterModel.phaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.pha02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_purt500s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_purt500s>();
                    //檢查數量
                    if (WfChkPhb05(drTemp, detailModel) == false)
                    {
                        return false;
                    }
                }
                //檢查銷退金額是否有大於來源單據
                l_bab = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["pha01"], ""));
                if (l_bab == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (l_bab.bab08 == "Y")
                {
                    var listSumPhbAmt = from o in detailList
                                        where o.phb10 > 0 && o.phb11 != "" && o.phb12 != null
                                        group o by new { o.phb11, o.phb12 } into phbSum
                                        select new
                                        {
                                            phb11 = phbSum.Key.phb11,
                                            phb12 = phbSum.Key.phb12,
                                            phb10 = phbSum.Sum(p => p.phb10)
                                        }
                                   ;
                    foreach (var sumPhb in listSumPhbAmt)
                    {
                        sbSql = new StringBuilder();
                        sbSql.AppendLine("SELECT SUM(phb10) FROM phb_tb");
                        sbSql.AppendLine("  INNER JOIN pha_tb ON phb01=pha01");
                        sbSql.AppendLine("WHERE phb01<>@phb01");
                        sbSql.AppendLine("AND phaconf='Y'");
                        sbSql.AppendLine("AND phb11=@phb11 AND phb12=@phb12");
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@phb01", GlobalFn.isNullRet(DrMaster["pha01"], "")));
                        sqlParmList.Add(new SqlParameter("@phb11", sumPhb.phb11));
                        sqlParmList.Add(new SqlParameter("@phb12", sumPhb.phb12));
                        var otherDocTot = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                        var l_pgb = BoPur.OfGetPgbModel(sumPhb.phb11, (Int16)sumPhb.phb12);
                        if (l_pgb == null)
                        {
                            errMsg = "取得入庫單明細失敗!";
                            WfShowErrorMsg(errMsg);
                            return false;
                        }
                        if (l_pgb.pgb10 < (otherDocTot + sumPhb.phb10))
                        {
                            errMsg = string.Format("入庫單號{0} 項次{1} 單據金額{2}+已退金額{3}={4}已大於入庫金額{5},是否繼續?",
                                        sumPhb.phb11,
                                        sumPhb.phb12,
                                        GlobalFn.FormatDigit(sumPhb.phb10).ToString(),
                                        GlobalFn.FormatDigit(otherDocTot).ToString(),
                                        GlobalFn.FormatDigit((sumPhb.phb10 + otherDocTot)).ToString(),
                                        GlobalFn.FormatDigit(l_pgb.pgb10).ToString());
                            var dialogResult = WfShowConfirmMsg(errMsg);
                            //if (WfShowConfirmMsg(errMsg) != 1)
                            if (dialogResult != DialogResult.Yes)
                            {
                                return false;
                            }
                        }
                    }

                }

                //檢查庫存的數量是否足夠做退貨出庫
                detailList = TabDetailList[0].DtSource.ToList<vw_purt500s>();
                var listSumPhb =                         //依退貨單的料號及倉庫做加總
                        from pgb in detailList
                        where pgb.phb18 > 0
                        group pgb by new { pgb.phb03, pgb.phb16 } into shb_sum
                        select new
                        {
                            phb03 = shb_sum.Key.phb03,
                            phb16 = shb_sum.Key.phb16,
                            phb18 = shb_sum.Sum(p => p.phb18)
                        }
                    ;
                foreach (var sumPhb in listSumPhb)
                {
                    icc05 = BoInv.OfGetIcc05(sumPhb.phb03, sumPhb.phb16);
                    if (icc05 < sumPhb.phb18)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", sumPhb.phb03, icc05));
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
            pha_tb phaModel = null;
            phb_tb phbModel = null;
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
                phaModel = DrMaster.ToItem<pha_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                if (WfUpdPgb17(false) == false)
                {
                    WfRollback();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    phbModel = dr.ToItem<phb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("1", phbModel.phb03, phbModel.phb16, phbModel.phb18, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }

                    //更新庫存交易歷史檔
                    if (BoInv.OfDelIna(phbModel.phb01, phbModel.phb02, "2", out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                }

                DrMaster["phaconf"] = "N";
                DrMaster["phacond"] = DBNull.Value;
                DrMaster["phaconu"] = "";
                DrMaster["phamodu"] = LoginInfo.UserNo;
                DrMaster["phamodg"] = LoginInfo.DeptNo;
                DrMaster["phamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                phaModel = DrMaster.ToItem<pha_tb>();
                WfSetDocPicture("", phaModel.phaconf, "", pbxDoc);
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
            vw_purt500 masterModel = null;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt500>();
                if (masterModel.phaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.pha02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
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
            vw_purt500 masterModel = null;
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
                masterModel = DrMaster.ToItem<vw_purt500>();

                if (masterModel.phaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.phaconf == "N")//走作廢
                {

                    DrMaster["phaconf"] = "X";
                    DrMaster["phaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.phaconf == "X")//走還原
                {
                    //檢查銷貨單是否為有效資料
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT COUNT(1)");
                    sbSql.AppendLine("FROM pga_tb");
                    sbSql.AppendLine("WHERE exists ");
                    sbSql.AppendLine("      (SELECT 1 FROM shb_tb WHERE phb01=@phb01");
                    sbSql.AppendLine("      AND phb11=pga01)");
                    sbSql.AppendLine("AND ISNULL(sgaconf,'')<>'Y'");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@phb01", GlobalFn.isNullRet(DrMaster["pha01"], "")));
                    iChkCnts = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                    if (iChkCnts > 0)
                    {
                        WfShowErrorMsg("入庫單含有未確認資料!不可作廢還原!");
                        WfRollback();
                        return;
                    }
                    DrMaster["phaconf"] = "N";
                    DrMaster["phaconu"] = "";
                }
                DrMaster["phamodu"] = LoginInfo.UserNo;
                DrMaster["phamodg"] = LoginInfo.DeptNo;
                DrMaster["phamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_purt500>();
                WfSetDocPicture("", masterModel.phaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfSetDetailAmt 處理小計
        private bool WfSetDetailAmt(DataRow drPhb)
        {
            pha_tb phaModel;
            phb_tb phbModel;
            decimal phb10t = 0, phb10 = 0;
            try
            {
                phaModel = DrMaster.ToItem<pha_tb>();
                phbModel = drPhb.ToItem<phb_tb>();

                if (phbModel.phb17 == "2")//如為折讓,數量會為0 ,因此先以1帶入
                    phbModel.phb05 = 1;

                if (phaModel.pha08 == "Y")//稅內含
                {
                    phb10t = phbModel.phb09 * phbModel.phb05;
                    phb10t = GlobalFn.Round(phb10t, BekTbModel.bek04);
                    phb10 = phb10t / (1 + (phaModel.pha07 / 100));
                    phb10 = GlobalFn.Round(phb10, BekTbModel.bek04);
                }
                else//稅外加
                {
                    phb10 = phbModel.phb09 * phbModel.phb05;
                    phb10 = GlobalFn.Round(phb10, BekTbModel.bek04);
                    phb10t = phb10 * (1 + (phaModel.pha07 / 100));
                    phb10t = GlobalFn.Round(phb10t, BekTbModel.bek04);
                }
                drPhb["phb10"] = phb10;
                drPhb["phb10t"] = phb10t;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfUpdPgb17 更新入庫單已轉入庫退回單數量 確認/取消確認
        private bool WfUpdPgb17(bool pbConfirm)
        {
            bab_tb babModel = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            DataTable dtPhbDistinct = null;
            string pgb01;
            decimal pgb02;
            decimal docQty = 0, otherDocQty = 0;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["pha01"], ""));
                if (babModel == null)
                {
                    WfShowErrorMsg("Get bab_tb Error!");
                    return false;
                }
                if (babModel.bab08 == "Y")   //更新銷貨單出庫數量
                {
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT DISTINCT phb11,phb12,SUM(phb14) phb14");
                    sbSql.AppendLine("FROM phb_tb");
                    sbSql.AppendLine("WHERE phb01=@phb01");
                    sbSql.AppendLine("  AND ISNULL(phb11,'')<>''");
                    sbSql.AppendLine("GROUP BY phb11,phb12");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@phb01", GlobalFn.isNullRet(DrMaster["pha01"], "")));
                    dtPhbDistinct = BoPur.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                    if (dtPhbDistinct == null)
                        return true;
                    foreach (DataRow dr in dtPhbDistinct.Rows)
                    {
                        pgb01 = dr["phb11"].ToString();
                        pgb02 = Convert.ToDecimal(dr["phb12"]);
                        docQty = Convert.ToDecimal(dr["phb14"]);

                        sbSql = new StringBuilder();
                        sbSql.AppendLine("SELECT SUM(phb14) FROM pha_tb");
                        sbSql.AppendLine("  INNER JOIN phb_tb ON pha01=phb01");
                        sbSql.AppendLine("WHERE phaconf='Y'");
                        sbSql.AppendLine("AND phb11=@phb11 AND phb12=@phb12");
                        sbSql.AppendLine("AND pha01<>@pha01");


                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@phb11", pgb01));
                        sqlParmList.Add(new SqlParameter("@phb12", pgb02));
                        sqlParmList.Add(new SqlParameter("@pha01", GlobalFn.isNullRet(DrMaster["pha01"], "")));
                        otherDocQty = 0;
                        otherDocQty = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);

                        sbSql = new StringBuilder();
                        sbSql = sbSql.AppendLine("UPDATE pgb_tb");
                        sbSql = sbSql.AppendLine("SET pgb17=@pgb17");
                        sbSql = sbSql.AppendLine("WHERE pgb01=@pgb01");
                        sbSql = sbSql.AppendLine("AND pgb02=@pgb02");
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@pgb01", pgb01));
                        sqlParmList.Add(new SqlParameter("@pgb02", pgb02));
                        if (pbConfirm)  //確認的要加本身單據
                            sqlParmList.Add(new SqlParameter("@pgb17", docQty + otherDocQty));
                        else
                            sqlParmList.Add(new SqlParameter("@pgb17", otherDocQty));

                        if (BoPur.OfExecuteNonquery(sbSql.ToString(), sqlParmList.ToArray()) != 1)
                        {
                            WfShowErrorMsg("更新入庫單退回數量失敗!");
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

        #region WfSetTotalAmt 處理總計
        private bool WfSetTotalAmt()
        {
            pha_tb phaModel;
            decimal pha13 = 0, pha13t = 0, pha13g;
            try
            {
                phaModel = DrMaster.ToItem<pha_tb>();
                if (phaModel.pha08 == "Y")//稅內含,先處理含稅金額
                {
                    foreach (phb_tb l_phb in TabDetailList[0].DtSource.ToList<phb_tb>())
                    {
                        pha13t += l_phb.phb10t;
                    }
                    pha13t = GlobalFn.Round(pha13t, BekTbModel.bek04);
                    pha13 = pha13t / (1 + phaModel.pha07 / 100);
                    pha13 = GlobalFn.Round(pha13, BekTbModel.bek04);
                    pha13g = pha13t - pha13;

                }
                else//稅外加
                {
                    foreach (phb_tb l_phb in TabDetailList[0].DtSource.ToList<phb_tb>())
                    {
                        pha13 += l_phb.phb10;
                    }
                    pha13 = GlobalFn.Round(pha13, BekTbModel.bek04);
                    pha13g = pha13 * (phaModel.pha07 / 100);
                    pha13g = GlobalFn.Round(pha13g, BekTbModel.bek04);
                    pha13t = pha13 + pha13g;
                }

                DrMaster["pha13"] = pha13;
                DrMaster["pha13t"] = pha13t;
                DrMaster["pha13g"] = pha13g;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfResetAmtByPha06 依稅別更新單身及單頭的金額
        private void WfResetAmtByPha06()
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
