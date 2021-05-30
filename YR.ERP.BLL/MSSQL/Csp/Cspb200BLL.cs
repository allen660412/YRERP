using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YR.ERP.BLL.Model;
using YR.ERP.DAL.YRModel;
using YR.ERP.Shared;
using YR.Util;

namespace YR.ERP.BLL.MSSQL.Csp
{
    public class Cspb200BLL : YR.ERP.BLL.MSSQL.CspBLL
    {

        private UserInfo _loginInfo = null;

        #region Cspb200BLL() :　建構子
        public Cspb200BLL()
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

        public Cspb200BLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion


        #region OfGenCost
        public List<Result> OfGenCost(vw_cspb200 pModel, UserInfo pLoginInfo)
        {

            List<Result> rtnResultList = null;
            CspBLL boCsp = null;
            //PurBLL boPur = null;
            StpBLL boStp = null;
            Result result = null;
            string selectSql = "", deleteSql = "",updateSql="";
            string strQueryRange = "", strQuerySingle = "";
            string strWhere;
            List<SqlParameter> sqlParmList = null;
            //DataTable dtPga, dtPgb, dtJja, dtSha, dtShb, dtSga, dtSgb, dtPha, dtPhb, dtIna;
            DataTable dtJja,dtIna;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            string ina01, ina04 = "";

            int chkCnts = 0;
            try
            {
                boCsp = new CspBLL(OfGetConntion());
                boStp = new StpBLL(OfGetConntion());
                boCsp.TRAN = this.TRAN;
                boStp.TRAN = this.TRAN;

                boCsp.OfCreateDao("jja_tb", "*", "");
                selectSql = @"SELECT * FROM jja_tb WHERE 1<>1";
                dtJja = boCsp.OfGetDataTable(selectSql);

                _loginInfo = pLoginInfo;
                rtnResultList = new List<Result>();


                sqlParmList = new List<SqlParameter>();
                queryInfoList = new List<QueryInfo>();

                #region range 處理
                if (!GlobalFn.varIsNull(pModel.jja04))
                {
                    queryModel = new QueryInfo();
                    queryModel.TableName = "jja_tb";
                    queryModel.ColumnName = "jja04";
                    queryModel.ColumnType = "string";
                    queryModel.Value = pModel.jja04;
                    queryInfoList.Add(queryModel);
                }

                sqlParmList = new List<SqlParameter>();
                strQueryRange = WfGetQueryString(queryInfoList, out sqlParmList);
                strWhere = strQueryRange;
                #endregion

                #region single處理
                //if (!GlobalFn.varIsNull(pModel.gea05_s))
                //{
                //    sqlParmList.Add(new SqlParameter("@gea05_s", pModel.gea05_s));
                //    strQuerySingle += string.Format(" AND gea05>=@gea05_s");
                //}
                //if (!GlobalFn.varIsNull(pModel.gea05_e))
                //{
                //    sqlParmList.Add(new SqlParameter("@gea05_e", pModel.gea05_e));
                //    strQuerySingle += string.Format(" AND gea05<=@gea05_e");
                //}
                #endregion

                //先刪進項
                if (!GlobalFn.varIsNull(pModel.jja06)) //日期不為空時
                {
                    deleteSql = string.Concat("DELETE FROM jja_tb WHERE jja06>=@jja06 ", strWhere);
                    sqlParmList.Add(new SqlParameter("@jja06",pModel.jja06));
                }
                else
                {
                    deleteSql = string.Concat("DELETE FROM jja_tb WHERE 1=1 ", strWhere);
                }
                chkCnts = OfExecuteNonquery(deleteSql, sqlParmList);

                //再清空銷項
                if (!GlobalFn.varIsNull(pModel.jja06)) //日期不為空時
                {
                    updateSql = @"UPDATE jja_tb 
                                  Set jja13=null,jja14=null,jja15=null,jja16=null,jja17=null,
                                        jja17_c=null,jja18=0,jja19=0,jja20=0,jja21=null,
                                        jja22=null,jja23='N',jja24=0
                                   WHERE jja16>=@jja16";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@jja16", pModel.jja06));

                    chkCnts = OfExecuteNonquery(updateSql, sqlParmList);
                }



                //來源改取ina_tb 庫存異動明細表
                if (GlobalFn.varIsNull(pModel.jja06)) //日期為空時
                {
                    selectSql = @"SELECT distinct ina01,ina04,ina03,ina13
                              FROM ina_tb WHERE 1=1
                              ORDER BY ina03,ina13"; //先做進項
                    dtIna = OfGetDataTable(selectSql);
                }
                else
                {
                    selectSql = @"SELECT distinct ina01,ina04,ina03,ina13
                              FROM ina_tb WHERE ina13>=@ina13
                              ORDER BY ina03,ina13"; //先做進項
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@ina13", pModel.jja06));

                    dtIna = OfGetDataTable(selectSql, sqlParmList.ToArray());

                }

                foreach (DataRow drIna in dtIna.Rows)
                {
                    ina01 = GlobalFn.isNullRet(drIna["ina01"], "");
                    ina04 = GlobalFn.isNullRet(drIna["ina04"], "");

                    //依不同程式來處理資料
                    result = new Result();
                    switch (ina04.ToLower())
                    {
                        case "purt400": //進貨單 +項
                            result = OfGenCosyByPurt400(ina01);
                            if (result.Success == false)
                            {
                                rtnResultList.Add(result);
                                return rtnResultList;
                            }
                            break;
                        case "stpt500": //銷退單 +項                            
                            result = OfGenCosyByStpt500(ina01);
                            if (result.Success == false)
                            {
                                rtnResultList.Add(result);
                                return rtnResultList;
                            }
                            break;

                        case "stpt400": //出貨單 -項                       
                            result = OfGenCosyByStpt400(ina01);
                            if (result.Success == false)
                            {
                                rtnResultList.Add(result);
                                return rtnResultList;
                            }
                            break;
                        case "purt500": //進貨退回單 -項              
                            result = OfGenCosyByPurt500(ina01);
                            if (result.Success == false)
                            {
                                rtnResultList.Add(result);
                                return rtnResultList;
                            }
                            break;
                    }
                }

                #region 產生jja_tb 目前來源 1.進貨單 2.銷退單
                //1.由進貨單 產生成本進項
                //selectSql = @"SELECT * FROM pga_tb WHERE pgaconf='Y'";
                //dtPga = OfGetDataTable(selectSql);

                //foreach (DataRow drPga in dtPga.Rows)
                //{
                //    pgaModel = drPga.ToItem<pga_tb>();
                //    selectSql = @"SELECT * FROM pgb_tb WHERE pgb05>0 AND pgb01=@pgb01";
                //    sqlParmList = new List<SqlParameter>();
                //    sqlParmList.Add(new SqlParameter("@pgb01", pgaModel.pga01));

