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

namespace YR.ERP.BLL.MSSQL
{
    public class TaxBLL : YR.ERP.BLL.MSSQL.CommonBLL
    {
        #region TaxBLL() :　建構子
        public TaxBLL()
            : base()
        {
        }

        public TaxBLL(YR.ERP.DAL.ERP_MSSQLDAL pdao)
            : base(pdao)
        {
        }


        public TaxBLL(string ps_comp, string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable)
            : base(ps_comp, ps_TargetTable, ps_TargetColumn, ps_ViewTable)
        {

        }

        public TaxBLL(string ps_comp, string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable,bool pGenNonSelectCommand=true)
            : base(ps_comp, ps_TargetTable, ps_TargetColumn, ps_ViewTable, pGenNonSelectCommand)
        {

        }

        public TaxBLL(System.Data.Common.DbConnection pConnection, string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable)
            : base(pConnection, ps_TargetTable, ps_TargetColumn, ps_ViewTable)
        {
            //this.OfCreateDao(pConnection, ps_TargetTable, ps_TargetColumn, ps_ViewTable);
        }

        public TaxBLL(System.Data.Common.DbConnection pConnection, string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable, bool pGenNonSelectCommand)
            : base(pConnection, ps_TargetTable, ps_TargetColumn, ps_ViewTable, pGenNonSelectCommand)
        {
            //this.OfCreateDao(pConnection, ps_TargetTable, ps_TargetColumn, ps_ViewTable);
        }

