/* 程式名稱: 出貨單維護作業
   系統代號: stpt400
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
    public partial class FrmStpt400 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        StpBLL BoStp = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;

        baa_tb BaaTbModel = null;
        bek_tb BekTbModel = null;      //幣別檔,在display mode載入

        sga_tb sgaOrg = null;//記錄進入修改前的原始資料
        #endregion

        #region 建構子
        public FrmStpt400()
        {
            InitializeComponent();
        }

        public FrmStpt400(string pSourceForm, YR.ERP.Shared.UserInfo pUserInfo, string pWhere)
        {
            InitializeComponent();
            StrQueryWhereAppend = pWhere;
            this.LoginInfo = pUserInfo;
            this.WindowState = FormWindowState.Maximized;
        }
        #endregion

        #region WfSetVar() : 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// <summary>
        /// 設定變數預設值, 繼承的 frm 要依需求覆寫
        /// </summary>
        /// <returns></returns>
        protected override Boolean WfSetVar()
        {
            this.StrFormID = "stpt400";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("sga01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "sgasecu";
                TabMaster.GroupColumn = "sgasecg";

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
                WfSetUcomboxDataSource(ucb_sga06, sourceList);

                //發票聯數
                sourceList = BoStp.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_sga09, sourceList);

                //單據確認
                sourceList = BoStp.OfGetSgaconfKVPList();
                WfSetUcomboxDataSource(ucb_sgaconf, sourceList);
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

            this.TabDetailList[0].TargetTable = "sgb_tb";
            this.TabDetailList[0].ViewTable = "vw_stpt400s";
            keyParm = new SqlParameter("sgb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "sga01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_stpt400 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_stpt400>();
                    WfSetDocPicture("", masterModel.sgaconf, masterModel.sgastat, pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.sga10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekTbModel = BoBas.OfGetBekModel(masterModel.sga10);
                        if (BekTbModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.sga10));
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

                    WfSetControlReadonly(new List<Control> { ute_sgacreu, ute_sgacreg, udt_sgacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_sgamodu, ute_sgamodg, udt_sgamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_sgasecu, ute_sgasecg }, true);

                    if (GlobalFn.isNullRet(masterModel.sga01, "") != "")
                    {
                        WfSetControlReadonly(ute_sga01, true);
                        WfSetSga01RelReadonly(GlobalFn.isNullRet(masterModel.sga01, ""));
                    }
                    else
                        WfSetControlReadonly(ute_sga19, true);

                    if (!GlobalFn.varIsNull(masterModel.sga19)) //客戶狀態處理
                        WfSetControlReadonly(ute_sga03, true);

                    WfSetControlReadonly(new List<Control> { ute_sga01_c, ute_sga03_c, ute_sga04_c, ute_sga05_c, ute_sga11_c, ute_sga12_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_sga07, ucx_sga08 }, true);
                    WfSetControlReadonly(new List<Control> { ute_sga13, ute_sga13t, ute_sga13g }, true);
                    WfSetControlReadonly(new List<Control> { ute_sga14_c, ute_sga15_c, ute_sga16_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_sga22 }, true);
                    WfSetControlReadonly(new List<Control> { ucb_sgaconf, udt_sgacond, ute_sgaconu, ute_sgastat }, true);
                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_sga01, true);
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
                                columnName == "sgb05" ||
                                columnName == "sgb06" ||
                                columnName == "sgb09" ||
                                columnName == "sgb16"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "sgb02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }

                            if (columnName == "sgb03" ||
                                columnName == "sgb11" ||
                                columnName == "sgb12"
                                )
                            {
                                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sga01"], ""));
                                if (babModel == null)
                                {
                                    WfShowErrorMsg("請先輸銷貨單單頭資料!");
                                }
                                if (GlobalFn.isNullRet(babModel.bab08, "") == "Y")    //有來源單據
                                {
                                    if (columnName == "sgb03")//料號
                                        WfSetControlReadonly(ugc, true);    //不可輸入
                                    else
                                        WfSetControlReadonly(ugc, false);
                                }
                                else
                                {
                                    if (columnName == "sgb03")//料號
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
            try
            {
                vw_stpt400 masterModel = null;
                vw_stpt400s detailModel = null;
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_stpt400
                if (pDr.Table.Prefix.ToLower() == "vw_stpt400")
                {
                    masterModel = DrMaster.ToItem<vw_stpt400>();
                    switch (pColName.ToLower())
                    {
                        case "sga01"://銷貨單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "stp"));
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
                        case "sga03"://客戶編號
                            WfShowPickUtility("p_sca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sca01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "sga04"://業務人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "sga05"://業務部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sga10"://幣別
                            WfShowPickUtility("p_bek1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bek01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sga11"://收款條件
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

                        case "sga12"://取價條件
                            WfShowPickUtility("p_sbb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sga14"://貨運方式
                            WfShowPickUtility("p_bel1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bel01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sga15"://貨運起點
                            WfShowPickUtility("p_sbg1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbg01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sga16"://貨運終點
                            WfShowPickUtility("p_sbg1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbg01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sga18"://送貨地址碼
                            if (GlobalFn.varIsNull(masterModel.sga03))
                                return false;
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.ParamSearchList.Add(new SqlParameter("@scb01", masterModel.sga03));
                            WfShowPickUtility("p_scb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["scb02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sga19"://訂單單號
                            if (!GlobalFn.varIsNull(masterModel.sga03))   //如果先輸入客編要帶入查詢
                            {
                                messageModel.StrWhereAppend = " AND sfa03=@sfa03";
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@sfa03", masterModel.sga03));
                            }

                            WfShowPickUtility("p_sfb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sfb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_stpt400s
                if (pDr.Table.Prefix.ToLower() == "vw_stpt400s")
                {
                    masterModel = DrMaster.ToItem<vw_stpt400>();
                    detailModel = pDr.ToItem<vw_stpt400s>();
                    switch (pColName.ToLower())
                    {
                        case "sgb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sgb06"://銷貨單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sgb11"://訂單單號
                            if (GlobalFn.isNullRet(masterModel.sga03, "") == "")
                                WfShowPickUtility("p_sfb1", messageModel);
                            else
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@sfa03", masterModel.sga03));
                                WfShowPickUtility("p_sfb2", messageModel);
                            }
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                {
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sfb01"], "");
                                    pDr["sgb12"] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sfb02"], "");
                                }
                                else
                                {
                                    pDr[pColName] = "";
                                    pDr["sgb12"] = "";

                                }
                            }
                            break;

                        case "sgb16"://倉庫
                            if (GlobalFn.isNullRet(detailModel.sgb03, "") == "")
                                WfShowPickUtility("p_icb1", messageModel);
                            else
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.sgb03));
                                WfShowPickUtility("p_icc1", messageModel);
                            }
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["icc02"], "");
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
                pDr["sga02"] = Today;
                pDr["sga04"] = LoginInfo.UserNo;
                pDr["sga04_c"] = LoginInfo.UserName;
                pDr["sga05"] = LoginInfo.DeptNo;
                pDr["sga05_c"] = LoginInfo.DeptName;
                pDr["sga07"] = 0;
                pDr["sga08"] = "N";
                pDr["sga10"] = BaaTbModel.baa04;
                pDr["sga13"] = 0;
                pDr["sga13t"] = 0;
                pDr["sga13g"] = 0;
                pDr["sga21"] = 1;       //匯率
                pDr["sga23"] = 0;       //出貨成本
                pDr["sga25"] = Today;   //訂單日期
                pDr["sgaconf"] = "N";
                pDr["sgastat"] = "1";
                pDr["sgacomp"] = LoginInfo.CompNo;
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
                        pDr["sgb02"] = WfGetMaxSeq(pDr.Table, "sgb02");
                        pDr["sgb05"] = 0;
                        pDr["sgb08"] = 0;
                        pDr["sgb09"] = 0;
                        pDr["sgb10"] = 0;
                        pDr["sgb10t"] = 0;
                        pDr["sgb11"] = DrMaster["sga19"];
                        pDr["sgb13"] = 0;
                        pDr["sgb14"] = 0;
                        pDr["sgb17"] = 0;
                        pDr["sgb18"] = 0;
                        pDr["sgb19"] = 0;
                        pDr["sgbcomp"] = LoginInfo.CompNo;
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
                sgaOrg = null;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPreInUpdateModeCheck() 進修改模式前檢查,及設定變數
        protected override bool WfPreInUpdateModeCheck()
        {
            vw_stpt400 masterModel;
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_stpt400>();
                if (masterModel.sgaconf != "N")
                    return false;
                sgaOrg = DrMaster.ToItem<sga_tb>();
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
            vw_stpt400 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_stpt400>();
                if (masterModel.sgaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認,不可刪除!");
                    return false;
                }

                //還需檢查銷退單
                if (WfChkShaExists(masterModel.sga01) == true)
                {
                    WfShowErrorMsg("已有銷退資料!不可刪除!");
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
            vw_stpt400 masterModel = null;
            vw_stpt400s detailModel = null, newDetailModel = null;
            List<vw_stpt400s> detailList = null;
            bab_tb babModel = null;
            sfa_tb sfaModel = null;
            UltraGrid uGrid = null;
            int ChkCnts = 0;
            decimal price = 0;
            Result result = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt400>();
                if (e.Column.ToLower() != "sga01" && GlobalFn.isNullRet(DrMaster["sga01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false;
                }
                #region 單頭-vw_stpt400
                if (e.Row.Table.Prefix.ToLower() == "vw_stpt400")
                {
                    switch (e.Column.ToLower())
                    {
                        case "sga01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "stp", "30") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["sga01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            WfSetSga01RelReadonly(GlobalFn.isNullRet(e.Value, ""));
                            if (ute_sga19.ReadOnly != true)
                                WfItemChkForceFocus(ute_sga19);
                            break;
                        case "sga02":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(e.Value), BaaModel);
                            if (result.Success == false)
                            {
                                WfShowErrorMsg(result.Message);
                                return false;
                            }
                            break;
                        case "sga03"://客戶
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sga03_c"] = "";
                                e.Row["sga14"] = "";    //貨運方式
                                e.Row["sga15"] = "";    //起運地
                                e.Row["sga16"] = "";    //到達地
                                e.Row["sga17"] = "";    //客戶單號
                                e.Row["sga18"] = "";    //送貨地址碼
                                e.Row["sga19"] = "";    //訂單編號
                                return true;
                            }

                            if (BoStp.OfChkScaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此客戶資料,請檢核!");
                                return false;
                            }

                            if (!GlobalFn.varIsNull(masterModel.sga19))
                            {
                                if (WfChkSfa01Sfa03(masterModel.sga19, e.Value.ToString()) == false)
                                {
                                    WfShowErrorMsg("客戶與訂單客戶不同,請檢核");
                                    return false;
                                }
                            }
                            //檢查單身訂單編號
                            foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                            {
                                var sgb11 = GlobalFn.isNullRet(drTemp["sgb11"], "");
                                if (GlobalFn.varIsNull(sgb11))
                                    continue;
                                if (WfChkSfa01Sfa03(sgb11, e.Value.ToString()) == false)
                                {
                                    WfShowErrorMsg("客戶與明細訂單客戶不同,請檢核");
                                    return false;
                                }
                            }
                            WfSetSga03Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtBySga06();
                            break;

                        case "sga04"://業務人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sga04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["sga04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sga05"://業務部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sga05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["sga05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sga06"://課稅別
                            WfSetSga06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtBySga06();
                            break;

                        case "sga10"://幣別
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (BoBas.OfChkBekPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此幣別");
                                return false;
                            }
                            BekTbModel = BoBas.OfGetBekModel(e.Value.ToString());
                            break;

                        case "sga11"://收款條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sga11_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBefPKValid("2", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此收款條件,請檢核!");
                                return false;
                            }
                            e.Row["sga11_c"] = BoBas.OfGetBef03("1", GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sga12"://取價條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["sga12_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbbPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此取價條件,請檢核!");
                                return false;
                            }
                            e.Row["sga12_c"] = BoStp.OfGetSbb02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sga14"://貨運方式
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sga14_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBelPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運方式,請檢核!");
                                return false;
                            }
                            e.Row["sga14_c"] = BoBas.OfGetBel02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sga15"://貨運起點
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sga15_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbgPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運起點,請檢核!");
                                return false;
                            }
                            e.Row["sga15_c"] = BoStp.OfGetSbg02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sga16"://貨運終點
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sga16_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbgPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運終點,請檢核!");
                                return false;
                            }
                            e.Row["sga16_c"] = BoStp.OfGetSbg02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sga19"://訂單單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfSetControlReadonly(new List<Control> { ute_sga03 }, false);
                                DrMaster["sga12"] = "";
                                DrMaster["sga12_c"] = "";
                                return true;
                            }
                            sfaModel = BoStp.OfGetSfaModel(GlobalFn.isNullRet(e.Value, ""));
                            if (sfaModel == null)
                            {
                                WfShowErrorMsg("無此訂單!");
                                return false;
                            }
                            if (sfaModel.sfaconf != "Y")
                            {
                                WfShowErrorMsg("訂單非確認狀態!");
                                return false;
                            }
                            //檢查與單身是否一致!
                            detailList = TabDetailList[0].DtSource.ToList<vw_stpt400s>();
                            ChkCnts = detailList.Where(p => p.sgb11 != sfaModel.sfa01 && GlobalFn.isNullRet(p.sgb11, "") != "")
                                               .Count();
                            if (ChkCnts > 0)
                            {
                                WfShowErrorMsg("單頭與單身的訂單單號不一致!");
                                return false;
                            }
                            //這裡會有客戶跟訂單單搶壓資料的問題:先以訂單單為主
                            DrMaster["sga03"] = sfaModel.sfa03;
                            DrMaster["sga03_c"] = BoStp.OfGetSca02(sfaModel.sfa03);
                            WfSetControlReadonly(new List<Control> { ute_sga03 }, true);
                            DrMaster["sga06"] = sfaModel.sfa06;
                            DrMaster["sga07"] = sfaModel.sfa07;
                            DrMaster["sga08"] = sfaModel.sfa08;
                            DrMaster["sga09"] = sfaModel.sfa09;
                            DrMaster["sga10"] = sfaModel.sfa10;
                            DrMaster["sga11"] = sfaModel.sfa11;
                            DrMaster["sga11_c"] = BoBas.OfGetBef03("2", sfaModel.sfa11);
                            DrMaster["sga12"] = sfaModel.sfa12;
                            DrMaster["sga12_c"] = BoStp.OfGetSbb02(sfaModel.sfa12);
                            DrMaster["sga14"] = sfaModel.sfa14;
                            DrMaster["sga15"] = sfaModel.sfa15;
                            DrMaster["sga16"] = sfaModel.sfa16;
                            DrMaster["sga21"] = sfaModel.sfa21;
                            break;
                        case "sga21": //匯率
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

                #region 單身-vw_stpt400s
                if (e.Row.Table.Prefix.ToLower() == "vw_stpt400s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_stpt400s>();
                    detailList = e.Row.Table.ToList<vw_stpt400s>();
                    babModel = BoBas.OfGetBabModel(masterModel.sga01);

                    switch (e.Column.ToLower())
                    {
                        case "sgb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            detailList = e.Row.Table.ToList<vw_stpt400s>();
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.sgb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "sgb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sgb04"] = "";//品名
                                e.Row["sgb05"] = 0;//銷貨數量
                                e.Row["sgb06"] = "";//銷貨單位
                                e.Row["sgb07"] = "";//庫存單位
                                e.Row["sgb08"] = 0;//庫存轉換率
                                e.Row["sgb13"] = 0;//訂單轉換率
                                e.Row["sgb14"] = 0;//訂單數量
                                e.Row["sgb15"] = "";//訂單單位
                                e.Row["sgb18"] = 0;//庫存數量
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }

                            if (WfSetSgb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;

                            if (!GlobalFn.varIsNull(masterModel.sga12))
                            {
                                newDetailModel = e.Row.ToItem<vw_stpt400s>();
                                result = BoStp.OfGetPrice(masterModel.sga12, masterModel.sga03, e.Value.ToString(), newDetailModel.sgb06,
                                    masterModel.sga02, masterModel.sga10, "3", newDetailModel.sgb05,
                                    masterModel.sga08, masterModel.sga07, masterModel.sga21, out price);
                                if (result.Success == true)
                                {
                                    e.Row["sgb09"] = price;
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
                        case "sgb05"://出貨數量
                            if (GlobalFn.varIsNull(detailModel.sgb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["sgb03"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(detailModel.sgb06))
                            {
                                WfShowErrorMsg("請先輸入銷貨單位!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["sgb06"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入銷貨數量!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["sgb05"]);
                                return false;
                            }
                            detailModel.sgb05 = BoBas.OfGetUnitRoundQty(detailModel.sgb06, detailModel.sgb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detailModel.sgb05;
                            if (WfChkSgb05(e.Row, detailModel) == false)
                                return false;
                            e.Row["sgb14"] = BoBas.OfGetUnitRoundQty(detailModel.sgb06, detailModel.sgb05 * detailModel.sgb13); //轉換訂單數量(並四拾伍入)
                            e.Row["sgb18"] = BoBas.OfGetUnitRoundQty(detailModel.sgb07, detailModel.sgb05 * detailModel.sgb08); //轉換庫存數量(並四拾伍入)
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "sgb06"://銷貨單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (GlobalFn.varIsNull(detailModel.sgb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["sgb03"]);
                                return false;
                            }
                            if (WfChkSgb06(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;

                            if (WfSetSgb06Relation(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;
                            break;

                        case "sgb09":   //單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.sgb09 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.sgb09, BekTbModel.bek03);
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "sgb11"://訂單單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("訂單單號不可為空白!");
                                return false;
                            }

                            if (WfChkSgb11(e.Value.ToString()) == false)
                                return false;

                            if (!GlobalFn.varIsNull(detailModel.sgb12))
                            {
                                if (WfChkSgb12(GlobalFn.isNullRet(detailModel.sgb11, ""), GlobalFn.isNullRet(detailModel.sgb12, 0)) == false)
                                    return false;

                                if (WfSetSgb12Relation(e.Row, GlobalFn.isNullRet(detailModel.sgb11, ""), GlobalFn.isNullRet(detailModel.sgb12, 0)) == false)
                                    return false;
                            }
                            WfSetTotalAmt();
                            break;

                        case "sgb12"://訂單項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("訂單項次不可為空白!");
                                return false;
                            }

                            if (WfChkSgb12(GlobalFn.isNullRet(detailModel.sgb11, ""), GlobalFn.isNullRet(detailModel.sgb12, 0)) == false)
                                return false;
                            WfSetSgb12Relation(e.Row, GlobalFn.isNullRet(detailModel.sgb11, ""), GlobalFn.isNullRet(detailModel.sgb12, 0));
                            WfSetTotalAmt();
                            break;

                        case "sgb16"://倉庫
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
                    case "vw_stpt400s":
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
            vw_stpt400 masterModel = null;
            vw_stpt400s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt400>();
                if (!GlobalFn.varIsNull(masterModel.sga01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.sga01, ""));
                #region 單頭資料檢查
                chkColName = "sga01";       //銷貨單號
                chkControl = ute_sga01;
                if (GlobalFn.varIsNull(masterModel.sga01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sga02";       //出庫日期
                chkControl = udt_sga02;
                if (GlobalFn.varIsNull(masterModel.sga02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.sga02), BaaModel);
                if (result.Success == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                chkColName = "sga03";       //客戶編號
                chkControl = ute_sga03;
                if (GlobalFn.varIsNull(masterModel.sga03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sga04";       //業務人員
                chkControl = ute_sga04;
                if (GlobalFn.varIsNull(masterModel.sga04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sga05";       //業務部門
                chkControl = ute_sga05;
                if (GlobalFn.varIsNull(masterModel.sga05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sga06";       //課稅別
                chkControl = ucb_sga06;
                if (GlobalFn.varIsNull(masterModel.sga06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sga12";
                chkControl = ute_sga12;
                if (GlobalFn.varIsNull(masterModel.sga12) && babModel.bab08 != "Y")//無來源單據取價條件要輸入
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sga21";
                chkControl = ute_sga21;
                if (GlobalFn.varIsNull(masterModel.sga21))
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

                    detailModel = drTemp.ToItem<vw_stpt400s>();
                    chkColName = "sgb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.sgb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sgb11";   //訂單單號
                    if (GlobalFn.varIsNull(detailModel.sgb11) && babModel.bab08 == "Y")  //有來源單據訂單單號要輸入
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sgb12";   //訂單項次
                    if (GlobalFn.varIsNull(detailModel.sgb12) && babModel.bab08 == "Y")  //有來源單據訂單單號要輸入
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sgb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.sgb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sgb05";   //出庫數量
                    #region pfb05 銷貨數量
                    if (GlobalFn.varIsNull(detailModel.sgb05) || detailModel.sgb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    if (WfChkSgb05(drTemp, detailModel) == false)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    chkColName = "sgb06";   //出庫單位
                    if (GlobalFn.varIsNull(detailModel.sgb06))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "sgb16";   //倉庫別
                    if (GlobalFn.varIsNull(detailModel.sgb16))
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

                WfGenSga23();//網拍平台自動產生手續費
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
            string sga01New, errMsg;
            vw_stpt400 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt400>();
                if (FormEditMode == YREditType.新增)
                {
                    sga01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.sga01, ModuleType.stp, (DateTime)masterModel.sga02, out sga01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["sga01"] = sga01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["sgasecu"] = LoginInfo.UserNo;
                        DrMaster["sgasecg"] = LoginInfo.GroupNo;
                        DrMaster["sgacreu"] = LoginInfo.UserNo;
                        DrMaster["sgacreg"] = LoginInfo.DeptNo;
                        DrMaster["sgacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["sgamodu"] = LoginInfo.UserNo;
                        DrMaster["sgamodg"] = LoginInfo.DeptNo;
                        DrMaster["sgamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["sgbcreu"] = LoginInfo.UserNo;
                            drDetail["sgbcreg"] = LoginInfo.DeptNo;
                            drDetail["sgbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["sgbmodu"] = LoginInfo.UserNo;
                            drDetail["sgbmodg"] = LoginInfo.DeptNo;
                            drDetail["sgbmodd"] = Now;
                        }
                    }
                }

                DrMaster.EndEdit();
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

                bt = new ButtonTool("Stpr400");
                adoModel = BoAdm.OfGetAdoModel("Stpr400");
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

                    case "Stpr400":
                        vw_stpr400 stpr400Model;
                        vw_stpt400 masterModel;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        masterModel = DrMaster.ToItem<vw_stpt400>();
                        stpr400Model = new vw_stpr400();
                        stpr400Model.sga01 = masterModel.sga01;
                        stpr400Model.sga03 = "";
                        stpr400Model.jump_yn = "N";
                        stpr400Model.order_by = "1";

                        FrmStpr400 rpt = new FrmStpr400(this.LoginInfo, stpr400Model, true, true);
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
        #region WfSetSga01RelReadonly 依單別設定相關控制項 readonly
        private void WfSetSga01RelReadonly(string pSga01)
        {
            bab_tb babModel = null;
            string bab01;
            try
            {
                WfSetControlReadonly(new List<Control> { ute_sga01 }, true);
                bab01 = pSga01.Substring(0, GlobalFn.isNullRet(BaaTbModel.baa06, 0));
                babModel = BoBas.OfGetBabModel(bab01);
                if (babModel.bab08 == "Y")   //有來源單據
                {
                    WfSetControlReadonly(new List<Control> { ute_sga12 }, true);
                    WfSetControlReadonly(new List<Control> { ute_sga19 }, false);
                }
                else
                {
                    WfSetControlReadonly(new List<Control> { ute_sga12 }, false);
                    WfSetControlReadonly(new List<Control> { ute_sga19 }, true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetSga03Relation 設定客戶相關聯
        private void WfSetSga03Relation(string pSga03)
        {
            sca_tb scaModel;
            bab_tb babModel = null;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sga01"], ""));
                scaModel = BoStp.OfGetScaModel(pSga03);
                if (scaModel == null)
                    return;

                DrMaster["sga03_c"] = scaModel.sca03;
                DrMaster["sga06"] = scaModel.sca22;    //課稅別
                WfSetSga06Relation(scaModel.sca22);
                DrMaster["sga09"] = scaModel.sca23;    //發票聯數
                DrMaster["sga11"] = scaModel.sca21;    //付款條件
                DrMaster["sga11_c"] = BoBas.OfGetBef03("1", scaModel.sca21);
                //DRMASTER["sga14"] = l_sca.pca35;    //送貨地址
                //DRMASTER["sga15"] = l_sca.pca36;    //帳單地址

                if (babModel.bab08 != "Y")   //無前置單據則取價條件帶入
                {
                    DrMaster["sga12"] = scaModel.sca24;    //取價條件
                    DrMaster["sga12_c"] = BoStp.OfGetSbb02(scaModel.sca24);
                }
                else
                {
                    DrMaster["sga12"] = "";
                    DrMaster["sga12_c"] = "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetSga06Relation 設定稅別關聯
        private void WfSetSga06Relation(string pSga06)
        {
            try
            {
                if (pSga06 == "1")
                {
                    DrMaster["sga07"] = BaaTbModel.baa05;
                    DrMaster["sga08"] = "Y";
                }
                else if (pSga06 == "2")
                {
                    DrMaster["sga07"] = BaaTbModel.baa05;
                    DrMaster["sga08"] = "N";
                }
                else
                {
                    DrMaster["sga07"] = 0;
                    DrMaster["sga08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetSgb03Relation 設定料號關聯
        private bool WfSetSgb03Relation(string pSgb03, DataRow pDr)
        {
            ica_tb icaModel;
            decimal sgb08;   //轉換率
            try
            {
                icaModel = BoInv.OfGetIcaModel(pSgb03);
                sgb08 = 0;
                if (icaModel == null)
                {
                    pDr["sgb04"] = "";//品名
                    pDr["sgb05"] = 0;//出貨數量
                    pDr["sgb06"] = "";//出貨單位
                    pDr["sgb07"] = "";//庫存單位
                    pDr["sgb08"] = 0;//庫存轉換率
                    pDr["sgb18"] = 0;//庫存數量
                }
                else
                {
                    if (BoInv.OfGetUnitCovert(pSgb03, icaModel.ica09, icaModel.ica07, out sgb08) == false)
                    {
                        WfShowErrorMsg("未設定料號轉換,請先設定!");
                        return false;
                    }
                    pDr["sgb04"] = icaModel.ica02;//品名
                    pDr["sgb05"] = 0;//出貨數量
                    pDr["sgb06"] = icaModel.ica09;//出貨單位帶銷售單位
                    pDr["sgb07"] = icaModel.ica07;//庫存單位
                    pDr["sgb08"] = sgb08;
                    pDr["sgb18"] = 0;//庫存數量
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetSgb06Relation 設定銷貨單位關聯
        //檢查前要先確認料號是否已輸入
        private bool WfSetSgb06Relation(DataRow pDr, string pSgb06, string pBab08)
        {
            vw_stpt400s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_stpt400s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pSgb06, "")) == false)
                {
                    WfShowErrorMsg("無此銷售單位!請確認");
                    return false;
                }
                //取得是否有銷售對庫存的轉換率
                dConvert = 0;
                if (BoInv.OfGetUnitCovert(detailModel.sgb03, pSgb06, detailModel.sgb07, out dConvert) == true)
                {
                    pDr["sgb08"] = dConvert;
                    pDr["sgb18"] = BoBas.OfGetUnitRoundQty(detailModel.sgb07, detailModel.sgb05 * dConvert); //轉換庫存數量(並四拾伍入)
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有銷售對訂單的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.sgb03, pSgb06, detailModel.sgb15, out dConvert) == true)
                    {
                        pDr["sgb13"] = dConvert;
                        pDr["sgb14"] = BoBas.OfGetUnitRoundQty(detailModel.sgb15, detailModel.sgb05 * dConvert); //轉換庫存數量(並四拾伍入)
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

        #region WfSetSgb12Relation 設定訂單明細資料關聯
        private bool WfSetSgb12Relation(DataRow pdr, string pSgb11, int pSgb12)
        {
            sfb_tb sfbModel;
            decimal sgb18;
            try
            {
                sfbModel = BoStp.OfGetSfbModel(pSgb11, pSgb12);
                if (sfbModel == null)
                    return false;
                sgb18 = BoBas.OfGetUnitRoundQty(sfbModel.sfb07, sfbModel.sfb05 * sfbModel.sfb08); //轉換庫存數量(並四拾伍入) 先取,避免錯誤

                pdr["sgb03"] = sfbModel.sfb03;//料號
                pdr["sgb04"] = sfbModel.sfb04;//品名
                pdr["sgb05"] = sfbModel.sfb05;//銷售數量
                pdr["sgb06"] = sfbModel.sfb06;//銷售單位
                pdr["sgb07"] = sfbModel.sfb07;//庫存單位
                pdr["sgb08"] = sfbModel.sfb08;//庫存轉換率
                pdr["sgb09"] = sfbModel.sfb09;//單價
                pdr["sgb10"] = sfbModel.sfb10;//未稅金額
                pdr["sgb10t"] = sfbModel.sfb10t;//含稅金額

                pdr["sgb13"] = 1;          //銷售對訂單轉換率
                pdr["sgb14"] = sfbModel.sfb05;//轉換訂單數量
                pdr["sgb15"] = sfbModel.sfb06;//原訂單單位
                pdr["sgb16"] = sfbModel.sfb16;//倉庫別

                pdr["sgb18"] = sgb18; //轉換庫存數量(並四拾伍入)
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkSfa01Sfa03 檢查客戶與訂單的合理性
        private bool WfChkSfa01Sfa03(string pSfa01, string pSfa03)
        {
            int iChkCnts = 0;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sfa_tb");
                sbSql.AppendLine("WHERE sfa01=@sfa01 AND sfa03=@sfa03");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@sfa01",pSfa01),
                                                      new SqlParameter("@sfa03",pSfa03)
                                                    };
                iChkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (iChkCnts == 0)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkSgb05 數量檢查
        private bool WfChkSgb05(DataRow pdr, vw_stpt400s pListDetail)
        {
            List<vw_stpt400s> detailList = null;
            sfb_tb sfbModel = null;
            bab_tb babModel = null;
            string errMsg;
            decimal docThisQty = 0;         //本項次的轉換訂單數量
            decimal docOtherQtyTotal = 0;   //本單據其他項次的訂單數量加總
            decimal otherQtyTotal = 0;      //其他單據的數量加總
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                detailList = pdr.Table.ToList<vw_stpt400s>();
                if (GlobalFn.varIsNull(pListDetail.sgb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.sgb03))
                {
                    errMsg = "請先輸入料號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.sgb06))
                {
                    errMsg = "請先輸入銷售單位!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (pListDetail.sgb05 <= 0)
                {
                    errMsg = "銷貨數量應大於0!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sga01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (babModel.bab08 != "Y") //無來源單據,以下不檢查!
                    return true;

                sfbModel = BoStp.OfGetSfbModel(pListDetail.sgb11, Convert.ToInt16(pListDetail.sgb12));
                if (babModel == null)
                {
                    errMsg = "Get sfb_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                //取得本身單據中的所有數量轉換銷貨單位後的總和!
                //先取本身這筆
                docThisQty = BoBas.OfGetUnitRoundQty(pListDetail.sgb06, pListDetail.sgb05 * pListDetail.sgb13);
                //再取其他筆加總
                docOtherQtyTotal = detailList.Where(p => p.sgb02 != pListDetail.sgb02)
                                   .Where(p => p.sgb11 == pListDetail.sgb11 && p.sgb12 == pListDetail.sgb12)
                                   .Sum(p => p.sgb14);
                //取得其他單據上的加總
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT SUM(sgb14) FROM sga_tb");
                sbSql.AppendLine("  INNER JOIN sgb_tb ON sga01=sgb01");
                sbSql.AppendLine("WHERE sgaconf<>'X'");
                sbSql.AppendLine("  AND sga01 <> @sga01");
                sbSql.AppendLine("  AND sgb11 = @sgb11 AND sgb12 = @sgb12");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@sga01", GlobalFn.isNullRet(DrMaster["sga01"], "")));
                sqlParmList.Add(new SqlParameter("@sgb11", pListDetail.sgb11));
                sqlParmList.Add(new SqlParameter("@sgb12", pListDetail.sgb12));
                otherQtyTotal = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (sfbModel.sfb05 < (docThisQty + docOtherQtyTotal + otherQtyTotal))
                {
                    errMsg = string.Format("項次{0}訂單最大可輸入數量為 {1}",
                                            pListDetail.sgb02.ToString(),
                                            (sfbModel.sfb05 - docOtherQtyTotal - otherQtyTotal).ToString()
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

        #region WfChkSgb06 檢查銷貨單位
        //檢查前要先確認料號是否已輸入
        private bool WfChkSgb06(DataRow pDr, string pSgb06, string pBab08)
        {
            vw_stpt400s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_stpt400s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pSgb06, "")) == false)
                {
                    WfShowErrorMsg("無此銷售單位!請確認");
                    return false;
                }
                //檢查是否有銷售對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.sgb03, pSgb06, detailModel.sgb07, out dConvert) == false)
                {
                    WfShowErrorMsg("未設定銷貨單位對庫存單位的轉換率,請先設定!");
                    return false;
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有銷售對訂單的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.sgb03, pSgb06, detailModel.sgb06, out dConvert) == false)
                    {
                        WfShowErrorMsg("未設定銷售單位對訂單單位的轉換率,請先設定!");
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

        #region WfChkSgb11 訂單檢查
        private bool WfChkSgb11(string pSgb11)
        {
            string errMsg;
            sfa_tb sfaModel;
            string sga19;
            try
            {
                sfaModel = BoStp.OfGetSfaModel(pSgb11);
                if (sfaModel == null)
                {
                    errMsg = "查無此訂單!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (sfaModel.sfaconf != "Y")
                {
                    errMsg = "訂單非確認狀態!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (sfaModel.sfastat == "9")
                {
                    errMsg = "訂單已結案!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                sga19 = GlobalFn.isNullRet(DrMaster["sga19"], "");
                if (sga19 != "" && sga19 != pSgb11)
                {
                    errMsg = "訂單單號與單頭不同!";
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

        #region WfChkSgb12 訂單項次檢查
        private bool WfChkSgb12(string pSgb11, int pSgb12)
        {
            string errMsg;
            sfb_tb sfbModel;
            try
            {
                sfbModel = BoStp.OfGetSfbModel(pSgb11, pSgb12);
                if (sfbModel == null)
                {
                    errMsg = "查無此訂單項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if ((sfbModel.sfb05 - sfbModel.sfb17) <= 0)
                {
                    errMsg = string.Format("無可用訂單數量!(可用訂單數量為{0})", GlobalFn.isNullRet(sfbModel.sfb05 - sfbModel.sfb17, 0));
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

        #region WfChkShaExists 檢查銷退單是否存在
        private bool WfChkShaExists(string pSga01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChkCnts = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sha_tb");
                sbSql.AppendLine("  INNER JOIN shb_tb ON sha01=shb01");
                sbSql.AppendLine("WHERE");
                sbSql.AppendLine(" shaconf <>'X' ");
                sbSql.AppendLine(" AND shb11=@shb11 ");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@shb11", pSga01));
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
            sga_tb sgaModel = null;
            sgb_tb sgbModel = null;

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

                sgaModel = DrMaster.ToItem<sga_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                sgaModel.sgaconf = "Y";
                sgaModel.sgacond = Today;
                sgaModel.sgaconu = LoginInfo.UserNo;

                //更新訂單銷貨量
                if (WfUpdSfb17(true) == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }

                if (WfUpdSfastatByConfirm() == false)
                {
                    WfRollback();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    sgbModel = dr.ToItem<sgb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("2", sgbModel.sgb03, sgbModel.sgb16, sgbModel.sgb18, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                    //更新庫存交易歷史檔
                    if (BoInv.OfStockPost("stpt400", sgaModel, sgbModel, LoginInfo, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                }

                DrMaster["sgastat"] = "1";
                DrMaster["sgaconf"] = "Y";
                DrMaster["sgacond"] = Today;
                DrMaster["sgaconu"] = LoginInfo.UserNo;
                DrMaster["sgamodu"] = LoginInfo.UserNo;
                DrMaster["sgamodg"] = LoginInfo.DeptNo;
                DrMaster["sgamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                sgaModel = DrMaster.ToItem<sga_tb>();
                WfSetDocPicture("", sgaModel.sgaconf, sgaModel.sgastat, pbxDoc);
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
            vw_stpt400 masterModel = null;
            vw_stpt400s detailModel = null;
            List<vw_stpt400s> detailList = null;
            StringBuilder sbSql;
            decimal icc05 = 0;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt400>();
                if (masterModel.sgaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    return false;
                }

                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.sga02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_stpt400s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_stpt400s>();
                    if (WfChkSgb05(drTemp, detailModel) == false)
                    {
                        return false;
                    }
                }

                //檢查銷貨單的數量是否足夠做出庫
                detailList = TabDetailList[0].DtSource.ToList<vw_stpt400s>();
                var sgbSumList =                         //依銷貨單的料號及倉庫做加總
                        from sgb in detailList
                        where sgb.sgb18 > 0
                        group sgb by new { sgb.sgb03, sgb.sgb16 } into sgbSumTemp
                        select new
                        {
                            sgb03 = sgbSumTemp.Key.sgb03,
                            sgb16 = sgbSumTemp.Key.sgb16,
                            sgb18 = sgbSumTemp.Sum(p => p.sgb18)
                        }
                    ;
                foreach (var sgbSumModel in sgbSumList)
                {
                    icc05 = BoInv.OfGetIcc05(sgbSumModel.sgb03, sgbSumModel.sgb16);
                    if (icc05 < sgbSumModel.sgb18)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", sgbSumModel.sgb03, icc05));
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
            sga_tb sgaModel = null;
            sgb_tb sgbModel = null;
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
                sgaModel = DrMaster.ToItem<sga_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                if (WfUpdSfb17(false) == false)
                {
                    WfRollback();
                    return;
                }

                if (WfUpdSfastatByCancleConfirm() == false)
                {
                    WfRollback();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    sgbModel = dr.ToItem<sgb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("1", sgbModel.sgb03, sgbModel.sgb16, sgbModel.sgb18, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }

                    //更新庫存交易歷史檔
                    if (BoInv.OfDelIna(sgbModel.sgb01, sgbModel.sgb02, "2", out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                }

                DrMaster["sgastat"] = "0";
                DrMaster["sgaconf"] = "N";
                DrMaster["sgacond"] = DBNull.Value;
                DrMaster["sgaconu"] = "";
                DrMaster["sgamodu"] = LoginInfo.UserNo;
                DrMaster["sgamodg"] = LoginInfo.DeptNo;
                DrMaster["sgamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                sgaModel = DrMaster.ToItem<sga_tb>();
                WfSetDocPicture("", sgaModel.sgaconf, sgaModel.sgastat, pbxDoc);
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
            vw_stpt400 masterModel = null;
            List<sgb_tb> sgbList = null;
            decimal icc05;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpt400>();
                if (masterModel.sgaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.sga02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                //檢查是否存在銷退單
                if (WfChkShaExists(masterModel.sga01) == true)
                {
                    WfShowErrorMsg("已存在銷退單,不可取消確認!");
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
            vw_stpt400 masterModel = null;
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
                masterModel = DrMaster.ToItem<vw_stpt400>();

                if (masterModel.sgaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.sgaconf == "N")//走作廢
                {

                    DrMaster["sgastat"] = "X";
                    DrMaster["sgaconf"] = "X";
                    DrMaster["sgaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.sgaconf == "X")
                {
                    //檢查訂單是否為有效資料
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT COUNT(1)");
                    sbSql.AppendLine("FROM sfa_tb");
                    sbSql.AppendLine("WHERE exists ");
                    sbSql.AppendLine("      (SELECT 1 FROM sgb_tb WHERE sgb01=@sgb01");
                    sbSql.AppendLine("      AND sgb11=sfa01)");
                    sbSql.AppendLine("AND ISNULL(sfaconf,'')<>'Y'");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@sgb01", GlobalFn.isNullRet(DrMaster["sga01"], "")));
                    iChkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                    if (iChkCnts > 0)
                    {
                        WfShowErrorMsg("訂單含有未確認資料!不可作廢還原!");
                        WfRollback();
                        return;
                    }
                    DrMaster["sgastat"] = "0";
                    DrMaster["sgaconf"] = "N";
                    DrMaster["sgaconu"] = "";
                }
                DrMaster["sgamodu"] = LoginInfo.UserNo;
                DrMaster["sgamodg"] = LoginInfo.DeptNo;
                DrMaster["sgamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_stpt400>();
                WfSetDocPicture("", masterModel.sgaconf, masterModel.sgastat, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfUpdPfb17 更新訂單已轉銷貨數量 確認/取消確認
        private bool WfUpdSfb17(bool pbConfirm)
        {
            bab_tb babModel = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            DataTable dtSgbDistinct = null;
            string sgb01;
            decimal sgb02;
            decimal docQty = 0, otherDocQty = 0;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["sga01"], ""));
                if (babModel == null)
                {
                    WfShowErrorMsg("Get bab_tb Error!");
                    return false;
                }
                if (babModel.bab08 == "Y")   //更新訂單出庫數量
                {
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT DISTINCT sgb11,sgb12,SUM(sgb14) sgb14");
                    sbSql.AppendLine("FROM sgb_tb");
                    sbSql.AppendLine("WHERE sgb01=@sgb01");
                    sbSql.AppendLine("  AND ISNULL(sgb11,'')<>''");
                    sbSql.AppendLine("GROUP BY sgb11,sgb12");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@sgb01", GlobalFn.isNullRet(DrMaster["sga01"], "")));
                    dtSgbDistinct = BoStp.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                    if (dtSgbDistinct == null)
                        return true;
                    foreach (DataRow dr in dtSgbDistinct.Rows)
                    {
                        sgb01 = dr["sgb11"].ToString();
                        sgb02 = Convert.ToDecimal(dr["sgb12"]);
                        docQty = Convert.ToDecimal(dr["sgb14"]);

                        sbSql = new StringBuilder();
                        sbSql.AppendLine("SELECT SUM(sgb14) FROM sga_tb");
                        sbSql.AppendLine("  INNER JOIN sgb_tb ON sga01=sgb01");
                        sbSql.AppendLine("WHERE sgaconf='Y'");
                        sbSql.AppendLine("AND sgb11=@sgb11 AND sgb12=@sgb12");
                        sbSql.AppendLine("AND sga01<>@sga01");

                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@sgb11", sgb01));
                        sqlParmList.Add(new SqlParameter("@sgb12", sgb02));
                        sqlParmList.Add(new SqlParameter("@sga01", GlobalFn.isNullRet(DrMaster["sga01"], "")));
                        otherDocQty = 0;
                        otherDocQty = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);

                        sbSql = new StringBuilder();
                        sbSql = sbSql.AppendLine("UPDATE sfb_tb");
                        sbSql = sbSql.AppendLine("SET sfb17=@sfb17");
                        sbSql = sbSql.AppendLine("WHERE sfb01=@sfb01");
                        sbSql = sbSql.AppendLine("AND sfb02=@sfb02");
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@sfb01", sgb01));
                        sqlParmList.Add(new SqlParameter("@sfb02", sgb02));
                        if (pbConfirm)  //確認的要加本身單據
                            sqlParmList.Add(new SqlParameter("@sfb17", docQty + otherDocQty));
                        else
                            sqlParmList.Add(new SqlParameter("@sfb17", otherDocQty));

                        if (BoStp.OfExecuteNonquery(sbSql.ToString(), sqlParmList.ToArray()) != 1)
                        {
                            WfShowErrorMsg("更新訂單出貨數量失敗!");
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

        #region WfUpdSfastatByConfirm() 更新客戶訂單狀態 確認
        private bool WfUpdSfastatByConfirm()
        {
            List<vw_stpt400s> detailList;
            int chkCnts = 0;
            string querySql = "", updateSql = "";
            List<SqlParameter> sqlParms;
            try
            {
                detailList = TabDetailList[0].DtSource.ToList<vw_stpt400s>();
                //取得請購單單號
                var sgbDistinct = from o in detailList
                                  where !GlobalFn.varIsNull(o.sgb11)
                                  group o by new { o.sgb11 } into sgbTemp
                                  select new
                                  {
                                      sgb11 = sgbTemp.Key.sgb11
                                  }
                               ;

                if (sgbDistinct == null)
                    return true;
                //查詢
                querySql = @"SELECT COUNT(1)
                        FROM sfa_tb
	                        INNER JOIN sfb_tb ON sfa01=sfb01
                        WHERE sfa01=@sfa01
	                        AND sfastat='1'
	                        AND (sfb05-sfb17)>0
                        ";
                //逐筆更新
                updateSql = @"UPDATE dbo.sfa_tb
                            SET sfastat='9'
                            WHERE sfa01=@sfa01
                        ";

                foreach (var sgbModel in sgbDistinct)
                {
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@sfa01", sgbModel.sgb11));
                    chkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(querySql, sqlParms.ToArray()), 0);
                    if (chkCnts > 0)
                        continue;
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@sfa01", sgbModel.sgb11));
                    if (BoStp.OfExecuteNonquery(updateSql, sqlParms.ToArray()) < 1)
                    {
                        WfShowErrorMsg("更新sfastat失敗!");
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

        #region WfUpdSfastatByCancleConfirm() 更新客戶訂單狀態 取消確認
        private bool WfUpdSfastatByCancleConfirm()
        {
            List<vw_stpt400s> detailList;
            int chkCnts = 0;
            string querySql = "", updateSql = "";
            List<SqlParameter> sqlParms;
            try
            {
                detailList = TabDetailList[0].DtSource.ToList<vw_stpt400s>();
                //取得採購單單號
                var sgbDistinct = from o in detailList
                                  where !GlobalFn.varIsNull(o.sgb11)
                                  group o by new { o.sgb11 } into sgbTemp
                                  select new
                                  {
                                      sgb11 = sgbTemp.Key.sgb11
                                  }
                               ;

                if (sgbDistinct == null)
                    return true;
                //查詢
                querySql = @"SELECT COUNT(1)
                        FROM sfa_tb
                        WHERE sfa01=@sfa01
	                        AND sfastat='9'
                        ";
                //逐筆更新
                updateSql = @"UPDATE dbo.sfa_tb
                            SET sfastat='1'
                            WHERE sfa01=@sfa01
                        ";

                foreach (var sgbModel in sgbDistinct)
                {
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@sfa01", sgbModel.sgb11));
                    chkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(querySql, sqlParms.ToArray()), 0);
                    if (chkCnts == 0)
                        continue;
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@sfa01", sgbModel.sgb11));
                    if (BoStp.OfExecuteNonquery(updateSql, sqlParms.ToArray()) < 1)
                    {
                        WfShowErrorMsg("更新sfastat失敗!");
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
        private bool WfSetDetailAmt(DataRow drSeb)
        {
            sga_tb sgaModel;
            sgb_tb sgbModel;
            decimal sgb10t = 0, sgb10 = 0;
            try
            {
                sgaModel = DrMaster.ToItem<sga_tb>();
                sgbModel = drSeb.ToItem<sgb_tb>();

                if (sgaModel.sga08 == "Y")//稅內含
                {
                    sgb10t = sgbModel.sgb09 * sgbModel.sgb05;
                    sgb10t = GlobalFn.Round(sgb10t, BekTbModel.bek04);
                    sgb10 = sgb10t / (1 + (sgaModel.sga07 / 100));
                    sgb10 = GlobalFn.Round(sgb10, BekTbModel.bek04);
                }
                else//稅外加
                {
                    sgb10 = sgbModel.sgb09 * sgbModel.sgb05;
                    sgb10 = GlobalFn.Round(sgb10, BekTbModel.bek04);
                    sgb10t = sgb10 * (1 + (sgaModel.sga07 / 100));
                    sgb10t = GlobalFn.Round(sgb10t, BekTbModel.bek04);
                }
                drSeb["sgb10"] = sgb10;
                drSeb["sgb10t"] = sgb10t;
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
            sga_tb sgaModel;
            decimal sga13 = 0, sga13t = 0, sga13g;
            try
            {
                sgaModel = DrMaster.ToItem<sga_tb>();
                if (sgaModel.sga08 == "Y")//稅內含,先處理含稅金額
                {
                    foreach (sgb_tb l_sgb in TabDetailList[0].DtSource.ToList<sgb_tb>())
                    {
                        sga13t += l_sgb.sgb10t;
                    }
                    sga13t = GlobalFn.Round(sga13t, BekTbModel.bek04);
                    sga13 = sga13t / (1 + sgaModel.sga07 / 100);
                    sga13 = GlobalFn.Round(sga13, BekTbModel.bek04);
                    sga13g = sga13t - sga13;
                }
                else//稅外加
                {
                    foreach (sgb_tb sgbModel in TabDetailList[0].DtSource.ToList<sgb_tb>())
                    {
                        sga13 += sgbModel.sgb10;
                    }
                    sga13 = GlobalFn.Round(sga13, BekTbModel.bek04);
                    sga13g = sga13 * (sgaModel.sga07 / 100);
                    sga13g = GlobalFn.Round(sga13g, BekTbModel.bek04);
                    sga13t = sga13 + sga13g;
                }

                DrMaster["sga13"] = sga13;
                DrMaster["sga13t"] = sga13t;
                DrMaster["sga13g"] = sga13g;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfResetAmtBySga06 依稅別更新單身及單頭的金額
        private void WfResetAmtBySga06()
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

        #region WfGenSga23 依不同平台自動計算額外的出貨成本
        private void WfGenSga23()
        {
            sga_tb masterModel = null;
            decimal sga23 = 0;           

            try
            {
                if (DrMaster == null)
                    return;
                masterModel = DrMaster.ToItem<sga_tb>();
                if (masterModel.sga03 != "C000002"  //蝦皮 
                    && masterModel.sga03 != "C000003"//露天
                    && masterModel.sga03 != "C000004"  //奇摩
                    )
                {
                    return;
                }

                if (FormEditMode == YREditType.新增 && masterModel.sga13t != 0 ||
                    FormEditMode == YREditType.修改 && masterModel.sga13t != sgaOrg.sga13t
                    )
                {
                    DialogResult diaResult = WfShowConfirmMsg("是否要自動計算出貨成本?");
                    if (diaResult == DialogResult.Yes)
                    {
                        Result rtnResult = BoStp.OfGetSga23(masterModel,out sga23);
                        if (rtnResult.Success==false)
                        {
                            WfShowErrorMsg(rtnResult.Message);
                            return;
                        }

                        sga23 = GlobalFn.Round(sga23, 0);
                        DrMaster["sga23"] = sga23;
                    }

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
