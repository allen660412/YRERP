/* 程式名稱: ManBLL.cs
   系統代號: 
   作    者: Allen
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
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


namespace YR.ERP.BLL.MSSQL
{
    public class ManBLL : YR.ERP.BLL.MSSQL.CommonBLL
    {
        #region ManBLL() :　建構子
        public ManBLL()
            : base()
        {
        }

        public ManBLL(YR.ERP.DAL.ERP_MSSQLDAL pdao)
            : base(pdao)
        {
        }


        public ManBLL(string pComp, string pTargetTable, string pTargetColumn, string pViewTable)
            : base(pComp, pTargetTable, pTargetColumn, pViewTable)
        {

        }

        public ManBLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfGetMea 製令單單頭 含廠內及托工
        public DataRow OfGetMeaDr(string pMea01)
        {
            DataRow drMea = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM mea_tb");
                sbSql.AppendLine("WHERE mea01=@mea01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@mea01", pMea01) };
                drMea = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drMea;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mea_tb OfGetMeaModel(string pMea01)
        {
            DataRow drMea = null;
            mea_tb rtnModel = null;
            try
            {
                drMea = OfGetMeaDr(pMea01);
                if (drMea == null)
                    return null;
                rtnModel = drMea.ToItem<mea_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetMeb 製令單單身 含廠內及托工
        public DataTable OfGetMebDt(string pMeb01)
        {
            DataTable dtMeb = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM meb_tb");
                sbSql.AppendLine("WHERE meb01=@meb01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@meb01", pMeb01) };
                dtMeb = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                return dtMeb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<meb_tb> OfGetMebList(string pMeb01)
        {
            DataTable dtMeb = null;
            List<meb_tb> rtnList = null;
            try
            {
                dtMeb = OfGetMebDt(pMeb01);
                if (dtMeb == null)
                    return null;
                rtnList = dtMeb.ToList<meb_tb>();

                return rtnList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataRow OfGetMebDr(string pMeb01, int pMeb02)
        {
            DataRow drMeb = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM meb_tb");
                sbSql.AppendLine("WHERE meb01=@meb01");
                sbSql.AppendLine("AND meb02=@meb02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@meb01", pMeb01),
                                                         new SqlParameter("@meb02", pMeb02)
                                                        };
                drMeb = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drMeb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public meb_tb OfGetMebModel(string pMeb01, int pMeb02)
        {
            DataRow drMea = null;
            meb_tb rtnModel = null;
            try
            {
                drMea = OfGetMebDr(pMeb01, pMeb02);
                if (drMea == null)
                    return null;
                rtnModel = drMea.ToItem<meb_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetMeaconfKVPList 託工單確認來源
        public List<KeyValuePair<string, string>> OfGetMeaconfKVPList()
        {
            try
            {
                return OfGetDocConfirmKVPList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetMeastatKVPList 託工單狀態來源
        public List<KeyValuePair<string, string>> OfGetMeastatKVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.製單"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.單據確認"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.已發料"));
                sourceList.Add(new KeyValuePair<string, string>("4", "4.生產中"));
                sourceList.Add(new KeyValuePair<string, string>("8", "8.指定結案"));
                sourceList.Add(new KeyValuePair<string, string>("9", "9.完工"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetMfaconfKVPList 託工送料單確認來源
        public List<KeyValuePair<string, string>> OfGetMfaconfKVPList()
        {
            try
            {
                return OfGetDocConfirmKVPList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetMgaconfKVPList 託工退料單確認來源
        public List<KeyValuePair<string, string>> OfGetMgaconfKVPList()
        {
            try
            {
                return OfGetDocConfirmKVPList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetMhaconfKVPList 託工入庫單確認來源
        public List<KeyValuePair<string, string>> OfGetMhaconfKVPList()
        {
            try
            {
                return OfGetDocConfirmKVPList();                
            }
            catch (Exception ex)
            {
                throw ex;                
            }
        }
        #endregion

        #region OfGetMiaconfKVPList 託工退庫單確認來源
        public List<KeyValuePair<string, string>> OfGetMiaconfKVPList()
        {
            try
            {
                return OfGetDocConfirmKVPList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
