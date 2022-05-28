/* 程式名稱: PurBLL.cs
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
using YR.ERP.Shared;

namespace YR.ERP.BLL.MSSQL
{
    public class PurBLL : YR.ERP.BLL.MSSQL.CommonBLL
    {
        #region PurBLL() :　建構子
        public PurBLL()
            : base()
        {
        }

        public PurBLL(YR.ERP.DAL.ERP_MSSQLDAL pdao)
            : base(pdao)
        {
        }


        public PurBLL(string pComp, string pTargetTable, string pTargetColumn, string pViewTable)
            : base(pComp, pTargetTable, pTargetColumn, pViewTable)
        {

        }

        public PurBLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfChkPbaPK 檢查廠商類別編號
        public bool OfChkPbaPKExists(string pPba01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM pba_tb");
                sbSql.AppendLine("WHERE pba01=@pba01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@pba01", pPba01));

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

        #region OfChkPbbPK 檢查採購取價原則pbb01是否存在
        public bool OfChkPbbPKExists(string pPbb01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM pbb_tb");
                sbSql.AppendLine("WHERE pbb01=@pbb01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@pbb01", pPbb01));

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

        #region OfGetPbb02 取得採購取價名稱
        public string OfGetPbb02(string pPbb01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT pbb02 FROM pbb_tb");
                sbSql.AppendLine("WHERE pbb01=@pbb01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("pbb01", pPbb01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return retValue;
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

        #region OfGetPbc 採購取價明細
        public DataRow OfGetPbcDr(string pPbc01, decimal pPbc02)
        {
            DataRow drPbc = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM pbc_tb");
                sbSql.AppendLine("WHERE pbc01=@pbc01");
                sbSql.AppendLine("AND pbc02=@pbc02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@pbc01", pPbc01) };
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@pbc02", pPbc02) };
                drPbc = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drPbc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public pbc_tb OfGetPbcModel(string pPbc01, decimal pPbc02)
        {
            DataRow drPbc = null;
            pbc_tb rtnModel = null;
            try
            {
                drPbc = OfGetPbcDr(pPbc01, pPbc02);
                if (drPbc == null)
                    return null;
                rtnModel = drPbc.ToItem<pbc_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable OfgetPbcDt(string pPbc01)
        {
            DataTable dtPbc = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM pbc_tb");
                sbSql.AppendLine("WHERE pbc01=@pbc01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@pbc01", pPbc01) };
                dtPbc = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                return dtPbc;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<pbc_tb> OfgetPbcList(string pPbc01)
        {
            DataTable dtPbc = null;
            List<pbc_tb> rtnList = null;
            try
            {
                dtPbc = OfgetPbcDt(pPbc01);
                if (dtPbc == null)
                    return null;
                rtnList = dtPbc.ToList<pbc_tb>();

                return rtnList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetPbc03KVPList 採購取價原則明細
        public List<KeyValuePair<string, string>> OfGetPbc03KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("A1", "A1.料件最近市價"));
                sourceList.Add(new KeyValuePair<string, string>("A2", "A2.料件最近採購價"));
                sourceList.Add(new KeyValuePair<string, string>("A3", "A3.料件供應商單價"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetPca 廠商基本資料
        public DataRow OfGetPcaDr(string pPca01)
        {
            DataRow drPca = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM pca_tb");
                sbSql.AppendLine("WHERE pca01=@pca01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@pca01", pPca01) };
                drPca = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drPca;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public pca_tb OfGetPcaModel(string pPca01)
        {
            DataRow drPca = null;
            pca_tb rtnModel = null;
            try
            {
                drPca = OfGetPcaDr(pPca01);
                if (drPca == null)
                    return null;
                rtnModel = drPca.ToItem<pca_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkPcaPK 檢查廠商基本資料PK
        public bool OfChkPcaPKValid(string pPca01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM pca_tb");
                sbSql.AppendLine("WHERE pcavali='Y' AND pcaconf='Y'");
                sbSql.AppendLine("AND pca01=@pca01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@pca01", pPca01));

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

        public bool OfChkPcaPKExists(string pPca01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM pca_tb");
                sbSql.AppendLine("WHERE ");
                sbSql.AppendLine("pca01=@pca01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@pca01", pPca01));

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

        #region OfGetPca02 取得廠商全名
        public string OfGetPca02(string pPca01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT pca02 FROM pca_tb");
                sbSql.AppendLine("WHERE pca01=@pca01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("pca01", pPca01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetPca03 取得廠商簡稱
        public string OfGetPca03(string pPca01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT pca03 FROM pca_tb");
                sbSql.AppendLine("WHERE pca01=@pca01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("pca01", pPca01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetPca20KVPList 票據寄領方式
        public List<KeyValuePair<string, string>> OfGetPca20KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.郵寄"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.自取"));
                sourceList.Add(new KeyValuePair<string, string>("9", "9.其他"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetPca28KVPList 採購單發送方式
        public List<KeyValuePair<string, string>> OfGetPca28KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.郵寄"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.傳真"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.E-mail"));
                sourceList.Add(new KeyValuePair<string, string>("9", "9.其他"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkPcbPKExists
        public bool OfChkPcbPKExists(string pPcb01, string pPcb02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM pcb_tb");
                sbSql.AppendLine("WHERE ");
                sbSql.AppendLine("pcb01=@pcb01");
                sbSql.AppendLine("AND pcb02=@pcb02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@pcb01", pPcb01));
                sqlParmList.Add(new SqlParameter("@pcb02", pPcb02));

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

        #region OfGetPdd 料號/廠商價格檔
        public DataRow OfGetPddDr(string pPdd01, string pPdd02, string pPdd03)
        {
            DataRow drPdd = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM pdd_tb");
                sbSql.AppendLine("WHERE pdd01=@pdd01 AND pdd02=@pdd02 AND pdd03=@pdd03");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@pdd01", pPdd01),
                                    new SqlParameter("@pdd02", pPdd02), 
                                    new SqlParameter("@pdd03", pPdd03) };
                drPdd = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drPdd;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public pdd_tb OfGetPddModel(string pPdd01, string pPdd02, string pPdd03)
        {
            DataRow drPdd = null;
            pdd_tb rtnModel = null;
            try
            {
                drPdd = OfGetPddDr(pPdd01, pPdd02, pPdd03);
                if (drPdd == null)
                    return null;
                rtnModel = drPdd.ToItem<pdd_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetPea 送貨/帳單地址
        public DataRow OfGetPccDr(string pPcc01)
        {
            DataRow drPcc = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM pcc_tb");
                sbSql.AppendLine("WHERE pcc01=@pcc01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@pcc01", pPcc01) };
                drPcc = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drPcc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public pcc_tb OfGetPccModel(string pPcc01)
        {
            DataRow drPcc = null;
            pcc_tb rtnModel = null;
            try
            {
                drPcc = OfGetPccDr(pPcc01);
                if (drPcc == null)
                    return null;
                rtnModel = drPcc.ToItem<pcc_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkPccPk 檢查送貨/帳單地址PK
        public bool OfChkPccPkExists(string pPcc01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM pcc_tb");
                sbSql.AppendLine("WHERE pcc01=@pcc01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@pcc01", pPcc01));

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

        public bool OfChkPccPKValid(string pPcc01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM pcc_tb");
                sbSql.AppendLine("WHERE pcc01=@pcc01");
                sbSql.AppendLine("AND pccvali='Y'");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@pcc01", pPcc01));

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

        public bool OfChkPccPKValid(string pPcc01, string pPcc02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM pcc_tb");
                sbSql.AppendLine("WHERE pcc01=@pcc01");
                sbSql.AppendLine("AND pcc02 in ('0',@pcc02)");
                sbSql.AppendLine("AND pccvali='Y'");


                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@pcc01", pPcc01));
                sqlParmList.Add(new SqlParameter("@pcc02", pPcc02));

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

        #region OfGetPcc02KVPList 性質:送貨/帳單地址
        public List<KeyValuePair<string, string>> OfGetPcc02KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("0", "0.全部"));
                sourceList.Add(new KeyValuePair<string, string>("1", "1.送貨地址"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.帳單地址"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetPeaconfKVPList 請購單確認來源
        public List<KeyValuePair<string, string>> OfGetPeaconfKVPList()
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

        #region OfGetPeastatKVPList 請購單狀態來源
        public List<KeyValuePair<string, string>> OfGetPeastatKVPList()
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

        #region OfGetPea 請購單單頭
        public DataRow OfGetPeaDr(string pPea01)
        {
            DataRow drPea = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM pea_tb");
                sbSql.AppendLine("WHERE pea01=@pea01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@pea01", pPea01) };
                drPea = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drPea;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public pea_tb OfGetPeaModel(string pPea01)
        {
            DataRow drPea = null;
            pea_tb rtnModel = null;
            try
            {
                drPea = OfGetPeaDr(pPea01);
                if (drPea == null)
                    return null;
                rtnModel = drPea.ToItem<pea_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetPeb 請購單單身
        public DataRow OfGetPebDr(string pPeb01, int pPeb02)
        {
            DataRow drPca = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM peb_tb");
                sbSql.AppendLine("WHERE peb01=@peb01");
                sbSql.AppendLine("AND peb02=@peb02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@peb01", pPeb01), new SqlParameter("@peb02", pPeb02) };
                drPca = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drPca;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public peb_tb OfGetPebModel(string pPeb01, int pPeb02)
        {
            DataRow drPeb = null;
            peb_tb rtnModel = null;
            try
            {
                drPeb = OfGetPebDr(pPeb01, pPeb02);
                if (drPeb == null)
                    return null;
                rtnModel = drPeb.ToItem<peb_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetPfaconfKVPList 採購單確認來源
        public List<KeyValuePair<string, string>> OfGetPfaconfKVPList()
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

        #region OfGetPfastatKVPList 採購單狀態來源
        public List<KeyValuePair<string, string>> OfGetPfastatKVPList()
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

        #region OfGetPfaDr 採購單單頭
        public DataRow OfGetPfaDr(string pPfa01)
        {
            DataRow drPea = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM pfa_tb");
                sbSql.AppendLine("WHERE pfa01=@pfa01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@pfa01", pPfa01) };
                drPea = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drPea;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public pfa_tb OfGetPfaModel(string pPfa01)
        {
            DataRow drPfa = null;
            pfa_tb rtnModel = null;
            try
            {
                drPfa = OfGetPfaDr(pPfa01);
                if (drPfa == null)
                    return null;
                rtnModel = drPfa.ToItem<pfa_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetPfbDr 採購單單身
        public DataRow OfGetPfbDr(string pPfb01, int pPfb02)
        {
            DataRow drPca = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM pfb_tb");
                sbSql.AppendLine("WHERE pfb01=@pfb01");
                sbSql.AppendLine("AND pfb02=@pfb02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@pfb01", pPfb01), new SqlParameter("@pfb02", pPfb02) };
                drPca = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drPca;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public pfb_tb OfGetPfbModel(string pPfb01, int pPfb02)
        {
            DataRow drPfb = null;
            pfb_tb rtnModel = null;
            try
            {
                drPfb = OfGetPfbDr(pPfb01, pPfb02);
                if (drPfb == null)
                    return null;
                rtnModel = drPfb.ToItem<pfb_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetPgaconfKVPList 入庫單確認來源
        public List<KeyValuePair<string, string>> OfGetPgaconfKVPList()
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

        #region OfGetPgaDr 入庫單單頭
        public DataRow OfGetPgaDr(string pPga01)
        {
            DataRow drPga = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM pga_tb");
                sbSql.AppendLine("WHERE pga01=@pga01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@pga01", pPga01) };
                drPga = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drPga;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public pga_tb OfGetPgaModel(string pPga01)
        {
            DataRow drSga = null;
            pga_tb rtnModel = null;
            try
            {
                drSga = OfGetPgaDr(pPga01);
                if (drSga == null)
                    return null;
                rtnModel = drSga.ToItem<pga_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetPgbDr 入庫單單身
        public DataRow OfGetPgbDr(string pPgb01, int pPgb02)
        {
            DataRow drPgb = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM pgb_tb");
                sbSql.AppendLine("WHERE pgb01=@pgb01");
                sbSql.AppendLine("AND pgb02=@pgb02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@pgb01", pPgb01), new SqlParameter("@pgb02", pPgb02) };
                drPgb = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drPgb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public pgb_tb OfGetPgbModel(string pPgb01, int pPgb02)
        {
            DataRow drPgb = null;
            pgb_tb rtnModel = null;
            try
            {
                drPgb = OfGetPgbDr(pPgb01, pPgb02);
                if (drPgb == null)
                    return null;
                rtnModel = drPgb.ToItem<pgb_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetPhaconfKVPList 退回單確認來源
        public List<KeyValuePair<string, string>> OfGetPhaconfKVPList()
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

        #region OfGetPhb17KVPList 退貨單退回類型
        public List<KeyValuePair<string, string>> OfGetPhb17KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.退貨"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.折讓"));
                return sourceList;
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
        /// <param name="pPbc01">價格條件</param>
        /// <param name="pVendor">客戶編號</param>
        /// <param name="pItem">料號</param>
        /// <param name="pUnit">單位</param>
        /// <param name="pDate">日期</param>
        /// <param name="pCurrency">幣別</param>
        /// <param name="pType">2.採購 3.收貨</param>
        /// <param name="pQty">數量</param>
        /// <param name="pTaxYN">含稅否</param>
        /// <param name="pTaxRate">稅率</param>
        /// <param name="pExRate">匯率</param>
        /// <param name="pUtaxPrice">回傳未稅價格</param>
        /// <param name="pTaxPrice">回傳含稅價格</param>
        /// <returns></returns>
        public Result OfGetPrice(string pPbc01, string pVendor, string pItem, string pUnit, DateTime? pDate, string pCurrency,
                            string pType, decimal pQty, string pTaxYN, decimal pTaxRate, decimal pExRate,
                            out decimal pUtaxPrice, out decimal pTaxPrice
            )
        {
            Result rtnResult = null;
            string sqlSelect = "";
            List<SqlParameter> sqlParmList = null;
            List<pbc_tb> pbcList = null;
            ica_tb icaModel = null;
            bek_tb bekModel = null;
            InvBLL boInv = null;
            BasBLL boBas = null;
            decimal tempUtaxPrice = 0, tempTaxPrice = 0, unitRate = 0;
            bool flag = false;
            pUtaxPrice = 0; pTaxPrice = 0;
            try
            {
                rtnResult = new Result();
                rtnResult.Key1 = pItem;
                boInv = new InvBLL(OfGetConntion());
                boInv.TRAN = this.TRAN;
                boBas = new BasBLL(OfGetConntion());
                boBas.TRAN = this.TRAN;

                pbcList = OfgetPbcList(pPbc01);
                if (pbcList == null || pbcList.Count == 0)
                {
                    rtnResult.Message = "未設定取價原則!";
                    return rtnResult;
                }

                icaModel = boInv.OfGetIcaModel(pItem);
                if (icaModel == null)
                {
                    rtnResult.Message = "查無此料號資料!";
                    return rtnResult;
                }

                bekModel = boBas.OfGetBekModel(pCurrency);
                if (bekModel == null)
                {
                    rtnResult.Message = "查無此幣別資料!";
                    return rtnResult;
                }

                foreach (pbc_tb pbcModel in pbcList.OrderBy(p => p.pbc04))
                {
                    switch (pbcModel.pbc03.ToUpper())
                    {
                        case "A1":  //依料號最近市價 考量含稅否及匯率
                            tempUtaxPrice = icaModel.ica19;
                            if (pTaxYN == "Y")
                            {
                                tempTaxPrice = tempUtaxPrice * (1 + pTaxRate / 100);
                            }
                            //取得單位換算率 傳入單位與採購單位的換算率--這個適用A1跟A2
                            flag = boInv.OfGetUnitCovert(pItem, icaModel.ica08, pUnit, out unitRate);
                            if (flag == false)
                            {
                                rtnResult.Message = "取得料號換算率失敗!";
                                return rtnResult;
                            }
                            tempUtaxPrice = tempUtaxPrice * unitRate;
                            tempTaxPrice = tempTaxPrice * unitRate;
                            break;
                        case "A2": //依料號最近採購價 考量含稅否及匯率
                            tempUtaxPrice = icaModel.ica18;
                            if (pTaxYN == "Y")
                            {
                                tempTaxPrice = tempUtaxPrice * (1 + pTaxRate / 100);
                            }
                            //取得單位換算率 傳入單位與採購單位的換算率--這個適用A1跟A2
                            flag = boInv.OfGetUnitCovert(pItem, icaModel.ica08, pUnit, out unitRate);
                            if (flag == false)
                            {
                                rtnResult.Message = "取得料號換算率失敗!";
                                return rtnResult;
                            }
                            tempUtaxPrice = tempUtaxPrice * unitRate;
                            tempTaxPrice = tempTaxPrice * unitRate;
                            break;
                        case "A3":  //依料號供應商單價
                            var pddModel = OfGetPddModel(pItem, pVendor, pCurrency);
                            if (pddModel == null)
                                continue;
                            tempUtaxPrice = pddModel.pdd09;
                            tempTaxPrice = pddModel.pdd10;

                            //取得匯率
                            //取得單位換算率 傳入單位與採購單位的換算率--這個適用A1跟A2
                            flag = boInv.OfGetUnitCovert(pItem, pddModel.pdd12, pUnit, out unitRate);
                            if (flag == false)
                            {
                                rtnResult.Message = "取得料號換算率失敗!";
                                return rtnResult;
                            }

                            tempUtaxPrice = tempUtaxPrice * unitRate;
                            tempTaxPrice = tempTaxPrice * unitRate;
                            break;
                    }
                    if (pTaxYN == "Y" && tempTaxPrice == 0)
                        continue;
                    if (pTaxYN != "Y" && tempUtaxPrice == 0)
                        continue;

                    if (pbcModel.pbc03.ToUpper() == "A1" || pbcModel.pbc03.ToUpper() == "A2")
                    {
                        pUtaxPrice = tempUtaxPrice / pExRate;
                        pTaxPrice = tempTaxPrice / pExRate;
                        pUtaxPrice = GlobalFn.Round(pUtaxPrice, bekModel.bek03);
                        pTaxPrice = GlobalFn.Round(pTaxPrice, bekModel.bek03);
                        break;
                    }

                    if (pbcModel.pbc03.ToUpper() == "A3") //查詢已包含幣別,所以不用考量匯率
                    {
                        pUtaxPrice = GlobalFn.Round(tempUtaxPrice, bekModel.bek03);
                        pTaxPrice = GlobalFn.Round(tempTaxPrice, bekModel.bek03);
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

        #region OfInsUpdSddTb 調整sdd_tb 產品客戶價格表
        //來源為採購單
        public bool OfInsUpdPddTb(pfa_tb pPfaModel, pfb_tb pPfbModel, UserInfo pLoginInfo, out string pErrMsg)
        {
            pdd_tb pddModel = null;
            int iChkCnts = 0;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            pErrMsg = "";
            DataTable dtSdd;
            DataRow drSdd;
            try
            {
                pddModel = new pdd_tb();
                pddModel.pdd01 = pPfbModel.pfb03;    //料號
                pddModel.pdd02 = pPfaModel.pfa03;    //廠商編號
                pddModel.pdd03 = pPfaModel.pfa10;    //幣別
                pddModel.pdd04 = pPfaModel.pfa17;    //匯率
                pddModel.pdd05 = pPfaModel.pfa02;    //最近採購日期
                pddModel.pdd06 = pPfaModel.pfa06;    //稅別
                pddModel.pdd07 = pPfaModel.pfa07;    //稅率
                pddModel.pdd08 = pPfaModel.pfa08;    //含稅否
                if (pPfaModel.pfa08 == "Y")//含稅時
                {
                    pddModel.pdd10 = pPfbModel.pfb09;    //含稅單價
                    pddModel.pdd09 = pPfbModel.pfb09 / (1+pPfaModel.pfa07);    //未稅單價
                }
                else
                {
                    pddModel.pdd09 = pPfbModel.pfb09;    //未稅單價
                    pddModel.pdd10 = pPfbModel.pfb05 * (1+pPfaModel.pfa07);    //含稅單價
                }
                pddModel.pdd11 = pPfbModel.pfb05;    //最近採購數量

                pddModel.pdd12 = pPfbModel.pfb06;
                pddModel.pdd13 = "";
                pddModel.pdd14 = "";
                pddModel.pdd15 = "";
                pddModel.pdd16 = "";
                pddModel.pdd17 = "";
                pddModel.pdd18 = "";
                pddModel.pdd19 = "";
                pddModel.pdd20 = "";
                pddModel.pddcreu = "";
                pddModel.pddcreg = "";
                pddModel.pddcred = null;

                OfCreateDao("pdd_tb", "*", "");
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM pdd_tb");
                sbSql.AppendLine("WHERE pdd01=@pdd01");
                sbSql.AppendLine("AND pdd02=@pdd02");
                sbSql.AppendLine("AND pdd03=@pdd03");

                sqlParmList = new List<SqlParameter>() { new SqlParameter("@pdd01", pPfbModel.pfb03), 
                                        new SqlParameter("@pdd02", pPfaModel.pfa03) ,
                                        new SqlParameter("@pdd03", pPfaModel.pfa10) 
                };
                dtSdd = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                iChkCnts = dtSdd.Rows.Count;
                if (iChkCnts == 0)//新增
                {
                    drSdd = dtSdd.NewRow();
                    dtSdd.Rows.Add(drSdd);
                }
                else
                {
                    drSdd = dtSdd.Rows[0];
                    pddModel.pddmodu = "";
                    pddModel.pddmodg = "";
                    pddModel.pddmodd = null;
                    pddModel.pddsecu = "";
                    pddModel.pddsecg = "";
                }

                drSdd["pdd01"] = pddModel.pdd01;
                drSdd["pdd02"] = pddModel.pdd02;
                drSdd["pdd03"] = pddModel.pdd03;
                drSdd["pdd04"] = pddModel.pdd04;
                drSdd["pdd05"] = pddModel.pdd05;
                drSdd["pdd06"] = pddModel.pdd06;
                drSdd["pdd07"] = pddModel.pdd07;
                drSdd["pdd08"] = pddModel.pdd08;
                drSdd["pdd09"] = pddModel.pdd09;
                drSdd["pdd10"] = pddModel.pdd10;
                drSdd["pdd11"] = pddModel.pdd11;
                drSdd["pdd12"] = pddModel.pdd12;
                drSdd["pdd13"] = pddModel.pdd13;
                drSdd["pdd14"] = pddModel.pdd14;
                drSdd["pdd15"] = pddModel.pdd15;
                drSdd["pdd16"] = pddModel.pdd16;
                drSdd["pdd17"] = pddModel.pdd17;
                drSdd["pdd18"] = pddModel.pdd18;
                drSdd["pdd19"] = pddModel.pdd19;
                drSdd["pdd20"] = pddModel.pdd20;
                if (iChkCnts == 0)//新增
                {
                    drSdd["pddcreu"] = pLoginInfo.UserNo;
                    drSdd["pddcreg"] = pLoginInfo.DeptNo;
                    drSdd["pddcred"] = OfGetToday();
                    if (OfUpdate(dtSdd) != 1)
                    {
                        pErrMsg = "新增產品廠商價格表(pdd_tb)失敗!";
                        return false;
                    }
                }
                else
                {
                    drSdd["pddmodu"] = pLoginInfo.UserNo;
                    drSdd["pddmodg"] = pLoginInfo.DeptNo;
                    drSdd["pddsecu"] = pLoginInfo.UserNo;
                    drSdd["pddsecg"] = pLoginInfo.GroupNo;
                    if (OfUpdate(dtSdd) != 1)
                    {
                        pErrMsg = "異動產品廠商價格表(pdd_tb)失敗!";
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //來源為入庫單
        public bool OfInsUpdPddTb(pga_tb pPgaModel, pgb_tb pPgbModel, UserInfo pLoginInfo, out string pErrMsg)
        {
            pdd_tb pddModel = null;
            int iChkCnts = 0;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            pErrMsg = "";
            DataTable dtSdd;
            DataRow drSdd;
            try
            {
                pddModel = new pdd_tb();
                pddModel.pdd01 = pPgbModel.pgb03;    //料號
                pddModel.pdd02 = pPgaModel.pga03;    //廠商編號
                pddModel.pdd03 = pPgaModel.pga10;    //幣別
                pddModel.pdd04 = pPgaModel.pga18;    //匯率
                pddModel.pdd05 = pPgaModel.pga02;    //最近採購日期
                pddModel.pdd06 = pPgaModel.pga06;    //稅別
                pddModel.pdd07 = pPgaModel.pga07;    //稅率
                pddModel.pdd08 = pPgaModel.pga08;    //含稅否
                if (pPgaModel.pga08 == "Y")//含稅時
                {
                    pddModel.pdd10 = pPgbModel.pgb09;    //含稅單價
                    pddModel.pdd09 = pPgbModel.pgb09 / (1+pPgaModel.pga07);    //未稅單價
                }
                else
                {
                    pddModel.pdd09 = pPgbModel.pgb09;    //未稅單價
                    pddModel.pdd10 = pPgbModel.pgb05 * (1+pPgaModel.pga07);    //含稅單價
                }
                pddModel.pdd11 = pPgbModel.pgb05;    //最近採購數量

                pddModel.pdd12 = pPgbModel.pgb06;
                pddModel.pdd13 = "";
                pddModel.pdd14 = "";
                pddModel.pdd15 = "";
                pddModel.pdd16 = "";
                pddModel.pdd17 = "";
                pddModel.pdd18 = "";
                pddModel.pdd19 = "";
                pddModel.pdd20 = "";
                pddModel.pddcreu = "";
                pddModel.pddcreg = "";
                pddModel.pddcred = null;

                OfCreateDao("pdd_tb", "*", "");
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM pdd_tb");
                sbSql.AppendLine("WHERE pdd01=@pdd01");
                sbSql.AppendLine("AND pdd02=@pdd02");
                sbSql.AppendLine("AND pdd03=@pdd03");

                sqlParmList = new List<SqlParameter>() { new SqlParameter("@pdd01", pPgbModel.pgb03), 
                                        new SqlParameter("@pdd02", pPgaModel.pga03) ,
                                        new SqlParameter("@pdd03", pPgaModel.pga10) 
                };
                dtSdd = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                iChkCnts = dtSdd.Rows.Count;
                if (iChkCnts == 0)//新增
                {
                    drSdd = dtSdd.NewRow();
                    dtSdd.Rows.Add(drSdd);
                }
                else
                {
                    drSdd = dtSdd.Rows[0];
                    pddModel.pddmodu = "";
                    pddModel.pddmodg = "";
                    pddModel.pddmodd = null;
                    pddModel.pddsecu = "";
                    pddModel.pddsecg = "";
                }

                drSdd["pdd01"] = pddModel.pdd01;
                drSdd["pdd02"] = pddModel.pdd02;
                drSdd["pdd03"] = pddModel.pdd03;
                drSdd["pdd04"] = pddModel.pdd04;
                drSdd["pdd05"] = pddModel.pdd05;
                drSdd["pdd06"] = pddModel.pdd06;
                drSdd["pdd07"] = pddModel.pdd07;
                drSdd["pdd08"] = pddModel.pdd08;
                drSdd["pdd09"] = pddModel.pdd09;
                drSdd["pdd10"] = pddModel.pdd10;
                drSdd["pdd11"] = pddModel.pdd11;
                drSdd["pdd12"] = pddModel.pdd12;
                drSdd["pdd13"] = pddModel.pdd13;
                drSdd["pdd14"] = pddModel.pdd14;
                drSdd["pdd15"] = pddModel.pdd15;
                drSdd["pdd16"] = pddModel.pdd16;
                drSdd["pdd17"] = pddModel.pdd17;
                drSdd["pdd18"] = pddModel.pdd18;
                drSdd["pdd19"] = pddModel.pdd19;
                drSdd["pdd20"] = pddModel.pdd20;
                if (iChkCnts == 0)//新增
                {
                    drSdd["pddcreu"] = pLoginInfo.UserNo;
                    drSdd["pddcreg"] = pLoginInfo.DeptNo;
                    drSdd["pddcred"] = OfGetToday();
                    if (OfUpdate(dtSdd) != 1)
                    {
                        pErrMsg = "新增產品廠商價格表(pdd_tb)失敗!";
                        return false;
                    }
                }
                else
                {
                    drSdd["pddmodu"] = pLoginInfo.UserNo;
                    drSdd["pddmodg"] = pLoginInfo.DeptNo;
                    drSdd["pddsecu"] = pLoginInfo.UserNo;
                    drSdd["pddsecg"] = pLoginInfo.GroupNo;
                    if (OfUpdate(dtSdd) != 1)
                    {
                        pErrMsg = "異動產品廠商價格表(pdd_tb)失敗!";
                        return false;
                    }
                }

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
