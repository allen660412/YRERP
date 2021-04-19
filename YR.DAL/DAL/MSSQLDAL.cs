using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Configuration;

namespace YR.DAL
{
    public class MSSQLDAL : DALBase
    {

        #region 屬性
        //private string m_str_database_name = ConfigurationManager.AppSettings["DBName"];
        //private string m_str_pu_user = ConfigurationManager.AppSettings["pu_user"];
        //private string m_str_pu_password = ConfigurationManager.AppSettings["pu_password"];
        //private string m_str_connection = ConfigurationManager.AppSettings["ConnectionSring"];
        public new string ConnectionString
        {
            get { return this.DBConntion.ConnectionString; }
            set
            {
                //if (value.Length == 0)
                //    this.DB_Conntion.ConnectionString = m_str_connection;
                //else
                this.DBConntion.ConnectionString = value;

                this.OfOpenDb();
            }
        }
        #endregion

        #region 建構子
        public MSSQLDAL()
        {
            //this.m_db_type = DB_TYPE.MSSQLServer;
            //if (this.m_str_connection == null) return;

            //this.DB_Conntion = this.of_create_connection(m_str_connection);
            //this.of_open_db();
        }

        public MSSQLDAL(string ps_connection_string)
        {
            this.m_db_type = DB_TYPE.MSSQLServer;
            if (ps_connection_string == null) return;
            this.DBConntion = this.OfCreateConnection(ps_connection_string);
            this.OfOpenDb();
        }

        public MSSQLDAL(System.Data.Common.DbConnection p_db_con)
        {
            this.m_db_type = DB_TYPE.MSSQLServer;
            if (p_db_con == null) return;
            this.DBConntion = p_db_con;
            this.OfOpenDb();
        }

        #endregion

        #region of_create_connection(string ps_con_string)
        public override System.Data.Common.DbConnection OfCreateConnection(string ps_con_string)
        {
            System.Data.SqlClient.SqlConnection ldb_return = null;
            if (ps_con_string.Length > 0)
                ldb_return = new System.Data.SqlClient.SqlConnection(ps_con_string);
            else
                ldb_return = new System.Data.SqlClient.SqlConnection();

            return ldb_return;
        }
        #endregion

