/* 程式名稱: 請購單憑證列印作業
   系統代號: purr200
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
using YR.ERP.DAL.YRModel.Reports.Pur.Purr200;

namespace YR.ERP.Forms.Pur
{
    public partial class FrmPurr200 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region property
        vw_purr200 Vw_Purr200;  //給他窗引用時,預設查詢條件
        #endregion

        #region 建構子
        public FrmPurr200()
        {
            InitializeComponent();
        }

        public FrmPurr200(YR.ERP.Shared.UserInfo pUserInfo, vw_purr200 pPurr200Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Purr200 = pPurr200Model;
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
            this.StrFormID = "purr200";
            TabMaster.ReportName = @"\Pur\purr200.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "peasecu";
            TabMaster.GroupColumn = "peasecu";
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
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依請購日期"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.依廠商"));
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
                pDr["pea01"] = "";
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
            vw_purr200 purr200Model;
            try
            {
                purr200Model = DrMaster.ToItem<vw_purr200>();
                switch (e.Column.ToLower())
                {
                    case "pea02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(purr200Model.pea02_e))
                        {
                            if (purr200Model.pea02_s > purr200Model.pea02_e)
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
            vw_purr200 purr200Model = null;
            string msg;
            try
            {
                purr200Model = DrMaster.ToItem<vw_purr200>();
                if (GlobalFn.varIsNull(purr200Model.pea01)
                    && GlobalFn.varIsNull(purr200Model.pea02_s)
                    && GlobalFn.varIsNull(purr200Model.pea02_e)
                    && GlobalFn.varIsNull(purr200Model.pea03)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_pea01, msg);
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
                    case "pea01"://報價單號
                        messageModel.StrMultiColumn = "pea01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_pea1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "pea03"://廠商編號
                        messageModel.StrMultiColumn = "pca01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_pca1", messageModel);
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
            vw_purr200 purr200Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            DataTable dtSeaTb, dtSebTb;
            List<YR.ERP.DAL.YRModel.Reports.Pur.Purr200.Detail> detailList;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            try
            {
                if (Vw_Purr200 != null) //他窗引用時
                    purr200Model = Vw_Purr200;
                else
                    purr200Model = DrMaster.ToItem<vw_purr200>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(purr200Model.pea01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "pea_tb";
                    queryModel.ColumnName = "pea01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["pea01"].DataType.Name;
                    queryModel.Value = purr200Model.pea01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(purr200Model.pea03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "pea_tb";
                    queryModel.ColumnName = "pea03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["pea03"].DataType.Name;
                    queryModel.Value = purr200Model.pea03;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(purr200Model.pea02_s))
                {
                    sbQuerySingle.AppendLine("AND pea02>=@pea02_s");
                    sqlParmList.Add(new SqlParameter("@pea02_s", purr200Model.pea02_s));
                }
                if (!GlobalFn.varIsNull(purr200Model.pea02_e))
                {
                    sbQuerySingle.AppendLine("AND pea02<=@pea02_e");
                    sqlParmList.Add(new SqlParameter("@pea02_e", purr200Model.pea02_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"SELECT pea01,pea02,pea03,pea04,pea05,
                                pea06,pea07,pea08,pea10,
                                pea11,pea12,peacomp,
                                pca02 as pea03_c,
                                bec02 as pea04_c,
                                beb02 as pea05_c,
                                bef03 as pea11_c,
                                pbb02 as pea12_c,
                                bab02 as pea01_c,
                                bea03,bea04,bea05
                            FROM pea_tb                                
	                            LEFT JOIN pca_tb ON pea03=pca01	--廠商
	                            LEFT JOIN bec_tb ON pea04=bec01	--員工
	                            LEFT JOIN beb_tb ON pea05=beb01	--部門
	                            LEFT JOIN bef_tb ON pea11=bef02 AND bef01='1'	--收付款條件
	                            LEFT JOIN pbb_tb ON pea12=pbb01	--採購取價原則
	                            LEFT JOIN baa_tb ON 1=1	
	                            LEFT JOIN bab_tb ON substring(pea01,1,baa06)=bab01
                                LEFT JOIN bea_tb ON peacomp=beacomp
                            WHERE 1=1 
                                AND peaconf='Y'
                           ";
                dtSeaTb = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere), sqlParmList.ToArray());
                dtSeaTb.TableName = "Master";

                if (dtSeaTb == null || dtSeaTb.Rows.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    return false;
                }

                //取得明細
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT ");
                sbSql.AppendLine("  peb01, ");
                sbSql.AppendLine("  peb02,peb03,peb04,peb05,peb06,");
                sbSql.AppendLine("  ica03,bej03,");
                sbSql.AppendLine("  '' AS peb05_str");
                sbSql.AppendLine("FROM peb_tb");
                sbSql.AppendLine("  LEFT JOIN ica_tb ON peb03=ica01");
                sbSql.AppendLine("  LEFT JOIN bej_tb ON peb06=bej01");
                sbSql.AppendLine("WHERE EXISTS(");
                sbSql.AppendLine("  SELECT 1 FROM  pea_tb");
                sbSql.AppendLine("  WHERE pea01=peb01");
                sbSql.AppendLine(strWhere);
                sbSql.AppendLine(")");
                dtSebTb = BoMaster.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                dtSebTb.TableName = "Detail";

                detailList = dtSebTb.ToList<Detail>();
                foreach(Detail detailModel in detailList)
                {
                    detailModel.peb05_str = string.Format("{0:N" + detailModel.bej03 + "}", detailModel.peb05);//數量
                }

                pReport.RegData(dtSeaTb);
                pReport.RegData("Detail",detailList);
                pReport.CacheAllData = true;
                ////處理排序
                //StiDataBand stiDataBand1 = (StiDataBand)pReport.GetComponents()["DataBand1"];
                //switch (purr200Model.order_by)
                //{
                //    case "1"://1.依報價日期
                //        stiDataBand1.Sort = new string[] { "ASC", "pea02" };
                //        break;
                //    case "2"://2.依客戶
                //        stiDataBand1.Sort = new string[] { "ASC", "pea03" };
                //        break;
                //}
                ////處理跳頁
                //StiFooterBand footerBand1 = (StiFooterBand)pReport.GetComponents()["FooterBand1"];
                //if (purr200Model.jump_yn.ToUpper() == "Y")
                //{
                //    footerBand1.NewPageAfter = true;
                //    footerBand1.ResetPageNumber = true;
                //}
                //else
                //{
                //    footerBand1.NewPageAfter = false;
                //    footerBand1.ResetPageNumber = false;
                //}

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
