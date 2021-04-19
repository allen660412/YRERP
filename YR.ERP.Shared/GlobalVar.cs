using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Configuration;
using YR.DAL;
using System.Globalization;

namespace YR.ERP.Shared
{
    public  class GlobalVar
    {
        public static string SQLCA_SecConSTR = ConfigurationManager.AppSettings["SQLCA_SecConSTR"];
        //public static string SQLCA_SecConSTR = "server=localhost;database=YR;user id=sa;password=1234;Pooling=true;Max Pool Size=100;Enlist=true; Application Name=YRERP";
        public static readonly string m_DALAssemblyName = ConfigurationManager.AppSettings["DALAssemblyName"];
        public static readonly string m_DALClassName = ConfigurationManager.AppSettings["DALClassName"];

        public static readonly string m_DefaultBLLClass = ConfigurationManager.AppSettings["DefaultBLLClass"];

        public static IFormatProvider  FormatProvider = new CultureInfo("zh-TW", true); 

        public static string DefaultBLLClass
        {
            get { return GlobalVar.m_DefaultBLLClass; }
        }

        private static DbConnection _dbAdmConn;
        public static DbConnection DbAdmConn
        {
            get { return GlobalVar._dbAdmConn; }
            set { GlobalVar._dbAdmConn = value; }
        }

        private static IDAL m_adm_dao;
        public static IDAL Adm_DAO
        {
            get { return GlobalVar.m_adm_dao; }
            set { GlobalVar.m_adm_dao = value; }
        }

        #region m_pu_busobj : Adm 共用 BLL
        private static IBLL m_pu_busobj;
        public static IBLL PU_BUSOBJ
        {
            get { return GlobalVar.m_pu_busobj; }
            set { GlobalVar.m_pu_busobj = value; }
        }
        #endregion
    }
}
