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
using YR.ERP.DAL.YRModel.Reports.Inv.Invr410;


namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvr410 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region Property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmInvr410()
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
            this.StrFormID = "invr410";
            TabMaster.ReportName = @"\Inv\invr410.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "ilasecu";
            TabMaster.GroupColumn = "ilasecg";
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
                //排序
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依借出日期"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.依客戶"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.依部門"));
                sourceList.Add(new KeyValuePair<string, string>("4", "4.業務"));
                sourceList.Add(new KeyValuePair<string, string>("5", "5.出庫倉"));
                WfSetUcomboxDataSource(ucb_order_by, sourceList);

                //結案否
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("0", "0.全部"));
                sourceList.Add(new KeyValuePair<string, string>("1", "1.未結案"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.已結案"));
                WfSetUcomboxDataSource(ucb_close_yn, sourceList);

                //確認否
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("0", "0.全部"));
                sourceList.Add(new KeyValuePair<string, string>("1", "1.已確認"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.未確認"));
                WfSetUcomboxDataSource(ucb_conf_yn, sourceList);
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
                pDr["conf_yn"] = "1";
                pDr["close_yn"] = "1";
                pDr["jump_yn"] = "N";
                pDr["order_by"] = "1";
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
            vw_invr410 invr410Model;
            try
            {
                invr410Model = DrMaster.ToItem<vw_invr410>();
                switch (e.Column.ToLower())
                {
                    case "ila02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(invr410Model.ila02_e))
                        {
                            if (invr410Model.ila02_s > invr410Model.ila02_e)
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
            vw_invr410 invr410Model = null;
            string msg;
            try
            {
                invr410Model = DrMaster.ToItem<vw_invr410>();
                if (
                     GlobalFn.varIsNull(invr410Model.ila02_s)
                    && GlobalFn.varIsNull(invr410Model.ila02_e)
                    && GlobalFn.varIsNull(invr410Model.ila03)
                    && GlobalFn.varIsNull(invr410Model.ila04)
                    && GlobalFn.varIsNull(invr410Model.ila05)
                    && GlobalFn.varIsNull(invr410Model.ilb03)
                    && GlobalFn.varIsNull(invr410Model.ilb11)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_ila03, msg);
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
                    case "ila03"://客戶編號
                        messageModel.StrMultiColumn = "sca01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_sca", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "ila04"://業務人員
                        messageModel.IntMaxRow = 999;
                        messageModel.StrMultiColumn = "bec01";
                        WfShowPickUtility("p_bec1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;

                    case "ila05"://業務部門
                        messageModel.IntMaxRow = 999;
                        messageModel.StrMultiColumn = "beb01";
                        WfShowPickUtility("p_beb1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "ilb03"://料號
                        messageModel.IntMaxRow = 999;
                        messageModel.StrMultiColumn = "ica01";
                        WfShowPickUtility("p_ica1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "ilb11"://出庫倉
                        messageModel.IntMaxRow = 999;
                        messageModel.StrMultiColumn = "icb01";
                        WfShowPickUtility("p_icb1", messageModel);
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
            vw_invr410 invr410Model;
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
                invr410Model = DrMaster.ToItem<vw_invr410>();
                resultList = new List<Master>();
                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(invr410Model.ila03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ila_tb";
                    queryModel.ColumnName = "ila03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ila03"].DataType.Name;
                    queryModel.Value = invr410Model.ila03;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(invr410Model.ila04))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ila_tb";
                    queryModel.ColumnName = "ila04";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ila04"].DataType.Name;
                    queryModel.Value = invr410Model.ila04;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(invr410Model.ila05))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ila_tb";
                    queryModel.ColumnName = "ila05";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ila05"].DataType.Name;
                    queryModel.Value = invr410Model.ila05;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(invr410Model.ila02_s))
                {
                    sbQuerySingle.AppendLine("AND ila02>=@ila02_s");
                    sqlParmList.Add(new SqlParameter("@ila02_s", invr410Model.ila02_s));
                }
                if (!GlobalFn.varIsNull(invr410Model.ila02_e))
                {
                    sbQuerySingle.AppendLine("AND ila02<=@ila02_e");
                    sqlParmList.Add(new SqlParameter("@ila02_e", invr410Model.ila02_e));
                }

                if (invr410Model.conf_yn!="0" )
                {
                    if (invr410Model.conf_yn=="1")
                    {
                        sbQuerySingle.AppendLine("AND ilaconf = 'Y'");
                    }
                    if (invr410Model.conf_yn == "2")
                    {
                        sbQuerySingle.AppendLine("AND ilaconf = 'N'");
                    }
                }

                if (invr410Model.close_yn != "0")
                {
                    if (invr410Model.close_yn == "1")//未結案
                    {
                        sbQuerySingle.AppendLine("AND (ilb05-ilb15) >0 ");
                    }
                    if (invr410Model.close_yn == "2")//已結案
                    {
                        sbQuerySingle.AppendLine("AND (ilb05-ilb15) <=0  ");
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
                                ila01,ila02,ila03,sca03 as ila03_c ,
                                ilb02,ila04,bec02 AS ila04_c,ila05,beb03 AS ila05_c,
                                ilb03,ilb04,ilb05,'' as ilb05_str,ilb06,
                                bej03,ilb11,
                                ilb15,'' as ilb15_str,
                                ilb16
                              FROM ila_tb
                                    LEFT JOIN ilb_tb ON ila01=ilb01
                                    LEFT JOIN sca_tb ON ila03=sca01
                                    LEFT JOIN bec_tb ON ila04=bec01
                                    LEFT JOIN beb_tb ON ila05=beb01
                                    LEFT JOIN bej_tb ON ilb06=bej01
                              WHERE ilaconf='Y'
                                AND ilb01 IS NOT NULL
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
                    masterModel.ilb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.ilb05);//數量
                    masterModel.ilb15_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.ilb15);//數量
                }

                pReport.RegData("master", resultList);
                pReport.CacheAllData = true;
                //處理排序--這裡利用group來達到
                StiGroupHeaderBand stiGroupHeaderBand1 = (StiGroupHeaderBand)pReport.GetComponents()["GroupHeaderBand1"];
                switch (invr410Model.order_by)
                {
                    case "1"://1.依出庫日期
                        stiGroupHeaderBand1.Condition.Value = "{Master.ila02}";
                        break;
                    case "2"://2.依客戶
                        stiGroupHeaderBand1.Condition.Value = "{Master.ila03}";
                        break;
                    case "3"://3.依部門
                        stiGroupHeaderBand1.Condition.Value = "{Master.ila05}";
                        break;
                    case "4"://4.依業務
                        stiGroupHeaderBand1.Condition.Value = "{Master.ila04}";
                        break;
                    case "5"://4.依出庫倉
                        stiGroupHeaderBand1.Condition.Value = "{Master.ila11}";
                        break;
                }
                //處理跳頁

                StiDataBand dataBand1 = (StiDataBand)pReport.GetComponents()["DataBand1"];
                if (invr410Model.jump_yn.ToUpper() == "Y")
                {
                    dataBand1.NewPageAfter = true;
                    dataBand1.ResetPageNumber = true;
                }
                else
                {
                    dataBand1.NewPageAfter = false;
                    dataBand1.ResetPageNumber = false;
                }

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
