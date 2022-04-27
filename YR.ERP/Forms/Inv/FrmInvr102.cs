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
using YR.ERP.DAL.YRModel.Reports.Inv.Invr102;
using System.Drawing;
using System.IO;

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvr102 : YR.ERP.Base.Forms.FrmReportBase
    {
        
        #region property
        //vw_invr301 Vw_Invr301;  //給他窗引用時,預設查詢條件
        BasBLL BoInv = null;
        #endregion


        public FrmInvr102()
        {
            InitializeComponent();
        }


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
            this.StrFormID = "invr102";
            TabMaster.ReportName = @"\Inv\invr102.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            //TabMaster.UserColumn = "igasecu";
            //TabMaster.GroupColumn = "igasecg";
        }
        #endregion

        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
            BoInv = new BasBLL(BoMaster.OfGetConntion());
            return;

        }
        #endregion

        #region WfBindMaster 設定數據源與組件的 binding
        protected override void WfBindMaster()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                ////調撥類型
                //sourceList = new List<KeyValuePair<string, string>>();
                //sourceList.Add(new KeyValuePair<string, string>("", ""));
                //sourceList.Add(new KeyValuePair<string, string>("1", "1.直接調撥單"));
                //sourceList.Add(new KeyValuePair<string, string>("2", "2.二階段調撥單"));
                //WfSetUcomboxDataSource(ucb_ifa00, sourceList);
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
                //pDr["iga01"] = "";
                //pDr["jump_yn"] = "Y";
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
            vw_invr102 invr102Model;
            try
            {
                invr102Model = DrMaster.ToItem<vw_invr102>();
                switch (e.Column.ToLower())
                {
                    case "ica01":
                        //if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(invr301Model.iga02_s))
                        //{
                        //    if (invr301Model.iga02_s > invr301Model.iga02_e)
                        //    {
                        //        WfShowErrorMsg("起日不得大於迄日!");
                        //        return false;
                        //    }
                        //}
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
            vw_invr102 invr102Model = null;
            string msg;
            try
            {

                invr102Model = DrMaster.ToItem<vw_invr102>();
                //if (GlobalFn.varIsNull(invr101Model.ica01)
                //    )
                //{
                //    msg = "請至少輸入一個條件!";
                //    WfShowErrorMsg(msg);
                //    errorProvider.SetError(ute_ica01, msg);
                //    return false;
                //}

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
                    case "ica01":
                        messageModel.StrMultiColumn = "ica01";
                        messageModel.IntMaxRow = 999;
                        //messageModel.StrWhereAppend = " AND iga00='1'";
                        WfShowPickUtility("p_ica1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] += messageModel.StrMultiRtn;
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
            vw_invr102 invr102Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            string sqlOrderBy = "";
            DataTable dtMaster;
            List<Master> masterList;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;
            object icp03;

            string selectSql = "";
            List<SqlParameter> sqlParmList;
            try
            {
                //if (Vw_Invr301 != null) //他窗引用時
                //    invr301Model = Vw_Invr301;
                //else
                //    invr301Model = DrMaster.ToItem<vw_invr301>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                invr102Model = DrMaster.ToItem<vw_invr102>();
                if (!GlobalFn.varIsNull(invr102Model.ica01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "ica_tb";
                    queryModel.ColumnName = "ica01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["ica01"].DataType.Name;
                    queryModel.Value = invr102Model.ica01;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion


                strWhere = strQueryRange;
                //var strSecurity = WfGetSecurityString();        //取得權限字串
                //if (!GlobalFn.varIsNull(strSecurity))
                //    strWhere += strSecurity;

                //取得單頭
                //sqlBody = @"SELECT V.ica01,v.ica02,v.ica11,v.icc05_tot
                sqlBody = @"SELECT V.*
                            FROM 
                            (
	                            SELECT a.*,
		                            (
		                            SELECT SUM(icc05) as icc05_tot
		                            FROM icc_tb p
		                            WHERE p.icc01=a.ica01
		                            ) AS icc05_tot
	                            FROM ica_tb a
	                            WHERE 
		                            LEN(a.ica01)=11
                                    AND a.icaconf='Y'
                                    AND ISNULL(a.ica30,'') <>'Y'
                            ) V
                            WHERE
                            V.icc05_tot>0
                            order by 2,1
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
                    selectSql = @"
                            SELECT TOP 1 icp03
                            FROM icp_tb
                            WHERE icp01=@icp01
                            ORDER BY ISNULL(icp06,'N') DESC,
                                     icp05
                           ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@icp01", masterModel.ica01));

                    icp03 = BoInv.OfGetFieldValue(selectSql, sqlParmList.ToArray());
                    if (icp03 != null && icp03!=DBNull.Value)
                    {
                        masterModel.icp03 = Image.FromStream(new MemoryStream((byte[])icp03)); ;                        
                    }
                    masterModel.ica03 = masterModel.ica02;
                }

                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;
                ////處理跳頁
                //StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                //if (invr101Model.jump_yn.ToUpper() == "Y")
                //{
                //    footerBand1.NewPageAfter = true;
                //    footerBand1.ResetPageNumber = true;
                //}
                //else
                //{
                //    footerBand1.NewPageAfter = false;
                //    footerBand1.ResetPageNumber = false;
                //}

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
