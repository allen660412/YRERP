using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.DAL.YRModel;
using YR.Util;
using YR.ERP.Base.Forms;
using System.Data.SqlClient;
using YR.ERP.DAL.YRModel.Reports.Csp.Cspr110;
using Stimulsoft.Report.Components;
using bpac;

namespace YR.ERP.Forms.Csp
{
    public partial class FrmCspr110 : YR.ERP.Base.Forms.FrmReportBase
    {

        #region property
        vw_invr302 Vw_Invr302;  //給他窗引用時,預設查詢條件
        BasBLL BoBas = null;
        InvBLL BoInv = null;
        #endregion

        #region 建構子
        public FrmCspr110()
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
            this.StrFormID = "cspr110";
            TabMaster.ReportName = @"\Csp\cspr110.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            //TabMaster.UserColumn = "igasecu";
            //TabMaster.GroupColumn = "igasecg";
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
            BoBas = new BasBLL(BoMaster.OfGetConntion());
            BoInv = new InvBLL(BoMaster.OfGetConntion());
            return;

        }
        #endregion

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                ////調撥類型
                //sourceList = new List<KeyValuePair<string, string>>();
                //sourceList.Add(new KeyValuePair<string, string>("", ""));
                //sourceList.Add(new KeyValuePair<string, string>("1", "1.直接調撥單"));
                //sourceList.Add(new KeyValuePair<string, string>("2", "2.二階段調撥單"));
                //WfSetUcomboxDataSource(ucb_ifa00, sourceList);
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
                pDr["jja16_s"] = Today;
                pDr["jja16_e"] = Today;
                pDr["icc05_s"] = 0;
                pDr["icc05_e"] = 0;
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
            vw_cspr110 cspr110Model;
            try
            {
                cspr110Model = DrMaster.ToItem<vw_cspr110>();
                switch (e.Column.ToLower())
                {
                    case "iga02_s":
                        //if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(invr302Model.iga02_s))
                        //{
                        //    if (invr302Model.iga02_s > invr302Model.iga02_e)
                        //    {
                        //        WfShowErrorMsg("起日不得大於迄日!");
                        //        return false;
                        //    }
                        //}
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
            vw_cspr110 cspr110Model = null;
            string msg;
            try
            {

                cspr110Model = DrMaster.ToItem<vw_cspr110>();
                if (GlobalFn.varIsNull(cspr110Model.jja16_s)
                    && GlobalFn.varIsNull(cspr110Model.jja16_e)
                    && GlobalFn.varIsNull(cspr110Model.icc05_s)
                    && GlobalFn.varIsNull(cspr110Model.icc05_e)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(udt_jja16_s, msg);
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
                    case "iga01":
                        //messageModel.StrMultiColumn = "iga01";
                        //messageModel.IntMaxRow = 999;
                        //messageModel.StrWhereAppend = " AND iga00='2'";
                        //WfShowPickUtility("p_iga1", messageModel);
                        //if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        //{
                        //    pDr[pColName] = messageModel.StrMultiRtn;
                        //}
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
            vw_cspr110 cspr110Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            string sqlOrderBy = "";
            DataTable dtMaster;
            List<Master> masterList;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange = "", strWhere = "";
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            try
            {
                //if (Vw_Invr302 != null) //他窗引用時
                //    cspr110Model = Vw_Invr302;
                //else
                //    cspr110Model = DrMaster.ToItem<vw_invr302>();

                cspr110Model = DrMaster.ToItem<vw_cspr110>();
                queryInfoList = new List<QueryInfo>();
                #region range 處理
                //if (!GlobalFn.varIsNull(cspr110Model.iga01))
                //{
                //    queryModel = new QueryInfo();
                //    queryModel.TableName = "iga_tb";
                //    queryModel.ColumnName = "iga01";
                //    queryModel.ColumnType = TabMaster.DtSource.Columns["iga01"].DataType.Name;
                //    queryModel.Value = cspr110Model.iga01;
                //    queryInfoList.Add(queryModel);
                //}
                sqlParmList = new List<SqlParameter>();
                //strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                sqlParmList = new List<SqlParameter>();
                if (!GlobalFn.varIsNull(cspr110Model.jja16_s))
                {
                    //sbQuerySingle.AppendLine("AND jja16>=@jja16_s");
                    sqlParmList.Add(new SqlParameter("@jja16_s", cspr110Model.jja16_s));
                }
                if (!GlobalFn.varIsNull(cspr110Model.jja16_e))
                {
                    //sbQuerySingle.AppendLine("AND jja16<=@jja16_e");
                    sqlParmList.Add(new SqlParameter("@jja16_e", cspr110Model.jja16_e));
                }

                if (!GlobalFn.varIsNull(cspr110Model.icc05_s))
                {
                    //sbQuerySingle.AppendLine("AND icc05>=@icc05_s");
                    sqlParmList.Add(new SqlParameter("@icc05_s", cspr110Model.icc05_s));
                }

                if (!GlobalFn.varIsNull(cspr110Model.icc05_e))
                {
                    //sbQuerySingle.AppendLine("AND icc05>=@icc05_e");
                    sqlParmList.Add(new SqlParameter("@icc05_e", cspr110Model.icc05_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"
                            select a.*,b.icc05
                            from 
                            (
	                            select 
	                            jja04,jja05
	                            from jja_tb
	                            where jja23='Y'
                                    AND jja16>=@jja16_s
                                    AND jja16<=@jja16_e
	                            group by jja04,jja05
                            ) a
	                            left join icc_tb b on a.jja04=b.icc01
                            where
	                            icc05>=@icc05_s
                                AND icc05<=@icc05_e
                           ";
                dtMaster = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere, sqlOrderBy), sqlParmList.ToArray());
                dtMaster.TableName = "Master";

                if (dtMaster == null || dtMaster.Rows.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    return false;
                }
                masterList = dtMaster.ToList<Master>(true);
                //foreach (Master masterModel in masterList)
                //{
                //    var bejModel = BoBas.OfGetBejModel(masterModel.igb06);
                //    if (bejModel != null)
                //        masterModel.igb05_str = string.Format("{0:N" + bejModel.bej03 + "}", masterModel.igb05);//數量
                //    else
                //        masterModel.igb05_str = masterModel.igb05.ToString();
                //}

                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;
                //////處理跳頁
                //StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                //if (cspr110Model.jump_yn.ToUpper() == "Y")
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
