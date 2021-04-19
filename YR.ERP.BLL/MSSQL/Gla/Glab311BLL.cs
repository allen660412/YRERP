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
using YR.Util;
using System.Globalization;
using YR.ERP.Shared;
using YR.ERP.BLL.Model;

namespace YR.ERP.BLL.MSSQL.Gla
{
    public class Glab311BLL : YR.ERP.BLL.MSSQL.GlaBLL
    {
        private UserInfo _loginInfo = null;

        #region Glab311BLL() :　建構子
        public Glab311BLL()
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

        public Glab311BLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfGlab311Post
        /// <summary>
        /// 過帳及重過帳共用程式
        /// </summary>
        /// <param name="pModel"></param>
        /// <param name="pPost"></param>
        /// <returns></returns>
        public List<Result> OfGlab311Post(vw_glab311 pModel, string pPost, string pSecurityString, string pSrcFormId, UserInfo pLoginInfo)
        {
            List<Result> rtnResultList = null;
            string selectSql = "";
            string strQueryRange = "", strQuerySingle = "";
            List<SqlParameter> sqlParmList = null;
            DataTable dtGfa = null;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            gfa_tb gfaModel;
            try
            {
                _loginInfo = pLoginInfo;

                selectSql = @"
                            SELECT * FROM gfa_tb
                            WHERE gfaconf='Y'
                        ";

                selectSql += string.Format(" AND gfapost='{0}'", pPost);

                sqlParmList = new List<SqlParameter>();
                queryInfoList = new List<QueryInfo>();
                #region range 處理
                if (!GlobalFn.varIsNull(pModel.gfa01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "gfa_tb";
                    queryModel.ColumnName = "gfa01";
                    queryModel.ColumnType = "string";
                    queryModel.Value = pModel.gfa01;
                    queryInfoList.Add(queryModel);
                }
                sqlParmList = new List<SqlParameter>();
                strQueryRange = WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single處理
                if (!GlobalFn.varIsNull(pModel.gfa02_s))
                {
                    sqlParmList.Add(new SqlParameter("@gfa02_s", pModel.gfa02_s));
                    strQuerySingle += string.Format(" AND gfa02>=@gfa02_s");
                }
                if (!GlobalFn.varIsNull(pModel.gfa02_e))
                {
                    sqlParmList.Add(new SqlParameter("@gfa02_e", pModel.gfa02_e));
                    strQuerySingle += string.Format(" AND gfa02<=@gfa02_e");
                }
                #endregion
                selectSql = string.Concat(selectSql, strQueryRange, strQuerySingle, pSecurityString);   //加入權限處理
                dtGfa = OfGetDataTable(selectSql, sqlParmList.ToArray());
                if (dtGfa == null || dtGfa.Rows.Count == 0)
                    return null;

                rtnResultList = new List<Result>();
                foreach (DataRow drGfa in dtGfa.Rows)
                {
                    gfaModel = drGfa.ToItem<gfa_tb>();
                    var result = new Result();
                    rtnResultList.Add(result);

                    if (gfaModel.gfa03 != gfaModel.gfa04)
                    {
                        result.Key1 = gfaModel.gfa01;
                        result.Message = "借貸不平衡!";
                        continue;
                    }
                    if (pPost == "N") //重過帳不修改傳票單頭
                    {
                        if (OfPostGfa(gfaModel) == false)
                        {
                            result.Key1 = gfaModel.gfa01;
                            result.Message = "更新gfa_tb失敗!";
                            continue;
                        }
                    }

                    if (OfPostGfb(gfaModel, result) == false)
                    {
                        continue;
                    }

                    result.Success = true;
                }

                return rtnResultList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region OfPostGfa
        private bool OfPostGfa(gfa_tb pGfaModel)
        {
            string updateSql = "";
            List<SqlParameter> sqlParmList = null;
            int chkCnts = 0;
            try
            {
                updateSql = @"UPDATE gfa_tb
                             SET gfapost='Y',
                                gfaposd=@gfaposd,
                                gfaposu=@gfaposu
                             WHERE gfa01=@gfa01
                            ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gfa01", pGfaModel.gfa01));
                sqlParmList.Add(new SqlParameter("@gfaposd", OfGetToday()));
                sqlParmList.Add(new SqlParameter("@gfaposu", _loginInfo.UserName));
                chkCnts = OfExecuteNonquery(updateSql, sqlParmList.ToArray());
                if (chkCnts != 1)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfPostGfb
        private bool OfPostGfb(gfa_tb pGfaModel, Result pResult)
        {
            DataTable dtGfb = null;
            gfb_tb gfbModel = null;
            gba_tb gbaModel = null;
            string selectSql = "";
            List<SqlParameter> sqlParmList = null;
            try
            {
                selectSql = @"SELECT * FROM gfb_tb WHERE gfb01=@gfb01";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gfb01", pGfaModel.gfa01));
                dtGfb = OfGetDataTable(selectSql, sqlParmList.ToArray());
                if (dtGfb == null && dtGfb.Rows.Count == 0)
                    return true;
                foreach (DataRow drGfb in dtGfb.Rows)
                {
                    gfbModel = drGfb.ToItem<gfb_tb>();
                    gbaModel = OfGetGbaModel(gfbModel.gfb03);
                    if (gfbModel == null)
                    {
                        pResult.Key1 = string.Concat(gfbModel.gfb01, "/", gfbModel.gfb02);
                        pResult.Message = "查無此項次,會科資料!";
                        return false;
                    }

                    //更新會計科目每期餘額檔
                    if (OfPostGbh(pGfaModel, gfbModel, gbaModel, pResult) == false)
                    {
                        return false;
                    }
                    //更新會計科目每日餘額檔
                    if (OfPostGbi(pGfaModel, gfbModel, gbaModel, pResult) == false)
                    {
                        return false;
                    }
                    //更新部門各期餘額檔
                    if (OfPostGbj(pGfaModel, gfbModel, gbaModel, pResult) == false)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region OfPostGbh 會科各期餘額檔
        private bool OfPostGbh(gfa_tb pGfaModel, gfb_tb pGfbModel, gba_tb pGbaModel, Result pResult)
        {
            string selectSql = "";
            List<SqlParameter> sqlParmList = null;
            decimal dAmt = 0, cAmt = 0, dRec = 0, cRec = 0;
            GlaBLL boGbh = null;
            DataTable dtGbh = null;
            DataRow drGbh = null;
            try
            {
                boGbh = new GlaBLL(OfGetConntion());
                boGbh.TRAN = this.TRAN;
                boGbh.OfCreateDao("gbh_tb", "*", "");
                selectSql = @"SELECT * FROM gbh_tb 
                              WHERE gbh01=@gbh01 AND gbh02=@gbh02 AND gbh03=@gbh03";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbh01", pGfbModel.gfb03));
                sqlParmList.Add(new SqlParameter("@gbh02", pGfaModel.gfa08));
                sqlParmList.Add(new SqlParameter("@gbh03", pGfaModel.gfa09));
                dtGbh = boGbh.OfGetDataTable(selectSql, sqlParmList.ToArray());

                if (pGfbModel.gfb06 == "1")
                {
                    dAmt = pGfbModel.gfb07;
                    dRec = 1;
                }
                else
                {
                    cAmt = pGfbModel.gfb07;
                    cRec = 1;
                }

                if (dtGbh.Rows.Count == 0)
                {
                    drGbh = dtGbh.NewRow();
                    drGbh["gbh01"] = pGfbModel.gfb03;
                    drGbh["gbh02"] = pGfaModel.gfa08;
                    drGbh["gbh03"] = pGfaModel.gfa09;
                    drGbh["gbh04"] = dAmt;
                    drGbh["gbh05"] = cAmt;
                    drGbh["gbh06"] = dRec;
                    drGbh["gbh07"] = cRec;
                    drGbh["gbhcomp"] = _loginInfo.CompNo;
                    dtGbh.Rows.Add(drGbh);
                }
                else
                {
                    drGbh = dtGbh.Rows[0];
                    drGbh["gbh04"] = Convert.ToDecimal(drGbh["gbh04"]) + dAmt;
                    drGbh["gbh05"] = Convert.ToDecimal(drGbh["gbh05"]) + cAmt;
                    drGbh["gbh06"] = Convert.ToDecimal(drGbh["gbh06"]) + dRec;
                    drGbh["gbh07"] = Convert.ToDecimal(drGbh["gbh07"]) + cRec;

                }
                if (boGbh.OfUpdate(dtGbh) != 1)
                {
                    pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                    pResult.Message = "更新會科各期餘額檔(gbh_tb)失敗!";
                    return false;
                }

                //明細科目,需遞迴處理統制科目
                if (pGbaModel.gba06 == "2")
                {
                    if (OfPostGbhRecursive(pGbaModel.gba07, pGfaModel, pGfbModel, dAmt, cAmt, dRec, cRec, pResult) == false)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                pResult.Message = "更新會科各期餘額檔(gbh_tb)失敗!";
                pResult.Exception = ex;
                return false;
            }
        }
        #endregion

        #region OfPostGbhRecursive 會科各期餘額檔-遞迴處理
        private bool OfPostGbhRecursive(string pGba07, gfa_tb pGfaModel, gfb_tb pGfbModel,
                                        decimal pDAmt, decimal pCAmt, decimal pDRec, decimal pCRec, Result pResult)
        {
            string selectSql = "";
            List<SqlParameter> sqlParmList = null;
            GlaBLL boGbh = null;
            DataTable dtGbh = null;
            DataRow drGbh = null;
            try
            {
                boGbh = new GlaBLL(OfGetConntion());
                boGbh.TRAN = this.TRAN;
                boGbh.OfCreateDao("gbh_tb", "*", "");
                selectSql = @"SELECT * FROM gbh_tb 
                              WHERE gbh01=@gbh01 AND gbh02=@gbh02 AND gbh03=@gbh03";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbh01", pGba07));
                sqlParmList.Add(new SqlParameter("@gbh02", pGfaModel.gfa08));
                sqlParmList.Add(new SqlParameter("@gbh03", pGfaModel.gfa09));
                dtGbh = boGbh.OfGetDataTable(selectSql, sqlParmList.ToArray());

                if (dtGbh.Rows.Count == 0)
                {
                    drGbh = dtGbh.NewRow();
                    drGbh["gbh01"] = pGba07;
                    drGbh["gbh02"] = pGfaModel.gfa08;
                    drGbh["gbh03"] = pGfaModel.gfa09;
                    drGbh["gbh04"] = pDAmt;
                    drGbh["gbh05"] = pCAmt;
                    drGbh["gbh06"] = pDRec;
                    drGbh["gbh07"] = pCRec;
                    drGbh["gbhcomp"] = _loginInfo.CompNo;
                    dtGbh.Rows.Add(drGbh);
                }
                else
                {
                    drGbh = dtGbh.Rows[0];
                    drGbh["gbh04"] = Convert.ToDecimal(drGbh["gbh04"]) + pDAmt;
                    drGbh["gbh05"] = Convert.ToDecimal(drGbh["gbh05"]) + pCAmt;
                    drGbh["gbh06"] = Convert.ToDecimal(drGbh["gbh06"]) + pDRec;
                    drGbh["gbh07"] = Convert.ToDecimal(drGbh["gbh07"]) + pCRec;

                }
                if (boGbh.OfUpdate(dtGbh) != 1)
                {
                    pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                    pResult.Message = "更新會科各期餘額檔統制科目(gbh_tb)失敗!";
                    return false;
                }

                var gbaParentsModel = OfGetGbaModel(pGba07);
                if (gbaParentsModel.gba07.ToLower() != "root")
                {
                    if (OfPostGbhRecursive(gbaParentsModel.gba07, pGfaModel, pGfbModel,
                                            pDAmt, pCAmt, pDRec, pCRec, pResult) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                pResult.Message = "更新會科各期餘額檔統制科目(gbh_tb)失敗!";
                pResult.Exception = ex;
                return false;
            }
        }
        #endregion

        #region OfPostGbi 會科每日餘額檔
        private bool OfPostGbi(gfa_tb pGfaModel, gfb_tb pGfbModel, gba_tb pGbaModel, Result pResult)
        {
            string selectSql = "", updateSql = "", insertSql = "";
            List<SqlParameter> sqlParmList = null;
            decimal dAmt = 0, cAmt = 0, dRec = 0, cRec = 0;
            GlaBLL boGbi = null;
            DataTable dtGbi = null;
            DataRow drGbi = null;
            try
            {
                boGbi = new GlaBLL(OfGetConntion());
                boGbi.TRAN = this.TRAN;
                boGbi.OfCreateDao("gbi_tb", "*", "");
                selectSql = @"SELECT * FROM gbi_tb 
                              WHERE gbi01=@gbi01 AND gbi02=@gbi02";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbi01", pGfbModel.gfb03));
                sqlParmList.Add(new SqlParameter("@gbi02", pGfaModel.gfa02));
                dtGbi = boGbi.OfGetDataTable(selectSql, sqlParmList.ToArray());

                if (pGfbModel.gfb06 == "1")
                {
                    dAmt = pGfbModel.gfb07;
                    dRec = 1;
                }
                else
                {
                    cAmt = pGfbModel.gfb07;
                    cRec = 1;
                }

                if (dtGbi.Rows.Count == 0)
                {
                    drGbi = dtGbi.NewRow();
                    drGbi["gbi01"] = pGfbModel.gfb03;
                    drGbi["gbi02"] = pGfaModel.gfa02;
                    drGbi["gbi03"] = dAmt;
                    drGbi["gbi04"] = cAmt;
                    drGbi["gbi05"] = dRec;
                    drGbi["gbi06"] = cRec;
                    drGbi["gbicomp"] = _loginInfo.CompNo;
                    dtGbi.Rows.Add(drGbi);
                }
                else
                {
                    drGbi = dtGbi.Rows[0];
                    drGbi["gbi03"] = Convert.ToDecimal(drGbi["gbi03"]) + dAmt;
                    drGbi["gbi04"] = Convert.ToDecimal(drGbi["gbi04"]) + cAmt;
                    drGbi["gbi05"] = Convert.ToDecimal(drGbi["gbi05"]) + dRec;
                    drGbi["gbi06"] = Convert.ToDecimal(drGbi["gbi06"]) + cRec;

                }
                if (boGbi.OfUpdate(dtGbi) != 1)
                {
                    pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                    pResult.Message = "更新會科各期餘額檔(gbi_tb)失敗!";
                    return false;
                }

                //明細科目,需遞迴處理統制科目
                if (pGbaModel.gba06 == "2")
                {
                    if (OfPostGbiRecursive(pGbaModel.gba07, pGfaModel, pGfbModel, dAmt, cAmt, dRec, cRec, pResult) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                pResult.Message = "更新會科每日餘額檔(gbi_tb)失敗!";
                pResult.Exception = ex;
                return false;
            }
        }
        #endregion

        #region OfPostGbiRecursive 會科科目每日餘額檔-遞迴處理
        private bool OfPostGbiRecursive(string pGba07, gfa_tb pGfaModel, gfb_tb pGfbModel,
                                        decimal pDAmt, decimal pCAmt, decimal pDRec, decimal pCRec, Result pResult)
        {
            string selectSql = "";
            List<SqlParameter> sqlParmList = null;
            GlaBLL boGbi = null;
            DataTable dtGbi = null;
            DataRow drGbi = null;
            try
            {
                boGbi = new GlaBLL(OfGetConntion());
                boGbi.TRAN = this.TRAN;
                boGbi.OfCreateDao("gbi_tb", "*", "");
                selectSql = @"SELECT * FROM gbi_tb 
                              WHERE gbi01=@gbi01 AND gbi02=@gbi02 ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbi01", pGba07));
                sqlParmList.Add(new SqlParameter("@gbi02", pGfaModel.gfa02));
                dtGbi = boGbi.OfGetDataTable(selectSql, sqlParmList.ToArray());

                if (dtGbi.Rows.Count == 0)
                {
                    drGbi = dtGbi.NewRow();
                    drGbi["gbi01"] = pGba07;
                    drGbi["gbi02"] = pGfaModel.gfa02;
                    drGbi["gbi03"] = pDAmt;
                    drGbi["gbi04"] = pCAmt;
                    drGbi["gbi05"] = pDRec;
                    drGbi["gbi06"] = pCRec;
                    drGbi["gbicomp"] = _loginInfo.CompNo;
                    dtGbi.Rows.Add(drGbi);
                }
                else
                {
                    drGbi = dtGbi.Rows[0];

                    drGbi["gbi03"] = Convert.ToDecimal(drGbi["gbi03"]) + pDAmt;
                    drGbi["gbi04"] = Convert.ToDecimal(drGbi["gbi04"]) + pCAmt;
                    drGbi["gbi05"] = Convert.ToDecimal(drGbi["gbi05"]) + pDRec;
                    drGbi["gbi06"] = Convert.ToDecimal(drGbi["gbi06"]) + pCRec;

                }
                if (boGbi.OfUpdate(dtGbi) != 1)
                {
                    pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                    pResult.Message = "更新會科每日餘額檔(gbi_tb)失敗!";
                    return false;
                }

                var gbaParentsModel = OfGetGbaModel(pGba07);
                if (gbaParentsModel.gba07.ToLower() != "root")
                {
                    if (OfPostGbiRecursive(gbaParentsModel.gba07, pGfaModel, pGfbModel,
                                            pDAmt, pCAmt, pDRec, pCRec, pResult) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                pResult.Message = "更新會科每日餘額檔(gbi_tb)失敗!";
                pResult.Exception = ex;
                return false;
            }
        }
        #endregion

        #region OfPostGbj 部門各期餘額檔
        private bool OfPostGbj(gfa_tb pGfaModel, gfb_tb pGfbModel, gba_tb pGbaModel, Result pResult)
        {
            string selectSql = "";
            List<SqlParameter> sqlParmList = null;
            decimal dAmt = 0, cAmt = 0, dRec = 0, cRec = 0;
            GlaBLL boGbj = null;
            DataTable dtGbj = null;
            DataRow drGbj = null;
            try
            {
                if (pGbaModel.gba09 != "Y")
                    return true;

                boGbj = new GlaBLL(OfGetConntion());
                boGbj.TRAN = this.TRAN;
                boGbj.OfCreateDao("gbj_tb", "*", "");
                selectSql = @"SELECT * FROM gbj_tb 
                              WHERE gbj01=@gbj01 AND gbj02=@gbj02 
                                    AND gbj03=@gbj03 AND gbj04=@gbj04";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbj01", pGfbModel.gfb03));
                sqlParmList.Add(new SqlParameter("@gbj02", pGfbModel.gfb05));
                sqlParmList.Add(new SqlParameter("@gbj03", pGfaModel.gfa08));
                sqlParmList.Add(new SqlParameter("@gbj04", pGfaModel.gfa09));
                dtGbj = boGbj.OfGetDataTable(selectSql, sqlParmList.ToArray());

                if (pGfbModel.gfb06 == "1")
                {
                    dAmt = pGfbModel.gfb07;
                    dRec = 1;
                }
                else
                {
                    cAmt = pGfbModel.gfb07;
                    cRec = 1;
                }

                if (dtGbj.Rows.Count == 0)
                {
                    drGbj = dtGbj.NewRow();
                    drGbj["gbj01"] = pGfbModel.gfb03;
                    drGbj["gbj02"] = pGfbModel.gfb05;
                    drGbj["gbj03"] = pGfaModel.gfa08;
                    drGbj["gbj04"] = pGfaModel.gfa09;
                    drGbj["gbj05"] = dAmt;
                    drGbj["gbj06"] = cAmt;
                    drGbj["gbj07"] = dRec;
                    drGbj["gbj08"] = cRec;
                    drGbj["gbjcomp"] = _loginInfo.CompNo;
                    dtGbj.Rows.Add(drGbj);
                }
                else
                {
                    drGbj = dtGbj.Rows[0];
                    drGbj["gbj05"] = Convert.ToDecimal(drGbj["gbj05"]) + dAmt;
                    drGbj["gbj06"] = Convert.ToDecimal(drGbj["gbj06"]) + cAmt;
                    drGbj["gbj07"] = Convert.ToDecimal(drGbj["gbj07"]) + dRec;
                    drGbj["gbj08"] = Convert.ToDecimal(drGbj["gbj08"]) + cRec;
                }
                if (boGbj.OfUpdate(dtGbj) != 1)
                {
                    pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                    pResult.Message = "更新部門各期餘額檔(gbj_tb)失敗!";
                    return false;
                }

                //需遞迴處理上層部門
                selectSql = "SELECT * FROM gbd_tb WHERE gbd02=@gbd02";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbd02", pGfbModel.gfb05));
                var dtGbd = OfGetDataTable(selectSql, sqlParmList.ToArray());
                if (dtGbd == null || dtGbd.Rows.Count == 0)
                {
                    return true;
                }

                if (dtGbd.Rows.Count > 1)
                {
                    pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                    pResult.Message = "更新部門餘額檔,有多個上層部門異常!";
                    return false;
                }

                var gbdModel = dtGbd.Rows[0].ToItem<gbd_tb>();
                if (OfPostGbjRecursive(gbdModel.gbd01, pGfaModel, pGfbModel,
                                            dAmt, cAmt, dRec, cRec, pResult) == false)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                pResult.Message = "更新部門各期餘額檔(gbj_tb)失敗!";
                pResult.Exception = ex;
                return false;
            }
        }
        #endregion

        #region OfPostGbjRecursive 會科科目部門餘額檔-遞迴處理
        private bool OfPostGbjRecursive(string pGbj01Parents, gfa_tb pGfaModel, gfb_tb pGfbModel,
                                        decimal pDAmt, decimal pCAmt, decimal pDRec, decimal pCRec, Result pResult)
        {
            string selectSql = "";
            List<SqlParameter> sqlParmList = null;
            GlaBLL boGbj = null;
            DataTable dtGbj = null;
            DataRow drGbj = null;
            try
            {
                boGbj = new GlaBLL(OfGetConntion());
                boGbj.TRAN = this.TRAN;
                boGbj.OfCreateDao("gbj_tb", "*", "");
                selectSql = @"SELECT * FROM gbj_tb 
                              WHERE gbj01=@gbj01 AND gbj02=@gbj02 
                              AND gbj03=@gbj03 AND gbj04=@gbj04
                            ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbj01", pGfbModel.gfb03));
                sqlParmList.Add(new SqlParameter("@gbj02", pGbj01Parents));
                sqlParmList.Add(new SqlParameter("@gbj03", pGfaModel.gfa08));
                sqlParmList.Add(new SqlParameter("@gbj04", pGfaModel.gfa09));
                dtGbj = boGbj.OfGetDataTable(selectSql, sqlParmList.ToArray());

                if (dtGbj.Rows.Count == 0)
                {
                    drGbj = dtGbj.NewRow();
                    drGbj["gbj01"] = pGfbModel.gfb03;
                    drGbj["gbj02"] = pGbj01Parents;
                    drGbj["gbj03"] = pGfaModel.gfa08;
                    drGbj["gbj04"] = pGfaModel.gfa09;
                    drGbj["gbj05"] = pDAmt;
                    drGbj["gbj06"] = pCAmt;
                    drGbj["gbj07"] = pDRec;
                    drGbj["gbj08"] = pCRec;
                    drGbj["gbjcomp"] = _loginInfo.CompNo;
                    dtGbj.Rows.Add(drGbj);
                }
                else
                {
                    drGbj = dtGbj.Rows[0];

                    drGbj["gbj05"] = Convert.ToDecimal(drGbj["gbj05"]) + pDAmt;
                    drGbj["gbj06"] = Convert.ToDecimal(drGbj["gbj06"]) + pCAmt;
                    drGbj["gbj07"] = Convert.ToDecimal(drGbj["gbj07"]) + pDRec;
                    drGbj["gbj08"] = Convert.ToDecimal(drGbj["gbj08"]) + pCRec;

                }
                if (boGbj.OfUpdate(dtGbj) != 1)
                {
                    pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                    pResult.Message = "更新部門餘額檔,上層部門(gbj_tb)失敗!";
                    return false;
                }

                selectSql = "SELECT * FROM gbd_tb WHERE gbd02=@gbd02";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbd02", pGbj01Parents));
                var dtGbd = OfGetDataTable(selectSql, sqlParmList.ToArray());
                if (dtGbd == null || dtGbd.Rows.Count == 0)
                {
                    return true;
                }

                if (dtGbd.Rows.Count > 1)
                {
                    pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                    pResult.Message = "更新部門餘額檔,有多個上層部門異常!";
                    return false;
                }

                var gbdModel = dtGbd.Rows[0].ToItem<gbd_tb>();
                if (OfPostGbjRecursive(gbdModel.gbd01, pGfaModel, pGfbModel,
                                            pDAmt, pCAmt, pDRec, pCRec, pResult) == false)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                pResult.Key1 = string.Concat(pGfbModel.gfb01, "/", pGfbModel.gfb02);
                pResult.Message = "更新部門餘額檔,上層部門(gbj_tb)失敗!";
                pResult.Exception = ex;
                return false;
            }
        }
        #endregion

    }
}
