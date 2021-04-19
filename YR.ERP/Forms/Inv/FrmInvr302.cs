/* 程式名稱: 庫存異動收料處理作業
   系統代號: invr302
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
using YR.ERP.DAL.YRModel.Reports.Inv.Invr302;

namespace YR.ERP.Forms.Inv
{
    public partial class FrmInvr302 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region property
        vw_invr302 Vw_Invr302;  //給他窗引用時,預設查詢條件
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmInvr302()
        {
            InitializeComponent();
        } 
        
        public FrmInvr302(YR.ERP.Shared.UserInfo pUserInfo, vw_invr302 pInvr302Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();

            this.LoginInfo = pUserInfo;
            this.Vw_Invr302 = pInvr302Model;
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
            this.StrFormID = "invr302";
            TabMaster.ReportName = @"\Inv\invr302.mrt";

            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "igasecu";
            TabMaster.GroupColumn = "igasecg";
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
                pDr["iga01"] = "";
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
            vw_invr302 invr302Model;
            try
            {
                invr302Model = DrMaster.ToItem<vw_invr302>();
                switch (e.Column.ToLower())
                {
                    case "iga02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(invr302Model.iga02_s))
                        {
                            if (invr302Model.iga02_s > invr302Model.iga02_e) 
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
            vw_invr302 invr302Model = null;
            string msg;
            try
            {

                invr302Model = DrMaster.ToItem<vw_invr302>();
                if (GlobalFn.varIsNull(invr302Model.iga01)
                    && GlobalFn.varIsNull(invr302Model.iga02_s)
                    && GlobalFn.varIsNull(invr302Model.iga02_e)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_iga01, msg);
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
                    case "iga01":
                        messageModel.StrMultiColumn = "iga01";
                        messageModel.IntMaxRow = 999;
                        messageModel.StrWhereAppend = " AND iga00='2'";
                        WfShowPickUtility("p_iga1", messageModel);
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
            vw_invr302 invr302Model;
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
                if (Vw_Invr302 != null) //他窗引用時
                    invr302Model = Vw_Invr302;
                else
                    invr302Model = DrMaster.ToItem<vw_invr302>();

                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(invr302Model.iga01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "iga_tb";
                    queryModel.ColumnName = "iga01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["iga01"].DataType.Name;
                    queryModel.Value = invr302Model.iga01;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange =BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(invr302Model.iga02_s))
                {
                    sbQuerySingle.AppendLine("AND iga02>=@iga02_s");
                    sqlParmList.Add(new SqlParameter("@iga02_s", invr302Model.iga02_s));
                }
                if (!GlobalFn.varIsNull(invr302Model.iga02_e))
                {
                    sbQuerySingle.AppendLine("AND iga02<=@iga02_e");
                    sqlParmList.Add(new SqlParameter("@iga02_e", invr302Model.iga02_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"SELECT iga_tb.*,
                                bec02 as iga03_c,
                                beb02 as iga04_c,
                                bab02 as iga01_c,
                                igb02,igb03,igb04,igb05,igb06,
                                igb09,igb10,
                                ica03
                            FROM iga_tb                                
                                LEFT JOIN igb_tb ON iga01=igb01
	                            LEFT JOIN bec_tb ON iga03=bec01	--員工
	                            LEFT JOIN beb_tb ON iga04=beb01	--部門
	                            LEFT JOIN baa_tb ON 1=1	
	                            LEFT JOIN bab_tb ON substring(iga01,1,baa06)=bab01
                                LEFT JOIN ica_tb ON igb03=ica01
                            WHERE 1=1 
                                AND igaconf='Y'
                                AND iga00='2'
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
                    var bejModel = BoBas.OfGetBejModel(masterModel.igb06);
                    if (bejModel != null)
                        masterModel.igb05_str = string.Format("{0:N" + bejModel.bej03 + "}", masterModel.igb05);//數量
                    else
                        masterModel.igb05_str = masterModel.igb05.ToString();
                }

                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;
                ////處理跳頁
                StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                if (invr302Model.jump_yn.ToUpper() == "Y")
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
