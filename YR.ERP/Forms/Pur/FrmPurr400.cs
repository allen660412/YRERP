/* 程式名稱: 入購單憑證列印作業
   系統代號: purr400
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
using YR.ERP.DAL.YRModel.Reports.Pur.Purr400;

namespace YR.ERP.Forms.Pur
{
    public partial class FrmPurr400 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region property
        vw_purr400 Vw_Purr400;  //給他窗引用時,預設查詢條件
        PurBLL BoPur = null;
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmPurr400()
        {
            InitializeComponent();
        }

        public FrmPurr400(YR.ERP.Shared.UserInfo pUserInfo, vw_purr400 pPurr400Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Purr400 = pPurr400Model;
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
            this.StrFormID = "purr400";
            TabMaster.ReportName = @"\Pur\purr400.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "pgasecu";
            TabMaster.GroupColumn = "pgasecu";
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
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依入庫日期"));
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
                pDr["pga01"] = "";
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
            vw_purr400 purr400Model;
            try
            {
                purr400Model = DrMaster.ToItem<vw_purr400>();
                switch (e.Column.ToLower())
                {
                    case "pga02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(purr400Model.pga02_e))
                        {
                            if (purr400Model.pga02_s > purr400Model.pga02_e)
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
            vw_purr400 purr400Model = null;
            string msg;
            try
            {
                purr400Model = DrMaster.ToItem<vw_purr400>();
                if (GlobalFn.varIsNull(purr400Model.pga01)
                    && GlobalFn.varIsNull(purr400Model.pga02_s)
                    && GlobalFn.varIsNull(purr400Model.pga02_e)
                    && GlobalFn.varIsNull(purr400Model.pga03)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_pga01, msg);
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
                    case "pga01"://入庫單號
                        messageModel.StrMultiColumn = "pga01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_pga1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "pga03"://廠商編號
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
            vw_purr400 purr400Model;
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
                if (Vw_Purr400 != null) //他窗引用時
                    purr400Model = Vw_Purr400;
                else
                    purr400Model = DrMaster.ToItem<vw_purr400>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(purr400Model.pga01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "pga_tb";
                    queryModel.ColumnName = "pga01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["pga01"].DataType.Name;
                    queryModel.Value = purr400Model.pga01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(purr400Model.pga03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "pga_tb";
                    queryModel.ColumnName = "pga03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["pga03"].DataType.Name;
                    queryModel.Value = purr400Model.pga03;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(purr400Model.pga02_s))
                {
                    sbQuerySingle.AppendLine("AND pga02>=@pga02_s");
                    sqlParmList.Add(new SqlParameter("@pga02_s", purr400Model.pga02_s));
                }
                if (!GlobalFn.varIsNull(purr400Model.pga02_e))
                {
                    sbQuerySingle.AppendLine("AND pga02<=@pga02_e");
                    sqlParmList.Add(new SqlParameter("@pga02_e", purr400Model.pga02_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"SELECT pga_tb.*,
                                pca02 as pga03_c,
                                bec02 as pga04_c,
                                beb02 as pga05_c,
                                bef03 as pga11_c,
                                pbb02 as pga12_c,
                                bab02 as pga01_c,
                                bea03,bea04,bea05,
                                pgb02,pgb03,pgb04,pgb05,pgb06,
                                pgb09,pgb10,pgb10t,
                                pgb16,
                                ica03,
                                bej03
                            FROM pga_tb                                
                                LEFT JOIN pgb_tb ON pga01=pgb01
	                            LEFT JOIN pca_tb ON pga03=pca01	--廠商
	                            LEFT JOIN bec_tb ON pga04=bec01	--員工
	                            LEFT JOIN beb_tb ON pga05=beb01	--部門
	                            LEFT JOIN bef_tb ON pga11=bef02 AND bef01='1'	--收付款條件
	                            LEFT JOIN pbb_tb ON pga12=pbb01	--採購取價原則
	                            LEFT JOIN baa_tb ON 1=1	
	                            LEFT JOIN bab_tb ON substring(pga01,1,baa06)=bab01
                                LEFT JOIN bea_tb ON pgacomp=beacomp
                                LEFT JOIN ica_tb ON pgb03=ica01
                                LEFT JOIN bej_tb ON pgb06=bej01
                            WHERE 1=1 
                                AND pgaconf='Y'
                           ";
                //處理排序
                switch (purr400Model.order_by)
                {
                    case "1"://1.依出貨日期
                        sqlOrderBy = " ORDER BY pga02";
                        break;
                    case "2"://2.依客戶
                        sqlOrderBy = " ORDER BY pga03";
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
                    if (!GlobalFn.varIsNull(masterModel.pga10))
                    {
                        var bekModel = BoBas.OfGetBekModel(masterModel.pga10);
                        if (bekModel != null)
                        {
                            //單頭
                            masterModel.pga13_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pga13);
                            masterModel.pga13t_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pga13t);
                            masterModel.pga13g_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pga13g);
                            //單身
                            masterModel.pgb09_str = string.Format("{0:N" + bekModel.bek03 + "}", masterModel.pgb09);
                            masterModel.pgb10_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pgb10);
                            masterModel.pgb10t_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pgb10t);
                        }
                    }


                    //送貨地址
                    if (!GlobalFn.varIsNull(masterModel.pga14))
                    {
                        var pccModel = BoPur.OfGetPccModel(masterModel.pga14);
                        if (pccModel != null)
                            masterModel.pga14_c = pccModel.pcc03;
                    }

                    //帳單地址
                    if (!GlobalFn.varIsNull(masterModel.pga15))
                    {
                        var pccModel = BoPur.OfGetPccModel(masterModel.pga15);
                        if (pccModel != null)
                            masterModel.pga15_c = pccModel.pcc03;
                    }

                    masterModel.pgb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.pgb05);//數量

                }

                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;
                ////處理跳頁
                StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                if (purr400Model.jump_yn.ToUpper() == "Y")
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
