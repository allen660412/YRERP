/* 程式名稱: AdmBLL.cs
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
    public class AdmBLL : YR.ERP.BLL.MSSQL.CommonBLL
    {
        #region AdmBLL() :　建構子
        public AdmBLL()
            : base()
        {
        }

        public AdmBLL(YR.ERP.DAL.ERP_MSSQLDAL pdao)
            : base(pdao)
        {
        }


        public AdmBLL(string ps_comp, string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable)
            : base(ps_comp, ps_TargetTable, ps_TargetColumn, ps_ViewTable)
        {

        }

        public AdmBLL(string ps_comp, string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable,bool pGenNonSelectCommand=true)
            : base(ps_comp, ps_TargetTable, ps_TargetColumn, ps_ViewTable, pGenNonSelectCommand)
        {

        }

        public AdmBLL(System.Data.Common.DbConnection pConnection, string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable)
            : base(pConnection, ps_TargetTable, ps_TargetColumn, ps_ViewTable)
        {
            //this.OfCreateDao(pConnection, ps_TargetTable, ps_TargetColumn, ps_ViewTable);
        }

        public AdmBLL(System.Data.Common.DbConnection pConnection, string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable, bool pGenNonSelectCommand)
            : base(pConnection, ps_TargetTable, ps_TargetColumn, ps_ViewTable, pGenNonSelectCommand)
        {
            //this.OfCreateDao(pConnection, ps_TargetTable, ps_TargetColumn, ps_ViewTable);
        }

        public AdmBLL(DbConnection pConnection) :base(pConnection)
        {
            base.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfGetAda 使用者登入檔
        public DataRow OfGetAdaDr(string pAda01)
        {
            DataRow drAda = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ada_tb");
                sbSql.AppendLine("WHERE ada01=@ada01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@ada01", pAda01) };
                drAda = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drAda;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ada_tb OfGetAdaModel(string pAda01)
        {
            DataRow drAda = null;
            ada_tb rtnModel = null;
            try
            {
                drAda = OfGetAdaDr(pAda01);
                if (drAda == null)
                    return null;
                rtnModel = drAda.ToItem<ada_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkAdaPKExists
        public bool OfChkAdaPKExists(string pAda01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int chkCnts = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM ada_tb");
                sbSql.AppendLine("WHERE ada01=@ada01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("ada01", pAda01));
                
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

        #region OfGetAdcDr 角色權限檔
        public DataRow OfGetAdcDr(string pAdc01)
        {
            DataRow drAdd = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM adc_tb");
                sbSql.AppendLine("WHERE adc01=@adc01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@adc01", pAdc01));
                drAdd = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drAdd;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public adc_tb OfGetAdcModel(string pAdc01)
        {
            DataRow drAdd = null;
            adc_tb rtnModel = null;
            try
            {
                drAdd = OfGetAdcDr(pAdc01);
                if (drAdd == null)
                    return null;
                rtnModel = drAdd.ToItem<adc_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetAdc02 角色定義說明
        public string OfGetAdc02(string pAdc01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT adc02 FROM adc_tb");
                sbSql.AppendLine("WHERE adc01=@adc01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("adc01", pAdc01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");
                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkAdcPK 檢查角色權限檔pk是否存在
        public bool OfChkAdcPKExists(string pAdc01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM adc_tb");
                sbSql.AppendLine("WHERE adc01=@adc01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@adc01", pAdc01));
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

        #region OfGetAddDr 角色權限明細檔
        public DataRow OfGetAddDr(string pAdd01,string pAdd02)
        {
            DataRow drAdd = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM add_tb");
                sbSql.AppendLine("WHERE add01=@add01");
                sbSql.AppendLine("AND add02=@add02");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@add01", pAdd01));
                sqlParmList.Add(new SqlParameter("@add02", pAdd02));
                drAdd = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drAdd;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public add_tb OfGetAddModel(string pAdd01, string pAdd02)
        {
            DataRow drAdd = null;
            add_tb rtnModel = null;
            try
            {
                drAdd = OfGetAddDr(pAdd01,pAdd02);
                if (drAdd == null)
                    return null;
                rtnModel = drAdd.ToItem<add_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkAddPK 檢查角色權限明細檔pk是否存在
        public bool OfChkAddPKExists(string pAdd01, string pAdd02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM add_tb");
                sbSql.AppendLine("WHERE add01=@add01");
                sbSql.AppendLine("AND add02=@add02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@add01", pAdd01));
                sqlParmList.Add(new SqlParameter("@add02", pAdd02));

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

        #region OfGetAdeDr 群組設定
        public DataRow OfGetAdeDr(string pAde01)
        {
            DataRow drAde = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ade_tb");
                sbSql.AppendLine("WHERE ade01=@ade01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@ade01", pAde01));
                drAde = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drAde;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ade_tb OfGetAdeModel(string pAde01)
        {
            DataRow drAde = null;
            ade_tb rtnModel = null;
            try
            {
                drAde = OfGetAdeDr(pAde01);
                if (drAde == null)
                    return null;
                rtnModel = drAde.ToItem<ade_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetAde02 群組名稱
        public string OfGetAde02(string pAde01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT ade02 FROM ade_tb");
                sbSql.AppendLine("WHERE ade01=@ade01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("ade01", pAde01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkAdePK 檢查角色權限明細檔pk是否存在
        public bool OfChkAdePKExists(string pAde01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM ade_tb");
                sbSql.AppendLine("WHERE ade01=@ade01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@ade01", pAde01));

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

        #region OfGetAdf 流程圖資料--待作廢
        //public DataRow OfGetAdfDr(string pAdf01)
        //{
        //    DataRow drAdf = null;
        //    List<SqlParameter> sqlParmList;
        //    StringBuilder sbSql;
        //    try
        //    {
        //        sbSql = new StringBuilder();
        //        sbSql.AppendLine("SELECT * FROM adf_tb");
        //        sbSql.AppendLine("WHERE adf01=@adf01");
        //        sqlParmList = new List<SqlParameter>() { new SqlParameter("@adf01", pAdf01) };
        //        drAdf = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
        //        return drAdf;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public adf_tb OfGetAdfModel(string pAdf01)
        //{
        //    DataRow drAdf = null;
        //    adf_tb rtnModel = null;
        //    try
        //    {
        //        drAdf = OfGetAdfDr(pAdf01);
        //        if (drAdf == null)
        //            return null;
        //        rtnModel = drAdf.ToItem<adf_tb>();

        //        return rtnModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion

        #region OfChkAdfPK 檢查流程圖pk是否存在--待作廢
        //public bool OfChkAdfPKExists(string pAdf01)
        //{
        //    StringBuilder sbSql;
        //    List<SqlParameter> sqlParmList;
        //    int iChks = 0;
        //    try
        //    {
        //        sbSql = new StringBuilder();
        //        sbSql.AppendLine("SELECT COUNT(1) FROM adf_tb");
        //        sbSql.AppendLine("WHERE adf01=@adf01");

        //        sqlParmList = new List<SqlParameter>();
        //        sqlParmList.Add(new SqlParameter("@adf01", pAdf01));

        //        iChks = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
        //        if (iChks == 0)
        //            return false;

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion

        #region OfGetAdg 流程圖元件位置--待作廢
        //public DataRow OfGetAdgDr(string pAdg01,string pAdg02)
        //{
        //    DataRow drAdg = null;
        //    List<SqlParameter> sqlParmList;
        //    StringBuilder sbSql;
        //    try
        //    {
        //        sbSql = new StringBuilder();
        //        sbSql.AppendLine("SELECT * FROM adg_tb");
        //        sbSql.AppendLine("WHERE adg01=@adg01");
        //        sbSql.AppendLine("      AND adg02=@adg02");
        //        sqlParmList = new List<SqlParameter>() { new SqlParameter("@adg01", pAdg01),
        //                                                  new SqlParameter("@adg02", pAdg02)};
        //        drAdg = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
        //        return drAdg;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public adg_tb OfGetAdgModel(string pAdg01, string pAdg02)
        //{
        //    DataRow drAdg = null;
        //    adg_tb rtnModel = null;
        //    try
        //    {
        //        drAdg = OfGetAdgDr(pAdg01, pAdg02);
        //        if (drAdg == null)
        //            return null;
        //        rtnModel = drAdg.ToItem<adg_tb>();

        //        return rtnModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public List<adg_tb> OfGetAdgList(string pAdg01)
        //{
        //    DataTable dtAdg = null;
        //    List<SqlParameter> sqlParmList;
        //    StringBuilder sbSql;
        //    try
        //    {
        //        sbSql = new StringBuilder();
        //        sbSql.AppendLine("SELECT * FROM adg_tb");
        //        sbSql.AppendLine("WHERE adg01=@adg01");
        //        sqlParmList = new List<SqlParameter>() { new SqlParameter("@adg01", pAdg01),
        //                                                  };
        //        dtAdg = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
        //        return dtAdg.ToList<adg_tb>();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion

        #region OfChkAdo01Exists 檢查程式代號
        public bool OfChkAdoPKExists(string pAdo01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM ado_tb");
                sbSql.AppendLine("WHERE ado01=@ado01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@ado01", pAdo01));

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

        public bool OfChkAdoPKExists(string pAdo01,string pAdo07)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM ado_tb");
                sbSql.AppendLine("WHERE ado01=@ado01");
                sbSql.AppendLine("AND ado07=@ado07");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@ado01", pAdo01));
                sqlParmList.Add(new SqlParameter("@ado07", pAdo07));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

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

        #region OfGetAdo07KVPList
        public List<KeyValuePair<string, string>> OfGetAdo07KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("M", "M.目錄"));
                sourceList.Add(new KeyValuePair<string, string>("P", "P.程式"));
                sourceList.Add(new KeyValuePair<string, string>("R", "R.報表"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetAdo13KVPList
        public List<KeyValuePair<string, string>> OfGetAdo13KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.清單/明細表"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.憑證"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.統計表"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetAdo 程式設定資料檔
        public DataRow OfGetAdoDr(string psAdo01)
        {
            DataRow drAdo = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ado_tb");
                sbSql.AppendLine("WHERE ado01=@ado01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@ado01", psAdo01) };
                drAdo = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drAdo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ado_tb OfGetAdoModel(string psAdo01)
        {
            DataRow drAdo = null;
            ado_tb rtnModel=null;
            try
            {
                drAdo = OfGetAdoDr(psAdo01);
                if (drAdo == null)
                    return null;
                rtnModel = drAdo.ToItem<ado_tb>();
                
                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region OfGetAdo02
        public string OfGetAdo02(string pAdo01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT ado02 FROM ado_tb");
                sbSql.AppendLine("WHERE ado01=@ado01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("ado01", pAdo01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #endregion

        #region OfGetAdp 程式對應報表設定明細檔
        public DataRow OfGetAdpDr(string pAdp01, string adp02)
        {
            DataRow drAdp = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM adp_tb");
                sbSql.AppendLine("WHERE adp01=@adp01");
                sbSql.AppendLine("AND adp02=@adp02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@adp01", pAdp01),
                                                          new SqlParameter("@adp02",adp02)};
                drAdp = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drAdp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable OfGetAdpDt(string pAdp01)
        {
            DataTable dtAdp = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM adp_tb");
                sbSql.AppendLine("WHERE adp01=@adp01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@adp01", pAdp01) };
                dtAdp = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                return dtAdp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public adp_tb OfGetAdpModel(string pAdp01, string pAdp02)
        {
            DataRow drAdp;
            adp_tb returnModel;
            try
            {
                drAdp = OfGetAdpDr(pAdp01, pAdp02);
                if (drAdp == null)
                    return null;
                returnModel = drAdp.ToItem<adp_tb>();

                return returnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<adp_tb> OfGetAdpModels(string pAdp01)
        {
            DataTable dtAdp;
            List<adp_tb> adpTbList;
            try
            {
                dtAdp = OfGetAdpDt(pAdp01);
                if (dtAdp == null)
                    return null;
                adpTbList = dtAdp.ToList<adp_tb>();

                return adpTbList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetAdq 程式功能設定 action+report
        public DataRow OfGetAdqDr(string pAdq01,string pAdq02)
        {
            DataRow drAdq = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM adq_tb");
                sbSql.AppendLine("WHERE adq01=@adq01");
                sbSql.AppendLine("AND adq02=@adq02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@adq01", pAdq01),
                                                         new SqlParameter("@adq02",pAdq02)};
                drAdq = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drAdq;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public adq_tb OfGetAdqModel(string pAdq01, string pAdq02)
        {
            DataRow drAdq = null;
            adq_tb rtnModel = null;
            try
            {
                drAdq = OfGetAdqDr(pAdq01,pAdq02);
                if (drAdq == null)
                    return null;
                rtnModel = drAdq.ToItem<adq_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable OfGetAdqDt(string pAdq01)
        {
            DataTable dtAdq = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM adq_tb");
                sbSql.AppendLine("WHERE adq01=@adq01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@adq01", pAdq01)};
                dtAdq = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                return dtAdq;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<adq_tb> OfGetAdqList(string pAdq01)
        {
            DataTable dtAdq = null;
            List<adq_tb> rtnList = null;
            try
            {
                dtAdq = OfGetAdqDt(pAdq01);
                if (dtAdq == null)
                    return null;
                rtnList = dtAdq.ToList<adq_tb>();

                return rtnList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetAdq04KVPList 功能類型-1.ACTION 2.報表
        public List<KeyValuePair<string, string>> OfGetAdq04KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.action"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.report"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetAdx 流程圖資料
        public DataRow OfGetAdxDr(string pAdx01)
        {
            DataRow drAdx = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM adx_tb");
                sbSql.AppendLine("WHERE adx01=@adx01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@adx01", pAdx01) };
                drAdx = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drAdx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public adx_tb OfGetAdxModel(string pAdx01)
        {
            DataRow drAdx = null;
            adx_tb rtnModel = null;
            try
            {
                drAdx = OfGetAdxDr(pAdx01);
                if (drAdx == null)
                    return null;
                rtnModel = drAdx.ToItem<adx_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkAdxPK 檢查流程圖pk是否存在
        public bool OfChkAdxPKExists(string pAdx01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM adx_tb");
                sbSql.AppendLine("WHERE adx01=@adx01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@adx01", pAdx01));

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

        #region OfGetAdg 流程圖元件位置--待作廢
        public DataRow OfGetAdyDr(string pAdy01, string pAdy02)
        {
            DataRow drAdy = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ady_tb");
                sbSql.AppendLine("WHERE ady01=@ady01");
                sbSql.AppendLine("      AND ady02=@ady02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@ady01", pAdy01),
                                                          new SqlParameter("@ady02", pAdy02)};
                drAdy = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drAdy;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ady_tb OfGetAdyModel(string pAdy01, string pAdy02)
        {
            DataRow drAdy = null;
            ady_tb rtnModel = null;
            try
            {
                drAdy = OfGetAdyDr(pAdy01, pAdy02);
                if (drAdy == null)
                    return null;
                rtnModel = drAdy.ToItem<ady_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ady_tb> OfGetAdyList(string pAdy01)
        {
            DataTable dtAdy = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ady_tb");
                sbSql.AppendLine("WHERE ady01=@ady01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@ady01", pAdy01),
                                                          };
                dtAdy = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                return dtAdy.ToList<ady_tb>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetAta 公司對應資料庫
        public DataRow OfGetAtaDr(string pAta01)
        {
            DataRow drAta = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM ata_tb");
                sbSql.AppendLine("WHERE ata01=@ata01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@ata01", pAta01) };
                drAta = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drAta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ata_tb OfGetAtaModel(string pAta01)
        {
            DataRow drAdo = null;
            ata_tb rtnModel = null;
            try
            {
                drAdo = OfGetAtaDr(pAta01);
                if (drAdo == null)
                    return null;
                rtnModel = drAdo.ToItem<ata_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string OfGetAta02(string pAda01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT ata02 FROM ata_tb");
                sbSql.AppendLine("WHERE ata01=@ata01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("ata01", pAda01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkAtaPKExists
        public bool OfChkAtaPKExists(string pAta01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int chkCnts = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM ata_tb");
                sbSql.AppendLine("WHERE ata01=@ata01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("ata01", pAta01));

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

        #region OfChkAtb01Exists
        public bool OfChkAtb01Exists(string pAtb01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM atb_tb");
                sbSql.AppendLine("WHERE atb01=@atb01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("atb01", pAtb01));

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

        #region OfGetAtb02
        public string OfGetAtb02(string pAtb01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT atb02 FROM atb_tb");
                sbSql.AppendLine("WHERE atb01=@atb01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("atb01", pAtb01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion

        #region OfGetAtc 資料欄位說明檔
        public DataRow OfGetAtcDr(string pAtc02)
        {
            DataRow drPca = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM atc_tb");
                sbSql.AppendLine("WHERE atc02=@atc02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@atc02", pAtc02) };
                drPca = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drPca;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public atc_tb OfGetScaModel(string pAtc02)
        {
            DataRow drBab = null;
            atc_tb rtnModel = null;
            try
            {
                drBab = OfGetAtcDr(pAtc02);
                if (drBab == null)
                    return null;
                rtnModel = drBab.ToItem<atc_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetAtc03 取得欄位(column)說明
        public string OfGetAtc03(string pAtc02)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT atc03 FROM atc_tb");
                sbSql.AppendLine("WHERE atc02=@atc02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@atc02", pAtc02));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkAtc02Exists 取得欄位(column)說明
        public bool OfChkAtc02Exists(string pAtc02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM atc_tb");
                sbSql.AppendLine("WHERE atc02=@atc02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@atc02", pAtc02));

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

        #region OfGetAza 畫面名稱說明檔
        public DataRow OfGetAzaDr(string pAza01,string pAza03)
        {
            DataRow drAza = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM aza_tb");
                sbSql.AppendLine("WHERE aza01=@aza01");
                sbSql.AppendLine("AND aza03=@aza03");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@aza01", pAza01),
                                                          new SqlParameter("@aza03",pAza03)};
                drAza = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drAza;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable OfGetAzaDt(string psAza01)
        {
            DataTable dtAza = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM aza_tb");
                sbSql.AppendLine("WHERE aza01=@aza01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@aza01", psAza01) };
                dtAza = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                return dtAza;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public aza_tb OfGetAzaModel(string pAza01,string pAza03)
        {
            DataRow drAza;
            aza_tb azaTbModel;
            try
            {
                drAza = OfGetAzaDr(pAza01, pAza03);
                if (drAza == null)
                    return null;
                azaTbModel = drAza.ToItem<aza_tb>();

                return azaTbModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<aza_tb> OfGetAzaModels(string psAza01)
        {
            DataTable dtAza;
            List<aza_tb> azaTbModel;
            try
            {
                dtAza = OfGetAzaDt(psAza01);
                if (dtAza == null)
                    return null;
                azaTbModel=dtAza.ToList<aza_tb>();

                return azaTbModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkAzaPK 檢查畫面名稱說明檔PK是否存在
        public bool OfChkAzaPKExists(string pAza01, string pAza03)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM aza_tb");
                sbSql.AppendLine("WHERE aza01=@aza01");
                sbSql.AppendLine("AND aza03=@aza03");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@aza01", pAza01));
                sqlParmList.Add(new SqlParameter("@aza03", pAza03));

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

        #region OfGetAza12KVPList
        public List<KeyValuePair<string, string>> OfGetAza12KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("", "不處理"));
                sourceList.Add(new KeyValuePair<string, string>("U", "U.大寫"));
                sourceList.Add(new KeyValuePair<string, string>("L", "L.小寫"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkAze01Exists 模組代號
        public bool OfChkAze01Exists(string pAze01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM aze_tb");
                sbSql.AppendLine("WHERE aze01=@aze01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@aze01", pAze01));

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

        #region OfGetAze01KVPList
        public List<KeyValuePair<string, string>> OfGetAze01KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT aze01,aze02 FROM aze_tb");
                sourceList=OfGetKVP(sbSql.ToString());
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetAzf 單別性質--單身
        public DataRow OfGetAzfDr(string pAzf01,string pAzf02)
        {
            DataRow drBab = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM azf_tb");
                sbSql.AppendLine("WHERE azf01=@azf01");
                sbSql.AppendLine("  AND azf02=@azf02");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@azf01", pAzf01),
                                                          new SqlParameter("@azf02", pAzf02)};
                drBab = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drBab;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public azf_tb OfGetAzfModel(string pAzf01, string pAzf02)
        {
            DataRow drAzf = null;
            azf_tb rtnModel = null;            
            try
            {
                drAzf = OfGetAzfDr(pAzf01, pAzf02);
                if (drAzf == null)
                    return null;
                rtnModel = drAzf.ToItem<azf_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkAzf02Exists 模組代號+單據性質代號
        public bool OfChkAzf02Exists(string pAzf01,string pAzf02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM azf_tb");
                sbSql.AppendLine("WHERE azf01=@azf01");
                sbSql.AppendLine("  AND azf02=@azf02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@azf01", pAzf01));
                sqlParmList.Add(new SqlParameter("@azf02", pAzf02));

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

        #region OfGetAzf02 取得單據性質說明
        public string OfGetAzf02(string pAzf01,string pAzf02)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT azf03 FROM azf_tb");
                sbSql.AppendLine("WHERE azf01=@azf01");
                sbSql.AppendLine("AND azf02=@azf02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@azf01", pAzf01));
                sqlParmList.Add(new SqlParameter("@azf02", pAzf02));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetAzq06KVPList Pick明細-元件類型
        public List<KeyValuePair<string, string>> OfGetAzq06KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.edit"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.checkbox"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.combo"));
                sourceList.Add(new KeyValuePair<string, string>("4", "4.date"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkAzp01Exists 開窗代號
        public bool OfChkAzp01Exists(string pAzp01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM azp_tb");
                sbSql.AppendLine("WHERE azp01=@azp01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("azp01", pAzp01));

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

    }
}
