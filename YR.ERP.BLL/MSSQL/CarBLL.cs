/* 程式名稱: CarBLL.cs
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
using YR.ERP.Shared;

namespace YR.ERP.BLL.MSSQL
{
    public class CarBLL : YR.ERP.BLL.MSSQL.CommonBLL
    {
        
        #region BasBLL() :　建構子
        public CarBLL()
            : base()
        {
        }

        public CarBLL(YR.ERP.DAL.ERP_MSSQLDAL pdao)
            : base(pdao)
        {
        }


        public CarBLL(string pComp, string pTargetTable, string pTargetColumn, string pViewTable)
            : base(pComp, pTargetTable, pTargetColumn, pViewTable)
        {

        }

        public CarBLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfGetCac02 取得應收單別名稱
        public string OfGetCac02(string pCac01)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT cac02 FROM cac_tb");
                sbSql.AppendLine("WHERE cac01=@cac01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@cac01", pCac01));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetCac 單別資料維護
        public DataRow OfGetCacDr(string pCac01, int pCac01Length)
        {
            DataRow drCac = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM cac_tb");
                sbSql.AppendLine("WHERE cac01=SUBSTRING(@cac01,1,@length)");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@cac01", pCac01), new SqlParameter("@length", pCac01Length) };
                drCac = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drCac;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataRow OfGetCacDr(string pCac01)
        {
            DataRow drCac = null;
            BasBLL boBll = null;
            int baa06 = 0;  //單別碼數
            try
            {
                boBll = new BasBLL(this.OfGetConntion());
                if (this.TRAN != null)
                    boBll.TRAN = this.TRAN;

                baa06 = boBll.OfGetBaa06();
                drCac = OfGetCacDr(pCac01, baa06);
                return drCac;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 傳入單別,自動會以baa06的單別碼數截取資料
        /// </summary>
        /// <param name="pCac01"></param>
        /// <returns></returns>
        public cac_tb OfGetCacModel(string pCac01)
        {
            DataRow drCac = null;
            cac_tb rtnModel = null;
            try
            {
                drCac = OfGetCacDr(pCac01);
                if (drCac == null)
                    return null;
                rtnModel = drCac.ToItem<cac_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public cac_tb OfGetCacModel(string pCac01, int pCac01Length)
        {
            DataRow drBab = null;
            cac_tb rtnModel = null;
            try
            {
                drBab = OfGetCacDr(pCac01, pCac01Length);
                if (drBab == null)
                    return null;
                rtnModel = drBab.ToItem<cac_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkCacPK 檢查單別是否存在 
        public bool OfChkCacPKValid(string pCac01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM cac_tb");
                sbSql.AppendLine("WHERE cacvali='Y'");
                sbSql.AppendLine("AND cac01=@cac01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@cac01", pCac01));

                iChks = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (iChks == 0)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool OfChkCacPKValid(string pCac01, string pCac03, string pCac04)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM cac_tb");
                sbSql.AppendLine("WHERE cacvali='Y'");
                sbSql.AppendLine("AND cac01=@cac01");
                sbSql.AppendLine("AND cac03=@cac03");
                sbSql.AppendLine("AND cac04=@cac04");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@cac01", pCac01));
                sqlParmList.Add(new SqlParameter("@cac03", pCac03));
                sqlParmList.Add(new SqlParameter("@cac04", pCac04));

                iChks = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (iChks == 0)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetCba02 取得應收科目類別名稱
        public string OfGetCba02(string pCba02)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT cba02 FROM cba_tb");
                sbSql.AppendLine("WHERE cba01=@cba01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@cba01", pCba02));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetCba 應收科目類別
        public DataRow OfGetCbaDr(string pCba01)
        {
            DataRow drCba = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM cba_tb");
                sbSql.AppendLine("WHERE cba01=@cba01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@cba01", pCba01) };
                drCba = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drCba;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public cba_tb OfGetCbaModel(string pCba01)
        {
            DataRow drCba = null;
            cba_tb rtnModel = null;
            try
            {
                drCba = OfGetCbaDr(pCba01);
                if (drCba == null)
                    return null;
                rtnModel = drCba.ToItem<cba_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkCbaPK 檢查應收帳款類別PK
        public bool OfChkCbaPKExists(string pCba01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM cba_tb");
                sbSql.AppendLine("WHERE cba01=@cba01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@cba01", pCba01));

                iChks = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                if (iChks == 0)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetCeaconfKVPList 應收帳款確認來源
        public List<KeyValuePair<string, string>> OfGetCeaconfKVPList()
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

        #region OfGetCeastatKVPList 應收帳款狀態來源
        public List<KeyValuePair<string, string>> OfGetCeastatKVPList()
        {
            try
            {
                return OfGetDocStatusKVPList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetCea 應收帳款單頭
        public DataRow OfGetCeaDr(string pCea01)
        {
            DataRow drCea = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM cea_tb");
                sbSql.AppendLine("WHERE cea01=@cea01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@cea01", pCea01) };
                drCea = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drCea;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public cea_tb OfGetCeaModel(string pCea01)
        {
            DataRow drCea = null;
            cea_tb rtnModel = null;
            try
            {
                drCea = OfGetCeaDr(pCea01);
                if (drCea == null)
                    return null;
                rtnModel = drCea.ToItem<cea_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetCeb 應收帳款單身
        public DataTable OfGetCebDt(string pCeb01)
        {
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            DataTable dtCeb = null;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ceb_tb");
                sbSql.AppendLine("WHERE ceb01=@ceb01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@ceb01", pCeb01) };
                dtCeb = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                return dtCeb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataRow OfGetCebDr(string pCeb01, int pCeb02)
        {
            DataRow drCeb = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ceb_tb");
                sbSql.AppendLine("WHERE ceb01=@ceb01");
                sbSql.AppendLine("AND ceb02=@ceb02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@ceb01", pCeb01), new SqlParameter("@ceb02", pCeb02) };
                drCeb = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drCeb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ceb_tb OfGetCebModel(string pCeb01, int pCeb02)
        {
            DataRow drCeb = null;
            ceb_tb rtnModel = null;
            try
            {
                drCeb = OfGetCebDr(pCeb01, pCeb02);
                if (drCeb == null)
                    return null;
                rtnModel = drCeb.ToItem<ceb_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ceb_tb> OfGetCebList(string pCeb01)
        {
            DataTable dtCeb = null;
            List<ceb_tb> rtnList = null;
            try
            {
                dtCeb = OfGetCebDt(pCeb01);
                if (dtCeb == null)
                    return null;
                rtnList = dtCeb.ToList<ceb_tb>();

                return rtnList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetCfaconfKVPList 收款單確認來源
        public List<KeyValuePair<string, string>> OfGetCfaconfKVPList()
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

        #region OfGetCfastatKVPList 收款單狀態來源
        public List<KeyValuePair<string, string>> OfGetCfastatKVPList()
        {
            try
            {
                return OfGetDocStatusKVPList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetCfa 收款單單頭
        public DataRow OfGetCfaDr(string pCfa01)
        {
            DataRow drCfa = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM cfa_tb");
                sbSql.AppendLine("WHERE cfa01=@cfa01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@cfa01", pCfa01) };
                drCfa = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drCfa;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public cfa_tb OfGetCfaModel(string pCfa01)
        {
            DataRow drCfa = null;
            cfa_tb rtnModel = null;
            try
            {
                drCfa = OfGetCfaDr(pCfa01);
                if (drCfa == null)
                    return null;
                rtnModel = drCfa.ToItem<cfa_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetCfb 收款單單身
        public DataTable OfGetCfbDt(string pCfb01)
        {
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            DataTable dtCfb = null;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM cfb_tb");
                sbSql.AppendLine("WHERE cfb01=@cfb01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@cfb01", pCfb01) };
                dtCfb = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                return dtCfb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataRow OfGetCfbDr(string pCfb01, int pCfb02)
        {
            DataRow drCfb = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM cfb_tb");
                sbSql.AppendLine("WHERE cfb01=@cfb01");
                sbSql.AppendLine("AND cfb02=@cfb02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@cfb01", pCfb01), new SqlParameter("@cfb02", pCfb02) };
                drCfb = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drCfb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public cfb_tb OfGetCfbModel(string pCfb01, int pCfb02)
        {
            DataRow drCfb = null;
            cfb_tb rtnModel = null;
            try
            {
                drCfb = OfGetCfbDr(pCfb01, pCfb02);
                if (drCfb == null)
                    return null;
                rtnModel = drCfb.ToItem<cfb_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<cfb_tb> OfGetCfbList(string pCfb01)
        {
            DataTable dtCfb = null;
            List<cfb_tb> rtnList = null;
            try
            {
                dtCfb = OfGetCfbDt(pCfb01);
                if (dtCfb == null)
                    return null;
                rtnList = dtCfb.ToList<cfb_tb>();

                return rtnList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
    
}