                //    dtPgb = OfGetDataTable(selectSql, sqlParmList.ToArray());
                //    foreach (DataRow drPgb in dtPgb.Rows)
                //    {
                //        pgbModel = drPgb.ToItem<pgb_tb>();
                //        for (int i = 1; i <= pgbModel.pgb05; i++)
                //        {
                //            drJja = dtJja.NewRow();
                //            drJja["jja01"] = pgbModel.pgb01;    //入庫單號
                //            drJja["jja02"] = pgbModel.pgb02;    //入庫項次
                //            drJja["jja03"] = i; //入庫序號
                //            drJja["jja04"] = pgbModel.pgb03; //料號
                //            drJja["jja05"] = pgbModel.pgb04; //品名
                //            drJja["jja06"] = pgaModel.pga02; //進項日期
                //            drJja["jja07"] = pgaModel.pga03; //進項對象
                //            drJja["jja07_c"] = boPur.OfGetPca03(pgaModel.pga03); //廠商簡稱
                //            drJja["jja08"] = GlobalFn.Round(pgbModel.pgb10t / pgbModel.pgb05, 0); //進項單價
                //            drJja["jja09"] = 0; //進項額外成本
                //            drJja["jja10"] = 0; //進項總成本
                //            drJja["jja10"] = GlobalFn.Round(pgbModel.pgb10t / pgbModel.pgb05, 0); //進項總成本
                //            drJja["jja11"] = pgbModel.pgb06; //進貨單位
                //            drJja["jja12"] = pgbModel.pgb16; //進貨倉庫
                //            drJja["jja13"] = 0; //出庫單號
                //            drJja["jja14"] = 0; //出庫項次
                //            drJja["jja15"] = 0; //出庫序號
                //            drJja["jja16"] = DBNull.Value; //出庫日期
                //            drJja["jja17"] = 0; //客戶編號
                //            drJja["jja18"] = 0; //出貨單價
                //            drJja["jja19"] = 0; //出貨額外成本
                //            drJja["jja20"] = 0; //出貨總金額
                //            drJja["jja21"] = 0; //出貨單位
                //            drJja["jja22"] = 0; //出貨倉庫
                //            drJja["jja23"] = 0; //已沖銷完否
                //            drJja["jja24"] = 0; //銷貨毛利

                //            dtJja.Rows.Add(drJja);
                //            if (boCsp.OfUpdate(dtJja) == -1)
                //            {
                //                result = new Result();
                //                result.Key1 = pgbModel.pgb01;
                //                result.Key2 = pgbModel.pgb02.ToString();
                //                result.Message = "新增成本明細表失敗!";
                //                rtnResultList.Add(result);
                //                return rtnResultList;
                //            }
                //        }

                //    }

                //}

                //2.由銷退單產生成本進項
                //selectSql = @"SELECT * FROM sha_tb WHERE shaconf='Y'";
                //dtSha = OfGetDataTable(selectSql);
                //foreach (DataRow drSha in dtSha.Rows)
                //{
                //    shaModel = drSha.ToItem<sha_tb>();
                //    selectSql = @"SELECT * FROM shb_tb WHERE shb05>0 AND shb01=@shb01";
                //    sqlParmList = new List<SqlParameter>();
                //    sqlParmList.Add(new SqlParameter("@shb01", shaModel.sha01));

                //    dtShb = OfGetDataTable(selectSql, sqlParmList.ToArray());
                //    foreach (DataRow drShb in dtShb.Rows)
                //    {
                //        shbModel = drShb.ToItem<shb_tb>();
                //        for (int i = 1; i <= shbModel.shb05; i++)
                //        {
                //            drJja = dtJja.NewRow();
                //            drJja["jja01"] = shbModel.shb01;    //入庫單號
                //            drJja["jja02"] = shbModel.shb02;    //入庫項次
                //            drJja["jja03"] = i; //入庫序號
                //            drJja["jja04"] = shbModel.shb03; //料號
                //            drJja["jja05"] = shbModel.shb04; //品名
                //            drJja["jja06"] = shaModel.sha02; //進項日期
                //            drJja["jja07"] = shaModel.sha03; //進項對象
                //            drJja["jja07_c"] = boStp.OfGetSca03(shaModel.sha03); //進項對象名稱
                //            drJja["jja08"] = GlobalFn.Round(shbModel.shb10t / shbModel.shb05, 0); //進項單價
                //            drJja["jja09"] = 0; //進項額外成本
                //            drJja["jja10"] = GlobalFn.Round(shbModel.shb10t / shbModel.shb05, 0); //進項總成本
                //            drJja["jja11"] = shbModel.shb06; //進貨單位
                //            drJja["jja12"] = shbModel.shb16; //進貨倉庫
                //            drJja["jja13"] = 0; //出庫單號
                //            drJja["jja14"] = 0; //出庫項次
                //            drJja["jja15"] = 0; //出庫序號
                //            drJja["jja16"] = DBNull.Value; //出庫日期
                //            drJja["jja17"] = 0; //客戶編號
                //            drJja["jja18"] = 0; //出貨單價
                //            drJja["jja19"] = 0; //出貨額外成本
                //            drJja["jja20"] = 0; //出貨總金額
                //            drJja["jja21"] = 0; //出貨單位
                //            drJja["jja22"] = 0; //出貨倉庫
                //            drJja["jja23"] = 0; //已沖銷完否
                //            drJja["jja24"] = 0; //銷貨毛利

                //            dtJja.Rows.Add(drJja);
                //            if (boCsp.OfUpdate(dtJja) == -1)
                //            {
                //                result = new Result();
                //                result.Key1 = shbModel.shb01;
                //                result.Key2 = shbModel.shb02.ToString();
                //                result.Message = "新增成本明細表失敗!";
                //                rtnResultList.Add(result);
                //                return rtnResultList;
                //            }
                //        }

                //    }

                //}
                #endregion


                #region 更新jaa_tb 銷項資料 1.銷貨單 2.進退單

                #region 1.由銷貨單 更新成本進項
                //                selectSql = @"SELECT * FROM sga_tb WHERE sgaconf='Y' ORDER BY sga01,sga02";
                //                dtSga = OfGetDataTable(selectSql);

                //                foreach (DataRow drSga in dtSga.Rows)
                //                {
                //                    sgaModel = drSga.ToItem<sga_tb>();
                //                    selectSql = @"SELECT * FROM sgb_tb WHERE sgb05>0 AND sgb01=@sgb01";
                //                    sqlParmList = new List<SqlParameter>();
                //                    sqlParmList.Add(new SqlParameter("@sgb01", sgaModel.sga01));

                //                    if (sgaModel.sga13t == 0)
                //                        costRate = 1;
                //                    else
                //                        costRate = (sgaModel.sga23 / sgaModel.sga13t);

                //                    dtSgb = OfGetDataTable(selectSql, sqlParmList.ToArray());
                //                    foreach (DataRow drSgb in dtSgb.Rows)
                //                    {
                //                        sgbModel = drSgb.ToItem<sgb_tb>();
                //                        for (int i = 1; i <= sgbModel.sgb05; i++)
                //                        {
                //                            selectSql = @"SELECT TOP 1 * FROM jja_tb WHERE ISNULL(jja23,'') <>'Y' 
                //                                            AND jja04=@jja04 AND jja12=@jja12 ORDER BY jja06,jja01,jja02,jja03 ";
                //                            sqlParmList = new List<SqlParameter>();
                //                            sqlParmList.Add(new SqlParameter("@jja04", sgbModel.sgb03));
                //                            sqlParmList.Add(new SqlParameter("@jja12", sgbModel.sgb16));
                //                            dtJja = boCsp.OfGetDataTable(selectSql, sqlParmList.ToArray());

