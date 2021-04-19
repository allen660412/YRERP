/* 程式名稱: BasBLL.cs
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
    public class BasBLL : YR.ERP.BLL.MSSQL.CommonBLL
    {
        #region BasBLL() :　建構子
        public BasBLL()
            : base()
        {
        }

        public BasBLL(YR.ERP.DAL.ERP_MSSQLDAL pdao)
            : base(pdao)
        {
        }


        public BasBLL(string pComp, string pTargetTable, string pTargetColumn, string pViewTable)
            : base(pComp, pTargetTable, pTargetColumn, pViewTable)
        {

        }

        public BasBLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region OfGetAutoNO 取得各類單據,單別取號
        /// <summary>
        /// 取得各類單據,單別取號
        /// </summary>
        /// <param name="pBab01">單別</param>
        /// <param name="pMoudle">模組別</param>
        /// <param name="pDate">取號日期</param>
        /// <param name="pAutoNo">傳回號碼</param>
        /// <param name="pErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public bool OfGetAutoNo(string pBab01, ModuleType pModule, DateTime pDate, out string pAutoNo, out string pErrMsg)
        {
            pAutoNo = "";
            pErrMsg = "";
            baa_tb baaModel;   //共用基本參數
            bab_tb babModel;   //單別設定檔
            azf_tb azfModel = null;   //模組+單別代碼
            AdmBLL boAdm;
            CarBLL boCar;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iMaxNo = 0;
            int iTotLength = 0;     //單別+"-"+單號總長度
            int iDateLength = 0;    //時間格式的長度
            string strDate = "";
            try
            {
                boAdm = new AdmBLL(OfGetConntion());
                boAdm.TRAN = this.TRAN;

                baaModel = OfGetBaaModel();
                //這裡要特別對pBab01 處理,有可能因為前端的錯誤,取號完了又重新傳入所以要重新截字串
                pBab01 = pBab01.Substring(0, Convert.ToInt16(baaModel.baa06));

                switch (pModule)
                {
                    case ModuleType.stp:
                        babModel = OfGetBabModel(pBab01);
                        azfModel = boAdm.OfGetAzfModel(babModel.bab03, babModel.bab04);
                        break;
                    case ModuleType.pur:
                        babModel = OfGetBabModel(pBab01);
                        azfModel = boAdm.OfGetAzfModel(babModel.bab03, babModel.bab04);
                        break;
                    case ModuleType.inv:
                        babModel = OfGetBabModel(pBab01);
                        azfModel = boAdm.OfGetAzfModel(babModel.bab03, babModel.bab04);
                        break;
                    case ModuleType.man:
                        babModel = OfGetBabModel(pBab01);
                        azfModel = boAdm.OfGetAzfModel(babModel.bab03, babModel.bab04);
                        break;
                    case ModuleType.gla:
                        azfModel = boAdm.OfGetAzfModel(pModule.ToString(), "10");
                        break;
                    case ModuleType.car:
                        boCar = new CarBLL(OfGetConntion());
                        boCar.TRAN = this.TRAN;
                        var cacModel = boCar.OfGetCacModel(pBab01);
                        azfModel = boAdm.OfGetAzfModel(pModule.ToString(), cacModel.cac04);
                        break;
                    default:
                        pErrMsg = "未設定此模組,請檢核!";
                        return false;
                }

                if (azfModel == null)
                {
                    pErrMsg = "未設定單別性質欄位對應(azf_tb),請檢核!";
                    return false;
                }
                if (GlobalFn.varIsNull(azfModel.azf04) || GlobalFn.varIsNull(azfModel.azf05))
                {
                    pErrMsg = "未設定單號資料表或單號欄位(azf_tb),請檢核!";
                    return false;
                }

                iTotLength = GlobalFn.isNullRet(baaModel.baa06, 0) + GlobalFn.isNullRet(baaModel.baa08, 0) + 1;

                sbSql = new StringBuilder();
                sqlParmList = new List<SqlParameter>();
                switch (baaModel.baa07)     //編號方式
                {
                    case "1":       //1.依流水號
                        iDateLength = 0;
                        strDate = "";
                        break;
                    case "2":       //2.依年月
                        iDateLength = 4;        //yyMM
                        strDate = pDate.ToString("yyMM");
                        break;
                    case "3":       //3.依年週
                        iDateLength = 4;        //yyWW
                        System.Globalization.Calendar TW = new System.Globalization.CultureInfo("zh-TW").Calendar;
                        strDate = TW.GetWeekOfYear(pDate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Sunday).ToString();
                        strDate = pDate.ToString("yy") + strDate.PadLeft(2, '0');//左補兩個0
                        break;
                    case "4":       //4.依年月日
                        iDateLength = 6;        //yyMMdd
                        strDate = pDate.ToString("yyMMdd");
                        break;
                }
                sbSql.AppendLine(string.Format("SELECT MAX(RIGHT({0},@baa08-@iDateLength))", azfModel.azf05));
                sbSql.AppendLine(string.Format("FROM {0}", azfModel.azf04));
                sbSql.AppendLine(string.Format("WHERE SUBSTRING({0},1,@baa06+1+@iDateLength)=@pBab01+'-'+@strDate ", azfModel.azf05));
                sbSql.AppendLine(string.Format("  AND LEN({0})=@iTotLength", azfModel.azf05));

                sqlParmList.Add(new SqlParameter("@baa08", baaModel.baa08));
                sqlParmList.Add(new SqlParameter("@iDateLength", iDateLength));
                sqlParmList.Add(new SqlParameter("@baa06", baaModel.baa06));
                sqlParmList.Add(new SqlParameter("@pBab01", pBab01));
                sqlParmList.Add(new SqlParameter("@iTotLength", iTotLength));
                sqlParmList.Add(new SqlParameter("@strDate", strDate));
                iMaxNo = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0) + 1;
                pAutoNo = pBab01 + "-" + strDate + iMaxNo.ToString().PadLeft(GlobalFn.isNullRet(baaModel.baa08, 0) - iDateLength, '0');
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetbaa 共用基本參數
        public DataRow OfGetBaaDr()
        {
            DataRow drBaa = null;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM baa_tb");
                drBaa = OfGetDataRow(sbSql.ToString(), null);
                return drBaa;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public baa_tb OfGetBaaModel()
        {
            DataRow drBaa = null;
            baa_tb rtnModel = null;
            try
            {
                drBaa = OfGetBaaDr();
                if (drBaa == null)
                    return null;
                rtnModel = drBaa.ToItem<baa_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBaa01KVPList baa01日期格式
        public List<KeyValuePair<string, string>> OfGetBaa01KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.yy/MM/dd"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.yyy/MM/dd"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.yyyy/MM/dd"));
                sourceList.Add(new KeyValuePair<string, string>("4", "4.MM/dd/yy"));
                sourceList.Add(new KeyValuePair<string, string>("5", "5.dd/MM/yy"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBaa06 取得單別設定位數
        public int OfGetBaa06()
        {
            int rtnValue = 0;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT baa06 FROM baa_tb");

                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), null), 0);

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBaa06KVPList baa06.單別設定位數
        public List<KeyValuePair<string, string>> OfGetBaa06KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("3", "3.三碼"));
                sourceList.Add(new KeyValuePair<string, string>("4", "4.四碼"));
                sourceList.Add(new KeyValuePair<string, string>("5", "5.五碼"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBaa07KVPList baa07.自動編號方式
        public List<KeyValuePair<string, string>> OfGetBaa07KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.依流水號"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.依年月"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.依年週"));
                sourceList.Add(new KeyValuePair<string, string>("4", "4.依年月日"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBaa08KVPList baa08.單號設定位數
        public List<KeyValuePair<string, string>> OfGetBaa08KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("8", "8.八碼"));
                sourceList.Add(new KeyValuePair<string, string>("9", "9.九碼"));
                sourceList.Add(new KeyValuePair<string, string>("10", "10.十碼"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfChkBaa02Baa03 檢查日期是否在現行年月及關帳年月間
        public Result OfChkBaa02Baa03(DateTime pCheckDate, baa_tb pBaaModel)
        {
            Result result;
            result = new Result(false);
            try
            {
                var checkYm = pCheckDate.ToString("yyyyMM");
                if (Convert.ToInt32(checkYm) > Convert.ToInt32(pBaaModel.baa02))
                {
                    result.Message = "輸入日期，不可大於庫存現行年月!";
                    return result;
                }

                if (Convert.ToInt32(checkYm) <= Convert.ToInt32(pBaaModel.baa03))
                {
                    result.Message = "已關帳，不可執行此作業!";
                    //result.Message = "輸入日期，不可小於關帳日期";
                    return result;
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.Message = "發生未預期錯誤，請聯絡資訊人員！";
            }
            return result;
        }
        #endregion

        #region OfGetBab 單別資料維護
        public DataRow OfGetBabDr(string pBab01, int pBab01Length)
        {
            DataRow drBab = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM bab_tb");
                sbSql.AppendLine("WHERE bab01=SUBSTRING(@bab01,1,@length)");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@bab01", pBab01), new SqlParameter("@length", pBab01Length) };
                drBab = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drBab;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataRow OfGetBabDr(string pBab01)
        {
            DataRow drBab = null;
            int baa06 = 0;
            try
            {
                baa06 = OfGetBaa06();
                drBab = OfGetBabDr(pBab01, baa06);
                return drBab;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 傳入單別,自動會以baa06的單別碼數截取資料
        /// </summary>
        /// <param name="pBab01"></param>
        /// <returns></returns>
        public bab_tb OfGetBabModel(string pBab01)
        {
            DataRow drBab = null;
            bab_tb rtnModel = null;
            try
            {
                drBab = OfGetBabDr(pBab01);
                if (drBab == null)
                    return null;
                rtnModel = drBab.ToItem<bab_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bab_tb OfGetBabModel(string pBab01, int pBab01Length)
        {
            DataRow drBab = null;
            bab_tb rtnModel = null;
            try
            {
                drBab = OfGetBabDr(pBab01, pBab01Length);
                if (drBab == null)
                    return null;
                rtnModel = drBab.ToItem<bab_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBea 公司基本資料
        public DataRow OfGetBeaDr()
        {
            DataRow drBab = null;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT TOP 1 * FROM bea_tb");
                drBab = OfGetDataRow(sbSql.ToString(), null);
                return drBab;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bea_tb OfGetBeaModel()
        {
            DataRow drBea = null;
            bea_tb rtnModel = null;
            try
            {
                drBea = OfGetBeaDr();
                if (drBea == null)
                    return null;
                rtnModel = drBea.ToItem<bea_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBea01 取得公司全名
        public string OfGetBea01(string pBeacomp)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT bea01 FROM bea_tb");
                sbSql.AppendLine("WHERE beacomp=@beacomp");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@beacomp", pBeacomp));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkBabPK 檢查單別是否存在 多型
        public bool OfChkBabPKValid(string pBab01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bab_tb");
                sbSql.AppendLine("WHERE babvali='Y'");
                sbSql.AppendLine("AND bab01=@bab01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bab01", pBab01));

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

        public bool OfChkBabPKValid(string pBab01, string pBab03, string pBab04)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bab_tb");
                sbSql.AppendLine("WHERE babvali='Y'");
                sbSql.AppendLine("AND bab01=@bab01");
                sbSql.AppendLine("AND bab03=@bab03");
                sbSql.AppendLine("AND bab04=@bab04");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bab01", pBab01));
                sqlParmList.Add(new SqlParameter("@bab03", pBab03));
                sqlParmList.Add(new SqlParameter("@bab04", pBab04));

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

        #region OfGetBab02 取得單別名稱
        public string OfGetBab02(string pBab01)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT bab02 FROM bab_tb");
                sbSql.AppendLine("WHERE bab01=@bab01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bab01", pBab01));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkBeb 檢查部門
        public bool OfChkBebPkExists(string pBeb01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM beb_tb");
                sbSql.AppendLine("WHERE beb01=@beb01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@beb01", pBeb01));

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

        public bool OfChkBebPkValid(string pBeb01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM beb_tb");
                sbSql.AppendLine("WHERE beb01=@beb01");
                sbSql.AppendLine("AND bebvali='Y'");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@beb01", pBeb01));

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

        #region OfGetBeb  DataRow/Model 部門編號
        public DataRow OfGetBebDr(string pBeb01)
        {
            DataRow drReturn;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM beb_tb");
                sbSql.AppendLine("WHERE beb01=@beb01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@beb01", pBeb01));
                drReturn = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());

                return drReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public beb_tb OfGetBebModel(string pBeb01)
        {
            DataRow drBeb;
            try
            {
                drBeb = OfGetBebDr(pBeb01);
                if (drBeb == null)
                    return null;
                else
                    return drBeb.ToItem<beb_tb>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBeb02 取得部門全名
        public string OfGetBeb02(string pBeb01)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT beb02 FROM beb_tb");
                sbSql.AppendLine("WHERE beb01=@beb01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@beb01", pBeb01));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBeb03 取得部門簡稱
        public string OfGetBeb03(string pBeb01)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT beb03 FROM beb_tb");
                sbSql.AppendLine("WHERE beb01=@beb01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@beb01", pBeb01));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkBecPK 檢查員工編號
        public bool OfChkBecPKExists(string pBec01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bec_tb");
                sbSql.AppendLine("WHERE bec01=@bec01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bec01", pBec01));

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

        public bool OfChkBecPKValid(string pBec01)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bec_tb");
                sbSql.AppendLine("WHERE bec01=@bec01");
                sbSql.AppendLine("AND becvali='Y'");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bec01", pBec01));
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

        #region OfGetBec 員工基本資料
        public DataRow OfGetBecDr(string pBec01)
        {
            DataRow drBec = null;
            List<SqlParameter> sqlParmList;
            StringBuilder sbSql;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM bec_tb");
                sbSql.AppendLine("WHERE bec01=@bec01");
                sqlParmList = new List<SqlParameter>() { new SqlParameter("@bec01", pBec01) };
                drBec = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                return drBec;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bec_tb OfGetBecModel(string pBec01)
        {
            DataRow drBec = null;
            bec_tb rtnModel = null;
            try
            {
                drBec = OfGetBecDr(pBec01);
                if (drBec == null)
                    return null;
                rtnModel = drBec.ToItem<bec_tb>();

                return rtnModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBec02 取得員工編號
        public string OfGetBec02(string pBec01)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT bec02 FROM bec_tb");
                sbSql.AppendLine("WHERE bec01=@bec01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bec01", pBec01));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBef01KVPList bef01付款條件分類
        public List<KeyValuePair<string, string>> OfGetBef01KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.廠商"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.客戶"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkBefPK 檢查付款條件編號
        //bef01='1'==> 廠商 '2'==>客戶
        public bool OfChkBefPKExists(string pBef01, string pBef02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bef_tb");
                sbSql.AppendLine("WHERE bef01=@bef01 AND bef02=@bef02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bef01", pBef01));
                sqlParmList.Add(new SqlParameter("@bef02", pBef02));

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

        public bool OfChkBefPKValid(string pBef01, string pBef02)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bef_tb");
                sbSql.AppendLine("WHERE bef01=@bef01 AND bef02=@bef02");
                sbSql.AppendLine("AND befvali='Y'");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bef01", pBef01));
                sqlParmList.Add(new SqlParameter("@bef02", pBef02));

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

        #region OfGetBef03 取得收付款條件說明
        /// <summary>
        /// 收付款條件說明
        /// </summary>
        /// <param name="pBef01">1.廠商 2.客戶</param>
        /// <param name="pBef02">條件編號</param>
        /// <returns></returns>
        public string OfGetBef03(string pBef01, string pBef02)
        {
            string rtnValue = "";
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT bef03 FROM bef_tb");
                sbSql.AppendLine("WHERE bef01=@bef01");
                sbSql.AppendLine("AND bef02=@bef02");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bef01", pBef01));
                sqlParmList.Add(new SqlParameter("@bef02", pBef02));
                rtnValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return rtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkBegPK 檢查金融機構編號
        public bool OfChkBegPKExists(string pBeg01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM beg_tb");
                sbSql.AppendLine("WHERE beg01=@beg01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@beg01", pBeg01));

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

        #region OfChkBejPk 檢查單位
        public bool OfChkBejPkExists(string pBej01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bej_tb");
                sbSql.AppendLine("WHERE bej01=@bej01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bej01", pBej01));

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

        public bool OfChkBejPkValid(string pBej01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bej_tb");
                sbSql.AppendLine("WHERE bej01=@bej01");
                sbSql.AppendLine("AND bejvali='Y'");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bej01", pBej01));

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

        #region OfGetBej02 單位名稱
        public string OfGetBej02(string pBej01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT bej02 FROM bej_tb");
                sbSql.AppendLine("WHERE bej01=@bej01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("bej01", pBej01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBej  DataRow/Model 單位基本資料
        public DataRow OfGetBejDataRow(string pBej01)
        {
            DataRow drReturn;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM bej_tb");
                sbSql.AppendLine("WHERE bej01=@bej01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bej01", pBej01));
                drReturn = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());

                return drReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bej_tb OfGetBejModel(string pBej01)
        {
            DataRow drBej;
            try
            {
                drBej = OfGetBejDataRow(pBej01);
                if (drBej == null)
                    return null;
                else
                    return drBej.ToItem<bej_tb>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBek  DataRow/Model 幣別資料
        public DataRow OfGetBekDataRow(string pBek01)
        {
            DataRow drReturn;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM bek_tb");
                sbSql.AppendLine("WHERE bek01=@bek01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bek01", pBek01));
                drReturn = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());

                return drReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bek_tb OfGetBekModel(string pBek01)
        {
            DataRow drBek;
            try
            {
                drBek = OfGetBekDataRow(pBek01);
                if (drBek == null)
                    return null;
                else
                    return drBek.ToItem<bek_tb>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkBekPK 檢查幣別PK
        public bool OfChkBekPKExists(string pBek01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bek_tb");
                sbSql.AppendLine("WHERE bek01=@bek01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bek01", pBek01));

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

        public bool OfChkBekPKValid(string pBek01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bek_tb");
                sbSql.AppendLine("WHERE bek01=@bek01");
                sbSql.AppendLine("AND bekvali='Y'");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bek01", pBek01));

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

        #region OfGetBel  DataRow/Model 貨運方式
        public DataRow OfGetBelDataRow(string pBel01)
        {
            DataRow drReturn;
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM bel_tb");
                sbSql.AppendLine("WHERE bel01=@bel01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bel01", pBel01));
                drReturn = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());

                return drReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bel_tb OfGetBelModel(string pBel01)
        {
            DataRow drBel;
            try
            {
                drBel = OfGetBelDataRow(pBel01);
                if (drBel == null)
                    return null;
                else
                    return drBel.ToItem<bel_tb>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkBelPKExists 檢查貨運基本方式
        public bool OfChkBelPKExists(string pBel01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bel_tb");
                sbSql.AppendLine("WHERE bel01=@bel01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bel01", pBel01));

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

        #region OfGetBel02 貨運基本資料
        public string OfGetBel02(string pBel01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT bel02 FROM bel_tb");
                sbSql.AppendLine("WHERE bel01=@bel01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("bel01", pBel01));

                var retValue = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), "");

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfChkBgaPK 檢查編碼類別PK
        public bool OfChkBgaPKExists(string pBga01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bga_tb");
                sbSql.AppendLine("WHERE bga01=@bga01");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bga01", pBga01));

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

        public bool OfChkBgaPKValid(string pBga01)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            int iChks = 0;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT COUNT(1) FROM bga_tb");
                sbSql.AppendLine("WHERE bga01=@bga01");
                sbSql.AppendLine("AND bgavali='Y'");

                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bga01", pBga01));

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

        #region OfGetBga03KVPList bga02編碼性質
        public List<KeyValuePair<string, string>> OfGetBga03KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.客戶"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.廠商"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.料號"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBgb05KVPList bgb05編碼段次類別
        public List<KeyValuePair<string, string>> OfGetBgb05KVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.固定值"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.流水號"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.分類碼"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetBga01AutoNo 取得客戶/廠商/料號編號
        public bool OfGetBga01AutoNo(string pBga01, string pBgc03, out string pNo, out string pMsg)
        {
            StringBuilder sbSql;
            List<SqlParameter> sqlParmList;
            DataRow drBga;
            bga_tb bgaModel;
            DataTable dtBgb;
            bgb_tb bgbModel;
            int iMaxNO;
            pNo = "";
            pMsg = "";
            decimal dTotLength, dFirstLength;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM bga_tb WHERE bga01=@bga01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bga01", pBga01));
                drBga = OfGetDataRow(sbSql.ToString(), sqlParmList.ToArray());
                if (drBga == null)
                {
                    pMsg = "無此編碼原則";
                    return false;
                }
                bgaModel = drBga.ToItem<bga_tb>();

                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT * FROM bgb_tb WHERE bgb01=@bgb01");
                sqlParmList = new List<SqlParameter>();
                sqlParmList.Add(new SqlParameter("@bgb01", pBga01));
                dtBgb = OfGetDataTable(sbSql.ToString(), sqlParmList.ToArray());
                if (dtBgb == null || dtBgb.Rows.Count == 0)
                {
                    pMsg = "編碼段數資料未設定";
                    return false;
                }
                foreach (DataRow dr_bgb in dtBgb.Select("", "bgb02"))
                {
                    bgbModel = dr_bgb.ToItem<bgb_tb>();
                    switch (bgbModel.bgb05)//段次類別 1固定 2流水號 3.分類碼
                    {
                        case "1"://固定
                            pNo += bgbModel.bgb06;
                            break;
                        case "2"://流水號
                            sbSql = new StringBuilder();
                            dTotLength = GlobalFn.isNullRet(bgaModel.bga04, 0);
                            dFirstLength = pNo.Length;
                            switch (bgaModel.bga03)//1.客戶 2.廠商 3.料號
                            {
                                case "1"://客戶
                                    sbSql.AppendLine("SELECT MAX(RIGHT(sca01,@dTotLength-@dFirstLength)) FROM sca_tb");
                                    sbSql.AppendLine("WHERE LEFT(sca01,@dFirstLength)=@pNO AND LEN(sca01)=@dTotLength");
                                    sqlParmList = new List<SqlParameter>();
                                    sqlParmList.Add(new SqlParameter("@dTotLength", dTotLength));
                                    sqlParmList.Add(new SqlParameter("@dFirstLength", dFirstLength));
                                    sqlParmList.Add(new SqlParameter("@pNO", pNo));
                                    break;
                                case "2"://廠商
                                    sbSql.AppendLine("SELECT MAX(RIGHT(pca01,@dTotLength-@dFirstLength)) FROM pca_tb");
                                    sbSql.AppendLine("WHERE LEFT(pca01,@dFirstLength)=@pNO AND LEN(pca01)=@dTotLength");
                                    sqlParmList = new List<SqlParameter>();
                                    sqlParmList.Add(new SqlParameter("@dTotLength", dTotLength));
                                    sqlParmList.Add(new SqlParameter("@dFirstLength", dFirstLength));
                                    sqlParmList.Add(new SqlParameter("@pNO", pNo));
                                    break;
                                case "3"://料號
                                    sbSql.AppendLine("SELECT MAX(RIGHT(ica01,@dTotLength-@dFirstLength)) FROM ica_tb");
                                    sbSql.AppendLine("WHERE LEFT(ica01,@dFirstLength)=@pNO AND LEN(ica01)=@dTotLength");
                                    sqlParmList = new List<SqlParameter>();
                                    sqlParmList.Add(new SqlParameter("@dTotLength", dTotLength));
                                    sqlParmList.Add(new SqlParameter("@dFirstLength", dFirstLength));
                                    sqlParmList.Add(new SqlParameter("@pNO", pNo));
                                    break;
                            }

                            iMaxNO = GlobalFn.isNullRet(OfGetFieldValue(sbSql.ToString(), sqlParmList.ToArray()), 0);
                            iMaxNO += 1;
                            pNo += iMaxNO.ToString().PadLeft(Convert.ToInt16(bgbModel.bgb04), '0');
                            break;
                        case "3"://分類碼
                            pNo += pBgc03;
                            break;
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

        //******************************************其他共用Function***************************************************
        #region OfGetUnitRoundQty 取得單位依設定四捨伍入後的數量
        /// <summary>
        /// 取得單位依設定四捨伍入後的數量
        /// </summary>
        /// <param name="pUnit">單位</param>
        /// <param name="pQty">數量</param>
        /// <returns></returns>
        public decimal OfGetUnitRoundQty(string pUnit, decimal pQty)
        {
            decimal rtnQty = 0;
            bej_tb bejModel = null;
            try
            {
                bejModel = OfGetBejModel(pUnit);
                if (bejModel == null)
                    return rtnQty;
                rtnQty = Math.Round(pQty, Convert.ToInt16(bejModel.bej03), MidpointRounding.AwayFromZero);

                return rtnQty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
