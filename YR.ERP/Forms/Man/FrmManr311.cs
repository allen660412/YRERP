/* 程式名稱: 送料單憑證列印作業
   系統代號: manr311
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
using YR.ERP.DAL.YRModel.Reports.Man.Manr311;
using YR.Util;
using YR.ERP.Base.Forms;

namespace YR.ERP.Forms.Man
{
    public partial class FrmManr311 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region property
        vw_manr311 Vw_Manr311;  //給他窗引用時,預設查詢條件
        BasBLL BoBas = null;
        #endregion
        #region 建構子
        public FrmManr311()
        {
            InitializeComponent();
        }

        public FrmManr311(YR.ERP.Shared.UserInfo pUserInfo, vw_manr311 pManr311Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Manr311 = pManr311Model;
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
            this.StrFormID = "manr311";
            TabMaster.ReportName = @"\Man\manr311.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "mfasecu";
            TabMaster.GroupColumn = "mfasecg";
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
                //排序
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依出庫日期"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.廠商"));
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
                pDr["mfa01"] = "";
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
            vw_manr311 manr311Model;
            try
            {
                manr311Model = DrMaster.ToItem<vw_manr311>();
                switch (e.Column.ToLower())
                {
                    case "mfa02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(manr311Model.mfa02_e))
                        {
                            if (manr311Model.mfa02_s > manr311Model.mfa02_e)
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
            vw_manr311 manr311Model = null;
            string msg;
            try
            {
                manr311Model = DrMaster.ToItem<vw_manr311>();
                if (GlobalFn.varIsNull(manr311Model.mfa01)
                    && GlobalFn.varIsNull(manr311Model.mfa02_s)
                    && GlobalFn.varIsNull(manr311Model.mfa02_e)
                    && GlobalFn.varIsNull(manr311Model.mfa03)
                    && GlobalFn.varIsNull(manr311Model.mfa06)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_mfa01, msg);
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
                    case "mfa01"://送料單號
                        messageModel.StrMultiColumn = "mfa01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_mfa2", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "mfa03"://廠商編號
                        messageModel.StrMultiColumn = "pca01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_pca1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;

                    case "mfa06"://託工單號
                        messageModel.StrMultiColumn = "mfa06";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_mfa2", messageModel);
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
            vw_manr311 manr311Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            DataTable dtMaster;
            List<YR.ERP.DAL.YRModel.Reports.Man.Manr311.Master> masterList;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;
            
            List<SqlParameter> sqlParmList;
            try
            {                
                if (Vw_Manr311 != null) //他窗引用時
                    manr311Model = Vw_Manr311;
                else
                    manr311Model = DrMaster.ToItem<vw_manr311>();
                
                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(manr311Model.mfa01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "mfa_tb";
                    queryModel.ColumnName = "mfa01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["mfa01"].DataType.Name;
                    queryModel.Value = manr311Model.mfa01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(manr311Model.mfa03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "mfa_tb";
                    queryModel.ColumnName = "mfa03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["mfa03"].DataType.Name;
                    queryModel.Value = manr311Model.mfa03;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();

                if (!GlobalFn.varIsNull(manr311Model.mfa06))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "mfa_tb";
                    queryModel.ColumnName = "mfa06";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["mfa06"].DataType.Name;
                    queryModel.Value = manr311Model.mfa06;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(manr311Model.mfa02_s))
                {
                    sbQuerySingle.AppendLine("AND mfa02>=@mfa02_s");
                    sqlParmList.Add(new SqlParameter("@mfa02_s", manr311Model.mfa02_s));
                }
                if (!GlobalFn.varIsNull(manr311Model.mfa02_e))
                {
                    sbQuerySingle.AppendLine("AND mfa02<=@mfa02_e");
                    sqlParmList.Add(new SqlParameter("@mfa02_e", manr311Model.mfa02_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"SELECT 
	                            mfa00,mfa01,mfa02,mfa03,mfa04,mfa05,
	                            mfa06,mfa07,mfa08,
                                bab02 as mfa01_c,
                                pca02 as mfa03_c,
	                            bec02 as mfa04_c,beb02 as mfa05_c,
	                            mfb02,mfb03,mfb04,mfb05,mfb06,
	                            mfb07,mfb09,
                                ica.ica03 as ica03,
                                bej.bej03 as bej03,
                                '' as mfb05_str,
                                pcb03,pcb04,pcb06,pcb07
                            FROM mfa_tb   
                                LEFT JOIN mfb_tb On mfa01=mfb01
	                            LEFT JOIN baa_tb ON 1=1	
	                            LEFT JOIN bab_tb ON substring(mfa01,1,baa06)=bab01
	                            LEFT JOIN pca_tb ON mfa03=pca01	--廠商
	                            LEFT JOIN bec_tb ON mfa04=bec01	--員工
	                            LEFT JOIN beb_tb ON mfa05=beb01	--部門
                                LEFT JOIN ica_tb ica ON mfb03=ica.ica01
                                LEFT JOIN bej_tb bej ON mfb06=bej.bej01
                                LEFT JOIN pcb_tb ON mfa03=pcb01 and mfa07=pcb02
                            WHERE 1=1 
                                AND mfaconf='Y'
                                AND mfa00='2'
                           ";
                dtMaster = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere), sqlParmList.ToArray());
                dtMaster.TableName = "Master";

                if (dtMaster == null || dtMaster.Rows.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    return false;
                }

                masterList = dtMaster.ToList<Master>();
                foreach (Master masterModel in masterList)
                {
                    masterModel.mfb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.mfb05);//子件數量
                }

                pReport.RegData(dtMaster);
                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;
                //處理排序
                StiDataBand stiDataBand1 = (StiDataBand)pReport.GetComponents()["DataBand1"];
                switch (manr311Model.order_by)
                {
                    case "1"://1.依出庫日期
                        stiDataBand1.Sort = new string[] { "ASC", "mfa02" };
                        break;
                    case "2"://2.依廠商
                        stiDataBand1.Sort = new string[] { "ASC", "mfa03" };
                        break;
                }
                //處理跳頁
                StiGroupFooterBand grouperBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                if (manr311Model.jump_yn.ToUpper() == "Y")
                {
                    grouperBand1.NewPageAfter = true;
                    grouperBand1.ResetPageNumber = true;
                }
                else
                {
                    grouperBand1.NewPageAfter = false;
                    grouperBand1.ResetPageNumber = false;
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