                //                            if (dtJja == null || dtJja.Rows.Count == 0)
                //                            {
                //                                result = new Result();
                //                                result.Key1 = sgbModel.sgb03;
                //                                result.Key2 = sgbModel.sgb16;
                //                                result.Message = string.Format("以銷貨單更新對應成本資料時發生錯誤,料號{0},倉庫{1}", sgbModel.sgb03, sgbModel.sgb16);
                //                                rtnResultList.Add(result);
                //                                return rtnResultList;
                //                            }
                //                            drJja = dtJja.Rows[0];
                //                            drJja["jja13"] = sgbModel.sgb01; //出庫單號
                //                            drJja["jja14"] = sgbModel.sgb02; //出庫項次
                //                            drJja["jja15"] = i; //出庫序號
                //                            drJja["jja16"] = sgaModel.sga02; //出庫日期
                //                            drJja["jja17"] = sgaModel.sga03; ; //客戶編號
                //                            drJja["jja17_c"] = boStp.OfGetSca03(sgaModel.sga03);
                //                            drJja["jja18"] = GlobalFn.Round(sgbModel.sgb10t / sgbModel.sgb05, 0); //出貨單價
                //                            drJja["jja19"] = GlobalFn.Round((sgbModel.sgb10t / sgbModel.sgb05) * costRate, 0); //出貨額外成本
                //                            drJja["jja20"] = GlobalFn.isNullRet(drJja["jja18"], 0m) - GlobalFn.isNullRet(drJja["jja19"], 0m); //出貨總金額(已扣出貨成本)
                //                            drJja["jja21"] = sgbModel.sgb07; //出貨單位
                //                            drJja["jja22"] = sgbModel.sgb16; //出貨倉庫
                //                            drJja["jja23"] = 'Y'; //已沖銷完否
                //                            drJja["jja24"] = GlobalFn.isNullRet(drJja["jja20"], 0m) - GlobalFn.isNullRet(drJja["jja10"], 0m); //銷貨毛利

                //                            if (boCsp.OfUpdate(dtJja) == -1)
                //                            {
                //                                result = new Result();
                //                                result.Key1 = sgbModel.sgb01;
                //                                result.Key2 = sgbModel.sgb02.ToString();
                //                                result.Message = "更新成本明細表失敗!";
                //                                rtnResultList.Add(result);
                //                                return rtnResultList;
                //                            }
                //                        }
                //                    }

                //                }
                #endregion

                #region 2.由進貨退回單 更新成本進項
                //                selectSql = @"SELECT * FROM pha_tb WHERE phaconf='Y' ORDER BY pha01,pha02";
                //                dtPha = OfGetDataTable(selectSql);

                //                foreach (DataRow drPha in dtPha.Rows)
                //                {
                //                    try
                //                    {

                //                        phaModel = drPha.ToItem<pha_tb>();
                //                        selectSql = @"SELECT * FROM phb_tb WHERE phb05>0 AND phb01=@phb01";
                //                        sqlParmList = new List<SqlParameter>();
                //                        sqlParmList.Add(new SqlParameter("@phb01", phaModel.pha01));

                //                        dtPhb = OfGetDataTable(selectSql, sqlParmList.ToArray());
                //                        foreach (DataRow drPhb in dtPhb.Rows)
                //                        {
                //                            phbModel = drPhb.ToItem<phb_tb>();
                //                            for (int i = 1; i <= phbModel.phb05; i++)
                //                            {
                //                                selectSql = @"SELECT TOP 1 * FROM jja_tb WHERE ISNULL(jja23,'') <>'Y' 
                //                                            AND jja04=@jja04 AND jja12=@jja12 ORDER BY jja06,jja01,jja02,jja03 ";
                //                                sqlParmList = new List<SqlParameter>();
                //                                sqlParmList.Add(new SqlParameter("@jja04", phbModel.phb03));
                //                                sqlParmList.Add(new SqlParameter("@jja12", phbModel.phb16));
                //                                dtJja = boCsp.OfGetDataTable(selectSql, sqlParmList.ToArray());

                //                                if (dtJja == null || dtJja.Rows.Count == 0)
                //                                {
                //                                    result = new Result();
                //                                    result.Key1 = phbModel.phb03;
                //                                    result.Key2 = phbModel.phb16;
                //                                    result.Message = string.Format("以進貨退回單更新對應成本資料時發生錯誤,料號{0},倉庫{1}", phbModel.phb03, phbModel.phb16);
                //                                    rtnResultList.Add(result);
                //                                    return rtnResultList;
                //                                }
                //                                drJja = dtJja.Rows[0];
                //                                drJja["jja13"] = phbModel.phb01; //進退單號
                //                                drJja["jja14"] = phbModel.phb02; //進退項次
                //                                drJja["jja15"] = i; //進退序號
                //                                drJja["jja16"] = phaModel.pha02; //出庫日期
                //                                drJja["jja17"] = phaModel.pha03; ; //廠商編號
                //                                drJja["jja17_c"] = boPur.OfGetPca03(phaModel.pha03);
                //                                drJja["jja18"] = GlobalFn.Round(phbModel.phb10t / phbModel.phb05, 0); //出貨單價
                //                                drJja["jja19"] = 0; //進退額外成本
                //                                drJja["jja20"] = GlobalFn.isNullRet(drJja["jja18"], 0m) - GlobalFn.isNullRet(drJja["jja19"], 0m); //進退總金額(已扣出貨成本)
                //                                drJja["jja21"] = phbModel.phb16; //進退單位
                //                                drJja["jja22"] = phbModel.phb07; //進退倉庫
                //                                drJja["jja23"] = 'Y'; //已沖銷完否
                //                                drJja["jja24"] = GlobalFn.isNullRet(drJja["jja20"], 0m) - GlobalFn.isNullRet(drJja["jja10"], 0m); //銷貨毛利

                //                                if (boCsp.OfUpdate(dtJja) == -1)
                //                                {
                //                                    result = new Result();
                //                                    result.Key1 = phbModel.phb01;
                //                                    result.Key2 = phbModel.phb02.ToString();
                //                                    result.Message = "更新成本明細表失敗!";
                //                                    rtnResultList.Add(result);
                //                                    return rtnResultList;
                //                                }
                //                            }
                //                        }
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        throw ex;
                //                    }


                //                }
                #endregion

