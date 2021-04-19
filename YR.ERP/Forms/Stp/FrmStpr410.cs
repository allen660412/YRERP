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
using YR.ERP.DAL.YRModel.Reports.Stp.Stpr410;
using System.Linq;

namespace YR.ERP.Forms.Stp
{
    public partial class FrmStpr410 : YR.ERP.Base.Forms.FrmReportBase
    {

        #region Property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmStpr410()
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
            this.StrFormID = "stpr410";
            TabMaster.ReportName = @"\Stp\stpr410.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            //TabMaster.UserColumn = "seasecu";
            //TabMaster.GroupColumn = "seasecg";
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
                sourceList.Add(new KeyValuePair<string, string>("0", "0.全部"));
                sourceList.Add(new KeyValuePair<string, string>("1", "1.已確認"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.未確認"));
                WfSetUcomboxDataSource(ucb_confirm_type, sourceList);

                //排序
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依出貨日期"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.依客戶"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.依部門"));
                sourceList.Add(new KeyValuePair<string, string>("4", "4.業務"));
                WfSetUcomboxDataSource(ucb_order_by, sourceList);

                //類別
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("0", "0.全部"));
                sourceList.Add(new KeyValuePair<string, string>("1", "1.銷貨"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.銷退/折讓"));
                WfSetUcomboxDataSource(ucb_sale_type, sourceList);
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
                pDr["confirm_type"] = "0";
                pDr["sale_type"] = "0";
                pDr["jump_yn"] = "Y";
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
            vw_stpr410 stpr410Model;
            try
            {
                stpr410Model = DrMaster.ToItem<vw_stpr410>();
                switch (e.Column.ToLower())
                {
                    case "sea02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(stpr410Model.sga02_e))
                        {
                            if (stpr410Model.sga02_s > stpr410Model.sga02_e)
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

        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                MessageInfo messageModel = new MessageInfo();
                switch (pColName.ToLower())
                {
                    case "sga03"://客戶編號
                        messageModel.StrMultiColumn = "sca01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_sca", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "sga04"://業務人員
                        messageModel.IntMaxRow = 999;
                        messageModel.StrMultiColumn = "bec01";
                        WfShowPickUtility("p_bec1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;

                    case "sga05"://業務部門
                        messageModel.IntMaxRow = 999;
                        messageModel.StrMultiColumn = "beb01";
                        WfShowPickUtility("p_beb1", messageModel);
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

        #region WfFormCheck() 執行報表前檢查
        protected override bool WfFormCheck()
        {
            vw_stpr410 stpr410Model = null;
            string msg;
            try
            {
                stpr410Model = DrMaster.ToItem<vw_stpr410>();
                if (
                     GlobalFn.varIsNull(stpr410Model.sga02_s)
                    && GlobalFn.varIsNull(stpr410Model.sga02_e)
                    && GlobalFn.varIsNull(stpr410Model.sga03)
                    && GlobalFn.varIsNull(stpr410Model.sga04)
                    && GlobalFn.varIsNull(stpr410Model.sga05)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_sga03, msg);
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

        #region WfExecReport 執行報表處理
        protected override bool WfExecReport(Stimulsoft.Report.StiReport pReport)
        {
            //DataSet ds;
            vw_stpr410 stpr410Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            DataTable dtSgaTb, dtShaTb;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            List<MasterA> resultList = null;
            try
            {
                stpr410Model = DrMaster.ToItem<vw_stpr410>();
                resultList = new List<MasterA>();
                #region 處理銷售
                if (stpr410Model.sale_type == "0" || stpr410Model.sale_type == "1")
                {
                    queryInfoList = new List<QueryInfo>();
                    #region range 處理
                    if (!GlobalFn.varIsNull(stpr410Model.sga03))
                    {
                        queryModel = new QueryInfo();
                        queryModel.TableName = "sga_tb";
                        queryModel.ColumnName = "sga03";
                        queryModel.ColumnType = TabMaster.DtSource.Columns["sga03"].DataType.Name;
                        queryModel.Value = stpr410Model.sga03;
                        queryInfoList.Add(queryModel);
                    }
                    if (!GlobalFn.varIsNull(stpr410Model.sga04))
                    {
                        queryModel = new QueryInfo();
                        queryModel.TableName = "sga_tb";
                        queryModel.ColumnName = "sga04";
                        queryModel.ColumnType = TabMaster.DtSource.Columns["sga04"].DataType.Name;
                        queryModel.Value = stpr410Model.sga04;
                        queryInfoList.Add(queryModel);
                    }
                    if (!GlobalFn.varIsNull(stpr410Model.sga05))
                    {
                        queryModel = new QueryInfo();
                        queryModel.TableName = "sga_tb";
                        queryModel.ColumnName = "sga05";
                        queryModel.ColumnType = TabMaster.DtSource.Columns["sga05"].DataType.Name;
                        queryModel.Value = stpr410Model.sga05;
                        queryInfoList.Add(queryModel);
                    }
                    sqlParmList = new List<SqlParameter>();
                    strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                    #endregion

                    #region single 處理
                    sbQuerySingle = new StringBuilder();
                    if (!GlobalFn.varIsNull(stpr410Model.sga02_s))
                    {
                        sbQuerySingle.AppendLine("AND sga02>=@sga02_s");
                        sqlParmList.Add(new SqlParameter("@sga02_s", stpr410Model.sga02_s));
                    }
                    if (!GlobalFn.varIsNull(stpr410Model.sga02_e))
                    {
                        sbQuerySingle.AppendLine("AND sga02<=@sga02_e");
                        sqlParmList.Add(new SqlParameter("@sga02_e", stpr410Model.sga02_e));
                    }

                    if (stpr410Model.confirm_type == "1")
                    {
                        sbQuerySingle.AppendLine("AND sgaconf ='Y'");
                    }
                    else if (stpr410Model.confirm_type == "2")
                    {
                        sbQuerySingle.AppendLine("AND ISNULL(sgaconf,'') <>'Y'");
                    }
                    #endregion

                    strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                    var strSecurity = WfGetSecurityString();        //取得權限字串
                    if (!GlobalFn.varIsNull(strSecurity))
                        strWhere += strSecurity;

                    //取得單頭
                    sqlBody = @"
                              SELECT 
                                '銷貨' AS sale_type,
                                sga01,sga02,sga03,sca03 as sga03_c ,
                                sgb02,sga04,bec02 AS sga04_c,sga05,beb03 AS sga05_c,
                                sga10,sga21,
                                sgb03,sgb04,sgb05,'' as sgb05_str,sgb06,
                                bej03,
                                0.0 as price,'' as price_string,
                                sgb10,'' as sgb10_string,
                                sgb10t,'' as sgb10t_string,
                                sgb16
                              FROM sga_tb
                                    LEFT JOIN sgb_tb ON sga01=sgb01
                                    LEFT JOIN sca_tb ON sga03=sca01
                                    LEFT JOIN bec_tb ON sga04=bec01
                                    LEFT JOIN beb_tb ON sga05=beb01
                                    LEFT JOIN bej_tb ON sgb06=bej01
                              WHERE  sgb01 IS NOT NULL
                                ";

                    dtSgaTb = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere), sqlParmList.ToArray());
                    dtSgaTb.TableName = "Master";
                    if (dtSgaTb != null)
                    {
                        resultList.AddRange(dtSgaTb.ToList<YR.ERP.DAL.YRModel.Reports.Stp.Stpr410.MasterA>());
                    }
                }
                #endregion

                #region 處理退貨/折讓
                if (stpr410Model.sale_type == "0" || stpr410Model.sale_type == "2")
                {
                    queryInfoList = new List<QueryInfo>();
                    #region range 處理
                    if (!GlobalFn.varIsNull(stpr410Model.sga03))
                    {
                        queryModel = new QueryInfo();
                        queryModel.TableName = "sha_tb";
                        queryModel.ColumnName = "sha03";
                        queryModel.ColumnType = TabMaster.DtSource.Columns["sga03"].DataType.Name;
                        queryModel.Value = stpr410Model.sga03;
                        queryInfoList.Add(queryModel);
                    }
                    if (!GlobalFn.varIsNull(stpr410Model.sga04))
                    {
                        queryModel = new QueryInfo();
                        queryModel.TableName = "sha_tb";
                        queryModel.ColumnName = "sha04";
                        queryModel.ColumnType = TabMaster.DtSource.Columns["sga04"].DataType.Name;
                        queryModel.Value = stpr410Model.sga04;
                        queryInfoList.Add(queryModel);
                    }
                    if (!GlobalFn.varIsNull(stpr410Model.sga05))
                    {
                        queryModel = new QueryInfo();
                        queryModel.TableName = "sha_tb";
                        queryModel.ColumnName = "sha05";
                        queryModel.ColumnType = TabMaster.DtSource.Columns["sga05"].DataType.Name;
                        queryModel.Value = stpr410Model.sga05;
                        queryInfoList.Add(queryModel);
                    }
                    sqlParmList = new List<SqlParameter>();
                    strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                    #endregion

                    #region single 處理
                    sbQuerySingle = new StringBuilder();
                    if (!GlobalFn.varIsNull(stpr410Model.sga02_s))
                    {
                        sbQuerySingle.AppendLine("AND sha02>=@sha02_s");
                        sqlParmList.Add(new SqlParameter("@sha02_s", stpr410Model.sga02_s));
                    }
                    if (!GlobalFn.varIsNull(stpr410Model.sga02_e))
                    {
                        sbQuerySingle.AppendLine("AND sha02<=@sha02_e");
                        sqlParmList.Add(new SqlParameter("@sha02_e", stpr410Model.sga02_e));
                    }

                    if (stpr410Model.confirm_type == "1")
                    {
                        sbQuerySingle.AppendLine("AND shaconf ='Y'");
                    }
                    else if (stpr410Model.confirm_type == "2")
                    {
                        sbQuerySingle.AppendLine("AND ISNULL(shaconf,'') <>'Y'");
                    }
                    #endregion

                    strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                    var strSecurity = WfGetSecurityString();        //取得權限字串
                    if (!GlobalFn.varIsNull(strSecurity))
                        strWhere += strSecurity;

                    //取得單頭
                    sqlBody = @"
                              SELECT 
                                CASE WHEN shb17='1' THEN '銷退' ELSE '折讓' END AS sale_type,
                                sha01 AS sga01,sha02 AS sga02,sha03 AS sga03,sca03 as sga03_c ,
                                shb02 AS sgb02,sha04 AS sga04,bec02 AS sga04_c,sha05 AS sga05,beb03 AS sga05_c,
                                sha10 AS sga10,shb19 AS sga21,
                                shb03 AS sgb03,shb04 AS sgb04,shb05 AS sgb05,'' as sgb05_str,shb06 AS sgb06,
                                bej03,
                                0.0 as price,'' as price_string,
                                -1*shb10 AS sgb10,'' as sgb10_string,
                                -1*shb10t AS sgb10t,'' as sgb10t_string,
                                shb16 AS sgb16
                              FROM sha_tb
                                    LEFT JOIN shb_tb ON sha01=shb01
                                    LEFT JOIN sca_tb ON sha03=sca01
                                    LEFT JOIN bec_tb ON sha04=bec01
                                    LEFT JOIN beb_tb ON sha05=beb01
                                    LEFT JOIN bej_tb ON shb06=bej01
                              WHERE  shb01 IS NOT NULL
                                ";

                    dtSgaTb = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere), sqlParmList.ToArray());
                    dtSgaTb.TableName = "Master";
                    if (dtSgaTb != null)
                    {
                        resultList.AddRange(dtSgaTb.ToList<YR.ERP.DAL.YRModel.Reports.Stp.Stpr410.MasterA>());
                    }
                }
                #endregion

                if (resultList == null || resultList.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    return false;
                }

                foreach (MasterA masterModel in resultList)
                {
                    var bekModel = BoBas.OfGetBekModel(masterModel.sga10);
                    //處理單價
                    if (masterModel.sgb05 > 0)
                    {
                        masterModel.price = Math.Round((masterModel.sgb10 / masterModel.sgb05), Convert.ToInt16(bekModel.bek03), MidpointRounding.AwayFromZero);
                    }
                    else
                        masterModel.price = masterModel.sgb10;

                    masterModel.price_string = string.Format("{0:N" + bekModel.bek03 + "}", masterModel.price);
                    masterModel.sgb10_string = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.sgb10);
                    masterModel.sgb10t_string = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.sgb10t);
                    //數量處理
                    masterModel.sgb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.sgb05);//數量
                }

                pReport.RegData("master", resultList);
                pReport.CacheAllData = true;
                //處理排序--這裡利用group來達到

                StiGroupHeaderBand stiGroupHeaderBand1 = (StiGroupHeaderBand)pReport.GetComponents()["GroupHeaderBand1"];
                StiDataBand StiDataBand1 = (StiDataBand)pReport.GetComponents()["DataBand1"];
                switch (stpr410Model.order_by)
                {
                    case "1"://1.依出庫日期
                        stiGroupHeaderBand1.Condition.Value = "{Master.sga02}";
                        StiDataBand1.Sort = new string[6]{
                                        "ASC",
                                        "sga02",
                                        "ASC",
                                        "sga01",
                                        "ASC",
                                        "sgb02",
                                        };
                        break;
                    case "2"://2.依客戶
                        stiGroupHeaderBand1.Condition.Value = "{Master.sga03}";
                        StiDataBand1.Sort = new string[6]{
                                        "ASC",
                                        "sga03",
                                        "ASC",
                                        "sga01",
                                        "ASC",
                                        "sgb02",
                                        };
                        break;
                    case "3"://3.依部門
                        stiGroupHeaderBand1.Condition.Value = "{Master.sga05}";
                        StiDataBand1.Sort = new string[6]{
                                        "ASC",
                                        "sga05",
                                        "ASC",
                                        "sga01",
                                        "ASC",
                                        "sgb02",
                                        };
                        break;
                    case "4"://4.依業務
                        stiGroupHeaderBand1.Condition.Value = "{Master.sga04}";
                        StiDataBand1.Sort = new string[6]{
                                        "ASC",
                                        "sga04",
                                        "ASC",
                                        "sga01",
                                        "ASC",
                                        "sgb02",
                                        };
                        break;
                }
                //處理跳頁
                StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                if (stpr410Model.jump_yn.ToUpper() == "Y")
                {
                    footerBand1.NewPageAfter = true;
                    footerBand1.ResetPageNumber = true;
                }
                else
                {
                    footerBand1.NewPageAfter = false;
                    footerBand1.ResetPageNumber = false;
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
