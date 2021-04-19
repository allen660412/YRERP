/* 程式名稱: 客戶基本資料建立
   系統代號: stpi100
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

namespace YR.ERP.Forms.Stp
{
    public partial class FrmStpi100 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        StpBLL BoStp = null;
        BasBLL BoBas = null;
        string Bga01 = "", Bgc03 = "", Sca01New = "";          //新增時在存檔前用來重新取號使用
        #endregion

        #region 建構子
        public FrmStpi100()
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
            this.StrFormID = "stpi100";
            uTab_Header.Tabs[0].Text = "基本資料";
            uTab_Header.Tabs[1].Text = "狀態";

            this.IntTabCount = 3;
            this.IntMasterGridPos = 3;
            uTab_Master.Tabs[0].Text = "客戶資料";
            uTab_Master.Tabs[1].Text = "交易資料";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("sca01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "seasecu";
                TabMaster.GroupColumn = "seasecg";
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

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                //單據發送方式
                sourceList = BoStp.OfGetSca20KVPList();
                WfSetUcomboxDataSource(ucb_sca20, sourceList);

                //課稅別
                sourceList = BoStp.OfGetTaxTypeKVPList();
                WfSetUcomboxDataSource(ucb_sca22, sourceList);
                
                //發票聯數
                sourceList = BoStp.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_sca23, sourceList);
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
            vw_stpi100 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_stpi100>();
                    WfSetDocPicture(masterModel.scavali, masterModel.scaconf, "", pbxDoc);
                }
                else
                {
                    WfSetDocPicture("", "", "", pbxDoc);
                }

                if (FormEditMode == YREditType.NA)
                    WfSetControlsReadOnlyRecursion(this, true);
                else
                {
                    //明細先全開,並交由 WfSetDetailDisplayMode處理-事件觸發
                    WfSetControlsReadOnlyRecursion(this, false);    //先全開
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯

                    WfSetControlReadonly(new List<Control> { ute_scacreu, ute_scacreg, udt_scacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_scamodu, ute_scamodg, udt_scamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_scasecu, ute_scasecg }, true);

                    WfSetControlReadonly(new List<Control> { ute_sca05_c, ute_sca21_c, ute_sca24_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_sca25_c, ute_sca26_c, ute_sca27_c }, true);

                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_sca01, true);
                    }

                    SelectNextControl(this.uTab_Header, true, true, true, false);
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
                                columnName == "scb03" ||
                                columnName == "scb04" ||
                                columnName == "scb05" ||
                                columnName == "scb06" ||
                                columnName == "scb07" ||
                                columnName == "scb08"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "scb02")
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

        #region WfIniTabDetailInfo(): 設定明細的資料來源
        protected override Boolean WfIniTabDetailInfo()
        {
            SqlParameter keyParm;
            this.TabDetailList[0].TargetTable = "scb_tb";
            this.TabDetailList[0].ViewTable = "vw_stpi100s";
            keyParm = new SqlParameter("scb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "sca01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            string l_bga01 = "", l_bgc03 = "";
            string l_sca01_new;
            int iChkCnts = 0;
            string errMsg;
            StringBuilder sbSql;
            List<SqlParameter> sqlParms;
            try
            {
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_stpi100
                if (pDr.Table.Prefix.ToLower() == "vw_stpi100")
                {
                    switch (pColName.ToLower())
                    {
                        case "sca01"://客戶編號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bga03", "1"));
                            WfShowPickUtility("p_bga1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                {
                                    l_bga01 = GlobalFn.isNullRet(messageModel.DataRowList[0]["bga01"], "");
                                    //檢查是否有使用分類編碼
                                    sbSql = new StringBuilder();
                                    sbSql.AppendLine("SELECT COUNT(1) FROM bgb_tb");
                                    sbSql.AppendLine("WHERE bgb01=@bgb01 AND bgb05='3'");
                                    sqlParms = new List<SqlParameter>();
                                    sqlParms.Add(new SqlParameter("@bgb01", l_bga01));
                                    iChkCnts = GlobalFn.isNullRet(BoMaster.OfGetFieldValue(sbSql.ToString(), sqlParms.ToArray()), 0);
                                    if (iChkCnts > 0)
                                    {
                                        messageModel = new MessageInfo();
                                        messageModel.IsAutoQuery = true;
                                        messageModel.ParamSearchList = new List<SqlParameter>();
                                        messageModel.ParamSearchList.Add(new SqlParameter("@bgc01", l_bga01));

                                        WfShowPickUtility("p_bgc1", messageModel);
                                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                                        {
                                            if (messageModel.DataRowList.Count > 0)
                                                l_bgc03 = GlobalFn.isNullRet(messageModel.DataRowList[0]["bgc03"], "");
                                            else
                                                l_bgc03 = "";
                                        }
                                        else
                                            break;
                                        //if (messageModel != null && messageModel.DataRowList.Count > 0)
                                        //{
                                        //    l_bgc03 = GlobalFn.isNullRet(messageModel.DataRowList[0]["bgc03"], "");
                                        //}
                                        //else
                                        //    break;
                                    }
                                    if (BoBas.OfGetBga01AutoNo(l_bga01, l_bgc03, out l_sca01_new, out errMsg) == false)
                                    {
                                        WfShowErrorMsg(errMsg);
                                        break;
                                    }
                                    Sca01New = l_sca01_new;
                                    Bga01 = l_bga01;
                                    Bgc03 = l_bgc03;
                                    pDr["sca01"] = l_sca01_new;
                                }
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sca04"://客戶分類
                            WfShowPickUtility("p_sba1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sba01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sca05"://負責業務人員
                            WfShowPickUtility("p_bec", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "sca21"://收款方式
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bef01", "2"));
                            WfShowPickUtility("p_bef1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bef02"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["bef02"];
                            //}
                            break;
                            
                        case "sca24"://取價原則
                            WfShowPickUtility("p_sbb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["sbb01"];
                            //}
                            break;

                        case "sca25"://貨運方式
                            WfShowPickUtility("p_bel1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bel01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["bel01"];
                            //}
                            break;
                            
                        case "sca26"://貨運起點
                            WfShowPickUtility("p_sbg1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbg01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["sbg01"];
                            //}
                            break;

                        case "sca27"://貨運終點
                            WfShowPickUtility("p_sbg1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["sbg01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            //if (messageModel != null && messageModel.DataRowList.Count > 0)
                            //{
                            //    pDr[pColName] = messageModel.DataRowList[0]["sbg01"];
                            //}
                            break;

                        case "sca28"://幣別
                            WfShowPickUtility("p_bek1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bek01"], "");
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
                pDr["scavali"] = "";
                pDr["scaconf"] = "N";
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
                    case "vw_stpi100s":
                        //pDr["scb02"] = WfGetMaxSeq(pDr.Table, "scb02");   //不預帶,由使用者自行輸入
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
            vw_stpi100 masterModel;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpi100>();
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

        #region WfPreInInsertModeCheck 進新增模式前的檢查及清變數
        protected override bool WfPreInInsertModeCheck()
        {
            try
            {
                Sca01New = "";
                Bga01 = "";
                Bgc03 = "";
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfPreDeleteCheck 進主檔刪除前檢查
        /* 1.檢查該客戶是否已存在銷售模組各單據中了
         */
        protected override bool WfPreDeleteCheck(DataRow pDr)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            vw_stpi100 masterModel;
            int iChkCnts;
            try
            {
                masterModel=pDr.ToItem<vw_stpi100>();

                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sea_tb");
                sbSql.AppendLine("WHERE sea03=@sea03");
                sqlParmList = new List<SqlParameter> { new SqlParameter("@sea03",masterModel.sca01)};
                iChkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(),sqlParmList.ToArray()),0);
                if (iChkCnts > 0)
                {
                    WfShowErrorMsg("客戶已存在於報價單中，不可刪除！");
                    return false;
                }

                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sfa_tb");
                sbSql.AppendLine("WHERE sfa03=@sfa03");
                sqlParmList = new List<SqlParameter> { new SqlParameter("@sfa03", masterModel.sca01) };
                iChkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (iChkCnts > 0)
                {
                    WfShowErrorMsg("客戶已存在於訂單中，不可刪除！");
                    return false;
                }

                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sga_tb");
                sbSql.AppendLine("WHERE sga03=@sga03");
                sqlParmList = new List<SqlParameter> { new SqlParameter("@sga03", masterModel.sca01) };
                iChkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (iChkCnts > 0)
                {
                    WfShowErrorMsg("客戶已存在於出貨單中，不可刪除！");
                    return false;
                }

                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sha_tb");
                sbSql.AppendLine("WHERE sha03=@sha03");
                sqlParmList = new List<SqlParameter> { new SqlParameter("@sha03", masterModel.sca01) };
                iChkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (iChkCnts > 0)
                {
                    WfShowErrorMsg("客戶已存在於銷退單中，不可刪除！");
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

        #region WfPreDeleteDetailCheck (): 刪除明細前檢查
        protected override bool WfPreDeleteDetailCheck(int pCurTabDetail, DataRow pDrDetail)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            vw_stpi100 masterModel;
            vw_stpi100s detailModel;
            int iChkCnts;
            try
            {
                if (pDrDetail.RowState == DataRowState.Added)
                    return true;

                masterModel = DrMaster.ToItem<vw_stpi100>();
                detailModel = pDrDetail.ToItem<vw_stpi100s>();

                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sga_tb");
                sbSql.AppendLine("WHERE sga03=@sga03");
                sbSql.AppendLine("AND sga18=@sga18");
                sqlParmList = new List<SqlParameter> { new SqlParameter("@sga03", masterModel.sca01), 
                                                       new SqlParameter("@sga18", detailModel.scb02), 
                                            };
                iChkCnts = GlobalFn.isNullRet(BoStp.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (iChkCnts > 0)
                {
                    WfShowErrorMsg("客戶地址碼已存在於出貨單中，不可刪除！");
                    return false;
                }

                return false;
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
            List<vw_stpi100s> detailList;
            vw_stpi100s detailModel;
            int iChkCnts = 0;
            try
            {
                #region 單頭-pick vw_stpi100
                if (e.Row.Table.Prefix.ToLower() == "vw_stpi100")
                {
                    switch (e.Column.ToLower())
                    {
                        case "sca01":
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (BoStp.OfChkScaPKExists(e.Value.ToString()) == true)
                            {
                                WfShowErrorMsg("客戶編號已存存,請檢核!");
                                return false;
                            }
                            break;

                        case "sca04":
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (BoStp.OfChkSbaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此有效客戶分類,請檢核!");
                                return false;
                            }
                            break;

                        case "sca05"://負責業務
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sca05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此負責業務,請檢核!");
                                return false;
                            }
                            e.Row["sca05_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sca21"://收款方式
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sca21_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBefPKValid("2", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此收款方式,請檢核!");
                                return false;
                            }
                            e.Row["sca21_c"] = BoBas.OfGetBef03("2", GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sca24"://取價原則
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sca24_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbbPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此取價原則,請檢核!");
                                return false;
                            }
                            e.Row["sca24_c"] = BoStp.OfGetSbb02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sca25"://貨運方式
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sca25_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBelPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運方式,請檢核!");
                                return false;
                            }
                            e.Row["sca25_c"] = BoBas.OfGetBel02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sca26"://貨運起點
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sca26_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbgPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運起點,請檢核!");
                                return false;
                            }
                            e.Row["sca26_c"] = BoStp.OfGetSbg02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "sca27"://貨運終點
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["sca27_c"] = "";
                                return true;
                            }
                            if (BoStp.OfChkSbgPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此貨運終點,請檢核!");
                                return false;
                            }
                            e.Row["sca27_c"] = BoStp.OfGetSbg02(GlobalFn.isNullRet(e.Value, ""));
                            break;
                            
                        case "sca28"://慣用幣別
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
                    }
                }
                #endregion

                #region 單身-pick vw_stpi100
                if (e.Row.Table.Prefix.ToLower() == "vw_stpi100s")
                {
                    detailModel = e.Row.ToItem<vw_stpi100s>();
                    detailList = e.Row.Table.ToList<vw_stpi100s>();
                    switch (e.Column.ToLower())
                    {
                        case "scb02":
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            iChkCnts = detailList.Where(x => x.scb02 == detailModel.scb02).Count();
                            if (iChkCnts>1)
                            {
                                WfShowErrorMsg("地址碼已存在!");
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
            vw_stpi100 masterModel = null;
            string msg;
            Control chkControl;
            string chkColName;
            try
            {
                masterModel = DrMaster.ToItem<vw_stpi100>();
                #region 單頭資料檢查
                chkColName = "sca01";
                chkControl = ute_sca01;
                if (GlobalFn.varIsNull(masterModel.sca01))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sca02";
                chkControl = ute_sca02;
                if (GlobalFn.varIsNull(masterModel.sca02))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sca03";
                chkControl = ute_sca03;
                if (GlobalFn.varIsNull(masterModel.sca03))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sca20";//單據發送方式
                chkControl = ucb_sca20;
                if (GlobalFn.varIsNull(masterModel.sca20))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                
                chkColName = "sca21";//收款方式
                chkControl = ute_sca21;
                if (GlobalFn.varIsNull(masterModel.sca21))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "sca22";
                chkControl = ucb_sca22;//課稅別
                if (GlobalFn.varIsNull(masterModel.sca22))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                
                chkColName = "sca23";
                chkControl = ucb_sca23;//發票聯數
                if (GlobalFn.varIsNull(masterModel.sca23))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                
                chkColName = "sca24";
                chkControl = ute_sca24;//慣用價格條件
                if (GlobalFn.varIsNull(masterModel.sca24))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                
                chkColName = "sca28";
                chkControl = ute_sca28;//慣用幣別
                if (GlobalFn.varIsNull(masterModel.sca28))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[1];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
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
            string sca01New, errMsg;
            try
            {
                if (FormEditMode == YREditType.新增 && Sca01New == GlobalFn.isNullRet(DrMaster["sca01"], ""))     //避免搶號,重新取號
                {
                    if (BoBas.OfGetBga01AutoNo(Bga01, Bgc03, out sca01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["sca01"] = sca01New;
                }
                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["scasecu"] = LoginInfo.UserNo;
                        DrMaster["scasecg"] = LoginInfo.GroupNo;
                        DrMaster["scacreu"] = LoginInfo.UserNo;
                        DrMaster["scacreg"] = LoginInfo.DeptNo;
                        DrMaster["scacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["scamodu"] = LoginInfo.UserNo;
                        DrMaster["scamodg"] = LoginInfo.DeptNo;
                        DrMaster["scamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["scbcreu"] = LoginInfo.UserNo;
                            drDetail["scbcreg"] = LoginInfo.DeptNo;
                            drDetail["scbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["scbmodu"] = LoginInfo.UserNo;
                            drDetail["scbmodg"] = LoginInfo.DeptNo;
                            drDetail["scbmodd"] = Now;
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
        //*****************************表單自訂Fuction****************************************

        #region WfConfirm 確認
        private void WfConfirm()
        {
            vw_stpi100 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)       
                    return;

                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;

                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_stpi100>();

                if (masterModel.scavali == "N")
                {
                    WfShowErrorMsg("客戶已失效!");
                    WfRollback();
                    return;
                }

                if (GlobalFn.isNullRet(masterModel.scaconf,"N") != "N")
                {
                    WfShowErrorMsg("客戶非未確認狀態!");
                    WfRollback();
                    return;
                }

                DrMaster["scaconf"] = "Y";
                DrMaster["scavali"] = "Y";
                DrMaster["scamodu"] = LoginInfo.UserNo;
                DrMaster["scamodg"] = LoginInfo.DeptNo;
                DrMaster["scamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_stpi100>();
                WfSetDocPicture(masterModel.scavali, masterModel.scaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfCancelConfirm 取消確認
        private void WfCancelConfirm()
        {
            vw_stpi100 masterModel = null;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)        
                    return;

                if (WfLockMasterRow() == false) //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_stpi100>();

                if (masterModel.scavali == "N")
                {
                    WfShowErrorMsg("客戶已失效!");
                    WfRollback();
                    return;
                }

                if (masterModel.scaconf != "Y")
                {
                    WfShowErrorMsg("客戶非已確認狀態!");
                    WfRollback();
                    return;
                }

                DrMaster["scaconf"] = "N";
                DrMaster["scamodu"] = LoginInfo.UserNo;
                DrMaster["scamodg"] = LoginInfo.DeptNo;
                DrMaster["scamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_stpi100>();
                WfSetDocPicture(masterModel.scavali, masterModel.scaconf, "", pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfInvalid 作廢/作廢還原
        private void WfInvalid()
        {
            vw_stpi100 masterModel = null;
            string msg;
            try
            {
                if (DrMaster == null)
                    return;

                if (WfRetrieveMaster() == false)        //這裡會LOCK資料
                    return;
                WfSetBllTransaction();
                masterModel = DrMaster.ToItem<vw_stpi100>();


                if (masterModel.scavali == "Y")
                    msg = "是否要作廢客戶?";
                else
                    msg = "是否要作廢還原客戶?";

                var result = WfShowConfirmMsg(msg);

                //if (WfShowConfirmMsg(msg) != 1)
                if (result != DialogResult.Yes)
                    return;


                if (masterModel.scavali == "Y" || masterModel.scavali == "W")//走作廢
                {
                    DrMaster["scavali"] = "N";
                }
                else
                {
                    DrMaster["scavali"] = "Y";
                }
                DrMaster["scamodu"] = LoginInfo.UserNo;
                DrMaster["scamodg"] = LoginInfo.DeptNo;
                DrMaster["scamodd"] = Now;

                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_stpi100>();
                WfSetDocPicture(masterModel.scavali, masterModel.scaconf, "", pbxDoc);
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
