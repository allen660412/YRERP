/* 程式名稱: 銷退單憑證列印作業
   系統代號: stpr501
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
    public partial class FrmStpr501 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region property
        vw_stpr501 Vw_Stpr501;  //給他窗引用時,預設查詢條件
        #endregion

        #region 建構子
        public FrmStpr501()
        {
            InitializeComponent();
        }

        public FrmStpr501(YR.ERP.Shared.UserInfo pUserInfo, vw_stpr501 pStpr501Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Stpr501 = pStpr501Model;
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
            this.StrFormID = "stpr501";
            TabMaster.ReportName = @"\Stp\stpr501.mrt";

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
            vw_stpr501 stpr501Model;
            try
            {
                stpr501Model = DrMaster.ToItem<vw_stpr501>();
                switch (e.Column.ToLower())
                {
                    case "sha02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(stpr501Model.sha02_e))
                        {
                            if (stpr501Model.sha02_s > stpr501Model.sha02_e)
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
                        messageModel.StrMultiColumn = "sha01";
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
            vw_stpr501 stpr501Model;
            //StringBuilder sbSql = null;
            string sqlBody;
            string sqlOrderBy="";
            DataTable dtSeaTb;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            try
            {
                if (Vw_Stpr501 != null) //他窗引用時
                    stpr501Model = Vw_Stpr501;
                else
                    stpr501Model = DrMaster.ToItem<vw_stpr501>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(stpr501Model.sha01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "sha_tb";
                    queryModel.ColumnName = "sha01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["sha01"].DataType.Name;
                    queryModel.Value = stpr501Model.sha01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(stpr501Model.sha03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "sha_tb";
                    queryModel.ColumnName = "sha03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["sha03"].DataType.Name;
                    queryModel.Value = stpr501Model.sha03;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(stpr501Model.sha02_s))
                {
                    sbQuerySingle.AppendLine("AND sha02>=@sha02_s");
                    sqlParmList.Add(new SqlParameter("@sha02_s", stpr501Model.sha02_s));
                }
                if (!GlobalFn.varIsNull(stpr501Model.sha02_e))
                {
                    sbQuerySingle.AppendLine("AND sha02<=@sha02_e");
                    sqlParmList.Add(new SqlParameter("@sha02_e", stpr501Model.sha02_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                sqlBody = @"SELECT
                                sha01,sha02,sha03,sca02 as sha03_c,
                                sha06,sha09,
                                sha13,sha13t,sha13g,
                                '' as sha13_str,'' as sha13t_str,'' as sha13g_str,
                                bea01,bea03,bea04,bea05,bea06,
                                sca12,sca17,
                                bek03,bek04,
                                shb02,shb03,shb04,shb05,
                                shb09,shb10,shb10t,
                                shb17,shb19,shb20,
                                bej03,
                                '' as shb05_str,
                                '' as shb09_str,'' as shb10_str,'' as shb10g_str
                            FROM sha_tb 
                                LEFT JOIN shb_tb ON sha01=shb01
                                LEFT JOIN sca_tb ON sha03=sca01
                                LEFT JOIN baa_tb ON 1=1
                                LEFT JOIN bab_tb ON substring(sha01,1,baa06)=bab01
                                LEFT JOIN bea_tb ON beacomp=shacomp
                                LEFT JOIN bek_tb ON sha10=bek01
                                LEFT JOIN bej_tb ON shb06=bej01
                            WHERE 1=1    
                                AND shaconf='Y'
                            ";
                //處理排序
                switch (stpr501Model.order_by)
                {
                    case "1"://1.依出貨日期
                        sqlOrderBy = " ORDER BY sha02";
                        break;
                    case "2"://2.依客戶
                        sqlOrderBy = " ORDER BY sha03";
                        break;
                }

                dtSeaTb = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere, sqlOrderBy), sqlParmList.ToArray());
                dtSeaTb.TableName = "Master";

                if (dtSeaTb == null || dtSeaTb.Rows.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    return false;
                }
                var MasterList=dtSeaTb.ToList<YR.ERP.DAL.YRModel.Reports.Stp.Stpr501.Master>();
                foreach(YR.ERP.DAL.YRModel.Reports.Stp.Stpr501.Master masterModel in MasterList)
                {
                    //處理單頭金額
                    masterModel.sha13_str = string.Format("{0:N" + masterModel.bek04 + "}", masterModel.sha13);
                    masterModel.sha13t_str = string.Format("{0:N" + masterModel.bek04 + "}", masterModel.sha13t);
                    masterModel.sha13g_str = string.Format("{0:N" + masterModel.bek04 + "}", masterModel.sha13g);
                    if (masterModel.shb17=="1")
                    {
                        masterModel.shb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.shb05);//數量
                        masterModel.shb09_str = string.Format("{0:N" + masterModel.bek03 + "}", masterModel.shb09);//單價
                    }
                    else//折讓
                    {
                        masterModel.shb05_str = "";
                        masterModel.shb09_str = "";
                    }
                    masterModel.shb10_str = string.Format("{0:N" + masterModel.bek03 + "}", masterModel.shb10);//未稅
                    masterModel.shb10g_str = string.Format("{0:N" + masterModel.bek03 + "}", masterModel.shb10t - masterModel.shb10);//稅額
                }


                pReport.RegData("Master", MasterList);
                //pReport.RegData(dtSeaTb);
                pReport.CacheAllData = true;
                ////處理跳頁
                StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                if (stpr501Model.jump_yn.ToUpper() == "Y")
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
