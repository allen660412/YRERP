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

namespace YR.ERP.BLL.MSSQL.Tax
{
    public class Taxb010BLL : YR.ERP.BLL.MSSQL.TaxBLL
    {
        
        private UserInfo _loginInfo = null;

        #region Cspb200BLL() :　建構子
        public Taxb010BLL()
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

        public Taxb010BLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        
        #region OfGenInv
        public List<Result> OfGenInv(vw_taxb010 pModel, UserInfo pLoginInfo)
        {

            List<Result> rtnResultList = null;
            TaxBLL boTax = null;
            StpBLL boStp = null;
            Result result = null;
            string selectSql = "", deleteSql = "",updateSql="";
            string strQueryRange = "", strQuerySingle = "";
            string strWhere="";
            List<SqlParameter> sqlParmList = null;
            DataTable dtTca,dtSga,dtTbe;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            sga_tb sgaModel;
            tbe_tb tbeModel;

            int chkCnts = 0;
            try
            {
                boTax = new TaxBLL(OfGetConntion());
                boStp = new StpBLL(OfGetConntion());
                boTax.TRAN = this.TRAN;
                boStp.TRAN = this.TRAN;

                boTax.OfCreateDao("tca_tb", "*", "");
                selectSql = @"SELECT * FROM tca_tb WHERE 1<>1";
                dtTca = boTax.OfGetDataTable(selectSql);

                _loginInfo = pLoginInfo;
                rtnResultList = new List<Result>();


                sqlParmList = new List<SqlParameter>();
                queryInfoList = new List<QueryInfo>();

                #region range 處理
                //if (!GlobalFn.varIsNull(pModel.jja04))
                //{
                //    queryModel = new QueryInfo();
                //    queryModel.TableName = "jja_tb";
                //    queryModel.ColumnName = "jja04";
                //    queryModel.ColumnType = "string";
                //    queryModel.Value = pModel.jja04;
                //    queryInfoList.Add(queryModel);
                //}

                //sqlParmList = new List<SqlParameter>();
                //strQueryRange = WfGetQueryString(queryInfoList, out sqlParmList);
                //strWhere = strQueryRange;
                #endregion

                #region single處理
                if (!GlobalFn.varIsNull(pModel.tca03))
                {
                    sqlParmList.Add(new SqlParameter("@tca03", pModel.tca03.Substring(4, 2)));
                    strQuerySingle += string.Format(" AND tca03=@tca03");
                }
                if (!GlobalFn.varIsNull(pModel.tca04))
                {
                    sqlParmList.Add(new SqlParameter("@tca04", pModel.tca04.Substring(4, 2)));
                    strQuerySingle += string.Format(" AND tca04=@tca04");
                }
                #endregion

                strWhere = string.Concat(strQueryRange, strQuerySingle);
                //先刪進項
                if (pModel.tca01=="0" || pModel.tca01=="1")
                {

                    deleteSql = string.Concat("DELETE FROM tca_tb WHERE tca01='1' ", strWhere);
                    chkCnts = OfExecuteNonquery(deleteSql, sqlParmList);
                }

                //再刪銷項
                if (pModel.tca01 == "0" || pModel.tca01 == "2")
                {
                    deleteSql = string.Concat("DELETE FROM tca_tb WHERE tca01='2' ", strWhere);
                    chkCnts = OfExecuteNonquery(deleteSql, sqlParmList);
                }

                //產生銷項發票
                selectSql = @"SELECT * FROM sga_tb
                            WHERE sgaconf ='Y' AND ISNULL(sga24,'') <> ''
                            ";

                #region single處理
                sqlParmList=new List<SqlParameter>();
                strWhere = "";
                sqlParmList.Add(new SqlParameter("@tca03", pModel.tca03));
                sqlParmList.Add(new SqlParameter("@tca04", pModel.tca04));  //先用出貨日期
                strWhere += string.Format(" AND convert(nvarchar(6),sga02,112) between @tca03 AND @tca04");      //先用出貨日期

                //if (!GlobalFn.varIsNull(pModel.tca03))
                //{
                //    sqlParmList.Add(new SqlParameter("@tca03", pModel.tca03));
                //    strWhere += string.Format(" AND sga02>=@tca03");      //先用出貨日期
                //}
                //if (!GlobalFn.varIsNull(pModel.tca04))
                //{
                //    sqlParmList.Add(new SqlParameter("@tca04", pModel.tca04));  //先用出貨日期
                //    strWhere += string.Format(" AND sga02<=@tca04");
                //}



                #endregion
                selectSql = string.Concat(selectSql,strWhere);
                dtSga = OfGetDataTable(selectSql,sqlParmList.ToArray());


                foreach (DataRow drSga in dtSga.Rows)
                {
                    sgaModel=drSga.ToItem<sga_tb>();
                    selectSql = @"SELECT * FROM tbe_tb
                                WHERE @sga24 between tbe07 AND tbe08
                                    AND tbe01=@tbe01
                                    AND tbe02=@tbe02
                                    AND tbe03=@tbe03
                            ";
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@sga24", sgaModel.sga24));//先用發票日期
                    sqlParmList.Add(new SqlParameter("@tbe01", pModel.tca03.Substring(0,4)));
                    sqlParmList.Add(new SqlParameter("@tbe02", pModel.tca03.Substring(4,2)));
                    sqlParmList.Add(new SqlParameter("@tbe03", pModel.tca04.Substring(4, 2)));
                    dtTbe = OfGetDataTable(selectSql, sqlParmList.ToArray());
                    if (dtTbe==null ||dtTbe.Rows.Count !=1)
                    {
                        result = new Result();
                        result.Key1 = sgaModel.sga24;
                        result.Message = "發票異常";
                        result.Success = false;
                        rtnResultList.Add(result);
                        return rtnResultList;
                    }
                    tbeModel=dtTbe.Rows[0].ToItem<tbe_tb>();

                    DataRow drTca = dtTca.NewRow();
                    drTca["tca01"] = "2";
                    drTca["tca02"] = tbeModel.tbe01;
                    drTca["tca03"] = tbeModel.tbe02;
                    drTca["tca04"] = tbeModel.tbe03;
                    drTca["tca05"] = sgaModel.sga24;
                    drTca["tca06"] = tbeModel.tbe11;
                    drTca["tca07"] = "2";   //批次轉入
                    drTca["tca08"] = tbeModel.tbe05;    //發票聯數
                    drTca["tca09"] = sgaModel.sga13;
                    drTca["tca09g"] = sgaModel.sga13g;
                    drTca["tca09t"] = sgaModel.sga13t;
                    drTca["tca10"] = sgaModel.sga02;    //先用出貨日期
                    drTca["tca11"] = "";    
                    drTca["tca12"] = "";    
                    drTca["tca13"] = "";    

                    dtTca.Rows.Add(drTca);
                }
                if (boTax.OfUpdate(dtTca) == -1)
                {
                    result = new Result();
                    //result.Key1 = sgaModel.sga24;
                    //result.Key2 = pgbModel.pgb02.ToString();
                    result.Message = "新增發票資料檔失敗!";
                    rtnResultList.Add(result);
                    return rtnResultList;
                }

                return rtnResultList;
            }
            catch (Exception ex)
            {

                throw;
            }

        } 
        #endregion
    }
}
