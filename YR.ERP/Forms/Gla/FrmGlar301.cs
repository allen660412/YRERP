/* 程式名稱: 傳票清單列印
   系統代號: glar301
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
using YR.ERP.DAL.YRModel.Reports.Gla.Glar301;
using Stimulsoft.Report;

namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlar301 : YR.ERP.Base.Forms.FrmReportBase
    {

        #region property
        vw_glar301 Vw_Galr301;  //給他窗引用時,預設查詢條件
        BasBLL BoBas = null;
        #endregion
        

        #region 建構子
        public FrmGlar301()
        {
            InitializeComponent();
        }
        public FrmGlar301(YR.ERP.Shared.UserInfo pUserInfo, vw_glar301 pGlar301Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Galr301 = pGlar301Model;
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
            this.StrFormID = "glar301";
            TabMaster.ReportName = @"\Gla\glar301.mrt";

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
            vw_glar301 glar301Model;
            try
            {
                glar301Model = DrMaster.ToItem<vw_glar301>();
                switch (e.Column.ToLower())
                {
                    case "gfa02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(glar301Model.gfa02_e))
                        {
                            if (glar301Model.gfa02_s > glar301Model.gfa02_e)
                            {
                                WfShowErrorMsg("起日不得大於迄日!");
                                return false;
                            }
                        }
                        break;
                    case "gfa02_e":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(glar301Model.gfa02_s))
                        {
                            if (glar301Model.gfa02_s > glar301Model.gfa02_e)
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
            vw_glar301 glar301Model;
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
                if (Vw_Galr301 != null) //他窗引用時
                    glar301Model = Vw_Galr301;
                else
                    glar301Model = DrMaster.ToItem<vw_glar301>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(glar301Model.gfa01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "gfa_tb";
                    queryModel.ColumnName = "gfa01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["gfa01"].DataType.Name;
                    queryModel.Value = glar301Model.gfa01;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion
                
                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (Vw_Galr301==null)
                {
                    if (!GlobalFn.varIsNull(glar301Model.gfa02_s))
                    {
                        sbQuerySingle.AppendLine("AND gfa02>=@gfa02_s");
                        sqlParmList.Add(new SqlParameter("@gfa02_s", glar301Model.gfa02_s));
                    }
                    if (!GlobalFn.varIsNull(glar301Model.gfa02_e))
                    {
                        sbQuerySingle.AppendLine("AND gfa02<=@gfa02_e");
                        sqlParmList.Add(new SqlParameter("@gfa02_e", glar301Model.gfa02_e));
                    }
                    
                    //過帳否
                    if (glar301Model.gfapost == "1")
                    {
                        sbQuerySingle.AppendLine("AND gfapost='N'");
                    }
                    else if (glar301Model.gfapost == "2")
                    {
                        sbQuerySingle.AppendLine("AND gfapost='Y'");
                    }
                    
                    //有效否
                    if (glar301Model.gfaconf == "1")
                    {
                        sbQuerySingle.AppendLine("AND gfaconf <>'X' ");
                    }
                    else if (glar301Model.gfaconf == "2")
                    {
                        sbQuerySingle.AppendLine("AND gfapost='X' ");
                    }

                    //列印否
                    if (glar301Model.gfaprno == 1)
                    {
                        sbQuerySingle.AppendLine("AND gfaprno =0");
                    }
                    else if (glar301Model.gfapost == "2")
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
                masterList = dtMaster.ToList<Master>(true);
                
                foreach (Master masterModel in masterList)
                {
                    masterModel.gfa03_str = string.Format("{0:N" + bekUsModel.bek04 + "}", masterModel.gfa03);
                    masterModel.gfa04_str = string.Format("{0:N" + bekUsModel.bek04 + "}", masterModel.gfa04);
                    masterModel.gfb07_str = string.Format("{0:N" + bekUsModel.bek04 + "}", masterModel.gfb07);
                    masterModel.gfb10_str = string.Format("{0:N" + masterModel.bek04 + "}", masterModel.gfb10);
                }
                
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


    }
}
