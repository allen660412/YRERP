/* 程式名稱: 退料單憑證列印作業
   系統代號: man312
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
using YR.ERP.DAL.YRModel.Reports.Man.Manr312;
using YR.Util;
using YR.ERP.Base.Forms;

namespace YR.ERP.Forms.Man
{
    public partial class FrmManr312 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region property
        vw_manr312 Vw_Manr312;  //給他窗引用時,預設查詢條件
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmManr312()
        {
            InitializeComponent();
        }

        public FrmManr312(YR.ERP.Shared.UserInfo pUserInfo, vw_manr312 pManr312Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Manr312 = pManr312Model;
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
            this.StrFormID = "manr312";
            TabMaster.ReportName = @"\Man\manr312.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "mgasecu";
            TabMaster.GroupColumn = "mgasecg";
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
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依入庫日期"));
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
                pDr["mga01"] = "";
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
            vw_manr312 manr312Model;
            try
            {
                manr312Model = DrMaster.ToItem<vw_manr312>();
                switch (e.Column.ToLower())
                {
                    case "mga02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(manr312Model.mga02_e))
                        {
                            if (manr312Model.mga02_s > manr312Model.mga02_e)
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
            vw_manr312 manr312Model = null;
            string msg;
            try
            {
                manr312Model = DrMaster.ToItem<vw_manr312>();
                if (GlobalFn.varIsNull(manr312Model.mga01)
                    && GlobalFn.varIsNull(manr312Model.mga02_s)
                    && GlobalFn.varIsNull(manr312Model.mga02_e)
                    && GlobalFn.varIsNull(manr312Model.mga03)
                    && GlobalFn.varIsNull(manr312Model.mga06)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_mga01, msg);
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
                    case "mga01"://退料號
                        messageModel.StrMultiColumn = "mga01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_mga2", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "mga03"://廠商編號
                        messageModel.StrMultiColumn = "pca01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_pca1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;

                    case "mga06"://託工單號
                        messageModel.StrMultiColumn = "mga06";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_mga2", messageModel);
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
            vw_manr312 manr312Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            DataTable dtMaster;
            List<YR.ERP.DAL.YRModel.Reports.Man.Manr312.Master> masterList;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            try
            {
                if (Vw_Manr312 != null) //他窗引用時
                    manr312Model = Vw_Manr312;
                else
                    manr312Model = DrMaster.ToItem<vw_manr312>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(manr312Model.mga01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "mga_tb";
                    queryModel.ColumnName = "mga01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["mga01"].DataType.Name;
                    queryModel.Value = manr312Model.mga01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(manr312Model.mga03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "mga_tb";
                    queryModel.ColumnName = "mga03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["mga03"].DataType.Name;
                    queryModel.Value = manr312Model.mga03;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();

                if (!GlobalFn.varIsNull(manr312Model.mga06))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "mga_tb";
                    queryModel.ColumnName = "mga06";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["mga06"].DataType.Name;
                    queryModel.Value = manr312Model.mga06;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(manr312Model.mga02_s))
                {
                    sbQuerySingle.AppendLine("AND mga02>=@mga02_s");
                    sqlParmList.Add(new SqlParameter("@mga02_s", manr312Model.mga02_s));
                }
                if (!GlobalFn.varIsNull(manr312Model.mga02_e))
                {
                    sbQuerySingle.AppendLine("AND mga02<=@mga02_e");
                    sqlParmList.Add(new SqlParameter("@mga02_e", manr312Model.mga02_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"SELECT 
	                            mga00,mga01,mga02,mga03,mga04,mga05,
	                            mga06,mga07,mga08,
                                bab02 as mga01_c,
                                pca02 as mga03_c,
	                            bec02 as mga04_c,beb02 as mga05_c,
	                            mgb02,mgb03,mgb04,mgb05,mgb06,
	                            mgb07,mgb09,
                                ica.ica03 as ica03,
                                bej.bej03 as bej03,
                                '' as mgb05_str,
                                pcb03,pcb04,pcb06,pcb07
                            FROM mga_tb
	                            LEFT JOIN mgb_tb ON mga01=mgb01
	                            LEFT JOIN baa_tb ON 1=1	
	                            LEFT JOIN bab_tb ON substring(mga01,1,baa06)=bab01
	                            LEFT JOIN pca_tb ON mga03=pca01	--廠商
	                            LEFT JOIN bec_tb ON mga04=bec01	--員工
	                            LEFT JOIN beb_tb ON mga05=beb01	--部門
                                LEFT JOIN ica_tb ica ON mgb03=ica.ica01
                                LEFT JOIN bej_tb bej ON mgb06=bej.bej01
                                LEFT JOIN pcb_tb ON mga03=pcb01 and mga07=pcb02
                            WHERE 1=1 
                                AND mgaconf='Y'
                                AND mga00='2'
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
                    masterModel.mgb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.mgb05);//子件數量
                }

                pReport.RegData(dtMaster);
                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;
                //處理排序
                StiDataBand stiDataBand1 = (StiDataBand)pReport.GetComponents()["DataBand1"];
                switch (manr312Model.order_by)
                {
                    case "1"://1.依入庫日期
                        stiDataBand1.Sort = new string[] { "ASC", "mga02" };
                        break;
                    case "2"://2.依廠商
                        stiDataBand1.Sort = new string[] { "ASC", "mga03" };
                        break;
                }
                //處理跳頁
                StiGroupFooterBand grouperBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                if (manr312Model.jump_yn.ToUpper() == "Y")
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
