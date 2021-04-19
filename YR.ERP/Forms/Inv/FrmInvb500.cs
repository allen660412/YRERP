/* 程式名稱: 盤點清冊產生作業
   系統代號: invb500
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
    public partial class FrmInvb500 : YR.ERP.Base.Forms.FrmBatchBase
    {

        #region Property
        InvBLL BoInv;
        BasBLL BoBas;
        #endregion

        #region 建構子
        public FrmInvb500()
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
            this.StrFormID = "invb500";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            //todo:不處理執行範圍權限,要測試
            //TabMaster.UserColumn = "seasecu";
            //TabMaster.GroupColumn = "seasecg";
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
            BoInv = new InvBLL(BoMaster.OfGetConntion());
            BoBas = new BasBLL(BoMaster.OfGetConntion());
            return;

        }
        #endregion

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {

                //排序
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.儲位"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.料號"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.倉庫"));
                WfSetUcomboxDataSource(ucb_order_by_1, sourceList);
                WfSetUcomboxDataSource(ucb_order_by_2, sourceList);
                WfSetUcomboxDataSource(ucb_order_by_3, sourceList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetMasterRowDefault(DataRow pDr) 設定MasterRow的初始值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["ipa02"] = Today;
                pDr["ipa03"] = Today;
                pDr["order_by_1"] = "1";
                pDr["order_by_2"] = "2";
                pDr["order_by_3"] = "3";
                return true;
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
                WfSetControlsReadOnlyRecursion(this.PnlFillMaster, false);
                WfSetControlReadonly(udt_ipa02, true);
                return true;
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
                MessageInfo messageModel = new MessageInfo();
                switch (pColName.ToLower())
                {

                    case "ipa01"://異動單號
                        messageModel.ParamSearchList = new List<SqlParameter>();
                        messageModel.IsAutoQuery = true;
                        messageModel.ParamSearchList.Add(new SqlParameter("@bab03", "inv"));
                        messageModel.ParamSearchList.Add(new SqlParameter("@bab04", "50"));
                        WfShowPickUtility("p_bab1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            if (messageModel.DataRowList.Count > 0)
                                pDr[pColName] = GlobalFn.isNullRet(messageModel.DataRowList[0]["bab01"], "");
                            else
                                pDr[pColName] = "";
                        }
                        break;
                    case "icc01":       //料號
                        messageModel.StrMultiColumn = "ica01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_ica1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            pDr[pColName] = messageModel.StrMultiRtn;
                        break;
                    case "icc02":       //倉庫
                        messageModel.StrMultiColumn = "icb01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_icb1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            pDr[pColName] = messageModel.StrMultiRtn;
                        break;
                    case "icc03":       //儲位
                        messageModel.StrMultiColumn = "icc03";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_icc5", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                            pDr[pColName] = messageModel.StrMultiRtn;
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

        #region WfItemCheck 控制項離開檢查
        protected override bool WfItemCheck(object sender, ItemCheckInfo e)
        {
            vw_invb500 masterModel = null;
            try
            {
                masterModel = DrMaster.ToItem<vw_invb500>();
                switch (e.Column.ToLower())
                {
                    case "ipa01":
                        if (GlobalFn.isNullRet(e.Value, "") == "")
                            return true;
                        if (BoBas.OfChkBabPKValid(GlobalFn.isNullRet(e.Value, ""), "inv", "50") == false)
                        {
                            WfShowErrorMsg("無此單別,請檢核!");
                            return false;
                        }
                        //e.Row["iga01_c"] = BoBas.OfGetBab02(GlobalFn.isNullRet(e.Value, ""));
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

        #region WfFormCheck() 執行報表前檢查
        protected override bool WfFormCheck()
        {
            vw_invb500 masterModel;
            bab_tb l_bab = null;
            string msg;
            Control chkControl;
            string chkColName;
            int iChkDetailTab;
            try
            {
                
                masterModel = DrMaster.ToItem<vw_invb500>();
                chkColName = "ipa01";       //盤點單號
                chkControl = ute_ipa01;
                if (GlobalFn.varIsNull(masterModel.ipa01))
                {
                    chkControl.Focus();

                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "ipa03";       //盤點單號
                chkControl = udt_ipa03;
                if (GlobalFn.varIsNull(masterModel.ipa03))
                {
                    chkControl.Focus();

                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);
                    return false;
                }

                chkColName = "icc02";       //倉庫
                chkControl = ute_icc02;
                if (GlobalFn.varIsNull(masterModel.icc02))
                {
                    chkControl.Focus();

                    msg = TabMaster.AzaTbList.Where(p => p.aza03 == chkColName).Select(p => p.aza04).FirstOrDefault();
                    msg += "不可為空白";
                    errorProvider.SetError(chkControl, msg);
                    WfShowErrorMsg(msg);                    
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

        #region WfExecute 批次執行開始
        protected override bool WfExecute()
        {
            vw_invb500 invb500Model;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            List<SqlParameter> sqlParmList;
            string strQueryRange, strWhere, strOrderBy;
            DataTable dtMain, dtIpaInsert, dtIpbInsert;
            DataRow drIpa, drIpb;
            StringBuilder sbSql, sbInsert;
            int chkCnts = 0;
            string ipa01New = "", errMsg = "";
            try
            {

                //取得交易物件
                BoMaster.TRAN = BoMaster.OfGetConntion().BeginTransaction(IsolationLevel.ReadUncommitted);
                BoInv.TRAN = BoMaster.TRAN;
                BoBas.TRAN = BoMaster.TRAN;

                invb500Model = DrMaster.ToItem<vw_invb500>();
                #region range 查詢條件
                queryInfoList = new List<QueryInfo>();
                if (!GlobalFn.varIsNull(invb500Model.icc01))
                {
                    queryModel = new QueryInfo();

                    queryModel.TableName = "icc_tb";
                    queryModel.ColumnName = "icc01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["icc01"].DataType.Name;
                    queryModel.Value = invb500Model.icc01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(invb500Model.icc02))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "icc_tb";
                    queryModel.ColumnName = "icc02";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["icc02"].DataType.Name;
                    queryModel.Value = invb500Model.icc02;
                    queryInfoList.Add(queryModel);
                }

                if (!GlobalFn.varIsNull(invb500Model.icc03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "icc_tb";
                    queryModel.ColumnName = "icc03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["icc03"].DataType.Name;
                    queryModel.Value = invb500Model.icc02;
                    queryInfoList.Add(queryModel);
                }

                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                strWhere = strQueryRange;
                #endregion

                var strSecurity = WfGetSecurityString();    //暫時無權限問題,先保留
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得資料
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM icc_tb");
                sbSql.AppendLine("WHERE 1=1");

                #region 加入排序
                strOrderBy = "";
                switch (invb500Model.order_by_1)
                {
                    case "1":
                        strOrderBy = " ORDER BY icc03,";
                        break;
                    case "2":
                        strOrderBy = " ORDER BY icc01,";
                        break;
                    case "3":
                        strOrderBy = " ORDER BY icc02,";
                        break;
                }
                switch (invb500Model.order_by_2)
                {
                    case "1":
                        strOrderBy += "icc03,";
                        break;
                    case "2":
                        strOrderBy += "icc01,";
                        break;
                    case "3":
                        strOrderBy += "icc02,";
                        break;
                }
                switch (invb500Model.order_by_3)
                {
                    case "1":
                        strOrderBy += "icc03";
                        break;
                    case "2":
                        strOrderBy += "icc01";
                        break;
                    case "3":
                        strOrderBy += "icc02";
                        break;
                }
                #endregion

                dtMain = BoMaster.OfGetDataTable(string.Concat(sbSql.ToString(), strWhere, strOrderBy), sqlParmList.ToArray());
                dtMain.TableName = "Master";

                if (dtMain == null || dtMain.Rows.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    BoMaster.TRAN.Rollback();
                    return false;
                }

                #region 寫入盤點清冊
                //新增單頭
                if (BoBas.OfGetAutoNo(invb500Model.ipa01, ModuleType.stp, (DateTime)invb500Model.ipa02, out ipa01New, out errMsg) == false)
                {
                    WfShowErrorMsg(errMsg);
                    return false;
                }

                BoInv.OfCreateDao("ipa_tb", "*", "");
                sbInsert = new StringBuilder();
                sbInsert.AppendLine("SELECT * FROM ipa_tb");
                sbInsert.AppendLine("WHERE 1<>1");
                dtIpaInsert = BoInv.OfGetDataTable(sbInsert.ToString());
                drIpa = dtIpaInsert.NewRow();
                drIpa["ipa01"] = ipa01New;
                drIpa["ipa02"] = invb500Model.ipa02;
                drIpa["ipa03"] = invb500Model.ipa03;
                drIpa["ipa04"] = invb500Model.ipa04;
                drIpa["ipa05"] = "N";
                drIpa["ipa06"] = DBNull.Value;
                drIpa["ipa07"] = "N";
                drIpa["ipa08"] = DBNull.Value;
                drIpa["ipacomp"] = LoginInfo.CompNo;
                drIpa["ipasecu"] = LoginInfo.UserNo;
                drIpa["ipasecg"] = LoginInfo.GroupNo;
                drIpa["ipacreu"] = LoginInfo.UserNo;
                drIpa["ipacreg"] = LoginInfo.DeptNo;
                drIpa["ipacred"] = Now;
                drIpa["ipamodu"] = DBNull.Value;
                drIpa["ipamodg"] = DBNull.Value;
                drIpa["ipamodd"] = DBNull.Value;

                dtIpaInsert.Rows.Add(drIpa);
                if (BoInv.OfUpdate(dtIpaInsert) != 1)
                {
                    errMsg = "新增盤點清冊(ipa_tb)失敗!";
                    BoMaster.TRAN.Rollback();
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                
                BoInv.OfCreateDao("ipb_tb", "*", "");
                sbInsert = new StringBuilder();
                sbInsert.AppendLine("SELECT * FROM ipb_tb");
                sbInsert.AppendLine("WHERE 1<>1");
                dtIpbInsert = BoInv.OfGetDataTable(sbInsert.ToString());
                var i = 0;
                foreach (DataRow drIcc in dtMain.Rows)
                {
                    i++;
                    drIpb = dtIpbInsert.NewRow();
                    var iccModel = drIcc.ToItem<icc_tb>();
                    drIpb["ipb01"] = ipa01New;
                    drIpb["ipb02"] = i;
                    drIpb["ipb03"] = iccModel.icc01;
                    drIpb["ipb04"] = iccModel.icc02;
                    drIpb["ipb05"] = iccModel.icc03;
                    drIpb["ipb06"] = iccModel.icc05;
                    drIpb["ipb07"] = iccModel.icc04;
                    drIpb["ipb30"] = 0;
                    drIpb["ipb31"] = DBNull.Value;
                    drIpb["ipb32"] = DBNull.Value;
                    drIpb["ipb40"] = 0;
                    drIpb["ipb41"] = DBNull.Value;
                    drIpb["ipb42"] = DBNull.Value;
                    drIpb["ipb50"] = 0;
                    drIpb["ipb51"] = DBNull.Value;
                    drIpb["ipb52"] = DBNull.Value;
                    drIpb["ipbcomp"] = LoginInfo.CompNo;
                    drIpb["ipbcreu"] = LoginInfo.UserNo;
                    drIpb["ipbcreg"] = LoginInfo.DeptNo;
                    drIpb["ipbcred"] = Now;
                    drIpb["ipbmodu"] = DBNull.Value;
                    drIpb["ipbmodg"] = DBNull.Value;
                    drIpb["ipbmodd"] = DBNull.Value;
                    dtIpbInsert.Rows.Add(drIpb);
                }
                if (BoInv.OfUpdate(dtIpbInsert) <= 0)
                {
                    errMsg = "新增盤點清冊(ipb_tb)失敗!";
                    BoMaster.TRAN.Rollback();
                    WfShowErrorMsg(errMsg);
                    return false;
                }
                #endregion
                BoMaster.TRAN.Commit();

                #region 檢視產生資料
                if (WfShowConfirmMsg("盤點清冊展開完成，是否要檢視盤點資料?")==DialogResult.Yes)
                {
                    sbSql = new StringBuilder();
                    sbSql.AppendLine(string.Format(" AND ipa01='{0}'", ipa01New));
                    WfShowForm("invb501", false, new object[] { "admi600", this.LoginInfo, sbSql.ToString() });

                }
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                BoMaster.TRAN.Rollback();
                throw ex;
            }
        }
        #endregion

    }
}
