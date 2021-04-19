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

    }
}
