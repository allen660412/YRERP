using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;
using YR.Util;
using System.Globalization;
using System.Security.Cryptography;

namespace YR.Util
{
    public static class GlobalFn
    {
        #region 日期時間函式

        #region fmtDate

        /// <summary>
        /// 傳入8碼或14碼日期字串與分隔字元，返回格式化日期字串
        /// (日期字串, 分隔字元, [e: 西元; c: 民國])
        /// </summary>
        /// <param name="date"></param>
        /// <param name="s"></param>
        /// <param name="ce"></param>
        /// <returns></returns>
        public static string fmtDate(string date, string s, string ce)
        {
            string retVal = "";
            try
            {
                if (date.Length < 4) return retVal;

                string yy = date.Substring(0, 4);
                if (ce.ToLower() == "c") yy = (Convert.ToInt16(yy) - 1911).ToString();
                retVal += yy;

                if (date.Length < 6) return retVal;
                string mm = date.Substring(4, 2);
                retVal += s + mm;

                if (date.Length < 8) return retVal;
                string dd = date.Substring(6, 2);
                retVal += s + dd;
            }
            catch { }

            return retVal;
        }
        #endregion

        #region fmtDate(string date, string s)
        /// <summary>
        /// 返回格式化日期字串(預設西元日期)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string fmtDate(string date, string s)
        {
            return fmtDate(date, s, "e");
        }
        #endregion

        #region fmtDate(string date)
        /// <summary>
        /// 返回格式化日期字串(預設西元日期; 預設分隔字元)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string fmtDate(string date)
        {
            return fmtDate(date, "/", "e");
        }
        #endregion

        #region fmtDateTime(string date, string s1, string s2, string ce)
        /// <summary>
        /// 傳入8碼或14碼日期字串與分隔字元，返回格式化日期時間字串(日期字串, , , )
        /// </summary>
        /// <param name="date">日期字串</param>
        /// <param name="s1">分隔字元1</param>
        /// <param name="s2">分隔字元2</param>
        /// <param name="ce">[e: 西元; c: 民國]</param>
        /// <returns></returns>
        public static string fmtDateTime(string date, string s1, string s2, string ce)
        {
            if (date.Length < 14) return fmtDate(date, s1, ce);

            string retVal = "";
            try
            {
                if (date.Length < 4) return retVal;

                string yy = date.Substring(0, 4);
                if (ce.ToLower() == "c") yy = (Convert.ToInt16(yy) - 1911).ToString();
                retVal += yy;

                if (date.Length < 6) return retVal;
                string mm = date.Substring(4, 2);
                retVal += s1 + mm;

                if (date.Length < 8) return retVal;
                string dd = date.Substring(6, 2);
                retVal += s1 + dd;

                if (date.Length < 10) return retVal;
                string hh = date.Substring(8, 2);
                retVal += " " + hh;

                if (date.Length < 12) return retVal;
                string ii = date.Substring(10, 2);
                retVal += s2 + ii;

                if (date.Length < 14) return retVal;
                string ss = date.Substring(12, 2);
                retVal += s2 + ss;
            }
            catch { }

            return retVal;
        }
        #endregion

        #region fmtDateTime(string date, string s1, string s2)
        // 返回格式化日期時間字串(預設西元日期)
        public static string fmtDateTime(string date, string s1, string s2)
        {
            return fmtDateTime(date, s1, s2, "e");
        }
        #endregion

        #region fmtDateTime(string date)
        // 返回格式化日期時間字串(預設西元日期; 預設分隔字元)
        public static string fmtDateTime(string date)
        {
            return fmtDateTime(date, "/", ":", "e");
        }
        #endregion

        #region getServerDate()
        // 返回格式化日期時間字串(預設西元日期; 預設分隔字元)
        public static string getServerDate()
        {
            return System.DateTime.Now.ToString("yyyyMMdd");
        }
        #endregion

