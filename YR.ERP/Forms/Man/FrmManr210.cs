/* 程式名稱: 託工單憑證列印作業
   系統代號: manr210
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
using YR.ERP.DAL.YRModel.Reports.Man.Manr210;
using YR.Util;
using YR.ERP.Base.Forms;

namespace YR.ERP.Forms.Man
{
    public partial class FrmManr210 : YR.ERP.Base.Forms.FrmReportBase
    {
        #region property
        vw_manr210 Vw_Manr210;  //給他窗引用時,預設查詢條件
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmManr210()
        {
            InitializeComponent();
        }

        public FrmManr210(YR.ERP.Shared.UserInfo pUserInfo, vw_manr210 pManr210Model, bool pAutoExecuted, bool pCloseAfterExecute)
        {
            InitializeComponent();
            
            this.LoginInfo = pUserInfo;
            this.Vw_Manr210 = pManr210Model;
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
            this.StrFormID = "manr210";
            TabMaster.ReportName = @"\Man\manr210.mrt";
            
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            TabMaster.UserColumn = "measecu";
            TabMaster.GroupColumn = "measecg";            
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
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依託工日期"));
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
                pDr["mea01"] = "";
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
            vw_manr210 manr210Model;
            try
            {
                manr210Model = DrMaster.ToItem<vw_manr210>();
                switch (e.Column.ToLower())
                {
                    case "mea02_s":
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(manr210Model.mea02_e))
                        {
                            if (manr210Model.mea02_s > manr210Model.mea02_e)
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
            vw_manr210 manr210Model = null;
            string msg;
            try
            {
                manr210Model = DrMaster.ToItem<vw_manr210>();
                if (GlobalFn.varIsNull(manr210Model.mea01)
                    && GlobalFn.varIsNull(manr210Model.mea02_s)
                    && GlobalFn.varIsNull(manr210Model.mea02_e)
                    && GlobalFn.varIsNull(manr210Model.mea03)
                    && GlobalFn.varIsNull(manr210Model.mea20)
                    )
                {
                    msg = "請至少輸入一個條件!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_mea01, msg);
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
                    case "mea01"://報價單號
                        messageModel.StrMultiColumn = "mea01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_mea2", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;
                        }
                        break;
                    case "mea03"://廠商編號
                        messageModel.StrMultiColumn = "pca01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_pca1", messageModel);
                        if (messageModel.Result == System.Windows.Forms.DialogResult.OK)
                        {
                            pDr[pColName] = messageModel.StrMultiRtn;                            
                        }
                        break;

                    case "mea20"://主件料號
                        messageModel.StrMultiColumn = "ica01";
                        messageModel.IntMaxRow = 999;
                        WfShowPickUtility("p_ica1", messageModel);
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
            vw_manr210 manr210Model;
            StringBuilder sbSql = null;
            string sqlBody = "";
            DataTable dtMaster;
            List<YR.ERP.DAL.YRModel.Reports.Man.Manr210.Master> masterList;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            try
            {
                if (Vw_Manr210 != null) //他窗引用時
                    manr210Model = Vw_Manr210;
                else
                    manr210Model = DrMaster.ToItem<vw_manr210>();
                
                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(manr210Model.mea01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "mea_tb";
                    queryModel.ColumnName = "mea01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["mea01"].DataType.Name;
                    queryModel.Value = manr210Model.mea01;
                    queryInfoList.Add(queryModel);
                }
                if (!GlobalFn.varIsNull(manr210Model.mea03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "mea_tb";
                    queryModel.ColumnName = "mea03";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["mea03"].DataType.Name;
                    queryModel.Value = manr210Model.mea03;
                    queryInfoList.Add(queryModel);
                }

                if (!GlobalFn.varIsNull(manr210Model.mea20))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "mea_tb";
                    queryModel.ColumnName = "mea20";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["mea20"].DataType.Name;
                    queryModel.Value = manr210Model.mea03;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion
                
                #region single 處理
                sbQuerySingle = new StringBuilder();
                if (!GlobalFn.varIsNull(manr210Model.mea02_s))
                {
                    sbQuerySingle.AppendLine("AND mea02>=@mea02_s");                    
                    sqlParmList.Add(new SqlParameter("@mea02_s", manr210Model.mea02_s));
                }
                if (!GlobalFn.varIsNull(manr210Model.mea02_e))
                {
                    sbQuerySingle.AppendLine("AND mea02<=@mea02_e");
                    sqlParmList.Add(new SqlParameter("@mea02_e", manr210Model.mea02_e));
                }
                #endregion

                strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                var strSecurity = WfGetSecurityString();        //取得權限字串
                if (!GlobalFn.varIsNull(strSecurity))
                    strWhere += strSecurity;

                //取得單頭
                sqlBody = @"SELECT 
                                mea00,mea01,mea02,mea03,mea04,mea05,
                                mea06,mea07,mea08,mea09,mea10,
                                mea11,mea12,mea13,mea14,mea14t,
                                mea15,mea16,mea17,mea18,mea19,
                                mea20,mea21,mea22,mea23,mea24,
                                mea25,mea26,mea27,mea28,mea29,
                                bab02 as mea01_c,
                                pca02 as mea03_c,
                                bec02 as  mea04_c,beb02 as mea05_c,
                                bef03 as mea11_c,pbb02 as mea12_c,
                                ica_main.ica03 as ica03_main,
                                '' as mea13_str,'' as mea14_str,'' as mea14t_str,
                                '' as mea22_str,
                                bej_main.bej03 as bej03_main,
                                meb02,meb03,meb04,meb05,meb06,
                                meb07,
                                ica_detail.ica03 as ica03_detail,
                                bej_detail.bej03 as bej03_detail,
                                '' as meb05_str
                            FROM mea_tb                                
                                LEFT JOIN meb_tb On mea01=meb01
	                            LEFT JOIN pca_tb ON mea03=pca01	--廠商
	                            LEFT JOIN bec_tb ON mea04=bec01	--員工
	                            LEFT JOIN beb_tb ON mea05=beb01	--部門
	                            LEFT JOIN bef_tb ON mea11=bef02 AND bef01='1'	--收付款條件
	                            LEFT JOIN pbb_tb ON mea12=pbb01	--採購取價原則
	                            LEFT JOIN baa_tb ON 1=1	
	                            LEFT JOIN bab_tb ON substring(mea01,1,baa06)=bab01
                                LEFT JOIN ica_tb ica_main ON mea20=ica_main.ica01
                                LEFT JOIN bej_tb bej_main ON mea23=bej_main.bej01
                                LEFT JOIN ica_tb ica_detail ON meb03=ica_detail.ica01
                                LEFT JOIN bej_tb bej_detail ON meb06=bej_detail.bej01
                            WHERE 1=1 
                                AND meaconf='Y'
                                AND mea00='2'
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
                    masterModel.mea22_str = string.Format("{0:N" + masterModel.bej03_main + "}", masterModel.mea22);//主件數量
                    masterModel.meb05_str = string.Format("{0:N" + masterModel.bej03_detail + "}", masterModel.meb05);//子件數量

                    var bekModel = BoBas.OfGetBekModel(masterModel.mea10);
                    masterModel.mea13_str = string.Format("{0:N" + bekModel.bek03 + "}", masterModel.mea13);
                    masterModel.mea14_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.mea14);
                    masterModel.mea14t_str = string.Format("{0:N" + bekModel.bek04 + "}", masterModel.mea14t);
                }

                pReport.RegData(dtMaster);
                pReport.RegData("Master", masterList);
                pReport.CacheAllData = true;
                //處理排序
                StiDataBand stiDataBand1 = (StiDataBand)pReport.GetComponents()["DataBand1"];
                switch (manr210Model.order_by)
                {
                    case "1"://1.依託工日期
                        stiDataBand1.Sort = new string[] { "ASC", "mea02" };
                        break;
                    case "2"://2.依廠商
                        stiDataBand1.Sort = new string[] { "ASC", "mea03" };
                        break;
                }
                //處理跳頁
                StiGroupFooterBand grouperBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                if (manr210Model.jump_yn.ToUpper() == "Y")
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
