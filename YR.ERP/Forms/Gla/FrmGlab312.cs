/* 程式名稱: 已過帳傳票重新過帳作業
   系統代號: glab312
   作　　者: Allen
   描　　述: 

   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using YR.ERP.BLL.Model;
using YR.ERP.BLL.MSSQL;
using YR.ERP.BLL.MSSQL.Gla;
using YR.ERP.DAL.YRModel;
using YR.Util;
using System.Linq;

namespace YR.ERP.Forms.Gla
{
    public partial class FrmGlab312 :  YR.ERP.Base.Forms.FrmBatchBase
    {
        #region Property
        Glab311BLL BoGlab311;
        #endregion

        #region 建構子
        public FrmGlab312()
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
            this.StrFormID = "glab312";
            
            return true;
        }
        #endregion

        #region WfIniTabMasterInfo 用來控制是否可做新增修改刪除等功能,及設定PK,並且在修改時重取mater資料及Lock
        protected override void WfIniTabMasterInfo()
        {
            // 因為刪除整期的過帳資料,所以權限不做控管
            //TabMaster.UserColumn = "gfasecu";
            //TabMaster.GroupColumn = "gfasecg";
        }
        #endregion
        
        #region WfCreateBoBasic() : 建立 business object
        protected override void WfCreateBoBasic()
        {
            BoMaster = new AdmBLL(LoginInfo.CompNo, TabMaster.TargetTable, TabMaster.TargetColumn, TabMaster.ViewTable);
            BoGlab311 = new Glab311BLL(BoMaster.OfGetConntion());            
            return;            
        }
        #endregion

        #region WfSetMasterRowDefault(DataRow pDr) 設定MasterRow的初始值
        protected override bool WfSetMasterRowDefault(DataRow pDr)
        {
            int year=0, period=0;
            try
            {
                var dtToday = Today;
                if (BoGlab311.OfGetGlaYearPeriod(Today,out year,out period) == false)
                    return false;
                
                pDr["gfa08"] = year;
                pDr["gfa09"] = period;

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
            vw_glab312 glab312Model;
            try
            {                
                glab312Model = DrMaster.ToItem<vw_glab312>();
                switch (e.Column.ToLower())
                {
                    case "gfa08":
                        if (GlobalFn.varIsNull(e.Value))
                        {
                            WfShowErrorMsg("會計年度不可為空白,請檢核!");
                            return false;
                        }
                        
                        if (GlobalFn.isNullRet(e.Value, 0m) <= 0)
                        {
                            WfShowErrorMsg("會計年度應大於0,請檢核!");
                            return false;
                        }
                        break;
                    case "gfa09":
                        if (GlobalFn.varIsNull(e.Value))
                        {
                            return true;
                        }
                        if (GlobalFn.isNullRet(e.Value, 0) <= 0 || GlobalFn.isNullRet(e.Value, 0)>12)
                        {
                            WfShowErrorMsg("非正確期數,請檢核!");
                            return false;
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
            vw_glab312 masterModel = null;
            string msg = "";
            try
            {
                masterModel = DrMaster.ToItem<vw_glab312>();
                if (GlobalFn.varIsNull(masterModel.gfa08))
                {
                    msg = "會計年度不可為空白!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_gfa08, msg);
                    return false;
                }
                
                if (GlobalFn.varIsNull(masterModel.gfa09))
                {
                    msg = "期數不可為空白!";
                    WfShowErrorMsg(msg);
                    errorProvider.SetError(ute_gfa09, msg);
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

        #region WfExecute 批次執行開始
        protected override bool WfExecute()
        {
            vw_glab311 glab311Model = null;
            vw_glab312 glab312Model = null;
            int chkCnts = 0;
            StringBuilder sbResult = null;
            DateTime dtStart, dtEnd;
            try
            {
                glab312Model = DrMaster.ToItem<vw_glab312>();
                glab311Model = new vw_glab311();
                if (BoGlab311.OfGetGlaYearPeriod(Convert.ToInt16(glab312Model.gfa08), Convert.ToInt16(glab312Model.gfa09),
                                            out dtStart, out dtEnd) == false)
                {
                    WfShowErrorMsg("無法取得期數區間,請檢核!");
                    return false;
                }
                glab311Model.gfa02_s = dtStart;
                glab311Model.gfa02_e = dtEnd;
                
                //取得交易物件
                BoMaster.TRAN = BoMaster.OfGetConntion().BeginTransaction();
                BoGlab311.TRAN = BoMaster.TRAN;
                if (WfUpdatePostRelation(glab311Model,glab312Model)==false)
                {
                    return false;
                }
                var resultList = BoGlab311.OfGlab311Post(glab311Model, "Y", "", "", LoginInfo);

                if (resultList == null || resultList.Count == 0)
                {
                    WfShowBottomStatusMsg("無可過帳資料!");
                    BoGlab311.TRAN.Rollback();
                    return true;
                }

                chkCnts = resultList.Where(p => p.Success == false).Count();
                if (chkCnts > 0)
                {
                    BoGlab311.TRAN.Rollback();
                    sbResult = new StringBuilder();
                    sbResult.AppendLine(string.Format("執行失敗!"));
                    sbResult.AppendLine(string.Format("過帳傳票筆數【{0}】 成功:【{1}】  失敗【{2}】",
                                                            resultList.Count,
                                                            resultList.Count - chkCnts,
                                                            chkCnts
                                                            ));
                    sbResult.AppendLine();
                    sbResult.AppendLine(string.Format("錯誤內容如下"));
                    sbResult.AppendLine("====================================");
                    foreach (Result result in resultList.Where(p => p.Success == false))
                    {
                        sbResult.AppendLine(string.Format("key1:【{0}】 錯誤訊息:【{1}】", result.Key1, result.Message));
                    }
                    WfShowErrorMsg(sbResult.ToString());
                    return false;
                }
                BoMaster.TRAN.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (BoMaster.TRAN != null)
                    BoMaster.TRAN.Rollback();
                throw ex;
            }
        }
        #endregion

        #region WfUpdatePostRelation
        /// <summary>
        /// 刪除此會計期間內的相關內容
        /// 1.會計期間餘額檔 gbh_tb
        /// 2.每日餘額檔 gbi_tb
        /// 3.部門餘額檔 gbj_tb
        /// </summary>
        /// <param name="glab311Model"></param>
        /// <returns></returns>
        private bool WfUpdatePostRelation(vw_glab311 glab311Model, vw_glab312 glab312Model)
        {
            string updateSql = "";
            List<SqlParameter> sqlParmList = null;
            try
            {
                //更新會計期間餘額檔
                updateSql = @"UPDATE gbh_tb
                        SET gbh04=0,gbh05=0,gbh06=0,gbh07=0
                        WHERE gbh02=@gbh02
                            AND gbh03=@gbh03
                        ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbh02", glab312Model.gfa08));
                sqlParmList.Add(new SqlParameter("@gbh03", glab312Model.gfa09));
                BoGlab311.OfExecuteNonquery(updateSql, sqlParmList.ToArray());

                //更新每日餘額檔
                updateSql = @"UPDATE gbi_tb
                        SET gbi03=0,gbi04=0,gbi05=0,gbi06=0
                        WHERE gbi02 between @gbi02_s AND @gbi02_e
                        ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbi02_s", glab311Model.gfa02_s));
                sqlParmList.Add(new SqlParameter("@gbi02_e", glab311Model.gfa02_e));
                BoGlab311.OfExecuteNonquery(updateSql, sqlParmList.ToArray());

                //更新部門餘額檔
                updateSql = @"UPDATE gbj_tb
                        SET gbj05=0,gbj06=0,gbj07=0,gbj08=0
                        WHERE gbj03=@gbj03
                            AND gbj04=@gbj04
                        ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbj03", glab312Model.gfa08));
                sqlParmList.Add(new SqlParameter("@gbj04", glab312Model.gfa09));
                BoGlab311.OfExecuteNonquery(updateSql, sqlParmList.ToArray());

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
