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
using YR.ERP.DAL.YRModel.Reports.Inv.Invr520;

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvr520 : YR.ERP.Base.Forms.FrmReportBase
    {

        #region Property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmInvr520()
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
            this.StrFormID = "invr520";
            TabMaster.ReportName = @"\Inv\invr520.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "ipasecu";
            TabMaster.GroupColumn = "ipasecg";
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable, false);
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
            vw_invr520 invr520Model;
            try
            {
                invr520Model = DrMaster.ToItem<vw_invr520>();
                switch (e.Column.ToLower())
                {
                    case "ipa02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(invr520Model.ipa03_e))
                        {
                            if (invr520Model.ipa03_s > invr520Model.ipa03_e)
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
            vw_invr520 invr520Model = null;
            string msg;
            try
            {
                invr520Model = DrMaster.ToItem<vw_invr520>();
                if (
                     GlobalFn.varIsNull(invr520Model.ipa03_s)
                    && GlobalFn.varIsNull(invr520Model.ipa03_e)
                    && GlobalFn.varIsNull(invr520Model.ipa01)
                    && GlobalFn.varIsNull(invr520Model.ipacreu)
                    && GlobalFn.varIsNull(invr520Model.ipacreg)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_ipa01, msg);
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
                    case "ipa01"://盤點單號
                        messageModel.StrMultiColumn = "ipa01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_ipa", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "ipacreu"://業務人員
                        messageModel.IntMaxRow = 999;
                        messageModel.StrMultiColumn = "bec01";
                        WfShowPickUtility("p_bec1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;

                    case "ipacreg"://業務部門
                        messageModel.IntMaxRow = 999;
                        messageModel.StrMultiColumn = "beb01";
                        WfShowPickUtility("p_beb1", messageModel);
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
            vw_invr520 invr520Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            DataTable dtIlaTb;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere,strOrderBy;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            List<Master> resultList = null;
            try
            {
                invr520Model = DrMaster.ToItem<vw_invr520>();
                resultList = new List<Master>();
                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(invr520Model.ipa01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ipa_tb";
                    queryModel.ColumnName = "ipa01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ipa01"].DataType.Name;
                    queryModel.Value = invr520Model.ipa01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(invr520Model.ipacreu))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ipa_tb";
                    queryModel.ColumnName = "ipacreu";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ipacreu"].DataType.Name;
                    queryModel.Value = invr520Model.ipacreu;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(invr520Model.ipacreg))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ipa_tb";
                    queryModel.ColumnName = "ipacreg";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ipacreg"].DataType.Name;
                    queryModel.Value = invr520Model.ipacreg;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(invr520Model.ipa03_s))
                {
                    sbQuerySingle.AppendLine("AND ipa03>=@ipa03_s");
                    sqlParmList.Add(new SqlParameter("@ila02_s", invr520Model.ipa03_s));
                }
                if (!GlobalFn.varIsNull(invr520Model.ipa03_e))
                {
                    sbQuerySingle.AppendLine("AND ipa03<=@ipa03_e");
                    sqlParmList.Add(new SqlParameter("@ipa03_e", invr520Model.ipa03_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"
                              SELECT 
                                ipa01,ipa02,ipa03,ipa04,ipa05,
                                ipa06,ipa07,ipa08,
                                ipb02,ipb03,ipb04,ipb05,
                                ipb06,ipb07,
                                ica02,ica03
                              FROM ipa_tb
                                    LEFT JOIN ipb_tb ON ipa01=ipb01
                                    LEFT JOIN ica_tb ON ipb03=ica01
                              WHERE 1=1
                                ";
                strOrderBy = " ORDER BY ipa01,ipb02";
                dtIlaTb = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere,strOrderBy), sqlParmList.ToArray());
                dtIlaTb.TableName = "Master";
                if (dtIlaTb != null)
                {
                    resultList.AddRange(dtIlaTb.ToList<Master>());
                }

                if (resultList == null || resultList.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    return false;
                }

                //foreach (Master masterModel in resultList)
                //{
                //    //數量處理
                //    masterModel.ilb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.ilb05);//數量
                //    masterModel.ilb15_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.ilb15);//數量
                //}

                pReport.RegData("master", resultList);
                pReport.CacheAllData = true;
                StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                footerBand1.NewPageAfter = true;
                footerBand1.ResetPageNumber = true;
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
