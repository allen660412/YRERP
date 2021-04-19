/* 程式名稱: 借出歸還單憑證列印
   系統代號: invr401
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
using YR.ERP.DAL.YRModel.Reports.Inv.Invr402;

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvr402 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region property
        vw_invr402 Vw_Invr402;  //給他窗引用時,預設查詢條件
        #endregion

        #region 建構子
        public FrmInvr402()
        {
            InitializeComponent();
        }

        public FrmInvr402(YR.ERP.Shared.UserInfo pUserInfo, vw_invr402 pInvr402Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Invr402 = pInvr402Model;
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
            this.StrFormID = "invr402";
            TabMaster.ReportName = @"\Inv\invr402.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "imasecu";
            TabMaster.GroupColumn = "imasecg";
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
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依歸還日期"));
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
                pDr["ima01"] = "";
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
            vw_invr402 invr402Model;
            try
            {
                invr402Model = DrMaster.ToItem<vw_invr402>();
                switch (e.Column.ToLower())
                {
                    case "ima02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(invr402Model.ima02_e))
                        {
                            if (invr402Model.ima02_s > invr402Model.ima02_e)
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
            vw_invr402 invr402Model = null;
            string msg;
            try
            {

                invr402Model = DrMaster.ToItem<vw_invr402>();
                if (GlobalFn.varIsNull(invr402Model.ima01)
                    && GlobalFn.varIsNull(invr402Model.ima02_s)
                    && GlobalFn.varIsNull(invr402Model.ima02_e)
                    && GlobalFn.varIsNull(invr402Model.ima03)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_ima01, msg);
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
                    case "ima01"://借出單號
                        messageModel.StrMultiColumn = "ima01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_ima1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "ima03"://客戶編號
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
            vw_invr402 invr402Model;
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
                if (Vw_Invr402 != null) //他窗引用時
                    invr402Model = Vw_Invr402;
                else
                    invr402Model = DrMaster.ToItem<vw_invr402>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(invr402Model.ima01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ima_tb";
                    queryModel.ColumnName = "ima01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ima01"].DataType.Name;
                    queryModel.Value = invr402Model.ima01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(invr402Model.ima03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ima_tb";
                    queryModel.ColumnName = "ima03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ima03"].DataType.Name;
                    queryModel.Value = invr402Model.ima03;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(invr402Model.ima02_s))
                {
                    sbQuerySingle.AppendLine("AND ima02>=@ima02_s");
                    sqlParmList.Add(new SqlParameter("@ima02_s", invr402Model.ima02_s));
                }
                if (!GlobalFn.varIsNull(invr402Model.ima02_e))
                {
                    sbQuerySingle.AppendLine("AND ima02<=@ima02_e");
                    sqlParmList.Add(new SqlParameter("@ima02_e", invr402Model.ima02_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"SELECT ima_tb.*,
                                sca02 as ima03_c,
                                bec02 as ima04_c,
                                beb02 as ima05_c,
                                bab02 as ima01_c,
                                imb02,imb03,imb04,imb05,imb06,
                                imb11,imb12,imb17,imb18,imb19,
                                bej03
                            FROM ima_tb                                
                                LEFT JOIN imb_tb ON ima01=imb01
	                            LEFT JOIN sca_tb ON ima03=sca01	--客戶
	                            LEFT JOIN bec_tb ON ima04=bec01	--員工
	                            LEFT JOIN beb_tb ON ima05=beb01	--部門
	                            LEFT JOIN baa_tb ON 1=1	
	                            LEFT JOIN bab_tb ON substring(ima01,1,baa06)=bab01
                                LEFT JOIN ica_tb ON imb03=ica01
                                LEFT JOIN bej_tb ON imb03=bej01
                            WHERE 1=1 
                                AND imaconf='Y'
                           ";
                //處理排序
                switch (invr402Model.order_by)
                {
                    case "1"://1.依借出日期
                        sqlOrderBy = " ORDER BY ima02";
                        break;
                    case "2"://2.依客戶
                        sqlOrderBy = " ORDER BY ima03";
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
                    masterModel.imb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.imb05);//數量

                }

                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;
                ////處理跳頁
                StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                if (invr402Model.jump_yn.ToUpper() == "Y")
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