        public TaxBLL(DbConnection pConnection)
            : base(pConnection)
        {
            base.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfChkTbaPKExists
        public bool OfChkTbaPKExists(string pTba01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int chkCnts = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM tba_tb");
                sbSql.AppendLine("WHERE tba01=@tba01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("tba01", pTba01));

                chkCnts = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (chkCnts == 0)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkTbaPKExists
        public bool OfChkTbePKExists(decimal pTbe01, decimal pTbe02, decimal pTbe03, string pTbe04, string pTbe05, decimal pTbe06)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int chkCnts = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM tbe_tb");
                sbSql.AppendLine("WHERE tbe01=@tbe01");
                sbSql.AppendLine("AND tbe02=@tbe02");
                sbSql.AppendLine("AND tbe03=@tbe03");
                sbSql.AppendLine("AND tbe04=@tbe04");
                sbSql.AppendLine("AND tbe05=@tbe05");
                sbSql.AppendLine("AND tbe06=@tbe06");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("tbe01", pTbe01));
                sqlParmList.Add(new SqlParameter("tbe02", pTbe02));
                sqlParmList.Add(new SqlParameter("tbe03", pTbe03));
                sqlParmList.Add(new SqlParameter("tbe04", pTbe04));
                sqlParmList.Add(new SqlParameter("tbe05", pTbe05));
                sqlParmList.Add(new SqlParameter("tbe06", pTbe06));

                chkCnts = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (chkCnts == 0)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetTba 申報別資料
        public DataRow OfGetTbaDr(string pTba01)
        {
            DataRow drSbc = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM tba_tb");
                sbSql.AppendLine("WHERE tba01=@tba01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@tba01", pTba01) };
                drSbc = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drSbc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public tba_tb OfGetTbaModel(string pTba01)
        {
            DataRow drTba = null;
            tba_tb rtnModel = null;
            try
            {
                drTba = OfGetTbaDr(pTba01);
                if (drTba == null)
                    return null;
                rtnModel = drTba.ToItem<tba_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 取得發票號碼
        /// <summary>
        /// 取得發票號碼
        /// </summary>
        /// <param name="pTbe04">發票別</param>
        /// <param name="pTbe05">發票聯數</param>
        /// <param name="pInvDate">發票日期</param>
        /// <param name="sga23"></param>
        /// <returns></returns>
        public Result OfGetInvoice(string pTbe04, string pTbe05, DateTime pInvDate, out string invoice)
        {
            invoice = "";
            Result rtnResult = null;
            rtnResult = new Result();
            rtnResult.Success = false;
            string sqlSelect = "";
            int iChkCnts = 0;
            int iInvYear = 0, iInvMonth = 0;
            string strInvWord = "";//發票字軌
            string strInvNum = "";//發票號碼
            int iInvNum = 0;//發票號碼
            List<SqlParameter> sqlParmList = null;
            tbe_tb tbeModel = null;
            DataRow drTbe = null;
            
            try
            {
                //if (sgaModel == null)
                //    return rtnResult;
                //rtnResult.Key1 = sgaModel.sga01;
                if (GlobalFn.varIsNull(pTbe04))
                {
                    rtnResult.Message = "發票別不可為空值!";
                    return rtnResult;
                }

                if (GlobalFn.varIsNull(pTbe05))
                {
                    rtnResult.Message = "發票聯數不可為空值!";
                    return rtnResult;
                }

                if (GlobalFn.varIsNull(pInvDate))
                {
                    rtnResult.Message = "發票日期不可為空值!";
                    return rtnResult;
                }

                iInvYear = pInvDate.Year;
                iInvMonth = pInvDate.Month;

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@tbe01", iInvYear));
                sqlParmList.Add(new SqlParameter("@tbe02", iInvMonth));
                sqlParmList.Add(new SqlParameter("@tbe03", iInvMonth));
                sqlParmList.Add(new SqlParameter("@tbe04", pTbe04));
                sqlParmList.Add(new SqlParameter("@tbe05", pTbe05));

                sqlSelect = @"SELECT COUNT(1) FROM tbe_tb 
                                WHERE tbe01=@tbe01
                                AND tbe02<=@tbe02
                                AND tbe03>=@tbe03
                                AND tbe04=@tbe04
                                AND tbe05=@tbe05
                                AND tbe12='Y'
                                ";
                iChkCnts = GlobalFn.isNullRet(OfGetFieldValue(sqlSelect, sqlParmList.ToArray()), 0);
                if (iChkCnts==0)
                {
                    rtnResult.Message = "查無作用中的發票簿!";
                    return rtnResult;
                }

                iChkCnts = GlobalFn.isNullRet(OfGetFieldValue(sqlSelect, sqlParmList.ToArray()), 0);
                if (iChkCnts > 1)
                {
                    rtnResult.Message = "查到多筆作用中的發票簿,請先檢核!";
                    return rtnResult;
                }               


                sqlSelect = @"SELECT * FROM tbe_tb 
                                WHERE tbe01=@tbe01
                                AND tbe02<=@tbe02
                                AND tbe03>=@tbe03
                                AND tbe04=@tbe04
                                AND tbe05=@tbe05
                                AND tbe12='Y'
                                ";
                drTbe= OfGetDataRow(sqlSelect, sqlParmList.ToArray());
                tbeModel = drTbe.ToItem<tbe_tb>();
                if (GlobalFn.varIsNull(tbeModel.tbe09))
                {
                    invoice = tbeModel.tbe07;
                    rtnResult.Success = true;
                    return rtnResult;
                }
                if (tbeModel.tbe08==tbeModel.tbe09)
                {
                    rtnResult.Message = "作用中的發票簿已開立完，請重新調整!";
                    return rtnResult;

                }


                strInvWord=tbeModel.tbe09.Substring(0, 2);
                strInvNum = tbeModel.tbe09.Substring(2, 8);
                iInvNum = Convert.ToInt32(strInvNum);
                iInvNum+=1;
                invoice=strInvWord+iInvNum.ToString().PadLeft(8,'0');
                                
                rtnResult.Success = true;
                return rtnResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion

        #region OfUpdTbe09
        /// <summary>
        /// 更新發票本的已開發票號碼及日期
        /// </summary>
        /// <param name="pTbe04">發票別</param>
        /// <param name="pTbe05">發票聯數</param>
        /// <param name="pTbe09">已開發票號碼</param>
        /// <param name="pInvDate">發票日期</param>
        /// <returns></returns>
        public Result OfUpdTbe09(string pTbe04, string pTbe05, string pTbe09, DateTime pInvDate)
        {
            Result rtnResult;
            int iChkCnts = 0;
            string sqlSelect = "";
            string sqlUpdate = "";
            List<SqlParameter> sqlParmList = null;
            int iInvYear = 0, iInvMonth = 0;

            try
            {
                rtnResult = new Result();
                rtnResult.Success = false;

                iInvYear = pInvDate.Year;
                iInvMonth = pInvDate.Month;

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@tbe01", iInvYear));
                sqlParmList.Add(new SqlParameter("@tbe02", iInvMonth));
                sqlParmList.Add(new SqlParameter("@tbe03", iInvMonth));
                sqlParmList.Add(new SqlParameter("@tbe04", pTbe04));
                sqlParmList.Add(new SqlParameter("@tbe05", pTbe05));
                sqlParmList.Add(new SqlParameter("@tbe09", pTbe09));

                sqlSelect = @"
                            SELECT COUNT(1)
                            FROM tbe_tb
                            WHERE tbe01=@tbe01
                                AND tbe02<=@tbe02
                                AND tbe03>=@tbe03
                                AND tbe04=@tbe04
                                AND tbe05=@tbe05
                                AND @tbe09 between tbe07 AND tbe08
                        ";
                iChkCnts =  GlobalFn.isNullRet(OfGetFieldValue(sqlSelect,sqlParmList.ToArray()),0);
                
                if (iChkCnts==0)
                {
                    rtnResult.Message = "查無可更新資料";
                    return rtnResult;
                }


                if (iChkCnts == 0)
                {
                    rtnResult.Message = "可更新資料大於1筆,請檢核設定檔!";
                    return rtnResult;
                }

                sqlUpdate = @"
                            UPDATE tbe_tb
                            SET tbe09=@tbe09
                            WHERE tbe01=@tbe01
                                AND tbe02<=@tbe02
                                AND tbe03>=@tbe03
                                AND tbe04=@tbe04
                                AND tbe05=@tbe05
                                AND @tbe09 between tbe07 AND tbe08
                        ";
                iChkCnts = OfExecuteNonquery(sqlUpdate, sqlParmList.ToArray());
                if (iChkCnts!=1)
                {
                    rtnResult.Message = "更新發票本失敗,請檢核!";
                    return rtnResult;
                }


                rtnResult.Success = true;
                return rtnResult;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        } 
        #endregion
    }
}
