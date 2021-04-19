/* 程式名稱: 託工進貨單憑證列印作業
   系統代號: manr411
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
using YR.ERP.DAL.YRModel.Reports.Man.Manr411;
using YR.Util;
using YR.ERP.Base.Forms;

namespace YR.ERP.Forms.Man
{
    public partial class FrmManr411 : YR.ERP.Base.Forms.FrmReportBase
    {

        #region property
        vw_manr411 Vw_Manr411;  //給他窗引用時,預設查詢條件
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmManr411()
        {
            InitializeComponent();
        }

        public FrmManr411(YR.ERP.Shared.UserInfo pUserInfo, vw_manr411 pManr411Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Manr411 = pManr411Model;
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
            this.StrFormID = "manr411";
            TabMaster.ReportName = @"\Man\manr411.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "mhasecu";
            TabMaster.GroupColumn = "mhasecg";
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
                pDr["mha01"] = "";
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
            vw_manr411 manr411Model;
            try
            {
                manr411Model = DrMaster.ToItem<vw_manr411>();
                switch (e.Column.ToLower())
                {
                    case "mha02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(manr411Model.mha02_e))
                        {
                            if (manr411Model.mha02_s > manr411Model.mha02_e)
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
            vw_manr411 manr411Model = null;
            string msg;
            try
            {
                manr411Model = DrMaster.ToItem<vw_manr411>();
                if (GlobalFn.varIsNull(manr411Model.mha01)
                    && GlobalFn.varIsNull(manr411Model.mha02_s)
                    && GlobalFn.varIsNull(manr411Model.mha02_e)
                    && GlobalFn.varIsNull(manr411Model.mha03)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_mha01, msg);
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
                    case "mha01"://報價單號
                        messageModel.StrMultiColumn = "mha01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_mha2", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "mha03"://廠商編號
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
            vw_manr411 manr411Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            DataTable dtMaster;
            List<YR.ERP.DAL.YRModel.Reports.Man.Manr411.Master> masterList;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            try
            {
                if (Vw_Manr411 != null) //他窗引用時
                    manr411Model = Vw_Manr411;
                else
                    manr411Model = DrMaster.ToItem<vw_manr411>();


                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(manr411Model.mha01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "mha_tb";
                    queryModel.ColumnName = "mha01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["mha01"].DataType.Name;
                    queryModel.Value = manr411Model.mha01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(manr411Model.mha03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "mha_tb";
                    queryModel.ColumnName = "mha03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["mha03"].DataType.Name;
                    queryModel.Value = manr411Model.mha03;
                    queryInfoList.Add(queryModel);
                }
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(manr411Model.mha02_s))
                {
                    sbQuerySingle.AppendLine("AND mha02>=@mha02_s");
                    sqlParmList.Add(new SqlParameter("@mha02_s", manr411Model.mha02_s));
                }
                if (!GlobalFn.varIsNull(manr411Model.mha02_e))
                {
                    sbQuerySingle.AppendLine("AND mha02<=@mha02_e");
                    sqlParmList.Add(new SqlParameter("@mha02_e", manr411Model.mha02_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"SELECT 
	                            mha00,mha01,mha02,mha03,mha04,mha05,
	                            mha06,mha07,mha08,mha09,mha10,
	                            mha11,mha12,mha13,mha13t,mha13g,
	                            mha14,
                                bab02 as mha01_c,
                                pca02 as mha03_c,
                                bec02 as mha04_c,beb02 as mha05_c,
                                bef03 as mha11_c,
                                '' as mha13_str,'' as mha13t_str,'' as mha13g_str,
	                            mhb02,mhb03,mhb04,mhb05,mhb06,
	                            mhb07,mhb08,mhb09,mhb10,mhb10t,
	                            mhb11,
	                            ica03,bej03,'' AS mhb05_str
                            FROM dbo.mha_tb
	                            LEFT JOIN dbo.mhb_tb ON mha01=mhb01
	                            LEFT JOIN pca_tb ON mha03=pca01	--廠商
	                            LEFT JOIN bec_tb ON mha04=bec01	--員工
	                            LEFT JOIN beb_tb ON mha05=beb01	--部門
	                            LEFT JOIN bef_tb ON mha11=bef02 AND bef01='1'	--收付款條件
	                            LEFT JOIN baa_tb ON 1=1	
	                            LEFT JOIN bab_tb ON substring(mha01,1,baa06)=bab01
                                LEFT JOIN ica_tb ON mhb03=ica01
                                LEFT JOIN bej_tb ON mhb06=bej01
                            WHERE 1=1 
                                AND mhaconf='Y'
                                AND mha00='2'
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
                    masterModel.mhb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.mhb05);//子件數量

                    var bekModel = BoBas.OfGetBekModel(masterModel.mha10);
                    masterModel.mha13_str = string.Format("{0:N" + bekModel.bek03 + "}", masterModel.mha13);
                    masterModel.mha13t_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.mha13t);
                    masterModel.mha13g_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.mha13g);
                }

                pReport.RegData(dtMaster);
                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;
                //處理排序
                StiDataBand stiDataBand1 = (StiDataBand)pReport.GetComponents()["DataBand1"];
                switch (manr411Model.order_by)
                {
                    case "1"://1.依入庫日期
                        stiDataBand1.Sort = new string[] { "ASC", "mha02" };
                        break;
                    case "2"://2.依廠商
                        stiDataBand1.Sort = new string[] { "ASC", "mha03" };
                        break;
                }
                
                //處理跳頁
                StiGroupFooterBand grouperBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                if (manr411Model.jump_yn.ToUpper() == "Y")
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
