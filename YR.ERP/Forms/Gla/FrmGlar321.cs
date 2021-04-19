/* 程式名稱: 總分類帳列印
   系統代號: glar321
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
using System.Linq;
using YR.ERP.DAL.YRModel.Reports.Gla.Glar301;
using Stimulsoft.Report;


namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlar321 : YR.ERP.Base.Forms.FrmReportBase
    {

        #region property
        BasBLL BoBas = null;
        #endregion

        #region 建構子
        public FrmGlar321()
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
            this.StrFormID = "glar321";
            TabMaster.ReportName = @"\Gla\glar321.mrt";

            return true;
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

        #region WfSetMasterRowDefault(DataRow pDr) 設定MasterRow的初始值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            try
            {
                pDr["c1"] = "N";
                pDr["c2"] = "N";
                pDr["c3"] = "N";
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
            vw_glar321 glar321Model;
            try
            {
                glar321Model = DrMaster.ToItem<vw_glar321>();
                switch (e.Column.ToLower())
                {
                    case "gfa02_s":
                        if (GlobalFn.varIsNull(e.Value))
                        {
                            return true;
                        }

                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(glar321Model.gfa02_e))
                        {
                            if (glar321Model.gfa02_s > glar321Model.gfa02_e)
                            {
                                WfShowErrorMsg("起日不得大於迄日!");
                                return false;
                            }
                        }
                        break;
                    case "gfa02_e":
                        if (GlobalFn.varIsNull(e.Value))
                        {
                            return true;
                        }
                        if (!GlobalFn.varIsNull(e.Value) && !GlobalFn.varIsNull(glar321Model.gfa02_s))
                        {
                            if (glar321Model.gfa02_s > glar321Model.gfa02_e)
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
         
        #region WfPickClickOnEditMode(object sender, string pKey, DataRow pDr)
        protected override bool WfPickClickOnEditMode(object sender, string pColName, DataRow pDr)
        {
            try
            {
                MessageInfo messageModel = new MessageInfo();
                switch (pColName.ToLower())
                {
                    case "gfb03"://會計科目
                        messageModel.StrMultiColumn = "gba01";
                        messageModel.IntMaxRow = 999;
                        messageModel.StrWhereAppend = "AND gba06 in ('1','3')";
                        WfShowPickUtility("p_gba1", messageModel);
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
            vw_glar321 glar321Model = null;
            string msg;
            try
            {
                glar321Model = DrMaster.ToItem<vw_glar321>();
                if (GlobalFn.varIsNull(glar321Model.gfa02_s))
                {
                    msg = "傳票起日不可為空白!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(udt_gfa02_s, msg);
                    return false;
                }

                if (GlobalFn.varIsNull(glar321Model.gfa02_e))
                {
                    msg = "傳票迄日不可為空白!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(udt_gfa02_e, msg);
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
            vw_glar321 glar321Model;
            string sqlBody = "";
            string sqlOrderBy = "";
            DataTable dtMaster;
            List<Master> masterList;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string strQueryRange, strWhere;
            StringBuilder sbQuerySingle = null;

            List<SqlParameter> sqlParmList;
            string sqlDistinct = "";//取得會科
            DataTable dtDistinct = null;
            try
            {
                glar321Model = DrMaster.ToItem<vw_glar321>();
                
                queryInfoList = new List<QueryInfo>();
                sqlParmList = new List<SqlParameter>();
                #region range 處理
                if (!GlobalFn.varIsNull(glar321Model.gfb03))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "gba_tb";
                    queryModel.ColumnName = "gba01";
                    queryModel.ColumnType = TabMaster.DtSource.Columns["gba01"].DataType.Name;
                    queryModel.Value = glar321Model.gfb03;
                    queryInfoList.Add(queryModel);
                }
                strQueryRange = BoMaster.WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion
                                
                //strWhere = strQueryRange + " " + sbQuerySingle.ToString();
                //var strSecurity = WfGetSecurityString();        //取得權限字串
                //if (!GlobalFn.varIsNull(strSecurity))
                //    strWhere += strSecurity;
                //無異動科目列印時,以會計科目來處理
                if (glar321Model.c1=="Y")
                {
                    sqlDistinct = @"SELECT gba01
                                      FROM gba_tb
                                      WHERE gba06 in ('1','3')
                                ";
                    strWhere = string.Concat( sbQuerySingle.ToString());
                        
                }
                else //以期間內的傳票資料做為處理來源
                {
                    sqlDistinct = @"SELECT gba01
                                      FROM gfa_tb 
                                        INNER JOIN gfb_tb ON gfa01=gfb01
                                        INNER JOIN gba_tb on gfb03=gba01
                                      WHERE gba06 in ('1','3')
                                ";
                    #region single 處理
                    sbQuerySingle = new StringBuilder();
                    if (!GlobalFn.varIsNull(glar321Model.gfa02_s))
                    {
                        sbQuerySingle.AppendLine("AND gfa02>=@gfa02_s");
                        sqlParmList.Add(new SqlParameter("@gfa02_s", glar321Model.gfa02_s));
                    }
                    if (!GlobalFn.varIsNull(glar321Model.gfa02_e))
                    {
                        sbQuerySingle.AppendLine("AND gfa02<=@gfa02_e");
                        sqlParmList.Add(new SqlParameter("@gfa02_e", glar321Model.gfa02_e));
                    }
                    #endregion
                    strWhere = string.Concat(sbQuerySingle.ToString(), strQueryRange);
                }
                sqlOrderBy=" ORDER BY gba01";
                dtDistinct = BoMaster.OfGetDataTable(string.Concat(sqlDistinct, strWhere, sqlOrderBy), sqlParmList.ToArray());
                if (dtDistinct == null || dtDistinct.Rows.Count == 0)
                {
                    WfShowErrorMsg("查無資料!");
                    return false;
                }

                foreach(DataRow drDistinct in dtDistinct.Rows)
                {
                    var gba01 = drDistinct[0].ToString();
                }


                //處理排序
                //switch (glar321Model.order_by)
                //{
                //    case "1"://1.依借出日期
                //        sqlOrderBy = " ORDER BY ila02";
                //        break;
                //    case "2"://2.依客戶
                //        sqlOrderBy = " ORDER BY ila03";
                //        break;
                //}
                //dtMaster.TableName = "Master";
                
                //if (dtMaster == null || dtMaster.Rows.Count == 0)
                //{
                //    WfShowErrorMsg("查無資料,請重新過濾條件!");
                //    return false;
                //}
                //masterList = dtMaster.ToList<Master>(true);
                //foreach (Master masterModel in masterList)
                //{
                //    masterModel.ilb05_str = string.Format("{0:N" + masterModel.bej03 + "}", masterModel.ilb05);//數量

                //}

                //pReport.RegData("Master", masterList);
                //pReport.CacheAllData = true;
                //////處理跳頁
                //StiGroupFooterBand footerBand1 = (StiGroupFooterBand)pReport.GetComponents()["GroupFooterBand1"];
                //if (invr401Model.jump_yn.ToUpper() == "Y")
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
