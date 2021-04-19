using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace YR.DAL
{
    public interface IDAL
    {
        bool OfOpenDb();
        bool OfCreateCommand();
        bool OfCreateCommand(string ps_target_table, string ps_view_table);
        bool OfCreateCommand(string ps_target_table, string ps_view_table, DbTransaction pTran, bool pGenNonSelectCommand = true);

        bool OfChkCommand();
        System.Data.Common.DbConnection OfCreateConnection(string ps_con_string);

        bool OfInitialConnection(string ps_con_string);
        bool OfInitialConnection(System.Data.Common.DbConnection p_db_con);


        string OfBuildConnectionString(string ps_server, string ps_database_name, string ps_userid, string ps_password);

        DataTable OfGetDatatableByTablename(DbTransaction ptran, string ps_tablename, params object[] p_params);
        DataTable OfGetDatatableByCmdTxt(DbTransaction ptran, string ps_sql, params object[] p_params);
        DataTable OfExecuteDatatable(DbTransaction ptran, string ps_sql);
        DataRow OfExecuteDatarow(DbTransaction ptran, string ps_cmd, params object[] p_params);
        DataTable OfSelect(DbTransaction ptran, string ps_where);

        int OfExecuteSql(DbTransaction ptran, string ps_sql);
        int OfExecuteNonquery(DbTransaction ptran, string ps_sql, params object[] p_params);



        /// <summary>
        /// 傳入 SQL String 返回 DataSet 
        /// </summary>
        /// <param name="ps_tablename"></param>
        /// <returns></returns>
        int OfExecuteDatatable(DbTransaction ptran, string ps_sql, ref DataTable pdt_return);

        /// <summary>
        /// 更新資料表
        /// </summary>
        /// <param name="ldr_source"></param>
        /// <returns></returns>
        int OfUpdateTable(DbTransaction ptran, DataTable pdt_source);

        /// <summary>
        /// 更新資料表
        /// </summary>
        /// <param name="ldr_source"></param>
        /// <returns></returns>
        int OfUpdateTable(DbTransaction ptran, DataRow ldr_source);

        bool OfSetConnectionString(string ps_string);

        System.Data.Common.DbConnection OfGetConntion();
    }
}
