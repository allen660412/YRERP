/* 程式名稱: 採購單憑證列印作業
   系統代號: purr300
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
using YR.ERP.DAL.YRModel.Reports.Pur.Purr300;
using System.Linq;

namespace YR.ERP.Forms.Pur
{
    public partial class FrmPurr300 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region property
        vw_purr300 Vw_Purr300;  //給他窗引用時,預設查詢條件
        PurBLL BoPur = null;
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmPurr300()
        {
            InitializeComponent();
        }

        public FrmPurr300(YR.ERP.Shared.UserInfo pUserInfo, vw_purr300 pPurr300Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Purr300 = pPurr300Model;
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
            this.StrFormID = "purr300";
            TabMaster.ReportName = @"\Pur\purr300.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "pfasecu";
            TabMaster.GroupColumn = "pfasecu";
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
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依採購日期"));
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
                pDr["pfa01"] = "";
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
            vw_purr300 purr300Model;
            try
            {
                purr300Model = DrMaster.ToItem<vw_purr300>();
                switch (e.Column.ToLower())
                {
                    case "pfa02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(purr300Model.pfa02_e))
                        {
                            if (purr300Model.pfa02_s > purr300Model.pfa02_e)
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
            vw_purr300 purr300Model = null;
            string msg;
            try
            {
                purr300Model = DrMaster.ToItem<vw_purr300>();
                if (GlobalFn.varIsNull(purr300Model.pfa01)
                    && GlobalFn.varIsNull(purr300Model.pfa02_s)
                    && GlobalFn.varIsNull(purr300Model.pfa02_e)
                    && GlobalFn.varIsNull(purr300Model.pfa03)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_pfa01, msg);
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
                    case "pfa01"://報價單號
                        messageModel.StrMultiColumn = "pfa01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_pfa1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "pfa03"://廠商編號
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
            vw_purr300 purr300Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            string sqlOrderBy = "";
            DataTable dtMaster;
            List<YR.ERP.DAL.YRModel.Reports.Pur.Purr300.Master> masterList;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            try
            {
                if (Vw_Purr300 != null) //他窗引用時
                    purr300Model = Vw_Purr300;
                else
                    purr300Model = DrMaster.ToItem<vw_purr300>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(purr300Model.pfa01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "pfa_tb";
                    queryModel.ColumnName = "pfa01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["pfa01"].DataType.Name;
                    queryModel.Value = purr300Model.pfa01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(purr300Model.pfa03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "pfa_tb";
                    queryModel.ColumnName = "pfa03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["pfa03"].DataType.Name;
                    queryModel.Value = purr300Model.pfa03;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(purr300Model.pfa02_s))
                {
                    sbQuerySingle.AppendLine("AND pfa02>=@pfa02_s");
                    sqlParmList.Add(new SqlParameter("@pfa02_s", purr300Model.pfa02_s));
                }
                if (!GlobalFn.varIsNull(purr300Model.pfa02_e))
                {
                    sbQuerySingle.AppendLine("AND pfa02<=@pfa02_e");
                    sqlParmList.Add(new SqlParameter("@pfa02_e", purr300Model.pfa02_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"SELECT pfa_tb.*,
                                pca02 as pfa03_c,
                                bec02 as pfa04_c,
                                beb02 as pfa05_c,
                                bef03 as pfa11_c,
                                pbb02 as pfa12_c,
                                bab02 as pfa01_c,
                                bea03,bea04,bea05,
                                pfb02,pfb03,pfb04,pfb05,pfb06,
                                pfb09,pfb10,pfb10t,
                                pfb16,
                                ica03,
                                bej03
                            FROM pfa_tb                                
                                LEFT JOIN pfb_tb ON pfa01=pfb01
	                            LEFT JOIN pca_tb ON pfa03=pca01	--廠商
	                            LEFT JOIN bec_tb ON pfa04=bec01	--員工
	                            LEFT JOIN beb_tb ON pfa05=beb01	--部門
	                            LEFT JOIN bef_tb ON pfa11=bef02 AND bef01='1'	--收付款條件
	                            LEFT JOIN pbb_tb ON pfa12=pbb01	--採購取價原則
	                            LEFT JOIN baa_tb ON 1=1	
	                            LEFT JOIN bab_tb ON substring(pfa01,1,baa06)=bab01
                                LEFT JOIN bea_tb ON pfacomp=beacomp
                                LEFT JOIN ica_tb ON pfb03=ica01
                                LEFT JOIN bej_tb ON pfb03=bej01
                            WHERE 1=1 
                                AND pfaconf='Y'
                           ";
                //處理排序
                switch (purr300Model.order_by)
                {
                    case "1"://1.依出貨日期
                        sqlOrderBy = " ORDER BY pfa02,pfb02";
                        break;
                    case "2"://2.依客戶
                        sqlOrderBy = " ORDER BY pfa03,pfb02";
                        break;
                }
                dtMaster = BoMaster.OfGetDataTable(string.Concat(sqlBody, strWhere, sqlOrderBy), sqlParmList.ToArray());
                dtMaster.TableName = "Master";

                if (dtMaster == null || dtMaster.Rows.Count == 0)
                {
                    WfShowErrorMsg("查無資料,請重新過濾條件!");
                    return false;
                }
                masterList = dtMaster.ToList<Master>(true).OrderBy(o=>o.pfb02).ToList();
                foreach (Master masterModel in masterList)
                {
                    //處理金額
                    if (!GlobalFn.varIsNull(masterModel.pfa10))
                    {
                        var bekModel = BoBas.OfGetBekModel(masterModel.pfa10);
                        if (bekModel != null)
                        {
                            //單頭
                            masterModel.pfa13_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pfa13);
                            masterModel.pfa13t_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pfa13t);
                            masterModel.pfa13g_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pfa13g);
                            //單身
                            masterModel.pfb09_str = string.Format("{0:N" + bekModel.bek03 + "}", masterModel.pfb09);
                            masterModel.pfb10_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pfb10);
                            masterModel.pfb10t_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.pfb10t);
                        }
                    }


                    //送貨地址
                    if (!GlobalFn.varIsNull(masterModel.pfa14))
                    {
                        var pccModel = BoPur.OfGetPccModel(masterModel.pfa14);
                        if (pccModel != null)
                            masterModel.pfa14_c = pccModel.pcc03;
                    }

                    //帳單地址
                    if (!GlobalFn.varIsNull(masterModel.pfa15))
                    {
                        var pccModel = BoPur.OfGetPccModel(masterModel.pfa15);
                        if (pccModel != null)
                            masterModel.pfa15_c = pccModel.pcc03;
                    }

                    masterModel.pfb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.pfb05);//數量

                }

                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;
                ////處理跳頁
                StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                if (purr300Model.jump_yn.ToUpper() == "Y")
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
