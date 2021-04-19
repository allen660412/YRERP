/* 程式名稱: 調撥單憑證列印
   系統代號: invr330
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
using YR.ERP.DAL.YRModel.Reports.Inv.Invr330;

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvr330 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region property
        vw_invr330 Vw_Invr330;  //給他窗引用時,預設查詢條件
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmInvr330()
        {
            InitializeComponent();
        }

        public FrmInvr330(YR.ERP.Shared.UserInfo pUserInfo, vw_invr330 pInvr330Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Invr330 = pInvr330Model;
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
            this.StrFormID = "invr330";
            TabMaster.ReportName = @"\Inv\invr330.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "ifasecu";
            TabMaster.GroupColumn = "ifasecg";
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
                //調撥類型
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("", ""));
                sourceList.Add(new KeyValuePair<string, string>("1", "1.直接調撥單"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.二階段調撥單"));
                WfSetUcomboxDataSource(ucb_ifa00, sourceList);
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
                pDr["ifa00"] = "";
                pDr["ifa01"] = "";
                pDr["jump_yn"] = "Y";
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
            vw_invr330 invr330Model;
            try
            {
                invr330Model = DrMaster.ToItem<vw_invr330>();
                switch (e.Column.ToLower())
                {
                    case "ifa02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(invr330Model.ifa02_e))
                        {
                            if (invr330Model.ifa02_s > invr330Model.ifa02_e)
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
            vw_invr330 invr330Model = null;
            string msg;
            try
            {

                invr330Model = DrMaster.ToItem<vw_invr330>();
                if (GlobalFn.varIsNull(invr330Model.ifa01)
                    && GlobalFn.varIsNull(invr330Model.ifa02_s)
                    && GlobalFn.varIsNull(invr330Model.ifa02_e)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_ifa01, msg);
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
                    case "ifa01"://調撥單號
                        messageModel.StrMultiColumn = "ifa01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_ifa1", messageModel);
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
            vw_invr330 invr330Model;
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
                if (Vw_Invr330 != null) //他窗引用時
                    invr330Model = Vw_Invr330;
                else
                    invr330Model = DrMaster.ToItem<vw_invr330>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(invr330Model.ifa01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ifa_tb";
                    queryModel.ColumnName = "ifa01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ifa01"].DataType.Name;
                    queryModel.Value = invr330Model.ifa01;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(invr330Model.ifa02_s))
                {
                    sbQuerySingle.AppendLine("AND ifa02>=@ifa02_s");
                    sqlParmList.Add(new SqlParameter("@ifa02_s", invr330Model.ifa02_s));
                }
                if (!GlobalFn.varIsNull(invr330Model.ifa02_e))
                {
                    sbQuerySingle.AppendLine("AND ifa02<=@ifa02_e");
                    sqlParmList.Add(new SqlParameter("@ifa02_e", invr330Model.ifa02_e));
                }

                if (!GlobalFn.varIsNull(invr330Model.ifa00))
                {
                    sbQuerySingle.AppendLine("AND ifa00=@ifa00");
                    sqlParmList.Add(new SqlParameter("@ifa00", invr330Model.ifa00));

                    //if (invr330Model.ifa00=="2")
                    //{
                    //    sbQuerySingle.AppendLine("AND ifa09='Y'");  //撥入確認
                    //}
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"SELECT ifa_tb.*,
                                bec02 as ifa03_c,
                                beb02 as ifa04_c,
                                bab02 as ifa01_c,
                                ifb02,ifb03,ifb04,ifb05,ifb06,
                                ifb07,ifb08,ifb09,ifb10,ifb13,
                                ica03
                            FROM ifa_tb                                
                                LEFT JOIN ifb_tb ON ifa01=ifb01
	                            LEFT JOIN bec_tb ON ifa03=bec01	--員工
	                            LEFT JOIN beb_tb ON ifa04=beb01	--部門
	                            LEFT JOIN baa_tb ON 1=1	
	                            LEFT JOIN bab_tb ON substring(ifa01,1,baa06)=bab01
                                LEFT JOIN ica_tb ON ifb03=ica01
                            WHERE 1=1 
                                AND ifaconf='Y'
                           ";
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
                    var bejOutModel = BoBas.OfGetBejModel(masterModel.ifb06);
                    if (bejOutModel != null)
                        masterModel.ifb05_str = string.Format("{0:N" + bejOutModel.bej03 + "}", masterModel.ifb05);//數量
                    else
                        masterModel.ifb05_str = masterModel.ifb05.ToString();

                    var bejInModel = BoBas.OfGetBejModel(masterModel.ifb06);
                    if (bejInModel != null)
                        masterModel.ifb05_str = string.Format("{0:N" + bejInModel.bej03 + "}", masterModel.ifb05);//數量
                    else
                        masterModel.ifb05_str = masterModel.ifb05.ToString();
                }

                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;
                ////處理跳頁
                StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                if (invr330Model.jump_yn.ToUpper() == "Y")
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
