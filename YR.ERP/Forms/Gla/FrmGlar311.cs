/* 程式名稱: 傳票明細表列印
   系統代號: glar311
   作　　者: Allen
   描　　述: 
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
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
using System.Linq;
using YR.ERP.DAL.YRModel.Reports.Gla.Glar311;
using Stimulsoft.Report;

namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlar311 : YR.ERP.Base.Forms.FrmReportBase
    {

        #region property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmGlar311()
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
            this.StrFormID = "glar311";
            TabMaster.ReportName = @"\Gla\glar311.mrt";

            return true;
        }
        #endregion
        
        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "glasecu";
            TabMaster.GroupColumn = "glasecg";
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
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
                //過帳否
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.未過帳"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.已過帳"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.全部"));
                WfSetUcomboxDataSource(ucb_gfapost, sourceList);

                //有效否
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.有效"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.無效"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.全部"));


                //排序
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.傳票編號"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.傳票日期"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.科目編號"));
                sourceList.Add(new KeyValuePair<string, string>("4", "4.部門編號"));
                sourceList.Add(new KeyValuePair<string, string>("5", "5.傳票項次"));
                sourceList.Add(new KeyValuePair<string, string>("6", "6.借貸方"));
                sourceList.Add(new KeyValuePair<string, string>("", ""));

                WfSetUcomboxDataSource(ucb_order1, sourceList);
                WfSetUcomboxDataSource(ucb_order2, sourceList);
                WfSetUcomboxDataSource(ucb_order3, sourceList);
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
                pDr["gfaconf"] = "3";
                pDr["gfapost"] = "3";
                pDr["order1"] = "1";
                pDr["order1_jump_yn"] = "N";
                pDr["order1_sum_yn"] = "Y";
                pDr["order2"] = "5";
                pDr["order2_jump_yn"] = "N";
                pDr["order2_sum_yn"] = "N";
                pDr["order3"] = "";
                pDr["order3_jump_yn"] = "N";
                pDr["order3_sum_yn"] = "N";
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
            vw_glar311 glar311Model;
            try
            {
                glar311Model = DrMaster.ToItem<vw_glar311>();
                switch (e.Column.ToLower())
                {
                    case "gfa02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(glar311Model.gfa02_e))
                        {
                            if (glar311Model.gfa02_s > glar311Model.gfa02_e)
                            {
                                WfShowErrorMsg("起日不得大於迄日!");
                                return false;
                            }
                        }
                        break;
                    case "gfa02_e":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(glar311Model.gfa02_s))
                        {
                            if (glar311Model.gfa02_s > glar311Model.gfa02_e)
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
                    case "gfa01"://傳票單號
                        messageModel.StrMultiColumn = "gfa01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_gfa", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "gfb05"://會計部門
                        messageModel.StrMultiColumn = "beb01";
                        messageModel.IntMaxRow = 999;
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

        #region WfExecReport 執行報表處理
        protected override bool WfExecReport(Stimulsoft.Report.StiReport pReport)
        {
            vw_glar311 glar311Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            string sqlOrderBy = "";
            DataTable dtMaster;
            List<Master> masterList;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            try
            {
                glar311Model = DrMaster.ToItem<vw_glar311>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(glar311Model.gfa01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "gfa_tb";
                    queryModel.ColumnName = "gfa01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["gfa01"].DataType.Name;
                    queryModel.Value = glar311Model.gfa01;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(glar311Model.gfa02_s))
                {
                    sbQuerySingle.AppendLine("AND gfa02>=@gfa02_s");
                    sqlParmList.Add(new SqlParameter("@gfa02_s", glar311Model.gfa02_s));
                }
                if (!GlobalFn.varIsNull(glar311Model.gfa02_e))
                {
                    sbQuerySingle.AppendLine("AND gfa02<=@gfa02_e");
                    sqlParmList.Add(new SqlParameter("@gfa02_e", glar311Model.gfa02_e));
                }
                
                //過帳否
                if (glar311Model.gfapost == "1")
                {
                    sbQuerySingle.AppendLine("AND gfapost='N'");
                }
                else if (glar311Model.gfapost == "2")
                {
                    sbQuerySingle.AppendLine("AND gfapost='Y'");
                }

                //有效否
                if (glar311Model.gfaconf == "1")
                {
                    sbQuerySingle.AppendLine("AND gfaconf <>'X' ");
                }
                else if (glar311Model.gfaconf == "2")
                {
                    sbQuerySingle.AppendLine("AND gfapost='X' ");
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"SELECT 
	                            gfa01,gfa02,gfa03,gfa04,gfa05,
	                            gfa06,gfa07,gfa08,gfa09,
	                            gfaprno,gfaconf,gfacond,gfaconu,
	                            gfapost,gfaposd,gfaposu,
                                gfacreu,gfacred,
                                gac02 as gfa01_c,
	                            gfb02,gfb03,gfb04,gfb05,
	                            gfb06,gfb07,gfb08,gfb09,gfb10,
	                            gba02 AS gfb03_c,
	                            beb03 AS gfb05_c,
	                            bek04,
	                            gba05
                            FROM gfa_tb                                
                                LEFT JOIN gfb_tb ON gfa01=gfb01
	                            LEFT JOIN baa_tb ON 1=1	
	                            LEFT JOIN gac_tb ON substring(gfa01,1,baa06)=gac01
	                            LEFT JOIN gba_tb ON gfb03=gba01
	                            LEFT JOIN bek_tb ON gfb08=bek01
	                            LEFT JOIN beb_tb ON gfb05=beb01
                            WHERE 1=1 		
                           ";
                #region 排序處理
                if (!GlobalFn.varIsNull(glar311Model.order1))
                {
                    switch (glar311Model.order1)
                    {
                        case "1":
                            sqlOrderBy = "ORDER BY gfa01";
                            break;
                        case "2":
                            sqlOrderBy = "ORDER BY gfa02";
                            break;
                        case "3":
                            sqlOrderBy = "ORDER BY gfb03";
                            break;
                        case "4":
                            sqlOrderBy = "ORDER BY gfb05";
                            break;
                        case "5":
                            sqlOrderBy = "ORDER BY gfb02";
                            break;
                        case "6":
                            sqlOrderBy = "ORDER BY gfa06";
                            break;
                    }
                }
                if (!GlobalFn.varIsNull(glar311Model.order2))
                {
                    if (GlobalFn.varIsNull(sqlOrderBy))
                    {
                        switch (glar311Model.order2)
                        {
                            case "1":
                                sqlOrderBy = "ORDER BY gfa01";
                                break;
                            case "2":
                                sqlOrderBy = "ORDER BY gfa02";
                                break;
                            case "3":
                                sqlOrderBy = "ORDER BY gfb03";
                                break;
                            case "4":
                                sqlOrderBy = "ORDER BY gfb05";
                                break;
                            case "5":
                                sqlOrderBy = "ORDER BY gfb02";
                                break;
                            case "6":
                                sqlOrderBy = "ORDER BY gfa06";
                                break;
                        }
                    }
                    else
                    {
                        switch (glar311Model.order2)
                        {
                            case "1":
                                sqlOrderBy += ",gfa01";
                                break;
                            case "2":
                                sqlOrderBy += ",gfa02";
                                break;
                            case "3":
                                sqlOrderBy += ",gfb03";
                                break;
                            case "4":
                                sqlOrderBy += ",gfb05";
                                break;
                            case "5":
                                sqlOrderBy += ",gfb02";
                                break;
                            case "6":
                                sqlOrderBy += ",gfa06";
                                break;
                        }

                    }
                }
                if (!GlobalFn.varIsNull(glar311Model.order3))
                {
                    if (GlobalFn.varIsNull(sqlOrderBy))
                    {
                        switch (glar311Model.order3)
                        {
                            case "1":
                                sqlOrderBy = "ORDER BY gfa01";
                                break;
                            case "2":
                                sqlOrderBy = "ORDER BY gfa02";
                                break;
                            case "3":
                                sqlOrderBy = "ORDER BY gfb03";
                                break;
                            case "4":
                                sqlOrderBy = "ORDER BY gfb05";
                                break;
                            case "5":
                                sqlOrderBy = "ORDER BY gfb02";
                                break;
                            case "6":
                                sqlOrderBy = "ORDER BY gfa06";
                                break;
                        }
                    }
                    else
                    {
                        switch (glar311Model.order3)
                        {
                            case "1":
                                sqlOrderBy += ",gfa01";
                                break;
                            case "2":
                                sqlOrderBy += ",gfa02";
                                break;
                            case "3":
                                sqlOrderBy += ",gfb03";
                                break;
                            case "4":
                                sqlOrderBy += ",gfb05";
                                break;
                            case "5":
                                sqlOrderBy += ",gfb02";
                                break;
                            case "6":
                                sqlOrderBy += ",gfa06";
                                break;
                        }

                    }
                }
                #endregion

                dtMaster = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere, sqlOrderBy), sqlParmList.ToArray());
                dtMaster.TableName = "Master";

                if (dtMaster == null || dtMaster.Rows.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    return false;
                }
                masterList = dtMaster.ToList<Master>(true);
                //取得本幣資料
                var baaModel = BoBas.OfGetBaaModel();
                if (baaModel == null)
                {
                    WfShowErrorMsg("未設定基本參數,請先設定!");
                    return false;
                }
                var baa04 = baaModel.baa04;
                var bekUsModel = BoBas.OfGetBekModel(baa04);
                if (bekUsModel == null)
                {
                    WfShowErrorMsg("查無本幣資料,請先設定!");
                    return false;
                }
                masterList = dtMaster.ToList<Master>(true);

                foreach (Master masterModel in masterList)
                {
                    if (masterModel.gfb06 == "1")
                    {
                        masterModel.gfb07d = masterModel.gfb07;
                        masterModel.gfb07d_str = string.Format("{0:N" + bekUsModel.bek04 + "}", masterModel.gfb07);
                        masterModel.gfb10d = masterModel.gfb10;
                        masterModel.gfb10d_str = string.Format("{0:N" + masterModel.bek04 + "}", masterModel.gfb10);
                    }
                    else
                    {
                        masterModel.gfb07c = masterModel.gfb07;
                        masterModel.gfb07c_str = string.Format("{0:N" + bekUsModel.bek04 + "}", masterModel.gfb07);
                        masterModel.gfb10c = masterModel.gfb10;
                        masterModel.gfb10c_str = string.Format("{0:N" + masterModel.bek04 + "}", masterModel.gfb10);
                    }
                }

                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;

                #region 列印選項處理
                StiGroupHeaderBand headerBand1 = (StiGroupHeaderBand)pReport.GetComponents()["GroupHeaderBand1"];
                StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                StiGroupHeaderBand headerBand2 = (StiGroupHeaderBand)pReport.GetComponents()["GroupHeaderBand2"];
                StiGroupFooterBand footerBand2 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand2"];
                StiGroupHeaderBand headerBand3 = (StiGroupHeaderBand)pReport.GetComponents()["GroupHeaderBand3"];
                StiGroupFooterBand footerBand3 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand3"];
                if (!GlobalFn.varIsNull(glar311Model.order1)
                            && (glar311Model.order1_jump_yn == "Y" || glar311Model.order1_sum_yn == "Y"))
                {
                    switch (glar311Model.order1)
                    {
                        case "1":
                            headerBand1.Condition.Value = "{Master.gfa01}";
                            break;
                        case "2":
                            headerBand1.Condition.Value = "{Master.gfa02}";
                            break;
                        case "3":
                            headerBand1.Condition.Value = "{Master.gfb03}";
                            break;
                        case "4":
                            headerBand1.Condition.Value = "{Master.gfb05}";
                            break;
                        case "5":
                            headerBand1.Condition.Value = "{Master.gfb02}";
                            break;
                        case "6":
                            headerBand1.Condition.Value = "{Master.gfa06}";
                            break;
                    }
                    
                    //處理跳頁
                    if (glar311Model.order1_jump_yn == "Y")
                    {
                        headerBand1.NewPageBefore = true;
                        headerBand1.ResetPageNumber = true;
                    }
                    else
                    {
                        headerBand1.NewPageBefore = false;
                        headerBand1.ResetPageNumber = false;
                    }
                    //處理小計
                    if (glar311Model.order1_sum_yn.ToUpper() != "Y")
                    {
                        footerBand1.Enabled = false;
                    }
                }
                else
                    footerBand1.Enabled = false;

                if (!GlobalFn.varIsNull(glar311Model.order2)
                            && (glar311Model.order2_jump_yn == "Y" || glar311Model.order2_sum_yn == "Y"))
                {
                    switch (glar311Model.order2)
                    {
                        case "1":
                            headerBand2.Condition.Value = "{Master.gfa01}";
                            break;
                        case "2":
                            headerBand2.Condition.Value = "{Master.gfa02}";
                            break;
                        case "3":
                            headerBand2.Condition.Value = "{Master.gfb03}";
                            break;
                        case "4":
                            headerBand2.Condition.Value = "{Master.gfb05}";
                            break;
                        case "5":
                            headerBand2.Condition.Value = "{Master.gfb02}";
                            break;
                        case "6":
                            headerBand2.Condition.Value = "{Master.gfa06}";
                            break;
                    }

                    //處理跳頁
                    if (glar311Model.order2_jump_yn.ToUpper() == "Y")
                    {
                        headerBand2.NewPageBefore = true;
                        headerBand2.ResetPageNumber = true;
                    }
                    else
                    {
                        headerBand2.NewPageBefore = false;
                        headerBand2.ResetPageNumber = false;
                    }
                    //處理小計
                    if (glar311Model.order2_sum_yn.ToUpper() != "Y")
                    {
                        footerBand2.Enabled = false;
                    }
                }
                else
                    footerBand2.Enabled = false;
                
                if (!GlobalFn.varIsNull(glar311Model.order3)
                            && (glar311Model.order3_jump_yn == "Y" || glar311Model.order3_sum_yn == "Y"))
                {
                    switch (glar311Model.order3)
                    {
                        case "1":
                            headerBand3.Condition.Value = "{Master.gfa01}";
                            break;
                        case "2":
                            headerBand3.Condition.Value = "{Master.gfa02}";
                            break;
                        case "3":
                            headerBand3.Condition.Value = "{Master.gfb03}";
                            break;
                        case "4":
                            headerBand3.Condition.Value = "{Master.gfb05}";
                            break;
                        case "5":
                            headerBand3.Condition.Value = "{Master.gfb02}";
                            break;
                        case "6":
                            headerBand3.Condition.Value = "{Master.gfa06}";
                            break;
                    }

                    //處理跳頁
                    if (glar311Model.order3_jump_yn.ToUpper() == "Y")
                    {
                        headerBand3.NewPageBefore = true;
                        headerBand3.ResetPageNumber = true;
                    }
                    else
                    {
                        headerBand3.NewPageBefore = false;
                        headerBand3.ResetPageNumber = false;
                    }
                    //處理小計
                    if (glar311Model.order3_sum_yn.ToUpper() != "Y")
                    {
                        footerBand3.Enabled = false;
                    }
                }
                else
                    footerBand3.Enabled = false;
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfExecReportEnd 產生報表完成後
        /// <summary>
        /// 報表額外參數要放在這才會有做用
        /// </summary>
        /// <param name="pReport"></param>
        /// <returns></returns>
        protected override bool WfExecReportEnd(Stimulsoft.Report.StiReport pReport)
        {
            vw_glar311 glar311Model = null;
            try
            {
                glar311Model = DrMaster.ToItem<vw_glar311>();
                if (!GlobalFn.varIsNull(glar311Model.order1))
                {
                    pReport["order1"] = glar311Model.order1;

                    switch (glar311Model.order1)
                    {
                        case "1":
                            pReport["order1_c"] = "傳票編號";
                            break;
                        case "2":
                            pReport["order1_c"] = "傳票日期";
                            break;
                        case "3":
                            pReport["order1_c"] = "科目編號";
                            break;
                        case "4":
                            pReport["order1_c"] = "部門編號";
                            break;
                        case "5":
                            pReport["order1_c"] = "傳票項次";
                            break;
                        case "6":
                            pReport["order1_c"] = "借貸方";
                            break;
                    }
                }
                if (!GlobalFn.varIsNull(glar311Model.order2))
                {
                    pReport["order2"] = glar311Model.order2;

                    switch (glar311Model.order2)
                    {
                        case "1":
                            pReport["order2_c"] = "傳票編號";
                            break;
                        case "2":
                            pReport["order2_c"] = "傳票日期";
                            break;
                        case "3":
                            pReport["order2_c"] = "科目編號";
                            break;
                        case "4":
                            pReport["order2_c"] = "部門編號";
                            break;
                        case "5":
                            pReport["order2_c"] = "傳票項次";
                            break;
                        case "6":
                            pReport["order2_c"] = "借貸方";
                            break;
                    }
                }
                if (!GlobalFn.varIsNull(glar311Model.order3))
                {
                    pReport["order3"] = glar311Model.order3;

                    switch (glar311Model.order3)
                    {
                        case "1":
                            pReport["order3"] = "傳票編號";
                            break;
                        case "2":
                            pReport["order3"] = "傳票日期";
                            break;
                        case "3":
                            pReport["order3"] = "科目編號";
                            break;
                        case "4":
                            pReport["order3"] = "部門編號";
                            break;
                        case "5":
                            pReport["order3"] = "傳票項次";
                            break;
                        case "6":
                            pReport["order3"] = "借貸方";
                            break;
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
    }
}
