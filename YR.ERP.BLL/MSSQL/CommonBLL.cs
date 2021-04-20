/* 程式名稱: CommonBLL.cs
   系統代號: 
   作    者: Allen
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using YR.ERP.BLL.Model;
using YR.Util;

namespace YR.ERP.BLL.MSSQL
{
    public class CommonBLL : YR.ERP.BLL.BLLMSSQLBase
    {
        #region 建構子
        public CommonBLL()
            : base()
        {
        }

        public CommonBLL(YR.ERP.DAL.ERP_MSSQLDAL pdao)
            : base(pdao)
        {

        }

        public CommonBLL(string pComp, string pTargetTable, string pTargetColumn, string pViewTable)
            : base(pComp, pTargetTable, pTargetColumn, pViewTable)
        {

        }

        public CommonBLL(string pComp, string pTargetTable, string pTargetColumn, string pViewTable, bool pGenNonSelectCommand = true)
            : base(pComp, pTargetTable, pTargetColumn, pViewTable, pGenNonSelectCommand)
        {

        }

        public CommonBLL(System.Data.Common.DbConnection pConnection, string pTargetTable, string pTargetColumn, string pViewTable)
            : base(pConnection, pTargetTable, pTargetColumn, pViewTable)
        {
            //this.OfCreateDao(pConnection, ps_TargetTable, ps_TargetColumn, ps_ViewTable);
        }

        public CommonBLL(System.Data.Common.DbConnection pConnection, string pTargetTable, string pTargetColumn, string pViewTable, bool pGenNonSelectCommand)
            : base(pConnection, pTargetTable, pTargetColumn, pViewTable, false)
        {
            //this.OfCreateDao(pConnection, ps_TargetTable, ps_TargetColumn, ps_ViewTable);
        }

        public CommonBLL(DbConnection pConnection)
            : base(pConnection)
        {
            this.OfCreateDao(pConnection, "", "", "");
        }
        #endregion

        #region KeyValuePair相關方法
        #region OfGetKVP(strSqlCmd) : 傳入查詢字串,取得泛型物件 (List<KeyValuePair) 清單
        /// <summary>
        /// 傳入查詢字串,取得泛型物件的資料來源
        /// </summary>
        /// <param name="strSqlCmd">傳入查詢字串</param>
        /// <returns>List清單</returns>
        public List<KeyValuePair<string, string>> OfGetKVP(string strSqlCmd)
        {

            try
            {
                return OfGetKVP(strSqlCmd, null, 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetKVP(strSqlCmd, iType) : 傳入查詢字串,取得泛型物件 (List<KeyValuePair) 清單
        /// <summary>
        /// 傳入查詢字串,取得泛型物件的資料來源
        /// </summary>
        /// <param name="pSql">傳入查詢字串</param>
        /// <param name="iType">傳入種類 1.回傳的值會以 key.value  2.直接以value</param>
        /// <returns>List清單</returns>
        public List<KeyValuePair<string, string>> OfGetKVP(string pSql, int iType)
        {
            try
            {
                return OfGetKVP(pSql, null, iType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetKVP(strSqlCmd, sqlParms) : 傳入查詢字串,取得泛型物件 (List<KeyValuePair) 清單
        /// <summary>
        /// 傳入查詢字串,取得泛型物件的資料來源
        /// </summary>
        /// <param name="pSql">傳入查詢字串</param>
        /// <param name="pListSqlParms">查詢參數</param>
        /// <returns>List清單</returns>
        public List<KeyValuePair<string, string>> OfGetKVP(string pSql, List<SqlParameter> pListSqlParms)
        {
            try
            {
                return OfGetKVP(pSql, pListSqlParms, 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetKVP(strSqlCmd, sqlParms) : 傳入查詢字串與sql參數,取得泛型物件 (List<KeyValuePair) 清單
        /// <summary>
        /// 傳入查詢字串與sql參數,取得泛型物件的資料來源
        /// </summary>
        /// <param name="pSql"> 傳入查詢字串</param>
        /// <param name="sqlParms"> 傳入sql參數</param>
        /// <param name="iType">傳入種類 1.回傳的值會以 key.value  2.直接以value</param>
        /// <returns>List清單</returns>
        public List<KeyValuePair<string, string>> OfGetKVP(string pSql, List<SqlParameter> pListSqlParms, int iType)
        {
            SqlDataReader dr = null;
            string Key = "";
            string Value = "";

            try
            {
                if (pSql.ToLower().IndexOf("order") < 0)
                {
                    pSql += " order by 1";
                }
                if (pListSqlParms == null)
                {
                    dr = this.AP_DAO.of_execute_datareader(TRAN, pSql);
                }
                else
                {
                    dr = this.AP_DAO.of_execute_datareader(TRAN, pSql, pListSqlParms.ToArray());
                }

                List<KeyValuePair<string, string>> lstSoruce = new List<KeyValuePair<string, string>>();

                while (dr.Read())
                {
                    Key = GlobalFn.isNullRet(dr[0], "");//先不TRIM掉,不然會導致資料BINDING 對應不到
                    Value = GlobalFn.isNullRet(dr[1], "").Trim();

                    if (iType == 1)
                        lstSoruce.Add(new KeyValuePair<string, string>(Key, Key.Trim() + "." + Value));
                    else
                        lstSoruce.Add(new KeyValuePair<string, string>(Key, Value));
                }
                return lstSoruce;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dr.IsClosed == false)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }
        }
        #endregion

        #endregion

        #region OfGetTaxTypeKVPList 課稅別
        public List<KeyValuePair<string, string>> OfGetTaxTypeKVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.稅內含"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.稅外加"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.零稅"));
                sourceList.Add(new KeyValuePair<string, string>("4", "4.免稅"));
                sourceList.Add(new KeyValuePair<string, string>("9", "9.不計稅"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetInvoWayKVPList 發票聯數
        public List<KeyValuePair<string, string>> OfGetInvoWayKVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("1", "1.二聯式"));
                sourceList.Add(new KeyValuePair<string, string>("2", "2.三聯式"));
                sourceList.Add(new KeyValuePair<string, string>("3", "3.二聯式收銀機發票"));
                sourceList.Add(new KeyValuePair<string, string>("4", "4.三聯式收銀機發票"));
                sourceList.Add(new KeyValuePair<string, string>("5", "5.電子計算機發票"));
                sourceList.Add(new KeyValuePair<string, string>("6", "6.電子發票"));
                sourceList.Add(new KeyValuePair<string, string>("7", "7.免用統一發票"));
                sourceList.Add(new KeyValuePair<string, string>("X", "X.不申報"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region OfChkUniformNumber
        /*
            碼數	0	1	2	3	4	5	6	7		
            統一編號	0	0	2	3	8	7	7	8		
            運算	x1	x2	x1	x2	x1	x2	x4	x1		
            新值1	0	0	2	6	8	14	28	8		每個值的十位數與個位數加總直到只剩個位數
            新值2	0	0	2	6	8	5	10	8		
            新值3	0	+0	+2	+6	+8	+5	+1	+8	=30	每個數值加總後可整除10即代表統編正確
         * */
        /// <summary>
        /// 統一編號檢核
        /// </summary>
        /// <param name="pUniNumber"></param>
        /// <returns></returns>
        public Result OfChkUniformNumber(string pUniNumber)
        {
            Result rtnResult;
            rtnResult = new Result();
            try
            {
                if (pUniNumber.Trim().Length < 8)
                {
                    rtnResult.Message = "長度應為8碼!";
                    return rtnResult;
                }
                //else
                //{
                //    int[] intTmpVal = new int[6];
                //    int intTmpSum = 0;
                //    for (int i = 0; i < 6; i++)
                //    {
                //        //位置在奇數位置的*2，偶數位置*1，位置計算從0開始
                //        if (i % 2 == 1)
                //            intTmpVal[i] = overTen(int.Parse(pUniNumber[i].ToString()) * 2);
                //        else
                //            intTmpVal[i] = overTen(int.Parse(pUniNumber[i].ToString()));

                //        intTmpSum += intTmpVal[i];
                //    }
                //    intTmpSum += overTen(int.Parse(pUniNumber[6].ToString()) * 4); //第6碼*4
                //    intTmpSum += overTen(int.Parse(pUniNumber[7].ToString())); //第7碼*1

                //    if (intTmpSum % 10 != 0) //除以10後餘0表示正確，反之則錯誤
                //    {
                //        rtnResult.Message = "統一編號輸入異常!";
                //        return rtnResult;
                //    }
                //}

                rtnResult.Success = true;
                return rtnResult;
            }
            catch (Exception ex)
            {
                rtnResult.Message = "發生未預期的錯誤！";
                rtnResult.Exception = ex;
                return rtnResult;
            }
        }

        private int overTen(int intVal) //超過10則十位數與個位數相加，直到相加後小於10
        {
            if (intVal >= 10)
                intVal = overTen((intVal / 10) + (intVal % 10));
            return intVal;
        }
        #endregion

        #region OfGetDocConfirmKVPList 常用的確認清單來源
        public List<KeyValuePair<string, string>> OfGetDocConfirmKVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("Y", "Y.已確認"));
                sourceList.Add(new KeyValuePair<string, string>("N", "N.未確認"));
                sourceList.Add(new KeyValuePair<string, string>("X", "X.作廢"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetDocStatusKVPList 常用的單據狀態清單來源
        public List<KeyValuePair<string, string>> OfGetDocStatusKVPList()
        {
            List<KeyValuePair<string, string>> sourceList;
            try
            {
                sourceList = new List<KeyValuePair<string, string>>();
                sourceList.Add(new KeyValuePair<string, string>("0", "0.Open"));
                sourceList.Add(new KeyValuePair<string, string>("1", "1.已核准"));
                sourceList.Add(new KeyValuePair<string, string>("9", "9.結案"));
                sourceList.Add(new KeyValuePair<string, string>("X", "X.作廢"));
                return sourceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OfGetToday
        public DateTime OfGetToday()
        {
            StringBuilder sbSql;
            DateTime today;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT getdate()");
                today = Convert.ToDateTime(OfGetFieldValue(sbSql.ToString()));

                return today.Date;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DateTime OfGetNow()
        {
            StringBuilder sbSql;
            DateTime today;
            try
            {
                sbSql = new StringBuilder();
                sbSql.AppendLine("SELECT getdate()");
                today = Convert.ToDateTime(OfGetFieldValue(sbSql.ToString()));

                return today;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfCombineQuerystring 組合查詢的字串
        /// <summary>
        /// 單檔,batch,report 專用的組合查詢
        /// </summary>
        /// <param name="pSqlParmList"></param>
        /// <param name="pTabMaster"></param>
        /// <returns></returns>
        public string WfCombineQuerystring(TabInfo pTabMaster, out List<SqlParameter> pSqlParmList)
        {
            string rtnQueryString = "";
            List<QueryInfo> queryInfoList;
            QueryInfo queryModel;
            try
            {
                queryInfoList = new List<QueryInfo>();
                foreach (DataColumn dc in pTabMaster.DtSource.Columns)
                {
                    if (GlobalFn.varIsNull(pTabMaster.DtSource.Rows[0][dc.ColumnName]))
                        continue;

                    queryModel = new QueryInfo();
                    queryModel.TableName = pTabMaster.ViewTable;
                    queryModel.ColumnName = dc.ColumnName;
                    queryModel.ColumnType = dc.Prefix.ToString();//改用記在 prefix的型別
                    queryModel.Value = pTabMaster.DtSource.Rows[0][dc.ColumnName];
                    queryInfoList.Add(queryModel);
                }
                rtnQueryString = WfGetQueryString(queryInfoList, out pSqlParmList);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return rtnQueryString;
        }

        /// <summary>
        /// 雙檔專用的組合查詢
        /// </summary>
        /// <param name="pSqlParmList"></param>
        /// <param name="pTabMaster"></param>
        /// <param name="pTabDetailList"></param>
        /// <returns></returns>
        public string WfCombineQuerystring(TabInfo pTabMaster, List<TabDetailInfo> pTabDetailList, out List<SqlParameter> pSqlParmList)
        {
            string rtnQueryString = "";
            List<QueryInfo> queryInfoList;
            QueryInfo queryInfo;
            string tempSring;
            StringBuilder sbSqlDetail;
            QueryInfo queryModel;
            try
            {
                #region 先取單頭資料
                queryInfoList = new List<QueryInfo>();
                pSqlParmList = new List<SqlParameter>();
                foreach (DataColumn dc in pTabMaster.DtSource.Columns)
                {
                    if (GlobalFn.varIsNull(pTabMaster.DtSource.Rows[0][dc.ColumnName]))
                        continue;

                    queryModel = new QueryInfo();
                    queryModel.TableName = pTabMaster.ViewTable;
                    queryModel.ColumnName = dc.ColumnName;
                    queryModel.ColumnType = dc.Prefix.ToString();//改用記在 prefix的型別
                    queryModel.Value = pTabMaster.DtSource.Rows[0][dc.ColumnName];
                    queryInfoList.Add(queryModel);
                }
                var tempSqlParmList = new List<SqlParameter>();
                rtnQueryString = WfGetQueryString(queryInfoList, out tempSqlParmList);
                if (tempSqlParmList != null)
                    pSqlParmList.AddRange(tempSqlParmList);
                #endregion

                #region 再取單身資料
                for (int i = 0; i < pTabDetailList.Count; i++)
                {
                    if (pTabDetailList[i].ViewTable != "" && pTabDetailList[i].DtSource != null && pTabDetailList[i].DtSource.Rows.Count > 0)
                    {
                        queryInfoList = new List<QueryInfo>();
                        foreach (DataColumn dc in pTabDetailList[i].DtSource.Columns)
                        {
                            if (GlobalFn.varIsNull(pTabDetailList[i].DtSource.Rows[0][dc.ColumnName]))
                                continue;

                            queryInfo = new QueryInfo();
                            queryInfo.TableName = pTabDetailList[i].ViewTable;
                            queryInfo.ColumnName = dc.ColumnName;
                            queryInfo.ColumnType = dc.Prefix.ToString();//改用記在 prefix的型別
                            queryInfo.Value = pTabDetailList[i].DtSource.Rows[0][dc.ColumnName];
                            queryInfoList.Add(queryInfo);
                        }
                        //ls_temp = WfGetQueryString(TabDetailList[i].DtSource.Rows[0], queryInfoList, out sqlParmList);
                        var tempDetailSqlParmList = new List<SqlParameter>();
                        tempSring = WfGetQueryString(queryInfoList, out tempDetailSqlParmList);
                        if (tempSring == "")
                            continue;

                        if (tempDetailSqlParmList != null)
                            pSqlParmList.AddRange(tempDetailSqlParmList);

                        if (pTabDetailList[i].RelationParams.Count == 0)
                            throw new Exception("tabdetaillist未設定與主表關聯性!(WfCombineQuerystring)");
                        sbSqlDetail = new StringBuilder();
                        sbSqlDetail.AppendLine(string.Format("AND EXISTS ("));
                        sbSqlDetail.AppendLine(string.Format("SELECT 1 FROM {0}", pTabDetailList[i].ViewTable));
                        sbSqlDetail.AppendLine(string.Format("WHERE 1=1 "));
                        foreach (SqlParameter tempParmeter in pTabDetailList[i].RelationParams)
                        {
                            sbSqlDetail.AppendLine(string.Format("AND {0}.{1} ={2}.{3} ", pTabMaster.ViewTable, tempParmeter.SourceColumn, pTabDetailList[i].ViewTable, tempParmeter.ParameterName));
                        }
                        sbSqlDetail.AppendLine(tempSring);
                        sbSqlDetail.AppendLine(string.Format(")"));
                        rtnQueryString += sbSqlDetail.ToString();

                    }

                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rtnQueryString;
        }
        #endregion

        #region  WfGetQueryString(DataRow pdr, List<QueryInfo> pQueryInfo)
        /// <summary>
        /// 依來源DataRow 組合出 where 字串及 sqlparms 目前型別支援 datetime,decimal,string
        /// </summary>
        /// <param name="pdr"></param>
        /// <param name="pQueryInfoList"></param>
        /// <param name="pSqlParmList"></param>
        /// <returns></returns>
        public string WfGetQueryString(List<QueryInfo> pQueryInfoList, out List<SqlParameter> pSqlParmList)
        {
            StringBuilder sbSqlWhere;
            string strOriginalValue = "";
            string strCondition = "", strNewValue = "";
            try
            {
                pSqlParmList = new List<SqlParameter>();    //傳回sqlparms
                sbSqlWhere = new StringBuilder();           //傳回where字串
                foreach (QueryInfo queryModel in pQueryInfoList)
                {
                    //取得該欄位輸入的值並轉為字串
                    if (queryModel.ColumnType.ToLower() == "datetime")
                    {
                        if (queryModel.Value == DBNull.Value)
                            strOriginalValue = "";
                        else
                        {
                            //已經都轉為字串了所以不用再轉型
                            strOriginalValue = queryModel.Value.ToString();
                        }
                    }
                    else
                        strOriginalValue = GlobalFn.isNullRet(queryModel.Value, "");

                    strCondition = "";
                    strNewValue = "";
                    var tempStingList = new List<string>(); //拆解輸入值 如為=只有一筆 between 為二筆 in時則為多筆
                    if (strOriginalValue == "") //輸入=表示該欄位查詢全部,不做處理
                        continue;
                    #region 取得運算式 及截取後的字串
                    if (queryModel.ColumnType.ToLower() == "datetime")//日期限定用
                    {
                        strCondition = "=";
                        tempStingList.Add(strOriginalValue);
                    }
                    else if (strOriginalValue.StartsWith("<>"))
                    {
                        strCondition = "<>";
                        tempStingList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
                    }
                    else if (strOriginalValue.StartsWith(">="))
                    {
                        strCondition = ">=";
                        tempStingList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
                    }
                    else if (strOriginalValue.StartsWith("<="))
                    {
                        strCondition = "<=";
                        tempStingList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
                    }
                    else if (strOriginalValue.StartsWith("<"))
                    {
                        strCondition = "<";
                        tempStingList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
                    }
                    else if (strOriginalValue.StartsWith(">"))
                    {
                        strCondition = ">";
                        tempStingList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
                    }
                    else if (strOriginalValue.IndexOf('=') >= 0)
                    {
                        strCondition = "=";
                        tempStingList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
                    }
                    else if (strOriginalValue.IndexOf("%") >= 0)
                    {
                        strCondition = "LIKE";
                        tempStingList.Add(strOriginalValue);
                    }
                    else if ((strOriginalValue.IndexOf(":") >= 0))  //遇到日期格式時會有問題
                    {
                        strCondition = "BETWEEN";
                        tempStingList = strOriginalValue.Split(new char[] { ':' }, 2).ToList<string>();
                    }
                    else if ((strOriginalValue.IndexOf("|") >= 0))
                    {
                        strCondition = "IN";
                        tempStingList = strOriginalValue.Split(new char[] { '|' }).ToList<string>();
                    }
                    else
                    {
                        strCondition = "=";
                        tempStingList.Add(strOriginalValue);
                    }
                    #endregion

                    #region 依輸入值依不同的CONDITION以sqlparmater方式重取字串
                    if (strCondition.ToLower() == "between")
                    {
                        strNewValue = string.Format(" @{0} AND @{1}", queryModel.ColumnName + "1", queryModel.ColumnName + "2");
                        pSqlParmList.Add(new SqlParameter(string.Format("@{0}1", queryModel.ColumnName), tempStingList[0]));
                        pSqlParmList.Add(new SqlParameter(string.Format("@{0}2", queryModel.ColumnName), tempStingList[1]));
                    }
                    else if (strCondition.ToLower() == "in")
                    {
                        strNewValue = " (";
                        var i = 0;
                        foreach (string tempString in tempStingList)
                        {
                            i++;
                            if (tempStingList.LastOrDefault<string>() != tempString)
                            {
                                strNewValue += string.Format("@{0},", queryModel.ColumnName + i.ToString());
                            }
                            else
                            {
                                strNewValue += string.Format("@{0}", queryModel.ColumnName + i.ToString());
                            }
                            pSqlParmList.Add(new SqlParameter(string.Format("@{0}", queryModel.ColumnName + i.ToString()), tempString));
                        }
                        strNewValue += ")";
                    }
                    else
                    {
                        strNewValue = string.Format("@{0}", queryModel.ColumnName);
                        pSqlParmList.Add(new SqlParameter(string.Format("@{0}", queryModel.ColumnName), tempStingList[0]));
                    }
                    #endregion

                    #region 依型別將相關變數組合
                    if (queryModel.ColumnType.ToLower() == "string")
                        sbSqlWhere.AppendLine(string.Format(" AND {0}.{1} {2} {3} ", queryModel.TableName, queryModel.ColumnName, strCondition, strNewValue));
                    else if (queryModel.ColumnType.ToLower() == "decimal" ||
                                queryModel.ColumnType.ToLower() == "double" ||
                                queryModel.ColumnType.ToLower().IndexOf("int") >= 0)
                    {
                        sbSqlWhere.AppendLine(string.Format(" AND {0}.{1} {2} {3} ", queryModel.TableName, queryModel.ColumnName, strCondition, strNewValue));
                    }
                    else if (queryModel.ColumnType.ToLower() == "datetime")
                        sbSqlWhere.AppendLine(string.Format(" AND convert(nvarchar(8),{0}.{1},112) {2} {3} ", queryModel.TableName, queryModel.ColumnName, strCondition, strNewValue));

                    else
                        sbSqlWhere.AppendLine();
                    #endregion

                }
                return sbSqlWhere.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
