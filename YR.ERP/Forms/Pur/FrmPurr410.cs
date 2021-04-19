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
using YR.ERP.DAL.YRModel.Reports.Pur.Purr410;

namespace YR.ERP.Forms.Pur
{
    public partial class FrmPurr410 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region Property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmPurr410()
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
            this.StrFormID = "purr410";
            TabMaster.ReportName = @"\Pur\purr410.mrt";

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
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依入庫日期"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.依廠商"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.依部門"));
                sourceList.Add(new KeyValuePair<string, string>("4", "4.入庫入員"));
                WfSetUcomboxDataSource(ucb_order_by, sourceList);

                //類別
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("0", "0.全部"));
                sourceList.Add(new KeyValuePair<string, string>("1", "1.入庫"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.退庫/折讓"));
                WfSetUcomboxDataSource(ucb_type, sourceList);
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
                pDr["type"] = "0";
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
            vw_purr410 purr410Model;
            try
            {
                purr410Model = DrMaster.ToItem<vw_purr410>();
                switch (e.Column.ToLower())
                {
                    case "sea02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(purr410Model.pga02_e))
                        {
                            if (purr410Model.pga02_s > purr410Model.pga02_e)
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
                    case "pga01"://入庫單號
                        messageModel.StrMultiColumn = "pga01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_pga1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "pga03"://廠商編號
                        messageModel.StrMultiColumn = "pca01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_pca", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "pga04"://業務人員
                        messageModel.IntMaxRow = 999;
                        messageModel.StrMultiColumn = "bec01";
                        WfShowPickUtility("p_bec1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;

                    case "pga05"://業務部門
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
            vw_purr410 purr410Model = null;
            string msg;
            try
            {
                purr410Model = DrMaster.ToItem<vw_purr410>();
                if (
                     GlobalFn.varIsNull(purr410Model.pga02_s)
                    && GlobalFn.varIsNull(purr410Model.pga02_e)
                    && GlobalFn.varIsNull(purr410Model.pga01)
                    && GlobalFn.varIsNull(purr410Model.pga03)
                    && GlobalFn.varIsNull(purr410Model.pga04)
                    && GlobalFn.varIsNull(purr410Model.pga05)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_pga01, msg);
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
            vw_purr410 purr410Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            DataTable dtPgaTb, dtPhaTb;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            List<MasterA> resultList = null;
            try
            {
                purr410Model = DrMaster.ToItem<vw_purr410>();
                resultList = new List<MasterA>();
                #region 處理類型
                if (purr410Model.type == "0" || purr410Model.type == "1")
                {
                    queryInfoList = new List<QueryInfo>();
                    #region range 處理
                    if (!GlobalFn.varIsNull(purr410Model.pga03))
                    {
                        queryModel = new QueryInfo();
                        queryModel.TableName = "pga_tb";
                        queryModel.ColumnName = "pga03";
                        queryModel.ColumnType = TabMaster.DtSource.Columns["pga03"].DataType.Name;
                        queryModel.Value = purr410Model.pga03;
                        queryInfoList.Add(queryModel);
                    }
                    if (!GlobalFn.varIsNull(purr410Model.pga04))
                    {
                        queryModel = new QueryInfo();
                        queryModel.TableName = "pga_tb";
                        queryModel.ColumnName = "pga04";
                        queryModel.ColumnType = TabMaster.DtSource.Columns["pga04"].DataType.Name;
                        queryModel.Value = purr410Model.pga04;
                        queryInfoList.Add(queryModel);
                    }
                    if (!GlobalFn.varIsNull(purr410Model.pga05))
                    {
                        queryModel = new QueryInfo();
                        queryModel.TableName = "pga_tb";
                        queryModel.ColumnName = "pga05";
                        queryModel.ColumnType = TabMaster.DtSource.Columns["pga05"].DataType.Name;
                        queryModel.Value = purr410Model.pga05;
                        queryInfoList.Add(queryModel);
                    }
                    sqlParmList = new List<SqlParameter>();
                    strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                    #endregion

                    #region single 處理
                    sbQuerySingle = new StringBuilder();
                    if (!GlobalFn.varIsNull(purr410Model.pga02_s))
                    {
                        sbQuerySingle.AppendLine("AND pga02>=@pga02_s");
                        sqlParmList.Add(new SqlParameter("@pga02_s", purr410Model.pga02_s));
                    }
                    if (!GlobalFn.varIsNull(purr410Model.pga02_e))
                    {
                        sbQuerySingle.AppendLine("AND pga02<=@pga02_e");
                        sqlParmList.Add(new SqlParameter("@pga02_e", purr410Model.pga02_e));
                    }
                    #endregion

                    strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                    var strSecurity = WfGetSecurityString();        //取得權限字串
                    if (!GlobalFn.varIsNull(strSecurity))
                        strWhere += strSecurity;

                    //取得單頭
                    sqlBody = @"
                              SELECT 
                                '入庫' AS type,
                                pga01,pga02,pga03,pca03 as pga03_c ,
                                pga04,bec02 AS pga04_c,pga05,beb03 AS pga05_c,
                                pga10,
                                pgb02,pgb03,pgb04,pgb05,'' as pgb05_str,pgb06,
                                bej03,
                                0.0 as price,'' as price_string,
                                pgb10,'' as pgb10_string,
                                pgb16
                              FROM pga_tb
                                    LEFT JOIN pgb_tb ON pga01=pgb01
                                    LEFT JOIN pca_tb ON pga03=pca01
                                    LEFT JOIN bec_tb ON pga04=bec01
                                    LEFT JOIN beb_tb ON pga05=beb01
                                    LEFT JOIN bej_tb ON pgb06=bej01
                              WHERE pgaconf='Y'
                                AND pgb01 IS NOT NULL
                                ";

                    dtPgaTb = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere), sqlParmList.ToArray());
                    dtPgaTb.TableName = "Master";
                    if (dtPgaTb != null)
                    {
                        resultList.AddRange(dtPgaTb.ToList<MasterA>());
                    }
                }
                #endregion

                #region 處理退貨折讓
                if (purr410Model.type == "0" || purr410Model.type == "2")
                {
                    queryInfoList = new List<QueryInfo>();
                    #region range 處理
                    if (!GlobalFn.varIsNull(purr410Model.pga03))
                    {
                        queryModel = new QueryInfo();
                        queryModel.TableName = "pha_tb";
                        queryModel.ColumnName = "pha03";
                        queryModel.ColumnType = TabMaster.DtSource.Columns["pga03"].DataType.Name;
                        queryModel.Value = purr410Model.pga03;
                        queryInfoList.Add(queryModel);
                    }
                    if (!GlobalFn.varIsNull(purr410Model.pga04))
                    {
                        queryModel = new QueryInfo();
                        queryModel.TableName = "pha_tb";
                        queryModel.ColumnName = "pha04";
                        queryModel.ColumnType = TabMaster.DtSource.Columns["pga04"].DataType.Name;
                        queryModel.Value = purr410Model.pga04;
                        queryInfoList.Add(queryModel);
                    }
                    if (!GlobalFn.varIsNull(purr410Model.pga05))
                    {
                        queryModel = new QueryInfo();
                        queryModel.TableName = "pha_tb";
                        queryModel.ColumnName = "pha05";
                        queryModel.ColumnType = TabMaster.DtSource.Columns["pga05"].DataType.Name;
                        queryModel.Value = purr410Model.pga05;
                        queryInfoList.Add(queryModel);
                    }
                    sqlParmList = new List<SqlParameter>();
                    strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                    #endregion

                    #region single 處理
                    sbQuerySingle = new StringBuilder();
                    if (!GlobalFn.varIsNull(purr410Model.pga02_s))
                    {
                        sbQuerySingle.AppendLine("AND pha02>=@pha02_s");
                        sqlParmList.Add(new SqlParameter("@pha02_s", purr410Model.pga02_s));
                    }
                    if (!GlobalFn.varIsNull(purr410Model.pga02_e))
                    {
                        sbQuerySingle.AppendLine("AND pha02<=@pha02_e");
                        sqlParmList.Add(new SqlParameter("@pga02_e", purr410Model.pga02_e));
                    }
                    #endregion

                    strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                    var strSecurity = WfGetSecurityString();        //取得權限字串
                    if (!GlobalFn.varIsNull(strSecurity))
                        strWhere += strSecurity;

                    //取得單頭
                    sqlBody = @"
                              SELECT 
                                CASE WHEN phb17='1' THEN '退貨' ELSE '折讓' END AS type,
                                pha01 AS pga01,pha02 AS pga02,pha03 AS pga03,pca03 as pga03_c ,
                                pha04 AS pga04,bec02 AS pga04_c,pha05 AS pga05,beb03 AS pga05_c,
                                pha10 AS pga10,
                                phb02 AS pgb02,phb03 AS pgb03,phb04 AS pgb04,phb05 AS pgb05,'' as pgb05_str,phb06 AS pgb06,
                                bej03,
                                0.0 as price,'' as price_string,
                                -1*phb10 AS pgb10,'' as pgb10_string,
                                phb16 AS pgb16
                              FROM pha_tb
                                    LEFT JOIN phb_tb ON pha01=phb01
                                    LEFT JOIN pca_tb ON pha03=pca01
                                    LEFT JOIN bec_tb ON pha04=bec01
                                    LEFT JOIN beb_tb ON pha05=beb01
                                    LEFT JOIN bej_tb ON phb06=bej01
                              WHERE phaconf='Y'
                                    AND phb01 IS NOT NULL
                                ";

                    dtPgaTb = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere), sqlParmList.ToArray());
                    dtPgaTb.TableName = "Master";
                    if (dtPgaTb != null)
                    {
                        resultList.AddRange(dtPgaTb.ToList<MasterA>());
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
                    var bekModel = BoBas.OfGetBekModel(masterModel.pga10);
                    //處理單價
                    if (masterModel.pgb05 > 0)
                    {
                        masterModel.price = Math.Round((masterModel.pgb10 / masterModel.pgb05), Convert.ToInt16(bekModel.bek03), MidpointRounding.AwayFromZero);
                    }
                    else
                        masterModel.price = masterModel.pgb10;

                    masterModel.price_string = string.Format("{0:N" + bekModel.bek03 + "}", masterModel.price);
                    masterModel.pgb10_string = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pgb10);
                    //數量處理
                    masterModel.pgb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.pgb05);//數量
                }

                pReport.RegData("master", resultList);
                pReport.CacheAllData = true;
                //處理排序--這裡利用group來達到
                StiGroupHeaderBand stiGroupHeaderBand1 = (StiGroupHeaderBand)pReport.GetComponents()["GroupHeaderBand1"];
                switch (purr410Model.order_by)
                {
                    case "1"://1.依出庫日期
                        stiGroupHeaderBand1.Condition.Value = "{Master.pga02}";
                        break;
                    case "2"://2.依客戶
                        stiGroupHeaderBand1.Condition.Value = "{Master.pga03}";
                        break;
                    case "3"://3.依部門
                        stiGroupHeaderBand1.Condition.Value = "{Master.pga05}";
                        break;
                    case "4"://4.依業務
                        stiGroupHeaderBand1.Condition.Value = "{Master.pga04}";
                        break;
                }
                //處理跳頁
                StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                if (purr410Model.jump_yn.ToUpper() == "Y")
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
