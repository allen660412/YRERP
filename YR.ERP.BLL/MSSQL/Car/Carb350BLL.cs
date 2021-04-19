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

namespace YR.ERP.BLL.MSSQL.Car
{
    public class Carb350BLL : YR.ERP.BLL.MSSQL.CarBLL
    {
        private UserInfo _loginInfo = null;

        #region Carb350BLL() :　建構子
        public Carb350BLL()
            : base()
        {
        }

        public Carb350BLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfGenVoucher
        public List<Result> OfGenVoucher(vw_carb350 pModel, string pSecurityString, UserInfo pLoginInfo)
        {
            List<Result> rtnResultList = null;
            Result result = null;
            string selectSql = "";
            string strQueryRange = "", strQuerySingle = "";
            List<SqlParameter> sqlParmList = null;
            DataTable dtSource = null;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            cea_tb ceaModel = null;
            cfa_tb cfaModel = null;
            gea_tb geaModel = null;
            DataRow drGea = null;
            int chkCnts = 0;
            try
            {
                _loginInfo = pLoginInfo;

                //區分應收及收款處理
                rtnResultList = new List<Result>();
                if (pModel.gea03 == 0)
                {
                    result = new Result();
                    result.Key1 = "";
                    result.Message = "類別未輸入!";
                    return rtnResultList;
                }
                if (pModel.gea03==1)  //應收
                {
                    selectSql = @"
                            SELECT cea_tb.* FROM cea_tb
                                INNER JOIN gea_tb ON cea01=gea01 AND gea02='AR' AND gea03=1 AND gea04=1
                            WHERE ceaconf='Y' AND ISNULL(cea29,'')=''
                        ";
                }
                else
                {
                    selectSql = @"
                            SELECT cfa_tb.* FROM cfa_tb
                                INNER JOIN gea_tb ON cfa01=gea01 AND gea02='AR' AND gea03=2 AND gea04=1
                            WHERE cfaconf='Y' AND ISNULL(cfa12,'')=''
                        ";
                }

                sqlParmList = new List<SqlParameter>();
                queryInfoList = new List<QueryInfo>();

                #region range 處理
                if (!GlobalFn.varIsNull(pModel.gea01))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "gea_tb";
                    queryModel.ColumnName = "gea01";
                    queryModel.ColumnType = "string";
                    queryModel.Value = pModel.gfa01;
                    queryInfoList.Add(queryModel);
                }

                if (!GlobalFn.varIsNull(pModel.ceasecg))
                {
                    queryModel = new QueryInfo();
                    if (pModel.gea03 == 1)  //應收
                    {
                        queryModel.TableName = "cea_tb";
                        queryModel.ColumnName = "ceasecg";
                    }
                    else
                    {
                        queryModel.TableName = "cfa_tb";
                        queryModel.ColumnName = "cfasecg";
                    }
                    queryModel.ColumnType = "string";
                    queryModel.Value = pModel.ceasecg;
                    queryInfoList.Add(queryModel);
                }

                sqlParmList = new List<SqlParameter>();
                strQueryRange = WfGetQueryString(queryInfoList, out sqlParmList);
                #endregion

                #region single處理
                if (!GlobalFn.varIsNull(pModel.gea05_s))
                {
                    sqlParmList.Add(new SqlParameter("@gea05_s", pModel.gea05_s));
                    strQuerySingle += string.Format(" AND gea05>=@gea05_s");
                }
                if (!GlobalFn.varIsNull(pModel.gea05_e))
                {
                    sqlParmList.Add(new SqlParameter("@gea05_e", pModel.gea05_e));
                    strQuerySingle += string.Format(" AND gea05<=@gea05_e");
                }
                #endregion

                selectSql = string.Concat(selectSql, strQueryRange, strQuerySingle, pSecurityString);       //加入權限處理
                dtSource = OfGetDataTable(selectSql, sqlParmList.ToArray());
                if (dtSource == null || dtSource.Rows.Count == 0)
                    return null;

                foreach (DataRow drTemp in dtSource.Rows)
                {
                    result = new Result();
                    rtnResultList.Add(result);
                    if (pModel.gea03 == 1)  //應收
                    {
                        ceaModel = drTemp.ToItem<cea_tb>();
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@gea01", ceaModel.cea01));
                        sqlParmList.Add(new SqlParameter("@gea03", pModel.gea03));
                    }
                    else
                    {
                        cfaModel = drTemp.ToItem<cfa_tb>();
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@gea01", cfaModel.cfa01));
                        sqlParmList.Add(new SqlParameter("@gea03", pModel.gea03));
                    }

                    selectSql = @"SELECT * FROM gea_tb 
                                WHERE gea01=@gea01 AND gea02='AR'
                                    AND gea03=@gea03 AND gea04=1
                                ";

                    drGea = OfGetDataRow(selectSql, sqlParmList.ToArray());
                    if (drGea == null)
                    {
                        result.Message = "查無分錄底稿!";
                        continue;
                    }
                    geaModel = drGea.ToItem<gea_tb>();
                    result.Key1 = geaModel.gea01;
                    
                    if (geaModel.gea08 != geaModel.gea09)
                    {
                        result.Message = "借貸不平衡!";
                        continue;
                    }

                    if (pModel.gea03 == 1)  //應收
                    {
                        if (OfGenGfa(pModel, ceaModel, geaModel, result) == false)
                        {
                            continue;
                        }
                    }
                    else  //收款沖帳
                    {
                        if (OfGenGfa(pModel, cfaModel, geaModel, result) == false)
                        {
                            continue;
                        }
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

        #region OfGenGfa
        /// <summary>
        /// 產生分錄底稿 來源為應收帳款
        /// </summary>
        /// <param name="pCarb350Model"></param>
        /// <param name="pCeaModel"></param>
        /// <param name="pGeaModel"></param>
        /// <param name="pResult"></param>
        /// <returns></returns>
        private bool OfGenGfa(vw_carb350 pCarb350Model, cea_tb pCeaModel, gea_tb pGeaModel, Result pResult)
        {
            string selectSql = "", updateSql = "";
            List<SqlParameter> sqlParmList = null;
            GlaBLL boGla = null;
            BasBLL boBas = null;
            DataTable dtGfa = null, dtGfb = null;
            DataRow drGfa = null, drGfb = null;
            string gfa01 = "", gfa01New = "";
            DateTime? gfa02;
            string errMsg = "";
            int year = 0, period = 0;
            List<geb_tb> gebList = null;
            cac_tb cacModel = null; //應收單別
            int chkCnts = 0;
            try
            {
                boGla = new GlaBLL(OfGetConntion());
                boGla.TRAN = this.TRAN;
                boBas = new BasBLL(OfGetConntion());
                boBas.TRAN = this.TRAN;

                boGla.OfCreateDao("gfa_tb", "*", "");
                selectSql = @"SELECT * FROM gfa_tb WHERE 1<>1";
                dtGfa = boGla.OfGetDataTable(selectSql);
                drGfa = dtGfa.NewRow();
                if (!GlobalFn.varIsNull(pCarb350Model.gfa01))
                {
                    gfa01 = pCarb350Model.gfa01;
                }
                else
                {
                    //以應收單別設定的來拋轉
                    cacModel = OfGetCacModel(pCeaModel.cea01);
                    if (cacModel == null)
                    {
                        pResult.Key1 = pCeaModel.cea01;
                        pResult.Message = "取得應收單別失敗!";
                        return false;  
                    }
                    if (GlobalFn.varIsNull(cacModel.cac10))
                    {
                        pResult.Key1 = pCeaModel.cea01;
                        pResult.Message = "未設定應收拋轉總帳單別,請先至單別資料設定!";
                        return false;
                    }
                    gfa01 = cacModel.cac10;
                }
                if (!GlobalFn.varIsNull(pCarb350Model.gfa02))
                    gfa02 = pCarb350Model.gfa02;
                else
                    gfa02 = pGeaModel.gea05;

                errMsg = "";
                if (boBas.OfGetAutoNo(gfa01, ModuleType.gla, (DateTime)gfa02, out gfa01New, out errMsg) == false)
                {
                    pResult.Key1 = pCeaModel.cea01;
                    pResult.Message = errMsg;
                    return false;
                }
                
                drGfa["gfa01"] = gfa01New;
                drGfa["gfa02"] = gfa02;
                drGfa["gfa03"] = pGeaModel.gea08;
                drGfa["gfa04"] = pGeaModel.gea09;
                drGfa["gfa05"] = "";        //備註
                drGfa["gfa06"] = pGeaModel.gea02;        //來源碼-系統別
                drGfa["gfa07"] = pGeaModel.gea01;        //來源單號

                if (boGla.OfGetGlaYearPeriod(Convert.ToDateTime(gfa02), out year, out period) == false)
                {
                    pResult.Message = "取得會計年度失敗!";
                    return false;
                }
                drGfa["gfa08"] = year;
                drGfa["gfa09"] = period;
                drGfa["gfa10"] = DBNull.Value;      //保留
                drGfa["gfa10"] = DBNull.Value;      //保留
                drGfa["gfa11"] = DBNull.Value;      //保留
                drGfa["gfa12"] = DBNull.Value;      //保留
                drGfa["gfa13"] = DBNull.Value;      //保留
                drGfa["gfa14"] = DBNull.Value;      //保留
                drGfa["gfa15"] = DBNull.Value;      //保留
                drGfa["gfa16"] = DBNull.Value;      //保留
                drGfa["gfa17"] = DBNull.Value;      //保留
                drGfa["gfa18"] = DBNull.Value;      //保留
                drGfa["gfa19"] = DBNull.Value;      //保留
                drGfa["gfa20"] = DBNull.Value;      //保留
                drGfa["gfaprno"] = 0;
                drGfa["gfaconf"] = "N";
                drGfa["gfaconu"] = DBNull.Value;
                drGfa["gfapost"] = "N";
                drGfa["gfaposd"] = DBNull.Value;
                drGfa["gfaposu"] = DBNull.Value;
                drGfa["gfacomp"] = _loginInfo.CompNo;
                drGfa["gfacreu"] = _loginInfo.UserNo;
                drGfa["gfacreg"] = _loginInfo.DeptNo;
                drGfa["gfacred"] = OfGetNow();
                drGfa["gfamodu"] = DBNull.Value;
                drGfa["gfamodg"] = DBNull.Value;
                drGfa["gfamodd"] = DBNull.Value;
                drGfa["gfasecu"] = _loginInfo.UserNo;
                drGfa["gfasecg"] = _loginInfo.GroupNo;
                dtGfa.Rows.Add(drGfa);

                if (boGla.OfUpdate(dtGfa) == -1)
                {
                    pResult.Key1 = pGeaModel.gea01;
                    pResult.Message = "新增傳票單頭失敗!";
                    return false;
                }

                gebList = boGla.OfGetGebList(pGeaModel.gea01, pGeaModel.gea02, pGeaModel.gea03, pGeaModel.gea04);
                if (gebList == null || gebList.Count == 0)
                {
                    pResult.Key1 = pGeaModel.gea01;
                    pResult.Message = "查無分錄底稿單身資料!";
                    return false;
                }
                
                boGla.OfCreateDao("gfb_tb", "*", "");
                selectSql = @"SELECT * FROM gfb_tb WHERE 1<>1";
                dtGfb = boGla.OfGetDataTable(selectSql);
                foreach (geb_tb gebModel in gebList.OrderBy(p => p.geb05))
                {
                    drGfb = dtGfb.NewRow();
                    drGfb["gfb01"] = gfa01New;          //傳票編號
                    drGfb["gfb02"] = gebModel.geb05;    //項次
                    drGfb["gfb03"] = gebModel.geb06;    //科目編號
                    drGfb["gfb04"] = gebModel.geb07;    //摘要
                    drGfb["gfb05"] = gebModel.geb08;    //部門
                    drGfb["gfb06"] = gebModel.geb09;    //借貸別
                    drGfb["gfb07"] = gebModel.geb10;    //本幣金額
                    drGfb["gfb08"] = gebModel.geb11;    //原幣幣別
                    drGfb["gfb09"] = gebModel.geb12;    //匯率
                    drGfb["gfb10"] = gebModel.geb13;    //原幣金額
                    drGfb["gfb11"] = DBNull.Value;      //保留
                    drGfb["gfb12"] = DBNull.Value;      //保留
                    drGfb["gfb13"] = DBNull.Value;      //保留
                    drGfb["gfb14"] = DBNull.Value;      //保留
                    drGfb["gfb15"] = DBNull.Value;      //保留
                    drGfb["gfb16"] = DBNull.Value;      //保留
                    drGfb["gfb17"] = DBNull.Value;      //保留
                    drGfb["gfb18"] = DBNull.Value;      //保留
                    drGfb["gfb19"] = DBNull.Value;      //保留
                    drGfb["gfb20"] = DBNull.Value;      //保留
                    drGfb["gfbcomp"] = _loginInfo.CompNo;
                    drGfb["gfbcreu"] = _loginInfo.UserNo;
                    drGfb["gfbcreg"] = _loginInfo.DeptNo;
                    drGfb["gfbcred"] = OfGetNow();
                    drGfb["gfbmodu"] = DBNull.Value;
                    drGfb["gfbmodg"] = DBNull.Value;
                    drGfb["gfbmodd"] = DBNull.Value;
                    dtGfb.Rows.Add(drGfb);
                }

                if (boGla.OfUpdate(dtGfb) == -1)
                {
                    pResult.Key1 = pGeaModel.gea01;
                    pResult.Message = "新增傳票單身失敗!";
                    return false;
                }

                updateSql = @"UPDATE cea_tb 
                            SET cea29=@cea29
                            WHERE cea01=@cea01
                            ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@cea29", gfa01New));
                sqlParmList.Add(new SqlParameter("@cea01", pCeaModel.cea01));
                chkCnts = OfExecuteNonquery(updateSql, sqlParmList.ToArray());
                if (chkCnts != 1)
                {
                    pResult.Key1 = pGeaModel.gea01;
                    pResult.Message = "更新應收帳款失敗!";
                    return false;
                }

                updateSql = @"UPDATE gea_tb 
                            SET gea06=@gea06,
                                gea07=@gea07
                            WHERE gea01=@gea01
                                 AND gea02=@gea02
                                AND gea03=@gea03
                                AND gea04=@gea04
                            ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gea06", gfa01New));
                sqlParmList.Add(new SqlParameter("@gea07", gfa02));
                sqlParmList.Add(new SqlParameter("@gea01", pGeaModel.gea01));
                sqlParmList.Add(new SqlParameter("@gea02", pGeaModel.gea02));
                sqlParmList.Add(new SqlParameter("@gea03", pGeaModel.gea03));
                sqlParmList.Add(new SqlParameter("@gea04", pGeaModel.gea04));
                chkCnts = OfExecuteNonquery(updateSql, sqlParmList.ToArray());
                if (chkCnts != 1)
                {
                    pResult.Key1 = pGeaModel.gea01;
                    pResult.Message = "更新應收帳款失敗!";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                pResult.Key1 = pCeaModel.cea01;
                pResult.Message = ex.Message;
                pResult.Exception = ex;
                throw;
            }
        } 
        #endregion

        #region OfGenGfa
        /// <summary>
        /// 產生分錄底稿 來源為收款沖帳單
        /// </summary>
        /// <param name="pCarb350Model"></param>
        /// <param name="pCfaModel"></param>
        /// <param name="pGeaModel"></param>
        /// <param name="pResult"></param>
        /// <returns></returns>
        private bool OfGenGfa(vw_carb350 pCarb350Model, cfa_tb pCfaModel, gea_tb pGeaModel, Result pResult)
        {
            string selectSql = "", updateSql = "";
            List<SqlParameter> sqlParmList = null;
            GlaBLL boGla = null;
            BasBLL boBas = null;
            DataTable dtGfa = null, dtGfb = null;
            DataRow drGfa = null, drGfb = null;
            string gfa01 = "", gfa01New = "";
            DateTime? gfa02;
            string errMsg = "";
            int year = 0, period = 0;
            List<geb_tb> gebList = null;
            cac_tb cacModel = null; //收款單別
            int chkCnts = 0;
            try
            {
                boGla = new GlaBLL(OfGetConntion());
                boGla.TRAN = this.TRAN;
                boBas = new BasBLL(OfGetConntion());
                boBas.TRAN = this.TRAN;

                boGla.OfCreateDao("gfa_tb", "*", "");
                selectSql = @"SELECT * FROM gfa_tb WHERE 1<>1";
                dtGfa = boGla.OfGetDataTable(selectSql);
                drGfa = dtGfa.NewRow();
                if (!GlobalFn.varIsNull(pCarb350Model.gfa01))
                {
                    gfa01 = pCarb350Model.gfa01;
                }
                else
                {
                    //以應收單別設定的來拋轉
                    cacModel = OfGetCacModel(pCfaModel.cfa01);
                    if (cacModel == null)
                    {
                        pResult.Key1 = pCfaModel.cfa01;
                        pResult.Message = "取得應收單別失敗!";
                        return false;
                    }
                    if (GlobalFn.varIsNull(cacModel.cac10))
                    {
                        pResult.Key1 = pCfaModel.cfa01;
                        pResult.Message = "未設定應收拋轉總帳單別,請先至單別資料設定!";
                        return false;
                    }
                    gfa01 = cacModel.cac10;
                }
                if (!GlobalFn.varIsNull(pCarb350Model.gfa02))
                    gfa02 = pCarb350Model.gfa02;
                else
                    gfa02 = pGeaModel.gea05;

                errMsg = "";
                if (boBas.OfGetAutoNo(gfa01, ModuleType.gla, (DateTime)gfa02, out gfa01New, out errMsg) == false)
                {
                    pResult.Key1 = pCfaModel.cfa01;
                    pResult.Message = errMsg;
                    return false;
                }

                drGfa["gfa01"] = gfa01New;
                drGfa["gfa02"] = gfa02;
                drGfa["gfa03"] = pGeaModel.gea08;
                drGfa["gfa04"] = pGeaModel.gea09;
                drGfa["gfa05"] = "";        //備註
                drGfa["gfa06"] = pGeaModel.gea02;        //來源碼-系統別
                drGfa["gfa07"] = pGeaModel.gea01;        //來源單號

                if (boGla.OfGetGlaYearPeriod(Convert.ToDateTime(gfa02), out year, out period) == false)
                {
                    pResult.Message = "取得會計年度失敗!";
                    return false;
                }
                drGfa["gfa08"] = year;
                drGfa["gfa09"] = period;
                drGfa["gfa10"] = DBNull.Value;      //保留
                drGfa["gfa10"] = DBNull.Value;      //保留
                drGfa["gfa11"] = DBNull.Value;      //保留
                drGfa["gfa12"] = DBNull.Value;      //保留
                drGfa["gfa13"] = DBNull.Value;      //保留
                drGfa["gfa14"] = DBNull.Value;      //保留
                drGfa["gfa15"] = DBNull.Value;      //保留
                drGfa["gfa16"] = DBNull.Value;      //保留
                drGfa["gfa17"] = DBNull.Value;      //保留
                drGfa["gfa18"] = DBNull.Value;      //保留
                drGfa["gfa19"] = DBNull.Value;      //保留
                drGfa["gfa20"] = DBNull.Value;      //保留
                drGfa["gfaprno"] = 0;
                drGfa["gfaconf"] = "N";
                drGfa["gfaconu"] = DBNull.Value;
                drGfa["gfapost"] = "N";
                drGfa["gfaposd"] = DBNull.Value;
                drGfa["gfaposu"] = DBNull.Value;
                drGfa["gfacomp"] = _loginInfo.CompNo;
                drGfa["gfacreu"] = _loginInfo.UserNo;
                drGfa["gfacreg"] = _loginInfo.DeptNo;
                drGfa["gfacred"] = OfGetNow();
                drGfa["gfamodu"] = DBNull.Value;
                drGfa["gfamodg"] = DBNull.Value;
                drGfa["gfamodd"] = DBNull.Value;
                drGfa["gfasecu"] = _loginInfo.UserNo;
                drGfa["gfasecg"] = _loginInfo.GroupNo;
                dtGfa.Rows.Add(drGfa);

                if (boGla.OfUpdate(dtGfa) == -1)
                {
                    pResult.Key1 = pGeaModel.gea01;
                    pResult.Message = "新增傳票單頭失敗!";
                    return false;
                }

                gebList = boGla.OfGetGebList(pGeaModel.gea01, pGeaModel.gea02, pGeaModel.gea03, pGeaModel.gea04);
                if (gebList == null || gebList.Count == 0)
                {
                    pResult.Key1 = pGeaModel.gea01;
                    pResult.Message = "查無分錄底稿單身資料!";
                    return false;
                }

                boGla.OfCreateDao("gfb_tb", "*", "");
                selectSql = @"SELECT * FROM gfb_tb WHERE 1<>1";
                dtGfb = boGla.OfGetDataTable(selectSql);
                foreach (geb_tb gebModel in gebList.OrderBy(p => p.geb05))
                {
                    drGfb = dtGfb.NewRow();
                    drGfb["gfb01"] = gfa01New;          //傳票編號
                    drGfb["gfb02"] = gebModel.geb05;    //項次
                    drGfb["gfb03"] = gebModel.geb06;    //科目編號
                    drGfb["gfb04"] = gebModel.geb07;    //摘要
                    drGfb["gfb05"] = gebModel.geb08;    //部門
                    drGfb["gfb06"] = gebModel.geb09;    //借貸別
                    drGfb["gfb07"] = gebModel.geb10;    //本幣金額
                    drGfb["gfb08"] = gebModel.geb11;    //原幣幣別
                    drGfb["gfb09"] = gebModel.geb12;    //匯率
                    drGfb["gfb10"] = gebModel.geb13;    //原幣金額
                    drGfb["gfb11"] = DBNull.Value;      //保留
                    drGfb["gfb12"] = DBNull.Value;      //保留
                    drGfb["gfb13"] = DBNull.Value;      //保留
                    drGfb["gfb14"] = DBNull.Value;      //保留
                    drGfb["gfb15"] = DBNull.Value;      //保留
                    drGfb["gfb16"] = DBNull.Value;      //保留
                    drGfb["gfb17"] = DBNull.Value;      //保留
                    drGfb["gfb18"] = DBNull.Value;      //保留
                    drGfb["gfb19"] = DBNull.Value;      //保留
                    drGfb["gfb20"] = DBNull.Value;      //保留
                    drGfb["gfbcomp"] = _loginInfo.CompNo;
                    drGfb["gfbcreu"] = _loginInfo.UserNo;
                    drGfb["gfbcreg"] = _loginInfo.DeptNo;
                    drGfb["gfbcred"] = OfGetNow();
                    drGfb["gfbmodu"] = DBNull.Value;
                    drGfb["gfbmodg"] = DBNull.Value;
                    drGfb["gfbmodd"] = DBNull.Value;
                    dtGfb.Rows.Add(drGfb);
                }

                if (boGla.OfUpdate(dtGfb) == -1)
                {
                    pResult.Key1 = pGeaModel.gea01;
                    pResult.Message = "新增傳票單身失敗!";
                    return false;
                }

                updateSql = @"UPDATE cfa_tb 
                            SET cfa12=@cfa12
                            WHERE cfa01=@cfa01
                            ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@cfa12", gfa01New));
                sqlParmList.Add(new SqlParameter("@cfa01", pCfaModel.cfa01));
                chkCnts = OfExecuteNonquery(updateSql, sqlParmList.ToArray());
                if (chkCnts != 1)
                {
                    pResult.Key1 = pGeaModel.gea01;
                    pResult.Message = "更新收款單失敗!";
                    return false;
                }

                updateSql = @"UPDATE gea_tb 
                            SET gea06=@gea06,
                                gea07=@gea07
                            WHERE gea01=@gea01
                                 AND gea02=@gea02
                                AND gea03=@gea03
                                AND gea04=@gea04
                            ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gea06", gfa01New));
                sqlParmList.Add(new SqlParameter("@gea07", gfa02));
                sqlParmList.Add(new SqlParameter("@gea01", pGeaModel.gea01));
                sqlParmList.Add(new SqlParameter("@gea02", pGeaModel.gea02));
                sqlParmList.Add(new SqlParameter("@gea03", pGeaModel.gea03));
                sqlParmList.Add(new SqlParameter("@gea04", pGeaModel.gea04));
                chkCnts = OfExecuteNonquery(updateSql, sqlParmList.ToArray());
                if (chkCnts != 1)
                {
                    pResult.Key1 = pGeaModel.gea01;
                    pResult.Message = "更新分稿底稿失敗!";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                pResult.Key1 = pCfaModel.cfa01;
                pResult.Message = ex.Message;
                pResult.Exception = ex;
                throw;
            }
        }
        #endregion
    }
}
