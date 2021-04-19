/* 程式名稱: 傳票憑證列印
   系統代號: glar300
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
using YR.ERP.DAL.YRModel.Reports.Gla.Glar300;
using Stimulsoft.Report;


namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlar300 : YR.ERP.Base.Forms.FrmReportBase
    {

        #region property
        vw_glar300 Vw_Galr300;  //給他窗引用時,預設查詢條件
        BasBLL BoBas = null;
        GlaBLL BoGla = null;
        #endregion

        #region 建構子
        public FrmGlar300()
        {
            InitializeComponent();
        }

        public FrmGlar300(YR.ERP.Shared.UserInfo pUserInfo, vw_glar300 pGlar300Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Galr300 = pGlar300Model;
            this.TabMaster.IsCloseAfterExecuted = pCloseAfterExecute;
            this.TabMaster.IsAutoExecuted = pAutoExecuted;
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
            this.StrFormID = "glar300";
            TabMaster.ReportName = @"\Gla\glar300.mrt";

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
            BoGla = new GlaBLL(BoMaster.OfGetConntion());
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
                WfSetUcomboxDataSource(ucb_gfaconf, sourceList);

                //列印否
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.未列印"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.已列印"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.全部"));
                WfSetUcomboxDataSource(ucb_gfaprno, sourceList);
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
                pDr["gfaprno"] = 3;
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
            vw_glar300 glar300Model;
            try
            {
                glar300Model = DrMaster.ToItem<vw_glar300>();
                switch (e.Column.ToLower())
                {
                    case "gfa02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(glar300Model.gfa02_e))
                        {
                            if (glar300Model.gfa02_s > glar300Model.gfa02_e)
                            {
                                WfShowErrorMsg("起日不得大於迄日!");
                                return false;
                            }
                        }
                        break;
                    case "gfa02_e":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(glar300Model.gfa02_s))
                        {
                            if (glar300Model.gfa02_s > glar300Model.gfa02_e)
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
                        messageModel.StrMultiColumn = "ila01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_gfa", messageModel);
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
            vw_glar300 glar300Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            string sqlOrderBy = "";
            DataTable dtMaster;
            List<Master> masterList;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;
            decimal maxPageRows = 5;//每頁限定筆數
            List<Master> addMasterList = null;  //用來增加空白筆數的集合
            
            List<SqlParameter> sqlParmList;
            try
            {
                if (Vw_Galr300 != null) //他窗引用時
                    glar300Model = Vw_Galr300;
                else
                    glar300Model = DrMaster.ToItem<vw_glar300>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(glar300Model.gfa01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "gfa_tb";
                    queryModel.ColumnName = "gfa01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["gfa01"].DataType.Name;
                    queryModel.Value = glar300Model.gfa01;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                //他窗引用時,僅查詢單號
                if (Vw_Galr300==null)
                {
                    if (!GlobalFn.varIsNull(glar300Model.gfa02_s))
                    {
                        sbQuerySingle.AppendLine("AND gfa02>=@gfa02_s");
                        sqlParmList.Add(new SqlParameter("@gfa02_s", glar300Model.gfa02_s));
                    }
                    if (!GlobalFn.varIsNull(glar300Model.gfa02_e))
                    {
                        sbQuerySingle.AppendLine("AND gfa02<=@gfa02_e");
                        sqlParmList.Add(new SqlParameter("@gfa02_e", glar300Model.gfa02_e));
                    }

                    //過帳否
                    if (glar300Model.gfapost == "1")
                    {
                        sbQuerySingle.AppendLine("AND gfapost='N'");
                    }
                    else if (glar300Model.gfapost == "2")
                    {
                        sbQuerySingle.AppendLine("AND gfapost='Y'");
                    }

                    //有效否
                    if (glar300Model.gfaconf == "1")
                    {
                        sbQuerySingle.AppendLine("AND gfaconf <>'X' ");
                    }
                    else if (glar300Model.gfaconf == "2")
                    {
                        sbQuerySingle.AppendLine("AND gfapost='X' ");
                    }

                    //列印否
                    if (glar300Model.gfaprno == 1)
                    {
                        sbQuerySingle.AppendLine("AND gfaprno =0");
                    }
                    else if (glar300Model.gfapost == "2")
                    {
                        sbQuerySingle.AppendLine("AND gfaprno >0 ");
                    }

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
                sqlOrderBy = " ORDER BY gfa01,gfa02";
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
                
                //依群組來處理
                var gfa01GroupList = from o in masterList
                                     group o by o.gfa01 into g
                                     select g.First()
                              ;
                addMasterList = new List<Master>();
                foreach (Master groupModel in gfa01GroupList)
                {
                    var gfa01 = groupModel.gfa01;
                    var groupList = masterList.Where(p => p.gfa01 == gfa01).ToList();
                    int groupTotalRecs = groupList.Count();
                    int groupTotalPages = Convert.ToInt16(Math.Ceiling(Convert.ToDecimal(groupTotalRecs / maxPageRows)));

                    for (int i = 0; i < groupList.Count(); i++)
                    {
                        var masterModel = groupList[i];
                        masterModel.gfa03_str = string.Format("{0:N" + bekUsModel.bek04 + "}", masterModel.gfa03);
                        masterModel.gfa04_str = string.Format("{0:N" + bekUsModel.bek04 + "}", masterModel.gfa04);
                        if (masterModel.gfb06 == "1")
                        {
                            masterModel.gfb07d_str = string.Format("{0:N" + bekUsModel.bek04 + "}", masterModel.gfb07);
                            masterModel.gfb10d_str = string.Format("{0:N" + masterModel.bek04 + "}", masterModel.gfb10);
                        }
                        else
                        {
                            masterModel.gfb07c_str = string.Format("{0:N" + bekUsModel.bek04 + "}", masterModel.gfb07);
                            masterModel.gfb10c_str = string.Format("{0:N" + masterModel.bek04 + "}", masterModel.gfb10);
                        }
                        
                        masterModel.groupPageNo = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal((i+1) / maxPageRows)));
                        masterModel.groupSeqNo = i+1;
                        masterModel.groupTotalPages = groupTotalPages;
                        if (masterModel.groupTotalPages != masterModel.groupPageNo)
                            masterModel.isPageEnd = false;
                        else
                        {
                            masterModel.isPageEnd = true;
                        }
                        if (i == groupTotalRecs-1)
                        {
                            //處理要新增的空白筆數
                            var mod =groupTotalRecs % maxPageRows;
                            if (mod!=0)
                            {
                                var addRecs =  maxPageRows-mod;
                                if (addRecs > 0)
                                {
                                    for (int j = 0; j < addRecs; j++)
                                    {
                                        var addMasterModel = new Master();
                                        addMasterModel.gfa01 = masterModel.gfa01;
                                        addMasterModel.gfa02 = masterModel.gfa02;
                                        addMasterModel.gfa03 = masterModel.gfa03;
                                        addMasterModel.gfa04 = masterModel.gfa04;
                                        addMasterModel.gfa05 = masterModel.gfa05;
                                        addMasterModel.gfa06 = masterModel.gfa06;
                                        addMasterModel.gfa07 = masterModel.gfa07;
                                        addMasterModel.gfa08 = masterModel.gfa08;
                                        addMasterModel.gfa09 = masterModel.gfa09;
                                        addMasterModel.gfaprno = masterModel.gfaprno;
                                        addMasterModel.gfaconf = masterModel.gfaconf;
                                        addMasterModel.gfacond = masterModel.gfacond;
                                        addMasterModel.gfaconu = masterModel.gfaconu;
                                        addMasterModel.gfapost = masterModel.gfapost;
                                        addMasterModel.gfaposd = masterModel.gfaposd;
                                        addMasterModel.gfaposu = masterModel.gfaposu;
                                        addMasterModel.gfa01_c = masterModel.gfa01_c;
                                        addMasterModel.gfb02 = 0;
                                        addMasterModel.gfb03 = "";
                                        addMasterModel.gfb04 = "";
                                        addMasterModel.gfb05 = "";
                                        addMasterModel.gfb06 = "";
                                        addMasterModel.gfb07 = 0;
                                        addMasterModel.gfb08 = "";
                                        addMasterModel.gfb09 = 0;
                                        addMasterModel.gfb10 = 0;
                                        addMasterModel.gfb03_c = "";
                                        addMasterModel.gfb05_c = "";
                                        addMasterModel.bek04 = 0;
                                        addMasterModel.gba05 = "";
                                        addMasterModel.gfa03_str = masterModel.gfa03_str;
                                        addMasterModel.gfa04_str = masterModel.gfa04_str;
                                        addMasterModel.gfb07c_str = "";
                                        addMasterModel.gfb07d_str = "";
                                        addMasterModel.gfb10c_str = "";
                                        addMasterModel.gfb10d_str = "";
                                        addMasterModel.groupPageNo = masterModel.groupPageNo;
                                        addMasterModel.groupTotalPages = masterModel.groupTotalPages;
                                        addMasterModel.groupSeqNo = i + j+2;
                                        addMasterModel.isPageEnd = masterModel.isPageEnd;

                                        addMasterList.Add(addMasterModel);
                                    }
                                }
                            }
                        }
                    }
                }
                
                if (addMasterList != null && addMasterList.Count > 0)
                    masterList.AddRange(addMasterList);
                
                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;
                ////處理跳頁
                //StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                //if (glar300Model.jump_yn.ToUpper() == "Y")
                //{
                //    footerBand1.NewPageAfter = true;
                //    footerBand1.ResetPageNumber = true;
                //}
                //else
                //{
                //    footerBand1.NewPageAfter = false;
                //    footerBand1.ResetPageNumber = false;
                //}
                
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
         		
        #region WfAfterPrint
        protected override void  WfAfterPrint(StiReport pStiReport)
        {
            string updateSql = "";
            List<SqlParameter> sqlParmList = null;
            try
            {
                var masterList = pStiReport.DataStore["Master"].Data as List<Master>;
                List<string> gfa01List = (from o in masterList
                                         group o by o.gfa01 into g
                                         select g.First().gfa01
                                       ).ToList()
                                     ;
                updateSql = @"UPDATE gfa_tb 
                              SET gfaprno=gfaprno+1
                              WHERE gfa01=@gfa01
                            ";
                foreach(string gfa01 in gfa01List)
                {
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@gfa01",gfa01));
                    BoGla.OfExecuteNonquery(updateSql,sqlParmList.ToArray());
                }
                
            }
            catch (Exception ex)
            {
                WfShowErrorMsg(ex.Message);
            }
        }
        #endregion
    }
}
