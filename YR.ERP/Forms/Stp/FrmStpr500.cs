/* 程式名稱: 銷退單憑證列印作業
   系統代號: stpr500
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

namespace YR.ERP.Forms.Stp
{
    public partial class FrmStpr500 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region property
        vw_stpr500 Vw_Stpr500;  //給他窗引用時,預設查詢條件
        #endregion

        #region 建構子
        public FrmStpr500()
        {
            InitializeComponent();
        } 
        
        public FrmStpr500(YR.ERP.Shared.UserInfo pUserInfo, vw_stpr500 pStpr500Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Stpr500 = pStpr500Model;
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
            this.StrFormID = "stpr500";
            TabMaster.ReportName = @"\Stp\stpr500.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "shasecu";
            TabMaster.GroupColumn = "shasecg";
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
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
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依退貨日期"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.依客戶"));
                WfSetUcomboxDataSource(ucb_order_by, sourceList);
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
                pDr["sha01"] = "";
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
            vw_stpr500 stpr500Model;
            try
            {
                stpr500Model = DrMaster.ToItem<vw_stpr500>();
                switch (e.Column.ToLower())
                {
                    case "sha02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(stpr500Model.sha02_e))
                        {
                            if (stpr500Model.sha02_s > stpr500Model.sha02_e)
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
            vw_stpr500 stpr500Model = null;
            string msg;
            try
            {
                
                stpr500Model = DrMaster.ToItem<vw_stpr500>();
                if (GlobalFn.varIsNull(stpr500Model.sha01)
                    && GlobalFn.varIsNull(stpr500Model.sha02_s)
                    && GlobalFn.varIsNull(stpr500Model.sha02_e)
                    && GlobalFn.varIsNull(stpr500Model.sha03)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_sha01, msg);
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
                //this.MsgInfoReturned = new MessageInfo();
                MessageInfo messageModel = new MessageInfo();
                switch (pColName.ToLower())
                {
                    case "sha01"://銷貨單號
                        messageModel.StrMultiColumn = "sha01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_sha1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "sha03"://客戶編號
                        messageModel.StrMultiColumn = "sca01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_sca1", messageModel);
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
            vw_stpr500 stpr500Model;
            StringBuilder sbSql = null;
            DataTable dtSeaTb, dtSebTb;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            try
            {
                if (Vw_Stpr500 != null) //他窗引用時
                    stpr500Model = Vw_Stpr500;
                else
                    stpr500Model = DrMaster.ToItem<vw_stpr500>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(stpr500Model.sha01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "sha_tb";
                    queryModel.ColumnName = "sha01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["sha01"].DataType.Name;
                    queryModel.Value = stpr500Model.sha01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(stpr500Model.sha03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "sha_tb";
                    queryModel.ColumnName = "sha03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["sha03"].DataType.Name;
                    queryModel.Value = stpr500Model.sha03;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(stpr500Model.sha02_s))
                {
                    sbQuerySingle.AppendLine("AND sha02>=@sha02_s");
                    sqlParmList.Add(new SqlParameter("@sha02_s", stpr500Model.sha02_s));
                }
                if (!GlobalFn.varIsNull(stpr500Model.sha02_e))
                {
                    sbQuerySingle.AppendLine("AND sha02<=@sha02_e");
                    sqlParmList.Add(new SqlParameter("@sha02_e", stpr500Model.sha02_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT sha_tb.*,");
                sbSql.AppendLine("  sca03 AS sha03_c,");
                sbSql.AppendLine("  bec02 AS sha04_c,");
                sbSql.AppendLine("  beb02 AS sha05_c,");
                sbSql.AppendLine("  bab02 AS sha01_c,");
                sbSql.AppendLine("  bea04,");
                sbSql.AppendLine("  bea05,");
                sbSql.AppendLine("  bea06");
                sbSql.AppendLine("FROM sha_tb");
                sbSql.AppendLine("  LEFT JOIN sca_tb ON sha03=sca01");
                sbSql.AppendLine("  LEFT JOIN bec_tb ON sha04=bec01");
                sbSql.AppendLine("  LEFT JOIN beb_tb ON sha05=beb01");
                sbSql.AppendLine("  LEFT JOIN baa_tb ON 1=1");
                sbSql.AppendLine("  LEFT JOIN bab_tb ON substring(sha01,1,baa06)=bab01");
                sbSql.AppendLine("  LEFT JOIN bea_tb ON beacomp=shacomp");
                sbSql.AppendLine("WHERE 1=1");
                sbSql.AppendLine("  AND shaconf='Y'");
                dtSeaTb = BoMaster.OfGetDataTable(string.Concat(sbSql.ToString(), strWhere), sqlParmList.ToArray());
                dtSeaTb.TableName = "Master";

                if (dtSeaTb == null || dtSeaTb.Rows.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    return false;
                }

                //取得明細
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM shb_tb");
                sbSql.AppendLine("WHERE EXISTS(");
                sbSql.AppendLine("  SELECT 1 FROM  sha_tb");
                sbSql.AppendLine("  WHERE sha01=shb01");
                sbSql.AppendLine(strWhere);
                sbSql.AppendLine(")");
                dtSebTb = BoMaster.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                dtSebTb.TableName = "Detail";

                pReport.RegData(dtSeaTb);
                pReport.RegData(dtSebTb);                
                pReport.CacheAllData = true;
                //處理排序
                StiDataBand stiDataBand1 = (StiDataBand)pReport.GetComponents()["DataBand1"];
                switch (stpr500Model.order_by)
                {
                    case "1"://1.依出貨日期
                        stiDataBand1.Sort = new string[] { "ASC", "sha02" };
                        break;
                    case "2"://2.依客戶
                        stiDataBand1.Sort = new string[] { "ASC", "sha03" };
                        break;
                }
                //處理跳頁
                StiFooterBand footerBand1 = (StiFooterBand)pReport.GetComponents()["FooterBand1"];
                if (stpr500Model.jump_yn.ToUpper() == "Y")
                {
                    footerBand1.NewPageAfter = true;
                    footerBand1.ResetPageNumber = true;
                }
                else
                {
                    footerBand1.NewPageAfter = false;
                    footerBand1.ResetPageNumber = false;
                }

                //pReport.Compile();                
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