        #region of_open_db()
        public override bool OfOpenDb()
        {
            try
            {
                if (DBConntion == null)
                    return false;

                if (DBConntion.State == ConnectionState.Open)
                    return true;

                if (DBConntion.State != ConnectionState.Open)
                {
                    DBConntion.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return false;
        }
        #endregion

        #region bool of_create_command()
        public override bool OfCreateCommand()
        {
            return this.OfCreateCommand("", "", null);
        }

        public override bool OfCreateCommand(string ps_target_table, string ps_view_table)
        {
            return this.OfCreateCommand("", "", null);
        }

        public override bool OfCreateCommand(string ps_target_table, string ps_view_table, DbTransaction pTran,
            bool pGenNonSelectCommand = true
            )
        {
            if (ps_view_table.Trim().Length == 0)
                ps_view_table = ps_target_table;

            if (ps_target_table.Trim().Length > 0)
            {
                this.GenCommandSQLString = "SELECT * FROM " + ps_target_table + " ";
            }

            // 如果不是以 SELECT 開頭，則加上 SELECT 語句
            if (ps_view_table.ToUpper().StartsWith("SELECT") == false && ps_view_table.Trim().Length > 0)
            {
                this.SelectSQLString = "SELECT * FROM " + ps_view_table + " ";
                this.SelectCountSQLString = "SELECT Count(1) FROM " + ps_view_table + " ";
            }

            this.DataAdapter = new SqlDataAdapter();

            // 查詢 Select Command
            if (SelectSQLString.Trim().Length > 0)
            {
                DataAdapter.SelectCommand = new SqlCommand(SelectSQLString, (SqlConnection)DBConntion, (SqlTransaction)pTran);
            }

            // 更新 Command
            if (this.ReadOnly == false && ps_target_table.Trim().Length > 0)
            {
                if (GenCommandSQLString.Length == 0 || GenCommandSQLString.Length == 0)
                    return false;

                SqlCommandBuilder lcommand_builder = new SqlCommandBuilder((SqlDataAdapter)DataAdapter);
                lcommand_builder.ConflictOption = ConflictOption.OverwriteChanges;
                lcommand_builder.RefreshSchema();

                DataAdapter.SelectCommand = new SqlCommand(GenCommandSQLString, (SqlConnection)DBConntion, (SqlTransaction)pTran);
                if (pGenNonSelectCommand)
                {
                    DataAdapter.UpdateCommand = lcommand_builder.GetUpdateCommand();
                    DataAdapter.InsertCommand = lcommand_builder.GetInsertCommand();
                    DataAdapter.DeleteCommand = lcommand_builder.GetDeleteCommand();
                }

                // 用 select sql 再重新產生 SelectCommand
                DataAdapter.SelectCommand = new SqlCommand(SelectSQLString, (SqlConnection)DBConntion, (SqlTransaction)pTran);
            }

            return true;
        }
        #endregion

        #region of_chk_command
        public override bool OfChkCommand()
        {
            if (DataAdapter == null)
                return false;

            if (this.ReadOnly)
            {
                // check Command object
                if (this.DataAdapter.SelectCommand == null)
                {
                    if (GenCommandSQLString.Length == 0)
                        return false;
                    if (this.OfCreateCommand() == false)
                        return false;
                }
            }
            else
            {
                if (this.DataAdapter.DeleteCommand == null || this.DataAdapter.UpdateCommand == null || this.DataAdapter.InsertCommand == null
                    || this.DataAdapter.SelectCommand == null)
                {
                    if (GenCommandSQLString.Length == 0)
                        return false;
                    if (this.OfCreateCommand() == false)
                        return false;
                }
            }
            return true;
        }
        #endregion

        #region of_build_connection_string(string ps_server, string ps_database_name, string ps_userid, string ps_password)
        public override string OfBuildConnectionString(string ps_server, string ps_database_name, string ps_userid, string ps_password)
        {
            // 建立 connection string 
            string ls_connction = String.Format("server={0};database={1};user id={2};password={3};min pool size=4;max pool size=4;", ps_server, ps_database_name, ps_userid, ps_password);
            return ls_connction;
        }
        #endregion

        #region of_get_datatable(DbTransaction ptran, string ps_tablename)
        ///// <summary>
        ///// 取得 DataTable
        ///// </summary>
        ///// <param name="ps_tablename"></param>
        ///// <returns></returns>
        public override DataTable OfGetDatatable(DbTransaction ptran, string ps_tablename)
        {
            string ls_sql = "SELECT * FROM " + ps_tablename;
            return OfExecuteDatatable(ptran, ls_sql);
        }
        #endregion

        #region of_execute_datatable(DbTransaction ptran, string ps_sql)
        /// <summary>
        /// 傳入 SQL String 返回 DataTable 
        /// </summary>
        /// <param name="ps_tablename"></param>
        /// <returns></returns>
        public override DataTable OfExecuteDatatable(DbTransaction ptran, string ps_sql)
        {
            return OfExecuteDatatable(ptran, ps_sql, null);
        }

        public override DataTable OfExecuteDatatable(DbTransaction ptran, string ps_sql, params object[] p_params)
        {
            if (!this.OfOpenDb())
                return null;

            if (!this.OfOpenDb())
                return null;

            SqlParameter[] lparams = null;
            if (p_params != null)
            {
                lparams = new SqlParameter[p_params.Length];
                for (int i = 0; i <= p_params.Length - 1; i++)
                {
                    lparams[i] = (SqlParameter)p_params[i];
                }
            }


            DataSet lds_return = null;
            if (ptran != null && ptran.Connection != null)
            {
                lds_return = YR.DAL.DBUtility.MSSqlHelper.ExecuteDataset((SqlTransaction)ptran, CommandType.Text, ps_sql, lparams);
            }
            else
            {
                lds_return = YR.DAL.DBUtility.MSSqlHelper.ExecuteDataset((SqlConnection)this.DBConntion, CommandType.Text, ps_sql, lparams);
            }


            if (lds_return != null && lds_return.Tables.Count > 0)
            {
                return lds_return.Tables[0];
            }

            return null;
        }
        #endregion

        #region of_execute_datareader(DbTransaction ptran, string ps_sql)
        /// <summary>
        /// 傳入 SQL String 返回 SqlDataReader 
        /// </summary>
        /// <param name="ps_tablename"></param>
        /// <returns></returns>
        public SqlDataReader of_execute_datareader(DbTransaction ptran, string ps_sql)
        {
            if (!this.OfOpenDb())
                return null;

            if (!this.OfOpenDb())
                return null;

            if (ptran != null && ptran.Connection != null)
            { return YR.DAL.DBUtility.MSSqlHelper.ExecuteReader((SqlTransaction)ptran, CommandType.Text, ps_sql); }
            else
            { return YR.DAL.DBUtility.MSSqlHelper.ExecuteReader((SqlConnection)this.DBConntion, CommandType.Text, ps_sql); }
        }
        #endregion

        #region of_execute_datareader(DbTransaction ptran, string ps_sql, params object[] p_params)
        /// <summary>
        /// 傳入 SQL String 返回 SqlDataReader 
        /// </summary>
        /// <param name="ps_tablename"></param>
        /// <returns></returns>
        public SqlDataReader of_execute_datareader(DbTransaction ptran, string ps_sql, params object[] p_params)
        {
            if (!this.OfOpenDb())
                return null;

            if (!this.OfOpenDb())
                return null;

            SqlParameter[] lparams = null;
            if (p_params != null)
            {

                lparams = new SqlParameter[p_params.Length];
                for (int i = 0; i <= p_params.Length - 1; i++)
                {
                    lparams[i] = (SqlParameter)p_params[i];
                }
            }

            if (ptran != null && ptran.Connection != null)
            { return YR.DAL.DBUtility.MSSqlHelper.ExecuteReader((SqlTransaction)ptran, CommandType.Text, ps_sql, lparams); }

            else
            { return YR.DAL.DBUtility.MSSqlHelper.ExecuteReader((SqlConnection)this.DBConntion, CommandType.Text, ps_sql, lparams); }


            //return DBUtility.SqlHelper.ExecuteReader((SqlTransaction)ptran, CommandType.Text, ps_sql, lparams);

        }
        #endregion

        #region of_execute_scalar(System.Data.Common.DbTransaction ptran, ps_sql, params object[] p_params) :  傳入 SQL String 返回 object
        /// <summary>
        /// 傳入 SQL String 返回 object
        /// </summary>
        /// <param name="ps_sql">SQL 字串</param>
        /// <param name="p_params">SQL 參數</param>
        /// <returns>object</returns>
        public object of_execute_scalar(System.Data.Common.DbTransaction ptran, string ps_sql, params object[] p_params)
        {
            if (!this.OfOpenDb())
                return null;

            if (!this.OfOpenDb())
                return null;



            SqlParameter[] lparams = null;
            if (p_params != null)
            {
                lparams = new SqlParameter[p_params.Length];
                for (int i = 0; i <= p_params.Length - 1; i++)
                {
                    lparams[i] = (SqlParameter)p_params[i];
                }
            }

            if (ptran != null && ptran.Connection != null)
            {
                return YR.DAL.DBUtility.MSSqlHelper.ExecuteScalar((SqlTransaction)ptran, CommandType.Text, ps_sql, lparams);
            }
            else
            {
                return YR.DAL.DBUtility.MSSqlHelper.ExecuteScalar((SqlConnection)this.DBConntion, CommandType.Text, ps_sql, lparams);
            }

        }
        #endregion

        #region of_execute_datarow(DbTransaction ptran, ps_cmd, object[] p_params) : 傳入sql字串跟SqlParms 回傳第一列資料
        /// <summary>
        /// 傳入sql字串跟SqlParms 回傳第一列資料
        /// </summary>
        /// <param name="ptran">DBTransaction 物件</param>       
        /// <param name="strCmd">傳入sql字串</param>
        /// <param name="sqlParms">傳入SqlParmeter[]</param>
        /// <returns>datarow</returns>
        public override DataRow OfExecuteDatarow(System.Data.Common.DbTransaction ptran, string ps_cmd, params object[] p_params)
        {
            using (DataTable dt = this.OfGetDatatableByCmdTxt(ptran, ps_cmd, p_params))
            {
                if (dt != null && dt.Rows.Count > 0)
                    return dt.Rows[0];
            }

            return null;
        }
        #endregion

        #region of_execute_nonquery(DbTransaction ptran, ps_cmd, p_params) : 執行傳入的 SQL 語句
        /// <summary>
        /// 執行傳入的 SQL 語句
        /// </summary>
        /// <param name="ptran">DBTransaction 物件</param>
        /// <param name="ps_cmd">SQL 語句</param>
        /// <param name="p_params">SQL 參數條件</param>
        /// <returns> 正數: 執行成功  負數:執行失敗</returns>
        public override int OfExecuteNonquery(DbTransaction ptran, string ps_cmd, params object[] p_params)
        {
            SqlParameter[] lparams = null;

            if (p_params != null)
            {
                lparams = new SqlParameter[p_params.Length];
                for (int i = 0; i <= p_params.Length - 1; i++)
                {
                    lparams[i] = (SqlParameter)p_params[i];
                }

            }

            if (ptran != null && ptran.Connection != null)
            { return YR.DAL.DBUtility.MSSqlHelper.ExecuteNonQuery((SqlTransaction)ptran, CommandType.Text, ps_cmd, lparams); }
            else
            { return YR.DAL.DBUtility.MSSqlHelper.ExecuteNonQuery((SqlConnection)this.DBConntion, CommandType.Text, ps_cmd, lparams); }

        }
        #endregion

        #region of_execute_sql(DbTransaction ptran, ps_sql) : 執行傳入的 SQL 語句
        /// <summary>
        /// 執行傳入的 SQL 語句
        /// </summary>
        /// <param name="ptran">DBTransaction 物件</param>
        /// <param name="ps_sql">SQL 語句</param>
        /// <returns> 正數: 執行成功  負數:執行失敗</returns>
        public override int OfExecuteSql(System.Data.Common.DbTransaction ptran, string ps_sql)
        {

            if (!this.OfOpenDb())
                return -1;

            if (!this.OfOpenDb())
                return -1;

            if (ptran != null && ptran.Connection != null)
            {
                return YR.DAL.DBUtility.MSSqlHelper.ExecuteNonQuery((SqlTransaction)ptran, CommandType.Text, ps_sql);
            }
            else
            {
                return YR.DAL.DBUtility.MSSqlHelper.ExecuteNonQuery((SqlConnection)this.DBConntion, CommandType.Text, ps_sql);
            }

        }
        #endregion

        #region of_get_datatable_by_cmdtxt(DbTransaction ptran, ps_sql, object[] p_params) : 以傳入的 SQL 字串取得整個 datatable
        /// <summary>
        /// 以傳入的 SQL 字串取得整個 datatable
        /// </summary>
        /// <param name="ptran">DBTransaction 物件</param>
        /// <param name="ps_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public override DataTable OfGetDatatableByCmdTxt(DbTransaction ptran, string ps_sql, params object[] p_params)
        {
            if (!this.OfOpenDb())
                return null;

            if (!this.OfOpenDb())
                return null;


            SqlParameter[] lparams = null;
            if (p_params != null)
            {
                lparams = new SqlParameter[p_params.Length];
                for (int i = 0; i <= p_params.Length - 1; i++)
                {
                    lparams[i] = (SqlParameter)p_params[i];
                }
            }
            DataSet lds_return = null;

            if (ptran != null && ptran.Connection != null)
            {
                lds_return = YR.DAL.DBUtility.MSSqlHelper.ExecuteDataset((SqlTransaction)ptran, CommandType.Text, ps_sql, lparams);
            }
            else
            {
                lds_return = YR.DAL.DBUtility.MSSqlHelper.ExecuteDataset((SqlConnection)this.DBConntion, CommandType.Text, ps_sql, lparams);
            }

            if (lds_return != null && lds_return.Tables.Count > 0)
            {
                return lds_return.Tables[0];
            }

            return null;
        }
        #endregion

        #region of_get_datatable_by_cmdtxt(DbTransaction ptran, CommandType cmdType, string strCmd) : 以傳入的 SQL Command  取得整個 datatable
        /// <summary>
        /// 以傳入的 SQL Command  取得整個 datatable
        /// </summary>
        /// <param name="ptran">DBTransaction 物件</param>
        /// <param name="cmdType">CommandType</param>
        /// <param name="strCmd">SQL Command String</param>
        /// <returns>DataTable</returns>
        public override DataTable OfGetDatatableByCmdTxt(DbTransaction ptran, CommandType cmdType, string strCmd)
        {
            if (!this.OfOpenDb())
                return null;

            if (!this.OfOpenDb())
                return null;


            DataSet lds_return = null;

            if (ptran != null && ptran.Connection != null)
            {
                lds_return = YR.DAL.DBUtility.MSSqlHelper.ExecuteDataset((SqlTransaction)ptran, cmdType, strCmd);
            }
            else
            {
                lds_return = YR.DAL.DBUtility.MSSqlHelper.ExecuteDataset((SqlConnection)this.DBConntion, cmdType, strCmd);
            }

            if (lds_return != null && lds_return.Tables.Count > 0)
            {
                return lds_return.Tables[0];
            }

            return null;
        }
        #endregion

        #region of_get_dataset_by_cmdtxt(DbTransaction ptran, CommandType cmdType, string strCmd) : 以傳入的 SQL Command  取得整個 datatable
        /// <summary>
        /// 以傳入的 SQL Command  取得整個 datatable
        /// </summary>
        /// <param name="ptran">DBTransaction 物件</param>
        /// <param name="cmdType">CommandType</param>
        /// <param name="strCmd">SQL Command String</param>
        /// <returns>DataTable</returns>
        public override DataSet OfGetDatasetByCmdTxt(DbTransaction ptran, CommandType cmdType, string strCmd)
        {
            if (!this.OfOpenDb())
                return null;

            if (!this.OfOpenDb())
                return null;


            DataSet lds_return = null;

            if (ptran != null && ptran.Connection != null)
            {
                lds_return = YR.DAL.DBUtility.MSSqlHelper.ExecuteDataset((SqlTransaction)ptran, cmdType, strCmd);
            }
            else
            {
                lds_return = YR.DAL.DBUtility.MSSqlHelper.ExecuteDataset((SqlConnection)this.DBConntion, cmdType, strCmd);
            }

            if (lds_return != null && lds_return.Tables.Count > 0)
            {
                return lds_return;
            }

            return null;
        }
        #endregion

        #region of_get_dataset_by_cmdtxt(DbTransaction ptran, ps_sql, object[] p_params) : 以傳入的 SQL 字串取得整個 DataSet
        /// <summary>
        /// 以傳入的 SQL 字串取得整個 DataSet
        /// </summary>
        /// <param name="ptran">DBTransaction 物件</param>
        /// <param name="ps_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public override DataSet OfGetDatasetByCmdTxt(DbTransaction ptran, string ps_sql, params object[] p_params)
        {
            if (!this.OfOpenDb())
                return null;

            if (!this.OfOpenDb())
                return null;


            SqlParameter[] lparams = null;
            if (p_params != null)
            {
                lparams = new SqlParameter[p_params.Length];
                for (int i = 0; i <= p_params.Length - 1; i++)
                {
                    lparams[i] = (SqlParameter)p_params[i];
                }
            }
            DataSet lds_return = null;

            if (ptran != null && ptran.Connection != null)
            {
                lds_return = YR.DAL.DBUtility.MSSqlHelper.ExecuteDataset((SqlTransaction)ptran, CommandType.Text, ps_sql, lparams);
            }
            else
            {
                lds_return = YR.DAL.DBUtility.MSSqlHelper.ExecuteDataset((SqlConnection)this.DBConntion, CommandType.Text, ps_sql, lparams);
            }

            if (lds_return != null && lds_return.Tables.Count > 0)
            {
                return lds_return;
            }

            return null;
        }
        #endregion

        #region of_get_datatable_by_sp(DbTransaction ptran, string spName, object[] p_params) : 以傳入的 Stored Procedure  取得整個 datatable
        /// <summary>
        ///  以傳入的 Stored Procedure  取得整個 datatable
        /// </summary>
        /// <param name="ptran">DBTransaction 物件</param>
        /// <param name="spName"> Stored Procedure  名稱</param>
        /// <param name="p_params">參數</param>
        /// <returns>DataTable</returns>
        public DataTable of_get_datatable_by_sp(DbTransaction ptran, string spName, object[] p_params)
        {
            if (!this.OfOpenDb())
                return null;

            if (!this.OfOpenDb())
                return null;


            DataSet lds_return = null;

            if (ptran != null && ptran.Connection != null)
            {
                lds_return = YR.DAL.DBUtility.MSSqlHelper.ExecuteDataset((SqlTransaction)ptran, spName, p_params);
            }
            else
            {
                lds_return = YR.DAL.DBUtility.MSSqlHelper.ExecuteDataset((SqlConnection)this.DBConntion, spName, p_params);
            }

            if (lds_return != null && lds_return.Tables.Count > 0)
            {
                return lds_return.Tables[0];
            }

            return null;
        }
        #endregion

        #region of_execute_datatable(DbTransaction ptran, string ps_sql, ref DataTable pdt_return) : 傳入 SQL String 返回 DataTable
        /// <summary>
        /// 傳入 SQL String 返回 DataSet 
        /// </summary>
        /// <param name="ps_sql">SQL String</param>
        /// <param name="pdt_return">DataTable</param>
        /// <returns></returns>
        public override int OfExecuteDatatable(DbTransaction ptran, string ps_sql, ref DataTable pdt_return)
        {
            if (!this.OfOpenDb())
                return -1;

            if (!this.OfOpenDb())
                return -1;


            if (pdt_return == null)
                pdt_return = new DataTable();

            SqlConnection lcon = null;
            // OleDbTransaction l_oletran = this.m_oledb_cn.BeginTransaction();
            if (ptran != null && ptran.Connection != null)
            {
                lcon = ((SqlTransaction)ptran).Connection;
            }
            else
            {
                lcon = (SqlConnection)this.DBConntion;
            }
            using (SqlDataAdapter lda = new SqlDataAdapter(ps_sql, lcon))
            {
                if (this.DBConntion.State != ConnectionState.Open)
                    DBConntion.Open();

                pdt_return.Rows.Clear();
                lda.Fill(pdt_return);
                //l_oletran.Commit();
            }


            if (pdt_return != null)
            {
                return pdt_return.Rows.Count;
            }

            return -1;
        }
        #endregion

        #region of_update_table(DbTransaction tran, DataTable pdt_source) : 更新資料表 DataTable
        /// <summary>
        /// 更新資料表
        /// </summary>
        /// <param name="tran">DbTransaction</param>
        /// <param name="pdt_source">要更新的 DataTable</param>
        /// <returns></returns>
        public override int OfUpdateTable(System.Data.Common.DbTransaction tran, DataTable pdt_source)
        {
            if (OfChkCommand() == false)
                return -1;

            this.DataAdapter.UpdateCommand.Transaction = tran;
            this.DataAdapter.InsertCommand.Transaction = tran;
            this.DataAdapter.DeleteCommand.Transaction = tran;
            return this.DataAdapter.Update(pdt_source);
        }
        #endregion

        #region of_update_table(DbTransaction tran, DataTable pdt_source) : 更新資料表 DataTable
        /// <summary>
        /// 更新資料表
        /// </summary>
        /// <param name="tran">DbTransaction</param>
        /// <param name="pdt_source">要更新的 DataTable</param>
        /// <returns></returns>
        public override int OfUpdateDataset(System.Data.Common.DbTransaction tran, DataSet pdt_dataset)
        {
            if (OfChkCommand() == false)
                return -1;

            this.DataAdapter.UpdateCommand.Transaction = tran;
            this.DataAdapter.InsertCommand.Transaction = tran;
            this.DataAdapter.DeleteCommand.Transaction = tran;
            return this.DataAdapter.Update(pdt_dataset);

        }
        #endregion

        #region of_update_table(DbTransaction tran, DataRow pdr_source) : 更新資料表 DataTable
        /// <summary>
        /// 更新 DataRow
        /// </summary>
        /// <param name="tran">DbTransaction</param>
        /// <param name="pdt_source">要更新的 DataRow</param>
        /// <returns></returns>
        public override int OfUpdateTable(System.Data.Common.DbTransaction tran, DataRow pdr_source)
        {
            if (OfChkCommand() == false)
                return -1;

            this.DataAdapter.UpdateCommand.Transaction = tran;
            this.DataAdapter.InsertCommand.Transaction = tran;
            this.DataAdapter.DeleteCommand.Transaction = tran;

            return this.DataAdapter.Update(new DataRow[] { pdr_source });
        }
        #endregion

        #region of_update_table(DbTransaction tran, DataRow[] pdr_sources) : 更新資料表 DataTable
        /// <summary>
        /// 更新 DataRow
        /// </summary>
        /// <param name="tran">DbTransaction</param>
        /// <param name="pdt_source">要更新的 DataRow</param>
        /// <returns></returns>
        public override int OfUpdateTable(System.Data.Common.DbTransaction tran, DataRow[] pdr_sources)
        {

            if (OfChkCommand() == false)
                return -1;

            this.DataAdapter.UpdateCommand.Transaction = tran;
            this.DataAdapter.InsertCommand.Transaction = tran;
            this.DataAdapter.DeleteCommand.Transaction = tran;


            return this.DataAdapter.Update(pdr_sources);
        }
        #endregion

        #region of_get_datatable_by_tablename(ptran, ps_tablename, object[] p_params)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ps_tablename"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public override DataTable OfGetDatatableByTablename(DbTransaction ptran, string ps_tablename, params object[] p_params)
        {

            SqlParameter[] lparams = new SqlParameter[p_params.Length];
            for (int i = 0; i <= p_params.Length - 1; i++)
            {
                lparams[i] = (SqlParameter)p_params[i];
            }

            string ls_sql = "SELECT * FROM " + ps_tablename;
            return OfGetDatatableByCmdTxt(ptran, ls_sql, lparams);

        }
        #endregion

        #region of_select(DbTransaction ptran, string ps_where)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ps_where"></param>
        /// <returns></returns>
        public override DataTable OfSelect(DbTransaction ptran, string ps_where)
        {
            return OfSelect(ptran, ps_where, null);
        }

        public override DataTable OfSelect(DbTransaction ptran, string ps_where, params object[] p_params)
        {

            string ls_sql = "";

            if (SelectSQLString.ToLower().Contains("where") || ps_where.Trim().ToLower().StartsWith("where"))
                ls_sql = this.SelectSQLString + ps_where;
            else
            {

                ls_sql = this.SelectSQLString + " WHERE 1=1 " + ps_where;
            }
            return OfExecuteDatatable(ptran, ls_sql, p_params);
        }
        #endregion

        #region of_select_count(ptran, string ps_where) : 查詢筆數
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ps_where"></param>
        /// <returns></returns>
        public int of_select_count(DbTransaction ptran, string ps_where)
        {
            string ls_sql = "";
            if (SelectSQLString.ToLower().Contains("where") || ps_where.Trim().ToLower().StartsWith("where"))
                ls_sql = this.SelectCountSQLString + ps_where;
            else
                ls_sql = this.SelectCountSQLString + " WHERE " + ps_where;

            int li_return = int.Parse(of_execute_scalar(ptran, ls_sql, null).ToString());
            return li_return;
        }
        #endregion
    }



}

