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
    public class Carb351BLL : YR.ERP.BLL.MSSQL.CarBLL
    {
        private UserInfo _loginInfo = null;

        #region Carb351BLL() :　建構子
        public Carb351BLL()
            : base()
        {
        }

        public Carb351BLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfGenVoucher
        public List<Result> OfUndoGenVoucher(vw_carb351 pModel, string pSecurityString, UserInfo pLoginInfo)
        {
            List<Result> rtnResultList = null;
            string selectSql = "";
            string updateGea = "", updateCea = "",updateCfa="";
            string deleteGfa = "", deleteGfb = "";
            string strQueryRange = "", strQuerySingle = "";
            List<SqlParameter> sqlParmList = null;
            DataTable dtGfa = null;
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            cea_tb ceaModel = null;
            gfa_tb gfaModel = null;            
            DataRow drGea = null;
            int chkCnts = 0;
            string cac04 = "";
            try
            {
                _loginInfo = pLoginInfo;

                selectSql = @"
                            SELECT * FROM gfa_tb
                            WHERE gfa06='AR' AND gfaconf='N'
                        ";
                updateCea = @"UPDATE cea_tb 
                            SET cea29=NULL
                            WHERE cea29=@gfa01
                            ";
                updateCfa = @"UPDATE cfa_tb 
                            SET cfa12=NULL
                            WHERE cfa12=@gfa01
                            ";
                updateGea = @"UPDATE gea_tb 
                            SET gea06=NULL,
                                gea07=NULL
                            WHERE gea06=@gfa01
                            ";
                deleteGfa = @"DELETE FROM gfa_tb WHERE gfa01=@gfa01 ";
                deleteGfb = @"DELETE FROM gfb_tb WHERE gfb01=@gfb01 ";

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

                selectSql = string.Concat(selectSql, strQueryRange, strQuerySingle, pSecurityString);       //加入權限處理
                dtGfa = OfGetDataTable(selectSql, sqlParmList.ToArray());
                if (dtGfa == null || dtGfa.Rows.Count == 0)
                    return null;

                rtnResultList = new List<Result>();
                foreach (DataRow drGfa in dtGfa.Rows)
                {
                    gfaModel = drGfa.ToItem<gfa_tb>();
                    var result = new Result();
                    rtnResultList.Add(result);

                    //更新應收帳款或是收款沖帳
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@gfa01", gfaModel.gfa01));
                    chkCnts = OfExecuteNonquery(updateCea, sqlParmList.ToArray());
                    if (chkCnts < 0)
                    {
                        result.Key1 = gfaModel.gfa01;
                        result.Message = "更新應收帳款失敗!";
                        continue;
                    }
                    else if (chkCnts==0)    //更新收款單
                    {
                        sqlParmList = new List<SqlParameter>();
                        sqlParmList.Add(new SqlParameter("@gfa01", gfaModel.gfa01));
                        chkCnts = OfExecuteNonquery(updateCfa, sqlParmList.ToArray());
                        if (chkCnts==0)
                        {
                            result.Key1 = gfaModel.gfa01;
                            result.Message = "查無收款單可更新!";
                            continue;
                        }
                        else if (chkCnts<=0)
                        {
                            result.Key1 = gfaModel.gfa01;
                            result.Message = "更新收款單失敗!";
                            continue;
                        }

                    }


                    //更新分錄底稿
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@gfa01", gfaModel.gfa01));
                    if (OfExecuteNonquery(updateGea, sqlParmList.ToArray()) < 0)
                    {
                        result.Key1 = gfaModel.gfa01;
                        result.Message = "更新分錄底稿失敗!";
                        continue;
                    }
                    //刪除傳票單身
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@gfb01", gfaModel.gfa01));
                    if (OfExecuteNonquery(deleteGfb, sqlParmList.ToArray()) < 0)
                    {
                        result.Key1 = gfaModel.gfa01;
                        result.Message = "刪除傳票單身失敗!";
                        continue;
                    }

                    //刪除傳票單頭
                    sqlParmList = new List<SqlParameter>();
                    sqlParmList.Add(new SqlParameter("@gfa01", gfaModel.gfa01));
                    if (OfExecuteNonquery(deleteGfa, sqlParmList.ToArray()) < 0)
                    {
                        result.Key1 = gfaModel.gfa01;
                        result.Message = "刪除傳票單頭失敗!";
                        continue;
                    }

                    result.Key1 = gfaModel.gfa01;
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

    }
}
