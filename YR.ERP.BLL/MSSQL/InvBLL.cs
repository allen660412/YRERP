/* 程式名稱: InvBLL.cs
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
    public class InvBLL : YR.ERP.BLL.MSSQL.CommonBLL
    {
        #region BasBLL() :　建構子
        public InvBLL()
            : base()
        {
        }

        public InvBLL(YR.ERP.DAL.ERP_MSSQLDAL pdao)
            : base(pdao)
        {
        }


        public InvBLL(string pComp, string pTargetTable, string pTargetColumn, string pViewTable)
            : base(pComp, pTargetTable, pTargetColumn, pViewTable)
        {

        }

        public InvBLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfChkPbaPK 檢查廠商分類
        public bool OfChkPbaPKExists(string pIca01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM ica_tb");
                sbSql.AppendLine("WHERE ica01=@ica01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@ica01", pIca01));

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

        #region OfGetIca 料號主檔
        public DataRow OfGetIcaDr(string pIca01)
        {
            DataRow drIca = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ica_tb");
                sbSql.AppendLine("WHERE ica01=@ica01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@ica01", pIca01));
                drIca = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drIca;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ica_tb OfGetIcaModel(string pIca01)
        {
            DataRow drIca = null;
            ica_tb rtnModel = null;
            try
            {
                drIca = OfGetIcaDr(pIca01);
                if (drIca == null)
                    return null;
                rtnModel = drIca.ToItem<ica_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkIcaPK 檢查料號PK是否存在
        public bool OfChkIcaPKExists(string pIca01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM ica_tb");
                sbSql.AppendLine("WHERE ica01=@ica01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@ica01", pIca01));

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

        public bool OfChkIcaPKValid(string pIca01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM ica_tb");
                sbSql.AppendLine("WHERE ica01=@ica01");
                sbSql.AppendLine("AND icaconf='Y' AND icavali='Y'");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@ica01", pIca01));

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

        #region OfGetIca02 料件品名
        public string OfGetIca02(string pIca01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT ica02 FROM ica_tb");
                sbSql.AppendLine("WHERE ica01=@ica01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("ica01", pIca01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");
                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetIca17KVPList ica17 料件屬性 P.採購件 M.自製件 S.採購件
        public List<KeyValuePair<string, string>> OfGetIca17KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("P", "P.採購件"));
                sourceList.Add(new KeyValuePair<string, string>("M", "M.自製件"));
                sourceList.Add(new KeyValuePair<string, string>("S", "S.托外生產件"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetIcb07KVPList icb07 倉庫類別 1.store 2.wip
        public List<KeyValuePair<string, string>> OfGetIcb07KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.庫存倉"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.在途倉"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetIcb 倉庫資料
        public DataRow OfGetIcbDr(string pIcb01)
        {
            DataRow drIcb = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM icb_tb");
                sbSql.AppendLine("WHERE icb01=@icb01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icb01", pIcb01));
                drIcb = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drIcb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public icb_tb OfGetIcbModel(string pIcb01)
        {
            DataRow drIcb = null;
            icb_tb rtnModel = null;
            try
            {
                drIcb = OfGetIcaDr(pIcb01);
                if (drIcb == null)
                    return null;
                rtnModel = drIcb.ToItem<icb_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkIcbPK: 檢查倉庫PK
        public bool OfChkIcbPKValid(string pIcb01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM icb_tb");
                sbSql.AppendLine("WHERE icb01=@icb01");
                sbSql.AppendLine("AND  icbvali='Y'");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icb01", pIcb01));

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

        /// <summary>
        /// 檢查是否為有效倉庫別,並區分1.庫存 2.可用倉
        /// </summary>
        /// <param name="pIcb01">倉庫</param>
        /// <param name="pIcb07">倉庫類別 1.庫存 2.可用倉</param>
        /// <returns></returns>
        public bool OfChkIcbPKValid(string pIcb01, string pIcb07)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM icb_tb");
                sbSql.AppendLine("WHERE icb01=@icb01");
                sbSql.AppendLine("AND  icb07=@icb07");
                sbSql.AppendLine("AND  icbvali='Y'");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icb01", pIcb01));
                sqlParmList.Add(new SqlParameter("@icb07", pIcb07));

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

        #region OfGetIcb02 倉庫名稱
        public string OfGetIcb02(string pIcb01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT icb02 FROM icb_tb");
                sbSql.AppendLine("WHERE icb01=@icb01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icb01", pIcb01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");
                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetIcb03 是否為成本倉
        public string OfGetIcb03(string pIcb01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT icb03 FROM icb_tb");
                sbSql.AppendLine("WHERE icb01=@icb01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icb01", pIcb01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");
                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetIcc 料號庫存資料
        public DataRow OfGetIccDr(string pIcc01, string pIcc02)
        {
            DataRow drIcc = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM icc_tb");
                sbSql.AppendLine("WHERE icc01=@icc01");
                sbSql.AppendLine("AND icc02=@icc02");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icc01", pIcc01));
                sqlParmList.Add(new SqlParameter("@icc02", pIcc02));
                drIcc = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drIcc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public icc_tb OfGetIccModel(string pIcc01, string pIcc02)
        {
            DataRow drIcc = null;
            icc_tb rtnModel = null;
            try
            {
                drIcc = OfGetIccDr(pIcc01, pIcc02);
                if (drIcc == null)
                    return null;
                rtnModel = drIcc.ToItem<icc_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetIcc05 庫存數量
        public decimal OfGetIcc05(string pIcc01, string pIcc02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            decimal retValue;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT icc05 FROM icc_tb");
                sbSql.AppendLine("WHERE icc01=@icc01");
                sbSql.AppendLine("AND icc02=@icc02");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("icc01", pIcc01));
                sqlParmList.Add(new SqlParameter("icc02", pIcc02));

                retValue = Convert.ToDecimal(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()));
                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetIcc05TotByIcc01 庫存數量加總By料號
        public decimal OfGetIcc05TotByIcc01(string pIcc01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            decimal retValue;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT ISNULL(SUM(icc05),0) FROM icc_tb");
                sbSql.AppendLine("WHERE icc01=@icc01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("icc01", pIcc01));

                retValue = Convert.ToDecimal(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()));
                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfUpdIcc05
        /// <summary>
        /// 更新料號庫存明細檔,庫存數
        /// </summary>
        /// <param name="pKind">1.加項 2.減項</param>
        /// <param name="pIcc01">料號</param>
        /// <param name="pIcc02">倉庫</param>
        /// <param name="pQty">差異量,均為加項</param>
        /// <returns></returns>
        public bool OfUpdIcc05(string pKind, string pIcc01, string pIcc02, decimal pQty, out string pMsg)
        {
            StringBuilder sbUpd;
            List<SqlParameter> sqlParmList;
            pMsg = "";
            try
            {
                sbUpd = new StringBuilder();
                sbUpd.AppendLine("UPDATE icc_tb");
                sbUpd.AppendLine("SET");
                sbUpd.AppendLine("icc05=icc05+@pQty");
                sbUpd.AppendLine("WHERE icc01=@icc01");
                sbUpd.AppendLine("AND icc02=@icc02");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icc01", pIcc01));
                sqlParmList.Add(new SqlParameter("@icc02", pIcc02));
                if (pKind == "1")
                    sqlParmList.Add(new SqlParameter("@pQty", pQty));
                else
                    sqlParmList.Add(new SqlParameter("@pQty", -1 * pQty));

                if (OfExecuteNonquery(sbUpd.ToString(), sqlParmList.ToArray()) != 1)
                {
                    pMsg = "更新失敗料號庫存明細檔(icc_tb)失敗!";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region OfChkIccPK 檢查料號庫存PK是否存在
        public bool OfChkIccPKExists(string pIcc01, string pIcc02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM icc_tb");
                sbSql.AppendLine("WHERE icc01=@icc01");
                sbSql.AppendLine("AND icc02=@icc02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icc01", pIcc01));
                sqlParmList.Add(new SqlParameter("@icc02", pIcc02));

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

        #region OfChkIcmPK 檢查單位基本換算pk是否存在
        public bool OfChkIcmPKExists(string pIcm01, string pIcm02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM icm_tb");
                sbSql.AppendLine("WHERE icm01=@icm01");
                sbSql.AppendLine("AND icm02=@icm02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icm01", pIcm01));
                sqlParmList.Add(new SqlParameter("@icm02", pIcm02));

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

        #region OfGetIcm 單位基本換算資料維護
        public DataRow OfGetIcmDr(string pIcm01, string pIcm02)
        {
            DataRow drBab = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM icm_tb");
                sbSql.AppendLine("WHERE icm01=@icm01");
                sbSql.AppendLine("AND icm02=@icm02");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icm01", pIcm01));
                sqlParmList.Add(new SqlParameter("@icm02", pIcm02));
                drBab = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drBab;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public icm_tb OfGetIcmModel(string pIcm01, string pIcm02)
        {
            DataRow drIcm = null;
            icm_tb rtnModel = null;
            try
            {
                drIcm = OfGetIcmDr(pIcm01, pIcm02);
                if (drIcm == null)
                    return null;
                rtnModel = drIcm.ToItem<icm_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkIcnPK 檢查料件單位基本換算pk是否存在
        public bool OfChkIcnPKExists(string pIcm01, string pIcm02, string pIcm03)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM icn_tb");
                sbSql.AppendLine("WHERE icn01=@icn01");
                sbSql.AppendLine("AND icn02=@icn02");
                sbSql.AppendLine("AND icn03=@icn03");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icm01", pIcm01));
                sqlParmList.Add(new SqlParameter("@icm02", pIcm02));
                sqlParmList.Add(new SqlParameter("@icm03", pIcm03));

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

        #region OfGetIcnDr 料件單位基本換算資料維護
        public DataRow OfGetIcnDr(string pIcn01, string pIcn02, string pIcn03)
        {
            DataRow drBab = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM icn_tb");
                sbSql.AppendLine("WHERE icn01=@icn01");
                sbSql.AppendLine("AND icn02=@icn02");
                sbSql.AppendLine("AND icn03=@icn03");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icn01", pIcn01));
                sqlParmList.Add(new SqlParameter("@icn02", pIcn02));
                sqlParmList.Add(new SqlParameter("@icn03", pIcn03));
                drBab = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drBab;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public icn_tb OfGetIcnModel(string pIcm01, string pIcm02, string pIcn03)
        {
            DataRow drIcm = null;
            icn_tb rtnModel = null;
            try
            {
                drIcm = OfGetIcnDr(pIcm01, pIcm02, pIcn03);
                if (drIcm == null)
                    return null;
                rtnModel = drIcm.ToItem<icn_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetIcpDr 料件圖檔
        public DataRow OfGetIcpDr(string pIcp01)
        {
            DataRow drIcp = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM icp_tb");
                sbSql.AppendLine("WHERE icp01=@icp01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@icp01", pIcp01) };
                drIcp = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drIcp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public icp_tb OfGetIcpModel(string pIcp01)
        {
            DataRow drIcp = null;
            icp_tb rtnModel = null;
            try
            {
                drIcp = OfGetIlaDr(pIcp01);
                if (drIcp == null)
                    return null;
                rtnModel = drIcp.ToItem<icp_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkIcpPK 檢查料號圖檔PK是否存在
        public bool OfChkIcpPKExists(string pIcp01, int pIcp02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM icp_tb");
                sbSql.AppendLine("WHERE icp01=@icp01");
                sbSql.AppendLine("AND icp02=@icp02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icp01", pIcp01));
                sqlParmList.Add(new SqlParameter("@icp02", pIcp02));

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

        #region OfGetIcx01KVPList icx01料號分類別
        public List<KeyValuePair<string, string>> OfGetIcx01KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.大分類"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.中分類"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.小分類"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkIcxPK 檢查料號分類
        public bool OfChkIcxPKExists(string pIcx01, string pIcx02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM icx_tb");
                sbSql.AppendLine("WHERE icx01=@icx01");
                sbSql.AppendLine("AND icx02=@icx02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icx01", pIcx01));
                sqlParmList.Add(new SqlParameter("@icx02", pIcx02));

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

        #region OfGetIcx03 取得料件分類碼 大中小類
        public string OfGetIcx03(string pIcx01, string pIcx02)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT icx03 FROM icx_tb");
                sbSql.AppendLine("WHERE icx01=@icx01");
                sbSql.AppendLine("AND icx02=@icx02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@icx01", pIcx01));
                sqlParmList.Add(new SqlParameter("@icx02", pIcx02));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetIfaconfKVPList 調撥單確認來源
        public List<KeyValuePair<string, string>> OfGetIfaconfKVPList()
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

        #region OfGetIgaconfKVPList 採購單確認來源
        public List<KeyValuePair<string, string>> OfGetIgaconfKVPList()
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

        #region OfGetIlaconfKVPList 借出單確認來源
        public List<KeyValuePair<string, string>> OfGetIlaconfKVPList()
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

        #region OfGetPfastatKVPList 借出單狀態來源
        public List<KeyValuePair<string, string>> OfGetIlastatKVPList()
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

        #region OfGetImaconfKVPList 借出歸還單確認來源
        public List<KeyValuePair<string, string>> OfGetImaconfKVPList()
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

        #region OfGetIlaDr 借出單單頭
        public DataRow OfGetIlaDr(string pIla01)
        {
            DataRow drIla = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ila_tb");
                sbSql.AppendLine("WHERE ila01=@ila01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@ila01", pIla01) };
                drIla = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drIla;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ila_tb OfGetIlaModel(string pIla01)
        {
            DataRow drIla = null;
            ila_tb rtnModel = null;
            try
            {
                drIla = OfGetIlaDr(pIla01);
                if (drIla == null)
                    return null;
                rtnModel = drIla.ToItem<ila_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetIlbDr 借出單單身
        public DataRow OfGetIlbDr(string pIlb01, int pIlb02)
        {
            DataRow drIlb = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ilb_tb");
                sbSql.AppendLine("WHERE ilb01=@ilb01");
                sbSql.AppendLine("AND ilb02=@ilb02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@ilb01", pIlb01), new SqlParameter("@ilb02", pIlb02) };
                drIlb = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drIlb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ilb_tb OfGetIlbModel(string pIlb01, int pIlb02)
        {
            DataRow drIlb = null;
            ilb_tb rtnModel = null;
            try
            {
                drIlb = OfGetIlbDr(pIlb01, pIlb02);
                if (drIlb == null)
                    return null;
                rtnModel = drIlb.ToItem<ilb_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfInsIna 新增庫存異動單據歷史檔
        public bool OfInsIna(ina_tb pIna, out string pErrMsg)
        {
            DataTable dtIca;
            DataRow drIca;
            StringBuilder sbIns = null;
            pErrMsg = "";
            try
            {
                OfCreateDao("ina_tb", "*", "");
                sbIns = new StringBuilder();
                sbIns.AppendLine("SELECT * FROM ina_tb");
                sbIns.AppendLine("WHERE 1<>1");
                dtIca = OfGetDataTable(sbIns.ToString());
                drIca = dtIca.NewRow();

                drIca["ina01"] = pIna.ina01;
                drIca["ina02"] = pIna.ina02;
                drIca["ina03"] = pIna.ina03;
                drIca["ina04"] = pIna.ina04;
                drIca["ina05"] = pIna.ina05;
                drIca["ina06"] = pIna.ina06;
                drIca["ina07"] = pIna.ina07;
                drIca["ina08"] = pIna.ina08;
                drIca["ina09"] = pIna.ina09;
                drIca["ina10"] = pIna.ina10;
                drIca["ina11"] = pIna.ina11;
                drIca["ina12"] = pIna.ina12;
                drIca["ina13"] = pIna.ina13;
                drIca["ina14"] = pIna.ina14;
                drIca["ina15"] = pIna.ina15;
                drIca["inacomp"] = pIna.inacomp;
                drIca["inacreu"] = pIna.inacreu;
                drIca["inacreg"] = pIna.inacreg;
                if (pIna.inacred != null)
                    drIca["inacred"] = pIna.inacred;
                else
                    drIca["inacred"] = DBNull.Value;
                drIca["inamodu"] = pIna.inamodu;
                drIca["inamodg"] = pIna.inamodg;
                if (pIna.inamodd != null)
                    drIca["inamodd"] = pIna.inamodd;
                else
                    drIca["inamodd"] = DBNull.Value;

                drIca["inasecu"] = pIna.inasecu;
                drIca["inasecg"] = pIna.inasecg;

                dtIca.Rows.Add(drIca);
                if (OfUpdate(dtIca) != 1)
                {
                    pErrMsg = "新增庫存異動單據歷史檔(ina_tb)失敗!";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfDelIna 刪除庫存異動單據歷史檔
        /// <summary>
        /// 刪除庫存異動單據歷史檔
        /// </summary>
        /// <param name="pIna01">單號</param>
        /// <param name="pIna02">項次</param>
        /// <param name="pIna03">1.入庫 2.出庫</param>
        /// <param name="pErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public bool OfDelIna(string pIna01, decimal pIna02, string pIna03, out string pErrMsg)
        {
            StringBuilder sbDel = null;
            List<SqlParameter> sqlParmList = null;
            pErrMsg = "";
            try
            {
                sbDel = new StringBuilder();
                sbDel.AppendLine("DELETE FROM ina_tb");
                sbDel.AppendLine("WHERE ina01=@ina01");
                sbDel.AppendLine("AND ina02=@ina02");
                sbDel.AppendLine("AND ina03=@ina03");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@ina01", pIna01));
                sqlParmList.Add(new SqlParameter("@ina02", pIna02));
                sqlParmList.Add(new SqlParameter("@ina03", pIna03));
                if (OfExecuteNonquery(sbDel.ToString(), sqlParmList.ToArray()) < 0)//刪不到時 不報錯
                {
                    pErrMsg = "刪除庫存異動單據歷史檔ina_tb,失敗!";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool OfDelIna(string pIna01, decimal pIna02, string pIna03, string pIna04, out string pErrMsg)
        {
            StringBuilder sbDel = null;
            List<SqlParameter> sqlParmList = null;
            pErrMsg = "";
            try
            {
                sbDel = new StringBuilder();
                sbDel.AppendLine("DELETE FROM ina_tb");
                sbDel.AppendLine("WHERE ina01=@ina01");
                sbDel.AppendLine("AND ina02=@ina02");
                sbDel.AppendLine("AND ina03=@ina03");
                sbDel.AppendLine("AND ina04=@ina04");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@ina01", pIna01));
                sqlParmList.Add(new SqlParameter("@ina02", pIna02));
                sqlParmList.Add(new SqlParameter("@ina03", pIna03));
                sqlParmList.Add(new SqlParameter("@ina04", pIna04));
                if (OfExecuteNonquery(sbDel.ToString(), sqlParmList.ToArray()) < 0)//刪不到時 不報錯
                {
                    pErrMsg = "刪除庫存異動單據歷史檔ina_tb,失敗!";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetIna03KVPList ina03入出庫 1.入庫 2.出庫
        public List<KeyValuePair<string, string>> OfGetIna03KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.入庫"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.出庫"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfStockPost 庫存扣帳 針對各隻程式處理 ina_tb 庫存異動單據歷史檔
        //入庫單
        public bool OfStockPost(string pFormId, pga_tb pPgaModel, pgb_tb pPgbModel,UserInfo pLoginInfo, out string pErrMsg)
        {
            ina_tb inaModel = null;
            pErrMsg = "";
            try
            {
                inaModel = new ina_tb();
                inaModel.ina01 = pPgaModel.pga01;
                inaModel.ina02 = pPgbModel.pgb02;
                inaModel.ina03 = "1";//入庫
                inaModel.ina04 = pFormId;
                inaModel.ina05 = pPgbModel.pgb03;   //料號
                inaModel.ina06 = pPgbModel.pgb16;   //倉庫
                inaModel.ina07 = pPgbModel.pgb05;   //單據數量
                inaModel.ina08 = pPgbModel.pgb06;   //單據單位
                inaModel.ina09 = pPgbModel.pgb08;   //對庫存轉換率
                inaModel.ina10 = pPgbModel.pgb18;   //庫存數量
                inaModel.ina11 = pPgbModel.pgb07;   //庫存單位
                inaModel.ina12 = OfGetIcb03(pPgbModel.pgb16);
                inaModel.ina13 = pPgaModel.pga02;   //入庫日期
                inaModel.ina14 = OfGetNow();
                inaModel.ina15 = pPgaModel.pga03;
                inaModel.inacomp = pPgbModel.pgbcomp;

                inaModel.inacreu = pLoginInfo.UserNo;
                inaModel.inacreg = pLoginInfo.DeptNo;
                inaModel.inacred = OfGetNow();
                inaModel.inamodu = pLoginInfo.UserNo;
                inaModel.inamodg = pLoginInfo.DeptNo;
                inaModel.inamodd = OfGetNow();
                inaModel.inasecu = pLoginInfo.UserNo;
                inaModel.inasecg = pLoginInfo.GroupNo;
                if (OfInsIna(inaModel, out pErrMsg) == false)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //入庫退回
        public bool OfStockPost(string pFormId, pha_tb pPhaModel, phb_tb pPhbModel, UserInfo pLoginInfo, out string pErrMsg)
        {
            ina_tb inaModel = null;
            pErrMsg = "";
            try
            {
                inaModel = new ina_tb();
                inaModel.ina01 = pPhaModel.pha01;
                inaModel.ina02 = pPhbModel.phb02;
                inaModel.ina03 = "2";//入庫
                inaModel.ina04 = pFormId;
                inaModel.ina05 = pPhbModel.phb03;   //料號
                inaModel.ina06 = pPhbModel.phb16;   //倉庫
                inaModel.ina07 = pPhbModel.phb05;   //單據數量
                inaModel.ina08 = pPhbModel.phb06;   //單據單位
                inaModel.ina09 = pPhbModel.phb08;   //對庫存轉換率
                inaModel.ina10 = pPhbModel.phb18;   //庫存數量
                inaModel.ina11 = pPhbModel.phb07;   //庫存單位
                inaModel.ina12 = OfGetIcb03(pPhbModel.phb16);
                inaModel.ina13 = pPhaModel.pha02;   //銷退日期
                inaModel.ina14 = OfGetNow();
                inaModel.ina15 = pPhaModel.pha03;
                inaModel.inacomp = pPhbModel.phbcomp;
                inaModel.inacreu = pLoginInfo.UserNo;
                inaModel.inacreg = pLoginInfo.DeptNo;
                inaModel.inacred = OfGetNow();
                inaModel.inamodu = pLoginInfo.UserNo;
                inaModel.inamodg = pLoginInfo.DeptNo;
                inaModel.inamodd = OfGetNow();
                inaModel.inasecu = pLoginInfo.UserNo;
                inaModel.inasecg = pLoginInfo.GroupNo;
                if (OfInsIna(inaModel, out pErrMsg) == false)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //銷貨單
        public bool OfStockPost(string pFormId, sga_tb pSgaModel, sgb_tb pSgbModel, UserInfo pLoginInfo, out string pErrMsg)
        {
            ina_tb inaModel = null;
            pErrMsg = "";
            try
            {
                inaModel = new ina_tb();
                inaModel.ina01 = pSgaModel.sga01;
                inaModel.ina02 = pSgbModel.sgb02;
                inaModel.ina03 = "2";//出庫
                inaModel.ina04 = pFormId;
                inaModel.ina05 = pSgbModel.sgb03;   //料號
                inaModel.ina06 = pSgbModel.sgb16;   //倉庫
                inaModel.ina07 = pSgbModel.sgb05;   //單據數量
                inaModel.ina08 = pSgbModel.sgb06;   //單據單位
                inaModel.ina09 = pSgbModel.sgb08;   //對庫存轉換率
                inaModel.ina10 = pSgbModel.sgb18;   //庫存數量
                inaModel.ina11 = pSgbModel.sgb07;   //庫存單位
                inaModel.ina12 = OfGetIcb03(pSgbModel.sgb16);
                inaModel.ina13 = pSgaModel.sga02; //出庫日期
                inaModel.ina14 = OfGetNow();
                inaModel.ina15 = pSgaModel.sga03;
                inaModel.inacomp = pSgbModel.sgbcomp;
                inaModel.inacreu = pLoginInfo.UserNo;
                inaModel.inacreg = pLoginInfo.DeptNo;
                inaModel.inacred = OfGetNow();
                inaModel.inamodu = pLoginInfo.UserNo;
                inaModel.inamodg = pLoginInfo.DeptNo;
                inaModel.inamodd = OfGetNow();
                inaModel.inasecu = pLoginInfo.UserNo;
                inaModel.inasecg = pLoginInfo.GroupNo;
                if (OfInsIna(inaModel, out pErrMsg) == false)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //銷貨退回
        public bool OfStockPost(string pFormId, sha_tb pShaModel, shb_tb pShbModel, UserInfo pLoginInfo, out string pErrMsg)
        {
            ina_tb inaModel = null;
            pErrMsg = "";
            try
            {
                inaModel = new ina_tb();
                inaModel.ina01 = pShaModel.sha01;
                inaModel.ina02 = pShbModel.shb02;
                inaModel.ina03 = "1";//入庫
                inaModel.ina04 = pFormId;
                inaModel.ina05 = pShbModel.shb03;   //料號
                inaModel.ina06 = pShbModel.shb16;   //倉庫
                inaModel.ina07 = pShbModel.shb05;   //單據數量
                inaModel.ina08 = pShbModel.shb06;   //單據單位
                inaModel.ina09 = pShbModel.shb08;   //對庫存轉換率
                inaModel.ina10 = pShbModel.shb18;   //庫存數量
                inaModel.ina11 = pShbModel.shb07;   //庫存單位
                inaModel.ina12 = OfGetIcb03(pShbModel.shb16);
                inaModel.ina13 = pShaModel.sha02;   //銷退日期
                inaModel.ina14 = OfGetNow();
                inaModel.ina15 = pShaModel.sha03;
                inaModel.inacomp = pShbModel.shbcomp;
                inaModel.inacreu = pLoginInfo.UserNo;
                inaModel.inacreg = pLoginInfo.DeptNo;
                inaModel.inacred = OfGetNow();
                inaModel.inamodu = pLoginInfo.UserNo;
                inaModel.inamodg = pLoginInfo.DeptNo;
                inaModel.inamodd = OfGetNow();
                inaModel.inasecu = pLoginInfo.UserNo;
                inaModel.inasecg = pLoginInfo.GroupNo;
                if (OfInsIna(inaModel, out pErrMsg) == false)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //庫存異動單
        public bool OfStockPost(string pFormId, iga_tb pIgaModel, igb_tb pIgbModel, UserInfo pLoginInfo, out string pErrMsg)
        {
            ina_tb inaModel = null;
            pErrMsg = "";
            try
            {
                inaModel = new ina_tb();
                inaModel.ina01 = pIgaModel.iga01;
                inaModel.ina02 = pIgbModel.igb02;
                if (pIgaModel.iga00 == "1")  //庫存異動-發出
                    inaModel.ina03 = "2";//出庫
                else if (pIgaModel.iga00 == "2")//庫存異動-收料
                    inaModel.ina03 = "1";//入庫

                inaModel.ina04 = pFormId;
                inaModel.ina05 = pIgbModel.igb03;   //料號
                inaModel.ina06 = pIgbModel.igb09;   //倉庫
                inaModel.ina07 = pIgbModel.igb05;   //單據數量
                inaModel.ina08 = pIgbModel.igb06;   //單據單位
                inaModel.ina09 = pIgbModel.igb12;   //對庫存轉換率
                inaModel.ina10 = pIgbModel.igb13;   //庫存數量
                inaModel.ina11 = pIgbModel.igb11;   //庫存單位
                inaModel.ina12 = OfGetIcb03(pIgbModel.igb09);
                inaModel.ina13 = pIgaModel.iga02;   //異動日期
                inaModel.ina14 = OfGetNow();
                inaModel.ina15 = "";
                inaModel.inacomp = pIgbModel.igbcomp;
                inaModel.inacreu = pLoginInfo.UserNo;
                inaModel.inacreg = pLoginInfo.DeptNo;
                inaModel.inacred = OfGetNow();
                inaModel.inamodu = pLoginInfo.UserNo;
                inaModel.inamodg = pLoginInfo.DeptNo;
                inaModel.inamodd = OfGetNow();
                inaModel.inasecu = pLoginInfo.UserNo;
                inaModel.inasecg = pLoginInfo.GroupNo;
                if (OfInsIna(inaModel, out pErrMsg) == false)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //調撥單
        public bool OfStockPost(string pFormId, ifa_tb pIfaModel, ifb_tb pIfbModel, UserInfo pLoginInfo, out string pErrMsg)
        {
            ina_tb inaModel = null;
            pErrMsg = "";
            try
            {
                if (pFormId.ToLower() != "invt330" &&
                    pFormId.ToLower() != "invt331" &&
                    pFormId.ToLower() != "invt332"
                    )
                {
                    pErrMsg = "無此作業";
                    return false;
                }
                switch (pFormId.ToLower())
                {
                    case "invt330": //直接調撥
                        //先處理撥出
                        inaModel = new ina_tb();
                        inaModel.ina01 = pIfaModel.ifa01;
                        inaModel.ina02 = pIfbModel.ifb02;
                        inaModel.ina03 = "2";//出庫
                        inaModel.ina04 = pFormId;
                        inaModel.ina05 = pIfbModel.ifb03;   //料號
                        inaModel.ina06 = pIfbModel.ifb07;   //倉庫
                        inaModel.ina07 = pIfbModel.ifb05;   //單據數量
                        inaModel.ina08 = pIfbModel.ifb06;   //單據單位
                        inaModel.ina09 = pIfbModel.ifb12;   //對庫存轉換率
                        inaModel.ina10 = pIfbModel.ifb13;   //庫存數量
                        inaModel.ina11 = pIfbModel.ifb11;   //庫存單位
                        inaModel.ina12 = OfGetIcb03(pIfbModel.ifb07);
                        inaModel.ina13 = pIfaModel.ifa07;   //異動日期 以單頭撥出日期
                        inaModel.ina14 = OfGetNow();
                        inaModel.ina15 = "";
                        inaModel.inacomp = pIfbModel.ifbcomp;
                        inaModel.inacreu = pLoginInfo.UserNo;
                        inaModel.inacreg = pLoginInfo.DeptNo;
                        inaModel.inacred = OfGetNow();
                        inaModel.inamodu = pLoginInfo.UserNo;
                        inaModel.inamodg = pLoginInfo.DeptNo;
                        inaModel.inamodd = OfGetNow();
                        inaModel.inasecu = pLoginInfo.UserNo;
                        inaModel.inasecg = pLoginInfo.GroupNo;
                        if (OfInsIna(inaModel, out pErrMsg) == false)
                        {
                            return false;
                        }
                        //再處理轉撥入倉
                        inaModel = new ina_tb();
                        inaModel.ina01 = pIfaModel.ifa01;
                        inaModel.ina02 = pIfbModel.ifb02;
                        inaModel.ina03 = "1";//入庫
                        inaModel.ina04 = pFormId;
                        inaModel.ina05 = pIfbModel.ifb03;   //料號
                        inaModel.ina06 = pIfbModel.ifb10;   //倉庫
                        inaModel.ina07 = pIfbModel.ifb05;   //單據數量
                        inaModel.ina08 = pIfbModel.ifb06;   //單據單位
                        inaModel.ina09 = pIfbModel.ifb12;   //對庫存轉換率
                        inaModel.ina10 = pIfbModel.ifb13;   //庫存數量
                        inaModel.ina11 = pIfbModel.ifb11;   //庫存單位
                        inaModel.ina12 = OfGetIcb03(pIfbModel.ifb10);
                        inaModel.ina13 = pIfaModel.ifa07;   //異動日期 以單頭撥入日期
                        inaModel.ina14 = OfGetNow();
                        inaModel.ina15 = "";
                        inaModel.inacomp = pIfbModel.ifbcomp;
                        inaModel.inacreu = pLoginInfo.UserNo;
                        inaModel.inacreg = pLoginInfo.DeptNo;
                        inaModel.inacred = OfGetNow();
                        inaModel.inamodu = pLoginInfo.UserNo;
                        inaModel.inamodg = pLoginInfo.DeptNo;
                        inaModel.inamodd = OfGetNow();
                        inaModel.inasecu = pLoginInfo.UserNo;
                        inaModel.inasecg = pLoginInfo.GroupNo;
                        if (OfInsIna(inaModel, out pErrMsg) == false)
                        {
                            return false;
                        }
                        break;

                    case "invt331"://兩階段撥出
                        inaModel = new ina_tb();
                        //先處理撥出
                        inaModel.ina01 = pIfaModel.ifa01;
                        inaModel.ina02 = pIfbModel.ifb02;
                        inaModel.ina03 = "2";//出庫
                        inaModel.ina04 = pFormId;
                        inaModel.ina05 = pIfbModel.ifb03;   //料號
                        inaModel.ina06 = pIfbModel.ifb07;   //倉庫
                        inaModel.ina07 = pIfbModel.ifb05;   //單據數量
                        inaModel.ina08 = pIfbModel.ifb06;   //單據單位
                        inaModel.ina09 = pIfbModel.ifb12;   //對庫存轉換率
                        inaModel.ina10 = pIfbModel.ifb13;   //庫存數量
                        inaModel.ina11 = pIfbModel.ifb11;   //庫存單位
                        inaModel.ina12 = OfGetIcb03(pIfbModel.ifb07);
                        inaModel.ina13 = pIfaModel.ifa07;   //異動日期 以單頭撥出日期
                        inaModel.ina14 = OfGetNow();
                        inaModel.ina15 = "";
                        inaModel.inacomp = pIfbModel.ifbcomp;
                        inaModel.inacreu = pLoginInfo.UserNo;
                        inaModel.inacreg = pLoginInfo.DeptNo;
                        inaModel.inacred = OfGetNow();
                        inaModel.inamodu = pLoginInfo.UserNo;
                        inaModel.inamodg = pLoginInfo.DeptNo;
                        inaModel.inamodd = OfGetNow();
                        inaModel.inasecu = pLoginInfo.UserNo;
                        inaModel.inasecg = pLoginInfo.GroupNo;
                        if (OfInsIna(inaModel, out pErrMsg) == false)
                        {
                            return false;
                        }
                        //再處理轉再途倉
                        inaModel = new ina_tb();
                        inaModel.ina01 = pIfaModel.ifa01;
                        inaModel.ina02 = pIfbModel.ifb02;
                        inaModel.ina03 = "1";//入庫
                        inaModel.ina04 = pFormId;
                        inaModel.ina05 = pIfbModel.ifb03;   //料號
                        inaModel.ina06 = pIfaModel.ifa06;   //倉庫
                        inaModel.ina07 = pIfbModel.ifb05;   //單據數量
                        inaModel.ina08 = pIfbModel.ifb06;   //單據單位
                        inaModel.ina09 = pIfbModel.ifb12;   //對庫存轉換率
                        inaModel.ina10 = pIfbModel.ifb13;   //庫存數量
                        inaModel.ina11 = pIfbModel.ifb11;   //庫存單位
                        inaModel.ina12 = OfGetIcb03(pIfaModel.ifa06);
                        inaModel.ina13 = pIfaModel.ifa07;   //異動日期 以單頭撥出日期
                        inaModel.ina14 = OfGetNow();
                        inaModel.ina15 = "";
                        inaModel.inacomp = pIfbModel.ifbcomp;
                        inaModel.inacreu = pLoginInfo.UserNo;
                        inaModel.inacreg = pLoginInfo.DeptNo;
                        inaModel.inacred = OfGetNow();
                        inaModel.inamodu = pLoginInfo.UserNo;
                        inaModel.inamodg = pLoginInfo.DeptNo;
                        inaModel.inamodd = OfGetNow();
                        inaModel.inasecu = pLoginInfo.UserNo;
                        inaModel.inasecg = pLoginInfo.GroupNo;
                        if (OfInsIna(inaModel, out pErrMsg) == false)
                        {
                            return false;
                        }
                        break;

                    case "invt332"://兩階段撥入
                        inaModel = new ina_tb();
                        //先處理撥出-在途倉
                        inaModel.ina01 = pIfaModel.ifa01;
                        inaModel.ina02 = pIfbModel.ifb02;
                        inaModel.ina03 = "2";//出庫
                        inaModel.ina04 = pFormId;
                        inaModel.ina05 = pIfbModel.ifb03;   //料號
                        inaModel.ina06 = pIfaModel.ifa06;   //倉庫
                        inaModel.ina07 = pIfbModel.ifb05;   //單據數量
                        inaModel.ina08 = pIfbModel.ifb06;   //單據單位
                        inaModel.ina09 = pIfbModel.ifb12;   //對庫存轉換率
                        inaModel.ina10 = pIfbModel.ifb13;   //庫存數量
                        inaModel.ina11 = pIfbModel.ifb11;   //庫存單位
                        inaModel.ina12 = OfGetIcb03(pIfaModel.ifa06);
                        inaModel.ina13 = pIfaModel.ifa10;   //異動日期 以單頭撥入日期
                        inaModel.ina14 = OfGetNow();
                        inaModel.ina15 = "";
                        inaModel.inacomp = pIfbModel.ifbcomp;
                        inaModel.inacreu = pLoginInfo.UserNo;
                        inaModel.inacreg = pLoginInfo.DeptNo;
                        inaModel.inacred = OfGetNow();
                        inaModel.inamodu = pLoginInfo.UserNo;
                        inaModel.inamodg = pLoginInfo.DeptNo;
                        inaModel.inamodd = OfGetNow();
                        inaModel.inasecu = pLoginInfo.UserNo;
                        inaModel.inasecg = pLoginInfo.GroupNo;
                        if (OfInsIna(inaModel, out pErrMsg) == false)
                        {
                            return false;
                        }
                        //再處理轉撥入倉
                        inaModel = new ina_tb();
                        inaModel.ina01 = pIfaModel.ifa01;
                        inaModel.ina02 = pIfbModel.ifb02;
                        inaModel.ina03 = "1";//入庫
                        inaModel.ina04 = pFormId;
                        inaModel.ina05 = pIfbModel.ifb03;   //料號
                        inaModel.ina06 = pIfbModel.ifb10;   //倉庫
                        inaModel.ina07 = pIfbModel.ifb05;   //單據數量
                        inaModel.ina08 = pIfbModel.ifb06;   //單據單位
                        inaModel.ina09 = pIfbModel.ifb12;   //對庫存轉換率
                        inaModel.ina10 = pIfbModel.ifb13;   //庫存數量
                        inaModel.ina11 = pIfbModel.ifb11;   //庫存單位
                        inaModel.ina12 = OfGetIcb03(pIfbModel.ifb10);
                        inaModel.ina13 = pIfaModel.ifa10;   //異動日期 以單頭撥入日期
                        inaModel.ina14 = OfGetNow();
                        inaModel.ina15 = "";
                        inaModel.inacomp = pIfbModel.ifbcomp;
                        inaModel.inacreu = pLoginInfo.UserNo;
                        inaModel.inacreg = pLoginInfo.DeptNo;
                        inaModel.inacred = OfGetNow();
                        inaModel.inamodu = pLoginInfo.UserNo;
                        inaModel.inamodg = pLoginInfo.DeptNo;
                        inaModel.inamodd = OfGetNow();
                        inaModel.inasecu = pLoginInfo.UserNo;
                        inaModel.inasecg = pLoginInfo.GroupNo;
                        if (OfInsIna(inaModel, out pErrMsg) == false)
                        {
                            return false;
                        }
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //借出單 一筆資料會含出庫與入庫
        public bool OfStockPost(string pFormId, ila_tb pIlaModel, ilb_tb pIlbModel, UserInfo pLoginInfo, out string pErrMsg)
        {
            ina_tb inaModel = null;
            pErrMsg = "";
            try
            {
                if (pFormId == "invt401")  //借出單
                {
                    //先處理 出庫
                    inaModel = new ina_tb();
                    inaModel.ina01 = pIlaModel.ila01;
                    inaModel.ina02 = pIlbModel.ilb02;
                    inaModel.ina03 = "2";//出庫

                    inaModel.ina04 = pFormId;
                    inaModel.ina05 = pIlbModel.ilb03;   //料號
                    inaModel.ina06 = pIlbModel.ilb11;   //倉庫
                    inaModel.ina07 = pIlbModel.ilb05;   //單據數量
                    inaModel.ina08 = pIlbModel.ilb06;   //單據單位
                    inaModel.ina09 = pIlbModel.ilb08;   //對庫存轉換率
                    inaModel.ina10 = pIlbModel.ilb17;   //庫存數量
                    inaModel.ina11 = pIlbModel.ilb07;   //庫存單位
                    inaModel.ina12 = OfGetIcb03(pIlbModel.ilb11);
                    inaModel.ina13 = pIlaModel.ila02;   //借出日期
                    inaModel.ina14 = OfGetNow();
                    inaModel.ina15 = pIlaModel.ila03;
                    inaModel.inacomp = pIlbModel.ilbcomp;
                    inaModel.inacreu = pLoginInfo.UserNo;
                    inaModel.inacreg = pLoginInfo.DeptNo;
                    inaModel.inacred = OfGetNow();
                    inaModel.inamodu = pLoginInfo.UserNo;
                    inaModel.inamodg = pLoginInfo.DeptNo;
                    inaModel.inamodd = OfGetNow();
                    inaModel.inasecu = pLoginInfo.UserNo;
                    inaModel.inasecg = pLoginInfo.GroupNo;
                    if (OfInsIna(inaModel, out pErrMsg) == false)
                    {
                        return false;
                    }

                    //再處理 入庫
                    inaModel = new ina_tb();
                    inaModel.ina01 = pIlaModel.ila01;
                    inaModel.ina02 = pIlbModel.ilb02;
                    inaModel.ina03 = "1";//入庫

                    inaModel.ina04 = pFormId;
                    inaModel.ina05 = pIlbModel.ilb03;   //料號
                    inaModel.ina06 = pIlbModel.ilb12;   //倉庫
                    inaModel.ina07 = pIlbModel.ilb05;   //單據數量
                    inaModel.ina08 = pIlbModel.ilb06;   //單據單位
                    inaModel.ina09 = pIlbModel.ilb08;   //對庫存轉換率
                    inaModel.ina10 = pIlbModel.ilb17;   //庫存數量
                    inaModel.ina11 = pIlbModel.ilb07;   //庫存單位
                    inaModel.ina12 = OfGetIcb03(pIlbModel.ilb11);
                    inaModel.ina13 = pIlaModel.ila02;   //借出日期
                    inaModel.ina14 = OfGetNow();
                    inaModel.ina15 = pIlaModel.ila03;
                    inaModel.inacomp = pIlbModel.ilbcomp;
                    inaModel.inacreu = pLoginInfo.UserNo;
                    inaModel.inacreg = pLoginInfo.DeptNo;
                    inaModel.inacred = OfGetNow();
                    inaModel.inamodu = pLoginInfo.UserNo;
                    inaModel.inamodg = pLoginInfo.DeptNo;
                    inaModel.inamodd = OfGetNow();
                    inaModel.inasecu = pLoginInfo.UserNo;
                    inaModel.inasecg = pLoginInfo.GroupNo;
                    if (OfInsIna(inaModel, out pErrMsg) == false)
                    {
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
        //借出歸還單 一筆資料會含出庫與入庫
        public bool OfStockPost(string pFormId, ima_tb pImaModel, imb_tb pImbModel, UserInfo pLoginInfo, out string pErrMsg)
        {
            ina_tb inaModel = null;
            pErrMsg = "";
            try
            {
                if (pFormId == "invt402")  //借出單
                {
                    //先處理 出庫
                    inaModel = new ina_tb();
                    inaModel.ina01 = pImaModel.ima01;
                    inaModel.ina02 = pImbModel.imb02;
                    inaModel.ina03 = "2";//出庫

                    inaModel.ina04 = pFormId;
                    inaModel.ina05 = pImbModel.imb03;   //料號
                    inaModel.ina06 = pImbModel.imb18;   //倉庫--出庫倉
                    inaModel.ina07 = pImbModel.imb05;   //單據數量
                    inaModel.ina08 = pImbModel.imb06;   //單據單位
                    inaModel.ina09 = pImbModel.imb08;   //對庫存轉換率
                    inaModel.ina10 = pImbModel.imb16;   //庫存數量
                    inaModel.ina11 = pImbModel.imb07;   //庫存單位
                    inaModel.ina12 = OfGetIcb03(pImbModel.imb18);   //是否為成本倉
                    inaModel.ina13 = pImaModel.ima02;   //歸還日期
                    inaModel.ina14 = OfGetNow();
                    inaModel.ina15 = pImaModel.ima03;
                    inaModel.inacomp = pImbModel.imbcomp;
                    inaModel.inacreu = pLoginInfo.UserNo;
                    inaModel.inacreg = pLoginInfo.DeptNo;
                    inaModel.inacred = OfGetNow();
                    inaModel.inamodu = pLoginInfo.UserNo;
                    inaModel.inamodg = pLoginInfo.DeptNo;
                    inaModel.inamodd = OfGetNow();
                    inaModel.inasecu = pLoginInfo.UserNo;
                    inaModel.inasecg = pLoginInfo.GroupNo;
                    if (OfInsIna(inaModel, out pErrMsg) == false)
                    {
                        return false;
                    }

                    //再處理 入庫
                    inaModel = new ina_tb();
                    inaModel.ina01 = pImaModel.ima01;
                    inaModel.ina02 = pImbModel.imb02;
                    inaModel.ina03 = "1";//入庫

                    inaModel.ina04 = pFormId;
                    inaModel.ina05 = pImbModel.imb03;   //料號
                    inaModel.ina06 = pImbModel.imb19;   //倉庫--入庫倉
                    inaModel.ina07 = pImbModel.imb05;   //單據數量
                    inaModel.ina08 = pImbModel.imb06;   //單據單位
                    inaModel.ina09 = pImbModel.imb08;   //對庫存轉換率
                    inaModel.ina10 = pImbModel.imb16;   //庫存數量
                    inaModel.ina11 = pImbModel.imb07;   //庫存單位
                    inaModel.ina12 = OfGetIcb03(pImbModel.imb19);   //是否為成本倉
                    inaModel.ina13 = pImaModel.ima02;   //歸還日期
                    inaModel.ina14 = OfGetNow();
                    inaModel.ina15 = pImaModel.ima03;
                    inaModel.inacomp = pImbModel.imbcomp;
                    inaModel.inacreu = pLoginInfo.UserNo;
                    inaModel.inacreg = pLoginInfo.DeptNo;
                    inaModel.inacred = OfGetNow();
                    inaModel.inamodu = pLoginInfo.UserNo;
                    inaModel.inamodg = pLoginInfo.DeptNo;
                    inaModel.inamodd = OfGetNow();
                    inaModel.inasecu = pLoginInfo.UserNo;
                    inaModel.inasecg = pLoginInfo.GroupNo;
                    if (OfInsIna(inaModel, out pErrMsg) == false)
                    {
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

        //託工送料單 
        public bool OfStockPost(string pFormId, mfa_tb pMfaModel, mfb_tb pMfbModel, UserInfo pLoginInfo, out string pErrMsg)
        {
            ina_tb inaModel = null;
            pErrMsg = "";
            try
            {
                if (pFormId == "mant311")  //託工單送料單
                {
                    //先處理 出庫
                    inaModel = new ina_tb();
                    inaModel.ina01 = pMfaModel.mfa01;
                    inaModel.ina02 = pMfbModel.mfb02;
                    inaModel.ina03 = "2";//出庫

                    inaModel.ina04 = pFormId;
                    inaModel.ina05 = pMfbModel.mfb03;   //料號
                    inaModel.ina06 = pMfbModel.mfb07;   //倉庫
                    inaModel.ina07 = pMfbModel.mfb05;   //單據數量
                    inaModel.ina08 = pMfbModel.mfb06;   //單據單位
                    inaModel.ina09 = 1;            //對庫存轉換率
                    inaModel.ina10 = pMfbModel.mfb05;   //庫存數量
                    inaModel.ina11 = pMfbModel.mfb06;   //庫存單位
                    inaModel.ina12 = OfGetIcb03(pMfbModel.mfb07);
                    inaModel.ina13 = pMfaModel.mfa02;   //借出日期
                    inaModel.ina14 = OfGetNow();
                    inaModel.ina15 = pMfaModel.mfa03;
                    inaModel.inacomp = pMfbModel.mfbcomp;
                    inaModel.inacreu = pLoginInfo.UserNo;
                    inaModel.inacreg = pLoginInfo.DeptNo;
                    inaModel.inacred = OfGetNow();
                    inaModel.inamodu = pLoginInfo.UserNo;
                    inaModel.inamodg = pLoginInfo.DeptNo;
                    inaModel.inamodd = OfGetNow();
                    inaModel.inasecu = pLoginInfo.UserNo;
                    inaModel.inasecg = pLoginInfo.GroupNo;
                    if (OfInsIna(inaModel, out pErrMsg) == false)
                    {
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

        //託工退料單
        public bool OfStockPost(string pFormId, mga_tb pMgaModel, mgb_tb pMgbModel, UserInfo pLoginInfo, out string pErrMsg)
        {
            ina_tb inaModel = null;
            pErrMsg = "";
            try
            {
                if (pFormId == "mant312")  //託工單退料單
                {
                    //先處理 出庫
                    inaModel = new ina_tb();
                    inaModel.ina01 = pMgaModel.mga01;
                    inaModel.ina02 = pMgbModel.mgb02;
                    inaModel.ina03 = "2";//出庫

                    inaModel.ina04 = pFormId;
                    inaModel.ina05 = pMgbModel.mgb03;   //料號
                    inaModel.ina06 = pMgbModel.mgb07;   //出庫倉
                    inaModel.ina07 = pMgbModel.mgb05;   //單據數量
                    inaModel.ina08 = pMgbModel.mgb06;   //單據單位
                    inaModel.ina09 = 1;            //對庫存轉換率
                    inaModel.ina10 = pMgbModel.mgb05;   //庫存數量
                    inaModel.ina11 = pMgbModel.mgb06;   //庫存單位
                    inaModel.ina12 = OfGetIcb03(pMgbModel.mgb07);
                    inaModel.ina13 = pMgaModel.mga02;   //借出日期
                    inaModel.ina14 = OfGetNow();
                    inaModel.ina15 = pMgaModel.mga03;
                    inaModel.inacomp = pMgbModel.mgbcomp;
                    inaModel.inacreu = pLoginInfo.UserNo;
                    inaModel.inacreg = pLoginInfo.DeptNo;
                    inaModel.inacred = OfGetNow();
                    inaModel.inamodu = pLoginInfo.UserNo;
                    inaModel.inamodg = pLoginInfo.DeptNo;
                    inaModel.inamodd = OfGetNow();
                    inaModel.inasecu = pLoginInfo.UserNo;
                    inaModel.inasecg = pLoginInfo.GroupNo;
                    if (OfInsIna(inaModel, out pErrMsg) == false)
                    {
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

        //託工入庫單
        public bool OfStockPost(string pFormId, mha_tb pMhaModel, mhb_tb pMhbModel, UserInfo pLoginInfo, out string pErrMsg)
        {
            ina_tb inaModel = null;
            pErrMsg = "";
            try
            {
                if (pFormId == "mant411")  //託工入庫單
                {
                    //先處理 出庫
                    inaModel = new ina_tb();
                    inaModel.ina01 = pMhaModel.mha01;
                    inaModel.ina02 = pMhbModel.mhb02;
                    inaModel.ina03 = "1";//入庫

                    inaModel.ina04 = pFormId;
                    inaModel.ina05 = pMhbModel.mhb03;   //料號
                    inaModel.ina06 = pMhbModel.mhb07;   //入庫倉
                    inaModel.ina07 = pMhbModel.mhb05;   //單據數量
                    inaModel.ina08 = pMhbModel.mhb06;   //單據單位
                    inaModel.ina09 = 1;            //對庫存轉換率
                    inaModel.ina10 = pMhbModel.mhb05;   //庫存數量
                    inaModel.ina11 = pMhbModel.mhb06;   //庫存單位
                    inaModel.ina12 = OfGetIcb03(pMhbModel.mhb07);
                    inaModel.ina13 = pMhaModel.mha02;   //入庫日期
                    inaModel.ina14 = OfGetNow();
                    inaModel.ina15 = pMhaModel.mha03;
                    inaModel.inacomp = pMhbModel.mhbcomp;
                    inaModel.inacreu = pLoginInfo.UserNo;
                    inaModel.inacreg = pLoginInfo.DeptNo;
                    inaModel.inacred = OfGetNow();
                    inaModel.inamodu = pLoginInfo.UserNo;
                    inaModel.inamodg = pLoginInfo.DeptNo;
                    inaModel.inamodd = OfGetNow();
                    inaModel.inasecu = pLoginInfo.UserNo;
                    inaModel.inasecg = pLoginInfo.GroupNo;
                    if (OfInsIna(inaModel, out pErrMsg) == false)
                    {
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

        //託工入庫退回單
        public bool OfStockPost(string pFormId, mia_tb pMiaModel, mib_tb pMibModel, UserInfo pLoginInfo, out string pErrMsg)
        {
            ina_tb inaModel = null;
            pErrMsg = "";
            try
            {
                if (pFormId == "mant412")  //託工入庫退回單
                {
                    inaModel = new ina_tb();
                    inaModel.ina01 = pMiaModel.mia01;
                    inaModel.ina02 = pMibModel.mib02;
                    inaModel.ina03 = "2";//入庫

                    inaModel.ina04 = pFormId;
                    inaModel.ina05 = pMibModel.mib03;   //料號
                    inaModel.ina06 = pMibModel.mib07;   //入庫倉
                    inaModel.ina07 = pMibModel.mib05;   //單據數量
                    inaModel.ina08 = pMibModel.mib06;   //單據單位
                    inaModel.ina09 = 1;            //對庫存轉換率
                    inaModel.ina10 = pMibModel.mib05;   //庫存數量
                    inaModel.ina11 = pMibModel.mib06;   //庫存單位
                    inaModel.ina12 = OfGetIcb03(pMibModel.mib07);
                    inaModel.ina13 = pMiaModel.mia02;   //退回日期
                    inaModel.ina14 = OfGetNow();
                    inaModel.ina15 = pMiaModel.mia03;
                    inaModel.inacomp = pMibModel.mibcomp;
                    inaModel.inacreu = pLoginInfo.UserNo;
                    inaModel.inacreg = pLoginInfo.DeptNo;
                    inaModel.inacred = OfGetNow();
                    inaModel.inamodu = pLoginInfo.UserNo;
                    inaModel.inamodg = pLoginInfo.DeptNo;
                    inaModel.inamodd = OfGetNow();
                    inaModel.inasecu = pLoginInfo.UserNo;
                    inaModel.inasecg = pLoginInfo.GroupNo;
                    if (OfInsIna(inaModel, out pErrMsg) == false)
                    {
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

        #region OfGetUnitCovert 取得料件轉換率
        public bool OfGetUnitCovert(string pIca01, string pSourceUnit, string pTargetUnit, out decimal pdConvert)
        {
            icn_tb l_icn;//料件單位
            icm_tb l_icm;//一般單位換算
            pdConvert = 0;
            try
            {
                if (pSourceUnit == pTargetUnit)
                {
                    pdConvert = 1;
                    return true;
                }
                l_icn = OfGetIcnModel(pIca01, pSourceUnit, pTargetUnit);
                if (l_icn != null)
                {
                    pdConvert = Math.Round(l_icn.icn05 / l_icn.icn04, 8, MidpointRounding.AwayFromZero);
                    return true;
                }
                l_icm = OfGetIcmModel(pSourceUnit, pTargetUnit);
                if (l_icn == null)
                {
                    return false;
                }
                pdConvert = Math.Round(l_icm.icm04 / l_icm.icm03, 8, MidpointRounding.AwayFromZero);
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
