using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Runtime;
using System.Reflection;
using YR.DAL;
using YR.ERP.DAL;
using YR.Util;
using System.Configuration;

namespace YR.ERP.BLL
{
    public class BLLMSSQLBase : YR.ERP.Shared.IBLL, IDisposable
    {
        #region Property
        public System.Data.Common.DbTransaction TRAN;
        //protected internal YR.ERP.DAL.ERP_MSSQLDAL m_ap_dao;
        private YR.ERP.DAL.ERP_MSSQLDAL m_ap_dao;
        protected YR.ERP.DAL.ERP_MSSQLDAL AP_DAO
        {
            get { return m_ap_dao; }
            set { m_ap_dao = value; }
        }         
        #endregion

        #region BLLBase() : 建構子
        public BLLMSSQLBase()
        {
            this.OfCreateDao();
        }
        #endregion

        #region BLLBase(YR.ERP.DAL.ERP_MSSQLDAL pdao) : 建構子
        public BLLMSSQLBase(YR.ERP.DAL.ERP_MSSQLDAL pdao)
        {
            AP_DAO = pdao;
        }
        #endregion

        #region BLLBase(YR.ERP.DAL.ERP_MSSQLDAL pdao) : 建構子
        public BLLMSSQLBase(DbConnection pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion


        #region  BLLBase(string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable) : 建構子
        public BLLMSSQLBase(System.Data.Common.DbConnection pConnection, string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable)
        {
            this.OfCreateDao(pConnection, ps_TargetTable, ps_TargetColumn, ps_ViewTable);
        }
        #endregion


        #region  BLLBase(string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable) : 建構子
        public BLLMSSQLBase(System.Data.Common.DbConnection pConnection, string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable,
            bool pGenNonSelectCommand = true
            )
        {
            this.OfCreateDao(pConnection, ps_TargetTable, ps_TargetColumn, ps_ViewTable, pGenNonSelectCommand);
        }
        #endregion


        #region  BLLBase(string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable) : 建構子
        public BLLMSSQLBase(string ps_comp,string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable,
            bool pGenNonSelectCommand = true
            )
        {
            this.OfCreateDao(ps_comp,ps_TargetTable, ps_TargetColumn, ps_ViewTable,pGenNonSelectCommand);
        }
        #endregion

        #region OfCreateDao() : 建立 DAL 物件 instance
        /// <summary>
        /// 建立 DAL 物件 instance
        /// </summary>
        /// <returns></returns>
        public virtual void OfCreateDao()
        {
            if (m_ap_dao == null)
            {
                this.OfCreateDao("", "", "");
            }
        }
        #endregion

        #region OfCreateDao(ps_comp,ps_TargetTable, ps_TargetColumn, ps_ViewTable) : 建立 DAL 物件 instance
        /// <summary>
        /// 建立 DAL 物件 instance
        /// </summary>
        public virtual bool OfCreateDao(string ps_comp, string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable, bool pGenNonSelectCommand = true)
        {
            string ls_connstr;
            ls_connstr=ConfigurationManager.AppSettings[ps_comp];

            if (ls_connstr == "")
                return false;

            if (m_ap_dao == null)
            {
                m_ap_dao = YR.ERP.DAL.DALFactory.CreateDAO(ls_connstr);
            }

            if (m_ap_dao == null)
                return false;
            if (OfCreateCommand(m_ap_dao, ps_TargetTable, ps_TargetColumn, ps_ViewTable, pGenNonSelectCommand) == false)
                return false;

            return true;
        }
        #endregion

        #region OfCreateDao(ps_TargetTable, ps_TargetColumn, ps_ViewTable) : 建立 DAL 物件 instance
        /// <summary>
        /// 建立 DAL 物件 instance
        /// </summary>
        public virtual bool OfCreateDao(string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable)
        {
            if (m_ap_dao == null)
            {
                m_ap_dao = YR.ERP.DAL.DALFactory.CreateDAO();
            }

            if (m_ap_dao == null)
                return false;
            if (OfCreateCommand(m_ap_dao, ps_TargetTable, ps_TargetColumn, ps_ViewTable) == false)
                return false;

            return true;
        }
        #endregion

        #region OfCreateDao(ps_comp,ps_TargetTable, ps_TargetColumn, ps_ViewTable) : 建立 DAL 物件 instance
        /// <summary>
        /// 建立 DAL 物件 instance
        /// </summary>
        public virtual bool OfCreateDao(System.Data.Common.DbConnection pConnection, string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable, bool pGenNonSelectCommand = true)
        {

            if (m_ap_dao == null)
            {
                m_ap_dao = YR.ERP.DAL.DALFactory.CreateDAO(pConnection);
            }


            if (m_ap_dao == null)
                return false;

            if (OfCreateCommand(m_ap_dao, ps_TargetTable, ps_TargetColumn, ps_ViewTable, pGenNonSelectCommand) == false)
                return false;
            return true;
        }
        #endregion

        #region of_create_command
        private bool OfCreateCommand(ERP_MSSQLDAL pApDao, string ps_TargetTable, string ps_TargetColumn, string ps_ViewTable, bool pGenNonSelectCommand = true)
        {
            if (pApDao == null)
                return false ;

            ps_TargetTable = GlobalFn.isNullRet(ps_TargetTable, "");
            ps_ViewTable = GlobalFn.isNullRet(ps_ViewTable, "");
            ps_TargetColumn = GlobalFn.isNullRet(ps_TargetColumn, "");

            // 如未指定 view table ，則以 target table 為 view table
            if (ps_ViewTable == "")
            { ps_ViewTable = ps_TargetTable; }

            if (ps_TargetColumn == "")
            { ps_TargetColumn = "*"; }

            // 如有指定 CRUD Table, 則以 update table 建構 m_ap_dao

            if (ps_TargetTable.Length > 0 || ps_ViewTable.Length > 0)
                ((YR.DAL.DALBase)this.m_ap_dao).OfCreateCommand(ps_TargetTable, ps_ViewTable, TRAN, pGenNonSelectCommand);

            return true;
        }
        #endregion

        #region OfGetConntion : 取得資料庫連線
        /// <summary>
        /// 取得資料庫連線
        /// </summary>
        /// <returns>DbConnection</returns>
        public System.Data.Common.DbConnection OfGetConntion()
        {
            if(this.AP_DAO == null) return null;

            return this.AP_DAO.OfGetConntion();
        }
        #endregion

        #region OfSelect(string ps_where) : 以傳入的  where string ，查找主表(master view) 資料
        /// <summary>
        /// 以傳入的  where string ，查找主表(master view) 資料 
        /// </summary>
        /// <param name="pWhere">where string</param>
        /// <returns>DataTable</returns>
        public DataTable OfSelect(string pWhere)
        {
            return this.m_ap_dao.OfSelect(TRAN, pWhere);
        }

        public DataTable OfSelect(string pWhere,List<SqlParameter> pParmList)
        {
            return this.m_ap_dao.OfSelect(TRAN, pWhere,pParmList.ToArray());
        }
        #endregion

        #region OfGetFieldValue(ps_CommandText) : 取得輸入 SQL 執行結果的第一筆記錄的第一個欄位值 (object)
        /// <summary>
        /// 取得輸入 SQL 執行結果的第一筆記錄的第一個欄位值
        /// </summary>
        /// <param name="ps_CommandText">SQL 字串</param>
        /// <returns>第一筆記錄的第一個欄位值</returns>
        public object OfGetFieldValue(string ps_CommandText)
        {
            return this.OfGetFieldValue(ps_CommandText, null);
        }
        #endregion

        #region OfGetFieldValue(ps_CommandText) : 取得輸入 SQL 執行結果的第一筆記錄的第一個欄位值 (object)
        /// <summary>
        /// 取得輸入 SQL 執行結果的第一筆記錄的第一個欄位值
        /// </summary>
        /// <param name="ps_CommandText">SQL 字串</param>
        /// <param name="psCommandParameters">SQL 參數 Parameters</param>
        /// <returns>第一筆記錄的第一個欄位值 (object)</returns>
        public object OfGetFieldValue(string ps_CommandText, params SqlParameter[] psCommandParameters)
        {
            return AP_DAO.of_execute_scalar(this.TRAN, ps_CommandText, psCommandParameters);
        }
        #endregion

        #region OfGetFieldValue(DbTransaction ptran, ps_CommandText, psCommandParameters) :  輸入 Table 及欄位， 傳回資料值
        public object OfGetFieldValue(System.Data.Common.DbTransaction ptran, string ps_CommandText, params SqlParameter[] psCommandParameters)
        {
            return AP_DAO.of_execute_scalar(ptran, ps_CommandText, psCommandParameters);
        }
        
        #endregion

        #region OfGetFieldValue(ps_table, ps_column, ps_where) :  輸入 Table 及欄位， 傳回資料值
        /// <summary>
        ///  輸入 Table 及欄位， 傳回資料值
        /// </summary>      
        public object OfGetFieldValue(string ps_table, string ps_column, string ps_where)
        {
            ps_table = GlobalFn.isNullRet(ps_table, "");
            ps_where = GlobalFn.isNullRet(ps_where, "");

            if (ps_table == "") { return null; }

            StringBuilder ls_sql = new StringBuilder();
            ls_sql.AppendFormat(" SELECT TOP 1 {0} FROM {1}  (NOLOCK)  WHERE 1=1 ", ps_column, ps_table);
            if (ps_where != "")
            {
                ls_sql.Append(ps_where);
            }
            return this.OfGetFieldValue(ls_sql.ToString(), null);
        }
        #endregion

        #region OfGetDataRow(ps_cmd, p_params) : 取得傳入 SQL 字串執行後的 DataRow
        /// <summary>
        /// 取得傳入 SQL 字串執行後的 DataRow
        /// </summary>
        /// <param name="ps_cmd">SQL 字串</param>
        /// <param name="p_params">SQL 參數陣列</param>
        /// <returns>DataRow</returns>
        public DataRow OfGetDataRow(string ps_cmd, params object[] p_params)
        {
            return this.OfGetDataRow(TRAN, ps_cmd, p_params);
        }
        #endregion

        #region OfGetDataRow(DbTransaction ptran, ps_cmd, p_params) : 取得傳入 SQL 字串執行後的 DataRow
        /// <summary>
        /// 取得傳入 SQL 字串執行後的 DataRow
        /// </summary>
        /// <param name="ptran">DbTransaction 物件</param>
        /// <param name="ps_cmd">SQL 字串</param>
        /// <param name="p_params">SQL 參數陣列</param>
        /// <returns>DataRow</returns>
        public DataRow OfGetDataRow(System.Data.Common.DbTransaction ptran, string ps_cmd, params object[] p_params)
        {
            DataRow drReturn = AP_DAO.OfExecuteDatarow(ptran, ps_cmd, p_params);

            return drReturn;
        }
        #endregion

        #region OfGetDatatable(ps_sql, params DbParameter[] p_params) : 以傳入的 SQL 字串執行，並回傳 DataTable
        /// <summary>
        /// 以傳入的 SQL 字串執行，並回傳 DataTable
        /// </summary>        
        /// <param name="ps_sql">SQL 字串</param>
        /// <param name="p_params">SQL 參數</param>
        /// <returns>DataTable</returns>
        public DataTable OfGetDataTable(string ps_sql, params DbParameter[] p_params)
        {
            return this.OfGetDataTable(TRAN, ps_sql, p_params);
        }
        #endregion

        #region OfGetDataTable(string ps_sql) : 以傳入的 SQL 字串執行，並回傳 DataTable
        /// <summary>
        /// 以傳入的 SQL 字串執行，並回傳 DataTable
        /// </summary>
        /// <param name="ptran">DB交易管理物件 DbTransaction</param>
        /// <param name="ps_sql">SQL 字串</param>
        /// <param name="p_params">SQL 參數</param>
        /// <returns>DataTable</returns>
        public DataTable OfGetDataTable(string ps_sql)
        {
            return this.m_ap_dao.OfGetDatatableByCmdTxt(this.TRAN, ps_sql, null);
        }
        #endregion

        #region OfGetDataTable(DbTransaction ptran, ps_sql, params DbParameter[] p_params) : 以傳入的 SQL 字串執行，並回傳 DataTable
        /// <summary>
        /// 以傳入的 SQL 字串執行，並回傳 DataTable
        /// </summary>
        /// <param name="ptran">DB交易管理物件 DbTransaction</param>
        /// <param name="ps_sql">SQL 字串</param>
        /// <param name="p_params">SQL 參數</param>
        /// <returns>DataTable</returns>
        public DataTable OfGetDataTable(DbTransaction ptran, string ps_sql, params DbParameter[] p_params)
        {
            return this.m_ap_dao.OfGetDatatableByCmdTxt(ptran, ps_sql, p_params);
        }
        #endregion

        #region OfGetDatatable(CommandType cmdType, string strCmd): 執行傳入的 SQL CommandType, SQL Command 並回傳 DataTable
        /// <summary>
        /// 執行傳入的 SQL CommandType, SQL Command 並回傳 DataTable
        /// </summary>
        /// <param name="cmdType">CommandType</param>
        /// <param name="strCmd">SQL Command String</param>
        /// <returns>DataTable</returns>
        public DataTable OfGetDataTable(CommandType cmdType, string strCmd)
        {
            return this.OfGetDataTable(TRAN, cmdType, strCmd);
        }
        #endregion

        #region OfGetDatatable(DbTransaction ptran, CommandType cmdType, string strCmd): 執行傳入的 SQL CommandType, SQL Command 並回傳 DataTable
        /// <summary>
        /// 執行傳入的 SQL CommandType, SQL Command 並回傳 DataTable
        /// </summary>
        /// <param name="ptran">DB交易管理物件 DbTransaction</param>
        /// <param name="cmdType">CommandType</param>
        /// <param name="strCmd">SQL Command String</param>
        /// <returns>DataTable</returns>
        public DataTable OfGetDataTable(DbTransaction ptran, CommandType cmdType, string strCmd)
        {
            return this.m_ap_dao.OfGetDatatableByCmdTxt(ptran, cmdType, strCmd);
        }
        #endregion

        #region OfGetDataSet(CommandType cmdType, string strCmd): 執行傳入的 SQL CommandType, SQL Command 並回傳 DataSet
        /// <summary>
        /// 執行傳入的 SQL CommandType, SQL Command 並回傳 DataSet
        /// </summary>
        /// <param name="cmdType">CommandType</param>
        /// <param name="strCmd">SQL Command String</param>
        /// <returns>DataSet</returns>
        public DataSet OfGetDataSet(CommandType cmdType, string strCmd)
        {
            return this.OfGetDataSet(TRAN, cmdType, strCmd);
        }
        #endregion

        #region OfGetDataSet(DbTransaction ptran, CommandType cmdType, string strCmd): 執行傳入的 SQL CommandType, SQL Command 並回傳 DataSet
        /// <summary>
        /// 執行傳入的 SQL CommandType, SQL Command 並回傳 DataSet
        /// </summary>
        /// <param name="ptran">DB交易管理物件 DbTransaction</param>
        /// <param name="cmdType">CommandType</param>
        /// <param name="strCmd">SQL Command String</param>
        /// <returns>DataSet</returns>
        public DataSet OfGetDataSet(DbTransaction ptran, CommandType cmdType, string strCmd)
        {
            return this.m_ap_dao.OfGetDatasetByCmdTxt(ptran, cmdType, strCmd);
        }
        #endregion

        #region OfGetDataSet(ps_sql, params DbParameter[] p_params) : 以傳入的 SQL 字串執行，並回傳 DataSet
        /// <summary>
        /// 以傳入的 SQL 字串執行，並回傳 DataSet
        /// </summary>        
        /// <param name="ps_sql">SQL 字串</param>
        /// <param name="p_params">SQL 參數</param>
        /// <returns>DataTable</returns>
        public DataSet OfGetDataSet(string ps_sql, params DbParameter[] p_params)
        {
            return this.OfGetDataSet(TRAN, ps_sql, p_params);
        }
        #endregion

        #region OfGetDataSet(DbTransaction ptran, ps_sql, params DbParameter[] p_params) : 以傳入的 SQL 字串執行，並回傳 DataSet
        /// <summary>
        /// 以傳入的 SQL 字串執行，並回傳 DataSet
        /// </summary>
        /// <param name="ptran">DB交易管理物件 DbTransaction</param>
        /// <param name="ps_sql">SQL 字串</param>
        /// <param name="p_params">SQL 參數</param>
        /// <returns>DataTable</returns>
        public DataSet OfGetDataSet(DbTransaction ptran, string ps_sql, params DbParameter[] p_params)
        {
            return this.m_ap_dao.OfGetDatasetByCmdTxt(ptran, ps_sql, p_params);
        }
        #endregion

        #region OfGetDataTableBySP(string spName, params): 執行傳入的 StoreProcedure 並回傳 DataTable
        /// <summary>
        /// 執行傳入的 StoreProcedure 並回傳 DataTable
        /// </summary>
        /// <param name="spName">StoreProcedure 名稱</param>
        /// <param name="strCmd">StoreProcedure 參數</param>
        /// <returns>DataTable</returns>
        public DataTable OfGetDataTableBySP(string spName, params DbParameter[] p_params)
        {
            return this.OfGetDataTableBySP(TRAN, spName, p_params);
        }
        #endregion

        #region OfGetDataTableBySP(DbTransaction, spName, params) : 執行傳入的 StoreProcedure 並回傳 DataTable
        /// <summary>
        /// 執行傳入的 StoreProcedure 並回傳 DataTable
        /// </summary>
        /// <param name="ptran">DB交易管理物件 DbTransaction</param>
        /// <param name="spName">StoreProcedure 名稱</param>
        /// <param name="strCmd">StoreProcedure 參數</param>
        /// <returns>DataTable</returns>
        public DataTable OfGetDataTableBySP(DbTransaction ptran, string spName, params DbParameter[] p_params)
        {
            return this.m_ap_dao.of_get_datatable_by_sp(ptran, spName, p_params);
        }
        #endregion

        #region OfGetDatatableByName(ps_tbname, params DbParameter[] p_params): 以傳入的 TABLE 名稱做查詢，並以 DataTable 回傳查詢結果
        /// <summary>
        /// 以傳入的 TABLE 名稱做查詢，並以 DataTable 回傳查詢結果
        /// </summary>
        /// <param name="ps_tbname">TABLE 名稱</param>
        /// <param name="p_params">查詢參數</param>
        /// <returns>查詢結果 DataTable</returns>
        public DataTable OfGetDatatableByName(string ps_tbname, params DbParameter[] p_params)
        {
            return this.m_ap_dao.OfGetDatatableByTablename(this.TRAN, ps_tbname, p_params);
        }
        #endregion

        #region OfExecuteNonquery(ps_sql) : 執行傳入的 SQL String
        /// <summary>
        /// 執行傳入的 SQL String
        /// </summary>
        /// <param name="ps_cmd">SQL 字串</param>
        /// <param name="p_params">SQL 參數陣列</param>
        /// <returns>正數: 異動到的筆數  負數: 失敗</returns>
        public int OfExecuteNonquery(string ps_sql)
        {
            return this.AP_DAO.OfExecuteSql(TRAN, ps_sql);
        }
        #endregion

        #region OfExecuteNonquery(ps_cmd, p_params) : 執行傳入的 SQL String
        /// <summary>
        /// 執行傳入的 SQL String
        /// </summary>
        /// <param name="ps_cmd">SQL 字串</param>
        /// <param name="p_params">SQL 參數陣列</param>
        /// <returns>正數: 成功  負數: 失敗</returns>
        public int OfExecuteNonquery(string ps_cmd, params object[] p_params)
        {
            return this.AP_DAO.OfExecuteNonquery(TRAN, ps_cmd, p_params);
        }
        #endregion

        #region OfExecuteNonquery(ps_cmd, pSqlParmList) : 執行傳入的 SQL String
        /// <summary>
        /// 執行傳入的 SQL String
        /// </summary>
        /// <param name="ps_cmd"></param>
        /// <param name="pSqlParmList">SQL 參數陣列</param>
        /// <returns>正數: 成功  負數: 失敗</returns>
        public int OfExecuteNonquery(string ps_cmd,List<SqlParameter> pSqlParmList)
        {
            return this.AP_DAO.OfExecuteNonquery(TRAN, ps_cmd, pSqlParmList.ToArray());
        }
        #endregion

        #region OfExecuteNonquery(DbTransaction ptran, ps_cmd, p_params) : 執行傳入的 SQL String
        /// <summary>
        /// 執行傳入的 SQL String
        /// </summary>
        /// <param name="ptran">DbTransaction 物件</param>
        /// <param name="ps_cmd">SQL 字串</param>
        /// <param name="p_params">SQL 參數陣列</param>
        /// <returns>正數: 成功  負數: 失敗</returns>
        public int OfExecuteNonquery(System.Data.Common.DbTransaction ptran, string ps_cmd, params object[] p_params)
        {
            return this.AP_DAO.OfExecuteNonquery(ptran, ps_cmd, p_params);
        }
        #endregion

        #region  OfUpdate(DataTable dt): Update 傳入的 DataTable
        /// <summary>
        /// Update 傳入的 DataTable
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>負數: 失敗</returns>        
        public int OfUpdate(DataTable dt)
        {
            return this.m_ap_dao.OfUpdateTable(this.TRAN, dt);
        }
        #endregion

        #region  OfUpdate(DataTable dt): Update 傳入的 DataTable
        /// <summary>
        /// Update 傳入的 DataTable
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>負數: 失敗</returns>        
        public int OfUpdate(DataSet p_dataset)
        {
            return this.m_ap_dao.OfUpdateDataset(this.TRAN, p_dataset);
        }
        #endregion

        #region  OfUpdate(DataRow[] pdrs): Update 傳入的 DataTable
        /// <summary>
        /// Update 傳入的 pdrs
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>負數: 失敗</returns>        
        public int OfUpdate(DataRow[] pdrs)
        {
            return this.m_ap_dao.OfUpdateTable(this.TRAN, pdrs);
        }
        #endregion


        #region  OfUpdate(DataRow[] pdrs): Update 傳入的 DataTable
        public int OfUpdate(DbTransaction tran, DataRow[] pdrs)
        {
            return this.m_ap_dao.OfUpdateTable(tran, pdrs);
        }
        #endregion

        #region  OfUpdate(DbTransaction tran, DataTable dt): Update 傳入的 DataTable
        /// <summary>
        /// Update 傳入的 DataTable
        /// </summary>
        /// <param name="tran">DB交易管理物件 DbTransaction</param>
        /// <param name="dt">DataTable</param>
        /// <returns>負數: 失敗</returns>
        public int OfUpdate(DbTransaction tran, DataTable dt)
        {
            return this.m_ap_dao.OfUpdateTable(tran, dt);
        }
        #endregion

        #region OfUpdateByDataRow(ps_tablename, pdr) : 以傳入的 Table 和 Datarow，更新資料庫
        /// <summary>
        /// 以傳入的 Table 和 Datarow，更新資料庫
        /// </summary>
        /// <param name="ps_tablename"></param>
        /// <param name="pdr"></param>
        /// <returns></returns>
        public bool OfUpdateByDataRow(string ps_tablename, DataRow pdr)
        {
            if (pdr == null) return false;
            if (GlobalFn.isNullRet(ps_tablename, "") == "") return false;

            DataTable ldt = pdr.Table;
            string ls_sql = "SELECT * FROM " + ps_tablename + " ";

            using (SqlDataAdapter ladapter = new SqlDataAdapter())
            {

                //ladapter.SelectCommand = new SqlCommand(ls_sql,(SqlConnection)this.AP_DAO.of_get_conntion());
                //20100222 Allen add:transaction
                ladapter.SelectCommand = new SqlCommand(ls_sql, (SqlConnection)this.AP_DAO.OfGetConntion(), (SqlTransaction)TRAN);
                
                SqlCommandBuilder lcommand_builder = new SqlCommandBuilder((SqlDataAdapter)ladapter);
                lcommand_builder.ConflictOption = ConflictOption.OverwriteChanges;
                ladapter.UpdateCommand = lcommand_builder.GetUpdateCommand();
                ladapter.InsertCommand = lcommand_builder.GetInsertCommand();
                ladapter.DeleteCommand = lcommand_builder.GetDeleteCommand();

                if (ladapter.Update(new DataRow[] { pdr }) <= 0)
                { return false; }
            }
            return true;
        }
        #endregion of_update_by_datarow

        #region OfGetDataRowByWhere(ps_tablename, ps_where) : 以傳入的 Table 和條件，回傳第一筆 datarow
        /// <summary>
        ///  以傳入的 Table 和條件，回傳第一筆 datarow
        /// </summary>
        /// <param name="ps_tablename">Table 名稱</param>
        /// <param name="ps_where">where 條件</param>
        /// <returns>DataRow</returns>
        public DataRow OfGetDataRowByWhere(string ps_tablename, string ps_where)
        {
            ps_tablename = GlobalFn.isNullRet(ps_tablename, "");
            ps_where = GlobalFn.isNullRet(ps_where, "");

            if (ps_tablename == "") { return null; }

            StringBuilder ls_sql = new StringBuilder();
            ls_sql.AppendFormat(" SELECT TOP 1 * FROM {0}  (NOLOCK)  WHERE 1=1 ", ps_tablename);
            if (ps_where != "")
            {
                ls_sql.Append(ps_where);
            }
            return this.AP_DAO.OfExecuteDatarow(TRAN, ls_sql.ToString());
        }
        #endregion of_get_datarow_bywhere

        #region OfGetDataRowCollectionByWhere(ps_tablename, ps_where) : 以傳入的 Table 和條件，回傳 DataRowCollection
        /// <summary>
        ///  以傳入的 Table 和條件，回傳符合條件的 DataRow 集合
        /// </summary>
        /// <param name="ps_tablename">Table 名稱</param>
        /// <param name="ps_where">where 條件</param>
        /// <returns>DataRow</returns>
        public DataRowCollection OfGetDataRowCollectionByWhere(string ps_tablename, string ps_where)
        {
            DataTable ldt_temp = null;
            ps_tablename = GlobalFn.isNullRet(ps_tablename, "");
            ps_where = GlobalFn.isNullRet(ps_where, "");

            if (ps_tablename == "") { return null; }

            StringBuilder ls_sql = new StringBuilder();
            ls_sql.AppendFormat(" SELECT * FROM {0}  (NOLOCK)  WHERE 1=1 ", ps_tablename);
            if (ps_where != "")
            {
                ls_sql.Append(ps_where);
            }
            ldt_temp = this.AP_DAO.OfGetDatatableByCmdTxt(TRAN, ls_sql.ToString(), null);

            if (ldt_temp != null)
            {
                return ldt_temp.Rows;
            }
            else
            {
                return null;
            }

        }
        #endregion of_get_datarowcollection_bywhere

        #region OfGetDataRowCollectionByWhereDistinct(ps_tablename, ps_where,ps_group) : 以傳入的 Table 和條件，回傳 Group by DataRowCollection
        /// <summary>
        ///  以傳入的 Table 和條件，回傳符合條件的 DataRow 集合
        /// </summary>
        /// <param name="ps_tablename">Table 名稱</param>
        /// <param name="ps_where">where 條件</param>
        /// <returns>DataRow</returns>
        public DataRowCollection OfGetDataRowCollectionByWhereDistinct(string ps_tablename, string ps_where, string ps_group)
        {
            DataTable ldt_temp = null;
            ps_tablename = GlobalFn.isNullRet(ps_tablename, "");
            ps_where = GlobalFn.isNullRet(ps_where, "");
            ps_group = GlobalFn.isNullRet(ps_group, "");

            if (ps_tablename == "") { return null; }

            StringBuilder ls_sql = new StringBuilder();
            if (ps_group.Length > 0)
            {
                ls_sql.AppendFormat(" SELECT DISTINCT {0} FROM {1} (NOLOCK)  WHERE 1=1 ", ps_group,ps_tablename);
            }
            else
            {
                ls_sql.AppendFormat(" SELECT * FROM {0}  (NOLOCK)  WHERE 1=1 ", ps_tablename);
            }
            if (ps_where != "")
            {
                ls_sql.Append(ps_where);
            }
            ldt_temp = this.AP_DAO.OfGetDatatableByCmdTxt(TRAN, ls_sql.ToString(), null);

            if (ldt_temp != null)
            {
                return ldt_temp.Rows;
            }
            else
            {
                return null;
            }

        }
        #endregion of_get_datarowcollection_bywhere_distinct

        #region OfGetDataCntByWhere(ps_tablename, ps_where) : 以傳入的 Table 和條件，回傳資料筆數
        /// <summary>
        ///  以傳入的 Table 和條件，回傳資料筆數
        /// </summary>
        /// <param name="ps_tablename">Table 名稱</param>
        /// <param name="ps_where">where 條件</param>
        /// <returns>int</returns>
        public int OfGetDataCntByWhere(string ps_tablename, string ps_where)
        {
            ps_tablename = GlobalFn.isNullRet(ps_tablename, "");
            ps_where = GlobalFn.isNullRet(ps_where, "");

            if (ps_tablename == "") { return -1; }

            StringBuilder ls_sql = new StringBuilder();
            ls_sql.AppendFormat(" SELECT COUNT(1) FROM {0} (NOLOCK) WHERE 1=1 ", ps_tablename);
            if (ps_where != "")
            {
                ls_sql.Append(ps_where);
            }
            return GlobalFn.isNullRet(OfGetFieldValue(ls_sql.ToString()), 0);
        }
        #endregion of_get_datacnt_bywhere

        #region OfSelectCount(ptran, string ps_where) : 查詢筆數
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ps_where"></param>
        /// <returns></returns>
        public int OfSelectCount(DbTransaction ptran, string ps_where)
        {
            return this.AP_DAO.of_select_count(ptran, ps_where);
        }
        #endregion

        #region OfSetSelectSql : 設定 AP_DAO 的 SELECT SQL String
        /// <summary>
        /// 設定 AP_DAO 的 SELECT SQL String
        /// </summary>
        /// <param name="ps_sql"></param>
        public void OfSetSelectSql(string ps_sql)
        {
            this.AP_DAO.SelectSQLString = ps_sql;
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
        } 
        #endregion



    }
}
