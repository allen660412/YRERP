/* 程式名稱: FrmBase.cs
   系統代號: 
   作    者: Allen
   異動記錄:
   Date        PG         Memo
   ---------   ---------  -------------------------
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YR.ERP.BLL.Model;
using YR.Util;
using YR.ERP.Shared;
using Infragistics.Win.UltraWinGrid;
using YR.Util.Controls;
using Infragistics.Win;
using YR.ERP.DAL.YRModel;
using Infragistics.Win.UltraWinEditors;
using System.IO;
using System.Data.SqlClient;
using Infragistics.Win.UltraMessageBox;
using Infragistics.Win.UltraWinTabbedMdi;
using System.Reflection;
using Infragistics.Win.UltraWinToolbars;

namespace YR.ERP.Base.Forms
{
    public partial class FrmBase : Form
    {
        #region Property
        //public MessageInfo MsgInfoReturned { get; set; }      //不同表單之間的物件傳遞-改為PickBase才有的屬性
        public string MainConnStr { get; set; }                 //保存主要公司別的連結字串
        public ado_tb AdoModel { get; set; }                    //取得程式的設定資料檔
        public baa_tb BaaModel { get; set; }                    //取得BoMaster公司別的共用參數代碼
        protected string DateFormat { get; set; }               //設定元件日期格式 yyyy/MM/dd 
        public YR.ERP.BLL.MSSQL.AdmBLL BoSecurity { get; set; } //權限使用的BLL 指向資料庫Y00
        protected bool InFormClosing = false;                   //表單處於關閉中,避免觸發不必要的事件,ex itemchck,rowdecative....
        public string Title { get; set; }                       //表單上方的tilte bar 名稱

        private YR.ERP.Shared.UserInfo _loginInfo;              //使用者登入資訊
        public YR.ERP.Shared.UserInfo LoginInfo
        {
            get
            {
                if (_loginInfo == null)
                    _loginInfo = new YR.ERP.Shared.UserInfo();
                return _loginInfo;
            }
            set { _loginInfo = value; }
        }

        public DateTime Today
        {
            get
            {
                return BoSecurity.OfGetToday();
            }
        }                                //取得ERP目前日期

        public DateTime Now
        {
            get
            {
                return BoSecurity.OfGetNow();
            }
        }                                  //取得ERP目前時間

        protected bool IniSuccess = true;                        //用來驗證 FormLoad時是否成功
        protected System.Windows.Forms.BindingSource BindingMaster = new BindingSource();
        protected object OldValue { get; set; }                 //control enter時 設定舊資料
        protected ErrorProvider errorProvider = new ErrorProvider();

        protected Dictionary<string, string> _actionDic = null;
        public Dictionary<string, string> ActionDic { get { return _actionDic; } }  //取得

        protected Dictionary<string, string> _reportDic = null;
        public Dictionary<string, string> ReportDic { get { return _reportDic; } }  //取得

        #endregion Property

        #region 建構子
        public FrmBase()
        {
            InitializeComponent();
        }
        #endregion

        #region FrmBase_Load
        private void FrmBase_Load(object sender, EventArgs e)
        {
            this.Shown += FrmBase_Shown;
            this.BackColor = GetStyleLibrary.FormBackGRoundColor;
        }

        private void FrmBase_Shown(object sender, EventArgs e)
        {
            if (IniSuccess == false)
                this.Close();
        }
        #endregion

        #region From Message 公用方法
        #region WfCleanBottomMsg 清除所有訊息
        protected void WfCleanBottomMsg()
        {
            WfShowBottomHelpMsg("");
            WfShowBottomStatusMsg("");
        }
        #endregion

        #region WfShowBottomHelpMsg 顯示底部左邊的輔助訊息
        /// <summary>
        /// 欄位的型態說明
        /// </summary>
        /// <param name="pMsg"></param>
        protected void WfShowBottomHelpMsg(string pMsg)
        {
            WfShowBottomMsg("Status1", pMsg);
        }
        #endregion

        #region 顯示底部右邊的輔助訊息
        /// <summary>
        /// 檢核有異常時,出現訊息
        /// </summary>
        /// <param name="pMsg"></param>
        protected void WfShowBottomStatusMsg(string pMsg)
        {
            WfShowBottomMsg("Status2", pMsg);
        }
        #endregion

        #region WfShowBottomMsg(string pKey, string pMsg) 底部狀態訊息顯示方式
        private void WfShowBottomMsg(string pKey, string pMsg)
        {
            try
            {
                if (UsbButtom.Panels.Exists(pKey))
                    UsbButtom.Panels[pKey].Text = pMsg;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region WfShowErrorMsg 顯示錯誤訊息視窗
        protected DialogResult WfShowErrorMsg(string pMsg)
        {
            try
            {
                using (UltraMessageBoxManager uMessageBox = new UltraMessageBoxManager())
                {
                    UltraMessageBoxInfo uMessageInfo = new UltraMessageBoxInfo();
                    uMessageInfo.Text = pMsg;
                    uMessageInfo.Icon = MessageBoxIcon.Error;

                    return uMessageBox.ShowMessageBox(uMessageInfo);
                }

                //using (YR.Util.Forms.FrmMessage msgFrom = new Util.Forms.FrmMessage())
                //{
                //    msgFrom.ps_Msg = pMsg;
                //    msgFrom.StartPosition = FormStartPosition.CenterScreen;
                //    msgFrom.ShowDialog();
                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected System.Windows.Forms.DialogResult WfShowMsg(UltraMessageBoxInfo pUltraMessageInfo)
        {
            DialogResult result;
            try
            {
                using (UltraMessageBoxManager uMessageBox = new UltraMessageBoxManager())
                {
                    result = new DialogResult();
                    pUltraMessageInfo.MinimumWidth = 300;
                    pUltraMessageInfo.MaximumWidth = 500;
                    uMessageBox.UseAppStyling = false;
                    result = uMessageBox.ShowMessageBox(pUltraMessageInfo);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfShowMsg 顯示訊息視窗
        protected DialogResult WfShowConfirmMsg(string pMsg)
        {
            try
            {
                return WfShowConfirmMsg(pMsg, MessageBoxButtons.YesNo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected DialogResult WfShowConfirmMsg(string pMsg, MessageBoxButtons pMessageButtons)
        {
            try
            {
                using (UltraMessageBoxManager uMessageBox = new UltraMessageBoxManager())
                {
                    UltraMessageBoxInfo uMessageInfo = new UltraMessageBoxInfo();
                    uMessageInfo.Text = pMsg;
                    uMessageInfo.Icon = MessageBoxIcon.Asterisk;
                    uMessageInfo.Buttons = pMessageButtons;

                    return uMessageBox.ShowMessageBox(uMessageInfo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //protected int WfShowConfirmMsg(string PsMsg)
        //{
        //    int i;
        //    try
        //    {
        //        i = GlobalFn.ofShowDialog("1", PsMsg, "確認");
        //        return i;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion
        #endregion

        #region WfGetControlName(object sender) : 獲取 windows control 控制項的名稱
        /// <summary>
        /// 獲取 windows control 控制項的名稱 
        /// </summary>
        /// <param name="sender">control 控制項</param>
        /// <returns>控制項的名稱 </returns>
        public static string WfGetControlName(object sender)
        {
            try
            {
                string controlName = "";
                try
                {
                    Control control = (System.Windows.Forms.Control)sender;
                    controlName = control.Name;
                    return controlName;
                }
                catch { }

                if (sender.GetType() == typeof(System.Windows.Forms.ToolStripItem) ||
                    sender.GetType() == typeof(System.Windows.Forms.ToolStripButton) ||
                    sender.GetType() == typeof(System.Windows.Forms.ToolStripDropDownButton) ||
                    sender.GetType() == typeof(System.Windows.Forms.ToolStripMenuItem)
                     )
                {
                    controlName = ((ToolStripItem)sender).Name;
                    return controlName;
                }

                if (sender.GetType() == typeof(Infragistics.Win.Misc.UltraButton))
                {
                    controlName = (sender as System.Windows.Forms.Control).Name;
                    return controlName;
                }

                return controlName;
            }
            catch
            {
                return "";
            }


        }
        #endregion

        #region WfSetFormTitle 設定form的 tiltebar
        protected void WfSetFormTitle(string pPgmid, string pPgName, YR.ERP.Shared.UserInfo pInfo)
        {
            string title;
            try
            {
                title = string.Format("{0}({1})-{2}[{3}] 使用者:{4}", pPgName, pPgmid, pInfo.CompNo, pInfo.CompNameA, pInfo.UserName);
                //因infragistics的 mditab 會將parents及mdichild的text-combine 因此不更新子視窗的 text
                this.Title = title;
                if (this.IsMdiChild && pPgmid.ToLower() != "menu")
                    this.Text = "";
                else
                    Text = title;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region  WfGetQueryString(DataRow pdr, List<QueryInfo> pQueryInfo)  不使用 改搬到 CommonBLL處理
        /// <summary>
        /// 依來源DataRow 組合出 where 字串及 sqlparms
        /// </summary>
        /// <param name="pdr"></param>
        /// <param name="pQueryInfoList"></param>
        /// <param name="pSqlParmList"></param>
        /// <returns></returns>
        //protected string WfGetQueryString(List<QueryInfo> pQueryInfoList, out List<SqlParameter> pSqlParmList)
        //{
        //    StringBuilder sbSqlWhere;
        //    string strOriginalValue = "";
        //    string strCondition = "", strNewValue = "";
        //    try
        //    {
        //        pSqlParmList = new List<SqlParameter>();    //傳回sqlparms
        //        sbSqlWhere = new StringBuilder();           //傳回where字串
        //        foreach (QueryInfo queryModel in pQueryInfoList)
        //        {
        //            //取得該欄位輸入的值並轉為字串
        //            if (queryModel.ColumnType.ToLower() == "datetime")
        //            {
        //                if (queryModel.Value == DBNull.Value)
        //                    strOriginalValue = "";
        //                else
        //                {
        //                    //已經都轉為字串了所以不用再轉型
        //                    strOriginalValue = queryModel.Value.ToString();
        //                }
        //            }
        //            else
        //                strOriginalValue = GlobalFn.isNullRet(queryModel.Value, "");

        //            strCondition = "";
        //            strNewValue = "";
        //            var tempStingList = new List<string>(); //拆解輸入值 如為=只有一筆 between 為二筆 in時則為多筆
        //            if (strOriginalValue == "") //輸入=表示該欄位查詢全部,不做處理
        //                continue;
        //            #region 取得運算式 及截取後的字串
        //            if (queryModel.ColumnType.ToLower() == "datetime")//日期限定用
        //            {
        //                strCondition = "=";
        //                tempStingList.Add(strOriginalValue);
        //            }
        //            else if (strOriginalValue.StartsWith("<>"))
        //            {
        //                strCondition = "<>";
        //                tempStingList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
        //            }
        //            else if (strOriginalValue.StartsWith(">="))
        //            {
        //                strCondition = ">=";
        //                tempStingList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
        //            }
        //            else if (strOriginalValue.StartsWith("<="))
        //            {
        //                strCondition = "<=";
        //                tempStingList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
        //            }
        //            else if (strOriginalValue.StartsWith("<"))
        //            {
        //                strCondition = "<";
        //                tempStingList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
        //            }
        //            else if (strOriginalValue.StartsWith(">"))
        //            {
        //                strCondition = ">";
        //                tempStingList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
        //            }
        //            else if (strOriginalValue.IndexOf('=') >= 0)
        //            {
        //                strCondition = "=";
        //                tempStingList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
        //            }
        //            else if (strOriginalValue.IndexOf("%") >= 0)
        //            {
        //                strCondition = "LIKE";
        //                tempStingList.Add(strOriginalValue);
        //            }
        //            else if ((strOriginalValue.IndexOf(":") >= 0))  //遇到日期格式時會有問題
        //            {
        //                strCondition = "BETWEEN";
        //                tempStingList = strOriginalValue.Split(new char[] { ':' }, 2).ToList<string>();
        //            }
        //            else if ((strOriginalValue.IndexOf("|") >= 0))
        //            {
        //                strCondition = "IN";
        //                tempStingList = strOriginalValue.Split(new char[] { '|' }).ToList<string>();
        //            }
        //            else
        //            {
        //                strCondition = "=";
        //                tempStingList.Add(strOriginalValue);
        //            }
        //            #endregion

        //            #region 依輸入值依不同的CONDITION以sqlparmater方式重取字串
        //            if (strCondition.ToLower() == "between")
        //            {
        //                strNewValue = string.Format(" @{0} AND @{1}", queryModel.ColumnName + "1", queryModel.ColumnName + "2");
        //                pSqlParmList.Add(new SqlParameter(string.Format("@{0}1", queryModel.ColumnName), tempStingList[0]));
        //                pSqlParmList.Add(new SqlParameter(string.Format("@{0}2", queryModel.ColumnName), tempStingList[1]));
        //            }
        //            else if (strCondition.ToLower() == "in")
        //            {
        //                strNewValue = " (";
        //                var i = 0;
        //                foreach (string tempString in tempStingList)
        //                {
        //                    i++;
        //                    if (tempStingList.LastOrDefault<string>() != tempString)
        //                    {
        //                        strNewValue += string.Format("@{0},", queryModel.ColumnName + i.ToString());
        //                    }
        //                    else
        //                    {
        //                        strNewValue += string.Format("@{0}", queryModel.ColumnName + i.ToString());
        //                    }
        //                    pSqlParmList.Add(new SqlParameter(string.Format("@{0}", queryModel.ColumnName + i.ToString()), tempString));
        //                }
        //                strNewValue += ")";
        //            }
        //            else
        //            {
        //                strNewValue = string.Format("@{0}", queryModel.ColumnName);
        //                pSqlParmList.Add(new SqlParameter(string.Format("@{0}", queryModel.ColumnName), tempStingList[0]));
        //            }
        //            #endregion

        //            #region 依型別將相關變數組合
        //            if (queryModel.ColumnType.ToLower() == "string")
        //                sbSqlWhere.AppendLine(string.Format(" AND {0}.{1} {2} {3} ", queryModel.TableName, queryModel.ColumnName, strCondition, strNewValue));
        //            else if (queryModel.ColumnType.ToLower() == "decimal" ||
        //                        queryModel.ColumnType.ToLower() == "double" ||
        //                        queryModel.ColumnType.ToLower().IndexOf("int") >= 0)
        //            {
        //                sbSqlWhere.AppendLine(string.Format(" AND {0}.{1} {2} {3} ", queryModel.TableName, queryModel.ColumnName, strCondition, strNewValue));
        //            }
        //            else if (queryModel.ColumnType.ToLower() == "datetime")
        //                sbSqlWhere.AppendLine(string.Format(" AND convert(nvarchar(8),{0}.{1},112) {2} {3} ", queryModel.TableName, queryModel.ColumnName, strCondition, strNewValue));

        //            else
        //                sbSqlWhere.AppendLine();
        //            #endregion

        //        }
        //        return sbSqlWhere.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //protected string WfGetQueryString(List<QueryInfo> pQueryInfoList, out List<SqlParameter> pSqlParmList)
        //{
        //    StringBuilder sbSqlWhere;
        //    string strOriginalValue = "";
        //    string strCondition = "", strNewValue = "";
        //    try
        //    {
        //        pSqlParmList = new List<SqlParameter>();    //傳回sqlparms
        //        sbSqlWhere = new StringBuilder();           //傳回where字串
        //        foreach (QueryInfo queryModel in pQueryInfoList)
        //        {
        //            //取得該欄位輸入的值並轉為字串
        //            if (queryModel.ColumnType.ToLower() == "datetime")
        //            {
        //                if (queryModel.Value == DBNull.Value)
        //                    strOriginalValue = "";
        //                else
        //                {
        //                    //已經都轉為字串了所以不用再轉型
        //                    strOriginalValue = queryModel.Value.ToString();
        //                }
        //            }
        //            else
        //                strOriginalValue = GlobalFn.isNullRet(queryModel.Value, "");

        //            strCondition = "";
        //            strNewValue = "";
        //            var tempStingList = new List<string>(); //拆解輸入值 如為=只有一筆 between 為二筆 in時則為多筆
        //            if (strOriginalValue == "") //輸入=表示該欄位查詢全部,不做處理
        //                continue;
        //            #region 取得運算式 及截取後的字串
        //            if (queryModel.ColumnType.ToLower() == "datetime")//日期限定用
        //            {
        //                strCondition = "=";
        //                tempStingList.Add(strOriginalValue);
        //            }
        //            else if (strOriginalValue.StartsWith("<>"))
        //            {
        //                strCondition = "<>";
        //                tempStingList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
        //            }
        //            else if (strOriginalValue.StartsWith(">="))
        //            {
        //                strCondition = ">=";
        //                tempStingList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
        //            }
        //            else if (strOriginalValue.StartsWith("<="))
        //            {
        //                strCondition = "<=";
        //                tempStingList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
        //            }
        //            else if (strOriginalValue.StartsWith("<"))
        //            {
        //                strCondition = "<";
        //                tempStingList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
        //            }
        //            else if (strOriginalValue.StartsWith(">"))
        //            {
        //                strCondition = ">";
        //                tempStingList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
        //            }
        //            else if (strOriginalValue.IndexOf('=') >= 0)
        //            {
        //                strCondition = "=";
        //                tempStingList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
        //            }
        //            else if (strOriginalValue.IndexOf("%") >= 0)
        //            {
        //                strCondition = "LIKE";
        //                tempStingList.Add(strOriginalValue);
        //            }
        //            else if ((strOriginalValue.IndexOf(":") >= 0))  //遇到日期格式時會有問題
        //            {
        //                strCondition = "BETWEEN";
        //                tempStingList = strOriginalValue.Split(new char[] { ':' }, 2).ToList<string>();
        //            }
        //            else if ((strOriginalValue.IndexOf("|") >= 0))
        //            {
        //                strCondition = "IN";
        //                tempStingList = strOriginalValue.Split(new char[] { '|' }).ToList<string>();
        //            }
        //            else
        //            {
        //                strCondition = "=";
        //                tempStingList.Add(strOriginalValue);
        //            }
        //            #endregion

        //            if (queryModel.ColumnType.ToLower() == "string")
        //            {
        //                if (strCondition.ToLower() == "between")
        //                {
        //                    strNewValue = string.Format(" @{0} AND @{1}", queryModel.ColumnName + "1", queryModel.ColumnName+"2");
        //                    pSqlParmList.Add(new SqlParameter(string.Format("@{0}1", queryModel.ColumnName), tempStingList[0]));
        //                    pSqlParmList.Add(new SqlParameter(string.Format("@{0}2", queryModel.ColumnName), tempStingList[1]));
        //                }
        //                else if (strCondition.ToLower() == "in")
        //                {
        //                    strNewValue = " (";
        //                    var i = 0;
        //                    foreach (string tempString in tempStingList)
        //                    {
        //                        i++;
        //                        if (tempStingList.LastOrDefault<string>() != tempString)
        //                        {
        //                            strNewValue += string.Format("@{0},", queryModel.ColumnName + i.ToString());
        //                        }
        //                        else
        //                        {
        //                            strNewValue += string.Format("@{0}", queryModel.ColumnName + i.ToString());
        //                        }
        //                        pSqlParmList.Add(new SqlParameter(string.Format("@{0}", queryModel.ColumnName + i.ToString()), tempString));
        //                    }
        //                    strNewValue += ")";
        //                }
        //                else
        //                {
        //                    strNewValue = string.Format("@{0}", queryModel.ColumnName);
        //                    pSqlParmList.Add(new SqlParameter(string.Format("@{0}", queryModel.ColumnName), tempStingList[0]));
        //                }
        //                sbSqlWhere.AppendLine(string.Format(" AND {0}.{1} {2} {3} ", queryModel.TableName, queryModel.ColumnName, strCondition, strNewValue));
        //            }
        //            else if (queryModel.ColumnType.ToLower() == "decimal" ||
        //                        queryModel.ColumnType.ToLower() == "double" ||
        //                        queryModel.ColumnType.ToLower().IndexOf("int") >= 0)
        //            {
        //                if (strCondition.ToLower() == "between")
        //                {
        //                    strNewValue = string.Format(" @{0} AND @{1}", queryModel.ColumnName + "1", queryModel.ColumnName + "2");
        //                    pSqlParmList.Add(new SqlParameter(string.Format("@{0}1", queryModel.ColumnName), tempStingList[0]));
        //                    pSqlParmList.Add(new SqlParameter(string.Format("@{0}2", queryModel.ColumnName), tempStingList[1]));
        //                }
        //                else if (strCondition.ToLower() == "in")
        //                {
        //                    strNewValue = " (";
        //                    var i = 0;
        //                    foreach (string tempString in tempStingList)
        //                    {
        //                        i++;
        //                        if (tempStingList.LastOrDefault<string>() != tempString)
        //                        {
        //                            strNewValue += string.Format("@{0},", queryModel.ColumnName + i.ToString());
        //                        }
        //                        else
        //                        {
        //                            strNewValue += string.Format("@{0}", queryModel.ColumnName + i.ToString());
        //                        }
        //                        pSqlParmList.Add(new SqlParameter(string.Format("@{0}", queryModel.ColumnName + i.ToString()), tempString));
        //                    }
        //                    strNewValue += ")";
        //                }
        //                else
        //                {
        //                    //strNewValue = tempStingList[0];
        //                    strNewValue = string.Format("@{0}", queryModel.ColumnName);
        //                    pSqlParmList.Add(new SqlParameter(string.Format("@{0}", queryModel.ColumnName), tempStingList[0]));
        //                }
        //                sbSqlWhere.AppendLine(string.Format(" AND {0}.{1} {2} {3} ", queryModel.TableName, queryModel.ColumnName, strCondition, strNewValue));
        //            }
        //            else if (queryModel.ColumnType.ToLower() == "datetime")
        //            {
        //                if (strCondition.ToLower() == "between")
        //                {
        //                    strNewValue = string.Format(" @{0} AND @{1}", queryModel.ColumnName + "1", queryModel.ColumnName + "2");
        //                    pSqlParmList.Add(new SqlParameter(string.Format("@{0}1", queryModel.ColumnName), tempStingList[0]));
        //                    pSqlParmList.Add(new SqlParameter(string.Format("@{0}2", queryModel.ColumnName), tempStingList[1]));
        //                    //strNewValue = string.Format(" '{0}' AND '{1}'", tempStingList[0], tempStingList[1]);
        //                }
        //                else if (strCondition.ToLower() == "in")
        //                {
        //                    strNewValue = " (";
        //                    var i = 0;
        //                    foreach (string tempString in tempStingList)
        //                    {
        //                        i++;
        //                        if (tempStingList.LastOrDefault<string>() != tempString)
        //                        {
        //                            strNewValue += string.Format("@{0},", queryModel.ColumnName + i.ToString());
        //                        }
        //                        else
        //                        {
        //                            strNewValue += string.Format("@{0}", queryModel.ColumnName + i.ToString());
        //                        }
        //                        pSqlParmList.Add(new SqlParameter(string.Format("@{0}", queryModel.ColumnName + i.ToString()), tempString));
        //                    }
        //                    strNewValue += ")";
        //                }
        //                else
        //                {
        //                    strNewValue = string.Format("@{0}", queryModel.ColumnName);
        //                    pSqlParmList.Add(new SqlParameter(string.Format("@{0}", queryModel.ColumnName), tempStingList[0]));
        //                }
        //                sbSqlWhere.AppendLine(string.Format(" AND convert(nvarchar(8),{0}.{1},112) {2} {3} ", queryModel.TableName, queryModel.ColumnName, strCondition, strNewValue));
        //            }
        //            else
        //                sbSqlWhere.AppendLine();
        //        }
        //        return sbSqlWhere.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //protected string WfGetQueryString(List<QueryInfo> pQueryInfoList, out List<SqlParameter> pSqlParmList)
        //{
        //    StringBuilder sbSqlWhere;
        //    string strOriginalValue = "";
        //    string strCondition = "", strNewValue = "";
        //    try
        //    {
        //        pSqlParmList = new List<SqlParameter>();    //傳回sqlparms
        //        sbSqlWhere = new StringBuilder();           //傳回where字串
        //        foreach (QueryInfo queryModel in pQueryInfoList)
        //        {
        //            //取得該欄位輸入的值並轉為字串
        //            if (queryModel.ColumnType.ToLower() == "datetime")
        //            {
        //                if (queryModel.Value == DBNull.Value)
        //                    strOriginalValue = "";
        //                else
        //                {
        //                    //已經都轉為字串了所以不用再轉型
        //                    strOriginalValue = queryModel.Value.ToString();
        //                }
        //            }
        //            else
        //                strOriginalValue = GlobalFn.isNullRet(queryModel.Value, "");

        //            strCondition = "";
        //            strNewValue = "";
        //            var tempStingList = new List<string>(); //拆解輸入值 如為=只有一筆 between 為二筆 in時則為多筆
        //            if (strOriginalValue == "") //輸入=表示該欄位查詢全部,不做處理
        //                continue;
        //            #region 取得運算式 及截取後的字串
        //            if (queryModel.ColumnType.ToLower() == "datetime")//日期限定用
        //            {
        //                strCondition = "=";
        //                tempStingList.Add(strOriginalValue);
        //            }
        //            else if (strOriginalValue.StartsWith("<>"))
        //            {
        //                strCondition = "<>";
        //                tempStingList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
        //            }
        //            else if (strOriginalValue.StartsWith(">="))
        //            {
        //                strCondition = ">=";
        //                tempStingList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
        //            }
        //            else if (strOriginalValue.StartsWith("<="))
        //            {
        //                strCondition = "<=";
        //                tempStingList.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
        //            }
        //            else if (strOriginalValue.StartsWith("<"))
        //            {
        //                strCondition = "<";
        //                tempStingList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
        //            }
        //            else if (strOriginalValue.StartsWith(">"))
        //            {
        //                strCondition = ">";
        //                tempStingList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
        //            }
        //            else if (strOriginalValue.IndexOf('=') >= 0)
        //            {
        //                strCondition = "=";
        //                tempStingList.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
        //            }
        //            else if (strOriginalValue.IndexOf("%") >= 0)
        //            {
        //                strCondition = "LIKE";
        //                tempStingList.Add(strOriginalValue);
        //            }
        //            else if ((strOriginalValue.IndexOf(":") >= 0))  //遇到日期格式時會有問題
        //            {
        //                strCondition = "BETWEEN";
        //                tempStingList = strOriginalValue.Split(new char[] { ':' }, 2).ToList<string>();
        //            }
        //            else if ((strOriginalValue.IndexOf("|") >= 0))
        //            {
        //                strCondition = "IN";
        //                tempStingList = strOriginalValue.Split(new char[] { '|' }).ToList<string>();
        //            }
        //            else
        //            {
        //                strCondition = "=";
        //                tempStingList.Add(strOriginalValue);
        //            }
        //            #endregion

        //            if (queryModel.ColumnType.ToLower() == "string")
        //            {
        //                if (strCondition.ToLower() == "between")
        //                {
        //                    strNewValue = string.Format(" N'{0}' AND N'{1}'", tempStingList[0], tempStingList[1]);
        //                }
        //                else if (strCondition.ToLower() == "in")
        //                {
        //                    strNewValue = " (";
        //                    foreach (string ls_temp in tempStingList)
        //                    {
        //                        if (tempStingList.LastOrDefault<string>() != ls_temp)
        //                            strNewValue += string.Format("N'{0}',", ls_temp);
        //                        else
        //                            strNewValue += string.Format("N'{0}'", ls_temp);
        //                    }
        //                    strNewValue += ")";
        //                }
        //                else
        //                    strNewValue = string.Format("N'{0}'", tempStingList[0]);
        //                sbSqlWhere.AppendLine(string.Format(" AND {0}.{1} {2} {3} ", queryModel.TableName, queryModel.ColumnName, strCondition, strNewValue));
        //            }
        //            else if (queryModel.ColumnType.ToLower() == "decimal" ||
        //                        queryModel.ColumnType.ToLower() == "double" ||
        //                        queryModel.ColumnType.ToLower().IndexOf("int") >= 0)
        //            {
        //                if (strCondition.ToLower() == "between")
        //                {
        //                    strNewValue = string.Format(" {0} AND {1}", tempStingList[0], tempStingList[1]);
        //                }
        //                else if (strCondition.ToLower() == "in")
        //                {
        //                    strNewValue = " (";
        //                    foreach (string ls_temp in tempStingList)
        //                    {
        //                        if (tempStingList.LastOrDefault<string>() != ls_temp)
        //                            strNewValue += string.Format("{0},", ls_temp);
        //                        else
        //                            strNewValue += string.Format("{0}", ls_temp);
        //                    }
        //                    strNewValue += ")";
        //                }
        //                else
        //                    strNewValue = tempStingList[0];
        //                sbSqlWhere.AppendLine(string.Format(" AND {0}.{1} {2} {3} ", queryModel.TableName, queryModel.ColumnName, strCondition, strNewValue));
        //            }
        //            else if (queryModel.ColumnType.ToLower() == "datetime")
        //            {
        //                if (strCondition.ToLower() == "between")
        //                {
        //                    strNewValue = string.Format(" '{0}' AND '{1}'", tempStingList[0], tempStingList[1]);
        //                }
        //                else if (strCondition.ToLower() == "in")
        //                {
        //                    strNewValue = " (";
        //                    foreach (string ls_temp in tempStingList)
        //                    {
        //                        if (tempStingList.LastOrDefault<string>() != ls_temp)
        //                            strNewValue += string.Format("'{0}',", ls_temp);
        //                        else
        //                            strNewValue += string.Format("'{0}'", ls_temp);
        //                    }
        //                    strNewValue += ")";
        //                }
        //                else
        //                {
        //                    strNewValue = string.Format(" '{0}' ", tempStingList[0]);
        //                }
        //                sbSqlWhere.AppendLine(string.Format(" AND convert(nvarchar(8),{0}.{1},112) {2} {3} ", queryModel.TableName, queryModel.ColumnName, strCondition, strNewValue));
        //            }
        //            else
        //                sbSqlWhere.AppendLine();
        //        }
        //        return sbSqlWhere.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //protected string WfGetQueryString(DataRow pdr, List<QueryInfo> pQueryInfo)
        //{
        //    string strReturn = "";
        //    StringBuilder sbSqlExtendWhere;
        //    string strColumnType;
        //    string strOriginalValue = "";
        //    string strCondition = "", strNewValue = "";
        //    List<string> listTemp = new List<string>();
        //    try
        //    {
        //        if (pdr == null)
        //            throw new Exception("WfGetQueryString 傳入datarow為空值!");

        //        sbSqlExtendWhere = new StringBuilder();
        //        foreach (QueryInfo lrec in pQueryInfo)
        //        {
        //            if (!(pdr.Table.Columns.Contains(lrec.ColumnName)))
        //                continue;

        //            strColumnType = pdr.Table.Columns[lrec.ColumnName].Prefix.ToString();//改用記在 prefix的型別
        //            if (strColumnType.ToLower() == "datetime")
        //            {
        //                if (pdr[lrec.ColumnName] == DBNull.Value)
        //                    strOriginalValue = "";
        //                else
        //                {
        //                    //已經都轉為字串了所以不用再轉型
        //                    strOriginalValue = pdr[lrec.ColumnName].ToString();
        //                }
        //            }
        //            else
        //                strOriginalValue = GlobalFn.isNullRet(pdr[lrec.ColumnName], "");

        //            strCondition = ""; strNewValue = ""; listTemp = new List<string>();
        //            if (strOriginalValue == "")
        //                continue;
        //            #region 取得運算式 及截取後的字串
        //            if (strColumnType.ToLower() == "datetime")//日期限定用
        //            {
        //                strCondition = "=";
        //                listTemp.Add(strOriginalValue);
        //            }
        //            else if (strOriginalValue.StartsWith("<>"))
        //            {
        //                strCondition = "<>";
        //                listTemp.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
        //            }
        //            else if (strOriginalValue.StartsWith(">="))
        //            {
        //                strCondition = ">=";
        //                listTemp.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
        //            }
        //            else if (strOriginalValue.StartsWith("<="))
        //            {
        //                strCondition = "<=";
        //                listTemp.Add(strOriginalValue.Substring(2, strOriginalValue.Length - 2));
        //            }
        //            else if (strOriginalValue.StartsWith("<"))
        //            {
        //                strCondition = "<";
        //                listTemp.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
        //            }
        //            else if (strOriginalValue.StartsWith(">"))
        //            {
        //                strCondition = ">";
        //                listTemp.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
        //            }
        //            else if (strOriginalValue.IndexOf('=') >= 0)
        //            {
        //                strCondition = "=";
        //                listTemp.Add(strOriginalValue.Substring(1, strOriginalValue.Length - 1));
        //            }
        //            else if (strOriginalValue.IndexOf("%") >= 0)
        //            {
        //                strCondition = "LIKE";
        //                listTemp.Add(strOriginalValue);
        //            }
        //            else if ((strOriginalValue.IndexOf(":") >= 0))  //遇到日期格式時會有問題
        //            {
        //                strCondition = "BETWEEN";
        //                listTemp = strOriginalValue.Split(new char[] { ':' }, 2).ToList<string>();
        //            }
        //            else if ((strOriginalValue.IndexOf("|") >= 0))
        //            {
        //                strCondition = "IN";
        //                listTemp = strOriginalValue.Split(new char[] { '|' }).ToList<string>();
        //            }
        //            else
        //            {
        //                strCondition = "=";
        //                listTemp.Add(strOriginalValue);
        //            }
        //            #endregion

        //            if (strColumnType.ToLower() == "string")
        //            {
        //                if (strCondition.ToLower() == "between")
        //                {
        //                    strNewValue = string.Format(" N'{0}' AND N'{1}'", listTemp[0], listTemp[1]);
        //                }
        //                else if (strCondition.ToLower() == "in")
        //                {
        //                    strNewValue = " (";
        //                    foreach (string ls_temp in listTemp)
        //                    {
        //                        if (listTemp.LastOrDefault<string>() != ls_temp)
        //                            strNewValue += string.Format("N'{0}',", ls_temp);
        //                        else
        //                            strNewValue += string.Format("N'{0}'", ls_temp);
        //                    }
        //                    strNewValue += ")";
        //                }
        //                else
        //                    strNewValue = string.Format("N'{0}'", listTemp[0]);
        //                sbSqlExtendWhere.AppendLine(string.Format(" AND {0}.{1} {2} {3} ", lrec.TableName, lrec.ColumnName, strCondition, strNewValue));
        //            }
        //            else if (strColumnType.ToLower() == "decimal" || strColumnType.ToLower() == "double" || strColumnType.ToLower().IndexOf("int") >= 0)
        //            {
        //                if (strCondition.ToLower() == "between")
        //                {
        //                    strNewValue = string.Format(" {0} AND {1}", listTemp[0], listTemp[1]);
        //                }
        //                else if (strCondition.ToLower() == "in")
        //                {
        //                    strNewValue = " (";
        //                    foreach (string ls_temp in listTemp)
        //                    {
        //                        if (listTemp.LastOrDefault<string>() != ls_temp)
        //                            strNewValue += string.Format("{0},", ls_temp);
        //                        else
        //                            strNewValue += string.Format("{0}", ls_temp);
        //                    }
        //                    strNewValue += ")";
        //                }
        //                else
        //                    strNewValue = listTemp[0];
        //                sbSqlExtendWhere.AppendLine(string.Format(" AND {0}.{1} {2} {3} ", lrec.TableName, lrec.ColumnName, strCondition, strNewValue));
        //            }
        //            else if (strColumnType.ToLower() == "datetime")
        //            {
        //                if (strCondition.ToLower() == "between")
        //                {
        //                    strNewValue = string.Format(" '{0}' AND '{1}'", listTemp[0], listTemp[1]);
        //                }
        //                else if (strCondition.ToLower() == "in")
        //                {
        //                    strNewValue = " (";
        //                    foreach (string ls_temp in listTemp)
        //                    {
        //                        if (listTemp.LastOrDefault<string>() != ls_temp)
        //                            strNewValue += string.Format("'{0}',", ls_temp);
        //                        else
        //                            strNewValue += string.Format("'{0}'", ls_temp);
        //                    }
        //                    strNewValue += ")";
        //                }
        //                else
        //                {
        //                    strNewValue = string.Format(" '{0}' ", listTemp[0]);
        //                }
        //                sbSqlExtendWhere.AppendLine(string.Format(" AND convert(nvarchar(8),{0}.{1},112) {2} {3} ", lrec.TableName, lrec.ColumnName, strCondition, strNewValue));
        //            }
        //            else
        //                sbSqlExtendWhere.AppendLine();
        //        }
        //        strReturn = sbSqlExtendWhere.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return strReturn;
        //}
        #endregion

        #region WfAfterInitialForm BLL載入後,供後續載入一次性的model或資料
        protected virtual bool WfAfterInitialForm()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /******************* 其它 Control 事件 ***********************/

        /******************* 其它 Control 方法 ***********************/
        #region WfBindMasterByTag() : 將 mater tab 所有的頁面的控制項，以 control.tag 屬性與 bindingmaster 做 binding
        //只做一次,因此順便處理元件不可編輯,及各種控制項的appearance
        protected virtual Boolean WfBindMasterByTag(Control pctrl, List<aza_tb> pAzaTbList, string pDateFormat)
        {
            //string ls_type = "";
            string tagValue = "";
            Type controlType;
            aza_tb azaModel = null;
            try
            {
                foreach (Control control in pctrl.Controls)
                {
                    //ls_type = lcontrol.GetType().ToString();
                    controlType = control.GetType();
                    if (control.Tag != null
                        )
                    {
                        tagValue = control.Tag.ToString().Trim();
                        azaModel = (from o in pAzaTbList
                                    where o.aza03 == tagValue
                                    select o
                            ).FirstOrDefault<aza_tb>()
                                ;
                        if (azaModel != null)
                        {
                            //if (ls_type != "System.Windows.Forms.PictureBox")   //pictuebox 不要顯示說明
                            //{
                            //    lsbTip = new StringBuilder();
                            //    lsbTip.AppendLine(string.Format("{0}-{1}", l_aza.aza03, l_aza.aza04));
                            //    WfShowTip(lcontrol, lsbTip.ToString());
                            //}

                            control.Enter += new System.EventHandler(this.Control_Enter);

                        }
                    }
                    if (control.HasChildren)
                    {
                        //if (ls_type == "Infragistics.Win.UltraWinTabControl.UltraTabControl")
                        if (controlType == typeof(Infragistics.Win.UltraWinTabControl.UltraTabControl))
                        {
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).TabStop = false;
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Default;
                            //(lcontrol as Infragistics.Win.UltraWinTabControl.UltraTabControl).ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.Office2007;
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).UseAppStyling = false;
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Office2007Ribbon;
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).TabButtonStyle = UIElementButtonStyle.Office2013Button;
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).Appearance.BackColor = ColorTranslator.FromHtml("#D8E6D1");
                            (control as Infragistics.Win.UltraWinTabControl.UltraTabControl).SelectedTabAppearance.BackColor = GetStyleLibrary.FormBackGRoundColor;
                            //(lcontrol as Infragistics.Win.UltraWinTabControl.UltraTabControl).Appearance.ForeColor = Color.Black;
                            //(lcontrol as Infragistics.Win.UltraWinTabControl.UltraTabControl).SelectedTabAppearance.ForeColor = Color.Black;
                        }
                        WfBindMasterByTag(control, pAzaTbList, pDateFormat);    //自我遞迴
                    }
                    #region switch (ls_type)
                    switch (controlType.ToString())
                    {
                        //case "Infragistics.Win.UltraWinGrid.UltraGrid":
                        //    WfSetControlsReadOnly(lcontrol as UltraGrid, true); //一開始繫結時因為是不可編輯,預設唯護  
                        //    if (GetStyleLibrary.FontNormal != null)
                        //        (lcontrol as UltraGrid).Font = GetStyleLibrary.FontNormal;

                        //    (lcontrol as UltraGrid).BeforeExitEditMode += new Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventHandler(UltraGrid_BeforeExitEditMode);
                        //    (lcontrol as UltraGrid).BeforeRowDeactivate += new System.ComponentModel.CancelEventHandler(this.UGrid_BeforeRowDeactivate);
                        //    break;
                        case "System.Windows.Forms.Label":
                            if (GetStyleLibrary.FontControlNormal != null)
                                (control as Label).Font = GetStyleLibrary.FontControlNormal;
                            (control as Label).BackColor = Color.Transparent;
                            if (tagValue.Length > 0)
                            {
                                if (azaModel != null)
                                {
                                    control.Text = azaModel.aza04;
                                }
                            }
                            break;
                        case "Infragistics.Win.UltraWinEditors.UltraTextEditor":
                            if (GetStyleLibrary.FontControlNormal != null)
                                (control as UltraTextEditor).Font = GetStyleLibrary.FontControlNormal;
                            (control as UltraTextEditor).AlwaysInEditMode = true;
                            (control as UltraTextEditor).UseAppStyling = false;
                            (control as UltraTextEditor).DisplayStyle = GetStyleLibrary.ControlDisplayStyle;
                            if (tagValue.Length > 0)
                            {
                                control.DataBindings.Clear();

                                (control as UltraTextEditor).Validating += new CancelEventHandler(UltraTextEditor_Validating);
                                if (azaModel != null)
                                {
                                    control.DataBindings.Add("Text", BindingMaster, tagValue);
                                    control.DataBindings["Text"].DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
                                    if (azaModel.aza08 == "numeric" || azaModel.aza08 == "int")
                                    {
                                        (control as UltraTextEditor).Appearance.TextHAlign = HAlign.Right;         //靠右對齊
                                        control.DataBindings["Text"].FormattingEnabled = true;
                                        control.DataBindings["Text"].FormatString = "###,###,###,###,###0.######";
                                        //control.DataBindings.Add("Text", BindingMaster, tagValue, true, DataSourceUpdateMode.OnPropertyChanged, null, "#,0.########");
                                    }
                                    //else
                                    //    control.DataBindings.Add("Text", BindingMaster, tagValue, false, DataSourceUpdateMode.OnPropertyChanged);

                                    (control as UltraTextEditor).KeyUp += new KeyEventHandler(UltraTextEditor_KeyUp);
                                    if ((azaModel.aza13 == null ? "" : azaModel.aza13).ToUpper() == "Y")                  //是否有PICK功能
                                    {
                                        WfSetUltraTxtEditPick(control as Infragistics.Win.UltraWinEditors.UltraTextEditor);
                                        (control as UltraTextEditor).EditorButtonClick += new EditorButtonEventHandler(UltraTextEditor_EditorPickButtonClick);
                                    }
                                }
                            }
                            WfSetControlReadonly(control as Infragistics.Win.UltraWinEditors.UltraTextEditor, true);
                            break;

                        case "Infragistics.Win.UltraWinGrid.UltraCombo":
                            if (GetStyleLibrary.FontControlNormal != null)
                                (control as UltraCombo).Font = GetStyleLibrary.FontControlNormal;
                            WfSetControlReadonly(control as Infragistics.Win.UltraWinGrid.UltraCombo, true);
                            (control as UltraCombo).RowSelected += new RowSelectedEventHandler(this.Ucombo_RowSelected);
                            (control as UltraCombo).UseAppStyling = false;
                            (control as UltraCombo).DisplayStyle = GetStyleLibrary.ControlDisplayStyle;
                            (control as UltraCombo).KeyUp += new KeyEventHandler(UltraCombo_KeyUp);
                            if (tagValue.Length > 0)
                            {
                                control.DataBindings.Clear();
                                control.DataBindings.Add("Value", BindingMaster, tagValue, false, DataSourceUpdateMode.OnPropertyChanged);
                                (control as UltraCombo).Validating += new CancelEventHandler(UltraCombo_Validating);
                            }
                            break;

                        case "YR.Util.Controls.UcCheckBox":
                            if (GetStyleLibrary.FontControlNormal != null)
                                (control as YR.Util.Controls.UcCheckBox).Font = GetStyleLibrary.FontControlNormal;
                            (control as UcCheckBox).BackColor = Color.Transparent;
                            WfSetControlReadonly(control as YR.Util.Controls.UcCheckBox, true);
                            (control as UcCheckBox).KeyUp += new KeyEventHandler(UcCheckBox_KeyUp);
                            if (tagValue.Length > 0)
                            {
                                control.DataBindings.Clear();
                                control.DataBindings.Add("CheckValue", BindingMaster, tagValue, false, DataSourceUpdateMode.OnPropertyChanged);
                                (control as YR.Util.Controls.UcCheckBox).Validating += new CancelEventHandler(UcCheckBox_Validating);
                                if (azaModel != null)
                                {
                                    control.Text = azaModel.aza04;
                                }
                            }
                            break;

                        //case "YR.Util.Controls.UcDatePicker":
                        //    if (GetStyleLibrary.FontControlNormal != null)
                        //        (lcontrol as YR.Util.Controls.UcDatePicker).Font = GetStyleLibrary.FontControlNormal;
                        //    (lcontrol as YR.Util.Controls.UcDatePicker).UseAppStyling = false;
                        //    (lcontrol as YR.Util.Controls.UcDatePicker).ButtonStyle = UIElementButtonStyle.Office2013Button;
                        //    WfSetControlReadonly(lcontrol as YR.Util.Controls.UcDatePicker, true);
                        //    if (ls_tag.Length > 0)
                        //    {
                        //        lcontrol.DataBindings.Clear();
                        //        lcontrol.DataBindings.Add("Value", bindingMaster, ls_tag, false, DataSourceUpdateMode.OnPropertyChanged);
                        //        (lcontrol as UcDatePicker).Validating += new CancelEventHandler(UcDatePicker_Validating);
                        //    }
                        //    break;

                        case "Infragistics.Win.UltraWinEditors.UltraDateTimeEditor":
                            (control as UltraDateTimeEditor).AlwaysInEditMode = true;
                            (control as UltraDateTimeEditor).PromptChar = ' ';

                            if (GetStyleLibrary.FontControlNormal != null)
                                (control as Infragistics.Win.UltraWinEditors.UltraDateTimeEditor).Font = GetStyleLibrary.FontControlNormal;
                            (control as Infragistics.Win.UltraWinEditors.UltraDateTimeEditor).UseAppStyling = false;
                            if (((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).FormatString == null)
                                ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).FormatString = "yyyy/MM/dd";
                            if (((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).MaskInput == null)
                                ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).MaskInput = "yyyy/mm/dd";
                            ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).Appearance.TextHAlign = HAlign.Center;
                            ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.OnMouseEnter;
                            ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).DisplayStyle = GetStyleLibrary.ControlDisplayStyle;

                            WfSetControlReadonly(control as UltraDateTimeEditor, true);
                            (control as UltraDateTimeEditor).KeyUp += new KeyEventHandler(UltraDateTimeEditor_KeyUp);
                            if (tagValue.Length > 0)
                            {
                                control.DataBindings.Clear();
                                control.DataBindings.Add("Value", BindingMaster, tagValue, false, DataSourceUpdateMode.OnPropertyChanged);
                                (control as UltraDateTimeEditor).Validating += new CancelEventHandler(UltraDateTimeEditor_Validating);
                            }
                            if (azaModel.aza14 == "Y")   //是否顯示時間
                            {
                                ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).FormatString = pDateFormat + " hh:mm";
                                ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).MaskInput = pDateFormat.ToLower() + " hh:mm";
                            }
                            else
                            {
                                ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).FormatString = pDateFormat;
                                ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(control)).MaskInput = pDateFormat.ToLower();
                            }
                            //((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(lcontrol)).FormatString = "yyyy/MM/dd tt hh:mm";
                            //((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)(lcontrol)).MaskInput = "{LOC}yyyy/mm/dd hh:mm";
                            break;
                        case "System.Windows.Forms.PictureBox":
                            control.DataBindings.Add("Image", BindingMaster, tagValue, true);
                            break;
                        case "Infragistics.Win.Misc.UltraSplitter":
                            WfSetAppearance(control as Infragistics.Win.Misc.UltraSplitter);
                            break;
                        default:
                            break;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return true;
        }

        #endregion

        #region WfSetUltraTxtEditPick 新增pick按鈕
        protected void WfSetUltraTxtEditPick(Infragistics.Win.UltraWinEditors.UltraTextEditor pUte)
        {
            Infragistics.Win.UltraWinEditors.EditorButton eb;
            ImageList ilLarge;
            try
            {
                ilLarge = GlobalPictuer.LoadToolBarImage();
                eb = new Infragistics.Win.UltraWinEditors.EditorButton();
                eb.Key = "pick";
                eb.Tag = pUte.Tag;
                if (ilLarge != null)
                {
                    if (ilLarge.Images.ContainsKey("pick_32"))
                        eb.Appearance.Image = ilLarge.Images["pick_32"];
                    eb.ButtonStyle = UIElementButtonStyle.Flat;
                    //eb.Appearance.BackColor = Color.FromArgb(67, 168, 152);
                }
                eb.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2013Button;
                pUte.ButtonsRight.Add(eb);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfSetUgridColumnPick(ucol) : 設定 UltraGrid 中的 Column 為 文字編輯 + Button
        public void WfSetUgridColumnPick(Infragistics.Win.UltraWinGrid.UltraGridColumn ugc)
        {
            ImageList ilLarge;
            ilLarge = GlobalPictuer.LoadToolBarImage();
            try
            {
                ugc.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
                ugc.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnCellActivate;
                if (ilLarge != null)
                {
                    if (ilLarge.Images.ContainsKey("pick_32"))
                        ugc.CellButtonAppearance.Image = ilLarge.Images["pick_32"];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetUcomboxDataSource : 設定 ComboBox 的數據源 Key Value 字段
        public void WfSetUcomboxDataSource(Infragistics.Win.UltraWinGrid.UltraCombo ucb, List<KeyValuePair<string, string>> pSourceList)
        {
            try
            {
                ucb.DisplayMember = "Value";
                ucb.ValueMember = "Key";
                ucb.DataSource = pSourceList;
                WfSetAppearance(ucb, 1);
                ucb.DisplayLayout.Bands[0].Override.ColumnAutoSizeMode = ColumnAutoSizeMode.VisibleRows;
                ucb.DropDownStyle = UltraComboStyle.DropDownList;
                ucb.DisplayLayout.Bands[0].Columns["key"].Width = 60;
                ucb.DisplayLayout.Bands[0].Columns["value"].Width = ucb.Width - 60;

                // 預設定位在第一筆
                if (ucb.DataSource != null && pSourceList.Count > 0)
                {
                    ucb.PerformAction(UltraComboAction.FirstRow);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetGridValueList : 將傳入的 GridColumn 設為下拉選單
        /// <summary>
        /// 將傳入的 GridColumn 設為下拉選單
        /// </summary>
        /// <param name="ucol">傳入的 GridColumn</param>
        /// <param name="lstKVP">下拉選單的清單來源 (List)</param>
        public void WfSetGridValueList(Infragistics.Win.UltraWinGrid.UltraGridColumn ucol, List<KeyValuePair<string, string>> lstKVP)
        {
            try
            {
                if (ucol.ValueList == null)
                    this.WfCreateGridDropdown(ucol, "Key", "Value");

                if (ucol.ValueList.GetType() == typeof(UltraDropDown))
                {
                    UltraDropDown dropDown = ucol.ValueList as UltraDropDown;
                    dropDown.DataSource = lstKVP;
                }
                ucol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetGridValueList : 將傳入的 GridCell 設為下拉選單
        /// <summary>
        /// 將傳入的 GridCell 設為下拉選單
        /// </summary>
        /// <param name="uCell">傳入的 GridCell</param>
        /// <param name="lstKVP">下拉選單的清單來源 (List)</param>
        public void WfSetGridValueList(Infragistics.Win.UltraWinGrid.UltraGridCell uCell, List<KeyValuePair<string, string>> lstKVP)
        {
            try
            {
                if (uCell.ValueList == null)
                    this.WfCreateGridDropdown(uCell, "Key", "Value");

                if (uCell.ValueList.GetType() == typeof(UltraDropDown))
                {
                    UltraDropDown dropDown = uCell.ValueList as UltraDropDown;
                    dropDown.DataSource = lstKVP;
                }
                uCell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfCreateGridDropdown : 建立 Grid 的下拉選單 (DropDown) 物件
        /// <summary>
        /// 建立 Grid 的 DropDown 物件
        /// </summary>
        /// <param name="ucol">要建立下拉選單的 Grid Column</param>
        /// <param name="ValueMember">於來源清單(List)中，要存入資料庫的欄位名</param>
        /// <param name="strDisplayMember">於來源清單(List)中，顯示的欄位名</param>
        public void WfCreateGridDropdown(Infragistics.Win.UltraWinGrid.UltraGridColumn ucol, string ValueMember, string strDisplayMember)
        {
            if (ucol.ValueList == null)
            {
                Infragistics.Win.UltraWinGrid.UltraDropDown dropDown = new UltraDropDown();
                dropDown.Visible = false;
                this.Controls.Add(dropDown);

                dropDown.ValueMember = ValueMember;
                dropDown.DisplayMember = strDisplayMember;
                ucol.ValueList = dropDown;
                ucol.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                WfSetAppearance(dropDown);
            }
        }
        #endregion

        #region WfCreateGridDropdown : 建立 Grid 的下拉選單 (DropDown) 物件
        /// <summary>
        /// 建立 Grid 的 DropDown 物件
        /// </summary>
        /// <param name="uCell">要建立下拉選單的 Grid Column</param>
        /// <param name="ValueMember">於來源清單(List)中，要存入資料庫的欄位名</param>
        /// <param name="strDisplayMember">於來源清單(List)中，顯示的欄位名</param>
        public void WfCreateGridDropdown(Infragistics.Win.UltraWinGrid.UltraGridCell uCell, string ValueMember, string strDisplayMember)
        {
            if (uCell.ValueList == null)
            {
                Infragistics.Win.UltraWinGrid.UltraDropDown dropDown = new UltraDropDown();
                dropDown.Visible = false;
                this.Controls.Add(dropDown);

                dropDown.ValueMember = ValueMember;
                dropDown.DisplayMember = strDisplayMember;
                uCell.ValueList = dropDown;
                //uCell.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;

                WfSetAppearance(dropDown);
            }
        }
        #endregion

        #region Ucombo_RowSelected : combobox 資料列變更事件
        /// <summary>
        /// combobox 資料列變更
        /// </summary>
        /// <param name="sender">變更的 object</param>
        /// <param name="e">EventArgs</param>
        protected virtual void Ucombo_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                string controlName = WfGetControlName(sender);
                //string ls_type = sender.GetType().ToString().ToLower().Trim();
                var type = sender.GetType();
                //if (ls_type == "infragistics.win.ultrawingrid.ultracombo")
                if (type == typeof(Infragistics.Win.UltraWinGrid.UltraCombo))
                {
                    Infragistics.Win.UltraWinGrid.UltraCombo uCombo = ((Infragistics.Win.UltraWinGrid.UltraCombo)sender);
                    if (uCombo.IsDroppedDown) { uCombo.PerformAction(UltraComboAction.CloseDropdown); }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return;
        }
        #endregion

        #region WfSetUgridCheckBox : 設置 Master Grid 的 checkbox ( Y/N)
        protected void WfSetUgridCheckBox(UltraGridColumn pcol)
        {
            if (pcol == null) return;

            Infragistics.Win.UltraWinEditors.UltraCheckEditor uCheckeditor = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            uCheckeditor.Editor.DataFilter = new YR.Util.Controls.CheckEditorDataFilter();
            uCheckeditor.Visible = false;
            uCheckeditor.CheckAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(uCheckeditor);
            pcol.EditorComponent = uCheckeditor;
            //pcol.EditorControl = lcheckeditor;
            pcol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            //pcol.Editor.BeforeExitEditMode += new Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventHandler(ultraGrid_BeforeExitEditMode);
            //pcol.Editor.ValueChanged += new System.EventHandler(this.uGridCheckEditor_ValueChanged);
        }
        #endregion

        #region WfSetGridActiveRow : 設定傳入 Grid 的 ActiveRow 至指定的 index
        /// <summary>
        /// 設定傳入 Grid 的 ActiveRow 至指定的 index
        /// </summary>
        /// <param name="pUgrid">要設定的 Grid </param>
        /// <param name="pIndex">要設定 active row 的 index</param>
        protected virtual void WfSetGridActiveRow(UltraGrid pUgrid, int pIndex)
        {
            if (pUgrid == null) return;

            // 檢查要設定的 active 的 row 編號不可大於全部可見的 row 筆數
            if (pIndex >= pUgrid.Rows.VisibleRowCount)
                return;
            pUgrid.ActiveRow = pUgrid.Rows.GetRowAtVisibleIndex(pIndex);
        }
        #endregion

        #region WfGetMaxSeq 傳回seq最大值
        protected decimal WfGetMaxSeq(DataTable pDt, string pSeqName)
        {
            decimal RtnValue;
            try
            {
                var dMaxSeq = pDt.AsEnumerable()
                    //.Where(row=>row.Field<decimal?>(pSeqName)!=null)
                        .Max(row => row.Field<decimal?>(pSeqName))
                        ;

                if (dMaxSeq == null)
                    dMaxSeq = 0;

                RtnValue = Convert.ToDecimal(dMaxSeq) + 1;
                return RtnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfFindErrUltraGridCell 尋找錯誤的detail cell 並focus 至該筆
        protected void WfFindErrUltraGridCell(Infragistics.Win.UltraWinGrid.UltraGrid pUgrid, DataRow pdr, string pColName)
        {
            try
            {
                foreach (Infragistics.Win.UltraWinGrid.UltraGridRow lugr in pUgrid.Rows)
                {
                    if (((DataRowView)lugr.ListObject).Row == pdr)
                    {
                        pUgrid.Focus();
                        pUgrid.ActiveCell = lugr.Cells[pColName];
                        pUgrid.ActiveCell.Selected = true;
                        lugr.Cells[pColName].Activate();
                        if (pUgrid.ActiveCell.CanEnterEditMode == true)
                        {
                            pUgrid.PerformAction(UltraGridAction.EnterEditMode, false, false);

                        }

                        pdr.SetColumnError(pColName, "Error!");
                        pdr.RowError = "Error!";
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetFirstVisibelCellFocus Grid的focus 移至可見的第一個cell
        protected void WfSetFirstVisibelCellFocus(UltraGrid pGrid)
        {
            try
            {
                if (pGrid.ActiveRow == null)
                    return;
                var visibelColumns = pGrid.ActiveRow.Band.Columns.Cast<UltraGridColumn>()
                                            .Where(p => p.Header.VisiblePosition >= 0)
                                            .Where(p => p.Hidden == false)
                                            .OrderBy(p => p.Header.VisiblePosition)
                                            ;
                foreach (UltraGridColumn ugc in visibelColumns)
                {
                    if (pGrid.ActiveRow.Cells[ugc.Key].Activation == Activation.AllowEdit)
                    {
                        pGrid.ActiveCell = pGrid.ActiveRow.Cells[ugc.Key];
                        break;
                    }
                }
                pGrid.PerformAction(UltraGridAction.EnterEditMode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetGridButtomRowCount 設定grid筆數總計
        protected void WfSetGridButtomRowCount(UltraGrid pUgrid)
        {
            try
            {
                //pUgrid.DisplayLayout.Override.AllowRowSummaries = AllowRowSummaries.True;
                pUgrid.DisplayLayout.Bands[0].Summaries.Clear();
                UltraGridColumn columnToSummarize = pUgrid.DisplayLayout.Bands[0].GetFirstVisibleCol(pUgrid.ActiveColScrollRegion, true);
                if (columnToSummarize == null)
                    return;
                SummarySettings summary = pUgrid.DisplayLayout.Bands[0].Summaries.Add("Count", SummaryType.Count, columnToSummarize);
                summary.DisplayFormat = "總筆數: {0:N0}";
                summary.Appearance.BackColor = GetStyleLibrary.SummaryBackGroundColor;
                //summary.Appearance.TextVAlign = VAlign.Middle;
                pUgrid.DisplayLayout.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
                pUgrid.DisplayLayout.Override.SummaryDisplayArea |= SummaryDisplayAreas.GroupByRowsFooter;
                pUgrid.DisplayLayout.Override.SummaryDisplayArea |= SummaryDisplayAreas.InGroupByRows;

                summary.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.GroupByRowsFooter;
                pUgrid.DisplayLayout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;
                //pUgrid.DisplayLayout.Override.BorderStyleSummaryValue = Infragistics.Win.UIElementBorderStyle.None;

                //pUgrid.DisplayLayout.Override.SummaryFooterAppearance.BackColor = GetStyleLibrary.SummaryBackGroundColor;                
                pUgrid.DisplayLayout.Override.SummaryFooterAppearance.FontData.Bold = DefaultableBoolean.True;
                pUgrid.DisplayLayout.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
                pUgrid.DisplayLayout.Override.SummaryFooterAppearance.TextVAlign = VAlign.Middle;
                pUgrid.DisplayLayout.Override.SummaryFooterAppearance.TextHAlign = HAlign.Left;
                pUgrid.DisplayLayout.Override.BorderStyleSummaryFooter = Infragistics.Win.UIElementBorderStyle.None;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfShowStatusBar 是否顯示 buttom的statusbar
        protected void WfShowStatusBar(Boolean pVisibled)
        {
            try
            {
                if (pVisibled)
                    UsbButtom.Show();
                else
                    UsbButtom.Hide();

                //UsbButtom.Visible = pVisibled;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfShowTip tips 說明
        protected virtual void WfShowTip(Control p_control, string ps_tag)
        {

            try
            {
                System.Windows.Forms.ToolTip tips = new System.Windows.Forms.ToolTip();
                tips.UseAnimation = true;
                tips.UseFading = true;
                tips.AutoPopDelay = 5000;
                tips.SetToolTip(p_control, ps_tag);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfBindMaster 設定數據源與組件的 binding
        protected virtual void WfBindMaster()
        {
            try
            {

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfSetMasterGridLayout
        protected virtual void WfSetMasterGridLayout()
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetGridHeader:設定 Grid 各欄位的 Title
        public void WfSetGridHeader(Infragistics.Win.UltraWinGrid.UltraGrid uGrid, List<aza_tb> pListAza, string pDefaultDateFormat)
        {
            try
            {
                string columnName;
                aza_tb azaModel;

                foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn ugc in uGrid.DisplayLayout.Bands[0].Columns)
                {
                    columnName = ugc.Key;
                    azaModel = (from o in pListAza
                                where o.aza03 == columnName
                                select o
                        ).FirstOrDefault<aza_tb>();
                    if (azaModel == null)
                    {
                        ugc.Hidden = true;
                        continue;
                    }
                    else
                    {
                        ugc.Header.Caption = azaModel.aza04;
                        if (azaModel.aza07 < 30)
                            ugc.Width = 30;
                        else
                            ugc.Width = Convert.ToInt16(azaModel.aza07);

                        if (azaModel.aza05 != "Y")
                            ugc.Hidden = true;

                        //處理欄位類型
                        switch (azaModel.aza08.ToLower())
                        {
                            case "nvarchar":
                                ugc.CellAppearance.TextHAlign = HAlign.Left;
                                break;
                            case "numeric":
                                ugc.CellAppearance.TextHAlign = HAlign.Right;
                                ugc.Format = "#,0.########";
                                break;
                            case "date":
                                ugc.CellAppearance.TextHAlign = HAlign.Center;
                                ugc.Format = pDefaultDateFormat;
                                ugc.MaskInput = pDefaultDateFormat.ToLower();
                                break;
                            case "datetime":
                                ugc.CellAppearance.TextHAlign = HAlign.Center;
                                if (azaModel.aza14 == "Y")   //是否顯示時間
                                {
                                    ugc.Format = pDefaultDateFormat + " hh:mm";
                                    ugc.MaskInput = pDefaultDateFormat.ToLower() + " hh:mm";

                                    //ugc.Format = "yyyy/MM/dd";  //時間預設顯示格式至日期
                                    //ugc.MaskInput = "yyyy/mm/dd";
                                }
                                else
                                {
                                    ugc.Format = pDefaultDateFormat;
                                    ugc.MaskInput = pDefaultDateFormat.ToLower();
                                }
                                break;
                            case "datetime2":
                                ugc.CellAppearance.TextHAlign = HAlign.Center;
                                if (azaModel.aza14 == "Y")   //是否顯示時間
                                {
                                    ugc.Format = pDefaultDateFormat + " hh:mm";
                                    ugc.MaskInput = pDefaultDateFormat.ToLower() + " hh:mm";

                                    //ugc.Format = "yyyy/MM/dd";  //時間預設顯示格式至日期
                                    //ugc.MaskInput = "yyyy/mm/dd";
                                }
                                else
                                {
                                    ugc.Format = pDefaultDateFormat;
                                    ugc.MaskInput = pDefaultDateFormat.ToLower();
                                }
                                break;
                            default:
                                break;
                        }
                        //處理欄位大小寫       
                        switch (azaModel.aza12 == null ? "" : azaModel.aza12.ToLower())
                        {
                            case "u":
                                ugc.CharacterCasing = CharacterCasing.Upper;
                                break;
                            case "l":
                                ugc.CharacterCasing = CharacterCasing.Lower;
                                break;
                            default:
                                ugc.CharacterCasing = CharacterCasing.Normal;
                                break;
                        }
                        //查詢PICK
                        if (azaModel.aza13 != null && azaModel.aza13 == "Y")
                        {
                            WfSetUgridColumnPick(ugc);
                        }

                    }
                }
                var q = from o in pListAza
                        orderby o.aza06
                        where o.aza05 == "Y"
                        select o;
                var li_position = 0;
                foreach (aza_tb azaTempModel in q.ToList<aza_tb>())
                {
                    if (uGrid.DisplayLayout.Bands[0].Columns.Exists(azaTempModel.aza03))
                    {
                        uGrid.DisplayLayout.Bands[0].Columns[azaTempModel.aza03].Header.VisiblePosition = li_position;
                        li_position++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /******************* ToolBar 工具列事件 ***********************/
        #region WfAddAction 新增action及報表按鈕
        protected virtual List<ButtonTool> WfAddAction()
        {
            try
            {

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfActionClick(string pBtName) 報表及ACTION功能
        protected virtual void WfActionClick(string pBtName)
        {
            try
            {
                // sample code
                switch (pBtName.ToLower())
                {
                    case "":
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion


        /******************* Control Readonly ***********************/
        #region WfSetControlsReadOnlyRecursion 設定 ControlCollection 唯讀或可編輯
        /// <summary>
        /// 設定控制項及子項的唯讀屬性(指定開,則未指定的會關閉)
        /// </summary>
        /// <param name="pctrl"></param>
        /// <param name="pbReadOnly"></param>
        /// <returns></returns>
        protected virtual Boolean WfSetControlsReadOnlyRecursion(Control pctrl, bool pbReadOnly)
        {
            try
            {
                foreach (Control control in pctrl.Controls)
                {
                    if (control.HasChildren)
                    {
                        WfSetControlsReadOnlyRecursion(control, pbReadOnly);    //自我遞迴
                    }
                    WfSetControlReadonly(control, pbReadOnly);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        #endregion

        #region WfSetControlReadonly List<Control>
        protected void WfSetControlReadonly(List<Control> pControlCollection, bool pbReadOnly)
        {
            try
            {
                foreach (Control control in pControlCollection)
                {
                    WfSetControlReadonly(control, pbReadOnly);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetControlReadonly : Control 傳入控制項,方法會自動辨識類別
        protected void WfSetControlReadonly(Control pControl, bool pbReadOnly)
        {
            string type;
            try
            {
                type = pControl.GetType().ToString();
                #region switch (ls_type)
                switch (type)
                {
                    case "Infragistics.Win.UltraWinGrid.UltraGrid":
                        WfSetControlReadonly(pControl as UltraGrid, pbReadOnly);
                        break;
                    case "System.Windows.Forms.Label":
                        break;
                    case "System.Windows.Forms.TextBox":
                        break;
                    case "Infragistics.Win.UltraWinEditors.UltraTextEditor":
                        WfSetControlReadonly(pControl as Infragistics.Win.UltraWinEditors.UltraTextEditor, pbReadOnly);
                        break;

                    case "YR.Util.Controls.ButtonLeftLabel":
                        break;

                    case "Infragistics.Win.UltraWinGrid.UltraCombo":
                        WfSetControlReadonly(pControl as UltraCombo, pbReadOnly);
                        break;

                    case "YR.Util.Controls.UcCheckBox":
                        WfSetControlReadonly(pControl as YR.Util.Controls.UcCheckBox, pbReadOnly);
                        break;
                    case "System.Windows.Forms.DateTimePicker":
                        break;

                    //case "YR.Util.Controls.UcDatePicker":
                    //    WfSetControlReadonly(pControl as YR.Util.Controls.UcDatePicker, pbReadOnly);
                    //    break;
                    case "Infragistics.Win.UltraWinEditors.UltraDateTimeEditor":
                        WfSetControlReadonly(pControl as UltraDateTimeEditor, pbReadOnly);
                        break;
                    default:

                        break;
                }
                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region WfSetControlReadonly : 設定 TextBox 為唯讀
        protected void WfSetControlReadonly(Infragistics.Win.UltraWinEditors.UltraTextEditor pUte, bool pbReadOnly)
        {
            try
            {
                bool isRequired = false;
                if (pUte.AccessibleDescription != null && pUte.AccessibleDescription.ToLower() == "r")
                    isRequired = true;
                WfSetControlReadonly(pUte, pbReadOnly, isRequired);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 設定 TextBox 為唯讀
        /// </summary>
        /// <param name="pUte">TextBox</param>
        /// <param name="pbReadOnly">是否唯讀</param>
        /// <param name="pbRequiredColumn">是否為必要欄位</param>
        protected void WfSetControlReadonly(Infragistics.Win.UltraWinEditors.UltraTextEditor pUte, bool pbReadOnly, bool pbRequiredColumn)
        {
            try
            {
                pUte.ReadOnly = pbReadOnly;

                // 唯讀
                if (pbReadOnly)
                {
                    if (pbRequiredColumn)
                    { pUte.BackColor = GetStyleLibrary.BackGround_readonly; }
                    else
                    { pUte.BackColor = GetStyleLibrary.BackGround_readonly; }

                    pUte.ForeColor = GetStyleLibrary.Font_readonly;

                    pUte.TabStop = false;
                }
                else
                {
                    if (pbRequiredColumn)
                    { pUte.BackColor = GetStyleLibrary.BackGround_required; }
                    else
                    { pUte.BackColor = GetStyleLibrary.BackGround_edit; }
                    pUte.ForeColor = GetStyleLibrary.Font_edit;
                    pUte.TabStop = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetControlReadonly : Grid 相關 UltraGrid、UltraGridColumn、UltraGridCell
        protected void WfSetControlReadonly(UltraGrid pUgrid, bool pbReadOnly)
        {
            try
            {
                if (pUgrid.DisplayLayout.Bands[0] != null)
                {
                    foreach (UltraGridColumn ugc in pUgrid.DisplayLayout.Bands[0].Columns)
                    {
                        WfSetControlReadonly(ugc, pbReadOnly);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void WfSetControlReadonly(UltraGridColumn pUgc, bool pbReadOnly)
        {
            try
            {
                if (pbReadOnly)
                {
                    pUgc.CellActivation = Activation.ActivateOnly;
                    pUgc.TabStop = false;
                }
                else
                {
                    pUgc.CellActivation = Activation.AllowEdit;
                    pUgc.TabStop = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void WfSetControlReadonly(UltraGrid pUgrid, List<string> pColumnNameList, bool pbReadOnly)
        {
            try
            {
                foreach (string s in pColumnNameList)
                {
                    if (pUgrid.DisplayLayout.Bands[0].Columns.Exists(s))
                    {
                        var uColumn = pUgrid.DisplayLayout.Bands[0].Columns[s];
                        WfSetControlReadonly(uColumn, pbReadOnly);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region WfSetControlReadonly : 設定 cell 為唯讀
        protected void WfSetControlReadonly(Infragistics.Win.UltraWinGrid.UltraGridCell pUgc, bool pbReadOnly)
        {
            try
            {
                if (pbReadOnly == true)
                    pUgc.TabStop = DefaultableBoolean.False;
                else
                    pUgc.TabStop = DefaultableBoolean.True;

                // 唯讀
                if (pbReadOnly)
                {
                    pUgc.Activation = Activation.ActivateOnly;
                }
                else
                {
                    pUgc.Activation = Activation.AllowEdit;
                    pUgc.Appearance.BackColor = Color.Empty;
                    pUgc.ButtonAppearance.BackColor = pUgc.Band.Override.CellButtonAppearance.BackColor;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #endregion

        #region WfSetControlReadonly : UcCheckBox
        protected void WfSetControlReadonly(YR.Util.Controls.UcCheckBox pUcx, bool pbReadOnly)
        {
            try
            {
                pUcx.Enabled = !pbReadOnly;
                //if (bReadOnly)
                //{
                //    // pUcx.BackColor = GetStyleLibrary.BackGround_readonly;
                //    pUcx.ForeColor = GetStyleLibrary.Font_edit;
                //}
                //else
                //{
                //    //pUcx.BackColor = GetStyleLibrary.BackGround_edit;
                //    pUcx.ForeColor = GetStyleLibrary.Font_edit;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetControlReadonly : 設定 ultra combo 為唯讀
        public void WfSetControlReadonly(UltraCombo pUCombo, bool pbReadOnly)
        {
            bool lb_required = false;
            try
            {
                if (pUCombo.AccessibleDescription != null && pUCombo.AccessibleDescription.ToLower() == "r")
                    lb_required = true;
                WfSetControlReadonly(pUCombo, pbReadOnly, lb_required);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 設定 ultra combo 為可用
        /// </summary>
        /// <param name="txtbox">TextBox</param>
        /// <param name="pbReadOnly">是否可點選</param>
        public void WfSetControlReadonly(UltraCombo pUcombo, bool pbReadOnly, bool pbRequiredColumn)
        {
            try
            {
                pUcombo.ReadOnly = pbReadOnly;
                if (pbReadOnly)
                {
                    pUcombo.TabStop = false;
                    // 可修改且為必要欄位
                    pUcombo.Appearance.BackColor = GetStyleLibrary.BackGround_readonly;
                    //}
                    //if (bRequiredColumn)
                    //{
                    //    ucombo.Appearance.BackColor = GetStyleLibrary.BackGround_readonly;
                    //}
                }
                else
                {
                    pUcombo.TabStop = true;
                    if (pbRequiredColumn)
                        pUcombo.Appearance.BackColor = GetStyleLibrary.BackGround_required;
                    else
                        pUcombo.Appearance.BackColor = GetStyleLibrary.BackGround_edit;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfSetControlReadonly : 設定 UcDatePicker 為唯讀
        //public void WfSetControlReadonly(UcDatePicker udp, bool pbReadOnly)
        //{
        //    bool lb_required = false;
        //    try
        //    {
        //        if (udp.AccessibleDescription != null && udp.AccessibleDescription.ToLower() == "r")
        //            lb_required = true;
        //        WfSetControlReadonly(udp, pbReadOnly, lb_required);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public void WfSetControlReadonly(UcDatePicker udp, bool pbReadOnly, bool pbRequiredColumn)
        //{
        //    try
        //    {
        //        udp.ReadOnly = pbReadOnly;
        //        if (pbReadOnly)
        //        {
        //            udp.TabStop = false;
        //            // 可修改且為必要欄位
        //            udp.Appearance.BackColor = GetStyleLibrary.BackGround_readonly;
        //        }
        //        else
        //        {
        //            udp.TabStop = true;
        //            if (pbRequiredColumn)
        //                udp.Appearance.BackColor = GetStyleLibrary.BackGround_required;
        //            else
        //                udp.Appearance.BackColor = GetStyleLibrary.BackGround_edit;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion

        #region WfSetControlReadonly : 設定 UltraDateTimeEditor 為唯讀
        public void WfSetControlReadonly(UltraDateTimeEditor pUdt, bool pbReadOnly)
        {
            bool lb_required = false;
            try
            {
                if (pUdt.AccessibleDescription != null && pUdt.AccessibleDescription.ToLower() == "r")
                    lb_required = true;
                WfSetControlReadonly(pUdt, pbReadOnly, lb_required);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void WfSetControlReadonly(UltraDateTimeEditor pUdt, bool pbReadOnly, bool pbRequiredColumn)
        {
            try
            {
                pUdt.ReadOnly = pbReadOnly;
                if (pbReadOnly)
                {
                    pUdt.TabStop = false;
                    // 可修改且為必要欄位
                    pUdt.Appearance.BackColor = GetStyleLibrary.BackGround_readonly;
                }
                else
                {
                    pUdt.TabStop = true;
                    if (pbRequiredColumn)
                        pUdt.Appearance.BackColor = GetStyleLibrary.BackGround_required;
                    else
                        pUdt.Appearance.BackColor = GetStyleLibrary.BackGround_edit;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        /******************* Control Appearance ***********************/
        #region WfSetAppearance :  UltraCombo
        protected void WfSetAppearance(Infragistics.Win.UltraWinGrid.UltraCombo pUCombo, int pStyle)
        {
            try
            {
                switch (pStyle)
                {
                    case 1:
                        //主要樣式 header:淺綠背景 交替列:水藍色
                        //交替列
                        pUCombo.DisplayLayout.Override.RowAlternateAppearance.BackColor = ColorTranslator.FromHtml("#D2E9FF");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfSetAppearance :  UltraDropDown
        protected void WfSetAppearance(Infragistics.Win.UltraWinGrid.UltraDropDown pUDropDown)
        {
            try
            {
                pUDropDown.DisplayLayout.Override.RowAlternateAppearance.BackColor = ColorTranslator.FromHtml("#D2E9FF"); //水藍色
                pUDropDown.DropDownWidth = 200;
                pUDropDown.Font = YR.ERP.Shared.GetStyleLibrary.FontGrid;
                pUDropDown.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
                pUDropDown.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
                //pUDropDown.DisplayLayout.Bands[0].Columns[1].Width = pUDropDown.Width - 60;
                //pDropDown.DisplayLayout.ValueLists[0].ValueListItems[0].
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region WfSetAppearance : UltraGrid
        protected virtual void WfSetAppearance(Infragistics.Win.UltraWinGrid.UltraGrid pUGrid)
        {
            WfSetAppearance(pUGrid, 1);
        }

        protected virtual void WfSetAppearance(Infragistics.Win.UltraWinGrid.UltraGrid pUGrid, int piStyle)
        {
            //外觀調整
            pUGrid.DisplayLayout.Override.RowSelectorStyle = HeaderStyle.WindowsXPCommand;
            pUGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            pUGrid.DisplayLayout.Override.RowSelectorNumberStyle = Infragistics.Win.UltraWinGrid.RowSelectorNumberStyle.VisibleIndex;
            pUGrid.DisplayLayout.Override.RowSelectorWidth = 20;

            pUGrid.DisplayLayout.Override.HeaderAppearance.TextHAlign = HAlign.Left;
            pUGrid.DisplayLayout.Override.ActiveCellColumnHeaderAppearance.BackColor = Color.FromKnownColor(KnownColor.SkyBlue);   //暗藍色
            pUGrid.DisplayLayout.Override.ActiveCellRowSelectorAppearance.BackColor = Color.FromKnownColor(KnownColor.SkyBlue);   //暗藍色
            pUGrid.DisplayLayout.Override.CellPadding = 2;
            //pUGrid.DisplayLayout.Override.CardSpacing = 2;
            //pUGrid.DisplayLayout.Override.CellAppearance.TextVAlign = VAlign.Middle;
            pUGrid.DisplayLayout.Override.CellButtonAppearance.BackColor = Color.Empty;
            if (GetStyleLibrary.FontGrid != null)
                pUGrid.Font = GetStyleLibrary.FontGrid;

            //啟動 IDataErrorInfo 功能
            pUGrid.DisplayLayout.Override.SupportDataErrorInfo = SupportDataErrorInfo.RowsAndCells;
            pUGrid.DisplayLayout.Override.DataErrorCellAppearance.ForeColor = Color.Red;
            pUGrid.DisplayLayout.Override.DataErrorRowAppearance.BackColor = Color.LightYellow;
            pUGrid.DisplayLayout.Override.DataErrorRowSelectorAppearance.BackColor = Color.Red;

            //功能調整
            pUGrid.DisplayLayout.Override.AllowRowFiltering = DefaultableBoolean.False;
            pUGrid.DisplayLayout.Override.AllowGroupBy = DefaultableBoolean.False;
            pUGrid.DisplayLayout.UseFixedHeaders = false;
            pUGrid.DisplayLayout.GroupByBox.Hidden = true;
            pUGrid.Cursor = System.Windows.Forms.Cursors.Default;
            pUGrid.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
            pUGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            pUGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            pUGrid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            pUGrid.Dock = System.Windows.Forms.DockStyle.Fill;

            switch (piStyle)
            {
                case 1:
                    //主要樣式 header:淺綠背景 交替列:黃色
                    //框樣式
                    pUGrid.DisplayLayout.Override.BorderStyleCell = UIElementBorderStyle.Default;
                    pUGrid.DisplayLayout.Override.BorderStyleHeader = UIElementBorderStyle.Default;
                    pUGrid.DisplayLayout.Override.RowAppearance.BorderColor = ColorTranslator.FromHtml("#fffafa");

                    //交替列
                    pUGrid.DisplayLayout.Override.RowAlternateAppearance.BackColor = ColorTranslator.FromHtml("#E8E8D0");
                    //抬頭列
                    pUGrid.DisplayLayout.Override.HeaderAppearance.BackColor = Color.Empty;
                    //p_grid.DisplayLayout.Override.HeaderAppearance.BackColor = ColorTranslator.FromHtml("#D1E9E9");
                    //選取列
                    pUGrid.DisplayLayout.Override.RowSelectorAppearance.BackColor = Color.Empty;
                    //作用列

                    pUGrid.DisplayLayout.Override.ActiveRowAppearance.BackColor = ColorTranslator.FromHtml("#A9D0F5");//淺藍色
                    pUGrid.DisplayLayout.Override.ActiveRowAppearance.ForeColor = Color.Black;
                    //作用cell
                    pUGrid.DisplayLayout.Override.ActiveCellAppearance.BackColor = Color.Empty;
                    pUGrid.DisplayLayout.Override.ActiveCellAppearance.ForeColor = Color.Black;
                    //編輯cell
                    pUGrid.DisplayLayout.Override.EditCellAppearance.BackColor = Color.FromArgb(255, 224, 192);//橘色
                    pUGrid.DisplayLayout.Appearance.BackColor = Color.WhiteSmoke;
                    //p_grid.DisplayLayout.Appearance.BackColor = ColorTranslator.FromHtml("#fffaf0");      //之後以此為背景色
                    //p_grid.DisplayLayout.Appearance.BackColor2 = Color.PapayaWhip;
                    //p_grid.DisplayLayout.Appearance.BackGradientStyle = GradientStyle.ForwardDiagonal;
                    break;
                case 2: //主要樣式 header:淺綠背景 白底字 交替列:水藍
                    //交替列
                    pUGrid.DisplayLayout.Override.RowAlternateAppearance.BackColor = ColorTranslator.FromHtml("#FFDCB9");
                    //抬頭列
                    pUGrid.DisplayLayout.Override.HeaderAppearance.BackColor = ColorTranslator.FromHtml("#D1E9E9");
                    //選取列
                    pUGrid.DisplayLayout.Override.RowSelectorAppearance.BackColor = Color.Empty;
                    //作用列
                    pUGrid.DisplayLayout.Override.ActiveRowAppearance.BackColor = Color.Empty;

                    break;

                case 3:
                    //主要樣式 header:淺綠背景 白底字 交替列:水藍
                    //框樣式
                    pUGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
                    //抬頭列
                    pUGrid.DisplayLayout.Override.HeaderAppearance.BackColor = Color.LightSteelBlue;
                    //交替列
                    pUGrid.DisplayLayout.Override.RowAlternateAppearance.BackColor = ColorTranslator.FromHtml("#D2E9FF");
                    ////選取列
                    //p_grid.DisplayLayout.Override.RowSelectorAppearance.BackColor = Color.LightSteelBlue;
                    ////作用列
                    //p_grid.DisplayLayout.Override.ActiveRowAppearance.BackColor = Color.SteelBlue;

                    break;

                case 4:
                    //主要樣式 header:淺綠背景 交替列:黃色
                    //框樣式
                    pUGrid.DisplayLayout.Override.BorderStyleCell = UIElementBorderStyle.Default;
                    pUGrid.DisplayLayout.Override.BorderStyleHeader = UIElementBorderStyle.Default;
                    pUGrid.DisplayLayout.Override.RowAppearance.BorderColor = ColorTranslator.FromHtml("#fffafa");


                    //p_grid.DisplayLayout.Override.CellPadding = -1;
                    //p_grid.DisplayLayout.Override.RowSpacingAfter = -1;
                    //p_grid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
                    //交替列
                    pUGrid.DisplayLayout.Override.RowAlternateAppearance.BackColor = ColorTranslator.FromHtml("#E8E8D0");
                    //抬頭列
                    pUGrid.DisplayLayout.Override.HeaderAppearance.BackColor = ColorTranslator.FromHtml("#D1E9E9");
                    //選取列
                    pUGrid.DisplayLayout.Override.RowSelectorAppearance.BackColor = Color.Empty;
                    //作用列

                    pUGrid.DisplayLayout.Override.ActiveRowAppearance.BackColor = ColorTranslator.FromHtml("#A9D0F5");//淺藍色
                    //p_grid.DisplayLayout.Override.ActiveRowAppearance.BackColor = ColorTranslator.FromHtml("#1e90ff");
                    pUGrid.DisplayLayout.Override.ActiveRowAppearance.ForeColor = Color.Black;
                    //p_grid.DisplayLayout.Override.ActiveRowAppearance.ForeColor = Color.White;
                    //作用cell
                    pUGrid.DisplayLayout.Override.ActiveCellAppearance.BackColor = Color.Empty;
                    pUGrid.DisplayLayout.Override.ActiveCellAppearance.ForeColor = Color.Black;
                    //編輯cell
                    pUGrid.DisplayLayout.Override.EditCellAppearance.BackColor = Color.FromArgb(255, 224, 192);//橘色

                    ////背景色     
                    //p_grid.DisplayLayout.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 217, 230);
                    //p_grid.DisplayLayout.Appearance.BackColor2 = System.Drawing.Color.FromArgb(249, 240, 230);  //水藍色

                    pUGrid.DisplayLayout.Appearance.BackColor = Color.Empty;
                    //p_grid.DisplayLayout.Appearance.BackColor = ColorTranslator.FromHtml("#fffaf0");      //之後以此為背景色
                    //p_grid.DisplayLayout.Appearance.BackColor2 = Color.PapayaWhip;
                    //p_grid.DisplayLayout.Appearance.BackGradientStyle = GradientStyle.ForwardDiagonal;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region WfSetAppearance : UltraSplitter
        protected virtual void WfSetAppearance(Infragistics.Win.Misc.UltraSplitter pUSplitter)
        {
            pUSplitter.UseAppStyling = false;
            pUSplitter.Height = 8;
            //pSplitter.Appearance.BackColor = Color.LightGoldenrodYellow;
            pUSplitter.Appearance.BackColor = Color.LightYellow;

            pUSplitter.BorderStyle = UIElementBorderStyle.Rounded3;
            pUSplitter.ButtonStyle = UIElementButtonStyle.Button3D;
            pUSplitter.ButtonExtent = 40;
            pUSplitter.ButtonAppearance.BackColor = Color.Black;
        }
        #endregion

        #region WfSetControlEditNumeric 這裡是為了讓編輯模式時,讓小數點數字若為0時,不顯示 ex:123.000==>123
        protected void WfSetControlEditNumeric(UltraGridCell pUgc)
        {
            //NumberFormatInfo nfi;
            //CultureInfo culture;
            //culture = new CultureInfo("zh-TW");
            //nfi = NumberFormatInfo.GetInstance(culture);
            //pUgc.Value = Convert.ToSingle(pUgc.Value, nfi);
            pUgc.Value = Convert.ToDecimal(pUgc.Value);
        }

        protected void WfSetControlEditNumeric(UltraTextEditor pUte)
        {
            //NumberFormatInfo nfi;
            //CultureInfo culture;
            //culture = new CultureInfo("zh-TW");
            //nfi = NumberFormatInfo.GetInstance(culture);
            //pUte.Value = Convert.ToSingle(pUte.Value, nfi);
            pUte.Value = Convert.ToDecimal(pUte.Value);
        }
        #endregion

        #region UltraTextEditor_KeyUp
        protected virtual internal void UltraTextEditor_KeyUp(object sender, KeyEventArgs e)
        {
        }
        #endregion

        #region 相關控制項Validating事件 方法
        #region UltraTextEditor_Validating
        protected internal virtual void UltraTextEditor_Validating(object sender, CancelEventArgs e)
        {
        }
        #endregion

        #region UltraCombo_Validating
        protected internal virtual void UltraCombo_Validating(object sender, CancelEventArgs e)
        {
        }
        #endregion

        #region UcCheckBox_Validating
        protected internal virtual void UcCheckBox_Validating(object sender, CancelEventArgs e)
        {
        }
        #endregion

        #region UltraGrid_BeforeExitEditMode
        protected internal virtual void UltraGrid_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {
        }
        #endregion

        #region UcDatePicker 時間控制項
        protected internal virtual void UcDatePicker_Validating(object sender, CancelEventArgs e)
        {
        }
        #endregion

        #region UltraDateTimeEditor 時間控制項
        protected internal virtual void UltraDateTimeEditor_Validating(object sender, CancelEventArgs e)
        {
        }
        #endregion

        #region WfItemChkForceFocus/ChangeActiveDelegate 在itemcheck方法(validating)中,強制事件結束後變更focust
        protected void WfItemChkForceFocus(Control pControl)
        {
            this.BeginInvoke(new ChangeFocusEventHandler(changeFocus), pControl);
        }
        private delegate void ChangeFocusEventHandler(Control pControl);
        private void changeFocus(Control pControl)
        {
            pControl.Focus();
        }

        protected void WfItemChkForceFocus(UltraGrid pGrid, UltraGridCell pCell)
        {
            this.BeginInvoke(new ChangeActiveCellEventHandler(changeCellFocus), pGrid, pCell);
        }
        private delegate void ChangeActiveCellEventHandler(UltraGrid pGrid, UltraGridCell pCell);
        private void changeCellFocus(UltraGrid pGrid, UltraGridCell pCell)
        {
            try
            {
                pCell.Activate();
                pGrid.PerformAction(UltraGridAction.EnterEditMode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #endregion

        #region UltraTextEditor_EditorPickButtonClick
        protected virtual void UltraTextEditor_EditorPickButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
        }
        #endregion

        #region UltraCombo_KeyUp
        protected virtual internal void UltraCombo_KeyUp(object sender, KeyEventArgs e)
        {
        }
        #endregion

        #region UltraDateTimeEditor_KeyUp
        protected virtual internal void UltraDateTimeEditor_KeyUp(object sender, KeyEventArgs e)
        {

        }
        #endregion

        #region UcCheckBox_KeyUp
        protected virtual internal void UcCheckBox_KeyUp(object sender, KeyEventArgs e)
        {

        }
        #endregion

        #region Control_Enter(object sender, EventArgs e) 所有控制項進入(ENTER)事件(未包含GRID)
        protected internal virtual void Control_Enter(object sender, EventArgs e)
        {
        }
        #endregion

        #region WfDisplayMode 新增修改刪除後的readonly顯示
        protected virtual Boolean WfDisplayMode()
        {
            return true;
        }
        #endregion

        #region WfGetOldValue 進元件 及新增修改時取得該元件的初始值,並選取全部
        protected void WfGetOldValue(Control pControl)
        {
            string typeName;
            Control actContrl;
            try
            {
                typeName = pControl.GetType().ToString();
                this.OldValue = null;

                if (typeName == "Infragistics.Win.EmbeddableTextBoxWithUIPermissions")
                {
                    typeName = pControl.Parent.GetType().ToString();
                    actContrl = pControl.Parent;
                }
                else
                    actContrl = pControl;

                switch (typeName)
                {
                    case "Infragistics.Win.UltraWinGrid.UltraGrid":
                        if ((actContrl as UltraGrid).ActiveCell != null)
                        {
                            this.OldValue = (actContrl as UltraGrid).ActiveCell.Value;
                        }
                        break;
                    case "Infragistics.Win.UltraWinEditors.UltraTextEditor":
                        //20161125 Allen modify:因為有format的問題,所以改成取背後的值
                        //this.OldValue = (actContrl as UltraTextEditor).Value;
                        if ((actContrl as UltraTextEditor).DataBindings["Text"].IsBinding)
                        {
                            DataRowView dataRowView = ((actContrl as UltraTextEditor).DataBindings["Text"].DataSource as BindingSource).Current as DataRowView;
                            this.OldValue = dataRowView[actContrl.Tag.ToString()];
                        }
                        else
                            this.OldValue = (actContrl as UltraTextEditor).Value;

                        (actContrl as UltraTextEditor).SelectAll();
                        break;
                    case "YR.Util.Controls.UcCheckBox":
                        this.OldValue = (actContrl as UcCheckBox).CheckValue;
                        break;
                    case "Infragistics.Win.UltraWinGrid.UltraCombo":
                        this.OldValue = (actContrl as UltraCombo).Value;
                        break;
                    //case "YR.Util.Controls.UcDatePicker":
                    //    this.OldValue = (actContrl as UcDatePicker).Value;
                    //    break;
                    case "Infragistics.Win.UltraWinEditors.UltraDateTimeEditor":
                        this.OldValue = (actContrl as UltraDateTimeEditor).Value;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfShowPickBase 呼叫 公用挑選視窗
        public MessageInfo WfShowPickUtility(string pPickNo, MessageInfo pMessageInfo)
        {
            try
            {
                return WfShowPickUtility(pPickNo, pMessageInfo, FormStartPosition.WindowsDefaultLocation);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MessageInfo WfShowPickUtility(string pPickNo, MessageInfo pMessageInfo, FormStartPosition pStartPosition)
        {
            try
            {
                using (YR.ERP.Base.Forms.FrmPickBase frmPick = new YR.ERP.Base.Forms.FrmPickBase(pPickNo, LoginInfo))
                {
                    frmPick.MsgInfoReturned = pMessageInfo;
                    frmPick.StartPosition = pStartPosition;
                    frmPick.ShowDialog(this);

                    return frmPick.MsgInfoReturned;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfShowForm 開啟程式、報表
        #region WfShowForm(string pProgramId)
        public bool WfShowForm(string pProgramId)
        {
            DataRow drAdo;
            try
            {
                drAdo = BoSecurity.OfGetAdoDr(pProgramId);
                if (drAdo == null)
                {
                    WfShowErrorMsg("無此程式代號");
                    return false;
                }

                return WfShowForm(drAdo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfShowForm(DataRow pdrAdo)
        public bool WfShowForm(ado_tb pAdoModel)
        {
            try
            {
                return WfShowForm(pAdoModel, this.IsMdiChild, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfShowForm(DataRow pdrAdo)
        public bool WfShowForm(DataRow pdrAdo)
        {
            try
            {
                return WfShowForm(pdrAdo, this.IsMdiChild);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfShowForm(DataRow pDrAdo, bool pIsMdiChild)
        public bool WfShowForm(DataRow pDrAdo, bool pIsMdiChild)
        {
            try
            {
                return WfShowForm(pDrAdo, pIsMdiChild, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfShowForm(string pProgramId, bool pIsMdiChild, object[] pConstructorArray)
        /// <summary>
        /// 開啟視窗,並傳入建構子參數
        /// </summary>
        /// <param name="pProgramId">formId</param>
        /// <param name="pIsMdiChild">true.MDI子視窗 false.Single視窗</param>
        /// <param name="pConstructorArray">建構子參數陣列</param>
        /// <returns></returns>
        public bool WfShowForm(string pProgramId, bool pIsMdiChild, object[] pConstructorArray)
        {
            DataRow drAdo = null;
            try
            {
                drAdo = BoSecurity.OfGetAdoDr(pProgramId);
                if (drAdo == null)
                {
                    WfShowErrorMsg("無此程式代號");
                    return false;
                }

                return WfShowForm(drAdo, pIsMdiChild, pConstructorArray);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfShowForm(DataRow pDrAdo, bool pIsMdiChild, object[] pConstructorArray)
        /// <summary>
        /// 開啟視窗,並傳入建構子參數
        /// </summary>
        /// <param name="pDrAdo">ado_tb DataRow </param>
        /// <param name="pIsMdiChild">true.MDI子視窗 false.Single視窗</param>
        /// <param name="pConstructorArray">建構子參數陣列</param>
        /// <returns></returns>
        public bool WfShowForm(DataRow pDrAdo, bool pIsMdiChild, object[] pConstructorArray)
        {
            try
            {
                var adoModel = pDrAdo.ToItem<ado_tb>();

                return WfShowForm(adoModel, pIsMdiChild, pConstructorArray);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfShowForm(DataRow pdr_ado, bool pIsMdiChild, object[] pConstructorArray)
        /// <summary>
        /// 開啟視窗,並傳入建構子參數
        /// </summary>
        /// <param name="pdr_ado">ado_tb DataRow </param>
        /// <param name="pIsMdiChild">true.MDI子視窗 false.Single視窗</param>
        /// <param name="pConstructorArray">建構子參數陣列</param>
        /// <returns></returns>
        public bool WfShowForm(ado_tb pAdoModle, bool pIsMdiChild, object[] pConstructorArray)
        {
            YR.ERP.Base.Forms.FrmBase frmActive = null;
            System.Reflection.Assembly assembly;

            try
            {
                assembly = System.Reflection.Assembly.LoadFile(Path.Combine(Application.StartupPath, pAdoModle.ado03));
                var type = assembly.GetType(pAdoModle.ado04);
                if (pConstructorArray == null)
                    frmActive = Activator.CreateInstance(type) as YR.ERP.Base.Forms.FrmBase;
                else
                    frmActive = Activator.CreateInstance(type, pConstructorArray) as YR.ERP.Base.Forms.FrmBase;

                if (frmActive == null)
                {
                    WfShowErrorMsg(string.Format("視窗初始化失敗,無,{0}", pAdoModle.ado04));
                    return false;
                }

                //frmActive.LoginInfo = this.LoginInfo;     //改為新增實體
                frmActive.LoginInfo = new UserInfo();
                frmActive.LoginInfo.CompNameA = this.LoginInfo.CompNameA;
                frmActive.LoginInfo.CompNo = this.LoginInfo.CompNo;
                frmActive.LoginInfo.DeptName = this.LoginInfo.DeptName;
                frmActive.LoginInfo.DeptNo = this.LoginInfo.DeptNo;
                frmActive.LoginInfo.GroupLevel = this.LoginInfo.GroupLevel;
                frmActive.LoginInfo.GroupNo = this.LoginInfo.GroupNo;
                frmActive.LoginInfo.UserName = this.LoginInfo.UserName;
                frmActive.LoginInfo.UserNo = this.LoginInfo.UserNo;
                frmActive.LoginInfo.UserRole = this.LoginInfo.UserRole;

                if (pIsMdiChild)
                {
                    frmActive.MdiParent = this.MdiParent;
                    frmActive.WindowState = FormWindowState.Maximized;
                    frmActive.FormBorderStyle = FormBorderStyle.None;
                    frmActive.Dock = DockStyle.None;
                    frmActive.BringToFront();
                    frmActive.Show();
                }
                else
                {
                    frmActive.ControlBox = true;
                    frmActive.BringToFront();
                    frmActive.ShowDialog(this);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #endregion

        #region WfOpenFrmNavigateor 開啟active window
        protected virtual void WfOpenFrmNavigator()
        {
            try
            {
                YR.ERP.Base.Forms.FrmNavigator frmNav = new YR.ERP.Base.Forms.FrmNavigator();
                frmNav.FormBorderStyle = FormBorderStyle.None;
                frmNav.WindowState = FormWindowState.Maximized;
                frmNav.ShowInTaskbar = false;
                //frmNav.TopMost = true;
                //frmNav.Show(this);
                frmNav.Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WfGetUgridDatarow(UltraGridRow) : 取得傳入  DataGridRow Binding 的 DataRow
        /// <summary>
        /// 取得傳入  DataGridRow Binding 的 DataRow
        /// </summary>
        /// <param name="ugr"></param>
        /// <returns></returns>
        protected DataRow WfGetUgridDatarow(Infragistics.Win.UltraWinGrid.UltraGridRow ugr)
        {
            try
            {
                if (ugr == null) return null;

                if (ugr.ListObject == null) return null;


                if (ugr.ListObject.GetType() == typeof(System.Data.DataRowView))
                {
                    System.Data.DataRowView drv = (System.Data.DataRowView)ugr.ListObject;
                    return drv.Row;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region Enum 列舉型態
        public enum YREditType
        {
            NA = 0,
            新增 = 1,
            //複製 = 2,
            修改 = 3,
            刪除 = 4, //僅用來辨識,不會有此類流程
            查詢 = 5
        }
        #endregion
    }
}