                #endregion
                return rtnResultList;
            }
            catch (Exception ex)
            {

                throw;
            }

        } 
        #endregion

        #region 由進貨單產生成本資料
        public Result OfGenCosyByPurt400(string pPga01)
        {
            Result rtnResult = null;
            string selectSql="";
            DataTable dtPga,dtPgb = null,dtJja=null;
            DataRow drJja = null;
            pga_tb pgaModel = null  ;
            pgb_tb pgbModel = null;
            List<SqlParameter> sqlParmList = null;
            PurBLL boPur = null;
            CspBLL boCsp = null;

            boPur = new PurBLL(OfGetConntion());
            boCsp = new CspBLL(OfGetConntion());
            boPur.TRAN = this.TRAN;
            boCsp.TRAN = this.TRAN;

            try
            {
                rtnResult = new Result();
                rtnResult.Success = false;
                rtnResult.Key1 = pPga01;
                rtnResult.Message = "由進貨單產生資料失敗!";

                boCsp.OfCreateDao("jja_tb", "*", "");
                selectSql = @"SELECT * FROM jja_tb WHERE 1<>1";
                dtJja = boCsp.OfGetDataTable(selectSql);
                
                //1.由進貨單 產生成本進項
                selectSql = @"SELECT * FROM pga_tb
                              WHERE pgaconf='Y' AND pga01=@pga01";

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@pga01", pPga01));
                dtPga = OfGetDataTable(selectSql, sqlParmList.ToArray());

                foreach (DataRow drPga in dtPga.Rows)
                {
                    pgaModel = drPga.ToItem<pga_tb>();
                    //5.16 加入只做成本管理的料號控管
                    selectSql = @"SELECT a.* FROM pgb_tb a
                                        LEFT JOIN ica_tb b ON a.pgb03=b.ica01
                                    WHERE pgb05>0 AND pgb01=@pgb01
                                        AND b.ica29='Y'
                                        ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@pgb01", pgaModel.pga01));

                    dtPgb = OfGetDataTable(selectSql, sqlParmList.ToArray());
                    foreach (DataRow drPgb in dtPgb.Rows)
                    {
                        pgbModel = drPgb.ToItem<pgb_tb>();
                        for (int i = 1; i <= pgbModel.pgb05; i++)
                        {
                            drJja = dtJja.NewRow();
                            drJja["jja01"] = pgbModel.pgb01;    //入庫單號
                            drJja["jja02"] = pgbModel.pgb02;    //入庫項次
                            drJja["jja03"] = i; //入庫序號
                            drJja["jja04"] = pgbModel.pgb03; //料號
                            drJja["jja05"] = pgbModel.pgb04; //品名
                            drJja["jja06"] = pgaModel.pga02; //進項日期
                            drJja["jja07"] = pgaModel.pga03; //進項對象
                            drJja["jja07_c"] = boPur.OfGetPca03(pgaModel.pga03); //廠商簡稱
                            drJja["jja08"] = GlobalFn.Round(pgbModel.pgb10t / pgbModel.pgb05, 0); //進項單價
                            drJja["jja09"] = 0; //進項額外成本
                            drJja["jja10"] = 0; //進項總成本
                            drJja["jja10"] = GlobalFn.Round(pgbModel.pgb10t / pgbModel.pgb05, 0); //進項總成本
                            drJja["jja11"] = pgbModel.pgb06; //進貨單位
                            drJja["jja12"] = pgbModel.pgb16; //進貨倉庫
                            drJja["jja13"] = 0; //出庫單號
                            drJja["jja14"] = 0; //出庫項次
                            drJja["jja15"] = 0; //出庫序號
                            drJja["jja16"] = DBNull.Value; //出庫日期
                            drJja["jja17"] = 0; //客戶編號
                            drJja["jja18"] = 0; //出貨單價
                            drJja["jja19"] = 0; //出貨額外成本
                            drJja["jja20"] = 0; //出貨總金額
                            drJja["jja21"] = 0; //出貨單位
                            drJja["jja22"] = 0; //出貨倉庫
                            drJja["jja23"] = 0; //已沖銷完否
                            drJja["jja24"] = 0; //銷貨毛利

                            dtJja.Rows.Add(drJja);
                        }
                        if (boCsp.OfUpdate(dtJja) == -1)
                        {
                            rtnResult = new Result();
                            rtnResult.Key1 = pgbModel.pgb01;
                            rtnResult.Key2 = pgbModel.pgb02.ToString();
                            rtnResult.Message = "新增成本明細表失敗!";
                            return rtnResult;
                        }
                    }

                }


                rtnResult = new Result();
                rtnResult.Success = true;
                return rtnResult;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        } 
        #endregion

        #region 由銷退單產生成本資料
        public Result OfGenCosyByStpt500(string pSha01)
        {
            Result rtnResult = null;
            string selectSql = "";
            DataTable dtSha, dtShb = null, dtJja = null;
            DataRow drJja = null;
            sha_tb shaModel;
            shb_tb shbModel;
            List<SqlParameter> sqlParmList = null;
            StpBLL boStp = null;
            CspBLL boCsp = null;

            boStp = new StpBLL(OfGetConntion());
            boCsp = new CspBLL(OfGetConntion());
            boStp.TRAN = this.TRAN;
            boCsp.TRAN = this.TRAN;
            boCsp.OfCreateDao("jja_tb", "*", "");
            try
            {
                //2.由銷退單產生成本進項
                selectSql = @"SELECT * FROM sha_tb WHERE shaconf='Y' AND sha01=@sha01";


                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@sha01", pSha01));
                dtSha = OfGetDataTable(selectSql,sqlParmList.ToArray());
                foreach (DataRow drSha in dtSha.Rows)
                {
                    shaModel = drSha.ToItem<sha_tb>();
                    //selectSql = @"SELECT * FROM shb_tb WHERE shb05>0 AND shb01=@shb01";
                    //5.16 加入只做成本管理的料號控管
                    selectSql = @"SELECT a.* FROM shb_tb a
                                    LEFT JOIN ica_tb b ON a.shb03=b.ica01
                                WHERE a.shb05>0 AND a.shb01=@shb01
                                    AND b.ica29='Y'";


                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@shb01", shaModel.sha01));

                    dtShb = OfGetDataTable(selectSql, sqlParmList.ToArray());
                    foreach (DataRow drShb in dtShb.Rows)
                    {
                        shbModel = drShb.ToItem<shb_tb>();
                        for (int i = 1; i <= shbModel.shb05; i++)
                        {
                            drJja = dtJja.NewRow();
                            drJja["jja01"] = shbModel.shb01;    //入庫單號
                            drJja["jja02"] = shbModel.shb02;    //入庫項次
                            drJja["jja03"] = i; //入庫序號
                            drJja["jja04"] = shbModel.shb03; //料號
                            drJja["jja05"] = shbModel.shb04; //品名
                            drJja["jja06"] = shaModel.sha02; //進項日期
                            drJja["jja07"] = shaModel.sha03; //進項對象
                            drJja["jja07_c"] = boStp.OfGetSca03(shaModel.sha03); //進項對象名稱
                            drJja["jja08"] = GlobalFn.Round(shbModel.shb10t / shbModel.shb05, 0); //進項單價
                            drJja["jja09"] = 0; //進項額外成本
                            drJja["jja10"] = GlobalFn.Round(shbModel.shb10t / shbModel.shb05, 0); //進項總成本
                            drJja["jja11"] = shbModel.shb06; //進貨單位
                            drJja["jja12"] = shbModel.shb16; //進貨倉庫
                            drJja["jja13"] = 0; //出庫單號
                            drJja["jja14"] = 0; //出庫項次
                            drJja["jja15"] = 0; //出庫序號
                            drJja["jja16"] = DBNull.Value; //出庫日期
                            drJja["jja17"] = 0; //客戶編號
                            drJja["jja18"] = 0; //出貨單價
                            drJja["jja19"] = 0; //出貨額外成本
                            drJja["jja20"] = 0; //出貨總金額
                            drJja["jja21"] = 0; //出貨單位
                            drJja["jja22"] = 0; //出貨倉庫
                            drJja["jja23"] = 0; //已沖銷完否
                            drJja["jja24"] = 0; //銷貨毛利

                            dtJja.Rows.Add(drJja);
                            if (boCsp.OfUpdate(dtJja) == -1)
                            {
                                rtnResult = new Result();
                                rtnResult.Key1 = shbModel.shb01;
                                rtnResult.Key2 = shbModel.shb02.ToString();
                                rtnResult.Message = "新增成本明細表失敗!";
                                return rtnResult;
                            }
                        }

                    }

                }



                rtnResult = new Result();
                rtnResult.Success = true;
                return rtnResult;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        } 
        #endregion

        #region 由出貨單產生成本資料
        public Result OfGenCosyByStpt400(string pSga01)
        {
            Result rtnResult = null;
            string selectSql = "";
            DataTable dtSga = null, dtSgb = null, dtJja = null;
            DataRow drJja = null;
            sga_tb sgaModel;
            sgb_tb sgbModel;
            List<SqlParameter> sqlParmList = null;
            StpBLL boStp = null;
            CspBLL boCsp = null;
            decimal costRate = 0;//銷貨成本

            boStp = new StpBLL(OfGetConntion());
            boCsp = new CspBLL(OfGetConntion());
            boStp.TRAN = this.TRAN;
            boCsp.TRAN = this.TRAN;
            boCsp.OfCreateDao("jja_tb", "*", "");
            try
            {
                selectSql = @"SELECT * FROM sga_tb WHERE sgaconf='Y' AND sga01=@sga01";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@sga01", pSga01));
                dtSga = OfGetDataTable(selectSql, sqlParmList.ToArray());


                foreach (DataRow drSga in dtSga.Rows)
                {
                    sgaModel = drSga.ToItem<sga_tb>();
                    //selectSql = @"SELECT * FROM sgb_tb WHERE sgb05>0 AND sgb01=@sgb01";
                    //5.16 加入只做成本管理的料號控管
                    selectSql = @"SELECT a.* FROM sgb_tb a
                                        LEFT JOIN ica_tb b ON a.sgb03=ica01
                                    WHERE a.sgb05>0 AND a.sgb01=@sgb01
                                        AND b.ica29='Y'
                                ";

                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@sgb01", sgaModel.sga01));
                    dtSgb = OfGetDataTable(selectSql, sqlParmList.ToArray());


                    //sga13t改用成本計算的料號去代替
                    selectSql = @"SELECT SUM(a.sgb10t) FROM sgb_tb a
                                        LEFT JOIN ica_tb b ON a.sgb03=ica01
                                    WHERE a.sgb05>0 AND a.sgb01=@sgb01
                                        AND b.ica29='Y'
                                ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@sgb01", sgaModel.sga01));
                    sgaModel.sga13t = GlobalFn.isNullRet(boCsp.OfGetFieldValue(selectSql, sqlParmList.ToArray()),0);
                    if (sgaModel.sga13t == 0)
                        costRate = 1;
                    else
                        costRate = (sgaModel.sga23 / sgaModel.sga13t);


                    foreach (DataRow drSgb in dtSgb.Rows)
                    {
                        sgbModel = drSgb.ToItem<sgb_tb>();
                        for (int i = 1; i <= sgbModel.sgb05; i++)
                        {
                            selectSql = @"SELECT TOP 1 * FROM jja_tb WHERE ISNULL(jja23,'') <>'Y' 
                                            AND jja04=@jja04 AND jja12=@jja12 ORDER BY jja06,jja01,jja02,jja03 ";
                            sqlParmList = new List<SqlParameter>();
                            sqlParmList.Add(new SqlParameter("@jja04", sgbModel.sgb03));
                            sqlParmList.Add(new SqlParameter("@jja12", sgbModel.sgb16));
                            dtJja = boCsp.OfGetDataTable(selectSql, sqlParmList.ToArray());

                            if (dtJja == null || dtJja.Rows.Count == 0)
                            {
                                rtnResult = new Result();
                                rtnResult.Key1 = sgbModel.sgb03;
                                rtnResult.Key2 = sgbModel.sgb16;
                                rtnResult.Message = string.Format("以銷貨單更新對應成本資料時發生錯誤,料號{0},倉庫{1}", sgbModel.sgb03, sgbModel.sgb16);
                                return rtnResult;
                            }
                            drJja = dtJja.Rows[0];
                            drJja["jja13"] = sgbModel.sgb01; //出庫單號
                            drJja["jja14"] = sgbModel.sgb02; //出庫項次
                            drJja["jja15"] = i; //出庫序號
                            drJja["jja16"] = sgaModel.sga02; //出庫日期
                            drJja["jja17"] = sgaModel.sga03; ; //客戶編號
                            drJja["jja17_c"] = boStp.OfGetSca03(sgaModel.sga03);
                            drJja["jja18"] = GlobalFn.Round(sgbModel.sgb10t / sgbModel.sgb05, 0); //出貨單價
                            drJja["jja19"] = GlobalFn.Round((sgbModel.sgb10t / sgbModel.sgb05) * costRate, 0); //出貨額外成本
                            drJja["jja20"] = GlobalFn.isNullRet(drJja["jja18"], 0m) - GlobalFn.isNullRet(drJja["jja19"], 0m); //出貨總金額(已扣出貨成本)
                            drJja["jja21"] = sgbModel.sgb07; //出貨單位
                            drJja["jja22"] = sgbModel.sgb16; //出貨倉庫
                            drJja["jja23"] = 'Y'; //已沖銷完否
                            drJja["jja24"] = GlobalFn.isNullRet(drJja["jja20"], 0m) - GlobalFn.isNullRet(drJja["jja10"], 0m); //銷貨毛利

                            if (boCsp.OfUpdate(dtJja) == -1)
                            {
                                rtnResult = new Result();
                                rtnResult.Key1 = sgbModel.sgb01;
                                rtnResult.Key2 = sgbModel.sgb02.ToString();
                                rtnResult.Message = "更新成本明細表失敗!";
                                return rtnResult;
                            }
                        }
                    }
                }

                rtnResult = new Result();
                rtnResult.Success = true;
                return rtnResult;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region 由進貨退回單單產生成本資料
        public Result OfGenCosyByPurt500(string pPha01)
        {
            Result rtnResult = null;
            string selectSql = "";
            DataTable dtPha = null, dtPhb = null, dtJja = null;
            DataRow drJja = null;
            pha_tb phaModel;
            phb_tb phbModel;
            List<SqlParameter> sqlParmList = null;
            PurBLL boPur = null;
            CspBLL boCsp = null;

            boPur = new PurBLL(OfGetConntion());
            boCsp = new CspBLL(OfGetConntion());
            boPur.TRAN = this.TRAN;
            boCsp.TRAN = this.TRAN;
            boCsp.OfCreateDao("jja_tb", "*", "");
            //selectSql = @"SELECT * FROM pha_tb WHERE phaconf='Y' ORDER BY pha01,pha02";
            //dtPha = OfGetDataTable(selectSql);


            selectSql = @"SELECT * FROM pha_tb WHERE phaconf='Y' AND pha01=@pha01";
            sqlParmList = new List<SqlParameter>();
            sqlParmList.Add(new SqlParameter("@pha01", pPha01));
            dtPha = OfGetDataTable(selectSql, sqlParmList.ToArray());

            foreach (DataRow drPha in dtPha.Rows)
            {
                try
                {

                    phaModel = drPha.ToItem<pha_tb>();
                    //selectSql = @"SELECT * FROM phb_tb WHERE phb05>0 AND phb01=@phb01";
                    //5.16 加入只做成本管理的料號控管
                    selectSql = @"SELECT a.* FROM phb_tb a
                                        LEFT JOIN ica_tb b on a.phb03=b.ica01
                                    WHERE a.phb05>0 AND a.phb01=@phb01
                                        AND b.ica29='Y'";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@phb01", phaModel.pha01));

                    dtPhb = OfGetDataTable(selectSql, sqlParmList.ToArray());
                    foreach (DataRow drPhb in dtPhb.Rows)
                    {
                        phbModel = drPhb.ToItem<phb_tb>();
                        for (int i = 1; i <= phbModel.phb05; i++)
                        {
                            selectSql = @"SELECT TOP 1 * FROM jja_tb WHERE ISNULL(jja23,'') <>'Y' 
                                            AND jja04=@jja04 AND jja12=@jja12 ORDER BY jja06,jja01,jja02,jja03 ";
                            sqlParmList = new List<SqlParameter>();
                            sqlParmList.Add(new SqlParameter("@jja04", phbModel.phb03));
                            sqlParmList.Add(new SqlParameter("@jja12", phbModel.phb16));
                            dtJja = boCsp.OfGetDataTable(selectSql, sqlParmList.ToArray());

                            if (dtJja == null || dtJja.Rows.Count == 0)
                            {
                                rtnResult = new Result();
                                rtnResult.Key1 = phbModel.phb03;
                                rtnResult.Key2 = phbModel.phb16;
                                rtnResult.Message = string.Format("以進貨退回單更新對應成本資料時發生錯誤,料號{0},倉庫{1}", phbModel.phb03, phbModel.phb16);
                                return rtnResult;
                            }
                            drJja = dtJja.Rows[0];
                            drJja["jja13"] = phbModel.phb01; //進退單號
                            drJja["jja14"] = phbModel.phb02; //進退項次
                            drJja["jja15"] = i; //進退序號
                            drJja["jja16"] = phaModel.pha02; //出庫日期
                            drJja["jja17"] = phaModel.pha03; ; //廠商編號
                            drJja["jja17_c"] = boPur.OfGetPca03(phaModel.pha03);
                            drJja["jja18"] = GlobalFn.Round(phbModel.phb10t / phbModel.phb05, 0); //出貨單價
                            drJja["jja19"] = 0; //進退額外成本
                            drJja["jja20"] = GlobalFn.isNullRet(drJja["jja18"], 0m) - GlobalFn.isNullRet(drJja["jja19"], 0m); //進退總金額(已扣出貨成本)
                            drJja["jja21"] = phbModel.phb16; //進退單位
                            drJja["jja22"] = phbModel.phb07; //進退倉庫
                            drJja["jja23"] = 'Y'; //已沖銷完否
                            drJja["jja24"] = GlobalFn.isNullRet(drJja["jja20"], 0m) - GlobalFn.isNullRet(drJja["jja10"], 0m); //銷貨毛利

                            if (boCsp.OfUpdate(dtJja) == -1)
                            {
                                rtnResult = new Result();
                                rtnResult.Key1 = phbModel.phb01;
                                rtnResult.Key2 = phbModel.phb02.ToString();
                                rtnResult.Message = "更新成本明細表失敗!";
                                return rtnResult;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            rtnResult = new Result();
            rtnResult.Success = true;
            return rtnResult;
        }
        #endregion


//        public List<Result> OfGenCost(vw_cspb200 pModel, UserInfo pLoginInfo)
//        {

//            List<Result> rtnResultList = null;
//            CspBLL boCsp = null;
//            PurBLL boPur = null;
//            StpBLL boStp = null;
//            Result result = null;
//            string selectSql = "", deleteSql = "";
//            string strQueryRange = "", strQuerySingle = "";
//            StringBuilder sbSelect;
//            string strWhere;
//            decimal costRate = 0;//銷貨成本
//            List<SqlParameter> sqlParmList = null;
//            DataTable dtPga, dtPgb, dtJja, dtSha, dtShb, dtSga, dtSgb, dtPha, dtPhb;
//            List<QueryInfo> queryInfoList;
//            QueryInfo queryModel;
//            jja_tb jjaModel = null;
//            DataRow drJja = null;
//            pga_tb pgaModel = null;
//            pgb_tb pgbModel = null;
//            sha_tb shaModel = null;
//            shb_tb shbModel = null;
//            sga_tb sgaModel = null;
//            sgb_tb sgbModel = null;
//            pha_tb phaModel = null;
//            phb_tb phbModel = null;

//            int chkCnts = 0;
//            try
//            {
//                boCsp = new CspBLL(OfGetConntion());
//                boPur = new PurBLL(OfGetConntion());
//                boStp = new StpBLL(OfGetConntion());
//                boCsp.TRAN = this.TRAN;
//                boPur.TRAN = this.TRAN;
//                boStp.TRAN = this.TRAN;

//                boCsp.OfCreateDao("jja_tb", "*", "");
//                selectSql = @"SELECT * FROM jja_tb WHERE 1<>1";
//                dtJja = boCsp.OfGetDataTable(selectSql);

//                _loginInfo = pLoginInfo;
//                rtnResultList = new List<Result>();


//                sqlParmList = new List<SqlParameter>();
//                queryInfoList = new List<QueryInfo>();

//                #region range 處理
//                if (!GlobalFn.varIsNull(pModel.jja04))
//                {
//                    queryModel = new QueryInfo();
//                    queryModel.TableName = "jja_tb";
//                    queryModel.ColumnName = "jja04";
//                    queryModel.ColumnType = "string";
//                    queryModel.Value = pModel.jja04;
//                    queryInfoList.Add(queryModel);
//                }

//                sqlParmList = new List<SqlParameter>();
//                strQueryRange = WfGetQueryString(queryInfoList, out sqlParmList);
//                strWhere = strQueryRange;
//                #endregion

//                #region single處理
//                //if (!GlobalFn.varIsNull(pModel.gea05_s))
//                //{
//                //    sqlParmList.Add(new SqlParameter("@gea05_s", pModel.gea05_s));
//                //    strQuerySingle += string.Format(" AND gea05>=@gea05_s");
//                //}
//                //if (!GlobalFn.varIsNull(pModel.gea05_e))
//                //{
//                //    sqlParmList.Add(new SqlParameter("@gea05_e", pModel.gea05_e));
//                //    strQuerySingle += string.Format(" AND gea05<=@gea05_e");
//                //}
//                #endregion

//                deleteSql = string.Concat("DELETE FROM jja_tb WHERE 1=1 ", strWhere);
//                chkCnts = OfExecuteNonquery(deleteSql, sqlParmList);


//                #region 產生jja_tb 目前來源 1.進貨單 2.銷退單
//                //1.由進貨單 產生成本進項
//                selectSql = @"SELECT * FROM pga_tb WHERE pgaconf='Y'";
//                dtPga = OfGetDataTable(selectSql);

//                foreach (DataRow drPga in dtPga.Rows)
//                {
//                    pgaModel = drPga.ToItem<pga_tb>();
//                    selectSql = @"SELECT * FROM pgb_tb WHERE pgb05>0 AND pgb01=@pgb01";
//                    sqlParmList = new List<SqlParameter>();
//                    sqlParmList.Add(new SqlParameter("@pgb01", pgaModel.pga01));

//                    dtPgb = OfGetDataTable(selectSql, sqlParmList.ToArray());
//                    foreach (DataRow drPgb in dtPgb.Rows)
//                    {
//                        pgbModel = drPgb.ToItem<pgb_tb>();
//                        for (int i = 1; i <= pgbModel.pgb05; i++)
//                        {
//                            drJja = dtJja.NewRow();
//                            drJja["jja01"] = pgbModel.pgb01;    //入庫單號
//                            drJja["jja02"] = pgbModel.pgb02;    //入庫項次
//                            drJja["jja03"] = i; //入庫序號
//                            drJja["jja04"] = pgbModel.pgb03; //料號
//                            drJja["jja05"] = pgbModel.pgb04; //品名
//                            drJja["jja06"] = pgaModel.pga02; //進項日期
//                            drJja["jja07"] = pgaModel.pga03; //進項對象
//                            drJja["jja07_c"] = boPur.OfGetPca03(pgaModel.pga03); //廠商簡稱
//                            drJja["jja08"] = GlobalFn.Round(pgbModel.pgb10t / pgbModel.pgb05, 0); //進項單價
//                            drJja["jja09"] = 0; //進項額外成本
//                            drJja["jja10"] = 0; //進項總成本
//                            drJja["jja10"] = GlobalFn.Round(pgbModel.pgb10t / pgbModel.pgb05, 0); //進項總成本
//                            drJja["jja11"] = pgbModel.pgb06; //進貨單位
//                            drJja["jja12"] = pgbModel.pgb16; //進貨倉庫
//                            drJja["jja13"] = 0; //出庫單號
//                            drJja["jja14"] = 0; //出庫項次
//                            drJja["jja15"] = 0; //出庫序號
//                            drJja["jja16"] = DBNull.Value; //出庫日期
//                            drJja["jja17"] = 0; //客戶編號
//                            drJja["jja18"] = 0; //出貨單價
//                            drJja["jja19"] = 0; //出貨額外成本
//                            drJja["jja20"] = 0; //出貨總金額
//                            drJja["jja21"] = 0; //出貨單位
//                            drJja["jja22"] = 0; //出貨倉庫
//                            drJja["jja23"] = 0; //已沖銷完否
//                            drJja["jja24"] = 0; //銷貨毛利

//                            dtJja.Rows.Add(drJja);
//                            if (boCsp.OfUpdate(dtJja) == -1)
//                            {
//                                result = new Result();
//                                result.Key1 = pgbModel.pgb01;
//                                result.Key2 = pgbModel.pgb02.ToString();
//                                result.Message = "新增成本明細表失敗!";
//                                rtnResultList.Add(result);
//                                return rtnResultList;
//                            }
//                        }

//                    }

//                }

//                //2.由銷退單產生成本進項
//                selectSql = @"SELECT * FROM sha_tb WHERE shaconf='Y'";
//                dtSha = OfGetDataTable(selectSql);
//                foreach (DataRow drSha in dtSha.Rows)
//                {
//                    shaModel = drSha.ToItem<sha_tb>();
//                    selectSql = @"SELECT * FROM shb_tb WHERE shb05>0 AND shb01=@shb01";
//                    sqlParmList = new List<SqlParameter>();
//                    sqlParmList.Add(new SqlParameter("@shb01", shaModel.sha01));

//                    dtShb = OfGetDataTable(selectSql, sqlParmList.ToArray());
//                    foreach (DataRow drShb in dtShb.Rows)
//                    {
//                        shbModel = drShb.ToItem<shb_tb>();
//                        for (int i = 1; i <= shbModel.shb05; i++)
//                        {
//                            drJja = dtJja.NewRow();
//                            drJja["jja01"] = shbModel.shb01;    //入庫單號
//                            drJja["jja02"] = shbModel.shb02;    //入庫項次
//                            drJja["jja03"] = i; //入庫序號
//                            drJja["jja04"] = shbModel.shb03; //料號
//                            drJja["jja05"] = shbModel.shb04; //品名
//                            drJja["jja06"] = shaModel.sha02; //進項日期
//                            drJja["jja07"] = shaModel.sha03; //進項對象
//                            drJja["jja07_c"] = boStp.OfGetSca03(shaModel.sha03); //進項對象名稱
//                            drJja["jja08"] = GlobalFn.Round(shbModel.shb10t / shbModel.shb05,0); //進項單價
//                            drJja["jja09"] = 0; //進項額外成本
//                            drJja["jja10"] = GlobalFn.Round(shbModel.shb10t / shbModel.shb05, 0); //進項總成本
//                            drJja["jja11"] = shbModel.shb06; //進貨單位
//                            drJja["jja12"] = shbModel.shb16; //進貨倉庫
//                            drJja["jja13"] = 0; //出庫單號
//                            drJja["jja14"] = 0; //出庫項次
//                            drJja["jja15"] = 0; //出庫序號
//                            drJja["jja16"] = DBNull.Value; //出庫日期
//                            drJja["jja17"] = 0; //客戶編號
//                            drJja["jja18"] = 0; //出貨單價
//                            drJja["jja19"] = 0; //出貨額外成本
//                            drJja["jja20"] = 0; //出貨總金額
//                            drJja["jja21"] = 0; //出貨單位
//                            drJja["jja22"] = 0; //出貨倉庫
//                            drJja["jja23"] = 0; //已沖銷完否
//                            drJja["jja24"] = 0; //銷貨毛利

//                            dtJja.Rows.Add(drJja);
//                            if (boCsp.OfUpdate(dtJja) == -1)
//                            {
//                                result = new Result();
//                                result.Key1 = shbModel.shb01;
//                                result.Key2 = shbModel.shb02.ToString();
//                                result.Message = "新增成本明細表失敗!";
//                                rtnResultList.Add(result);
//                                return rtnResultList;
//                            }
//                        }

//                    }

//                }
//                #endregion


//                #region 更新jaa_tb 銷項資料 1.銷貨單 2.進退單

//                #region 1.由銷貨單 更新成本進項
//                selectSql = @"SELECT * FROM sga_tb WHERE sgaconf='Y' ORDER BY sga01,sga02";
//                dtSga = OfGetDataTable(selectSql);

//                foreach (DataRow drSga in dtSga.Rows)
//                {
//                    sgaModel = drSga.ToItem<sga_tb>();
//                    selectSql = @"SELECT * FROM sgb_tb WHERE sgb05>0 AND sgb01=@sgb01";
//                    sqlParmList = new List<SqlParameter>();
//                    sqlParmList.Add(new SqlParameter("@sgb01", sgaModel.sga01));

//                    if (sgaModel.sga13t == 0)
//                        costRate = 1;
//                    else
//                        costRate = (sgaModel.sga23 / sgaModel.sga13t);

//                    dtSgb = OfGetDataTable(selectSql, sqlParmList.ToArray());
//                    foreach (DataRow drSgb in dtSgb.Rows)
//                    {
//                        sgbModel = drSgb.ToItem<sgb_tb>();
//                        for (int i = 1; i <= sgbModel.sgb05; i++)
//                        {
//                            selectSql = @"SELECT TOP 1 * FROM jja_tb WHERE ISNULL(jja23,'') <>'Y' 
//                                            AND jja04=@jja04 AND jja12=@jja12 ORDER BY jja06,jja01,jja02,jja03 ";
//                            sqlParmList = new List<SqlParameter>();
//                            sqlParmList.Add(new SqlParameter("@jja04", sgbModel.sgb03));
//                            sqlParmList.Add(new SqlParameter("@jja12", sgbModel.sgb16));
//                            dtJja = boCsp.OfGetDataTable(selectSql, sqlParmList.ToArray());

//                            if (dtJja == null || dtJja.Rows.Count == 0)
//                            {
//                                result = new Result();
//                                result.Key1 = sgbModel.sgb03;
//                                result.Key2 = sgbModel.sgb16;
//                                result.Message = string.Format("以銷貨單更新對應成本資料時發生錯誤,料號{0},倉庫{1}", sgbModel.sgb03, sgbModel.sgb16);
//                                rtnResultList.Add(result);
//                                return rtnResultList;
//                            }
//                            drJja = dtJja.Rows[0];
//                            drJja["jja13"] = sgbModel.sgb01; //出庫單號
//                            drJja["jja14"] = sgbModel.sgb02; //出庫項次
//                            drJja["jja15"] = i; //出庫序號
//                            drJja["jja16"] = sgaModel.sga02; //出庫日期
//                            drJja["jja17"] = sgaModel.sga03; ; //客戶編號
//                            drJja["jja17_c"] = boStp.OfGetSca03(sgaModel.sga03);
//                            drJja["jja18"] = GlobalFn.Round(sgbModel.sgb10t / sgbModel.sgb05,0); //出貨單價
//                            drJja["jja19"] = GlobalFn.Round((sgbModel.sgb10t / sgbModel.sgb05) * costRate,0); //出貨額外成本
//                            drJja["jja20"] = GlobalFn.isNullRet(drJja["jja18"], 0m) - GlobalFn.isNullRet(drJja["jja19"], 0m); //出貨總金額(已扣出貨成本)
//                            drJja["jja21"] = sgbModel.sgb07; //出貨單位
//                            drJja["jja22"] = sgbModel.sgb16; //出貨倉庫
//                            drJja["jja23"] = 'Y'; //已沖銷完否
//                            drJja["jja24"] = GlobalFn.isNullRet(drJja["jja20"], 0m) - GlobalFn.isNullRet(drJja["jja10"], 0m); //銷貨毛利

//                            if (boCsp.OfUpdate(dtJja) == -1)
//                            {
//                                result = new Result();
//                                result.Key1 = sgbModel.sgb01;
//                                result.Key2 = sgbModel.sgb02.ToString();
//                                result.Message = "更新成本明細表失敗!";
//                                rtnResultList.Add(result);
//                                return rtnResultList;
//                            }
//                        }
//                    }

//                } 
//                #endregion

//                #region 2.由進貨退回單 更新成本進項                
//                selectSql = @"SELECT * FROM pha_tb WHERE phaconf='Y' ORDER BY pha01,pha02";
//                dtPha = OfGetDataTable(selectSql);

//                foreach (DataRow drPha in dtPha.Rows)
//                {
//                    try
//                    {

//                        phaModel = drPha.ToItem<pha_tb>();
//                        selectSql = @"SELECT * FROM phb_tb WHERE phb05>0 AND phb01=@phb01";
//                        sqlParmList = new List<SqlParameter>();
//                        sqlParmList.Add(new SqlParameter("@phb01", phaModel.pha01));

//                        dtPhb = OfGetDataTable(selectSql, sqlParmList.ToArray());
//                        foreach (DataRow drPhb in dtPhb.Rows)
//                        {
//                            phbModel = drPhb.ToItem<phb_tb>();
//                            for (int i = 1; i <= phbModel.phb05; i++)
//                            {
//                                selectSql = @"SELECT TOP 1 * FROM jja_tb WHERE ISNULL(jja23,'') <>'Y' 
//                                            AND jja04=@jja04 AND jja12=@jja12 ORDER BY jja06,jja01,jja02,jja03 ";
//                                sqlParmList = new List<SqlParameter>();
//                                sqlParmList.Add(new SqlParameter("@jja04", phbModel.phb03));
//                                sqlParmList.Add(new SqlParameter("@jja12", phbModel.phb16));
//                                dtJja = boCsp.OfGetDataTable(selectSql, sqlParmList.ToArray());

//                                if (dtJja == null || dtJja.Rows.Count == 0)
//                                {
//                                    result = new Result();
//                                    result.Key1 = phbModel.phb03;
//                                    result.Key2 = phbModel.phb16;
//                                    result.Message = string.Format("以進貨退回單更新對應成本資料時發生錯誤,料號{0},倉庫{1}", phbModel.phb03, phbModel.phb16);
//                                    rtnResultList.Add(result);
//                                    return rtnResultList;
//                                }
//                                drJja = dtJja.Rows[0];
//                                drJja["jja13"] = phbModel.phb01; //進退單號
//                                drJja["jja14"] = phbModel.phb02; //進退項次
//                                drJja["jja15"] = i; //進退序號
//                                drJja["jja16"] = phaModel.pha02; //出庫日期
//                                drJja["jja17"] = phaModel.pha03; ; //廠商編號
//                                drJja["jja17_c"] = boPur.OfGetPca03(phaModel.pha03);
//                                drJja["jja18"] = GlobalFn.Round(phbModel.phb10t / phbModel.phb05, 0); //出貨單價
//                                drJja["jja19"] = 0; //進退額外成本
//                                drJja["jja20"] = GlobalFn.isNullRet(drJja["jja18"], 0m) - GlobalFn.isNullRet(drJja["jja19"], 0m); //進退總金額(已扣出貨成本)
//                                drJja["jja21"] = phbModel.phb16; //進退單位
//                                drJja["jja22"] = phbModel.phb07; //進退倉庫
//                                drJja["jja23"] = 'Y'; //已沖銷完否
//                                drJja["jja24"] = GlobalFn.isNullRet(drJja["jja20"], 0m) - GlobalFn.isNullRet(drJja["jja10"], 0m); //銷貨毛利

//                                if (boCsp.OfUpdate(dtJja) == -1)
//                                {
//                                    result = new Result();
//                                    result.Key1 = phbModel.phb01;
//                                    result.Key2 = phbModel.phb02.ToString();
//                                    result.Message = "更新成本明細表失敗!";
//                                    rtnResultList.Add(result);
//                                    return rtnResultList;
//                                }
//                            }
//                        }
//                    }
//                    catch (Exception ex)
//                    {                        
//                        throw ex;
//                    }


//                }
//                #endregion

//                #endregion
//                return rtnResultList;
//            }
//            catch (Exception ex)
//            {

//                throw;
//            }

//        }
    }
}
