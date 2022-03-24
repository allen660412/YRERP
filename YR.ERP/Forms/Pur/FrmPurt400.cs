/* 程式名稱: 入庫單維護作業
   系統代號: purt400
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
    public partial class FrmPurt400 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        BasBLL BoBas = null;
        PurBLL BoPur = null;
        InvBLL BoInv = null;
        AdmBLL BoAdm = null;

        baa_tb BaaTbModel = null;
        bek_tb BekTbModel = null;      //幣別檔,在display mode載入
        #endregion

        #region 建構子
        public FrmPurt400()
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
            this.StrFormID = "purt400";
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
                TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("pga01", SqlDbType.NVarChar) });
                TabMaster.UserColumn = "pgasecu";
                TabMaster.GroupColumn = "pgasecg";

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
                WfSetUcomboxDataSource(ucb_pga06, sourceList);

                //發票聯數
                sourceList = BoPur.OfGetInvoWayKVPList();
                WfSetUcomboxDataSource(ucb_pga09, sourceList);

                //單據確認
                sourceList = BoPur.OfGetPgaconfKVPList();
                WfSetUcomboxDataSource(ucb_pgaconf, sourceList);
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

            this.TabDetailList[0].TargetTable = "pgb_tb";
            this.TabDetailList[0].ViewTable = "vw_purt400s";
            keyParm = new SqlParameter("pgb01", SqlDbType.NVarChar);
            keyParm.SourceColumn = "pga01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { keyParm };
            return true;
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示 僅控制 新增/修改/NA
        protected override Boolean WfDisplayMode()
        {
            vw_purt400 masterModel = null;
            try
            {
                if (DrMaster != null && DrMaster.RowState != DataRowState.Detached)
                {
                    masterModel = DrMaster.ToItem<vw_purt400>();
                    WfSetDocPicture("", masterModel.pgaconf, masterModel.pgastat, pbxDoc);
                    if (GlobalFn.varIsNull(masterModel.pga10) != true
                            && (FormEditMode == YREditType.新增 || FormEditMode == YREditType.修改))
                    {
                        BekTbModel = BoBas.OfGetBekModel(masterModel.pga10);
                        if (BekTbModel == null)
                        {
                            WfShowErrorMsg(string.Format("未設定此幣別{0},於幣別基本資料檔,請先設定!", masterModel.pga10));
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

                    WfSetControlReadonly(new List<Control> { ute_pgacreu, ute_pgacreg, udt_pgacred }, true);
                    WfSetControlReadonly(new List<Control> { ute_pgamodu, ute_pgamodg, udt_pgamodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_pgasecu, ute_pgasecg }, true);
                    WfSetControlReadonly(new List<Control> { ute_tot_cnts }, true);

                    if (GlobalFn.isNullRet(masterModel.pga01, "") != "")
                    {
                        WfSetControlReadonly(ute_pga01, true);
                        WfSetPga01RelReadonly(GlobalFn.isNullRet(masterModel.pga01, ""));
                    }
                    else
                        WfSetControlReadonly(ute_pga16, true);

                    if (!GlobalFn.varIsNull(masterModel.pga16)) //客戶狀態處理
                        WfSetControlReadonly(ute_pga03, true);

                    WfSetControlReadonly(new List<Control> { ute_pga01_c, ute_pga03_c, ute_pga04_c, ute_pga05_c, ute_pga11_c, ute_pga12_c }, true);
                    WfSetControlReadonly(new List<Control> { ute_pga07, ucx_pga08 }, true);
                    WfSetControlReadonly(new List<Control> { ute_pga13, ute_pga13t, ute_pga13g }, true);
                    WfSetControlReadonly(new List<Control> { ucb_pgaconf, udt_pgacond, ute_pgaconu }, true);
                    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    if (FormEditMode == YREditType.修改)
                    {
                        WfSetControlReadonly(ute_pga01, true);
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
                                columnName == "pgb05" ||
                                columnName == "pgb06" ||
                                columnName == "pgb09" ||
                                columnName == "pgb16"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "pgb02")
                            {
                                if (pDr.RowState == DataRowState.Added)//新增時
                                    WfSetControlReadonly(ugc, false);
                                else    //修改時
                                {
                                    WfSetControlReadonly(ugc, true);
                                }
                                continue;
                            }

                            if (columnName == "pgb03" ||
                                columnName == "pgb11" ||
                                columnName == "pgb12"
                                )
                            {
                                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["pga01"], ""));
                                if (babModel == null)
                                {
                                    WfShowErrorMsg("請先輸入庫單單頭資料!");
                                }
                                if (GlobalFn.isNullRet(babModel.bab08, "") == "Y")    //有來源單據
                                {
                                    if (columnName == "pgb03")//料號
                                        WfSetControlReadonly(ugc, true);    //不可輸入
                                    else
                                        WfSetControlReadonly(ugc, false);
                                }
                                else
                                {
                                    if (columnName == "pgb03")//料號
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

        #region WfSetMasterRowDefault 設定主表新增時的預設值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["pga02"] = Today;
                pDr["pga04"] = LoginInfo.UserNo;
                pDr["pga04_c"] = LoginInfo.UserName;
                pDr["pga05"] = LoginInfo.DeptNo;
                pDr["pga05_c"] = LoginInfo.DeptName;
                pDr["pga07"] = 0;
                pDr["pga08"] = "N";
                pDr["pga10"] = BaaTbModel.baa04;
                pDr["pga13"] = 0;
                pDr["pga13t"] = 0;
                pDr["pga13g"] = 0;
                pDr["pga18"] = 1;
                pDr["pgaconf"] = "N";
                pDr["pgastat"] = "1";
                pDr["pgacomp"] = LoginInfo.CompNo;
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

                decimal RtnValue;
                try
                {

                    //var dMaxSeq = pDr.Table.AsEnumerable()
                    //        .Max(row => row.Field<decimal?>(pSeqName))
                    //        ;

                    //if (dMaxSeq == null)
                    //    dMaxSeq = 0;

                    //RtnValue = Convert.ToDecimal(dMaxSeq) + 1;
                    //return RtnValue;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                switch (pCurTabDetail)
                {
                    case 0:
                        decimal? maxSeqNo = WfGetMaxSeq(pDr.Table, "pgb02");
                        pDr["pgb02"] = maxSeqNo;
                        pDr["pgb05"] = 0;
                        pDr["pgb08"] = 0;
                        pDr["pgb09"] = 0;
                        pDr["pgb10"] = 0;
                        pDr["pgb10t"] = 0;
                        pDr["pgb11"] = DrMaster["pga16"];
                        pDr["pgb13"] = 0;
                        pDr["pgb14"] = 0;
                        pDr["pgb17"] = 0;
                        pDr["pgbcomp"] = LoginInfo.CompNo;


                        var query =
                            from pgb in pDr.Table.AsEnumerable()
                            where pgb.Field<decimal?>("pgb02") == (maxSeqNo - 1)
                            select new
                            {
                                pgb16 =
                                    pgb.Field<string>("pgb16"),
                            };

                        if (query != null && query.Count() > 0)
                        {
                            pDr["pgb16"] = query.First().pgb16;
                        }
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
            vw_purt400 masterModel;
            try
            {
                BaaTbModel = BoBas.OfGetBaaModel();
                masterModel = DrMaster.ToItem<vw_purt400>();
                if (masterModel.pgaconf != "N")
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
            vw_purt400 masterModel = null;
            try
            {
                WfRetrieveMaster();//更新Master資料
                masterModel = DrMaster.ToItem<vw_purt400>();
                if (masterModel.pgaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認,不可作廢!");
                    return false;
                }

                //還需檢查入庫單
                //if (WfChkPgaExists(listMaster.pfa01) == true)
                //{
                //    WfShowMsg("已有退庫單資料!不可取消確認!");
                //    return false;
                //}

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

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            //int iChkCnts = 0;
            //string errMsg;
            //StringBuilder sbSql;
            //List<SqlParameter> sqlParms;
            try
            {
                vw_purt400 masterModel = null;
                vw_purt400s detailModel = null;
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                #region 單頭-pick vw_purt400
                if (pDr.Table.Prefix.ToLower() == "vw_purt400")
                {
                    switch (pColName.ToLower())
                    {
                        case "pga01"://入庫單號
                            messageModel.ParamSearchList = new List<SqlParameter>();
                            messageModel.IsAutoQuery = true;
                            messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "pur"));
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
                        case "pga03"://廠商編號
                            WfShowPickUtility("p_pca1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pca01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "pga04"://入庫人員
                            WfShowPickUtility("p_bec1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bec01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                        case "pga05"://入庫部門
                            WfShowPickUtility("p_beb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["beb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "pga10"://幣別
                            WfShowPickUtility("p_bek1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bek01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "pga11"://付款條件
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

                        case "pga12"://取價條件
                            WfShowPickUtility("p_pbb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pbb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "pga14"://送貨地址
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

                        case "pga15"://帳單地址
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

                        case "pga16"://採購單號
                            WfShowPickUtility("p_pfb1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pfb01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;
                    }
                }
                #endregion

                #region 單身-pick vw_purt400s
                if (pDr.Table.Prefix.ToLower() == "vw_purt400s")
                {
                    masterModel = DrMaster.ToItem<vw_purt400>();
                    detailModel = pDr.ToItem<vw_purt400s>();
                    switch (pColName.ToLower())
                    {
                        case "pgb03"://料號
                            WfShowPickUtility("p_ica1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["ica01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "pgb06"://入庫單位
                            WfShowPickUtility("p_bej1", messageModel);
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bej01"], "");
                                else
                                    pDr[pColName] = "";
                            }
                            break;

                        case "pgb11"://採購單號
                            if (GlobalFn.isNullRet(masterModel.pga03, "") == "")
                                WfShowPickUtility("p_pfb1", messageModel);
                            else
                            {
                                messageModel.ParamSearchList = new List<SqlParameter>();
                                messageModel.ParamSearchList.Add(new SqlParameter("@pfa03", masterModel.pga03));
                                WfShowPickUtility("p_pfb2", messageModel);
                            }
                            if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (messageModel.DataRowList.Count > 0)
                                {
                                    pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pfb01"], "");
                                    pDr["pgb12"] = GlobalFn.isNullRet(messageModel.DataRowList[0]["pfb02"], "");
                                }
                                else
                                {
                                    pDr[pColName] = "";
                                    pDr["pgb12"] = "";
                                }
                            }
                            break;

                        case "pgb16"://倉庫
                            WfShowPickUtility("p_icb1", messageModel);
                            //if (GlobalFn.isNullRet(detailModel.pgb03, "") == "")
                            //    WfShowPickUtility("p_icb1", messageModel);
                            //else
                            //{
                            //    messageModel.ParamSearchList = new List<SqlParameter>();
                            //    messageModel.ParamSearchList.Add(new SqlParameter("@icc01", detailModel.pgb03));
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

        #region WfItemCheck
        //回傳值 true.通過驗證 false.未通過驗證,會把值還原
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            int iChkCnts = 0;
            vw_purt400 masterModel = null;
            vw_purt400s detailModel = null, newDetailModel = null;
            List<vw_purt400s> detailList = null;
            bab_tb babModel = null;
            pfa_tb pfaModel = null;
            UltraGrid uGrid = null;
            int ChkCnts = 0;
            decimal uTaxPrice, taxPrice = 0;
            Result result = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt400>();
                if (e.Column.ToLower() != "pga01" && GlobalFn.isNullRet(DrMaster["pga01"], "") == "")
                {
                    WfShowErrorMsg("請先輸入單別!");
                    return false;
                }
                #region 單頭-pick vw_purt400
                if (e.Row.Table.Prefix.ToLower() == "vw_purt400")
                {
                    switch (e.Column.ToLower())
                    {
                        case "pga01":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "pur", "30") == false)
                            {
                                WfShowErrorMsg("無此單別,請檢核!");
                                return false;
                            }
                            e.Row["pga01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
                            WfSetPga01RelReadonly(GlobalFn.isNullRet(e.Value, ""));
                            if (ute_pga16.ReadOnly != true)
                                WfItemChkForceFocus(ute_pga16);
                            break;
                        case "pga02":
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                                return true;
                            result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(e.Value), BaaModel);
                            if (result.Success == false)
                            {
                                WfShowErrorMsg(result.Message);
                                return false;
                            }
                            break;
                        case "pga03"://廠商
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pga03_c"] = "";
                                e.Row["pga14"] = "";    //送貨地址
                                e.Row["pga15"] = "";    //帳單地址
                                return true;
                            }
                            if (BoPur.OfChkPcaPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此廠商資料,請檢核!");
                                return false;
                            }
                            WfSetPga03Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByPga06();
                            break;

                        case "pga04"://入庫人員
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pga04_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBecPKValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此員工資料,請檢核!");
                                return false;
                            }
                            e.Row["pga04_c"] = BoBas.OfGetBec02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pga05"://入庫部門
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pga05_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBebPkValid(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此部門資料,請檢核!");
                                return false;
                            }
                            e.Row["pga05_c"] = BoBas.OfGetBeb03(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pga06"://課稅別
                            WfSetPga06Relation(GlobalFn.isNullRet(e.Value, ""));
                            WfResetAmtByPga06();
                            break;

                        case "pga10"://幣別
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

                        case "pga11"://付款條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pga11_c"] = "";
                                return true;
                            }
                            if (BoBas.OfChkBefPKValid("1", GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此付款條件,請檢核!");
                                return false;
                            }
                            e.Row["pga11_c"] = BoBas.OfGetBef03("1", GlobalFn.isNullRet(e.Value, ""));
                            break;
                        case "pga12"://取價條件
                            if (GlobalFn.isNullRet(e.Value, "") == "")
                            {
                                e.Row["pga12_c"] = "";
                                return true;
                            }
                            if (BoPur.OfChkPbbPKExists(GlobalFn.isNullRet(e.Value, "")) == false)
                            {
                                WfShowErrorMsg("無此取價條件,請檢核!");
                                return false;
                            }
                            e.Row["pga12_c"] = BoPur.OfGetPbb02(GlobalFn.isNullRet(e.Value, ""));
                            break;

                        case "pga14"://送貨地址
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

                        case "pga15"://帳單地址
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

                        case "pga16"://採購單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfSetControlReadonly(new List<Control> { ute_pga03 }, false);
                                DrMaster["pga12"] = "";
                                DrMaster["pga12_c"] = "";
                                return true;
                            }
                            pfaModel = BoPur.OfGetPfaModel(GlobalFn.isNullRet(e.Value, ""));
                            if (pfaModel == null)
                            {
                                WfShowErrorMsg("無此採購單!");
                                return false;
                            }
                            if (pfaModel.pfaconf != "Y")
                            {
                                WfShowErrorMsg("採購單非確認狀態!");
                                return false;
                            }
                            //檢查與單身是否一致!
                            detailList = TabDetailList[0].DtSource.ToList<vw_purt400s>();
                            ChkCnts = detailList.Where(p => p.pgb11 != pfaModel.pfa01 && GlobalFn.isNullRet(p.pgb11, "") != "")
                                               .Count();
                            if (ChkCnts > 0)
                            {
                                WfShowErrorMsg("單頭與單身的採購單號不一致!");
                                return false;
                            }
                            //這裡會有廠商跟採購單搶壓資料的問題:先以採購單為主
                            DrMaster["pga03"] = pfaModel.pfa03;
                            DrMaster["pga03_c"] = pfaModel.pfa03;
                            WfSetControlReadonly(new List<Control> { ute_pga03 }, true);
                            DrMaster["pga06"] = pfaModel.pfa06;
                            DrMaster["pga07"] = pfaModel.pfa07;
                            DrMaster["pga08"] = pfaModel.pfa08;
                            DrMaster["pga09"] = pfaModel.pfa09;
                            DrMaster["pga10"] = pfaModel.pfa10;
                            DrMaster["pga11"] = pfaModel.pfa11;
                            DrMaster["pga11_c"] = BoBas.OfGetBef03("1", pfaModel.pfa11);
                            DrMaster["pga12"] = pfaModel.pfa12;
                            DrMaster["pga12_c"] = BoPur.OfGetPbb02(pfaModel.pfa12);
                            DrMaster["pga14"] = pfaModel.pfa14;
                            DrMaster["pga15"] = pfaModel.pfa15;
                            DrMaster["pga17"] = pfaModel.pfa16;
                            DrMaster["pga18"] = pfaModel.pfa17;
                            DrMaster["pga20"] = pfaModel.pfa18;
                            DrMaster["pga21"] = pfaModel.pfa19;
                            DrMaster["pga22"] = pfaModel.pfa20;
                            break;

                        case "pga18": //匯率
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

                #region 單身-pick vw_purt400s
                if (e.Row.Table.Prefix.ToLower() == "vw_purt400s")
                {
                    uGrid = sender as UltraGrid;
                    detailModel = e.Row.ToItem<vw_purt400s>();
                    detailList = e.Row.Table.ToList<vw_purt400s>();
                    babModel = BoBas.OfGetBabModel(masterModel.pga01);

                    switch (e.Column.ToLower())
                    {
                        case "pgb02"://項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }
                            if (GlobalFn.isNullRet(e.Value, 0) <= 0)
                            {
                                WfShowErrorMsg("項次應大於0,請檢核!");
                                return false;
                            }
                            iChkCnts = detailList.Where(p => GlobalFn.isNullRet(p.pgb02, 0) == GlobalFn.isNullRet(e.Value, 0))
                                                  .Count();
                            if (iChkCnts > 1)
                            {
                                WfShowErrorMsg("項次已存在,請檢核!");
                                return false;
                            }
                            break;

                        case "pgb03"://料號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                e.Row["pgb04"] = "";//品名
                                e.Row["pgb05"] = 0;//入庫數量
                                e.Row["pgb06"] = "";//入庫單位
                                e.Row["pgb07"] = "";//庫存單位
                                e.Row["pgb08"] = 0;//庫存轉換率
                                e.Row["pgb13"] = 0;//採購轉換率
                                e.Row["pgb14"] = 0;//採購數量
                                e.Row["pgb15"] = "";//採購單位
                                e.Row["pgb18"] = 0;//庫存數量
                                return true;
                            }
                            if (BoInv.OfChkIcaPKValid(e.Value.ToString()) == false)
                            {
                                WfShowErrorMsg("無此料號!");
                                return false;
                            }

                            if (WfSetPgb03Relation(GlobalFn.isNullRet(e.Value, ""), e.Row) == false)
                                return false;

                            if (!GlobalFn.varIsNull(masterModel.pga12))
                            {
                                newDetailModel = e.Row.ToItem<vw_purt400s>();
                                result = BoPur.OfGetPrice(masterModel.pga12, masterModel.pga03, e.Value.ToString(), newDetailModel.pgb06,
                                    masterModel.pga02, masterModel.pga10, "3", newDetailModel.pgb05,
                                    masterModel.pga08, masterModel.pga07, masterModel.pga18, out uTaxPrice, out taxPrice);
                                if (result.Success == true)
                                {
                                    if (masterModel.pga08 == "Y")
                                        e.Row["pgb09"] = taxPrice;
                                    else
                                        e.Row["pgb09"] = uTaxPrice;
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
                        case "pgb05"://入庫數量
                            if (GlobalFn.varIsNull(detailModel.pgb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["pgb03"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(detailModel.pgb06))
                            {
                                WfShowErrorMsg("請先輸入入庫單位!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["pgb06"]);
                                return false;
                            }

                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("請輸入入庫數量!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["pgb05"]);
                                return false;
                            }
                            detailModel.pgb05 = BoBas.OfGetUnitRoundQty(detailModel.pgb06, detailModel.pgb05);    //先轉換數量(四捨伍入)
                            e.Row[e.Column] = detailModel.pgb05;
                            if (WfChkPgb05(e.Row, detailModel) == false)
                                return false;
                            e.Row["pgb14"] = BoBas.OfGetUnitRoundQty(detailModel.pgb06, detailModel.pgb05 * detailModel.pgb13); //轉換採購數量(並四拾伍入)
                            e.Row["pgb18"] = BoBas.OfGetUnitRoundQty(detailModel.pgb07, detailModel.pgb05 * detailModel.pgb08); //轉換庫存數量(並四拾伍入)
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;
                        case "pgb06"://入庫單位
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                return true;
                            }

                            if (GlobalFn.varIsNull(detailModel.pgb03))
                            {
                                WfShowErrorMsg("請先輸入料號!");
                                WfItemChkForceFocus(uGrid, uGrid.ActiveCell.Row.Cells["pgb03"]);
                                return false;
                            }
                            if (WfChkPgb06(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;

                            if (WfSetPgb06Relation(e.Row, e.Value.ToString(), babModel.bab08) == false)
                                return false;
                            break;

                        case "pgb09":   //單價
                            if (GlobalFn.varIsNull(e.Value))
                                return true;
                            if (detailModel.pgb09 < 0)
                            {
                                WfShowErrorMsg("單價不可小於0!");
                                return false;
                            }
                            //依幣別檔設定單價小數
                            e.Row[e.Column] = GlobalFn.Round(detailModel.pgb09, BekTbModel.bek03);
                            WfSetDetailAmt(e.Row);
                            WfSetTotalAmt();
                            break;

                        case "pgb11"://採購單號
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("採購單號不可為空白!");
                                return false;
                            }

                            if (WfChkPgb11(e.Value.ToString()) == false)
                                return false;

                            if (!GlobalFn.varIsNull(detailModel.pgb12))
                            {
                                if (WfChkPgb12(GlobalFn.isNullRet(detailModel.pgb11, ""), GlobalFn.isNullRet(detailModel.pgb12, 0)) == false)
                                    return false;

                                if (WfSetPgb12Relation(e.Row, GlobalFn.isNullRet(detailModel.pgb11, ""), GlobalFn.isNullRet(detailModel.pgb12, 0)) == false)
                                    return false;
                            }
                            WfSetTotalAmt();
                            break;

                        case "pgb12"://採購項次
                            if (GlobalFn.varIsNull(e.Value))
                            {
                                WfShowErrorMsg("採購項次不可為空白!");
                                return false;
                            }

                            if (WfChkPgb12(GlobalFn.isNullRet(detailModel.pgb11, ""), GlobalFn.isNullRet(detailModel.pgb12, 0)) == false)
                                return false;
                            WfSetPgb12Relation(e.Row, GlobalFn.isNullRet(detailModel.pgb11, ""), GlobalFn.isNullRet(detailModel.pgb12, 0));
                            WfSetTotalAmt();
                            break;

                        case "pgb16"://倉庫
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
                    case "vw_purt400s":
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
            vw_purt400 masterModel = null;
            vw_purt400s detailModel = null;
            bab_tb babModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            Result result;
            try
            {

                masterModel = DrMaster.ToItem<vw_purt400>();
                if (!GlobalFn.varIsNull(masterModel.pga01))
                    babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(masterModel.pga01, ""));
                #region 單頭資料檢查
                chkColName = "pga01";       //入庫單號
                chkControl = ute_pga01;
                if (GlobalFn.varIsNull(masterModel.pga01))
                {
                    this.uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pga02";       //入庫日期
                chkControl = udt_pga02;
                if (GlobalFn.varIsNull(masterModel.pga02))
                {
                    this.uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.pga02), BaaModel);
                if (result.Success == false)
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    errorProvider.SetError(chkControl, result.Message);
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                chkColName = "pga03";       //廠商編號
                chkControl = ute_pga03;
                if (GlobalFn.varIsNull(masterModel.pga03))
                {
                    this.uTab_Header.SelectedTab = uTab_Header.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pga04";       //入庫人員
                chkControl = ute_pga04;
                if (GlobalFn.varIsNull(masterModel.pga04))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pga05";       //入庫部門
                chkControl = ute_pga05;
                if (GlobalFn.varIsNull(masterModel.pga05))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pga06";       //課稅別
                chkControl = ucb_pga06;
                if (GlobalFn.varIsNull(masterModel.pga06))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pga10";       //幣別
                chkControl = ute_pga10;
                if (GlobalFn.varIsNull(masterModel.pga10))
                {
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "pga12";
                chkControl = ute_pga12;
                if (GlobalFn.varIsNull(masterModel.pga12) && babModel.bab08 != "Y")//無來源單據取價條件要輸入
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

                    detailModel = drTemp.ToItem<vw_purt400s>();
                    chkColName = "pgb02";   //項次
                    if (GlobalFn.varIsNull(detailModel.pgb02))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "pgb11";   //採購單號
                    if (GlobalFn.varIsNull(detailModel.pgb11) && babModel.bab08 == "Y")  //有來源單據採購單號要輸入
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "pgb12";   //採購項次
                    if (GlobalFn.varIsNull(detailModel.pgb12) && babModel.bab08 == "Y")  //有來源單據採購單號要輸入
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "pgb03";   //料號
                    if (GlobalFn.varIsNull(detailModel.pgb03))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "pgb05";   //入庫數量
                    #region pfb05 採購數量
                    if (GlobalFn.varIsNull(detailModel.pgb05) || detailModel.pgb05 <= 0)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白或小於0!";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    if (WfChkPgb05(drTemp, detailModel) == false)
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    chkColName = "pgb06";   //入庫單位
                    if (GlobalFn.varIsNull(detailModel.pgb06))
                    {
                        this.uTab_Detail.SelectedTab = uTab_Detail.Tabs[iChkDetailTab];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }

                    chkColName = "pgb16";   //倉庫別
                    if (GlobalFn.varIsNull(detailModel.pgb16))
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
            string pga01New, errMsg;
            vw_purt400 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt400>();
                if (FormEditMode == YREditType.新增)
                {
                    pga01New = "";
                    errMsg = "";
                    if (BoBas.OfGetAutoNo(masterModel.pga01, ModuleType.pur, (DateTime)masterModel.pga02, out pga01New, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        return false;
                    }
                    DrMaster["pga01"] = pga01New;
                }

                //填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["pgasecu"] = LoginInfo.UserNo;
                        DrMaster["pgasecg"] = LoginInfo.GroupNo;
                        DrMaster["pgacreu"] = LoginInfo.UserNo;
                        DrMaster["pgacreg"] = LoginInfo.DeptNo;
                        DrMaster["pgacred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["pgamodu"] = LoginInfo.UserNo;
                        DrMaster["pgamodg"] = LoginInfo.DeptNo;
                        DrMaster["pgamodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["pgbcreu"] = LoginInfo.UserNo;
                            drDetail["pgbcreg"] = LoginInfo.DeptNo;
                            drDetail["pgbcred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["pgbmodu"] = LoginInfo.UserNo;
                            drDetail["pgbmodg"] = LoginInfo.DeptNo;
                            drDetail["pgbmodd"] = Now;
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


                bt = new ButtonTool("Purr400");
                adoModel = BoAdm.OfGetAdoModel("Purr400");
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

                    case "Purr400":
                        vw_purr400 purr400Model;
                        vw_purt400 purt400Model;
                        if (DrMaster == null)
                        {
                            WfShowBottomStatusMsg("未指定任何資料!");
                            return;
                        }
                        purt400Model = DrMaster.ToItem<vw_purt400>();
                        purr400Model = new vw_purr400();
                        purr400Model.pga01 = purt400Model.pga01;
                        purr400Model.pga03 = "";
                        purr400Model.jump_yn = "N";
                        purr400Model.order_by = "1";

                        FrmPurr400 rpt = new FrmPurr400(this.LoginInfo, purr400Model, true, true);
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
            List<SqlParameter> sqlParmsList;
            vw_purt400s detailModel = null;
            int iChkCnts;
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
                    detailModel = dr.ToItem<vw_purt400s>();
                    if (BoInv.OfChkIccPKExists(detailModel.pgb03, detailModel.pgb16) == false)
                    {
                        if (dtIcc.Rows.Count > 0)
                        {
                            var drIccs = dtIcc.Select(string.Format("icc01='{0}' AND icc02='{1}'", detailModel.pgb03, detailModel.pgb16));
                            if (drIccs != null && drIccs.Length > 0)
                                continue;
                        }
                        drIcc = dtIcc.NewRow();
                        drIcc["icc01"] = detailModel.pgb03;  //料號
                        drIcc["icc02"] = detailModel.pgb16;
                        drIcc["icc03"] = "";
                        drIcc["icc04"] = detailModel.pgb07;
                        drIcc["icc05"] = 0;
                        drIcc["icccomp"] = detailModel.pgbcomp;
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
        #region WfSetPga01RelReadonly 依單別設定相關控制項 readonly
        private void WfSetPga01RelReadonly(string pPga01)
        {
            bab_tb babModel = null;
            string bab01;
            try
            {
                WfSetControlReadonly(new List<Control> { ute_pga01 }, true);
                bab01 = pPga01.Substring(0, GlobalFn.isNullRet(BaaTbModel.baa06, 0));
                babModel = BoBas.OfGetBabModel(bab01);
                if (babModel.bab08 == "Y")   //有來源單據
                {
                    WfSetControlReadonly(new List<Control> { ute_pga12 }, true);
                    WfSetControlReadonly(new List<Control> { ute_pga16 }, false);
                }
                else
                {
                    WfSetControlReadonly(new List<Control> { ute_pga12 }, false);
                    WfSetControlReadonly(new List<Control> { ute_pga16 }, true);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPga03Relation 設定廠商相關聯
        private void WfSetPga03Relation(string pPga03)
        {
            pca_tb pcaModel;
            bab_tb babModel = null;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["pga01"], ""));
                pcaModel = BoPur.OfGetPcaModel(pPga03);
                if (pcaModel == null)
                    return;

                DrMaster["pga03_c"] = pcaModel.pca03;
                DrMaster["pga06"] = pcaModel.pca22;    //課稅別
                WfSetPga06Relation(pcaModel.pca22);
                DrMaster["pga09"] = pcaModel.pca23;    //發票聯數
                DrMaster["pga11"] = pcaModel.pca21;    //付款條件
                DrMaster["pga11_c"] = BoBas.OfGetBef03("1", pcaModel.pca21);
                DrMaster["pga14"] = pcaModel.pca35;    //送貨地址
                DrMaster["pga15"] = pcaModel.pca36;    //帳單地址

                if (babModel.bab08 != "Y")   //無前置單據則取價條件帶入
                {
                    DrMaster["pga12"] = pcaModel.pca24;    //取價條件
                    DrMaster["pga12_c"] = BoPur.OfGetPbb02(pcaModel.pca24);
                }
                else
                {
                    DrMaster["pga12"] = "";
                    DrMaster["pga12_c"] = "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPga06Relation 設定稅別關聯
        private void WfSetPga06Relation(string pPga06)
        {
            try
            {
                if (pPga06 == "1")
                {
                    DrMaster["pga07"] = BaaTbModel.baa05;
                    DrMaster["pga08"] = "Y";
                }
                else if (pPga06 == "2")
                {
                    DrMaster["pga07"] = BaaTbModel.baa05;
                    DrMaster["pga08"] = "N";
                }
                else
                {
                    DrMaster["pga07"] = 0;
                    DrMaster["pga08"] = "N";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPgb03Relation 設定料號關聯
        private bool WfSetPgb03Relation(string pPgb03, DataRow pDr)
        {
            ica_tb icaModel;
            decimal peb08;
            try
            {
                icaModel = BoInv.OfGetIcaModel(pPgb03);
                peb08 = 0;
                if (icaModel == null)
                {
                    pDr["pgb04"] = "";//品名
                    pDr["pgb05"] = 0;//入庫數量
                    pDr["pgb06"] = "";//入庫單位
                    pDr["pgb07"] = "";//庫存單位
                    pDr["pgb08"] = 0;//庫存轉換率
                    pDr["pgb18"] = 0;//庫存數量
                }
                else
                {
                    if (BoInv.OfGetUnitCovert(pPgb03, icaModel.ica08, icaModel.ica07, out peb08) == false)
                    {
                        WfShowErrorMsg("未設定料號轉換,請先設定!");
                        return false;
                    }
                    pDr["pgb04"] = icaModel.ica02;//品名
                    pDr["pgb05"] = 0;//入庫數量
                    pDr["pgb06"] = icaModel.ica08;//入庫單位
                    pDr["pgb07"] = icaModel.ica07;//庫存單位
                    pDr["pgb08"] = peb08;
                    pDr["pgb18"] = 0;//庫存數量
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetPgb06Relation 設定入庫單位關聯
        //檢查前要先確認料號是否已輸入
        private bool WfSetPgb06Relation(DataRow pDr, string pPgb06, string pBab08)
        {
            vw_purt400s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_purt400s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pPgb06, "")) == false)
                {
                    WfShowErrorMsg("無此入庫單位!請確認");
                    return false;
                }
                //取得是否有入庫對庫存的轉換率
                dConvert = 0;
                if (BoInv.OfGetUnitCovert(detailModel.pgb03, pPgb06, detailModel.pgb07, out dConvert) == true)
                {
                    pDr["pgb08"] = dConvert;
                    pDr["pgb18"] = BoBas.OfGetUnitRoundQty(detailModel.pgb07, detailModel.pgb05 * dConvert); //轉換庫存數量(並四拾伍入)
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有入庫對採購的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.pgb03, pPgb06, detailModel.pgb06, out dConvert) == true)
                    {
                        pDr["pgb13"] = dConvert;
                        pDr["pgb14"] = BoBas.OfGetUnitRoundQty(detailModel.pgb07, detailModel.pgb05 * dConvert); //轉換庫存數量(並四拾伍入)
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

        #region WfSetPgb12Relation 設定採購單明細資料關聯
        private bool WfSetPgb12Relation(DataRow pdr, string pPgb11, int pPgb12)
        {
            pfb_tb pfbModel;
            decimal pgb18;
            try
            {
                pfbModel = BoPur.OfGetPfbModel(pPgb11, pPgb12);
                if (pfbModel == null)
                    return false;
                pgb18 = BoBas.OfGetUnitRoundQty(pfbModel.pfb07, pfbModel.pfb05 * pfbModel.pfb08); //轉換庫存數量(並四拾伍入) 先取,避免錯誤

                pdr["pgb03"] = pfbModel.pfb03;//料號
                pdr["pgb04"] = pfbModel.pfb04;//品名
                pdr["pgb05"] = pfbModel.pfb05;//入庫數量
                pdr["pgb06"] = pfbModel.pfb06;//入庫單位
                pdr["pgb07"] = pfbModel.pfb07;//庫存單位
                pdr["pgb08"] = pfbModel.pfb08;//庫存轉換率
                pdr["pgb09"] = pfbModel.pfb08;//單價
                pdr["pgb10"] = pfbModel.pfb09;//未稅
                pdr["pgb10t"] = pfbModel.pfb10t;//含稅

                pdr["pgb13"] = 1;          //入庫對採購轉換率
                pdr["pgb14"] = pfbModel.pfb05;//轉換採購數量
                pdr["pgb15"] = pfbModel.pfb06;//原採購單位
                pdr["pgb16"] = pfbModel.pfb16;//倉庫別

                pdr["pgb18"] = pgb18; //轉換庫存數量(並四拾伍入)
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkPgb05 數量檢查
        private bool WfChkPgb05(DataRow pdr, vw_purt400s pListDetail)
        {
            List<vw_purt400s> detailList = null;
            pfb_tb pfbModel = null;
            bab_tb babModel = null;
            string errMsg;
            decimal docThisQty = 0;         //本項次的轉換採購數量
            decimal docOtherQtyTotal = 0;   //本單據其他項次的採購數量加總
            decimal otherQtyTotal = 0;      //其他單據的數量加總
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                detailList = pdr.Table.ToList<vw_purt400s>();
                if (GlobalFn.varIsNull(pListDetail.pgb02))
                {
                    errMsg = "請先輸入項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.pgb03))
                {
                    errMsg = "請先輸入料號!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (GlobalFn.varIsNull(pListDetail.pgb06))
                {
                    errMsg = "請先輸入入庫單位!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                if (pListDetail.pgb05 <= 0)
                {
                    errMsg = "入庫數量應大於0!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["pga01"], ""));
                if (babModel == null)
                {
                    errMsg = "Get bab_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (babModel.bab08 != "Y") //無來源單據,以下不檢查!
                    return true;

                pfbModel = BoPur.OfGetPfbModel(pListDetail.pgb11, Convert.ToInt16(pListDetail.pgb12));
                if (babModel == null)
                {
                    errMsg = "Get peb_tb Error!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                //取得本身單據中的所有數量轉換入庫單位後的總和!
                //先取本身這筆
                docThisQty = BoBas.OfGetUnitRoundQty(pListDetail.pgb06, pListDetail.pgb05 * pListDetail.pgb13);
                //再取其他筆加總
                docOtherQtyTotal = detailList.Where(p => p.pgb02 != pListDetail.pgb02)
                                   .Where(p => p.pgb11 == pListDetail.pgb11 && p.pgb12 == pListDetail.pgb12)
                                   .Sum(p => p.pgb14);
                //取得其他單據上的加總
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT SUM(pgb14) FROM pga_tb");
                sbSql.AppendLine("  INNER JOIN pgb_tb ON pga01=pgb01");
                sbSql.AppendLine("WHERE pgaconf<>'X'");
                sbSql.AppendLine("  AND pga01 <> @pga01");
                sbSql.AppendLine("  AND pgb11 = @pgb11 AND pgb12 = @pgb12");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@pga01", GlobalFn.isNullRet(DrMaster["pga01"], "")));
                sqlParmList.Add(new SqlParameter("@pgb11", pListDetail.pgb11));
                sqlParmList.Add(new SqlParameter("@pgb12", pListDetail.pgb12));
                otherQtyTotal = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (pfbModel.pfb05 < (docThisQty + docOtherQtyTotal + otherQtyTotal))
                {
                    errMsg = string.Format("項次{0}採購單最大可輸入數量為 {1}",
                                            pListDetail.pgb02.ToString(),
                                            (pfbModel.pfb05 - docOtherQtyTotal - otherQtyTotal).ToString()
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

        #region WfChkPgb06 檢查入庫單位
        //檢查前要先確認料號是否已輸入
        private bool WfChkPgb06(DataRow pDr, string pPgb06, string pBab08)
        {
            vw_purt400s detailModel;
            decimal dConvert;
            try
            {
                detailModel = pDr.ToItem<vw_purt400s>();
                if (BoBas.OfChkBejPkValid(GlobalFn.isNullRet(pPgb06, "")) == false)
                {
                    WfShowErrorMsg("無此入庫單位!請確認");
                    return false;
                }
                //檢查是否有入庫對庫存的轉換率
                if (BoInv.OfGetUnitCovert(detailModel.pgb03, pPgb06, detailModel.pgb07, out dConvert) == false)
                {
                    WfShowErrorMsg("未設定入庫單位對庫存單位的轉換率,請先設定!");
                    return false;
                }

                dConvert = 0;
                if (pBab08 == "Y")//有來源單據時檢查是否有入庫對採購的轉換率
                {
                    if (BoInv.OfGetUnitCovert(detailModel.pgb03, pPgb06, detailModel.pgb06, out dConvert) == false)
                    {
                        WfShowErrorMsg("未設定入庫單位對採購單位的轉換率,請先設定!");
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

        #region WfChkPgb11 採購單檢查
        private bool WfChkPgb11(string pPgb11)
        {
            string errMsg;
            pfa_tb pfaModel;
            string pga16;
            try
            {
                pfaModel = BoPur.OfGetPfaModel(pPgb11);
                if (pfaModel == null)
                {
                    errMsg = "查無此採購單!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (pfaModel.pfaconf != "Y")
                {
                    errMsg = "採購單非確認狀態!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if (pfaModel.pfastat == "9")
                {
                    errMsg = "採購單已結案!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                pga16 = GlobalFn.isNullRet(DrMaster["pga16"], "");
                if (pga16 != "" && pga16 != pPgb11)
                {
                    errMsg = "採購單號與單頭不同!";
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

        #region WfChkPgb12 採購項次檢查
        private bool WfChkPgb12(string pPgb11, int pPgb12)
        {
            string errMsg;
            pfb_tb pfbModel;
            try
            {
                pfbModel = BoPur.OfGetPfbModel(pPgb11, pPgb12);
                if (pfbModel == null)
                {
                    errMsg = "查無此採購項次!";
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                if ((pfbModel.pfb05 - pfbModel.pfb17) <= 0)
                {
                    errMsg = string.Format("無可用入庫數量!(可用入庫數量為{0})", GlobalFn.isNullRet(pfbModel.pfb05 - pfbModel.pfb17, 0));
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
            pga_tb pgaModel = null;
            pgb_tb pgbModel = null;

            string errMsg;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmsList;
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

                pgaModel = DrMaster.ToItem<pga_tb>();

                if (WfConfirmChk() == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }
                //檢查後更改單頭狀態,讓後面可以直接使用
                pgaModel.pgaconf = "Y";
                pgaModel.pgacond = Today;
                pgaModel.pgaconu = LoginInfo.UserNo;

                //更新採購單入庫量
                if (WfUpdPfb17(true) == false)
                {
                    WfRollback();
                    DrMaster.RejectChanges();
                    return;
                }

                if (WfUpdPfbstatByConfirm() == false)
                {
                    WfRollback();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    pgbModel = dr.ToItem<pgb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("1", pgbModel.pgb03, pgbModel.pgb16, pgbModel.pgb18, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                    //更新料號檔的最近採購單價
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("UPDATE ica_tb");
                    sbSql.AppendLine("SET ica18=@ica18");
                    sbSql.AppendLine("WHERE ica01=@ica01");
                    sqlParmsList = new List<SqlParameter>();
                    sqlParmsList.Add(new SqlParameter("ica01", pgbModel.pgb03));
                    if (pgbModel.pgb05 == 0)
                        sqlParmsList.Add(new SqlParameter("ica18", 0));
                    else
                    {
                        sqlParmsList.Add(new SqlParameter("ica18", GlobalFn.Round(pgbModel.pgb10t / pgbModel.pgb05, 0)));
                    }
                    iChkCnts = BoInv.OfExecuteNonquery(sbSql.ToString(), sqlParmsList);
                    if (iChkCnts <= 0)
                    {
                        WfShowErrorMsg("更新料號檔最近採購單價(ica_tb)失敗!");
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }

                    //更新庫存交易歷史檔
                    if (BoInv.OfStockPost("purt400", pgaModel, pgbModel, LoginInfo, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        DrMaster.RejectChanges();
                        WfRollback();
                        return;
                    }
                }

                DrMaster["pgaconf"] = "Y";
                DrMaster["pgacond"] = Today;
                DrMaster["pgaconu"] = LoginInfo.UserNo;
                DrMaster["pgamodu"] = LoginInfo.UserNo;
                DrMaster["pgamodg"] = LoginInfo.DeptNo;
                DrMaster["pgamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                pgaModel = DrMaster.ToItem<pga_tb>();
                WfSetDocPicture("", pgaModel.pgaconf, pgaModel.pgastat, pbxDoc);
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
            vw_purt400 masterModel = null;
            vw_purt400s detailModel = null;
            List<vw_purt400s> detailList = null;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt400>();
                if (masterModel.pgaconf != "N")
                {
                    WfShowErrorMsg("單據非未確認狀態!");
                    WfRollback();
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.pga02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                detailList = TabDetailList[0].DtSource.ToList<vw_purt400s>();
                foreach (DataRow drTemp in TabDetailList[0].DtSource.Rows)
                {
                    detailModel = drTemp.ToItem<vw_purt400s>();
                    if (WfChkPgb05(drTemp, detailModel) == false)
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
            pga_tb pgaModel = null;
            pgb_tb pgbModel = null;
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
                pgaModel = DrMaster.ToItem<pga_tb>();

                if (WfCancelConfirmChk() == false)
                {
                    WfRollback();
                    return;
                }

                if (WfUpdPfb17(false) == false)
                {
                    WfRollback();
                    return;
                }

                if (WfUpdPfbstatByCancleConfirm() == false)
                {
                    WfRollback();
                    return;
                }

                foreach (DataRow dr in TabDetailList[0].DtSource.Rows)
                {
                    pgbModel = dr.ToItem<pgb_tb>();
                    //更新料件庫存量 icc_tb
                    if (BoInv.OfUpdIcc05("2", pgbModel.pgb03, pgbModel.pgb16, pgbModel.pgb18, out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                    //更新庫存交易歷史檔
                    if (BoInv.OfDelIna(pgbModel.pgb01, pgbModel.pgb02, "1", out errMsg) == false)
                    {
                        WfShowErrorMsg(errMsg);
                        WfRollback();
                        return;
                    }
                }

                DrMaster["pgaconf"] = "N";
                DrMaster["pgacond"] = DBNull.Value;
                DrMaster["pgaconu"] = "";
                DrMaster["pgamodu"] = LoginInfo.UserNo;
                DrMaster["pgamodg"] = LoginInfo.DeptNo;
                DrMaster["pgamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                pgaModel = DrMaster.ToItem<pga_tb>();
                WfSetDocPicture("", pgaModel.pgaconf, pgaModel.pgastat, pbxDoc);
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
            vw_purt400 masterModel = null;
            List<pgb_tb> pgbList = null;
            decimal icc05;
            Result result;
            try
            {
                masterModel = DrMaster.ToItem<vw_purt400>();
                if (masterModel.pgaconf != "Y")
                {
                    WfShowErrorMsg("單據非已確認狀態!");
                    return false;
                }
                result = BoBas.OfChkBaa02Baa03(Convert.ToDateTime(masterModel.pga02), BaaModel);
                if (result.Success == false)
                {
                    WfShowErrorMsg(result.Message);
                    return false;
                }

                //檢查入庫退還單是否存在
                if (WfChkPhaExists(masterModel.pga01) == true)
                {
                    WfShowErrorMsg("已有入庫退回單資料!不可取消確認!");
                    WfRollback();
                    return false;
                }

                //檢查入庫單的數量是否足夠做庫存還原
                pgbList = TabDetailList[0].DtSource.ToList<pgb_tb>();
                var sumPgbList =                         //依入庫單的料號及倉庫做加總
                        from pgb in pgbList
                        where pgb.pgb18 > 0
                        group pgb by new { pgb.pgb03, pgb.pgb16 } into pgb_sum
                        select new
                        {
                            pgb03 = pgb_sum.Key.pgb03,
                            pgb16 = pgb_sum.Key.pgb16,
                            pgb18 = pgb_sum.Sum(p => p.pgb18)
                        }
                    ;
                foreach (var sumPgb in sumPgbList)
                {
                    icc05 = BoInv.OfGetIcc05(sumPgb.pgb03, sumPgb.pgb16);
                    if (icc05 < sumPgb.pgb18)
                    {
                        WfShowErrorMsg(string.Format("料號{0} 庫存量{1}不足!", sumPgb.pgb03, icc05));
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
            vw_purt400 masterModel = null;
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
                masterModel = DrMaster.ToItem<vw_purt400>();

                if (masterModel.pgaconf == "Y")
                {
                    WfShowErrorMsg("單據已確認狀態!");
                    WfRollback();
                    return;
                }

                if (masterModel.pgaconf == "N")//走作廢
                {

                    DrMaster["pgaconf"] = "X";
                    DrMaster["pgaconu"] = LoginInfo.UserNo;
                }
                else if (masterModel.pgaconf == "X")
                {
                    //檢查採購單是否為有效資料
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT COUNT(1)");
                    sbSql.AppendLine("FROM pfa_tb");
                    sbSql.AppendLine("WHERE exists ");
                    sbSql.AppendLine("      (SELECT 1 FROM pgb_tb WHERE pgb01=@pgb01");
                    sbSql.AppendLine("      AND pgb11=pfa01)");
                    sbSql.AppendLine("AND ISNULL(pfaconf,'')<>'Y'");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@pgb01", GlobalFn.isNullRet(DrMaster["pga01"], "")));
                    iChkCnts = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                    if (iChkCnts > 0)
                    {
                        WfShowErrorMsg("採購單含有未確認資料!不可作廢還原!");
                        WfRollback();
                        return;
                    }
                    DrMaster["pgaconf"] = "N";
                    DrMaster["pgaconu"] = "";
                }

                DrMaster["pgamodu"] = LoginInfo.UserNo;
                DrMaster["pgamodg"] = LoginInfo.DeptNo;
                DrMaster["pgamodd"] = Now;
                BoMaster.OfUpdate(DrMaster.Table);
                WfCommit();
                DrMaster.AcceptChanges();
                WfShowBottomStatusMsg("作業成功!");
                masterModel = DrMaster.ToItem<vw_purt400>();
                WfSetDocPicture("", masterModel.pgaconf, masterModel.pgastat, pbxDoc);
            }
            catch (Exception ex)
            {
                WfRollback();
                DrMaster.RejectChanges();
                throw ex;
            }
        }
        #endregion

        #region WfUpdPfb17 更新採購單已轉入庫數量 確認/取消確認
        private bool WfUpdPfb17(bool pbConfirm)
        {
            bab_tb babModel = null;
            StringBuilder sbSql;
            List<SqlParameter> sqlParms;
            DataTable dtPgbDistinct = null;
            string pgb11;
            decimal pgb12;
            decimal docQty = 0, otherDocQty = 0;
            try
            {
                babModel = BoBas.OfGetBabModel(GlobalFn.isNullRet(DrMaster["pga01"], ""));
                if (babModel == null)
                {
                    WfShowErrorMsg("Get bab_tb Error!");
                    return false;
                }
                if (babModel.bab08 == "Y")   //更新採購數量
                {
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT DISTINCT pgb11,pgb12,SUM(pgb14) pgb14");
                    sbSql.AppendLine("FROM pgb_tb");
                    sbSql.AppendLine("WHERE pgb01=@pgb01");
                    sbSql.AppendLine("  AND ISNULL(pgb11,'')<>''");
                    sbSql.AppendLine("GROUP BY pgb11,pgb12");
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@pgb01", GlobalFn.isNullRet(DrMaster["pga01"], "")));
                    dtPgbDistinct = BoPur.OfGetDataTable(sbSql.ToString(), sqlParms.ToArray());
                    if (dtPgbDistinct == null)
                        return true;
                    foreach (DataRow dr in dtPgbDistinct.Rows)
                    {
                        pgb11 = dr["pgb11"].ToString();
                        pgb12 = Convert.ToDecimal(dr["pgb12"]);
                        docQty = Convert.ToDecimal(dr["pgb14"]);

                        sbSql = new StringBuilder();
                        sbSql.AppendLine("SELECT SUM(pgb14) FROM pga_tb");
                        sbSql.AppendLine("  INNER JOIN pgb_tb ON pga01=pgb01");
                        sbSql.AppendLine("WHERE pgaconf='Y'");
                        sbSql.AppendLine("AND pgb11=@pgb11 AND pgb12=@pgb12");
                        sbSql.AppendLine("AND pga01<>@pga01");


                        sqlParms = new List<SqlParameter>();
                        sqlParms.Add(new SqlParameter("@pgb11", pgb11));
                        sqlParms.Add(new SqlParameter("@pgb12", pgb12));
                        sqlParms.Add(new SqlParameter("@pga01", GlobalFn.isNullRet(DrMaster["pga01"], "")));
                        otherDocQty = 0;
                        otherDocQty = GlobalFn.isNullRet(BoPur.OfGetFieldValue(sbSql.ToString(), sqlParms.ToArray()), 0);

                        sbSql = new StringBuilder();
                        sbSql = sbSql.AppendLine("UPDATE pfb_tb");
                        sbSql = sbSql.AppendLine("SET pfb17=@pfb17");
                        sbSql = sbSql.AppendLine("WHERE pfb01=@pfb01");
                        sbSql = sbSql.AppendLine("AND pfb02=@pfb02");
                        sqlParms = new List<SqlParameter>();
                        sqlParms.Add(new SqlParameter("@pfb01", pgb11));
                        sqlParms.Add(new SqlParameter("@pfb02", pgb12));
                        if (pbConfirm)  //確認的要加本身單據
                            sqlParms.Add(new SqlParameter("@pfb17", docQty + otherDocQty));
                        else
                            sqlParms.Add(new SqlParameter("@pfb17", otherDocQty));

                        if (BoPur.OfExecuteNonquery(sbSql.ToString(), sqlParms.ToArray()) != 1)
                        {
                            WfShowErrorMsg("更新採購單入庫數量失敗!");
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

        #region WfUpdPfbstatByConfirm() 更新請購單狀態 確認
        private bool WfUpdPfbstatByConfirm()
        {
            List<vw_purt400s> detailList;
            int chkCnts = 0;
            string querySql = "", updateSql = "";
            List<SqlParameter> sqlParms;
            try
            {
                detailList = TabDetailList[0].DtSource.ToList<vw_purt400s>();
                //取得請購單單號
                var pgbDistinct = from o in detailList
                                  where !GlobalFn.varIsNull(o.pgb11)
                                  group o by new { o.pgb11 } into pgbTemp
                                  select new
                                  {
                                      pgb11 = pgbTemp.Key.pgb11
                                  }
                               ;

                if (pgbDistinct == null)
                    return true;
                //查詢
                querySql = @"SELECT COUNT(1)
                        FROM pfa_tb
	                        INNER JOIN pfb_tb ON pfa01=pfb01
                        WHERE pfa01=@pfa01
	                        AND pfastat='1'
	                        AND (pfb05-pfb17)>0
                        ";
                //逐筆更新
                updateSql = @"UPDATE dbo.pfa_tb
                            SET pfastat='9'
                            WHERE pfa01=@pfa01
                        ";

                foreach (var pgbModel in pgbDistinct)
                {
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@pfa01", pgbModel.pgb11));
                    chkCnts = GlobalFn.isNullRet(BoPur.OfGetFieldValue(querySql, sqlParms.ToArray()), 0);
                    if (chkCnts > 0)
                        continue;
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@pfa01", pgbModel.pgb11));
                    if (BoPur.OfExecuteNonquery(updateSql, sqlParms.ToArray()) < 1)
                    {
                        WfShowErrorMsg("更新pfastat失敗!");
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

        #region WfUpdPfbstatByCancleConfirm() 更新採購單狀態 取消確認
        private bool WfUpdPfbstatByCancleConfirm()
        {
            List<vw_purt400s> detailList;
            int chkCnts = 0;
            string querySql = "", updateSql = "";
            List<SqlParameter> sqlParms;
            try
            {
                detailList = TabDetailList[0].DtSource.ToList<vw_purt400s>();
                //取得採購單單號
                var pgbDistinct = from o in detailList
                                  where !GlobalFn.varIsNull(o.pgb11)
                                  group o by new { o.pgb11 } into pgbTemp
                                  select new
                                  {
                                      pgb11 = pgbTemp.Key.pgb11
                                  }
                               ;

                if (pgbDistinct == null)
                    return true;
                //查詢
                querySql = @"SELECT COUNT(1)
                        FROM pfa_tb
                        WHERE pfa01=@pfa01
	                        AND pfastat='9'
                        ";
                //逐筆更新
                updateSql = @"UPDATE dbo.pfa_tb
                            SET pfastat='1'
                            WHERE pfa01=@pfa01
                        ";

                foreach (var pgbModel in pgbDistinct)
                {
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@pfa01", pgbModel.pgb11));
                    chkCnts = GlobalFn.isNullRet(BoPur.OfGetFieldValue(querySql, sqlParms.ToArray()), 0);
                    if (chkCnts == 0)
                        continue;
                    sqlParms = new List<SqlParameter>();
                    sqlParms.Add(new SqlParameter("@pfa01", pgbModel.pgb11));
                    if (BoPur.OfExecuteNonquery(updateSql, sqlParms.ToArray()) < 1)
                    {
                        WfShowErrorMsg("更新pfastat失敗!");
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
        private bool WfSetDetailAmt(DataRow drPgb)
        {
            pga_tb pgaModel;
            pgb_tb pgbModel;
            decimal pgb10t = 0, pgb10 = 0;
            try
            {
                pgaModel = DrMaster.ToItem<pga_tb>();
                pgbModel = drPgb.ToItem<pgb_tb>();

                if (pgaModel.pga08 == "Y")//稅內含
                {
                    pgb10t = pgbModel.pgb09 * pgbModel.pgb05;
                    pgb10t = GlobalFn.Round(pgb10t, BekTbModel.bek04);
                    pgb10 = pgb10t / (1 + (pgaModel.pga07 / 100));
                    pgb10 = GlobalFn.Round(pgb10, BekTbModel.bek04);
                }
                else//稅外加
                {
                    pgb10 = pgbModel.pgb09 * pgbModel.pgb05;
                    pgb10 = GlobalFn.Round(pgb10, BekTbModel.bek04);
                    pgb10t = pgb10 * (1 + (pgaModel.pga07 / 100));
                    pgb10t = GlobalFn.Round(pgb10t, BekTbModel.bek04);
                }
                drPgb["pgb10"] = pgb10;
                drPgb["pgb10t"] = pgb10t;

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
            pga_tb pgaModel;
            decimal tot_cnts = 0;
            decimal pga13 = 0, pga13t = 0, pga13g;
            try
            {
                pgaModel = DrMaster.ToItem<pga_tb>();
                if (pgaModel.pga08 == "Y")//稅內含,先處理含稅金額
                {
                    foreach (pgb_tb l_pgb in TabDetailList[0].DtSource.ToList<pgb_tb>())
                    {
                        pga13t += l_pgb.pgb10t;
                        tot_cnts += l_pgb.pgb05;
                    }
                    pga13t = GlobalFn.Round(pga13t, BekTbModel.bek04);
                    pga13 = pga13t / (1 + pgaModel.pga07 / 100);
                    pga13 = GlobalFn.Round(pga13, BekTbModel.bek04);
                    pga13g = pga13t - pga13;

                }
                else//稅外加
                {
                    foreach (pgb_tb l_pgb in TabDetailList[0].DtSource.ToList<pgb_tb>())
                    {
                        pga13 += l_pgb.pgb10;
                        tot_cnts += l_pgb.pgb05;
                    }
                    pga13 = GlobalFn.Round(pga13, BekTbModel.bek04);
                    pga13g = pga13 * (pgaModel.pga07 / 100);
                    pga13g = GlobalFn.Round(pga13g, BekTbModel.bek04);
                    pga13t = pga13 + pga13g;
                }

                DrMaster["pga13"] = pga13;
                DrMaster["pga13t"] = pga13t;
                DrMaster["pga13g"] = pga13g;
                DrMaster["tot_cnts"] = tot_cnts;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkPhaExists 檢查入庫退還單是否存在
        private bool WfChkPhaExists(string pPga01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChkCnts = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM pha_tb");
                sbSql.AppendLine("  INNER JOIN phb_tb ON pha01=phb01");
                sbSql.AppendLine("WHERE");
                sbSql.AppendLine(" phaconf <>'X' ");
                sbSql.AppendLine(" AND phb11=@phb11 ");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@phb11", pPga01));
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

        #region WfResetAmtByPga06 依稅別更新單身及單頭的金額
        private void WfResetAmtByPga06()
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
