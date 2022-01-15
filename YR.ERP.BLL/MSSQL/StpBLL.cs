/* 程式名稱: StpBLL.cs
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

namespace YR.ERP.BLL.MSSQL
{
    public class StpBLL : YR.ERP.BLL.MSSQL.CommonBLL
    {
        #region StpBLL() :　建構子
        public StpBLL()
            : base()
        {
        }

        public StpBLL(YR.ERP.DAL.ERP_MSSQLDAL pDao)
            : base(pDao)
        {
        }


        public StpBLL(string pComp, string pTargetTable, string pTargetColumn, string pViewTable)
            : base(pComp, pTargetTable, pTargetColumn, pViewTable)
        {

        }

        public StpBLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfChkSbaPK 檢查客戶類別編號
        public bool OfChkSbaPKExists(string pSba01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sba_tb");
                sbSql.AppendLine("WHERE sba01=@sba01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@sba01", pSba01));

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

        public bool OfChkSbaPKValid(string pSba01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sba_tb");
                sbSql.AppendLine("WHERE sba01=@sba01");
                sbSql.AppendLine("AND sbavali='Y'");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@sba01", pSba01));

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

        #region OfChkSbbPK 檢查銷售取價原則sbb01是否存在
        public bool OfChkSbbPKExists(string pSbb01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sbb_tb");
                sbSql.AppendLine("WHERE sbb01=@sbb01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@sbb01", pSbb01));

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

        #region OfGetSbc 銷售取價明細
        public DataRow OfGetSbcDr(string pSbc01, decimal pSbc02)
        {
            DataRow drSbc = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM sbc_tb");
                sbSql.AppendLine("WHERE sbc01=@sbc01");
                sbSql.AppendLine("AND sbc02=@sbc02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@sbc01", pSbc01) };
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@sbc02", pSbc02) };
                drSbc = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drSbc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public sbc_tb OfGetSbcModel(string pSbc01, decimal pSbc02)
        {
            DataRow drSbc = null;
            sbc_tb rtnModel = null;
            try
            {
                drSbc = OfGetSbcDr(pSbc01, pSbc02);
                if (drSbc == null)
                    return null;
                rtnModel = drSbc.ToItem<sbc_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable OfgetSbcDt(string pSbc01)
        {
            DataTable dtSbc = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM sbc_tb");
                sbSql.AppendLine("WHERE sbc01=@sbc01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@sbc01", pSbc01) };
                dtSbc = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                return dtSbc;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<sbc_tb> OfgetSbcList(string pSbc01)
        {
            DataTable dtSbc = null;
            List<sbc_tb> rtnList = null;
            try
            {
                dtSbc = OfgetSbcDt(pSbc01);
                if (dtSbc == null)
                    return null;
                rtnList = dtSbc.ToList<sbc_tb>();

                return rtnList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetSbb02 取得銷售取價名稱
        public string OfGetSbb02(string pSbb01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT sbb02 FROM sbb_tb");
                sbSql.AppendLine("WHERE sbb01=@sbb01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("sbb01", pSbb01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetSbc03KVPList 銷售取價原則明細
        public List<KeyValuePair<string, string>> OfGetSbc03KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("A1", "A1.料號檔售價"));
                sourceList.Add(new KeyValuePair<string, string>("A2", "A2.料件/客戶價格維護作業"));
                //sourceList.Add(new KeyValuePair<string, string>("A3", "A3.價格表維護作業"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkSbgPK 檢查運輸地點PK
        public bool OfChkSbgPKExists(string pSbg01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sbg_tb");
                sbSql.AppendLine("WHERE sbg01=@sbg01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@sbg01", pSbg01));

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

        #region OfGetSbg02 取得運輸地點名稱
        public string OfGetSbg02(string pSbg01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT sbg02 FROM sbg_tb");
                sbSql.AppendLine("WHERE sbg01=@sbg01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("sbg01", pSbg01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetSca 客戶基本資料
        public DataRow OfGetScaDr(string pSca01)
        {
            DataRow drSca = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM sca_tb");
                sbSql.AppendLine("WHERE sca01=@sca01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@sca01", pSca01) };
                drSca = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drSca;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public sca_tb OfGetScaModel(string pSca01)
        {
            DataRow drSca = null;
            sca_tb rtnModel = null;
            try
            {
                drSca = OfGetScaDr(pSca01);
                if (drSca == null)
                    return null;
                rtnModel = drSca.ToItem<sca_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetSca02 取得客戶全名
        public string OfGetSca02(string pSca01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT sca02 FROM sca_tb");
                sbSql.AppendLine("WHERE sca01=@sca01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("sca01", pSca01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetSca03 取得客戶簡稱
        public string OfGetSca03(string pSca01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT sca03 FROM sca_tb");
                sbSql.AppendLine("WHERE sca01=@sca01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("sca01", pSca01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkSga03Sga17 檢查客編+客戶單號是否存在
        /// <summary>
        /// 檢查客編+客戶單號是否存在
        /// </summary>
        /// <param name="pSga03">客編</param>
        /// <param name="pSga17">訂單編號</param>
        /// <param name="pConfirm">0.全部 N.未確認 Y.已確認 X.作廢</param>
        /// <returns></returns>
        public bool OfChkSga03Sga17(string pSga03,string pSga17,string pConfirm)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sga_tb");
                sbSql.AppendLine("WHERE sga03=@sga03");
                sbSql.AppendLine("AND sga17=@sga17");
                if (GlobalFn.isNullRet(pConfirm,"")!="0")
                    sbSql.AppendLine("AND sgaconf=@sgaconf");


                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@sga03", pSga03));
                sqlParmList.Add(new SqlParameter("@sga17", pSga17));
                if (GlobalFn.isNullRet(pConfirm, "") != "0")
                    sqlParmList.Add(new SqlParameter("@sgaconf", pConfirm));

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

        #region OfChkScaPK 檢查客戶基本資料Pk是否存在
        public bool OfChkScaPKValid(string pSca01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sca_tb");
                sbSql.AppendLine("WHERE scavali='Y' and scaconf='Y'");
                sbSql.AppendLine("AND sca01=@sca01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@sca01", pSca01));

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

        public bool OfChkScaPKExists(string pSca01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM sca_tb");
                sbSql.AppendLine("WHERE sca01=@sca01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@sca01", pSca01));

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

        #region OfGetSca20KVPList 單據發送方式
        public List<KeyValuePair<string, string>> OfGetSca20KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.郵寄"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.FAX"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.E-MAIL"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetSeaconfKVPList 報價單確認來源
        public List<KeyValuePair<string, string>> OfGetSeaconfKVPList()
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

        #region OfGetSea 報價單頭
        public DataRow OfGetSeaDr(string pSea01)
        {
            DataRow drSea = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM sea_tb");
                sbSql.AppendLine("WHERE sea01=@sea01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@sea01", pSea01) };
                drSea = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drSea;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public sea_tb OfGetSeaModel(string pSea01)
        {
            DataRow drSea = null;
            sea_tb rtnModel = null;
            try
            {
                drSea = OfGetSeaDr(pSea01);
                if (drSea == null)
                    return null;
                rtnModel = drSea.ToItem<sea_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetSeb 報價單單身
        public DataTable OfGetSebDt(string pSeb01)
        {
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            DataTable dtSeb = null;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM seb_tb");
                sbSql.AppendLine("WHERE seb01=@seb01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@seb01", pSeb01) };
                dtSeb = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                return dtSeb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataRow OfGetSebDr(string pSeb01, int pSeb02)
        {
            DataRow drSeb = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM seb_tb");
                sbSql.AppendLine("WHERE seb01=@seb01");
                sbSql.AppendLine("AND seb02=@seb02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@seb01", pSeb01), new SqlParameter("@seb02", pSeb02) };
                drSeb = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drSeb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public seb_tb OfGetSebModel(string pSeb01, int pSeb02)
        {
            DataRow drPeb = null;
            seb_tb rtnModel = null;
            try
            {
                drPeb = OfGetSebDr(pSeb01, pSeb02);
                if (drPeb == null)
                    return null;
                rtnModel = drPeb.ToItem<seb_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetSfaconfKVPList 訂單確認來源
        public List<KeyValuePair<string, string>> OfGetSfaconfKVPList()
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

        #region OfGetSfaDr 訂單單單頭
        public DataRow OfGetSfaDr(string pSfa01)
        {
            DataRow drSfa = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM sfa_tb");
                sbSql.AppendLine("WHERE sfa01=@sfa01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@sfa01", pSfa01) };
                drSfa = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drSfa;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public sfa_tb OfGetSfaModel(string pSfa01)
        {
            DataRow drSfa = null;
            sfa_tb rtnModel = null;
            try
            {
                drSfa = OfGetSfaDr(pSfa01);
                if (drSfa == null)
                    return null;
                rtnModel = drSfa.ToItem<sfa_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetSfb 訂單單身
        public DataTable OfGetSfbDt(string pSfb01)
        {
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            DataTable dtSfb = null;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM sfb_tb");
                sbSql.AppendLine("WHERE sfb01=@sfb01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@sfb01", pSfb01) };
                dtSfb = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                return dtSfb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataRow OfGetSfbDr(string pSfb01, int pSfb02)
        {
            DataRow drSfb = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM sfb_tb");
                sbSql.AppendLine("WHERE sfb01=@sfb01");
                sbSql.AppendLine("AND sfb02=@sfb02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@sfb01", pSfb01), new SqlParameter("@sfb02", pSfb02) };
                drSfb = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drSfb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public sfb_tb OfGetSfbModel(string pSfb01, int pSfb02)
        {
            DataRow drSfb = null;
            sfb_tb rtnModel = null;
            try
            {
                drSfb = OfGetSfbDr(pSfb01, pSfb02);
                if (drSfb == null)
                    return null;
                rtnModel = drSfb.ToItem<sfb_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetSgaconfKVPList 銷貨單確認來源
        public List<KeyValuePair<string, string>> OfGetSgaconfKVPList()
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

        #region OfGetSfastatKVPList 客戶訂單狀態來源
        public List<KeyValuePair<string, string>> OfGetSfastatKVPList()
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

        #region OfGetSgaDr 銷貨單單頭
        public DataRow OfGetSgaDr(string pSga01)
        {
            DataRow drSga = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM sga_tb");
                sbSql.AppendLine("WHERE sga01=@sga01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@sga01", pSga01) };
                drSga = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drSga;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public sga_tb OfGetSgaModel(string pSga01)
        {
            DataRow drSga = null;
            sga_tb rtnModel = null;
            try
            {
                drSga = OfGetSgaDr(pSga01);
                if (drSga == null)
                    return null;
                rtnModel = drSga.ToItem<sga_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetSgbDr 銷貨單單身
        public DataRow OfGetSgbDr(string pSgb01, int pSgb02)
        {
            DataRow drSgb = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM sgb_tb");
                sbSql.AppendLine("WHERE sgb01=@sgb01");
                sbSql.AppendLine("AND sgb02=@sgb02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@sgb01", pSgb01), new SqlParameter("@sgb02", pSgb02) };
                drSgb = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drSgb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public sgb_tb OfGetSgbModel(string pSgb01, int pSgb02)
        {
            DataRow drSgb = null;
            sgb_tb rtnModel = null;
            try
            {
                drSgb = OfGetSgbDr(pSgb01, pSgb02);
                if (drSgb == null)
                    return null;
                rtnModel = drSgb.ToItem<sgb_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetShaconfKVPList 銷退單確認來源
        public List<KeyValuePair<string, string>> OfGetShaconfKVPList()
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

        #region OfGetShb17KVPList 銷退單退回類型
        public List<KeyValuePair<string, string>> OfGetShb17KVPList()
        {
            List<KeyValuePair<string, string>> sourcerList;
            try
            {
                sourcerList = new List<KeyValuePair<string, string>>();
                sourcerList.Add(new KeyValuePair<string, string>("1", "1.銷退"));
                sourcerList.Add(new KeyValuePair<string, string>("2", "2.折讓"));
                return sourcerList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        /**********  其他常用function ********/
        #region OfGetPrice
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pSbc01">價格條件</param>
        /// <param name="pCust">客戶編號</param>
        /// <param name="pItem">料號</param>
        /// <param name="pUnit">單位</param>
        /// <param name="pDate">日期</param>
        /// <param name="pCurrency">幣別</param>
        /// <param name="pType">1.報價 2.訂單 3.出貨單</param>
        /// <param name="pQty">數量</param>
        /// <param name="pTaxYN">含稅否</param>
        /// <param name="pTaxRate">稅率</param>
        /// <param name="pExRate">匯率</param>
        /// <param name="pPrice">回傳價格</param>
        /// <returns></returns>
        public Result OfGetPrice(string pSbc01, string pCust, string pItem, string pUnit, DateTime? pDate, string pCurrency,
                            string pType, decimal pQty, string pTaxYN, decimal pTaxRate, decimal pExRate,
                            out decimal pPrice
            )
        {
            Result rtnResult = null;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            List<sbc_tb> sbcList = null;
            BasBLL boBas = null;
            bek_tb bekModel = null;
            decimal tempPrice = 0;
            bool flag = false;
            pPrice = 0;
            try
            {
                rtnResult = new Result();
                rtnResult.Key1 = pItem;
                boBas = new BasBLL(OfGetConntion());
                boBas.TRAN = this.TRAN;

                sbcList = OfgetSbcList(pSbc01);
                if (sbcList == null || sbcList.Count == 0)
                {
                    rtnResult.Message = "未設定取價原則!";
                    return rtnResult;
                }
                bekModel = boBas.OfGetBekModel(pCurrency);
                if (bekModel == null)
                {
                    rtnResult.Message = "查無此幣別資料!";
                    return rtnResult;
                }

                foreach (sbc_tb sbcModel in sbcList.OrderBy(p => p.sbc04))
                {
                    switch (sbcModel.sbc03.ToUpper())
                    {
                        case "A1":  //依料號主檔 考量含稅否及匯率
                            sqlParmList = new List<SqlParameter>();
                            sqlParmList.Add(new SqlParameter("@ica01", pItem));
                            if (pTaxYN == "Y")
                            {
                                sqlSelect = @"SELECT ica11 FROM ica_tb WHERE ica01=@ica01";
                            }
                            else
                            {
                                sqlSelect = @"SELECT ica10 FROM ica_tb WHERE ica01=@ica01";
                            }
                            tempPrice = GlobalFn.isNullRet(OfGetFieldValue(sqlSelect, sqlParmList.ToArray()), 0m);
                            break;
                        case "A2":  //依料號客戶價格
                            sqlParmList = new List<SqlParameter>();
                            sqlParmList.Add(new SqlParameter("@sdd01", pItem));
                            sqlParmList.Add(new SqlParameter("@sdd02", pCust));
                            sqlParmList.Add(new SqlParameter("@sdd03", pCurrency));
                            sqlSelect = @"SELECT sdd09 FROM sdd_tb 
                                            WHERE sdd01=@sdd01
                                            AND sdd02=@sdd02 AND sdd03=@sdd03
                                        ";
                            tempPrice = GlobalFn.isNullRet(OfGetFieldValue(sqlSelect, sqlParmList.ToArray()), 0m);
                            break;
                        case "A3":  //依產品主檔
                            break;
                    }

                    if (sbcModel.sbc03.ToUpper() == "A1" && tempPrice > 0)
                    {
                        pPrice = tempPrice / pExRate;
                        pPrice = GlobalFn.Round(pPrice, bekModel.bek03);
                        break;
                    }

                    if (sbcModel.sbc03.ToUpper() == "A2" && tempPrice > 0)
                    {
                        pPrice = tempPrice;
                        pPrice = GlobalFn.Round(pPrice, bekModel.bek03);
                        break;
                    }

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

        #region OfChkEbusinessPlateForm 檢查是否為電商平台
        public bool OfChkEbusinessPlateForm(string custNo)
        {
            try
            {
                if (custNo == "C000002"  //蝦皮1
                 || custNo == "C000003"  //露天
                 || custNo == "C000004"  //奇摩
                 || custNo == "C000005"  //松果
                 || custNo == "C000006"  //蝦皮2
                 || custNo == "C000007"  //蝦皮2
                 )
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion

        #region OfGetSga23 取得出貨成本
        public Result OfGetSga23(sga_tb sgaModel, out decimal sga23)
        {
            sga23 = 0;
            Result rtnResult = null;
            rtnResult = new Result();
            rtnResult.Success = false;

            try
            {
                if (sgaModel == null)
                    return rtnResult;
                rtnResult.Key1 = sgaModel.sga01;

                if (OfChkEbusinessPlateForm(sgaModel.sga03)==false)
                {
                    rtnResult.Message = "非出貨成本計算客戶!";
                    return rtnResult;
                }

                switch (sgaModel.sga03)
                {
                    case "C000002"://蝦皮
                        sga23 = sgaModel.sga13t * 0.04m;
                        if (sgaModel.sga13t > 4490)
                            sga23 += 60;
                        break;
                    case "C000003"://露天-正中
                        sga23 = sgaModel.sga13t * 0.03m;
                        break;
                    case "C000004"://奇摩
                        sga23 = sgaModel.sga13t * 0.015m;
                        if (sgaModel.sga13t > 4990)
                            sga23 += 60;
                        break;
                    case "C000005"://松果
                        sga23 = sgaModel.sga13t * 0.04m; //預設刷卡
                        //sga23 += 60;
                        break;
                    case "C000006"://蝦皮2
                        sga23 = sgaModel.sga13t * 0.04m;
                        break;
                    case "C000007"://露天-艾達
                        sga23 = sgaModel.sga13t * 0.03m;
                        break;
                }
                sga23 = GlobalFn.Round(sga23, 0);

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
