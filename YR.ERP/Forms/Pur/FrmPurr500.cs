/* 程式名稱: 退回單憑證列印作業
   系統代號: purr500
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
using YR.ERP.DAL.YRModel.Reports.Pur.Purr500;

namespace YR.ERP.Forms.Pur
{
    public partial class FrmPurr500 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region property
        vw_purr500 Vw_Purr500;  //給他窗引用時,預設查詢條件
        PurBLL BoPur = null;
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmPurr500()
        {
            InitializeComponent();
        }

        public FrmPurr500(YR.ERP.Shared.UserInfo pUserInfo, vw_purr500 pPurr500Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Purr500 = pPurr500Model;
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
            this.StrFormID = "purr500";
            TabMaster.ReportName = @"\Pur\purr500.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "phasecu";
            TabMaster.GroupColumn = "phasecu";
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
            BoBas = new BasBLL(BoMaster.OfGetConntion());
            BoPur = new PurBLL(BoMaster.OfGetConntion());
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
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依退庫日期"));
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
                pDr["pha01"] = "";
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
            vw_purr500 purr500Model;
            try
            {
                purr500Model = DrMaster.ToItem<vw_purr500>();
                switch (e.Column.ToLower())
                {
                    case "pha02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(purr500Model.pha02_e))
                        {
                            if (purr500Model.pha02_s > purr500Model.pha02_e)
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
            vw_purr500 purr500Model = null;
            string msg;
            try
            {
                purr500Model = DrMaster.ToItem<vw_purr500>();
                if (GlobalFn.varIsNull(purr500Model.pha01)
                    && GlobalFn.varIsNull(purr500Model.pha02_s)
                    && GlobalFn.varIsNull(purr500Model.pha02_e)
                    && GlobalFn.varIsNull(purr500Model.pha03)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_pha01, msg);
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
                    case "pha01"://退回單號
                        messageModel.StrMultiColumn = "pha01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_pha1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "pha03"://廠商編號
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
            vw_purr500 purr500Model;
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
                if (Vw_Purr500 != null) //他窗引用時
                    purr500Model = Vw_Purr500;
                else
                    purr500Model = DrMaster.ToItem<vw_purr500>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(purr500Model.pha01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "pha_tb";
                    queryModel.ColumnName = "pha01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["pha01"].DataType.Name;
                    queryModel.Value = purr500Model.pha01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(purr500Model.pha03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "pha_tb";
                    queryModel.ColumnName = "pha03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["pha03"].DataType.Name;
                    queryModel.Value = purr500Model.pha03;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(purr500Model.pha02_s))
                {
                    sbQuerySingle.AppendLine("AND pha02>=@pha02_s");
                    sqlParmList.Add(new SqlParameter("@pha02_s", purr500Model.pha02_s));
                }
                if (!GlobalFn.varIsNull(purr500Model.pha02_e))
                {
                    sbQuerySingle.AppendLine("AND pha02<=@pha02_e");
                    sqlParmList.Add(new SqlParameter("@pha02_e", purr500Model.pha02_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"SELECT pha_tb.*,
                                pca02 as pha03_c,
                                bec02 as pha04_c,
                                beb02 as pha05_c,
                                bab02 as pha01_c,
                                bea03,bea04,bea05,
                                phb02,phb03,phb04,phb05,phb06,
                                phb09,phb10,phb10t,
                                phb16,
                                ica03,
                                bej03
                            FROM pha_tb                                
                                LEFT JOIN phb_tb ON pha01=phb01
	                            LEFT JOIN pca_tb ON pha03=pca01	--廠商
	                            LEFT JOIN bec_tb ON pha04=bec01	--員工
	                            LEFT JOIN beb_tb ON pha05=beb01	--部門
	                            LEFT JOIN baa_tb ON 1=1	
	                            LEFT JOIN bab_tb ON substring(pha01,1,baa06)=bab01
                                LEFT JOIN bea_tb ON phacomp=beacomp
                                LEFT JOIN ica_tb ON phb03=ica01
                                LEFT JOIN bej_tb ON phb03=bej01
                            WHERE 1=1 
                                AND phaconf='Y'
                           ";
                //處理排序
                switch (purr500Model.order_by)
                {
                    case "1"://1.依出貨日期
                        sqlOrderBy = " ORDER BY pha02";
                        break;
                    case "2"://2.依客戶
                        sqlOrderBy = " ORDER BY pha03";
                        break;
                }
                dtMaster = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere, sqlOrderBy), sqlParmList.ToArray());
                dtMaster.TableName = "Master";

                if (dtMaster == null || dtMaster.Rows.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    return false;
                }
                masterList = dtMaster.ToList<Master>(true);
                foreach (Master masterModel in masterList)
                {
                    //處理金額
                    if (!GlobalFn.varIsNull(masterModel.pha10))
                    {
                        var bekModel = BoBas.OfGetBekModel(masterModel.pha10);
                        if (bekModel != null)
                        {
                            //單頭
                            masterModel.pha13_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pha13);
                            masterModel.pha13t_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pha13t);
                            masterModel.pha13g_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pha13g);
                            //單身
                            masterModel.phb09_str = string.Format("{0:N" + bekModel.bek03 + "}", masterModel.phb09);
                            masterModel.phb10_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.phb10);
                            masterModel.phb10t_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.phb10t);
                        }
                    }

                    masterModel.phb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.phb05);//數量

                }

                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;
                ////處理跳頁
                StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                if (purr500Model.jump_yn.ToUpper() == "Y")
                {
                    footerBand1.NewPageAfter = true;
                    footerBand1.ResetPageNumber = true;
                }
                else
                {
                    footerBand1.NewPageAfter = false;
                    footerBand1.ResetPageNumber = false;
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