        #region getServerDatetime()
        // 返回格式化日期時間字串(預設西元日期; 預設分隔字元)
        public static string getServerDatetime()
        {
            return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        #endregion

        #region getServerTime() 返回格式化時間字串
        public static string getServerTime()
        {
            return System.DateTime.Now.ToString("HHmm");
        }
        #endregion

        #endregion

        #region 變數函式
        #region varIsNull
        /// <summary>
        /// 驗證是否為Null或空值
        /// </summary>
        /// <param name="value">驗證值</param>
        /// <returns>null或空值返回 true</returns>
        public static bool varIsNull(object value)
        {
            try
            {
                if (value == null) return true;
                if (value == DBNull.Value) return true;
                if (value.ToString().Trim().Length == 0) return true;
            }
            catch
            {
                return true;
            }

            return false;
        } 
        #endregion

        #region isNullRet(object value, string retVal)
        /// <summary>
        /// Null或空值時, 返回替代值(string)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="retVal"></param>
        /// <returns></returns>
        public static string isNullRet(object value, string retVal)
        {
            return (varIsNull(value)) ? retVal : value.ToString();
        } 
        #endregion

        #region isNullRet(object value, int retVal)
        /// <summary>
        /// Null或空值時, 返回替代值(int)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="retVal"></param>
        /// <returns></returns>
        public static int isNullRet(object value, int retVal)
        {
            try
            {
                return (varIsNull(value)) ? retVal : Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        } 
        #endregion

        #region isNullRet(object value, decimal retVal)
        /// <summary>
        /// Null或空值時, 返回替代值(decimal)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="retVal"></param>
        /// <returns></returns>
        public static decimal isNullRet(object value, decimal retVal)
        {
            try
            {
                return (varIsNull(value)) ? retVal : Convert.ToDecimal(value);
            }
            catch
            {
                return 0;
            }
        } 
        #endregion

        #region isNullRet(object value, bool retVal)
        /// <summary>
        ///  Null或空值時, 返回替代值(double)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="retVal"></param>
        /// <returns></returns>
        public static bool isNullRet(object value, bool retVal)
        {
            return (varIsNull(value)) ? retVal : Convert.ToBoolean(value);
        } 
        #endregion

        #region isNullRet(object value, DateTime? retVal)
        /// <summary>
        /// Null或空值時, 返回替代值(datetime)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="retVal"></param>
        /// <returns></returns>
        public static DateTime? isNullRet(object value, DateTime? retVal)
        {
            try
            {
                return (varIsNull(value)) ? retVal : Convert.ToDateTime(value);
            }
            catch
            {
                return null;
            }
        } 
        #endregion

        #region isDouble(string str) : 驗證字串是否是數字
        /// <summary>
        /// 驗證字串是否是數字
        /// </summary>
        /// <param name="str">要檢查的字串</param>
        /// <returns>True: 是  false: 否</returns>
        public static bool isDouble(string str)
        {
            try
            {
                Convert.ToDouble(str);
            }
            catch
            {
                return false;
            }

            return true;
        }
        #endregion

        #region isEnglishLettrs 判斷是否為英文字母字元
        public static bool isEnglishLettrs(string str)
        {

            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z]+$");

            return reg1.IsMatch(str);

        } 
        #endregion

        #region isNumberCharachter 判斷是否為數字字元
        public static bool isNumberCharachter(string str)
        {

            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[0-9]+$");

            return reg1.IsMatch(str);

        } 
        #endregion
        #endregion

        #region 字串函數
        #region rmVbCrLf(string str) 移除掉字串中的前後空白與斷行字元
        public static string rmVbCrLf(string str)
        {
            return str.Replace("\r", " ").Replace("\n", " ").Trim();
        } 
        #endregion

        #region sqlEncode(string str) 迴避SQL指令的特殊字元
        public static string sqlEncode(string str)
        {
            return str.Replace("'", "''").Trim();
        } 
        #endregion

        #region ofShowDialog(pbutton_type, ps_text, ps_title) : 訊息顯示
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pbutton_type">"1" 表 YesNo 訊息,  "2" 表示 "OK" 訊息</param>
        /// <param name="ps_text"></param>
        /// <param name="ps_title"></param>
        /// <returns>1 Yes/ok  2.NO</returns>
        public static int ofShowDialog(string pbutton_type, string ps_text, string ps_title)
        {
            // ps_text  語言轉換...
            // ps_title 語言轉換...   

            // pbutton_type == "1" 表 YesNo 訊息,  "2" 表示 "OK" 訊息
            if (pbutton_type == "1")
            {
                DialogResult result = MessageBox.Show(ps_text, ps_title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    return 1;
                else
                    return 2;
            }
            if (pbutton_type == "2")
            {
                YR.Util.Forms.FrmMessage frmMsg = new YR.Util.Forms.FrmMessage(ps_text);
                frmMsg.StartPosition = FormStartPosition.CenterScreen;
                frmMsg.MaximizeBox = false;
                frmMsg.ShowDialog();
                return 1;
            }

            return 1;
        }
        public static int ofShowDialog(string ps_text)
        {
            // ps_text  語言轉換...
            // ps_title 語言轉換...   

            //MessageBox.Show(ps_text, ps_title, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            YR.Util.Forms.FrmMessage frmMsg = new YR.Util.Forms.FrmMessage(ps_text);
            frmMsg.StartPosition = FormStartPosition.CenterScreen;
            frmMsg.MaximizeBox = false;
            frmMsg.TopMost = true;
            frmMsg.ShowDialog();

            //MessageBox.Show(ps_text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

            return 1;
        }
        #endregion

        // 處理數值、金額 Function
        #region 處理數值、金額 Function ==============================================================

        #region of_get_tax_amt(pd_tot_amt, pd_tax_rate, ps_tot_include_tax) : 由傳入金額推算稅額
        /// <summary>
        /// 由總金額推算稅額
        /// </summary>
        /// <param name="pd_amt">金額</param>
        /// <param name="pd_tax_rate">稅率 (如 0.05)</param>
        /// <param name="ps_tot_include_tax">金額否含稅</param>
        /// <returns>稅額 (double) , 失敗傳回負數 -1 </returns>
        public static double of_get_tax_amt(double pd_amt, double pd_tax_rate, string ps_tot_include_tax)
        {
            double ld_tax = 0;
            double ld_calc1 = 0;
            try
            {
                if (ps_tot_include_tax == "Y")
                {
                    // 稅內含                    
                    ld_calc1 = System.Math.Round(pd_amt / (1 + pd_tax_rate), MidpointRounding.AwayFromZero);
                    ld_tax = System.Math.Round(ld_calc1 * pd_tax_rate, MidpointRounding.AwayFromZero);
                }
                else
                {
                    // 稅外加
                    ld_tax = System.Math.Round(pd_amt * pd_tax_rate, MidpointRounding.AwayFromZero);
                }

                return ld_tax;
            }
            catch
            {
                throw new Exception("由傳入金額推算稅額錯誤! (Global_fn.of_get_tax_amt)");
            }
        }
        #endregion

        #region of_get_int(ps_numeric): 將傳入字串轉換為整數
        /// <summary>
        /// 將傳入字串轉換為整數
        /// </summary>
        /// <param name="ps_numeric">傳入字串</param>
        /// <returns>int</returns>
        public static int of_get_int(string ps_numeric)
        {
            int li_return = 0;
            decimal ld_return = 0;

            if (Decimal.TryParse(ps_numeric, out ld_return))
            {
                li_return = Convert.ToInt32(ld_return);
                return li_return;
            }


            if (Int32.TryParse(ps_numeric, out li_return))
            {
                return li_return;
            }

            return li_return;
        }
        #endregion

        #region isNumeric(str): 以傳入字串判斷是否為數值
        /// <summary>
        /// 以傳入字串判斷是否為數值
        /// </summary>
        /// <param name="str">傳入字串</param>
        /// <returns>true:數值  false:非數值</returns>
        public static bool isNumeric(string str)
        {
            try
            {
                double i = double.Parse(str);
            }
            catch (Exception e)
            {
                string error = e.Message;
                return false;
            }
            return true;
        }
        #endregion

        #endregion 處理數值、金額 ==============================================================

        private static DataTable of_SelectDistinct(DataTable SourceTable, params string[] FieldNames)
        {
            object[] lastValues;
            DataTable newTable;
            DataRow[] orderedRows;

            if (FieldNames == null || FieldNames.Length == 0)
                throw new ArgumentNullException("FieldNames");

            lastValues = new object[FieldNames.Length];
            newTable = new DataTable();

            foreach (string fieldName in FieldNames)
                newTable.Columns.Add(fieldName, SourceTable.Columns[fieldName].DataType);

            orderedRows = SourceTable.Select("", string.Join(", ", FieldNames));

            foreach (DataRow row in orderedRows)
            {
                if (!fieldValuesAreEqual(lastValues, row, FieldNames))
                {
                    newTable.Rows.Add(createRowClone(row, newTable.NewRow(), FieldNames));

                    setLastValues(lastValues, row, FieldNames);
                }
            }

            return newTable;
        }
        private static bool fieldValuesAreEqual(object[] lastValues, DataRow currentRow, string[] fieldNames)
        {
            bool areEqual = true;

            for (int i = 0; i < fieldNames.Length; i++)
            {
                if (lastValues[i] == null || !lastValues[i].Equals(currentRow[fieldNames[i]]))
                {
                    areEqual = false;
                    break;
                }
            }

            return areEqual;
        }
        private static DataRow createRowClone(DataRow sourceRow, DataRow newRow, string[] fieldNames)
        {
            foreach (string field in fieldNames)
                newRow[field] = sourceRow[field];

            return newRow;
        }
        private static void setLastValues(object[] lastValues, DataRow sourceRow, string[] fieldNames)
        {
            for (int i = 0; i < fieldNames.Length; i++)
                lastValues[i] = sourceRow[fieldNames[i]];
        }

        #endregion Global_Fn

        #region Round 四捨伍入型
        /// <summary>
        /// 四捨伍入
        /// </summary>
        /// <param name="pValue">傳入數值</param>
        /// <param name="pDigit">小數下幾位</param>
        /// <returns></returns>
        public static decimal Round(decimal pValue, decimal pDigit)
        {
            decimal retValue = 0;
            retValue = Math.Round(pValue, (int)pDigit, MidpointRounding.AwayFromZero);

            return retValue;
        } 
        #endregion

        #region FormatDigit 處理數字,自動去除小數點後面的0 數值??
        public static Single FormatDigit(decimal pNum)
        {
            NumberFormatInfo nfi;
            CultureInfo culture;
            culture = new CultureInfo("zh-TW");
            nfi = NumberFormatInfo.GetInstance(culture);
            return Convert.ToSingle(pNum, nfi);
        } 
        #endregion

        #region genMd5Hash 產生Md5編碼
        public static string genMd5Hash(string password)
        {
            try
            {
                MD5 md5 = MD5.Create();//建立一個MD5
                byte[] source = Encoding.Default.GetBytes(password);//將字串轉為Byte[]
                byte[] crypto = md5.ComputeHash(source);//進行MD5加密
                return Convert.ToBase64String(crypto);//把加密後的字串從Byte[]轉為字串
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
