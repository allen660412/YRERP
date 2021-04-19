/* 程式名稱: 資料表欄位說明設定作業
   系統代號: admi630
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


namespace YR.ERP.Forms.Adm
{
    public partial class FrmAdmi630 : YR.ERP.Base.Forms.FrmEntryMDBase
    {
        #region Property
        AdmBLL YRAdmBll = null; //YR專用連線
        #endregion

        #region 建構子
        public FrmAdmi630()
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
            this.StrFormID = "admi630";
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
            TabMaster.CanAddMode = false;
            TabMaster.CanCopyMode = false;
            TabMaster.UserColumn = "atbsecu";
            TabMaster.GroupColumn = "atbsecg";
            TabMaster.PKParms = new List<SqlParameter>(new SqlParameter[] { new SqlParameter("atb01", SqlDbType.NVarChar) });
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
            YRAdmBll = new AdmBLL();    //指向Y00 不共用transaction
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
            //YR.ERP.Base.Model.MessageInfo ss = new MessageInfo();

            SqlParameter lParm;
            // 設定 Detail tab1 資料 : 終端印件
            this.TabDetailList[0].TargetTable = "atc_tb";
            this.TabDetailList[0].ViewTable = "vw_admi630s";
            lParm = new SqlParameter("atc01", SqlDbType.NVarChar);
            lParm.SourceColumn = "atb01";
            this.TabDetailList[0].RelationParams = new List<SqlParameter>() { lParm };
            return true;
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
                    WfSetControlsReadOnlyRecursion(this, false);    //先全開    //明細先全開,並交由 WfSetDetailDisplayMode處理
                    WfSetControlReadonly(uGridMaster, true);//主表grid不可編輯
                    WfSetControlReadonly(new List<Control> { ute_atbcreu, ute_atbcreg, udt_atbcred }, true);
                    WfSetControlReadonly(new List<Control> { ute_atbmodu, ute_atbmodg, udt_atbmodd }, true);
                    WfSetControlReadonly(new List<Control> { ute_atbsecu, ute_atbsecg }, true);

                    WfSetControlReadonly(new List<Control> { ute_ado01 }, true);
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
                            if (columnName == "atc03" ||
                                columnName == "atc04"
                                )
                            {
                                WfSetControlReadonly(ugc, false);
                                continue;
                            }

                            if (columnName == "atc02")
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

        #region WfFormCheck() 存檔前檢查
        protected override bool WfFormCheck()
        {
            vw_admi630 masterModel = null;
            vw_admi630s detailModel = null;
            UltraGrid uGrid;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {
                masterModel = DrMaster.ToItem<vw_admi630>();
                #region 單頭資料檢查
                if (GlobalFn.varIsNull(masterModel.atb01))
                {
                    chkControl = ute_ado01;
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == "azp01").Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                if (GlobalFn.varIsNull(masterModel.atb02))
                {
                    chkControl = ute_ado02;
                    this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                    chkControl.Focus();
                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == "azp01").Select(p => p.aza04).FirstOrDefault();
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

                    detailModel = drTemp.ToItem<vw_admi630s>();
                    #region atc02-資料欄位代碼
                    chkColName = "atc02";
                    if (GlobalFn.varIsNull(detailModel.atc02))
                    {
                        this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

                    #region atc03-資料欄位說明
                    chkColName = "atc03";
                    if (GlobalFn.varIsNull(detailModel.atc03))
                    {
                        this.uTab_Master.SelectedTab = uTab_Master.Tabs[0];
                        msg = TabDetailList[iChkDetailTab].AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                        msg += "不可為空白";
                        WfShowErrorMsg(msg);
                        WfFindErrUltraGridCell(uGrid, drTemp, chkColName);
                        return false;
                    }
                    #endregion

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
            {//填入系統資訊
                if (DrMaster.RowState != DataRowState.Unchanged)
                {
                    if (DrMaster.RowState == DataRowState.Added)
                    {
                        DrMaster["atbsecu"] = LoginInfo.UserNo;
                        DrMaster["atbsecg"] = LoginInfo.GroupNo;
                        DrMaster["atbcreu"] = LoginInfo.UserNo;
                        DrMaster["atbcreg"] = LoginInfo.DeptNo;
                        DrMaster["atbcred"] = Now;
                    }
                    else if (DrMaster.RowState == DataRowState.Modified)
                    {
                        DrMaster["atbmodu"] = LoginInfo.UserNo;
                        DrMaster["atbmodg"] = LoginInfo.DeptNo;
                        DrMaster["atbmodd"] = Now;
                    }
                }

                foreach (DataRow drDetail in TabDetailList[0].DtSource.Rows)
                {
                    if (drDetail.RowState != DataRowState.Unchanged)
                    {
                        if (drDetail.RowState == DataRowState.Added)
                        {
                            drDetail["atccreu"] = LoginInfo.UserNo;
                            drDetail["atccreg"] = LoginInfo.DeptNo;
                            drDetail["atccred"] = Now;
                        }
                        else if (drDetail.RowState == DataRowState.Modified)
                        {
                            drDetail["atcmodu"] = LoginInfo.UserNo;
                            drDetail["atcmodg"] = LoginInfo.DeptNo;
                            drDetail["atcmodd"] = Now;
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
            List<ButtonTool> buttonToolList = new List<ButtonTool>();
            ButtonTool bt;
            try
            {
                bt = new ButtonTool("GenAllData");
                bt.SharedProps.Caption = "依資料庫產生所有資料";
                bt.SharedProps.Category = "action";
                buttonToolList.Add(bt);

                bt = new ButtonTool("ReUpdateDeatail");
                bt.SharedProps.Caption = "更新明細資料";
                bt.SharedProps.Category = "action";
                buttonToolList.Add(bt);
                return buttonToolList;
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
            atb_tb atbModel;
            try
            {
                switch (pActionName)
                {
                    case "GenAllData":  //依YR資料庫,重新產生至所有DB
                        if (FormEditMode != YREditType.NA)
                            return;
                        WfGenAllData(YRAdmBll);
                        break;

                    case "ReUpdateDeatail":  //依YR資料庫,重新產生至所有DB
                        if (FormEditMode != YREditType.NA)
                            return;
                        if (DrMaster == null)
                            return;
                        atbModel = DrMaster.ToItem<atb_tb>();
                        WfReUpdateDeatail(YRAdmBll, atbModel.atb01);
                        WfRetrieveDetail(0);
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
        #region 以 YR資料庫來產生所有的資料來源
        private void WfGenAllData(AdmBLL pBoAdm)
        {
            StringBuilder sbSql;
            DataTable dtBase;
            string tableName;
            int chkCnts;
            List<SqlParameter> sqlParmList;
            try
            {
                pBoAdm.TRAN = pBoAdm.OfGetConntion().BeginTransaction();

                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_type='BASE TABLE'");
                dtBase = pBoAdm.OfGetDataTable(sbSql.ToString());
                if (dtBase == null)
                    return;
                foreach (DataRow ldr in dtBase.Rows)
                {
                    tableName = GlobalFn.isNullRet(ldr["table_name"], "");
                    sbSql = new StringBuilder();
                    sbSql.AppendLine("SELECT COUNT(1) FROM atb_tb WHERE atb01=@atb01");
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@atb01", tableName));
                    chkCnts = GlobalFn.isNullRet(pBoAdm.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                    if (chkCnts == 0)
                    {
                        WfGenAtb(pBoAdm, tableName, false);//不更新 atc_tb
                    }
                }
                pBoAdm.TRAN.Commit();
            }
            catch (Exception ex)
            {
                pBoAdm.TRAN.Rollback();
                throw ex;
            }
        }
        #endregion

        #region 更新明細資料-以Y00 及DB SCHEMA 若原本就有資料則不更新
        private void WfReUpdateDeatail(AdmBLL pBoAdm, string pTableName)
        {
            try
            {
                pBoAdm.TRAN = pBoAdm.OfGetConntion().BeginTransaction();
                WfGenAtb(pBoAdm, pTableName, true);

                pBoAdm.TRAN.Commit();
            }
            catch (Exception ex)
            {
                pBoAdm.TRAN.Rollback();
                throw ex;
            }
        }
        #endregion

        #region WfGenAtb 以tablename 產生資料 atb+atc資料
        private void WfGenAtb(AdmBLL pBoAdm, string pTableName, bool reUpdateAtc)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            DataTable dtAtb, dtAtc;
            DataRow drAtb, drAtc;
            List<atc_tb> atcTableSchemaList;//從db schema 截取來源
            List<atc_tb> atcList;//從atc_tb 截取來源
            int chkCnts = 0;
            try
            {

                pBoAdm.OfCreateDao("atb_tb", "*", "atb_tb");
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM atb_tb");
                sbSql.AppendLine("WHERE atb01=@atb01 ");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@atb01", pTableName));
                dtAtb = pBoAdm.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());

                if (dtAtb.Rows.Count == 0)
                {
                    drAtb = dtAtb.NewRow();
                    drAtb["atb01"] = pTableName;
                    drAtb["atb02"] = WfGetTbMSDesc(pBoAdm, pTableName);
                    drAtb["atb03"] = "";
                    drAtb["atbsecu"] = LoginInfo.UserNo;
                    drAtb["atbsecg"] = LoginInfo.GroupNo;
                    drAtb["atbcreu"] = LoginInfo.UserNo;
                    drAtb["atbcreg"] = LoginInfo.DeptNo;
                    drAtb["atbcred"] = Now;
                    dtAtb.Rows.Add(drAtb);
                    pBoAdm.OfUpdate(dtAtb);
                }

                //更新資料表明細
                atcTableSchemaList = WfGetAtcAll(pBoAdm, pTableName);
                pBoAdm.OfCreateDao("atc_tb", "*", "atc_tb");
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM atc_tb");
                sbSql.AppendLine("WHERE atc01=@atc01 ");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@atc01", pTableName));
                dtAtc = pBoAdm.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                atcList = dtAtc.ToList<atc_tb>();
                foreach (atc_tb atcModel in atcTableSchemaList)
                {
                    chkCnts = atcList.Where(p => p.atc02 == atcModel.atc02)
                                   .Count();
                    if (chkCnts > 0)
                    {
                        if (reUpdateAtc == true)
                        {
                            var drAtcs = dtAtc.Select(string.Format("atc02='{0}'", atcModel.atc02));
                            drAtcs[0]["atc03"] = atcModel.atc03;
                            //drAtcs[0]["atc04"] = atcModel.atc04;   //額外功能要保留
                            drAtcs[0]["atcmodu"] = LoginInfo.UserNo;
                            drAtcs[0]["atcmodg"] = LoginInfo.DeptNo;
                            drAtcs[0]["atcmodd"] = Now;
                            continue;
                        }
                        else
                            continue;
                    }
                    else
                    {
                        drAtc = dtAtc.NewRow();
                        drAtc["atc01"] = atcModel.atc01;
                        drAtc["atc02"] = atcModel.atc02;
                        drAtc["atc03"] = atcModel.atc03;
                        drAtc["atc04"] = atcModel.atc04;
                        drAtc["atccreu"] = LoginInfo.UserNo;
                        drAtc["atccreg"] = LoginInfo.DeptNo;
                        drAtc["atccred"] = Now;
                        dtAtc.Rows.Add(drAtc);
                    }
                }
                pBoAdm.OfUpdate(dtAtc);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //原始檔保留
        //private void WfGenAtb(string pTableName)
        //{
        //    atb_tb l_atb;
        //    List<atc_tb> l_atcs;
        //    try
        //    {
        //        YREntities yrContent = new YREntities();
        //        l_atb = new atb_tb();
        //        l_atb.atb01 = pTableName;
        //        l_atb.atb02 = WfGetTbMSDesc(pTableName);
        //        l_atb.atb03 = "";

        //        yrContent.atb_tb.Add(l_atb);
        //        l_atcs = WfGetAtcAll(pTableName);
        //        if (l_atcs != null)
        //            yrContent.atc_tb.AddRange(l_atcs);

        //        yrContent.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion

        #region WfGetAtcAll 取得明細 atc_tb list
        private List<atc_tb> WfGetAtcAll(AdmBLL pBoAdm, string pTableName)
        {
            StringBuilder sbSql;
            DataTable dtBase;
            List<SqlParameter> sqlParmList;
            atc_tb atcModel;
            List<atc_tb> atcList;
            try
            {
                sbSql = new StringBuilder();
                //sbSql.AppendLine("SELECT a.table_name,a.column_name,b.value");
                //sbSql.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS a");
                //sbSql.AppendLine("	LEFT OUTER JOIN     sys.extended_properties b ");
                //sbSql.AppendLine("		ON b.major_id = OBJECT_ID(a.TABLE_SCHEMA+'.'+a.TABLE_NAME) ");
                //sbSql.AppendLine("			AND b.minor_id = a.ORDINAL_POSITION AND b.name = 'MS_Description' ");
                //sbSql.AppendLine("WHERE");
                //sbSql.AppendLine("	a.table_name=@table_name");
                sbSql.AppendLine("SELECT");
                sbSql.AppendLine("a.name [table_name],b.name [column_name],c.value [Description]");
                sbSql.AppendLine("FROM sys.tables a");
                sbSql.AppendLine("INNER JOIN sys.columns b on a.object_id = b.object_id");
                sbSql.AppendLine("LEFT JOIN sys.extended_properties c ON a.object_id = c.major_id AND b.column_id = c.minor_id AND c.name = 'MS_Description'");
                sbSql.AppendLine("WHERE a.name = @table_name");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@table_name", pTableName));
                dtBase = pBoAdm.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                if (dtBase == null) return null;

                atcList = new List<atc_tb>();
                foreach (DataRow ldr in dtBase.Rows)
                {
                    atcModel = new atc_tb();
                    atcModel.atc01 = GlobalFn.isNullRet(ldr["table_name"], "");
                    atcModel.atc02 = GlobalFn.isNullRet(ldr["column_name"], "");
                    atcModel.atc03 = GlobalFn.isNullRet(ldr["Description"], "");
                    atcModel.atc04 = "";
                    atcList.Add(atcModel);
                }
                return atcList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfGetTbMSDesc 以資料庫的說明來產生資料表名稱
        private string WfGetTbMSDesc(AdmBLL pBoAdm, string pTableName)
        {
            string rtnTableName = "";
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT value FROM sys.extended_properties");
                sbSql.AppendLine("WHERE");
                sbSql.AppendLine("  name='MS_Description'");
                sbSql.AppendLine("  and minor_id=0");
                sbSql.AppendLine(string.Format("  and major_id=object_id('dbo.{0}')", pTableName));
                rtnTableName = GlobalFn.isNullRet(pBoAdm.OfGetFieldValue(sbSql.ToString(), null), "");
                return rtnTableName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfGetFieldMSDesc 以資料庫的說明來產生資料欄位名稱
        private string WfGetFieldMSDesc(AdmBLL pBoAdm, string pTableName, string pColumnName)
        {
            string rtnColumnName = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT b.value");
                sbSql.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS a");
                sbSql.AppendLine("  LEFT JOIN sys.extended_properties b ON b.name='MS_Description' AND  b.major_id=object_id(a.TABLE_SCHEMA+'.'+a.TABLE_NAME)");
                sbSql.AppendLine("      AND b.minor_id = a.ORDINAL_POSITION ");
                sbSql.AppendLine("WHERE");
                sbSql.AppendLine("  OBJECTPROPERTY(OBJECT_ID(a.TABLE_SCHEMA+'.'+a.TABLE_NAME), 'IsMsShipped')=0  ");
                sbSql.AppendLine("  AND a.table_name=@table_name");
                sbSql.AppendLine("  AND a.column_name=@column_name");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@table_name", pTableName));
                sqlParmList.Add(new SqlParameter("@column_name", pColumnName));
                rtnColumnName = GlobalFn.isNullRet(pBoAdm.OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnColumnName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
