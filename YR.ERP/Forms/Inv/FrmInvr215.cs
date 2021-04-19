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
using YR.ERP.DAL.YRModel.Reports.Inv.Invr215;

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvr215 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region Property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmInvr215()
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
            this.StrFormID = "invr215";
            TabMaster.ReportName = @"\Inv\invr215.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            //TabMaster.UserColumn = "seasecu";
            //TabMaster.GroupColumn = "seasecg";
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
                //排序
                //sourceList = new List<KeyValuePair<string, string>>();
                //sourceList.Add(new KeyValuePair<string, string>("1", "1.依入庫日期"));
                //sourceList.Add(new KeyValuePair<string, string>("2", "2.依廠商"));
                //sourceList.Add(new KeyValuePair<string, string>("3", "3.依部門"));
                //sourceList.Add(new KeyValuePair<string, string>("4", "4.入庫入員"));
                //WfSetUcomboxDataSource(ucb_order_by, sourceList);

                ////類別
                //sourceList = new List<KeyValuePair<string, string>>();
                //sourceList.Add(new KeyValuePair<string, string>("0", "0.全部"));
                //sourceList.Add(new KeyValuePair<string, string>("1", "1.入庫"));
                //sourceList.Add(new KeyValuePair<string, string>("2", "2.退庫/折讓"));
                //WfSetUcomboxDataSource(ucb_type, sourceList);
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
                pDr["show_warehouse_yn"] = "Y";
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
            vw_invr215 invr215Model;
            try
            {
                invr215Model = DrMaster.ToItem<vw_invr215>();
                //switch (e.Column.ToLower())
                //{
                //    case "sea02_s":
                //        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(invr215Model.pga02_e))
                //        {
                //            if (invr215Model.pga02_s > invr215Model.pga02_e)
                //            {
                //                WfShowErrorMsg("起日不得大於迄日!");
                //                return false;
                //            }
                //        }
                //        break;
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
                    case "icc01"://料號
                        messageModel.StrMultiColumn = "ica01";
                        messageModel.IntMaxRow = 999;
                        messageModel.StrWhereAppend = " AND icaconf='Y'";
                        WfShowPickUtility("p_ica", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "icc02"://倉庫編號
                        messageModel.StrMultiColumn = "icb01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_icb", messageModel);
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

        #region WfFormCheck() 執行報表前檢查
        protected override bool WfFormCheck()
        {
            vw_invr215 invr215Model = null;
            string msg;
            try
            {
                invr215Model = DrMaster.ToItem<vw_invr215>();
                if (
                     GlobalFn.varIsNull(invr215Model.icc01)
                    && GlobalFn.varIsNull(invr215Model.icc02)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_icc01, msg);
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

        #region WfExecReport 執行報表處理
        protected override bool WfExecReport(Stimulsoft.Report.StiReport pReport)
        {
            //DataSet ds;
            vw_invr215 invr215Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            DataTable dtMaster;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            List<Master> resultList = null;
            try
            {
                invr215Model = DrMaster.ToItem<vw_invr215>();
                resultList = new List<Master>();
                #region 處理類型
                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(invr215Model.icc01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "icc_tb";
                    queryModel.ColumnName = "icc01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["icc01"].DataType.Name;
                    queryModel.Value = invr215Model.icc01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(invr215Model.icc02))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "icc_tb";
                    queryModel.ColumnName = "icc02";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["icc02"].DataType.Name;
                    queryModel.Value = invr215Model.icc02;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion
                sbQuerySingle = new StringBuilder();
                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;
                
                //取得單頭
                if (invr215Model.show_warehouse_yn=="Y")
                {
                    sqlBody = @"
                              SELECT 
                                icc01,icc02,icc03,icc05,
                                ica02,ica03,ica07,
                                bej03,'' as icc05_str
                              FROM icc_tb
                                    LEFT JOIN ica_tb ON icc01=ica01
                                    LEFT JOIN bej_tb ON ica07=bej01
                              WHERE ica05>0
                                ";
                    dtMaster = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere), sqlParmList.ToArray());
                }
                else
                {
                    sqlBody = @"
                              SELECT 
                                icc01,'' AS icc02,icc03,sum(icc05) as icc05,
                                ica02,ica03,ica07,
                                bej03,'' as icc05_str
                              FROM icc_tb
                                    LEFT JOIN ica_tb ON icc01=ica01
                                    LEFT JOIN bej_tb ON ica07=bej01
                              WHERE ica05>0
                                ";
                    var sqlGroupBy = @"
                              GROUP BY
                                icc01,icc03,
                                ica02,ica03,ica07,
                                bej03";
                    dtMaster = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere,sqlGroupBy), sqlParmList.ToArray());
                }

                dtMaster.TableName = "Master";
                if (dtMaster != null)
                {
                    resultList.AddRange(dtMaster.ToList<Master>());
                }
                #endregion

                if (resultList == null || resultList.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    return false;
                }

                foreach (Master masterModel in resultList)
                {
                    //數量處理
                    masterModel.icc05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.icc05);//數量
                }

                pReport.RegData("master", resultList);
                pReport.CacheAllData = true;
                pReport.Compile();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfExecReportEnd 產生報表完成後
        protected override bool WfExecReportEnd(Stimulsoft.Report.StiReport pReport)
        {
            vw_invr215 invr215Model;
            try
            {
                invr215Model = DrMaster.ToItem<vw_invr215>();
                pReport["show_warehouse_yn"] = invr215Model.show_warehouse_yn;
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
