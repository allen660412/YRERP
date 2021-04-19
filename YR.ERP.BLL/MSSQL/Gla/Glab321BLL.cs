using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using YR.ERP.DAL.YRModel;
using YR.Util;
using System.Data.Common;
using System.Globalization;
using YR.ERP.Shared;
using YR.ERP.BLL.Model;

namespace YR.ERP.BLL.MSSQL.Gla
{
    public class Glab321BLL : YR.ERP.BLL.MSSQL.GlaBLL
    {
        private UserInfo _loginInfo = null;

        #region Glab321BLL() :　建構子
        public Glab321BLL()
            : base()
        {
        }

        //public Glab311BLL(YR.ERP.DAL.ERP_MSSQLDAL pdao)
        //    : base(pdao)
        //{
        //}


        //public Glab311BLL(string pComp, string pTargetTable, string pTargetColumn, string pViewTable)
        //    : base(pComp, pTargetTable, pTargetColumn, pViewTable)
        //{

        //}

        public Glab321BLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfGlab321Post
        public Result OfGlab321Post(vw_glab321 pModel, UserInfo pLoginInfo)
        {
            Result rtnResult = null;
            try
            {
                _loginInfo = pLoginInfo;
                rtnResult = new Result();
                if (OfDeleteRelation(pModel, rtnResult) == false)
                {
                    return rtnResult;
                }
                
                if (OfPostGbh(pModel, rtnResult) == false)
                {
                    return rtnResult;
                }
                
                if (OfPostGbj(pModel, rtnResult) == false)
                {
                    return rtnResult;
                }
                return rtnResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion
        
        #region OfDeleteRelation 刪除次年度相關結轉後年初資料
        private bool OfDeleteRelation(vw_glab321 pModel, Result pResult)
        {
            string deleteSql = "";
            List<SqlParameter> sqlParmList = null;
            int nextYear = 0;
            //DateTime startDate, endDate;
            try
            {
                nextYear = (new DateTime(Convert.ToInt16(pModel.gfa08), 1, 1)).AddYears(1).Year;
                //刪除會科各期餘額檔 下期期初資料
                deleteSql = "DELETE FROM gbh_tb WHERE gbh02=@gbh02 AND gbh03=0";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbh02", nextYear));
                OfExecuteNonquery(deleteSql, sqlParmList.ToArray());
                //刪除部門各期餘額檔
                deleteSql = "DELETE FROM gbj_tb WHERE gbj03=@gbj03 AND gbj04=0";
                sqlParmList = new List<SqlParameter>();  
                sqlParmList.Add(new SqlParameter("@gbj03", nextYear));
                OfExecuteNonquery(deleteSql, sqlParmList.ToArray());
                
                return true;
            }
            catch (Exception ex)
            {
                pResult.Message = "刪除年結相關資料失敗!";
                pResult.Exception = ex;
                return false;
            }
        } 
        #endregion
              
        #region 更新會科各期餘額
        private bool OfPostGbh(vw_glab321 pModel, Result pResult)
        {
            DataTable dtGbhSum = null;
            string selectSql = "";
            List<SqlParameter> sqlParmList = null;
            int nextYear = 0, chkCnts = 0;
            GlaBLL boGbh = null;
            DataTable dtGbhNext = null;
            try
            {
                nextYear = (new DateTime(Convert.ToInt16(pModel.gfa08), 1, 1)).AddYears(1).Year;
                //過帳會科各期餘額檔 下期期初資料
                selectSql = @"SELECT 
                                gbh01,
                                SUM(gbh04) AS gbh04,
                                SUM(gbh05) AS gbh05,
                                SUM(gbh06) AS gbh06,
                                SUM(gbh07) AS gbh07
                              FROM gbh_tb
                              WHERE gbh02=@gbh02
                                AND gbh03<>0
                              GROUP BY gbh01
                            ";
                
                  
                sqlParmList = new List<SqlParameter>();                
                sqlParmList.Add(new SqlParameter("@gbh02", pModel.gfa08));
                dtGbhSum = OfGetDataTable(selectSql, sqlParmList.ToArray());
                if (dtGbhSum == null || dtGbhSum.Rows.Count == 0)
                {
                    pResult.Message = "取得會科各期餘額加總失敗!";
                    return false;
                }                
                
                boGbh = new GlaBLL(OfGetConntion());
                boGbh.TRAN = this.TRAN;
                boGbh.OfCreateDao("gbh_tb", "*", "");
                selectSql = @"SELECT * FROM gbh_tb 
                              WHERE gbh01=@gbh01 AND gbh02=@gbh02 AND gbh03=@gbh03";
                foreach (DataRow drGbhSum in dtGbhSum.Rows)
                {
                    var gbhSumModel = drGbhSum.ToItem<gbh_tb>(true);
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@gbh01", gbhSumModel.gbh01));
                    sqlParmList.Add(new SqlParameter("@gbh02", nextYear));
                    sqlParmList.Add(new SqlParameter("@gbh03", 0));
                    dtGbhNext = boGbh.OfGetDataTable(selectSql, sqlParmList.ToArray());
                    if (dtGbhNext.Rows.Count == 0)
                    {
                        var drGbhNext = dtGbhNext.NewRow();
                        drGbhNext["gbh01"] = gbhSumModel.gbh01;
                        drGbhNext["gbh02"] = nextYear;
                        drGbhNext["gbh03"] = 0;
                        drGbhNext["gbh04"] = gbhSumModel.gbh04;
                        drGbhNext["gbh05"] = gbhSumModel.gbh05;
                        drGbhNext["gbh06"] = 0;
                        drGbhNext["gbh07"] = 0;
                        drGbhNext["gbhcomp"] = _loginInfo.CompNo;
                        dtGbhNext.Rows.Add(drGbhNext);
                    }
                    else
                    {
                        var drGbhNext = dtGbhNext.Rows[0];
                        drGbhNext["gbh04"] = gbhSumModel.gbh04;
                        drGbhNext["gbh05"] = gbhSumModel.gbh05;
                        drGbhNext["gbh06"] = 0;
                        drGbhNext["gbh07"] = 0;
                    }
                    chkCnts = boGbh.OfUpdate(dtGbhNext);
                    if (chkCnts != 1)
                    {
                        pResult.Message = "更新年結相關資料失敗!";
                        return false;
                    }
                }
                pResult.Success = true;
                return true;
            }
            catch (Exception ex)
            {
                pResult.Message = "更新年結相關資料失敗!";
                pResult.Exception = ex;
                return false;
            }
        } 
        #endregion        
        
        #region OfPostGbj 更新部門各期餘額
        private bool OfPostGbj(vw_glab321 pModel, Result pResult)
        {
            DataTable dtGbjSum = null;
            string selectSql = "";
            List<SqlParameter> sqlParmList = null;
            int nextYear = 0, chkCnts = 0;
            GlaBLL boGbh = null;
            DataTable dtGbjNext = null;
            try
            {
                nextYear = (new DateTime(Convert.ToInt16(pModel.gfa08), 1, 1)).AddYears(1).Year;
                //過帳會科部門各期餘額檔 下期期初資料
                selectSql = @"SELECT 
                                gbj01,gbj02,
                                SUM(gbj05) AS gbj05,
                                SUM(gbj06) AS gbj06
                              FROM gbj_tb
                              WHERE gbj03=@gbj03
                                AND gbj04<>4
                              GROUP BY gbj01,gbj02
                            ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbj03", pModel.gfa08));
                dtGbjSum = OfGetDataTable(selectSql, sqlParmList.ToArray());
                if (dtGbjSum == null || dtGbjSum.Rows.Count == 0)
                {
                    pResult.Message = "取得部門各期餘額加總失敗!";
                    return false;
                }

                boGbh = new GlaBLL(OfGetConntion());
                boGbh.TRAN = this.TRAN;
                boGbh.OfCreateDao("gbj_tb", "*", "");
                selectSql = @"SELECT * FROM gbj_tb 
                              WHERE gbj01=@gbj01 AND gbj02=@gbj02 AND gbj03=@gbj03 AND gbj04=@gbj04";
                foreach (DataRow drGbjSum in dtGbjSum.Rows)
                {
                    var gbjSumModel = drGbjSum.ToItem<gbj_tb>(true);
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@gbj01", gbjSumModel.gbj01));
                    sqlParmList.Add(new SqlParameter("@gbj02", gbjSumModel.gbj02));
                    sqlParmList.Add(new SqlParameter("@gbj03", nextYear));
                    sqlParmList.Add(new SqlParameter("@gbj04", 0));
                    dtGbjNext = boGbh.OfGetDataTable(selectSql, sqlParmList.ToArray());
                    if (dtGbjNext.Rows.Count == 0)
                    {
                        var drGbhNext = dtGbjNext.NewRow();
                        drGbhNext["gbj01"] = gbjSumModel.gbj01;
                        drGbhNext["gbj02"] = gbjSumModel.gbj02;
                        drGbhNext["gbj03"] = nextYear;
                        drGbhNext["gbj04"] = 0;
                        drGbhNext["gbj05"] = gbjSumModel.gbj05;
                        drGbhNext["gbj06"] = gbjSumModel.gbj06;
                        drGbhNext["gbj07"] = 0;
                        drGbhNext["gbj08"] = 0;
                        drGbhNext["gbjcomp"] = _loginInfo.CompNo;
                        dtGbjNext.Rows.Add(drGbhNext);
                    }
                    else
                    {
                        var drGbhNext = dtGbjNext.Rows[0];
                        drGbhNext["gbj05"] = gbjSumModel.gbj05;
                        drGbhNext["gbj06"] = gbjSumModel.gbj06;
                        drGbhNext["gbj07"] = 0;
                        drGbhNext["gbj08"] = 0;
                    }
                    chkCnts = boGbh.OfUpdate(dtGbjNext);
                    if (chkCnts != 1)
                    {
                        pResult.Message = "更新年結相關資料(部門餘額)失敗!";
                        return false;

                    }
                }
                pResult.Success = true;
                return true;
            }
            catch (Exception ex)
            {
                pResult.Message = "更新年結相關資料(部門餘額)失敗!";
                pResult.Exception = ex;
                return false;
            }

        } 
        #endregion
    }
}
