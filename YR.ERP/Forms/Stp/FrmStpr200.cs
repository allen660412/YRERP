/* 程式名稱: 報價單憑證列印作業
   系統代號: stpr200
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
    public partial class FrmStpr200 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region property
        vw_stpr200 Vw_Stpr200;  //給他窗引用時,預設查詢條件
        #endregion

        #region 建構子
        public FrmStpr200()
        {
            InitializeComponent();
        }

        public FrmStpr200(YR.ERP.Shared.UserInfo pUserInfo,vw_stpr200 pStpr200Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Stpr200 = pStpr200Model;
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
            this.StrFormID = "stpr200";
            TabMaster.ReportName = @"\Stp\stpr200.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "seasecu";
            TabMaster.GroupColumn = "seasecg";
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
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依報價日期"));
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
                pDr["sea01"] = "";
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
            vw_stpr200 stpr200Model;
            try
            {
                stpr200Model = DrMaster.ToItem<vw_stpr200>();
                switch (e.Column.ToLower())
                {
                    case "sea02_s":
                        if (!GlobalFn.varIsNull(e.Value)&&!GlobalFn.varIsNull(stpr200Model.sea02_e))
                        {
                            if (stpr200Model.sea02_s>stpr200Model.sea02_e)
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
            vw_stpr200 stpr200Model = null;
            string msg;
            try
            {
                stpr200Model = DrMaster.ToItem<vw_stpr200>();
                if (GlobalFn.varIsNull(stpr200Model.sea01)
                    && GlobalFn.varIsNull(stpr200Model.sea02_s)
                    && GlobalFn.varIsNull(stpr200Model.sea02_e)
                    && GlobalFn.varIsNull(stpr200Model.sea03)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_sea01, msg);
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
                    case "sea01"://報價單號
                        messageModel.StrMultiColumn = "sea01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_sea1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "sea03"://客戶編號
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
            vw_stpr200 stpr200Model;
            StringBuilder sbSql = null;
            DataTable dtSeaTb, dtSebTb;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange,strWhere;
            StringBuilder sbQuerySingle = null;
             
            List<SqlParameter> sqlParmList;
            try
            {
                if (Vw_Stpr200 != null) //他窗引用時
                    stpr200Model = Vw_Stpr200;
                else
                    stpr200Model = DrMaster.ToItem<vw_stpr200>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(stpr200Model.sea01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "vw_stpt200";
                    queryModel.ColumnName = "sea01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["sea01"].DataType.Name;
                    queryModel.Value = stpr200Model.sea01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(stpr200Model.sea03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "vw_stpt200";
                    queryModel.ColumnName = "sea03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["sea03"].DataType.Name;
                    queryModel.Value = stpr200Model.sea03;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList); 
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(stpr200Model.sea02_s))
                {
                    sbQuerySingle.AppendLine("AND sea02>=@sea02_s");
                    sqlParmList.Add(new SqlParameter("@sea02_s", stpr200Model.sea02_s));
                }
                if (!GlobalFn.varIsNull(stpr200Model.sea02_e))
                {
                    sbQuerySingle.AppendLine("AND sea02<=@sea02_e");
                    sqlParmList.Add(new SqlParameter("@sea02_e", stpr200Model.sea02_e));
                } 
                #endregion
                
                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM vw_stpt200");
                sbSql.AppendLine("WHERE 1=1");
                sbSql.AppendLine("  AND seaconf='Y'");
                dtSeaTb = BoMaster.OfGetDataTable(string.Concat(sbSql.ToString(), strWhere), sqlParmList.ToArray());
                dtSeaTb.TableName = "Master";

                if (dtSeaTb==null||dtSeaTb.Rows.Count==0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    return false;
                }

                //取得明細
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM vw_stpt200s");
                sbSql.AppendLine("WHERE EXISTS(");
                sbSql.AppendLine("  SELECT 1 FROM  vw_stpt200");
                sbSql.AppendLine("  WHERE sea01=seb01");
                sbSql.AppendLine("  AND seaconf='Y'");

                sbSql.AppendLine(strWhere);
                sbSql.AppendLine(")");
                dtSebTb = BoMaster.OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                dtSebTb.TableName = "Detail";

                pReport.RegData(dtSeaTb);
                pReport.RegData(dtSebTb);
                pReport.CacheAllData = true;
                //處理排序
                StiDataBand stiDataBand1 = (StiDataBand)pReport.GetComponents()["DataBand1"];
                switch (stpr200Model.order_by)
                {
                    case "1"://1.依報價日期
                        stiDataBand1.Sort = new string[] { "ASC", "sea02" };
                        break;
                    case "2"://2.依客戶
                        stiDataBand1.Sort = new string[] { "ASC", "sea03" };
                        break;
                }
                //處理跳頁
                StiFooterBand footerBand1 = (StiFooterBand)pReport.GetComponents()["FooterBand1"];
                if (stpr200Model.jump_yn.ToUpper() == "Y")
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
