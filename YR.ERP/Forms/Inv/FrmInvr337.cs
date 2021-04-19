using Stimulsoft.Report.Components;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.DAL.YRModel;
using YR.Util;
using YR.ERP.Base.Forms;
using YR.ERP.DAL.YRModel.Reports.Inv.Invr337;


namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvr337 : YR.ERP.Base.Forms.FrmReportBase
    {

        #region Property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmInvr337()
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

            // 繼承的表單要覆寫, 更改參數值            
            /*
            this.strFormID = "XXX";
             */
            this.StrFormID = "invr337";
            TabMaster.ReportName = @"\Inv\invr337.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "ifasecu";
            TabMaster.GroupColumn = "ifasecg";
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable, false);
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
                //確認否
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("0", "0.全部"));
                sourceList.Add(new KeyValuePair<string, string>("1", "1.撥出在途"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.撥入確認"));
                WfSetUcomboxDataSource(ucb_doc_status, sourceList);
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
                pDr["doc_status"] = "0";
                return true;
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
            vw_invr337 invr337Model;
            try
            {
                invr337Model = DrMaster.ToItem<vw_invr337>();
                switch (e.Column.ToLower())
                {
                    case "ifa02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(invr337Model.ifa02_e))
                        {
                            if (invr337Model.ifa02_s > invr337Model.ifa02_e)
                            {
                                WfShowErrorMsg("起日不得大於迄日!");
                                return false;
                            }
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

        #region WfFormCheck() 執行報表前檢查
        protected override bool WfFormCheck()
        {
            vw_invr337 invr337Model = null;
            string msg;
            try
            {
                invr337Model = DrMaster.ToItem<vw_invr337>();
                if (
                     GlobalFn.varIsNull(invr337Model.ifa02_s)
                    && GlobalFn.varIsNull(invr337Model.ifa02_e)
                    && GlobalFn.varIsNull(invr337Model.ifa01)
                    && GlobalFn.varIsNull(invr337Model.ifa03)
                    && GlobalFn.varIsNull(invr337Model.ifa04)
                    && GlobalFn.varIsNull(invr337Model.ifa06)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_ifa01, msg);
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

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                MessageInfo messageModel = new MessageInfo();
                switch (pColName.ToLower())
                {
                    case "ifa01"://調撥單號
                        messageModel.StrMultiColumn = "ifa01";
                        messageModel.IntMaxRow = 999;
                        messageModel.StrWhereAppend = " AND ifa00='2' ";
                        WfShowPickUtility("p_ifa", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "ifa03"://申請人員
                        messageModel.IntMaxRow = 999;
                        messageModel.StrMultiColumn = "bec01";
                        WfShowPickUtility("p_bec1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;

                    case "ifa04"://申請部門
                        messageModel.IntMaxRow = 999;
                        messageModel.StrMultiColumn = "beb01";
                        WfShowPickUtility("p_beb1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;

                    case "ifa06"://在途倉
                        messageModel.IntMaxRow = 999;
                        messageModel.StrMultiColumn = "icb01";
                        WfShowPickUtility("p_icb", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
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

        #region WfExecReport 執行報表處理
        protected override bool WfExecReport(Stimulsoft.Report.StiReport pReport)
        {
            //DataSet ds;
            vw_invr337 invr337Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            DataTable dtIlaTb;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            List<Master> resultList = null;
            try
            {
                invr337Model = DrMaster.ToItem<vw_invr337>();
                resultList = new List<Master>();
                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(invr337Model.ifa01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ifa_tb";
                    queryModel.ColumnName = "ifa01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ifa01"].DataType.Name;
                    queryModel.Value = invr337Model.ifa01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(invr337Model.ifa03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ifa_tb";
                    queryModel.ColumnName = "ifa03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ifa03"].DataType.Name;
                    queryModel.Value = invr337Model.ifa03;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(invr337Model.ifa04))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ifa_tb";
                    queryModel.ColumnName = "ifa04";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ifa04"].DataType.Name;
                    queryModel.Value = invr337Model.ifa04;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(invr337Model.ifa06))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ifa_tb";
                    queryModel.ColumnName = "ifa06";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ifa06"].DataType.Name;
                    queryModel.Value = invr337Model.ifa04;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(invr337Model.ifa02_s))
                {
                    sbQuerySingle.AppendLine("AND ifa02>=@ifa02_s");
                    sqlParmList.Add(new SqlParameter("@ifa02_s", invr337Model.ifa02_s));
                }
                if (!GlobalFn.varIsNull(invr337Model.ifa02_e))
                {
                    sbQuerySingle.AppendLine("AND ifa02<=@ifa02_e");
                    sqlParmList.Add(new SqlParameter("@ifa02_s", invr337Model.ifa02_e));
                }

                if (invr337Model.doc_status != "0")
                {
                    if (invr337Model.doc_status == "1") //撥出在途
                    {
                        sbQuerySingle.AppendLine("AND ISNULL(ifa09,'N') = 'N'");
                    }
                    if (invr337Model.doc_status == "2") //撥入確認
                    {
                        sbQuerySingle.AppendLine("AND ifaconf = 'Y'");
                    }
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"
                              SELECT 
                                ifa01,ifa02,ifa03,bec02 as ifa03_c ,
                                ifa04,beb03 AS ifa04_c,ifa05,ifa06,
                                ifa09,ifaconf,
                                ifb02,ifb03,ifb04,ifb05,'' as ifb05_str,ifb06,
                                bej03,ifb07,
                                ifb08,'' as ifb08_str,ifb09,ifb10,
                                ica03
                              FROM ifa_tb
                                    LEFT JOIN ifb_tb ON ifa01=ifb01
                                    LEFT JOIN bec_tb ON ifa03=bec01
                                    LEFT JOIN beb_tb ON ifa04=beb01
                                    LEFT JOIN bej_tb ON ifb06=bej01
                                    LEFT JOIN ica_tb ON ifb03=ica01
                              WHERE ifa00='2'
                                AND ifb01 IS NOT NULL
                                AND ifaconf='Y'
                                ";

                dtIlaTb = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere), sqlParmList.ToArray());
                dtIlaTb.TableName = "Master";
                if (dtIlaTb != null)
                {
                    resultList.AddRange(dtIlaTb.ToList<Master>());
                }

                if (resultList == null || resultList.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    return false;
                }

                foreach (Master masterModel in resultList)
                {
                    //數量處理
                    masterModel.ifb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.ifb05);//數量
                    masterModel.ifb08_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.ifb08);//數量
                }

                pReport.RegData("master", resultList);
                pReport.CacheAllData = true;

                pReport.Compile();
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
