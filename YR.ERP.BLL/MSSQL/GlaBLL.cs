/* 程式名稱: GlaBLL.cs
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
    public class GlaBLL : YR.ERP.BLL.MSSQL.CommonBLL
    {
        #region GlaBLL() :　建構子
        public GlaBLL()
            : base()
        {
        }

        public GlaBLL(YR.ERP.DAL.ERP_MSSQLDAL pdao)
            : base(pdao)
        {
        }


        public GlaBLL(string pComp, string pTargetTable, string pTargetColumn, string pViewTable)
            : base(pComp, pTargetTable, pTargetColumn, pViewTable)
        {

        }

        public GlaBLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfChkGacPKValid 檢查單別是否存在
        public bool OfChkGacPKValid(string pGac01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM gac_tb");
                sbSql.AppendLine("WHERE gacvali='Y'");
                sbSql.AppendLine("AND gac01=@gac01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gac01", pGac01));

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

        #region OfGetGac02 取得單別名稱
        public string OfGetGac02(string pGac01)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT gac02 FROM gac_tb");
                sbSql.AppendLine("WHERE gac01=@gac01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gac01", pGac01));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkGba: 檢查會科PK
        public bool OfChkGbaPKExists(string pGba01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM gba_tb");
                sbSql.AppendLine("WHERE gba01=@gba01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gba01", pGba01));

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

        public bool OfChkGbaPKValid(string pGba01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM gba_tb");
                sbSql.AppendLine("WHERE gba01=@gba01");
                sbSql.AppendLine("AND  gbavali='Y'");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gba01", pGba01));

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

        #region OfGetGba05KVPList gba05借貸方
        public List<KeyValuePair<string, string>> OfGetGba05KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.借"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.貸"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetGba02 取得會科名稱
        public string OfGetGba02(string pGba01)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT gba02 FROM gba_tb");
                sbSql.AppendLine("WHERE gba01=@gba01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gba01", pGba01));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetGba 會計科目
        public DataRow OfGetGbaDr(string pgba01)
        {
            DataRow drGba = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM gba_tb");
                sbSql.AppendLine("WHERE gba01=@gba01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gba01", pgba01));
                drGba = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drGba;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public gba_tb OfGetGbaModel(string pGba01)
        {
            DataRow drGba = null;
            gba_tb rtnModel = null;
            try
            {
                drGba = OfGetGbaDr(pGba01);
                if (drGba == null)
                    return null;
                rtnModel = drGba.ToItem<gba_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkGbd: 檢查部門層級PK
        public bool OfChkGbdPKExists(string pGbd01, string pGbd02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM gbd_tb");
                sbSql.AppendLine("WHERE gbd01=@gbd01");
                sbSql.AppendLine("AND gbd02=@gbd02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbd01", pGbd01));
                sqlParmList.Add(new SqlParameter("@gbd02", pGbd02));

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

        public bool OfChkGbdPKValid(string pGbd01, string pGbd02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM gbd_tb");
                sbSql.AppendLine("WHERE gbd01=@gbd01");
                sbSql.AppendLine("AND gbd02=@gbd02");
                sbSql.AppendLine("AND  gbdvali='Y'");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gbd01", pGbd01));
                sqlParmList.Add(new SqlParameter("@gbd02", pGbd02));

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

        #region OfGetGbdDr 部門層級
        public DataRow OfGetGbdDr(string pGbd01,string pGbd02)
        {
            DataRow drGbd = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM gbd_tb");
                sbSql.AppendLine("WHERE gbd01=@gbd01");
                sbSql.AppendLine("AND gbd02=@gbd02");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gb201", pGbd01));
                sqlParmList.Add(new SqlParameter("@gbd02", pGbd02));
                drGbd = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drGbd;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public gbd_tb OfGetGbdModel(string pGbd01, string pGbd02)
        {
            DataRow drGbd = null;
            gbd_tb rtnModel = null;
            try
            {
                drGbd = OfGetGbdDr(pGbd01,pGbd02);
                if (drGbd == null)
                    return null;
                rtnModel = drGbd.ToItem<gbd_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetGea 分錄底稿單頭
        public DataRow OfGetGeaDr(string pGea01,string pGea02,decimal pGea03,decimal pGea04)
        {
            DataRow drGea = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM gea_tb");
                sbSql.AppendLine("WHERE gea01=@gea01 AND gea02=@gea02 AND gea03=@gea03 AND gea04=@gea04");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gea01", pGea01));
                sqlParmList.Add(new SqlParameter("@gea02", pGea02));
                sqlParmList.Add(new SqlParameter("@gea03", pGea03));
                sqlParmList.Add(new SqlParameter("@gea04", pGea04));
                drGea = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drGea;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public gea_tb OfGetGeaModel(string pGea01, string pGea02, decimal pGea03, decimal pGea04)
        {
            DataRow drGea = null;
            gea_tb rtnModel = null;
            try
            {
                drGea = OfGetGeaDr(pGea01,pGea02,pGea03,pGea04);
                if (drGea == null)
                    return null;
                rtnModel = drGea.ToItem<gea_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetGeb 分錄底稿單身
        public DataRow OfGetGebDr(string pGeb01, string pGeb02,decimal pGeb03,decimal pGeb04,decimal pGeb05)
        {
            DataRow drGeb = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM geb_tb");
                sbSql.AppendLine("WHERE geb01=@geb01");
                sbSql.AppendLine("AND geb02=@geb02");
                sbSql.AppendLine("AND geb03=@geb03");
                sbSql.AppendLine("AND geb04=@geb04");
                sbSql.AppendLine("AND geb05=@geb05");
                sqlParmList = new List<SqlParameter>() { 
                                    new SqlParameter("@geb01", pGeb01), 
                                    new SqlParameter("@geb02", pGeb02), 
                                    new SqlParameter("@geb03", pGeb03), 
                                    new SqlParameter("@geb04", pGeb04), 
                                    new SqlParameter("@geb05", pGeb05) };
                drGeb = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drGeb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public geb_tb OfGetGebModel(string pGeb01, string pGeb02,decimal pGeb03,decimal pGeb04,decimal pGeb05)
        {
            DataRow drGeb = null;
            geb_tb rtnModel = null;
            try
            {
                drGeb = OfGetGebDr(pGeb01, pGeb02, pGeb03, pGeb04, pGeb05);
                if (drGeb == null)
                    return null;
                rtnModel = drGeb.ToItem<geb_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable OfGetGebDt(string pGeb01,string pGeb02,decimal pGeb03,decimal pGeb04)
        {
            DataTable dtGeb = null;
            List<SqlParameter> sqlParmList;

            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM geb_tb");
                sbSql.AppendLine("WHERE geb01=@geb01");
                sbSql.AppendLine("AND geb02=@geb02");
                sbSql.AppendLine("AND geb03=@geb03");
                sbSql.AppendLine("AND geb04=@geb04");
                sqlParmList = new List<SqlParameter>() { 
                                    new SqlParameter("@geb01", pGeb01), 
                                    new SqlParameter("@geb02", pGeb02), 
                                    new SqlParameter("@geb03", pGeb03), 
                                    new SqlParameter("@geb04", pGeb04) };
                dtGeb = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                return dtGeb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<geb_tb> OfGetGebList(string pGeb01, string pGeb02, decimal pGeb03, decimal pGeb04)
        {
            DataTable dtGeb = null;
            try
            {
                dtGeb = OfGetGebDt(pGeb01, pGeb02, pGeb03, pGeb04);
                if (dtGeb == null || dtGeb.Rows.Count == 0)
                    return null;

                return dtGeb.ToList<geb_tb>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetGfa 一般傳票單頭
        public DataRow OfGetGfaDr(string pGfa01)
        {
            DataRow drGfa = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM gfa_tb");
                sbSql.AppendLine("WHERE gfa01=@gfa01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gfa01", pGfa01));
                drGfa = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drGfa;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public gfa_tb OfGetGfaModel(string pGfa01)
        {
            DataRow drGfa = null;
            gfa_tb rtnModel = null;
            try
            {
                drGfa = OfGetGfaDr(pGfa01);
                if (drGfa == null)
                    return null;
                rtnModel = drGfa.ToItem<gfa_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetGfb 一般傳票單身
        public DataRow OfGetGfbDr(string pGfb01, int pGfb02)
        {
            DataRow drGfb = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM gfb_tb");
                sbSql.AppendLine("WHERE gfb01=@gfb01");
                sbSql.AppendLine("AND gfb02=@gfb02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@gfb01", pGfb01), new SqlParameter("@gfb02", pGfb02) };
                drGfb = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drGfb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public gfb_tb OfGetGfbModel(string pGfb01, int pGfb02)
        {
            DataRow drGfb = null;
            gfb_tb rtnModel = null;
            try
            {
                drGfb = OfGetGfbDr(pGfb01, pGfb02);
                if (drGfb == null)
                    return null;
                rtnModel = drGfb.ToItem<gfb_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable OfGetGfbDt(string pGfb01)
        {
            DataTable dtGfb = null;
            List<SqlParameter> sqlParmList;
            string sqlSelect = "";
            try
            {
                sqlSelect = @"SELECT * FROM gfb_tb
                        WHERE gfb01=@gfb01
                        ";
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gfb01",pGfb01));
                dtGfb = OfGetDataTable(sqlSelect, sqlParmList.ToArray());
                return dtGfb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<gfb_tb> OfGetGfbList(string pGfb01)
        {
            DataTable dtGfb=null;
            try
            {
                dtGfb = OfGetGfbDt(pGfb01);
                if (dtGfb == null || dtGfb.Rows.Count == 0)
                    return null;

                return dtGfb.ToList<gfb_tb>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetGfb06KVPList gfb06借貸方
        public List<KeyValuePair<string, string>> OfGetGfb06KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.借"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.貸"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetGfg 傳票立帳單頭
        public DataRow OfGetGfgDr(string pGfg01,int pGfg02)
        {
            DataRow drGfg = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM gfg_tb");
                sbSql.AppendLine("WHERE gfg01=@gfg01");
                sbSql.AppendLine("AND gfg02=@gfg02");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gfg01", pGfg01));
                sqlParmList.Add(new SqlParameter("@gfg02", pGfg02));
                drGfg = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drGfg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public gfg_tb OfGetGfgModel(string pGfg01)
        {
            DataRow drGfa = null;
            gfg_tb rtnModel = null;
            try
            {
                drGfa = OfGetGfaDr(pGfg01);
                if (drGfa == null)
                    return null;
                rtnModel = drGfa.ToItem<gfg_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetGfh 傳票沖帳單身
        public DataRow OfGetGfhDr(string pGfh01, int pGfh02, string pGfh03, int pGfh04)
        {
            DataRow drGfg = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM gfh_tb");
                sbSql.AppendLine("WHERE gfh01=@gfh01");
                sbSql.AppendLine("AND gfh02=@gfh02");
                sbSql.AppendLine("AND gfh03=@gfh03");
                sbSql.AppendLine("AND gfh04=@gfh04");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@gfh01", pGfh01));
                sqlParmList.Add(new SqlParameter("@gfh02", pGfh02));
                sqlParmList.Add(new SqlParameter("@gfh03", pGfh03));
                sqlParmList.Add(new SqlParameter("@gfh04", pGfh04));
                drGfg = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drGfg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public gfh_tb OfGetGfhModel(string pGfh01, int pGfh02, string pGfh03, int pGfh04)
        {
            DataRow drGfh = null;
            gfh_tb rtnModel = null;
            try
            {
                drGfh = OfGetGfhDr(pGfh01, pGfh02, pGfh03, pGfh04);
                if (drGfh == null)
                    return null;
                rtnModel = drGfh.ToItem<gfh_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetGlaYearPeriod 取得會計年度跟期間
        /// <summary>
        /// 傳入日期取得期間
        /// </summary>
        /// <param name="pDate"></param>
        /// <param name="pYear"></param>
        /// <param name="pPeriod"></param>
        /// <returns></returns>
        public bool OfGetGlaYearPeriod(DateTime pDate, out int pYear, out int pPeriod)
        {
            try
            {
                pYear = 0;
                pPeriod = 0;
                //目前先以傳入的年跟月做為回傳年度及期數,之後要開表處理
                pYear = pDate.Year;
                pPeriod = pDate.Month;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool OfGetGlaYearPeriod( int pYear, int pPeriod,out DateTime pDateBegin,out DateTime pDateEnd)
        {
            try
            {
                pDateBegin=new DateTime(pYear,pPeriod,1);
                pDateEnd = pDateBegin.AddMonths(1).AddDays(-1);

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
