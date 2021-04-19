using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Configuration;
using System.Configuration.Assemblies;
using YR.DAL;
using YR.ERP.DAL;
using YR.ERP.Shared;
using System.ComponentModel;

namespace YR.ERP.DAL
{
    public sealed class DALFactory
    {
        #region mssql server 
        #region CreateDAO() 建立 Data Access Object, 由 Global_Var 中的指定的型別建立
        public static YR.ERP.DAL.ERP_MSSQLDAL CreateDAO()
        {

            //都沒傳改為取YR資料庫
            return CreateDAO(GlobalVar.m_DALAssemblyName, GlobalVar.m_DALClassName, "", GlobalVar.SQLCA_SecConSTR);
            //return CreateDAO(Global_Var.m_DALAssemblyName, Global_Var.m_DALClassName, "", Global_Var.SQLCA_ConSTR);
        }
        #endregion
        
        #region CreateDAO(string ps_con_string) 以指定的連接字串建立 Data Access Object
        public static YR.ERP.DAL.ERP_MSSQLDAL CreateDAO(string ps_con_string)
        {
            if (ps_con_string == null || ps_con_string == "")
                ps_con_string = GlobalVar.SQLCA_SecConSTR;
            return CreateDAO(GlobalVar.m_DALAssemblyName, GlobalVar.m_DALClassName, "", ps_con_string);
        }
        #endregion

        #region CreateDAO(string ps_con_string) 以指定的連接connection建立 Data Access Object
        public static YR.ERP.DAL.ERP_MSSQLDAL CreateDAO(System.Data.Common.DbConnection pConnection)
        {
            return CreateDAO(GlobalVar.m_DALAssemblyName, GlobalVar.m_DALClassName, "", pConnection);
        }
        #endregion

        #region static YR.ERP.DAL.ERP_MSSQLDAL CreateDAO(string pAssemblyName, string pDalClass, string pDbType, string pConnectionString)
        public static YR.ERP.DAL.ERP_MSSQLDAL CreateDAO(string pAssemblyName, string pDalClass, string pDbType, string pConnectionString)
        {
            if (pAssemblyName == null || pDalClass == null || pConnectionString == null)
                return null;
            Assembly asm = GetAssembly(pAssemblyName, pDalClass);

            ERP_MSSQLDAL dalReturn = (ERP_MSSQLDAL)asm.CreateInstance(pDalClass);
            if (dalReturn != null)
            {                
                dalReturn.OfInitialConnection(pConnectionString); ;

                int iDbType;
                int.TryParse(pDbType, out iDbType);
                ((YR.DAL.DALBase)dalReturn).DB_TYPE = (DB_TYPE)iDbType;
            }
            return dalReturn;            
        }
        #endregion

        
        #region CreateDAO(string pAssemblyName, string pDalClass, string pDbType, System.Data.Common.DbConnection pConnection)
        public static YR.ERP.DAL.ERP_MSSQLDAL CreateDAO(string pAssemblyName, string pDalClass, string pDbType, System.Data.Common.DbConnection pConnection)
        {
            Assembly asm = GetAssembly(pAssemblyName, pDalClass);

            ERP_MSSQLDAL dalReturn = (ERP_MSSQLDAL)asm.CreateInstance(pDalClass);
            if (dalReturn != null)
            {
                dalReturn.OfInitialConnection(pConnection); ;

                int iDbType;
                int.TryParse(pDbType, out iDbType);
                ((YR.DAL.DALBase)dalReturn).DB_TYPE = (DB_TYPE)iDbType;
            }
            return dalReturn;
        }
        #endregion

        #region GetAssembly
        private static Assembly GetAssembly(string ps_assembly_name, string ps_dal_class)
        {

            if (ps_assembly_name == "" || ps_assembly_name == null) return null;
            if (ps_dal_class == "" || ps_dal_class == null) return null;

            Assembly asm = null;
            Assembly asm1 = Assembly.GetExecutingAssembly();

            if (ps_assembly_name.Trim().Length > 0)
                asm = Assembly.LoadFrom(System.Windows.Forms.Application.StartupPath + "\\" + ps_assembly_name);   //指定組件名稱
            else
                asm = Assembly.GetExecutingAssembly();  //使用執行檔組件

            return asm;
        }
        #endregion

        #endregion  mssql server
    }


}
