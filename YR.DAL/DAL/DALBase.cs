using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace YR.DAL
{
    public abstract class DALBase : YR.DAL.IDAL
    {
        #region 屬性
        internal string StrSelectSql = "";
        internal string StrSelectCountSql = "";

        internal string StrGenCommandSql = "";
        internal bool IsReadOnly = false;
        protected internal DB_TYPE m_db_type = DB_TYPE.UNKNOWN;

        protected internal System.Data.Common.DbConnection DBConntion;
        protected internal System.Data.Common.DbDataAdapter DataAdapter;

        public string ConnectionString;

        public bool ReadOnly
        {
            set { this.IsReadOnly = value; }
            get { return this.IsReadOnly; }
        }

        public string SelectSQLString
        {
            get { return this.StrSelectSql; }
            set
            {
                this.StrSelectSql = value;
            }
        }

        public string SelectCountSQLString
        {
            get { return this.StrSelectCountSql; }
            set
            {
                this.StrSelectCountSql = value;
            }
        }

        public string GenCommandSQLString
        {
            get { return this.StrGenCommandSql; }
            set
            {
                this.StrGenCommandSql = value;
            }
        }

        public DB_TYPE DB_TYPE
        {
            set { this.m_db_type = value; }
            get { return this.m_db_type; }
        }
        #endregion     

        #region 方法
        public bool OfSetConnectionString(string pConnectionString)
        {
            this.ConnectionString = pConnectionString;
            return true;
        }

        public virtual bool OfInitialConnection(string pConnectionString)
        {
            this.DBConntion = this.OfCreateConnection(pConnectionString);
            
            if (this.OfOpenDb())
                return true;
            else
                return false;
        }

        public virtual bool OfInitialConnection(System.Data.Common.DbConnection pDbConnection)
        {
            this.DBConntion = pDbConnection;

            if (this.OfOpenDb())
                return true;
            else
                return false;
        }

        public virtual System.Data.Common.DbConnection OfGetConntion()
        {
            return this.DBConntion;
        }


        // abstract function
        public abstract bool OfCreateCommand();
        public abstract bool OfCreateCommand(string pGenCommandSql, string pViewSql);
        public abstract bool OfCreateCommand(string pGenCommandSql, string pViewSql, DbTransaction pTran, bool pGenNonSelectCommand = true);

        public abstract bool OfOpenDb();
        public abstract string OfBuildConnectionString(string pServerName, string pDatabaseName, string pUserid, string pPassword);

        public abstract DataTable OfGetDatatableByTablename(DbTransaction ptran, string pTableName, params object[] pParams);
        public abstract DataTable OfGetDatatableByCmdTxt(DbTransaction ptran, string ps_sql, params object[] pParams);
        public abstract DataTable OfGetDatatableByCmdTxt(DbTransaction ptran, CommandType cmdType, string strCmd);
        public abstract DataSet OfGetDatasetByCmdTxt(DbTransaction ptran, string ps_sql, params object[] pParams);
        public abstract DataSet OfGetDatasetByCmdTxt(DbTransaction ptran, CommandType cmdType, string strCmd);
        public abstract DataTable OfExecuteDatatable(DbTransaction ptran, string pSql);
        public abstract DataTable OfExecuteDatatable(DbTransaction ptran, string pSql, params object[] pParams);
        public abstract DataRow OfExecuteDatarow(DbTransaction ptran, string pCmd, params object[] pParams);

        public abstract DataTable OfGetDatatable(DbTransaction ptran, string pTablename);
        public abstract int OfExecuteDatatable(DbTransaction ptran, string pSql, ref DataTable pDtReturn);

        public abstract DataTable OfSelect(DbTransaction ptran, string pWhere);
        public abstract DataTable OfSelect(DbTransaction ptran, string pWhere, params object[] pParams);

        public abstract int OfExecuteSql(DbTransaction ptran, string pSql);
        public abstract int OfUpdateTable(DbTransaction ptran, DataRow pDrSource);
        public abstract int OfUpdateTable(DbTransaction ptran, DataTable pDtTable);
        public abstract int OfUpdateTable(DbTransaction ptran, DataRow[] pDrSource);   //20121227 Allen add
        public abstract int OfUpdateDataset(DbTransaction ptran, DataSet pDtDataSet);   //20121227 Allen add
        public abstract bool OfChkCommand();

        public abstract int OfExecuteNonquery(DbTransaction ptran, string pCmd, params object[] pParams);

        public abstract System.Data.Common.DbConnection OfCreateConnection(string pConnectionString);
        
        #endregion
    }

    #region DATA_ACCESS_TYPE
    public enum DATA_ACCESS_TYPE : uint
    {
        SELECT = 0,
        INSERT = 1,
        UPDATE = 2,
        DELETE = 3
    } 
    #endregion

    #region DB_TYPE
    public enum DB_TYPE : uint
    {
        UNKNOWN = 0,
        MSSQLServer = 1,
        MSACCESS = 2,
        MYSQL = 3,
        Oracle = 4,
        PostgreSQL = 5,
        OleDB = 6,
        OTHERS = 999
    } 
    #endregion
}
